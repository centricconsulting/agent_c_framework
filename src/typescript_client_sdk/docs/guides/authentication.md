# Agent C Authentication Guide

This guide explains how to properly integrate Agent C authentication into production applications. **Important**: Production applications should never expose ChatUser credentials to frontend clients. Instead, your backend manages ChatUsers and provides tokens to your frontend.

## Production Authentication Architecture

Agent C is designed to be wrapped by your application's authentication layer. Here's the correct architecture:

### How It Really Works

```
┌─────────────────────────────────────────────────────────────┐
│                     Your Application                         │
├─────────────────────────────────────────────────────────────┤
│                                                              │
│  Frontend (Browser/Mobile)          Backend (Your Server)    │
│  ┌──────────────────────┐         ┌────────────────────────┐│
│  │                      │         │                        ││
│  │  1. User login       │────────▶│  2. Authenticate user  ││
│  │     (your auth)      │         │     (your database)    ││
│  │                      │         │                        ││
│  │                      │         │  3. Create/get         ││
│  │  5. Receive tokens   │◀────────│     ChatUser           ││
│  │     (no credentials) │         │     (Agent C library)  ││
│  │                      │         │                        ││
│  │  6. Connect to       │         │  4. Generate JWT       ││
│  │     Agent C WS       │         │     for user           ││
│  │                      │         │                        ││
│  └──────────────────────┘         └────────────────────────┘│
│                                              │               │
└──────────────────────────────────────────────┼───────────────┘
                                               │
                                               ▼
                                    ┌──────────────────┐
                                    │    Agent C API    │
                                    │  (Never exposed   │
                                    │   to frontend)    │
                                    └──────────────────┘
```

### Key Security Principles

1. **Frontend never has ChatUser credentials** - Only your backend knows these
2. **Backend manages ChatUsers** - Creates, maps, and manages ChatUser accounts
3. **Backend generates tokens** - Your backend calls Agent C to get tokens
4. **Frontend receives tokens only** - Gets JWT from your backend, not Agent C
5. **Your app owns user authentication** - Agent C doesn't manage your users

## Understanding ChatUsers

**ChatUsers** are Agent C platform accounts that your backend manages:

- Created and managed by your backend using Agent C library functions
- Have access to agents, voices, avatars, and tools
- Can be shared across multiple application users (pooling)
- Or mapped 1:1 with application users (dedicated)
- **Never exposed to frontend applications**

## Backend Integration with Agent C

Your backend server is responsible for managing the relationship between your application users and Agent C ChatUsers. Here's how to properly integrate:

### 1. Backend ChatUser Management

```typescript
// backend/services/AgentCService.ts
import { AgentCClient } from '@agentc/server-sdk'; // Server-side SDK

export class AgentCService {
  private agentC: AgentCClient;
  private chatUserCache = new Map<string, ChatUserInfo>();
  
  constructor() {
    // Initialize with your Agent C server credentials
    // These are NEVER exposed to frontend
    this.agentC = new AgentCClient({
      apiUrl: process.env.AGENTC_API_URL,
      adminUsername: process.env.AGENTC_ADMIN_USER,
      adminPassword: process.env.AGENTC_ADMIN_PASS
    });
  }
  
  /**
   * Get or create a ChatUser for an application user
   */
  async getChatUserForAppUser(appUserId: string): Promise<ChatUserInfo> {
    // Check cache first
    if (this.chatUserCache.has(appUserId)) {
      return this.chatUserCache.get(appUserId)!;
    }
    
    // Check if ChatUser already exists for this app user
    const chatUserId = await this.getChatUserMapping(appUserId);
    
    if (chatUserId) {
      // Existing ChatUser found
      const info = await this.agentC.getChatUser(chatUserId);
      this.chatUserCache.set(appUserId, info);
      return info;
    }
    
    // Create new ChatUser for this app user
    const newChatUser = await this.createChatUser(appUserId);
    await this.saveChatUserMapping(appUserId, newChatUser.id);
    this.chatUserCache.set(appUserId, newChatUser);
    
    return newChatUser;
  }
  
  /**
   * Generate JWT token for frontend to use
   */
  async generateTokenForUser(appUserId: string): Promise<TokenPayload> {
    const chatUser = await this.getChatUserForAppUser(appUserId);
    
    // Use Agent C library to generate JWT for this ChatUser
    const tokenData = await this.agentC.generateToken({
      chatUserId: chatUser.id,
      expiresIn: '1h'
    });
    
    return {
      agent_c_token: tokenData.token,
      apiUrl: 'https://api.agentc.ai', // Base API URL for SDK to construct WebSocket
      expires_at: tokenData.expires_at,
      agents: chatUser.agents,
      voices: chatUser.voices,
      avatars: chatUser.avatars
    };
  }
  
  private async createChatUser(appUserId: string): Promise<ChatUserInfo> {
    // Create a ChatUser with appropriate permissions
    return await this.agentC.createChatUser({
      username: `app_user_${appUserId}`,
      password: this.generateSecurePassword(),
      agents: ['default_agent'],
      voices: ['nova', 'echo'],
      metadata: {
        app_user_id: appUserId,
        created_at: new Date().toISOString()
      }
    });
  }
  
  private async getChatUserMapping(appUserId: string): Promise<string | null> {
    // Query your database for existing mapping
    const mapping = await db.query(
      'SELECT chat_user_id FROM user_mappings WHERE app_user_id = ?',
      [appUserId]
    );
    return mapping?.chat_user_id || null;
  }
  
  private async saveChatUserMapping(appUserId: string, chatUserId: string): Promise<void> {
    // Save mapping to your database
    await db.query(
      'INSERT INTO user_mappings (app_user_id, chat_user_id) VALUES (?, ?)',
      [appUserId, chatUserId]
    );
  }
  
  private generateSecurePassword(): string {
    // Generate a secure password that your backend manages
    // Frontend never sees this
    return crypto.randomBytes(32).toString('hex');
  }
}
```

