/**
 * ImageLightbox Component Tests
 * Testing modal behavior, keyboard navigation, and image display
 */

import React from 'react';
import { describe, it, expect, beforeEach, afterEach, vi } from 'vitest';
import { render, screen, fireEvent } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { ImageLightbox } from '../ImageLightbox';

// Mock lucide-react icons
vi.mock('lucide-react', () => ({
  X: ({ className }: any) => <div data-testid="x-icon" className={className} />,
}));

// Mock Dialog components
vi.mock('../../ui/dialog', () => ({
  Dialog: ({ open, onOpenChange, children }: any) => {
    if (!open) return null;
    
    return (
      <div data-testid="dialog-wrapper" data-open={open}>
        {typeof onOpenChange === 'function' && (
          <button 
            data-testid="dialog-backdrop" 
            onClick={() => onOpenChange(false)}
          >
            Backdrop
          </button>
        )}
        {children}
      </div>
    );
  },
  DialogContent: ({ className, 'aria-label': ariaLabel, children }: any) => (
    <div 
      data-testid="dialog-content" 
      className={className}
      aria-label={ariaLabel}
      role="dialog"
    >
      {children}
    </div>
  ),
  DialogTitle: ({ className, children }: any) => (
    <h2 className={className} data-testid="dialog-title">
      {children}
    </h2>
  ),
  DialogDescription: ({ className, children }: any) => (
    <p className={className} data-testid="dialog-description">
      {children}
    </p>
  ),
}));

