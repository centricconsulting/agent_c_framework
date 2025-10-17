# AvatarManager API Reference

The `AvatarManager` class handles HeyGen avatar integration, managing avatar sessions and coordinating with the Agent C backend.

## Import

```typescript
import { AvatarManager } from '@agentc/realtime-core';
```

## Overview

The AvatarManager tracks available avatars, manages the current avatar session, and coordinates avatar state between HeyGen and Agent C.

## Constructor

```typescript
constructor(config?: AvatarManagerConfig)
```

Creates a new AvatarManager instance.

### Parameters

- `config` (AvatarManagerConfig, optional) - Configuration object

```typescript
interface AvatarManagerConfig {
  availableAvatars?: Avatar[];  // Initial list of avatars
  debug?: boolean;               // Enable debug logging (default: false)
}

interface Avatar {
  avatar_id: string;            // Unique avatar identifier
  name: string;                 // Display name
  preview_url?: string;         // Preview image URL
  description?: string;         // Avatar description
  gender?: string;              // Avatar gender
  style?: string;               // Visual style
}
```

**Note:** AvatarManager is typically created automatically by RealtimeClient

### Example

```typescript
// Setup authentication first
const authManager = new AuthManager({
  apiUrl: 'https://localhost:8000'
});

await authManager.login('username', 'password');

// AvatarManager is created automatically
const client = new RealtimeClient({
  apiUrl: 'wss://localhost:8000/rt/ws',
  authManager
});

// Access the AvatarManager instance
const avatarManager = client.getAvatarManager();
```

## Properties

### isSessionActive

Checks if an avatar session is currently active.

```typescript
get isSessionActive(): boolean
```

**Returns:** `true` if avatar session is active

**Example:**
```typescript
if (avatarManager.isSessionActive) {
  console.log('Avatar is active');
  // Hide audio visualizer, show video
}
```

### currentAvatar

Gets the currently active avatar.

```typescript
get currentAvatar(): Avatar | null
```

**Returns:** Current avatar object or null

**Example:**
```typescript
const avatar = avatarManager.currentAvatar;
if (avatar) {
  console.log(`Active avatar: ${avatar.name}`);
  console.log(`Avatar ID: ${avatar.avatar_id}`);
}
```

### sessionId

Gets the current HeyGen session ID.

```typescript
get sessionId(): string | null
```

**Returns:** HeyGen session ID or null

**Example:**
```typescript
const sessionId = avatarManager.sessionId;
if (sessionId) {
  console.log(`HeyGen session: ${sessionId}`);
}
```

## Avatar Management Methods

### setAvailableAvatars()

Sets the list of available avatars.

```typescript
setAvailableAvatars(avatars: Avatar[]): void
```

**Parameters:**
- `avatars` (Avatar[]) - Array of available avatars

**Note:** Usually called automatically from AuthManager

**Example:**
```typescript
const avatars = [
  {
    avatar_id: 'avatar_001',
    name: 'Alex',
    preview_url: 'https://example.com/alex.jpg',
    description: 'Professional business avatar',
    gender: 'male',
    style: 'formal'
  },
  {
    avatar_id: 'avatar_002',
    name: 'Sarah',
    preview_url: 'https://example.com/sarah.jpg',
    description: 'Friendly support avatar',
    gender: 'female',
    style: 'casual'
  }
];

avatarManager.setAvailableAvatars(avatars);
```

### getAvailableAvatars()

Gets all available avatars.

```typescript
getAvailableAvatars(): Avatar[]
```

**Returns:** Array of available avatars

**Example:**
```typescript
const avatars = avatarManager.getAvailableAvatars();
console.log(`${avatars.length} avatars available:`);
avatars.forEach(avatar => {
  console.log(`  - ${avatar.name}: ${avatar.description}`);
});
```

### getAvatarById()

Gets a specific avatar by ID.

```typescript
getAvatarById(avatarId: string): Avatar | undefined
```

**Parameters:**
- `avatarId` (string) - Avatar ID to find

