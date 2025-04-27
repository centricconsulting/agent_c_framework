/**
 * Diagnostics utilities for Agent C React Client
 *
 * This module has been temporarily disabled due to performance issues.
 * A properly designed version will be implemented in the future.
 */

import logger from './logger';

// Force debug mode to be false
// This must remain false for production to prevent console logging and performance issues
export const DEBUG_MODE = process.env.NODE_ENV === 'development' && false;

// Minimal implementation of context tracking
const contextStats = {};

/**
 * Track context initialization events
 * @param {string} contextName - The name of the context being tracked
 * @param {string} status - The status of initialization
 * @param {Object} data - Additional data about the initialization
 */
export const trackContextInitialization = (contextName, status, data) => {
  if (!contextStats[contextName]) {
    contextStats[contextName] = {
      status: 'unknown',
      timestamps: {},
      data: {}
    };
  }
  
  const context = contextStats[contextName];
  context.status = status;
  context.timestamps[status] = Date.now();
  context.data = { ...context.data, ...(data || {}) };
  
  // Log only in development mode and when debug is enabled
  if (process.env.NODE_ENV !== 'production' && DEBUG_MODE) {
    logger.debug(`Context ${contextName}: ${status}`, 'diagnostic', data);
  }
};

/**
 * Mark a context as completely initialized
 * @param {string} contextName - The name of the context
 * @param {Object} data - Additional data about completion
 */
export const completeContextInitialization = (contextName, data) => {
  trackContextInitialization(contextName, 'complete', data);
};

/**
 * Get the current status of all context initializations
 * @returns {Object} - Status of all tracked contexts
 */
export const getContextInitializationStatus = () => {
  return { ...contextStats };
};
/**
 * Track component rendering events
 * This implementation is intentionally a no-op to prevent performance issues
 * @param {string} phase - The rendering phase (mount, update, unmount)
 * @param {string} componentName - The name of the component being tracked
 * @param {Object} data - Additional data about the rendering
 */
export const trackComponentRendering = (phase, componentName, data) => {
  // No-op implementation for performance reasons
  if (process.env.NODE_ENV !== 'production' && DEBUG_MODE) {
    logger.debug(`Component ${phase}: ${componentName}`, 'diagnostic', data);
  }
};
/**
 * Track chat interface rendering events
 * This implementation is intentionally a no-op to prevent performance issues
 * @param {string} phase - The rendering phase (mount, update, unmount)
 * @param {Object} data - Additional data about the rendering
 */
export const trackChatInterfaceRendering = (phase, data) => {
  // No-op implementation for performance reasons
  if (process.env.NODE_ENV !== 'production' && DEBUG_MODE) {
    logger.debug(`ChatInterface ${phase}`, 'diagnostic', data);
  }
};

export default {
  DEBUG_MODE,
  trackContextInitialization,
  completeContextInitialization,
  getContextInitializationStatus,
  trackComponentRendering,
  trackChatInterfaceRendering
};