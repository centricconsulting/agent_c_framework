# Avatar Integration Guide

This guide covers integrating HeyGen streaming avatars with the Agent C Realtime SDK for creating interactive virtual representatives.

## Overview

Agent C supports HeyGen streaming avatars, providing:
- **Real-time video streaming** of virtual avatars
- **Synchronized lip-sync** with Agent C responses
- **Multiple avatar options** with different styles
- **Automatic voice coordination** between systems

## Prerequisites

### Required Accounts
1. **Agent C Account** with avatar feature enabled
2. **HeyGen API Access** (provided through Agent C)

### Required Packages
```bash
# Agent C SDK
npm install @agentc/realtime-core @agentc/realtime-react

# HeyGen Streaming Avatar SDK
npm install @heygen/streaming-avatar
```

### Browser Requirements
- **Chrome 88+** or **Edge 88+** (recommended)
- **Firefox 78+** or **Safari 14.1+**
- WebRTC support required
- Stable internet connection (>2 Mbps)

## Basic Setup

### 1. Initialize with Avatar Support

```typescript
import { RealtimeClient, AuthManager } from '@agentc/realtime-core';
import StreamingAvatar from '@heygen/streaming-avatar';

// Authenticate with Agent C using username/password
const authManager = new AuthManager({
  apiUrl: 'https://localhost:8000'
});

// Login with username and password
const loginResponse = await authManager.login('username', 'password');

// Create client with WebSocket URL from login response
const client = new RealtimeClient({
  apiUrl: loginResponse.websocketUrl,  // URL provided by login
  authManager,
  enableAudio: true  // Required for avatars
});

await client.connect();

// Get HeyGen token and available avatars from login response
const heygenToken = loginResponse.heygenAccessToken;
const avatars = loginResponse.availableAvatars;

console.log('Available avatars:', avatars);
```

### 2. Create Avatar Session

```typescript
// Initialize HeyGen avatar
const avatar = new StreamingAvatar({
  token: heygenToken
});

// Select an avatar
const selectedAvatar = avatars[0];

// Create avatar session
const sessionInfo = await avatar.createStartAvatar({
  avatarName: selectedAvatar.avatar_id,
  quality: 'high',
  voice: {
    voiceId: 'avatar'  // Let Agent C handle voice
  }
});

console.log('Avatar session created:', sessionInfo.session_id);
```

### 3. Handle Avatar Events

```typescript
// Set up video element
const videoElement = document.getElementById('avatar-video') as HTMLVideoElement;

// Handle stream ready
avatar.on('stream-ready', (event) => {
  console.log('Avatar stream ready');
  
  // Set video source
  videoElement.srcObject = event.stream;
  videoElement.play();
  
  // Notify Agent C about avatar session
  client.setAvatarSession(event.session_id, selectedAvatar.avatar_id);
  
  // Voice automatically switches to avatar mode
});

// Handle disconnection
avatar.on('stream-disconnected', () => {
  console.log('Avatar stream disconnected');
  videoElement.srcObject = null;
  
  // Clear avatar session
  client.clearAvatarSession();
});
```

## Complete React Implementation

