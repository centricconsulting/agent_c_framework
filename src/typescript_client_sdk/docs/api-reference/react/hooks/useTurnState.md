# useTurnState Hook

## Purpose and Overview

The `useTurnState` hook provides real-time access to turn management state in the Agent C Realtime SDK. It monitors whether the user can currently send input (has the turn) and optionally tracks turn state history for debugging or UI visualization. This hook is essential for building conversational interfaces that respect turn-taking protocols.

Key capabilities:
- Real-time turn state monitoring
- Turn history tracking with timestamps
- Automatic fallback for non-turn-managed sessions
- Event-driven updates for immediate UI responsiveness
- Configurable history tracking for debugging

## Import Statement

```typescript
import { useTurnState } from '@agentc/realtime-react';
```

## TypeScript Types

```typescript
interface UseTurnStateOptions {
  /** Whether to track turn state history */
  trackHistory?: boolean;
  
  /** Maximum number of history events to keep */
  maxHistorySize?: number;
}

interface TurnStateEvent {
  /** Whether user can send input */
  canSendInput: boolean;
  
  /** Timestamp of the event */
  timestamp: number;
  
  /** Event type for clarity */
  type: 'user_turn_start' | 'user_turn_end';
}

interface UseTurnStateReturn {
  /** Whether user can currently send input */
  canSendInput: boolean;
  
  /** Turn state history (if tracking is enabled) */
  turnStateHistory?: TurnStateEvent[];
  
  /** Whether a turn manager is available */
  hasTurnManager: boolean;
  
  /** Clear the turn state history */
  clearHistory?: () => void;
}
```

## Return Value Descriptions

### Core Properties

- **`canSendInput`**: Boolean indicating whether the user currently has permission to send input (audio or text). When `true`, the user has the turn and can speak/type. When `false`, the agent is speaking and user input should be prevented or queued
- **`hasTurnManager`**: Boolean indicating whether turn management is active. If `false`, the session doesn't use turn management and users can always send input

### History Tracking Properties (when `trackHistory: true`)

- **`turnStateHistory`**: Array of `TurnStateEvent` objects recording all turn changes with timestamps. Useful for debugging turn management or showing conversation flow visualization
- **`clearHistory`**: Function to clear the accumulated turn history. Only available when history tracking is enabled

## Usage Examples

### Basic Turn State Monitoring

```tsx
function TurnIndicator() {
  const { canSendInput, hasTurnManager } = useTurnState();

  if (!hasTurnManager) {
    return <div>Free conversation mode</div>;
  }

  return (
    <div className={`turn-indicator ${canSendInput ? 'active' : 'waiting'}`}>
      {canSendInput ? '🟢 Your turn to speak' : '🔴 Agent is speaking'}
    </div>
  );
}
```

### Input Control Based on Turn

```tsx
function TurnAwareInput() {
  const { canSendInput } = useTurnState();
  const [message, setMessage] = useState('');
  
  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    if (!canSendInput) {
      alert('Please wait for your turn to speak');
      return;
    }
    // Send message logic
    sendMessage(message);
    setMessage('');
  };

  return (
    <form onSubmit={handleSubmit}>
      <input
        type="text"
        value={message}
        onChange={(e) => setMessage(e.target.value)}
        disabled={!canSendInput}
        placeholder={canSendInput ? 'Type your message...' : 'Waiting for turn...'}
      />
      <button type="submit" disabled={!canSendInput}>
        Send
      </button>
    </form>
  );
}
```

### Turn History Visualization

```tsx
function TurnHistoryTimeline() {
  const { 
    canSendInput, 
    turnStateHistory, 
    clearHistory 
  } = useTurnState({ 
    trackHistory: true, 
    maxHistorySize: 50 
  });

  const formatTime = (timestamp: number) => {
    return new Date(timestamp).toLocaleTimeString();
  };

  const getDuration = (event: TurnStateEvent, index: number) => {
    if (index === turnStateHistory!.length - 1) {
      return 'ongoing';
    }
    const nextEvent = turnStateHistory![index + 1];
    const duration = (nextEvent.timestamp - event.timestamp) / 1000;
    return `${duration.toFixed(1)}s`;
  };

  return (
    <div className="turn-history">
      <div className="current-state">
        Current: {canSendInput ? 'User Turn' : 'Agent Turn'}
      </div>
      
      {turnStateHistory && (
        <>
          <div className="history-timeline">
            {turnStateHistory.map((event, index) => (
              <div key={event.timestamp} className={`event ${event.type}`}>
                <span className="time">{formatTime(event.timestamp)}</span>
                <span className="type">
                  {event.type === 'user_turn_start' ? '👤 User' : '🤖 Agent'}
                </span>
                <span className="duration">{getDuration(event, index)}</span>
              </div>
            ))}
          </div>
          <button onClick={clearHistory}>Clear History</button>
        </>
      )}
    </div>
  );
}
```

