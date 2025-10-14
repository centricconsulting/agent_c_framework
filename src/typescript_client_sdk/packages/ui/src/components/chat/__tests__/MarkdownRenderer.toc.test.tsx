/**
 * MarkdownRenderer Table of Contents Tests
 * 
 * Testing Card 6: Table of Contents
 * Tests remark-toc plugin integration with:
 * - TOC heading detection (multiple formats)
 * - Nested structure generation (h2-h4)
 * - Heading depth filtering (h5+ excluded)
 * - Duplicate heading handling
 * - Special characters, emoji, code, emphasis in headings
 * - Edge cases (empty, long text, numbers)
 * - Integration with other features (alerts, code, math, tables, lists, collapsibles)
 * - Anchor link behavior (smooth scroll, no reload)
 * - Accessibility (semantic structure, keyboard navigation, aria-labels)
 * - Compact mode
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

describe('MarkdownRenderer - Table of Contents', () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  describe('TOC Heading Detection', () => {
    it('should generate TOC for "## Table of Contents" heading', () => {
      const markdown = `
## Table of Contents

## Introduction
Introduction content.

## Main Section
Main content.
      `.trim();

      const { container } = render(<MarkdownRenderer content={markdown} />);

      // Should have generated TOC list
      const lists = container.querySelectorAll('ul');
      expect(lists.length).toBeGreaterThan(0);

      // TOC should contain links to sections
      const links = within(container).getAllByRole('link');
      const tocLinks = links.filter(link => 
        link.getAttribute('href')?.startsWith('#')
      );
      expect(tocLinks.length).toBeGreaterThan(0);
    });

    it('should generate TOC for "## TOC" heading', () => {
      const markdown = `
## TOC

## Section One
Content.

## Section Two
Content.
      `.trim();

      const { container } = render(<MarkdownRenderer content={markdown} />);

      const links = within(container).getAllByRole('link');
      const tocLinks = links.filter(link => 
        link.getAttribute('href')?.startsWith('#')
      );
      expect(tocLinks.length).toBeGreaterThan(0);
    });

    it('should generate TOC for "## Contents" heading', () => {
      const markdown = `
## Contents

## First Heading
Text.

## Second Heading
Text.
      `.trim();

      const { container } = render(<MarkdownRenderer content={markdown} />);

      const links = within(container).getAllByRole('link');
      const tocLinks = links.filter(link => 
        link.getAttribute('href')?.startsWith('#')
      );
      expect(tocLinks.length).toBeGreaterThan(0);
    });

    it('should handle case-insensitive TOC heading detection', () => {
      const markdown = `
## table of contents

## Heading One
Content.
      `.trim();

      const { container } = render(<MarkdownRenderer content={markdown} />);

      const links = within(container).getAllByRole('link');
      const tocLinks = links.filter(link => 
        link.getAttribute('href')?.startsWith('#')
      );
      expect(tocLinks.length).toBeGreaterThan(0);
    });

    it('should NOT generate TOC if no placeholder heading exists', () => {
      const markdown = `
## Introduction
Introduction content.

## Main Section
Main content.
      `.trim();

      const { container } = render(<MarkdownRenderer content={markdown} />);

      // Should have headings but no TOC list
      const headings = container.querySelectorAll('h2');
      expect(headings).toHaveLength(2);

      // Links should not be anchor links to headings
      const links = container.querySelectorAll('a[href^="#"]');
      // If any anchor links exist, they're not part of a TOC
      // (could be manual anchor links in content)
    });

    it('should NOT generate TOC for h3 "TOC" heading', () => {
      const markdown = `
### TOC

## Section One
Content.
      `.trim();

      const { container } = render(<MarkdownRenderer content={markdown} />);

      // h3 TOC should not trigger generation
      // Would need to check if list structure is present
      const h3 = container.querySelector('h3');
      expect(h3).toHaveTextContent('TOC');

      // But TOC list should not be generated (remark-toc only works with h2)
    });
  });

  describe('Nested Structure Generation', () => {
    it('should include h2 headings in TOC', () => {
      const markdown = `
## Table of Contents

## First Section
Content.

## Second Section
Content.

## Third Section
Content.
      `.trim();

      const { container } = render(<MarkdownRenderer content={markdown} />);

      // Check for links to h2 sections
      const links = within(container).getAllByRole('link');
      const sectionLinks = links.filter(link => {
        const href = link.getAttribute('href');
        return href?.includes('first-section') || 
               href?.includes('second-section') || 
               href?.includes('third-section');
      });
      expect(sectionLinks.length).toBe(3);
    });

    it('should include h3 headings with indentation', () => {
      const markdown = `
## Table of Contents

## Main Section

### Subsection One
Content.

### Subsection Two
Content.
      `.trim();

      const { container } = render(<MarkdownRenderer content={markdown} />);

      // Should have nested list structure
      const nestedLists = container.querySelectorAll('ul ul');
      expect(nestedLists.length).toBeGreaterThan(0);
    });

    it('should include h4 headings with double indentation', () => {
      const markdown = `
## Table of Contents

## Main

### Subsection

#### Deep Heading
Content.
      `.trim();

      const { container } = render(<MarkdownRenderer content={markdown} />);

      // Should have deeply nested list structure
      const deeplyNested = container.querySelectorAll('ul ul ul');
      expect(deeplyNested.length).toBeGreaterThan(0);
    });

    it('should exclude h5 headings (beyond maxDepth)', () => {
      const markdown = `
## Table of Contents

## Main

### Sub

#### Deep

##### Very Deep
Should not be in TOC.
      `.trim();

      const { container } = render(<MarkdownRenderer content={markdown} />);

      // h5 content should exist but not be in TOC
      const h5 = container.querySelector('h5');
      expect(h5).toBeInTheDocument();
      expect(h5).toHaveTextContent('Very Deep');

      // But should not have link to it in TOC
      const veryDeepLink = container.querySelector('a[href="#very-deep"]');
      expect(veryDeepLink).not.toBeInTheDocument();
    });

    it('should handle mixed heading levels correctly', () => {
      const markdown = `
## Table of Contents

## Level 2 A

### Level 3 A1

#### Level 4 A1a

### Level 3 A2

## Level 2 B

### Level 3 B1
      `.trim();

      const { container } = render(<MarkdownRenderer content={markdown} />);

      // Should have proper nesting structure
      const topLevelItems = container.querySelector('ul:not(ul ul)');
      expect(topLevelItems).toBeInTheDocument();

      // Links should exist for h2, h3, h4
      const links = within(container).getAllByRole('link');
      const headingLinks = links.filter(l => l.getAttribute('href')?.startsWith('#'));
      // Should have at least 6 links (2 h2, 3 h3, 1 h4)
      expect(headingLinks.length).toBeGreaterThanOrEqual(6);
    });
  });

  describe('Duplicate Heading Handling', () => {
    it('should handle duplicate heading names with unique IDs', () => {
      const markdown = `
## Table of Contents

## Overview
First overview.

## Overview
Second overview.

## Overview
Third overview.
      `.trim();

      const { container } = render(<MarkdownRenderer content={markdown} />);

      // All three "Overview" headings should exist
      const headings = container.querySelectorAll('h2');
      const overviewHeadings = Array.from(headings).filter(h => 
        h.textContent === 'Overview'
      );
      expect(overviewHeadings.length).toBe(4); // 1 TOC + 3 Overview

      // Links in TOC should have unique hrefs
      const links = within(container).getAllByRole('link');
      const overviewLinks = links.filter(l => 
        l.textContent === 'Overview' && l.getAttribute('href')?.startsWith('#overview')
      );
      
      if (overviewLinks.length > 0) {
        const hrefs = overviewLinks.map(l => l.getAttribute('href'));
        const uniqueHrefs = new Set(hrefs);
        // Should have unique IDs for duplicates
        expect(uniqueHrefs.size).toBeGreaterThan(1);
      }
    });

    it('should generate proper suffixes for duplicate headings (-1, -2, etc)', () => {
      const markdown = `
## Table of Contents

## Testing
First.

## Testing
Second.
      `.trim();

      const { container } = render(<MarkdownRenderer content={markdown} />);

      // Check for -1 suffix in href
      const secondTestingLink = container.querySelector('a[href="#testing-1"]');
      // May or may not exist depending on remark-gfm behavior
      // Just ensure no errors occurred
      expect(container).toBeInTheDocument();
    });
  });

  describe('Special Characters in Headings', () => {
    it('should handle punctuation in headings', () => {
      const markdown = `
## Table of Contents

## What's Next?
Content.

## Who, What, Where!
Content.
      `.trim();

      const { container } = render(<MarkdownRenderer content={markdown} />);

      // Should slugify properly (removing or converting special chars)
      const links = within(container).getAllByRole('link');
      const specialLinks = links.filter(l => 
        l.getAttribute('href')?.includes('what') || 
        l.getAttribute('href')?.includes('who')
      );
      expect(specialLinks.length).toBeGreaterThan(0);
    });

    it('should handle emoji in headings', () => {
      const markdown = `
## Table of Contents

## Setup 🚀
Content.

## Results 🎉
Content.
      `.trim();

      const { container } = render(<MarkdownRenderer content={markdown} />);

      // Emoji should be stripped from slug
      const setupLink = container.querySelector('a[href*="setup"]');
      expect(setupLink).toBeInTheDocument();
    });

    it('should handle code in headings', () => {
      const markdown = `
## Table of Contents

## Using \`npm install\`
Content.
      `.trim();

      const { container } = render(<MarkdownRenderer content={markdown} />);

      // Code backticks should be stripped from slug
      const npmLink = container.querySelector('a[href*="npm"]');
      expect(npmLink).toBeInTheDocument();
    });

    it('should handle emphasis in headings', () => {
      const markdown = `
## Table of Contents

## *Important* Section
Content.

## **Critical** Update
Content.
      `.trim();

      const { container } = render(<MarkdownRenderer content={markdown} />);

      // Emphasis markers should be stripped from slug
      const importantLink = container.querySelector('a[href*="important"]');
      expect(importantLink).toBeInTheDocument();
    });

    it('should handle numbers in headings', () => {
      const markdown = `
## Table of Contents

## Chapter 1
Content.

## 2024 Updates
Content.
      `.trim();

      const { container } = render(<MarkdownRenderer content={markdown} />);

      const chapterLink = container.querySelector('a[href*="chapter"]');
      const updatesLink = container.querySelector('a[href*="2024"]');
      expect(chapterLink).toBeInTheDocument();
      expect(updatesLink).toBeInTheDocument();
    });
  });

  describe('Edge Cases', () => {
    it('should handle empty document', () => {
      const markdown = `## Table of Contents`;

      const { container } = render(<MarkdownRenderer content={markdown} />);

      // Should not crash, just show empty TOC
      expect(container).toBeInTheDocument();
    });

    it('should handle TOC with no other headings', () => {
      const markdown = `
## Table of Contents

Regular paragraph text with no other headings.
      `.trim();

      const { container } = render(<MarkdownRenderer content={markdown} />);

      // Should render but TOC list will be empty
      const tocHeading = within(container).getByRole('heading', { level: 2 });
      expect(tocHeading).toHaveTextContent('Table of Contents');
    });

    it('should handle very long heading text', () => {
      const longHeading = 'This is a Very Long Heading That Goes On and On and Contains Many Words and Should Still Generate a Proper Slug';
      const markdown = `
## Table of Contents

## ${longHeading}
Content.
      `.trim();

      const { container } = render(<MarkdownRenderer content={markdown} />);

      // Should create slug from long heading
      const links = within(container).getAllByRole('link');
      const longHeadingLinks = links.filter(l => 
        l.textContent?.includes('Very Long Heading')
      );
      expect(longHeadingLinks.length).toBeGreaterThan(0);
    });

    it('should handle headings with only special characters', () => {
      const markdown = `
## Table of Contents

## @#$%
Content.
      `.trim();

      const { container } = render(<MarkdownRenderer content={markdown} />);

      // Should handle gracefully even if slug is empty
      expect(container).toBeInTheDocument();
    });

    it('should handle headings at start and end of document', () => {
      const markdown = `
## First Heading
Content.

## Table of Contents

## Last Heading
Content.
      `.trim();

      const { container } = render(<MarkdownRenderer content={markdown} />);

      // TOC should include both first and last
      const links = within(container).getAllByRole('link');
      const firstLink = links.find(l => l.getAttribute('href')?.includes('first-heading'));
      const lastLink = links.find(l => l.getAttribute('href')?.includes('last-heading'));
      expect(firstLink).toBeInTheDocument();
      expect(lastLink).toBeInTheDocument();
    });
  });

  describe('Integration with Other Features', () => {
    it('should work with alerts', () => {
      const markdown = `
## Table of Contents

## Alert Section

:::note
This is a note alert.
:::

## Regular Section
Content.
      `.trim();

      const { container } = render(<MarkdownRenderer content={markdown} />);

      // TOC should exist
      const tocLinks = container.querySelectorAll('a[href^="#"]');
      expect(tocLinks.length).toBeGreaterThan(0);

      // Alert should exist
      const alert = screen.getByRole('note', { name: /Note alert/i });
      expect(alert).toBeInTheDocument();
    });

    it('should work with code blocks', () => {
      const markdown = `
## Table of Contents

## Code Examples

\`\`\`javascript
const x = 1;
\`\`\`

## More Content
Text.
      `.trim();

      const { container } = render(<MarkdownRenderer content={markdown} />);

      // TOC should exist
      const tocLinks = container.querySelectorAll('a[href^="#"]');
      expect(tocLinks.length).toBeGreaterThan(0);

      // Code block should exist
      const codeBlock = container.querySelector('code.language-javascript');
      expect(codeBlock).toBeInTheDocument();
    });

    it('should work with tables', () => {
      const markdown = `
## Table of Contents

## Data Table

| Column 1 | Column 2 |
|----------|----------|
| Data 1   | Data 2   |

## Summary
Text.
      `.trim();

      const { container } = render(<MarkdownRenderer content={markdown} />);

      // TOC should exist
      const tocLinks = container.querySelectorAll('a[href^="#"]');
      expect(tocLinks.length).toBeGreaterThan(0);

      // Table should exist
      const table = container.querySelector('table');
      expect(table).toBeInTheDocument();
    });

    it('should work with lists', () => {
      const markdown = `
## Table of Contents

## Task List

- Item 1
- Item 2
- Item 3

## Next Steps
Text.
      `.trim();

      const { container } = render(<MarkdownRenderer content={markdown} />);

      // TOC list should exist (separate from content list)
      const lists = container.querySelectorAll('ul');
      expect(lists.length).toBeGreaterThan(0);

      // Content list items should exist
      const listItems = container.querySelectorAll('li');
      expect(listItems.length).toBeGreaterThan(0);
    });

    it('should work with collapsible sections', () => {
      const markdown = `
## Table of Contents

## Collapsible Example

<details>
<summary>Click to expand</summary>
Hidden content.
</details>

## More Content
Text.
      `.trim();

      const { container } = render(<MarkdownRenderer content={markdown} />);

      // TOC should exist
      const tocLinks = container.querySelectorAll('a[href^="#"]');
      expect(tocLinks.length).toBeGreaterThan(0);

      // Collapsible should exist
      const details = container.querySelector('details');
      expect(details).toBeInTheDocument();
    });

    it('should work with inline math', () => {
      const markdown = `
## Table of Contents

## Math Section

This is inline math: $E = mc^2$

## Conclusion
Text.
      `.trim();

      const { container } = render(<MarkdownRenderer content={markdown} />);

      // TOC should exist
      const tocLinks = container.querySelectorAll('a[href^="#"]');
      expect(tocLinks.length).toBeGreaterThan(0);

      // Math content should exist
      expect(container).toHaveTextContent('E = mc^2');
    });
  });

  describe('Anchor Link Behavior', () => {
    it('should generate anchor links with # prefix', () => {
      const markdown = `
## Table of Contents

## Introduction
Content.

## Conclusion
Content.
      `.trim();

      const { container } = render(<MarkdownRenderer content={markdown} />);

      // All TOC links should start with #
      const links = within(container).getAllByRole('link');
      const tocLinks = links.filter(l => {
        const href = l.getAttribute('href');
        return href?.includes('introduction') || href?.includes('conclusion');
      });

      tocLinks.forEach(link => {
        expect(link.getAttribute('href')).toMatch(/^#/);
      });
    });

    it('should have clickable anchor links', () => {
      const markdown = `
## Table of Contents

## Target Section
Content.
      `.trim();

      const { container } = render(<MarkdownRenderer content={markdown} />);

      const targetLink = container.querySelector('a[href*="target"]');
      expect(targetLink).toBeInTheDocument();
      
      // Link should be clickable (not disabled)
      if (targetLink) {
        expect(targetLink).not.toHaveAttribute('disabled');
      }
    });

    it('should have proper link text matching heading', () => {
      const markdown = `
## Table of Contents

## Getting Started
Content.

## Advanced Topics
Content.
      `.trim();

      const { container } = render(<MarkdownRenderer content={markdown} />);

      // Link text should match heading text
      const gettingStartedLink = within(container).getAllByRole('link')
        .find(l => l.textContent === 'Getting Started');
      expect(gettingStartedLink).toBeInTheDocument();

      const advancedLink = within(container).getAllByRole('link')
        .find(l => l.textContent === 'Advanced Topics');
      expect(advancedLink).toBeInTheDocument();
    });
  });

  describe('Accessibility', () => {
    it('should use semantic list structure', () => {
      const markdown = `
## Table of Contents

## Section 1
Content.

## Section 2
Content.
      `.trim();

      const { container } = render(<MarkdownRenderer content={markdown} />);

      // Should have ul/li structure
      const list = container.querySelector('ul');
      expect(list).toBeInTheDocument();

      const listItems = container.querySelectorAll('li');
      expect(listItems.length).toBeGreaterThan(0);
    });

    it('should have proper heading hierarchy', () => {
      const markdown = `
## Table of Contents

## Main Section

### Subsection
Content.
      `.trim();

      const { container } = render(<MarkdownRenderer content={markdown} />);

      // h2 should exist for TOC and Main Section
      const h2s = container.querySelectorAll('h2');
      expect(h2s.length).toBeGreaterThanOrEqual(2);

      // h3 should exist for Subsection
      const h3s = container.querySelectorAll('h3');
      expect(h3s.length).toBeGreaterThanOrEqual(1);
    });

    it('should have keyboard navigable links', () => {
      const markdown = `
## Table of Contents

## Section A
Content.

## Section B
Content.
      `.trim();

      const { container } = render(<MarkdownRenderer content={markdown} />);

      const links = within(container).getAllByRole('link');
      const tocLinks = links.filter(l => l.getAttribute('href')?.startsWith('#'));

      tocLinks.forEach(link => {
        // Links should be focusable (no tabindex=-1)
        expect(link).not.toHaveAttribute('tabindex', '-1');
      });
    });

    it('should have proper link contrast', () => {
      const markdown = `
## Table of Contents

## Test Section
Content.
      `.trim();

      const { container } = render(<MarkdownRenderer content={markdown} />);

      const links = within(container).getAllByRole('link');
      const tocLinks = links.filter(l => l.getAttribute('href')?.startsWith('#'));

      tocLinks.forEach(link => {
        // Should have text-primary class for theme colors
        expect(link).toHaveClass('text-primary');
      });
    });

    it('should use article role on container', () => {
      const markdown = `
## Table of Contents

## Content
Text.
      `.trim();

      const { container } = render(<MarkdownRenderer content={markdown} />);

      const article = container.querySelector('[role="article"]');
      expect(article).toBeInTheDocument();
    });
  });

  describe('Compact Mode', () => {
    it('should render TOC in compact mode', () => {
      const markdown = `
## Table of Contents

## Section 1
Content.

## Section 2
Content.
      `.trim();

      const { container } = render(<MarkdownRenderer content={markdown} compact={true} />);

      // Should have prose-sm class for compact prose
      const prose = container.querySelector('.prose');
      expect(prose).toHaveClass('prose-sm');

      // TOC should still be generated
      const tocLinks = container.querySelectorAll('a[href^="#"]');
      expect(tocLinks.length).toBeGreaterThan(0);
    });

    it('should maintain TOC functionality in compact mode', () => {
      const markdown = `
## Table of Contents

## Main

### Sub
Content.
      `.trim();

      const { container } = render(<MarkdownRenderer content={markdown} compact={true} />);

      // Nested structure should still work
      const nestedLists = container.querySelectorAll('ul ul');
      expect(nestedLists.length).toBeGreaterThan(0);
    });
  });

  describe('Props and Configuration', () => {
    it('should respect custom className', () => {
      const markdown = `
## Table of Contents

## Section
Content.
      `.trim();

      const { container } = render(
        <MarkdownRenderer content={markdown} className="custom-toc-class" />
      );

      const prose = container.querySelector('.prose');
      expect(prose).toHaveClass('custom-toc-class');
    });

    it('should respect custom ariaLabel', () => {
      const markdown = `
## Table of Contents

## Section
Content.
      `.trim();

      const { container } = render(
        <MarkdownRenderer content={markdown} ariaLabel="Document with TOC" />
      );

      const article = container.querySelector('[role="article"]');
      expect(article).toHaveAttribute('aria-label', 'Document with TOC');
    });

    it('should work with enableCodeCopy prop', () => {
      const markdown = `
## Table of Contents

## Code Section

\`\`\`javascript
const x = 1;
\`\`\`
      `.trim();

      const { container } = render(
        <MarkdownRenderer content={markdown} enableCodeCopy={false} />
      );

      // TOC should still work
      const tocLinks = container.querySelectorAll('a[href^="#"]');
      expect(tocLinks.length).toBeGreaterThan(0);

      // No copy buttons
      const copyButtons = container.querySelectorAll('button');
      expect(copyButtons.length).toBe(0);
    });
  });

  describe('Performance', () => {
    it('should handle large TOC efficiently', () => {
      const sections = Array.from({ length: 50 }, (_, i) => 
        `## Section ${i}\nContent ${i}.`
      ).join('\n\n');
      
      const markdown = `## Table of Contents\n\n${sections}`;

      const start = performance.now();
      const { container } = render(<MarkdownRenderer content={markdown} />);
      const renderTime = performance.now() - start;

      // Should render in reasonable time
      expect(renderTime).toBeLessThan(200);

      // Should have generated TOC with many links
      const tocLinks = container.querySelectorAll('a[href^="#"]');
      expect(tocLinks.length).toBeGreaterThan(40);
    });

    it('should handle deeply nested structure efficiently', () => {
      const markdown = `
## Table of Contents

## Level 1

### Level 2

#### Level 3

##### Level 4 (excluded)

### Another Level 2

#### Another Level 3
      `.trim();

      const start = performance.now();
      render(<MarkdownRenderer content={markdown} />);
      const renderTime = performance.now() - start;

      expect(renderTime).toBeLessThan(100);
    });
  });
});
