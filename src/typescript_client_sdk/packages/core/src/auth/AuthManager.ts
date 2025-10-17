/**
 * Authentication manager for the Agent C Realtime SDK
 * Handles login, token management, and automatic refresh
 */

import { EventEmitter } from '../events/EventEmitter';
import { 
  LoginResponse, 
  RefreshTokenResponse,
  User
} from '../events/types/CommonTypes';
import {
  AuthConfig,
  AuthState,
  LoginCredentials,
  TokenPair,
  JWTPayload,
  DEFAULT_AUTH_CONFIG,
  MemoryTokenStorage
} from './AuthConfig';

/**
 * Events emitted by the AuthManager
 */
export interface AuthManagerEvents {
  'auth:login': LoginResponse;
  'auth:logout': void;
  'auth:tokens-refreshed': TokenPair;
  'auth:state-changed': AuthState;
  'auth:error': Error;
}

/**
 * Authentication manager for handling Agent C API authentication
 */
export class AuthManager extends EventEmitter<AuthManagerEvents> {
  private config: Required<AuthConfig>;
  private state: AuthState;
  private refreshTimer?: ReturnType<typeof setTimeout>;

  constructor(config: AuthConfig) {
    super();
    
    // Merge with defaults
    this.config = {
      ...DEFAULT_AUTH_CONFIG,
      ...config,
      storage: config.storage || new MemoryTokenStorage(),
      fetch: config.fetch || (typeof fetch !== 'undefined' ? fetch.bind(globalThis) : (() => {
        throw new Error('Fetch is not available. Please provide a fetch implementation.');
      })()),
    } as Required<AuthConfig>;

    // Initialize state
    this.state = {
      isAuthenticated: false,
      tokens: null,
      user: null,
      uiSessionId: null,
      wsUrl: null,
      isAuthenticating: false,
      isRefreshing: false,
      error: null,
    };

    // Load tokens from storage on initialization
    this.loadStoredTokens();
  }

  /**
   * Get current authentication state
   */
  getState(): AuthState {
    return { ...this.state };
  }

  /**
   * Get current authentication tokens
   */
  getTokens(): TokenPair | null {
    return this.state.tokens;
  }

  /**
   * Get current Agent C JWT token
   */
  getAgentCToken(): string | null {
    return this.state.tokens?.agentCToken || null;
  }

  /**
   * Get current HeyGen access token
   */
  getHeygenToken(): string | null {
    return this.state.tokens?.heygenToken || null;
  }

  /**
   * Get current user information
   */
  getUser(): User | null {
    return this.state.user;
  }

  /**
   * Get UI session ID for reconnection
   */
  getUiSessionId(): string | null {
    return this.state.uiSessionId;
  }

  /**
   * Get WebSocket URL for realtime connection
   * Constructs the URL from the base API URL
   */
  getWebSocketUrl(): string | null {
    if (!this.config.apiUrl) {
      return null;
    }
    
    // Convert HTTP(S) URL to WebSocket URL
    const url = new URL(this.config.apiUrl);
    url.protocol = url.protocol === 'https:' ? 'wss:' : 'ws:';
    url.pathname = '/rt/ws';
    
    return url.toString();
  }



  /**
   * Check if currently authenticated
   */
  isAuthenticated(): boolean {
    return this.state.isAuthenticated && !!this.state.tokens;
  }

  /**
   * Load stored tokens from storage adapter
   */
  private async loadStoredTokens(): Promise<void> {
    try {
      const tokens = await this.config.storage.getTokens();
      if (tokens && !this.isTokenExpired(tokens)) {
        this.updateState({
          isAuthenticated: true,
          tokens,
        });
        this.scheduleTokenRefresh(tokens);
      } else if (tokens) {
        // Clear expired tokens
        await this.config.storage.clearTokens();
      }
    } catch (error: unknown) {
      console.error('Failed to load stored tokens:', error);
    }
  }

