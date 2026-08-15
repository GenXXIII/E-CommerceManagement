import { useMutation, useQueries, useQuery, useQueryClient } from "@tanstack/react-query";
import { ArrowLeft, ArrowRight, Check, CheckCircle2, CreditCard, MapPin, Minus, PackageCheck, Plus, Trash2, XCircle } from "lucide-react";
import { useState } from "react";
import { Link, useNavigate, useParams, useSearchParams } from "react-router-dom";
import { Button, Card, EmptyState, ErrorState, PageHeader, Spinner } from "../components/ui";
import { ProductVisual } from "../components/ProductCard";
import { useAuth } from "../core/auth/AuthProvider";
import { formatCurrency, paymentStatusLabel } from "../core/format";
import type { CartItem, Product } from "../core/types";
import { catalogApi, catalogKeys } from "../features/catalog/api";
import { commerceApi, commerceKeys } from "../features/commerce/api";

function useCustomerId() {
  const { session } = useAuth();
  if (!session.customerProfileId) throw new Error("Your customer profile is unavailable.");
  return session.customerProfileId;
}

function CartLine({ item, product }: { item: CartItem; product?: Product }) {
  const customerId = useCustomerId();
  const queryClient = useQueryClient();
  const update = useMutation({ mutationFn: (quantity: number) => commerceApi.updateCart(customerId, item.productId, quantity), onSuccess: cart => queryClient.setQueryData(commerceKeys.cart(customerId), cart) });
  const remove = useMutation({ mutationFn: () => commerceApi.removeCart(customerId, item.productId), onSuccess: cart => queryClient.setQueryData(commerceKeys.cart(customerId), cart) });
  return <div className="cart-line"><ProductVisual compact name={product?.name ?? "Product"}/><div className="cart-line__info"><Link to={`/products/${item.productId}`}>{product?.name ?? `Product ${item.productId.slice(0,8)}`}</Link><span>{formatCurrency(item.unitPrice)} each</span>{(update.error || remove.error) && <small className="inline-error">{(update.error ?? remove.error)?.message}</small>}</div><div className="quantity-control"><button aria-label="Decrease quantity" disabled={item.quantity <= 1 || update.isPending} onClick={() => update.mutate(item.quantity - 1)}><Minus size={14}/></button><span>{item.quantity}</span><button aria-label="Increase quantity" disabled={update.isPending || item.quantity >= (product?.quantity ?? Number.MAX_SAFE_INTEGER)} onClick={() => update.mutate(item.quantity + 1)}><Plus size={14}/></button></div><strong>{formatCurrency(item.totalPrice)}</strong><button className="icon-button icon-button--danger" aria-label="Remove item" disabled={remove.isPending} onClick={() => { if (window.confirm("Remove this item from your cart?")) remove.mutate(); }}><Trash2 size={17}/></button></div>;
}

export function CartPage() {
  const customerId = useCustomerId();
  const cart = useQuery({ queryKey: commerceKeys.cart(customerId), queryFn: () => commerceApi.cart(customerId) });
  const productQueries = useQueries({ queries: (cart.data?.items ?? []).map(item => ({ queryKey: catalogKeys.product(item.productId), queryFn: () => catalogApi.product(item.productId) })) });
  if (cart.isLoading) return <div className="container page-pad"><Spinner label="Loading your cart"/></div>;
  if (cart.isError) return <div className="container page-pad"><ErrorState error={cart.error} onRetry={() => cart.refetch()}/></div>;
  const items = cart.data?.items ?? [];
  return <div className="container commerce-page"><PageHeader eyebrow="Your basket" title="Shopping cart" description="Quantities and totals are saved to the backend as you make changes."/><div className="commerce-grid"><Card className="cart-list">{items.length ? items.map((item,index)=><CartLine key={item.productId} item={item} product={productQueries[index]?.data}/>) : <EmptyState title="Your cart is ready for a find" description="Add something from the live catalog and it will appear here." action={<Link className="button button--primary button--md" to="/products">Browse products</Link>}/>}</Card><Card className="order-summary"><span className="eyebrow">Order summary</span><h2>{items.length} {items.length===1?"item":"items"}</h2><div><span>Merchandise</span><strong>{formatCurrency(cart.data?.totalAmount ?? 0)}</strong></div><div><span>Shipping</span><span>Calculated at checkout</span></div><hr/><div className="summary-total"><span>Total</span><strong>{formatCurrency(cart.data?.totalAmount ?? 0)}</strong></div><Link className={`button button--primary button--lg ${!items.length ? "is-disabled":""}`} aria-disabled={!items.length} to={items.length?"/checkout":"#"}>Continue to checkout <ArrowRight size={17}/></Link><small>Final totals are recalculated by the backend.</small></Card></div></div>;
}

