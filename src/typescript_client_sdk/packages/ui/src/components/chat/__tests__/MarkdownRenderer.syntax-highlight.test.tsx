/**
 * MarkdownRenderer Syntax Highlighting Tests
 * 
 * Testing Card 2: Syntax Highlighting for Code Blocks
 * Tests rehype-highlight integration with:
 * - Basic syntax highlighting for common languages
 * - Copy button functionality preserved
 * - Compact mode with highlighting
 * - Multiple languages in one message
 * - No highlighting for unknown languages (expected)
 * - No highlighting for inline code (expected)
 * - Performance with large code blocks
 * - Accessibility maintained
 * - Integration with existing features
 */

import React from 'react';
import { describe, it, expect, beforeEach, afterEach, vi } from 'vitest';
import { render, screen, within, waitFor, act } from '@testing-library/react';
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

// Mock clipboard API - must return a resolved Promise
const mockWriteText = vi.fn().mockImplementation(() => Promise.resolve());

Object.defineProperty(navigator, 'clipboard', {
  writable: true,
  value: {
    writeText: mockWriteText
  }
});

describe('MarkdownRenderer - Syntax Highlighting', () => {
  beforeEach(() => {
    vi.clearAllMocks();
    mockWriteText.mockClear();
    mockWriteText.mockImplementation(() => Promise.resolve());
  });

  afterEach(() => {
    vi.restoreAllMocks();
  });

  describe('Basic Syntax Highlighting', () => {
    it('should render JavaScript code block with syntax classes', () => {
      const markdown = `
\`\`\`javascript
function fibonacci(n) {
  if (n <= 1) return n;
  return fibonacci(n - 1) + fibonacci(n - 2);
}
\`\`\`
      `.trim();

      const { container } = render(<MarkdownRenderer content={markdown} />);

      // Code block should exist
      const codeBlock = container.querySelector('pre code');
      expect(codeBlock).toBeInTheDocument();

      // Should contain the function code
      expect(codeBlock?.textContent).toContain('function fibonacci');
      expect(codeBlock?.textContent).toContain('if (n <= 1) return n');

      // Should have language class
      expect(codeBlock?.className).toContain('language-javascript');
    });

    it('should render Python code block with syntax classes', () => {
      const markdown = `
\`\`\`python
def quick_sort(arr):
    if len(arr) <= 1:
        return arr
    pivot = arr[len(arr) // 2]
    return quick_sort(left) + middle + quick_sort(right)
\`\`\`
      `.trim();

      const { container } = render(<MarkdownRenderer content={markdown} />);

      const codeBlock = container.querySelector('pre code');
      expect(codeBlock).toBeInTheDocument();
      expect(codeBlock?.textContent).toContain('def quick_sort');
      expect(codeBlock?.className).toContain('language-python');
    });

    it('should render TypeScript code block with syntax classes', () => {
      const markdown = `
\`\`\`typescript
interface User {
  id: string;
  name: string;
}

const fetchUser = async (id: string): Promise<User> => {
  return await fetch(\`/api/users/\${id}\`);
}
\`\`\`
      `.trim();

      const { container } = render(<MarkdownRenderer content={markdown} />);

      const codeBlock = container.querySelector('pre code');
      expect(codeBlock).toBeInTheDocument();
      expect(codeBlock?.textContent).toContain('interface User');
      expect(codeBlock?.textContent).toContain('Promise<User>');
      expect(codeBlock?.className).toContain('language-typescript');
    });

    it('should render Bash/Shell code block with syntax classes', () => {
      const markdown = `
\`\`\`bash
#!/bin/bash
echo "Hello, World!"
cd /usr/local
ls -la
\`\`\`
      `.trim();

      const { container } = render(<MarkdownRenderer content={markdown} />);

      const codeBlock = container.querySelector('pre code');
      expect(codeBlock).toBeInTheDocument();
      expect(codeBlock?.textContent).toContain('#!/bin/bash');
      expect(codeBlock?.className).toContain('language-bash');
    });

    it('should render JSON code block with syntax classes', () => {
      const markdown = `
\`\`\`json
{
  "name": "test",
  "version": "1.0.0",
  "dependencies": {
    "react": "^18.0.0"
  }
}
\`\`\`
      `.trim();

      const { container } = render(<MarkdownRenderer content={markdown} />);

      const codeBlock = container.querySelector('pre code');
      expect(codeBlock).toBeInTheDocument();
      expect(codeBlock?.textContent).toContain('"name": "test"');
      expect(codeBlock?.className).toContain('language-json');
    });

    it('should render SQL code block with syntax classes', () => {
      const markdown = `
\`\`\`sql
SELECT users.name, orders.total
FROM users
INNER JOIN orders ON users.id = orders.user_id
WHERE orders.status = 'completed';
\`\`\`
      `.trim();

      const { container } = render(<MarkdownRenderer content={markdown} />);

      const codeBlock = container.querySelector('pre code');
      expect(codeBlock).toBeInTheDocument();
      expect(codeBlock?.textContent).toContain('SELECT');
      expect(codeBlock?.className).toContain('language-sql');
    });
  });

  describe('Copy Button Functionality with Highlighting', () => {
    it('should display copy button on code block with syntax highlighting', () => {
      const markdown = `
\`\`\`javascript
const x = 1;
console.log(x);
\`\`\`
      `.trim();

      const { container } = render(<MarkdownRenderer content={markdown} enableCodeCopy={true} />);

      // Copy button should exist
      const copyButton = container.querySelector('button');
      expect(copyButton).toBeInTheDocument();
      expect(copyButton).toHaveTextContent('Copy');
    });

    it('should copy plain code without syntax classes when clicked', async () => {
      const user = userEvent.setup();
      const markdown = `
\`\`\`javascript
function test() {
  return 42;
}
\`\`\`
      `.trim();

      const { container } = render(<MarkdownRenderer content={markdown} enableCodeCopy={true} />);

      const copyButton = container.querySelector('button');
      expect(copyButton).toBeInTheDocument();

      // Click and wait for async operations
      await act(async () => {
        await user.click(copyButton!);
        // Wait for promise to resolve
        await new Promise(resolve => setTimeout(resolve, 50));
      });

      // Should copy plain text (no HTML/classes)
      await waitFor(() => {
        expect(mockWriteText).toHaveBeenCalled();
      }, { timeout: 1000 });
      
      // Check the actual call
      const calls = mockWriteText.mock.calls;
      expect(calls.length).toBeGreaterThan(0);
      expect(calls[0][0]).toContain('function test()');
      expect(calls[0][0]).toContain('return 42;');
    });

    it('should show "Copied" confirmation after copying highlighted code', async () => {
      const user = userEvent.setup();
      const markdown = `
\`\`\`python
def hello():
    print("world")
\`\`\`
      `.trim();

      const { container } = render(<MarkdownRenderer content={markdown} enableCodeCopy={true} />);

      const copyButton = container.querySelector('button');
      expect(copyButton).toHaveTextContent('Copy');
      
      // Click and wait for async state updates
      await act(async () => {
        await user.click(copyButton!);
        // Wait for clipboard promise and state update
        await new Promise(resolve => setTimeout(resolve, 100));
      });

      // Should show "Copied" with checkmark after click
      await waitFor(() => {
        expect(copyButton).toHaveTextContent('Copied');
      }, { timeout: 1000 });
      
      // Check for checkmark icon
      expect(screen.getByTestId('check-icon')).toBeInTheDocument();
    });

    it('should revert to "Copy" after 2 seconds', async () => {
      // Use real timers for this test to avoid timing conflicts
      const markdown = `
\`\`\`typescript
const greeting: string = "hello";
\`\`\`
      `.trim();

      const { container } = render(<MarkdownRenderer content={markdown} enableCodeCopy={true} />);

      const copyButton = container.querySelector('button');
      expect(copyButton).toHaveTextContent('Copy');
      
      // Click the button with act() wrapper
      await act(async () => {
        await userEvent.click(copyButton!);
        await new Promise(resolve => setTimeout(resolve, 100));
      });

      // Should change to "Copied"
      await waitFor(() => {
        expect(copyButton).toHaveTextContent('Copied');
      }, { timeout: 1000 });

      // Wait for 2 seconds + a bit for the timeout to fire
      await act(async () => {
        await new Promise(resolve => setTimeout(resolve, 2100));
      });

      // Should revert to "Copy"
      expect(copyButton).toHaveTextContent('Copy');
      expect(screen.getByTestId('copy-icon')).toBeInTheDocument();
    });

    it('should have proper ARIA label with language name', () => {
      const markdown = `
\`\`\`javascript
const x = 1;
\`\`\`
      `.trim();

      const { container } = render(<MarkdownRenderer content={markdown} enableCodeCopy={true} />);

      const copyButton = container.querySelector('button');
      expect(copyButton).toHaveAttribute('aria-label', 'Copy javascript code');
    });
  });

  describe('Compact Mode with Highlighting', () => {
    it('should render syntax highlighting in compact mode', () => {
      const markdown = `
\`\`\`javascript
function compact() {
  return true;
}
\`\`\`
      `.trim();

      const { container } = render(<MarkdownRenderer content={markdown} compact={true} />);

      const codeBlock = container.querySelector('pre code');
      expect(codeBlock).toBeInTheDocument();
      expect(codeBlock?.className).toContain('language-javascript');
      expect(codeBlock).toHaveClass('text-xs'); // Compact size
    });

    it('should display smaller copy button in compact mode', () => {
      const markdown = `
\`\`\`python
def compact():
    return True
\`\`\`
      `.trim();

      const { container } = render(
        <MarkdownRenderer content={markdown} compact={true} enableCodeCopy={true} />
      );

      const copyButton = container.querySelector('button');
      expect(copyButton).toHaveClass('h-6'); // Compact height
    });

    it('should have tighter spacing in compact mode', () => {
      const markdown = `
\`\`\`typescript
const x: number = 1;
\`\`\`
      `.trim();

      const { container } = render(<MarkdownRenderer content={markdown} compact={true} />);

      const wrapper = container.querySelector('div.relative.group');
      expect(wrapper).toHaveClass('my-2'); // Compact margin

      const pre = container.querySelector('pre');
      expect(pre).toHaveClass('p-2'); // Compact padding
    });
  });

  describe('No Language Specified', () => {
    it('should render code block without syntax highlighting when no language', () => {
      const markdown = `
\`\`\`
const x = 1;
console.log(x);
\`\`\`
      `.trim();

      const { container } = render(<MarkdownRenderer content={markdown} />);

      const codeBlock = container.querySelector('pre code');
      expect(codeBlock).toBeInTheDocument();
      expect(codeBlock?.textContent).toContain('const x = 1');

      // Should not have language-specific class (only generic classes)
      const className = codeBlock?.className || '';
      expect(className).not.toContain('language-javascript');
      expect(className).not.toContain('language-python');
    });

    it('should show copy button even without language', () => {
      const markdown = `
\`\`\`
This has no highlighting
But copy button still works
\`\`\`
      `.trim();

      const { container } = render(<MarkdownRenderer content={markdown} enableCodeCopy={true} />);

      // Fixed: Copy button now shows regardless of language presence
      // Only enableCodeCopy controls visibility
      const copyButton = container.querySelector('button');
      expect(copyButton).toBeInTheDocument();
      expect(copyButton).toHaveTextContent('Copy');
      
      // Should have generic aria-label without language
      expect(copyButton).toHaveAttribute('aria-label', 'Copy code');
    });

    it('should render properly without errors', () => {
      const markdown = `
\`\`\`
Code without language
Should render fine
No highlighting
\`\`\`
      `.trim();

      const { container } = render(<MarkdownRenderer content={markdown} />);

      const pre = container.querySelector('pre');
      expect(pre).toBeInTheDocument();
      expect(pre?.textContent).toContain('Code without language');
    });
  });

  describe('Inline Code NOT Highlighted', () => {
    it('should NOT apply syntax highlighting to inline code', () => {
      const markdown = `Here is some \`inline code\` in a sentence.`;

      const { container } = render(<MarkdownRenderer content={markdown} />);

      const inlineCode = container.querySelector('code');
      expect(inlineCode).toBeInTheDocument();
      expect(inlineCode?.textContent).toBe('inline code');

      // Should have simple gray background classes, not language classes
      expect(inlineCode).toHaveClass('bg-muted', 'rounded', 'font-mono');
      expect(inlineCode?.className).not.toContain('language-');
    });

    it('should NOT highlight inline code with JavaScript syntax', () => {
      const markdown = `The function \`const x = 1;\` is simple.`;

      const { container } = render(<MarkdownRenderer content={markdown} />);

      const inlineCode = container.querySelector('code');
      expect(inlineCode).toBeInTheDocument();
      expect(inlineCode?.textContent).toBe('const x = 1;');

      // No syntax highlighting classes
      expect(inlineCode?.className).not.toContain('language-');
    });

    it('should render multiple inline codes without highlighting', () => {
      const markdown = `Use \`const\` or \`let\` but never \`var\` in modern code.`;

      const { container } = render(<MarkdownRenderer content={markdown} />);

      const inlineCodes = container.querySelectorAll('code');
      expect(inlineCodes).toHaveLength(3);

      inlineCodes.forEach(code => {
        expect(code).toHaveClass('bg-muted');
        expect(code.className).not.toContain('language-');
      });
    });
  });

  describe('Multiple Languages in Same Message', () => {
    it('should highlight each code block according to its language', () => {
      const markdown = `
Here's JavaScript:

\`\`\`javascript
function hello() {
  return "world";
}
\`\`\`

Here's Python:

\`\`\`python
def hello():
    return "world"
\`\`\`

Here's TypeScript:

\`\`\`typescript
const hello = (): string => {
  return "world";
}
\`\`\`
      `.trim();

      const { container } = render(<MarkdownRenderer content={markdown} />);

      const codeBlocks = container.querySelectorAll('pre code');
      expect(codeBlocks).toHaveLength(3);

      // First is JavaScript
      expect(codeBlocks[0]?.className).toContain('language-javascript');
      expect(codeBlocks[0]?.textContent).toContain('function hello');

      // Second is Python
      expect(codeBlocks[1]?.className).toContain('language-python');
      expect(codeBlocks[1]?.textContent).toContain('def hello');

      // Third is TypeScript
      expect(codeBlocks[2]?.className).toContain('language-typescript');
      expect(codeBlocks[2]?.textContent).toContain('const hello');
    });

    it('should provide copy button for each code block independently', () => {
      const markdown = `
\`\`\`javascript
const js = 1;
\`\`\`

\`\`\`python
py = 1
\`\`\`
      `.trim();

      const { container } = render(<MarkdownRenderer content={markdown} enableCodeCopy={true} />);

      const copyButtons = container.querySelectorAll('button');
      expect(copyButtons).toHaveLength(2);

      // Each button should have correct aria-label
      expect(copyButtons[0]).toHaveAttribute('aria-label', 'Copy javascript code');
      expect(copyButtons[1]).toHaveAttribute('aria-label', 'Copy python code');
    });

    it('should not interfere between different language blocks', async () => {
      const markdown = `
\`\`\`javascript
const x = 1;
\`\`\`

\`\`\`python
y = 2
\`\`\`
      `.trim();

      const { container } = render(<MarkdownRenderer content={markdown} enableCodeCopy={true} />);

      const copyButtons = container.querySelectorAll('button');
      expect(copyButtons.length).toBe(2);

      // Copy from first block (JavaScript)
      await act(async () => {
        await userEvent.click(copyButtons[0]);
        await new Promise(resolve => setTimeout(resolve, 100));
      });

      await waitFor(() => {
        expect(mockWriteText).toHaveBeenCalled();
      }, { timeout: 1000 });
      
      const firstCall = mockWriteText.mock.calls[0][0];
      expect(firstCall).toContain('const x = 1');

      mockWriteText.mockClear();

      // Copy from second block (Python)
      await act(async () => {
        await userEvent.click(copyButtons[1]);
        await new Promise(resolve => setTimeout(resolve, 100));
      });

      await waitFor(() => {
        expect(mockWriteText).toHaveBeenCalled();
      }, { timeout: 1000 });
      
      const secondCall = mockWriteText.mock.calls[0][0];
      expect(secondCall).toContain('y = 2');
    });
  });

  describe('Common Languages Support', () => {
    const testLanguages = [
      { lang: 'javascript', code: 'const x = 1;' },
      { lang: 'typescript', code: 'const x: number = 1;' },
      { lang: 'python', code: 'x = 1' },
      { lang: 'bash', code: 'echo "hello"' },
      { lang: 'json', code: '{"key": "value"}' },
      { lang: 'sql', code: 'SELECT * FROM users;' },
      { lang: 'ruby', code: 'x = 1' },
      { lang: 'go', code: 'var x int = 1' },
      { lang: 'rust', code: 'let x: i32 = 1;' },
      { lang: 'yaml', code: 'key: value' }
    ];

    testLanguages.forEach(({ lang, code }) => {
      it(`should support ${lang} syntax highlighting`, () => {
        const markdown = `\`\`\`${lang}\n${code}\n\`\`\``;

        const { container } = render(<MarkdownRenderer content={markdown} />);

        const codeBlock = container.querySelector('pre code');
        expect(codeBlock).toBeInTheDocument();
        expect(codeBlock?.className).toContain(`language-${lang}`);
        expect(codeBlock?.textContent).toContain(code);
      });
    });
  });

  describe('Edge Cases', () => {
    it('should handle empty code block with language', () => {
      const markdown = `\`\`\`javascript\n\`\`\``;

      const { container } = render(<MarkdownRenderer content={markdown} />);

      const codeBlock = container.querySelector('pre code');
      expect(codeBlock).toBeInTheDocument();
      expect(codeBlock?.className).toContain('language-javascript');
    });

    it('should handle very long code blocks', () => {
      const longCode = 'console.log("line");\n'.repeat(100);
      const markdown = `\`\`\`javascript\n${longCode}\`\`\``;

      const { container } = render(<MarkdownRenderer content={markdown} />);

      const codeBlock = container.querySelector('pre code');
      expect(codeBlock).toBeInTheDocument();
      expect(codeBlock?.textContent).toContain('console.log("line")');
      expect(codeBlock?.textContent?.split('\n').length).toBeGreaterThan(50);
    });

    it('should handle special characters in code', () => {
      const markdown = `
\`\`\`javascript
const special = "<>&'\"@#$%^&*()[]{}|\\";
\`\`\`
      `.trim();

      const { container } = render(<MarkdownRenderer content={markdown} />);

      const codeBlock = container.querySelector('pre code');
      expect(codeBlock).toBeInTheDocument();
      // Special characters should be preserved
      expect(codeBlock?.textContent).toContain('<>&');
    });

    it('should handle invalid/unknown language names', () => {
      const markdown = `
\`\`\`unknownlang123
Some code here
\`\`\`
      `.trim();

      const { container } = render(<MarkdownRenderer content={markdown} />);

      const codeBlock = container.querySelector('pre code');
      expect(codeBlock).toBeInTheDocument();
      expect(codeBlock?.textContent).toContain('Some code here');
      
      // Should have language class even if unknown
      expect(codeBlock?.className).toContain('language-unknownlang123');
    });

    it('should handle code with multiple blank lines', () => {
      const markdown = `
\`\`\`javascript
function test() {


  return true;


}
\`\`\`
      `.trim();

      const { container } = render(<MarkdownRenderer content={markdown} />);

      const codeBlock = container.querySelector('pre code');
      expect(codeBlock).toBeInTheDocument();
      expect(codeBlock?.textContent).toContain('function test');
    });
  });

  describe('Accessibility with Highlighting', () => {
    it('should maintain aria-label on pre element', () => {
      const markdown = `
\`\`\`javascript
const x = 1;
\`\`\`
      `.trim();

      const { container } = render(<MarkdownRenderer content={markdown} />);

      const pre = container.querySelector('pre');
      expect(pre).toHaveAttribute('aria-label', 'Code block in javascript');
    });

    it('should maintain semantic structure', () => {
      const markdown = `
\`\`\`python
def test():
    pass
\`\`\`
      `.trim();

      const { container } = render(<MarkdownRenderer content={markdown} />);

      // Should have proper nesting: div > pre > code
      const wrapper = container.querySelector('div.relative.group');
      expect(wrapper).toBeInTheDocument();

      const pre = wrapper?.querySelector('pre');
      expect(pre).toBeInTheDocument();

      const code = pre?.querySelector('code');
      expect(code).toBeInTheDocument();
    });

    it('should maintain keyboard accessibility for copy button', () => {
      const markdown = `
\`\`\`typescript
const x: number = 1;
\`\`\`
      `.trim();

      const { container } = render(<MarkdownRenderer content={markdown} enableCodeCopy={true} />);

      const copyButton = container.querySelector('button');
      expect(copyButton).toBeInTheDocument();
      
      // Button should be focusable (no tabIndex=-1)
      expect(copyButton).not.toHaveAttribute('tabIndex', '-1');
    });
  });

  describe('Integration with Other Features', () => {
    it('should work alongside collapsible sections', () => {
      const markdown = `
<details>
<summary>Code Example</summary>

\`\`\`javascript
function example() {
  return 42;
}
\`\`\`

</details>
      `.trim();

      const { container } = render(<MarkdownRenderer content={markdown} />);

      // Should have collapsible
      const details = container.querySelector('details');
      expect(details).toBeInTheDocument();

      // Should have highlighted code
      const codeBlock = container.querySelector('pre code');
      expect(codeBlock).toBeInTheDocument();
      expect(codeBlock?.className).toContain('language-javascript');
    });

    it('should work with markdown mixed content', () => {
      const markdown = `
# Heading

Regular paragraph with **bold** text.

\`\`\`python
def hello():
    return "world"
\`\`\`

Another paragraph with \`inline code\`.

- List item 1
- List item 2
      `.trim();

      const { container } = render(<MarkdownRenderer content={markdown} />);

      // Check heading
      expect(screen.getByRole('heading', { level: 1 })).toHaveTextContent('Heading');

      // Check bold
      expect(container.querySelector('strong')).toHaveTextContent('bold');

      // Check highlighted code block
      const codeBlock = container.querySelector('pre code');
      expect(codeBlock?.className).toContain('language-python');

      // Check inline code (NOT highlighted)
      const inlineCodes = container.querySelectorAll('code');
      const inlineCode = Array.from(inlineCodes).find(code => 
        code.textContent === 'inline code'
      );
      expect(inlineCode).toHaveClass('bg-muted');

      // Check list
      expect(container.querySelector('ul')).toBeInTheDocument();
    });

    it('should not interfere with copy buttons on multiple code blocks', async () => {
      const markdown = `
First block:
\`\`\`javascript
const a = 1;
\`\`\`

Second block:
\`\`\`python
b = 2
\`\`\`
      `.trim();

      const { container } = render(<MarkdownRenderer content={markdown} enableCodeCopy={true} />);

      const [btn1, btn2] = Array.from(container.querySelectorAll('button'));
      
      // Initially both should show "Copy"
      expect(btn1).toHaveTextContent('Copy');
      expect(btn2).toHaveTextContent('Copy');

      // Copy from first with act() wrapper
      await act(async () => {
        await userEvent.click(btn1);
        await new Promise(resolve => setTimeout(resolve, 100));
      });
      
      // First should change to "Copied"
      await waitFor(() => {
        expect(btn1).toHaveTextContent('Copied');
      }, { timeout: 1000 });

      // Second should still show "Copy" (not affected)
      expect(btn2).toHaveTextContent('Copy');
    });
  });

  describe('Performance', () => {
    it('should render large highlighted code block in reasonable time', () => {
      const largeCode = 'function line() { return true; }\n'.repeat(500);
      const markdown = `\`\`\`javascript\n${largeCode}\`\`\``;

      const start = performance.now();
      const { container } = render(<MarkdownRenderer content={markdown} />);
      const renderTime = performance.now() - start;

      // Should render in less than 200ms
      expect(renderTime).toBeLessThan(200);

      const codeBlock = container.querySelector('pre code');
      expect(codeBlock).toBeInTheDocument();
    });

    it('should handle multiple highlighted code blocks efficiently', () => {
      const blocks = Array.from({ length: 10 }, (_, i) => 
        `\`\`\`javascript\nfunction test${i}() { return ${i}; }\n\`\`\``
      ).join('\n\n');

      const start = performance.now();
      const { container } = render(<MarkdownRenderer content={blocks} />);
      const renderTime = performance.now() - start;

      // Should render in less than 300ms
      expect(renderTime).toBeLessThan(300);

      const codeBlocks = container.querySelectorAll('pre code');
      expect(codeBlocks).toHaveLength(10);
    });
  });

  describe('Props and Configuration', () => {
    it('should respect enableCodeCopy=false', () => {
      const markdown = `
\`\`\`javascript
const x = 1;
\`\`\`
      `.trim();

      const { container } = render(
        <MarkdownRenderer content={markdown} enableCodeCopy={false} />
      );

      const copyButton = container.querySelector('button');
      expect(copyButton).not.toBeInTheDocument();
    });

    it('should respect custom className', () => {
      const markdown = `
\`\`\`python
x = 1
\`\`\`
      `.trim();

      const { container } = render(
        <MarkdownRenderer content={markdown} className="custom-class" />
      );

      const prose = container.querySelector('.prose');
      expect(prose).toHaveClass('custom-class');
    });

    it('should respect custom ariaLabel', () => {
      const markdown = `
\`\`\`typescript
const x: number = 1;
\`\`\`
      `.trim();

      const { container } = render(
        <MarkdownRenderer content={markdown} ariaLabel="Custom test content" />
      );

      const article = container.querySelector('[role="article"]');
      expect(article).toHaveAttribute('aria-label', 'Custom test content');
    });
  });
});
