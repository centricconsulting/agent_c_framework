# Audio System Troubleshooting Guide

This guide addresses common issues and solutions when working with the Agent C Realtime SDK's audio system, including AudioWorklet deployment, sample rate handling, and browser compatibility.

## Table of Contents
- [Common Issues](#common-issues)
  - [404 AudioWorklet File Not Found](#404-audioworklet-file-not-found)
  - [Sample Rate Mismatches](#sample-rate-mismatches)
  - [No Audio Input/Output](#no-audio-inputoutput)
  - [Browser Compatibility Issues](#browser-compatibility-issues)
- [AudioWorklet Deployment](#audioworklet-deployment)
- [Sample Rate Configuration](#sample-rate-configuration)
- [Debugging Tools](#debugging-tools)
- [Production Checklist](#production-checklist)

## Common Issues

### 404 AudioWorklet File Not Found

**Symptoms:**
- Console error: `Failed to load module script: Expected a JavaScript module script but the server responded with a MIME type of "text/html"`
- Console error: `404 Not Found` for `/worklets/audio-processor.worklet.js`
- Audio input fails silently or throws errors

**Root Cause:**
The AudioWorklet file must be served from a public URL path. The SDK expects it at `/worklets/audio-processor.worklet.js` by default.

**Solutions:**

#### Solution 1: Deploy Worklet File (Recommended)

1. **Copy the worklet file to your public directory:**

   For Next.js applications:
   ```bash
   # Copy from the SDK to your public directory
   cp node_modules/@agentc/realtime-core/dist/worklets/audio-processor.worklet.js public/worklets/
   ```

   For other frameworks, copy to your static/public directory:
   ```bash
   # React (Create React App)
   cp node_modules/@agentc/realtime-core/dist/worklets/audio-processor.worklet.js public/worklets/

   # Vue
   cp node_modules/@agentc/realtime-core/dist/worklets/audio-processor.worklet.js public/worklets/

   # Angular
   cp node_modules/@agentc/realtime-core/dist/worklets/audio-processor.worklet.js src/assets/worklets/
   ```

2. **Verify the file is accessible:**
   
   Navigate to `http://localhost:3000/worklets/audio-processor.worklet.js` in your browser. You should see the JavaScript code, not a 404 error.

3. **Configure build process to include worklet:**

   For Next.js (`next.config.js`):
   ```javascript
   module.exports = {
     webpack: (config) => {
       // Ensure worklets are copied during build
       config.plugins.push(
         new CopyWebpackPlugin({
           patterns: [
             {
               from: 'node_modules/@agentc/realtime-core/dist/worklets',
               to: '../public/worklets'
             }
           ]
         })
       );
       return config;
     }
   };
   ```

#### Solution 2: Custom Worklet Path

If you need to serve the worklet from a different location:

```typescript
const client = new RealtimeClient({
    apiUrl: 'wss://api.agentc.ai',
    authManager,
    audioConfig: {
        workletPath: '/custom/path/to/audio-processor.worklet.js'
    }
});
```

#### Solution 3: CDN Deployment

For production, you might want to serve the worklet from a CDN:

```typescript
const client = new RealtimeClient({
    apiUrl: 'wss://api.agentc.ai',
    authManager,
    audioConfig: {
        workletPath: 'https://cdn.yourdomain.com/worklets/audio-processor.worklet.js'
    }
});
```

### Sample Rate Mismatches

**Symptoms:**
- Audio sounds distorted, too fast, or too slow
- Console warning: `Sample rate mismatch: browser native X Hz, expected 16000 Hz`
- Pitched or garbled audio output

**Root Cause:**
Different browsers and audio devices use different native sample rates (commonly 44100Hz, 48000Hz, or 16000Hz). The Agent C server expects 16000Hz (16kHz) audio.

**How It's Handled:**

The SDK now handles resampling automatically in the AudioWorklet:

```javascript
// AudioWorklet automatically resamples from browser native rate to 16kHz
// Browser native (e.g., 48000Hz) → Resampling → 16000Hz → Server
```

**Solutions:**

#### Solution 1: Verify Automatic Resampling (Default)

The SDK should handle resampling automatically. To verify:

```typescript
const client = new RealtimeClient({
    apiUrl: 'wss://api.agentc.ai',
    authManager,
    audioConfig: {
        sampleRate: 16000,  // Target sample rate (server expectation)
        // The SDK will automatically resample from browser native rate
    }
});

// Check the actual rates being used
client.on('audio:config', (config) => {
    console.log('Browser native sample rate:', config.nativeSampleRate);
    console.log('Target sample rate:', config.targetSampleRate);
    console.log('Resampling:', config.isResampling ? 'Yes' : 'No');
});
```

#### Solution 2: Force Specific Sample Rate

If you need to override the automatic detection:

```typescript
const client = new RealtimeClient({
    apiUrl: 'wss://api.agentc.ai',
    authManager,
    audioConfig: {
        sampleRate: 16000,
        forceNativeSampleRate: 48000,  // Override browser detection
    }
});
```

#### Solution 3: Debug Sample Rate Issues

```typescript
// Enable detailed audio debugging
const client = new RealtimeClient({
    apiUrl: 'wss://api.agentc.ai',
    authManager,
    debug: true,
    audioConfig: {
        sampleRate: 16000,
        debugAudio: true  // Enables detailed audio logging
    }
});

// Listen for audio diagnostics
client.on('audio:diagnostic', (info) => {
    console.log('Audio Diagnostic:', {
        inputSampleRate: info.inputSampleRate,
        outputSampleRate: info.outputSampleRate,
        resamplingRatio: info.resamplingRatio,
        bufferSize: info.bufferSize,
        latency: info.latency
    });
});
```

### No Audio Input/Output

**Symptoms:**
- No audio is captured from microphone
- No sound plays from speakers
- Audio level indicators stay at zero

**Common Causes and Solutions:**

#### 1. Microphone Permission Denied

```typescript
try {
    await client.startAudioRecording();
} catch (error) {
    if (error.name === 'NotAllowedError') {
        // User denied permission
        alert('Please allow microphone access to use voice features');
        
        // Show instructions for re-enabling
        showMicrophoneInstructions();
    } else if (error.name === 'NotFoundError') {
        // No microphone available
        alert('No microphone found. Please connect a microphone.');
    }
}
```

#### 2. HTTPS Required

Browsers require HTTPS for microphone access:

```typescript
// Check if running on HTTPS
if (location.protocol !== 'https:' && location.hostname !== 'localhost') {
    console.error('HTTPS is required for microphone access');
    alert('This site must be served over HTTPS to use voice features');
}
```

#### 3. AudioContext State Issues

```typescript
// Ensure AudioContext is running
const audioService = AudioService.getInstance();
const context = audioService.getAudioContext();

if (context.state === 'suspended') {
    // Resume on user interaction
    document.addEventListener('click', async () => {
        await context.resume();
        console.log('AudioContext resumed');
    }, { once: true });
}
```

#### 4. Browser Autoplay Policy

Modern browsers block autoplay. Handle this gracefully:

```typescript
// Attempt to play audio
try {
    await client.startAudioOutput();
} catch (error) {
    if (error.name === 'NotAllowedError') {
        // Show a button for user to click
        showPlayButton(() => client.startAudioOutput());
    }
}
```

### Browser Compatibility Issues

**Different browsers have different audio capabilities and quirks:**

#### Chrome/Edge (Chromium)
- Native sample rates: Usually 48000Hz
- Best AudioWorklet support
- Requires HTTPS for microphone

```typescript
const isChromium = /Chrome|Chromium|Edge/.test(navigator.userAgent);
if (isChromium) {
    // Chromium-specific optimizations
    audioConfig.latencyHint = 'interactive';
}
```

#### Firefox
- Native sample rates: Can vary (44100Hz or 48000Hz)
- Good AudioWorklet support (version 76+)
- May need user interaction for AudioContext

```typescript
const isFirefox = /Firefox/.test(navigator.userAgent);
if (isFirefox) {
    // Firefox-specific handling
    audioConfig.echoCancellation = true;  // Firefox handles this well
}
```

#### Safari
- Native sample rates: Often 44100Hz
- AudioWorklet support from version 14.1+
- Stricter autoplay policies

```typescript
const isSafari = /^((?!chrome|android).)*safari/i.test(navigator.userAgent);
if (isSafari) {
    // Safari-specific handling
    audioConfig.latencyHint = 'playback';  // More stable on Safari
    
    // Check version for AudioWorklet support
    const version = parseFloat(navigator.userAgent.match(/Version\\/([\\d.]+)/)?.[1] || '0');
    if (version < 14.1) {
        console.warn('AudioWorklet not supported. Audio features may not work correctly.');
    }
}
```

## AudioWorklet Deployment

### Development Setup

1. **Create worklets directory in your public folder:**

```bash
mkdir -p public/worklets
```

2. **Copy the AudioWorklet file:**

```bash
cp node_modules/@agentc/realtime-core/dist/worklets/audio-processor.worklet.js public/worklets/
```

3. **Add to .gitignore if needed:**

```bash
# .gitignore
public/worklets/*.js
!public/worklets/.gitkeep
```

4. **Create a setup script:**

```json
// package.json
{
  "scripts": {
    "setup:audio": "cp node_modules/@agentc/realtime-core/dist/worklets/audio-processor.worklet.js public/worklets/",
    "postinstall": "npm run setup:audio"
  }
}
```

### Production Deployment

#### Option 1: Bundle with Application

Include the worklet in your build output:

```javascript
// webpack.config.js or next.config.js
const CopyPlugin = require('copy-webpack-plugin');

module.exports = {
  plugins: [
    new CopyPlugin({
      patterns: [
        {
          from: 'node_modules/@agentc/realtime-core/dist/worklets',
          to: 'worklets'
        }
      ]
    })
  ]
};
```

#### Option 2: Serve from CDN

```typescript
const client = new RealtimeClient({
    audioConfig: {
        workletPath: process.env.REACT_APP_CDN_URL + '/worklets/audio-processor.worklet.js'
    }
});
```

#### Option 3: Inline Worklet (Advanced)

For special deployment scenarios, you can inline the worklet:

```typescript
import audioWorkletCode from '@agentc/realtime-core/dist/worklets/audio-processor.worklet.js?raw';

const blob = new Blob([audioWorkletCode], { type: 'application/javascript' });
const workletUrl = URL.createObjectURL(blob);

const client = new RealtimeClient({
    audioConfig: {
        workletPath: workletUrl
    }
});
```

## Sample Rate Configuration

### Understanding the Audio Pipeline

```
Microphone (Native Rate) → Browser Capture → Resampling → 16kHz → Server
                48kHz         Float32         Worklet     PCM16

Server → 16kHz PCM16 → Decoding → Resampling → Speaker (Native Rate)
                        Worklet    If needed      48kHz
```

### Configuration Options

```typescript
const client = new RealtimeClient({
    audioConfig: {
        // Target sample rate (what the server expects)
        sampleRate: 16000,
        
        // Browser native sample rate (auto-detected if not set)
        // forceNativeSampleRate: 48000,
        
        // Resampling quality (default: 'medium')
        resamplingQuality: 'high', // 'low' | 'medium' | 'high'
        
        // Buffer size for processing (affects latency)
        bufferSize: 2048, // Lower = less latency, higher = more stable
        
        // Audio constraints for getUserMedia
        constraints: {
            sampleRate: { ideal: 16000 }, // Hint to browser
            echoCancellation: true,
            noiseSuppression: true,
            autoGainControl: true
        }
    }
});
```

### Sample Rate Detection

```typescript
// Get actual sample rates being used
async function detectAudioCapabilities() {
    const audioContext = new AudioContext();
    
    console.log('Native sample rate:', audioContext.sampleRate);
    
    // Test microphone sample rate
    try {
        const stream = await navigator.mediaDevices.getUserMedia({ 
            audio: { sampleRate: { ideal: 16000 } } 
        });
        
        const source = audioContext.createMediaStreamSource(stream);
        console.log('Microphone sample rate:', source.mediaStream.getAudioTracks()[0].getSettings().sampleRate);
        
        // Cleanup
        stream.getTracks().forEach(track => track.stop());
    } catch (error) {
        console.error('Microphone test failed:', error);
    }
    
    audioContext.close();
}

// Run detection
detectAudioCapabilities();
```

## Debugging Tools

### Enable Comprehensive Logging

```typescript
const client = new RealtimeClient({
    debug: true,
    audioConfig: {
        debugAudio: true,
        logLevel: 'verbose' // 'error' | 'warn' | 'info' | 'verbose'
    }
});

// Subscribe to all audio events for debugging
[
    'audio:input:start',
    'audio:input:stop',
    'audio:input:error',
    'audio:output:start',
    'audio:output:stop',
    'audio:output:error',
    'audio:level',
    'audio:config',
    'audio:diagnostic'
].forEach(event => {
    client.on(event, (data) => {
        console.log(`[AUDIO DEBUG] ${event}:`, data);
    });
});
```

### Audio System Health Check

```typescript
class AudioHealthCheck {
    static async runDiagnostics() {
        const results = {
            microphone: false,
            speakers: false,
            audioContext: false,
            worklet: false,
            sampleRate: 0,
            latency: 0
        };
        
        try {
            // Test AudioContext
            const ctx = new AudioContext();
            results.audioContext = true;
            results.sampleRate = ctx.sampleRate;
            results.latency = ctx.baseLatency || ctx.outputLatency || 0;
            
            // Test AudioWorklet
            try {
                await ctx.audioWorklet.addModule('/worklets/audio-processor.worklet.js');
                results.worklet = true;
            } catch (e) {
                console.error('Worklet test failed:', e);
            }
            
            // Test microphone
            try {
                const stream = await navigator.mediaDevices.getUserMedia({ audio: true });
                results.microphone = true;
                stream.getTracks().forEach(t => t.stop());
            } catch (e) {
                console.error('Microphone test failed:', e);
            }
            
            // Test speakers (create a silent oscillator)
            const oscillator = ctx.createOscillator();
            const gain = ctx.createGain();
            gain.gain.value = 0;
            oscillator.connect(gain);
            gain.connect(ctx.destination);
            oscillator.start();
            oscillator.stop(ctx.currentTime + 0.1);
            results.speakers = true;
            
            await new Promise(r => setTimeout(r, 200));
            ctx.close();
            
        } catch (error) {
            console.error('Audio diagnostics failed:', error);
        }
        
        return results;
    }
}

// Run health check
const health = await AudioHealthCheck.runDiagnostics();
console.log('Audio System Health:', health);
```

### Network Audio Monitor

```typescript
class AudioNetworkMonitor {
    private bytesSent = 0;
    private bytesReceived = 0;
    private startTime = Date.now();
    
    constructor(client: RealtimeClient) {
        // Monitor outgoing audio
        client.on('audio:input:chunk', (chunk) => {
            this.bytesSent += chunk.byteLength;
        });
        
        // Monitor incoming audio
        client.on('audio:output', (chunk) => {
            this.bytesReceived += chunk.byteLength;
        });
    }
    
    getStats() {
        const elapsed = (Date.now() - this.startTime) / 1000;
        
        return {
            uploadKbps: (this.bytesSent * 8) / 1000 / elapsed,
            downloadKbps: (this.bytesReceived * 8) / 1000 / elapsed,
            totalSentMB: this.bytesSent / 1024 / 1024,
            totalReceivedMB: this.bytesReceived / 1024 / 1024
        };
    }
}
```

## Production Checklist

### Pre-Deployment Audio Checklist

- [ ] **AudioWorklet file is deployed and accessible**
  ```bash
  curl https://yourapp.com/worklets/audio-processor.worklet.js
  ```

- [ ] **HTTPS is configured for production**
  ```javascript
  if (location.protocol !== 'https:') {
      console.warn('Audio features require HTTPS');
  }
  ```

- [ ] **Error handlers are in place**
  ```typescript
  client.on('audio:input:error', handleAudioError);
  client.on('audio:output:error', handleAudioError);
  ```

- [ ] **Fallback for unsupported browsers**
  ```typescript
  if (!window.AudioWorklet) {
      showTextOnlyInterface();
  }
  ```

- [ ] **Sample rate configuration is correct**
  ```typescript
  audioConfig: {
      sampleRate: 16000,  // Must match server expectation
  }
  ```

- [ ] **User permissions flow is implemented**
  ```typescript
  // Clear UI flow for requesting microphone permission
  ```

- [ ] **Audio level indicators work**
  ```typescript
  client.on('audio:level', updateVolumeIndicator);
  ```

- [ ] **Network error recovery**
  ```typescript
  client.on('disconnected', handleDisconnection);
  client.on('reconnected', resumeAudio);
  ```

### Testing Different Scenarios

```typescript
// Test suite for audio system
describe('Audio System Tests', () => {
    test('Worklet loads successfully', async () => {
        const response = await fetch('/worklets/audio-processor.worklet.js');
        expect(response.ok).toBe(true);
    });
    
    test('Handles permission denial gracefully', async () => {
        // Mock permission denial
        navigator.mediaDevices.getUserMedia = jest.fn()
            .mockRejectedValue(new DOMException('', 'NotAllowedError'));
        
        // Should show appropriate UI
        await expect(client.startAudioRecording()).rejects.toThrow();
    });
    
    test('Resampling works correctly', () => {
        // Test resampling from 48kHz to 16kHz
        const input = new Float32Array(48000); // 1 second at 48kHz
        const output = resample(input, 48000, 16000);
        expect(output.length).toBe(16000); // Should be 1 second at 16kHz
    });
});
```

## Getting Help

If you continue to experience issues after following this guide:

1. **Enable debug logging** and collect console output
2. **Run the audio health check** and share results
3. **Check browser console** for specific error messages
4. **Test with the demo application** to isolate issues
5. **Report issues** with:
   - Browser name and version
   - Operating system
   - Console errors
   - Network tab showing 404s or failed requests
   - Audio health check results

Remember: Most audio issues are related to:
- Missing AudioWorklet file (404)
- Sample rate mismatches
- Browser permissions
- HTTPS requirements

Following this guide should resolve 95% of audio-related issues.