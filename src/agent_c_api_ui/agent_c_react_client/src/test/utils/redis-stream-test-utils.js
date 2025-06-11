import { vi } from 'vitest'
import { ChatEvent } from '../../components/chat_interface/utils/ChatEvent'
import { createMockResponse } from './api-test-utils'

/**
 * Redis Stream Test Utilities
 * 
 * Provides specialized utilities for testing Redis stream integration
 * including mock stream data generation and stream response simulation.
 */

/**
 * Generate mock Redis stream event data
 * @param {Object} options - Configuration options for mock data
 * @returns {Object} Mock stream event data
 */
export const createMockStreamEvent = (options = {}) => {
  const {
    type = 'text_delta',
    data = 'Mock event data',
    sessionId = 'test-session',
    streamId = null,
    correlationId = null,
    role = 'assistant',
    vendor = 'openai',
    timestamp = new Date().toISOString(),
    metadata = {}
  } = options;

  return {
    id: `event-${Date.now()}-${Math.random().toString(36).substring(2, 9)}`,
    type,
    data: typeof data === 'object' ? JSON.stringify(data) : data,
    sessionId,
    streamId: streamId || `stream-${sessionId}-${type}`,
    correlationId: correlationId || `correlation-${Date.now()}`,
    timestamp,
    role,
    vendor,
    format: 'markdown',
    metadata: JSON.stringify(metadata),
    version: '1.0'
  };
};

/**
 * Generate a series of related mock events with the same correlation ID
 * @param {number} count - Number of events to generate
 * @param {Object} baseOptions - Base options for all events
 * @returns {Array} Array of related mock events
 */
export const createMockEventSeries = (count = 3, baseOptions = {}) => {
  const correlationId = baseOptions.correlationId || `series-${Date.now()}`;
  const sessionId = baseOptions.sessionId || 'test-session';
  const baseTimestamp = Date.now();

  const eventTypes = ['user_message', 'tool_call', 'tool_result', 'assistant_response'];
  
  return Array.from({ length: count }, (_, index) => {
    return createMockStreamEvent({
      ...baseOptions,
      type: eventTypes[index % eventTypes.length] || 'generic_event',
      data: `Event ${index + 1} of ${count}`,
      sessionId,
      correlationId,
      timestamp: new Date(baseTimestamp + index * 1000).toISOString(),
      metadata: {
        sequence: index + 1,
        total: count,
        ...baseOptions.metadata
      }
    });
  });
};

/**
 * Create mock Redis stream response for API endpoints
 * @param {Object} options - Response configuration
 * @returns {Object} Mock API response
 */
export const createMockStreamResponse = (options = {}) => {
  const {
    streamName = 'agent_events:test-session:chat',
    sessionId = 'test-session',
    events = [],
    totalEvents = events.length,
    operation = 'get_events'
  } = options;

  const baseResponse = {
    success: true,
    data: {
      stream_name: streamName,
      session_id: sessionId
    }
  };

  switch (operation) {
    case 'create_stream':
      return {
        ...baseResponse,
        data: {
          ...baseResponse.data,
          created_at: new Date().toISOString(),
          max_length: 10000,
          ttl: 86400
        }
      };

    case 'publish_event':
      return {
        ...baseResponse,
        data: {
          ...baseResponse.data,
          event_id: `event-${Date.now()}`,
          timestamp: new Date().toISOString()
        }
      };

    case 'get_events':
      return {
        ...baseResponse,
        data: {
          events: events,
          stream_info: {
            name: streamName,
            total_events: totalEvents,
            first_event: events.length > 0 ? events[0].id : null,
            last_event: events.length > 0 ? events[events.length - 1].id : null,
            last_generated_id: events.length > 0 ? events[events.length - 1].id : '0-0'
          }
        }
      };

    case 'list_streams':
      return {
        ...baseResponse,
        data: {
          streams: [
            {
              name: streamName,
              event_count: totalEvents,
              last_event: events.length > 0 ? events[events.length - 1].timestamp : null,
              created_at: new Date().toISOString()
            }
          ]
        }
      };

    case 'stream_stats':
      return {
        ...baseResponse,
        data: {
          name: streamName,
          length: totalEvents,
          first_entry_id: events.length > 0 ? events[0].id : null,
          last_entry_id: events.length > 0 ? events[events.length - 1].id : null,
          last_generated_id: events.length > 0 ? events[events.length - 1].id : '0-0',
          groups: 0,
          consumers_count: 0,
          pending_count: 0
        }
      };

    default:
      return baseResponse;
  }
};

