# useVoiceModel Hook

## Purpose and Overview

The `useVoiceModel` hook provides an interface for managing voice model selection in the Agent C Realtime SDK. It handles voice switching, availability checking, and special modes like text-only and avatar modes. The hook maintains synchronization between the UI state and the underlying VoiceManager in the SDK.

Key capabilities:
- Voice model selection and switching
- Available voice discovery
- Special mode detection (text-only, avatar)
- Voice availability checking
- Error handling for voice changes
- Bi-directional sync with server voice changes

## Import Statement

```typescript
import { useVoiceModel } from '@agentc/realtime-react';
```

## TypeScript Types

```typescript
interface UseVoiceModelReturn {
  /** Currently selected voice */
  currentVoice: Voice | null;
  
  /** List of available voices */
  availableVoices: Voice[];
  
  /** Whether avatar mode is active */
  isAvatarMode: boolean;
  
  /** Whether text-only mode is active (voice id = 'none') */
  isTextOnly: boolean;
  
  /** Set the active voice */
  setVoice: (voiceId: string) => Promise<void>;
  
  /** Check if a voice is available */
  isVoiceAvailable: (voiceId: string) => boolean;
  
  /** Get voice by ID */
  getVoiceById: (voiceId: string) => Voice | undefined;
  
  /** Whether voice selection is loading */
  isLoading: boolean;
  
  /** Error message if voice selection failed */
  error: string | null;
}

interface Voice {
  voice_id: string;
  name: string;
  preview_url?: string;
  description?: string;
  metadata?: Record<string, any>;
}
```

## Return Value Descriptions

### Properties

- **`currentVoice`**: The currently active `Voice` object, or `null` if no voice is selected
- **`availableVoices`**: Array of all available voice models that can be selected
- **`isAvatarMode`**: Boolean indicating if the special avatar voice mode is active (`voice_id === 'avatar'`)
- **`isTextOnly`**: Boolean indicating if text-only mode is active (`voice_id === 'none'`)
- **`isLoading`**: Boolean indicating if a voice change operation is in progress
- **`error`**: String error message if the last voice operation failed, `null` otherwise

### Methods

- **`setVoice(voiceId)`**: Asynchronously sets the active voice. Updates both local state and sends the change to the server
- **`isVoiceAvailable(voiceId)`**: Checks if a specific voice ID is in the available voices list
- **`getVoiceById(voiceId)`**: Returns the full `Voice` object for a given ID, or `undefined` if not found

## Usage Examples

### Basic Voice Selection

```tsx
function VoiceSelector() {
  const {
    currentVoice,
    availableVoices,
    setVoice,
    isLoading,
    error
  } = useVoiceModel();

  const handleVoiceChange = async (voiceId: string) => {
    try {
      await setVoice(voiceId);
      console.log(`Switched to voice: ${voiceId}`);
    } catch (err) {
      console.error('Failed to change voice:', err);
    }
  };

  return (
    <div>
      <select
        value={currentVoice?.voice_id || ''}
        onChange={(e) => handleVoiceChange(e.target.value)}
        disabled={isLoading}
      >
        <option value="">Select a voice</option>
        {availableVoices.map(voice => (
          <option key={voice.voice_id} value={voice.voice_id}>
            {voice.name}
          </option>
        ))}
      </select>
      {error && <div className="error">{error}</div>}
      {isLoading && <div>Changing voice...</div>}
    </div>
  );
}
```

### Mode-Based Voice UI

```tsx
function VoiceModeDisplay() {
  const {
    currentVoice,
    isTextOnly,
    isAvatarMode,
    setVoice
  } = useVoiceModel();

  const switchToTextMode = () => setVoice('none');
  const switchToAvatarMode = () => setVoice('avatar');
  const switchToVoiceMode = () => setVoice('alloy');

  return (
    <div>
      <div className="mode-indicators">
        {isTextOnly && <span>📝 Text Only Mode</span>}
        {isAvatarMode && <span>👤 Avatar Mode</span>}
        {!isTextOnly && !isAvatarMode && (
          <span>🔊 Voice Mode: {currentVoice?.name}</span>
        )}
      </div>
      
      <div className="mode-buttons">
        <button onClick={switchToTextMode}>Text Only</button>
        <button onClick={switchToVoiceMode}>Voice</button>
        <button onClick={switchToAvatarMode}>Avatar</button>
      </div>
    </div>
  );
}
```

### Voice Preview Player

