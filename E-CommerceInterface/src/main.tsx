import { StrictMode } from "react";
import { createRoot } from "react-dom/client";
import { BrowserRouter } from "react-router-dom";
import App from "./App";
import "./styles/globals.css";
import { initializeKeycloak } from "./core/auth/keycloak";

async function bootstrap() {
  try {
    await initializeKeycloak();
  } catch (error) {
    console.error("Keycloak initialization failed", error);
  }

  createRoot(document.getElementById("root")!).render(
    <StrictMode><BrowserRouter><App /></BrowserRouter></StrictMode>,
  );
}

void bootstrap();
