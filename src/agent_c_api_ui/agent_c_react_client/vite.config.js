import path from "path"
import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

export default defineConfig({
  plugins: [react()],
  resolve: {
    alias: {
      "@": path.resolve(__dirname, "./src"),
    },
    extensions: ['.js', '.jsx', '.ts', '.tsx']
  },
  define: {
    __API_URL__: JSON.stringify(process.env.VITE_API_URL),
  },
  build: {
    sourcemap: true, // Enable source maps for production builds
  },
  server: {
    host: true,
    strictPort: true,
    allowedHosts: true,
    sourcemapIgnoreList: (sourcePath) => {

      // Ignore source maps for external libraries

      return sourcePath.includes('node_modules');

    }
  }
})