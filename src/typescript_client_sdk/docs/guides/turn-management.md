# Turn Management Guide

This guide explains conversation turn management in the Agent C Realtime SDK, ensuring smooth dialogue flow and preventing audio conflicts.

## Overview

Turn management coordinates who can speak when, preventing users and agents from talking over each other. The Agent C platform uses server-driven turn control for consistent behavior across all clients.

## Turn States

```typescript
enum TurnState {
  USER_TURN = 'user_turn',    // User can speak
  AGENT_TURN = 'agent_turn',  // Agent is responding
  IDLE = 'idle'               // No active conversation
}
```

## Turn Lifecycle

```
    IDLE
     ↓
USER_TURN ←─────┐
     ↓          │
  (user speaks) │
     ↓          │
AGENT_TURN      │
     ↓          │
 (agent responds)
     └──────────┘
```

## Basic Turn Management

### Enable Turn Management

```typescript
import { RealtimeClient, AuthManager } from '@agentc/realtime-core';

// First authenticate with Agent C
const authManager = new AuthManager({
  apiUrl: 'https://localhost:8000'
});

// Login with username/password
const loginResponse = await authManager.login('username', 'password');

// Create client with WebSocket URL from login response
const client = new RealtimeClient({
  apiUrl: loginResponse.websocketUrl,  // URL provided by login
  authManager,
  enableTurnManager: true,  // Enabled by default
  audioConfig: {
    respectTurnState: true   // Audio respects turns
  }
});

await client.connect();

// Access turn manager
const turnManager = client.getTurnManager();
```

### Monitor Turn Changes

```typescript
turnManager.on('turn:changed', (event) => {
  console.log(`Turn changed: ${event.from} → ${event.to}`);
  console.log(`Previous turn duration: ${event.duration}ms`);
});

turnManager.on('turn:user_start', () => {
  console.log('Your turn to speak');
  enableMicrophone();
});

turnManager.on('turn:user_end', () => {
  console.log('Please wait...');
  disableMicrophone();
});

turnManager.on('turn:agent_start', () => {
  console.log('Agent is responding');
  showAgentIndicator();
});

turnManager.on('turn:agent_end', () => {
  console.log('Agent finished');
  hideAgentIndicator();
});
```

## Turn-Aware Audio

### Automatic Audio Control

When `respectTurnState` is enabled, audio streaming automatically follows turn rules:

```typescript
// Audio only streams during user turns
const client = new RealtimeClient({
  audioConfig: {
    respectTurnState: true  // Default: true
  }
});

// Start recording
await client.startAudioRecording();
client.startAudioStreaming();

// Audio will automatically:
// - Stream during USER_TURN
// - Pause during AGENT_TURN
// - Resume on next USER_TURN
```

### Manual Turn Checking

```typescript
// Check if audio can be transmitted
if (turnManager.canTransmitAudio()) {
  // Safe to send audio
  client.sendBinaryFrame(audioData);
} else {
  // Wait for user turn
  console.log('Waiting for your turn...');
}
```

## UI Integration

### Visual Turn Indicators

```typescript
function TurnIndicator() {
  const { currentTurn, isUserTurn, isAgentTurn, turnDuration } = useTurnState();
  
  return (
    <div className={`turn-indicator ${currentTurn}`}>
      {isUserTurn && (
        <div className="user-turn">
          <span className="icon">🎤</span>
          <span className="text">Your turn to speak</span>
          <span className="duration">{(turnDuration / 1000).toFixed(1)}s</span>
        </div>
      )}
      
      {isAgentTurn && (
        <div className="agent-turn">
          <span className="icon">🤖</span>
          <span className="text">Agent is speaking</span>
          <div className="typing-indicator">
            <span></span><span></span><span></span>
          </div>
        </div>
      )}
      
      {!isUserTurn && !isAgentTurn && (
        <div className="idle">
          <span className="icon">⏸️</span>
          <span className="text">Ready</span>
        </div>
      )}
    </div>
  );
}
```

### Microphone Control

