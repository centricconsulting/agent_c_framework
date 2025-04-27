/**
 * Logger utility - Performance optimized version
 * 
 * This implementation has been completely rewritten to eliminate performance issues.
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