# Markdown Editor Design Specification

## Overview

This document specifies the technical design for a rich markdown editor that provides live rendering within the input area, similar to Claude's implementation. The editor supports real-time markdown preview, syntax highlighting, image uploads, and seamless integration with the Agent C Realtime SDK's message system.

## Key Architecture Points

### What This Editor Does

- **Text Input Only** - The editor handles typed text that will be sent via `useChat()`
- **Markdown Formatting** - Live preview of markdown as users type
- **Image Uploads** - Smart paste/drop handling that uploads to server
- **Syntax Highlighting** - Code blocks with language detection

### What This Editor Does NOT Do

- **NO Voice Features** - Voice transcriptions come from the server as complete messages
- **NO Transcription Display** - Transcriptions appear in chat history, not in the editor
- **NO Voice Commands** - No client-side voice processing or commands
- **NO Mode Switching** - The editor is always for text input only

### How Voice Actually Works

1. User clicks microphone button (separate from editor)
2. Audio streams to server via `useAudio()` hook
3. Server transcribes and sends back as a user message
4. Message appears in chat history via `useChat()` hook
5. Editor remains empty and ready for text input

## Note from Donavan:

I have installed  these packages to support this.

- **@tiptap/core** ^3.3.0
- **@tiptap/extension-code-block-lowlight** ^3.3.0
- **@tiptap/extension-link** ^3.3.0
- **@tiptap/extension-placeholder** ^3.3.0
- **@tiptap/extension-typography** ^3.3.0
- **@tiptap/pm** ^3.3.0
- **@tiptap/react** ^3.3.0
- **@tiptap/starter-kit** ^3.3.0
- **highlight.js** ^11.11.1
- **lowlight** ^3.3.0

## Technical Approach

### Editor Library Selection

**Recommended: Tiptap (ProseMirror wrapper)**

**Rationale:**

- Built on ProseMirror's robust architecture
- Excellent React integration
- Modular extension system
- Real-time collaborative editing support
- Smaller bundle size than Lexical
- Better TypeScript support than vanilla ProseMirror

**Alternative Options Comparison:**

| Library         | Pros                                | Cons                  | Bundle Size |
| --------------- | ----------------------------------- | --------------------- | ----------- |
| **Tiptap**      | Modular, React-friendly, TypeScript | Learning curve        | ~100KB      |
| **Lexical**     | Facebook-backed, Modern             | Newer, less ecosystem | ~150KB      |
| **Slate**       | React-first design                  | Complex API           | ~120KB      |
| **ProseMirror** | Battle-tested, Flexible             | Complex setup         | ~80KB       |
| **CodeMirror**  | Excellent for code                  | Overkill for markdown | ~200KB      |

### Core Architecture

```typescript
// Editor instance with markdown support
import { useEditor } from '@tiptap/react'
import StarterKit from '@tiptap/starter-kit'
import Markdown from '@tiptap/extension-markdown'
import Typography from '@tiptap/extension-typography'
import Link from '@tiptap/extension-link'
import CodeBlock from '@tiptap/extension-code-block'
import Table from '@tiptap/extension-table'

const MarkdownEditor = () => {
  const editor = useEditor({
    extensions: [
      StarterKit,
      Markdown.configure({
        html: false,
        transformPastedText: true,
        transformCopiedText: true,
      }),
      Typography,
      Link.configure({
        openOnClick: false,
        autolink: true,
      }),
      CodeBlock.configure({
        languageClassPrefix: 'language-',
      }),
      Table.configure({
        resizable: true,
      }),
    ],
    content: '',
    editorProps: {
      attributes: {
        class: 'markdown-editor',
      },
    },
  })

  return <EditorContent editor={editor} />
}
```

## Feature Set

### 1. Core Markdown Features

#### Text Formatting

```typescript
// Inline formatting with live preview
const inlineFormattingExtensions = [
  {
    pattern: '**text**',
    render: 'Bold',
    shortcut: 'Ctrl+B',
    command: 'toggleBold'
  },
  {
    pattern: '*text*',
    render: 'Italic', 
    shortcut: 'Ctrl+I',
    command: 'toggleItalic'
  },
  {
    pattern: '`code`',
    render: 'Inline Code',
    shortcut: 'Ctrl+E',
    command: 'toggleCode'
  },
  {
    pattern: '~~text~~',
    render: 'Strikethrough',
    shortcut: 'Ctrl+Shift+S',
    command: 'toggleStrike'
  }
]
```

