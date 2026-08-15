import Keycloak from "keycloak-js";

const keycloak = new Keycloak({
  url: import.meta.env.VITE_KEYCLOAK_URL || "http://localhost:8080",
  realm: import.meta.env.VITE_KEYCLOAK_REALM || "nexrig",
  clientId: import.meta.env.VITE_KEYCLOAK_CLIENT_ID || "nexrig-web",
});

export async function initializeKeycloak() {
  return keycloak.init({
    onLoad: "check-sso",
    pkceMethod: "S256",
    checkLoginIframe: false,
    silentCheckSsoRedirectUri: `${window.location.origin}/silent-check-sso.html`,
  });
}

export async function accessToken() {
  if (!keycloak.authenticated) return null;

  try {
    await keycloak.updateToken(30);
    return keycloak.token ?? null;
  } catch {
    keycloak.clearToken();
    return null;
  }
}

export default keycloak;
