import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'
import path from 'path'

// https://vitejs.dev/config/
export default defineConfig({
  plugins: [react()],
  
  resolve: {
    alias: {
      '@': path.resolve(__dirname, './src'),
    },
  },
  
  server: {
    https: true,  // Required for microphone access
    host: true,
    port: 5173,
    proxy: {
      // In dev mode, proxy API calls to Python backend
      '/api': {
        target: 'https://localhost:8000',
        changeOrigin: true,
        secure: false,
      },
      // WebSocket proxy for development
      '/ws': {
        target: 'wss://localhost:8000',
        ws: true,
        secure: false,
      },
    },
  },
  
  build: {
    outDir: 'dist',
    sourcemap: false,
    rollupOptions: {
      output: {
        // Keep it simple - single bundle
        manualChunks: undefined,
      },
    },
  },
})
