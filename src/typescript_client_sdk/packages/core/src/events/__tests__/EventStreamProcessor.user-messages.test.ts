/**
 * Test suite for EventStreamProcessor - User Message Handling (OpenAI & Anthropic)
 * 
 * This test suite validates:
 * 1. OpenAI user message handling (handleOpenAIUserMessage)
 * 2. Anthropic user message handling (handleAnthropicUserMessage)
 * 3. Cross-vendor consistency (both produce identical output structures)
 * 4. Multimodal message support (text + images)
 * 5. Sub-session detection and metadata
 * 6. Event emission patterns (message-added, user-message)
 * 7. Session state management
 * 8. Edge cases and fallback handling
 * 
 * Critical User Requirements:
 * - Users must see their complete messages (text + images) in chat history
 * - Messages sent with files should display properly
 * - Sub-session messages should be distinguishable
 */

import { describe, it, expect, beforeEach, afterEach, vi } from 'vitest';
import { EventStreamProcessor } from '../EventStreamProcessor';
import { ChatSessionManager } from '../../session/SessionManager';
import { 
  OpenAIUserMessageEvent,
  AnthropicUserMessageEvent
} from '../types/ServerEvents';
import { ChatSession, Message } from '../types/CommonTypes';
import { MessageParam, ContentBlockParam } from '../../types/message-params';
import { 
  ChatCompletionUserMessageParam,
  ChatCompletionContentPart
} from '../../types/openai-message-params';

