# useOutputMode Hook

## Purpose and Overview

The `useOutputMode` hook provides high-level control over the agent's output mode, coordinating between UI state (text/voice/avatar) and the underlying SDK voice model selection. It abstracts the complexity of voice management into simple mode switching, automatically handling the appropriate voice configurations for each mode.

Key capabilities:
- Simple mode switching between text, voice, and avatar outputs
- Automatic voice configuration for each mode
- Bi-directional sync with SDK voice changes
- Loading and error state management
- Convenience methods for mode switching
- Smart fallback handling for unavailable voices

## Import Statement

```typescript
import { useOutputMode } from '@agentc/realtime-react';
```

## TypeScript Types

```typescript
type OutputMode = 'text' | 'voice' | 'avatar';

interface UseOutputModeOptions {
  /** Initial output mode */
  initialMode?: OutputMode;
  
  /** Default voice ID for voice mode */
  defaultVoiceId?: string;
  
  /** Callback when mode changes */
  onModeChange?: (mode: OutputMode) => void;
}

interface UseOutputModeReturn {
  /** Current output mode */
  outputMode: OutputMode;
  
  /** Set the output mode */
  setOutputMode: (mode: OutputMode) => void;
  
  /** Whether currently in text-only mode */
  isTextMode: boolean;
  
  /** Whether currently in voice mode */
  isVoiceMode: boolean;
  
  /** Whether currently in avatar mode */
  isAvatarMode: boolean;
  
  /** Switch to text mode */
  switchToText: () => void;
  
  /** Switch to voice mode */
  switchToVoice: () => void;
  
  /** Switch to avatar mode */
  switchToAvatar: () => void;
  
  /** Whether the mode is currently switching */
  isSwitching: boolean;
  
  /** Error message if mode switch failed */
  error: string | null;
}
```

## Voice Mode Configurations

The hook uses these default voice configurations:

| Mode | Voice ID | Description |
|------|----------|-------------|
| `text` | `'none'` | Text-only output, no audio |
| `voice` | `'alloy'` (default) | Standard voice output |
| `avatar` | `'avatar'` | Special avatar mode with visual representation |

## Return Value Descriptions

### State Properties

- **`outputMode`**: Current mode as `'text'`, `'voice'`, or `'avatar'`
- **`isTextMode`**: Boolean indicating if text-only mode is active
- **`isVoiceMode`**: Boolean indicating if voice mode is active
- **`isAvatarMode`**: Boolean indicating if avatar mode is active
- **`isSwitching`**: Boolean indicating if a mode change is in progress
- **`error`**: Error message string if the last mode switch failed, `null` otherwise

### Control Methods

- **`setOutputMode(mode)`**: Asynchronously switches to the specified mode, updating both UI state and SDK voice
- **`switchToText()`**: Convenience method to switch to text-only mode
- **`switchToVoice()`**: Convenience method to switch to voice mode
- **`switchToAvatar()`**: Convenience method to switch to avatar mode

## Usage Examples

### Basic Mode Switcher

```tsx
function OutputModeSwitcher() {
  const {
    outputMode,
    setOutputMode,
    isSwitching,
    error
  } = useOutputMode({ initialMode: 'voice' });

  return (
    <div className="mode-switcher">
      <div className="mode-buttons">
        <button
          onClick={() => setOutputMode('text')}
          disabled={isSwitching}
          className={outputMode === 'text' ? 'active' : ''}
        >
          📝 Text Only
        </button>
        <button
          onClick={() => setOutputMode('voice')}
          disabled={isSwitching}
          className={outputMode === 'voice' ? 'active' : ''}
        >
          🔊 Voice
        </button>
        <button
          onClick={() => setOutputMode('avatar')}
          disabled={isSwitching}
          className={outputMode === 'avatar' ? 'active' : ''}
        >
          👤 Avatar
        </button>
      </div>
      
      {isSwitching && <div>Switching mode...</div>}
      {error && <div className="error">{error}</div>}
    </div>
  );
}
```

### Mode-Aware UI Components

```tsx
function AdaptiveAgentOutput() {
  const {
    isTextMode,
    isVoiceMode,
    isAvatarMode,
    switchToText,
    switchToVoice,
    switchToAvatar
  } = useOutputMode();

  return (
    <div className="agent-output">
      {isTextMode && (
        <div className="text-output">
          <h3>Text Mode Active</h3>
          <p>Agent responses will appear as text only.</p>
          <button onClick={switchToVoice}>Enable Voice</button>
        </div>
      )}
      
      {isVoiceMode && (
        <div className="voice-output">
          <h3>🔊 Voice Mode Active</h3>
          <div className="audio-visualizer">
            {/* Audio visualization component */}
          </div>
          <button onClick={switchToText}>Switch to Text</button>
          <button onClick={switchToAvatar}>Enable Avatar</button>
        </div>
      )}
      
      {isAvatarMode && (
        <div className="avatar-output">
          <h3>Avatar Mode Active</h3>
          <div className="avatar-container">
            {/* Avatar video component */}
          </div>
          <button onClick={switchToVoice}>Voice Only</button>
        </div>
      )}
    </div>
  );
}
```