#### Block Elements

```typescript
// Block-level markdown elements
const blockFormattingExtensions = [
  {
    pattern: '# ',
    render: 'Heading 1',
    command: 'toggleHeading',
    attributes: { level: 1 }
  },
  {
    pattern: '> ',
    render: 'Blockquote',
    command: 'toggleBlockquote'
  },
  {
    pattern: '- ',
    render: 'Bullet List',
    command: 'toggleBulletList'
  },
  {
    pattern: '1. ',
    render: 'Ordered List',
    command: 'toggleOrderedList'
  },
  {
    pattern: '```',
    render: 'Code Block',
    command: 'toggleCodeBlock'
  },
  {
    pattern: '---',
    render: 'Horizontal Rule',
    command: 'setHorizontalRule'
  }
]
```

### 2. Live Rendering Implementation

#### Real-time Markdown Parser

```typescript
import { InputRule } from '@tiptap/core'
import { markdownToHtml } from './markdown-parser'

// Custom input rules for live markdown conversion
const createMarkdownInputRules = () => {
  return [
    // Bold
    new InputRule({
      find: /(?:^|\s)(\*\*|__)([^*_]+)(\*\*|__)$/,
      handler: ({ state, range, match }) => {
        const { from, to } = range
        const text = match[2]

        // Replace with bold mark
        return state.tr
          .delete(from, to)
          .insertText(text)
          .addMark(from, from + text.length, state.schema.marks.bold.create())
      }
    }),

    // Italic
    new InputRule({
      find: /(?:^|\s)(\*|_)([^*_]+)(\*|_)$/,
      handler: ({ state, range, match }) => {
        const text = match[2]
        return applyMark(state, range, text, 'italic')
      }
    }),

    // Inline code
    new InputRule({
      find: /(?:^|\s)`([^`]+)`$/,
      handler: ({ state, range, match }) => {
        const text = match[1]
        return applyMark(state, range, text, 'code')
      }
    }),
  ]
}
```

#### Smart Markdown Shortcuts

```typescript
// Automatic list continuation
const SmartLists = Extension.create({
  name: 'smartLists',

  addKeyboardShortcuts() {
    return {
      'Enter': ({ editor }) => {
        const { $from, $to } = editor.state.selection
        const range = $from.blockRange($to)

        if (!range) return false

        const listItem = editor.state.schema.nodes.listItem

        if (range.parent.type === listItem) {
          // Continue list with proper indentation
          return editor.commands.splitListItem('listItem')
        }

        return false
      },

      'Tab': ({ editor }) => {
        // Indent list item
        return editor.commands.sinkListItem('listItem')
      },

      'Shift-Tab': ({ editor }) => {
        // Outdent list item
        return editor.commands.liftListItem('listItem')
      }
    }
  }
})
```

### 3. Syntax Highlighting

#### Code Block Highlighting

```typescript
import { lowlight } from 'lowlight/lib/core'
import javascript from 'highlight.js/lib/languages/javascript'
import python from 'highlight.js/lib/languages/python'
import CodeBlockLowlight from '@tiptap/extension-code-block-lowlight'

// Register languages
lowlight.registerLanguage('javascript', javascript)
lowlight.registerLanguage('python', python)

const CodeHighlighting = CodeBlockLowlight.extend({
  addOptions() {
    return {
      ...this.parent?.(),
      lowlight,
      defaultLanguage: 'plaintext',
      HTMLAttributes: {
        class: 'code-block',
        spellcheck: false,
      },
    }
  },

  addKeyboardShortcuts() {
    return {
      'Mod-Alt-c': () => this.editor.commands.toggleCodeBlock(),
    }
  },
})
```

#### Inline Syntax Highlighting

```typescript
// Custom mark for inline code with syntax awareness
const InlineCode = Mark.create({
  name: 'inlineCode',

  parseHTML() {
    return [{ tag: 'code' }]
  },

  renderHTML({ HTMLAttributes }) {
    return ['code', mergeAttributes(HTMLAttributes, {
      class: 'inline-code',
      spellcheck: false,
    }), 0]
  },

  addInputRules() {
    return [
      markInputRule({
        find: /(?:^|\s)`([^`]+)`$/,
        type: this.type,
      }),
    ]
  },
})
```

### 4. Image Upload Integration

#### Smart Image Paste Handler

```typescript
const ImageUpload = Extension.create({
  name: 'imageUpload',

  addOptions() {
    return {
      uploadEndpoint: '/api/upload/image',
      maxFileSize: 10 * 1024 * 1024, // 10MB
      acceptedFormats: ['image/png', 'image/jpeg', 'image/gif', 'image/webp'],
      onUploadStart: () => {},
      onUploadProgress: (progress: number) => {},
      onUploadComplete: (url: string) => {},
      onUploadError: (error: Error) => {},
    }
  },

  addProseMirrorPlugins() {
    return [
      new Plugin({
        props: {
          handlePaste: (view, event) => {
            const items = Array.from(event.clipboardData?.items || [])
            const imageItem = items.find(item => item.type.startsWith('image/'))

            if (!imageItem) return false

            const file = imageItem.getAsFile()
            if (!file) return false

            event.preventDefault()
            this.handleImageUpload(file, view)
            return true
          },

          handleDrop: (view, event) => {
            const files = Array.from(event.dataTransfer?.files || [])
            const imageFiles = files.filter(file => file.type.startsWith('image/'))

            if (imageFiles.length === 0) return false

            event.preventDefault()
            imageFiles.forEach(file => this.handleImageUpload(file, view))
            return true
          },
        },
      }),
    ]
  },

  async handleImageUpload(file: File, view: EditorView) {
    // Validate file
    if (file.size > this.options.maxFileSize) {
      this.options.onUploadError(new Error('File too large'))
      return
    }

    if (!this.options.acceptedFormats.includes(file.type)) {
      this.options.onUploadError(new Error('Invalid file format'))
      return
    }

    // Insert placeholder while uploading
    const placeholderId = `uploading-${Date.now()}`
    const placeholder = view.state.schema.nodes.image.create({
      src: 'data:image/svg+xml;base64,...', // Loading spinner SVG
      alt: 'Uploading...',
      'data-placeholder-id': placeholderId,
    })

    const tr = view.state.tr.replaceSelectionWith(placeholder)
    view.dispatch(tr)

    try {
      this.options.onUploadStart()

      // Upload to server
      const formData = new FormData()
      formData.append('image', file)

      const response = await fetch(this.options.uploadEndpoint, {
        method: 'POST',
        body: formData,
        // Progress tracking
        onUploadProgress: (progress) => {
          this.options.onUploadProgress(progress.loaded / progress.total)
        },
      })

      if (!response.ok) throw new Error('Upload failed')

      const { url } = await response.json()

      // Replace placeholder with actual image
      const { tr } = view.state
      view.state.doc.descendants((node, pos) => {
        if (node.attrs['data-placeholder-id'] === placeholderId) {
          tr.setNodeMarkup(pos, null, {
            src: url,
            alt: file.name,
          })
        }
      })

      view.dispatch(tr)
      this.options.onUploadComplete(url)

    } catch (error) {
      // Remove placeholder on error
      const { tr } = view.state
      view.state.doc.descendants((node, pos) => {
        if (node.attrs['data-placeholder-id'] === placeholderId) {
          tr.delete(pos, pos + node.nodeSize)
        }
      })
      view.dispatch(tr)

      this.options.onUploadError(error as Error)
    }
  },
})
```

#### Inline Image Preview

```typescript
const ImageNode = Node.create({
  name: 'image',

  group: 'inline',
  inline: true,

  attrs: {
    src: { default: null },
    alt: { default: null },
    title: { default: null },
    width: { default: null },
    height: { default: null },
  },

  parseHTML() {
    return [{ tag: 'img[src]' }]
  },

  renderHTML({ HTMLAttributes }) {
    return ['img', mergeAttributes(HTMLAttributes)]
  },

  addCommands() {
    return {
      setImage: (options) => ({ commands }) => {
        return commands.insertContent({
          type: this.name,
          attrs: options,
        })
      },
    }
  },
})
```

## SDK Integration

### Integration with useChat Hook

```typescript
import { useEditor } from '@tiptap/react'
import { useChat } from '@agentc/realtime-react'

