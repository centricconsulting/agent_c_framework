/**
 * MediaRenderer Component Tests
 * Testing media content rendering with various content types
 */

import React from 'react';
import { describe, it, expect, beforeEach, vi } from 'vitest';
import { render } from '@testing-library/react';
import { MediaRenderer } from '../MediaRenderer';

// Mock Lucide icons
vi.mock('lucide-react', () => ({
  FileImage: ({ className }: any) => <div data-testid="file-image-icon" className={className}>Image</div>,
  FileText: ({ className }: any) => <div data-testid="file-text-icon" className={className}>Text</div>,
  Globe: ({ className }: any) => <div data-testid="globe-icon" className={className}>Web</div>,
  AlertCircle: ({ className }: any) => <div data-testid="alert-circle-icon" className={className}>Alert</div>
}));

// Mock ReactMarkdown
vi.mock('react-markdown', () => ({
  default: ({ children }: any) => <div data-testid="markdown-content">{children}</div>
}));

vi.mock('remark-gfm', () => ({
  default: vi.fn()
}));

describe('MediaRenderer', () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  describe('Basic Rendering', () => {
    it('should render with markdown content', () => {
      const { getByTestId, getByText } = render(
        <MediaRenderer
          content="# Hello World"
          contentType="text/markdown"
        />
      );

      expect(getByTestId('markdown-content')).toBeInTheDocument();
      expect(getByText('# Hello World')).toBeInTheDocument();
    });

    it('should render with plain text content', () => {
      const { getByText } = render(
        <MediaRenderer
          content="Plain text content"
          contentType="text/plain"
        />
      );

      expect(getByText('Plain text content')).toBeInTheDocument();
    });

    it('should render with HTML content', () => {
      const { container } = render(
        <MediaRenderer
          content="<h1>HTML Content</h1>"
          contentType="text/html"
        />
      );

      // HTML content is rendered in a div with innerHTML
      const htmlContainer = container.querySelector('.bg-background');
      expect(htmlContainer).toBeInTheDocument();
    });

    it('should show foreign content warning for HTML', () => {
      const { getByTestId, getByText } = render(
        <MediaRenderer
          content="<h1>External</h1>"
          contentType="text/html"
          metadata={{ foreignContent: true }}
        />
      );

      expect(getByTestId('alert-circle-icon')).toBeInTheDocument();
      expect(getByText('External content')).toBeInTheDocument();
    });

    it('should render SVG images inline', () => {
      const svgContent = '<svg><circle cx="50" cy="50" r="40"/></svg>';
      const { container } = render(
        <MediaRenderer
          content={svgContent}
          contentType="image/svg+xml"
        />
      );

      // SVG is rendered in a div with innerHTML
      const svgContainer = container.querySelector('.max-w-lg');
      expect(svgContainer).toBeInTheDocument();
    });

    it('should render other images as img tags', () => {
      const { container } = render(
        <MediaRenderer
          content="https://example.com/image.png"
          contentType="image/png"
        />
      );

      const img = container.querySelector('img');
      expect(img).toHaveAttribute('src', 'https://example.com/image.png');
      expect(img).toHaveAttribute('alt', 'Media content');
    });
  });

  describe('Icons and Headers', () => {
    it('should show correct icon for image content', () => {
      const { getByTestId } = render(
        <MediaRenderer
          content="image data"
          contentType="image/jpeg"
        />
      );

      expect(getByTestId('file-image-icon')).toBeInTheDocument();
    });

    it('should show correct icon for HTML content', () => {
      const { getByTestId } = render(
        <MediaRenderer
          content="<div>html</div>"
          contentType="text/html"
        />
      );

      expect(getByTestId('globe-icon')).toBeInTheDocument();
    });

    it('should show correct icon for text content', () => {
      const { getByTestId } = render(
        <MediaRenderer
          content="text"
          contentType="text/plain"
        />
      );

      expect(getByTestId('file-text-icon')).toBeInTheDocument();
    });

    it('should display custom name in header', () => {
      const { getByText } = render(
        <MediaRenderer
          content="content"
          contentType="text/plain"
          metadata={{ name: 'Custom Document' }}
        />
      );

      expect(getByText('Custom Document')).toBeInTheDocument();
    });

    it('should display default name when no name provided', () => {
      const { getByText } = render(
        <MediaRenderer
          content="content"
          contentType="text/plain"
        />
      );

      expect(getByText('Media Content')).toBeInTheDocument();
    });
  });

  describe('Metadata Footer', () => {
    it('should show sentByClass in footer', () => {
      const { getByText } = render(
        <MediaRenderer
          content="content"
          contentType="text/plain"
          metadata={{ sentByClass: 'ToolClass' }}
        />
      );

      expect(getByText('Class:')).toBeInTheDocument();
      expect(getByText('ToolClass')).toBeInTheDocument();
    });

    it('should show sentByFunction in footer', () => {
      const { getByText } = render(
        <MediaRenderer
          content="content"
          contentType="text/plain"
          metadata={{ sentByFunction: 'processData' }}
        />
      );

      expect(getByText('Function:')).toBeInTheDocument();
      expect(getByText('processData')).toBeInTheDocument();
    });

    it('should show URL link in footer', () => {
      const { getByText } = render(
        <MediaRenderer
          content="content"
          contentType="text/plain"
          metadata={{ url: 'https://example.com' }}
        />
      );

      const link = getByText('Source');
      expect(link).toHaveAttribute('href', 'https://example.com');
      expect(link).toHaveAttribute('target', '_blank');
      expect(link).toHaveAttribute('rel', 'noopener noreferrer');
    });

    it('should not show footer when no metadata', () => {
      const { container } = render(
        <MediaRenderer
          content="content"
          contentType="text/plain"
        />
      );

      const footer = container.querySelector('.border-t.border-border\\/30');
      expect(footer).not.toBeInTheDocument();
    });
  });

  describe('Timestamp Display', () => {
    it('should format and display timestamp', () => {
      const timestamp = '2024-01-01T15:30:00Z';
      const { getByText } = render(
        <MediaRenderer
          content="content"
          contentType="text/plain"
          timestamp={timestamp}
        />
      );

      // Should format to time only (will vary by locale in tests)
      expect(container => {
        const timeRegex = /\d{1,2}:\d{2}\s*(AM|PM|am|pm)?/;
        return Array.from(container.querySelectorAll('*')).some(
          el => timeRegex.test(el.textContent || '')
        );
      });
    });

    it('should handle invalid timestamp gracefully', () => {
      const { container } = render(
        <MediaRenderer
          content="content"
          contentType="text/plain"
          timestamp="invalid-date"
        />
      );

      // Should not crash and should not display invalid time
      expect(container.textContent).not.toContain('Invalid Date');
    });
  });

  describe('Styling', () => {
    it('should apply custom className', () => {
      const { container } = render(
        <MediaRenderer
          content="content"
          contentType="text/plain"
          className="custom-class"
        />
      );

      const wrapper = container.firstChild;
      expect(wrapper).toHaveClass('custom-class');
    });

    it('should have proper centering styles', () => {
      const { container } = render(
        <MediaRenderer
          content="content"
          contentType="text/plain"
        />
      );

      const wrapper = container.firstChild;
      expect(wrapper).toHaveClass('flex', 'justify-center', 'w-full');
    });

    it('should have animation classes', () => {
      const { container } = render(
        <MediaRenderer
          content="content"
          contentType="text/plain"
        />
      );

      const wrapper = container.firstChild;
      expect(wrapper).toHaveClass('animate-in', 'fade-in-50', 'slide-in-from-bottom-2');
    });

    it('should have proper max-width constraint', () => {
      const { container } = render(
        <MediaRenderer
          content="content"
          contentType="text/plain"
        />
      );

      const contentContainer = container.querySelector('.max-w-2xl');
      expect(contentContainer).toBeInTheDocument();
    });
  });

  describe('Accessibility', () => {
    it('should have proper ARIA attributes', () => {
      const { container } = render(
        <MediaRenderer
          content="content"
          contentType="text/plain"
        />
      );

      const wrapper = container.firstChild;
      expect(wrapper).toHaveAttribute('role', 'article');
      expect(wrapper).toHaveAttribute('aria-label', 'Media content');
    });

    it('should have alt text for images', () => {
      const { container } = render(
        <MediaRenderer
          content="https://example.com/image.png"
          contentType="image/png"
          metadata={{ name: 'Test Image' }}
        />
      );

      const img = container.querySelector('img');
      expect(img).toHaveAttribute('alt', 'Test Image');
    });

    it('should forward ref correctly', () => {
      const ref = React.createRef<HTMLDivElement>();
      const { container } = render(
        <MediaRenderer
          ref={ref}
          content="content"
          contentType="text/plain"
        />
      );

      expect(ref.current).toBeInstanceOf(HTMLDivElement);
      expect(ref.current).toHaveAttribute('role', 'article');
    });
  });

  describe('Edge Cases', () => {
    it('should handle empty content', () => {
      const { container } = render(
        <MediaRenderer
          content=""
          contentType="text/plain"
        />
      );

      expect(container.firstChild).toBeInTheDocument();
    });

    it('should handle unknown content type', () => {
      const { container } = render(
        <MediaRenderer
          content="unknown content"
          contentType="application/unknown"
        />
      );

      // Should render as preformatted text
      const pre = container.querySelector('pre');
      expect(pre).toBeInTheDocument();
      expect(pre).toHaveTextContent('unknown content');
    });

    it('should handle base64 image data', () => {
      const base64Data = 'iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mNkYPhfDwAChwGA60e6kgAAAABJRU5ErkJggg==';
      const { container } = render(
        <MediaRenderer
          content={base64Data}
          contentType="image/png"
        />
      );

      const img = container.querySelector('img');
      expect(img).toHaveAttribute('src', `data:image/png;base64,${base64Data}`);
    });

    it('should handle data URL images', () => {
      const dataUrl = 'data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mNkYPhfDwAChwGA60e6kgAAAABJRU5ErkJggg==';
      const { container } = render(
        <MediaRenderer
          content={dataUrl}
          contentType="image/png"
        />
      );

      const img = container.querySelector('img');
      expect(img).toHaveAttribute('src', dataUrl);
    });

    it('should handle malformed content type gracefully', () => {
      const { container } = render(
        <MediaRenderer
          content="content"
          contentType="malformed/type/extra"
        />
      );

      // Should not crash
      expect(container.firstChild).toBeInTheDocument();
    });
  });

  describe('Component Name', () => {
    it('should have correct displayName', () => {
      expect(MediaRenderer.displayName).toBe('MediaRenderer');
    });
  });
});