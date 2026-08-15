import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { Navigate, Route, Routes, useLocation } from "react-router-dom";
import type { ReactNode } from "react";
import type { DemoRole } from "./core/types";
import { AuthProvider, useAuth } from "./core/auth/AuthProvider";
import { StorefrontLayout } from "./layouts/StorefrontLayout";
import { AccountLayout } from "./layouts/AccountLayout";
import { AdminLayout } from "./layouts/AdminLayout";
import { CategoryPage, HomePage, LoginCallbackPage, LoginPage, ProductDetailPage, ProductsPage, RegisterPage } from "./pages/StorefrontPages";
import { CartPage, CheckoutPage, PaymentPage, PaymentResultPage } from "./pages/CommercePages";
import { AccountOverviewPage, AddressesPage, OrderDetailPage, OrdersPage, ProfilePage, RefundsPage, WishlistPage } from "./pages/AccountPages";
import { AdminCategoriesPage, AdminCustomersPage, AdminDashboardPage, AdminInventoryPage, AdminOrdersPage, AdminPaymentsPage, AdminProductsPage, AdminRefundsPage, AdminReportsPage, AdminReviewsPage } from "./pages/AdminPages";
import { EmptyState } from "./components/ui";
import { Link } from "react-router-dom";
import { ThemeProvider } from "./core/theme/ThemeProvider";

const queryClient = new QueryClient({
  defaultOptions: {
    queries: { staleTime: 30_000, retry: (count, error: any) => error?.status !== 404 && count < 2, refetchOnWindowFocus: false },
    mutations: { retry: false },
  },
});

function Guard({ children, role }: { children: ReactNode; role?: DemoRole }) {
  const { session } = useAuth();
  const location = useLocation();
  if (!session.isAuthenticated) return <Navigate replace to={`/login?redirect=${encodeURIComponent(location.pathname + location.search)}`} />;
  if (role && session.role !== role) {
    if (role === "customer") {
      return <Navigate replace to={`/login?switch=1&redirect=${encodeURIComponent(location.pathname + location.search)}`} />;
    }
    return <Navigate replace to="/unauthorized" />;
  }
  return children;
}

function UnauthorizedPage() {
  return <div className="container page-pad"><EmptyState title="This area needs administrator access" description="The customer account is separate from the operations workspace. Sign in with the administrator account to continue." action={<Link className="button button--primary button--md" to="/login?switch=1&redirect=/admin">Switch to Admin</Link>} /></div>;
}

function NotFoundPage() {
  return <div className="container page-pad"><EmptyState title="That page wandered off" description="The address may be outdated, or this feature has not been connected yet." action={<Link className="button button--primary button--md" to="/">Return home</Link>} /></div>;
}

export default function App() {
  return <ThemeProvider><QueryClientProvider client={queryClient}><AuthProvider><Routes>
    <Route path="/login" element={<LoginPage />} />
    <Route path="/login/callback" element={<LoginCallbackPage />} />
    <Route path="/register" element={<RegisterPage />} />
    <Route element={<StorefrontLayout />}>
      <Route index element={<HomePage />} />
      <Route path="products" element={<ProductsPage />} />
      <Route path="products/:productId" element={<ProductDetailPage />} />
      <Route path="categories/:categoryId" element={<CategoryPage />} />
      <Route path="unauthorized" element={<UnauthorizedPage />} />
      <Route path="cart" element={<Guard role="customer"><CartPage /></Guard>} />
      <Route path="checkout" element={<Guard role="customer"><CheckoutPage /></Guard>} />
      <Route path="payment/:paymentId" element={<Guard role="customer"><PaymentPage /></Guard>} />
      <Route path="payment/:paymentId/result" element={<Guard role="customer"><PaymentResultPage /></Guard>} />
      <Route path="account" element={<Guard role="customer"><AccountLayout /></Guard>}>
        <Route index element={<AccountOverviewPage />} />
        <Route path="profile" element={<ProfilePage />} />
        <Route path="addresses" element={<AddressesPage />} />
        <Route path="wishlist" element={<WishlistPage />} />
        <Route path="orders" element={<OrdersPage />} />
        <Route path="orders/:orderId" element={<OrderDetailPage />} />
        <Route path="refunds" element={<RefundsPage />} />
      </Route>
      <Route path="*" element={<NotFoundPage />} />
    </Route>
    <Route path="admin" element={<Guard role="admin"><AdminLayout /></Guard>}>
      <Route index element={<AdminDashboardPage />} />
      <Route path="categories" element={<AdminCategoriesPage />} />
      <Route path="products" element={<AdminProductsPage />} />
      <Route path="inventory" element={<AdminInventoryPage />} />
      <Route path="orders" element={<AdminOrdersPage />} />
      <Route path="payments" element={<AdminPaymentsPage />} />
      <Route path="refunds" element={<AdminRefundsPage />} />
      <Route path="reviews" element={<AdminReviewsPage />} />
      <Route path="customers" element={<AdminCustomersPage />} />
      <Route path="reports" element={<AdminReportsPage />} />
    </Route>
  </Routes></AuthProvider></QueryClientProvider></ThemeProvider>;
}
