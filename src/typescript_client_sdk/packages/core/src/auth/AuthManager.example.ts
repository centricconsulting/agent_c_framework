/**
 * Example usage of AuthManager.initializeFromPayload
 * 
 * This demonstrates how production apps can initialize the SDK
 * with a complete login payload from their backend.
 */

import { AuthManager } from './AuthManager';
import { LoginResponse } from '../events/types/CommonTypes';
import { Logger } from '../utils/logger';

// Example 1: Traditional login flow (existing functionality)
async function traditionalLogin() {
  const authManager = new AuthManager({
    apiUrl: 'https://api.example.com',
    autoRefresh: true,
  });

  // Login with credentials
  await authManager.login({
    username: 'user@example.com',
    password: 'password123',
  });

  // Access resources
  // const agents = authManager.getAgents();
  // const voices = authManager.getVoices();
  // const token = authManager.getAgentCToken();
}

// Example 2: Initialize from backend payload (NEW METHOD)
async function initializeFromBackend() {
  const authManager = new AuthManager({
    apiUrl: 'https://api.example.com',
    autoRefresh: true,
  });

  // Your backend provides the simplified login payload
  // NOTE: Breaking change - login now only returns tokens and session ID
  // All configuration data comes through WebSocket events after connection
  const payloadFromBackend: LoginResponse = {
    agent_c_token: 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...',
    heygen_token: 'hg_token_123...',
    ui_session_id: 'session_abc123',
  };

  // WebSocket URL (might also come from your backend)
  const wsUrl = 'wss://api.example.com/rt/ws';

  // Initialize with the simplified payload
  await authManager.initializeFromPayload(payloadFromBackend, wsUrl);

  // NOTE: Configuration data is no longer available from authManager
  // Data like agents, voices, avatars, tools, and sessions now come
  // through WebSocket events after connection.
  
  const token = authManager.getAgentCToken();
  const heygenToken = authManager.getHeygenToken();
  const user = authManager.getUser(); // Will be null until chat_user_data event
  const sessionId = authManager.getUiSessionId();

  // AuthManager is initialized with tokens only
  Logger.info('Authentication token:', token ? 'Present' : 'Missing');
  Logger.info('HeyGen token:', heygenToken ? 'Present' : 'Missing');
  Logger.info('User data:', user ? user.user_name : 'Will come from WebSocket events');
  Logger.info('UI Session ID:', sessionId);
  
  // The auth tokens will auto-refresh if autoRefresh is enabled
  // The authManager will emit the same events as if you had logged in normally
}

// Example 3: Using with RealtimeClient
async function useWithRealtimeClient() {
  const authManager = new AuthManager({
    apiUrl: 'https://api.example.com',
    autoRefresh: true,
  });

  // Get payload from your backend
  const payload = await fetch('/api/agent-c-auth')
    .then(r => r.json());
    
  const wsUrl = 'wss://api.example.com/rt/ws';

  // Initialize AuthManager
  await authManager.initializeFromPayload(payload, wsUrl);

  // Now create RealtimeClient with the initialized AuthManager
  // (RealtimeClient implementation would use authManager.getAgentCToken())
  // const client = new RealtimeClient({
  //   wsUrl,
  //   authManager,
  // });
}

export { traditionalLogin, initializeFromBackend, useWithRealtimeClient };