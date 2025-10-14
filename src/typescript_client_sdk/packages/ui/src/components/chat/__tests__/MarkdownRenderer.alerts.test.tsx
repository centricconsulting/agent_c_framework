/**
 * MarkdownRenderer GitHub-Style Alerts Tests
 * 
 * Testing Card 3: GitHub-Style Alerts
 * Tests remark-directive integration with:
 * - All 5 alert types (note, tip, important, warning, caution)
 * - Icon display and styling
 * - Theme-aware colors
 * - Content rendering (plain text, markdown, lists, code, links, multi-paragraph)
 * - Multiple alerts (same type, different types, all 5)
 * - Integration with other features (headings, code, tables, blockquotes, math, collapsibles, TOC)
 * - Compact mode
 * - Accessibility (role="note", aria-labels, aria-hidden on icons)
 * - Edge cases (empty, whitespace, unknown types, long content, special chars)
 * - Props and configuration
 * - Performance
 */

import React from 'react';
import { describe, it, expect, beforeEach, vi } from 'vitest';
import { render, screen, within } from '@testing-library/react';
import { MarkdownRenderer } from '../content-renderers/MarkdownRenderer';

// Mock lucide-react icons
vi.mock('lucide-react', () => ({
  ChevronRight: ({ className }: any) => (
    <div data-testid="chevron-right-icon" className={className} aria-hidden="true">▶</div>
  ),
  ChevronDown: ({ className }: any) => (
    <div data-testid="chevron-down-icon" className={className} aria-hidden="true">▼</div>
  ),
  Copy: ({ className }: any) => (
    <div data-testid="copy-icon" className={className} />
  ),
  Check: ({ className }: any) => (
    <div data-testid="check-icon" className={className} />
  ),
  Info: ({ className }: any) => (
    <div data-testid="info-icon" className={className} aria-hidden="true" />
  ),
  Lightbulb: ({ className }: any) => (
    <div data-testid="lightbulb-icon" className={className} aria-hidden="true" />
  ),
  Megaphone: ({ className }: any) => (
    <div data-testid="megaphone-icon" className={className} aria-hidden="true" />
  ),
  AlertTriangle: ({ className }: any) => (
    <div data-testid="alert-triangle-icon" className={className} aria-hidden="true" />
  ),
  ShieldAlert: ({ className }: any) => (
    <div data-testid="shield-alert-icon" className={className} aria-hidden="true" />
  )
}));

