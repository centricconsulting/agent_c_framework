/**
 * Authentication module exports for the Agent C Realtime SDK
 */

export { AuthManager } from './AuthManager';
export type { AuthManagerEvents } from './AuthManager';

export {
  // Configuration
  type AuthConfig,
  type TokenStorage,
  type TokenPair,
  type LoginCredentials,
  type JWTPayload,
  type AuthState,
  DEFAULT_AUTH_CONFIG,
  
  // Storage implementations
  MemoryTokenStorage,
  LocalStorageTokenStorage,
} from './AuthConfig';