import { defineConfig } from "vite";
import react from "@vitejs/plugin-react";

const target = "https://localhost:7162";

export default defineConfig({
  plugins: [react()],
  server: {
    port: 44472,
    host: true,
    proxy: {
      "/version": {
        target: target,
        secure: false,
        changeOrigin: true,
      },
      "/standort": { target: target, secure: false, changeOrigin: true },
      "/bohrung": { target: target, secure: false, changeOrigin: true },
      "/bohrprofil": { target: target, secure: false, changeOrigin: true },
      "/schicht": { target: target, secure: false, changeOrigin: true },
      "/vorkommnis": { target: target, secure: false, changeOrigin: true },
      "/export": { target: target, secure: false, changeOrigin: true },
      "/code": { target: target, secure: false, changeOrigin: true },
      "/codeschicht": { target: target, secure: false, changeOrigin: true },
      "/user": { target: target, secure: false, changeOrigin: true },
    },
    headers: {
      Connection: "Keep-Alive",
    },
  },
});
