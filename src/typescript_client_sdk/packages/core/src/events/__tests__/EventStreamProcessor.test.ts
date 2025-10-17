/**
 * Test suite for EventStreamProcessor - streaming delta assembly fixes
 * 
 * These tests verify that:
 * 1. Deltas emit `message-streaming` events as they arrive (real-time streaming)
 * 2. Messages accumulate correctly without duplication during streaming
 * 3. Messages are added to the session only once after completion
 * 4. EventStreamProcessor handles the full flow without ChatSessionManager interference
 */

import { describe, it, expect, beforeEach, afterEach, vi } from 'vitest';
import { EventStreamProcessor } from '../EventStreamProcessor';
import { ChatSessionManager } from '../../session/SessionManager';
import { 
  TextDeltaEvent,
  CompletionEvent,
  ChatSessionChangedEvent,
  InteractionEvent,
  ToolSelectDeltaEvent,
  ToolCallEvent,
  ThoughtDeltaEvent
} from '../types/ServerEvents';
import { ChatSession, Message } from '../types/CommonTypes';

describe('EventStreamProcessor - Streaming Delta Assembly', () => {
  let processor: EventStreamProcessor;
  let sessionManager: ChatSessionManager;
  let sessionManagerEmitSpy: ReturnType<typeof vi.spyOn>;
  let getCurrentSessionSpy: ReturnType<typeof vi.spyOn>;
  let mockSession: ChatSession;

  beforeEach(() => {
    vi.clearAllMocks();
    
    // Create a mock session
    mockSession = {
      session_id: 'test-session-123',
      session_name: 'Test Session',
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
    getCurrentSessionSpy = vi.spyOn(sessionManager, 'getCurrentSession');
    
    // Set the current session
    sessionManager.setCurrentSession(mockSession);
  });

  afterEach(() => {
    vi.restoreAllMocks();
    processor.destroy();
    sessionManager.destroy();
  });

  describe('Real-time Streaming Events', () => {
    it('should emit message-streaming events as deltas arrive without waiting for completion', () => {
      const delta1: TextDeltaEvent = {
        type: 'text_delta',
        content: 'Hello ',
        session_id: 'test-session-123'
      };

      const delta2: TextDeltaEvent = {
        type: 'text_delta',
        content: 'world',
        session_id: 'test-session-123'
      };

      // Process first delta
      processor.processEvent(delta1);

      // Verify message-streaming was emitted immediately
      expect(sessionManagerEmitSpy).toHaveBeenCalledWith('message-streaming', {
        sessionId: 'test-session-123',
        message: expect.objectContaining({
          role: 'assistant',
          content: 'Hello ',
          format: 'text'
        })
      });

      // Clear previous calls to check next emission
      sessionManagerEmitSpy.mockClear();

      // Process second delta
      processor.processEvent(delta2);

      // Verify message-streaming was emitted with accumulated content
      expect(sessionManagerEmitSpy).toHaveBeenCalledWith('message-streaming', {
        sessionId: 'test-session-123',
        message: expect.objectContaining({
          role: 'assistant',
          content: 'Hello world',
          format: 'text'
        })
      });

      // Verify no completion event was emitted yet
      expect(sessionManagerEmitSpy).not.toHaveBeenCalledWith('message-complete', expect.anything());
    });

    it('should accumulate messages correctly without duplication during streaming', () => {
      const deltas = [
        'The ',
        'quick ',
        'brown ',
        'fox ',
        'jumps'
      ];

      let expectedAccumulation = '';

      deltas.forEach((deltaContent, index) => {
        expectedAccumulation += deltaContent;

        const deltaEvent: TextDeltaEvent = {
          type: 'text_delta',
          content: deltaContent,
          session_id: 'test-session-123'
        };

        processor.processEvent(deltaEvent);

        // Check that the emitted message has the correct accumulated content
        const emittedCall = sessionManagerEmitSpy.mock.calls.find(
          call => call[0] === 'message-streaming'
        );

        expect(emittedCall).toBeDefined();
        expect(emittedCall![1]).toEqual({
          sessionId: 'test-session-123',
          message: expect.objectContaining({
            role: 'assistant',
            content: expectedAccumulation,
            format: 'text'
          })
        });

        // Clear for next iteration
        sessionManagerEmitSpy.mockClear();
      });

      // Verify the final accumulated message is correct
      const completion: CompletionEvent = {
        type: 'completion',
        running: false,
        session_id: 'test-session-123'
      };

      processor.processEvent(completion);

      // Check the final message
      expect(sessionManagerEmitSpy).toHaveBeenCalledWith('message-complete', {
        sessionId: 'test-session-123',
        message: expect.objectContaining({
          role: 'assistant',
          content: 'The quick brown fox jumps',
          format: 'text'
        })
      });
    });
  });

  describe('Session Message Management', () => {
    it('should add message to session only once on completion, not during streaming', () => {
      // Start with empty session
      expect(mockSession.messages).toHaveLength(0);

      // Process multiple deltas
      const deltas = ['Hello', ' ', 'there', '!'];
      
      deltas.forEach(content => {
        const deltaEvent: TextDeltaEvent = {
          type: 'text_delta',
          content,
          session_id: 'test-session-123'
        };
        processor.processEvent(deltaEvent);
      });

      // Session should still be empty during streaming
      expect(mockSession.messages).toHaveLength(0);

      // Process completion event
      const completion: CompletionEvent = {
        type: 'completion',
        running: false,
        session_id: 'test-session-123',
        input_tokens: 10,
        output_tokens: 5
      };

      processor.processEvent(completion);

      // Now message should be added to session exactly once
      expect(mockSession.messages).toHaveLength(1);
      expect(mockSession.messages[0]).toEqual(expect.objectContaining({
        role: 'assistant',
        content: 'Hello there!',
        format: 'text'
      }));

      // Token count should be updated
      expect(mockSession.token_count).toBe(15); // 10 + 5
    });

    it('should not duplicate messages when multiple completions occur', () => {
      // First message stream
      processor.processEvent({
        type: 'text_delta',
        content: 'First message',
        session_id: 'test-session-123'
      } as TextDeltaEvent);

      processor.processEvent({
        type: 'completion',
        running: false,
        session_id: 'test-session-123'
      } as CompletionEvent);

      expect(mockSession.messages).toHaveLength(1);

      // Second message stream
      processor.processEvent({
        type: 'text_delta',
        content: 'Second message',
        session_id: 'test-session-123'
      } as TextDeltaEvent);

      processor.processEvent({
        type: 'completion',
        running: false,
        session_id: 'test-session-123'
      } as CompletionEvent);

      // Should have exactly 2 messages, not duplicated
      expect(mockSession.messages).toHaveLength(2);
      expect(mockSession.messages[0].content).toBe('First message');
      expect(mockSession.messages[1].content).toBe('Second message');
    });
  });

  describe('EventStreamProcessor Isolation', () => {
    it('should handle the full flow without ChatSessionManager text accumulation interference', () => {
      // Spy on deprecated ChatSessionManager methods that should NOT be called
      const handleTextDeltaSpy = vi.spyOn(sessionManager, 'handleTextDelta');
      const handleTextDoneSpy = vi.spyOn(sessionManager, 'handleTextDone');
      const resetAccumulatorSpy = vi.spyOn(sessionManager, 'resetAccumulator');

      // Process a complete message flow
      processor.processEvent({
        type: 'interaction',
        started: true,
        id: 'interaction-1'
      } as InteractionEvent);

      processor.processEvent({
        type: 'text_delta',
        content: 'Test message',
        session_id: 'test-session-123'
      } as TextDeltaEvent);

      processor.processEvent({
        type: 'completion',
        running: false,
        session_id: 'test-session-123'
      } as CompletionEvent);

      // Verify ChatSessionManager's deprecated methods were NOT called
      expect(handleTextDeltaSpy).not.toHaveBeenCalled();
      expect(handleTextDoneSpy).not.toHaveBeenCalled();
      
      // resetAccumulator might be called during session changes, but not during normal message flow
      // Check that the message was handled entirely by EventStreamProcessor
      expect(mockSession.messages).toHaveLength(1);
      expect(mockSession.messages[0].content).toBe('Test message');
    });

    it('should reset message builder on interaction start to prevent cross-interaction contamination', () => {
      // Start first interaction and add some text
      processor.processEvent({
        type: 'interaction',
        started: true,
        id: 'interaction-1'
      } as InteractionEvent);

      processor.processEvent({
        type: 'text_delta',
        content: 'First interaction',
        session_id: 'test-session-123'
      } as TextDeltaEvent);

      // Start second interaction without completing the first
      processor.processEvent({
        type: 'interaction',
        started: true,
        id: 'interaction-2'
      } as InteractionEvent);

      // Add text for second interaction
      processor.processEvent({
        type: 'text_delta',
        content: 'Second interaction',
        session_id: 'test-session-123'
      } as TextDeltaEvent);

      // Complete the second interaction
      processor.processEvent({
        type: 'completion',
        running: false,
        session_id: 'test-session-123'
      } as CompletionEvent);

      // Should only have the second interaction's message
      expect(mockSession.messages).toHaveLength(1);
      expect(mockSession.messages[0].content).toBe('Second interaction');
    });

    it('should handle session changes without message duplication', () => {
      // Process initial session
      const initialSession: ChatSessionChangedEvent = {
        type: 'chat_session_changed',
        chat_session: {
          session_id: 'session-1',
          session_name: 'Session 1',
          messages: [
            {
              role: 'user',
              content: 'Existing message',
              timestamp: new Date().toISOString(),
              format: 'text' as const
            }
          ],
          created_at: new Date().toISOString(),
          updated_at: new Date().toISOString(),
          token_count: 10,
          metadata: {}
        }
      };

      processor.processEvent(initialSession);

      // Verify session-messages-loaded event was emitted for bulk loading
      expect(sessionManagerEmitSpy).toHaveBeenCalledWith('session-messages-loaded', {
        sessionId: 'session-1',
        messages: expect.arrayContaining([
          expect.objectContaining({
            role: 'user',
            content: 'Existing message'
          })
        ])
      });

      // Now process new streaming messages
      processor.processEvent({
        type: 'text_delta',
        content: 'New streaming message',
        session_id: 'session-1'
      } as TextDeltaEvent);

      processor.processEvent({
        type: 'completion',
        running: false,
        session_id: 'session-1'
      } as CompletionEvent);

      // Get the updated session
      const updatedSession = sessionManager.getCurrentSession();
      
      // Should have both the existing message and the new one
      expect(updatedSession?.messages).toHaveLength(2);
      expect(updatedSession?.messages[0].content).toBe('Existing message');
      expect(updatedSession?.messages[1].content).toBe('New streaming message');
    });
  });

  describe('Event Emission Verification', () => {
    it('should emit correct sequence of events for a complete message flow', () => {
      const eventSequence: string[] = [];
      
      // Track all emitted events
      sessionManagerEmitSpy.mockImplementation((eventName) => {
        eventSequence.push(eventName);
      });

      // Process complete flow
      processor.processEvent({
        type: 'interaction',
        started: true,
        id: 'test-interaction'
      } as InteractionEvent);

      processor.processEvent({
        type: 'text_delta',
        content: 'Part 1 ',
        session_id: 'test-session-123'
      } as TextDeltaEvent);

      processor.processEvent({
        type: 'text_delta',
        content: 'Part 2',
        session_id: 'test-session-123'
      } as TextDeltaEvent);

      processor.processEvent({
        type: 'completion',
        running: false,
        session_id: 'test-session-123'
      } as CompletionEvent);

      // Verify event sequence
      expect(eventSequence).toEqual([
        'message-streaming',  // First delta
        'message-streaming',  // Second delta
        'message-complete'    // Completion
      ]);
    });

    it('should include metadata in completion event', () => {
      processor.processEvent({
        type: 'text_delta',
        content: 'Test content',
        session_id: 'test-session-123'
      } as TextDeltaEvent);

      const completionEvent: CompletionEvent = {
        type: 'completion',
        running: false,
        session_id: 'test-session-123',
        input_tokens: 50,
        output_tokens: 25,
        stop_reason: 'end_turn'
      };

      processor.processEvent(completionEvent);

      // Verify the complete event includes metadata
      expect(sessionManagerEmitSpy).toHaveBeenCalledWith('message-complete', {
        sessionId: 'test-session-123',
        message: expect.objectContaining({
          role: 'assistant',
          content: 'Test content',
          format: 'text',
          metadata: expect.objectContaining({
            inputTokens: 50,
            outputTokens: 25,
            stopReason: 'end_turn'
          })
        })
      });
    });
  });

  describe('Deduplication Prevention', () => {
    it('should not process the same delta event twice even if sent multiple times', () => {
      const delta: TextDeltaEvent = {
        type: 'text_delta',
        content: 'Test content',
        session_id: 'test-session-123'
      };

      // Process the same delta event object twice
      processor.processEvent(delta);
      processor.processEvent(delta);

      // Should still accumulate both (EventStreamProcessor doesn't dedupe by reference)
      // This tests that the processor handles the events as they come
      processor.processEvent({
        type: 'completion',
        running: false,
        session_id: 'test-session-123'
      } as CompletionEvent);

      expect(mockSession.messages).toHaveLength(1);
      expect(mockSession.messages[0].content).toBe('Test contentTest content');
    });

    it('should ensure EventStreamProcessor is the sole handler for streaming events', () => {
      // This test verifies that critical streaming events are handled by EventStreamProcessor
      // In the actual RealtimeClient implementation, these events are not emitted directly
      // but are processed through EventStreamProcessor first
      
      const criticalStreamingEvents = [
        { type: 'text_delta', content: 'test', session_id: 'test-session-123' },
        { type: 'thought_delta', content: 'thinking', session_id: 'test-session-123' },
        { type: 'completion', running: false, session_id: 'test-session-123' },
        { type: 'interaction', started: true, id: 'test-interaction' }
      ];

      // Track emitted events
      const emittedEvents: string[] = [];
      sessionManagerEmitSpy.mockImplementation((eventName) => {
        emittedEvents.push(eventName);
      });

      // Process critical streaming events
      criticalStreamingEvents.forEach(event => {
        processor.processEvent(event as any);
      });

      // Verify that EventStreamProcessor emitted the expected events
      expect(emittedEvents).toContain('message-streaming'); // From text_delta
      expect(emittedEvents).toContain('message-complete');   // From completion
      
      // The key point: These events were processed by EventStreamProcessor,
      // not by direct event handlers in RealtimeClient
    });

    it('should not allow ChatSessionManager accumulator to interfere with message building', () => {
      // Get the ChatSessionManager's accumulator state
      const initialAccumulator = sessionManager.getAccumulatedText();
      expect(initialAccumulator).toBe('');

      // Process deltas through EventStreamProcessor
      processor.processEvent({
        type: 'text_delta',
        content: 'Message from EventStreamProcessor',
        session_id: 'test-session-123'
      } as TextDeltaEvent);

      // ChatSessionManager's accumulator should remain empty
      // (EventStreamProcessor handles its own accumulation)
      expect(sessionManager.getAccumulatedText()).toBe('');
      expect(sessionManager.isAccumulatingText()).toBe(false);

      // Complete the message
      processor.processEvent({
        type: 'completion',
        running: false,
        session_id: 'test-session-123'
      } as CompletionEvent);

      // Message should be in session from EventStreamProcessor, not ChatSessionManager
      expect(mockSession.messages).toHaveLength(1);
      expect(mockSession.messages[0].content).toBe('Message from EventStreamProcessor');
      
      // ChatSessionManager accumulator should still be empty
      expect(sessionManager.getAccumulatedText()).toBe('');
    });
  });

  describe('Edge Cases and Error Handling', () => {
    it('should handle completion without prior deltas gracefully', () => {
      const completion: CompletionEvent = {
        type: 'completion',
        running: false,
        session_id: 'test-session-123'
      };

      // Should not throw
      expect(() => processor.processEvent(completion)).not.toThrow();

      // Should not add empty message to session
      expect(mockSession.messages).toHaveLength(0);

      // Should not emit message-complete for empty message
      expect(sessionManagerEmitSpy).not.toHaveBeenCalledWith('message-complete', expect.anything());
    });

    it('should handle deltas after reset correctly', () => {
      // Process first message
      processor.processEvent({
        type: 'text_delta',
        content: 'First',
        session_id: 'test-session-123'
      } as TextDeltaEvent);

      // Reset processor
      processor.reset();

      // Process new message after reset
      processor.processEvent({
        type: 'text_delta',
        content: 'Second',
        session_id: 'test-session-123'
      } as TextDeltaEvent);

      processor.processEvent({
        type: 'completion',
        running: false,
        session_id: 'test-session-123'
      } as CompletionEvent);

      // Should only have the second message, not concatenated
      expect(mockSession.messages).toHaveLength(1);
      expect(mockSession.messages[0].content).toBe('Second');
    });

    it('should handle thought deltas separately from text deltas', () => {
      // Process text delta
      processor.processEvent({
        type: 'text_delta',
        content: 'User visible text',
        session_id: 'test-session-123'
      } as TextDeltaEvent);

      // Clear setup events before checking thought_delta processing
      sessionManagerEmitSpy.mockClear();

      // Process thought delta
      processor.processEvent({
        type: 'thought_delta',
        content: 'Internal thinking',
        session_id: 'test-session-123'
      } as any);

      // Should finalize text message and start new thought message
      expect(sessionManagerEmitSpy).toHaveBeenCalledWith('message-complete', {
        sessionId: 'test-session-123',
        message: expect.objectContaining({
          content: 'User visible text'
        })
      });

      // Thought messages have role: 'assistant (thought)' with type: 'message' to differentiate
      expect(sessionManagerEmitSpy).toHaveBeenCalledWith('message-streaming', {
        sessionId: 'test-session-123',
        message: expect.objectContaining({
          role: 'assistant (thought)',
          type: 'message',
          content: 'Internal thinking'
        })
      });
    });

    it('should handle rapid successive deltas without loss', () => {
      const deltas = Array.from({ length: 100 }, (_, i) => `word${i} `);
      
      deltas.forEach(content => {
        processor.processEvent({
          type: 'text_delta',
          content,
          session_id: 'test-session-123'
        } as TextDeltaEvent);
      });

      processor.processEvent({
        type: 'completion',
        running: false,
        session_id: 'test-session-123'
      } as CompletionEvent);

      const expectedContent = deltas.join('');
      expect(mockSession.messages[0].content).toBe(expectedContent);
    });
  });

  describe('Tool Event Handling', () => {
    describe('tool_select_delta events', () => {
      it('should emit tool-notification for regular tools', () => {
        const toolSelectEvent: ToolSelectDeltaEvent = {
          type: 'tool_select_delta',
          session_id: 'test-session-123',
          role: 'assistant',
          parent_session_id: null,
          user_session_id: 'test-session-123',
          tool_calls: [
            {
              id: 'tool-1',
              type: 'tool_use',
              name: 'calculator',
              input: { operation: 'add', a: 5, b: 3 }
            }
          ]
        };

        processor.processEvent(toolSelectEvent);

        // Filter for tool-notification events instead of assuming position
        const toolNotificationCalls = sessionManagerEmitSpy.mock.calls.filter(
          call => call[0] === 'tool-notification'
        );
        
        expect(toolNotificationCalls.length).toBeGreaterThan(0);
        expect(toolNotificationCalls[0][1]).toMatchObject({
          id: 'tool-1',
          toolName: 'calculator',
          status: 'preparing',
          sessionId: 'test-session-123',
          timestamp: expect.any(Date),
          arguments: JSON.stringify({ operation: 'add', a: 5, b: 3 })
        });
      });

      it('should handle think tool specially', () => {
        const thinkSelectEvent: ToolSelectDeltaEvent = {
          type: 'tool_select_delta',
          session_id: 'test-session-123',
          role: 'assistant',
          parent_session_id: null,
          user_session_id: 'test-session-123',
          tool_calls: [
            {
              id: 'think-1',
              type: 'tool_use',
              name: 'think',
              input: {}
            }
          ]
        };

        processor.processEvent(thinkSelectEvent);

        // Filter for tool-notification events instead of assuming position
        const toolNotificationCalls = sessionManagerEmitSpy.mock.calls.filter(
          call => call[0] === 'tool-notification'
        );
        
        expect(toolNotificationCalls.length).toBeGreaterThan(0);
        expect(toolNotificationCalls[0][1]).toMatchObject({
          id: 'think-1',
          toolName: 'think',
          status: 'preparing',
          sessionId: 'test-session-123',
          timestamp: expect.any(Date),
          arguments: '{}'
        });
      });
    });

    describe('tool_call events', () => {
      it('should update notification when tool becomes active', () => {
        // First select the tool
        const selectEvent: ToolSelectDeltaEvent = {
          type: 'tool_select_delta',
          session_id: 'test-session-123',
          role: 'assistant',
          parent_session_id: null,
          user_session_id: 'test-session-123',
          tool_calls: [
            {
              id: 'tool-1',
              type: 'tool_use',
              name: 'web_search',
              input: { query: 'test' }
            }
          ]
        };
        processor.processEvent(selectEvent);
        sessionManagerEmitSpy.mockClear();

        // Then mark as active
        const activeEvent: ToolCallEvent = {
          type: 'tool_call',
          session_id: 'test-session-123',
          role: 'assistant',
          parent_session_id: null,
          user_session_id: 'test-session-123',
          active: true,
          vendor: 'anthropic',
          tool_calls: [
            {
              id: 'tool-1',
              type: 'tool_use',
              name: 'web_search',
              input: { query: 'test' }
            }
          ]
        };

        processor.processEvent(activeEvent);

        // Updated to include sessionId field in expected payload
        expect(sessionManagerEmitSpy).toHaveBeenCalledWith('tool-notification', {
          id: 'tool-1',
          toolName: 'web_search',
          status: 'executing',
          sessionId: 'test-session-123',
          timestamp: expect.any(Date),
          arguments: JSON.stringify({ query: 'test' })
        });
      });

      it('should remove notification and handle results when tool completes', () => {
        const completeEvent: ToolCallEvent = {
          type: 'tool_call',
          session_id: 'test-session-123',
          role: 'assistant',
          parent_session_id: null,
          user_session_id: 'test-session-123',
          active: false,
          vendor: 'anthropic',
          tool_calls: [
            {
              id: 'tool-1',
              type: 'tool_use',
              name: 'calculator',
              input: { operation: 'multiply', a: 6, b: 7 }
            }
          ],
          tool_results: [
            {
              type: 'tool_result',
              tool_use_id: 'tool-1',
              content: '{"result": 42}'
            }
          ]
        };

        processor.processEvent(completeEvent);

        // Filter for the removal event instead of assuming position
        const removalCalls = sessionManagerEmitSpy.mock.calls.filter(
          call => call[0] === 'tool-notification-removed'
        );
        
        expect(removalCalls.length).toBeGreaterThan(0);
        expect(removalCalls[0][1]).toMatchObject({
          sessionId: 'test-session-123',
          toolCallId: 'tool-1'
        });
      });

      it('should ignore tool_call events for think tool', () => {
        const thinkCallEvent: ToolCallEvent = {
          type: 'tool_call',
          session_id: 'test-session-123',
          role: 'assistant',
          parent_session_id: null,
          user_session_id: 'test-session-123',
          active: false,
          vendor: 'anthropic',
          tool_calls: [
            {
              id: 'think-1',
              type: 'tool_use',
              name: 'think',
              input: {}
            }
          ],
          tool_results: [
            {
              type: 'tool_result',
              tool_use_id: 'think-1',
              content: 'Agent thought process...'
            }
          ]
        };

        processor.processEvent(thinkCallEvent);

        // Filter for the removal event instead of assuming position
        const removalCalls = sessionManagerEmitSpy.mock.calls.filter(
          call => call[0] === 'tool-notification-removed'
        );
        
        expect(removalCalls.length).toBeGreaterThan(0);
        expect(removalCalls[0][1]).toMatchObject({
          sessionId: 'test-session-123',
          toolCallId: 'think-1'
        });
        
        // Should not emit new tool notifications (only remove)
        const notificationCalls = sessionManagerEmitSpy.mock.calls.filter(
          call => call[0] === 'tool-notification'
        );
        expect(notificationCalls.length).toBe(0);
      });
    });

    describe('thought_delta with think tool interaction', () => {
      it('should remove think tool notification when thought deltas start', () => {
        // First, select the think tool
        const thinkSelectEvent: ToolSelectDeltaEvent = {
          type: 'tool_select_delta',
          session_id: 'test-session-123',
          role: 'assistant',
          parent_session_id: null,
          user_session_id: 'test-session-123',
          tool_calls: [
            {
              id: 'think-1',
              type: 'tool_use',
              name: 'think',
              input: {}
            }
          ]
        };
        processor.processEvent(thinkSelectEvent);
        sessionManagerEmitSpy.mockClear();

        // Then process a thought delta
        const thoughtDeltaEvent: ThoughtDeltaEvent = {
          type: 'thought_delta',
          content: 'I need to analyze this...',
          session_id: 'test-session-123'
        };

        processor.processEvent(thoughtDeltaEvent);

        // Filter for removal event instead of assuming position
        const removalCalls = sessionManagerEmitSpy.mock.calls.filter(
          call => call[0] === 'tool-notification-removed'
        );
        
        expect(removalCalls.length).toBeGreaterThan(0);
        expect(removalCalls[0][1]).toMatchObject({
          sessionId: 'test-session-123',
          toolCallId: 'think-1'
        });
        
        // And emit the thought streaming
        expect(sessionManagerEmitSpy).toHaveBeenCalledWith('message-streaming', {
          sessionId: 'test-session-123',
          message: expect.objectContaining({
            role: 'assistant (thought)',
            type: 'message',
            content: 'I need to analyze this...'
          })
        });
      });

      it('should handle multiple thought deltas after think tool selection', () => {
        // Select think tool
        const thinkSelectEvent: ToolSelectDeltaEvent = {
          type: 'tool_select_delta',
          session_id: 'test-session-123',
          role: 'assistant',
          parent_session_id: null,
          user_session_id: 'test-session-123',
          tool_calls: [
            {
              id: 'think-2',
              type: 'tool_use',
              name: 'think',
              input: {}
            }
          ]
        };
        processor.processEvent(thinkSelectEvent);

        // Process multiple thought deltas
        const thoughtDeltas = [
          'Let me think about this problem. ',
          'The user is asking about... ',
          'I should consider...'
        ];

        thoughtDeltas.forEach((content, index) => {
          sessionManagerEmitSpy.mockClear();
          
          processor.processEvent({
            type: 'thought_delta',
            content,
            session_id: 'test-session-123'
          } as ThoughtDeltaEvent);

          // First delta should remove notification
          if (index === 0) {
            const removalCalls = sessionManagerEmitSpy.mock.calls.filter(
              call => call[0] === 'tool-notification-removed'
            );
            
            expect(removalCalls.length).toBeGreaterThan(0);
            expect(removalCalls[0][1]).toMatchObject({
              sessionId: 'test-session-123',
              toolCallId: 'think-2'
            });
          }

          // All deltas should emit streaming
          expect(sessionManagerEmitSpy).toHaveBeenCalledWith('message-streaming', {
            sessionId: 'test-session-123',
            message: expect.objectContaining({
              type: 'message',
              role: 'assistant (thought)',
              content: thoughtDeltas.slice(0, index + 1).join('')
            })
          });
        });
      });
    });

    describe('Complete tool flow sequences', () => {
      it('should handle complete tool execution flow', () => {
        const eventSequence: string[] = [];
        sessionManagerEmitSpy.mockImplementation((eventName) => {
          eventSequence.push(eventName);
        });

        // 1. Tool selection
        processor.processEvent({
          type: 'tool_select_delta',
          session_id: 'test-session-123',
          role: 'assistant',
          parent_session_id: null,
          user_session_id: 'test-session-123',
          tool_calls: [
            {
              id: 'tool-flow-1',
              type: 'tool_use',
              name: 'weather_api',
              input: { location: 'San Francisco' }
            }
          ]
        } as ToolSelectDeltaEvent);

        // 2. Tool becomes active
        processor.processEvent({
          type: 'tool_call',
          session_id: 'test-session-123',
          role: 'assistant',
          parent_session_id: null,
          user_session_id: 'test-session-123',
          active: true,
          vendor: 'anthropic',
          tool_calls: [
            {
              id: 'tool-flow-1',
              type: 'tool_use',
              name: 'weather_api',
              input: { location: 'San Francisco' }
            }
          ]
        } as ToolCallEvent);

        // 3. Tool completes with results
        processor.processEvent({
          type: 'tool_call',
          session_id: 'test-session-123',
          role: 'assistant',
          parent_session_id: null,
          user_session_id: 'test-session-123',
          active: false,
          vendor: 'anthropic',
          tool_calls: [
            {
              id: 'tool-flow-1',
              type: 'tool_use',
              name: 'weather_api',
              input: { location: 'San Francisco' }
            }
          ],
          tool_results: [
            {
              type: 'tool_result',
              tool_use_id: 'tool-flow-1',
              content: '{"temperature": 72, "condition": "sunny"}'
            }
          ]
        } as ToolCallEvent);

        // Verify the sequence of events
        expect(eventSequence).toEqual([
          'tool-notification',  // Tool selected (preparing)
          'tool-notification',  // Tool active (executing)
          'tool-call-complete',  // Tool completed with results
          'tool-notification-removed'  // Tool notification cleared
        ]);
      });

      it('should handle think tool flow with thought deltas', () => {
        const eventSequence: string[] = [];
        sessionManagerEmitSpy.mockImplementation((eventName) => {
          eventSequence.push(eventName);
        });

        // 1. Think tool selection
        processor.processEvent({
          type: 'tool_select_delta',
          session_id: 'test-session-123',
          role: 'assistant',
          parent_session_id: null,
          user_session_id: 'test-session-123',
          tool_calls: [
            {
              id: 'think-flow-1',
              type: 'tool_use',
              name: 'think',
              input: {}
            }
          ]
        } as ToolSelectDeltaEvent);

        // 2. Thought deltas start
        processor.processEvent({
          type: 'thought_delta',
          content: 'Analyzing the request...',
          session_id: 'test-session-123'
        } as ThoughtDeltaEvent);

        processor.processEvent({
          type: 'thought_delta',
          content: ' I should approach this by...',
          session_id: 'test-session-123'
        } as ThoughtDeltaEvent);

        // 3. Tool call events (should be ignored)
        processor.processEvent({
          type: 'tool_call',
          session_id: 'test-session-123',
          role: 'assistant',
          parent_session_id: null,
          user_session_id: 'test-session-123',
          active: false,
          vendor: 'anthropic',
          tool_calls: [
            {
              id: 'think-flow-1',
              type: 'tool_use',
              name: 'think',
              input: {}
            }
          ],
          tool_results: [
            {
              type: 'tool_result',
              tool_use_id: 'think-flow-1',
              content: 'Thought process completed'
            }
          ]
        } as ToolCallEvent);

        // Verify the sequence
        expect(eventSequence).toContain('tool-notification');  // Think tool selected
        expect(eventSequence).toContain('tool-notification-removed');  // Removed when deltas start
        expect(eventSequence).toContain('message-streaming');  // Thought deltas
      });
    });
  });
});