describe('ImageLightbox Component', () => {
  const mockOnClose = vi.fn();
  const testSrc = 'https://example.com/image.jpg';
  const testAlt = 'Test image';

  beforeEach(() => {
    // Reset mocks before each test
    vi.clearAllMocks();
    // Reset body overflow style
    document.body.style.overflow = '';
  });

  afterEach(() => {
    // Clean up after each test
    document.body.style.overflow = '';
  });

  describe('Conditional Rendering', () => {
    it('should render when isOpen is true', () => {
      render(
        <ImageLightbox 
          src={testSrc} 
          isOpen={true} 
          onClose={mockOnClose} 
        />
      );
      
      expect(screen.getByRole('dialog')).toBeInTheDocument();
    });

    it('should not render when isOpen is false', () => {
      render(
        <ImageLightbox 
          src={testSrc} 
          isOpen={false} 
          onClose={mockOnClose} 
        />
      );
      
      expect(screen.queryByRole('dialog')).not.toBeInTheDocument();
    });

    it('should return null when not open', () => {
      const { container } = render(
        <ImageLightbox 
          src={testSrc} 
          isOpen={false} 
          onClose={mockOnClose} 
        />
      );
      
      expect(container.firstChild).toBeNull();
    });

    it('should toggle visibility when isOpen changes', () => {
      const { rerender } = render(
        <ImageLightbox 
          src={testSrc} 
          isOpen={false} 
          onClose={mockOnClose} 
        />
      );
      
      expect(screen.queryByRole('dialog')).not.toBeInTheDocument();
      
      rerender(
        <ImageLightbox 
          src={testSrc} 
          isOpen={true} 
          onClose={mockOnClose} 
        />
      );
      
      expect(screen.getByRole('dialog')).toBeInTheDocument();
    });
  });

  describe('Image Display', () => {
    it('should display the image', () => {
      render(
        <ImageLightbox 
          src={testSrc} 
          isOpen={true} 
          onClose={mockOnClose} 
        />
      );
      
      const img = screen.getByRole('img');
      expect(img).toBeInTheDocument();
      expect(img).toHaveAttribute('src', testSrc);
    });

    it('should use provided alt text', () => {
      render(
        <ImageLightbox 
          src={testSrc} 
          alt={testAlt}
          isOpen={true} 
          onClose={mockOnClose} 
        />
      );
      
      const img = screen.getByRole('img');
      expect(img).toHaveAttribute('alt', testAlt);
    });

    it('should use default alt text when not provided', () => {
      render(
        <ImageLightbox 
          src={testSrc} 
          isOpen={true} 
          onClose={mockOnClose} 
        />
      );
      
      const img = screen.getByRole('img');
      expect(img).toHaveAttribute('alt', 'Image');
    });

    it('should have correct image styling', () => {
      render(
        <ImageLightbox 
          src={testSrc} 
          isOpen={true} 
          onClose={mockOnClose} 
        />
      );
      
      const img = screen.getByRole('img');
      expect(img).toHaveClass('max-w-full', 'max-h-full', 'object-contain', 'p-8');
    });

    it('should update image when src changes', () => {
      const { rerender } = render(
        <ImageLightbox 
          src={testSrc} 
          isOpen={true} 
          onClose={mockOnClose} 
        />
      );
      
      const newSrc = 'https://example.com/new-image.jpg';
      rerender(
        <ImageLightbox 
          src={newSrc} 
          isOpen={true} 
          onClose={mockOnClose} 
        />
      );
      
      const img = screen.getByRole('img');
      expect(img).toHaveAttribute('src', newSrc);
    });
  });

  describe('Close Button', () => {
    it('should render close button', () => {
      render(
        <ImageLightbox 
          src={testSrc} 
          isOpen={true} 
          onClose={mockOnClose} 
        />
      );
      
      const closeButton = screen.getByRole('button', { name: 'Close image viewer' });
      expect(closeButton).toBeInTheDocument();
    });

    it('should call onClose when close button is clicked', () => {
      render(
        <ImageLightbox 
          src={testSrc} 
          isOpen={true} 
          onClose={mockOnClose} 
        />
      );
      
      const closeButton = screen.getByRole('button', { name: 'Close image viewer' });
      fireEvent.click(closeButton);
      
      expect(mockOnClose).toHaveBeenCalledTimes(1);
    });

    it('should have proper close button styling', () => {
      render(
        <ImageLightbox 
          src={testSrc} 
          isOpen={true} 
          onClose={mockOnClose} 
        />
      );
      
      const closeButton = screen.getByRole('button', { name: 'Close image viewer' });
      expect(closeButton).toHaveClass('absolute', 'top-4', 'right-4', 'z-50');
      expect(closeButton).toHaveClass('p-2', 'rounded-full', 'bg-background', 'border');
    });

    it('should display X icon in close button', () => {
      render(
        <ImageLightbox 
          src={testSrc} 
          isOpen={true} 
          onClose={mockOnClose} 
        />
      );
      
      const xIcon = screen.getByTestId('x-icon');
      expect(xIcon).toBeInTheDocument();
      expect(xIcon).toHaveClass('h-6', 'w-6');
    });
  });

  describe('Keyboard Navigation', () => {
    it('should call onClose when Escape key is pressed', () => {
      render(
        <ImageLightbox 
          src={testSrc} 
          isOpen={true} 
          onClose={mockOnClose} 
        />
      );
      
      fireEvent.keyDown(document, { key: 'Escape' });
      
      expect(mockOnClose).toHaveBeenCalledTimes(1);
    });

    it('should not handle Escape when not open', () => {
      render(
        <ImageLightbox 
          src={testSrc} 
          isOpen={false} 
          onClose={mockOnClose} 
        />
      );
      
      fireEvent.keyDown(document, { key: 'Escape' });
      
      expect(mockOnClose).not.toHaveBeenCalled();
    });

    it('should handle Escape key case-sensitively', () => {
      render(
        <ImageLightbox 
          src={testSrc} 
          isOpen={true} 
          onClose={mockOnClose} 
        />
      );
      
      fireEvent.keyDown(document, { key: 'Escape' });
      expect(mockOnClose).toHaveBeenCalledTimes(1);
      
      mockOnClose.mockClear();
      
      // Different key should not trigger
      fireEvent.keyDown(document, { key: 'Enter' });
      expect(mockOnClose).not.toHaveBeenCalled();
    });

    it('should remove event listener when closed', () => {
      const { rerender } = render(
        <ImageLightbox 
          src={testSrc} 
          isOpen={true} 
          onClose={mockOnClose} 
        />
      );
      
      // Close the lightbox
      rerender(
        <ImageLightbox 
          src={testSrc} 
          isOpen={false} 
          onClose={mockOnClose} 
        />
      );
      
      // Try to trigger Escape
      fireEvent.keyDown(document, { key: 'Escape' });
      
      // Should not call onClose since lightbox is closed
      expect(mockOnClose).not.toHaveBeenCalled();
    });

    it('should remove event listener on unmount', () => {
      const { unmount } = render(
        <ImageLightbox 
          src={testSrc} 
          isOpen={true} 
          onClose={mockOnClose} 
        />
      );
      
      unmount();
      
      fireEvent.keyDown(document, { key: 'Escape' });
      
      // Should not call onClose after unmount
      expect(mockOnClose).not.toHaveBeenCalled();
    });
  });

  describe('Body Scroll Lock', () => {
    it('should lock body scroll when open', () => {
      render(
        <ImageLightbox 
          src={testSrc} 
          isOpen={true} 
          onClose={mockOnClose} 
        />
      );
      
      expect(document.body.style.overflow).toBe('hidden');
    });

    it('should not lock scroll when not open', () => {
      render(
        <ImageLightbox 
          src={testSrc} 
          isOpen={false} 
          onClose={mockOnClose} 
        />
      );
      
      expect(document.body.style.overflow).toBe('');
    });

    it('should restore scroll when closed', () => {
      const { rerender } = render(
        <ImageLightbox 
          src={testSrc} 
          isOpen={true} 
          onClose={mockOnClose} 
        />
      );
      
      expect(document.body.style.overflow).toBe('hidden');
      
      rerender(
        <ImageLightbox 
          src={testSrc} 
          isOpen={false} 
          onClose={mockOnClose} 
        />
      );
      
      expect(document.body.style.overflow).toBe('');
    });

    it('should restore scroll on unmount', () => {
      const { unmount } = render(
        <ImageLightbox 
          src={testSrc} 
          isOpen={true} 
          onClose={mockOnClose} 
        />
      );
      
      expect(document.body.style.overflow).toBe('hidden');
      
      unmount();
      
      expect(document.body.style.overflow).toBe('');
    });
  });

  describe('Dialog Integration', () => {
    it('should pass isOpen to Dialog component', () => {
      render(
        <ImageLightbox 
          src={testSrc} 
          isOpen={true} 
          onClose={mockOnClose} 
        />
      );
      
      const dialogWrapper = screen.getByTestId('dialog-wrapper');
      expect(dialogWrapper).toHaveAttribute('data-open', 'true');
    });

    it('should have correct DialogContent styling', () => {
      render(
        <ImageLightbox 
          src={testSrc} 
          isOpen={true} 
          onClose={mockOnClose} 
        />
      );
      
      const dialogContent = screen.getByTestId('dialog-content');
      expect(dialogContent).toHaveClass('max-w-screen-xl', 'w-full', 'h-full', 'max-h-screen', 'p-0');
    });

    it('should have aria-label on dialog', () => {
      render(
        <ImageLightbox 
          src={testSrc} 
          isOpen={true} 
          onClose={mockOnClose} 
        />
      );
      
      const dialog = screen.getByRole('dialog');
      expect(dialog).toHaveAttribute('aria-label', 'Image viewer');
    });
  });

  describe('Layout Structure', () => {
    it('should have centered content container', () => {
      const { container } = render(
        <ImageLightbox 
          src={testSrc} 
          isOpen={true} 
          onClose={mockOnClose} 
        />
      );
      
      const contentContainer = container.querySelector('.relative.w-full.h-full.flex.items-center.justify-center');
      expect(contentContainer).toBeInTheDocument();
    });

    it('should have semi-transparent background', () => {
      const { container } = render(
        <ImageLightbox 
          src={testSrc} 
          isOpen={true} 
          onClose={mockOnClose} 
        />
      );
      
      const contentContainer = container.querySelector('.bg-background\\/95');
      expect(contentContainer).toBeInTheDocument();
    });

    it('should position close button absolutely', () => {
      render(
        <ImageLightbox 
          src={testSrc} 
          isOpen={true} 
          onClose={mockOnClose} 
        />
      );
      
      const closeButton = screen.getByRole('button', { name: 'Close image viewer' });
      expect(closeButton).toHaveClass('absolute', 'top-4', 'right-4');
    });
  });

  describe('Accessibility', () => {
    it('should have dialog role', () => {
      render(
        <ImageLightbox 
          src={testSrc} 
          isOpen={true} 
          onClose={mockOnClose} 
        />
      );
      
      expect(screen.getByRole('dialog')).toBeInTheDocument();
    });

    it('should have screen reader accessible title', () => {
      render(
        <ImageLightbox 
          src={testSrc} 
          isOpen={true} 
          onClose={mockOnClose} 
        />
      );
      
      const title = screen.getByTestId('dialog-title');
      expect(title).toBeInTheDocument();
      expect(title).toHaveTextContent('Image viewer');
      expect(title).toHaveClass('sr-only');
    });

    it('should have screen reader accessible description', () => {
      render(
        <ImageLightbox 
          src={testSrc} 
          isOpen={true} 
          onClose={mockOnClose} 
        />
      );
      
      const description = screen.getByTestId('dialog-description');
      expect(description).toBeInTheDocument();
      expect(description).toHaveTextContent('Full screen image view. Press Escape to close.');
      expect(description).toHaveClass('sr-only');
    });

    it('should have descriptive aria-label', () => {
      render(
        <ImageLightbox 
          src={testSrc} 
          isOpen={true} 
          onClose={mockOnClose} 
        />
      );
      
      const dialog = screen.getByRole('dialog');
      expect(dialog).toHaveAttribute('aria-label', 'Image viewer');
    });

    it('should have accessible close button', () => {
      render(
        <ImageLightbox 
          src={testSrc} 
          isOpen={true} 
          onClose={mockOnClose} 
        />
      );
      
      const closeButton = screen.getByRole('button', { name: 'Close image viewer' });
      expect(closeButton).toBeInTheDocument();
    });

    it('should have focus styles on close button', () => {
      render(
        <ImageLightbox 
          src={testSrc} 
          isOpen={true} 
          onClose={mockOnClose} 
        />
      );
      
      const closeButton = screen.getByRole('button', { name: 'Close image viewer' });
      expect(closeButton).toHaveClass('focus:outline-none', 'focus:ring-2', 'focus:ring-primary');
    });
  });

  describe('Edge Cases', () => {
    it('should handle empty src', () => {
      render(
        <ImageLightbox 
          src="" 
          isOpen={true} 
          onClose={mockOnClose} 
        />
      );
      
      const img = screen.getByRole('img');
      expect(img).toHaveAttribute('src', '');
    });

    it('should handle undefined alt', () => {
      render(
        <ImageLightbox 
          src={testSrc} 
          alt={undefined}
          isOpen={true} 
          onClose={mockOnClose} 
        />
      );
      
      const img = screen.getByRole('img');
      expect(img).toHaveAttribute('alt', 'Image');
    });

    it('should handle multiple rapid open/close cycles', () => {
      const { rerender } = render(
        <ImageLightbox 
          src={testSrc} 
          isOpen={false} 
          onClose={mockOnClose} 
        />
      );
      
      for (let i = 0; i < 5; i++) {
        rerender(
          <ImageLightbox 
            src={testSrc} 
            isOpen={true} 
            onClose={mockOnClose} 
          />
        );
        expect(screen.getByRole('dialog')).toBeInTheDocument();
        
        rerender(
          <ImageLightbox 
            src={testSrc} 
            isOpen={false} 
            onClose={mockOnClose} 
          />
        );
        expect(screen.queryByRole('dialog')).not.toBeInTheDocument();
      }
      
      // Body scroll should be restored
      expect(document.body.style.overflow).toBe('');
    });

    it('should handle onClose prop changes', () => {
      const firstOnClose = vi.fn();
      const secondOnClose = vi.fn();
      
      const { rerender } = render(
        <ImageLightbox 
          src={testSrc} 
          isOpen={true} 
          onClose={firstOnClose} 
        />
      );
      
      fireEvent.keyDown(document, { key: 'Escape' });
      expect(firstOnClose).toHaveBeenCalledTimes(1);
      expect(secondOnClose).not.toHaveBeenCalled();
      
      rerender(
        <ImageLightbox 
          src={testSrc} 
          isOpen={true} 
          onClose={secondOnClose} 
        />
      );
      
      fireEvent.keyDown(document, { key: 'Escape' });
      expect(firstOnClose).toHaveBeenCalledTimes(1); // Still 1
      expect(secondOnClose).toHaveBeenCalledTimes(1);
    });

    it('should handle very long image URLs', () => {
      const longSrc = 'https://example.com/' + 'a'.repeat(1000) + '.jpg';
      
      render(
        <ImageLightbox 
          src={longSrc} 
          isOpen={true} 
          onClose={mockOnClose} 
        />
      );
      
      const img = screen.getByRole('img');
      expect(img).toHaveAttribute('src', longSrc);
    });
  });
});
