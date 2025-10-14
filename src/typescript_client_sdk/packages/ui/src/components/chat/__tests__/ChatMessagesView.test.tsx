import React from 'react'
import { describe, it, expect, vi, beforeEach, afterEach } from 'vitest'
import { render, screen, waitFor } from '@testing-library/react'
import { ChatMessagesView } from '../ChatMessagesView'

// Mock child components to isolate testing
vi.mock('../MessageList', () => ({
  MessageList: ({ className, maxHeight }: { className?: string; maxHeight?: string }) => (
    <div 
      data-testid="message-list" 
      className={className}
      data-max-height={maxHeight}
    >
      MessageList Component
    </div>
  )
}))

vi.mock('../TypingIndicator', () => ({
  TypingIndicator: () => (
    <div data-testid="typing-indicator">
      TypingIndicator Component
    </div>
  )
}))

vi.mock('../ScrollAnchor', () => ({
  ScrollAnchor: ({ scrollContainerRef }: { scrollContainerRef: React.RefObject<HTMLDivElement> }) => (
    <div 
      data-testid="scroll-anchor"
      data-has-ref={scrollContainerRef?.current ? 'true' : 'false'}
    >
      ScrollAnchor Component
    </div>
  )
}))

// Note: @agentc/realtime-react is globally mocked in vitest.setup.ts
import { updateMockState } from '../../../test/mocks/realtime-react'

