# Agent C Realtime Chat Interface Design Document (Revised)

## Executive Summary

This document outlines the component architecture and visual design for the Agent C Realtime chat interface. The interface supports three distinct output modes (chat messages, HeyGen avatar, voice visualizer) and leverages our existing InputArea component for user input.

## Design Principles

1. **Clarity First**: Every UI element must have clear purpose and state
2. **SDK-Aligned**: Only use functionality that exists in our current hooks
3. **Progressive Enhancement**: Build core features first, stub advanced features
4. **Accessibility**: WCAG 2.1 AA compliance throughout
5. **Responsive**: Mobile-first with desktop enhancements
6. **Mode Flexibility**: Main content area adapts to selected output mode

## Overall Layout Architecture

### Page Structure

```tsx
<div className="min-h-screen bg-background flex">
  {/* Sidebar - Desktop: sticky, Mobile: overlay */}
  <ChatSidebar />

  {/* Main Content Area */}
  <div className="flex-1 flex flex-col overflow-hidden">
    {/* Header Toolbar with Output Mode Selector */}
    <ChatHeader />

    {/* Dynamic Content Area - Changes based on selected output mode */}
    <MainContentArea outputMode={outputMode}>
      {outputMode === 'chat' && <ChatMessagesView />}
      {outputMode === 'avatar' && <AvatarDisplayView />}
      {outputMode === 'voice' && <VoiceVisualizerView />}
    </MainContentArea>

    {/* Input Area - Using existing InputArea component */}
    <InputArea />
  </div>
</div>
```

### Responsive Breakpoint Strategy

- **Mobile (default)**: Single column, overlay sidebar
- **Tablet (md)**: Improved spacing, better touch targets
- **Desktop (lg)**: Sidebar visible, optimal reading width

## Component Hierarchy

### 1. Primary Layout Components

#### ChatLayout (Container)

```tsx
// CenSuite Classes
className="min-h-screen bg-background text-foreground"

// Responsibilities:
- Manages overall layout structure
- Handles sidebar toggle state
- Provides layout context to children
```

#### ChatSidebar (Complete Redesign)

```tsx
// Desktop Classes
className="w-72 border-r border-border bg-muted/30 lg:sticky lg:top-0 lg:h-screen flex flex-col"

// Mobile Classes  
className="fixed inset-y-0 left-0 z-50 w-72 bg-background shadow-lg lg:shadow-none"

// Three Distinct Components:
```

##### 1. SidebarTopMenu

```tsx
className="px-2 pt-1 gap-px mb-6"

// New Chat Button (Functional NOW)
className="inline-flex items-center justify-center h-9 px-4 py-2 rounded-lg w-full
           bg-primary text-primary-foreground hover:bg-primary/90
           transition duration-300"

// Action: Clears current session and starts fresh
onClick={() => {
  // Clear messages
  // Reset connection if needed
  // Initialize new session
}}
```

##### 2. ChatSessionList (Full Design Available)

```tsx
className="flex-grow overflow-y-auto px-2"

// Full component specification available in:
// .scratch/design_docs/chat_session_list_design.md

// Key Features:
- Session grouping by time (Today, Yesterday, This Week, etc.)
- Rich session information display
- Message previews and token counts
- Active session highlighting
- Search and filtering
- Empty state handling
- Mobile-optimized interactions

// Integration with API:
// Uses sessions array from login response:
interface ChatSession {
  session_id: string
  session_name: string | null
  created_at: string | null
  updated_at: string | null
  token_count: number
  messages: Message[]
  agent_config: AgentConfiguration
}
```

##### 3. UserDisplay (Functional NOW)

