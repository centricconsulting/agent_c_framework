/**
 * Global authentication helper for API integration.
 * 
 * This module creates a global interface for accessing authentication tokens
 * that can be used by various services without direct React context dependencies.
 * 
 * Enhanced with token refresh management for improved reliability.
 */

import { TokenManager } from './token-manager';

// Create a global namespace for Agent C if it doesn't exist
if (typeof window !== 'undefined') {
  window._agentC = window._agentC || {};
  
  // Initialize the auth interface
  window._agentC.auth = {
    // Default implementation returns null (no token)
    getToken: async () => null,
    
    // Reference to token manager instance
    tokenManager: null,
    
    // Method to be called by AuthContext to register the real token provider
    registerTokenProvider: (tokenProviderFn, config = {}) => {
      // Clean up existing token manager if present
      if (window._agentC.auth.tokenManager) {
        window._agentC.auth.tokenManager.dispose();
      }
      
      // Create a new token manager with the provider function
      if (tokenProviderFn) {
        window._agentC.auth.tokenManager = new TokenManager(tokenProviderFn, config);
        window._agentC.auth.tokenManager.initialize();
        
        // Set up token getter to use the token manager
        window._agentC.auth.getToken = async () => {
          if (window._agentC.auth.tokenManager) {
            return window._agentC.auth.tokenManager.getToken();
          }
          return null;
        };
      } else {
        // Reset to default when no provider is supplied
        window._agentC.auth.tokenManager = null;
        window._agentC.auth.getToken = async () => null;
      }
    }
  };
}

/**
 * Register a token provider function
 * @param {Function} tokenProviderFn - Async function that returns an authentication token
 * @param {Object} config - Configuration options for token refresh
 */
export function registerTokenProvider(tokenProviderFn, config = {}) {
  if (typeof window !== 'undefined') {
    window._agentC = window._agentC || {};
    window._agentC.auth = window._agentC.auth || {};
    
    if (window._agentC.auth.registerTokenProvider) {
      window._agentC.auth.registerTokenProvider(tokenProviderFn, config);
    } else {
      // Fallback if full auth interface not initialized
      window._agentC.auth.getToken = tokenProviderFn;
    }
  }
}

/**
 * Get the current authentication token
 * @returns {Promise<string|null>} Authentication token or null if not available
 */
export async function getAuthToken() {
  if (typeof window !== 'undefined' && window._agentC?.auth?.getToken) {
    return await window._agentC.auth.getToken();
  }
  return null;
}

/**
 * Add authentication token to request headers
 * @param {Object} headers - Existing headers object
 * @returns {Promise<Object>} Headers with authentication token added
 */
export async function addAuthHeader(headers = {}) {
  const token = await getAuthToken();
  if (token) {
    return {
      ...headers,
      'Authorization': `Bearer ${token}`
    };
  }
  return headers;
}

/**
 * Get the current token manager instance
 * @returns {TokenManager|null} The token manager or null if not available
 */
export function getTokenManager() {
  if (typeof window !== 'undefined' && window._agentC?.auth?.tokenManager) {
    return window._agentC.auth.tokenManager;
  }
  return null;
}

/**
 * Force an immediate token refresh
 * @returns {Promise<string|null>} The refreshed token or null
 */
export async function refreshAuthToken() {
  const tokenManager = getTokenManager();
  if (tokenManager) {
    return await tokenManager.refreshToken();
  }
  return null;
}

export default {
  registerTokenProvider,
  getAuthToken,
  addAuthHeader,
  getTokenManager,
  refreshAuthToken
};