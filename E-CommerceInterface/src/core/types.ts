export type DemoRole = "customer" | "admin";

export interface AuthSession {
  isAuthenticated: boolean;
  username: string | null;
  displayName: string | null;
  role: DemoRole | null;
  customerProfileId: string | null;
}

export interface Category {
  id: string;
  name: string;
  description?: string | null;
  imageUrl?: string | null;
  isActive: boolean;
  createdAt: string;
  updatedAt?: string | null;
}

export interface Product {
  id: string;
  categoryId: string;
  name: string;
  description: string;
  price: number;
  quantity: number;
  status: number;
  isFeatured: boolean;
  imageUrls: string[];
  createdAt: string;
  updatedAt?: string | null;
}

export interface Paginated<T> {
  items: T[];
  page: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
}

export interface CartItem {
  id: string;
  productId: string;
  quantity: number;
  unitPrice: number;
  totalPrice: number;
}

export interface ShoppingCart {
  id: string;
  customerProfileId: string;
  items: CartItem[];
  totalAmount: number;
  createdAt: string;
  updatedAt?: string | null;
}

export interface WishlistItem { id: string; productId: string; product: Product }
export interface Wishlist { id: string; customerProfileId: string; items: WishlistItem[] }

export interface CustomerProfile {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
  phone: string;
  isActive: boolean;
  createdAt: string;
  updatedAt?: string | null;
}

export interface Address {
  id: string;
  customerProfileId: string;
  receiverName: string;
  phone: string;
  province: string;
  district: string;
  commune: string;
  street: string;
  isDefault: boolean;
  createdAt: string;
}

export interface OrderItem { id: string; productId: string; quantity: number; unitPrice: number; totalPrice: number }
export interface Order {
  id: string;
  customerProfileId: string;
  addressId: string;
  note?: string | null;
  status: number;
  totalAmount: number;
  orderItems: OrderItem[];
  createdAt: string;
  updatedAt?: string | null;
}

export interface Payment {
  id: string;
  orderId: string;
  amount: number;
  paymentMethod: string;
  status: number;
  createdAt: string;
  updatedAt?: string | null;
}

export interface Refund {
  id: string;
  paymentId: string;
  amount: number;
  reason: string;
  status: number;
  createdAt: string;
  updatedAt?: string | null;
}

export interface ProductReview {
  id: string;
  customerProfileId: string;
  productId: string;
  orderId?: string | null;
  rating: number;
  comment?: string | null;
  status: number;
  createdAt: string;
}

export interface StorefrontReview {
  id: string;
  productId: string;
  productName: string;
  productImageUrl?: string | null;
  customerName: string;
  rating: number;
  comment: string;
  createdAt: string;
}

export interface InventoryTransaction {
  id: string;
  productId: string;
  type: number;
  quantity: number;
  note?: string | null;
  createdAt: string;
}

export interface SalesStats { totalUnitsSold: number; totalRevenue: number }

export interface ApiErrorShape {
  status: number;
  message: string;
  errors?: Record<string, string[]>;
  traceId?: string;
}
