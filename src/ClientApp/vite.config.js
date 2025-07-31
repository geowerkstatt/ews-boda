import { defineConfig } from "vite";
import react from "@vitejs/plugin-react";

const target = "https://localhost:7162";
const proxyRoutes = [
  "/version",
  "/standort",
  "/bohrung",
  "/bohrprofil",
  "/schicht",
  "/vorkommnis",
  "/export",
  "/code",
  "/codeschicht",
  "/user",
];

const proxyConfig = Object.fromEntries(
  proxyRoutes.map((route) => [
    route,
    {
      target,
      secure: false,
      changeOrigin: true,
    },
  ]),
);

export default defineConfig({
  plugins: [react()],
  server: {
    port: 44472,
    host: true,
    proxy: proxyConfig,
    headers: {
      Connection: "Keep-Alive",
    },
  },
});
