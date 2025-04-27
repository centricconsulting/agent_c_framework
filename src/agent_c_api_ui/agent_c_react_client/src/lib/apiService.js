/**
 * API Service for Agent C React Client
 * 
 * This service centralizes all API interactions, providing consistent error handling,
 * request formatting, and response processing.
 */

import { API_URL } from '@/config/config';
import logger from './logger';

// Default request timeout in milliseconds
const DEFAULT_TIMEOUT = 30000;

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
 * @throws {Error} If response is not OK or not JSON
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
    let errorText;
    try {
      errorText = await response.text();
    } catch (e) {
      errorText = 'Could not read error response';
    }
    throw new Error(`${endpoint} failed: ${response.status} - ${errorText}`);
  }

  if (!contentType?.includes('application/json')) {
    throw new Error(`${endpoint} returned non-JSON content-type: ${contentType}`);
  }

  const text = await response.text();
  logger.debug(`Raw response from ${endpoint}`, 'apiService', { responseText: text });

  try {
    return JSON.parse(text);
  } catch (e) {
    logger.error(`Failed to parse JSON from ${endpoint}`, 'apiService', { text });
    throw new Error(`Invalid JSON from ${endpoint}: ${e.message}`);
  }
};

/**
 * Fetch with automatic timeout and error handling
 * @param {string} endpoint - API endpoint
 * @param {Object} options - Fetch options
 * @param {number} timeoutMs - Timeout in milliseconds
 * @returns {Promise<Object>} Processed API response
 */
const fetchWithTimeout = async (endpoint, options = {}, timeoutMs = DEFAULT_TIMEOUT) => {
  const url = endpoint.startsWith('http') ? endpoint : `${API_URL}${endpoint}`;
  const { controller, signal, clearTimeout } = createAbortController(timeoutMs);
  
  try {
    const startTime = performance.now();
    const response = await fetch(url, { ...options, signal });
    const duration = performance.now() - startTime;
    
    logger.performance('apiService', `${options.method || 'GET'} ${endpoint}`, Math.round(duration));
    
    return await processResponse(response, endpoint);
  } catch (error) {
    if (error.name === 'AbortError') {
      throw new Error(`Request to ${endpoint} timed out after ${timeoutMs}ms`);
    }
    throw error;
  } finally {
    clearTimeout();
  }
};

/**
 * API Service object with methods for common API operations
 */
const apiService = {
  /**
   * Fetch session information
   * @param {string} sessionId - Session ID
   * @returns {Promise<Object>} Session details
   */
  getSession: (sessionId) => {
    return fetchWithTimeout(`/session/${sessionId}`);
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
    
    return fetchWithTimeout('/initialize', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify(body)
    });
  },
  
  /**
   * Initialize or reinitialize an agent session
   * @param {Object} config - Agent configuration 
   * @returns {Promise<Object>} Initialized agent session information
   */
  initializeAgent: (config) => {
    logger.info('Initializing agent via API', 'apiService', { hasSessionId: !!config.ui_session_id });
    
    return fetchWithTimeout('/initialize', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify(config)
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
    
    return fetchWithTimeout('/update_settings', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify(payload)
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
    return fetchWithTimeout('/update_tools', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify({
        ui_session_id: sessionId,
        tools
      })
    });
  },
  
  /**
   * Get available agent tools
   * @param {string} sessionId - Session ID
   * @returns {Promise<Object>} Tools information
   */
  getAgentTools: (sessionId) => {
    return fetchWithTimeout(`/get_agent_tools/${sessionId}`);
  },
  
  /**
   * Fetch available models
   * @returns {Promise<Object>} Available models
   */
  getModels: () => {
    return fetchWithTimeout('/models');
  },
  
  /**
   * Fetch available personas
   * @returns {Promise<Object>} Available personas
   */
  getPersonas: () => {
    return fetchWithTimeout('/personas');
  },
  
  /**
   * Fetch available tools
   * @returns {Promise<Object>} Available tools
   */
  getTools: () => {
    return fetchWithTimeout('/tools');
  },
  
  /**
   * Send a message to the agent
   * @param {string} sessionId - Session ID
   * @param {string} content - Message content
   * @param {Object} options - Additional options
   * @returns {Promise<Object>} Message response
   */
  sendMessage: (sessionId, content, options = {}) => {
    return fetchWithTimeout('/chat', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify({
        ui_session_id: sessionId,
        content,
        ...options
      })
    }, 60000); // Longer timeout for chat
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
    
    return fetchWithTimeout('/upload', {
      method: 'POST',
      body: formData
    });
  }
};

export default apiService;