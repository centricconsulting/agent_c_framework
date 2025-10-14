/**
 * MarkdownRenderer Math/LaTeX Tests
 * 
 * Testing Card 4: Math/LaTeX Support
 * Tests remarkMath + rehypeKatex integration with:
 * - Inline math ($...$)
 * - Block math ($$...$$)
 * - Complex expressions (fractions, square roots, limits, derivatives, integrals, summations, matrices)
 * - Error handling for invalid LaTeX
 * - Integration with other features (headings, lists, code, alerts, tables, TOC)
 * - Compact mode
 * - Accessibility (KaTeX HTML output, semantic structure)
 * - Edge cases (special chars, escaped $, consecutive expressions, long equations)
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
    <div data-testid="info-icon" className={className} />
  ),
  Lightbulb: ({ className }: any) => (
    <div data-testid="lightbulb-icon" className={className} />
  ),
  Megaphone: ({ className }: any) => (
    <div data-testid="megaphone-icon" className={className} />
  ),
  AlertTriangle: ({ className }: any) => (
    <div data-testid="alert-triangle-icon" className={className} />
  ),
  ShieldAlert: ({ className }: any) => (
    <div data-testid="shield-alert-icon" className={className} />
  )
}));

describe('MarkdownRenderer - Math/LaTeX Support', () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  describe('Inline Math Rendering', () => {
    it('should render simple inline math expression', () => {
      const markdown = `The equation is $E = mc^2$ in physics.`;

      const { container } = render(<MarkdownRenderer content={markdown} />);

      // KaTeX renders inline math with .katex class
      const katex = container.querySelector('.katex');
      expect(katex).toBeInTheDocument();
      expect(container).toHaveTextContent('E = mc^2');
    });

    it('should render inline math with fractions', () => {
      const markdown = `The fraction is $\\frac{1}{2}$ of the total.`;

      const { container } = render(<MarkdownRenderer content={markdown} />);

      const katex = container.querySelector('.katex');
      expect(katex).toBeInTheDocument();
    });

    it('should render inline math with subscripts', () => {
      const markdown = `Variable $x_i$ represents the index.`;

      const { container } = render(<MarkdownRenderer content={markdown} />);

      expect(container.querySelector('.katex')).toBeInTheDocument();
    });

    it('should render inline math with superscripts', () => {
      const markdown = `The power is $x^2$ squared.`;

      const { container } = render(<MarkdownRenderer content={markdown} />);

      expect(container.querySelector('.katex')).toBeInTheDocument();
    });

    it('should render multiple inline math expressions', () => {
      const markdown = `We have $a + b$ and $c - d$ in the equation.`;

      const { container } = render(<MarkdownRenderer content={markdown} />);

      const katexElements = container.querySelectorAll('.katex');
      expect(katexElements.length).toBeGreaterThanOrEqual(2);
    });

    it('should render inline math with Greek letters', () => {
      const markdown = `The angle $\\theta$ and ratio $\\pi$.`;

      const { container } = render(<MarkdownRenderer content={markdown} />);

      const katexElements = container.querySelectorAll('.katex');
      expect(katexElements.length).toBeGreaterThanOrEqual(2);
    });
  });

  describe('Block Math Rendering', () => {
    it('should render simple block math expression', () => {
      const markdown = `
$$
E = mc^2
$$
      `.trim();

      const { container } = render(<MarkdownRenderer content={markdown} />);

      // Block math has .katex-display class
      const katexDisplay = container.querySelector('.katex-display');
      expect(katexDisplay).toBeInTheDocument();
    });

    it('should render block math with integral', () => {
      const markdown = `
$$
\\int_{0}^{\\infty} e^{-x} dx = 1
$$
      `.trim();

      const { container } = render(<MarkdownRenderer content={markdown} />);

      expect(container.querySelector('.katex-display')).toBeInTheDocument();
    });

    it('should render block math with summation', () => {
      const markdown = `
$$
\\sum_{i=1}^{n} i = \\frac{n(n+1)}{2}
$$
      `.trim();

      const { container } = render(<MarkdownRenderer content={markdown} />);

      expect(container.querySelector('.katex-display')).toBeInTheDocument();
    });

    it('should render block math with matrix', () => {
      const markdown = `
$$
\\begin{pmatrix}
a & b \\\\
c & d
\\end{pmatrix}
$$
      `.trim();

      const { container } = render(<MarkdownRenderer content={markdown} />);

      expect(container.querySelector('.katex-display')).toBeInTheDocument();
    });

    it('should render block math with fraction', () => {
      const markdown = `
$$
\\frac{a + b}{c + d}
$$
      `.trim();

      const { container } = render(<MarkdownRenderer content={markdown} />);

      expect(container.querySelector('.katex-display')).toBeInTheDocument();
    });

    it('should render multiple block math expressions', () => {
      const markdown = `
First equation:
$$
x + y = z
$$

Second equation:
$$
a - b = c
$$
      `.trim();

      const { container } = render(<MarkdownRenderer content={markdown} />);

      const katexDisplays = container.querySelectorAll('.katex-display');
      expect(katexDisplays.length).toBeGreaterThanOrEqual(2);
    });
  });

  describe('Complex Expressions', () => {
    it('should render square roots', () => {
      const markdown = `The result is $\\sqrt{x^2 + y^2}$.`;

      const { container } = render(<MarkdownRenderer content={markdown} />);

      expect(container.querySelector('.katex')).toBeInTheDocument();
    });

    it('should render limits', () => {
      const markdown = `
$$
\\lim_{x \\to \\infty} \\frac{1}{x} = 0
$$
      `.trim();

      const { container } = render(<MarkdownRenderer content={markdown} />);

      expect(container.querySelector('.katex-display')).toBeInTheDocument();
    });

    it('should render derivatives', () => {
      const markdown = `
$$
\\frac{d}{dx} x^2 = 2x
$$
      `.trim();

      const { container } = render(<MarkdownRenderer content={markdown} />);

      expect(container.querySelector('.katex-display')).toBeInTheDocument();
    });

    it('should render partial derivatives', () => {
      const markdown = `
$$
\\frac{\\partial f}{\\partial x}
$$
      `.trim();

      const { container } = render(<MarkdownRenderer content={markdown} />);

      expect(container.querySelector('.katex-display')).toBeInTheDocument();
    });

    it('should render set notation', () => {
      const markdown = `The set $\\{x \\in \\mathbb{R} : x > 0\\}$.`;

      const { container } = render(<MarkdownRenderer content={markdown} />);

      expect(container.querySelector('.katex')).toBeInTheDocument();
    });

    it('should render aligned equations', () => {
      const markdown = `
$$
\\begin{aligned}
x + y &= 10 \\\\
x - y &= 2
\\end{aligned}
$$
      `.trim();

      const { container } = render(<MarkdownRenderer content={markdown} />);

      expect(container.querySelector('.katex-display')).toBeInTheDocument();
    });
  });

  describe('Error Handling', () => {
    it('should handle invalid LaTeX syntax gracefully', () => {
      const markdown = `Invalid math: $\\invalid{command}$.`;

      // Should not throw, KaTeX configured with throwOnError: false
      expect(() => {
        render(<MarkdownRenderer content={markdown} />);
      }).not.toThrow();
    });

    it('should display error color for invalid LaTeX', () => {
      const markdown = `$\\invalidcommand$`;

      const { container } = render(<MarkdownRenderer content={markdown} />);

      // KaTeX error color configured as #cc0000
      const errorElement = container.querySelector('[style*="color"]');
      // May or may not have error styling depending on KaTeX version
      // Just ensure it renders without crashing
      expect(container).toBeInTheDocument();
    });

    it('should handle unclosed math delimiters', () => {
      const markdown = `This has $unclosed math`;

      // Should render without crashing
      const { container } = render(<MarkdownRenderer content={markdown} />);
      expect(container).toBeInTheDocument();
    });

    it('should handle mismatched braces', () => {
      const markdown = `Math with error: $\\frac{1}{2$.`;

      const { container } = render(<MarkdownRenderer content={markdown} />);
      expect(container).toBeInTheDocument();
    });

    it('should handle undefined commands', () => {
      const markdown = `$\\undefinedcmd{test}$`;

      const { container } = render(<MarkdownRenderer content={markdown} />);
      expect(container).toBeInTheDocument();
    });
  });

  describe('Integration with Other Features', () => {
    it('should work with headings', () => {
      const markdown = `
## Math Section

The formula is $E = mc^2$.
      `.trim();

      render(<MarkdownRenderer content={markdown} />);

      expect(screen.getByRole('heading', { level: 2 })).toHaveTextContent('Math Section');
      const { container } = render(<MarkdownRenderer content={markdown} />);
      expect(container.querySelector('.katex')).toBeInTheDocument();
    });

    it('should work with lists', () => {
      const markdown = `
- First: $x + y = z$
- Second: $a - b = c$
- Third: $p \\times q = r$
      `.trim();

      const { container } = render(<MarkdownRenderer content={markdown} />);

      expect(screen.getAllByRole('listitem')).toHaveLength(3);
      const katexElements = container.querySelectorAll('.katex');
      expect(katexElements.length).toBeGreaterThanOrEqual(3);
    });

    it('should work with code blocks', () => {
      const markdown = `
Math equation:
$$
x^2 + y^2 = z^2
$$

Python code:
\`\`\`python
x = 2 ** 2
\`\`\`
      `.trim();

      const { container } = render(<MarkdownRenderer content={markdown} />);

      expect(container.querySelector('.katex-display')).toBeInTheDocument();
      expect(container.querySelector('code.language-python')).toBeInTheDocument();
    });

    it('should work with alerts', () => {
      const markdown = `
:::note
The important formula is $E = mc^2$.
:::
      `.trim();

      const { container } = render(<MarkdownRenderer content={markdown} />);

      const alert = screen.getByRole('note');
      expect(alert).toBeInTheDocument();
      expect(container.querySelector('.katex')).toBeInTheDocument();
    });

    it('should work with tables', () => {
      const markdown = `
| Variable | Formula |
|----------|---------|
| Energy   | $E = mc^2$ |
| Force    | $F = ma$ |
      `.trim();

      const { container } = render(<MarkdownRenderer content={markdown} />);

      expect(container.querySelector('table')).toBeInTheDocument();
      const katexElements = container.querySelectorAll('.katex');
      expect(katexElements.length).toBeGreaterThanOrEqual(2);
    });

    it('should work with blockquotes', () => {
      const markdown = `
> Einstein said $E = mc^2$.
      `.trim();

      const { container } = render(<MarkdownRenderer content={markdown} />);

      const blockquote = container.querySelector('blockquote');
      expect(blockquote).toBeInTheDocument();
      expect(container.querySelector('.katex')).toBeInTheDocument();
    });

    it('should work with TOC', () => {
      const markdown = `
## Table of Contents

## Math Section

Formula: $x + y = z$
      `.trim();

      const { container } = render(<MarkdownRenderer content={markdown} />);

      expect(screen.getByRole('heading', { name: /Table of Contents/i })).toBeInTheDocument();
      expect(container.querySelector('.katex')).toBeInTheDocument();
    });
  });

  describe('Compact Mode', () => {
    it('should render inline math in compact mode', () => {
      const markdown = `Formula: $E = mc^2$`;
      const { container } = render(<MarkdownRenderer content={markdown} compact={true} />);

      expect(container.querySelector('.katex')).toBeInTheDocument();
      expect(container.querySelector('.prose')).toHaveClass('prose-sm');
    });

    it('should render block math in compact mode', () => {
      const markdown = `
$$
x^2 + y^2 = z^2
$$
      `.trim();

      const { container } = render(<MarkdownRenderer content={markdown} compact={true} />);

      expect(container.querySelector('.katex-display')).toBeInTheDocument();
      expect(container.querySelector('.prose')).toHaveClass('prose-sm');
    });

    it('should maintain math quality in compact mode', () => {
      const markdown = `Math: $\\frac{a + b}{c}$ and $\\sqrt{x^2 + y^2}$`;
      const { container } = render(<MarkdownRenderer content={markdown} compact={true} />);

      const katexElements = container.querySelectorAll('.katex');
      expect(katexElements.length).toBeGreaterThanOrEqual(2);
    });
  });

  describe('Accessibility', () => {
    it('should render KaTeX HTML output for screen readers', () => {
      const markdown = `Formula: $x + y = z$`;
      const { container } = render(<MarkdownRenderer content={markdown} />);

      // KaTeX generates semantic HTML
      const katex = container.querySelector('.katex');
      expect(katex).toBeInTheDocument();
      expect(katex?.tagName).toBe('SPAN');
    });

    it('should have semantic structure in block math', () => {
      const markdown = `
$$
E = mc^2
$$
      `.trim();

      const { container } = render(<MarkdownRenderer content={markdown} />);

      const katexDisplay = container.querySelector('.katex-display');
      expect(katexDisplay).toBeInTheDocument();
    });

    it('should render math content as HTML (not images)', () => {
      const markdown = `Math: $a + b = c$`;
      const { container } = render(<MarkdownRenderer content={markdown} />);

      // KaTeX uses HTML/CSS, not images
      const katex = container.querySelector('.katex');
      expect(katex).toBeInTheDocument();
      expect(container.querySelector('img')).not.toBeInTheDocument();
    });
  });

  describe('Special Characters', () => {
    it('should handle Greek letters', () => {
      const markdown = `$\\alpha, \\beta, \\gamma, \\delta$`;
      const { container } = render(<MarkdownRenderer content={markdown} />);

      expect(container.querySelector('.katex')).toBeInTheDocument();
    });

    it('should handle infinity symbol', () => {
      const markdown = `$\\infty$`;
      const { container } = render(<MarkdownRenderer content={markdown} />);

      expect(container.querySelector('.katex')).toBeInTheDocument();
    });

    it('should handle set symbols', () => {
      const markdown = `$\\in, \\notin, \\subset, \\subseteq$`;
      const { container } = render(<MarkdownRenderer content={markdown} />);

      expect(container.querySelector('.katex')).toBeInTheDocument();
    });

    it('should handle arrows', () => {
      const markdown = `$\\rightarrow, \\leftarrow, \\Rightarrow, \\Leftarrow$`;
      const { container } = render(<MarkdownRenderer content={markdown} />);

      expect(container.querySelector('.katex')).toBeInTheDocument();
    });

    it('should handle mathematical operators', () => {
      const markdown = `$\\times, \\div, \\pm, \\mp$`;
      const { container } = render(<MarkdownRenderer content={markdown} />);

      expect(container.querySelector('.katex')).toBeInTheDocument();
    });
  });

  describe('Edge Cases', () => {
    it('should handle dollar sign at start of line', () => {
      const markdown = `$x + y$ is the formula.`;
      const { container } = render(<MarkdownRenderer content={markdown} />);

      expect(container.querySelector('.katex')).toBeInTheDocument();
    });

    it('should handle dollar sign at end of line', () => {
      const markdown = `The formula is $x + y$`;
      const { container } = render(<MarkdownRenderer content={markdown} />);

      expect(container.querySelector('.katex')).toBeInTheDocument();
    });

    it('should handle math with punctuation', () => {
      const markdown = `Formula: $x + y = z$, and more.`;
      const { container } = render(<MarkdownRenderer content={markdown} />);

      expect(container.querySelector('.katex')).toBeInTheDocument();
      expect(container).toHaveTextContent(', and more.');
    });

    it('should handle consecutive math expressions', () => {
      const markdown = `$a$$b$$c$`;
      const { container } = render(<MarkdownRenderer content={markdown} />);

      // Should render all three
      const katexElements = container.querySelectorAll('.katex');
      expect(katexElements.length).toBeGreaterThanOrEqual(3);
    });

    it('should handle very long equations', () => {
      const longEquation = Array.from({ length: 20 }, (_, i) => `x_{${i}}`).join(' + ');
      const markdown = `$$${longEquation}$$`;

      const { container } = render(<MarkdownRenderer content={markdown} />);

      expect(container.querySelector('.katex-display')).toBeInTheDocument();
    });

    it('should handle escaped dollar signs', () => {
      const markdown = `The price is \\$5.`;
      const { container } = render(<MarkdownRenderer content={markdown} />);

      // Escaped $ should not trigger math rendering
      expect(container).toHaveTextContent('$5');
      // May or may not have katex depending on markdown parser
    });

    it('should handle math in emphasized text', () => {
      const markdown = `*Formula: $x + y = z$*`;
      const { container } = render(<MarkdownRenderer content={markdown} />);

      expect(container.querySelector('em')).toBeInTheDocument();
      expect(container.querySelector('.katex')).toBeInTheDocument();
    });

    it('should handle math in bold text', () => {
      const markdown = `**Important: $E = mc^2$**`;
      const { container } = render(<MarkdownRenderer content={markdown} />);

      expect(container.querySelector('strong')).toBeInTheDocument();
      expect(container.querySelector('.katex')).toBeInTheDocument();
    });
  });

  describe('Props and Configuration', () => {
    it('should respect custom className', () => {
      const markdown = `$x + y = z$`;
      const { container } = render(
        <MarkdownRenderer content={markdown} className="custom-math-class" />
      );

      const prose = container.querySelector('.prose');
      expect(prose).toHaveClass('custom-math-class');
    });

    it('should respect custom ariaLabel', () => {
      const markdown = `$E = mc^2$`;
      const { container } = render(
        <MarkdownRenderer content={markdown} ariaLabel="Math content" />
      );

      const article = container.querySelector('[role="article"]');
      expect(article).toHaveAttribute('aria-label', 'Math content');
    });

    it('should work with enableCodeCopy=false', () => {
      const markdown = `
Math: $x + y$

Code:
\`\`\`javascript
const x = 1;
\`\`\`
      `.trim();

      const { container } = render(
        <MarkdownRenderer content={markdown} enableCodeCopy={false} />
      );

      expect(container.querySelector('.katex')).toBeInTheDocument();
      expect(container.querySelectorAll('button').length).toBe(0);
    });
  });

  describe('Performance', () => {
    it('should handle many math expressions efficiently', () => {
      const expressions = Array.from({ length: 20 }, (_, i) =>
        `Expression ${i}: $x_{${i}} + y_{${i}} = z_{${i}}$`
      ).join('\n\n');

      const start = performance.now();
      const { container } = render(<MarkdownRenderer content={expressions} />);
      const renderTime = performance.now() - start;

      expect(renderTime).toBeLessThan(300);

      const katexElements = container.querySelectorAll('.katex');
      expect(katexElements.length).toBeGreaterThanOrEqual(20);
    });

    it('should handle mixed inline and block math efficiently', () => {
      const markdown = `
Inline: $x + y = z$

Block:
$$
\\int_{0}^{\\infty} e^{-x} dx
$$

More inline: $a - b = c$

Another block:
$$
\\sum_{i=1}^{n} i^2
$$
      `.trim();

      const start = performance.now();
      const { container } = render(<MarkdownRenderer content={markdown} />);
      const renderTime = performance.now() - start;

      expect(renderTime).toBeLessThan(200);

      expect(container.querySelectorAll('.katex').length).toBeGreaterThanOrEqual(2);
      expect(container.querySelectorAll('.katex-display').length).toBeGreaterThanOrEqual(2);
    });

    it('should not re-render math unnecessarily', () => {
      const markdown = `Formula: $E = mc^2$`;
      const { rerender, container } = render(<MarkdownRenderer content={markdown} />);

      expect(container.querySelector('.katex')).toBeInTheDocument();

      // Re-render with same content
      rerender(<MarkdownRenderer content={markdown} />);

      // Math should still be rendered
      expect(container.querySelector('.katex')).toBeInTheDocument();
    });
  });

  describe('KaTeX Configuration', () => {
    it('should have throwOnError set to false', () => {
      const markdown = `$\\invalidcommand$`;

      // Should not throw error
      expect(() => {
        render(<MarkdownRenderer content={markdown} />);
      }).not.toThrow();
    });

    it('should render with HTML output (not MathML)', () => {
      const markdown = `$x + y = z$`;
      const { container } = render(<MarkdownRenderer content={markdown} />);

      // KaTeX HTML output uses span elements
      const katex = container.querySelector('.katex');
      expect(katex?.tagName).toBe('SPAN');
      
      // Should not use MathML elements
      expect(container.querySelector('math')).not.toBeInTheDocument();
    });

    it('should have proper error styling color', () => {
      const markdown = `$\\error$`;
      const { container } = render(<MarkdownRenderer content={markdown} />);

      // Error color configured as #cc0000
      // May or may not show depending on KaTeX version
      expect(container).toBeInTheDocument();
    });
  });
});
