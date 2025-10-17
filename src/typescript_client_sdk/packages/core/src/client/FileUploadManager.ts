/**
 * FileUploadManager - Handles HTTP file upload operations
 * 
 * This manager handles multipart/form-data file uploads to the Agent C API
 * separate from the WebSocket connection. It provides progress tracking,
 * cancellation support, and proper error handling.
 */

import type { UserFileResponse, FileUploadOptions, UploadProgress } from '../events/types/CommonTypes';
import type { RealtimeClientConfig } from './ClientConfig';

/**
 * Manager for file upload operations
 * Handles HTTP multipart/form-data uploads to the Agent C API
 */
export class FileUploadManager {
  private apiUrl: string;
  private authToken: string | null = null;
  private uiSessionId: string | null = null;
  private maxUploadSize: number;
  private allowedMimeTypes?: string[];
  private maxFilesPerMessage: number;

  /**
   * Create a new FileUploadManager
   * @param apiUrl - Base API URL for uploads
   * @param authToken - Initial authentication token (optional)
   * @param uiSessionId - Initial UI session ID (optional)
   * @param config - Upload configuration (maxUploadSize, allowedMimeTypes, maxFilesPerMessage)
   */
  constructor(
    apiUrl: string,
    authToken: string | undefined,
    uiSessionId: string | undefined,
    config: Pick<RealtimeClientConfig, 'maxUploadSize' | 'allowedMimeTypes' | 'maxFilesPerMessage'>
  ) {
    this.apiUrl = apiUrl;
    this.authToken = authToken || null;
    this.uiSessionId = uiSessionId || null;
    this.maxUploadSize = config.maxUploadSize || 10 * 1024 * 1024; // Default 10MB
    this.allowedMimeTypes = config.allowedMimeTypes;
    this.maxFilesPerMessage = config.maxFilesPerMessage || 10;
  }

  /**
   * Upload a file to the server
   * @param file - File object to upload
   * @param options - Upload options (progress callback, abort signal)
   * @returns Promise resolving to UserFileResponse with file metadata
   * @throws Error if authentication is missing, file is too large, or upload fails
   */
  async uploadFile(file: File, options?: FileUploadOptions): Promise<UserFileResponse> {
    // Validate authentication
    if (!this.authToken) {
      throw new Error('Authentication required for file upload');
    }

    if (!this.uiSessionId) {
      throw new Error('UI session ID required for file upload');
    }

    // Validate file size (client-side check)
    if (file.size > this.maxUploadSize) {
      throw new Error(
        `File size ${file.size} bytes exceeds maximum ${this.maxUploadSize} bytes`
      );
    }

    // Validate MIME type if restrictions are configured
    if (this.allowedMimeTypes && this.allowedMimeTypes.length > 0) {
      if (!this.allowedMimeTypes.includes(file.type)) {
        throw new Error(
          `File type ${file.type} is not allowed. Allowed types: ${this.allowedMimeTypes.join(', ')}`
        );
      }
    }

    return new Promise<UserFileResponse>((resolve, reject) => {
      const xhr = new XMLHttpRequest();
      const formData = this.createFormData(file);

      // Setup abort signal if provided
      if (options?.signal) {
        // Check if already aborted
        if (options.signal.aborted) {
          reject(new Error('Upload cancelled'));
          return;
        }

        // Listen for abort event
        options.signal.addEventListener('abort', () => {
          xhr.abort();
          reject(new Error('Upload cancelled'));
        });
      }

      // Setup progress tracking if callback provided
      if (options?.onProgress) {
        this.trackUploadProgress(xhr, options.onProgress);
      }

      // Setup response handlers
      xhr.onload = () => {
        if (xhr.status >= 200 && xhr.status < 300) {
          try {
            const response: UserFileResponse = JSON.parse(xhr.responseText);
            resolve(response);
          } catch (error) {
            reject(new Error(`Failed to parse server response: ${error instanceof Error ? error.message : 'Unknown error'}`));
          }
        } else {
          // Handle HTTP error responses
          let errorMessage = `Upload failed with status ${xhr.status}`;
          try {
            const errorResponse = JSON.parse(xhr.responseText);
            if (errorResponse.error || errorResponse.message) {
              errorMessage = errorResponse.error || errorResponse.message;
            }
          } catch {
            // If response is not JSON, use status text
            errorMessage = xhr.statusText ? `Upload failed: ${xhr.statusText}` : errorMessage;
          }
          reject(new Error(errorMessage));
        }
      };

      xhr.onerror = () => {
        reject(new Error('Network error during upload'));
      };

      xhr.ontimeout = () => {
        reject(new Error('Upload timed out'));
      };

      // Open connection and send request
      try {
        xhr.open('POST', this.buildUploadUrl());
        xhr.setRequestHeader('Authorization', `Bearer ${this.authToken}`);
        
        // Set timeout to 5 minutes for large file uploads
        xhr.timeout = 5 * 60 * 1000; // 5 minutes
        
        xhr.send(formData);
      } catch (error) {
        reject(new Error(`Failed to initiate upload: ${error instanceof Error ? error.message : 'Unknown error'}`));
      }
    });
  }