### Mode with Voice Selection

```tsx
function VoiceEnabledModes() {
  const {
    outputMode,
    setOutputMode,
    isVoiceMode,
    error
  } = useOutputMode({ 
    defaultVoiceId: 'echo' // Use 'echo' instead of default 'alloy'
  });
  
  const { 
    currentVoice, 
    availableVoices, 
    setVoice 
  } = useVoiceModel();

  const handleVoiceChange = async (voiceId: string) => {
    // Only allow voice changes in voice mode
    if (!isVoiceMode) {
      console.log('Switch to voice mode first');
      return;
    }
    
    try {
      await setVoice(voiceId);
    } catch (err) {
      console.error('Failed to change voice:', err);
    }
  };

  return (
    <div>
      <div className="mode-selector">
        <select 
          value={outputMode} 
          onChange={(e) => setOutputMode(e.target.value as OutputMode)}
        >
          <option value="text">Text Only</option>
          <option value="voice">Voice</option>
          <option value="avatar">Avatar</option>
        </select>
      </div>

      {isVoiceMode && (
        <div className="voice-selector">
          <label>Select Voice:</label>
          <select 
            value={currentVoice?.voice_id || ''} 
            onChange={(e) => handleVoiceChange(e.target.value)}
          >
            {availableVoices
              .filter(v => v.voice_id !== 'none' && v.voice_id !== 'avatar')
              .map(voice => (
                <option key={voice.voice_id} value={voice.voice_id}>
                  {voice.name}
                </option>
              ))}
          </select>
        </div>
      )}
      
      {error && <div className="error">Error: {error}</div>}
    </div>
  );
}
```

### Accessibility Mode Toggle

```tsx
function AccessibilityModeToggle() {
  const {
    outputMode,
    switchToText,
    switchToVoice,
    isSwitching
  } = useOutputMode();
  
  const [prefersTextOnly, setPrefersTextOnly] = useState(false);
  
  // React to accessibility preference
  useEffect(() => {
    if (prefersTextOnly && outputMode !== 'text') {
      switchToText();
    } else if (!prefersTextOnly && outputMode === 'text') {
      switchToVoice();
    }
  }, [prefersTextOnly, outputMode]);

  return (
    <div className="accessibility-controls">
      <label>
        <input
          type="checkbox"
          checked={prefersTextOnly}
          onChange={(e) => setPrefersTextOnly(e.target.checked)}
          disabled={isSwitching}
        />
        Text-only mode (accessibility)
      </label>
      
      <div className="mode-status">
        Current mode: {outputMode}
        {outputMode === 'text' && ' (No audio output)'}
      </div>
    </div>
  );
}
```

### Mode Persistence

```tsx
function PersistentOutputMode() {
  const savedMode = localStorage.getItem('preferredOutputMode') as OutputMode;
  
  const {
    outputMode,
    setOutputMode,
    error
  } = useOutputMode({
    initialMode: savedMode || 'voice',
    onModeChange: (newMode) => {
      localStorage.setItem('preferredOutputMode', newMode);
      console.log(`Mode saved: ${newMode}`);
    }
  });

  const modes: Array<{ value: OutputMode; label: string; icon: string }> = [
    { value: 'text', label: 'Text Only', icon: '📝' },
    { value: 'voice', label: 'Voice', icon: '🔊' },
    { value: 'avatar', label: 'Avatar', icon: '👤' }
  ];

  return (
    <div className="persistent-mode-selector">
      <h3>Output Preference (Saved)</h3>
      <div className="mode-grid">
        {modes.map(mode => (
          <button
            key={mode.value}
            onClick={() => setOutputMode(mode.value)}
            className={outputMode === mode.value ? 'selected' : ''}
          >
            <span className="icon">{mode.icon}</span>
            <span className="label">{mode.label}</span>
          </button>
        ))}
      </div>
      {error && <div className="error">Failed to switch: {error}</div>}
    </div>
  );
}
```

### Advanced Mode Management

