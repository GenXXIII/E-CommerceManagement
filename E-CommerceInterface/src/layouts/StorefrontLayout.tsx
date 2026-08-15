import { useQuery } from "@tanstack/react-query";
import {
  Bell,
  ChevronDown,
  Heart,
  Menu,
  Search,
  ShoppingCart,
  UserRound,
  X,
} from "lucide-react";
import { useState, type FormEvent, type MouseEventHandler } from "react";
import { Link, NavLink, Outlet, useLocation, useNavigate } from "react-router-dom";
import { useAuth } from "../core/auth/AuthProvider";
import { accountApi, accountKeys } from "../features/account/api";
import { catalogApi, catalogKeys } from "../features/catalog/api";
import { commerceApi, commerceKeys } from "../features/commerce/api";
import { SignOutButton, ThemeToggle } from "../components/SessionControls";

export function Brand({ inverse = false, replace = false, onClick }: { inverse?: boolean; replace?: boolean; onClick?: MouseEventHandler<HTMLAnchorElement> }) {
  return (
    <Link
      to="/"
      replace={replace}
      onClick={onClick}
      className={`brand ${inverse ? "brand--inverse" : ""}`}
      aria-label="NEXRIG computer store home"
    >
      <span className="brand__word" aria-hidden="true">
        <img className="brand__mark-image" src="/nexrig-mark.svg" alt="" />
        <span>NEXRIG</span>
      </span>
    </Link>
  );
}

