/**
 * Base API service that provides core HTTP functionality and error handling
 * 
 * This service serves as the foundation for all API interactions in the application.
 * It handles common concerns like authentication, error processing, and request formatting.
 */

import { toast } from "@/hooks/use-toast";
import { refreshAuthToken } from "@/lib/auth-helper";

// Default request timeout in milliseconds
const DEFAULT_TIMEOUT = 30000;

/**
 * Configuration for API requests
 */
export const API_CONFIG = {
  baseUrl: import.meta.env.VITE_API_URL || '/api/v2', // Updated to v2
  timeout: DEFAULT_TIMEOUT,
  credentials: 'include',
  headers: {
    'Content-Type': 'application/json',
  },
};

/**
 * Extract standardized data from v2 API responses
 * @param {Object} response - The API response object
 * @returns {Object} Extracted data with data, meta, and errors fields
 */
export function extractResponseData(response) {
  // Handle v2 API standard response format
  if (response && typeof response === 'object') {
    return {
      data: response.data !== undefined ? response.data : response,
      meta: response.meta || {},
      errors: response.errors || []
    };
  }
  
  // Return the response itself for endpoints that don't follow the standard format
  return { data: response, meta: {}, errors: [] };
}

/**
 * Parse the response based on content type
 * @param {Response} response - Fetch Response object
 * @returns {Promise<any>} Parsed response data
 */
async function parseResponse(response) {
  // Handle 204 No Content responses
  if (response.status === 204) {
    return { status: 204 }; // Return a minimal object with status
  }
  
  const contentType = response.headers.get('content-type') || '';
  
  if (contentType.includes('application/json')) {
    return response.json();
  } else if (contentType.includes('text/')) {
    return response.text();
  } else {
    // For binary data or other types
    return response.blob();
  }
}

/**
 * Process API errors and provide consistent error handling
 * @param {Error|Response} error - The error object or response
 * @param {string} fallbackMessage - Default message if error details are unavailable
 * @returns {Error} Enhanced error object with additional context
 */
export function processApiError(error, fallbackMessage = 'An unexpected error occurred') {
  console.error('API Error:', error);
  
  // If it's already a processed error, return it
  if (error && error.isProcessed) {
    return error;
  }
  
  let errorMessage = fallbackMessage;
  let statusCode = null;
  let responseData = null;
  let errorDetails = null;
  
  // Handle fetch Response objects
  if (error instanceof Response) {
    statusCode = error.status;
    errorMessage = `Request failed with status ${statusCode}`;
  } 
  // Handle standard Error objects
  else if (error instanceof Error) {
    errorMessage = error.message || errorMessage;
  }
  // Handle error objects with response data (common in fetch catch blocks)
  else if (error && error.message) {
    errorMessage = error.message;
    if (error.response) {
      statusCode = error.response.status;
      responseData = error.response.data;
    }
  }
  
  // Extract v2 API specific error format
  if (responseData && responseData.detail) {
    const detail = responseData.detail;
    if (detail.message) {
      errorMessage = detail.message;
    }
    errorDetails = {
      error: detail.error,
      error_code: detail.error_code,
      params: detail.params
    };
  }
  
  // Create enhanced error object
  const enhancedError = new Error(errorMessage);
  enhancedError.statusCode = statusCode;
  enhancedError.responseData = responseData;
  enhancedError.errorDetails = errorDetails; // Add v2 API specific error details
  enhancedError.originalError = error;
  enhancedError.isProcessed = true;
  
  return enhancedError;
}

/**
 * Display error toast notification
 * @param {Error} error - The error object
 * @param {string} fallbackMessage - Default message if error details are unavailable
 */
export function showErrorToast(error, fallbackMessage = 'An unexpected error occurred') {
  const message = error.message || fallbackMessage;
  
  toast({
    variant: "destructive",
    title: "Error",
    description: message,
  });
}

