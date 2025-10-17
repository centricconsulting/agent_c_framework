# useToolNotifications

React hook for tracking and managing agent tool execution notifications in the Agent C Realtime Client SDK. Provides real-time visibility into tool calls, their status, and results.

## Overview

The `useToolNotifications` hook monitors agent tool executions, providing real-time status updates as tools are prepared, executed, and completed. It maintains a history of tool notifications and completed tool calls with their results, enabling rich UI feedback for agent activities.

### Key Features
- Real-time tool execution status tracking
- Tool call history with results
- Auto-removal of completed notifications
- Configurable notification limits
- Active tool detection and counting
- StrictMode compatibility with proper cleanup
- Type-safe TypeScript interface

## Import

```typescript
import { useToolNotifications } from '@agentc/realtime-react';
// or
import { useToolNotifications } from '@agentc/realtime-react/hooks';
```

## TypeScript Types

### Core Types

```typescript
/**
 * Tool notification data structure
 */
interface ToolNotification {
  id: string;
  toolName: string;
  status: 'preparing' | 'executing' | 'complete';
  timestamp: Date;
  arguments?: string;
}

/**
 * Tool call result structure
 */
interface ToolCallResult {
  id: string;
  toolName: string;
  arguments: string;
  result: string;
  timestamp: Date;
}

/**
 * Options for the useToolNotifications hook
 */
interface UseToolNotificationsOptions {
  /** Maximum number of notifications to keep in memory */
  maxNotifications?: number;
  
  /** Whether to auto-remove completed notifications */
  autoRemoveCompleted?: boolean;
  
  /** Delay in ms before auto-removing completed notifications */
  autoRemoveDelay?: number;
}

/**
 * Return type for the useToolNotifications hook
 */
interface UseToolNotificationsReturn {
  /** Active tool notifications */
  notifications: ToolNotification[];
  
  /** Completed tool calls with results */
  completedToolCalls: ToolCallResult[];
  
  /** Get notification by ID */
  getNotification: (id: string) => ToolNotification | undefined;
  
  /** Clear all notifications */
  clearNotifications: () => void;
  
  /** Check if any tools are active */
  hasActiveTools: boolean;
  
  /** Check if a specific tool is active */
  isToolActive: (toolName: string) => boolean;
  
  /** Get count of active tools */
  activeToolCount: number;
}
```

## Return Values

### notifications
- **Type:** `ToolNotification[]`
- **Description:** Array of current tool notifications, ordered by timestamp. Includes preparing, executing, and optionally completed tools.
- **Initial Value:** Empty array

### completedToolCalls
- **Type:** `ToolCallResult[]`
- **Description:** Array of completed tool calls with their results. Maintained separately from active notifications.
- **Initial Value:** Empty array

### getNotification
- **Type:** `(id: string) => ToolNotification | undefined`
- **Description:** Retrieves a specific notification by its ID.
- **Returns:** The notification object or undefined if not found

### clearNotifications
- **Type:** `() => void`
- **Description:** Clears all notifications and completed tool calls. Also cancels any pending auto-removal timers.

### hasActiveTools
- **Type:** `boolean`
- **Description:** Computed property indicating if any tools are currently preparing or executing.
- **Initial Value:** `false`

### isToolActive
- **Type:** `(toolName: string) => boolean`
- **Description:** Checks if a specific tool is currently active (preparing or executing).
- **Returns:** Boolean indicating tool's active status

### activeToolCount
- **Type:** `number`
- **Description:** Computed count of currently active tools (preparing or executing).
- **Initial Value:** `0`

## Configuration Options

### maxNotifications
- **Type:** `number`
- **Default:** `10`
- **Description:** Maximum number of notifications to keep in memory. Older notifications are removed when limit is exceeded.

### autoRemoveCompleted
- **Type:** `boolean`
- **Default:** `false`
- **Description:** Whether to automatically remove completed notifications after a delay.

### autoRemoveDelay
- **Type:** `number`
- **Default:** `3000` (3 seconds)
- **Description:** Delay in milliseconds before auto-removing completed notifications.

## Usage Examples

