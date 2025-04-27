/**
 * Safe serialization utilities
 *
 * This module provides minimal implementations to prevent any application errors,
 * but has been stripped of functionality that could cause performance issues.
 */

// Simple implementation that doesn't cause issues
export function toSafeString(value) {
  if (value === null) return 'null';
  if (value === undefined) return 'undefined';
  
  try {
    if (typeof value === 'object') {
      return '[Object]';
    }
    return String(value);
  } catch (e) {
    return '[Error]';
  }
}

export function safeStringify(obj) {
  try {
    return typeof obj === 'object' ? '[Object]' : String(obj);
  } catch (e) {
    return '[Error]';
  }
}

export function safeInspect(obj) {
  return obj;
}

export function createSafeLogger(namespace = 'app') {
  // Minimal implementation that doesn't cause issues
  return {
    debug: () => {},
    log: () => {},
    info: () => {},
    warn: () => {},
    error: () => {}
  };
}

export default {
  safeStringify,
  safeInspect,
  createSafeLogger
};