import { describe, it, expect, beforeEach, afterEach, vi } from 'vitest'
import * as sessionApi from './session-api'
import { setupFetchMock, setupNetworkErrorMock, createMockResponse } from '../test/utils/api-test-utils'

describe('Session API Service', () => {
  // Store the cleanup function for each test
  let restoreFetch;
  
  afterEach(() => {
    // Clean up any fetch mocks
    if (restoreFetch) {
      restoreFetch();
      restoreFetch = null;
    }
  })

  // Test Redis Stream Management Methods
  describe('Redis Stream Management', () => {
    const sessionId = 'test-session-123';
    const streamName = 'chat-events';

    describe('createEventStream', () => {
      it('should create an event stream successfully', async () => {
        const mockOptions = {
          maxLength: 1000,
          ttl: 3600,
          compression: 'gzip'
        };

        const mockData = {
          success: true,
          data: {
            streamName: streamName,
            sessionId: sessionId,
            maxLength: 1000,
            ttl: 3600,
            createdAt: '2025-06-09T12:00:00Z',
            status: 'active'
          }
        };

        restoreFetch = setupFetchMock({
          [`/sessions/${sessionId}/streams/${streamName}`]: mockData
        });

        const result = await sessionApi.createEventStream(sessionId, streamName, mockOptions);

        expect(result).toEqual(mockData.data);
        expect(global.fetch).toHaveBeenCalledTimes(1);
        expect(global.fetch).toHaveBeenCalledWith(
          expect.stringContaining(`/sessions/${sessionId}/streams/${streamName}`),
          expect.objectContaining({
            method: 'POST',
            body: JSON.stringify(mockOptions),
            headers: expect.objectContaining({
              'Content-Type': 'application/json'
            })
          })
        );
      })

      it('should handle stream creation errors', async () => {
        restoreFetch = setupFetchMock({
          [`/sessions/${sessionId}/streams/${streamName}`]: {
            detail: { message: 'Stream already exists' },
            status: 409
          }
        });

        await expect(sessionApi.createEventStream(sessionId, streamName))
          .rejects.toThrow('Stream already exists');
        expect(global.fetch).toHaveBeenCalledTimes(1);
      })

      it('should create stream with default options', async () => {
        const mockData = {
          success: true,
          data: { streamName, sessionId, status: 'active' }
        };

        restoreFetch = setupFetchMock({
          [`/sessions/${sessionId}/streams/${streamName}`]: mockData
        });

        const result = await sessionApi.createEventStream(sessionId, streamName);

        expect(result).toEqual(mockData.data);
        expect(global.fetch).toHaveBeenCalledWith(
          expect.stringContaining(`/sessions/${sessionId}/streams/${streamName}`),
          expect.objectContaining({
            method: 'POST',
            body: JSON.stringify({})
          })
        );
      })
    })

    describe('deleteEventStream', () => {
      it('should delete an event stream successfully', async () => {
        restoreFetch = setupFetchMock({
          [`/sessions/${sessionId}/streams/${streamName}`]: { success: true }
        });

        const result = await sessionApi.deleteEventStream(sessionId, streamName);

        expect(result).toBe(true);
        expect(global.fetch).toHaveBeenCalledTimes(1);
        expect(global.fetch).toHaveBeenCalledWith(
          expect.stringContaining(`/sessions/${sessionId}/streams/${streamName}`),
          expect.objectContaining({
            method: 'DELETE'
          })
        );
      })

      it('should handle stream not found errors', async () => {
        restoreFetch = setupFetchMock({
          [`/sessions/${sessionId}/streams/${streamName}`]: {
            detail: { message: 'Stream not found' },
            status: 404
          }
        });

        await expect(sessionApi.deleteEventStream(sessionId, streamName))
          .rejects.toThrow('Stream not found');
        expect(global.fetch).toHaveBeenCalledTimes(1);
      })
    })

    describe('listEventStreams', () => {
      it('should list event streams successfully', async () => {
        const mockData = {
          success: true,
          data: {
            streams: [
              { name: 'chat-events', eventCount: 42, status: 'active' },
              { name: 'system-events', eventCount: 15, status: 'active' },
              { name: 'debug-events', eventCount: 3, status: 'inactive' }
            ]
          }
        };

        restoreFetch = setupFetchMock({
          [`/sessions/${sessionId}/streams`]: mockData
        });

        const result = await sessionApi.listEventStreams(sessionId);

        expect(result).toEqual(mockData.data.streams);
        expect(global.fetch).toHaveBeenCalledTimes(1);
        expect(global.fetch).toHaveBeenCalledWith(
          expect.stringContaining(`/sessions/${sessionId}/streams`),
          expect.objectContaining({
            method: 'GET'
          })
        );
      })

      it('should return empty array when no streams exist', async () => {
        const mockData = {
          success: true,
          data: { streams: [] }
        };

        restoreFetch = setupFetchMock({
          [`/sessions/${sessionId}/streams`]: mockData
        });

        const result = await sessionApi.listEventStreams(sessionId);

        expect(result).toEqual([]);
        expect(global.fetch).toHaveBeenCalledTimes(1);
      })

      it('should handle missing data.streams gracefully', async () => {
        const mockData = {
          success: true,
          data: {}
        };

        restoreFetch = setupFetchMock({
          [`/sessions/${sessionId}/streams`]: mockData
        });

        const result = await sessionApi.listEventStreams(sessionId);

        expect(result).toEqual([]);
        expect(global.fetch).toHaveBeenCalledTimes(1);
      })
    })
  })

  // Test Redis Event Operations
  describe('Redis Event Operations', () => {
    const sessionId = 'test-session-123';
    const streamName = 'chat-events';

    describe('publishEvent', () => {
      it('should publish an event successfully', async () => {
        const eventData = {
          type: 'user_message',
          message: 'Hello, world!',
          timestamp: '2025-06-09T12:00:00Z',
          metadata: {
            userId: 'user-123',
            conversationId: 'conv-456'
          }
        };

        const mockData = {
          success: true,
          data: {
            eventId: 'event-789',
            streamId: 'stream-id-12345',
            timestamp: '2025-06-09T12:00:00Z',
            status: 'published'
          }
        };

        restoreFetch = setupFetchMock({
          [`/sessions/${sessionId}/streams/${streamName}/events`]: mockData
        });

        const result = await sessionApi.publishEvent(sessionId, streamName, eventData);

        expect(result).toEqual(mockData.data);
        expect(global.fetch).toHaveBeenCalledTimes(1);
        expect(global.fetch).toHaveBeenCalledWith(
          expect.stringContaining(`/sessions/${sessionId}/streams/${streamName}/events`),
          expect.objectContaining({
            method: 'POST',
            body: JSON.stringify(eventData),
            headers: expect.objectContaining({
              'Content-Type': 'application/json'
            })
          })
        );
      })

      it('should handle publish errors', async () => {
        const eventData = { type: 'test', message: 'test' };

        restoreFetch = setupFetchMock({
          [`/sessions/${sessionId}/streams/${streamName}/events`]: {
            detail: { message: 'Stream is full' },
            status: 507
          }
        });

        await expect(sessionApi.publishEvent(sessionId, streamName, eventData))
          .rejects.toThrow('Stream is full');
        expect(global.fetch).toHaveBeenCalledTimes(1);
      })

      it('should handle invalid event data', async () => {
        const invalidEventData = null;

        restoreFetch = setupFetchMock({
          [`/sessions/${sessionId}/streams/${streamName}/events`]: {
            detail: { message: 'Invalid event data' },
            status: 400
          }
        });

        await expect(sessionApi.publishEvent(sessionId, streamName, invalidEventData))
          .rejects.toThrow('Invalid event data');
        expect(global.fetch).toHaveBeenCalledTimes(1);
      })
    })

    describe('getEventStream', () => {
      it('should retrieve events successfully', async () => {
        const options = {
          start: '0-0',
          end: '+',
          count: 10,
          direction: 'forward'
        };

        const mockData = {
          success: true,
          data: {
            events: [
              {
                id: 'event-1',
                timestamp: '2025-06-09T12:00:00Z',
                type: 'user_message',
                data: { message: 'Hello' }
              },
              {
                id: 'event-2',
                timestamp: '2025-06-09T12:01:00Z',
                type: 'assistant_message',
                data: { message: 'Hi there!' }
              }
            ],
            pagination: {
              total: 2,
              hasMore: false,
              nextCursor: null
            }
          }
        };

        restoreFetch = setupFetchMock({
          [`/sessions/${sessionId}/streams/${streamName}/events`]: mockData
        });

        const result = await sessionApi.getEventStream(sessionId, streamName, options);

        expect(result).toEqual(mockData);
        expect(global.fetch).toHaveBeenCalledTimes(1);
        expect(global.fetch).toHaveBeenCalledWith(
          expect.stringContaining(`/sessions/${sessionId}/streams/${streamName}/events`),
          expect.objectContaining({
            method: 'GET'
          })
        );
      })

      it('should handle empty streams', async () => {
        const mockData = {
          success: true,
          data: {
            events: [],
            pagination: { total: 0, hasMore: false, nextCursor: null }
          }
        };

        restoreFetch = setupFetchMock({
          [`/sessions/${sessionId}/streams/${streamName}/events`]: mockData
        });

        const result = await sessionApi.getEventStream(sessionId, streamName);

        expect(result.data.events).toEqual([]);
        expect(result.data.pagination.total).toBe(0);
        expect(global.fetch).toHaveBeenCalledTimes(1);
      })

      it('should pass query parameters correctly', async () => {
        const mockData = { success: true, data: { events: [] } };
        
        // Capture the request details
        let capturedUrl;
        const originalFetch = global.fetch;
        global.fetch = vi.fn((url, options) => {
          capturedUrl = url;
          return Promise.resolve({
            ok: true,
            status: 200,
            json: () => Promise.resolve(mockData),
            headers: { get: () => 'application/json' }
          });
        });
        
        restoreFetch = () => { global.fetch = originalFetch; };

        const options = { start: '0-0', count: 5, direction: 'backward' };
        await sessionApi.getEventStream(sessionId, streamName, options);

        expect(capturedUrl).toContain('start=0-0');
        expect(capturedUrl).toContain('count=5');
        expect(capturedUrl).toContain('direction=backward');
        expect(global.fetch).toHaveBeenCalledTimes(1);
      })
    })

    describe('getEventStreamStats', () => {
      it('should retrieve stream statistics successfully', async () => {
        const mockData = {
          success: true,
          data: {
            streamName: streamName,
            totalEvents: 150,
            firstEventId: '1-0',
            lastEventId: '150-0',
            memoryUsage: '2.5MB',
            lastActivity: '2025-06-09T12:00:00Z',
            status: 'active'
          }
        };

        restoreFetch = setupFetchMock({
          [`/sessions/${sessionId}/streams/${streamName}/stats`]: mockData
        });

        const result = await sessionApi.getEventStreamStats(sessionId, streamName);

        expect(result).toEqual(mockData.data);
        expect(global.fetch).toHaveBeenCalledTimes(1);
        expect(global.fetch).toHaveBeenCalledWith(
          expect.stringContaining(`/sessions/${sessionId}/streams/${streamName}/stats`),
          expect.objectContaining({
            method: 'GET'
          })
        );
      })

      it('should handle stats for non-existent stream', async () => {
        restoreFetch = setupFetchMock({
          [`/sessions/${sessionId}/streams/${streamName}/stats`]: {
            detail: { message: 'Stream not found' },
            status: 404
          }
        });

        await expect(sessionApi.getEventStreamStats(sessionId, streamName))
          .rejects.toThrow('Stream not found');
        expect(global.fetch).toHaveBeenCalledTimes(1);
      })
    })
  })

  // Test Real-time Subscriptions
  describe('Real-time Event Subscriptions', () => {
    const sessionId = 'test-session-123';
    const streamName = 'chat-events';

    describe('subscribeToEventStream', () => {
      let mockEventSource;
      let originalEventSource;

      beforeEach(() => {
        // Mock EventSource
        originalEventSource = global.EventSource;
        mockEventSource = {
          close: vi.fn(),
          readyState: 1, // OPEN
          onmessage: null,
          onerror: null,
          addEventListener: vi.fn(),
          removeEventListener: vi.fn(),
          dispatchEvent: vi.fn()
        };
        global.EventSource = vi.fn(() => mockEventSource);
      });

      afterEach(() => {
        global.EventSource = originalEventSource;
      });

      it('should set up event subscription successfully', async () => {
        const onEvent = vi.fn();
        const options = { 
          fromEventId: '0-0',
          buffer: 'off'
        };

        const subscription = await sessionApi.subscribeToEventStream(
          sessionId, 
          streamName, 
          onEvent, 
          options
        );

        expect(global.EventSource).toHaveBeenCalledTimes(1);
        expect(global.EventSource).toHaveBeenCalledWith(
          expect.stringContaining(`/sessions/${sessionId}/streams/${streamName}/subscribe`)
        );
        expect(subscription).toHaveProperty('close');
        expect(subscription).toHaveProperty('readyState');
        expect(typeof subscription.close).toBe('function');
        expect(typeof subscription.readyState).toBe('function');
      })

      it('should handle incoming events correctly', async () => {
        const onEvent = vi.fn();
        const testEventData = {
          id: 'event-123',
          type: 'user_message',
          data: { message: 'Test message' }
        };

        await sessionApi.subscribeToEventStream(sessionId, streamName, onEvent);

        // Simulate receiving an event
        const mockEvent = {
          data: JSON.stringify(testEventData)
        };
        mockEventSource.onmessage(mockEvent);

        expect(onEvent).toHaveBeenCalledTimes(1);
        expect(onEvent).toHaveBeenCalledWith(testEventData);
      })

      it('should handle malformed event data gracefully', async () => {
        const onEvent = vi.fn();
        const consoleSpy = vi.spyOn(console, 'error').mockImplementation(() => {});

        await sessionApi.subscribeToEventStream(sessionId, streamName, onEvent);

        // Simulate receiving malformed JSON
        const mockEvent = {
          data: 'invalid json {'
        };
        mockEventSource.onmessage(mockEvent);

        expect(onEvent).not.toHaveBeenCalled();
        expect(consoleSpy).toHaveBeenCalledWith(
          'Failed to parse stream event:',
          expect.any(Error)
        );

        consoleSpy.mockRestore();
      })

      it('should handle connection errors', async () => {
        const onEvent = vi.fn();
        const consoleSpy = vi.spyOn(console, 'error').mockImplementation(() => {});

        await sessionApi.subscribeToEventStream(sessionId, streamName, onEvent);

        // Simulate connection error
        const mockError = new Error('Connection failed');
        mockEventSource.onerror(mockError);

        expect(consoleSpy).toHaveBeenCalledWith(
          'Event stream error:',
          mockError
        );

        consoleSpy.mockRestore();
      })

      it('should return subscription control object', async () => {
        const onEvent = vi.fn();

        const subscription = await sessionApi.subscribeToEventStream(
          sessionId, 
          streamName, 
          onEvent
        );

        // Test close functionality
        subscription.close();
        expect(mockEventSource.close).toHaveBeenCalledTimes(1);

        // Test readyState functionality
        const state = subscription.readyState();
        expect(state).toBe(1); // OPEN
      })

      it('should include query parameters in subscription URL', async () => {
        const onEvent = vi.fn();
        const options = {
          fromEventId: 'last',
          format: 'json',
          buffer: 'on'
        };

        await sessionApi.subscribeToEventStream(sessionId, streamName, onEvent, options);

        const capturedUrl = global.EventSource.mock.calls[0][0];
        expect(capturedUrl).toContain('fromEventId=last');
        expect(capturedUrl).toContain('format=json');
        expect(capturedUrl).toContain('buffer=on');
      })
    })
  })

  // Test Error Handling
  describe('Error Handling', () => {
    const sessionId = 'test-session-123';
    const streamName = 'chat-events';

    it('should handle network errors across all methods', async () => {
      restoreFetch = setupNetworkErrorMock('Network connection failed');

      // Test all Redis stream methods handle network errors
      await expect(sessionApi.createEventStream(sessionId, streamName))
        .rejects.toThrow('Network connection failed');

      await expect(sessionApi.deleteEventStream(sessionId, streamName))
        .rejects.toThrow('Network connection failed');

      await expect(sessionApi.listEventStreams(sessionId))
        .rejects.toThrow('Network connection failed');

      await expect(sessionApi.publishEvent(sessionId, streamName, { test: 'data' }))
        .rejects.toThrow('Network connection failed');

      await expect(sessionApi.getEventStream(sessionId, streamName))
        .rejects.toThrow('Network connection failed');

      await expect(sessionApi.getEventStreamStats(sessionId, streamName))
        .rejects.toThrow('Network connection failed');

      expect(global.fetch).toHaveBeenCalledTimes(6);
    })

    it('should handle session not found errors', async () => {
      const nonExistentSessionId = 'non-existent-session';

      restoreFetch = setupFetchMock({
        [`/sessions/${nonExistentSessionId}/streams`]: {
          detail: { message: 'Session not found' },
          status: 404
        }
      });

      await expect(sessionApi.listEventStreams(nonExistentSessionId))
        .rejects.toThrow('Session not found');
      expect(global.fetch).toHaveBeenCalledTimes(1);
    })

    it('should handle server errors gracefully', async () => {
      restoreFetch = setupFetchMock({
        [`/sessions/${sessionId}/streams/${streamName}/events`]: {
          detail: { message: 'Internal server error' },
          status: 500
        }
      });

      await expect(sessionApi.publishEvent(sessionId, streamName, { test: 'data' }))
        .rejects.toThrow('Internal server error');
      expect(global.fetch).toHaveBeenCalledTimes(1);
    })

    it('should handle rate limiting errors', async () => {
      restoreFetch = setupFetchMock({
        [`/sessions/${sessionId}/streams/${streamName}/events`]: {
          detail: { 
            message: 'Rate limit exceeded',
            error_code: 'RATE_LIMIT_EXCEEDED',
            retry_after: 60
          },
          status: 429
        }
      });

      await expect(sessionApi.publishEvent(sessionId, streamName, { test: 'data' }))
        .rejects.toThrow('Rate limit exceeded');
      expect(global.fetch).toHaveBeenCalledTimes(1);
    })
  })

  // Test Integration Patterns
  describe('Integration Patterns', () => {
    const sessionId = 'test-session-123';

    it('should use extractResponseData utility correctly', async () => {
      const mockData = {
        success: true,
        data: { streamName: 'test-stream' },
        metadata: { version: '2.0' }
      };

      restoreFetch = setupFetchMock({
        [`/sessions/${sessionId}/streams/test-stream`]: mockData
      });

      const result = await sessionApi.createEventStream(sessionId, 'test-stream');

      // The extractResponseData utility should return just the data portion
      expect(result).toEqual(mockData.data);
      expect(result).not.toHaveProperty('success');
      expect(result).not.toHaveProperty('metadata');
    })

    it('should construct API endpoints correctly', async () => {
      const mockData = { success: true, data: { streams: [] } };
      
      let capturedUrl;
      const originalFetch = global.fetch;
      global.fetch = vi.fn((url, options) => {
        capturedUrl = url;
        return Promise.resolve({
          ok: true,
          status: 200,
          json: () => Promise.resolve(mockData),
          headers: { get: () => 'application/json' }
        });
      });
      
      restoreFetch = () => { global.fetch = originalFetch; };

      await sessionApi.listEventStreams(sessionId);

      expect(capturedUrl).toBe(`/api/v2/sessions/${sessionId}/streams`);
      expect(global.fetch).toHaveBeenCalledTimes(1);
    })

    it('should handle different response formats correctly', async () => {
      // Test 204 No Content response
      const originalFetch = global.fetch;
      global.fetch = vi.fn((url, options) => {
        return Promise.resolve({
          ok: true,
          status: 204,
          json: () => Promise.resolve({}),
          headers: { get: () => 'application/json' }
        });
      });
      
      restoreFetch = () => { global.fetch = originalFetch; };

      const result = await sessionApi.deleteEventStream(sessionId, 'test-stream');

      expect(result).toBe(true);
      expect(global.fetch).toHaveBeenCalledTimes(1);
    })
  })

  // Test Session API Compatibility
  describe('Compatibility with Existing Session Methods', () => {
    const sessionId = 'test-session-123';

    it('should work alongside regular session operations', async () => {
      const sessionData = {
        success: true,
        data: {
          id: sessionId,
          name: 'Test Session',
          status: 'active'
        }
      };

      const streamData = {
        success: true,
        data: { streams: [] }
      };

      restoreFetch = setupFetchMock({
        [`/sessions/${sessionId}`]: sessionData,
        [`/sessions/${sessionId}/streams`]: streamData
      });

      // Test that both regular session API and stream API work together
      const session = await sessionApi.getSession(sessionId);
      const streams = await sessionApi.listEventStreams(sessionId);

      expect(session).toEqual(sessionData.data);
      expect(streams).toEqual([]);
      expect(global.fetch).toHaveBeenCalledTimes(2);
    })

    it('should maintain consistent error handling patterns', async () => {
      // Both regular session methods and stream methods should handle errors the same way
      restoreFetch = setupFetchMock({
        [`/sessions/${sessionId}`]: {
          detail: { message: 'Session not found' },
          status: 404
        },
        [`/sessions/${sessionId}/streams`]: {
          detail: { message: 'Session not found' },
          status: 404
        }
      });

      // Both should throw similar errors
      await expect(sessionApi.getSession(sessionId))
        .rejects.toThrow('Session not found');

      await expect(sessionApi.listEventStreams(sessionId))
        .rejects.toThrow('Session not found');

      expect(global.fetch).toHaveBeenCalledTimes(2);
    })
  })
})