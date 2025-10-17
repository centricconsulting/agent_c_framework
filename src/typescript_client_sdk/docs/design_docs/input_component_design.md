# Input Component Design Specification

## Overview

This document specifies the design for a rich text input component that matches Claude's input area layout while integrating with the Agent C Realtime SDK. The component supports simultaneous text and voice input (not modes - users can always type OR speak), markdown rendering, and provides a comprehensive toolbar for agent output control and enhanced functionality.

## Newly installed packages for this:

+ @radix-ui/react-avatar ^1.1.10
+ @radix-ui/react-dialog ^1.1.15
+ @radix-ui/react-dropdown-menu ^2.1.16
+ @radix-ui/react-tooltip ^1.2.8

## Critical: How Voice Actually Works in Our System

### Voice Input Flow (User Speaking)

1. **User clicks microphone button** - Calls `startStreaming()` from useAudio hook
2. **Audio streams to server** - PCM16 audio sent via WebSocket in real-time
3. **Server processes audio** - Server performs VAD (Voice Activity Detection)
4. **Server transcribes** - When user stops speaking, server transcribes the audio
5. **Transcription returns as message** - Server sends transcription as a regular user message
6. **Message appears in chat** - Via the messages array from useChat hook

**KEY POINTS:**

- NO client-side voice events or transcription
- NO "voice mode" - microphone button just starts/stops recording
- User can ALWAYS type in the text editor, even while recording
- Transcriptions appear as messages in chat, NOT in the input field

### Agent Output Modes (How Agent Responds)

The OutputSelector controls how the AGENT responds, NOT how users input:

- **Text Only** - Agent responds with text, no audio (voice = 'none')
- **Voice** - Agent responds with voice audio (PCM16 streaming)
- **Avatar** - Agent responds with HeyGen avatar video (voice = 'avatar')

### Turn Management

The system uses server-controlled turns to prevent talk-over:

