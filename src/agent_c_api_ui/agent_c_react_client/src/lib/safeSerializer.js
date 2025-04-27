/**
 * Safe serialization utilities to prevent circular references and memory issues
 * when logging complex objects to the console or storing in state.
 */

/**
 * Maximum depth to traverse when serializing objects
 */
const MAX_DEPTH = 3;

/**
 * Maximum number of elements to include when serializing arrays
 */
const MAX_ARRAY_LENGTH = 50;

/**
 * Maximum length for string values in objects
 */
const MAX_STRING_LENGTH = 500;

/**
 * Convert a value to a safe string representation
 * @param {any} value - Value to convert
 * @param {number} depth - Current depth in the object
 * @param {Set} visited - Set of already visited objects to prevent circular references
 * @returns {string} Safe string representation of the value
 */
export function toSafeString(value, depth = 0, visited = new Set()) {
  // Handle null and undefined
  if (value === null) return 'null';
  if (value === undefined) return 'undefined';

  // Handle primitives
  if (typeof value === 'string') {
    if (value.length > MAX_STRING_LENGTH) {
      return `"${value.substring(0, MAX_STRING_LENGTH)}..."`;
    }
    return `"${value}"`;
  }
  if (typeof value === 'number') return value.toString();
  if (typeof value === 'boolean') return value.toString();
  if (typeof value === 'function') return '[Function]';
  if (typeof value === 'symbol') return value.toString();
  if (typeof value === 'bigint') return value.toString() + 'n';

  // Check for circular references
  if (depth > 0 && visited.has(value)) return '[Circular]';

  // Track this object to prevent circular references
  if (typeof value === 'object') {
    visited.add(value);
  }

  // Maximum depth reached
  if (depth >= MAX_DEPTH) return '[Object]';

  // Handle arrays
  if (Array.isArray(value)) {
    if (value.length === 0) return '[]';
    
    // Limit array length
    const length = Math.min(value.length, MAX_ARRAY_LENGTH);
    const items = [];
    
    for (let i = 0; i < length; i++) {
      items.push(toSafeString(value[i], depth + 1, visited));
    }
    
    if (value.length > MAX_ARRAY_LENGTH) {
      items.push(`...${value.length - MAX_ARRAY_LENGTH} more items`);
    }
    
    return `[${items.join(', ')}]`;
  }

  // Handle objects
  if (value instanceof Date) return value.toISOString();
  if (value instanceof RegExp) return value.toString();
  if (value instanceof Error) {
    return `[${value.name}: ${value.message}]`;
  }
  if (value instanceof Map) {
    return `Map(${value.size})`;
  }
  if (value instanceof Set) {
    return `Set(${value.size})`;
  }

  // Handle plain objects
  try {
    const entries = Object.entries(value);
    if (entries.length === 0) return '{}';
    
    const props = [];
    for (const [key, val] of entries) {
      props.push(`${key}: ${toSafeString(val, depth + 1, visited)}`);
      if (props.length >= 10) {
        props.push(`...${entries.length - 10} more properties`);
        break;
      }
    }
    
    return `{${props.join(', ')}}`;
  } catch (e) {
    return '[Object]';
  }
}

/**
 * Safely convert an object to a string for console logging
 * @param {any} obj - The object to stringify
 * @returns {string} A safe string representation
 */
export function safeStringify(obj) {
  return toSafeString(obj);
}

/**
 * Safely inspect an object for debugging
 * Limits depth, handles circular references, and truncates large values
 * @param {any} obj - The object to inspect
 * @returns {Object} A safe version of the object for inspection
 */
export function safeInspect(obj) {
  if (!obj || typeof obj !== 'object') return obj;
  
  const visited = new Set();
  const inspect = (value, depth = 0) => {
    // Handle null or primitive values
    if (value === null || typeof value !== 'object') {
      if (typeof value === 'string' && value.length > MAX_STRING_LENGTH) {
        return `${value.substring(0, MAX_STRING_LENGTH)}... (truncated)`;
      }
      return value;
    }
    
    // Check for circular references
    if (visited.has(value)) return '[Circular]';
    visited.add(value);
    
    // Maximum depth reached
    if (depth >= MAX_DEPTH) return '[Object]';
    
    // Handle arrays
    if (Array.isArray(value)) {
      if (value.length === 0) return [];
      
      const length = Math.min(value.length, MAX_ARRAY_LENGTH);
      const result = [];
      
      for (let i = 0; i < length; i++) {
        result.push(inspect(value[i], depth + 1));
      }
      
      if (value.length > MAX_ARRAY_LENGTH) {
        result.push(`...${value.length - MAX_ARRAY_LENGTH} more items`);
      }
      
      return result;
    }
    
    // Handle special objects
    if (value instanceof Date) return value.toISOString();
    if (value instanceof RegExp) return value.toString();
    if (value instanceof Error) {
      return {
        name: value.name,
        message: value.message,
        stack: value.stack ? value.stack.split('\n').slice(0, 3).join('\n') : undefined
      };
    }
    
    // Handle plain objects
    const result = {};
    const entries = Object.entries(value);
    let count = 0;
    
    for (const [key, val] of entries) {
      if (count >= 20) {
        result['...'] = `${entries.length - count} more properties`;
        break;
      }
      result[key] = inspect(val, depth + 1);
      count++;
    }
    
    return result;
  };
  
  return inspect(obj);
}

/**
 * Create a debug logger that safely handles objects with circular references
 * @param {string} namespace - The namespace for the logger
 * @returns {Object} A set of logging functions
 */
export function createSafeLogger(namespace = 'app') {
  return {
    debug: (message, data) => {
      console.debug(`[${namespace}] ${message}`, data ? safeInspect(data) : '');
    },
    log: (message, data) => {
      console.log(`[${namespace}] ${message}`, data ? safeInspect(data) : '');
    },
    info: (message, data) => {
      console.info(`[${namespace}] ${message}`, data ? safeInspect(data) : '');
    },
    warn: (message, data) => {
      console.warn(`[${namespace}] ${message}`, data ? safeInspect(data) : '');
    },
    error: (message, data) => {
      console.error(`[${namespace}] ${message}`, data ? safeInspect(data) : '');
    }
  };
}

export default {
  safeStringify,
  safeInspect,
  createSafeLogger
};