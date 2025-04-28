/**
 * API Service for Agent C React Client
 * 
 * This service centralizes all API interactions, providing consistent error handling,
 * request formatting, and response processing.
 */

import { API_URL } from '@/config/config';
import logger from './logger';
import eventBus from './eventBus.js';

// Default request timeout in milliseconds
const DEFAULT_TIMEOUT = 30000;

// Default retry configuration
const DEFAULT_RETRY_CONFIG = {
  maxRetries: 3,           // Maximum number of retry attempts
  initialDelay: 300,       // Initial delay in ms before first retry
  maxDelay: 5000,          // Maximum delay between retries
  factor: 2,               // Exponential backoff factor
  jitter: true,            // Add random jitter to delays
  retryOnNetworkError: true,   // Retry on network errors
  retryOn429: true,        // Retry on 429 Too Many Requests
  retryOn5xx: true         // Retry on 5xx Server Errors
};

// API event types for the event bus
export const API_EVENTS = {
  REQUEST_START: 'api:request-start',
  REQUEST_END: 'api:request-end',
  REQUEST_ERROR: 'api:request-error',
  REQUEST_RETRY: 'api:request-retry',
  NETWORK_OFFLINE: 'api:network-offline',
  NETWORK_ONLINE: 'api:network-online',
  SESSION_RECOVERY_START: 'api:session-recovery-start',
  SESSION_RECOVERY_SUCCESS: 'api:session-recovery-success',
  SESSION_RECOVERY_FAILED: 'api:session-recovery-failed'
};

// Error types for better error handling
export class ApiError extends Error {
  constructor(message, status, endpoint, details = {}) {
    super(message);
    this.name = 'ApiError';
    this.status = status;
    this.endpoint = endpoint;
    this.details = details;
  }
}

export class NetworkError extends Error {
  constructor(message, endpoint, originalError = null) {
    super(message);
    this.name = 'NetworkError';
    this.endpoint = endpoint;
    this.originalError = originalError;
  }
}

export class TimeoutError extends Error {
  constructor(endpoint, timeoutMs) {
    super(`Request to ${endpoint} timed out after ${timeoutMs}ms`);
    this.name = 'TimeoutError';
    this.endpoint = endpoint;
    this.timeoutMs = timeoutMs;
  }
}

// Track network status
let isOnline = navigator.onLine;
window.addEventListener('online', () => {
  isOnline = true;
  eventBus.publish(API_EVENTS.NETWORK_ONLINE);
  // Process any queued requests when back online
  processRequestQueue();
});

window.addEventListener('offline', () => {
  isOnline = false;
  eventBus.publish(API_EVENTS.NETWORK_OFFLINE);
});

// Request queue for offline operations
const requestQueue = [];

/**
 * Process the request queue
 */
const processRequestQueue = () => {
  if (!isOnline || requestQueue.length === 0) return;
  
  logger.info(`Processing API request queue: ${requestQueue.length} items`, 'apiService');
  
  // Process queue in FIFO order
  while (requestQueue.length > 0) {
    const request = requestQueue.shift();
    logger.debug('Processing queued request', 'apiService', { endpoint: request.endpoint });
    
    // Execute the request
    request.execute()
      .then(request.resolve)
      .catch(request.reject);
  }
};

/**
 * Add jitter to a delay
 * @param {number} delay - Base delay in ms
 * @returns {number} - Delay with jitter
 */
const addJitter = (delay) => {
  return delay * (0.8 + Math.random() * 0.4); // +/- 20%
};

/**
 * Create an AbortController with timeout
 * @param {number} timeoutMs - Timeout in milliseconds
 * @returns {Object} - AbortController and signal
 */
const createAbortController = (timeoutMs = DEFAULT_TIMEOUT) => {
  const controller = new AbortController();
  const timeoutId = setTimeout(() => controller.abort(), timeoutMs);
  
  return {
    controller,
    signal: controller.signal,
    clearTimeout: () => clearTimeout(timeoutId)
  };
};

/**
 * Process API response with error handling
 * @param {Response} response - Fetch API response
 * @param {string} endpoint - API endpoint for logging
 * @returns {Promise<Object>} Parsed JSON response
 * @throws {ApiError} If response is not OK
 */
