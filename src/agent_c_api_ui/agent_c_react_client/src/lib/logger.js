/**
 * Enhanced logging utility based on loglevel
 * Provides structured logging with context
 * 
 * This version has been simplified to avoid performance issues.
 */

import log from 'loglevel';

// Set default level based on environment - production logs should be minimal
const defaultLevel = process.env.NODE_ENV === 'production' ? 'error' : 'info';
log.setLevel(defaultLevel);

// Log history array - limited to 100 entries
const logHistory = [];
const MAX_LOG_HISTORY = 100;

// Subscribers for log events
const subscribers = [];

// Simple logger implementation that avoids performance issues
const logger = {
  /**
   * Log at TRACE level
   */
  trace: (message, componentName, data) => {
    log.trace(`[${componentName || 'unknown'}] ${message}`);
    addToHistory('trace', message, componentName, data);
  },
  
  /**
   * Log performance metrics
   */
  performance: (componentName, operation, durationMs) => {
    log.info(`[${componentName || 'unknown'}] PERF: ${operation} took ${durationMs}ms`);
    addToHistory('performance', `${operation} took ${durationMs}ms`, componentName, { duration: durationMs, operation });
  },
  
  /**
   * Log at DEBUG level
   * In production, this is a no-op for performance reasons
   */
  debug: (message, componentName, data) => {
    // Skip debug logging in production for performance
    if (process.env.NODE_ENV === 'production') return;
    
    log.debug(`[${componentName || 'unknown'}] ${message}`);
    addToHistory('debug', message, componentName, data);
  },
  
  /**
   * Log at INIT level (alias for info)
   */
  init: (message, componentName, data) => {
    log.info(`[${componentName || 'unknown'}] INIT: ${message}`);
    addToHistory('init', message, componentName, data);
  },
  
  /**
   * Log at INFO level
   */
  info: (message, componentName, data) => {
    log.info(`[${componentName || 'unknown'}] ${message}`);
    addToHistory('info', message, componentName, data);
  },
  
  /**
   * Log at WARN level
   */
  warn: (message, componentName, data) => {
    log.warn(`[${componentName || 'unknown'}] ${message}`);
    addToHistory('warn', message, componentName, data);
  },
  
  /**
   * Log at ERROR level
   */
  error: (message, componentName, data) => {
    log.error(`[${componentName || 'unknown'}] ${message}`);
    addToHistory('error', message, componentName, data);
  },
  
  /**
   * Set the logging level
   */
  setLevel: (level) => {
    log.setLevel(level);
  },
  
  /**
   * Get the current logging level
   */
  getLevel: () => log.getLevel(),
  
  /**
   * Get the log level name
   */
  getLevelName: () => {
    const level = log.getLevel();
    return Object.keys(log.levels).find(key => log.levels[key] === level) || 'unknown';
  },
  
  /**
   * Get the log history
   */
  getLogHistory: () => {
    return [...logHistory];
  },
  
  /**
   * Clear the log history
   */
  clearLogHistory: () => {
    logHistory.length = 0;
    notifySubscribers();
  },
  
  /**
   * Subscribe to log updates
   */
  onNewLog: (callback) => {
    subscribers.push(callback);
    return () => {
      const index = subscribers.indexOf(callback);
      if (index !== -1) {
        subscribers.splice(index, 1);
      }
    };
  }
};

/**
 * Add a log entry to the history
 */
function addToHistory(level, message, component, data) {
  // Keep history limited to prevent memory issues
  if (logHistory.length >= MAX_LOG_HISTORY) {
    logHistory.shift();
  }
  
  logHistory.push({
    level,
    timestamp: Date.now(),
    component,
    message,
    data
  });
  
  notifySubscribers();
}

/**
 * Notify all subscribers about a new log entry
 */
function notifySubscribers() {
  subscribers.forEach(callback => {
    try {
      callback();
    } catch (e) {
      console.error('Error in log subscriber:', e);
    }
  });
}

/**
 * In production, optionally replace console methods with no-ops
 * to prevent performance issues when dev tools are open
 */
function setupProductionConsole() {
  if (process.env.NODE_ENV === 'production') {
    // Store original console methods
    const originalConsole = {
      log: console.log,
      debug: console.debug,
      info: console.info,
      warn: console.warn,
      error: console.error
    };
    
    // Replace non-critical methods with no-ops
    console.log = function() {};
    console.debug = function() {};
    console.info = function() {};
    
    // Keep error and warn for critical issues
    // console.warn = function() {};
    // console.error = function() {};
    
    // Create a method to restore original behavior if needed
    console.enableFullLogging = function() {
      console.log = originalConsole.log;
      console.debug = originalConsole.debug;
      console.info = originalConsole.info;
      console.warn = originalConsole.warn;
      console.error = originalConsole.error;
      console.log('Full console logging restored');
    };
  }
}

// Set up production console behavior
setupProductionConsole();

export default logger;