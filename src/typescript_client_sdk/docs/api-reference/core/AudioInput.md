# AudioInput API Reference

The `AudioInput` system manages microphone capture, audio processing, and streaming to the Agent C server. It consists of multiple components working together.

## Components Overview

The audio input system consists of three main components:
- **AudioService** - Manages microphone access and Web Audio API
- **AudioProcessor** - AudioWorklet for real-time audio processing
- **AudioAgentCBridge** - Bridges audio data to the WebSocket connection

## Import

```typescript
import { AudioService, AudioAgentCBridge } from '@agentc/realtime-core';
```

## AudioService

The main service for managing microphone input and audio processing.

### Singleton Access

```typescript
static getInstance(): AudioService
```

Gets the singleton AudioService instance.

**Example:**
```typescript
const audioService = AudioService.getInstance();
```

### Methods

#### startRecording()

Starts recording from the microphone.

```typescript
async startRecording(): Promise<void>
```

**Returns:** Promise that resolves when recording starts

**Throws:** 
- `NotAllowedError` - Microphone permission denied
- `NotFoundError` - No microphone found
- `Error` - Other initialization errors

**Example:**
```typescript
try {
  await audioService.startRecording();
  console.log('Recording started');
} catch (error) {
  if (error.name === 'NotAllowedError') {
    console.error('Microphone permission denied');
  }
}
```

#### stopRecording()

Stops microphone recording.

```typescript
stopRecording(): void
```

**Example:**
```typescript
audioService.stopRecording();
console.log('Recording stopped');
```

#### getStatus()

Gets the current audio service status.

```typescript
getStatus(): AudioServiceStatus
```

**Returns:** Status object

```typescript
interface AudioServiceStatus {
  state: 'idle' | 'initializing' | 'recording' | 'permission-denied' | 'error';
  isRecording: boolean;
  hasPermission: boolean;
  audioLevel: number;      // Current level (0-1)
  sampleRate: number;      // Audio sample rate
  error?: string;          // Error message if any
}
```

**Example:**
```typescript
const status = audioService.getStatus();
console.log(`State: ${status.state}`);
console.log(`Recording: ${status.isRecording}`);
console.log(`Level: ${status.audioLevel}`);
```

#### setAudioProcessor()

Sets the audio processor callback.

```typescript
setAudioProcessor(processor: (audioData: Int16Array) => void): void
```

**Parameters:**
- `processor` - Callback function to process audio chunks

**Example:**
```typescript
audioService.setAudioProcessor((audioData) => {
  console.log(`Received ${audioData.length} samples`);
  // Process or send audio data
});
```

#### getAudioLevel()

Gets the current audio input level.

```typescript
getAudioLevel(): number
```

**Returns:** Audio level between 0 and 1

**Example:**
```typescript
// Monitor audio levels
setInterval(() => {
  const level = audioService.getAudioLevel();
  updateLevelMeter(level);
}, 100);
```

#### destroy()

Destroys the audio service and releases resources.

```typescript
destroy(): void
```

**Example:**
```typescript
// Clean up when done
audioService.destroy();
```

### Events

AudioService extends EventEmitter:

- `audio:chunk` - Audio chunk ready
- `audio:level` - Audio level update
- `audio:state-changed` - State changed
- `audio:error` - Error occurred

**Example:**
```typescript
audioService.on('audio:chunk', (data: Int16Array) => {
  console.log(`Audio chunk: ${data.length} samples`);
});

audioService.on('audio:level', (level: number) => {
  updateVolumeIndicator(level);
});

audioService.on('audio:state-changed', (state: string) => {
  console.log(`Audio state: ${state}`);
});

audioService.on('audio:error', (error: Error) => {
  console.error('Audio error:', error);
});
```

## AudioAgentCBridge

Bridges the audio service to the WebSocket client for streaming.

### Singleton Access

```typescript
static getInstance(): AudioAgentCBridge
```

Gets the singleton AudioAgentCBridge instance.

**Example:**
```typescript
const audioBridge = AudioAgentCBridge.getInstance();
```

### Methods

#### setClient()

