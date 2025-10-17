# useAvatar

React hook for managing HeyGen avatar integration in the Agent C Realtime Client SDK. Provides a comprehensive interface for avatar session management, voice synchronization, and state tracking.

## Overview

The `useAvatar` hook is a feature-rich React hook that manages HeyGen avatar sessions within the Agent C ecosystem. It handles avatar selection, session lifecycle, voice synchronization, and provides real-time state updates through the underlying `AvatarManager`.

### Key Features
- Avatar session management with HeyGen integration
- Automatic voice synchronization when avatars are activated
- Real-time state updates through event subscriptions
- Error handling and loading states
- StrictMode compatibility with proper cleanup
- Type-safe TypeScript interface

## Import

```typescript
import { useAvatar } from '@agentc/realtime-react';
// or
import { useAvatar } from '@agentc/realtime-react/hooks';
```

## TypeScript Types

### Core Types

```typescript
// Avatar data structure from @agentc/realtime-core
interface Avatar {
  avatar_id: string;
  created_at: number;
  default_voice: string;
  is_public: boolean;
  normal_preview: string;
  pose_name: string;
  status: string;
}

// Voice model configuration
interface Voice {
  voice_id: string;
  vendor: string;
  description: string;
  output_format: string;
}
```

### Hook Return Type

```typescript
interface UseAvatarReturn {
  /** List of available avatars */
  availableAvatars: Avatar[];
  
  /** Current avatar session info */
  avatarSession: { 
    sessionId: string | null; 
    avatarId: string | null;
  } | null;
  
  /** Whether an avatar is currently active */
  isAvatarActive: boolean;
  
  /** HeyGen access token for client-side SDK */
  heygenAccessToken: string | null;
  
  /** Set the active avatar */
  setAvatar: (avatarId: string, sessionId: string) => Promise<void>;
  
  /** Clear the current avatar session */
  clearAvatar: () => Promise<void>;
  
  /** Get avatar by ID */
  getAvatarById: (avatarId: string) => Avatar | undefined;
  
  /** Check if an avatar is available */
  isAvatarAvailable: (avatarId: string) => boolean;
  
  /** Whether avatar operations are loading */
  isLoading: boolean;
  
  /** Error state */
  error: string | null;
  
  /** Whether avatar feature is enabled */
  isAvatarEnabled: boolean;
}
```

## Return Values

### availableAvatars
- **Type:** `Avatar[]`
- **Description:** Array of available avatars from the server. Updates automatically when avatar catalog changes.
- **Initial Value:** Empty array

### avatarSession
- **Type:** `{ sessionId: string | null; avatarId: string | null } | null`
- **Description:** Current active avatar session information. Null when no avatar is active.
- **Initial Value:** `null`

### isAvatarActive
- **Type:** `boolean`
- **Description:** Computed property indicating if an avatar session is currently active.
- **Initial Value:** `false`

### heygenAccessToken
- **Type:** `string | null`
- **Description:** HeyGen SDK access token for client-side integration. Used by the HeyGen streaming SDK.
- **Initial Value:** `null`

### setAvatar
- **Type:** `(avatarId: string, sessionId: string) => Promise<void>`
- **Description:** Activates an avatar session. Automatically switches voice to 'avatar' mode.
- **Throws:** Error if client unavailable, avatar not found, or activation fails

### clearAvatar
- **Type:** `() => Promise<void>`
- **Description:** Deactivates the current avatar session. Restores voice to previous non-avatar setting.
- **Throws:** Error if client unavailable or deactivation fails

### getAvatarById
- **Type:** `(avatarId: string) => Avatar | undefined`
- **Description:** Helper function to find an avatar by its ID from available avatars.

### isAvatarAvailable
- **Type:** `(avatarId: string) => boolean`
- **Description:** Checks if an avatar ID exists in the available avatars list.

### isLoading
- **Type:** `boolean`
- **Description:** Indicates if an avatar operation (setAvatar/clearAvatar) is in progress.

### error
- **Type:** `string | null`
- **Description:** Error message from the last failed operation. Cleared on successful operations.

### isAvatarEnabled
- **Type:** `boolean`
- **Description:** Indicates if the avatar feature is available (avatars exist in catalog).

## Usage Examples

### Basic Avatar Selection