/**
 * Setup comprehensive mocks for Redis stream API endpoints
 * @param {Object} config - Mock configuration
 * @returns {Function} Cleanup function to restore original fetch
 */
export const setupRedisStreamMocks = (config = {}) => {
  const {
    sessionId = 'test-session',
    streamName = `agent_events:${sessionId}:chat`,
    events = [],
    enableSubscription = false,
    simulateErrors = false
  } = config;

  const originalFetch = global.fetch;
  const mockEvents = [...events];

  // Mock EventSource for subscriptions if enabled
  let mockEventSource = null;
  if (enableSubscription) {
    mockEventSource = {
      onmessage: null,
      onerror: null,
      onopen: null,
      close: vi.fn(),
      readyState: 1,
      CONNECTING: 0,
      OPEN: 1,
      CLOSED: 2
    };
    global.EventSource = vi.fn(() => mockEventSource);
  }

  global.fetch = vi.fn((url, options = {}) => {
    const method = options.method || 'GET';
    
    // Handle different endpoint patterns
    if (url.includes(`/sessions/${sessionId}/streams/${streamName}`)) {
      if (method === 'POST' && url.endsWith(`/streams/${streamName}`)) {
        // Create stream
        if (simulateErrors) {
          return Promise.resolve(createMockResponse(
            { detail: { message: 'Stream already exists' } },
            { status: 409 }
          ));
        }
        return Promise.resolve(createMockResponse(
          createMockStreamResponse({ 
            streamName, 
            sessionId, 
            operation: 'create_stream' 
          })
        ));
      }
      
      if (method === 'POST' && url.includes('/events')) {
        // Publish event
        if (simulateErrors) {
          return Promise.resolve(createMockResponse(
            { detail: { message: 'Failed to publish event' } },
            { status: 500 }
          ));
        }
        
        // Add event to mock events array
        const eventData = JSON.parse(options.body || '{}');
        const newEvent = createMockStreamEvent({
          ...eventData,
          sessionId,
          streamId: streamName
        });
        mockEvents.push(newEvent);
        
        return Promise.resolve(createMockResponse(
          createMockStreamResponse({ 
            streamName, 
            sessionId, 
            operation: 'publish_event' 
          })
        ));
      }
      
      if (method === 'GET' && url.includes('/events')) {
        // Get events
        if (simulateErrors) {
          return Promise.resolve(createMockResponse(
            { detail: { message: 'Stream not found' } },
            { status: 404 }
          ));
        }
        return Promise.resolve(createMockResponse(
          createMockStreamResponse({ 
            streamName, 
            sessionId, 
            events: mockEvents, 
            operation: 'get_events' 
          })
        ));
      }
      
      if (method === 'GET' && url.includes('/stats')) {
        // Get stream stats
        return Promise.resolve(createMockResponse(
          createMockStreamResponse({ 
            streamName, 
            sessionId, 
            events: mockEvents, 
            operation: 'stream_stats' 
          })
        ));
      }
      
      if (method === 'DELETE') {
        // Delete stream
        return Promise.resolve(createMockResponse({ success: true }));
      }
    }
    
    if (url.includes(`/sessions/${sessionId}/streams`) && method === 'GET') {
      // List streams
      return Promise.resolve(createMockResponse(
        createMockStreamResponse({ 
          streamName, 
          sessionId, 
          events: mockEvents, 
          operation: 'list_streams' 
        })
      ));
    }

    // Default response for unmatched endpoints
    return Promise.resolve(createMockResponse(
      { message: 'Endpoint not mocked' },
      { status: 404 }
    ));
  });

  // Return cleanup function and utilities
  return {
    // Cleanup function
    restore: () => {
      global.fetch = originalFetch;
      if (enableSubscription && global.EventSource.mockRestore) {
        global.EventSource.mockRestore();
      }
    },
    
    // Utilities for test interaction
    getMockEvents: () => [...mockEvents],
    addMockEvent: (event) => mockEvents.push(event),
    clearMockEvents: () => mockEvents.splice(0, mockEvents.length),
    
    // EventSource utilities (if enabled)
    getEventSource: () => mockEventSource,
    simulateEvent: (eventData) => {
      if (mockEventSource && mockEventSource.onmessage) {
        mockEventSource.onmessage({ 
          data: JSON.stringify(eventData) 
        });
      }
    },
    simulateError: (error) => {
      if (mockEventSource && mockEventSource.onerror) {
        mockEventSource.onerror(error);
      }
    },
    simulateOpen: () => {
      if (mockEventSource && mockEventSource.onopen) {
        mockEventSource.onopen();
      }
    }
  };
};

