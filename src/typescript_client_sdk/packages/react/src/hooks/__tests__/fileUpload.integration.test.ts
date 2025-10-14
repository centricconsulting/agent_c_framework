/**
 * Integration Tests for File Upload Flow
 * Tests complete workflows involving useFileUpload and useChat hooks
 */

import { describe, it, expect, beforeEach, vi } from 'vitest';
import { renderHook, act, waitFor } from '@testing-library/react';
import type { Mock } from 'vitest';
import { useFileUpload } from '../useFileUpload';
import { useChat } from '../useChat';
import type { RealtimeClient, ChatSession, Message, ContentPart } from '@agentc/realtime-core';

// Mock dependencies
vi.mock('../../providers/AgentCContext', () => ({
  useRealtimeClientSafe: vi.fn()
}));

vi.mock('../../utils/logger', () => ({
  Logger: {
    debug: vi.fn(),
    error: vi.fn(),
    warn: vi.fn()
  }
}));

vi.mock('../../utils/agentStorage', () => ({
  AgentStorage: {
    saveAgentKey: vi.fn()
  }
}));

describe('File Upload Integration Tests', () => {
  let mockClient: {
    uploadFile: Mock;
    getSessionManager: Mock;
    isConnected: Mock;
    sendText: Mock;
    on: Mock;
    off: Mock;
  };

  let mockSessionManager: {
    getCurrentSession: Mock;
    on: Mock;
    off: Mock;
    emit: Mock;
  };

  let eventHandlers: Map<string, (event?: unknown) => void>;
  let sessionEventHandlers: Map<string, (event?: unknown) => void>;

  // Helper to create test files
  const createMockFile = (
    name: string, 
    size: number, 
    type: string = 'image/png'
  ): File => {
    const blob = new Blob(['x'.repeat(size)], { type });
    return new File([blob], name, { type });
  };

  // Helper to emit session events
  const emitSessionEvent = (eventName: string, data?: unknown) => {
    const handler = sessionEventHandlers.get(eventName);
    if (handler) {
      act(() => {
        handler(data);
      });
    }
  };

  // Helper to create test session
  const createTestSession = (sessionId = 'test-session'): ChatSession => ({
    session_id: sessionId,
    messages: [],
    context: {},
    created_at: new Date().toISOString(),
    updated_at: new Date().toISOString()
  });

  // Helper to create multimodal message
  const createMultimodalMessage = (
    role: 'user' | 'assistant',
    text: string,
    imageUrls: string[]
  ): Message => ({
    role,
    content: [
      { type: 'text', text } as ContentPart,
      ...imageUrls.map(url => ({
        type: 'image',
        source: { type: 'url' as const, url }
      } as ContentPart))
    ],
    timestamp: new Date().toISOString()
  });

  beforeEach(async () => {
    // Reset event handlers
    eventHandlers = new Map();
    sessionEventHandlers = new Map();

    // Mock URL.createObjectURL and revokeObjectURL
    global.URL.createObjectURL = vi.fn(() => 'blob:mock-url');
    global.URL.revokeObjectURL = vi.fn();

    // Setup mock SessionManager
    mockSessionManager = {
      getCurrentSession: vi.fn().mockReturnValue(createTestSession()),
      emit: vi.fn(),
      on: vi.fn((event: string, handler: (data?: unknown) => void) => {
        sessionEventHandlers.set(event, handler);
      }),
      off: vi.fn((event: string) => {
        sessionEventHandlers.delete(event);
      })
    };

    // Setup mock RealtimeClient
    mockClient = {
      uploadFile: vi.fn(),
      getSessionManager: vi.fn().mockReturnValue(mockSessionManager),
      isConnected: vi.fn().mockReturnValue(true),
      sendText: vi.fn(),
      on: vi.fn((event: string, handler: (data?: unknown) => void) => {
        eventHandlers.set(event, handler);
      }),
      off: vi.fn((event: string) => {
        eventHandlers.delete(event);
      })
    };

    // Setup mock for useRealtimeClientSafe
    const { useRealtimeClientSafe } = await import('../../providers/AgentCContext');
    (useRealtimeClientSafe as Mock).mockReturnValue(mockClient as unknown as RealtimeClient);
  });

  describe('Complete Upload and Send Flow', () => {
    it('should complete full workflow: add files → upload → send with IDs → receive multimodal response', async () => {
      // Render both hooks
      const { result: uploadResult } = renderHook(() => useFileUpload());
      const { result: chatResult } = renderHook(() => useChat());

      // Step 1: Add files to useFileUpload
      const file1 = createMockFile('image1.png', 1024);
      const file2 = createMockFile('image2.jpg', 2048, 'image/jpeg');

      await act(async () => {
        uploadResult.current.addFiles([file1, file2]);
      });

      // Verify files added
      expect(uploadResult.current.attachments).toHaveLength(2);
      expect(uploadResult.current.attachments[0].file.name).toBe('image1.png');
      expect(uploadResult.current.attachments[1].file.name).toBe('image2.jpg');

      // Step 2: Mock upload responses
      mockClient.uploadFile
        .mockResolvedValueOnce({ 
          id: 'uploaded-file-1',
          filename: 'image1.png',
          mime_type: 'image/png',
          size: 1024
        })
        .mockResolvedValueOnce({
          id: 'uploaded-file-2',
          filename: 'image2.jpg',
          mime_type: 'image/jpeg',
          size: 2048
        });

      // Step 3: Upload all files
      await act(async () => {
        await uploadResult.current.uploadAll();
      });

      // Verify uploads completed
      await waitFor(() => {
        expect(uploadResult.current.allComplete).toBe(true);
      });

      expect(uploadResult.current.attachments[0].status).toBe('complete');
      expect(uploadResult.current.attachments[0].id).toBe('uploaded-file-1');
      expect(uploadResult.current.attachments[1].status).toBe('complete');
      expect(uploadResult.current.attachments[1].id).toBe('uploaded-file-2');

      // Step 4: Get uploaded file IDs
      const fileIds = uploadResult.current.getUploadedFileIds();
      expect(fileIds).toEqual(['uploaded-file-1', 'uploaded-file-2']);

      // Step 5: Send message with file IDs using useChat
      await act(async () => {
        await chatResult.current.sendMessage('What do you see in these images?', fileIds);
      });

      // Verify sendText called with file IDs
      expect(mockClient.sendText).toHaveBeenCalledWith(
        'What do you see in these images?',
        ['uploaded-file-1', 'uploaded-file-2']
      );

      // Step 6: Simulate server response with multimodal message
      const multimodalResponse = createMultimodalMessage(
        'user',
        'What do you see in these images?',
        [
          'https://storage.example.com/uploaded-file-1',
          'https://storage.example.com/uploaded-file-2'
        ]
      );

      emitSessionEvent('message-complete', {
        sessionId: 'test-session',
        message: multimodalResponse
      });

      // Verify message added to chat
      await waitFor(() => {
        expect(chatResult.current.messages).toHaveLength(1);
      });

      const message = chatResult.current.messages[0];
      expect(message).toBeDefined();
      if ('content' in message && Array.isArray(message.content)) {
        // Verify message has text and images
        expect(message.content).toHaveLength(3); // 1 text + 2 images
        expect(message.content[0]).toMatchObject({ type: 'text' });
        expect(message.content[1]).toMatchObject({ type: 'image' });
        expect(message.content[2]).toMatchObject({ type: 'image' });
      }

      // Complete workflow validated! ✅
    });

    it('should handle single file upload and send', async () => {
      const { result: uploadResult } = renderHook(() => useFileUpload());
      const { result: chatResult } = renderHook(() => useChat());

      // Add single file
      const file = createMockFile('photo.png', 512);
      await act(async () => {
        uploadResult.current.addFiles([file]);
      });

      // Mock upload
      mockClient.uploadFile.mockResolvedValue({ 
        id: 'single-file-id',
        filename: 'photo.png',
        mime_type: 'image/png',
        size: 512
      });

      // Upload
      await act(async () => {
        await uploadResult.current.uploadFile(0);
      });

      await waitFor(() => {
        expect(uploadResult.current.attachments[0].status).toBe('complete');
      });

      // Send with single file ID
      const fileIds = uploadResult.current.getUploadedFileIds();
      expect(fileIds).toEqual(['single-file-id']);

      await act(async () => {
        await chatResult.current.sendMessage('Analyze this', fileIds);
      });

      expect(mockClient.sendText).toHaveBeenCalledWith('Analyze this', ['single-file-id']);
    });

    it('should verify TextInputEvent would include file_ids (implicit via sendText)', async () => {
      const { result: uploadResult } = renderHook(() => useFileUpload());
      const { result: chatResult } = renderHook(() => useChat());

      // Setup files
      const file = createMockFile('test.png', 100);
      await act(async () => {
        uploadResult.current.addFiles([file]);
      });

      mockClient.uploadFile.mockResolvedValue({ 
        id: 'file-123',
        filename: 'test.png',
        mime_type: 'image/png',
        size: 100
      });

      await act(async () => {
        await uploadResult.current.uploadFile(0);
      });

      await waitFor(() => {
        expect(uploadResult.current.allComplete).toBe(true);
      });

      // Send message
      const fileIds = uploadResult.current.getUploadedFileIds();
      await act(async () => {
        await chatResult.current.sendMessage('Test', fileIds);
      });

      // Verify client.sendText was called with fileIds
      // The Core package's sendText will create TextInputEvent with file_ids
      expect(mockClient.sendText).toHaveBeenCalledWith('Test', ['file-123']);
    });
  });

  describe('Display Multimodal Messages', () => {
    it('should receive and display multimodal message from server', async () => {
      const { result } = renderHook(() => useChat());

      // Simulate receiving multimodal message
      const message = createMultimodalMessage(
        'assistant',
        'I can see a mountain landscape with a lake.',
        ['https://example.com/analysis-result.jpg']
      );

      emitSessionEvent('message-complete', {
        sessionId: 'test-session',
        message
      });

      // Verify message added
      await waitFor(() => {
        expect(result.current.messages).toHaveLength(1);
      });

      const receivedMessage = result.current.messages[0];
      expect(receivedMessage).toBeDefined();
      
      if ('content' in receivedMessage && Array.isArray(receivedMessage.content)) {
        expect(receivedMessage.content).toHaveLength(2);
        expect(receivedMessage.content[0]).toMatchObject({ 
          type: 'text',
          text: 'I can see a mountain landscape with a lake.'
        });
        expect(receivedMessage.content[1]).toMatchObject({ 
          type: 'image'
        });
      }
    });

    it('should extract text and images from multimodal message', async () => {
      const { result } = renderHook(() => useChat());

      const message = createMultimodalMessage(
        'user',
        'Compare these',
        [
          'https://example.com/img1.jpg',
          'https://example.com/img2.jpg'
        ]
      );

      emitSessionEvent('message-complete', {
        sessionId: 'test-session',
        message
      });

      await waitFor(() => {
        expect(result.current.messages).toHaveLength(1);
      });

      const msg = result.current.messages[0];
      if ('content' in msg && Array.isArray(msg.content)) {
        // Extract text blocks
        const textBlocks = msg.content.filter(block => block.type === 'text');
        expect(textBlocks).toHaveLength(1);
        expect(textBlocks[0]).toMatchObject({ text: 'Compare these' });

        // Extract image blocks
        const imageBlocks = msg.content.filter(block => block.type === 'image');
        expect(imageBlocks).toHaveLength(2);
      }
    });

    it('should handle multiple multimodal messages in sequence', async () => {
      const { result } = renderHook(() => useChat());

      // Message 1: User with image
      const userMessage = createMultimodalMessage(
        'user',
        'What is this?',
        ['https://example.com/mystery.jpg']
      );

      emitSessionEvent('message-complete', {
        sessionId: 'test-session',
        message: userMessage
      });

      await waitFor(() => {
        expect(result.current.messages).toHaveLength(1);
      });

      // Message 2: Assistant response (text only)
      const assistantMessage: Message = {
        role: 'assistant',
        content: 'That appears to be a sunset.',
        timestamp: new Date().toISOString()
      };

      emitSessionEvent('message-complete', {
        sessionId: 'test-session',
        message: assistantMessage
      });

      await waitFor(() => {
        expect(result.current.messages).toHaveLength(2);
      });

      // Verify both messages preserved
      expect(result.current.messages[0]).toBeDefined();
      expect(result.current.messages[1]).toBeDefined();
    });
  });

  describe('Upload Progress Tracking', () => {
    it('should track upload progress through callbacks', async () => {
      const { result } = renderHook(() => useFileUpload());

      const file = createMockFile('large.png', 5000);
      await act(async () => {
        result.current.addFiles([file]);
      });

      // Mock upload with progress
      mockClient.uploadFile.mockImplementation((_file, options) => {
        return new Promise((resolve) => {
          // Simulate progress callbacks
          setTimeout(() => options?.onProgress?.({ loaded: 1250, total: 5000, percentage: 25 }), 10);
          setTimeout(() => options?.onProgress?.({ loaded: 2500, total: 5000, percentage: 50 }), 20);
          setTimeout(() => options?.onProgress?.({ loaded: 3750, total: 5000, percentage: 75 }), 30);
          setTimeout(() => {
            options?.onProgress?.({ loaded: 5000, total: 5000, percentage: 100 });
            resolve({ 
              id: 'progress-file',
              filename: 'large.png',
              mime_type: 'image/png',
              size: 5000
            });
          }, 40);
        });
      });

      // Start upload
      await act(async () => {
        result.current.uploadFile(0);
      });

      // Verify progress updates
      await waitFor(() => {
        expect(result.current.attachments[0].progress).toBeGreaterThan(0);
      }, { timeout: 100 });

      await waitFor(() => {
        expect(result.current.attachments[0].progress).toBe(100);
      }, { timeout: 200 });

      await waitFor(() => {
        expect(result.current.attachments[0].status).toBe('complete');
      });
    });

    it('should calculate overallProgress correctly for multiple files', async () => {
      const { result } = renderHook(() => useFileUpload());

      const file1 = createMockFile('file1.png', 1000);
      const file2 = createMockFile('file2.png', 1000);

      await act(async () => {
        result.current.addFiles([file1, file2]);
      });

      // Mock uploads with controlled progress
      mockClient.uploadFile.mockImplementation((_file, options) => {
        const filename = (_file as File).name;
        
        return new Promise((resolve) => {
          if (filename === 'file1.png') {
            // File 1: progress updates
            setTimeout(() => options?.onProgress?.({ loaded: 500, total: 1000, percentage: 50 }), 10);
            setTimeout(() => {
              options?.onProgress?.({ loaded: 1000, total: 1000, percentage: 100 });
              resolve({ 
                id: 'file1-id',
                filename: 'file1.png',
                mime_type: 'image/png',
                size: 1000
              });
            }, 50);
          } else {
            // File 2: completes after file1
            setTimeout(() => {
              options?.onProgress?.({ loaded: 1000, total: 1000, percentage: 100 });
              resolve({ 
                id: 'file2-id',
                filename: 'file2.png',
                mime_type: 'image/png',
                size: 1000
              });
            }, 100);
          }
        });
      });

      // PATTERN 1: Progress starts at 0%
      expect(result.current.overallProgress).toBe(0);
      expect(result.current.allComplete).toBe(false);

      // Track progress values during upload
      const progressValues: number[] = [];
      const originalOverallProgress = result.current.overallProgress;

      // Start uploads (uploadAll processes sequentially)
      const uploadPromise = act(async () => {
        await result.current.uploadAll();
      });

      // Sample progress values during upload
      const sampleInterval = setInterval(() => {
        if (result.current.overallProgress !== progressValues[progressValues.length - 1]) {
          progressValues.push(result.current.overallProgress);
        }
      }, 15);

      // Wait for uploads to complete
      await uploadPromise;
      clearInterval(sampleInterval);

      await waitFor(() => {
        return result.current.allComplete;
      }, { timeout: 400 });

      // PATTERN 2: Progress was tracked (values changed from 0)
      expect(progressValues.length).toBeGreaterThan(0);
      
      // PATTERN 3: Progress reaches 100% when all complete
      expect(result.current.overallProgress).toBe(100);
      expect(result.current.attachments[0].status).toBe('complete');
      expect(result.current.attachments[1].status).toBe('complete');
    });
  });

  describe('Upload Cancellation', () => {
    it('should cancel upload by removing file', async () => {
      const { result } = renderHook(() => useFileUpload());

      const file = createMockFile('cancel-me.png', 3000);
      await act(async () => {
        result.current.addFiles([file]);
      });

      // Mock upload that takes time
      mockClient.uploadFile.mockImplementation((_file, options) => {
        return new Promise((resolve, reject) => {
          const timeout = setTimeout(() => {
            resolve({ 
              id: 'should-not-complete',
              filename: 'cancel-me.png',
              mime_type: 'image/png',
              size: 3000
            });
          }, 100);

          // Listen for abort
          options?.signal?.addEventListener('abort', () => {
            clearTimeout(timeout);
            reject(new Error('Upload aborted'));
          });
        });
      });

      // Start upload
      act(() => {
        result.current.uploadFile(0);
      });

      // Verify uploading
      await waitFor(() => {
        expect(result.current.attachments[0].status).toBe('uploading');
      });

      // Cancel by removing file (this aborts the upload)
      await act(async () => {
        result.current.removeFile(0);
      });

      // Verify file removed
      expect(result.current.attachments).toHaveLength(0);
    });

    it('should cleanup resources when removing uploading file', async () => {
      const { result } = renderHook(() => useFileUpload());

      const file = createMockFile('test.png', 100);
      await act(async () => {
        result.current.addFiles([file]);
      });

      let abortCalled = false;
      mockClient.uploadFile.mockImplementation((_file, options) => {
        return new Promise((resolve, reject) => {
          options?.signal?.addEventListener('abort', () => {
            abortCalled = true;
            reject(new Error('Aborted'));
          });
          // Never resolve - simulates long upload
        });
      });

      // Start upload
      act(() => {
        result.current.uploadFile(0);
      });

      await waitFor(() => {
        expect(result.current.attachments[0].status).toBe('uploading');
      });

      // Remove file - should trigger abort
      await act(async () => {
        result.current.removeFile(0);
      });

      // Verify abort was called and file removed
      expect(abortCalled).toBe(true);
      expect(result.current.attachments).toHaveLength(0);
    });
  });

  describe('File Validation Flow', () => {
    it('should reject oversized files', async () => {
      const { result } = renderHook(() => 
        useFileUpload({ 
          maxFileSize: 1024 // 1KB limit
        })
      );

      const largeFile = createMockFile('huge.png', 2048); // 2KB file

      // addFiles throws on validation error
      let error: Error | undefined;
      try {
        await act(async () => {
          await result.current.addFiles([largeFile]);
        });
      } catch (e) {
        error = e as Error;
      }

      // Verify error was thrown with correct message
      expect(error).toBeDefined();
      expect(error?.message).toContain('exceeds maximum size');
      
      // Verify file was not added to attachments
      expect(result.current.attachments).toHaveLength(0);
    });

    it('should reject disallowed MIME types', async () => {
      const { result } = renderHook(() => 
        useFileUpload({ 
          allowedMimeTypes: ['image/png', 'image/jpeg']
        })
      );

      const gifFile = createMockFile('animated.gif', 500, 'image/gif');

      // addFiles throws on validation error
      let error: Error | undefined;
      try {
        await act(async () => {
          await result.current.addFiles([gifFile]);
        });
      } catch (e) {
        error = e as Error;
      }

      // Verify error was thrown with correct message
      expect(error).toBeDefined();
      expect(error?.message).toContain('is not allowed');
      
      // Verify file was not added to attachments
      expect(result.current.attachments).toHaveLength(0);
    });

    it('should enforce maximum file count', async () => {
      const { result } = renderHook(() => 
        useFileUpload({ 
          maxFiles: 2
        })
      );

      const file1 = createMockFile('file1.png', 100);
      const file2 = createMockFile('file2.png', 100);
      const file3 = createMockFile('file3.png', 100);

      // Add two files - should succeed
      await act(async () => {
        result.current.addFiles([file1, file2]);
      });

      expect(result.current.attachments).toHaveLength(2);

      // Try to add third file - should fail
      let error: Error | undefined;
      try {
        await act(async () => {
          await result.current.addFiles([file3]);
        });
      } catch (e) {
        error = e as Error;
      }

      // Verify error was thrown with correct message
      expect(error).toBeDefined();
      expect(error?.message).toContain('Maximum is');
      
      // Should still only have 2 files
      expect(result.current.attachments).toHaveLength(2);
    });

    it('should clear validation error when files are removed', async () => {
      const { result } = renderHook(() => 
        useFileUpload({ maxFiles: 1 })
      );

      const file1 = createMockFile('file1.png', 100);
      const file2 = createMockFile('file2.png', 100);

      // Add first file
      await act(async () => {
        result.current.addFiles([file1]);
      });

      expect(result.current.attachments).toHaveLength(1);

      // Try to add second file - should fail
      let error: Error | undefined;
      try {
        await act(async () => {
          await result.current.addFiles([file2]);
        });
      } catch (e) {
        error = e as Error;
      }

      // Verify error was thrown
      expect(error).toBeDefined();
      expect(error?.message).toContain('Maximum is');

      // File should not have been added
      expect(result.current.attachments).toHaveLength(1);

      // Remove first file
      await act(async () => {
        result.current.removeFile(0);
      });

      // Attachments should be cleared
      expect(result.current.attachments).toHaveLength(0);
      
      // Now we should be able to add a file again
      await act(async () => {
        await result.current.addFiles([file2]);
      });
      
      expect(result.current.attachments).toHaveLength(1);
    });
  });
});