### Microphone Control with Turn State

```tsx
function TurnAwareMicrophone() {
  const { canSendInput } = useTurnState();
  const { startStreaming, stopStreaming, isStreaming } = useAudio();
  
  const handleMicrophoneToggle = async () => {
    if (!canSendInput) {
      console.log('Cannot use microphone - not your turn');
      return;
    }
    
    if (isStreaming) {
      stopStreaming();
    } else {
      try {
        await startStreaming();
      } catch (error) {
        console.error('Failed to start streaming:', error);
      }
    }
  };

  return (
    <button
      onClick={handleMicrophoneToggle}
      disabled={!canSendInput}
      className={`mic-button ${!canSendInput ? 'disabled' : ''}`}
    >
      {!canSendInput ? '⏸️ Wait for turn' : 
       isStreaming ? '🔴 Stop Recording' : '🎤 Start Recording'}
    </button>
  );
}
```

### Turn-Based Conversation UI

```tsx
function ConversationInterface() {
  const { canSendInput, hasTurnManager } = useTurnState();
  const [queuedMessage, setQueuedMessage] = useState('');
  
  // Auto-send queued message when turn arrives
  useEffect(() => {
    if (canSendInput && queuedMessage) {
      sendMessage(queuedMessage);
      setQueuedMessage('');
    }
  }, [canSendInput, queuedMessage]);

  const handleMessageSubmit = (message: string) => {
    if (canSendInput) {
      sendMessage(message);
    } else {
      setQueuedMessage(message);
      console.log('Message queued, waiting for turn');
    }
  };

  return (
    <div className="conversation">
      <div className="status-bar">
        {hasTurnManager ? (
          <>
            <span className={`indicator ${canSendInput ? 'green' : 'red'}`}>●</span>
            {canSendInput ? 'Your turn' : 'Agent speaking'}
            {queuedMessage && !canSendInput && (
              <span className="queued">Message queued...</span>
            )}
          </>
        ) : (
          <span>Open conversation</span>
        )}
      </div>
      
      <MessageInput 
        onSubmit={handleMessageSubmit}
        disabled={!canSendInput && hasTurnManager}
      />
    </div>
  );
}
```

### Turn Analytics Dashboard

```tsx
function TurnAnalytics() {
  const { 
    turnStateHistory 
  } = useTurnState({ 
    trackHistory: true, 
    maxHistorySize: 200 
  });

  const calculateStats = () => {
    if (!turnStateHistory || turnStateHistory.length < 2) {
      return null;
    }

    let userTime = 0;
    let agentTime = 0;
    let userTurns = 0;
    let agentTurns = 0;

    for (let i = 0; i < turnStateHistory.length - 1; i++) {
      const event = turnStateHistory[i];
      const nextEvent = turnStateHistory[i + 1];
      const duration = nextEvent.timestamp - event.timestamp;

      if (event.type === 'user_turn_start') {
        userTime += duration;
        userTurns++;
      } else {
        agentTime += duration;
        agentTurns++;
      }
    }

    return {
      userTime: (userTime / 1000).toFixed(1),
      agentTime: (agentTime / 1000).toFixed(1),
      userTurns,
      agentTurns,
      avgUserTurn: (userTime / userTurns / 1000).toFixed(1),
      avgAgentTurn: (agentTime / agentTurns / 1000).toFixed(1)
    };
  };

  const stats = calculateStats();

  return (
    <div className="turn-analytics">
      <h3>Conversation Analytics</h3>
      {stats ? (
        <dl>
          <dt>Total User Speaking Time:</dt>
          <dd>{stats.userTime}s</dd>
          
          <dt>Total Agent Speaking Time:</dt>
          <dd>{stats.agentTime}s</dd>
          
          <dt>User Turns:</dt>
          <dd>{stats.userTurns}</dd>
          
          <dt>Agent Turns:</dt>
          <dd>{stats.agentTurns}</dd>
          
          <dt>Avg User Turn Duration:</dt>
          <dd>{stats.avgUserTurn}s</dd>
          
          <dt>Avg Agent Turn Duration:</dt>
          <dd>{stats.avgAgentTurn}s</dd>
        </dl>
      ) : (
        <p>Not enough data for analytics</p>
      )}
    </div>
  );
}
```

### Visual Turn Indicator

