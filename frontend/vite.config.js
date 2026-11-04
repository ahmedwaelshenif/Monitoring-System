import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

const backendTarget = process.env.VITE_API_PROXY_TARGET || 'https://localhost:7062'

export default defineConfig({
  plugins: [react()],
  server: {
    proxy: Object.fromEntries(
      ['/lab', '/entryLog', '/member', '/dashboardAccount', '/registrationRequest', '/warning']
        .map(path => [path, { target: backendTarget, changeOrigin: true, secure: false }]),
    ),
  },
})
