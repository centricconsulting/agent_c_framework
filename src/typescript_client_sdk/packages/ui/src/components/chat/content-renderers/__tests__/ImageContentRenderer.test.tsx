/**
 * ImageContentRenderer Component Tests
 * Testing image rendering, loading states, error handling, lightbox integration, and accessibility
 */

import React from 'react';
import { describe, it, expect, beforeEach, afterEach, vi } from 'vitest';
import { render, screen, waitFor, fireEvent } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { ImageContentRenderer } from '../ImageContentRenderer';
import type { ImageContentBlock } from '@agentc/realtime-react';

// Mock ImageLightbox component
vi.mock('../../ImageLightbox', () => ({
  ImageLightbox: ({ src, isOpen, onClose }: any) => 
    isOpen ? (
      <div 
        data-testid="image-lightbox"
        data-src={src}
        onClick={onClose}
      >
        Lightbox Open
      </div>
    ) : null
}));

// Mock Lucide React icons
vi.mock('lucide-react', () => ({
  Loader2: ({ className }: any) => (
    <div 
      data-testid="loader-icon" 
      className={className}
    >
      Loading...
    </div>
  ),
  ImageOff: ({ className }: any) => (
    <div 
      data-testid="image-off-icon" 
      className={className}
    >
      Image Error
    </div>
  )
}));