  /**
   * Upload multiple files sequentially
   * @param files - Array of File objects to upload
   * @param options - Upload options (progress callback, abort signal)
   * @returns Promise resolving to array of UserFileResponse
   * @throws Error if too many files, authentication missing, or upload fails
   */
  async uploadFiles(files: File[], options?: FileUploadOptions): Promise<UserFileResponse[]> {
    // Validate file count
    if (files.length > this.maxFilesPerMessage) {
      throw new Error(
        `Cannot upload ${files.length} files. Maximum allowed per message: ${this.maxFilesPerMessage}`
      );
    }

    if (files.length === 0) {
      return [];
    }

    const results: UserFileResponse[] = [];
    const totalFiles = files.length;

    // Upload files sequentially to avoid overwhelming the server
    for (let i = 0; i < files.length; i++) {
      const file = files[i];
      
      // TypeScript safety check (should never happen due to loop bounds)
      if (!file) {
        throw new Error(`File at index ${i} is undefined`);
      }

      // Create per-file progress callback that aggregates total progress
      const fileOptions: FileUploadOptions = {
        ...options,
        onProgress: options?.onProgress
          ? (progress) => {
              // Calculate overall progress across all files
              const filesCompleted = i;
              const currentFileProgress = progress.percentage / 100;
              const overallProgress = (filesCompleted + currentFileProgress) / totalFiles;

              // Call the original progress callback with aggregated progress
              options.onProgress!({
                loaded: Math.round(overallProgress * 100),
                total: 100,
                percentage: Math.round(overallProgress * 100)
              });
            }
          : undefined
      };

      try {
        const result = await this.uploadFile(file, fileOptions);
        results.push(result);
      } catch (error) {
        // If one file fails, throw error with information about partial success
        throw new Error(
          `Failed to upload file ${i + 1} of ${totalFiles} (${file.name}): ${
            error instanceof Error ? error.message : 'Unknown error'
          }. ${results.length} file(s) uploaded successfully before failure.`
        );
      }
    }

    return results;
  }

  /**
   * Update authentication token
   * @param token - New authentication token
   */
  setAuthToken(token: string): void {
    this.authToken = token;
  }

  /**
   * Update UI session ID
   * @param sessionId - New UI session ID
   */
  setUiSessionId(sessionId: string): void {
    this.uiSessionId = sessionId;
  }

  /**
   * Build the upload URL from the base API URL
   * Converts WebSocket URL (ws://, wss://) to HTTP URL (http://, https://)
   * @returns Full URL for the upload endpoint
   * @private
   */
  private buildUploadUrl(): string {
    try {
      const url = new URL(this.apiUrl);
      
      // Convert WebSocket protocol to HTTP protocol
      if (url.protocol === 'ws:') {
        url.protocol = 'http:';
      } else if (url.protocol === 'wss:') {
        url.protocol = 'https:';
      }
      // If already http/https, leave as-is
      
      url.pathname = '/api/rt/upload_file';
      return url.toString();
    } catch (error) {
      throw new Error(`Invalid API URL: ${this.apiUrl}`);
    }
  }

  /**
   * Create FormData object with file and metadata
   * @param file - File to upload
   * @returns FormData ready for upload
   * @private
   */
  private createFormData(file: File): FormData {
    const formData = new FormData();
    
    // Add UI session ID
    if (this.uiSessionId) {
      formData.append('ui_session_id', this.uiSessionId);
    }
    
    // Add the file
    formData.append('file', file);
    
    return formData;
  }

  /**
   * Setup upload progress tracking
   * @param xhr - XMLHttpRequest instance
   * @param onProgress - Progress callback function
   * @private
   */
  private trackUploadProgress(
    xhr: XMLHttpRequest,
    onProgress: (progress: UploadProgress) => void
  ): void {
    // Track upload progress (outgoing data)
    xhr.upload.addEventListener('progress', (event) => {
      if (event.lengthComputable) {
        const progress: UploadProgress = {
          loaded: event.loaded,
          total: event.total,
          percentage: Math.round((event.loaded / event.total) * 100)
        };
        onProgress(progress);
      }
    });
  }
}
