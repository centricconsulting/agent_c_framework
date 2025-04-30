/**
 * Session API Service
 * 
 * Handles all API requests related to session management, including:
 * - Creating and managing sessions
 * - Session authentication
 * - Session state synchronization
 */

import api from './api';

/**
 * Initialize a session with model and parameters
 * @param {Object} config - Session configuration with model, parameters, persona, etc.
 * @returns {Promise<Object>} Session data including ui_session_id
 */
export async function initialize(config = {}) {
  try {
    return await api.post('/initialize', config);
  } catch (error) {
    throw api.processApiError(error, 'Failed to initialize session');
  }
}

/**
 * Create a new session
 * @param {Object} config - Initial session configuration
 * @returns {Promise<Object>} Session data
 */
export async function createSession(config = {}) {
  try {
    return await api.post('/session', config);
  } catch (error) {
    throw api.processApiError(error, 'Failed to create session');
  }
}

/**
 * Get an existing session by ID
 * @param {string} sessionId - Session identifier
 * @param {Object} options - Options for retrieval
 * @param {boolean} options.silent404 - If true, don't process 404 errors (for validation)
 * @returns {Promise<Object|null>} Session data or null if not found with silent404
 */
export async function getSession(sessionId, options = {}) {
  try {
    return await api.get(`/session/${sessionId}`);
  } catch (error) {
    // For session validation, we want to silently handle 404s
    const statusCode = error.statusCode || (error.originalError && error.originalError.statusCode);
    if (options.silent404 && (statusCode === 404 || (error.message && error.message.includes('status 404')))) {
      return null;
    }
    // For non-validation calls, or for other errors, process normally
    throw api.processApiError(error, 'Failed to retrieve session', { silent: options.silent404 });
  }
}

/**
 * Update session configuration
 * @param {string} sessionId - Session identifier
 * @param {Object} config - Updated session configuration
 * @returns {Promise<Object>} Updated session data
 */
export async function updateSession(sessionId, config) {
  try {
    // According to API docs, the correct endpoint for updating settings is '/update_settings'
    return await api.post('/update_settings', {
      ...config,
      ui_session_id: sessionId
    });
  } catch (error) {
    throw api.processApiError(error, 'Failed to update session');
  }
}

/**
 * Delete a session
 * @param {string} sessionId - Session identifier
 * @returns {Promise<void>}
 */
export async function deleteSession(sessionId) {
  try {
    return await api.delete(`/session/${sessionId}`);
  } catch (error) {
    throw api.processApiError(error, 'Failed to delete session');
  }
}

/**
 * Get available session templates
 * @returns {Promise<Array>} List of session templates
 */
export async function getSessionTemplates() {
  try {
    return await api.get('/session/templates');
  } catch (error) {
    throw api.processApiError(error, 'Failed to retrieve session templates');
  }
}

/**
 * Send a message to the session
 * @param {string} sessionId - Session identifier
 * @param {Object} message - Message to send (includes content, files, etc.)
 * @returns {Promise<Object>} Message response
 */
export async function sendMessage(sessionId, message) {
  try {
    return await api.post(`/session/${sessionId}/message`, message);
  } catch (error) {
    throw api.processApiError(error, 'Failed to send message');
  }
}

/**
 * Get message history for a session
 * @param {string} sessionId - Session identifier
 * @param {Object} params - Query parameters (limit, offset, etc.)
 * @returns {Promise<Array>} Message history
 */
export async function getMessages(sessionId, params = {}) {
  try {
    return await api.get(`/session/${sessionId}/messages`, { params });
  } catch (error) {
    throw api.processApiError(error, 'Failed to retrieve messages');
  }
}

/**
 * Get session UI state
 * @param {string} sessionId - Session identifier
 * @returns {Promise<Object>} UI state
 */
export async function getSessionUIState(sessionId) {
  try {
    return await api.get(`/session/${sessionId}/ui-state`);
  } catch (error) {
    throw api.processApiError(error, 'Failed to retrieve UI state');
  }
}

/**
 * Update session UI state
 * @param {string} sessionId - Session identifier
 * @param {Object} uiState - Updated UI state
 * @returns {Promise<Object>} Updated UI state
 */
export async function updateSessionUIState(sessionId, uiState) {
  try {
    return await api.put(`/session/${sessionId}/ui-state`, uiState);
  } catch (error) {
    throw api.processApiError(error, 'Failed to update UI state');
  }
}

/**
 * Stop the current session generation/stream
 * @param {string} sessionId - Session identifier
 * @returns {Promise<Object>} Result
 */
export async function stopGeneration(sessionId) {
  try {
    return await api.post(`/session/${sessionId}/stop`);
  } catch (error) {
    throw api.processApiError(error, 'Failed to stop generation');
  }
}

/**
 * Export session data (messages, config, etc.)
 * @param {string} sessionId - Session identifier
 * @param {string} format - Export format (json, html, txt)
 * @returns {Promise<Blob>} Exported data as a file
 */
export async function exportSession(sessionId, format = 'json') {
  try {
    return await api.downloadFile(`/session/${sessionId}/export?format=${format}`);
  } catch (error) {
    throw api.processApiError(error, 'Failed to export session');
  }
}

export default {
  initialize,
  createSession,
  getSession,
  updateSession,
  deleteSession,
  getSessionTemplates,
  sendMessage,
  getMessages,
  getSessionUIState,
  updateSessionUIState,
  stopGeneration,
  exportSession,
};