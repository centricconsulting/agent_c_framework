# AuthManager API Reference

The `AuthManager` class handles authentication with the Agent C platform, manages JWT tokens, and provides access to configuration like available voices and avatars.

## Import

```typescript
import { AuthManager } from '@agentc/realtime-core';
```

## Constructor

```typescript
constructor(config: AuthConfig)
```

Creates a new AuthManager instance.

### Parameters

- `config` (AuthConfig) - Configuration object

### Configuration Options

```typescript
interface AuthConfig {
  apiUrl: string;           // Base API URL (e.g., 'https://localhost:8000')
  tokenRefreshBuffer?: number; // Minutes before expiry to refresh (default: 5)
  enableAutoRefresh?: boolean; // Auto-refresh tokens (default: true)
  debug?: boolean;          // Enable debug logging (default: false)
}
```

### Example

```typescript
const authManager = new AuthManager({
  apiUrl: 'https://localhost:8000',
  tokenRefreshBuffer: 5,
  enableAutoRefresh: true,
  debug: true
});
```

## Authentication Methods

### login()

Authenticates with the Agent C platform using username and password credentials.

```typescript
async login(credentials: LoginCredentials): Promise<LoginResponse>
```

**Parameters:**
- `credentials` (LoginCredentials) - User credentials

```typescript
interface LoginCredentials {
  username: string;
  password: string;
}
```

**Returns:** Promise resolving to LoginResponse

```typescript
interface LoginResponse {
  agent_c_token: string;    // JWT for WebSocket connection
  heygen_token?: string;    // HeyGen access token (if available)
  websocket_url: string;    // WebSocket endpoint URL
  ui_session_id: string;    // UI session identifier
  user: User;               // User information
  agents: Agent[];          // Available agents
  voices: Voice[];          // Available TTS voices
  avatars?: Avatar[];       // Available avatars (if configured)
  tools: Tool[];            // Available tools
  sessions: ChatSession[];  // User's chat sessions
  expires_at: number;       // Token expiration timestamp
}

interface User {
  user_id: string;
  username: string;
  email?: string;
}

interface Agent {
  agent_id: string;
  name: string;
  description?: string;
}

interface Voice {
  voice_id: string;         // Voice identifier (e.g., 'nova', 'echo', 'avatar', 'none')
  name: string;             // Display name
  vendor: string;           // Provider (e.g., 'openai')
  description?: string;     // Voice description
  output_format?: string;   // Audio format (e.g., 'pcm16')
}

interface Avatar {
  avatar_id: string;        // Avatar identifier
  name: string;             // Display name
  preview_url?: string;     // Preview image URL
}

interface Tool {
  tool_id: string;
  name: string;
  description?: string;
}

interface ChatSession {
  session_id: string;           // Unique session identifier
  token_count: number;          // Total tokens used in session
  context_window_size: number;  // Maximum context window size
  session_name: string | null;  // Optional user-friendly session name
  created_at: string | null;    // ISO timestamp of creation
  updated_at: string | null;    // ISO timestamp of last update
  deleted_at: string | null;    // ISO timestamp of deletion (if soft-deleted)
  user_id: string | null;       // User who owns the session
  metadata: Record<string, any>; // Additional session metadata
  messages: Message[];          // Chat history messages
  agent_config: AgentConfiguration; // Agent configuration for this session
}
```

**Endpoint:** `POST /rt/login`

**Example:**
```typescript
try {
  const response = await authManager.login({
    username: 'myuser',
    password: 'mypassword'
  });
  console.log('Authenticated! Token:', response.agent_c_token);
  console.log('WebSocket URL:', response.websocket_url);
  console.log('Available voices:', response.voices);
  console.log('Available agents:', response.agents);
} catch (error) {
  console.error('Login failed:', error);
}
```

### refreshTokens()

Manually refreshes the authentication tokens.

```typescript
async refreshTokens(): Promise<TokenPair>
```

**Endpoint:** `GET /rt/refresh_token` with Bearer token

**Returns:** Promise resolving to new token pair

