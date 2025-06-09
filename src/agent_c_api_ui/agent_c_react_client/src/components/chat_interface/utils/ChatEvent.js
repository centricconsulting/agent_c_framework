/**
 * ChatEvent - Base class for chat events with Redis stream support
 * 
 * Provides a standardized structure for chat events that includes Redis stream ID support,
 * event correlation, and metadata tracking for enhanced replay and debugging capabilities.
 */

/**
 * Generates a unique correlation ID for tracking related events
 * @returns {string} A unique correlation ID
 */
const generateCorrelationId = () => {
  return `${Date.now()}-${Math.random().toString(36).substring(2, 9)}`;
};

/**
 * ChatEvent base class with Redis stream support
 */
export class ChatEvent {
  /**
   * Create a new ChatEvent instance
   * @param {Object} options - Event configuration options
   * @param {string} options.type - The event type (e.g., 'text_delta', 'tool_calls', 'system_message')
   * @param {any} options.data - The event data/content
   * @param {string} [options.sessionId] - Session identifier for event correlation
   * @param {string} [options.streamId] - Redis stream ID for event tracking
   * @param {string} [options.correlationId] - Correlation ID for grouping related events
   * @param {Object} [options.metadata={}] - Additional metadata for the event
   * @param {string} [options.timestamp] - Event timestamp (ISO-8601), defaults to current time
   * @param {string} [options.role] - Event role (user, assistant, system, etc.)
   * @param {string} [options.vendor] - Model vendor (anthropic, openai, etc.)
   * @param {string} [options.format='markdown'] - Content format
   */
  constructor(options = {}) {
    // Required properties
    this.type = options.type || 'unknown';
    this.data = options.data || null;
    
    // Stream and correlation properties
    this.sessionId = options.sessionId || null;
    this.streamId = options.streamId || null;
    this.correlationId = options.correlationId || generateCorrelationId();
    
    // Temporal properties
    this.timestamp = options.timestamp || new Date().toISOString();
    
    // Event context properties
    this.role = options.role || null;
    this.vendor = options.vendor || null;
    this.format = options.format || 'markdown';
    
    // Metadata for additional event information
    this.metadata = {
      ...options.metadata
    };
    
    // Internal tracking properties
    this.id = `${this.type}-${this.correlationId}`;
    this.version = '1.0';
  }

  /**
   * Set the Redis stream ID for this event
   * @param {string} streamId - The Redis stream ID
   * @returns {ChatEvent} This instance for method chaining
   */
  setStreamId(streamId) {
    this.streamId = streamId;
    this.metadata.streamId = streamId;
    return this;
  }

  /**
   * Set the session ID for this event
   * @param {string} sessionId - The session identifier
   * @returns {ChatEvent} This instance for method chaining
   */
  setSessionId(sessionId) {
    this.sessionId = sessionId;
    this.metadata.sessionId = sessionId;
    return this;
  }

  /**
   * Add metadata to the event
   * @param {string} key - Metadata key
   * @param {any} value - Metadata value
   * @returns {ChatEvent} This instance for method chaining
   */
  addMetadata(key, value) {
    this.metadata[key] = value;
    return this;
  }

  /**
   * Get metadata value by key
   * @param {string} key - Metadata key
   * @returns {any} The metadata value or undefined
   */
  getMetadata(key) {
    return this.metadata[key];
  }

  /**
   * Check if this event is of a specific type
   * @param {string|Array<string>} types - Type or array of types to check
   * @returns {boolean} True if the event matches any of the specified types
   */
  isType(types) {
    if (Array.isArray(types)) {
      return types.includes(this.type);
    }
    return this.type === types;
  }

  /**
   * Check if this event belongs to a specific session
   * @param {string} sessionId - Session ID to check
   * @returns {boolean} True if the event belongs to the session
   */
  belongsToSession(sessionId) {
    return this.sessionId === sessionId;
  }

  /**
   * Check if this event is correlated with another event
   * @param {ChatEvent|string} eventOrCorrelationId - Another ChatEvent or correlation ID
   * @returns {boolean} True if events are correlated
   */
  isCorrelated(eventOrCorrelationId) {
    if (typeof eventOrCorrelationId === 'string') {
      return this.correlationId === eventOrCorrelationId;
    }
    if (eventOrCorrelationId instanceof ChatEvent) {
      return this.correlationId === eventOrCorrelationId.correlationId;
    }
    return false;
  }

  /**
   * Get the age of this event in milliseconds
   * @returns {number} Age in milliseconds
   */
  getAge() {
    return Date.now() - new Date(this.timestamp).getTime();
  }

  /**
   * Create a formatted object suitable for display in UI components
   * @returns {Object} Formatted event object
   */
  toDisplayFormat() {
    return {
      id: this.id,
      type: this.type,
      content: this.data,
      timestamp: this.timestamp,
      role: this.role,
      vendor: this.vendor,
      format: this.format,
      streamId: this.streamId,
      correlationId: this.correlationId,
      metadata: { ...this.metadata }
    };
  }