export function CheckoutPage() {
  const customerId = useCustomerId();
  const navigate = useNavigate();
  const cart = useQuery({ queryKey: commerceKeys.cart(customerId), queryFn: () => commerceApi.cart(customerId) });
  const addresses = useQuery({ queryKey: commerceKeys.addresses(customerId), queryFn: () => commerceApi.addresses(customerId) });
  const [addressId, setAddressId] = useState("");
  const [note, setNote] = useState("");
  const selectedAddress = addressId || addresses.data?.find(a => a.isDefault)?.id || addresses.data?.[0]?.id || "";
  const placeOrder = useMutation({
    mutationFn: async () => {
      if (!cart.data || !selectedAddress) throw new Error("Select a delivery address first.");
      const orderId = await commerceApi.createOrder(customerId, selectedAddress, note, cart.data.items.map(item => ({ productId: item.productId, quantity: item.quantity })));
      const order = await commerceApi.order(orderId);
      const paymentId = await commerceApi.createPayment(orderId, order.totalAmount);
      return { orderId, paymentId };
    },
    onSuccess: ({ orderId, paymentId }) => navigate(`/payment/${paymentId}?orderId=${orderId}`),
  });
  if (cart.isLoading || addresses.isLoading) return <div className="container page-pad"><Spinner label="Preparing checkout"/></div>;
  if (cart.isError || addresses.isError) return <div className="container page-pad"><ErrorState error={cart.error ?? addresses.error}/></div>;
  if (!cart.data?.items.length) return <div className="container page-pad"><EmptyState title="Your cart is empty" description="Add at least one item before starting checkout." action={<Link className="button button--primary button--md" to="/products">Browse products</Link>}/></div>;
  return <div className="container checkout-page"><Link className="back-link" to="/cart"><ArrowLeft/> Back to cart</Link><PageHeader eyebrow="Secure development checkout" title="Review and place your order" description="The backend will verify inventory, prices, and totals before creating the order."/><div className="commerce-grid"><div className="checkout-stack"><Card><div className="card-heading"><div><span>1</span><h2>Delivery address</h2></div><Link to="/account/addresses">Manage addresses</Link></div>{addresses.data?.length ? <div className="address-options">{addresses.data.map(address=><label className={selectedAddress===address.id?"address-option is-selected":"address-option"} key={address.id}><input type="radio" name="address" checked={selectedAddress===address.id} onChange={()=>setAddressId(address.id)}/><MapPin/><span><strong>{address.receiverName}</strong><small>{address.street}, {address.commune}, {address.district}, {address.province}</small><small>{address.phone}</small></span>{address.isDefault&&<em>Default</em>}</label>)}</div> : <EmptyState title="Add a delivery address" description="Checkout needs a real address stored by the API." action={<Link className="button button--primary button--md" to="/account/addresses?return=/checkout">Add address</Link>}/>}</Card><Card><div className="card-heading"><div><span>2</span><h2>Order note</h2></div></div><textarea className="input textarea" value={note} onChange={e=>setNote(e.target.value)} placeholder="Optional delivery note" maxLength={500}/></Card><Card><div className="card-heading"><div><span>3</span><h2>Development payment</h2></div></div><div className="payment-method"><CreditCard/><div><strong>Backend test payment</strong><p>No card data is collected. The server persists the payment result.</p></div><CheckCircle2/></div></Card></div><Card className="order-summary order-summary--sticky"><span className="eyebrow">Review</span><h2>Your order</h2>{cart.data.items.map(item=><div key={item.productId}><span>{item.quantity} × Product {item.productId.slice(0,6)}</span><strong>{formatCurrency(item.totalPrice)}</strong></div>)}<hr/><div className="summary-total"><span>API cart total</span><strong>{formatCurrency(cart.data.totalAmount)}</strong></div>{placeOrder.error&&<div className="form-alert">{placeOrder.error.message}</div>}<Button size="lg" disabled={!selectedAddress||placeOrder.isPending} onClick={()=>placeOrder.mutate()}>{placeOrder.isPending?"Creating order…":"Place order & continue"}</Button><small>Submitting creates a persisted order and pending payment.</small></Card></div></div>;
}

