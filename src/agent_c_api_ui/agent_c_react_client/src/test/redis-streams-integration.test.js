import { describe, it, expect, beforeEach, afterEach, vi, beforeAll } from 'vitest'
import { ChatEvent } from '../components/chat_interface/utils/ChatEvent'
import * as sessionApi from '../services/session-api'
import { REDIS_STREAMS_CONFIG, getSessionStreamName, areRedisStreamsEnabled } from '../config/config'
import { setupFetchMock, createMockResponse } from './utils/api-test-utils'

describe('Redis Streams Integration - End-to-End Tests', () => {
  let restoreFetch;
  let testSessionId;
  let testStreamName;
  let testCorrelationId;

  beforeEach(() => {
    // Setup test data
    testSessionId = 'test-session-123';
    testStreamName = 'agent_events:test-session-123:chat';
    testCorrelationId = `interaction-${Date.now()}-abc123`;
    
    // Clear any environment variable mocks
    vi.clearAllMocks();
  });

  afterEach(() => {
    // Clean up any fetch mocks
    if (restoreFetch) {
      restoreFetch();
      restoreFetch = null;
    }
  });

  describe('Configuration Integration', () => {
    it('should load Redis streams configuration from environment', () => {
      // Verify configuration structure
      expect(REDIS_STREAMS_CONFIG).toBeDefined();
      expect(REDIS_STREAMS_CONFIG).toHaveProperty('enabled');
      expect(REDIS_STREAMS_CONFIG).toHaveProperty('host');
      expect(REDIS_STREAMS_CONFIG).toHaveProperty('port');
      expect(REDIS_STREAMS_CONFIG).toHaveProperty('prefixes');
    });

    it('should generate correct stream names', () => {
      const sessionId = 'session-456';
      const chatStreamName = getSessionStreamName(sessionId, 'chat');
      const debugStreamName = getSessionStreamName(sessionId, 'debug');

      expect(chatStreamName).toBe(`agent_events:${sessionId}:chat`);
      expect(debugStreamName).toBe(`agent_events:${sessionId}:debug`);
    });

    it('should provide Redis streams availability check', () => {
      const isEnabled = areRedisStreamsEnabled();
      expect(typeof isEnabled).toBe('boolean');
    });
  });

  describe('ChatEvent Class - Redis Stream Integration', () => {
    it('should create ChatEvent with stream metadata', () => {
      const eventData = {
        type: 'text_delta',
        data: 'Hello, world!',
        sessionId: testSessionId,
        streamId: 'stream-id-123',
        correlationId: testCorrelationId,
        role: 'assistant',
        vendor: 'openai'
      };

      const event = new ChatEvent(eventData);

      expect(event.type).toBe('text_delta');
      expect(event.data).toBe('Hello, world!');
      expect(event.sessionId).toBe(testSessionId);
      expect(event.streamId).toBe('stream-id-123');
      expect(event.correlationId).toBe(testCorrelationId);
      expect(event.role).toBe('assistant');
      expect(event.vendor).toBe('openai');
    });

    it('should serialize and deserialize ChatEvent for Redis storage', () => {
      const originalEvent = new ChatEvent({
        type: 'tool_call',
        data: { function: 'search', args: { query: 'test' } },
        sessionId: testSessionId,
        streamId: 'stream-456',
        correlationId: testCorrelationId,
        metadata: { source: 'user_interaction', priority: 'high' }
      });

      // Serialize for Redis storage
      const serialized = originalEvent.serialize();
      expect(serialized).toHaveProperty('id');
      expect(serialized).toHaveProperty('type', 'tool_call');
      expect(serialized).toHaveProperty('sessionId', testSessionId);
      expect(serialized).toHaveProperty('streamId', 'stream-456');

      // Deserialize from Redis data
      const deserialized = ChatEvent.deserialize(serialized);
      expect(deserialized.type).toBe(originalEvent.type);
      expect(deserialized.sessionId).toBe(originalEvent.sessionId);
      expect(deserialized.streamId).toBe(originalEvent.streamId);
      expect(deserialized.correlationId).toBe(originalEvent.correlationId);
      expect(deserialized.metadata).toEqual(originalEvent.metadata);
    });

    it('should handle event correlation and grouping', () => {
      const correlationId = 'shared-correlation-123';
      
      const event1 = new ChatEvent({
        type: 'user_message',
        data: 'Question',
        correlationId
      });

      const event2 = new ChatEvent({
        type: 'assistant_response',
        data: 'Answer',
        correlationId
      });

      const event3 = new ChatEvent({
        type: 'tool_call',
        data: 'Search result',
        correlationId: 'different-correlation'
      });

      // Test correlation detection
      expect(event1.isCorrelated(event2)).toBe(true);
      expect(event1.isCorrelated(event3)).toBe(false);
      expect(event2.isCorrelated(correlationId)).toBe(true);

      // Test grouping
      const events = [event1, event2, event3];
      const grouped = ChatEvent.groupByCorrelationId(events);
      
      expect(grouped[correlationId]).toHaveLength(2);
      expect(grouped['different-correlation']).toHaveLength(1);
    });

    it('should create ChatEvent from stream data', () => {
      const rawStreamData = {
        type: 'system_message',
        content: 'System initialized',
        role: 'system',
        vendor: 'internal',
        timestamp: '2024-01-01T10:00:00Z',
        metadata: { system_level: 'info' }
      };

      const options = {
        sessionId: testSessionId,
        streamId: 'system-stream-789',
        correlationId: testCorrelationId,
        metadata: { processed_by: 'test_handler' }
      };

      const event = ChatEvent.fromStreamData(rawStreamData, options);

      expect(event.type).toBe('system_message');
      expect(event.data).toBe('System initialized');
      expect(event.sessionId).toBe(testSessionId);
      expect(event.streamId).toBe('system-stream-789');
      expect(event.correlationId).toBe(testCorrelationId);
      expect(event.metadata.system_level).toBe('info');
      expect(event.metadata.processed_by).toBe('test_handler');
      expect(event.metadata.originalRaw).toEqual(rawStreamData);
    });
  });

  describe('Session API - Redis Stream Methods', () => {
    beforeEach(() => {
      // Setup fetch mocks for Redis stream endpoints
      restoreFetch = setupFetchMock({
        [`/sessions/${testSessionId}/streams/${testStreamName}`]: {
          success: true,
          data: {
            stream_name: testStreamName,
            session_id: testSessionId,
            created_at: '2024-01-01T10:00:00Z',
            max_length: 10000,
            ttl: 86400
          }
        },
        [`/sessions/${testSessionId}/streams/${testStreamName}/events`]: {
          success: true,
          data: {
            event_id: 'event-123-456',
            stream_name: testStreamName,
            timestamp: '2024-01-01T10:00:00Z'
          }
        },
        [`/sessions/${testSessionId}/streams`]: {
          success: true,
          data: {
            streams: [
              { name: testStreamName, event_count: 5, last_event: '2024-01-01T10:00:00Z' }
            ]
          }
        }
      });
    });

    it('should create event stream successfully', async () => {
      const options = {
        max_length: REDIS_STREAMS_CONFIG.maxLength,
        ttl: REDIS_STREAMS_CONFIG.ttl
      };

      const result = await sessionApi.createEventStream(testSessionId, testStreamName, options);

      expect(result).toHaveProperty('stream_name', testStreamName);
      expect(result).toHaveProperty('session_id', testSessionId);
      expect(result).toHaveProperty('max_length', 10000);
      expect(result).toHaveProperty('ttl', 86400);
    });

    it('should publish event to stream successfully', async () => {
      const eventData = {
        type: 'user_message',
        content: 'Test message',
        timestamp: new Date().toISOString(),
        metadata: { source: 'test' }
      };

      const result = await sessionApi.publishEvent(testSessionId, testStreamName, eventData);

      expect(result).toHaveProperty('event_id');
      expect(result).toHaveProperty('stream_name', testStreamName);
      expect(result).toHaveProperty('timestamp');
    });

    it('should retrieve events from stream with options', async () => {
      // Mock response for getting events
      const eventsResponse = {
        success: true,
        data: {
          events: [
            {
              id: 'event-1',
              type: 'user_message',
              data: 'First message',
              timestamp: '2024-01-01T09:00:00Z'
            },
            {
              id: 'event-2',
              type: 'assistant_response',
              data: 'Response to first message',
              timestamp: '2024-01-01T09:01:00Z'
            }
          ],
          stream_info: {
            name: testStreamName,
            total_events: 2,
            first_event: 'event-1',
            last_event: 'event-2'
          }
        }
      };

      restoreFetch();
      restoreFetch = setupFetchMock({
        [`/sessions/${testSessionId}/streams/${testStreamName}/events`]: eventsResponse
      });

      const options = { start: '0', count: 10 };
      const result = await sessionApi.getEventStream(testSessionId, testStreamName, options);

      expect(result.data.events).toHaveLength(2);
      expect(result.data.events[0]).toHaveProperty('type', 'user_message');
      expect(result.data.events[1]).toHaveProperty('type', 'assistant_response');
      expect(result.data.stream_info.total_events).toBe(2);
    });

    it('should list event streams for session', async () => {
      const result = await sessionApi.listEventStreams(testSessionId);

      expect(result).toHaveLength(1);
      expect(result[0]).toHaveProperty('name', testStreamName);
      expect(result[0]).toHaveProperty('event_count', 5);
    });

    it('should handle stream creation errors gracefully', async () => {
      restoreFetch();
      restoreFetch = setupFetchMock({
        [`/sessions/${testSessionId}/streams/${testStreamName}`]: {
          detail: { message: 'Stream already exists' },
          status: 409
        }
      });

      await expect(
        sessionApi.createEventStream(testSessionId, testStreamName)
      ).rejects.toThrow('Stream already exists');
    });
  });

  describe('End-to-End Integration Flow', () => {
    let mockStreamEvents;

    beforeEach(() => {
      // Setup comprehensive mock for full integration flow
      mockStreamEvents = [];
      
      restoreFetch = setupFetchMock({
        // Stream creation
        [`/sessions/${testSessionId}/streams/${testStreamName}`]: {
          success: true,
          data: {
            stream_name: testStreamName,
            session_id: testSessionId,
            created_at: new Date().toISOString()
          }
        },
        // Event publishing
        [`/sessions/${testSessionId}/streams/${testStreamName}/events`]: {
          success: true,
          data: {
            event_id: `event-${Date.now()}`,
            stream_name: testStreamName,
            timestamp: new Date().toISOString()
          }
        }
      });

      // Mock for event retrieval that returns published events
      const originalFetch = global.fetch;
      global.fetch = vi.fn((url, options) => {
        if (url.includes('/events') && options?.method === 'GET') {
          return Promise.resolve(createMockResponse({
            success: true,
            data: {
              events: mockStreamEvents.map(event => event.serialize()),
              stream_info: {
                name: testStreamName,
                total_events: mockStreamEvents.length
              }
            }
          }));
        }
        return originalFetch(url, options);
      });
    });

    it('should complete full event lifecycle: create → publish → retrieve → correlate', async () => {
      // Step 1: Create event stream
      const streamResult = await sessionApi.createEventStream(testSessionId, testStreamName, {
        max_length: 1000,
        ttl: 3600
      });
      expect(streamResult.stream_name).toBe(testStreamName);

      // Step 2: Create ChatEvent instances
      const userEvent = new ChatEvent({
        type: 'user_message',
        data: 'Hello, how can you help me?',
        sessionId: testSessionId,
        streamId: streamResult.stream_name,
        correlationId: testCorrelationId,
        role: 'user',
        metadata: { ui_component: 'chat_interface' }
      });

      const assistantEvent = new ChatEvent({
        type: 'assistant_response',
        data: 'I can help you with various tasks...',
        sessionId: testSessionId,
        streamId: streamResult.stream_name,
        correlationId: testCorrelationId,
        role: 'assistant',
        vendor: 'openai',
        metadata: { model: 'gpt-4', temperature: 0.7 }
      });

      // Step 3: Publish events to stream
      const userPublishResult = await sessionApi.publishEvent(
        testSessionId, 
        testStreamName, 
        userEvent.serialize()
      );
      mockStreamEvents.push(userEvent);

      const assistantPublishResult = await sessionApi.publishEvent(
        testSessionId, 
        testStreamName, 
        assistantEvent.serialize()
      );
      mockStreamEvents.push(assistantEvent);

      expect(userPublishResult.event_id).toBeDefined();
      expect(assistantPublishResult.event_id).toBeDefined();

      // Step 4: Retrieve events from stream
      const retrievedEvents = await sessionApi.getEventStream(testSessionId, testStreamName, {
        start: '0',
        count: 10
      });

      expect(retrievedEvents.data.events).toHaveLength(2);
      expect(retrievedEvents.data.stream_info.total_events).toBe(2);

      // Step 5: Reconstruct ChatEvent instances from retrieved data
      const reconstructedEvents = retrievedEvents.data.events.map(eventData => 
        ChatEvent.deserialize(eventData)
      );

      // Step 6: Verify event correlation and data integrity
      expect(reconstructedEvents[0].correlationId).toBe(testCorrelationId);
      expect(reconstructedEvents[1].correlationId).toBe(testCorrelationId);
      expect(reconstructedEvents[0].isCorrelated(reconstructedEvents[1])).toBe(true);

      // Verify original data is preserved
      expect(reconstructedEvents[0].type).toBe('user_message');
      expect(reconstructedEvents[0].data).toBe('Hello, how can you help me?');
      expect(reconstructedEvents[1].type).toBe('assistant_response');
      expect(reconstructedEvents[1].metadata.model).toBe('gpt-4');

      // Step 7: Test event grouping and analysis
      const groupedEvents = ChatEvent.groupByCorrelationId(reconstructedEvents);
      expect(groupedEvents[testCorrelationId]).toHaveLength(2);

      const sortedEvents = ChatEvent.sortByTimestamp(reconstructedEvents, 'asc');
      expect(sortedEvents[0].timestamp <= sortedEvents[1].timestamp).toBe(true);
    });

    it('should handle event stream subscription simulation', async () => {
      // Simulate real-time event subscription
      const receivedEvents = [];
      let subscriptionActive = false;

      // Mock EventSource for subscription testing
      const mockEventSource = {
        onmessage: null,
        onerror: null,
        close: vi.fn(() => { subscriptionActive = false; }),
        readyState: 1
      };

      // Mock EventSource constructor
      global.EventSource = vi.fn(() => mockEventSource);

      // Create subscription
      const subscription = await sessionApi.subscribeToEventStream(
        testSessionId,
        testStreamName,
        (eventData) => {
          receivedEvents.push(eventData);
        }
      );

      subscriptionActive = true;

      // Simulate receiving events
      const event1 = new ChatEvent({
        type: 'tool_call',
        data: { function: 'search', args: { query: 'redis streams' } },
        sessionId: testSessionId,
        correlationId: testCorrelationId
      });

      const event2 = new ChatEvent({
        type: 'tool_result',
        data: { results: ['Redis Streams documentation', 'Best practices'] },
        sessionId: testSessionId,
        correlationId: testCorrelationId
      });

      // Simulate EventSource message events
      mockEventSource.onmessage({ data: JSON.stringify(event1.serialize()) });
      mockEventSource.onmessage({ data: JSON.stringify(event2.serialize()) });

      // Verify events were received
      expect(receivedEvents).toHaveLength(2);
      expect(receivedEvents[0].type).toBe('tool_call');
      expect(receivedEvents[1].type).toBe('tool_result');

      // Test subscription cleanup
      subscription.close();
      expect(subscription.close).toHaveBeenCalled();
      expect(subscriptionActive).toBe(false);
    });

    it('should handle error recovery in stream operations', async () => {
      // Test publishing to non-existent stream
      restoreFetch();
      restoreFetch = setupFetchMock({
        [`/sessions/${testSessionId}/streams/non-existent-stream/events`]: {
          detail: { message: 'Stream not found' },
          status: 404
        }
      });

      const event = new ChatEvent({
        type: 'error_test',
        data: 'This should fail',
        sessionId: testSessionId
      });

      await expect(
        sessionApi.publishEvent(testSessionId, 'non-existent-stream', event.serialize())
      ).rejects.toThrow('Stream not found');

      // Test graceful handling of malformed event data
      const malformedEventData = { invalid: 'structure' };
      
      // ChatEvent should handle malformed data gracefully
      expect(() => {
        ChatEvent.deserialize(malformedEventData);
      }).toThrow('Failed to deserialize ChatEvent');
    });
  });

  describe('Integration with UI Components', () => {
    it('should integrate with MessageStreamProcessor pattern', () => {
      // Test the pattern used in MessageStreamProcessor for ChatEvent creation
      const rawStreamData = {
        type: 'text_delta',
        data: 'Streaming text...',
        role: 'assistant',
        vendor: 'anthropic',
        metadata: { model_id: 'claude-3' }
      };

      const processingOptions = {
        sessionId: testSessionId,
        streamId: getSessionStreamName(testSessionId, 'chat'),
        correlationId: testCorrelationId,
        metadata: {
          ui_component: 'MessageStreamProcessor',
          processing_timestamp: new Date().toISOString()
        }
      };

      const chatEvent = ChatEvent.fromStreamData(rawStreamData, processingOptions);

      // Verify integration points
      expect(chatEvent.sessionId).toBe(testSessionId);
      expect(chatEvent.streamId).toBe(getSessionStreamName(testSessionId, 'chat'));
      expect(chatEvent.metadata.ui_component).toBe('MessageStreamProcessor');
      expect(chatEvent.metadata.originalRaw).toEqual(rawStreamData);
      
      // Verify it can be displayed in UI
      const displayFormat = chatEvent.toDisplayFormat();
      expect(displayFormat).toHaveProperty('id');
      expect(displayFormat).toHaveProperty('type', 'text_delta');
      expect(displayFormat).toHaveProperty('streamId');
      expect(displayFormat).toHaveProperty('correlationId');
    });

    it('should support replay interface integration', () => {
      // Create a series of events that would be used in replay
      const events = [
        new ChatEvent({
          type: 'session_start',
          data: { session_id: testSessionId },
          sessionId: testSessionId,
          correlationId: testCorrelationId,
          timestamp: '2024-01-01T10:00:00Z'
        }),
        new ChatEvent({
          type: 'user_message',
          data: 'Hello',
          sessionId: testSessionId,
          correlationId: testCorrelationId,
          timestamp: '2024-01-01T10:01:00Z'
        }),
        new ChatEvent({
          type: 'assistant_response',
          data: 'Hi there!',
          sessionId: testSessionId,
          correlationId: testCorrelationId,
          timestamp: '2024-01-01T10:02:00Z'
        })
      ];

      // Test replay functionality
      const sortedEvents = ChatEvent.sortByTimestamp(events, 'asc');
      expect(sortedEvents[0].type).toBe('session_start');
      expect(sortedEvents[1].type).toBe('user_message');
      expect(sortedEvents[2].type).toBe('assistant_response');

      // Test time range filtering (useful for replay controls)
      const timeRangeEvents = ChatEvent.getEventsInTimeRange(
        events,
        '2024-01-01T10:00:30Z',
        '2024-01-01T10:01:30Z'
      );
      expect(timeRangeEvents).toHaveLength(1);
      expect(timeRangeEvents[0].type).toBe('user_message');
    });
  });

  describe('Performance and Resource Management', () => {
    it('should handle large numbers of events efficiently', () => {
      // Create a large number of events
      const largeEventSet = Array.from({ length: 1000 }, (_, index) => 
        new ChatEvent({
          type: 'performance_test',
          data: `Event ${index}`,
          sessionId: testSessionId,
          correlationId: index % 10 === 0 ? 'batch-a' : 'batch-b', // Create some correlation groups
          timestamp: new Date(Date.now() + index * 1000).toISOString()
        })
      );

      // Test grouping performance
      const start = performance.now();
      const grouped = ChatEvent.groupByCorrelationId(largeEventSet);
      const groupingTime = performance.now() - start;

      expect(groupingTime).toBeLessThan(100); // Should complete in reasonable time
      expect(Object.keys(grouped)).toContain('batch-a');
      expect(Object.keys(grouped)).toContain('batch-b');

      // Test filtering performance
      const filterStart = performance.now();
      const filtered = ChatEvent.filterByType(largeEventSet, 'performance_test');
      const filteringTime = performance.now() - filterStart;

      expect(filteringTime).toBeLessThan(50);
      expect(filtered).toHaveLength(1000);

      // Test sorting performance
      const sortStart = performance.now();
      const sorted = ChatEvent.sortByTimestamp(largeEventSet, 'desc');
      const sortingTime = performance.now() - sortStart;

      expect(sortingTime).toBeLessThan(100);
      expect(sorted[0].timestamp > sorted[sorted.length - 1].timestamp).toBe(true);
    });

    it('should properly clean up resources in error scenarios', async () => {
      // Test resource cleanup when stream operations fail
      restoreFetch();
      restoreFetch = setupFetchMock({
        [`/sessions/${testSessionId}/streams/${testStreamName}`]: {
          detail: { message: 'Database connection failed' },
          status: 500
        }
      });

      let errorCaught = false;
      try {
        await sessionApi.createEventStream(testSessionId, testStreamName);
      } catch (error) {
        errorCaught = true;
        expect(error.message).toContain('Database connection failed');
      }

      expect(errorCaught).toBe(true);
      
      // Verify that subsequent operations can still work (no resource leaks)
      restoreFetch();
      restoreFetch = setupFetchMock({
        [`/sessions/${testSessionId}/streams/${testStreamName}`]: {
          success: true,
          data: { stream_name: testStreamName }
        }
      });

      // This should work fine after the error
      const result = await sessionApi.createEventStream(testSessionId, testStreamName);
      expect(result.stream_name).toBe(testStreamName);
    });
  });
});