```typescript
interface TokenPair {
  agentCToken: string;
  heygenToken?: string;
  expiresAt: number;
}
```

**Note:** Usually called automatically before token expiry

**Example:**
```typescript
try {
  const tokens = await authManager.refreshTokens();
  console.log('Tokens refreshed, expires at:', new Date(tokens.expiresAt));
} catch (error) {
  console.error('Token refresh failed:', error);
}
```

### logout()

Clears stored tokens and stops auto-refresh.

```typescript
logout(): void
```

**Example:**
```typescript
authManager.logout();
console.log('Logged out');
```

## Token Management

### getTokens()

Gets the current token pair.

```typescript
getTokens(): TokenPair | null
```

**Returns:** Current tokens or null if not authenticated

**Example:**
```typescript
const tokens = authManager.getTokens();
if (tokens) {
  console.log('Agent C Token:', tokens.agentCToken);
  console.log('Expires:', new Date(tokens.expiresAt));
} else {
  console.log('Not authenticated');
}
```

### getAgentCToken()

Gets just the Agent C JWT token.

```typescript
getAgentCToken(): string | null
```

**Returns:** JWT token or null

**Example:**
```typescript
const token = authManager.getAgentCToken();
if (token) {
  // Use token for API calls
  headers['Authorization'] = `Bearer ${token}`;
}
```

### getHeyGenToken()

Gets the HeyGen access token for avatar sessions.

```typescript
getHeyGenToken(): string | null
```

**Returns:** HeyGen token or null

**Example:**
```typescript
const heygenToken = authManager.getHeyGenToken();
if (heygenToken) {
  // Use for HeyGen avatar creation
}
```

### isAuthenticated()

Checks if currently authenticated with valid tokens.

```typescript
isAuthenticated(): boolean
```

**Returns:** `true` if authenticated with non-expired tokens

**Example:**
```typescript
if (authManager.isAuthenticated()) {
  // Proceed with authenticated operations
} else {
  // Need to login
  await authManager.login({
    username: 'myuser',
    password: 'mypassword'
  });
}
```

### getTokenExpiry()

Gets the token expiration time.

```typescript
getTokenExpiry(): Date | null
```

**Returns:** Expiration date or null if not authenticated

**Example:**
```typescript
const expiry = authManager.getTokenExpiry();
if (expiry) {
  console.log('Token expires at:', expiry);
  const remaining = expiry.getTime() - Date.now();
  console.log('Minutes remaining:', Math.floor(remaining / 60000));
}
```

## Configuration Access

### getVoices()

Gets the list of available TTS voices.

```typescript
getVoices(): Voice[]
```

**Returns:** Array of available voices

**Special Voice IDs:**
- `none` - Text-only mode, no audio output
- `avatar` - Audio handled by HeyGen avatar
- Other IDs - Specific TTS voices (e.g., 'nova', 'echo')

**Example:**
```typescript
const voices = authManager.getVoices();
voices.forEach(voice => {
  console.log(`${voice.name} (${voice.voice_id}): ${voice.description}`);
});

// Find specific voice
const nova = voices.find(v => v.voice_id === 'nova');

// Check for special modes
const textOnly = voices.find(v => v.voice_id === 'none');
const avatarMode = voices.find(v => v.voice_id === 'avatar');
```

### getAvatars()

Gets the list of available avatars.

```typescript
getAvatars(): Avatar[]
```

**Returns:** Array of available avatars

**Example:**
```typescript
const avatars = authManager.getAvatars();
avatars.forEach(avatar => {
  console.log(`${avatar.name}: ${avatar.avatar_id}`);
  if (avatar.preview_url) {
    console.log('Preview:', avatar.preview_url);
  }
});
```

### getAgents()

Gets the list of available agents.

```typescript
getAgents(): Agent[]
```

**Returns:** Array of available agents

**Example:**
```typescript
const agents = authManager.getAgents();
agents.forEach(agent => {
  console.log(`${agent.name} (${agent.agent_id}): ${agent.description}`);
});
```

### getSessions()

Gets the list of user's chat sessions.

