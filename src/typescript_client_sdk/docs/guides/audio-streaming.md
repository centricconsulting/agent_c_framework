# Audio Streaming Guide

This guide covers binary audio streaming with the Agent C Realtime SDK, including microphone capture, PCM16 format, network optimization, and troubleshooting.

## Overview

The Agent C SDK uses binary WebSocket frames for audio streaming, providing:

- **33% bandwidth savings** over base64 encoding
- **Low latency** real-time streaming
- **PCM16 format** for compatibility
- **Turn-aware streaming** to prevent talk-over

## Audio Architecture

```
Microphone → Web Audio API → AudioWorklet → Resampling → PCM16 → WebSocket → Server
   (Native Rate)              (Float32)      (to 16kHz)   (Binary)
                                                              ↓
Speaker ← Web Audio API ← PCM16 ← WebSocket ← Server Response
   (Native Rate)         (16kHz)              (Binary Frames)
```

### Key Components

- **AudioWorklet**: Processes audio off the main thread for better performance
- **Resampling**: Automatically converts browser native sample rate (e.g., 48kHz) to 16kHz
- **PCM16 Format**: 16-bit signed integers for efficient transmission
- **Binary WebSocket**: Direct ArrayBuffer transmission for 33% bandwidth savings

## Getting Started

### Prerequisites

**Important:** The AudioWorklet file must be deployed to your application's public directory.

1. **Copy the AudioWorklet file to your public directory:**

   ```bash
   # For Next.js, React, Vue, or similar frameworks
   cp node_modules/@agentc/realtime-core/dist/worklets/audio-processor.worklet.js public/worklets/
   ```

2. **Verify the file is accessible:**
   
   Navigate to `http://localhost:3000/worklets/audio-processor.worklet.js` in your browser.
   You should see JavaScript code, not a 404 error.

3. **For production builds, ensure the worklet is included:**

   ```json
   // package.json
   {
     "scripts": {
       "postinstall": "cp node_modules/@agentc/realtime-core/dist/worklets/audio-processor.worklet.js public/worklets/"
     }
   }
   ```

### Basic Audio Setup

```typescript
import { RealtimeClient, AuthManager } from '@agentc/realtime-core';

// First authenticate with Agent C
const authManager = new AuthManager({
  apiUrl: 'https://localhost:8000'
});

// Login with username/password
const loginResponse = await authManager.login('username', 'password');

// Create client with WebSocket URL from login response
const client = new RealtimeClient({
  apiUrl: loginResponse.websocketUrl,  // URL provided by login
  authManager,
  enableAudio: true,
  audioConfig: {
    enableInput: true,      // Enable microphone
    enableOutput: true,     // Enable speakers
    sampleRate: 16000,      // 16 kHz sampling
    chunkDuration: 100,     // 100ms chunks
    respectTurnState: true  // Respect turn management
  }
});

await client.connect();

// Start audio capture
await client.startAudioRecording();

// Start streaming to server
client.startAudioStreaming();
```

## Audio Input (Microphone)

### Requesting Microphone Permission

```typescript
async function setupMicrophone() {
  try {
    await client.startAudioRecording();
    console.log('Microphone access granted');
  } catch (error) {
    if (error.name === 'NotAllowedError') {
      console.error('User denied microphone access');
      // Show UI to explain why microphone is needed
    } else if (error.name === 'NotFoundError') {
      console.error('No microphone found');
      // Show UI to connect a microphone
    } else {
      console.error('Microphone error:', error);
    }
  }
}
```

### Audio Processing Pipeline

```typescript
// The SDK handles this internally, but here's what happens:

// 1. Capture audio from microphone
const stream = await navigator.mediaDevices.getUserMedia({ 
  audio: {
    echoCancellation: true,
    noiseSuppression: true,
    autoGainControl: true
  } 
});

// 2. Process with AudioWorklet (runs on separate thread)
class AudioProcessor extends AudioWorkletProcessor {
  constructor() {
    super();
    // The worklet handles resampling automatically
    this.resampler = new Resampler(sampleRate, 16000); // Browser rate to 16kHz
  }
  
  process(inputs, outputs, parameters) {
    const input = inputs[0];
    if (input.length > 0) {
      const channelData = input[0]; // Mono channel
      
      // Resample to 16kHz if needed
      const resampled = this.resampler.process(channelData);
      
      // Convert Float32 to PCM16
      const pcm16 = this.float32ToPCM16(resampled);
      
      // Send to main thread
      this.port.postMessage({ audioData: pcm16 });
    }
    return true;
  }
  
  float32ToPCM16(float32Array) {
    const pcm16 = new Int16Array(float32Array.length);
    for (let i = 0; i < float32Array.length; i++) {
      // Clamp to [-1, 1] and convert to 16-bit
      const s = Math.max(-1, Math.min(1, float32Array[i]));
      pcm16[i] = s * 0x7FFF;
    }
    return pcm16;
  }
}
```

