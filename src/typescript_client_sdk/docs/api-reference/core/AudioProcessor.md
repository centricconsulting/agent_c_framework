# AudioProcessor API Reference

The AudioProcessor manages audio input from the user's microphone, converting it to PCM16 format using a Web Audio API AudioWorklet for efficient real-time processing.

## Overview

The AudioProcessor provides:
- **AudioWorklet processing** - Runs in separate thread for performance
- **PCM16 conversion** - Converts browser audio to 16kHz PCM16 format
- **Microphone management** - Handles permissions and stream access
- **Real-time monitoring** - Audio level detection and statistics
- **Chunk generation** - Produces audio chunks for transmission
- **Resampling** - Handles conversion from native to target sample rate

## Configuration

### AudioProcessorConfig

```typescript
interface AudioProcessorConfig {
  workletPath: string;      // Path to worklet processor file
  sampleRate: number;       // Target sample rate (default: 16000)
  channelCount: number;     // Number of channels (default: 1)  
  bufferSize: number;       // Samples per chunk (default: 3200)
  debug?: boolean;          // Enable debug logging
}
```

### Default Configuration

```typescript
const DEFAULT_AUDIO_CONFIG = {
  workletPath: '/audio-worklet-processor.js',
  sampleRate: 16000,        // 16kHz for Agent C API
  channelCount: 1,          // Mono audio
  bufferSize: 3200         // 200ms chunks at 16kHz
};
```

## API Methods

### `constructor(config?: AudioProcessorConfig)`

Create a new AudioProcessor instance.

```typescript
import { AudioProcessor } from '@agentc/realtime-core';

const processor = new AudioProcessor({
  workletPath: '/audio-worklet-processor.js',
  sampleRate: 16000,
  debug: true
});
```

### `initialize(): Promise<void>`

Initialize the processor and request microphone access.

```typescript
try {
  await processor.initialize();
  console.log('Audio processor ready');
} catch (error) {
  console.error('Failed to initialize:', error);
}
```

### `startProcessing(): Promise<void>`

Start capturing and processing audio.

```typescript
await processor.startProcessing();
// Audio chunks will now be emitted via 'audioChunk' event
```

### `stopProcessing(): void`

Stop audio processing.

```typescript
processor.stopProcessing();
// No more audio chunks will be emitted
```

### `getStatus(): AudioProcessorStatus`

Get current processor status.

```typescript
const status = processor.getStatus();
console.log('Processing:', status.isProcessing);
console.log('Audio level:', status.audioLevel);
```

### `cleanup(): Promise<void>`

Release all resources and stop audio capture.

```typescript
await processor.cleanup();
// Microphone released, audio context closed
```

### `on(event: string, listener: Function): () => void`

Subscribe to processor events. Returns unsubscribe function.

```typescript
const unsubscribe = processor.on('audioChunk', (chunk) => {
  console.log('Audio chunk:', chunk);
});

// Later: unsubscribe
unsubscribe();
```

## Events

### `audioChunk`

Emitted when an audio chunk is ready for transmission.

```typescript
processor.on('audioChunk', (chunk: AudioChunkData) => {
  // chunk.content - ArrayBuffer of PCM16 audio
  // chunk.audio_level - Current audio level (0-100)
  // chunk.frame_count - Incrementing frame counter
  // chunk.sample_rate - Sample rate (16000)
  // chunk.sample_count - Number of samples
  
  // Send to server
  websocket.send(chunk.content);
});
```

### `levelChange`

Emitted when audio level changes.

```typescript
processor.on('levelChange', (level: number) => {
  // level: 0-100 representing audio intensity
  updateVolumeIndicator(level);
});
```

### `statusChange`

Emitted when processor status changes.

```typescript
processor.on('statusChange', (status: AudioProcessorStatus) => {
  console.log('State:', status.state);
  console.log('Processing:', status.isProcessing);
});
```

### `error`

Emitted when an error occurs.

```typescript
processor.on('error', (error: AudioProcessorError) => {
  console.error('Audio error:', error.message);
  console.error('Error code:', error.code);
});
```

## Types

### AudioProcessorStatus

```typescript
interface AudioProcessorStatus {
  state: 'idle' | 'loading' | 'ready' | 'processing' | 'error';
  isProcessing: boolean;     // Currently capturing audio
  isReady: boolean;          // Initialized and ready
  audioLevel: number;        // Current level (0-100)
  chunksProcessed: number;   // Total chunks generated
  outputSampleRate: number;  // Target sample rate
  contextSampleRate?: number; // Native browser rate
  error?: string;            // Error message if any
}
```

