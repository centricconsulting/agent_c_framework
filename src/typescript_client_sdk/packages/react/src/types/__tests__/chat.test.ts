/**
 * Tests for chat type definitions
 * Validates type guards, type safety, and TypeScript inference
 */

import { describe, it, expect } from 'vitest';
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
  isSystemAlertItem
} from '../chat';

describe('File Upload Types', () => {
  describe('FileAttachment', () => {
    it('should accept valid FileAttachment with null id (uploading)', () => {
      const file = new File(['test'], 'test.png', { type: 'image/png' });
      const attachment: FileAttachment = {
        file,
        id: null,
        status: 'uploading',
        progress: 50
      };

      expect(attachment.file).toBe(file);
      expect(attachment.id).toBeNull();
      expect(attachment.status).toBe('uploading');
      expect(attachment.progress).toBe(50);
    });

    it('should accept valid FileAttachment with string id (complete)', () => {
      const file = new File(['test'], 'test.png', { type: 'image/png' });
      const attachment: FileAttachment = {
        file,
        id: 'file_123',
        status: 'complete',
        progress: 100
      };

      expect(attachment.id).toBe('file_123');
      expect(attachment.status).toBe('complete');
      expect(attachment.progress).toBe(100);
    });

    it('should accept FileAttachment with error status and message', () => {
      const file = new File(['test'], 'test.png', { type: 'image/png' });
      const attachment: FileAttachment = {
        file,
        id: null,
        status: 'error',
        progress: 0,
        error: 'Upload failed: Network error'
      };

      expect(attachment.status).toBe('error');
      expect(attachment.error).toBe('Upload failed: Network error');
    });

    it('should accept FileAttachment with preview URL', () => {
      const file = new File(['test'], 'test.png', { type: 'image/png' });
      const attachment: FileAttachment = {
        file,
        id: null,
        status: 'pending',
        progress: 0,
        previewUrl: 'blob:http://example.com/abc-123'
      };

      expect(attachment.previewUrl).toBe('blob:http://example.com/abc-123');
    });

    it('should handle all status values', () => {
      const file = new File(['test'], 'test.png', { type: 'image/png' });
      const statuses: FileAttachment['status'][] = ['pending', 'uploading', 'complete', 'error'];

      statuses.forEach(status => {
        const attachment: FileAttachment = {
          file,
          id: status === 'complete' ? 'file_123' : null,
          status,
          progress: status === 'complete' ? 100 : 0
        };

        expect(attachment.status).toBe(status);
      });
    });
  });

  describe('UseFileUploadOptions', () => {
    it('should accept empty options object', () => {
      const options: UseFileUploadOptions = {};

      expect(options).toBeDefined();
    });

    it('should accept all valid options', () => {
      const options: UseFileUploadOptions = {
        maxFileSize: 10 * 1024 * 1024, // 10MB
        allowedMimeTypes: ['image/png', 'image/jpeg'],
        maxFiles: 5,
        autoUpload: true,
        generatePreviews: true
      };

      expect(options.maxFileSize).toBe(10485760);
      expect(options.allowedMimeTypes).toEqual(['image/png', 'image/jpeg']);
      expect(options.maxFiles).toBe(5);
      expect(options.autoUpload).toBe(true);
      expect(options.generatePreviews).toBe(true);
    });

    it('should accept partial options', () => {
      const options: UseFileUploadOptions = {
        maxFileSize: 5 * 1024 * 1024,
        autoUpload: true
      };

      expect(options.maxFileSize).toBe(5242880);
      expect(options.autoUpload).toBe(true);
      expect(options.allowedMimeTypes).toBeUndefined();
    });
  });

  describe('UseFileUploadReturn', () => {
    it('should have correct structure', () => {
      // This test validates the type structure by creating a mock implementation
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

      expect(mockReturn.attachments).toEqual([]);
      expect(typeof mockReturn.addFiles).toBe('function');
      expect(typeof mockReturn.removeFile).toBe('function');
      expect(typeof mockReturn.uploadAll).toBe('function');
      expect(typeof mockReturn.uploadFile).toBe('function');
      expect(typeof mockReturn.clearAll).toBe('function');
      expect(typeof mockReturn.getUploadedFileIds).toBe('function');
      expect(mockReturn.isUploading).toBe(false);
      expect(mockReturn.allComplete).toBe(true);
      expect(mockReturn.hasErrors).toBe(false);
      expect(mockReturn.overallProgress).toBe(0);
      expect(mockReturn.validationError).toBeNull();
    });

    it('should handle validation error state', () => {
      const mockReturn: UseFileUploadReturn = {
        attachments: [],
        addFiles: async () => {},
        removeFile: () => {},
        uploadAll: async () => {},
        uploadFile: async () => {},
        clearAll: () => {},
        getUploadedFileIds: () => [],
        isUploading: false,
        allComplete: false,
        hasErrors: false,
        overallProgress: 0,
        validationError: 'File size exceeds maximum allowed size'
      };

      expect(mockReturn.validationError).toBe('File size exceeds maximum allowed size');
    });
  });
});