### Basic Tool Status Display

```tsx
import { useToolNotifications } from '@agentc/realtime-react';

function ToolStatusIndicator() {
  const {
    notifications,
    hasActiveTools,
    activeToolCount
  } = useToolNotifications();

  if (!hasActiveTools) {
    return <div>Agent is idle</div>;
  }

  return (
    <div className="tool-status">
      <h3>Active Tools ({activeToolCount})</h3>
      <ul>
        {notifications
          .filter(n => n.status !== 'complete')
          .map(notification => (
            <li key={notification.id}>
              <span className={`status-${notification.status}`}>
                {notification.status}
              </span>
              <span>{notification.toolName}</span>
              <time>{notification.timestamp.toLocaleTimeString()}</time>
            </li>
          ))}
      </ul>
    </div>
  );
}
```

### Tool Activity Feed with Auto-Removal

```tsx
import { useToolNotifications } from '@agentc/realtime-react';

function ToolActivityFeed() {
  const {
    notifications,
    completedToolCalls,
    clearNotifications
  } = useToolNotifications({
    maxNotifications: 20,
    autoRemoveCompleted: true,
    autoRemoveDelay: 5000 // Remove completed after 5 seconds
  });

  return (
    <div className="activity-feed">
      <div className="feed-header">
        <h3>Tool Activity</h3>
        <button onClick={clearNotifications}>Clear All</button>
      </div>
      
      <div className="active-tools">
        {notifications.map(notification => (
          <div 
            key={notification.id} 
            className={`notification ${notification.status}`}
          >
            <div className="tool-name">{notification.toolName}</div>
            <div className="tool-status">
              {notification.status === 'preparing' && '⏳ Preparing...'}
              {notification.status === 'executing' && '⚡ Executing...'}
              {notification.status === 'complete' && '✅ Complete'}
            </div>
            {notification.arguments && (
              <details>
                <summary>Arguments</summary>
                <pre>{notification.arguments}</pre>
              </details>
            )}
          </div>
        ))}
      </div>
      
      {completedToolCalls.length > 0 && (
        <div className="completed-tools">
          <h4>Recent Results</h4>
          {completedToolCalls.map(call => (
            <details key={call.id}>
              <summary>
                {call.toolName} - {call.timestamp.toLocaleTimeString()}
              </summary>
              <pre>{call.result}</pre>
            </details>
          ))}
        </div>
      )}
    </div>
  );
}
```

### Tool-Specific Monitoring

```tsx
import { useToolNotifications } from '@agentc/realtime-react';
import { useEffect, useState } from 'react';

function SpecificToolMonitor({ toolName }: { toolName: string }) {
  const {
    notifications,
    isToolActive,
    getNotification
  } = useToolNotifications();
  
  const [executionHistory, setExecutionHistory] = useState<{
    timestamp: Date;
    duration: number;
    status: string;
  }[]>([]);

  // Track execution times for specific tool
  useEffect(() => {
    const toolNotifications = notifications.filter(
      n => n.toolName === toolName
    );
    
    toolNotifications.forEach(notification => {
      if (notification.status === 'complete') {
        // Calculate duration if we have a start time
        const startNotification = notifications.find(
          n => n.id === notification.id && n.status === 'preparing'
        );
        
        if (startNotification) {
          const duration = 
            notification.timestamp.getTime() - 
            startNotification.timestamp.getTime();
          
          setExecutionHistory(prev => [...prev, {
            timestamp: notification.timestamp,
            duration,
            status: 'success'
          }]);
        }
      }
    });
  }, [notifications, toolName]);

  const isActive = isToolActive(toolName);
  
  return (
    <div className="tool-monitor">
      <h4>
        {toolName}
        {isActive && <span className="active-badge">Active</span>}
      </h4>
      
      {executionHistory.length > 0 && (
        <div className="execution-stats">
          <p>Executions: {executionHistory.length}</p>
          <p>
            Avg Duration: {
              Math.round(
                executionHistory.reduce((sum, h) => sum + h.duration, 0) / 
                executionHistory.length
              )
            }ms
          </p>
        </div>
      )}
    </div>
  );
}
```

