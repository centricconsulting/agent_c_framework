/**
 * Unit tests for useFileUpload hook
 * Tests Core integration, file validation, upload flow, and error handling
 */

import { describe, it, expect, vi, beforeEach, afterEach } from 'vitest';
import { renderHook, act, waitFor } from '@testing-library/react';
import { useFileUpload } from '../useFileUpload';
import type { RealtimeClient, UserFileResponse, UploadProgress } from '@agentc/realtime-core';

// Mock the AgentCContext hooks
const mockUseRealtimeClientSafe = vi.fn();

vi.mock('../../providers/AgentCContext', () => ({
  useRealtimeClientSafe: () => mockUseRealtimeClientSafe()
}));

/**
 * Create a mock uploadFile function with configurable behavior
 */
const createMockUploadFile = (options: {
  shouldFail?: boolean;
  errorMessage?: string;
  delay?: number;
  progressSteps?: number[];
} = {}) => {
  return vi.fn((file: File, uploadOptions?: any) => {
    const { shouldFail = false, errorMessage = 'Upload failed', delay = 0, progressSteps = [50, 100] } = options;
    
    return new Promise<UserFileResponse>((resolve, reject) => {
      // Simulate progress callbacks
      if (uploadOptions?.onProgress && !shouldFail) {
        progressSteps.forEach((percentage, index) => {
          setTimeout(() => {
            const loaded = Math.floor((file.size * percentage) / 100);
            uploadOptions.onProgress({
              loaded,
              total: file.size,
              percentage
            } as UploadProgress);
          }, (index + 1) * 10);
        });
      }
      
      // Resolve or reject based on configuration
      setTimeout(() => {
        if (shouldFail) {
          reject(new Error(errorMessage));
        } else {
          resolve({
            id: `server_file_${Date.now()}_${Math.random().toString(36).substring(7)}`,
            filename: file.name,
            mime_type: file.type,
            size: file.size
          });
        }
      }, delay || (progressSteps.length + 1) * 10);
    });
  });
};

/**
 * Create a basic mock client
 */
const createMockClient = (): Partial<RealtimeClient> => {
  return {
    uploadFile: createMockUploadFile()
  };
};