/**
 * Create request options with proper headers and configuration
 * @param {object} options - Custom request options to merge with defaults
 * @param {string} [authToken] - Optional auth token to include
 * @returns {object} Combined request options
 */
function createRequestOptions(options = {}, authToken = null) {
  const { headers = {}, ...restOptions } = options;
  
  const allHeaders = {
    ...API_CONFIG.headers,
    ...headers,
  };
  
  // Add Authorization header if token is provided
  if (authToken) {
    allHeaders['Authorization'] = `Bearer ${authToken}`;
  }
  
  return {
    ...API_CONFIG,
    ...restOptions,
    headers: allHeaders,
    credentials: API_CONFIG.credentials,
  };
}

/**
 * Core API request method with token refresh capability
 * @param {string} endpoint - API endpoint to call
 * @param {object} options - Request options
 * @param {string} [authToken] - Optional auth token to include
 * @param {boolean} [isRetry] - Internal flag to prevent infinite retry loops
 * @returns {Promise<any>} Response data
 * @throws {Error} Enhanced error with context
 */
export async function apiRequest(endpoint, options = {}, authToken = null, isRetry = false) {
  // Make sure endpoint starts with a slash if it's not an absolute URL
  const normalizedEndpoint = endpoint.startsWith('/') || endpoint.startsWith('http') ? endpoint : `/${endpoint}`;
  
  // Ensure we have a valid URL by combining with base URL
  const url = normalizedEndpoint.startsWith('http') 
    ? normalizedEndpoint 
    : `${API_CONFIG.baseUrl}${normalizedEndpoint}`;
  
  // Try to get auth token from AuthContext if not provided and we're in a browser
  if (!authToken && typeof window !== 'undefined') {
    try {
      // Only attempt this if we have the auth module available
      const auth = window._agentC?.auth;
      if (auth && typeof auth.getToken === 'function') {
        authToken = await auth.getToken();
        // Don't warn if token is null - this is expected for unauthenticated states
        if (!authToken) {
          console.debug('No auth token available - user may not be authenticated');
        }
      }
    } catch (error) {
      console.warn('Error accessing auth token provider:', error);
    }
  }
  
  const requestOptions = createRequestOptions(options, authToken);
  
  try {
    const response = await fetch(url, requestOptions);
    
    if (!response.ok) {
      // For error responses, try to parse the error details
      let errorData;
      try {
        errorData = await parseResponse(response);
      } catch (parseError) {
        // If parsing fails, use generic error
        throw processApiError(
          response, 
          `Request failed with status ${response.status}`
        );
      }
      
      // Check for authentication errors and retry with fresh token if not already retrying
      if ((response.status === 401 || response.status === 403) && !isRetry && authToken) {
        console.info('Authentication error detected, attempting token refresh and retry');
        
        try {
          // Force token refresh
          const newToken = await refreshAuthToken();
          
          if (newToken) {
            console.info('Token refreshed successfully, retrying request');
            // Retry the request with the new token, but mark as retry to prevent loops
            return await apiRequest(endpoint, options, newToken, true);
          } else {
            console.warn('Token refresh returned null, cannot retry request');
          }
        } catch (refreshError) {
          console.error('Failed to refresh token for retry:', refreshError);
          // Continue with original error if refresh fails
        }
      }
      
      // Create detailed error with parsed error information
      const error = new Error(
        errorData?.detail?.message || errorData?.message || `Request failed with status ${response.status}`
      );
      error.statusCode = response.status;
      error.responseData = errorData;
      throw processApiError(error);
    }
    
    return await parseResponse(response);
  } catch (error) {
    // Process all other errors (network issues, etc.)
    throw processApiError(error);
  }
}

/**
 * Shorthand for GET requests with pagination support
 * @param {string} endpoint - API endpoint
 * @param {object} options - Request options
 * @returns {Promise<any>} Response data
 */
