/**
 * Authentication library for static SPA
 * Uses relative URLs since everything is served from same host
 */

const TOKEN_KEY = 'agentc-auth-token'

export interface LoginCredentials {
  username?: string
  email?: string
  password: string
}

export interface LoginResponse {
  agent_c_token: string
  heygen_token: string
  user: {
    user_id: string
    user_name: string
    email: string | null
    first_name: string | null
    last_name: string | null
    is_active: boolean
    roles: string[]
    groups: string[]
    created_at: string | null
    last_login: string | null
  }
  ui_session_id: string
}

/**
 * Parse JWT token (client-side only, no verification)
 */
function parseJWT(token: string): { exp: number; sub: string } | null {
  try {
    const parts = token.split('.')
    if (parts.length !== 3) return null
    
    const payload = parts[1]
    const decoded = atob(payload.replace(/-/g, '+').replace(/_/g, '/'))
    return JSON.parse(decoded)
  } catch {
    return null
  }
}

/**
 * Login with credentials
 * Makes direct call to API on same host - no environment variables needed!
 */
export async function login(credentials: LoginCredentials): Promise<LoginResponse> {
  const response = await fetch('/api/rt/login', {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
    },
    credentials: 'include',
    body: JSON.stringify(credentials),
  })

  if (!response.ok) {
    const error = await response.json().catch(() => ({ error: 'Login failed' }))
    throw new Error(error.error || error.message || 'Login failed')
  }

  const data: LoginResponse = await response.json()
  
  if (!data.agent_c_token) {
    throw new Error('No token received from server')
  }

  // Store token in localStorage
  localStorage.setItem(TOKEN_KEY, data.agent_c_token)
  
  return data
}

/**
 * Logout - clear token
 */
export function logout(): void {
  localStorage.removeItem(TOKEN_KEY)
}

/**
 * Get current auth token
 */
export function getToken(): string | null {
  return localStorage.getItem(TOKEN_KEY)
}

/**
 * Check if user is authenticated
 */
export function isAuthenticated(): boolean {
  const token = getToken()
  
  if (!token) return false

  // Check token expiration
  const payload = parseJWT(token)
  if (!payload || !payload.exp) return false

  // Check if expired (with 30 second buffer)
  const now = Date.now() / 1000
  return payload.exp > now + 30
}

/**
 * Get WebSocket URL for the current host
 * Uses wss:// protocol and current window location
 */
export function getWebSocketUrl(): string {
  const protocol = window.location.protocol === 'https:' ? 'wss:' : 'ws:'
  const host = window.location.host
  return `${protocol}//${host}/api/rt/websocket`
}
