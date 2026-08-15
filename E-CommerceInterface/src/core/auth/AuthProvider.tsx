/* eslint-disable react-refresh/only-export-components */
import { createContext, use, useCallback, useEffect, useMemo, useState, type ReactNode } from "react";
import { useQueryClient } from "@tanstack/react-query";
import { apiRequest } from "../api/apiClient";
import type { AuthSession, DemoRole } from "../types";
import keycloak from "./keycloak";

const EMPTY_SESSION: AuthSession = { isAuthenticated: false, username: null, displayName: null, role: null, customerProfileId: null };

interface AuthSessionResponse { username: string; displayName: string; role: DemoRole; customerProfileId: string | null }
interface AuthContextValue {
  session: AuthSession;
  login: (redirect?: string, force?: boolean) => Promise<void>;
  register: (redirect?: string) => Promise<void>;
  logout: () => Promise<void>;
}

const AuthContext = createContext<AuthContextValue | null>(null);

export function AuthProvider({ children }: { children: ReactNode }) {
  const [session, setSession] = useState<AuthSession>(EMPTY_SESSION);
  const [ready, setReady] = useState(false);
  const queryClient = useQueryClient();

  useEffect(() => {
    let active = true;

    async function loadSession() {
      if (!keycloak.authenticated) {
        if (active) setReady(true);
        return;
      }

      try {
        const result = await apiRequest<AuthSessionResponse>("/auth/session");
        if (active) setSession({ isAuthenticated: true, ...result });
      } catch {
        keycloak.clearToken();
        if (active) setSession(EMPTY_SESSION);
      } finally {
        if (active) setReady(true);
      }
    }

    void loadSession();
    keycloak.onAuthLogout = () => window.location.replace("/");
    return () => {
      active = false;
      keycloak.onAuthLogout = undefined;
    };
  }, []);

  const login = useCallback(async (redirect = "/", force = false) => {
    const safeRedirect = redirect.startsWith("/") ? redirect : "/";
    const callbackUrl = new URL("/login/callback", window.location.origin);
    callbackUrl.searchParams.set("redirect", safeRedirect);
    await keycloak.login({
      redirectUri: callbackUrl.toString(),
      ...(force ? { prompt: "login" } : {}),
    });
  }, []);

  const register = useCallback(async (redirect = "/account") => {
    const safeRedirect = redirect.startsWith("/") ? redirect : "/account";
    const callbackUrl = new URL("/login/callback", window.location.origin);
    callbackUrl.searchParams.set("redirect", safeRedirect);
    await keycloak.register({ redirectUri: callbackUrl.toString() });
  }, []);

  const logout = useCallback(async () => {
    queryClient.clear();
    setSession(EMPTY_SESSION);

    const redirectUri = `${window.location.origin}/`;
    try {
      // Build one route-independent logout URL before leaving the SPA. Sending
      // every sign-out control through it also preserves Keycloak's id token
      // hint, so signing out works from nested storefront and admin routes.
      const logoutUrl = keycloak.createLogoutUrl({ redirectUri });
      window.location.replace(logoutUrl);
    } catch {
      keycloak.clearToken();
      window.location.replace(redirectUri);
    }
  }, [queryClient]);

  const value = useMemo(() => ({ session, login, register, logout }), [session, login, register, logout]);
  if (!ready) return <div className="auth-loading" role="status">Loading your account...</div>;
  return <AuthContext value={value}>{children}</AuthContext>;
}

export function useAuth() {
  const value = use(AuthContext);
  if (!value) throw new Error("useAuth must be used within AuthProvider");
  return value;
}
