# Test Harness Documentation

## Overview

The Test Harness provides a comprehensive system for loading and testing chat sessions in the demo application. It enables developers to validate chat rendering, test edge cases, and ensure the proper functioning of the EventStreamProcessor's message-to-event conversion.

## Features

- **Test Data Loading**: Load predefined JSON test sessions
- **Session Resumption**: Programmatically resume sessions with test data
- **Test Mode Toggle**: Enable/disable test mode via environment variable or runtime
- **Scenario Management**: Switch between multiple test scenarios
- **UI Controls**: Interactive controls for managing test sessions
- **TypeScript Support**: Fully typed interfaces for test data structures

## Quick Start

### 1. Enable Test Mode

#### Via Environment Variable
```bash
# In your .env.local file
NEXT_PUBLIC_TEST_MODE=true
```

#### Via Runtime Toggle (Browser Console)
```javascript
localStorage.setItem('agentc_test_mode', 'true');
location.reload();
```

### 2. Use Test Controls in Your Component

```typescript
import { useTestSession } from '@/hooks/use-test-session';
import { TestControls } from '@/components/test';

function ChatComponent() {
  const { testModeEnabled, loadScenario } = useTestSession();

  if (testModeEnabled) {
    return <TestControls />;
  }

  return <YourNormalChat />;
}
```

### 3. Load a Test Scenario

```typescript
const { loadScenario, scenarios } = useTestSession();

// Load a predefined scenario
await loadScenario('session-with-delegation');

// Or load the first available scenario
if (scenarios.length > 0) {
  await loadScenario(scenarios[0].id);
}
```

## Architecture

### Components

#### Test Data Loader Service (`services/test-data-loader.ts`)
- Loads and validates JSON test files
- Caches loaded sessions
- Parses delegation tool results
- Provides utility methods for content processing

#### Test Session Manager (`services/test-session-manager.ts`)
- Manages test scenarios and configurations
- Handles session resumption
- Controls test mode state
- Coordinates with chat SDK

#### useTestSession Hook (`hooks/use-test-session.tsx`)
- React hook for test functionality
- Integrates with useChat from SDK
- Manages UI state for test controls
- Handles scenario loading

#### Test Controls Component (`components/test/test-controls.tsx`)
- UI for scenario selection
- Test mode toggle
- Session management buttons
- Error and loading states

### Data Flow

1. **Test Data Loading**
   ```
   JSON File → TestDataLoader → Validation → Cache
   ```

2. **Session Resumption**
   ```
   TestSessionManager → Create Chat Session → Select Session → EventStreamProcessor
   ```

3. **Event Conversion**
   ```
   Message Array → EventStreamProcessor.mapResumedMessagesToEvents() → Rendered Events
   ```

## Test Session Format

### Basic Structure

```json
{
  "version": 1,
  "session_id": "unique-session-id",
  "session_name": "Human Readable Name",
  "created_at": "2025-01-01T10:00:00.000000",
  "updated_at": "2025-01-01T10:05:00.000000",
  "user_id": "test-user",
  "messages": [...],
  "agent_config": {...}
}
```

### Message Types

#### Simple Text Message
```json
{
  "role": "user",
  "content": "Hello, how are you?"
}
```

#### Complex Assistant Message
```json
{
  "role": "assistant",
  "content": [
    {
      "type": "text",
      "text": "I'm doing well, thank you!",
      "citations": null
    }
  ]
}
```

#### Tool Use Message
```json
{
  "role": "assistant",
  "content": [
    {
      "type": "tool_use",
      "id": "tool_123",
      "name": "think",
      "input": {
        "thought": "Processing the user's request..."
      }
    }
  ]
}
```

#### Delegation Tool Message
```json
{
  "role": "assistant",
  "content": [
    {
      "type": "tool_use",
      "id": "tool_456",
      "name": "ateam_oneshot",
      "input": {
        "request": "Analyze this data",
        "process_context": "Focus on accuracy",
        "agent_key": "analyzer_agent"
      }
    }
  ]
}
```

## Predefined Scenarios

### session-with-delegation
- Contains delegation tool calls and subsessions
- Tests complex message rendering
- Validates tool_use block handling

### simple-chat
- Basic conversation without tools
- Tests standard message flow
- Validates text rendering

### streaming-test
- Long messages with formatting
- Code blocks and special characters
- Tests streaming message display

