/**
 * MultimodalContentRenderer Component Tests
 * Testing routing, mixed content, spacing, role passing, and edge cases
 */

import React from 'react';
import { describe, it, expect, beforeEach, afterEach, vi } from 'vitest';
import { render, screen, within } from '@testing-library/react';
import { MultimodalContentRenderer } from '../MultimodalContentRenderer';
import type { MessageContentBlock } from '@agentc/realtime-react';

// Mock child renderers
vi.mock('../TextContentRenderer', () => ({
  TextContentRenderer: ({ content, role }: any) => (
    <div 
      data-testid="text-content-renderer" 
      data-role={role}
      data-content={content}
    >
      Text: {content}
    </div>
  )
}));

vi.mock('../ImageContentRenderer', () => ({
  ImageContentRenderer: ({ content }: any) => (
    <div 
      data-testid="image-content-renderer"
      data-source-type={content.source.type}
      data-source-data={content.source.data}
    >
      Image: {content.source.type}
    </div>
  )
}));

describe('MultimodalContentRenderer Component', () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  afterEach(() => {
    vi.restoreAllMocks();
  });

  describe('Text Block Routing', () => {
    it('should route text blocks to TextContentRenderer', () => {
      const content: MessageContentBlock[] = [
        { type: 'text', text: 'Hello world' }
      ];

      render(<MultimodalContentRenderer content={content} role="user" />);

      const textRenderer = screen.getByTestId('text-content-renderer');
      expect(textRenderer).toBeInTheDocument();
      expect(textRenderer).toHaveAttribute('data-content', 'Hello world');
    });

    it('should pass role prop to TextContentRenderer', () => {
      const content: MessageContentBlock[] = [
        { type: 'text', text: 'Test message' }
      ];

      render(<MultimodalContentRenderer content={content} role="assistant" />);

      const textRenderer = screen.getByTestId('text-content-renderer');
      expect(textRenderer).toHaveAttribute('data-role', 'assistant');
    });

    it('should render multiple text blocks', () => {
      const content: MessageContentBlock[] = [
        { type: 'text', text: 'First paragraph' },
        { type: 'text', text: 'Second paragraph' },
        { type: 'text', text: 'Third paragraph' }
      ];

      render(<MultimodalContentRenderer content={content} role="user" />);

      const textRenderers = screen.getAllByTestId('text-content-renderer');
      expect(textRenderers).toHaveLength(3);
      expect(textRenderers[0]).toHaveAttribute('data-content', 'First paragraph');
      expect(textRenderers[1]).toHaveAttribute('data-content', 'Second paragraph');
      expect(textRenderers[2]).toHaveAttribute('data-content', 'Third paragraph');
    });

    it('should handle empty text blocks', () => {
      const content: MessageContentBlock[] = [
        { type: 'text', text: '' }
      ];

      render(<MultimodalContentRenderer content={content} role="user" />);

      const textRenderer = screen.getByTestId('text-content-renderer');
      expect(textRenderer).toBeInTheDocument();
      expect(textRenderer).toHaveAttribute('data-content', '');
    });

    it('should handle text blocks with special characters', () => {
      const content: MessageContentBlock[] = [
        { type: 'text', text: 'Special chars: !@#$%^&*()_+-=[]{}|;:,.<>?' }
      ];

      render(<MultimodalContentRenderer content={content} role="user" />);

      const textRenderer = screen.getByTestId('text-content-renderer');
      expect(textRenderer).toHaveAttribute('data-content', 'Special chars: !@#$%^&*()_+-=[]{}|;:,.<>?');
    });

    it('should handle text blocks with newlines', () => {
      const content: MessageContentBlock[] = [
        { type: 'text', text: 'Line 1\nLine 2\nLine 3' }
      ];

      render(<MultimodalContentRenderer content={content} role="user" />);

      const textRenderer = screen.getByTestId('text-content-renderer');
      expect(textRenderer).toHaveAttribute('data-content', 'Line 1\nLine 2\nLine 3');
    });

    it('should handle long text blocks', () => {
      const longText = 'Lorem ipsum '.repeat(100);
      const content: MessageContentBlock[] = [
        { type: 'text', text: longText }
      ];

      render(<MultimodalContentRenderer content={content} role="user" />);

      const textRenderer = screen.getByTestId('text-content-renderer');
      expect(textRenderer).toHaveAttribute('data-content', longText);
    });
  });

  describe('Image Block Routing', () => {
    it('should route image blocks to ImageContentRenderer - URL source', () => {
      const content: MessageContentBlock[] = [
        {
          type: 'image',
          source: {
            type: 'url',
            data: 'https://example.com/image.jpg'
          }
        }
      ];

      render(<MultimodalContentRenderer content={content} role="user" />);

      const imageRenderer = screen.getByTestId('image-content-renderer');
      expect(imageRenderer).toBeInTheDocument();
      expect(imageRenderer).toHaveAttribute('data-source-type', 'url');
      expect(imageRenderer).toHaveAttribute('data-source-data', 'https://example.com/image.jpg');
    });

    it('should route image blocks to ImageContentRenderer - base64 source', () => {
      const content: MessageContentBlock[] = [
        {
          type: 'image',
          source: {
            type: 'base64',
            media_type: 'image/png',
            data: 'iVBORw0KGgoAAAANSUhEUgAAAAUA'
          }
        }
      ];

      render(<MultimodalContentRenderer content={content} role="assistant" />);

      const imageRenderer = screen.getByTestId('image-content-renderer');
      expect(imageRenderer).toBeInTheDocument();
      expect(imageRenderer).toHaveAttribute('data-source-type', 'base64');
      expect(imageRenderer).toHaveAttribute('data-source-data', 'iVBORw0KGgoAAAANSUhEUgAAAAUA');
    });

    it('should render multiple image blocks', () => {
      const content: MessageContentBlock[] = [
        {
          type: 'image',
          source: { type: 'url', data: 'https://example.com/image1.jpg' }
        },
        {
          type: 'image',
          source: { type: 'url', data: 'https://example.com/image2.jpg' }
        },
        {
          type: 'image',
          source: { type: 'url', data: 'https://example.com/image3.jpg' }
        }
      ];

      render(<MultimodalContentRenderer content={content} role="user" />);

      const imageRenderers = screen.getAllByTestId('image-content-renderer');
      expect(imageRenderers).toHaveLength(3);
      expect(imageRenderers[0]).toHaveAttribute('data-source-data', 'https://example.com/image1.jpg');
      expect(imageRenderers[1]).toHaveAttribute('data-source-data', 'https://example.com/image2.jpg');
      expect(imageRenderers[2]).toHaveAttribute('data-source-data', 'https://example.com/image3.jpg');
    });

    it('should handle images with media_type', () => {
      const content: MessageContentBlock[] = [
        {
          type: 'image',
          source: {
            type: 'base64',
            media_type: 'image/jpeg',
            data: '/9j/4AAQSkZJRgABAQAAAQABAAD'
          }
        }
      ];

      render(<MultimodalContentRenderer content={content} role="user" />);

      const imageRenderer = screen.getByTestId('image-content-renderer');
      expect(imageRenderer).toBeInTheDocument();
    });
  });

  describe('Mixed Content (Text + Images)', () => {
    it('should render text followed by image', () => {
      const content: MessageContentBlock[] = [
        { type: 'text', text: 'Check out this image:' },
        {
          type: 'image',
          source: { type: 'url', data: 'https://example.com/image.jpg' }
        }
      ];

      render(<MultimodalContentRenderer content={content} role="user" />);

      const textRenderer = screen.getByTestId('text-content-renderer');
      const imageRenderer = screen.getByTestId('image-content-renderer');

      expect(textRenderer).toBeInTheDocument();
      expect(imageRenderer).toBeInTheDocument();

      // Check order using container
      const container = screen.getByTestId('text-content-renderer').parentElement!;
      const children = Array.from(container.children);
      const textIndex = children.findIndex(el => el === textRenderer);
      const imageIndex = children.findIndex(el => el === imageRenderer);
      expect(textIndex).toBeLessThan(imageIndex);
    });

    it('should render image followed by text', () => {
      const content: MessageContentBlock[] = [
        {
          type: 'image',
          source: { type: 'url', data: 'https://example.com/image.jpg' }
        },
        { type: 'text', text: 'Caption for the image above' }
      ];

      render(<MultimodalContentRenderer content={content} role="assistant" />);

      const textRenderer = screen.getByTestId('text-content-renderer');
      const imageRenderer = screen.getByTestId('image-content-renderer');

      expect(textRenderer).toBeInTheDocument();
      expect(imageRenderer).toBeInTheDocument();

      // Check order
      const container = screen.getByTestId('text-content-renderer').parentElement!;
      const children = Array.from(container.children);
      const textIndex = children.findIndex(el => el === textRenderer);
      const imageIndex = children.findIndex(el => el === imageRenderer);
      expect(imageIndex).toBeLessThan(textIndex);
    });

    it('should render interleaved text and images', () => {
      const content: MessageContentBlock[] = [
        { type: 'text', text: 'First text' },
        {
          type: 'image',
          source: { type: 'url', data: 'https://example.com/image1.jpg' }
        },
        { type: 'text', text: 'Second text' },
        {
          type: 'image',
          source: { type: 'url', data: 'https://example.com/image2.jpg' }
        },
        { type: 'text', text: 'Third text' }
      ];

      render(<MultimodalContentRenderer content={content} role="user" />);

      const textRenderers = screen.getAllByTestId('text-content-renderer');
      const imageRenderers = screen.getAllByTestId('image-content-renderer');

      expect(textRenderers).toHaveLength(3);
      expect(imageRenderers).toHaveLength(2);

      // Verify content
      expect(textRenderers[0]).toHaveAttribute('data-content', 'First text');
      expect(textRenderers[1]).toHaveAttribute('data-content', 'Second text');
      expect(textRenderers[2]).toHaveAttribute('data-content', 'Third text');
    });

    it('should render multiple images followed by multiple texts', () => {
      const content: MessageContentBlock[] = [
        {
          type: 'image',
          source: { type: 'url', data: 'https://example.com/image1.jpg' }
        },
        {
          type: 'image',
          source: { type: 'url', data: 'https://example.com/image2.jpg' }
        },
        { type: 'text', text: 'Caption 1' },
        { type: 'text', text: 'Caption 2' }
      ];

      render(<MultimodalContentRenderer content={content} role="assistant" />);

      const textRenderers = screen.getAllByTestId('text-content-renderer');
      const imageRenderers = screen.getAllByTestId('image-content-renderer');

      expect(textRenderers).toHaveLength(2);
      expect(imageRenderers).toHaveLength(2);
    });

    it('should render complex mixed content with various types', () => {
      const content: MessageContentBlock[] = [
        { type: 'text', text: 'Introduction' },
        {
          type: 'image',
          source: { type: 'url', data: 'https://example.com/chart.png' }
        },
        { type: 'text', text: 'Analysis of the chart' },
        {
          type: 'image',
          source: {
            type: 'base64',
            media_type: 'image/png',
            data: 'base64data'
          }
        },
        { type: 'text', text: 'Conclusion' }
      ];

      render(<MultimodalContentRenderer content={content} role="user" />);

      const textRenderers = screen.getAllByTestId('text-content-renderer');
      const imageRenderers = screen.getAllByTestId('image-content-renderer');

      expect(textRenderers).toHaveLength(3);
      expect(imageRenderers).toHaveLength(2);

      // Verify first and last are text
      expect(textRenderers[0]).toHaveAttribute('data-content', 'Introduction');
      expect(textRenderers[2]).toHaveAttribute('data-content', 'Conclusion');
    });
  });

  describe('Empty Content Handling', () => {
    it('should handle empty content array', () => {
      const content: MessageContentBlock[] = [];

      const { container } = render(
        <MultimodalContentRenderer content={content} role="user" />
      );

      // Should render container but no content
      const wrapper = container.querySelector('.flex.flex-col');
      expect(wrapper).toBeInTheDocument();
      expect(wrapper?.children).toHaveLength(0);
    });

    it('should not render anything for empty array', () => {
      const content: MessageContentBlock[] = [];

      render(<MultimodalContentRenderer content={content} role="assistant" />);

      expect(screen.queryByTestId('text-content-renderer')).not.toBeInTheDocument();
      expect(screen.queryByTestId('image-content-renderer')).not.toBeInTheDocument();
    });

    it('should handle array with only null blocks gracefully', () => {
      const content = [
        null as any,
        null as any
      ];

      const { container } = render(
        <MultimodalContentRenderer content={content} role="user" />
      );

      // Should render container
      expect(container.querySelector('.flex.flex-col')).toBeInTheDocument();
    });
  });

  describe('Vertical Stacking and Spacing', () => {
    it('should apply flex-col for vertical stacking', () => {
      const content: MessageContentBlock[] = [
        { type: 'text', text: 'First' },
        { type: 'text', text: 'Second' }
      ];

      const { container } = render(
        <MultimodalContentRenderer content={content} role="user" />
      );

      const wrapper = container.querySelector('.flex.flex-col');
      expect(wrapper).toBeInTheDocument();
    });

    it('should apply gap-3 spacing between blocks', () => {
      const content: MessageContentBlock[] = [
        { type: 'text', text: 'First' },
        {
          type: 'image',
          source: { type: 'url', data: 'https://example.com/image.jpg' }
        }
      ];

      const { container } = render(
        <MultimodalContentRenderer content={content} role="user" />
      );

      const wrapper = container.querySelector('.flex.flex-col.gap-3');
      expect(wrapper).toBeInTheDocument();
    });

    it('should maintain consistent spacing for multiple blocks', () => {
      const content: MessageContentBlock[] = [
        { type: 'text', text: 'Block 1' },
        { type: 'text', text: 'Block 2' },
        { type: 'text', text: 'Block 3' },
        { type: 'text', text: 'Block 4' }
      ];

      const { container } = render(
        <MultimodalContentRenderer content={content} role="user" />
      );

      const wrapper = container.querySelector('.gap-3');
      expect(wrapper).toBeInTheDocument();
      expect(wrapper?.children).toHaveLength(4);
    });

    it('should apply spacing for mixed content types', () => {
      const content: MessageContentBlock[] = [
        { type: 'text', text: 'Text 1' },
        {
          type: 'image',
          source: { type: 'url', data: 'https://example.com/image1.jpg' }
        },
        { type: 'text', text: 'Text 2' },
        {
          type: 'image',
          source: { type: 'url', data: 'https://example.com/image2.jpg' }
        }
      ];

      const { container } = render(
        <MultimodalContentRenderer content={content} role="user" />
      );

      const wrapper = container.querySelector('.gap-3');
      expect(wrapper).toBeInTheDocument();
    });
  });

  describe('Role Prop Passing', () => {
    it('should pass user role to TextContentRenderer', () => {
      const content: MessageContentBlock[] = [
        { type: 'text', text: 'User message' }
      ];

      render(<MultimodalContentRenderer content={content} role="user" />);

      const textRenderer = screen.getByTestId('text-content-renderer');
      expect(textRenderer).toHaveAttribute('data-role', 'user');
    });

    it('should pass assistant role to TextContentRenderer', () => {
      const content: MessageContentBlock[] = [
        { type: 'text', text: 'Assistant message' }
      ];

      render(<MultimodalContentRenderer content={content} role="assistant" />);

      const textRenderer = screen.getByTestId('text-content-renderer');
      expect(textRenderer).toHaveAttribute('data-role', 'assistant');
    });

    it('should pass system role to TextContentRenderer', () => {
      const content: MessageContentBlock[] = [
        { type: 'text', text: 'System message' }
      ];

      render(<MultimodalContentRenderer content={content} role="system" />);

      const textRenderer = screen.getByTestId('text-content-renderer');
      expect(textRenderer).toHaveAttribute('data-role', 'system');
    });

    it('should pass role to all text blocks consistently', () => {
      const content: MessageContentBlock[] = [
        { type: 'text', text: 'First' },
        { type: 'text', text: 'Second' },
        { type: 'text', text: 'Third' }
      ];

      render(<MultimodalContentRenderer content={content} role="assistant" />);

      const textRenderers = screen.getAllByTestId('text-content-renderer');
      textRenderers.forEach(renderer => {
        expect(renderer).toHaveAttribute('data-role', 'assistant');
      });
    });

    it('should maintain role across mixed content', () => {
      const content: MessageContentBlock[] = [
        { type: 'text', text: 'Text 1' },
        {
          type: 'image',
          source: { type: 'url', data: 'https://example.com/image.jpg' }
        },
        { type: 'text', text: 'Text 2' }
      ];

      render(<MultimodalContentRenderer content={content} role="user" />);

      const textRenderers = screen.getAllByTestId('text-content-renderer');
      expect(textRenderers).toHaveLength(2);
      textRenderers.forEach(renderer => {
        expect(renderer).toHaveAttribute('data-role', 'user');
      });
    });
  });

  describe('Edge Cases', () => {
    it('should handle unknown block types gracefully', () => {
      const content = [
        { type: 'text', text: 'Valid text' },
        { type: 'unknown', data: 'something' } as any,
        { type: 'text', text: 'More valid text' }
      ];

      render(<MultimodalContentRenderer content={content} role="user" />);

      // Should render valid blocks
      const textRenderers = screen.getAllByTestId('text-content-renderer');
      expect(textRenderers).toHaveLength(2);

      // Unknown block should return null (not rendered)
      expect(screen.queryByText('unknown')).not.toBeInTheDocument();
    });

    it('should handle blocks with missing properties', () => {
      const content = [
        { type: 'text' } as any, // Missing text property
        { type: 'text', text: 'Valid text' }
      ];

      render(<MultimodalContentRenderer content={content} role="user" />);

      // Should attempt to render both
      const textRenderers = screen.getAllByTestId('text-content-renderer');
      expect(textRenderers.length).toBeGreaterThanOrEqual(1);
    });

    it('should handle image blocks with malformed source', () => {
      const content = [
        { type: 'text', text: 'Before image' },
        {
          type: 'image',
          source: {} as any // Malformed source
        },
        { type: 'text', text: 'After image' }
      ];

      render(<MultimodalContentRenderer content={content} role="user" />);

      // Text blocks should still render
      const textRenderers = screen.getAllByTestId('text-content-renderer');
      expect(textRenderers).toHaveLength(2);
    });

    it('should handle null blocks in array', () => {
      const content = [
        { type: 'text', text: 'First' },
        null as any,
        { type: 'text', text: 'Second' }
      ];

      render(<MultimodalContentRenderer content={content} role="user" />);

      // Valid blocks should render
      const textRenderers = screen.getAllByTestId('text-content-renderer');
      expect(textRenderers).toHaveLength(2);
    });

    it('should handle undefined blocks in array', () => {
      const content = [
        { type: 'text', text: 'First' },
        undefined as any,
        { type: 'text', text: 'Second' }
      ];

      render(<MultimodalContentRenderer content={content} role="user" />);

      const textRenderers = screen.getAllByTestId('text-content-renderer');
      expect(textRenderers).toHaveLength(2);
    });

    it('should use index as key for blocks', () => {
      const content: MessageContentBlock[] = [
        { type: 'text', text: 'Block 1' },
        { type: 'text', text: 'Block 2' }
      ];

      // Multiple renders should work without key warnings
      const { rerender } = render(
        <MultimodalContentRenderer content={content} role="user" />
      );

      rerender(<MultimodalContentRenderer content={content} role="user" />);
      rerender(<MultimodalContentRenderer content={content} role="user" />);

      const textRenderers = screen.getAllByTestId('text-content-renderer');
      expect(textRenderers).toHaveLength(2);
    });

    it('should handle rapid content updates', () => {
      const content1: MessageContentBlock[] = [
        { type: 'text', text: 'Content 1' }
      ];

      const content2: MessageContentBlock[] = [
        { type: 'text', text: 'Content 2' }
      ];

      const { rerender } = render(
        <MultimodalContentRenderer content={content1} role="user" />
      );

      rerender(<MultimodalContentRenderer content={content2} role="user" />);

      const textRenderer = screen.getByTestId('text-content-renderer');
      expect(textRenderer).toHaveAttribute('data-content', 'Content 2');
    });

    it('should handle switching between different content structures', () => {
      const textOnly: MessageContentBlock[] = [
        { type: 'text', text: 'Text only' }
      ];

      const imageOnly: MessageContentBlock[] = [
        {
          type: 'image',
          source: { type: 'url', data: 'https://example.com/image.jpg' }
        }
      ];

      const { rerender } = render(
        <MultimodalContentRenderer content={textOnly} role="user" />
      );

      expect(screen.getByTestId('text-content-renderer')).toBeInTheDocument();
      expect(screen.queryByTestId('image-content-renderer')).not.toBeInTheDocument();

      rerender(<MultimodalContentRenderer content={imageOnly} role="user" />);

      expect(screen.queryByTestId('text-content-renderer')).not.toBeInTheDocument();
      expect(screen.getByTestId('image-content-renderer')).toBeInTheDocument();
    });
  });

  describe('Custom ClassName', () => {
    it('should apply custom className to container', () => {
      const content: MessageContentBlock[] = [
        { type: 'text', text: 'Test' }
      ];

      const { container } = render(
        <MultimodalContentRenderer 
          content={content} 
          role="user"
          className="custom-class"
        />
      );

      const wrapper = container.querySelector('.flex.flex-col.gap-3');
      expect(wrapper).toHaveClass('custom-class');
    });

    it('should merge custom className with default classes', () => {
      const content: MessageContentBlock[] = [
        { type: 'text', text: 'Test' }
      ];

      const { container } = render(
        <MultimodalContentRenderer 
          content={content} 
          role="user"
          className="my-4 p-2"
        />
      );

      const wrapper = container.querySelector('.flex.flex-col.gap-3');
      expect(wrapper).toHaveClass('my-4', 'p-2');
      expect(wrapper).toHaveClass('flex', 'flex-col', 'gap-3');
    });

    it('should handle undefined className', () => {
      const content: MessageContentBlock[] = [
        { type: 'text', text: 'Test' }
      ];

      const { container } = render(
        <MultimodalContentRenderer 
          content={content} 
          role="user"
          className={undefined}
        />
      );

      const wrapper = container.querySelector('.flex.flex-col.gap-3');
      expect(wrapper).toBeInTheDocument();
    });
  });

  describe('Component Structure', () => {
    it('should render correct container structure', () => {
      const content: MessageContentBlock[] = [
        { type: 'text', text: 'Test' }
      ];

      const { container } = render(
        <MultimodalContentRenderer content={content} role="user" />
      );

      const wrapper = container.querySelector('.flex.flex-col.gap-3');
      expect(wrapper).toBeInTheDocument();
      expect(wrapper?.tagName).toBe('DIV');
    });

    it('should contain all rendered blocks in main container', () => {
      const content: MessageContentBlock[] = [
        { type: 'text', text: 'Text 1' },
        {
          type: 'image',
          source: { type: 'url', data: 'https://example.com/image.jpg' }
        },
        { type: 'text', text: 'Text 2' }
      ];

      const { container } = render(
        <MultimodalContentRenderer content={content} role="user" />
      );

      const wrapper = container.querySelector('.flex.flex-col.gap-3');
      expect(wrapper?.children).toHaveLength(3);
    });

    it('should maintain structure with empty content', () => {
      const content: MessageContentBlock[] = [];

      const { container } = render(
        <MultimodalContentRenderer content={content} role="user" />
      );

      const wrapper = container.querySelector('.flex.flex-col.gap-3');
      expect(wrapper).toBeInTheDocument();
      expect(wrapper?.children).toHaveLength(0);
    });
  });

  describe('Performance and Rendering', () => {
    it('should handle large content arrays', () => {
      const content: MessageContentBlock[] = Array.from({ length: 50 }, (_, i) => ({
        type: 'text',
        text: `Block ${i + 1}`
      }));

      render(<MultimodalContentRenderer content={content} role="user" />);

      const textRenderers = screen.getAllByTestId('text-content-renderer');
      expect(textRenderers).toHaveLength(50);
    });

    it('should handle mixed content with many blocks', () => {
      const content: MessageContentBlock[] = [];
      for (let i = 0; i < 25; i++) {
        content.push({ type: 'text', text: `Text ${i}` });
        content.push({
          type: 'image',
          source: { type: 'url', data: `https://example.com/image${i}.jpg` }
        });
      }

      render(<MultimodalContentRenderer content={content} role="user" />);

      const textRenderers = screen.getAllByTestId('text-content-renderer');
      const imageRenderers = screen.getAllByTestId('image-content-renderer');

      expect(textRenderers).toHaveLength(25);
      expect(imageRenderers).toHaveLength(25);
    });

    it('should not crash with very long text content', () => {
      const veryLongText = 'A'.repeat(10000);
      const content: MessageContentBlock[] = [
        { type: 'text', text: veryLongText }
      ];

      expect(() => {
        render(<MultimodalContentRenderer content={content} role="user" />);
      }).not.toThrow();

      const textRenderer = screen.getByTestId('text-content-renderer');
      expect(textRenderer).toHaveAttribute('data-content', veryLongText);
    });
  });
});
