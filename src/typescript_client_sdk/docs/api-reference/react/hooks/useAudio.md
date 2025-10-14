# useAudio Hook

## Purpose and Overview

The `useAudio` hook provides a comprehensive interface for managing audio recording, streaming, and playback in the Agent C Realtime SDK. It handles microphone permissions, device selection, volume control, and integrates seamlessly with turn management to ensure proper conversation flow.

Key capabilities:
- Microphone recording and streaming control
- Audio device management and selection  
- Volume and mute controls
- Turn state awareness for proper conversation management
- Real-time audio level monitoring
- Automatic permission handling

## Import Statement

```typescript
import { useAudio } from '@agentc/realtime-react';
```

## TypeScript Types

```typescript
interface UseAudioOptions {
  /** Automatically start recording when component mounts */
  autoStart?: boolean;
  
  /** Whether to respect turn state when streaming audio */
  respectTurnState?: boolean;
}

interface UseAudioReturn {
  // Status Properties
  status: AudioStatus;                    // Full audio system status
  isRecording: boolean;                   // Whether microphone is recording
  isStreaming: boolean;                   // Whether audio is streaming to server
  canSendInput: boolean;                  // Whether user can send audio (based on turn)
  audioLevel: number;                     // Current input level (0.0 to 1.0)
  volume: number;                         // Output volume (0-100)
  isMuted: boolean;                      // Whether output is muted
  inputDevice: string;                    // Current input device ID
  availableDevices: MediaDeviceInfo[];   // Available input devices
  
  // Control Methods
  startRecording: () => Promise<void>;    // Start microphone recording
  stopRecording: () => void;              // Stop microphone recording
  startStreaming: () => Promise<void>;    // Start streaming to server
  stopStreaming: () => void;              // Stop streaming to server
  requestPermission: () => Promise<boolean>; // Request mic permission
  setVolume: (volume: number) => void;    // Set output volume (0-100)
  setMuted: (muted: boolean) => void;     // Set mute state
  toggleMute: () => void;                 // Toggle mute on/off
  setInputDevice: (deviceId: string) => Promise<void>; // Change input device
  
  // Derived States
  canStartRecording: boolean;             // Whether recording can start
  needsPermission: boolean;               // Whether permission is needed
  hasError: boolean;                      // Whether there's an error
  errorMessage?: string;                  // Error details if any
}
```

## Return Value Descriptions

### Status Properties

- **`status`**: Complete `AudioStatus` object from the core SDK containing detailed audio system state
- **`isRecording`**: Boolean indicating if the microphone is actively recording audio
- **`isStreaming`**: Boolean indicating if recorded audio is being sent to the server
- **`canSendInput`**: Boolean based on turn management - true when it's the user's turn to speak
- **`audioLevel`**: Real-time audio input level from 0.0 (silence) to 1.0 (maximum)
- **`volume`**: Current output volume from 0 to 100 (percentage)
- **`isMuted`**: Whether audio output is muted (volume temporarily set to 0)
- **`inputDevice`**: Device ID of the currently selected microphone
- **`availableDevices`**: Array of available audio input devices detected by the browser

### Control Methods

- **`startRecording()`**: Begins capturing audio from the microphone. Returns a promise that resolves when recording starts successfully
- **`stopRecording()`**: Stops microphone recording immediately
- **`startStreaming()`**: Starts sending recorded audio to the server. Automatically starts recording if not already active
- **`stopStreaming()`**: Stops sending audio to the server while keeping recording active
- **`requestPermission()`**: Prompts user for microphone permission. Returns true if granted, false if denied
- **`setVolume(volume)`**: Sets output volume (0-100). Automatically unmutes if volume > 0
- **`setMuted(muted)`**: Mutes or unmutes output while preserving volume setting
- **`toggleMute()`**: Toggles between muted and unmuted states
- **`setInputDevice(deviceId)`**: Changes the active microphone. Maintains recording state during switch

### Derived States

- **`canStartRecording`**: True when client is connected and audio system is ready
- **`needsPermission`**: True when microphone permission hasn't been granted yet
- **`hasError`**: True when an audio error has occurred
- **`errorMessage`**: Description of the current error if one exists