const MarkdownInput: React.FC = () => {
  const { sendMessage, isAgentTyping, messages } = useChat()
  const [content, setContent] = useState('')

  const editor = useEditor({
    extensions: [/* ... */],
    content: '',
    onUpdate: ({ editor }) => {
      // Update local state with plain text for sending
      const text = editor.getText()
      setContent(text)
    },
  })

  // Send message via SDK
  const handleSubmit = async () => {
    if (!content.trim() || isAgentTyping) return

    try {
      // Send plain text content via useChat
      await sendMessage(content)

      // Clear editor after successful send
      editor?.commands.clearContent()
      setContent('')
    } catch (error) {
      console.error('Failed to send message:', error)
    }
  }

  // Handle Enter key for submission
  useEffect(() => {
    if (!editor) return

    const handleKeyDown = (event: KeyboardEvent) => {
      if (event.key === 'Enter' && !event.shiftKey) {
        event.preventDefault()
        handleSubmit()
      }
    }

    editor.view.dom.addEventListener('keydown', handleKeyDown)
    return () => {
      editor.view.dom.removeEventListener('keydown', handleKeyDown)
    }
  }, [editor, content])

  return (
    <div>
      <EditorContent editor={editor} />
      <button onClick={handleSubmit} disabled={!content.trim() || isAgentTyping}>
        Send
      </button>
    </div>
  )
}
```

### Message Display with Markdown Rendering

```typescript
// Messages from useChat() may contain markdown
// Render them properly in the chat display