### Advanced Tool Catalog Integration

```tsx
import { useToolNotifications } from '@agentc/realtime-react';
import { useState, useEffect } from 'react';

// Tool catalog with metadata
const TOOL_CATALOG = {
  'search_web': {
    name: 'Web Search',
    icon: '🔍',
    description: 'Searches the internet for information'
  },
  'analyze_data': {
    name: 'Data Analysis',
    icon: '📊',
    description: 'Analyzes and processes data'
  },
  'generate_image': {
    name: 'Image Generation',
    icon: '🎨',
    description: 'Creates images from text descriptions'
  },
  'send_email': {
    name: 'Email Sender',
    icon: '📧',
    description: 'Composes and sends emails'
  }
};

function ToolDashboard() {
  const {
    notifications,
    completedToolCalls,
    hasActiveTools,
    activeToolCount,
    isToolActive,
    clearNotifications
  } = useToolNotifications({
    maxNotifications: 50,
    autoRemoveCompleted: false
  });

  const [toolStats, setToolStats] = useState<Map<string, {
    count: number;
    lastUsed: Date | null;
    totalDuration: number;
  }>>(new Map());

  // Calculate tool statistics
  useEffect(() => {
    const stats = new Map();
    
    completedToolCalls.forEach(call => {
      const existing = stats.get(call.toolName) || {
        count: 0,
        lastUsed: null,
        totalDuration: 0
      };
      
      stats.set(call.toolName, {
        count: existing.count + 1,
        lastUsed: call.timestamp,
        totalDuration: existing.totalDuration + 100 // Estimate
      });
    });
    
    setToolStats(stats);
  }, [completedToolCalls]);

  return (
    <div className="tool-dashboard">
      <div className="dashboard-header">
        <h2>Agent Tool Activity</h2>
        <div className="status-indicators">
          <span className={`indicator ${hasActiveTools ? 'active' : 'idle'}`}>
            {hasActiveTools ? `${activeToolCount} Active` : 'Idle'}
          </span>
          <button onClick={clearNotifications}>Clear History</button>
        </div>
      </div>

      <div className="tool-catalog">
        {Object.entries(TOOL_CATALOG).map(([toolId, toolInfo]) => {
          const isActive = isToolActive(toolId);
          const stats = toolStats.get(toolId);
          const activeNotification = notifications.find(
            n => n.toolName === toolId && n.status !== 'complete'
          );

          return (
            <div 
              key={toolId} 
              className={`tool-card ${isActive ? 'active' : ''}`}
            >
              <div className="tool-header">
                <span className="tool-icon">{toolInfo.icon}</span>
                <h3>{toolInfo.name}</h3>
                {isActive && (
                  <span className="status-badge">
                    {activeNotification?.status}
                  </span>
                )}
              </div>
              
              <p className="tool-description">{toolInfo.description}</p>
              
              {stats && (
                <div className="tool-stats">
                  <span>Used {stats.count} times</span>
                  {stats.lastUsed && (
                    <span>Last: {stats.lastUsed.toLocaleTimeString()}</span>
                  )}
                </div>
              )}
              
              {activeNotification?.arguments && (
                <div className="active-arguments">
                  <strong>Current Arguments:</strong>
                  <code>{activeNotification.arguments}</code>
                </div>
              )}
            </div>
          );
        })}
      </div>

      <div className="recent-results">
        <h3>Recent Tool Results</h3>
        <div className="results-list">
          {completedToolCalls.slice(-5).reverse().map(call => {
            const toolInfo = TOOL_CATALOG[call.toolName as keyof typeof TOOL_CATALOG];
            return (
              <div key={call.id} className="result-item">
                <div className="result-header">
                  <span>{toolInfo?.icon || '🔧'}</span>
                  <span>{toolInfo?.name || call.toolName}</span>
                  <time>{call.timestamp.toLocaleTimeString()}</time>
                </div>
                <div className="result-content">
                  <div className="arguments">
                    <strong>Input:</strong>
                    <pre>{call.arguments}</pre>
                  </div>
                  <div className="result">
                    <strong>Output:</strong>
                    <pre>{call.result}</pre>
                  </div>
                </div>
              </div>
            );
          })}
        </div>
      </div>
    </div>
  );
}
```

