import Redis from 'ioredis';
import { v4 as uuidv4 } from 'uuid';
import { DateTime } from 'luxon';

/**
 * RedisStreamManager handles event publishing and consumption using Redis Streams.
 * 
 * This class provides a clean interface for working with Redis Streams in the context
 * of the Agent C event system, handling connection management, event publishing,
 * and event consumption.
 */
class RedisStreamManager {
  /**
   * Initialize the Redis Stream Manager
   * 
   * @param {Object} config - Redis configuration options
   * @param {string} config.host - Redis host (default: 'localhost')
   * @param {number} config.port - Redis port (default: 6379)
   * @param {string} config.password - Redis password (default: null)
   * @param {number} config.db - Redis database (default: 0)
   * @param {boolean} config.tls - Use TLS for connection (default: false)
   * @param {number} config.maxConnections - Maximum connections in pool (default: 10)
   * @param {number} config.streamMaxLen - Maximum length of streams (default: 1000)
   * @param {number} config.streamTtl - TTL for streams in seconds (default: 3600)
   */
  constructor(config = {}) {
    this.config = {
      host: config.host || process.env.REDIS_HOST || 'localhost',
      port: config.port || parseInt(process.env.REDIS_PORT || '6379', 10),
      password: config.password || process.env.REDIS_PASSWORD,
      db: config.db || parseInt(process.env.REDIS_DB || '0', 10),
      tls: config.tls || process.env.REDIS_SSL === 'true',
      maxConnections: config.maxConnections || parseInt(process.env.REDIS_MAX_CONNECTIONS || '10', 10),
      streamMaxLen: config.streamMaxLen || parseInt(process.env.REDIS_STREAM_MAX_LEN || '1000', 10),
      streamTtl: config.streamTtl || parseInt(process.env.REDIS_STREAM_TTL || '3600', 10),
      readTimeout: config.readTimeout || parseInt(process.env.REDIS_STREAM_READ_TIMEOUT || '1000', 10)
    };
    
    // Create Redis connection pool
    this.redis = new Redis({
      host: this.config.host,
      port: this.config.port,
      password: this.config.password,
      db: this.config.db,
      tls: this.config.tls ? {} : undefined,
      maxRetriesPerRequest: 3,
      retryStrategy: (times) => {
        const delay = Math.min(times * 50, 2000);
        return delay;
      }
    });
    
    // Set up error handling
    this.redis.on('error', (error) => {
      console.error('Redis connection error:', error);
    });
  }
  
  /**
   * Get the stream key for a session and interaction
   * 
   * @param {string} sessionId - The session ID
   * @param {string} interactionId - The interaction ID
   * @returns {string} The stream key
   */
  getStreamKey(sessionId, interactionId) {
    return `agent_c:stream:${sessionId}:${interactionId}`;
  }
  
  /**
   * Publish an event to a Redis Stream
   * 
   * @param {string} eventType - Type of event (text_delta, tool_call, etc.)
   * @param {Object} eventData - Event data to publish
   * @param {string} sessionId - Session identifier
   * @param {string} interactionId - Interaction identifier
   * @param {number} sequence - Optional sequence number for ordering
   * @returns {Promise<string>} The ID of the message in the stream
   */
  async publishEvent(eventType, eventData, sessionId, interactionId, sequence = null) {
    const streamKey = this.getStreamKey(sessionId, interactionId);
    const timestamp = DateTime.now().toISO();
    const seq = sequence || await this.getNextSequence(streamKey);
    
    // Serialize event data to JSON
    const serializedData = typeof eventData === 'string' 
      ? eventData 
      : JSON.stringify(eventData);
    
    // Prepare fields for the stream entry
    const fields = {
      event_type: eventType,
      event_data: serializedData,
      timestamp,
      session_id: sessionId,
      interaction_id: interactionId,
      sequence: seq.toString()
    };
    
    try {
      // Add entry to the stream with automatic trimming
      const messageId = await this.redis.xadd(
        streamKey,
        'MAXLEN', '~', this.config.streamMaxLen,
        '*', // Auto-generate ID
        ...Object.entries(fields).flat()
      );
      
      // Set TTL on the stream key if not already set
      await this.redis.expire(streamKey, this.config.streamTtl);
      
      return messageId;
    } catch (error) {
      console.error(`Error publishing event to stream ${streamKey}:`, error);
      throw error;
    }
  }
  
  /**
   * Get the next sequence number for a stream
   * 
   * @param {string} streamKey - The stream key
   * @returns {Promise<number>} The next sequence number
   */
  async getNextSequence(streamKey) {
    // Get the last entry in the stream to determine the next sequence number
    const lastEntries = await this.redis.xrevrange(streamKey, '+', '-', 'COUNT', 1);
    
    if (lastEntries && lastEntries.length > 0) {
      const lastEntry = lastEntries[0];
      const fields = lastEntry[1];
      
      // Find the sequence field
      for (let i = 0; i < fields.length; i += 2) {
        if (fields[i] === 'sequence') {
          return parseInt(fields[i + 1], 10) + 1;
        }
      }
    }
    
    // If no entries or no sequence field, start at 1
    return 1;
  }
  
