/**
 * TipTap v2 Compatible Version of markdownExtensions.ts
 * 
 * Key changes from v3:
 * - Import statements are simpler (no module resolution issues)
 * - All extensions work with standard imports
 * - Better Next.js compatibility
 * 
 * IMPORTANT: StarterKit already includes markdown InputRules for:
 * - Bold (**text**)
 * - Italic (*text*)
 * - Headers (#, ##, etc.)
 * - Lists (-, *, 1.)
 * - Blockquotes (>)
 * - Code blocks (```)
 * We don't need to add custom InputRules for these!
 */
import { Extension } from '@tiptap/core';
import StarterKit from '@tiptap/starter-kit';
import Typography from '@tiptap/extension-typography';
import Link from '@tiptap/extension-link';
import CodeBlockLowlight from '@tiptap/extension-code-block-lowlight';
import { common, createLowlight } from 'lowlight';
// SmartPasteExtension removed - file upload is now handled by ChatInputArea with useFileUpload hook
// import { SmartPasteExtension, type SmartPasteOptions } from './SmartPasteExtension';

// Import language modules from highlight.js
import javascript from 'highlight.js/lib/languages/javascript';
import typescript from 'highlight.js/lib/languages/typescript';
import python from 'highlight.js/lib/languages/python';
import json from 'highlight.js/lib/languages/json';
import html from 'highlight.js/lib/languages/xml';
import css from 'highlight.js/lib/languages/css';
import markdown from 'highlight.js/lib/languages/markdown';
import bash from 'highlight.js/lib/languages/bash';
import shell from 'highlight.js/lib/languages/shell';
import yaml from 'highlight.js/lib/languages/yaml';
import sql from 'highlight.js/lib/languages/sql';
import dockerfile from 'highlight.js/lib/languages/dockerfile';

// Create lowlight instance with common languages
const lowlight = createLowlight(common);

// Register additional languages
lowlight.register('dockerfile', dockerfile);
lowlight.register('docker', dockerfile); // Alias

/**
 * Custom Keyboard Shortcuts Extension - v2 Compatible
 */
const KeyboardShortcuts = Extension.create({
  name: 'customKeyboardShortcuts',

  addKeyboardShortcuts() {
    return {
      // Cmd/Ctrl+B → Toggle bold
      'Mod-b': () => this.editor.commands.toggleBold(),
      'Mod-B': () => this.editor.commands.toggleBold(),
      
      // Cmd/Ctrl+I → Toggle italic
      'Mod-i': () => this.editor.commands.toggleItalic(),
      'Mod-I': () => this.editor.commands.toggleItalic(),
      
      // Cmd/Ctrl+Shift+S → Toggle strikethrough
      'Mod-Shift-s': () => this.editor.commands.toggleStrike(),
      'Mod-Shift-S': () => this.editor.commands.toggleStrike(),
      
      // Cmd/Ctrl+E → Toggle inline code
      'Mod-e': () => this.editor.commands.toggleCode(),
      'Mod-E': () => this.editor.commands.toggleCode(),
      
      // Cmd/Ctrl+Alt+C → Toggle code block
      'Mod-Alt-c': () => this.editor.commands.toggleCodeBlock(),
      'Mod-Alt-C': () => this.editor.commands.toggleCodeBlock(),
      
      // Cmd/Ctrl+K → Add/edit link
      'Mod-k': () => {
        const previousUrl = this.editor.getAttributes('link').href;
        const url = window.prompt('URL', previousUrl);

        if (url === null) {
          return false;
        }

        if (url === '') {
          this.editor
            .chain()
            .focus()
            .extendMarkRange('link')
            .unsetLink()
            .run();
          return true;
        }

        this.editor
          .chain()
          .focus()
          .extendMarkRange('link')
          .setLink({ href: url })
          .run();

        return true;
      },
      
      // Cmd/Ctrl+Z → Undo
      'Mod-z': () => this.editor.commands.undo(),
      
      // Cmd/Ctrl+Shift+Z → Redo
      'Mod-Shift-z': () => this.editor.commands.redo(),
      'Mod-y': () => this.editor.commands.redo(),
    };
  },
});

/**
 * Get configured markdown extensions - v2 Compatible
 */
export function getMarkdownExtensions(options: {
  placeholder?: string;
  onSubmit?: (text: string) => void;
  disabled?: boolean;
  // SmartPaste options removed - file upload is now handled by ChatInputArea
  // enableSmartPaste?: boolean;
  // maxImageSize?: number;
  // onImageUpload?: (file: File) => Promise<string>;
  // onImageUploadStart?: (file: File) => void;
  // onImageUploadComplete?: (url: string) => void;
  // onImageUploadError?: (error: Error) => void;
} = {}) {
  return [
    // StarterKit bundles most of what we need
    // Disable codeBlock since we're using CodeBlockLowlight for syntax highlighting
    StarterKit.configure({
      codeBlock: false,  // Disable to avoid conflict with CodeBlockLowlight
    }),
    
    // Typography extension for smart quotes and other text transformations
    Typography,

    // Enhanced code block with syntax highlighting
    CodeBlockLowlight.configure({
      lowlight,
      defaultLanguage: 'plaintext',
      HTMLAttributes: {
        class: 'not-prose',
      },
    }),

    // Link extension (not in StarterKit)
    Link.configure({
      openOnClick: false,
      HTMLAttributes: {
        class: 'text-primary underline hover:text-primary/80',
      },
    }),

    // Custom keyboard shortcuts (these are different from InputRules)
    KeyboardShortcuts,
    
    // SmartPasteExtension removed - file upload is now handled by ChatInputArea with useFileUpload hook
    // Smart paste handler for images would have been here but is now disabled
  ];
}

/**
 * Keyboard shortcut reference (same as v3)
 */
export const keyboardShortcuts = {
  formatting: [
    { keys: 'Cmd/Ctrl+B', description: 'Toggle bold' },
    { keys: 'Cmd/Ctrl+I', description: 'Toggle italic' },
    { keys: 'Cmd/Ctrl+E', description: 'Toggle inline code' },
    { keys: 'Cmd/Ctrl+Alt+C', description: 'Toggle code block' },
    { keys: 'Cmd/Ctrl+Shift+S', description: 'Toggle strikethrough' },
    { keys: 'Cmd/Ctrl+K', description: 'Add/edit link' },
  ],
  history: [
    { keys: 'Cmd/Ctrl+Z', description: 'Undo' },
    { keys: 'Cmd/Ctrl+Shift+Z', description: 'Redo' },
  ],
  actions: [
    { keys: 'Cmd/Ctrl+Enter', description: 'Submit' },
  ],
  markdown: [
    { syntax: '**text**', description: 'Bold text' },
    { syntax: '*text* or _text_', description: 'Italic text' },
    { syntax: '`text`', description: 'Inline code' },
    { syntax: '~~text~~', description: 'Strikethrough text' },
    { syntax: '# ', description: 'Heading 1' },
    { syntax: '## ', description: 'Heading 2' },
    { syntax: '### ', description: 'Heading 3' },
    { syntax: '- or * ', description: 'Bullet list' },
    { syntax: '1. ', description: 'Numbered list' },
    { syntax: '> ', description: 'Blockquote' },
    { syntax: '---', description: 'Horizontal rule' },
    { syntax: '```language ', description: 'Code block with syntax highlighting' },
  ],
};