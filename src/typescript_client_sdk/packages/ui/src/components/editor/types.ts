/**
 * TypeScript interfaces for the Markdown Editor component
 */

/**
 * Props for the MarkdownEditor component
 */
export interface MarkdownEditorProps {
  /**
   * The current value of the editor (plain text)
   */
  value?: string;
  
  /**
   * Callback fired when the editor content changes
   * @param value - The new content as plain text
   */
  onChange?: (value: string) => void;
  
  /**
   * Placeholder text shown when the editor is empty
   * @default "How can I help you today?"
   */
  placeholder?: string;
  
  /**
   * Callback fired when the user submits the content (Cmd/Ctrl+Enter)
   * @param value - The current content as plain text
   */
  onSubmit?: (value: string) => void;
  
  /**
   * Whether the editor is disabled
   */
  disabled?: boolean;
  
  /**
   * Additional CSS classes to apply to the editor container
   */
  className?: string;
  
  /**
   * Enable smart paste handling for images
   * @default true
   */
  enableSmartPaste?: boolean;
  
  /**
   * Maximum file size for image uploads in bytes
   * @default 10485760 (10MB)
   */
  maxImageSize?: number;
  
  /**
   * Custom image upload handler
   * @param file - The image file to upload
   * @returns Promise with the uploaded image URL
   */
  onImageUpload?: (file: File) => Promise<string>;
  
  /**
   * Callback when image upload starts
   */
  onImageUploadStart?: (file: File) => void;
  
  /**
   * Callback when image upload completes
   */
  onImageUploadComplete?: (url: string) => void;
  
  /**
   * Callback when image upload fails
   */
  onImageUploadError?: (error: Error) => void;
  
  /**
   * Custom key handler for the editor
   * Return true to prevent default behavior
   */
  onKeyDown?: (event: KeyboardEvent) => boolean;
  
  /**
   * Custom paste handler for the editor
   * Used for handling file pastes from parent component
   */
  onPaste?: (event: React.ClipboardEvent) => void;
}