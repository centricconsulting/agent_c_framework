/**
 * Unit tests for FileUploadManager
 * Tests file upload operations, progress tracking, cancellation, and error handling
 */

import { describe, it, expect, beforeEach, afterEach, vi } from 'vitest';
import { FileUploadManager } from '../FileUploadManager';
import type { UserFileResponse, FileUploadOptions, UploadProgress } from '../../events/types/CommonTypes';

// Mock XMLHttpRequest
class MockXMLHttpRequest {
  public status: number = 200;
  public statusText: string = 'OK';
  public responseText: string = '';
  public timeout: number = 0;
  public upload: { addEventListener: ReturnType<typeof vi.fn> } = {
    addEventListener: vi.fn(),
  };

  public onload: (() => void) | null = null;
  public onerror: (() => void) | null = null;
  public ontimeout: (() => void) | null = null;

  private headers: Record<string, string> = {};
  private method: string = '';
  private url: string = '';
  private sentData: any = null;
  private aborted: boolean = false;

  open(method: string, url: string): void {
    this.method = method;
    this.url = url;
  }

  setRequestHeader(name: string, value: string): void {
    this.headers[name] = value;
  }

  send(data: any): void {
    this.sentData = data;
    // Don't call onload automatically - let tests control when it's called
  }

  abort(): void {
    this.aborted = true;
  }

  getMethod(): string {
    return this.method;
  }

  getUrl(): string {
    return this.url;
  }

  getHeaders(): Record<string, string> {
    return this.headers;
  }

  getSentData(): any {
    return this.sentData;
  }

  isAborted(): boolean {
    return this.aborted;
  }
}

