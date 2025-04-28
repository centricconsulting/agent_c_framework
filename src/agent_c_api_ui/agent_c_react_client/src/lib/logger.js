/**
 * Logger utility - Performance optimized version with context state tracking
 * 
 * This implementation has been enhanced with context state transition tracking
 * to help diagnose initialization and state management issues.
 * 
 * Performance optimization features:
 * - No history tracking in production
 * - No subscribers in production
 * - No complex data processing
 * - No console method overriding
 */

import log from 'loglevel';

// Set default level based on environment
const isProduction = process.env.NODE_ENV === 'production';
const defaultLevel = isProduction ? 'error' : 'info';
log.setLevel(defaultLevel);

// Only in development: maintain minimal log history
const MAX_HISTORY_SIZE = 50;
const logHistory = isProduction ? null : [];

// State transition history (even more limited)
const MAX_STATE_HISTORY_SIZE = 20;
const stateTransitionHistory = isProduction ? null : [];

// High performance logger implementation
const logger = {
  /**
   * Log for initialization - same as info in most cases
   */
  init: (message, componentName, data) => {
    if (isProduction) {
      log.info(`[${componentName || 'unknown'}] ${message}`);
      return;
    }
    
    log.info(`[${componentName || 'unknown'}] ${message}`);
    if (logHistory) {
      addToMinimalHistory('init', message, componentName);
    }
  },

  /**
   * Log at TRACE level - minimal implementation
   */
  trace: (message, componentName) => {
    if (isProduction) return;
    log.trace(`[${componentName || 'unknown'}] ${message}`);
    if (!isProduction && logHistory) {
      addToMinimalHistory('trace', message, componentName);
    }
  },
  
  /**
   * Log performance metrics - keeps working in production for critical perf issues
   */
  performance: (componentName, operation, durationMs) => {
    if (durationMs > 100) { // Only log slow operations even in production
      log.warn(`[${componentName || 'unknown'}] PERF WARNING: ${operation} took ${durationMs}ms`);
    } else if (!isProduction) {
      log.info(`[${componentName || 'unknown'}] PERF: ${operation} took ${durationMs}ms`);
      if (logHistory) {
        addToMinimalHistory('performance', `${operation}: ${durationMs}ms`, componentName);
      }
    }
  },
  
  /**
   * Log at DEBUG level - complete no-op in production
   */
  debug: (message, componentName, data) => {
    // Complete no-op in production for maximum performance
    if (isProduction) return;
    
    // Only log in development
    log.debug(`[${componentName || 'unknown'}] ${message}`);
    if (logHistory) {
      addToMinimalHistory('debug', message, componentName);
    }
  },
  
  /**
   * Log at INFO level - simplified in production
   */
  info: (message, componentName) => {
    // In production, just do the minimal logging without history
    if (isProduction) {
      log.info(`[${componentName || 'unknown'}] ${message}`);
      return;
    }
    
    log.info(`[${componentName || 'unknown'}] ${message}`);
    if (logHistory) {
      addToMinimalHistory('info', message, componentName);
    }
  },
  
  /**
   * Log at WARN level - always enabled
   */
  warn: (message, componentName) => {
    log.warn(`[${componentName || 'unknown'}] ${message}`);
    if (!isProduction && logHistory) {
      addToMinimalHistory('warn', message, componentName);
    }
  },
  
  /**
   * Log at ERROR level - always enabled
   */
  error: (message, componentName, error) => {
    // Include error details if available
    const errorDetails = error instanceof Error ? ` ${error.name}: ${error.message}` : '';
    log.error(`[${componentName || 'unknown'}] ${message}${errorDetails}`);
    
    if (!isProduction && logHistory) {
      addToMinimalHistory('error', message, componentName);
    }
  },
  
  /**
   * Log context state transitions - specialized method for context debugging
   * @param {string} contextName - Name of the context
   * @param {string} previousState - Previous state descriptor
   * @param {string} newState - New state descriptor
   * @param {Object} details - Additional transition details
   */
  contextState: (contextName, previousState, newState, details = {}) => {
    // Create a formatted message for the log
    const message = `Context state transition: ${previousState} → ${newState}`;
    
    // Always log as info at minimum to ensure visibility
    log.info(`[${contextName || 'UnknownContext'}] ${message}`);
    
    // Skip history tracking in production
    if (isProduction || !stateTransitionHistory) {
      return;
    }
    
    // Keep history limited
    if (stateTransitionHistory.length >= MAX_STATE_HISTORY_SIZE) {
      stateTransitionHistory.shift();
    }
    
    // Add to state transition history with more details
    stateTransitionHistory.push({
      timestamp: Date.now(),
      contextName: contextName || 'UnknownContext',
      previousState,
      newState,
      details: { ...details },
      durationMs: details.startTime ? Date.now() - details.startTime : undefined
    });
    
    // Also add to regular history
    addToMinimalHistory('state', message, contextName);
  },
  
  // Simplified utility methods - most are no-ops in production
  setLevel: (level) => log.setLevel(level),
  getLevel: () => log.getLevel(),
  getLevelName: () => isProduction ? 'error' : Object.keys(log.levels).find(key => log.levels[key] === log.getLevel()) || 'unknown',
  
  // History is disabled in production
  getLogHistory: () => isProduction ? [] : [...(logHistory || [])],
  clearLogHistory: () => {
    if (!isProduction && logHistory) {
      logHistory.length = 0;
    }
  },
  
  // State transition history methods
  getStateHistory: () => isProduction ? [] : [...(stateTransitionHistory || [])],
  clearStateHistory: () => {
    if (!isProduction && stateTransitionHistory) {
      stateTransitionHistory.length = 0;
    }
  },
  getLatestStates: (contextName, limit = 5) => {
    if (isProduction || !stateTransitionHistory) {
      return [];
    }
    // Filter by context name if provided
    const filtered = contextName 
      ? stateTransitionHistory.filter(s => s.contextName === contextName)
      : stateTransitionHistory;
    
    // Return the most recent entries up to the limit
    return filtered.slice(-limit);
  },
  
  // Subscriber pattern disabled in production
  onNewLog: () => {
    // Return a no-op cleanup function
    return () => {};
  }
};

/**
 * Add a minimal log entry to history (dev only)
 */
function addToMinimalHistory(level, message, component) {
  if (isProduction || !logHistory) return;
  
  // Keep history limited
  if (logHistory.length >= MAX_HISTORY_SIZE) {
    logHistory.shift();
  }
  
  // Only store minimal information
  logHistory.push({
    level,
    timestamp: Date.now(),
    component,
    message
  });
}

export default logger;