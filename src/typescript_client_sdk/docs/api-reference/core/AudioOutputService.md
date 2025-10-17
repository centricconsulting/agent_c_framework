# AudioOutputService API Reference

The AudioOutputService is a singleton service that manages TTS (Text-to-Speech) audio playback from the Agent C Realtime server, handling PCM16 audio chunks with voice model awareness and smooth playback.

## Overview

The AudioOutputService provides:
- **Singleton pattern** - Single instance manages all audio output
- **PCM16 audio handling** - Converts and plays 16kHz PCM16 audio chunks
- **Voice model awareness** - Automatically skips playback for avatar/none modes
- **Smooth playback** - Queues chunks with overlap to prevent gaps
- **Volume control** - Adjustable playback volume
- **Statistics tracking** - Monitor chunks received, played, and skipped
- **Status monitoring** - Real-time playback status updates

## Configuration

### Audio Format

| Property | Value | Description |
|----------|-------|-------------|
| Format | PCM16 | 16-bit linear PCM |
| Sample Rate | 16000 Hz | 16kHz audio |
| Channels | Mono | Single channel audio |
| Chunk Overlap | 10ms | Prevents playback gaps |

## API Methods

### `AudioOutputService.getInstance(): AudioOutputService`

Get the singleton instance of the service.

```typescript
import { AudioOutputService } from '@agentc/realtime-core';

const audioOutput = AudioOutputService.getInstance();
```

### `playAudioChunk(audioData: ArrayBuffer): Promise<void>`

Queue and play an audio chunk from the server.

```typescript
// Handle binary audio from WebSocket
ws.on('message', async (data: ArrayBuffer) => {
  await audioOutput.playAudioChunk(data);
});
```

### `stopPlayback(): void`

Stop current playback and clear the queue.

```typescript
// Stop all audio immediately
audioOutput.stopPlayback();
```

### `clearBuffers(): void`

Clear queued audio without stopping current playback.

```typescript
// Clear pending audio but let current chunk finish
audioOutput.clearBuffers();
```

### `setVolume(volume: number): void`

Set playback volume (0.0 to 1.0).

```typescript
// Set to 50% volume
audioOutput.setVolume(0.5);

// Mute
audioOutput.setVolume(0);

// Full volume
audioOutput.setVolume(1.0);
```

### `setVoiceModel(voiceModel: VoiceModel | null): void`

Set the current voice model to control playback behavior.

```typescript
// Normal voice playback
audioOutput.setVoiceModel({
  voice_id: 'alloy',
  name: 'Alloy',
  preview_url: '...'
});

// Avatar mode - HeyGen handles audio
audioOutput.setVoiceModel({
  voice_id: 'avatar',
  name: 'Avatar',
  preview_url: null
});

// Text-only mode - no audio playback
audioOutput.setVoiceModel({
  voice_id: 'none',
  name: 'None',
  preview_url: null
});
```

### `setEnabled(enabled: boolean): void`

Enable or disable playback globally.

```typescript
// Disable all playback
audioOutput.setEnabled(false);

// Re-enable playback
audioOutput.setEnabled(true);
```

### `getStatus(): AudioOutputStatus`

Get current playback status.

```typescript
const status = audioOutput.getStatus();
console.log('Playing:', status.isPlaying);
console.log('Queue length:', status.queueLength);
console.log('Volume:', status.volume);
```

### `onStatusChange(callback: (status: AudioOutputStatus) => void): () => void`

Subscribe to status changes. Returns an unsubscribe function.

```typescript
// Subscribe to status updates
const unsubscribe = audioOutput.onStatusChange((status) => {
  console.log('Audio status:', status);
});

// Later: unsubscribe
unsubscribe();
```

### `resetStats(): void`

Reset playback statistics.

```typescript
// Reset counters
audioOutput.resetStats();
```

### `cleanup(): Promise<void>`

Clean up resources and close audio context.

```typescript
// Clean up when done
await audioOutput.cleanup();
```

## Types

### AudioOutputStatus

```typescript
interface AudioOutputStatus {
  isPlaying: boolean;        // Currently playing audio
  isEnabled: boolean;        // Service enabled/disabled
  chunksReceived: number;    // Total chunks received
  chunksPlayed: number;      // Total chunks played
  chunksSkipped: number;     // Chunks skipped (avatar/none mode)
  queueLength: number;       // Pending chunks in queue
  volume: number;            // Current volume (0-1)
  voiceModel: VoiceModel | null;  // Current voice model
  skipPlayback: boolean;     // Whether playback is being skipped
}
```

