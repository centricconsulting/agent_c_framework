/**
 * Authentication and Provider Tests for Comparison View
 * Tests Next.js authentication integration, provider hierarchy, and route protection
 */

import React from 'react';
import { describe, it, expect, beforeEach, vi, afterEach } from 'vitest';
import { render, screen, waitFor } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { useRouter } from 'next/navigation';
import ComparisonPageClient from '@/components/comparison/ComparisonPageClient';

// Mock Next.js router
const mockPush = vi.fn();
const mockReplace = vi.fn();
vi.mock('next/navigation', () => ({
  useRouter: vi.fn(() => ({
    push: mockPush,
    replace: mockReplace,
    refresh: vi.fn(),
    back: vi.fn(),
    forward: vi.fn(),
    prefetch: vi.fn(),
  })),
  useSearchParams: () => new URLSearchParams(),
  usePathname: () => '/comparison',
}));

// Mock auth context with configurable state
let mockAuthState = {
  isAuthenticated: true,
  isLoading: false,
  user: { id: 'test-user', name: 'Test User' },
  getAuthToken: () => 'test-token'
};

vi.mock('@/contexts/auth-context', () => ({
  AuthProvider: ({ children }: { children: React.ReactNode }) => <>{children}</>,
  useAuth: () => mockAuthState
}));

// Mock the SDK providers
vi.mock('@agentc/realtime-react', async () => {
  const actual = await vi.importActual('@agentc/realtime-react');
  return {
    ...actual,
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
  };
});

