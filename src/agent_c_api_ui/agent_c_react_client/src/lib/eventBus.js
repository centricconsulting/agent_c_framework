/**
 * Event Bus for Inter-Context Communication
 * 
 * This module provides a centralized event bus for communication between contexts
 * without creating direct dependencies. It implements a simple pub/sub pattern
 * that allows components to subscribe to events and publish events.
 */

import logger from './logger';

// Map of event types to arrays of subscribers
const subscribers = new Map();

// Event history for debugging (limited size in dev only)
const isProduction = process.env.NODE_ENV === 'production';
const MAX_HISTORY_SIZE = isProduction ? 0 : 50;
const eventHistory = isProduction ? null : [];

const eventBus = {
  /**
   * Subscribe to an event
   * @param {string} eventType - The event type to subscribe to
   * @param {Function} callback - Function to call when event is published
   * @param {Object} options - Subscription options
   * @param {string} options.id - Optional identifier for this subscription
   * @param {string} options.componentName - Name of component for logging
   * @returns {Function} Unsubscribe function
   */
  subscribe: (eventType, callback, options = {}) => {
    if (!eventType || typeof callback !== 'function') {
      logger.warn('Invalid eventBus.subscribe call', 'eventBus', { 
        hasEventType: !!eventType, 
        hasCallback: typeof callback === 'function' 
      });
      return () => {}; // Return no-op unsubscribe function
    }
    
    const id = options.id || `sub_${Date.now()}_${Math.random().toString(36).substr(2, 9)}`;
    const componentName = options.componentName || 'unknown';
    
    // Create subscription entry
    const subscription = {
      id,
      callback,
      componentName,
      subscribeTime: Date.now()
    };
    
    // Get or create subscribers array for this event type
    if (!subscribers.has(eventType)) {
      subscribers.set(eventType, []);
    }
    
    // Add to subscribers
    subscribers.get(eventType).push(subscription);
    
    logger.debug(`Subscribed to event: ${eventType}`, 'eventBus', { 
      id, 
      componentName,
      subscriberCount: subscribers.get(eventType).length 
    });
    
    // Return unsubscribe function
    return () => {
      eventBus.unsubscribe(eventType, id);
    };
  },
  
  /**
   * Unsubscribe from an event
   * @param {string} eventType - The event type to unsubscribe from
   * @param {string} id - The subscription ID returned from subscribe
   * @returns {boolean} True if successfully unsubscribed
   */
  unsubscribe: (eventType, id) => {
    if (!eventType || !id || !subscribers.has(eventType)) {
      return false;
    }
    
    const subs = subscribers.get(eventType);
    const initialLength = subs.length;
    
    // Filter out the subscription with the matching ID
    const filtered = subs.filter(sub => sub.id !== id);
    subscribers.set(eventType, filtered);
    
    const removed = initialLength > filtered.length;
    
    if (removed) {
      logger.debug(`Unsubscribed from event: ${eventType}`, 'eventBus', { 
        id, 
        subscriberCount: filtered.length 
      });
    }
    
    return removed;
  },
  
  /**
   * Publish an event
   * @param {string} eventType - The event type to publish
   * @param {*} data - Data to pass to subscribers
   * @param {Object} options - Publishing options
   * @param {string} options.publisherName - Name of publisher for logging
   * @returns {boolean} True if event was successfully published
   */
  publish: (eventType, data = {}, options = {}) => {
    if (!eventType) {
      logger.warn('Invalid eventBus.publish call: missing eventType', 'eventBus');
      return false;
    }
    
    const publisherName = options.publisherName || 'unknown';
    
    // Record event in history if in development
    if (!isProduction && eventHistory) {
      if (eventHistory.length >= MAX_HISTORY_SIZE) {
        eventHistory.shift(); // Remove oldest event
      }
      
      eventHistory.push({
        timestamp: Date.now(),
        eventType,
        publisherName,
        data: JSON.parse(JSON.stringify(data || {}))
      });
    }
    
    // Log the event
    logger.debug(`Event published: ${eventType}`, 'eventBus', { 
      publisherName,
      hasSubscribers: subscribers.has(eventType) && subscribers.get(eventType).length > 0,
      subscriberCount: subscribers.has(eventType) ? subscribers.get(eventType).length : 0 
    });
    
    // If no subscribers, just return true (successful publish)
    if (!subscribers.has(eventType) || subscribers.get(eventType).length === 0) {
      return true;
    }
    
    // Call all subscribers with the data
    const subs = subscribers.get(eventType);
    
    try {
      // Clone data to prevent modification by subscribers affecting other subscribers
      const clonedData = JSON.parse(JSON.stringify(data || {}));
      
      // Call all subscribers
      subs.forEach(sub => {
        try {
          sub.callback(clonedData, {
            eventType,
            publisherName,
            publishTime: Date.now()
          });
        } catch (err) {
          // Log error but don't stop other subscribers from receiving the event
          logger.error(`Error in event subscriber callback: ${eventType}`, 'eventBus', {
            eventType,
            subscriberId: sub.id,
            componentName: sub.componentName,
            error: err.message,
            stack: err.stack
          });
        }
      });
      
      return true;
    } catch (err) {
      logger.error(`Error publishing event: ${eventType}`, 'eventBus', {
        error: err.message,
        stack: err.stack
      });
      return false;
    }
  },
  
  /**
   * Get event history (development only)
   * @returns {Array} Event history or empty array in production
   */
  getEventHistory: () => {
    if (isProduction || !eventHistory) {
      return [];
    }
    return [...eventHistory];
  },
  
  /**
   * Clear event history (development only)
   */
  clearEventHistory: () => {
    if (!isProduction && eventHistory) {
      eventHistory.length = 0;
    }
  },
  
  /**
   * Get subscriber count for an event type
   * @param {string} eventType - The event type to check
   * @returns {number} Number of subscribers
   */
  getSubscriberCount: (eventType) => {
    if (!eventType || !subscribers.has(eventType)) {
      return 0;
    }
    return subscribers.get(eventType).length;
  },
  
  /**
   * Get all event types with subscribers
   * @returns {Array<string>} Array of event types
   */
  getEventTypes: () => {
    return Array.from(subscribers.keys());
  }
};

export default eventBus;