**Returns:** Avatar object or undefined

**Example:**
```typescript
const avatar = avatarManager.getAvatarById('avatar_001');
if (avatar) {
  console.log(`Found avatar: ${avatar.name}`);
  console.log(`Preview: ${avatar.preview_url}`);
}
```

### setAvatarSession()

Sets the active avatar session.

```typescript
setAvatarSession(sessionId: string, avatarId: string): void
```

**Parameters:**
- `sessionId` (string) - HeyGen session ID
- `avatarId` (string) - Avatar ID being used

**Note:** Called after HeyGen STREAM_READY event

**Example:**
```typescript
// After creating HeyGen session and receiving STREAM_READY
avatarManager.setAvatarSession('heygen-session-123', 'avatar_001');

// This also notifies Agent C
client.setAvatarSession('heygen-session-123', 'avatar_001');
```

### clearAvatarSession()

Clears the current avatar session.

```typescript
clearAvatarSession(): void
```

**Example:**
```typescript
// When HeyGen session ends
avatarManager.clearAvatarSession();

// Also notify Agent C
client.clearAvatarSession();
```

### getSessionInfo()

Gets current session information.

```typescript
getSessionInfo(): AvatarSessionInfo | null
```

**Returns:** Session info or null

```typescript
interface AvatarSessionInfo {
  sessionId: string;
  avatarId: string;
  avatar: Avatar | null;
  startTime: number;
  duration: number;
}
```

**Example:**
```typescript
const info = avatarManager.getSessionInfo();
if (info) {
  console.log(`Session: ${info.sessionId}`);
  console.log(`Avatar: ${info.avatar?.name}`);
  console.log(`Duration: ${info.duration}ms`);
}
```

## Avatar Selection Methods

### selectAvatar()

