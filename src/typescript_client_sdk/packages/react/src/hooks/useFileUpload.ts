/**
 * useFileUpload - React hook for file upload state management
 * Provides interface for adding, validating, and uploading files
 */

import { useEffect, useState, useCallback, useRef, useMemo } from 'react';
import { useRealtimeClientSafe } from '../providers/AgentCContext';
import type { FileAttachment, UseFileUploadOptions, UseFileUploadReturn } from '../types/chat';

// Re-export types for convenience
export type { FileAttachment, UseFileUploadOptions, UseFileUploadReturn };

/**
 * Default options for the hook
 */
const DEFAULT_OPTIONS: Required<UseFileUploadOptions> = {
  maxFileSize: 10 * 1024 * 1024, // 10MB
  allowedMimeTypes: ['image/*'],
  maxFiles: 5,
  autoUpload: false,
  generatePreviews: true,
};

/**
 * React hook for file upload state management
 * Manages file attachments, validation, upload state, and progress tracking
 */
export function useFileUpload(options: UseFileUploadOptions = {}): UseFileUploadReturn {
  // Merge options with defaults
  const config = useMemo(() => ({ ...DEFAULT_OPTIONS, ...options }), [options]);
  
  // Get client for upload API integration
  const client = useRealtimeClientSafe();
  
  // State
  const [attachments, setAttachments] = useState<FileAttachment[]>([]);
  const [validationError, setValidationError] = useState<string | null>(null);
  
  // Refs for cleanup
  const abortControllersRef = useRef<Map<number, AbortController>>(new Map());
  const previewUrlsRef = useRef<Set<string>>(new Set());
  const isMountedRef = useRef(true);
  
  /**
   * Validate a single file against constraints
   */
  const validateFile = useCallback(
    (file: File): { valid: boolean; error?: string } => {
      // Check file size
      if (file.size > config.maxFileSize) {
        const sizeMB = (config.maxFileSize / (1024 * 1024)).toFixed(1);
        return {
          valid: false,
          error: `File "${file.name}" exceeds maximum size of ${sizeMB}MB`,
        };
      }
      
      // Check MIME type
      const isAllowed = config.allowedMimeTypes.some((pattern) => {
        if (pattern.endsWith('/*')) {
          const prefix = pattern.slice(0, -2);
          return file.type.startsWith(prefix);
        }
        return file.type === pattern;
      });
      
      if (!isAllowed) {
        return {
          valid: false,
          error: `File type "${file.type}" is not allowed`,
        };
      }
      
      return { valid: true };
    },
    [config.maxFileSize, config.allowedMimeTypes]
  );
  
  /**
   * Generate preview URL for an image file
   */
  const generatePreviewUrl = useCallback(
    (file: File): string | undefined => {
      if (!config.generatePreviews) return undefined;
      
      // Only generate previews for images
      if (!file.type.startsWith('image/')) return undefined;
      
      const url = URL.createObjectURL(file);
      previewUrlsRef.current.add(url);
      return url;
    },
    [config.generatePreviews]
  );
  
  /**
   * Revoke a preview URL and remove from tracking
   */
  const revokePreviewUrl = useCallback((url: string | undefined) => {
    if (url && previewUrlsRef.current.has(url)) {
      URL.revokeObjectURL(url);
      previewUrlsRef.current.delete(url);
    }
  }, []);
  
  /**
   * Upload a single file to the server using the Core package's uploadFile method
   */
  const performUpload = useCallback(
    async (
      file: File,
      index: number,
      onProgress: (progress: number) => void
    ): Promise<string> => {
      // Verify client is available
      if (!client) {
        throw new Error('RealtimeClient not available. Ensure AgentCProvider is wrapping this component.');
      }
      
      // Create abort controller for this upload
      const abortController = new AbortController();
      abortControllersRef.current.set(index, abortController);
      
      try {
        // Call Core package's uploadFile method with progress tracking
        const result = await client.uploadFile(file, {
          onProgress: (progressData) => {
            // Convert UploadProgress to percentage (0-100)
            onProgress(progressData.percentage);
          },
          signal: abortController.signal
        });
        
        // Return the server-assigned file ID
        return result.id;
      } finally {
        // Cleanup abort controller
        abortControllersRef.current.delete(index);
      }
    },
    [client]
  );
  
  /**
   * Upload a specific file by index
   */
  const uploadFile = useCallback(
    async (index: number): Promise<void> => {
      console.log('[useFileUpload] uploadFile called for index:', index);
      if (!isMountedRef.current) {
        console.log('[useFileUpload] Not mounted, returning');
        return;
      }
      
      // Check current attachment - need to depend on attachments for this
      const attachment = attachments[index];
      console.log('[useFileUpload] Current attachment:', attachment ? { status: attachment.status, name: attachment.file.name } : 'NOT FOUND');
      
      if (!attachment) {
        console.warn(`[useFileUpload] No attachment at index ${index}`);
        return;
      }
      
      // Skip if already uploading or complete
      if (attachment.status === 'uploading' || attachment.status === 'complete') {
        console.log('[useFileUpload] Skipping - already', attachment.status);
        return;
      }
      
      console.log('[useFileUpload] Will upload:', attachment.file.name);
      const file = attachment.file;
      
      // Update status to uploading
      setAttachments((prev) => {
        const updated = [...prev];
        if (updated[index]) {
          updated[index] = { 
            ...updated[index], 
            status: 'uploading' as const, 
            progress: 0 
          };
        }
        return updated;
      });
      
      console.log('[useFileUpload] Status updated to uploading, starting performUpload...');
      
      try {
        console.log('[useFileUpload] Calling performUpload...');
        const uploadedId = await performUpload(
          file,
          index,
          (progress) => {
            console.log('[useFileUpload] Progress update:', progress, '%');
            if (!isMountedRef.current) return;
            
            setAttachments((prev) => {
              const updated = [...prev];
              if (updated[index]) {
                updated[index] = { ...updated[index], progress };
              }
              return updated;
            });
          }
        );
        
        console.log('[useFileUpload] Upload complete! File ID:', uploadedId);
        if (!isMountedRef.current) return;
        
        // Update status to complete with uploaded ID
        setAttachments((prev) => {
          const updated = [...prev];
          if (updated[index]) {
            updated[index] = {
              ...updated[index],
              id: uploadedId,
              status: 'complete',
              progress: 100,
              error: undefined,
            };
          }
          return updated;
        });
      } catch (error) {
        console.error('[useFileUpload] Upload error:', error);
        if (!isMountedRef.current) return;
        
        const errorMessage = error instanceof Error ? error.message : 'Upload failed';
        console.log('[useFileUpload] Setting error status:', errorMessage);
        
        // Update status to error
        setAttachments((prev) => {
          const updated = [...prev];
          if (updated[index]) {
            updated[index] = {
              ...updated[index],
              status: 'error',
              error: errorMessage,
            };
          }
          return updated;
        });
      }
    },
    [attachments, performUpload]
  );
  
  /**
   * Auto-upload effect - uploads pending files when autoUpload is enabled
   * This runs after state updates to avoid closure issues
   */
  useEffect(() => {
    console.log('[useFileUpload] Auto-upload effect triggered');
    console.log('[useFileUpload]   config.autoUpload:', config.autoUpload);
    console.log('[useFileUpload]   attachments:', attachments.map(a => ({ status: a.status, name: a.file.name })));
    
    if (!config.autoUpload) {
      console.log('[useFileUpload] Auto-upload disabled, returning');
      return;
    }
    if (!isMountedRef.current) {
      console.log('[useFileUpload] Not mounted, returning');
      return;
    }
    
    const pendingIndices = attachments
      .map((att, index) => ({ att, index }))
      .filter(({ att }) => att.status === 'pending')
      .map(({ index }) => index);
    
    console.log('[useFileUpload] Pending file indices:', pendingIndices);
    
    if (pendingIndices.length === 0) {
      console.log('[useFileUpload] No pending files, returning');
      return;
    }
    
    // Upload each pending file - call uploadFile directly, not through function dependency
    console.log('[useFileUpload] Starting uploads for', pendingIndices.length, 'files');
    pendingIndices.forEach(index => {
      uploadFile(index);
    });
  }, [attachments, config.autoUpload, uploadFile]);
  
  /**
   * Upload all pending files sequentially
   */
  const uploadAll = useCallback(async (): Promise<void> => {
    const pendingIndices = attachments
      .map((att, index) => ({ att, index }))
      .filter(({ att }) => att.status === 'pending')
      .map(({ index }) => index);
    
    // Upload files sequentially to avoid overwhelming the server
    for (const index of pendingIndices) {
      await uploadFile(index);
    }
  }, [attachments, uploadFile]);
  
  /**
   * Add files to attachments list after validation
   */
  const addFiles = useCallback(
    async (files: File[]): Promise<void> => {
      console.log('[useFileUpload] addFiles called with', files.length, 'files:', files.map(f => f.name));
      setValidationError(null);
      
      // Check if adding these files would exceed max files
      if (attachments.length + files.length > config.maxFiles) {
        const error = `Cannot add ${files.length} files. Maximum is ${config.maxFiles} files.`;
        setValidationError(error);
        throw new Error(error);
      }
      
      // Validate all files first
      const validatedFiles: Array<{ file: File; previewUrl?: string }> = [];
      
      for (const file of files) {
        const validation = validateFile(file);
        
        if (!validation.valid) {
          setValidationError(validation.error || 'Validation failed');
          throw new Error(validation.error || 'Validation failed');
        }
        
        // Generate preview URL if applicable
        const previewUrl = generatePreviewUrl(file);
        validatedFiles.push({ file, previewUrl });
      }
      
      // All files valid - add them as attachments
      const newAttachments: FileAttachment[] = validatedFiles.map(({ file, previewUrl }) => ({
        file,
        id: null,
        status: 'pending',
        progress: 0,
        previewUrl,
      }));
      
      setAttachments((prev) => [...prev, ...newAttachments]);
      
      // Auto-upload is handled by the useEffect above
      // This avoids closure issues with stale uploadFile references
    },
    [attachments.length, config.maxFiles, validateFile, generatePreviewUrl]
  );
  
  /**
   * Remove a file by index and cleanup resources
   */
  const removeFile = useCallback((index: number): void => {
    const attachment = attachments[index];
    if (!attachment) return;
    
    // Abort upload if in progress
    const abortController = abortControllersRef.current.get(index);
    if (abortController) {
      abortController.abort();
      abortControllersRef.current.delete(index);
    }
    
    // Revoke preview URL
    revokePreviewUrl(attachment.previewUrl);
    
    // Remove from attachments
    setAttachments((prev) => prev.filter((_, i) => i !== index));
    
    // Clear validation error
    setValidationError(null);
  }, [attachments, revokePreviewUrl]);
  
  /**
   * Clear all attachments and cleanup resources
   */
  const clearAll = useCallback((): void => {
    // Abort all uploads
    abortControllersRef.current.forEach((controller) => controller.abort());
    abortControllersRef.current.clear();
    
    // Revoke all preview URLs
    attachments.forEach((att) => revokePreviewUrl(att.previewUrl));
    
    // Clear attachments
    setAttachments([]);
    setValidationError(null);
  }, [attachments, revokePreviewUrl]);
  
  /**
   * Get array of successfully uploaded file IDs
   */
  const getUploadedFileIds = useCallback((): string[] => {
    return attachments
      .filter((att) => att.status === 'complete' && att.id !== null)
      .map((att) => att.id as string);
  }, [attachments]);
  
  // Computed states
  const isUploading = useMemo(
    () => attachments.some((att) => att.status === 'uploading'),
    [attachments]
  );
  
  const allComplete = useMemo(
    () => attachments.length > 0 && attachments.every((att) => att.status === 'complete'),
    [attachments]
  );
  
  const hasErrors = useMemo(
    () => attachments.some((att) => att.status === 'error'),
    [attachments]
  );
  
  const overallProgress = useMemo(() => {
    if (attachments.length === 0) return 0;
    
    const totalProgress = attachments.reduce((sum, att) => sum + att.progress, 0);
    return Math.round(totalProgress / attachments.length);
  }, [attachments]);
  
  // Cleanup on unmount
  useEffect(() => {
    isMountedRef.current = true;
    
    return () => {
      isMountedRef.current = false;
      
      // Abort all uploads
      abortControllersRef.current.forEach((controller) => controller.abort());
      abortControllersRef.current.clear();
      
      // Revoke all preview URLs
      previewUrlsRef.current.forEach((url) => URL.revokeObjectURL(url));
      previewUrlsRef.current.clear();
    };
  }, []);
  
  return {
    attachments,
    addFiles,
    removeFile,
    uploadAll,
    uploadFile,
    clearAll,
    getUploadedFileIds,
    isUploading,
    allComplete,
    hasErrors,
    overallProgress,
    validationError,
  };
}