### 2. Backend API Endpoints

```typescript
// backend/routes/auth.ts
import { AgentCService } from '../services/AgentCService';

const agentCService = new AgentCService();

/**
 * Your application's login endpoint
 */
app.post('/api/auth/login', async (req, res) => {
  const { email, password } = req.body;
  
  // 1. Authenticate against YOUR user database
  const user = await authenticateUser(email, password);
  if (!user) {
    return res.status(401).json({ error: 'Invalid credentials' });
  }
  
  // 2. Generate Agent C token for this user
  const agentCTokens = await agentCService.generateTokenForUser(user.id);
  
  // 3. Create session in your application
  const sessionToken = await createUserSession(user.id);
  
  // 4. Return tokens to frontend
  return res.json({
    session_token: sessionToken,      // Your app's session
    agent_c: agentCTokens,            // Agent C tokens for frontend
    user: {
      id: user.id,
      email: user.email,
      name: user.name
    }
  });
});

/**
 * Refresh Agent C token
 */
app.post('/api/auth/refresh-agent-token', authenticateSession, async (req, res) => {
  // User is authenticated via your session middleware
  const userId = req.user.id;
  
  // Generate fresh Agent C token
  const agentCTokens = await agentCService.generateTokenForUser(userId);
  
  return res.json({
    agent_c: agentCTokens
  });
});
```

## Frontend SDK Usage (Production Pattern)

The frontend SDK should be initialized with tokens received from YOUR backend, not by directly calling Agent C:

### 1. Frontend Authentication Flow

```typescript
// frontend/services/auth.ts
import { AuthManager, RealtimeClient } from '@agentc/realtime-core';

class ApplicationAuth {
  private authManager: AuthManager;
  private client?: RealtimeClient;
  
  constructor() {
    this.authManager = new AuthManager();
  }
  
  /**
   * Login through YOUR application backend
   */
  async login(email: string, password: string): Promise<void> {
    // 1. Authenticate with YOUR backend, not Agent C
    const response = await fetch('/api/auth/login', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ email, password })
    });
    
    if (!response.ok) {
      throw new Error('Login failed');
    }
    
    const data = await response.json();
    
    // 2. Store your application session
    localStorage.setItem('app_session', data.session_token);
    
    // 3. Initialize Agent C with tokens from YOUR backend
    await this.initializeAgentC(data.agent_c);
  }
  
  /**
   * Initialize Agent C with tokens (production pattern)
   */
  private async initializeAgentC(agentCData: any): Promise<void> {
    // Use initializeFromPayload for production
    // NOT login() which requires ChatUser credentials
    await this.authManager.initializeFromPayload({
      agent_c_token: agentCData.agent_c_token,
      websocket_url: agentCData.websocket_url,
      heygen_token: agentCData.heygen_token,
      agents: agentCData.agents,
      voices: agentCData.voices,
      avatars: agentCData.avatars,
      expires_at: agentCData.expires_at
    });
    
    // Create the realtime client
    this.client = new RealtimeClient({
      apiUrl: agentCData.websocket_url,
      authManager: this.authManager,
      autoReconnect: true
    });
    
    // Set up token refresh through YOUR backend
    this.authManager.on('token-expiring', async () => {
      await this.refreshAgentCToken();
    });
  }
  
  /**
   * Refresh Agent C token through YOUR backend
   */
  private async refreshAgentCToken(): Promise<void> {
    const response = await fetch('/api/auth/refresh-agent-token', {
      method: 'POST',
      headers: {
        'Authorization': `Bearer ${localStorage.getItem('app_session')}`
      }
    });
    
    if (!response.ok) {
      throw new Error('Token refresh failed');
    }
    
    const data = await response.json();
    
    // Update Agent C tokens
    await this.authManager.updateTokens({
      agent_c_token: data.agent_c.agent_c_token,
      expires_at: data.agent_c.expires_at
    });
  }
  
  async connect(): Promise<void> {
    if (!this.client) {
      throw new Error('Not authenticated');
    }
    await this.client.connect();
  }
}
```

### 2. React Component Example

```tsx
// frontend/components/AgentChat.tsx
import React, { useEffect, useState } from 'react';
import { useRealtimeClient } from '@agentc/realtime-react';

function AgentChat() {
  const [isAuthenticated, setIsAuthenticated] = useState(false);
  const client = useRealtimeClient();
  
  const handleLogin = async (email: string, password: string) => {
    try {
      // Login through YOUR backend
      const response = await fetch('/api/auth/login', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ email, password })
      });
      
      const data = await response.json();
      
      // Initialize Agent C client with tokens from YOUR backend
      await client.initializeFromPayload(data.agent_c);
      await client.connect();
      
      setIsAuthenticated(true);
    } catch (error) {
      console.error('Login failed:', error);
    }
  };
  
  // Rest of component...
}
```

