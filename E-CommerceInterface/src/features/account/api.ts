import { apiRequest } from "../../core/api/apiClient";
import type { CustomerProfile } from "../../core/types";

export const accountKeys = { profile: (id: string) => ["profile", id] as const };
export const accountApi = {
  profile: (id: string) => apiRequest<CustomerProfile>(`/customerprofiles/${id}`),
  updateProfile: (id: string, payload: Partial<CustomerProfile>) => apiRequest<void>(`/customerprofiles/${id}`, { method: "PUT", body: JSON.stringify(payload) }),
};
