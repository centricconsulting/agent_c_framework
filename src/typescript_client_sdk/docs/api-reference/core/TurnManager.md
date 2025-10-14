# TurnManager API Reference

The `TurnManager` class manages conversation turns between the user and agent, preventing audio conflicts and ensuring smooth conversation flow.

## Import

```typescript
import { TurnManager } from '@agentc/realtime-core';
```

## Overview

The TurnManager handles server-driven turn control to prevent users and agents from talking over each other. It tracks whose turn it is to speak and can optionally gate audio transmission based on turn state.

## Constructor

```typescript
constructor(client: RealtimeClient)
```

**Note:** TurnManager is typically created automatically by RealtimeClient when `enableTurnManager: true` is set in the config.

### Example

```typescript
// Setup authentication first
const authManager = new AuthManager({
  apiUrl: 'https://localhost:8000'
});

await authManager.login('username', 'password');

// TurnManager is created automatically
const client = new RealtimeClient({
  apiUrl: 'wss://localhost:8000/rt/ws',
  authManager,
  enableTurnManager: true  // Default: true
});

// Access the TurnManager instance
const turnManager = client.getTurnManager();
```

## Properties

### currentTurn

Gets the current turn state.

```typescript
get currentTurn(): TurnState
```

**Returns:** Current TurnState enum value

```typescript
enum TurnState {
  USER_TURN = 'user_turn',
  AGENT_TURN = 'agent_turn',
  IDLE = 'idle'
}
```

**Example:**
```typescript
const turn = turnManager.currentTurn;
if (turn === TurnState.USER_TURN) {
  console.log('User can speak');
} else if (turn === TurnState.AGENT_TURN) {
  console.log('Agent is speaking');
}
```

### isUserTurn

Checks if it's currently the user's turn.

```typescript
get isUserTurn(): boolean
```

**Returns:** `true` if user's turn, `false` otherwise

**Example:**
```typescript
if (turnManager.isUserTurn) {
  // Enable microphone
  client.startAudioStreaming();
}
```

### isAgentTurn

Checks if it's currently the agent's turn.

```typescript
get isAgentTurn(): boolean
```

**Returns:** `true` if agent's turn, `false` otherwise

**Example:**
```typescript
if (turnManager.isAgentTurn) {
  // Show "Agent is typing..." indicator
  showAgentTyping();
}
```

## Methods

### canTransmitAudio()

Checks if audio transmission is allowed based on turn state.

```typescript
canTransmitAudio(): boolean
```

**Returns:** `true` if audio can be transmitted

**Note:** Always returns `true` if turn management is disabled or respectTurnState is false

**Example:**
```typescript
// Check before streaming audio
if (turnManager.canTransmitAudio()) {
  client.sendBinaryFrame(audioData);
} else {
  console.log('Waiting for user turn...');
}
```

### getTurnHistory()

Gets the history of turn changes.

```typescript
getTurnHistory(): TurnHistoryEntry[]
```

**Returns:** Array of turn history entries

```typescript
interface TurnHistoryEntry {
  state: TurnState;
  timestamp: number;
  duration?: number;  // Duration of previous turn in ms
}
```

**Example:**
```typescript
const history = turnManager.getTurnHistory();
history.forEach(entry => {
  const time = new Date(entry.timestamp).toLocaleTimeString();
  console.log(`${time}: ${entry.state} (duration: ${entry.duration}ms)`);
});

// Get last 5 turns
const recentTurns = history.slice(-5);
```

### getTurnDuration()

Gets the duration of the current turn.

```typescript
getTurnDuration(): number
```

**Returns:** Duration in milliseconds

**Example:**
```typescript
const duration = turnManager.getTurnDuration();
console.log(`Current turn duration: ${duration}ms`);

if (duration > 30000) {
  console.log('Turn has been active for over 30 seconds');
}
```

### getStats()

Gets turn management statistics.

```typescript
getStats(): TurnStats
```

**Returns:** Turn statistics object

```typescript
interface TurnStats {
  totalTurns: number;
  userTurns: number;
  agentTurns: number;
  averageUserTurnDuration: number;
  averageAgentTurnDuration: number;
  currentSessionDuration: number;
}
```

**Example:**
```typescript
const stats = turnManager.getStats();
console.log('Turn Statistics:');
console.log(`Total turns: ${stats.totalTurns}`);
console.log(`User turns: ${stats.userTurns}`);
console.log(`Agent turns: ${stats.agentTurns}`);
console.log(`Avg user turn: ${stats.averageUserTurnDuration}ms`);
console.log(`Avg agent turn: ${stats.averageAgentTurnDuration}ms`);
```

### reset()

Resets the turn manager to initial state.

```typescript
reset(): void
```

