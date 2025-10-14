# Complete Initialization Examples

This document provides complete, working examples demonstrating the new authentication and initialization patterns in Agent C Realtime SDK v2.0.0.

## Table of Contents

1. [Basic Connection and Initialization](#basic-connection-and-initialization)
2. [React Application with Hooks](#react-application-with-hooks)
3. [Production Authentication Pattern](#production-authentication-pattern)
4. [Progressive Loading UI](#progressive-loading-ui)
5. [Voice-First Application](#voice-first-application)
6. [Multi-Session Chat](#multi-session-chat)

## Basic Connection and Initialization

The simplest example showing the new event-based initialization:

```typescript
import { AuthManager, RealtimeClient } from '@agentc/realtime-core';

async function connectToAgentC() {
  // Step 1: Create auth manager
  const authManager = new AuthManager({
    apiUrl: 'https://api.agentc.ai'
  });
  
  // Step 2: Login (returns only tokens now)
  const loginResponse = await authManager.login({
    username: 'your-username',
    password: 'your-password'
  });
  
  console.log('Login response (simplified):', {
    hasToken: !!loginResponse.jwt_token,
    hasWebSocketUrl: !!loginResponse.websocket_url,
    // Note: No user data, voices, agents, etc. in response anymore
  });
  
  // Step 3: Create client
  const client = new RealtimeClient({
    apiUrl: authManager.getWebSocketUrl(),
    authManager: authManager
  });
  
  // Step 4: Set up event listeners for initialization data
  const initializationData = {
    user: null,
    voices: [],
    agents: [],
    avatars: [],
    tools: [],
    session: null
  };
  
  client.on('chat_user_data', (event) => {
    console.log('✅ User data received:', event.user);
    initializationData.user = event.user;
  });
  
  client.on('voice_list', (event) => {
    console.log('✅ Voices received:', event.voices.length, 'voices');
    initializationData.voices = event.voices;
  });
  
  client.on('agent_list', (event) => {
    console.log('✅ Agents received:', event.agents.length, 'agents');
    initializationData.agents = event.agents;
  });
  
  client.on('avatar_list', (event) => {
    console.log('✅ Avatars received:', event.avatars.length, 'avatars');
    initializationData.avatars = event.avatars;
  });
  
  client.on('tool_catalog', (event) => {
    console.log('✅ Tools received:', event.tools.length, 'tools');
    initializationData.tools = event.tools;
  });
  
  client.on('chat_session_changed', (event) => {
    console.log('✅ Session received:', event.session.id);
    initializationData.session = event.session;
  });
  
  client.on('initialization:complete', () => {
    console.log('🎉 Initialization complete! Ready to chat.');
    console.log('Final data:', initializationData);
  });
  
  // Step 5: Connect (triggers automatic initialization)
  await client.connect();
  
  // Step 6: Wait for initialization to complete
  await new Promise<void>((resolve) => {
    client.on('initialization:complete', resolve);
  });
  
  // Step 7: Now ready to interact
  await client.sendText('Hello! I just connected using the new SDK.');
  
  return { client, initializationData };
}

// Run the example
connectToAgentC()
  .then(({ client, initializationData }) => {
    console.log('Connected successfully!');
    console.log('User:', initializationData.user?.display_name);
    console.log('Available voices:', initializationData.voices.map(v => v.name));
  })
  .catch(console.error);
```

## React Application with Hooks

Complete React application using the new hooks:

```tsx
import React, { useState, useEffect } from 'react';
import { AuthManager, RealtimeClient } from '@agentc/realtime-core';
import {
  AgentCProvider,
  useAgentCData,
  useInitializationStatus,
  useChat,
  useAudio,
  useVoiceModel,
  useConnection
} from '@agentc/realtime-react';

// Main App Component
function App() {
  const [client, setClient] = useState<RealtimeClient | null>(null);
  const [authError, setAuthError] = useState<string | null>(null);
  
  const handleLogin = async (username: string, password: string) => {
    try {
      setAuthError(null);
      
      // Create auth manager
      const authManager = new AuthManager({
        apiUrl: process.env.REACT_APP_API_URL || 'https://api.agentc.ai'
      });
      
      // Login - returns only tokens
      await authManager.login({ username, password });
      
      // Create client
      const newClient = new RealtimeClient({
        apiUrl: authManager.getWebSocketUrl(),
        authManager,
        audioConfig: {
          enableAudio: true,
          enableVAD: true
        }
      });
      
      // Connect - initialization events will follow
      await newClient.connect();
      setClient(newClient);
      
    } catch (error) {
      setAuthError(error.message);
    }
  };
  
  if (!client) {
    return (
      <LoginScreen 
        onLogin={handleLogin} 
        error={authError} 
      />
    );
  }
  
  return (
    <AgentCProvider client={client}>
      <MainApplication />
    </AgentCProvider>
  );
}

// Login Screen Component
function LoginScreen({ onLogin, error }: {
  onLogin: (username: string, password: string) => Promise<void>;
  error: string | null;
}) {
  const [username, setUsername] = useState('');
  const [password, setPassword] = useState('');
  const [isLoading, setIsLoading] = useState(false);
  
  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setIsLoading(true);
    await onLogin(username, password);
    setIsLoading(false);
  };
  
  return (
    <div className="login-screen">
      <h1>Agent C Login</h1>
      <form onSubmit={handleSubmit}>
        <input
          type="text"
          placeholder="Username"
          value={username}
          onChange={(e) => setUsername(e.target.value)}
          disabled={isLoading}
        />
        <input
          type="password"
          placeholder="Password"
          value={password}
          onChange={(e) => setPassword(e.target.value)}
          disabled={isLoading}
        />
        <button type="submit" disabled={isLoading}>
          {isLoading ? 'Logging in...' : 'Login'}
        </button>
        {error && <div className="error">{error}</div>}
      </form>
    </div>
  );
}

// Main Application Component
function MainApplication() {
  // Check initialization status
  const { isInitialized, isConnecting, error, initializationProgress } = useInitializationStatus();
  
  // Show connection status
  if (error) {
    return <ErrorScreen error={error} />;
  }
  
  if (isConnecting) {
    return <ConnectingScreen />;
  }
  
  if (!isInitialized) {
    return <InitializingScreen progress={initializationProgress} />;
  }
  
  // Fully initialized - show main UI
  return <ChatInterface />;
}

// Initializing Screen Component
function InitializingScreen({ progress }: {
  progress: {
    userData: boolean;
    voices: boolean;
    agents: boolean;
    avatars: boolean;
    tools: boolean;
    session: boolean;
  }
}) {
  const completedCount = Object.values(progress).filter(Boolean).length;
  const totalCount = Object.keys(progress).length;
  
  return (
    <div className="initializing-screen">
      <h2>Initializing Agent C</h2>
      <div className="progress-bar">
        <div 
          className="progress-fill" 
          style={{ width: `${(completedCount / totalCount) * 100}%` }}
        />
      </div>
      <ul className="progress-list">
        <li className={progress.userData ? 'complete' : 'pending'}>
          {progress.userData ? '✅' : '⏳'} Loading user data
        </li>
        <li className={progress.voices ? 'complete' : 'pending'}>
          {progress.voices ? '✅' : '⏳'} Loading voices
        </li>
        <li className={progress.agents ? 'complete' : 'pending'}>
          {progress.agents ? '✅' : '⏳'} Loading agents
        </li>
        <li className={progress.avatars ? 'complete' : 'pending'}>
          {progress.avatars ? '✅' : '⏳'} Loading avatars
        </li>
        <li className={progress.tools ? 'complete' : 'pending'}>
          {progress.tools ? '✅' : '⏳'} Loading tools
        </li>
        <li className={progress.session ? 'complete' : 'pending'}>
          {progress.session ? '✅' : '⏳'} Loading session
        </li>
      </ul>
    </div>
  );
}

// Chat Interface Component
function ChatInterface() {
  // Access all data from hooks
  const { user, voices, agents } = useAgentCData();
  const { messages, sendMessage, clearMessages, isAgentTyping } = useChat();
  const { voiceModel, setVoiceModel } = useVoiceModel();
  const { isRecording, startRecording, stopRecording, audioLevel } = useAudio();
  const { isConnected, connectionStats } = useConnection();
  
  const [inputText, setInputText] = useState('');
  
  const handleSendMessage = async () => {
    if (inputText.trim()) {
      await sendMessage(inputText);
      setInputText('');
    }
  };
  
  return (
    <div className="chat-interface">
      {/* Header */}
      <header className="chat-header">
        <div className="user-info">
          <span>👤 {user?.display_name || 'User'}</span>
          <span className={`connection-status ${isConnected ? 'connected' : 'disconnected'}`}>
            {isConnected ? '🟢 Connected' : '🔴 Disconnected'}
          </span>
          {connectionStats.latency && (
            <span className="latency">{connectionStats.latency}ms</span>
          )}
        </div>
        
        <div className="voice-selector">
          <label>Voice:</label>
          <select 
            value={voiceModel} 
            onChange={(e) => setVoiceModel(e.target.value)}
          >
            <option value="none">Text Only</option>
            {voices.map(voice => (
              <option key={voice.voice_id} value={voice.voice_id}>
                {voice.name}
              </option>
            ))}
          </select>
        </div>
      </header>
      
      {/* Messages */}
      <div className="messages-container">
        {messages.map((message, index) => (
          <div key={index} className={`message ${message.role}`}>
            <span className="role">{message.role}:</span>
            <span className="content">{message.content}</span>
          </div>
        ))}
        {isAgentTyping && (
          <div className="typing-indicator">
            Agent is typing<span className="dots">...</span>
          </div>
        )}
      </div>
      
      {/* Input Controls */}
      <div className="input-controls">
        <button 
          className={`voice-button ${isRecording ? 'recording' : ''}`}
          onClick={() => isRecording ? stopRecording() : startRecording()}
        >
          {isRecording ? '🔴' : '🎤'}
          {isRecording && (
            <div 
              className="audio-level" 
              style={{ width: `${audioLevel * 100}%` }}
            />
          )}
        </button>
        
        <input
          type="text"
          placeholder="Type a message..."
          value={inputText}
          onChange={(e) => setInputText(e.target.value)}
          onKeyPress={(e) => e.key === 'Enter' && handleSendMessage()}
        />
        
        <button onClick={handleSendMessage}>Send</button>
        <button onClick={clearMessages}>Clear</button>
      </div>
    </div>
  );
}

// Error Screen Component
function ErrorScreen({ error }: { error: Error }) {
  const { reconnect } = useConnection();
  
  return (
    <div className="error-screen">
      <h2>Connection Error</h2>
      <p>{error.message}</p>
      <button onClick={reconnect}>Retry Connection</button>
    </div>
  );
}

// Connecting Screen Component
function ConnectingScreen() {
  return (
    <div className="connecting-screen">
      <div className="spinner" />
      <h2>Connecting to Agent C...</h2>
    </div>
  );
}
```

## Production Authentication Pattern

Example showing how to integrate with your backend for production:

```typescript
// Frontend: AuthService.ts
export class AuthService {
  private authManager: AuthManager;
  private client?: RealtimeClient;
  
  constructor() {
    this.authManager = new AuthManager();
  }
  
  /**
   * Login through YOUR backend (production pattern)
   */
  async loginThroughBackend(email: string, password: string): Promise<void> {
    // Step 1: Authenticate with YOUR backend
    const response = await fetch('/api/auth/login', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ email, password })
    });
    
    if (!response.ok) {
      throw new Error('Login failed');
    }
    
    const data = await response.json();
    
    // Step 2: Store your app's session
    sessionStorage.setItem('app_session', data.session_token);
    
    // Step 3: Initialize Agent C with tokens from your backend
    await this.authManager.initializeFromPayload({
      agent_c_token: data.agent_c.jwt_token,
      websocket_url: data.agent_c.websocket_url,
      heygen_token: data.agent_c.heygen_token,
      expires_at: data.agent_c.expires_at
    });
    
    // Step 4: Create and connect client
    this.client = new RealtimeClient({
      apiUrl: this.authManager.getWebSocketUrl(),
      authManager: this.authManager
    });
    
    // Step 5: Set up token refresh
    this.authManager.on('token-expiring', async () => {
      await this.refreshToken();
    });
    
    await this.client.connect();
  }
  
  /**
   * Refresh token through YOUR backend
   */
  private async refreshToken(): Promise<void> {
    const response = await fetch('/api/auth/refresh-agent-token', {
      method: 'POST',
      headers: {
        'Authorization': `Bearer ${sessionStorage.getItem('app_session')}`
      }
    });
    
    if (!response.ok) {
      throw new Error('Token refresh failed');
    }
    
    const data = await response.json();
    
    await this.authManager.updateTokens({
      agent_c_token: data.agent_c.jwt_token,
      expires_at: data.agent_c.expires_at
    });
  }
  
  getClient(): RealtimeClient | undefined {
    return this.client;
  }
}

// Backend: Express endpoint example
app.post('/api/auth/login', async (req, res) => {
  const { email, password } = req.body;
  
  // 1. Authenticate YOUR user
  const user = await authenticateUser(email, password);
  if (!user) {
    return res.status(401).json({ error: 'Invalid credentials' });
  }
  
  // 2. Get Agent C tokens for this user
  // (Your backend manages ChatUser credentials)
  const agentCTokens = await agentCService.getTokensForUser(user.id);
  
  // 3. Return to frontend
  res.json({
    session_token: generateSessionToken(user),
    user: {
      id: user.id,
      email: user.email,
      name: user.name
    },
    agent_c: {
      jwt_token: agentCTokens.jwt_token,
      websocket_url: agentCTokens.websocket_url,
      heygen_token: agentCTokens.heygen_token,
      expires_at: agentCTokens.expires_at
    }
  });
});
```

## Progressive Loading UI

Example showing progressive UI updates as data arrives:

```tsx
import React, { useState, useEffect } from 'react';
import { useEventListener, useAgentCData } from '@agentc/realtime-react';

function ProgressiveLoadingApp() {
  const [loadingStates, setLoadingStates] = useState({
    user: 'loading',
    voices: 'loading',
    agents: 'loading',
    avatars: 'loading',
    tools: 'loading',
    session: 'loading'
  });
  
  // Listen to individual events for progressive loading
  useEventListener('chat_user_data', () => {
    setLoadingStates(prev => ({ ...prev, user: 'loaded' }));
  });
  
  useEventListener('voice_list', () => {
    setLoadingStates(prev => ({ ...prev, voices: 'loaded' }));
  });
  
  useEventListener('agent_list', () => {
    setLoadingStates(prev => ({ ...prev, agents: 'loaded' }));
  });
  
  useEventListener('avatar_list', () => {
    setLoadingStates(prev => ({ ...prev, avatars: 'loaded' }));
  });
  
  useEventListener('tool_catalog', () => {
    setLoadingStates(prev => ({ ...prev, tools: 'loaded' }));
  });
  
  useEventListener('chat_session_changed', () => {
    setLoadingStates(prev => ({ ...prev, session: 'loaded' }));
  });
  
  // Get actual data
  const { user, voices, agents, avatars, tools, currentSession } = useAgentCData();
  
  return (
    <div className="progressive-app">
      {/* User Section - Shows as soon as user data loads */}
      <section className="user-section">
        {loadingStates.user === 'loading' ? (
          <div className="skeleton user-skeleton" />
        ) : (
          <div className="user-card">
            <img src={user?.avatar_url || '/default-avatar.png'} />
            <h2>{user?.display_name}</h2>
            <p>{user?.email}</p>
          </div>
        )}
      </section>
      
      {/* Voice Section - Shows when voices load */}
      <section className="voice-section">
        <h3>Available Voices</h3>
        {loadingStates.voices === 'loading' ? (
          <div className="skeleton voice-skeleton" />
        ) : (
          <div className="voice-grid">
            {voices.map(voice => (
              <div key={voice.voice_id} className="voice-card">
                <span className="voice-name">{voice.name}</span>
                <span className="voice-lang">{voice.language}</span>
              </div>
            ))}
          </div>
        )}
      </section>
      
      {/* Agents Section - Shows when agents load */}
      <section className="agents-section">
        <h3>Available Agents</h3>
        {loadingStates.agents === 'loading' ? (
          <div className="skeleton agents-skeleton" />
        ) : (
          <div className="agents-list">
            {agents.map(agent => (
              <div key={agent.id} className="agent-card">
                <img src={agent.avatar_url} />
                <div className="agent-info">
                  <h4>{agent.name}</h4>
                  <p>{agent.description}</p>
                  <div className="agent-tools">
                    {agent.tools.map(tool => (
                      <span key={tool} className="tool-badge">{tool}</span>
                    ))}
                  </div>
                </div>
              </div>
            ))}
          </div>
        )}
      </section>
      
      {/* Session Info - Shows when session loads */}
      <section className="session-section">
        {loadingStates.session === 'loading' ? (
          <div className="skeleton session-skeleton" />
        ) : (
          <div className="session-info">
            <h3>Current Session</h3>
            <p>ID: {currentSession?.id}</p>
            <p>Messages: {currentSession?.message_count}</p>
            <p>Created: {new Date(currentSession?.created_at || 0).toLocaleString()}</p>
          </div>
        )}
      </section>
    </div>
  );
}
```

## Voice-First Application

Example of a voice-focused interface:

```tsx
import React, { useEffect, useState } from 'react';
import {
  useAudio,
  useTurnState,
  useChat,
  useVoiceModel,
  useInitializationStatus
} from '@agentc/realtime-react';

function VoiceFirstApp() {
  const { isInitialized } = useInitializationStatus();
  const { isRecording, startRecording, stopRecording, audioLevel, isVADActive } = useAudio();
  const { turnState, isUserTurn, isAgentTurn } = useTurnState();
  const { messages } = useChat();
  const { voiceModel, setVoiceModel, availableVoices } = useVoiceModel();
  
  const [isListening, setIsListening] = useState(false);
  
  // Auto-start recording when it's user's turn
  useEffect(() => {
    if (isUserTurn && isListening && !isRecording) {
      startRecording();
    } else if (!isUserTurn && isRecording) {
      stopRecording();
    }
  }, [isUserTurn, isListening, isRecording]);
  
  if (!isInitialized) {
    return <div className="voice-app-loading">Initializing voice assistant...</div>;
  }
  
  const toggleListening = () => {
    if (isListening) {
      setIsListening(false);
      if (isRecording) stopRecording();
    } else {
      setIsListening(true);
      if (isUserTurn) startRecording();
    }
  };
  
  return (
    <div className="voice-first-app">
      {/* Voice Model Selector */}
      <div className="voice-selector-bar">
        <label>Assistant Voice:</label>
        <select value={voiceModel} onChange={(e) => setVoiceModel(e.target.value)}>
          {availableVoices.map(voice => (
            <option key={voice.voice_id} value={voice.voice_id}>
              {voice.name} ({voice.gender})
            </option>
          ))}
        </select>
      </div>
      
      {/* Main Voice Interface */}
      <div className="voice-interface">
        {/* Status Display */}
        <div className="status-display">
          {isUserTurn && <div className="status user-turn">Your turn to speak</div>}
          {isAgentTurn && <div className="status agent-turn">Assistant is speaking</div>}
          {turnState === 'processing' && <div className="status processing">Processing...</div>}
          {turnState === 'idle' && <div className="status idle">Ready</div>}
        </div>
        
        {/* Voice Button */}
        <button 
          className={`voice-button ${isListening ? 'listening' : ''} ${isRecording ? 'recording' : ''}`}
          onClick={toggleListening}
        >
          <div className="voice-button-inner">
            {isListening ? (
              isRecording ? '🔴' : '⏸️'
            ) : (
              '🎤'
            )}
          </div>
          
          {/* Audio Level Indicator */}
          {isRecording && (
            <div className="audio-rings">
              <div 
                className="audio-ring" 
                style={{ 
                  transform: `scale(${1 + audioLevel * 0.5})`,
                  opacity: audioLevel 
                }}
              />
            </div>
          )}
          
          {/* VAD Indicator */}
          {isVADActive && (
            <div className="vad-indicator">Speaking detected</div>
          )}
        </button>
        
        <div className="button-label">
          {isListening ? 'Tap to stop' : 'Tap to start'}
        </div>
      </div>
      
      {/* Transcript */}
      <div className="transcript">
        <h3>Conversation</h3>
        <div className="transcript-messages">
          {messages.slice(-10).map((msg, i) => (
            <div key={i} className={`transcript-message ${msg.role}`}>
              <span className="role">{msg.role === 'user' ? 'You' : 'Assistant'}:</span>
              <span className="text">{msg.content}</span>
            </div>
          ))}
        </div>
      </div>
    </div>
  );
}
```

## Multi-Session Chat

Example with session management:

```tsx
import React, { useState } from 'react';
import {
  useChatSession,
  useChat,
  useInitializationStatus,
  useAgentCData
} from '@agentc/realtime-react';

function MultiSessionChat() {
  const { isInitialized } = useInitializationStatus();
  const { user } = useAgentCData();
  const { 
    currentSession, 
    sessions, 
    createSession, 
    switchSession, 
    deleteSession,
    renameSession 
  } = useChatSession();
  const { messages, sendMessage } = useChat();
  
  const [showNewSessionDialog, setShowNewSessionDialog] = useState(false);
  const [newSessionName, setNewSessionName] = useState('');
  const [messageInput, setMessageInput] = useState('');
  
  if (!isInitialized) {
    return <div>Loading sessions...</div>;
  }
  
  const handleCreateSession = async () => {
    if (newSessionName.trim()) {
      await createSession(newSessionName);
      setNewSessionName('');
      setShowNewSessionDialog(false);
    }
  };
  
  const handleSendMessage = async () => {
    if (messageInput.trim()) {
      await sendMessage(messageInput);
      setMessageInput('');
    }
  };
  
  return (
    <div className="multi-session-chat">
      {/* Sidebar with sessions */}
      <aside className="sessions-sidebar">
        <div className="sidebar-header">
          <h3>Chat Sessions</h3>
          <button onClick={() => setShowNewSessionDialog(true)}>+ New</button>
        </div>
        
        <div className="sessions-list">
          {sessions.map(session => (
            <div 
              key={session.id}
              className={`session-item ${session.id === currentSession?.id ? 'active' : ''}`}
              onClick={() => switchSession(session.id)}
            >
              <div className="session-info">
                <div className="session-name">
                  {session.name || `Session ${session.id.slice(0, 8)}`}
                </div>
                <div className="session-meta">
                  {session.message_count} messages • 
                  {new Date(session.updated_at).toLocaleDateString()}
                </div>
              </div>
              <div className="session-actions">
                <button 
                  onClick={(e) => {
                    e.stopPropagation();
                    const newName = prompt('Rename session:', session.name);
                    if (newName) renameSession(session.id, newName);
                  }}
                >
                  ✏️
                </button>
                <button 
                  onClick={(e) => {
                    e.stopPropagation();
                    if (confirm('Delete this session?')) {
                      deleteSession(session.id);
                    }
                  }}
                >
                  🗑️
                </button>
              </div>
            </div>
          ))}
        </div>
      </aside>
      
      {/* Main chat area */}
      <main className="chat-area">
        <div className="chat-header">
          <h2>{currentSession?.name || 'Chat'}</h2>
          <span className="user-info">👤 {user?.display_name}</span>
        </div>
        
        <div className="messages-area">
          {messages.map((msg, i) => (
            <div key={i} className={`message ${msg.role}`}>
              <div className="message-content">{msg.content}</div>
              <div className="message-time">
                {new Date(msg.timestamp).toLocaleTimeString()}
              </div>
            </div>
          ))}
        </div>
        
        <div className="input-area">
          <input
            type="text"
            value={messageInput}
            onChange={(e) => setMessageInput(e.target.value)}
            onKeyPress={(e) => e.key === 'Enter' && handleSendMessage()}
            placeholder="Type your message..."
          />
          <button onClick={handleSendMessage}>Send</button>
        </div>
      </main>
      
      {/* New session dialog */}
      {showNewSessionDialog && (
        <div className="dialog-overlay">
          <div className="dialog">
            <h3>Create New Session</h3>
            <input
              type="text"
              value={newSessionName}
              onChange={(e) => setNewSessionName(e.target.value)}
              placeholder="Session name..."
              autoFocus
            />
            <div className="dialog-buttons">
              <button onClick={handleCreateSession}>Create</button>
              <button onClick={() => setShowNewSessionDialog(false)}>Cancel</button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}
```

## Summary

These examples demonstrate the key changes in v2.0.0:

1. **Simplified Login** - Login only returns tokens, not data
2. **Automatic Events** - 6 events deliver all configuration
3. **New Hooks** - `useAgentCData()`, `useInitializationStatus()`, etc.
4. **Progressive Loading** - UI can update as data arrives
5. **No Manual Storage** - SDK handles all data internally

For more examples and patterns, see:
- [Migration Guide](../guides/authentication-migration.md)
- [API Reference](../api-reference/core/events.md)
- [React Hooks Guide](../api-reference/react/hooks.md)