```tsx
className="px-2 pb-1 border-t border-border mt-auto"

// User Button Trigger
<button className="flex flex-row flex-grow items-center !px-1.5 py-6 gap-3 w-full
                   hover:bg-muted rounded-lg transition-colors">
  <div className="flex-shrink-0 size-8 rounded-full bg-primary/10 flex items-center justify-center">
    {user.avatar || user.name[0]}
  </div>
  <div className="flex flex-col items-start overflow-hidden pr-4">
    <span className="truncate text-sm font-medium">{user.name}</span>
    <span className="text-xs text-muted-foreground">{user.email}</span>
  </div>
  <ChevronUp className="h-4 w-4 text-muted-foreground" />
</button>

// Dropdown Menu (Using Radix UI pattern)
<DropdownMenu>
  <DropdownMenuTrigger asChild>
    {/* User button above */}
  </DropdownMenuTrigger>
  <DropdownMenuContent className="w-56" align="end" side="top">
    <DropdownMenuItem onClick={handleLogout}>
      <LogOut className="mr-2 h-4 w-4" />
      <span>Log out</span>
    </DropdownMenuItem>
  </DropdownMenuContent>
</DropdownMenu>

// User data from authentication context
const { user } = useAuth() // Will have name, email, avatar from login
```

#### ChatHeader

```tsx
className="sticky top-0 z-40 border-b border-border bg-background/95 backdrop-blur supports-[backdrop-filter]:bg-background/60"

// Height & Padding
className="h-14 px-4 flex items-center justify-between"

// Content:
- Left: Menu toggle (mobile), Agent name
- Center: SessionNameDropdown component (current session name with dropdown)
- Right: Connection status, Audio controls
```

##### SessionNameDropdown Component (STUB - Functionality Coming Later)

```tsx
// Component Purpose:
// Displays the current session name with dropdown menu for session management
// This is a STUB component - full functionality will be implemented when
// working on chat sessions/messages feature

// Visual Design
className="inline-flex items-center gap-2 px-3 py-1.5 rounded-md hover:bg-muted transition-colors cursor-pointer"

// Component Structure
<DropdownMenu>
  <DropdownMenuTrigger asChild>
    <button className="inline-flex items-center gap-2 px-3 py-1.5 rounded-md hover:bg-muted transition-colors">
      <span className="text-sm font-medium truncate max-w-[200px]">
        {sessionName || "New Chat"}
      </span>
      <ChevronDown className="h-4 w-4 text-muted-foreground" />
    </button>
  </DropdownMenuTrigger>
  <DropdownMenuContent align="center" className="w-56">
    <DropdownMenuItem disabled>
      <Edit2 className="mr-2 h-4 w-4" />
      <span>Rename session</span>
      <span className="ml-auto text-xs text-muted-foreground">Coming soon</span>
    </DropdownMenuItem>
    <DropdownMenuItem disabled>
      <Trash2 className="mr-2 h-4 w-4" />
      <span>Delete session</span>
      <span className="ml-auto text-xs text-muted-foreground">Coming soon</span>
    </DropdownMenuItem>
  </DropdownMenuContent>
</DropdownMenu>

// Stub Implementation Note:
// For now, this will simply display "New Chat" and show the dropdown
// with disabled options. When session management is implemented,
// this will connect to session state management.
```

**Important Note:** The OutputSelector component that controls output mode (chat/avatar/voice) is already part of the InputArea component toolbar. Users select their preferred output mode from the input area, not from the header.

### 2. Main Content Area Components (3 Display Modes)

#### MainContentArea (Container)

```tsx
className="flex-1 overflow-hidden relative"

// Handles switching between three display modes
interface MainContentAreaProps {
  outputMode: 'chat' | 'avatar' | 'voice'
  children: React.ReactNode
}
```

#### Mode A: ChatMessagesView (PLACEHOLDER for now)

```tsx
// Container
className="flex-1 overflow-y-auto"

// Placeholder Component (Complex design coming later)
<div className="flex items-center justify-center h-full text-muted-foreground">
  <div className="text-center space-y-2">
    <MessageSquare className="h-12 w-12 mx-auto opacity-50" />
    <p className="text-sm">Chat messages will appear here</p>
    <p className="text-xs">Complex message rendering coming soon</p>
  </div>
</div>

// Note: Full chat message design will be created separately
// Will include rich formatting, code blocks, artifacts, etc.
```