### Real-time Tool Progress Indicator

```tsx
import { useToolNotifications } from '@agentc/realtime-react';
import { useEffect, useState } from 'react';

function ToolProgressBar() {
  const { notifications, hasActiveTools } = useToolNotifications();
  const [progress, setProgress] = useState<Map<string, number>>(new Map());

  useEffect(() => {
    if (!hasActiveTools) {
      setProgress(new Map());
      return;
    }

    // Simulate progress for active tools
    const interval = setInterval(() => {
      setProgress(prev => {
        const next = new Map(prev);
        
        notifications
          .filter(n => n.status !== 'complete')
          .forEach(notification => {
            const current = next.get(notification.id) || 0;
            if (notification.status === 'preparing') {
              next.set(notification.id, Math.min(current + 10, 30));
            } else if (notification.status === 'executing') {
              next.set(notification.id, Math.min(current + 5, 95));
            }
          });
        
        return next;
      });
    }, 500);

    return () => clearInterval(interval);
  }, [notifications, hasActiveTools]);

  if (!hasActiveTools) {
    return null;
  }

  return (
    <div className="tool-progress-container">
      {notifications
        .filter(n => n.status !== 'complete')
        .map(notification => (
          <div key={notification.id} className="tool-progress">
            <div className="progress-label">
              <span>{notification.toolName}</span>
              <span>{progress.get(notification.id) || 0}%</span>
            </div>
            <div className="progress-bar">
              <div 
                className="progress-fill"
                style={{ width: `${progress.get(notification.id) || 0}%` }}
              />
            </div>
          </div>
        ))}
    </div>
  );
}
```

## Event Handling and Tool Execution Flow

### Tool Lifecycle Events

The hook monitors the following tool execution stages:

1. **Tool Preparation** (`status: 'preparing'`)
   - Tool is being initialized
   - Arguments are being validated
   - Resources are being allocated

2. **Tool Execution** (`status: 'executing'`)
   - Tool is actively running
   - Processing is underway
   - Results are being generated

3. **Tool Completion** (`status: 'complete'`)
   - Tool has finished execution
   - Results are available
   - Resources are released

### Event Sources

The hook subscribes to SessionManager events:

```typescript
// Events monitored by the hook
- 'tool-notification'         // New tool status update
- 'tool-notification-removed' // Tool notification removal
- 'tool-call-complete'        // Tool execution complete with results
```

### Tool Catalog Integration

Tools are identified by their string names as registered in the agent's tool catalog. Common tool types include:

- **Information Retrieval:** `search_web`, `query_database`, `fetch_data`
- **Data Processing:** `analyze_data`, `transform_json`, `calculate_metrics`
- **Content Generation:** `generate_text`, `create_image`, `format_document`
- **System Operations:** `send_email`, `schedule_task`, `update_record`

## StrictMode Compatibility

The `useToolNotifications` hook is fully compatible with React StrictMode:

1. **Cleanup Management:** All timers and event listeners properly cleaned up
2. **Idempotent Operations:** Multiple mount/unmount cycles handled correctly
3. **State Consistency:** Notifications remain consistent across re-renders
4. **Timer Safety:** Auto-removal timers properly managed and cleared

```tsx
// Safe to use in StrictMode
import { StrictMode } from 'react';
import { AgentCProvider } from '@agentc/realtime-react';

function App() {
  return (
    <StrictMode>
      <AgentCProvider>
        <ToolNotificationComponent />
      </AgentCProvider>
    </StrictMode>
  );
}
```

## Best Practices and Common Patterns

### 1. Notification Filtering

