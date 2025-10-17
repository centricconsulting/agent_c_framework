'use client'

import { useDropzone } from 'react-dropzone';
import { useState, useRef, useCallback } from 'react';
import { useChat, useFileUpload } from '@agentc/realtime-react';
import { Send, Paperclip } from 'lucide-react';
import { Button } from '../ui/button';
import { cn } from '../../lib/utils';
import { DropOverlay } from './DropOverlay';
import { FileAttachmentList } from './FileAttachmentList';

export interface ChatInputAreaProps {
  /** Placeholder text for input */
  placeholder?: string;
  /** Maximum number of files */
  maxFiles?: number;
  /** Maximum file size in bytes */
  maxFileSize?: number;
  /** Allowed MIME types */
  allowedMimeTypes?: string[];
  /** Callback when message is sent */
  onSend?: (text: string, fileIds?: string[]) => void;
  /** Whether input is disabled */
  disabled?: boolean;
  /** Whether to show file picker button */
  showFilePicker?: boolean;
  /** Custom className */
  className?: string;
  /** Auto-focus on mount */
  autoFocus?: boolean;
}

export function ChatInputArea({
  placeholder = "Type a message, paste an image, or drag files here...",
  maxFiles = 10,
  maxFileSize = 10 * 1024 * 1024,
  allowedMimeTypes = ['image/png', 'image/jpeg', 'image/gif', 'image/webp', 'image/svg+xml'],
  onSend,
  disabled = false,
  showFilePicker = true,
  className,
  autoFocus = false,
}: ChatInputAreaProps) {
  const { sendMessage, isAgentTyping } = useChat();
  const [text, setText] = useState('');
  const textareaRef = useRef<HTMLTextAreaElement>(null);
  
  // File upload management
  const fileUpload = useFileUpload({
    autoUpload: true,
    maxFiles,
    maxFileSize,
    allowedMimeTypes,
  });
  
  // Drag-drop support with react-dropzone
  const { getRootProps, getInputProps, isDragActive, open } = useDropzone({
    onDrop: (acceptedFiles) => fileUpload.addFiles(acceptedFiles),
    accept: allowedMimeTypes ? 
      Object.fromEntries(allowedMimeTypes.map(type => [type, []])) : 
      undefined,
    disabled: disabled,
    noClick: true,  // Only trigger on drag, not click (we have our own button)
    noKeyboard: true,  // We handle keyboard via file picker button
    maxFiles,
    maxSize: maxFileSize,
  });
  
  // Clipboard paste support (inline handler)
  const handlePaste = useCallback((e: React.ClipboardEvent) => {
    const items = e.clipboardData?.items;
    if (!items) return;
    
    const files: File[] = [];
    for (let i = 0; i < items.length; i++) {
      const item = items[i];
      if (item.kind === 'file') {
        const file = item.getAsFile();
        if (file && allowedMimeTypes.some(type => file.type.match(type.replace('*', '.*')))) {
          files.push(file);
        }
      }
    }
    
    if (files.length > 0) {
      e.preventDefault();
      fileUpload.addFiles(files);
    }
  }, [fileUpload, allowedMimeTypes]);
  
  const handleSend = async () => {
    if (!text.trim() && fileUpload.attachments.length === 0) return;
    if (fileUpload.isUploading) return;
    if (disabled) return;
    
    const fileIds = fileUpload.getUploadedFileIds();
    
    try {
      if (onSend) {
        onSend(text, fileIds.length > 0 ? fileIds : undefined);
      } else {
        await sendMessage(text, fileIds.length > 0 ? fileIds : undefined);
      }
      
      setText('');
      fileUpload.clearAll();
      textareaRef.current?.focus();
    } catch (error) {
      console.error('Failed to send message:', error);
    }
  };
  
  const handleKeyDown = (e: React.KeyboardEvent) => {
    if (e.key === 'Enter' && !e.shiftKey) {
      e.preventDefault();
      if (canSend) handleSend();
    }
  };
  
  const canSend = (text.trim() || fileUpload.attachments.length > 0)
    && !fileUpload.isUploading
    && !disabled
    && !isAgentTyping;
  
  return (
    <div 
      {...getRootProps()}
      className={cn(
        'relative flex flex-col gap-2 p-4 bg-background border-t',
        'transition-colors duration-200',
        isDragActive && 'bg-primary/5 border-primary',
        className
      )}
      aria-label="Message input area with file upload support"
    >
      <input {...getInputProps({ 'aria-label': 'Upload image files' })} />
      
      {/* Drag-drop overlay */}
      {isDragActive && (
        <DropOverlay isActive={isDragActive} />
      )}
      
      {/* File attachments */}
      {fileUpload.attachments.length > 0 && (
        <FileAttachmentList
          attachments={fileUpload.attachments}
          onRemove={fileUpload.removeFile}
        />
      )}
      
      {/* Validation error */}
      {fileUpload.validationError && (
        <div 
          className="text-sm text-destructive bg-destructive/10 p-2 rounded"
          role="alert"
        >
          {fileUpload.validationError}
        </div>
      )}
      
      {/* Upload progress */}
      {fileUpload.isUploading && (
        <div className="text-sm text-muted-foreground">
          Uploading files... {fileUpload.overallProgress}%
        </div>
      )}
      
      {/* Input area */}
      <div className="flex gap-2 items-end">
        <div className="flex-1 relative">
          <textarea
            ref={textareaRef}
            value={text}
            onChange={(e) => setText(e.target.value)}
            onKeyDown={handleKeyDown}
            onPaste={handlePaste}
            placeholder={placeholder}
            disabled={disabled || isAgentTyping}
            autoFocus={autoFocus}
            rows={1}
            className={cn(
              'w-full min-h-[44px] max-h-[200px] resize-none',
              'px-4 py-3 pr-12',
              'bg-background border rounded-lg',
              'focus:outline-none focus:ring-2 focus:ring-primary',
              'disabled:opacity-50 disabled:cursor-not-allowed',
              'transition-all duration-200'
            )}
            aria-label="Message text input"
            aria-describedby={fileUpload.validationError ? 'file-error' : undefined}
          />
        </div>
        
        {/* File picker button (uses dropzone's open function) */}
        {showFilePicker && (
          <Button
            type="button"
            onClick={open}
            disabled={disabled || fileUpload.attachments.length >= maxFiles}
            variant="ghost"
            size="icon"
            className="h-[44px] w-[44px]"
            aria-label="Attach files"
          >
            <Paperclip className="h-5 w-5" />
          </Button>
        )}
        
        {/* Send button */}
        <Button
          onClick={handleSend}
          disabled={!canSend}
          size="icon"
          aria-label="Send message"
          className="h-[44px] w-[44px]"
        >
          <Send className="h-5 w-5" />
        </Button>
      </div>
    </div>
  );
}
