// API endpoints are now proxied through Next.js API routes for security

import { Logger } from '@/utils/logger';

// DEBUG MODE - Set to true for verbose logging
const DEBUG_AUTH = false;

// Debug logger for auth library
const authLibLog = {
  info: (message: string, data?: any) => {
    if (DEBUG_AUTH) {
      Logger.info(`[AUTH-LIB] ✅ ${message}`, data || '');
    }
  },
  error: (message: string, error?: any) => {
    Logger.error(`[AUTH-LIB] ❌ ${message}`, error || '');
  },
  warn: (message: string, data?: any) => {
    Logger.warn(`[AUTH-LIB] ⚠️ ${message}`, data || '');
  },
  critical: (message: string, data?: any) => {
    Logger.error(`[AUTH-LIB] 🚨 CRITICAL: ${message}`, data || '');
  },
  debug: (message: string, data?: any) => {
    if (DEBUG_AUTH) {
      Logger.debug(`[AUTH-LIB] 🔍 ${message}`, data || '');
    }
  }
};

/**
 * Authentication credentials for login
 */
export interface LoginCredentials {
  username?: string;
  email?: string;
  password: string;
}

/**
 * Login response from the /api/rt/login endpoint
 */
export interface LoginResponse {
  agent_c_token: string;
  heygen_token: string;
  user: {
    user_id: string;
    user_name: string;
    email: string | null;
    first_name: string | null;
    last_name: string | null;
    is_active: boolean;
    roles: string[];
    groups: string[];
    created_at: string | null;
    last_login: string | null;
  };
  agents: Array<{
    name: string;
    key: string;
    agent_description: string | null;
    category: string[];
  }>;
  avatars: Array<{
    avatar_id: string;
    created_at: number;
    default_voice: string;
    is_public: boolean;
    normal_preview: string;
    pose_name: string;
    status: string;
  }>;
  tools: Array<{
    name: string;
    key: string;
    description: string | null;
    category: string[];
  }>;
  voices: Array<{
    voice_id: string;
    vendor: string;
    description: string;
    output_format: string;
  }>;
  ui_session_id: string;
}

/**
 * Decoded JWT token payload
 */
export interface JWTPayload {
  sub: string;
  exp: number;
  iat: number;
  [key: string]: any;
}

/**
 * Auth service configuration
 */
const AUTH_CONFIG = {
  tokenCookieName: 'agentc-auth-token',
  tokenCookieOptions: {
    secure: true, // Always use secure cookies for JWT tokens
    sameSite: 'strict' as const,
    httpOnly: false, // Need to access from JavaScript for API calls
    path: '/',
    maxAge: 60 * 60 * 24 * 7, // 7 days
  },
  apiEndpoints: {
    login: '/api/auth/login',
    session: '/api/auth/session',
  },
};

/**
 * Session information returned from the API
 */
export interface SessionInfo {
  websocketUrl: string;
  user: {
    id: string;
    [key: string]: any;
  };
  expiresAt: number;
}

/**
 * Parse JWT token without verification (client-side only)
 * For actual verification, rely on server-side validation
 */
function parseJWT(token: string): JWTPayload | null {
  try {
    authLibLog.debug('Parsing JWT token...');
    
    const parts = token.split('.');
    if (parts.length !== 3) {
      authLibLog.error('Invalid JWT format - expected 3 parts, got', parts.length);
      return null;
    }
    
    const payload = parts[1];
    const decoded = atob(payload.replace(/-/g, '+').replace(/_/g, '/'));
    const parsed = JSON.parse(decoded);
    
    authLibLog.debug('JWT parsed successfully', {
      sub: parsed.sub,
      exp: parsed.exp,
      iat: parsed.iat
    });
    
    return parsed;
  } catch (error) {
    authLibLog.error('Failed to parse JWT:', error);
    return null;
  }
}

/**
 * Set a cookie value
 */