export function StorefrontLayout() {
  const { session } = useAuth();
  const navigate = useNavigate();
  const location = useLocation();
  const [search, setSearch] = useState("");
  const [mobileOpen, setMobileOpen] = useState(false);
  const categories = useQuery({
    queryKey: catalogKeys.categories,
    queryFn: catalogApi.categories,
  });
  const activeCategories =
    categories.data?.filter((category) => category.isActive) ?? [];
  const customerId = session.customerProfileId ?? "";
  const cart = useQuery({
    queryKey: commerceKeys.cart(customerId),
    queryFn: () => commerceApi.cart(customerId),
    enabled: Boolean(customerId),
  });
  const profile = useQuery({
    queryKey: accountKeys.profile(customerId),
    queryFn: () => accountApi.profile(customerId),
    enabled: session.role === "customer" && Boolean(customerId),
  });
  const cartCount =
    cart.data?.items.reduce((sum, item) => sum + item.quantity, 0) ?? 0;
  const profileDestination = session.role === "admin"
    ? "/admin"
    : session.role === "customer"
      ? "/account"
      : "/login?redirect=/account";
  const wishlistDestination = session.role === "customer"
    ? "/account/wishlist"
    : session.isAuthenticated
      ? "/login?switch=1&redirect=/account/wishlist"
      : "/login?redirect=/account/wishlist";
  const cartDestination = session.role === "customer"
    ? "/cart"
    : session.isAuthenticated
      ? "/login?switch=1&redirect=/cart"
      : "/login?redirect=/cart";
  const ordersDestination = session.role === "customer"
    ? "/account/orders"
    : session.isAuthenticated
      ? "/login?switch=1&redirect=/account/orders"
      : "/login?redirect=/account/orders";
  const profileLabel = session.role === "admin"
    ? "Admin"
    : profile.data
      ? `${profile.data.firstName} ${profile.data.lastName}`.trim()
      : session.role === "customer"
        ? session.displayName || "Profile"
        : "Profile";

  const replaceTopDestination = location.pathname !== "/";
  const submitSearch = (event: FormEvent) => {
    event.preventDefault();
    const params = new URLSearchParams({ page: "1", pageSize: "12" });
    if (search.trim()) params.set("keyword", search.trim());
    navigate(`/products?${params.toString()}`, { replace: replaceTopDestination });
  };

  return (
    <div className="site-shell">
      <a className="skip-link" href="#main-content">
        Skip to content
      </a>
      <div className="storefront-chrome">
        <div className="utility-bar">
          <div className="container utility-bar__inner">
          <nav aria-label="Account shortcuts">
            {session.isAuthenticated ? (
              <>
                <span>Hi, {session.displayName}!</span>
                <SignOutButton />
              </>
            ) : (
              <Link to="/login" replace={replaceTopDestination}>Sign in</Link>
            )}
            <Link to="/products" replace={replaceTopDestination}>Tech deals</Link>
            <Link to={ordersDestination} replace={replaceTopDestination}>Track an order</Link>
          </nav>
          <nav aria-label="Shopping shortcuts">
            <ThemeToggle />
            <Link
              to={wishlistDestination}
              replace={replaceTopDestination}
            >
              Wishlist
            </Link>
            <Link
              className="utility-account"
              to={profileDestination}
              replace={replaceTopDestination}
            >
              {profileLabel} <ChevronDown size={12} />
            </Link>
            <Link
              className="utility-icon"
              to={profileDestination}
              replace={replaceTopDestination}
              aria-label="Notifications"
            >
              <Bell size={17} />
            </Link>
            <Link
              className="utility-icon cart-link"
              to={cartDestination}
              replace={replaceTopDestination}
              aria-label={`Cart with ${cartCount} items`}
            >
              <ShoppingCart size={19} />
              {session.isAuthenticated && cartCount > 0 && (
                <b>{cart.isLoading ? "…" : cartCount}</b>
              )}
            </Link>
          </nav>
          </div>
        </div>
        <header className="site-header">
        <div className="container header-main">
          <button
            className="icon-button mobile-only"
            onClick={() => setMobileOpen(true)}
            aria-label="Open menu"
          >
            <Menu />
          </button>
          <Brand replace={replaceTopDestination} />
          <form className="search-bar" role="search" onSubmit={submitSearch}>
            <Search aria-hidden="true" size={20} />
            <input
              value={search}
              onChange={(event) => setSearch(event.target.value)}
              aria-label="Search products"
              placeholder="Search laptops, parts, monitors and more"
            />
            <button type="submit">Search</button>
          </form>
          <Link className="advanced-link" to="/products" replace={replaceTopDestination}>
            Advanced
          </Link>
          <nav className="header-actions">
            <Link
              to={wishlistDestination}
              replace={replaceTopDestination}
              aria-label="Saved items"
            >
              <Heart />
              <span>Saved</span>
            </Link>
            <Link
              to={profileDestination}
              replace={replaceTopDestination}
              aria-label="Account"
            >
              <UserRound />
              <span>{session.role === "admin" ? "Admin" : "Account"}</span>
            </Link>
            <Link
              className="cart-link"
              to={cartDestination}
              replace={replaceTopDestination}
              aria-label={`Cart with ${cartCount} items`}
            >
              <ShoppingCart />
              <span>Cart</span>
              {session.isAuthenticated && cartCount > 0 && (
                <b>{cart.isLoading ? "…" : cartCount}</b>
              )}
            </Link>
          </nav>
        </div>
        <nav className="category-nav" aria-label="Product categories">
          <div className="container category-nav__inner">
            <NavLink to="/" replace>Home</NavLink>
            <NavLink to="/products" replace={replaceTopDestination}>Explore</NavLink>
            {activeCategories.slice(0, 8).map((category) => (
              <NavLink key={category.id} to={`/categories/${category.id}`} replace={replaceTopDestination}>
                {category.name}
              </NavLink>
            ))}
            {/* <NavLink to="/account/wishlist">WishList</NavLink>
            {categories.isError && (
              <span className="category-nav__status">
                Categories unavailable
              </span>
            )} */}
          </div>
        </nav>
        </header>
      </div>
      {mobileOpen && (
        <div
          className="mobile-drawer-backdrop"
          onMouseDown={() => setMobileOpen(false)}
        >
          <aside
            className="mobile-drawer"
            onMouseDown={(event) => event.stopPropagation()}
          >
            <div className="mobile-drawer__header">
              <Brand replace={replaceTopDestination} />
              <button
                className="icon-button"
                onClick={() => setMobileOpen(false)}
                aria-label="Close menu"
              >
                <X />
              </button>
            </div>
            <nav onClick={() => setMobileOpen(false)}>
              <NavLink to="/" replace>Home</NavLink>
              <NavLink to="/products" replace={replaceTopDestination}>Explore all</NavLink>
              <NavLink to={wishlistDestination} replace={replaceTopDestination}>Wishlist</NavLink>
              <NavLink to={cartDestination} replace={replaceTopDestination}>Cart</NavLink>
              <NavLink to={profileDestination} replace={replaceTopDestination}>{profileLabel}</NavLink>
              {session.isAuthenticated && (
                <SignOutButton
                  className="mobile-drawer__signout"
                  showIcon
                  onSignOut={() => setMobileOpen(false)}
                />
              )}
              <ThemeToggle showLabel />
            </nav>
            <div className="mobile-drawer__categories">
              {activeCategories.map((category) => (
                <Link
                  key={category.id}
                  onClick={() => setMobileOpen(false)}
                  replace={replaceTopDestination}
                  to={`/categories/${category.id}`}
                >
                  {category.name}
                </Link>
              ))}
            </div>
          </aside>
        </div>
      )}
      <main id="main-content">
        <Outlet />
      </main>
      {session.role !== "admin" && (
      <footer className="site-footer">
        <div className="container footer-grid">
          <div>
            <Brand inverse />
            <p>
              Your computer store for complete systems, components, upgrades,
              and the gear that brings a setup together.
            </p>
          </div>
          <div>
            <h2>Buy</h2>
            <Link to="/products">Shop computers</Link>
            <Link to="/account/orders">Order tracking</Link>
            <Link to="/account/wishlist">Saved gear</Link>
          </div>
          <div>
            <h2>Support</h2>
            <Link to="/account/orders">Order help</Link>
            <Link to="/account/refunds">Returns and refunds</Link>
            <Link to="/account/addresses">Delivery addresses</Link>
          </div>
          <div>
            <h2>Stay connected</h2>
            <Link to="/account">My NEXRIG</Link>
            <Link to="/account/profile">Account settings</Link>
            <Link to="/cart">Shopping cart</Link>
          </div>
        </div>
        <div className="container footer-bottom">
          <span>© {new Date().getFullYear()} NEXRIG Computer Store</span>
          <span>Computers, components, and setup essentials.</span>
        </div>
      </footer>
      )}
    </div>
  );
}
