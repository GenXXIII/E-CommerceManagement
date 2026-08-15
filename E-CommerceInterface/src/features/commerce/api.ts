import { apiRequest } from "../../core/api/apiClient";
import type { Address, Order, Payment, Refund, ShoppingCart, Wishlist } from "../../core/types";

export const commerceKeys = {
  cart: (customerId: string) => ["cart", customerId] as const,
  wishlist: (customerId: string) => ["wishlist", customerId] as const,
  addresses: (customerId: string) => ["addresses", customerId] as const,
  orders: (customerId: string) => ["orders", customerId] as const,
  order: (id: string) => ["order", id] as const,
  payment: (id: string) => ["payment", id] as const,
  refunds: (customerId: string) => ["refunds", customerId] as const,
};

export const commerceApi = {
  cart: (customerId: string) => apiRequest<ShoppingCart>(`/shoppingcarts/customer/${customerId}`),
  addCart: (customerProfileId: string, productId: string, quantity: number) => apiRequest<ShoppingCart>("/shoppingcarts/add", { method: "POST", body: JSON.stringify({ customerProfileId, productId, quantity }) }),
  updateCart: (customerProfileId: string, productId: string, quantity: number) => apiRequest<ShoppingCart>("/shoppingcarts/update", { method: "PUT", body: JSON.stringify({ customerProfileId, productId, quantity }) }),
  removeCart: (customerProfileId: string, productId: string) => apiRequest<ShoppingCart>("/shoppingcarts/remove", { method: "POST", body: JSON.stringify({ customerProfileId, productId }) }),
  wishlist: (customerId: string) => apiRequest<Wishlist>(`/wishlists/customer/${customerId}`),
  addWishlist: (customerProfileId: string, productId: string) => apiRequest<Wishlist>("/wishlists/add", { method: "POST", body: JSON.stringify({ customerProfileId, productId }) }),
  removeWishlist: (customerProfileId: string, productId: string) => apiRequest<Wishlist>("/wishlists/remove", { method: "POST", body: JSON.stringify({ customerProfileId, productId }) }),
  clearWishlist: (customerProfileId: string) => apiRequest<Wishlist>("/wishlists/clear", { method: "POST", body: JSON.stringify({ customerProfileId }) }),
  addresses: (customerId: string) => apiRequest<Address[]>(`/addresses/customer/${customerId}`),
  createAddress: (payload: Omit<Address, "id" | "createdAt">) => apiRequest<string>("/addresses", { method: "POST", body: JSON.stringify(payload) }),
  updateAddress: (id: string, payload: Partial<Address>) => apiRequest<void>(`/addresses/${id}`, { method: "PUT", body: JSON.stringify(payload) }),
  deleteAddress: (id: string) => apiRequest<void>(`/addresses/${id}`, { method: "DELETE" }),
  createOrder: (customerProfileId: string, addressId: string, note: string, items: { productId: string; quantity: number }[]) => apiRequest<string>("/orders", { method: "POST", body: JSON.stringify({ customerProfileId, addressId, note, items }) }),
  orders: (customerId: string) => apiRequest<Order[]>(`/orders/customer/${customerId}`),
  order: (id: string) => apiRequest<Order>(`/orders/${id}`),
  createPayment: (orderId: string, amount: number) => apiRequest<string>("/payments", { method: "POST", body: JSON.stringify({ orderId, amount, paymentMethod: "Development test payment" }) }),
  payment: (id: string) => apiRequest<Payment>(`/payments/${id}`),
  processPayment: (id: string, simulateSuccess: boolean) => apiRequest<Payment>(`/payments/${id}/process`, { method: "PATCH", body: JSON.stringify({ paymentId: id, simulateSuccess }) }),
  refunds: (customerId: string) => apiRequest<Refund[]>(`/refunds/customer/${customerId}`),
};
