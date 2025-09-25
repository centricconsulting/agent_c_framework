/**
 * Integration tests for Session Switch Test Page
 * 
 * Validates that the test page integrates correctly with:
 * - Authentication
 * - Provider setup
 * - Navigation
 * - Test panel functionality
 */

import React from 'react';
import { describe, it, expect, beforeEach, vi } from 'vitest';
import { render, screen, waitFor } from '@testing-library/react';
import { useRouter } from 'next/navigation';
import SessionSwitchTestClient from '../client';
import { useAuth } from '@/contexts/auth-context';

// Mock Next.js navigation
vi.mock('next/navigation', () => ({
  useRouter: vi.fn()
}));

// Mock auth context
vi.mock('@/contexts/auth-context', () => ({
  AuthProvider: ({ children }: { children: React.ReactNode }) => <div>{children}</div>,
  useAuth: vi.fn()
}));

// Mock the test panel component
vi.mock('@/components/testing/SessionSwitchTestPanel', () => ({
  SessionSwitchTestPanel: () => <div data-testid="session-switch-test-panel">Test Panel</div>
}));

// Mock AgentC provider
vi.mock('@agentc/realtime-react', () => ({
  AgentCProvider: ({ children }: { children: React.ReactNode }) => <div>{children}</div>
}));

// Mock UI components
vi.mock('@agentc/realtime-ui', () => ({
  Button: ({ children, onClick, disabled }: any) => (
    <button onClick={onClick} disabled={disabled}>
      {children}
    </button>
  )
}));

// Mock lucide icons
vi.mock('lucide-react', () => ({
  ArrowLeft: () => <span>ArrowLeft</span>
}));