```typescript
class MicrophoneController {
  private isEnabled = false;
  
  constructor(
    private turnManager: TurnManager,
    private audioService: AudioService
  ) {
    this.setupTurnHandlers();
  }
  
  private setupTurnHandlers() {
    this.turnManager.on('turn:user_start', () => {
      this.enableMicrophone();
    });
    
    this.turnManager.on('turn:user_end', () => {
      this.disableMicrophone();
    });
  }
  
  private async enableMicrophone() {
    if (!this.isEnabled) {
      try {
        await this.audioService.startRecording();
        this.isEnabled = true;
        this.updateUI(true);
      } catch (error) {
        console.error('Failed to enable microphone:', error);
      }
    }
  }
  
  private disableMicrophone() {
    if (this.isEnabled) {
      this.audioService.stopRecording();
      this.isEnabled = false;
      this.updateUI(false);
    }
  }
  
  private updateUI(enabled: boolean) {
    const button = document.getElementById('mic-button');
    if (button) {
      button.classList.toggle('active', enabled);
      button.textContent = enabled ? '🔴 Recording' : '🎤 Muted';
    }
  }
}
```

## Turn Timing

### Turn Duration Tracking

```typescript
// Get current turn duration
const duration = turnManager.getTurnDuration();
console.log(`Current turn has lasted ${duration}ms`);

// Track turn history
const history = turnManager.getTurnHistory();
history.forEach(entry => {
  console.log(`${entry.state}: ${entry.duration}ms at ${new Date(entry.timestamp)}`);
});

// Get statistics
const stats = turnManager.getStats();
console.log('Turn Statistics:');
console.log(`Total turns: ${stats.totalTurns}`);
console.log(`User turns: ${stats.userTurns}`);
console.log(`Agent turns: ${stats.agentTurns}`);
console.log(`Avg user turn: ${stats.averageUserTurnDuration}ms`);
console.log(`Avg agent turn: ${stats.averageAgentTurnDuration}ms`);
```

### Turn Timeouts

```typescript
class TurnTimeoutManager {
  private maxUserTurnDuration = 60000;  // 1 minute
  private maxAgentTurnDuration = 120000; // 2 minutes
  private timeoutTimer: NodeJS.Timeout | null = null;
  
  constructor(private turnManager: TurnManager) {
    this.setupTimeouts();
  }
  
  private setupTimeouts() {
    this.turnManager.on('turn:changed', (event) => {
      this.clearTimeout();
      
      if (event.to === TurnState.USER_TURN) {
        this.setTimeout(this.maxUserTurnDuration, 'user');
      } else if (event.to === TurnState.AGENT_TURN) {
        this.setTimeout(this.maxAgentTurnDuration, 'agent');
      }
    });
  }
  
  private setTimeout(duration: number, type: 'user' | 'agent') {
    this.timeoutTimer = setTimeout(() => {
      console.warn(`${type} turn timeout after ${duration}ms`);
      this.handleTimeout(type);
    }, duration);
  }
  
  private clearTimeout() {
    if (this.timeoutTimer) {
      clearTimeout(this.timeoutTimer);
      this.timeoutTimer = null;
    }
  }
  
  private handleTimeout(type: 'user' | 'agent') {
    if (type === 'user') {
      // User taking too long, maybe prompt them
      this.showPrompt('Are you still there?');
    } else {
      // Agent taking too long, maybe show status
      this.showStatus('Processing your request...');
    }
  }
  
  private showPrompt(message: string) {
    // Show UI prompt
  }
  
  private showStatus(message: string) {
    // Show status message
  }
}
```

## Advanced Turn Patterns

### Interruption Handling

```typescript
class InterruptionHandler {
  private isInterrupting = false;
  
  constructor(
    private client: RealtimeClient,
    private turnManager: TurnManager
  ) {}
  
  interrupt() {
    if (this.turnManager.isAgentTurn) {
      this.isInterrupting = true;
      
      // Send interruption signal
      this.client.sendText('[INTERRUPT]');
      
      // Force user turn locally (testing only)
      if (DEBUG_MODE) {
        this.turnManager.forceUserTurn();
      }
    }
  }
  
  handleInterruptionResponse() {
    if (this.isInterrupting) {
      console.log('Interruption acknowledged');
      this.isInterrupting = false;
    }
  }
}
```