### Monitoring Audio Levels

```typescript
import { AudioService } from '@agentc/realtime-core';

const audioService = AudioService.getInstance();

// Monitor audio levels
audioService.on('audio:level', (level) => {
  // level is between 0 and 1
  const percentage = Math.round(level * 100);
  console.log(`Audio level: ${percentage}%`);
  
  // Update UI level meter
  document.getElementById('level-meter').style.width = `${percentage}%`;
  
  // Detect clipping
  if (level > 0.95) {
    console.warn('Audio might be clipping, reduce input gain');
  }
  
  // Detect silence
  if (level < 0.01) {
    console.log('Silent or no audio');
  }
});
```

### Voice Activity Detection (VAD)

```typescript
class VoiceActivityDetector {
  private silenceThreshold = 0.01;
  private silenceDuration = 1000; // 1 second
  private silenceTimer: NodeJS.Timeout | null = null;
  private isSpeaking = false;
  
  constructor(audioService: AudioService) {
    audioService.on('audio:level', (level) => {
      this.processLevel(level);
    });
  }
  
  private processLevel(level: number) {
    if (level > this.silenceThreshold) {
      // User is speaking
      if (!this.isSpeaking) {
        this.onSpeechStart();
      }
      this.isSpeaking = true;
      
      // Clear silence timer
      if (this.silenceTimer) {
        clearTimeout(this.silenceTimer);
        this.silenceTimer = null;
      }
    } else {
      // Possible silence
      if (this.isSpeaking && !this.silenceTimer) {
        // Start silence timer
        this.silenceTimer = setTimeout(() => {
          this.onSpeechEnd();
          this.isSpeaking = false;
          this.silenceTimer = null;
        }, this.silenceDuration);
      }
    }
  }
  
  private onSpeechStart() {
    console.log('Speech started');
    // Could trigger UI updates or start recording
  }
  
  private onSpeechEnd() {
    console.log('Speech ended');
    // Could stop recording or send end-of-speech signal
  }
}
```

## Audio Output (Speakers)

### Handling Incoming Audio

```typescript
// The SDK handles this automatically
client.on('audio:output', (audioData: ArrayBuffer) => {
  // Audio is automatically queued and played
  console.log(`Received ${audioData.byteLength} bytes of audio`);
});

// Monitor playback status
import { AudioOutputService } from '@agentc/realtime-core';

const audioOutput = AudioOutputService.getInstance();

audioOutput.on('audio:play-started', () => {
  console.log('Started playing audio');
});

audioOutput.on('audio:play-stopped', () => {
  console.log('Stopped playing audio');
});
```

### Volume Control

```typescript
// Set initial volume
const audioOutput = AudioOutputService.getInstance();
audioOutput.setVolume(0.8); // 80% volume

// Create volume slider
function createVolumeControl() {
  const slider = document.getElementById('volume-slider') as HTMLInputElement;
  
  slider.addEventListener('input', (e) => {
    const volume = parseFloat((e.target as HTMLInputElement).value);
    audioOutput.setVolume(volume);
    
    // Save preference
    localStorage.setItem('audioVolume', volume.toString());
  });
  
  // Load saved volume
  const savedVolume = localStorage.getItem('audioVolume');
  if (savedVolume) {
    const volume = parseFloat(savedVolume);
    audioOutput.setVolume(volume);
    slider.value = savedVolume;
  }
}
```

### Audio Queue Management

```typescript
// Monitor audio queue for smooth playback
const audioOutput = AudioOutputService.getInstance();

setInterval(() => {
  const status = audioOutput.getStatus();
  console.log(`Audio queue: ${status.queueLength} chunks`);
  
  if (status.queueLength > 20) {
    console.warn('Large audio buffer, possible latency');
    // Consider clearing old chunks
    audioOutput.clearBuffers();
  }
  
  if (status.queueLength === 0 && status.isPlaying) {
    console.warn('Audio buffer underrun');
    // Network might be slow
  }
}, 1000);
```