  /**
   * Login with username and password
   */
  async login(credentials: LoginCredentials): Promise<LoginResponse> {
    if (this.state.isAuthenticating) {
      throw new Error('Authentication already in progress');
    }

    this.updateState({ isAuthenticating: true, error: null });

    try {
      const response = await this.config.fetch(`${this.config.apiUrl}/rt/login`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(credentials),
      });

      if (!response.ok) {
        const errorText = await response.text().catch(() => 'Login failed');
        throw new Error(`Authentication failed: ${response.status} - ${errorText}`);
      }

      const data: LoginResponse = await response.json();
      
      // Parse JWT to get expiry
      const expiresAt = this.parseJWTExpiry(data.agent_c_token);
      
      // Create token pair
      const tokens: TokenPair = {
        agentCToken: data.agent_c_token,
        heygenToken: data.heygen_token,
        expiresAt,
      };

      // Store tokens
      await this.config.storage.setTokens(tokens);

      // Update state - note that user data will come from WebSocket events
      this.updateState({
        isAuthenticated: true,
        tokens,
        user: null, // User data will come from chat_user_data event
        uiSessionId: data.ui_session_id,
        isAuthenticating: false,
      });

      // Schedule refresh if enabled
      if (this.config.autoRefresh) {
        this.scheduleTokenRefresh(tokens);
      }

      // Emit events
      this.emit('auth:login', data);

      return data;
    } catch (error: unknown) {
      const authError = error instanceof Error ? error : new Error('Authentication failed');
      this.updateState({
        isAuthenticating: false,
        error: authError,
      });
      this.emit('auth:error', authError);
      this.config.onAuthError?.(authError);
      throw authError;
    }
  }

  /**
   * Refresh authentication tokens
   */
  async refreshTokens(): Promise<TokenPair> {
    if (!this.state.tokens) {
      throw new Error('No tokens to refresh');
    }

    if (this.state.isRefreshing) {
      throw new Error('Token refresh already in progress');
    }

    this.updateState({ isRefreshing: true, error: null });

    try {
      const response = await this.config.fetch(`${this.config.apiUrl}/rt/refresh_token`, {
        method: 'GET',
        headers: {
          'Authorization': `Bearer ${this.state.tokens.agentCToken}`,
        },
      });

      if (!response.ok) {
        const errorText = await response.text().catch(() => 'Token refresh failed');
        throw new Error(`Token refresh failed: ${response.status} - ${errorText}`);
      }

      const data: RefreshTokenResponse = await response.json();
      
      // Parse JWT to get expiry
      const expiresAt = this.parseJWTExpiry(data.agent_c_token);
      
      // Create new token pair
      const tokens: TokenPair = {
        agentCToken: data.agent_c_token,
        heygenToken: data.heygen_token,
        expiresAt,
      };

      // Store new tokens
      await this.config.storage.setTokens(tokens);

      // Update state
      this.updateState({
        tokens,
        isRefreshing: false,
      });

      // Schedule next refresh if enabled
      if (this.config.autoRefresh) {
        this.scheduleTokenRefresh(tokens);
      }

      // Emit events and callbacks
      this.emit('auth:tokens-refreshed', tokens);
      this.config.onTokensRefreshed?.(tokens);

      return tokens;
    } catch (error: unknown) {
      const refreshError = error instanceof Error ? error : new Error('Token refresh failed');
      this.updateState({
        isRefreshing: false,
        error: refreshError,
      });
      
      // If refresh fails, consider the user logged out
      if (refreshError.message.includes('401')) {
        await this.logout();
      }
      
      this.emit('auth:error', refreshError);
      this.config.onAuthError?.(refreshError);
      throw refreshError;
    }
  }

  /**
   * Logout and clear authentication state
   */
  async logout(): Promise<void> {
    // Cancel refresh timer
    if (this.refreshTimer) {
      clearTimeout(this.refreshTimer);
      this.refreshTimer = undefined;
    }

    // Clear stored tokens
    await this.config.storage.clearTokens();

    // Reset state
    this.updateState({
      isAuthenticated: false,
      tokens: null,
      user: null,
      uiSessionId: null,
      wsUrl: null,
      isAuthenticating: false,
      isRefreshing: false,
      error: null,
    });

    // Emit logout event
    this.emit('auth:logout', undefined);
  }

  /**
   * Initialize AuthManager from a login payload
   * Note: The simplified login response only contains tokens and session ID.
   * User data and configuration will come through WebSocket events.
   * 
   * @param payload - Login response from the backend (tokens and session ID only)
   * @param wsUrl - Optional WebSocket URL for the realtime connection
   */
  async initializeFromPayload(payload: LoginResponse, wsUrl?: string): Promise<void> {
    try {
      // Parse JWT to get expiry
      const expiresAt = this.parseJWTExpiry(payload.agent_c_token);
      
      // Create token pair
      const tokens: TokenPair = {
        agentCToken: payload.agent_c_token,
        heygenToken: payload.heygen_token,
        expiresAt,
      };

      // Store tokens
      await this.config.storage.setTokens(tokens);

      // Update state - user data will come from WebSocket events
      this.updateState({
        isAuthenticated: true,
        tokens,
        user: null,  // Will be populated by chat_user_data event
        uiSessionId: payload.ui_session_id,
        wsUrl: wsUrl || null,  // Store the WebSocket URL if provided
        isAuthenticating: false,
        isRefreshing: false,
        error: null,
      });

      // Schedule refresh if enabled
      if (this.config.autoRefresh) {
        this.scheduleTokenRefresh(tokens);
      }

      // Emit login event with the payload
      this.emit('auth:login', payload);
      
    } catch (error: unknown) {
      const authError = error instanceof Error ? error : new Error('Failed to initialize from payload');
      this.updateState({
        isAuthenticating: false,
        error: authError,
      });
      this.emit('auth:error', authError);
      this.config.onAuthError?.(authError);
      throw authError;
    }
  }

  /**
   * Set tokens directly (useful for SSO or external auth)
   */
  async setTokens(tokens: TokenPair): Promise<void> {
    // Add expiry if not provided
    if (!tokens.expiresAt) {
      tokens.expiresAt = this.parseJWTExpiry(tokens.agentCToken);
    }

    // Store tokens
    await this.config.storage.setTokens(tokens);

    // Update state
    this.updateState({
      isAuthenticated: true,
      tokens,
    });

    // Schedule refresh if enabled
    if (this.config.autoRefresh) {
      this.scheduleTokenRefresh(tokens);
    }
  }

  /**
   * Parse JWT token to extract expiry time
   */
  private parseJWTExpiry(token: string): number | undefined {
    try {
      // JWT format: header.payload.signature
      const parts = token.split('.');
      if (parts.length !== 3) {
        return undefined;
      }

      // Decode base64url payload
      const payload = parts[1];
      if (!payload) {
        return undefined;
      }
      const decoded = atob(payload.replace(/-/g, '+').replace(/_/g, '/'));
      const parsed = JSON.parse(decoded) as JWTPayload;

      // Convert exp (seconds) to milliseconds
      return parsed.exp ? parsed.exp * 1000 : undefined;
    } catch (error: unknown) {
      console.error('Failed to parse JWT expiry:', error);
      return undefined;
    }
  }

  /**
   * Check if token is expired or about to expire
   */
  private isTokenExpired(tokens: TokenPair): boolean {
    if (!tokens.expiresAt) {
      return false; // Can't determine, assume valid
    }

    const now = Date.now();
    const bufferMs = this.config.refreshBufferMs;
    return tokens.expiresAt <= now + bufferMs;
  }

  /**
   * Schedule automatic token refresh
   */
  private scheduleTokenRefresh(tokens: TokenPair): void {
    // Cancel existing timer
    if (this.refreshTimer) {
      clearTimeout(this.refreshTimer);
    }

    if (!tokens.expiresAt || !this.config.autoRefresh) {
      return;
    }

    // Calculate when to refresh (with buffer)
    const now = Date.now();
    const refreshAt = tokens.expiresAt - this.config.refreshBufferMs;
    const delay = Math.max(0, refreshAt - now);

    // Don't schedule if token is already expired
    if (delay === 0 && this.isTokenExpired(tokens)) {
      // Token is already expired, try to refresh immediately
      this.refreshTokens().catch((error: unknown) => {
        console.error('Failed to refresh expired token:', error);
      });
      return;
    }

    // Schedule refresh
    this.refreshTimer = setTimeout(() => {
      this.refreshTokens().catch((error: unknown) => {
        console.error('Scheduled token refresh failed:', error);
      });
    }, delay);
  }

  /**
   * Update internal state and emit change event
   */
  private updateState(partial: Partial<AuthState>): void {
    this.state = {
      ...this.state,
      ...partial,
    };
    this.emit('auth:state-changed', this.getState());
  }

  /**
   * Clean up resources
   */
  destroy(): void {
    if (this.refreshTimer) {
      clearTimeout(this.refreshTimer);
      this.refreshTimer = undefined;
    }
    this.removeAllListeners();
  }
}