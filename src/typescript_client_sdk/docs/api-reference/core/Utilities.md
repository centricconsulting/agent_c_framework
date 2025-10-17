# Utilities API Reference

The Agent C Realtime SDK includes essential utility components that support core functionality across the SDK. These utilities provide logging, audio processing, and message handling capabilities.

## Utility Components

### [Logger](./Logger.md)
**Purpose:** Structured logging with environment-aware levels  
**Location:** `@agentc/realtime-core/utils/logger`

The Logger provides centralized, structured logging with:
- Environment-aware default levels (ERROR for tests, DEBUG for development, WARN for production)
- Five log levels: ERROR, WARN, INFO, DEBUG, TRACE
- Consistent formatting with component prefixes
- Runtime configurable logging levels

```typescript
import { Logger, LogLevel } from '@agentc/realtime-core';

Logger.setLevel(LogLevel.DEBUG);
Logger.info('[Component] Initialized');
Logger.error('[Component] Error occurred', error);
```

---

### [AudioOutputService](./AudioOutputService.md)
**Purpose:** Manages TTS audio playback from the server  
**Location:** `@agentc/realtime-core/audio`

Singleton service for playing PCM16 audio chunks with:
- Automatic voice model awareness (skip for avatar/none modes)
- Smooth playback with chunk queuing and overlap
- Volume control and playback statistics
- Real-time status monitoring

```typescript
import { AudioOutputService } from '@agentc/realtime-core';

const audioOutput = AudioOutputService.getInstance();
await audioOutput.playAudioChunk(audioData);
audioOutput.setVolume(0.5);
```

---

### [AudioProcessor](./AudioProcessor.md)
**Purpose:** Captures and processes microphone input  
**Location:** `@agentc/realtime-core/audio`

Manages audio input with AudioWorklet processing:
- Real-time PCM16 conversion in separate thread
- Automatic resampling from native to 16kHz
- Audio level monitoring and chunk generation
- Microphone permission handling

```typescript
import { AudioProcessor } from '@agentc/realtime-core';

const processor = new AudioProcessor({
  workletPath: '/audio-worklet-processor.js',
  sampleRate: 16000
});

await processor.initialize();
await processor.startProcessing();

processor.on('audioChunk', (chunk) => {
  websocket.send(chunk.content);
});
```

---

### [Message Utilities](./MessageUtilities.md)
**Purpose:** Message normalization and format conversion  
**Location:** `@agentc/realtime-core/utils`

Two modules for message handling:

#### messageUtils
- `normalizeMessageContent()` - Ensure content is string format
- `normalizeMessage()` - Validate and normalize message structure
- `isValidMessage()` - Type guard for message validation

#### message-converter
- `convertMessageParamToMessage()` - Convert server to client format
- `ensureMessageFormat()` - Ensure proper UI rendering format
- Handles content blocks, multi-part messages, and special fields

```typescript
import { 
  normalizeMessage, 
  convertMessageParamToMessage 
} from '@agentc/realtime-core/utils';

// Normalize user input
const message = normalizeMessage({ 
  role: 'user', 
  content: userInput 
});

// Convert server response
const clientMessage = convertMessageParamToMessage(serverParam);
```

## Common Integration Patterns

### Logging Throughout Components

```typescript
import { Logger } from '@agentc/realtime-core';

export class MyComponent {
  private readonly logPrefix = '[MyComponent]';
  
  initialize() {
    Logger.debug(`${this.logPrefix} Initializing`);
    try {
      // ... initialization logic
      Logger.info(`${this.logPrefix} Initialized successfully`);
    } catch (error) {
      Logger.error(`${this.logPrefix} Initialization failed`, error);
      throw error;
    }
  }
}
```

### Audio Pipeline Setup

```typescript
import { 
  AudioProcessor, 
  AudioOutputService 
} from '@agentc/realtime-core';

class AudioPipeline {
  private processor: AudioProcessor;
  private output = AudioOutputService.getInstance();
  
  async initialize() {
    // Set up input
    this.processor = new AudioProcessor();
    await this.processor.initialize();
    
    // Set up output
    this.output.setVolume(1.0);
    
    // Connect to WebSocket
    this.processor.on('audioChunk', (chunk) => {
      this.websocket.send(chunk.content);
    });
    
    this.websocket.on('message', async (data) => {
      if (data instanceof ArrayBuffer) {
        await this.output.playAudioChunk(data);
      }
    });
  }
}
```

### Message Processing Pipeline

```typescript
import {
  normalizeMessage,
  convertMessageParamToMessage,
  isValidMessage
} from '@agentc/realtime-core/utils';
import { Logger } from '@agentc/realtime-core';

class MessageProcessor {
  processIncoming(data: any): Message | null {
    // Try to convert from server format
    if (data.role && data.content) {
      const message = convertMessageParamToMessage(data);
      Logger.debug('[MessageProcessor] Converted server message', message);
      return message;
    }
    
    // Validate and normalize
    if (isValidMessage(data)) {
      return data;
    }
    
    // Attempt normalization
    const normalized = normalizeMessage(data);
    if (normalized.content) {
      Logger.warn('[MessageProcessor] Normalized invalid message', data);
      return normalized;
    }
    
    Logger.error('[MessageProcessor] Failed to process message', data);
    return null;
  }
}
```