### 3. AuthManager Methods for Production

```typescript
// The AuthManager class provides these methods for production use:

class AuthManager {
  /**
   * Initialize with tokens from your backend (PRODUCTION)
   * Use this instead of login() in production
   */
  async initializeFromPayload(payload: LoginPayload): Promise<void> {
    this.token = payload.agent_c_token;
    this.websocketUrl = payload.websocket_url;
    this.heygenToken = payload.heygen_token;
    this.agents = payload.agents;
    this.voices = payload.voices;
    this.avatars = payload.avatars;
    this.expiresAt = payload.expires_at;
    
    // Start token refresh timer
    this.scheduleTokenRefresh();
  }
  
  /**
   * Direct login with ChatUser credentials (DEVELOPMENT ONLY)
   * Never use this in production - credentials should stay on backend
   */
  async login(credentials: LoginCredentials): Promise<LoginResponse> {
    console.warn('AuthManager.login() should only be used for development!');
    // Direct login implementation...
  }
  
  /**
   * Update tokens when refreshed through your backend
   */
  async updateTokens(tokens: TokenUpdate): Promise<void> {
    this.token = tokens.agent_c_token;
    this.expiresAt = tokens.expires_at;
    this.scheduleTokenRefresh();
  }
}
```

## ChatUser Management Strategies

Your backend can implement different strategies for managing ChatUsers based on your application's needs:

### Strategy 1: Shared ChatUser Pool

For applications where users don't need isolated resources:

```typescript
// backend/services/SharedChatUserPool.ts
export class SharedChatUserPool {
  private pool: ChatUserConnection[] = [];
  private readonly poolSize = 10;
  
  async initialize() {
    // Create a pool of ChatUsers for load balancing
    for (let i = 0; i < this.poolSize; i++) {
      const chatUser = await this.agentC.createChatUser({
        username: `pool_user_${i}`,
        password: this.generateSecurePassword(),
        agents: ['shared_agent'],
        voices: ['nova', 'echo']
      });
      
      this.pool.push({
        chatUser,
        activeConnections: 0,
        maxConnections: 100
      });
    }
  }
  
  async assignChatUser(appUserId: string): Promise<TokenPayload> {
    // Find least loaded ChatUser
    const connection = this.pool
      .filter(c => c.activeConnections < c.maxConnections)
      .sort((a, b) => a.activeConnections - b.activeConnections)[0];
    
    if (!connection) {
      throw new Error('No available ChatUsers in pool');
    }
    
    connection.activeConnections++;
    
    // Generate token for this ChatUser
    return await this.agentC.generateToken({
      chatUserId: connection.chatUser.id,
      metadata: {
        app_user_id: appUserId,
        pool_assignment: true
      }
    });
  }
  
  async releaseChatUser(appUserId: string) {
    // Decrement connection count when user disconnects
    // Implementation...
  }
}
```

### Strategy 2: Dedicated ChatUser per User

For applications requiring user isolation:

```typescript
// backend/services/DedicatedChatUserManager.ts
export class DedicatedChatUserManager {
  async getOrCreateChatUser(appUser: ApplicationUser): Promise<ChatUser> {
    // Check if user already has a ChatUser
    const existing = await db.query(
      'SELECT chat_user_id FROM user_mappings WHERE app_user_id = ?',
      [appUser.id]
    );
    
    if (existing) {
      return await this.agentC.getChatUser(existing.chat_user_id);
    }
    
    // Create dedicated ChatUser
    const chatUser = await this.agentC.createChatUser({
      username: `user_${appUser.id}`,
      password: this.generateSecurePassword(),
      agents: this.getAgentsForUserTier(appUser.subscription_tier),
      voices: this.getVoicesForUserTier(appUser.subscription_tier),
      metadata: {
        app_user_id: appUser.id,
        tier: appUser.subscription_tier,
        created_at: new Date().toISOString()
      }
    });
    
    // Save mapping
    await db.query(
      'INSERT INTO user_mappings (app_user_id, chat_user_id) VALUES (?, ?)',
      [appUser.id, chatUser.id]
    );
    
    return chatUser;
  }
  
  private getAgentsForUserTier(tier: string): string[] {
    switch(tier) {
      case 'premium':
        return ['advanced_agent', 'specialist_agent', 'default_agent'];
      case 'standard':
        return ['default_agent'];
      default:
        return ['basic_agent'];
    }
  }
}
```

### Strategy 3: Tenant-Based Isolation

For multi-tenant SaaS applications:

```typescript
// backend/services/TenantChatUserManager.ts
export class TenantChatUserManager {
  private tenantPools = new Map<string, ChatUserPool>();
  
  async initializeTenant(tenantId: string, config: TenantConfig) {
    // Create ChatUsers for this tenant
    const pool = new ChatUserPool();
    
    for (let i = 0; i < config.poolSize; i++) {
      const chatUser = await this.agentC.createChatUser({
        username: `tenant_${tenantId}_user_${i}`,
        password: this.generateSecurePassword(),
        agents: config.agents,
        voices: config.voices,
        tools: config.tools,
        metadata: {
          tenant_id: tenantId,
          tenant_name: config.name
        }
      });
      
      pool.addChatUser(chatUser);
    }
    
    this.tenantPools.set(tenantId, pool);
  }
  
  async getTokenForTenantUser(
    tenantId: string, 
    userId: string
  ): Promise<TokenPayload> {
    const pool = this.tenantPools.get(tenantId);
    if (!pool) {
      throw new Error(`Tenant ${tenantId} not initialized`);
    }
    
    return await pool.assignChatUser(userId);
  }
}
```

