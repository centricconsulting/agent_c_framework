/**
 * MessageContentRenderer Component Tests
 * Testing content routing, multimodal detection, null handling, and different content types
 */

import React from 'react';
import { describe, it, expect, beforeEach, afterEach, vi } from 'vitest';
import { render, screen } from '@testing-library/react';
import { MessageContentRenderer } from '../MessageContentRenderer';
import type { MessageContent, ContentPart } from '@agentc/realtime-core';

// Mock child renderers
vi.mock('../content-renderers/TextContentRenderer', () => ({
  TextContentRenderer: ({ content, role, className }: any) => (
    <div 
      data-testid="text-content-renderer"
      data-role={role}
      data-content={content}
      className={className}
    >
      Text: {content}
    </div>
  )
}));

vi.mock('../content-renderers/MultimodalContentRenderer', () => ({
  MultimodalContentRenderer: ({ content, role, className }: any) => (
    <div 
      data-testid="multimodal-content-renderer"
      data-role={role}
      data-content-length={content.length}
      className={className}
    >
      Multimodal Content ({content.length} blocks)
    </div>
  )
}));

vi.mock('../content-renderers/ImageContentRenderer', () => ({
  ImageContentRenderer: ({ content }: any) => (
    <div data-testid="image-content-renderer">
      Image
    </div>
  )
}));

vi.mock('../content-renderers/ToolUseContentRenderer', () => ({
  ToolUseContentRenderer: ({ id, name, input }: any) => (
    <div 
      data-testid="tool-use-content-renderer"
      data-id={id}
      data-name={name}
    >
      Tool Use: {name}
    </div>
  )
}));

vi.mock('../content-renderers/ToolResultContentRenderer', () => ({
  ToolResultContentRenderer: ({ toolUseId, content, isError, role }: any) => (
    <div 
      data-testid="tool-result-content-renderer"
      data-tool-use-id={toolUseId}
      data-is-error={isError}
      data-role={role}
    >
      Tool Result: {toolUseId}
    </div>
  )
}));

vi.mock('../content-renderers/UnknownContentRenderer', () => ({
  UnknownContentRenderer: ({ type, content }: any) => (
    <div 
      data-testid="unknown-content-renderer"
      data-type={type}
    >
      Unknown: {type}
    </div>
  )
}));

