import { Extension } from '@tiptap/core';
import { Plugin, PluginKey } from '@tiptap/pm/state';
import { Decoration, DecorationSet, EditorView } from '@tiptap/pm/view';

/**
 * Upload states for tracking image uploads
 */
export interface UploadState {
  id: string;
  file: File;
  pos: number;
  progress: number;
  status: 'uploading' | 'success' | 'error';
  url?: string;
  error?: string;
}

/**
 * Configuration options for the SmartPaste extension
 */
export interface SmartPasteOptions {
  /**
   * Maximum file size in bytes (default: 10MB)
   */
  maxFileSize?: number;
  
  /**
   * Accepted image formats
   */
  acceptedFormats?: string[];
  
  /**
   * Upload handler function
   * @param file - The file to upload
   * @returns Promise with the uploaded image URL
   */
  uploadImage?: (file: File) => Promise<string>;
  
  /**
   * Whether to show upload progress
   */
  showProgress?: boolean;
  
  /**
   * Callback when upload starts
   */
  onUploadStart?: (file: File) => void;
  
  /**
   * Callback when upload completes
   */
  onUploadComplete?: (url: string) => void;
  
  /**
   * Callback when upload fails
   */
  onUploadError?: (error: Error) => void;
}

// Plugin key for managing state
const uploadKey = new PluginKey<Map<string, UploadState>>('smartPaste');

/**
 * Helper function for class names
 */
function cn(...classes: (string | boolean | undefined)[]) {
  return classes.filter(Boolean).join(' ');
}

/**
 * Smart Paste Extension
 * 
 * Handles intelligent paste and drop operations for images:
 * - Detects when users paste or drop images
 * - Shows a placeholder while uploading
 * - Handles upload success/failure
 * - Replaces placeholder with actual image or error state
 * 
 * Features:
 * - Clipboard paste support (Ctrl/Cmd+V)
 * - Drag and drop support
 * - File size validation
 * - Format validation (PNG, JPEG, GIF, WebP)
 * - Upload progress tracking
 * - Error handling with user feedback
 */