const processResponse = async (response, endpoint) => {
  const contentType = response.headers.get('content-type');
  logger.debug(`Response from ${endpoint}`, 'apiService', {
    status: response.status,
    contentType,
    ok: response.ok
  });

  if (!response.ok) {
    // Try to get error details
    let errorDetails;
    try {
      const text = await response.text();
      try {
        errorDetails = JSON.parse(text);
      } catch (e) {
        errorDetails = { message: text };
      }
    } catch (e) {
      errorDetails = { message: 'Could not read error response' };
    }
    
    throw new ApiError(
      `${endpoint} failed: ${response.status} - ${errorDetails.message || 'Unknown error'}`,
      response.status,
      endpoint,
      errorDetails
    );
  }

  // Handle non-JSON responses appropriately
  if (!contentType?.includes('application/json')) {
    if (contentType?.includes('text/')) {
      return { text: await response.text() };
    }
    return { raw: response };
  }

  const text = await response.text();
  logger.debug(`Raw response from ${endpoint}`, 'apiService', { responseText: text });

  try {
    return JSON.parse(text);
  } catch (e) {
    logger.error(`Failed to parse JSON from ${endpoint}`, 'apiService', { text });
    throw new ApiError(`Invalid JSON from ${endpoint}`, response.status, endpoint, { error: e.message, text });
  }
};

/**
 * Check if a request should be retried based on error and configuration
 * @param {Error} error - The error that occurred
 * @param {Object} retryConfig - Retry configuration
 * @param {string} method - HTTP method used
 * @returns {boolean} - Whether the request should be retried
 */
const shouldRetry = (error, retryConfig, method) => {
  // Don't retry if retries are disabled
  if (retryConfig.maxRetries <= 0) {
    return false;
  }
  
  // Only retry idempotent methods by default (GET, HEAD, OPTIONS, etc)
  const isIdempotent = !['POST', 'PUT', 'PATCH', 'DELETE'].includes(method.toUpperCase());
  if (!isIdempotent && !retryConfig.retryNonIdempotent) {
    return false;
  }
  
  // Check specific error types
  if (error instanceof TimeoutError && retryConfig.retryOnTimeout) {
    return true;
  }
  
  if (error instanceof NetworkError && retryConfig.retryOnNetworkError) {
    return true;
  }
  
  if (error instanceof ApiError) {
    // Retry on specific status codes
    if (error.status === 429 && retryConfig.retryOn429) {
      return true;
    }
    
    if (error.status >= 500 && error.status < 600 && retryConfig.retryOn5xx) {
      return true;
    }
    
    // Don't retry on client errors (except those handled above)
    if (error.status >= 400 && error.status < 500) {
      return false;
    }
  }
  
  // By default, don't retry
  return false;
};

/**
 * Calculate delay before next retry attempt
 * @param {number} retryAttempt - Current retry attempt number (0-based)
 * @param {Object} retryConfig - Retry configuration
 * @returns {number} - Delay in milliseconds
 */
const getRetryDelay = (retryAttempt, retryConfig) => {
  // Exponential backoff: initialDelay * factor^retryAttempt
  let delay = retryConfig.initialDelay * Math.pow(retryConfig.factor, retryAttempt);
  
  // Cap at max delay
  delay = Math.min(delay, retryConfig.maxDelay);
  
  // Add jitter if configured
  if (retryConfig.jitter) {
    delay = addJitter(delay);
  }
  
  return delay;
};

/**
 * Fetch with automatic retries, timeout, and error handling
 * @param {string} endpoint - API endpoint
 * @param {Object} options - Fetch options
 * @param {number} timeoutMs - Timeout in milliseconds
 * @param {Object} retryConfig - Retry configuration
 * @returns {Promise<Object>} Processed API response
 */