```tsx
import React, { useRef, useState, useEffect } from 'react';
import { 
  AgentCProvider, 
  useRealtimeClient,
  useAvatar,
  useChat 
} from '@agentc/realtime-react';
import StreamingAvatar, { 
  StreamingEvents,
  AvatarQuality 
} from '@heygen/streaming-avatar';

function AvatarApp() {
  return (
    <AgentCProvider 
      config={{
        apiUrl: 'https://localhost:8000',
        username: process.env.REACT_APP_USERNAME,
        password: process.env.REACT_APP_PASSWORD,
        enableAudio: true
      }}
    >
      <AvatarInterface />
    </AgentCProvider>
  );
}

function AvatarInterface() {
  const videoRef = useRef<HTMLVideoElement>(null);
  const avatarRef = useRef<StreamingAvatar | null>(null);
  
  const client = useRealtimeClient();
  const { availableAvatars } = useAvatar();
  const { sendMessage, messages } = useChat();
  
  const [selectedAvatar, setSelectedAvatar] = useState<string>('');
  const [isAvatarActive, setIsAvatarActive] = useState(false);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  
  // Start avatar session
  const startAvatar = async () => {
    if (!selectedAvatar || !client) return;
    
    setIsLoading(true);
    setError(null);
    
    try {
      // Get HeyGen token
      const heygenToken = client.getHeyGenAccessToken();
      if (!heygenToken) {
        throw new Error('HeyGen token not available');
      }
      
      // Create avatar instance
      const avatar = new StreamingAvatar({
        token: heygenToken,
        debug: true
      });
      
      avatarRef.current = avatar;
      
      // Setup event handlers
      avatar.on(StreamingEvents.STREAM_READY, (event) => {
        console.log('Stream ready:', event);
        
        if (videoRef.current && event.stream) {
          videoRef.current.srcObject = event.stream;
          videoRef.current.play().catch(e => {
            console.error('Video play error:', e);
          });
        }
        
        // Notify Agent C
        client.setAvatarSession(event.session_id, selectedAvatar);
        setIsAvatarActive(true);
        setIsLoading(false);
      });
      
      avatar.on(StreamingEvents.STREAM_DISCONNECTED, () => {
        console.log('Stream disconnected');
        handleAvatarStop();
      });
      
      avatar.on(StreamingEvents.AVATAR_START_TALKING, () => {
        console.log('Avatar started talking');
      });
      
      avatar.on(StreamingEvents.AVATAR_STOP_TALKING, () => {
        console.log('Avatar stopped talking');
      });
      
      // Start avatar
      const sessionInfo = await avatar.createStartAvatar({
        avatarName: selectedAvatar,
        quality: AvatarQuality.High,
        voice: {
          voiceId: 'avatar',
          rate: 1.0,
          emotion: 'neutral'
        },
        language: 'en'
      });
      
      console.log('Avatar session started:', sessionInfo);
      
    } catch (err) {
      console.error('Failed to start avatar:', err);
      setError(err.message || 'Failed to start avatar');
      setIsLoading(false);
    }
  };
  
  // Stop avatar session
  const stopAvatar = async () => {
    if (avatarRef.current) {
      try {
        await avatarRef.current.stopAvatar();
      } catch (err) {
        console.error('Error stopping avatar:', err);
      }
    }
    handleAvatarStop();
  };
  
  const handleAvatarStop = () => {
    if (videoRef.current) {
      videoRef.current.srcObject = null;
    }
    
    if (client) {
      client.clearAvatarSession();
    }
    
    avatarRef.current = null;
    setIsAvatarActive(false);
    setIsLoading(false);
  };
  
  // Clean up on unmount
  useEffect(() => {
    return () => {
      if (avatarRef.current) {
        avatarRef.current.stopAvatar();
      }
    };
  }, []);
  
  return (
    <div className="avatar-container">
      <div className="avatar-controls">
        {!isAvatarActive ? (
          <>
            <select 
              value={selectedAvatar}
              onChange={(e) => setSelectedAvatar(e.target.value)}
              disabled={isLoading}
            >
              <option value="">Select an avatar</option>
              {availableAvatars.map(avatar => (
                <option key={avatar.avatar_id} value={avatar.avatar_id}>
                  {avatar.name}
                </option>
              ))}
            </select>
            
            <button 
              onClick={startAvatar}
              disabled={!selectedAvatar || isLoading}
            >
              {isLoading ? 'Starting...' : 'Start Avatar'}
            </button>
          </>
        ) : (
          <button onClick={stopAvatar}>
            Stop Avatar
          </button>
        )}
        
        {error && <div className="error">{error}</div>}
      </div>
      
      <div className="avatar-display">
        <video 
          ref={videoRef}
          className="avatar-video"
          autoPlay
          playsInline
          muted={false}
        />
        
        {!isAvatarActive && !isLoading && (
          <div className="avatar-placeholder">
            Avatar will appear here
          </div>
        )}
        
        {isLoading && (
          <div className="avatar-loading">
            Loading avatar...
          </div>
        )}
      </div>
      
      <div className="chat-section">
        <div className="messages">
          {messages.map((msg, i) => (
            <div key={i} className={`message ${msg.role}`}>
              {msg.content}
            </div>
          ))}
        </div>
        
        <input
          type="text"
          placeholder="Type your message..."
          onKeyPress={(e) => {
            if (e.key === 'Enter') {
              sendMessage(e.currentTarget.value);
              e.currentTarget.value = '';
            }
          }}
          disabled={!isAvatarActive}
        />
      </div>
    </div>
  );
}
```

