# Connection Components

## Overview

The `@agentc/realtime-ui` package provides a suite of connection-related components that visualize and control WebSocket connection state. These components integrate seamlessly with the `@agentc/realtime-react` hooks and follow CenSuite design system guidelines.

## Components

### ConnectionIndicator

A visual indicator component that displays the current connection state with optional labels, tooltips, and statistics.

#### Location
`@agentc/realtime-ui/components/connection/ConnectionIndicator`

#### Props Interface

```typescript
export interface ConnectionIndicatorProps extends React.HTMLAttributes<HTMLDivElement> {
  /**
   * Show text label alongside the indicator
   * @default false
   */
  showLabel?: boolean;
  
  /**
   * Show tooltip with connection details on hover
   * @default false
   */
  showTooltip?: boolean;
  
  /**
   * Show connection statistics in tooltip (requires showTooltip)
   * @default false
   */
  showStats?: boolean;
  
  /**
   * Size variant for the indicator dot
   * @default 'default'
   */
  size?: 'small' | 'default' | 'large';
}
```

#### Connection State Visualization

| State | Visual | Color | Animation |
|-------|--------|-------|-----------|
| Connected | Solid dot | Green (`bg-green-500`) | None |
| Connecting | Pulsing dot | Yellow (`bg-yellow-500`) | `animate-pulse` |
| Reconnecting | Pulsing dot | Yellow (`bg-yellow-500`) | `animate-pulse` |
| Disconnected | Solid dot | Gray (`bg-gray-400`) | None |

#### Usage Examples

##### Basic Indicator
```tsx
import { ConnectionIndicator } from '@agentc/realtime-ui';

function StatusBar() {
  return (
    <div className="flex items-center gap-2">
      <ConnectionIndicator />
      <span>Real-time Status</span>
    </div>
  );
}
```

##### With Label
```tsx
<ConnectionIndicator showLabel={true} />
// Displays: [●] Connected
```

##### With Tooltip and Statistics
```tsx
<ConnectionIndicator 
  showTooltip={true}
  showStats={true}
/>
// Hover shows:
// Connected
// Latency: 25ms
// Messages: 100 received, 50 sent
// Data: 10KB received
```

##### Different Sizes
```tsx
// Small indicator for tight spaces
<ConnectionIndicator size="small" />

// Default size for standard UI
<ConnectionIndicator size="default" />

// Large indicator for prominent display
<ConnectionIndicator size="large" />
```

##### Custom Styling
```tsx
<ConnectionIndicator 
  className="fixed top-4 right-4"
  showLabel={true}
  size="large"
/>
```

#### CenSuite Design System Compliance

- **Color System**: Uses semantic HSL-based design tokens
  - Success state: Green for connected
  - Warning state: Yellow for connecting/reconnecting
  - Neutral state: Gray for disconnected
  - Error state: Shown in tooltip with `text-destructive`
  
- **Typography**: Label uses `text-sm text-muted-foreground`
- **Spacing**: Follows 4px base unit system with `gap-2` between indicator and label
- **Animation**: Smooth transitions with `transition-all duration-200`
- **Accessibility**: 
  - `role="status"` for screen readers
  - `aria-label` with current connection state
  - `aria-live="polite"` for state updates

---

### ConnectionButton

An interactive button component that manages WebSocket connection/disconnection with visual feedback.

#### Location
`@agentc/realtime-ui/components/controls/ConnectionButton`

#### Props Interface

```typescript
export interface ConnectionButtonProps
  extends React.ButtonHTMLAttributes<HTMLButtonElement>,
    VariantProps<typeof connectionButtonVariants> {
  /**
   * Show status indicator dot
   * @default true
   */
  showStatus?: boolean;
  
  /**
   * Position of status indicator
   * @default 'left'
   */
  statusPosition?: 'left' | 'right';
  
  /**
   * Button visual variant
   */
  variant?: 'default' | 'outline' | 'secondary' | 'ghost' | 'destructive';
  
  /**
   * Button size
   */
  size?: 'default' | 'sm' | 'lg' | 'icon';
  
  /**
   * Internal state (managed automatically)
   */
  state?: 'idle' | 'loading' | 'error';
}
```

#### Connection State Behavior

