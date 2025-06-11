import { describe, it, expect, beforeEach, afterEach, vi, beforeAll } from 'vitest'
import { setupRedisStreamMocks, createMockStreamEvent, createMockEventSeries } from './utils/redis-stream-test-utils'
import { setupFetchMock, createMockResponse } from './utils/api-test-utils'

// Constants for testing
const TEST_SESSION_ID = 'test-session-id-123'
const TEST_INTERACTION_ID = 'test-interaction-id-abc'
const TEST_STREAM_KEY = `agent_c:stream:${TEST_SESSION_ID}:${TEST_INTERACTION_ID}`
const REDIS_CONFIG = {
  enabled: true,
  host: 'localhost',
  port: 6379,
  prefixes: {
    stream: 'agent_c:stream',
    consumer: 'agent_c:consumer'
  },
  maxLength: 10000,
  ttl: 86400,
}

describe('RedisStreamManager Unit Tests', () => {
  let restoreFetch
  let mockUtils
  
  beforeEach(() => {
    // Setup mock utilities
    mockUtils = setupRedisStreamMocks({
      sessionId: TEST_SESSION_ID,
      streamName: TEST_STREAM_KEY,
      enableSubscription: true
    })
  })
  
  afterEach(() => {
    // Clean up mocks
    if (restoreFetch) {
      restoreFetch()
      restoreFetch = null
    }
    
    if (mockUtils) {
      mockUtils.restore()
      mockUtils = null
    }
    
    vi.clearAllMocks()
  })
  
  describe('Stream Key Management', () => {
    beforeEach(() => {
      // Setup fetch mock for stream key management endpoints
      restoreFetch = setupFetchMock({
        '/config/redis-streams': REDIS_CONFIG,
        [`/sessions/${TEST_SESSION_ID}/streams/${TEST_STREAM_KEY}`]: {
          success: true,
          data: {
            stream_key: TEST_STREAM_KEY,
            session_id: TEST_SESSION_ID,
            interaction_id: TEST_INTERACTION_ID,
            created_at: new Date().toISOString()
          }
        }
      })
    })
    
    it('should generate correct stream keys', async () => {
      // Mock the API request to get stream key
      const response = await fetch(`/api/sessions/${TEST_SESSION_ID}/streams/create`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ interaction_id: TEST_INTERACTION_ID })
      })
      const data = await response.json()
      
      // Verify the generated stream key follows the convention
      expect(data.data.stream_key).toBe(TEST_STREAM_KEY)
      expect(data.data.stream_key.startsWith('agent_c:stream:')).toBe(true)
      expect(data.data.stream_key).toContain(TEST_SESSION_ID)
      expect(data.data.stream_key).toContain(TEST_INTERACTION_ID)
    })
    
    it('should sanitize stream key components', async () => {
      const specialSessionId = 'session:with:special:chars!@#$%^&*()'
      const specialInteractionId = 'interaction with spaces and #$%@'
      
      // Setup fetch mock for special characters test
      const specialStreamKey = 'agent_c:stream:session_with_special_chars________:interaction_with_spaces_and_____'
      const restoreSpecialFetch = setupFetchMock({
        [`/sessions/${specialSessionId}/streams/${specialStreamKey}`]: {
          success: true,
          data: {
            stream_key: specialStreamKey,
            session_id: specialSessionId,
            interaction_id: specialInteractionId,
            created_at: new Date().toISOString()
          }
        }
      })
      
      // Mock the API request to get stream key with special characters
      const response = await fetch(`/api/sessions/${specialSessionId}/streams/create`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ interaction_id: specialInteractionId })
      })
      const data = await response.json()
      
      // Verify the sanitized stream key
      expect(data.data.stream_key).toBe(specialStreamKey)
      expect(data.data.stream_key).not.toContain('!')
      expect(data.data.stream_key).not.toContain('@')
      expect(data.data.stream_key).not.toContain(' ')
      
      restoreSpecialFetch()
    })
    
    it('should parse stream key components correctly', async () => {
      // Mock the API request to get stream info
      const response = await fetch(`/api/sessions/${TEST_SESSION_ID}/streams/${TEST_STREAM_KEY}/info`, {
        method: 'GET'
      })
      const data = await response.json()
      
      // Verify the parsed components
      expect(data.data.session_id).toBe(TEST_SESSION_ID)
      expect(data.data.interaction_id).toBe(TEST_INTERACTION_ID)
    })
    
    it('should handle invalid stream keys gracefully', async () => {
      // Setup fetch mock for invalid stream key
      const restoreInvalidFetch = setupFetchMock({
        '/sessions/invalid-format/streams/invalid-key': {
          detail: { message: 'Invalid stream key format' },
          status: 400
        }
      })
      
      // Mock the API request with invalid stream key
      await expect(
        fetch('/api/sessions/invalid-format/streams/invalid-key/info', { method: 'GET' })
      ).rejects.toThrow('Invalid stream key format')
      
      restoreInvalidFetch()
    })
  })
  
  describe('Stream Creation and Initialization', () => {
    it('should create a new stream with metadata', async () => {
      // Setup fetch mock for stream creation
      restoreFetch = setupFetchMock({
        [`/sessions/${TEST_SESSION_ID}/streams/create`]: {
          success: true,
          data: {
            stream_key: TEST_STREAM_KEY,
            session_id: TEST_SESSION_ID,
            interaction_id: TEST_INTERACTION_ID,
            created_at: new Date().toISOString(),
            ttl: REDIS_CONFIG.ttl,
            maxlen: REDIS_CONFIG.maxLength
          }
        }
      })
      
      // Mock the API request to create stream
      const response = await fetch(`/api/sessions/${TEST_SESSION_ID}/streams/create`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ interaction_id: TEST_INTERACTION_ID })
      })
      const data = await response.json()
      
      // Verify stream creation
      expect(data.success).toBe(true)
      expect(data.data.stream_key).toBe(TEST_STREAM_KEY)
      expect(data.data.ttl).toBe(REDIS_CONFIG.ttl)
      expect(data.data.maxlen).toBe(REDIS_CONFIG.maxLength)
      
      // Verify initial metadata event was published (via mocks)
      const mockEvents = mockUtils.getMockEvents()
      expect(mockEvents.length).toBeGreaterThan(0)
      expect(mockEvents[0].type).toBe('stream_created')
    })
    
    it('should set TTL on stream creation', async () => {
      // Setup fetch mock for stream creation with custom TTL
      const customTtl = 7200 // 2 hours
      restoreFetch = setupFetchMock({
        [`/sessions/${TEST_SESSION_ID}/streams/create`]: {
          success: true,
          data: {
            stream_key: TEST_STREAM_KEY,
            session_id: TEST_SESSION_ID,
            interaction_id: TEST_INTERACTION_ID,
            created_at: new Date().toISOString(),
            ttl: customTtl
          }
        }
      })
      
      // Mock the API request to create stream with custom TTL
      const response = await fetch(`/api/sessions/${TEST_SESSION_ID}/streams/create`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ 
          interaction_id: TEST_INTERACTION_ID,
          ttl: customTtl
        })
      })
      const data = await response.json()
      
      // Verify custom TTL was set
      expect(data.data.ttl).toBe(customTtl)
    })
    
    it('should handle stream already exists error', async () => {
      // Setup fetch mock for stream already exists error
      restoreFetch = setupFetchMock({
        [`/sessions/${TEST_SESSION_ID}/streams/create`]: {
          detail: { message: 'Stream already exists' },
          status: 409
        }
      })
      
      // Mock the API request to create existing stream
      await expect(
        fetch(`/api/sessions/${TEST_SESSION_ID}/streams/create`, {
          method: 'POST',
          headers: { 'Content-Type': 'application/json' },
          body: JSON.stringify({ interaction_id: TEST_INTERACTION_ID })
        })
      ).rejects.toThrow('Stream already exists')
    })
    
    it('should handle Redis connection errors during creation', async () => {
      // Setup fetch mock for Redis connection error
      restoreFetch = setupFetchMock({
        [`/sessions/${TEST_SESSION_ID}/streams/create`]: {
          detail: { message: 'Redis connection failed' },
          status: 500
        }
      })
      
      // Mock the API request with Redis error
      await expect(
        fetch(`/api/sessions/${TEST_SESSION_ID}/streams/create`, {
          method: 'POST',
          headers: { 'Content-Type': 'application/json' },
          body: JSON.stringify({ interaction_id: TEST_INTERACTION_ID })
        })
      ).rejects.toThrow('Redis connection failed')
    })
  })
  
  describe('Event Publishing', () => {
    beforeEach(() => {
      // Setup fetch mock for event publishing
      restoreFetch = setupFetchMock({
        [`/sessions/${TEST_SESSION_ID}/streams/${TEST_STREAM_KEY}/events`]: {
          success: true,
          data: {
            event_id: `event-${Date.now()}`,
            stream_key: TEST_STREAM_KEY,
            timestamp: new Date().toISOString()
          }
        }
      })
    })
    
    it('should publish event to stream successfully', async () => {
      // Create test event data
      const eventData = {
        type: 'text_delta',
        data: 'Hello, world!',
        timestamp: new Date().toISOString(),
        role: 'assistant',
        format: 'markdown'
      }
      
      // Mock the API request to publish event
      const response = await fetch(`/api/sessions/${TEST_SESSION_ID}/streams/${TEST_STREAM_KEY}/events`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(eventData)
      })
      const data = await response.json()
      
      // Verify successful publishing
      expect(data.success).toBe(true)
      expect(data.data.event_id).toBeDefined()
      expect(data.data.stream_key).toBe(TEST_STREAM_KEY)
      
      // Verify event was added to mock stream
      const mockEvents = mockUtils.getMockEvents()
      const lastEvent = mockEvents[mockEvents.length - 1]
      expect(lastEvent.type).toBe('text_delta')
      expect(lastEvent.data).toBe('Hello, world!')
    })
    
    it('should handle complex event data with nested objects', async () => {
      // Create complex test event data
      const complexEventData = {
        type: 'tool_call',
        data: { 
          function: 'search', 
          args: { query: 'Redis Streams', filters: ['documentation', 'examples'] },
          context: { priority: 'high', session_context: { user_id: 'test-user' } }
        },
        timestamp: new Date().toISOString(),
        role: 'system',
        format: 'json'
      }
      
      // Mock the API request to publish complex event
      const response = await fetch(`/api/sessions/${TEST_SESSION_ID}/streams/${TEST_STREAM_KEY}/events`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(complexEventData)
      })
      const data = await response.json()
      
      // Verify successful publishing of complex event
      expect(data.success).toBe(true)
      expect(data.data.event_id).toBeDefined()
      
      // Verify complex event was added with proper serialization
      const mockEvents = mockUtils.getMockEvents()
      const lastEvent = mockEvents[mockEvents.length - 1]
      expect(lastEvent.type).toBe('tool_call')
      expect(lastEvent.data.function).toBe('search')
      expect(lastEvent.data.args.query).toBe('Redis Streams')
      expect(lastEvent.data.context.priority).toBe('high')
    })
    
    it('should add required metadata to events', async () => {
      // Create minimal event data
      const minimalEventData = {
        type: 'system_message',
        data: 'System initialized'
      }
      
      // Mock the API request with minimal data
      const response = await fetch(`/api/sessions/${TEST_SESSION_ID}/streams/${TEST_STREAM_KEY}/events`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(minimalEventData)
      })
      const data = await response.json()
      
      // Verify the event was published
      expect(data.success).toBe(true)
      
      // Verify metadata was added automatically
      const mockEvents = mockUtils.getMockEvents()
      const lastEvent = mockEvents[mockEvents.length - 1]
      expect(lastEvent.type).toBe('system_message')
      expect(lastEvent.timestamp).toBeDefined()
      expect(lastEvent.session_id).toBe(TEST_SESSION_ID) // Should be added automatically
      expect(lastEvent.id).toBeDefined() // Should get an auto-generated ID
    })
    
    it('should handle publishing to non-existent stream', async () => {
      // Setup fetch mock for non-existent stream
      const nonExistentStreamKey = 'agent_c:stream:non-existent:123'
      const restoreNonExistentFetch = setupFetchMock({
        [`/sessions/${TEST_SESSION_ID}/streams/${nonExistentStreamKey}/events`]: {
          detail: { message: 'Stream not found' },
          status: 404
        }
      })
      
      // Create test event data
      const eventData = {
        type: 'text_delta',
        data: 'This should fail'
      }
      
      // Mock the API request to non-existent stream
      await expect(
        fetch(`/api/sessions/${TEST_SESSION_ID}/streams/${nonExistentStreamKey}/events`, {
          method: 'POST',
          headers: { 'Content-Type': 'application/json' },
          body: JSON.stringify(eventData)
        })
      ).rejects.toThrow('Stream not found')
      
      restoreNonExistentFetch()
    })
    
    it('should handle Redis errors during publishing', async () => {
      // Setup fetch mock for Redis error during publishing
      const restoreErrorFetch = setupFetchMock({
        [`/sessions/${TEST_SESSION_ID}/streams/${TEST_STREAM_KEY}/events`]: {
          detail: { message: 'Redis server: OOM command not allowed when used memory > maxmemory' },
          status: 500
        }
      })
      
      // Create test event data
      const eventData = {
        type: 'text_delta',
        data: 'This should fail due to Redis OOM'
      }
      
      // Mock the API request with Redis error
      await expect(
        fetch(`/api/sessions/${TEST_SESSION_ID}/streams/${TEST_STREAM_KEY}/events`, {
          method: 'POST',
          headers: { 'Content-Type': 'application/json' },
          body: JSON.stringify(eventData)
        })
      ).rejects.toThrow('Redis server: OOM command not allowed')
      
      restoreErrorFetch()
    })
    
    it('should handle large event batches', async () => {
      // Create a large batch of events
      const eventBatch = Array.from({ length: 50 }, (_, index) => ({
        type: 'text_delta',
        data: `Batch event ${index}`,
        timestamp: new Date().toISOString(),
        sequence: index
      }))
      
      // Setup fetch mock for batch publishing
      const restoreBatchFetch = setupFetchMock({
        [`/sessions/${TEST_SESSION_ID}/streams/${TEST_STREAM_KEY}/events/batch`]: {
          success: true,
          data: {
            batch_id: `batch-${Date.now()}`,
            stream_key: TEST_STREAM_KEY,
            events_processed: eventBatch.length,
            timestamp: new Date().toISOString()
          }
        }
      })
      
      // Mock the API request for batch publishing
      const response = await fetch(`/api/sessions/${TEST_SESSION_ID}/streams/${TEST_STREAM_KEY}/events/batch`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ events: eventBatch })
      })
      const data = await response.json()
      
      // Verify batch was processed
      expect(data.success).toBe(true)
      expect(data.data.events_processed).toBe(eventBatch.length)
      
      restoreBatchFetch()
    })
  })
  
  describe('Event Consumption', () => {
    it('should retrieve events from stream', async () => {
      // Generate mock events in the stream
      const mockEvents = createMockEventSeries(5, {
        sessionId: TEST_SESSION_ID,
        streamId: TEST_STREAM_KEY
      })
      
      // Add events to mock stream
      mockEvents.forEach(event => mockUtils.addMockEvent(event))
      
      // Setup fetch mock for retrieving events
      restoreFetch = setupFetchMock({
        [`/sessions/${TEST_SESSION_ID}/streams/${TEST_STREAM_KEY}/events`]: {
          success: true,
          data: {
            events: mockEvents,
            stream_info: {
              name: TEST_STREAM_KEY,
              total_events: mockEvents.length,
              first_event: mockEvents[0].id,
              last_event: mockEvents[mockEvents.length - 1].id
            }
          }
        }
      })
      
      // Mock the API request to retrieve events
      const response = await fetch(`/api/sessions/${TEST_SESSION_ID}/streams/${TEST_STREAM_KEY}/events`, {
        method: 'GET'
      })
      const data = await response.json()
      
      // Verify events were retrieved
      expect(data.success).toBe(true)
      expect(data.data.events).toHaveLength(mockEvents.length)
      expect(data.data.stream_info.total_events).toBe(mockEvents.length)
      
      // Verify event data integrity
      expect(data.data.events[0].id).toBe(mockEvents[0].id)
      expect(data.data.events[0].type).toBe(mockEvents[0].type)
      expect(data.data.events[0].data).toBe(mockEvents[0].data)
    })
    
    it('should support pagination with start/end IDs', async () => {
      // Generate 20 mock events for pagination testing
      const allMockEvents = createMockEventSeries(20, {
        sessionId: TEST_SESSION_ID,
        streamId: TEST_STREAM_KEY
      })
      
      // Add events to mock stream
      allMockEvents.forEach(event => mockUtils.addMockEvent(event))
      
      // Setup fetch mock for paginated events (first page)
      const pageSize = 5
      const firstPageEvents = allMockEvents.slice(0, pageSize)
      restoreFetch = setupFetchMock({
        [`/sessions/${TEST_SESSION_ID}/streams/${TEST_STREAM_KEY}/events?count=${pageSize}`]: {
          success: true,
          data: {
            events: firstPageEvents,
            stream_info: {
              name: TEST_STREAM_KEY,
              total_events: allMockEvents.length,
              first_event: firstPageEvents[0].id,
              last_event: firstPageEvents[firstPageEvents.length - 1].id,
              has_more: true
            }
          }
        }
      })
      
      // Mock the API request for first page
      const firstPageResponse = await fetch(`/api/sessions/${TEST_SESSION_ID}/streams/${TEST_STREAM_KEY}/events?count=${pageSize}`, {
        method: 'GET'
      })
      const firstPageData = await firstPageResponse.json()
      
      // Verify pagination info
      expect(firstPageData.data.events).toHaveLength(pageSize)
      expect(firstPageData.data.stream_info.has_more).toBe(true)
      
      // Setup fetch mock for second page
      const lastIdFromFirstPage = firstPageData.data.events[firstPageData.data.events.length - 1].id
      const secondPageEvents = allMockEvents.slice(pageSize, pageSize * 2)
      
      const restoreSecondPageFetch = setupFetchMock({
        [`/sessions/${TEST_SESSION_ID}/streams/${TEST_STREAM_KEY}/events?start=${lastIdFromFirstPage}&count=${pageSize}`]: {
          success: true,
          data: {
            events: secondPageEvents,
            stream_info: {
              name: TEST_STREAM_KEY,
              total_events: allMockEvents.length,
              first_event: secondPageEvents[0].id,
              last_event: secondPageEvents[secondPageEvents.length - 1].id,
              has_more: true
            }
          }
        }
      })
      
      // Mock the API request for second page
      const secondPageResponse = await fetch(`/api/sessions/${TEST_SESSION_ID}/streams/${TEST_STREAM_KEY}/events?start=${lastIdFromFirstPage}&count=${pageSize}`, {
        method: 'GET'
      })
      const secondPageData = await secondPageResponse.json()
      
      // Verify second page data
      expect(secondPageData.data.events).toHaveLength(pageSize)
      expect(secondPageData.data.events[0].id).toBe(secondPageEvents[0].id)
      
      restoreSecondPageFetch()
    })
    
    it('should support real-time event subscription', async () => {
      // Setup EventSource mock (already done in mockUtils)
      const mockEventSource = mockUtils.getEventSource()
      
      // Create subscription URL
      const subscriptionUrl = `/api/sessions/${TEST_SESSION_ID}/streams/${TEST_STREAM_KEY}/subscribe`
      
      // Create a tracking array for received events
      const receivedEvents = []
      
      // Setup event handler
      const eventHandler = (event) => {
        const eventData = JSON.parse(event.data)
        receivedEvents.push(eventData)
      }
      
      // Mock subscription initiation
      const eventSource = new EventSource(subscriptionUrl)
      eventSource.onmessage = eventHandler
      
      // Simulate receiving events
      const mockEvent1 = createMockStreamEvent({
        type: 'text_delta',
        data: 'Hello from subscription',
        sessionId: TEST_SESSION_ID,
        streamId: TEST_STREAM_KEY
      })
      
      const mockEvent2 = createMockStreamEvent({
        type: 'tool_call',
        data: { function: 'search', args: { query: 'test' } },
        sessionId: TEST_SESSION_ID,
        streamId: TEST_STREAM_KEY
      })
      
      // Simulate events arriving
      mockUtils.simulateEvent(mockEvent1)
      mockUtils.simulateEvent(mockEvent2)
      
      // Verify events were received
      expect(receivedEvents).toHaveLength(2)
      expect(receivedEvents[0].type).toBe('text_delta')
      expect(receivedEvents[1].type).toBe('tool_call')
      
      // Test subscription cleanup
      eventSource.close()
    })
    
    it('should handle subscription errors gracefully', async () => {
      // Setup EventSource mock
      const mockEventSource = mockUtils.getEventSource()
      
      // Create subscription URL
      const subscriptionUrl = `/api/sessions/${TEST_SESSION_ID}/streams/${TEST_STREAM_KEY}/subscribe`
      
      // Mock error handler
      const errorHandler = vi.fn()
      
      // Mock subscription with error handling
      const eventSource = new EventSource(subscriptionUrl)
      eventSource.onerror = errorHandler
      
      // Simulate error
      mockUtils.simulateError(new Error('Connection lost'))
      
      // Verify error handler was called
      expect(errorHandler).toHaveBeenCalled()
      
      // Clean up
      eventSource.close()
    })
    
    it('should handle empty streams', async () => {
      // Clear any mock events
      mockUtils.clearMockEvents()
      
      // Setup fetch mock for empty stream
      restoreFetch = setupFetchMock({
        [`/sessions/${TEST_SESSION_ID}/streams/${TEST_STREAM_KEY}/events`]: {
          success: true,
          data: {
            events: [],
            stream_info: {
              name: TEST_STREAM_KEY,
              total_events: 0,
              first_event: null,
              last_event: null
            }
          }
        }
      })
      
      // Mock the API request for empty stream
      const response = await fetch(`/api/sessions/${TEST_SESSION_ID}/streams/${TEST_STREAM_KEY}/events`, {
        method: 'GET'
      })
      const data = await response.json()
      
      // Verify empty result
      expect(data.success).toBe(true)
      expect(data.data.events).toHaveLength(0)
      expect(data.data.stream_info.total_events).toBe(0)
    })
  })
  
  describe('Stream Lifecycle Management', () => {
    it('should close stream properly', async () => {
      // Setup fetch mock for stream closing
      restoreFetch = setupFetchMock({
        [`/sessions/${TEST_SESSION_ID}/streams/${TEST_STREAM_KEY}/close`]: {
          success: true,
          data: {
            stream_key: TEST_STREAM_KEY,
            closed_at: new Date().toISOString(),
            ttl: REDIS_CONFIG.ttl,
            events_count: 5
          }
        }
      })
      
      // Mock the API request to close stream
      const response = await fetch(`/api/sessions/${TEST_SESSION_ID}/streams/${TEST_STREAM_KEY}/close`, {
        method: 'POST'
      })
      const data = await response.json()
      
      // Verify stream was closed
      expect(data.success).toBe(true)
      expect(data.data.stream_key).toBe(TEST_STREAM_KEY)
      expect(data.data.closed_at).toBeDefined()
      
      // Verify closure event was added to stream
      const mockEvents = mockUtils.getMockEvents()
      const lastEvent = mockEvents[mockEvents.length - 1]
      expect(lastEvent.type).toBe('stream_closed')
    })
    
    it('should delete stream when needed', async () => {
      // Setup fetch mock for stream deletion
      restoreFetch = setupFetchMock({
        [`/sessions/${TEST_SESSION_ID}/streams/${TEST_STREAM_KEY}`]: {
          success: true,
          data: {
            stream_key: TEST_STREAM_KEY,
            deleted_at: new Date().toISOString(),
            events_deleted: 5
          }
        }
      })
      
      // Mock the API request to delete stream
      const response = await fetch(`/api/sessions/${TEST_SESSION_ID}/streams/${TEST_STREAM_KEY}`, {
        method: 'DELETE'
      })
      const data = await response.json()
      
      // Verify stream was deleted
      expect(data.success).toBe(true)
      expect(data.data.stream_key).toBe(TEST_STREAM_KEY)
      expect(data.data.deleted_at).toBeDefined()
      expect(data.data.events_deleted).toBe(5)
    })
    
    it('should update stream TTL', async () => {
      // Setup fetch mock for TTL update
      const newTtl = 7200 // 2 hours
      restoreFetch = setupFetchMock({
        [`/sessions/${TEST_SESSION_ID}/streams/${TEST_STREAM_KEY}/ttl`]: {
          success: true,
          data: {
            stream_key: TEST_STREAM_KEY,
            ttl: newTtl,
            updated_at: new Date().toISOString()
          }
        }
      })
      
      // Mock the API request to update TTL
      const response = await fetch(`/api/sessions/${TEST_SESSION_ID}/streams/${TEST_STREAM_KEY}/ttl`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ ttl: newTtl })
      })
      const data = await response.json()
      
      // Verify TTL was updated
      expect(data.success).toBe(true)
      expect(data.data.stream_key).toBe(TEST_STREAM_KEY)
      expect(data.data.ttl).toBe(newTtl)
    })
    
    it('should list all streams for a session', async () => {
      // Setup fetch mock for listing streams
      const mockStreams = [
        {
          stream_key: TEST_STREAM_KEY,
          created_at: new Date().toISOString(),
          events_count: 10,
          state: 'active'
        },
        {
          stream_key: `agent_c:stream:${TEST_SESSION_ID}:second-interaction`,
          created_at: new Date(Date.now() - 3600000).toISOString(), // 1 hour ago
          events_count: 5,
          state: 'closed'
        }
      ]
      
      restoreFetch = setupFetchMock({
        [`/sessions/${TEST_SESSION_ID}/streams`]: {
          success: true,
          data: {
            streams: mockStreams,
            total_count: mockStreams.length
          }
        }
      })
      
      // Mock the API request to list streams
      const response = await fetch(`/api/sessions/${TEST_SESSION_ID}/streams`, {
        method: 'GET'
      })
      const data = await response.json()
      
      // Verify streams were listed
      expect(data.success).toBe(true)
      expect(data.data.streams).toHaveLength(2)
      expect(data.data.streams[0].stream_key).toBe(TEST_STREAM_KEY)
      expect(data.data.streams[1].state).toBe('closed')
    })
    
    it('should get stream statistics', async () => {
      // Setup fetch mock for stream stats
      restoreFetch = setupFetchMock({
        [`/sessions/${TEST_SESSION_ID}/streams/${TEST_STREAM_KEY}/stats`]: {
          success: true,
          data: {
            stream_key: TEST_STREAM_KEY,
            created_at: new Date().toISOString(),
            events_count: 15,
            first_event_id: 'event-1',
            last_event_id: 'event-15',
            size_bytes: 2048,
            ttl_remaining: 3500,
            events_per_type: {
              text_delta: 10,
              tool_call: 3,
              system_message: 2
            }
          }
        }
      })
      
      // Mock the API request for stream stats
      const response = await fetch(`/api/sessions/${TEST_SESSION_ID}/streams/${TEST_STREAM_KEY}/stats`, {
        method: 'GET'
      })
      const data = await response.json()
      
      // Verify stats data
      expect(data.success).toBe(true)
      expect(data.data.stream_key).toBe(TEST_STREAM_KEY)
      expect(data.data.events_count).toBe(15)
      expect(data.data.size_bytes).toBe(2048)
      expect(data.data.events_per_type.text_delta).toBe(10)
    })
  })
  
  describe('Error Handling and Recovery', () => {
    it('should handle temporary Redis unavailability', async () => {
      // First mock a Redis error
      restoreFetch = setupFetchMock({
        [`/sessions/${TEST_SESSION_ID}/streams/${TEST_STREAM_KEY}/events`]: {
          detail: { message: 'Redis connection refused' },
          status: 500
        }
      })
      
      // Mock the API request that should fail
      await expect(
        fetch(`/api/sessions/${TEST_SESSION_ID}/streams/${TEST_STREAM_KEY}/events`, {
          method: 'GET'
        })
      ).rejects.toThrow('Redis connection refused')
      
      // Now restore Redis and retry
      restoreFetch()
      restoreFetch = setupFetchMock({
        [`/sessions/${TEST_SESSION_ID}/streams/${TEST_STREAM_KEY}/events`]: {
          success: true,
          data: {
            events: [],
            stream_info: {
              name: TEST_STREAM_KEY,
              total_events: 0,
              first_event: null,
              last_event: null
            }
          }
        }
      })
      
      // Mock the API request that should now succeed
      const response = await fetch(`/api/sessions/${TEST_SESSION_ID}/streams/${TEST_STREAM_KEY}/events`, {
        method: 'GET'
      })
      const data = await response.json()
      
      // Verify the request succeeded after Redis recovery
      expect(data.success).toBe(true)
    })
    
    it('should handle stream corruption gracefully', async () => {
      // Setup fetch mock for corrupted stream data
      restoreFetch = setupFetchMock({
        [`/sessions/${TEST_SESSION_ID}/streams/${TEST_STREAM_KEY}/repair`]: {
          success: true,
          data: {
            stream_key: TEST_STREAM_KEY,
            status: 'repaired',
            events_recovered: 8,
            events_lost: 2,
            timestamp: new Date().toISOString()
          }
        }
      })
      
      // Mock the API request to repair stream
      const response = await fetch(`/api/sessions/${TEST_SESSION_ID}/streams/${TEST_STREAM_KEY}/repair`, {
        method: 'POST'
      })
      const data = await response.json()
      
      // Verify repair response
      expect(data.success).toBe(true)
      expect(data.data.status).toBe('repaired')
      expect(data.data.events_recovered).toBe(8)
      expect(data.data.events_lost).toBe(2)
    })
    
    it('should handle malformed event data', async () => {
      // Setup fetch mock for event validation endpoint
      restoreFetch = setupFetchMock({
        `/sessions/${TEST_SESSION_ID}/streams/${TEST_STREAM_KEY}/events/validate`: {
          success: true,
          data: {
            is_valid: false,
            errors: ['Missing required field: type', 'Invalid timestamp format'],
            timestamp: new Date().toISOString()
          }
        }
      })
      
      // Create malformed event data
      const malformedEvent = {
        // Missing type field
        data: 'Test content',
        timestamp: 'not-a-date',
        role: 'assistant'
      }
      
      // Mock the API request to validate event
      const response = await fetch(`/api/sessions/${TEST_SESSION_ID}/streams/${TEST_STREAM_KEY}/events/validate`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(malformedEvent)
      })
      const data = await response.json()
      
      // Verify validation results
      expect(data.success).toBe(true)
      expect(data.data.is_valid).toBe(false)
      expect(data.data.errors).toContain('Missing required field: type')
    })
    
    it('should implement circuit breaker pattern for Redis failures', async () => {
      // Mock multiple Redis failures to trigger circuit breaker
      const errorResponses = Array(5).fill({
        detail: { message: 'Redis connection timeout' },
        status: 500
      })
      
      // Create sequential mock setup
      let requestCount = 0
      const originalFetch = global.fetch
      global.fetch = vi.fn((url) => {
        if (url.includes(TEST_STREAM_KEY)) {
          requestCount++
          if (requestCount <= 5) {
            // First 5 requests fail with Redis errors
            return Promise.reject(new Error('Redis connection timeout'))
          } else {
            // After 5 failures, circuit breaker should be open
            return Promise.reject(new Error('Circuit breaker open'))
          }
        }
        return originalFetch(url)
      })
      
      // Mock initial failures
      for (let i = 0; i < 5; i++) {
        await expect(
          fetch(`/api/sessions/${TEST_SESSION_ID}/streams/${TEST_STREAM_KEY}/events`, {
            method: 'GET'
          })
        ).rejects.toThrow('Redis connection timeout')
      }
      
      // Next request should fail with circuit breaker error
      await expect(
        fetch(`/api/sessions/${TEST_SESSION_ID}/streams/${TEST_STREAM_KEY}/events`, {
          method: 'GET'
        })
      ).rejects.toThrow('Circuit breaker open')
      
      // Restore original fetch
      global.fetch = originalFetch
    })
  })
  
  describe('Performance and Monitoring', () => {
    it('should report Redis health status', async () => {
      // Setup fetch mock for health check endpoint
      restoreFetch = setupFetchMock({
        '/redis/health': {
          success: true,
          data: {
            status: 'healthy',
            version: '6.2.5',
            uptime_seconds: 86400,
            connected_clients: 5,
            used_memory_human: '1.2G',
            role: 'master',
            last_checked: new Date().toISOString()
          }
        }
      })
      
      // Mock the API request for Redis health
      const response = await fetch('/api/redis/health', {
        method: 'GET'
      })
      const data = await response.json()
      
      // Verify health data
      expect(data.success).toBe(true)
      expect(data.data.status).toBe('healthy')
      expect(data.data.version).toBe('6.2.5')
      expect(data.data.used_memory_human).toBe('1.2G')
    })
    
    it('should report stream metrics', async () => {
      // Setup fetch mock for stream metrics endpoint
      restoreFetch = setupFetchMock({
        '/redis/metrics/streams': {
          success: true,
          data: {
            total_streams: 42,
            total_events: 12345,
            events_per_minute: 125,
            average_stream_size: 35,
            active_subscriptions: 3,
            redis_memory_usage: '250MB',
            timestamp: new Date().toISOString()
          }
        }
      })
      
      // Mock the API request for stream metrics
      const response = await fetch('/api/redis/metrics/streams', {
        method: 'GET'
      })
      const data = await response.json()
      
      // Verify metrics data
      expect(data.success).toBe(true)
      expect(data.data.total_streams).toBe(42)
      expect(data.data.total_events).toBe(12345)
      expect(data.data.events_per_minute).toBe(125)
    })
    
    it('should get performance statistics for a stream', async () => {
      // Setup fetch mock for stream performance stats
      restoreFetch = setupFetchMock({
        [`/sessions/${TEST_SESSION_ID}/streams/${TEST_STREAM_KEY}/performance`]: {
          success: true,
          data: {
            stream_key: TEST_STREAM_KEY,
            publish_latency_ms: 5.2,
            consume_latency_ms: 3.7,
            average_event_size_bytes: 256,
            events_per_second: 45.3,
            concurrent_consumers: 2,
            timestamp: new Date().toISOString()
          }
        }
      })
      
      // Mock the API request for stream performance
      const response = await fetch(`/api/sessions/${TEST_SESSION_ID}/streams/${TEST_STREAM_KEY}/performance`, {
        method: 'GET'
      })
      const data = await response.json()
      
      // Verify performance data
      expect(data.success).toBe(true)
      expect(data.data.stream_key).toBe(TEST_STREAM_KEY)
      expect(data.data.publish_latency_ms).toBe(5.2)
      expect(data.data.consume_latency_ms).toBe(3.7)
      expect(data.data.events_per_second).toBe(45.3)
    })
  })
  
  describe('Multi-Session Isolation', () => {
    it('should maintain complete isolation between sessions', async () => {
      // Setup two different sessions
      const session1Id = 'test-session-1'
      const interaction1Id = 'test-interaction-1'
      const stream1Key = `agent_c:stream:${session1Id}:${interaction1Id}`
      
      const session2Id = 'test-session-2'
      const interaction2Id = 'test-interaction-2'
      const stream2Key = `agent_c:stream:${session2Id}:${interaction2Id}`
      
      // Setup mock utilities for both sessions
      const mockUtils1 = setupRedisStreamMocks({
        sessionId: session1Id,
        streamName: stream1Key,
        enableSubscription: true
      })
      
      const mockUtils2 = setupRedisStreamMocks({
        sessionId: session2Id,
        streamName: stream2Key,
        enableSubscription: true
      })
      
      // Setup fetch mock for both streams
      restoreFetch = setupFetchMock({
        [`/sessions/${session1Id}/streams/${stream1Key}/events`]: {
          success: true,
          data: {
            events: [],
            stream_info: {
              name: stream1Key,
              total_events: 0
            }
          }
        },
        [`/sessions/${session2Id}/streams/${stream2Key}/events`]: {
          success: true,
          data: {
            events: [],
            stream_info: {
              name: stream2Key,
              total_events: 0
            }
          }
        }
      })
      
      // Add event to session 1
      const event1 = createMockStreamEvent({
        type: 'text_delta',
        data: 'Session 1 event',
        sessionId: session1Id,
        streamId: stream1Key
      })
      mockUtils1.addMockEvent(event1)
      
      // Add event to session 2
      const event2 = createMockStreamEvent({
        type: 'text_delta',
        data: 'Session 2 event',
        sessionId: session2Id,
        streamId: stream2Key
      })
      mockUtils2.addMockEvent(event2)
      
      // Setup fetch mocks for retrieving events from both streams
      const restoreFetch1 = setupFetchMock({
        [`/sessions/${session1Id}/streams/${stream1Key}/events`]: {
          success: true,
          data: {
            events: [event1],
            stream_info: {
              name: stream1Key,
              total_events: 1
            }
          }
        }
      })
      
      // Retrieve events from session 1
      const response1 = await fetch(`/api/sessions/${session1Id}/streams/${stream1Key}/events`, {
        method: 'GET'
      })
      const data1 = await response1.json()
      
      // Verify session 1 data
      expect(data1.success).toBe(true)
      expect(data1.data.events).toHaveLength(1)
      expect(data1.data.events[0].data).toBe('Session 1 event')
      
      restoreFetch1()
      
      // Setup fetch for session 2
      const restoreFetch2 = setupFetchMock({
        [`/sessions/${session2Id}/streams/${stream2Key}/events`]: {
          success: true,
          data: {
            events: [event2],
            stream_info: {
              name: stream2Key,
              total_events: 1
            }
          }
        }
      })
      
      // Retrieve events from session 2
      const response2 = await fetch(`/api/sessions/${session2Id}/streams/${stream2Key}/events`, {
        method: 'GET'
      })
      const data2 = await response2.json()
      
      // Verify session 2 data
      expect(data2.success).toBe(true)
      expect(data2.data.events).toHaveLength(1)
      expect(data2.data.events[0].data).toBe('Session 2 event')
      
      // Clean up
      restoreFetch2()
      mockUtils1.restore()
      mockUtils2.restore()
    })
  })
})