describe('FileUploadManager', () => {
  let uploadManager: FileUploadManager;
  let mockXHR: MockXMLHttpRequest;
  let originalXMLHttpRequest: any;

  const testConfig = {
    maxUploadSize: 10 * 1024 * 1024, // 10MB
    allowedMimeTypes: undefined,
    maxFilesPerMessage: 10,
  };

  beforeEach(() => {
    // Save original XMLHttpRequest
    originalXMLHttpRequest = global.XMLHttpRequest;

    // Setup mock XMLHttpRequest
    mockXHR = new MockXMLHttpRequest();
    global.XMLHttpRequest = vi.fn(() => mockXHR) as any;

    // Create FileUploadManager
    uploadManager = new FileUploadManager(
      'https://api.example.com',
      'test-auth-token',
      'test-session-id',
      testConfig
    );
  });

  afterEach(() => {
    // Restore original XMLHttpRequest
    global.XMLHttpRequest = originalXMLHttpRequest;
    vi.clearAllMocks();
  });

  describe('Basic Upload Operations', () => {
    it('should upload single file successfully', async () => {
      const mockFile = new File(['test content'], 'test.txt', { type: 'text/plain' });
      const mockResponse: UserFileResponse = {
        id: 'file-123',
        filename: 'test.txt',
        mime_type: 'text/plain',
        size: 12,
      };

      // Setup mock response
      mockXHR.status = 200;
      mockXHR.responseText = JSON.stringify(mockResponse);
      mockXHR.send = vi.fn((data: any) => {
        mockXHR.sentData = data;
        setTimeout(() => {
          if (!mockXHR.isAborted() && mockXHR.onload) {
            mockXHR.onload();
          }
        }, 0);
      });

      const uploadPromise = uploadManager.uploadFile(mockFile);
      const result = await uploadPromise;

      expect(result).toEqual(mockResponse);
      expect(mockXHR.getMethod()).toBe('POST');
      expect(mockXHR.getUrl()).toBe('https://api.example.com/api/rt/upload_file');
    });

    it('should receive correct UserFileResponse structure', async () => {
      const mockFile = new File(['test content'], 'document.pdf', { type: 'application/pdf' });
      const mockResponse: UserFileResponse = {
        id: 'file-456',
        filename: 'document.pdf',
        mime_type: 'application/pdf',
        size: 1024,
      };

      mockXHR.status = 200;
      mockXHR.responseText = JSON.stringify(mockResponse);
      mockXHR.send = vi.fn((data: any) => {
        mockXHR.sentData = data;
        setTimeout(() => {
          if (!mockXHR.isAborted() && mockXHR.onload) {
            mockXHR.onload();
          }
        }, 0);
      });

      const result = await uploadManager.uploadFile(mockFile);

      expect(result).toHaveProperty('id');
      expect(result).toHaveProperty('filename');
      expect(result).toHaveProperty('mime_type');
      expect(result).toHaveProperty('size');
      expect(result.id).toBe('file-456');
      expect(result.filename).toBe('document.pdf');
      expect(result.mime_type).toBe('application/pdf');
      expect(result.size).toBe(1024);
    });

    it('should include ui_session_id in form data', async () => {
      const mockFile = new File(['test'], 'test.txt', { type: 'text/plain' });
      const mockResponse: UserFileResponse = {
        id: 'file-789',
        filename: 'test.txt',
        mime_type: 'text/plain',
        size: 4,
      };

      mockXHR.status = 200;
      mockXHR.responseText = JSON.stringify(mockResponse);
      mockXHR.send = vi.fn((data: any) => {
        mockXHR.sentData = data;
        setTimeout(() => {
          if (!mockXHR.isAborted() && mockXHR.onload) {
            mockXHR.onload();
          }
        }, 0);
      });

      await uploadManager.uploadFile(mockFile);

      const formData = mockXHR.getSentData() as FormData;
      expect(formData).toBeInstanceOf(FormData);
      expect(formData.get('ui_session_id')).toBe('test-session-id');
      expect(formData.get('file')).toBe(mockFile);
    });

    it('should include auth token in headers', async () => {
      const mockFile = new File(['test'], 'test.txt', { type: 'text/plain' });
      const mockResponse: UserFileResponse = {
        id: 'file-abc',
        filename: 'test.txt',
        mime_type: 'text/plain',
        size: 4,
      };

      mockXHR.status = 200;
      mockXHR.responseText = JSON.stringify(mockResponse);
      mockXHR.send = vi.fn((data: any) => {
        mockXHR.sentData = data;
        setTimeout(() => {
          if (!mockXHR.isAborted() && mockXHR.onload) {
            mockXHR.onload();
          }
        }, 0);
      });

      await uploadManager.uploadFile(mockFile);

      const headers = mockXHR.getHeaders();
      expect(headers['Authorization']).toBe('Bearer test-auth-token');
    });

    it('should upload multiple files successfully', async () => {
      const mockFiles = [
        new File(['test 1'], 'test1.txt', { type: 'text/plain' }),
        new File(['test 2'], 'test2.txt', { type: 'text/plain' }),
      ];

      const mockResponses: UserFileResponse[] = [
        { id: 'file-1', filename: 'test1.txt', mime_type: 'text/plain', size: 6 },
        { id: 'file-2', filename: 'test2.txt', mime_type: 'text/plain', size: 6 },
      ];

      let responseIndex = 0;
      mockXHR.send = vi.fn((data: any) => {
        mockXHR.sentData = data;
        mockXHR.status = 200;
        mockXHR.responseText = JSON.stringify(mockResponses[responseIndex]);
        responseIndex++;
        setTimeout(() => {
          if (!mockXHR.isAborted() && mockXHR.onload) {
            mockXHR.onload();
          }
        }, 0);
      });

      const results = await uploadManager.uploadFiles(mockFiles);

      expect(results).toHaveLength(2);
      expect(results[0]).toEqual(mockResponses[0]);
      expect(results[1]).toEqual(mockResponses[1]);
    });
  });

  describe('Progress Tracking', () => {
    it('should invoke progress callback with correct data', async () => {
      const mockFile = new File(['test content'], 'test.txt', { type: 'text/plain' });
      const mockResponse: UserFileResponse = {
        id: 'file-123',
        filename: 'test.txt',
        mime_type: 'text/plain',
        size: 12,
      };

      mockXHR.status = 200;
      mockXHR.responseText = JSON.stringify(mockResponse);

      const progressCallback = vi.fn();
      const options: FileUploadOptions = {
        onProgress: progressCallback,
      };

      // Setup progress event simulation
      mockXHR.upload.addEventListener = vi.fn((event: string, callback: any) => {
        if (event === 'progress') {
          // Simulate progress event
          setTimeout(() => {
            callback({
              lengthComputable: true,
              loaded: 6,
              total: 12,
            });
          }, 0);
        }
      });

      mockXHR.send = vi.fn((data: any) => {
        mockXHR.sentData = data;
        setTimeout(() => {
          if (!mockXHR.isAborted() && mockXHR.onload) {
            mockXHR.onload();
          }
        }, 20); // Delay to allow progress event
      });

      await uploadManager.uploadFile(mockFile, options);

      // Wait for progress callback
      await new Promise(resolve => setTimeout(resolve, 30));

      expect(progressCallback).toHaveBeenCalled();
      expect(mockXHR.upload.addEventListener).toHaveBeenCalledWith('progress', expect.any(Function));
    });

    it('should calculate progress percentage correctly', async () => {
      const mockFile = new File(['test content'], 'test.txt', { type: 'text/plain' });
      const mockResponse: UserFileResponse = {
        id: 'file-123',
        filename: 'test.txt',
        mime_type: 'text/plain',
        size: 12,
      };

      mockXHR.status = 200;
      mockXHR.responseText = JSON.stringify(mockResponse);

      const progressCallback = vi.fn();
      const options: FileUploadOptions = {
        onProgress: progressCallback,
      };

      // Setup progress event simulation with specific values
      mockXHR.upload.addEventListener = vi.fn((event: string, callback: any) => {
        if (event === 'progress') {
          setTimeout(() => {
            callback({
              lengthComputable: true,
              loaded: 50,
              total: 100,
            });
          }, 0);
        }
      });

      mockXHR.send = vi.fn((data: any) => {
        mockXHR.sentData = data;
        setTimeout(() => {
          if (!mockXHR.isAborted() && mockXHR.onload) {
            mockXHR.onload();
          }
        }, 20); // Delay to allow progress event
      });

      await uploadManager.uploadFile(mockFile, options);

      // Wait for progress callback
      await new Promise(resolve => setTimeout(resolve, 30));

      expect(progressCallback).toHaveBeenCalledWith({
        loaded: 50,
        total: 100,
        percentage: 50,
      });
    });

    it('should report aggregated progress for multiple files', async () => {
      const mockFiles = [
        new File(['test 1'], 'test1.txt', { type: 'text/plain' }),
        new File(['test 2'], 'test2.txt', { type: 'text/plain' }),
      ];

      const mockResponses: UserFileResponse[] = [
        { id: 'file-1', filename: 'test1.txt', mime_type: 'text/plain', size: 6 },
        { id: 'file-2', filename: 'test2.txt', mime_type: 'text/plain', size: 6 },
      ];

      let fileIndex = 0;
      const progressCallback = vi.fn();
      const options: FileUploadOptions = {
        onProgress: progressCallback,
      };

      mockXHR.send = vi.fn((data: any) => {
        mockXHR.sentData = data;
        const currentIndex = fileIndex;
        mockXHR.status = 200;
        mockXHR.responseText = JSON.stringify(mockResponses[currentIndex]);
        
        // Simulate progress for this file
        const progressListener = (mockXHR.upload.addEventListener as any).mock.calls.find(
          (call: any[]) => call[0] === 'progress'
        )?.[1];
        
        if (progressListener) {
          // Simulate 50% progress on current file
          progressListener({
            lengthComputable: true,
            loaded: 50,
            total: 100,
          });
        }
        
        fileIndex++;
        setTimeout(() => {
          if (!mockXHR.isAborted() && mockXHR.onload) {
            mockXHR.onload();
          }
        }, 0);
      });

      await uploadManager.uploadFiles(mockFiles, options);

      // Progress should be called with aggregated values
      expect(progressCallback).toHaveBeenCalled();
    });
  });

  describe('Cancellation', () => {
    it('should cancel upload via AbortSignal', async () => {
      const mockFile = new File(['test content'], 'test.txt', { type: 'text/plain' });
      const abortController = new AbortController();

      const options: FileUploadOptions = {
        signal: abortController.signal,
      };

      // Abort immediately
      abortController.abort();

      await expect(uploadManager.uploadFile(mockFile, options)).rejects.toThrow('Upload cancelled');
    });

    it('should cancel upload after start', async () => {
      const mockFile = new File(['test content'], 'test.txt', { type: 'text/plain' });
      const abortController = new AbortController();

      const options: FileUploadOptions = {
        signal: abortController.signal,
      };

      // Setup send to simulate async upload
      mockXHR.send = vi.fn((data: any) => {
        mockXHR.sentData = data;
        // Don't call onload - simulating in-flight upload
      });

      // Start upload
      const uploadPromise = uploadManager.uploadFile(mockFile, options);

      // Abort immediately
      setTimeout(() => {
        abortController.abort();
      }, 5);

      await expect(uploadPromise).rejects.toThrow('Upload cancelled');
    });

    it('should abort XHR when cancelled', async () => {
      const mockFile = new File(['test content'], 'test.txt', { type: 'text/plain' });
      const abortController = new AbortController();

      const abortSpy = vi.spyOn(mockXHR, 'abort');

      const options: FileUploadOptions = {
        signal: abortController.signal,
      };

      // Start upload
      const uploadPromise = uploadManager.uploadFile(mockFile, options);

      // Abort immediately
      abortController.abort();

      await expect(uploadPromise).rejects.toThrow('Upload cancelled');
      
      // Wait a bit for the abort to be processed
      await new Promise(resolve => setTimeout(resolve, 10));
      
      expect(abortSpy).toHaveBeenCalled();
    });

    it('should throw expected error message on cancellation', async () => {
      const mockFile = new File(['test content'], 'test.txt', { type: 'text/plain' });
      const abortController = new AbortController();

      abortController.abort();

      const options: FileUploadOptions = {
        signal: abortController.signal,
      };

      await expect(uploadManager.uploadFile(mockFile, options)).rejects.toThrow('Upload cancelled');
    });
  });

  describe('Error Handling', () => {
    it('should throw when not authenticated', async () => {
      const unauthenticatedManager = new FileUploadManager(
        'https://api.example.com',
        undefined, // No auth token
        'test-session-id',
        testConfig
      );

      const mockFile = new File(['test'], 'test.txt', { type: 'text/plain' });

      await expect(unauthenticatedManager.uploadFile(mockFile)).rejects.toThrow(
        'Authentication required for file upload'
      );
    });

    it('should throw when no UI session ID', async () => {
      const noSessionManager = new FileUploadManager(
        'https://api.example.com',
        'test-auth-token',
        undefined, // No session ID
        testConfig
      );

      const mockFile = new File(['test'], 'test.txt', { type: 'text/plain' });

      await expect(noSessionManager.uploadFile(mockFile)).rejects.toThrow(
        'UI session ID required for file upload'
      );
    });

    it('should throw when file exceeds max size', async () => {
      const largeFile = new File(
        [new ArrayBuffer(11 * 1024 * 1024)], // 11MB
        'large.txt',
        { type: 'text/plain' }
      );

      await expect(uploadManager.uploadFile(largeFile)).rejects.toThrow(/exceeds maximum/);
    });

    it('should handle 4xx server errors', async () => {
      const mockFile = new File(['test'], 'test.txt', { type: 'text/plain' });

      mockXHR.status = 400;
      mockXHR.statusText = 'Bad Request';
      mockXHR.responseText = JSON.stringify({ error: 'Invalid file format' });
      mockXHR.send = vi.fn((data: any) => {
        mockXHR.sentData = data;
        setTimeout(() => {
          if (mockXHR.onload) {
            mockXHR.onload();
          }
        }, 0);
      });

      await expect(uploadManager.uploadFile(mockFile)).rejects.toThrow('Invalid file format');
    });

    it('should handle 5xx server errors', async () => {
      const mockFile = new File(['test'], 'test.txt', { type: 'text/plain' });

      mockXHR.status = 500;
      mockXHR.statusText = 'Internal Server Error';
      mockXHR.responseText = JSON.stringify({ error: 'Server error occurred' });
      mockXHR.send = vi.fn((data: any) => {
        mockXHR.sentData = data;
        setTimeout(() => {
          if (mockXHR.onload) {
            mockXHR.onload();
          }
        }, 0);
      });

      await expect(uploadManager.uploadFile(mockFile)).rejects.toThrow('Server error occurred');
    });

    it('should handle network errors', async () => {
      const mockFile = new File(['test'], 'test.txt', { type: 'text/plain' });

      // Override send to trigger error
      mockXHR.send = vi.fn(() => {
        setTimeout(() => {
          if (mockXHR.onerror) {
            mockXHR.onerror();
          }
        }, 0);
      });

      await expect(uploadManager.uploadFile(mockFile)).rejects.toThrow('Network error during upload');
    });

    it('should handle timeout errors', async () => {
      const mockFile = new File(['test'], 'test.txt', { type: 'text/plain' });

      // Override send to trigger timeout
      mockXHR.send = vi.fn(() => {
        setTimeout(() => {
          if (mockXHR.ontimeout) {
            mockXHR.ontimeout();
          }
        }, 0);
      });

      await expect(uploadManager.uploadFile(mockFile)).rejects.toThrow('Upload timed out');
    });

    it('should handle invalid JSON response', async () => {
      const mockFile = new File(['test'], 'test.txt', { type: 'text/plain' });

      mockXHR.status = 200;
      mockXHR.responseText = 'Invalid JSON {{{';
      mockXHR.send = vi.fn((data: any) => {
        mockXHR.sentData = data;
        setTimeout(() => {
          if (mockXHR.onload) {
            mockXHR.onload();
          }
        }, 0);
      });

      await expect(uploadManager.uploadFile(mockFile)).rejects.toThrow(/Failed to parse server response/);
    });

    it('should handle error with message field in response', async () => {
      const mockFile = new File(['test'], 'test.txt', { type: 'text/plain' });

      mockXHR.status = 403;
      mockXHR.responseText = JSON.stringify({ message: 'Access denied' });
      mockXHR.send = vi.fn((data: any) => {
        mockXHR.sentData = data;
        setTimeout(() => {
          if (mockXHR.onload) {
            mockXHR.onload();
          }
        }, 0);
      });

      await expect(uploadManager.uploadFile(mockFile)).rejects.toThrow('Access denied');
    });

    it('should use statusText when no error details available', async () => {
      const mockFile = new File(['test'], 'test.txt', { type: 'text/plain' });

      mockXHR.status = 404;
      mockXHR.statusText = 'Not Found';
      mockXHR.responseText = 'Some non-JSON response';
      mockXHR.send = vi.fn((data: any) => {
        mockXHR.sentData = data;
        setTimeout(() => {
          if (mockXHR.onload) {
            mockXHR.onload();
          }
        }, 0);
      });

      await expect(uploadManager.uploadFile(mockFile)).rejects.toThrow('Upload failed: Not Found');
    });

    it('should handle multiple files with partial failure', async () => {
      const mockFiles = [
        new File(['test 1'], 'test1.txt', { type: 'text/plain' }),
        new File(['test 2'], 'test2.txt', { type: 'text/plain' }),
      ];

      let callCount = 0;
      mockXHR.send = vi.fn((data: any) => {
        mockXHR.sentData = data;
        const currentCall = callCount;
        callCount++;
        
        if (currentCall === 0) {
          // First file succeeds
          mockXHR.status = 200;
          mockXHR.responseText = JSON.stringify({
            id: 'file-1',
            filename: 'test1.txt',
            mime_type: 'text/plain',
            size: 6,
          });
          setTimeout(() => {
            if (!mockXHR.isAborted() && mockXHR.onload) {
              mockXHR.onload();
            }
          }, 0);
        } else {
          // Second file fails
          mockXHR.status = 500;
          mockXHR.responseText = JSON.stringify({ error: 'Upload failed' });
          setTimeout(() => {
            if (mockXHR.onload) {
              mockXHR.onload();
            }
          }, 0);
        }
      });

      try {
        await uploadManager.uploadFiles(mockFiles);
        // Should not reach here
        expect.fail('Expected uploadFiles to throw an error');
      } catch (error: any) {
        expect(error.message).toMatch(/Failed to upload file 2 of 2/);
        expect(error.message).toMatch(/1 file\(s\) uploaded successfully/);
      }
    });

    it('should reject empty file array', async () => {
      const result = await uploadManager.uploadFiles([]);
      expect(result).toEqual([]);
    });

    it('should enforce max files per message limit', async () => {
      const tooManyFiles = Array.from({ length: 11 }, (_, i) =>
        new File([`test ${i}`], `test${i}.txt`, { type: 'text/plain' })
      );

      await expect(uploadManager.uploadFiles(tooManyFiles)).rejects.toThrow(
        /Cannot upload 11 files.*Maximum allowed per message: 10/
      );
    });
  });

  describe('Token Management', () => {
    it('should update auth token via setAuthToken()', async () => {
      const newToken = 'new-auth-token-456';
      uploadManager.setAuthToken(newToken);

      const mockFile = new File(['test'], 'test.txt', { type: 'text/plain' });
      const mockResponse: UserFileResponse = {
        id: 'file-xyz',
        filename: 'test.txt',
        mime_type: 'text/plain',
        size: 4,
      };

      mockXHR.status = 200;
      mockXHR.responseText = JSON.stringify(mockResponse);
      mockXHR.send = vi.fn((data: any) => {
        mockXHR.sentData = data;
        setTimeout(() => {
          if (!mockXHR.isAborted() && mockXHR.onload) {
            mockXHR.onload();
          }
        }, 0);
      });

      await uploadManager.uploadFile(mockFile);

      const headers = mockXHR.getHeaders();
      expect(headers['Authorization']).toBe(`Bearer ${newToken}`);
    });

    it('should update UI session ID via setUiSessionId()', async () => {
      const newSessionId = 'new-session-id-789';
      uploadManager.setUiSessionId(newSessionId);

      const mockFile = new File(['test'], 'test.txt', { type: 'text/plain' });
      const mockResponse: UserFileResponse = {
        id: 'file-xyz',
        filename: 'test.txt',
        mime_type: 'text/plain',
        size: 4,
      };

      mockXHR.status = 200;
      mockXHR.responseText = JSON.stringify(mockResponse);
      mockXHR.send = vi.fn((data: any) => {
        mockXHR.sentData = data;
        setTimeout(() => {
          if (!mockXHR.isAborted() && mockXHR.onload) {
            mockXHR.onload();
          }
        }, 0);
      });

      await uploadManager.uploadFile(mockFile);

      const formData = mockXHR.getSentData() as FormData;
      expect(formData.get('ui_session_id')).toBe(newSessionId);
    });

    it('should use new tokens in subsequent uploads', async () => {
      // Setup send mock that will be reused
      mockXHR.send = vi.fn((data: any) => {
        mockXHR.sentData = data;
        setTimeout(() => {
          if (!mockXHR.isAborted() && mockXHR.onload) {
            mockXHR.onload();
          }
        }, 0);
      });

      // First upload with original tokens
      const mockFile1 = new File(['test 1'], 'test1.txt', { type: 'text/plain' });
      const mockResponse1: UserFileResponse = {
        id: 'file-1',
        filename: 'test1.txt',
        mime_type: 'text/plain',
        size: 6,
      };

      mockXHR.status = 200;
      mockXHR.responseText = JSON.stringify(mockResponse1);

      await uploadManager.uploadFile(mockFile1);

      const headers1 = mockXHR.getHeaders();
      const formData1 = mockXHR.getSentData() as FormData;
      expect(headers1['Authorization']).toBe('Bearer test-auth-token');
      expect(formData1.get('ui_session_id')).toBe('test-session-id');

      // Update tokens
      uploadManager.setAuthToken('updated-token');
      uploadManager.setUiSessionId('updated-session');

      // Second upload with new tokens
      const mockFile2 = new File(['test 2'], 'test2.txt', { type: 'text/plain' });
      const mockResponse2: UserFileResponse = {
        id: 'file-2',
        filename: 'test2.txt',
        mime_type: 'text/plain',
        size: 6,
      };

      mockXHR.status = 200;
      mockXHR.responseText = JSON.stringify(mockResponse2);

      await uploadManager.uploadFile(mockFile2);

      const headers2 = mockXHR.getHeaders();
      const formData2 = mockXHR.getSentData() as FormData;
      expect(headers2['Authorization']).toBe('Bearer updated-token');
      expect(formData2.get('ui_session_id')).toBe('updated-session');
    });
  });

  describe('MIME Type Validation', () => {
    it('should allow upload when no MIME type restrictions', async () => {
      const mockFile = new File(['test'], 'test.pdf', { type: 'application/pdf' });
      const mockResponse: UserFileResponse = {
        id: 'file-pdf',
        filename: 'test.pdf',
        mime_type: 'application/pdf',
        size: 4,
      };

      mockXHR.status = 200;
      mockXHR.responseText = JSON.stringify(mockResponse);
      mockXHR.send = vi.fn((data: any) => {
        mockXHR.sentData = data;
        setTimeout(() => {
          if (!mockXHR.isAborted() && mockXHR.onload) {
            mockXHR.onload();
          }
        }, 0);
      });

      const result = await uploadManager.uploadFile(mockFile);
      expect(result).toEqual(mockResponse);
    });

    it('should reject file with disallowed MIME type', async () => {
      const restrictedManager = new FileUploadManager(
        'https://api.example.com',
        'test-auth-token',
        'test-session-id',
        {
          ...testConfig,
          allowedMimeTypes: ['image/png', 'image/jpeg'],
        }
      );

      const mockFile = new File(['test'], 'test.pdf', { type: 'application/pdf' });

      await expect(restrictedManager.uploadFile(mockFile)).rejects.toThrow(
        /File type application\/pdf is not allowed/
      );
    });

    it('should allow file with allowed MIME type', async () => {
      const restrictedManager = new FileUploadManager(
        'https://api.example.com',
        'test-auth-token',
        'test-session-id',
        {
          ...testConfig,
          allowedMimeTypes: ['image/png', 'image/jpeg'],
        }
      );

      const mockFile = new File(['test'], 'test.png', { type: 'image/png' });
      const mockResponse: UserFileResponse = {
        id: 'file-png',
        filename: 'test.png',
        mime_type: 'image/png',
        size: 4,
      };

      mockXHR.status = 200;
      mockXHR.responseText = JSON.stringify(mockResponse);
      mockXHR.send = vi.fn((data: any) => {
        mockXHR.sentData = data;
        setTimeout(() => {
          if (!mockXHR.isAborted() && mockXHR.onload) {
            mockXHR.onload();
          }
        }, 0);
      });

      const result = await restrictedManager.uploadFile(mockFile);
      expect(result).toEqual(mockResponse);
    });
  });

  describe('URL Building', () => {
    it('should build correct upload URL from API URL', async () => {
      const mockFile = new File(['test'], 'test.txt', { type: 'text/plain' });
      const mockResponse: UserFileResponse = {
        id: 'file-123',
        filename: 'test.txt',
        mime_type: 'text/plain',
        size: 4,
      };

      mockXHR.status = 200;
      mockXHR.responseText = JSON.stringify(mockResponse);
      mockXHR.send = vi.fn((data: any) => {
        mockXHR.sentData = data;
        setTimeout(() => {
          if (!mockXHR.isAborted() && mockXHR.onload) {
            mockXHR.onload();
          }
        }, 0);
      });

      await uploadManager.uploadFile(mockFile);

      expect(mockXHR.getUrl()).toBe('https://api.example.com/api/rt/upload_file');
    });

    it('should handle API URL with trailing slash', async () => {
      const managerWithSlash = new FileUploadManager(
        'https://api.example.com/',
        'test-auth-token',
        'test-session-id',
        testConfig
      );

      const mockFile = new File(['test'], 'test.txt', { type: 'text/plain' });
      const mockResponse: UserFileResponse = {
        id: 'file-123',
        filename: 'test.txt',
        mime_type: 'text/plain',
        size: 4,
      };

      mockXHR.status = 200;
      mockXHR.responseText = JSON.stringify(mockResponse);
      mockXHR.send = vi.fn((data: any) => {
        mockXHR.sentData = data;
        setTimeout(() => {
          if (!mockXHR.isAborted() && mockXHR.onload) {
            mockXHR.onload();
          }
        }, 0);
      });

      await managerWithSlash.uploadFile(mockFile);

      expect(mockXHR.getUrl()).toBe('https://api.example.com/api/rt/upload_file');
    });

    it('should handle API URL with path', async () => {
      const managerWithPath = new FileUploadManager(
        'https://api.example.com/v1',
        'test-auth-token',
        'test-session-id',
        testConfig
      );

      const mockFile = new File(['test'], 'test.txt', { type: 'text/plain' });
      const mockResponse: UserFileResponse = {
        id: 'file-123',
        filename: 'test.txt',
        mime_type: 'text/plain',
        size: 4,
      };

      mockXHR.status = 200;
      mockXHR.responseText = JSON.stringify(mockResponse);
      mockXHR.send = vi.fn((data: any) => {
        mockXHR.sentData = data;
        setTimeout(() => {
          if (!mockXHR.isAborted() && mockXHR.onload) {
            mockXHR.onload();
          }
        }, 0);
      });

      await managerWithPath.uploadFile(mockFile);

      // The URL builder should replace the path with /api/rt/upload_file
      expect(mockXHR.getUrl()).toBe('https://api.example.com/api/rt/upload_file');
    });
  });
});