##### Note on OutputSelector Location

The OutputSelector component for switching between chat/avatar/voice modes is part of the InputArea component toolbar (already implemented). This keeps mode selection close to where users input their messages, following the principle of proximity in UI design.

#### Mode B: AvatarDisplayView (Based on HeyGen Reference)

```tsx
// Container matching HeyGen demo structure
className="flex flex-col items-center justify-center h-full p-4"

// Video Container
<div className="relative w-full max-w-[900px] aspect-video overflow-hidden rounded-xl bg-zinc-900">
  {/* Video Element */}
  <video
    ref={videoRef}
    autoPlay
    playsInline
    className="w-full h-full object-contain"
    style={{ objectFit: 'contain' }}
  />

  {/* Connection Quality Indicator */}
  <div className="absolute top-3 left-3 flex items-center gap-2 bg-background/80 backdrop-blur-sm rounded-full px-3 py-1">
    <div className="h-2 w-2 rounded-full bg-green-500" />
    <span className="text-xs">Connected</span>
  </div>

  {/* Loading Overlay */}
  {!avatarReady && (
    <div className="absolute inset-0 flex items-center justify-center bg-background/50">
      <Loader2 className="h-8 w-8 animate-spin" />
    </div>
  )}
</div>

// HeyGen Session Management
const { avatarSession, initializeAvatar, terminateAvatar } = useAvatar()

// Stream Assignment Pattern from Reference
useEffect(() => {
  if (avatarSession?.stream && videoRef.current) {
    videoRef.current.srcObject = avatarSession.stream
    videoRef.current.onloadedmetadata = () => {
      videoRef.current?.play()
    }
  }
}, [avatarSession?.stream])

// Note: Agent C handles all input processing and avatar task sending
// Client only needs to establish session and display avatar
```

#### Mode C: VoiceVisualizerView (PLACEHOLDER)

```tsx
// Container
className="flex items-center justify-center h-full bg-gradient-to-b from-background to-muted/20"

// Placeholder for Three.js Visualizer
<div className="text-center space-y-4">
  <AudioWaveform className="h-16 w-16 mx-auto text-primary animate-pulse" />
  <div>
    <p className="text-lg font-medium">Voice Mode Active</p>
    <p className="text-sm text-muted-foreground">Visualizer integration coming soon</p>
  </div>
</div>

// Note: Three.js voice visualizer already built, will be integrated later
```

### 3. Input Component (Using Existing InputArea)

#### InputArea Reference

```tsx
// We already have a fully built InputArea component
import { InputArea } from '@agentc/realtime-ui'

// The InputArea component handles:
- Text input with auto-resize
- Voice/text mode toggle
- Send button with loading states
- Turn state management
- Audio level visualization in voice mode
- Keyboard shortcuts (Enter to send, etc.)
- OutputSelector for choosing output mode (chat/avatar/voice)

// No need to redesign - just use as-is
<InputArea />
```

**Important:** The InputArea component is already fully implemented with all necessary SDK integrations, including the OutputSelector for switching between chat, avatar, and voice output modes. We should not recreate this component or move the OutputSelector elsewhere.

### 4. Control Components

#### ConnectionButton

```tsx
// Default State
className="inline-flex items-center justify-center h-10 px-4 rounded-md bg-primary text-primary-foreground hover:bg-primary/90"

// Connected State
className="bg-secondary text-secondary-foreground hover:bg-secondary/80"

// Error State
className="bg-destructive text-destructive-foreground"

// Loading State
className="opacity-70 cursor-wait"

// SDK: Uses useConnection()
```

#### AudioControls

```tsx
// Container
className="inline-flex items-center gap-2 rounded-md border border-border bg-background p-1"

// Buttons
className="h-8 w-8 rounded hover:bg-muted"

// Audio Level Indicator
className="h-1.5 bg-primary/20 rounded-full overflow-hidden"
className="h-full bg-primary transition-all duration-100" // Width based on level

// SDK: Uses useAudio()
```