**Example:**
```typescript
// Reset when starting new conversation
client.newChatSession();
turnManager.reset();
```

### forceUserTurn()

Manually forces a user turn (for testing/debugging).

```typescript
forceUserTurn(): void
```

**Warning:** This should only be used for testing as it can conflict with server state

**Example:**
```typescript
// Testing only
if (DEBUG_MODE) {
  turnManager.forceUserTurn();
}
```

### forceAgentTurn()

Manually forces an agent turn (for testing/debugging).

```typescript
forceAgentTurn(): void
```

**Warning:** This should only be used for testing as it can conflict with server state

## Event Handling

The TurnManager listens to and emits turn-related events:

### Emitted Events

- `turn:changed` - Turn state changed
- `turn:user_start` - User turn started
- `turn:user_end` - User turn ended
- `turn:agent_start` - Agent turn started
- `turn:agent_end` - Agent turn ended

### on()

Subscribe to turn events.

```typescript
on(event: string, handler: Function): void
```

**Example:**
```typescript
turnManager.on('turn:changed', (data: TurnChangeEvent) => {
  console.log(`Turn changed: ${data.from} → ${data.to}`);
  console.log(`Previous turn duration: ${data.duration}ms`);
});

turnManager.on('turn:user_start', () => {
  console.log('🎤 Your turn to speak');
  enableMicrophone();
});

turnManager.on('turn:user_end', () => {
  console.log('🔇 User turn ended');
  disableMicrophone();
});

turnManager.on('turn:agent_start', () => {
  console.log('🤖 Agent is responding');
  showAgentIndicator();
});

turnManager.on('turn:agent_end', () => {
  console.log('✅ Agent finished');
  hideAgentIndicator();
});
```

### Event Data Types

```typescript
interface TurnChangeEvent {
  from: TurnState;
  to: TurnState;
  timestamp: number;
  duration: number;  // Duration of previous turn
}
```

## Integration with Audio

The TurnManager integrates with the audio system to prevent talk-over:

```typescript
// When respectTurnState is enabled in audio config
const client = new RealtimeClient({
  audioConfig: {
    respectTurnState: true  // Audio respects turn management
  }
});

// Audio will automatically check turn state before transmission
// This happens internally in AudioAgentCBridge:

if (turnManager.canTransmitAudio()) {
  // Transmit audio
} else {
  // Buffer or drop audio
}
```

## Turn State Lifecycle

```
Initial State: IDLE
       │
       ▼
User speaks or types
       │
       ▼
  USER_TURN ──────► user_turn_start event
       │
       ▼
User finishes input
       │
       ▼
  AGENT_TURN ─────► agent_turn_start event
       │
       ▼
Agent completes response
       │
       ▼
  USER_TURN ──────► user_turn_start event
       │
     (cycle continues)
```

## Complete Example