## Usage Examples

### Basic Audio Recording

```tsx
function AudioRecorder() {
  const { 
    isRecording, 
    startRecording, 
    stopRecording,
    audioLevel 
  } = useAudio();

  return (
    <div>
      <button onClick={isRecording ? stopRecording : startRecording}>
        {isRecording ? 'Stop Recording' : 'Start Recording'}
      </button>
      <div>Audio Level: {(audioLevel * 100).toFixed(0)}%</div>
    </div>
  );
}
```

### Auto-Start with Permission Handling

```tsx
function AutoRecordingChat() {
  const { 
    isRecording,
    needsPermission,
    requestPermission,
    hasError,
    errorMessage
  } = useAudio({ autoStart: true });

  useEffect(() => {
    if (needsPermission) {
      requestPermission();
    }
  }, [needsPermission, requestPermission]);

  if (hasError) {
    return <div>Audio Error: {errorMessage}</div>;
  }

  return (
    <div>
      {isRecording ? '🎤 Recording...' : '🔇 Not Recording'}
    </div>
  );
}
```

### Turn-Aware Audio Streaming

```tsx
function TurnBasedAudioChat() {
  const { 
    isStreaming,
    canSendInput,
    startStreaming,
    stopStreaming,
    audioLevel
  } = useAudio({ respectTurnState: true });

  const handlePushToTalk = async () => {
    if (!canSendInput) {
      alert("Please wait for your turn to speak");
      return;
    }
    
    try {
      await startStreaming();
    } catch (error) {
      console.error('Failed to start streaming:', error);
    }
  };

  return (
    <div>
      <button 
        onMouseDown={handlePushToTalk}
        onMouseUp={stopStreaming}
        disabled={!canSendInput}
      >
        {canSendInput ? 'Push to Talk' : 'Wait for Turn'}
      </button>
      {isStreaming && (
        <div>Speaking... Level: {(audioLevel * 100).toFixed(0)}%</div>
      )}
    </div>
  );
}
```

### Volume and Mute Controls

```tsx
function AudioControls() {
  const { 
    volume,
    isMuted,
    setVolume,
    toggleMute
  } = useAudio();

  return (
    <div>
      <button onClick={toggleMute}>
        {isMuted ? '🔇 Unmute' : '🔊 Mute'}
      </button>
      <input
        type="range"
        min="0"
        max="100"
        value={isMuted ? 0 : volume}
        onChange={(e) => setVolume(Number(e.target.value))}
        disabled={isMuted}
      />
      <span>{isMuted ? 'Muted' : `${volume}%`}</span>
    </div>
  );
}
```

### Device Selection

```tsx
function MicrophoneSelector() {
  const { 
    inputDevice,
    availableDevices,
    setInputDevice,
    isRecording
  } = useAudio();

  const handleDeviceChange = async (deviceId: string) => {
    try {
      await setInputDevice(deviceId);
    } catch (error) {
      console.error('Failed to change device:', error);
    }
  };

  return (
    <select 
      value={inputDevice} 
      onChange={(e) => handleDeviceChange(e.target.value)}
    >
      {availableDevices.map(device => (
        <option key={device.deviceId} value={device.deviceId}>
          {device.label || `Microphone ${device.deviceId.slice(0, 8)}`}
        </option>
      ))}
    </select>
  );
}
```

### Complete Audio Interface