## Development vs Production Authentication

### Development Mode (Direct Login)

For development and testing ONLY, you can use direct login:

```typescript
// development/test-client.ts
// ⚠️ DEVELOPMENT ONLY - Never use in production!

import { AuthManager, RealtimeClient } from '@agentc/realtime-core';

async function developmentLogin() {
  const authManager = new AuthManager({
    apiUrl: 'https://localhost:8000'
  });
  
  // Direct login - ONLY for development
  const response = await authManager.login({
    username: process.env.DEV_CHATUSER_USERNAME,
    password: process.env.DEV_CHATUSER_PASSWORD
  });
  
  const client = new RealtimeClient({
    apiUrl: response.websocket_url,
    authManager: authManager
  });
  
  await client.connect();
  
  console.warn('Using development authentication - not for production!');
}
```

### Production Mode (Token from Backend)

```typescript
// production/app.ts
// ✅ PRODUCTION PATTERN - Secure authentication

import { AuthManager, RealtimeClient } from '@agentc/realtime-core';

class ProductionApp {
  private authManager = new AuthManager();
  private client?: RealtimeClient;
  
  async initialize() {
    // Get tokens from YOUR backend
    const loginData = await this.loginToYourBackend();
    
    // Initialize with tokens (no credentials)
    await this.authManager.initializeFromPayload(loginData.agent_c);
    
    // Create client
    this.client = new RealtimeClient({
      apiUrl: loginData.agent_c.websocket_url,
      authManager: this.authManager
    });
    
    await this.client.connect();
  }
  
  private async loginToYourBackend() {
    // Your app's authentication
    const response = await fetch('/api/auth/login', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({
        email: 'user@example.com',
        password: 'user_password'
      })
    });
    
    return response.json();
  }
}
```

## Security Architecture Explained

### Why This Architecture?

1. **Credential Security**: ChatUser credentials never leave your backend
2. **User Management**: Your app controls user access and permissions
3. **Token Control**: Your backend manages token generation and refresh
4. **Audit Trail**: All authentication flows through your system
5. **Flexibility**: Support multiple user models without exposing internals

### The Security Layers

```
┌─────────────────────────────────────────────────────────────┐
│                    Security Boundaries                       │
├─────────────────────────────────────────────────────────────┤
│                                                             │
│  Frontend (Untrusted)                                      │
│  - Has: JWT tokens, WebSocket URL                          │
│  - Never has: ChatUser credentials, admin access           │
│                                                             │
│  ═══════════════════════════════════════════════════════   │
│                     ↑ Security Boundary ↑                   │
│  ═══════════════════════════════════════════════════════   │
│                                                             │
│  Your Backend (Trusted)                                    │
│  - Has: ChatUser credentials (encrypted)                   │
│  - Manages: User mappings, token generation                │
│  - Controls: Access policies, rate limiting                │
│                                                             │
│  ═══════════════════════════════════════════════════════   │
│                     ↑ API Boundary ↑                        │
│  ═══════════════════════════════════════════════════════   │
│                                                             │
│  Agent C Server                                            │
│  - Validates: JWT tokens                                   │
│  - Manages: ChatUsers, agents, resources                   │
│  - Provides: Realtime WebSocket API                        │
│                                                             │
└─────────────────────────────────────────────────────────────┘
```

## Complete Implementation Examples

### Backend Implementation (Node.js/Express)

```typescript
// backend/server.ts
import express from 'express';
import { AgentCServerSDK } from '@agentc/server-sdk';
import { authenticateUser } from './auth';
import { ChatUserManager } from './services/ChatUserManager';

const app = express();
const agentC = new AgentCServerSDK({
  apiUrl: process.env.AGENTC_API_URL,
  adminCredentials: {
    username: process.env.AGENTC_ADMIN_USER,
    password: process.env.AGENTC_ADMIN_PASS
  }
});

const chatUserManager = new ChatUserManager(agentC);

// Your application's login endpoint
app.post('/api/auth/login', async (req, res) => {
  try {
    const { email, password } = req.body;
    
    // 1. Authenticate YOUR user
    const user = await authenticateUser(email, password);
    if (!user) {
      return res.status(401).json({ error: 'Invalid credentials' });
    }
    
    // 2. Get or create ChatUser for this user
    const chatUser = await chatUserManager.getChatUserForAppUser(user.id);
    
    // 3. Generate Agent C tokens
    const tokens = await agentC.generateTokenForChatUser(chatUser.id);
    
    // 4. Create session in YOUR system
    const session = await createUserSession(user.id);
    
    // 5. Return everything the frontend needs
    res.json({
      // Your app's session
      session: {
        token: session.token,
        expires_at: session.expires_at
      },
      // User info
      user: {
        id: user.id,
        email: user.email,
        name: user.name
      },
      // Agent C configuration for frontend
      agent_c: {
        agent_c_token: tokens.jwt_token,
        websocket_url: tokens.websocket_url,
        heygen_token: tokens.heygen_token,
        expires_at: tokens.expires_at,
        agents: chatUser.agents,
        voices: chatUser.voices,
        avatars: chatUser.avatars
      }
    });
  } catch (error) {
    console.error('Login error:', error);
    res.status(500).json({ error: 'Internal server error' });
  }
});

// Token refresh endpoint
app.post('/api/auth/refresh', authenticateSession, async (req, res) => {
  try {
    const userId = req.user.id;  // From your session middleware
    
    // Get ChatUser for this user
    const chatUser = await chatUserManager.getChatUserForAppUser(userId);
    
    // Generate fresh tokens
    const tokens = await agentC.generateTokenForChatUser(chatUser.id);
    
    res.json({
      agent_c: {
        agent_c_token: tokens.jwt_token,
        expires_at: tokens.expires_at
      }
    });
  } catch (error) {
    console.error('Token refresh error:', error);
    res.status(500).json({ error: 'Failed to refresh token' });
  }
});
```

