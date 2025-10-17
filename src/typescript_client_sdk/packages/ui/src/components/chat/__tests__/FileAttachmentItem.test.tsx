/**
 * FileAttachmentItem Component Tests
 * Testing file attachment display, status states, progress, and interactions
 */

import React from 'react';
import { describe, it, expect, vi, beforeEach } from 'vitest';
import { render, screen, within } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { FileAttachmentItem } from '../FileAttachmentItem';
import type { FileAttachment } from '@agentc/realtime-react';

// Mock lucide-react icons
vi.mock('lucide-react', () => ({
  FileIcon: ({ className }: any) => (
    <div data-testid="file-icon" className={className} />
  ),
  AlertCircle: ({ className }: any) => (
    <div data-testid="alert-circle-icon" className={className} />
  ),
  Check: ({ className }: any) => (
    <div data-testid="check-icon" className={className} />
  ),
  X: ({ className }: any) => (
    <div data-testid="x-icon" className={className} />
  ),
}));

// Mock UploadProgressIndicator component
vi.mock('../UploadProgressIndicator', () => ({
  UploadProgressIndicator: ({ progress, size }: any) => (
    <div 
      data-testid="upload-progress-indicator" 
      data-progress={progress}
      data-size={size}
    >
      Progress: {progress}%
    </div>
  ),
}));