```tsx
function FilteredToolNotifications() {
  const { notifications } = useToolNotifications();
  
  // Filter by status
  const activeTools = notifications.filter(
    n => n.status === 'preparing' || n.status === 'executing'
  );
  
  // Filter by tool name pattern
  const searchTools = notifications.filter(
    n => n.toolName.includes('search')
  );
  
  // Filter by time window
  const recentTools = notifications.filter(
    n => Date.now() - n.timestamp.getTime() < 60000 // Last minute
  );
  
  return (
    <div>
      <h3>Active: {activeTools.length}</h3>
      <h3>Search Tools: {searchTools.length}</h3>
      <h3>Recent: {recentTools.length}</h3>
    </div>
  );
}
```

### 2. Tool Performance Monitoring

```tsx
function ToolPerformanceMonitor() {
  const { completedToolCalls } = useToolNotifications();
  const [metrics, setMetrics] = useState<Map<string, {
    avgDuration: number;
    successRate: number;
    lastError: string | null;
  }>>(new Map());
  
  useEffect(() => {
    // Calculate performance metrics
    const toolMetrics = new Map();
    
    completedToolCalls.forEach(call => {
      const existing = toolMetrics.get(call.toolName) || {
        totalDuration: 0,
        count: 0,
        errors: 0
      };
      
      // Check if result indicates error
      const isError = call.result.toLowerCase().includes('error');
      
      toolMetrics.set(call.toolName, {
        totalDuration: existing.totalDuration + 100, // Estimate
        count: existing.count + 1,
        errors: existing.errors + (isError ? 1 : 0)
      });
    });
    
    // Convert to display metrics
    const displayMetrics = new Map();
    toolMetrics.forEach((stats, toolName) => {
      displayMetrics.set(toolName, {
        avgDuration: Math.round(stats.totalDuration / stats.count),
        successRate: ((stats.count - stats.errors) / stats.count) * 100,
        lastError: null // Would track actual errors
      });
    });
    
    setMetrics(displayMetrics);
  }, [completedToolCalls]);
  
  return (
    <div className="performance-metrics">
      {Array.from(metrics.entries()).map(([tool, stats]) => (
        <div key={tool} className="metric-card">
          <h4>{tool}</h4>
          <dl>
            <dt>Avg Duration:</dt>
            <dd>{stats.avgDuration}ms</dd>
            <dt>Success Rate:</dt>
            <dd>{stats.successRate.toFixed(1)}%</dd>
          </dl>
        </div>
      ))}
    </div>
  );
}
```

### 3. Tool Queue Visualization

```tsx
function ToolQueueVisualizer() {
  const { 
    notifications, 
    activeToolCount,
    hasActiveTools 
  } = useToolNotifications({
    maxNotifications: 100,
    autoRemoveCompleted: true,
    autoRemoveDelay: 2000
  });
  
  const preparingTools = notifications.filter(n => n.status === 'preparing');
  const executingTools = notifications.filter(n => n.status === 'executing');
  const completedTools = notifications.filter(n => n.status === 'complete');
  
  return (
    <div className="queue-visualizer">
      <div className="queue-stats">
        <div className="stat">
          <span className="label">Queue Length</span>
          <span className="value">{preparingTools.length}</span>
        </div>
        <div className="stat">
          <span className="label">Processing</span>
          <span className="value">{executingTools.length}</span>
        </div>
        <div className="stat">
          <span className="label">Completed</span>
          <span className="value">{completedTools.length}</span>
        </div>
      </div>
      
      <div className="queue-visualization">
        <div className="queue preparing">
          <h4>Preparing</h4>
          {preparingTools.map(t => (
            <div key={t.id} className="queue-item">
              {t.toolName}
            </div>
          ))}
        </div>
        
        <div className="queue executing">
          <h4>Executing</h4>
          {executingTools.map(t => (
            <div key={t.id} className="queue-item">
              {t.toolName}
            </div>
          ))}
        </div>
        
        <div className="queue complete">
          <h4>Recently Completed</h4>
          {completedTools.map(t => (
            <div key={t.id} className="queue-item">
              {t.toolName}
            </div>
          ))}
        </div>
      </div>
    </div>
  );
}
```

### 4. Tool Error Handling