Sets the RealtimeClient for WebSocket communication.

```typescript
setClient(client: RealtimeClient | null): void
```

**Parameters:**
- `client` - RealtimeClient instance or null to disconnect

**Example:**
```typescript
audioBridge.setClient(realtimeClient);
```

#### startStreaming()

Starts streaming audio to the server.

```typescript
startStreaming(): void
```

**Throws:** Error if client not set or audio not initialized

**Example:**
```typescript
// Start recording first
await audioService.startRecording();

// Then start streaming
audioBridge.startStreaming();
```

#### stopStreaming()

Stops streaming audio to the server.

```typescript
stopStreaming(): void
```

**Example:**
```typescript
audioBridge.stopStreaming();
```

#### getStatus()

Gets the bridge status.

```typescript
getStatus(): AudioBridgeStatus
```

**Returns:** Status object

```typescript
interface AudioBridgeStatus {
  isStreaming: boolean;
  respectsTurnState: boolean;
  chunksProcessed: number;
  bytesSent: number;
  lastChunkTime: number;
}
```

**Example:**
```typescript
const status = audioBridge.getStatus();
console.log(`Streaming: ${status.isStreaming}`);
console.log(`Bytes sent: ${status.bytesSent}`);
```

#### setRespectTurnState()

Sets whether to respect turn management.

```typescript
setRespectTurnState(respect: boolean): void
```

**Parameters:**
- `respect` - Whether to check turn state before streaming

**Example:**
```typescript
// Enable turn management
audioBridge.setRespectTurnState(true);

// Disable for testing
audioBridge.setRespectTurnState(false);
```

## Audio Configuration

Configure audio through RealtimeClient:

```typescript
const client = new RealtimeClient({
  enableAudio: true,
  audioConfig: {
    enableInput: true,        // Enable microphone
    sampleRate: 16000,       // Sample rate in Hz
    chunkDuration: 100,      // Chunk duration in ms
    respectTurnState: true,  // Respect turn management
    vadThreshold: 0.01       // Voice activity threshold
  }
});
```

### Configuration Options

```typescript
interface AudioConfig {
  enableInput?: boolean;      // Enable microphone (default: true)
  sampleRate?: number;        // Sample rate (default: 16000)
  chunkDuration?: number;     // Chunk size in ms (default: 100)
  respectTurnState?: boolean; // Respect turns (default: true)
  vadThreshold?: number;      // VAD threshold (default: 0.01)
  echoCancellation?: boolean; // Echo cancellation (default: true)
  noiseSuppression?: boolean; // Noise suppression (default: true)
  autoGainControl?: boolean;  // Auto gain control (default: true)
}
```

## Audio Processing Pipeline

The audio flows through this pipeline:

```
Microphone
    │
    ▼
getUserMedia (AudioService)
    │
    ▼
AudioContext (Web Audio API)
    │
    ▼
AudioWorklet (AudioProcessor)
    │
    ├─► Float32 to PCM16 conversion
    ├─► Chunking (100ms chunks)
    ├─► Level detection
    │
    ▼
AudioAgentCBridge
    │
    ├─► Turn state check (optional)
    ├─► Binary frame creation
    │
    ▼
WebSocket (binary transmission)
```

## Complete Example

