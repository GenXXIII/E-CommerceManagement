import { Heart, Monitor, ShoppingBag, Star } from "lucide-react";
import { Link, useLocation, useNavigate } from "react-router-dom";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import type { Product } from "../core/types";
import { formatCurrency } from "../core/format";
import { useAuth } from "../core/auth/AuthProvider";
import { commerceApi, commerceKeys } from "../features/commerce/api";
import { Button } from "./ui";
import { resolveApiAsset } from "../core/api/apiClient";
import { catalogApi, catalogKeys } from "../features/catalog/api";
import { useState } from "react";

export function ProductVisual({
  name,
  compact = false,
  imageUrl,
}: {
  name: string;
  compact?: boolean;
  imageUrl?: string;
}) {
  const tone = (name.charCodeAt(0) || 0) % 6;
  const source = resolveApiAsset(imageUrl);
  return (
    <div
      className={`product-visual product-visual--tone-${tone}${compact ? " product-visual--compact" : ""}`}
    >
      {source ? (
        <img src={source} alt={name} />
      ) : (
        <>
          <span className="product-visual__blob" />
          <Monitor aria-hidden="true" />
          <span className="product-visual__letter">
            {name.slice(0, 1).toUpperCase()}
          </span>
        </>
      )}
    </div>
  );
}

export function ProductCard({ product }: { product: Product }) {
  const { session } = useAuth();
  const navigate = useNavigate();
  const location = useLocation();
  const queryClient = useQueryClient();
  const [adminActionMessage, setAdminActionMessage] = useState("");
  const customerId = session.customerProfileId ?? "";
  const wishlist = useQuery({
    queryKey: commerceKeys.wishlist(customerId),
    queryFn: () => commerceApi.wishlist(customerId),
    enabled: session.role === "customer" && Boolean(customerId),
  });
  const reviews = useQuery({
    queryKey: catalogKeys.reviews(product.id),
    queryFn: () => catalogApi.reviews(product.id),
  });
  const reviewCount = reviews.data?.length ?? 0;
  const averageRating = reviewCount
    ? (reviews.data?.reduce((total, review) => total + review.rating, 0) ?? 0) / reviewCount
    : 0;
  const isSaved = Boolean(
    wishlist.data?.items.some((item) => item.productId === product.id),
  );
  const addCart = useMutation({
    mutationFn: () => commerceApi.addCart(customerId, product.id, 1),
    onSuccess: (cart) =>
      queryClient.setQueryData(commerceKeys.cart(customerId), cart),
  });
  const toggleWishlist = useMutation({
    mutationFn: () =>
      isSaved
        ? commerceApi.removeWishlist(customerId, product.id)
        : commerceApi.addWishlist(customerId, product.id),
    onSuccess: (updatedWishlist) =>
      queryClient.setQueryData(
        commerceKeys.wishlist(customerId),
        updatedWishlist,
      ),
  });
  const requireLogin = () =>
    navigate(
      `/login?redirect=${encodeURIComponent(location.pathname + location.search)}`,
    );
  const runCustomerAction = (action: () => void) => {
    if (session.role === "admin") {
      setAdminActionMessage(
        "You are signed in as an administrator. Switch to a customer account to buy or save items.",
      );
      return;
    }
    if (session.isAuthenticated) action();
    else requireLogin();
  };

  return (
    <article className="product-card">
      <Link className="product-card__main" to={`/products/${product.id}`}>
        <ProductVisual name={product.name} imageUrl={product.imageUrls?.[0]} />
        <div className="product-card__copy">
          <span className="product-card__category">Explore this find</span>
          <h3>{product.name}</h3>
          <span className="product-card__reviews">
            <Star size={14} fill={reviewCount ? "currentColor" : "none"} />
            {reviewCount ? `${averageRating.toFixed(1)} (${reviewCount})` : "No reviews"}
          </span>
          <p className="product-price">{formatCurrency(product.price)}</p>
        </div>
      </Link>
      <div className="product-card__actions">
        <button
          className={`icon-button save-toggle${isSaved ? " is-saved" : ""}`}
          aria-label={
            isSaved
              ? `Remove ${product.name} from saved items`
              : `Save ${product.name}`
          }
          title={isSaved ? "Remove from saved items" : "Save item"}
          onClick={() => runCustomerAction(() => toggleWishlist.mutate())}
          disabled={
            toggleWishlist.isPending ||
            (session.role === "customer" && wishlist.isLoading)
          }
        >
          <Heart size={18} fill={isSaved ? "currentColor" : "none"} />
        </button>
        <Button
          size="sm"
          onClick={() => runCustomerAction(() => addCart.mutate())}
          disabled={
            (product.quantity <= 0 && session.role !== "admin") ||
            addCart.isPending
          }
        >
          <ShoppingBag size={16} /> {addCart.isPending ? "Adding" : "Add"}
        </Button>
      </div>
      {(addCart.error || toggleWishlist.error) && (
        <p className="inline-error">
          {(addCart.error ?? toggleWishlist.error)?.message}
        </p>
      )}
      {adminActionMessage && (
        <p className="admin-shop-notice" role="status">
          {adminActionMessage}
        </p>
      )}
    </article>
  );
}
