import clsx from "clsx";
import { AlertTriangle, LoaderCircle, PackageOpen, RefreshCw } from "lucide-react";
import type { ButtonHTMLAttributes, InputHTMLAttributes, ReactNode, SelectHTMLAttributes } from "react";

export function Button({ className, variant = "primary", size = "md", ...props }: ButtonHTMLAttributes<HTMLButtonElement> & { variant?: "primary" | "secondary" | "ghost" | "danger"; size?: "sm" | "md" | "lg" }) {
  return <button className={clsx("button", `button--${variant}`, `button--${size}`, className)} {...props} />;
}

export function Input({ className, ...props }: InputHTMLAttributes<HTMLInputElement>) {
  return <input className={clsx("input", className)} {...props} />;
}

export function Select({ className, ...props }: SelectHTMLAttributes<HTMLSelectElement>) {
  return <select className={clsx("input select", className)} {...props} />;
}

export function Card({ className, children }: { className?: string; children: ReactNode }) {
  return <section className={clsx("card", className)}>{children}</section>;
}

export function Badge({ children, tone = "neutral" }: { children: ReactNode; tone?: "neutral" | "success" | "warning" | "danger" | "info" }) {
  return <span className={`badge badge--${tone}`}>{children}</span>;
}

export function Spinner({ label = "Loading" }: { label?: string }) {
  return <div className="loading-state" role="status"><LoaderCircle className="spin" aria-hidden="true" /><span>{label}</span></div>;
}

export function SkeletonGrid({ count = 4 }: { count?: number }) {
  return <div className="product-grid" aria-label="Loading products">{Array.from({ length: count }, (_, index) => <div className="skeleton-card" key={index}><div className="skeleton skeleton--visual" /><div className="skeleton skeleton--line" /><div className="skeleton skeleton--short" /></div>)}</div>;
}

export function EmptyState({ title, description, action }: { title: string; description: string; action?: ReactNode }) {
  return <div className="state-panel"><span className="state-icon"><PackageOpen aria-hidden="true" /></span><h2>{title}</h2><p>{description}</p>{action}</div>;
}

export function ErrorState({ error, onRetry }: { error: unknown; onRetry?: () => void }) {
  const message = error instanceof Error ? error.message : "Something went wrong while loading this content.";
  return <div className="state-panel state-panel--error" role="alert"><span className="state-icon"><AlertTriangle aria-hidden="true" /></span><h2>We hit a snag</h2><p>{message}</p>{onRetry && <Button variant="secondary" onClick={onRetry}><RefreshCw size={16} /> Try again</Button>}</div>;
}

export function PageHeader({ eyebrow, title, description, action }: { eyebrow?: string; title: string; description?: string; action?: ReactNode }) {
  return <header className="page-header"><div>{eyebrow && <span className="eyebrow">{eyebrow}</span>}<h1>{title}</h1>{description && <p>{description}</p>}</div>{action}</header>;
}

export function Field({ label, error, children, hint }: { label: string; error?: string; children: ReactNode; hint?: string }) {
  return <label className="field"><span className="field__label">{label}</span>{children}{hint && <small>{hint}</small>}{error && <span className="field__error">{error}</span>}</label>;
}

export function Modal({ title, children, onClose }: { title: string; children: ReactNode; onClose: () => void }) {
  return <div className="modal-backdrop" role="presentation" onMouseDown={(event) => { if (event.target === event.currentTarget) onClose(); }}><div className="modal" role="dialog" aria-modal="true" aria-labelledby="modal-title"><div className="modal__header"><h2 id="modal-title">{title}</h2><button className="icon-button" onClick={onClose} aria-label="Close dialog">×</button></div>{children}</div></div>;
}
