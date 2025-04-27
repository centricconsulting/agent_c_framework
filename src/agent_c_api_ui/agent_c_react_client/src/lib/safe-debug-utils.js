/**
 * Safe debugging utilities
 *
 * This module has been temporarily disabled due to performance issues.
 * A properly designed version will be implemented in the future.
 */

// Provide no-op implementations of all functions
export function sanitizeForDevTools(value) {
  return '[Debug disabled]';
}

export function attachSafeDebugData() {
  return false;
}

export function safeConsoleLog() {
  return false;
}

export function getContextSummary() {
  return {};
}

export function getLocalStorageSummary() {
  return {};
}

export function createSafeDebugUtilities() {
  return {
    log: () => {},
    logExpanded: () => {},
    getLocalStorage: () => ({}),
    clearDebugObjects: () => 'Debug disabled',
    checkSessionId: () => ({}),
    checkElementVisibility: () => ({})
  };
}

export default {
  sanitizeForDevTools,
  attachSafeDebugData,
  safeConsoleLog,
  getContextSummary,
  getLocalStorageSummary,
  createSafeDebugUtilities
};