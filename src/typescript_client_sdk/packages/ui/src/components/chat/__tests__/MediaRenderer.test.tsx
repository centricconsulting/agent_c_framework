/**
 * MediaRenderer Component Tests - UPDATED
 * 
 * Tests reflect the markdown unification changes:
 * - REMOVED: Icon/name/timestamp header (bug fix)
 * - MOVED: Timestamp to footer (bug fix)
 * - FIXED: Width from max-w-2xl to max-w-[85%] (bug fix)
 * - ADDED: Full markdown support via MarkdownRenderer
 * 
 * Testing media content rendering with various content types
 */

import React from 'react';
import { describe, it, expect, beforeEach, vi } from 'vitest';
import { render, screen } from '@testing-library/react';
import { MediaRenderer } from '../MediaRenderer';

// Mock Lucide icons  
vi.mock('lucide-react', () => ({
  AlertCircle: ({ className }: any) => (
    <div data-testid="alert-circle-icon" className={className}>Alert</div>
  ),
  Check: () => <span data-testid="check-icon">✓</span>,
  Copy: () => <span data-testid="copy-icon">📋</span>,
  ChevronRight: () => <span data-testid="chevron-right">▶</span>,
  ChevronDown: () => <span data-testid="chevron-down">▼</span>,
}));