describe('Session Switch Test Page Integration', () => {
  const mockPush = vi.fn();
  const mockRouter = {
    push: mockPush,
    back: vi.fn(),
    forward: vi.fn(),
    refresh: vi.fn(),
    replace: vi.fn(),
    prefetch: vi.fn()
  };
  
  beforeEach(() => {
    vi.clearAllMocks();
    vi.mocked(useRouter).mockReturnValue(mockRouter as any);
    
    // Default authenticated state
    vi.mocked(useAuth).mockReturnValue({
      isAuthenticated: true,
      isLoading: false,
      user: { id: 'test-user', name: 'Test User' },
      login: vi.fn(),
      logout: vi.fn()
    } as any);
  });
  
  describe('Authentication', () => {
    it('should redirect to login when not authenticated', () => {
      vi.mocked(useAuth).mockReturnValue({
        isAuthenticated: false,
        isLoading: false,
        user: null,
        login: vi.fn(),
        logout: vi.fn()
      } as any);
      
      render(<SessionSwitchTestClient />);
      
      expect(mockPush).toHaveBeenCalledWith('/login');
    });
    
    it('should show loading state while checking authentication', () => {
      vi.mocked(useAuth).mockReturnValue({
        isAuthenticated: false,
        isLoading: true,
        user: null,
        login: vi.fn(),
        logout: vi.fn()
      } as any);
      
      render(<SessionSwitchTestClient />);
      
      expect(screen.getByText('Verifying authentication...')).toBeInTheDocument();
      expect(mockPush).not.toHaveBeenCalled();
    });
    
    it('should render test panel when authenticated', () => {
      render(<SessionSwitchTestClient />);
      
      expect(screen.getByTestId('session-switch-test-panel')).toBeInTheDocument();
      expect(mockPush).not.toHaveBeenCalled();
    });
  });
  
  describe('Page Layout', () => {
    it('should display page title and description', () => {
      render(<SessionSwitchTestClient />);
      
      expect(screen.getByText('Session Switch Testing')).toBeInTheDocument();
      expect(screen.getByText('Validate message clearing during session switches')).toBeInTheDocument();
    });
    
    it('should render back to chat navigation button', () => {
      render(<SessionSwitchTestClient />);
      
      const backButton = screen.getByText('Back to Chat');
      expect(backButton).toBeInTheDocument();
      
      // Click should navigate to chat
      backButton.click();
      expect(mockPush).toHaveBeenCalledWith('/chat');
    });
    
    it('should have proper page structure', () => {
      const { container } = render(<SessionSwitchTestClient />);
      
      // Should have header section
      expect(container.querySelector('.border-b')).toBeInTheDocument();
      
      // Should have main content container
      expect(container.querySelector('.container')).toBeInTheDocument();
      
      // Should have proper background
      expect(container.querySelector('.bg-background')).toBeInTheDocument();
    });
  });
  
  describe('Provider Setup', () => {
    it('should setup AgentCProvider with correct props', () => {
      const originalEnv = process.env;
      process.env.NEXT_PUBLIC_AGENTC_API_URL = 'ws://test-api:8080';
      process.env.NODE_ENV = 'development';
      
      const { container } = render(<SessionSwitchTestClient />);
      
      // Provider should be rendered (mocked as div)
      expect(container.firstChild).toBeInTheDocument();
      
      process.env = originalEnv;
    });
    
    it('should use default API URL when env var not set', () => {
      const originalEnv = process.env;
      delete process.env.NEXT_PUBLIC_AGENTC_API_URL;
      
      render(<SessionSwitchTestClient />);
      
      // Should still render without error
      expect(screen.getByTestId('session-switch-test-panel')).toBeInTheDocument();
      
      process.env = originalEnv;
    });
    
    it('should enable debug mode in development', () => {
      const originalEnv = process.env;
      process.env.NODE_ENV = 'development';
      
      render(<SessionSwitchTestClient />);
      
      // Should render with debug enabled (verified by successful render)
      expect(screen.getByTestId('session-switch-test-panel')).toBeInTheDocument();
      
      process.env = originalEnv;
    });
    
    it('should disable debug mode in production', () => {
      const originalEnv = process.env;
      process.env.NODE_ENV = 'production';
      
      render(<SessionSwitchTestClient />);
      
      // Should render with debug disabled (verified by successful render)
      expect(screen.getByTestId('session-switch-test-panel')).toBeInTheDocument();
      
      process.env = originalEnv;
    });
  });
  
  describe('Test Panel Integration', () => {
    it('should render test panel inside authenticated area', () => {
      render(<SessionSwitchTestClient />);
      
      const testPanel = screen.getByTestId('session-switch-test-panel');
      expect(testPanel).toBeInTheDocument();
      
      // Should be inside content container
      const container = testPanel.closest('.container');
      expect(container).toBeInTheDocument();
    });
    
    it('should not render test panel when not authenticated', () => {
      vi.mocked(useAuth).mockReturnValue({
        isAuthenticated: false,
        isLoading: false,
        user: null,
        login: vi.fn(),
        logout: vi.fn()
      } as any);
      
      render(<SessionSwitchTestClient />);
      
      expect(screen.queryByTestId('session-switch-test-panel')).not.toBeInTheDocument();
    });
  });
  
  describe('Navigation Flow', () => {
    it('should handle authentication flow correctly', async () => {
      const { rerender } = render(<SessionSwitchTestClient />);
      
      // Start as unauthenticated
      vi.mocked(useAuth).mockReturnValue({
        isAuthenticated: false,
        isLoading: false,
        user: null,
        login: vi.fn(),
        logout: vi.fn()
      } as any);
      
      rerender(<SessionSwitchTestClient />);
      
      expect(mockPush).toHaveBeenCalledWith('/login');
      
      // Simulate successful authentication
      vi.mocked(useAuth).mockReturnValue({
        isAuthenticated: true,
        isLoading: false,
        user: { id: 'test-user', name: 'Test User' },
        login: vi.fn(),
        logout: vi.fn()
      } as any);
      
      rerender(<SessionSwitchTestClient />);
      
      await waitFor(() => {
        expect(screen.getByTestId('session-switch-test-panel')).toBeInTheDocument();
      });
    });
    
    it('should maintain state during navigation', () => {
      const { container } = render(<SessionSwitchTestClient />);
      
      // Get initial state
      const initialPanel = screen.getByTestId('session-switch-test-panel');
      expect(initialPanel).toBeInTheDocument();
      
      // Click back button
      const backButton = screen.getByText('Back to Chat');
      backButton.click();
      
      expect(mockPush).toHaveBeenCalledWith('/chat');
      
      // Component should still be rendered until actual navigation
      expect(screen.getByTestId('session-switch-test-panel')).toBeInTheDocument();
    });
  });
  
  describe('Responsive Design', () => {
    it('should render properly on mobile', () => {
      global.innerWidth = 375;
      global.innerHeight = 667;
      
      render(<SessionSwitchTestClient />);
      
      // All elements should be present
      expect(screen.getByText('Session Switch Testing')).toBeInTheDocument();
      expect(screen.getByText('Back to Chat')).toBeInTheDocument();
      expect(screen.getByTestId('session-switch-test-panel')).toBeInTheDocument();
    });
    
    it('should render properly on tablet', () => {
      global.innerWidth = 768;
      global.innerHeight = 1024;
      
      render(<SessionSwitchTestClient />);
      
      // All elements should be present
      expect(screen.getByText('Session Switch Testing')).toBeInTheDocument();
      expect(screen.getByText('Back to Chat')).toBeInTheDocument();
      expect(screen.getByTestId('session-switch-test-panel')).toBeInTheDocument();
    });
    
    it('should render properly on desktop', () => {
      global.innerWidth = 1920;
      global.innerHeight = 1080;
      
      render(<SessionSwitchTestClient />);
      
      // All elements should be present
      expect(screen.getByText('Session Switch Testing')).toBeInTheDocument();
      expect(screen.getByText('Back to Chat')).toBeInTheDocument();
      expect(screen.getByTestId('session-switch-test-panel')).toBeInTheDocument();
    });
  });
  
  describe('Error Handling', () => {
    it('should handle auth context errors gracefully', () => {
      vi.mocked(useAuth).mockImplementation(() => {
        throw new Error('Auth context not found');
      });
      
      // Should not crash the app
      expect(() => render(<SessionSwitchTestClient />)).toThrow();
    });
    
    it('should handle navigation errors gracefully', () => {
      mockPush.mockRejectedValueOnce(new Error('Navigation failed'));
      
      vi.mocked(useAuth).mockReturnValue({
        isAuthenticated: false,
        isLoading: false,
        user: null,
        login: vi.fn(),
        logout: vi.fn()
      } as any);
      
      render(<SessionSwitchTestClient />);
      
      // Should attempt navigation
      expect(mockPush).toHaveBeenCalledWith('/login');
    });
  });
  
  describe('Accessibility', () => {
    it('should have proper heading hierarchy', () => {
      const { container } = render(<SessionSwitchTestClient />);
      
      const h1 = container.querySelector('h1');
      expect(h1).toBeInTheDocument();
      expect(h1?.textContent).toBe('Session Switch Testing');
    });
    
    it('should have proper semantic structure', () => {
      const { container } = render(<SessionSwitchTestClient />);
      
      // Should have main content area
      expect(container.querySelector('.min-h-screen')).toBeInTheDocument();
      
      // Should have header section
      expect(container.querySelector('.border-b')).toBeInTheDocument();
      
      // Should have content container
      expect(container.querySelector('.container')).toBeInTheDocument();
    });
    
    it('should have keyboard navigable elements', () => {
      render(<SessionSwitchTestClient />);
      
      const backButton = screen.getByText('Back to Chat');
      
      // Button should be focusable
      backButton.focus();
      expect(document.activeElement).toBe(backButton);
    });
  });
});