describe('Multimodal Content Types', () => {
  describe('TextContentBlock', () => {
    it('should accept valid text content block', () => {
      const block: TextContentBlock = {
        type: 'text',
        text: 'Hello, world!'
      };

      expect(block.type).toBe('text');
      expect(block.text).toBe('Hello, world!');
    });

    it('should accept empty text content', () => {
      const block: TextContentBlock = {
        type: 'text',
        text: ''
      };

      expect(block.text).toBe('');
    });
  });

  describe('ImageContentBlock', () => {
    it('should accept image with base64 source', () => {
      const block: ImageContentBlock = {
        type: 'image',
        source: {
          type: 'base64',
          media_type: 'image/png',
          data: 'iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mNk+M9QDwADhgGAWjR9awAAAABJRU5ErkJggg=='
        }
      };

      expect(block.type).toBe('image');
      expect(block.source.type).toBe('base64');
      expect(block.source.media_type).toBe('image/png');
      expect(block.source.data).toBeDefined();
    });

    it('should accept image with URL source', () => {
      const block: ImageContentBlock = {
        type: 'image',
        source: {
          type: 'url',
          media_type: 'image/jpeg',
          url: 'https://example.com/image.jpg'
        }
      };

      expect(block.type).toBe('image');
      expect(block.source.type).toBe('url');
      expect(block.source.media_type).toBe('image/jpeg');
      expect(block.source.url).toBe('https://example.com/image.jpg');
    });

    it('should accept various image MIME types', () => {
      const mimeTypes = ['image/png', 'image/jpeg', 'image/gif', 'image/webp'];

      mimeTypes.forEach(mimeType => {
        const block: ImageContentBlock = {
          type: 'image',
          source: {
            type: 'url',
            media_type: mimeType,
            url: 'https://example.com/image'
          }
        };

        expect(block.source.media_type).toBe(mimeType);
      });
    });
  });

  describe('isTextContent type guard', () => {
    it('should correctly identify text content blocks', () => {
      const textBlock: MessageContentBlock = {
        type: 'text',
        text: 'Test text'
      };

      expect(isTextContent(textBlock)).toBe(true);
      
      // TypeScript should narrow the type after the guard
      if (isTextContent(textBlock)) {
        // This should compile without errors
        const text: string = textBlock.text;
        expect(text).toBe('Test text');
      }
    });

    it('should return false for image content blocks', () => {
      const imageBlock: MessageContentBlock = {
        type: 'image',
        source: {
          type: 'url',
          media_type: 'image/png',
          url: 'https://example.com/image.png'
        }
      };

      expect(isTextContent(imageBlock)).toBe(false);
    });

    it('should handle union type correctly', () => {
      const blocks: MessageContentBlock[] = [
        { type: 'text', text: 'First' },
        { type: 'image', source: { type: 'url', media_type: 'image/png', url: 'https://example.com/1.png' } },
        { type: 'text', text: 'Second' },
        { type: 'image', source: { type: 'url', media_type: 'image/jpeg', url: 'https://example.com/2.jpg' } }
      ];

      const textBlocks = blocks.filter(isTextContent);
      expect(textBlocks).toHaveLength(2);
      expect(textBlocks[0].text).toBe('First');
      expect(textBlocks[1].text).toBe('Second');
    });
  });

  describe('isImageContent type guard', () => {
    it('should correctly identify image content blocks', () => {
      const imageBlock: MessageContentBlock = {
        type: 'image',
        source: {
          type: 'base64',
          media_type: 'image/png',
          data: 'base64data'
        }
      };

      expect(isImageContent(imageBlock)).toBe(true);
      
      // TypeScript should narrow the type after the guard
      if (isImageContent(imageBlock)) {
        // This should compile without errors
        const source = imageBlock.source;
        expect(source.type).toBe('base64');
        expect(source.media_type).toBe('image/png');
      }
    });

    it('should return false for text content blocks', () => {
      const textBlock: MessageContentBlock = {
        type: 'text',
        text: 'Test text'
      };

      expect(isImageContent(textBlock)).toBe(false);
    });

    it('should handle union type correctly', () => {
      const blocks: MessageContentBlock[] = [
        { type: 'text', text: 'First' },
        { type: 'image', source: { type: 'url', media_type: 'image/png', url: 'https://example.com/1.png' } },
        { type: 'text', text: 'Second' },
        { type: 'image', source: { type: 'url', media_type: 'image/jpeg', url: 'https://example.com/2.jpg' } }
      ];

      const imageBlocks = blocks.filter(isImageContent);
      expect(imageBlocks).toHaveLength(2);
      expect(imageBlocks[0].source.media_type).toBe('image/png');
      expect(imageBlocks[1].source.media_type).toBe('image/jpeg');
    });
  });

  describe('MessageContentBlock discriminated union', () => {
    it('should allow creating mixed content arrays', () => {
      const content: MessageContentBlock[] = [
        {
          type: 'text',
          text: 'Here is an image:'
        },
        {
          type: 'image',
          source: {
            type: 'url',
            media_type: 'image/png',
            url: 'https://example.com/image.png'
          }
        },
        {
          type: 'text',
          text: 'What do you think?'
        }
      ];

      expect(content).toHaveLength(3);
      expect(content[0].type).toBe('text');
      expect(content[1].type).toBe('image');
      expect(content[2].type).toBe('text');
    });

    it('should enable type-safe processing of content blocks', () => {
      const blocks: MessageContentBlock[] = [
        { type: 'text', text: 'Hello' },
        { type: 'image', source: { type: 'url', media_type: 'image/png', url: 'test.png' } }
      ];

      const processedContent = blocks.map(block => {
        if (isTextContent(block)) {
          return `Text: ${block.text}`;
        } else if (isImageContent(block)) {
          return `Image: ${block.source.media_type}`;
        }
        return 'Unknown';
      });

      expect(processedContent).toEqual(['Text: Hello', 'Image: image/png']);
    });
  });
});

