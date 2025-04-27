/**
 * Diagnostics utilities for Agent C React Client
 *
 * This module has been completely disabled for all environments to prevent performance issues.
 * All functions are now pure no-ops.
 */

// Ensure debug mode is always false
export const DEBUG_MODE = false;

// Empty object to avoid errors when accessing
const contextStats = {};

/**
 * Track context initialization events
 * This is now a complete no-op function
 */
export const trackContextInitialization = () => {
  // No-op implementation
};

/**
 * Mark a context as completely initialized
 * This is now a complete no-op function
 */
export const completeContextInitialization = () => {
  // No-op implementation
};

/**
 * Get the current status of all context initializations
 * Returns empty object to prevent errors
 */
export const getContextInitializationStatus = () => {
  return {};
};

/**
 * Track component rendering events
 * This is now a complete no-op function
 */
export const trackComponentRendering = () => {
  // No-op implementation
};

/**
 * Track chat interface rendering events
 * This is now a complete no-op function
 */
export const trackChatInterfaceRendering = () => {
  // No-op implementation
};

export default {
  DEBUG_MODE,
  trackContextInitialization,
  completeContextInitialization,
  getContextInitializationStatus,
  trackComponentRendering,
  trackChatInterfaceRendering
};
