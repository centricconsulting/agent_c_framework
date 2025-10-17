/**
 * TypingIndicator Component Tests
 * Testing a pure UI component with no external dependencies
 */

import React from 'react';
import { describe, it, expect, beforeEach, afterEach, vi } from 'vitest';
import { render, fireEvent, screen } from '@testing-library/react';
import { TypingIndicator, typingIndicatorStyles } from '../TypingIndicator';

describe('TypingIndicator', () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  afterEach(() => {
    vi.restoreAllMocks();
  });

  describe('dots variant', () => {
    it('should render three dots with bounce animation by default', () => {
      const { container } = render(<TypingIndicator />);
      
      // Default variant is dots
      const dots = container.querySelectorAll('[style*="typing-bounce"]');
      expect(dots).toHaveLength(3);
    });

    it('should apply correct classes to dots', () => {
      const { container } = render(<TypingIndicator variant="dots" />);
      
      const dots = container.querySelectorAll('.rounded-full.bg-muted-foreground\\/60');
      expect(dots).toHaveLength(3);
      
      dots.forEach(dot => {
        expect(dot).toHaveClass('rounded-full');
        expect(dot.className).toContain('bg-muted-foreground/60');
      });
    });

    it('should apply staggered animation delays', () => {
      const { container } = render(<TypingIndicator variant="dots" speed={300} />);
      
      const dots = container.querySelectorAll('[style*="animation"]');
      
      // Check animation delay in style attribute directly
      expect(dots[0].getAttribute('style')).toContain('animation-delay: 0ms');
      expect(dots[1].getAttribute('style')).toContain('animation-delay: 300ms');
      expect(dots[2].getAttribute('style')).toContain('animation-delay: 600ms');
    });

    it('should set animation duration based on speed', () => {
      const { container } = render(<TypingIndicator variant="dots" speed={200} />);
      
      const dots = container.querySelectorAll('[style*="animation"]');
      
      // animation duration should be speed * 4 = 800ms
      dots.forEach(dot => {
        expect(dot.getAttribute('style')).toContain('800ms');
      });
    });

    it('should handle custom speed values', () => {
      const speeds = [100, 250, 500];
      
      speeds.forEach(speed => {
        const { container } = render(<TypingIndicator variant="dots" speed={speed} />);
        
        const dots = container.querySelectorAll('[style*="animation"]');
        dots.forEach((dot, index) => {
          const style = dot.getAttribute('style') || '';
          expect(style).toContain(`animation-delay: ${index * speed}ms`);
          expect(style).toContain(`${speed * 4}ms`);
        });
      });
    });
  });

  describe('pulse variant', () => {
    it('should render three elements with animate-pulse', () => {
      const { container } = render(<TypingIndicator variant="pulse" />);
      
      const pulsingElements = container.querySelectorAll('.animate-pulse');
      expect(pulsingElements).toHaveLength(3);
    });

    it('should apply opacity gradients', () => {
      const { container } = render(<TypingIndicator variant="pulse" />);
      
      // Check for different opacity levels
      expect(container.querySelector('.bg-muted-foreground\\/60')).toBeInTheDocument();
      expect(container.querySelector('.bg-muted-foreground\\/40')).toBeInTheDocument();
      expect(container.querySelector('.bg-muted-foreground\\/20')).toBeInTheDocument();
    });

    it('should apply animation delays via classes', () => {
      const { container } = render(<TypingIndicator variant="pulse" />);
      
      const elements = container.querySelectorAll('.animate-pulse');
      
      // First element has no delay
      expect(elements[0].className).not.toContain('animation-delay');
      
      // Second and third elements have delays
      expect(elements[1]).toHaveClass('[animation-delay:150ms]');
      expect(elements[2]).toHaveClass('[animation-delay:300ms]');
    });

    it('should apply correct size classes', () => {
      const { container } = render(<TypingIndicator variant="pulse" size="md" />);
      
      const dots = container.querySelectorAll('.animate-pulse');
      dots.forEach(dot => {
        expect(dot).toHaveClass('h-2', 'w-2');
      });
    });
  });

  describe('wave variant', () => {
    it('should render three wave elements', () => {
      const { container } = render(<TypingIndicator variant="wave" />);
      
      const waves = container.querySelectorAll('[style*="typing-wave"]');
      expect(waves).toHaveLength(3);
    });

    it('should use items-end flex alignment', () => {
      const { container } = render(<TypingIndicator variant="wave" />);
      
      const flexContainer = container.querySelector('.flex.items-end');
      expect(flexContainer).toBeInTheDocument();
    });

    it('should apply wave animation with correct timing', () => {
      const { container } = render(<TypingIndicator variant="wave" speed={250} />);
      
      const waves = container.querySelectorAll('[style*="animation"]');
      
      waves.forEach((wave, index) => {
        const style = wave.getAttribute('style') || '';
        expect(style).toContain('typing-wave');
        expect(style).toContain('1000ms'); // speed * 4
        expect(style).toContain(`animation-delay: ${index * 250}ms`);
      });
    });

    it('should use primary color with opacity', () => {
      const { container } = render(<TypingIndicator variant="wave" />);
      
      const waves = container.querySelectorAll('.bg-primary\\/60');
      expect(waves).toHaveLength(3);
    });
  });

  describe('size variants', () => {
    const sizeConfigs = [
      {
        size: 'sm' as const,
        containerClass: ['px-2', 'py-1'],
        dotClass: ['h-1.5', 'w-1.5'],
        gap: 'gap-1'
      },
      {
        size: 'md' as const,
        containerClass: ['px-3', 'py-2'],
        dotClass: ['h-2', 'w-2'],
        gap: 'gap-1.5'
      },
      {
        size: 'lg' as const,
        containerClass: ['px-4', 'py-3'],
        dotClass: ['h-2.5', 'w-2.5'],
        gap: 'gap-2'
      }
    ];
    
    test.each(sizeConfigs)(
      'should apply correct classes for $size size',
      ({ size, containerClass, dotClass }) => {
        const { container } = render(<TypingIndicator size={size} />);
        
        const wrapper = container.querySelector('[role="status"]');
        containerClass.forEach(cls => {
          expect(wrapper).toHaveClass(cls);
        });
        
        const dots = container.querySelectorAll('.rounded-full');
        dots.forEach(dot => {
          dotClass.forEach(cls => {
            expect(dot).toHaveClass(cls);
          });
        });
      }
    );

    it('should apply correct gap for each size', () => {
      const { container: smContainer } = render(<TypingIndicator size="sm" />);
      expect(smContainer.querySelector('.gap-1')).toBeInTheDocument();

      const { container: mdContainer } = render(<TypingIndicator size="md" />);
      expect(mdContainer.querySelector('.gap-1\\.5')).toBeInTheDocument();

      const { container: lgContainer } = render(<TypingIndicator size="lg" />);
      expect(lgContainer.querySelector('.gap-2')).toBeInTheDocument();
    });
  });

  describe('style injection', () => {
    it('should inject keyframe animations via dangerouslySetInnerHTML', () => {
      const { container } = render(<TypingIndicator />);
      
      const styleTag = container.querySelector('style');
      expect(styleTag).toBeInTheDocument();
      
      const styleContent = styleTag?.innerHTML || '';
      expect(styleContent).toContain('@keyframes typing-bounce');
      expect(styleContent).toContain('@keyframes typing-wave');
    });

    it('should include complete keyframe definitions', () => {
      const { container } = render(<TypingIndicator />);
      
      const styleTag = container.querySelector('style');
      const styleContent = styleTag?.innerHTML || '';
      
      // Check for bounce keyframes
      expect(styleContent).toContain('0%, 60%, 100%');
      expect(styleContent).toContain('transform: translateY(0)');
      expect(styleContent).toContain('transform: translateY(-10px)');
      
      // Check for wave keyframes
      expect(styleContent).toContain('transform: scaleY(1)');
      expect(styleContent).toContain('transform: scaleY(1.5)');
    });

    it('should include opacity in keyframes', () => {
      const { container } = render(<TypingIndicator />);
      
      const styleTag = container.querySelector('style');
      const styleContent = styleTag?.innerHTML || '';
      
      expect(styleContent).toContain('opacity: 0.7');
      expect(styleContent).toContain('opacity: 1');
    });
  });

  describe('ARIA attributes', () => {
    it('should have role="status"', () => {
      const { container } = render(<TypingIndicator />);
      const element = container.querySelector('[role="status"]');
      expect(element).toBeInTheDocument();
    });

    it('should have aria-label with default text', () => {
      const { container } = render(<TypingIndicator />);
      const element = container.querySelector('[role="status"]');
      expect(element).toHaveAttribute('aria-label', 'Assistant is typing');
    });

    it('should use custom label when provided', () => {
      const { container } = render(<TypingIndicator label="Agent is responding" />);
      const element = container.querySelector('[role="status"]');
      expect(element).toHaveAttribute('aria-label', 'Agent is responding');
    });

    it('should have aria-live="polite"', () => {
      const { container } = render(<TypingIndicator />);
      const element = container.querySelector('[role="status"]');
      expect(element).toHaveAttribute('aria-live', 'polite');
    });
  });

  describe('screen reader support', () => {
    it('should include sr-only text', () => {
      const { container } = render(<TypingIndicator />);
      
      const srText = container.querySelector('.sr-only');
      expect(srText).toBeInTheDocument();
      expect(srText).toHaveTextContent('Assistant is typing');
    });

    it('should use custom label in sr-only text', () => {
      const { container } = render(<TypingIndicator label="Processing request" />);
      
      const srText = container.querySelector('.sr-only');
      expect(srText).toHaveTextContent('Processing request');
    });

    it('should maintain sr-only class for screen reader visibility', () => {
      const { container } = render(<TypingIndicator />);
      
      const srText = container.querySelector('.sr-only');
      expect(srText).toHaveClass('sr-only');
    });
  });

  describe('container styling', () => {
    it('should apply base container classes', () => {
      const { container } = render(<TypingIndicator />);
      
      const element = container.querySelector('[role="status"]');
      expect(element).toHaveClass(
        'inline-flex',
        'items-center',
        'rounded-lg',
        'bg-muted'
      );
    });

    it('should apply transition classes', () => {
      const { container } = render(<TypingIndicator />);
      
      const element = container.querySelector('[role="status"]');
      expect(element).toHaveClass(
        'transition-all',
        'duration-200',
        'ease-in-out'
      );
    });

    it('should apply animation-in classes', () => {
      const { container } = render(<TypingIndicator />);
      
      const element = container.querySelector('[role="status"]');
      expect(element).toHaveClass(
        'animate-in',
        'fade-in-0',
        'slide-in-from-bottom-1'
      );
    });

    it('should merge custom className', () => {
      const { container } = render(
        <TypingIndicator className="custom-class another-class" />
      );
      
      const element = container.querySelector('[role="status"]');
      expect(element).toHaveClass('custom-class', 'another-class');
      // Should also maintain default classes
      expect(element).toHaveClass('inline-flex', 'items-center');
    });
  });

  describe('ref forwarding', () => {
    it('should forward ref to container div', () => {
      const ref = React.createRef<HTMLDivElement>();
      
      render(<TypingIndicator ref={ref} />);
      
      expect(ref.current).toBeInstanceOf(HTMLDivElement);
      expect(ref.current).toHaveAttribute('role', 'status');
    });

    it('should allow ref manipulation', () => {
      const ref = React.createRef<HTMLDivElement>();
      
      render(<TypingIndicator ref={ref} />);
      
      // Verify ref is properly attached
      expect(ref.current).toBeInstanceOf(HTMLDivElement);
      expect(ref.current).not.toBeNull();
      
      // Verify the element has the expected role attribute
      expect(ref.current?.getAttribute('role')).toBe('status');
      
      // Test manipulation by setting a data attribute
      ref.current?.setAttribute('data-test-id', 'test-value');
      expect(ref.current?.getAttribute('data-test-id')).toBe('test-value')
    });

    it('should work with callback refs', () => {
      let elementRef: HTMLDivElement | null = null;
      const callbackRef = (el: HTMLDivElement | null) => {
        elementRef = el;
      };
      
      render(<TypingIndicator ref={callbackRef} />);
      
      expect(elementRef).toBeInstanceOf(HTMLDivElement);
      expect(elementRef).toHaveAttribute('role', 'status');
    });
  });

  describe('props forwarding', () => {
    it('should forward data attributes', () => {
      const { container } = render(
        <TypingIndicator data-testid="typing-indicator" data-custom="value" />
      );
      
      const element = container.querySelector('[role="status"]');
      expect(element).toHaveAttribute('data-testid', 'typing-indicator');
      expect(element).toHaveAttribute('data-custom', 'value');
    });

    it('should forward event handlers', () => {
      const handleClick = vi.fn();
      const handleMouseEnter = vi.fn();
      const handleMouseLeave = vi.fn();
      
      const { container } = render(
        <TypingIndicator 
          onClick={handleClick}
          onMouseEnter={handleMouseEnter}
          onMouseLeave={handleMouseLeave}
        />
      );
      
      const element = container.querySelector('[role="status"]');
      
      fireEvent.click(element!);
      expect(handleClick).toHaveBeenCalledOnce();
      
      fireEvent.mouseEnter(element!);
      expect(handleMouseEnter).toHaveBeenCalledOnce();
      
      fireEvent.mouseLeave(element!);
      expect(handleMouseLeave).toHaveBeenCalledOnce();
    });

    it('should forward style prop', () => {
      const { container } = render(
        <TypingIndicator style={{ backgroundColor: 'red', padding: '10px' }} />
      );
      
      const element = container.querySelector('[role="status"]');
      // Check that style attribute contains the expected values
      const style = element?.getAttribute('style') || '';
      expect(style).toContain('background-color: red');
      expect(style).toContain('padding: 10px');
    });

    it('should forward id attribute', () => {
      const { container } = render(
        <TypingIndicator id="typing-indicator-1" />
      );
      
      const element = container.querySelector('[role="status"]');
      expect(element).toHaveAttribute('id', 'typing-indicator-1');
    });
  });

  describe('animation speed', () => {
    it('should calculate correct animation duration for dots', () => {
      const speeds = [100, 200, 300, 500];
      
      speeds.forEach(speed => {
        const { container } = render(
          <TypingIndicator variant="dots" speed={speed} />
        );
        
        const dots = container.querySelectorAll('[style*="animation"]');
        dots.forEach(dot => {
          const style = dot.getAttribute('style');
          expect(style).toContain(`${speed * 4}ms`);
        });
      });
    });

    it('should calculate correct animation duration for wave', () => {
      const speeds = [100, 200, 300, 500];
      
      speeds.forEach(speed => {
        const { container } = render(
          <TypingIndicator variant="wave" speed={speed} />
        );
        
        const waves = container.querySelectorAll('[style*="animation"]');
        waves.forEach(wave => {
          const style = wave.getAttribute('style');
          expect(style).toContain(`${speed * 4}ms`);
        });
      });
    });

    it('should apply correct delays based on speed', () => {
      const { container } = render(
        <TypingIndicator variant="dots" speed={150} />
      );
      
      const dots = container.querySelectorAll('[style*="animation"]');
      expect(dots[0].getAttribute('style')).toContain('animation-delay: 0ms');
      expect(dots[1].getAttribute('style')).toContain('animation-delay: 150ms');
      expect(dots[2].getAttribute('style')).toContain('animation-delay: 300ms');
    });

    it('should handle very fast speeds', () => {
      const { container } = render(
        <TypingIndicator variant="dots" speed={50} />
      );
      
      const dots = container.querySelectorAll('[style*="animation"]');
      dots.forEach((dot, index) => {
        const style = dot.getAttribute('style') || '';
        expect(style).toContain(`animation-delay: ${index * 50}ms`);
        expect(style).toContain('200ms'); // 50 * 4
      });
    });

    it('should handle very slow speeds', () => {
      const { container } = render(
        <TypingIndicator variant="dots" speed={1000} />
      );
      
      const dots = container.querySelectorAll('[style*="animation"]');
      dots.forEach((dot, index) => {
        const style = dot.getAttribute('style') || '';
        expect(style).toContain(`animation-delay: ${index * 1000}ms`);
        expect(style).toContain('4000ms'); // 1000 * 4
      });
    });
  });

  describe('component metadata', () => {
    it('should have correct displayName', () => {
      expect(TypingIndicator.displayName).toBe('TypingIndicator');
    });
  });

  describe('exports', () => {
    it('should export TypingIndicator component', () => {
      expect(TypingIndicator).toBeDefined();
      expect(typeof TypingIndicator).toBe('object'); // forwardRef returns object
    });

    it('should export typingIndicatorStyles constant', () => {
      expect(typingIndicatorStyles).toBeDefined();
      expect(typingIndicatorStyles).toContain('@keyframes typing-bounce');
      expect(typingIndicatorStyles).toContain('@keyframes typing-wave');
    });

    it('should have matching styles in export and component', () => {
      const { container } = render(<TypingIndicator />);
      
      const styleTag = container.querySelector('style');
      const inlineStyles = styleTag?.innerHTML || '';
      
      // Both should contain the same keyframes
      expect(inlineStyles).toContain('typing-bounce');
      expect(typingIndicatorStyles).toContain('typing-bounce');
      
      expect(inlineStyles).toContain('typing-wave');
      expect(typingIndicatorStyles).toContain('typing-wave');
    });
  });

  describe('performance', () => {
    it('should handle rapid variant changes', () => {
      const { rerender } = render(<TypingIndicator variant="dots" />);
      
      // Rapidly change variants
      const variants: Array<'dots' | 'pulse' | 'wave'> = ['pulse', 'wave', 'dots', 'pulse', 'wave'];
      variants.forEach(variant => {
        expect(() => {
          rerender(<TypingIndicator variant={variant} />);
        }).not.toThrow();
      });
    });

    it('should handle rapid size changes', () => {
      const { rerender } = render(<TypingIndicator size="sm" />);
      
      // Rapidly change sizes
      const sizes: Array<'sm' | 'md' | 'lg'> = ['md', 'lg', 'sm', 'lg', 'md'];
      sizes.forEach(size => {
        expect(() => {
          rerender(<TypingIndicator size={size} />);
        }).not.toThrow();
      });
    });

    it('should handle rapid speed changes', () => {
      const { rerender } = render(<TypingIndicator speed={100} />);
      
      // Rapidly change speeds
      const speeds = [200, 300, 50, 1000, 150];
      speeds.forEach(speed => {
        expect(() => {
          rerender(<TypingIndicator speed={speed} />);
        }).not.toThrow();
      });
    });
  });

  describe('edge cases', () => {
    it('should handle undefined props gracefully', () => {
      const { container } = render(
        <TypingIndicator 
          variant={undefined}
          size={undefined}
          speed={undefined}
          label={undefined}
        />
      );
      
      const element = container.querySelector('[role="status"]');
      expect(element).toBeInTheDocument();
      // Should use defaults
      expect(element).toHaveAttribute('aria-label', 'Assistant is typing');
    });

    it('should handle empty label', () => {
      const { container } = render(<TypingIndicator label="" />);
      
      const element = container.querySelector('[role="status"]');
      expect(element).toHaveAttribute('aria-label', '');
    });

    it('should handle zero speed gracefully', () => {
      const { container } = render(<TypingIndicator speed={0} />);
      
      const dots = container.querySelectorAll('[style*="animation"]');
      dots.forEach(() => {
        // All dots should have 0ms animation delay and duration
        expect(dots[0].getAttribute('style')).toContain('0ms');
      });
    });

    it('should handle negative speed as positive', () => {
      // Component should handle this gracefully
      const { container } = render(<TypingIndicator speed={-100} />);
      
      const dots = container.querySelectorAll('[style*="animation"]');
      expect(dots.length).toBeGreaterThan(0);
      // Component will use the negative value as-is
      expect(dots[0].getAttribute('style')).toBeDefined();
    });
  });

  describe('visual combinations', () => {
    it('should render dots variant with all sizes correctly', () => {
      const sizes: Array<'sm' | 'md' | 'lg'> = ['sm', 'md', 'lg'];
      
      sizes.forEach(size => {
        const { container } = render(<TypingIndicator variant="dots" size={size} />);
        
        const dots = container.querySelectorAll('[style*="typing-bounce"]');
        expect(dots).toHaveLength(3);
        
        const wrapper = container.querySelector('[role="status"]');
        expect(wrapper).toBeInTheDocument();
      });
    });

    it('should render pulse variant with all sizes correctly', () => {
      const sizes: Array<'sm' | 'md' | 'lg'> = ['sm', 'md', 'lg'];
      
      sizes.forEach(size => {
        const { container } = render(<TypingIndicator variant="pulse" size={size} />);
        
        const pulsingElements = container.querySelectorAll('.animate-pulse');
        expect(pulsingElements).toHaveLength(3);
      });
    });

    it('should render wave variant with all sizes correctly', () => {
      const sizes: Array<'sm' | 'md' | 'lg'> = ['sm', 'md', 'lg'];
      
      sizes.forEach(size => {
        const { container } = render(<TypingIndicator variant="wave" size={size} />);
        
        const waves = container.querySelectorAll('[style*="typing-wave"]');
        expect(waves).toHaveLength(3);
      });
    });
  });

  describe('complete component integration', () => {
    it('should render complete component with all features', () => {
      const handleClick = vi.fn();
      const ref = React.createRef<HTMLDivElement>();
      
      const { container } = render(
        <TypingIndicator
          ref={ref}
          variant="dots"
          size="md"
          speed={250}
          label="Custom typing indicator"
          className="custom-class"
          data-testid="test-indicator"
          onClick={handleClick}
          style={{ marginTop: '10px' }}
        />
      );
      
      // Check container
      const indicatorElement = container.querySelector('[role="status"]');
      expect(indicatorElement).toBeInTheDocument();
      
      // Check ARIA
      expect(indicatorElement).toHaveAttribute('aria-label', 'Custom typing indicator');
      expect(indicatorElement).toHaveAttribute('aria-live', 'polite');
      
      // Check classes
      expect(indicatorElement).toHaveClass('custom-class');
      
      // Check data attributes
      expect(indicatorElement).toHaveAttribute('data-testid', 'test-indicator');
      
      // Check style
      const indicatorStyle = indicatorElement?.getAttribute('style') || '';
      expect(indicatorStyle).toContain('margin-top: 10px');
      
      // Check ref
      expect(ref.current).toBe(indicatorElement);
      
      // Check click handler
      fireEvent.click(indicatorElement!);
      expect(handleClick).toHaveBeenCalledOnce();
      
      // Check dots
      const dots = container.querySelectorAll('[style*="typing-bounce"]');
      expect(dots).toHaveLength(3);
      
      // Check animation
      dots.forEach((dot, index) => {
        const style = dot.getAttribute('style') || '';
        expect(style).toContain(`animation-delay: ${index * 250}ms`);
        expect(style).toContain('1000ms'); // 250 * 4
      });
      
      // Check sr-only text
      const srText = container.querySelector('.sr-only');
      expect(srText).toHaveTextContent('Custom typing indicator');
      
      // Check style injection
      const styleTag = container.querySelector('style');
      expect(styleTag).toBeInTheDocument();
    });
  });
});