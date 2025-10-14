# Deployment Guide

This guide covers deploying applications that use the Agent C Realtime SDK, with special attention to audio system requirements.

## Table of Contents
- [Pre-Deployment Checklist](#pre-deployment-checklist)
- [AudioWorklet Deployment](#audioworklet-deployment)
- [Environment Configuration](#environment-configuration)
- [Platform-Specific Guides](#platform-specific-guides)
- [Production Optimization](#production-optimization)
- [Monitoring and Debugging](#monitoring-and-debugging)

## Pre-Deployment Checklist

### Essential Requirements

- [ ] **HTTPS configured** - Required for microphone access and WebSocket Secure (WSS)
- [ ] **AudioWorklet file deployed** - Must be accessible at `/worklets/audio-processor.worklet.js`
- [ ] **Environment variables set** - API endpoints and configuration
- [ ] **CORS configured** - For API and WebSocket connections
- [ ] **CSP headers allow worklets** - Content Security Policy must permit AudioWorklet

### Audio System Requirements

- [ ] AudioWorklet file is accessible via HTTPS
- [ ] Sample rate set to 16000Hz in configuration
- [ ] Binary WebSocket support enabled
- [ ] Fallback for browsers without AudioWorklet support

## AudioWorklet Deployment

The AudioWorklet is a critical component that must be properly deployed for audio features to work.

### Why AudioWorklet Deployment Matters

The AudioWorklet runs audio processing code off the main thread, providing:
- Real-time audio resampling (browser native rate to 16kHz)
- Float32 to PCM16 conversion
- Low-latency audio processing
- Better performance on all devices

### Deployment Methods

#### Method 1: Static File Deployment (Recommended)

1. **Copy worklet during build:**

   ```json
   // package.json
   {
     "scripts": {
       "build": "next build && npm run copy-worklet",
       "copy-worklet": "cp node_modules/@agentc/realtime-core/dist/worklets/audio-processor.worklet.js public/worklets/",
       "postinstall": "npm run copy-worklet"
     }
   }
   ```

2. **Verify in build output:**

   ```bash
   # After build, verify the file exists
   ls -la build/worklets/audio-processor.worklet.js
   # or
   ls -la dist/worklets/audio-processor.worklet.js
   ```

#### Method 2: Webpack Configuration

For webpack-based builds (Next.js, Create React App with ejected config):

```javascript
// webpack.config.js or next.config.js
const CopyWebpackPlugin = require('copy-webpack-plugin');

module.exports = {
  webpack: (config) => {
    config.plugins.push(
      new CopyWebpackPlugin({
        patterns: [
          {
            from: 'node_modules/@agentc/realtime-core/dist/worklets',
            to: 'worklets',
            info: { minimized: true }
          }
        ]
      })
    );
    return config;
  }
};
```

#### Method 3: CDN Deployment

For high-traffic applications, serve the worklet from a CDN:

1. **Upload worklet to CDN:**

   ```bash
   # Upload to your CDN
   aws s3 cp node_modules/@agentc/realtime-core/dist/worklets/audio-processor.worklet.js \
     s3://your-cdn-bucket/worklets/audio-processor.worklet.js
   ```

2. **Configure SDK to use CDN:**

   ```typescript
   const client = new RealtimeClient({
     apiUrl: 'wss://api.agentc.ai',
     authManager,
     audioConfig: {
       workletPath: 'https://cdn.yourdomain.com/worklets/audio-processor.worklet.js',
       sampleRate: 16000
     }
   });
   ```

#### Method 4: Docker Deployment

For containerized deployments:

```dockerfile
# Dockerfile
FROM node:18-alpine as builder

WORKDIR /app
COPY package*.json ./
RUN npm ci

COPY . .
RUN npm run build

# Copy worklet to static directory
RUN mkdir -p /app/build/worklets && \
    cp node_modules/@agentc/realtime-core/dist/worklets/audio-processor.worklet.js \
    /app/build/worklets/

FROM nginx:alpine
COPY --from=builder /app/build /usr/share/nginx/html

# Ensure proper MIME type for .js files
RUN echo "application/javascript js;" >> /etc/nginx/mime.types
```

### Verifying Worklet Deployment

After deployment, verify the worklet is accessible:

```javascript
// Test script
async function verifyWorklet() {
  const workletUrl = window.location.origin + '/worklets/audio-processor.worklet.js';
  
  try {
    const response = await fetch(workletUrl);
    
    if (!response.ok) {
      throw new Error(`HTTP ${response.status}: ${response.statusText}`);
    }
    
    const contentType = response.headers.get('content-type');
    if (!contentType?.includes('javascript')) {
      console.warn(`Unexpected content-type: ${contentType}`);
    }
    
    console.log('✅ Worklet file is accessible');
    return true;
  } catch (error) {
    console.error('❌ Worklet verification failed:', error);
    return false;
  }
}

// Run verification
verifyWorklet();
```

## Environment Configuration

### Production Environment Variables

```bash
# .env.production

# Your Backend API (NOT Agent C directly)
REACT_APP_API_URL=https://api.yourdomain.com

# WebSocket configuration
REACT_APP_WS_RECONNECT_ATTEMPTS=5
REACT_APP_WS_RECONNECT_DELAY=1000

# Audio configuration
REACT_APP_AUDIO_SAMPLE_RATE=16000
REACT_APP_AUDIO_WORKLET_PATH=/worklets/audio-processor.worklet.js

# Feature flags
REACT_APP_ENABLE_AUDIO=true
REACT_APP_ENABLE_AVATARS=false

# Analytics (optional)
REACT_APP_ANALYTICS_ID=UA-XXXXXXXXX
```

### Security Configuration

**Content Security Policy (CSP):**

```html
<!-- Allow AudioWorklet -->
<meta http-equiv="Content-Security-Policy" 
      content="default-src 'self'; 
               script-src 'self' 'unsafe-inline' blob:; 
               worker-src 'self' blob:;
               connect-src 'self' wss://api.agentc.ai https://api.yourdomain.com">
```

**CORS Configuration:**

```javascript
// Express.js backend
app.use(cors({
  origin: process.env.FRONTEND_URL,
  credentials: true,
  methods: ['GET', 'POST'],
  allowedHeaders: ['Content-Type', 'Authorization']
}));
```

## Platform-Specific Guides

### Vercel Deployment

```json
// vercel.json
{
  "rewrites": [
    {
      "source": "/worklets/:path*",
      "destination": "/worklets/:path*"
    }
  ],
  "headers": [
    {
      "source": "/worklets/(.*)",
      "headers": [
        {
          "key": "Content-Type",
          "value": "application/javascript"
        },
        {
          "key": "Cache-Control",
          "value": "public, max-age=31536000, immutable"
        }
      ]
    }
  ]
}
```

### Netlify Deployment

```toml
# netlify.toml
[[headers]]
  for = "/worklets/*"
  [headers.values]
    Content-Type = "application/javascript"
    Cache-Control = "public, max-age=31536000, immutable"

[[redirects]]
  from = "/worklets/*"
  to = "/worklets/:splat"
  status = 200
```

### AWS Amplify

```yaml
# amplify.yml
version: 1
frontend:
  phases:
    preBuild:
      commands:
        - npm ci
    build:
      commands:
        - npm run build
        - cp node_modules/@agentc/realtime-core/dist/worklets/audio-processor.worklet.js dist/worklets/
  artifacts:
    baseDirectory: dist
    files:
      - '**/*'
  cache:
    paths:
      - node_modules/**/*
```

### Google Cloud Run

```yaml
# cloudbuild.yaml
steps:
  - name: 'gcr.io/cloud-builders/npm'
    args: ['ci']
  
  - name: 'gcr.io/cloud-builders/npm'
    args: ['run', 'build']
  
  - name: 'gcr.io/cloud-builders/npm'
    entrypoint: 'sh'
    args:
      - '-c'
      - 'cp node_modules/@agentc/realtime-core/dist/worklets/audio-processor.worklet.js build/worklets/'
  
  - name: 'gcr.io/cloud-builders/docker'
    args: ['build', '-t', 'gcr.io/$PROJECT_ID/app', '.']
  
  - name: 'gcr.io/cloud-builders/docker'
    args: ['push', 'gcr.io/$PROJECT_ID/app']
```

## Production Optimization

### Audio Optimization

```typescript
// Production audio configuration
const productionAudioConfig = {
  // Core settings
  sampleRate: 16000,              // Server expects 16kHz
  enableInput: true,
  enableOutput: true,
  
  // Performance settings
  bufferSize: 4096,               // Larger buffer for stability
  latencyHint: 'balanced',        // Balance between latency and stability
  
  // Quality settings
  echoCancellation: true,
  noiseSuppression: true,
  autoGainControl: true,
  
  // Resampling
  resamplingQuality: 'high',      // Better quality for production
  
  // Turn management
  respectTurnState: true,         // Prevent talk-over
  
  // Monitoring
  enableMetrics: true,            // Enable performance metrics
  metricsInterval: 5000,          // Report every 5 seconds
  
  // Fallbacks
  fallbackToTextOnly: true,       // Graceful degradation
  
  // CDN worklet (optional)
  workletPath: process.env.REACT_APP_CDN_URL + '/worklets/audio-processor.worklet.js'
};
```

### Bundle Size Optimization

```javascript
// Lazy load audio components
const AudioInterface = lazy(() => 
  import(/* webpackChunkName: "audio" */ './AudioInterface')
);

// Tree-shake unused features
import { RealtimeClient } from '@agentc/realtime-core/client';
// Instead of: import { RealtimeClient } from '@agentc/realtime-core';
```

### Caching Strategy

```nginx
# nginx.conf
location /worklets/ {
    # Long cache for immutable worklet files
    add_header Cache-Control "public, max-age=31536000, immutable";
    
    # CORS headers for cross-origin worklet loading
    add_header Access-Control-Allow-Origin *;
    
    # Correct MIME type
    add_header Content-Type application/javascript;
}

location /api/ {
    # No cache for API calls
    add_header Cache-Control "no-cache, no-store, must-revalidate";
}
```

## Monitoring and Debugging

### Health Check Endpoint

```typescript
// Health check for audio system
app.get('/health/audio', async (req, res) => {
  const checks = {
    workletAccessible: false,
    websocketReachable: false,
    audioContextSupported: false
  };
  
  try {
    // Check worklet accessibility
    const workletResponse = await fetch(
      `${req.protocol}://${req.get('host')}/worklets/audio-processor.worklet.js`
    );
    checks.workletAccessible = workletResponse.ok;
    
    // Check WebSocket endpoint
    // Implementation depends on your setup
    checks.websocketReachable = true;
    
    // This would be checked client-side
    checks.audioContextSupported = true;
    
    res.json({
      status: Object.values(checks).every(v => v) ? 'healthy' : 'degraded',
      checks
    });
  } catch (error) {
    res.status(500).json({
      status: 'unhealthy',
      error: error.message,
      checks
    });
  }
});
```

### Client-Side Diagnostics

```typescript
// Include in production for debugging user issues
class AudioDiagnostics {
  static async runAndReport() {
    const diagnostics = {
      timestamp: new Date().toISOString(),
      userAgent: navigator.userAgent,
      url: window.location.href,
      
      // Audio capabilities
      audioContext: typeof AudioContext !== 'undefined',
      audioWorklet: typeof AudioWorklet !== 'undefined',
      getUserMedia: navigator.mediaDevices?.getUserMedia !== undefined,
      
      // Worklet check
      workletAccessible: false,
      workletError: null,
      
      // Sample rates
      nativeSampleRate: null,
      targetSampleRate: 16000,
      
      // Permissions
      microphonePermission: null,
      
      // Connection
      websocketSupport: typeof WebSocket !== 'undefined',
      protocol: window.location.protocol
    };
    
    try {
      // Check AudioContext sample rate
      const ctx = new AudioContext();
      diagnostics.nativeSampleRate = ctx.sampleRate;
      ctx.close();
      
      // Check worklet
      const workletUrl = '/worklets/audio-processor.worklet.js';
      const response = await fetch(workletUrl);
      diagnostics.workletAccessible = response.ok;
      
      // Check microphone permission
      const permissionStatus = await navigator.permissions.query({ name: 'microphone' });
      diagnostics.microphonePermission = permissionStatus.state;
      
    } catch (error) {
      diagnostics.workletError = error.message;
    }
    
    // Send to analytics or logging service
    console.log('Audio Diagnostics:', diagnostics);
    
    // Send to backend for monitoring
    fetch('/api/diagnostics/audio', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(diagnostics)
    });
    
    return diagnostics;
  }
}

// Run on audio errors
window.addEventListener('error', (event) => {
  if (event.message?.includes('audio') || event.message?.includes('worklet')) {
    AudioDiagnostics.runAndReport();
  }
});
```

### Production Logging

```typescript
// Structured logging for production
const logger = {
  info: (message: string, data?: any) => {
    console.log(JSON.stringify({
      level: 'info',
      timestamp: new Date().toISOString(),
      message,
      data,
      session: sessionStorage.getItem('sessionId')
    }));
  },
  
  error: (message: string, error?: any) => {
    console.error(JSON.stringify({
      level: 'error',
      timestamp: new Date().toISOString(),
      message,
      error: error?.message || error,
      stack: error?.stack,
      session: sessionStorage.getItem('sessionId')
    }));
    
    // Send to error tracking service
    if (window.Sentry) {
      window.Sentry.captureException(error || new Error(message));
    }
  }
};

// Use in audio initialization
try {
  await client.startAudioRecording();
  logger.info('Audio recording started successfully');
} catch (error) {
  logger.error('Failed to start audio recording', error);
}
```

## Post-Deployment Verification

After deploying to production:

1. **Test Audio System:**
   ```bash
   curl https://yourapp.com/worklets/audio-processor.worklet.js
   # Should return JavaScript code, not 404 or HTML
   ```

2. **Test WebSocket Connection:**
   ```javascript
   // Browser console
   const ws = new WebSocket('wss://api.agentc.ai');
   ws.onopen = () => console.log('WebSocket connected');
   ws.onerror = (e) => console.error('WebSocket error:', e);
   ```

3. **Test Audio Permissions:**
   - Open the application in an incognito/private window
   - Verify microphone permission prompt appears
   - Test audio input and output

4. **Monitor Error Rates:**
   - Check for 404 errors on `/worklets/audio-processor.worklet.js`
   - Monitor WebSocket connection failures
   - Track audio initialization errors

5. **Performance Metrics:**
   - Audio latency should be < 200ms
   - WebSocket reconnection should succeed within 5 attempts
   - No memory leaks in long-running sessions

## Troubleshooting Deployment Issues

### Common Issues and Solutions

| Issue | Symptoms | Solution |
|-------|----------|----------|
| Worklet 404 | Console error, no audio | Verify worklet file deployment |
| HTTPS not configured | Microphone permission denied | Enable HTTPS/WSS |
| CORS blocked | Network errors | Configure CORS headers |
| CSP violation | Worklet won't load | Update CSP to allow worklets |
| Sample rate mismatch | Distorted audio | Ensure 16kHz configuration |

For detailed troubleshooting steps, see the [Audio Troubleshooting Guide](./audio-troubleshooting.md).

## Support

If you encounter deployment issues:

1. Run the diagnostic script above
2. Check the browser console for errors
3. Review server logs for WebSocket issues
4. Contact support with diagnostic output

Remember: Most deployment issues are related to:
- Missing AudioWorklet file
- HTTPS/WSS configuration
- CORS/CSP policies
- Environment variables