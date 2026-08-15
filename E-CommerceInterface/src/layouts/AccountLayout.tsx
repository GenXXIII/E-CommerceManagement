import {
  Heart,
  House,
  MapPin,
  Package,
  RotateCcw,
  UserRound,
} from "lucide-react";
import { NavLink, Outlet } from "react-router-dom";

const links = [
  ["/account", "Overview", House, true],
  ["/account/profile", "Profile", UserRound],
  ["/account/addresses", "Addresses", MapPin],
  ["/account/wishlist", "Wishlist", Heart],
  ["/account/orders", "Orders", Package],
  ["/account/refunds", "Refunds", RotateCcw],
] as const;

export function AccountLayout() {
  return (
    <div className="container account-shell">
      <aside className="account-nav">
        <span className="eyebrow">Your account</span>
        <h2>Manage your NEXRIG</h2>
        <nav>
          {links.map(([to, label, Icon, end]) => (
            <NavLink key={to} to={to} end={end}>
              <Icon size={18} />
              {label}
            </NavLink>
          ))}
        </nav>
      </aside>
      <div className="account-content">
        <Outlet />
      </div>
    </div>
  );
}