### AudioChunkData

```typescript
interface AudioChunkData {
  content: ArrayBuffer;      // PCM16 audio data
  content_type: 'audio/L16'; // Audio format
  audio_level: number;       // Volume level (0-100)
  frame_count: number;       // Incrementing counter
  timestamp: number;         // Unix timestamp
  sample_rate: number;       // Sample rate (16000)
  sample_count: number;      // Number of samples
}
```

### AudioProcessorError

```typescript
class AudioProcessorError extends Error {
  code: AudioProcessorErrorCode;
  originalError?: Error;
}

enum AudioProcessorErrorCode {
  NOT_SUPPORTED = 'NOT_SUPPORTED',
  MICROPHONE_ACCESS_ERROR = 'MICROPHONE_ACCESS_ERROR',
  WORKLET_LOAD_ERROR = 'WORKLET_LOAD_ERROR',
  AUDIO_CONTEXT_ERROR = 'AUDIO_CONTEXT_ERROR',
  PROCESSING_ERROR = 'PROCESSING_ERROR'
}
```

## Usage Examples

### Basic Audio Capture

```typescript
import { AudioProcessor } from '@agentc/realtime-core';

async function startAudioCapture() {
  const processor = new AudioProcessor();
  
  // Handle audio chunks
  processor.on('audioChunk', (chunk) => {
    // Send to server
    sendAudioToServer(chunk.content);
  });
  
  // Initialize and start
  await processor.initialize();
  await processor.startProcessing();
  
  return processor;
}
```

### With WebSocket Integration

```typescript
import { AudioProcessor } from '@agentc/realtime-core';

class AudioStreamManager {
  private processor: AudioProcessor;
  private websocket: WebSocket;
  
  async connect(wsUrl: string) {
    // Set up WebSocket
    this.websocket = new WebSocket(wsUrl);
    
    // Set up audio processor
    this.processor = new AudioProcessor({
      workletPath: '/audio-worklet-processor.js',
      sampleRate: 16000
    });
    
    // Stream audio chunks to server
    this.processor.on('audioChunk', (chunk) => {
      if (this.websocket.readyState === WebSocket.OPEN) {
        this.websocket.send(chunk.content);
      }
    });
    
    // Initialize audio
    await this.processor.initialize();
  }
  
  async startStreaming() {
    await this.processor.startProcessing();
  }
  
  stopStreaming() {
    this.processor.stopProcessing();
  }
}
```

### Volume Level Indicator

```typescript
import { AudioProcessor } from '@agentc/realtime-core';

class VolumeIndicator {
  private processor: AudioProcessor;
  private indicator: HTMLElement;
  
  constructor(indicatorElement: HTMLElement) {
    this.indicator = indicatorElement;
    this.processor = new AudioProcessor();
    
    // Update indicator on level changes
    this.processor.on('levelChange', (level) => {
      this.updateIndicator(level);
    });
  }
  
  private updateIndicator(level: number) {
    // Scale to percentage
    const percentage = Math.min(100, level);
    
    // Update visual indicator
    this.indicator.style.width = `${percentage}%`;
    
    // Color based on level
    if (level > 80) {
      this.indicator.style.backgroundColor = 'red';
    } else if (level > 50) {
      this.indicator.style.backgroundColor = 'yellow';
    } else {
      this.indicator.style.backgroundColor = 'green';
    }
  }
}
```

### Error Handling

```typescript
import { AudioProcessor, AudioProcessorError, AudioProcessorErrorCode } from '@agentc/realtime-core';

async function setupAudioWithFallback() {
  const processor = new AudioProcessor();
  
  processor.on('error', (error: AudioProcessorError) => {
    switch (error.code) {
      case AudioProcessorErrorCode.MICROPHONE_ACCESS_ERROR:
        showMessage('Please allow microphone access');
        break;
        
      case AudioProcessorErrorCode.NOT_SUPPORTED:
        showMessage('Your browser does not support audio processing');
        break;
        
      case AudioProcessorErrorCode.WORKLET_LOAD_ERROR:
        showMessage('Failed to load audio processor');
        break;
        
      default:
        showMessage('Audio error: ' + error.message);
    }
  });
  
  try {
    await processor.initialize();
    return processor;
  } catch (error) {
    console.error('Failed to initialize audio:', error);
    return null;
  }
}
```

### With React Hook

