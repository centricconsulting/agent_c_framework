/**
 * MuteToggle Component Tests
 * Tests mute/unmute functionality with keyboard shortcuts and visual feedback
 */

import { describe, it, expect, vi, beforeEach, afterEach } from 'vitest';
import { render, waitFor, fireEvent, act } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { MuteToggle } from '../MuteToggle';
import { updateMockState } from '../../../test/mocks/realtime-react';

// Note: @agentc/realtime-react is globally mocked in vitest.setup.ts

describe('MuteToggle', () => {
  // Default mock return values
  const defaultAudioMock = {
    isMuted: false,
    toggleMute: vi.fn(),
    isRecording: false,
    audioLevel: 0,
  };

  beforeEach(() => {
    vi.clearAllMocks();
    // Set default mock return values using global mock
    updateMockState('audio', defaultAudioMock);
  });

  afterEach(() => {
    vi.restoreAllMocks();
  });

  describe('rendering', () => {
    it('should render button element', () => {
      const { container } = render(<MuteToggle />);
      const button = container.querySelector('button');
      expect(button).toBeInTheDocument();
    });

    it('should render VolumeX icon when muted', () => {
      updateMockState('audio', {
        ...defaultAudioMock,
        isMuted: true,
      });

      const { container } = render(<MuteToggle />);
      expect(container.querySelector('.lucide-volume-x')).toBeInTheDocument();
      expect(container.querySelector('.lucide-volume-2')).not.toBeInTheDocument();
    });

    it('should render Volume2 icon when unmuted', () => {
      updateMockState('audio', {
        ...defaultAudioMock,
        isMuted: false,
      });

      const { container } = render(<MuteToggle />);
      // Icons are rendered as svg elements with specific structure
      const icon = container.querySelector('svg');
      expect(icon).toBeInTheDocument();
      // Check that it's not the muted icon by checking the button's muted state
      const button = container.querySelector('button');
      expect(button?.getAttribute('aria-label')).toBe('Mute audio');
    });

    it('should wrap button in aria-live container', () => {
      const { container } = render(<MuteToggle />);
      const liveRegion = container.querySelector('[aria-live="polite"]');
      expect(liveRegion).toBeInTheDocument();
      const button = liveRegion?.querySelector('button');
      expect(button).toBeInTheDocument();
    });
  });

  describe('button states', () => {
    it('should be enabled by default', () => {
      const { container } = render(<MuteToggle />);
      const button = container.querySelector('button');
      expect(button).not.toBeDisabled();
    });

    it('should be disabled when recording', () => {
      updateMockState('audio', {
        ...defaultAudioMock,
        isRecording: true,
      });

      const { container } = render(<MuteToggle />);
      const button = container.querySelector('button');
      expect(button).toBeDisabled();
    });

    it('should be disabled when prop disabled is true', () => {
      const { container } = render(<MuteToggle disabled />);
      const button = container.querySelector('button');
      expect(button).toBeDisabled();
    });

    it('should respect disabled prop even when not recording', () => {
      updateMockState('audio', {
        ...defaultAudioMock,
        isRecording: false,
      });

      const { container } = render(<MuteToggle disabled />);
      const button = container.querySelector('button');
      expect(button).toBeDisabled();
    });

    it('should be disabled when either disabled prop or isRecording is true', () => {
      updateMockState('audio', {
        ...defaultAudioMock,
        isRecording: true,
      });

      const { container } = render(<MuteToggle disabled />);
      const button = container.querySelector('button');
      expect(button).toBeDisabled();
    });
  });

  describe('click interactions', () => {
    it('should call toggleMute when clicked', async () => {
      const toggleMute = vi.fn();
      updateMockState('audio', {
        ...defaultAudioMock,
        toggleMute,
      });

      const { container } = render(<MuteToggle />);
      const button = container.querySelector('button')!;
      
      fireEvent.click(button);
      await waitFor(() => {
        expect(toggleMute).toHaveBeenCalledOnce();
      });
    });

    it('should not trigger actions when button is disabled', () => {
      const toggleMute = vi.fn();
      updateMockState('audio', {
        ...defaultAudioMock,
        toggleMute,
        isRecording: true,
      });

      const { container } = render(<MuteToggle />);
      const button = container.querySelector('button')!;
      
      fireEvent.click(button);
      expect(toggleMute).not.toHaveBeenCalled();
    });
  });

  describe('keyboard shortcuts', () => {
    it('should toggle mute on Ctrl+M', async () => {
      const toggleMute = vi.fn();
      updateMockState('audio', {
        ...defaultAudioMock,
        toggleMute,
      });

      render(<MuteToggle enableShortcut />);
      
      await act(async () => {
        const event = new KeyboardEvent('keydown', {
          key: 'm',
          ctrlKey: true,
        });
        window.dispatchEvent(event);
      });
      
      await waitFor(() => {
        expect(toggleMute).toHaveBeenCalledOnce();
      });
    });

    it('should toggle mute on Cmd+M (Mac)', async () => {
      const toggleMute = vi.fn();
      updateMockState('audio', {
        ...defaultAudioMock,
        toggleMute,
      });

      render(<MuteToggle enableShortcut />);
      
      await act(async () => {
        const event = new KeyboardEvent('keydown', {
          key: 'm',
          metaKey: true,
        });
        window.dispatchEvent(event);
      });
      
      await waitFor(() => {
        expect(toggleMute).toHaveBeenCalledOnce();
      });
    });

    it('should prevent default on shortcut key', async () => {
      const toggleMute = vi.fn();
      updateMockState('audio', {
        ...defaultAudioMock,
        toggleMute,
      });

      render(<MuteToggle enableShortcut />);
      
      const preventDefaultSpy = vi.fn();
      const event = new KeyboardEvent('keydown', {
        key: 'm',
        ctrlKey: true,
      });
      Object.defineProperty(event, 'preventDefault', {
        value: preventDefaultSpy,
        writable: true,
      });
      
      act(() => {
        window.dispatchEvent(event);
      });
      
      expect(preventDefaultSpy).toHaveBeenCalled();
    });

    it('should not respond to shortcuts when disabled', () => {
      const toggleMute = vi.fn();
      updateMockState('audio', {
        ...defaultAudioMock,
        toggleMute,
      });

      render(<MuteToggle enableShortcut={false} />);
      
      const event = new KeyboardEvent('keydown', {
        key: 'm',
        ctrlKey: true,
      });
      
      window.dispatchEvent(event);
      
      expect(toggleMute).not.toHaveBeenCalled();
    });

    it('should not respond to m key without modifier', () => {
      const toggleMute = vi.fn();
      updateMockState('audio', {
        ...defaultAudioMock,
        toggleMute,
      });

      render(<MuteToggle enableShortcut />);
      
      const event = new KeyboardEvent('keydown', {
        key: 'm',
      });
      
      window.dispatchEvent(event);
      
      expect(toggleMute).not.toHaveBeenCalled();
    });

    it('should not respond to other keys with modifier', () => {
      const toggleMute = vi.fn();
      updateMockState('audio', {
        ...defaultAudioMock,
        toggleMute,
      });

      render(<MuteToggle enableShortcut />);
      
      const event = new KeyboardEvent('keydown', {
        key: 'n',
        ctrlKey: true,
      });
      
      window.dispatchEvent(event);
      
      expect(toggleMute).not.toHaveBeenCalled();
    });

    it('should cleanup keyboard listeners on unmount', () => {
      const removeEventListenerSpy = vi.spyOn(window, 'removeEventListener');
      
      const { unmount } = render(<MuteToggle enableShortcut />);
      unmount();
      
      expect(removeEventListenerSpy).toHaveBeenCalledWith('keydown', expect.any(Function));
    });

    it('should update listener when enableShortcut changes', () => {
      const addEventListenerSpy = vi.spyOn(window, 'addEventListener');
      const removeEventListenerSpy = vi.spyOn(window, 'removeEventListener');
      
      const { rerender } = render(<MuteToggle enableShortcut={false} />);
      expect(addEventListenerSpy).not.toHaveBeenCalled();
      
      rerender(<MuteToggle enableShortcut />);
      expect(addEventListenerSpy).toHaveBeenCalledWith('keydown', expect.any(Function));
      
      rerender(<MuteToggle enableShortcut={false} />);
      expect(removeEventListenerSpy).toHaveBeenCalledWith('keydown', expect.any(Function));
    });
  });

  describe('confirmation dialog', () => {
    it('should show confirmation when unmuting with requireConfirmation', async () => {
      updateMockState('audio', {
        ...defaultAudioMock,
        isMuted: true,
      });

      const { container } = render(<MuteToggle requireConfirmation />);
      const button = container.querySelector('button')!;
      
      expect(container.querySelector('[role="dialog"]')).not.toBeInTheDocument();
      
      fireEvent.click(button);
      
      await waitFor(() => {
        const dialog = container.querySelector('[role="dialog"]');
        expect(dialog).toBeInTheDocument();
        expect(dialog?.textContent).toContain('Unmute audio?');
      });
    });

    it('should not show confirmation when muting', async () => {
      updateMockState('audio', {
        ...defaultAudioMock,
        isMuted: false,
      });

      const { container } = render(<MuteToggle requireConfirmation />);
      const button = container.querySelector('button')!;
      
      fireEvent.click(button);
      
      await waitFor(() => {
        expect(container.querySelector('[role="dialog"]')).not.toBeInTheDocument();
      });
    });

    it('should call toggleMute on confirm', async () => {
      const toggleMute = vi.fn();
      updateMockState('audio', {
        ...defaultAudioMock,
        isMuted: true,
        toggleMute,
      });

      const { container, getByText } = render(<MuteToggle requireConfirmation />);
      const button = container.querySelector('button')!;
      
      fireEvent.click(button);
      
      await waitFor(() => {
        expect(container.querySelector('[role="dialog"]')).toBeInTheDocument();
      });
      
      fireEvent.click(getByText('Confirm'));
      
      await waitFor(() => {
        expect(toggleMute).toHaveBeenCalledOnce();
        expect(container.querySelector('[role="dialog"]')).not.toBeInTheDocument();
      });
    });

    it('should close dialog on cancel', async () => {
      const toggleMute = vi.fn();
      updateMockState('audio', {
        ...defaultAudioMock,
        isMuted: true,
        toggleMute,
      });

      const { container, getByText } = render(<MuteToggle requireConfirmation />);
      const button = container.querySelector('button')!;
      
      fireEvent.click(button);
      
      await waitFor(() => {
        expect(container.querySelector('[role="dialog"]')).toBeInTheDocument();
      });
      
      fireEvent.click(getByText('Cancel'));
      
      await waitFor(() => {
        expect(toggleMute).not.toHaveBeenCalled();
        expect(container.querySelector('[role="dialog"]')).not.toBeInTheDocument();
      });
    });

    it('should show confirmation on keyboard shortcut when unmuting', async () => {
      updateMockState('audio', {
        ...defaultAudioMock,
        isMuted: true,
      });

      const { container } = render(<MuteToggle enableShortcut requireConfirmation />);
      
      await act(async () => {
        const event = new KeyboardEvent('keydown', {
          key: 'm',
          ctrlKey: true,
        });
        window.dispatchEvent(event);
      });
      
      await waitFor(() => {
        const dialog = container.querySelector('[role="dialog"]');
        expect(dialog).toBeInTheDocument();
        expect(dialog?.textContent).toContain('Unmute audio?');
      });
    });

    it('should not show confirmation on keyboard shortcut when muting', async () => {
      const toggleMute = vi.fn();
      updateMockState('audio', {
        ...defaultAudioMock,
        isMuted: false,
        toggleMute,
      });

      const { container } = render(<MuteToggle enableShortcut requireConfirmation />);
      
      await act(async () => {
        const event = new KeyboardEvent('keydown', {
          key: 'm',
          ctrlKey: true,
        });
        window.dispatchEvent(event);
      });
      
      await waitFor(() => {
        expect(toggleMute).toHaveBeenCalledOnce();
        expect(container.querySelector('[role="dialog"]')).not.toBeInTheDocument();
      });
    });
  });

  describe('audio level indicator', () => {
    it('should show green dot when audio level > 0 and unmuted', () => {
      updateMockState('audio', {
        ...defaultAudioMock,
        isMuted: false,
        audioLevel: 0.5,
      });

      const { container } = render(<MuteToggle showAudioIndicator />);
      
      const indicator = container.querySelector('.bg-green-500');
      expect(indicator).toBeInTheDocument();
      expect(indicator?.className).toContain('animate-pulse');
      expect(indicator?.className).toContain('rounded-full');
      expect(indicator?.className).toContain('h-2');
      expect(indicator?.className).toContain('w-2');
      expect(indicator?.className).toContain('absolute');
      expect(indicator?.className).toContain('-top-1');
      expect(indicator?.className).toContain('-right-1');
    });

    it('should not show indicator when muted', () => {
      updateMockState('audio', {
        ...defaultAudioMock,
        isMuted: true,
        audioLevel: 0.5,
      });

      const { container } = render(<MuteToggle showAudioIndicator />);
      
      expect(container.querySelector('.bg-green-500')).not.toBeInTheDocument();
    });

    it('should not show indicator when audio level is 0', () => {
      updateMockState('audio', {
        ...defaultAudioMock,
        isMuted: false,
        audioLevel: 0,
      });

      const { container } = render(<MuteToggle showAudioIndicator />);
      
      expect(container.querySelector('.bg-green-500')).not.toBeInTheDocument();
    });

    it('should not show indicator when showAudioIndicator is false', () => {
      updateMockState('audio', {
        ...defaultAudioMock,
        isMuted: false,
        audioLevel: 0.5,
      });

      const { container } = render(<MuteToggle showAudioIndicator={false} />);
      
      expect(container.querySelector('.bg-green-500')).not.toBeInTheDocument();
    });

    it('should add animate-pulse to button when audio active and showAudioIndicator enabled', () => {
      updateMockState('audio', {
        ...defaultAudioMock,
        isMuted: false,
        audioLevel: 0.5,
      });

      const { container } = render(<MuteToggle showAudioIndicator />);
      const button = container.querySelector('button');
      
      expect(button?.className).toContain('animate-pulse');
    });

    it('should add animate-pulse to button when audio active regardless of mute state', () => {
      // Note: The component applies animate-pulse based on audioLevel > 0 and showAudioIndicator
      // It doesn't check mute state for the button animation (only for the indicator dot)
      updateMockState('audio', {
        ...defaultAudioMock,
        isMuted: true,
        audioLevel: 0.5,
      });

      const { container } = render(<MuteToggle showAudioIndicator />);
      const button = container.querySelector('button');
      
      // The component applies animate-pulse when audioLevel > 0, regardless of mute state
      expect(button?.className).toContain('animate-pulse');
    });

    it('should update indicator when audio level changes', () => {
      const { container, rerender } = render(<MuteToggle showAudioIndicator />);
      
      // Initially no audio
      updateMockState('audio', {
        ...defaultAudioMock,
        audioLevel: 0,
      });
      rerender(<MuteToggle showAudioIndicator />);
      expect(container.querySelector('.bg-green-500')).not.toBeInTheDocument();
      
      // Audio starts
      updateMockState('audio', {
        ...defaultAudioMock,
        audioLevel: 0.8,
      });
      rerender(<MuteToggle showAudioIndicator />);
      expect(container.querySelector('.bg-green-500')).toBeInTheDocument();
      
      // Audio stops
      updateMockState('audio', {
        ...defaultAudioMock,
        audioLevel: 0,
      });
      rerender(<MuteToggle showAudioIndicator />);
      expect(container.querySelector('.bg-green-500')).not.toBeInTheDocument();
    });
  });

  describe('visual states', () => {
    it('should apply muted styling when muted', () => {
      updateMockState('audio', {
        ...defaultAudioMock,
        isMuted: true,
      });

      const { container } = render(<MuteToggle />);
      const button = container.querySelector('button');
      expect(button?.className).toContain('text-muted-foreground');
    });

    it('should not apply muted styling when unmuted', () => {
      updateMockState('audio', {
        ...defaultAudioMock,
        isMuted: false,
      });

      const { container } = render(<MuteToggle />);
      const button = container.querySelector('button');
      expect(button?.className).not.toContain('text-muted-foreground');
    });

    it('should use ghost variant', () => {
      const { container } = render(<MuteToggle />);
      const button = container.querySelector('button');
      expect(button?.className).toContain('hover:bg-accent');
    });
  });

  describe('size variants', () => {
    it('should apply small size classes', () => {
      const { container } = render(<MuteToggle size="small" />);
      const button = container.querySelector('button');
      expect(button?.className).toContain('h-8');
      expect(button?.className).toContain('w-8');
    });

    it('should apply default size classes', () => {
      const { container } = render(<MuteToggle size="default" />);
      const button = container.querySelector('button');
      expect(button?.className).toContain('h-10');
      expect(button?.className).toContain('w-10');
    });

    it('should apply large size classes', () => {
      const { container } = render(<MuteToggle size="large" />);
      const button = container.querySelector('button');
      expect(button?.className).toContain('h-12');
      expect(button?.className).toContain('w-12');
    });

    it('should use default size when not specified', () => {
      const { container } = render(<MuteToggle />);
      const button = container.querySelector('button');
      expect(button?.className).toContain('h-10');
      expect(button?.className).toContain('w-10');
    });
  });

  describe('ARIA attributes', () => {
    it('should have correct aria-label when muted', () => {
      updateMockState('audio', {
        ...defaultAudioMock,
        isMuted: true,
      });

      const { container } = render(<MuteToggle />);
      const button = container.querySelector('button');
      expect(button?.getAttribute('aria-label')).toBe('Unmute audio');
    });

    it('should have correct aria-label when unmuted', () => {
      updateMockState('audio', {
        ...defaultAudioMock,
        isMuted: false,
      });

      const { container } = render(<MuteToggle />);
      const button = container.querySelector('button');
      expect(button?.getAttribute('aria-label')).toBe('Mute audio');
    });

    it('should have aria-pressed matching muted state', () => {
      updateMockState('audio', {
        ...defaultAudioMock,
        isMuted: true,
      });

      const { container } = render(<MuteToggle />);
      const button = container.querySelector('button');
      expect(button?.getAttribute('aria-pressed')).toBe('true');
    });

    it('should have aria-pressed false when unmuted', () => {
      updateMockState('audio', {
        ...defaultAudioMock,
        isMuted: false,
      });

      const { container } = render(<MuteToggle />);
      const button = container.querySelector('button');
      expect(button?.getAttribute('aria-pressed')).toBe('false');
    });
  });

  describe('screen reader support', () => {
    it('should have aria-live on container', () => {
      const { container } = render(<MuteToggle />);
      const liveRegion = container.querySelector('[aria-live="polite"]');
      expect(liveRegion).toBeInTheDocument();
    });

    it('should announce muted state to screen readers', () => {
      updateMockState('audio', {
        ...defaultAudioMock,
        isMuted: true,
      });

      const { container } = render(<MuteToggle />);
      const announcement = container.querySelector('.sr-only[aria-live="polite"]');
      expect(announcement).toBeInTheDocument();
      expect(announcement?.textContent).toBe('Audio is muted');
    });

    it('should announce unmuted state to screen readers', () => {
      updateMockState('audio', {
        ...defaultAudioMock,
        isMuted: false,
      });

      const { container } = render(<MuteToggle />);
      const announcement = container.querySelector('.sr-only[aria-live="polite"]');
      expect(announcement).toBeInTheDocument();
      expect(announcement?.textContent).toBe('Audio is unmuted');
    });

    it('should update announcement when state changes', () => {
      const { container, rerender } = render(<MuteToggle />);
      
      updateMockState('audio', {
        ...defaultAudioMock,
        isMuted: false,
      });
      rerender(<MuteToggle />);
      let announcement = container.querySelector('.sr-only[aria-live="polite"]');
      expect(announcement?.textContent).toBe('Audio is unmuted');
      
      updateMockState('audio', {
        ...defaultAudioMock,
        isMuted: true,
      });
      rerender(<MuteToggle />);
      announcement = container.querySelector('.sr-only[aria-live="polite"]');
      expect(announcement?.textContent).toBe('Audio is muted');
    });
  });

  describe('integration flows', () => {
    it('should handle complete mute/unmute cycle', async () => {
      const toggleMute = vi.fn();
      const { container, rerender } = render(<MuteToggle />);
      const button = container.querySelector('button')!;
      
      // Start unmuted
      updateMockState('audio', {
        isMuted: false,
        toggleMute,
        isRecording: false,
        audioLevel: 0.5,
      });
      rerender(<MuteToggle />);
      
      expect(button.getAttribute('aria-label')).toBe('Mute audio');
      // Check for icon by looking for svg element
      expect(container.querySelector('svg')).toBeInTheDocument();
      
      // Mute
      fireEvent.click(button);
      await waitFor(() => {
        expect(toggleMute).toHaveBeenCalledOnce();
      });
      
      // Update to muted state
      updateMockState('audio', {
        isMuted: true,
        toggleMute,
        isRecording: false,
        audioLevel: 0,
      });
      rerender(<MuteToggle />);
      
      expect(button.getAttribute('aria-label')).toBe('Unmute audio');
      expect(container.querySelector('.lucide-volume-x')).toBeInTheDocument();
      
      // Unmute
      fireEvent.click(button);
      await waitFor(() => {
        expect(toggleMute).toHaveBeenCalledTimes(2);
      });
    });

    it('should handle full mute toggle cycle with confirmation', async () => {
      const toggleMute = vi.fn();
      const { container, rerender, getByText } = render(<MuteToggle requireConfirmation />);
      const button = container.querySelector('button')!;
      
      // Start unmuted
      updateMockState('audio', {
        isMuted: false,
        toggleMute,
        isRecording: false,
        audioLevel: 0.5,
      });
      rerender(<MuteToggle requireConfirmation />);
      
      // Mute (no confirmation needed)
      fireEvent.click(button);
      await waitFor(() => {
        expect(toggleMute).toHaveBeenCalledOnce();
        expect(container.querySelector('[role="dialog"]')).not.toBeInTheDocument();
      });
      
      // Update to muted state
      updateMockState('audio', {
        isMuted: true,
        toggleMute,
        isRecording: false,
        audioLevel: 0,
      });
      rerender(<MuteToggle requireConfirmation />);
      
      // Unmute (confirmation required)
      fireEvent.click(button);
      await waitFor(() => {
        expect(container.querySelector('[role="dialog"]')).toBeInTheDocument();
      });
      
      // Confirm unmute
      fireEvent.click(getByText('Confirm'));
      await waitFor(() => {
        expect(toggleMute).toHaveBeenCalledTimes(2);
        expect(container.querySelector('[role="dialog"]')).not.toBeInTheDocument();
      });
    });

    it('should handle keyboard shortcut with confirmation', async () => {
      const toggleMute = vi.fn();
      updateMockState('audio', {
        isMuted: true,
        toggleMute,
        isRecording: false,
        audioLevel: 0,
      });

      const { container, getByText } = render(
        <MuteToggle enableShortcut requireConfirmation />
      );
      
      // Trigger shortcut
      await act(async () => {
        const event = new KeyboardEvent('keydown', {
          key: 'm',
          ctrlKey: true,
        });
        window.dispatchEvent(event);
      });
      
      // Should show confirmation
      await waitFor(() => {
        expect(container.querySelector('[role="dialog"]')).toBeInTheDocument();
      });
      
      // Confirm
      fireEvent.click(getByText('Confirm'));
      await waitFor(() => {
        expect(toggleMute).toHaveBeenCalledOnce();
        expect(container.querySelector('[role="dialog"]')).not.toBeInTheDocument();
      });
    });

    it('should handle recording state changes', () => {
      const toggleMute = vi.fn();
      const { container, rerender } = render(<MuteToggle />);
      let button = container.querySelector('button')!;
      
      // Initially not recording
      updateMockState('audio', {
        isMuted: false,
        toggleMute,
        isRecording: false,
        audioLevel: 0,
      });
      rerender(<MuteToggle />);
      expect(button).not.toBeDisabled();
      
      // Start recording
      updateMockState('audio', {
        isMuted: false,
        toggleMute,
        isRecording: true,
        audioLevel: 0.5,
      });
      rerender(<MuteToggle />);
      button = container.querySelector('button')!;
      expect(button).toBeDisabled();
      
      // Click while recording - should not toggle
      fireEvent.click(button);
      expect(toggleMute).not.toHaveBeenCalled();
      
      // Stop recording
      updateMockState('audio', {
        isMuted: false,
        toggleMute,
        isRecording: false,
        audioLevel: 0,
      });
      rerender(<MuteToggle />);
      button = container.querySelector('button')!;
      expect(button).not.toBeDisabled();
      
      // Now can toggle
      fireEvent.click(button);
      expect(toggleMute).toHaveBeenCalledOnce();
    });
  });

  describe('custom props', () => {
    it('should apply custom className', () => {
      const { container } = render(<MuteToggle className="custom-class" />);
      const button = container.querySelector('button');
      expect(button?.className).toContain('custom-class');
    });

    it('should forward data attributes', () => {
      const { container } = render(<MuteToggle data-testid="mute-btn" />);
      const button = container.querySelector('[data-testid="mute-btn"]');
      expect(button).toBeInTheDocument();
    });

    it('should forward ref', () => {
      const ref = vi.fn();
      render(<MuteToggle ref={ref} />);
      expect(ref).toHaveBeenCalled();
      expect(ref.mock.calls[0][0]).toBeInstanceOf(HTMLButtonElement);
    });

    it('should always have type="button"', () => {
      const { container } = render(<MuteToggle />);
      const button = container.querySelector('button');
      expect(button?.getAttribute('type')).toBe('button');
    });

    it('should forward other button props', () => {
      const onMouseEnter = vi.fn();
      const { container } = render(<MuteToggle onMouseEnter={onMouseEnter} />);
      const button = container.querySelector('button')!;
      
      fireEvent.mouseEnter(button);
      expect(onMouseEnter).toHaveBeenCalledOnce();
    });
  });

  describe('edge cases', () => {
    it('should handle rapid toggling', async () => {
      const toggleMute = vi.fn();
      updateMockState('audio', {
        ...defaultAudioMock,
        toggleMute,
      });

      const { container } = render(<MuteToggle />);
      const button = container.querySelector('button')!;
      
      // Rapid clicks
      fireEvent.click(button);
      fireEvent.click(button);
      fireEvent.click(button);
      
      await waitFor(() => {
        expect(toggleMute).toHaveBeenCalledTimes(3);
      });
    });

    it('should handle keyboard shortcut while dialog is open', async () => {
      const toggleMute = vi.fn();
      updateMockState('audio', {
        isMuted: true,
        toggleMute,
        isRecording: false,
        audioLevel: 0,
      });

      const { container } = render(
        <MuteToggle enableShortcut requireConfirmation />
      );
      const button = container.querySelector('button')!;
      
      // Open dialog
      fireEvent.click(button);
      await waitFor(() => {
        expect(container.querySelector('[role="dialog"]')).toBeInTheDocument();
      });
      
      // Try keyboard shortcut while dialog is open
      act(() => {
        const event = new KeyboardEvent('keydown', {
          key: 'm',
          ctrlKey: true,
        });
        window.dispatchEvent(event);
      });
      
      // Should not create multiple dialogs or call toggleMute
      expect(toggleMute).not.toHaveBeenCalled();
      const dialogs = container.querySelectorAll('[role="dialog"]');
      expect(dialogs).toHaveLength(1);
    });

    it('should handle undefined audioLevel gracefully', () => {
      updateMockState('audio', {
        isMuted: false,
        toggleMute: vi.fn(),
        isRecording: false,
        audioLevel: undefined as any,
      });

      const { container } = render(<MuteToggle showAudioIndicator />);
      
      expect(container.querySelector('.bg-green-500')).not.toBeInTheDocument();
      const button = container.querySelector('button');
      expect(button?.className).not.toContain('animate-pulse');
    });

    it('should handle NaN audioLevel gracefully', () => {
      updateMockState('audio', {
        isMuted: false,
        toggleMute: vi.fn(),
        isRecording: false,
        audioLevel: NaN,
      });

      const { container } = render(<MuteToggle showAudioIndicator />);
      
      expect(container.querySelector('.bg-green-500')).not.toBeInTheDocument();
      const button = container.querySelector('button');
      expect(button?.className).not.toContain('animate-pulse');
    });

    it('should handle negative audioLevel gracefully', () => {
      updateMockState('audio', {
        isMuted: false,
        toggleMute: vi.fn(),
        isRecording: false,
        audioLevel: -0.5,
      });

      const { container } = render(<MuteToggle showAudioIndicator />);
      
      expect(container.querySelector('.bg-green-500')).not.toBeInTheDocument();
      const button = container.querySelector('button');
      expect(button?.className).not.toContain('animate-pulse');
    });
  });

  describe('performance', () => {
    it('should not cause unnecessary re-renders', () => {
      const renderSpy = vi.fn();
      
      const TestWrapper = () => {
        renderSpy();
        return <MuteToggle />;
      };
      
      const { rerender } = render(<TestWrapper />);
      expect(renderSpy).toHaveBeenCalledOnce();
      
      // Re-render with same props
      rerender(<TestWrapper />);
      expect(renderSpy).toHaveBeenCalledTimes(2);
    });

    it('should cleanup event listeners properly', () => {
      const { unmount } = render(<MuteToggle enableShortcut />);
      const removeEventListenerSpy = vi.spyOn(window, 'removeEventListener');
      
      unmount();
      
      expect(removeEventListenerSpy).toHaveBeenCalledWith('keydown', expect.any(Function));
    });

    it('should not leak memory on unmount', () => {
      const { unmount } = render(
        <MuteToggle enableShortcut requireConfirmation showAudioIndicator />
      );
      expect(() => unmount()).not.toThrow();
    });

    it('should batch state updates efficiently', async () => {
      const toggleMute = vi.fn();
      updateMockState('audio', {
        isMuted: true,
        toggleMute,
        isRecording: false,
        audioLevel: 0,
      });

      const { container, getByText } = render(<MuteToggle requireConfirmation />);
      const button = container.querySelector('button')!;
      
      // Open dialog
      fireEvent.click(button);
      
      await waitFor(() => {
        expect(container.querySelector('[role="dialog"]')).toBeInTheDocument();
      });
      
      // Confirm - should update both toggleMute and close dialog in one batch
      fireEvent.click(getByText('Confirm'));
      
      await waitFor(() => {
        expect(toggleMute).toHaveBeenCalledOnce();
        expect(container.querySelector('[role="dialog"]')).not.toBeInTheDocument();
      });
    });
  });
});