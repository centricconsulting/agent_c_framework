# React Examples

Complete examples demonstrating how to build applications with the Agent C Realtime React SDK.

## Authentication Patterns

**IMPORTANT**: Production applications should NEVER include Agent C credentials in frontend code. The correct pattern is:

### Production Pattern (Recommended)
Your application backend handles authentication:
1. **Your backend** manages user authentication (login, sessions, etc.)
2. **Your backend** calls Agent C library functions to create ChatUsers and generate tokens
3. **Your frontend** receives a payload from YOUR backend containing:
   - JWT token (`access_token`)
   - WebSocket URL (`ws_url`)
   - Refresh token (`refresh_token`)
   - HeyGen token if needed (`heygen_access_token`)
   - Other configuration
4. **Your frontend** initializes the SDK with this payload

### Development Pattern (Testing Only)
Direct login is ONLY for development/testing:
- Uses username/password directly in frontend (NEVER do this in production!)
- Suitable only for local development and prototypes
- Must be replaced with proper backend integration before deployment

## Table of Contents

- [Production Authentication](#production-authentication)
- [Basic Chat Application](#basic-chat-application)
- [Voice Assistant](#voice-assistant)
- [Avatar Integration](#avatar-integration)
- [Multi-Session Manager](#multi-session-manager)
- [Custom UI Components](#custom-ui-components)
- [Advanced Features](#advanced-features)
- [Using Additional Hooks](#using-additional-hooks)
- [Error Handling & Edge Cases](#error-handling--edge-cases)
- [StrictMode Compatibility](#strictmode-compatibility)
- [Development-Only Examples](#development-only-examples)

---

## Production Authentication

**This is the correct pattern for production applications.** Your backend handles Agent C authentication and passes tokens to your frontend.

### Backend Integration (Your Server)

```javascript
// YOUR BACKEND CODE (Node.js example)
// This runs on YOUR server, not in the browser!
import { ChatUser } from '@agentc/server-sdk'; // Agent C server library

// Your login endpoint
app.post('/api/login', async (req, res) => {
  // 1. Authenticate YOUR user however you want
  const user = await authenticateUser(req.body.email, req.body.password);
  
  // 2. Create or get Agent C ChatUser for your user
  const chatUser = await ChatUser.createOrGet({
    username: `user_${user.id}`, // Your internal user ID
    metadata: { 
      email: user.email,
      name: user.name 
    }
  });
  
  // 3. Generate Agent C tokens for this user
  const agentCTokens = await chatUser.generateTokens();
  
  // 4. Return the payload to YOUR frontend
  res.json({
    // Your app's auth data
    userId: user.id,
    userToken: generateYourAppToken(user),
    
    // Agent C configuration for the frontend
    agentC: {
      wsUrl: agentCTokens.ws_url,
      authToken: agentCTokens.access_token,
      refreshToken: agentCTokens.refresh_token,
      heygenToken: agentCTokens.heygen_access_token
    }
  });
});
```

### Frontend Integration (React)

```tsx
// ProductionApp.tsx
import React, { useState, useEffect } from 'react';
import { AgentCProvider } from '@agentc/realtime-react';
import { AuthManager } from '@agentc/realtime-core';

function App() {
  const [authPayload, setAuthPayload] = useState<any>(null);
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    // Fetch auth payload from YOUR backend
    fetchAuthFromYourBackend();
  }, []);

  const fetchAuthFromYourBackend = async () => {
    try {
      // Call YOUR backend's login endpoint
      const response = await fetch('/api/login', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
          email: 'user@example.com',
          password: 'user_password'
        })
      });
      
      const data = await response.json();
      
      // Initialize AuthManager with the payload from YOUR backend
      // No username/password needed - just the tokens!
      const authManager = new AuthManager();
      authManager.initializeFromPayload(data.agentC);
      
      setAuthPayload(data.agentC);
    } catch (error) {
      console.error('Authentication failed:', error);
    } finally {
      setIsLoading(false);
    }
  };

  if (isLoading) {
    return <div>Loading...</div>;
  }

  if (!authPayload) {
    return <div>Authentication required</div>;
  }

  // Initialize the SDK with tokens from YOUR backend
  return (
    <AgentCProvider 
      config={{
        wsUrl: authPayload.wsUrl,
        authToken: authPayload.authToken,
        refreshToken: authPayload.refreshToken,
        heygenToken: authPayload.heygenToken,
        autoConnect: true
      }}
    >
      <YourApplication />
    </AgentCProvider>
  );
}
```

### Using Pre-Authenticated Payload

If your backend provides the Agent C configuration on page load (e.g., via server-side rendering or initial API call):

```tsx
// Initialize with payload already available
import React from 'react';
import { AgentCProvider } from '@agentc/realtime-react';
import { AuthManager } from '@agentc/realtime-core';

function App({ agentCConfig }: { agentCConfig: any }) {
  // agentCConfig comes from your backend via:
  // - Server-side rendering
  // - Initial API call
  // - Global window variable
  // - React context from parent component
  
  // Initialize AuthManager without login - just use the payload
  const authManager = new AuthManager();
  authManager.initializeFromPayload(agentCConfig);
  
  return (
    <AgentCProvider 
      config={{
        wsUrl: agentCConfig.wsUrl,
        authToken: agentCConfig.authToken,
        refreshToken: agentCConfig.refreshToken,
        heygenToken: agentCConfig.heygenToken,
        autoConnect: true
      }}
    >
      <YourApplication />
    </AgentCProvider>
  );
}

// Example: Getting config from window object (set by your backend)
function AppWithGlobalConfig() {
  // Your backend might inject this into the HTML
  const config = (window as any).AGENT_C_CONFIG;
  
  if (!config) {
    return <div>Configuration not found</div>;
  }
  
  return <App agentCConfig={config} />;
}
```

### Token Refresh Pattern

Your backend should handle token refresh:

```typescript
// YOUR BACKEND CODE
app.post('/api/refresh-agent-c-token', async (req, res) => {
  // Verify YOUR user's session
  const user = await verifyUserSession(req.headers.authorization);
  
  // Get the ChatUser
  const chatUser = await ChatUser.get(`user_${user.id}`);
  
  // Refresh Agent C tokens
  const newTokens = await chatUser.refreshTokens();
  
  res.json({
    agentC: {
      wsUrl: newTokens.ws_url,
      authToken: newTokens.access_token,
      refreshToken: newTokens.refresh_token,
      heygenToken: newTokens.heygen_access_token
    }
  });
});
```

---

## Basic Chat Application

A simple text chat interface with Agent C using the production authentication pattern.

```tsx
// BasicChat.tsx
import React, { useState, useEffect } from 'react';
import {
  AgentCProvider,
  useConnection,
  useChat
} from '@agentc/realtime-react';
import { AuthManager } from '@agentc/realtime-core';

function BasicChatApp() {
  const [agentCConfig, setAgentCConfig] = useState<any>(null);
  const [error, setError] = useState<string>('');

  useEffect(() => {
    // Fetch Agent C configuration from YOUR backend
    fetchConfigFromYourBackend();
  }, []);

  const fetchConfigFromYourBackend = async () => {
    try {
      // This calls YOUR backend, which handles Agent C authentication
      const response = await fetch('/api/agent-c/config', {
        method: 'GET',
        headers: {
          // Include YOUR app's auth token
          'Authorization': `Bearer ${localStorage.getItem('your_app_token')}`
        }
      });

      if (!response.ok) {
        throw new Error('Failed to get Agent C configuration');
      }

      const data = await response.json();
      
      // Initialize AuthManager with the payload from YOUR backend
      const authManager = new AuthManager();
      authManager.initializeFromPayload(data.agentC);
      
      setAgentCConfig(data.agentC);
    } catch (error) {
      console.error('Failed to get Agent C config:', error);
      setError('Unable to initialize chat. Please try again.');
    }
  };

  if (error) {
    return (
      <div className="error-container">
        <p>{error}</p>
        <button onClick={fetchConfigFromYourBackend}>Retry</button>
      </div>
    );
  }

  if (!agentCConfig) {
    return <div>Loading chat configuration...</div>;
  }

  return (
    <AgentCProvider 
      config={{
        wsUrl: agentCConfig.wsUrl,
        authToken: agentCConfig.authToken,
        refreshToken: agentCConfig.refreshToken,
        autoConnect: true
      }}
    >
      <ChatInterface />
    </AgentCProvider>
  );
}

function ChatInterface() {
  const { isConnected, connectionState } = useConnection();
  const { messages, sendMessage, isStreaming, streamingText } = useChat();
  const [input, setInput] = useState('');
  
  const handleSend = () => {
    if (input.trim() && isConnected) {
      sendMessage(input);
      setInput('');
    }
  };
  
  return (
    <div className="chat-container">
      <div className="status-bar">
        <span className={`status ${isConnected ? 'connected' : 'disconnected'}`}>
          {connectionState}
        </span>
      </div>
      
      <div className="messages-container">
        {messages.map((message, index) => (
          <div key={index} className={`message ${message.role}`}>
            <div className="message-role">{message.role}</div>
            <div className="message-content">{message.content}</div>
            <div className="message-time">
              {new Date(message.timestamp).toLocaleTimeString()}
            </div>
          </div>
        ))}
        
        {isStreaming && (
          <div className="message assistant streaming">
            <div className="message-role">assistant</div>
            <div className="message-content">
              {streamingText}
              <span className="typing-indicator">...</span>
            </div>
          </div>
        )}
      </div>
      
      <div className="input-container">
        <input
          type="text"
          value={input}
          onChange={(e) => setInput(e.target.value)}
          onKeyPress={(e) => e.key === 'Enter' && handleSend()}
          placeholder="Type your message..."
          disabled={!isConnected}
        />
        <button 
          onClick={handleSend}
          disabled={!isConnected || !input.trim()}
        >
          Send
        </button>
      </div>
    </div>
  );
}

export default BasicChatApp;
```

---

## Voice Assistant

Voice-enabled assistant with turn management using production authentication.

```tsx
// VoiceAssistant.tsx
import React, { useEffect, useState } from 'react';
import {
  AgentCProvider,
  useConnection,
  useChat,
  useAudio,
  useTurnState,
  useVoiceModel
} from '@agentc/realtime-react';
import { AuthManager } from '@agentc/realtime-core';

function VoiceAssistantApp() {
  const [agentCConfig, setAgentCConfig] = useState<any>(null);
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    initializeFromBackend();
  }, []);

  const initializeFromBackend = async () => {
    try {
      // Get Agent C config from YOUR backend
      const response = await fetch('/api/agent-c/voice-config', {
        headers: {
          'Authorization': `Bearer ${localStorage.getItem('your_app_token')}`
        }
      });
      
      const data = await response.json();
      
      // Initialize AuthManager with backend payload
      const authManager = new AuthManager();
      authManager.initializeFromPayload(data.agentC);
      
      // Set config with audio enabled
      setAgentCConfig({
        wsUrl: data.agentC.wsUrl,
        authToken: data.agentC.authToken,
        refreshToken: data.agentC.refreshToken,
        enableAudio: true,
        audioConfig: {
          enableInput: true,
          enableOutput: true,
          respectTurnState: true,
          initialVolume: 0.8
        }
      });
    } catch (error) {
      console.error('Failed to initialize:', error);
    } finally {
      setIsLoading(false);
    }
  };

  if (isLoading) {
    return <div>Loading voice assistant...</div>;
  }

  if (!agentCConfig) {
    return <div>Failed to load configuration</div>;
  }

  return (
    <AgentCProvider config={agentCConfig}>
      <VoiceInterface />
    </AgentCProvider>
  );
}

function VoiceInterface() {
  const { connect, isConnected } = useConnection();
  const { messages, sendMessage } = useChat();
  const { 
    startRecording, 
    stopRecording, 
    isRecording,
    startStreaming,
    stopStreaming,
    isStreaming,
    audioLevel,
    volume,
    setVolume,
    error: audioError
  } = useAudio();
  const { currentTurn, isUserTurn, isAgentTurn } = useTurnState();
  const { currentVoice, availableVoices, setVoice } = useVoiceModel();
  
  const [isListening, setIsListening] = useState(false);
  
  // Auto-connect on mount
  useEffect(() => {
    connect().catch(console.error);
  }, []);
  
  // Handle turn-based audio
  useEffect(() => {
    if (isUserTurn && isListening && !isStreaming) {
      startStreaming();
    } else if (!isUserTurn && isStreaming) {
      stopStreaming();
    }
  }, [isUserTurn, isListening, isStreaming]);
  
  const toggleListening = async () => {
    if (!isListening) {
      try {
        await startRecording();
        setIsListening(true);
        if (isUserTurn) {
          startStreaming();
        }
      } catch (err) {
        console.error('Microphone error:', err);
        if (err.name === 'NotAllowedError') {
          alert('Please allow microphone access to use voice features');
        }
      }
    } else {
      stopStreaming();
      stopRecording();
      setIsListening(false);
    }
  };
  
  return (
    <div className="voice-assistant">
      <header className="control-panel">
        <div className="connection-status">
          {isConnected ? '🟢 Connected' : '🔴 Disconnected'}
        </div>
        
        <div className="voice-selector">
          <label>Voice:</label>
          <select 
            value={currentVoice?.voice_id || 'none'}
            onChange={(e) => setVoice(e.target.value)}
          >
            <option value="none">Text Only</option>
            {availableVoices.map(voice => (
              <option key={voice.voice_id} value={voice.voice_id}>
                {voice.name}
              </option>
            ))}
          </select>
        </div>
        
        <div className="volume-control">
          <label>Volume:</label>
          <input
            type="range"
            min="0"
            max="1"
            step="0.1"
            value={volume}
            onChange={(e) => setVolume(parseFloat(e.target.value))}
          />
          <span>{Math.round(volume * 100)}%</span>
        </div>
      </header>
      
      <main className="conversation">
        <div className="turn-indicator">
          <div className={`turn-state ${currentTurn}`}>
            {isUserTurn && '🎤 Your turn to speak'}
            {isAgentTurn && '🤖 Agent is speaking'}
            {!isUserTurn && !isAgentTurn && '⏸️ Ready'}
          </div>
        </div>
        
        <div className="messages">
          {messages.map((msg, i) => (
            <div key={i} className={`message ${msg.role}`}>
              <div className="avatar">
                {msg.role === 'user' ? '👤' : '🤖'}
              </div>
              <div className="content">
                {msg.content}
              </div>
            </div>
          ))}
        </div>
        
        {audioError && (
          <div className="error-message">
            Audio Error: {audioError.message}
          </div>
        )}
      </main>
      
      <footer className="voice-controls">
        <button 
          className={`mic-button ${isListening ? 'active' : ''}`}
          onClick={toggleListening}
          disabled={!isConnected}
        >
          {isListening ? '🔴 Stop' : '🎤 Start'} Listening
        </button>
        
        <div className="audio-visualizer">
          <div className="level-meter">
            <div 
              className="level-bar"
              style={{ width: `${audioLevel * 100}%` }}
            />
          </div>
          <span className="level-label">
            {isStreaming ? 'Streaming' : isRecording ? 'Recording' : 'Idle'}
          </span>
        </div>
        
        <button 
          className="text-input-toggle"
          onClick={() => {
            const text = prompt('Enter your message:');
            if (text) sendMessage(text);
          }}
        >
          ⌨️ Type Instead
        </button>
      </footer>
    </div>
  );
}

export default VoiceAssistantApp;
```

---

## Avatar Integration

HeyGen avatar with synchronized audio using production authentication.

```tsx
// AvatarChat.tsx
import React, { useRef, useState, useEffect } from 'react';
import {
  AgentCProvider,
  useConnection,
  useChat,
  useAvatar,
  useVoiceModel,
  useRealtimeClient
} from '@agentc/realtime-react';
import { AuthManager } from '@agentc/realtime-core';
import NewStreamingAvatar, { StreamingEvents } from '@heygen/streaming-avatar';

function AvatarChatApp() {
  const [agentCConfig, setAgentCConfig] = useState<any>(null);
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    loadAvatarConfig();
  }, []);

  const loadAvatarConfig = async () => {
    try {
      // YOUR backend provides Agent C config including HeyGen token
      const response = await fetch('/api/agent-c/avatar-config', {
        headers: {
          'Authorization': `Bearer ${localStorage.getItem('your_app_token')}`
        }
      });
      
      const data = await response.json();
      
      // Initialize AuthManager with backend payload
      const authManager = new AuthManager();
      authManager.initializeFromPayload(data.agentC);
      
      setAgentCConfig({
        wsUrl: data.agentC.wsUrl,
        authToken: data.agentC.authToken,
        refreshToken: data.agentC.refreshToken,
        heygenToken: data.agentC.heygenToken, // HeyGen token from YOUR backend
        enableAudio: true
      });
    } catch (error) {
      console.error('Failed to load avatar config:', error);
    } finally {
      setIsLoading(false);
    }
  };

  if (isLoading) {
    return <div>Loading avatar configuration...</div>;
  }

  if (!agentCConfig) {
    return <div>Avatar configuration not available</div>;
  }

  return (
    <AgentCProvider config={agentCConfig}>
      <AvatarInterface />
    </AgentCProvider>
  );
}

function AvatarInterface() {
  const videoRef = useRef<HTMLVideoElement>(null);
  const heygenRef = useRef<NewStreamingAvatar | null>(null);
  
  const client = useRealtimeClient();
  const { connect, isConnected } = useConnection();
  const { messages, sendMessage } = useChat();
  const {
    availableAvatars,
    currentAvatar,
    isSessionActive,
    startAvatarSession,
    endAvatarSession
  } = useAvatar();
  const { setVoice } = useVoiceModel();
  
  const [selectedAvatarId, setSelectedAvatarId] = useState('');
  const [isInitializing, setIsInitializing] = useState(false);
  const [input, setInput] = useState('');
  
  useEffect(() => {
    connect().catch(console.error);
  }, []);
  
  const initializeAvatar = async () => {
    if (!selectedAvatarId) return;
    
    setIsInitializing(true);
    
    try {
      // Get HeyGen token from the client config
      const heygenToken = client?.getConfig()?.heygenToken;
      
      if (!heygenToken) {
        throw new Error('HeyGen token not available in configuration.');
      }
      
      // Create HeyGen instance
      const avatar = new NewStreamingAvatar({ token: heygenToken });
      heygenRef.current = avatar;
      
      // Set up event listeners
      avatar.on(StreamingEvents.STREAM_READY, (event) => {
        console.log('Avatar stream ready');
        if (videoRef.current && event.stream) {
          videoRef.current.srcObject = event.stream;
          videoRef.current.play();
        }
        
        // Notify Agent C about avatar session
        startAvatarSession(selectedAvatarId);
        
        // Switch to avatar voice mode
        setVoice('avatar');
      });
      
      avatar.on(StreamingEvents.STREAM_DISCONNECTED, () => {
        console.log('Avatar stream disconnected');
        if (videoRef.current) {
          videoRef.current.srcObject = null;
        }
        endAvatarSession();
      });
      
      // Start avatar
      await avatar.createStartAvatar({
        avatarName: selectedAvatarId,
        quality: 'high',
        voice: {
          voiceId: 'avatar' // Agent C handles voice
        }
      });
      
    } catch (error) {
      console.error('Failed to initialize avatar:', error);
      alert('Failed to start avatar. Please try again.');
    } finally {
      setIsInitializing(false);
    }
  };
  
  const stopAvatar = async () => {
    if (heygenRef.current) {
      await heygenRef.current.stopAvatar();
      heygenRef.current = null;
    }
    endAvatarSession();
    setVoice('nova'); // Switch back to default voice
  };
  
  const handleSend = () => {
    if (input.trim()) {
      sendMessage(input);
      setInput('');
    }
  };
  
  return (
    <div className="avatar-chat">
      <div className="avatar-section">
        {!isSessionActive ? (
          <div className="avatar-selector">
            <h3>Select an Avatar</h3>
            <div className="avatar-grid">
              {availableAvatars.map(avatar => (
                <div 
                  key={avatar.avatar_id}
                  className={`avatar-card ${selectedAvatarId === avatar.avatar_id ? 'selected' : ''}`}
                  onClick={() => setSelectedAvatarId(avatar.avatar_id)}
                >
                  {avatar.preview_url && (
                    <img src={avatar.preview_url} alt={avatar.name} />
                  )}
                  <h4>{avatar.name}</h4>
                  <p>{avatar.description}</p>
                </div>
              ))}
            </div>
            
            <button 
              onClick={initializeAvatar}
              disabled={!selectedAvatarId || !isConnected || isInitializing}
              className="start-avatar-btn"
            >
              {isInitializing ? 'Starting...' : 'Start Avatar'}
            </button>
          </div>
        ) : (
          <div className="avatar-view">
            <video 
              ref={videoRef}
              className="avatar-video"
              autoPlay
              playsInline
            />
            <div className="avatar-info">
              <h3>{currentAvatar?.name}</h3>
              <button onClick={stopAvatar} className="stop-avatar-btn">
                Stop Avatar
              </button>
            </div>
          </div>
        )}
      </div>
      
      <div className="chat-section">
        <div className="messages">
          {messages.map((msg, i) => (
            <div key={i} className={`message ${msg.role}`}>
              <strong>{msg.role}:</strong> {msg.content}
            </div>
          ))}
        </div>
        
        <div className="input-area">
          <input
            value={input}
            onChange={(e) => setInput(e.target.value)}
            onKeyPress={(e) => e.key === 'Enter' && handleSend()}
            placeholder="Type your message..."
            disabled={!isConnected}
          />
          <button onClick={handleSend} disabled={!isConnected}>
            Send
          </button>
        </div>
      </div>
    </div>
  );
}

export default AvatarChatApp;
```

---

## Multi-Session Manager

Manage multiple chat sessions with history using production authentication.

```tsx
// MultiSessionManager.tsx
import React, { useState, useEffect } from 'react';
import {
  AgentCProvider,
  useConnection,
  useChat
} from '@agentc/realtime-react';
import { AuthManager } from '@agentc/realtime-core';

function MultiSessionApp() {
  const [agentCConfig, setAgentCConfig] = useState<any>(null);
  const [loadError, setLoadError] = useState<string>('');

  useEffect(() => {
    loadConfiguration();
  }, []);

  const loadConfiguration = async () => {
    try {
      // Get Agent C config from YOUR backend
      const response = await fetch('/api/agent-c/session-config', {
        headers: {
          'Authorization': `Bearer ${localStorage.getItem('your_app_token')}`
        }
      });

      if (!response.ok) {
        throw new Error('Failed to load configuration');
      }

      const data = await response.json();
      
      // Initialize AuthManager with backend payload
      const authManager = new AuthManager();
      authManager.initializeFromPayload(data.agentC);
      
      setAgentCConfig({
        wsUrl: data.agentC.wsUrl,
        authToken: data.agentC.authToken,
        refreshToken: data.agentC.refreshToken
      });
    } catch (error: any) {
      setLoadError(error.message || 'Configuration failed');
    }
  };

  if (loadError) {
    return (
      <div className="error">
        <p>Error: {loadError}</p>
        <button onClick={loadConfiguration}>Retry</button>
      </div>
    );
  }

  if (!agentCConfig) {
    return <div>Loading configuration...</div>;
  }

  return (
    <AgentCProvider 
      config={agentCConfig}
      autoConnect={true}
    >
      <SessionManager />
    </AgentCProvider>
  );
}

function SessionManager() {
  const { isConnected } = useConnection();
  const {
    messages,
    sendMessage,
    currentSessionId,
    sessions,
    newSession,
    resumeSession,
    setSessionName,
    clearMessages
  } = useChat();
  
  const [input, setInput] = useState('');
  const [isRenamingSession, setIsRenamingSession] = useState(false);
  const [newName, setNewName] = useState('');
  
  const handleNewSession = () => {
    const agentKey = prompt('Enter agent key (optional):');
    newSession(agentKey || undefined);
  };
  
  const handleRenameSession = () => {
    if (newName.trim()) {
      setSessionName(newName);
      setIsRenamingSession(false);
      setNewName('');
    }
  };
  
  const handleSend = () => {
    if (input.trim()) {
      sendMessage(input);
      setInput('');
    }
  };
  
  return (
    <div className="session-manager">
      <aside className="session-list">
        <div className="session-header">
          <h3>Sessions</h3>
          <button onClick={handleNewSession} disabled={!isConnected}>
            + New
          </button>
        </div>
        
        <ul className="sessions">
          {sessions.map(session => (
            <li
              key={session.session_id}
              className={session.session_id === currentSessionId ? 'active' : ''}
              onClick={() => resumeSession(session.session_id)}
            >
              <div className="session-name">
                {session.session_name || `Session ${session.session_id.slice(0, 8)}`}
              </div>
              <div className="session-meta">
                {session.messages.length} messages
              </div>
              <div className="session-time">
                {new Date(session.updated_at).toLocaleDateString()}
              </div>
            </li>
          ))}
        </ul>
      </aside>
      
      <main className="chat-area">
        <header className="chat-header">
          {currentSessionId && (
            <div className="session-info">
              {isRenamingSession ? (
                <div className="rename-form">
                  <input
                    value={newName}
                    onChange={(e) => setNewName(e.target.value)}
                    onKeyPress={(e) => e.key === 'Enter' && handleRenameSession()}
                    placeholder="Session name..."
                    autoFocus
                  />
                  <button onClick={handleRenameSession}>Save</button>
                  <button onClick={() => setIsRenamingSession(false)}>Cancel</button>
                </div>
              ) : (
                <div className="session-title">
                  <h2>
                    {sessions.find(s => s.session_id === currentSessionId)?.session_name || 
                     `Session ${currentSessionId?.slice(0, 8)}`}
                  </h2>
                  <button onClick={() => setIsRenamingSession(true)}>
                    ✏️ Rename
                  </button>
                  <button onClick={clearMessages}>
                    🗑️ Clear
                  </button>
                </div>
              )}
            </div>
          )}
        </header>
        
        <div className="messages">
          {messages.map((msg, i) => (
            <div key={i} className={`message ${msg.role}`}>
              <div className="message-header">
                <span className="role">{msg.role}</span>
                <span className="time">
                  {new Date(msg.timestamp).toLocaleTimeString()}
                </span>
              </div>
              <div className="message-content">{msg.content}</div>
            </div>
          ))}
        </div>
        
        <footer className="input-area">
          <input
            value={input}
            onChange={(e) => setInput(e.target.value)}
            onKeyPress={(e) => e.key === 'Enter' && handleSend()}
            placeholder="Type your message..."
            disabled={!isConnected || !currentSessionId}
          />
          <button 
            onClick={handleSend}
            disabled={!isConnected || !currentSessionId || !input.trim()}
          >
            Send
          </button>
        </footer>
      </main>
    </div>
  );
}

export default MultiSessionApp;
```

---

## Custom UI Components

Reusable components for Agent C applications.

```tsx
// CustomComponents.tsx
import React from 'react';
import {
  useConnection,
  useAudio,
  useTurnState,
  useVoiceModel
} from '@agentc/realtime-react';

// Connection Status Badge
export function ConnectionBadge() {
  const { isConnected, connectionState, reconnectAttempt } = useConnection();
  
  return (
    <div className={`connection-badge ${connectionState.toLowerCase()}`}>
      {isConnected && '🟢'}
      {!isConnected && '🔴'}
      {reconnectAttempt > 0 && '🔄'}
      <span>{connectionState}</span>
      {reconnectAttempt > 0 && (
        <span className="reconnect-info">
          Attempt {reconnectAttempt}
        </span>
      )}
    </div>
  );
}

// Audio Level Meter
export function AudioLevelMeter() {
  const { audioLevel, isRecording } = useAudio();
  
  const bars = 20;
  const activeBars = Math.round(audioLevel * bars);
  
  return (
    <div className="audio-level-meter">
      {Array.from({ length: bars }).map((_, i) => (
        <div 
          key={i}
          className={`bar ${i < activeBars ? 'active' : ''} ${
            i > bars * 0.8 ? 'high' : i > bars * 0.5 ? 'medium' : 'low'
          }`}
        />
      ))}
      {!isRecording && <div className="muted-overlay">Muted</div>}
    </div>
  );
}

// Turn Status Indicator
export function TurnIndicator() {
  const { currentTurn, isUserTurn, isAgentTurn, turnDuration } = useTurnState();
  
  const formatDuration = (ms: number) => {
    const seconds = Math.floor(ms / 1000);
    const minutes = Math.floor(seconds / 60);
    const remainingSeconds = seconds % 60;
    return minutes > 0 
      ? `${minutes}:${remainingSeconds.toString().padStart(2, '0')}`
      : `${seconds}s`;
  };
  
  return (
    <div className={`turn-indicator ${currentTurn}`}>
      <div className="turn-icon">
        {isUserTurn && '🎤'}
        {isAgentTurn && '🤖'}
        {!isUserTurn && !isAgentTurn && '⏸️'}
      </div>
      <div className="turn-text">
        {isUserTurn && 'Your Turn'}
        {isAgentTurn && 'Agent Speaking'}
        {!isUserTurn && !isAgentTurn && 'Ready'}
      </div>
      <div className="turn-duration">
        {formatDuration(turnDuration)}
      </div>
    </div>
  );
}

// Voice Selector Dropdown
export function VoiceSelector() {
  const { currentVoice, availableVoices, setVoice, isAvatarMode, isTextOnly } = useVoiceModel();
  
  return (
    <div className="voice-selector">
      <label htmlFor="voice-select">Voice:</label>
      <select 
        id="voice-select"
        value={currentVoice?.voice_id || 'none'}
        onChange={(e) => setVoice(e.target.value)}
      >
        <option value="none">📝 Text Only</option>
        {availableVoices.map(voice => (
          <option key={voice.voice_id} value={voice.voice_id}>
            🔊 {voice.name} ({voice.vendor})
          </option>
        ))}
        <option value="avatar">🎭 Avatar Mode</option>
      </select>
      
      <div className="voice-status">
        {isTextOnly && <span className="text-only">No Audio</span>}
        {isAvatarMode && <span className="avatar-mode">Avatar Active</span>}
        {!isTextOnly && !isAvatarMode && currentVoice && (
          <span className="voice-active">{currentVoice.name}</span>
        )}
      </div>
    </div>
  );
}

// Microphone Button with States
export function MicrophoneButton() {
  const { 
    startRecording, 
    stopRecording, 
    isRecording,
    startStreaming,
    stopStreaming,
    isStreaming,
    hasPermission,
    error
  } = useAudio();
  const { isUserTurn } = useTurnState();
  const { isConnected } = useConnection();
  
  const handleClick = async () => {
    if (!isRecording) {
      try {
        await startRecording();
        if (isUserTurn) {
          startStreaming();
        }
      } catch (err) {
        console.error('Microphone error:', err);
      }
    } else {
      stopStreaming();
      stopRecording();
    }
  };
  
  const getButtonState = () => {
    if (!isConnected) return 'disabled';
    if (!hasPermission) return 'no-permission';
    if (isStreaming) return 'streaming';
    if (isRecording) return 'recording';
    return 'idle';
  };
  
  const state = getButtonState();
  
  return (
    <button 
      className={`microphone-button ${state}`}
      onClick={handleClick}
      disabled={!isConnected}
      title={
        !hasPermission ? 'Click to grant microphone permission' :
        isStreaming ? 'Streaming audio' :
        isRecording ? 'Recording (not streaming)' :
        'Click to start recording'
      }
    >
      {state === 'streaming' && '🔴'}
      {state === 'recording' && '🟠'}
      {state === 'idle' && '🎤'}
      {state === 'no-permission' && '🔇'}
      {state === 'disabled' && '⏸️'}
      
      <span className="button-label">
        {state === 'streaming' && 'Streaming'}
        {state === 'recording' && 'Recording'}
        {state === 'idle' && 'Start'}
        {state === 'no-permission' && 'Enable Mic'}
        {state === 'disabled' && 'Offline'}
      </span>
      
      {error && (
        <span className="error-tooltip">{error.message}</span>
      )}
    </button>
  );
}
```

---

## Advanced Features

Complete application with all features integrated.

```tsx
// AdvancedApp.tsx
import React, { useState, useEffect, useCallback } from 'react';
import {
  AgentCProvider,
  useRealtimeClient,
  useConnection,
  useChat,
  useAudio,
  useTurnState,
  useVoiceModel,
  useAvatar
} from '@agentc/realtime-react';
import { AuthManager } from '@agentc/realtime-core';

// Import custom components
import {
  ConnectionBadge,
  AudioLevelMeter,
  TurnIndicator,
  VoiceSelector,
  MicrophoneButton
} from './CustomComponents';

function AdvancedApp() {
  const [agentCConfig, setAgentCConfig] = useState<any>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string>('');

  useEffect(() => {
    loadAdvancedConfig();
  }, []);

  const loadAdvancedConfig = async () => {
    try {
      // Get comprehensive config from YOUR backend
      const response = await fetch('/api/agent-c/advanced-config', {
        headers: {
          'Authorization': `Bearer ${localStorage.getItem('your_app_token')}`
        }
      });

      if (!response.ok) {
        // If user not authenticated with YOUR app, redirect to YOUR login
        if (response.status === 401) {
          window.location.href = '/login'; // YOUR app's login page
          return;
        }
        throw new Error('Failed to load configuration');
      }

      const data = await response.json();
      
      // Initialize AuthManager with backend payload
      const authManager = new AuthManager();
      authManager.initializeFromPayload(data.agentC);
      
      setAgentCConfig({
        wsUrl: data.agentC.wsUrl,
        authToken: data.agentC.authToken,
        refreshToken: data.agentC.refreshToken,
        heygenToken: data.agentC.heygenToken,
        enableAudio: true,
        audioConfig: {
          enableInput: true,
          enableOutput: true,
          respectTurnState: true,
          initialVolume: 0.8
        }
      });
    } catch (error: any) {
      console.error('Configuration failed:', error);
      setError(error.message || 'Failed to load configuration');
    } finally {
      setIsLoading(false);
    }
  };

  if (isLoading) {
    return <div className="loading">Loading configuration...</div>;
  }

  if (error) {
    return (
      <div className="error-container">
        <h2>Configuration Error</h2>
        <p>{error}</p>
        <button onClick={loadAdvancedConfig}>Retry</button>
      </div>
    );
  }
  
  if (!agentCConfig) {
    return null;
  }

  return (
    <AgentCProvider config={agentCConfig}>
      <AdvancedInterface />
    </AgentCProvider>
  );
}

function AdvancedInterface() {
  const client = useRealtimeClient();
  const { connect, disconnect, isConnected } = useConnection();
  const { 
    messages, 
    sendMessage, 
    streamingText, 
    isStreaming,
    newSession,
    sessions,
    currentSessionId
  } = useChat();
  const { volume, setVolume } = useAudio();
  const { currentTurn } = useTurnState();
  const { availableAvatars } = useAvatar();
  
  const [input, setInput] = useState('');
  const [showSettings, setShowSettings] = useState(false);
  const [theme, setTheme] = useState<'light' | 'dark'>('light');
  
  // Auto-connect on mount
  useEffect(() => {
    connect().catch(console.error);
  }, []);
  
  // Keyboard shortcuts
  useEffect(() => {
    const handleKeyPress = (e: KeyboardEvent) => {
      if (e.ctrlKey || e.metaKey) {
        switch(e.key) {
          case 'n':
            e.preventDefault();
            newSession();
            break;
          case 'd':
            e.preventDefault();
            if (isConnected) disconnect();
            else connect();
            break;
          case ',':
            e.preventDefault();
            setShowSettings(!showSettings);
            break;
        }
      }
    };
    
    window.addEventListener('keydown', handleKeyPress);
    return () => window.removeEventListener('keydown', handleKeyPress);
  }, [isConnected, showSettings]);
  
  // Custom event handling
  useEffect(() => {
    if (!client) return;
    
    const handleCustomEvent = (event: any) => {
      console.log('Custom event:', event);
      // Handle custom events from server
    };
    
    client.on('custom_event', handleCustomEvent);
    
    return () => {
      client.off('custom_event', handleCustomEvent);
    };
  }, [client]);
  
  const handleSend = useCallback(() => {
    if (input.trim()) {
      sendMessage(input);
      setInput('');
    }
  }, [input, sendMessage]);
  
  return (
    <div className={`advanced-app theme-${theme}`}>
      <header className="app-header">
        <div className="header-left">
          <h1>Agent C Advanced</h1>
          <ConnectionBadge />
        </div>
        
        <div className="header-center">
          <TurnIndicator />
          <AudioLevelMeter />
        </div>
        
        <div className="header-right">
          <VoiceSelector />
          <button 
            onClick={() => setShowSettings(!showSettings)}
            className="settings-button"
          >
            ⚙️
          </button>
        </div>
      </header>
      
      {showSettings && (
        <aside className="settings-panel">
          <h3>Settings</h3>
          
          <div className="setting-group">
            <label>Theme</label>
            <select 
              value={theme} 
              onChange={(e) => setTheme(e.target.value as 'light' | 'dark')}
            >
              <option value="light">Light</option>
              <option value="dark">Dark</option>
            </select>
          </div>
          
          <div className="setting-group">
            <label>Volume: {Math.round(volume * 100)}%</label>
            <input
              type="range"
              min="0"
              max="1"
              step="0.05"
              value={volume}
              onChange={(e) => setVolume(parseFloat(e.target.value))}
            />
          </div>
          
          <div className="setting-group">
            <h4>Sessions ({sessions.length})</h4>
            <button onClick={() => newSession()}>New Session</button>
          </div>
          
          <div className="setting-group">
            <h4>Avatars ({availableAvatars.length})</h4>
            {availableAvatars.map(avatar => (
              <div key={avatar.avatar_id}>
                {avatar.name}
              </div>
            ))}
          </div>
          
          <div className="setting-group">
            <h4>Keyboard Shortcuts</h4>
            <ul>
              <li>Ctrl+N - New session</li>
              <li>Ctrl+D - Toggle connection</li>
              <li>Ctrl+, - Toggle settings</li>
              <li>Enter - Send message</li>
            </ul>
          </div>
        </aside>
      )}
      
      <main className="app-main">
        <div className="chat-container">
          <div className="messages-area">
            {messages.map((msg, i) => (
              <div 
                key={i} 
                className={`message ${msg.role}`}
                data-timestamp={msg.timestamp}
              >
                <div className="message-avatar">
                  {msg.role === 'user' ? '👤' : '🤖'}
                </div>
                <div className="message-body">
                  <div className="message-header">
                    <span className="role">{msg.role}</span>
                    <span className="time">
                      {new Date(msg.timestamp).toLocaleTimeString()}
                    </span>
                  </div>
                  <div className="message-content">
                    {msg.content}
                  </div>
                </div>
              </div>
            ))}
            
            {isStreaming && streamingText && (
              <div className="message assistant streaming">
                <div className="message-avatar">🤖</div>
                <div className="message-body">
                  <div className="message-content">
                    {streamingText}
                    <span className="typing-dots">
                      <span>.</span><span>.</span><span>.</span>
                    </span>
                  </div>
                </div>
              </div>
            )}
          </div>
        </div>
      </main>
      
      <footer className="app-footer">
        <div className="input-group">
          <MicrophoneButton />
          
          <input
            type="text"
            value={input}
            onChange={(e) => setInput(e.target.value)}
            onKeyPress={(e) => e.key === 'Enter' && handleSend()}
            placeholder={
              !isConnected ? 'Connecting...' :
              currentTurn === 'user_turn' ? 'Type your message...' :
              currentTurn === 'agent_turn' ? 'Agent is responding...' :
              'Type your message...'
            }
            disabled={!isConnected}
            className="message-input"
          />
          
          <button 
            onClick={handleSend}
            disabled={!isConnected || !input.trim()}
            className="send-button"
          >
            Send
          </button>
        </div>
        
        <div className="footer-info">
          <span>Session: {currentSessionId?.slice(0, 8) || 'None'}</span>
          <span>•</span>
          <span>{messages.length} messages</span>
          <span>•</span>
          <span>Turn: {currentTurn}</span>
        </div>
      </footer>
    </div>
  );
}

export default AdvancedApp;
```

## Styling Examples

```css
/* styles.css */

/* Basic Chat Styles */
.chat-container {
  display: flex;
  flex-direction: column;
  height: 100vh;
  max-width: 800px;
  margin: 0 auto;
}

.messages-container {
  flex: 1;
  overflow-y: auto;
  padding: 20px;
  background: #f5f5f5;
}

.message {
  margin-bottom: 15px;
  padding: 10px 15px;
  border-radius: 10px;
  max-width: 70%;
}

.message.user {
  background: #007bff;
  color: white;
  margin-left: auto;
  text-align: right;
}

.message.assistant {
  background: white;
  border: 1px solid #ddd;
}

.message.streaming {
  opacity: 0.8;
}

.typing-indicator {
  animation: pulse 1.5s infinite;
}

@keyframes pulse {
  0%, 100% { opacity: 0.3; }
  50% { opacity: 1; }
}

/* Voice Assistant Styles */
.voice-assistant {
  display: grid;
  grid-template-rows: auto 1fr auto;
  height: 100vh;
}

.turn-indicator {
  padding: 10px;
  text-align: center;
  font-weight: bold;
}

.turn-indicator.user_turn {
  background: #4caf50;
  color: white;
}

.turn-indicator.agent_turn {
  background: #ff9800;
  color: white;
}

.audio-visualizer {
  display: flex;
  align-items: center;
  gap: 10px;
}

.level-meter {
  width: 200px;
  height: 20px;
  background: #ddd;
  border-radius: 10px;
  overflow: hidden;
}

.level-bar {
  height: 100%;
  background: linear-gradient(90deg, #4caf50, #8bc34a, #ffeb3b, #ff9800, #f44336);
  transition: width 100ms;
}

.mic-button {
  width: 80px;
  height: 80px;
  border-radius: 50%;
  font-size: 30px;
  border: 3px solid #ddd;
  background: white;
  cursor: pointer;
  transition: all 0.3s;
}

.mic-button.active {
  background: #f44336;
  color: white;
  animation: recording 1s infinite;
}

@keyframes recording {
  0%, 100% { transform: scale(1); }
  50% { transform: scale(1.1); }
}

/* Avatar Styles */
.avatar-video {
  width: 100%;
  max-width: 600px;
  height: auto;
  border-radius: 10px;
  box-shadow: 0 4px 6px rgba(0,0,0,0.1);
}

.avatar-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(200px, 1fr));
  gap: 20px;
  padding: 20px;
}

.avatar-card {
  border: 2px solid #ddd;
  border-radius: 10px;
  padding: 15px;
  cursor: pointer;
  transition: all 0.3s;
}

.avatar-card.selected {
  border-color: #007bff;
  background: #f0f8ff;
}

.avatar-card:hover {
  transform: translateY(-2px);
  box-shadow: 0 4px 8px rgba(0,0,0,0.1);
}
```

## Testing Components

```tsx
// ComponentTests.tsx
import { render, screen, fireEvent, waitFor } from '@testing-library/react';
import { AgentCProvider } from '@agentc/realtime-react';
import { ChatInterface } from './ChatInterface';

// Mock AuthManager for testing
jest.mock('@agentc/realtime-core', () => ({
  ...jest.requireActual('@agentc/realtime-core'),
  AuthManager: jest.fn().mockImplementation(() => ({
    login: jest.fn().mockResolvedValue({
      ws_url: 'wss://test.api.com/rt/ws',
      access_token: 'test-token',
      refresh_token: 'test-refresh-token',
      heygen_access_token: 'test-heygen-token'
    })
  }))
}));

describe('ChatInterface', () => {
  const mockConfig = {
    wsUrl: 'wss://test.api.com/rt/ws',
    authToken: 'test-token',
    refreshToken: 'test-refresh-token'
  };
  
  it('renders without crashing', () => {
    render(
      <AgentCProvider config={mockConfig}>
        <ChatInterface />
      </AgentCProvider>
    );
    
    expect(screen.getByPlaceholderText(/type your message/i)).toBeInTheDocument();
  });
  
  it('sends message on button click', async () => {
    render(
      <AgentCProvider config={mockConfig} autoConnect={true}>
        <ChatInterface />
      </AgentCProvider>
    );
    
    const input = screen.getByPlaceholderText(/type your message/i);
    const button = screen.getByText(/send/i);
    
    fireEvent.change(input, { target: { value: 'Test message' } });
    fireEvent.click(button);
    
    await waitFor(() => {
      expect(input).toHaveValue('');
    });
  });
});
```

## Deployment

### Environment Variables

**NEVER store Agent C credentials in environment variables!** Your backend should handle Agent C authentication.

For your React app:

```bash
# .env.production
REACT_APP_API_URL=https://your-backend.com  # YOUR backend, not Agent C!
REACT_APP_PUBLIC_URL=https://your-app.com
```

For your backend (Node.js example):

```bash
# Backend .env (NEVER exposed to frontend)
AGENT_C_API_URL=https://api.agentc.com
AGENT_C_API_KEY=your_api_key  # If using API keys
AGENT_C_SECRET=your_secret    # Server-side only!
```

### Production Configuration

```tsx
// productionConfig.ts
import { AuthManager } from '@agentc/realtime-core';

export async function getProductionConfig() {
  try {
    // Get Agent C config from YOUR backend (not Agent C directly!)
    const response = await fetch('/api/agent-c/config', {
      headers: {
        'Authorization': `Bearer ${localStorage.getItem('your_app_token')}`
      }
    });

    if (!response.ok) {
      if (response.status === 401) {
        // User not authenticated with YOUR app
        window.location.href = '/login';
        return null;
      }
      throw new Error('Failed to get configuration');
    }

    const data = await response.json();
    
    // Initialize AuthManager with the payload from YOUR backend
    const authManager = new AuthManager();
    authManager.initializeFromPayload(data.agentC);

    return {
      wsUrl: data.agentC.wsUrl,
      authToken: data.agentC.authToken,
      refreshToken: data.agentC.refreshToken,
      heygenToken: data.agentC.heygenToken,
      enableAudio: true,
      audioConfig: {
        enableInput: true,
        enableOutput: true,
        respectTurnState: true
      },
      reconnection: {
        maxAttempts: 10,
        initialDelay: 1000,
        maxDelay: 30000
      },
      debug: process.env.NODE_ENV !== 'production'
    };
  } catch (error) {
    console.error('Failed to get configuration:', error);
    throw error;
  }
}

// Usage in your app
import { getProductionConfig } from './productionConfig';

function App() {
  const [config, setConfig] = useState<any>(null);
  const [error, setError] = useState<string>('');

  useEffect(() => {
    getProductionConfig()
      .then(setConfig)
      .catch(error => {
        setError('Unable to load chat configuration');
        console.error('Setup failed:', error);
      });
  }, []);

  if (error) {
    return (
      <div className="error">
        <p>{error}</p>
        <a href="/login">Please login</a>
      </div>
    );
  }

  if (!config) {
    return <div>Loading...</div>;
  }

  return (
    <AgentCProvider config={config}>
      <YourApp />
    </AgentCProvider>
  );
}
```

### Your Backend Implementation

Your backend is responsible for managing Agent C authentication:

```javascript
// YOUR BACKEND CODE (Node.js example)
import { ChatUser } from '@agentc/server-sdk';

// Endpoint to provide Agent C config to YOUR frontend
app.get('/api/agent-c/config', authenticateUser, async (req, res) => {
  try {
    // req.user is YOUR authenticated user
    const chatUser = await ChatUser.createOrGet({
      username: `user_${req.user.id}`,
      metadata: {
        email: req.user.email,
        name: req.user.name
      }
    });
    
    const tokens = await chatUser.generateTokens();
    
    res.json({
      agentC: {
        wsUrl: tokens.ws_url,
        authToken: tokens.access_token,
        refreshToken: tokens.refresh_token,
        heygenToken: tokens.heygen_access_token
      }
    });
  } catch (error) {
    console.error('Failed to get Agent C tokens:', error);
    res.status(500).json({ error: 'Configuration error' });
  }
});
```

### Docker Deployment

```dockerfile
# Dockerfile
FROM node:18-alpine

WORKDIR /app

# Copy package files
COPY package*.json ./
RUN npm ci --only=production

# Copy application files
COPY . .

# Build the app
RUN npm run build

# Serve with nginx
FROM nginx:alpine
COPY --from=0 /app/build /usr/share/nginx/html
COPY nginx.conf /etc/nginx/conf.d/default.conf

EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
```

### Security Best Practices

1. **Never expose Agent C credentials** - Frontend should NEVER have username/password
2. **Backend handles authentication** - YOUR backend manages Agent C tokens
3. **Use HTTPS in production** - Ensure all API calls are encrypted
4. **Implement token refresh** - YOUR backend refreshes Agent C tokens
5. **Store tokens securely** - Use httpOnly cookies or secure storage
6. **Validate input** - Always validate user input before sending

```tsx
// Secure token storage example
import { SecureStorage } from '@agentc/realtime-react';

const storage = new SecureStorage({
  encryptionKey: process.env.REACT_APP_ENCRYPTION_KEY,
  storageType: 'sessionStorage' // or 'localStorage' for persistence
});

// Store tokens securely
storage.setItem('auth_token', loginResponse.access_token);
storage.setItem('refresh_token', loginResponse.refresh_token);

// Retrieve tokens
const authToken = storage.getItem('auth_token');
```

## Best Practices Summary

### Authentication
1. **Frontend gets tokens from YOUR backend**, never calls Agent C directly
2. **YOUR backend manages Agent C authentication** and user mapping
3. **Never put Agent C credentials in frontend code** or environment variables
4. **Initialize SDK with payload from YOUR backend** using `initializeFromPayload`
5. **Handle authentication errors** gracefully with user feedback
6. **Store tokens securely** using appropriate storage mechanisms
7. **YOUR backend handles token refresh** with Agent C

### Connection Management
1. **Always handle connection state** and show status to users
2. **Implement reconnection logic** for network failures
3. **Use the WebSocket URL from login response**, not hardcoded values
4. **Clean up connections** on component unmount

### Development
1. **Use localhost:8000** for local development
2. **Provide visual feedback** for all states
3. **Handle errors gracefully** with informative messages
4. **Respect turn management** in voice applications
5. **Use TypeScript** for type safety throughout

### Testing
1. **Mock AuthManager** in tests
2. **Test authentication flows** including failure cases
3. **Verify token refresh** behavior
4. **Test connection handling** and reconnection

### Security
1. **Never expose credentials** in client-side code
2. **Use HTTPS** in production environments
3. **Implement proper logout** to clear tokens
4. **Validate all user input** before sending to server
5. **Monitor for authentication failures** and handle appropriately

### Performance
1. **Lazy load authentication** when needed
2. **Cache authentication state** appropriately
3. **Optimize WebSocket message handling**
4. **Clean up event listeners** to prevent memory leaks
5. **Use React.memo** for expensive components

## Common Authentication Patterns

### Login Flow with Your Backend

```tsx
function LoginPage() {
  const navigate = useNavigate();
  const [credentials, setCredentials] = useState({ email: '', password: '' });
  
  const handleLogin = async () => {
    try {
      // Authenticate with YOUR backend (not Agent C!)
      const response = await fetch('/api/login', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(credentials)
      });
      
      if (!response.ok) {
        throw new Error('Login failed');
      }
      
      const data = await response.json();
      
      // Store YOUR app token
      localStorage.setItem('your_app_token', data.userToken);
      
      // Store Agent C config for the app
      sessionStorage.setItem('agent_c_config', JSON.stringify(data.agentC));
      
      navigate('/chat');
    } catch (error) {
      alert('Login failed');
    }
  };
  
  return (
    <form onSubmit={(e) => { e.preventDefault(); handleLogin(); }}>
      <input
        type="email"
        value={credentials.email}
        onChange={(e) => setCredentials({...credentials, email: e.target.value})}
        placeholder="Email"
        required
      />
      <input
        type="password"
        value={credentials.password}
        onChange={(e) => setCredentials({...credentials, password: e.target.value})}
        placeholder="Password"
        required
      />
      <button type="submit">Login</button>
    </form>
  );
}
```

### Protected Route Pattern

```tsx
import { AuthManager } from '@agentc/realtime-core';
import { Navigate } from 'react-router-dom';

function ProtectedRoute({ children }: { children: React.ReactNode }) {
  const [agentCConfig, setAgentCConfig] = useState<any>(null);
  const [isLoading, setIsLoading] = useState(true);
  
  useEffect(() => {
    // Check if user is authenticated with YOUR app
    const yourAppToken = localStorage.getItem('your_app_token');
    
    if (!yourAppToken) {
      // Not authenticated with YOUR app
      setIsLoading(false);
      return;
    }
    
    // Check for stored Agent C config or fetch it
    const stored = sessionStorage.getItem('agent_c_config');
    
    if (stored) {
      const config = JSON.parse(stored);
      
      // Initialize AuthManager with stored payload
      const authManager = new AuthManager();
      authManager.initializeFromPayload(config);
      
      setAgentCConfig(config);
      setIsLoading(false);
    } else {
      // Fetch from YOUR backend
      fetchAgentCConfig(yourAppToken);
    }
  }, []);
  
  const fetchAgentCConfig = async (token: string) => {
    try {
      const response = await fetch('/api/agent-c/config', {
        headers: { 'Authorization': `Bearer ${token}` }
      });
      
      if (response.ok) {
        const data = await response.json();
        
        // Initialize AuthManager
        const authManager = new AuthManager();
        authManager.initializeFromPayload(data.agentC);
        
        sessionStorage.setItem('agent_c_config', JSON.stringify(data.agentC));
        setAgentCConfig(data.agentC);
      }
    } catch (error) {
      console.error('Failed to get Agent C config:', error);
    } finally {
      setIsLoading(false);
    }
  };
  
  if (isLoading) return <div>Loading...</div>;
  if (!agentCConfig) return <Navigate to="/login" />;
  
  return (
    <AgentCProvider config={agentCConfig}>
      {children}
    </AgentCProvider>
  );
}

// Usage
<Routes>
  <Route path="/login" element={<LoginPage />} />
  <Route path="/chat" element={
    <ProtectedRoute>
      <ChatInterface />
    </ProtectedRoute>
  } />
</Routes>
```

### Logout Pattern

```tsx
function LogoutButton() {
  const { disconnect } = useConnection();
  const navigate = useNavigate();
  
  const handleLogout = async () => {
    // Disconnect WebSocket
    await disconnect();
    
    // Clear YOUR app's authentication
    localStorage.removeItem('your_app_token');
    
    // Clear Agent C config
    sessionStorage.removeItem('agent_c_config');
    
    // Optional: Notify YOUR backend about logout
    try {
      await fetch('/api/logout', {
        method: 'POST',
        headers: {
          'Authorization': `Bearer ${localStorage.getItem('your_app_token')}`
        }
      });
    } catch (error) {
      console.error('Logout notification failed:', error);
    }
    
    // Redirect to login
    navigate('/login');
  };
  
  return <button onClick={handleLogout}>Logout</button>;
}
```

---

## Using Additional Hooks

Examples of other useful hooks provided by the SDK.

### Using useAgentCData

Access all initialization data from the WebSocket connection.

```tsx
import React from 'react';
import { useAgentCData, useConnection } from '@agentc/realtime-react';

function SystemInfo() {
  const { isConnected } = useConnection();
  const { 
    agents, 
    voices, 
    avatars, 
    tools,
    isLoading,
    error 
  } = useAgentCData();
  
  if (!isConnected) {
    return <div>Not connected</div>;
  }
  
  if (isLoading) {
    return <div>Loading system data...</div>;
  }
  
  if (error) {
    return <div>Error loading data: {error}</div>;
  }
  
  return (
    <div className="system-info">
      <section>
        <h3>Available Agents ({agents.length})</h3>
        <ul>
          {agents.map(agent => (
            <li key={agent.agent_id}>
              <strong>{agent.name}</strong>
              <p>{agent.description}</p>
              <code>Key: {agent.key}</code>
            </li>
          ))}
        </ul>
      </section>
      
      <section>
        <h3>Available Voices ({voices.length})</h3>
        <ul>
          {voices.map(voice => (
            <li key={voice.voice_id}>
              {voice.name} ({voice.vendor})
              {voice.is_default && ' ✓ Default'}
            </li>
          ))}
        </ul>
      </section>
      
      <section>
        <h3>Available Avatars ({avatars.length})</h3>
        <ul>
          {avatars.map(avatar => (
            <li key={avatar.avatar_id}>
              {avatar.name}
              {avatar.preview_url && (
                <img src={avatar.preview_url} alt={avatar.name} width="50" />
              )}
            </li>
          ))}
        </ul>
      </section>
      
      <section>
        <h3>Available Tools ({tools.length})</h3>
        <ul>
          {tools.map(tool => (
            <li key={tool.name}>
              <strong>{tool.name}</strong>
              <p>{tool.description}</p>
            </li>
          ))}
        </ul>
      </section>
    </div>
  );
}
```

### Using useUserData

Access current user information from the WebSocket connection.

```tsx
import React from 'react';
import { useUserData } from '@agentc/realtime-react';

function UserProfile() {
  const { user, isLoading, error } = useUserData();
  
  if (isLoading) {
    return <div>Loading user data...</div>;
  }
  
  if (error) {
    return <div>Error: {error}</div>;
  }
  
  if (!user) {
    return <div>No user data available</div>;
  }
  
  return (
    <div className="user-profile">
      <h2>User Profile</h2>
      <dl>
        <dt>Username:</dt>
        <dd>{user.username}</dd>
        
        <dt>User ID:</dt>
        <dd>{user.user_id}</dd>
        
        <dt>Account Type:</dt>
        <dd>{user.is_authenticated ? 'Authenticated' : 'Anonymous'}</dd>
        
        {user.metadata && (
          <>
            <dt>Metadata:</dt>
            <dd>
              <pre>{JSON.stringify(user.metadata, null, 2)}</pre>
            </dd>
          </>
        )}
      </dl>
    </div>
  );
}
```

### Using useInitializationStatus

Track the detailed initialization status of the connection.

```tsx
import React from 'react';
import { useInitializationStatus, ConnectionState } from '@agentc/realtime-react';

function ConnectionStatus() {
  const { 
    connectionState, 
    isInitialized, 
    initializationError 
  } = useInitializationStatus();
  
  const getStatusIcon = () => {
    switch (connectionState) {
      case ConnectionState.DISCONNECTED:
        return '🔴';
      case ConnectionState.CONNECTING:
        return '🟡';
      case ConnectionState.AUTHENTICATING:
        return '🟠';
      case ConnectionState.INITIALIZING:
        return '🔵';
      case ConnectionState.READY:
        return '🟢';
      case ConnectionState.ERROR:
        return '❌';
      default:
        return '⚪';
    }
  };
  
  return (
    <div className="connection-status">
      <div className="status-indicator">
        <span className="icon">{getStatusIcon()}</span>
        <span className="state">{connectionState}</span>
      </div>
      
      {!isInitialized && connectionState === ConnectionState.INITIALIZING && (
        <div className="initialization-progress">
          <div className="spinner" />
          <span>Initializing WebSocket connection...</span>
        </div>
      )}
      
      {initializationError && (
        <div className="error-message">
          <strong>Initialization Error:</strong>
          <p>{initializationError}</p>
        </div>
      )}
      
      {isInitialized && (
        <div className="ready-message">
          ✅ Ready to communicate
        </div>
      )}
    </div>
  );
}
```

### Using useChatSessionList

Manage and display grouped chat sessions.

```tsx
import React, { useState } from 'react';
import { useChatSessionList, useChat } from '@agentc/realtime-react';

function SessionExplorer() {
  const {
    groupedSessions,
    totalSessions,
    refreshSessions,
    isLoading
  } = useChatSessionList({
    groupBy: 'date',
    sortOrder: 'desc',
    includeEmpty: false
  });
  
  const { resumeSession, currentSessionId } = useChat();
  const [expandedGroups, setExpandedGroups] = useState<Set<string>>(new Set());
  
  const toggleGroup = (groupKey: string) => {
    const newExpanded = new Set(expandedGroups);
    if (newExpanded.has(groupKey)) {
      newExpanded.delete(groupKey);
    } else {
      newExpanded.add(groupKey);
    }
    setExpandedGroups(newExpanded);
  };
  
  if (isLoading) {
    return <div>Loading sessions...</div>;
  }
  
  return (
    <div className="session-explorer">
      <header>
        <h2>Chat Sessions ({totalSessions})</h2>
        <button onClick={refreshSessions}>🔄 Refresh</button>
      </header>
      
      {Object.entries(groupedSessions).map(([groupKey, group]) => (
        <div key={groupKey} className="session-group">
          <button 
            className="group-header"
            onClick={() => toggleGroup(groupKey)}
          >
            <span className="arrow">
              {expandedGroups.has(groupKey) ? '▼' : '▶'}
            </span>
            <span className="group-title">{group.title}</span>
            <span className="group-count">({group.sessions.length})</span>
          </button>
          
          {expandedGroups.has(groupKey) && (
            <div className="group-sessions">
              {group.sessions.map(session => (
                <div 
                  key={session.session_id}
                  className={`session-item ${
                    session.session_id === currentSessionId ? 'active' : ''
                  }`}
                  onClick={() => resumeSession(session.session_id)}
                >
                  <div className="session-name">
                    {session.session_name || `Session ${session.session_id.slice(0, 8)}`}
                  </div>
                  <div className="session-details">
                    <span>{session.messages.length} messages</span>
                    <span>•</span>
                    <span>{new Date(session.updated_at).toLocaleTimeString()}</span>
                  </div>
                  {session.messages.length > 0 && (
                    <div className="last-message">
                      {session.messages[session.messages.length - 1].content.slice(0, 50)}...
                    </div>
                  )}
                </div>
              ))}
            </div>
          )}
        </div>
      ))}
    </div>
  );
}
```

### Using useOutputMode

Coordinate output modes between text, audio, and avatar.

```tsx
import React from 'react';
import { useOutputMode, OutputMode } from '@agentc/realtime-react';

function OutputModeSelector() {
  const {
    currentMode,
    availableModes,
    setOutputMode,
    isAudioEnabled,
    isAvatarActive,
    canUseAudio,
    canUseAvatar
  } = useOutputMode();
  
  const handleModeChange = (mode: OutputMode) => {
    if (mode === OutputMode.AUDIO && !canUseAudio) {
      alert('Audio is not available. Please check your microphone settings.');
      return;
    }
    
    if (mode === OutputMode.AVATAR && !canUseAvatar) {
      alert('Avatar mode requires an active avatar session.');
      return;
    }
    
    setOutputMode(mode);
  };
  
  return (
    <div className="output-mode-selector">
      <h3>Output Mode</h3>
      
      <div className="mode-buttons">
        {availableModes.map(mode => (
          <button
            key={mode}
            className={`mode-button ${currentMode === mode ? 'active' : ''}`}
            onClick={() => handleModeChange(mode)}
            disabled={
              (mode === OutputMode.AUDIO && !canUseAudio) ||
              (mode === OutputMode.AVATAR && !canUseAvatar)
            }
          >
            {mode === OutputMode.TEXT && '📝 Text Only'}
            {mode === OutputMode.AUDIO && '🔊 Audio'}
            {mode === OutputMode.AVATAR && '🎭 Avatar'}
          </button>
        ))}
      </div>
      
      <div className="mode-status">
        <p>Current: <strong>{currentMode}</strong></p>
        {isAudioEnabled && <p>✅ Audio enabled</p>}
        {isAvatarActive && <p>✅ Avatar active</p>}
      </div>
    </div>
  );
}
```

### Using useToolNotifications

Display notifications for tool calls made by the agent.

```tsx
import React, { useEffect } from 'react';
import { useToolNotifications } from '@agentc/realtime-react';

function ToolNotificationPanel() {
  const {
    notifications,
    clearNotification,
    clearAll
  } = useToolNotifications({
    maxNotifications: 10,
    autoDismiss: 5000 // Auto-dismiss after 5 seconds
  });
  
  return (
    <div className="tool-notifications">
      <header>
        <h3>Tool Activity</h3>
        {notifications.length > 0 && (
          <button onClick={clearAll}>Clear All</button>
        )}
      </header>
      
      <div className="notifications-list">
        {notifications.length === 0 && (
          <p className="no-notifications">No recent tool activity</p>
        )}
        
        {notifications.map(notification => (
          <div 
            key={notification.id}
            className={`notification ${notification.status}`}
          >
            <button 
              className="close-btn"
              onClick={() => clearNotification(notification.id)}
            >
              ×
            </button>
            
            <div className="notification-header">
              <span className="tool-name">{notification.tool_name}</span>
              <span className="timestamp">
                {new Date(notification.timestamp).toLocaleTimeString()}
              </span>
            </div>
            
            {notification.status === 'pending' && (
              <div className="pending">
                <div className="spinner" />
                Executing...
              </div>
            )}
            
            {notification.status === 'success' && notification.result && (
              <div className="result success">
                ✅ {typeof notification.result === 'string' 
                  ? notification.result 
                  : JSON.stringify(notification.result, null, 2)}
              </div>
            )}
            
            {notification.status === 'error' && notification.error && (
              <div className="result error">
                ❌ {notification.error}
              </div>
            )}
            
            {notification.arguments && (
              <details className="arguments">
                <summary>Arguments</summary>
                <pre>{JSON.stringify(notification.arguments, null, 2)}</pre>
              </details>
            )}
          </div>
        ))}
      </div>
    </div>
  );
}
```

---

## Error Handling & Edge Cases

Robust error handling patterns for production applications.

### Connection Error Recovery

```tsx
import React, { useState, useEffect } from 'react';
import { 
  AgentCProvider, 
  useConnection,
  useInitializationStatus,
  ConnectionState 
} from '@agentc/realtime-react';
import { AuthManager } from '@agentc/realtime-core';

function ResilientApp() {
  const [config, setConfig] = useState<any>(null);
  const [authError, setAuthError] = useState<string>('');
  const [retryCount, setRetryCount] = useState(0);
  const maxRetries = 3;
  
  const loadConfiguration = async () => {
    try {
      setAuthError('');
      
      // Get config from your backend
      const response = await fetch('/api/agent-c/config', {
        headers: {
          'Authorization': `Bearer ${localStorage.getItem('your_app_token')}`
        }
      });
      
      if (response.status === 401) {
        // Token expired, try to refresh
        const refreshed = await refreshYourAppToken();
        if (!refreshed) {
          throw new Error('Session expired. Please log in again.');
        }
        // Retry with new token
        return loadConfiguration();
      }
      
      if (!response.ok) {
        throw new Error(`Configuration failed: ${response.statusText}`);
      }
      
      const data = await response.json();
      
      // Initialize AuthManager
      const authManager = new AuthManager();
      authManager.initializeFromPayload(data.agentC);
      
      setConfig({
        wsUrl: data.agentC.wsUrl,
        authToken: data.agentC.authToken,
        refreshToken: data.agentC.refreshToken,
        reconnection: {
          maxAttempts: 10,
          initialDelay: 1000,
          maxDelay: 30000,
          onReconnectFailed: () => {
            console.error('Failed to reconnect after max attempts');
            // Try to get fresh tokens
            if (retryCount < maxRetries) {
              setRetryCount(retryCount + 1);
              setTimeout(() => loadConfiguration(), 5000);
            }
          }
        }
      });
      
      setRetryCount(0); // Reset on success
      
    } catch (error: any) {
      console.error('Configuration error:', error);
      setAuthError(error.message || 'Failed to load configuration');
      
      // Retry logic
      if (retryCount < maxRetries) {
        setTimeout(() => {
          setRetryCount(retryCount + 1);
          loadConfiguration();
        }, 5000 * Math.pow(2, retryCount)); // Exponential backoff
      }
    }
  };
  
  useEffect(() => {
    loadConfiguration();
  }, []);
  
  if (authError) {
    return (
      <div className="error-container">
        <h2>Connection Error</h2>
        <p>{authError}</p>
        {retryCount < maxRetries ? (
          <p>Retrying... (Attempt {retryCount + 1} of {maxRetries})</p>
        ) : (
          <>
            <p>Maximum retry attempts reached.</p>
            <button onClick={() => {
              setRetryCount(0);
              loadConfiguration();
            }}>Try Again</button>
            <a href="/login">Return to Login</a>
          </>
        )}
      </div>
    );
  }
  
  if (!config) {
    return <div>Initializing...</div>;
  }
  
  return (
    <AgentCProvider config={config}>
      <ConnectionMonitor />
      <YourApp />
    </AgentCProvider>
  );
}

function ConnectionMonitor() {
  const { 
    isConnected, 
    connectionState, 
    error,
    reconnectAttempt,
    connect 
  } = useConnection();
  
  const { connectionState: detailedState } = useInitializationStatus();
  
  // Show connection issues to user
  if (error) {
    return (
      <div className="connection-banner error">
        <span>Connection error: {error.message}</span>
        <button onClick={() => connect()}>Reconnect</button>
      </div>
    );
  }
  
  if (reconnectAttempt > 0) {
    return (
      <div className="connection-banner warning">
        <span>Reconnecting... (Attempt {reconnectAttempt})</span>
      </div>
    );
  }
  
  if (detailedState === ConnectionState.AUTHENTICATING) {
    return (
      <div className="connection-banner info">
        <span>Authenticating...</span>
      </div>
    );
  }
  
  return null;
}

// Helper function to refresh your app's token
async function refreshYourAppToken(): Promise<boolean> {
  try {
    const refreshToken = localStorage.getItem('your_refresh_token');
    if (!refreshToken) return false;
    
    const response = await fetch('/api/refresh', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ refresh_token: refreshToken })
    });
    
    if (response.ok) {
      const data = await response.json();
      localStorage.setItem('your_app_token', data.access_token);
      localStorage.setItem('your_refresh_token', data.refresh_token);
      return true;
    }
  } catch (error) {
    console.error('Token refresh failed:', error);
  }
  return false;
}
```

### Audio Permission Handling

```tsx
import React, { useState, useEffect } from 'react';
import { useAudio } from '@agentc/realtime-react';

function AudioPermissionHandler() {
  const { 
    hasPermission,
    error,
    startRecording,
    stopRecording,
    isRecording 
  } = useAudio();
  
  const [permissionStatus, setPermissionStatus] = useState<string>('unknown');
  const [isRequesting, setIsRequesting] = useState(false);
  
  useEffect(() => {
    // Check browser permission API if available
    if ('permissions' in navigator) {
      navigator.permissions.query({ name: 'microphone' as PermissionName })
        .then(result => {
          setPermissionStatus(result.state);
          
          result.addEventListener('change', () => {
            setPermissionStatus(result.state);
          });
        })
        .catch(() => {
          // Permission API not supported or error
          setPermissionStatus('unknown');
        });
    }
  }, []);
  
  const requestPermission = async () => {
    setIsRequesting(true);
    
    try {
      // This will trigger the browser's permission prompt
      await startRecording();
      
      // If we get here, permission was granted
      setPermissionStatus('granted');
      
      // Stop recording immediately - we just wanted permission
      stopRecording();
      
    } catch (err: any) {
      console.error('Microphone permission error:', err);
      
      if (err.name === 'NotAllowedError' || err.name === 'PermissionDeniedError') {
        setPermissionStatus('denied');
        
        // Show helpful message based on browser
        const isChrome = /Chrome/.test(navigator.userAgent);
        const isFirefox = /Firefox/.test(navigator.userAgent);
        const isSafari = /Safari/.test(navigator.userAgent) && !/Chrome/.test(navigator.userAgent);
        
        let helpMessage = 'Microphone access was denied. ';
        
        if (isChrome) {
          helpMessage += 'Click the camera icon in the address bar to change permissions.';
        } else if (isFirefox) {
          helpMessage += 'Click the microphone icon in the address bar to change permissions.';
        } else if (isSafari) {
          helpMessage += 'Go to Safari > Preferences > Websites > Microphone to change permissions.';
        } else {
          helpMessage += 'Please check your browser settings to allow microphone access.';
        }
        
        alert(helpMessage);
        
      } else if (err.name === 'NotFoundError' || err.name === 'DevicesNotFoundError') {
        setPermissionStatus('no-device');
        alert('No microphone found. Please connect a microphone and try again.');
        
      } else if (err.name === 'NotReadableError' || err.name === 'TrackStartError') {
        setPermissionStatus('in-use');
        alert('Microphone is already in use by another application.');
        
      } else {
        setPermissionStatus('error');
        alert(`Microphone error: ${err.message}`);
      }
    } finally {
      setIsRequesting(false);
    }
  };
  
  // Already have permission
  if (hasPermission) {
    return (
      <div className="permission-status granted">
        ✅ Microphone access granted
      </div>
    );
  }
  
  // Permission explicitly denied
  if (permissionStatus === 'denied') {
    return (
      <div className="permission-status denied">
        <p>❌ Microphone access denied</p>
        <p>Please enable microphone access in your browser settings to use voice features.</p>
        <button onClick={requestPermission}>Try Again</button>
      </div>
    );
  }
  
  // No microphone detected
  if (permissionStatus === 'no-device') {
    return (
      <div className="permission-status no-device">
        <p>🎤 No microphone detected</p>
        <p>Please connect a microphone to use voice features.</p>
        <button onClick={requestPermission}>Check Again</button>
      </div>
    );
  }
  
  // Microphone in use by another app
  if (permissionStatus === 'in-use') {
    return (
      <div className="permission-status in-use">
        <p>🔒 Microphone is in use</p>
        <p>Please close other applications using the microphone.</p>
        <button onClick={requestPermission}>Try Again</button>
      </div>
    );
  }
  
  // Permission not yet requested or unknown
  return (
    <div className="permission-status prompt">
      <p>🎤 Voice features require microphone access</p>
      <button 
        onClick={requestPermission}
        disabled={isRequesting}
        className="request-permission-btn"
      >
        {isRequesting ? 'Requesting...' : 'Enable Microphone'}
      </button>
      {error && (
        <p className="error-message">{error.message}</p>
      )}
    </div>
  );
}
```

---

## StrictMode Compatibility

The SDK is fully compatible with React StrictMode, handling double-mounting correctly.

```tsx
import React, { StrictMode } from 'react';
import { createRoot } from 'react-dom/client';
import { AgentCProvider } from '@agentc/realtime-react';
import { AuthManager } from '@agentc/realtime-core';

function App() {
  // The provider handles StrictMode double-mounting automatically
  // It will NOT create duplicate connections or cause issues
  
  return (
    <StrictMode>
      <AgentCProvider 
        config={{
          wsUrl: 'wss://api.agentc.com/rt/ws',
          authToken: 'your-token',
          refreshToken: 'your-refresh-token',
          debug: true // Will show StrictMode handling in console
        }}
      >
        <YourApplication />
      </AgentCProvider>
    </StrictMode>
  );
}

const container = document.getElementById('root')!;
const root = createRoot(container);
root.render(<App />);
```

### Testing with StrictMode

```tsx
import { render, waitFor } from '@testing-library/react';
import { StrictMode } from 'react';
import { AgentCProvider, useConnection } from '@agentc/realtime-react';

function TestComponent() {
  const { isConnected, connectionState } = useConnection();
  return (
    <div>
      <span data-testid="connected">{String(isConnected)}</span>
      <span data-testid="state">{connectionState}</span>
    </div>
  );
}

test('handles StrictMode double-mounting correctly', async () => {
  const mockConfig = {
    wsUrl: 'wss://test.api.com/ws',
    authToken: 'test-token',
    refreshToken: 'test-refresh'
  };
  
  // Render with StrictMode
  const { getByTestId, unmount } = render(
    <StrictMode>
      <AgentCProvider config={mockConfig}>
        <TestComponent />
      </AgentCProvider>
    </StrictMode>
  );
  
  // Should only create one connection despite double-mounting
  await waitFor(() => {
    expect(getByTestId('state')).toHaveTextContent('CONNECTING');
  });
  
  // Unmounting should cleanup properly
  unmount();
  
  // No lingering connections or memory leaks
});
```

---

## Development-Only Examples

⚠️ **WARNING**: The following patterns are for DEVELOPMENT AND TESTING ONLY. Never use these patterns in production!

### Direct Login (Development Only)

**NEVER USE THIS IN PRODUCTION!** This pattern exposes credentials in frontend code.

```tsx
// DEV-ONLY-LoginExample.tsx
// ⚠️ DEVELOPMENT ONLY - DO NOT USE IN PRODUCTION ⚠️
import React, { useState } from 'react';
import { AgentCProvider } from '@agentc/realtime-react';
import { AuthManager } from '@agentc/realtime-core';

function DevOnlyApp() {
  const [config, setConfig] = useState<any>(null);
  const [credentials, setCredentials] = useState({
    username: '',
    password: ''
  });

  // ⚠️ BAD PATTERN - Direct login from frontend
  // Only use this for local development!
  const handleDirectLogin = async () => {
    console.warn('Using development-only direct login!');
    
    const authManager = new AuthManager({
      apiUrl: 'https://localhost:8000' // Local dev server
    });
    
    try {
      // ⚠️ NEVER do this in production!
      const loginResponse = await authManager.login({
        username: credentials.username,
        password: credentials.password
      });
      
      setConfig({
        wsUrl: loginResponse.ws_url,
        authToken: loginResponse.access_token,
        refreshToken: loginResponse.refresh_token,
        heygenToken: loginResponse.heygen_access_token
      });
    } catch (error) {
      console.error('Dev login failed:', error);
      alert('Login failed. Check your dev credentials.');
    }
  };

  if (!config) {
    return (
      <div className="dev-login">
        <h2>⚠️ Development Login</h2>
        <p style={{color: 'red'}}>This is for development only!</p>
        <input
          type="text"
          placeholder="Dev Username"
          value={credentials.username}
          onChange={(e) => setCredentials({...credentials, username: e.target.value})}
        />
        <input
          type="password"
          placeholder="Dev Password"
          value={credentials.password}
          onChange={(e) => setCredentials({...credentials, password: e.target.value})}
        />
        <button onClick={handleDirectLogin}>Dev Login</button>
      </div>
    );
  }

  return (
    <AgentCProvider config={config}>
      <YourDevApp />
    </AgentCProvider>
  );
}
```

### Environment Variables (Development Only)

**NEVER store real credentials in environment variables!** Use this only for local development with test accounts.

```bash
# .env.development.local
# ⚠️ DEVELOPMENT ONLY - NEVER commit this file!
REACT_APP_DEV_AGENT_C_URL=https://localhost:8000
REACT_APP_DEV_USERNAME=test_user  # Test account only!
REACT_APP_DEV_PASSWORD=test_pass  # Test account only!
```

```tsx
// DevOnlyAutoLogin.tsx
// ⚠️ DEVELOPMENT ONLY - DO NOT USE IN PRODUCTION ⚠️
import React, { useState, useEffect } from 'react';
import { AgentCProvider } from '@agentc/realtime-react';
import { AuthManager } from '@agentc/realtime-core';

function DevOnlyAutoLogin() {
  const [config, setConfig] = useState<any>(null);

  useEffect(() => {
    if (process.env.NODE_ENV !== 'development') {
      throw new Error('This component is for development only!');
    }

    const devLogin = async () => {
      const authManager = new AuthManager({
        apiUrl: process.env.REACT_APP_DEV_AGENT_C_URL
      });

      try {
        // ⚠️ Only for development with test accounts!
        const response = await authManager.login({
          username: process.env.REACT_APP_DEV_USERNAME!,
          password: process.env.REACT_APP_DEV_PASSWORD!
        });

        setConfig({
          wsUrl: response.ws_url,
          authToken: response.access_token,
          refreshToken: response.refresh_token
        });
      } catch (error) {
        console.error('Dev auto-login failed:', error);
      }
    };

    devLogin();
  }, []);

  if (!config) {
    return <div>Dev: Auto-authenticating...</div>;
  }

  return (
    <AgentCProvider config={config}>
      <YourDevApp />
    </AgentCProvider>
  );
}
```

### Local Development Script

For local development, you might create a script that generates tokens:

```javascript
// scripts/dev-tokens.js
// Run this locally to generate dev tokens
const { ChatUser } = require('@agentc/server-sdk');

async function generateDevTokens() {
  // This runs locally, not in production
  const chatUser = await ChatUser.createOrGet({
    username: 'dev_user',
    metadata: { dev: true }
  });
  
  const tokens = await chatUser.generateTokens();
  
  console.log('Dev tokens for frontend testing:');
  console.log(JSON.stringify(tokens, null, 2));
  console.log('\nPaste this into your dev app for testing.');
}

generateDevTokens();
```

### Why These Patterns Are Dangerous in Production

1. **Security Risk**: Credentials in frontend code can be extracted by anyone
2. **No User Management**: Can't map Agent C users to your app's users
3. **No Access Control**: Can't control who accesses Agent C
4. **No Audit Trail**: Can't track usage per user
5. **Token Leakage**: Tokens visible in browser DevTools
6. **Credential Rotation**: Can't rotate credentials without updating all clients

### Migrating from Development to Production

When moving from development to production:

1. **Remove all direct login code**
2. **Implement backend authentication endpoint**
3. **Map your users to Agent C ChatUsers**
4. **Never expose Agent C credentials**
5. **Use the production pattern shown above**

Example migration checklist:
- [ ] Backend endpoint for Agent C token generation
- [ ] User authentication in YOUR backend
- [ ] Remove all `authManager.login()` calls from frontend
- [ ] Replace with `authManager.initializeFromPayload()`
- [ ] Remove Agent C credentials from environment variables
- [ ] Update all components to use backend-provided tokens
- [ ] Test token refresh through YOUR backend
- [ ] Verify no credentials in frontend bundle

---

## Complete Application Example

Here's a complete, production-ready application that combines all the patterns:

```tsx
// App.tsx - Complete Production Application
import React, { Suspense, lazy, useState, useEffect } from 'react';
import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import { 
  AgentCProvider,
  useConnection,
  useChat,
  useAudio,
  useTurnState,
  useVoiceModel,
  useInitializationStatus,
  ConnectionState
} from '@agentc/realtime-react';
import { AuthManager } from '@agentc/realtime-core';

// Lazy load components for better performance
const ChatInterface = lazy(() => import('./components/ChatInterface'));
const VoiceAssistant = lazy(() => import('./components/VoiceAssistant'));
const AvatarChat = lazy(() => import('./components/AvatarChat'));
const AdminDashboard = lazy(() => import('./components/AdminDashboard'));

// Main App Component
function App() {
  const [authState, setAuthState] = useState<'loading' | 'authenticated' | 'unauthenticated'>('loading');
  const [agentCConfig, setAgentCConfig] = useState<any>(null);
  const [error, setError] = useState<string>('');

  useEffect(() => {
    checkAuthentication();
  }, []);

  const checkAuthentication = async () => {
    try {
      const token = localStorage.getItem('your_app_token');
      
      if (!token) {
        setAuthState('unauthenticated');
        return;
      }

      // Verify token with YOUR backend
      const response = await fetch('/api/verify', {
        headers: { 'Authorization': `Bearer ${token}` }
      });

      if (response.ok) {
        await loadAgentCConfig(token);
        setAuthState('authenticated');
      } else {
        setAuthState('unauthenticated');
      }
    } catch (error) {
      console.error('Auth check failed:', error);
      setAuthState('unauthenticated');
    }
  };

  const loadAgentCConfig = async (token: string) => {
    try {
      const response = await fetch('/api/agent-c/config', {
        headers: { 'Authorization': `Bearer ${token}` }
      });

      if (!response.ok) {
        throw new Error('Failed to load Agent C configuration');
      }

      const data = await response.json();
      
      // Initialize AuthManager with backend payload
      const authManager = new AuthManager();
      authManager.initializeFromPayload(data.agentC);
      
      setAgentCConfig({
        wsUrl: data.agentC.wsUrl,
        authToken: data.agentC.authToken,
        refreshToken: data.agentC.refreshToken,
        heygenToken: data.agentC.heygenToken,
        enableAudio: true,
        audioConfig: {
          enableInput: true,
          enableOutput: true,
          respectTurnState: true,
          initialVolume: 0.8
        },
        reconnection: {
          maxAttempts: 10,
          initialDelay: 1000,
          maxDelay: 30000
        },
        debug: process.env.NODE_ENV === 'development'
      });
    } catch (error: any) {
      console.error('Failed to load Agent C config:', error);
      setError(error.message);
    }
  };

  if (authState === 'loading') {
    return <LoadingScreen />;
  }

  if (authState === 'unauthenticated') {
    return <LoginPage onSuccess={() => checkAuthentication()} />;
  }

  if (error) {
    return (
      <ErrorBoundary error={error} onRetry={() => {
        setError('');
        checkAuthentication();
      }} />
    );
  }

  if (!agentCConfig) {
    return <div>Initializing chat system...</div>;
  }

  return (
    <AgentCProvider config={agentCConfig}>
      <Router>
        <AppLayout>
          <Suspense fallback={<LoadingScreen />}>
            <Routes>
              <Route path="/" element={<Navigate to="/chat" replace />} />
              <Route path="/chat" element={<ChatInterface />} />
              <Route path="/voice" element={<VoiceAssistant />} />
              <Route path="/avatar" element={<AvatarChat />} />
              <Route path="/admin" element={<AdminDashboard />} />
              <Route path="*" element={<NotFound />} />
            </Routes>
          </Suspense>
        </AppLayout>
      </Router>
    </AgentCProvider>
  );
}

// App Layout with Navigation and Status
function AppLayout({ children }: { children: React.ReactNode }) {
  const { isConnected, connectionState } = useConnection();
  const { connectionState: detailedState } = useInitializationStatus();
  const { currentTurn } = useTurnState();
  const { audioLevel, isRecording } = useAudio();

  return (
    <div className="app-layout">
      <header className="app-header">
        <nav className="main-nav">
          <a href="/chat">Chat</a>
          <a href="/voice">Voice</a>
          <a href="/avatar">Avatar</a>
          <a href="/admin">Admin</a>
        </nav>
        
        <div className="status-bar">
          <ConnectionStatus 
            isConnected={isConnected}
            connectionState={connectionState}
            detailedState={detailedState}
          />
          <TurnIndicator currentTurn={currentTurn} />
          <AudioIndicator 
            audioLevel={audioLevel}
            isRecording={isRecording}
          />
        </div>
      </header>
      
      <main className="app-content">
        {children}
      </main>
      
      <footer className="app-footer">
        <QuickActions />
      </footer>
    </div>
  );
}

// Connection Status Component
function ConnectionStatus({ 
  isConnected, 
  connectionState, 
  detailedState 
}: {
  isConnected: boolean;
  connectionState: string;
  detailedState: ConnectionState;
}) {
  const getStatusColor = () => {
    if (!isConnected) return 'red';
    if (detailedState === ConnectionState.READY) return 'green';
    if (detailedState === ConnectionState.AUTHENTICATING) return 'yellow';
    return 'orange';
  };

  return (
    <div className={`connection-status ${getStatusColor()}`}>
      <span className="status-dot" />
      <span className="status-text">{connectionState}</span>
    </div>
  );
}

// Quick Actions Bar
function QuickActions() {
  const { newSession, clearMessages } = useChat();
  const { disconnect, connect, isConnected } = useConnection();
  const { startRecording, stopRecording, isRecording } = useAudio();

  return (
    <div className="quick-actions">
      <button onClick={() => newSession()}>
        🆕 New Session
      </button>
      <button onClick={clearMessages}>
        🗑️ Clear Chat
      </button>
      <button onClick={() => isConnected ? disconnect() : connect()}>
        {isConnected ? '🔌 Disconnect' : '🔋 Connect'}
      </button>
      <button onClick={() => isRecording ? stopRecording() : startRecording()}>
        {isRecording ? '🔴 Stop' : '🎤 Record'}
      </button>
    </div>
  );
}

// Helper Components
function LoadingScreen() {
  return (
    <div className="loading-screen">
      <div className="spinner" />
      <p>Loading...</p>
    </div>
  );
}

function LoginPage({ onSuccess }: { onSuccess: () => void }) {
  const [credentials, setCredentials] = useState({ email: '', password: '' });
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState('');

  const handleLogin = async (e: React.FormEvent) => {
    e.preventDefault();
    setIsLoading(true);
    setError('');

    try {
      const response = await fetch('/api/login', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(credentials)
      });

      if (!response.ok) {
        throw new Error('Invalid credentials');
      }

      const data = await response.json();
      localStorage.setItem('your_app_token', data.token);
      localStorage.setItem('your_refresh_token', data.refreshToken);
      
      onSuccess();
    } catch (error: any) {
      setError(error.message || 'Login failed');
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div className="login-page">
      <form onSubmit={handleLogin}>
        <h1>Login</h1>
        {error && <div className="error">{error}</div>}
        <input
          type="email"
          placeholder="Email"
          value={credentials.email}
          onChange={(e) => setCredentials({...credentials, email: e.target.value})}
          required
        />
        <input
          type="password"
          placeholder="Password"
          value={credentials.password}
          onChange={(e) => setCredentials({...credentials, password: e.target.value})}
          required
        />
        <button type="submit" disabled={isLoading}>
          {isLoading ? 'Logging in...' : 'Login'}
        </button>
      </form>
    </div>
  );
}

function ErrorBoundary({ error, onRetry }: { error: string; onRetry: () => void }) {
  return (
    <div className="error-boundary">
      <h1>Something went wrong</h1>
      <p>{error}</p>
      <button onClick={onRetry}>Try Again</button>
    </div>
  );
}

function TurnIndicator({ currentTurn }: { currentTurn: string }) {
  return (
    <div className={`turn-indicator ${currentTurn}`}>
      {currentTurn === 'user_turn' && '🎤 Your Turn'}
      {currentTurn === 'agent_turn' && '🤖 Agent Speaking'}
      {!currentTurn && '⏸️ Ready'}
    </div>
  );
}

function AudioIndicator({ audioLevel, isRecording }: { audioLevel: number; isRecording: boolean }) {
  if (!isRecording) return null;
  
  return (
    <div className="audio-indicator">
      <div 
        className="audio-bar" 
        style={{ width: `${audioLevel * 100}%` }}
      />
    </div>
  );
}

function NotFound() {
  return (
    <div className="not-found">
      <h1>404 - Page Not Found</h1>
      <a href="/">Go Home</a>
    </div>
  );
}

export default App;
```

## Summary

This documentation provides comprehensive examples for building production-ready applications with the Agent C Realtime React SDK. Key takeaways:

1. **Always use backend authentication** - Never expose Agent C credentials in frontend code
2. **Use the correct imports** - `AuthManager` is from `@agentc/realtime-core`
3. **Handle errors gracefully** - Provide user feedback and recovery options
4. **Follow React best practices** - Use hooks correctly, handle StrictMode, clean up effects
5. **Test thoroughly** - Mock appropriately and test edge cases
6. **Consider performance** - Use lazy loading, memoization, and proper state management

For more details on individual hooks, see the [hooks documentation](./hooks/). For the core SDK documentation, see [@agentc/realtime-core](../core/).