function setCookie(name: string, value: string, options = AUTH_CONFIG.tokenCookieOptions) {
  authLibLog.debug(`Setting cookie: ${name}`, {
    valueLength: value?.length,
    valuePreview: value ? `${value.substring(0, 20)}...` : 'null',
    options
  });
  
  let cookie = `${name}=${encodeURIComponent(value)}`;
  
  if (options.maxAge) {
    cookie += `; Max-Age=${options.maxAge}`;
  }
  
  if (options.path) {
    cookie += `; Path=${options.path}`;
  }
  
  if (options.secure) {
    cookie += '; Secure';
  }
  
  if (options.sameSite) {
    cookie += `; SameSite=${options.sameSite}`;
  }
  
  if (options.httpOnly) {
    // Note: httpOnly cookies cannot be set from JavaScript
    authLibLog.warn('httpOnly flag is ignored when setting cookies from JavaScript');
  }
  
  const beforeCookie = document.cookie;
  document.cookie = cookie;
  const afterCookie = document.cookie;
  
  // Verify cookie was set
  const verifyValue = getCookie(name);
  if (verifyValue === value) {
    authLibLog.info(`Cookie ${name} set successfully`);
  } else {
    authLibLog.critical(`FAILED to set cookie ${name}!`, {
      expected: value?.substring(0, 20),
      actual: verifyValue?.substring(0, 20),
      cookieLengthBefore: beforeCookie.length,
      cookieLengthAfter: afterCookie.length
    });
  }
}

/**
 * Get a cookie value
 */
function getCookie(name: string): string | null {
  authLibLog.debug(`Getting cookie: ${name}`);
  
  const value = `; ${document.cookie}`;
  const parts = value.split(`; ${name}=`);
  
  if (parts.length === 2) {
    const cookieValue = parts.pop()?.split(';').shift();
    const decoded = cookieValue ? decodeURIComponent(cookieValue) : null;
    
    if (decoded) {
      authLibLog.debug(`Cookie ${name} found`, {
        length: decoded.length,
        preview: decoded.substring(0, 20) + '...'
      });
    }
    
    return decoded;
  }
  
  authLibLog.debug(`Cookie ${name} not found`);
  return null;
}

/**
 * Delete a cookie
 */
function deleteCookie(name: string, path = '/') {
  authLibLog.info(`Deleting cookie: ${name}`);
  document.cookie = `${name}=; Path=${path}; Expires=Thu, 01 Jan 1970 00:00:01 GMT; Secure; SameSite=strict`;
  
  // Verify deletion
  const checkValue = getCookie(name);
  if (checkValue) {
    authLibLog.error(`Failed to delete cookie ${name} - still present!`);
  } else {
    authLibLog.debug(`Cookie ${name} deleted successfully`);
  }
}

/**
 * Login with credentials
 * Calls the Next.js API route that proxies to the Agent C backend
 */
