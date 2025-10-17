# Agent C Static SPA

A pure static single-page application that showcases the Agent C Realtime SDK. This app is designed to be served from **any web server**, including the Python API itself.

## Key Features

- ✅ **Zero Configuration** - No environment variables needed
- ✅ **Static Build** - Pure HTML/CSS/JS bundle
- ✅ **Any Web Server** - nginx, Apache, Python, S3, etc.
- ✅ **Relative URLs** - All API calls to same host
- ✅ **Minimal Dependencies** - Just React + Router + Agent C SDK

## Architecture

```
Static SPA (served from Python API)
├── / (index.html)
├── /login → Login page
└── /chat → Chat interface with AgentC WebSocket

Same Host API Endpoints:
├── /api/rt/login → Authentication
└── /api/rt/websocket → WebSocket connection
```

## Development

```bash
# Install dependencies
pnpm install

# Run dev server (proxies to Python API at localhost:8000)
pnpm dev

# Build static bundle
pnpm build

# Preview production build
pnpm preview
```

## Production Deployment

### Option 1: Serve from Python API (Recommended)

```python
from fastapi import FastAPI
from fastapi.staticfiles import StaticFiles

app = FastAPI()

# Your API routes
@app.post("/api/rt/login")
async def login(...): ...

@app.websocket("/api/rt/websocket")
async def websocket(...): ...

# Serve static SPA (must be LAST!)
app.mount("/", StaticFiles(directory="path/to/dist", html=True), name="spa")
```

### Option 2: Any Web Server

Copy the `dist/` folder to any web server:

```bash
# nginx
cp -r dist/* /var/www/html/

# Apache
cp -r dist/* /var/www/html/

# Python http.server
cd dist && python -m http.server 8080
```

## No Environment Variables!

Because the SPA is served from the same host as the API, all URLs are relative:

```typescript
// Login - relative URL
fetch('/api/rt/login', { ... })

// WebSocket - uses current host
const wsUrl = `wss://${window.location.host}/api/rt/websocket`
```

## Technology Stack

- **React 18** - UI library
- **React Router 7** - Client-side routing
- **Vite 7** - Build tool (fast HMR, optimized builds)
- **TypeScript** - Type safety
- **Tailwind CSS** - Styling (via UI package)
- **@agentc/realtime-ui** - Pre-built UI components
- **@agentc/realtime-react** - React hooks for Agent C
- **@agentc/realtime-core** - WebSocket client

## Build Output

The `dist/` folder contains:

```
dist/
├── index.html              # Entry point
├── assets/
│   ├── index-[hash].js    # Bundled JavaScript
│   └── index-[hash].css   # Bundled CSS
└── worklets/
    └── audio-processor.worklet.js
```

Total bundle size: ~500KB (gzipped)

## Comparison with Demo App

| Feature | Demo (Next.js) | Static SPA |
|---------|----------------|------------|
| Runtime | Node.js required | Any web server |
| Config | Multiple env vars | Zero config |
| Deploy | `npm start` | Copy files |
| Size | ~2MB+ | ~500KB |
| Complexity | High | Low |

## License

Private - Agent C Platform