## PCM16 Format

### Understanding PCM16

PCM16 (Pulse Code Modulation 16-bit) characteristics:

- **Sample Rate**: 16,000 Hz (16 kHz)
- **Bit Depth**: 16 bits per sample
- **Channels**: 1 (Mono)
- **Byte Order**: Little-endian
- **Signed**: Yes (range: -32768 to 32767)

### Binary Format Efficiency

```typescript
// Base64 encoding (traditional approach)
const base64Size = Math.ceil(pcm16Data.byteLength * 4/3);
// 1000 bytes → 1334 bytes (33% overhead)

// Binary transmission (our approach)
const binarySize = pcm16Data.byteLength;
// 1000 bytes → 1000 bytes (0% overhead)

// Bandwidth savings calculation
const savings = ((base64Size - binarySize) / base64Size) * 100;
console.log(`Bandwidth saved: ${savings}%`); // 25% saved
```

### Manual PCM16 Conversion

```typescript
// Convert Float32 (Web Audio) to PCM16
function float32ToPCM16(float32Array: Float32Array): Int16Array {
  const pcm16 = new Int16Array(float32Array.length);
  
  for (let i = 0; i < float32Array.length; i++) {
    // Clamp to [-1, 1]
    let sample = Math.max(-1, Math.min(1, float32Array[i]));
    
    // Scale to 16-bit range
    // -1 → -32768, 1 → 32767
    pcm16[i] = Math.floor(sample * 32767);
  }
  
  return pcm16;
}

// Convert PCM16 to Float32 (for playback)
function pcm16ToFloat32(pcm16Array: Int16Array): Float32Array {
  const float32 = new Float32Array(pcm16Array.length);
  
  for (let i = 0; i < pcm16Array.length; i++) {
    // Scale from 16-bit to [-1, 1]
    float32[i] = pcm16Array[i] / 32768;
  }
  
  return float32;
}
```

## Turn Management Integration

### Automatic Turn-Based Streaming

```typescript
const client = new RealtimeClient({
  audioConfig: {
    respectTurnState: true  // Enable turn management
  }
});

// Audio automatically streams only during user turns
const turnManager = client.getTurnManager();

turnManager.on('turn:user_start', () => {
  console.log('Your turn - audio streaming enabled');
  // SDK automatically starts streaming if recording
});

turnManager.on('turn:user_end', () => {
  console.log('Agent turn - audio streaming paused');
  // SDK automatically stops streaming
});
```

### Manual Turn Override

```typescript
// For testing or special cases
const audioBridge = AudioAgentCBridge.getInstance();

// Disable turn checking temporarily
audioBridge.setRespectTurnState(false);

// Stream regardless of turn
audioBridge.startStreaming();

// Re-enable turn checking
audioBridge.setRespectTurnState(true);
```

## Network Optimization

### Chunk Size Configuration

```typescript
// Smaller chunks = lower latency but more overhead
// Larger chunks = higher latency but less overhead

const client = new RealtimeClient({
  audioConfig: {
    chunkDuration: 100  // 100ms chunks (balanced)
    // chunkDuration: 50   // 50ms chunks (low latency)
    // chunkDuration: 200  // 200ms chunks (efficient)
  }
});

// Calculate chunk size in bytes
const sampleRate = 16000;
const bytesPerSample = 2; // 16-bit = 2 bytes
const chunkDuration = 100; // ms

const samplesPerChunk = (sampleRate * chunkDuration) / 1000;
const bytesPerChunk = samplesPerChunk * bytesPerSample;
console.log(`Chunk size: ${bytesPerChunk} bytes`); // 3200 bytes
```

### Buffering Strategy

```typescript
class AdaptiveBuffer {
  private targetLatency = 200; // Target 200ms latency
  private minBuffer = 100;     // Minimum 100ms buffer
  private maxBuffer = 500;     // Maximum 500ms buffer
  
  adjustBuffer(networkLatency: number) {
    // Adjust buffer based on network conditions
    const bufferSize = Math.min(
      this.maxBuffer,
      Math.max(this.minBuffer, networkLatency * 2)
    );
    
    return bufferSize;
  }
}
```

### Bandwidth Monitoring