```tsx
import { useAvatar } from '@agentc/realtime-react';

function AvatarSelector() {
  const {
    availableAvatars,
    avatarSession,
    setAvatar,
    clearAvatar,
    isLoading,
    error
  } = useAvatar();

  const handleAvatarSelect = async (avatarId: string) => {
    try {
      // Generate a session ID (typically from HeyGen SDK)
      const sessionId = `session-${Date.now()}`;
      await setAvatar(avatarId, sessionId);
      console.log('Avatar activated:', avatarId);
    } catch (err) {
      console.error('Failed to activate avatar:', err);
    }
  };

  return (
    <div>
      {error && <div className="error">{error}</div>}
      
      <div className="avatar-grid">
        {availableAvatars.map(avatar => (
          <button
            key={avatar.avatar_id}
            onClick={() => handleAvatarSelect(avatar.avatar_id)}
            disabled={isLoading || avatarSession?.avatarId === avatar.avatar_id}
          >
            <img src={avatar.normal_preview} alt={avatar.pose_name} />
            <span>{avatar.avatar_id}</span>
          </button>
        ))}
      </div>
      
      {avatarSession && (
        <button onClick={clearAvatar} disabled={isLoading}>
          Deactivate Avatar
        </button>
      )}
    </div>
  );
}
```

### HeyGen SDK Integration

```tsx
import { useAvatar } from '@agentc/realtime-react';
import { useEffect, useRef } from 'react';
import StreamingAvatar from '@heygen/streaming-avatar';

function HeyGenAvatarView() {
  const {
    avatarSession,
    heygenAccessToken,
    isAvatarActive,
    setAvatar,
    clearAvatar
  } = useAvatar();
  
  const avatarRef = useRef<StreamingAvatar | null>(null);
  const containerRef = useRef<HTMLDivElement>(null);

  // Initialize HeyGen SDK when avatar becomes active
  useEffect(() => {
    if (!isAvatarActive || !heygenAccessToken || !containerRef.current) {
      return;
    }

    async function initializeHeyGen() {
      try {
        // Create HeyGen streaming avatar instance
        const avatar = new StreamingAvatar({
          token: heygenAccessToken!,
          container: containerRef.current!,
        });

        // Start the avatar session
        await avatar.startSession({
          sessionId: avatarSession!.sessionId!,
          avatarId: avatarSession!.avatarId!,
        });

        avatarRef.current = avatar;

        // Listen for HeyGen events
        avatar.on('stream_ready', () => {
          console.log('HeyGen stream ready');
        });

        avatar.on('stream_disconnected', () => {
          console.log('HeyGen stream disconnected');
          clearAvatar();
        });

      } catch (error) {
        console.error('Failed to initialize HeyGen:', error);
        clearAvatar();
      }
    }

    initializeHeyGen();

    // Cleanup on unmount or session change
    return () => {
      if (avatarRef.current) {
        avatarRef.current.stopSession();
        avatarRef.current = null;
      }
    };
  }, [isAvatarActive, heygenAccessToken, avatarSession, clearAvatar]);

  return (
    <div className="avatar-container">
      {isAvatarActive ? (
        <div ref={containerRef} className="heygen-video" />
      ) : (
        <div className="avatar-placeholder">
          No avatar active
        </div>
      )}
    </div>
  );
}
```

### Advanced Avatar Management