### VoiceModel

```typescript
interface VoiceModel {
  voice_id: string;          // Voice identifier
  name: string;              // Display name
  preview_url: string | null; // Sample audio URL
}
```

## Usage Examples

### Basic Audio Playback

```typescript
import { AudioOutputService } from '@agentc/realtime-core';

const audioOutput = AudioOutputService.getInstance();

// Handle incoming audio chunks
async function handleAudioData(data: ArrayBuffer) {
  try {
    await audioOutput.playAudioChunk(data);
  } catch (error) {
    console.error('Audio playback failed:', error);
  }
}
```

### WebSocket Integration

```typescript
import { AudioOutputService } from '@agentc/realtime-core';

class WebSocketHandler {
  private audioOutput = AudioOutputService.getInstance();
  
  handleMessage(data: ArrayBuffer | string) {
    // Binary data is audio
    if (data instanceof ArrayBuffer) {
      this.audioOutput.playAudioChunk(data);
      return;
    }
    
    // Text data is JSON events
    const event = JSON.parse(data);
    this.handleEvent(event);
  }
}
```

### Voice Model Management

```typescript
import { AudioOutputService, VoiceManager } from '@agentc/realtime-core';

class AudioController {
  private audioOutput = AudioOutputService.getInstance();
  private voiceManager = new VoiceManager();
  
  async selectVoice(voiceId: string) {
    const voice = await this.voiceManager.getVoice(voiceId);
    
    // Update audio service with new voice
    this.audioOutput.setVoiceModel(voice);
    
    // Special handling for avatar mode
    if (voiceId === 'avatar') {
      console.log('Avatar mode - audio handled by HeyGen');
    } else if (voiceId === 'none') {
      console.log('Text-only mode - no audio playback');
    }
  }
}
```

### Volume Control UI

```typescript
import React from 'react';
import { AudioOutputService } from '@agentc/realtime-core';

function VolumeControl() {
  const audioOutput = AudioOutputService.getInstance();
  const [volume, setVolume] = React.useState(1.0);
  
  const handleVolumeChange = (newVolume: number) => {
    setVolume(newVolume);
    audioOutput.setVolume(newVolume);
  };
  
  const handleMute = () => {
    const newVolume = volume > 0 ? 0 : 1.0;
    handleVolumeChange(newVolume);
  };
  
  return (
    <div>
      <input
        type="range"
        min="0"
        max="1"
        step="0.1"
        value={volume}
        onChange={(e) => handleVolumeChange(parseFloat(e.target.value))}
      />
      <button onClick={handleMute}>
        {volume > 0 ? 'Mute' : 'Unmute'}
      </button>
    </div>
  );
}
```

### Status Monitoring

```typescript
import { AudioOutputService } from '@agentc/realtime-core';

class AudioMonitor {
  private audioOutput = AudioOutputService.getInstance();
  private statusInterval?: number;
  
  startMonitoring() {
    // Subscribe to status changes
    const unsubscribe = this.audioOutput.onStatusChange((status) => {
      this.updateUI(status);
    });
    
    // Also poll for statistics
    this.statusInterval = setInterval(() => {
      const status = this.audioOutput.getStatus();
      console.log('Audio stats:', {
        received: status.chunksReceived,
        played: status.chunksPlayed,
        skipped: status.chunksSkipped,
        queue: status.queueLength
      });
    }, 1000);
    
    return () => {
      unsubscribe();
      clearInterval(this.statusInterval);
    };
  }
  
  private updateUI(status: AudioOutputStatus) {
    // Update UI indicators
    document.getElementById('playing')?.classList.toggle('active', status.isPlaying);
    document.getElementById('queue')?.textContent = status.queueLength.toString();
  }
}
```

### Turn Management Integration

