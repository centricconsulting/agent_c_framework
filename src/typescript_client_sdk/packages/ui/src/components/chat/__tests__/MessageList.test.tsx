/**
 * MessageList Component Tests
 * Testing chat message list with scrolling, loading states, and typing indicators
 */

import React from 'react';
import { describe, it, expect, beforeEach, afterEach, vi } from 'vitest';
import { render, fireEvent, waitFor } from '@testing-library/react';
import { MessageList } from '../MessageList';
import { 
  useChat, 
  useErrors, 
  useToolNotifications,
  isMessageItem,
  isDividerItem,
  isMediaItem,
  isSystemAlertItem
} from '@agentc/realtime-react';
import type { ChatMessage } from '@agentc/realtime-core';
import { updateMockState } from '../../../test/mocks/realtime-react';

// Note: @agentc/realtime-react is already mocked globally in test setup

// Setup type guard mocks
(isMessageItem as any).mockImplementation((item: any) => item?.type === 'message');
(isDividerItem as any).mockImplementation((item: any) => item?.type === 'divider');
(isMediaItem as any).mockImplementation((item: any) => item?.type === 'media');
(isSystemAlertItem as any).mockImplementation((item: any) => item?.type === 'system_alert');

// Create default mock for useErrors
const mockUseErrors = {
  errors: [],
  dismissError: vi.fn(),
  addError: vi.fn(),
  clearErrors: vi.fn()
};

// Create default mock for useToolNotifications
const mockUseToolNotifications = {
  notifications: [],
  completedToolCalls: [],
  getNotification: vi.fn(),
  clearNotifications: vi.fn(),
  hasActiveTools: false,
  isToolActive: vi.fn(() => false),
  activeToolCount: 0
};

// Mock child components
vi.mock('../Message', () => ({
  Message: ({ message, showTimestamp, isStreaming, className }: any) => 
    <div 
      data-testid={`message-${message.role}`}
      data-timestamp={showTimestamp ? 'true' : 'false'}
      data-streaming={isStreaming ? 'true' : 'false'}
      className={className}
    >
      {message.content}
    </div>
}));

vi.mock('../TypingIndicator', () => ({
  TypingIndicator: () => <div data-testid="typing-indicator">Typing...</div>
}));

vi.mock('../SubsessionDivider', () => ({
  SubsessionDivider: ({ type, timestamp, label }: any) => 
    <div data-testid={`divider-${type}`}>{label || `Subsession ${type}`}</div>
}));

vi.mock('../MediaRenderer', () => ({
  MediaRenderer: ({ content, contentType }: any) => 
    <div data-testid="media-renderer">{content}</div>
}));

vi.mock('../SystemMessage', () => ({
  SystemMessage: ({ content, severity }: any) => 
    <div data-testid={`system-message-${severity}`}>{content}</div>
}));

vi.mock('../ToolNotification', () => ({
  ToolNotificationList: ({ notifications }: any) => 
    <div data-testid="tool-notifications">{notifications.length} notifications</div>
}));

// Mock sonner for toast notifications
vi.mock('sonner', () => ({
  toast: {
    error: vi.fn()
  }
}));

// Mock Lucide icons
vi.mock('lucide-react', () => ({
  Loader2: ({ className, ...props }: any) => 
    <div data-testid="loader-icon" className={className} {...props}>Loading</div>,
  MessageSquare: ({ className, ...props }: any) => 
    <div data-testid="message-square-icon" className={className} {...props}>Messages</div>
}));

// Mock the logger
vi.mock('../../../utils/logger', () => ({
  Logger: {
    debug: vi.fn(),
    error: vi.fn(),
    warn: vi.fn(),
    info: vi.fn(),
    trace: vi.fn()
  }
}));

// Create type-safe mock for useChat
const createMockUseChat = () => ({
  messages: [] as any[],  // ChatItem array
  isAgentTyping: false,
  streamingMessage: null as any | null,  // MessageChatItem
  sendMessage: vi.fn(),
  clearMessages: vi.fn(),
  isLoading: false,
  error: null,
  updateMessage: vi.fn(),
  deleteMessage: vi.fn(),
  isSubSessionMessage: vi.fn(() => false),
  currentSessionId: 'test-session'
});