```tsx
function ToolErrorMonitor() {
  const { completedToolCalls } = useToolNotifications();
  const [errors, setErrors] = useState<{
    toolName: string;
    error: string;
    timestamp: Date;
  }[]>([]);
  
  useEffect(() => {
    // Detect and track tool errors
    const toolErrors = completedToolCalls
      .filter(call => {
        // Check for error indicators in result
        return call.result.includes('error') || 
               call.result.includes('failed') ||
               call.result.includes('exception');
      })
      .map(call => ({
        toolName: call.toolName,
        error: call.result,
        timestamp: call.timestamp
      }));
    
    setErrors(toolErrors);
  }, [completedToolCalls]);
  
  if (errors.length === 0) {
    return <div>No tool errors detected</div>;
  }
  
  return (
    <div className="error-monitor">
      <h3>Tool Errors ({errors.length})</h3>
      {errors.map((error, idx) => (
        <div key={idx} className="error-item">
          <div className="error-header">
            <strong>{error.toolName}</strong>
            <time>{error.timestamp.toLocaleTimeString()}</time>
          </div>
          <pre className="error-message">{error.error}</pre>
        </div>
      ))}
    </div>
  );
}
```

### 5. Tool Activity Logger

```tsx
function ToolActivityLogger() {
  const { 
    notifications, 
    completedToolCalls,
    clearNotifications 
  } = useToolNotifications({
    maxNotifications: 1000 // Keep extensive history
  });
  
  const exportLog = () => {
    const log = {
      timestamp: new Date().toISOString(),
      activeTools: notifications.filter(n => n.status !== 'complete'),
      completedTools: completedToolCalls,
      summary: {
        total: completedToolCalls.length,
        tools: Array.from(new Set(completedToolCalls.map(c => c.toolName)))
      }
    };
    
    // Download as JSON
    const blob = new Blob([JSON.stringify(log, null, 2)], {
      type: 'application/json'
    });
    const url = URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = `tool-log-${Date.now()}.json`;
    a.click();
    URL.revokeObjectURL(url);
  };
  
  return (
    <div className="activity-logger">
      <div className="logger-controls">
        <button onClick={exportLog}>Export Log</button>
        <button onClick={clearNotifications}>Clear Log</button>
      </div>
      
      <div className="log-summary">
        <p>Total Executions: {completedToolCalls.length}</p>
        <p>Active Tools: {notifications.filter(n => n.status !== 'complete').length}</p>
        <p>Unique Tools: {new Set(completedToolCalls.map(c => c.toolName)).size}</p>
      </div>
    </div>
  );
}
```

## Performance Considerations

### Memory Management

1. **Notification Limits:** Use `maxNotifications` to prevent unbounded growth
2. **Auto-Removal:** Enable `autoRemoveCompleted` for transient notifications
3. **Clear Periodically:** Call `clearNotifications()` to reset state when appropriate

### Rendering Optimization

1. **Memoization:** Use `React.memo` for notification list components
2. **Virtualization:** Consider virtual scrolling for large notification lists
3. **Debouncing:** Debounce rapid notification updates if needed

### Timer Management

1. **Cleanup:** All timers are automatically cleaned up on unmount
2. **Timer Refs:** Internal timer management prevents memory leaks
3. **Configurable Delays:** Adjust `autoRemoveDelay` based on UI requirements

## Troubleshooting

### Common Issues

1. **Notifications not appearing:**
   - Verify SessionManager is emitting tool events
   - Check that client is connected
   - Ensure tool names match expected format

2. **Memory growth:**
   - Set appropriate `maxNotifications` limit
   - Enable `autoRemoveCompleted`
   - Periodically call `clearNotifications()`

3. **Stale notifications:**
   - Check `autoRemoveDelay` setting
   - Verify cleanup in useEffect
   - Monitor for duplicate event subscriptions

4. **Missing tool results:**
   - Confirm `tool-call-complete` events are fired
   - Check result format in completedToolCalls
   - Verify tool IDs match between events

## Related Hooks

- [`useChat`](./useChat.md) - Chat messages often trigger tool calls
- [`useConnection`](./useConnection.md) - Connection required for tool events
- [`useTurnState`](./useTurnState.md) - Tools execute during agent turns
- [`useRealtimeClient`](./useRealtimeClient.md) - Direct access to SessionManager