### Barge-In Support

```typescript
class BargeInController {
  private bargeInEnabled = true;
  private minimumSpeechDuration = 500; // 500ms minimum
  private speechStartTime = 0;
  
  constructor(
    private audioService: AudioService,
    private turnManager: TurnManager
  ) {
    this.setupBargeIn();
  }
  
  private setupBargeIn() {
    this.audioService.on('audio:speech-start', () => {
      this.speechStartTime = Date.now();
      
      if (this.bargeInEnabled && this.turnManager.isAgentTurn) {
        // Check if user has spoken long enough
        setTimeout(() => {
          const duration = Date.now() - this.speechStartTime;
          if (duration >= this.minimumSpeechDuration) {
            this.performBargeIn();
          }
        }, this.minimumSpeechDuration);
      }
    });
  }
  
  private performBargeIn() {
    console.log('User barge-in detected');
    // Signal server to stop agent and switch to user turn
    // Server will handle the actual turn switch
  }
}
```

### Multi-Party Turns

```typescript
class MultiPartyTurnManager {
  private participants = new Map<string, ParticipantState>();
  
  addParticipant(id: string, name: string) {
    this.participants.set(id, {
      id,
      name,
      isSpeaking: false,
      lastSpokeAt: 0
    });
  }
  
  updateSpeakingState(participantId: string, isSpeaking: boolean) {
    const participant = this.participants.get(participantId);
    if (participant) {
      participant.isSpeaking = isSpeaking;
      if (isSpeaking) {
        participant.lastSpokeAt = Date.now();
      }
    }
    
    this.updateTurnIndicator();
  }
  
  private updateTurnIndicator() {
    const speaking = Array.from(this.participants.values())
      .filter(p => p.isSpeaking);
    
    if (speaking.length === 0) {
      this.showStatus('No one is speaking');
    } else if (speaking.length === 1) {
      this.showStatus(`${speaking[0].name} is speaking`);
    } else {
      this.showStatus(`Multiple people speaking`);
      // Could implement collision detection
    }
  }
  
  private showStatus(message: string) {
    // Update UI
  }
}

interface ParticipantState {
  id: string;
  name: string;
  isSpeaking: boolean;
  lastSpokeAt: number;
}
```

## Turn Coordination

### Text and Voice Coordination

```typescript
class HybridTurnManager {
  private lastInputType: 'text' | 'voice' = 'text';
  
  constructor(
    private client: RealtimeClient,
    private turnManager: TurnManager
  ) {}
  
  sendTextMessage(text: string) {
    this.lastInputType = 'text';
    
    // Text input triggers turn change
    this.client.sendText(text);
    
    // Expect agent turn next
    this.expectAgentResponse();
  }
  
  startVoiceInput() {
    this.lastInputType = 'voice';
    
    // Voice requires user turn
    if (this.turnManager.isUserTurn) {
      this.client.startAudioStreaming();
    } else {
      console.log('Waiting for your turn to speak');
      
      // Queue for next user turn
      this.turnManager.once('turn:user_start', () => {
        this.client.startAudioStreaming();
      });
    }
  }
  
  private expectAgentResponse() {
    // Prepare UI for agent response based on last input type
    if (this.lastInputType === 'voice') {
      // Expect voice response
      this.prepareAudioPlayback();
    } else {
      // Expect text response
      this.prepareTextDisplay();
    }
  }
  
  private prepareAudioPlayback() {
    // Setup audio output
  }
  
  private prepareTextDisplay() {
    // Setup text display
  }
}
```

### Turn Queueing

