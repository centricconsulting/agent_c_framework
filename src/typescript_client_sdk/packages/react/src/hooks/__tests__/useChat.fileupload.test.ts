/**
 * Tests for useChat hook - File Upload Extensions
 * Tests sendMessage with fileIds parameter and multimodal message handling
 */

import { describe, it, expect, beforeEach, vi } from 'vitest';
import { renderHook, act, waitFor } from '@testing-library/react';
import type { Mock } from 'vitest';
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

describe('useChat - File Upload Extensions', () => {
  // Test utilities
  let mockClient: {
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

  // Helper to emit client events
  const emitClientEvent = (eventName: string, data?: unknown) => {
    const handler = eventHandlers.get(eventName);
    if (handler) {
      act(() => {
        handler(data);
      });
    }
  };

  // Helper to emit session manager events
  const emitSessionEvent = (eventName: string, data?: unknown) => {
    const handler = sessionEventHandlers.get(eventName);
    if (handler) {
      act(() => {
        handler(data);
      });
    }
  };

  // Helper to create test session
  const createTestSession = (
    sessionId = 'test-session',
    messages: Message[] = []
  ): ChatSession => ({
    session_id: sessionId,
    messages,
    context: {},
    created_at: new Date().toISOString(),
    updated_at: new Date().toISOString()
  });

  // Helper to create text-only message
  const createTextMessage = (
    role: 'user' | 'assistant' | 'system',
    text: string
  ): Message => ({
    role,
    content: text,
    timestamp: new Date().toISOString()
  });

  // Helper to create multimodal message (Anthropic format)
  const createMultimodalMessage = (
    role: 'user' | 'assistant',
    textParts: string[],
    imageSources: Array<{ type: 'base64' | 'url'; data?: string; url?: string; media_type: string }>
  ): Message => {
    const content: ContentPart[] = [];
    
    textParts.forEach(text => {
      content.push({ type: 'text', text } as ContentPart);
    });
    
    imageSources.forEach(source => {
      content.push({ 
        type: 'image', 
        source 
      } as ContentPart);
    });
    
    return {
      role,
      content,
      timestamp: new Date().toISOString()
    };
  };

  beforeEach(async () => {
    // Initialize event handler storage
    eventHandlers = new Map();
    sessionEventHandlers = new Map();

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

  describe('sendMessage with File IDs', () => {
    it('should call client.sendText with fileIds array', async () => {
      const { result } = renderHook(() => useChat());

      const fileIds = ['file-123', 'file-456'];
      await act(async () => {
        await result.current.sendMessage('Check these images', fileIds);
      });

      expect(mockClient.sendText).toHaveBeenCalledWith('Check these images', fileIds);
      expect(mockClient.sendText).toHaveBeenCalledTimes(1);
    });

    it('should call client.sendText with single file ID', async () => {
      const { result } = renderHook(() => useChat());

      const fileIds = ['file-123'];
      await act(async () => {
        await result.current.sendMessage('Look at this', fileIds);
      });

      expect(mockClient.sendText).toHaveBeenCalledWith('Look at this', fileIds);
    });

    it('should call client.sendText with multiple file IDs', async () => {
      const { result } = renderHook(() => useChat());

      const fileIds = ['file-1', 'file-2', 'file-3', 'file-4'];
      await act(async () => {
        await result.current.sendMessage('Compare these four images', fileIds);
      });

      expect(mockClient.sendText).toHaveBeenCalledWith('Compare these four images', fileIds);
    });

    it('should handle empty fileIds array', async () => {
      const { result } = renderHook(() => useChat());

      await act(async () => {
        await result.current.sendMessage('Just text', []);
      });

      expect(mockClient.sendText).toHaveBeenCalledWith('Just text', []);
    });

    it('should pass fileIds when sending message with text and attachments', async () => {
      const { result } = renderHook(() => useChat());

      const text = 'What do you see in these images?';
      const fileIds = ['uploaded-file-1', 'uploaded-file-2'];

      await act(async () => {
        await result.current.sendMessage(text, fileIds);
      });

      expect(mockClient.sendText).toHaveBeenCalledWith(text, fileIds);
      expect(mockClient.sendText).toHaveBeenCalledTimes(1);
    });
  });

  describe('Backward Compatibility', () => {
    it('should send message without fileIds parameter', async () => {
      const { result } = renderHook(() => useChat());

      await act(async () => {
        await result.current.sendMessage('Hello without files');
      });

      // Should be called with text and undefined fileIds
      expect(mockClient.sendText).toHaveBeenCalledWith('Hello without files', undefined);
      expect(mockClient.sendText).toHaveBeenCalledTimes(1);
    });

    it('should call client.sendText with undefined fileIds when not provided', async () => {
      const { result } = renderHook(() => useChat());

      await act(async () => {
        // Call without second parameter
        await result.current.sendMessage('Plain text message');
      });

      // Verify undefined is passed for fileIds
      const calls = mockClient.sendText.mock.calls;
      expect(calls[0][0]).toBe('Plain text message');
      expect(calls[0][1]).toBeUndefined();
    });

    it('should maintain existing validation behavior', async () => {
      const { result } = renderHook(() => useChat());

      // Test empty message validation
      await expect(async () => {
        await act(async () => {
          await result.current.sendMessage('');
        });
      }).rejects.toThrow('Message cannot be empty');

      // Test whitespace-only message validation
      await expect(async () => {
        await act(async () => {
          await result.current.sendMessage('   ');
        });
      }).rejects.toThrow('Message cannot be empty');

      // sendText should not have been called
      expect(mockClient.sendText).not.toHaveBeenCalled();
    });

    it('should maintain existing connection check', async () => {
      mockClient.isConnected.mockReturnValue(false);
      const { result } = renderHook(() => useChat());

      await expect(async () => {
        await act(async () => {
          await result.current.sendMessage('Test', ['file-1']);
        });
      }).rejects.toThrow('Not connected to server');

      expect(mockClient.sendText).not.toHaveBeenCalled();
    });
  });

  describe('Multimodal Message Handling', () => {
    it('should handle text-only messages (existing behavior)', async () => {
      const { result } = renderHook(() => useChat());

      // Emit a text-only message from server
      const textMessage = createTextMessage('assistant', 'This is a text response');
      
      emitSessionEvent('message-complete', {
        sessionId: 'test-session',
        message: textMessage
      });

      await waitFor(() => {
        expect(result.current.messages).toHaveLength(1);
      });

      const message = result.current.messages[0];
      expect(message).toBeDefined();
      if ('content' in message) {
        expect(message.content).toBe('This is a text response');
      }
    });

    it('should handle messages with image content blocks (Anthropic format)', async () => {
      const { result } = renderHook(() => useChat());

      // Create multimodal message with Anthropic format
      const multimodalMessage = createMultimodalMessage(
        'user',
        ['What do you see in this image?'],
        [{
          type: 'base64',
          media_type: 'image/jpeg',
          data: 'base64encodedimagedata...'
        }]
      );

      emitSessionEvent('message-complete', {
        sessionId: 'test-session',
        message: multimodalMessage
      });

      await waitFor(() => {
        expect(result.current.messages).toHaveLength(1);
      });

      const message = result.current.messages[0];
      expect(message).toBeDefined();
      if ('content' in message && Array.isArray(message.content)) {
        expect(message.content).toHaveLength(2);
        expect(message.content[0]).toMatchObject({ type: 'text' });
        expect(message.content[1]).toMatchObject({ type: 'image' });
      }
    });

    it('should handle messages with mixed content (text + images)', async () => {
      const { result } = renderHook(() => useChat());

      // Create message with multiple text and image blocks
      const mixedMessage = createMultimodalMessage(
        'user',
        ['Compare ', ' and ', '. What are the differences?'],
        [
          {
            type: 'url',
            url: 'https://example.com/image1.jpg',
            media_type: 'image/jpeg'
          },
          {
            type: 'url',
            url: 'https://example.com/image2.jpg',
            media_type: 'image/jpeg'
          }
        ]
      );

      // Interleave text and images manually for proper mixed content
      mixedMessage.content = [
        { type: 'text', text: 'Compare ' } as ContentPart,
        { 
          type: 'image', 
          source: {
            type: 'url',
            url: 'https://example.com/image1.jpg'
          }
        } as ContentPart,
        { type: 'text', text: ' and ' } as ContentPart,
        { 
          type: 'image', 
          source: {
            type: 'url',
            url: 'https://example.com/image2.jpg'
          }
        } as ContentPart,
        { type: 'text', text: '. What are the differences?' } as ContentPart
      ];

      emitSessionEvent('message-complete', {
        sessionId: 'test-session',
        message: mixedMessage
      });

      await waitFor(() => {
        expect(result.current.messages).toHaveLength(1);
      });

      const message = result.current.messages[0];
      expect(message).toBeDefined();
      if ('content' in message && Array.isArray(message.content)) {
        expect(message.content).toHaveLength(5);
        
        // Verify alternating text and image blocks
        expect(message.content[0]).toMatchObject({ type: 'text' });
        expect(message.content[1]).toMatchObject({ type: 'image' });
        expect(message.content[2]).toMatchObject({ type: 'text' });
        expect(message.content[3]).toMatchObject({ type: 'image' });
        expect(message.content[4]).toMatchObject({ type: 'text' });
      }
    });

    it('should handle OpenAI format multimodal messages', async () => {
      const { result } = renderHook(() => useChat());

      // OpenAI uses image_url format
      const openAIMessage: Message = {
        role: 'user',
        content: [
          { type: 'text', text: 'Analyze this image' } as ContentPart,
          { 
            type: 'image_url', 
            image_url: {
              url: 'https://example.com/image.png',
              detail: 'high'
            }
          } as any // OpenAI format
        ],
        timestamp: new Date().toISOString()
      };

      emitSessionEvent('message-complete', {
        sessionId: 'test-session',
        message: openAIMessage
      });

      await waitFor(() => {
        expect(result.current.messages).toHaveLength(1);
      });

      const message = result.current.messages[0];
      expect(message).toBeDefined();
      if ('content' in message && Array.isArray(message.content)) {
        expect(message.content).toHaveLength(2);
        expect(message.content[0]).toMatchObject({ type: 'text' });
        // OpenAI format may be stored as-is or normalized
      }
    });

    it('should emit message-complete events with vendor-specific formats', async () => {
      const { result } = renderHook(() => useChat());

      // Test Anthropic vendor format
      const anthropicMessage = createMultimodalMessage(
        'assistant',
        ['I can see a landscape with mountains.'],
        []
      );

      emitSessionEvent('message-complete', {
        sessionId: 'test-session',
        message: anthropicMessage,
        vendor: 'anthropic'
      });

      await waitFor(() => {
        expect(result.current.messages).toHaveLength(1);
      });

      // Message should be stored regardless of vendor
      expect(result.current.messages[0]).toBeDefined();
    });

    it('should handle image-only messages (no text)', async () => {
      const { result } = renderHook(() => useChat());

      const imageOnlyMessage: Message = {
        role: 'user',
        content: [
          { 
            type: 'image', 
            source: {
              type: 'base64',
              media_type: 'image/png',
              data: 'imagedata...'
            }
          } as ContentPart
        ],
        timestamp: new Date().toISOString()
      };

      emitSessionEvent('message-complete', {
        sessionId: 'test-session',
        message: imageOnlyMessage
      });

      await waitFor(() => {
        expect(result.current.messages).toHaveLength(1);
      });

      const message = result.current.messages[0];
      expect(message).toBeDefined();
      if ('content' in message && Array.isArray(message.content)) {
        expect(message.content).toHaveLength(1);
        expect(message.content[0]).toMatchObject({ type: 'image' });
      }
    });

    it('should handle multiple multimodal messages in sequence', async () => {
      const { result } = renderHook(() => useChat());

      // User sends multimodal message
      const userMessage = createMultimodalMessage(
        'user',
        ['Describe this image'],
        [{
          type: 'base64',
          media_type: 'image/jpeg',
          data: 'imagedata...'
        }]
      );

      emitSessionEvent('message-complete', {
        sessionId: 'test-session',
        message: userMessage
      });

      await waitFor(() => {
        expect(result.current.messages).toHaveLength(1);
      });

      // Assistant responds with text
      const assistantMessage = createTextMessage('assistant', 'This is a beautiful sunset.');

      emitSessionEvent('message-complete', {
        sessionId: 'test-session',
        message: assistantMessage
      });

      await waitFor(() => {
        expect(result.current.messages).toHaveLength(2);
      });

      // Verify both messages stored correctly
      expect(result.current.messages[0]).toBeDefined();
      expect(result.current.messages[1]).toBeDefined();
    });
  });

  describe('Integration: Upload and Send Flow', () => {
    it('should support complete upload-to-send workflow', async () => {
      const { result } = renderHook(() => useChat());

      // Simulate: User uploaded files and got IDs back
      const uploadedFileIds = ['file-abc123', 'file-def456'];
      
      // User sends message with uploaded file IDs
      await act(async () => {
        await result.current.sendMessage('What are in these images?', uploadedFileIds);
      });

      // Verify sendText called with file IDs
      expect(mockClient.sendText).toHaveBeenCalledWith(
        'What are in these images?',
        uploadedFileIds
      );

      // Simulate server response with multimodal message
      const responseMessage = createMultimodalMessage(
        'user',
        ['What are in these images?'],
        [
          {
            type: 'url',
            url: 'https://storage.example.com/file-abc123',
            media_type: 'image/jpeg'
          },
          {
            type: 'url',
            url: 'https://storage.example.com/file-def456',
            media_type: 'image/png'
          }
        ]
      );

      emitSessionEvent('message-complete', {
        sessionId: 'test-session',
        message: responseMessage
      });

      await waitFor(() => {
        expect(result.current.messages).toHaveLength(1);
      });

      // Verify message has images
      const message = result.current.messages[0];
      if ('content' in message && Array.isArray(message.content)) {
        const imageBlocks = message.content.filter(block => block.type === 'image');
        expect(imageBlocks).toHaveLength(2);
      }
    });
  });
});