describe('Chat Item Types', () => {
  describe('MessageChatItem', () => {
    it('should accept valid message item', () => {
      const item: MessageChatItem = {
        id: 'msg_123',
        type: 'message',
        role: 'assistant',
        content: 'Hello!',
        timestamp: new Date().toISOString()
      };

      expect(item.type).toBe('message');
      expect(item.role).toBe('assistant');
      expect(item.content).toBe('Hello!');
    });

    it('should accept message with subsession metadata', () => {
      const item: MessageChatItem = {
        id: 'msg_123',
        type: 'message',
        role: 'assistant',
        content: 'Hello!',
        timestamp: new Date().toISOString(),
        isSubSession: true,
        metadata: {
          sessionId: 'session_abc',
          parentSessionId: 'parent_xyz',
          userSessionId: 'user_session_123'
        }
      };

      expect(item.isSubSession).toBe(true);
      expect(item.metadata?.sessionId).toBe('session_abc');
    });
  });

  describe('DividerChatItem', () => {
    it('should accept divider item', () => {
      const item: DividerChatItem = {
        id: 'divider_123',
        type: 'divider',
        timestamp: new Date().toISOString(),
        dividerType: 'start'
      };

      expect(item.type).toBe('divider');
      expect(item.dividerType).toBe('start');
    });

    it('should accept divider with metadata', () => {
      const item: DividerChatItem = {
        id: 'divider_123',
        type: 'divider',
        timestamp: new Date().toISOString(),
        dividerType: 'start',
        metadata: {
          subSessionType: 'chat',
          subAgentType: 'clone',
          primeAgentKey: 'prime_agent',
          subAgentKey: 'sub_agent'
        }
      };

      expect(item.metadata?.subSessionType).toBe('chat');
      expect(item.metadata?.subAgentType).toBe('clone');
    });
  });

  describe('MediaChatItem', () => {
    it('should accept media item with content', () => {
      const item: MediaChatItem = {
        id: 'media_123',
        type: 'media',
        timestamp: new Date().toISOString(),
        content: '<svg>...</svg>',
        contentType: 'image/svg+xml'
      };

      expect(item.type).toBe('media');
      expect(item.contentType).toBe('image/svg+xml');
      expect(item.content).toBeDefined();
    });

    it('should accept media item with URL only', () => {
      const item: MediaChatItem = {
        id: 'media_123',
        type: 'media',
        timestamp: new Date().toISOString(),
        contentType: 'text/html',
        metadata: {
          url: 'https://example.com/content.html',
          name: 'External Content'
        }
      };

      expect(item.metadata?.url).toBe('https://example.com/content.html');
      expect(item.content).toBeUndefined();
    });
  });

  describe('SystemAlertChatItem', () => {
    it('should accept system alert item', () => {
      const item: SystemAlertChatItem = {
        id: 'alert_123',
        type: 'system_alert',
        timestamp: new Date().toISOString(),
        content: 'Connection lost',
        severity: 'error',
        format: 'text'
      };

      expect(item.type).toBe('system_alert');
      expect(item.severity).toBe('error');
      expect(item.format).toBe('text');
    });

    it('should handle all severity levels', () => {
      const severities: SystemAlertChatItem['severity'][] = ['info', 'warning', 'error'];

      severities.forEach(severity => {
        const item: SystemAlertChatItem = {
          id: `alert_${severity}`,
          type: 'system_alert',
          timestamp: new Date().toISOString(),
          content: `${severity} message`,
          severity,
          format: 'text'
        };

        expect(item.severity).toBe(severity);
      });
    });
  });

  describe('Chat item type guards', () => {
    it('should correctly identify message items', () => {
      const item: ChatItem = {
        id: 'msg_123',
        type: 'message',
        role: 'user',
        content: 'Hello',
        timestamp: new Date().toISOString()
      };

      expect(isMessageItem(item)).toBe(true);
      expect(isDividerItem(item)).toBe(false);
      expect(isMediaItem(item)).toBe(false);
      expect(isSystemAlertItem(item)).toBe(false);
    });

    it('should correctly identify divider items', () => {
      const item: ChatItem = {
        id: 'div_123',
        type: 'divider',
        timestamp: new Date().toISOString(),
        dividerType: 'start'
      };

      expect(isDividerItem(item)).toBe(true);
      expect(isMessageItem(item)).toBe(false);
      expect(isMediaItem(item)).toBe(false);
      expect(isSystemAlertItem(item)).toBe(false);
    });

    it('should correctly identify media items', () => {
      const item: ChatItem = {
        id: 'media_123',
        type: 'media',
        timestamp: new Date().toISOString(),
        contentType: 'text/markdown',
        content: '# Title'
      };

      expect(isMediaItem(item)).toBe(true);
      expect(isMessageItem(item)).toBe(false);
      expect(isDividerItem(item)).toBe(false);
      expect(isSystemAlertItem(item)).toBe(false);
    });

    it('should correctly identify system alert items', () => {
      const item: ChatItem = {
        id: 'alert_123',
        type: 'system_alert',
        timestamp: new Date().toISOString(),
        content: 'Error occurred',
        severity: 'error',
        format: 'text'
      };

      expect(isSystemAlertItem(item)).toBe(true);
      expect(isMessageItem(item)).toBe(false);
      expect(isDividerItem(item)).toBe(false);
      expect(isMediaItem(item)).toBe(false);
    });

    it('should enable type-safe filtering of chat items', () => {
      const items: ChatItem[] = [
        { id: 'msg_1', type: 'message', role: 'user', content: 'Hi', timestamp: new Date().toISOString() },
        { id: 'div_1', type: 'divider', dividerType: 'start', timestamp: new Date().toISOString() },
        { id: 'media_1', type: 'media', contentType: 'text/html', content: '<p>Test</p>', timestamp: new Date().toISOString() },
        { id: 'alert_1', type: 'system_alert', content: 'Info', severity: 'info', format: 'text', timestamp: new Date().toISOString() }
      ];

      const messages = items.filter(isMessageItem);
      const dividers = items.filter(isDividerItem);
      const media = items.filter(isMediaItem);
      const alerts = items.filter(isSystemAlertItem);

      expect(messages).toHaveLength(1);
      expect(dividers).toHaveLength(1);
      expect(media).toHaveLength(1);
      expect(alerts).toHaveLength(1);
    });
  });
});