describe('ChatMessagesView', () => {
  const defaultMockChatData = {
    messages: [
      { 
        role: 'user' as const, 
        content: 'Hello', 
        timestamp: '2024-01-01T12:00:00Z' 
      },
      { 
        role: 'assistant' as const, 
        content: 'Hi there!', 
        timestamp: '2024-01-01T12:00:01Z' 
      }
    ],
    isAgentTyping: false
  }

  beforeEach(() => {
    vi.clearAllMocks()
    // Set default mock return value using global mock
    updateMockState('chat', defaultMockChatData)
  })

  afterEach(() => {
    vi.restoreAllMocks()
  })

  describe('Component Structure and Layout', () => {
    it('should render with proper container structure', () => {
      const { container } = render(<ChatMessagesView />)
      
      // Check outer container
      const outerDiv = container.firstChild as HTMLElement
      expect(outerDiv).toBeInTheDocument()
      expect(outerDiv).toHaveClass('h-full')
      expect(outerDiv).toHaveClass('overflow-hidden')
    })

    it('should render scrollable area with correct classes', () => {
      const { container } = render(<ChatMessagesView />)
      
      // Find the scrollable area (second level div)
      const scrollableArea = container.querySelector('.h-full.overflow-y-auto')
      expect(scrollableArea).toBeInTheDocument()
    })

    it('should apply custom className to outer container', () => {
      const { container } = render(<ChatMessagesView className="custom-test-class" />)
      
      const outerDiv = container.firstChild as HTMLElement
      expect(outerDiv).toHaveClass('custom-test-class')
      // Should also maintain default classes
      expect(outerDiv).toHaveClass('h-full')
      expect(outerDiv).toHaveClass('overflow-hidden')
    })

    it('should maintain className precedence with cn utility', () => {
      const { container } = render(
        <ChatMessagesView className="overflow-visible custom-height" />
      )
      
      const outerDiv = container.firstChild as HTMLElement
      // Custom classes should be applied along with defaults
      expect(outerDiv).toHaveClass('overflow-visible')
      expect(outerDiv).toHaveClass('custom-height')
    })
  })

  describe('MessageList Integration', () => {
    it('should render MessageList component', () => {
      render(<ChatMessagesView />)
      
      const messageList = screen.getByTestId('message-list')
      expect(messageList).toBeInTheDocument()
    })

    it('should pass correct props to MessageList', () => {
      render(<ChatMessagesView />)
      
      const messageList = screen.getByTestId('message-list')
      expect(messageList).toHaveClass('px-4')
      expect(messageList).toHaveClass('py-4')
      expect(messageList).toHaveAttribute('data-max-height', 'none')
    })

    it('should render MessageList even with empty messages', () => {
      updateMockState('chat', {
        messages: [],
        isAgentTyping: false
      })
      
      render(<ChatMessagesView />)
      
      const messageList = screen.getByTestId('message-list')
      expect(messageList).toBeInTheDocument()
    })

    it('should render MessageList regardless of typing state', () => {
      updateMockState('chat', {
        ...defaultMockChatData,
        isAgentTyping: true
      })
      
      render(<ChatMessagesView />)
      
      const messageList = screen.getByTestId('message-list')
      expect(messageList).toBeInTheDocument()
    })
  })

  describe('TypingIndicator Conditional Rendering', () => {
    it('should pass typing state to MessageList', () => {
      updateMockState('chat', {
        ...defaultMockChatData,
        isAgentTyping: true
      })
      
      render(<ChatMessagesView />)
      
      // TypingIndicator is now handled within MessageList
      const messageList = screen.getByTestId('message-list')
      expect(messageList).toBeInTheDocument()
    })

    it('should render MessageList regardless of typing state', () => {
      updateMockState('chat', {
        ...defaultMockChatData,
        isAgentTyping: false
      })
      
      render(<ChatMessagesView />)
      
      // MessageList handles typing indicator internally
      const messageList = screen.getByTestId('message-list')
      expect(messageList).toBeInTheDocument()
    })

    it('should render MessageList with correct layout classes', () => {
      updateMockState('chat', {
        ...defaultMockChatData,
        isAgentTyping: true
      })
      
      render(<ChatMessagesView />)
      
      // MessageList has the appropriate layout classes
      const messageList = screen.getByTestId('message-list')
      
      expect(messageList).toHaveClass('max-w-4xl')
      expect(messageList).toHaveClass('mx-auto')
      expect(messageList).toHaveClass('px-4')
      expect(messageList).toHaveClass('py-4')
    })

    it('should handle rapid typing state changes', async () => {
      const { rerender } = render(<ChatMessagesView />)
      
      // MessageList is always present
      expect(screen.getByTestId('message-list')).toBeInTheDocument()
      
      // Start typing
      updateMockState('chat', {
        ...defaultMockChatData,
        isAgentTyping: true
      })
      rerender(<ChatMessagesView />)
      expect(screen.getByTestId('message-list')).toBeInTheDocument()
      
      // Stop typing
      updateMockState('chat', {
        ...defaultMockChatData,
        isAgentTyping: false
      })
      rerender(<ChatMessagesView />)
      expect(screen.getByTestId('message-list')).toBeInTheDocument()
      
      // Start typing again
      updateMockState('chat', {
        ...defaultMockChatData,
        isAgentTyping: true
      })
      rerender(<ChatMessagesView />)
      expect(screen.getByTestId('message-list')).toBeInTheDocument()
    })

    it('should maintain MessageList during message updates', () => {
      updateMockState('chat', {
        messages: defaultMockChatData.messages,
        isAgentTyping: true
      })
      
      const { rerender } = render(<ChatMessagesView />)
      expect(screen.getByTestId('message-list')).toBeInTheDocument()
      
      // Update messages while still typing
      updateMockState('chat', {
        messages: [
          ...defaultMockChatData.messages,
          {
            role: 'assistant' as const,
            content: 'New message',
            timestamp: '2024-01-01T12:00:02Z'
          }
        ],
        isAgentTyping: true
      })
      
      rerender(<ChatMessagesView />)
      expect(screen.getByTestId('message-list')).toBeInTheDocument()
    })
  })

  describe('ScrollAnchor Integration', () => {
    it('should render ScrollAnchor component', () => {
      render(<ChatMessagesView />)
      
      const scrollAnchor = screen.getByTestId('scroll-anchor')
      expect(scrollAnchor).toBeInTheDocument()
    })

    it('should pass scrollRef to ScrollAnchor', async () => {
      render(<ChatMessagesView />)
      
      // Wait for refs to be set up
      await waitFor(() => {
        const scrollAnchor = screen.getByTestId('scroll-anchor')
        // Initially might be false, but should become true once refs are set
        expect(scrollAnchor).toBeInTheDocument()
      })
    })

    it('should render ScrollAnchor at the bottom of scrollable area', () => {
      updateMockState('chat', {
        ...defaultMockChatData,
        isAgentTyping: true
      })
      
      const { container } = render(<ChatMessagesView />)
      
      const scrollableArea = container.querySelector('.h-full.overflow-y-auto')
      const lastChild = scrollableArea?.lastElementChild
      
      expect(lastChild).toHaveAttribute('data-testid', 'scroll-anchor')
    })

    it('should maintain ScrollAnchor position with MessageList', () => {
      updateMockState('chat', {
        ...defaultMockChatData,
        isAgentTyping: true
      })
      
      const { container } = render(<ChatMessagesView />)
      
      const scrollableArea = container.querySelector('.h-full.overflow-y-auto')
      const children = Array.from(scrollableArea?.children || [])
      
      // Order should be: MessageList, ScrollAnchor
      expect(children[0]).toHaveAttribute('data-testid', 'message-list')
      expect(children[1]).toHaveAttribute('data-testid', 'scroll-anchor')
      expect(children.length).toBe(2)
    })
  })

  describe('Ref Forwarding', () => {
    it('should forward ref to outer container', () => {
      const ref = React.createRef<HTMLDivElement>()
      render(<ChatMessagesView ref={ref} />)
      
      expect(ref.current).toBeInstanceOf(HTMLDivElement)
      expect(ref.current).toHaveClass('h-full')
      expect(ref.current).toHaveClass('overflow-hidden')
    })

    it('should allow external control through forwarded ref', () => {
      const ref = React.createRef<HTMLDivElement>()
      render(<ChatMessagesView ref={ref} />)
      
      // Test that we can access properties through the ref
      expect(ref.current?.className).toContain('h-full')
      expect(ref.current?.className).toContain('overflow-hidden')
    })

    it('should handle null ref gracefully', () => {
      // This should not throw
      expect(() => {
        render(<ChatMessagesView ref={null} />)
      }).not.toThrow()
    })

    it('should work with callback refs', () => {
      let refElement: HTMLDivElement | null = null
      const callbackRef = (element: HTMLDivElement | null) => {
        refElement = element
      }
      
      render(<ChatMessagesView ref={callbackRef} />)
      
      expect(refElement).toBeInstanceOf(HTMLDivElement)
      expect(refElement).toHaveClass('h-full')
    })
  })

  describe('Edge Cases and Error Handling', () => {
    it('should handle undefined messages gracefully', () => {
      updateMockState('chat', {
        messages: undefined,
        isAgentTyping: false
      })
      
      // Should not throw
      expect(() => {
        render(<ChatMessagesView />)
      }).not.toThrow()
    })

    it('should handle null typing state', () => {
      updateMockState('chat', {
        ...defaultMockChatData,
        isAgentTyping: null
      })
      
      render(<ChatMessagesView />)
      
      // Should treat null as false (no typing indicator)
      const typingIndicator = screen.queryByTestId('typing-indicator')
      expect(typingIndicator).not.toBeInTheDocument()
    })

    it('should handle undefined typing state', () => {
      updateMockState('chat', {
        ...defaultMockChatData,
        isAgentTyping: undefined
      })
      
      render(<ChatMessagesView />)
      
      // Should treat undefined as false (no typing indicator)
      const typingIndicator = screen.queryByTestId('typing-indicator')
      expect(typingIndicator).not.toBeInTheDocument()
    })

    it('should maintain component structure with empty className', () => {
      const { container } = render(<ChatMessagesView className="" />)
      
      const outerDiv = container.firstChild as HTMLElement
      expect(outerDiv).toHaveClass('h-full')
      expect(outerDiv).toHaveClass('overflow-hidden')
    })

    it('should handle className with extra spaces', () => {
      const { container } = render(<ChatMessagesView className="  custom-class  " />)
      
      const outerDiv = container.firstChild as HTMLElement
      expect(outerDiv).toHaveClass('custom-class')
    })
  })

  describe('Component Composition', () => {
    it('should render all child components in correct order', () => {
      updateMockState('chat', {
        ...defaultMockChatData,
        isAgentTyping: false
      })
      
      const { container } = render(<ChatMessagesView />)
      
      const scrollableArea = container.querySelector('.h-full.overflow-y-auto')
      const children = Array.from(scrollableArea?.children || [])
      
      // Without typing indicator: MessageList, ScrollAnchor
      expect(children).toHaveLength(2)
      expect(children[0]).toHaveAttribute('data-testid', 'message-list')
      expect(children[1]).toHaveAttribute('data-testid', 'scroll-anchor')
    })

    it('should integrate all components properly', () => {
      updateMockState('chat', {
        ...defaultMockChatData,
        isAgentTyping: true
      })
      
      const { container } = render(<ChatMessagesView />)
      
      const scrollableArea = container.querySelector('.h-full.overflow-y-auto')
      const children = Array.from(scrollableArea?.children || [])
      
      // Components: MessageList and ScrollAnchor (typing indicator is inside MessageList)
      expect(children).toHaveLength(2)
      expect(children[0]).toHaveAttribute('data-testid', 'message-list')
      expect(children[1]).toHaveAttribute('data-testid', 'scroll-anchor')
    })

    it('should maintain proper nesting structure', () => {
      const { container } = render(<ChatMessagesView />)
      
      // Verify nesting: outer > scrollable > children
      const outerDiv = container.firstChild as HTMLElement
      const scrollableArea = outerDiv.querySelector('.h-full.overflow-y-auto')
      const messageList = scrollableArea?.querySelector('[data-testid="message-list"]')
      const scrollAnchor = scrollableArea?.querySelector('[data-testid="scroll-anchor"]')
      
      expect(outerDiv).toBeInTheDocument()
      expect(scrollableArea).toBeInTheDocument()
      expect(messageList).toBeInTheDocument()
      expect(scrollAnchor).toBeInTheDocument()
    })
  })

  describe('Display Name', () => {
    it('should have correct displayName for debugging', () => {
      expect(ChatMessagesView.displayName).toBe('ChatMessagesView')
    })
  })
})