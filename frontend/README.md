# Admin frontend

React and Vite admin interface for the Monitoring System API.

## Run locally

```sh
npm install
npm run dev
```

Open the local address printed by Vite. In the API address field, enter the backend origin (for example, `https://localhost:7062`) only when bypassing the local proxy. The API must allow cross-origin requests when the frontend and backend use different origins.

For local development, the Vite server proxies API routes to `https://localhost:7062` by default and accepts the local development certificate. Start the backend with its HTTPS launch profile before using the dashboard. Set `VITE_API_PROXY_TARGET` if your backend listens on another address; use `http://localhost:5135` with the HTTP-only launch profile.

## Build

```sh
npm run build
```

The Warnings view is informational because the backend currently has no warning controller routes.