export function get(endpoint, options = {}) {
  // Extract pagination parameters if present
  const { params, ...restOptions } = options;
  
  // If pagination parameters are provided, add them to the query string
  if (params) {
    const queryParams = new URLSearchParams();
    
    // Add pagination parameters
    Object.entries(params).forEach(([key, value]) => {
      if (value !== undefined && value !== null) {
        queryParams.append(key, value);
      }
    });
    
    // Append query string to endpoint if there are parameters
    const queryString = queryParams.toString();
    if (queryString) {
      endpoint = `${endpoint}${endpoint.includes('?') ? '&' : '?'}${queryString}`;
    }
  }
  
  return apiRequest(endpoint, { 
    method: 'GET', 
    ...restOptions 
  });
}

/**
 * Shorthand for POST requests
 * @param {string} endpoint - API endpoint
 * @param {any} data - Request body data
 * @param {object} options - Additional request options
 * @returns {Promise<any>} Response data
 */
export function post(endpoint, data, options = {}) {
  return apiRequest(endpoint, {
    method: 'POST',
    body: JSON.stringify(data),
    ...options,
  });
}

/**
 * Shorthand for PUT requests
 * @param {string} endpoint - API endpoint
 * @param {any} data - Request body data
 * @param {object} options - Additional request options
 * @returns {Promise<any>} Response data
 */
export function put(endpoint, data, options = {}) {
  return apiRequest(endpoint, {
    method: 'PUT',
    body: JSON.stringify(data),
    ...options,
  });
}

/**
 * Shorthand for PATCH requests
 * @param {string} endpoint - API endpoint
 * @param {any} data - Request body data
 * @param {object} options - Additional request options
 * @returns {Promise<any>} Response data
 */
export function patch(endpoint, data, options = {}) {
  return apiRequest(endpoint, {
    method: 'PATCH',
    body: JSON.stringify(data),
    ...options,
  });
}

/**
 * Shorthand for DELETE requests
 * @param {string} endpoint - API endpoint
 * @param {object} options - Request options
 * @returns {Promise<any>} Response data
 */
export function del(endpoint, options = {}) {
  return apiRequest(endpoint, {
    method: 'DELETE',
    ...options,
  });
}

/**
 * Upload a file to the API
 * @param {string} endpoint - API endpoint
 * @param {File|Blob} file - File to upload
 * @param {string} fieldName - Form field name for the file
 * @param {object} additionalData - Additional form data to include
 * @param {object} options - Additional request options
 * @returns {Promise<any>} Response data
 */
export function uploadFile(endpoint, file, fieldName = 'file', additionalData = {}, options = {}) {
  const formData = new FormData();
  formData.append(fieldName, file);
  
  // Add any additional data to the form
  Object.entries(additionalData).forEach(([key, value]) => {
    formData.append(key, value);
  });
  
  return apiRequest(endpoint, {
    method: 'POST',
    body: formData,
    headers: {}, // Let browser set content-type with boundary
    ...options,
  });
}

/**
 * Download a file from the API
 * @param {string} endpoint - API endpoint
 * @param {object} options - Request options
 * @returns {Promise<Blob>} File blob
 */
export async function downloadFile(endpoint, options = {}) {
  const url = `${API_CONFIG.baseUrl}${endpoint}`;
  const requestOptions = createRequestOptions({
    method: 'GET',
    ...options,
  });
  
  try {
    const response = await fetch(url, requestOptions);
    
    if (!response.ok) {
      throw processApiError(
        response, 
        `File download failed with status ${response.status}`
      );
    }
    
    return await response.blob();
  } catch (error) {
    throw processApiError(error, 'File download failed');
  }
}

// Export default API object with all methods
export default {
  get,
  post,
  put,
  patch,
  delete: del,
  uploadFile,
  downloadFile,
  apiRequest,
  processApiError,
  showErrorToast,
  extractResponseData,
  API_CONFIG,
};