```tsx
import { useAvatar, useVoiceModel } from '@agentc/realtime-react';
import { useState, useEffect } from 'react';

function AdvancedAvatarControl() {
  const {
    availableAvatars,
    avatarSession,
    isAvatarEnabled,
    setAvatar,
    clearAvatar,
    getAvatarById,
    isAvatarAvailable,
    isLoading,
    error
  } = useAvatar();
  
  const { currentVoice, setVoice } = useVoiceModel();
  const [selectedAvatarId, setSelectedAvatarId] = useState<string>('');

  // Check if avatar feature is available
  if (!isAvatarEnabled) {
    return <div>Avatar feature is not available</div>;
  }

  const handleAvatarActivation = async () => {
    if (!selectedAvatarId) return;

    // Validate avatar exists
    if (!isAvatarAvailable(selectedAvatarId)) {
      console.error('Selected avatar not available');
      return;
    }

    const avatar = getAvatarById(selectedAvatarId);
    if (!avatar) return;

    try {
      // Create session with avatar's default voice configuration
      const sessionId = await createHeyGenSession({
        avatarId: avatar.avatar_id,
        voice: avatar.default_voice
      });

      // Activate avatar (automatically switches to avatar voice)
      await setAvatar(avatar.avatar_id, sessionId);

      console.log('Avatar activated with session:', sessionId);
    } catch (err) {
      console.error('Avatar activation failed:', err);
    }
  };

  const handleAvatarDeactivation = async () => {
    try {
      await clearAvatar();
      // Voice will automatically switch back to non-avatar voice
      console.log('Avatar deactivated, voice restored');
    } catch (err) {
      console.error('Avatar deactivation failed:', err);
    }
  };

  // Monitor voice changes when avatar state changes
  useEffect(() => {
    if (avatarSession) {
      console.log('Avatar active, voice switched to:', currentVoice);
    } else {
      console.log('Avatar inactive, voice is:', currentVoice);
    }
  }, [avatarSession, currentVoice]);

  return (
    <div className="advanced-avatar-control">
      <select
        value={selectedAvatarId}
        onChange={(e) => setSelectedAvatarId(e.target.value)}
        disabled={isLoading || avatarSession !== null}
      >
        <option value="">Select an avatar</option>
        {availableAvatars.map(avatar => (
          <option key={avatar.avatar_id} value={avatar.avatar_id}>
            {avatar.pose_name || avatar.avatar_id}
          </option>
        ))}
      </select>

      {!avatarSession ? (
        <button 
          onClick={handleAvatarActivation}
          disabled={isLoading || !selectedAvatarId}
        >
          Activate Avatar
        </button>
      ) : (
        <div>
          <p>Active: {avatarSession.avatarId}</p>
          <p>Session: {avatarSession.sessionId}</p>
          <button onClick={handleAvatarDeactivation} disabled={isLoading}>
            Deactivate
          </button>
        </div>
      )}

      {error && <div className="error">{error}</div>}
    </div>
  );
}

// Helper function to create HeyGen session
async function createHeyGenSession(config: {
  avatarId: string;
  voice: string;
}): Promise<string> {
  // This would typically call your backend API to create a HeyGen session
  // and return the session ID
  const response = await fetch('/api/heygen/sessions', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(config)
  });
  
  const data = await response.json();
  return data.sessionId;
}
```

## Event Handling Patterns

The hook automatically subscribes to AvatarManager events for real-time updates:

```typescript
// Events monitored by the hook
- 'avatar-state-changed'    // Avatar catalog or state changes
- 'avatar-session-started'   // New avatar session activated
- 'avatar-session-ended'     // Avatar session deactivated
```

These events trigger automatic updates to the hook's state, ensuring UI components always reflect the current avatar status.

## StrictMode Compatibility

The `useAvatar` hook is fully compatible with React StrictMode:

1. **Proper cleanup:** Event listeners are removed in the cleanup function
2. **Idempotent operations:** Multiple mount/unmount cycles don't cause issues
3. **State consistency:** Avatar state remains consistent across re-renders
4. **Timer management:** Auto-removal timers are properly cleaned up

```tsx
// Safe to use in StrictMode
import { StrictMode } from 'react';
import { AgentCProvider } from '@agentc/realtime-react';

function App() {
  return (
    <StrictMode>
      <AgentCProvider>
        <AvatarComponent />
      </AgentCProvider>
    </StrictMode>
  );
}
```

## Best Practices and Common Patterns

### 1. Error Boundary Integration

```tsx
function AvatarWithErrorBoundary() {
  const { setAvatar, error } = useAvatar();
  
  // Display errors gracefully
  if (error) {
    return <ErrorFallback message={error} />;
  }
  
  // Normal avatar UI
  return <AvatarSelector />;
}
```

### 2. Loading States

```tsx
function AvatarLoader() {
  const { isLoading, availableAvatars } = useAvatar();
  
  if (isLoading) {
    return <Spinner message="Processing avatar request..." />;
  }
  
  if (availableAvatars.length === 0) {
    return <EmptyState message="No avatars available" />;
  }
  
  return <AvatarGrid avatars={availableAvatars} />;
}
```

### 3. Session Persistence

```tsx
function PersistentAvatarSession() {
  const { avatarSession, setAvatar } = useAvatar();
  
  // Save session to localStorage
  useEffect(() => {
    if (avatarSession) {
      localStorage.setItem('avatar-session', JSON.stringify(avatarSession));
    } else {
      localStorage.removeItem('avatar-session');
    }
  }, [avatarSession]);
  
  // Restore session on mount
  useEffect(() => {
    const saved = localStorage.getItem('avatar-session');
    if (saved && !avatarSession) {
      const { avatarId, sessionId } = JSON.parse(saved);
      setAvatar(avatarId, sessionId).catch(console.error);
    }
  }, []); // eslint-disable-line react-hooks/exhaustive-deps
  
  return <AvatarUI />;
}
```