const fetchWithRetry = async (endpoint, options = {}, timeoutMs = DEFAULT_TIMEOUT, retryConfig = {}) => {
  // Merge with default retry config
  const config = { ...DEFAULT_RETRY_CONFIG, ...retryConfig };
  const method = options.method || 'GET';
  const url = endpoint.startsWith('http') ? endpoint : `${API_URL}${endpoint}`;
  
  logger.debug(`Making request to ${endpoint}`, 'apiService', { method, timeoutMs });
  
  // Publish request start event
  eventBus.publish(API_EVENTS.REQUEST_START, { endpoint, method });
  
  // If we're offline, queue the request instead of making it
  if (!isOnline && config.queueOfflineRequests) {
    logger.info(`Network offline, queueing request to ${endpoint}`, 'apiService');
    
    return new Promise((resolve, reject) => {
      requestQueue.push({
        endpoint,
        execute: () => fetchWithRetry(endpoint, options, timeoutMs, retryConfig),
        resolve,
        reject,
        priority: config.priority || 0,
        timestamp: Date.now()
      });
    });
  }
  
  let currentAttempt = 0;
  const maxAttempts = config.maxRetries + 1; // +1 for the initial attempt
  
  while (currentAttempt < maxAttempts) {
    const { controller, signal, clearTimeout } = createAbortController(timeoutMs);
    const requestOptions = { ...options, signal };
    
    try {
      // If not the first attempt, log retry information
      if (currentAttempt > 0) {
        logger.info(`Retry attempt ${currentAttempt}/${config.maxRetries} for ${endpoint}`, 'apiService');
        eventBus.publish(API_EVENTS.REQUEST_RETRY, { 
          endpoint, 
          attempt: currentAttempt, 
          maxRetries: config.maxRetries 
        });
      }
      
      const startTime = performance.now();
      const response = await fetch(url, requestOptions);
      const duration = performance.now() - startTime;
      
      logger.performance('apiService', `${method} ${endpoint}`, Math.round(duration));
      
      const result = await processResponse(response, endpoint);
      
      // Publish request end event
      eventBus.publish(API_EVENTS.REQUEST_END, { 
        endpoint, 
        method, 
        duration, 
        status: response.status,
        attempt: currentAttempt + 1
      });
      
      return result;
    } catch (error) {
      // Clear timeout to prevent memory leaks
      clearTimeout();
      
      let retryableError;
      
      // Transform error into a more specific type
      if (error.name === 'AbortError') {
        retryableError = new TimeoutError(endpoint, timeoutMs);
      } else if (error instanceof ApiError) {
        retryableError = error;
      } else {
        retryableError = new NetworkError(
          `Network error for ${endpoint}: ${error.message}`,
          endpoint,
          error
        );
      }
      
      // Log the error
      logger.error(
        `Request failed: ${retryableError.message}`, 
        'apiService', 
        { endpoint, attempt: currentAttempt + 1, maxAttempts }
      );
      
      // Check if we should retry
      if (currentAttempt + 1 < maxAttempts && shouldRetry(retryableError, config, method)) {
        // Calculate delay for next retry
        const delay = getRetryDelay(currentAttempt, config);
        logger.debug(`Will retry in ${delay}ms`, 'apiService');
        
        // Wait before the next retry
        await new Promise(resolve => setTimeout(resolve, delay));
        
        // Increment attempt counter
        currentAttempt++;
      } else {
        // No more retries, publish error event
        eventBus.publish(API_EVENTS.REQUEST_ERROR, { 
          endpoint, 
          method, 
          error: retryableError,
          attempts: currentAttempt + 1
        });
        
        // Rethrow the error
        throw retryableError;
      }
    }
  }
  
  // This should not be reached due to the error throwing above
  throw new Error(`Unexpected error in retry loop for ${endpoint}`);
};

/**
 * Recover a broken session
 * @param {string} sessionId - The broken session ID
 * @param {Object} config - Session configuration
 * @returns {Promise<Object>} - New or recovered session
 */
const recoverSession = async (sessionId, config = {}) => {
  logger.warn(`Attempting to recover session ${sessionId}`, 'apiService');
  eventBus.publish(API_EVENTS.SESSION_RECOVERY_START, { sessionId });
  
  try {
    // First try: Attempt to reconnect to existing session
    logger.info(`Attempting to reconnect to session ${sessionId}`, 'apiService');
    
    const reconnectResult = await fetchWithRetry('/initialize', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify({
        ...config,
        ui_session_id: sessionId,
        recovery_attempt: true
      })
    }, 30000, {
      maxRetries: 1, // Only retry once for reconnection
      retryNonIdempotent: true // Allow retry on POST for this specific operation
    });
    
    logger.info('Session reconnection successful', 'apiService');
    eventBus.publish(API_EVENTS.SESSION_RECOVERY_SUCCESS, { 
      sessionId, 
      reconnected: true 
    });
    
    return reconnectResult;
  } catch (error) {
    // Second try: Create a new session if reconnection fails
    logger.warn(
      `Failed to reconnect to session ${sessionId}, creating new session`, 
      'apiService', 
      { error: error.message }
    );
    
    try {
      // Create a new session without referencing the old one
      const newSessionResult = await fetchWithRetry('/initialize', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json'
        },
        body: JSON.stringify({
          ...config,
          // Explicitly exclude old session ID
          create_new_session: true
        })
      }, 30000, {
        maxRetries: 2,
        retryNonIdempotent: true // Allow retry on POST for this specific operation
      });
      
      logger.info('Created new session after recovery failure', 'apiService', { 
        oldSessionId: sessionId, 
        newSessionId: newSessionResult.ui_session_id 
      });
      
      eventBus.publish(API_EVENTS.SESSION_RECOVERY_SUCCESS, { 
        oldSessionId: sessionId, 
        newSessionId: newSessionResult.ui_session_id,
        reconnected: false
      });
      
      return newSessionResult;
    } catch (secondError) {
      // Both reconnection and new session creation failed
      logger.error(
        'Session recovery failed completely', 
        'apiService', 
        { sessionId, error: secondError.message }
      );
      
      eventBus.publish(API_EVENTS.SESSION_RECOVERY_FAILED, { 
        sessionId, 
        error: secondError
      });
      
      throw secondError;
    }
  }
};