```tsx
function AdvancedModeManager() {
  const {
    outputMode,
    setOutputMode,
    isTextMode,
    isVoiceMode,
    isAvatarMode,
    isSwitching,
    error
  } = useOutputMode();
  
  const [modeHistory, setModeHistory] = useState<OutputMode[]>([]);
  const [autoSwitch, setAutoSwitch] = useState(false);
  
  // Track mode changes
  useEffect(() => {
    setModeHistory(prev => [...prev, outputMode].slice(-10));
  }, [outputMode]);
  
  // Auto-switch demo
  useEffect(() => {
    if (!autoSwitch) return;
    
    const modes: OutputMode[] = ['text', 'voice', 'avatar'];
    let index = 0;
    
    const interval = setInterval(() => {
      setOutputMode(modes[index % 3]);
      index++;
    }, 3000);
    
    return () => clearInterval(interval);
  }, [autoSwitch, setOutputMode]);

  const getModeStats = () => {
    const counts = modeHistory.reduce((acc, mode) => {
      acc[mode] = (acc[mode] || 0) + 1;
      return acc;
    }, {} as Record<OutputMode, number>);
    
    return counts;
  };

  const stats = getModeStats();

  return (
    <div className="advanced-manager">
      <div className="current-mode">
        <h3>Current Mode: {outputMode}</h3>
        <div className="mode-indicators">
          <span className={isTextMode ? 'active' : ''}>TEXT</span>
          <span className={isVoiceMode ? 'active' : ''}>VOICE</span>
          <span className={isAvatarMode ? 'active' : ''}>AVATAR</span>
        </div>
      </div>

      <div className="mode-controls">
        <button
          onClick={() => setOutputMode('text')}
          disabled={isSwitching || isTextMode}
        >
          Text Mode
        </button>
        <button
          onClick={() => setOutputMode('voice')}
          disabled={isSwitching || isVoiceMode}
        >
          Voice Mode
        </button>
        <button
          onClick={() => setOutputMode('avatar')}
          disabled={isSwitching || isAvatarMode}
        >
          Avatar Mode
        </button>
      </div>

      <div className="auto-switch">
        <label>
          <input
            type="checkbox"
            checked={autoSwitch}
            onChange={(e) => setAutoSwitch(e.target.checked)}
          />
          Auto-switch demo (every 3s)
        </label>
      </div>

      <div className="mode-stats">
        <h4>Mode Usage (last 10 changes):</h4>
        <ul>
          <li>Text: {stats.text || 0}</li>
          <li>Voice: {stats.voice || 0}</li>
          <li>Avatar: {stats.avatar || 0}</li>
        </ul>
      </div>

      {error && (
        <div className="error-display">
          <strong>Error:</strong> {error}
        </div>
      )}
    </div>
  );
}
```

### Conditional Mode Availability

```tsx
function ConditionalModes() {
  const { availableVoices } = useVoiceModel();
  const {
    outputMode,
    setOutputMode,
    isSwitching,
    error
  } = useOutputMode();
  
  // Determine available modes based on voices
  const hasVoiceSupport = availableVoices.some(
    v => v.voice_id !== 'none' && v.voice_id !== 'avatar'
  );
  const hasAvatarSupport = availableVoices.some(
    v => v.voice_id === 'avatar'
  );

  const handleModeChange = (mode: OutputMode) => {
    // Validate mode availability
    if (mode === 'voice' && !hasVoiceSupport) {
      alert('Voice mode not available');
      return;
    }
    if (mode === 'avatar' && !hasAvatarSupport) {
      alert('Avatar mode not available');
      return;
    }
    
    setOutputMode(mode);
  };

  return (
    <div className="conditional-modes">
      <h3>Available Output Modes</h3>
      
      <div className="mode-options">
        <label className="mode-option">
          <input
            type="radio"
            name="mode"
            value="text"
            checked={outputMode === 'text'}
            onChange={() => handleModeChange('text')}
            disabled={isSwitching}
          />
          <span>Text Only ✅</span>
        </label>

        <label className={`mode-option ${!hasVoiceSupport ? 'disabled' : ''}`}>
          <input
            type="radio"
            name="mode"
            value="voice"
            checked={outputMode === 'voice'}
            onChange={() => handleModeChange('voice')}
            disabled={isSwitching || !hasVoiceSupport}
          />
          <span>Voice {hasVoiceSupport ? '✅' : '❌'}</span>
        </label>

        <label className={`mode-option ${!hasAvatarSupport ? 'disabled' : ''}`}>
          <input
            type="radio"
            name="mode"
            value="avatar"
            checked={outputMode === 'avatar'}
            onChange={() => handleModeChange('avatar')}
            disabled={isSwitching || !hasAvatarSupport}
          />
          <span>Avatar {hasAvatarSupport ? '✅' : '❌'}</span>
        </label>
      </div>

      {error && <div className="error">{error}</div>}
    </div>
  );
}
```