export const SmartPasteExtension = Extension.create<SmartPasteOptions>({
  name: 'smartPaste',

  addOptions() {
    return {
      maxFileSize: 10 * 1024 * 1024, // 10MB
      acceptedFormats: ['image/png', 'image/jpeg', 'image/jpg', 'image/gif', 'image/webp'],
      showProgress: true,
      // Default mock upload handler
      uploadImage: async (file: File): Promise<string> => {
        // Simulate upload delay
        await new Promise(resolve => setTimeout(resolve, 2000));
        
        // Mock successful upload with placeholder URL
        // In production, this would upload to a real endpoint
        return `https://via.placeholder.com/400x300?text=${encodeURIComponent(file.name)}`;
      },
    };
  },

  addProseMirrorPlugins() {
    const extensionOptions = this.options;

    /**
     * Create placeholder element for uploading images
     */
    function createPlaceholderElement(upload: UploadState): HTMLElement {
      const container = document.createElement('div');
      container.className = 'inline-block align-middle mx-1 my-2';
      
      const placeholder = document.createElement('div');
      placeholder.className = cn(
        'relative inline-flex items-center gap-2 px-3 py-2 rounded-md',
        'bg-muted/50 border border-border',
        'min-w-[200px] max-w-[400px]',
        upload.status === 'error' && 'border-destructive bg-destructive/10'
      );
      
      // Status icon
      const statusIcon = document.createElement('div');
      statusIcon.className = 'flex-shrink-0';
      
      if (upload.status === 'uploading') {
        // Spinning loader
        statusIcon.innerHTML = `
          <svg class="animate-spin h-4 w-4 text-muted-foreground" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
            <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
            <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
          </svg>
        `;
      } else if (upload.status === 'success') {
        // Check icon
        statusIcon.innerHTML = `
          <svg class="h-4 w-4 text-green-600" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke="currentColor">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M5 13l4 4L19 7"></path>
          </svg>
        `;
      } else if (upload.status === 'error') {
        // Error icon
        statusIcon.innerHTML = `
          <svg class="h-4 w-4 text-destructive" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke="currentColor">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8v4m0 4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z"></path>
          </svg>
        `;
      }
      
      // Text content
      const text = document.createElement('div');
      text.className = 'flex-1 text-sm';
      
      const fileName = document.createElement('div');
      fileName.className = 'font-medium text-foreground truncate';
      fileName.textContent = upload.file.name;
      
      const status = document.createElement('div');
      status.className = cn(
        'text-xs text-muted-foreground',
        upload.status === 'error' && 'text-destructive'
      );
      
      if (upload.status === 'uploading') {
        status.textContent = 'Uploading image...';
      } else if (upload.status === 'success') {
        status.textContent = 'Upload complete';
      } else if (upload.status === 'error') {
        status.textContent = upload.error || 'Upload failed';
      }
      
      text.appendChild(fileName);
      text.appendChild(status);
      
      // Progress bar (if uploading and progress tracking is enabled)
      if (upload.status === 'uploading' && extensionOptions.showProgress) {
        const progressContainer = document.createElement('div');
        progressContainer.className = 'absolute bottom-0 left-0 right-0 h-1 bg-muted rounded-b-md overflow-hidden';
        
        const progressBar = document.createElement('div');
        progressBar.className = 'h-full bg-primary transition-all duration-300';
        progressBar.style.width = `${upload.progress}%`;
        
        progressContainer.appendChild(progressBar);
        placeholder.appendChild(progressContainer);
      }
      
      placeholder.appendChild(statusIcon);
      placeholder.appendChild(text);
      container.appendChild(placeholder);
      
      return container;
    }

    /**
     * Handle image upload process
     */
    function handleImageUpload(view: EditorView, file: File, pos?: number) {
      const { maxFileSize, uploadImage, onUploadStart, onUploadComplete, onUploadError } = extensionOptions;
      
      // Validate file size
      if (maxFileSize && file.size > maxFileSize) {
        const errorMsg = `File size exceeds maximum allowed size of ${(maxFileSize / 1024 / 1024).toFixed(1)}MB`;
        if (onUploadError) {
          onUploadError(new Error(errorMsg));
        } else {
          alert(errorMsg);
        }
        return;
      }
      
      // Generate unique ID for this upload
      const uploadId = `upload-${Date.now()}-${Math.random().toString(36).substr(2, 9)}`;
      
      // Determine insertion position
      const insertPos = pos !== undefined ? pos : view.state.selection.from;
      
      // Insert placeholder text node
      const tr = view.state.tr.insertText('​', insertPos); // Zero-width space as placeholder
      
      // Add upload to state
      const uploadState: UploadState = {
        id: uploadId,
        file,
        pos: insertPos,
        progress: 0,
        status: 'uploading',
      };
      
      tr.setMeta(uploadKey, { action: 'add', id: uploadId, upload: uploadState });
      view.dispatch(tr);
      
      // Notify upload start
      if (onUploadStart) {
        onUploadStart(file);
      }
      
      // Start upload
      if (uploadImage) {
        uploadImage(file)
          .then((url: string) => {
            // Update state with success
            const tr = view.state.tr;
            tr.setMeta(uploadKey, {
              action: 'update',
              id: uploadId,
              upload: { status: 'success', url, progress: 100 },
            });
            view.dispatch(tr);
            
            // After a brief delay, replace placeholder with actual image
            setTimeout(() => {
              const uploads = uploadKey.getState(view.state);
              const upload = uploads?.get(uploadId);
              
              if (upload && upload.url) {
                // Remove placeholder and insert image
                const tr = view.state.tr;
                tr.delete(upload.pos, upload.pos + 1); // Remove zero-width space
                
                // Insert image with markdown syntax
                const imageText = `![${file.name}](${upload.url})`;
                tr.insertText(imageText, upload.pos);
                
                // Remove from uploads state
                tr.setMeta(uploadKey, { action: 'remove', id: uploadId });
                view.dispatch(tr);
                
                if (onUploadComplete) {
                  onUploadComplete(upload.url);
                }
              }
            }, 500); // Brief delay to show success state
          })
          .catch((error: Error) => {
            // Update state with error
            const tr = view.state.tr;
            tr.setMeta(uploadKey, {
              action: 'update',
              id: uploadId,
              upload: { 
                status: 'error', 
                error: error.message || 'Upload failed',
                progress: 0 
              },
            });
            view.dispatch(tr);
            
            if (onUploadError) {
              onUploadError(error);
            }
            
            // Remove failed upload after delay
            setTimeout(() => {
              const tr = view.state.tr;
              const uploads = uploadKey.getState(view.state);
              const upload = uploads?.get(uploadId);
              
              if (upload) {
                tr.delete(upload.pos, upload.pos + 1); // Remove placeholder
                tr.setMeta(uploadKey, { action: 'remove', id: uploadId });
                view.dispatch(tr);
              }
            }, 3000);
          });
      }
    }
    
    // Create and return the plugin
    return [
      new Plugin({
        key: uploadKey,
        state: {
          init() {
            return new Map<string, UploadState>();
          },
          apply(tr, value) {
            const meta = tr.getMeta(uploadKey);
            if (meta) {
              const newUploads = new Map(value);
              
              if (meta.action === 'add') {
                newUploads.set(meta.id, meta.upload);
              } else if (meta.action === 'update') {
                const existing = newUploads.get(meta.id);
                if (existing) {
                  newUploads.set(meta.id, { ...existing, ...meta.upload });
                }
              } else if (meta.action === 'remove') {
                newUploads.delete(meta.id);
              }
              
              return newUploads;
            }
            
            return value;
          },
        },
        
        props: {
          // Handle paste events
          handlePaste(view, event) {
            const items = Array.from(event.clipboardData?.items || []);
            const imageItems = items.filter(item => 
              item.type.startsWith('image/') && 
              extensionOptions.acceptedFormats?.includes(item.type)
            );
            
            if (imageItems.length === 0) {
              return false;
            }
            
            event.preventDefault();
            
            imageItems.forEach(item => {
              const file = item.getAsFile();
              if (file) {
                handleImageUpload(view, file);
              }
            });
            
            return true;
          },
          
          // Handle drop events
          handleDrop(view, event) {
            // Check if this is an internal drag-drop (moving content within editor)
            const dataTransfer = event.dataTransfer;
            if (!dataTransfer) {
              return false;
            }
            
            const files = Array.from(dataTransfer.files || []);
            const imageFiles = files.filter(file => 
              file.type.startsWith('image/') && 
              extensionOptions.acceptedFormats?.includes(file.type)
            );
            
            if (imageFiles.length === 0) {
              return false;
            }
            
            event.preventDefault();
            
            // Get drop position
            const pos = view.posAtCoords({
              left: event.clientX,
              top: event.clientY,
            })?.pos;
            
            if (pos === undefined) {
              return false;
            }
            
            imageFiles.forEach(file => {
              handleImageUpload(view, file, pos);
            });
            
            return true;
          },
          
          // Custom decorations for upload placeholders
          decorations(state) {
            const uploads = uploadKey.getState(state);
            if (!uploads || uploads.size === 0) {
              return DecorationSet.empty;
            }
            
            const decorations: Decoration[] = [];
            
            uploads.forEach((upload, id) => {
              const widget = Decoration.widget(upload.pos, () => {
                return createPlaceholderElement(upload);
              }, {
                id,
                side: -1,
              });
              
              decorations.push(widget);
            });
            
            return DecorationSet.create(state.doc, decorations);
          },
        },
      }),
    ];
  },
});