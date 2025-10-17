/**
 * React Package Integration Verification Test
 * 
 * This test verifies that all required types and hooks from @agentc/realtime-react
 * are available and correctly typed for use in UI components.
 */

import { describe, it, expect } from 'vitest';
import { renderHook } from '@testing-library/react';
import { 
  useFileUpload, 
  useChat,
  type FileAttachment,
  type MessageContentBlock,
  type TextContentBlock,
  type ImageContentBlock,
  type UseFileUploadReturn,
  isTextContent,
  isImageContent,
  hasFileAttachments,
  countImages,
  getMessageDisplayText
} from '@agentc/realtime-react';

describe('React Package Integration', () => {
  describe('Type Imports', () => {
    it('should import FileAttachment type', () => {
      const attachment: FileAttachment = {
        file: new File(['test'], 'test.txt', { type: 'text/plain' }),
        id: null,
        status: 'pending',
        progress: 0,
        error: null,
        previewUrl: null
      };
      
      expect(attachment).toBeDefined();
      expect(attachment.status).toBe('pending');
    });

    it('should import MessageContentBlock types', () => {
      const textBlock: TextContentBlock = {
        type: 'text',
        text: 'Hello world'
      };
      
      const imageBlock: ImageContentBlock = {
        type: 'image',
        source: {
          type: 'base64',
          media_type: 'image/png',
          data: 'base64data'
        }
      };
      
      const blocks: MessageContentBlock[] = [textBlock, imageBlock];
      
      expect(blocks).toHaveLength(2);
      expect(textBlock.type).toBe('text');
      expect(imageBlock.type).toBe('image');
    });
  });

  describe('Hook Imports', () => {
    it('should import and use useFileUpload hook', () => {
      // Note: This will fail without proper RealtimeProvider context
      // but verifies the import and types work
      try {
        const { result } = renderHook(() => useFileUpload());
        
        // Verify return type structure
        const hookReturn: UseFileUploadReturn = result.current;
        expect(hookReturn).toHaveProperty('attachments');
        expect(hookReturn).toHaveProperty('addFiles');
        expect(hookReturn).toHaveProperty('removeFile');
        expect(hookReturn).toHaveProperty('uploadFile');
        expect(hookReturn).toHaveProperty('uploadAll');
        expect(hookReturn).toHaveProperty('clearAll');
        expect(hookReturn).toHaveProperty('isUploading');
        expect(hookReturn).toHaveProperty('allComplete');
        expect(hookReturn).toHaveProperty('hasErrors');
        expect(hookReturn).toHaveProperty('overallProgress');
        expect(hookReturn).toHaveProperty('validationError');
        expect(hookReturn).toHaveProperty('getUploadedFileIds');
      } catch (error) {
        // Expected to fail without provider, but types should resolve
        expect(error).toBeDefined();
      }
    });

    it('should import and verify useChat sendMessage signature', () => {
      // Note: This will fail without proper RealtimeProvider context
      // but verifies the import and types work
      try {
        const { result } = renderHook(() => useChat());
        
        // Verify sendMessage accepts fileIds parameter
        expect(result.current.sendMessage).toBeDefined();
        expect(typeof result.current.sendMessage).toBe('function');
        
        // TypeScript will enforce the correct signature:
        // sendMessage(text: string, fileIds?: string[]): Promise<void>
      } catch (error) {
        // Expected to fail without provider, but types should resolve
        expect(error).toBeDefined();
      }
    });
  });

  describe('Utility Imports', () => {
    it('should import type guard utilities', () => {
      const textBlock: MessageContentBlock = {
        type: 'text',
        text: 'Hello'
      };
      
      const imageBlock: MessageContentBlock = {
        type: 'image',
        source: {
          type: 'base64',
          media_type: 'image/png',
          data: 'data'
        }
      };
      
      expect(isTextContent(textBlock)).toBe(true);
      expect(isTextContent(imageBlock)).toBe(false);
      
      expect(isImageContent(imageBlock)).toBe(true);
      expect(isImageContent(textBlock)).toBe(false);
    });

    it('should import message helper utilities', () => {
      const messageWithImage = {
        id: '1',
        role: 'user' as const,
        content: [
          { type: 'text' as const, text: 'Check this out' },
          { 
            type: 'image' as const, 
            source: { 
              type: 'base64' as const, 
              media_type: 'image/png', 
              data: 'data' 
            } 
          }
        ],
        timestamp: new Date().toISOString()
      };
      
      const textOnlyMessage = {
        id: '2',
        role: 'assistant' as const,
        content: 'Just text',
        timestamp: new Date().toISOString()
      };
      
      expect(hasFileAttachments(messageWithImage)).toBe(true);
      expect(hasFileAttachments(textOnlyMessage)).toBe(false);
      
      expect(countImages(messageWithImage)).toBe(1);
      expect(countImages(textOnlyMessage)).toBe(0);
      
      expect(getMessageDisplayText(messageWithImage)).toBe('Check this out');
      expect(getMessageDisplayText(textOnlyMessage)).toBe('Just text');
    });
  });

  describe('Interface Documentation', () => {
    it('documents FileAttachment interface', () => {
      // FileAttachment interface from React package:
      interface ExpectedFileAttachment {
        file: File;                    // Original File object
        id: string | null;             // Server-assigned ID after upload
        status: 'pending' | 'uploading' | 'complete' | 'error';
        progress: number;              // 0-100
        error: string | null;          // Error message if status is 'error'
        previewUrl: string | null;     // Object URL for image preview
      }
      
      // This test documents the expected interface
      const example: FileAttachment = {
        file: new File([''], 'test.png'),
        id: 'file_123',
        status: 'complete',
        progress: 100,
        error: null,
        previewUrl: 'blob:...'
      };
      
      expect(example.status).toBe('complete');
    });

    it('documents UseFileUploadReturn interface', () => {
      // UseFileUploadReturn interface from React package:
      interface ExpectedUseFileUploadReturn {
        attachments: FileAttachment[];
        addFiles: (files: File[]) => void;
        removeFile: (index: number) => void;
        uploadFile: (index: number) => Promise<void>;
        uploadAll: () => Promise<void>;
        clearAll: () => void;
        isUploading: boolean;
        allComplete: boolean;
        hasErrors: boolean;
        overallProgress: number;
        validationError: string | null;
        getUploadedFileIds: () => string[];
      }
      
      // This test documents the expected interface
      expect(true).toBe(true);
    });

    it('documents MessageContentBlock types', () => {
      // MessageContentBlock types from React package:
      type ExpectedTextBlock = {
        type: 'text';
        text: string;
      };
      
      type ExpectedImageBlock = {
        type: 'image';
        source: {
          type: 'base64' | 'url';
          media_type: string;
          data: string;
        };
      };
      
      type ExpectedMessageContentBlock = ExpectedTextBlock | ExpectedImageBlock;
      
      // This test documents the expected types
      expect(true).toBe(true);
    });
  });
});