Selects an avatar for use (doesn't start session).

```typescript
selectAvatar(avatarId: string): Avatar | null
```

**Parameters:**
- `avatarId` (string) - Avatar ID to select

**Returns:** Selected avatar or null if not found

**Example:**
```typescript
const avatar = avatarManager.selectAvatar('avatar_001');
if (avatar) {
  console.log(`Selected: ${avatar.name}`);
  // Now create HeyGen session with this avatar
}
```

### getAvatarsByStyle()

Gets avatars filtered by style.

```typescript
getAvatarsByStyle(style: string): Avatar[]
```

**Parameters:**
- `style` (string) - Style to filter by (e.g., 'formal', 'casual')

**Returns:** Array of matching avatars

**Example:**
```typescript
const formalAvatars = avatarManager.getAvatarsByStyle('formal');
console.log(`${formalAvatars.length} formal avatars available`);
```

### getAvatarsByGender()

Gets avatars filtered by gender.

```typescript
getAvatarsByGender(gender: string): Avatar[]
```

**Parameters:**
- `gender` (string) - Gender to filter by

**Returns:** Array of matching avatars

**Example:**
```typescript
const femaleAvatars = avatarManager.getAvatarsByGender('female');
femaleAvatars.forEach(avatar => {
  console.log(`${avatar.name}: ${avatar.description}`);
});
```

## Session Statistics

### getSessionStats()

Gets statistics for the current or past sessions.

```typescript
getSessionStats(): AvatarSessionStats
```

**Returns:** Session statistics

```typescript
interface AvatarSessionStats {
  totalSessions: number;
  totalDuration: number;
  averageSessionDuration: number;
  currentSessionDuration: number;
  mostUsedAvatar: string | null;
  avatarUsageCount: Map<string, number>;
}
```

**Example:**
```typescript
const stats = avatarManager.getSessionStats();
console.log(`Total sessions: ${stats.totalSessions}`);
console.log(`Total duration: ${stats.totalDuration}ms`);
console.log(`Average session: ${stats.averageSessionDuration}ms`);
console.log(`Most used avatar: ${stats.mostUsedAvatar}`);
```

## Event Handling

AvatarManager extends EventEmitter:

### Events

- `avatar:session-started` - Avatar session started
- `avatar:session-ended` - Avatar session ended
- `avatar:selected` - Avatar selected
- `avatar:list-updated` - Available avatars updated

### Example Event Handling

```typescript
avatarManager.on('avatar:session-started', (event: AvatarSessionEvent) => {
  console.log(`Avatar session started: ${event.sessionId}`);
  console.log(`Using avatar: ${event.avatar.name}`);
  
  // Switch UI to avatar mode
  showAvatarVideo();
  hideAudioVisualizer();
});

avatarManager.on('avatar:session-ended', (event: AvatarSessionEndEvent) => {
  console.log(`Avatar session ended: ${event.sessionId}`);
  console.log(`Duration: ${event.duration}ms`);
  
  // Switch UI back to audio mode
  hideAvatarVideo();
  showAudioVisualizer();
});

avatarManager.on('avatar:selected', (avatar: Avatar) => {
  console.log(`Avatar selected: ${avatar.name}`);
  updateAvatarPreview(avatar);
});

avatarManager.on('avatar:list-updated', (avatars: Avatar[]) => {
  console.log(`Avatar list updated: ${avatars.length} available`);
  updateAvatarSelector(avatars);
});
```

### Event Types

```typescript
interface AvatarSessionEvent {
  sessionId: string;
  avatarId: string;
  avatar: Avatar;
  timestamp: number;
}

interface AvatarSessionEndEvent {
  sessionId: string;
  avatarId: string;
  duration: number;
  timestamp: number;
}
```

## HeyGen Integration

The AvatarManager coordinates with HeyGen streaming avatars:

```typescript
import NewStreamingAvatar from '@heygen/streaming-avatar';

async function setupAvatar() {
  // Get HeyGen token from auth
  const heygenToken = client.getHeyGenAccessToken();
  if (!heygenToken) {
    console.error('No HeyGen token available');
    return;
  }
  
  // Create HeyGen avatar instance
  const heygenAvatar = new NewStreamingAvatar({
    token: heygenToken
  });
  
  // Select avatar from manager
  const avatarManager = client.getAvatarManager();
  const avatar = avatarManager.selectAvatar('avatar_001');
  
  if (!avatar) {
    console.error('Avatar not found');
    return;
  }
  
  // Create HeyGen session
  await heygenAvatar.createStartAvatar({
    avatarName: avatar.avatar_id,
    quality: 'high',
    voice: {
      voiceId: 'avatar'  // Let Agent C handle voice
    }
  });
  
  // Listen for HeyGen events
  heygenAvatar.on('stream-ready', (event) => {
    console.log('HeyGen stream ready:', event.session_id);
    
    // Notify Agent C and avatar manager
    avatarManager.setAvatarSession(event.session_id, avatar.avatar_id);
    client.setAvatarSession(event.session_id, avatar.avatar_id);
    
    // Voice will automatically switch to avatar mode
  });
  
  heygenAvatar.on('stream-disconnected', () => {
    console.log('HeyGen stream disconnected');
    
    // Clear avatar session
    avatarManager.clearAvatarSession();
    client.clearAvatarSession();
  });
}
```

## Complete Example

```typescript
import { RealtimeClient, AuthManager } from '@agentc/realtime-core';
import NewStreamingAvatar from '@heygen/streaming-avatar';

async function avatarExample() {
  // Setup authentication
  const authManager = new AuthManager({
    apiUrl: 'https://localhost:8000'
  });
  
  await authManager.login('username', 'password');
  
  // Setup client with auth
  const client = new RealtimeClient({
    apiUrl: 'wss://localhost:8000/rt/ws',
    authManager,
    enableAudio: true
  });
  
  // Get avatar manager
  const avatarManager = client.getAvatarManager();
  
  if (!avatarManager) {
    console.error('Avatar manager not initialized');
    return;
  }
  
  // Display available avatars
  console.log('🎭 Available Avatars:');
  const avatars = avatarManager.getAvailableAvatars();
  avatars.forEach(avatar => {
    console.log(`  ${avatar.name} (${avatar.avatar_id})`);
    console.log(`    Style: ${avatar.style}`);
    console.log(`    Gender: ${avatar.gender}`);
    console.log(`    Description: ${avatar.description}`);
  });
  
  // Filter avatars
  const casualAvatars = avatarManager.getAvatarsByStyle('casual');
  console.log(`\n👔 Casual avatars: ${casualAvatars.length}`);
  
  // Subscribe to avatar events
  avatarManager.on('avatar:session-started', (event) => {
    console.log(`\n🎬 Avatar session started`);
    console.log(`  Session: ${event.sessionId}`);
    console.log(`  Avatar: ${event.avatar.name}`);
    
    // Update UI
    document.getElementById('avatar-status')!.textContent = 'Avatar Active';
    document.getElementById('avatar-name')!.textContent = event.avatar.name;
  });
  
  avatarManager.on('avatar:session-ended', (event) => {
    console.log(`\n🛑 Avatar session ended`);
    console.log(`  Duration: ${(event.duration / 1000).toFixed(1)} seconds`);
    
    // Update UI
    document.getElementById('avatar-status')!.textContent = 'Avatar Inactive';
  });
  
  // Connect to Agent C
  await client.connect();
  
  // Function to start avatar session
  async function startAvatarSession(avatarId: string) {
    // Check if avatar exists
    const avatar = avatarManager.getAvatarById(avatarId);
    if (!avatar) {
      console.error(`Avatar ${avatarId} not found`);
      return;
    }
    
    console.log(`\n🚀 Starting avatar session with ${avatar.name}`);
    
    // Get HeyGen token
    const heygenToken = client.getHeyGenAccessToken();
    if (!heygenToken) {
      console.error('No HeyGen token available');
      return;
    }
    
    // Create HeyGen instance
    const heygenAvatar = new NewStreamingAvatar({
      token: heygenToken
    });
    
    // Get video element
    const videoElement = document.getElementById('avatar-video') as HTMLVideoElement;
    
    try {
      // Create avatar session
      await heygenAvatar.createStartAvatar({
        avatarName: avatarId,
        quality: 'high',
        voice: {
          voiceId: 'avatar'  // Agent C handles voice
        }
      });
      
      // Set up event handlers
      heygenAvatar.on('stream-ready', (event) => {
        console.log('✅ HeyGen stream ready');
        
        // Set video source
        videoElement.srcObject = event.stream;
        videoElement.play();
        
        // Update avatar manager and notify Agent C
        avatarManager.setAvatarSession(event.session_id, avatarId);
        client.setAvatarSession(event.session_id, avatarId);
        
        // Voice automatically switches to avatar mode
        const voiceManager = client.getVoiceManager();
        if (voiceManager?.currentVoiceId !== 'avatar') {
          console.log('Voice switched to avatar mode');
        }
      });
      
      heygenAvatar.on('stream-disconnected', () => {
        console.log('❌ HeyGen stream disconnected');
        
        // Clear video
        videoElement.srcObject = null;
        
        // Clear avatar session
        avatarManager.clearAvatarSession();
        client.clearAvatarSession();
        
        // Voice will switch back to previous mode
      });
      
      return heygenAvatar;
      
    } catch (error) {
      console.error('Failed to start avatar:', error);
      return null;
    }
  }
  
  // Start an avatar session
  const heygenInstance = await startAvatarSession('avatar_001');
  
  if (heygenInstance) {
    // Send a message with avatar active
    client.sendText('Hello! Can you see me through the avatar?');
    
    // Monitor session
    setInterval(() => {
      if (avatarManager.isSessionActive) {
        const info = avatarManager.getSessionInfo();
        if (info) {
          const duration = (info.duration / 1000).toFixed(1);
          console.log(`Avatar session active: ${duration}s`);
        }
      }
    }, 5000);
    
    // Stop avatar after 30 seconds (demo)
    setTimeout(async () => {
      console.log('\n🛑 Stopping avatar session...');
      
      if (heygenInstance) {
        await heygenInstance.stopAvatar();
      }
      
      // Get session statistics
      const stats = avatarManager.getSessionStats();
      console.log('\n📊 Avatar Statistics:');
      console.log(`Total sessions: ${stats.totalSessions}`);
      console.log(`Total duration: ${(stats.totalDuration / 1000).toFixed(1)}s`);
      console.log(`Average session: ${(stats.averageSessionDuration / 1000).toFixed(1)}s`);
      
      if (stats.mostUsedAvatar) {
        const mostUsed = avatarManager.getAvatarById(stats.mostUsedAvatar);
        console.log(`Most used avatar: ${mostUsed?.name}`);
      }
      
      // Show usage count
      console.log('\nAvatar usage:');
      stats.avatarUsageCount.forEach((count, avatarId) => {
        const avatar = avatarManager.getAvatarById(avatarId);
        console.log(`  ${avatar?.name}: ${count} times`);
      });
      
    }, 30000);
  }
  
  // Create avatar selector UI
  function createAvatarSelector() {
    const select = document.createElement('select');
    select.id = 'avatar-selector';
    
    // Add default option
    const defaultOption = document.createElement('option');
    defaultOption.value = '';
    defaultOption.textContent = 'Select an avatar';
    select.appendChild(defaultOption);
    
    // Add avatar options
    avatars.forEach(avatar => {
      const option = document.createElement('option');
      option.value = avatar.avatar_id;
      option.textContent = `${avatar.name} (${avatar.style})`;
      select.appendChild(option);
    });
    
    // Handle selection
    select.addEventListener('change', async (e) => {
      const avatarId = (e.target as HTMLSelectElement).value;
      if (avatarId) {
        // Stop current session if active
        if (avatarManager.isSessionActive) {
          avatarManager.clearAvatarSession();
        }
        
        // Start new session
        await startAvatarSession(avatarId);
      }
    });
    
    return select;
  }
  
  // Add selector to page
  const selector = createAvatarSelector();
  document.body.appendChild(selector);
}

avatarExample().catch(console.error);
```

## Best Practices

1. **Check avatar availability before use:**
```typescript
const avatarId = 'avatar_001';
if (avatarManager.getAvatarById(avatarId)) {
  // Avatar exists, safe to use
  await startAvatarSession(avatarId);
} else {
  console.error(`Avatar ${avatarId} not available`);
}
```

2. **Handle session lifecycle properly:**
```typescript
// Start session
avatarManager.setAvatarSession(sessionId, avatarId);
client.setAvatarSession(sessionId, avatarId);

// End session
avatarManager.clearAvatarSession();
client.clearAvatarSession();
```

3. **Monitor session state:**
```typescript
if (avatarManager.isSessionActive) {
  // Avatar is active, hide audio UI
  audioVisualizer.style.display = 'none';
} else {
  // No avatar, show audio UI
  audioVisualizer.style.display = 'block';
}
```

4. **Provide avatar preview:**
```typescript
function showAvatarPreview(avatarId: string) {
  const avatar = avatarManager.getAvatarById(avatarId);
  if (avatar?.preview_url) {
    previewImage.src = avatar.preview_url;
    previewName.textContent = avatar.name;
    previewDesc.textContent = avatar.description || '';
  }
}
```

5. **Track avatar usage:**
```typescript
// Log avatar usage for analytics
avatarManager.on('avatar:session-ended', (event) => {
  analytics.track('avatar_session', {
    avatar_id: event.avatarId,
    duration: event.duration,
    timestamp: event.timestamp
  });
});
```

## TypeScript Types

```typescript
import {
  AvatarManager,
  AvatarManagerConfig,
  Avatar,
  AvatarSessionInfo,
  AvatarSessionStats,
  AvatarSessionEvent,
  AvatarSessionEndEvent
} from '@agentc/realtime-core';
```

All methods and properties are fully typed for TypeScript applications.