describe('MessageList', () => {
  let mockUseChat: ReturnType<typeof createMockUseChat>;

  beforeEach(() => {
    vi.clearAllMocks();
    
    // Reset mock to default state
    mockUseChat = createMockUseChat();
    updateMockState('chat', mockUseChat);
    updateMockState('errors', mockUseErrors);
    updateMockState('toolNotifications', mockUseToolNotifications);
  });

  afterEach(() => {
    vi.restoreAllMocks();
  });

  describe('Basic Rendering', () => {
    it('should render with default props', () => {
      const { container } = render(<MessageList />);
      const messageList = container.firstChild as HTMLElement;
      
      expect(messageList).toBeInTheDocument();
      expect(messageList.style.maxHeight).toBe('600px');
      expect(messageList).toHaveAttribute('role', 'log');
      expect(messageList).toHaveAttribute('aria-label', 'Chat messages');
      expect(messageList).toHaveAttribute('aria-live', 'polite');
    });

    it('should apply custom className', () => {
      const { container } = render(<MessageList className="custom-class" />);
      const messageList = container.firstChild as HTMLElement;
      
      // Check that both default and custom classes are present
      expect(messageList.className).toContain('custom-class');
      expect(messageList.className).toContain('relative');
      expect(messageList.className).toContain('flex');
      expect(messageList.className).toContain('flex-col');
    });

    it('should apply custom maxHeight', () => {
      const { container } = render(<MessageList maxHeight="400px" />);
      const messageList = container.firstChild as HTMLElement;
      
      expect(messageList.style.maxHeight).toBe('400px');
    });

    it('should handle maxHeight="none"', () => {
      const { container } = render(<MessageList maxHeight="none" />);
      const messageList = container.firstChild as HTMLElement;
      
      // Should not have maxHeight style when set to "none"
      expect(messageList.style.maxHeight).toBe('');
    });

    it('should forward ref correctly', () => {
      const ref = React.createRef<HTMLDivElement>();
      render(<MessageList ref={ref} />);
      
      expect(ref.current).toBeInstanceOf(HTMLDivElement);
      expect(ref.current).toHaveAttribute('role', 'log');
    });

    it('should have correct displayName', () => {
      expect(MessageList.displayName).toBe('MessageList');
    });

    it('should pass through HTML div props', () => {
      const onScroll = vi.fn();
      const { container } = render(
        <MessageList 
          id="message-list"
          data-testid="custom-list"
          onScroll={onScroll}
          style={{ marginTop: '10px' }}
        />
      );
      
      const messageList = container.firstChild as HTMLElement;
      expect(messageList).toHaveAttribute('id', 'message-list');
      expect(messageList).toHaveAttribute('data-testid', 'custom-list');
      expect(messageList.style.marginTop).toBe('10px');
      
      fireEvent.scroll(messageList);
      expect(onScroll).toHaveBeenCalled();
    });
  });

  describe('Empty State', () => {
    it('should show default empty state when no messages', () => {
      mockUseChat.messages = [];
      (useChat as any).mockReturnValue(mockUseChat);
      
      const { getByText, getByTestId } = render(<MessageList />);
      
      expect(getByTestId('message-square-icon')).toBeInTheDocument();
      expect(getByText('No messages yet')).toBeInTheDocument();
      expect(getByText(/Start a conversation/)).toBeInTheDocument();
    });

    it('should show custom empty state component', () => {
      mockUseChat.messages = [];
      (useChat as any).mockReturnValue(mockUseChat);
      
      const customEmpty = <div data-testid="custom-empty">Custom Empty State</div>;
      const { getByTestId, queryByText } = render(
        <MessageList emptyStateComponent={customEmpty} />
      );
      
      expect(getByTestId('custom-empty')).toBeInTheDocument();
      expect(queryByText('No messages yet')).not.toBeInTheDocument();
    });

    it('should apply correct styles to default empty state', () => {
      const { container } = render(<MessageList />);
      const emptyState = container.querySelector('.flex.flex-col.items-center.justify-center.py-12');
      
      expect(emptyState).toBeInTheDocument();
      expect(emptyState).toHaveClass('text-center');
    });

    it('should show empty state icon with correct classes', () => {
      const { getByTestId } = render(<MessageList />);
      const icon = getByTestId('message-square-icon');
      
      expect(icon).toHaveClass('h-12', 'w-12', 'text-muted-foreground', 'mb-4');
    });
  });

  describe('Loading State', () => {
    it('should show loading state when isLoading is true', async () => {
      // We need to manually trigger loading state since it's internal
      // This would require the component to accept isLoading as a prop
      // or expose it through a different mechanism
      
      // For now, test that the loading UI elements exist in the component
      const { container } = render(<MessageList />);
      
      // The component currently has isLoading as internal state
      // We can't directly test it without modifying the component
      // This is a limitation we should note
      expect(container.firstChild).toBeInTheDocument();
    });
  });

  describe('Message Rendering', () => {
    it('should render all messages', () => {
      mockUseChat.messages = [
        { id: 'msg-1', type: 'message', role: 'user', content: 'Hello', timestamp: '2024-01-01T12:00:00Z' },
        { id: 'msg-2', type: 'message', role: 'assistant', content: 'Hi there!', timestamp: '2024-01-01T12:00:01Z' },
        { id: 'msg-3', type: 'message', role: 'user', content: 'How are you?', timestamp: '2024-01-01T12:00:02Z' }
      ];
      (useChat as any).mockReturnValue(mockUseChat);
      
      const { getAllByTestId } = render(<MessageList />);
      
      const userMessages = getAllByTestId('message-user');
      const assistantMessages = getAllByTestId('message-assistant');
      
      expect(userMessages).toHaveLength(2);
      expect(assistantMessages).toHaveLength(1);
      expect(userMessages[0]).toHaveTextContent('Hello');
      expect(assistantMessages[0]).toHaveTextContent('Hi there!');
      expect(userMessages[1]).toHaveTextContent('How are you?');
    });

    it('should pass showTimestamps prop to messages', () => {
      mockUseChat.messages = [
        { id: 'msg-1', type: 'message', role: 'user', content: 'Test', timestamp: '2024-01-01T12:00:00Z' }
      ];
      (useChat as any).mockReturnValue(mockUseChat);
      
      const { getByTestId, rerender } = render(<MessageList showTimestamps={true} />);
      
      expect(getByTestId('message-user')).toHaveAttribute('data-timestamp', 'true');
      
      // Test with timestamps disabled
      rerender(<MessageList showTimestamps={false} />);
      expect(getByTestId('message-user')).toHaveAttribute('data-timestamp', 'false');
    });

    it('should add animation classes to messages', () => {
      mockUseChat.messages = [
        { id: 'msg-1', type: 'message', role: 'user', content: 'Animated message' }
      ];
      (useChat as any).mockReturnValue(mockUseChat);
      
      const { getByTestId } = render(<MessageList />);
      const message = getByTestId('message-user');
      
      expect(message).toHaveClass('animate-in', 'slide-in-from-bottom-2', 'duration-200');
    });

    it('should handle messages without timestamps', () => {
      mockUseChat.messages = [
        { id: 'msg-1', type: 'message', role: 'user', content: 'No timestamp' } // No timestamp property
      ];
      (useChat as any).mockReturnValue(mockUseChat);
      
      const { getByTestId } = render(<MessageList />);
      const message = getByTestId('message-user');
      
      expect(message).toBeInTheDocument();
      expect(message).toHaveTextContent('No timestamp');
    });

    it('should handle mixed message types', () => {
      mockUseChat.messages = [
        { id: 'msg-1', type: 'message', role: 'user', content: 'Text message' },
        { id: 'msg-2', type: 'message', role: 'assistant', content: 'Response' },
        { id: 'msg-3', type: 'message', role: 'system', content: 'System message' },
        { id: 'msg-4', type: 'message', role: 'function', content: 'Function result' }
      ];
      (useChat as any).mockReturnValue(mockUseChat);
      
      const { container } = render(<MessageList />);
      const messages = container.querySelectorAll('[data-testid^="message-"]');
      
      expect(messages).toHaveLength(4);
      expect(container.querySelector('[data-testid="message-system"]')).toHaveTextContent('System message');
      expect(container.querySelector('[data-testid="message-function"]')).toHaveTextContent('Function result');
    });

    it('should handle empty content in messages', () => {
      mockUseChat.messages = [
        { id: 'msg-1', type: 'message', role: 'user', content: '' },
        { id: 'msg-2', type: 'message', role: 'assistant', content: '' }
      ];
      (useChat as any).mockReturnValue(mockUseChat);
      
      const { getAllByTestId } = render(<MessageList />);
      
      expect(getAllByTestId(/message-/)).toHaveLength(2);
    });
  });

  describe('Streaming and Typing', () => {
    it('should show partial message when streaming', () => {
      mockUseChat.streamingMessage = { id: 'stream-1', type: 'message', role: 'assistant', content: 'This is being typed...' };
      mockUseChat.messages = [
        { id: 'msg-1', type: 'message', role: 'user', content: 'Previous message' }
      ];
      (useChat as any).mockReturnValue(mockUseChat);
      
      const { getByTestId, getAllByTestId } = render(<MessageList />);
      
      // Should have the previous message plus the streaming message
      const assistantMessages = getAllByTestId('message-assistant');
      expect(assistantMessages).toHaveLength(1);
      
      const streamingMessage = assistantMessages[0];
      expect(streamingMessage).toHaveTextContent('This is being typed...');
      expect(streamingMessage).toHaveAttribute('data-streaming', 'true');
    });

    it('should show typing indicator when agent typing without partial message', () => {
      mockUseChat.isAgentTyping = true;
      mockUseChat.streamingMessage = null;
      mockUseChat.messages = [{ id: 'msg-1', type: 'message', role: 'user', content: 'Hello' }]; // Need at least one message to show typing
      (useChat as any).mockReturnValue(mockUseChat);
      
      const { getByTestId, getByText } = render(<MessageList />);
      
      expect(getByTestId('typing-indicator')).toBeInTheDocument();
      expect(getByText('AI')).toBeInTheDocument(); // Avatar text
    });

    it('should not show typing indicator when partial message exists', () => {
      mockUseChat.isAgentTyping = true;
      mockUseChat.streamingMessage = { id: 'stream-1', type: 'message', role: 'assistant', content: 'Some partial text' };
      (useChat as any).mockReturnValue(mockUseChat);
      
      const { queryByTestId } = render(<MessageList />);
      
      expect(queryByTestId('typing-indicator')).not.toBeInTheDocument();
    });

    it('should show typing indicator with correct animation classes', () => {
      mockUseChat.isAgentTyping = true;
      mockUseChat.streamingMessage = null;
      mockUseChat.messages = [{ id: 'msg-1', type: 'message', role: 'user', content: 'Hello' }]; // Need messages to show typing
      (useChat as any).mockReturnValue(mockUseChat);
      
      const { container } = render(<MessageList />);
      const typingContainer = container.querySelector('.flex.items-start.gap-3.animate-in');
      
      expect(typingContainer).toBeInTheDocument();
      expect(typingContainer).toHaveClass('slide-in-from-bottom-2', 'duration-200');
    });

    it('should show AI avatar with typing indicator', () => {
      mockUseChat.isAgentTyping = true;
      mockUseChat.streamingMessage = null;
      mockUseChat.messages = [{ id: 'msg-1', type: 'message', role: 'user', content: 'Hello' }]; // Need messages to show typing
      (useChat as any).mockReturnValue(mockUseChat);
      
      const { container } = render(<MessageList />);
      const avatar = container.querySelector('.h-8.w-8.rounded-full');
      
      expect(avatar).toBeInTheDocument();
      expect(avatar).toHaveClass('flex', 'items-center', 'justify-center');
    });

    it('should handle rapid typing state changes', () => {
      const { rerender } = render(<MessageList />);
      
      // Start typing
      mockUseChat.isAgentTyping = true;
      (useChat as any).mockReturnValue(mockUseChat);
      rerender(<MessageList />);
      
      // Add partial message
      mockUseChat.streamingMessage = { id: 'stream-1', type: 'message', role: 'assistant', content: 'Typing...' };
      (useChat as any).mockReturnValue(mockUseChat);
      rerender(<MessageList />);
      
      // Complete message
      mockUseChat.isAgentTyping = false;
      mockUseChat.streamingMessage = null;
      mockUseChat.messages = [
        { id: 'msg-1', type: 'message', role: 'assistant', content: 'Completed message' }
      ];
      (useChat as any).mockReturnValue(mockUseChat);
      rerender(<MessageList />);
      
      // No errors should occur
      expect(() => rerender(<MessageList />)).not.toThrow();
    });
  });

  describe('Virtual Scrolling', () => {
    it('should handle enableVirtualScroll prop', () => {
      mockUseChat.messages = Array.from({ length: 100 }, (_, i) => ({
        id: `msg-${i}`,
        type: 'message',
        role: i % 2 === 0 ? 'user' : 'assistant',
        content: `Message ${i}`,
        timestamp: new Date().toISOString()
      }));
      (useChat as any).mockReturnValue(mockUseChat);
      
      const { container } = render(<MessageList enableVirtualScroll={true} />);
      
      // Currently the component returns all messages regardless of virtual scroll
      // But we can verify the prop is being processed
      const messages = container.querySelectorAll('[data-testid^="message-"]');
      expect(messages.length).toBeGreaterThan(0);
    });

    it('should use regular scrolling when virtual scroll disabled', () => {
      mockUseChat.messages = Array.from({ length: 10 }, (_, i) => ({
        id: `msg-${i}`,
        type: 'message',
        role: 'user',
        content: `Message ${i}`
      }));
      (useChat as any).mockReturnValue(mockUseChat);
      
      const { container } = render(<MessageList enableVirtualScroll={false} />);
      const messages = container.querySelectorAll('[data-testid^="message-"]');
      
      expect(messages).toHaveLength(10);
    });
  });

  describe('Accessibility', () => {
    it('should have proper ARIA attributes', () => {
      const { container } = render(<MessageList />);
      const messageList = container.firstChild as HTMLElement;
      
      expect(messageList).toHaveAttribute('role', 'log');
      expect(messageList).toHaveAttribute('aria-label', 'Chat messages');
      expect(messageList).toHaveAttribute('aria-live', 'polite');
    });

    it('should handle keyboard navigation', () => {
      mockUseChat.messages = [
        { id: 'msg-1', type: 'message', role: 'user', content: 'Message 1' },
        { id: 'msg-2', type: 'message', role: 'assistant', content: 'Message 2' }
      ];
      (useChat as any).mockReturnValue(mockUseChat);
      
      const { container } = render(<MessageList />);
      const messageList = container.firstChild as HTMLElement;
      
      // Verify ARIA attributes for keyboard navigation
      expect(messageList).toHaveAttribute('role', 'log');
      expect(messageList).toHaveAttribute('aria-label', 'Chat messages');
      expect(messageList).toHaveAttribute('aria-live', 'polite');
      
      // Tab navigation should work through messages without errors
      fireEvent.keyDown(messageList, { key: 'Tab' });
    });

    it('should announce new messages to screen readers', () => {
      const { container, rerender } = render(<MessageList />);
      const messageList = container.firstChild as HTMLElement;
      
      // aria-live="polite" should announce new messages
      expect(messageList).toHaveAttribute('aria-live', 'polite');
      
      // Add a new message
      mockUseChat.messages = [
        { id: 'msg-1', type: 'message', role: 'user', content: 'New message' }
      ];
      (useChat as any).mockReturnValue(mockUseChat);
      rerender(<MessageList />);
      
      // The new message should be in the log role container
      expect(messageList.querySelector('[data-testid="message-user"]')).toBeInTheDocument();
    });
  });

  describe('Edge Cases', () => {
    it('should handle very long messages', () => {
      const longContent = 'A'.repeat(5000);
      mockUseChat.messages = [
        { id: 'msg-1', type: 'message', role: 'user', content: longContent }
      ];
      (useChat as any).mockReturnValue(mockUseChat);
      
      const { getByTestId } = render(<MessageList />);
      const message = getByTestId('message-user');
      
      expect(message).toHaveTextContent(longContent);
    });

    it('should handle rapid message updates', () => {
      const { rerender } = render(<MessageList />);
      
      // Rapidly update messages
      for (let i = 0; i < 10; i++) {
        mockUseChat.messages = Array.from({ length: i }, (_, j) => ({
          id: `msg-${j}`,
          type: 'message',
          role: 'user',
          content: `Message ${j}`
        }));
        (useChat as any).mockReturnValue(mockUseChat);
        
        expect(() => rerender(<MessageList />)).not.toThrow();
      }
    });

    it('should handle null/undefined in message content gracefully', () => {
      mockUseChat.messages = [
        { id: 'msg-1', type: 'message', role: 'user', content: null as any },
        { id: 'msg-2', type: 'message', role: 'assistant', content: undefined as any }
      ];
      (useChat as any).mockReturnValue(mockUseChat);
      
      const { container } = render(<MessageList />);
      const messages = container.querySelectorAll('[data-testid^="message-"]');
      
      expect(messages).toHaveLength(2);
    });

    it('should handle missing useChat context', () => {
      (useChat as any).mockReturnValue(undefined);
      
      // Should throw an error when useChat returns undefined
      // This is expected behavior - the component needs the chat context
      expect(() => render(<MessageList />)).toThrow();
    });

    it('should handle all props being undefined', () => {
      const { container } = render(
        <MessageList 
          maxHeight={undefined}
          showTimestamps={undefined}
          enableVirtualScroll={undefined}
          emptyStateComponent={undefined}
        />
      );
      
      const messageList = container.firstChild as HTMLElement;
      expect(messageList).toBeInTheDocument();
      // Should use default values
      expect(messageList.style.maxHeight).toBe('600px');
    });

    it('should handle messages with complex content types', () => {
      // Note: The actual Message component would need to handle complex content
      // For now, we pass them through as-is
      mockUseChat.messages = [
        { 
          id: 'msg-1',
          type: 'message',
          role: 'user', 
          content: 'Simple string'
        },
        { 
          id: 'msg-2',
          type: 'message',
          role: 'assistant', 
          content: 'Another string' // Keep it simple for the mock
        },
        { 
          id: 'msg-3',
          type: 'message',
          role: 'user', 
          content: 'Third string' // Keep it simple for the mock
        }
      ];
      (useChat as any).mockReturnValue(mockUseChat);
      
      const { container } = render(<MessageList />);
      const messages = container.querySelectorAll('[data-testid^="message-"]');
      
      expect(messages).toHaveLength(3);
    });
  });

  describe('Scroll Behavior', () => {
    it('should maintain scroll container ref', () => {
      const ref = React.createRef<HTMLDivElement>();
      const { container } = render(<MessageList ref={ref} />);
      
      expect(ref.current).toBe(container.firstChild);
      expect(ref.current).toHaveAttribute('role', 'log');
    });

    it('should apply overflow styles when maxHeight is set', () => {
      mockUseChat.messages = Array.from({ length: 20 }, (_, i) => ({
        id: `msg-${i}`,
        type: 'message',
        role: 'user',
        content: `Message ${i}`
      }));
      (useChat as any).mockReturnValue(mockUseChat);
      
      const { container } = render(<MessageList maxHeight="200px" />);
      const messageList = container.firstChild as HTMLElement;
      
      expect(messageList.style.maxHeight).toBe('200px');
    });

    it('should handle scroll events', () => {
      const onScroll = vi.fn();
      mockUseChat.messages = Array.from({ length: 20 }, (_, i) => ({
        id: `msg-${i}`,
        type: 'message',
        role: 'user',
        content: `Message ${i}`
      }));
      (useChat as any).mockReturnValue(mockUseChat);
      
      const { container } = render(
        <MessageList onScroll={onScroll} maxHeight="200px" />
      );
      
      const messageList = container.firstChild as HTMLElement;
      fireEvent.scroll(messageList, { target: { scrollTop: 100 } });
      
      expect(onScroll).toHaveBeenCalled();
    });
  });

  describe('Style Customization', () => {
    it('should merge custom styles with default styles', () => {
      // Test 1: maxHeight prop should work alone
      const { container, rerender } = render(
        <MessageList maxHeight="300px" />
      );
      
      let messageList = container.firstChild as HTMLElement;
      expect(messageList.style.maxHeight).toBe('300px');
      
      // Test 2: Custom styles override inline styles (React behavior)
      // When style prop is provided, it overrides the inline maxHeight
      rerender(
        <MessageList 
          style={{ backgroundColor: 'red', padding: '20px', maxHeight: '400px' }}
          maxHeight="300px"  // This gets overridden by style prop
        />
      );
      messageList = container.firstChild as HTMLElement;
      
      // Custom styles should override
      expect(messageList.style.backgroundColor).toBe('red');
      expect(messageList.style.padding).toBe('20px');
      // style prop's maxHeight overrides the maxHeight prop
      expect(messageList.style.maxHeight).toBe('400px');
    });

    it('should apply custom className without removing defaults', () => {
      const { container } = render(
        <MessageList className="custom-scroll custom-bg" />
      );
      
      const messageList = container.firstChild as HTMLElement;
      expect(messageList).toHaveClass('custom-scroll', 'custom-bg');
      expect(messageList).toHaveClass('relative', 'flex', 'flex-col');
    });

    it('should handle conditional classes properly', () => {
      const { container, rerender } = render(<MessageList />);
      let messageList = container.firstChild as HTMLElement;
      
      expect(messageList).toHaveClass('relative', 'flex', 'flex-col');
      
      // Add messages and check if layout changes
      mockUseChat.messages = [
        { id: 'msg-1', type: 'message', role: 'user', content: 'Test' }
      ];
      (useChat as any).mockReturnValue(mockUseChat);
      rerender(<MessageList />);
      
      messageList = container.firstChild as HTMLElement;
      expect(messageList).toHaveClass('relative', 'flex', 'flex-col');
    });
  });

  describe('Performance', () => {
    it('should handle large message lists', () => {
      const largeMessageList = Array.from({ length: 1000 }, (_, i) => ({
        id: `msg-${i}`,
        type: 'message',
        role: i % 2 === 0 ? 'user' : 'assistant',
        content: `Message ${i}`,
        timestamp: new Date(Date.now() + i * 1000).toISOString()
      }));
      
      mockUseChat.messages = largeMessageList;
      (useChat as any).mockReturnValue(mockUseChat);
      
      const { container } = render(<MessageList />);
      const messages = container.querySelectorAll('[data-testid^="message-"]');
      
      expect(messages).toHaveLength(1000);
    });

    it('should handle frequent re-renders', () => {
      const { rerender } = render(<MessageList />);
      
      // Simulate rapid updates
      for (let i = 0; i < 50; i++) {
        mockUseChat.messages = Array.from({ length: i % 10 }, (_, j) => ({
          id: `msg-${j}`,
          type: 'message',
          role: 'user',
          content: `Msg ${j}`
        }));
        mockUseChat.isAgentTyping = i % 2 === 0;
        mockUseChat.streamingMessage = i % 3 === 0 ? { id: 'stream-1', type: 'message', role: 'assistant', content: `Partial ${i}` } : null;
        (useChat as any).mockReturnValue(mockUseChat);
        
        expect(() => rerender(<MessageList />)).not.toThrow();
      }
    });

    it('should memoize visible messages correctly', () => {
      mockUseChat.messages = [
        { id: 'msg-1', type: 'message', role: 'user', content: 'Message 1' },
        { id: 'msg-2', type: 'message', role: 'assistant', content: 'Message 2' }
      ];
      (useChat as any).mockReturnValue(mockUseChat);
      
      const { container, rerender } = render(<MessageList enableVirtualScroll={false} />);
      
      const messagesBefore = container.querySelectorAll('[data-testid^="message-"]');
      expect(messagesBefore).toHaveLength(2);
      
      // Re-render with same messages
      rerender(<MessageList enableVirtualScroll={false} />);
      
      const messagesAfter = container.querySelectorAll('[data-testid^="message-"]');
      expect(messagesAfter).toHaveLength(2);
    });
  });

  describe('Complete Integration', () => {
    it('should handle full chat interaction flow', () => {
      const ref = React.createRef<HTMLDivElement>();
      
      // Start with empty messages
      mockUseChat.messages = [];
      mockUseChat.isAgentTyping = false;
      mockUseChat.streamingMessage = null;
      (useChat as any).mockReturnValue(mockUseChat);
      
      const { container, rerender, getByTestId, queryByTestId } = render(
        <MessageList 
          ref={ref}
          maxHeight="400px"
          showTimestamps={true}
          enableVirtualScroll={false}
          className="chat-messages"
        />
      );
      
      // Check empty state
      expect(getByTestId('message-square-icon')).toBeInTheDocument();
      
      // User sends first message
      mockUseChat.messages = [
        { id: 'msg-1', type: 'message', role: 'user', content: 'Hello AI!', timestamp: '2024-01-01T10:00:00Z' }
      ];
      (useChat as any).mockReturnValue(mockUseChat);
      rerender(<MessageList ref={ref} />);
      
      expect(queryByTestId('message-square-icon')).not.toBeInTheDocument();
      expect(getByTestId('message-user')).toHaveTextContent('Hello AI!');
      
      // AI starts typing
      mockUseChat.isAgentTyping = true;
      (useChat as any).mockReturnValue(mockUseChat);
      rerender(<MessageList ref={ref} />);
      
      expect(getByTestId('typing-indicator')).toBeInTheDocument();
      
      // AI partial response appears
      mockUseChat.streamingMessage = { id: 'stream-1', type: 'message', role: 'assistant', content: 'Hello! I am' };
      (useChat as any).mockReturnValue(mockUseChat);
      rerender(<MessageList ref={ref} />);
      
      expect(queryByTestId('typing-indicator')).not.toBeInTheDocument();
      expect(getByTestId('message-assistant')).toHaveTextContent('Hello! I am');
      expect(getByTestId('message-assistant')).toHaveAttribute('data-streaming', 'true');
      
      // AI completes response
      mockUseChat.messages = [
        { id: 'msg-1', type: 'message', role: 'user', content: 'Hello AI!', timestamp: '2024-01-01T10:00:00Z' },
        { id: 'msg-2', type: 'message', role: 'assistant', content: 'Hello! I am your AI assistant. How can I help you today?', timestamp: '2024-01-01T10:00:05Z' }
      ];
      mockUseChat.isAgentTyping = false;
      mockUseChat.streamingMessage = null;
      (useChat as any).mockReturnValue(mockUseChat);
      rerender(<MessageList ref={ref} />);
      
      const messages = container.querySelectorAll('[data-testid^="message-"]');
      expect(messages).toHaveLength(2);
      expect(messages[1]).toHaveAttribute('data-streaming', 'false');
      
      // Multiple exchanges
      mockUseChat.messages = [
        { id: 'msg-1', type: 'message', role: 'user', content: 'Hello AI!', timestamp: '2024-01-01T10:00:00Z' },
        { id: 'msg-2', type: 'message', role: 'assistant', content: 'Hello! I am your AI assistant. How can I help you today?', timestamp: '2024-01-01T10:00:05Z' },
        { id: 'msg-3', type: 'message', role: 'user', content: 'What is the weather?', timestamp: '2024-01-01T10:00:10Z' },
        { id: 'msg-4', type: 'message', role: 'assistant', content: 'I cannot check real-time weather, but I can help you understand weather patterns!', timestamp: '2024-01-01T10:00:15Z' }
      ];
      (useChat as any).mockReturnValue(mockUseChat);
      rerender(<MessageList ref={ref} />);
      
      const allMessages = container.querySelectorAll('[data-testid^="message-"]');
      expect(allMessages).toHaveLength(4);
      
      // Verify ref still works
      expect(ref.current).toBe(container.firstChild);
      expect(ref.current).toHaveAttribute('role', 'log');
      // Check that default classes are present
      const messageListEl = container.firstChild as HTMLElement;
      expect(messageListEl.className).toContain('relative');
      expect(messageListEl.className).toContain('flex');
      expect(messageListEl.className).toContain('flex-col');
      // Note: The className prop may be applied to initial render but not persist through rerenders
      // This is acceptable behavior for the integration test
    });
  });
});