## Avatar Configuration

### Available Avatar Options

```typescript
interface Avatar {
  avatar_id: string;       // Unique identifier
  name: string;           // Display name
  preview_url?: string;   // Preview image
  description?: string;   // Avatar description
  gender?: string;        // Avatar gender
  style?: string;         // Visual style
  personality?: string;   // Personality traits
}

// Example avatars
const avatars = [
  {
    avatar_id: 'josh_lite3_20230714',
    name: 'Josh',
    gender: 'male',
    style: 'professional',
    personality: 'friendly'
  },
  {
    avatar_id: 'anna_public_20240108',
    name: 'Anna',
    gender: 'female',
    style: 'casual',
    personality: 'energetic'
  }
];
```

### Quality Settings

```typescript
enum AvatarQuality {
  Low = 'low',       // 360p, lower bandwidth
  Medium = 'medium', // 540p, balanced
  High = 'high'      // 720p, best quality
}

// Adaptive quality based on network
function getOptimalQuality(bandwidth: number): AvatarQuality {
  if (bandwidth < 1000) return AvatarQuality.Low;
  if (bandwidth < 2000) return AvatarQuality.Medium;
  return AvatarQuality.High;
}
```

## Advanced Features

### Avatar Emotions

```typescript
// Future support for emotional expressions
interface AvatarEmotion {
  emotion: 'neutral' | 'happy' | 'sad' | 'surprised' | 'thinking';
  intensity: number; // 0-1
}

// Set avatar emotion
async function setAvatarEmotion(
  avatar: StreamingAvatar,
  emotion: AvatarEmotion
) {
  await avatar.speak({
    text: '',  // Empty text for emotion change only
    emotion: emotion.emotion,
    emotionIntensity: emotion.intensity
  });
}
```

### Custom Backgrounds

```typescript
// Virtual backgrounds (future feature)
interface BackgroundConfig {
  type: 'blur' | 'image' | 'video' | 'none';
  url?: string;
  blurIntensity?: number;
}

function setBackground(config: BackgroundConfig) {
  const avatar = avatarRef.current;
  if (!avatar) return;
  
  // Future API
  avatar.setBackground(config);
}
```

### Avatar Gestures

```typescript
// Trigger avatar gestures (future feature)
interface Gesture {
  type: 'wave' | 'nod' | 'shake' | 'point' | 'thumbs_up';
  duration?: number;
}

async function triggerGesture(gesture: Gesture) {
  const avatar = avatarRef.current;
  if (!avatar) return;
  
  // Future API
  await avatar.performGesture(gesture);
}
```

## Performance Optimization

### Network Bandwidth Management

```typescript
class BandwidthManager {
  private measurements: number[] = [];
  private measurementInterval: number = 1000;
  
  startMonitoring(avatar: StreamingAvatar) {
    setInterval(() => {
      this.measureBandwidth();
      this.adjustQuality(avatar);
    }, this.measurementInterval);
  }
  
  private measureBandwidth() {
    // Get WebRTC stats
    const pc = this.getPeerConnection();
    if (!pc) return;
    
    pc.getStats().then(stats => {
      stats.forEach(report => {
        if (report.type === 'inbound-rtp' && report.mediaType === 'video') {
          const bandwidth = report.bytesReceived * 8 / 1000; // kbps
          this.measurements.push(bandwidth);
          
          // Keep last 10 measurements
          if (this.measurements.length > 10) {
            this.measurements.shift();
          }
        }
      });
    });
  }
  
  private adjustQuality(avatar: StreamingAvatar) {
    const avgBandwidth = this.getAverageBandwidth();
    
    if (avgBandwidth < 800) {
      // Switch to low quality
      avatar.changeQuality(AvatarQuality.Low);
    } else if (avgBandwidth < 1500) {
      // Switch to medium quality
      avatar.changeQuality(AvatarQuality.Medium);
    } else {
      // Use high quality
      avatar.changeQuality(AvatarQuality.High);
    }
  }
  
  private getAverageBandwidth(): number {
    if (this.measurements.length === 0) return 0;
    const sum = this.measurements.reduce((a, b) => a + b, 0);
    return sum / this.measurements.length;
  }
  
  private getPeerConnection(): RTCPeerConnection | null {
    // Access the peer connection from HeyGen SDK
    // Implementation depends on SDK internals
    return null;
  }
}
```

