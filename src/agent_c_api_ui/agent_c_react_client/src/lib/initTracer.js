/**
 * Initialization Tracing Utilities
 * 
 * This module provides utilities for tracing the initialization flow of the application's
 * context hierarchy, providing visibility into state transitions, initialization order,
 * and potential issues during startup.
 */

import logger from './logger';
import eventBus from './eventBus';

// Initialization states as constants
export const InitState = {
  // General states
  UNKNOWN: 'unknown',
  INITIALIZING: 'initializing',
  READY: 'ready',
  ERROR: 'error',
  
  // Auth context specific states
  AUTH_CHECKING_STORAGE: 'auth_checking_storage',
  AUTH_SESSION_FOUND: 'auth_session_found',
  AUTH_NO_SESSION: 'auth_no_session',
  AUTH_CREATING_SESSION: 'auth_creating_session',
  AUTH_COMPLETE: 'auth_complete',
  AUTH_ERROR: 'auth_error',
  
  // Model context specific states
  MODEL_FETCHING_AVAILABLE: 'model_fetching_available',
  MODEL_FETCHED_AVAILABLE: 'model_fetched_available',
  MODEL_LOADING: 'model_loading',
  MODEL_LIST_LOADING: 'model_list_loading',
  MODEL_CONFIGURING: 'model_configuring',
  MODEL_LOADING_PREFERENCES: 'model_loading_preferences',
  MODEL_FOUND_PREFERENCES: 'model_found_preferences',
  MODEL_NO_PREFERENCES: 'model_validating_preferences',
  MODEL_VALIDATING_PREFERENCES: 'model_validating_preferences',
  MODEL_READY: 'model_ready',
  MODEL_ERROR: 'model_error',
  
  // Session context specific states
  SESSION_LOADING: 'session_loading',
  SESSION_INITIALIZING: 'session_initializing',
  SESSION_READY: 'session_ready',
  SESSION_ERROR: 'session_error',
  SESSION_CHECKING_STORAGE: 'session_checking_storage',
  SESSION_FOUND: 'session_found',
  SESSION_NOT_FOUND: 'session_not_found',
  SESSION_VALIDATING: 'session_validating',
  SESSION_VALID: 'session_valid',
  SESSION_INVALID: 'session_invalid',
  SESSION_CREATING_NEW: 'session_creating_new',
  SESSION_CREATED: 'session_created',
  SESSION_LOADING_HISTORY: 'session_loading_history',
  SESSION_HISTORY_LOADED: 'session_history_loaded',
  SESSION_LOADING_PREFERENCES: 'session_loading_preferences',
  SESSION_STORAGE_ERROR: 'session_storage_error',

};

// Event types for initialization events
export const InitEvents = {
  STATE_CHANGED: 'init_state_changed',
  AUTH_COMPLETE: 'auth_complete',
  MODEL_READY: 'model_ready',
  SESSION_READY: 'session_ready',
  INITIALIZATION_COMPLETE: 'initialization_complete',
  INITIALIZATION_ERROR: 'initialization_error',
};

/**
 * Create an initialization tracer for a specific context
 * @param {string} contextName - Name of the context
 * @returns {Object} Tracer object with methods for tracking initialization
 */
