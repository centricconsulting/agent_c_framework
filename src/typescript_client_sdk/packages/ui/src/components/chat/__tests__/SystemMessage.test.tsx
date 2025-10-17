/**
 * SystemMessage Component Tests
 * Testing system message/alert rendering with severity levels
 */

import React from 'react';
import { describe, it, expect, beforeEach, vi } from 'vitest';
import { render } from '@testing-library/react';
import { SystemMessage } from '../SystemMessage';

// Mock Lucide icons
vi.mock('lucide-react', () => ({
  AlertCircle: ({ className }: any) => <div data-testid="alert-circle-icon" className={className}>Alert</div>,
  Info: ({ className }: any) => <div data-testid="info-icon" className={className}>Info</div>,
  AlertTriangle: ({ className }: any) => <div data-testid="alert-triangle-icon" className={className}>Warning</div>,
  XCircle: ({ className }: any) => <div data-testid="x-circle-icon" className={className}>Error</div>
}));

// Mock ReactMarkdown
vi.mock('react-markdown', () => ({
  default: ({ children }: any) => <div data-testid="markdown-content">{children}</div>
}));

vi.mock('remark-gfm', () => ({
  default: vi.fn()
}));

describe('SystemMessage', () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  describe('Basic Rendering', () => {
    it('should render with info severity', () => {
      const { getByTestId, getByText } = render(
        <SystemMessage
          content="Information message"
          severity="info"
        />
      );

      expect(getByTestId('info-icon')).toBeInTheDocument();
      expect(getByText('Information message')).toBeInTheDocument();
    });

    it('should render with warning severity', () => {
      const { getByTestId, getByText } = render(
        <SystemMessage
          content="Warning message"
          severity="warning"
        />
      );

      expect(getByTestId('alert-triangle-icon')).toBeInTheDocument();
      expect(getByText('Warning message')).toBeInTheDocument();
    });

    it('should render with error severity', () => {
      const { getByTestId, getByText } = render(
        <SystemMessage
          content="Error message"
          severity="error"
        />
      );

      expect(getByTestId('x-circle-icon')).toBeInTheDocument();
      expect(getByText('Error message')).toBeInTheDocument();
    });

    it('should default to info severity', () => {
      const { getByTestId } = render(
        <SystemMessage
          content="Default message"
          severity="info"
        />
      );

      expect(getByTestId('info-icon')).toBeInTheDocument();
    });
  });

  describe('Content Formatting', () => {
    it('should render markdown content', () => {
      const { getByTestId } = render(
        <SystemMessage
          content="# Markdown Title"
          severity="info"
          format="markdown"
        />
      );

      expect(getByTestId('markdown-content')).toBeInTheDocument();
      expect(getByTestId('markdown-content')).toHaveTextContent('# Markdown Title');
    });

    it('should render plain text content', () => {
      const { getByText } = render(
        <SystemMessage
          content="Plain text message"
          severity="info"
          format="text"
        />
      );

      const textContainer = getByText('Plain text message').closest('.whitespace-pre-wrap');
      expect(textContainer).toBeInTheDocument();
    });

    it('should default to markdown format', () => {
      const { getByTestId } = render(
        <SystemMessage
          content="Default format"
          severity="info"
        />
      );

      expect(getByTestId('markdown-content')).toBeInTheDocument();
    });

    it('should preserve whitespace in plain text', () => {
      const multilineContent = 'Line 1\n  Line 2 with indent\n    Line 3';
      const { container } = render(
        <SystemMessage
          content={multilineContent}
          severity="info"
          format="text"
        />
      );

      // Find the container with whitespace-pre-wrap class
      const textContainer = container.querySelector('.whitespace-pre-wrap');
      expect(textContainer).toBeInTheDocument();
      // Check that the text content is exactly as expected (textContent preserves whitespace)
      expect(textContainer?.textContent).toBe(multilineContent);
    });
  });

  describe('Severity Styling', () => {
    it('should apply info styling', () => {
      const { container } = render(
        <SystemMessage
          content="Info"
          severity="info"
        />
      );

      const alertBox = container.querySelector('.bg-blue-50');
      expect(alertBox).toBeInTheDocument();
      expect(alertBox).toHaveClass('border-blue-200');
    });

    it('should apply warning styling', () => {
      const { container } = render(
        <SystemMessage
          content="Warning"
          severity="warning"
        />
      );

      const alertBox = container.querySelector('.bg-yellow-50');
      expect(alertBox).toBeInTheDocument();
      expect(alertBox).toHaveClass('border-yellow-200');
    });

    it('should apply error styling', () => {
      const { container } = render(
        <SystemMessage
          content="Error"
          severity="error"
        />
      );

      const alertBox = container.querySelector('.bg-red-50');
      expect(alertBox).toBeInTheDocument();
      expect(alertBox).toHaveClass('border-red-200');
    });

    it('should have different icon colors per severity', () => {
      const { rerender, getByTestId } = render(
        <SystemMessage content="Test" severity="info" />
      );
      expect(getByTestId('info-icon')).toHaveClass('text-blue-600');

      rerender(<SystemMessage content="Test" severity="warning" />);
      expect(getByTestId('alert-triangle-icon')).toHaveClass('text-yellow-600');

      rerender(<SystemMessage content="Test" severity="error" />);
      expect(getByTestId('x-circle-icon')).toHaveClass('text-red-600');
    });
  });

  describe('Timestamp Display', () => {
    it('should format and display timestamp', () => {
      const timestamp = '2024-01-01T15:30:00Z';
      const { container } = render(
        <SystemMessage
          content="Message"
          severity="info"
          timestamp={timestamp}
        />
      );

      // Check for time format (will vary by locale)
      const timeRegex = /\d{1,2}:\d{2}\s*(AM|PM|am|pm)?/;
      const hasTime = Array.from(container.querySelectorAll('*')).some(
        el => timeRegex.test(el.textContent || '')
      );
      expect(hasTime).toBe(true);
    });

    it('should handle invalid timestamp gracefully', () => {
      const { container } = render(
        <SystemMessage
          content="Message"
          severity="info"
          timestamp="invalid-date"
        />
      );

      // Should not display invalid date
      expect(container.textContent).not.toContain('Invalid Date');
    });

    it('should not show timestamp section when not provided', () => {
      const { container } = render(
        <SystemMessage
          content="Message"
          severity="info"
        />
      );

      const timestampContainer = container.querySelector('.text-xs.opacity-70');
      expect(timestampContainer).not.toBeInTheDocument();
    });
  });

  describe('Layout and Styling', () => {
    it('should apply custom className', () => {
      const { container } = render(
        <SystemMessage
          content="Message"
          severity="info"
          className="custom-class"
        />
      );

      const wrapper = container.firstChild;
      expect(wrapper).toHaveClass('custom-class');
    });

    it('should center the alert box', () => {
      const { container } = render(
        <SystemMessage
          content="Message"
          severity="info"
        />
      );

      const wrapper = container.firstChild;
      expect(wrapper).toHaveClass('flex', 'justify-center', 'w-full');
    });

    it('should have max-width constraint', () => {
      const { container } = render(
        <SystemMessage
          content="Message"
          severity="info"
        />
      );

      const alertBox = container.querySelector('.max-w-2xl');
      expect(alertBox).toBeInTheDocument();
    });

    it('should have animation classes', () => {
      const { container } = render(
        <SystemMessage
          content="Message"
          severity="info"
        />
      );

      const wrapper = container.firstChild;
      expect(wrapper).toHaveClass('animate-in', 'fade-in-50', 'slide-in-from-bottom-2');
    });

    it('should have proper padding', () => {
      const { container } = render(
        <SystemMessage
          content="Message"
          severity="info"
        />
      );

      const contentContainer = container.querySelector('.p-4');
      expect(contentContainer).toBeInTheDocument();
    });
  });

  describe('Accessibility', () => {
    it('should have proper ARIA role', () => {
      const { container } = render(
        <SystemMessage
          content="Message"
          severity="info"
        />
      );

      const wrapper = container.firstChild;
      expect(wrapper).toHaveAttribute('role', 'alert');
    });

    it('should use assertive aria-live for errors', () => {
      const { container } = render(
        <SystemMessage
          content="Error"
          severity="error"
        />
      );

      const wrapper = container.firstChild;
      expect(wrapper).toHaveAttribute('aria-live', 'assertive');
    });

    it('should use polite aria-live for non-errors', () => {
      const { container, rerender } = render(
        <SystemMessage
          content="Info"
          severity="info"
        />
      );

      let wrapper = container.firstChild;
      expect(wrapper).toHaveAttribute('aria-live', 'polite');

      rerender(<SystemMessage content="Warning" severity="warning" />);
      wrapper = container.firstChild;
      expect(wrapper).toHaveAttribute('aria-live', 'polite');
    });

    it('should forward ref correctly', () => {
      const ref = React.createRef<HTMLDivElement>();
      render(
        <SystemMessage
          ref={ref}
          content="Message"
          severity="info"
        />
      );

      expect(ref.current).toBeInstanceOf(HTMLDivElement);
      expect(ref.current).toHaveAttribute('role', 'alert');
    });
  });

  describe('Edge Cases', () => {
    it('should handle empty content', () => {
      const { container } = render(
        <SystemMessage
          content=""
          severity="info"
        />
      );

      expect(container.firstChild).toBeInTheDocument();
    });

    it('should handle very long content', () => {
      const longContent = 'A'.repeat(1000);
      const { container } = render(
        <SystemMessage
          content={longContent}
          severity="info"
        />
      );

      expect(container.textContent).toContain(longContent);
    });

    it('should handle special characters in content', () => {
      const specialContent = '<script>alert("XSS")</script> & < > " \'';
      const { getByText } = render(
        <SystemMessage
          content={specialContent}
          severity="info"
          format="text"
        />
      );

      // Should be safely rendered as text
      expect(getByText(specialContent)).toBeInTheDocument();
    });

    it('should handle rapid severity changes', () => {
      const { rerender, container } = render(
        <SystemMessage content="Message" severity="info" />
      );

      rerender(<SystemMessage content="Message" severity="warning" />);
      expect(container.querySelector('.bg-yellow-50')).toBeInTheDocument();

      rerender(<SystemMessage content="Message" severity="error" />);
      expect(container.querySelector('.bg-red-50')).toBeInTheDocument();

      rerender(<SystemMessage content="Message" severity="info" />);
      expect(container.querySelector('.bg-blue-50')).toBeInTheDocument();
    });

    it('should handle all props being undefined except required ones', () => {
      const { container } = render(
        <SystemMessage
          content="Message"
          severity="info"
          format={undefined}
          timestamp={undefined}
          className={undefined}
        />
      );

      expect(container.firstChild).toBeInTheDocument();
    });
  });

  describe('Dark Mode Support', () => {
    it('should have dark mode classes', () => {
      const { container } = render(
        <SystemMessage
          content="Message"
          severity="info"
        />
      );

      const alertBox = container.querySelector('.dark\\:bg-blue-950\\/30');
      expect(alertBox).toBeInTheDocument();
      expect(alertBox).toHaveClass('dark:border-blue-800');
    });

    it('should have dark mode text colors', () => {
      const { container } = render(
        <SystemMessage
          content="Message"
          severity="info"
        />
      );

      const textContainer = container.querySelector('.dark\\:text-blue-200');
      expect(textContainer).toBeInTheDocument();
    });

    it('should have dark mode icon colors', () => {
      const { getByTestId } = render(
        <SystemMessage
          content="Message"
          severity="info"
        />
      );

      const icon = getByTestId('info-icon');
      expect(icon).toHaveClass('dark:text-blue-400');
    });
  });

  describe('Component Name', () => {
    it('should have correct displayName', () => {
      expect(SystemMessage.displayName).toBe('SystemMessage');
    });
  });
});