```typescript
getSessions(): ChatSession[]
```

**Returns:** Array of user's chat sessions

**Example:**
```typescript
const sessions = authManager.getSessions();
console.log(`Found ${sessions.length} sessions`);

sessions.forEach(session => {
  console.log(`Session: ${session.session_name || session.session_id}`);
  console.log(`  Created: ${session.created_at}`);
  console.log(`  Messages: ${session.messages.length}`);
  console.log(`  Tokens used: ${session.token_count}`);
});

// Find recent sessions
const recentSessions = sessions
  .filter(s => !s.deleted_at)
  .sort((a, b) => {
    const dateA = new Date(a.updated_at || a.created_at || 0);
    const dateB = new Date(b.updated_at || b.created_at || 0);
    return dateB.getTime() - dateA.getTime();
  })
  .slice(0, 10);

console.log(`Most recent ${recentSessions.length} sessions:`);
recentSessions.forEach(session => {
  console.log(`- ${session.session_name || 'Untitled'} (${session.session_id})`);
});
```

### getWebSocketUrl()

Gets the WebSocket URL for realtime connections.

```typescript
getWebSocketUrl(): string | null
```

**Returns:** WebSocket URL or null

**Example:**
```typescript
const wsUrl = authManager.getWebSocketUrl();
if (wsUrl) {
  console.log('WebSocket endpoint:', wsUrl);
  // Use for RealtimeClient connection
}
```

### getUiSessionId()

Gets the UI session identifier.

```typescript
getUiSessionId(): string | null
```

**Returns:** UI session ID or null

**Example:**
```typescript
const sessionId = authManager.getUiSessionId();
console.log('UI Session:', sessionId);
```

### getLoginResponse()

Gets the complete login response data.

```typescript
getLoginResponse(): LoginResponse | null
```

**Returns:** Full login response or null

**Example:**
```typescript
const loginData = authManager.getLoginResponse();
if (loginData) {
  console.log('User:', loginData.user);
  console.log('Available agents:', loginData.agents.length);
  console.log('Available tools:', loginData.tools.length);
}
```

## Auto-Refresh Configuration

### setAutoRefresh()

Enables or disables automatic token refresh.

```typescript
setAutoRefresh(enabled: boolean): void
```

**Parameters:**
- `enabled` (boolean) - Whether to enable auto-refresh

**Example:**
```typescript
// Disable auto-refresh
authManager.setAutoRefresh(false);

// Re-enable auto-refresh
authManager.setAutoRefresh(true);
```

### setTokenRefreshBuffer()

Sets how many minutes before expiry to refresh tokens.

```typescript
setTokenRefreshBuffer(minutes: number): void
```

**Parameters:**
- `minutes` (number) - Minutes before expiry to refresh

**Example:**
```typescript
// Refresh tokens 10 minutes before expiry
authManager.setTokenRefreshBuffer(10);
```

### stopAutoRefresh()

Stops the automatic token refresh timer.

```typescript
stopAutoRefresh(): void
```

**Example:**
```typescript
authManager.stopAutoRefresh();
```

## Event Handling

AuthManager extends EventEmitter and emits events for token lifecycle:

### Events

- `auth:login` - Successful login
- `auth:logout` - Logout occurred
- `auth:tokens-refreshed` - Tokens refreshed
- `auth:token-expired` - Token expired
- `auth:error` - Authentication error

### on()

Subscribe to authentication events.

```typescript
on(event: string, handler: Function): void
```

**Example:**
```typescript
authManager.on('auth:login', (response: LoginResponse) => {
  console.log('Logged in successfully');
  console.log('User:', response.user.username);
  console.log('Available voices:', response.voices.length);
  console.log('Available agents:', response.agents.length);
});

authManager.on('auth:tokens-refreshed', (tokens: TokenPair) => {
  console.log('Tokens refreshed, new expiry:', new Date(tokens.expiresAt));
});

authManager.on('auth:token-expired', () => {
  console.log('Token expired, need to re-authenticate');
});

authManager.on('auth:error', (error: Error) => {
  console.error('Auth error:', error.message);
});
```

