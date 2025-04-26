/**
 * Simple logging utility based on loglevel
 * Much more conservative implementation that won't flood the console
 */

import log from 'loglevel';

// Set default level based on environment
const defaultLevel = process.env.NODE_ENV === 'production' ? 'warn' : 'info';
log.setLevel(defaultLevel);

// Enhanced logger with component tracking
const logger = {
  trace: (message, componentName, data) => {
    log.trace(`[${componentName || 'unknown'}] ${message}`, data);
  },
  debug: (message, componentName, data) => {
    log.debug(`[${componentName || 'unknown'}] ${message}`, data);
  },
  info: (message, componentName, data) => {
    log.info(`[${componentName || 'unknown'}] ${message}`, data);
  },
  warn: (message, componentName, data) => {
    log.warn(`[${componentName || 'unknown'}] ${message}`, data);
  },
  error: (message, componentName, data) => {
    log.error(`[${componentName || 'unknown'}] ${message}`, data);
  },
  
  // State change tracking helper (use sparingly)
  stateChange: (componentName, stateName, prevValue, newValue) => {
    return logger.debug(
      `State '${stateName}' changed`, 
      componentName, 
      { prev: prevValue, new: newValue }
    );
  },
  
  // Configuration
  setLevel: (level) => {
    log.setLevel(level);
  },
  getLevel: () => log.getLevel()
};

export default logger;