### Frontend Implementation (React)

```tsx
// frontend/src/services/AgentCService.ts
import { AuthManager, RealtimeClient } from '@agentc/realtime-core';

export class AgentCService {
  private authManager: AuthManager;
  private client?: RealtimeClient;
  
  constructor() {
    this.authManager = new AuthManager();
  }
  
  /**
   * Initialize with tokens from your backend
   */
  async initialize(agentCData: any) {
    // Use initializeFromPayload for production
    await this.authManager.initializeFromPayload({
      agent_c_token: agentCData.agent_c_token,
      websocket_url: agentCData.websocket_url,
      heygen_token: agentCData.heygen_token,
      agents: agentCData.agents,
      voices: agentCData.voices,
      avatars: agentCData.avatars,
      expires_at: agentCData.expires_at
    });
    
    // Create realtime client
    this.client = new RealtimeClient({
      apiUrl: agentCData.websocket_url,
      authManager: this.authManager,
      autoReconnect: true
    });
    
    // Set up token refresh
    this.authManager.on('token-expiring', async () => {
      await this.refreshToken();
    });
    
    // Connect
    await this.client.connect();
  }
  
  private async refreshToken() {
    const response = await fetch('/api/auth/refresh', {
      method: 'POST',
      headers: {
        'Authorization': `Bearer ${localStorage.getItem('session_token')}`
      }
    });
    
    if (!response.ok) {
      throw new Error('Token refresh failed');
    }
    
    const data = await response.json();
    await this.authManager.updateTokens({
      agent_c_token: data.agent_c.agent_c_token,
      expires_at: data.agent_c.expires_at
    });
  }
  
  getClient(): RealtimeClient | undefined {
    return this.client;
  }
}

// frontend/src/App.tsx
import React, { useState, useEffect } from 'react';
import { AgentCProvider } from '@agentc/realtime-react';
import { AgentCService } from './services/AgentCService';

function App() {
  const [agentCService, setAgentCService] = useState<AgentCService>();
  const [isAuthenticated, setIsAuthenticated] = useState(false);
  
  const handleLogin = async (email: string, password: string) => {
    // Login through YOUR backend
    const response = await fetch('/api/auth/login', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ email, password })
    });
    
    if (!response.ok) {
      throw new Error('Login failed');
    }
    
    const data = await response.json();
    
    // Store your session
    localStorage.setItem('session_token', data.session.token);
    
    // Initialize Agent C with tokens from your backend
    const service = new AgentCService();
    await service.initialize(data.agent_c);
    
    setAgentCService(service);
    setIsAuthenticated(true);
  };
  
  if (!isAuthenticated) {
    return <LoginForm onLogin={handleLogin} />;
  }
  
  return (
    <AgentCProvider client={agentCService?.getClient()}>
      {/* Your app components */}
    </AgentCProvider>
  );
}
```

## Environment Configuration

### Backend Environment Variables

```bash
# backend/.env (NEVER commit to version control)

# Your application database
DATABASE_URL=postgresql://user:pass@localhost/myapp

# Your application secrets
APP_SECRET_KEY=your-app-secret-key
JWT_SECRET=your-jwt-secret

# Agent C server configuration (backend only)
AGENTC_API_URL=https://agentc-server.internal
AGENTC_ADMIN_USERNAME=admin_user
AGENTC_ADMIN_PASSWORD=secure_admin_password

# Never expose these to frontend!
```

### Frontend Environment Variables

```bash
# frontend/.env

# Your backend API (not Agent C directly)
REACT_APP_API_URL=https://api.yourapp.com

# Public configuration only
REACT_APP_NAME=My Application
REACT_APP_VERSION=1.0.0

# NO Agent C credentials here!
```

### Docker Secrets (Production)

```yaml
# docker-compose.yml
version: '3.8'

services:
  backend:
    image: myapp/backend
    environment:
      - DATABASE_URL
      - APP_SECRET_KEY
    secrets:
      - agentc_admin_username
      - agentc_admin_password
    
secrets:
  agentc_admin_username:
    external: true
  agentc_admin_password:
    external: true
```

### Kubernetes Secrets (Production)

```yaml
# k8s/secrets.yaml
apiVersion: v1
kind: Secret
metadata:
  name: agentc-credentials
type: Opaque
data:
  admin-username: <base64-encoded-username>
  admin-password: <base64-encoded-password>
---
apiVersion: v1
kind: ConfigMap
metadata:
  name: agentc-config
data:
  api-url: "https://agentc-server.internal"
```

## Error Handling

### Backend Error Handling