  /**
   * Serialize the event for Redis stream storage
   * @returns {Object} Serializable event object
   */
  serialize() {
    return {
      id: this.id,
      type: this.type,
      data: typeof this.data === 'string' ? this.data : JSON.stringify(this.data),
      sessionId: this.sessionId,
      streamId: this.streamId,
      correlationId: this.correlationId,
      timestamp: this.timestamp,
      role: this.role,
      vendor: this.vendor,
      format: this.format,
      metadata: JSON.stringify(this.metadata),
      version: this.version
    };
  }

  /**
   * Create a ChatEvent instance from serialized data
   * @param {Object} serializedData - Serialized event data
   * @returns {ChatEvent} Reconstructed ChatEvent instance
   */
  static deserialize(serializedData) {
    try {
      let data = serializedData.data;
      let metadata = {};

      // Parse JSON data if it's a string
      if (typeof data === 'string') {
        try {
          data = JSON.parse(data);
        } catch {
          // Keep as string if parsing fails
        }
      }

      // Parse metadata if it's a string
      if (typeof serializedData.metadata === 'string') {
        try {
          metadata = JSON.parse(serializedData.metadata);
        } catch {
          metadata = {};
        }
      } else if (serializedData.metadata && typeof serializedData.metadata === 'object') {
        metadata = serializedData.metadata;
      }

      return new ChatEvent({
        type: serializedData.type,
        data: data,
        sessionId: serializedData.sessionId,
        streamId: serializedData.streamId,
        correlationId: serializedData.correlationId,
        timestamp: serializedData.timestamp,
        role: serializedData.role,
        vendor: serializedData.vendor,
        format: serializedData.format,
        metadata: metadata
      });
    } catch (error) {
      console.error('Error deserializing ChatEvent:', error);
      throw new Error(`Failed to deserialize ChatEvent: ${error.message}`);
    }
  }

  /**
   * Create a ChatEvent from raw streaming data
   * @param {Object} rawData - Raw data from message stream
   * @param {Object} options - Additional options for event creation
   * @returns {ChatEvent} New ChatEvent instance
   */
  static fromStreamData(rawData, options = {}) {
    // Extract event information from raw streaming data
    const eventType = rawData.type || 'unknown';
    const eventData = rawData.data || rawData.content || '';
    
    return new ChatEvent({
      type: eventType,
      data: eventData,
      role: rawData.role,
      vendor: rawData.vendor,
      format: rawData.format || 'markdown',
      timestamp: rawData.timestamp || new Date().toISOString(),
      sessionId: options.sessionId,
      streamId: options.streamId,
      correlationId: options.correlationId,
      metadata: {
        ...rawData.metadata,
        ...options.metadata,
        // Store original raw data for debugging
        originalRaw: rawData
      }
    });
  }

  /**
   * Create a collection of ChatEvent instances from an array of raw data
   * @param {Array} rawDataArray - Array of raw event data
   * @param {Object} options - Common options for all events
   * @returns {Array<ChatEvent>} Array of ChatEvent instances
   */
  static fromStreamDataArray(rawDataArray, options = {}) {
    if (!Array.isArray(rawDataArray)) {
      return [];
    }

    return rawDataArray.map(rawData => 
      ChatEvent.fromStreamData(rawData, options)
    );
  }

  /**
   * Group events by correlation ID
   * @param {Array<ChatEvent>} events - Array of ChatEvent instances
   * @returns {Object} Object with correlation IDs as keys and event arrays as values
   */
  static groupByCorrelationId(events) {
    return events.reduce((groups, event) => {
      const correlationId = event.correlationId;
      if (!groups[correlationId]) {
        groups[correlationId] = [];
      }
      groups[correlationId].push(event);
      return groups;
    }, {});
  }

  /**
   * Filter events by type
   * @param {Array<ChatEvent>} events - Array of ChatEvent instances
   * @param {string|Array<string>} types - Event type(s) to filter by
   * @returns {Array<ChatEvent>} Filtered events
   */
  static filterByType(events, types) {
    const typeArray = Array.isArray(types) ? types : [types];
    return events.filter(event => typeArray.includes(event.type));
  }

  /**
   * Sort events by timestamp
   * @param {Array<ChatEvent>} events - Array of ChatEvent instances
   * @param {string} order - Sort order ('asc' or 'desc')
   * @returns {Array<ChatEvent>} Sorted events
   */
  static sortByTimestamp(events, order = 'asc') {
    return [...events].sort((a, b) => {
      const aTime = new Date(a.timestamp).getTime();
      const bTime = new Date(b.timestamp).getTime();
      return order === 'asc' ? aTime - bTime : bTime - aTime;
    });
  }

  /**
   * Get events within a time range
   * @param {Array<ChatEvent>} events - Array of ChatEvent instances
   * @param {string|Date} startTime - Start time (ISO-8601 string or Date)
   * @param {string|Date} endTime - End time (ISO-8601 string or Date)
   * @returns {Array<ChatEvent>} Events within the time range
   */
  static getEventsInTimeRange(events, startTime, endTime) {
    const start = new Date(startTime).getTime();
    const end = new Date(endTime).getTime();
    
    return events.filter(event => {
      const eventTime = new Date(event.timestamp).getTime();
      return eventTime >= start && eventTime <= end;
    });
  }
}

export default ChatEvent;