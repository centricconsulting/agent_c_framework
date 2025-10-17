/**
 * Unit tests for AuthManager
 * Tests simple getters, setters, and authentication state management
 */

import { describe, it, expect, beforeEach, afterEach, vi } from 'vitest';
import { AuthManager } from '../AuthManager';
import type { AuthConfig, TokenPair, LoginCredentials } from '../AuthConfig';
import type { LoginResponse } from '../../events/types/CommonTypes';

describe('AuthManager', () => {
  let authManager: AuthManager;
  let mockFetch: ReturnType<typeof vi.fn>;
  let mockStorage: {
    getTokens: ReturnType<typeof vi.fn>;
    setTokens: ReturnType<typeof vi.fn>;
    clearTokens: ReturnType<typeof vi.fn>;
  };

  const mockConfig: AuthConfig = {
    apiUrl: 'https://api.example.com',
    autoRefresh: false, // Disable for simple tests
    refreshBufferMs: 300000,
  };

  const mockTokenPair: TokenPair = {
    agentCToken: 'mock-agentc-token',
    heygenToken: 'mock-heygen-token',
    expiresAt: Date.now() + 3600000, // 1 hour from now
  };

  const mockLoginResponse: LoginResponse = {
    agent_c_token: 'mock-agentc-token',
    heygen_token: 'mock-heygen-token',
    ui_session_id: 'mock-session-id',
  };

  beforeEach(() => {
    vi.clearAllMocks();
    
    // Mock fetch
    mockFetch = vi.fn();
    
    // Mock storage
    mockStorage = {
      getTokens: vi.fn().mockResolvedValue(null),
      setTokens: vi.fn().mockResolvedValue(undefined),
      clearTokens: vi.fn().mockResolvedValue(undefined),
    };
    
    // Create AuthManager with mocked dependencies
    authManager = new AuthManager({
      ...mockConfig,
      fetch: mockFetch as unknown as typeof fetch,
      storage: mockStorage,
    });
  });

  afterEach(() => {
    vi.restoreAllMocks();
    authManager.destroy();
  });

  describe('constructor', () => {
    it('should initialize with unauthenticated state', () => {
      expect(authManager.isAuthenticated()).toBe(false);
      expect(authManager.getTokens()).toBeNull();
      expect(authManager.getUser()).toBeNull();
    });

    it('should attempt to load stored tokens on initialization', () => {
      expect(mockStorage.getTokens).toHaveBeenCalled();
    });
  });

  describe('getTokens', () => {
    it('should return null when not authenticated', () => {
      expect(authManager.getTokens()).toBeNull();
    });

    it('should return token pair when authenticated', async () => {
      // Setup authenticated state
      await authManager.setTokens(mockTokenPair);
      
      const tokens = authManager.getTokens();
      expect(tokens).toEqual(mockTokenPair);
    });
  });

  describe('getAgentCToken', () => {
    it('should return null when not authenticated', () => {
      expect(authManager.getAgentCToken()).toBeNull();
    });

    it('should return agent C token when authenticated', async () => {
      await authManager.setTokens(mockTokenPair);
      
      expect(authManager.getAgentCToken()).toBe('mock-agentc-token');
    });
  });

  describe('getHeygenToken', () => {
    it('should return null when not authenticated', () => {
      expect(authManager.getHeygenToken()).toBeNull();
    });

    it('should return HeyGen token when authenticated', async () => {
      await authManager.setTokens(mockTokenPair);
      
      expect(authManager.getHeygenToken()).toBe('mock-heygen-token');
    });
  });

  describe('getUser', () => {
    it('should return null initially', () => {
      expect(authManager.getUser()).toBeNull();
    });
  });

  describe('getUiSessionId', () => {
    it('should return null when not logged in', () => {
      expect(authManager.getUiSessionId()).toBeNull();
    });

    it('should return session ID after login', async () => {
      mockFetch.mockResolvedValue({
        ok: true,
        json: vi.fn().mockResolvedValue(mockLoginResponse),
      });
      
      await authManager.login({ username: 'test', password: 'test' });
      
      expect(authManager.getUiSessionId()).toBe('mock-session-id');
    });
  });

  describe('getWebSocketUrl', () => {
    it('should construct WebSocket URL from API URL', () => {
      const wsUrl = authManager.getWebSocketUrl();
      expect(wsUrl).toBe('wss://api.example.com/rt/ws');
    });

    it('should handle http protocol', () => {
      authManager = new AuthManager({
        ...mockConfig,
        apiUrl: 'http://localhost:3000',
        fetch: mockFetch as unknown as typeof fetch,
        storage: mockStorage,
      });
      
      const wsUrl = authManager.getWebSocketUrl();
      expect(wsUrl).toBe('ws://localhost:3000/rt/ws');
    });
  });

  describe('isAuthenticated', () => {
    it('should return false when not authenticated', () => {
      expect(authManager.isAuthenticated()).toBe(false);
    });

    it('should return true when tokens are set', async () => {
      await authManager.setTokens(mockTokenPair);
      
      expect(authManager.isAuthenticated()).toBe(true);
    });
  });

  describe('login', () => {
    const credentials: LoginCredentials = {
      username: 'testuser',
      password: 'testpass',
    };

    it('should call fetch with correct parameters', async () => {
      mockFetch.mockResolvedValue({
        ok: true,
        json: vi.fn().mockResolvedValue(mockLoginResponse),
      });
      
      await authManager.login(credentials);
      
      expect(mockFetch).toHaveBeenCalledWith(
        'https://api.example.com/rt/login',
        {
          method: 'POST',
          headers: {
            'Content-Type': 'application/json',
          },
          body: JSON.stringify(credentials),
        }
      );
    });

    it('should store tokens after successful login', async () => {
      mockFetch.mockResolvedValue({
        ok: true,
        json: vi.fn().mockResolvedValue(mockLoginResponse),
      });
      
      await authManager.login(credentials);
      
      expect(mockStorage.setTokens).toHaveBeenCalledWith(
        expect.objectContaining({
          agentCToken: 'mock-agentc-token',
          heygenToken: 'mock-heygen-token',
        })
      );
    });

    it('should update authentication state after login', async () => {
      mockFetch.mockResolvedValue({
        ok: true,
        json: vi.fn().mockResolvedValue(mockLoginResponse),
      });
      
      await authManager.login(credentials);
      
      expect(authManager.isAuthenticated()).toBe(true);
      expect(authManager.getAgentCToken()).toBe('mock-agentc-token');
    });

    it('should emit auth:login event', async () => {
      mockFetch.mockResolvedValue({
        ok: true,
        json: vi.fn().mockResolvedValue(mockLoginResponse),
      });
      
      const listener = vi.fn();
      authManager.on('auth:login', listener);
      
      await authManager.login(credentials);
      
      expect(listener).toHaveBeenCalledWith(mockLoginResponse);
    });

    it('should throw error on failed login', async () => {
      mockFetch.mockResolvedValue({
        ok: false,
        status: 401,
        text: vi.fn().mockResolvedValue('Unauthorized'),
      });
      
      await expect(authManager.login(credentials)).rejects.toThrow(
        'Authentication failed: 401 - Unauthorized'
      );
    });
  });

  describe('logout', () => {
    beforeEach(async () => {
      // Set up authenticated state
      await authManager.setTokens(mockTokenPair);
    });

    it('should clear stored tokens', async () => {
      await authManager.logout();
      
      expect(mockStorage.clearTokens).toHaveBeenCalled();
    });

    it('should reset authentication state', async () => {
      expect(authManager.isAuthenticated()).toBe(true);
      
      await authManager.logout();
      
      expect(authManager.isAuthenticated()).toBe(false);
      expect(authManager.getTokens()).toBeNull();
      expect(authManager.getUser()).toBeNull();
    });

    it('should emit auth:logout event', async () => {
      const listener = vi.fn();
      authManager.on('auth:logout', listener);
      
      await authManager.logout();
      
      expect(listener).toHaveBeenCalled();
    });
  });

  describe('refreshTokens', () => {
    beforeEach(async () => {
      // Set up authenticated state
      await authManager.setTokens(mockTokenPair);
    });

    it('should call fetch with refresh token in header', async () => {
      const refreshResponse = {
        agent_c_token: 'new-agentc-token',
        heygen_token: 'new-heygen-token',
      };
      
      mockFetch.mockResolvedValue({
        ok: true,
        json: vi.fn().mockResolvedValue(refreshResponse),
      });
      
      await authManager.refreshTokens();
      
      expect(mockFetch).toHaveBeenCalledWith(
        'https://api.example.com/rt/refresh_token',
        {
          method: 'GET',
          headers: {
            'Authorization': 'Bearer mock-agentc-token',
          },
        }
      );
    });

    it('should update tokens after successful refresh', async () => {
      const refreshResponse = {
        agent_c_token: 'new-agentc-token',
        heygen_token: 'new-heygen-token',
      };
      
      mockFetch.mockResolvedValue({
        ok: true,
        json: vi.fn().mockResolvedValue(refreshResponse),
      });
      
      await authManager.refreshTokens();
      
      expect(mockStorage.setTokens).toHaveBeenCalledWith(
        expect.objectContaining({
          agentCToken: 'new-agentc-token',
          heygenToken: 'new-heygen-token',
        })
      );
      
      expect(authManager.getAgentCToken()).toBe('new-agentc-token');
    });

    it('should emit auth:tokens-refreshed event', async () => {
      const refreshResponse = {
        agent_c_token: 'new-agentc-token',
        heygen_token: 'new-heygen-token',
      };
      
      mockFetch.mockResolvedValue({
        ok: true,
        json: vi.fn().mockResolvedValue(refreshResponse),
      });
      
      const listener = vi.fn();
      authManager.on('auth:tokens-refreshed', listener);
      
      await authManager.refreshTokens();
      
      expect(listener).toHaveBeenCalledWith(
        expect.objectContaining({
          agentCToken: 'new-agentc-token',
          heygenToken: 'new-heygen-token',
        })
      );
    });

    it('should throw error when no tokens to refresh', async () => {
      authManager = new AuthManager({
        ...mockConfig,
        fetch: mockFetch as unknown as typeof fetch,
        storage: mockStorage,
      });
      
      await expect(authManager.refreshTokens()).rejects.toThrow(
        'No tokens to refresh'
      );
    });
  });

  describe('setTokens', () => {
    it('should store tokens in storage', async () => {
      await authManager.setTokens(mockTokenPair);
      
      expect(mockStorage.setTokens).toHaveBeenCalledWith(mockTokenPair);
    });

    it('should update authentication state', async () => {
      await authManager.setTokens(mockTokenPair);
      
      expect(authManager.isAuthenticated()).toBe(true);
      expect(authManager.getTokens()).toEqual(mockTokenPair);
    });
  });

  describe('initializeFromPayload', () => {
    it('should set tokens from login payload', async () => {
      await authManager.initializeFromPayload(mockLoginResponse);
      
      expect(mockStorage.setTokens).toHaveBeenCalledWith(
        expect.objectContaining({
          agentCToken: 'mock-agentc-token',
          heygenToken: 'mock-heygen-token',
        })
      );
    });

    it('should set UI session ID', async () => {
      await authManager.initializeFromPayload(mockLoginResponse);
      
      expect(authManager.getUiSessionId()).toBe('mock-session-id');
    });

    it('should emit auth:login event', async () => {
      const listener = vi.fn();
      authManager.on('auth:login', listener);
      
      await authManager.initializeFromPayload(mockLoginResponse);
      
      expect(listener).toHaveBeenCalledWith(mockLoginResponse);
    });

    it('should store WebSocket URL if provided', async () => {
      await authManager.initializeFromPayload(
        mockLoginResponse,
        'wss://custom.example.com/ws'
      );
      
      const state = authManager.getState();
      expect(state.wsUrl).toBe('wss://custom.example.com/ws');
    });
  });

  describe('getState', () => {
    it('should return current authentication state', () => {
      const state = authManager.getState();
      
      expect(state).toMatchObject({
        isAuthenticated: false,
        tokens: null,
        user: null,
        uiSessionId: null,
        wsUrl: null,
        isAuthenticating: false,
        isRefreshing: false,
        error: null,
      });
    });

    it('should return updated state after authentication', async () => {
      await authManager.setTokens(mockTokenPair);
      
      const state = authManager.getState();
      expect(state.isAuthenticated).toBe(true);
      expect(state.tokens).toEqual(mockTokenPair);
    });
  });
});