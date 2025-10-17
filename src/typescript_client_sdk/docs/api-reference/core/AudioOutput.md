# AudioOutput API Reference

The `AudioOutputService` manages text-to-speech audio playback, handling binary audio streaming from the Agent C server.

## Import

```typescript
import { AudioOutputService } from '@agentc/realtime-core';
```

## Overview

The AudioOutputService receives binary PCM16 audio data from the server, queues it for smooth playback, and manages the Web Audio API for output.

## Singleton Access

```typescript
static getInstance(): AudioOutputService
```

Gets the singleton AudioOutputService instance.

**Example:**
```typescript
const audioOutput = AudioOutputService.getInstance();
```

## Methods

### playAudioChunk()

Plays a chunk of audio data.

```typescript
playAudioChunk(audioData: ArrayBuffer): void
```

**Parameters:**
- `audioData` (ArrayBuffer) - Binary audio data (PCM16 format)

**Example:**
```typescript
// Handle incoming audio from server
client.on('audio:output', (audioData: ArrayBuffer) => {
  audioOutput.playAudioChunk(audioData);
});
```

### setVolume()

Sets the playback volume.

```typescript
setVolume(volume: number): void
```

**Parameters:**
- `volume` (number) - Volume level between 0 and 1

**Throws:** Error if volume is out of range

**Example:**
```typescript
audioOutput.setVolume(0.8); // 80% volume

// Mute
audioOutput.setVolume(0);

// Full volume
audioOutput.setVolume(1);
```

### getVolume()

Gets the current volume level.

```typescript
getVolume(): number
```

**Returns:** Current volume (0-1)

**Example:**
```typescript
const volume = audioOutput.getVolume();
console.log(`Volume: ${volume * 100}%`);
```

### pause()

Pauses audio playback.

```typescript
pause(): void
```

**Example:**
```typescript
audioOutput.pause();
console.log('Audio paused');
```

### resume()

Resumes audio playback.

```typescript
resume(): void
```

**Example:**
```typescript
audioOutput.resume();
console.log('Audio resumed');
```

### stop()

Stops playback and clears the queue.

```typescript
stop(): void
```

**Example:**
```typescript
audioOutput.stop();
console.log('Audio stopped and queue cleared');
```

### clearBuffers()

Clears all queued audio without stopping playback.

```typescript
clearBuffers(): void
```

**Example:**
```typescript
// Clear pending audio
audioOutput.clearBuffers();
```

### setVoiceModel()

Sets the voice model for format detection.

```typescript
setVoiceModel(voiceModel: VoiceModel | null): void
```

**Parameters:**
- `voiceModel` - Voice model with format information

```typescript
interface VoiceModel {
  voice_id: string;
  format: string;        // e.g., 'pcm16'
  sampleRate: number;    // e.g., 16000
  vendor: string;
  description?: string;
}
```

**Example:**
```typescript
audioOutput.setVoiceModel({
  voice_id: 'nova',
  format: 'pcm16',
  sampleRate: 16000,
  vendor: 'openai'
});
```

### getStatus()

Gets the current service status.

```typescript
getStatus(): AudioOutputStatus
```

**Returns:** Status object

```typescript
interface AudioOutputStatus {
  isPlaying: boolean;
  isPaused: boolean;
  volume: number;
  queueLength: number;
  currentFormat: string;
  totalChunksPlayed: number;
  totalBytesPlayed: number;
}
```

**Example:**
```typescript
const status = audioOutput.getStatus();
console.log(`Playing: ${status.isPlaying}`);
console.log(`Queue: ${status.queueLength} chunks`);
console.log(`Format: ${status.currentFormat}`);
console.log(`Volume: ${status.volume * 100}%`);
```

### getQueueLength()

Gets the number of chunks in the playback queue.

```typescript
getQueueLength(): number
```

**Returns:** Number of queued chunks

**Example:**
```typescript
const queueSize = audioOutput.getQueueLength();
if (queueSize > 10) {
  console.log('Large audio buffer, possible latency');
}
```

### getPlaybackTime()

Gets the current playback time.

```typescript
getPlaybackTime(): number
```

**Returns:** Playback time in seconds

**Example:**
```typescript
const time = audioOutput.getPlaybackTime();
console.log(`Playback time: ${time.toFixed(2)}s`);
```

### destroy()

Destroys the service and releases resources.

```typescript
destroy(): void
```

**Example:**
```typescript
// Clean up when done
audioOutput.destroy();
```

## Event Handling

