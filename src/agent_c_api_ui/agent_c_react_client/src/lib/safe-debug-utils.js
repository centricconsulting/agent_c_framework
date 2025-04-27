/**
 * Safe debugging utilities - Performance optimized version
 *
 * This module has been completely disabled for performance reasons.
 * All exports are simple no-op functions that do absolutely nothing.
 */

// Simple no-op implementations that return fixed values
const DISABLED_MESSAGE = '[Debug disabled]';
const EMPTY_OBJECT = {};
const FALSE_VALUE = false;
const NO_OP = () => {};

// Export all functions as complete no-ops
export const sanitizeForDevTools = () => DISABLED_MESSAGE;
export const attachSafeDebugData = () => FALSE_VALUE;
export const safeConsoleLog = () => FALSE_VALUE;
export const getContextSummary = () => EMPTY_OBJECT;
export const getLocalStorageSummary = () => EMPTY_OBJECT;

// Return object with all no-op methods
export const createSafeDebugUtilities = () => ({
  log: NO_OP,
  logExpanded: NO_OP,
  getLocalStorage: () => EMPTY_OBJECT,
  clearDebugObjects: () => DISABLED_MESSAGE,
  checkSessionId: () => EMPTY_OBJECT,
  checkElementVisibility: () => EMPTY_OBJECT
});

// Export all functions as a single object
export default {
  sanitizeForDevTools,
  attachSafeDebugData,
  safeConsoleLog,
  getContextSummary,
  getLocalStorageSummary,
  createSafeDebugUtilities
};