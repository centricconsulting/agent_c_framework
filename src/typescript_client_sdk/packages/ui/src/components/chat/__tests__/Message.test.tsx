/**
 * Message Component Tests - Part 1: Basic Rendering
 * Testing message rendering, timestamps, avatars, and different message types
 */

import React from 'react';
import { describe, it, expect, beforeEach, afterEach, vi } from 'vitest';
import { render, screen, within } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { Message, type MessageData } from '../Message';

// Mock MessageContentRenderer to avoid dependency issues
vi.mock('../MessageContentRenderer', () => ({
  MessageContentRenderer: ({ content, role }: any) => (
    <div 
      data-testid="message-content-renderer" 
      data-role={role}
      data-content={typeof content === 'string' ? content : JSON.stringify(content)}
    >
      {typeof content === 'string' ? content : 'Complex content'}
    </div>
  )
}));

// Mock MessageFooter
vi.mock('../MessageFooter', () => ({
  MessageFooter: ({ message, onEdit, showTimestamp }: any) => {
    // Format timestamp if showTimestamp is true
    const formattedTime = React.useMemo(() => {
      if (!showTimestamp || !message.timestamp) return null;
      try {
        const date = new Date(message.timestamp);
        if (isNaN(date.getTime())) return null;
        return date.toLocaleTimeString('en-US', { 
          hour: 'numeric', 
          minute: '2-digit',
          hour12: true 
        });
      } catch {
        return null;
      }
    }, [message.timestamp, showTimestamp]);
    
    return (
      <div 
        data-testid="message-footer"
        data-role={message.role}
        onClick={onEdit}
      >
        Footer for {message.role}
        {formattedTime && (
          <span className="text-xs text-muted-foreground flex items-center gap-1">
            <div data-testid="clock-icon" className="h-3 w-3" />
            {formattedTime}
          </span>
        )}
      </div>
    );
  }
}));

// Mock Logger
vi.mock('../../../utils/logger', () => ({
  Logger: {
    debug: vi.fn(),
    error: vi.fn(),
    warn: vi.fn()
  }
}));

// Mock Lucide React icons
vi.mock('lucide-react', () => ({
  User: ({ className }: any) => <div data-testid="user-icon" className={className} />,
  Bot: ({ className }: any) => <div data-testid="bot-icon" className={className} />,
  Brain: ({ className }: any) => <div data-testid="brain-icon" className={className} />,
  AlertCircle: ({ className }: any) => <div data-testid="alert-icon" className={className} />,
  Clock: ({ className }: any) => <div data-testid="clock-icon" className={className} />,
  ChevronRight: ({ className }: any) => <div data-testid="chevron-icon" className={className} />,
  Loader2: ({ className }: any) => <div data-testid="loader-icon" className={className} />,
  Check: ({ className }: any) => <div data-testid="check-icon" className={className} />,
  Copy: ({ className }: any) => <div data-testid="copy-icon" className={className} />
}));

// Mock ReactMarkdown
vi.mock('react-markdown', () => ({
  default: ({ children }: any) => <div data-testid="markdown-content">{children}</div>
}));

// Mock remark-gfm
vi.mock('remark-gfm', () => ({
  default: () => {}
}));

// Mock framer-motion
vi.mock('framer-motion', () => ({
  motion: {
    div: ({ children, initial, animate, exit, transition, ...props }: any) => (
      <div 
        data-testid="motion-div"
        data-initial={JSON.stringify(initial)}
        data-animate={JSON.stringify(animate)}
        data-exit={JSON.stringify(exit)}
        {...props}
      >
        {children}
      </div>
    )
  },
  AnimatePresence: ({ children }: any) => <>{children}</>
}));