const MessageDisplay: React.FC<{ message: Message }> = ({ message }) => {
  const editor = useEditor({
    extensions: [
      StarterKit,
      // Add other extensions for display
    ],
    content: message.content,
    editable: false, // Read-only for display
  })

  return (
    <div className="message">
      <div className="message-role">{message.role}</div>
      <EditorContent editor={editor} className="message-content" />
    </div>
  )
}
```

### Important: Voice Transcriptions

**Voice transcriptions appear as regular messages in the chat history, NOT in the editor:**

```typescript
const ChatInterface: React.FC = () => {
  const { messages } = useChat()
  const { isStreaming, startStreaming, stopStreaming } = useAudio()

  // Voice transcriptions come back as messages from server
  // They appear in the messages array just like typed messages

  return (
    <div>
      {/* Chat history - includes both typed and voice messages */}
      <div className="chat-messages">
        {messages.map((msg, idx) => (
          <MessageDisplay key={idx} message={msg} />
        ))}
      </div>

      {/* Editor for typing messages */}
      <MarkdownInput />

      {/* Microphone button for voice input */}
      <button onClick={isStreaming ? stopStreaming : startStreaming}>
        {isStreaming ? 'Stop Recording' : 'Start Recording'}
      </button>
    </div>
  )
}
```

## Accessibility Considerations

### 1. Screen Reader Support

```typescript
// ARIA live regions for markdown preview
const MarkdownPreviewAnnouncer = () => {
  const [announcement, setAnnouncement] = useState('')

  useEffect(() => {
    const announceFormatting = (format: string) => {
      const announcements = {
        bold: 'Bold text applied',
        italic: 'Italic text applied',
        code: 'Code formatting applied',
        link: 'Link created',
        list: 'List started',
      }

      setAnnouncement(announcements[format] || `${format} applied`)

      // Clear after announcement
      setTimeout(() => setAnnouncement(''), 1000)
    }

    editor?.on('markApplied', announceFormatting)

    return () => {
      editor?.off('markApplied', announceFormatting)
    }
  }, [editor])

  return (
    <div className="sr-only" aria-live="polite" aria-atomic="true">
      {announcement}
    </div>
  )
}
```

### 2. Keyboard Navigation

```typescript
const keyboardShortcuts = {
  // Formatting
  'Mod-b': () => editor.chain().focus().toggleBold().run(),
  'Mod-i': () => editor.chain().focus().toggleItalic().run(),
  'Mod-e': () => editor.chain().focus().toggleCode().run(),
  'Mod-Shift-x': () => editor.chain().focus().toggleStrike().run(),

  // Blocks
  'Mod-Alt-1': () => editor.chain().focus().toggleHeading({ level: 1 }).run(),
  'Mod-Alt-2': () => editor.chain().focus().toggleHeading({ level: 2 }).run(),
  'Mod-Shift-7': () => editor.chain().focus().toggleOrderedList().run(),
  'Mod-Shift-8': () => editor.chain().focus().toggleBulletList().run(),
  'Mod-Shift-9': () => editor.chain().focus().toggleBlockquote().run(),

  // Navigation
  'Mod-Home': () => editor.chain().focus().setTextSelection(0).run(),
  'Mod-End': () => editor.chain().focus().setTextSelection(editor.state.doc.content.size).run(),

  // Undo/Redo
  'Mod-z': () => editor.chain().focus().undo().run(),
  'Mod-Shift-z': () => editor.chain().focus().redo().run(),
}
```

### 3. Focus Management

```typescript
const FocusManager = Extension.create({
  name: 'focusManager',

  addOptions() {
    return {
      trapFocus: false,
      returnFocus: null,
    }
  },

  onFocus({ editor }) {
    // Store previous focus
    this.options.returnFocus = document.activeElement

    // Announce editor focus
    announce('Markdown editor focused. Use keyboard shortcuts for formatting.')
  },

  onBlur({ editor }) {
    // Return focus if needed
    if (this.options.returnFocus && this.options.trapFocus) {
      this.options.returnFocus.focus()
    }
  },

  addKeyboardShortcuts() {
    return {
      'Escape': () => {
        // Exit editor and return focus
        if (this.options.returnFocus) {
          this.options.returnFocus.focus()
          return true
        }
        return false
      }
    }
  }
})
```

## Performance Optimization Strategies

### 1. Debounced Rendering

```typescript
// Debounce expensive markdown parsing
const DebouncedMarkdown = Extension.create({
  name: 'debouncedMarkdown',

  onCreate() {
    this.debouncedParse = debounce((content: string) => {
      this.parseMarkdown(content)
    }, 150)
  },

  onUpdate({ editor }) {
    const content = editor.getText()
    this.debouncedParse(content)
  },

  parseMarkdown(content: string) {
    // Parse markdown in web worker
    markdownWorker.postMessage({ type: 'parse', content })
  }
})
```

### 2. Virtual Scrolling for Long Documents

```typescript
const VirtualScroller = ({ editor, height = 400 }) => {
  const [visibleRange, setVisibleRange] = useState({ start: 0, end: 50 })

  const handleScroll = useCallback((scrollTop: number) => {
    const lineHeight = 24
    const buffer = 10

    const start = Math.max(0, Math.floor(scrollTop / lineHeight) - buffer)
    const end = Math.min(
      editor.state.doc.content.size,
      start + Math.ceil(height / lineHeight) + buffer * 2
    )

    setVisibleRange({ start, end })
  }, [height, editor])

  return (
    <VirtualScrollContainer onScroll={handleScroll}>
      <EditorContent 
        editor={editor}
        visibleRange={visibleRange}
      />
    </VirtualScrollContainer>
  )
}
```

### 3. Lazy Extension Loading

```typescript
// Load heavy extensions on demand
const loadCodeHighlighting = () => import('@tiptap/extension-code-block-lowlight')
const loadTables = () => import('@tiptap/extension-table')
const loadMathematics = () => import('@tiptap/extension-mathematics')