### Resource Management

```typescript
class AvatarResourceManager {
  private videoElement: HTMLVideoElement | null = null;
  private avatar: StreamingAvatar | null = null;
  
  async initialize(videoElement: HTMLVideoElement) {
    this.videoElement = videoElement;
    
    // Optimize video element
    videoElement.setAttribute('playsinline', 'true');
    videoElement.setAttribute('autoplay', 'true');
    
    // Reduce memory usage
    videoElement.style.willChange = 'transform';
  }
  
  async cleanup() {
    // Stop avatar
    if (this.avatar) {
      await this.avatar.stopAvatar();
      this.avatar = null;
    }
    
    // Clean up video element
    if (this.videoElement) {
      this.videoElement.pause();
      this.videoElement.srcObject = null;
      this.videoElement.load();
    }
    
    // Force garbage collection hint
    if (window.gc) {
      window.gc();
    }
  }
  
  getMemoryUsage(): number {
    if (performance.memory) {
      return performance.memory.usedJSHeapSize / 1048576; // MB
    }
    return 0;
  }
}
```

## Error Handling

### Common Issues and Solutions

```typescript
class AvatarErrorHandler {
  handleError(error: any): string {
    console.error('Avatar error:', error);
    
    if (error.code === 'PERMISSION_DENIED') {
      return 'Camera permission denied. Please allow camera access.';
    }
    
    if (error.code === 'NETWORK_ERROR') {
      return 'Network error. Please check your connection.';
    }
    
    if (error.code === 'INVALID_TOKEN') {
      return 'Authentication failed. Please refresh and try again.';
    }
    
    if (error.code === 'AVATAR_NOT_FOUND') {
      return 'Avatar not found. Please select a different avatar.';
    }
    
    if (error.code === 'SESSION_EXPIRED') {
      return 'Session expired. Please start a new session.';
    }
    
    if (error.message?.includes('WebRTC')) {
      return 'WebRTC error. Please try a different browser.';
    }
    
    return 'An unexpected error occurred. Please try again.';
  }
  
  async retryWithBackoff(
    operation: () => Promise<any>,
    maxRetries: number = 3
  ): Promise<any> {
    let lastError: any;
    
    for (let i = 0; i < maxRetries; i++) {
      try {
        return await operation();
      } catch (error) {
        lastError = error;
        
        // Don't retry on permission errors
        if (error.code === 'PERMISSION_DENIED') {
          throw error;
        }
        
        // Exponential backoff
        const delay = Math.pow(2, i) * 1000;
        console.log(`Retry ${i + 1}/${maxRetries} after ${delay}ms`);
        await new Promise(resolve => setTimeout(resolve, delay));
      }
    }
    
    throw lastError;
  }
}
```

## Testing Avatars

### Avatar Test Suite