```typescript
class TurnQueue {
  private queue: QueuedAction[] = [];
  
  constructor(private turnManager: TurnManager) {
    this.setupQueueProcessor();
  }
  
  enqueue(action: QueuedAction) {
    this.queue.push(action);
    this.processQueue();
  }
  
  private setupQueueProcessor() {
    this.turnManager.on('turn:user_start', () => {
      this.processQueue();
    });
  }
  
  private processQueue() {
    if (this.queue.length === 0) return;
    
    if (this.turnManager.isUserTurn) {
      const action = this.queue.shift()!;
      this.executeAction(action);
    }
  }
  
  private executeAction(action: QueuedAction) {
    switch (action.type) {
      case 'speak':
        this.client.sendText(action.text);
        break;
      case 'audio':
        this.client.startAudioStreaming();
        break;
    }
  }
}

interface QueuedAction {
  type: 'speak' | 'audio';
  text?: string;
}
```

## Testing Turn Management

### Turn Simulation

```typescript
class TurnSimulator {
  constructor(private turnManager: TurnManager) {}
  
  async simulateConversation() {
    console.log('Starting turn simulation...');
    
    // Simulate user turn
    this.turnManager.forceUserTurn();
    await this.delay(2000);
    console.log('User speaking for 2s');
    
    // Simulate agent turn
    this.turnManager.forceAgentTurn();
    await this.delay(3000);
    console.log('Agent responding for 3s');
    
    // Back to user
    this.turnManager.forceUserTurn();
    await this.delay(1500);
    console.log('User speaking for 1.5s');
    
    // Get statistics
    const stats = this.turnManager.getStats();
    console.log('Simulation complete:', stats);
  }
  
  private delay(ms: number): Promise<void> {
    return new Promise(resolve => setTimeout(resolve, ms));
  }
}
```

### Turn Debugging

```typescript
class TurnDebugger {
  private eventLog: TurnEvent[] = [];
  
  constructor(private turnManager: TurnManager) {
    this.attachDebugListeners();
  }
  
  private attachDebugListeners() {
    // Log all turn events
    const events = [
      'turn:changed',
      'turn:user_start',
      'turn:user_end',
      'turn:agent_start',
      'turn:agent_end'
    ];
    
    events.forEach(event => {
      this.turnManager.on(event, (data) => {
        this.logEvent(event, data);
      });
    });
  }
  
  private logEvent(event: string, data: any) {
    const entry: TurnEvent = {
      event,
      data,
      timestamp: Date.now(),
      turnState: this.turnManager.currentTurn,
      canTransmit: this.turnManager.canTransmitAudio()
    };
    
    this.eventLog.push(entry);
    console.debug('Turn Event:', entry);
  }
  
  exportLog(): string {
    return JSON.stringify(this.eventLog, null, 2);
  }
  
  analyzeLog() {
    const analysis = {
      totalEvents: this.eventLog.length,
      turnChanges: this.eventLog.filter(e => e.event === 'turn:changed').length,
      averageGap: this.calculateAverageGap(),
      longestTurn: this.findLongestTurn()
    };
    
    return analysis;
  }
  
  private calculateAverageGap(): number {
    // Calculate average time between turns
    const changes = this.eventLog.filter(e => e.event === 'turn:changed');
    if (changes.length < 2) return 0;
    
    let totalGap = 0;
    for (let i = 1; i < changes.length; i++) {
      totalGap += changes[i].timestamp - changes[i-1].timestamp;
    }
    
    return totalGap / (changes.length - 1);
  }
  
  private findLongestTurn(): TurnEvent | null {
    // Find the longest turn duration
    return this.eventLog
      .filter(e => e.event === 'turn:changed' && e.data.duration)
      .sort((a, b) => b.data.duration - a.data.duration)[0] || null;
  }
}

interface TurnEvent {
  event: string;
  data: any;
  timestamp: number;
  turnState: TurnState;
  canTransmit: boolean;
}
```

## Best Practices

### 1. Always Respect Turn State

```typescript
// Good - respects turns
const client = new RealtimeClient({
  audioConfig: {
    respectTurnState: true
  }
});

// Bad - ignores turns (only for testing)
const client = new RealtimeClient({
  audioConfig: {
    respectTurnState: false
  }
});
```