#### VoiceSelector

```tsx
// Dropdown Trigger
className="inline-flex items-center justify-between h-10 px-3 rounded-md border border-input bg-background hover:bg-muted"

// Dropdown Content
className="z-50 min-w-[200px] rounded-md border border-border bg-background shadow-md"

// Option Items
className="px-2 py-1.5 text-sm hover:bg-muted cursor-pointer"

// SDK: Uses useVoiceModel()
```

## Visual Design Specifications

### Color Mapping (Claude → CenSuite)

```css
/* Claude → CenSuite Token Mapping */
bg-bg-100 → bg-background
bg-bg-200 → bg-muted
bg-bg-300 → bg-secondary
text-text-000 → text-foreground
text-text-200 → text-muted-foreground
border-border-300 → border-border
accent-brand-200 → bg-primary
```

### Spacing System

```css
/* Consistent 4px base unit */
Micro: p-1 (4px)
Tight: p-2 (8px)
Default: p-4 (16px)
Comfortable: p-6 (24px)
Spacious: p-8 (32px)

/* Component-specific spacing */
Message gap: gap-3 (12px)
Button group gap: gap-1 (4px)
Section gap: gap-6 (24px)
```

### Typography Scale

```css
/* Headers */
Page Title: text-2xl font-semibold
Section Title: text-lg font-medium
Component Label: text-sm font-medium

/* Body Text */
Messages: text-sm leading-relaxed
UI Labels: text-sm
Captions: text-xs text-muted-foreground

/* Interactive */
Buttons: text-sm font-medium
Links: text-sm underline-offset-2
```

### Interactive States

```css
/* Hover Effects */
hover:bg-muted - Subtle background change
hover:bg-primary/90 - Primary button hover
hover:scale-[1.02] - Subtle scale for emphasis

/* Active/Pressed */
active:scale-[0.98] - Button press feedback
aria-pressed:bg-primary - Toggle state

/* Focus */
focus-visible:ring-2 focus-visible:ring-ring
focus-visible:ring-offset-2

/* Disabled */
disabled:opacity-50 disabled:pointer-events-none
```

## Components Implementation Phases

### Phase 1: Core Components (Build Now)

✅ **Ready to implement with current SDK:**

- `ChatLayout` - Main container structure
- `ChatSidebar` with three sub-components:
  - `SidebarTopMenu` - New Chat button (functional)
  - `ChatSessionList` - Stub with placeholder
  - `UserDisplay` - User info and logout (functional)
- `ChatHeader` with `SessionNameDropdown` - Session name display (stub)
- `MainContentArea` - Container for display modes
- `AvatarDisplayView` - HeyGen avatar display
- `ConnectionButton` - Uses useConnection()
- `AudioControls` - Uses useAudio()