```typescript
class BandwidthMonitor {
  private bytesSent = 0;
  private startTime = Date.now();
  
  onDataSent(bytes: number) {
    this.bytesSent += bytes;
  }
  
  getBandwidthKbps(): number {
    const elapsed = (Date.now() - this.startTime) / 1000;
    const kbps = (this.bytesSent * 8) / 1000 / elapsed;
    return kbps;
  }
  
  getExpectedBandwidth(): number {
    // PCM16 at 16kHz mono
    const bitsPerSecond = 16000 * 16 * 1;
    return bitsPerSecond / 1000; // 256 kbps
  }
}
```

## Advanced Features

### Echo Cancellation

```typescript
// Configure echo cancellation
const constraints = {
  audio: {
    echoCancellation: true,
    noiseSuppression: true,
    autoGainControl: true,
    sampleRate: 16000
  }
};

// SDK handles this internally, but you can customize
const stream = await navigator.mediaDevices.getUserMedia(constraints);
```

### Noise Gate

```typescript
class NoiseGate {
  private threshold = 0.01;
  
  process(audioData: Int16Array): Int16Array {
    const processed = new Int16Array(audioData.length);
    
    for (let i = 0; i < audioData.length; i++) {
      const sample = audioData[i] / 32768;
      
      if (Math.abs(sample) < this.threshold) {
        processed[i] = 0; // Gate closed
      } else {
        processed[i] = audioData[i]; // Gate open
      }
    }
    
    return processed;
  }
}
```

### Audio Compression

```typescript
class SimpleCompressor {
  private threshold = 0.7;
  private ratio = 4; // 4:1 compression
  
  process(audioData: Float32Array): Float32Array {
    const compressed = new Float32Array(audioData.length);
    
    for (let i = 0; i < audioData.length; i++) {
      const sample = audioData[i];
      const abs = Math.abs(sample);
      
      if (abs > this.threshold) {
        // Apply compression
        const excess = abs - this.threshold;
        const compressedExcess = excess / this.ratio;
        const newAbs = this.threshold + compressedExcess;
        compressed[i] = sample > 0 ? newAbs : -newAbs;
      } else {
        compressed[i] = sample;
      }
    }
    
    return compressed;
  }
}
```

## Testing Audio

### Audio Test Suite

```typescript
class AudioTester {
  async testMicrophone(): Promise<boolean> {
    try {
      const stream = await navigator.mediaDevices.getUserMedia({ audio: true });
      
      // Test audio level detection
      const audioContext = new AudioContext();
      const source = audioContext.createMediaStreamSource(stream);
      const analyser = audioContext.createAnalyser();
      source.connect(analyser);
      
      const dataArray = new Uint8Array(analyser.frequencyBinCount);
      analyser.getByteFrequencyData(dataArray);
      
      // Check for audio activity
      const average = dataArray.reduce((a, b) => a + b) / dataArray.length;
      
      // Cleanup
      stream.getTracks().forEach(track => track.stop());
      audioContext.close();
      
      return average > 0;
    } catch {
      return false;
    }
  }
  
  async testSpeakers(): Promise<boolean> {
    try {
      const audioContext = new AudioContext();
      
      // Create test tone
      const oscillator = audioContext.createOscillator();
      const gainNode = audioContext.createGain();
      
      oscillator.connect(gainNode);
      gainNode.connect(audioContext.destination);
      
      gainNode.gain.value = 0.1; // Low volume
      oscillator.frequency.value = 440; // A4 note
      
      oscillator.start();
      oscillator.stop(audioContext.currentTime + 0.1); // 100ms beep
      
      return true;
    } catch {
      return false;
    }
  }
}
```

### Generate Test Audio

```typescript
function generateTestTone(
  frequency: number = 440,
  duration: number = 1000,
  sampleRate: number = 16000
): Int16Array {
  const samples = (sampleRate * duration) / 1000;
  const audioData = new Int16Array(samples);
  
  for (let i = 0; i < samples; i++) {
    const t = i / sampleRate;
    const sample = Math.sin(2 * Math.PI * frequency * t);
    audioData[i] = sample * 32767;
  }
  
  return audioData;
}

// Send test tone
const testTone = generateTestTone(440, 1000);
client.sendBinaryFrame(testTone.buffer);
```

## Troubleshooting

**📚 Comprehensive Troubleshooting Guide**  
For detailed solutions to AudioWorklet deployment, sample rate issues, and browser compatibility problems, see our [Audio Troubleshooting Guide](./audio-troubleshooting.md).