export async function login(credentials: LoginCredentials): Promise<LoginResponse> {
  authLibLog.info('=== AUTH-LIB LOGIN STARTING ===');
  authLibLog.debug('Login credentials:', {
    email: credentials.email,
    username: credentials.username,
    hasPassword: !!credentials.password,
    passwordLength: credentials.password?.length
  });
  
  try {
    authLibLog.info(`Calling login endpoint: ${AUTH_CONFIG.apiEndpoints.login}`);
    
    const response = await fetch(AUTH_CONFIG.apiEndpoints.login, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(credentials),
    });

    authLibLog.info('Login response received', {
      status: response.status,
      statusText: response.statusText,
      ok: response.ok
    });

    if (!response.ok) {
      const errorData = await response.json().catch(() => ({}));
      authLibLog.error('Login failed with error response', {
        status: response.status,
        errorData
      });
      throw new Error(errorData.error || errorData.message || `Login failed: ${response.statusText}`);
    }

    authLibLog.info('Parsing login response JSON...');
    const data: LoginResponse = await response.json();
    
    authLibLog.info('Login response parsed successfully', {
      hasAgentCToken: !!data.agent_c_token,
      agentCTokenLength: data.agent_c_token?.length,
      hasHeygenToken: !!data.heygen_token,
      hasUser: !!data.user,
      userId: data.user?.user_id,
      userEmail: data.user?.email,
      userName: data.user?.user_name,
      uiSessionId: data.ui_session_id,
      agentsCount: data.agents?.length,
      avatarsCount: data.avatars?.length,
      voicesCount: data.voices?.length
    });
    
    // CRITICAL: Check for required fields
    if (!data.agent_c_token) {
      authLibLog.critical('NO AGENT_C_TOKEN IN LOGIN RESPONSE!');
      authLibLog.error('Full response:', data);
      throw new Error('No agent_c_token received from login endpoint');
    }
    
    // Note: User data now comes from WebSocket events, not the login response
    // The login response only contains tokens and session ID
    authLibLog.debug('Login response received (user data will come from WebSocket events)', {
      hasAgentCToken: !!data.agent_c_token,
      hasHeygenToken: !!data.heygen_token,
      hasUiSessionId: !!data.ui_session_id
    });

    // Store agent_c_token in secure cookie
    authLibLog.info('Storing agent_c_token in cookie...');
    authLibLog.debug('Cookie name:', AUTH_CONFIG.tokenCookieName);
    authLibLog.debug('Token to store:', {
      length: data.agent_c_token.length,
      preview: `${data.agent_c_token.substring(0, 30)}...`
    });
    
    setCookie(AUTH_CONFIG.tokenCookieName, data.agent_c_token);
    
    // Verify token was stored
    const verifyToken = getCookie(AUTH_CONFIG.tokenCookieName);
    if (verifyToken === data.agent_c_token) {
      authLibLog.info('agent_c_token successfully stored in cookie');
    } else {
      authLibLog.critical('FAILED to store agent_c_token in cookie!', {
        expected: data.agent_c_token?.substring(0, 20),
        actual: verifyToken?.substring(0, 20),
        cookieName: AUTH_CONFIG.tokenCookieName
      });
      
      // Try alternative storage in localStorage as backup
      authLibLog.warn('Attempting localStorage backup storage...');
      try {
        localStorage.setItem('agentc-auth-token', data.agent_c_token);
        authLibLog.info('Token stored in localStorage as backup');
      } catch (e) {
        authLibLog.error('Failed to store in localStorage backup:', e);
      }
    }

    // Store HeyGen token if provided
    if (data.heygen_token) {
      authLibLog.info('Storing HeyGen token in cookie...');
      setCookie('agentc-heygen-token', data.heygen_token, {
        ...AUTH_CONFIG.tokenCookieOptions,
        maxAge: 60 * 60, // 1 hour for HeyGen token
      });
    }

    authLibLog.info('=== AUTH-LIB LOGIN COMPLETE ===', {
      success: true,
      userId: data.user?.user_id,
      userEmail: data.user?.email,
      userName: data.user?.user_name
    });

    return data;
  } catch (error) {
    authLibLog.error('AUTH-LIB LOGIN FAILED!', error);
    authLibLog.critical('Login error details:', {
      message: (error as any)?.message,
      stack: (error as any)?.stack
    });
    throw error;
  }
}

/**
 * Logout the current user
 * Logout is handled purely client-side by clearing tokens from cookies
 */
export function logout(): void {
  authLibLog.info('=== AUTH-LIB LOGOUT ===');
  
  // Clear all authentication tokens from cookies
  authLibLog.info('Clearing authentication tokens...');
  deleteCookie(AUTH_CONFIG.tokenCookieName);
  deleteCookie('agentc-heygen-token');
  
  // Verify tokens are cleared
  const checkToken = getCookie(AUTH_CONFIG.tokenCookieName);
  const checkHeyGen = getCookie('agentc-heygen-token');
  
  if (checkToken || checkHeyGen) {
    authLibLog.critical('TOKENS NOT CLEARED AFTER LOGOUT!', {
      agentCToken: !!checkToken,
      heygenToken: !!checkHeyGen
    });
  } else {
    authLibLog.info('All tokens successfully cleared');
  }
}