```typescript
import { RealtimeClient, AuthManager } from '@agentc/realtime-core';

async function turnManagementExample() {
  // Setup authentication
  const authManager = new AuthManager({
    apiUrl: 'https://localhost:8000'
  });
  
  await authManager.login('username', 'password');
  
  // Create client with turn management
  const client = new RealtimeClient({
    apiUrl: 'wss://localhost:8000/rt/ws',
    authManager,
    enableTurnManager: true,
    audioConfig: {
      enableInput: true,
      enableOutput: true,
      respectTurnState: true  // Audio respects turns
    }
  });
  
  // Get turn manager instance
  const turnManager = client.getTurnManager();
  
  if (!turnManager) {
    console.error('Turn manager not initialized');
    return;
  }
  
  // UI state management
  let microphoneEnabled = false;
  let agentSpeaking = false;
  
  // Subscribe to turn events
  turnManager.on('turn:user_start', () => {
    console.log('🎤 Your turn to speak');
    microphoneEnabled = true;
    updateUI();
    
    // Automatically start streaming if recording
    const audioStatus = client.getAudioStatus();
    if (audioStatus.isRecording && !audioStatus.isStreaming) {
      client.startAudioStreaming();
    }
  });
  
  turnManager.on('turn:user_end', () => {
    console.log('🔇 Please wait...');
    microphoneEnabled = false;
    updateUI();
    
    // Stop streaming to prevent talk-over
    if (client.getAudioStatus().isStreaming) {
      client.stopAudioStreaming();
    }
  });
  
  turnManager.on('turn:agent_start', () => {
    console.log('🤖 Agent is speaking');
    agentSpeaking = true;
    updateUI();
  });
  
  turnManager.on('turn:agent_end', () => {
    console.log('✅ Agent finished');
    agentSpeaking = false;
    updateUI();
  });
  
  turnManager.on('turn:changed', (event) => {
    console.log(`Turn: ${event.from} → ${event.to}`);
    
    // Log turn duration
    if (event.duration > 0) {
      const seconds = (event.duration / 1000).toFixed(1);
      console.log(`Previous turn lasted ${seconds} seconds`);
    }
  });
  
  // Connect and start conversation
  await client.connect();
  
  // Monitor turn state
  setInterval(() => {
    const state = turnManager.currentTurn;
    const duration = turnManager.getTurnDuration();
    const canTransmit = turnManager.canTransmitAudio();
    
    console.log(`Current: ${state}, Duration: ${duration}ms, Can transmit: ${canTransmit}`);
  }, 1000);
  
  // Send initial message (will trigger turn management)
  client.sendText('Hello! Can you help me?');
  
  // After some conversation, check statistics
  setTimeout(() => {
    const stats = turnManager.getStats();
    console.log('\n📊 Conversation Statistics:');
    console.log(`Total turns: ${stats.totalTurns}`);
    console.log(`User spoke ${stats.userTurns} times`);
    console.log(`Agent responded ${stats.agentTurns} times`);
    console.log(`Average user turn: ${(stats.averageUserTurnDuration / 1000).toFixed(1)}s`);
    console.log(`Average agent turn: ${(stats.averageAgentTurnDuration / 1000).toFixed(1)}s`);
    
    // Get turn history
    const history = turnManager.getTurnHistory();
    console.log(`\n📜 Last 3 turns:`);
    history.slice(-3).forEach(entry => {
      const time = new Date(entry.timestamp).toLocaleTimeString();
      console.log(`  ${time}: ${entry.state}`);
    });
  }, 30000);
  
  function updateUI() {
    // Update your UI based on turn state
    const micButton = document.getElementById('mic-button');
    const statusText = document.getElementById('status');
    
    if (microphoneEnabled) {
      micButton?.classList.add('active');
      statusText!.textContent = 'Your turn to speak';
    } else if (agentSpeaking) {
      micButton?.classList.remove('active');
      statusText!.textContent = 'Agent is speaking...';
    } else {
      micButton?.classList.remove('active');
      statusText!.textContent = 'Waiting...';
    }
  }
}

turnManagementExample().catch(console.error);
```

## Best Practices

1. **Always respect turn state for audio:**
```typescript
// Good - respects turns
const client = new RealtimeClient({
  audioConfig: {
    respectTurnState: true
  }
});

// Bad - might cause talk-over
const client = new RealtimeClient({
  audioConfig: {
    respectTurnState: false
  }
});
```

2. **Use turn events for UI updates:**
```typescript
turnManager.on('turn:user_start', () => {
  enableMicrophoneButton();
  showSpeakingIndicator();
});

turnManager.on('turn:agent_start', () => {
  disableMicrophoneButton();
  showAgentThinking();
});
```

3. **Monitor turn duration for timeouts:**
```typescript
setInterval(() => {
  const duration = turnManager.getTurnDuration();
  if (duration > 60000) {  // 1 minute
    console.warn('Turn has been active for over 1 minute');
    // Maybe show a timeout warning
  }
}, 5000);
```

4. **Handle edge cases gracefully:**
```typescript
// Handle disconnection during turn
client.on('disconnected', () => {
  turnManager.reset();
  // Reset UI to safe state
});

// Handle errors during turn
client.on('error', () => {
  // Don't assume turn state is correct
  const state = turnManager.currentTurn;
  console.log('Error occurred during:', state);
});
```

5. **Use statistics for analytics:**
```typescript
// Track conversation quality
client.on('disconnected', () => {
  const stats = turnManager.getStats();
  analytics.track('conversation_ended', {
    total_turns: stats.totalTurns,
    avg_user_turn_duration: stats.averageUserTurnDuration,
    avg_agent_turn_duration: stats.averageAgentTurnDuration,
    total_duration: stats.currentSessionDuration
  });
});
```

## Troubleshooting

### Common Issues

**Microphone stays disabled:**
- Check that turn management is working: `turnManager.currentTurn`
- Verify WebSocket connection: `client.isConnected()`
- Check audio config: `respectTurnState` might be false

**Audio cuts off mid-sentence:**
- Turn might be ending prematurely
- Check network latency
- Consider increasing buffer time

**Turn state seems stuck:**
- Check for errors in console
- Try `turnManager.reset()` (testing only)
- Reconnect to server

**Statistics show incorrect values:**
- Ensure turn manager was enabled from start
- Check that events are being received from server
- Reset statistics with `turnManager.reset()`

## TypeScript Types

```typescript
import {
  TurnManager,
  TurnState,
  TurnHistoryEntry,
  TurnStats,
  TurnChangeEvent
} from '@agentc/realtime-core';
```

All properties and methods are fully typed for TypeScript applications.