  /**
   * Consume events from a Redis Stream as an async generator
   * 
   * @param {string} sessionId - Session identifier
   * @param {string} interactionId - Interaction identifier
   * @param {string} lastId - ID to start reading from (default: '0-0' for beginning)
   * @param {number} block - Milliseconds to block waiting for new messages (default: config.readTimeout)
   * @yields {Object} The next event from the stream
   */
  async *consumeEvents(sessionId, interactionId, lastId = '0-0', block = null) {
    const streamKey = this.getStreamKey(sessionId, interactionId);
    const blockTime = block !== null ? block : this.config.readTimeout;
    let currentId = lastId;
    
    try {
      while (true) {
        // Read from the stream with blocking
        const streams = await this.redis.xread(
          'BLOCK', blockTime,
          'STREAMS', streamKey, currentId
        );
        
        // If no data and we're using a non-blocking read, we're done
        if (!streams || streams.length === 0) {
          if (blockTime === 0) break;
          continue; // For blocking reads, continue waiting
        }
        
        // Process messages
        const [key, messages] = streams[0];
        
        for (const [id, fields] of messages) {
          currentId = id; // Update ID for next read
          
          // Convert array of [field, value, field, value] to object
          const eventData = {};
          for (let i = 0; i < fields.length; i += 2) {
            eventData[fields[i]] = fields[i + 1];
          }
          
          // Parse JSON event data if it's a string
          if (eventData.event_data && typeof eventData.event_data === 'string') {
            try {
              eventData.event_data = JSON.parse(eventData.event_data);
            } catch (e) {
              // If it's not valid JSON, keep it as a string
              console.warn(`Could not parse event data as JSON: ${e.message}`);
            }
          }
          
          // Yield the event
          yield {
            id,
            ...eventData,
            sequence: parseInt(eventData.sequence || '0', 10)
          };
        }
      }
    } catch (error) {
      console.error(`Error consuming events from stream ${streamKey}:`, error);
      throw error;
    }
  }
  
  /**
   * Create a new stream for a session/interaction
   * 
   * @param {string} sessionId - Session identifier
   * @param {string} interactionId - Interaction identifier
   * @returns {Promise<string>} The stream key
   */
  async createStream(sessionId, interactionId) {
    const streamKey = this.getStreamKey(sessionId, interactionId);
    
    // Add a marker entry to create the stream
    await this.redis.xadd(
      streamKey,
      'MAXLEN', '~', this.config.streamMaxLen,
      '*',
      'event_type', 'stream_created',
      'timestamp', DateTime.now().toISO(),
      'session_id', sessionId,
      'interaction_id', interactionId,
      'sequence', '0'
    );
    
    // Set TTL on the stream
    await this.redis.expire(streamKey, this.config.streamTtl);
    
    return streamKey;
  }
  
  /**
   * Delete a stream or streams for a session/interaction
   * 
   * @param {string} sessionId - Session identifier
   * @param {string|null} interactionId - Interaction identifier (null to delete all for session)
   * @returns {Promise<number>} Number of streams deleted
   */
  async deleteStream(sessionId, interactionId = null) {
    if (interactionId) {
      // Delete specific stream
      const streamKey = this.getStreamKey(sessionId, interactionId);
      await this.redis.del(streamKey);
      return 1;
    } else {
      // Delete all streams for session
      const pattern = `agent_c:stream:${sessionId}:*`;
      const keys = await this.redis.keys(pattern);
      
      if (keys.length > 0) {
        await this.redis.del(...keys);
      }
      
      return keys.length;
    }
  }
  
  /**
   * List all streams matching the pattern
   * 
   * @param {string} pattern - Pattern to match (default: 'agent_c:stream:*')
   * @returns {Promise<string[]>} Array of stream keys
   */
  async listStreams(pattern = 'agent_c:stream:*') {
    return await this.redis.keys(pattern);
  }
  
  /**
   * Clean up streams older than the specified age
   * 
   * @param {number} maxAgeSeconds - Maximum age in seconds (default: config.streamTtl)
   * @returns {Promise<number>} Number of streams deleted
   */
  async cleanupOldStreams(maxAgeSeconds = null) {
    const ttl = maxAgeSeconds || this.config.streamTtl;
    const now = DateTime.now();
    const pattern = 'agent_c:stream:*';
    const streams = await this.listStreams(pattern);
    let deleted = 0;
    
    for (const streamKey of streams) {
      // Check the creation time of the first message
      const firstMessage = await this.redis.xrange(streamKey, '-', '+', 'COUNT', 1);
      
      if (firstMessage && firstMessage.length > 0) {
        const id = firstMessage[0][0];
        const fields = firstMessage[0][1];
        
        // Find the timestamp field
        let timestamp = null;
        for (let i = 0; i < fields.length; i += 2) {
          if (fields[i] === 'timestamp') {
            timestamp = fields[i + 1];
            break;
          }
        }
        
        if (timestamp) {
          const messageTime = DateTime.fromISO(timestamp);
          const ageSeconds = now.diff(messageTime, 'seconds').seconds;
          
          if (ageSeconds > ttl) {
            await this.redis.del(streamKey);
            deleted++;
          }
        }
      }
    }
    
    return deleted;
  }
  
  /**
   * Close the Redis connection
   * 
   * @returns {Promise<void>}
   */
  async close() {
    await this.redis.quit();
  }
}

export default RedisStreamManager;