```typescript
// backend/middleware/errorHandler.ts
export class AgentCErrorHandler {
  async handleChatUserCreation(appUserId: string) {
    try {
      return await this.createChatUser(appUserId);
    } catch (error) {
      if (error.code === 'QUOTA_EXCEEDED') {
        // Your ChatUser quota is exhausted
        console.error('ChatUser quota exceeded');
        // Fallback to shared pool
        return await this.assignFromSharedPool(appUserId);
      }
      
      if (error.code === 'AGENT_C_UNAVAILABLE') {
        // Agent C server is down
        console.error('Agent C unavailable');
        // Queue for retry
        await this.queueForRetry(appUserId);
        throw new Error('Service temporarily unavailable');
      }
      
      throw error;
    }
  }
  
  async handleTokenGeneration(chatUserId: string) {
    try {
      return await this.agentC.generateToken(chatUserId);
    } catch (error) {
      if (error.code === 'CHATUSER_NOT_FOUND') {
        // ChatUser was deleted or doesn't exist
        console.error(`ChatUser ${chatUserId} not found`);
        // Recreate the ChatUser
        const newChatUser = await this.recreateChatUser(chatUserId);
        return await this.agentC.generateToken(newChatUser.id);
      }
      
      if (error.code === 'INVALID_CREDENTIALS') {
        // Admin credentials are wrong
        console.error('Agent C admin credentials invalid');
        // Alert ops team
        await this.alertOperations('AGENTC_AUTH_FAILURE');
        throw new Error('Internal configuration error');
      }
      
      throw error;
    }
  }
}
```

### Frontend Error Handling

```typescript
// frontend/src/services/ErrorHandler.ts
export class AuthErrorHandler {
  async handleLoginError(error: any) {
    // Check if it's a network error
    if (!navigator.onLine) {
      return {
        type: 'network',
        message: 'No internet connection',
        retry: true
      };
    }
    
    // Check HTTP status from your backend
    if (error.status === 401) {
      return {
        type: 'auth',
        message: 'Invalid email or password',
        retry: false
      };
    }
    
    if (error.status === 429) {
      const retryAfter = error.headers?.get('Retry-After') || 60;
      return {
        type: 'rate_limit',
        message: `Too many attempts. Try again in ${retryAfter} seconds`,
        retry: true,
        retryAfter
      };
    }
    
    if (error.status === 503) {
      return {
        type: 'maintenance',
        message: 'Service under maintenance. Please try again later',
        retry: true
      };
    }
    
    // Generic error
    return {
      type: 'unknown',
      message: 'Something went wrong. Please try again',
      retry: true
    };
  }
  
  async handleTokenRefreshError(error: any) {
    // If refresh fails, user needs to login again
    if (error.status === 401) {
      // Session expired
      localStorage.removeItem('session_token');
      window.location.href = '/login';
      return;
    }
    
    // Try again with exponential backoff
    await this.retryWithBackoff(async () => {
      return await this.refreshToken();
    });
  }
  
  private async retryWithBackoff(
    fn: () => Promise<any>,
    maxRetries = 3
  ) {
    let retries = 0;
    
    while (retries < maxRetries) {
      try {
        return await fn();
      } catch (error) {
        retries++;
        if (retries >= maxRetries) {
          throw error;
        }
        
        const delay = Math.pow(2, retries) * 1000;
        await new Promise(resolve => setTimeout(resolve, delay));
      }
    }
  }
}
```

## Security Best Practices

### 1. Backend Security

```typescript
// backend/security/best-practices.ts

// ✅ CORRECT - ChatUser credentials only on backend
export class SecureAgentCManager {
  private credentials: EncryptedCredentials;
  
  constructor() {
    // Load from secure storage (e.g., AWS Secrets Manager, Vault)
    this.credentials = this.loadFromSecureStorage();
  }
  
  private loadFromSecureStorage(): EncryptedCredentials {
    // Use proper secret management
    return {
      username: process.env.AGENTC_ADMIN_USER,
      password: process.env.AGENTC_ADMIN_PASS,
      // Encrypt at rest
      encrypted: true,
      keyId: process.env.ENCRYPTION_KEY_ID
    };
  }
  
  // Never expose credentials in API responses
  async generateUserToken(userId: string): Promise<PublicTokenData> {
    const chatUser = await this.getChatUser(userId);
    const token = await this.generateToken(chatUser.id);
    
    // Return only what frontend needs
    return {
      token: token.jwt,
      websocket_url: token.ws_url,
      expires_at: token.expires_at
      // NO credentials, NO internal IDs
    };
  }
}

// ❌ WRONG - Never do this
export class InsecureManager {
  getCredentials() {
    // Never expose ChatUser credentials to API
    return {
      username: this.username,  // NO!
      password: this.password   // NO!
    };
  }
}
```

### 2. Frontend Security

```typescript
// frontend/security/best-practices.ts

// ✅ CORRECT - Frontend only handles tokens
export class SecureFrontendAuth {
  private authManager: AuthManager;
  
  async initialize(backendResponse: any) {
    // Only use tokens from YOUR backend
    await this.authManager.initializeFromPayload({
      agent_c_token: backendResponse.agent_c.token,
      websocket_url: backendResponse.agent_c.websocket_url,
      expires_at: backendResponse.agent_c.expires_at
    });
  }
  
  // ❌ NEVER store ChatUser credentials in frontend
  // ❌ NEVER call Agent C login endpoint directly
  // ❌ NEVER hardcode credentials in frontend code
}

// Token storage best practices
export class SecureTokenStorage {
  // Use memory for sensitive tokens
  private agentCToken?: string;
  
  setAgentCToken(token: string): void {
    // Memory only for Agent C token
    this.agentCToken = token;
    
    // DO NOT store in localStorage (XSS vulnerable)
    // Consider sessionStorage for less sensitive data
  }
  
  getAgentCToken(): string | undefined {
    return this.agentCToken;
  }
  
  clearTokens(): void {
    this.agentCToken = undefined;
  }
}
```

