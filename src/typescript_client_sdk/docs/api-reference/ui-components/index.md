# @agentc/realtime-ui Documentation Index

> Complete documentation index for the Agent C Realtime UI component library

## 📚 Quick Navigation

### Core Documentation
- 📖 [**Package Overview & Getting Started**](./README.md) - Complete guide to the UI library, installation, setup, and architecture
- 🎨 [**CenSuite Design System Compliance**](./README.md#design-system-compliance) - Design principles, color system, and typography guidelines

### Component Categories

#### 🎙️ [Audio Components](./audio-components.md)
Real-time audio visualization and control components for voice interactions
- **VoiceVisualizerView** - WebGL-powered audio waveform visualization with frequency analysis
- **AudioLevelIndicator** - Visual feedback for microphone and speaker levels
- **AudioControlPanel** - Comprehensive audio settings and controls

#### 👤 [Avatar Components](./avatar-components.md)
HeyGen streaming avatar integration for enhanced user experience
- **AvatarDisplayView** - Live avatar video display with synchronized lip-sync
- **AvatarControls** - Avatar selection and configuration interface
- **AvatarStatusIndicator** - Connection and streaming status display

#### 💬 [Chat Components](./chat-components.md)
Complete chat interface building blocks for real-time conversations
- **ChatMessagesView** - Main container with auto-scroll and typing indicators
- **Message** - Flexible message display with markdown and rich content support
- **MessageList** - Virtualized list for performance with large histories
- **MessageFooter** - Timestamps, status indicators, and actions
- **TypingIndicator** - Animated feedback during agent responses
- **SystemNotification** - System messages and connection alerts
- **SubsessionDivider** - Visual separation between conversation segments
- **ToolNotification** - Agent tool usage notifications

#### 🔌 [Connection Components](./connection-components.md)
WebSocket connection management and status visualization
- **ConnectionButton** - Primary connect/disconnect control with visual states
- **ConnectionStatus** - Detailed connection information and statistics
- **ConnectionIndicator** - Minimal status dot for space-constrained layouts
- **ConnectionErrorBoundary** - Graceful error handling with recovery options

#### 🎛️ [Control Components](./control-components.md)
Core application controls for configuration and settings
- **AudioControls** - Microphone and speaker management with visual feedback
- **AgentSelector** - Dynamic agent switching with profile display
- **OutputSelector** - Text/Voice/Avatar mode selection interface
- **ThemeSwitcher** - Light/dark mode toggle with system preference support
- **VoiceModelSelector** - TTS voice selection with preview capability

#### 📝 [Other Components](./other-components.md)
Additional UI elements for complete application functionality

**Editor Components** - Rich text editing capabilities
- **MarkdownEditor** - Legacy markdown editor with toolbar
- **RichTextEditor** - Modern TipTap-based rich text input

**Input Components** - Modern input interfaces
- **InputArea** - Unified text/voice input with mode switching
- **InputToolbar** - Formatting tools and quick actions
- **MicrophoneButton** - Push-to-talk and toggle recording modes

**Layout Components** - Application structure
- **MainContentArea** - Primary content container with responsive layout
- **AppShell** - Complete application wrapper with sidebar support

**Session Components** - Chat session management
- **ChatSessionList** - Session history browser with search
- **SessionNameDropdown** - Quick session switcher

**Sidebar Components** - Navigation and app chrome
- **ChatSidebar** - Collapsible navigation with session management
- **SidebarTopMenu** - Primary navigation and actions
- **UserDisplay** - User profile and settings access

**UI Primitives** - Core shadcn/ui components (30+)
- Form controls: Button, Input, Select, Checkbox, Radio, Switch
- Layout: Card, Sheet, Tabs, Accordion, Separator
- Feedback: Alert, Badge, Toast, Progress, Skeleton
- Overlays: Dialog, Dropdown, Popover, Tooltip
- Data: Avatar, Label, ScrollArea, Slider

## 🚀 Quick Reference Guide

### Essential Imports

```typescript
// Connection and status
import { 
  ConnectionButton, 
  ConnectionStatus,
  ConnectionIndicator 
} from '@agentc/realtime-ui';

// Chat interface
import { 
  ChatMessagesView,
  Message,
  MessageList,
  InputArea,
  TypingIndicator 
} from '@agentc/realtime-ui';

// Controls
import { 
  AudioControls,
  AgentSelector,
  OutputSelector,
  ThemeSwitcher 
} from '@agentc/realtime-ui';

// Layout
import { 
  MainContentArea,
  ChatSidebar,
  UserDisplay 
} from '@agentc/realtime-ui';

// UI primitives
import { 
  Button,
  Card,
  Dialog,
  Alert,
  Badge,
  Input,
  Select 
} from '@agentc/realtime-ui';
```

### Common Component Patterns

#### Basic Chat Interface
```tsx
<div className="flex flex-col h-screen">
  <header className="p-4 border-b">
    <ConnectionButton />
  </header>
  <main className="flex-1 overflow-hidden">
    <ChatMessagesView />
  </main>
  <footer className="border-t">
    <InputArea />
  </footer>
</div>
```

#### Control Panel
```tsx
<Card>
  <CardHeader>
    <CardTitle>Settings</CardTitle>
  </CardHeader>
  <CardContent className="space-y-4">
    <AudioControls />
    <AgentSelector />
    <OutputSelector />
    <ThemeSwitcher />
  </CardContent>
</Card>
```

#### Status Bar
```tsx
<div className="flex items-center gap-4 px-4 py-2 bg-muted">
  <ConnectionIndicator />
  <span className="text-sm text-muted-foreground">
    {connectionState}
  </span>
  <AudioLevelIndicator />
</div>
```

## 🎨 Design System Quick Reference

### Color Tokens (HSL-based)
```css
--primary: 255 55% 23.5%;         /* Deep blue */
--secondary: 255 30% 90%;         /* Light blue-gray */
--muted: 217 30% 95%;            /* Very light gray-blue */
--destructive: 0 100% 50%;       /* Red */
--background: 255 100% 100%;     /* White */
--foreground: 255 5% 10%;        /* Near black */
```

### Spacing Scale (4px base)
```typescript
"p-2"   // 8px - Tight
"p-4"   // 16px - Standard  
"p-6"   // 24px - Comfortable
"p-8"   // 32px - Spacious
```

### Typography Scale
```typescript
"text-5xl font-extrabold"  // H1 - Page titles
"text-4xl font-bold"        // H2 - Section headers  
"text-3xl font-semibold"    // H3 - Subsections
"text-base font-normal"     // Body text
"text-sm font-medium"       // Secondary text
```

### Component Variants
```typescript
// Button variants
<Button variant="default|outline|secondary|ghost|destructive" />

// Size options
<Button size="default|sm|lg|icon" />

// Alert variants
<Alert variant="default|destructive" />
```

## ♿ Accessibility Checklist

### Required Attributes
- ✅ `aria-label` for icon-only buttons
- ✅ `aria-describedby` for form validation
- ✅ `aria-live` for dynamic content
- ✅ `role` for semantic meaning
- ✅ `tabIndex` for keyboard navigation

### Keyboard Support
- ✅ Tab navigation through interactive elements
- ✅ Enter/Space for activation
- ✅ Escape for dismissal
- ✅ Arrow keys for selection

### Visual Requirements
- ✅ Focus indicators (ring)
- ✅ 4.5:1 contrast ratios minimum
- ✅ Error states clearly marked
- ✅ Loading states announced

## 🧪 Testing Components

### Test Setup
```typescript
import { render, screen, act } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { AgentCProvider } from '@agentc/realtime-react';

const renderWithProvider = (component: React.ReactElement) => {
  return render(
    <AgentCProvider config={mockConfig}>
      {component}
    </AgentCProvider>
  );
};
```

### Common Test Patterns
```typescript
// Test user interaction
it('should handle click events', async () => {
  const user = userEvent.setup();
  renderWithProvider(<ConnectionButton />);
  
  const button = screen.getByRole('button');
  await act(async () => {
    await user.click(button);
  });
  
  expect(mockConnect).toHaveBeenCalled();
});

// Test accessibility
it('should be keyboard navigable', async () => {
  renderWithProvider(<OutputSelector />);
  
  const selector = screen.getByRole('combobox');
  expect(selector).toHaveAttribute('aria-label');
  expect(selector).toHaveAttribute('aria-expanded');
});
```

## 📦 Package Information

### Dependencies
- **React**: ^18.0.0
- **Tailwind CSS**: ^3.0.0
- **@agentc/realtime-core**: workspace:*
- **@agentc/realtime-react**: workspace:*
- **Radix UI**: ^1.0.0
- **Framer Motion**: ^12.0.0
- **Lucide React**: ^0.330.0

### Build Output
- **Format**: ESM and CJS dual format
- **Types**: Full TypeScript definitions
- **Tree-shaking**: Supported
- **Bundle size**: Optimized with code splitting

## 📚 Additional Resources

### Internal Documentation
- [Testing Standards](../../testing_standards_and_architecture.md)
- [CenSuite Design System](../../ref/CenSuite_Starter)
- [SDK Core Documentation](../core/README.md)
- [React Hooks Documentation](../react/README.md)

### External Resources
- [shadcn/ui Documentation](https://ui.shadcn.com/)
- [Radix UI Primitives](https://www.radix-ui.com/)
- [Tailwind CSS](https://tailwindcss.com/)
- [WCAG 2.1 Guidelines](https://www.w3.org/WAI/WCAG21/quickref/)

## 🤝 Getting Help

### Support Channels
1. Review component documentation files
2. Check the troubleshooting section in README
3. Consult the design system guidelines
4. Contact the Agent C team

### Contributing
- Follow CenSuite design principles
- Maintain accessibility standards
- Write comprehensive tests
- Update documentation

---

*Last updated: Documentation current as of latest package version*
*Built with ❤️ by the Agent C Realtime Team*