const LazyExtensions = {
  async loadAdvancedFeatures(editor: Editor) {
    const [CodeHighlighting, Tables, Mathematics] = await Promise.all([
      loadCodeHighlighting(),
      loadTables(),
      loadMathematics(),
    ])

    editor.extensionManager.addExtensions([
      CodeHighlighting.default,
      Tables.default,
      Mathematics.default,
    ])
  }
}
```

### 4. Memory Management

```typescript
const MemoryOptimizedEditor = () => {
  const editorRef = useRef<Editor | null>(null)

  useEffect(() => {
    // Create editor
    editorRef.current = new Editor({
      extensions: [...],
      editorProps: {
        // Limit history size
        historyOptions: {
          depth: 100,
          newGroupDelay: 500,
        },
      },
    })

    // Cleanup on unmount
    return () => {
      editorRef.current?.destroy()
      editorRef.current = null
    }
  }, [])

  // Periodically clean up old history
  useEffect(() => {
    const interval = setInterval(() => {
      const editor = editorRef.current
      if (editor && editor.state.doc.content.size > 50000) {
        // Trim old history
        editor.commands.clearHistory()
      }
    }, 60000) // Every minute

    return () => clearInterval(interval)
  }, [])
}
```

## Custom Extensions

### 1. Mention/Autocomplete

```typescript
const MentionExtension = Mention.configure({
  HTMLAttributes: {
    class: 'mention',
  },
  suggestion: {
    items: ({ query }) => {
      // Fetch suggestions based on query
      return fetchMentions(query)
    },
    render: () => {
      let component: ReactRenderer
      let popup: Instance

      return {
        onStart: (props) => {
          component = new ReactRenderer(MentionList, {
            props,
            editor: props.editor,
          })

          popup = tippy('body', {
            getReferenceClientRect: props.clientRect,
            appendTo: () => document.body,
            content: component.element,
            showOnCreate: true,
            interactive: true,
          })
        },

        onUpdate: (props) => {
          component.updateProps(props)
          popup.setProps({
            getReferenceClientRect: props.clientRect,
          })
        },

        onKeyDown: (props) => {
          if (props.event.key === 'Escape') {
            popup.hide()
            return true
          }

          return component.ref?.onKeyDown(props)
        },

        onExit: () => {
          popup.destroy()
          component.destroy()
        },
      }
    },
  },
})
```

### 2. Smart Paste Handler

```typescript
const SmartPaste = Extension.create({
  name: 'smartPaste',

  addOptions() {
    return {
      uploadImage: async (file: File) => {
        // Upload logic
        const formData = new FormData()
        formData.append('image', file)
        const response = await fetch('/api/upload/image', {
          method: 'POST',
          body: formData,
        })
        const { url } = await response.json()
        return url
      },
    }
  },

  addProseMirrorPlugins() {
    return [
      new Plugin({
        props: {
          handlePaste: async (view, event, slice) => {
            // Handle image paste
            const items = Array.from(event.clipboardData?.items || [])
            const imageItem = items.find(item => item.type.startsWith('image/'))

            if (imageItem) {
              const file = imageItem.getAsFile()
              if (file) {
                event.preventDefault()
                const url = await this.options.uploadImage(file)
                const node = view.state.schema.nodes.image.create({ src: url })
                view.dispatch(view.state.tr.replaceSelectionWith(node))
                return true
              }
            }

            // Handle text paste
            const text = event.clipboardData?.getData('text/plain')
            if (!text) return false

            // Detect markdown
            if (this.isMarkdown(text)) {
              const html = this.markdownToHtml(text)
              const parser = DOMParser.fromSchema(view.state.schema)
              const doc = parser.parse(html)
              view.dispatch(view.state.tr.replaceSelection(doc.slice(0)))
              return true
            }

            // Detect code
            if (this.isCode(text)) {
              view.dispatch(
                view.state.tr.replaceSelectionWith(
                  view.state.schema.nodes.codeBlock.create({
                    language: this.detectLanguage(text),
                  }, view.state.schema.text(text))
                )
              )
              return true
            }

            return false
          },

          handleDrop: async (view, event) => {
            // Handle image drop
            const files = Array.from(event.dataTransfer?.files || [])
            const imageFiles = files.filter(f => f.type.startsWith('image/'))

            if (imageFiles.length > 0) {
              event.preventDefault()
              for (const file of imageFiles) {
                const url = await this.options.uploadImage(file)
                const node = view.state.schema.nodes.image.create({ src: url })
                view.dispatch(view.state.tr.replaceSelectionWith(node))
              }
              return true
            }

            return false
          },
        },
      }),
    ]
  },

  isMarkdown(text: string): boolean {
    const patterns = [/^#{1,6}\s/, /^\*{1,2}/, /^-\s/, /^>\s/, /^```/]
    return patterns.some(pattern => pattern.test(text))
  },

  isCode(text: string): boolean {
    const codePatterns = [
      /^(function|const|let|var|class|import|export)/,
      /^def\s+\w+\(/,
      /^public\s+class/,
    ]
    return codePatterns.some(pattern => pattern.test(text.trim()))
  },
})
```

### ## Testing Strategy

### 1. Unit Tests

```typescript
describe('MarkdownEditor', () => {
  it('should convert markdown syntax to formatted text', () => {
    const editor = createTestEditor()

    editor.commands.setContent('**bold text**')
    expect(editor.getHTML()).toContain('<strong>bold text</strong>')

    editor.commands.setContent('*italic text*')
    expect(editor.getHTML()).toContain('<em>italic text</em>')

    editor.commands.setContent('`code`')
    expect(editor.getHTML()).toContain('<code>code</code>')
  })

  it('should handle keyboard shortcuts', () => {
    const editor = createTestEditor()

    editor.commands.setContent('text')
    editor.commands.selectAll()

    // Simulate Ctrl+B
    fireEvent.keyDown(editor.view.dom, { key: 'b', ctrlKey: true })
    expect(editor.getHTML()).toContain('<strong>text</strong>')
  })

  it('should handle image paste and upload', async () => {
    const editor = createTestEditor()
    const mockFile = new File(['image'], 'test.png', { type: 'image/png' })

    // Simulate image paste
    const pasteEvent = new ClipboardEvent('paste', {
      clipboardData: new DataTransfer(),
    })
    pasteEvent.clipboardData.items.add(mockFile)

    editor.view.dom.dispatchEvent(pasteEvent)

    // Should show upload placeholder
    expect(editor.getHTML()).toContain('Uploading...')

    // Wait for upload to complete
    await waitFor(() => {
      expect(editor.getHTML()).toContain('<img src="https://example.com/image.png">')
    })
  })
})
```

### 2. Integration Tests

```typescript
describe('MarkdownEditor Integration', () => {
  it('should work with useChat hook', async () => {
    const { result } = renderHook(() => useChat())
    const editor = createTestEditor()

    editor.commands.setContent('Test message')

    await act(async () => {
      await result.current.sendMessage(editor.getText())
    })

    expect(result.current.messages).toHaveLength(1)
    expect(result.current.messages[0].content).toBe('Test message')
  })

  it('should properly handle voice transcriptions in chat', async () => {
    const { result: chatResult } = renderHook(() => useChat())
    const { result: audioResult } = renderHook(() => useAudio())

    // Start voice recording
    await act(async () => {
      await audioResult.current.startStreaming()
    })

    // Voice transcriptions come back as messages from server
    // NOT inserted into the editor
    // Simulate server sending transcription as a message
    act(() => {
      // This would be triggered by server event
      chatResult.current.messages.push({
        role: 'user',
        content: 'Voice transcription from server',
        isTranscription: true,
      })
    })

    // Transcription appears in chat history, not editor
    expect(chatResult.current.messages).toHaveLength(1)
    expect(chatResult.current.messages[0].content).toBe('Voice transcription from server')

    // Editor remains empty - transcriptions don't go in editor
    const editor = createTestEditor()
    expect(editor.getText()).toBe('')
  })
})
```

### 3. Performance Tests

```typescript
describe('MarkdownEditor Performance', () => {
  it('should handle large documents efficiently', () => {
    const editor = createTestEditor()
    const largeText = 'Lorem ipsum '.repeat(10000)

    const startTime = performance.now()
    editor.commands.setContent(largeText)
    const endTime = performance.now()

    expect(endTime - startTime).toBeLessThan(100) // Under 100ms
  })

  it('should debounce markdown parsing', async () => {
    const parseSpy = jest.fn()
    const editor = createTestEditor({
      onParse: parseSpy,
    })

    // Type rapidly
    for (let i = 0; i < 10; i++) {
      editor.commands.insertContent('a')
      await new Promise(resolve => setTimeout(resolve, 10))
    }

    // Wait for debounce
    await new Promise(resolve => setTimeout(resolve, 200))

    // Should only parse once after debounce
    expect(parseSpy).toHaveBeenCalledTimes(1)
  })
})
```

## Implementation Roadmap

### Phase 1: Foundation (Day 1-2)

- Set up Tiptap with basic StarterKit
- Implement core markdown input rules
- Add basic keyboard shortcuts
- Create editor component structure
- Integrate with useChat() for message sending

### Phase 2: Markdown Features (Day 3-4)

- Add live markdown preview
- Implement syntax highlighting with lowlight
- Add smart lists and indentation
- Create custom markdown parser
- Test markdown rendering in both input and display

### Phase 3: Image Upload (Day 5)

- Implement image paste handler
- Add drag-and-drop support
- Create upload progress indicators
- Handle upload errors gracefully
- Add image preview in editor

### Phase 4: Advanced Features (Day 6-7)

- Add mention/autocomplete (if needed)
- Implement smart paste for code detection
- Optimize performance for large documents
- Add undo/redo history management

### Phase 5: Polish & Testing (Day 8)

- Complete accessibility features
- Add comprehensive tests
- Ensure proper cleanup on unmount
- Documentation and examples
- Integration testing with full chat flow

## Bundle Size Considerations

### Base Editor

- Tiptap Core: ~25KB
- StarterKit: ~40KB
- React bindings: ~10KB
- **Total Base**: ~75KB

### Optional Extensions

- Syntax highlighting: ~30KB
- Markdown parser: ~15KB
- Collaboration: ~20KB
- Table support: ~20KB
- **Total with all features**: ~160KB

### Optimization Strategies

1. Dynamic imports for heavy features
2. Tree-shaking unused extensions
3. Custom builds for specific use cases
4. CDN delivery for syntax highlighting languages