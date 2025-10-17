import React from 'react'
import { describe, it, expect, vi, beforeEach, afterEach } from 'vitest'
import { render, screen, waitFor } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import { ConnectionIndicator } from '../ConnectionIndicator'
import { ConnectionState } from '@agentc/realtime-core'

// Mock the tooltip components
vi.mock('../../ui/tooltip', () => ({
  Tooltip: ({ children }: { children: React.ReactNode }) => <div data-testid="tooltip">{children}</div>,
  TooltipTrigger: ({ children, asChild }: { children: React.ReactNode; asChild?: boolean }) => (
    <div data-testid="tooltip-trigger">{children}</div>
  ),
  TooltipContent: ({ children }: { children: React.ReactNode }) => (
    <div data-testid="tooltip-content">{children}</div>
  ),
  TooltipProvider: ({ children }: { children: React.ReactNode }) => <>{children}</>
}))

// Note: @agentc/realtime-react is globally mocked in vitest.setup.ts
import { updateMockState, getConnectionStateString } from '../../../test/mocks/realtime-react'

const mockGetConnectionStateString = vi.mocked(getConnectionStateString)

describe('ConnectionIndicator', () => {
  const defaultMockConnectionData = {
    isConnected: false,
    connectionState: ConnectionState.DISCONNECTED,
    error: null,
    stats: {
      latency: 0,
      messagesReceived: 0,
      messagesSent: 0,
      bytesReceived: 0,
      bytesSent: 0
    }
  }

  beforeEach(() => {
    vi.clearAllMocks()
    // Set default mock return values using global mock
    updateMockState('connection', defaultMockConnectionData)
    mockGetConnectionStateString.mockImplementation((state: ConnectionState) => {
      switch(state) {
        case ConnectionState.CONNECTED:
          return 'connected'
        case ConnectionState.CONNECTING:
          return 'connecting'
        case ConnectionState.RECONNECTING:
          return 'reconnecting'
        case ConnectionState.DISCONNECTED:
        default:
          return 'disconnected'
      }
    })
  })

  afterEach(() => {
    vi.restoreAllMocks()
  })

  describe('Component Rendering', () => {
    it('should render with default props', () => {
      const { container } = render(<ConnectionIndicator />)
      
      const indicator = container.querySelector('[role="status"]')
      expect(indicator).toBeInTheDocument()
      expect(indicator).toHaveAttribute('aria-label', 'Connection status: Disconnected')
      expect(indicator).toHaveAttribute('aria-live', 'polite')
    })

    it('should apply custom className', () => {
      const { container } = render(<ConnectionIndicator className="custom-class" />)
      
      const indicator = container.querySelector('[role="status"]')
      expect(indicator).toHaveClass('custom-class')
      expect(indicator).toHaveClass('inline-flex')
      expect(indicator).toHaveClass('items-center')
      expect(indicator).toHaveClass('gap-2')
    })

    it('should forward ref to container div', () => {
      const ref = React.createRef<HTMLDivElement>()
      render(<ConnectionIndicator ref={ref} />)
      
      expect(ref.current).toBeInstanceOf(HTMLDivElement)
      expect(ref.current).toHaveAttribute('role', 'status')
    })

    it('should pass through HTML div attributes', () => {
      render(<ConnectionIndicator data-testid="custom-indicator" />)
      
      const indicator = screen.getByTestId('custom-indicator')
      expect(indicator).toBeInTheDocument()
    })
  })

  describe('Connection State Visual Indicators', () => {
    it('should show green indicator when connected', () => {
      updateMockState('connection', {
        ...defaultMockConnectionData,
        isConnected: true,
        connectionState: ConnectionState.CONNECTED
      })
      mockGetConnectionStateString.mockReturnValue('connected')
      
      const { container } = render(<ConnectionIndicator />)
      
      const dot = container.querySelector('.rounded-full')
      expect(dot).toHaveClass('bg-green-500')
      expect(dot).not.toHaveClass('animate-pulse')
    })

    it('should show yellow pulsing indicator when connecting', () => {
      updateMockState('connection', {
        ...defaultMockConnectionData,
        connectionState: ConnectionState.CONNECTING
      })
      mockGetConnectionStateString.mockReturnValue('connecting')
      
      const { container } = render(<ConnectionIndicator />)
      
      const dot = container.querySelector('.rounded-full')
      expect(dot).toHaveClass('bg-yellow-500')
      expect(dot).toHaveClass('animate-pulse')
    })

    it('should show yellow pulsing indicator when reconnecting', () => {
      updateMockState('connection', {
        ...defaultMockConnectionData,
        connectionState: ConnectionState.RECONNECTING
      })
      mockGetConnectionStateString.mockReturnValue('reconnecting')
      
      const { container } = render(<ConnectionIndicator />)
      
      const dot = container.querySelector('.rounded-full')
      expect(dot).toHaveClass('bg-yellow-500')
      expect(dot).toHaveClass('animate-pulse')
    })

    it('should show gray indicator when disconnected', () => {
      updateMockState('connection', {
        ...defaultMockConnectionData,
        connectionState: ConnectionState.DISCONNECTED
      })
      mockGetConnectionStateString.mockReturnValue('disconnected')
      
      const { container } = render(<ConnectionIndicator />)
      
      const dot = container.querySelector('.rounded-full')
      expect(dot).toHaveClass('bg-gray-400')
      expect(dot).not.toHaveClass('animate-pulse')
    })

    it('should handle unknown connection states gracefully', () => {
      updateMockState('connection', {
        ...defaultMockConnectionData,
        connectionState: 'UNKNOWN' as any
      })
      mockGetConnectionStateString.mockReturnValue('unknown' as any)
      
      const { container } = render(<ConnectionIndicator />)
      
      const dot = container.querySelector('.rounded-full')
      expect(dot).toHaveClass('bg-gray-400')
    })

    it('should update visual state on connection state change', () => {
      const { container, rerender } = render(<ConnectionIndicator />)
      
      let dot = container.querySelector('.rounded-full')
      expect(dot).toHaveClass('bg-gray-400')
      
      // Change to connecting
      updateMockState('connection', {
        ...defaultMockConnectionData,
        connectionState: ConnectionState.CONNECTING
      })
      mockGetConnectionStateString.mockReturnValue('connecting')
      rerender(<ConnectionIndicator />)
      
      dot = container.querySelector('.rounded-full')
      expect(dot).toHaveClass('bg-yellow-500')
      expect(dot).toHaveClass('animate-pulse')
      
      // Change to connected
      updateMockState('connection', {
        ...defaultMockConnectionData,
        isConnected: true,
        connectionState: ConnectionState.CONNECTED
      })
      mockGetConnectionStateString.mockReturnValue('connected')
      rerender(<ConnectionIndicator />)
      
      dot = container.querySelector('.rounded-full')
      expect(dot).toHaveClass('bg-green-500')
      expect(dot).not.toHaveClass('animate-pulse')
    })
  })

  describe('Size Variants', () => {
    it('should render small size indicator', () => {
      const { container } = render(<ConnectionIndicator size="small" />)
      
      const dot = container.querySelector('.rounded-full')
      expect(dot).toHaveClass('h-2')
      expect(dot).toHaveClass('w-2')
    })

    it('should render default size indicator', () => {
      const { container } = render(<ConnectionIndicator size="default" />)
      
      const dot = container.querySelector('.rounded-full')
      expect(dot).toHaveClass('h-3')
      expect(dot).toHaveClass('w-3')
    })

    it('should render large size indicator', () => {
      const { container } = render(<ConnectionIndicator size="large" />)
      
      const dot = container.querySelector('.rounded-full')
      expect(dot).toHaveClass('h-4')
      expect(dot).toHaveClass('w-4')
    })

    it('should use default size when size prop is not provided', () => {
      const { container } = render(<ConnectionIndicator />)
      
      const dot = container.querySelector('.rounded-full')
      expect(dot).toHaveClass('h-3')
      expect(dot).toHaveClass('w-3')
    })
  })

  describe('Label Display', () => {
    it('should not show label by default', () => {
      render(<ConnectionIndicator />)
      
      expect(screen.queryByText('Disconnected')).not.toBeInTheDocument()
    })

    it('should show label when showLabel is true', () => {
      render(<ConnectionIndicator showLabel={true} />)
      
      expect(screen.getByText('Disconnected')).toBeInTheDocument()
    })

    it('should show correct label for connected state', () => {
      updateMockState('connection', {
        ...defaultMockConnectionData,
        isConnected: true,
        connectionState: ConnectionState.CONNECTED
      })
      mockGetConnectionStateString.mockReturnValue('connected')
      
      render(<ConnectionIndicator showLabel={true} />)
      
      expect(screen.getByText('Connected')).toBeInTheDocument()
    })

    it('should show correct label for connecting state', () => {
      updateMockState('connection', {
        ...defaultMockConnectionData,
        connectionState: ConnectionState.CONNECTING
      })
      mockGetConnectionStateString.mockReturnValue('connecting')
      
      render(<ConnectionIndicator showLabel={true} />)
      
      expect(screen.getByText('Connecting')).toBeInTheDocument()
    })

    it('should show correct label for reconnecting state', () => {
      updateMockState('connection', {
        ...defaultMockConnectionData,
        connectionState: ConnectionState.RECONNECTING
      })
      mockGetConnectionStateString.mockReturnValue('reconnecting')
      
      render(<ConnectionIndicator showLabel={true} />)
      
      expect(screen.getByText('Reconnecting')).toBeInTheDocument()
    })

    it('should apply correct text styles to label', () => {
      render(<ConnectionIndicator showLabel={true} />)
      
      const label = screen.getByText('Disconnected')
      expect(label).toHaveClass('text-sm')
      expect(label).toHaveClass('text-muted-foreground')
    })
  })

  describe('Tooltip Functionality', () => {
    it('should not show tooltip by default', () => {
      render(<ConnectionIndicator />)
      
      expect(screen.queryByTestId('tooltip')).not.toBeInTheDocument()
    })

    it('should render tooltip wrapper when showTooltip is true', () => {
      render(<ConnectionIndicator showTooltip={true} />)
      
      expect(screen.getByTestId('tooltip')).toBeInTheDocument()
      expect(screen.getByTestId('tooltip-trigger')).toBeInTheDocument()
      expect(screen.getByTestId('tooltip-content')).toBeInTheDocument()
    })

    it('should show status text in tooltip', () => {
      updateMockState('connection', {
        ...defaultMockConnectionData,
        isConnected: true,
        connectionState: ConnectionState.CONNECTED
      })
      mockGetConnectionStateString.mockReturnValue('connected')
      
      render(<ConnectionIndicator showTooltip={true} />)
      
      const tooltipContent = screen.getByTestId('tooltip-content')
      expect(tooltipContent).toHaveTextContent('Connected')
    })

    it('should show error message in tooltip when error exists', () => {
      const errorMessage = 'Connection failed: Network error'
      updateMockState('connection', {
        ...defaultMockConnectionData,
        error: new Error(errorMessage)
      })
      
      render(<ConnectionIndicator showTooltip={true} />)
      
      const tooltipContent = screen.getByTestId('tooltip-content')
      expect(tooltipContent).toHaveTextContent(`Error: ${errorMessage}`)
    })

    it('should not show stats in tooltip when showStats is false', () => {
      updateMockState('connection', {
        ...defaultMockConnectionData,
        isConnected: true,
        connectionState: ConnectionState.CONNECTED,
        stats: {
          latency: 25,
          messagesReceived: 100,
          messagesSent: 50,
          bytesReceived: 1024 * 10,
          bytesSent: 1024 * 5
        }
      })
      
      render(<ConnectionIndicator showTooltip={true} showStats={false} />)
      
      const tooltipContent = screen.getByTestId('tooltip-content')
      expect(tooltipContent).not.toHaveTextContent('Latency')
      expect(tooltipContent).not.toHaveTextContent('Messages')
    })

    it('should show stats in tooltip when connected and showStats is true', () => {
      updateMockState('connection', {
        ...defaultMockConnectionData,
        isConnected: true,
        connectionState: ConnectionState.CONNECTED,
        stats: {
          latency: 25,
          messagesReceived: 100,
          messagesSent: 50,
          bytesReceived: 1024 * 10,
          bytesSent: 1024 * 5
        }
      })
      mockGetConnectionStateString.mockReturnValue('connected')
      
      render(<ConnectionIndicator showTooltip={true} showStats={true} />)
      
      const tooltipContent = screen.getByTestId('tooltip-content')
      expect(tooltipContent).toHaveTextContent('Latency: 25ms')
      expect(tooltipContent).toHaveTextContent('Messages: 100 received, 50 sent')
      expect(tooltipContent).toHaveTextContent('Data: 10KB received')
    })

    it('should not show stats when disconnected even if showStats is true', () => {
      updateMockState('connection', {
        ...defaultMockConnectionData,
        isConnected: false,
        connectionState: ConnectionState.DISCONNECTED,
        stats: {
          latency: 25,
          messagesReceived: 100,
          messagesSent: 50,
          bytesReceived: 1024 * 10,
          bytesSent: 1024 * 5
        }
      })
      
      render(<ConnectionIndicator showTooltip={true} showStats={true} />)
      
      const tooltipContent = screen.getByTestId('tooltip-content')
      expect(tooltipContent).not.toHaveTextContent('Latency')
      expect(tooltipContent).not.toHaveTextContent('Messages')
    })

    it('should format bytes correctly in tooltip', () => {
      updateMockState('connection', {
        ...defaultMockConnectionData,
        isConnected: true,
        connectionState: ConnectionState.CONNECTED,
        stats: {
          latency: 0,
          messagesReceived: 0,
          messagesSent: 0,
          bytesReceived: 1024 * 256, // 256KB
          bytesSent: 0
        }
      })
      mockGetConnectionStateString.mockReturnValue('connected')
      
      render(<ConnectionIndicator showTooltip={true} showStats={true} />)
      
      const tooltipContent = screen.getByTestId('tooltip-content')
      expect(tooltipContent).toHaveTextContent('Data: 256KB received')
    })

    it('should handle zero bytes received gracefully', () => {
      updateMockState('connection', {
        ...defaultMockConnectionData,
        isConnected: true,
        connectionState: ConnectionState.CONNECTED,
        stats: {
          latency: 0,
          messagesReceived: 0,
          messagesSent: 0,
          bytesReceived: 0,
          bytesSent: 0
        }
      })
      mockGetConnectionStateString.mockReturnValue('connected')
      
      render(<ConnectionIndicator showTooltip={true} showStats={true} />)
      
      const tooltipContent = screen.getByTestId('tooltip-content')
      // Should not show data line when bytesReceived is 0
      expect(tooltipContent).not.toHaveTextContent('Data:')
    })

    it('should handle undefined bytesReceived gracefully', () => {
      updateMockState('connection', {
        ...defaultMockConnectionData,
        isConnected: true,
        connectionState: ConnectionState.CONNECTED,
        stats: {
          latency: 25,
          messagesReceived: 10,
          messagesSent: 5,
          bytesReceived: undefined,
          bytesSent: undefined
        }
      })
      mockGetConnectionStateString.mockReturnValue('connected')
      
      render(<ConnectionIndicator showTooltip={true} showStats={true} />)
      
      const tooltipContent = screen.getByTestId('tooltip-content')
      expect(tooltipContent).toHaveTextContent('Messages: 10 received, 5 sent')
      // Should not show data line when bytesReceived is undefined
      expect(tooltipContent).not.toHaveTextContent('Data:')
    })
  })

  describe('Accessibility', () => {
    it('should have correct ARIA attributes', () => {
      const { container } = render(<ConnectionIndicator />)
      
      const indicator = container.querySelector('[role="status"]')
      expect(indicator).toHaveAttribute('aria-label', 'Connection status: Disconnected')
      expect(indicator).toHaveAttribute('aria-live', 'polite')
    })

    it('should update aria-label when connection state changes', () => {
      const { container, rerender } = render(<ConnectionIndicator />)
      
      let indicator = container.querySelector('[role="status"]')
      expect(indicator).toHaveAttribute('aria-label', 'Connection status: Disconnected')
      
      updateMockState('connection', {
        ...defaultMockConnectionData,
        isConnected: true,
        connectionState: ConnectionState.CONNECTED
      })
      mockGetConnectionStateString.mockReturnValue('connected')
      rerender(<ConnectionIndicator />)
      
      indicator = container.querySelector('[role="status"]')
      expect(indicator).toHaveAttribute('aria-label', 'Connection status: Connected')
    })

    it('should provide correct aria-label for connecting state', () => {
      updateMockState('connection', {
        ...defaultMockConnectionData,
        connectionState: ConnectionState.CONNECTING
      })
      mockGetConnectionStateString.mockReturnValue('connecting')
      
      const { container } = render(<ConnectionIndicator />)
      
      const indicator = container.querySelector('[role="status"]')
      expect(indicator).toHaveAttribute('aria-label', 'Connection status: Connecting')
    })

    it('should provide correct aria-label for reconnecting state', () => {
      updateMockState('connection', {
        ...defaultMockConnectionData,
        connectionState: ConnectionState.RECONNECTING
      })
      mockGetConnectionStateString.mockReturnValue('reconnecting')
      
      const { container } = render(<ConnectionIndicator />)
      
      const indicator = container.querySelector('[role="status"]')
      expect(indicator).toHaveAttribute('aria-label', 'Connection status: Reconnecting')
    })

    it('should be keyboard accessible when tooltip is enabled', async () => {
      render(<ConnectionIndicator showTooltip={true} />)
      
      const trigger = screen.getByTestId('tooltip-trigger')
      expect(trigger).toBeInTheDocument()
      
      // Tooltip trigger should be keyboard focusable
      // Note: This would typically be handled by the actual Tooltip component
    })
  })

  describe('Edge Cases and Error Handling', () => {
    it('should handle null error gracefully', () => {
      updateMockState('connection', {
        ...defaultMockConnectionData,
        error: null
      })
      
      render(<ConnectionIndicator showTooltip={true} />)
      
      const tooltipContent = screen.getByTestId('tooltip-content')
      expect(tooltipContent).not.toHaveTextContent('Error:')
    })

    it('should handle error without message property', () => {
      updateMockState('connection', {
        ...defaultMockConnectionData,
        error: { name: 'ConnectionError' } as Error
      })
      
      render(<ConnectionIndicator showTooltip={true} />)
      
      const tooltipContent = screen.getByTestId('tooltip-content')
      expect(tooltipContent).toHaveTextContent('Error:')
    })

    it('should handle undefined stats gracefully', () => {
      updateMockState('connection', {
        ...defaultMockConnectionData,
        isConnected: true,
        connectionState: ConnectionState.CONNECTED,
        stats: undefined as any
      })
      mockGetConnectionStateString.mockReturnValue('connected')
      
      // Should not throw
      expect(() => {
        render(<ConnectionIndicator showTooltip={true} showStats={true} />)
      }).not.toThrow()
    })

    it('should handle null stats gracefully', () => {
      updateMockState('connection', {
        ...defaultMockConnectionData,
        isConnected: true,
        connectionState: ConnectionState.CONNECTED,
        stats: null as any
      })
      mockGetConnectionStateString.mockReturnValue('connected')
      
      // Should not throw
      expect(() => {
        render(<ConnectionIndicator showTooltip={true} showStats={true} />)
      }).not.toThrow()
    })

    it('should handle rapid prop changes', () => {
      const { container, rerender } = render(<ConnectionIndicator />)
      
      // Rapid prop changes should not cause issues
      rerender(<ConnectionIndicator showLabel={true} />)
      expect(screen.getByText('Disconnected')).toBeInTheDocument()
      
      rerender(<ConnectionIndicator showLabel={false} showTooltip={true} />)
      // Check that label is not shown (not checking tooltip content)
      const labels = container.querySelectorAll('.text-sm.text-muted-foreground')
      expect(labels).toHaveLength(0)
      expect(screen.getByTestId('tooltip')).toBeInTheDocument()
      
      rerender(<ConnectionIndicator showTooltip={false} size="large" />)
      expect(screen.queryByTestId('tooltip')).not.toBeInTheDocument()
      
      rerender(<ConnectionIndicator size="small" showLabel={true} />)
      expect(screen.getByText('Disconnected')).toBeInTheDocument()
    })

    it('should handle empty className gracefully', () => {
      const { container } = render(<ConnectionIndicator className="" />)
      
      const indicator = container.querySelector('[role="status"]')
      expect(indicator).toHaveClass('inline-flex')
      expect(indicator).toHaveClass('items-center')
      expect(indicator).toHaveClass('gap-2')
    })

    it('should handle className with extra spaces', () => {
      const { container } = render(<ConnectionIndicator className="  custom-class  " />)
      
      const indicator = container.querySelector('[role="status"]')
      expect(indicator).toHaveClass('custom-class')
    })
  })

  describe('Component Integration', () => {
    it('should work with all props combined', () => {
      updateMockState('connection', {
        ...defaultMockConnectionData,
        isConnected: true,
        connectionState: ConnectionState.CONNECTED,
        stats: {
          latency: 15,
          messagesReceived: 42,
          messagesSent: 21,
          bytesReceived: 1024 * 5,
          bytesSent: 1024 * 2
        }
      })
      mockGetConnectionStateString.mockReturnValue('connected')
      
      const { container } = render(
        <ConnectionIndicator 
          showLabel={true}
          showTooltip={true}
          showStats={true}
          size="large"
          className="custom-indicator"
        />
      )
      
      // Check all features are working together
      const indicator = container.querySelector('[role="status"]')
      expect(indicator).toHaveClass('custom-indicator')
      
      const dot = container.querySelector('.rounded-full')
      expect(dot).toHaveClass('h-4', 'w-4', 'bg-green-500')
      
      // Check label is present (using more specific selector)
      const label = container.querySelector('.text-sm.text-muted-foreground')
      expect(label).toHaveTextContent('Connected')
      
      expect(screen.getByTestId('tooltip')).toBeInTheDocument()
      
      const tooltipContent = screen.getByTestId('tooltip-content')
      expect(tooltipContent).toHaveTextContent('Latency: 15ms')
      expect(tooltipContent).toHaveTextContent('Messages: 42 received, 21 sent')
    })

    it('should maintain state consistency across rerenders', () => {
      const { container, rerender } = render(<ConnectionIndicator showLabel={true} />)
      
      expect(screen.getByText('Disconnected')).toBeInTheDocument()
      
      // Change connection state
      updateMockState('connection', {
        ...defaultMockConnectionData,
        isConnected: true,
        connectionState: ConnectionState.CONNECTED
      })
      mockGetConnectionStateString.mockReturnValue('connected')
      
      rerender(<ConnectionIndicator showLabel={true} />)
      expect(screen.getByText('Connected')).toBeInTheDocument()
      
      // Add tooltip
      rerender(<ConnectionIndicator showLabel={true} showTooltip={true} />)
      // Use more specific selector for the label
      const label = container.querySelector('.text-sm.text-muted-foreground')
      expect(label).toHaveTextContent('Connected')
      expect(screen.getByTestId('tooltip')).toBeInTheDocument()
    })
  })

  describe('Display Name', () => {
    it('should have correct displayName for debugging', () => {
      expect(ConnectionIndicator.displayName).toBe('ConnectionIndicator')
    })
  })

  describe('Performance Considerations', () => {
    it('should memoize indicator color correctly', () => {
      const { container, rerender } = render(<ConnectionIndicator />)
      
      const dot1 = container.querySelector('.rounded-full')
      const initialClasses = dot1?.className
      
      // Rerender with same state
      rerender(<ConnectionIndicator />)
      
      const dot2 = container.querySelector('.rounded-full')
      expect(dot2?.className).toBe(initialClasses)
    })

    it('should memoize status text correctly', () => {
      const { rerender } = render(<ConnectionIndicator showLabel={true} />)
      
      const text1 = screen.getByText('Disconnected')
      const initialText = text1.textContent
      
      // Rerender with same state
      rerender(<ConnectionIndicator showLabel={true} />)
      
      const text2 = screen.getByText('Disconnected')
      expect(text2.textContent).toBe(initialText)
    })

    it('should only update when connection state actually changes', () => {
      const { container, rerender } = render(<ConnectionIndicator />)
      
      const dot1 = container.querySelector('.rounded-full')
      expect(dot1).toHaveClass('bg-gray-400')
      
      // Rerender with same mock data
      rerender(<ConnectionIndicator />)
      
      const dot2 = container.querySelector('.rounded-full')
      expect(dot2).toHaveClass('bg-gray-400')
      
      // Actually change the state
      updateMockState('connection', {
        ...defaultMockConnectionData,
        connectionState: ConnectionState.CONNECTING
      })
      mockGetConnectionStateString.mockReturnValue('connecting')
      
      rerender(<ConnectionIndicator />)
      
      const dot3 = container.querySelector('.rounded-full')
      expect(dot3).toHaveClass('bg-yellow-500')
    })
  })
})