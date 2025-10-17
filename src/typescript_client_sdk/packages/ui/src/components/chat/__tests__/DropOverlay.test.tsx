/**
 * DropOverlay Component Tests
 * Testing conditional rendering, ARIA attributes, and visual feedback
 */

import React from 'react';
import { describe, it, expect, vi } from 'vitest';
import { render, screen } from '@testing-library/react';
import { DropOverlay } from '../DropOverlay';

// Mock lucide-react icons
vi.mock('lucide-react', () => ({
  Upload: ({ className }: any) => (
    <div data-testid="upload-icon" className={className} />
  ),
}));

describe('DropOverlay Component', () => {
  describe('Conditional Rendering', () => {
    it('should render when isActive is true', () => {
      render(<DropOverlay isActive={true} />);
      
      const overlay = screen.getByRole('status');
      expect(overlay).toBeInTheDocument();
    });

    it('should not render when isActive is false', () => {
      render(<DropOverlay isActive={false} />);
      
      const overlay = screen.queryByRole('status');
      expect(overlay).not.toBeInTheDocument();
    });

    it('should not render anything initially if inactive', () => {
      const { container } = render(<DropOverlay isActive={false} />);
      
      expect(container.firstChild).toBeNull();
    });

    it('should toggle visibility when isActive changes', () => {
      const { rerender } = render(<DropOverlay isActive={false} />);
      
      expect(screen.queryByRole('status')).not.toBeInTheDocument();
      
      rerender(<DropOverlay isActive={true} />);
      expect(screen.getByRole('status')).toBeInTheDocument();
      
      rerender(<DropOverlay isActive={false} />);
      expect(screen.queryByRole('status')).not.toBeInTheDocument();
    });
  });

  describe('Visual Appearance', () => {
    it('should have semi-transparent backdrop', () => {
      const { container } = render(<DropOverlay isActive={true} />);
      
      const overlay = container.querySelector('.bg-primary\\/10');
      expect(overlay).toBeInTheDocument();
      expect(overlay).toHaveClass('backdrop-blur-sm');
    });

    it('should have dashed border', () => {
      const { container } = render(<DropOverlay isActive={true} />);
      
      const overlay = screen.getByRole('status');
      expect(overlay).toHaveClass('border-2', 'border-dashed', 'border-primary');
    });

    it('should have rounded corners', () => {
      const { container } = render(<DropOverlay isActive={true} />);
      
      const overlay = screen.getByRole('status');
      expect(overlay).toHaveClass('rounded-lg');
    });

    it('should cover full container (absolute inset-0)', () => {
      const { container } = render(<DropOverlay isActive={true} />);
      
      const overlay = screen.getByRole('status');
      expect(overlay).toHaveClass('absolute', 'inset-0');
    });

    it('should have high z-index', () => {
      const { container } = render(<DropOverlay isActive={true} />);
      
      const overlay = screen.getByRole('status');
      expect(overlay).toHaveClass('z-50');
    });

    it('should center content', () => {
      const { container } = render(<DropOverlay isActive={true} />);
      
      const overlay = screen.getByRole('status');
      expect(overlay).toHaveClass('flex', 'items-center', 'justify-center');
    });

    it('should not capture pointer events', () => {
      const { container } = render(<DropOverlay isActive={true} />);
      
      const overlay = screen.getByRole('status');
      expect(overlay).toHaveClass('pointer-events-none');
    });
  });

  describe('Upload Icon', () => {
    it('should display upload icon', () => {
      render(<DropOverlay isActive={true} />);
      
      const uploadIcon = screen.getByTestId('upload-icon');
      expect(uploadIcon).toBeInTheDocument();
    });

    it('should have correct icon size', () => {
      render(<DropOverlay isActive={true} />);
      
      const uploadIcon = screen.getByTestId('upload-icon');
      expect(uploadIcon).toHaveClass('h-12', 'w-12');
    });

    it('should be centered in overlay', () => {
      const { container } = render(<DropOverlay isActive={true} />);
      
      const contentWrapper = container.querySelector('.flex.flex-col.items-center');
      expect(contentWrapper).toBeInTheDocument();
      expect(contentWrapper).toHaveClass('gap-2', 'text-primary');
    });
  });

  describe('Allowed Types Message', () => {
    it('should display default allowed type (images)', () => {
      render(<DropOverlay isActive={true} />);
      
      expect(screen.getByText('Drop images here')).toBeInTheDocument();
    });

    it('should display single custom allowed type', () => {
      render(<DropOverlay isActive={true} allowedTypes={['documents']} />);
      
      expect(screen.getByText('Drop documents here')).toBeInTheDocument();
    });

    it('should display multiple allowed types', () => {
      render(
        <DropOverlay isActive={true} allowedTypes={['images', 'videos', 'documents']} />
      );
      
      expect(screen.getByText('Drop images, videos, documents here')).toBeInTheDocument();
    });

    it('should join types with commas', () => {
      render(<DropOverlay isActive={true} allowedTypes={['PDF', 'Word', 'Excel']} />);
      
      expect(screen.getByText('Drop PDF, Word, Excel here')).toBeInTheDocument();
    });

    it('should handle empty allowedTypes array', () => {
      render(<DropOverlay isActive={true} allowedTypes={[]} />);
      
      expect(screen.getByText('Drop here')).toBeInTheDocument();
    });

    it('should have proper text styling', () => {
      const { container } = render(<DropOverlay isActive={true} />);
      
      const message = screen.getByText('Drop images here');
      expect(message).toHaveClass('text-lg', 'font-medium');
    });
  });

  describe('Accessibility', () => {
    it('should have status role', () => {
      render(<DropOverlay isActive={true} />);
      
      const overlay = screen.getByRole('status');
      expect(overlay).toBeInTheDocument();
    });

    it('should have aria-live="polite"', () => {
      render(<DropOverlay isActive={true} />);
      
      const overlay = screen.getByRole('status');
      expect(overlay).toHaveAttribute('aria-live', 'polite');
    });

    it('should announce to screen readers when appearing', () => {
      render(<DropOverlay isActive={true} />);
      
      // With aria-live="polite", screen readers will announce the content
      const overlay = screen.getByRole('status');
      expect(overlay).toBeInTheDocument();
      expect(screen.getByText('Drop images here')).toBeInTheDocument();
    });

    it('should not interfere with drag-drop events', () => {
      render(<DropOverlay isActive={true} />);
      
      const overlay = screen.getByRole('status');
      // pointer-events-none ensures overlay doesn't block drop events
      expect(overlay).toHaveClass('pointer-events-none');
    });
  });

  describe('Custom Styling', () => {
    it('should apply custom className', () => {
      const { container } = render(
        <DropOverlay isActive={true} className="custom-overlay-class" />
      );
      
      const overlay = screen.getByRole('status');
      expect(overlay).toHaveClass('custom-overlay-class');
    });

    it('should preserve default classes with custom className', () => {
      const { container } = render(
        <DropOverlay isActive={true} className="my-custom-class" />
      );
      
      const overlay = screen.getByRole('status');
      expect(overlay).toHaveClass('my-custom-class');
      expect(overlay).toHaveClass('absolute', 'inset-0', 'z-50');
    });
  });

  describe('Content Structure', () => {
    it('should have correct content hierarchy', () => {
      const { container } = render(<DropOverlay isActive={true} />);
      
      // Main overlay container
      const overlay = screen.getByRole('status');
      expect(overlay).toBeInTheDocument();
      
      // Content wrapper
      const contentWrapper = container.querySelector('.flex.flex-col.items-center.gap-2');
      expect(contentWrapper).toBeInTheDocument();
      expect(overlay).toContainElement(contentWrapper!);
      
      // Icon
      const icon = screen.getByTestId('upload-icon');
      expect(contentWrapper).toContainElement(icon);
      
      // Message
      const message = screen.getByText('Drop images here');
      expect(contentWrapper).toContainElement(message);
    });

    it('should have vertical layout (flex-col)', () => {
      const { container } = render(<DropOverlay isActive={true} />);
      
      const contentWrapper = container.querySelector('.flex-col');
      expect(contentWrapper).toBeInTheDocument();
      expect(contentWrapper).toHaveClass('items-center', 'gap-2');
    });

    it('should use primary color theme', () => {
      const { container } = render(<DropOverlay isActive={true} />);
      
      const contentWrapper = container.querySelector('.text-primary');
      expect(contentWrapper).toBeInTheDocument();
      
      const overlay = screen.getByRole('status');
      expect(overlay).toHaveClass('border-primary');
    });
  });

  describe('Edge Cases', () => {
    it('should handle undefined allowedTypes', () => {
      render(<DropOverlay isActive={true} allowedTypes={undefined} />);
      
      // Should use default
      expect(screen.getByText('Drop images here')).toBeInTheDocument();
    });

    it('should handle rapid isActive toggling', () => {
      const { rerender } = render(<DropOverlay isActive={false} />);
      
      for (let i = 0; i < 10; i++) {
        rerender(<DropOverlay isActive={true} />);
        expect(screen.getByRole('status')).toBeInTheDocument();
        
        rerender(<DropOverlay isActive={false} />);
        expect(screen.queryByRole('status')).not.toBeInTheDocument();
      }
    });

    it('should handle very long allowedTypes array', () => {
      const manyTypes = Array.from({ length: 20 }, (_, i) => `type${i}`);
      
      render(<DropOverlay isActive={true} allowedTypes={manyTypes} />);
      
      const expectedText = `Drop ${manyTypes.join(', ')} here`;
      expect(screen.getByText(expectedText)).toBeInTheDocument();
    });

    it('should handle special characters in allowedTypes', () => {
      render(
        <DropOverlay isActive={true} allowedTypes={['image/png', 'image/jpeg', 'text/plain']} />
      );
      
      expect(screen.getByText('Drop image/png, image/jpeg, text/plain here')).toBeInTheDocument();
    });

    it('should handle single allowed type without comma', () => {
      render(<DropOverlay isActive={true} allowedTypes={['images']} />);
      
      const text = screen.getByText('Drop images here');
      expect(text.textContent).not.toContain(',');
    });
  });

  describe('Integration with Parent Container', () => {
    it('should overlay parent content with absolute positioning', () => {
      const { container } = render(
        <div style={{ position: 'relative', width: '400px', height: '300px' }}>
          <DropOverlay isActive={true} />
        </div>
      );
      
      const overlay = screen.getByRole('status');
      expect(overlay).toHaveClass('absolute', 'inset-0');
    });

    it('should be above other content (z-50)', () => {
      render(<DropOverlay isActive={true} />);
      
      const overlay = screen.getByRole('status');
      expect(overlay).toHaveClass('z-50');
    });
  });
});