describe('Type Imports', () => {
  it('should allow importing types from module', () => {
    // This test verifies that types are properly exported and importable
    // The imports at the top of this file demonstrate this
    expect(true).toBe(true);
  });
});

describe('TypeScript Inference', () => {
  it('should infer types correctly with type guards', () => {
    const block: MessageContentBlock = {
      type: 'text',
      text: 'Hello'
    };

    if (isTextContent(block)) {
      // TypeScript should know block is TextContentBlock here
      const text: string = block.text; // Should compile without error
      expect(text).toBe('Hello');
    }

    if (isImageContent(block)) {
      // This branch shouldn't execute, but TypeScript should know block is ImageContentBlock
      const source = block.source; // Should compile without error
      expect(source).toBeDefined();
    }
  });

  it('should handle discriminated unions correctly', () => {
    const processBlock = (block: MessageContentBlock): string => {
      switch (block.type) {
        case 'text':
          // TypeScript knows block is TextContentBlock
          return block.text;
        case 'image':
          // TypeScript knows block is ImageContentBlock
          return block.source.media_type;
        default:
          // Exhaustiveness check
          const _exhaustive: never = block;
          return _exhaustive;
      }
    };

    expect(processBlock({ type: 'text', text: 'Hi' })).toBe('Hi');
    expect(processBlock({ 
      type: 'image', 
      source: { type: 'url', media_type: 'image/png', url: 'test.png' } 
    })).toBe('image/png');
  });
});
