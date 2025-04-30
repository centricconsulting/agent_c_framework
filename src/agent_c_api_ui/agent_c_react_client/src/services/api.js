/**
 * Base API service that provides core HTTP functionality and error handling
 * 
 * This service serves as the foundation for all API interactions in the application.
 * It handles common concerns like authentication, error processing, and request formatting.
 */

import { toast } from "@/hooks/use-toast";

// Default request timeout in milliseconds
const DEFAULT_TIMEOUT = 30000;

/**
 * Configuration for API requests
 */
export const API_CONFIG = {
  baseUrl: import.meta.env.VITE_API_URL || '/api/v1',
  timeout: DEFAULT_TIMEOUT,
  credentials: 'include',
  headers: {
    'Content-Type': 'application/json',
  },
};

/**
 * Parse the response based on content type
 * @param {Response} response - Fetch Response object
 * @returns {Promise<any>} Parsed response data
 */
async function parseResponse(response) {
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
export function processApiError(error, fallbackMessage = 'An unexpected error occurred', options = {}) {
  // Only log errors if not silenced (for expected error cases like 404 validations)
  if (!options.silent) {
    console.error('API Error:', error);
  }
  
  // If it's already a processed error, return it
  if (error && error.isProcessed) {
    return error;
  }
  
  let errorMessage = fallbackMessage;
  let statusCode = null;
  let responseData = null;
  
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
  
  // Create enhanced error object
  const enhancedError = new Error(errorMessage);
  enhancedError.statusCode = statusCode;
  enhancedError.responseData = responseData;
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
 * @returns {object} Combined request options
 */
function createRequestOptions(options = {}) {
  const { headers = {}, ...restOptions } = options;
  
  return {
    ...API_CONFIG,
    ...restOptions,
    headers: {
      ...API_CONFIG.headers,
      ...headers,
    },
    credentials: API_CONFIG.credentials,
  };
}

/**
 * Core API request method
 * @param {string} endpoint - API endpoint to call
 * @param {object} options - Request options
 * @returns {Promise<any>} Response data
 * @throws {Error} Enhanced error with context
 */
export async function apiRequest(endpoint, options = {}) {
  const url = `${API_CONFIG.baseUrl}${endpoint}`;
  const requestOptions = createRequestOptions(options);
  
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
      
      // Create detailed error with parsed error information
      const error = new Error(
        errorData?.message || `Request failed with status ${response.status}`
      );
      error.statusCode = response.status;
      error.responseData = errorData;
      throw processApiError(error);
    }
    
    return await parseResponse(response);
  } catch (error) {
    // Check if this is a regular Error with a statusCode property (added by processApiError)
    if (error instanceof Error && error.statusCode) {
      throw error; // Already processed
    }
    // Process all other errors (network issues, etc.)
    throw processApiError(error);
  }
}

/**
 * Shorthand for GET requests
 * @param {string} endpoint - API endpoint
 * @param {object} options - Request options
 * @returns {Promise<any>} Response data
 */
export function get(endpoint, options = {}) {
  return apiRequest(endpoint, { 
    method: 'GET', 
    ...options 
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
  delete: del,
  uploadFile,
  downloadFile,
  apiRequest,
  processApiError,
  showErrorToast,
  API_CONFIG,
};