```tsx
function VoicePreview() {
  const {
    availableVoices,
    currentVoice,
    setVoice,
    getVoiceById
  } = useVoiceModel();
  
  const [previewVoiceId, setPreviewVoiceId] = useState<string | null>(null);
  
  const handlePreview = (voiceId: string) => {
    const voice = getVoiceById(voiceId);
    if (voice?.preview_url) {
      const audio = new Audio(voice.preview_url);
      audio.play();
      setPreviewVoiceId(voiceId);
      audio.onended = () => setPreviewVoiceId(null);
    }
  };

  return (
    <div className="voice-gallery">
      {availableVoices.map(voice => (
        <div key={voice.voice_id} className="voice-card">
          <h3>{voice.name}</h3>
          <p>{voice.description}</p>
          <div className="voice-actions">
            <button 
              onClick={() => handlePreview(voice.voice_id)}
              disabled={!voice.preview_url || previewVoiceId === voice.voice_id}
            >
              {previewVoiceId === voice.voice_id ? 'Playing...' : 'Preview'}
            </button>
            <button
              onClick={() => setVoice(voice.voice_id)}
              disabled={currentVoice?.voice_id === voice.voice_id}
            >
              {currentVoice?.voice_id === voice.voice_id ? 'Selected' : 'Select'}
            </button>
          </div>
        </div>
      ))}
    </div>
  );
}
```

### Availability Checking

```tsx
function SmartVoiceSelector() {
  const {
    setVoice,
    isVoiceAvailable,
    currentVoice
  } = useVoiceModel();
  
  const preferredVoices = ['alloy', 'echo', 'fable', 'onyx'];
  
  const selectBestAvailableVoice = async () => {
    for (const voiceId of preferredVoices) {
      if (isVoiceAvailable(voiceId)) {
        await setVoice(voiceId);
        console.log(`Selected available voice: ${voiceId}`);
        return;
      }
    }
    console.warn('No preferred voices available');
  };
  
  useEffect(() => {
    // Auto-select if no voice is set
    if (!currentVoice) {
      selectBestAvailableVoice();
    }
  }, []);

  return (
    <div>
      Current Voice: {currentVoice?.name || 'None'}
      <button onClick={selectBestAvailableVoice}>
        Auto-Select Best Voice
      </button>
    </div>
  );
}
```

### Voice Settings Panel

```tsx
function VoiceSettings() {
  const {
    currentVoice,
    availableVoices,
    setVoice,
    isTextOnly,
    isAvatarMode,
    isLoading,
    error
  } = useVoiceModel();
  
  const regularVoices = availableVoices.filter(
    v => v.voice_id !== 'none' && v.voice_id !== 'avatar'
  );

  return (
    <div className="voice-settings">
      <h2>Voice Settings</h2>
      
      <div className="output-mode">
        <label>Output Mode:</label>
        <div className="mode-toggle">
          <button
            className={isTextOnly ? 'active' : ''}
            onClick={() => setVoice('none')}
          >
            Text Only
          </button>
          <button
            className={!isTextOnly && !isAvatarMode ? 'active' : ''}
            onClick={() => setVoice(regularVoices[0]?.voice_id || 'alloy')}
          >
            Voice
          </button>
          <button
            className={isAvatarMode ? 'active' : ''}
            onClick={() => setVoice('avatar')}
          >
            Avatar
          </button>
        </div>
      </div>

      {!isTextOnly && !isAvatarMode && (
        <div className="voice-selection">
          <label>Select Voice:</label>
          <select
            value={currentVoice?.voice_id || ''}
            onChange={(e) => setVoice(e.target.value)}
            disabled={isLoading}
          >
            {regularVoices.map(voice => (
              <option key={voice.voice_id} value={voice.voice_id}>
                {voice.name}
              </option>
            ))}
          </select>
        </div>
      )}

      {currentVoice && (
        <div className="current-voice-info">
          <h3>Active Voice</h3>
          <dl>
            <dt>Name:</dt>
            <dd>{currentVoice.name}</dd>
            <dt>ID:</dt>
            <dd>{currentVoice.voice_id}</dd>
            {currentVoice.description && (
              <>
                <dt>Description:</dt>
                <dd>{currentVoice.description}</dd>
              </>
            )}
          </dl>
        </div>
      )}

      {error && (
        <div className="error-message">
          Failed to change voice: {error}
        </div>
      )}
    </div>
  );
}
```

### Server Sync Example

