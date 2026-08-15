import { LogOut, Moon, Sun } from "lucide-react";
import { useState } from "react";
import { useAuth } from "../core/auth/AuthProvider";
import { useTheme } from "../core/theme/ThemeProvider";

interface SignOutButtonProps {
  className?: string;
  showIcon?: boolean;
  onSignOut?: () => void;
}

export function SignOutButton({ className, showIcon = false, onSignOut }: SignOutButtonProps) {
  const { logout } = useAuth();
  const [isSigningOut, setIsSigningOut] = useState(false);

  const signOut = () => {
    if (isSigningOut) return;
    setIsSigningOut(true);
    onSignOut?.();
    void logout();
  };

  return (
    <button type="button" className={className} onClick={signOut} disabled={isSigningOut}>
      {showIcon && <LogOut size={18} aria-hidden="true" />}
      {isSigningOut ? "Signing out..." : "Sign out"}
    </button>
  );
}

export function ThemeToggle({ showLabel = false }: { showLabel?: boolean }) {
  const { theme, toggleTheme } = useTheme();
  const nextTheme = theme === "dark" ? "light" : "dark";

  return (
    <button
      type="button"
      className={`theme-toggle ${showLabel ? "theme-toggle--labeled" : ""}`}
      onClick={toggleTheme}
      aria-label={`Switch to ${nextTheme} mode`}
      title={`Switch to ${nextTheme} mode`}
    >
      {theme === "dark" ? <Sun aria-hidden="true" /> : <Moon aria-hidden="true" />}
      {showLabel && <span>{theme === "dark" ? "Light mode" : "Dark mode"}</span>}
    </button>
  );
}
