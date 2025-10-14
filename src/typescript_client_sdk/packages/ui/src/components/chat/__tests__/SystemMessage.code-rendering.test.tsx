/**
 * SystemMessage Code Rendering Tests
 * 
 * Validates the fix for HTML nesting error: "In HTML, <pre> cannot be a descendant of <p>"
 * Tests the separate handling of pre and code components in ReactMarkdown
 */

import React from 'react';
import { describe, it, expect, beforeEach, vi } from 'vitest';
import { render, screen } from '@testing-library/react';
import { SystemMessage } from '../SystemMessage';

// Mock Lucide icons
vi.mock('lucide-react', () => ({
  Info: ({ className }: any) => <div data-testid="info-icon" className={className}>Info</div>,
  AlertTriangle: ({ className }: any) => <div data-testid="alert-triangle-icon" className={className}>Warning</div>,
  XCircle: ({ className }: any) => <div data-testid="x-circle-icon" className={className}>Error</div>
}));

// Enhanced ReactMarkdown mock that actually uses component overrides
vi.mock('react-markdown', () => ({
  default: ({ children, components }: any) => {
    // Simulate markdown parsing for code blocks and inline code
    const content = String(children);
    
    // Detect code blocks (triple backticks)
    const codeBlockRegex = /```(\w+)?\n?([\s\S]*?)```/g;
    const inlineCodeRegex = /`([^`]+)`/g;
    
    let parts: React.ReactNode[] = [];
    let lastIndex = 0;
    let match;
    
    // Process code blocks first
    const codeBlockMatches: Array<{ start: number; end: number; language: string; code: string }> = [];
    while ((match = codeBlockRegex.exec(content)) !== null) {
      codeBlockMatches.push({
        start: match.index,
        end: match.index + match[0].length,
        language: match[1] || '',
        code: match[2]
      });
    }
    
    // If we have code blocks, process them
    if (codeBlockMatches.length > 0) {
      codeBlockMatches.forEach((blockMatch, idx) => {
        // Add text before code block
        if (blockMatch.start > lastIndex) {
          const textBefore = content.substring(lastIndex, blockMatch.start);
          parts.push(<span key={`text-${idx}`}>{textBefore}</span>);
        }
        
        // Add code block using pre component
        const PreComponent = components?.pre || 'pre';
        const CodeComponent = components?.code || 'code';
        parts.push(
          <PreComponent key={`pre-${idx}`}>
            <CodeComponent inline={false} className={`language-${blockMatch.language}`}>
              {blockMatch.code}
            </CodeComponent>
          </PreComponent>
        );
        
        lastIndex = blockMatch.end;
      });
      
      // Add remaining text
      if (lastIndex < content.length) {
        parts.push(<span key="text-end">{content.substring(lastIndex)}</span>);
      }
      
      return <div data-testid="markdown-content">{parts}</div>;
    }
    
    // Process inline code
    const inlineCodeMatches: Array<{ start: number; end: number; code: string }> = [];
    while ((match = inlineCodeRegex.exec(content)) !== null) {
      inlineCodeMatches.push({
        start: match.index,
        end: match.index + match[0].length,
        code: match[1]
      });
    }
    
    if (inlineCodeMatches.length > 0) {
      lastIndex = 0;
      parts = [];
      
      inlineCodeMatches.forEach((codeMatch, idx) => {
        // Add text before inline code
        if (codeMatch.start > lastIndex) {
          parts.push(<span key={`text-${idx}`}>{content.substring(lastIndex, codeMatch.start)}</span>);
        }
        
        // Add inline code using code component
        const CodeComponent = components?.code || 'code';
        parts.push(
          <CodeComponent key={`code-${idx}`} inline={true}>
            {codeMatch.code}
          </CodeComponent>
        );
        
        lastIndex = codeMatch.end;
      });
      
      // Add remaining text
      if (lastIndex < content.length) {
        parts.push(<span key="text-end">{content.substring(lastIndex)}</span>);
      }
      
      return <div data-testid="markdown-content">{parts}</div>;
    }
    
    // No code detected, just return the content
    const PComponent = components?.p || 'p';
    return (
      <div data-testid="markdown-content">
        <PComponent>{children}</PComponent>
      </div>
    );
  }
}));

vi.mock('remark-gfm', () => ({
  default: vi.fn()
}));

describe('SystemMessage - Code Rendering (HTML Nesting Fix)', () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  describe('Inline Code Rendering', () => {
    it('should render inline code with proper styling', () => {
      const { container } = render(
        <SystemMessage
          content="Use the `npm install` command to install packages"
          severity="info"
          format="markdown"
        />
      );

      // Find inline code element
      const codeElements = container.querySelectorAll('code');
      expect(codeElements.length).toBe(1);
      
      const inlineCode = codeElements[0];
      expect(inlineCode).toBeInTheDocument();
      expect(inlineCode.textContent).toBe('npm install');
      
      // Verify inline code styling
      expect(inlineCode).toHaveClass('px-1', 'py-0.5', 'rounded', 'text-sm');
      expect(inlineCode.className).toMatch(/bg-black\/10/);
      expect(inlineCode.className).toMatch(/dark:bg-white\/10/);
    });

    it('should render multiple inline code segments', () => {
      const { container } = render(
        <SystemMessage
          content="Compare `foo` and `bar` values"
          severity="info"
          format="markdown"
        />
      );

      const codeElements = container.querySelectorAll('code');
      expect(codeElements.length).toBe(2);
      
      expect(codeElements[0].textContent).toBe('foo');
      expect(codeElements[1].textContent).toBe('bar');
      
      // Both should have inline styling
      codeElements.forEach(code => {
        expect(code).toHaveClass('px-1', 'rounded');
      });
    });

    it('should NOT wrap inline code in <pre> element', () => {
      const { container } = render(
        <SystemMessage
          content="Run `npm test` to execute tests"
          severity="info"
          format="markdown"
        />
      );

      // Inline code should NOT have a pre parent
      const codeElements = container.querySelectorAll('code');
      expect(codeElements.length).toBe(1);
      
      const inlineCode = codeElements[0];
      expect(inlineCode.parentElement?.tagName).not.toBe('PRE');
    });
  });

  describe('Code Block Rendering', () => {
    it('should render code block with pre wrapper', () => {
      const codeContent = `function hello() {
  console.log('world');
}`;
      
      const { container } = render(
        <SystemMessage
          content={`Here's an example:\n\`\`\`javascript\n${codeContent}\n\`\`\``}
          severity="info"
          format="markdown"
        />
      );

      // Should have pre element
      const preElement = container.querySelector('pre');
      expect(preElement).toBeInTheDocument();
      
      // Pre should have proper styling
      expect(preElement).toHaveClass('p-2', 'rounded', 'overflow-x-auto');
      expect(preElement?.className).toMatch(/bg-black\/10/);
      expect(preElement?.className).toMatch(/dark:bg-white\/10/);
      
      // Pre should contain code element
      const codeElement = preElement?.querySelector('code');
      expect(codeElement).toBeInTheDocument();
      expect(codeElement?.textContent?.trim()).toBe(codeContent);
    });

    it('should apply language class to code block', () => {
      const { container } = render(
        <SystemMessage
          content="```typescript\nconst x: number = 42;\n```"
          severity="info"
          format="markdown"
        />
      );

      const codeElement = container.querySelector('code');
      expect(codeElement).toHaveClass('language-typescript');
    });

    it('should handle code block without language specifier', () => {
      const { container } = render(
        <SystemMessage
          content="```\nplain code block\n```"
          severity="info"
          format="markdown"
        />
      );

      const preElement = container.querySelector('pre');
      const codeElement = preElement?.querySelector('code');
      
      expect(preElement).toBeInTheDocument();
      expect(codeElement).toBeInTheDocument();
      expect(codeElement?.textContent).toContain('plain code block');
    });

    it('should have proper pre/code nesting structure', () => {
      const { container } = render(
        <SystemMessage
          content="```\ncode\n```"
          severity="info"
          format="markdown"
        />
      );

      const preElement = container.querySelector('pre');
      expect(preElement).toBeInTheDocument();
      
      // Verify structure: pre > code (not nested pre elements)
      const codeElement = preElement?.querySelector('code');
      expect(codeElement).toBeInTheDocument();
      expect(codeElement?.parentElement?.tagName).toBe('PRE');
      
      // Ensure no nested pre elements
      const nestedPre = preElement?.querySelector('pre');
      expect(nestedPre).toBeNull();
    });

    it('should apply block code styling', () => {
      const { container } = render(
        <SystemMessage
          content="```\ncode block\n```"
          severity="info"
          format="markdown"
        />
      );

      const codeElement = container.querySelector('code');
      expect(codeElement).toHaveClass('text-sm');
      
      // Block code should NOT have inline styling (no px-1, py-0.5)
      expect(codeElement?.classList.contains('px-1')).toBe(false);
      expect(codeElement?.classList.contains('py-0.5')).toBe(false);
    });
  });

  describe('Mixed Content Rendering', () => {
    it('should handle both inline and block code in same message', () => {
      const content = `Use \`npm install\` first, then:\n\`\`\`bash\nnpm run build\n\`\`\``;
      
      const { container } = render(
        <SystemMessage
          content={content}
          severity="info"
          format="markdown"
        />
      );

      // Should have both inline and block code
      const codeElements = container.querySelectorAll('code');
      expect(codeElements.length).toBe(2);
      
      // First should be inline (has inline styling)
      const inlineCode = codeElements[0];
      expect(inlineCode.textContent).toBe('npm install');
      expect(inlineCode).toHaveClass('px-1');
      expect(inlineCode.parentElement?.tagName).not.toBe('PRE');
      
      // Second should be in pre block
      const blockCode = codeElements[1];
      expect(blockCode.textContent?.trim()).toBe('npm run build');
      expect(blockCode.parentElement?.tagName).toBe('PRE');
    });

    it('should handle multiple code blocks', () => {
      const content = `First block:\n\`\`\`js\nconst a = 1;\n\`\`\`\n\nSecond block:\n\`\`\`js\nconst b = 2;\n\`\`\``;
      
      const { container } = render(
        <SystemMessage
          content={content}
          severity="info"
          format="markdown"
        />
      );

      const preElements = container.querySelectorAll('pre');
      expect(preElements.length).toBe(2);
      
      preElements.forEach(pre => {
        expect(pre.querySelector('code')).toBeInTheDocument();
        expect(pre).toHaveClass('p-2', 'rounded');
      });
    });
  });

  describe('HTML Nesting Validation', () => {
    it('should NOT create nested pre elements', () => {
      const { container } = render(
        <SystemMessage
          content="```javascript\nconst x = 1;\n```"
          severity="info"
          format="markdown"
        />
      );

      const outerPre = container.querySelector('pre');
      expect(outerPre).toBeInTheDocument();
      
      // Check that there are no nested pre elements
      const nestedPre = outerPre?.querySelector('pre');
      expect(nestedPre).toBeNull();
    });

    it('should NOT place pre inside p element', () => {
      const { container } = render(
        <SystemMessage
          content="Text before\n```\ncode\n```\nText after"
          severity="info"
          format="markdown"
        />
      );

      const preElement = container.querySelector('pre');
      expect(preElement).toBeInTheDocument();
      
      // Walk up the DOM tree to ensure no <p> parent
      let parent = preElement?.parentElement;
      while (parent && parent !== container) {
        expect(parent.tagName).not.toBe('P');
        parent = parent.parentElement;
      }
    });

    it('should maintain proper semantic HTML structure', () => {
      const { container } = render(
        <SystemMessage
          content="```\ncode\n```"
          severity="info"
          format="markdown"
        />
      );

      // Expected structure: div (markdown-content) > pre > code
      const markdownDiv = container.querySelector('[data-testid="markdown-content"]');
      expect(markdownDiv).toBeInTheDocument();
      
      const preElement = markdownDiv?.querySelector(':scope > pre');
      expect(preElement).toBeInTheDocument();
      
      const codeElement = preElement?.querySelector(':scope > code');
      expect(codeElement).toBeInTheDocument();
    });
  });

  describe('Dark Mode Code Styling', () => {
    it('should apply dark mode styles to inline code', () => {
      const { container } = render(
        <SystemMessage
          content="Run `npm install` command"
          severity="info"
          format="markdown"
        />
      );

      const codeElement = container.querySelector('code');
      expect(codeElement?.className).toMatch(/dark:bg-white\/10/);
    });

    it('should apply dark mode styles to code blocks', () => {
      const { container } = render(
        <SystemMessage
          content="```\ncode\n```"
          severity="info"
          format="markdown"
        />
      );

      const preElement = container.querySelector('pre');
      expect(preElement?.className).toMatch(/dark:bg-white\/10/);
    });
  });

  describe('Edge Cases', () => {
    it('should handle empty code block', () => {
      const { container } = render(
        <SystemMessage
          content="```\n```"
          severity="info"
          format="markdown"
        />
      );

      const preElement = container.querySelector('pre');
      expect(preElement).toBeInTheDocument();
    });

    it('should handle code block with only whitespace', () => {
      const { container } = render(
        <SystemMessage
          content="```\n   \n```"
          severity="info"
          format="markdown"
        />
      );

      const preElement = container.querySelector('pre');
      const codeElement = preElement?.querySelector('code');
      expect(codeElement).toBeInTheDocument();
    });

    it('should handle inline code with special characters', () => {
      const { container } = render(
        <SystemMessage
          content="Use `<Component />` in JSX"
          severity="info"
          format="markdown"
        />
      );

      const codeElement = container.querySelector('code');
      expect(codeElement?.textContent).toBe('<Component />');
    });

    it('should handle very long code blocks', () => {
      const longCode = 'const x = 1;\n'.repeat(100);
      const { container } = render(
        <SystemMessage
          content={`\`\`\`javascript\n${longCode}\`\`\``}
          severity="info"
          format="markdown"
        />
      );

      const preElement = container.querySelector('pre');
      expect(preElement).toHaveClass('overflow-x-auto');
      expect(preElement?.textContent).toContain('const x = 1;');
    });
  });

  describe('Accessibility with Code Content', () => {
    it('should maintain ARIA attributes with code blocks', () => {
      const { container } = render(
        <SystemMessage
          content="```\ncode\n```"
          severity="error"
          format="markdown"
        />
      );

      const alert = container.querySelector('[role="alert"]');
      expect(alert).toHaveAttribute('aria-live', 'assertive');
    });

    it('should be readable by screen readers', () => {
      const { container } = render(
        <SystemMessage
          content="Error in `processData` function"
          severity="error"
          format="markdown"
        />
      );

      // Code should be readable as part of the message
      const alert = container.querySelector('[role="alert"]');
      expect(alert?.textContent).toContain('processData');
    });
  });
});
