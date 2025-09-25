/**
 * Simplified tests for the Comparison View focusing on isolated functionality
 * Avoids React concurrent mode issues by testing components in isolation
 */

import React from 'react';
import { describe, it, expect, beforeEach, vi } from 'vitest';
import { render, screen, waitFor } from '@testing-library/react';
import userEvent from '@testing-library/user-event';

// Mock modules
vi.mock('next/navigation', () => ({
  useRouter: vi.fn(() => ({
    push: vi.fn(),
    replace: vi.fn(),
    refresh: vi.fn(),
  })),
  useSearchParams: () => new URLSearchParams(),
  usePathname: () => '/comparison',
}));

vi.mock('@/contexts/auth-context', () => ({
  AuthProvider: ({ children }: { children: React.ReactNode }) => <>{children}</>,
  useAuth: () => ({
    isAuthenticated: true,
    isLoading: false,
    user: { id: 'test-user', name: 'Test User' },
    getAuthToken: () => 'test-token'
  })
}));

vi.mock('@agentc/realtime-react', () => ({
  useChat: vi.fn(() => ({
    messages: [],
    sendMessage: vi.fn(),
    isConnected: false,
    connectionState: 'disconnected'
  })),
  useConnection: vi.fn(() => ({
    connect: vi.fn(),
    disconnect: vi.fn(),
    isConnected: false,
    connectionState: 'disconnected',
    error: null
  }))
}));

// Mock the panels to avoid complex rendering
vi.mock('@/components/comparison/StreamingPanel', () => ({
  StreamingPanel: ({ syncScroll }: { syncScroll: boolean }) => (
    <div data-testid="streaming-panel">
      Streaming Panel (sync: {syncScroll ? 'on' : 'off'})
    </div>
  )
}));

vi.mock('@/components/comparison/ResumedPanel', () => ({
  ResumedPanel: ({ sessionData, syncScroll }: any) => (
    <div data-testid="resumed-panel">
      Resumed Panel (sync: {syncScroll ? 'on' : 'off'})
      {sessionData && <span>Session: {sessionData.session_id}</span>}
    </div>
  )
}));

// Direct import of ComparisonContent without auth wrapper for isolated testing
import { ComparisonContent } from '@/components/comparison/ComparisonPageClient';