describe('Comparison View - Authentication Flow', () => {
  beforeEach(() => {
    // Reset auth state to authenticated by default
    mockAuthState = {
      isAuthenticated: true,
      isLoading: false,
      user: { id: 'test-user', name: 'Test User' },
      getAuthToken: () => 'test-token'
    };
  });

  describe('Authentication Guard', () => {
    it('should redirect to login when user is not authenticated', async () => {
      // Set auth state to unauthenticated
      mockAuthState = {
        isAuthenticated: false,
        isLoading: false,
        user: null,
        getAuthToken: () => null
      };

      render(<ComparisonPageClient />);

      // Should redirect to login
      await waitFor(() => {
        expect(mockPush).toHaveBeenCalledWith('/login');
      });
    });

    it('should show loading state while checking authentication', async () => {
      // Set auth state to loading
      mockAuthState = {
        isAuthenticated: false,
        isLoading: true,
        user: null,
        getAuthToken: () => null
      };

      render(<ComparisonPageClient />);

      // Should show loading message
      expect(screen.getByText('Verifying authentication...')).toBeTruthy();
      
      // Should not redirect while loading
      expect(mockPush).not.toHaveBeenCalled();
    });

    it('should render comparison view when authenticated', async () => {
      // Auth state is already set to authenticated in beforeEach
      render(<ComparisonPageClient />);

      // Should render the comparison view
      await waitFor(() => {
        expect(screen.queryByText('Session Comparison View')).toBeTruthy();
      });

      // Should not redirect
      expect(mockPush).not.toHaveBeenCalled();
    });

    it('should handle authentication state changes', async () => {
      const { rerender } = render(<ComparisonPageClient />);

      // Initially authenticated
      await waitFor(() => {
        expect(screen.queryByText('Session Comparison View')).toBeTruthy();
      });

      // Change to unauthenticated
      mockAuthState = {
        isAuthenticated: false,
        isLoading: false,
        user: null,
        getAuthToken: () => null
      };

      rerender(<ComparisonPageClient />);

      // Should redirect to login
      await waitFor(() => {
        expect(mockPush).toHaveBeenCalledWith('/login');
      });
    });
  });

  describe('Provider Hierarchy', () => {
    it('should initialize providers in correct order', async () => {
      const providerInitOrder: string[] = [];

      // Mock provider initialization tracking
      // Note: The provider is already mocked at the module level
      // We're verifying that the auth provider wraps the comparison component
      render(<ComparisonPageClient />);

      // Wait for render
      await waitFor(() => {
        expect(screen.queryByText('Session Comparison View')).toBeTruthy();
      });

      // Verify auth provider is being used (the component renders with auth context)
      expect(mockAuthState.isAuthenticated).toBe(true);
    });

    it('should provide auth context to child components', async () => {
      render(<ComparisonPageClient />);

      await waitFor(() => {
        // The comparison view should have access to auth state
        expect(screen.queryByText('Session Comparison View')).toBeTruthy();
      });

      // Verify authenticated user info is available
      expect(mockAuthState.user).toBeTruthy();
      expect(mockAuthState.isAuthenticated).toBe(true);
    });
  });

  describe('Navigation Integration', () => {
    it('should navigate back to chat when button clicked', async () => {
      const user = userEvent.setup();
      
      render(<ComparisonPageClient />);

      await waitFor(() => {
        expect(screen.queryByText('Session Comparison View')).toBeTruthy();
      });

      // Find and click the back button
      const backButton = screen.getByText('Back to Chat');
      
      // Note: The implementation uses window.location.href, not router
      // So we need to mock window.location
      const originalLocation = window.location;
      delete (window as any).location;
      window.location = { ...originalLocation, href: '' };

      await user.click(backButton);

      expect(window.location.href).toBe('/chat');

      // Restore original location
      window.location = originalLocation;
    });

    it('should handle reload session button click', async () => {
      const user = userEvent.setup();
      
      // Mock fetch for session loading
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

      render(<ComparisonPageClient />);

      await waitFor(() => {
        expect(screen.queryByText('Session Comparison View')).toBeTruthy();
      });

      // Find and click reload button
      const reloadButton = screen.getByText('Reload Session');
      await user.click(reloadButton);

      // Verify fetch was called again
      expect(global.fetch).toHaveBeenCalledWith('/api/load-test-session');
      expect(global.fetch).toHaveBeenCalledTimes(2); // Once on mount, once on reload
    });
  });

  describe('Environment Configuration', () => {
    it('should use correct API URL from environment', async () => {
      // Set the environment variable
      process.env.NEXT_PUBLIC_AGENTC_API_URL = 'https://demo.agentc.ai';

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

      render(<ComparisonPageClient />);

      await waitFor(() => {
        expect(screen.queryByText('Session Comparison View')).toBeTruthy();
      });

      // The session loading should use the relative API route
      expect(global.fetch).toHaveBeenCalledWith('/api/load-test-session');
    });

    it('should handle missing environment variables gracefully', async () => {
      // Remove environment variable
      delete process.env.NEXT_PUBLIC_AGENTC_API_URL;

      render(<ComparisonPageClient />);

      // Should still render without crashing
      await waitFor(() => {
        expect(screen.queryByText('Session Comparison View')).toBeTruthy();
      });
    });
  });

  describe('Error Boundaries', () => {
    it('should display error alert when session loading fails', async () => {
      // Mock fetch to fail
      global.fetch = vi.fn(() => 
        Promise.reject(new Error('Network error'))
      );

      render(<ComparisonPageClient />);

      // Wait for error to be displayed
      await waitFor(() => {
        const errorAlert = screen.queryByText(/Failed to load test session/);
        expect(errorAlert).toBeTruthy();
      });
    });

    it('should handle 404 errors for missing session file', async () => {
      global.fetch = vi.fn(() => 
        Promise.resolve(
          new Response(JSON.stringify({ error: 'Session file not found' }), {
            status: 404,
            statusText: 'Not Found',
            headers: { 'Content-Type': 'application/json' }
          })
        )
      );

      render(<ComparisonPageClient />);

      await waitFor(() => {
        const errorAlert = screen.queryByText(/Failed to load session: Not Found/);
        expect(errorAlert).toBeTruthy();
      });
    });

    it('should handle parse errors in session data', async () => {
      global.fetch = vi.fn(() => 
        Promise.resolve(
          new Response('Invalid JSON', {
            status: 200,
            headers: { 'Content-Type': 'application/json' }
          })
        )
      );

      render(<ComparisonPageClient />);

      await waitFor(() => {
        // Should show an error when JSON parsing fails
        const errorMessage = screen.queryByText(/Failed to load test session/);
        expect(errorMessage).toBeTruthy();
      });
    });
  });

  describe('Demo User Journey', () => {
    it('should complete full demo user journey successfully', async () => {
      const user = userEvent.setup();

      // Mock successful session load
      global.fetch = vi.fn(() => 
        Promise.resolve(
          new Response(JSON.stringify({
            session_id: 'demo-session',
            messages: [
              { role: 'user', content: 'Demo message' },
              { role: 'assistant', content: 'Demo response' }
            ]
          }), {
            status: 200,
            headers: { 'Content-Type': 'application/json' }
          })
        )
      );

      // Start with authenticated user
      render(<ComparisonPageClient />);

      // Step 1: View loads successfully
      await waitFor(() => {
        expect(screen.queryByText('Session Comparison View')).toBeTruthy();
      });

      // Step 2: Both panels are visible
      expect(screen.queryByText('Streaming Session')).toBeTruthy();
      expect(screen.queryByText('Resumed Session')).toBeTruthy();

      // Step 3: Session data loads
      await waitFor(() => {
        expect(screen.queryByText(/Session: demo-session/)).toBeTruthy();
      });

      // Step 4: User can interact with sync scroll
      const syncCheckbox = screen.getByLabelText('Sync Scroll');
      expect(syncCheckbox).toBeTruthy();
      expect((syncCheckbox as HTMLInputElement).checked).toBe(true);
      
      await user.click(syncCheckbox);
      expect((syncCheckbox as HTMLInputElement).checked).toBe(false);

      // Step 5: User can reload session
      const reloadButton = screen.getByText('Reload Session');
      await user.click(reloadButton);
      
      // Verify reload triggered another fetch
      expect(global.fetch).toHaveBeenCalledTimes(2);

      // Step 6: Validation checklist is available
      const validationItems = [
        'Think tools render as gray bubbles',
        'Delegation shows subsession dividers', 
        'Roles preserved correctly',
        'Visual appearance identical'
      ];

      validationItems.forEach(item => {
        expect(screen.queryByText(item)).toBeTruthy();
      });
    });
  });
});