AudioOutputService extends EventEmitter:

### Events

- `audio:play-started` - Playback started
- `audio:play-stopped` - Playback stopped
- `audio:chunk-played` - Chunk finished playing
- `audio:queue-empty` - Queue emptied
- `audio:error` - Playback error

### Example Event Handling

```typescript
audioOutput.on('audio:play-started', () => {
  console.log('🔊 Audio playback started');
  showSpeakerIcon();
});

audioOutput.on('audio:play-stopped', () => {
  console.log('🔇 Audio playback stopped');
  hideSpeakerIcon();
});

audioOutput.on('audio:chunk-played', (chunkInfo: ChunkInfo) => {
  console.log(`Played chunk: ${chunkInfo.size} bytes, duration: ${chunkInfo.duration}s`);
});

audioOutput.on('audio:queue-empty', () => {
  console.log('Audio queue empty');
  // Could request more audio or show waiting state
});

audioOutput.on('audio:error', (error: Error) => {
  console.error('Audio playback error:', error);
});
```

### Event Types

```typescript
interface ChunkInfo {
  size: number;          // Chunk size in bytes
  duration: number;      // Duration in seconds
  timestamp: number;     // When played
}
```

## Audio Configuration

Configure audio output through RealtimeClient:

```typescript
const client = new RealtimeClient({
  enableAudio: true,
  audioConfig: {
    enableOutput: true,      // Enable audio output
    initialVolume: 0.8,      // Initial volume (0-1)
    outputBufferSize: 4096,  // Output buffer size
    outputLatency: 0.1       // Target latency in seconds
  }
});
```

## Audio Format Support

### PCM16 Format

The primary format for Agent C audio:

```typescript
// PCM16 characteristics
const pcm16Format = {
  format: 'pcm16',
  sampleRate: 16000,      // 16 kHz
  channels: 1,            // Mono
  bitDepth: 16,          // 16-bit samples
  signed: true,           // Signed integers
  littleEndian: true      // Little-endian byte order
};
```

### Converting PCM16 to Float32

The service automatically converts PCM16 to Float32 for Web Audio:

```typescript
function pcm16ToFloat32(pcm16Data: ArrayBuffer): Float32Array {
  const int16Array = new Int16Array(pcm16Data);
  const float32Array = new Float32Array(int16Array.length);
  
  for (let i = 0; i < int16Array.length; i++) {
    // Convert from 16-bit signed to float (-1 to 1)
    float32Array[i] = int16Array[i] / 32768;
  }
  
  return float32Array;
}
```

## Complete Example

