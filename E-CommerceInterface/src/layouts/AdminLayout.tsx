import { BarChart3, Boxes, ChevronLeft, CircleDollarSign, FolderTree, House, Menu, MessageSquareText, PackageCheck, RotateCcw, ShoppingBag, UsersRound, X } from "lucide-react";
import { useState } from "react";
import { NavLink, Outlet, useNavigate } from "react-router-dom";
import { Brand } from "./StorefrontLayout";
import { useAuth } from "../core/auth/AuthProvider";
import { SignOutButton, ThemeToggle } from "../components/SessionControls";

const groups = [
  ["Overview", [["/admin", "Dashboard", BarChart3, true], ["/admin/reports", "Reports", BarChart3]]],
  ["Catalog", [["/admin/products", "Products", ShoppingBag], ["/admin/categories", "Categories", FolderTree]]],
  ["Operations", [["/admin/inventory", "Inventory", Boxes], ["/admin/orders", "Orders", PackageCheck], ["/admin/payments", "Payments", CircleDollarSign], ["/admin/refunds", "Refunds", RotateCcw]]],
  ["Customers", [["/admin/customers", "Customers", UsersRound], ["/admin/reviews", "Reviews", MessageSquareText]]],
] as const;

export function AdminLayout() {
  const [open, setOpen] = useState(false);
  const [collapsed, setCollapsed] = useState(false);
  const { session } = useAuth();
  const navigate = useNavigate();
  const leaveAdmin = () => {
    const historyIndex = Number(window.history.state?.idx ?? 0);
    if (historyIndex > 0) navigate(-1);
    else navigate("/", { replace: true });
  };
  return <div className={`admin-shell ${collapsed ? "admin-shell--collapsed" : ""}`}>
    <aside className={`admin-sidebar ${open ? "admin-sidebar--open" : ""}`}><div className="admin-sidebar__brand"><Brand inverse replace /><button className="icon-button admin-close" onClick={() => setOpen(false)}><X /></button></div><nav>{groups.map(([label, items]) => <div className="admin-nav-group" key={label}><span>{label}</span>{items.map(([to, text, Icon, end]) => <NavLink key={to} to={to} end={Boolean(end)} replace onClick={() => setOpen(false)} title={text}><Icon size={19} /><b>{text}</b></NavLink>)}</div>)}</nav><button className="admin-collapse" onClick={() => setCollapsed(!collapsed)}><ChevronLeft size={17} /><span>Collapse sidebar</span></button></aside>
    {open && <div className="admin-overlay" onClick={() => setOpen(false)} />}
    <div className="admin-workspace"><header className="admin-topbar"><button className="icon-button" onClick={() => setOpen(true)}><Menu /></button><div><span className="eyebrow">Operations workspace</span><strong>{session.displayName}</strong></div><div className="admin-topbar__actions"><ThemeToggle /><SignOutButton showIcon /><button type="button" onClick={leaveAdmin}><House size={17} /> Storefront</button></div></header><main className="admin-main"><Outlet /></main></div>
  </div>;
}
