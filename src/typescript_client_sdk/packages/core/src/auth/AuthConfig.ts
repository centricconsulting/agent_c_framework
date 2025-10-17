/**
 * Authentication configuration and types for the Agent C Realtime SDK
 */

/**
 * Authentication configuration options
 */
export interface AuthConfig {
  /**
   * Base URL for the Agent C API (without /rt paths)
   * @example 'https://api.example.com'
   */
  apiUrl: string;

  /**
   * Optional custom fetch implementation (for testing or custom headers)
   */
  fetch?: typeof fetch;

  /**
   * Token refresh buffer time in milliseconds
   * Tokens will be refreshed this many ms before expiry
   * @default 60000 (1 minute)
   */
  refreshBufferMs?: number;

  /**
   * Whether to automatically refresh tokens before expiry
   * @default true
   */
  autoRefresh?: boolean;

  /**
   * Optional callback when tokens are refreshed
   */
  onTokensRefreshed?: (tokens: TokenPair) => void;

  /**
   * Optional callback when authentication fails
   */
  onAuthError?: (error: Error) => void;

  /**
   * Storage adapter for persisting tokens (optional)
   * If not provided, tokens are only stored in memory
   */
  storage?: TokenStorage;
}

/**
 * Token storage interface for persisting authentication tokens
 */
export interface TokenStorage {
  /**
   * Get stored tokens
   */
  getTokens(): Promise<TokenPair | null>;

  /**
   * Store tokens
   */
  setTokens(tokens: TokenPair): Promise<void>;

  /**
   * Clear stored tokens
   */
  clearTokens(): Promise<void>;
}

/**
 * Authentication tokens pair
 */
export interface TokenPair {
  /**
   * JWT token for Agent C API authentication
   */
  agentCToken: string;

  /**
   * Access token for HeyGen avatar sessions
   */
  heygenToken: string;

  /**
   * Token expiry time (Unix timestamp in milliseconds)
   * Calculated from JWT exp claim
   */
  expiresAt?: number;
}

/**
 * Login credentials
 */
export interface LoginCredentials {
  username: string;
  password: string;
}

/**
 * JWT payload structure
 */
export interface JWTPayload {
  user_id: string;
  permissions?: string[];
  exp: number; // Unix timestamp in seconds
  iat: number; // Unix timestamp in seconds
  [key: string]: unknown;
}

/**
 * Authentication state
 */
export interface AuthState {
  /**
   * Whether the user is currently authenticated
   */
  isAuthenticated: boolean;

  /**
   * Current authentication tokens
   */
  tokens: TokenPair | null;

  /**
   * Current user information
   */
  user: import('../events/types/CommonTypes').User | null;

  /**
   * UI session ID for reconnection
   */
  uiSessionId: string | null;

  /**
   * WebSocket URL for realtime connection
   */
  wsUrl: string | null;

  /**
   * Whether authentication is in progress
   */
  isAuthenticating: boolean;

  /**
   * Whether token refresh is in progress
   */
  isRefreshing: boolean;

  /**
   * Last authentication error
   */
  error: Error | null;
}

/**
 * Default authentication configuration
 */
export const DEFAULT_AUTH_CONFIG: Partial<AuthConfig> = {
  refreshBufferMs: 60000, // 1 minute before expiry
  autoRefresh: true,
  fetch: typeof globalThis !== 'undefined' ? globalThis.fetch?.bind(globalThis) : undefined,
};

/**
 * In-memory token storage implementation
 */
export class MemoryTokenStorage implements TokenStorage {
  private tokens: TokenPair | null = null;

  async getTokens(): Promise<TokenPair | null> {
    return this.tokens;
  }

  async setTokens(tokens: TokenPair): Promise<void> {
    this.tokens = tokens;
  }

  async clearTokens(): Promise<void> {
    this.tokens = null;
  }
}

/**
 * Browser localStorage token storage implementation
 */
export class LocalStorageTokenStorage implements TokenStorage {
  private readonly storageKey: string;

  constructor(storageKey = 'agentc_auth_tokens') {
    this.storageKey = storageKey;
  }

  async getTokens(): Promise<TokenPair | null> {
    if (typeof localStorage === 'undefined') {
      return null;
    }

    try {
      const stored = localStorage.getItem(this.storageKey);
      if (!stored) return null;
      return JSON.parse(stored) as TokenPair;
    } catch (error) {
      console.error('Failed to load tokens from localStorage:', error);
      return null;
    }
  }

  async setTokens(tokens: TokenPair): Promise<void> {
    if (typeof localStorage === 'undefined') {
      return;
    }

    try {
      localStorage.setItem(this.storageKey, JSON.stringify(tokens));
    } catch (error) {
      console.error('Failed to save tokens to localStorage:', error);
    }
  }

  async clearTokens(): Promise<void> {
    if (typeof localStorage === 'undefined') {
      return;
    }

    try {
      localStorage.removeItem(this.storageKey);
    } catch (error) {
      console.error('Failed to clear tokens from localStorage:', error);
    }
  }
}