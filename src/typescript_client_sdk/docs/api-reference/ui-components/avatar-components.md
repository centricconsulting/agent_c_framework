# Avatar Components API Reference

The `@agentc/realtime-ui` package provides components for integrating HeyGen streaming avatars into your Agent C Realtime applications. These components handle avatar display, state management, and integration with the SDK's avatar management system.

## Table of Contents

- [AvatarDisplayView](#avatardisplayview)
- [Avatar Selection (OutputSelector)](#avatar-selection-outputselector)
- [Base Avatar Components](#base-avatar-components)
- [Usage Examples](#usage-examples)
- [HeyGen Integration](#heygen-integration)
- [State Management](#state-management)
- [Design System Compliance](#design-system-compliance)

---

## AvatarDisplayView

The primary component for displaying HeyGen avatar video streams with built-in loading, error, and connection states.

### Component Overview

```typescript
import { AvatarDisplayView } from '@agentc/realtime-ui'
```

### Props Interface

```typescript
export interface AvatarDisplayViewProps {
  /**
   * Additional CSS classes to apply to the root element
   * @default undefined
   */
  className?: string
}
```

### Features

- **Automatic Stream Management**: Handles video stream setup and cleanup
- **Connection Quality Indicator**: Shows real-time connection status
- **Loading States**: Displays initialization progress
- **Error Handling**: Shows error messages when avatar fails to load
- **Responsive Design**: Video container adapts to different screen sizes
- **Accessibility**: Proper ARIA labels and semantic HTML

### Usage

```tsx
// Basic usage
<AvatarDisplayView />

// With custom styling
<AvatarDisplayView className="rounded-2xl shadow-lg" />

// With ref forwarding
const avatarRef = useRef<HTMLDivElement>(null)
<AvatarDisplayView ref={avatarRef} />
```

### State Management

The component automatically manages several states:

#### Loading State
Shows a spinner with "Initializing avatar..." message when:
- `isLoading` is true from `useAvatar` hook
- Avatar is active but stream is not yet ready

#### Connected State
Displays a green connection indicator when:
- Avatar session is active
- Stream has been successfully initialized
- Shows "Connected" badge in top-left corner

#### Error State
Shows error message when:
- Avatar fails to initialize
- Connection is lost
- HeyGen API returns an error

#### No Avatar State
Default state showing video icon when:
- No avatar is selected
- Avatar mode is not active
- Displays: "Avatar mode selected" with instructions

### Video Element Configuration

The component configures the video element with:

```html
<video
  autoPlay
  playsInline
  muted={false}
  style={{ objectFit: 'contain' }}
  aria-label="Avatar video stream"
/>
```

- **autoPlay**: Starts playing automatically when stream is ready
- **playsInline**: Prevents fullscreen on mobile devices
- **muted={false}**: Audio enabled for avatar speech
- **objectFit: contain**: Maintains aspect ratio within container

### Responsive Behavior

The video container uses responsive classes:
- Maximum width: 900px
- Aspect ratio: 16:9 (`aspect-video`)
- Full width on smaller screens
- Object-fit contain for proper scaling

---

## Avatar Selection (OutputSelector)

The OutputSelector component includes avatar selection functionality, allowing users to choose between text-only, voice, and avatar output modes.

### Avatar Tab Features

```tsx
<OutputSelector
  className="w-[250px]"
  showIcon={true}
  ariaLabel="Select output mode"
/>
```

### Avatar Selection Interface

The OutputSelector provides:

1. **Avatar Tab**: Lists all available avatars
2. **Search Functionality**: Filter avatars by name or ID
3. **Selection State**: Shows checkmark for active avatar
4. **Avatar Count**: Displays total available avatars
5. **Coming Soon Notice**: Indicates feature is in development

### Avatar Data Structure

```typescript
interface Avatar {
  avatar_id: string        // Unique identifier
  pose_name: string       // Human-readable pose name
  is_public: boolean      // Whether avatar is publicly available
  // Additional HeyGen metadata
}
```

### Filtering Logic

The component filters avatars to show only public ones:

```typescript
const activeAvatars = data.avatars.filter(
  avatar => avatar.is_public
)
```

---

## Base Avatar Components

The UI package also includes base avatar components from Radix UI for user profile avatars (not HeyGen avatars):

### Avatar

Basic avatar container component:

```tsx
import { Avatar, AvatarImage, AvatarFallback } from '@agentc/realtime-ui'

<Avatar>
  <AvatarImage src="/user-photo.jpg" alt="User" />
  <AvatarFallback>UN</AvatarFallback>
</Avatar>
```

### Props

```typescript
// Avatar - Container component
interface AvatarProps extends React.ComponentPropsWithoutRef<'div'> {
  className?: string
}

// AvatarImage - Image component
interface AvatarImageProps extends React.ComponentPropsWithoutRef<'img'> {
  className?: string
}

// AvatarFallback - Fallback when image fails
interface AvatarFallbackProps extends React.ComponentPropsWithoutRef<'div'> {
  className?: string
}
```

---

## Usage Examples

### Basic Avatar Setup

```tsx
import { AvatarDisplayView } from '@agentc/realtime-ui'
import { useAvatar } from '@agentc/realtime-react'

function ChatWithAvatar() {
  const { isAvatarActive, error } = useAvatar()
  
  return (
    <div className="flex flex-col h-screen">
      {/* Avatar Display Area */}
      <div className="flex-1">
        <AvatarDisplayView />
      </div>
      
      {/* Chat Interface */}
      <div className="p-4">
        {isAvatarActive ? (
          <p>Avatar is ready for interaction</p>
        ) : (
          <p>Select an avatar to begin</p>
        )}
        {error && <p className="text-destructive">{error}</p>}
      </div>
    </div>
  )
}
```

### Avatar Selection with OutputSelector

```tsx
import { OutputSelector, AvatarDisplayView } from '@agentc/realtime-ui'

function AvatarChat() {
  return (
    <div className="space-y-4">
      {/* Mode Selection */}
      <div className="flex justify-center">
        <OutputSelector 
          className="w-[300px]"
          showIcon={true}
        />
      </div>
      
      {/* Avatar Display */}
      <AvatarDisplayView className="rounded-xl shadow-lg" />
    </div>
  )
}
```

### Custom Avatar Implementation

```tsx
import { useAvatar } from '@agentc/realtime-react'

function CustomAvatarControls() {
  const {
    availableAvatars,
    avatarSession,
    setAvatar,
    clearAvatar,
    isLoading,
    error
  } = useAvatar()
  
  const handleSelectAvatar = async (avatar: Avatar) => {
    try {
      // Generate session ID (implement your logic)
      const sessionId = generateSessionId()
      await setAvatar(avatar.avatar_id, sessionId)
    } catch (err) {
      console.error('Failed to set avatar:', err)
    }
  }
  
  return (
    <div className="space-y-4">
      {/* Avatar Selection Grid */}
      <div className="grid grid-cols-2 gap-4">
        {availableAvatars.map(avatar => (
          <button
            key={avatar.avatar_id}
            onClick={() => handleSelectAvatar(avatar)}
            disabled={isLoading}
            className={cn(
              "p-4 rounded-lg border-2 transition-colors",
              avatarSession?.avatarId === avatar.avatar_id
                ? "border-primary bg-primary/10"
                : "border-border hover:border-primary/50"
            )}
          >
            <h3 className="font-medium">{avatar.pose_name}</h3>
            <p className="text-sm text-muted-foreground">
              ID: {avatar.avatar_id}
            </p>
          </button>
        ))}
      </div>
      
      {/* Clear Avatar */}
      {avatarSession && (
        <button
          onClick={clearAvatar}
          disabled={isLoading}
          className="w-full p-2 bg-destructive text-destructive-foreground rounded-md"
        >
          Clear Avatar
        </button>
      )}
      
      {/* Error Display */}
      {error && (
        <Alert variant="destructive">
          <AlertDescription>{error}</AlertDescription>
        </Alert>
      )}
    </div>
  )
}
```

### Error Handling Example

```tsx
function AvatarWithErrorHandling() {
  const { error: avatarError, isAvatarActive } = useAvatar()
  const [localError, setLocalError] = useState<string | null>(null)
  
  // Handle avatar initialization errors
  useEffect(() => {
    if (avatarError) {
      // Log to monitoring service
      console.error('Avatar error:', avatarError)
      
      // Show user-friendly message
      if (avatarError.includes('token')) {
        setLocalError('Authentication failed. Please reconnect.')
      } else if (avatarError.includes('network')) {
        setLocalError('Connection issue. Check your internet.')
      } else {
        setLocalError('Avatar unavailable. Try again later.')
      }
    }
  }, [avatarError])
  
  return (
    <div>
      <AvatarDisplayView />
      
      {localError && (
        <Alert variant="destructive" className="mt-4">
          <AlertCircle className="h-4 w-4" />
          <AlertTitle>Avatar Error</AlertTitle>
          <AlertDescription>{localError}</AlertDescription>
        </Alert>
      )}
    </div>
  )
}
```

---

## HeyGen Integration

### Integration Points

The avatar components integrate with HeyGen through:

1. **Avatar Manager**: Core SDK manager handling HeyGen sessions
2. **Access Token**: HeyGen authentication token from server
3. **Session Management**: Session ID and avatar ID tracking
4. **Stream Handling**: WebRTC video stream management (future)

### Current Implementation Status

```typescript
// Placeholder for HeyGen SDK integration
React.useEffect(() => {
  if (isAvatarActive && avatarSession) {
    // HeyGen SDK initialization will go here
    // const heygenClient = new HeyGenStreamingClient({
    //   token: heygenAccessToken,
    //   sessionId: avatarSession.sessionId
    // })
    
    // Simulate stream ready for now
    const timer = setTimeout(() => {
      setStreamReady(true)
    }, 1000)
    
    return () => clearTimeout(timer)
  }
}, [isAvatarActive, avatarSession])
```

### Future HeyGen Integration

When HeyGen SDK is integrated:

```typescript
// Expected integration pattern
const initializeHeyGenStream = async () => {
  const videoElement = videoRef.current
  if (!videoElement || !heygenAccessToken) return
  
  // Initialize HeyGen client
  const heygenClient = new HeyGenStreamingClient({
    token: heygenAccessToken
  })
  
  // Start streaming session
  const stream = await heygenClient.startSession({
    avatarId: avatarSession.avatarId,
    sessionId: avatarSession.sessionId,
    quality: 'high'
  })
  
  // Attach stream to video element
  videoElement.srcObject = stream
}
```

---

## State Management

### useAvatar Hook Integration

The components integrate with the `useAvatar` hook from `@agentc/realtime-react`:

```typescript
export interface UseAvatarReturn {
  availableAvatars: Avatar[]         // List of available avatars
  avatarSession: {                    // Current session info
    sessionId: string | null
    avatarId: string | null
  } | null
  isAvatarActive: boolean            // Whether avatar is active
  heygenAccessToken: string | null   // HeyGen auth token
  setAvatar: (avatarId: string, sessionId: string) => Promise<void>
  clearAvatar: () => Promise<void>
  getAvatarById: (avatarId: string) => Avatar | undefined
  isAvatarAvailable: (avatarId: string) => boolean
  isLoading: boolean                 // Loading state
  error: string | null              // Error message
  isAvatarEnabled: boolean          // Feature availability
}
```

### State Flow

1. **Initialization**: Component checks for avatar availability
2. **Selection**: User selects avatar through OutputSelector
3. **Session Start**: SDK creates HeyGen session
4. **Stream Setup**: Video element receives WebRTC stream
5. **Active State**: Avatar responds to agent messages
6. **Cleanup**: Proper disposal of stream and session

### Event Handling

The components listen to avatar events:

```typescript
avatarManager.on('avatar-state-changed', handleStateChange)
avatarManager.on('avatar-session-started', handleSessionStarted)
avatarManager.on('avatar-session-ended', handleSessionEnded)
```

---

## Design System Compliance

### CenSuite Design Principles

The avatar components follow CenSuite's five principles:

1. **Clarity**: Clear visual states (loading, connected, error)
2. **Consistency**: Uniform styling with other UI components
3. **Efficiency**: Minimal clicks to select and activate avatars
4. **Scalability**: Supports multiple avatar options
5. **Accessibility**: Full ARIA support and keyboard navigation

### Color System Usage

```css
/* Component uses semantic color tokens */
--background: Container backgrounds
--primary: Connected indicator, selected state
--destructive: Error states
--muted: Placeholder text and borders
--accent: Hover states
```

### Spacing and Layout

- Padding: Uses standard scale (p-2, p-4, p-6)
- Gaps: Consistent spacing between elements
- Container: Max width of 900px for optimal viewing
- Aspect ratio: 16:9 for video display

### Typography

- Headings: font-medium for labels
- Body: text-sm for descriptions  
- Captions: text-xs for metadata
- Error text: text-destructive for warnings

### Visual Feedback

1. **Loading States**:
   - Spinner animation with Loader2 icon
   - Descriptive text: "Initializing avatar..."
   - Semi-transparent overlay

2. **Connection Status**:
   - Green dot for connected
   - Position: top-left with backdrop blur
   - Text label for clarity

3. **Error States**:
   - Red text color (destructive)
   - Clear error messages
   - Centered in container

### Accessibility Features

1. **ARIA Labels**:
   ```html
   aria-label="Avatar video stream"
   aria-hidden="true" (for decorative elements)
   ```

2. **Semantic HTML**:
   - Video element with proper attributes
   - Button roles for interactive elements
   - Status regions for updates

3. **Keyboard Navigation**:
   - Tab navigation through controls
   - Escape key closes selection popover
   - Enter/Space activates buttons

4. **Screen Reader Support**:
   - Descriptive text for all states
   - Live regions for status updates
   - Hidden text for visual-only elements

### Responsive Design

- Mobile: Full width with padding
- Tablet: Constrained width with margins
- Desktop: Maximum 900px width
- Maintains aspect ratio across breakpoints

---

## Testing Considerations

### Unit Testing

The components include comprehensive test coverage:

- Rendering with different props
- State transitions (loading → connected → error)
- User interactions (selection, clearing)
- Cleanup on unmount
- Timer management
- Accessibility compliance

### Test Example

```typescript
describe('AvatarDisplayView', () => {
  it('should show loading state when initializing', () => {
    mockUseAvatar.mockReturnValue({
      isLoading: true,
      // ... other values
    })
    
    render(<AvatarDisplayView />)
    expect(screen.getByText('Initializing avatar...')).toBeInTheDocument()
  })
  
  it('should display connection indicator when active', async () => {
    mockUseAvatar.mockReturnValue({
      isAvatarActive: true,
      avatarSession: { sessionId: 'test', avatarId: 'avatar-1' }
    })
    
    render(<AvatarDisplayView />)
    
    await act(async () => {
      vi.advanceTimersByTime(1000)
    })
    
    expect(screen.getByText('Connected')).toBeInTheDocument()
  })
})
```

### Integration Testing

Test with real SDK context:

```typescript
describe('Avatar Integration', () => {
  it('should handle avatar selection through OutputSelector', async () => {
    const { user } = render(
      <AgentCProvider config={testConfig}>
        <OutputSelector />
        <AvatarDisplayView />
      </AgentCProvider>
    )
    
    // Open selector
    await user.click(screen.getByRole('combobox'))
    
    // Select avatar tab
    await user.click(screen.getByText('Avatars'))
    
    // Choose avatar
    await user.click(screen.getByText('Professional'))
    
    // Verify display updates
    await waitFor(() => {
      expect(screen.getByText('Initializing avatar...')).toBeInTheDocument()
    })
  })
})
```

---

## Best Practices

### Performance

1. **Stream Management**: Clean up video streams on unmount
2. **Timer Cleanup**: Clear timeouts when component updates
3. **Memoization**: Use React.memo for expensive computations
4. **Lazy Loading**: Load HeyGen SDK only when needed

### Error Recovery

1. **Graceful Degradation**: Fall back to voice/text mode
2. **Retry Logic**: Implement exponential backoff for failures
3. **User Feedback**: Clear error messages with recovery options
4. **Logging**: Track errors for debugging

### Security

1. **Token Management**: Never expose HeyGen tokens in client
2. **Session Validation**: Verify session IDs from server
3. **Content Security**: Validate video streams
4. **CORS Configuration**: Proper cross-origin setup

### User Experience

1. **Progressive Disclosure**: Show avatars only when available
2. **Loading Feedback**: Clear initialization progress
3. **Connection Status**: Real-time quality indicators
4. **Fallback Options**: Alternative output modes ready

---

## Migration Guide

### From Custom Implementation

If migrating from a custom avatar implementation:

1. Replace custom video elements with `AvatarDisplayView`
2. Use `useAvatar` hook instead of direct API calls
3. Integrate `OutputSelector` for mode switching
4. Update error handling to use component states

### Version Compatibility

- Requires `@agentc/realtime-core` >= 0.1.0
- Requires `@agentc/realtime-react` >= 0.1.0
- HeyGen SDK integration pending (future release)

---

## API Reference Summary

### Exports

```typescript
// Components
export { AvatarDisplayView } from './components/avatar'
export type { AvatarDisplayViewProps } from './components/avatar'

// Base avatar components (Radix UI)
export { Avatar, AvatarImage, AvatarFallback } from './components/ui/avatar'

// Related components with avatar support
export { OutputSelector } from './components/controls'
export type { OutputSelectorProps } from './components/controls'
```

### Required Context

All avatar components require the AgentCProvider:

```tsx
import { AgentCProvider } from '@agentc/realtime-react'

<AgentCProvider config={realtimeConfig}>
  <AvatarDisplayView />
</AgentCProvider>
```

### Configuration

Avatar configuration is managed through the SDK:

```typescript
const config = {
  // ... other config
  features: {
    enableAvatars: true,
    avatarProvider: 'heygen'
  }
}
```