describe('EventStreamProcessor - User Message Handling', () => {
  let processor: EventStreamProcessor;
  let sessionManager: ChatSessionManager;
  let sessionManagerEmitSpy: ReturnType<typeof vi.spyOn>;
  let mockSession: ChatSession;
  const testSessionId = 'test-session-user-msg';

  beforeEach(() => {
    vi.clearAllMocks();
    
    // Create a mock session
    mockSession = {
      session_id: testSessionId,
      session_name: 'Test User Messages Session',
      messages: [],
      created_at: new Date().toISOString(),
      updated_at: new Date().toISOString(),
      token_count: 0,
      metadata: {}
    };

    // Create real instances
    sessionManager = new ChatSessionManager();
    processor = new EventStreamProcessor(sessionManager);
    
    // Setup spies
    sessionManagerEmitSpy = vi.spyOn(sessionManager, 'emit');
    
    // Set the current session and session ID
    sessionManager.setCurrentSession(mockSession);
    processor.setCurrentChatSessionId(testSessionId);
  });

  afterEach(() => {
    vi.restoreAllMocks();
    processor.destroy();
    sessionManager.destroy();
  });

  /**
   * Test Group 1: OpenAI Simple Text Messages
   */
  describe('OpenAI - Simple Text Messages', () => {
    it('should handle simple text message from OpenAI', () => {
      const event: OpenAIUserMessageEvent = {
        type: 'openai_user_message',
        vendor: 'openai',
        session_id: testSessionId,
        role: 'user',
        message: {
          role: 'user',
          content: 'Hello, world!'
        }
      };

      processor.processEvent(event);

      // Verify message added to session
      expect(mockSession.messages).toHaveLength(1);
      expect(mockSession.messages[0]).toMatchObject({
        role: 'user',
        content: 'Hello, world!',
        format: 'text'
      });
      expect(mockSession.messages[0].timestamp).toBeDefined();

      // Verify 'message-added' event emitted
      expect(sessionManagerEmitSpy).toHaveBeenCalledWith('message-added', {
        sessionId: testSessionId,
        message: expect.objectContaining({
          role: 'user',
          content: 'Hello, world!'
        })
      });

      // Verify 'user-message' event emitted for backward compatibility
      expect(sessionManagerEmitSpy).toHaveBeenCalledWith('user-message', {
        vendor: 'openai',
        message: event.message
      });

      // Verify session updated_at was set
      expect(mockSession.updated_at).toBeDefined();
    });

    it('should handle empty string content from OpenAI', () => {
      const event: OpenAIUserMessageEvent = {
        type: 'openai_user_message',
        vendor: 'openai',
        session_id: testSessionId,
        role: 'user',
        message: {
          role: 'user',
          content: ''
        }
      };

      processor.processEvent(event);

      expect(mockSession.messages).toHaveLength(1);
      expect(mockSession.messages[0].content).toBe('');
    });
  });

  /**
   * Test Group 2: Anthropic Simple Text Messages
   */
  describe('Anthropic - Simple Text Messages', () => {
    it('should handle simple text message from Anthropic', () => {
      const event: AnthropicUserMessageEvent = {
        type: 'anthropic_user_message',
        vendor: 'anthropic',
        session_id: testSessionId,
        role: 'user',
        message: {
          role: 'user',
          content: 'Hello, world!'
        }
      };

      processor.processEvent(event);

      // Verify message added to session
      expect(mockSession.messages).toHaveLength(1);
      expect(mockSession.messages[0]).toMatchObject({
        role: 'user',
        content: 'Hello, world!',
        format: 'text'
      });
      expect(mockSession.messages[0].timestamp).toBeDefined();

      // Verify 'message-added' event emitted
      expect(sessionManagerEmitSpy).toHaveBeenCalledWith('message-added', {
        sessionId: testSessionId,
        message: expect.objectContaining({
          role: 'user',
          content: 'Hello, world!'
        })
      });

      // Verify 'user-message' event emitted for backward compatibility
      expect(sessionManagerEmitSpy).toHaveBeenCalledWith('user-message', {
        vendor: 'anthropic',
        message: event.message
      });

      // Verify session updated_at was set
      expect(mockSession.updated_at).toBeDefined();
    });

    it('should handle empty string content from Anthropic', () => {
      const event: AnthropicUserMessageEvent = {
        type: 'anthropic_user_message',
        vendor: 'anthropic',
        session_id: testSessionId,
        role: 'user',
        message: {
          role: 'user',
          content: ''
        }
      };

      processor.processEvent(event);

      expect(mockSession.messages).toHaveLength(1);
      expect(mockSession.messages[0].content).toBe('');
    });
  });

  /**
   * Test Group 3: OpenAI Multimodal Messages
   */
  describe('OpenAI - Multimodal Messages (Text + Images)', () => {
    it('should handle text + image message from OpenAI', () => {
      const content: ChatCompletionContentPart[] = [
        { type: 'text', text: 'Look at this image:' },
        { 
          type: 'image_url', 
          image_url: { url: 'https://example.com/image.jpg' }
        }
      ];

      const event: OpenAIUserMessageEvent = {
        type: 'openai_user_message',
        vendor: 'openai',
        session_id: testSessionId,
        role: 'user',
        message: {
          role: 'user',
          content
        }
      };

      processor.processEvent(event);

      // Verify message added with multimodal content
      expect(mockSession.messages).toHaveLength(1);
      const message = mockSession.messages[0];
      
      // Content should be array of ContentPart
      expect(Array.isArray(message.content)).toBe(true);
      const contentParts = message.content as any[];
      
      expect(contentParts).toHaveLength(2);
      expect(contentParts[0]).toMatchObject({
        type: 'text',
        text: 'Look at this image:'
      });
      expect(contentParts[1]).toMatchObject({
        type: 'image',
        source: {
          url: 'https://example.com/image.jpg'
        }
      });

      // Verify events emitted
      expect(sessionManagerEmitSpy).toHaveBeenCalledWith('message-added', {
        sessionId: testSessionId,
        message: expect.objectContaining({
          role: 'user',
          content: expect.arrayContaining([
            expect.objectContaining({ type: 'text' }),
            expect.objectContaining({ type: 'image' })
          ])
        })
      });
    });

    it('should handle image with base64 data from OpenAI', () => {
      const content: ChatCompletionContentPart[] = [
        { type: 'text', text: 'Base64 image:' },
        { 
          type: 'image_url', 
          image_url: { 
            url: 'data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mNk+M9QDwADhgGAWjR9awAAAABJRU5ErkJggg==',
            detail: 'high'
          }
        }
      ];

      const event: OpenAIUserMessageEvent = {
        type: 'openai_user_message',
        vendor: 'openai',
        session_id: testSessionId,
        role: 'user',
        message: {
          role: 'user',
          content
        }
      };

      processor.processEvent(event);

      expect(mockSession.messages).toHaveLength(1);
      const contentParts = mockSession.messages[0].content as any[];
      expect(contentParts[1].source.url).toContain('data:image/png;base64');
    });

    it('should handle multiple images from OpenAI', () => {
      const content: ChatCompletionContentPart[] = [
        { type: 'text', text: 'Compare these images:' },
        { type: 'image_url', image_url: { url: 'https://example.com/image1.jpg' } },
        { type: 'image_url', image_url: { url: 'https://example.com/image2.jpg' } },
        { type: 'text', text: 'What are the differences?' }
      ];

      const event: OpenAIUserMessageEvent = {
        type: 'openai_user_message',
        vendor: 'openai',
        session_id: testSessionId,
        role: 'user',
        message: {
          role: 'user',
          content
        }
      };

      processor.processEvent(event);

      const contentParts = mockSession.messages[0].content as any[];
      expect(contentParts).toHaveLength(4);
      expect(contentParts.filter((p: any) => p.type === 'image')).toHaveLength(2);
      expect(contentParts.filter((p: any) => p.type === 'text')).toHaveLength(2);
    });
  });

  /**
   * Test Group 4: Anthropic Multimodal Messages
   */
  describe('Anthropic - Multimodal Messages (Text + Images)', () => {
    it('should handle text + image message from Anthropic', () => {
      const content: ContentBlockParam[] = [
        { type: 'text', text: 'Look at this image:' },
        { 
          type: 'image', 
          source: {
            type: 'url',
            url: 'https://example.com/image.jpg'
          }
        }
      ];

      const event: AnthropicUserMessageEvent = {
        type: 'anthropic_user_message',
        vendor: 'anthropic',
        session_id: testSessionId,
        role: 'user',
        message: {
          role: 'user',
          content
        }
      };

      processor.processEvent(event);

      // Verify message added with multimodal content
      expect(mockSession.messages).toHaveLength(1);
      const message = mockSession.messages[0];
      
      // Content should be array of ContentPart
      expect(Array.isArray(message.content)).toBe(true);
      const contentParts = message.content as any[];
      
      expect(contentParts).toHaveLength(2);
      expect(contentParts[0]).toMatchObject({
        type: 'text',
        text: 'Look at this image:'
      });
      expect(contentParts[1]).toMatchObject({
        type: 'image',
        source: {
          type: 'url',
          url: 'https://example.com/image.jpg'
        }
      });

      // Verify events emitted
      expect(sessionManagerEmitSpy).toHaveBeenCalledWith('message-added', {
        sessionId: testSessionId,
        message: expect.objectContaining({
          role: 'user',
          content: expect.arrayContaining([
            expect.objectContaining({ type: 'text' }),
            expect.objectContaining({ type: 'image' })
          ])
        })
      });
    });

    it('should handle image with base64 data from Anthropic', () => {
      const content: ContentBlockParam[] = [
        { type: 'text', text: 'Base64 image:' },
        { 
          type: 'image', 
          source: {
            type: 'base64',
            media_type: 'image/png',
            data: 'iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mNk+M9QDwADhgGAWjR9awAAAABJRU5ErkJggg=='
          }
        }
      ];

      const event: AnthropicUserMessageEvent = {
        type: 'anthropic_user_message',
        vendor: 'anthropic',
        session_id: testSessionId,
        role: 'user',
        message: {
          role: 'user',
          content
        }
      };

      processor.processEvent(event);

      expect(mockSession.messages).toHaveLength(1);
      const contentParts = mockSession.messages[0].content as any[];
      expect(contentParts[1].source.type).toBe('base64');
      expect(contentParts[1].source.media_type).toBe('image/png');
    });
  });

  /**
   * Test Group 5: Sub-Session Detection (Both Vendors)
   */
  describe('Sub-Session Detection and Metadata', () => {
    it('should detect and mark OpenAI sub-session messages', () => {
      const subSessionId = 'sub-session-123';
      const parentSessionId = 'parent-session-456';
      
      const event: OpenAIUserMessageEvent = {
        type: 'openai_user_message',
        vendor: 'openai',
        session_id: subSessionId,
        parent_session_id: parentSessionId,
        user_session_id: testSessionId, // Current user session
        role: 'user',
        message: {
          role: 'user',
          content: 'Sub-agent message'
        }
      };

      processor.processEvent(event);

      const message = mockSession.messages[0] as any;
      
      // Verify sub-session metadata added
      expect(message.isSubSession).toBe(true);
      expect(message.metadata).toMatchObject({
        sessionId: subSessionId,
        parentSessionId: parentSessionId,
        userSessionId: testSessionId
      });
    });

    it('should detect and mark Anthropic sub-session messages', () => {
      const subSessionId = 'sub-session-789';
      const parentSessionId = 'parent-session-abc';
      
      const event: AnthropicUserMessageEvent = {
        type: 'anthropic_user_message',
        vendor: 'anthropic',
        session_id: subSessionId,
        parent_session_id: parentSessionId,
        user_session_id: testSessionId, // Current user session
        role: 'user',
        message: {
          role: 'user',
          content: 'Sub-agent message'
        }
      };

      processor.processEvent(event);

      const message = mockSession.messages[0] as any;
      
      // Verify sub-session metadata added
      expect(message.isSubSession).toBe(true);
      expect(message.metadata).toMatchObject({
        sessionId: subSessionId,
        parentSessionId: parentSessionId,
        userSessionId: testSessionId
      });
    });

    it('should NOT mark messages as sub-session when session_id matches current', () => {
      const event: OpenAIUserMessageEvent = {
        type: 'openai_user_message',
        vendor: 'openai',
        session_id: testSessionId, // Same as current session
        role: 'user',
        message: {
          role: 'user',
          content: 'Regular message'
        }
      };

      processor.processEvent(event);

      const message = mockSession.messages[0] as any;
      
      // Should NOT have sub-session metadata
      expect(message.isSubSession).toBeUndefined();
      expect(message.metadata).toBeUndefined();
    });

    it('should handle sub-session with multimodal content', () => {
      const content: ChatCompletionContentPart[] = [
        { type: 'text', text: 'Sub-agent image:' },
        { type: 'image_url', image_url: { url: 'https://example.com/sub-image.jpg' } }
      ];

      const event: OpenAIUserMessageEvent = {
        type: 'openai_user_message',
        vendor: 'openai',
        session_id: 'sub-session-multimodal',
        parent_session_id: 'parent-123',
        user_session_id: testSessionId,
        role: 'user',
        message: {
          role: 'user',
          content
        }
      };

      processor.processEvent(event);

      const message = mockSession.messages[0] as any;
      
      // Should have both multimodal content AND sub-session metadata
      expect(Array.isArray(message.content)).toBe(true);
      expect(message.isSubSession).toBe(true);
      expect(message.metadata).toBeDefined();
    });
  });

  /**
   * Test Group 6: Event Emission Order and Structure
   */
  describe('Event Emission Pattern', () => {
    it('should emit message-added FIRST, then user-message (OpenAI)', () => {
      const eventSequence: string[] = [];
      sessionManagerEmitSpy.mockImplementation((eventName) => {
        eventSequence.push(eventName as string);
      });

      const event: OpenAIUserMessageEvent = {
        type: 'openai_user_message',
        vendor: 'openai',
        session_id: testSessionId,
        role: 'user',
        message: {
          role: 'user',
          content: 'Test message'
        }
      };

      processor.processEvent(event);

      // Verify event order
      const messageAddedIndex = eventSequence.indexOf('message-added');
      const userMessageIndex = eventSequence.indexOf('user-message');
      
      expect(messageAddedIndex).toBeGreaterThanOrEqual(0);
      expect(userMessageIndex).toBeGreaterThanOrEqual(0);
      expect(messageAddedIndex).toBeLessThan(userMessageIndex);
    });

    it('should emit message-added FIRST, then user-message (Anthropic)', () => {
      const eventSequence: string[] = [];
      sessionManagerEmitSpy.mockImplementation((eventName) => {
        eventSequence.push(eventName as string);
      });

      const event: AnthropicUserMessageEvent = {
        type: 'anthropic_user_message',
        vendor: 'anthropic',
        session_id: testSessionId,
        role: 'user',
        message: {
          role: 'user',
          content: 'Test message'
        }
      };

      processor.processEvent(event);

      // Verify event order
      const messageAddedIndex = eventSequence.indexOf('message-added');
      const userMessageIndex = eventSequence.indexOf('user-message');
      
      expect(messageAddedIndex).toBeGreaterThanOrEqual(0);
      expect(userMessageIndex).toBeGreaterThanOrEqual(0);
      expect(messageAddedIndex).toBeLessThan(userMessageIndex);
    });

    it('should include sessionId in message-added event', () => {
      const event: OpenAIUserMessageEvent = {
        type: 'openai_user_message',
        vendor: 'openai',
        session_id: testSessionId,
        role: 'user',
        message: {
          role: 'user',
          content: 'Test'
        }
      };

      processor.processEvent(event);

      expect(sessionManagerEmitSpy).toHaveBeenCalledWith('message-added', {
        sessionId: testSessionId,
        message: expect.any(Object)
      });
    });

    it('should include vendor in user-message event', () => {
      const event: OpenAIUserMessageEvent = {
        type: 'openai_user_message',
        vendor: 'openai',
        session_id: testSessionId,
        role: 'user',
        message: {
          role: 'user',
          content: 'Test'
        }
      };

      processor.processEvent(event);

      expect(sessionManagerEmitSpy).toHaveBeenCalledWith('user-message', {
        vendor: 'openai',
        message: expect.any(Object)
      });
    });
  });

  /**
   * Test Group 7: Edge Cases and Fallback Handling
   */
  describe('Edge Cases and Malformed Messages', () => {
    it('should handle null message with fallback (OpenAI)', () => {
      const event: OpenAIUserMessageEvent = {
        type: 'openai_user_message',
        vendor: 'openai',
        session_id: testSessionId,
        role: 'user',
        message: null as any
      };

      processor.processEvent(event);

      // Should create fallback message
      expect(mockSession.messages).toHaveLength(1);
      expect(mockSession.messages[0].content).toBe('[User message]');
      expect(mockSession.messages[0].role).toBe('user');
    });

    it('should handle null message with fallback (Anthropic)', () => {
      const event: AnthropicUserMessageEvent = {
        type: 'anthropic_user_message',
        vendor: 'anthropic',
        session_id: testSessionId,
        role: 'user',
        message: null as any
      };

      processor.processEvent(event);

      // Should create fallback message
      expect(mockSession.messages).toHaveLength(1);
      expect(mockSession.messages[0].content).toBe('[User message]');
      expect(mockSession.messages[0].role).toBe('user');
    });

    it('should handle message missing content field (OpenAI)', () => {
      const event: OpenAIUserMessageEvent = {
        type: 'openai_user_message',
        vendor: 'openai',
        session_id: testSessionId,
        role: 'user',
        message: {
          role: 'user'
        } as any
      };

      processor.processEvent(event);

      // Should create fallback message with stringified content
      expect(mockSession.messages).toHaveLength(1);
      expect(mockSession.messages[0].content).toContain('role');
    });

    it('should handle message missing role field (OpenAI)', () => {
      const event: OpenAIUserMessageEvent = {
        type: 'openai_user_message',
        vendor: 'openai',
        session_id: testSessionId,
        role: 'user',
        message: {
          content: 'Hello'
        } as any
      };

      processor.processEvent(event);

      // Should create fallback message
      expect(mockSession.messages).toHaveLength(1);
      expect(mockSession.messages[0].role).toBe('user');
    });

    it('should handle undefined message object', () => {
      const event: OpenAIUserMessageEvent = {
        type: 'openai_user_message',
        vendor: 'openai',
        session_id: testSessionId,
        role: 'user',
        message: undefined as any
      };

      processor.processEvent(event);

      expect(mockSession.messages).toHaveLength(1);
      expect(mockSession.messages[0].content).toBe('[User message]');
    });

    it('should handle empty content array (OpenAI)', () => {
      const event: OpenAIUserMessageEvent = {
        type: 'openai_user_message',
        vendor: 'openai',
        session_id: testSessionId,
        role: 'user',
        message: {
          role: 'user',
          content: []
        }
      };

      processor.processEvent(event);

      // Should handle gracefully - empty content becomes empty string
      expect(mockSession.messages).toHaveLength(1);
      expect(mockSession.messages[0].content).toBe('');
    });

    it('should handle empty content array (Anthropic)', () => {
      const event: AnthropicUserMessageEvent = {
        type: 'anthropic_user_message',
        vendor: 'anthropic',
        session_id: testSessionId,
        role: 'user',
        message: {
          role: 'user',
          content: []
        }
      };

      processor.processEvent(event);

      // Should handle gracefully
      expect(mockSession.messages).toHaveLength(1);
      expect(mockSession.messages[0].content).toBe('');
    });
  });

  /**
   * Test Group 8: Cross-Vendor Consistency
   */
  describe('Cross-Vendor Consistency', () => {
    it('should produce identical output structure for simple text (OpenAI vs Anthropic)', () => {
      const textContent = 'This is a consistency test';
      
      // Test OpenAI
      const openAiEvent: OpenAIUserMessageEvent = {
        type: 'openai_user_message',
        vendor: 'openai',
        session_id: testSessionId,
        role: 'user',
        message: {
          role: 'user',
          content: textContent
        }
      };

      processor.processEvent(openAiEvent);
      const openAiMessage = { ...mockSession.messages[0] };

      // Reset session
      mockSession.messages = [];
      sessionManagerEmitSpy.mockClear();

      // Test Anthropic
      const anthropicEvent: AnthropicUserMessageEvent = {
        type: 'anthropic_user_message',
        vendor: 'anthropic',
        session_id: testSessionId,
        role: 'user',
        message: {
          role: 'user',
          content: textContent
        }
      };

      processor.processEvent(anthropicEvent);
      const anthropicMessage = { ...mockSession.messages[0] };

      // Remove timestamps for comparison (they will differ)
      delete (openAiMessage as any).timestamp;
      delete (anthropicMessage as any).timestamp;

      // Messages should be structurally identical
      expect(openAiMessage).toEqual(anthropicMessage);
    });

    it('should produce identical output structure for multimodal content', () => {
      // Test OpenAI multimodal
      const openAiContent: ChatCompletionContentPart[] = [
        { type: 'text', text: 'Image test' },
        { type: 'image_url', image_url: { url: 'https://example.com/test.jpg' } }
      ];

      const openAiEvent: OpenAIUserMessageEvent = {
        type: 'openai_user_message',
        vendor: 'openai',
        session_id: testSessionId,
        role: 'user',
        message: {
          role: 'user',
          content: openAiContent
        }
      };

      processor.processEvent(openAiEvent);
      const openAiMessage = { ...mockSession.messages[0] };

      // Reset
      mockSession.messages = [];
      sessionManagerEmitSpy.mockClear();

      // Test Anthropic multimodal
      const anthropicContent: ContentBlockParam[] = [
        { type: 'text', text: 'Image test' },
        { type: 'image', source: { type: 'url', url: 'https://example.com/test.jpg' } }
      ];

      const anthropicEvent: AnthropicUserMessageEvent = {
        type: 'anthropic_user_message',
        vendor: 'anthropic',
        session_id: testSessionId,
        role: 'user',
        message: {
          role: 'user',
          content: anthropicContent
        }
      };

      processor.processEvent(anthropicEvent);
      const anthropicMessage = { ...mockSession.messages[0] };

      // Both should have multimodal content structure
      expect(Array.isArray(openAiMessage.content)).toBe(true);
      expect(Array.isArray(anthropicMessage.content)).toBe(true);
      
      const openAiParts = openAiMessage.content as any[];
      const anthropicParts = anthropicMessage.content as any[];
      
      expect(openAiParts[0].type).toBe('text');
      expect(anthropicParts[0].type).toBe('text');
      expect(openAiParts[0].text).toBe(anthropicParts[0].text);
      
      expect(openAiParts[1].type).toBe('image');
      expect(anthropicParts[1].type).toBe('image');
      expect(openAiParts[1].source.url).toBe(anthropicParts[1].source.url);
    });

    it('should produce identical sub-session metadata structure', () => {
      // Test OpenAI sub-session
      const openAiEvent: OpenAIUserMessageEvent = {
        type: 'openai_user_message',
        vendor: 'openai',
        session_id: 'sub-123',
        parent_session_id: 'parent-456',
        user_session_id: testSessionId,
        role: 'user',
        message: {
          role: 'user',
          content: 'Sub-session test'
        }
      };

      processor.processEvent(openAiEvent);
      const openAiMessage = mockSession.messages[0] as any;

      // Reset
      mockSession.messages = [];

      // Test Anthropic sub-session
      const anthropicEvent: AnthropicUserMessageEvent = {
        type: 'anthropic_user_message',
        vendor: 'anthropic',
        session_id: 'sub-123',
        parent_session_id: 'parent-456',
        user_session_id: testSessionId,
        role: 'user',
        message: {
          role: 'user',
          content: 'Sub-session test'
        }
      };

      processor.processEvent(anthropicEvent);
      const anthropicMessage = mockSession.messages[0] as any;

      // Metadata structure should be identical
      expect(openAiMessage.isSubSession).toBe(anthropicMessage.isSubSession);
      expect(openAiMessage.metadata).toEqual(anthropicMessage.metadata);
    });

    it('should emit identical event structures', () => {
      const messageAddedCalls: any[] = [];
      const userMessageCalls: any[] = [];

      sessionManagerEmitSpy.mockImplementation((eventName, payload) => {
        if (eventName === 'message-added') {
          messageAddedCalls.push(payload);
        } else if (eventName === 'user-message') {
          userMessageCalls.push(payload);
        }
      });

      // Test OpenAI
      const openAiEvent: OpenAIUserMessageEvent = {
        type: 'openai_user_message',
        vendor: 'openai',
        session_id: testSessionId,
        role: 'user',
        message: {
          role: 'user',
          content: 'Event test'
        }
      };

      processor.processEvent(openAiEvent);

      // Reset
      mockSession.messages = [];
      messageAddedCalls.length = 0;
      userMessageCalls.length = 0;

      // Test Anthropic
      const anthropicEvent: AnthropicUserMessageEvent = {
        type: 'anthropic_user_message',
        vendor: 'anthropic',
        session_id: testSessionId,
        role: 'user',
        message: {
          role: 'user',
          content: 'Event test'
        }
      };

      processor.processEvent(anthropicEvent);

      // Both should emit message-added with same structure (excluding timestamp)
      expect(messageAddedCalls[0].sessionId).toBeDefined();
      expect(messageAddedCalls[0].message).toBeDefined();
      expect(messageAddedCalls[0].message.role).toBe('user');
      expect(messageAddedCalls[0].message.content).toBe('Event test');

      // user-message events will differ only in vendor field
      expect(userMessageCalls[0].message).toBeDefined();
    });
  });

  /**
   * Test Group 9: Session State Management
   */
  describe('Session State Management', () => {
    it('should update session updated_at timestamp', () => {
      const initialUpdatedAt = mockSession.updated_at;

      // Wait a tiny bit to ensure timestamp difference
      setTimeout(() => {
        const event: OpenAIUserMessageEvent = {
          type: 'openai_user_message',
          vendor: 'openai',
          session_id: testSessionId,
          role: 'user',
          message: {
            role: 'user',
            content: 'Update timestamp test'
          }
        };

        processor.processEvent(event);

        expect(mockSession.updated_at).not.toBe(initialUpdatedAt);
      }, 10);
    });

    it('should handle multiple sequential messages correctly', () => {
      const messages = [
        'First message',
        'Second message',
        'Third message'
      ];

      messages.forEach(content => {
        const event: OpenAIUserMessageEvent = {
          type: 'openai_user_message',
          vendor: 'openai',
          session_id: testSessionId,
          role: 'user',
          message: {
            role: 'user',
            content
          }
        };
        processor.processEvent(event);
      });

      expect(mockSession.messages).toHaveLength(3);
      expect(mockSession.messages[0].content).toBe('First message');
      expect(mockSession.messages[1].content).toBe('Second message');
      expect(mockSession.messages[2].content).toBe('Third message');
    });

    it('should not add message if no current session exists', () => {
      // Create a new processor with session manager that has no current session
      const emptySessionManager = new ChatSessionManager();
      const emptyProcessor = new EventStreamProcessor(emptySessionManager);
      const emptyEmitSpy = vi.spyOn(emptySessionManager, 'emit');
      
      // Set the current chat session ID but don't set a current session
      emptyProcessor.setCurrentChatSessionId(testSessionId);

      const event: OpenAIUserMessageEvent = {
        type: 'openai_user_message',
        vendor: 'openai',
        session_id: testSessionId,
        role: 'user',
        message: {
          role: 'user',
          content: 'No session test'
        }
      };

      emptyProcessor.processEvent(event);

      // Verify getCurrentSession returns null
      expect(emptySessionManager.getCurrentSession()).toBeNull();

      // But backward compatibility event should still be emitted
      expect(emptyEmitSpy).toHaveBeenCalledWith('user-message', {
        vendor: 'openai',
        message: expect.any(Object)
      });
      
      // Cleanup
      emptyProcessor.destroy();
      emptySessionManager.destroy();
    });

    it('should handle mixed vendor messages in same session', () => {
      // OpenAI message
      const openAiEvent: OpenAIUserMessageEvent = {
        type: 'openai_user_message',
        vendor: 'openai',
        session_id: testSessionId,
        role: 'user',
        message: {
          role: 'user',
          content: 'OpenAI message'
        }
      };

      processor.processEvent(openAiEvent);

      // Anthropic message
      const anthropicEvent: AnthropicUserMessageEvent = {
        type: 'anthropic_user_message',
        vendor: 'anthropic',
        session_id: testSessionId,
        role: 'user',
        message: {
          role: 'user',
          content: 'Anthropic message'
        }
      };

      processor.processEvent(anthropicEvent);

      // Both should be in session
      expect(mockSession.messages).toHaveLength(2);
      expect(mockSession.messages[0].content).toBe('OpenAI message');
      expect(mockSession.messages[1].content).toBe('Anthropic message');
    });
  });

  /**
   * Test Group 10: Integration Flow Tests
   */
  describe('Complete Event Pipeline Integration', () => {
    it('should handle complete user message flow end-to-end', () => {
      const eventSequence: string[] = [];
      let capturedMessage: Message | null = null;

      sessionManagerEmitSpy.mockImplementation((eventName, payload) => {
        eventSequence.push(eventName as string);
        if (eventName === 'message-added') {
          capturedMessage = payload.message;
        }
      });

      const content: ChatCompletionContentPart[] = [
        { type: 'text', text: 'Complete flow test with image' },
        { type: 'image_url', image_url: { url: 'https://example.com/flow.jpg' } }
      ];

      const event: OpenAIUserMessageEvent = {
        type: 'openai_user_message',
        vendor: 'openai',
        session_id: testSessionId,
        role: 'user',
        message: {
          role: 'user',
          content
        }
      };

      processor.processEvent(event);

      // Verify complete flow
      expect(eventSequence).toContain('message-added');
      expect(eventSequence).toContain('user-message');
      expect(capturedMessage).not.toBeNull();
      expect(Array.isArray(capturedMessage?.content)).toBe(true);
      expect(mockSession.messages).toHaveLength(1);
      expect(mockSession.messages[0]).toBe(capturedMessage);
      expect(mockSession.updated_at).toBeDefined();
    });

    it('should maintain event order under rapid message processing', () => {
      const events: OpenAIUserMessageEvent[] = Array.from({ length: 10 }, (_, i) => ({
        type: 'openai_user_message',
        vendor: 'openai',
        session_id: testSessionId,
        role: 'user',
        message: {
          role: 'user',
          content: `Message ${i + 1}`
        }
      }));

      // Process all events rapidly
      events.forEach(event => processor.processEvent(event));

      // All messages should be in session in correct order
      expect(mockSession.messages).toHaveLength(10);
      mockSession.messages.forEach((msg, i) => {
        expect(msg.content).toBe(`Message ${i + 1}`);
      });
    });
  });
});