### 4. Voice Synchronization

```tsx
function AvatarWithVoiceSync() {
  const { avatarSession, clearAvatar } = useAvatar();
  const { currentVoice, setVoice, availableVoices } = useVoiceModel();
  
  // Monitor voice changes
  useEffect(() => {
    if (avatarSession && currentVoice !== 'avatar') {
      console.warn('Avatar active but voice not set to avatar mode');
    }
  }, [avatarSession, currentVoice]);
  
  // Handle avatar deactivation with voice restoration
  const handleDeactivate = async () => {
    await clearAvatar();
    // Voice automatically restored to non-avatar voice
    const defaultVoice = availableVoices.find(v => 
      v.voice_id !== 'avatar' && v.voice_id !== 'none'
    );
    if (defaultVoice) {
      await setVoice(defaultVoice.voice_id);
    }
  };
  
  return (
    <button onClick={handleDeactivate}>
      Deactivate Avatar & Restore Voice
    </button>
  );
}
```

### 5. Avatar Preview

```tsx
function AvatarPreview() {
  const { availableAvatars, getAvatarById } = useAvatar();
  const [previewId, setPreviewId] = useState<string>('');
  
  const previewAvatar = previewId ? getAvatarById(previewId) : null;
  
  return (
    <div className="avatar-preview">
      <div className="avatar-list">
        {availableAvatars.map(avatar => (
          <button
            key={avatar.avatar_id}
            onMouseEnter={() => setPreviewId(avatar.avatar_id)}
            onMouseLeave={() => setPreviewId('')}
          >
            {avatar.avatar_id}
          </button>
        ))}
      </div>
      
      {previewAvatar && (
        <div className="preview-panel">
          <img src={previewAvatar.normal_preview} alt="Preview" />
          <dl>
            <dt>ID:</dt>
            <dd>{previewAvatar.avatar_id}</dd>
            <dt>Pose:</dt>
            <dd>{previewAvatar.pose_name}</dd>
            <dt>Default Voice:</dt>
            <dd>{previewAvatar.default_voice}</dd>
            <dt>Status:</dt>
            <dd>{previewAvatar.status}</dd>
          </dl>
        </div>
      )}
    </div>
  );
}
```

## Integration Notes

### HeyGen SDK Requirements

When using avatars, you'll need to integrate with the HeyGen Streaming Avatar SDK:

1. **Install HeyGen SDK:** `npm install @heygen/streaming-avatar`
2. **Token Management:** Use `heygenAccessToken` from the hook
3. **Session Lifecycle:** Coordinate avatar sessions with HeyGen SDK
4. **Error Handling:** Handle both Agent C and HeyGen errors

### Voice Model Interaction

The avatar system automatically manages voice synchronization:

- When avatar activated → Voice switches to 'avatar' mode
- When avatar deactivated → Voice restores to previous non-avatar setting
- Voice changes are handled through VoiceManager integration

### Performance Considerations

1. **Avatar Preview Images:** Cache preview images for better performance
2. **Session Creation:** Create HeyGen sessions asynchronously
3. **Event Subscriptions:** Automatically cleaned up to prevent memory leaks
4. **State Updates:** Batched for optimal React rendering

## Troubleshooting

### Common Issues

1. **Avatar not activating:**
   - Check if `isAvatarEnabled` is true
   - Verify avatar ID exists in `availableAvatars`
   - Ensure valid session ID is provided

2. **Voice not switching:**
   - Voice automatically switches to 'avatar' mode
   - Check VoiceManager for available voices
   - Verify 'avatar' voice exists in voice catalog

3. **HeyGen integration issues:**
   - Verify `heygenAccessToken` is valid
   - Check HeyGen SDK initialization
   - Monitor browser console for HeyGen errors

4. **Memory leaks:**
   - Hook automatically handles cleanup
   - Ensure HeyGen SDK sessions are properly terminated
   - Check for proper component unmounting

## Related Hooks

- [`useVoiceModel`](./useVoiceModel.md) - Voice selection and management
- [`useConnection`](./useConnection.md) - WebSocket connection state
- [`useChat`](./useChat.md) - Chat message management
- [`useRealtimeClient`](./useRealtimeClient.md) - Direct client access