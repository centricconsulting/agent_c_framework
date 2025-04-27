/**
 * Debug Mode Toggle Utility
 * 
 * This script has been permanently disabled for performance reasons.
 * Debug functionality should only be available in development builds.
 */

// Debug mode is permanently disabled
export const isDebugMode = false;

// Additional utilities to prevent globals
export const getDebugState = () => false;

// Export an object with all utilities
export default {
  isDebugMode,
  getDebugState
};