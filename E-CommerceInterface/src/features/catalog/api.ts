import { apiRequest, qs } from "../../core/api/apiClient";
import type { Category, Paginated, Product, ProductReview, StorefrontReview } from "../../core/types";

export const catalogKeys = {
  categories: ["categories"] as const,
  products: (params: object) => ["products", params] as const,
  product: (id: string) => ["products", id] as const,
  reviews: (id: string) => ["product-reviews", id] as const,
  customerReview: (productId: string, customerId: string) => ["customer-review", productId, customerId] as const,
  storefrontReviews: ["storefront-reviews"] as const,
};

export const catalogApi = {
  categories: () => apiRequest<Category[]>("/categories"),
  category: (id: string) => apiRequest<Category>(`/categories/${id}`),
  products: (params: { keyword?: string; categoryId?: string; page?: number; pageSize?: number; includeHidden?: boolean; featuredOnly?: boolean }) =>
    apiRequest<Paginated<Product>>(`/products/search${qs(params)}`),
  product: (id: string) => apiRequest<Product>(`/products/${id}`),
  reviews: (id: string) => apiRequest<ProductReview[]>(`/productreviews/product/${id}`),
  customerReview: (productId: string, customerId: string) => apiRequest<ProductReview | null>(`/productreviews/product/${productId}/customer/${customerId}`),
  storefrontReviews: (limit = 6) => apiRequest<StorefrontReview[]>(`/productreviews/storefront?limit=${limit}`),
  createReview: (payload: Pick<ProductReview, "customerProfileId" | "productId" | "orderId" | "rating" | "comment">) => apiRequest<string>("/productreviews", { method: "POST", body: JSON.stringify(payload) }),
  deleteReview: (reviewId: string, customerId: string) => apiRequest<void>(`/productreviews/${reviewId}/customer/${customerId}`, { method: "DELETE" }),
};