export function PaymentPage() {
  const { paymentId = "" } = useParams();
  const [params] = useSearchParams();
  const navigate = useNavigate();
  const queryClient = useQueryClient();
  const customerId = useCustomerId();
  const payment = useQuery({ queryKey: commerceKeys.payment(paymentId), queryFn: () => commerceApi.payment(paymentId) });
  const process = useMutation({ mutationFn: (success: boolean) => commerceApi.processPayment(paymentId, success), onSuccess: data => { queryClient.setQueryData(commerceKeys.payment(paymentId), data); if (data.status === 2) { queryClient.invalidateQueries({ queryKey: commerceKeys.cart(customerId) }); queryClient.invalidateQueries({ queryKey: commerceKeys.orders(customerId) }); } navigate(`/payment/${paymentId}/result?orderId=${params.get("orderId") ?? data.orderId}`, { replace: true }); } });
  if (payment.isLoading) return <div className="container page-pad"><Spinner label="Loading payment"/></div>;
  if (payment.isError) return <div className="container page-pad"><ErrorState error={payment.error} onRetry={()=>payment.refetch()}/></div>;
  return <div className="payment-page"><Card className="payment-card"><span className="payment-icon"><CreditCard/></span><span className="eyebrow">Development test payment</span><h1>Complete your test order</h1><p>This screen calls the backend payment processor. The browser never marks a payment as paid on its own.</p><div className="payment-total"><span>Amount due</span><strong>{formatCurrency(payment.data?.amount ?? 0)}</strong></div><div className="payment-detail"><span>Payment ID</span><code>{paymentId}</code></div>{process.error&&<div className="form-alert">{process.error.message}</div>}<Button size="lg" onClick={()=>process.mutate(true)} disabled={process.isPending}>{process.isPending?"Processing…":"Process successful test"}</Button><Button size="lg" variant="secondary" onClick={()=>process.mutate(false)} disabled={process.isPending}>Simulate declined test</Button><small>For local development only. No real payment credentials are used.</small></Card></div>;
}

export function PaymentResultPage() {
  const { paymentId = "" } = useParams();
  const [params] = useSearchParams();
  const payment = useQuery({ queryKey: commerceKeys.payment(paymentId), queryFn: () => commerceApi.payment(paymentId) });
  if (payment.isLoading) return <div className="container page-pad"><Spinner label="Confirming persisted result"/></div>;
  if (payment.isError) return <div className="container page-pad"><ErrorState error={payment.error} onRetry={()=>payment.refetch()}/></div>;
  const paid = payment.data?.status === 2;
  return <div className="payment-page"><Card className="payment-card payment-result"><span className={`payment-icon ${paid?"payment-icon--success":"payment-icon--danger"}`}>{paid?<Check/>:<XCircle/>}</span><span className="eyebrow">Persisted backend result</span><h1>{paid?"Payment complete":"Payment not completed"}</h1><p>{paid?"Your test payment was persisted successfully. You can now review the order details.":"The backend recorded a failed payment. Your order remains available for recovery."}</p><div className="payment-total"><span>Status</span><strong>{paymentStatusLabel(payment.data?.status ?? 0)}</strong></div><div className="result-actions">{params.get("orderId")&&<Link className="button button--primary button--lg" to={`/account/orders/${params.get("orderId")}`}><PackageCheck/> View order</Link>}<Link className="button button--secondary button--lg" to="/products">Continue shopping</Link></div></Card></div>;
}