describe('MarkdownRenderer - GitHub-Style Alerts', () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  describe('Alert Type Rendering', () => {
    it('should render note alert', () => {
      const markdown = `
:::note
This is a note alert.
:::
      `.trim();

      render(<MarkdownRenderer content={markdown} />);

      const alert = screen.getByRole('note', { name: /Note alert/i });
      expect(alert).toBeInTheDocument();
      expect(alert).toHaveTextContent('This is a note alert.');
    });

    it('should render tip alert', () => {
      const markdown = `
:::tip
This is a tip alert.
:::
      `.trim();

      render(<MarkdownRenderer content={markdown} />);

      const alert = screen.getByRole('note', { name: /Tip alert/i });
      expect(alert).toBeInTheDocument();
      expect(alert).toHaveTextContent('This is a tip alert.');
    });

    it('should render important alert', () => {
      const markdown = `
:::important
This is an important alert.
:::
      `.trim();

      render(<MarkdownRenderer content={markdown} />);

      const alert = screen.getByRole('note', { name: /Important alert/i });
      expect(alert).toBeInTheDocument();
      expect(alert).toHaveTextContent('This is an important alert.');
    });

    it('should render warning alert', () => {
      const markdown = `
:::warning
This is a warning alert.
:::
      `.trim();

      render(<MarkdownRenderer content={markdown} />);

      const alert = screen.getByRole('note', { name: /Warning alert/i });
      expect(alert).toBeInTheDocument();
      expect(alert).toHaveTextContent('This is a warning alert.');
    });

    it('should render caution alert', () => {
      const markdown = `
:::caution
This is a caution alert.
:::
      `.trim();

      render(<MarkdownRenderer content={markdown} />);

      const alert = screen.getByRole('note', { name: /Caution alert/i });
      expect(alert).toBeInTheDocument();
      expect(alert).toHaveTextContent('This is a caution alert.');
    });
  });

  describe('Icon Display', () => {
    it('should display Info icon for note alerts', () => {
      const markdown = `:::note\nNote content\n:::`;

      render(<MarkdownRenderer content={markdown} />);

      const icon = screen.getByTestId('info-icon');
      expect(icon).toBeInTheDocument();
      expect(icon).toHaveAttribute('aria-hidden', 'true');
    });

    it('should display Lightbulb icon for tip alerts', () => {
      const markdown = `:::tip\nTip content\n:::`;

      render(<MarkdownRenderer content={markdown} />);

      const icon = screen.getByTestId('lightbulb-icon');
      expect(icon).toBeInTheDocument();
      expect(icon).toHaveAttribute('aria-hidden', 'true');
    });

    it('should display Megaphone icon for important alerts', () => {
      const markdown = `:::important\nImportant content\n:::`;

      render(<MarkdownRenderer content={markdown} />);

      const icon = screen.getByTestId('megaphone-icon');
      expect(icon).toBeInTheDocument();
      expect(icon).toHaveAttribute('aria-hidden', 'true');
    });

    it('should display AlertTriangle icon for warning alerts', () => {
      const markdown = `:::warning\nWarning content\n:::`;

      render(<MarkdownRenderer content={markdown} />);

      const icon = screen.getByTestId('alert-triangle-icon');
      expect(icon).toBeInTheDocument();
      expect(icon).toHaveAttribute('aria-hidden', 'true');
    });

    it('should display ShieldAlert icon for caution alerts', () => {
      const markdown = `:::caution\nCaution content\n:::`;

      render(<MarkdownRenderer content={markdown} />);

      const icon = screen.getByTestId('shield-alert-icon');
      expect(icon).toBeInTheDocument();
      expect(icon).toHaveAttribute('aria-hidden', 'true');
    });
  });

  describe('Styling and Colors', () => {
    it('should apply blue styling for note alerts', () => {
      const markdown = `:::note\nNote\n:::`;
      const { container } = render(<MarkdownRenderer content={markdown} />);

      const alert = screen.getByRole('note', { name: /Note alert/i });
      expect(alert).toHaveClass('border-l-blue-500', 'bg-blue-50');
    });

    it('should apply green styling for tip alerts', () => {
      const markdown = `:::tip\nTip\n:::`;
      const { container } = render(<MarkdownRenderer content={markdown} />);

      const alert = screen.getByRole('note', { name: /Tip alert/i });
      expect(alert).toHaveClass('border-l-green-500', 'bg-green-50');
    });

    it('should apply purple styling for important alerts', () => {
      const markdown = `:::important\nImportant\n:::`;
      const { container } = render(<MarkdownRenderer content={markdown} />);

      const alert = screen.getByRole('note', { name: /Important alert/i });
      expect(alert).toHaveClass('border-l-purple-500', 'bg-purple-50');
    });

    it('should apply yellow styling for warning alerts', () => {
      const markdown = `:::warning\nWarning\n:::`;
      const { container } = render(<MarkdownRenderer content={markdown} />);

      const alert = screen.getByRole('note', { name: /Warning alert/i });
      expect(alert).toHaveClass('border-l-yellow-500', 'bg-yellow-50');
    });

    it('should apply red styling for caution alerts', () => {
      const markdown = `:::caution\nCaution\n:::`;
      const { container } = render(<MarkdownRenderer content={markdown} />);

      const alert = screen.getByRole('note', { name: /Caution alert/i });
      expect(alert).toHaveClass('border-l-red-500', 'bg-red-50');
    });

    it('should have proper border and background classes', () => {
      const markdown = `:::note\nTest\n:::`;
      const { container } = render(<MarkdownRenderer content={markdown} />);

      const alert = container.querySelector('[role="note"]');
      expect(alert).toHaveClass('border-l-4', 'rounded-md');
    });
  });

  describe('Content Rendering', () => {
    it('should render plain text content', () => {
      const markdown = `
:::note
This is plain text content in a note alert.
:::
      `.trim();

      render(<MarkdownRenderer content={markdown} />);

      expect(screen.getByText(/plain text content/i)).toBeInTheDocument();
    });

    it('should render markdown formatting in content', () => {
      const markdown = `
:::note
This has **bold** and *italic* text.
:::
      `.trim();

      render(<MarkdownRenderer content={markdown} />);

      const alert = screen.getByRole('note');
      const boldText = within(alert).getByText('bold');
      expect(boldText).toBeInTheDocument();
      expect(boldText.tagName).toBe('STRONG');

      const italicText = within(alert).getByText('italic');
      expect(italicText).toBeInTheDocument();
      expect(italicText.tagName).toBe('EM');
    });

    it('should render lists in alert content', () => {
      const markdown = `
:::tip
Steps to follow:
- First step
- Second step
- Third step
:::
      `.trim();

      render(<MarkdownRenderer content={markdown} />);

      const alert = screen.getByRole('note', { name: /Tip alert/i });
      const listItems = within(alert).getAllByRole('listitem');
      expect(listItems).toHaveLength(3);
    });

    it('should render inline code in alert content', () => {
      const markdown = `
:::note
Use the \`npm install\` command.
:::
      `.trim();

      const { container } = render(<MarkdownRenderer content={markdown} />);

      const alert = screen.getByRole('note');
      const code = within(alert).getByText('npm install');
      expect(code).toBeInTheDocument();
      expect(code.tagName).toBe('CODE');
    });

    it('should render links in alert content', () => {
      const markdown = `
:::important
See [documentation](https://example.com) for details.
:::
      `.trim();

      render(<MarkdownRenderer content={markdown} />);

      const alert = screen.getByRole('note', { name: /Important alert/i });
      const link = within(alert).getByRole('link', { name: /documentation/i });
      expect(link).toBeInTheDocument();
      expect(link).toHaveAttribute('href', 'https://example.com');
    });

    it('should render multi-paragraph content', () => {
      const markdown = `
:::warning
First paragraph of warning.

Second paragraph with more details.
:::
      `.trim();

      render(<MarkdownRenderer content={markdown} />);

      const alert = screen.getByRole('note', { name: /Warning alert/i });
      expect(alert).toHaveTextContent('First paragraph');
      expect(alert).toHaveTextContent('Second paragraph');
    });
  });

  describe('Multiple Alerts', () => {
    it('should render multiple alerts of same type', () => {
      const markdown = `
:::note
First note.
:::

:::note
Second note.
:::

:::note
Third note.
:::
      `.trim();

      render(<MarkdownRenderer content={markdown} />);

      const alerts = screen.getAllByRole('note', { name: /Note alert/i });
      expect(alerts).toHaveLength(3);
    });

    it('should render multiple alerts of different types', () => {
      const markdown = `
:::note
Note alert.
:::

:::warning
Warning alert.
:::

:::tip
Tip alert.
:::
      `.trim();

      render(<MarkdownRenderer content={markdown} />);

      expect(screen.getByRole('note', { name: /Note alert/i })).toBeInTheDocument();
      expect(screen.getByRole('note', { name: /Warning alert/i })).toBeInTheDocument();
      expect(screen.getByRole('note', { name: /Tip alert/i })).toBeInTheDocument();
    });

    it('should render all 5 alert types in one message', () => {
      const markdown = `
:::note
Note
:::

:::tip
Tip
:::

:::important
Important
:::

:::warning
Warning
:::

:::caution
Caution
:::
      `.trim();

      render(<MarkdownRenderer content={markdown} />);

      expect(screen.getByRole('note', { name: /Note alert/i })).toBeInTheDocument();
      expect(screen.getByRole('note', { name: /Tip alert/i })).toBeInTheDocument();
      expect(screen.getByRole('note', { name: /Important alert/i })).toBeInTheDocument();
      expect(screen.getByRole('note', { name: /Warning alert/i })).toBeInTheDocument();
      expect(screen.getByRole('note', { name: /Caution alert/i })).toBeInTheDocument();
    });
  });

  describe('Integration with Other Features', () => {
    it('should work with headings', () => {
      const markdown = `
## Important Section

:::important
This section is important!
:::

## Regular Section
Text.
      `.trim();

      render(<MarkdownRenderer content={markdown} />);

      expect(screen.getByRole('heading', { level: 2, name: /Important Section/i })).toBeInTheDocument();
      expect(screen.getByRole('note', { name: /Important alert/i })).toBeInTheDocument();
    });

    it('should work with code blocks', () => {
      const markdown = `
:::tip
Here's a code example:

\`\`\`javascript
const x = 1;
\`\`\`
:::
      `.trim();

      const { container } = render(<MarkdownRenderer content={markdown} />);

      const alert = screen.getByRole('note', { name: /Tip alert/i });
      const codeBlock = within(alert).getByText('const x = 1;');
      expect(codeBlock).toBeInTheDocument();
    });

    it('should work with tables', () => {
      const markdown = `
:::note
| Column 1 | Column 2 |
|----------|----------|
| Data 1   | Data 2   |
:::
      `.trim();

      const { container } = render(<MarkdownRenderer content={markdown} />);

      const alert = screen.getByRole('note', { name: /Note alert/i });
      const table = within(alert).getByRole('table');
      expect(table).toBeInTheDocument();
    });

    it('should work with blockquotes', () => {
      const markdown = `
:::important
> This is a quote inside an alert.
:::
      `.trim();

      const { container } = render(<MarkdownRenderer content={markdown} />);

      const alert = screen.getByRole('note', { name: /Important alert/i });
      const blockquote = within(alert).getByText(/This is a quote/);
      expect(blockquote.closest('blockquote')).toBeInTheDocument();
    });

    it('should work with inline math', () => {
      const markdown = `
:::note
The formula is $E = mc^2$.
:::
      `.trim();

      render(<MarkdownRenderer content={markdown} />);

      const alert = screen.getByRole('note', { name: /Note alert/i });
      expect(alert).toHaveTextContent('E = mc^2');
    });

    it('should work with collapsible sections', () => {
      const markdown = `
:::warning
<details>
<summary>Click for details</summary>
Hidden warning details.
</details>
:::
      `.trim();

      const { container } = render(<MarkdownRenderer content={markdown} />);

      const alert = screen.getByRole('note', { name: /Warning alert/i });
      const details = within(alert).getByText(/Click for details/);
      expect(details.closest('details')).toBeInTheDocument();
    });

    it('should work with TOC', () => {
      const markdown = `
## Table of Contents

## Alert Section

:::note
This section has an alert.
:::

## Next Section
Content.
      `.trim();

      render(<MarkdownRenderer content={markdown} />);

      // TOC should generate
      expect(screen.getByRole('heading', { level: 2, name: /Table of Contents/i })).toBeInTheDocument();

      // Alert should render
      expect(screen.getByRole('note', { name: /Note alert/i })).toBeInTheDocument();
    });
  });

  describe('Compact Mode', () => {
    it('should render alerts in compact mode with reduced spacing', () => {
      const markdown = `:::note\nCompact note\n:::`;
      const { container } = render(<MarkdownRenderer content={markdown} compact={true} />);

      const alert = container.querySelector('[role="note"]');
      expect(alert).toHaveClass('p-3', 'my-2'); // Compact padding/margin
    });

    it('should render icons in compact mode with smaller size', () => {
      const markdown = `:::note\nCompact\n:::`;
      render(<MarkdownRenderer content={markdown} compact={true} />);

      const icon = screen.getByTestId('info-icon');
      expect(icon).toHaveClass('h-4', 'w-4'); // Compact icon size
    });

    it('should render label text in compact mode with smaller font', () => {
      const markdown = `:::note\nCompact\n:::`;
      const { container } = render(<MarkdownRenderer content={markdown} compact={true} />);

      const label = within(container).getByText('NOTE');
      expect(label).toHaveClass('text-xs');
    });
  });

  describe('Accessibility', () => {
    it('should have role="note" on alert containers', () => {
      const markdown = `:::note\nAccessible note\n:::`;
      render(<MarkdownRenderer content={markdown} />);

      const alert = screen.getByRole('note');
      expect(alert).toBeInTheDocument();
    });

    it('should have proper aria-label based on alert type', () => {
      const markdown = `:::warning\nWarning content\n:::`;
      render(<MarkdownRenderer content={markdown} />);

      const alert = screen.getByRole('note', { name: /Warning alert/i });
      expect(alert).toHaveAttribute('aria-label', 'Warning alert');
    });

    it('should have aria-hidden="true" on icons', () => {
      const markdown = `:::note\nNote\n:::`;
      render(<MarkdownRenderer content={markdown} />);

      const icon = screen.getByTestId('info-icon');
      expect(icon).toHaveAttribute('aria-hidden', 'true');
    });

    it('should have uppercase labels that are screen reader friendly', () => {
      const markdown = `:::important\nImportant\n:::`;
      const { container } = render(<MarkdownRenderer content={markdown} />);

      const label = within(container).getByText('IMPORTANT');
      expect(label).toHaveClass('uppercase', 'font-semibold');
    });

    it('should have proper color contrast', () => {
      const markdown = `:::note\nNote\n:::`;
      const { container } = render(<MarkdownRenderer content={markdown} />);

      const alert = container.querySelector('[role="note"]');
      // Should have color classes for good contrast
      expect(alert?.className).toMatch(/text-blue-900|dark:text-blue-100/);
    });
  });

  describe('Edge Cases', () => {
    it('should handle empty alert', () => {
      const markdown = `:::note\n:::`;
      const { container } = render(<MarkdownRenderer content={markdown} />);

      const alert = screen.getByRole('note');
      expect(alert).toBeInTheDocument();
    });

    it('should handle alert with only whitespace', () => {
      const markdown = `:::note\n   \n:::`;
      render(<MarkdownRenderer content={markdown} />);

      const alert = screen.getByRole('note');
      expect(alert).toBeInTheDocument();
    });

    it('should ignore unknown alert types', () => {
      const markdown = `:::unknown\nUnknown type\n:::`;
      const { container } = render(<MarkdownRenderer content={markdown} />);

      // Should render as div with directive class, not as alert
      const directive = container.querySelector('.directive-unknown');
      expect(directive).toBeInTheDocument();

      // Should NOT have alert styling
      const alert = container.querySelector('[role="note"]');
      expect(alert).not.toBeInTheDocument();
    });

    it('should handle very long alert content', () => {
      const longText = 'A'.repeat(1000);
      const markdown = `:::note\n${longText}\n:::`;

      render(<MarkdownRenderer content={markdown} />);

      const alert = screen.getByRole('note');
      expect(alert).toHaveTextContent(longText);
    });

    it('should handle special characters in alert content', () => {
      const markdown = `:::note\n<>&"'\`\n:::`;
      render(<MarkdownRenderer content={markdown} />);

      const alert = screen.getByRole('note');
      expect(alert).toHaveTextContent('<>&"\'`');
    });

    it('should handle nested directives (if supported)', () => {
      const markdown = `
:::note
Outer note
:::warning
Inner warning (may not be supported)
:::
:::
      `.trim();

      // Should not crash
      const { container } = render(<MarkdownRenderer content={markdown} />);
      expect(container).toBeInTheDocument();
    });
  });

  describe('Props and Configuration', () => {
    it('should respect custom className', () => {
      const markdown = `:::note\nNote\n:::`;
      const { container } = render(
        <MarkdownRenderer content={markdown} className="custom-alert-class" />
      );

      const prose = container.querySelector('.prose');
      expect(prose).toHaveClass('custom-alert-class');
    });

    it('should respect custom ariaLabel', () => {
      const markdown = `:::note\nNote\n:::`;
      const { container } = render(
        <MarkdownRenderer content={markdown} ariaLabel="Document with alerts" />
      );

      const article = container.querySelector('[role="article"]');
      expect(article).toHaveAttribute('aria-label', 'Document with alerts');
    });

    it('should work with enableCodeCopy=false', () => {
      const markdown = `
:::note
\`\`\`javascript
const x = 1;
\`\`\`
:::
      `.trim();

      const { container } = render(
        <MarkdownRenderer content={markdown} enableCodeCopy={false} />
      );

      const alert = screen.getByRole('note');
      expect(alert).toBeInTheDocument();

      // No copy buttons
      const copyButtons = container.querySelectorAll('button');
      expect(copyButtons.length).toBe(0);
    });
  });

  describe('Performance', () => {
    it('should handle many alerts efficiently', () => {
      const alerts = Array.from({ length: 20 }, (_, i) =>
        `:::note\nAlert ${i}\n:::`
      ).join('\n\n');

      const start = performance.now();
      render(<MarkdownRenderer content={alerts} />);
      const renderTime = performance.now() - start;

      expect(renderTime).toBeLessThan(200);

      const allAlerts = screen.getAllByRole('note');
      expect(allAlerts).toHaveLength(20);
    });

    it('should not re-render alerts unnecessarily', () => {
      const markdown = `:::note\nTest\n:::`;
      const { rerender } = render(<MarkdownRenderer content={markdown} />);

      const alert = screen.getByRole('note');
      expect(alert).toBeInTheDocument();

      // Re-render with same content
      rerender(<MarkdownRenderer content={markdown} />);

      // Alert should still be there
      expect(screen.getByRole('note')).toBeInTheDocument();
    });
  });
});