```typescript
import { AudioOutputService, TurnManager } from '@agentc/realtime-core';

class TurnAwareAudio {
  private audioOutput = AudioOutputService.getInstance();
  private turnManager: TurnManager;
  
  constructor(turnManager: TurnManager) {
    this.turnManager = turnManager;
    this.setupTurnHandling();
  }
  
  private setupTurnHandling() {
    // Stop audio when user starts speaking
    this.turnManager.on('turnStart', (turn) => {
      if (turn === 'user') {
        this.audioOutput.stopPlayback();
      }
    });
    
    // Clear buffers on turn end
    this.turnManager.on('turnEnd', () => {
      this.audioOutput.clearBuffers();
    });
  }
}
```

## Integration Patterns

### With RealtimeClient

```typescript
import { RealtimeClient, AudioOutputService } from '@agentc/realtime-core';

class AudioEnabledClient {
  private client: RealtimeClient;
  private audioOutput = AudioOutputService.getInstance();
  
  constructor() {
    this.client = new RealtimeClient();
    this.setupAudioHandling();
  }
  
  private setupAudioHandling() {
    // Handle binary audio frames
    this.client.on('audio:output', async (data) => {
      await this.audioOutput.playAudioChunk(data);
    });
    
    // Handle voice model changes
    this.client.on('voice:changed', (voice) => {
      this.audioOutput.setVoiceModel(voice);
    });
    
    // Stop audio on disconnect
    this.client.on('disconnected', () => {
      this.audioOutput.stopPlayback();
    });
  }
}
```

### With React Hooks

```typescript
import { useEffect, useState } from 'react';
import { AudioOutputService, AudioOutputStatus } from '@agentc/realtime-core';

export function useAudioOutput() {
  const [status, setStatus] = useState<AudioOutputStatus>();
  const audioOutput = AudioOutputService.getInstance();
  
  useEffect(() => {
    // Subscribe to status updates
    const unsubscribe = audioOutput.onStatusChange(setStatus);
    
    // Get initial status
    setStatus(audioOutput.getStatus());
    
    return unsubscribe;
  }, []);
  
  return {
    status,
    setVolume: (v: number) => audioOutput.setVolume(v),
    stop: () => audioOutput.stopPlayback(),
    clear: () => audioOutput.clearBuffers()
  };
}
```

## Best Practices

### 1. Handle Initialization Errors

```typescript
async function initAudio() {
  try {
    const audioOutput = AudioOutputService.getInstance();
    // First playback will trigger initialization
    await audioOutput.playAudioChunk(firstChunk);
  } catch (error) {
    // Handle browser compatibility issues
    console.error('Audio initialization failed:', error);
    // Show user message about audio requirements
  }
}
```

### 2. Respect Voice Models

```typescript
// Always set voice model to control behavior
audioOutput.setVoiceModel(currentVoice);

// Don't force playback when in special modes
if (voice.voice_id !== 'avatar' && voice.voice_id !== 'none') {
  await audioOutput.playAudioChunk(data);
}
```

### 3. Clean Up Resources

```typescript
// Clean up on component unmount or app close
window.addEventListener('beforeunload', async () => {
  await audioOutput.cleanup();
});
```

### 4. Monitor Queue Length

```typescript
// Prevent excessive buffering
audioOutput.onStatusChange((status) => {
  if (status.queueLength > 10) {
    console.warn('Audio queue growing large');
    // Consider clearing old chunks
    audioOutput.clearBuffers();
  }
});
```

## Performance Considerations

- Service uses Web Audio API for efficient playback
- Chunks are queued to prevent blocking
- Small overlap (10ms) prevents audible gaps
- Voice model checks prevent unnecessary processing
- Statistics are lightweight counters

## Browser Compatibility

Requires browser support for:
- Web Audio API
- AudioContext
- AudioWorkletNode (for input processing)
- PCM audio format handling

## Error Handling

```typescript
try {
  await audioOutput.playAudioChunk(audioData);
} catch (error) {
  if (error.message.includes('AudioContext not supported')) {
    // Show browser compatibility message
  } else if (error.message.includes('not initialized')) {
    // Retry initialization
  } else {
    // Log unexpected error
    Logger.error('[AudioOutput] Playback error:', error);
  }
}
```

## Related Documentation

- [AudioProcessor](./AudioProcessor.md) - Audio input processing
- [VoiceManager](./VoiceManager.md) - Voice model management  
- [TurnManager](./TurnManager.md) - Turn-based audio control
- [RealtimeClient](./RealtimeClient.md) - Client integration