### off()

Unsubscribe from events.

```typescript
off(event: string, handler: Function): void
```

### once()

Subscribe to an event for a single occurrence.

```typescript
once(event: string, handler: Function): void
```

## Complete Example

```typescript
import { AuthManager, RealtimeClient } from '@agentc/realtime-core';

async function setupAuthentication() {
  // Create auth manager
  const authManager = new AuthManager({
    apiUrl: 'https://localhost:8000',
    tokenRefreshBuffer: 5,
    enableAutoRefresh: true,
    debug: true
  });
  
  // Subscribe to events
  authManager.on('auth:login', (response) => {
    console.log('✅ Authenticated successfully');
    console.log(`👤 Logged in as: ${response.user.username}`);
    console.log(`🤖 ${response.agents.length} agents available`);
    console.log(`📢 ${response.voices.length} voices available`);
    if (response.avatars) {
      console.log(`🎭 ${response.avatars.length} avatars available`);
    }
    console.log(`💬 ${response.sessions.length} previous sessions`);
  });
  
  authManager.on('auth:tokens-refreshed', (tokens) => {
    console.log('🔄 Tokens refreshed automatically');
    const remaining = tokens.expiresAt - Date.now();
    console.log(`⏰ Next refresh in ${Math.floor(remaining / 60000)} minutes`);
  });
  
  authManager.on('auth:token-expired', () => {
    console.log('⚠️ Token expired - re-authenticating...');
    // Re-authenticate
    authManager.login({
      username: process.env.AGENTC_USERNAME!,
      password: process.env.AGENTC_PASSWORD!
    });
  });
  
  authManager.on('auth:error', (error) => {
    console.error('❌ Authentication error:', error.message);
  });
  
  // Perform login
  try {
    const response = await authManager.login({
      username: process.env.AGENTC_USERNAME!,
      password: process.env.AGENTC_PASSWORD!
    });
    
    // Display user info
    console.log('\nAuthenticated User:');
    console.log(`  Username: ${response.user.username}`);
    console.log(`  User ID: ${response.user.user_id}`);
    
    // Display available agents
    console.log('\nAvailable Agents:');
    response.agents.forEach(agent => {
      console.log(`  - ${agent.name} (${agent.agent_id})`);
      if (agent.description) {
        console.log(`    ${agent.description}`);
      }
    });
    
    // Display available voices
    console.log('\nAvailable Voices:');
    response.voices.forEach(voice => {
      console.log(`  - ${voice.name} (${voice.voice_id})`);
      console.log(`    Vendor: ${voice.vendor}`);
      console.log(`    Format: ${voice.output_format || 'default'}`);
    });
    
    if (response.avatars && response.avatars.length > 0) {
      console.log('\nAvailable Avatars:');
      response.avatars.forEach(avatar => {
        console.log(`  - ${avatar.name} (${avatar.avatar_id})`);
      });
    }
    
    // Display user's chat sessions
    if (response.sessions && response.sessions.length > 0) {
      console.log('\nPrevious Chat Sessions:');
      const recentSessions = response.sessions
        .filter(s => !s.deleted_at)
        .slice(0, 5);
      recentSessions.forEach(session => {
        console.log(`  - ${session.session_name || 'Untitled'} (${session.session_id})`);
        console.log(`    Messages: ${session.messages.length}, Tokens: ${session.token_count}`);
      });
    }
    
    // Check authentication status
    console.log('\nAuthentication Status:');
    console.log('Authenticated:', authManager.isAuthenticated());
    console.log('Token expires:', authManager.getTokenExpiry());
    console.log('WebSocket URL:', authManager.getWebSocketUrl());
    console.log('UI Session:', authManager.getUiSessionId());
    
    // Create realtime client with auth manager
    const client = new RealtimeClient({
      apiUrl: response.websocket_url, // Use the WebSocket URL from login
      authManager,  // Pass auth manager to client
      enableAudio: true
    });
    
    // Connect using managed authentication
    await client.connect();
    console.log('✅ Connected to realtime API');
    
    // The client will automatically use tokens from auth manager
    // and reconnect when tokens are refreshed
    
    return { authManager, client };
    
  } catch (error) {
    console.error('Login failed:', error);
    throw error;
  }
}

// Usage
setupAuthentication()
  .then(({ client }) => {
    // Use the authenticated client
    client.sendText('Hello!');
  })
  .catch(console.error);
```