| Connection State | Button Text | Variant | Visual Indicators |
|-----------------|-------------|---------|-------------------|
| Disconnected | "Connect" | `outline` | Gray dot |
| Connecting | "Connecting..." | Previous variant | Spinner + yellow pulsing dot |
| Connected | "Disconnect" | `secondary` | Green dot |
| Error | "Connect" | `destructive` | Red border |

#### Usage Examples

##### Basic Connection Button
```tsx
import { ConnectionButton } from '@agentc/realtime-ui';

function Header() {
  return (
    <header className="flex items-center justify-between p-4">
      <h1>Chat Application</h1>
      <ConnectionButton />
    </header>
  );
}
```

##### Without Status Indicator
```tsx
<ConnectionButton showStatus={false} />
```

##### Status on Right Side
```tsx
<ConnectionButton statusPosition="right" />
```

##### Different Sizes
```tsx
// Small button for compact UI
<ConnectionButton size="sm" />

// Default size
<ConnectionButton size="default" />

// Large button for emphasis
<ConnectionButton size="lg" />

// Icon-only button
<ConnectionButton size="icon" showStatus={false}>
  <WifiIcon />
</ConnectionButton>
```

##### Custom Variants
```tsx
// Ghost variant for subtle appearance
<ConnectionButton variant="ghost" />

// Destructive variant (auto-applied on error)
<ConnectionButton variant="destructive" />
```

##### With Custom Click Handler
```tsx
function CustomConnection() {
  const handleConnectionChange = async () => {
    // Custom logic before connection
    console.log('Connection state changing...');
  };

  return (
    <ConnectionButton 
      onClick={handleConnectionChange}
      className="custom-connection-btn"
    />
  );
}
```

#### Error Handling and Recovery

The button automatically handles connection errors:
- Changes to `destructive` variant on error
- Shows error state with red border
- Maintains clickable state for retry attempts
- Console logs errors for debugging

```tsx
// Error recovery workflow
// 1. Connection fails -> Button shows destructive variant
// 2. User clicks button -> Retry connection
// 3. Success -> Returns to normal state
```

#### CenSuite Design System Compliance

- **Button Patterns**: Follows CenSuite button specifications
  - `h-10 px-4 py-2` for default size
  - `h-9 px-3` for small
  - `h-11 px-8` for large
  
- **Focus States**: `focus-visible:ring-2 focus-visible:ring-ring focus-visible:ring-offset-2`
- **Disabled State**: `disabled:pointer-events-none disabled:opacity-50`
- **Loading State**: Shows spinner with `Loader2` icon
- **Transitions**: Smooth color transitions with `transition-colors`

---

### ConnectionStatus

A comprehensive status card component that displays detailed connection information with statistics.

#### Location
`@agentc/realtime-ui/components/controls/ConnectionStatus`

#### Props Interface

```typescript
export interface ConnectionStatusProps {
  /**
   * Additional CSS classes
   */
  className?: string;
  
  /**
   * Show detailed connection statistics
   * @default true
   */
  showDetails?: boolean;
  
  /**
   * Render as compact badge instead of card
   * @default false
   */
  compact?: boolean;
}
```

#### Display Modes

##### Full Card Mode (Default)
Shows comprehensive connection information in a card layout:
- Connection status with animated indicator
- Status icon (Wifi/WifiOff)
- Error messages when applicable
- Session duration when connected
- Retry/Disconnect action buttons
- Connection attempt statistics

##### Compact Badge Mode
Minimal badge display for space-constrained layouts:
- Status dot indicator
- Status text only
- No statistics or actions

#### Usage Examples

##### Full Status Card
```tsx
import { ConnectionStatus } from '@agentc/realtime-ui';

function ConnectionPanel() {
  return (
    <div className="p-4">
      <h2>Connection Details</h2>
      <ConnectionStatus />
    </div>
  );
}
```

##### Compact Badge
```tsx
<ConnectionStatus compact={true} />
// Renders: [●] Connected (as a badge)
```

##### Without Details
```tsx
<ConnectionStatus showDetails={false} />
// Shows status without statistics
```

##### Custom Styling
```tsx
<ConnectionStatus 
  className="shadow-lg border-2"
  showDetails={true}
/>
```

#### Status Visualization

