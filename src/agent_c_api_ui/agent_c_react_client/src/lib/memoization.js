/**
 * This utility module provides memoization helpers to optimize React component performance
 * and prevent unnecessary re-renders throughout the application.
 */

/**
 * Performs a shallow equality check between objects
 * @param {Object} objA - First object to compare
 * @param {Object} objB - Second object to compare
 * @returns {boolean} - True if objects are shallowly equal
 */
export const shallowEqual = (objA, objB) => {
  if (objA === objB) return true;
  if (!objA || !objB) return false;
  
  const keysA = Object.keys(objA);
  const keysB = Object.keys(objB);
  
  if (keysA.length !== keysB.length) return false;
  
  for (let i = 0; i < keysA.length; i++) {
    const key = keysA[i];
    if (objA[key] !== objB[key]) return false;
  }
  
  return true;
};

/**
 * Performs a shallow equality check on arrays
 * @param {Array} arrA - First array to compare
 * @param {Array} arrB - Second array to compare
 * @returns {boolean} - True if arrays are shallowly equal
 */
export const arrayShallowEqual = (arrA, arrB) => {
  if (arrA === arrB) return true;
  if (!arrA || !arrB) return false;
  if (arrA.length !== arrB.length) return false;
  
  for (let i = 0; i < arrA.length; i++) {
    if (arrA[i] !== arrB[i]) return false;
  }
  
  return true;
};

/**
 * Compares two arrays of objects by using the provided key to get unique identifiers
 * @param {Array} arrA - First array to compare
 * @param {Array} arrB - Second array to compare
 * @param {string|Function} key - Property name to use as ID or function to extract ID
 * @returns {boolean} - True if arrays contain the same objects based on ID
 */
export const arrayEqualByKey = (arrA, arrB, key) => {
  if (arrA === arrB) return true;
  if (!arrA || !arrB) return false;
  if (arrA.length !== arrB.length) return false;
  
  // Create maps for faster lookup
  const mapA = new Map();
  const getKey = typeof key === 'function' ? key : (item) => item[key];
  
  // Populate map A with keys from array A
  for (const item of arrA) {
    const itemKey = getKey(item);
    mapA.set(itemKey, item);
  }
  
  // Check each item in B against map A
  for (const item of arrB) {
    const itemKey = getKey(item);
    if (!mapA.has(itemKey)) return false;
    
    // Optional deeper comparison can be added here if needed
  }
  
  return true;
};

/**
 * Creates a debounced version of a function
 * @param {Function} fn - Function to debounce
 * @param {number} wait - Wait time in ms
 * @returns {Function} - Debounced function
 */
export const debounce = (fn, wait = 100) => {
  let timeout;
  return function(...args) {
    clearTimeout(timeout);
    timeout = setTimeout(() => fn.apply(this, args), wait);
  };
};

/**
 * Creates a throttled version of a function
 * @param {Function} fn - Function to throttle
 * @param {number} limit - Throttle time in ms
 * @returns {Function} - Throttled function
 */
export const throttle = (fn, limit = 100) => {
  let inThrottle;
  return function(...args) {
    if (!inThrottle) {
      fn.apply(this, args);
      inThrottle = true;
      setTimeout(() => inThrottle = false, limit);
    }
  };
};

/**
 * For development use - logs a component's render with props for debugging
 * Only works in development mode, becomes a no-op in production
 * @param {string} componentName - Name of the component
 * @param {Object} props - Component props
 */
export const logRender = (componentName, props) => {
  if (process.env.NODE_ENV === 'development') {
    console.log(`[RENDER] ${componentName}`, props);
  }
};

/**
 * Debug utility to track which prop changes are causing re-renders
 * Only works in development mode, becomes a no-op in production
 * @param {string} componentName - Name of the component
 * @param {Object} prevProps - Previous props
 * @param {Object} nextProps - Next props
 */
export const debugPropChanges = (componentName, prevProps, nextProps) => {
  if (process.env.NODE_ENV === 'development') {
    const allKeys = new Set([...Object.keys(prevProps), ...Object.keys(nextProps)]);
    const changedProps = {};
    
    allKeys.forEach(key => {
      if (prevProps[key] !== nextProps[key]) {
        changedProps[key] = {
          from: prevProps[key],
          to: nextProps[key]
        };
      }
    });
    
    if (Object.keys(changedProps).length > 0) {
      console.log(`[PROPS CHANGED] ${componentName}:`, changedProps);
    }
  }
};