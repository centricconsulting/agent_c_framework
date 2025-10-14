/**
 * FileAttachmentList Component Tests
 * Testing container component for file attachments display
 */

import React from 'react';
import { describe, it, expect, vi } from 'vitest';
import { render, screen, fireEvent } from '@testing-library/react';
import { FileAttachmentList } from '../FileAttachmentList';
import type { FileAttachment } from '@agentc/realtime-react';

// Mock FileAttachmentItem component
vi.mock('../FileAttachmentItem', () => ({
  FileAttachmentItem: ({ attachment, onRemove }: any) => (
    <div 
      data-testid={`attachment-item-${attachment.file.name}`}
      data-filename={attachment.file.name}
      data-status={attachment.status}
    >
      <button 
        data-testid={`remove-${attachment.file.name}`}
        onClick={onRemove}
      >
        Remove {attachment.file.name}
      </button>
    </div>
  ),
}));

describe('FileAttachmentList Component', () => {
  const createMockFile = (name: string): File => {
    return new File(['content'], name, { type: 'image/png' });
  };

  const createMockAttachment = (name: string, status: 'pending' | 'uploading' | 'complete' | 'error' = 'pending'): FileAttachment => ({
    file: createMockFile(name),
    id: status === 'complete' ? `file-${name}` : null,
    status,
    progress: status === 'uploading' ? 50 : status === 'complete' ? 100 : 0,
    error: status === 'error' ? 'Upload failed' : null,
    previewUrl: null,
  });

  const mockOnRemove = vi.fn();

  beforeEach(() => {
    vi.clearAllMocks();
  });

  describe('Conditional Rendering', () => {
    it('should render when attachments array has items', () => {
      const attachments = [createMockAttachment('file1.png')];
      
      render(
        <FileAttachmentList attachments={attachments} onRemove={mockOnRemove} />
      );
      
      expect(screen.getByRole('list')).toBeInTheDocument();
    });

    it('should not render when attachments array is empty', () => {
      render(
        <FileAttachmentList attachments={[]} onRemove={mockOnRemove} />
      );
      
      expect(screen.queryByRole('list')).not.toBeInTheDocument();
    });

    it('should return null for empty array', () => {
      const { container } = render(
        <FileAttachmentList attachments={[]} onRemove={mockOnRemove} />
      );
      
      expect(container.firstChild).toBeNull();
    });
  });

  describe('List Rendering', () => {
    it('should render single attachment', () => {
      const attachments = [createMockAttachment('single.png')];
      
      render(
        <FileAttachmentList attachments={attachments} onRemove={mockOnRemove} />
      );
      
      expect(screen.getByTestId('attachment-item-single.png')).toBeInTheDocument();
    });

    it('should render multiple attachments', () => {
      const attachments = [
        createMockAttachment('file1.png'),
        createMockAttachment('file2.jpg'),
        createMockAttachment('file3.gif'),
      ];
      
      render(
        <FileAttachmentList attachments={attachments} onRemove={mockOnRemove} />
      );
      
      expect(screen.getByTestId('attachment-item-file1.png')).toBeInTheDocument();
      expect(screen.getByTestId('attachment-item-file2.jpg')).toBeInTheDocument();
      expect(screen.getByTestId('attachment-item-file3.gif')).toBeInTheDocument();
    });

    it('should render all attachments in order', () => {
      const attachments = [
        createMockAttachment('first.png'),
        createMockAttachment('second.png'),
        createMockAttachment('third.png'),
      ];
      
      const { container } = render(
        <FileAttachmentList attachments={attachments} onRemove={mockOnRemove} />
      );
      
      const items = container.querySelectorAll('[data-testid^="attachment-item"]');
      expect(items).toHaveLength(3);
      expect(items[0]).toHaveAttribute('data-filename', 'first.png');
      expect(items[1]).toHaveAttribute('data-filename', 'second.png');
      expect(items[2]).toHaveAttribute('data-filename', 'third.png');
    });
  });

  describe('Orientation Variants', () => {
    it('should use horizontal orientation by default', () => {
      const attachments = [createMockAttachment('file.png')];
      
      const { container } = render(
        <FileAttachmentList attachments={attachments} onRemove={mockOnRemove} />
      );
      
      const list = container.querySelector('[role="list"]');
      expect(list).toHaveClass('flex-row');
    });

    it('should apply horizontal orientation when specified', () => {
      const attachments = [createMockAttachment('file.png')];
      
      const { container } = render(
        <FileAttachmentList 
          attachments={attachments} 
          onRemove={mockOnRemove}
          orientation="horizontal"
        />
      );
      
      const list = container.querySelector('[role="list"]');
      expect(list).toHaveClass('flex-row');
    });

    it('should apply vertical orientation when specified', () => {
      const attachments = [createMockAttachment('file.png')];
      
      const { container } = render(
        <FileAttachmentList 
          attachments={attachments} 
          onRemove={mockOnRemove}
          orientation="vertical"
        />
      );
      
      const list = container.querySelector('[role="list"]');
      expect(list).toHaveClass('flex-col');
    });

    it('should wrap items in horizontal orientation', () => {
      const attachments = [createMockAttachment('file.png')];
      
      const { container } = render(
        <FileAttachmentList 
          attachments={attachments} 
          onRemove={mockOnRemove}
          orientation="horizontal"
        />
      );
      
      const list = container.querySelector('[role="list"]');
      expect(list).toHaveClass('flex-wrap');
    });

    it('should not wrap items in vertical orientation', () => {
      const attachments = [createMockAttachment('file.png')];
      
      const { container } = render(
        <FileAttachmentList 
          attachments={attachments} 
          onRemove={mockOnRemove}
          orientation="vertical"
        />
      );
      
      const list = container.querySelector('[role="list"]');
      expect(list).not.toHaveClass('flex-wrap');
    });
  });

  describe('Remove Functionality', () => {
    it('should call onRemove with correct index when item is removed', () => {
      const attachments = [
        createMockAttachment('file1.png'),
        createMockAttachment('file2.png'),
      ];
      
      render(
        <FileAttachmentList attachments={attachments} onRemove={mockOnRemove} />
      );
      
      // Remove second item
      fireEvent.click(screen.getByTestId('remove-file2.png'));
      
      expect(mockOnRemove).toHaveBeenCalledWith(1);
      expect(mockOnRemove).toHaveBeenCalledTimes(1);
    });

    it('should call onRemove for first item with index 0', () => {
      const attachments = [
        createMockAttachment('file1.png'),
        createMockAttachment('file2.png'),
      ];
      
      render(
        <FileAttachmentList attachments={attachments} onRemove={mockOnRemove} />
      );
      
      fireEvent.click(screen.getByTestId('remove-file1.png'));
      
      expect(mockOnRemove).toHaveBeenCalledWith(0);
    });

    it('should handle remove for multiple items independently', () => {
      const attachments = [
        createMockAttachment('file1.png'),
        createMockAttachment('file2.png'),
        createMockAttachment('file3.png'),
      ];
      
      render(
        <FileAttachmentList attachments={attachments} onRemove={mockOnRemove} />
      );
      
      fireEvent.click(screen.getByTestId('remove-file2.png'));
      expect(mockOnRemove).toHaveBeenCalledWith(1);
      
      fireEvent.click(screen.getByTestId('remove-file3.png'));
      expect(mockOnRemove).toHaveBeenCalledWith(2);
      
      fireEvent.click(screen.getByTestId('remove-file1.png'));
      expect(mockOnRemove).toHaveBeenCalledWith(0);
      
      expect(mockOnRemove).toHaveBeenCalledTimes(3);
    });
  });

  describe('Accessibility', () => {
    it('should have list role', () => {
      const attachments = [createMockAttachment('file.png')];
      
      render(
        <FileAttachmentList attachments={attachments} onRemove={mockOnRemove} />
      );
      
      expect(screen.getByRole('list')).toBeInTheDocument();
    });

    it('should have descriptive aria-label for single attachment', () => {
      const attachments = [createMockAttachment('file.png')];
      
      render(
        <FileAttachmentList attachments={attachments} onRemove={mockOnRemove} />
      );
      
      const list = screen.getByRole('list');
      expect(list).toHaveAttribute('aria-label', '1 file attachment');
    });

    it('should have descriptive aria-label for multiple attachments', () => {
      const attachments = [
        createMockAttachment('file1.png'),
        createMockAttachment('file2.png'),
        createMockAttachment('file3.png'),
      ];
      
      render(
        <FileAttachmentList attachments={attachments} onRemove={mockOnRemove} />
      );
      
      const list = screen.getByRole('list');
      expect(list).toHaveAttribute('aria-label', '3 file attachments');
    });

    it('should use plural "attachments" for count > 1', () => {
      const attachments = [
        createMockAttachment('file1.png'),
        createMockAttachment('file2.png'),
      ];
      
      render(
        <FileAttachmentList attachments={attachments} onRemove={mockOnRemove} />
      );
      
      const list = screen.getByRole('list');
      expect(list.getAttribute('aria-label')).toContain('attachments');
    });

    it('should use singular "attachment" for count = 1', () => {
      const attachments = [createMockAttachment('file.png')];
      
      render(
        <FileAttachmentList attachments={attachments} onRemove={mockOnRemove} />
      );
      
      const list = screen.getByRole('list');
      expect(list.getAttribute('aria-label')).toContain('attachment');
      expect(list.getAttribute('aria-label')).not.toContain('attachments');
    });
  });

  describe('Layout Styling', () => {
    it('should have flex layout', () => {
      const attachments = [createMockAttachment('file.png')];
      
      const { container } = render(
        <FileAttachmentList attachments={attachments} onRemove={mockOnRemove} />
      );
      
      const list = container.querySelector('[role="list"]');
      expect(list).toHaveClass('flex');
    });

    it('should have gap between items', () => {
      const attachments = [createMockAttachment('file.png')];
      
      const { container } = render(
        <FileAttachmentList attachments={attachments} onRemove={mockOnRemove} />
      );
      
      const list = container.querySelector('[role="list"]');
      expect(list).toHaveClass('gap-2');
    });

    it('should apply custom className', () => {
      const attachments = [createMockAttachment('file.png')];
      
      const { container } = render(
        <FileAttachmentList 
          attachments={attachments} 
          onRemove={mockOnRemove}
          className="custom-list-class"
        />
      );
      
      const list = container.querySelector('[role="list"]');
      expect(list).toHaveClass('custom-list-class');
    });

    it('should preserve default classes with custom className', () => {
      const attachments = [createMockAttachment('file.png')];
      
      const { container } = render(
        <FileAttachmentList 
          attachments={attachments} 
          onRemove={mockOnRemove}
          className="my-custom"
        />
      );
      
      const list = container.querySelector('[role="list"]');
      expect(list).toHaveClass('my-custom', 'flex', 'gap-2');
    });
  });

  describe('Attachment Status Display', () => {
    it('should render attachments with different statuses', () => {
      const attachments = [
        createMockAttachment('pending.png', 'pending'),
        createMockAttachment('uploading.png', 'uploading'),
        createMockAttachment('complete.png', 'complete'),
        createMockAttachment('error.png', 'error'),
      ];
      
      render(
        <FileAttachmentList attachments={attachments} onRemove={mockOnRemove} />
      );
      
      expect(screen.getByTestId('attachment-item-pending.png')).toHaveAttribute('data-status', 'pending');
      expect(screen.getByTestId('attachment-item-uploading.png')).toHaveAttribute('data-status', 'uploading');
      expect(screen.getByTestId('attachment-item-complete.png')).toHaveAttribute('data-status', 'complete');
      expect(screen.getByTestId('attachment-item-error.png')).toHaveAttribute('data-status', 'error');
    });
  });

  describe('Edge Cases', () => {
    it('should handle very long list of attachments', () => {
      const attachments = Array.from({ length: 50 }, (_, i) => 
        createMockAttachment(`file${i}.png`)
      );
      
      render(
        <FileAttachmentList attachments={attachments} onRemove={mockOnRemove} />
      );
      
      const list = screen.getByRole('list');
      expect(list.querySelectorAll('[data-testid^="attachment-item"]')).toHaveLength(50);
    });

    it('should handle attachments being added', () => {
      const { rerender } = render(
        <FileAttachmentList attachments={[]} onRemove={mockOnRemove} />
      );
      
      expect(screen.queryByRole('list')).not.toBeInTheDocument();
      
      const attachments = [createMockAttachment('new.png')];
      rerender(
        <FileAttachmentList attachments={attachments} onRemove={mockOnRemove} />
      );
      
      expect(screen.getByRole('list')).toBeInTheDocument();
      expect(screen.getByTestId('attachment-item-new.png')).toBeInTheDocument();
    });

    it('should handle all attachments being removed', () => {
      const attachments = [createMockAttachment('file.png')];
      
      const { rerender } = render(
        <FileAttachmentList attachments={attachments} onRemove={mockOnRemove} />
      );
      
      expect(screen.getByRole('list')).toBeInTheDocument();
      
      rerender(
        <FileAttachmentList attachments={[]} onRemove={mockOnRemove} />
      );
      
      expect(screen.queryByRole('list')).not.toBeInTheDocument();
    });

    it('should handle attachments with special characters in names', () => {
      const attachments = [
        createMockAttachment('file (1).png'),
        createMockAttachment('file [copy].jpg'),
        createMockAttachment('file_name-v2.gif'),
      ];
      
      render(
        <FileAttachmentList attachments={attachments} onRemove={mockOnRemove} />
      );
      
      expect(screen.getByTestId('attachment-item-file (1).png')).toBeInTheDocument();
      expect(screen.getByTestId('attachment-item-file [copy].jpg')).toBeInTheDocument();
      expect(screen.getByTestId('attachment-item-file_name-v2.gif')).toBeInTheDocument();
    });

    it('should handle orientation changes', () => {
      const attachments = [createMockAttachment('file.png')];
      
      const { container, rerender } = render(
        <FileAttachmentList 
          attachments={attachments} 
          onRemove={mockOnRemove}
          orientation="horizontal"
        />
      );
      
      let list = container.querySelector('[role="list"]');
      expect(list).toHaveClass('flex-row');
      
      rerender(
        <FileAttachmentList 
          attachments={attachments} 
          onRemove={mockOnRemove}
          orientation="vertical"
        />
      );
      
      list = container.querySelector('[role="list"]');
      expect(list).toHaveClass('flex-col');
    });

    it('should handle undefined orientation (defaults to horizontal)', () => {
      const attachments = [createMockAttachment('file.png')];
      
      const { container } = render(
        <FileAttachmentList 
          attachments={attachments} 
          onRemove={mockOnRemove}
          orientation={undefined}
        />
      );
      
      const list = container.querySelector('[role="list"]');
      expect(list).toHaveClass('flex-row');
    });
  });

  describe('Component Integration', () => {
    it('should pass attachment data to FileAttachmentItem', () => {
      const attachment = createMockAttachment('test.png', 'uploading');
      
      render(
        <FileAttachmentList attachments={[attachment]} onRemove={mockOnRemove} />
      );
      
      const item = screen.getByTestId('attachment-item-test.png');
      expect(item).toHaveAttribute('data-filename', 'test.png');
      expect(item).toHaveAttribute('data-status', 'uploading');
    });

    it('should create unique onRemove handlers for each item', () => {
      const attachments = [
        createMockAttachment('file1.png'),
        createMockAttachment('file2.png'),
      ];
      
      render(
        <FileAttachmentList attachments={attachments} onRemove={mockOnRemove} />
      );
      
      fireEvent.click(screen.getByTestId('remove-file1.png'));
      fireEvent.click(screen.getByTestId('remove-file2.png'));
      
      expect(mockOnRemove).toHaveBeenNthCalledWith(1, 0);
      expect(mockOnRemove).toHaveBeenNthCalledWith(2, 1);
    });
  });
});