| State | Indicator Color | Icon | Animation | Additional Info |
|-------|----------------|------|-----------|-----------------|
| Connected | Green | Wifi | Ping animation | Session duration |
| Connecting | Yellow | WifiOff | Pulse | "Connecting..." |
| Reconnecting | Orange | WifiOff | Pulse | Attempt count |
| Disconnected | Gray/Red | WifiOff | None | Error message if applicable |

#### Statistics Display

When `showDetails` is true and statistics are available:
- **Connection Attempts**: Total number of connection attempts
- **Successful Connections**: Number of successful connections
- **Failed Connections**: Number of failed connections
- **Session Duration**: Formatted as "Xh Ym" or "Xm Ys"

#### Error Recovery UI

The component provides built-in error recovery:
```tsx
// On error, shows:
// - Error message
// - "Retry" button with RefreshCw icon

// On successful connection, shows:
// - "Disconnect" button
```

---

### ConnectionIndicator (Simple Variant)

A lightweight inline indicator for headers and navigation bars (exported from ConnectionStatus.tsx).

#### Location
`@agentc/realtime-ui/components/controls/ConnectionStatus` (secondary export)

#### Props Interface

```typescript
interface ConnectionIndicatorProps {
  className?: string;
}
```

#### Usage Examples

##### In Navigation Bar
```tsx
import { ConnectionIndicator } from '@agentc/realtime-ui';

function NavBar() {
  return (
    <nav className="flex items-center justify-between p-4">
      <Logo />
      <ConnectionIndicator />
    </nav>
  );
}
```

##### Custom Positioning
```tsx
<ConnectionIndicator className="absolute top-2 right-2" />
```

This simple variant displays:
- Colored dot indicator
- "Online"/"Offline" text
- Pulse animation for connecting states

---

## Integration with @agentc/realtime-react Hooks

All connection components integrate seamlessly with the `useConnection` hook:

```typescript
const {
  isConnected,        // Boolean connection state
  connectionState,    // Detailed state (ConnectionState enum)
  error,             // Error object if connection failed
  stats,             // Connection statistics
  connect,           // Function to initiate connection
  disconnect,        // Function to close connection
  reconnectAttempt   // Current reconnection attempt number
} = useConnection();
```

### Connection State Enum

```typescript
enum ConnectionState {
  DISCONNECTED = 'DISCONNECTED',
  CONNECTING = 'CONNECTING',
  CONNECTED = 'CONNECTED',
  RECONNECTING = 'RECONNECTING'
}
```

---

## Complete Integration Example

```tsx
import React from 'react';
import {
  ConnectionButton,
  ConnectionStatus,
  ConnectionIndicator
} from '@agentc/realtime-ui';
import { AgentCProvider } from '@agentc/realtime-react';

function ChatApplication() {
  return (
    <AgentCProvider>
      <div className="min-h-screen bg-background">
        {/* Header with simple indicator */}
        <header className="border-b p-4">
          <div className="flex items-center justify-between">
            <h1 className="text-2xl font-bold">Agent C Chat</h1>
            <div className="flex items-center gap-4">
              <ConnectionIndicator />
              <ConnectionButton size="sm" />
            </div>
          </div>
        </header>

        {/* Main content */}
        <div className="container mx-auto p-4">
          <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
            {/* Connection status panel */}
            <div className="md:col-span-1">
              <ConnectionStatus showDetails={true} />
            </div>
            
            {/* Chat area */}
            <div className="md:col-span-2">
              {/* Chat messages */}
            </div>
          </div>
        </div>
      </div>
    </AgentCProvider>
  );
}
```

---

## Error Handling Patterns

### Connection Failure Recovery

```tsx
function ConnectionManager() {
  const { error, connectionState } = useConnection();

  return (
    <div>
      {error && (
        <Alert variant="destructive">
          <AlertTitle>Connection Error</AlertTitle>
          <AlertDescription>{error.message}</AlertDescription>
        </Alert>
      )}
      
      <ConnectionButton />
      {connectionState === 'RECONNECTING' && (
        <p className="text-sm text-muted-foreground">
          Attempting to reconnect...
        </p>
      )}
    </div>
  );
}
```

### Auto-Reconnection Feedback