```typescript
import { 
  RealtimeClient,
  AuthManager, 
  AudioOutputService 
} from '@agentc/realtime-core';

async function audioOutputExample() {
  // Setup authentication
  const authManager = new AuthManager({
    apiUrl: 'https://localhost:8000'
  });
  
  await authManager.login('username', 'password');
  
  // Create client with audio output enabled
  const client = new RealtimeClient({
    apiUrl: 'wss://localhost:8000/rt/ws',
    authManager,
    enableAudio: true,
    audioConfig: {
      enableOutput: true,
      initialVolume: 0.8
    }
  });
  
  // Get audio output service
  const audioOutput = AudioOutputService.getInstance();
  
  // Set initial volume
  audioOutput.setVolume(0.8);
  
  // Monitor playback events
  audioOutput.on('audio:play-started', () => {
    console.log('🔊 Started playing audio');
    document.getElementById('speaker-icon')!.classList.add('active');
  });
  
  audioOutput.on('audio:play-stopped', () => {
    console.log('🔇 Stopped playing audio');
    document.getElementById('speaker-icon')!.classList.remove('active');
  });
  
  audioOutput.on('audio:chunk-played', (info) => {
    console.log(`Played ${info.size} bytes (${info.duration.toFixed(2)}s)`);
  });
  
  audioOutput.on('audio:queue-empty', () => {
    console.log('📭 Audio queue empty');
  });
  
  audioOutput.on('audio:error', (error) => {
    console.error('❌ Audio error:', error);
  });
  
  // Connect to server
  await client.connect();
  
  // Handle incoming audio
  client.on('audio:output', (audioData: ArrayBuffer) => {
    console.log(`Received audio: ${audioData.byteLength} bytes`);
    
    // Play the audio chunk
    audioOutput.playAudioChunk(audioData);
    
    // Update UI with queue status
    const queueLength = audioOutput.getQueueLength();
    document.getElementById('queue-status')!.textContent = 
      `Queue: ${queueLength} chunks`;
  });
  
  // Handle voice changes
  client.on('agent_voice_changed', (event) => {
    const voiceManager = client.getVoiceManager();
    const voice = voiceManager?.getVoiceById(event.voice_id);
    
    if (voice) {
      // Update audio output for new voice format
      audioOutput.setVoiceModel({
        voice_id: voice.voice_id,
        format: voice.output_format || 'pcm16',
        sampleRate: 16000,
        vendor: voice.vendor,
        description: voice.description
      });
      
      console.log(`Voice changed to: ${voice.name}`);
      console.log(`Format: ${voice.output_format}`);
    }
  });
  
  // Volume control
  const volumeSlider = document.getElementById('volume-slider') as HTMLInputElement;
  volumeSlider?.addEventListener('input', (e) => {
    const volume = parseFloat((e.target as HTMLInputElement).value);
    audioOutput.setVolume(volume);
    console.log(`Volume: ${Math.round(volume * 100)}%`);
  });
  
  // Playback controls
  const pauseButton = document.getElementById('pause-button');
  pauseButton?.addEventListener('click', () => {
    const status = audioOutput.getStatus();
    
    if (status.isPlaying && !status.isPaused) {
      audioOutput.pause();
      pauseButton.textContent = 'Resume';
    } else if (status.isPaused) {
      audioOutput.resume();
      pauseButton.textContent = 'Pause';
    }
  });
  
  const stopButton = document.getElementById('stop-button');
  stopButton?.addEventListener('click', () => {
    audioOutput.stop();
    console.log('Playback stopped');
  });
  
  // Monitor audio statistics
  setInterval(() => {
    const status = audioOutput.getStatus();
    
    console.log('📊 Audio Output Stats:');
    console.log(`  Playing: ${status.isPlaying}`);
    console.log(`  Paused: ${status.isPaused}`);
    console.log(`  Queue: ${status.queueLength} chunks`);
    console.log(`  Volume: ${Math.round(status.volume * 100)}%`);
    console.log(`  Format: ${status.currentFormat}`);
    console.log(`  Total played: ${status.totalChunksPlayed} chunks`);
    console.log(`  Bytes played: ${status.totalBytesPlayed}`);
    console.log(`  Playback time: ${audioOutput.getPlaybackTime().toFixed(2)}s`);
  }, 5000);
  
  // Send a test message to generate audio
  client.sendText('Hello! Please tell me a short story.');
  
  // Advanced: Audio visualization
  let audioContext: AudioContext | null = null;
  let analyser: AnalyserNode | null = null;
  
  function setupAudioVisualization() {
    audioContext = new AudioContext();
    analyser = audioContext.createAnalyser();
    analyser.fftSize = 256;
    
    // Connect to audio output (would need access to internal audio node)
    // This is a simplified example
    
    const bufferLength = analyser.frequencyBinCount;
    const dataArray = new Uint8Array(bufferLength);
    
    function draw() {
      requestAnimationFrame(draw);
      
      if (!analyser) return;
      
      analyser.getByteFrequencyData(dataArray);
      
      // Draw frequency bars
      const canvas = document.getElementById('visualizer') as HTMLCanvasElement;
      const ctx = canvas.getContext('2d')!;
      
      ctx.fillStyle = 'rgb(0, 0, 0)';
      ctx.fillRect(0, 0, canvas.width, canvas.height);
      
      const barWidth = (canvas.width / bufferLength) * 2.5;
      let barHeight;
      let x = 0;
      
      for (let i = 0; i < bufferLength; i++) {
        barHeight = dataArray[i] / 2;
        
        ctx.fillStyle = `rgb(50, ${barHeight + 100}, 50)`;
        ctx.fillRect(x, canvas.height - barHeight, barWidth, barHeight);
        
        x += barWidth + 1;
      }
    }
    
    draw();
  }
  
  // Preload audio for lower latency
  audioOutput.on('audio:play-started', () => {
    // Audio context is automatically created
    // Could warm up audio system here if needed
  });
  
  // Handle interruptions
  client.on('user_turn_start', () => {
    // User is speaking, might want to lower volume or pause
    const currentVolume = audioOutput.getVolume();
    audioOutput.setVolume(currentVolume * 0.3); // Duck audio
  });
  
  client.on('user_turn_end', () => {
    // Restore volume
    audioOutput.setVolume(0.8);
  });
  
  // Clean up on exit
  window.addEventListener('beforeunload', () => {
    audioOutput.stop();
    audioOutput.destroy();
  });
  
  // Test different audio scenarios
  async function testAudioScenarios() {
    console.log('\n🧪 Testing Audio Scenarios:\n');
    
    // Test 1: Volume ramping
    console.log('Test 1: Volume ramping');
    for (let v = 0; v <= 1; v += 0.1) {
      audioOutput.setVolume(v);
      await new Promise(r => setTimeout(r, 200));
    }
    audioOutput.setVolume(0.8);
    
    // Test 2: Pause/Resume
    console.log('Test 2: Pause/Resume');
    setTimeout(() => {
      audioOutput.pause();
      console.log('Paused');
    }, 2000);
    
    setTimeout(() => {
      audioOutput.resume();
      console.log('Resumed');
    }, 4000);
    
    // Test 3: Clear buffers
    console.log('Test 3: Clear buffers');
    setTimeout(() => {
      const beforeClear = audioOutput.getQueueLength();
      audioOutput.clearBuffers();
      const afterClear = audioOutput.getQueueLength();
      console.log(`Cleared ${beforeClear - afterClear} chunks`);
    }, 6000);
  }
  
  // Run tests after connection
  setTimeout(testAudioScenarios, 5000);
}

audioOutputExample().catch(console.error);
```