/**
 * API Service object with methods for common API operations
 */
const apiService = {
  // Export events for subscription
  events: API_EVENTS,
  
  /**
   * Get the current network status
   * @returns {boolean} - True if online, false if offline
   */
  isOnline: () => isOnline,
  
  /**
   * Subscribe to API events
   * @param {string} eventType - Event type to subscribe to
   * @param {Function} callback - Callback function
   * @returns {Function} - Unsubscribe function
   */
  subscribe: (eventType, callback) => {
    return eventBus.subscribe(eventType, callback);
  },
  
  /**
   * Fetch session information
   * @param {string} sessionId - Session ID
   * @returns {Promise<Object>} Session details
   */
  getSession: (sessionId) => {
    return fetchWithRetry(`/session/${sessionId}`);
  },
  
  /**
   * Initialize a new session
   * @param {Object} config - Session configuration
   * @param {string} [existingSessionId] - Optional existing session ID
   * @returns {Promise<Object>} New session info
   */
  initializeSession: (config, existingSessionId = null) => {
    const body = {
      ...config
    };
    
    if (existingSessionId) {
      body.ui_session_id = existingSessionId;
    }
    
    return fetchWithRetry('/initialize', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify(body)
    }, 45000, { // Longer timeout for initialization
      retryNonIdempotent: true, // Allow retry for initialization
      maxRetries: 2
    }).catch(error => {
      // If initialization fails and we provided an existing session ID,
      // attempt recovery
      if (existingSessionId) {
        logger.warn(
          `Session initialization failed, attempting recovery`, 
          'apiService', 
          { sessionId: existingSessionId, error: error.message }
        );
        return recoverSession(existingSessionId, config);
      }
      throw error;
    });
  },
  
  /**
   * Recover a broken session
   * @param {string} sessionId - The broken session ID
   * @param {Object} config - Session configuration
   * @returns {Promise<Object>} - New or recovered session
   */
  recoverSession,
  
  /**
   * Initialize or reinitialize an agent session
   * @param {Object} config - Agent configuration 
   * @returns {Promise<Object>} Initialized agent session information
   */
  initializeAgent: (config) => {
    logger.info('Initializing agent via API', 'apiService', { hasSessionId: !!config.ui_session_id });
    
    return fetchWithRetry('/initialize', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify(config)
    }, 45000, { // Longer timeout for initialization
      retryNonIdempotent: true, // Allow retry for initialization
      maxRetries: 2
    });
  },
  
  /**
   * Update session settings
   * @param {string} sessionId - Session ID
   * @param {Object} settings - Settings to update
   * @returns {Promise<Object>} Updated settings
   */
  updateSettings: (sessionId, settings) => {
    const payload = {
      ui_session_id: sessionId,
      ...settings
    };
    
    // Add extra detailed logging specifically for model changes
    if (settings.model_name) {
      logger.info('Updating model settings via API', 'apiService', {
        sessionId,
        modelName: settings.model_name,
        backend: settings.backend,
        payloadKeys: Object.keys(payload)
      });
      
      // Add specific handling for MODEL_CHANGE operations
      if (settings.backend) {
        logger.debug('Processing model change operation', 'apiService', {
          sessionId,
          modelName: settings.model_name,
          backend: settings.backend,
          fullPayload: JSON.stringify(payload)
        });
      }
    } else {
      logger.info('Updating settings via API', 'apiService', {
        sessionId,
        settingsKeys: Object.keys(settings),
        hasModelName: !!settings.model_name
      });
    }
    
    // Log the full payload in debug mode
    logger.debug('Settings update payload', 'apiService', JSON.stringify(payload));
    
    return fetchWithRetry('/update_settings', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify(payload)
    }, 30000, {
      retryNonIdempotent: true, // Settings updates can be retried
      maxRetries: 2
    }).then(response => {
      logger.info('Settings updated successfully', 'apiService', {
        statusCode: response.status || 200,
        settingsUpdated: Object.keys(settings),
        modelChanged: !!settings.model_name,
        modelName: settings.model_name || 'not-changed'
      });
      return response;
    }).catch(error => {
      logger.error('Failed to update settings', 'apiService', {
        error: error.message,
        errorStack: error.stack,
        sessionId,
        settingsKeys: Object.keys(settings),
        modelName: settings.model_name || 'not-specified'
      });
      throw error;
    });
  },
  
  /**
   * Update agent tools
   * @param {string} sessionId - Session ID
   * @param {Array<string>} tools - Array of tool names
   * @returns {Promise<Object>} Updated tools status
   */
  updateTools: (sessionId, tools) => {
    return fetchWithRetry('/update_tools', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify({
        ui_session_id: sessionId,
        tools
      })
    }, 30000, {
      retryNonIdempotent: true, // Tool updates can be retried
      maxRetries: 2
    });
  },
  
  /**
   * Get available agent tools
   * @param {string} sessionId - Session ID
   * @returns {Promise<Object>} Tools information
   */
  getAgentTools: (sessionId) => {
    return fetchWithRetry(`/get_agent_tools/${sessionId}`);
  },
  
  /**
   * Get current agent configuration
   * @param {string} sessionId - Session ID
   * @returns {Promise<Object>} Current agent configuration
   */
  getAgentConfig: (sessionId) => {
    return fetchWithRetry(`/agent_config/${sessionId}`);
  },
  
  /**
   * Fetch available models
   * @returns {Promise<Object>} Available models
   */
  getModels: () => {
    return fetchWithRetry('/models');
  },
  
  /**
   * Fetch available personas
   * @returns {Promise<Object>} Available personas
   */
  getPersonas: () => {
    return fetchWithRetry('/personas');
  },
  
  /**
   * Fetch available tools
   * @returns {Promise<Object>} Available tools
   */
  getTools: () => {
    return fetchWithRetry('/tools');
  },
  
  /**
   * Check API health status
   * @returns {Promise<Object>} API health status
   */
  checkHealth: () => {
    return fetchWithRetry('/health', {}, 5000, {
      maxRetries: 0 // Don't retry health checks
    }).catch(error => {
      // Return a standardized response for health checks
      return {
        status: 'error',
        error: error.message,
        timestamp: new Date().toISOString()
      };
    });
  },
  
  /**
   * Send a message to the agent
   * @param {string} sessionId - Session ID
   * @param {string} content - Message content
   * @param {Object} options - Additional options
   * @returns {Promise<Object>} Message response
   */
  sendMessage: (sessionId, content, options = {}) => {
    return fetchWithRetry('/chat', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify({
        ui_session_id: sessionId,
        content,
        ...options
      })
    }, 60000, { // Longer timeout for chat
      retryNonIdempotent: true, // Chat can be retried if idempotency key is used
      maxRetries: options.idempotencyKey ? 2 : 0 // Only retry if idempotency key is provided
    }); 
  },
  
  /**
   * Upload a file
   * @param {string} sessionId - Session ID
   * @param {File} file - File to upload
   * @returns {Promise<Object>} Upload result
   */
  uploadFile: async (sessionId, file) => {
    const formData = new FormData();
    formData.append('file', file);
    formData.append('ui_session_id', sessionId);
    
    return fetchWithRetry('/upload', {
      method: 'POST',
      body: formData
    }, 60000, { // Longer timeout for file uploads
      retryNonIdempotent: true, // File uploads can be retried
      maxRetries: 2
    });
  },
  
  /**
   * Get the current queue length
   * @returns {number} Number of queued requests
   */
  getQueueLength: () => requestQueue.length,
  
  /**
   * Process any queued requests immediately
   * Only processes requests if online
   */
  processQueue: () => processRequestQueue(),
  
  /**
   * Clear the request queue
   * @returns {number} Number of cleared requests
   */
  clearQueue: () => {
    const count = requestQueue.length;
    requestQueue.length = 0;
    return count;
  }
};

export default apiService;