describe('ImageContentRenderer Component', () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  afterEach(() => {
    vi.restoreAllMocks();
  });

  describe('Rendering - URL Source', () => {
    it('should render image with URL source', async () => {
      const content: ImageContentBlock = {
        type: 'image',
        source: {
          type: 'url',
          data: 'https://example.com/image.jpg'
        }
      };

      render(<ImageContentRenderer content={content} />);

      // Wait for image to load
      const img = screen.getByRole('img');
      expect(img).toBeInTheDocument();
      expect(img).toHaveAttribute('src', 'https://example.com/image.jpg');
    });

    it('should render image with URL source and media_type', async () => {
      const content: ImageContentBlock = {
        type: 'image',
        source: {
          type: 'url',
          data: 'https://example.com/photo.png',
          media_type: 'image/png'
        }
      };

      render(<ImageContentRenderer content={content} />);

      const img = screen.getByRole('img');
      expect(img).toHaveAttribute('src', 'https://example.com/photo.png');
    });

    it('should handle URL with query parameters', async () => {
      const content: ImageContentBlock = {
        type: 'image',
        source: {
          type: 'url',
          data: 'https://example.com/image.jpg?size=large&quality=high'
        }
      };

      render(<ImageContentRenderer content={content} />);

      const img = screen.getByRole('img');
      expect(img).toHaveAttribute('src', 'https://example.com/image.jpg?size=large&quality=high');
    });

    it('should handle HTTPS URLs', async () => {
      const content: ImageContentBlock = {
        type: 'image',
        source: {
          type: 'url',
          data: 'https://secure.example.com/image.jpg'
        }
      };

      render(<ImageContentRenderer content={content} />);

      const img = screen.getByRole('img');
      expect(img).toHaveAttribute('src', 'https://secure.example.com/image.jpg');
    });

    it('should handle data URLs', async () => {
      const content: ImageContentBlock = {
        type: 'image',
        source: {
          type: 'url',
          data: 'data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAUA'
        }
      };

      render(<ImageContentRenderer content={content} />);

      const img = screen.getByRole('img');
      expect(img).toHaveAttribute('src', 'data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAUA');
    });
  });

  describe('Rendering - Base64 Source', () => {
    it('should render image with base64 source', async () => {
      const content: ImageContentBlock = {
        type: 'image',
        source: {
          type: 'base64',
          media_type: 'image/png',
          data: 'iVBORw0KGgoAAAANSUhEUgAAAAUA'
        }
      };

      render(<ImageContentRenderer content={content} />);

      const img = screen.getByRole('img');
      expect(img).toBeInTheDocument();
      expect(img).toHaveAttribute('src', 'data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAUA');
    });

    it('should render image with base64 JPEG source', async () => {
      const content: ImageContentBlock = {
        type: 'image',
        source: {
          type: 'base64',
          media_type: 'image/jpeg',
          data: '/9j/4AAQSkZJRgABAQAAAQABAAD'
        }
      };

      render(<ImageContentRenderer content={content} />);

      const img = screen.getByRole('img');
      expect(img).toHaveAttribute('src', 'data:image/jpeg;base64,/9j/4AAQSkZJRgABAQAAAQABAAD');
    });

    it('should render image with base64 GIF source', async () => {
      const content: ImageContentBlock = {
        type: 'image',
        source: {
          type: 'base64',
          media_type: 'image/gif',
          data: 'R0lGODlhAQABAIAAAP///wAAACH5BAEAAAAALAAAAAABAAEAAAICRAEAOw=='
        }
      };

      render(<ImageContentRenderer content={content} />);

      const img = screen.getByRole('img');
      expect(img).toHaveAttribute('src', 'data:image/gif;base64,R0lGODlhAQABAIAAAP///wAAACH5BAEAAAAALAAAAAABAAEAAAICRAEAOw==');
    });

    it('should render image with base64 WebP source', async () => {
      const content: ImageContentBlock = {
        type: 'image',
        source: {
          type: 'base64',
          media_type: 'image/webp',
          data: 'UklGRiQAAABXRUJQVlA4IBgAAAAwAQCdASoBAAEAAwA0JaQAA3AA/vuUAAA='
        }
      };

      render(<ImageContentRenderer content={content} />);

      const img = screen.getByRole('img');
      expect(img).toHaveAttribute('src', 'data:image/webp;base64,UklGRiQAAABXRUJQVlA4IBgAAAAwAQCdASoBAAEAAwA0JaQAA3AA/vuUAAA=');
    });
  });

  describe('Loading State', () => {
    it('should display loading spinner initially', () => {
      const content: ImageContentBlock = {
        type: 'image',
        source: {
          type: 'url',
          data: 'https://example.com/image.jpg'
        }
      };

      render(<ImageContentRenderer content={content} />);

      // Loader should be visible initially
      const loader = screen.getByTestId('loader-icon');
      expect(loader).toBeInTheDocument();
      expect(loader).toHaveClass('h-8', 'w-8', 'animate-spin', 'text-muted-foreground');
    });

    it('should hide loading spinner after image loads', async () => {
      const content: ImageContentBlock = {
        type: 'image',
        source: {
          type: 'url',
          data: 'https://example.com/image.jpg'
        }
      };

      render(<ImageContentRenderer content={content} />);

      const img = screen.getByRole('img');
      
      // Trigger load event
      fireEvent.load(img);

      // Wait for state update
      await waitFor(() => {
        expect(screen.queryByTestId('loader-icon')).not.toBeInTheDocument();
      });
    });

    it('should have loader in centered container', () => {
      const content: ImageContentBlock = {
        type: 'image',
        source: {
          type: 'url',
          data: 'https://example.com/image.jpg'
        }
      };

      const { container } = render(<ImageContentRenderer content={content} />);

      const loaderContainer = container.querySelector('.absolute.inset-0.flex.items-center.justify-center.bg-muted');
      expect(loaderContainer).toBeInTheDocument();
      
      const loader = screen.getByTestId('loader-icon');
      expect(loaderContainer).toContainElement(loader);
    });

    it('should set image opacity to 0 while loading', () => {
      const content: ImageContentBlock = {
        type: 'image',
        source: {
          type: 'url',
          data: 'https://example.com/image.jpg'
        }
      };

      render(<ImageContentRenderer content={content} />);

      const img = screen.getByRole('img');
      expect(img).toHaveClass('opacity-0');
    });

    it('should remove opacity-0 class after loading', async () => {
      const content: ImageContentBlock = {
        type: 'image',
        source: {
          type: 'url',
          data: 'https://example.com/image.jpg'
        }
      };

      render(<ImageContentRenderer content={content} />);

      const img = screen.getByRole('img');
      fireEvent.load(img);

      await waitFor(() => {
        expect(img).not.toHaveClass('opacity-0');
      });
    });
  });

  describe('Error State', () => {
    it('should display error when image fails to load', async () => {
      const content: ImageContentBlock = {
        type: 'image',
        source: {
          type: 'url',
          data: 'https://example.com/broken.jpg'
        }
      };

      render(<ImageContentRenderer content={content} />);

      const img = screen.getByRole('img');
      fireEvent.error(img);

      // Wait for error state
      await waitFor(() => {
        expect(screen.getByTestId('image-off-icon')).toBeInTheDocument();
        expect(screen.getByText('Failed to load image')).toBeInTheDocument();
      });
    });

    it('should have proper error styling', async () => {
      const content: ImageContentBlock = {
        type: 'image',
        source: {
          type: 'url',
          data: 'https://example.com/broken.jpg'
        }
      };

      const { container } = render(<ImageContentRenderer content={content} />);

      const img = screen.getByRole('img');
      fireEvent.error(img);

      await waitFor(() => {
        const errorContainer = container.querySelector('.flex.items-center.gap-2.p-3.rounded-lg.bg-muted.text-muted-foreground');
        expect(errorContainer).toBeInTheDocument();
      });
    });

    it('should display error message with proper accessibility', async () => {
      const content: ImageContentBlock = {
        type: 'image',
        source: {
          type: 'url',
          data: 'https://example.com/broken.jpg'
        }
      };

      render(<ImageContentRenderer content={content} />);

      const img = screen.getByRole('img');
      fireEvent.error(img);

      await waitFor(() => {
        const errorAlert = screen.getByRole('alert');
        expect(errorAlert).toBeInTheDocument();
        expect(errorAlert).toHaveTextContent('Failed to load image');
      });
    });

    it('should hide loading spinner on error', async () => {
      const content: ImageContentBlock = {
        type: 'image',
        source: {
          type: 'url',
          data: 'https://example.com/broken.jpg'
        }
      };

      render(<ImageContentRenderer content={content} />);

      const img = screen.getByRole('img');
      fireEvent.error(img);

      await waitFor(() => {
        expect(screen.queryByTestId('loader-icon')).not.toBeInTheDocument();
      });
    });

    it('should not display image after error', async () => {
      const content: ImageContentBlock = {
        type: 'image',
        source: {
          type: 'url',
          data: 'https://example.com/broken.jpg'
        }
      };

      render(<ImageContentRenderer content={content} />);

      const img = screen.getByRole('img');
      fireEvent.error(img);

      await waitFor(() => {
        expect(screen.queryByRole('img')).not.toBeInTheDocument();
      });
    });

    it('should display ImageOff icon on error', async () => {
      const content: ImageContentBlock = {
        type: 'image',
        source: {
          type: 'url',
          data: 'https://example.com/broken.jpg'
        }
      };

      render(<ImageContentRenderer content={content} />);

      const img = screen.getByRole('img');
      fireEvent.error(img);

      await waitFor(() => {
        const icon = screen.getByTestId('image-off-icon');
        expect(icon).toBeInTheDocument();
        expect(icon).toHaveClass('h-5', 'w-5');
      });
    });
  });

  describe('Lightbox Integration', () => {
    it('should open lightbox on image click when enabled', async () => {
      const content: ImageContentBlock = {
        type: 'image',
        source: {
          type: 'url',
          data: 'https://example.com/image.jpg'
        }
      };

      const user = userEvent.setup();
      render(<ImageContentRenderer content={content} enableLightbox={true} />);

      const img = screen.getByRole('img');
      fireEvent.load(img);

      await user.click(img);

      await waitFor(() => {
        expect(screen.getByTestId('image-lightbox')).toBeInTheDocument();
      });
    });

    it('should pass correct src to lightbox', async () => {
      const content: ImageContentBlock = {
        type: 'image',
        source: {
          type: 'url',
          data: 'https://example.com/image.jpg'
        }
      };

      const user = userEvent.setup();
      render(<ImageContentRenderer content={content} enableLightbox={true} />);

      const img = screen.getByRole('img');
      fireEvent.load(img);
      await user.click(img);

      await waitFor(() => {
        const lightbox = screen.getByTestId('image-lightbox');
        expect(lightbox).toHaveAttribute('data-src', 'https://example.com/image.jpg');
      });
    });

    it('should close lightbox when clicking it', async () => {
      const content: ImageContentBlock = {
        type: 'image',
        source: {
          type: 'url',
          data: 'https://example.com/image.jpg'
        }
      };

      const user = userEvent.setup();
      render(<ImageContentRenderer content={content} enableLightbox={true} />);

      const img = screen.getByRole('img');
      fireEvent.load(img);
      await user.click(img);

      const lightbox = await screen.findByTestId('image-lightbox');
      await user.click(lightbox);

      await waitFor(() => {
        expect(screen.queryByTestId('image-lightbox')).not.toBeInTheDocument();
      });
    });

    it('should not open lightbox when enableLightbox is false', async () => {
      const content: ImageContentBlock = {
        type: 'image',
        source: {
          type: 'url',
          data: 'https://example.com/image.jpg'
        }
      };

      const user = userEvent.setup();
      render(<ImageContentRenderer content={content} enableLightbox={false} />);

      const img = screen.getByRole('img');
      fireEvent.load(img);
      await user.click(img);

      // Lightbox should not appear
      expect(screen.queryByTestId('image-lightbox')).not.toBeInTheDocument();
    });

    it('should not render ImageLightbox component when disabled', () => {
      const content: ImageContentBlock = {
        type: 'image',
        source: {
          type: 'url',
          data: 'https://example.com/image.jpg'
        }
      };

      const { container } = render(<ImageContentRenderer content={content} enableLightbox={false} />);
      
      // ImageLightbox should not be in the tree at all
      expect(screen.queryByTestId('image-lightbox')).not.toBeInTheDocument();
    });

    it('should have cursor-pointer class when lightbox enabled', () => {
      const content: ImageContentBlock = {
        type: 'image',
        source: {
          type: 'url',
          data: 'https://example.com/image.jpg'
        }
      };

      render(<ImageContentRenderer content={content} enableLightbox={true} />);

      const img = screen.getByRole('img');
      expect(img).toHaveClass('cursor-pointer', 'hover:opacity-90', 'transition-opacity');
    });

    it('should not have cursor-pointer class when lightbox disabled', () => {
      const content: ImageContentBlock = {
        type: 'image',
        source: {
          type: 'url',
          data: 'https://example.com/image.jpg'
        }
      };

      render(<ImageContentRenderer content={content} enableLightbox={false} />);

      const img = screen.getByRole('img');
      expect(img).not.toHaveClass('cursor-pointer');
      expect(img).not.toHaveClass('hover:opacity-90');
    });

    it('should open lightbox for base64 images', async () => {
      const content: ImageContentBlock = {
        type: 'image',
        source: {
          type: 'base64',
          media_type: 'image/png',
          data: 'iVBORw0KGgoAAAANSUhEUgAAAAUA'
        }
      };

      const user = userEvent.setup();
      render(<ImageContentRenderer content={content} enableLightbox={true} />);

      const img = screen.getByRole('img');
      fireEvent.load(img);
      await user.click(img);

      await waitFor(() => {
        const lightbox = screen.getByTestId('image-lightbox');
        expect(lightbox).toHaveAttribute('data-src', 'data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAUA');
      });
    });
  });

  describe('Lazy Loading', () => {
    it('should have loading="lazy" attribute', () => {
      const content: ImageContentBlock = {
        type: 'image',
        source: {
          type: 'url',
          data: 'https://example.com/image.jpg'
        }
      };

      render(<ImageContentRenderer content={content} />);

      const img = screen.getByRole('img');
      expect(img).toHaveAttribute('loading', 'lazy');
    });

    it('should apply lazy loading to base64 images', () => {
      const content: ImageContentBlock = {
        type: 'image',
        source: {
          type: 'base64',
          media_type: 'image/png',
          data: 'iVBORw0KGgoAAAANSUhEUgAAAAUA'
        }
      };

      render(<ImageContentRenderer content={content} />);

      const img = screen.getByRole('img');
      expect(img).toHaveAttribute('loading', 'lazy');
    });
  });

  describe('Max Width', () => {
    it('should apply default maxWidth of 100%', () => {
      const content: ImageContentBlock = {
        type: 'image',
        source: {
          type: 'url',
          data: 'https://example.com/image.jpg'
        }
      };

      const { container } = render(<ImageContentRenderer content={content} />);

      const wrapper = container.querySelector('.relative.inline-block.rounded-lg.overflow-hidden');
      expect(wrapper).toHaveStyle({ maxWidth: '100%' });
    });

    it('should accept custom maxWidth as string', () => {
      const content: ImageContentBlock = {
        type: 'image',
        source: {
          type: 'url',
          data: 'https://example.com/image.jpg'
        }
      };

      const { container } = render(
        <ImageContentRenderer content={content} maxWidth="400px" />
      );

      const wrapper = container.querySelector('.relative.inline-block.rounded-lg.overflow-hidden');
      expect(wrapper).toHaveStyle({ maxWidth: '400px' });
    });

    it('should accept custom maxWidth as number', () => {
      const content: ImageContentBlock = {
        type: 'image',
        source: {
          type: 'url',
          data: 'https://example.com/image.jpg'
        }
      };

      const { container } = render(
        <ImageContentRenderer content={content} maxWidth={600} />
      );

      const wrapper = container.querySelector('.relative.inline-block.rounded-lg.overflow-hidden');
      expect(wrapper).toHaveStyle({ maxWidth: '600px' });
    });

    it('should accept maxWidth in different units', () => {
      const content: ImageContentBlock = {
        type: 'image',
        source: {
          type: 'url',
          data: 'https://example.com/image.jpg'
        }
      };

      const { rerender, container } = render(
        <ImageContentRenderer content={content} maxWidth="50rem" />
      );

      let wrapper = container.querySelector('.relative.inline-block.rounded-lg.overflow-hidden');
      expect(wrapper).toHaveStyle({ maxWidth: '50rem' });

      rerender(<ImageContentRenderer content={content} maxWidth="80vw" />);
      wrapper = container.querySelector('.relative.inline-block.rounded-lg.overflow-hidden');
      expect(wrapper).toHaveStyle({ maxWidth: '80vw' });
    });

    it('should accept percentage maxWidth', () => {
      const content: ImageContentBlock = {
        type: 'image',
        source: {
          type: 'url',
          data: 'https://example.com/image.jpg'
        }
      };

      const { container } = render(
        <ImageContentRenderer content={content} maxWidth="75%" />
      );

      const wrapper = container.querySelector('.relative.inline-block.rounded-lg.overflow-hidden');
      expect(wrapper).toHaveStyle({ maxWidth: '75%' });
    });
  });

  describe('Accessibility', () => {
    it('should have alt text for images', () => {
      const content: ImageContentBlock = {
        type: 'image',
        source: {
          type: 'url',
          data: 'https://example.com/image.jpg'
        }
      };

      render(<ImageContentRenderer content={content} />);

      const img = screen.getByRole('img');
      expect(img).toHaveAttribute('alt', 'Message attachment');
    });

    it('should have descriptive alt text', () => {
      const content: ImageContentBlock = {
        type: 'image',
        source: {
          type: 'base64',
          media_type: 'image/png',
          data: 'iVBORw0KGgoAAAANSUhEUgAAAAUA'
        }
      };

      render(<ImageContentRenderer content={content} />);

      const img = screen.getByRole('img');
      expect(img).toHaveAttribute('alt', 'Message attachment');
    });

    it('should have role="alert" for error messages', async () => {
      const content: ImageContentBlock = {
        type: 'image',
        source: {
          type: 'url',
          data: 'https://example.com/broken.jpg'
        }
      };

      render(<ImageContentRenderer content={content} />);

      const img = screen.getByRole('img');
      fireEvent.error(img);

      await waitFor(() => {
        const alert = screen.getByRole('alert');
        expect(alert).toBeInTheDocument();
        expect(alert).toHaveTextContent('Failed to load image');
      });
    });

    it('should have descriptive error message', async () => {
      const content: ImageContentBlock = {
        type: 'image',
        source: {
          type: 'url',
          data: 'https://example.com/broken.jpg'
        }
      };

      render(<ImageContentRenderer content={content} />);

      const img = screen.getByRole('img');
      fireEvent.error(img);

      await waitFor(() => {
        const errorMessage = screen.getByText('Failed to load image');
        expect(errorMessage).toBeInTheDocument();
        expect(errorMessage).toHaveClass('text-sm');
      });
    });
  });

  describe('Edge Cases', () => {
    it('should handle missing source.data for URL', async () => {
      const content: ImageContentBlock = {
        type: 'image',
        source: {
          type: 'url',
          data: ''
        }
      };

      render(<ImageContentRenderer content={content} />);

      // Should show error state immediately
      await waitFor(() => {
        expect(screen.getByTestId('image-off-icon')).toBeInTheDocument();
        expect(screen.getByText('Failed to load image')).toBeInTheDocument();
      });
    });

    it('should handle missing source.data for base64', async () => {
      const content: ImageContentBlock = {
        type: 'image',
        source: {
          type: 'base64',
          media_type: 'image/png',
          data: ''
        }
      };

      render(<ImageContentRenderer content={content} />);

      // Should show error state immediately
      await waitFor(() => {
        expect(screen.getByTestId('image-off-icon')).toBeInTheDocument();
        expect(screen.getByText('Failed to load image')).toBeInTheDocument();
      });
    });

    it('should handle undefined source.data', async () => {
      const content: ImageContentBlock = {
        type: 'image',
        source: {
          type: 'url',
          data: undefined as any
        }
      };

      render(<ImageContentRenderer content={content} />);

      // Should show error state
      await waitFor(() => {
        expect(screen.getByTestId('image-off-icon')).toBeInTheDocument();
      });
    });

    it('should handle malformed URLs gracefully', async () => {
      const content: ImageContentBlock = {
        type: 'image',
        source: {
          type: 'url',
          data: 'not-a-valid-url'
        }
      };

      render(<ImageContentRenderer content={content} />);

      const img = screen.getByRole('img');
      
      // When browser tries to load malformed URL, error event fires
      fireEvent.error(img);

      await waitFor(() => {
        expect(screen.getByTestId('image-off-icon')).toBeInTheDocument();
      });
    });

    it('should handle rapid prop changes', () => {
      const content1: ImageContentBlock = {
        type: 'image',
        source: {
          type: 'url',
          data: 'https://example.com/image1.jpg'
        }
      };

      const content2: ImageContentBlock = {
        type: 'image',
        source: {
          type: 'url',
          data: 'https://example.com/image2.jpg'
        }
      };

      const { rerender } = render(<ImageContentRenderer content={content1} />);
      
      let img = screen.getByRole('img');
      expect(img).toHaveAttribute('src', 'https://example.com/image1.jpg');

      rerender(<ImageContentRenderer content={content2} />);
      
      img = screen.getByRole('img');
      expect(img).toHaveAttribute('src', 'https://example.com/image2.jpg');
    });

    it('should handle switching between URL and base64', () => {
      const urlContent: ImageContentBlock = {
        type: 'image',
        source: {
          type: 'url',
          data: 'https://example.com/image.jpg'
        }
      };

      const base64Content: ImageContentBlock = {
        type: 'image',
        source: {
          type: 'base64',
          media_type: 'image/png',
          data: 'iVBORw0KGgoAAAANSUhEUgAAAAUA'
        }
      };

      const { rerender } = render(<ImageContentRenderer content={urlContent} />);
      
      let img = screen.getByRole('img');
      expect(img).toHaveAttribute('src', 'https://example.com/image.jpg');

      rerender(<ImageContentRenderer content={base64Content} />);
      
      img = screen.getByRole('img');
      expect(img).toHaveAttribute('src', 'data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAUA');
    });

    it('should handle invalid base64 data', async () => {
      const content: ImageContentBlock = {
        type: 'image',
        source: {
          type: 'base64',
          media_type: 'image/png',
          data: 'not-valid-base64!!!'
        }
      };

      render(<ImageContentRenderer content={content} />);

      const img = screen.getByRole('img');
      
      // Browser will fail to decode invalid base64
      fireEvent.error(img);

      await waitFor(() => {
        expect(screen.getByTestId('image-off-icon')).toBeInTheDocument();
      });
    });

    it('should handle missing media_type for base64', () => {
      const content: ImageContentBlock = {
        type: 'image',
        source: {
          type: 'base64',
          media_type: undefined as any,
          data: 'iVBORw0KGgoAAAANSUhEUgAAAAUA'
        }
      };

      render(<ImageContentRenderer content={content} />);

      const img = screen.getByRole('img');
      // Should still create data URL with undefined media type
      expect(img).toHaveAttribute('src', expect.stringContaining('data:'));
    });

    it('should handle className prop', () => {
      const content: ImageContentBlock = {
        type: 'image',
        source: {
          type: 'url',
          data: 'https://example.com/image.jpg'
        }
      };

      const { container } = render(
        <ImageContentRenderer content={content} className="custom-class" />
      );

      const wrapper = container.querySelector('.relative.inline-block.rounded-lg.overflow-hidden');
      expect(wrapper).toHaveClass('custom-class');
    });

    it('should not crash when lightbox toggled rapidly', async () => {
      const content: ImageContentBlock = {
        type: 'image',
        source: {
          type: 'url',
          data: 'https://example.com/image.jpg'
        }
      };

      const user = userEvent.setup();
      render(<ImageContentRenderer content={content} enableLightbox={true} />);

      const img = screen.getByRole('img');
      fireEvent.load(img);

      // Rapidly toggle lightbox
      await user.click(img);
      const lightbox = await screen.findByTestId('image-lightbox');
      await user.click(lightbox);
      
      await waitFor(() => {
        expect(screen.queryByTestId('image-lightbox')).not.toBeInTheDocument();
      });

      // Open again
      await user.click(img);
      await waitFor(() => {
        expect(screen.getByTestId('image-lightbox')).toBeInTheDocument();
      });
    });

    it('should maintain state through enableLightbox prop changes', async () => {
      const content: ImageContentBlock = {
        type: 'image',
        source: {
          type: 'url',
          data: 'https://example.com/image.jpg'
        }
      };

      const { rerender } = render(
        <ImageContentRenderer content={content} enableLightbox={true} />
      );

      const img = screen.getByRole('img');
      fireEvent.load(img);

      // Image should be loaded
      await waitFor(() => {
        expect(screen.queryByTestId('loader-icon')).not.toBeInTheDocument();
      });

      // Change enableLightbox
      rerender(<ImageContentRenderer content={content} enableLightbox={false} />);

      // Image should still be loaded (not reset to loading state)
      expect(screen.queryByTestId('loader-icon')).not.toBeInTheDocument();
    });

    it('should handle very long base64 strings', () => {
      const longBase64 = 'A'.repeat(10000);
      const content: ImageContentBlock = {
        type: 'image',
        source: {
          type: 'base64',
          media_type: 'image/png',
          data: longBase64
        }
      };

      expect(() => {
        render(<ImageContentRenderer content={content} />);
      }).not.toThrow();

      const img = screen.getByRole('img');
      expect(img).toHaveAttribute('src', `data:image/png;base64,${longBase64}`);
    });
  });

  describe('Component Structure', () => {
    it('should have correct wrapper structure', () => {
      const content: ImageContentBlock = {
        type: 'image',
        source: {
          type: 'url',
          data: 'https://example.com/image.jpg'
        }
      };

      const { container } = render(<ImageContentRenderer content={content} />);

      const wrapper = container.querySelector('.relative.inline-block.rounded-lg.overflow-hidden');
      expect(wrapper).toBeInTheDocument();
    });

    it('should apply proper image styling', () => {
      const content: ImageContentBlock = {
        type: 'image',
        source: {
          type: 'url',
          data: 'https://example.com/image.jpg'
        }
      };

      render(<ImageContentRenderer content={content} />);

      const img = screen.getByRole('img');
      expect(img).toHaveClass('max-w-full', 'h-auto', 'rounded-lg');
    });

    it('should have rounded corners on wrapper', () => {
      const content: ImageContentBlock = {
        type: 'image',
        source: {
          type: 'url',
          data: 'https://example.com/image.jpg'
        }
      };

      const { container } = render(<ImageContentRenderer content={content} />);

      const wrapper = container.querySelector('.rounded-lg');
      expect(wrapper).toBeInTheDocument();
    });

    it('should have overflow-hidden on wrapper', () => {
      const content: ImageContentBlock = {
        type: 'image',
        source: {
          type: 'url',
          data: 'https://example.com/image.jpg'
        }
      };

      const { container } = render(<ImageContentRenderer content={content} />);

      const wrapper = container.querySelector('.overflow-hidden');
      expect(wrapper).toBeInTheDocument();
    });
  });

  describe('Interaction Behavior', () => {
    it('should show hover effect when lightbox enabled', () => {
      const content: ImageContentBlock = {
        type: 'image',
        source: {
          type: 'url',
          data: 'https://example.com/image.jpg'
        }
      };

      render(<ImageContentRenderer content={content} enableLightbox={true} />);

      const img = screen.getByRole('img');
      expect(img).toHaveClass('hover:opacity-90', 'transition-opacity');
    });

    it('should not show hover effect when lightbox disabled', () => {
      const content: ImageContentBlock = {
        type: 'image',
        source: {
          type: 'url',
          data: 'https://example.com/image.jpg'
        }
      };

      render(<ImageContentRenderer content={content} enableLightbox={false} />);

      const img = screen.getByRole('img');
      expect(img).not.toHaveClass('hover:opacity-90');
    });

    it('should handle click only when lightbox enabled', async () => {
      const content: ImageContentBlock = {
        type: 'image',
        source: {
          type: 'url',
          data: 'https://example.com/image.jpg'
        }
      };

      const user = userEvent.setup();
      const { rerender } = render(
        <ImageContentRenderer content={content} enableLightbox={false} />
      );

      const img = screen.getByRole('img');
      fireEvent.load(img);
      await user.click(img);

      // No lightbox
      expect(screen.queryByTestId('image-lightbox')).not.toBeInTheDocument();

      // Enable lightbox
      rerender(<ImageContentRenderer content={content} enableLightbox={true} />);
      await user.click(img);

      // Lightbox should appear
      await waitFor(() => {
        expect(screen.getByTestId('image-lightbox')).toBeInTheDocument();
      });
    });
  });
});