describe('Message Component - Basic Rendering', () => {
  const baseMessage: MessageData = {
    id: 'msg-1',
    role: 'user',
    content: 'Hello, this is a test message',
    timestamp: new Date('2024-01-01T12:00:00Z')
  };

  beforeEach(() => {
    vi.clearAllMocks();
  });

  afterEach(() => {
    vi.restoreAllMocks();
  });

  describe('User Message Rendering', () => {
    it('should render a user message with correct alignment', () => {
      const { container } = render(<Message message={baseMessage} />);
      
      // Check for flex-row-reverse class for right alignment
      const messageElement = container.querySelector('.group');
      expect(messageElement).toHaveClass('flex-row-reverse');
      
      // Check for article role
      const article = container.querySelector('[role="article"]');
      expect(article).toBeInTheDocument();
      expect(article).toHaveAttribute('aria-label', 'Message from user');
    });

    it('should render user message with muted background', () => {
      const { container } = render(<Message message={baseMessage} />);
      
      // Check for bg-muted class on message bubble
      const bubble = container.querySelector('.bg-muted');
      expect(bubble).toBeInTheDocument();
    });

    it('should display user avatar icon', () => {
      render(<Message message={baseMessage} />);
      
      // Check for user icon
      const userIcon = screen.getByTestId('user-icon');
      expect(userIcon).toBeInTheDocument();
    });

    it('should render user message content through MessageContentRenderer', () => {
      render(<Message message={baseMessage} />);
      
      // Check that MessageContentRenderer is used
      const contentRenderer = screen.getByTestId('message-content-renderer');
      expect(contentRenderer).toBeInTheDocument();
      expect(contentRenderer).toHaveAttribute('data-role', 'user');
      expect(contentRenderer).toHaveAttribute('data-content', 'Hello, this is a test message');
    });

    it('should align user message content to the right', () => {
      const { container } = render(<Message message={baseMessage} />);
      
      // Check for flex alignment classes
      const contentWrapper = container.querySelector('.flex-1.max-w-full');
      expect(contentWrapper).toHaveClass('flex', 'flex-col', 'items-end');
    });
  });

  describe('Assistant Message Rendering', () => {
    const assistantMessage: MessageData = {
      ...baseMessage,
      id: 'msg-2',
      role: 'assistant',
      content: 'I can help you with that!'
    };

    it('should render an assistant message with correct alignment', () => {
      const { container } = render(<Message message={assistantMessage} />);
      
      // Should NOT have flex-row-reverse for left alignment
      const messageElement = container.querySelector('.group');
      expect(messageElement).not.toHaveClass('flex-row-reverse');
      
      // Check for article role
      const article = container.querySelector('[role="article"]');
      expect(article).toHaveAttribute('aria-label', 'Message from assistant');
    });

    it('should render assistant message with card background color', () => {
      const { container } = render(<Message message={assistantMessage} />);
      
      // Check for bg-card class on message bubble
      const bubble = container.querySelector('.bg-card');
      expect(bubble).toBeInTheDocument();
    });

    it('should display bot avatar icon', () => {
      render(<Message message={assistantMessage} />);
      
      // Check for bot icon
      const botIcon = screen.getByTestId('bot-icon');
      expect(botIcon).toBeInTheDocument();
    });

    it('should render assistant message content through MessageContentRenderer', () => {
      render(<Message message={assistantMessage} />);
      
      // Check that MessageContentRenderer is used
      const contentRenderer = screen.getByTestId('message-content-renderer');
      expect(contentRenderer).toBeInTheDocument();
      expect(contentRenderer).toHaveAttribute('data-role', 'assistant');
      expect(contentRenderer).toHaveAttribute('data-content', 'I can help you with that!');
    });

    it('should NOT align assistant message content to the right', () => {
      const { container } = render(<Message message={assistantMessage} />);
      
      // Check that it doesn't have right alignment classes
      const contentWrapper = container.querySelector('.flex-1.max-w-full');
      expect(contentWrapper).not.toHaveClass('items-end');
    });

    it('should show footer for assistant messages when showFooter is true', () => {
      render(<Message message={assistantMessage} showFooter={true} />);
      
      const footer = screen.getByTestId('message-footer');
      expect(footer).toBeInTheDocument();
      expect(footer).toHaveAttribute('data-role', 'assistant');
    });

    it('should not show footer when showFooter is false', () => {
      render(<Message message={assistantMessage} showFooter={false} />);
      
      const footer = screen.queryByTestId('message-footer');
      expect(footer).not.toBeInTheDocument();
    });
  });

  describe('System Message Rendering', () => {
    const systemMessage: MessageData = {
      ...baseMessage,
      id: 'msg-3',
      role: 'system',
      content: 'System notification: Connection established'
    };

    it('should render a system message', () => {
      const { container } = render(<Message message={systemMessage} />);
      
      // Check for article element
      const article = container.querySelector('[role="article"]');
      expect(article).toBeInTheDocument();
      
      // System messages should use assistant-like rendering
      const messageElement = container.querySelector('.group');
      expect(messageElement).not.toHaveClass('flex-row-reverse');
    });

    it('should render system message content through MessageContentRenderer', () => {
      render(<Message message={systemMessage} />);
      
      const contentRenderer = screen.getByTestId('message-content-renderer');
      expect(contentRenderer).toBeInTheDocument();
      expect(contentRenderer).toHaveAttribute('data-role', 'system');
      expect(contentRenderer).toHaveAttribute('data-content', 'System notification: Connection established');
    });

    it('should display bot avatar for system messages', () => {
      render(<Message message={systemMessage} />);
      
      // System messages use bot icon like assistant messages
      const botIcon = screen.getByTestId('bot-icon');
      expect(botIcon).toBeInTheDocument();
    });
  });

  describe('Timestamp Display', () => {
    it('should pass showTimestamp to MessageFooter', () => {
      render(<Message message={baseMessage} showTimestamp={true} showFooter={true} />);
      
      // Timestamp is now handled by MessageFooter
      const footer = screen.getByTestId('message-footer');
      expect(footer).toBeInTheDocument();
      
      // Check for clock icon in footer
      const clockIcon = within(footer).getByTestId('clock-icon');
      expect(clockIcon).toBeInTheDocument();
      
      // Check that a time is displayed in footer
      const timeElements = within(footer).getAllByText(/\d{1,2}:\d{2}\s*(AM|PM)?/i);
      expect(timeElements.length).toBeGreaterThan(0);
    });

    it('should not show timestamp when showTimestamp is false', () => {
      render(<Message message={baseMessage} showTimestamp={false} showFooter={true} />);
      
      // Footer should exist but timestamp should not be shown
      const footer = screen.queryByTestId('message-footer');
      if (footer) {
        // Clock icon should not be present in footer
        const clockIcon = within(footer).queryByTestId('clock-icon');
        expect(clockIcon).not.toBeInTheDocument();
        
        // Time text should not be present in footer
        const timeText = within(footer).queryByText(/\d{1,2}:\d{2}\s*(AM|PM)?/i);
        expect(timeText).toBeNull();
      }
    });

    it('should format timestamp correctly for different times', () => {
      const morningMessage = {
        ...baseMessage,
        timestamp: new Date('2024-01-01T09:30:00Z')
      };
      
      const { rerender } = render(<Message message={morningMessage} showTimestamp={true} showFooter={true} />);
      // Check that a time is displayed in footer
      const footer = screen.getByTestId('message-footer');
      let timeElements = within(footer).getAllByText(/\d{1,2}:\d{2}\s*(AM|PM)?/i);
      expect(timeElements.length).toBeGreaterThan(0);
      
      const eveningMessage = {
        ...baseMessage,
        timestamp: new Date('2024-01-01T18:45:00Z')
      };
      
      rerender(<Message message={eveningMessage} showTimestamp={true} showFooter={true} />);
      // Check that a time is displayed in footer
      timeElements = within(footer).getAllByText(/\d{1,2}:\d{2}\s*(AM|PM)?/i);
      expect(timeElements.length).toBeGreaterThan(0);
    });

    it('should handle string timestamps', () => {
      const messageWithStringTimestamp = {
        ...baseMessage,
        timestamp: '2024-01-01T15:30:00Z'
      };
      
      render(<Message message={messageWithStringTimestamp} showTimestamp={true} />);
      // Just check that a time is displayed
      const timeElements = screen.getAllByText(/\d{1,2}:\d{2}\s*(AM|PM)?/i);
      expect(timeElements.length).toBeGreaterThan(0);
    });

    it('should handle Date object timestamps', () => {
      const messageWithDateTimestamp = {
        ...baseMessage,
        timestamp: new Date('2024-01-01T10:15:00Z')
      };
      
      render(<Message message={messageWithDateTimestamp} showTimestamp={true} />);
      // Just check that a time is displayed
      const timeElements = screen.getAllByText(/\d{1,2}:\d{2}\s*(AM|PM)?/i);
      expect(timeElements.length).toBeGreaterThan(0);
    });

    it('should show timestamp in footer when showFooter is true', () => {
      render(<Message message={baseMessage} showTimestamp={true} showFooter={true} />);
      
      // Timestamp is now in MessageFooter
      const footer = screen.getByTestId('message-footer');
      expect(footer).toBeInTheDocument();
      
      // Check that timestamp is within footer
      const timestampElement = within(footer).getByText(/\d{1,2}:\d{2}\s*(AM|PM)?/i);
      expect(timestampElement).toBeInTheDocument();
      expect(timestampElement).toHaveClass('text-xs', 'text-muted-foreground');
    });
  });

  describe('Avatar Display', () => {
    it('should display default user avatar for user messages', () => {
      const { container } = render(<Message message={baseMessage} />);
      
      // Check for user icon
      const userIcon = screen.getByTestId('user-icon');
      expect(userIcon).toBeInTheDocument();
      
      // Icon should have size and color classes
      expect(userIcon).toHaveClass('h-4', 'w-4', 'text-primary-foreground');
      
      // Check that avatar icon exists in the flex-shrink-0 container
      const avatarWrapper = container.querySelector('.flex-shrink-0');
      expect(avatarWrapper).toContainElement(userIcon);
    });

    it('should display default bot avatar for assistant messages', () => {
      const assistantMessage = {
        ...baseMessage,
        role: 'assistant' as const
      };
      
      const { container } = render(<Message message={assistantMessage} />);
      
      // Check for bot icon
      const botIcon = screen.getByTestId('bot-icon');
      expect(botIcon).toBeInTheDocument();
      
      // Icon should have size and color classes
      expect(botIcon).toHaveClass('h-4', 'w-4', 'text-primary');
      
      // Check that avatar icon exists in the flex-shrink-0 container
      const avatarWrapper = container.querySelector('.flex-shrink-0');
      expect(avatarWrapper).toContainElement(botIcon);
    });

    it('should use custom avatar component when provided', () => {
      const customAvatar = <div data-testid="custom-avatar">Custom Avatar</div>;
      
      render(<Message message={baseMessage} avatarComponent={customAvatar} />);
      
      // Check for custom avatar
      const avatar = screen.getByTestId('custom-avatar');
      expect(avatar).toBeInTheDocument();
      expect(avatar).toHaveTextContent('Custom Avatar');
      
      // Default avatars should not be present
      expect(screen.queryByTestId('user-icon')).not.toBeInTheDocument();
      expect(screen.queryByTestId('bot-icon')).not.toBeInTheDocument();
    });

    it('should render avatar in flex-shrink-0 container', () => {
      const { container } = render(<Message message={baseMessage} />);
      
      // Find avatar container
      const avatarWrapper = container.querySelector('.flex-shrink-0');
      expect(avatarWrapper).toBeInTheDocument();
      
      // Check that avatar is within this container
      const userIcon = avatarWrapper?.querySelector('[data-testid="user-icon"]');
      expect(userIcon).toBeInTheDocument();
    });

    it('should position avatar correctly for user messages', () => {
      const { container } = render(<Message message={baseMessage} />);
      
      // For user messages with flex-row-reverse, avatar should be on the right
      const messageContainer = container.querySelector('.group.flex-row-reverse');
      expect(messageContainer).toBeInTheDocument();
      
      // Avatar should be in the container
      const avatar = messageContainer?.querySelector('.flex-shrink-0');
      expect(avatar).toBeInTheDocument();
    });

    it('should position avatar correctly for assistant messages', () => {
      const assistantMessage = {
        ...baseMessage,
        role: 'assistant' as const
      };
      
      const { container } = render(<Message message={assistantMessage} />);
      
      // For assistant messages without flex-row-reverse, avatar should be on the left
      const messageContainer = container.querySelector('.group:not(.flex-row-reverse)');
      expect(messageContainer).toBeInTheDocument();
      
      // Avatar should be in the container
      const avatar = messageContainer?.querySelector('.flex-shrink-0');
      expect(avatar).toBeInTheDocument();
    });
  });

  describe('Message Content Types', () => {
    it('should handle null content gracefully', () => {
      const nullContentMessage: MessageData = {
        ...baseMessage,
        content: null
      };
      
      render(<Message message={nullContentMessage} />);
      
      // MessageContentRenderer should handle null
      const contentRenderer = screen.getByTestId('message-content-renderer');
      expect(contentRenderer).toBeInTheDocument();
      expect(contentRenderer).toHaveAttribute('data-content', 'null');
    });

    it('should handle empty string content', () => {
      const emptyContentMessage: MessageData = {
        ...baseMessage,
        content: ''
      };
      
      render(<Message message={emptyContentMessage} />);
      
      const contentRenderer = screen.getByTestId('message-content-renderer');
      expect(contentRenderer).toBeInTheDocument();
      expect(contentRenderer).toHaveAttribute('data-content', '');
    });

    it('should handle array content', () => {
      const arrayContentMessage: MessageData = {
        ...baseMessage,
        content: [
          { type: 'text', text: 'Part 1' },
          { type: 'text', text: 'Part 2' }
        ] as any
      };
      
      render(<Message message={arrayContentMessage} />);
      
      const contentRenderer = screen.getByTestId('message-content-renderer');
      expect(contentRenderer).toBeInTheDocument();
      // Array content is stringified in our mock
      expect(contentRenderer).toHaveTextContent('Complex content');
    });

    it('should handle long text content', () => {
      const longText = 'Lorem ipsum '.repeat(100);
      const longContentMessage: MessageData = {
        ...baseMessage,
        content: longText
      };
      
      render(<Message message={longContentMessage} />);
      
      const contentRenderer = screen.getByTestId('message-content-renderer');
      expect(contentRenderer).toBeInTheDocument();
      expect(contentRenderer).toHaveAttribute('data-content', longText);
    });
  });

  describe('Message Status Indicators', () => {
    it('should show sending status', () => {
      const sendingMessage: MessageData = {
        ...baseMessage,
        status: 'sending'
      };
      
      render(<Message message={sendingMessage} />);
      
      const sendingText = screen.getByText('Sending...');
      expect(sendingText).toBeInTheDocument();
      expect(sendingText).toHaveClass('text-xs', 'text-muted-foreground');
    });

    it('should show sent status with check icon', () => {
      const sentMessage: MessageData = {
        ...baseMessage,
        status: 'sent'
      };
      
      render(<Message message={sentMessage} />);
      
      const checkIcon = screen.getByTestId('check-icon');
      expect(checkIcon).toBeInTheDocument();
      expect(checkIcon).toHaveClass('h-3', 'w-3', 'text-muted-foreground');
    });

    it('should show error status with error styling', () => {
      const errorMessage: MessageData = {
        ...baseMessage,
        status: 'error',
        error: 'Failed to send message'
      };
      
      const { container } = render(<Message message={errorMessage} />);
      
      // Check for error background
      const errorBubble = container.querySelector('.bg-destructive\\/10');
      expect(errorBubble).toBeInTheDocument();
      expect(errorBubble).toHaveClass('border', 'border-destructive');
      
      // Check for error icon and message
      const alertIcon = screen.getByTestId('alert-icon');
      expect(alertIcon).toBeInTheDocument();
      
      const errorText = screen.getByText('Failed to send message');
      expect(errorText).toBeInTheDocument();
    });

    it('should not show check icon for error messages', () => {
      const errorMessage: MessageData = {
        ...baseMessage,
        status: 'error',
        error: 'Network error'
      };
      
      render(<Message message={errorMessage} />);
      
      const checkIcon = screen.queryByTestId('check-icon');
      expect(checkIcon).not.toBeInTheDocument();
    });
  });

  describe('Component Structure', () => {
    it('should have correct container structure', () => {
      const { container } = render(<Message message={baseMessage} />);
      
      // Main container with group class
      const mainContainer = container.querySelector('.group.relative.flex.gap-3.py-2');
      expect(mainContainer).toBeInTheDocument();
      
      // Avatar container
      const avatarContainer = mainContainer?.querySelector('.flex-shrink-0');
      expect(avatarContainer).toBeInTheDocument();
      
      // Content container
      const contentContainer = mainContainer?.querySelector('.flex-1.max-w-full');
      expect(contentContainer).toBeInTheDocument();
      
      // Message bubble
      const bubble = contentContainer?.querySelector('.relative.rounded-xl.px-4.py-2\\.5');
      expect(bubble).toBeInTheDocument();
    });

    it('should apply transition classes to message bubble', () => {
      const { container } = render(<Message message={baseMessage} />);
      
      const bubble = container.querySelector('.relative.rounded-xl');
      expect(bubble).toHaveClass('transition-all', 'duration-200');
    });

    it('should have proper spacing between elements', () => {
      const { container } = render(<Message message={baseMessage} />);
      
      // Gap between avatar and content
      const mainContainer = container.querySelector('.gap-3');
      expect(mainContainer).toBeInTheDocument();
      
      // Vertical spacing in content
      const contentContainer = container.querySelector('.space-y-2');
      expect(contentContainer).toBeInTheDocument();
    });
  });

  describe('Accessibility', () => {
    it('should have proper ARIA attributes for user messages', () => {
      const { container } = render(<Message message={baseMessage} />);
      
      const article = container.querySelector('[role="article"]');
      expect(article).toHaveAttribute('aria-label', 'Message from user');
    });

    it('should have proper ARIA attributes for assistant messages', () => {
      const assistantMessage = {
        ...baseMessage,
        role: 'assistant' as const
      };
      
      const { container } = render(<Message message={assistantMessage} />);
      
      const article = container.querySelector('[role="article"]');
      expect(article).toHaveAttribute('aria-label', 'Message from assistant');
    });

    it('should maintain semantic HTML structure', () => {
      const { container } = render(<Message message={baseMessage} />);
      
      // Should use div with article role for semantic meaning
      const article = container.querySelector('[role="article"]');
      expect(article).toBeInTheDocument();
    });
  });

  describe('Props Forwarding', () => {
    it('should forward className prop', () => {
      const { container } = render(
        <Message message={baseMessage} className="custom-class" />
      );
      
      const messageElement = container.querySelector('.group');
      expect(messageElement).toHaveClass('custom-class');
    });

    it('should forward data attributes', () => {
      const { container } = render(
        <Message 
          message={baseMessage} 
          data-testid="custom-message"
          data-custom="value"
        />
      );
      
      const messageElement = container.querySelector('[role="article"]');
      expect(messageElement).toHaveAttribute('data-testid', 'custom-message');
      expect(messageElement).toHaveAttribute('data-custom', 'value');
    });

    it('should forward ref', () => {
      const ref = React.createRef<HTMLDivElement>();
      
      render(<Message message={baseMessage} ref={ref} />);
      
      expect(ref.current).toBeInstanceOf(HTMLDivElement);
      expect(ref.current).toHaveAttribute('role', 'article');
    });
  });

  describe('Edge Cases', () => {
    it('should handle message without id', () => {
      const messageWithoutId = {
        role: 'user' as const,
        content: 'Test',
        timestamp: new Date()
      };
      
      expect(() => {
        render(<Message message={messageWithoutId} />);
      }).not.toThrow();
    });

    it('should handle undefined optional props', () => {
      expect(() => {
        render(
          <Message 
            message={baseMessage}
            showTimestamp={undefined}
            isStreaming={undefined}
            avatarComponent={undefined}
            onEdit={undefined}
            showFooter={undefined}
          />
        );
      }).not.toThrow();
    });

    it('should handle rapid prop changes', () => {
      const { rerender } = render(<Message message={baseMessage} />);
      
      const messages = [
        { ...baseMessage, role: 'assistant' as const },
        { ...baseMessage, role: 'user' as const },
        { ...baseMessage, role: 'system' as const }
      ];
      
      messages.forEach(msg => {
        expect(() => {
          rerender(<Message message={msg} />);
        }).not.toThrow();
      });
    });
  });
});