"use client"

/**
 * TipTap v2 Compatible Version of MarkdownEditor.tsx
 * 
 * Key features:
 * - Import statements are cleaner
 * - SSR-safe with immediatelyRender: false to prevent hydration mismatches
 * - Better Next.js compatibility out of the box
 */
import * as React from 'react';
import { useEditor, EditorContent } from '@tiptap/react';
import Placeholder from '@tiptap/extension-placeholder';
import { cn } from '../../lib/utils';
import type { MarkdownEditorProps } from './types';
import { getMarkdownExtensions, keyboardShortcuts } from './markdownExtensions';
import { codeHighlightStyles } from './codeHighlightTheme';
import { editorStyles } from './editorStyles';

/**
 * MarkdownEditor component - v2 Compatible
 * 
 * This version works with TipTap v2 which has better Next.js compatibility
 * and doesn't have the module resolution issues of v3.
 */
const MarkdownEditor = React.forwardRef<HTMLDivElement, MarkdownEditorProps>(
  ({ 
    value = '', 
    onChange, 
    placeholder = 'How can I help you today?',
    onSubmit,
    disabled = false,
    className,
    enableSmartPaste = true,
    maxImageSize,
    onImageUpload,
    onImageUploadStart,
    onImageUploadComplete,
    onImageUploadError,
    onKeyDown,
    onPaste,
    ...props 
  }, ref) => {
    
    // Initialize Tiptap editor with markdown extensions and shortcuts
    const editor = useEditor({
      extensions: [
        // Get all markdown extensions with InputRules and keyboard shortcuts
        ...getMarkdownExtensions({ 
          placeholder, 
          onSubmit, 
          disabled,
          // SmartPaste options removed - file upload now handled by ChatInputArea
        }),
        // Placeholder extension for empty state
        Placeholder.configure({
          placeholder,
          emptyEditorClass: 'is-editor-empty',
        }),
        // Typography is already included in getMarkdownExtensions(), no need to add it again
      ],
      content: value,
      editable: !disabled,
      // Disable immediate rendering to prevent SSR hydration mismatches with Next.js
      immediatelyRender: false,
      onUpdate: ({ editor }) => {
        // Get plain text content and notify parent component
        const text = editor.getText();
        if (onChange) {
          onChange(text);
        }
      },
      editorProps: {
        attributes: {
          class: cn(
            'prose prose-sm max-w-none focus:outline-none',
            'p-3',
            // Prose overrides for better markdown display
            'prose-headings:mt-2 prose-headings:mb-1',
            'prose-p:my-1',
            'prose-ul:my-1 prose-ol:my-1',
            'prose-blockquote:my-2',
            'prose-code:before:content-none prose-code:after:content-none',
            'prose-hr:my-4',
            disabled && 'opacity-50 cursor-not-allowed',
            className
          ),
        },
        handleKeyDown: (view, event) => {
          // Custom key handler takes precedence
          if (onKeyDown) {
            const handled = onKeyDown(event);
            if (handled) {
              return true;
            }
          }
          
          // Handle Cmd/Ctrl+Enter for submit
          if ((event.metaKey || event.ctrlKey) && event.key === 'Enter') {
            event.preventDefault();
            if (onSubmit && !disabled) {
              const text = editor?.getText() || '';
              onSubmit(text);
            }
            return true;
          }
          return false;
        },
      },
    });

    // Update editor content when value prop changes
    React.useEffect(() => {
      if (editor && value !== editor.getText()) {
        editor.commands.setContent(value);
      }
    }, [value, editor]);

    // Update editor editable state when disabled prop changes
    React.useEffect(() => {
      if (editor) {
        editor.setEditable(!disabled);
      }
    }, [disabled, editor]);

    // Cleanup editor on unmount
    React.useEffect(() => {
      return () => {
        editor?.destroy();
      };
    }, [editor]);

    return (
      <div 
        ref={ref}
        className={cn(
          'relative w-full rounded-md border border-input bg-background',
          'focus-within:ring-2 focus-within:ring-ring focus-within:ring-offset-2',
          disabled && 'opacity-50',
          className
        )}
        onPaste={onPaste}
        {...props}
      >
        <EditorContent 
          editor={editor}
          className="w-full"
        />
        {/* Inject editor and syntax highlighting styles */}
        <style dangerouslySetInnerHTML={{ __html: editorStyles + codeHighlightStyles }} />
      </div>
    );
  }
);

MarkdownEditor.displayName = 'MarkdownEditor';

// Export keyboard shortcuts for documentation
export { MarkdownEditor, keyboardShortcuts };