## Turn Management Integration Notes

While `useOutputMode` doesn't directly interact with turn management, the output mode affects how agent responses are delivered:

- **All modes respect turn state**: Regardless of output mode, agent responses follow turn management rules
- **Mode changes don't affect turns**: Switching modes doesn't interrupt or change the current turn
- **Audio playback in voice/avatar modes**: Audio output respects turn boundaries and doesn't overlap with user input

Example coordination:
```tsx
const { outputMode } = useOutputMode();
const { canSendInput } = useTurnState();

// Mode affects presentation, turn affects timing
if (!canSendInput && (outputMode === 'voice' || outputMode === 'avatar')) {
  // Agent is speaking with audio
}
```

## StrictMode Compatibility

The hook is fully StrictMode compatible:

- Handles double-mounting gracefully
- Effect cleanup prevents duplicate voice changes
- Initial mode setting happens once despite re-mounting
- No memory leaks from event subscriptions

## Best Practices for Audio/Voice Features

### Mode Selection Strategy

```tsx
// Provide sensible defaults based on context
const getInitialMode = (): OutputMode => {
  // Check user preferences
  const saved = localStorage.getItem('outputMode');
  if (saved) return saved as OutputMode;
  
  // Check device capabilities
  const hasAudio = 'AudioContext' in window;
  if (!hasAudio) return 'text';
  
  // Default to voice for best experience
  return 'voice';
};

const { outputMode } = useOutputMode({ 
  initialMode: getInitialMode() 
});
```

### Error Recovery

```tsx
// Implement fallback on errors
const { setOutputMode, error } = useOutputMode();

const safeModeSwitch = async (targetMode: OutputMode) => {
  try {
    await setOutputMode(targetMode);
  } catch (err) {
    console.error(`Failed to switch to ${targetMode}:`, err);
    // Fallback to text mode
    try {
      await setOutputMode('text');
      console.log('Fell back to text mode');
    } catch (fallbackErr) {
      console.error('Even text mode failed:', fallbackErr);
    }
  }
};
```

### Progressive Enhancement

```tsx
// Start simple, enhance based on capabilities
useEffect(() => {
  const checkAndUpgrade = async () => {
    // Start in text mode
    if (outputMode !== 'text') return;
    
    // Check for voice capability
    const { availableVoices } = await getVoices();
    if (availableVoices.length > 2) { // More than just 'none' and 'avatar'
      setOutputMode('voice');
    }
  };
  
  checkAndUpgrade();
}, []);
```

### User Preference Management

```tsx
// Remember and apply user preferences
const usePreferredMode = () => {
  const [preferred, setPreferred] = useState<OutputMode>('voice');
  
  const { outputMode, setOutputMode } = useOutputMode({
    initialMode: preferred,
    onModeChange: (mode) => {
      setPreferred(mode);
      savePreference('outputMode', mode);
    }
  });
  
  return { outputMode, setOutputMode, preferred };
};
```

## Performance Considerations

### Mode Switching Performance

- Mode changes trigger voice changes in the SDK
- Voice changes are asynchronous but typically fast (<100ms)
- UI updates immediately while SDK catches up
- Error states are handled without blocking UI

### State Synchronization

- Two-way binding with SDK voice state
- Automatic mode detection from voice changes
- Efficient re-renders using boolean flags
- No unnecessary voice API calls

### Memory and Resources

- No polling - all updates are event-driven
- Minimal state storage (just mode and error)
- Voice resources managed by underlying SDK
- Clean effect cleanup prevents leaks

### Optimization Best Practices

1. **Cache mode preferences**: Store in localStorage to avoid repeated API calls
2. **Debounce rapid switches**: Prevent mode thrashing with debounced switching
3. **Preload avatar resources**: If avatar mode is likely, preload resources early
4. **Use mode-specific components**: Lazy load components based on active mode
5. **Monitor switching frequency**: Track and limit rapid mode changes

Example optimization:
```tsx
const DebouncedModeSwitcher = () => {
  const { setOutputMode } = useOutputMode();
  
  const debouncedSetMode = useMemo(
    () => debounce(setOutputMode, 500),
    [setOutputMode]
  );
  
  return (
    <input
      type="range"
      min="0"
      max="2"
      onChange={(e) => {
        const modes: OutputMode[] = ['text', 'voice', 'avatar'];
        debouncedSetMode(modes[parseInt(e.target.value)]);
      }}
    />
  );
};
```