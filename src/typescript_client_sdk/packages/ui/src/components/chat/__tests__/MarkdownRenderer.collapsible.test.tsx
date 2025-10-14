/**
 * MarkdownRenderer Collapsible Sections Tests
 * 
 * Testing Card 1: Collapsible Sections Implementation
 * Tests HTML <details> and <summary> elements in markdown with:
 * - Basic expand/collapse functionality
 * - Chevron icon state changes (▶/▼)
 * - Markdown content inside collapsibles
 * - Nested collapsibles
 * - Keyboard navigation (Tab/Space/Enter)
 * - Accessibility (ARIA attributes, focus)
 * - Compact mode for thoughts
 * - Edge cases and error handling
 */

import React from 'react';
import { describe, it, expect, beforeEach, afterEach, vi } from 'vitest';
import { render, screen, within, waitFor } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
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
  )
}));

describe('MarkdownRenderer - Collapsible Sections', () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  afterEach(() => {
    vi.restoreAllMocks();
  });

  describe('Basic Collapsible Functionality', () => {
    it('should render a basic collapsible section', () => {
      const markdown = `
<details>
<summary>Click to expand</summary>

This content is hidden until you click the summary.

</details>
      `.trim();

      render(<MarkdownRenderer content={markdown} />);

      // Check for summary element
      const summary = screen.getByText((content, element) => {
        return element?.tagName.toLowerCase() === 'summary' && 
               element?.textContent?.includes('Click to expand') || false;
      });
      expect(summary).toBeInTheDocument();
    });

    it('should display closed chevron (▶) when collapsed', () => {
      const markdown = `
<details>
<summary>Collapsed Section</summary>
Hidden content
</details>
      `.trim();

      render(<MarkdownRenderer content={markdown} />);

      // Initially collapsed - should show right chevron
      const chevronRight = screen.getByTestId('chevron-right-icon');
      expect(chevronRight).toBeInTheDocument();
      expect(chevronRight).toHaveTextContent('▶');

      // Should not show down chevron when collapsed
      expect(screen.queryByTestId('chevron-down-icon')).not.toBeInTheDocument();
    });

    it('should display open chevron (▼) when expanded', async () => {
      const markdown = `
<details open>
<summary>Expanded Section</summary>
Visible content
</details>
      `.trim();

      render(<MarkdownRenderer content={markdown} />);

      // Initially expanded - should show down chevron
      await waitFor(() => {
        const chevronDown = screen.getByTestId('chevron-down-icon');
        expect(chevronDown).toBeInTheDocument();
        expect(chevronDown).toHaveTextContent('▼');
      });

      // Should not show right chevron when expanded
      expect(screen.queryByTestId('chevron-right-icon')).not.toBeInTheDocument();
    });

    it('should toggle content visibility on click', async () => {
      const user = userEvent.setup();
      const markdown = `
<details>
<summary>Toggle Me</summary>

Hidden content that appears on click.

</details>
      `.trim();

      const { container } = render(<MarkdownRenderer content={markdown} />);

      // Find the details element
      const details = container.querySelector('details');
      expect(details).toBeInTheDocument();

      // Initially closed
      expect(details).not.toHaveAttribute('open');

      // Find summary and click it
      const summary = container.querySelector('summary');
      expect(summary).toBeInTheDocument();
      
      await user.click(summary!);

      // Should be open after click
      await waitFor(() => {
        expect(details).toHaveAttribute('open');
      });

      // Click again to close
      await user.click(summary!);

      // Should be closed again
      await waitFor(() => {
        expect(details).not.toHaveAttribute('open');
      });
    });

    it('should have border and proper styling', () => {
      const markdown = `
<details>
<summary>Styled Section</summary>
Content
</details>
      `.trim();

      const { container } = render(<MarkdownRenderer content={markdown} />);

      const details = container.querySelector('details');
      expect(details).toHaveClass('border', 'border-border', 'rounded-md', 'group');
    });

    it('should have hover effects on summary', () => {
      const markdown = `
<details>
<summary>Hoverable</summary>
Content
</details>
      `.trim();

      const { container } = render(<MarkdownRenderer content={markdown} />);

      const summary = container.querySelector('summary');
      expect(summary).toHaveClass('hover:text-primary', 'transition-colors');
    });
  });

  describe('Chevron State Synchronization', () => {
    it('should update chevron when details opens', async () => {
      const user = userEvent.setup();
      const markdown = `
<details>
<summary>Test</summary>
Content
</details>
      `.trim();

      const { container } = render(<MarkdownRenderer content={markdown} />);

      // Initially closed - right chevron
      expect(screen.getByTestId('chevron-right-icon')).toBeInTheDocument();

      // Click to open
      const summary = container.querySelector('summary');
      await user.click(summary!);

      // Wait for state update and chevron change
      await waitFor(() => {
        expect(screen.queryByTestId('chevron-right-icon')).not.toBeInTheDocument();
        expect(screen.getByTestId('chevron-down-icon')).toBeInTheDocument();
      }, { timeout: 1000 });
    });

    it('should update chevron when details closes', async () => {
      const user = userEvent.setup();
      const markdown = `
<details open>
<summary>Test</summary>
Content
</details>
      `.trim();

      const { container } = render(<MarkdownRenderer content={markdown} />);

      // Initially open - down chevron
      await waitFor(() => {
        expect(screen.getByTestId('chevron-down-icon')).toBeInTheDocument();
      });

      // Click to close
      const summary = container.querySelector('summary');
      await user.click(summary!);

      // Wait for state update and chevron change
      await waitFor(() => {
        expect(screen.queryByTestId('chevron-down-icon')).not.toBeInTheDocument();
        expect(screen.getByTestId('chevron-right-icon')).toBeInTheDocument();
      }, { timeout: 1000 });
    });
  });

  describe('Markdown Content Inside Collapsibles', () => {
    it('should render headings inside collapsibles', () => {
      const markdown = `
<details>
<summary>With Heading</summary>

### Heading Inside

Regular text

</details>
      `.trim();

      render(<MarkdownRenderer content={markdown} />);

      const heading = screen.getByRole('heading', { level: 3 });
      expect(heading).toHaveTextContent('Heading Inside');
    });

    it('should render lists inside collapsibles', () => {
      const markdown = `
<details>
<summary>With List</summary>

- Item 1
- Item 2
- Item 3

</details>
      `.trim();

      const { container } = render(<MarkdownRenderer content={markdown} />);

      const list = container.querySelector('ul');
      expect(list).toBeInTheDocument();
      
      const items = container.querySelectorAll('li');
      expect(items).toHaveLength(3);
      expect(items[0]).toHaveTextContent('Item 1');
      expect(items[1]).toHaveTextContent('Item 2');
      expect(items[2]).toHaveTextContent('Item 3');
    });

    it('should render code blocks inside collapsibles', () => {
      const markdown = `
<details>
<summary>With Code</summary>

\`\`\`javascript
console.log("Hello");
\`\`\`

</details>
      `.trim();

      const { container } = render(<MarkdownRenderer content={markdown} />);

      const codeBlock = container.querySelector('pre code');
      expect(codeBlock).toBeInTheDocument();
      expect(codeBlock).toHaveTextContent('console.log("Hello");');
    });

    it('should render inline code inside collapsibles', () => {
      const markdown = `
<details>
<summary>With Inline Code</summary>

This has \`inline code\` in it.

</details>
      `.trim();

      const { container } = render(<MarkdownRenderer content={markdown} />);

      const inlineCode = container.querySelector('code');
      expect(inlineCode).toBeInTheDocument();
      expect(inlineCode).toHaveTextContent('inline code');
    });

    it('should render bold and italic text inside collapsibles', () => {
      const markdown = `
<details>
<summary>With Formatting</summary>

This is **bold** and this is *italic*.

</details>
      `.trim();

      const { container } = render(<MarkdownRenderer content={markdown} />);

      const strong = container.querySelector('strong');
      expect(strong).toBeInTheDocument();
      expect(strong).toHaveTextContent('bold');

      const em = container.querySelector('em');
      expect(em).toBeInTheDocument();
      expect(em).toHaveTextContent('italic');
    });

    it('should render links inside collapsibles', () => {
      const markdown = `
<details>
<summary>With Link</summary>

[Link text](https://example.com)

</details>
      `.trim();

      const { container } = render(<MarkdownRenderer content={markdown} />);

      const link = container.querySelector('a');
      expect(link).toBeInTheDocument();
      expect(link).toHaveTextContent('Link text');
      expect(link).toHaveAttribute('href', 'https://example.com');
      expect(link).toHaveAttribute('target', '_blank');
    });

    it('should render blockquotes inside collapsibles', () => {
      const markdown = `
<details>
<summary>With Quote</summary>

> This is a blockquote

</details>
      `.trim();

      const { container } = render(<MarkdownRenderer content={markdown} />);

      const blockquote = container.querySelector('blockquote');
      expect(blockquote).toBeInTheDocument();
      expect(blockquote).toHaveTextContent('This is a blockquote');
    });

    it('should render tables inside collapsibles', () => {
      const markdown = `
<details>
<summary>With Table</summary>

| Column 1 | Column 2 |
|----------|----------|
| Data 1   | Data 2   |

</details>
      `.trim();

      const { container } = render(<MarkdownRenderer content={markdown} />);

      const table = container.querySelector('table');
      expect(table).toBeInTheDocument();

      const headers = container.querySelectorAll('th');
      expect(headers).toHaveLength(2);
      expect(headers[0]).toHaveTextContent('Column 1');
      expect(headers[1]).toHaveTextContent('Column 2');

      const cells = container.querySelectorAll('td');
      expect(cells).toHaveLength(2);
      expect(cells[0]).toHaveTextContent('Data 1');
      expect(cells[1]).toHaveTextContent('Data 2');
    });
  });

  describe('Nested Collapsibles', () => {
    it('should render nested collapsible sections', () => {
      const markdown = `
<details>
<summary>Outer Section</summary>

Outer content

<details>
<summary>Inner Section</summary>

Inner content

</details>

More outer content

</details>
      `.trim();

      const { container } = render(<MarkdownRenderer content={markdown} />);

      // Should have 2 details elements
      const allDetails = container.querySelectorAll('details');
      expect(allDetails).toHaveLength(2);

      // Should have 2 summary elements
      const allSummaries = container.querySelectorAll('summary');
      expect(allSummaries).toHaveLength(2);
    });

    it('should allow independent operation of nested collapsibles', async () => {
      const user = userEvent.setup();
      const markdown = `
<details>
<summary>Outer</summary>

<details>
<summary>Inner</summary>
Content
</details>

</details>
      `.trim();

      const { container } = render(<MarkdownRenderer content={markdown} />);

      const [outerDetails, innerDetails] = Array.from(container.querySelectorAll('details'));
      const [outerSummary, innerSummary] = Array.from(container.querySelectorAll('summary'));

      // Both initially closed
      expect(outerDetails).not.toHaveAttribute('open');
      expect(innerDetails).not.toHaveAttribute('open');

      // Open outer
      await user.click(outerSummary);
      await waitFor(() => {
        expect(outerDetails).toHaveAttribute('open');
      });

      // Inner should still be closed
      expect(innerDetails).not.toHaveAttribute('open');

      // Open inner
      await user.click(innerSummary);
      await waitFor(() => {
        expect(innerDetails).toHaveAttribute('open');
      });

      // Outer should still be open
      expect(outerDetails).toHaveAttribute('open');

      // Close outer (should not affect inner state)
      await user.click(outerSummary);
      await waitFor(() => {
        expect(outerDetails).not.toHaveAttribute('open');
      });
    });

    it('should show correct chevron for each nested collapsible', async () => {
      const markdown = `
<details>
<summary>Outer</summary>

<details>
<summary>Inner</summary>
Content
</details>

</details>
      `.trim();

      render(<MarkdownRenderer content={markdown} />);

      // Both should have right chevrons initially (closed)
      const rightChevrons = screen.getAllByTestId('chevron-right-icon');
      expect(rightChevrons).toHaveLength(2);
    });
  });

  describe('Multiple Collapsibles', () => {
    it('should render multiple independent collapsibles', () => {
      const markdown = `
<details>
<summary>Section 1</summary>
Content 1
</details>

<details>
<summary>Section 2</summary>
Content 2
</details>

<details>
<summary>Section 3</summary>
Content 3
</details>
      `.trim();

      const { container } = render(<MarkdownRenderer content={markdown} />);

      // Should have 3 details elements
      const allDetails = container.querySelectorAll('details');
      expect(allDetails).toHaveLength(3);

      // Should have 3 summary elements
      const allSummaries = container.querySelectorAll('summary');
      expect(allSummaries).toHaveLength(3);
    });

    it('should allow independent operation of multiple collapsibles', async () => {
      const user = userEvent.setup();
      const markdown = `
<details>
<summary>First</summary>
Content 1
</details>

<details>
<summary>Second</summary>
Content 2
</details>

<details>
<summary>Third</summary>
Content 3
</details>
      `.trim();

      const { container } = render(<MarkdownRenderer content={markdown} />);

      const [first, second, third] = Array.from(container.querySelectorAll('details'));
      const [firstSummary, secondSummary, thirdSummary] = Array.from(container.querySelectorAll('summary'));

      // Open first
      await user.click(firstSummary);
      await waitFor(() => expect(first).toHaveAttribute('open'));

      // Others should remain closed
      expect(second).not.toHaveAttribute('open');
      expect(third).not.toHaveAttribute('open');

      // Open second
      await user.click(secondSummary);
      await waitFor(() => expect(second).toHaveAttribute('open'));

      // First should still be open, third still closed
      expect(first).toHaveAttribute('open');
      expect(third).not.toHaveAttribute('open');

      // Open third
      await user.click(thirdSummary);
      await waitFor(() => expect(third).toHaveAttribute('open'));

      // All should be open
      expect(first).toHaveAttribute('open');
      expect(second).toHaveAttribute('open');
      expect(third).toHaveAttribute('open');
    });
  });

  describe('Initially Open Collapsibles', () => {
    it('should respect open attribute', () => {
      const markdown = `
<details open>
<summary>Already Open</summary>
This content is visible immediately.
</details>
      `.trim();

      const { container } = render(<MarkdownRenderer content={markdown} />);

      const details = container.querySelector('details');
      expect(details).toHaveAttribute('open');
    });

    it('should show down chevron for initially open collapsibles', async () => {
      const markdown = `
<details open>
<summary>Already Open</summary>
Visible content
</details>
      `.trim();

      render(<MarkdownRenderer content={markdown} />);

      await waitFor(() => {
        const chevronDown = screen.getByTestId('chevron-down-icon');
        expect(chevronDown).toBeInTheDocument();
      });
    });

    it('should allow closing initially open collapsibles', async () => {
      const user = userEvent.setup();
      const markdown = `
<details open>
<summary>Initially Open</summary>
Content
</details>
      `.trim();

      const { container } = render(<MarkdownRenderer content={markdown} />);

      const details = container.querySelector('details');
      const summary = container.querySelector('summary');

      // Initially open
      expect(details).toHaveAttribute('open');

      // Click to close
      await user.click(summary!);

      await waitFor(() => {
        expect(details).not.toHaveAttribute('open');
      });
    });
  });

  describe('Keyboard Navigation', () => {
    it('should make summary focusable', () => {
      const markdown = `
<details>
<summary>Focusable</summary>
Content
</details>
      `.trim();

      const { container } = render(<MarkdownRenderer content={markdown} />);

      const summary = container.querySelector('summary');
      expect(summary).toHaveAttribute('tabIndex', '0');
    });

    it('should toggle on Enter key', async () => {
      const user = userEvent.setup();
      const markdown = `
<details>
<summary>Keyboard Test</summary>
Content
</details>
      `.trim();

      const { container } = render(<MarkdownRenderer content={markdown} />);

      const details = container.querySelector('details');
      const summary = container.querySelector('summary');

      // Focus and press Enter
      summary!.focus();
      await user.keyboard('{Enter}');

      await waitFor(() => {
        expect(details).toHaveAttribute('open');
      });
    });

    it('should toggle on Space key', async () => {
      const user = userEvent.setup();
      const markdown = `
<details>
<summary>Space Test</summary>
Content
</details>
      `.trim();

      const { container } = render(<MarkdownRenderer content={markdown} />);

      const details = container.querySelector('details');
      const summary = container.querySelector('summary');

      // Focus and press Space
      summary!.focus();
      await user.keyboard(' ');

      await waitFor(() => {
        expect(details).toHaveAttribute('open');
      });
    });

    it('should show focus ring when focused', () => {
      const markdown = `
<details>
<summary>Focus Test</summary>
Content
</details>
      `.trim();

      const { container } = render(<MarkdownRenderer content={markdown} />);

      const summary = container.querySelector('summary');
      expect(summary).toHaveClass('focus:outline-none', 'focus:ring-2', 'focus:ring-primary');
    });

    it('should not activate on other keys', async () => {
      const user = userEvent.setup();
      const markdown = `
<details>
<summary>Key Test</summary>
Content
</details>
      `.trim();

      const { container } = render(<MarkdownRenderer content={markdown} />);

      const details = container.querySelector('details');
      const summary = container.querySelector('summary');

      // Focus
      summary!.focus();

      // Press non-activation keys
      await user.keyboard('a');
      await user.keyboard('{Escape}');
      await user.keyboard('{Tab}');

      // Should remain closed
      expect(details).not.toHaveAttribute('open');
    });
  });

  describe('Accessibility', () => {
    it('should have role="button" on summary', () => {
      const markdown = `
<details>
<summary>Accessible</summary>
Content
</details>
      `.trim();

      const { container } = render(<MarkdownRenderer content={markdown} />);

      const summary = container.querySelector('summary');
      expect(summary).toHaveAttribute('role', 'button');
    });

    it('should have aria-expanded attribute', () => {
      const markdown = `
<details>
<summary>ARIA Test</summary>
Content
</details>
      `.trim();

      const { container } = render(<MarkdownRenderer content={markdown} />);

      const summary = container.querySelector('summary');
      expect(summary).toHaveAttribute('aria-expanded');
    });

    it('should update aria-expanded when toggled', async () => {
      const user = userEvent.setup();
      const markdown = `
<details>
<summary>ARIA Toggle</summary>
Content
</details>
      `.trim();

      const { container } = render(<MarkdownRenderer content={markdown} />);

      const summary = container.querySelector('summary');

      // Initially collapsed - aria-expanded should be false
      expect(summary).toHaveAttribute('aria-expanded', 'false');

      // Click to expand
      await user.click(summary!);

      // Should update to true
      await waitFor(() => {
        expect(summary).toHaveAttribute('aria-expanded', 'true');
      });
    });

    it('should have aria-hidden on chevron icons', () => {
      const markdown = `
<details>
<summary>Icon Test</summary>
Content
</details>
      `.trim();

      render(<MarkdownRenderer content={markdown} />);

      const chevron = screen.getByTestId('chevron-right-icon');
      expect(chevron).toHaveAttribute('aria-hidden', 'true');
    });

    it('should prevent text selection on summary', () => {
      const markdown = `
<details>
<summary>No Select</summary>
Content
</details>
      `.trim();

      const { container } = render(<MarkdownRenderer content={markdown} />);

      const summary = container.querySelector('summary');
      expect(summary).toHaveClass('select-none');
    });
  });

  describe('Compact Mode', () => {
    it('should apply compact styles when compact prop is true', () => {
      const markdown = `
<details>
<summary>Compact</summary>
Content
</details>
      `.trim();

      const { container } = render(<MarkdownRenderer content={markdown} compact={true} />);

      const details = container.querySelector('details');
      expect(details).toHaveClass('my-2', 'p-2');
    });

    it('should use smaller chevron icons in compact mode', () => {
      const markdown = `
<details>
<summary>Compact Chevron</summary>
Content
</details>
      `.trim();

      render(<MarkdownRenderer content={markdown} compact={true} />);

      const chevron = screen.getByTestId('chevron-right-icon');
      expect(chevron).toHaveClass('h-3', 'w-3');
    });

    it('should use smaller text in compact mode', () => {
      const markdown = `
<details>
<summary>Compact Text</summary>
Content
</details>
      `.trim();

      const { container } = render(<MarkdownRenderer content={markdown} compact={true} />);

      const summary = container.querySelector('summary');
      expect(summary).toHaveClass('text-sm', 'font-medium');
    });

    it('should use normal styles when compact prop is false', () => {
      const markdown = `
<details>
<summary>Normal</summary>
Content
</details>
      `.trim();

      const { container } = render(<MarkdownRenderer content={markdown} compact={false} />);

      const details = container.querySelector('details');
      expect(details).toHaveClass('my-3', 'p-3');

      const summary = container.querySelector('summary');
      expect(summary).toHaveClass('text-base', 'font-semibold');
    });
  });

  describe('Mixed Content', () => {
    it('should render collapsibles mixed with regular markdown', () => {
      const markdown = `
# Regular Heading

This is regular content.

<details>
<summary>Collapsible Section</summary>
Hidden content
</details>

More regular content after.

**Bold text** and \`inline code\`.
      `.trim();

      const { container } = render(<MarkdownRenderer content={markdown} />);

      // Check for regular heading
      const heading = screen.getByRole('heading', { level: 1 });
      expect(heading).toHaveTextContent('Regular Heading');

      // Check for collapsible
      const details = container.querySelector('details');
      expect(details).toBeInTheDocument();

      // Check for bold text
      const strong = container.querySelector('strong');
      expect(strong).toHaveTextContent('Bold text');

      // Check for inline code
      const code = container.querySelector('code');
      expect(code).toHaveTextContent('inline code');
    });

    it('should handle spacing correctly between collapsibles and other content', () => {
      const markdown = `
Regular paragraph.

<details>
<summary>Section</summary>
Content
</details>

Another paragraph.
      `.trim();

      const { container } = render(<MarkdownRenderer content={markdown} />);

      const details = container.querySelector('details');
      expect(details).toHaveClass('my-3');

      const paragraphs = container.querySelectorAll('p');
      expect(paragraphs.length).toBeGreaterThan(0);
    });
  });

  describe('Edge Cases', () => {
    it('should handle empty collapsible content', () => {
      const markdown = `
<details>
<summary>Empty</summary>
</details>
      `.trim();

      const { container } = render(<MarkdownRenderer content={markdown} />);

      const details = container.querySelector('details');
      expect(details).toBeInTheDocument();

      const summary = container.querySelector('summary');
      expect(summary).toBeInTheDocument();
    });

    it('should handle very long content', () => {
      const longContent = 'Lorem ipsum dolor sit amet. '.repeat(100);
      const markdown = `
<details>
<summary>Long Content</summary>

${longContent}

</details>
      `.trim();

      const { container } = render(<MarkdownRenderer content={markdown} />);

      const details = container.querySelector('details');
      expect(details).toBeInTheDocument();
      expect(details?.textContent).toContain('Lorem ipsum');
    });

    it('should handle special characters in summary', () => {
      const markdown = `
<details>
<summary>Special: @#$%^&*()[]{}|</summary>
Content
</details>
      `.trim();

      const { container } = render(<MarkdownRenderer content={markdown} />);

      const summary = container.querySelector('summary');
      expect(summary?.textContent).toContain('Special: @#$%^&*()[]{}|');
    });

    it('should handle multiple line breaks', () => {
      const markdown = `
<details>
<summary>Line Breaks</summary>

Content line 1


Content line 2


Content line 3

</details>
      `.trim();

      const { container } = render(<MarkdownRenderer content={markdown} />);

      const details = container.querySelector('details');
      expect(details).toBeInTheDocument();
    });

    it('should handle HTML entities in content', () => {
      const markdown = `
<details>
<summary>Entities Test</summary>

&lt;div&gt;HTML entities&lt;/div&gt;

</details>
      `.trim();

      render(<MarkdownRenderer content={markdown} />);

      // HTML entities should be rendered
      expect(screen.getByText((content) => content.includes('<div>'))).toBeInTheDocument();
    });
  });

  describe('Component Rendering', () => {
    it('should render within prose wrapper', () => {
      const markdown = `
<details>
<summary>Test</summary>
Content
</details>
      `.trim();

      const { container } = render(<MarkdownRenderer content={markdown} />);

      const prose = container.querySelector('.prose');
      expect(prose).toBeInTheDocument();

      const details = prose?.querySelector('details');
      expect(details).toBeInTheDocument();
    });

    it('should respect custom className', () => {
      const markdown = `
<details>
<summary>Test</summary>
Content
</details>
      `.trim();

      const { container } = render(
        <MarkdownRenderer content={markdown} className="custom-class" />
      );

      const prose = container.querySelector('.prose');
      expect(prose).toHaveClass('custom-class');
    });

    it('should respect custom ariaLabel', () => {
      const markdown = `
<details>
<summary>Test</summary>
Content
</details>
      `.trim();

      const { container } = render(
        <MarkdownRenderer content={markdown} ariaLabel="Custom label" />
      );

      const article = container.querySelector('[role="article"]');
      expect(article).toHaveAttribute('aria-label', 'Custom label');
    });
  });

  describe('Integration', () => {
    it('should work alongside code blocks with copy buttons', () => {
      const markdown = `
<details>
<summary>Code Example</summary>

\`\`\`javascript
const x = 1;
\`\`\`

</details>
      `.trim();

      const { container } = render(<MarkdownRenderer content={markdown} enableCodeCopy={true} />);

      // Should have collapsible
      const details = container.querySelector('details');
      expect(details).toBeInTheDocument();

      // Should have code block
      const codeBlock = container.querySelector('pre code');
      expect(codeBlock).toBeInTheDocument();

      // Should have copy button (in hover state)
      const copyButton = container.querySelector('button');
      expect(copyButton).toBeInTheDocument();
    });

    it('should not interfere with other markdown features', () => {
      const markdown = `
### Heading

<details>
<summary>Section</summary>

- List item
- Another item

</details>

**Bold** text and [link](https://example.com).
      `.trim();

      const { container } = render(<MarkdownRenderer content={markdown} />);

      // All features should work
      expect(screen.getByRole('heading', { level: 3 })).toBeInTheDocument();
      expect(container.querySelector('details')).toBeInTheDocument();
      expect(container.querySelector('ul')).toBeInTheDocument();
      expect(container.querySelector('strong')).toBeInTheDocument();
      expect(container.querySelector('a')).toBeInTheDocument();
    });
  });
});