```tsx
function FullAudioInterface() {
  const {
    isRecording,
    isStreaming,
    canSendInput,
    audioLevel,
    volume,
    isMuted,
    availableDevices,
    inputDevice,
    startRecording,
    stopRecording,
    startStreaming,
    stopStreaming,
    setVolume,
    toggleMute,
    setInputDevice,
    needsPermission,
    requestPermission,
    hasError,
    errorMessage
  } = useAudio({ respectTurnState: true });

  // Handle permission on mount
  useEffect(() => {
    if (needsPermission) {
      requestPermission();
    }
  }, [needsPermission, requestPermission]);

  return (
    <div className="audio-interface">
      {hasError && (
        <div className="error">Error: {errorMessage}</div>
      )}
      
      <div className="recording-controls">
        <button onClick={isRecording ? stopRecording : startRecording}>
          {isRecording ? '⏹️ Stop' : '⏺️ Record'}
        </button>
        
        <button 
          onClick={isStreaming ? stopStreaming : startStreaming}
          disabled={!canSendInput || !isRecording}
        >
          {isStreaming ? '⏸️ Pause Stream' : '📡 Stream'}
        </button>
        
        {!canSendInput && (
          <span>Waiting for turn...</span>
        )}
      </div>

      <div className="audio-levels">
        <label>Input Level:</label>
        <progress value={audioLevel} max="1" />
        <span>{(audioLevel * 100).toFixed(0)}%</span>
      </div>

      <div className="volume-controls">
        <button onClick={toggleMute}>
          {isMuted ? '🔇' : '🔊'}
        </button>
        <input
          type="range"
          min="0"
          max="100"
          value={volume}
          onChange={(e) => setVolume(Number(e.target.value))}
        />
        <span>{volume}%</span>
      </div>

      <div className="device-selection">
        <label>Microphone:</label>
        <select 
          value={inputDevice}
          onChange={(e) => setInputDevice(e.target.value)}
        >
          {availableDevices.map(device => (
            <option key={device.deviceId} value={device.deviceId}>
              {device.label || 'Unnamed Device'}
            </option>
          ))}
        </select>
      </div>
    </div>
  );
}
```

## Turn Management Integration

The `useAudio` hook deeply integrates with the SDK's turn management system:

- **`respectTurnState` option**: When enabled (default: true), the hook prevents audio streaming when it's not the user's turn
- **Automatic stream stopping**: If the user loses their turn while streaming, the stream automatically stops
- **`canSendInput` property**: Always reflects the current turn state, allowing UI to show appropriate feedback
- **Error handling**: Attempting to stream when not allowed throws a descriptive error

Example with turn awareness:
```tsx
const { canSendInput, startStreaming } = useAudio({ respectTurnState: true });

// startStreaming will fail if !canSendInput
// The hook automatically stops streaming if turn is lost
```

## StrictMode Compatibility

The hook is fully compatible with React StrictMode:

- Proper cleanup of intervals and event listeners
- No memory leaks from multiple mount/unmount cycles
- Maintains single audio instance through provider pattern
- Handles concurrent mode gracefully

## Best Practices

### Permission Management
Always check and request permissions before attempting to record:
```tsx
if (needsPermission) {
  await requestPermission();
}
```

### Error Handling
Always wrap async operations in try-catch blocks:
```tsx
try {
  await startRecording();
} catch (error) {
  // Handle error appropriately
}
```

### Turn State Respect
Use `respectTurnState: true` for conversational interfaces:
```tsx
const audio = useAudio({ respectTurnState: true });
```

### Volume Persistence
The hook maintains previous volume when muting/unmuting:
```tsx
// Volume is preserved
toggleMute(); // Mutes (volume becomes 0)
toggleMute(); // Unmutes (volume restored)
```

### Device Switching
The hook maintains recording state during device switches:
```tsx
// Recording continues with new device
await setInputDevice(newDeviceId);
```

## Performance Considerations

### Status Polling
The hook polls audio status every 100ms to provide real-time updates. This is optimized to:
- Only run when client is connected
- Clean up properly on unmount
- Use refs to prevent stale closures
- Batch state updates efficiently

### Memory Management
- Event listeners are properly cleaned up
- Intervals are cleared on unmount
- No memory leaks in StrictMode
- Efficient state updates using functional setState

### Browser Compatibility
- Requests permissions using standard MediaDevices API
- Handles missing permissions gracefully
- Device enumeration works across all modern browsers
- Automatic cleanup of media streams

### Optimization Tips
1. Use `autoStart: false` if recording isn't immediately needed
2. Respect turn state to prevent unnecessary streaming
3. Implement debouncing for volume changes if using sliders
4. Cache device lists if not expecting changes
5. Use error boundaries for production error handling