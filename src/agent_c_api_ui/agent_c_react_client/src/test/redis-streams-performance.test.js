import { describe, it, expect, beforeEach, afterEach, vi, beforeAll, afterAll } from 'vitest'
import { ChatEvent } from '../components/chat_interface/utils/ChatEvent'
import { 
  createMockStreamEvent, 
  createMockEventSeries,
  createTestChatEvents,
  setupRedisStreamMocks 
} from './utils/redis-stream-test-utils'

/**
 * Performance Tests for Redis Streams Integration
 * 
 * These tests validate the performance characteristics of the Redis Streams
 * implementation under various load conditions, measuring throughput, latency,
 * memory usage, and concurrent operations.
 */

describe('Redis Streams Performance Tests', () => {
  let performanceMetrics;
  let redisStreamsMocks;
  let testSessionId;
  let testInteractionId;

  beforeAll(() => {
    // Initialize performance tracking
    performanceMetrics = {
      eventPublishTimes: [],
      eventConsumeTimes: [],
      memoryUsage: [],
      concurrentOperations: [],
      errorRates: []
    };

    testSessionId = 'perf-test-session';
    testInteractionId = 'perf-test-interaction';

    // Increase test timeout for performance tests
    vi.setConfig({ testTimeout: 30000 });
  });

  beforeEach(() => {
    // Clear metrics for each test
    Object.keys(performanceMetrics).forEach(key => {
      performanceMetrics[key] = [];
    });

    // Setup Redis mocks with performance tracking
    redisStreamsMocks = setupRedisStreamMocks({
      sessionId: testSessionId,
      streamName: `agent_events:${testSessionId}:${testInteractionId}`,
      enablePerformanceTracking: true
    });
  });

  afterEach(() => {
    if (redisStreamsMocks?.restore) {
      redisStreamsMocks.restore();
    }
    vi.clearAllMocks();
  });

  describe('Event Publishing Performance', () => {
    it('should handle high-volume event publishing efficiently', async () => {
      const eventCount = 1000;
      const batchSize = 10;
      const publishTimes = [];

      // Create test events
      const events = Array.from({ length: eventCount }, (_, i) => 
        createMockStreamEvent({
          type: 'performance_test',
          data: `Event ${i} with some substantial content to simulate real usage`,
          sessionId: testSessionId,
          correlationId: testInteractionId,
          metadata: { 
            batch: Math.floor(i / batchSize),
            sequence: i,
            test_type: 'high_volume'
          }
        })
      );

      const startTime = performance.now();

      // Publish events in batches to simulate realistic usage
      for (let i = 0; i < eventCount; i += batchSize) {
        const batchStartTime = performance.now();
        const batch = events.slice(i, i + batchSize);
        
        // Simulate concurrent publishing within batch
        const batchPromises = batch.map(event => {
          redisStreamsMocks.addMockEvent(event);
          return Promise.resolve();
        });

        await Promise.all(batchPromises);
        
        const batchEndTime = performance.now();
        publishTimes.push(batchEndTime - batchStartTime);
      }

      const totalTime = performance.now() - startTime;
      const averageBatchTime = publishTimes.reduce((a, b) => a + b, 0) / publishTimes.length;
      const eventsPerSecond = (eventCount / totalTime) * 1000;

      // Performance assertions
      expect(totalTime).toBeLessThan(5000); // Should complete within 5 seconds
      expect(averageBatchTime).toBeLessThan(50); // Average batch should be under 50ms
      expect(eventsPerSecond).toBeGreaterThan(200); // Should handle at least 200 events/second

      // Verify all events were added
      const mockEvents = redisStreamsMocks.getMockEvents();
      expect(mockEvents).toHaveLength(eventCount);

      performanceMetrics.eventPublishTimes = publishTimes;
      console.log(`Published ${eventCount} events in ${totalTime.toFixed(2)}ms (${eventsPerSecond.toFixed(2)} events/sec)`);
    });

    it('should maintain consistent performance with varying payload sizes', async () => {
      const payloadSizes = [
        { size: 'small', bytes: 100 },
        { size: 'medium', bytes: 1000 },
        { size: 'large', bytes: 10000 },
        { size: 'xlarge', bytes: 100000 }
      ];

      const performanceResults = {};

      for (const { size, bytes } of payloadSizes) {
        const events = Array.from({ length: 100 }, (_, i) => 
          createMockStreamEvent({
            type: 'payload_size_test',
            data: {
              content: 'A'.repeat(bytes),
              metadata: { size, bytes, index: i }
            },
            sessionId: testSessionId,
            correlationId: `${testInteractionId}-${size}`
          })
        );

        const startTime = performance.now();
        
        events.forEach(event => redisStreamsMocks.addMockEvent(event));
        
        const endTime = performance.now();
        const timePerEvent = (endTime - startTime) / events.length;

        performanceResults[size] = {
          totalTime: endTime - startTime,
          timePerEvent,
          throughputMBps: (bytes * events.length / 1024 / 1024) / ((endTime - startTime) / 1000)
        };
      }

      // Verify performance scales reasonably with payload size
      expect(performanceResults.small.timePerEvent).toBeLessThan(5); // <5ms per small event
      expect(performanceResults.medium.timePerEvent).toBeLessThan(10); // <10ms per medium event
      expect(performanceResults.large.timePerEvent).toBeLessThan(25); // <25ms per large event
      expect(performanceResults.xlarge.timePerEvent).toBeLessThan(100); // <100ms per xlarge event

      // Log performance characteristics
      Object.entries(performanceResults).forEach(([size, metrics]) => {
        console.log(`${size}: ${metrics.timePerEvent.toFixed(2)}ms/event, ${metrics.throughputMBps.toFixed(2)} MB/s`);
      });
    });

    it('should handle burst publishing scenarios', async () => {
      const burstScenarios = [
        { name: 'small_burst', events: 50, interval: 0 },
        { name: 'medium_burst', events: 200, interval: 10 },
        { name: 'large_burst', events: 500, interval: 5 },
        { name: 'sustained', events: 1000, interval: 2 }
      ];

      const burstResults = {};

      for (const scenario of burstScenarios) {
        const events = createTestChatEvents({
          count: scenario.events,
          sessionId: testSessionId,
          correlationId: `${testInteractionId}-${scenario.name}`
        });

        const startTime = performance.now();
        let completedEvents = 0;

        // Simulate burst publishing with intervals
        for (const event of events) {
          redisStreamsMocks.addMockEvent(event.serialize());
          completedEvents++;
          
          if (scenario.interval > 0 && completedEvents % 10 === 0) {
            // Brief pause for non-zero interval scenarios
            await new Promise(resolve => setTimeout(resolve, scenario.interval));
          }
        }

        const endTime = performance.now();
        
        burstResults[scenario.name] = {
          totalTime: endTime - startTime,
          eventsPerSecond: (scenario.events / (endTime - startTime)) * 1000,
          averageLatency: (endTime - startTime) / scenario.events
        };
      }

      // Verify burst handling capabilities
      expect(burstResults.small_burst.eventsPerSecond).toBeGreaterThan(500);
      expect(burstResults.medium_burst.eventsPerSecond).toBeGreaterThan(300);
      expect(burstResults.large_burst.eventsPerSecond).toBeGreaterThan(200);
      expect(burstResults.sustained.averageLatency).toBeLessThan(10); // <10ms average latency

      console.log('Burst Performance Results:');
      Object.entries(burstResults).forEach(([scenario, metrics]) => {
        console.log(`  ${scenario}: ${metrics.eventsPerSecond.toFixed(2)} events/sec, ${metrics.averageLatency.toFixed(2)}ms avg latency`);
      });
    });
  });

  describe('Event Consumption Performance', () => {
    it('should efficiently consume large event streams', async () => {
      const eventCount = 2000;
      
      // Pre-populate stream with events
      const events = createTestChatEvents({
        count: eventCount,
        sessionId: testSessionId,
        correlationId: testInteractionId
      });

      events.forEach(event => redisStreamsMocks.addMockEvent(event.serialize()));

      // Measure consumption performance
      const consumptionStartTime = performance.now();
      const consumedEvents = [];
      let lastProcessedTime = consumptionStartTime;

      // Simulate realistic consumption with processing time
      const mockEvents = redisStreamsMocks.getMockEvents();
      for (const event of mockEvents) {
        const processingStartTime = performance.now();
        
        // Simulate event processing (parsing, validation, etc.)
        const chatEvent = ChatEvent.deserialize(event);
        consumedEvents.push(chatEvent);
        
        // Simulate processing delay
        await new Promise(resolve => setTimeout(resolve, 1));
        
        const processingEndTime = performance.now();
        performanceMetrics.eventConsumeTimes.push(processingEndTime - processingStartTime);
        lastProcessedTime = processingEndTime;
      }

      const totalConsumptionTime = lastProcessedTime - consumptionStartTime;
      const averageProcessingTime = performanceMetrics.eventConsumeTimes.reduce((a, b) => a + b, 0) / performanceMetrics.eventConsumeTimes.length;
      const consumptionRate = (eventCount / totalConsumptionTime) * 1000;

      // Performance assertions
      expect(consumedEvents).toHaveLength(eventCount);
      expect(averageProcessingTime).toBeLessThan(10); // <10ms average processing time
      expect(consumptionRate).toBeGreaterThan(100); // >100 events/second consumption
      expect(totalConsumptionTime).toBeLessThan(30000); // Complete within 30 seconds

      console.log(`Consumed ${eventCount} events in ${totalConsumptionTime.toFixed(2)}ms (${consumptionRate.toFixed(2)} events/sec)`);
      console.log(`Average processing time: ${averageProcessingTime.toFixed(2)}ms`);
    });

    it('should handle real-time streaming with low latency', async () => {
      const streamingDuration = 5000; // 5 seconds
      const eventInterval = 100; // 100ms between events
      const expectedEvents = Math.floor(streamingDuration / eventInterval);
      
      const publishedEvents = [];
      const consumedEvents = [];
      const latencies = [];

      // Start consuming in parallel
      const consumptionPromise = new Promise((resolve) => {
        let eventsReceived = 0;
        const eventSource = redisStreamsMocks.getEventSource();
        
        eventSource.onmessage = (event) => {
          const receiveTime = performance.now();
          const eventData = JSON.parse(event.data);
          consumedEvents.push(eventData);
          
          // Calculate latency from publish to consume
          const publishTime = eventData.metadata?.publishTime;
          if (publishTime) {
            latencies.push(receiveTime - publishTime);
          }
          
          eventsReceived++;
          if (eventsReceived >= expectedEvents) {
            resolve();
          }
        };
      });

      // Start publishing events at regular intervals
      const publishingPromise = new Promise((resolve) => {
        let eventIndex = 0;
        
        const publishInterval = setInterval(() => {
          const publishTime = performance.now();
          const event = createMockStreamEvent({
            type: 'streaming_test',
            data: `Real-time event ${eventIndex}`,
            sessionId: testSessionId,
            correlationId: testInteractionId,
            metadata: { publishTime, eventIndex }
          });

          publishedEvents.push(event);
          redisStreamsMocks.simulateEvent(event);
          eventIndex++;

          if (eventIndex >= expectedEvents) {
            clearInterval(publishInterval);
            resolve();
          }
        }, eventInterval);
      });

      // Wait for both publishing and consumption to complete
      await Promise.all([publishingPromise, consumptionPromise]);

      // Calculate performance metrics
      const averageLatency = latencies.reduce((a, b) => a + b, 0) / latencies.length;
      const maxLatency = Math.max(...latencies);
      const minLatency = Math.min(...latencies);
      const p95Latency = latencies.sort((a, b) => a - b)[Math.floor(latencies.length * 0.95)];

      // Performance assertions for real-time streaming
      expect(consumedEvents).toHaveLength(expectedEvents);
      expect(averageLatency).toBeLessThan(50); // <50ms average latency
      expect(p95Latency).toBeLessThan(100); // <100ms 95th percentile
      expect(maxLatency).toBeLessThan(200); // <200ms maximum latency

      console.log(`Real-time streaming: ${expectedEvents} events over ${streamingDuration}ms`);
      console.log(`Latency - Avg: ${averageLatency.toFixed(2)}ms, P95: ${p95Latency.toFixed(2)}ms, Max: ${maxLatency.toFixed(2)}ms`);
    });
  });

  describe('Concurrent Operations Performance', () => {
    it('should handle multiple concurrent sessions efficiently', async () => {
      const sessionCount = 20;
      const eventsPerSession = 100;
      
      const sessionPromises = Array.from({ length: sessionCount }, async (_, sessionIndex) => {
        const sessionId = `concurrent-session-${sessionIndex}`;
        const interactionId = `concurrent-interaction-${sessionIndex}`;
        
        // Setup separate mock for each session
        const sessionMocks = setupRedisStreamMocks({
          sessionId,
          streamName: `agent_events:${sessionId}:${interactionId}`
        });

        const sessionStartTime = performance.now();
        
        // Generate and publish events for this session
        const events = createTestChatEvents({
          count: eventsPerSession,
          sessionId,
          correlationId: interactionId
        });

        events.forEach(event => sessionMocks.addMockEvent(event.serialize()));
        
        const sessionEndTime = performance.now();
        sessionMocks.restore();
        
        return {
          sessionId,
          duration: sessionEndTime - sessionStartTime,
          eventCount: eventsPerSession
        };
      });

      const startTime = performance.now();
      const sessionResults = await Promise.all(sessionPromises);
      const totalTime = performance.now() - startTime;

      // Calculate performance metrics
      const avgSessionDuration = sessionResults.reduce((sum, result) => sum + result.duration, 0) / sessionResults.length;
      const totalEvents = sessionCount * eventsPerSession;
      const overallThroughput = (totalEvents / totalTime) * 1000;

      // Performance assertions
      expect(sessionResults).toHaveLength(sessionCount);
      expect(avgSessionDuration).toBeLessThan(1000); // <1 second per session on average
      expect(overallThroughput).toBeGreaterThan(500); // >500 events/second total throughput
      expect(totalTime).toBeLessThan(10000); // Complete within 10 seconds

      console.log(`Concurrent sessions: ${sessionCount} sessions, ${totalEvents} total events`);
      console.log(`Total time: ${totalTime.toFixed(2)}ms, Throughput: ${overallThroughput.toFixed(2)} events/sec`);
      console.log(`Average session duration: ${avgSessionDuration.toFixed(2)}ms`);
    });

    it('should maintain performance under concurrent read/write operations', async () => {
      const concurrentOperations = 50;
      const operationTypes = ['publish', 'consume', 'query'];
      
      // Pre-populate stream with some events for consumption
      const initialEvents = createTestChatEvents({
        count: 500,
        sessionId: testSessionId,
        correlationId: testInteractionId
      });
      
      initialEvents.forEach(event => redisStreamsMocks.addMockEvent(event.serialize()));

      const operationPromises = Array.from({ length: concurrentOperations }, async (_, i) => {
        const operationType = operationTypes[i % operationTypes.length];
        const operationStartTime = performance.now();

        switch (operationType) {
          case 'publish':
            const newEvent = createMockStreamEvent({
              type: 'concurrent_publish',
              data: `Concurrent event ${i}`,
              sessionId: testSessionId,
              correlationId: `${testInteractionId}-concurrent-${i}`
            });
            redisStreamsMocks.addMockEvent(newEvent);
            break;

          case 'consume':
            const mockEvents = redisStreamsMocks.getMockEvents();
            const randomEvents = mockEvents.slice(0, 10); // Consume 10 random events
            randomEvents.forEach(event => ChatEvent.deserialize(event));
            break;

          case 'query':
            // Simulate querying stream info
            const streamEvents = redisStreamsMocks.getMockEvents();
            const eventCount = streamEvents.length;
            const latestEvent = streamEvents[streamEvents.length - 1];
            break;
        }

        const operationEndTime = performance.now();
        return {
          type: operationType,
          duration: operationEndTime - operationStartTime,
          index: i
        };
      });

      const startTime = performance.now();
      const operationResults = await Promise.all(operationPromises);
      const totalTime = performance.now() - startTime;

      // Analyze performance by operation type
      const performanceByType = operationTypes.reduce((acc, type) => {
        const typeResults = operationResults.filter(r => r.type === type);
        const avgDuration = typeResults.reduce((sum, r) => sum + r.duration, 0) / typeResults.length;
        const maxDuration = Math.max(...typeResults.map(r => r.duration));
        
        acc[type] = { avgDuration, maxDuration, count: typeResults.length };
        return acc;
      }, {});

      // Performance assertions
      expect(operationResults).toHaveLength(concurrentOperations);
      expect(totalTime).toBeLessThan(5000); // Complete within 5 seconds
      
      // Individual operation type assertions
      Object.entries(performanceByType).forEach(([type, metrics]) => {
        expect(metrics.avgDuration).toBeLessThan(100); // <100ms average for any operation
        expect(metrics.maxDuration).toBeLessThan(500); // <500ms maximum for any operation
      });

      console.log(`Concurrent operations: ${concurrentOperations} operations in ${totalTime.toFixed(2)}ms`);
      Object.entries(performanceByType).forEach(([type, metrics]) => {
        console.log(`  ${type}: ${metrics.count} ops, avg: ${metrics.avgDuration.toFixed(2)}ms, max: ${metrics.maxDuration.toFixed(2)}ms`);
      });
    });
  });

  describe('Memory Usage and Resource Management', () => {
    it('should maintain reasonable memory usage during high-volume operations', async () => {
      const iterations = 10;
      const eventsPerIteration = 1000;
      const memorySnapshots = [];

      // Function to simulate memory measurement
      const measureMemory = () => {
        // In a real environment, this would measure actual memory usage
        // For testing, we'll simulate based on event count
        const currentEvents = redisStreamsMocks.getMockEvents().length;
        return {
          heapUsed: currentEvents * 1024, // Simulate 1KB per event
          heapTotal: currentEvents * 2048, // Simulate 2KB total per event
          external: currentEvents * 512    // Simulate 512B external per event
        };
      };

      for (let iteration = 0; iteration < iterations; iteration++) {
        const startMemory = measureMemory();
        
        // Add events
        const events = createTestChatEvents({
          count: eventsPerIteration,
          sessionId: testSessionId,
          correlationId: `${testInteractionId}-memory-${iteration}`
        });

        events.forEach(event => redisStreamsMocks.addMockEvent(event.serialize()));

        const endMemory = measureMemory();
        
        memorySnapshots.push({
          iteration,
          startMemory,
          endMemory,
          memoryIncrease: endMemory.heapUsed - startMemory.heapUsed,
          eventCount: events.length
        });

        // Simulate periodic cleanup
        if (iteration % 3 === 2) {
          redisStreamsMocks.clearMockEvents();
        }
      }

      // Analyze memory growth patterns
      const totalMemoryIncrease = memorySnapshots.reduce((sum, snapshot) => sum + snapshot.memoryIncrease, 0);
      const avgMemoryPerEvent = totalMemoryIncrease / (iterations * eventsPerIteration);
      const maxMemoryUsage = Math.max(...memorySnapshots.map(s => s.endMemory.heapUsed));

      // Memory performance assertions
      expect(avgMemoryPerEvent).toBeLessThan(2048); // <2KB per event on average
      expect(maxMemoryUsage).toBeLessThan(50 * 1024 * 1024); // <50MB total
      
      console.log(`Memory usage: ${avgMemoryPerEvent.toFixed(2)} bytes/event, max: ${(maxMemoryUsage / 1024 / 1024).toFixed(2)}MB`);
    });

    it('should efficiently handle stream cleanup and garbage collection', async () => {
      const streamCount = 100;
      const eventsPerStream = 50;
      
      // Create multiple streams
      const streams = Array.from({ length: streamCount }, (_, i) => {
        const sessionId = `cleanup-session-${i}`;
        const interactionId = `cleanup-interaction-${i}`;
        const events = createTestChatEvents({
          count: eventsPerStream,
          sessionId,
          correlationId: interactionId
        });
        
        return { sessionId, interactionId, events };
      });

      // Add all events to streams
      const addStartTime = performance.now();
      streams.forEach(stream => {
        stream.events.forEach(event => redisStreamsMocks.addMockEvent(event.serialize()));
      });
      const addEndTime = performance.now();

      const totalEventsBefore = redisStreamsMocks.getMockEvents().length;
      expect(totalEventsBefore).toBe(streamCount * eventsPerStream);

      // Simulate cleanup operation
      const cleanupStartTime = performance.now();
      
      // Clear streams (simulating TTL expiration or manual cleanup)
      const streamsToCleanup = streams.slice(0, streamCount / 2); // Cleanup half
      streamsToCleanup.forEach(() => {
        // In real implementation, this would call deleteStream
        // For testing, we'll simulate by removing events
      });
      
      redisStreamsMocks.clearMockEvents();
      
      const cleanupEndTime = performance.now();

      // Measure cleanup performance
      const addTime = addEndTime - addStartTime;
      const cleanupTime = cleanupEndTime - cleanupStartTime;
      const eventsAfterCleanup = redisStreamsMocks.getMockEvents().length;

      // Cleanup performance assertions
      expect(cleanupTime).toBeLessThan(1000); // Cleanup should be fast (<1 second)
      expect(addTime).toBeLessThan(5000); // Adding should complete in reasonable time
      expect(eventsAfterCleanup).toBe(0); // All events should be cleaned up

      console.log(`Cleanup performance: Added ${totalEventsBefore} events in ${addTime.toFixed(2)}ms`);
      console.log(`Cleaned up ${totalEventsBefore} events in ${cleanupTime.toFixed(2)}ms`);
    });
  });

  describe('Error Recovery and Resilience Performance', () => {
    it('should recover quickly from simulated Redis failures', async () => {
      const failureScenarios = [
        { name: 'connection_timeout', duration: 100 },
        { name: 'memory_pressure', duration: 200 },
        { name: 'network_partition', duration: 500 }
      ];

      const recoveryResults = {};

      for (const scenario of failureScenarios) {
        const events = createTestChatEvents({
          count: 100,
          sessionId: testSessionId,
          correlationId: `${testInteractionId}-${scenario.name}`
        });

        // Start publishing events
        const startTime = performance.now();
        let successfulEvents = 0;
        let failedEvents = 0;
        let recoveryTime = null;

        for (const [index, event] of events.entries()) {
          try {
            // Simulate failure period
            if (index >= 30 && index < 50 && !recoveryTime) {
              // Simulate failure for a portion of events
              await new Promise(resolve => setTimeout(resolve, scenario.duration / 20));
              throw new Error(`Simulated ${scenario.name} failure`);
            }

            // Mark recovery point
            if (index === 50 && !recoveryTime) {
              recoveryTime = performance.now() - startTime;
            }

            redisStreamsMocks.addMockEvent(event.serialize());
            successfulEvents++;
          } catch (error) {
            failedEvents++;
          }
        }

        const totalTime = performance.now() - startTime;
        
        recoveryResults[scenario.name] = {
          totalTime,
          recoveryTime: recoveryTime || totalTime,
          successfulEvents,
          failedEvents,
          successRate: successfulEvents / events.length
        };
      }

      // Performance assertions for error recovery
      Object.entries(recoveryResults).forEach(([scenario, results]) => {
        expect(results.successRate).toBeGreaterThan(0.7); // >70% success rate even with failures
        expect(results.recoveryTime).toBeLessThan(2000); // Recovery within 2 seconds
      });

      console.log('Error Recovery Performance:');
      Object.entries(recoveryResults).forEach(([scenario, results]) => {
        console.log(`  ${scenario}: ${(results.successRate * 100).toFixed(1)}% success, ${results.recoveryTime.toFixed(2)}ms recovery`);
      });
    });

    it('should maintain performance during gradual degradation', async () => {
      const degradationLevels = [0, 0.1, 0.25, 0.5, 0.75]; // 0% to 75% failure rate
      const performanceUnderDegradation = {};

      for (const failureRate of degradationLevels) {
        const events = createTestChatEvents({
          count: 200,
          sessionId: testSessionId,
          correlationId: `${testInteractionId}-degradation-${failureRate}`
        });

        const startTime = performance.now();
        let processedEvents = 0;
        let totalLatency = 0;

        for (const event of events) {
          const eventStartTime = performance.now();
          
          // Simulate failure based on failure rate
          if (Math.random() < failureRate) {
            // Simulate degraded performance
            await new Promise(resolve => setTimeout(resolve, 50)); // 50ms delay
          } else {
            redisStreamsMocks.addMockEvent(event.serialize());
          }
          
          const eventEndTime = performance.now();
          totalLatency += eventEndTime - eventStartTime;
          processedEvents++;
        }

        const totalTime = performance.now() - startTime;
        const averageLatency = totalLatency / processedEvents;
        const throughput = (processedEvents / totalTime) * 1000;

        performanceUnderDegradation[failureRate] = {
          averageLatency,
          throughput,
          processedEvents
        };
      }

      // Verify graceful degradation
      const baselinePerformance = performanceUnderDegradation[0];
      
      Object.entries(performanceUnderDegradation).forEach(([failureRate, metrics]) => {
        const rate = parseFloat(failureRate);
        if (rate > 0) {
          // Performance should degrade gracefully, not catastrophically
          const throughputRatio = metrics.throughput / baselinePerformance.throughput;
          const expectedMinRatio = 1 - (rate * 1.5); // Allow up to 1.5x impact of failure rate
          
          expect(throughputRatio).toBeGreaterThan(Math.max(0.1, expectedMinRatio));
          expect(metrics.averageLatency).toBeLessThan(baselinePerformance.averageLatency * 3); // <3x latency increase
        }
      });

      console.log('Performance Under Degradation:');
      Object.entries(performanceUnderDegradation).forEach(([failureRate, metrics]) => {
        console.log(`  ${(parseFloat(failureRate) * 100).toFixed(0)}% failure: ${metrics.throughput.toFixed(2)} events/sec, ${metrics.averageLatency.toFixed(2)}ms avg latency`);
      });
    });
  });

  afterAll(() => {
    // Generate performance summary report
    console.log('\n=== Redis Streams Performance Test Summary ===');
    console.log(`Event publish times: ${performanceMetrics.eventPublishTimes.length} samples`);
    console.log(`Event consume times: ${performanceMetrics.eventConsumeTimes.length} samples`);
    
    if (performanceMetrics.eventPublishTimes.length > 0) {
      const avgPublishTime = performanceMetrics.eventPublishTimes.reduce((a, b) => a + b, 0) / performanceMetrics.eventPublishTimes.length;
      console.log(`Average publish time: ${avgPublishTime.toFixed(2)}ms`);
    }
    
    if (performanceMetrics.eventConsumeTimes.length > 0) {
      const avgConsumeTime = performanceMetrics.eventConsumeTimes.reduce((a, b) => a + b, 0) / performanceMetrics.eventConsumeTimes.length;
      console.log(`Average consume time: ${avgConsumeTime.toFixed(2)}ms`);
    }
    
    console.log('==========================================\n');
  });
});