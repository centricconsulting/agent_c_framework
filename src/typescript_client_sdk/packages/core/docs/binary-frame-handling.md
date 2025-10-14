# Binary Frame Handling in Agent C Realtime SDK

## Overview

The Agent C Realtime SDK supports mixed message types over WebSocket:
- **JSON messages** for control events (text messages, configuration, etc.)
- **Binary frames** for audio data transmission (no base64 encoding)

This dual approach provides optimal performance for real-time audio streaming while maintaining clean APIs for control messages.

## Architecture

### WebSocket Configuration

The WebSocket is configured with `binaryType: 'arraybuffer'` to properly handle binary data:

```typescript
const wsManager = new WebSocketManager({
  url: wsUrl,
  binaryType: 'arraybuffer',  // Critical for binary audio
  // ... other options
});
```

### Audio Format Specifications

- **Sample Rate**: 16kHz (16000Hz) - Server expectation
- **Format**: PCM16 (16-bit signed integers, little-endian)
- **Channels**: Mono (1 channel)
- **Processing**: AudioWorklet automatically resamples from browser native rate (typically 44.1/48kHz) to 16kHz

### Sending Data

#### Binary Audio (Raw PCM16)
Audio is sent directly as binary frames at 16kHz sample rate, NOT wrapped in JSON events:

```typescript
// CORRECT: Send raw binary audio (16kHz PCM16)
const audioBuffer: ArrayBuffer = ... // PCM16 audio data at 16kHz
client.sendBinaryFrame(audioBuffer);

// The AudioWorklet handles resampling:
// Browser capture (48kHz) → Resample → 16kHz PCM16 → sendBinaryFrame()

// INCORRECT: Don't wrap audio in JSON events
// client.send({ type: 'audio_input_delta', audio: base64Audio }); // ❌ Wrong
```

#### Control Messages (JSON)
Control messages are sent as JSON-encoded events:

```typescript
// Send text input
client.sendText("Hello, agent!");

// Change voice
client.setAgentVoice("voice_id");
```

### Receiving Data

The client automatically distinguishes between message types:

```typescript
// Binary audio from server (TTS output)
client.on('audio:output', (audioData: ArrayBuffer) => {
  // audioData is raw binary audio (format depends on voice model)
  audioPlayer.playAudio(audioData);
});

// JSON events from server
client.on('text_delta', (event) => {
  console.log('Agent said:', event.content);
});
```

## Implementation Details

### WebSocketManager

The `WebSocketManager` class handles low-level WebSocket operations:

- **supportsBinary()**: Verifies the connection supports binary data
- **sendBinary()**: Dedicated method for sending binary frames
- **Message handling**: Automatically detects and routes JSON vs binary messages

### RealtimeClient

The `RealtimeClient` provides high-level APIs:

- **sendBinaryFrame(data: ArrayBuffer)**: Send raw audio to server
- **'audio:output' event**: Emitted when binary audio is received
- **Mixed message handling**: Seamlessly handles both JSON and binary

## Key Differences from JSON-wrapped Audio

### Before (Wrong Approach)
```typescript
// Audio wrapped in JSON - 33% overhead from base64
const event = {
  type: 'audio_input_delta',
  audio: btoa(audioData)  // Base64 encoding adds overhead
};
client.send(JSON.stringify(event));
```

### After (Correct Approach)
```typescript
// Direct binary transmission - no encoding overhead
client.sendBinaryFrame(audioBuffer);  // Raw PCM16 bytes
```

## Benefits

1. **Performance**: 33% bandwidth reduction by eliminating base64 encoding
2. **Simplicity**: Audio is just binary data, not wrapped in events
3. **Separation**: Clear distinction between control (JSON) and media (binary)
4. **Compatibility**: Works with standard WebSocket binary frame handling
5. **Automatic Resampling**: AudioWorklet handles sample rate conversion transparently
6. **Off-Thread Processing**: Audio processing doesn't block the main thread

## Testing

To verify binary frame handling:

```typescript
// 1. Check WebSocket configuration
const client = new RealtimeClient({
  audioConfig: {
    sampleRate: 16000  // Server expects 16kHz
  }
});
await client.connect();
// WebSocket should have binaryType = 'arraybuffer'

// 2. Verify AudioWorklet is loaded
// Check browser console for worklet loading
// Should load from /worklets/audio-processor.worklet.js

// 3. Send binary audio (automatically resampled to 16kHz)
const testAudio = new ArrayBuffer(3200);  // 100ms at 16kHz PCM16
client.sendBinaryFrame(testAudio);  // Should send raw binary

// 4. Monitor sample rates
client.on('audio:config', (config) => {
  console.log('Browser native rate:', config.nativeSampleRate);  // e.g., 48000
  console.log('Target rate:', config.targetSampleRate);  // 16000
  console.log('Resampling active:', config.isResampling);  // true if rates differ
});

// 5. Receive binary audio (16kHz PCM16 from server)
client.on('audio:output', (data) => {
  console.log('Received binary audio:', data.byteLength, 'bytes');
  // At 16kHz PCM16: 32,000 bytes/second (16000 samples × 2 bytes)
});
```

## Migration Guide

If migrating from JSON-wrapped audio:

1. Replace `AudioInputDelta` events with `sendBinaryFrame()`
2. Listen for `'audio:output'` instead of parsing JSON audio events
3. Remove any base64 encoding/decoding of audio data
4. Ensure WebSocket binaryType is set to 'arraybuffer'