### Common Issues

**AudioWorklet 404 Error:**

```typescript
// Ensure the worklet file is in your public directory
// public/worklets/audio-processor.worklet.js

// Or specify a custom path
const client = new RealtimeClient({
  audioConfig: {
    workletPath: '/custom/path/to/audio-processor.worklet.js'
  }
});
```

**Sample Rate Mismatch:**

```typescript
// The SDK automatically handles resampling
// Browser native rate (e.g., 48kHz) → Resampled to 16kHz → Server

// To debug sample rate issues:
client.on('audio:config', (config) => {
  console.log('Browser native rate:', config.nativeSampleRate);
  console.log('Target rate:', config.targetSampleRate);
  console.log('Resampling:', config.isResampling);
});
```

**No audio input detected:**

```typescript
// Check microphone permission
navigator.permissions.query({ name: 'microphone' }).then(result => {
  console.log('Microphone permission:', result.state);
  
  if (result.state === 'denied') {
    // Show instructions to enable microphone
  }
});

// Check for available devices
navigator.mediaDevices.enumerateDevices().then(devices => {
  const mics = devices.filter(d => d.kind === 'audioinput');
  console.log('Available microphones:', mics.length);
});
```

**Audio cutting out:**

```typescript
// Monitor connection stability
client.on('disconnected', () => {
  console.log('Connection lost - audio will stop');
});

client.on('reconnected', () => {
  console.log('Reconnected - resuming audio');
  client.startAudioRecording();
  client.startAudioStreaming();
});
```

**Echo or feedback:**

```typescript
// Ensure echo cancellation is enabled
const constraints = {
  audio: {
    echoCancellation: true,
    noiseSuppression: true
  }
};

// Use headphones to prevent feedback
if (detectSpeakerFeedback()) {
  alert('Please use headphones to prevent echo');
}
```

**High latency:**

```typescript
// Monitor and log latency
class LatencyMonitor {
  private sendTime = 0;
  
  onSend() {
    this.sendTime = Date.now();
  }
  
  onResponse() {
    const latency = Date.now() - this.sendTime;
    console.log(`Round-trip latency: ${latency}ms`);
    
    if (latency > 500) {
      console.warn('High latency detected');
      // Consider reducing chunk size or checking network
    }
  }
}
```

### Debug Logging

```typescript
// Enable detailed audio logging
const client = new RealtimeClient({
  debug: true,
  audioConfig: {
    enableInput: true,
    enableOutput: true
  }
});

// Monitor all audio events
const audioService = AudioService.getInstance();

audioService.on('audio:chunk', (data) => {
  console.log('Audio chunk:', data.length, 'samples');
});

audioService.on('audio:error', (error) => {
  console.error('Audio error:', error);
});

const audioOutput = AudioOutputService.getInstance();

audioOutput.on('audio:chunk-played', (info) => {
  console.log('Played chunk:', info.duration, 'seconds');
});
```

## Performance Optimization

### CPU Usage

```typescript
// Monitor audio processing CPU usage
class CPUMonitor {
  private lastTime = performance.now();
  private lastCPU = 0;
  
  measure(callback: () => void) {
    const startTime = performance.now();
    callback();
    const endTime = performance.now();
    
    const elapsed = endTime - startTime;
    console.log(`Audio processing took ${elapsed.toFixed(2)}ms`);
    
    if (elapsed > 10) {
      console.warn('High CPU usage in audio processing');
    }
  }
}
```

### Memory Management

```typescript
// Prevent memory leaks in audio buffers
class AudioBufferPool {
  private pool: ArrayBuffer[] = [];
  private maxSize = 10;
  
  get(): ArrayBuffer {
    return this.pool.pop() || new ArrayBuffer(3200);
  }
  
  release(buffer: ArrayBuffer) {
    if (this.pool.length < this.maxSize) {
      this.pool.push(buffer);
    }
  }
}
```

## Best Practices

1. **Always handle permission errors gracefully**
2. **Use binary transmission for efficiency**
3. **Monitor audio levels to prevent clipping**
4. **Implement echo cancellation**
5. **Respect turn management**
6. **Buffer appropriately for network conditions**
7. **Test on various devices and networks**
8. **Provide visual feedback for audio state**
9. **Clean up resources properly**
10. **Log audio metrics for debugging**