export function createInitTracer(contextName) {
  if (!contextName) {
    logger.warn('createInitTracer called without contextName', 'initTracer');
    contextName = 'UnknownContext';
  }
  
  // Internal state
  let currentState = InitState.UNKNOWN;
  let startTime = Date.now();
  let lastUpdateTime = startTime;
  let error = null;
  let stateHistory = [];
  
  // The tracer object
  const tracer = {
    /**
     * Get the current initialization state
     * @returns {string} Current state
     */
    getState: () => currentState,
    
    /**
     * Get initialization info including timing
     * @returns {Object} Initialization info
     */
    getInfo: () => ({
      contextName,
      currentState,
      startTime,
      lastUpdateTime,
      elapsed: Date.now() - startTime,
      timeSinceLastUpdate: Date.now() - lastUpdateTime,
      error,
      stateHistory: [...stateHistory],
    }),
    
    /**
     * Update the initialization state
     * @param {string} newState - New state from InitState constants
     * @param {Object} details - Additional details about the state change
     */
    setState: (newState, details = {}) => {
      if (!newState || newState === currentState) {
        return; // No change or invalid state
      }
      
      const previousState = currentState;
      const timestamp = Date.now();
      const elapsed = timestamp - startTime;
      const timeSinceLastUpdate = timestamp - lastUpdateTime;
      
      // Update internal state
      currentState = newState;
      lastUpdateTime = timestamp;
      
      // Add to state history
      stateHistory.push({
        timestamp,
        state: newState,
        elapsed,
        timeSinceLastUpdate
      });
      
      // Limit history size
      if (stateHistory.length > 20) {
        stateHistory.shift();
      }
      
      // Log the state change
      logger.contextState(contextName, previousState, newState, {
        ...details,
        elapsed,
        timeSinceLastUpdate,
        startTime
      });
      
      // Publish state change event
      eventBus.publish(InitEvents.STATE_CHANGED, {
        contextName,
        previousState,
        newState,
        timestamp,
        elapsed,
        details
      }, { publisherName: contextName });
      
      // Publish specific events based on state
      switch (newState) {
        case InitState.AUTH_COMPLETE:
          eventBus.publish(InitEvents.AUTH_COMPLETE, {
            timestamp,
            elapsed
          }, { publisherName: contextName });
          break;
        case InitState.MODEL_READY:
          eventBus.publish(InitEvents.MODEL_READY, {
            timestamp,
            elapsed
          }, { publisherName: contextName });
          break;
        case InitState.SESSION_READY:
          eventBus.publish(InitEvents.SESSION_READY, {
            timestamp,
            elapsed
          }, { publisherName: contextName });
          break;
        case InitState.READY:
          eventBus.publish(InitEvents.INITIALIZATION_COMPLETE, {
            timestamp,
            elapsed,
            contextName
          }, { publisherName: contextName });
          break;
        case InitState.ERROR:
          eventBus.publish(InitEvents.INITIALIZATION_ERROR, {
            timestamp,
            elapsed,
            contextName,
            error: details.error || error
          }, { publisherName: contextName });
          break;
      }
    },
    
    /**
     * Set error state with details
     * @param {Error|string} err - Error object or message
     * @param {Object} details - Additional error details
     */
    setError: (err, details = {}) => {
      // Normalize error object
      if (typeof err === 'string') {
        error = new Error(err);
      } else if (err instanceof Error) {
        error = err;
      } else {
        error = new Error('Unknown error');
      }
      
      // Extract error details
      const errorDetails = {
        message: error.message,
        name: error.name,
        stack: error.stack,
        ...details
      };
      
      // Update state to error
      tracer.setState(InitState.ERROR, {
        error: errorDetails,
        ...details
      });
    },
    
    /**
     * Reset the tracer to its initial state
     */
    reset: () => {
      currentState = InitState.UNKNOWN;
      startTime = Date.now();
      lastUpdateTime = startTime;
      error = null;
      stateHistory = [];
      
      logger.info(`Initialization tracer reset for ${contextName}`, 'initTracer');
    },
    
    /**
     * Mark initialization as complete
     * @param {Object} details - Additional completion details
     */
    markReady: (details = {}) => {
      tracer.setState(InitState.READY, details);
    }
  };
  
  logger.debug(`Initialization tracer created for ${contextName}`, 'initTracer');
  return tracer;
}

/**
 * Create a global initialization tracker that monitors all contexts
 * @returns {Object} Global tracker for monitoring initialization
 */
export function createGlobalInitTracker() {
  // Map of context names to their current states
  const contextStates = new Map();
  
  // Subscribe to initialization events
  const unsubscribe = eventBus.subscribe(InitEvents.STATE_CHANGED, (data) => {
    const { contextName, newState } = data;
    if (contextName && newState) {
      contextStates.set(contextName, {
        state: newState,
        timestamp: Date.now(),
        details: data
      });
    }
  }, { componentName: 'GlobalInitTracker' });
  
  return {
    /**
     * Get the current state of all contexts
     * @returns {Object} Map of context names to states
     */
    getAllContextStates: () => {
      return new Map(contextStates);
    },
    
    /**
     * Get the state of a specific context
     * @param {string} contextName - Name of the context
     * @returns {string|null} Context state or null if not found
     */
    getContextState: (contextName) => {
      const context = contextStates.get(contextName);
      return context ? context.state : null;
    },
    
    /**
     * Check if all required contexts are in the ready state
     * @param {Array<string>} requiredContexts - Array of context names to check
     * @returns {boolean} True if all contexts are ready
     */
    areContextsReady: (requiredContexts) => {
      if (!requiredContexts || !requiredContexts.length) {
        return false;
      }
      
      return requiredContexts.every(contextName => {
        const context = contextStates.get(contextName);
        return context && context.state === InitState.READY;
      });
    },
    
    /**
     * Get details about contexts that aren't ready
     * @param {Array<string>} requiredContexts - Array of context names to check
     * @returns {Array} Array of objects with context details
     */
    getPendingContexts: (requiredContexts) => {
      if (!requiredContexts || !requiredContexts.length) {
        return [];
      }
      
      return requiredContexts
        .filter(contextName => {
          const context = contextStates.get(contextName);
          return !context || context.state !== InitState.READY;
        })
        .map(contextName => {
          const context = contextStates.get(contextName) || { state: 'unknown' };
          return {
            contextName,
            state: context.state,
            details: context.details
          };
        });
    },
    
    /**
     * Check if any contexts are in error state
     * @param {Array<string>} contextsToCheck - Array of context names to check
     * @returns {Array} Array of contexts in error state
     */
    getErrorContexts: (contextsToCheck) => {
      const contextNames = contextsToCheck || Array.from(contextStates.keys());
      
      return contextNames
        .filter(contextName => {
          const context = contextStates.get(contextName);
          return context && context.state === InitState.ERROR;
        })
        .map(contextName => {
          const context = contextStates.get(contextName);
          return {
            contextName,
            details: context.details
          };
        });
    },
    
    /**
     * Reset the tracker
     */
    reset: () => {
      contextStates.clear();
    },
    
    /**
     * Unsubscribe from events when no longer needed
     */
    cleanup: () => {
      unsubscribe();
      contextStates.clear();
    }
  };
}

// Export singleton instance of the global tracker
export const globalInitTracker = createGlobalInitTracker();