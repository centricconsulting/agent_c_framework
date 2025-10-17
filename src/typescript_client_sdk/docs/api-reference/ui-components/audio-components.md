# Audio Components API Reference

The audio components in `@agentc/realtime-ui` provide comprehensive audio controls for the Agent C Realtime Client SDK. These components integrate seamlessly with `@agentc/realtime-react` hooks and follow the CenSuite design system.

## Table of Contents
- [AudioControlsPanel](#audiocontrolspanel)
- [RecordingButton](#recordingbutton)
- [MuteToggle](#mutetoggle)
- [VoiceVisualizerView](#voicevisualizerview)

---

## AudioControlsPanel

### Overview
A comprehensive audio control panel that combines recording, mute, volume, and device selection controls with optional level metering and collapsible functionality.

### Props Interface

```typescript
export interface AudioControlsPanelProps extends React.HTMLAttributes<HTMLDivElement> {
  /**
   * Layout orientation
   */
  orientation?: 'horizontal' | 'vertical'
  
  /**
   * Allow panel to be collapsed
   */
  collapsible?: boolean
  
  /**
   * Show device selector
   */
  showDeviceSelector?: boolean
  
  /**
   * Show audio level meter
   */
  showLevelMeter?: boolean
}
```

### Usage Examples

#### Basic Usage
```tsx
import { AudioControlsPanel } from '@agentc/realtime-ui'

function AudioPage() {
  return (
    <AudioControlsPanel />
  )
}
```

#### Advanced Configuration
```tsx
import { AudioControlsPanel } from '@agentc/realtime-ui'

function AdvancedAudioControls() {
  return (
    <AudioControlsPanel
      orientation="horizontal"
      collapsible={true}
      showDeviceSelector={true}
      showLevelMeter={true}
      className="shadow-lg rounded-lg"
    />
  )
}
```

#### Mobile-Responsive Implementation
```tsx
import { AudioControlsPanel } from '@agentc/realtime-ui'

function ResponsiveAudioControls() {
  return (
    <AudioControlsPanel
      orientation="vertical"  // Better for mobile
      collapsible={true}
      showDeviceSelector={false}  // Hide on mobile to save space
      showLevelMeter={true}
    />
  )
}
```

### CenSuite Design System Compliance
- **Spacing**: Uses 4px base unit scale (p-4, gap-4, gap-2)
- **Colors**: Semantic color tokens (bg-background, border, text-muted-foreground)
- **Typography**: Follows text sizing standards (text-sm, text-xs)
- **Interactive States**: Focus-visible styles on all interactive elements
- **Responsive**: Automatically adapts layout for mobile devices

### Accessibility Features
- **ARIA Labels**: Comprehensive labeling for screen readers
  - `role="group"` with `aria-label="Audio controls"`
  - Individual control labels (volume slider, device selector)
  - Live region for status announcements
- **Keyboard Navigation**: Full keyboard support
  - Tab navigation through all controls
  - Slider controls with arrow keys
  - Collapsible with Enter/Space
- **Screen Reader Support**: 
  - Audio level announcements
  - Clipping/silence warnings
  - Permission error messages

### Hook Integration
Integrates with `useAudio` hook from `@agentc/realtime-react`:
- `audioLevel`: Real-time audio level monitoring
- `isRecording`: Recording state management
- `volume`/`setVolume`: Volume control
- `isMuted`/`setMuted`: Mute state management
- `inputDevice`/`setInputDevice`: Device selection

### Customization Options
- **Layout**: Horizontal or vertical orientation
- **Collapsible State**: Can be minimized to save space
- **Device Selection**: Optional microphone selector
- **Level Meter**: Visual audio level indicator with clipping detection
- **Mobile Adaptation**: Automatic responsive behavior

### Related Components
- [RecordingButton](#recordingbutton) - Used for recording control
- [MuteToggle](#mutetoggle) - Used for mute control
- [Slider](#slider) - Used for volume control (from shadcn/ui)
- [Select](#select) - Used for device selection (from shadcn/ui)

---

## RecordingButton

### Overview
A button component that controls audio recording state with connection awareness and error handling.

### Props Interface

```typescript
export interface RecordingButtonProps extends React.ButtonHTMLAttributes<HTMLButtonElement> {
  /**
   * Show tooltip on hover
   */
  showTooltip?: boolean
  
  /**
   * Button size variant
   */
  size?: 'small' | 'default' | 'large'
  
  /**
   * Custom icon component
   */
  icon?: React.ReactNode
}
```

### Usage Examples

#### Basic Usage
```tsx
import { RecordingButton } from '@agentc/realtime-ui'

function SimpleRecorder() {
  return (
    <RecordingButton />
  )
}
```

#### Custom Icon and Size
```tsx
import { RecordingButton } from '@agentc/realtime-ui'
import { Phone } from 'lucide-react'

function CustomRecordingButton() {
  return (
    <RecordingButton
      size="large"
      icon={<Phone className="h-5 w-5" />}
      showTooltip={true}
    />
  )
}
```

#### With Error Handling Display
```tsx
import { RecordingButton } from '@agentc/realtime-ui'

function RecorderWithStatus() {
  return (
    <div className="flex flex-col items-center gap-2">
      <RecordingButton
        size="default"
        showTooltip={true}
        className="shadow-md"
      />
      {/* Error messages appear automatically below the button */}
    </div>
  )
}
```

### CenSuite Design System Compliance
- **Button Variants**: Uses standard button variants (default/destructive)
- **Sizing**: Follows button size scale (h-8/h-10/h-12)
- **Colors**: Destructive variant when recording (visual feedback)
- **Animation**: Pulse animation during active recording
- **Focus States**: Standard focus-visible ring implementation

### Accessibility Features
- **ARIA Attributes**:
  - `aria-label`: "Start recording"/"Stop recording"
  - `aria-pressed`: Indicates recording state
  - `aria-busy`: Shows connection loading state
- **Keyboard Support**: 
  - Activatable with Enter/Space
  - Tab focusable
- **Error Announcements**: 
  - `role="alert"` for error messages
  - Live region for status changes
- **Loading State**: 
  - Screen reader announcement for "Connecting..."
  - Visual spinner indicator

### Hook Integration
Uses `useAudio` and `useConnection` hooks:
- `isRecording`: Current recording state
- `startRecording`/`stopRecording`: Recording control methods
- `isConnected`: Connection state check
- `connectionState`: Connection status for disabling

### Customization Options
- **Size Variants**: Small (h-8), default (h-10), large (h-12)
- **Custom Icons**: Replace default Mic/MicOff icons
- **Tooltip Support**: Optional hover tooltips
- **Error Display**: Automatic error message rendering
- **Disabled States**: Connection-aware disabling

### Related Components
- [AudioControlsPanel](#audiocontrolspanel) - Contains RecordingButton
- [MuteToggle](#mutetoggle) - Complementary audio control
- Button component from shadcn/ui

---

## MuteToggle

### Overview
A toggle button for muting/unmuting audio with optional confirmation dialog and keyboard shortcuts.

### Props Interface

```typescript
export interface MuteToggleProps extends React.ButtonHTMLAttributes<HTMLButtonElement> {
  /**
   * Enable keyboard shortcuts
   */
  enableShortcut?: boolean
  
  /**
   * Require confirmation for unmute
   */
  requireConfirmation?: boolean
  
  /**
   * Show audio level indicator
   */
  showAudioIndicator?: boolean
  
  /**
   * Button size
   */
  size?: 'small' | 'default' | 'large'
}
```

### Usage Examples

#### Basic Usage
```tsx
import { MuteToggle } from '@agentc/realtime-ui'

function SimpleMuteControl() {
  return (
    <MuteToggle />
  )
}
```

#### With Keyboard Shortcut and Confirmation
```tsx
import { MuteToggle } from '@agentc/realtime-ui'

function AdvancedMuteControl() {
  return (
    <MuteToggle
      enableShortcut={true}  // Ctrl+M / Cmd+M
      requireConfirmation={true}
      showAudioIndicator={true}
      size="large"
    />
  )
}
```

#### In a Toolbar Layout
```tsx
import { MuteToggle, RecordingButton } from '@agentc/realtime-ui'

function AudioToolbar() {
  return (
    <div className="flex items-center gap-2 p-2 bg-muted rounded-lg">
      <RecordingButton size="small" />
      <MuteToggle 
        size="small"
        showAudioIndicator={true}
      />
      <span className="text-sm text-muted-foreground">
        Press Ctrl+M to toggle mute
      </span>
    </div>
  )
}
```

### CenSuite Design System Compliance
- **Button Styling**: Uses ghost variant for subtle appearance
- **Size System**: Consistent with button size scale
- **Color States**: Muted state uses text-muted-foreground
- **Visual Feedback**: Pulse animation for audio activity
- **Focus Management**: Standard focus-visible implementation

### Accessibility Features
- **ARIA Attributes**:
  - `aria-label`: "Mute audio"/"Unmute audio"
  - `aria-pressed`: Indicates mute state
  - `aria-live="polite"`: For state announcements
- **Keyboard Support**:
  - Activatable with Enter/Space
  - Optional Ctrl+M/Cmd+M shortcut
- **Screen Reader Announcements**:
  - Hidden span announces "Audio is muted/unmuted"
  - Live region for state changes
- **Confirmation Dialog**:
  - `role="dialog"` for confirmation modal
  - Focus management within dialog

### Hook Integration
Uses `useAudio` hook:
- `isMuted`: Current mute state
- `toggleMute`: Mute toggle method
- `isRecording`: Disables during recording
- `audioLevel`: For visual indicator

### Customization Options
- **Keyboard Shortcuts**: Enable Ctrl+M/Cmd+M toggle
- **Confirmation Dialog**: Optional unmute confirmation
- **Audio Indicator**: Visual pulse when audio detected
- **Size Variants**: Small, default, large sizes
- **Disabled During Recording**: Automatic disable when recording

### Related Components
- [AudioControlsPanel](#audiocontrolspanel) - Contains MuteToggle
- [RecordingButton](#recordingbutton) - Complementary control
- Button component from shadcn/ui

---

## VoiceVisualizerView

### Overview
A placeholder component for voice visualization that shows audio activity. Will be enhanced with Three.js visualization in future updates.

### Props Interface

```typescript
export interface VoiceVisualizerViewProps {
  className?: string
}
```

### Usage Examples

#### Basic Usage
```tsx
import { VoiceVisualizerView } from '@agentc/realtime-ui'

function VoiceMode() {
  return (
    <VoiceVisualizerView className="h-64" />
  )
}
```

#### Full Screen Voice Mode
```tsx
import { VoiceVisualizerView } from '@agentc/realtime-ui'

function FullScreenVoice() {
  return (
    <div className="fixed inset-0 bg-background">
      <VoiceVisualizerView className="w-full h-full" />
    </div>
  )
}
```

#### With Audio Controls
```tsx
import { VoiceVisualizerView, AudioControlsPanel } from '@agentc/realtime-ui'

function VoiceInterface() {
  return (
    <div className="flex flex-col h-screen">
      <VoiceVisualizerView className="flex-1" />
      <AudioControlsPanel 
        orientation="horizontal"
        showLevelMeter={false}  // Visualizer shows levels
      />
    </div>
  )
}
```

### CenSuite Design System Compliance
- **Background**: Gradient from background to muted/20
- **Typography**: Standard text sizes (text-lg, text-sm)
- **Colors**: Primary color for waveform icon
- **Spacing**: Consistent spacing scale (space-y-4)
- **Animation**: Pulse animation during recording

### Accessibility Features
- **Visual Feedback**: 
  - Icon opacity changes with audio level
  - Scale transformation based on activity
  - Audio level bar for visual indication
- **Text Alternatives**:
  - Clear status text ("Listening..." / "Voice Mode Active")
  - Descriptive messages for current state

### Hook Integration
Uses `useAudio` hook:
- `isRecording`: Shows listening state
- `audioLevel`: Drives visual animations and level indicator

### Customization Options
- **Custom Sizing**: Via className prop
- **Background Styling**: Gradient customizable via className
- **Future Enhancement**: Will support Three.js visualization

### Related Components
- [AudioControlsPanel](#audiocontrolspanel) - Complementary controls
- [RecordingButton](#recordingbutton) - Recording control
- Future Three.js integration for advanced visualization

---

## Common Patterns and Best Practices

### Error Handling
All audio components include robust error handling:
```tsx
// Components automatically handle and display errors
<RecordingButton />  // Shows "Microphone not available" if needed
<AudioControlsPanel showDeviceSelector />  // Shows permission errors
```

### Mobile Responsiveness
Components automatically adapt to mobile devices:
```tsx
// AudioControlsPanel detects mobile and adjusts layout
<AudioControlsPanel 
  orientation="horizontal"  // Becomes vertical on mobile
/>
```

### Connection Awareness
Components respect connection state:
```tsx
// Buttons automatically disable when disconnected
<RecordingButton />  // Disabled when not connected
<MuteToggle />  // Disabled during recording
```

### Accessibility First
All components follow WCAG 2.1 AA standards:
```tsx
// Full keyboard navigation and screen reader support
<AudioControlsPanel 
  showLevelMeter={true}  // Includes aria-valuenow for meters
  showDeviceSelector={true}  // Proper labeling for selects
/>
```

### State Management
Components integrate with React context via hooks:
```tsx
// No prop drilling needed - components use hooks directly
function App() {
  return (
    <AgentCProvider config={config}>
      <AudioControlsPanel />  {/* Automatically connected to state */}
    </AgentCProvider>
  )
}
```

## Import Statement

```typescript
// Import all audio components
import { 
  AudioControlsPanel,
  RecordingButton,
  MuteToggle,
  VoiceVisualizerView,
  type AudioControlsPanelProps,
  type RecordingButtonProps,
  type MuteToggleProps,
  type VoiceVisualizerViewProps
} from '@agentc/realtime-ui'
```

## CSS Requirements

These components require the following CSS variables from the CenSuite design system:

```css
/* Required CSS variables */
--background: 255 100% 100%;
--foreground: 255 5% 10%;
--primary: 255 55% 23.5%;
--primary-foreground: 0 0% 100%;
--destructive: 0 100% 50%;
--destructive-foreground: 255 5% 100%;
--muted: 217 30% 95%;
--muted-foreground: 255 5% 40%;
--border: 255 30% 82%;
--input: 255 30% 50%;
--ring: 255 55% 23.5%;
--accent: 255 30% 90%;
--accent-foreground: 0 0% 0%;
```

## TypeScript Support

All components are fully typed with TypeScript and export their prop interfaces:

```typescript
import type { 
  AudioControlsPanelProps,
  RecordingButtonProps,
  MuteToggleProps,
  VoiceVisualizerViewProps 
} from '@agentc/realtime-ui'
```

## Testing

Components can be tested using @testing-library/react:

```typescript
import { render, screen, fireEvent } from '@testing-library/react'
import { AudioControlsPanel } from '@agentc/realtime-ui'
import { AgentCProvider } from '@agentc/realtime-react'

describe('AudioControlsPanel', () => {
  it('should toggle mute state', () => {
    render(
      <AgentCProvider config={mockConfig}>
        <AudioControlsPanel />
      </AgentCProvider>
    )
    
    const muteButton = screen.getByLabelText(/mute audio/i)
    fireEvent.click(muteButton)
    
    expect(screen.getByLabelText(/unmute audio/i)).toBeInTheDocument()
  })
})
```