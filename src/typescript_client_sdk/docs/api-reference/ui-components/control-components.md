# Control Components API Reference

The control components in `@agentc/realtime-ui` provide essential UI elements for managing realtime sessions, agent configuration, and user settings. These components follow CenSuite design system guidelines and integrate seamlessly with the SDK hooks.

## Table of Contents

- [AgentSelector](#agentselector)
- [AudioControls](#audiocontrols)
- [ConnectionButton](#connectionbutton)
- [ConnectionStatus](#connectionstatus)
- [OutputSelector](#outputselector)
- [ThemeSwitcher](#themeswitcher)
- [CopyLinkButton](#copylinkbutton)
- [Control Panel Patterns](#control-panel-patterns)
- [Settings Management](#settings-management)
- [Form Validation Patterns](#form-validation-patterns)
- [Responsive Layouts](#responsive-layouts)

## AgentSelector

A searchable agent selector with rich metadata display, using React Portal for overlay rendering.

### Purpose

Enables users to browse and select from available AI agents, with search functionality and rich metadata display including agent descriptions and equipped tools.

### Props Interface

```typescript
interface AgentSelectorProps {
  className?: string                // Additional CSS classes
  disabled?: boolean                 // Disable the selector (default: false)
  showIcon?: boolean                // Show bot icon in button (default: true)
  placeholder?: string              // Placeholder when no agent selected
  searchPlaceholder?: string        // Placeholder for search input
}
```

### Usage Examples

```tsx
import { AgentSelector } from '@agentc/realtime-ui'

// Basic usage
<AgentSelector />

// Custom configuration
<AgentSelector 
  className="w-[250px]"
  placeholder="Choose an AI assistant"
  searchPlaceholder="Find agents..."
/>

// Without icon for compact layouts
<AgentSelector 
  showIcon={false}
  className="min-w-[150px]"
/>

// Disabled state (auto-disabled when disconnected)
<AgentSelector disabled={true} />
```

### CenSuite Design Compliance

- **Color System**: Uses semantic color tokens (primary, accent, muted-foreground)
- **Typography**: Follows hierarchical text sizing (text-sm for labels, text-xs for descriptions)
- **Spacing**: Consistent padding scale (p-3 for items, gap-2 for elements)
- **Accessibility**: Full ARIA labels, keyboard navigation, screen reader announcements
- **Visual Feedback**: Loading states with spinner, hover states, selection indicators

### SDK Integration

```tsx
// Hooks used internally
import { 
  useAgentCData,        // Access agent list
  useConnection,        // Check connection state
  useRealtimeClientSafe // Get client instance
} from '@agentc/realtime-react'

// The component automatically:
// - Loads agents from useAgentCData hook
// - Disables when disconnected
// - Calls client.setAgent() on selection
// - Shows loading state during agent switching
```

### State Management

The component manages several internal states:

```tsx
// Internal state management
const [isChanging, setIsChanging] = useState(false)  // Loading during switch
const [open, setOpen] = useState(false)              // Popover visibility
const [searchValue, setSearchValue] = useState('')   // Search filter

// Derived from SDK
const selectedAgent = data.currentAgentConfig?.key   // Current selection
const isDisabled = !client || !isConnected          // Auto-disable logic
```

## AudioControls

Manages audio recording state with visual feedback for audio levels and recording status.

### Purpose

Provides intuitive controls for starting/stopping audio recording and managing volume, with real-time audio level visualization.

### Props Interface

```typescript
interface AudioControlsProps {
  size?: 'sm' | 'default' | 'lg'    // Button size variant
  showLabel?: boolean                // Show text labels (default: true)
  showLevelIndicator?: boolean       // Show audio level bar (default: true)
  className?: string                 // Additional CSS classes
}
```

### Usage Examples

```tsx
import { AudioControls } from '@agentc/realtime-ui'

// Full-featured controls
<AudioControls 
  size="default"
  showLabel={true}
  showLevelIndicator={true}
/>

// Compact version for toolbars
<AudioControls 
  size="sm"
  showLabel={false}
/>

// Large controls for primary actions
<AudioControls 
  size="lg"
  className="shadow-lg"
/>
```

### SDK Integration

```tsx
// Uses useAudio hook for all functionality
const { 
  isRecording,      // Current recording state
  audioLevel,       // Real-time audio level (0-1)
  startRecording,   // Start audio capture
  stopRecording,    // Stop audio capture
  setVolume        // Control output volume
} = useAudio()
```

### Visual States

- **Recording Active**: Red background with active mic icon
- **Recording Inactive**: Ghost variant with muted mic icon
- **Audio Level**: Dynamic progress bar showing real-time input level
- **Volume Control**: Toggle between muted/unmuted states

## ConnectionButton

Manages WebSocket connection state with visual feedback and automatic state detection.

### Purpose

Primary control for establishing and terminating WebSocket connections to the Agent C Realtime API.

### Props Interface

```typescript
interface ConnectionButtonProps extends 
  React.ButtonHTMLAttributes<HTMLButtonElement>,
  VariantProps<typeof connectionButtonVariants> {
  showStatus?: boolean               // Show connection status indicator
  statusPosition?: 'left' | 'right'  // Status dot position
  variant?: 'default' | 'outline' | 'secondary' | 'ghost' | 'destructive'
  size?: 'default' | 'sm' | 'lg' | 'icon'
}
```

### Usage Examples

```tsx
import { ConnectionButton } from '@agentc/realtime-ui'

// Default with status indicator
<ConnectionButton showStatus={true} />

// Compact version
<ConnectionButton 
  size="sm"
  showStatus={false}
/>

// Custom variant based on state
<ConnectionButton 
  variant="outline"
  statusPosition="right"
/>

// Large primary action button
<ConnectionButton 
  size="lg"
  className="w-full"
/>
```

### State Management Patterns

The component automatically adapts its appearance based on connection state:

```tsx
// Automatic variant selection based on state
const currentVariant = useMemo(() => {
  if (error) return 'destructive'      // Red on error
  if (isConnected) return 'secondary'  // Secondary when connected
  return variant || 'outline'          // Default outline
}, [error, isConnected, variant])

// Connection states:
// - DISCONNECTED: "Connect" button
// - CONNECTING: Loading spinner + "Connecting..."
// - CONNECTED: "Disconnect" button
// - ERROR: Destructive variant with error state
```

### Status Indicator

The component includes a visual status indicator:

- 🟢 Green (solid): Connected
- 🟡 Yellow (pulsing): Connecting/Reconnecting
- 🔴 Gray: Disconnected
- 🔴 Red: Error state

## ConnectionStatus

Comprehensive connection status display with statistics and retry functionality.

### Purpose

Provides detailed connection information including session duration, connection attempts, and error messages.

### Props Interface

```typescript
interface ConnectionStatusProps {
  className?: string      // Additional CSS classes
  showDetails?: boolean   // Show connection statistics (default: true)
  compact?: boolean       // Compact badge mode (default: false)
}
```

### Usage Examples

```tsx
import { ConnectionStatus, ConnectionIndicator } from '@agentc/realtime-ui'

// Full status card with details
<ConnectionStatus showDetails={true} />

// Compact badge for headers
<ConnectionStatus compact={true} />

// Simple indicator for minimal UI
<ConnectionIndicator className="ml-auto" />

// Status panel in settings
<div className="space-y-4">
  <h3>Connection Info</h3>
  <ConnectionStatus showDetails={true} />
</div>
```

### Statistics Display

When `showDetails` is enabled, displays:

- Connection attempts count
- Successful connections
- Failed connections
- Session duration (formatted as "2h 15m" or "45s")

### Error Recovery

Includes automatic retry button on connection errors:

```tsx
// Retry button appears on error
{(connectionError || connectionState === 'DISCONNECTED') && (
  <Button onClick={() => connect()}>
    <RefreshCw className="w-3 h-3" />
    Retry
  </Button>
)}
```

## OutputSelector

A comprehensive selector for choosing agent output modes (text, voice, or avatar).

### Purpose

Allows users to control how the agent responds - text only, with synthesized voice, or with streaming avatar (future feature).

### Props Interface

```typescript
interface OutputSelectorProps {
  className?: string         // Additional CSS classes
  disabled?: boolean         // Disable selector (default: false)
  showIcon?: boolean        // Show mode icon (default: true)
  ariaLabel?: string        // Custom ARIA label for accessibility
  showErrorAlerts?: boolean // Show inline error alerts (default: true)
}
```

### Usage Examples

```tsx
import { OutputSelector } from '@agentc/realtime-ui'

// Standard usage
<OutputSelector />

// Custom configuration
<OutputSelector
  className="w-[250px]"
  showIcon={true}
  showErrorAlerts={false}
  ariaLabel="Choose response format"
/>

// Compact without errors
<OutputSelector 
  showErrorAlerts={false}
  className="min-w-[180px]"
/>
```

### Output Modes

The selector supports three modes:

1. **Text Only**: Agent responds with text messages only
   - Sets voice to 'none'
   - No audio output

2. **Voice**: Agent responds with synthesized speech
   - Browse available voices by vendor
   - Search by voice ID or description
   - Displays voice metadata

3. **Avatar**: HeyGen streaming avatar (coming soon)
   - Browse available avatar poses
   - Search by avatar ID or pose name
   - Currently displays "coming soon" notice

### Advanced State Management

```tsx
// Mode detection from SDK state
const currentMode = useMemo(() => {
  if (!currentVoice || currentVoice.voice_id === 'none') {
    return { type: 'text', label: 'Text Only', icon: Type }
  }
  
  if (currentVoice.voice_id === 'avatar') {
    return { type: 'avatar', label: avatarName, icon: User }
  }
  
  return { type: 'voice', label: voiceDescription, icon: Mic }
}, [currentVoice, avatars])
```

### Voice Filtering

The component automatically filters system voices:

```tsx
// Excludes 'none' and 'avatar' system voices
const filteredVoices = filterRegularVoices(data.voices)

// Only shows public avatars
const activeAvatars = data.avatars.filter(a => a.is_public)
```

## ThemeSwitcher

Toggle between light, dark, and system theme preferences.

### Purpose

Provides theme switching functionality with smooth transitions and system preference support.

### Props Interface

```typescript
interface ThemeSwitcherProps {
  className?: string                    // Additional CSS classes
  size?: "default" | "sm" | "lg" | "icon" // Button size
  showLabel?: boolean                   // Show "Theme" label
}
```

### Usage Examples

```tsx
import { ThemeSwitcher } from '@agentc/realtime-ui'

// Icon-only for headers
<ThemeSwitcher size="icon" />

// With label for settings
<ThemeSwitcher 
  size="default"
  showLabel={true}
/>

// Compact version
<ThemeSwitcher 
  size="sm"
  className="ml-auto"
/>
```

### Theme Transitions

Features smooth icon transitions:

- Sun icon rotates and scales out when switching to dark
- Moon icon rotates and scales in for dark mode
- Both icons visible with proper transition timing

### Integration with Next.js

```tsx
// Uses next-themes internally
import { useTheme } from "next-themes"

// Prevents hydration mismatch
const [mounted, setMounted] = useState(false)
useEffect(() => setMounted(true), [])
```

## CopyLinkButton

Allows users to copy session URLs to clipboard with visual feedback.

### Purpose

Enables sharing of session links with automatic URL construction and clipboard management.

### Props Interface

```typescript
interface CopyLinkButtonProps extends 
  Omit<React.ButtonHTMLAttributes<HTMLButtonElement>, 'onClick' | 'size'>,
  Pick<VariantProps<typeof buttonVariants>, 'variant' | 'size'> {
  sessionId: string          // Session ID for URL construction
  className?: string         // Additional CSS classes
}
```

### Usage Examples

```tsx
import { CopyLinkButton } from '@agentc/realtime-ui'

// Icon button in header
<CopyLinkButton 
  sessionId="friendly-panda" 
  variant="ghost" 
  size="icon" 
/>

// Full button with text
<CopyLinkButton 
  sessionId={sessionId}
  variant="outline"
  size="default"
/>

// Success state styling
<CopyLinkButton 
  sessionId={sessionId}
  className="transition-all"
/>
```

### Visual Feedback

- **Default**: Link2 icon
- **Success**: Check icon (green) for 2 seconds
- **Error**: Red icon with error tooltip
- **Background**: Green tint on success

### Clipboard Handling

```tsx
// Modern clipboard API with fallback
if (!navigator.clipboard) {
  // Legacy fallback using textarea
  const textArea = document.createElement('textarea')
  document.execCommand('copy')
} else {
  // Modern API
  await navigator.clipboard.writeText(url)
}
```

## Control Panel Patterns

### Basic Control Panel

```tsx
export function ControlPanel() {
  return (
    <Card className="p-4">
      <div className="space-y-4">
        {/* Connection Section */}
        <div className="flex items-center justify-between">
          <ConnectionStatus compact />
          <ConnectionButton size="sm" />
        </div>
        
        {/* Configuration Section */}
        <div className="grid grid-cols-2 gap-4">
          <div>
            <Label>Agent</Label>
            <AgentSelector className="w-full" />
          </div>
          <div>
            <Label>Output</Label>
            <OutputSelector className="w-full" />
          </div>
        </div>
        
        {/* Audio Section */}
        <div>
          <Label>Audio Controls</Label>
          <AudioControls className="w-full" />
        </div>
      </div>
    </Card>
  )
}
```

### Collapsible Settings Panel

```tsx
export function SettingsPanel() {
  const [expanded, setExpanded] = useState(false)
  
  return (
    <Collapsible open={expanded} onOpenChange={setExpanded}>
      <CollapsibleTrigger asChild>
        <Button variant="ghost" className="w-full justify-between">
          <span>Settings</span>
          <ChevronDown className={cn(
            "h-4 w-4 transition-transform",
            expanded && "rotate-180"
          )} />
        </Button>
      </CollapsibleTrigger>
      
      <CollapsibleContent className="space-y-4 pt-4">
        {/* Connection Settings */}
        <section>
          <h3 className="text-sm font-medium mb-2">Connection</h3>
          <ConnectionStatus showDetails={true} />
        </section>
        
        {/* Agent Settings */}
        <section>
          <h3 className="text-sm font-medium mb-2">Agent Configuration</h3>
          <div className="space-y-2">
            <AgentSelector className="w-full" />
            <OutputSelector className="w-full" />
          </div>
        </section>
        
        {/* Theme Settings */}
        <section>
          <h3 className="text-sm font-medium mb-2">Appearance</h3>
          <ThemeSwitcher showLabel={true} />
        </section>
      </CollapsibleContent>
    </Collapsible>
  )
}
```

## Settings Management

### Persistent Settings with Local Storage

```tsx
export function PersistentSettings() {
  const [settings, setSettings] = useState(() => {
    // Load from localStorage
    const stored = localStorage.getItem('agentc-settings')
    return stored ? JSON.parse(stored) : defaultSettings
  })
  
  // Save to localStorage on change
  useEffect(() => {
    localStorage.setItem('agentc-settings', JSON.stringify(settings))
  }, [settings])
  
  return (
    <div className="space-y-4">
      {/* Auto-connect on load */}
      <div className="flex items-center justify-between">
        <Label htmlFor="auto-connect">Auto-connect</Label>
        <Switch
          id="auto-connect"
          checked={settings.autoConnect}
          onCheckedChange={(checked) => 
            setSettings(prev => ({ ...prev, autoConnect: checked }))
          }
        />
      </div>
      
      {/* Default output mode */}
      <div>
        <Label>Default Output Mode</Label>
        <RadioGroup
          value={settings.defaultOutput}
          onValueChange={(value) =>
            setSettings(prev => ({ ...prev, defaultOutput: value }))
          }
        >
          <RadioGroupItem value="text" label="Text Only" />
          <RadioGroupItem value="voice" label="Voice" />
        </RadioGroup>
      </div>
    </div>
  )
}
```

### Settings Context Provider

```tsx
const SettingsContext = createContext<SettingsContextType>(null)

export function SettingsProvider({ children }) {
  const [settings, dispatch] = useReducer(settingsReducer, defaultSettings)
  
  // Apply settings to SDK
  const client = useRealtimeClient()
  
  useEffect(() => {
    if (client && settings.autoConnect) {
      client.connect()
    }
  }, [client, settings.autoConnect])
  
  return (
    <SettingsContext.Provider value={{ settings, dispatch }}>
      {children}
    </SettingsContext.Provider>
  )
}
```

## Form Validation Patterns

### Agent Configuration Form

```tsx
export function AgentConfigForm() {
  const [errors, setErrors] = useState({})
  const [isSubmitting, setIsSubmitting] = useState(false)
  
  const validateForm = (values) => {
    const errors = {}
    
    if (!values.agent) {
      errors.agent = 'Please select an agent'
    }
    
    if (!values.outputMode) {
      errors.outputMode = 'Please select an output mode'
    }
    
    if (values.outputMode === 'voice' && !values.voiceId) {
      errors.voiceId = 'Please select a voice'
    }
    
    return errors
  }
  
  const handleSubmit = async (e) => {
    e.preventDefault()
    setIsSubmitting(true)
    
    const formData = new FormData(e.target)
    const values = Object.fromEntries(formData)
    
    const validationErrors = validateForm(values)
    if (Object.keys(validationErrors).length > 0) {
      setErrors(validationErrors)
      setIsSubmitting(false)
      return
    }
    
    try {
      // Apply configuration
      await applyConfiguration(values)
      setErrors({})
    } catch (error) {
      setErrors({ submit: error.message })
    } finally {
      setIsSubmitting(false)
    }
  }
  
  return (
    <form onSubmit={handleSubmit} className="space-y-4">
      <div>
        <Label htmlFor="agent">Agent</Label>
        <AgentSelector
          name="agent"
          className={cn(errors.agent && "border-destructive")}
          aria-invalid={!!errors.agent}
          aria-describedby={errors.agent ? "agent-error" : undefined}
        />
        {errors.agent && (
          <p id="agent-error" className="text-sm text-destructive mt-1">
            {errors.agent}
          </p>
        )}
      </div>
      
      <div>
        <Label htmlFor="outputMode">Output Mode</Label>
        <OutputSelector
          name="outputMode"
          className={cn(errors.outputMode && "border-destructive")}
          showErrorAlerts={false}
        />
        {errors.outputMode && (
          <p className="text-sm text-destructive mt-1">
            {errors.outputMode}
          </p>
        )}
      </div>
      
      {errors.submit && (
        <Alert variant="destructive">
          <AlertDescription>{errors.submit}</AlertDescription>
        </Alert>
      )}
      
      <Button 
        type="submit" 
        disabled={isSubmitting}
        className="w-full"
      >
        {isSubmitting ? (
          <>
            <Loader2 className="mr-2 h-4 w-4 animate-spin" />
            Applying...
          </>
        ) : (
          'Apply Configuration'
        )}
      </Button>
    </form>
  )
}
```

### Error Recovery Patterns

```tsx
export function ErrorBoundaryControls() {
  const [hasError, setHasError] = useState(false)
  const { error, reconnect } = useConnection()
  
  if (hasError || error) {
    return (
      <Card className="p-4 border-destructive">
        <div className="space-y-3">
          <div className="flex items-center gap-2 text-destructive">
            <AlertCircle className="h-5 w-5" />
            <h3 className="font-medium">Connection Error</h3>
          </div>
          
          <p className="text-sm text-muted-foreground">
            {error?.message || 'An unexpected error occurred'}
          </p>
          
          <div className="flex gap-2">
            <Button
              variant="outline"
              onClick={() => window.location.reload()}
            >
              Refresh Page
            </Button>
            <Button
              onClick={() => {
                setHasError(false)
                reconnect()
              }}
            >
              Try Again
            </Button>
          </div>
        </div>
      </Card>
    )
  }
  
  return <ControlPanel />
}
```

## Responsive Layouts

### Mobile-Optimized Controls

```tsx
export function MobileControls() {
  const [drawerOpen, setDrawerOpen] = useState(false)
  
  return (
    <>
      {/* Mobile trigger button */}
      <Button
        variant="outline"
        size="icon"
        className="md:hidden fixed bottom-4 right-4 z-50"
        onClick={() => setDrawerOpen(true)}
      >
        <Settings className="h-5 w-5" />
      </Button>
      
      {/* Mobile drawer */}
      <Sheet open={drawerOpen} onOpenChange={setDrawerOpen}>
        <SheetContent side="bottom" className="h-[80vh]">
          <SheetHeader>
            <SheetTitle>Controls</SheetTitle>
          </SheetHeader>
          
          <div className="mt-4 space-y-4 overflow-auto">
            {/* Stack controls vertically on mobile */}
            <div className="space-y-3">
              <div>
                <Label>Connection</Label>
                <ConnectionButton className="w-full" />
              </div>
              
              <div>
                <Label>Agent</Label>
                <AgentSelector className="w-full" />
              </div>
              
              <div>
                <Label>Output Mode</Label>
                <OutputSelector className="w-full" />
              </div>
              
              <div>
                <Label>Audio</Label>
                <AudioControls 
                  className="w-full"
                  showLevelIndicator={true}
                />
              </div>
            </div>
          </div>
        </SheetContent>
      </Sheet>
    </>
  )
}
```

### Adaptive Grid Layout

```tsx
export function AdaptiveControlGrid() {
  return (
    <div className={cn(
      "grid gap-4",
      // Responsive grid columns
      "grid-cols-1",           // Mobile: single column
      "sm:grid-cols-2",        // Small: 2 columns
      "lg:grid-cols-4",        // Large: 4 columns
    )}>
      {/* Connection spans 2 columns on small screens */}
      <div className="sm:col-span-2 lg:col-span-1">
        <ConnectionStatus />
      </div>
      
      {/* Selectors adapt to available space */}
      <div className="lg:col-span-1">
        <AgentSelector className="w-full" />
      </div>
      
      <div className="lg:col-span-1">
        <OutputSelector className="w-full" />
      </div>
      
      {/* Audio controls span remaining space */}
      <div className="sm:col-span-2 lg:col-span-1">
        <AudioControls className="w-full" />
      </div>
    </div>
  )
}
```

### Responsive Text and Spacing

```tsx
export function ResponsiveControlBar() {
  return (
    <div className={cn(
      "flex items-center gap-2 p-2",
      "sm:gap-3 sm:p-3",        // Increase spacing on small
      "md:gap-4 md:p-4",        // More spacing on medium
      "lg:gap-6 lg:p-6"         // Maximum spacing on large
    )}>
      {/* Hide labels on mobile, show on desktop */}
      <div className="flex items-center gap-2">
        <span className="hidden sm:inline text-sm font-medium">
          Status:
        </span>
        <ConnectionIndicator />
      </div>
      
      {/* Responsive button sizes */}
      <ConnectionButton 
        size="sm"
        className="sm:hidden"    // Small on mobile
      />
      <ConnectionButton 
        size="default"
        className="hidden sm:block" // Default on desktop
      />
      
      {/* Collapsible options on mobile */}
      <div className="ml-auto flex items-center gap-2">
        <div className="hidden md:flex gap-2">
          <AgentSelector />
          <OutputSelector />
        </div>
        
        {/* Mobile menu for collapsed items */}
        <DropdownMenu>
          <DropdownMenuTrigger asChild className="md:hidden">
            <Button variant="ghost" size="icon">
              <MoreVertical className="h-4 w-4" />
            </Button>
          </DropdownMenuTrigger>
          <DropdownMenuContent align="end" className="w-48">
            <DropdownMenuItem>
              <Bot className="mr-2 h-4 w-4" />
              Change Agent
            </DropdownMenuItem>
            <DropdownMenuItem>
              <Mic className="mr-2 h-4 w-4" />
              Output Settings
            </DropdownMenuItem>
          </DropdownMenuContent>
        </DropdownMenu>
      </div>
    </div>
  )
}
```

## Best Practices

### Performance Optimization

1. **Memoization**: Use `React.memo` for expensive renders
2. **Debouncing**: Debounce search inputs in selectors
3. **Lazy Loading**: Load avatar data only when needed
4. **Portal Rendering**: Use portals for overlays to avoid reflow

### Accessibility

1. **ARIA Labels**: All controls have descriptive labels
2. **Keyboard Navigation**: Full keyboard support with Tab/Enter/Escape
3. **Screen Reader**: Live regions for status updates
4. **Focus Management**: Proper focus trapping in modals

### Error Handling

1. **Graceful Degradation**: Controls disable when SDK unavailable
2. **Error Messages**: Clear, actionable error messages
3. **Recovery Options**: Retry buttons and fallback states
4. **Validation Feedback**: Inline validation with helpful messages

### State Management

1. **Single Source of Truth**: SDK hooks as authoritative state
2. **Optimistic Updates**: Show loading states immediately
3. **Error Boundaries**: Catch and handle component errors
4. **Context Providers**: Share settings across components

## Related Documentation

- [SDK Hooks Reference](../sdk/hooks.md)
- [Chat Components](./chat-components.md)
- [Audio Components](./audio-components.md)
- [CenSuite Design System](../../design/censuite-guidelines.md)
- [Testing Standards](../../testing/standards.md)