**Already Built (Don't recreate):**

- `InputArea` - Existing component handles all input

### Phase 2: Enhanced Components

⚡ **Now Ready for Implementation:**

- `ChatSessionList` - Full design complete with API integration
- `ChatMessagesView` - Placeholder for complex chat UI
- `VoiceVisualizerView` - Placeholder for Three.js visualizer

### Phase 3: Future Enhancements (Not in scope)

🔮 **Coming later:**

- Full chat message rendering with artifacts
- Three.js voice visualizer integration
- Conversation history API integration
- Settings panel
- Advanced user preferences

## Responsive Design Considerations

### Mobile Optimizations

```css
/* Touch Targets */
min-height: 44px for all interactive elements

/* Sidebar Behavior */
Mobile: Full-screen overlay with backdrop
Desktop: Sticky sidebar always visible

/* Input Area */
Mobile: Fixed bottom with safe-area-inset-bottom
Desktop: Static positioning

/* Message Width */
Mobile: Full width minus padding
Desktop: max-w-[75ch] for readability
```

### Breakpoint-Specific Classes

```css
/* Sidebar Toggle */
className="lg:hidden" // Menu button
className="hidden lg:flex" // Desktop nav

/* Spacing Adjustments */
className="px-4 md:px-6 lg:px-8"
className="py-4 lg:py-6"

/* Layout Changes */
className="flex-col lg:flex-row"
className="w-full lg:w-72"
```

## Accessibility Requirements

### Focus Management

- Trap focus in modals/overlays
- Visible focus rings on all interactive elements
- Logical tab order throughout interface
- Skip links for keyboard navigation

### ARIA Implementation

```tsx
// Live Regions
aria-live="polite" // Status updates
aria-live="assertive" // Errors

// States
aria-expanded={isOpen}
aria-pressed={isActive}
aria-disabled={isDisabled}
aria-current="page" // Navigation

// Relationships
aria-labelledby={titleId}
aria-describedby={descriptionId}
```

### Screen Reader Support

- Semantic HTML structure
- Descriptive button labels
- Status announcements for connection changes
- Message sender identification

## Performance Considerations

### Component Optimization

```tsx
// Memoization for expensive renders
const MessageList = React.memo(({ messages }) => {
  // Virtualize long message lists
})

// Debounced inputs
const debouncedSend = useMemo(
  () => debounce(sendMessage, 300),
  [sendMessage]
)
```

### Bundle Size Management

- Lazy load heavy components (Avatar view)
- Code split settings/preferences
- Tree-shake unused icon imports

## Implementation Notes

### File Structure

```
packages/realtime-ui/src/
├── components/
│   ├── layout/
│   │   ├── ChatLayout.tsx
│   │   ├── ChatHeader.tsx
│   │   └── SessionNameDropdown.tsx (stub)
│   ├── sidebar/
│   │   ├── ChatSidebar.tsx
│   │   ├── SidebarTopMenu.tsx
│   │   ├── ChatSessionList.tsx (stub)
│   │   └── UserDisplay.tsx
│   ├── content/
│   │   ├── MainContentArea.tsx
│   │   ├── ChatMessagesView.tsx (placeholder)
│   │   ├── AvatarDisplayView.tsx
│   │   └── VoiceVisualizerView.tsx (placeholder)
│   ├── controls/
│   │   ├── ConnectionButton.tsx
│   │   └── AudioControls.tsx
│   └── input/
│       └── (Using existing InputArea component)
```

### Testing Approach

1. Component renders without errors
2. Accessibility audit passes
3. Responsive behavior verified
4. SDK hook integration tested
5. Build succeeds with no type errors

## Next Steps

1. **Immediate Actions:**
   
   - Create base `ChatLayout` component
   - Build `ChatSidebar` with three sub-components
   - Implement `SessionNameDropdown` stub component
   - Create `MainContentArea` container
   - Build `AvatarDisplayView` with HeyGen integration
   - Add placeholder components for chat and voice modes
   - Integrate existing `InputArea` component

2. **SDK Integration Points:**
   
   - `useConnection()` for connection status
   - `useAvatar()` for HeyGen session management  
   - `useAudio()` for audio controls
   - `useAuth()` for user information
   - Existing InputArea handles all input SDK hooks including OutputSelector

3. **Important Notes:**
   
   - DO NOT recreate the InputArea component
   - Chat message design will be done separately (complex)
   - Voice visualizer exists, integration coming later
   - Focus on layout structure and mode switching

## Conclusion

This revised design provides a flexible foundation for the Agent C Realtime interface that supports three distinct output modes while leveraging our existing InputArea component. The sidebar has been completely redesigned following Claude's patterns with clear separation between navigation, session management, and user controls.

Key improvements in this revision:

- Clear separation of what we build now vs stub for later
- Flexibility for three output modes (chat, avatar, voice)
- Proper use of existing components (InputArea)
- Realistic scope for immediate implementation
- Following established patterns from HeyGen demo and Claude's sidebar

The phased approach ensures we deliver core functionality quickly while maintaining flexibility for future enhancements. All components are designed with accessibility and responsiveness as primary concerns, ensuring a great experience across all devices and user capabilities.