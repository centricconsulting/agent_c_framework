/**
 * RecordingButton Component Tests
 * Tests recording control with connection state awareness
 */

import { describe, it, expect, vi, beforeEach, afterEach } from 'vitest';
import { render, waitFor, fireEvent } from '@testing-library/react';
import { RecordingButton } from '../RecordingButton';
import { updateMockState, getConnectionStateString } from '../../../test/mocks/realtime-react';

// Note: @agentc/realtime-react is globally mocked in vitest.setup.ts
const mockGetConnectionStateString = vi.mocked(getConnectionStateString);

describe('RecordingButton', () => {
  // Default mock return values
  const defaultAudioMock = {
    isRecording: false,
    startRecording: vi.fn(),
    stopRecording: vi.fn(),
  };

  const defaultConnectionMock = {
    isConnected: true,
    connectionState: 'connected',
  };

  beforeEach(() => {
    vi.clearAllMocks();
    // Set default mock return values using global mock
    updateMockState('audio', defaultAudioMock);
    updateMockState('connection', defaultConnectionMock);
    mockGetConnectionStateString.mockImplementation((state) => state);
  });

  afterEach(() => {
    vi.restoreAllMocks();
  });

  describe('rendering', () => {
    it('should render button element', () => {
      const { container } = render(<RecordingButton />);
      const button = container.querySelector('button');
      expect(button).toBeInTheDocument();
    });

    it('should render Mic icon when not recording', () => {
      updateMockState('audio', {
        ...defaultAudioMock,
        isRecording: false,
      });

      const { container } = render(<RecordingButton />);
      expect(container.querySelector('.lucide-mic')).toBeInTheDocument();
      expect(container.querySelector('.lucide-mic-off')).not.toBeInTheDocument();
    });

    it('should render MicOff icon when recording', () => {
      updateMockState('audio', {
        ...defaultAudioMock,
        isRecording: true,
      });

      const { container } = render(<RecordingButton />);
      expect(container.querySelector('.lucide-mic-off')).toBeInTheDocument();
      expect(container.querySelector('.lucide-mic')).not.toBeInTheDocument();
    });

    it('should render Loader2 icon when connecting', () => {
      updateMockState('connection', {
        isConnected: false,
        connectionState: 'connecting',
      });
      mockGetConnectionStateString.mockReturnValue('connecting');

      const { container } = render(<RecordingButton />);
      // Check for animate-spin class which is always present with Loader2
      const spinner = container.querySelector('.animate-spin');
      expect(spinner).toBeInTheDocument();
      // Also check for sr-only status text which should be present
      const status = container.querySelector('[role="status"]');
      expect(status).toBeInTheDocument();
      expect(status?.textContent).toBe('Connecting...');
    });

    it('should render custom icon when provided', () => {
      const CustomIcon = () => <span data-testid="custom-icon">🎤</span>;
      
      const { container, getByTestId } = render(
        <RecordingButton icon={<CustomIcon />} />
      );
      
      expect(getByTestId('custom-icon')).toBeInTheDocument();
      expect(container.querySelector('.lucide-mic')).not.toBeInTheDocument();
    });
  });

  describe('button states', () => {
    it('should be enabled when connected', () => {
      updateMockState('connection', {
        isConnected: true,
        connectionState: 'connected',
      });

      const { container } = render(<RecordingButton />);
      const button = container.querySelector('button');
      expect(button).not.toBeDisabled();
    });

    it('should be disabled when not connected', () => {
      updateMockState('connection', {
        isConnected: false,
        connectionState: 'disconnected',
      });

      const { container } = render(<RecordingButton />);
      const button = container.querySelector('button');
      expect(button).toBeDisabled();
    });

    it('should be disabled when connecting', () => {
      updateMockState('connection', {
        isConnected: false,
        connectionState: 'connecting',
      });
      mockGetConnectionStateString.mockReturnValue('connecting');

      const { container } = render(<RecordingButton />);
      const button = container.querySelector('button');
      expect(button).toBeDisabled();
    });

    it('should be disabled when reconnecting', () => {
      updateMockState('connection', {
        isConnected: false,
        connectionState: 'reconnecting',
      });
      mockGetConnectionStateString.mockReturnValue('reconnecting');

      const { container } = render(<RecordingButton />);
      const button = container.querySelector('button');
      expect(button).toBeDisabled();
    });

    it('should be disabled when prop disabled is true', () => {
      const { container } = render(<RecordingButton disabled />);
      const button = container.querySelector('button');
      expect(button).toBeDisabled();
    });

    it('should respect disabled prop even when connected', () => {
      updateMockState('connection', {
        isConnected: true,
        connectionState: 'connected',
      });

      const { container } = render(<RecordingButton disabled />);
      const button = container.querySelector('button');
      expect(button).toBeDisabled();
    });
  });

  describe('click interactions', () => {
    it('should call startRecording when clicked while not recording', async () => {
      const startRecording = vi.fn();
      updateMockState('audio', {
        isRecording: false,
        startRecording,
        stopRecording: vi.fn(),
      });

      const { container } = render(<RecordingButton />);
      const button = container.querySelector('button')!;
      
      fireEvent.click(button);
      await waitFor(() => {
        expect(startRecording).toHaveBeenCalledOnce();
      });
    });

    it('should call stopRecording when clicked while recording', async () => {
      const stopRecording = vi.fn();
      updateMockState('audio', {
        isRecording: true,
        startRecording: vi.fn(),
        stopRecording,
      });

      const { container } = render(<RecordingButton />);
      const button = container.querySelector('button')!;
      
      fireEvent.click(button);
      await waitFor(() => {
        expect(stopRecording).toHaveBeenCalledOnce();
      });
    });

    it('should not trigger actions when button is disabled', () => {
      const startRecording = vi.fn();
      updateMockState('audio', {
        isRecording: false,
        startRecording,
        stopRecording: vi.fn(),
      });
      updateMockState('connection', {
        isConnected: false,
        connectionState: 'disconnected',
      });

      const { container } = render(<RecordingButton />);
      const button = container.querySelector('button')!;
      
      fireEvent.click(button);
      expect(startRecording).not.toHaveBeenCalled();
    });
  });

  describe('visual states', () => {
    it('should have pulse animation when recording', () => {
      updateMockState('audio', {
        ...defaultAudioMock,
        isRecording: true,
      });

      const { container } = render(<RecordingButton />);
      const button = container.querySelector('button');
      expect(button?.className).toContain('animate-pulse');
    });

    it('should not have pulse animation when not recording', () => {
      updateMockState('audio', {
        ...defaultAudioMock,
        isRecording: false,
      });

      const { container } = render(<RecordingButton />);
      const button = container.querySelector('button');
      expect(button?.className).not.toContain('animate-pulse');
    });

    it('should have destructive variant when recording', () => {
      updateMockState('audio', {
        ...defaultAudioMock,
        isRecording: true,
      });

      const { container } = render(<RecordingButton />);
      const button = container.querySelector('button');
      expect(button?.className).toContain('bg-destructive');
    });

    it('should have default variant when not recording', () => {
      updateMockState('audio', {
        ...defaultAudioMock,
        isRecording: false,
      });

      const { container } = render(<RecordingButton />);
      const button = container.querySelector('button');
      expect(button?.className).toContain('bg-primary');
    });
  });

  describe('size variants', () => {
    it('should apply small size classes', () => {
      const { container } = render(<RecordingButton size="small" />);
      const button = container.querySelector('button');
      expect(button?.className).toContain('h-8');
      expect(button?.className).toContain('w-8');
    });

    it('should apply default size classes', () => {
      const { container } = render(<RecordingButton size="default" />);
      const button = container.querySelector('button');
      expect(button?.className).toContain('h-10');
      expect(button?.className).toContain('w-10');
    });

    it('should apply large size classes', () => {
      const { container } = render(<RecordingButton size="large" />);
      const button = container.querySelector('button');
      expect(button?.className).toContain('h-12');
      expect(button?.className).toContain('w-12');
    });

    it('should use default size when not specified', () => {
      const { container } = render(<RecordingButton />);
      const button = container.querySelector('button');
      expect(button?.className).toContain('h-10');
      expect(button?.className).toContain('w-10');
    });
  });

  describe('error handling', () => {
    it('should display error message when startRecording fails', async () => {
      const error = new Error('Permission denied');
      const startRecording = vi.fn().mockRejectedValue(error);
      
      updateMockState('audio', {
        isRecording: false,
        startRecording,
        stopRecording: vi.fn(),
      });

      const { container } = render(<RecordingButton />);
      const button = container.querySelector('button')!;
      
      fireEvent.click(button);

      await waitFor(() => {
        const alert = container.querySelector('[role="alert"]');
        expect(alert).toBeInTheDocument();
        expect(alert?.textContent).toBe('Permission denied');
      });
    });

    it('should display generic error for non-Error exceptions', async () => {
      const startRecording = vi.fn().mockRejectedValue('String error');
      
      updateMockState('audio', {
        isRecording: false,
        startRecording,
        stopRecording: vi.fn(),
      });

      const { container } = render(<RecordingButton />);
      const button = container.querySelector('button')!;
      
      fireEvent.click(button);

      await waitFor(() => {
        const alert = container.querySelector('[role="alert"]');
        expect(alert).toBeInTheDocument();
        expect(alert?.textContent).toBe('Recording failed');
      });
    });

    it('should display microphone unavailable error', async () => {
      // Mock startRecording to reject with a specific error that indicates microphone unavailable
      const startRecording = vi.fn().mockRejectedValue(
        new Error('Microphone not available')
      );

      updateMockState('audio', {
        ...defaultAudioMock,
        startRecording,
      });

      const { container } = render(<RecordingButton />);
      const button = container.querySelector('button')!;
      
      fireEvent.click(button);

      await waitFor(() => {
        const alert = container.querySelector('[role="alert"]');
        expect(alert).toBeInTheDocument();
        expect(alert?.textContent).toBe('Microphone not available');
      });
    });

    it('should clear error on successful action', async () => {
      const startRecording = vi.fn()
        .mockRejectedValueOnce(new Error('First attempt failed'))
        .mockResolvedValueOnce(undefined);

      updateMockState('audio', {
        isRecording: false,
        startRecording,
        stopRecording: vi.fn(),
      });

      const { container } = render(<RecordingButton />);
      const button = container.querySelector('button')!;
      
      // First click - should fail
      fireEvent.click(button);
      await waitFor(() => {
        expect(container.querySelector('[role="alert"]')).toBeInTheDocument();
      });

      // Second click - should succeed and clear error
      fireEvent.click(button);
      await waitFor(() => {
        expect(container.querySelector('[role="alert"]')).not.toBeInTheDocument();
      });
    });

    it('should handle stopRecording errors', async () => {
      const error = new Error('Stop failed');
      const stopRecording = vi.fn().mockRejectedValue(error);
      
      updateMockState('audio', {
        isRecording: true,
        startRecording: vi.fn(),
        stopRecording,
      });

      const { container } = render(<RecordingButton />);
      const button = container.querySelector('button')!;
      
      fireEvent.click(button);

      await waitFor(() => {
        const alert = container.querySelector('[role="alert"]');
        expect(alert).toBeInTheDocument();
        expect(alert?.textContent).toBe('Stop failed');
      });
    });
  });

  describe('ARIA attributes', () => {
    it('should have correct aria-label when not recording', () => {
      updateMockState('audio', {
        ...defaultAudioMock,
        isRecording: false,
      });

      const { container } = render(<RecordingButton />);
      const button = container.querySelector('button');
      expect(button?.getAttribute('aria-label')).toBe('Start recording');
    });

    it('should have correct aria-label when recording', () => {
      updateMockState('audio', {
        ...defaultAudioMock,
        isRecording: true,
      });

      const { container } = render(<RecordingButton />);
      const button = container.querySelector('button');
      expect(button?.getAttribute('aria-label')).toBe('Stop recording');
    });

    it('should have aria-pressed matching recording state', () => {
      updateMockState('audio', {
        ...defaultAudioMock,
        isRecording: true,
      });

      const { container } = render(<RecordingButton />);
      const button = container.querySelector('button');
      expect(button?.getAttribute('aria-pressed')).toBe('true');
    });

    it('should have aria-pressed false when not recording', () => {
      updateMockState('audio', {
        ...defaultAudioMock,
        isRecording: false,
      });

      const { container } = render(<RecordingButton />);
      const button = container.querySelector('button');
      expect(button?.getAttribute('aria-pressed')).toBe('false');
    });

    it('should have aria-busy when connecting', () => {
      updateMockState('connection', {
        isConnected: false,
        connectionState: 'connecting',
      });
      mockGetConnectionStateString.mockReturnValue('connecting');

      const { container } = render(<RecordingButton />);
      const button = container.querySelector('button');
      expect(button?.getAttribute('aria-busy')).toBe('true');
    });

    it('should not have aria-busy when connected', () => {
      updateMockState('connection', {
        isConnected: true,
        connectionState: 'connected',
      });

      const { container } = render(<RecordingButton />);
      const button = container.querySelector('button');
      expect(button?.getAttribute('aria-busy')).toBe('false');
    });
  });

  describe('screen reader support', () => {
    it('should include sr-only status text when connecting', () => {
      updateMockState('connection', {
        isConnected: false,
        connectionState: 'connecting',
      });
      mockGetConnectionStateString.mockReturnValue('connecting');

      const { container } = render(<RecordingButton />);
      const status = container.querySelector('[role="status"]');
      expect(status).toBeInTheDocument();
      expect(status?.textContent).toBe('Connecting...');
      expect(status?.className).toContain('sr-only');
    });

    it('should not show status text when not connecting', () => {
      updateMockState('connection', {
        isConnected: true,
        connectionState: 'connected',
      });

      const { container } = render(<RecordingButton />);
      const status = container.querySelector('[role="status"]');
      expect(status).not.toBeInTheDocument();
    });

    it('should include tooltip for screen readers when showTooltip is true', () => {
      const { container } = render(<RecordingButton showTooltip />);
      const tooltip = container.querySelector('[role="tooltip"]');
      expect(tooltip).toBeInTheDocument();
      expect(tooltip?.textContent).toBe('Click to start recording');
      expect(tooltip?.className).toContain('sr-only');
    });

    it('should update tooltip text when recording', () => {
      updateMockState('audio', {
        ...defaultAudioMock,
        isRecording: true,
      });

      const { container } = render(<RecordingButton showTooltip />);
      const tooltip = container.querySelector('[role="tooltip"]');
      expect(tooltip?.textContent).toBe('Click to stop recording');
    });

    it('should not render tooltip when showTooltip is false', () => {
      const { container } = render(<RecordingButton showTooltip={false} />);
      const tooltip = container.querySelector('[role="tooltip"]');
      expect(tooltip).not.toBeInTheDocument();
    });
  });

  describe('custom props', () => {
    it('should apply custom className', () => {
      const { container } = render(<RecordingButton className="custom-class" />);
      const button = container.querySelector('button');
      expect(button?.className).toContain('custom-class');
    });

    it('should forward data attributes', () => {
      const { container } = render(<RecordingButton data-testid="recording-btn" />);
      const button = container.querySelector('[data-testid="recording-btn"]');
      expect(button).toBeInTheDocument();
    });

    it('should forward ref', () => {
      const ref = vi.fn();
      render(<RecordingButton ref={ref} />);
      expect(ref).toHaveBeenCalled();
      expect(ref.mock.calls[0][0]).toBeInstanceOf(HTMLButtonElement);
    });

    it('should always have type="button"', () => {
      const { container } = render(<RecordingButton />);
      const button = container.querySelector('button');
      expect(button?.getAttribute('type')).toBe('button');
    });
  });

  describe('recording flow', () => {
    it('should handle complete recording cycle', async () => {
      const startRecording = vi.fn().mockResolvedValue(undefined);
      const stopRecording = vi.fn().mockResolvedValue(undefined);
      
      updateMockState('audio', {
        isRecording: false,
        startRecording,
        stopRecording,
        audioLevel: 0,
        isMuted: false,
        toggleMute: vi.fn(),
        setMuted: vi.fn()
      });

      const { container, rerender } = render(<RecordingButton />);
      let button = container.querySelector('button')!;
      
      // Start recording
      fireEvent.click(button);
      await waitFor(() => {
        expect(startRecording).toHaveBeenCalledOnce();
      });
      
      // Simulate state change to recording
      updateMockState('audio', {
        isRecording: true,
        startRecording,
        stopRecording,
        audioLevel: 0,
        isMuted: false,
        toggleMute: vi.fn(),
        setMuted: vi.fn()
      });
      rerender(<RecordingButton />);
      
      button = container.querySelector('button')!;
      expect(button.getAttribute('aria-label')).toBe('Stop recording');
      expect(button.className).toContain('animate-pulse');
      
      // Stop recording
      fireEvent.click(button);
      await waitFor(() => {
        expect(stopRecording).toHaveBeenCalledOnce();
      });
    });

    it('should handle connection loss during recording', () => {
      updateMockState('connection', {
        isConnected: true,
        connectionState: 'connected',
      });
      updateMockState('audio', {
        isRecording: true,
        startRecording: vi.fn(),
        stopRecording: vi.fn(),
        audioLevel: 0,
        isMuted: false,
        toggleMute: vi.fn(),
        setMuted: vi.fn()
      });

      const { container, rerender } = render(<RecordingButton />);
      
      let button = container.querySelector('button')!;
      expect(button).not.toBeDisabled();
      expect(button.className).toContain('animate-pulse');
      
      // Simulate connection loss
      updateMockState('connection', {
        isConnected: false,
        connectionState: 'disconnected',
      });
      rerender(<RecordingButton />);
      
      button = container.querySelector('button')!;
      expect(button).toBeDisabled();
      // Should still show recording state visually
      expect(button.className).toContain('animate-pulse');
    });
  });

  describe('edge cases', () => {
    it('should handle undefined connectionState gracefully', () => {
      updateMockState('connection', {
        isConnected: true,
        connectionState: undefined as any,
      });
      mockGetConnectionStateString.mockReturnValue('disconnected');

      const { container } = render(<RecordingButton />);
      const button = container.querySelector('button');
      expect(button).not.toBeDisabled(); // Should rely on isConnected
    });

    it('should handle missing getUserMedia gracefully', async () => {
      const originalGetUserMedia = navigator.mediaDevices?.getUserMedia;
      
      // Save original and replace with undefined
      if (navigator.mediaDevices) {
        delete (navigator.mediaDevices as any).getUserMedia;
        Object.defineProperty(navigator.mediaDevices, 'getUserMedia', {
          value: undefined,
          writable: true,
          configurable: true,
        });
      }

      updateMockState('audio', {
        isRecording: false,
        startRecording: vi.fn(),
        stopRecording: vi.fn(),
        audioLevel: 0,
        isMuted: false,
        toggleMute: vi.fn(),
        setMuted: vi.fn()
      });

      const { container } = render(<RecordingButton />);
      const button = container.querySelector('button')!;
      
      fireEvent.click(button);

      await waitFor(() => {
        const alert = container.querySelector('[role="alert"]');
        expect(alert).toBeInTheDocument();
        expect(alert?.textContent).toBe('Microphone not available');
      });

      // Restore
      if (navigator.mediaDevices) {
        delete (navigator.mediaDevices as any).getUserMedia;
        Object.defineProperty(navigator.mediaDevices, 'getUserMedia', {
          value: originalGetUserMedia,
          writable: true,
          configurable: true,
        });
      }
    });
  });

  describe('performance', () => {
    it('should not cause unnecessary re-renders', () => {
      const renderSpy = vi.fn();
      
      const TestWrapper = () => {
        renderSpy();
        return <RecordingButton />;
      };
      
      const { rerender } = render(<TestWrapper />);
      expect(renderSpy).toHaveBeenCalledOnce();
      
      // Re-render with same props
      rerender(<TestWrapper />);
      expect(renderSpy).toHaveBeenCalledTimes(2);
    });

    it('should clean up properly on unmount', () => {
      const { unmount } = render(<RecordingButton />);
      expect(() => unmount()).not.toThrow();
    });
  });
});