## Best Practices

### 1. Consistent Error Handling

```typescript
import { Logger } from '@agentc/realtime-core';

async function safeOperation() {
  try {
    // ... operation logic
  } catch (error) {
    Logger.error('[Component] Operation failed', error);
    // Handle gracefully
    throw error;  // Re-throw if needed
  }
}
```

### 2. Resource Cleanup

```typescript
// Always clean up audio resources
window.addEventListener('beforeunload', async () => {
  await processor.cleanup();
  await audioOutput.cleanup();
});

// In React components
useEffect(() => {
  const processor = new AudioProcessor();
  processor.initialize();
  
  return () => {
    processor.cleanup();
  };
}, []);
```

### 3. Type Safety

```typescript
import { isValidMessage, Message } from '@agentc/realtime-core';

function handleMessage(data: unknown) {
  // Type guard ensures type safety
  if (isValidMessage(data)) {
    // TypeScript knows data is Message type
    processMessage(data);
  } else {
    handleInvalidData(data);
  }
}
```

### 4. Environment-Aware Configuration

```typescript
import { Logger, LogLevel } from '@agentc/realtime-core';

// Set appropriate log level based on environment
if (process.env.NODE_ENV === 'development') {
  Logger.setLevel(LogLevel.DEBUG);
} else if (process.env.DEBUG === 'true') {
  Logger.setLevel(LogLevel.TRACE);
}
```

## Testing Utilities

### Mock Logger in Tests

```typescript
import { vi } from 'vitest';
import { Logger } from '@agentc/realtime-core';

describe('Component', () => {
  beforeEach(() => {
    vi.spyOn(Logger, 'error');
    vi.spyOn(Logger, 'info');
  });
  
  it('should log errors', () => {
    // ... trigger error
    expect(Logger.error).toHaveBeenCalledWith(
      expect.stringContaining('[Component]'),
      expect.any(Error)
    );
  });
});
```

### Test Audio Without Hardware

```typescript
import { AudioProcessor } from '@agentc/realtime-core';

// Mock getUserMedia for testing
global.navigator.mediaDevices = {
  getUserMedia: vi.fn().mockResolvedValue(mockMediaStream)
};

// Mock AudioContext
global.AudioContext = MockAudioContext;
```

### Validate Message Handling

```typescript
import { normalizeMessage, isValidMessage } from '@agentc/realtime-core/utils';

describe('Message handling', () => {
  it('should normalize various formats', () => {
    const inputs = [
      { role: 'user', content: 'text' },
      { role: 'assistant', content: { text: 'wrapped' } },
      { role: 'system', content: null }
    ];
    
    inputs.forEach(input => {
      const normalized = normalizeMessage(input);
      expect(isValidMessage(normalized)).toBe(true);
      expect(typeof normalized.content).toBe('string');
    });
  });
});
```

## Performance Guidelines

- **Logger:** Minimal overhead with level checks before formatting
- **AudioOutputService:** Efficient Web Audio API usage with queuing
- **AudioProcessor:** Separate thread processing via AudioWorklet
- **Message Utils:** Lightweight synchronous operations

## Security Considerations

- **Logger:** Never log sensitive data (passwords, API keys, PII)
- **Audio:** HTTPS required for microphone access
- **Messages:** Sanitize user input before processing
- **WebSocket:** Use secure connections (wss://) in production

## Troubleshooting

### Common Issues

| Component | Issue | Solution |
|-----------|-------|----------|
| Logger | No output in tests | Check log level (default ERROR in tests) |
| AudioOutputService | No audio playback | Check voice model and enabled state |
| AudioProcessor | No microphone access | Ensure HTTPS and handle permissions |
| Message Utils | Content lost | Check for unsupported content types |

### Debug Mode

Enable detailed logging for troubleshooting:

```typescript
import { Logger, LogLevel } from '@agentc/realtime-core';

// Enable trace logging
Logger.setLevel(LogLevel.TRACE);

// Enable audio debugging
const processor = new AudioProcessor({ debug: true });
```

## Package Exports

All utilities are exported from `@agentc/realtime-core`:

```typescript
// Logger
export { Logger, LogLevel } from './utils/logger';

// Audio
export { AudioOutputService } from './audio/AudioOutputService';
export { AudioProcessor } from './audio/AudioProcessor';

// Message utilities  
export {
  normalizeMessage,
  normalizeMessages,
  normalizeMessageContent,
  isValidMessage,
  convertMessageParamToMessage,
  convertMessageParamsToMessages,
  ensureMessageFormat,
  ensureMessagesFormat
} from './utils';
```

## Related Documentation

- [RealtimeClient](./RealtimeClient.md) - Main client using utilities
- [WebSocketManager](./WebSocketManager.md) - Audio streaming integration
- [SessionManager](./SessionManager.md) - Message history with normalization
- [Testing Standards](../testing_standards_and_architecture.md) - Testing utilities