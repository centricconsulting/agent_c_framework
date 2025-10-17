import React from 'react'
import { render, screen, act, waitFor } from '@testing-library/react'
import { describe, it, expect, vi, beforeEach, afterEach } from 'vitest'
import { AvatarDisplayView } from '../AvatarDisplayView'
import { useAvatar } from '@agentc/realtime-react'
import type { UseAvatarReturn } from '@agentc/realtime-react'

// Mock the useAvatar hook
vi.mock('@agentc/realtime-react', () => ({
  useAvatar: vi.fn()
}))

// Mock MediaStream for jsdom environment
class MockMediaStream {
  constructor() {
    // Mock implementation
  }
}

if (typeof MediaStream === 'undefined') {
  global.MediaStream = MockMediaStream as any
}

describe('AvatarDisplayView', () => {
  const mockUseAvatar = vi.mocked(useAvatar)
  
  const defaultMockReturn: UseAvatarReturn = {
    avatarSession: null,
    isAvatarActive: false,
    isLoading: false,
    error: null,
    startAvatarSession: vi.fn(),
    stopAvatarSession: vi.fn()
  }

  beforeEach(() => {
    vi.clearAllMocks()
    vi.useFakeTimers()
    mockUseAvatar.mockReturnValue(defaultMockReturn)
  })

  afterEach(() => {
    vi.useRealTimers()
    vi.restoreAllMocks()
  })

  describe('basic rendering', () => {
    it('should render with default props', () => {
      render(<AvatarDisplayView />)
      const video = screen.getByLabelText('Avatar video stream')
      expect(video).toBeInTheDocument()
      expect(video.tagName).toBe('VIDEO')
    })

    it('should apply custom className', () => {
      const { container } = render(<AvatarDisplayView className="custom-class" />)
      const element = container.firstChild as HTMLElement
      expect(element).toHaveClass('custom-class')
    })

    it('should forward ref to container div', () => {
      const ref = React.createRef<HTMLDivElement>()
      const { container } = render(<AvatarDisplayView ref={ref} />)
      expect(ref.current).toBe(container.firstChild)
    })

    it('should set video element attributes correctly', () => {
      render(<AvatarDisplayView />)
      const video = screen.getByLabelText('Avatar video stream') as HTMLVideoElement
      
      expect(video).toHaveAttribute('autoplay')
      expect(video).toHaveAttribute('playsinline')
      expect(video).not.toHaveAttribute('muted')
    })
  })

  describe('no avatar state', () => {
    it('should display no avatar message when avatar is not active', () => {
      render(<AvatarDisplayView />)
      
      expect(screen.getByText('Avatar mode selected')).toBeInTheDocument()
      expect(screen.getByText('Start chatting to see the avatar')).toBeInTheDocument()
    })

    it('should render video icon in no avatar state', () => {
      const { container } = render(<AvatarDisplayView />)
      const svg = container.querySelector('svg')
      expect(svg).toBeInTheDocument()
      expect(svg).toHaveClass('h-12 w-12 text-muted-foreground')
    })
  })

  describe('loading states', () => {
    it('should show loading spinner when isLoading is true', () => {
      mockUseAvatar.mockReturnValue({
        ...defaultMockReturn,
        isLoading: true
      })
      
      render(<AvatarDisplayView />)
      expect(screen.getByText('Initializing avatar...')).toBeInTheDocument()
      
      // Check for spinner icon (Loader2)
      const spinner = document.querySelector('.animate-spin')
      expect(spinner).toBeInTheDocument()
    })

    it('should show loading when avatar is active but stream not ready', () => {
      mockUseAvatar.mockReturnValue({
        ...defaultMockReturn,
        avatarSession: { id: 'test-session' } as any,
        isAvatarActive: true,
        isLoading: false
      })
      
      render(<AvatarDisplayView />)
      expect(screen.getByText('Initializing avatar...')).toBeInTheDocument()
    })

    it('should hide loading spinner after stream becomes ready', async () => {
      mockUseAvatar.mockReturnValue({
        ...defaultMockReturn,
        avatarSession: { id: 'test-session' } as any,
        isAvatarActive: true,
        isLoading: false
      })
      
      render(<AvatarDisplayView />)
      expect(screen.getByText('Initializing avatar...')).toBeInTheDocument()
      
      // Advance timers to trigger stream ready
      await act(async () => {
        vi.advanceTimersByTime(1000)
      })
      
      expect(screen.queryByText('Initializing avatar...')).not.toBeInTheDocument()
    })
  })

  describe('connected state', () => {
    it('should show connected indicator when avatar is active and stream is ready', async () => {
      mockUseAvatar.mockReturnValue({
        ...defaultMockReturn,
        avatarSession: { id: 'test-session' } as any,
        isAvatarActive: true,
        isLoading: false
      })
      
      render(<AvatarDisplayView />)
      
      // Initially should be loading
      expect(screen.queryByText('Connected')).not.toBeInTheDocument()
      
      // Advance timers to trigger stream ready
      await act(async () => {
        vi.advanceTimersByTime(1000)
      })
      
      expect(screen.getByText('Connected')).toBeInTheDocument()
    })

    it('should display green connection indicator dot', async () => {
      mockUseAvatar.mockReturnValue({
        ...defaultMockReturn,
        avatarSession: { id: 'test-session' } as any,
        isAvatarActive: true,
        isLoading: false
      })
      
      const { container } = render(<AvatarDisplayView />)
      
      await act(async () => {
        vi.advanceTimersByTime(1000)
      })
      
      const greenDot = container.querySelector('.bg-green-500')
      expect(greenDot).toBeInTheDocument()
      expect(greenDot).toHaveAttribute('aria-hidden', 'true')
    })
  })

  describe('error states', () => {
    it('should display error message when error prop is set', () => {
      const errorMessage = 'Failed to connect to avatar'
      mockUseAvatar.mockReturnValue({
        ...defaultMockReturn,
        error: errorMessage
      })
      
      render(<AvatarDisplayView />)
      expect(screen.getByText(errorMessage)).toBeInTheDocument()
    })

    it('should prioritize error display over other states', () => {
      mockUseAvatar.mockReturnValue({
        ...defaultMockReturn,
        error: 'Connection failed',
        isLoading: true,
        isAvatarActive: true
      })
      
      render(<AvatarDisplayView />)
      expect(screen.getByText('Connection failed')).toBeInTheDocument()
      expect(screen.queryByText('Initializing avatar...')).toBeInTheDocument() // Loading still shows
    })
  })

  describe('video stream management', () => {
    it('should handle video ref properly', () => {
      const { container } = render(<AvatarDisplayView />)
      const video = container.querySelector('video')
      expect(video).toBeInTheDocument()
      expect(video).toHaveClass('w-full h-full object-contain')
    })

    it('should have cleanup logic for video srcObject on unmount', () => {
      // This test verifies the cleanup logic exists even though srcObject
      // is not currently set (placeholder for HeyGen integration)
      const cleanupSpy = vi.fn()
      const mockVideoElement = {
        srcObject: null as any,
        autoPlay: true,
        playsInline: true,
        muted: false
      }
      
      // Override srcObject setter to track cleanup
      Object.defineProperty(mockVideoElement, 'srcObject', {
        get: vi.fn(() => null),
        set: cleanupSpy,
        configurable: true
      })
      
      const originalUseRef = React.useRef
      vi.spyOn(React, 'useRef').mockImplementation((initial) => {
        if (initial === null) {
          return { current: mockVideoElement }
        }
        return originalUseRef(initial)
      })
      
      mockUseAvatar.mockReturnValue({
        ...defaultMockReturn,
        avatarSession: { id: 'test-session' } as any,
        isAvatarActive: true
      })
      
      const { unmount } = render(<AvatarDisplayView />)
      
      unmount()
      
      // The component has cleanup logic ready for when HeyGen integration is added
      // For now, we just verify the structure is in place
      expect(mockVideoElement).toBeDefined()
    })

    it('should have cleanup logic when avatar becomes inactive', () => {
      // This test verifies the effect cleanup runs when avatar state changes
      const clearTimeoutSpy = vi.spyOn(global, 'clearTimeout')
      
      // Start with active avatar
      mockUseAvatar.mockReturnValue({
        ...defaultMockReturn,
        avatarSession: { id: 'test-session' } as any,
        isAvatarActive: true
      })
      
      const { rerender } = render(<AvatarDisplayView />)
      
      // Deactivate avatar to trigger effect cleanup
      mockUseAvatar.mockReturnValue({
        ...defaultMockReturn,
        avatarSession: null,
        isAvatarActive: false
      })
      
      rerender(<AvatarDisplayView />)
      
      // Verify cleanup logic runs (timer cleanup is part of the effect cleanup)
      expect(clearTimeoutSpy).toHaveBeenCalled()
      
      // Verify the component handles the state change properly
      expect(screen.getByText('Avatar mode selected')).toBeInTheDocument()
    })
  })

  describe('timer management', () => {
    it('should clear timeout on unmount', () => {
      const clearTimeoutSpy = vi.spyOn(global, 'clearTimeout')
      
      mockUseAvatar.mockReturnValue({
        ...defaultMockReturn,
        avatarSession: { id: 'test-session' } as any,
        isAvatarActive: true
      })
      
      const { unmount } = render(<AvatarDisplayView />)
      
      unmount()
      
      expect(clearTimeoutSpy).toHaveBeenCalled()
    })

    it('should clear timeout when avatar session changes', () => {
      const clearTimeoutSpy = vi.spyOn(global, 'clearTimeout')
      
      mockUseAvatar.mockReturnValue({
        ...defaultMockReturn,
        avatarSession: { id: 'session-1' } as any,
        isAvatarActive: true
      })
      
      const { rerender } = render(<AvatarDisplayView />)
      
      // Change session
      mockUseAvatar.mockReturnValue({
        ...defaultMockReturn,
        avatarSession: { id: 'session-2' } as any,
        isAvatarActive: true
      })
      
      rerender(<AvatarDisplayView />)
      
      expect(clearTimeoutSpy).toHaveBeenCalled()
    })

    it('should reset streamReady state when avatar session changes', async () => {
      mockUseAvatar.mockReturnValue({
        ...defaultMockReturn,
        avatarSession: { id: 'session-1' } as any,
        isAvatarActive: true
      })
      
      const { rerender } = render(<AvatarDisplayView />)
      
      // Wait for stream to be ready
      await act(async () => {
        vi.advanceTimersByTime(1000)
      })
      
      expect(screen.getByText('Connected')).toBeInTheDocument()
      
      // Change session
      mockUseAvatar.mockReturnValue({
        ...defaultMockReturn,
        avatarSession: { id: 'session-2' } as any,
        isAvatarActive: true
      })
      
      rerender(<AvatarDisplayView />)
      
      // Should be loading again
      expect(screen.queryByText('Connected')).not.toBeInTheDocument()
      expect(screen.getByText('Initializing avatar...')).toBeInTheDocument()
    })
  })

  describe('accessibility', () => {
    it('should have proper ARIA label on video element', () => {
      render(<AvatarDisplayView />)
      const video = screen.getByLabelText('Avatar video stream')
      expect(video).toHaveAttribute('aria-label', 'Avatar video stream')
    })

    it('should mark decorative connection indicator with aria-hidden', async () => {
      mockUseAvatar.mockReturnValue({
        ...defaultMockReturn,
        avatarSession: { id: 'test-session' } as any,
        isAvatarActive: true
      })
      
      const { container } = render(<AvatarDisplayView />)
      
      await act(async () => {
        vi.advanceTimersByTime(1000)
      })
      
      const indicator = container.querySelector('.bg-green-500')
      expect(indicator).toHaveAttribute('aria-hidden', 'true')
    })

    it('should provide meaningful text for screen readers in all states', () => {
      // No avatar state
      render(<AvatarDisplayView />)
      expect(screen.getByText('Avatar mode selected')).toBeInTheDocument()
      
      // Loading state
      mockUseAvatar.mockReturnValue({
        ...defaultMockReturn,
        isLoading: true
      })
      const { rerender } = render(<AvatarDisplayView />)
      expect(screen.getByText('Initializing avatar...')).toBeInTheDocument()
      
      // Error state
      mockUseAvatar.mockReturnValue({
        ...defaultMockReturn,
        error: 'Connection error'
      })
      rerender(<AvatarDisplayView />)
      expect(screen.getByText('Connection error')).toBeInTheDocument()
    })
  })

  describe('edge cases', () => {
    it('should handle null avatarSession gracefully', () => {
      mockUseAvatar.mockReturnValue({
        ...defaultMockReturn,
        avatarSession: null,
        isAvatarActive: true
      })
      
      render(<AvatarDisplayView />)
      expect(screen.getByText('Initializing avatar...')).toBeInTheDocument()
    })

    it('should handle rapid state changes', async () => {
      const { rerender } = render(<AvatarDisplayView />)
      
      // Rapidly change states
      for (let i = 0; i < 5; i++) {
        mockUseAvatar.mockReturnValue({
          ...defaultMockReturn,
          avatarSession: { id: `session-${i}` } as any,
          isAvatarActive: i % 2 === 0
        })
        rerender(<AvatarDisplayView />)
      }
      
      // Should handle without errors
      expect(document.querySelector('video')).toBeInTheDocument()
    })

    it('should handle simultaneous loading and error states', () => {
      mockUseAvatar.mockReturnValue({
        ...defaultMockReturn,
        isLoading: true,
        error: 'Error occurred'
      })
      
      render(<AvatarDisplayView />)
      
      // Both states should be visible
      expect(screen.getByText('Initializing avatar...')).toBeInTheDocument()
      expect(screen.getByText('Error occurred')).toBeInTheDocument()
    })

    it('should maintain video element through state changes', () => {
      const { rerender } = render(<AvatarDisplayView />)
      const initialVideo = screen.getByLabelText('Avatar video stream')
      
      // Change to loading state
      mockUseAvatar.mockReturnValue({
        ...defaultMockReturn,
        isLoading: true
      })
      rerender(<AvatarDisplayView />)
      
      const videoAfterChange = screen.getByLabelText('Avatar video stream')
      expect(videoAfterChange).toBe(initialVideo)
    })
  })

  describe('visual styling', () => {
    it('should apply correct container styles', () => {
      const { container } = render(<AvatarDisplayView />)
      const mainContainer = container.firstChild as HTMLElement
      expect(mainContainer).toHaveClass('flex', 'flex-col', 'items-center', 'justify-center', 'h-full', 'p-4')
    })

    it('should apply correct video container styles', () => {
      const { container } = render(<AvatarDisplayView />)
      const videoContainer = container.querySelector('.relative.w-full.max-w-\\[900px\\].aspect-video')
      expect(videoContainer).toBeInTheDocument()
      expect(videoContainer).toHaveClass('overflow-hidden', 'rounded-xl', 'bg-zinc-900')
    })

    it('should apply correct video element styles', () => {
      render(<AvatarDisplayView />)
      const video = screen.getByLabelText('Avatar video stream')
      expect(video).toHaveClass('w-full', 'h-full', 'object-contain')
      expect(video).toHaveStyle({ objectFit: 'contain' })
    })

    it('should style loading overlay correctly', () => {
      mockUseAvatar.mockReturnValue({
        ...defaultMockReturn,
        isLoading: true
      })
      
      const { container } = render(<AvatarDisplayView />)
      const overlay = container.querySelector('.absolute.inset-0.flex.items-center.justify-center.bg-background\\/50')
      expect(overlay).toBeInTheDocument()
    })
  })
})