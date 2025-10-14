/**
 * @vitest-environment happy-dom
 */

import React from 'react';
import { describe, it, expect, vi, beforeEach, afterEach } from 'vitest';
import { render, screen, fireEvent, waitFor, act } from '@testing-library/react';
import { InputToolbar } from '../InputToolbar';
import { EventEmitter } from 'events';
import type { RealtimeClient } from '@agentc/realtime-core';

// Mock the react hooks
const mockClient = {
  cancelResponse: vi.fn(),
  getSessionManager: vi.fn(),
  on: vi.fn(),
  off: vi.fn()
} as unknown as RealtimeClient;

const mockSessionManager = new EventEmitter();

vi.mock('@agentc/realtime-react', () => ({
  useRealtimeClient: vi.fn(() => mockClient),
  useTurnState: vi.fn(() => ({ canSendInput: true })),
  useAgentCData: vi.fn(() => ({
    agents: [],
    voices: [],
    avatars: [],
    tools: []
  }))
}));

// Mock the child components to simplify testing
vi.mock('../MicrophoneButton', () => ({
  MicrophoneButton: vi.fn(({ isRecording, onStartRecording, onStopRecording, audioLevel }) => (
    <button
      data-testid="microphone-button"
      onClick={isRecording ? onStopRecording : onStartRecording}
    >
      {isRecording ? 'Recording' : 'Not Recording'} - Level: {audioLevel}
    </button>
  ))
}));

vi.mock('../../controls/OutputSelector', () => ({
  OutputSelector: vi.fn(() => <div data-testid="output-selector">Output Selector</div>)
}));

vi.mock('../../controls/AgentSelector', () => ({
  AgentSelector: vi.fn(() => <div data-testid="agent-selector">Agent Selector</div>)
}));