/**
 * Get the current authentication token
 */
export function getToken(): string | null {
  if (typeof window === 'undefined') {
    authLibLog.debug('getToken called in SSR context - returning null');
    return null; // Server-side rendering
  }
  
  const token = getCookie(AUTH_CONFIG.tokenCookieName);
  
  if (DEBUG_AUTH) {
    authLibLog.debug('getToken called', {
      hasToken: !!token,
      tokenLength: token?.length || 0,
      tokenPreview: token ? `${token.substring(0, 20)}...` : 'null'
    });
  }
  
  if (!token) {
    authLibLog.debug('No auth token found in cookies');
  }
  
  return token;
}

/**
 * Get the HeyGen token if available
 */
export function getHeyGenToken(): string | null {
  if (typeof window === 'undefined') {
    return null;
  }
  
  return getCookie('agentc-heygen-token');
}

/**
 * Check if the user is authenticated
 * Validates token existence and expiration
 */
export function isAuthenticated(): boolean {
  authLibLog.debug('Checking authentication status...');
  
  const token = getToken();
  
  if (!token) {
    authLibLog.debug('No token found - user is NOT authenticated');
    return false;
  }
  
  authLibLog.debug('Token found, checking validity...', {
    tokenLength: token.length,
    tokenPreview: `${token.substring(0, 20)}...`
  });

  // Parse and check token expiration
  const payload = parseJWT(token);
  
  if (!payload) {
    authLibLog.error('Failed to parse JWT token - invalid format');
    return false;
  }
  
  if (!payload.exp) {
    authLibLog.error('JWT token has no expiration - invalid');
    return false;
  }
  
  authLibLog.debug('JWT payload parsed', {
    sub: payload.sub,
    exp: payload.exp,
    iat: payload.iat
  });

  // Check if token is expired (with 30 second buffer)
  const expirationTime = payload.exp * 1000; // Convert to milliseconds
  const currentTime = Date.now();
  const buffer = 30 * 1000; // 30 second buffer
  const timeRemaining = (expirationTime - buffer) - currentTime;
  
  const isValid = currentTime < (expirationTime - buffer);
  
  authLibLog.info('Authentication check complete', {
    isValid,
    expiresAt: new Date(expirationTime).toISOString(),
    timeRemaining: `${Math.floor(timeRemaining / 1000)} seconds`,
    userId: payload.sub
  });
  
  if (!isValid) {
    authLibLog.warn('Token is expired or expiring soon');
  }
  
  return isValid;
}

/**
 * Get the full user data from stored login response
 * 
 * This is the CORRECT way to get user profile data!
 * Returns the complete user object with email, name, roles, etc.
 * 
 * @returns Full user object or null if not available
 */
export function getStoredUser(): LoginResponse['user'] | null {
  authLibLog.debug('Getting stored user data...');
  
  if (typeof window === 'undefined') {
    authLibLog.debug('getStoredUser called in SSR context - returning null');
    return null;
  }
  
  try {
    // First try to get from dedicated user storage
    const storedUserData = localStorage.getItem('agentc-user-data');
    if (storedUserData) {
      const userData = JSON.parse(storedUserData);
      authLibLog.info('Retrieved user from dedicated storage', {
        userId: userData.user_id || userData.id,
        email: userData.email,
        userName: userData.user_name
      });
      return userData;
    }
    
    // Fallback to login response
    const storedResponse = localStorage.getItem('agentc-login-response');
    if (storedResponse) {
      const response: LoginResponse = JSON.parse(storedResponse);
      if (response.user) {
        authLibLog.info('Retrieved user from login response', {
          userId: response.user.user_id,
          email: response.user.email,
          userName: response.user.user_name
        });
        return response.user;
      }
    }
    
    authLibLog.warn('No stored user data found');
    return null;
  } catch (error) {
    authLibLog.error('Failed to get stored user data', error);
    return null;
  }
}

