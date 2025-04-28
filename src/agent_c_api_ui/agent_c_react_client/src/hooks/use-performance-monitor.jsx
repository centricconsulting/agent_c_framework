/**
 * usePerformanceMonitor Hook
 * 
 * A React hook for component-level performance tracking.
 * No-op in production mode for zero performance impact.
 */

import { useEffect, useRef } from 'react';
import performanceMonitor from '../lib/performance-monitor';

/**
 * Hook for monitoring component performance
 * @param {string} componentName - The name of the component being monitored
 * @param {Object} options - Configuration options
 * @param {boolean} options.trackRender - Whether to track renders (default: true)
 * @param {boolean} options.measureLifecycle - Whether to measure lifecycle methods (default: false)
 * @param {boolean} options.debugMount - Whether to console.debug mount timing (default: false)
 * @returns {Object} - Performance monitoring helpers
 */
const usePerformanceMonitor = (componentName, options = {}) => {
  // Default options
  const {
    trackRender = true,
    measureLifecycle = false,
    debugMount = false
  } = options;
  
  // Track render count
  if (trackRender && process.env.NODE_ENV === 'development') {
    performanceMonitor.trackRender(componentName);
  }
  
  // Reference to track mount time
  const mountTimeRef = useRef(null);
  
  // Measure mount time
  useEffect(() => {
    if (process.env.NODE_ENV !== 'development' || !measureLifecycle) return;
    
    // End timing for mount
    const mountTime = performanceMonitor.endTiming(`${componentName}_mount`);
    
    if (mountTime && debugMount) {
      console.debug(`${componentName} mounted in ${mountTime.toFixed(2)}ms`);
    }
    
    // Start timing for lifetime
    performanceMonitor.startTiming(`${componentName}_lifetime`);
    
    return () => {
      // Measure component lifetime on unmount
      const lifetime = performanceMonitor.endTiming(`${componentName}_lifetime`);
      if (lifetime && debugMount) {
        console.debug(`${componentName} lifetime: ${lifetime.toFixed(2)}ms`);
      }
    };
  }, [componentName, measureLifecycle, debugMount]);
  
  // Start mount timing on first render
  if (process.env.NODE_ENV === 'development' && measureLifecycle && !mountTimeRef.current) {
    mountTimeRef.current = true;
    performanceMonitor.startTiming(`${componentName}_mount`);
  }
  
  /**
   * Measure the execution time of a callback
   * @param {Function} callback - The callback to measure
   * @param {string} label - The operation label
   * @returns {Function} - The wrapped callback
   */
  const measureCallback = (callback, label) => {
    if (process.env.NODE_ENV !== 'development') return callback;
    return performanceMonitor.measureExecution(callback, `${componentName}_${label}`);
  };
  
  /**
   * Start timing an operation
   * @param {string} label - The operation label
   */
  const startMeasure = (label) => {
    performanceMonitor.startTiming(`${componentName}_${label}`);
  };
  
  /**
   * End timing an operation
   * @param {string} label - The operation label
   * @returns {number|null} - The elapsed time or null in production
   */
  const endMeasure = (label) => {
    return performanceMonitor.endTiming(`${componentName}_${label}`);
  };
  
  return {
    measureCallback,
    startMeasure,
    endMeasure
  };
};

export default usePerformanceMonitor;