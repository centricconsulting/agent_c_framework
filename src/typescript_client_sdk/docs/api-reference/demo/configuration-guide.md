# Agent C Realtime Demo Configuration Guide

This guide provides comprehensive documentation for configuring and customizing the Agent C Realtime demo application. The demo app serves as a reference implementation showcasing the capabilities of the Agent C Realtime SDK.

## Table of Contents

1. [Environment Configuration](#environment-configuration)
2. [API Endpoint Configuration](#api-endpoint-configuration)
3. [HTTPS Configuration](#https-configuration)
4. [Theming and Styling](#theming-and-styling)
5. [Feature Toggles](#feature-toggles)
6. [Build Configuration](#build-configuration)
7. [Testing Configuration](#testing-configuration)
8. [Deployment Configuration](#deployment-configuration)

## Environment Configuration

The demo app uses environment variables for configuration. Create a `.env.local` file from the provided `.env.example` template.

### Environment Variables

#### Required Variables

```env
# Public - accessible in browser
NEXT_PUBLIC_APP_URL=https://localhost:3000
# WebSocket endpoint for realtime client (must use wss:// protocol)
NEXT_PUBLIC_AGENTC_API_URL=wss://localhost:8000
```

#### Server-Side Only Variables

```env
# Agent C API endpoint for server-side operations
AGENT_C_API_URL=https://localhost:8000
```

### Environment Variable Descriptions

| Variable | Type | Default | Description |
|----------|------|---------|-------------|
| `NEXT_PUBLIC_APP_URL` | Public | `https://localhost:3000` | The URL where the demo app is accessible. Used for callbacks and redirects. |
| `NEXT_PUBLIC_AGENTC_API_URL` | Public | `wss://localhost:8000` | WebSocket endpoint for the Realtime API. Must use `wss://` for secure connections. |
| `AGENT_C_API_URL` | Server | `https://localhost:8000` | REST API endpoint for authentication and server-side operations. |

### Development vs Production Settings

#### Development Configuration
```env
# .env.local (development)
NEXT_PUBLIC_APP_URL=https://localhost:3000
NEXT_PUBLIC_AGENTC_API_URL=wss://localhost:8000
AGENT_C_API_URL=https://localhost:8000
```

#### Production Configuration
```env
# .env.production
NEXT_PUBLIC_APP_URL=https://your-domain.com
NEXT_PUBLIC_AGENTC_API_URL=wss://api.your-domain.com/realtime
AGENT_C_API_URL=https://api.your-domain.com
```

## API Endpoint Configuration

### Base URL Configuration

The demo app connects to the Agent C Realtime API through configurable endpoints:

1. **REST API Base URL**: Used for authentication and user management
   - Configured via `AGENT_C_API_URL`
   - Server-side only for security
   - Handles JWT token exchange

2. **WebSocket URL**: Used for realtime communication
   - Configured via `NEXT_PUBLIC_AGENTC_API_URL`
   - Must be publicly accessible for client connections
   - Protocol must be `wss://` for secure connections

### WebSocket Endpoint Setup

The WebSocket connection is established through the SDK's `RealtimeClient`:

```typescript
// Automatic configuration from environment
const client = new RealtimeClient({
  url: process.env.NEXT_PUBLIC_AGENTC_API_URL // defaults to wss://localhost:8000
})
```

### Authentication Endpoints

The demo app uses the following authentication endpoints:

| Endpoint | Method | Purpose |
|----------|--------|---------|
| `/api/auth/token` | POST | Exchange credentials for JWT token |
| `/api/auth/refresh` | POST | Refresh expired tokens |
| `/api/auth/logout` | POST | Invalidate tokens |

## HTTPS Configuration

### Local Development HTTPS Setup

The demo app runs with HTTPS by default using Next.js's experimental HTTPS support:

#### Certificate Requirements

Self-signed certificates must be available at:
- Certificate: `../../../../agent_c_config/localhost_self_signed.pem`
- Private Key: `../../../../agent_c_config/localhost_self_signed-key.pem`

#### Starting HTTPS Server

```bash
# Start with HTTPS (default)
pnpm dev

# Fallback to HTTP if needed
pnpm dev:http
```

### Certificate Generation

To generate self-signed certificates for local development:

```bash
# Using OpenSSL
openssl req -x509 -newkey rsa:4096 -keyout localhost_self_signed-key.pem \
  -out localhost_self_signed.pem -days 365 -nodes \
  -subj "/CN=localhost"
```

### Next.js HTTPS Configuration

The HTTPS configuration is defined in `package.json`:

```json
{
  "scripts": {
    "dev": "next dev --experimental-https --experimental-https-key [key-path] --experimental-https-cert [cert-path]"
  }
}
```

### Custom Port Configuration

To use a different port:

```bash
# Use port 3001 instead of 3000
pnpm dev -- --port 3001
```

## Theming and Styling

### CenSuite Design System Customization

The demo app implements the CenSuite design system using CSS custom properties and Tailwind CSS.

#### CSS Custom Properties

Located in `src/styles/globals.css`, these properties define the color scheme:

##### Light Theme
```css
:root {
  /* Core Colors */
  --background: 0 0% 100%;
  --foreground: 262 80% 10%;
  
  /* Primary Colors */
  --primary: 262 80% 50%;
  --primary-foreground: 0 0% 100%;
  
  /* Secondary Colors */
  --secondary: 262 30% 95%;
  --secondary-foreground: 262 80% 25%;
  
  /* Semantic Colors */
  --destructive: 0 84.31% 60%;
  --muted: 262 20% 96%;
  --accent: 262 60% 92%;
  
  /* UI Elements */
  --border: 240 5.88% 90%;
  --input: 240 5.88% 90%;
  --ring: 261.88 71.43% 23.33%;
  
  /* Typography */
  --font-sans: Geist, sans-serif;
  --font-serif: "Lora", Georgia, serif;
  --font-mono: "Fira Code", "Courier New", monospace;
  
  /* Spacing */
  --radius: 0.5rem;
}
```

##### Dark Theme
```css
.dark {
  --background: 262 47% 5%;
  --foreground: 0 0% 95%;
  --primary: 262 80% 55%;
  /* ... other dark theme properties */
}
```

### Tailwind Configuration

The `tailwind.config.js` file extends the default configuration:

```javascript
module.exports = {
  darkMode: ["class"], // Enable class-based dark mode
  content: [
    "./src/**/*.{ts,tsx}",
    "../../packages/ui/src/**/*.{ts,tsx}", // Include UI package
  ],
  theme: {
    extend: {
      colors: {
        // Map CSS variables to Tailwind utilities
        primary: {
          DEFAULT: 'hsl(var(--primary))',
          foreground: 'hsl(var(--primary-foreground))'
        },
        // ... other color mappings
      },
      borderRadius: {
        lg: 'var(--radius)',
        md: 'calc(var(--radius) - 2px)',
        sm: 'calc(var(--radius) - 4px)'
      }
    }
  },
  plugins: [require("tailwindcss-animate")] // Animation utilities
}
```

### Dark/Light Mode Setup

Theme switching is handled by `next-themes`:

```typescript
// In your root layout or _app.tsx
import { ThemeProvider } from "next-themes"

export default function RootLayout({ children }) {
  return (
    <ThemeProvider
      attribute="class"
      defaultTheme="system"
      enableSystem
      disableTransitionOnChange
    >
      {children}
    </ThemeProvider>
  )
}
```

### Component Styling Configuration

The `components.json` file configures the shadcn/ui component system:

```json
{
  "style": "default",
  "rsc": true,
  "tsx": true,
  "tailwind": {
    "config": "tailwind.config.js",
    "css": "src/styles/globals.css",
    "baseColor": "neutral",
    "cssVariables": true
  },
  "aliases": {
    "components": "@/components",
    "ui": "@/components/ui",
    "utils": "@/lib/utils"
  }
}
```

## Feature Toggles

While the demo app doesn't use explicit feature flags, certain features can be enabled/disabled through configuration:

### Audio Features

Control audio functionality through the SDK configuration:

```typescript
const client = new RealtimeClient({
  audio: {
    enabled: true,              // Enable/disable audio processing
    echoCancellation: true,     // Toggle echo cancellation
    noiseSuppression: true,     // Toggle noise suppression
    autoGainControl: true,      // Toggle automatic gain control
    sampleRate: 24000          // Audio sample rate (16000 or 24000)
  }
})
```

### Avatar Integration

HeyGen avatar support is conditionally loaded based on the presence of the streaming avatar package:

```typescript
// Avatar feature is available when @heygen/streaming-avatar is installed
const avatarEnabled = !!window.StreamingAvatar
```

### Debug Mode Settings

Enable debug mode for detailed logging:

```typescript
// Set via environment variable or directly in code
const client = new RealtimeClient({
  debug: process.env.NODE_ENV === 'development'
})
```

## Build Configuration

### Next.js Configuration

The `next.config.js` file configures the build process:

```javascript
module.exports = {
  // Transpile UI package dependencies
  transpilePackages: ['@agentc/realtime-ui'],
  
  // Source maps (disabled in production for security)
  productionBrowserSourceMaps: false,
  
  webpack: (config, { dev, isServer }) => {
    // Package resolution aliases
    config.resolve.alias = {
      '@agentc/realtime-core': path.resolve(__dirname, '../core/dist'),
      '@agentc/realtime-react': path.resolve(__dirname, '../react/dist'),
    };
    
    // Development source maps
    if (dev && !isServer) {
      config.devtool = 'eval-source-map';
    }
    
    return config;
  }
}
```

### TypeScript Configuration

The `tsconfig.json` configures TypeScript compilation:

```json
{
  "compilerOptions": {
    "target": "es5",
    "lib": ["dom", "dom.iterable", "esnext"],
    "strict": true,
    "jsx": "preserve",
    "module": "esnext",
    "moduleResolution": "bundler",
    "paths": {
      "@/*": ["./src/*"]
    }
  },
  "include": ["**/*.ts", "**/*.tsx"],
  "exclude": ["node_modules", "**/*.test.ts", "vitest.config.ts"]
}
```

### ESLint Configuration

The `.eslintrc.json` extends Next.js's recommended configuration:

```json
{
  "extends": "next/core-web-vitals"
}
```

### Prettier Configuration

Code formatting is configured in `prettier.config.js`:

```javascript
module.exports = {
  semi: false,
  singleQuote: false,
  tabWidth: 2,
  trailingComma: "es5",
  plugins: [
    "@ianvs/prettier-plugin-sort-imports",
    "prettier-plugin-tailwindcss"
  ],
  importOrder: [
    "^(react/(.*)$)|^(react$)",
    "^(next/(.*)$)|^(next$)",
    "<THIRD_PARTY_MODULES>",
    "^@/components/ui/(.*)$",
    "^@/components/(.*)$",
    "^[./]"
  ]
}
```

## Testing Configuration

### Vitest Configuration

The `vitest.config.ts` configures the test environment:

```typescript
export default defineConfig({
  test: {
    globals: true,
    environment: 'happy-dom',
    setupFiles: ['./src/test/setup.ts'],
    coverage: {
      provider: 'v8',
      reporter: ['text', 'json', 'html', 'lcov'],
      reportsDirectory: '../../.scratch/coverage/demo',
      exclude: ['node_modules/**', 'dist/**', '**/*.config.*'],
      thresholds: {
        branches: 80,
        functions: 80,
        lines: 80,
        statements: 80
      }
    },
    testTimeout: 10000
  },
  resolve: {
    alias: {
      '@': resolve(__dirname, './src'),
      '@test': resolve(__dirname, './src/test')
    }
  }
})
```

### Test Environment Setup

Create a `src/test/setup.ts` file for global test configuration:

```typescript
// Import testing utilities
import '@testing-library/jest-dom'

// Mock environment variables
process.env.NEXT_PUBLIC_APP_URL = 'https://localhost:3000'
process.env.NEXT_PUBLIC_AGENTC_API_URL = 'wss://localhost:8000'
```

### Mock Configuration

Configure mocks for external dependencies:

```typescript
// Mock WebSocket for testing
vi.mock('@agentc/realtime-core', () => ({
  RealtimeClient: vi.fn()
}))

// Mock Next.js router
vi.mock('next/navigation', () => ({
  useRouter: () => ({
    push: vi.fn(),
    replace: vi.fn()
  })
}))
```

### Test Commands

Available test commands in `package.json`:

```json
{
  "scripts": {
    "test": "vitest run",
    "test:watch": "vitest watch",
    "test:coverage": "vitest run --coverage",
    "test:ui": "vitest --ui",
    "test:integration": "vitest run --grep '\\.integration\\.'",
    "test:unit": "vitest run --grep -v '\\.integration\\.'"
  }
}
```

## Deployment Configuration

### Production Environment Variables

Configure for production deployment:

```env
# Production .env
NEXT_PUBLIC_APP_URL=https://demo.your-domain.com
NEXT_PUBLIC_AGENTC_API_URL=wss://api.your-domain.com/realtime
AGENT_C_API_URL=https://api.your-domain.com

# Optional: Analytics
NEXT_PUBLIC_GA_TRACKING_ID=UA-XXXXXXXXX-X
```

### Build Optimization Settings

#### Production Build

```bash
# Build for production
pnpm build

# Output analysis
pnpm build -- --analyze
```

#### Build Output Configuration

Configure output in `next.config.js`:

```javascript
module.exports = {
  output: 'standalone', // For Docker deployments
  compress: true,       // Enable gzip compression
  poweredByHeader: false, // Remove X-Powered-By header
  
  // Image optimization
  images: {
    domains: ['your-cdn.com'],
    formats: ['image/webp']
  }
}
```

### CDN and Caching Setup

#### Static Asset Caching

Configure caching headers in `next.config.js`:

```javascript
module.exports = {
  async headers() {
    return [
      {
        source: '/static/:path*',
        headers: [
          {
            key: 'Cache-Control',
            value: 'public, max-age=31536000, immutable'
          }
        ]
      }
    ]
  }
}
```

#### CDN Configuration

For CDN deployment, configure asset prefix:

```javascript
module.exports = {
  assetPrefix: process.env.CDN_URL || '',
  images: {
    loader: 'custom',
    loaderFile: './src/lib/image-loader.js'
  }
}
```

### Docker Deployment

Example `Dockerfile` for containerized deployment:

```dockerfile
FROM node:20-alpine AS builder
WORKDIR /app
COPY package*.json ./
RUN npm ci
COPY . .
RUN npm run build

FROM node:20-alpine AS runner
WORKDIR /app
ENV NODE_ENV production
COPY --from=builder /app/.next/standalone ./
COPY --from=builder /app/.next/static ./.next/static
COPY --from=builder /app/public ./public

EXPOSE 3000
CMD ["node", "server.js"]
```

### Performance Optimization

#### Bundle Analysis

```bash
# Install bundle analyzer
pnpm add -D @next/bundle-analyzer

# Configure in next.config.js
const withBundleAnalyzer = require('@next/bundle-analyzer')({
  enabled: process.env.ANALYZE === 'true'
})

module.exports = withBundleAnalyzer({
  // ... your config
})

# Run analysis
ANALYZE=true pnpm build
```

#### Optimization Strategies

1. **Code Splitting**: Automatic with Next.js dynamic imports
2. **Tree Shaking**: Enabled by default in production
3. **Minification**: Automatic with Terser in production
4. **Image Optimization**: Use Next.js Image component
5. **Font Optimization**: Use Next.js font optimization

### Security Configuration

#### Content Security Policy

Add CSP headers in `next.config.js`:

```javascript
module.exports = {
  async headers() {
    return [
      {
        source: '/:path*',
        headers: [
          {
            key: 'Content-Security-Policy',
            value: `
              default-src 'self';
              script-src 'self' 'unsafe-eval' 'unsafe-inline';
              style-src 'self' 'unsafe-inline';
              connect-src 'self' wss://*.your-domain.com;
            `.replace(/\n/g, ' ').trim()
          }
        ]
      }
    ]
  }
}
```

#### Environment Variable Validation

Use `@t3-oss/env-nextjs` for runtime validation:

```typescript
// src/env.mjs
import { createEnv } from "@t3-oss/env-nextjs"
import { z } from "zod"

export const env = createEnv({
  client: {
    NEXT_PUBLIC_APP_URL: z.string().url(),
    NEXT_PUBLIC_AGENTC_API_URL: z.string().url()
  },
  server: {
    AGENT_C_API_URL: z.string().url()
  }
})
```

## Troubleshooting

### Common Configuration Issues

1. **Certificate Errors**: Ensure certificates exist at the correct path
2. **WebSocket Connection Failed**: Verify `wss://` protocol in `NEXT_PUBLIC_AGENTC_API_URL`
3. **Build Failures**: Check TypeScript configuration and dependencies
4. **Theme Not Applied**: Verify `next-themes` provider is configured
5. **Test Failures**: Ensure test environment variables are set

### Debug Configuration

Enable verbose logging for troubleshooting:

```typescript
// Enable debug mode
localStorage.setItem('debug', 'agentc:*')

// Or via environment
DEBUG=agentc:* pnpm dev
```

## Summary

This configuration guide covers all aspects of customizing and deploying the Agent C Realtime demo application. The demo app is designed to be flexible and configurable while maintaining security and performance best practices. Use this guide as a reference when setting up your own implementation or deploying the demo to production environments.