```typescript
import { 
  RealtimeClient,
  AuthManager, 
  AudioService, 
  AudioAgentCBridge 
} from '@agentc/realtime-core';

async function audioInputExample() {
  // Setup authentication
  const authManager = new AuthManager({
    apiUrl: 'https://localhost:8000'
  });
  
  await authManager.login('username', 'password');
  
  // Create client with audio enabled
  const client = new RealtimeClient({
    apiUrl: 'wss://localhost:8000/rt/ws',
    authManager,
    enableAudio: true,
    audioConfig: {
      enableInput: true,
      sampleRate: 16000,
      chunkDuration: 100,
      respectTurnState: true
    }
  });
  
  // Get audio components
  const audioService = AudioService.getInstance();
  const audioBridge = AudioAgentCBridge.getInstance();
  
  // Monitor audio status
  audioService.on('audio:state-changed', (state) => {
    console.log(`Audio state: ${state}`);
    updateUI(state);
  });
  
  audioService.on('audio:level', (level) => {
    // Update level meter
    const percentage = Math.round(level * 100);
    document.getElementById('level-meter')!.style.width = `${percentage}%`;
  });
  
  audioService.on('audio:error', (error) => {
    console.error('Audio error:', error);
    showError(error.message);
  });
  
  // Monitor streaming
  audioBridge.on('audio:streaming-started', () => {
    console.log('🎤 Streaming started');
    document.getElementById('streaming-indicator')!.classList.add('active');
  });
  
  audioBridge.on('audio:streaming-stopped', () => {
    console.log('🔇 Streaming stopped');
    document.getElementById('streaming-indicator')!.classList.remove('active');
  });
  
  audioBridge.on('audio:chunk-sent', (size: number) => {
    console.log(`Sent ${size} bytes`);
  });
  
  // Connect to server
  await client.connect();
  
  // Set up audio bridge
  audioBridge.setClient(client);
  
  // UI Controls
  const recordButton = document.getElementById('record-button');
  const streamButton = document.getElementById('stream-button');
  
  recordButton?.addEventListener('click', async () => {
    const status = audioService.getStatus();
    
    if (!status.isRecording) {
      try {
        console.log('📍 Requesting microphone permission...');
        await audioService.startRecording();
        
        recordButton.textContent = 'Stop Recording';
        streamButton!.disabled = false;
        
        // Show audio stats
        setInterval(() => {
          const status = audioService.getStatus();
          const bridgeStatus = audioBridge.getStatus();
          
          console.log('📊 Audio Stats:');
          console.log(`  State: ${status.state}`);
          console.log(`  Level: ${(status.audioLevel * 100).toFixed(1)}%`);
          console.log(`  Sample Rate: ${status.sampleRate} Hz`);
          console.log(`  Streaming: ${bridgeStatus.isStreaming}`);
          console.log(`  Chunks: ${bridgeStatus.chunksProcessed}`);
          console.log(`  Bytes Sent: ${bridgeStatus.bytesSent}`);
        }, 1000);
        
      } catch (error) {
        console.error('Failed to start recording:', error);
        
        if (error.name === 'NotAllowedError') {
          alert('Microphone permission denied. Please allow microphone access.');
        } else if (error.name === 'NotFoundError') {
          alert('No microphone found. Please connect a microphone.');
        } else {
          alert(`Failed to start recording: ${error.message}`);
        }
      }
    } else {
      audioService.stopRecording();
      audioBridge.stopStreaming();
      
      recordButton.textContent = 'Start Recording';
      streamButton!.disabled = true;
    }
  });
  
  streamButton?.addEventListener('click', () => {
    const bridgeStatus = audioBridge.getStatus();
    
    if (!bridgeStatus.isStreaming) {
      // Check turn state if enabled
      const turnManager = client.getTurnManager();
      if (turnManager && !turnManager.isUserTurn) {
        console.log('⚠️ Not user turn, waiting...');
      }
      
      audioBridge.startStreaming();
      streamButton.textContent = 'Stop Streaming';
    } else {
      audioBridge.stopStreaming();
      streamButton.textContent = 'Start Streaming';
    }
  });
  
  // Turn management integration
  const turnManager = client.getTurnManager();
  if (turnManager) {
    turnManager.on('turn:user_start', () => {
      console.log('🎤 User turn - enabling streaming');
      if (audioService.getStatus().isRecording) {
        audioBridge.startStreaming();
      }
    });
    
    turnManager.on('turn:user_end', () => {
      console.log('🔇 User turn ended - stopping streaming');
      audioBridge.stopStreaming();
    });
  }
  
  // Advanced: Custom audio processing
  audioService.setAudioProcessor((audioData: Int16Array) => {
    // Custom processing before sending
    const processed = applyNoiseGate(audioData, 0.01);
    
    // Manual sending (if not using bridge)
    if (shouldSendManually) {
      client.sendBinaryFrame(processed.buffer);
    }
  });
  
  // Voice Activity Detection (VAD)
  let silenceTimer: NodeJS.Timeout | null = null;
  const SILENCE_THRESHOLD = 0.01;
  const SILENCE_DURATION = 1000; // 1 second
  
  audioService.on('audio:level', (level) => {
    if (level < SILENCE_THRESHOLD) {
      // Start silence timer
      if (!silenceTimer) {
        silenceTimer = setTimeout(() => {
          console.log('🔇 Silence detected');
          // Could stop streaming or send end-of-speech signal
        }, SILENCE_DURATION);
      }
    } else {
      // Cancel silence timer - user is speaking
      if (silenceTimer) {
        clearTimeout(silenceTimer);
        silenceTimer = null;
      }
    }
  });
  
  // Cleanup on exit
  window.addEventListener('beforeunload', () => {
    audioService.stopRecording();
    audioBridge.stopStreaming();
    audioService.destroy();
  });
  
  function updateUI(state: string) {
    const statusElement = document.getElementById('audio-status');
    if (statusElement) {
      statusElement.textContent = `Audio: ${state}`;
      statusElement.className = `status-${state}`;
    }
  }
  
  function showError(message: string) {
    const errorElement = document.getElementById('audio-error');
    if (errorElement) {
      errorElement.textContent = message;
      errorElement.style.display = 'block';
      setTimeout(() => {
        errorElement.style.display = 'none';
      }, 5000);
    }
  }
  
  function applyNoiseGate(audioData: Int16Array, threshold: number): Int16Array {
    // Simple noise gate implementation
    const result = new Int16Array(audioData.length);
    for (let i = 0; i < audioData.length; i++) {
      const sample = audioData[i] / 32768; // Normalize to -1 to 1
      if (Math.abs(sample) < threshold) {
        result[i] = 0;
      } else {
        result[i] = audioData[i];
      }
    }
    return result;
  }
}

audioInputExample().catch(console.error);
```

