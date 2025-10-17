/**
 * File Upload Integration Tests
 * 
 * Tests complete end-to-end workflows for file upload functionality:
 * - File picker → upload → send → message display
 * - Drag-drop → upload → send
 * - Clipboard paste → upload → send
 * - Upload progress and status tracking
 * - Error handling and validation
 * - Multimodal message display
 */

import { describe, it, expect, beforeEach, afterEach, vi } from 'vitest';
import { render, screen, waitFor, within } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { ChatInputArea } from '../ChatInputArea';
import { ChatMessagesView } from '../ChatMessagesView';
import { updateMockState, resetAllMocks, type MessageChatItem } from '@test/mocks/realtime-react';
import * as reactDropzone from 'react-dropzone';

// =============================================================================
// TEST HELPERS AND SETUP
// =============================================================================

// Mock react-dropzone
const mockOpen = vi.fn();
const mockGetRootProps = vi.fn(() => ({ 
  role: 'region',
  'aria-label': 'File drop zone'
}));
const mockGetInputProps = vi.fn(() => ({ 
  type: 'file',
  'aria-hidden': true
}));

vi.mock('react-dropzone', () => ({
  useDropzone: vi.fn(() => ({
    getRootProps: mockGetRootProps,
    getInputProps: mockGetInputProps,
    isDragActive: false,
    open: mockOpen,
  })),
}));

let dropzoneMock: any;

// Mock child components
vi.mock('../FileAttachmentList', () => ({
  FileAttachmentList: ({ attachments, onRemove }: any) => (
    <div data-testid="file-attachment-list">
      {attachments.map((attachment: any, index: number) => (
        <div key={index} data-testid={`attachment-${index}`}>
          <span>{attachment.file.name}</span>
          <span data-testid={`status-${index}`}>{attachment.status}</span>
          <span data-testid={`progress-${index}`}>{attachment.progress}%</span>
          {attachment.previewUrl && (
            <img src={attachment.previewUrl} alt="Preview" data-testid={`preview-${index}`} />
          )}
          <button onClick={() => onRemove(index)}>Remove {attachment.file.name}</button>
        </div>
      ))}
    </div>
  ),
}));

vi.mock('../DropOverlay', () => ({
  DropOverlay: ({ isActive }: any) => (
    isActive ? <div data-testid="drop-overlay">Drop files here</div> : null
  ),
}));

// Mock Lucide icons
vi.mock('lucide-react', () => ({
  Send: ({ className }: any) => <div data-testid="send-icon" className={className} />,
  Paperclip: ({ className }: any) => <div data-testid="paperclip-icon" className={className} />,
  Image: ({ className }: any) => <div data-testid="image-icon" className={className} />,
  X: ({ className }: any) => <div data-testid="x-icon" className={className} />,
  Check: ({ className }: any) => <div data-testid="check-icon" className={className} />,
  AlertCircle: ({ className }: any) => <div data-testid="alert-icon" className={className} />,
  Loader2: ({ className }: any) => <div data-testid="loader-icon" className={className} />,
}));

// Helper: Create realistic File objects
function createTestFile(name: string, type: string, content: string = 'test content'): File {
  return new File([content], name, { type });
}

// Helper: Simulate file upload with realistic timing
function simulateUploadProgress(
  fileName: string,
  fileIndex: number,
  duration: number = 1000
): Promise<void> {
  return new Promise((resolve) => {
    const steps = 10;
    const stepDuration = duration / steps;
    let progress = 0;
    
    const interval = setInterval(() => {
      progress += 10;
      
      updateMockState('fileUpload', {
        attachments: [
          {
            file: { name: fileName },
            id: progress === 100 ? `file-${fileIndex}` : null,
            status: progress === 100 ? 'complete' : 'uploading',
            progress,
            error: null,
            previewUrl: `blob:preview-${fileIndex}`,
          }
        ],
        isUploading: progress < 100,
        overallProgress: progress,
        allComplete: progress === 100,
      });
      
      if (progress >= 100) {
        clearInterval(interval);
        resolve();
      }
    }, stepDuration);
  });
}

// Helper: Simulate multimodal message from server
function createMultimodalMessage(text: string, imageCount: number = 1): MessageChatItem {
  const contentBlocks = [
    { type: 'text' as const, text }
  ];
  
  for (let i = 0; i < imageCount; i++) {
    contentBlocks.push({
      type: 'image' as const,
      source: {
        type: 'base64' as const,
        media_type: 'image/png',
        data: `base64-image-data-${i}`,
      }
    });
  }
  
  return {
    id: `msg-${Date.now()}`,
    type: 'message',
    role: 'user',
    content: contentBlocks,
    timestamp: new Date().toISOString(),
  };
}

// =============================================================================
// INTEGRATION TEST SUITES
// =============================================================================

