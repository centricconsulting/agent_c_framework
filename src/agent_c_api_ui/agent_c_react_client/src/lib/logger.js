/**
 * Enhanced logging utility based on loglevel
 * Provides structured logging with context and performance tracking
 */

import log from 'loglevel';
import { safeStringify, safeInspect } from './safeSerializer';
import { DEBUG_MODE } from './diagnostic';

// Set default level based on environment
const defaultLevel = process.env.NODE_ENV === 'production' ? 'warn' : 'info';
log.setLevel(defaultLevel);

// Performance tracking
const performanceMetrics = {};

// Maximum size for the performance metric history
const MAX_PERFORMANCE_HISTORY = 100;

// ID for group tracking
let currentGroupId = 0;

/**
 * Format data for display in logs
 * @param {any} data - Data to format
 * @returns {string} Formatted data
 */
const formatData = (data) => {
  if (!data) return '';
  
  try {
    if (typeof data === 'string') return data;
    // Use safe stringify to prevent circular reference errors
    return safeStringify(data);
  } catch (e) {
    return '[Unstringifiable data]';
  }
};

// Enhanced logger with component tracking
const logger = {
  /**
   * Log at TRACE level
   * @param {string} message - Log message
   * @param {string} componentName - Component/module name
   * @param {any} data - Additional data
   */
  trace: (message, componentName, data) => {
    // Only log trace messages if DEBUG_MODE is enabled
    if (DEBUG_MODE) {
      // Use safe inspection for complex objects
      const safeData = data ? safeInspect(data) : undefined;
      log.trace(`[${componentName || 'unknown'}] ${message}`, safeData);
    }
  },
  
  /**
   * Log at DEBUG level
   * @param {string} message - Log message
   * @param {string} componentName - Component/module name
   * @param {any} data - Additional data
   */
  debug: (message, componentName, data) => {
    // Only log debug messages if DEBUG_MODE is enabled, or we're in development
    if (DEBUG_MODE || process.env.NODE_ENV === 'development') {
      // Use safe inspection for complex objects
      const safeData = data ? safeInspect(data) : undefined;
      log.debug(`[${componentName || 'unknown'}] ${message}`, safeData);
    }
  },
  
  /**
   * Log at INFO level
   * @param {string} message - Log message
   * @param {string} componentName - Component/module name
   * @param {any} data - Additional data
   */
  info: (message, componentName, data) => {
    log.info(`[${componentName || 'unknown'}] ${message}`, data);
  },
  
  /**
   * Log at WARN level
   * @param {string} message - Log message
   * @param {string} componentName - Component/module name
   * @param {any} data - Additional data
   */
  warn: (message, componentName, data) => {
    log.warn(`[${componentName || 'unknown'}] ${message}`, data);
  },
  
  /**
   * Log at ERROR level
   * @param {string} message - Log message
   * @param {string} componentName - Component/module name
   * @param {any} data - Additional data
   */
  error: (message, componentName, data) => {
    log.error(`[${componentName || 'unknown'}] ${message}`, data);
  },
  
  /**
   * Log for application initialization 
   * @param {string} message - Log message
   * @param {string} componentName - Component/module name
   * @param {any} data - Additional data
   */
  init: (message, componentName, data) => {
    log.info(`[${componentName || 'unknown'}] 🚀 ${message}`, data);
  },
  
  /**
   * Track performance metrics
   * @param {string} componentName - Component/module name
   * @param {string} operation - Operation being measured
   * @param {number} duration - Duration in milliseconds
   */
  performance: (componentName, operation, duration) => {
    // Don't record if we're not at debug level
    if (log.getLevel() > log.levels.DEBUG) return;
    
    // Create component metrics if they don't exist
    if (!performanceMetrics[componentName]) {
      performanceMetrics[componentName] = {};
    }
    
    // Create operation metrics if they don't exist
    if (!performanceMetrics[componentName][operation]) {
      performanceMetrics[componentName][operation] = {
        count: 0,
        total: 0,
        min: Infinity,
        max: 0,
        history: []
      };
    }
    
    const metrics = performanceMetrics[componentName][operation];
    
    // Update metrics
    metrics.count++;
    metrics.total += duration;
    metrics.min = Math.min(metrics.min, duration);
    metrics.max = Math.max(metrics.max, duration);
    
    // Add to history, maintaining maximum size
    metrics.history.push({
      timestamp: Date.now(),
      duration
    });
    
    if (metrics.history.length > MAX_PERFORMANCE_HISTORY) {
      metrics.history.shift();
    }
    
    // Log the performance information
    log.debug(
      `[${componentName}] Performance: ${operation} took ${duration}ms`,
      { avg: Math.round(metrics.total / metrics.count), min: metrics.min, max: metrics.max }
    );
  },
  
  /**
   * Get all collected performance metrics
   * @returns {Object} Performance metrics
   */
  getPerformanceMetrics: () => {
    return { ...performanceMetrics };
  },
  
  /**
   * Log localStorage operations
   * @param {string} operation - Storage operation type ('read'|'write'|'remove'|'clear')
   * @param {string} key - Storage key
   * @param {boolean} success - Whether operation succeeded
   */
  storageOp: (operation, key, success) => {
    // Only log at debug level
    if (log.getLevel() > log.levels.DEBUG) return;
    
    const icon = success ? '💾' : '❌';
    const status = success ? 'succeeded' : 'failed';
    log.debug(`${icon} Storage ${operation} for '${key}' ${status}`);
  },
  
  /**
   * Start a logical grouping of logs
   * @param {string} name - Group name
   * @param {string} componentName - Component/module name
   * @returns {number} Group ID for ending the group
   */
  group: (name, componentName) => {
    const groupId = ++currentGroupId;
    log.info(`[${componentName || 'unknown'}] ▼ BEGIN ${name} (${groupId})`);
    return groupId;
  },
  
  /**
   * End a logical grouping of logs
   * @param {number} groupId - Group ID returned from group()
   * @param {string} name - Group name
   * @param {string} componentName - Component/module name
   */
  groupEnd: (groupId, name, componentName) => {
    log.info(`[${componentName || 'unknown'}] ▲ END ${name} (${groupId})`);
  },
  
  /**
   * Track state changes (use sparingly)
   * @param {string} componentName - Component/module name
   * @param {string} stateName - Name of the state being changed
   * @param {any} prevValue - Previous state value
   * @param {any} newValue - New state value
   */
  stateChange: (componentName, stateName, prevValue, newValue) => {
    // Only log at debug level
    if (log.getLevel() > log.levels.DEBUG) return;
    
    return logger.debug(
      `State '${stateName}' changed`, 
      componentName, 
      { prev: prevValue, new: newValue }
    );
  },
  
  /**
   * Set the logging level
   * @param {string|number} level - Log level (trace|debug|info|warn|error)
   */
  setLevel: (level) => {
    log.setLevel(level);
  },
  
  /**
   * Get the current logging level
   * @returns {number} Current log level
   */
  getLevel: () => log.getLevel(),
  
  /**
   * Get the log level name
   * @returns {string} Current log level name
   */
  getLevelName: () => {
    const level = log.getLevel();
    return Object.keys(log.levels).find(key => log.levels[key] === level) || 'unknown';
  }
};

export default logger;