- `useTurnState()` hook provides current turn state
- Audio automatically stops streaming when user loses turn
- Input controls respect turn state (disabled when not user's turn)

## Component Architecture

### 1. Component Hierarchy

```
InputArea/
├── InputContainer           // Main wrapper with flex layout
│   ├── EditorWrapper        // Relative container with scroll
│   │   └── RichTextEditor   // ProseMirror/Tiptap editor (always available for typing)
│   └── InputToolbar         // Bottom toolbar row
│       ├── ToolbarLeft      // Left-aligned tools
│       │   ├── AttachmentsButton
│       │   ├── ToolsButton (placeholder)
│       │   └── OutputSelector    // Controls AGENT response mode: Text/Voice/Avatar
│       ├── ToolbarCenter    // Center section
│       │   ├── MicrophoneButton  // Start/stop audio recording (NOT a mode toggle)
│       │   └── StatusIndicator   // Shows recording/turn state
│       ├── ToolbarRight     // Right-aligned controls
│       │   ├── AgentSelector    // Agent selection with cards
│       │   └── SendButton
```

### 2. Core Components

#### InputContainer

**Purpose:** Main wrapper that provides structure and spacing

#### RichTextEditor

**Purpose:** Core editor component using Tiptap (ProseMirror wrapper)

**Features:**

- Contenteditable with rich text support
- Placeholder text handling
- Markdown shortcuts
- Voice transcription integration

#### InputToolbar

**Purpose:** Action toolbar with microphone control, agent output settings, and send functionality

```
**Implementation Example:**

```typescript
const InputToolbar: React.FC<InputToolbarProps> = ({
  onSend,
  canSend,
  isRecording,
  onStartRecording,
  onStopRecording,
  audioLevel,
  onAttachment,
  selectedAgent,
  onAgentChange,
  outputMode,
  onOutputModeChange,
  ...props
}) => {
  return (
    <div className="flex items-center gap-2.5 w-full px-4 py-2 border-t border-border">
      {/* Left side tools */}
      <div className="flex items-center gap-1">
        {/* Attachments button */}
        <Button
          variant="ghost"
          size="icon"
          onClick={onAttachment}
          className="h-8 w-8"
          aria-label="Add attachment"
        >
          <Paperclip className="h-4 w-4" />
        </Button>

        {/* Tools button (placeholder) */}
        <ToolButton disabled={true} />

        {/* Output selector - Controls how AGENT responds */}
        <OutputSelector
          selectedOption={selectedOutputOption}
          onOptionSelect={handleOutputSelect}
          voiceOptions={voiceOptions}
          avatarOptions={avatarOptions}
        />
      </div>

      {/* Center section - Microphone button (NOT a mode toggle) */}
      <div className="flex-1 flex justify-center items-center gap-2">
        <Button
          variant={isRecording ? "destructive" : "ghost"}
          size="sm"
          onClick={isRecording ? onStopRecording : onStartRecording}
          className="gap-2"
        >
          <Mic2 className={cn(
            "h-4 w-4",
            isRecording && "animate-pulse"
          )} />
          {isRecording ? "Stop Recording" : "Start Recording"}
        </Button>

        {/* Audio level indicator when recording */}
        {isRecording && audioLevel !== undefined && (
          <div className="flex gap-0.5">
            {[...Array(5)].map((_, i) => (
              <div
                key={i}
                className={cn(
                  "w-1 h-3 bg-muted rounded-full transition-all",
                  audioLevel > (i * 20) && "bg-primary"
                )}
              />
            ))}
          </div>
        )}
      </div>

      {/* Right side controls */}
      <div className="flex items-center gap-2">
        {/* Agent selector */}
        <AgentSelector
          agents={availableAgents}
          selectedAgent={selectedAgent}
          onAgentSelect={onAgentChange}
          variant="dropdown"
        />

        {/* Send button - for text messages only */}
        <Button
          size="icon"
          onClick={onSend}
          disabled={!canSend}
          className="h-8 w-8"
          aria-label="Send message"
        >
          <Send className="h-4 w-4" />
        </Button>
      </div>
    </div>
  )
}
```

## State Management

### Local State (Component Level)

```typescript
interface InputAreaState {
  // Editor state
  content: string           // Current text in editor
  isEditing: boolean
  isFocused: boolean

  // Agent output preferences (NOT input mode)
  outputMode: 'text' | 'voice' | 'avatar'  // How AGENT responds

  // UI state
  showPlaceholder: boolean
  showToolsMenu: boolean  // Placeholder for future
  showAttachmentsMenu: boolean
  showAgentSelector: boolean
  showOutputSelector: boolean

  // Selection state
  selectedAgent: Agent | null
  selectedOutputOption: OutputOption | null

  // Validation
  canSubmit: boolean
  characterCount: number
  wordCount: number
}

// Agent interface
interface Agent {
  id: string
  name: string
  description: string
  avatar?: string
  tools: AgentTool[]
  capabilities?: string[]
}

interface AgentTool {
  id: string
  name: string
  icon?: string
  category?: string
}

// Output selection types - for AGENT responses
type OutputMode = 'text' | 'voice' | 'avatar'

interface OutputOption {
  id: string
  name: string
  type: OutputMode
  available: boolean
  metadata?: {
    voiceId?: string      // For voice mode
    avatarId?: string     // For avatar mode
  }
}
```

### SDK Integration State

```typescript
// From useChat hook - handles text messages and chat history
const {
  messages,           // Chat history including transcribed voice messages
  sendMessage,        // Send text message
  currentResponse,    // Current agent response being streamed
  isAgentTyping      // Agent is currently responding
} = useChat()

// From useAudio hook - handles microphone and audio streaming
const {
  isRecording,        // Currently recording audio
  isStreaming,        // Audio is being sent to server
  startStreaming,     // Start recording and streaming audio
  stopStreaming,      // Stop recording and streaming
  audioLevel,         // Current audio input level (0-100)
  status             // Detailed audio status
} = useAudio({
  respectTurnState: true  // Automatically stop when losing turn
})

// From useTurnState hook - server-controlled turn management
const {
  turnState,          // Current turn state from server
  isUserTurn,         // User currently has the turn
  isAgentTurn,        // Agent currently has the turn
  canSendInput       // Whether user can send input now
} = useTurnState()

// From useVoiceModel hook - controls agent voice output
const {
  voices,             // Available voice models
  currentVoice,       // Currently selected voice
  setVoice           // Change agent voice
} = useVoiceModel()

// From useAvatar hook - controls avatar sessions
const {
  avatars,            // Available avatars
  currentAvatar,      // Active avatar session
  startAvatarSession, // Start avatar session
  endAvatarSession   // End avatar session
} = useAvatar()
```

## Event Handling Patterns

### 1. Text Input Events

```typescript
// Keyboard shortcuts
const handleKeyDown = (event: KeyboardEvent) => {
  // Submit on Enter (without Shift)
  if (event.key === 'Enter' && !event.shiftKey) {
    event.preventDefault()
    handleSubmitText()
  }

  // Markdown shortcuts
  if (event.metaKey || event.ctrlKey) {
    switch (event.key) {
      case 'b': toggleBold()
      case 'i': toggleItalic()
      case 'k': insertLink()
      case 'e': toggleCode()
    }
  }
}

// Content changes
const handleContentChange = (newContent: string) => {
  setContent(newContent)
  updateCharacterCount(newContent)
  validateSubmitState(newContent)
}
```

### 2. Voice Input Flow (WebSocket-based)

```typescript
// Start recording - uses useAudio hook
const handleStartRecording = async () => {
  try {
    // Check turn state
    if (!canSendInput) {
      console.warn('Cannot start recording - not user turn')
      return
    }

    // Start audio streaming to server
    await startStreaming()  // From useAudio hook

    // Audio is now streaming as PCM16 via WebSocket
    // Server will:
    // 1. Perform VAD (Voice Activity Detection)
    // 2. Transcribe when user stops speaking
    // 3. Send transcription back as a user message
    // 4. Message appears in chat via 'message' event
  } catch (error) {
    console.error('Failed to start recording:', error)
    // Show error to user
  }
}

// Stop recording
const handleStopRecording = async () => {
  try {
    await stopStreaming()  // From useAudio hook
    // Server will finalize transcription and send as message
  } catch (error) {
    console.error('Failed to stop recording:', error)
  }
}

// NO client-side transcription events!
// Transcriptions come back as regular messages from server
// They appear in the chat history via useChat hook
```

### 3. Submit Events (Text Only)

```typescript
const handleSubmitText = async () => {
  if (!canSubmit || !content.trim()) return

  try {
    // Check turn state
    if (!canSendInput) {
      console.warn('Cannot send - not user turn')
      return
    }

    // Stop recording if active (user typed while recording)
    if (isRecording) {
      await stopStreaming()
    }

    // Send text message via SDK
    await sendMessage(content)  // From useChat hook

    // Clear editor
    editor?.commands.clearContent()
    setContent('')

  } catch (error) {
    console.error('Failed to send message:', error)
    // Handle error state
  }
}
```

### 4. Agent Output Mode Changes

```typescript
// Handle output mode selection (how AGENT responds)
const handleOutputModeChange = async (mode: OutputMode, option?: OutputOption) => {
  setOutputMode(mode)
  setSelectedOutputOption(option || null)

  switch (mode) {
    case 'text':
      // Agent will respond with text only
      setVoice('none')  // Special voice mode for text-only
      if (currentAvatar) {
        await endAvatarSession()
      }
      break

    case 'voice':
      // Agent will respond with voice
      if (option?.metadata?.voiceId) {
        setVoice(option.metadata.voiceId)
      }
      if (currentAvatar) {
        await endAvatarSession()
      }
      break

    case 'avatar':
      // Agent will respond with avatar video
      setVoice('avatar')  // Special voice mode for avatar
      if (option?.metadata?.avatarId) {
        await startAvatarSession({
          avatarId: option.metadata.avatarId
        })
      }
      break
  }
}
```

## CenSuite Styling Approach

### 1. Base Styles

```typescript
// Container styles
const containerStyles = cn(
  "flex flex-col gap-3.5",
  "bg-background rounded-lg",
  "border border-border",
  "transition-all duration-200",
  "focus-within:ring-2 focus-within:ring-ring focus-within:ring-offset-2"
)

// Editor wrapper styles
const editorWrapperStyles = cn(
  "relative w-full",
  "max-h-96 overflow-y-auto",
  "min-h-[3rem]",
  "px-4 py-3"
)

// Toolbar styles
const toolbarStyles = cn(
  "flex items-center gap-2.5 w-full",
  "px-4 py-2",
  "border-t border-border"
)
```

### 2. Button Variants

```typescript
// Tool buttons (attachments, tools placeholder, output selector)
const toolButtonVariants = cva(
  "inline-flex items-center justify-center rounded-lg transition-all",
  {
    variants: {
      variant: {
        default: "text-muted-foreground hover:text-foreground hover:bg-muted",
        active: "text-primary bg-primary/10 hover:bg-primary/20",
        disabled: "text-muted-foreground/50 cursor-not-allowed"  // For placeholder
      },
      size: {
        default: "h-8 min-w-8 px-2",
        icon: "h-8 w-8"
      }
    }
  }
)

// Send button
const sendButtonStyles = cn(
  "h-8 w-8 rounded-lg",
  "bg-primary text-primary-foreground",
  "hover:bg-primary/90",
  "active:scale-95",
  "disabled:opacity-50 disabled:cursor-not-allowed",
  "transition-all duration-150"
)
```

### 3. Focus States

```typescript
// Editor focus styles
".ProseMirror:focus": {
  outline: "none"
}

// Container focus-within styles
"focus-within:ring-2 focus-within:ring-ring focus-within:ring-offset-2"

// Button focus styles
"focus-visible:ring-2 focus-visible:ring-ring focus-visible:ring-offset-2"
```

## Integration Points

### 1. SDK Hook Integration

```typescript
const InputArea: React.FC = () => {
  // SDK hooks - properly integrated with our real architecture
  const { messages, sendMessage, isAgentTyping } = useChat()
  const { 
    isStreaming, 
    startStreaming, 
    stopStreaming, 
    audioLevel,
    status 
  } = useAudio({ respectTurnState: true })
  const { turnState, isUserTurn, canSendInput } = useTurnState()
  const { voices, currentVoice, setVoice } = useVoiceModel()
  const { 
    avatars,
    currentAvatar,
    startAvatarSession,
    endAvatarSession 
  } = useAvatar()

  // Local state
  const [content, setContent] = useState('')
  const [selectedAgent, setSelectedAgent] = useState<Agent | null>(null)
  const [outputMode, setOutputMode] = useState<OutputMode>('text')
  const [selectedOutputOption, setSelectedOutputOption] = useState<OutputOption | null>(null)

  // Disable text input when not user's turn or agent is typing
  const isTextDisabled = !canSendInput || isAgentTyping

  // Can submit text if we have content and it's user's turn
  const canSubmitText = content.trim().length > 0 && canSendInput

  // Handle microphone button click
  const handleMicrophoneClick = async () => {
    if (isStreaming) {
      await stopStreaming()
    } else {
      // Can only start if it's user's turn
      if (canSendInput) {
        await startStreaming()
      }
    }
  }

  // Handle text submission
  const handleSendText = async () => {
    if (!canSubmitText) return

    // Stop audio if recording
    if (isStreaming) {
      await stopStreaming()
    }

    // Send text message
    await sendMessage(content)
    setContent('')
  }

  // Handle output mode change (how AGENT responds)
  const handleOutputModeChange = async (mode: OutputMode, option?: OutputOption) => {
    setOutputMode(mode)
    setSelectedOutputOption(option || null)

    switch (mode) {
      case 'text':
        setVoice('none')  // Special voice for text-only
        if (currentAvatar) {
          await endAvatarSession()
        }
        break

      case 'voice':
        if (option?.metadata?.voiceId) {
          setVoice(option.metadata.voiceId)
        }
        if (currentAvatar) {
          await endAvatarSession()
        }
        break

      case 'avatar':
        setVoice('avatar')  // Special voice for avatar mode
        if (option?.metadata?.avatarId) {
          await startAvatarSession({
            avatarId: option.metadata.avatarId
          })
        }
        break
    }
  }

  return (
    <InputContainer>
      <RichTextEditor
        value={content}
        onChange={setContent}
        onSubmit={handleSendText}
        disabled={isTextDisabled}
        placeholder="Type a message or click the microphone to speak..."
      />
      <InputToolbar
        onSend={handleSendText}
        canSend={canSubmitText}
        isRecording={isStreaming}
        onStartRecording={() => startStreaming()}
        onStopRecording={() => stopStreaming()}
        audioLevel={audioLevel}
        selectedAgent={selectedAgent}
        onAgentChange={setSelectedAgent}
        outputMode={outputMode}
        onOutputModeChange={handleOutputModeChange}
      />
    </InputContainer>
  )
}
```

### 2. Tiptap Editor Configuration

```typescript
import { useEditor } from '@tiptap/react'
import StarterKit from '@tiptap/starter-kit'
import Placeholder from '@tiptap/extension-placeholder'
import CharacterCount from '@tiptap/extension-character-count'

const editor = useEditor({
  extensions: [
    StarterKit.configure({
      heading: false, // Disable headings in chat
      codeBlock: false, // Use inline code only
    }),
    Placeholder.configure({
      placeholder: 'How can I help you today?',
      showOnlyWhenEditable: true,
      emptyEditorClass: 'is-editor-empty',
    }),
    CharacterCount.configure({
      limit: 10000, // Character limit
    }),
  ],
  content: '',
  editorProps: {
    attributes: {
      class: cn(
        'prose prose-sm max-w-none',
        'focus:outline-none',
        'min-h-[3rem]',
        '[&_p]:my-1',
        '[&_ul]:my-2 [&_ol]:my-2',
        '[&_li]:my-0.5'
      ),
    },
  },
  onUpdate: ({ editor }) => {
    const text = editor.getText()
    handleContentChange(text)
  },
})
```

### 3. Voice Message Flow

```typescript
// IMPORTANT: Voice transcriptions come back as messages from the server
// There is NO client-side transcription or voice events

// The actual flow:
// 1. User clicks microphone button -> startStreaming()
// 2. Audio streams to server as PCM16 via WebSocket
// 3. Server performs VAD (Voice Activity Detection)
// 4. Server transcribes when user stops speaking
// 5. Server sends transcription as a user message
// 6. Message appears in chat via useChat() hook

// Listen for new messages that are transcriptions
useEffect(() => {
  // Voice transcriptions appear as regular messages
  // They're already in the chat history from useChat()
  const lastMessage = messages[messages.length - 1]

  if (lastMessage?.role === 'user' && lastMessage?.isTranscription) {
    // This was a voice message that just came in
    console.log('Voice transcription received:', lastMessage.content)
  }
}, [messages])

// Visual feedback during recording
const RecordingIndicator: React.FC = () => {
  if (!isStreaming) return null

  return (
    <div className="flex items-center gap-2 text-sm text-muted-foreground">
      <div className="w-2 h-2 bg-red-500 rounded-full animate-pulse" />
      <span>Recording... Speak naturally, then pause when done.</span>
    </div>
  )
}
```

## Accessibility Requirements

### 1. ARIA Attributes

```typescript
// Editor ARIA
<div
  role="textbox"
  aria-label="Type your message"
  aria-multiline="true"
  aria-required={required}
  aria-invalid={hasError}
  aria-describedby={errorId}
  tabIndex={0}
/>

// Microphone button (NOT a toggle)
<button
  aria-label={isRecording ? "Stop recording" : "Start recording"}
  aria-pressed={isRecording}
  aria-describedby="microphone-description"
/>

// Output selector
<button
  aria-label="Select agent response mode"
  aria-haspopup="menu"
  aria-expanded={isOutputMenuOpen}
/>

// Status announcements
<div className="sr-only" role="status" aria-live="polite">
  {isRecording && "Recording audio. Speak naturally, then pause when done."}
  {isAgentTyping && "Agent is typing"}
  {!canSendInput && "Please wait for your turn to speak"}
</div>
```

### 2. Keyboard Navigation

```typescript
// Tab order
tabIndex={0} // Editor
tabIndex={isDisabled ? -1 : 0} // Buttons when enabled

// Keyboard shortcuts
- Ctrl/Cmd + Enter: Submit (alternative)
- Escape: Cancel voice recording
- Ctrl/Cmd + B: Bold
- Ctrl/Cmd + I: Italic
- Ctrl/Cmd + K: Insert link
- Ctrl/Cmd + E: Inline code
```

### 3. Screen Reader Support

```typescript
// Live regions for dynamic updates
<div role="alert" aria-live="assertive">
  {errorMessage}
</div>

<div role="status" aria-live="polite">
  {characterCount}/{maxCharacters} characters
</div>

// Recording announcements
const announceRecordingState = (isRecording: boolean) => {
  const message = isRecording 
    ? "Recording started. Speak your message, then pause when done."
    : "Recording stopped. Your message is being processed."

  // Use aria-live region or screen reader API
  announce(message)
}

// Turn state announcements
const announceTurnState = (turnState: TurnState) => {
  const messages: Record<TurnState, string> = {
    'user_speaking': 'Your turn to speak',
    'agent_speaking': 'Agent is responding',
    'processing': 'Processing your message',
    'idle': 'Ready for input'
  }

  announce(messages[turnState])
}
```

## Performance Optimizations

### 1. Editor Optimizations

```typescript
// Debounce content updates
const debouncedUpdate = useMemo(
  () => debounce((content: string) => {
    updateContent(content)
  }, 300),
  []
)

// Memoize expensive computations
const wordCount = useMemo(() => {
  return content.trim().split(/\s+/).length
}, [content])

// Lazy load editor extensions
const extensions = useMemo(() => [
  // Load extensions once
], [])
```

### 2. Component Memoization

```typescript
// Memoize toolbar to prevent re-renders
const InputToolbar = React.memo(({ ...props }) => {
  // Toolbar implementation
}, (prevProps, nextProps) => {
  // Custom comparison
  return prevProps.canSend === nextProps.canSend &&
         prevProps.isVoiceMode === nextProps.isVoiceMode
})

// Memoize button callbacks
const handleSend = useCallback(() => {
  // Send logic
}, [content, canSubmit])
```

### 3. Audio Streaming Optimization

```typescript
// Audio level is already throttled in useAudio hook (100ms polling)
// Just use it directly for visual feedback
const audioLevelBars = useMemo(() => {
  if (!isRecording || !audioLevel) return null
  const bars = Math.ceil((audioLevel / 100) * 5)
  return Array(5).fill(0).map((_, i) => i < bars)
}, [isRecording, audioLevel])

// Debounce recording state changes to prevent UI flicker
const debouncedIsRecording = useDebounce(isRecording, 100)
```

## Error Handling

### 1. Input Validation

```typescript
const validateInput = (text: string): ValidationResult => {
  const errors: string[] = []

  // Check length
  if (text.length > maxCharacters) {
    errors.push(`Message exceeds ${maxCharacters} characters`)
  }

  // Check content
  if (!text.trim()) {
    errors.push('Message cannot be empty')
  }

  return {
    valid: errors.length === 0,
    errors
  }
}
```

### 2. Error Recovery

```typescript
// Network error handling
const handleSubmitError = (error: Error) => {
  setError(error.message)

  // Restore content for retry
  editor?.commands.setContent(lastContent)

  // Show retry button
  setShowRetry(true)
}

// Audio streaming error handling
const handleAudioError = (error: Error) => {
  // Stop streaming if active
  if (isStreaming) {
    stopStreaming()
  }

  // Show error message
  setError('Microphone unavailable. Please type your message instead.')

  // Note: User can always type - no "mode" to fall back to
}

// Turn state error handling
const handleTurnError = () => {
  // User tried to speak when not their turn
  setError('Please wait for your turn to speak')

  // Stop any active recording
  if (isStreaming) {
    stopStreaming()
  }
}
```

## Component Specifications

### AgentSelector Component

**Purpose:** Replace ModelSelector with rich agent selection showing capabilities

```typescript
interface AgentSelectorProps {
  agents: Agent[]
  selectedAgent: Agent | null
  onAgentSelect: (agent: Agent) => void
  disabled?: boolean
  className?: string
}

// Component features:
// - Dropdown trigger showing selected agent name
// - Expanded view shows agent cards
// - Each card displays:
//   - Agent avatar/icon
//   - Agent name and description
//   - Tool badges as a row at bottom
// - Search/filter capability
// - Keyboard navigation support
```

**Visual Design:**

```typescript
// Agent card layout
const AgentCard = ({ agent }: { agent: Agent }) => (
  <div className="p-3 rounded-lg border hover:bg-accent cursor-pointer">
    <div className="flex items-start gap-2 mb-2">
      <Avatar src={agent.avatar} fallback={agent.name[0]} />
      <div className="flex-1">
        <h4 className="font-medium text-sm">{agent.name}</h4>
        <p className="text-xs text-muted-foreground line-clamp-2">
          {agent.description}
        </p>
      </div>
    </div>
    {/* Tool badges row */}
    <div className="flex flex-wrap gap-1 mt-2">
      {agent.tools.map(tool => (
        <Badge key={tool.id} variant="secondary" className="text-xs">
          {tool.icon} {tool.name}
        </Badge>
      ))}
    </div>
  </div>
)
```

### OutputSelector Component

**Purpose:** Controls how the AGENT responds - Text only, Voice, or Avatar video (NOT user input mode)

```typescript
interface OutputSelectorProps {
  selectedOption: OutputOption | null
  onOptionSelect: (option: OutputOption) => void
  voiceOptions: OutputOption[]    // From useVoiceModel hook
  avatarOptions: OutputOption[]   // From useAvatar hook
  disabled?: boolean
  className?: string
}

// Component features:
// - Controls AGENT OUTPUT format, not user input
// - Collapsed: Shows current agent response mode (e.g., "Voice: Alloy", "Text Only", "Avatar: Josh")
// - Expanded: Hierarchical menu with:
//   - "Text Only" - Agent responds with text, no audio
//   - "Voice" submenu - Agent responds with voice audio
//   - "Avatar" submenu - Agent responds with HeyGen avatar video
// - Integration with useVoiceModel and useAvatar hooks
// - Sets voice to 'none' for text-only mode
// - Sets voice to 'avatar' for avatar mode
```

**Menu Structure:**

```typescript
// Hierarchical menu for AGENT OUTPUT selection
const OutputSelectorMenu = () => {
  const { voices, setVoice } = useVoiceModel()
  const { avatars, startAvatarSession, endAvatarSession } = useAvatar()

  const handleSelectTextOnly = () => {
    setVoice('none')  // Special voice mode for text-only
    if (currentAvatar) endAvatarSession()
    setSelectedOption({ 
      id: 'text', 
      name: 'Text Only', 
      type: 'text',
      available: true 
    })
  }

  const handleSelectVoice = (voice: Voice) => {
    setVoice(voice.id)
    if (currentAvatar) endAvatarSession()
    setSelectedOption({
      id: voice.id,
      name: `Voice: ${voice.name}`,
      type: 'voice',
      available: true,
      metadata: { voiceId: voice.id }
    })
  }

  const handleSelectAvatar = async (avatar: Avatar) => {
    setVoice('avatar')  // Special voice mode for avatar
    await startAvatarSession({ avatarId: avatar.id })
    setSelectedOption({
      id: avatar.id,
      name: `Avatar: ${avatar.name}`,
      type: 'avatar',
      available: true,
      metadata: { avatarId: avatar.id }
    })
  }

  return (
    <DropdownMenu>
      <DropdownMenuTrigger>
        <Button variant="ghost" size="sm" className="gap-2">
          {selectedOption?.type === 'text' && <MessageSquare className="h-4 w-4" />}
          {selectedOption?.type === 'voice' && <Volume2 className="h-4 w-4" />}
          {selectedOption?.type === 'avatar' && <User className="h-4 w-4" />}
          <span>{selectedOption?.name || 'Text Only'}</span>
          <ChevronDown className="h-3 w-3" />
        </Button>
      </DropdownMenuTrigger>
      <DropdownMenuContent align="start">
        <DropdownMenuLabel>Agent Response Mode</DropdownMenuLabel>
        <DropdownMenuSeparator />

        {/* Text-only option */}
        <DropdownMenuItem onClick={handleSelectTextOnly}>
          <MessageSquare className="h-4 w-4 mr-2" />
          Text Only
          {selectedOption?.type === 'text' && (
            <Check className="h-4 w-4 ml-auto" />
          )}
        </DropdownMenuItem>

        <DropdownMenuSeparator />

        {/* Voice options submenu */}
        <DropdownMenuSub>
          <DropdownMenuSubTrigger>
            <Volume2 className="h-4 w-4 mr-2" />
            Voice Responses
          </DropdownMenuSubTrigger>
          <DropdownMenuSubContent>
            {voices
              .filter(v => v.id !== 'none' && v.id !== 'avatar')  // Filter special modes
              .map(voice => (
                <DropdownMenuItem 
                  key={voice.id}
                  onClick={() => handleSelectVoice(voice)}
                >
                  {voice.name}
                  {selectedOption?.metadata?.voiceId === voice.id && (
                    <Check className="h-4 w-4 ml-auto" />
                  )}
                </DropdownMenuItem>
              ))}
          </DropdownMenuSubContent>
        </DropdownMenuSub>

        {/* Avatar options submenu */}
        <DropdownMenuSub>
          <DropdownMenuSubTrigger>
            <User className="h-4 w-4 mr-2" />
            Avatar Responses
          </DropdownMenuSubTrigger>
          <DropdownMenuSubContent>
            {avatars.map(avatar => (
              <DropdownMenuItem 
                key={avatar.id}
                onClick={() => handleSelectAvatar(avatar)}
              >
                {avatar.name}
                {selectedOption?.metadata?.avatarId === avatar.id && (
                  <Check className="h-4 w-4 ml-auto" />
                )}
              </DropdownMenuItem>
            ))}
          </DropdownMenuSubContent>
        </DropdownMenuSub>
      </DropdownMenuContent>
    </DropdownMenu>
  )
}
```

### ToolButton Component (Placeholder)

**Purpose:** Placeholder for future tool functionality

```typescript
const ToolButton: React.FC<{ disabled?: boolean }> = ({ disabled = true }) => (
  <Tooltip>
    <TooltipTrigger asChild>
      <Button
        variant="ghost"
        size="icon"
        disabled={disabled}
        className="h-8 w-8"
        aria-label="Tools (Coming Soon)"
      >
        <Wrench className="h-4 w-4" />
      </Button>
    </TooltipTrigger>
    <TooltipContent>
      <p>Tools (Coming Soon)</p>
    </TooltipContent>
  </Tooltip>
)
```

## Migration Path

### Phase 1: Basic Implementation

1. Implement core editor with Tiptap
2. Add basic toolbar with send button
3. Implement AgentSelector with card UI
4. Integrate with useChat hook

### Phase 2: Output Mode Integration

1. Implement OutputSelector with hierarchical menu
2. Integrate with useVoiceModel and useAvatar hooks
3. Add voice mode toggle
4. Implement transcription display

### Phase 3: Enhanced Features

1. Add attachment support
2. Keep ToolButton as placeholder
3. Add agent search/filtering
4. Optimize card rendering

### Phase 4: Polish

1. Add animations and transitions
2. Implement keyboard shortcuts
3. Optimize performance
4. Complete accessibility features

## Testing Requirements

### Unit Tests

- Input validation
- Character counting
- Keyboard shortcuts
- State management
- Agent selection
- Output mode switching

### Integration Tests

- SDK hook integration
- Voice/text mode switching
- Avatar initialization
- Message submission
- Error handling

### Accessibility Tests

- Keyboard navigation
- Screen reader compatibility
- ARIA attribute correctness
- Focus management

### Performance Tests

- Typing responsiveness
- Voice latency
- Memory usage
- Bundle size impact

## NPM Dependencies

### Required Packages

```bash
# Core editor (Tiptap/ProseMirror)
pnpm add @tiptap/react @tiptap/core @tiptap/pm @tiptap/starter-kit
pnpm add @tiptap/extension-placeholder @tiptap/extension-character-count

# UI components for new selectors
pnpm add @radix-ui/react-dropdown-menu   # For OutputSelector hierarchical menu
pnpm add @radix-ui/react-dialog          # For AgentSelector modal variant
pnpm add @radix-ui/react-tooltip         # For ToolButton placeholder
pnpm add @radix-ui/react-avatar          # For agent avatars

# Icons (if not already installed)
pnpm add lucide-react

# Development dependencies
pnpm add -D @types/react @types/react-dom
```

### Optional Performance Packages

```bash
# For virtualized agent lists (if many agents)
pnpm add @tanstack/react-virtual
```

## Implementation Summary

### Key Architecture Points

1. **No Voice/Text Modes** - Users can always type AND speak. The microphone button simply starts/stops audio streaming.

2. **Voice Flow is Server-Driven**:
   
   - Client streams PCM16 audio via WebSocket (useAudio hook)
   - Server does VAD and transcription
   - Transcriptions return as user messages in chat
   - NO client-side transcription events

3. **OutputSelector Controls Agent Response**:
   
   - Text Only: Agent responds with text (voice='none')
   - Voice: Agent responds with audio (voice='alloy', etc.)
   - Avatar: Agent responds with video (voice='avatar')

4. **Turn Management is Critical**:
   
   - Server controls who can speak via turn events
   - Audio respects turn state (stops when losing turn)
   - UI elements disable when not user's turn

5. **Proper SDK Integration**:
   
   - useChat() for text messages and history
   - useAudio() for microphone and streaming
   - useTurnState() for conversation flow
   - useVoiceModel() for agent voice selection
   - useAvatar() for avatar sessions

### Common Misconceptions to Avoid

❌ **Wrong**: "Switch between voice and text input modes"
✅ **Right**: Users can always type or speak simultaneously

❌ **Wrong**: "Display transcription in the input field"
✅ **Right**: Transcriptions appear as messages in chat

❌ **Wrong**: "Client handles voice transcription"
✅ **Right**: Server handles all VAD and transcription

❌ **Wrong**: "OutputSelector changes how users input"
✅ **Right**: OutputSelector changes how the agent responds

❌ **Wrong**: "Voice is a toggle state"
✅ **Right**: Microphone button starts/stops audio streaming

### Testing Checklist

- [ ] User can type while recording audio
- [ ] Microphone button shows recording state clearly
- [ ] Audio stops when user loses turn
- [ ] Voice transcriptions appear in chat, not input
- [ ] OutputSelector changes agent response format
- [ ] Text input disabled when not user's turn
- [ ] Audio level indicator works during recording
- [ ] Proper cleanup when switching output modes
- [ ] Error states handle microphone permissions
- [ ] Accessibility announcements for all states