```tsx
function VisualTurnIndicator() {
  const { canSendInput } = useTurnState();
  const [isFlashing, setIsFlashing] = useState(false);
  
  // Flash animation when turn changes
  useEffect(() => {
    setIsFlashing(true);
    const timer = setTimeout(() => setIsFlashing(false), 500);
    return () => clearTimeout(timer);
  }, [canSendInput]);

  return (
    <div className="visual-indicator">
      <div className={`
        turn-light 
        ${canSendInput ? 'user-active' : 'agent-active'}
        ${isFlashing ? 'flashing' : ''}
      `}>
        <div className="icon">
          {canSendInput ? '👤' : '🤖'}
        </div>
        <div className="label">
          {canSendInput ? 'Your Turn' : "Agent's Turn"}
        </div>
      </div>
      
      <style jsx>{`
        .turn-light {
          padding: 20px;
          border-radius: 10px;
          transition: all 0.3s ease;
        }
        
        .user-active {
          background: #4CAF50;
          color: white;
        }
        
        .agent-active {
          background: #f44336;
          color: white;
        }
        
        .flashing {
          animation: flash 0.5s ease;
        }
        
        @keyframes flash {
          0%, 100% { opacity: 1; }
          50% { opacity: 0.5; }
        }
      `}</style>
    </div>
  );
}
```

## Turn Management Integration Notes

### Turn State Behavior

The turn state system follows these rules:

1. **With Turn Manager**: When a TurnManager is present, turn state is strictly controlled by the server
2. **Without Turn Manager**: When no TurnManager exists, `canSendInput` is always `true` (free conversation mode)
3. **State Transitions**: Turn changes are event-driven and update immediately when the server sends turn change events

### Integration with Other Hooks

```tsx
// Coordinate with audio hook
const { canSendInput } = useTurnState();
const { startStreaming } = useAudio({ respectTurnState: true });

// Audio hook will respect turn state automatically
// but you can also check manually
if (canSendInput) {
  await startStreaming();
}
```

### Turn Loss Handling

```tsx
// Handle turn loss gracefully
const { canSendInput } = useTurnState();

useEffect(() => {
  if (!canSendInput) {
    // Stop any ongoing user actions
    stopRecording();
    pauseTyping();
    showWaitingIndicator();
  }
}, [canSendInput]);
```

## StrictMode Compatibility

The hook is designed for React StrictMode:

- Event subscriptions are properly cleaned up
- No duplicate event handlers during double-mounting
- State initialization happens correctly on remount
- History tracking maintains consistency across mounts

## Best Practices

### Visual Feedback
Always provide clear visual indication of turn state:
```tsx
// Use color, icons, and text for clarity
<div className={canSendInput ? 'active' : 'inactive'}>
  {canSendInput ? '✅ Speak now' : '⏳ Please wait'}
</div>
```

### Input Queuing
Consider queuing user input during agent turns:
```tsx
const [queue, setQueue] = useState<string[]>([]);

useEffect(() => {
  if (canSendInput && queue.length > 0) {
    queue.forEach(sendMessage);
    setQueue([]);
  }
}, [canSendInput, queue]);
```

### History Management
Use history tracking judiciously:
```tsx
// Only track history when needed for UI or debugging
const { turnStateHistory } = useTurnState({ 
  trackHistory: process.env.NODE_ENV === 'development',
  maxHistorySize: 20 // Keep memory usage low
});
```

### Error Prevention
Prevent user actions during wrong turns:
```tsx
const handleUserAction = () => {
  if (!canSendInput) {
    showToast('Please wait for your turn');
    return;
  }
  // Proceed with action
};
```

## Performance Considerations

### Event-Driven Updates
- No polling required - all updates are event-driven
- Immediate state updates when turn changes occur
- Minimal re-renders through efficient state management

### History Tracking Performance
- History is only stored when explicitly enabled
- Automatic size limiting prevents memory growth
- Consider disabling in production for better performance

### Memory Management
- Event listeners are properly cleaned up
- History array is capped at `maxHistorySize`
- No memory leaks from stale closures

### Optimization Tips

1. **Disable history in production**: Only enable history tracking during development or when specifically needed
2. **Use memo for derived values**: Memoize calculations based on turn state to prevent unnecessary recalculation
3. **Batch UI updates**: Group multiple UI changes based on turn state to minimize reflows
4. **Debounce rapid changes**: If turn state changes rapidly, consider debouncing UI updates
5. **Lazy load history UI**: Only render history visualization when user requests it

### Example Performance Optimization

```tsx
const OptimizedTurnUI = memo(() => {
  const { canSendInput } = useTurnState();
  
  // Memoize expensive calculations
  const uiState = useMemo(() => ({
    color: canSendInput ? 'green' : 'red',
    icon: canSendInput ? '🟢' : '🔴',
    message: canSendInput ? 'Your turn' : 'Please wait'
  }), [canSendInput]);
  
  return <TurnDisplay {...uiState} />;
});
```