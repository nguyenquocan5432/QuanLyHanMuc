import { defineConfig } from 'vite';
import react from '@vitejs/plugin-react';

const mayChu = 'http://localhost:5080';

export default defineConfig({
  plugins: [react()],
  build: { outDir: '../wwwroot', emptyOutDir: true },
  server: {
    port: 5173,
    proxy: {
      '/api': mayChu,
      '/xac-thuc': mayChu,
      '/signin-oidc': mayChu,
      '/signout-callback-oidc': mayChu,
    },
  },
});