```typescript
import { useEffect, useState, useRef } from 'react';
import { AudioProcessor, AudioProcessorStatus } from '@agentc/realtime-core';

export function useAudioProcessor() {
  const [status, setStatus] = useState<AudioProcessorStatus>();
  const [audioLevel, setAudioLevel] = useState(0);
  const processorRef = useRef<AudioProcessor>();
  
  useEffect(() => {
    const processor = new AudioProcessor({
      workletPath: '/audio-worklet-processor.js'
    });
    
    processorRef.current = processor;
    
    // Subscribe to events
    const unsubscribeStatus = processor.on('statusChange', setStatus);
    const unsubscribeLevel = processor.on('levelChange', setAudioLevel);
    
    // Initialize
    processor.initialize().catch(console.error);
    
    return () => {
      unsubscribeStatus();
      unsubscribeLevel();
      processor.cleanup();
    };
  }, []);
  
  return {
    status,
    audioLevel,
    start: () => processorRef.current?.startProcessing(),
    stop: () => processorRef.current?.stopProcessing()
  };
}
```

## Integration Patterns

### With Turn Management

```typescript
import { AudioProcessor, TurnManager } from '@agentc/realtime-core';

class TurnAwareAudioInput {
  private processor: AudioProcessor;
  private turnManager: TurnManager;
  
  constructor(turnManager: TurnManager) {
    this.turnManager = turnManager;
    this.processor = new AudioProcessor();
    this.setupTurnHandling();
  }
  
  private setupTurnHandling() {
    // Only process audio during user turn
    this.turnManager.on('turnStart', async (turn) => {
      if (turn === 'user' && this.processor.getStatus().isReady) {
        await this.processor.startProcessing();
      }
    });
    
    this.turnManager.on('turnEnd', () => {
      this.processor.stopProcessing();
    });
  }
}
```

### With Permission Management

```typescript
class MicrophonePermissionManager {
  private processor: AudioProcessor;
  
  async requestPermission(): Promise<boolean> {
    try {
      // Create processor to trigger permission
      this.processor = new AudioProcessor();
      await this.processor.initialize();
      
      // Permission granted
      return true;
    } catch (error) {
      if (error.code === 'MICROPHONE_ACCESS_ERROR') {
        // Permission denied
        return false;
      }
      throw error;
    }
  }
  
  async checkPermission(): Promise<PermissionState> {
    const result = await navigator.permissions.query({ 
      name: 'microphone' as PermissionName 
    });
    return result.state;
  }
}
```

## Best Practices

### 1. Handle Browser Compatibility

```typescript
// Check for required APIs
if (!navigator.mediaDevices?.getUserMedia) {
  throw new Error('getUserMedia not supported');
}

if (!window.AudioContext && !window.webkitAudioContext) {
  throw new Error('AudioContext not supported');
}
```

### 2. Clean Up Resources

```typescript
// Always clean up on unmount/close
window.addEventListener('beforeunload', async () => {
  await processor.cleanup();
});

// In React components
useEffect(() => {
  return () => {
    processor.cleanup();
  };
}, []);
```

### 3. Monitor Audio Levels

```typescript
// Detect silence or no input
processor.on('levelChange', (level) => {
  if (level < 5) {
    console.log('No audio detected - check microphone');
  }
});
```

### 4. Handle Worklet Loading

```typescript
// Ensure worklet file is accessible
const processor = new AudioProcessor({
  workletPath: '/audio-worklet-processor.js'
});

// Worklet file must be served with correct MIME type
// Content-Type: application/javascript
```

## Performance Considerations

- AudioWorklet runs in separate thread for optimal performance
- Resampling from native rate (usually 48kHz) to 16kHz happens in worklet
- Buffer size affects latency vs. efficiency trade-off
- Chunk generation is optimized for network transmission

## Security Considerations

- Requires HTTPS in production (getUserMedia requirement)
- Microphone permission must be explicitly granted by user
- Audio data should be transmitted over secure WebSocket (wss://)

## Troubleshooting

### Common Issues

| Issue | Cause | Solution |
|-------|-------|----------|
| No audio chunks | Microphone muted | Check system/browser mute settings |
| Worklet load error | File not found | Verify workletPath and server config |
| Permission denied | User rejected | Show UI explaining why audio is needed |
| Audio context error | Browser limitation | Try closing other audio tabs |

### Debug Mode

```typescript
const processor = new AudioProcessor({
  debug: true  // Enables detailed logging
});
```

## Related Documentation

- [AudioOutputService](./AudioOutputService.md) - Audio output handling
- [RealtimeClient](./RealtimeClient.md) - Client integration
- [TurnManager](./TurnManager.md) - Turn-based audio control
- [WebSocketManager](./WebSocketManager.md) - Audio streaming