### 3. Network Security

```typescript
// Always use HTTPS/WSS
const config = {
  // ✅ Production
  production: {
    apiUrl: 'https://api.yourapp.com',
    wsUrl: 'wss://api.yourapp.com/ws'
  },
  
  // ✅ Development (HTTPS required for WebRTC/microphone)
  development: {
    apiUrl: 'https://localhost:3001',
    wsUrl: 'wss://localhost:3001/ws'
  }
};

// Implement CSP headers
app.use((req, res, next) => {
  res.setHeader(
    'Content-Security-Policy',
    "default-src 'self'; " +
    "connect-src 'self' wss://api.yourapp.com; " +
    "script-src 'self' 'unsafe-inline';"
  );
  next();
});
```

### 4. Audit and Monitoring

```typescript
// backend/audit/logger.ts
export class SecurityAuditLogger {
  logAuthentication(userId: string, result: 'success' | 'failure') {
    this.log({
      event: 'authentication',
      userId,
      result,
      timestamp: new Date().toISOString(),
      ip: this.getClientIp(),
      userAgent: this.getUserAgent()
    });
  }
  
  logChatUserAccess(appUserId: string, chatUserId: string) {
    this.log({
      event: 'chatuser_access',
      appUserId,
      chatUserId,
      timestamp: new Date().toISOString()
    });
  }
  
  logTokenGeneration(userId: string) {
    this.log({
      event: 'token_generated',
      userId,
      timestamp: new Date().toISOString()
    });
  }
}
```

## Testing Authentication

### Backend Testing

```typescript
// backend/tests/auth.test.ts
import { AgentCService } from '../services/AgentCService';
import { MockAgentCClient } from './mocks/MockAgentCClient';

describe('Backend Authentication', () => {
  let service: AgentCService;
  let mockAgentC: MockAgentCClient;
  
  beforeEach(() => {
    mockAgentC = new MockAgentCClient();
    service = new AgentCService(mockAgentC);
  });
  
  it('should create ChatUser for new app user', async () => {
    const appUserId = 'user-123';
    
    const chatUser = await service.getChatUserForAppUser(appUserId);
    
    expect(chatUser).toBeDefined();
    expect(chatUser.id).toBeDefined();
    expect(mockAgentC.createChatUser).toHaveBeenCalledWith(
      expect.objectContaining({
        metadata: expect.objectContaining({
          app_user_id: appUserId
        })
      })
    );
  });
  
  it('should generate token for existing user', async () => {
    const appUserId = 'user-123';
    
    // First call creates ChatUser
    await service.getChatUserForAppUser(appUserId);
    
    // Generate token
    const tokens = await service.generateTokenForUser(appUserId);
    
    expect(tokens.agent_c_token).toBeDefined();
    expect(tokens.websocket_url).toBeDefined();
    expect(tokens.expires_at).toBeGreaterThan(Date.now() / 1000);
  });
});
```

### Frontend Testing

```typescript
// frontend/tests/auth.test.tsx
import { render, waitFor } from '@testing-library/react';
import { AgentCProvider } from '@agentc/realtime-react';
import { MockAuthManager } from './mocks/MockAuthManager';

describe('Frontend Authentication', () => {
  it('should initialize with backend tokens', async () => {
    const mockAuth = new MockAuthManager();
    
    // Simulate backend response
    const backendResponse = {
      agent_c: {
        agent_c_token: 'test-token',
        websocket_url: 'wss://test.example.com/ws',
        expires_at: Date.now() / 1000 + 3600,
        agents: [{ id: 'agent-1', name: 'Test Agent' }],
        voices: [{ voice_id: 'nova', name: 'Nova' }]
      }
    };
    
    // Initialize with payload (production pattern)
    await mockAuth.initializeFromPayload(backendResponse.agent_c);
    
    expect(mockAuth.getToken()).toBe('test-token');
    expect(mockAuth.getWebSocketUrl()).toBe('wss://test.example.com/ws');
  });
  
  it('should NOT use direct login in production', () => {
    const mockAuth = new MockAuthManager();
    
    // This should warn in production
    const consoleSpy = jest.spyOn(console, 'warn');
    
    mockAuth.login({ username: 'test', password: 'test' });
    
    expect(consoleSpy).toHaveBeenCalledWith(
      expect.stringContaining('should only be used for development')
    );
  });
});
```

### Integration Testing

```typescript
// integration/auth-flow.test.ts
describe('Complete Authentication Flow', () => {
  it('should authenticate through backend and connect to Agent C', async () => {
    // 1. User logs in to your backend
    const loginResponse = await request(app)
      .post('/api/auth/login')
      .send({ email: 'test@example.com', password: 'password' });
    
    expect(loginResponse.status).toBe(200);
    expect(loginResponse.body.agent_c).toBeDefined();
    expect(loginResponse.body.agent_c.agent_c_token).toBeDefined();
    
    // 2. Frontend initializes with tokens
    const authManager = new AuthManager();
    await authManager.initializeFromPayload(loginResponse.body.agent_c);
    
    // 3. WebSocket connection established
    const client = new RealtimeClient({
      apiUrl: loginResponse.body.agent_c.websocket_url,
      authManager
    });
    
    await client.connect();
    expect(client.isConnected()).toBe(true);
  });
});
```