describe('FileAttachmentItem Component', () => {
  let mockOnRemove: ReturnType<typeof vi.fn>;

  beforeEach(() => {
    mockOnRemove = vi.fn();
  });

  // Helper to create a File object
  const createMockFile = (name: string, type: string): File => {
    return new File(['content'], name, { type });
  };

  // Helper to create a complete FileAttachment object
  const createAttachment = (overrides?: Partial<FileAttachment>): FileAttachment => ({
    file: createMockFile('test-file.txt', 'text/plain'),
    id: null,
    status: 'pending',
    progress: 0,
    error: null,
    previewUrl: null,
    ...overrides,
  });

  describe('Basic Rendering', () => {
    it('should render file attachment item', () => {
      const attachment = createAttachment();
      
      render(<FileAttachmentItem attachment={attachment} onRemove={mockOnRemove} />);
      
      const listItem = screen.getByRole('listitem');
      expect(listItem).toBeInTheDocument();
    });

    it('should display file icon for non-image files', () => {
      const attachment = createAttachment({
        file: createMockFile('document.pdf', 'application/pdf'),
      });
      
      render(<FileAttachmentItem attachment={attachment} onRemove={mockOnRemove} />);
      
      expect(screen.getByTestId('file-icon')).toBeInTheDocument();
    });

    it('should display remove button', () => {
      const attachment = createAttachment();
      
      render(<FileAttachmentItem attachment={attachment} onRemove={mockOnRemove} />);
      
      const removeButton = screen.getByRole('button', { name: /remove test-file\.txt/i });
      expect(removeButton).toBeInTheDocument();
    });

    it('should have correct default structure', () => {
      const attachment = createAttachment();
      const { container } = render(
        <FileAttachmentItem attachment={attachment} onRemove={mockOnRemove} />
      );
      
      const listItem = screen.getByRole('listitem');
      expect(listItem).toHaveClass('relative', 'group');
      expect(listItem).toHaveClass('border', 'rounded-lg', 'overflow-hidden');
      expect(listItem).toHaveClass('transition-all', 'duration-200');
    });
  });

  describe('Size Variants', () => {
    it('should apply sm size classes', () => {
      const attachment = createAttachment();
      const { container } = render(
        <FileAttachmentItem attachment={attachment} onRemove={mockOnRemove} size="sm" />
      );
      
      const listItem = screen.getByRole('listitem');
      expect(listItem).toHaveClass('h-16', 'w-16');
    });

    it('should apply md size classes (default)', () => {
      const attachment = createAttachment();
      const { container } = render(
        <FileAttachmentItem attachment={attachment} onRemove={mockOnRemove} />
      );
      
      const listItem = screen.getByRole('listitem');
      expect(listItem).toHaveClass('h-20', 'w-20');
    });

    it('should apply lg size classes', () => {
      const attachment = createAttachment();
      const { container } = render(
        <FileAttachmentItem attachment={attachment} onRemove={mockOnRemove} size="lg" />
      );
      
      const listItem = screen.getByRole('listitem');
      expect(listItem).toHaveClass('h-24', 'w-24');
    });

    it('should default to md when size is not provided', () => {
      const attachment = createAttachment();
      const { container } = render(
        <FileAttachmentItem attachment={attachment} onRemove={mockOnRemove} />
      );
      
      const listItem = screen.getByRole('listitem');
      expect(listItem).toHaveClass('h-20', 'w-20');
      expect(listItem).not.toHaveClass('h-16', 'w-16');
      expect(listItem).not.toHaveClass('h-24', 'w-24');
    });
  });

  describe('Status States', () => {
    it('should render pending status', () => {
      const attachment = createAttachment({ status: 'pending' });
      
      render(<FileAttachmentItem attachment={attachment} onRemove={mockOnRemove} />);
      
      const listItem = screen.getByRole('listitem');
      expect(listItem).toHaveAttribute('aria-label', expect.stringContaining('pending'));
    });

    it('should render uploading status with progress indicator', () => {
      const attachment = createAttachment({
        status: 'uploading',
        progress: 45,
      });
      
      render(<FileAttachmentItem attachment={attachment} onRemove={mockOnRemove} />);
      
      const listItem = screen.getByRole('listitem');
      expect(listItem).toHaveAttribute('aria-label', expect.stringContaining('uploading'));
      
      const progressIndicator = screen.getByTestId('upload-progress-indicator');
      expect(progressIndicator).toBeInTheDocument();
      expect(progressIndicator).toHaveAttribute('data-progress', '45');
    });

    it('should render complete status with checkmark', () => {
      const attachment = createAttachment({
        status: 'complete',
        progress: 100,
      });
      
      render(<FileAttachmentItem attachment={attachment} onRemove={mockOnRemove} />);
      
      const listItem = screen.getByRole('listitem');
      expect(listItem).toHaveAttribute('aria-label', expect.stringContaining('complete'));
      expect(listItem).toHaveClass('border-primary');
      
      expect(screen.getByTestId('check-icon')).toBeInTheDocument();
    });

    it('should render error status with alert icon', () => {
      const attachment = createAttachment({
        status: 'error',
        error: 'Upload failed',
      });
      
      render(<FileAttachmentItem attachment={attachment} onRemove={mockOnRemove} />);
      
      const listItem = screen.getByRole('listitem');
      expect(listItem).toHaveAttribute('aria-label', expect.stringContaining('error'));
      expect(listItem).toHaveClass('border-destructive');
      
      expect(screen.getByTestId('alert-circle-icon')).toBeInTheDocument();
    });

    it('should apply correct border color for pending status', () => {
      const attachment = createAttachment({ status: 'pending' });
      
      render(<FileAttachmentItem attachment={attachment} onRemove={mockOnRemove} />);
      
      const listItem = screen.getByRole('listitem');
      expect(listItem).not.toHaveClass('border-destructive');
      expect(listItem).not.toHaveClass('border-primary');
    });

    it('should apply correct border color for complete status', () => {
      const attachment = createAttachment({ status: 'complete' });
      
      render(<FileAttachmentItem attachment={attachment} onRemove={mockOnRemove} />);
      
      const listItem = screen.getByRole('listitem');
      expect(listItem).toHaveClass('border-primary');
    });

    it('should apply correct border color for error status', () => {
      const attachment = createAttachment({ status: 'error' });
      
      render(<FileAttachmentItem attachment={attachment} onRemove={mockOnRemove} />);
      
      const listItem = screen.getByRole('listitem');
      expect(listItem).toHaveClass('border-destructive');
    });
  });

  describe('Upload Progress Display', () => {
    it('should display progress indicator during upload', () => {
      const attachment = createAttachment({
        status: 'uploading',
        progress: 60,
      });
      
      render(<FileAttachmentItem attachment={attachment} onRemove={mockOnRemove} />);
      
      const progressIndicator = screen.getByTestId('upload-progress-indicator');
      expect(progressIndicator).toBeInTheDocument();
      expect(progressIndicator).toHaveAttribute('data-progress', '60');
    });

    it('should pass sm size to progress indicator', () => {
      const attachment = createAttachment({
        status: 'uploading',
        progress: 50,
      });
      
      render(<FileAttachmentItem attachment={attachment} onRemove={mockOnRemove} />);
      
      const progressIndicator = screen.getByTestId('upload-progress-indicator');
      expect(progressIndicator).toHaveAttribute('data-size', 'sm');
    });

    it('should display progress overlay with proper styling', () => {
      const attachment = createAttachment({
        status: 'uploading',
        progress: 30,
      });
      
      const { container } = render(
        <FileAttachmentItem attachment={attachment} onRemove={mockOnRemove} />
      );
      
      const overlay = container.querySelector('.absolute.inset-0.bg-background\\/80');
      expect(overlay).toBeInTheDocument();
      expect(overlay).toHaveClass('flex', 'items-center', 'justify-center');
    });

    it('should not show progress indicator when not uploading', () => {
      const attachment = createAttachment({ status: 'complete' });
      
      render(<FileAttachmentItem attachment={attachment} onRemove={mockOnRemove} />);
      
      expect(screen.queryByTestId('upload-progress-indicator')).not.toBeInTheDocument();
    });

    it('should update progress indicator value', () => {
      const attachment = createAttachment({
        status: 'uploading',
        progress: 25,
      });
      
      const { rerender } = render(
        <FileAttachmentItem attachment={attachment} onRemove={mockOnRemove} />
      );
      
      let progressIndicator = screen.getByTestId('upload-progress-indicator');
      expect(progressIndicator).toHaveAttribute('data-progress', '25');
      
      const updatedAttachment = { ...attachment, progress: 75 };
      rerender(<FileAttachmentItem attachment={updatedAttachment} onRemove={mockOnRemove} />);
      
      progressIndicator = screen.getByTestId('upload-progress-indicator');
      expect(progressIndicator).toHaveAttribute('data-progress', '75');
    });
  });

  describe('Image Preview Display', () => {
    it('should display image preview for image files with previewUrl', () => {
      const attachment = createAttachment({
        file: createMockFile('photo.jpg', 'image/jpeg'),
        previewUrl: 'data:image/jpeg;base64,/9j/4AAQSkZJRg==',
      });
      
      render(<FileAttachmentItem attachment={attachment} onRemove={mockOnRemove} />);
      
      const img = screen.getByRole('img');
      expect(img).toBeInTheDocument();
      expect(img).toHaveAttribute('src', 'data:image/jpeg;base64,/9j/4AAQSkZJRg==');
      expect(img).toHaveAttribute('alt', 'photo.jpg');
    });

    it('should apply object-cover to image preview', () => {
      const attachment = createAttachment({
        file: createMockFile('photo.png', 'image/png'),
        previewUrl: 'blob:http://localhost/preview',
      });
      
      render(<FileAttachmentItem attachment={attachment} onRemove={mockOnRemove} />);
      
      const img = screen.getByRole('img');
      expect(img).toHaveClass('h-full', 'w-full', 'object-cover');
    });

    it('should display file icon for image files without previewUrl', () => {
      const attachment = createAttachment({
        file: createMockFile('photo.jpg', 'image/jpeg'),
        previewUrl: null,
      });
      
      render(<FileAttachmentItem attachment={attachment} onRemove={mockOnRemove} />);
      
      expect(screen.queryByRole('img')).not.toBeInTheDocument();
      expect(screen.getByTestId('file-icon')).toBeInTheDocument();
    });

    it('should display file icon for non-image files', () => {
      const attachment = createAttachment({
        file: createMockFile('document.pdf', 'application/pdf'),
      });
      
      render(<FileAttachmentItem attachment={attachment} onRemove={mockOnRemove} />);
      
      expect(screen.queryByRole('img')).not.toBeInTheDocument();
      expect(screen.getByTestId('file-icon')).toBeInTheDocument();
    });

    it('should apply muted background for file icon container', () => {
      const attachment = createAttachment({
        file: createMockFile('file.txt', 'text/plain'),
      });
      
      const { container } = render(
        <FileAttachmentItem attachment={attachment} onRemove={mockOnRemove} />
      );
      
      const iconContainer = container.querySelector('.bg-muted');
      expect(iconContainer).toBeInTheDocument();
      expect(iconContainer).toHaveClass('h-full', 'w-full', 'flex', 'items-center', 'justify-center');
    });

    it('should size file icon correctly', () => {
      const attachment = createAttachment();
      
      render(<FileAttachmentItem attachment={attachment} onRemove={mockOnRemove} />);
      
      const fileIcon = screen.getByTestId('file-icon');
      expect(fileIcon).toHaveClass('h-8', 'w-8', 'text-muted-foreground');
    });
  });

  describe('Remove Button Interaction', () => {
    it('should call onRemove when clicked', async () => {
      const user = userEvent.setup();
      const attachment = createAttachment();
      
      render(<FileAttachmentItem attachment={attachment} onRemove={mockOnRemove} />);
      
      const removeButton = screen.getByRole('button', { name: /remove test-file\.txt/i });
      await user.click(removeButton);
      
      expect(mockOnRemove).toHaveBeenCalledTimes(1);
    });

    it('should have correct aria-label with filename', () => {
      const attachment = createAttachment({
        file: createMockFile('my-document.pdf', 'application/pdf'),
      });
      
      render(<FileAttachmentItem attachment={attachment} onRemove={mockOnRemove} />);
      
      const removeButton = screen.getByRole('button', { name: 'Remove my-document.pdf' });
      expect(removeButton).toBeInTheDocument();
    });

    it('should have X icon', () => {
      const attachment = createAttachment();
      
      render(<FileAttachmentItem attachment={attachment} onRemove={mockOnRemove} />);
      
      const xIcon = screen.getByTestId('x-icon');
      expect(xIcon).toBeInTheDocument();
      expect(xIcon).toHaveClass('h-4', 'w-4');
    });

    it('should have correct positioning (top-left)', () => {
      const attachment = createAttachment();
      
      render(<FileAttachmentItem attachment={attachment} onRemove={mockOnRemove} />);
      
      const removeButton = screen.getByRole('button', { name: /remove/i });
      expect(removeButton).toHaveClass('absolute', 'top-1', 'left-1');
    });

    it('should have hover opacity transition', () => {
      const attachment = createAttachment();
      
      render(<FileAttachmentItem attachment={attachment} onRemove={mockOnRemove} />);
      
      const removeButton = screen.getByRole('button', { name: /remove/i });
      expect(removeButton).toHaveClass('opacity-0', 'group-hover:opacity-100');
      expect(removeButton).toHaveClass('transition-opacity', 'duration-200');
    });

    it('should have hover background color', () => {
      const attachment = createAttachment();
      
      render(<FileAttachmentItem attachment={attachment} onRemove={mockOnRemove} />);
      
      const removeButton = screen.getByRole('button', { name: /remove/i });
      expect(removeButton).toHaveClass('hover:bg-destructive', 'hover:border-destructive', 'hover:text-destructive-foreground');
    });

    it('should have focus styles', () => {
      const attachment = createAttachment();
      
      render(<FileAttachmentItem attachment={attachment} onRemove={mockOnRemove} />);
      
      const removeButton = screen.getByRole('button', { name: /remove/i });
      expect(removeButton).toHaveClass('focus:opacity-100', 'focus:outline-none', 'focus:ring-2', 'focus:ring-primary');
    });

    it('should have button type="button"', () => {
      const attachment = createAttachment();
      
      render(<FileAttachmentItem attachment={attachment} onRemove={mockOnRemove} />);
      
      const removeButton = screen.getByRole('button', { name: /remove/i });
      expect(removeButton).toHaveAttribute('type', 'button');
    });
  });

  describe('Complete Status Checkmark', () => {
    it('should display checkmark for complete files', () => {
      const attachment = createAttachment({ status: 'complete' });
      
      render(<FileAttachmentItem attachment={attachment} onRemove={mockOnRemove} />);
      
      const checkIcon = screen.getByTestId('check-icon');
      expect(checkIcon).toBeInTheDocument();
    });

    it('should position checkmark at top-right', () => {
      const attachment = createAttachment({ status: 'complete' });
      const { container } = render(
        <FileAttachmentItem attachment={attachment} onRemove={mockOnRemove} />
      );
      
      const checkmarkContainer = container.querySelector('.absolute.top-1.right-1');
      expect(checkmarkContainer).toBeInTheDocument();
      expect(checkmarkContainer).toHaveClass('bg-primary', 'rounded-full', 'p-1');
    });

    it('should size checkmark correctly', () => {
      const attachment = createAttachment({ status: 'complete' });
      
      render(<FileAttachmentItem attachment={attachment} onRemove={mockOnRemove} />);
      
      const checkIcon = screen.getByTestId('check-icon');
      expect(checkIcon).toHaveClass('h-3', 'w-3', 'text-primary-foreground');
    });

    it('should not show checkmark for non-complete statuses', () => {
      const statuses: Array<'pending' | 'uploading' | 'error'> = ['pending', 'uploading', 'error'];
      
      statuses.forEach(status => {
        const { unmount } = render(
          <FileAttachmentItem 
            attachment={createAttachment({ status })} 
            onRemove={mockOnRemove} 
          />
        );
        
        expect(screen.queryByTestId('check-icon')).not.toBeInTheDocument();
        unmount();
      });
    });
  });

  describe('Error Overlay', () => {
    it('should display error overlay for error status', () => {
      const attachment = createAttachment({
        status: 'error',
        error: 'Upload failed',
      });
      
      const { container } = render(
        <FileAttachmentItem attachment={attachment} onRemove={mockOnRemove} />
      );
      
      const errorOverlay = container.querySelector('.absolute.inset-0.bg-destructive\\/90');
      expect(errorOverlay).toBeInTheDocument();
      expect(errorOverlay).toHaveClass('flex', 'items-center', 'justify-center');
    });

    it('should have alert role on error overlay', () => {
      const attachment = createAttachment({
        status: 'error',
        error: 'Network error',
      });
      
      const { container } = render(
        <FileAttachmentItem attachment={attachment} onRemove={mockOnRemove} />
      );
      
      const errorOverlay = container.querySelector('[role="alert"]');
      expect(errorOverlay).toBeInTheDocument();
    });

    it('should display alert icon in error overlay', () => {
      const attachment = createAttachment({ status: 'error' });
      
      render(<FileAttachmentItem attachment={attachment} onRemove={mockOnRemove} />);
      
      const alertIcon = screen.getByTestId('alert-circle-icon');
      expect(alertIcon).toBeInTheDocument();
      expect(alertIcon).toHaveClass('h-6', 'w-6', 'text-destructive-foreground');
    });

    it('should not show error overlay for non-error statuses', () => {
      const statuses: Array<'pending' | 'uploading' | 'complete'> = ['pending', 'uploading', 'complete'];
      
      statuses.forEach(status => {
        const { container, unmount } = render(
          <FileAttachmentItem 
            attachment={createAttachment({ status })} 
            onRemove={mockOnRemove} 
          />
        );
        
        const errorOverlay = container.querySelector('.bg-destructive\\/90');
        expect(errorOverlay).not.toBeInTheDocument();
        unmount();
      });
    });
  });

  describe('Filename Tooltip', () => {
    it('should display filename in tooltip', () => {
      const attachment = createAttachment({
        file: createMockFile('my-important-file.pdf', 'application/pdf'),
      });
      
      const { container } = render(
        <FileAttachmentItem attachment={attachment} onRemove={mockOnRemove} />
      );
      
      const tooltip = container.querySelector('.absolute.-bottom-8') as HTMLElement;
      expect(tooltip).toHaveTextContent('my-important-file.pdf');
    });

    it('should position tooltip at bottom', () => {
      const attachment = createAttachment();
      const { container } = render(
        <FileAttachmentItem attachment={attachment} onRemove={mockOnRemove} />
      );
      
      // Get the filename tooltip (not error tooltip)
      const tooltips = container.querySelectorAll('.absolute.-bottom-8');
      const filenameTooltip = Array.from(tooltips).find(el => 
        el.classList.contains('bg-background')
      );
      
      expect(filenameTooltip).toBeInTheDocument();
      expect(filenameTooltip).toHaveClass('absolute', '-bottom-8', 'left-0', 'right-0');
    });

    it('should have hover opacity transition', () => {
      const attachment = createAttachment();
      const { container } = render(
        <FileAttachmentItem attachment={attachment} onRemove={mockOnRemove} />
      );
      
      const tooltip = container.querySelector('.bg-background.border.rounded');
      expect(tooltip).toHaveClass('opacity-0', 'group-hover:opacity-100');
      expect(tooltip).toHaveClass('transition-opacity', 'duration-200');
    });

    it('should have correct styling', () => {
      const attachment = createAttachment();
      const { container } = render(
        <FileAttachmentItem attachment={attachment} onRemove={mockOnRemove} />
      );
      
      const tooltip = container.querySelector('.bg-background.border.rounded');
      expect(tooltip).toHaveClass('px-2', 'py-1', 'text-xs', 'truncate', 'pointer-events-none');
    });

    it('should truncate long filenames', () => {
      const attachment = createAttachment({
        file: createMockFile(
          'very-long-filename-that-should-be-truncated-in-the-tooltip-display.pdf',
          'application/pdf'
        ),
      });
      
      const { container } = render(
        <FileAttachmentItem attachment={attachment} onRemove={mockOnRemove} />
      );
      
      const tooltip = container.querySelector('.bg-background.border.rounded');
      expect(tooltip).toHaveClass('truncate');
    });
  });

  describe('Error Tooltip', () => {
    it('should display error message in tooltip', () => {
      const attachment = createAttachment({
        status: 'error',
        error: 'File size exceeds limit',
      });
      
      const { container } = render(
        <FileAttachmentItem attachment={attachment} onRemove={mockOnRemove} />
      );
      
      const errorTooltip = container.querySelector('.bg-destructive.text-destructive-foreground');
      expect(errorTooltip).toHaveTextContent('File size exceeds limit');
    });

    it('should have alert role on error tooltip', () => {
      const attachment = createAttachment({
        status: 'error',
        error: 'Network timeout',
      });
      
      const { container } = render(
        <FileAttachmentItem attachment={attachment} onRemove={mockOnRemove} />
      );
      
      // Get the error tooltip specifically (there are two role="alert" - overlay and tooltip)
      const alerts = container.querySelectorAll('[role="alert"]');
      expect(alerts.length).toBeGreaterThanOrEqual(2);
    });

    it('should not display error tooltip when error is null', () => {
      const attachment = createAttachment({
        status: 'complete',
        error: null,
      });
      
      const { container } = render(
        <FileAttachmentItem attachment={attachment} onRemove={mockOnRemove} />
      );
      
      const errorTooltip = container.querySelector('.bg-destructive.text-destructive-foreground');
      expect(errorTooltip).not.toBeInTheDocument();
    });

    it('should position error tooltip at bottom', () => {
      const attachment = createAttachment({
        status: 'error',
        error: 'Upload failed',
      });
      
      const { container } = render(
        <FileAttachmentItem attachment={attachment} onRemove={mockOnRemove} />
      );
      
      const errorTooltip = container.querySelector('.bg-destructive.text-destructive-foreground');
      expect(errorTooltip).toHaveClass('absolute', '-bottom-8', 'left-0', 'right-0');
    });

    it('should have hover opacity transition', () => {
      const attachment = createAttachment({
        status: 'error',
        error: 'Error occurred',
      });
      
      const { container } = render(
        <FileAttachmentItem attachment={attachment} onRemove={mockOnRemove} />
      );
      
      const errorTooltip = container.querySelector('.bg-destructive.text-destructive-foreground');
      expect(errorTooltip).toHaveClass('opacity-0', 'group-hover:opacity-100');
      expect(errorTooltip).toHaveClass('transition-opacity', 'duration-200');
    });

    it('should have correct styling', () => {
      const attachment = createAttachment({
        status: 'error',
        error: 'Test error',
      });
      
      const { container } = render(
        <FileAttachmentItem attachment={attachment} onRemove={mockOnRemove} />
      );
      
      const errorTooltip = container.querySelector('.bg-destructive.text-destructive-foreground');
      expect(errorTooltip).toHaveClass('px-2', 'py-1', 'text-xs', 'rounded');
    });
  });

  describe('Accessibility', () => {
    it('should have listitem role', () => {
      const attachment = createAttachment();
      
      render(<FileAttachmentItem attachment={attachment} onRemove={mockOnRemove} />);
      
      const listItem = screen.getByRole('listitem');
      expect(listItem).toBeInTheDocument();
    });

    it('should have aria-label with filename and status', () => {
      const attachment = createAttachment({
        file: createMockFile('document.pdf', 'application/pdf'),
        status: 'complete',
      });
      
      render(<FileAttachmentItem attachment={attachment} onRemove={mockOnRemove} />);
      
      const listItem = screen.getByRole('listitem');
      expect(listItem).toHaveAttribute('aria-label', 'document.pdf, complete');
    });

    it('should update aria-label when status changes', () => {
      const attachment = createAttachment({
        file: createMockFile('test.jpg', 'image/jpeg'),
        status: 'pending',
      });
      
      const { rerender } = render(
        <FileAttachmentItem attachment={attachment} onRemove={mockOnRemove} />
      );
      
      let listItem = screen.getByRole('listitem');
      expect(listItem).toHaveAttribute('aria-label', 'test.jpg, pending');
      
      const uploadingAttachment = { ...attachment, status: 'uploading' as const };
      rerender(<FileAttachmentItem attachment={uploadingAttachment} onRemove={mockOnRemove} />);
      
      listItem = screen.getByRole('listitem');
      expect(listItem).toHaveAttribute('aria-label', 'test.jpg, uploading');
    });

    it('should have alert role on error overlay', () => {
      const attachment = createAttachment({ status: 'error' });
      
      const { container } = render(
        <FileAttachmentItem attachment={attachment} onRemove={mockOnRemove} />
      );
      
      const alerts = container.querySelectorAll('[role="alert"]');
      expect(alerts.length).toBeGreaterThan(0);
    });

    it('should have accessible remove button label', () => {
      const attachment = createAttachment({
        file: createMockFile('report.docx', 'application/vnd.openxmlformats-officedocument.wordprocessingml.document'),
      });
      
      render(<FileAttachmentItem attachment={attachment} onRemove={mockOnRemove} />);
      
      const removeButton = screen.getByRole('button', { name: 'Remove report.docx' });
      expect(removeButton).toBeInTheDocument();
    });

    it('should have keyboard-accessible remove button', async () => {
      const user = userEvent.setup();
      const attachment = createAttachment();
      
      render(<FileAttachmentItem attachment={attachment} onRemove={mockOnRemove} />);
      
      const removeButton = screen.getByRole('button', { name: /remove/i });
      
      // Tab to button and press Enter
      await user.tab();
      await user.keyboard('{Enter}');
      
      expect(mockOnRemove).toHaveBeenCalled();
    });

    it('should provide alt text for image previews', () => {
      const attachment = createAttachment({
        file: createMockFile('vacation-photo.jpg', 'image/jpeg'),
        previewUrl: 'blob:http://localhost/preview',
      });
      
      render(<FileAttachmentItem attachment={attachment} onRemove={mockOnRemove} />);
      
      const img = screen.getByRole('img');
      expect(img).toHaveAttribute('alt', 'vacation-photo.jpg');
    });
  });

  describe('Custom Styling', () => {
    it('should apply custom className', () => {
      const attachment = createAttachment();
      
      render(
        <FileAttachmentItem 
          attachment={attachment} 
          onRemove={mockOnRemove}
          className="custom-attachment-class"
        />
      );
      
      const listItem = screen.getByRole('listitem');
      expect(listItem).toHaveClass('custom-attachment-class');
    });

    it('should preserve default classes with custom className', () => {
      const attachment = createAttachment();
      
      render(
        <FileAttachmentItem 
          attachment={attachment} 
          onRemove={mockOnRemove}
          className="my-custom"
        />
      );
      
      const listItem = screen.getByRole('listitem');
      expect(listItem).toHaveClass('my-custom');
      expect(listItem).toHaveClass('relative', 'group', 'border', 'rounded-lg');
    });
  });

  describe('Edge Cases', () => {
    it('should handle very long filenames', () => {
      const longFilename = 'a'.repeat(200) + '.txt';
      const attachment = createAttachment({
        file: createMockFile(longFilename, 'text/plain'),
      });
      
      render(<FileAttachmentItem attachment={attachment} onRemove={mockOnRemove} />);
      
      const listItem = screen.getByRole('listitem');
      expect(listItem).toHaveAttribute('aria-label', expect.stringContaining(longFilename));
    });

    it('should handle filenames with special characters', () => {
      const specialFilename = 'file & name (2023) [final].pdf';
      const attachment = createAttachment({
        file: createMockFile(specialFilename, 'application/pdf'),
      });
      
      render(<FileAttachmentItem attachment={attachment} onRemove={mockOnRemove} />);
      
      const removeButton = screen.getByRole('button', { name: `Remove ${specialFilename}` });
      expect(removeButton).toBeInTheDocument();
    });

    it('should handle null previewUrl for images', () => {
      const attachment = createAttachment({
        file: createMockFile('image.png', 'image/png'),
        previewUrl: null,
      });
      
      render(<FileAttachmentItem attachment={attachment} onRemove={mockOnRemove} />);
      
      expect(screen.queryByRole('img')).not.toBeInTheDocument();
      expect(screen.getByTestId('file-icon')).toBeInTheDocument();
    });

    it('should handle null error gracefully', () => {
      const attachment = createAttachment({
        status: 'error',
        error: null,
      });
      
      const { container } = render(
        <FileAttachmentItem attachment={attachment} onRemove={mockOnRemove} />
      );
      
      // Should still show error status but no error tooltip
      const listItem = screen.getByRole('listitem');
      expect(listItem).toHaveClass('border-destructive');
      
      const errorTooltip = container.querySelector('.bg-destructive.text-destructive-foreground');
      expect(errorTooltip).not.toBeInTheDocument();
    });

    it('should handle 0% progress', () => {
      const attachment = createAttachment({
        status: 'uploading',
        progress: 0,
      });
      
      render(<FileAttachmentItem attachment={attachment} onRemove={mockOnRemove} />);
      
      const progressIndicator = screen.getByTestId('upload-progress-indicator');
      expect(progressIndicator).toHaveAttribute('data-progress', '0');
    });

    it('should handle 100% progress', () => {
      const attachment = createAttachment({
        status: 'uploading',
        progress: 100,
      });
      
      render(<FileAttachmentItem attachment={attachment} onRemove={mockOnRemove} />);
      
      const progressIndicator = screen.getByTestId('upload-progress-indicator');
      expect(progressIndicator).toHaveAttribute('data-progress', '100');
    });

    it('should handle empty filename', () => {
      const attachment = createAttachment({
        file: createMockFile('', 'text/plain'),
      });
      
      render(<FileAttachmentItem attachment={attachment} onRemove={mockOnRemove} />);
      
      // When filename is empty, accessible name is normalized to 'Remove' (trailing space is trimmed)
      const removeButton = screen.getByRole('button', { name: 'Remove' });
      expect(removeButton).toBeInTheDocument();
    });

    it('should handle Unicode filenames', () => {
      const unicodeFilename = '文档.pdf';
      const attachment = createAttachment({
        file: createMockFile(unicodeFilename, 'application/pdf'),
      });
      
      render(<FileAttachmentItem attachment={attachment} onRemove={mockOnRemove} />);
      
      const listItem = screen.getByRole('listitem');
      expect(listItem).toHaveAttribute('aria-label', expect.stringContaining(unicodeFilename));
    });

    it('should handle very long error messages', () => {
      const longError = 'Error: ' + 'a'.repeat(500);
      const attachment = createAttachment({
        status: 'error',
        error: longError,
      });
      
      const { container } = render(
        <FileAttachmentItem attachment={attachment} onRemove={mockOnRemove} />
      );
      
      const errorTooltip = container.querySelector('.bg-destructive.text-destructive-foreground');
      expect(errorTooltip).toHaveTextContent(longError);
    });
  });

  describe('State Transitions', () => {
    it('should transition from pending to uploading', () => {
      const attachment = createAttachment({ status: 'pending' });
      
      const { rerender } = render(
        <FileAttachmentItem attachment={attachment} onRemove={mockOnRemove} />
      );
      
      expect(screen.queryByTestId('upload-progress-indicator')).not.toBeInTheDocument();
      
      const uploadingAttachment = { ...attachment, status: 'uploading' as const, progress: 25 };
      rerender(<FileAttachmentItem attachment={uploadingAttachment} onRemove={mockOnRemove} />);
      
      expect(screen.getByTestId('upload-progress-indicator')).toBeInTheDocument();
    });

    it('should transition from uploading to complete', () => {
      const attachment = createAttachment({
        status: 'uploading',
        progress: 90,
      });
      
      const { rerender } = render(
        <FileAttachmentItem attachment={attachment} onRemove={mockOnRemove} />
      );
      
      expect(screen.getByTestId('upload-progress-indicator')).toBeInTheDocument();
      expect(screen.queryByTestId('check-icon')).not.toBeInTheDocument();
      
      const completeAttachment = { ...attachment, status: 'complete' as const, progress: 100 };
      rerender(<FileAttachmentItem attachment={completeAttachment} onRemove={mockOnRemove} />);
      
      expect(screen.queryByTestId('upload-progress-indicator')).not.toBeInTheDocument();
      expect(screen.getByTestId('check-icon')).toBeInTheDocument();
    });

    it('should transition from uploading to error', () => {
      const attachment = createAttachment({
        status: 'uploading',
        progress: 50,
      });
      
      const { rerender } = render(
        <FileAttachmentItem attachment={attachment} onRemove={mockOnRemove} />
      );
      
      expect(screen.getByTestId('upload-progress-indicator')).toBeInTheDocument();
      
      const errorAttachment = { 
        ...attachment, 
        status: 'error' as const,
        error: 'Upload interrupted'
      };
      rerender(<FileAttachmentItem attachment={errorAttachment} onRemove={mockOnRemove} />);
      
      expect(screen.queryByTestId('upload-progress-indicator')).not.toBeInTheDocument();
      expect(screen.getByTestId('alert-circle-icon')).toBeInTheDocument();
    });

    it('should update border color during state transitions', () => {
      const attachment = createAttachment({ status: 'pending' });
      
      const { rerender } = render(
        <FileAttachmentItem attachment={attachment} onRemove={mockOnRemove} />
      );
      
      let listItem = screen.getByRole('listitem');
      expect(listItem).not.toHaveClass('border-primary', 'border-destructive');
      
      // Transition to complete
      const completeAttachment = { ...attachment, status: 'complete' as const };
      rerender(<FileAttachmentItem attachment={completeAttachment} onRemove={mockOnRemove} />);
      
      listItem = screen.getByRole('listitem');
      expect(listItem).toHaveClass('border-primary');
      
      // Transition to error
      const errorAttachment = { ...attachment, status: 'error' as const };
      rerender(<FileAttachmentItem attachment={errorAttachment} onRemove={mockOnRemove} />);
      
      listItem = screen.getByRole('listitem');
      expect(listItem).toHaveClass('border-destructive');
      expect(listItem).not.toHaveClass('border-primary');
    });
  });

  describe('Image Type Detection', () => {
    it('should detect image/jpeg as image', () => {
      const attachment = createAttachment({
        file: createMockFile('photo.jpg', 'image/jpeg'),
        previewUrl: 'blob:preview',
      });
      
      render(<FileAttachmentItem attachment={attachment} onRemove={mockOnRemove} />);
      
      expect(screen.getByRole('img')).toBeInTheDocument();
    });

    it('should detect image/png as image', () => {
      const attachment = createAttachment({
        file: createMockFile('screenshot.png', 'image/png'),
        previewUrl: 'blob:preview',
      });
      
      render(<FileAttachmentItem attachment={attachment} onRemove={mockOnRemove} />);
      
      expect(screen.getByRole('img')).toBeInTheDocument();
    });

    it('should detect image/gif as image', () => {
      const attachment = createAttachment({
        file: createMockFile('animation.gif', 'image/gif'),
        previewUrl: 'blob:preview',
      });
      
      render(<FileAttachmentItem attachment={attachment} onRemove={mockOnRemove} />);
      
      expect(screen.getByRole('img')).toBeInTheDocument();
    });

    it('should detect image/webp as image', () => {
      const attachment = createAttachment({
        file: createMockFile('modern.webp', 'image/webp'),
        previewUrl: 'blob:preview',
      });
      
      render(<FileAttachmentItem attachment={attachment} onRemove={mockOnRemove} />);
      
      expect(screen.getByRole('img')).toBeInTheDocument();
    });

    it('should not treat video as image', () => {
      const attachment = createAttachment({
        file: createMockFile('video.mp4', 'video/mp4'),
        previewUrl: 'blob:preview',
      });
      
      render(<FileAttachmentItem attachment={attachment} onRemove={mockOnRemove} />);
      
      expect(screen.queryByRole('img')).not.toBeInTheDocument();
      expect(screen.getByTestId('file-icon')).toBeInTheDocument();
    });

    it('should not treat audio as image', () => {
      const attachment = createAttachment({
        file: createMockFile('audio.mp3', 'audio/mpeg'),
      });
      
      render(<FileAttachmentItem attachment={attachment} onRemove={mockOnRemove} />);
      
      expect(screen.queryByRole('img')).not.toBeInTheDocument();
      expect(screen.getByTestId('file-icon')).toBeInTheDocument();
    });
  });

  describe('Multiple Instances', () => {
    it('should render multiple items independently', () => {
      const attachment1 = createAttachment({
        file: createMockFile('file1.txt', 'text/plain'),
        status: 'complete',
      });
      
      const attachment2 = createAttachment({
        file: createMockFile('file2.jpg', 'image/jpeg'),
        status: 'uploading',
        progress: 50,
        previewUrl: 'blob:preview',
      });
      
      const { container } = render(
        <>
          <FileAttachmentItem attachment={attachment1} onRemove={vi.fn()} />
          <FileAttachmentItem attachment={attachment2} onRemove={vi.fn()} />
        </>
      );
      
      const listItems = screen.getAllByRole('listitem');
      expect(listItems).toHaveLength(2);
      
      // First item should have checkmark (complete)
      expect(screen.getByTestId('check-icon')).toBeInTheDocument();
      
      // Second item should have progress indicator and image preview
      expect(screen.getByTestId('upload-progress-indicator')).toBeInTheDocument();
      expect(screen.getByRole('img')).toBeInTheDocument();
    });

    it('should call correct onRemove for each item', async () => {
      const user = userEvent.setup();
      const onRemove1 = vi.fn();
      const onRemove2 = vi.fn();
      
      const attachment1 = createAttachment({ file: createMockFile('file1.txt', 'text/plain') });
      const attachment2 = createAttachment({ file: createMockFile('file2.txt', 'text/plain') });
      
      render(
        <>
          <FileAttachmentItem attachment={attachment1} onRemove={onRemove1} />
          <FileAttachmentItem attachment={attachment2} onRemove={onRemove2} />
        </>
      );
      
      const removeButton1 = screen.getByRole('button', { name: 'Remove file1.txt' });
      const removeButton2 = screen.getByRole('button', { name: 'Remove file2.txt' });
      
      await user.click(removeButton1);
      expect(onRemove1).toHaveBeenCalledTimes(1);
      expect(onRemove2).not.toHaveBeenCalled();
      
      await user.click(removeButton2);
      expect(onRemove2).toHaveBeenCalledTimes(1);
      expect(onRemove1).toHaveBeenCalledTimes(1);
    });
  });
});