describe('Comparison View - Simplified Tests', () => {
  beforeEach(() => {
    vi.clearAllMocks();
    // Reset fetch mock
    global.fetch = vi.fn();
  });

  describe('Component Structure', () => {
    it('should render the comparison view header', async () => {
      global.fetch = vi.fn(() => 
        Promise.resolve(
          new Response(JSON.stringify({
            session_id: 'test-session',
            messages: []
          }), {
            status: 200,
            headers: { 'Content-Type': 'application/json' }
          })
        )
      );

      render(<ComparisonContent />);

      expect(screen.getByText('Session Comparison View')).toBeTruthy();
      expect(screen.getByText(/Validate that resumed sessions/)).toBeTruthy();
    });

    it('should render both panels', async () => {
      global.fetch = vi.fn(() => 
        Promise.resolve(
          new Response(JSON.stringify({
            session_id: 'test-session',
            messages: []
          }), {
            status: 200,
            headers: { 'Content-Type': 'application/json' }
          })
        )
      );

      render(<ComparisonContent />);

      expect(screen.getByText('Streaming Session')).toBeTruthy();
      expect(screen.getByText('Resumed Session')).toBeTruthy();
    });

    it('should render validation checklist', async () => {
      global.fetch = vi.fn(() => 
        Promise.resolve(
          new Response(JSON.stringify({
            session_id: 'test-session',
            messages: []
          }), {
            status: 200,
            headers: { 'Content-Type': 'application/json' }
          })
        )
      );

      render(<ComparisonContent />);

      const validationItems = [
        'Think tools render as gray bubbles',
        'Delegation shows subsession dividers',
        'Roles preserved correctly',
        'Visual appearance identical'
      ];

      validationItems.forEach(item => {
        expect(screen.getByText(item)).toBeTruthy();
      });
    });
  });

  describe('Session Loading', () => {
    it('should load session data on mount', async () => {
      const mockSessionData = {
        session_id: 'test-123',
        messages: [
          { role: 'user', content: 'Test message' }
        ]
      };

      global.fetch = vi.fn(() => 
        Promise.resolve(
          new Response(JSON.stringify(mockSessionData), {
            status: 200,
            headers: { 'Content-Type': 'application/json' }
          })
        )
      );

      render(<ComparisonContent />);

      // Wait for session to load
      await waitFor(() => {
        expect(screen.getByText(/Session: test-123/)).toBeTruthy();
      });

      expect(global.fetch).toHaveBeenCalledWith('/api/load-test-session', expect.any(Object));
    });

    it('should show loading state during fetch', async () => {
      let resolvePromise: any;
      const fetchPromise = new Promise((resolve) => {
        resolvePromise = resolve;
      });

      global.fetch = vi.fn(() => fetchPromise);

      render(<ComparisonContent />);

      // Click reload button to trigger loading state
      const reloadButton = screen.getByText('Reload Session');
      await userEvent.click(reloadButton);

      expect(screen.getByText('Loading...')).toBeTruthy();

      // Resolve the promise
      resolvePromise(
        new Response(JSON.stringify({
          session_id: 'loaded',
          messages: []
        }), {
          status: 200,
          headers: { 'Content-Type': 'application/json' }
        })
      );

      await waitFor(() => {
        expect(screen.queryByText('Loading...')).toBeFalsy();
        expect(screen.getByText('Reload Session')).toBeTruthy();
      });
    });

    it('should handle session loading errors', async () => {
      global.fetch = vi.fn(() => 
        Promise.resolve(
          new Response('Not Found', {
            status: 404,
            statusText: 'Not Found'
          })
        )
      );

      render(<ComparisonContent />);

      await waitFor(() => {
        expect(screen.getByText(/Failed to load session: Not Found/)).toBeTruthy();
      });
    });

    it('should handle network errors', async () => {
      global.fetch = vi.fn(() => 
        Promise.reject(new Error('Network failure'))
      );

      render(<ComparisonContent />);

      await waitFor(() => {
        expect(screen.getByText(/Network failure/)).toBeTruthy();
      });
    });
  });

  describe('User Interactions', () => {
    it('should toggle sync scroll setting', async () => {
      global.fetch = vi.fn(() => 
        Promise.resolve(
          new Response(JSON.stringify({
            session_id: 'test',
            messages: []
          }), {
            status: 200,
            headers: { 'Content-Type': 'application/json' }
          })
        )
      );

      render(<ComparisonContent />);

      const syncCheckbox = screen.getByLabelText('Sync Scroll');
      expect((syncCheckbox as HTMLInputElement).checked).toBe(true);

      await userEvent.click(syncCheckbox);
      expect((syncCheckbox as HTMLInputElement).checked).toBe(false);

      // Verify panels receive the updated prop
      expect(screen.getByText(/sync: off/)).toBeTruthy();
    });

    it('should reload session when button clicked', async () => {
      global.fetch = vi.fn(() => 
        Promise.resolve(
          new Response(JSON.stringify({
            session_id: 'initial',
            messages: []
          }), {
            status: 200,
            headers: { 'Content-Type': 'application/json' }
          })
        )
      );

      render(<ComparisonContent />);

      await waitFor(() => {
        expect(screen.getByText(/Session: initial/)).toBeTruthy();
      });

      // Update mock for reload
      global.fetch = vi.fn(() => 
        Promise.resolve(
          new Response(JSON.stringify({
            session_id: 'reloaded',
            messages: []
          }), {
            status: 200,
            headers: { 'Content-Type': 'application/json' }
          })
        )
      );

      const reloadButton = screen.getByText('Reload Session');
      await userEvent.click(reloadButton);

      await waitFor(() => {
        expect(screen.getByText(/Session: reloaded/)).toBeTruthy();
      });
    });
  });

  describe('Session Data Processing', () => {
    it('should handle messages with tool_use content', async () => {
      const sessionWithTools = {
        session_id: 'tool-session',
        messages: [
          {
            role: 'assistant',
            content: [
              {
                id: 'tool_1',
                name: 'think',
                type: 'tool_use',
                input: { thought: 'Processing...' }
              }
            ]
          }
        ]
      };

      global.fetch = vi.fn(() => 
        Promise.resolve(
          new Response(JSON.stringify(sessionWithTools), {
            status: 200,
            headers: { 'Content-Type': 'application/json' }
          })
        )
      );

      render(<ComparisonContent />);

      await waitFor(() => {
        expect(screen.getByText(/Session: tool-session/)).toBeTruthy();
      });
    });

    it('should handle delegation messages', async () => {
      const sessionWithDelegation = {
        session_id: 'delegation-session',
        messages: [
          {
            role: 'assistant',
            content: [
              {
                id: 'tool_2',
                name: 'ateam_chat',
                type: 'tool_use',
                input: {
                  agent_key: 'helper_agent',
                  message: 'Please help'
                }
              }
            ]
          }
        ]
      };

      global.fetch = vi.fn(() => 
        Promise.resolve(
          new Response(JSON.stringify(sessionWithDelegation), {
            status: 200,
            headers: { 'Content-Type': 'application/json' }
          })
        )
      );

      render(<ComparisonContent />);

      await waitFor(() => {
        expect(screen.getByText(/Session: delegation-session/)).toBeTruthy();
      });
    });
  });

  describe('Error Recovery', () => {
    it('should allow retry after error', async () => {
      // First load fails
      global.fetch = vi.fn(() => 
        Promise.reject(new Error('Initial failure'))
      );

      render(<ComparisonContent />);

      await waitFor(() => {
        expect(screen.getByText(/Initial failure/)).toBeTruthy();
      });

      // Fix the mock for retry
      global.fetch = vi.fn(() => 
        Promise.resolve(
          new Response(JSON.stringify({
            session_id: 'recovered',
            messages: []
          }), {
            status: 200,
            headers: { 'Content-Type': 'application/json' }
          })
        )
      );

      const reloadButton = screen.getByText('Reload Session');
      await userEvent.click(reloadButton);

      await waitFor(() => {
        expect(screen.queryByText(/Initial failure/)).toBeFalsy();
        expect(screen.getByText(/Session: recovered/)).toBeTruthy();
      });
    });

    it('should clear error when successful load occurs', async () => {
      // Start with error
      global.fetch = vi.fn(() => 
        Promise.reject(new Error('Error state'))
      );

      const { rerender } = render(<ComparisonContent />);

      await waitFor(() => {
        expect(screen.getByText(/Error state/)).toBeTruthy();
      });

      // Mock successful response
      global.fetch = vi.fn(() => 
        Promise.resolve(
          new Response(JSON.stringify({
            session_id: 'success',
            messages: []
          }), {
            status: 200,
            headers: { 'Content-Type': 'application/json' }
          })
        )
      );

      // Trigger reload
      const reloadButton = screen.getByText('Reload Session');
      await userEvent.click(reloadButton);

      await waitFor(() => {
        expect(screen.queryByText(/Error state/)).toBeFalsy();
        expect(screen.getByText(/Session: success/)).toBeTruthy();
      });
    });
  });
});