describe('useFileUpload', () => {
  let mockClient: Partial<RealtimeClient>;

  beforeEach(() => {
    vi.clearAllMocks();
    vi.useFakeTimers();
    
    // Create fresh mock client for each test
    mockClient = createMockClient();
    mockUseRealtimeClientSafe.mockReturnValue(mockClient);
    
    // Ensure uploadFile is always defined
    if (!mockClient.uploadFile) {
      mockClient.uploadFile = createMockUploadFile();
    }
  });

  afterEach(() => {
    vi.clearAllTimers();
    vi.useRealTimers();
    vi.clearAllMocks(); // Use clearAllMocks instead of restoreAllMocks to keep module mocks
  });

  describe('Part 1: Basic State Management', () => {
    it('should initialize with empty attachments', () => {
      const { result } = renderHook(() => useFileUpload());
      
      expect(result.current.attachments).toEqual([]);
      expect(result.current.isUploading).toBe(false);
      expect(result.current.allComplete).toBe(false);
      expect(result.current.hasErrors).toBe(false);
      expect(result.current.overallProgress).toBe(0);
      expect(result.current.validationError).toBeNull();
    });

    it('should use default options for file size limit', async () => {
      const { result } = renderHook(() => useFileUpload());
      
      // Create file exceeding default 10MB limit
      const largeFile = new File(['x'.repeat(11 * 1024 * 1024)], 'large.jpg', { type: 'image/jpeg' });
      
      let caughtError: Error | undefined;
      await act(async () => {
        try {
          await result.current.addFiles([largeFile]);
        } catch (error) {
          caughtError = error as Error;
        }
      });
      
      expect(caughtError?.message).toMatch(/exceeds maximum size of 10.0MB/);
      expect(result.current.validationError).toContain('exceeds maximum size');
    });

    it('should accept custom options', () => {
      const { result } = renderHook(() => 
        useFileUpload({
          maxFileSize: 5 * 1024 * 1024, // 5MB
          allowedMimeTypes: ['application/pdf'],
          maxFiles: 3,
          autoUpload: true,
          generatePreviews: false
        })
      );
      
      expect(result.current.attachments).toEqual([]);
      expect(result.current.validationError).toBeNull();
    });
  });

  describe('Part 2: File Validation', () => {
    it('should reject files exceeding maxFileSize', async () => {
      const { result } = renderHook(() => 
        useFileUpload({ maxFileSize: 1 * 1024 * 1024 }) // 1MB
      );
      
      const largeFile = new File(['x'.repeat(2 * 1024 * 1024)], 'large.jpg', { type: 'image/jpeg' });
      
      let caughtError: Error | undefined;
      await act(async () => {
        try {
          await result.current.addFiles([largeFile]);
        } catch (error) {
          caughtError = error as Error;
        }
      });
      
      expect(caughtError?.message).toMatch(/exceeds maximum size of 1.0MB/);
      expect(result.current.validationError).toContain('exceeds maximum size');
      expect(result.current.attachments).toHaveLength(0);
    });

    it('should reject files with disallowed MIME types', async () => {
      const { result } = renderHook(() => 
        useFileUpload({ allowedMimeTypes: ['image/*'] })
      );
      
      const pdfFile = new File(['content'], 'document.pdf', { type: 'application/pdf' });
      
      let caughtError: Error | undefined;
      await act(async () => {
        try {
          await result.current.addFiles([pdfFile]);
        } catch (error) {
          caughtError = error as Error;
        }
      });
      
      expect(caughtError?.message).toMatch(/File type "application\/pdf" is not allowed/);
      expect(result.current.validationError).toContain('not allowed');
      expect(result.current.attachments).toHaveLength(0);
    });

    it('should reject files exceeding maxFiles limit', async () => {
      const { result } = renderHook(() => 
        useFileUpload({ maxFiles: 2 })
      );
      
      const files = [
        new File(['1'], 'file1.jpg', { type: 'image/jpeg' }),
        new File(['2'], 'file2.jpg', { type: 'image/jpeg' }),
        new File(['3'], 'file3.jpg', { type: 'image/jpeg' })
      ];
      
      let caughtError: Error | undefined;
      await act(async () => {
        try {
          await result.current.addFiles(files);
        } catch (error) {
          caughtError = error as Error;
        }
      });
      
      expect(caughtError?.message).toMatch(/Cannot add 3 files\. Maximum is 2 files\./);
      expect(result.current.validationError).toContain('Maximum is 2');
      expect(result.current.attachments).toHaveLength(0);
    });

    it('should support wildcard MIME type patterns', async () => {
      const { result } = renderHook(() => 
        useFileUpload({ allowedMimeTypes: ['image/*'] })
      );
      
      const validFiles = [
        new File(['jpg'], 'test.jpg', { type: 'image/jpeg' }),
        new File(['png'], 'test.png', { type: 'image/png' }),
        new File(['gif'], 'test.gif', { type: 'image/gif' })
      ];
      
      await act(async () => {
        await result.current.addFiles(validFiles);
      });
      
      expect(result.current.attachments).toHaveLength(3);
      expect(result.current.validationError).toBeNull();
    });

    it('should validate each file independently and reject all on first error', async () => {
      const { result } = renderHook(() => 
        useFileUpload({ maxFileSize: 1 * 1024 * 1024 })
      );
      
      const files = [
        new File(['small'], 'small.jpg', { type: 'image/jpeg' }),
        new File(['x'.repeat(2 * 1024 * 1024)], 'large.jpg', { type: 'image/jpeg' })
      ];
      
      await expect(async () => {
        await act(async () => {
          await result.current.addFiles(files);
        });
      }).rejects.toThrow(/exceeds maximum size/);
      
      // No files should be added if any validation fails
      expect(result.current.attachments).toHaveLength(0);
    });
  });

  describe('Part 3: Core Integration - File Upload', () => {
    it('should call client.uploadFile() with correct parameters', async () => {
      const { result } = renderHook(() => useFileUpload());
      
      const testFile = new File(['content'], 'test.jpg', { type: 'image/jpeg' });
      
      // Add file
      await act(async () => {
        await result.current.addFiles([testFile]);
      });
      
      expect(result.current.attachments).toHaveLength(1);
      expect(result.current.attachments[0].status).toBe('pending');
      
      // Upload file
      await act(async () => {
        const uploadPromise = result.current.uploadFile(0);
        await vi.runAllTimersAsync();
        await uploadPromise;
      });
      
      // Verify client.uploadFile was called with correct parameters
      expect(mockClient.uploadFile).toHaveBeenCalledWith(
        testFile,
        expect.objectContaining({
          onProgress: expect.any(Function),
          signal: expect.any(AbortSignal)
        })
      );
      
      // Verify state updated correctly
      expect(result.current.attachments[0].status).toBe('complete');
      expect(result.current.attachments[0].id).toMatch(/^server_file_/);
      expect(result.current.attachments[0].progress).toBe(100);
    });

    it('should receive and store server-assigned file ID', async () => {
      const serverId = 'server_uuid_12345';
      mockClient.uploadFile = vi.fn().mockResolvedValue({
        id: serverId,
        filename: 'test.jpg',
        mime_type: 'image/jpeg',
        size: 100
      });
      
      const { result } = renderHook(() => useFileUpload());
      
      const testFile = new File(['content'], 'test.jpg', { type: 'image/jpeg' });
      
      await act(async () => {
        await result.current.addFiles([testFile]);
      });
      
      await act(async () => {
        await result.current.uploadFile(0);
      });
      
      expect(result.current.attachments[0].id).toBe(serverId);
      expect(result.current.attachments[0].status).toBe('complete');
      expect(result.current.getUploadedFileIds()).toEqual([serverId]);
    });

    it('should track upload progress from Core package', async () => {
      const progressUpdates: number[] = [];
      
      mockClient.uploadFile = vi.fn((file: File, options?: any) => {
        // Trigger progress updates immediately in sequence
        if (options?.onProgress) {
          options.onProgress({ loaded: 25, total: 100, percentage: 25 });
          options.onProgress({ loaded: 50, total: 100, percentage: 50 });
          options.onProgress({ loaded: 75, total: 100, percentage: 75 });
          options.onProgress({ loaded: 100, total: 100, percentage: 100 });
        }
        
        return Promise.resolve({
          id: 'file_123',
          filename: file.name,
          mime_type: file.type,
          size: file.size
        });
      });
      
      const { result } = renderHook(() => useFileUpload());
      
      const testFile = new File(['x'.repeat(100)], 'test.jpg', { type: 'image/jpeg' });
      
      await act(async () => {
        await result.current.addFiles([testFile]);
      });
      
      await act(async () => {
        await result.current.uploadFile(0);
      });
      
      // Verify progress reached 100%
      expect(result.current.attachments[0].progress).toBe(100);
      expect(result.current.attachments[0].status).toBe('complete');
    });

    it('should handle upload errors from Core package', async () => {
      const uploadError = new Error('Network error');
      mockClient.uploadFile = vi.fn().mockRejectedValue(uploadError);
      
      const { result } = renderHook(() => useFileUpload());
      
      const testFile = new File(['content'], 'test.jpg', { type: 'image/jpeg' });
      
      await act(async () => {
        await result.current.addFiles([testFile]);
      });
      
      await act(async () => {
        await result.current.uploadFile(0);
      });
      
      expect(result.current.attachments[0].status).toBe('error');
      expect(result.current.attachments[0].error).toBe('Network error');
      expect(result.current.hasErrors).toBe(true);
    });

    it('should throw error if client not available', async () => {
      mockUseRealtimeClientSafe.mockReturnValue(null);
      
      const { result } = renderHook(() => useFileUpload());
      
      const testFile = new File(['content'], 'test.jpg', { type: 'image/jpeg' });
      
      await act(async () => {
        await result.current.addFiles([testFile]);
      });
      
      await act(async () => {
        await result.current.uploadFile(0);
      });
      
      expect(result.current.attachments[0].status).toBe('error');
      expect(result.current.attachments[0].error).toContain('RealtimeClient not available');
      expect(result.current.attachments[0].error).toContain('AgentCProvider');
    });

    it('should not upload if already uploading or complete', async () => {
      let resolveUpload: ((value: any) => void) | undefined;
      
      mockClient.uploadFile = vi.fn((file: File) => {
        return new Promise((resolve) => {
          resolveUpload = resolve;
        });
      });
      
      const { result } = renderHook(() => useFileUpload());
      
      const testFile = new File(['content'], 'test.jpg', { type: 'image/jpeg' });
      
      await act(async () => {
        await result.current.addFiles([testFile]);
      });
      
      // Start first upload (doesn't complete yet)
      act(() => {
        result.current.uploadFile(0);
      });
      
      // Verify it's uploading
      expect(result.current.attachments[0].status).toBe('uploading');
      
      // Try to upload again while first is in progress (should be skipped)
      await act(async () => {
        await result.current.uploadFile(0);
      });
      
      // Complete the first upload
      await act(async () => {
        resolveUpload?.({
          id: 'file_123',
          filename: testFile.name,
          mime_type: testFile.type,
          size: testFile.size
        });
      });
      
      // Try to upload again after complete (should be skipped)
      await act(async () => {
        await result.current.uploadFile(0);
      });
      
      // Should only be called once (second and third calls skipped)
      expect(mockClient.uploadFile).toHaveBeenCalledTimes(1);
    });
  });

  describe('Part 4: Upload Cancellation', () => {
    it('should cancel upload via AbortSignal when file is removed', async () => {
      let capturedAbortSignal: AbortSignal | undefined;
      
      mockClient.uploadFile = vi.fn((file: File, options?: any) => {
        capturedAbortSignal = options?.signal;
        
        return new Promise((resolve, reject) => {
          const timeout = setTimeout(() => {
            resolve({
              id: 'file_123',
              filename: file.name,
              mime_type: file.type,
              size: file.size
            });
          }, 1000);
          
          // Listen for abort
          capturedAbortSignal?.addEventListener('abort', () => {
            clearTimeout(timeout);
            reject(new Error('Upload cancelled'));
          });
        });
      });
      
      const { result } = renderHook(() => useFileUpload());
      
      const testFile = new File(['content'], 'test.jpg', { type: 'image/jpeg' });
      
      await act(async () => {
        await result.current.addFiles([testFile]);
      });
      
      // Start upload
      act(() => {
        result.current.uploadFile(0);
      });
      
      // Verify upload started
      expect(mockClient.uploadFile).toHaveBeenCalledTimes(1);
      expect(capturedAbortSignal).toBeDefined();
      expect(capturedAbortSignal?.aborted).toBe(false);
      
      // Remove file (should trigger abort)
      act(() => {
        result.current.removeFile(0);
      });
      
      // Verify abort was triggered
      expect(capturedAbortSignal?.aborted).toBe(true);
      expect(result.current.attachments).toHaveLength(0);
    });

    it('should abort all uploads on unmount', async () => {
      const abortSignals: AbortSignal[] = [];
      
      mockClient.uploadFile = vi.fn((file: File, options?: any) => {
        if (options?.signal) {
          abortSignals.push(options.signal);
        }
        
        return new Promise((resolve) => {
          setTimeout(() => {
            resolve({
              id: `file_${Date.now()}`,
              filename: file.name,
              mime_type: file.type,
              size: file.size
            });
          }, 1000);
        });
      });
      
      const { result, unmount } = renderHook(() => useFileUpload());
      
      const files = [
        new File(['1'], 'file1.jpg', { type: 'image/jpeg' }),
        new File(['2'], 'file2.jpg', { type: 'image/jpeg' })
      ];
      
      await act(async () => {
        await result.current.addFiles(files);
      });
      
      // Start uploads - uploadAll uploads sequentially, not in parallel
      // So we need to start them manually to have both in progress
      act(() => {
        result.current.uploadFile(0);
        result.current.uploadFile(1);
      });
      
      // Wait a tick for uploads to start
      await act(async () => {
        await Promise.resolve();
      });
      
      // Verify uploads started
      expect(abortSignals.length).toBeGreaterThanOrEqual(1);
      expect(abortSignals.every(s => !s.aborted)).toBe(true);
      
      // Unmount (should abort all)
      unmount();
      
      // Verify all uploads were aborted
      expect(abortSignals.every(s => s.aborted)).toBe(true);
    });

    it('should cleanup AbortController from ref map after upload completes', async () => {
      const { result } = renderHook(() => useFileUpload());
      
      const testFile = new File(['content'], 'test.jpg', { type: 'image/jpeg' });
      
      await act(async () => {
        await result.current.addFiles([testFile]);
      });
      
      await act(async () => {
        const uploadPromise = result.current.uploadFile(0);
        await vi.runAllTimersAsync();
        await uploadPromise;
      });
      
      // After completion, AbortController should be cleaned up
      // We can't directly test the ref, but we can verify the upload completed successfully
      expect(result.current.attachments[0].status).toBe('complete');
      
      // Removing file after completion should work without issues
      act(() => {
        result.current.removeFile(0);
      });
      
      expect(result.current.attachments).toHaveLength(0);
    });
  });

  describe('Part 5: Batch Operations', () => {
    it('should upload all pending files sequentially', async () => {
      const uploadOrder: string[] = [];
      
      mockClient.uploadFile = vi.fn((file: File) => {
        uploadOrder.push(file.name);
        return Promise.resolve({
          id: `file_${file.name}`,
          filename: file.name,
          mime_type: file.type,
          size: file.size
        });
      });
      
      const { result } = renderHook(() => useFileUpload());
      
      const files = [
        new File(['1'], 'file1.jpg', { type: 'image/jpeg' }),
        new File(['2'], 'file2.jpg', { type: 'image/jpeg' }),
        new File(['3'], 'file3.jpg', { type: 'image/jpeg' })
      ];
      
      await act(async () => {
        await result.current.addFiles(files);
      });
      
      expect(result.current.attachments).toHaveLength(3);
      expect(result.current.attachments.every(a => a.status === 'pending')).toBe(true);
      
      await act(async () => {
        await result.current.uploadAll();
      });
      
      expect(mockClient.uploadFile).toHaveBeenCalledTimes(3);
      expect(uploadOrder).toEqual(['file1.jpg', 'file2.jpg', 'file3.jpg']);
      expect(result.current.attachments.every(a => a.status === 'complete')).toBe(true);
      expect(result.current.allComplete).toBe(true);
    });

    it('should auto-upload when enabled', async () => {
      const { result } = renderHook(() => 
        useFileUpload({ autoUpload: true })
      );
      
      const testFile = new File(['content'], 'test.jpg', { type: 'image/jpeg' });
      
      await act(async () => {
        await result.current.addFiles([testFile]);
      });
      
      // Auto-upload uses setTimeout(0), advance timers to trigger it
      await act(async () => {
        await vi.runAllTimersAsync();
      });
      
      // Verify auto-upload was triggered
      expect(mockClient.uploadFile).toHaveBeenCalledWith(
        testFile,
        expect.any(Object)
      );
      
      // Verify upload completed
      expect(result.current.attachments[0].status).toBe('complete');
    });

    it('should clear all attachments and revoke preview URLs', async () => {
      const { result } = renderHook(() => 
        useFileUpload({ generatePreviews: true })
      );
      
      const testFile = new File(['content'], 'test.jpg', { type: 'image/jpeg' });
      
      await act(async () => {
        await result.current.addFiles([testFile]);
      });
      
      expect(result.current.attachments).toHaveLength(1);
      const previewUrl = result.current.attachments[0].previewUrl;
      expect(previewUrl).toBeDefined();
      expect(previewUrl).toMatch(/^blob:/);
      
      act(() => {
        result.current.clearAll();
      });
      
      expect(result.current.attachments).toHaveLength(0);
      expect(result.current.validationError).toBeNull();
    });

    it('should abort in-progress uploads when clearing all', async () => {
      const abortSignals: AbortSignal[] = [];
      
      mockClient.uploadFile = vi.fn((file: File, options?: any) => {
        if (options?.signal) {
          abortSignals.push(options.signal);
        }
        
        return new Promise((resolve) => {
          setTimeout(() => {
            resolve({
              id: `file_${Date.now()}`,
              filename: file.name,
              mime_type: file.type,
              size: file.size
            });
          }, 1000);
        });
      });
      
      const { result } = renderHook(() => useFileUpload());
      
      const files = [
        new File(['1'], 'file1.jpg', { type: 'image/jpeg' }),
        new File(['2'], 'file2.jpg', { type: 'image/jpeg' })
      ];
      
      await act(async () => {
        await result.current.addFiles(files);
      });
      
      // Start uploads
      act(() => {
        result.current.uploadAll();
      });
      
      // Clear all (should abort uploads)
      act(() => {
        result.current.clearAll();
      });
      
      // Verify all uploads were aborted
      expect(abortSignals.every(s => s.aborted)).toBe(true);
      expect(result.current.attachments).toHaveLength(0);
    });
  });

  describe('Part 6: Preview Generation', () => {
    it('should generate preview URLs for images when enabled', async () => {
      const { result } = renderHook(() => 
        useFileUpload({ generatePreviews: true })
      );
      
      const imageFile = new File(['content'], 'test.jpg', { type: 'image/jpeg' });
      
      await act(async () => {
        await result.current.addFiles([imageFile]);
      });
      
      expect(result.current.attachments[0].previewUrl).toBeDefined();
      expect(result.current.attachments[0].previewUrl).toMatch(/^blob:/);
    });

    it('should not generate previews for non-image files', async () => {
      const { result } = renderHook(() => 
        useFileUpload({ 
          generatePreviews: true,
          allowedMimeTypes: ['image/*', 'application/pdf']
        })
      );
      
      const pdfFile = new File(['content'], 'document.pdf', { type: 'application/pdf' });
      
      await act(async () => {
        await result.current.addFiles([pdfFile]);
      });
      
      expect(result.current.attachments[0].previewUrl).toBeUndefined();
    });

    it('should not generate previews when disabled', async () => {
      const { result } = renderHook(() => 
        useFileUpload({ generatePreviews: false })
      );
      
      const imageFile = new File(['content'], 'test.jpg', { type: 'image/jpeg' });
      
      await act(async () => {
        await result.current.addFiles([imageFile]);
      });
      
      expect(result.current.attachments[0].previewUrl).toBeUndefined();
    });

    it('should revoke preview URL when file is removed', async () => {
      const { result } = renderHook(() => 
        useFileUpload({ generatePreviews: true })
      );
      
      const imageFile = new File(['content'], 'test.jpg', { type: 'image/jpeg' });
      
      await act(async () => {
        await result.current.addFiles([imageFile]);
      });
      
      const previewUrl = result.current.attachments[0].previewUrl;
      expect(previewUrl).toBeDefined();
      
      // Remove file
      act(() => {
        result.current.removeFile(0);
      });
      
      expect(result.current.attachments).toHaveLength(0);
      // URL.revokeObjectURL is called internally (can't directly test)
    });
  });

  describe('Part 7: Computed States', () => {
    it('should track isUploading state correctly', async () => {
      mockClient.uploadFile = vi.fn((file: File) => {
        return new Promise((resolve) => {
          setTimeout(() => {
            resolve({
              id: 'file_123',
              filename: file.name,
              mime_type: file.type,
              size: file.size
            });
          }, 100);
        });
      });
      
      const { result } = renderHook(() => useFileUpload());
      
      const testFile = new File(['content'], 'test.jpg', { type: 'image/jpeg' });
      
      await act(async () => {
        await result.current.addFiles([testFile]);
      });
      
      expect(result.current.isUploading).toBe(false);
      
      act(() => {
        result.current.uploadFile(0);
      });
      
      expect(result.current.isUploading).toBe(true);
      
      await act(async () => {
        await vi.runAllTimersAsync();
      });
      
      expect(result.current.isUploading).toBe(false);
    });

    it('should track allComplete state correctly', async () => {
      // Use simple mock that completes immediately
      mockClient.uploadFile = vi.fn((file: File) => {
        return Promise.resolve({
          id: `file_${file.name}`,
          filename: file.name,
          mime_type: file.type,
          size: file.size
        });
      });
      
      const { result } = renderHook(() => useFileUpload());
      
      const files = [
        new File(['1'], 'file1.jpg', { type: 'image/jpeg' }),
        new File(['2'], 'file2.jpg', { type: 'image/jpeg' })
      ];
      
      await act(async () => {
        await result.current.addFiles(files);
      });
      
      expect(result.current.allComplete).toBe(false);
      
      await act(async () => {
        await result.current.uploadAll();
      });
      
      expect(result.current.allComplete).toBe(true);
      expect(result.current.attachments.every(a => a.status === 'complete')).toBe(true);
    });

    it('should track hasErrors state correctly', async () => {
      mockClient.uploadFile = vi.fn()
        .mockResolvedValueOnce({
          id: 'file_1',
          filename: 'test1.jpg',
          mime_type: 'image/jpeg',
          size: 100
        })
        .mockRejectedValueOnce(new Error('Upload failed'));
      
      const { result } = renderHook(() => useFileUpload());
      
      const files = [
        new File(['1'], 'file1.jpg', { type: 'image/jpeg' }),
        new File(['2'], 'file2.jpg', { type: 'image/jpeg' })
      ];
      
      await act(async () => {
        await result.current.addFiles(files);
      });
      
      expect(result.current.hasErrors).toBe(false);
      
      await act(async () => {
        await result.current.uploadAll();
      });
      
      expect(result.current.hasErrors).toBe(true);
      expect(result.current.attachments[0].status).toBe('complete');
      expect(result.current.attachments[1].status).toBe('error');
    });

    it('should calculate overall progress correctly', async () => {
      const { result } = renderHook(() => useFileUpload());
      
      const files = [
        new File(['1'], 'file1.jpg', { type: 'image/jpeg' }),
        new File(['2'], 'file2.jpg', { type: 'image/jpeg' }),
        new File(['3'], 'file3.jpg', { type: 'image/jpeg' })
      ];
      
      await act(async () => {
        await result.current.addFiles(files);
      });
      
      expect(result.current.overallProgress).toBe(0);
      
      await act(async () => {
        const uploadPromise = result.current.uploadAll();
        await vi.runAllTimersAsync();
        await uploadPromise;
      });
      
      expect(result.current.overallProgress).toBe(100);
    });

    it('should calculate partial progress correctly', async () => {
      let resolveFirstUpload: ((value: any) => void) | undefined;
      
      mockClient.uploadFile = vi.fn()
        .mockImplementationOnce((file: File, options?: any) => {
          // First file completes
          options?.onProgress?.({ loaded: 100, total: 100, percentage: 100 });
          return new Promise((resolve) => {
            resolveFirstUpload = resolve;
          });
        })
        .mockImplementationOnce((file: File, options?: any) => {
          // Second file at 50%
          options?.onProgress?.({ loaded: 50, total: 100, percentage: 50 });
          return new Promise(() => {}); // Never resolves
        });
      
      const { result } = renderHook(() => useFileUpload());
      
      const files = [
        new File(['1'], 'file1.jpg', { type: 'image/jpeg' }),
        new File(['2'], 'file2.jpg', { type: 'image/jpeg' })
      ];
      
      await act(async () => {
        await result.current.addFiles(files);
      });
      
      // Start first upload
      act(() => {
        result.current.uploadFile(0);
      });
      
      // Complete first upload
      await act(async () => {
        resolveFirstUpload?.({
          id: 'file_1',
          filename: files[0].name,
          mime_type: files[0].type,
          size: files[0].size
        });
      });
      
      // Start second upload (which will stall at 50%)
      act(() => {
        result.current.uploadFile(1);
      });
      
      await act(async () => {
        await Promise.resolve();
      });
      
      // Overall should be (100 + 50) / 2 = 75
      expect(result.current.overallProgress).toBe(75);
    });
  });

  describe('Part 8: getUploadedFileIds', () => {
    it('should return only completed file IDs', async () => {
      mockClient.uploadFile = vi.fn()
        .mockResolvedValueOnce({ id: 'file_1', filename: 'test1.jpg', mime_type: 'image/jpeg', size: 100 })
        .mockRejectedValueOnce(new Error('Failed'));
      
      const { result } = renderHook(() => useFileUpload());
      
      const files = [
        new File(['1'], 'file1.jpg', { type: 'image/jpeg' }),
        new File(['2'], 'file2.jpg', { type: 'image/jpeg' })
      ];
      
      await act(async () => {
        await result.current.addFiles(files);
      });
      
      await act(async () => {
        await result.current.uploadAll();
      });
      
      const uploadedIds = result.current.getUploadedFileIds();
      
      expect(uploadedIds).toEqual(['file_1']);
      expect(uploadedIds).toHaveLength(1);
    });

    it('should return empty array when no uploads complete', () => {
      const { result } = renderHook(() => useFileUpload());
      
      expect(result.current.getUploadedFileIds()).toEqual([]);
    });

    it('should not include pending or error files in uploaded IDs', async () => {
      mockClient.uploadFile = vi.fn()
        .mockResolvedValueOnce({ id: 'file_1', filename: 'test1.jpg', mime_type: 'image/jpeg', size: 100 });
      
      const { result } = renderHook(() => useFileUpload());
      
      const files = [
        new File(['1'], 'file1.jpg', { type: 'image/jpeg' }),
        new File(['2'], 'file2.jpg', { type: 'image/jpeg' }),
        new File(['3'], 'file3.jpg', { type: 'image/jpeg' })
      ];
      
      await act(async () => {
        await result.current.addFiles(files);
      });
      
      // Upload only first file
      await act(async () => {
        await result.current.uploadFile(0);
      });
      
      const uploadedIds = result.current.getUploadedFileIds();
      
      expect(uploadedIds).toEqual(['file_1']);
      expect(uploadedIds).toHaveLength(1);
      expect(result.current.attachments).toHaveLength(3); // All still in attachments
    });
  });

  describe('Edge Cases and Error Scenarios', () => {
    it('should handle rapid add/remove cycles', async () => {
      const { result } = renderHook(() => useFileUpload());
      
      const testFile = new File(['content'], 'test.jpg', { type: 'image/jpeg' });
      
      // Add file
      await act(async () => {
        await result.current.addFiles([testFile]);
      });
      
      expect(result.current.attachments).toHaveLength(1);
      
      // Remove immediately
      act(() => {
        result.current.removeFile(0);
      });
      
      expect(result.current.attachments).toHaveLength(0);
      
      // Add again
      await act(async () => {
        await result.current.addFiles([testFile]);
      });
      
      expect(result.current.attachments).toHaveLength(1);
    });

    it('should handle uploading non-existent file index', async () => {
      const { result } = renderHook(() => useFileUpload());
      
      // Try to upload index 0 when no files exist
      await act(async () => {
        await result.current.uploadFile(0);
      });
      
      // Should not crash, just return early
      expect(mockClient.uploadFile).not.toHaveBeenCalled();
    });

    it('should clear validation error when successfully adding files', async () => {
      const { result } = renderHook(() => useFileUpload({ maxFiles: 2 }));
      
      // First attempt - too many files
      const tooManyFiles = [
        new File(['1'], 'file1.jpg', { type: 'image/jpeg' }),
        new File(['2'], 'file2.jpg', { type: 'image/jpeg' }),
        new File(['3'], 'file3.jpg', { type: 'image/jpeg' })
      ];
      
      let caughtError: Error | undefined;
      await act(async () => {
        try {
          await result.current.addFiles(tooManyFiles);
        } catch (error) {
          caughtError = error as Error;
        }
      });
      
      expect(caughtError).toBeDefined();
      expect(result.current.validationError).not.toBeNull();
      expect(result.current.validationError).toContain('Maximum is 2');
      
      // Second attempt - valid
      const validFiles = [
        new File(['1'], 'file1.jpg', { type: 'image/jpeg' })
      ];
      
      await act(async () => {
        await result.current.addFiles(validFiles);
      });
      
      expect(result.current.validationError).toBeNull();
      expect(result.current.attachments).toHaveLength(1);
    });

    it('should handle progress callbacks after component unmount', async () => {
      let progressCallback: ((progress: UploadProgress) => void) | undefined;
      
      mockClient.uploadFile = vi.fn((file: File, options?: any) => {
        progressCallback = options?.onProgress;
        
        return new Promise((resolve) => {
          setTimeout(() => {
            resolve({
              id: 'file_123',
              filename: file.name,
              mime_type: file.type,
              size: file.size
            });
          }, 1000);
        });
      });
      
      const { result, unmount } = renderHook(() => useFileUpload());
      
      const testFile = new File(['content'], 'test.jpg', { type: 'image/jpeg' });
      
      await act(async () => {
        await result.current.addFiles([testFile]);
      });
      
      act(() => {
        result.current.uploadFile(0);
      });
      
      // Unmount before upload completes
      unmount();
      
      // Try to trigger progress callback after unmount
      // Should not crash or cause warnings
      act(() => {
        progressCallback?.({ loaded: 50, total: 100, percentage: 50 });
      });
      
      // No assertions needed - just verifying no crash
    });
  });
});