## Token Refresh Lifecycle

The AuthManager automatically handles token refresh:

```typescript
// Token refresh timeline example
// Assuming token expires in 60 minutes and buffer is 5 minutes

Time 0:    Login successful, token expires at Time 60
Time 55:   Auto-refresh triggered (5 minutes before expiry)
Time 55:   GET /rt/refresh_token with Bearer token
Time 55:   New token obtained, expires at Time 115
Time 110:  Auto-refresh triggered again
Time 110:  GET /rt/refresh_token with Bearer token
Time 110:  New token obtained, expires at Time 170
...continues until logout or error
```

## Error Handling

The AuthManager can throw or emit errors for:

1. **Network Errors**
   - Connection failures
   - Timeout errors

2. **Authentication Errors**
   - Invalid credentials
   - Expired tokens
   - Permission denied

3. **Configuration Errors**
   - Invalid API URL
   - Missing required config

Example error handling:

```typescript
// Handle login errors
try {
  await authManager.login({
    username: 'myuser',
    password: 'mypassword'
  });
} catch (error) {
  if (error.response?.status === 401) {
    console.error('Invalid username or password');
  } else if (error.code === 'ECONNREFUSED') {
    console.error('Cannot connect to API');
  } else {
    console.error('Login failed:', error.message);
  }
}

// Handle refresh errors
authManager.on('auth:error', (error) => {
  if (error.message.includes('refresh')) {
    console.error('Token refresh failed, need to re-login');
    // Attempt re-login
    authManager.login({
      username: process.env.AGENTC_USERNAME!,
      password: process.env.AGENTC_PASSWORD!
    });
  }
});
```

## Best Practices

1. **Always check authentication before operations:**
```typescript
if (!authManager.isAuthenticated()) {
  await authManager.login({
    username: process.env.AGENTC_USERNAME!,
    password: process.env.AGENTC_PASSWORD!
  });
}
```

2. **Store credentials securely:**
```typescript
// Use environment variables
const credentials = {
  username: process.env.AGENTC_USERNAME!,
  password: process.env.AGENTC_PASSWORD!
};

// Never hardcode credentials
// BAD: authManager.login({ username: 'user', password: 'pass123' })
// GOOD: authManager.login(credentials)
```

3. **Handle token expiration gracefully:**
```typescript
authManager.on('auth:token-expired', async () => {
  try {
    await authManager.login(credentials);
    // Reconnect client if needed
  } catch (error) {
    // Handle re-auth failure
  }
});
```

4. **Use with RealtimeClient for automatic token management:**
```typescript
const client = new RealtimeClient({
  authManager,  // Pass manager, not token
  // Client will use fresh tokens automatically
});
```

5. **Clean up on application exit:**
```typescript
process.on('SIGINT', () => {
  authManager.logout();
  authManager.stopAutoRefresh();
  process.exit(0);
});
```

## Authentication Flow

```
1. Client calls authManager.login({ username, password })
2. POST /rt/login with credentials
3. Server validates and returns:
   - agent_c_token (JWT)
   - heygen_token
   - websocket_url
   - user info
   - available agents, voices, avatars, tools
4. AuthManager stores tokens and metadata
5. Before token expiry (based on buffer):
   - GET /rt/refresh_token with Bearer token
   - Server returns new tokens
   - AuthManager updates stored tokens
6. On logout, tokens are cleared
```

## TypeScript Types

```typescript
import {
  AuthManager,
  AuthConfig,
  LoginCredentials,
  LoginResponse,
  TokenPair,
  Voice,
  Avatar,
  Agent,
  User,
  Tool
} from '@agentc/realtime-core';
```

All methods and events are fully typed for TypeScript applications.