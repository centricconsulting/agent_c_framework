# CopyLinkButton Component

## Overview
A reusable button component that allows users to copy session URLs to their clipboard with visual feedback.

## Features
- **Visual Feedback**: Shows Link2 icon by default, changes to Check icon when copied
- **Color Indication**: Green color for successful copy, red for errors
- **Tooltip Support**: Shows "Copy link", "Copied!", or error messages
- **Auto-revert**: Returns to default state after 2 seconds
- **Graceful Fallback**: Works with both modern Clipboard API and legacy execCommand
- **Accessibility**: Proper ARIA labels and keyboard support

## Usage

### Basic Usage
```tsx
import { CopyLinkButton } from '@agentc/realtime-ui'

<CopyLinkButton sessionId="friendly-panda" />
```

### With Different Variants
```tsx
// Ghost variant (default for icon buttons)
<CopyLinkButton 
  sessionId="swift-eagle" 
  variant="ghost" 
  size="icon" 
/>

// Outline variant
<CopyLinkButton 
  sessionId="curious-dolphin" 
  variant="outline" 
  size="icon" 
/>

// With text label
<CopyLinkButton 
  sessionId="mighty-oak" 
  variant="secondary" 
  size="default" 
/>
```

### In Context (e.g., Chat Header)
```tsx
<div className="flex items-center justify-between">
  <h1>Chat Session</h1>
  <CopyLinkButton 
    sessionId={currentSessionId} 
    variant="ghost" 
    size="icon" 
  />
</div>
```

## Props

| Prop | Type | Default | Description |
|------|------|---------|-------------|
| `sessionId` | `string` | required | The session ID to include in the URL |
| `variant` | `'ghost' \| 'outline' \| 'default' \| 'secondary' \| 'destructive'` | `'ghost'` | Button visual variant |
| `size` | `'icon' \| 'sm' \| 'default' \| 'lg'` | `'icon'` | Button size |
| `className` | `string` | - | Additional CSS classes |

## URL Format
The component constructs URLs in the format:
```
${window.location.origin}/chat/${sessionId}
```

For example:
- `https://app.example.com/chat/friendly-panda`
- `http://localhost:3000/chat/swift-eagle`

## Error Handling
The component handles several error scenarios:
1. **Clipboard API not available**: Falls back to textarea + execCommand method
2. **Copy failure**: Shows error message in tooltip
3. **Permission denied**: Gracefully handles clipboard permission errors

## Accessibility
- Includes proper `aria-label` that changes based on state
- Icons marked with `aria-hidden="true"` since they're decorative
- Full keyboard support through underlying Button component
- Focus states clearly visible

## Implementation Details

### State Management
```typescript
- isCopied: boolean - Tracks if URL was successfully copied
- error: string | null - Holds error message if copy fails
- timeoutRef: ref - Manages auto-revert timing
```

### Copy Logic
1. Constructs full URL using window.location.origin
2. Attempts modern Clipboard API first
3. Falls back to legacy method if needed
4. Updates UI state based on success/failure
5. Auto-reverts after 2 seconds

### Visual States
- **Default**: Link2 icon, normal button appearance
- **Success**: Check icon with green color
- **Error**: Link2 icon with red color and error tooltip
- **Loading**: Prevents rapid clicks during copy operation

## Browser Compatibility
- **Modern browsers**: Uses Clipboard API
- **Legacy browsers**: Falls back to execCommand('copy')
- **HTTPS required**: Clipboard API requires secure context

## Examples
See `CopyLinkButton.example.tsx` for comprehensive usage examples.