### error-handling
- System messages and warnings
- Connection error scenarios
- Unicode and special character handling

## API Reference

### useTestSession Hook

```typescript
interface UseTestSessionReturn {
  // State
  testModeEnabled: boolean;
  testConfig: TestModeConfig;
  scenarios: TestScenario[];
  currentScenario: TestScenario | null;
  isLoading: boolean;
  error: string | null;
  
  // Actions
  enableTestMode: (enabled: boolean) => void;
  loadScenario: (scenarioId: string) => Promise<void>;
  clearSession: () => void;
  reloadCurrentScenario: () => Promise<void>;
  
  // UI Controls
  showTestControls: boolean;
  setShowTestControls: (show: boolean) => void;
}
```

### TestSessionManager Methods

```typescript
class TestSessionManager {
  // Test mode control
  setTestMode(enabled: boolean): void;
  isEnabled(): boolean;
  
  // Scenario management
  getScenarios(): TestScenario[];
  loadScenario(scenarioId: string): Promise<TestSession>;
  resumeSession(sessionOrId: TestSession | string): Promise<void>;
  
  // Custom scenarios
  addCustomScenario(scenario: TestScenario): void;
  removeScenario(scenarioId: string): boolean;
  
  // Data loading
  loadTestDataFromFile(filePath: string): Promise<TestSession>;
  loadTestDataFromObject(data: TestSession): Promise<TestSession>;
}
```

## Adding Custom Test Scenarios

### 1. Create a JSON File

Place your test file in `public/test-data/`:

```json
{
  "version": 1,
  "session_id": "my-custom-test",
  "session_name": "My Custom Test",
  "messages": [
    {
      "role": "user",
      "content": "Test message"
    }
  ]
}
```

### 2. Register the Scenario

```typescript
import { testSessionManager } from '@/services/test-session-manager';

testSessionManager.addCustomScenario({
  id: 'my-custom-test',
  name: 'My Custom Test',
  description: 'Tests specific functionality',
  filePath: '/test-data/my-custom-test.json',
  tags: ['custom', 'specific']
});
```

### 3. Load and Use

```typescript
const { loadScenario } = useTestSession();
await loadScenario('my-custom-test');
```

## Testing Workflow

### Manual Testing

1. Enable test mode in development
2. Open the application
3. Use test controls to load scenarios
4. Verify message rendering
5. Check for proper event conversion
6. Test edge cases

### Automated Testing

```typescript
import { testSessionManager } from '@/services/test-session-manager';
import { renderHook, waitFor } from '@testing-library/react';
import { useTestSession } from '@/hooks/use-test-session';

test('loads test scenario correctly', async () => {
  const { result } = renderHook(() => useTestSession());
  
  await act(async () => {
    await result.current.loadScenario('session-with-delegation');
  });
  
  await waitFor(() => {
    expect(result.current.currentScenario).toBeDefined();
    expect(result.current.currentScenario?.id).toBe('session-with-delegation');
  });
});
```

## Troubleshooting

### Test Mode Not Activating

1. Check environment variable is set
2. Verify localStorage value
3. Ensure component re-renders after enabling

### Scenarios Not Loading

1. Verify file exists in `public/test-data/`
2. Check browser network tab for 404s
3. Validate JSON structure
4. Check console for validation errors

### Messages Not Displaying

1. Ensure WebSocket connection is established
2. Verify session is properly resumed
3. Check EventStreamProcessor is processing messages
4. Look for errors in browser console

## Best Practices

1. **Keep Test Data Realistic**: Use actual message formats from production
2. **Test Edge Cases**: Include error scenarios, long messages, special characters
3. **Document Scenarios**: Provide clear descriptions and tags
4. **Version Control**: Track test data files in git
5. **Clean Up**: Clear test sessions when switching scenarios
6. **Monitor Performance**: Watch for memory leaks with large test sessions

## Security Considerations

- Test mode should only be enabled in development/staging
- Don't include sensitive data in test files
- Test files are publicly accessible via URL
- Consider access controls for production deployments

## Future Enhancements

- [ ] Test data generator from production sessions
- [ ] Automated test runner for all scenarios
- [ ] Performance benchmarking mode
- [ ] Visual regression testing integration
- [ ] Export/import test scenarios
- [ ] Test coverage reporting