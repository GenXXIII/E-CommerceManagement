import { apiRequest } from "../../core/api/apiClient";
import type { Category, CustomerProfile, InventoryTransaction, Order, Payment, Product, ProductReview, Refund, SalesStats } from "../../core/types";

export const adminKeys = {
  stats: (range: SalesStatsRange) => ["admin", "stats", range] as const,
  inventory: (id: string) => ["admin", "inventory", id] as const,
  orders: ["admin", "orders"] as const,
  payments: ["admin", "payments"] as const,
  refunds: ["admin", "refunds"] as const,
  customers: ["admin", "customers"] as const,
  reviews: ["admin", "reviews"] as const,
  categories: ["admin", "categories"] as const,
};

export type SalesStatsRange = "day" | "month" | "year" | "overall";

export const adminApi = {
  stats: (range: SalesStatsRange, refresh = false) =>
    apiRequest<SalesStats>(`/salesstats?range=${range}&refresh=${refresh}`),
  orders: () => apiRequest<Order[]>("/orders"),
  payments: () => apiRequest<Payment[]>("/payments"),
  refunds: () => apiRequest<Refund[]>("/refunds"),
  approveRefund: (id: string) => apiRequest<Refund>(`/refunds/${id}/approve`, { method: "PATCH" }),
  customers: () => apiRequest<CustomerProfile[]>("/customerprofiles"),
  reviews: () => apiRequest<ProductReview[]>("/productreviews"),
  categories: () => apiRequest<Category[]>("/categories?includeHidden=true"),
  setReviewVisibility: (id: string, visible: boolean) => apiRequest<ProductReview>(`/productreviews/${id}/visibility`, { method: "PATCH", body: JSON.stringify({ reviewId: id, visible }) }),
  createCategory: (payload: Pick<Category, "name" | "description">) => apiRequest<string>("/categories", { method: "POST", body: JSON.stringify(payload) }),
  uploadCategoryImage: (id: string, image: File) => {
    const body = new FormData();
    body.append("image", image);
    return apiRequest<{ imageUrl: string }>(`/categories/${id}/image`, { method: "POST", body });
  },
  updateCategory: (id: string, payload: Partial<Category>) => apiRequest<void>(`/categories/${id}`, { method: "PUT", body: JSON.stringify(payload) }),
  setCategoryVisibility: (id: string, visible: boolean) => apiRequest<void>(`/categories/${id}/visibility`, { method: "PATCH", body: JSON.stringify({ visible }) }),
  deleteCategory: (id: string) => apiRequest<void>(`/categories/${id}`, { method: "DELETE" }),
  createProduct: (payload: Pick<Product, "categoryId" | "name" | "description" | "price" | "quantity">) => apiRequest<string>("/products", { method: "POST", body: JSON.stringify(payload) }),
  uploadProductImage: (id: string, image: File) => {
    const body = new FormData();
    body.append("image", image);
    return apiRequest<{ imageUrl: string }>(`/products/${id}/image`, { method: "POST", body });
  },
  updateProduct: (id: string, payload: Partial<Product>) => apiRequest<void>(`/products/${id}`, { method: "PUT", body: JSON.stringify(payload) }),
  activateProduct: (id: string) => apiRequest<void>(`/products/${id}/activate`, { method: "PATCH" }),
  deactivateProduct: (id: string) => apiRequest<void>(`/products/${id}/deactivate`, { method: "PATCH" }),
  setFreshTechVisibility: (id: string, visible: boolean) => apiRequest<void>(`/products/${id}/fresh-tech-visibility`, { method: "PATCH", body: JSON.stringify({ visible }) }),
  deleteProduct: (id: string) => apiRequest<void>(`/products/${id}`, { method: "DELETE" }),
  inventory: (productId: string) => apiRequest<InventoryTransaction[]>(`/inventorytransactions/product/${productId}`),
  createInventory: (productId: string, type: number, quantity: number, note?: string) => apiRequest<string>("/inventorytransactions", { method: "POST", body: JSON.stringify({ productId, type, quantity, note }) }),
};