### 2. Provide Clear Visual Feedback

```typescript
function TurnStatusBar() {
  const { currentTurn, turnDuration } = useTurnState();
  
  return (
    <div className="turn-status-bar">
      <div className="turn-indicator">
        {currentTurn === TurnState.USER_TURN && '🟢 Your Turn'}
        {currentTurn === TurnState.AGENT_TURN && '🔴 Agent Turn'}
        {currentTurn === TurnState.IDLE && '⚪ Ready'}
      </div>
      <div className="turn-timer">
        {formatDuration(turnDuration)}
      </div>
    </div>
  );
}
```

### 3. Handle Edge Cases

```typescript
// Handle disconnection during turn
client.on('disconnected', () => {
  turnManager.reset();
  // Reset UI to safe state
});

// Handle errors during turn
client.on('error', () => {
  const state = turnManager.currentTurn;
  console.log('Error during:', state);
  // Don't assume turn state is correct
});
```

### 4. Monitor Turn Health

```typescript
setInterval(() => {
  const duration = turnManager.getTurnDuration();
  
  if (duration > 60000) {
    console.warn('Turn has been active for over 1 minute');
    // Maybe show timeout warning
  }
  
  const stats = turnManager.getStats();
  if (stats.averageUserTurnDuration > 30000) {
    console.log('Long user turns detected');
    // Could adjust UI or prompts
  }
}, 5000);
```

### 5. Graceful Degradation

```typescript
// Fallback for turn management issues
class TurnFallback {
  private fallbackMode = false;
  
  enableFallback() {
    this.fallbackMode = true;
    console.warn('Turn management fallback enabled');
    
    // Allow manual control
    document.getElementById('manual-mic-control').style.display = 'block';
  }
  
  disableFallback() {
    this.fallbackMode = false;
    document.getElementById('manual-mic-control').style.display = 'none';
  }
}
```

## Troubleshooting

### Turn State Stuck

```typescript
// Debug stuck turn state
if (turnManager.getTurnDuration() > 120000) {
  console.error('Turn stuck for over 2 minutes');
  
  // Log state
  console.log('Current turn:', turnManager.currentTurn);
  console.log('Can transmit:', turnManager.canTransmitAudio());
  console.log('Turn history:', turnManager.getTurnHistory());
  
  // Reset if needed (testing only)
  if (DEBUG_MODE) {
    turnManager.reset();
  }
}
```

### Audio Not Transmitting

```typescript
// Check why audio isn't transmitting
const debugAudioTransmission = () => {
  console.log('=== Audio Transmission Debug ===');
  console.log('Connected:', client.isConnected());
  console.log('Current turn:', turnManager.currentTurn);
  console.log('Is user turn:', turnManager.isUserTurn);
  console.log('Can transmit:', turnManager.canTransmitAudio());
  console.log('Audio recording:', audioService.getStatus().isRecording);
  console.log('Audio streaming:', audioBridge.getStatus().isStreaming);
  console.log('Respect turn state:', audioConfig.respectTurnState);
};
```

### Turn Metrics

```typescript
// Collect turn metrics for analysis
class TurnMetrics {
  collect(): TurnMetricsData {
    const stats = turnManager.getStats();
    const history = turnManager.getTurnHistory();
    
    return {
      timestamp: Date.now(),
      stats,
      history: history.slice(-10), // Last 10 turns
      currentTurn: turnManager.currentTurn,
      currentDuration: turnManager.getTurnDuration(),
      queueLength: audioOutput.getQueueLength()
    };
  }
  
  send(metrics: TurnMetricsData) {
    // Send to analytics service
    analytics.track('turn_metrics', metrics);
  }
}
```

## Summary

Turn management ensures smooth conversations by:

1. **Coordinating who can speak when**
2. **Preventing audio conflicts**
3. **Providing clear visual feedback**
4. **Automatically controlling audio streaming**
5. **Tracking conversation flow**

The Agent C turn management system is server-driven, ensuring consistent behavior across all clients while providing flexibility for different interaction patterns.