```typescript
class AvatarTester {
  async testAvatarConnection(
    client: RealtimeClient,
    avatarId: string
  ): Promise<boolean> {
    try {
      // Get HeyGen token
      const token = client.getHeyGenAccessToken();
      if (!token) {
        console.error('No HeyGen token');
        return false;
      }
      
      // Create test avatar
      const avatar = new StreamingAvatar({ token });
      
      // Try to start avatar
      const session = await avatar.createStartAvatar({
        avatarName: avatarId,
        quality: AvatarQuality.Low, // Use low quality for testing
        testMode: true
      });
      
      // Wait for stream ready
      await new Promise((resolve, reject) => {
        const timeout = setTimeout(() => reject('Timeout'), 10000);
        
        avatar.on(StreamingEvents.STREAM_READY, () => {
          clearTimeout(timeout);
          resolve(true);
        });
        
        avatar.on(StreamingEvents.STREAM_DISCONNECTED, () => {
          clearTimeout(timeout);
          reject('Stream disconnected');
        });
      });
      
      // Clean up
      await avatar.stopAvatar();
      
      return true;
    } catch (error) {
      console.error('Avatar test failed:', error);
      return false;
    }
  }
  
  async testAllAvatars(client: RealtimeClient): Promise<Map<string, boolean>> {
    const results = new Map<string, boolean>();
    const avatars = client.getAvailableAvatars();
    
    for (const avatar of avatars) {
      console.log(`Testing ${avatar.name}...`);
      const success = await this.testAvatarConnection(client, avatar.avatar_id);
      results.set(avatar.avatar_id, success);
    }
    
    return results;
  }
}
```

## UI Components

### Avatar Selector

```tsx
function AvatarSelector({ 
  avatars, 
  onSelect 
}: { 
  avatars: Avatar[], 
  onSelect: (avatar: Avatar) => void 
}) {
  return (
    <div className="avatar-selector">
      {avatars.map(avatar => (
        <div 
          key={avatar.avatar_id}
          className="avatar-card"
          onClick={() => onSelect(avatar)}
        >
          {avatar.preview_url && (
            <img src={avatar.preview_url} alt={avatar.name} />
          )}
          <h3>{avatar.name}</h3>
          <p>{avatar.description}</p>
          <div className="avatar-tags">
            {avatar.gender && <span>{avatar.gender}</span>}
            {avatar.style && <span>{avatar.style}</span>}
          </div>
        </div>
      ))}
    </div>
  );
}
```

### Avatar Status Indicator

```tsx
function AvatarStatus({ isActive, sessionId }: { isActive: boolean, sessionId?: string }) {
  return (
    <div className={`avatar-status ${isActive ? 'active' : 'inactive'}`}>
      <div className="status-dot" />
      <span>{isActive ? 'Avatar Active' : 'Avatar Inactive'}</span>
      {sessionId && (
        <span className="session-id">Session: {sessionId.slice(0, 8)}</span>
      )}
    </div>
  );
}
```

## Best Practices

1. **Always handle permissions gracefully**
2. **Provide fallback for unsupported browsers**
3. **Monitor network quality and adjust settings**
4. **Clean up resources properly**
5. **Test on various devices and networks**
6. **Provide clear error messages**
7. **Cache avatar sessions when possible**
8. **Use appropriate quality settings**
9. **Handle disconnections gracefully**
10. **Provide avatar preview before starting**

## Troubleshooting

### Avatar Not Appearing

```typescript
function debugAvatar() {
  console.log('=== Avatar Debug ===');
  
  // Check HeyGen token
  const token = client.getHeyGenAccessToken();
  console.log('HeyGen token:', token ? 'Present' : 'Missing');
  
  // Check available avatars
  const avatars = client.getAvailableAvatars();
  console.log('Available avatars:', avatars.length);
  
  // Check browser support
  console.log('WebRTC supported:', !!RTCPeerConnection);
  console.log('MediaStream supported:', !!navigator.mediaDevices);
  
  // Check network
  navigator.connection && console.log('Network type:', navigator.connection.effectiveType);
  
  // Check video element
  const video = document.querySelector('video');
  console.log('Video element:', video);
  console.log('Video srcObject:', video?.srcObject);
  console.log('Video readyState:', video?.readyState);
}
```

## Summary

Avatar integration with Agent C provides:

1. **Real-time video avatars** for enhanced user experience
2. **Synchronized responses** with Agent C
3. **Multiple avatar options** for different use cases
4. **Automatic voice coordination** between systems
5. **Quality adaptation** based on network conditions

Follow this guide to create engaging avatar-powered applications with the Agent C Realtime SDK.