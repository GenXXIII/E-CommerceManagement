import type { ApiErrorShape } from "../types";
import { accessToken } from "../auth/keycloak";

const API_BASE = (import.meta.env.VITE_API_BASE_URL || "/api").replace(/\/$/, "");

export class ApiError extends Error implements ApiErrorShape {
  status: number;
  errors?: Record<string, string[]>;
  traceId?: string;

  constructor(shape: ApiErrorShape) {
    super(shape.message);
    this.name = "ApiError";
    this.status = shape.status;
    this.errors = shape.errors;
    this.traceId = shape.traceId;
  }
}

async function parseError(response: Response): Promise<ApiError> {
  let body: any = null;
  try { body = await response.json(); } catch { /* non-json error */ }
  const fallback: Record<number, string> = {
    400: "The request could not be completed.",
    401: "Invalid username or password.",
    403: "You do not have permission to do that.",
    404: "The requested record was not found.",
    409: "This record changed. Refresh and try again.",
    429: "Too many requests. Please wait a moment.",
  };
  return new ApiError({
    status: response.status,
    message: body?.message ?? body?.detail ?? body?.title ?? (typeof body === "string" ? body : null) ?? fallback[response.status] ?? "The service is temporarily unavailable.",
    errors: body?.errors,
    traceId: body?.traceId,
  });
}

export async function apiRequest<T>(path: string, init?: RequestInit): Promise<T> {
  let response: Response;
  try {
    const token = await accessToken();
    response = await fetch(`${API_BASE}${path}`, {
      ...init,
      headers: {
        ...(init?.body instanceof FormData ? {} : { "Content-Type": "application/json" }),
        Accept: "application/json",
        ...(token ? { Authorization: `Bearer ${token}` } : {}),
        ...init?.headers,
      },
    });
  } catch {
    throw new ApiError({ status: 0, message: "Cannot reach the API. Check that the backend is running." });
  }
  if (!response.ok) throw await parseError(response);
  if (response.status === 204) return undefined as T;
  return response.json() as Promise<T>;
}

export function resolveApiAsset(path?: string) {
  if (!path) return undefined;
  if (/^https?:\/\//i.test(path) || path.startsWith("data:") || path.startsWith("blob:")) return path;
  if (/^https?:\/\//i.test(API_BASE)) return new URL(path, new URL(API_BASE).origin).toString();
  return path;
}

export function qs(values: Record<string, string | number | boolean | null | undefined>) {
  const params = new URLSearchParams();
  Object.entries(values).forEach(([key, value]) => {
    if (value !== undefined && value !== null && value !== "") params.set(key, String(value));
  });
  const query = params.toString();
  return query ? `?${query}` : "";
}