## Troubleshooting

### Common Issues and Solutions

#### Backend Issues

**"Cannot create ChatUser"**
- Check Agent C admin credentials are correct
- Verify ChatUser quota hasn't been exceeded
- Ensure Agent C server is accessible from backend
- Check network connectivity between services

**"Token generation fails"**
- Verify ChatUser exists and is active
- Check admin permissions for token generation
- Ensure Agent C API is responding
- Review error logs for specific error codes

**"ChatUser mapping lost"**
- Implement proper database backups
- Add redundancy to user mapping storage
- Consider caching strategies
- Implement ChatUser recreation logic

#### Frontend Issues

**"WebSocket connection fails"**
- Verify token is valid and not expired
- Use the WebSocket URL from backend response
- Check for proxy/firewall blocking WebSocket
- Ensure CORS is properly configured

**"Token expired during session"**
- Implement automatic token refresh through backend
- Check token expiry handling in AuthManager
- Verify backend refresh endpoint is working
- Monitor token expiry times

**"No agents/voices available"**
- Check ChatUser has assigned resources
- Verify backend is passing resource lists
- Review ChatUser permissions in Agent C
- Ensure resources are included in token payload

### Debug Logging

#### Backend Debug Logging

```typescript
// backend/debug/auth-logger.ts
export class AuthDebugLogger {
  logChatUserCreation(appUserId: string, chatUserId: string) {
    console.log('[Backend Auth] Creating ChatUser', {
      appUserId,
      chatUserId,
      timestamp: new Date().toISOString()
    });
  }
  
  logTokenGeneration(chatUserId: string, expiresIn: number) {
    console.log('[Backend Auth] Generating token', {
      chatUserId,
      expiresIn,
      expiresAt: new Date(Date.now() + expiresIn * 1000).toISOString()
    });
  }
  
  logAuthError(error: any, context: string) {
    console.error('[Backend Auth] Error', {
      context,
      error: error.message,
      code: error.code,
      stack: error.stack
    });
  }
}
```

#### Frontend Debug Logging

```typescript
// frontend/debug/auth-logger.ts
export class FrontendAuthDebugger {
  logInitialization(payload: any) {
    console.log('[Frontend Auth] Initializing with tokens', {
      hasToken: !!payload.agent_c_token,
      websocketUrl: payload.websocket_url,
      expiresAt: new Date(payload.expires_at * 1000).toISOString(),
      agentCount: payload.agents?.length || 0,
      voiceCount: payload.voices?.length || 0
    });
  }
  
  logConnectionAttempt(url: string) {
    console.log('[Frontend Auth] Connecting to WebSocket', {
      url,
      timestamp: new Date().toISOString()
    });
  }
  
  logTokenRefresh(success: boolean) {
    console.log('[Frontend Auth] Token refresh', {
      success,
      timestamp: new Date().toISOString()
    });
  }
}
```

### Development Tools

```bash
# Backend: Test Agent C connectivity
curl -X POST https://agentc-server.internal/rt/login \
  -H "Content-Type: application/json" \
  -d '{"username": "admin_user", "password": "admin_pass"}'

# Backend: Verify ChatUser creation
node scripts/test-chatuser-creation.js --user-id test-user

# Frontend: Test token initialization
npm run test:auth -- --debug

# Integration: Full flow test
npm run test:integration:auth
```

## Summary

### The Correct Architecture

✅ **Production Architecture**:
1. **Your backend** manages ChatUsers and credentials
2. **Your backend** generates tokens for your users
3. **Frontend** receives tokens from your backend
4. **Frontend** never sees ChatUser credentials
5. **Your app** owns the user experience and security

❌ **What NOT to do**:
1. Don't expose ChatUser credentials to frontend
2. Don't have frontend call Agent C login directly
3. Don't store credentials in frontend code
4. Don't bypass your backend authentication

### Key Takeaways

1. **Agent C is a platform** - Your application wraps it with its own authentication
2. **ChatUsers are managed by your backend** - Not exposed to end users
3. **Security through layers** - Each layer has specific responsibilities
4. **Token-based frontend** - Frontend only handles tokens, never credentials
5. **Flexible user models** - Support any user architecture your app needs

### Quick Reference

| Component | Has Access To | Never Has |
|-----------|--------------|----------|
| Frontend | JWT tokens, WebSocket URL | ChatUser credentials |
| Your Backend | ChatUser credentials, Admin access | - |
| Agent C | ChatUser data, Resources | Your app's user data |

### Migration from Direct Login

If you're currently using direct login in production:

1. **Phase 1**: Move credentials to backend environment variables
2. **Phase 2**: Create backend endpoints for authentication
3. **Phase 3**: Update frontend to use `initializeFromPayload()`
4. **Phase 4**: Remove `login()` calls from frontend
5. **Phase 5**: Implement proper token refresh through backend

Remember: The goal is to have your application own the authentication flow while leveraging Agent C as a powerful backend service. Your users should never know that Agent C exists - they only interact with your application.