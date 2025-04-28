/**
 * Performance Monitoring Utilities
 * 
 * Lightweight utilities for monitoring performance in development mode only.
 * Has zero impact in production mode.
 */

// Only initialize in development mode
const isDev = process.env.NODE_ENV === 'development';

// Performance metrics store
const metrics = {
  timings: {},
  counts: {},
  renders: new Map()
};

/**
 * Start timing an operation
 * @param {string} label - The operation label
 */
export const startTiming = (label) => {
  if (!isDev) return;
  metrics.timings[label] = performance.now();
};

/**
 * End timing an operation and record result
 * @param {string} label - The operation label
 * @returns {number|null} - The elapsed time in ms, or null in production
 */
export const endTiming = (label) => {
  if (!isDev || !metrics.timings[label]) return null;
  
  const elapsed = performance.now() - metrics.timings[label];
  delete metrics.timings[label];
  return elapsed;
};

/**
 * Increment a counter for operations
 * @param {string} label - The counter label
 * @returns {number|null} - The new count, or null in production
 */
export const incrementCounter = (label) => {
  if (!isDev) return null;
  
  metrics.counts[label] = (metrics.counts[label] || 0) + 1;
  return metrics.counts[label];
};

/**
 * Track component renders
 * @param {string} componentName - The component name
 * @returns {number|null} - The render count, or null in production
 */
export const trackRender = (componentName) => {
  if (!isDev) return null;
  
  const count = metrics.renders.get(componentName) || 0;
  metrics.renders.set(componentName, count + 1);
  return count + 1;
};

/**
 * Get all performance metrics
 * @returns {Object|null} - All metrics, or null in production
 */
export const getMetrics = () => {
  if (!isDev) return null;
  
  return {
    counts: {...metrics.counts},
    renderCounts: Object.fromEntries(metrics.renders),
    activeTimings: {...metrics.timings}
  };
};

/**
 * Reset all metrics
 */
export const resetMetrics = () => {
  if (!isDev) return;
  
  metrics.timings = {};
  metrics.counts = {};
  metrics.renders.clear();
};

/**
 * Measure execution time of a function
 * @param {Function} fn - The function to measure
 * @param {string} label - The operation label
 * @returns {Function} - Wrapped function that reports timing
 */
export const measureExecution = (fn, label) => {
  if (!isDev) return fn;
  
  return (...args) => {
    startTiming(label);
    const result = fn(...args);
    const elapsed = endTiming(label);
    console.debug(`${label} executed in ${elapsed.toFixed(2)}ms`);
    return result;
  };
};

/**
 * Create a hook for component render tracking
 * @param {string} componentName - The component name
 * @returns {Function} - Function to call in component
 */
export const useRenderTracking = (componentName) => {
  if (!isDev) return () => {};
  
  return () => {
    trackRender(componentName);
  };
};

// Expose the API only in development
const performanceMonitor = isDev ? {
  startTiming,
  endTiming,
  incrementCounter,
  trackRender,
  getMetrics,
  resetMetrics,
  measureExecution,
  useRenderTracking
} : {
  // No-op implementations for production
  startTiming: () => {},
  endTiming: () => null,
  incrementCounter: () => null,
  trackRender: () => null,
  getMetrics: () => null,
  resetMetrics: () => {},
  measureExecution: (fn) => fn,
  useRenderTracking: () => () => {}
};

export default performanceMonitor;