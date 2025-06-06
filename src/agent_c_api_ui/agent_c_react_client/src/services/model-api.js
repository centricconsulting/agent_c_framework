/**
 * Model API Service
 * 
 * Handles all API requests related to models, including:
 * - Retrieving available models
 * - Getting model parameters and configurations
 * - Updating model settings
 */

import api from './api';

/**
 * Get list of available models
 * @returns {Promise<Array>} List of models
 */
export async function getModels() {
  try {
    return await api.get('/models');
  } catch (error) {
    throw api.processApiError(error, 'Failed to retrieve available models');
  }
}

/**
 * Get detailed information about a specific model
 * @param {string} modelId - Model identifier
 * @returns {Promise<Object>} Model details
 */
export async function getModelDetails(modelId) {
  try {
    return await api.get(`/models/${modelId}`);
  } catch (error) {
    throw api.processApiError(error, 'Failed to retrieve model details');
  }
}

/**
 * Get parameter configurations for a model
 * @param {string} modelId - Model identifier
 * @returns {Promise<Object>} Model parameters
 */
export async function getModelParameters(modelId) {
  try {
    return await api.get(`/models/${modelId}/parameters`);
  } catch (error) {
    throw api.processApiError(error, 'Failed to retrieve model parameters');
  }
}

/**
 * Set the model for an existing session
 * @param {string} sessionId - Session identifier
 * @param {string} modelId - Model identifier
 * @param {Object} parameters - Optional model parameters to set
 * @returns {Promise<Object>} Updated session
 */
export async function setSessionModel(sessionId, modelId, parameters = {}) {
  try {
    return await api.put(`/session/${sessionId}/model`, {
      model_id: modelId,
      parameters
    });
  } catch (error) {
    throw api.processApiError(error, 'Failed to set session model');
  }
}

/**
 * Update model parameters for a session
 * @param {string} sessionId - Session identifier
 * @param {Object} parameters - Model parameters to update
 * @returns {Promise<Object>} Updated parameters
 */
export async function updateModelParameters(sessionId, parameters) {
  try {
    // The backend expects the session ID to be part of the payload
    const payload = {
      ui_session_id: sessionId,
      ...parameters
    };
    
    return await api.post('/update_settings', payload);
  } catch (error) {
    throw api.processApiError(error, 'Failed to update model parameters');
  }
}

/**
 * Get default parameters for a model
 * @param {string} modelId - Model identifier
 * @returns {Promise<Object>} Default parameters
 */
export async function getDefaultParameters(modelId) {
  try {
    return await api.get(`/models/${modelId}/default-parameters`);
  } catch (error) {
    throw api.processApiError(error, 'Failed to retrieve default parameters');
  }
}

export default {
  getModels,
  getModelDetails,
  getModelParameters,
  setSessionModel,
  updateModelParameters,
  getDefaultParameters,
};