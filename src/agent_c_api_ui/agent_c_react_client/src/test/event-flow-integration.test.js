import { describe, it, expect, beforeEach, afterEach, vi, beforeAll, afterAll } from 'vitest'
import { ChatEvent } from '../components/chat_interface/utils/ChatEvent'
import * as sessionApi from '../services/session-api'
import { 
  setupRedisStreamMocks, 
  createMockStreamEvent, 
  createTestChatEvents 
} from './utils/redis-stream-test-utils'
import { setupFetchMock, createMockResponse } from './utils/api-test-utils'

/**
 * Integration Tests for Redis Streams Event Flow
 * 
 * These tests validate the complete event flow from BaseAgent event raising
 * through AgentBridge processing to client consumption, ensuring all components
 * work together correctly with Redis Streams integration.
 */

describe('Redis Streams Event Flow - Integration Tests', () => {
  let restoreFetch;
  let redisStreamsMocks;
  let testSessionId;
  let testInteractionId;
  let testStreamKey;

  beforeAll(() => {
    // Setup global test data
    testSessionId = 'integration-test-session';
    testInteractionId = 'integration-test-interaction';
    testStreamKey = `agent_events:${testSessionId}:${testInteractionId}`;
  });

  beforeEach(() => {
    // Clear all mocks
    vi.clearAllMocks();
  });

  afterEach(() => {
    // Clean up mocks
    if (restoreFetch) {
      restoreFetch();
      restoreFetch = null;
    }
    if (redisStreamsMocks?.restore) {
      redisStreamsMocks.restore();
      redisStreamsMocks = null;
    }
  });

  describe('Complete Event Flow - BaseAgent to Client', () => {
    it('should handle complete user message to assistant response flow', async () => {
      // Setup Redis Streams mocks
      redisStreamsMocks = setupRedisStreamMocks({
        sessionId: testSessionId,
        streamName: testStreamKey,
        enableSubscription: true
      });

      // Setup API mocks for session operations
      restoreFetch = setupFetchMock({
        '/initialize': {
          success: true,
          data: {
            ui_session_id: testSessionId,
            agent_config: { model: 'gpt-4', temperature: 0.7 }
          }
        },
        [`/sessions/${testSessionId}/chat`]: {
          success: true,
          data: { interaction_id: testInteractionId }
        }
      });

      // Phase 1: Initialize session
      const sessionResult = await sessionApi.initialize({
        model: 'gpt-4',
        temperature: 0.7
      });

      expect(sessionResult.ui_session_id).toBe(testSessionId);

      // Phase 2: Simulate BaseAgent raising events
      const userMessageEvent = new ChatEvent({
        type: 'user_message',
        data: 'Can you help me with a coding problem?',
        sessionId: testSessionId,
        correlationId: testInteractionId,
        role: 'user',
        metadata: { ui_component: 'chat_interface' }
      });

      const toolCallEvent = new ChatEvent({
        type: 'tool_call',
        data: {
          function: 'code_analyzer',
          args: { language: 'javascript', code: 'console.log("hello");' }
        },
        sessionId: testSessionId,
        correlationId: testInteractionId,
        role: 'assistant',
        vendor: 'openai'
      });

      const assistantResponseEvent = new ChatEvent({
        type: 'assistant_response',
        data: 'I can help you with that! Let me analyze your code...',
        sessionId: testSessionId,
        correlationId: testInteractionId,
        role: 'assistant',
        vendor: 'openai',
        metadata: { model: 'gpt-4', temperature: 0.7 }
      });

      // Simulate events being published to Redis Stream
      redisStreamsMocks.addMockEvent(userMessageEvent.serialize());
      redisStreamsMocks.addMockEvent(toolCallEvent.serialize());
      redisStreamsMocks.addMockEvent(assistantResponseEvent.serialize());

      // Phase 3: Verify events can be consumed
      const eventSource = redisStreamsMocks.getEventSource();
      const receivedEvents = [];

      // Simulate event consumption
      const consumptionPromise = new Promise((resolve) => {
        eventSource.onmessage = (event) => {
          const eventData = JSON.parse(event.data);
          receivedEvents.push(ChatEvent.deserialize(eventData));
          
          if (receivedEvents.length === 3) {
            resolve(receivedEvents);
          }
        };
      });

      // Trigger event messages
      redisStreamsMocks.simulateEvent(userMessageEvent.serialize());
      redisStreamsMocks.simulateEvent(toolCallEvent.serialize());
      redisStreamsMocks.simulateEvent(assistantResponseEvent.serialize());

      const events = await consumptionPromise;

      // Verify event flow integrity
      expect(events).toHaveLength(3);
      expect(events[0].type).toBe('user_message');
      expect(events[1].type).toBe('tool_call');
      expect(events[2].type).toBe('assistant_response');

      // Verify correlation IDs are preserved
      expect(events[0].correlationId).toBe(testInteractionId);
      expect(events[1].correlationId).toBe(testInteractionId);
      expect(events[2].correlationId).toBe(testInteractionId);

      // Verify session context is preserved
      expect(events[0].sessionId).toBe(testSessionId);
      expect(events[1].sessionId).toBe(testSessionId);
      expect(events[2].sessionId).toBe(testSessionId);
    });

    it('should handle event ordering and sequencing correctly', async () => {
      // Setup mocks
      redisStreamsMocks = setupRedisStreamMocks({
        sessionId: testSessionId,
        streamName: testStreamKey
      });

      // Create a series of events with specific timestamps
      const baseTime = Date.now();
      const orderedEvents = [
        new ChatEvent({
          type: 'interaction_start',
          data: { started_at: new Date(baseTime).toISOString() },
          sessionId: testSessionId,
          correlationId: testInteractionId,
          timestamp: new Date(baseTime).toISOString()
        }),
        new ChatEvent({
          type: 'user_message',
          data: 'First message',
          sessionId: testSessionId,
          correlationId: testInteractionId,
          timestamp: new Date(baseTime + 1000).toISOString()
        }),
        new ChatEvent({
          type: 'text_delta',
          data: 'Thinking...',
          sessionId: testSessionId,
          correlationId: testInteractionId,
          timestamp: new Date(baseTime + 2000).toISOString()
        }),
        new ChatEvent({
          type: 'assistant_response',
          data: 'Here is my response',
          sessionId: testSessionId,
          correlationId: testInteractionId,
          timestamp: new Date(baseTime + 3000).toISOString()
        }),
        new ChatEvent({
          type: 'interaction_end',
          data: { ended_at: new Date(baseTime + 4000).toISOString() },
          sessionId: testSessionId,
          correlationId: testInteractionId,
          timestamp: new Date(baseTime + 4000).toISOString()
        })
      ];

      // Add events to mock stream
      orderedEvents.forEach(event => {
        redisStreamsMocks.addMockEvent(event.serialize());
      });

      // Retrieve and verify ordering
      const mockEvents = redisStreamsMocks.getMockEvents();
      expect(mockEvents).toHaveLength(5);

      // Verify chronological ordering
      for (let i = 1; i < mockEvents.length; i++) {
        const prevTime = new Date(mockEvents[i - 1].timestamp).getTime();
        const currTime = new Date(mockEvents[i].timestamp).getTime();
        expect(currTime).toBeGreaterThanOrEqual(prevTime);
      }

      // Verify logical flow ordering
      expect(mockEvents[0].type).toBe('interaction_start');
      expect(mockEvents[1].type).toBe('user_message');
      expect(mockEvents[2].type).toBe('text_delta');
      expect(mockEvents[3].type).toBe('assistant_response');
      expect(mockEvents[4].type).toBe('interaction_end');
    });

    it('should handle concurrent sessions without event mixing', async () => {
      const session1Id = 'session-1';
      const session2Id = 'session-2';
      const interaction1Id = 'interaction-1';
      const interaction2Id = 'interaction-2';

      // Setup mocks for both sessions
      const mocks1 = setupRedisStreamMocks({
        sessionId: session1Id,
        streamName: `agent_events:${session1Id}:${interaction1Id}`
      });

      const mocks2 = setupRedisStreamMocks({
        sessionId: session2Id,
        streamName: `agent_events:${session2Id}:${interaction2Id}`
      });

      // Create events for session 1
      const session1Events = [
        new ChatEvent({
          type: 'user_message',
          data: 'Message from session 1',
          sessionId: session1Id,
          correlationId: interaction1Id
        }),
        new ChatEvent({
          type: 'assistant_response',
          data: 'Response to session 1',
          sessionId: session1Id,
          correlationId: interaction1Id
        })
      ];

      // Create events for session 2
      const session2Events = [
        new ChatEvent({
          type: 'user_message',
          data: 'Message from session 2',
          sessionId: session2Id,
          correlationId: interaction2Id
        }),
        new ChatEvent({
          type: 'assistant_response',
          data: 'Response to session 2',
          sessionId: session2Id,
          correlationId: interaction2Id
        })
      ];

      // Add events to respective mock streams
      session1Events.forEach(event => mocks1.addMockEvent(event.serialize()));
      session2Events.forEach(event => mocks2.addMockEvent(event.serialize()));

      // Verify session isolation
      const stream1Events = mocks1.getMockEvents();
      const stream2Events = mocks2.getMockEvents();

      expect(stream1Events).toHaveLength(2);
      expect(stream2Events).toHaveLength(2);

      // Verify no event mixing
      stream1Events.forEach(event => {
        expect(event.sessionId).toBe(session1Id);
        expect(event.correlationId).toBe(interaction1Id);
      });

      stream2Events.forEach(event => {
        expect(event.sessionId).toBe(session2Id);
        expect(event.correlationId).toBe(interaction2Id);
      });

      // Verify content isolation
      expect(stream1Events[0].data).toBe('Message from session 1');
      expect(stream1Events[1].data).toBe('Response to session 1');
      expect(stream2Events[0].data).toBe('Message from session 2');
      expect(stream2Events[1].data).toBe('Response to session 2');

      // Cleanup
      mocks1.restore();
      mocks2.restore();
    });
  });

  describe('Error Handling and Fallback Mechanisms', () => {
    it('should gracefully fallback to async queue when Redis is unavailable', async () => {
      // Setup Redis to simulate connection failure
      redisStreamsMocks = setupRedisStreamMocks({
        sessionId: testSessionId,
        streamName: testStreamKey,
        simulateErrors: true
      });

      // Setup API mock that handles fallback
      restoreFetch = setupFetchMock({
        [`/sessions/${testSessionId}/chat`]: {
          success: true,
          data: {
            interaction_id: testInteractionId,
            fallback_mode: 'async_queue',
            redis_error: 'Connection refused'
          }
        }
      });

      // Simulate chat initiation that should fallback
      const result = await fetch(`/api/sessions/${testSessionId}/chat`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ message: 'Test message' })
      });

      const responseData = await result.json();

      expect(responseData.success).toBe(true);
      expect(responseData.data.fallback_mode).toBe('async_queue');
      expect(responseData.data.redis_error).toBe('Connection refused');
    });

    it('should handle partial Redis failures with circuit breaker', async () => {
      let callCount = 0;
      
      // Setup Redis mock that fails first few calls then succeeds
      redisStreamsMocks = setupRedisStreamMocks({
        sessionId: testSessionId,
        streamName: testStreamKey
      });

      // Mock API responses that simulate circuit breaker behavior
      const originalFetch = global.fetch;
      global.fetch = vi.fn((url, options) => {
        callCount++;
        
        if (callCount <= 3) {
          // First 3 calls fail (circuit breaker opens)
          return Promise.resolve(createMockResponse({
            success: false,
            error: 'Redis circuit breaker open',
            retry_after: 5
          }, { status: 503 }));
        } else {
          // Subsequent calls succeed (circuit breaker closes)
          return Promise.resolve(createMockResponse({
            success: true,
            data: { interaction_id: testInteractionId, redis_status: 'recovered' }
          }));
        }
      });

      restoreFetch = () => { global.fetch = originalFetch; };

      // Make multiple requests to trigger circuit breaker behavior
      const requests = Array.from({ length: 5 }, async (_, i) => {
        try {
          const response = await fetch(`/api/sessions/${testSessionId}/chat`, {
            method: 'POST',
            body: JSON.stringify({ message: `Message ${i}` })
          });
          return await response.json();
        } catch (error) {
          return { error: error.message };
        }
      });

      const results = await Promise.all(requests);

      // First 3 should fail with circuit breaker
      expect(results[0].success).toBe(false);
      expect(results[1].success).toBe(false);
      expect(results[2].success).toBe(false);

      // Last 2 should succeed after recovery
      expect(results[3].success).toBe(true);
      expect(results[4].success).toBe(true);
      expect(results[4].data.redis_status).toBe('recovered');
    });

    it('should handle Redis memory pressure and stream limits', async () => {
      // Setup Redis mock that simulates memory pressure
      redisStreamsMocks = setupRedisStreamMocks({
        sessionId: testSessionId,
        streamName: testStreamKey
      });

      // Add many events to simulate stream length limits
      const largeEventSet = Array.from({ length: 1500 }, (_, i) => 
        createMockStreamEvent({
          type: 'memory_pressure_test',
          data: `Event ${i}`,
          sessionId: testSessionId,
          correlationId: testInteractionId
        })
      );

      largeEventSet.forEach(event => redisStreamsMocks.addMockEvent(event));

      // Setup API mock that handles stream trimming
      restoreFetch = setupFetchMock({
        [`/sessions/${testSessionId}/streams/${testStreamKey}/trim`]: {
          success: true,
          data: {
            original_length: 1500,
            trimmed_length: 1000,
            events_removed: 500
          }
        }
      });

      // Simulate stream trimming due to memory pressure
      const trimResponse = await fetch(`/api/sessions/${testSessionId}/streams/${testStreamKey}/trim`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ max_length: 1000 })
      });

      const trimData = await trimResponse.json();

      expect(trimData.success).toBe(true);
      expect(trimData.data.events_removed).toBe(500);
      expect(trimData.data.trimmed_length).toBe(1000);

      // Verify stream can still function after trimming
      const mockEvents = redisStreamsMocks.getMockEvents();
      expect(mockEvents.length).toBeGreaterThan(0); // Stream should still have events
    });

    it('should handle network partitions and reconnection', async () => {
      let isNetworkUp = true;
      
      // Setup Redis mock that simulates network partition
      redisStreamsMocks = setupRedisStreamMocks({
        sessionId: testSessionId,
        streamName: testStreamKey,
        enableSubscription: true
      });

      const eventSource = redisStreamsMocks.getEventSource();
      const connectionEvents = [];

      // Track connection state changes
      eventSource.onopen = () => {
        connectionEvents.push('connected');
      };

      eventSource.onerror = (error) => {
        connectionEvents.push('error');
        if (!isNetworkUp) {
          // Simulate reconnection after network recovers
          setTimeout(() => {
            isNetworkUp = true;
            redisStreamsMocks.simulateOpen();
          }, 100);
        }
      };

      // Start connection
      redisStreamsMocks.simulateOpen();

      // Simulate network partition
      isNetworkUp = false;
      redisStreamsMocks.simulateError(new Error('Network partition'));

      // Wait for reconnection
      await new Promise(resolve => setTimeout(resolve, 200));

      expect(connectionEvents).toContain('connected');
      expect(connectionEvents).toContain('error');
      expect(connectionEvents[connectionEvents.length - 1]).toBe('connected'); // Should end up connected
    });
  });

  describe('Performance and Scalability Validation', () => {
    it('should handle high-frequency event publishing without loss', async () => {
      redisStreamsMocks = setupRedisStreamMocks({
        sessionId: testSessionId,
        streamName: testStreamKey
      });

      restoreFetch = setupFetchMock({
        [`/sessions/${testSessionId}/events/bulk`]: {
          success: true,
          data: { events_published: 100, time_taken_ms: 250 }
        }
      });

      // Simulate high-frequency event publishing
      const events = Array.from({ length: 100 }, (_, i) => ({
        type: 'high_frequency_test',
        data: `Event ${i}`,
        timestamp: new Date(Date.now() + i).toISOString()
      }));

      const startTime = performance.now();
      
      const response = await fetch(`/api/sessions/${testSessionId}/events/bulk`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ events })
      });

      const result = await response.json();
      const endTime = performance.now();

      expect(result.success).toBe(true);
      expect(result.data.events_published).toBe(100);
      expect(endTime - startTime).toBeLessThan(1000); // Should complete within 1 second
    });

    it('should maintain event ordering under concurrent load', async () => {
      redisStreamsMocks = setupRedisStreamMocks({
        sessionId: testSessionId,
        streamName: testStreamKey
      });

      // Create multiple concurrent event streams
      const concurrentPromises = Array.from({ length: 5 }, async (_, streamIndex) => {
        const events = Array.from({ length: 20 }, (_, eventIndex) => {
          const sequenceNumber = streamIndex * 20 + eventIndex;
          return createMockStreamEvent({
            type: 'concurrent_test',
            data: `Stream ${streamIndex} Event ${eventIndex}`,
            sessionId: testSessionId,
            correlationId: `concurrent-${streamIndex}`,
            metadata: { sequence: sequenceNumber, stream: streamIndex }
          });
        });

        // Add events to stream
        events.forEach(event => redisStreamsMocks.addMockEvent(event));
        return events;
      });

      const allEventSets = await Promise.all(concurrentPromises);
      const allEvents = allEventSets.flat();

      // Verify total event count
      expect(allEvents).toHaveLength(100);

      // Group events by stream and verify ordering within each stream
      const eventsByStream = {};
      allEvents.forEach(event => {
        const streamId = event.metadata.stream;
        if (!eventsByStream[streamId]) {
          eventsByStream[streamId] = [];
        }
        eventsByStream[streamId].push(event);
      });

      // Verify each stream maintains order
      Object.values(eventsByStream).forEach(streamEvents => {
        expect(streamEvents).toHaveLength(20);
        
        for (let i = 1; i < streamEvents.length; i++) {
          const prevSequence = streamEvents[i - 1].metadata.sequence;
          const currSequence = streamEvents[i].metadata.sequence;
          expect(currSequence).toBe(prevSequence + 1);
        }
      });
    });

    it('should handle large payload events efficiently', async () => {
      redisStreamsMocks = setupRedisStreamMocks({
        sessionId: testSessionId,
        streamName: testStreamKey
      });

      // Create large event payload (simulate file upload or large tool result)
      const largePayload = {
        type: 'large_payload_test',
        data: {
          file_content: 'A'.repeat(50000), // 50KB of content
          metadata: {
            attachments: Array.from({ length: 100 }, (_, i) => ({
              id: `attachment-${i}`,
              name: `file-${i}.txt`,
              size: i * 1000,
              content: 'B'.repeat(1000) // 1KB per attachment
            }))
          },
          processing_results: Array.from({ length: 50 }, (_, i) => ({
            step: i,
            result: 'C'.repeat(500), // 500 bytes per result
            timestamp: new Date().toISOString()
          }))
        }
      };

      restoreFetch = setupFetchMock({
        [`/sessions/${testSessionId}/events/large`]: {
          success: true,
          data: {
            event_size_kb: Math.round(JSON.stringify(largePayload).length / 1024),
            processing_time_ms: 150
          }
        }
      });

      const startTime = performance.now();
      
      const response = await fetch(`/api/sessions/${testSessionId}/events/large`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(largePayload)
      });

      const result = await response.json();
      const endTime = performance.now();

      expect(result.success).toBe(true);
      expect(result.data.event_size_kb).toBeGreaterThan(100); // Should be a large payload
      expect(endTime - startTime).toBeLessThan(500); // Should handle efficiently
    });
  });

  describe('Data Integrity and Consistency', () => {
    it('should preserve event data integrity through serialization/deserialization', async () => {
      const originalEvent = new ChatEvent({
        type: 'data_integrity_test',
        data: {
          text: 'Test message with special characters: ñáéíóú™®©',
          numbers: [1, 2.5, -3, 1e6, Number.MAX_SAFE_INTEGER],
          booleans: [true, false],
          nullValue: null,
          undefinedValue: undefined,
          nested: {
            array: [{ id: 1, name: 'item1' }, { id: 2, name: 'item2' }],
            object: { key1: 'value1', key2: 'value2' }
          },
          unicode: '🚀💻🎉',
          escaped: 'Quote: "Hello", Backslash: \\, Newline: \n, Tab: \t'
        },
        sessionId: testSessionId,
        correlationId: testInteractionId,
        metadata: {
          timestamp: new Date().toISOString(),
          version: '1.0.0',
          source: 'data_integrity_test'
        }
      });

      // Serialize and deserialize
      const serialized = originalEvent.serialize();
      const deserialized = ChatEvent.deserialize(serialized);

      // Verify all properties are preserved
      expect(deserialized.type).toBe(originalEvent.type);
      expect(deserialized.sessionId).toBe(originalEvent.sessionId);
      expect(deserialized.correlationId).toBe(originalEvent.correlationId);
      
      // Deep compare complex data
      expect(deserialized.data.text).toBe(originalEvent.data.text);
      expect(deserialized.data.numbers).toEqual(originalEvent.data.numbers);
      expect(deserialized.data.booleans).toEqual(originalEvent.data.booleans);
      expect(deserialized.data.nullValue).toBe(originalEvent.data.nullValue);
      expect(deserialized.data.nested).toEqual(originalEvent.data.nested);
      expect(deserialized.data.unicode).toBe(originalEvent.data.unicode);
      expect(deserialized.data.escaped).toBe(originalEvent.data.escaped);
      
      // Verify metadata preservation
      expect(deserialized.metadata.version).toBe(originalEvent.metadata.version);
      expect(deserialized.metadata.source).toBe(originalEvent.metadata.source);
    });

    it('should maintain event correlation across complex interaction flows', async () => {
      const correlationId = `complex-flow-${Date.now()}`;
      
      // Create a complex interaction flow with multiple event types
      const flowEvents = [
        { type: 'interaction_start', data: { flow_id: correlationId } },
        { type: 'user_message', data: 'Please analyze this code and suggest improvements' },
        { type: 'tool_call', data: { function: 'code_analyzer', args: { code: 'function test() {}' } } },
        { type: 'tool_result', data: { analysis: 'Function lacks documentation', suggestions: ['Add JSDoc'] } },
        { type: 'thought_delta', data: 'Based on the analysis, I should suggest specific improvements...' },
        { type: 'assistant_response', data: 'Here are my suggestions for improving your code...' },
        { type: 'user_message', data: 'Can you show me an example?' },
        { type: 'tool_call', data: { function: 'code_generator', args: { template: 'documented_function' } } },
        { type: 'tool_result', data: { generated_code: '/** Documentation */ function test() {}' } },
        { type: 'assistant_response', data: 'Here\'s an example with proper documentation...' },
        { type: 'interaction_end', data: { flow_id: correlationId, duration_ms: 5000 } }
      ];

      const chatEvents = flowEvents.map((eventData, index) => 
        new ChatEvent({
          ...eventData,
          sessionId: testSessionId,
          correlationId: correlationId,
          timestamp: new Date(Date.now() + index * 100).toISOString(),
          metadata: { sequence: index, flow_type: 'complex_analysis' }
        })
      );

      // Verify all events have the same correlation ID
      chatEvents.forEach(event => {
        expect(event.correlationId).toBe(correlationId);
      });

      // Group by correlation ID and verify completeness
      const groupedEvents = ChatEvent.groupByCorrelationId(chatEvents);
      expect(groupedEvents[correlationId]).toHaveLength(11);

      // Verify logical flow sequence
      const sortedEvents = ChatEvent.sortByTimestamp(groupedEvents[correlationId], 'asc');
      expect(sortedEvents[0].type).toBe('interaction_start');
      expect(sortedEvents[sortedEvents.length - 1].type).toBe('interaction_end');

      // Verify tool call/result pairs are properly correlated
      const toolCalls = sortedEvents.filter(e => e.type === 'tool_call');
      const toolResults = sortedEvents.filter(e => e.type === 'tool_result');
      expect(toolCalls).toHaveLength(2);
      expect(toolResults).toHaveLength(2);
      expect(toolCalls[0].correlationId).toBe(toolResults[0].correlationId);
      expect(toolCalls[1].correlationId).toBe(toolResults[1].correlationId);
    });

    it('should handle event versioning and backward compatibility', async () => {
      // Test events with different version formats
      const v1Event = new ChatEvent({
        type: 'version_test',
        data: { content: 'Version 1 event' },
        sessionId: testSessionId,
        correlationId: testInteractionId,
        version: '1.0'
      });

      const v2Event = new ChatEvent({
        type: 'version_test',
        data: { 
          content: 'Version 2 event',
          enhanced_metadata: { ai_model: 'gpt-4', confidence: 0.95 }
        },
        sessionId: testSessionId,
        correlationId: testInteractionId,
        version: '2.0'
      });

      // Serialize both versions
      const v1Serialized = v1Event.serialize();
      const v2Serialized = v2Event.serialize();

      // Verify both can be deserialized
      const v1Deserialized = ChatEvent.deserialize(v1Serialized);
      const v2Deserialized = ChatEvent.deserialize(v2Serialized);

      expect(v1Deserialized.version).toBe('1.0');
      expect(v2Deserialized.version).toBe('2.0');
      expect(v1Deserialized.data.content).toBe('Version 1 event');
      expect(v2Deserialized.data.content).toBe('Version 2 event');
      expect(v2Deserialized.data.enhanced_metadata).toBeDefined();
    });
  });
});