```tsx
function AutoReconnectStatus() {
  const { connectionState, reconnectAttempt } = useConnection();

  if (connectionState !== 'RECONNECTING') return null;

  return (
    <Card className="p-4 border-yellow-500">
      <div className="flex items-center gap-2">
        <Loader2 className="h-4 w-4 animate-spin" />
        <span>Reconnecting (Attempt {reconnectAttempt})...</span>
      </div>
    </Card>
  );
}
```

---

## Responsive Behavior

All connection components are responsive by default:

### Mobile Optimizations
```tsx
// Use compact mode on mobile
<ConnectionStatus compact={window.innerWidth < 768} />

// Hide labels on small screens
<ConnectionIndicator 
  showLabel={false} 
  className="sm:hidden"
/>

// Smaller button on mobile
<ConnectionButton 
  size="sm" 
  className="md:hidden"
/>
<ConnectionButton 
  size="default" 
  className="hidden md:inline-flex"
/>
```

### Adaptive Layouts
```tsx
function ResponsiveConnectionBar() {
  return (
    <>
      {/* Mobile: Compact indicator */}
      <div className="md:hidden">
        <ConnectionStatus compact={true} />
      </div>
      
      {/* Desktop: Full status card */}
      <div className="hidden md:block">
        <ConnectionStatus showDetails={true} />
      </div>
    </>
  );
}
```

---

## Performance Considerations

### Memoization
All connection components use React.memo and useMemo for optimal performance:
- Status colors are memoized based on connection state
- Status text is memoized to prevent unnecessary recalculations
- Event handlers use useCallback to maintain referential equality

### Polling and Updates
- Connection statistics update at regular intervals
- Visual indicators use CSS animations for smooth transitions
- Components only re-render when relevant connection state changes

---

## Accessibility Features

### ARIA Support
- **role="status"**: All indicators have proper status role
- **aria-label**: Descriptive labels for screen readers
- **aria-live="polite"**: Announces connection state changes
- **aria-hidden**: Decorative elements hidden from screen readers

### Keyboard Navigation
- Connection buttons are fully keyboard accessible
- Tab navigation follows logical order
- Enter/Space keys activate buttons
- Focus states clearly visible with ring indicators

### Screen Reader Announcements
```tsx
// Announced: "Connection status: Connecting"
// Then: "Connection status: Connected"
<ConnectionIndicator showLabel={true} />
```

---

## Testing

All connection components include comprehensive test coverage:

```tsx
import { render, screen } from '@testing-library/react';
import { ConnectionIndicator } from '@agentc/realtime-ui';

test('shows correct connection state', () => {
  render(<ConnectionIndicator showLabel={true} />);
  expect(screen.getByText('Disconnected')).toBeInTheDocument();
});

test('updates on connection state change', async () => {
  const { rerender } = render(<ConnectionIndicator />);
  // Mock connection state change
  updateMockState('connection', { isConnected: true });
  rerender(<ConnectionIndicator />);
  expect(screen.getByRole('status')).toHaveAttribute(
    'aria-label', 
    'Connection status: Connected'
  );
});
```

---

## Best Practices

### Do's
- ✅ Use ConnectionIndicator for subtle status display
- ✅ Use ConnectionButton for primary connection control
- ✅ Use ConnectionStatus for detailed connection information
- ✅ Combine components for comprehensive connection UI
- ✅ Provide error recovery options
- ✅ Show reconnection feedback

### Don'ts
- ❌ Don't hide connection errors from users
- ❌ Don't use multiple ConnectionButtons in the same view
- ❌ Don't override semantic colors
- ❌ Don't disable accessibility features
- ❌ Don't ignore responsive design considerations

---

## Migration Guide

If migrating from custom connection UI:

1. **Replace custom indicators** with ConnectionIndicator
2. **Replace connect/disconnect buttons** with ConnectionButton
3. **Replace status panels** with ConnectionStatus
4. **Ensure AgentCProvider** wraps your application
5. **Remove redundant** connection state management
6. **Update tests** to use provided mocks

---

## Related Components

- [`AudioControls`](./audio-components.md) - Audio control components
- [`ChatMessagesView`](./chat-components.md) - Chat message display
- [`AgentSelector`](./control-components.md) - Agent selection UI

---

## Support

For issues or questions about connection components:
- Check the [SDK documentation](../core/connection.md)
- Review [example implementations](../../examples/connection-ui.tsx)
- Consult the [testing guide](../../testing/connection-components.md)