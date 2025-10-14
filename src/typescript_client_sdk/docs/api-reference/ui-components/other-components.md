# Other UI Components

This document provides comprehensive API reference for additional UI component categories in `@agentc/realtime-ui` including layout, editor, input, session, sidebar, and base UI components.

## Table of Contents

- [Layout Components](#layout-components)
- [Editor Components](#editor-components)
- [Input Components](#input-components)
- [Session Components](#session-components)
- [Sidebar Components](#sidebar-components)
- [Base UI Components](#base-ui-components)

---

## Layout Components

### MainContentArea

**Purpose**: Container component that switches between different display modes (chat, avatar, voice) based on the selected output mode.

**Category**: Layout

**Props Interface**:

```typescript
export type OutputMode = 'chat' | 'avatar' | 'voice'

export interface MainContentAreaProps {
  outputMode: OutputMode        // Current display mode
  className?: string            // Additional CSS classes
}
```

**Usage Example**:

```tsx
import { MainContentArea } from '@agentc/realtime-ui'

function App() {
  const [mode, setMode] = useState<OutputMode>('chat')
  
  return (
    <MainContentArea 
      outputMode={mode}
      className="h-screen"
    />
  )
}
```

**CenSuite Design Compliance**:
- ✅ Uses semantic layout structure
- ✅ Proper spacing with Tailwind classes
- ✅ Responsive height management
- ✅ Clear visual hierarchy

**Accessibility Features**:
- Semantic HTML structure
- Proper ref forwarding for focus management
- Clear component boundaries

**Integration Patterns**:
- Integrates with ChatMessagesView for chat mode
- Integrates with AvatarDisplayView for avatar mode
- Integrates with VoiceVisualizerView for voice mode
- Manages view switching seamlessly

---

## Editor Components

### MarkdownEditor

**Purpose**: TipTap-based rich text editor with markdown support, syntax highlighting, and smart paste functionality.

**Category**: Editor

**Props Interface**:

```typescript
export interface MarkdownEditorProps {
  value?: string                   // Current editor content
  onChange?: (value: string) => void  // Content change callback
  placeholder?: string             // Placeholder text
  onSubmit?: (value: string) => void  // Submit handler (Cmd/Ctrl+Enter)
  disabled?: boolean               // Disable editing
  className?: string               // Additional CSS classes
  enableSmartPaste?: boolean       // Enable smart paste extension
  maxImageSize?: number            // Max image size in bytes
  onImageUpload?: (file: File) => Promise<string>  // Image upload handler
  onImageUploadStart?: () => void    // Upload start callback
  onImageUploadComplete?: () => void // Upload complete callback
  onImageUploadError?: (error: Error) => void // Upload error callback
  onKeyDown?: (event: KeyboardEvent) => boolean // Custom key handler
}
```

**Usage Example**:

```tsx
import { MarkdownEditor } from '@agentc/realtime-ui'

function EditorExample() {
  const [content, setContent] = useState('')
  
  const handleSubmit = (text: string) => {
    console.log('Submitted:', text)
    // Send message or process content
  }
  
  return (
    <MarkdownEditor
      value={content}
      onChange={setContent}
      onSubmit={handleSubmit}
      placeholder="Write your message..."
      enableSmartPaste={true}
    />
  )
}
```

**CenSuite Design Compliance**:
- ✅ Consistent border and focus states
- ✅ Proper typography with prose classes
- ✅ Semantic color usage
- ✅ Responsive padding

**Accessibility Features**:
- Full keyboard navigation support
- Customizable keyboard shortcuts
- Focus visible states
- Screen reader compatible

**Integration Patterns**:
- Uses TipTap v2 for Next.js compatibility
- Supports markdown extensions
- Code syntax highlighting
- Smart paste for rich content
- SSR-safe with immediatelyRender: false

### MarkdownEditorClient

**Purpose**: Client-side wrapper for MarkdownEditor to ensure proper hydration in SSR environments.

**Category**: Editor

---

## Input Components

### InputArea

**Purpose**: Main input area component combining rich text editor, microphone button, and toolbar controls.

**Category**: Input

**Props Interface**:

```typescript
export interface InputAreaProps {
  className?: string              // Additional CSS classes
  maxHeight?: string              // Max height for input area
  placeholder?: string            // Input placeholder text
  agents?: Agent[]                // Available agents
  voiceOptions?: OutputOption[]  // Voice model options
  avatarOptions?: OutputOption[] // Avatar options
  onAgentChange?: (agent: Agent) => void // Agent selection callback
  onOutputModeChange?: (mode: OutputMode, option?: OutputOption) => void
  compact?: boolean               // Compact mode
  disabled?: boolean              // Disable all input
  orientation?: 'horizontal' | 'vertical' // Layout orientation
}
```

**Usage Example**:

```tsx
import { InputArea } from '@agentc/realtime-ui'

function ChatInput() {
  return (
    <InputArea
      maxHeight="200px"
      placeholder="Type or speak your message..."
      onAgentChange={(agent) => console.log('Selected agent:', agent)}
      orientation="horizontal"
    />
  )
}
```

**CenSuite Design Compliance**:
- ✅ Consistent spacing and padding
- ✅ Error state visualization with Alert component
- ✅ Proper button states and transitions
- ✅ Semantic color usage for status

**Accessibility Features**:
- Keyboard accessible controls
- Screen reader announcements for errors
- Focus management between text and voice input
- Disabled state handling

**Integration Patterns**:
- Integrates with SDK hooks (useChat, useAudio, useTurnState)
- Manages recording state automatically
- Handles turn-based interaction
- Error recovery and display

### InputContainer

**Purpose**: Container component for input area with configurable max height and scrolling.

**Category**: Input

**Props Interface**:

```typescript
export interface InputContainerProps {
  children: React.ReactNode
  maxHeight?: string
  className?: string
}
```

### InputToolbar

**Purpose**: Toolbar with send button, microphone control, and agent/output selectors.

**Category**: Input

**Props Interface**:

```typescript
export interface InputToolbarProps {
  onSend: () => void
  canSend: boolean
  isRecording: boolean
  onStartRecording: () => void
  onStopRecording: () => void
  audioLevel?: number
  agents?: Agent[]
  selectedAgent?: Agent
  onAgentChange?: (agent: Agent) => void
}
```

### MicrophoneButton

**Purpose**: Microphone button with recording state visualization and audio level indicator.

**Category**: Input

**Props Interface**:

```typescript
export interface MicrophoneButtonProps {
  isRecording: boolean
  onStartRecording: () => void
  onStopRecording: () => void
  audioLevel?: number
  disabled?: boolean
  className?: string
}
```

### RichTextEditor

**Purpose**: Simple rich text editor component with basic formatting support.

**Category**: Input

**Props Interface**:

```typescript
export interface RichTextEditorProps {
  value: string
  onChange: (value: string) => void
  onSubmit: () => void
  disabled?: boolean
  placeholder?: string
  className?: string
}
```

---

## Session Components

### ChatSessionList

**Purpose**: Virtual scrolling list of chat sessions with search, grouping, and management features.

**Category**: Session

**Props Interface**:

```typescript
export interface ChatSessionListProps extends React.HTMLAttributes<HTMLDivElement> {
  isCollapsed?: boolean           // Whether sidebar is collapsed
  className?: string              // Additional CSS classes
  onSessionSelect?: (sessionId: string) => void // Session selection callback
  autoLoad?: boolean              // Auto-load sessions on mount
}
```

**Usage Example**:

```tsx
import { ChatSessionList } from '@agentc/realtime-ui'

function SessionManager() {
  const handleSessionSelect = (sessionId: string) => {
    console.log('Selected session:', sessionId)
    // Load session messages
  }
  
  return (
    <ChatSessionList
      onSessionSelect={handleSessionSelect}
      autoLoad={true}
    />
  )
}
```

**CenSuite Design Compliance**:
- ✅ Virtual scrolling for performance
- ✅ Consistent spacing and padding
- ✅ Proper loading and error states
- ✅ Search functionality with debouncing

**Accessibility Features**:
- Full keyboard navigation (arrows, Home, End, Enter, Delete)
- Screen reader announcements
- ARIA roles and properties
- Focus management in virtual list
- Tooltip support for collapsed view

**Integration Patterns**:
- Uses @tanstack/react-virtual for efficient rendering
- Integrates with useChatSessionList hook
- Supports infinite scroll pagination
- Delete confirmation with dialog
- Search with debouncing

### SessionNameDropdown

**Purpose**: Dropdown component for selecting and managing session names.

**Category**: Session

**Props Interface**:

```typescript
export interface SessionNameDropdownProps {
  currentSessionName?: string
  sessions: ChatSessionIndexEntry[]
  onSessionSelect: (sessionId: string) => void
  className?: string
}
```

---

## Sidebar Components

### ChatSidebar

**Purpose**: Main sidebar container with responsive behavior - desktop sticky sidebar and mobile overlay.

**Category**: Sidebar

**Props Interface**:

```typescript
export interface ChatSidebarProps {
  isOpen?: boolean               // Whether sidebar is open (mobile)
  isCollapsed?: boolean          // Whether sidebar is collapsed (desktop)
  onClose?: () => void          // Close handler (mobile)
  onToggleCollapse?: () => void // Collapse toggle handler (desktop)
  onLogout?: () => void         // Logout callback
  className?: string            // Additional CSS classes
}
```

**Usage Example**:

```tsx
import { ChatSidebar } from '@agentc/realtime-ui'

function Layout() {
  const [sidebarOpen, setSidebarOpen] = useState(false)
  const [sidebarCollapsed, setSidebarCollapsed] = useState(false)
  
  return (
    <div className="flex">
      <ChatSidebar
        isOpen={sidebarOpen}
        isCollapsed={sidebarCollapsed}
        onClose={() => setSidebarOpen(false)}
        onToggleCollapse={() => setSidebarCollapsed(!sidebarCollapsed)}
        onLogout={() => console.log('Logout')}
      />
      <main className="flex-1">
        {/* Main content */}
      </main>
    </div>
  )
}
```

**CenSuite Design Compliance**:
- ✅ Responsive design (mobile overlay, desktop sticky)
- ✅ Smooth transitions and animations
- ✅ Consistent border and spacing
- ✅ Proper visual hierarchy

**Accessibility Features**:
- Keyboard accessible controls
- Proper ARIA labels
- Focus management
- Screen reader support

**Integration Patterns**:
- Responsive behavior with viewport detection
- Integrates SidebarTopMenu, ChatSessionList, UserDisplay
- Mobile backdrop for overlay
- Desktop collapse/expand functionality

### SidebarTopMenu

**Purpose**: Top menu section of sidebar with navigation options.

**Category**: Sidebar

**Props Interface**:

```typescript
export interface SidebarTopMenuProps {
  isCollapsed?: boolean
  className?: string
}
```

### UserDisplay

**Purpose**: User profile display with logout option in sidebar.

**Category**: Sidebar

**Props Interface**:

```typescript
export interface UserDisplayProps {
  isCollapsed?: boolean
  onLogout?: () => void
  className?: string
}
```

---

## Base UI Components

The `ui/` directory contains customized shadcn/ui base components that follow CenSuite design system guidelines. These are foundational components used throughout the SDK.

### Core Components

#### Alert
- **Purpose**: Display important messages with different severity levels
- **Variants**: default, destructive
- **CenSuite Compliance**: Uses semantic colors for status indication

#### Button
- **Purpose**: Interactive button with multiple variants and sizes
- **Variants**: default, destructive, outline, secondary, ghost, link
- **Sizes**: default (h-10 px-4 py-2), sm (h-9 px-3), lg (h-11 px-8), icon (h-10 w-10)
- **CenSuite Compliance**: Exact sizing per design system, focus states, semantic colors

#### Card
- **Purpose**: Container component for grouped content
- **Components**: Card, CardHeader, CardFooter, CardTitle, CardDescription, CardContent
- **CenSuite Compliance**: Consistent border, spacing, and shadows

#### Dropdown Menu
- **Purpose**: Contextual menu with keyboard navigation
- **Components**: DropdownMenu, DropdownMenuTrigger, DropdownMenuContent, DropdownMenuItem, etc.
- **Accessibility**: Full keyboard support, ARIA compliant

#### Form
- **Purpose**: Form components with react-hook-form integration
- **Components**: Form, FormItem, FormLabel, FormControl, FormDescription, FormMessage
- **CenSuite Compliance**: Consistent spacing, error states, validation messages

#### Input
- **Purpose**: Text input field with consistent styling
- **CenSuite Compliance**: Border colors, focus states, disabled states

#### Select
- **Purpose**: Dropdown selection component
- **Components**: Select, SelectTrigger, SelectContent, SelectItem, SelectValue
- **Accessibility**: Keyboard navigation, screen reader support

#### Tabs
- **Purpose**: Tabbed interface component
- **Components**: Tabs, TabsList, TabsTrigger, TabsContent
- **Accessibility**: ARIA roles, keyboard navigation

#### Tooltip
- **Purpose**: Contextual information on hover
- **Components**: Tooltip, TooltipTrigger, TooltipContent, TooltipProvider
- **Accessibility**: Screen reader support, keyboard accessible

### Utility Components

#### Avatar
- Standard avatar display with fallback

#### Badge
- Status and label badges with variants

#### Separator
- Visual divider component

#### Skeleton
- Loading placeholder animations

#### Slider
- Range input component

#### Toggle Group
- Group of toggle buttons

### Design System Compliance

All base UI components follow these CenSuite principles:

1. **Color System**: HSL-based semantic tokens only
2. **Spacing**: 4px base unit system (p-2, p-4, p-6, p-8)
3. **Typography**: Consistent font sizes and weights
4. **Focus States**: Visible ring with 2px offset
5. **Transitions**: Smooth, consistent animations
6. **Accessibility**: WCAG 2.1 AA compliant

### Integration Notes

- All components use `cn()` utility for className merging
- Components support ref forwarding with React.forwardRef
- Proper displayName for debugging
- TypeScript types exported for all props
- Consistent variant patterns using CVA (class-variance-authority)

---

## Common Patterns

### Error Handling
All components follow consistent error handling:
- Display errors using Alert components
- Auto-dismiss errors after 5 seconds
- Provide retry mechanisms where appropriate
- Log errors to console in development

### Loading States
Consistent loading indicators:
- Skeleton components for initial loads
- Spinner (Loader2) for active operations
- Disabled states during async operations
- Loading text for screen readers

### Keyboard Navigation
Full keyboard support across all components:
- Tab navigation through interactive elements
- Arrow keys for list navigation
- Enter/Space for activation
- Escape for dismissal
- Delete for removal actions

### SDK Integration
Components integrate with @agentc/realtime-react hooks:
- useChat for message management
- useAudio for voice interaction
- useTurnState for conversation flow
- useVoiceModel for voice selection
- useAvatar for avatar control
- useChatSessionList for session management

### Responsive Design
All components support responsive behavior:
- Mobile-first approach
- Breakpoint-aware layouts
- Touch-friendly interactions
- Appropriate sizing for different viewports

---

## Testing Considerations

When testing these components:

1. **Unit Tests**: Test individual component behavior, props, and events
2. **Integration Tests**: Test SDK hook integration
3. **Accessibility Tests**: Verify ARIA attributes and keyboard navigation
4. **Visual Tests**: Ensure CenSuite design compliance
5. **Performance Tests**: Validate virtual scrolling efficiency

---

## Migration Guide

If migrating from previous versions:

1. Update imports to use `@agentc/realtime-ui`
2. Review prop changes in component interfaces
3. Update event handlers to match new signatures
4. Ensure SDK context providers are properly configured
5. Test keyboard navigation and accessibility features

---

This completes the documentation for the remaining UI component categories in @agentc/realtime-ui.