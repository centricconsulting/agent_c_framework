/**
 * Integration test for type exports
 * Verifies that types are properly exported from the main package
 */

import { describe, it, expect } from 'vitest';

// Test importing from the main package index
// This validates the complete export chain: chat.ts -> types/index.ts -> src/index.ts
import {
  // File Upload Types
  type FileAttachment,
  type UseFileUploadOptions,
  type UseFileUploadReturn,
  // Multimodal Content Types
  type MessageContentBlock,
  type TextContentBlock,
  type ImageContentBlock,
  isTextContent,
  isImageContent,
  // Chat Item Types
  type ChatItem,
  type MessageChatItem,
  type DividerChatItem,
  type MediaChatItem,
  type SystemAlertChatItem,
  isMessageItem,
  isDividerItem,
  isMediaItem,
  isSystemAlertItem,
  // Error Types
  type ErrorInfo
} from '../../index';

describe('Type Exports from Main Package', () => {
  it('should export FileAttachment type', () => {
    const file = new File(['test'], 'test.png', { type: 'image/png' });
    const attachment: FileAttachment = {
      file,
      id: null,
      status: 'pending',
      progress: 0
    };

    expect(attachment).toBeDefined();
    expect(attachment.file).toBe(file);
  });

  it('should export UseFileUploadOptions type', () => {
    const options: UseFileUploadOptions = {
      maxFileSize: 5 * 1024 * 1024,
      autoUpload: true
    };

    expect(options).toBeDefined();
  });

  it('should export UseFileUploadReturn type', () => {
    const mockReturn: UseFileUploadReturn = {
      attachments: [],
      addFiles: async () => {},
      removeFile: () => {},
      uploadAll: async () => {},
      uploadFile: async () => {},
      clearAll: () => {},
      getUploadedFileIds: () => [],
      isUploading: false,
      allComplete: true,
      hasErrors: false,
      overallProgress: 0,
      validationError: null
    };

    expect(mockReturn).toBeDefined();
  });

  it('should export MessageContentBlock types', () => {
    const textBlock: TextContentBlock = {
      type: 'text',
      text: 'Hello'
    };

    const imageBlock: ImageContentBlock = {
      type: 'image',
      source: {
        type: 'url',
        media_type: 'image/png',
        url: 'https://example.com/image.png'
      }
    };

    const blocks: MessageContentBlock[] = [textBlock, imageBlock];

    expect(blocks).toHaveLength(2);
  });

  it('should export type guard functions', () => {
    expect(typeof isTextContent).toBe('function');
    expect(typeof isImageContent).toBe('function');
    expect(typeof isMessageItem).toBe('function');
    expect(typeof isDividerItem).toBe('function');
    expect(typeof isMediaItem).toBe('function');
    expect(typeof isSystemAlertItem).toBe('function');
  });

  it('should export ChatItem types', () => {
    const message: MessageChatItem = {
      id: 'msg_1',
      type: 'message',
      role: 'user',
      content: 'Hello',
      timestamp: new Date().toISOString()
    };

    const divider: DividerChatItem = {
      id: 'div_1',
      type: 'divider',
      dividerType: 'start',
      timestamp: new Date().toISOString()
    };

    const media: MediaChatItem = {
      id: 'media_1',
      type: 'media',
      contentType: 'text/html',
      content: '<p>Test</p>',
      timestamp: new Date().toISOString()
    };

    const alert: SystemAlertChatItem = {
      id: 'alert_1',
      type: 'system_alert',
      content: 'Test alert',
      severity: 'info',
      format: 'text',
      timestamp: new Date().toISOString()
    };

    const items: ChatItem[] = [message, divider, media, alert];

    expect(items).toHaveLength(4);
  });

  it('should export ErrorInfo type', () => {
    const error: ErrorInfo = {
      id: 'err_1',
      message: 'Test error',
      source: 'test',
      timestamp: new Date().toISOString(),
      dismissed: false
    };

    expect(error).toBeDefined();
  });

  it('should allow type narrowing with exported guards', () => {
    const block: MessageContentBlock = {
      type: 'text',
      text: 'Test'
    };

    if (isTextContent(block)) {
      // TypeScript should narrow the type here
      expect(block.text).toBe('Test');
    }
  });

  it('should work with discriminated unions from exports', () => {
    const items: ChatItem[] = [
      {
        id: 'msg_1',
        type: 'message',
        role: 'user',
        content: 'Hi',
        timestamp: new Date().toISOString()
      },
      {
        id: 'alert_1',
        type: 'system_alert',
        content: 'Info',
        severity: 'info',
        format: 'text',
        timestamp: new Date().toISOString()
      }
    ];

    const messages = items.filter(isMessageItem);
    const alerts = items.filter(isSystemAlertItem);

    expect(messages).toHaveLength(1);
    expect(alerts).toHaveLength(1);
  });
});

describe('TypeScript Compilation with Exports', () => {
  it('should compile with strict type checking', () => {
    // This test verifies that TypeScript can compile code using exported types
    const processContent = (blocks: MessageContentBlock[]): string[] => {
      return blocks.map(block => {
        if (isTextContent(block)) {
          return `text: ${block.text}`;
        } else if (isImageContent(block)) {
          return `image: ${block.source.media_type}`;
        }
        return 'unknown';
      });
    };

    const blocks: MessageContentBlock[] = [
      { type: 'text', text: 'Hello' },
      { type: 'image', source: { type: 'url', media_type: 'image/png', url: 'test.png' } }
    ];

    const result = processContent(blocks);

    expect(result).toEqual(['text: Hello', 'image: image/png']);
  });

  it('should enable exhaustive type checking', () => {
    const processItem = (item: ChatItem): string => {
      switch (item.type) {
        case 'message':
          return `message: ${item.content}`;
        case 'divider':
          return `divider: ${item.dividerType}`;
        case 'media':
          return `media: ${item.contentType}`;
        case 'system_alert':
          return `alert: ${item.severity}`;
        default:
          // TypeScript exhaustiveness check
          const _exhaustive: never = item;
          return _exhaustive;
      }
    };

    const message: ChatItem = {
      id: 'msg_1',
      type: 'message',
      role: 'user',
      content: 'Test',
      timestamp: new Date().toISOString()
    };

    expect(processItem(message)).toBe('message: Test');
  });
});