## Best Practices

1. **Handle audio in the correct event:**
```typescript
// Good - use the dedicated audio event
client.on('audio:output', (audioData) => {
  audioOutput.playAudioChunk(audioData);
});

// Not recommended - don't process in binary_audio
client.on('binary_audio', (data) => {
  // This is legacy, use audio:output instead
});
```

2. **Monitor queue size for latency:**
```typescript
const queueLength = audioOutput.getQueueLength();
if (queueLength > 20) {
  console.warn('High audio latency detected');
  // Consider clearing old chunks
  audioOutput.clearBuffers();
}
```

3. **Set appropriate volume levels:**
```typescript
// Start with moderate volume
audioOutput.setVolume(0.7);

// Provide user control
function setUserVolume(value: number) {
  // Clamp to safe range
  const volume = Math.max(0, Math.min(1, value));
  audioOutput.setVolume(volume);
  // Save preference
  localStorage.setItem('audioVolume', volume.toString());
}
```

4. **Handle voice model changes:**
```typescript
client.on('agent_voice_changed', (event) => {
  // Update audio output for new voice
  const voice = getVoiceInfo(event.voice_id);
  if (voice) {
    audioOutput.setVoiceModel(voice);
  }
});
```

5. **Clean up properly:**
```typescript
// Always clean up on unmount/unload
function cleanup() {
  audioOutput.stop();
  audioOutput.clearBuffers();
  audioOutput.destroy();
}
```

## Performance Optimization

### Buffer Management

```typescript
// Monitor and manage buffer size
setInterval(() => {
  const queueLength = audioOutput.getQueueLength();
  
  if (queueLength > 30) {
    // Too much buffering, clear old chunks
    console.log('Reducing audio buffer');
    // Keep only recent chunks
    for (let i = 0; i < queueLength - 10; i++) {
      // Would need internal access to skip chunks
    }
  }
}, 1000);
```

### Latency Reduction

```typescript
// Configure for low latency
const audioConfig = {
  enableOutput: true,
  outputBufferSize: 2048,  // Smaller buffer
  outputLatency: 0.05      // 50ms target latency
};
```

## Troubleshooting

### Common Issues

**No audio output:**
- Check volume is not 0
- Verify `enableOutput: true` in config
- Check browser audio permissions
- Ensure audio data is being received

**Choppy playback:**
- Check network stability
- Monitor queue length for underruns
- Increase buffer size
- Check CPU usage

**Audio delay/latency:**
- Reduce buffer size
- Clear old chunks from queue
- Check network latency
- Use wired connection

**Volume issues:**
- Verify volume is between 0 and 1
- Check system volume
- Test with different voices

## Browser Compatibility

- **Chrome**: 66+ (Full support)
- **Firefox**: 76+ (Full support)
- **Safari**: 14.1+ (Full support)
- **Edge**: 79+ (Full support)

**Required APIs:**
- Web Audio API
- AudioContext
- AudioBuffer
- GainNode

## TypeScript Types

```typescript
import {
  AudioOutputService,
  AudioOutputStatus,
  VoiceModel,
  ChunkInfo
} from '@agentc/realtime-core';
```

All methods and properties are fully typed for TypeScript applications.