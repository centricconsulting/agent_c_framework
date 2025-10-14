# Audio System Quick Reference

## 🚀 Quick Setup

### 1. Deploy AudioWorklet File (Required!)

```bash
# Copy worklet to your public directory
cp node_modules/@agentc/realtime-core/dist/worklets/audio-processor.worklet.js public/worklets/
```

### 2. Verify Deployment

```bash
# Should return JavaScript code, not 404
curl http://localhost:3000/worklets/audio-processor.worklet.js
```

### 3. Initialize Audio

```typescript
const client = new RealtimeClient({
  apiUrl: 'wss://api.agentc.ai',
  authManager,
  audioConfig: {
    sampleRate: 16000,  // Server expects 16kHz
    // SDK automatically resamples from browser native rate
  }
});
```

## 🎯 Key Facts

| Feature | Details |
|---------|---------|
| **Server Sample Rate** | 16kHz (16000Hz) |
| **Browser Native Rates** | Usually 44.1kHz or 48kHz |
| **Resampling** | Automatic in AudioWorklet |
| **Format** | PCM16 (16-bit signed integers) |
| **Transmission** | Binary ArrayBuffer (33% bandwidth savings) |
| **Processing** | Off-thread in AudioWorklet |

## 🔧 Common Issues & Solutions

### Issue: 404 Error for AudioWorklet

**Symptom:** Console error about missing `/worklets/audio-processor.worklet.js`

**Solution:**
```bash
# Ensure file exists in public directory
ls public/worklets/audio-processor.worklet.js

# If missing, copy it:
mkdir -p public/worklets
cp node_modules/@agentc/realtime-core/dist/worklets/audio-processor.worklet.js public/worklets/
```

### Issue: Audio Sounds Distorted

**Symptom:** Audio too fast/slow or garbled

**Solution:** Already handled! The SDK automatically resamples audio.

```typescript
// To debug:
client.on('audio:config', (config) => {
  console.log('Native rate:', config.nativeSampleRate);  // e.g., 48000
  console.log('Target rate:', config.targetSampleRate);  // Always 16000
  console.log('Resampling:', config.isResampling);       // true if rates differ
});
```

### Issue: No Microphone Access

**Symptom:** Can't record audio

**Solutions:**
1. **Check HTTPS:** Required for microphone (except localhost)
2. **Handle permission properly:**

```typescript
try {
  await client.startAudioRecording();
} catch (error) {
  if (error.name === 'NotAllowedError') {
    alert('Please allow microphone access');
  } else if (error.name === 'NotFoundError') {
    alert('No microphone found');
  }
}
```

## 📦 Package.json Setup

Add to your `package.json` for automatic worklet deployment:

```json
{
  "scripts": {
    "postinstall": "npm run copy-worklet",
    "copy-worklet": "mkdir -p public/worklets && cp node_modules/@agentc/realtime-core/dist/worklets/audio-processor.worklet.js public/worklets/",
    "build": "your-build-command && npm run copy-worklet"
  }
}
```

## 🏗️ Build Configuration Examples

### Next.js

```javascript
// next.config.js
module.exports = {
  webpack: (config) => {
    // Copy worklet during build
    const CopyPlugin = require('copy-webpack-plugin');
    config.plugins.push(
      new CopyPlugin({
        patterns: [{
          from: 'node_modules/@agentc/realtime-core/dist/worklets',
          to: '../public/worklets'
        }]
      })
    );
    return config;
  }
};
```

### Vite

```javascript
// vite.config.js
import { defineConfig } from 'vite';
import { copyFileSync, mkdirSync } from 'fs';

export default defineConfig({
  plugins: [
    {
      name: 'copy-worklet',
      buildStart() {
        mkdirSync('public/worklets', { recursive: true });
        copyFileSync(
          'node_modules/@agentc/realtime-core/dist/worklets/audio-processor.worklet.js',
          'public/worklets/audio-processor.worklet.js'
        );
      }
    }
  ]
});
```

### Docker

```dockerfile
# After npm install, copy worklet
RUN mkdir -p /app/public/worklets && \
    cp /app/node_modules/@agentc/realtime-core/dist/worklets/audio-processor.worklet.js \
    /app/public/worklets/
```

## 🔍 Debugging Commands

```javascript
// Check if worklet is accessible
fetch('/worklets/audio-processor.worklet.js')
  .then(r => console.log('Worklet status:', r.status))
  .catch(e => console.error('Worklet error:', e));

// Check browser audio capabilities
const ctx = new AudioContext();
console.log('Native sample rate:', ctx.sampleRate);
console.log('AudioWorklet support:', typeof AudioWorklet !== 'undefined');
ctx.close();

// Monitor audio performance
client.on('audio:metrics', (metrics) => {
  console.log('Audio metrics:', {
    inputLatency: metrics.inputLatency,
    outputLatency: metrics.outputLatency,
    packetsLost: metrics.packetsLost,
    jitter: metrics.jitter
  });
});
```

## 📋 Production Checklist

- [ ] AudioWorklet file deployed to `/worklets/audio-processor.worklet.js`
- [ ] HTTPS enabled (required for microphone)
- [ ] Sample rate set to 16000 in config
- [ ] Error handlers for audio permissions
- [ ] Fallback UI for unsupported browsers
- [ ] CSP headers allow AudioWorklet
- [ ] CORS configured if worklet served from CDN

## 🆘 Need Help?

1. Check [Audio Troubleshooting Guide](./guides/audio-troubleshooting.md)
2. Review [Deployment Guide](./guides/deployment.md)
3. See [Audio Streaming Guide](./guides/audio-streaming.md)

## 📊 Audio Data Flow

```
Microphone (48kHz) → Capture → Worklet → Resample → 16kHz PCM16 → WebSocket → Server
                                  ↑
                            Runs off-thread!
```

Remember: The AudioWorklet handles all the complex resampling automatically!