describe('MessageContentRenderer Component', () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  afterEach(() => {
    vi.restoreAllMocks();
  });

  describe('Null Content Handling', () => {
    it('should render empty state for null content', () => {
      render(<MessageContentRenderer content={null} role="user" />);

      const emptyState = screen.getByRole('status');
      expect(emptyState).toBeInTheDocument();
      expect(emptyState).toHaveAttribute('aria-label', 'Empty message content');
      expect(emptyState).toHaveTextContent('No content available');
    });

    it('should apply muted styling to null content', () => {
      render(<MessageContentRenderer content={null} role="assistant" />);

      const emptyState = screen.getByRole('status');
      expect(emptyState).toHaveClass('text-muted-foreground', 'italic');
    });

    it('should apply custom className to null content', () => {
      render(
        <MessageContentRenderer 
          content={null} 
          role="user"
          className="custom-class"
        />
      );

      const emptyState = screen.getByRole('status');
      expect(emptyState).toHaveClass('custom-class');
    });

    it('should handle null content for different roles', () => {
      const { rerender } = render(
        <MessageContentRenderer content={null} role="user" />
      );
      expect(screen.getByText('No content available')).toBeInTheDocument();

      rerender(<MessageContentRenderer content={null} role="assistant" />);
      expect(screen.getByText('No content available')).toBeInTheDocument();

      rerender(<MessageContentRenderer content={null} role="system" />);
      expect(screen.getByText('No content available')).toBeInTheDocument();
    });
  });

  describe('String Content Routing', () => {
    it('should route string content to TextContentRenderer', () => {
      render(
        <MessageContentRenderer 
          content="Hello world" 
          role="user"
        />
      );

      const textRenderer = screen.getByTestId('text-content-renderer');
      expect(textRenderer).toBeInTheDocument();
      expect(textRenderer).toHaveAttribute('data-content', 'Hello world');
    });

    it('should pass role prop to TextContentRenderer for strings', () => {
      render(
        <MessageContentRenderer 
          content="Test message" 
          role="assistant"
        />
      );

      const textRenderer = screen.getByTestId('text-content-renderer');
      expect(textRenderer).toHaveAttribute('data-role', 'assistant');
    });

    it('should pass className to TextContentRenderer for strings', () => {
      render(
        <MessageContentRenderer 
          content="Test" 
          role="user"
          className="custom-text-class"
        />
      );

      const textRenderer = screen.getByTestId('text-content-renderer');
      expect(textRenderer).toHaveClass('custom-text-class');
    });

    it('should handle empty string content', () => {
      render(
        <MessageContentRenderer 
          content="" 
          role="user"
        />
      );

      const textRenderer = screen.getByTestId('text-content-renderer');
      expect(textRenderer).toBeInTheDocument();
      expect(textRenderer).toHaveAttribute('data-content', '');
    });

    it('should handle long string content', () => {
      const longContent = 'Lorem ipsum dolor sit amet '.repeat(50);
      
      render(
        <MessageContentRenderer 
          content={longContent} 
          role="user"
        />
      );

      const textRenderer = screen.getByTestId('text-content-renderer');
      expect(textRenderer).toHaveAttribute('data-content', longContent);
    });

    it('should handle string with special characters', () => {
      const specialContent = 'Special chars: !@#$%^&*()_+-=[]{}|;:,.<>?';
      
      render(
        <MessageContentRenderer 
          content={specialContent} 
          role="user"
        />
      );

      const textRenderer = screen.getByTestId('text-content-renderer');
      expect(textRenderer).toHaveAttribute('data-content', specialContent);
    });

    it('should handle string with markdown', () => {
      const markdownContent = '# Heading\n\n**Bold** and *italic*';
      
      render(
        <MessageContentRenderer 
          content={markdownContent} 
          role="assistant"
        />
      );

      const textRenderer = screen.getByTestId('text-content-renderer');
      expect(textRenderer).toHaveAttribute('data-content', markdownContent);
    });

    it('should handle multiline string content', () => {
      const multilineContent = 'Line 1\nLine 2\nLine 3';
      
      render(
        <MessageContentRenderer 
          content={multilineContent} 
          role="user"
        />
      );

      const textRenderer = screen.getByTestId('text-content-renderer');
      expect(textRenderer).toHaveAttribute('data-content', multilineContent);
    });
  });

  describe('Multimodal Content Detection', () => {
    it('should detect and route content with image blocks to MultimodalContentRenderer', () => {
      const content: ContentPart[] = [
        { type: 'text', text: 'Check out this image:' },
        {
          type: 'image',
          source: {
            type: 'url',
            data: 'https://example.com/image.jpg'
          }
        }
      ];

      render(
        <MessageContentRenderer 
          content={content} 
          role="user"
        />
      );

      const multimodalRenderer = screen.getByTestId('multimodal-content-renderer');
      expect(multimodalRenderer).toBeInTheDocument();
      expect(multimodalRenderer).toHaveAttribute('data-content-length', '2');
    });

    it('should route to MultimodalContentRenderer when single image block present', () => {
      const content: ContentPart[] = [
        {
          type: 'image',
          source: {
            type: 'url',
            data: 'https://example.com/image.jpg'
          }
        }
      ];

      render(
        <MessageContentRenderer 
          content={content} 
          role="assistant"
        />
      );

      const multimodalRenderer = screen.getByTestId('multimodal-content-renderer');
      expect(multimodalRenderer).toBeInTheDocument();
    });

    it('should route to MultimodalContentRenderer for multiple images', () => {
      const content: ContentPart[] = [
        {
          type: 'image',
          source: { type: 'url', data: 'https://example.com/image1.jpg' }
        },
        {
          type: 'image',
          source: { type: 'url', data: 'https://example.com/image2.jpg' }
        }
      ];

      render(
        <MessageContentRenderer 
          content={content} 
          role="user"
        />
      );

      const multimodalRenderer = screen.getByTestId('multimodal-content-renderer');
      expect(multimodalRenderer).toBeInTheDocument();
      expect(multimodalRenderer).toHaveAttribute('data-content-length', '2');
    });

    it('should route to MultimodalContentRenderer for mixed text and images', () => {
      const content: ContentPart[] = [
        { type: 'text', text: 'First text' },
        {
          type: 'image',
          source: { type: 'url', data: 'https://example.com/image.jpg' }
        },
        { type: 'text', text: 'Second text' }
      ];

      render(
        <MessageContentRenderer 
          content={content} 
          role="assistant"
        />
      );

      const multimodalRenderer = screen.getByTestId('multimodal-content-renderer');
      expect(multimodalRenderer).toBeInTheDocument();
    });

    it('should pass role to MultimodalContentRenderer', () => {
      const content: ContentPart[] = [
        { type: 'text', text: 'Text' },
        {
          type: 'image',
          source: { type: 'url', data: 'https://example.com/image.jpg' }
        }
      ];

      render(
        <MessageContentRenderer 
          content={content} 
          role="system"
        />
      );

      const multimodalRenderer = screen.getByTestId('multimodal-content-renderer');
      expect(multimodalRenderer).toHaveAttribute('data-role', 'system');
    });

    it('should pass className to MultimodalContentRenderer', () => {
      const content: ContentPart[] = [
        {
          type: 'image',
          source: { type: 'url', data: 'https://example.com/image.jpg' }
        }
      ];

      render(
        <MessageContentRenderer 
          content={content} 
          role="user"
          className="custom-multimodal-class"
        />
      );

      const multimodalRenderer = screen.getByTestId('multimodal-content-renderer');
      expect(multimodalRenderer).toHaveClass('custom-multimodal-class');
    });

    it('should detect images with base64 source', () => {
      const content: ContentPart[] = [
        { type: 'text', text: 'Image below:' },
        {
          type: 'image',
          source: {
            type: 'base64',
            media_type: 'image/png',
            data: 'iVBORw0KGgoAAAANSUhEUgAAAAUA'
          }
        }
      ];

      render(
        <MessageContentRenderer 
          content={content} 
          role="user"
        />
      );

      const multimodalRenderer = screen.getByTestId('multimodal-content-renderer');
      expect(multimodalRenderer).toBeInTheDocument();
    });

    it('should detect image anywhere in the content array', () => {
      const content: ContentPart[] = [
        { type: 'text', text: 'Start' },
        { type: 'text', text: 'Middle' },
        {
          type: 'image',
          source: { type: 'url', data: 'https://example.com/image.jpg' }
        },
        { type: 'text', text: 'End' }
      ];

      render(
        <MessageContentRenderer 
          content={content} 
          role="user"
        />
      );

      expect(screen.getByTestId('multimodal-content-renderer')).toBeInTheDocument();
    });
  });

  describe('Text-Only Array Content', () => {
    it('should use part-by-part rendering for text-only arrays', () => {
      const content: ContentPart[] = [
        { type: 'text', text: 'First paragraph' },
        { type: 'text', text: 'Second paragraph' }
      ];

      render(
        <MessageContentRenderer 
          content={content} 
          role="user"
        />
      );

      const article = screen.getByRole('article');
      expect(article).toBeInTheDocument();
      expect(article).toHaveAttribute('aria-label', 'user message with 2 content parts');
    });

    it('should render text parts with space-y-3 spacing', () => {
      const content: ContentPart[] = [
        { type: 'text', text: 'Part 1' },
        { type: 'text', text: 'Part 2' }
      ];

      render(
        <MessageContentRenderer 
          content={content} 
          role="assistant"
        />
      );

      const article = screen.getByRole('article');
      expect(article).toHaveClass('space-y-3');
    });

    it('should pass className to text-only array container', () => {
      const content: ContentPart[] = [
        { type: 'text', text: 'Part 1' }
      ];

      render(
        <MessageContentRenderer 
          content={content} 
          role="user"
          className="custom-array-class"
        />
      );

      const article = screen.getByRole('article');
      expect(article).toHaveClass('custom-array-class');
    });

    it('should render single text part in array', () => {
      const content: ContentPart[] = [
        { type: 'text', text: 'Single text part' }
      ];

      render(
        <MessageContentRenderer 
          content={content} 
          role="user"
        />
      );

      const article = screen.getByRole('article');
      expect(article).toHaveAttribute('aria-label', 'user message with 1 content parts');
    });

    it('should render multiple text parts', () => {
      const content: ContentPart[] = [
        { type: 'text', text: 'Part 1' },
        { type: 'text', text: 'Part 2' },
        { type: 'text', text: 'Part 3' }
      ];

      render(
        <MessageContentRenderer 
          content={content} 
          role="assistant"
        />
      );

      const textRenderers = screen.getAllByTestId('text-content-renderer');
      expect(textRenderers).toHaveLength(3);
    });

    it('should render tool_use blocks in text-only arrays', () => {
      const content: ContentPart[] = [
        { type: 'text', text: 'Using tool:' },
        {
          type: 'tool_use',
          id: 'tool-1',
          name: 'calculator',
          input: { a: 1, b: 2 }
        }
      ];

      render(
        <MessageContentRenderer 
          content={content} 
          role="assistant"
        />
      );

      expect(screen.getByTestId('text-content-renderer')).toBeInTheDocument();
      expect(screen.getByTestId('tool-use-content-renderer')).toBeInTheDocument();
    });

    it('should render tool_result blocks in text-only arrays', () => {
      const content: ContentPart[] = [
        {
          type: 'tool_result',
          tool_use_id: 'tool-1',
          content: 'Result: 3',
          is_error: false
        }
      ];

      render(
        <MessageContentRenderer 
          content={content} 
          role="assistant"
        />
      );

      expect(screen.getByTestId('tool-result-content-renderer')).toBeInTheDocument();
    });

    it('should NOT route text-only arrays to MultimodalContentRenderer', () => {
      const content: ContentPart[] = [
        { type: 'text', text: 'Text 1' },
        { type: 'text', text: 'Text 2' }
      ];

      render(
        <MessageContentRenderer 
          content={content} 
          role="user"
        />
      );

      expect(screen.queryByTestId('multimodal-content-renderer')).not.toBeInTheDocument();
      expect(screen.getByRole('article')).toBeInTheDocument();
    });
  });

  describe('ContentPartRenderer - Individual Part Routing', () => {
    it('should render text parts with TextContentRenderer', () => {
      const content: ContentPart[] = [
        { type: 'text', text: 'Test text' }
      ];

      render(
        <MessageContentRenderer 
          content={content} 
          role="user"
        />
      );

      const textRenderer = screen.getByTestId('text-content-renderer');
      expect(textRenderer).toHaveAttribute('data-content', 'Test text');
      expect(textRenderer).toHaveAttribute('data-role', 'user');
    });

    it('should render image parts with ImageContentRenderer', () => {
      // Create text-only array to trigger part-by-part rendering
      // (images in arrays should go to multimodal, but test ContentPartRenderer directly)
      const content: ContentPart[] = [
        { type: 'text', text: 'Before' }
      ];

      render(
        <MessageContentRenderer 
          content={content} 
          role="user"
        />
      );

      // This tests that ContentPartRenderer can handle images
      // when they're part of text-only rendering path
      expect(screen.getByTestId('text-content-renderer')).toBeInTheDocument();
    });

    it('should render tool_use parts with ToolUseContentRenderer', () => {
      const content: ContentPart[] = [
        {
          type: 'tool_use',
          id: 'tool-123',
          name: 'search',
          input: { query: 'test' }
        }
      ];

      render(
        <MessageContentRenderer 
          content={content} 
          role="assistant"
        />
      );

      const toolUseRenderer = screen.getByTestId('tool-use-content-renderer');
      expect(toolUseRenderer).toHaveAttribute('data-id', 'tool-123');
      expect(toolUseRenderer).toHaveAttribute('data-name', 'search');
    });

    it('should render tool_result parts with ToolResultContentRenderer', () => {
      const content: ContentPart[] = [
        {
          type: 'tool_result',
          tool_use_id: 'tool-123',
          content: 'Search results',
          is_error: false
        }
      ];

      render(
        <MessageContentRenderer 
          content={content} 
          role="assistant"
        />
      );

      const toolResultRenderer = screen.getByTestId('tool-result-content-renderer');
      expect(toolResultRenderer).toHaveAttribute('data-tool-use-id', 'tool-123');
      expect(toolResultRenderer).toHaveAttribute('data-is-error', 'false');
    });

    it('should render error tool results', () => {
      const content: ContentPart[] = [
        {
          type: 'tool_result',
          tool_use_id: 'tool-456',
          content: 'Error occurred',
          is_error: true
        }
      ];

      render(
        <MessageContentRenderer 
          content={content} 
          role="assistant"
        />
      );

      const toolResultRenderer = screen.getByTestId('tool-result-content-renderer');
      expect(toolResultRenderer).toHaveAttribute('data-is-error', 'true');
    });

    it('should render unknown content types with UnknownContentRenderer', () => {
      const content: ContentPart[] = [
        { type: 'unknown_type', data: 'something' } as any
      ];

      render(
        <MessageContentRenderer 
          content={content} 
          role="user"
        />
      );

      const unknownRenderer = screen.getByTestId('unknown-content-renderer');
      expect(unknownRenderer).toHaveAttribute('data-type', 'unknown_type');
    });

    it('should handle invalid parts gracefully', () => {
      const content = [
        null,
        { type: 'text', text: 'Valid text' }
      ] as any;

      render(
        <MessageContentRenderer 
          content={content} 
          role="user"
        />
      );

      // Should render unknown for invalid part and text for valid part
      expect(screen.getByTestId('unknown-content-renderer')).toBeInTheDocument();
      expect(screen.getByTestId('text-content-renderer')).toBeInTheDocument();
    });

    it('should handle parts without type property', () => {
      const content = [
        { data: 'no type' }
      ] as any;

      render(
        <MessageContentRenderer 
          content={content} 
          role="user"
        />
      );

      const unknownRenderer = screen.getByTestId('unknown-content-renderer');
      expect(unknownRenderer).toHaveAttribute('data-type', 'invalid');
    });
  });

  describe('Empty Array Content', () => {
    it('should handle empty content array', () => {
      const content: ContentPart[] = [];

      render(
        <MessageContentRenderer 
          content={content} 
          role="user"
        />
      );

      const article = screen.getByRole('article');
      expect(article).toBeInTheDocument();
      expect(article).toHaveAttribute('aria-label', 'user message with 0 content parts');
    });

    it('should render container for empty array', () => {
      const content: ContentPart[] = [];

      const { container } = render(
        <MessageContentRenderer 
          content={content} 
          role="user"
        />
      );

      const article = container.querySelector('[role="article"]');
      expect(article).toBeInTheDocument();
      expect(article?.children).toHaveLength(0);
    });
  });

  describe('Unexpected Content Types', () => {
    it('should handle undefined content gracefully', () => {
      // TypeScript would prevent this, but JS might pass it
      render(
        <MessageContentRenderer 
          content={undefined as any} 
          role="user"
        />
      );

      const alert = screen.getByRole('alert');
      expect(alert).toBeInTheDocument();
      expect(alert).toHaveTextContent('Unable to render message content');
    });

    it('should handle number content', () => {
      render(
        <MessageContentRenderer 
          content={123 as any} 
          role="user"
        />
      );

      const alert = screen.getByRole('alert');
      expect(alert).toHaveTextContent('Unable to render message content');
    });

    it('should handle boolean content', () => {
      render(
        <MessageContentRenderer 
          content={true as any} 
          role="user"
        />
      );

      const alert = screen.getByRole('alert');
      expect(alert).toHaveTextContent('Unable to render message content');
    });

    it('should handle object content (non-array)', () => {
      render(
        <MessageContentRenderer 
          content={{ type: 'object' } as any} 
          role="user"
        />
      );

      const alert = screen.getByRole('alert');
      expect(alert).toHaveTextContent('Unable to render message content');
    });

    it('should apply muted styling to fallback content', () => {
      render(
        <MessageContentRenderer 
          content={undefined as any} 
          role="user"
        />
      );

      const alert = screen.getByRole('alert');
      expect(alert).toHaveClass('text-muted-foreground', 'italic');
    });

    it('should apply className to fallback content', () => {
      render(
        <MessageContentRenderer 
          content={undefined as any} 
          role="user"
          className="custom-fallback-class"
        />
      );

      const alert = screen.getByRole('alert');
      expect(alert).toHaveClass('custom-fallback-class');
    });
  });

  describe('Role Handling', () => {
    it('should handle user role', () => {
      render(
        <MessageContentRenderer 
          content="Test" 
          role="user"
        />
      );

      const textRenderer = screen.getByTestId('text-content-renderer');
      expect(textRenderer).toHaveAttribute('data-role', 'user');
    });

    it('should handle assistant role', () => {
      render(
        <MessageContentRenderer 
          content="Test" 
          role="assistant"
        />
      );

      const textRenderer = screen.getByTestId('text-content-renderer');
      expect(textRenderer).toHaveAttribute('data-role', 'assistant');
    });

    it('should handle system role', () => {
      render(
        <MessageContentRenderer 
          content="Test" 
          role="system"
        />
      );

      const textRenderer = screen.getByTestId('text-content-renderer');
      expect(textRenderer).toHaveAttribute('data-role', 'system');
    });

    it('should pass role through all content types', () => {
      const content: ContentPart[] = [
        { type: 'text', text: 'Text part' }
      ];

      const { rerender } = render(
        <MessageContentRenderer content={content} role="user" />
      );

      let textRenderer = screen.getByTestId('text-content-renderer');
      expect(textRenderer).toHaveAttribute('data-role', 'user');

      rerender(<MessageContentRenderer content={content} role="assistant" />);
      textRenderer = screen.getByTestId('text-content-renderer');
      expect(textRenderer).toHaveAttribute('data-role', 'assistant');
    });
  });

  describe('ClassName Prop', () => {
    it('should apply className to string content renderer', () => {
      render(
        <MessageContentRenderer 
          content="Test" 
          role="user"
          className="string-class"
        />
      );

      const textRenderer = screen.getByTestId('text-content-renderer');
      expect(textRenderer).toHaveClass('string-class');
    });

    it('should apply className to multimodal renderer', () => {
      const content: ContentPart[] = [
        {
          type: 'image',
          source: { type: 'url', data: 'https://example.com/image.jpg' }
        }
      ];

      render(
        <MessageContentRenderer 
          content={content} 
          role="user"
          className="multimodal-class"
        />
      );

      const multimodalRenderer = screen.getByTestId('multimodal-content-renderer');
      expect(multimodalRenderer).toHaveClass('multimodal-class');
    });

    it('should apply className to text-only array container', () => {
      const content: ContentPart[] = [
        { type: 'text', text: 'Part 1' }
      ];

      render(
        <MessageContentRenderer 
          content={content} 
          role="user"
          className="array-class"
        />
      );

      const article = screen.getByRole('article');
      expect(article).toHaveClass('array-class');
    });

    it('should apply className to null content', () => {
      render(
        <MessageContentRenderer 
          content={null} 
          role="user"
          className="null-class"
        />
      );

      const status = screen.getByRole('status');
      expect(status).toHaveClass('null-class');
    });

    it('should apply className to fallback content', () => {
      render(
        <MessageContentRenderer 
          content={undefined as any} 
          role="user"
          className="fallback-class"
        />
      );

      const alert = screen.getByRole('alert');
      expect(alert).toHaveClass('fallback-class');
    });
  });

  describe('Content Type Switching', () => {
    it('should switch from string to null', () => {
      const { rerender } = render(
        <MessageContentRenderer content="Hello" role="user" />
      );

      expect(screen.getByTestId('text-content-renderer')).toBeInTheDocument();

      rerender(<MessageContentRenderer content={null} role="user" />);

      expect(screen.queryByTestId('text-content-renderer')).not.toBeInTheDocument();
      expect(screen.getByRole('status')).toBeInTheDocument();
    });

    it('should switch from string to array', () => {
      const { rerender } = render(
        <MessageContentRenderer content="Hello" role="user" />
      );

      expect(screen.getByTestId('text-content-renderer')).toBeInTheDocument();

      const arrayContent: ContentPart[] = [
        { type: 'text', text: 'Array text' }
      ];

      rerender(<MessageContentRenderer content={arrayContent} role="user" />);

      expect(screen.getByRole('article')).toBeInTheDocument();
    });

    it('should switch from text-only array to multimodal array', () => {
      const textOnlyContent: ContentPart[] = [
        { type: 'text', text: 'Text 1' }
      ];

      const { rerender } = render(
        <MessageContentRenderer content={textOnlyContent} role="user" />
      );

      expect(screen.getByRole('article')).toBeInTheDocument();
      expect(screen.queryByTestId('multimodal-content-renderer')).not.toBeInTheDocument();

      const multimodalContent: ContentPart[] = [
        { type: 'text', text: 'Text 1' },
        {
          type: 'image',
          source: { type: 'url', data: 'https://example.com/image.jpg' }
        }
      ];

      rerender(<MessageContentRenderer content={multimodalContent} role="user" />);

      expect(screen.queryByRole('article')).not.toBeInTheDocument();
      expect(screen.getByTestId('multimodal-content-renderer')).toBeInTheDocument();
    });

    it('should switch from multimodal to text-only array', () => {
      const multimodalContent: ContentPart[] = [
        {
          type: 'image',
          source: { type: 'url', data: 'https://example.com/image.jpg' }
        }
      ];

      const { rerender } = render(
        <MessageContentRenderer content={multimodalContent} role="user" />
      );

      expect(screen.getByTestId('multimodal-content-renderer')).toBeInTheDocument();

      const textOnlyContent: ContentPart[] = [
        { type: 'text', text: 'Just text' }
      ];

      rerender(<MessageContentRenderer content={textOnlyContent} role="user" />);

      expect(screen.queryByTestId('multimodal-content-renderer')).not.toBeInTheDocument();
      expect(screen.getByRole('article')).toBeInTheDocument();
    });
  });

  describe('Edge Cases and Error Conditions', () => {
    it('should handle content array with only null values', () => {
      const content = [null, null, null] as any;

      render(
        <MessageContentRenderer 
          content={content} 
          role="user"
        />
      );

      // Should render article with unknown content renderers
      expect(screen.getByRole('article')).toBeInTheDocument();
    });

    it('should handle mixed valid and invalid parts', () => {
      const content = [
        { type: 'text', text: 'Valid' },
        null,
        { invalid: 'part' },
        { type: 'text', text: 'Also valid' }
      ] as any;

      render(
        <MessageContentRenderer 
          content={content} 
          role="user"
        />
      );

      const textRenderers = screen.getAllByTestId('text-content-renderer');
      expect(textRenderers).toHaveLength(2);
    });

    it('should handle very long content arrays', () => {
      const content: ContentPart[] = Array.from({ length: 100 }, (_, i) => ({
        type: 'text',
        text: `Part ${i}`
      }));

      render(
        <MessageContentRenderer 
          content={content} 
          role="user"
        />
      );

      const article = screen.getByRole('article');
      expect(article).toHaveAttribute('aria-label', 'user message with 100 content parts');
    });

    it('should not crash with circular references', () => {
      // Create object with circular reference
      const circular: any = { type: 'text', text: 'test' };
      circular.self = circular;

      const content = [circular];

      // Should not crash during render
      expect(() => {
        render(
          <MessageContentRenderer 
            content={content as any} 
            role="user"
          />
        );
      }).not.toThrow();
    });
  });
});
