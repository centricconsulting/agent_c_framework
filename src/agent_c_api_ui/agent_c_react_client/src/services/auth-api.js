import { get } from './api';

/**
 * Authentication service for handling auth-related API calls
 */

/**
 * Validate the authentication token on the server
 * @param {string} token - The access token to validate
 * @returns {Promise<Object>} User information if token is valid
 */
export async function validateToken(token) {
  try {
    const response = await get('/auth/validate', {
      headers: {
        Authorization: `Bearer ${token}`
      }
    });
    return response.data;
  } catch (error) {
    console.error('Token validation failed:', error);
    throw error;
  }
}

/**
 * Get the current authenticated user's information
 * @param {string} token - The access token
 * @returns {Promise<Object>} User information
 */
export async function getUserInfo(token) {
  try {
    const response = await get('/auth/me', {
      headers: {
        Authorization: `Bearer ${token}`
      }
    });
    return response.data;
  } catch (error) {
    console.error('Failed to get user info:', error);
    throw error;
  }
}

/**
 * Add authorization header to API requests
 * @param {Object} config - Request config
 * @param {string} token - Access token
 * @returns {Object} Updated config with auth header
 */
export function addAuthHeader(config, token) {
  if (!token) return config;
  
  return {
    ...config,
    headers: {
      ...config.headers,
      Authorization: `Bearer ${token}`
    }
  };
}

// Default export
const authService = {
  validateToken,
  getUserInfo,
  addAuthHeader,
};

export default authService;