describe('InputToolbar', () => {
  const defaultProps = {
    onSend: vi.fn(),
    canSend: true,
    isRecording: false,
    onStartRecording: vi.fn(),
    onStopRecording: vi.fn(),
    audioLevel: 0
  };

  beforeEach(() => {
    vi.clearAllMocks();
    (mockClient.getSessionManager as ReturnType<typeof vi.fn>).mockReturnValue(mockSessionManager);
    // Clear all event listeners
    mockSessionManager.removeAllListeners();
  });

  afterEach(() => {
    vi.restoreAllMocks();
  });

  describe('Basic Rendering', () => {
    it('should render all toolbar components', () => {
      render(<InputToolbar {...defaultProps} />);
      
      // Check for main components
      expect(screen.getByTestId('microphone-button')).toBeDefined();
      expect(screen.getByTestId('output-selector')).toBeDefined();
      expect(screen.getByTestId('agent-selector')).toBeDefined();
      
      // Check for buttons
      expect(screen.getByLabelText('Add attachment')).toBeDefined();
      expect(screen.getByLabelText('Tools (Coming Soon)')).toBeDefined();
      expect(screen.getByLabelText('Send message')).toBeDefined();
    });

    it('should disable send button when canSend is false', () => {
      render(<InputToolbar {...defaultProps} canSend={false} />);
      
      const sendButton = screen.getByLabelText('Send message');
      expect(sendButton).toHaveProperty('disabled', true);
    });

    it('should show audio level indicator when recording', () => {
      render(<InputToolbar {...defaultProps} isRecording={true} audioLevel={50} />);
      
      // Check that the microphone button shows recording state
      expect(screen.getByText(/Recording - Level: 50/)).toBeDefined();
      
      // Check for audio level bars (5 bars total)
      const bars = screen.getByRole('img', { name: 'Audio level: 50%' });
      expect(bars).toBeDefined();
    });
  });

  describe('Cancel Response Functionality', () => {
    it('should show cancel button when agent is responding and cancelResponse is available', async () => {
      // Import the actual hook and mock it
      const { useTurnState } = await import('@agentc/realtime-react');
      (useTurnState as ReturnType<typeof vi.fn>).mockReturnValue({ canSendInput: false });
      
      // Ensure cancelResponse method exists on mock client
      (mockClient as any).cancelResponse = vi.fn();

      render(<InputToolbar {...defaultProps} />);
      
      const button = screen.getByRole('button', { name: 'Cancel response' });
      expect(button).toBeDefined();
      
      // Button should be destructive variant (red)
      expect(button.className).toContain('destructive');
      
      // Button should pulse
      expect(button.className).toContain('animate-pulse');
    });

    it('should call cancelResponse when cancel button is clicked', async () => {
      const { useTurnState } = await import('@agentc/realtime-react');
      (useTurnState as ReturnType<typeof vi.fn>).mockReturnValue({ canSendInput: false });
      
      // Ensure cancelResponse method exists on mock client
      (mockClient as any).cancelResponse = vi.fn();

      render(<InputToolbar {...defaultProps} />);
      
      const cancelButton = screen.getByRole('button', { name: 'Cancel response' });
      
      await act(async () => {
        fireEvent.click(cancelButton);
      });
      
      expect((mockClient as any).cancelResponse).toHaveBeenCalledTimes(1);
    });

    it('should stop pulsing after cancellation is confirmed', async () => {
      const { useTurnState } = await import('@agentc/realtime-react');
      (useTurnState as ReturnType<typeof vi.fn>).mockReturnValue({ canSendInput: false });
      
      // Ensure cancelResponse method exists on mock client
      (mockClient as any).cancelResponse = vi.fn();

      const { rerender } = render(<InputToolbar {...defaultProps} />);
      
      // Initially should be pulsing
      let button = screen.getByRole('button', { name: 'Cancel response' });
      expect(button.className).toContain('animate-pulse');
      
      // Emit response-cancelled event on the client mock
      await act(async () => {
        // Find the handler that was registered and call it
        const onCall = (mockClient.on as ReturnType<typeof vi.fn>).mock.calls.find(
          call => call[0] === 'response-cancelled'
        );
        if (onCall && onCall[1]) {
          onCall[1]({});
        }
      });
      
      // Re-render to see updated state
      rerender(<InputToolbar {...defaultProps} />);
      
      // Should still show cancel button but no longer pulsing
      button = screen.getByRole('button', { name: 'Cancel response' });
      expect(button.className).toContain('destructive');
      expect(button.className).not.toContain('animate-pulse');
    });

    it('should return to send button when user turn starts', async () => {
      const { useTurnState } = await import('@agentc/realtime-react');
      const mockUseTurnState = useTurnState as ReturnType<typeof vi.fn>;
      
      // Start with agent responding
      mockUseTurnState.mockReturnValue({ canSendInput: false });
      
      const { rerender } = render(<InputToolbar {...defaultProps} />);
      
      // Should show cancel button
      expect(screen.getByRole('button', { name: 'Cancel response' })).toBeDefined();
      
      // Change to user turn
      mockUseTurnState.mockReturnValue({ canSendInput: true });
      rerender(<InputToolbar {...defaultProps} />);
      
      // Should show send button
      expect(screen.getByRole('button', { name: 'Send message' })).toBeDefined();
      expect(screen.queryByRole('button', { name: 'Cancel response' })).toBeNull();
    });

    it('should call onSend when send button is clicked during user turn', async () => {
      const { useTurnState } = await import('@agentc/realtime-react');
      (useTurnState as ReturnType<typeof vi.fn>).mockReturnValue({ canSendInput: true });

      const onSend = vi.fn();
      render(<InputToolbar {...defaultProps} onSend={onSend} />);
      
      const sendButton = screen.getByRole('button', { name: 'Send message' });
      
      await act(async () => {
        fireEvent.click(sendButton);
      });
      
      expect(onSend).toHaveBeenCalledTimes(1);
      expect(mockClient.cancelResponse).not.toHaveBeenCalled();
    });
  });

  describe('Accessibility', () => {
    it('should have appropriate ARIA labels for all buttons', () => {
      render(<InputToolbar {...defaultProps} />);
      
      expect(screen.getByLabelText('Add attachment')).toBeDefined();
      expect(screen.getByLabelText('Tools (Coming Soon)')).toBeDefined();
      expect(screen.getByLabelText('Send message')).toBeDefined();
    });

    it('should update ARIA label when switching between send and cancel', async () => {
      const { useTurnState } = await import('@agentc/realtime-react');
      const mockUseTurnState = useTurnState as ReturnType<typeof vi.fn>;
      
      // Start with user turn
      mockUseTurnState.mockReturnValue({ canSendInput: true });
      const { rerender } = render(<InputToolbar {...defaultProps} />);
      
      expect(screen.getByLabelText('Send message')).toBeDefined();
      
      // Switch to agent turn
      mockUseTurnState.mockReturnValue({ canSendInput: false });
      rerender(<InputToolbar {...defaultProps} />);
      
      expect(screen.getByLabelText('Cancel response')).toBeDefined();
    });

    it('should properly indicate audio levels for screen readers', () => {
      render(<InputToolbar {...defaultProps} isRecording={true} audioLevel={75} />);
      
      const audioIndicator = screen.getByRole('img', { name: 'Audio level: 75%' });
      expect(audioIndicator).toBeDefined();
    });
  });

  describe('Edge Cases', () => {
    it('should handle missing session manager gracefully', () => {
      (mockClient.getSessionManager as ReturnType<typeof vi.fn>).mockReturnValue(null);
      
      // Should not throw
      expect(() => render(<InputToolbar {...defaultProps} />)).not.toThrow();
    });

    it('should handle missing client gracefully', async () => {
      const { useRealtimeClient } = await import('@agentc/realtime-react');
      (useRealtimeClient as ReturnType<typeof vi.fn>).mockReturnValue(null);
      
      // Should not throw
      expect(() => render(<InputToolbar {...defaultProps} />)).not.toThrow();
      
      // Click should not cause errors
      const sendButton = screen.getByRole('button', { name: 'Send message' });
      await act(async () => {
        fireEvent.click(sendButton);
      });
      
      // Should call onSend
      expect(defaultProps.onSend).toHaveBeenCalledTimes(1);
    });  
    
    it('should handle missing cancelResponse method gracefully', async () => {
      const { useTurnState } = await import('@agentc/realtime-react');
      (useTurnState as ReturnType<typeof vi.fn>).mockReturnValue({ canSendInput: false });
      
      // Remove cancelResponse from mock to simulate old SDK version
      delete (mockClient as any).cancelResponse;
      
      render(<InputToolbar {...defaultProps} />);
      
      // Should show normal send button since cancel is not supported
      expect(screen.getByRole('button', { name: 'Send message' })).toBeDefined();
      expect(screen.queryByRole('button', { name: 'Cancel response' })).toBeNull();
    });

    it('should reset cancelled state when switching back to user turn', async () => {
      const { useTurnState } = await import('@agentc/realtime-react');
      const mockUseTurnState = useTurnState as ReturnType<typeof vi.fn>;
      
      // Start with agent responding
      mockUseTurnState.mockReturnValue({ canSendInput: false });
      const { rerender } = render(<InputToolbar {...defaultProps} />);
      
      // Emit cancellation
      await act(async () => {
        mockSessionManager.emit('response-cancelled', {});
      });
      
      // Switch to user turn
      mockUseTurnState.mockReturnValue({ canSendInput: true });
      rerender(<InputToolbar {...defaultProps} />);
      
      // Should show normal send button (not cancelled state)
      const button = screen.getByRole('button', { name: 'Send message' });
      expect(button.className).not.toContain('destructive');
      expect(button.className).not.toContain('animate-pulse');
    });
  });
});