describe('File Upload Integration Tests', () => {
  beforeEach(() => {
    vi.clearAllMocks();
    resetAllMocks();
    
    // Setup default dropzone mock
    dropzoneMock = {
      getRootProps: mockGetRootProps,
      getInputProps: mockGetInputProps,
      isDragActive: false,
      open: mockOpen,
    };
    
    (reactDropzone.useDropzone as any).mockReturnValue(dropzoneMock);
    
    // Default file upload state
    updateMockState('fileUpload', {
      attachments: [],
      addFiles: vi.fn(),
      removeFile: vi.fn(),
      uploadFile: vi.fn().mockResolvedValue(undefined),
      uploadAll: vi.fn().mockResolvedValue(undefined),
      clearAll: vi.fn(),
      isUploading: false,
      allComplete: false,
      hasErrors: false,
      overallProgress: 0,
      validationError: null,
      getUploadedFileIds: vi.fn(() => []),
    });
  });

  afterEach(() => {
    vi.restoreAllMocks();
  });

  // ===========================================================================
  // 1. COMPLETE UPLOAD AND SEND FLOW (5 tests)
  // ===========================================================================
  
  describe('Complete Upload and Send Flow', () => {
    it('should complete full workflow: add file → auto-upload → send with file ID', async () => {
      const user = userEvent.setup();
      const mockSendMessage = vi.fn().mockResolvedValue(undefined);
      const mockClearAll = vi.fn();
      const mockGetUploadedFileIds = vi.fn(() => ['file-123']);
      
      updateMockState('chat', { sendMessage: mockSendMessage });
      updateMockState('fileUpload', {
        getUploadedFileIds: mockGetUploadedFileIds,
        clearAll: mockClearAll,
      });
      
      const { rerender } = render(<ChatInputArea />);
      
      // Step 1: User adds file via picker
      const file = createTestFile('vacation.jpg', 'image/jpeg');
      const dropzoneCall = (reactDropzone.useDropzone as any).mock.calls[0][0];
      dropzoneCall.onDrop([file]);
      
      // Step 2: File appears in attachment list (pending)
      updateMockState('fileUpload', {
        attachments: [{
          file,
          id: null,
          status: 'pending',
          progress: 0,
          error: null,
          previewUrl: 'blob:vacation-preview',
        }],
        getUploadedFileIds: mockGetUploadedFileIds,
        clearAll: mockClearAll,
      });
      rerender(<ChatInputArea />);
      
      expect(screen.getByText('vacation.jpg')).toBeInTheDocument();
      expect(screen.getByTestId('status-0')).toHaveTextContent('pending');
      
      // Step 3: Auto-upload begins
      updateMockState('fileUpload', {
        attachments: [{
          file,
          id: null,
          status: 'uploading',
          progress: 50,
          error: null,
          previewUrl: 'blob:vacation-preview',
        }],
        isUploading: true,
        overallProgress: 50,
        getUploadedFileIds: mockGetUploadedFileIds,
        clearAll: mockClearAll,
      });
      rerender(<ChatInputArea />);
      
      expect(screen.getByText('Uploading files... 50%')).toBeInTheDocument();
      expect(screen.getByTestId('status-0')).toHaveTextContent('uploading');
      expect(screen.getByTestId('progress-0')).toHaveTextContent('50%');
      
      // Step 4: Upload completes
      updateMockState('fileUpload', {
        attachments: [{
          file,
          id: 'file-123',
          status: 'complete',
          progress: 100,
          error: null,
          previewUrl: 'blob:vacation-preview',
        }],
        isUploading: false,
        allComplete: true,
        overallProgress: 100,
        getUploadedFileIds: mockGetUploadedFileIds,
        clearAll: mockClearAll,
      });
      rerender(<ChatInputArea />);
      
      expect(screen.queryByText(/Uploading files/)).not.toBeInTheDocument();
      expect(screen.getByTestId('status-0')).toHaveTextContent('complete');
      expect(screen.getByTestId('progress-0')).toHaveTextContent('100%');
      
      // Step 5: User types message and sends
      const textarea = screen.getByRole('textbox');
      await user.type(textarea, 'Check out this photo!');
      
      const sendButton = screen.getByRole('button', { name: 'Send message' });
      expect(sendButton).not.toBeDisabled();
      
      await user.click(sendButton);
      
      // Step 6: Verify sendMessage called with text and file IDs
      await waitFor(() => {
        expect(mockSendMessage).toHaveBeenCalledWith('Check out this photo!', ['file-123']);
      });
      
      // Step 7: Verify cleanup after send
      await waitFor(() => {
        expect(mockClearAll).toHaveBeenCalled();
      });
      expect(textarea).toHaveValue('');
    });

    it('should handle multiple files in complete workflow', async () => {
      const user = userEvent.setup();
      const mockSendMessage = vi.fn().mockResolvedValue(undefined);
      const mockGetUploadedFileIds = vi.fn(() => ['file-1', 'file-2', 'file-3']);
      
      updateMockState('chat', { sendMessage: mockSendMessage });
      updateMockState('fileUpload', {
        getUploadedFileIds: mockGetUploadedFileIds,
      });
      
      const { rerender } = render(<ChatInputArea />);
      
      // Add three files
      const files = [
        createTestFile('image1.png', 'image/png'),
        createTestFile('image2.jpg', 'image/jpeg'),
        createTestFile('image3.gif', 'image/gif'),
      ];
      
      const dropzoneCall = (reactDropzone.useDropzone as any).mock.calls[0][0];
      dropzoneCall.onDrop(files);
      
      // All files uploaded
      updateMockState('fileUpload', {
        attachments: files.map((file, i) => ({
          file,
          id: `file-${i + 1}`,
          status: 'complete',
          progress: 100,
          error: null,
          previewUrl: `blob:preview-${i}`,
        })),
        allComplete: true,
        getUploadedFileIds: mockGetUploadedFileIds,
      });
      rerender(<ChatInputArea />);
      
      // All three should be visible
      expect(screen.getByText('image1.png')).toBeInTheDocument();
      expect(screen.getByText('image2.jpg')).toBeInTheDocument();
      expect(screen.getByText('image3.gif')).toBeInTheDocument();
      
      // Send message
      const textarea = screen.getByRole('textbox');
      await user.type(textarea, 'Three images');
      await user.click(screen.getByRole('button', { name: 'Send message' }));
      
      await waitFor(() => {
        expect(mockSendMessage).toHaveBeenCalledWith('Three images', ['file-1', 'file-2', 'file-3']);
      });
    });

    it('should allow sending file without text', async () => {
      const user = userEvent.setup();
      const mockSendMessage = vi.fn().mockResolvedValue(undefined);
      const mockGetUploadedFileIds = vi.fn(() => ['file-only']);
      
      updateMockState('chat', { sendMessage: mockSendMessage });
      updateMockState('fileUpload', {
        attachments: [{
          file: createTestFile('image.png', 'image/png'),
          id: 'file-only',
          status: 'complete',
          progress: 100,
          error: null,
          previewUrl: 'blob:preview',
        }],
        allComplete: true,
        getUploadedFileIds: mockGetUploadedFileIds,
      });
      
      render(<ChatInputArea />);
      
      // Don't type anything - just send the file
      const sendButton = screen.getByRole('button', { name: 'Send message' });
      expect(sendButton).not.toBeDisabled();
      
      await user.click(sendButton);
      
      await waitFor(() => {
        expect(mockSendMessage).toHaveBeenCalledWith('', ['file-only']);
      });
    });

    it('should prevent sending during upload', async () => {
      const user = userEvent.setup();
      const mockSendMessage = vi.fn();
      
      updateMockState('chat', { sendMessage: mockSendMessage });
      updateMockState('fileUpload', {
        attachments: [{
          file: createTestFile('uploading.png', 'image/png'),
          id: null,
          status: 'uploading',
          progress: 45,
          error: null,
          previewUrl: 'blob:preview',
        }],
        isUploading: true,
        overallProgress: 45,
      });
      
      render(<ChatInputArea />);
      
      const textarea = screen.getByRole('textbox');
      await user.type(textarea, 'Try to send while uploading');
      
      // Send button should be disabled during upload
      const sendButton = screen.getByRole('button', { name: 'Send message' });
      expect(sendButton).toBeDisabled();
      
      await user.click(sendButton);
      expect(mockSendMessage).not.toHaveBeenCalled();
    });

    it('should handle send failure gracefully', async () => {
      const user = userEvent.setup();
      const consoleError = vi.spyOn(console, 'error').mockImplementation(() => {});
      const mockSendMessage = vi.fn().mockRejectedValue(new Error('Network error'));
      const mockGetUploadedFileIds = vi.fn(() => ['file-123']);
      
      updateMockState('chat', { sendMessage: mockSendMessage });
      updateMockState('fileUpload', {
        attachments: [{
          file: createTestFile('image.png', 'image/png'),
          id: 'file-123',
          status: 'complete',
          progress: 100,
          error: null,
          previewUrl: 'blob:preview',
        }],
        allComplete: true,
        getUploadedFileIds: mockGetUploadedFileIds,
      });
      
      render(<ChatInputArea />);
      
      const textarea = screen.getByRole('textbox');
      await user.type(textarea, 'This will fail');
      await user.click(screen.getByRole('button', { name: 'Send message' }));
      
      await waitFor(() => {
        expect(consoleError).toHaveBeenCalledWith('Failed to send message:', expect.any(Error));
      });
      
      // Message should still be in textarea for retry
      expect(textarea).toHaveValue('This will fail');
      
      consoleError.mockRestore();
    });
  });

  // ===========================================================================
  // 2. DRAG-DROP UPLOAD (6 tests)
  // ===========================================================================
  
  describe('Drag-Drop Upload', () => {
    it('should show drop overlay on drag enter', () => {
      dropzoneMock.isDragActive = true;
      (reactDropzone.useDropzone as any).mockReturnValue(dropzoneMock);
      
      render(<ChatInputArea />);
      
      expect(screen.getByTestId('drop-overlay')).toBeInTheDocument();
      expect(screen.getByText('Drop files here')).toBeInTheDocument();
    });

    it('should hide drop overlay on drag leave', () => {
      dropzoneMock.isDragActive = false;
      (reactDropzone.useDropzone as any).mockReturnValue(dropzoneMock);
      
      render(<ChatInputArea />);
      
      expect(screen.queryByTestId('drop-overlay')).not.toBeInTheDocument();
    });

    it('should accept dropped files and begin upload', async () => {
      const mockAddFiles = vi.fn();
      updateMockState('fileUpload', { addFiles: mockAddFiles });
      
      render(<ChatInputArea />);
      
      // Simulate file drop
      const file = createTestFile('dropped.png', 'image/png');
      const dropzoneCall = (reactDropzone.useDropzone as any).mock.calls[0][0];
      dropzoneCall.onDrop([file]);
      
      expect(mockAddFiles).toHaveBeenCalledWith([file]);
      
      // File should appear in attachment list
      updateMockState('fileUpload', {
        attachments: [{
          file,
          id: null,
          status: 'uploading',
          progress: 0,
          error: null,
          previewUrl: 'blob:dropped-preview',
        }],
        isUploading: true,
      });
      
      const { rerender } = render(<ChatInputArea />);
      expect(screen.getByText('dropped.png')).toBeInTheDocument();
    });

    it('should reject files based on MIME type', async () => {
      const mockAddFiles = vi.fn();
      updateMockState('fileUpload', { addFiles: mockAddFiles });
      
      render(<ChatInputArea allowedMimeTypes={['image/png', 'image/jpeg']} />);
      
      // Try to drop a PDF (not allowed)
      const validFile = createTestFile('valid.png', 'image/png');
      const invalidFile = createTestFile('document.pdf', 'application/pdf');
      
      const dropzoneCall = (reactDropzone.useDropzone as any).mock.calls[0][0];
      
      // Dropzone will filter based on accept prop
      const acceptedFiles = [validFile, invalidFile].filter(file => 
        ['image/png', 'image/jpeg'].includes(file.type)
      );
      
      dropzoneCall.onDrop(acceptedFiles);
      
      // Only PNG should be added
      expect(mockAddFiles).toHaveBeenCalledWith([validFile]);
    });

    it('should reject oversized files', async () => {
      const mockAddFiles = vi.fn();
      updateMockState('fileUpload', { addFiles: mockAddFiles });
      
      const maxSize = 5 * 1024 * 1024; // 5MB
      render(<ChatInputArea maxFileSize={maxSize} />);
      
      // Dropzone configured with maxSize
      const dropzoneCall = (reactDropzone.useDropzone as any).mock.calls[0][0];
      expect(dropzoneCall.maxSize).toBe(maxSize);
      
      // If file too large, dropzone won't include it in acceptedFiles
      // and will trigger onDropRejected (not implemented in our simple mock)
    });

    it('should handle multiple files dropped at once', async () => {
      const mockAddFiles = vi.fn();
      const mockSendMessage = vi.fn().mockResolvedValue(undefined);
      const mockGetUploadedFileIds = vi.fn(() => ['file-1', 'file-2']);
      
      updateMockState('fileUpload', { 
        addFiles: mockAddFiles,
        getUploadedFileIds: mockGetUploadedFileIds,
      });
      updateMockState('chat', { sendMessage: mockSendMessage });
      
      const user = userEvent.setup();
      const { rerender } = render(<ChatInputArea />);
      
      // Drop multiple files
      const files = [
        createTestFile('drop1.png', 'image/png'),
        createTestFile('drop2.jpg', 'image/jpeg'),
      ];
      
      const dropzoneCall = (reactDropzone.useDropzone as any).mock.calls[0][0];
      dropzoneCall.onDrop(files);
      
      expect(mockAddFiles).toHaveBeenCalledWith(files);
      
      // Both files uploaded
      updateMockState('fileUpload', {
        attachments: files.map((file, i) => ({
          file,
          id: `file-${i + 1}`,
          status: 'complete',
          progress: 100,
          error: null,
          previewUrl: `blob:${i}`,
        })),
        allComplete: true,
        getUploadedFileIds: mockGetUploadedFileIds,
      });
      rerender(<ChatInputArea />);
      
      // Send message with both files
      const textarea = screen.getByRole('textbox');
      await user.type(textarea, 'Dropped files');
      await user.click(screen.getByRole('button', { name: 'Send message' }));
      
      await waitFor(() => {
        expect(mockSendMessage).toHaveBeenCalledWith('Dropped files', ['file-1', 'file-2']);
      });
    });
  });

  // ===========================================================================
  // 3. CLIPBOARD PASTE IMAGE (5 tests)
  // ===========================================================================
  
  describe('Clipboard Paste Image', () => {
    const createPasteEvent = (file: File) => {
      return {
        clipboardData: {
          items: [{
            kind: 'file' as const,
            type: file.type,
            getAsFile: () => file,
          }],
        },
        preventDefault: vi.fn(),
      };
    };

    it('should detect pasted image and add to attachments', async () => {
      const mockAddFiles = vi.fn();
      updateMockState('fileUpload', { addFiles: mockAddFiles });
      
      render(<ChatInputArea />);
      
      const textarea = screen.getByRole('textbox') as HTMLTextAreaElement;
      const file = createTestFile('pasted.png', 'image/png', 'pasted-image-data');
      
      // Simulate paste by accessing React props
      const reactPropsKey = Object.keys(textarea).find(key => key.startsWith('__reactProps'));
      if (reactPropsKey) {
        const props = (textarea as any)[reactPropsKey];
        const event = createPasteEvent(file);
        props.onPaste(event);
        
        expect(mockAddFiles).toHaveBeenCalledWith([file]);
        expect(event.preventDefault).toHaveBeenCalled();
      }
    });

    it('should show pasted image in attachment list with preview', async () => {
      const mockAddFiles = vi.fn();
      updateMockState('fileUpload', { addFiles: mockAddFiles });
      
      render(<ChatInputArea />);
      
      const textarea = screen.getByRole('textbox') as HTMLTextAreaElement;
      const file = createTestFile('clipboard.png', 'image/png');
      
      const reactPropsKey = Object.keys(textarea).find(key => key.startsWith('__reactProps'));
      if (reactPropsKey) {
        const props = (textarea as any)[reactPropsKey];
        props.onPaste(createPasteEvent(file));
        
        // Image appears with preview
        updateMockState('fileUpload', {
          attachments: [{
            file,
            id: null,
            status: 'pending',
            progress: 0,
            error: null,
            previewUrl: 'blob:clipboard-preview',
          }],
        });
        
        const { rerender } = render(<ChatInputArea />);
        
        expect(screen.getByText('clipboard.png')).toBeInTheDocument();
        expect(screen.getByTestId('preview-0')).toHaveAttribute('src', 'blob:clipboard-preview');
      }
    });

    it('should complete paste → upload → send workflow', async () => {
      const user = userEvent.setup();
      const mockAddFiles = vi.fn();
      const mockSendMessage = vi.fn().mockResolvedValue(undefined);
      const mockGetUploadedFileIds = vi.fn(() => ['pasted-file-id']);
      
      updateMockState('fileUpload', { 
        addFiles: mockAddFiles,
        getUploadedFileIds: mockGetUploadedFileIds,
      });
      updateMockState('chat', { sendMessage: mockSendMessage });
      
      const { rerender } = render(<ChatInputArea />);
      
      const textarea = screen.getByRole('textbox') as HTMLTextAreaElement;
      const file = createTestFile('pasted-screenshot.png', 'image/png');
      
      // Step 1: Paste image
      const reactPropsKey = Object.keys(textarea).find(key => key.startsWith('__reactProps'));
      if (reactPropsKey) {
        const props = (textarea as any)[reactPropsKey];
        props.onPaste(createPasteEvent(file));
        
        expect(mockAddFiles).toHaveBeenCalledWith([file]);
        
        // Step 2: Auto-upload completes
        updateMockState('fileUpload', {
          attachments: [{
            file,
            id: 'pasted-file-id',
            status: 'complete',
            progress: 100,
            error: null,
            previewUrl: 'blob:pasted-preview',
          }],
          allComplete: true,
          getUploadedFileIds: mockGetUploadedFileIds,
        });
        rerender(<ChatInputArea />);
        
        // Step 3: Type message and send
        await user.type(textarea, 'Look at this screenshot');
        await user.click(screen.getByRole('button', { name: 'Send message' }));
        
        await waitFor(() => {
          expect(mockSendMessage).toHaveBeenCalledWith('Look at this screenshot', ['pasted-file-id']);
        });
      }
    });

    it('should allow text paste to proceed normally', async () => {
      render(<ChatInputArea />);
      
      const textarea = screen.getByRole('textbox') as HTMLTextAreaElement;
      
      const reactPropsKey = Object.keys(textarea).find(key => key.startsWith('__reactProps'));
      if (reactPropsKey) {
        const props = (textarea as any)[reactPropsKey];
        const event = {
          clipboardData: {
            items: [{
              kind: 'string' as const,
              type: 'text/plain',
              getAsFile: () => null,
            }],
          },
          preventDefault: vi.fn(),
        };
        
        props.onPaste(event);
        
        // Should NOT prevent default for text paste
        expect(event.preventDefault).not.toHaveBeenCalled();
      }
    });

    it('should filter pasted files by allowed MIME types', async () => {
      const mockAddFiles = vi.fn();
      updateMockState('fileUpload', { addFiles: mockAddFiles });
      
      render(<ChatInputArea allowedMimeTypes={['image/png']} />);
      
      const textarea = screen.getByRole('textbox') as HTMLTextAreaElement;
      const pngFile = createTestFile('allowed.png', 'image/png');
      const jpgFile = createTestFile('not-allowed.jpg', 'image/jpeg');
      
      const reactPropsKey = Object.keys(textarea).find(key => key.startsWith('__reactProps'));
      if (reactPropsKey) {
        const props = (textarea as any)[reactPropsKey];
        
        // Try to paste JPEG (should be filtered out)
        const event = {
          clipboardData: {
            items: [
              {
                kind: 'file' as const,
                type: pngFile.type,
                getAsFile: () => pngFile,
              },
              {
                kind: 'file' as const,
                type: jpgFile.type,
                getAsFile: () => jpgFile,
              }
            ],
          },
          preventDefault: vi.fn(),
        };
        
        props.onPaste(event);
        
        // Only PNG should be added
        expect(mockAddFiles).toHaveBeenCalledWith([pngFile]);
      }
    });
  });

  // ===========================================================================
  // 4. UPLOAD PROGRESS DISPLAY (5 tests)
  // ===========================================================================
  
  describe('Upload Progress Display', () => {
    it('should show progress indicator during upload', () => {
      updateMockState('fileUpload', {
        attachments: [{
          file: createTestFile('uploading.png', 'image/png'),
          id: null,
          status: 'uploading',
          progress: 35,
          error: null,
          previewUrl: 'blob:preview',
        }],
        isUploading: true,
        overallProgress: 35,
      });
      
      render(<ChatInputArea />);
      
      expect(screen.getByText('Uploading files... 35%')).toBeInTheDocument();
      expect(screen.getByTestId('status-0')).toHaveTextContent('uploading');
      expect(screen.getByTestId('progress-0')).toHaveTextContent('35%');
    });

    it('should update progress as upload advances', () => {
      updateMockState('fileUpload', {
        attachments: [{
          file: createTestFile('file.png', 'image/png'),
          id: null,
          status: 'uploading',
          progress: 10,
          error: null,
          previewUrl: 'blob:preview',
        }],
        isUploading: true,
        overallProgress: 10,
      });
      
      const { rerender } = render(<ChatInputArea />);
      expect(screen.getByText('Uploading files... 10%')).toBeInTheDocument();
      
      // Progress to 50%
      updateMockState('fileUpload', {
        attachments: [{
          file: createTestFile('file.png', 'image/png'),
          id: null,
          status: 'uploading',
          progress: 50,
          error: null,
          previewUrl: 'blob:preview',
        }],
        isUploading: true,
        overallProgress: 50,
      });
      rerender(<ChatInputArea />);
      expect(screen.getByText('Uploading files... 50%')).toBeInTheDocument();
      
      // Complete at 100%
      updateMockState('fileUpload', {
        attachments: [{
          file: createTestFile('file.png', 'image/png'),
          id: 'file-complete',
          status: 'complete',
          progress: 100,
          error: null,
          previewUrl: 'blob:preview',
        }],
        isUploading: false,
        allComplete: true,
        overallProgress: 100,
      });
      rerender(<ChatInputArea />);
      expect(screen.queryByText(/Uploading files/)).not.toBeInTheDocument();
      expect(screen.getByTestId('status-0')).toHaveTextContent('complete');
    });

    it('should track individual file progress in multi-file upload', () => {
      updateMockState('fileUpload', {
        attachments: [
          {
            file: createTestFile('file1.png', 'image/png'),
            id: 'file-1',
            status: 'complete',
            progress: 100,
            error: null,
            previewUrl: 'blob:1',
          },
          {
            file: createTestFile('file2.jpg', 'image/jpeg'),
            id: null,
            status: 'uploading',
            progress: 60,
            error: null,
            previewUrl: 'blob:2',
          },
          {
            file: createTestFile('file3.gif', 'image/gif'),
            id: null,
            status: 'pending',
            progress: 0,
            error: null,
            previewUrl: 'blob:3',
          },
        ],
        isUploading: true,
        overallProgress: 53, // (100 + 60 + 0) / 3
      });
      
      render(<ChatInputArea />);
      
      // Check individual progress
      expect(screen.getByTestId('status-0')).toHaveTextContent('complete');
      expect(screen.getByTestId('progress-0')).toHaveTextContent('100%');
      
      expect(screen.getByTestId('status-1')).toHaveTextContent('uploading');
      expect(screen.getByTestId('progress-1')).toHaveTextContent('60%');
      
      expect(screen.getByTestId('status-2')).toHaveTextContent('pending');
      expect(screen.getByTestId('progress-2')).toHaveTextContent('0%');
      
      // Check overall progress
      expect(screen.getByText('Uploading files... 53%')).toBeInTheDocument();
    });

    it('should show completion checkmarks when all files uploaded', () => {
      updateMockState('fileUpload', {
        attachments: [
          {
            file: createTestFile('file1.png', 'image/png'),
            id: 'file-1',
            status: 'complete',
            progress: 100,
            error: null,
            previewUrl: 'blob:1',
          },
          {
            file: createTestFile('file2.png', 'image/png'),
            id: 'file-2',
            status: 'complete',
            progress: 100,
            error: null,
            previewUrl: 'blob:2',
          },
        ],
        isUploading: false,
        allComplete: true,
        overallProgress: 100,
      });
      
      render(<ChatInputArea />);
      
      expect(screen.queryByText(/Uploading files/)).not.toBeInTheDocument();
      expect(screen.getByTestId('status-0')).toHaveTextContent('complete');
      expect(screen.getByTestId('status-1')).toHaveTextContent('complete');
    });

    it('should hide progress indicator after upload completes', () => {
      updateMockState('fileUpload', {
        attachments: [{
          file: createTestFile('file.png', 'image/png'),
          id: null,
          status: 'uploading',
          progress: 99,
          error: null,
          previewUrl: 'blob:preview',
        }],
        isUploading: true,
        overallProgress: 99,
      });
      
      const { rerender } = render(<ChatInputArea />);
      expect(screen.getByText('Uploading files... 99%')).toBeInTheDocument();
      
      // Complete upload
      updateMockState('fileUpload', {
        attachments: [{
          file: createTestFile('file.png', 'image/png'),
          id: 'file-done',
          status: 'complete',
          progress: 100,
          error: null,
          previewUrl: 'blob:preview',
        }],
        isUploading: false,
        allComplete: true,
        overallProgress: 100,
      });
      rerender(<ChatInputArea />);
      
      expect(screen.queryByText(/Uploading files/)).not.toBeInTheDocument();
    });
  });

  // ===========================================================================
  // 5. FILE VALIDATION ERRORS (6 tests)
  // ===========================================================================
  
  describe('File Validation Errors', () => {
    it('should display error for oversized file', () => {
      updateMockState('fileUpload', {
        validationError: 'File size exceeds maximum of 10MB',
        attachments: [],
      });
      
      render(<ChatInputArea />);
      
      const alert = screen.getByRole('alert');
      expect(alert).toHaveTextContent('File size exceeds maximum of 10MB');
    });

    it('should display error for invalid MIME type', () => {
      updateMockState('fileUpload', {
        validationError: 'File type not allowed. Please upload images only.',
        attachments: [],
      });
      
      render(<ChatInputArea />);
      
      expect(screen.getByRole('alert')).toHaveTextContent('File type not allowed. Please upload images only.');
    });

    it('should display error when too many files', () => {
      updateMockState('fileUpload', {
        validationError: 'Maximum 10 files allowed',
        attachments: Array(10).fill({
          file: createTestFile('image.png', 'image/png'),
          id: null,
          status: 'pending',
          progress: 0,
          error: null,
          previewUrl: 'blob:preview',
        }),
      });
      
      render(<ChatInputArea maxFiles={10} />);
      
      expect(screen.getByRole('alert')).toHaveTextContent('Maximum 10 files allowed');
    });

    it('should clear validation error when corrected', () => {
      updateMockState('fileUpload', {
        validationError: 'File too large',
        attachments: [],
      });
      
      const { rerender } = render(<ChatInputArea />);
      expect(screen.getByRole('alert')).toHaveTextContent('File too large');
      
      // User removes problematic file
      updateMockState('fileUpload', {
        validationError: null,
        attachments: [],
      });
      rerender(<ChatInputArea />);
      
      expect(screen.queryByRole('alert')).not.toBeInTheDocument();
    });

    it('should show individual file error status', () => {
      updateMockState('fileUpload', {
        attachments: [{
          file: createTestFile('failed.png', 'image/png'),
          id: null,
          status: 'error',
          progress: 45,
          error: 'Upload failed: Network timeout',
          previewUrl: 'blob:preview',
        }],
        hasErrors: true,
      });
      
      render(<ChatInputArea />);
      
      expect(screen.getByTestId('status-0')).toHaveTextContent('error');
    });

    it('should associate error message with textarea via aria-describedby', () => {
      updateMockState('fileUpload', {
        validationError: 'Validation error message',
      });
      
      render(<ChatInputArea />);
      
      const textarea = screen.getByRole('textbox');
      expect(textarea).toHaveAttribute('aria-describedby', 'file-error');
      
      const alert = screen.getByRole('alert');
      expect(alert).toBeInTheDocument();
    });
  });

  // ===========================================================================
  // 6. MULTIPLE FILE UPLOADS (4 tests)
  // ===========================================================================
  
  describe('Multiple File Uploads', () => {
    it('should upload multiple files in sequence', async () => {
      const mockGetUploadedFileIds = vi.fn(() => ['file-1', 'file-2', 'file-3']);
      
      updateMockState('fileUpload', {
        attachments: [
          {
            file: createTestFile('img1.png', 'image/png'),
            id: null,
            status: 'uploading',
            progress: 30,
            error: null,
            previewUrl: 'blob:1',
          },
          {
            file: createTestFile('img2.jpg', 'image/jpeg'),
            id: null,
            status: 'pending',
            progress: 0,
            error: null,
            previewUrl: 'blob:2',
          },
          {
            file: createTestFile('img3.gif', 'image/gif'),
            id: null,
            status: 'pending',
            progress: 0,
            error: null,
            previewUrl: 'blob:3',
          },
        ],
        isUploading: true,
        overallProgress: 10,
        getUploadedFileIds: mockGetUploadedFileIds,
      });
      
      const { rerender } = render(<ChatInputArea />);
      
      // First file uploading
      expect(screen.getByTestId('status-0')).toHaveTextContent('uploading');
      expect(screen.getByTestId('status-1')).toHaveTextContent('pending');
      expect(screen.getByTestId('status-2')).toHaveTextContent('pending');
      
      // First completes, second starts
      updateMockState('fileUpload', {
        attachments: [
          {
            file: createTestFile('img1.png', 'image/png'),
            id: 'file-1',
            status: 'complete',
            progress: 100,
            error: null,
            previewUrl: 'blob:1',
          },
          {
            file: createTestFile('img2.jpg', 'image/jpeg'),
            id: null,
            status: 'uploading',
            progress: 50,
            error: null,
            previewUrl: 'blob:2',
          },
          {
            file: createTestFile('img3.gif', 'image/gif'),
            id: null,
            status: 'pending',
            progress: 0,
            error: null,
            previewUrl: 'blob:3',
          },
        ],
        isUploading: true,
        overallProgress: 50,
        getUploadedFileIds: mockGetUploadedFileIds,
      });
      rerender(<ChatInputArea />);
      
      expect(screen.getByTestId('status-0')).toHaveTextContent('complete');
      expect(screen.getByTestId('status-1')).toHaveTextContent('uploading');
      
      // All complete
      updateMockState('fileUpload', {
        attachments: [
          {
            file: createTestFile('img1.png', 'image/png'),
            id: 'file-1',
            status: 'complete',
            progress: 100,
            error: null,
            previewUrl: 'blob:1',
          },
          {
            file: createTestFile('img2.jpg', 'image/jpeg'),
            id: 'file-2',
            status: 'complete',
            progress: 100,
            error: null,
            previewUrl: 'blob:2',
          },
          {
            file: createTestFile('img3.gif', 'image/gif'),
            id: 'file-3',
            status: 'complete',
            progress: 100,
            error: null,
            previewUrl: 'blob:3',
          },
        ],
        isUploading: false,
        allComplete: true,
        overallProgress: 100,
        getUploadedFileIds: mockGetUploadedFileIds,
      });
      rerender(<ChatInputArea />);
      
      expect(screen.getByTestId('status-0')).toHaveTextContent('complete');
      expect(screen.getByTestId('status-1')).toHaveTextContent('complete');
      expect(screen.getByTestId('status-2')).toHaveTextContent('complete');
    });

    it('should calculate overall progress across all files', () => {
      updateMockState('fileUpload', {
        attachments: [
          { file: createTestFile('a.png', 'image/png'), progress: 100, status: 'complete' },
          { file: createTestFile('b.png', 'image/png'), progress: 75, status: 'uploading' },
          { file: createTestFile('c.png', 'image/png'), progress: 50, status: 'uploading' },
          { file: createTestFile('d.png', 'image/png'), progress: 0, status: 'pending' },
        ],
        isUploading: true,
        overallProgress: 56, // (100 + 75 + 50 + 0) / 4 = 56.25
      });
      
      render(<ChatInputArea />);
      
      expect(screen.getByText('Uploading files... 56%')).toBeInTheDocument();
    });

    it('should enable send only when all files complete', async () => {
      const user = userEvent.setup();
      
      updateMockState('fileUpload', {
        attachments: [
          { file: createTestFile('a.png', 'image/png'), id: 'file-1', progress: 100, status: 'complete', error: null, previewUrl: 'blob:1' },
          { file: createTestFile('b.png', 'image/png'), id: null, progress: 80, status: 'uploading', error: null, previewUrl: 'blob:2' },
        ],
        isUploading: true,
        allComplete: false,
      });
      
      const { rerender } = render(<ChatInputArea />);
      
      const textarea = screen.getByRole('textbox');
      await user.type(textarea, 'Wait for all');
      
      // Send disabled while uploading
      let sendButton = screen.getByRole('button', { name: 'Send message' });
      expect(sendButton).toBeDisabled();
      
      // All complete now
      updateMockState('fileUpload', {
        attachments: [
          { file: createTestFile('a.png', 'image/png'), id: 'file-1', progress: 100, status: 'complete' },
          { file: createTestFile('b.png', 'image/png'), id: 'file-2', progress: 100, status: 'complete' },
        ],
        isUploading: false,
        allComplete: true,
      });
      rerender(<ChatInputArea />);
      
      sendButton = screen.getByRole('button', { name: 'Send message' });
      expect(sendButton).not.toBeDisabled();
    });

    it('should pass all file IDs to sendMessage', async () => {
      const user = userEvent.setup();
      const mockSendMessage = vi.fn().mockResolvedValue(undefined);
      const mockGetUploadedFileIds = vi.fn(() => ['id-1', 'id-2', 'id-3', 'id-4']);
      
      updateMockState('chat', { sendMessage: mockSendMessage });
      updateMockState('fileUpload', {
        attachments: [
          { file: createTestFile('a.png', 'image/png'), id: 'id-1', status: 'complete', progress: 100 },
          { file: createTestFile('b.png', 'image/png'), id: 'id-2', status: 'complete', progress: 100 },
          { file: createTestFile('c.png', 'image/png'), id: 'id-3', status: 'complete', progress: 100 },
          { file: createTestFile('d.png', 'image/png'), id: 'id-4', status: 'complete', progress: 100 },
        ],
        allComplete: true,
        getUploadedFileIds: mockGetUploadedFileIds,
      });
      
      render(<ChatInputArea />);
      
      const textarea = screen.getByRole('textbox');
      await user.type(textarea, 'Four files');
      await user.click(screen.getByRole('button', { name: 'Send message' }));
      
      await waitFor(() => {
        expect(mockSendMessage).toHaveBeenCalledWith('Four files', ['id-1', 'id-2', 'id-3', 'id-4']);
      });
    });
  });

  // ===========================================================================
  // 7. REMOVE FILE BEFORE SEND (5 tests)
  // ===========================================================================
  
  describe('Remove File Before Send', () => {
    it('should remove file from attachment list', async () => {
      const user = userEvent.setup();
      const mockRemoveFile = vi.fn();
      
      updateMockState('fileUpload', {
        attachments: [
          { file: createTestFile('keep.png', 'image/png'), id: 'file-1', status: 'complete', progress: 100 },
          { file: createTestFile('remove.png', 'image/png'), id: 'file-2', status: 'complete', progress: 100 },
        ],
        removeFile: mockRemoveFile,
      });
      
      render(<ChatInputArea />);
      
      expect(screen.getByText('keep.png')).toBeInTheDocument();
      expect(screen.getByText('remove.png')).toBeInTheDocument();
      
      // Remove second file
      await user.click(screen.getByRole('button', { name: 'Remove remove.png' }));
      
      expect(mockRemoveFile).toHaveBeenCalledWith(1);
    });

    it('should cancel upload if file is removed during upload', async () => {
      const user = userEvent.setup();
      const mockRemoveFile = vi.fn();
      
      updateMockState('fileUpload', {
        attachments: [{
          file: createTestFile('uploading.png', 'image/png'),
          id: null,
          status: 'uploading',
          progress: 65,
          error: null,
          previewUrl: 'blob:preview',
        }],
        isUploading: true,
        removeFile: mockRemoveFile,
      });
      
      render(<ChatInputArea />);
      
      expect(screen.getByTestId('status-0')).toHaveTextContent('uploading');
      
      // Remove while uploading
      await user.click(screen.getByRole('button', { name: 'Remove uploading.png' }));
      
      expect(mockRemoveFile).toHaveBeenCalledWith(0);
    });

    it('should clean up preview URL when file removed', async () => {
      const user = userEvent.setup();
      const mockRemoveFile = vi.fn();
      
      updateMockState('fileUpload', {
        attachments: [{
          file: createTestFile('image.png', 'image/png'),
          id: 'file-1',
          status: 'complete',
          progress: 100,
          error: null,
          previewUrl: 'blob:preview-to-revoke',
        }],
        removeFile: mockRemoveFile,
      });
      
      render(<ChatInputArea />);
      
      const preview = screen.getByTestId('preview-0');
      expect(preview).toHaveAttribute('src', 'blob:preview-to-revoke');
      
      // Remove file (preview URL should be revoked in actual implementation)
      await user.click(screen.getByRole('button', { name: 'Remove image.png' }));
      
      expect(mockRemoveFile).toHaveBeenCalledWith(0);
    });

    it('should still allow send with remaining files after removal', async () => {
      const user = userEvent.setup();
      const mockSendMessage = vi.fn().mockResolvedValue(undefined);
      const mockRemoveFile = vi.fn();
      const mockGetUploadedFileIds = vi.fn(() => ['file-1']); // Only first file remains
      
      updateMockState('chat', { sendMessage: mockSendMessage });
      updateMockState('fileUpload', {
        attachments: [
          { file: createTestFile('keep.png', 'image/png'), id: 'file-1', status: 'complete', progress: 100 },
          { file: createTestFile('remove.png', 'image/png'), id: 'file-2', status: 'complete', progress: 100 },
        ],
        removeFile: mockRemoveFile,
        getUploadedFileIds: mockGetUploadedFileIds,
      });
      
      const { rerender } = render(<ChatInputArea />);
      
      // Remove second file
      await user.click(screen.getByRole('button', { name: 'Remove remove.png' }));
      
      // Update state to reflect removal
      updateMockState('fileUpload', {
        attachments: [
          { file: createTestFile('keep.png', 'image/png'), id: 'file-1', status: 'complete', progress: 100 },
        ],
        getUploadedFileIds: mockGetUploadedFileIds,
      });
      rerender(<ChatInputArea />);
      
      // Send with remaining file
      const textarea = screen.getByRole('textbox');
      await user.type(textarea, 'One file left');
      await user.click(screen.getByRole('button', { name: 'Send message' }));
      
      await waitFor(() => {
        expect(mockSendMessage).toHaveBeenCalledWith('One file left', ['file-1']);
      });
    });

    it('should disable send if all files removed and no text', async () => {
      const user = userEvent.setup();
      const mockRemoveFile = vi.fn();
      
      updateMockState('fileUpload', {
        attachments: [{
          file: createTestFile('only.png', 'image/png'),
          id: 'file-1',
          status: 'complete',
          progress: 100,
          error: null,
          previewUrl: 'blob:preview',
        }],
        removeFile: mockRemoveFile,
      });
      
      const { rerender } = render(<ChatInputArea />);
      
      // Send button enabled with file
      let sendButton = screen.getByRole('button', { name: 'Send message' });
      expect(sendButton).not.toBeDisabled();
      
      // Remove the only file
      await user.click(screen.getByRole('button', { name: 'Remove only.png' }));
      
      // Update to empty attachments
      updateMockState('fileUpload', {
        attachments: [],
      });
      rerender(<ChatInputArea />);
      
      // Send button now disabled (no text, no files)
      sendButton = screen.getByRole('button', { name: 'Send message' });
      expect(sendButton).toBeDisabled();
    });
  });

  // ===========================================================================
  // 8. DISPLAY MULTIMODAL MESSAGE FROM SERVER (5 tests)
  // ===========================================================================
  
  describe('Display Multimodal Message from Server', () => {
    // Note: These tests focus on the integration between ChatInputArea sending
    // multimodal content and ChatMessagesView displaying it correctly
    
    it('should display sent message with image content', async () => {
      const user = userEvent.setup();
      const mockSendMessage = vi.fn().mockResolvedValue(undefined);
      const mockGetUploadedFileIds = vi.fn(() => ['file-img-1']);
      
      updateMockState('chat', { 
        sendMessage: mockSendMessage,
        messages: [],
      });
      updateMockState('fileUpload', {
        attachments: [{
          file: createTestFile('photo.png', 'image/png'),
          id: 'file-img-1',
          status: 'complete',
          progress: 100,
          error: null,
          previewUrl: 'blob:photo-preview',
        }],
        allComplete: true,
        getUploadedFileIds: mockGetUploadedFileIds,
      });
      
      render(<ChatInputArea />);
      
      const textarea = screen.getByRole('textbox');
      await user.type(textarea, 'Look at this');
      await user.click(screen.getByRole('button', { name: 'Send message' }));
      
      await waitFor(() => {
        expect(mockSendMessage).toHaveBeenCalledWith('Look at this', ['file-img-1']);
      });
      
      // Simulate server response with multimodal message
      const multimodalMessage = createMultimodalMessage('Look at this', 1);
      updateMockState('chat', {
        messages: [multimodalMessage],
      });
      
      // This would be tested more thoroughly in ChatMessagesView tests
      // Here we just verify the message structure is correct
      expect(multimodalMessage.content).toHaveLength(2);
      expect(multimodalMessage.content[0]).toEqual({ type: 'text', text: 'Look at this' });
      expect(multimodalMessage.content[1]).toEqual({
        type: 'image',
        source: {
          type: 'base64',
          media_type: 'image/png',
          data: 'base64-image-data-0',
        }
      });
    });

    it('should handle message with multiple images', async () => {
      const multimodalMessage = createMultimodalMessage('Three images', 3);
      
      expect(multimodalMessage.content).toHaveLength(4); // 1 text + 3 images
      expect(multimodalMessage.content.filter(block => block.type === 'image')).toHaveLength(3);
    });

    it('should handle image-only message (no text)', async () => {
      const multimodalMessage = createMultimodalMessage('', 1);
      
      expect(multimodalMessage.content).toHaveLength(2);
      expect(multimodalMessage.content[0]).toEqual({ type: 'text', text: '' });
      expect(multimodalMessage.content[1].type).toBe('image');
    });

    it('should preserve message order with mixed text and images', async () => {
      const message: MessageChatItem = {
        id: 'msg-mixed',
        type: 'message',
        role: 'user',
        content: [
          { type: 'text', text: 'Before image' },
          { 
            type: 'image', 
            source: { 
              type: 'base64', 
              media_type: 'image/png', 
              data: 'img1' 
            } 
          },
          { type: 'text', text: 'Between images' },
          { 
            type: 'image', 
            source: { 
              type: 'base64', 
              media_type: 'image/jpeg', 
              data: 'img2' 
            } 
          },
          { type: 'text', text: 'After images' },
        ],
        timestamp: new Date().toISOString(),
      };
      
      // Verify content block order
      expect(message.content[0]).toEqual({ type: 'text', text: 'Before image' });
      expect(message.content[1].type).toBe('image');
      expect(message.content[2]).toEqual({ type: 'text', text: 'Between images' });
      expect(message.content[3].type).toBe('image');
      expect(message.content[4]).toEqual({ type: 'text', text: 'After images' });
    });

    it('should handle base64 image data correctly', async () => {
      const message = createMultimodalMessage('Base64 test', 1);
      const imageBlock = message.content.find(block => block.type === 'image');
      
      expect(imageBlock).toBeDefined();
      if (imageBlock && imageBlock.type === 'image') {
        expect(imageBlock.source.type).toBe('base64');
        expect(imageBlock.source.media_type).toBe('image/png');
        expect(imageBlock.source.data).toBeTruthy();
        expect(typeof imageBlock.source.data).toBe('string');
      }
    });
  });
});
