# Markdown Editor Components

This directory contains the markdown editor implementation with TipTap v3.

## Components

### MarkdownEditor
The core markdown editor component with full TipTap functionality. This component is NOT SSR-compatible due to TipTap v3's browser API dependencies.

### MarkdownEditorClient
A client-only wrapper component that ensures TipTap code is never executed during server-side rendering. Use this component in SSR environments like Next.js.

## SSR Compatibility

TipTap v3 is not SSR-compatible out of the box. It accesses browser APIs during module initialization, which causes errors during server-side rendering.

### For SSR Applications (Next.js, Remix, etc.)

Use `MarkdownEditorClient` instead of `MarkdownEditor`:

```tsx
import { MarkdownEditorClient } from '@agentc/ui';

export function MyComponent() {
  const [value, setValue] = useState('');
  
  return (
    <MarkdownEditorClient
      value={value}
      onChange={setValue}
      onSubmit={(text) => console.log('Submitted:', text)}
      placeholder="Type your message..."
    />
  );
}
```

### For Client-Only Applications

If your application doesn't use SSR, you can use `MarkdownEditor` directly:

```tsx
import { MarkdownEditor } from '@agentc/ui';

export function MyComponent() {
  const [value, setValue] = useState('');
  
  return (
    <MarkdownEditor
      value={value}
      onChange={setValue}
      onSubmit={(text) => console.log('Submitted:', text)}
    />
  );
}
```

### Alternative for Next.js

If you prefer to handle the dynamic import yourself in Next.js:

```tsx
import dynamic from 'next/dynamic';
import type { MarkdownEditorProps } from '@agentc/ui';

const MarkdownEditor = dynamic(
  () => import('@agentc/ui').then(mod => mod.MarkdownEditor),
  { 
    ssr: false,
    loading: () => <div>Loading editor...</div>
  }
);

// Then use MarkdownEditor normally
```

## Features

Both `MarkdownEditor` and `MarkdownEditorClient` provide the same features:

- **Markdown Syntax Support**: Bold, italic, code, headings, lists, etc.
- **Keyboard Shortcuts**: Cmd/Ctrl+B for bold, Cmd/Ctrl+I for italic, etc.
- **Smart Paste**: Drag & drop or paste images with upload support
- **Code Highlighting**: Syntax highlighting for code blocks
- **Controlled Component**: Full control via value/onChange props
- **Submit Handler**: Cmd/Ctrl+Enter to submit

## Props

Both components accept the same props:

```typescript
interface MarkdownEditorProps {
  value?: string;
  onChange?: (value: string) => void;
  placeholder?: string;
  onSubmit?: (value: string) => void;
  disabled?: boolean;
  className?: string;
  enableSmartPaste?: boolean;
  maxImageSize?: number;
  onImageUpload?: (file: File) => Promise<string>;
  onImageUploadStart?: (file: File) => void;
  onImageUploadComplete?: (url: string) => void;
  onImageUploadError?: (error: Error) => void;
}
```

## Implementation Details

The `MarkdownEditorClient` component:

1. Uses React.lazy() to dynamically import the MarkdownEditor
2. Shows a lightweight textarea fallback during loading
3. Only loads TipTap code after client hydration
4. Maintains the exact same API as MarkdownEditor
5. Properly forwards refs

This ensures that TipTap's browser API calls never happen during SSR, preventing build and runtime errors in SSR environments.