```tsx
function ServerSyncedVoice() {
  const {
    currentVoice,
    availableVoices,
    setVoice
  } = useVoiceModel();
  
  const [localSelection, setLocalSelection] = useState<string>('');
  
  // Sync local selection when server voice changes
  useEffect(() => {
    if (currentVoice && currentVoice.voice_id !== localSelection) {
      setLocalSelection(currentVoice.voice_id);
      console.log('Voice changed by server to:', currentVoice.name);
    }
  }, [currentVoice]);
  
  const handleLocalChange = async (voiceId: string) => {
    setLocalSelection(voiceId);
    try {
      await setVoice(voiceId);
      console.log('Local voice change successful');
    } catch (err) {
      console.error('Failed to update server:', err);
      // Revert local selection
      setLocalSelection(currentVoice?.voice_id || '');
    }
  };

  return (
    <div>
      <h3>Voice (Synced with Server)</h3>
      <select
        value={localSelection}
        onChange={(e) => handleLocalChange(e.target.value)}
      >
        {availableVoices.map(voice => (
          <option key={voice.voice_id} value={voice.voice_id}>
            {voice.name} {voice.voice_id === currentVoice?.voice_id && '(Active)'}
          </option>
        ))}
      </select>
    </div>
  );
}
```

## Turn Management Integration Notes

While the `useVoiceModel` hook doesn't directly manage turn state, voice changes can affect the conversation flow:

- **Voice changes are always allowed**: Unlike audio streaming, voice changes can occur at any time
- **Server synchronization**: Voice changes are immediately sent to the server via `client.setAgentVoice()`
- **No turn interruption**: Changing voices doesn't interrupt the current turn or conversation flow
- **Special modes**: Switching to text-only mode (`voice_id: 'none'`) disables audio output but doesn't affect turn management

## StrictMode Compatibility

The hook is fully compatible with React StrictMode:

- Event listeners are properly cleaned up on unmount
- No duplicate subscriptions during double-mounting
- State updates are properly batched
- No memory leaks from event handlers

## Best Practices for Audio/Voice Features

### Voice Selection Strategy
```tsx
// Provide fallback voices for better UX
const PRIMARY_VOICE = 'alloy';
const FALLBACK_VOICES = ['echo', 'fable'];

const selectVoiceWithFallback = async () => {
  if (isVoiceAvailable(PRIMARY_VOICE)) {
    await setVoice(PRIMARY_VOICE);
  } else {
    const fallback = FALLBACK_VOICES.find(isVoiceAvailable);
    if (fallback) await setVoice(fallback);
  }
};
```

### Mode Switching
```tsx
// Clear indication of mode switches
const switchMode = async (mode: 'text' | 'voice' | 'avatar') => {
  setIsTransitioning(true);
  try {
    const voiceId = mode === 'text' ? 'none' : 
                    mode === 'avatar' ? 'avatar' : 
                    'alloy';
    await setVoice(voiceId);
    showNotification(`Switched to ${mode} mode`);
  } finally {
    setIsTransitioning(false);
  }
};
```

### Error Recovery
```tsx
// Graceful error handling with recovery
const changeVoiceWithRecovery = async (voiceId: string) => {
  const previousVoice = currentVoice?.voice_id;
  try {
    await setVoice(voiceId);
  } catch (err) {
    console.error('Voice change failed:', err);
    if (previousVoice) {
      // Try to restore previous voice
      await setVoice(previousVoice);
    }
  }
};
```

### Voice Preloading
```tsx
// Preload voice data for better performance
useEffect(() => {
  availableVoices.forEach(voice => {
    if (voice.preview_url) {
      const audio = new Audio(voice.preview_url);
      audio.load(); // Preload audio
    }
  });
}, [availableVoices]);
```

## Performance Considerations

### Event Subscription Management
- The hook subscribes to both VoiceManager and client events
- Event handlers are cleaned up properly to prevent memory leaks
- Only subscribes when client is available

### State Update Optimization
- Voice information is only updated when changes occur
- Error states are cleared on successful operations
- Loading states are managed efficiently

### Server Communication
- Voice changes are sent to server asynchronously
- Local state updates immediately for responsive UI
- Server confirmations update state via event handlers

### Memory Management
- No polling required - uses event-driven updates
- Event listeners are removed on unmount
- No retained references to stale closures

### Best Practices for Performance
1. Cache voice selections in localStorage for persistence
2. Implement debouncing if allowing rapid voice switching
3. Preload voice preview audio for instant playback
4. Use `isVoiceAvailable()` before attempting voice changes
5. Handle errors gracefully without repeated retry attempts