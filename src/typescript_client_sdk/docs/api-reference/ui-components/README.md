# @agentc/realtime-ui

> Production-ready UI component library for Agent C Realtime SDK

## 📦 Package Overview

`@agentc/realtime-ui` is a comprehensive UI component library designed specifically for building real-time communication interfaces with the Agent C platform. Built on top of [shadcn/ui](https://ui.shadcn.com/) patterns and fully compliant with the CenSuite Design System, this library provides over 70 production-ready React components that seamlessly integrate with the Agent C Realtime SDK.

### Key Features

- **🎨 CenSuite Design System Compliance** - Every component follows CenSuite's five foundational principles
- **♿ WCAG 2.1 AA Accessibility** - Full keyboard navigation and screen reader support
- **🎭 Comprehensive Component Set** - 70+ components across 11 categories
- **🔧 TypeScript First** - Complete type definitions with full IntelliSense support
- **🎯 SDK Integration** - Direct hooks into @agentc/realtime-react for seamless state management
- **🌈 Theming Support** - HSL-based color system with light/dark mode support
- **📱 Responsive Design** - Mobile-first approach with adaptive layouts
- **⚡ Performance Optimized** - Tree-shakable exports and optimized bundle size

## 📥 Installation

```bash
# Using npm
npm install @agentc/realtime-ui

# Using yarn
yarn add @agentc/realtime-ui

# Using pnpm
pnpm add @agentc/realtime-ui
```

### Peer Dependencies

The following peer dependencies are required:

```json
{
  "react": "^18.0.0",
  "react-dom": "^18.0.0",
  "tailwindcss": "^3.0.0"
}
```

### Core Dependencies

The UI package depends on these Agent C packages:

```json
{
  "@agentc/realtime-core": "workspace:*",
  "@agentc/realtime-react": "workspace:*"
}
```

### UI Framework Dependencies

Built on industry-standard UI libraries:

- **Radix UI** - Unstyled, accessible component primitives (v1.0+)
- **Framer Motion** - Production-ready animation library (v12.0+)
- **Lucide React** - Beautiful & consistent icon set (v0.330+)
- **TipTap** - Headless rich text editor framework (v2.26+)
- **React Hook Form** - Performant forms with easy validation (v7.62+)

## 🎨 Design System Compliance

### CenSuite Principles

Every component in this library embodies CenSuite's five foundational principles:

#### 1. **Clarity**
Every UI element's function is immediately clear through:
- Descriptive labels and tooltips
- Consistent iconography from Lucide React
- Clear visual hierarchy with proper typography scales

#### 2. **Consistency**
Uniform patterns across all components via:
- Standardized component APIs with consistent prop interfaces
- Shared variant systems using class-variance-authority (CVA)
- Unified spacing scale based on 4px grid system

#### 3. **Efficiency**
Enable users to accomplish tasks effortlessly through:
- Smart defaults that work out of the box
- Optimized interaction patterns with minimal clicks
- Keyboard shortcuts and focus management

#### 4. **Scalability**
Components adapt to diverse requirements with:
- Flexible variant systems (size, variant, state)
- Composition-based architecture for complex UIs
- Responsive breakpoint system

#### 5. **Accessibility**
WCAG 2.1 AA compliance is mandatory, not optional:
- Full keyboard navigation support
- Screen reader announcements with ARIA labels
- Focus management and visual indicators
- High contrast ratios (4.5:1 minimum)

## 📁 Component Organization

The library is organized into 11 logical categories for easy discovery:

### 1. **Audio Components** (`/components/audio`)
Real-time audio visualization and control components
- `VoiceVisualizerView` - WebGL-powered audio waveform visualization

### 2. **Avatar Components** (`/components/avatar`)
HeyGen streaming avatar integration
- `AvatarDisplayView` - Live avatar video display with controls

### 3. **Chat Components** (`/components/chat`)
Complete chat interface building blocks
- `Message` - Flexible message display with markdown support
- `MessageList` - Virtualized message list with auto-scroll
- `MessageFooter` - Message metadata and actions
- `SystemNotification` - System messages and alerts
- `TypingIndicator` - Real-time typing status
- `ChatMessagesView` - Complete chat message container

### 4. **Connection Components** (`/components/connection`)
WebSocket connection management UI
- `ConnectionButton` - Connect/disconnect with status
- `ConnectionStatus` - Detailed connection information
- `ConnectionIndicator` - Minimal status indicator

### 5. **Control Components** (`/components/controls`)
Core application controls and selectors
- `AudioControls` - Microphone and speaker controls
- `AgentSelector` - Agent switching interface
- `OutputSelector` - Text/Voice/Avatar mode selector
- `ThemeSwitcher` - Light/dark mode toggle

### 6. **Editor Components** (`/components/editor`)
Rich text editing capabilities
- `MarkdownEditor` - Full-featured markdown editor (legacy)

### 7. **Input Components** (`/components/input`)
Modern input interfaces
- `InputArea` - Unified input with voice/text modes
- `RichTextEditor` - TipTap-based rich text input
- `InputToolbar` - Formatting and action toolbar
- `MicrophoneButton` - Push-to-talk or toggle modes

### 8. **Layout Components** (`/components/layout`)
Application structure components
- `MainContentArea` - Primary content container with responsive layout

### 9. **Session Components** (`/components/session`)
Chat session management
- `ChatSessionList` - Session history and management
- `SessionNameDropdown` - Quick session switcher

### 10. **Sidebar Components** (`/components/sidebar`)
Navigation and app chrome
- `ChatSidebar` - Collapsible navigation sidebar
- `SidebarTopMenu` - Primary navigation menu
- `UserDisplay` - User profile and settings

### 11. **UI Components** (`/components/ui`)
Core shadcn/ui primitives (30+ components)
- Form controls: `Button`, `Input`, `Select`, `Checkbox`, `Radio`, `Switch`
- Layout: `Card`, `Sheet`, `Tabs`, `Accordion`, `Separator`
- Feedback: `Alert`, `Badge`, `Toast`, `Progress`, `Skeleton`
- Overlays: `Dialog`, `Dropdown`, `Popover`, `Tooltip`
- Data: `Avatar`, `Label`, `ScrollArea`, `Slider`

## 🎨 Theming and Customization

### HSL-Based Color System

The library uses CSS custom properties with HSL values for maximum flexibility:

```css
@layer base {
  :root {
    /* Primary - Deep blue for primary actions */
    --primary: 255 55% 23.5%;
    --primary-foreground: 0 0% 100%;
    
    /* Secondary - Light blue-gray supporting elements */
    --secondary: 255 30% 90%;
    --secondary-foreground: 0 0% 0%;
    
    /* Muted - Very light gray-blue for backgrounds */
    --muted: 217 30% 95%;
    --muted-foreground: 255 5% 40%;
    
    /* Destructive - Red for errors/warnings */
    --destructive: 0 100% 50%;
    --destructive-foreground: 255 5% 100%;
    
    /* Structural colors */
    --background: 255 100% 100%;
    --foreground: 255 5% 10%;
    --border: 255 30% 82%;
    --input: 255 30% 50%;
    --ring: 255 55% 23.5%;
    
    /* Spacing */
    --radius: 0.5rem;
  }

  .dark {
    /* Dark mode overrides */
    --background: 222.2 84% 4.9%;
    --foreground: 210 40% 98%;
    /* ... additional dark mode values */
  }
}
```

### Customization Methods

#### 1. **Global Theme Override**
```css
/* In your global CSS */
:root {
  --primary: 200 100% 50%; /* Custom primary color */
  --radius: 0.75rem; /* Larger border radius */
}
```

#### 2. **Component-Level Styling**
```tsx
// Using className prop with Tailwind classes
<Button className="bg-blue-500 hover:bg-blue-600">
  Custom Button
</Button>

// Using the cn utility for conditional classes
<Card className={cn(
  "transition-all duration-200",
  isActive && "ring-2 ring-primary"
)}>
  Content
</Card>
```

#### 3. **Variant System Extension**
```tsx
// Components support variant props
<Button variant="outline" size="lg">
  Large Outline Button
</Button>

// Custom variants via className
<Alert className="border-green-500 bg-green-50">
  Success Message
</Alert>
```

## 📘 TypeScript Support

### Full Type Exports

Every component exports its props interface for type-safe usage:

```tsx
import { 
  Button, 
  type ButtonProps,
  ConnectionButton,
  type ConnectionButtonProps,
  OutputSelector,
  type OutputSelectorProps 
} from '@agentc/realtime-ui';

// Use types for custom wrappers
const CustomButton: React.FC<ButtonProps> = (props) => (
  <Button {...props} className={cn("custom-class", props.className)} />
);
```

### Discriminated Union Types

Complex components use discriminated unions for type safety:

```tsx
// Message types with role-specific properties
type MessageData = {
  role: 'user' | 'assistant' | 'system';
  content: string;
  timestamp: string;
  metadata?: Record<string, unknown>;
};

// Output modes with specific configurations
type OutputMode = 'text' | 'voice' | 'avatar';
type OutputOption = 
  | { type: 'text'; value: 'none' }
  | { type: 'voice'; value: string; vendor: string }
  | { type: 'avatar'; value: string; pose: string };
```

### Generic Components

Flexible generic components for various use cases:

```tsx
// Generic list component
interface ListProps<T> {
  items: T[];
  renderItem: (item: T) => React.ReactNode;
  keyExtractor: (item: T) => string;
}

// Usage with type inference
<MessageList
  items={messages}
  renderItem={(msg) => <Message {...msg} />}
  keyExtractor={(msg) => msg.id}
/>
```

## 🔨 Build Configuration

### TSConfig Setup

The package uses TypeScript 5.3+ with strict mode:

```json
{
  "compilerOptions": {
    "target": "ES2020",
    "module": "ESNext",
    "lib": ["ES2020", "DOM", "DOM.Iterable"],
    "jsx": "react-jsx",
    "strict": true,
    "esModuleInterop": true,
    "skipLibCheck": true,
    "forceConsistentCasingInFileNames": true,
    "declaration": true,
    "declarationMap": true,
    "sourceMap": true
  }
}
```

### Build Process

Built with `tsup` for optimal bundle size and tree-shaking:

```typescript
// tsup.config.ts
export default defineConfig({
  entry: ['src/index.ts'],
  format: ['cjs', 'esm'],  // Dual format support
  dts: {
    resolve: true,          // Resolve type imports
    entry: './src/index.ts'
  },
  splitting: false,         // No code splitting for libraries
  sourcemap: true,          // Include source maps
  clean: true,              // Clean dist folder
  external: [               // Don't bundle peer deps
    'react',
    'react-dom',
    '@agentc/realtime-react',
    'tailwindcss'
  ]
});
```

### Build Commands

```bash
# Development build with watch mode
pnpm dev

# Production build
pnpm build

# Type checking only
pnpm type-check

# Clean build artifacts
pnpm clean
```

## 🧪 Testing Approach

### Test Infrastructure

Uses Vitest with comprehensive testing utilities:

```typescript
// vitest.config.ts
export default defineConfig({
  test: {
    globals: true,
    environment: 'jsdom',           // Browser environment
    setupFiles: ['./src/test/setup.ts'],
    coverage: {
      provider: 'v8',
      reporter: ['text', 'json', 'html', 'lcov'],
      reportsDirectory: '../../.scratch/coverage/ui',
      thresholds: {
        branches: 80,               // 80% minimum coverage
        functions: 80,
        lines: 80,
        statements: 80
      }
    }
  }
});
```

### Testing Stack

- **Framework**: Vitest for fast unit testing
- **DOM Testing**: @testing-library/react with user-event
- **Accessibility**: jest-axe for automated a11y testing
- **Mocking**: MSW for API mocking, vi.mock() for modules

### Test Organization

```
src/
  components/
    chat/
      Message.tsx
      __tests__/
        Message.test.tsx          # Unit tests
        Message.integration.test.tsx  # Integration tests
      __mocks__/
        mockMessages.ts           # Test fixtures
```

### Test Commands

```bash
# Run all tests
pnpm test

# Watch mode for development
pnpm test:watch

# Generate coverage report
pnpm test:coverage

# Interactive UI mode
pnpm test:ui

# Run only unit tests
pnpm test:unit

# Run only integration tests
pnpm test:integration
```

### Testing Best Practices

1. **Component Testing Focus**
   - User interactions and behavior
   - Accessibility compliance
   - Error states and edge cases
   - Responsive behavior

2. **Mock Strategy**
   - Mock SDK hooks for isolation
   - Use MSW for API responses
   - Factory functions for test data
   - Avoid implementation details

3. **Coverage Requirements**
   - Minimum 80% for all metrics
   - 100% for critical paths
   - Focus on user-facing behavior
   - Test error boundaries

## 🚀 Quick Start

### Basic Setup

```tsx
// 1. Install dependencies
npm install @agentc/realtime-ui @agentc/realtime-react @agentc/realtime-core

// 2. Add Tailwind CSS configuration
// tailwind.config.js
module.exports = {
  content: [
    "./src/**/*.{js,ts,jsx,tsx}",
    "./node_modules/@agentc/realtime-ui/**/*.{js,ts,jsx,tsx}",
  ],
  theme: {
    extend: {},
  },
  plugins: [],
}

// 3. Add CSS variables to your global CSS
// globals.css
@import '@agentc/realtime-ui/styles/theme.css';

// 4. Wrap your app with AgentCProvider
// App.tsx
import { AgentCProvider } from '@agentc/realtime-react';
import { ConnectionButton, OutputSelector, InputArea } from '@agentc/realtime-ui';

function App() {
  return (
    <AgentCProvider 
      config={{
        apiKey: process.env.NEXT_PUBLIC_AGENTC_API_KEY,
        baseURL: process.env.NEXT_PUBLIC_AGENTC_BASE_URL,
      }}
    >
      <div className="flex flex-col h-screen">
        {/* Header Controls */}
        <div className="flex items-center gap-4 p-4 border-b">
          <ConnectionButton />
          <OutputSelector />
        </div>
        
        {/* Chat Area */}
        <div className="flex-1 overflow-hidden">
          <ChatMessagesView />
        </div>
        
        {/* Input Area */}
        <div className="border-t">
          <InputArea />
        </div>
      </div>
    </AgentCProvider>
  );
}
```

### Common Patterns

#### 1. **Chat Interface**
```tsx
import {
  ChatMessagesView,
  InputArea,
  ConnectionStatus,
  OutputSelector,
} from '@agentc/realtime-ui';

export function ChatInterface() {
  return (
    <div className="flex flex-col h-full">
      <header className="flex items-center justify-between p-4">
        <ConnectionStatus />
        <OutputSelector />
      </header>
      <main className="flex-1 overflow-hidden">
        <ChatMessagesView />
      </main>
      <footer>
        <InputArea 
          placeholder="Type a message..."
          submitOnEnter={true}
          showVoiceButton={true}
        />
      </footer>
    </div>
  );
}
```

#### 2. **Control Panel**
```tsx
import {
  Card,
  CardContent,
  CardHeader,
  CardTitle,
  AudioControls,
  AgentSelector,
  ThemeSwitcher,
} from '@agentc/realtime-ui';

export function ControlPanel() {
  return (
    <Card>
      <CardHeader>
        <CardTitle>Settings</CardTitle>
      </CardHeader>
      <CardContent className="space-y-4">
        <AgentSelector />
        <AudioControls />
        <ThemeSwitcher />
      </CardContent>
    </Card>
  );
}
```

#### 3. **Session Management**
```tsx
import {
  Sheet,
  SheetContent,
  SheetHeader,
  SheetTitle,
  SheetTrigger,
  Button,
  ChatSessionList,
} from '@agentc/realtime-ui';

export function SessionDrawer() {
  return (
    <Sheet>
      <SheetTrigger asChild>
        <Button variant="outline">Sessions</Button>
      </SheetTrigger>
      <SheetContent>
        <SheetHeader>
          <SheetTitle>Chat History</SheetTitle>
        </SheetHeader>
        <ChatSessionList 
          onSessionSelect={(id) => console.log('Selected:', id)}
        />
      </SheetContent>
    </Sheet>
  );
}
```

## 📚 Additional Resources

### Documentation
- [Agent C Realtime API Guide](../../api/docs/realtime_api_implementation_guide.md)
- [SDK Core Documentation](../core/README.md)
- [React Integration Guide](../react/README.md)
- [Testing Standards](../../testing_standards_and_architecture.md)

### Component Documentation
- [Audio Components](./audio-components.md)
- [Avatar Components](./avatar-components.md)
- [Chat Components](./chat-components.md)
- [Connection Components](./connection-components.md)
- [Control Components](./control-components.md)
- [Other Components (Layout, Editor, Input, Session, Sidebar, UI)](./other-components.md)

### External References
- [shadcn/ui Documentation](https://ui.shadcn.com/)
- [Radix UI Primitives](https://www.radix-ui.com/)
- [Tailwind CSS](https://tailwindcss.com/)
- [React Hook Form](https://react-hook-form.com/)
- [TipTap Editor](https://tiptap.dev/)

### Design Systems
- [CenSuite Design System Guidelines](../../ref/CenSuite_Starter)
- [WCAG 2.1 Guidelines](https://www.w3.org/WAI/WCAG21/quickref/)

## 📝 License

MIT © Agent C Team

## 🤝 Contributing

This package is part of the Agent C Realtime SDK monorepo. Contributions should follow the project's coding standards and design system guidelines.

### Development Workflow

1. **Clone the repository**
   ```bash
   git clone https://github.com/agentc/realtime-client.git
   cd realtime-client/packages/ui
   ```

2. **Install dependencies**
   ```bash
   pnpm install
   ```

3. **Run development mode**
   ```bash
   pnpm dev
   ```

4. **Run tests**
   ```bash
   pnpm test
   ```

5. **Build for production**
   ```bash
   pnpm build
   ```

### Code Standards

- Follow CenSuite Design System principles
- Maintain WCAG 2.1 AA accessibility compliance
- Write comprehensive tests for new components
- Use TypeScript strict mode
- Document all public APIs with JSDoc
- Follow the established component patterns

---

*Built with ❤️ by the Agent C Team*