/**
 * Get the current user from the JWT token
 * 
 * ⚠️ WARNING: This function only returns MINIMAL data from the JWT payload!
 * JWT tokens typically only contain: { sub, exp, iat, permissions }
 * 
 * DO NOT USE THIS FOR USER PROFILE DATA!
 * 
 * For full user data (email, name, etc.), use the user object from:
 * - The login response (stored in localStorage as 'agentc-login-response')
 * - The stored user data (stored in localStorage as 'agentc-user-data')
 * 
 * This function should ONLY be used for:
 * - Getting the user ID when nothing else is available
 * - Checking token validity
 * 
 * @deprecated for user profile data - use stored login response instead
 */
export function getCurrentUser(): { id: string; [key: string]: any } | null {
  authLibLog.debug('Getting current user from JWT...');
  authLibLog.warn('⚠️ getCurrentUser() returns MINIMAL JWT data - not suitable for user profiles!');
  
  const token = getToken();
  
  if (!token) {
    authLibLog.warn('No token found - cannot get current user');
    return null;
  }

  const payload = parseJWT(token);
  
  if (!payload) {
    authLibLog.error('Failed to parse JWT for user data');
    return null;
  }
  
  if (!payload.sub) {
    authLibLog.error('JWT has no subject (user ID)');
    return null;
  }
  
  const user = {
    id: payload.sub,
    ...payload,
  };
  
  authLibLog.info('Current user retrieved from JWT (MINIMAL DATA)', {
    userId: user.id,
    hasExp: !!user.exp,
    hasIat: !!user.iat,
    permissions: (user as any).permissions || 'none',
    additionalFields: Object.keys(payload).filter(k => !['sub', 'exp', 'iat'].includes(k))
  });
  
  authLibLog.critical('JWT DOES NOT CONTAIN: email, user_name, first_name, last_name, roles, etc.');
  authLibLog.critical('For full user data, use the stored login response!');
  
  return user;
}

/**
 * Create an Authorization header value
 */
export function getAuthHeader(): { Authorization: string } | {} {
  const token = getToken();
  
  if (!token) {
    return {};
  }

  return {
    Authorization: `Bearer ${token}`,
  };
}

/**
 * Get session information including WebSocket URL
 * Requires a valid authentication token
 */
export async function getSessionInfo(): Promise<SessionInfo> {
  const token = getToken();
  
  if (!token) {
    throw new Error('No authentication token available');
  }

  try {
    const response = await fetch(AUTH_CONFIG.apiEndpoints.session, {
      method: 'GET',
      headers: {
        'Authorization': `Bearer ${token}`,
      },
    });

    if (!response.ok) {
      const errorData = await response.json().catch(() => ({}));
      throw new Error(errorData.error || `Failed to get session info: ${response.statusText}`);
    }

    const data: SessionInfo = await response.json();
    return data;
  } catch (error) {
    Logger.error('Get session info error:', error);
    throw error;
  }
}

/**
 * Refresh the token if it's close to expiration
 * Returns true if token was refreshed or is still valid
 */
export async function refreshTokenIfNeeded(): Promise<boolean> {
  const token = getToken();
  
  if (!token) {
    return false;
  }

  const payload = parseJWT(token);
  
  if (!payload || !payload.exp) {
    return false;
  }

  // Check if token expires in less than 5 minutes
  const expirationTime = payload.exp * 1000;
  const currentTime = Date.now();
  const fiveMinutes = 5 * 60 * 1000;
  
  if (currentTime > (expirationTime - fiveMinutes)) {
    // Token is expiring soon, attempt refresh
    // Note: This would require a refresh endpoint which may not exist yet
    // For now, return false to indicate refresh is needed
    Logger.warn('Token expiring soon, refresh required');
    return false;
  }

  return true;
}