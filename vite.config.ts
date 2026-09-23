import path from "path"
import tailwindcss from "@tailwindcss/vite"
import react from "@vitejs/plugin-react"
import { defineConfig } from "vite"

// https://vite.dev/config/
export default defineConfig({
  plugins: [react(), tailwindcss()],
  resolve: {
    alias: {
      "@": path.resolve(__dirname, "./src"),
    },
  },
  server: {
    // Add your domain to the allowed hosts list
    allowedHosts: ["lcap.enxcl.com"],
    // Optional: If you are running this in Docker, you likely also need:
    host: true, 
    port: 5173, // or whatever port you mapped in Docker
  },
})