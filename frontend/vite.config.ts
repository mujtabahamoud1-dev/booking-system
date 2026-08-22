import { fileURLToPath, URL } from "node:url";
import { defineConfig } from "vite";
import vue from "@vitejs/plugin-vue";
import tailwindcss from "@tailwindcss/vite";
import vueDevTools from "vite-plugin-vue-devtools";

// Opt-in: the devtools toolbar covers the docked confirm button on a phone.
const devtools = process.env.VITE_DEVTOOLS === "true";

// https://vite.dev/config/
export default defineConfig({
  plugins: [vue(), ...(devtools ? [vueDevTools()] : []), tailwindcss()],
  resolve: {
    alias: {
      "@": fileURLToPath(new URL("./src", import.meta.url)),
    },
  },
  server: {
    // The API's CORS policy allows exactly this origin with credentials.
    port: 5173,
    strictPort: true,
  },
});