## Browser Requirements

- **Chrome**: 66+ (AudioWorklet support)
- **Firefox**: 76+ (AudioWorklet support)
- **Safari**: 14.1+ (AudioWorklet support)
- **Edge**: 79+ (Chromium-based)

**Required APIs:**
- getUserMedia
- Web Audio API
- AudioWorklet
- WebSocket with binary frame support

## Best Practices

1. **Always handle permission errors:**
```typescript
try {
  await audioService.startRecording();
} catch (error) {
  if (error.name === 'NotAllowedError') {
    // Show permission UI
  }
}
```

2. **Monitor audio levels:**
```typescript
audioService.on('audio:level', (level) => {
  if (level > 0.8) {
    console.warn('Audio might be clipping');
  }
});
```

3. **Respect turn management:**
```typescript
// Always use respectTurnState in production
audioBridge.setRespectTurnState(true);
```

4. **Clean up resources:**
```typescript
// Always stop recording and destroy when done
audioService.stopRecording();
audioService.destroy();
```

5. **Handle network issues:**
```typescript
client.on('disconnected', () => {
  // Stop streaming on disconnect
  audioBridge.stopStreaming();
});

client.on('reconnected', () => {
  // Resume streaming if was active
  if (audioService.getStatus().isRecording) {
    audioBridge.startStreaming();
  }
});
```

## Troubleshooting

### Common Issues

**No audio input detected:**
- Check microphone permissions in browser
- Verify microphone is connected
- Check system audio settings
- Try different browser

**Audio cutting out:**
- Check network stability
- Verify turn management settings
- Monitor audio levels for clipping
- Check CPU usage

**Permission denied:**
- Site must use HTTPS (or localhost)
- User must explicitly grant permission
- Check browser privacy settings

**Poor audio quality:**
- Adjust sample rate (higher = better quality)
- Enable echo cancellation
- Check microphone quality
- Reduce background noise

## TypeScript Types

```typescript
import {
  AudioService,
  AudioAgentCBridge,
  AudioServiceStatus,
  AudioBridgeStatus,
  AudioConfig
} from '@agentc/realtime-core';
```

All methods and properties are fully typed for TypeScript applications.