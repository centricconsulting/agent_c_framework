/**
 * Global authentication helper for API integration.
 * 
 * This module creates a global interface for accessing authentication tokens
 * that can be used by various services without direct React context dependencies.
 */

// Create a global namespace for Agent C if it doesn't exist
if (typeof window !== 'undefined') {
  window._agentC = window._agentC || {};
  
  // Initialize the auth interface
  window._agentC.auth = {
    // Default implementation returns null (no token)
    getToken: async () => null,
    
    // Method to be called by AuthContext to register the real token provider
    registerTokenProvider: (tokenProviderFn) => {
      window._agentC.auth.getToken = tokenProviderFn;
    }
  };
}

/**
 * Register a token provider function
 * @param {Function} tokenProviderFn - Async function that returns an authentication token
 */
export function registerTokenProvider(tokenProviderFn) {
  if (typeof window !== 'undefined') {
    window._agentC = window._agentC || {};
    window._agentC.auth = window._agentC.auth || {};
    window._agentC.auth.getToken = tokenProviderFn;
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

export default {
  registerTokenProvider,
  getAuthToken,
  addAuthHeader
};