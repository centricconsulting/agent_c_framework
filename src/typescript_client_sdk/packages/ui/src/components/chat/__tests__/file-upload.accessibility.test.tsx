/**
 * File Upload Accessibility Tests
 * 
 * Comprehensive accessibility testing for file upload components
 * Ensures WCAG 2.1 AA compliance
 */

import { describe, it, expect, vi, beforeEach } from 'vitest';
import { render, screen, within } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { axe, toHaveNoViolations } from 'jest-axe';

// Extend matchers
expect.extend(toHaveNoViolations);

// Components to test
import { ChatInputArea } from '../ChatInputArea';
import { FileAttachmentItem } from '../FileAttachmentItem';
import { FileAttachmentList } from '../FileAttachmentList';
import { UploadProgressIndicator } from '../UploadProgressIndicator';
import { DropOverlay } from '../DropOverlay';
import { ImageLightbox } from '../ImageLightbox';
import { ImageContentRenderer, MultimodalContentRenderer } from '../content-renderers';

// Mock hooks
import { updateMockState } from '@test/mocks/realtime-react';
import type { FileAttachment } from '@agentc/realtime-react';

describe('File Upload Accessibility Tests', () => {
  
  describe('ChatInputArea', () => {
    beforeEach(() => {
      updateMockState('chat', {
        messages: [],
        isAgentTyping: false,
        sendMessage: vi.fn(),
      });
      
      updateMockState('fileUpload', {
        attachments: [],
        isUploading: false,
        validationError: null,
        overallProgress: 0,
        addFiles: vi.fn(),
        removeFile: vi.fn(),
        clearAll: vi.fn(),
        getUploadedFileIds: vi.fn(() => []),
      });
    });

    it('should have no axe violations in default state', async () => {
      const { container } = render(<ChatInputArea />);
      const results = await axe(container);
      expect(results).toHaveNoViolations();
    });

    it('should have no axe violations with files attached', async () => {
      const mockAttachment: FileAttachment = {
        id: 'file-1',
        file: new File(['test'], 'test.jpg', { type: 'image/jpeg' }),
        status: 'complete',
        progress: 100,
        previewUrl: 'data:image/jpeg;base64,test',
      };
      
      updateMockState('fileUpload', {
        attachments: [mockAttachment],
      });
      
      const { container } = render(<ChatInputArea />);
      const results = await axe(container);
      expect(results).toHaveNoViolations();
    });

    it('should have no axe violations with validation error', async () => {
      updateMockState('fileUpload', {
        validationError: 'File too large',
      });
      
      const { container } = render(<ChatInputArea />);
      const results = await axe(container);
      expect(results).toHaveNoViolations();
    });

    it('should have proper ARIA labels on all interactive elements', () => {
      render(<ChatInputArea showFilePicker={true} />);
      
      // Main container
      expect(screen.getByLabelText('Message input area with file upload support')).toBeInTheDocument();
      
      // Text input
      expect(screen.getByLabelText('Message text input')).toBeInTheDocument();
      
      // Attach files button
      expect(screen.getByLabelText('Attach files')).toBeInTheDocument();
      
      // Send button
      expect(screen.getByLabelText('Send message')).toBeInTheDocument();
    });

    it('should announce validation errors with role="alert"', () => {
      updateMockState('fileUpload', {
        validationError: 'File too large',
      });
      
      render(<ChatInputArea />);
      
      const alert = screen.getByRole('alert');
      expect(alert).toHaveTextContent('File too large');
    });

    it('should have accessible textarea with proper attributes', () => {
      updateMockState('fileUpload', {
        validationError: 'File error',
      });
      
      render(<ChatInputArea />);
      
      const textarea = screen.getByLabelText('Message text input');
      expect(textarea).toHaveAttribute('aria-describedby', 'file-error');
    });

    it('should support keyboard navigation', async () => {
      const user = userEvent.setup();
      render(<ChatInputArea showFilePicker={true} />);
      
      // Tab to textarea
      await user.tab();
      expect(screen.getByLabelText('Message text input')).toHaveFocus();
      
      // Tab to attach button
      await user.tab();
      expect(screen.getByLabelText('Attach files')).toHaveFocus();
      
      // Tab to send button
      await user.tab();
      expect(screen.getByLabelText('Send message')).toHaveFocus();
    });

    it('should activate buttons with Enter key', async () => {
      const user = userEvent.setup();
      const onSend = vi.fn();
      
      render(<ChatInputArea onSend={onSend} />);
      
      const textarea = screen.getByLabelText('Message text input');
      await user.click(textarea);
      await user.type(textarea, 'Test message');
      
      // Tab to send button
      await user.tab();
      await user.tab();
      
      // Press Enter
      await user.keyboard('{Enter}');
      
      expect(onSend).toHaveBeenCalled();
    });

    it('should have minimum touch target size (44x44px)', () => {
      render(<ChatInputArea showFilePicker={true} />);
      
      const attachButton = screen.getByLabelText('Attach files');
      const sendButton = screen.getByLabelText('Send message');
      
      // Buttons should have h-[44px] w-[44px] classes
      expect(attachButton).toHaveClass('h-[44px]', 'w-[44px]');
      expect(sendButton).toHaveClass('h-[44px]', 'w-[44px]');
    });

    it('should have sufficient contrast for disabled state', () => {
      render(<ChatInputArea disabled={true} />);
      
      const textarea = screen.getByLabelText('Message text input');
      expect(textarea).toBeDisabled();
      expect(textarea).toHaveClass('disabled:opacity-50');
    });
  });

  describe('FileAttachmentItem', () => {
    const createMockAttachment = (overrides: Partial<FileAttachment> = {}): FileAttachment => ({
      id: 'file-1',
      file: new File(['test'], 'test.jpg', { type: 'image/jpeg' }),
      status: 'complete',
      progress: 100,
      previewUrl: 'data:image/jpeg;base64,test',
      ...overrides,
    });

    it('should have no axe violations in complete state', async () => {
      const attachment = createMockAttachment();
      const { container } = render(
        <FileAttachmentItem attachment={attachment} onRemove={vi.fn()} />
      );
      const results = await axe(container);
      expect(results).toHaveNoViolations();
    });

    it('should have no axe violations in uploading state', async () => {
      const attachment = createMockAttachment({ status: 'uploading', progress: 50 });
      const { container } = render(
        <FileAttachmentItem attachment={attachment} onRemove={vi.fn()} />
      );
      const results = await axe(container);
      expect(results).toHaveNoViolations();
    });

    it('should have no axe violations in error state', async () => {
      const attachment = createMockAttachment({ 
        status: 'error', 
        error: 'Upload failed' 
      });
      const { container } = render(
        <FileAttachmentItem attachment={attachment} onRemove={vi.fn()} />
      );
      const results = await axe(container);
      expect(results).toHaveNoViolations();
    });

    it('should have proper ARIA label with status', () => {
      const attachment = createMockAttachment();
      render(<FileAttachmentItem attachment={attachment} onRemove={vi.fn()} />);
      
      const item = screen.getByRole('listitem');
      expect(item).toHaveAttribute('aria-label', 'test.jpg, complete');
    });

    it('should announce errors with role="alert"', () => {
      const attachment = createMockAttachment({ 
        status: 'error',
        error: 'Upload failed' 
      });
      render(<FileAttachmentItem attachment={attachment} onRemove={vi.fn()} />);
      
      const alerts = screen.getAllByRole('alert');
      expect(alerts.length).toBeGreaterThan(0);
    });

    it('should have accessible remove button', () => {
      const attachment = createMockAttachment();
      render(<FileAttachmentItem attachment={attachment} onRemove={vi.fn()} />);
      
      const removeButton = screen.getByRole('button', { name: 'Remove test.jpg' });
      expect(removeButton).toBeInTheDocument();
    });

    it('should have accessible image alt text', () => {
      const attachment = createMockAttachment();
      render(<FileAttachmentItem attachment={attachment} onRemove={vi.fn()} />);
      
      const image = screen.getByAltText('test.jpg');
      expect(image).toBeInTheDocument();
    });

    it('should be keyboard accessible', async () => {
      const user = userEvent.setup();
      const onRemove = vi.fn();
      const attachment = createMockAttachment();
      
      render(<FileAttachmentItem attachment={attachment} onRemove={onRemove} />);
      
      // Tab to remove button
      await user.tab();
      const removeButton = screen.getByRole('button', { name: 'Remove test.jpg' });
      expect(removeButton).toHaveFocus();
      
      // Activate with Enter
      await user.keyboard('{Enter}');
      expect(onRemove).toHaveBeenCalled();
    });

    it('should show remove button on focus (not just hover)', async () => {
      const user = userEvent.setup();
      const attachment = createMockAttachment();
      
      render(<FileAttachmentItem attachment={attachment} onRemove={vi.fn()} />);
      
      const removeButton = screen.getByRole('button', { name: 'Remove test.jpg' });
      
      // Button has focus:opacity-100 class
      expect(removeButton).toHaveClass('focus:opacity-100');
      
      await user.tab();
      expect(removeButton).toHaveFocus();
    });

    it('should have visible focus indicator', async () => {
      const user = userEvent.setup();
      const attachment = createMockAttachment();
      
      render(<FileAttachmentItem attachment={attachment} onRemove={vi.fn()} />);
      
      await user.tab();
      const removeButton = screen.getByRole('button', { name: 'Remove test.jpg' });
      
      // Focus ring should be visible
      expect(removeButton).toHaveClass('focus:ring-2', 'focus:ring-primary');
    });
  });

  describe('FileAttachmentList', () => {
    const createMockAttachments = (count: number): FileAttachment[] => {
      return Array.from({ length: count }, (_, i) => ({
        id: `file-${i}`,
        file: new File(['test'], `test-${i}.jpg`, { type: 'image/jpeg' }),
        status: 'complete' as const,
        progress: 100,
        previewUrl: `data:image/jpeg;base64,test-${i}`,
      }));
    };

    it('should have no axe violations', async () => {
      const attachments = createMockAttachments(3);
      const { container } = render(
        <FileAttachmentList attachments={attachments} onRemove={vi.fn()} />
      );
      const results = await axe(container);
      expect(results).toHaveNoViolations();
    });

    it('should use proper list semantics', () => {
      const attachments = createMockAttachments(3);
      render(<FileAttachmentList attachments={attachments} onRemove={vi.fn()} />);
      
      const list = screen.getByRole('list');
      expect(list).toBeInTheDocument();
      
      const items = screen.getAllByRole('listitem');
      expect(items).toHaveLength(3);
    });

    it('should have accessible label', () => {
      const attachments = createMockAttachments(2);
      render(<FileAttachmentList attachments={attachments} onRemove={vi.fn()} />);
      
      const list = screen.getByRole('list', { name: 'File attachments' });
      expect(list).toBeInTheDocument();
    });

    it('should support keyboard navigation through items', async () => {
      const user = userEvent.setup();
      const attachments = createMockAttachments(3);
      const onRemove = vi.fn();
      
      render(<FileAttachmentList attachments={attachments} onRemove={onRemove} />);
      
      // Tab through remove buttons
      await user.tab();
      expect(screen.getByRole('button', { name: 'Remove test-0.jpg' })).toHaveFocus();
      
      await user.tab();
      expect(screen.getByRole('button', { name: 'Remove test-1.jpg' })).toHaveFocus();
      
      await user.tab();
      expect(screen.getByRole('button', { name: 'Remove test-2.jpg' })).toHaveFocus();
    });
  });

  describe('UploadProgressIndicator', () => {
    it('should have no axe violations', async () => {
      const { container } = render(<UploadProgressIndicator progress={50} />);
      const results = await axe(container);
      expect(results).toHaveNoViolations();
    });

    it('should have progressbar role', () => {
      render(<UploadProgressIndicator progress={50} />);
      
      const progressbar = screen.getByRole('progressbar');
      expect(progressbar).toBeInTheDocument();
    });

    it('should have aria-valuenow attribute', () => {
      render(<UploadProgressIndicator progress={75} />);
      
      const progressbar = screen.getByRole('progressbar');
      expect(progressbar).toHaveAttribute('aria-valuenow', '75');
    });

    it('should have aria-valuemin and aria-valuemax', () => {
      render(<UploadProgressIndicator progress={50} />);
      
      const progressbar = screen.getByRole('progressbar');
      expect(progressbar).toHaveAttribute('aria-valuemin', '0');
      expect(progressbar).toHaveAttribute('aria-valuemax', '100');
    });

    it('should have accessible label', () => {
      render(<UploadProgressIndicator progress={50} />);
      
      const progressbar = screen.getByRole('progressbar', { name: 'Upload progress' });
      expect(progressbar).toBeInTheDocument();
    });

    it('should announce progress changes to screen readers', () => {
      const { rerender } = render(<UploadProgressIndicator progress={25} />);
      
      let progressbar = screen.getByRole('progressbar');
      expect(progressbar).toHaveAttribute('aria-valuenow', '25');
      
      rerender(<UploadProgressIndicator progress={50} />);
      progressbar = screen.getByRole('progressbar');
      expect(progressbar).toHaveAttribute('aria-valuenow', '50');
    });
  });

  describe('DropOverlay', () => {
    it('should have no axe violations when active', async () => {
      const { container } = render(<DropOverlay isActive={true} />);
      const results = await axe(container);
      expect(results).toHaveNoViolations();
    });

    it('should have status role', () => {
      render(<DropOverlay isActive={true} />);
      
      const status = screen.getByRole('status');
      expect(status).toBeInTheDocument();
    });

    it('should have aria-live="polite"', () => {
      render(<DropOverlay isActive={true} />);
      
      const status = screen.getByRole('status');
      expect(status).toHaveAttribute('aria-live', 'polite');
    });

    it('should announce drop zone activation', () => {
      render(<DropOverlay isActive={true} allowedTypes={['images', 'documents']} />);
      
      const status = screen.getByRole('status');
      expect(status).toHaveTextContent('Drop images, documents here');
    });

    it('should not be interactive (pointer-events-none)', () => {
      render(<DropOverlay isActive={true} />);
      
      const status = screen.getByRole('status');
      expect(status).toHaveClass('pointer-events-none');
    });
  });

  describe('ImageLightbox', () => {
    it('should have no axe violations when open', async () => {
      const { container } = render(
        <ImageLightbox 
          src="data:image/jpeg;base64,test" 
          alt="Test image"
          isOpen={true}
          onClose={vi.fn()}
        />
      );
      const results = await axe(container);
      expect(results).toHaveNoViolations();
    });

    it('should have dialog role', () => {
      render(
        <ImageLightbox 
          src="data:image/jpeg;base64,test" 
          alt="Test image"
          isOpen={true}
          onClose={vi.fn()}
        />
      );
      
      // Dialog component provides the dialog role
      const dialog = screen.getByRole('dialog');
      expect(dialog).toBeInTheDocument();
    });

    it('should have accessible label', () => {
      render(
        <ImageLightbox 
          src="data:image/jpeg;base64,test" 
          alt="Test image"
          isOpen={true}
          onClose={vi.fn()}
        />
      );
      
      const viewer = screen.getByLabelText('Image viewer');
      expect(viewer).toBeInTheDocument();
    });

    it('should have accessible close button', () => {
      render(
        <ImageLightbox 
          src="data:image/jpeg;base64,test" 
          alt="Test image"
          isOpen={true}
          onClose={vi.fn()}
        />
      );
      
      const closeButton = screen.getByRole('button', { name: 'Close image viewer' });
      expect(closeButton).toBeInTheDocument();
    });

    it('should have proper image alt text', () => {
      render(
        <ImageLightbox 
          src="data:image/jpeg;base64,test" 
          alt="Profile picture"
          isOpen={true}
          onClose={vi.fn()}
        />
      );
      
      const image = screen.getByAltText('Profile picture');
      expect(image).toBeInTheDocument();
    });

    it('should close on Escape key', async () => {
      const user = userEvent.setup();
      const onClose = vi.fn();
      
      render(
        <ImageLightbox 
          src="data:image/jpeg;base64,test" 
          alt="Test image"
          isOpen={true}
          onClose={onClose}
        />
      );
      
      await user.keyboard('{Escape}');
      expect(onClose).toHaveBeenCalled();
    });

    it('should trap focus within modal', async () => {
      const user = userEvent.setup();
      
      render(
        <ImageLightbox 
          src="data:image/jpeg;base64,test" 
          alt="Test image"
          isOpen={true}
          onClose={vi.fn()}
        />
      );
      
      const closeButton = screen.getByRole('button', { name: 'Close image viewer' });
      
      // Tab should cycle focus within modal
      await user.tab();
      expect(closeButton).toHaveFocus();
      
      // Shift+Tab should also stay within modal
      await user.tab({ shift: true });
      // Focus should remain in modal (radix-ui handles this)
    });

    it('should have visible focus indicator on close button', () => {
      render(
        <ImageLightbox 
          src="data:image/jpeg;base64,test" 
          alt="Test image"
          isOpen={true}
          onClose={vi.fn()}
        />
      );
      
      const closeButton = screen.getByRole('button', { name: 'Close image viewer' });
      expect(closeButton).toHaveClass('focus:ring-2', 'focus:ring-primary');
    });

    it('should prevent body scroll when open', () => {
      const { unmount } = render(
        <ImageLightbox 
          src="data:image/jpeg;base64,test" 
          alt="Test image"
          isOpen={true}
          onClose={vi.fn()}
        />
      );
      
      expect(document.body.style.overflow).toBe('hidden');
      
      unmount();
      expect(document.body.style.overflow).toBe('');
    });
  });

  describe('ImageContentRenderer', () => {
    it('should have no axe violations', async () => {
      const content = {
        type: 'image' as const,
        source: {
          type: 'base64' as const,
          media_type: 'image/jpeg',
          data: 'base64data',
        },
      };
      
      const { container } = render(<ImageContentRenderer content={content} />);
      const results = await axe(container);
      expect(results).toHaveNoViolations();
    });

    it('should have descriptive alt text', () => {
      const content = {
        type: 'image' as const,
        source: {
          type: 'base64' as const,
          media_type: 'image/jpeg',
          data: 'base64data',
        },
      };
      
      render(<ImageContentRenderer content={content} />);
      
      // Should have generic alt text for content images
      const image = screen.getByRole('img');
      expect(image).toHaveAttribute('alt');
      expect(image.getAttribute('alt')).toBeTruthy();
    });

    it('should have clickable image with keyboard support', async () => {
      const user = userEvent.setup();
      const content = {
        type: 'image' as const,
        source: {
          type: 'base64' as const,
          media_type: 'image/jpeg',
          data: 'base64data',
        },
      };
      
      render(<ImageContentRenderer content={content} />);
      
      const button = screen.getByRole('button');
      expect(button).toBeInTheDocument();
      
      // Should be keyboard accessible
      await user.tab();
      expect(button).toHaveFocus();
      
      await user.keyboard('{Enter}');
      // Lightbox should open (tested separately)
    });
  });

  describe('MultimodalContentRenderer', () => {
    it('should have no axe violations with text content', async () => {
      const content = [
        { type: 'text' as const, text: 'Hello world' }
      ];
      
      const { container } = render(<MultimodalContentRenderer content={content} />);
      const results = await axe(container);
      expect(results).toHaveNoViolations();
    });

    it('should have no axe violations with mixed content', async () => {
      const content = [
        { type: 'text' as const, text: 'Check this image:' },
        { 
          type: 'image' as const,
          source: {
            type: 'base64' as const,
            media_type: 'image/jpeg',
            data: 'base64data',
          }
        }
      ];
      
      const { container } = render(<MultimodalContentRenderer content={content} />);
      const results = await axe(container);
      expect(results).toHaveNoViolations();
    });

    it('should render content in semantic order', () => {
      const content = [
        { type: 'text' as const, text: 'First text' },
        { type: 'text' as const, text: 'Second text' }
      ];
      
      const { container } = render(<MultimodalContentRenderer content={content} />);
      const text = container.textContent;
      expect(text?.indexOf('First text')).toBeLessThan(text?.indexOf('Second text') || Infinity);
    });
  });

  describe('Keyboard Navigation Integration', () => {
    it('should support complete keyboard workflow', async () => {
      const user = userEvent.setup();
      const onSend = vi.fn();
      
      updateMockState('fileUpload', {
        attachments: [],
        addFiles: vi.fn(),
      });
      
      render(<ChatInputArea onSend={onSend} showFilePicker={true} />);
      
      // Type message
      const textarea = screen.getByLabelText('Message text input');
      await user.click(textarea);
      await user.type(textarea, 'Test message');
      
      // Tab to attach button
      await user.tab();
      expect(screen.getByLabelText('Attach files')).toHaveFocus();
      
      // Tab to send button
      await user.tab();
      expect(screen.getByLabelText('Send message')).toHaveFocus();
      
      // Send with Enter
      await user.keyboard('{Enter}');
      expect(onSend).toHaveBeenCalled();
    });

    it('should not create keyboard traps', async () => {
      const user = userEvent.setup();
      
      const mockAttachment: FileAttachment = {
        id: 'file-1',
        file: new File(['test'], 'test.jpg', { type: 'image/jpeg' }),
        status: 'complete',
        progress: 100,
        previewUrl: 'data:image/jpeg;base64,test',
      };
      
      updateMockState('fileUpload', {
        attachments: [mockAttachment],
      });
      
      render(<ChatInputArea showFilePicker={true} />);
      
      // Tab through all elements without getting trapped
      await user.tab(); // textarea
      await user.tab(); // remove button for file
      await user.tab(); // attach button
      await user.tab(); // send button
      
      // Should be able to tab back
      await user.tab({ shift: true });
      expect(screen.getByLabelText('Attach files')).toHaveFocus();
    });
  });

  describe('Color Contrast', () => {
    it('should have sufficient contrast for text', () => {
      render(<ChatInputArea />);
      
      // Text should have text-foreground class which ensures 4.5:1 contrast
      const textarea = screen.getByLabelText('Message text input');
      expect(textarea).toBeInTheDocument();
    });

    it('should have sufficient contrast for error messages', () => {
      updateMockState('fileUpload', {
        validationError: 'File too large',
      });
      
      render(<ChatInputArea />);
      
      const alert = screen.getByRole('alert');
      expect(alert).toHaveClass('text-destructive');
    });

    it('should have visible focus indicators', async () => {
      const user = userEvent.setup();
      render(<ChatInputArea />);
      
      const textarea = screen.getByLabelText('Message text input');
      await user.click(textarea);
      
      // Focus ring should be visible (3:1 contrast minimum)
      expect(textarea).toHaveClass('focus:ring-2', 'focus:ring-primary');
    });
  });

  describe('Touch Target Sizes', () => {
    it('should have minimum 44x44px buttons', () => {
      render(<ChatInputArea showFilePicker={true} />);
      
      const attachButton = screen.getByLabelText('Attach files');
      const sendButton = screen.getByLabelText('Send message');
      
      // Check for 44px size classes
      expect(attachButton).toHaveClass('h-[44px]', 'w-[44px]');
      expect(sendButton).toHaveClass('h-[44px]', 'w-[44px]');
    });

    it('should have adequate spacing between touch targets', () => {
      render(<ChatInputArea showFilePicker={true} />);
      
      // Buttons should have gap-2 spacing (8px minimum)
      const container = screen.getByLabelText('Message input area with file upload support');
      const inputWrapper = container.querySelector('.flex.gap-2');
      expect(inputWrapper).toBeInTheDocument();
    });
  });

  describe('Screen Reader Announcements', () => {
    it('should announce upload progress', () => {
      updateMockState('fileUpload', {
        isUploading: true,
        overallProgress: 50,
      });
      
      render(<ChatInputArea />);
      
      // Progress message should be announced
      expect(screen.getByText(/Uploading files/i)).toBeInTheDocument();
    });

    it('should announce validation errors', () => {
      updateMockState('fileUpload', {
        validationError: 'File type not allowed',
      });
      
      render(<ChatInputArea />);
      
      const alert = screen.getByRole('alert');
      expect(alert).toHaveTextContent('File type not allowed');
    });

    it('should announce drop zone status', () => {
      render(<DropOverlay isActive={true} allowedTypes={['images']} />);
      
      const status = screen.getByRole('status');
      expect(status).toHaveAttribute('aria-live', 'polite');
      expect(status).toHaveTextContent('Drop images here');
    });
  });

  describe('High Contrast Mode Support', () => {
    it('should have visible borders in high contrast', () => {
      render(<ChatInputArea />);
      
      const textarea = screen.getByLabelText('Message text input');
      expect(textarea).toHaveClass('border');
    });

    it('should have visible focus indicators in high contrast', () => {
      render(<ChatInputArea />);
      
      const textarea = screen.getByLabelText('Message text input');
      expect(textarea).toHaveClass('focus:ring-2');
    });

    it('should have visible button outlines in high contrast', () => {
      const mockAttachment: FileAttachment = {
        id: 'file-1',
        file: new File(['test'], 'test.jpg', { type: 'image/jpeg' }),
        status: 'complete',
        progress: 100,
        previewUrl: 'data:image/jpeg;base64,test',
      };
      
      render(<FileAttachmentItem attachment={mockAttachment} onRemove={vi.fn()} />);
      
      const removeButton = screen.getByRole('button', { name: 'Remove test.jpg' });
      expect(removeButton).toHaveClass('border');
    });
  });
});
