# Smart Paste Handler Documentation

## Overview

The Smart Paste Handler is a custom Tiptap extension that intelligently handles image uploads when users paste or drop images into the markdown editor. It provides a seamless user experience with upload placeholders, progress tracking, and error handling.

## Features

### Core Functionality
- **Clipboard Paste Support**: Paste images directly from clipboard (Ctrl/Cmd+V)
- **Drag & Drop Support**: Drag image files into the editor
- **Upload Placeholders**: Shows visual feedback during upload
- **Progress Tracking**: Optional progress bar during upload
- **Error Handling**: Graceful error states with user feedback
- **Format Validation**: Supports PNG, JPEG, GIF, WebP
- **File Size Validation**: Configurable maximum file size (default: 10MB)

### Visual States

1. **Uploading State**
   - Spinning loader icon
   - "Uploading image..." text
   - Optional progress bar
   - Muted background color

2. **Success State**
   - Green checkmark icon
   - "Upload complete" text
   - Brief display before replacement

3. **Error State**
   - Red error icon
   - Error message display
   - Red-tinted background
   - Auto-removal after 3 seconds

## Usage

### Basic Implementation

```tsx
import { MarkdownEditor } from '@agentc/realtime-ui';

function MyComponent() {
  const [content, setContent] = useState('');

  return (
    <MarkdownEditor
      value={content}
      onChange={setContent}
      enableSmartPaste={true}
      maxImageSize={10 * 1024 * 1024} // 10MB
    />
  );
}
```

### Custom Upload Handler

```tsx
import { MarkdownEditor } from '@agentc/realtime-ui';

function MyComponent() {
  const [content, setContent] = useState('');

  const handleImageUpload = async (file: File): Promise<string> => {
    // Create form data
    const formData = new FormData();
    formData.append('image', file);

    // Upload to your server
    const response = await fetch('/api/upload/image', {
      method: 'POST',
      body: formData,
    });

    if (!response.ok) {
      throw new Error('Upload failed');
    }

    const data = await response.json();
    return data.url; // Return the uploaded image URL
  };

  return (
    <MarkdownEditor
      value={content}
      onChange={setContent}
      enableSmartPaste={true}
      onImageUpload={handleImageUpload}
      onImageUploadStart={(file) => console.log('Upload started:', file.name)}
      onImageUploadComplete={(url) => console.log('Upload complete:', url)}
      onImageUploadError={(error) => console.error('Upload failed:', error)}
    />
  );
}
```

## Configuration Options

### MarkdownEditor Props

| Prop | Type | Default | Description |
|------|------|---------|-------------|
| `enableSmartPaste` | `boolean` | `true` | Enable/disable smart paste functionality |
| `maxImageSize` | `number` | `10485760` (10MB) | Maximum file size in bytes |
| `onImageUpload` | `(file: File) => Promise<string>` | Mock handler | Custom upload handler |
| `onImageUploadStart` | `(file: File) => void` | - | Callback when upload starts |
| `onImageUploadComplete` | `(url: string) => void` | - | Callback when upload completes |
| `onImageUploadError` | `(error: Error) => void` | - | Callback when upload fails |

## How It Works

### Upload Flow

1. **Detection Phase**
   - User pastes image or drops file
   - Extension validates format and file size
   - Rejects invalid files with error message

2. **Placeholder Insertion**
   - Inserts zero-width space at cursor/drop position
   - Creates upload state with unique ID
   - Shows visual placeholder widget

3. **Upload Process**
   - Calls upload handler (mock or custom)
   - Updates placeholder with progress
   - Handles success or failure

4. **Completion**
   - **Success**: Replaces placeholder with markdown image syntax `![filename](url)`
   - **Error**: Shows error state, then removes placeholder

### Markdown Output

When an image is successfully uploaded, it's inserted as standard markdown:

```markdown
![image-name.png](https://example.com/uploads/image-name.png)
```

This ensures the editor maintains plain text output while providing rich upload functionality.

## Implementation Details

### Extension Architecture

The Smart Paste Extension uses ProseMirror plugins to:
- Intercept paste and drop events
- Manage upload state with a plugin key
- Create decorations for placeholder widgets
- Handle the upload lifecycle

### State Management

Upload states are tracked in the ProseMirror state using a plugin key:
- Each upload has a unique ID
- States include: uploading, success, error
- Position tracking for accurate replacement

### Error Handling

The extension handles various error scenarios:
- **File Too Large**: Shows size limit error
- **Invalid Format**: Rejects unsupported formats
- **Upload Failure**: Displays error message
- **Network Errors**: Graceful degradation

## Mock Upload Handler

For development and testing, a mock upload handler is included by default:
- 2-second simulated delay
- Returns placeholder.com URLs
- 10% random failure rate (in example)

```typescript
// Default mock handler
uploadImage: async (file: File): Promise<string> => {
  await new Promise(resolve => setTimeout(resolve, 2000));
  return `https://via.placeholder.com/400x300?text=${encodeURIComponent(file.name)}`;
}
```

## Browser Compatibility

The Smart Paste Handler uses standard web APIs:
- Clipboard API for paste events
- Drag and Drop API for file drops
- File API for file handling
- Works in all modern browsers

## Accessibility

The extension includes accessibility features:
- Visual loading states for screen readers
- Error messages with appropriate ARIA attributes
- Keyboard-accessible upload status
- Semantic HTML in placeholders

## Performance Considerations

- Lightweight placeholder widgets
- Efficient state updates
- Debounced UI updates
- Minimal re-renders
- CSS animations for smooth transitions

## Security Notes

When implementing a production upload handler:
1. Validate file types on the server
2. Implement virus scanning
3. Use secure upload endpoints
4. Sanitize file names
5. Implement rate limiting
6. Use proper authentication

## Example Integration

See `SmartPasteExample.tsx` for a complete working example with:
- Upload history tracking
- Success/error handling
- Raw markdown preview
- Interactive demonstration

## Troubleshooting

### Common Issues

1. **Images not pasting**
   - Ensure `enableSmartPaste` is true
   - Check browser clipboard permissions
   - Verify image format is supported

2. **Upload fails immediately**
   - Check `maxImageSize` setting
   - Verify upload handler is async
   - Check network connectivity

3. **Placeholder not showing**
   - Ensure CSS classes are available
   - Check for conflicting extensions
   - Verify Tailwind CSS is configured

### Debug Mode

Enable console logging to debug issues:

```tsx
onImageUploadStart={(file) => console.log('Upload start:', file)}
onImageUploadComplete={(url) => console.log('Upload complete:', url)}
onImageUploadError={(error) => console.error('Upload error:', error)}
```

## Future Enhancements

Potential improvements for future versions:
- [ ] Image compression before upload
- [ ] Multiple file selection
- [ ] Paste from URL support
- [ ] Image preview in placeholder
- [ ] Drag to reorder images
- [ ] Upload cancellation
- [ ] Retry failed uploads
- [ ] Upload queue management