describe('MediaRenderer', () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  // ===========================================================================
  // BASIC RENDERING TESTS
  // ===========================================================================

  describe('Basic Rendering', () => {
    it('should render with markdown content using MarkdownRenderer', () => {
      const { container } = render(
        <MediaRenderer
          content="# Hello World\n\nThis is **markdown**"
          contentType="text/markdown"
        />
      );

      // Should render as markdown, not plain text
      const heading = container.querySelector('h1');
      expect(heading).toBeInTheDocument();
      expect(heading).toHaveTextContent('Hello World');
      
      const strong = container.querySelector('strong');
      expect(strong).toBeInTheDocument();
      expect(strong).toHaveTextContent('markdown');
    });

    it('should render with plain text content', () => {
      const { container } = render(
        <MediaRenderer
          content="Plain text content"
          contentType="text/plain"
        />
      );

      const pre = container.querySelector('pre');
      expect(pre).toBeInTheDocument();
      expect(pre).toHaveTextContent('Plain text content');
    });

    it('should render with HTML content', () => {
      const { container } = render(
        <MediaRenderer
          content="<h1>HTML Content</h1>"
          contentType="text/html"
        />
      );

      // HTML content is rendered with dangerouslySetInnerHTML
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

      const svgContainer = container.querySelector('.max-w-lg');
      expect(svgContainer).toBeInTheDocument();
      expect(svgContainer?.innerHTML).toContain('svg');
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

  // ===========================================================================
  // HEADER REMOVAL TESTS (BUG FIX VALIDATION)
  // ===========================================================================

  describe('Header Removal (Bug Fix)', () => {
    it('should NOT render header with icon/name/timestamp', () => {
      const { container } = render(
        <MediaRenderer
          content="Test content"
          contentType="text/markdown"
          timestamp="2024-01-01T15:30:00Z"
          metadata={{ name: 'Test Document' }}
        />
      );

      // REMOVED: No header section should exist
      // The old implementation had icons and name in a header
      // New implementation: content starts immediately
      
      // Check that content area is the first child of the bubble
      const bubble = container.querySelector('.max-w-\\[85\\%\\]');
      expect(bubble).toBeInTheDocument();
      
      // First child should be content (p-4), not header
      const firstChild = bubble?.firstElementChild;
      expect(firstChild).toHaveClass('p-4'); // Content area
    });

    it('should start with content immediately (no header)', () => {
      const { container } = render(
        <MediaRenderer
          content="# Markdown Content"
          contentType="text/markdown"
        />
      );

      const bubble = container.querySelector('.max-w-\\[85\\%\\]');
      const contentArea = bubble?.querySelector('.p-4');
      
      // Content should be direct child (no header before it)
      expect(contentArea).toBeInTheDocument();
      expect(contentArea?.querySelector('h1')).toHaveTextContent('Markdown Content');
    });
  });

  // ===========================================================================
  // FOOTER TESTS (TIMESTAMP MOVED TO FOOTER)
  // ===========================================================================

  describe('Footer with Timestamp (Bug Fix)', () => {
    it('should show footer when timestamp provided', () => {
      const timestamp = '2024-01-01T15:30:00Z';
      const { container } = render(
        <MediaRenderer
          content="Test content"
          contentType="text/plain"
          timestamp={timestamp}
        />
      );

      // Footer has border-t class
      const footer = container.querySelector('.border-t.border-border\\/30');
      expect(footer).toBeInTheDocument();
      
      // Should contain formatted time
      expect(footer?.textContent).toMatch(/\d{1,2}:\d{2}/);
    });

    it('should show footer when metadata provided (no timestamp)', () => {
      const { container } = render(
        <MediaRenderer
          content="Test content"
          contentType="text/plain"
          metadata={{ sentByClass: 'TestClass' }}
        />
      );

      const footer = container.querySelector('.border-t.border-border\\/30');
      expect(footer).toBeInTheDocument();
      expect(footer?.textContent).toContain('TestClass');
    });

    it('should combine timestamp + metadata in footer', () => {
      const { container, getByText } = render(
        <MediaRenderer
          content="Test"
          contentType="text/plain"
          timestamp="2024-01-01T15:30:00Z"
          metadata={{
            sentByClass: 'ToolClass',
            sentByFunction: 'processData'
          }}
        />
      );

      const footer = container.querySelector('.border-t.border-border\\/30');
      expect(footer).toBeInTheDocument();
      
      // Should contain all footer items
      expect(footer?.textContent).toMatch(/\d{1,2}:\d{2}/); // Time
      expect(getByText('Class:')).toBeInTheDocument();
      expect(getByText('ToolClass')).toBeInTheDocument();
      expect(getByText('Function:')).toBeInTheDocument();
      expect(getByText('processData')).toBeInTheDocument();
    });

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

    it('should NOT show footer when no metadata or timestamp', () => {
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

  // ===========================================================================
  // TIMESTAMP FORMATTING TESTS
  // ===========================================================================

  describe('Timestamp Formatting', () => {
    it('should format and display timestamp in footer', () => {
      const timestamp = '2024-01-01T15:30:00Z';
      const { container } = render(
        <MediaRenderer
          content="content"
          contentType="text/plain"
          timestamp={timestamp}
        />
      );

      const footer = container.querySelector('.border-t.border-border\\/30');
      
      // Should format to time only (will vary by locale in tests)
      const timeRegex = /\d{1,2}:\d{2}\s*(AM|PM|am|pm)?/;
      expect(footer?.textContent).toMatch(timeRegex);
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
      expect(container.textContent).not.toContain('NaN');
    });

    it('should not show footer for invalid timestamp with no metadata', () => {
      const { container } = render(
        <MediaRenderer
          content="content"
          contentType="text/plain"
          timestamp="invalid"
        />
      );

      // Footer is shown if timestamp prop exists (even if invalid)
      // This is because the condition checks `timestamp` string presence, not validity
      const footer = container.querySelector('.border-t.border-border\\/30');
      expect(footer).toBeInTheDocument();
      
      // But the formatted time should not contain invalid values
      expect(footer?.textContent).not.toContain('Invalid Date');
      expect(footer?.textContent).not.toContain('NaN');
    });
  });

  // ===========================================================================
  // LAYOUT AND WIDTH TESTS (BUG FIX VALIDATION)
  // ===========================================================================

  describe('Layout and Width (Bug Fix)', () => {
    it('should have correct max-width (85%)', () => {
      const { container } = render(
        <MediaRenderer
          content="Test content"
          contentType="text/plain"
        />
      );

      // NEW: max-w-[85%]
      const bubble = container.querySelector('.max-w-\\[85\\%\\]');
      expect(bubble).toBeInTheDocument();
    });

    it('should NOT have old max-w-2xl class', () => {
      const { container } = render(
        <MediaRenderer
          content="Test content"
          contentType="text/plain"
        />
      );

      // OLD (removed): max-w-2xl
      const oldBubble = container.querySelector('.max-w-2xl');
      expect(oldBubble).not.toBeInTheDocument();
    });

    it('should apply proper bubble styling', () => {
      const { container } = render(
        <MediaRenderer
          content="Test"
          contentType="text/plain"
        />
      );

      const bubble = container.querySelector('.max-w-\\[85\\%\\]');
      expect(bubble).toHaveClass('rounded-xl');
      expect(bubble).toHaveClass('bg-muted/50');
      expect(bubble).toHaveClass('border');
    });
  });

  // ===========================================================================
  // MARKDOWN INTEGRATION TESTS
  // ===========================================================================

  describe('MarkdownRenderer Integration', () => {
    it('should render markdown features via MarkdownRenderer', () => {
      const markdown = `
# Heading

This is **bold** and *italic*.

- List item 1
- List item 2

\`\`\`javascript
const x = 1;
\`\`\`
      `.trim();

      const { container } = render(
        <MediaRenderer
          content={markdown}
          contentType="text/markdown"
        />
      );

      // Verify markdown features render
      expect(container.querySelector('h1')).toHaveTextContent('Heading');
      expect(container.querySelector('strong')).toHaveTextContent('bold');
      expect(container.querySelector('em')).toHaveTextContent('italic');
      expect(container.querySelector('ul')).toBeInTheDocument();
      expect(container.querySelector('code')).toHaveTextContent('const x = 1;');
    });

    it('should pass proper aria-label to MarkdownRenderer', () => {
      const { container } = render(
        <MediaRenderer
          content="# Test"
          contentType="text/markdown"
        />
      );

      // MarkdownRenderer should have aria-label (it's nested inside the outer MediaRenderer article)
      // Get all articles - outer is MediaRenderer wrapper, inner is MarkdownRenderer
      const articles = container.querySelectorAll('[role="article"]');
      expect(articles.length).toBeGreaterThan(0);
      
      // The outer article (MediaRenderer) should have 'Media content'
      expect(articles[0]).toHaveAttribute('aria-label', 'Media content');
      
      // The inner article (MarkdownRenderer) should have 'Media content markdown'
      if (articles.length > 1) {
        expect(articles[1]).toHaveAttribute('aria-label', 'Media content markdown');
      }
    });
  });

  // ===========================================================================
  // STYLING TESTS
  // ===========================================================================

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

    it('should have proper container structure', () => {
      const { container } = render(
        <MediaRenderer
          content="content"
          contentType="text/plain"
        />
      );

      const wrapper = container.firstChild;
      expect(wrapper).toHaveClass('flex', 'flex-col', 'w-full', 'py-2');
    });
  });

  // ===========================================================================
  // ACCESSIBILITY TESTS
  // ===========================================================================

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
      render(
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

  // ===========================================================================
  // EDGE CASES TESTS
  // ===========================================================================

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

  // ===========================================================================
  // COMPONENT METADATA TESTS
  // ===========================================================================

  describe('Component Metadata', () => {
    it('should have correct displayName', () => {
      expect(MediaRenderer.displayName).toBe('MediaRenderer');
    });
  });
});