/**
 * Create a series of ChatEvent instances for testing
 * @param {Object} options - Configuration options
 * @returns {Array<ChatEvent>} Array of ChatEvent instances
 */
export const createTestChatEvents = (options = {}) => {
  const {
    count = 5,
    sessionId = 'test-session',
    correlationId = `test-correlation-${Date.now()}`,
    startTimestamp = Date.now()
  } = options;

  const eventTypes = [
    { type: 'session_start', data: 'Session initialized' },
    { type: 'user_message', data: 'Hello, how can I get help?' },
    { type: 'tool_call', data: { function: 'search', args: { query: 'help' } } },
    { type: 'tool_result', data: { results: ['Help documentation', 'FAQ'] } },
    { type: 'assistant_response', data: 'I can help you with various tasks...' }
  ];

  return Array.from({ length: count }, (_, index) => {
    const eventConfig = eventTypes[index % eventTypes.length];
    return new ChatEvent({
      type: eventConfig.type,
      data: eventConfig.data,
      sessionId,
      correlationId,
      streamId: `stream-${sessionId}-${eventConfig.type}`,
      timestamp: new Date(startTimestamp + index * 1000).toISOString(),
      role: eventConfig.type.includes('user') ? 'user' : 'assistant',
      vendor: 'openai',
      metadata: {
        test_index: index,
        test_total: count,
        test_run_id: `test-${Date.now()}`
      }
    });
  });
};

/**
 * Validate ChatEvent instances for test assertions
 * @param {ChatEvent} event - Event to validate
 * @param {Object} expected - Expected properties
 * @returns {Object} Validation result
 */
export const validateChatEvent = (event, expected = {}) => {
  const errors = [];
  
  // Required properties
  if (!event.type) errors.push('Missing event type');
  if (!event.id) errors.push('Missing event ID');
  if (!event.timestamp) errors.push('Missing timestamp');
  if (!event.correlationId) errors.push('Missing correlation ID');
  
  // Expected properties
  Object.entries(expected).forEach(([key, value]) => {
    if (event[key] !== value) {
      errors.push(`Expected ${key} to be ${value}, got ${event[key]}`);
    }
  });
  
  // Timestamp validation
  if (event.timestamp) {
    const timestamp = new Date(event.timestamp);
    if (isNaN(timestamp.getTime())) {
      errors.push('Invalid timestamp format');
    }
  }
  
  return {
    isValid: errors.length === 0,
    errors
  };
};

export default {
  createMockStreamEvent,
  createMockEventSeries,
  createMockStreamResponse,
  setupRedisStreamMocks,
  createTestChatEvents,
  validateChatEvent
};