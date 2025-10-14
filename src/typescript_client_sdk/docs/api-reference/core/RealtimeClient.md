# RealtimeClient API Reference

The `RealtimeClient` class is the main entry point for the Agent C Realtime SDK. It manages WebSocket connections, handles events, coordinates subsystems, and provides methods for interacting with the Agent C platform.

## Import

```typescript
import { RealtimeClient } from '@agentc/realtime-core';
```

## Constructor

```typescript
constructor(config: RealtimeClientConfig)
```

Creates a new RealtimeClient instance.

### Parameters

- `config` (RealtimeClientConfig) - Configuration object for the client

### Configuration Options

```typescript
interface RealtimeClientConfig {
  // Required
  apiUrl: string;                    // WebSocket URL from login response or provided
  
  // Authentication (one required)
  authToken?: string;                // JWT token for authentication
  authManager?: AuthManager;         // AuthManager instance for token management
  
  // Optional
  sessionId?: string;                // UI session ID for reconnection
  enableAudio?: boolean;             // Enable audio subsystem (default: false)
  audioConfig?: AudioConfig;         // Audio configuration
  enableTurnManager?: boolean;       // Enable turn management (default: true)
  autoReconnect?: boolean;           // Enable automatic reconnection (default: true)
  reconnection?: ReconnectionConfig; // Reconnection settings
  debug?: boolean;                   // Enable debug logging (default: false)
  connectionTimeout?: number;        // Connection timeout in ms (default: 10000)
  pingInterval?: number;             // WebSocket ping interval in ms (default: 30000)
  pongTimeout?: number;              // Pong timeout in ms (default: 10000)
  maxMessageSize?: number;           // Max message size in bytes (default: 10MB)
  binaryType?: 'blob' | 'arraybuffer'; // Binary type (default: 'arraybuffer')
  protocols?: string[];              // WebSocket subprotocols
  headers?: Record<string, string>;  // Additional headers
}

interface AudioConfig {
  enableInput?: boolean;             // Enable microphone input (default: true)
  enableOutput?: boolean;            // Enable audio output (default: true)
  respectTurnState?: boolean;        // Respect turn management (default: true)
  logAudioChunks?: boolean;          // Log audio chunks for debugging (default: false)
  sampleRate?: number;               // Audio sample rate (default: 24000)
  chunkSize?: number;                // Chunk size in samples (default: 4800)
  initialVolume?: number;            // Initial volume 0-1 (default: 1.0)
  autoGainControl?: boolean;         // Enable AGC (default: true)
  echoCancellation?: boolean;        // Enable echo cancellation (default: true)
  noiseSuppression?: boolean;        // Enable noise suppression (default: true)
}

interface ReconnectionConfig {
  enabled?: boolean;                  // Enable auto-reconnection (default: true)
  initialDelay?: number;             // Initial delay in ms (default: 1000)
  maxDelay?: number;                 // Max delay in ms (default: 30000)
  backoffMultiplier?: number;        // Backoff multiplier (default: 1.5)
  maxAttempts?: number;              // Max attempts, 0=unlimited (default: 0)
  jitterFactor?: number;             // Jitter factor 0-1 (default: 0.3)
}
```

### Example

```typescript
// Using AuthManager (recommended)
const authManager = new AuthManager({ 
  apiUrl: 'https://localhost:8000' 
});
const loginResponse = await authManager.login({ 
  username: 'your-username',
  password: 'your-password' 
});

const client = new RealtimeClient({
  apiUrl: loginResponse.wsUrl, // WebSocket URL from login response
  authManager,
  enableAudio: true,
  audioConfig: {
    enableInput: true,
    enableOutput: true,
    respectTurnState: true,
    sampleRate: 24000
  },
  debug: true
});

// Or with JWT token directly
const client = new RealtimeClient({
  apiUrl: wsUrl, // WebSocket URL from login or environment
  authToken: jwtToken, // JWT token from login
  enableAudio: true,
  debug: true
});
```

## Connection Methods

### connect()

Establishes a WebSocket connection to the Agent C server.

```typescript
async connect(): Promise<void>
```

**Returns:** Promise that resolves when connected

**Throws:** Error if connection fails or times out

**Note:** Upon successful connection, the server sends 6 initialization events:
1. `chat_user_data` - User information
2. `avatar_list` - Available avatars
3. `voice_list` - Available voices
4. `agent_list` - Available agents
5. `tool_catalog` - Available tools
6. `chat_session_changed` - Initial session

**Example:**
```typescript
try {
  await client.connect();
  console.log('Connected!');
  
  // Wait for initialization to complete if needed
  await client.waitForInitialization();
} catch (error) {
  console.error('Connection failed:', error);
}
```

### disconnect()

Closes the WebSocket connection.

```typescript
disconnect(): void
```

**Example:**
```typescript
client.disconnect();
```

### isConnected()

Checks if the client is currently connected.

```typescript
isConnected(): boolean
```

**Returns:** `true` if connected, `false` otherwise

**Example:**
```typescript
if (client.isConnected()) {
  client.sendText('Hello!');
}
```

### getConnectionState()

Gets the current connection state.

```typescript
getConnectionState(): ConnectionState
```

**Returns:** Current ConnectionState enum value

```typescript
enum ConnectionState {
  DISCONNECTED = 'DISCONNECTED',
  CONNECTING = 'CONNECTING',
  CONNECTED = 'CONNECTED',
  RECONNECTING = 'RECONNECTING'
}
```

**Example:**
```typescript
const state = client.getConnectionState();
if (state === ConnectionState.CONNECTED) {
  // Ready to send messages
}
```

## Message Methods

### sendText()

Sends a text message to the agent.

```typescript
sendText(text: string, fileIds?: string[]): void
```

**Parameters:**
- `text` (string) - The text message to send
- `fileIds` (string[], optional) - Array of file IDs to attach

**Throws:** Error if not connected

**Example:**
```typescript
client.sendText('What is the weather today?');

// With file attachments
client.sendText('Analyze this document', ['file123', 'file456']);
```

### sendBinaryFrame()

Sends raw binary data (audio) to the server.

```typescript
sendBinaryFrame(data: ArrayBuffer | ArrayBufferView): void
```

**Parameters:**
- `data` - Binary audio data (PCM16 format)

**Throws:** Error if not connected or binary not supported

**Note:** This sends raw PCM16 audio directly over WebSocket, not wrapped in JSON

**Example:**
```typescript
const audioData = new Int16Array(2400); // 100ms of 24kHz audio
client.sendBinaryFrame(audioData.buffer);
```

### sendEvent()

Sends a raw event to the server. This is an advanced method that allows sending any valid ClientEventMap event directly.

```typescript
sendEvent<K extends keyof ClientEventMap>(event: ClientEventMap[K]): void
```

**Parameters:**
- `event` - A valid event object from the ClientEventMap type

**Throws:** Error if not connected to the server

**Note:** This is a public API method. Most use cases should use the higher-level methods like `sendText()`, `setAgent()`, etc. Use this method when you need direct control over the event structure or for advanced integrations.

**Example:**
```typescript
// Send a ping event
client.sendEvent({ type: 'ping' });

// Send a text input event (equivalent to sendText)
client.sendEvent({ 
  type: 'text_input', 
  text: 'Hello',
  file_ids: ['file123'] 
});

// Send a session management event
client.sendEvent({ 
  type: 'resume_chat_session', 
  session_id: 'session-123' 
});

// TypeScript ensures type safety
client.sendEvent<'set_agent'>({ 
  type: 'set_agent', 
  agent_key: 'support-agent' 
});

// Cancel the current agent response
client.sendEvent({ type: 'client_wants_cancel' });
```

### cancelResponse()

Cancels the current agent response. Convenience method for sending `client_wants_cancel` event.

```typescript
cancelResponse(): void
```

**Throws:** Error if not connected

**Note:** Server will respond with a `cancelled` event to confirm cancellation

**Example:**
```typescript
// Cancel ongoing agent response
client.cancelResponse();

// Listen for confirmation
client.on('cancelled', () => {
  console.log('Response cancelled');
});
```

## File Upload

### uploadFile()

Uploads a file to the server for use in chat messages. Returns file metadata including a unique ID that can be used in `sendText()` to attach the file to a message.

```typescript
async uploadFile(
  file: File, 
  options?: FileUploadOptions
): Promise<UserFileResponse>
```

**Parameters:**
- `file` (File) - The file to upload (from file input or drag-drop)
- `options` (FileUploadOptions, optional) - Upload options:
  - `onProgress` - Callback function for upload progress tracking
  - `signal` - AbortSignal for cancellation support

**Returns:** Promise resolving to `UserFileResponse` with file metadata:
- `id` (string) - Unique file identifier to use in messages
- `filename` (string) - Original filename
- `mime_type` (string) - File MIME type
- `size` (number) - File size in bytes

**Throws:** 
- Error if not authenticated
- Error if no UI session ID
- Error if file size exceeds `maxUploadSize` configuration
- Error if file MIME type not in `allowedMimeTypes` (if configured)
- Error if network failure or server error

**Upload Process:**
1. Validates authentication and session ID
2. Validates file size against `maxUploadSize` configuration
3. Validates file MIME type against `allowedMimeTypes` (if configured)
4. Uploads file via HTTP POST to `/api/rt/upload_file`
5. Returns file metadata including server-assigned ID

**Example: Basic Upload**
```typescript
const fileInput = document.getElementById('fileInput');
const file = fileInput.files[0];

try {
  const uploadedFile = await client.uploadFile(file);
  
  console.log('Upload complete:', uploadedFile);
  // { id: 'file-abc123', filename: 'report.pdf', mime_type: 'application/pdf', size: 245678 }
  
  // Send message with file attachment
  client.sendText('Please summarize this report', [uploadedFile.id]);
} catch (error) {
  console.error('Upload failed:', error.message);
}
```

**Example: Upload with Progress Tracking**
```typescript
const uploadedFile = await client.uploadFile(file, {
  onProgress: (progress) => {
    console.log(`Upload progress: ${progress.percentage}%`);
    console.log(`${progress.loaded} / ${progress.total} bytes`);
    
    // Update UI progress bar
    document.getElementById('progressBar').value = progress.percentage;
  }
});

client.sendText('Analyze this image', [uploadedFile.id]);
```

**Example: Upload with Cancellation**
```typescript
const controller = new AbortController();

// Start upload
const uploadPromise = client.uploadFile(file, {
  signal: controller.signal,
  onProgress: (progress) => {
    console.log(`Progress: ${progress.percentage}%`);
  }
});

// Allow user to cancel
document.getElementById('cancelButton').onclick = () => {
  controller.abort();
  console.log('Upload cancelled');
};

try {
  const result = await uploadPromise;
  console.log('Upload successful:', result);
} catch (error) {
  if (error.message === 'Upload cancelled') {
    console.log('User cancelled the upload');
  } else {
    console.error('Upload error:', error);
  }
}
```

**Error Handling:**
```typescript
try {
  const result = await client.uploadFile(file);
} catch (error) {
  if (error.message.includes('Authentication required')) {
    // Not logged in or token expired
    console.error('Please log in again');
  } else if (error.message.includes('exceeds maximum')) {
    // File too large
    console.error('File is too large. Maximum size: 10MB');
  } else if (error.message.includes('not allowed')) {
    // Invalid file type
    console.error('File type not supported');
  } else if (error.message.includes('Network error')) {
    // Network issue
    console.error('Upload failed due to network error');
  } else {
    // Other errors
    console.error('Upload failed:', error.message);
  }
}
```

### uploadFiles()

Uploads multiple files sequentially. Provides aggregated progress tracking across all files.

```typescript
async uploadFiles(
  files: File[], 
  options?: FileUploadOptions
): Promise<UserFileResponse[]>
```

**Parameters:**
- `files` (File[]) - Array of files to upload
- `options` (FileUploadOptions, optional) - Upload options:
  - `onProgress` - Callback for aggregated progress across all files
  - `signal` - AbortSignal to cancel all uploads

**Returns:** Promise resolving to array of `UserFileResponse` objects

**Throws:**
- Error if file count exceeds `maxFilesPerMessage` configuration
- Error if any individual file fails validation
- Error if network failure or server error (includes partial success info)

**Behavior:**
- Files are uploaded **sequentially** (one at a time), not in parallel
- Progress callback reports overall progress across all files
- If one upload fails, the error includes information about successfully uploaded files
- Cancellation via AbortSignal stops all remaining uploads

**Example: Upload Multiple Files**
```typescript
const fileInput = document.getElementById('multiFileInput');
const files = Array.from(fileInput.files);

try {
  const uploadedFiles = await client.uploadFiles(files, {
    onProgress: (progress) => {
      console.log(`Overall progress: ${progress.percentage}%`);
      updateProgressBar(progress.percentage);
    }
  });
  
  console.log(`Successfully uploaded ${uploadedFiles.length} files`);
  
  // Extract file IDs
  const fileIds = uploadedFiles.map(f => f.id);
  
  // Send message with all files
  client.sendText('Compare these documents', fileIds);
} catch (error) {
  console.error('Upload failed:', error.message);
  
  // Error message includes info about partial success
  // "Failed to upload file 3 of 5: Network error. 2 file(s) uploaded successfully before failure."
}
```

**Example: Upload with Cancellation**
```typescript
const controller = new AbortController();
let uploadedCount = 0;

const uploadPromise = client.uploadFiles(files, {
  signal: controller.signal,
  onProgress: (progress) => {
    console.log(`Uploading: ${progress.percentage}%`);
    
    // Estimate which file we're on
    uploadedCount = Math.floor((progress.percentage / 100) * files.length);
    console.log(`File ${uploadedCount + 1} of ${files.length}`);
  }
});

// Allow cancellation
cancelButton.onclick = () => controller.abort();

try {
  const results = await uploadPromise;
  console.log(`All ${results.length} files uploaded`);
} catch (error) {
  console.log(`Upload stopped. ${uploadedCount} files uploaded before cancellation.`);
}
```

**Configuration:**

File upload behavior is configured in `RealtimeClientConfig`:

```typescript
const client = new RealtimeClient({
  apiUrl: 'wss://api.agentc.ai/realtime',
  authManager: authManager,
  
  // File upload configuration
  maxUploadSize: 10 * 1024 * 1024,    // 10MB max file size (default)
  allowedMimeTypes: [                  // Allowed file types (default: undefined = all)
    'image/png',
    'image/jpeg',
    'image/gif',
    'application/pdf',
    'text/plain'
  ],
  maxFilesPerMessage: 10               // Max files per upload (default: 10)
});
```

**Type Definitions:**

```typescript
// File metadata response
interface UserFileResponse {
  id: string;           // Unique file identifier
  filename: string;     // Original filename
  mime_type: string;    // File MIME type (e.g., 'image/png')
  size: number;         // File size in bytes
}

// Upload options
interface FileUploadOptions {
  onProgress?: (progress: UploadProgress) => void;  // Progress callback
  signal?: AbortSignal;                              // Cancellation signal
}

// Progress information
interface UploadProgress {
  loaded: number;       // Bytes uploaded so far
  total: number;        // Total bytes to upload
  percentage: number;   // Progress percentage (0-100)
}
```

**Complete Example:**

```typescript
import { RealtimeClient, AuthManager } from '@agentc/realtime-core';

async function uploadAndChat() {
  // Setup client with upload configuration
  const authManager = new AuthManager({ apiUrl: 'https://api.agentc.ai' });
  await authManager.login({ username: 'user', password: 'pass' });
  
  const client = new RealtimeClient({
    apiUrl: authManager.getWebSocketUrl(),
    authManager,
    maxUploadSize: 10 * 1024 * 1024,
    allowedMimeTypes: ['image/png', 'image/jpeg', 'application/pdf']
  });
  
  await client.connect();
  await client.waitForInitialization();
  
  // Get files from input
  const fileInput = document.getElementById('fileInput');
  const files = Array.from(fileInput.files);
  
  // Validate files before upload
  for (const file of files) {
    if (file.size > 10 * 1024 * 1024) {
      alert(`File ${file.name} is too large (max 10MB)`);
      return;
    }
  }
  
  // Upload files with progress
  const progressBar = document.getElementById('progressBar');
  let uploadedFiles;
  
  try {
    uploadedFiles = await client.uploadFiles(files, {
      onProgress: (progress) => {
        progressBar.value = progress.percentage;
        console.log(`Upload: ${progress.percentage}%`);
      }
    });
  } catch (error) {
    alert(`Upload failed: ${error.message}`);
    return;
  }
  
  console.log('All files uploaded:', uploadedFiles);
  
  // Send message with uploaded files
  const fileIds = uploadedFiles.map(f => f.id);
  client.sendText('Please analyze these files', fileIds);
  
  // Handle response
  let response = '';
  client.on('text_delta', (event) => {
    response += event.content;
    document.getElementById('response').textContent = response;
  });
  
  client.on('completion', (event) => {
    if (!event.running) {
      console.log('Analysis complete');
    }
  });
}
```

**Notes:**

- Files must be uploaded **before** sending the message that references them
- File IDs are session-specific and temporary (don't persist across sessions)
- Large files may take time to upload; always provide progress feedback
- Upload uses HTTP (not WebSocket) for better progress tracking and reliability
- Files are validated both client-side (before upload) and server-side (during upload)
- Server performs additional security checks (virus scanning, content verification)

## Agent Management

### getAgents()

Requests the list of available agents. Note: This is typically not needed as agents are provided during initialization.

```typescript
getAgents(): void
```

**Emits:** `agent_list` event with available agents

**Example:**
```typescript
client.getAgents();
client.on('agent_list', (event) => {
  console.log('Available agents:', event.agents);
});
```

### getAgentsList()

Gets the list of agents received during initialization.

```typescript
getAgentsList(): Agent[]
```

**Returns:** Array of available agents

**Example:**
```typescript
const agents = client.getAgentsList();
agents.forEach(agent => {
  console.log(agent.name, agent.key);
});
```

### setAgent()

Sets the active agent for the session.

```typescript
setAgent(agentKey: string): void
```

**Parameters:**
- `agentKey` (string) - The agent identifier

**Example:**
```typescript
client.setAgent('customer-service-agent');
```

### getCurrentAgentConfig()

Gets the current agent configuration from the active session.

```typescript
getCurrentAgentConfig(): AgentConfiguration | null
```

**Returns:** Current agent configuration or null if no session is active

**Example:**
```typescript
const config = client.getCurrentAgentConfig();
if (config) {
  console.log('Current agent:', config.key, config.name);
}
```

## Voice Management

### setAgentVoice()

Changes the agent's text-to-speech voice.

```typescript
setAgentVoice(voiceId: string): void
```

**Parameters:**
- `voiceId` (string) - Voice identifier (e.g., 'nova', 'echo', 'none', 'avatar')

**Special voice IDs:**
- `'none'` - Disable audio output
- `'avatar'` - Use avatar voice (HeyGen integration)

**Example:**
```typescript
client.setAgentVoice('nova');

// Disable audio output
client.setAgentVoice('none');

// Use avatar voice (HeyGen)
client.setAgentVoice('avatar');
```

### getVoices()

Requests the list of available voices from the server.

```typescript
getVoices(): void
```

**Emits:** `voice_list` event with available voices

**Example:**
```typescript
client.getVoices();
client.on('voice_list', (event) => {
  console.log('Available voices:', event.voices);
});
```

### getVoicesList()

Gets the list of voices received during initialization.

```typescript
getVoicesList(): Voice[]
```

**Returns:** Array of available voices

**Example:**
```typescript
const voices = client.getVoicesList();
voices.forEach(voice => {
  console.log(voice.voice_id, voice.description);
});
```

### getVoiceManager()

Gets the VoiceManager instance.

```typescript
getVoiceManager(): VoiceManager | null
```

**Returns:** VoiceManager instance or null if not initialized

**Example:**
```typescript
const voiceManager = client.getVoiceManager();
const currentVoice = voiceManager?.getCurrentVoice();
```

## Session Management

### newChatSession()

Creates a new chat session.

```typescript
newChatSession(agentKey?: string): void
```

**Parameters:**
- `agentKey` (string, optional) - Agent to use for the new session

**Emits:** `chat_session_changed` event with new session details

**Example:**
```typescript
client.newChatSession();

// With specific agent
client.newChatSession('sales-agent');
```

### resumeChatSession()

Resumes an existing chat session.

```typescript
resumeChatSession(sessionId: string): void
```

**Parameters:**
- `sessionId` (string) - Session ID to resume

**Example:**
```typescript
client.resumeChatSession('session-123-456');
```

### setChatSessionName()

Sets a name for the current chat session.

```typescript
setChatSessionName(sessionName: string): void
```

**Parameters:**
- `sessionName` (string) - Name for the session

**Example:**
```typescript
client.setChatSessionName('Product inquiry - John Doe');
```

### setSessionMetadata()

Sets metadata for the current session.

```typescript
setSessionMetadata(meta: Record<string, any>): void
```

**Parameters:**
- `meta` (object) - Metadata object

**Example:**
```typescript
client.setSessionMetadata({
  userId: 'user123',
  topic: 'technical-support',
  priority: 'high'
});
```

### setSessionMessages()

Sets the message history for the current session.

```typescript
setSessionMessages(messages: Message[]): void
```

**Parameters:**
- `messages` (Message[]) - Array of message objects

**Example:**
```typescript
client.setSessionMessages([
  { role: 'user', content: 'Hello' },
  { role: 'assistant', content: 'Hi there!' }
]);
```

### fetchUserSessions()

Fetches paginated list of user sessions.

```typescript
fetchUserSessions(offset?: number, limit?: number): void
```

**Parameters:**
- `offset` (number, optional) - Starting offset for pagination (default: 0)
- `limit` (number, optional) - Number of sessions to fetch (default: 50)

**Emits:** `get_user_sessions_response` event with session list

**Example:**
```typescript
client.fetchUserSessions(0, 20);
client.on('get_user_sessions_response', (event) => {
  console.log('Sessions:', event.sessions.chat_sessions);
  console.log('Total:', event.sessions.total_sessions);
});
```

### getSessionManager()

Gets the SessionManager instance.

```typescript
getSessionManager(): SessionManager | null
```

**Returns:** SessionManager instance or null

**Example:**
```typescript
const sessionManager = client.getSessionManager();
const currentSession = sessionManager?.getCurrentSession();
```

## Audio Control

### startAudioRecording()

Starts recording from the microphone.

```typescript
async startAudioRecording(): Promise<void>
```

**Returns:** Promise that resolves when recording starts

**Throws:** Error if audio not enabled or permission denied

**Example:**
```typescript
try {
  await client.startAudioRecording();
  console.log('Recording started');
} catch (error) {
  if (error.name === 'NotAllowedError') {
    console.error('Microphone permission denied');
  }
}
```

### stopAudioRecording()

Stops microphone recording.

```typescript
stopAudioRecording(): void
```

**Example:**
```typescript
client.stopAudioRecording();
```

### startAudioStreaming()

Starts streaming audio to the server.

```typescript
startAudioStreaming(): void
```

**Throws:** Error if not connected or audio not initialized

**Example:**
```typescript
await client.startAudioRecording();
client.startAudioStreaming();
```

### stopAudioStreaming()

Stops streaming audio to the server.

```typescript
stopAudioStreaming(): void
```

**Example:**
```typescript
client.stopAudioStreaming();
```

### setAudioVolume()

Sets the audio playback volume.

```typescript
setAudioVolume(volume: number): void
```

**Parameters:**
- `volume` (number) - Volume level between 0 and 1

**Throws:** Error if volume out of range

**Example:**
```typescript
client.setAudioVolume(0.8); // 80% volume
```

### getAudioStatus()

Gets the current audio system status.

```typescript
getAudioStatus(): AudioStatus
```

**Returns:** AudioStatus object

```typescript
interface AudioStatus {
  // Input status
  isRecording: boolean;
  isStreaming: boolean;
  hasPermission: boolean;
  currentLevel: number;      // Current audio level (0-1)
  averageLevel: number;      // Average audio level (0-1)
  
  // Output status
  isPlaying: boolean;
  bufferSize: number;
  volume: number;
  
  // System status
  isAudioEnabled: boolean;
  isInputEnabled: boolean;
  isOutputEnabled: boolean;
}
```

**Example:**
```typescript
const status = client.getAudioStatus();
console.log('Recording:', status.isRecording);
console.log('Audio level:', status.currentLevel);
```

## Avatar Management

### setAvatar()

Sets the avatar for the current session. This creates a new HeyGen avatar session.

```typescript
setAvatar(avatarId: string, quality?: string, videoEncoding?: string): void
```

**Parameters:**
- `avatarId` (string) - The ID of the avatar to set
- `quality` (string, optional) - Quality setting (default: "auto")
- `videoEncoding` (string, optional) - Video encoding (default: "H265")

**Example:**
```typescript
client.setAvatar('avatar-123', 'auto', 'H265');
```

### setAvatarSession()

Notifies the server that a HeyGen avatar session is active. Called after HeyGen STREAM_READY event.

```typescript
setAvatarSession(accessToken: string, avatarSessionId: string): void
```

**Parameters:**
- `accessToken` (string) - HeyGen access token for the session
- `avatarSessionId` (string) - HeyGen avatar session ID

**Example:**
```typescript
// After HeyGen STREAM_READY event
client.setAvatarSession('heygen-token', 'heygen-session-123');
```

### clearAvatarSession()

Clears the current avatar session.

```typescript
clearAvatarSession(): void
```

**Example:**
```typescript
client.clearAvatarSession();
```

### getAvatarManager()

Gets the AvatarManager instance.

```typescript
getAvatarManager(): AvatarManager | null
```

**Returns:** AvatarManager instance or null

### getAvailableAvatars()

Gets the list of available avatars.

```typescript
getAvailableAvatars(): Avatar[]
```

**Returns:** Array of available avatars

**Example:**
```typescript
const avatars = client.getAvailableAvatars();
avatars.forEach(avatar => {
  console.log(avatar.name, avatar.avatar_id);
});
```

### getHeyGenAccessToken()

Gets the HeyGen access token for creating avatar sessions.

```typescript
getHeyGenAccessToken(): string | null
```

**Returns:** HeyGen access token or null

**Example:**
```typescript
const token = client.getHeyGenAccessToken();
if (token) {
  // Use token to create HeyGen session
}
```

## Initialization & Configuration

### isFullyInitialized()

Checks if the client has completed initialization (all 6 initialization events received).

```typescript
isFullyInitialized(): boolean
```

**Returns:** `true` if initialization is complete, `false` otherwise

**Example:**
```typescript
if (client.isFullyInitialized()) {
  // Safe to access initialization data
  const agents = client.getAgentsList();
}
```

### waitForInitialization()

Waits for initialization to complete.

```typescript
waitForInitialization(): Promise<void>
```

**Returns:** Promise that resolves when all initialization events are received

**Example:**
```typescript
await client.connect();
await client.waitForInitialization();
console.log('Initialization complete');
```

### getUserData()

Gets the current user data from initialization events.

```typescript
getUserData(): User | null
```

**Returns:** User object or null if not initialized

**Example:**
```typescript
const user = client.getUserData();
if (user) {
  console.log('Username:', user.user_name);
}
```

### getToolCatalog()

Requests the tool catalog from the server.

```typescript
getToolCatalog(): void
```

**Emits:** `tool_catalog` event with available tools

**Example:**
```typescript
client.getToolCatalog();
client.on('tool_catalog', (event) => {
  console.log('Available tools:', event.tools);
});
```

### getTools()

Gets the list of tools received during initialization.

```typescript
getTools(): Tool[]
```

**Returns:** Array of available tools

**Example:**
```typescript
const tools = client.getTools();
tools.forEach(tool => {
  console.log(tool.name, tool.description);
});
```

### ping()

Sends a ping to the server for connection health check.

```typescript
ping(): void
```

**Note:** Server responds with `pong` event

**Example:**
```typescript
client.ping();
client.on('pong', () => {
  console.log('Server is responsive');
});
```

## Event Handling

### on()

Subscribes to an event.

```typescript
on<K extends keyof RealtimeEventMap>(
  event: K, 
  handler: (data: RealtimeEventMap[K]) => void
): void
```

**Parameters:**
- `event` - Event name
- `handler` - Event handler function

**Example:**
```typescript
client.on('text_delta', (event) => {
  console.log('Received:', event.content);
});

client.on('connected', () => {
  console.log('Connected to server');
});

client.on('error', (error) => {
  console.error('Error:', error.message);
});
```

### off()

Unsubscribes from an event.

```typescript
off<K extends keyof RealtimeEventMap>(
  event: K, 
  handler: (data: RealtimeEventMap[K]) => void
): void
```

**Example:**
```typescript
const handler = (event) => console.log(event);
client.on('text_delta', handler);
// Later...
client.off('text_delta', handler);
```

### once()

Subscribes to an event for a single occurrence.

```typescript
once<K extends keyof RealtimeEventMap>(
  event: K, 
  handler: (data: RealtimeEventMap[K]) => void
): void
```

**Example:**
```typescript
client.once('connected', () => {
  console.log('First connection established');
});
```

## Available Events

### Connection Events
- `connected` - Connection established
- `disconnected` - Connection closed
- `reconnecting` - Attempting to reconnect
- `reconnected` - Successfully reconnected

### Initialization Events (sent on connection)
- `chat_user_data` - User information
- `avatar_list` - Available avatars
- `voice_list` - Available voices
- `agent_list` - Available agents
- `tool_catalog` - Available tools
- `initialized` - All initialization events received (custom event)

### Message Events
- `text_delta` - Streaming text chunk
- `thought_delta` - Agent thinking process chunk
- `completion` - Response completion status
- `interaction` - Full interaction event
- `error` - Error occurred
- `cancelled` - Response cancelled

### History Events
- `history` - Complete history update
- `history_delta` - Incremental history change
- `user_message` - User message added
- `anthropic_user_message` - Anthropic format user message
- `openai_user_message` - OpenAI format user message

### Audio Events
- `audio:output` - Binary audio data received
- `binary_audio` - Binary audio (legacy, same as audio:output)
- `voice_input_supported` - Voice input capability
- `server_listening` - Server listening for audio

### Turn Events
- `user_turn_start` - User's turn to speak
- `user_turn_end` - User turn ended
- `agent_turn_start` - Agent's turn to respond (deprecated)
- `agent_turn_end` - Agent turn ended (deprecated)

### Session Events
- `chat_session_changed` - Active session changed
- `chat_session_name_changed` - Session renamed
- `session_metadata_changed` - Metadata updated
- `chat_session_added` - New session created
- `chat_session_deleted` - Session deleted
- `get_user_sessions_response` - Session list received
- `subsession_started` - Sub-session started
- `subsession_ended` - Sub-session ended

### Voice Events
- `agent_voice_changed` - Agent voice changed
- `voice_list` - Available voices received

### Avatar Events
- `avatar_connection_changed` - Avatar connection status changed
- `avatar_list` - Available avatars received

### Agent Events
- `agent_list` - Available agents received
- `agent_configuration_changed` - Agent config updated

### Tool Events
- `tool_catalog` - Tool catalog received
- `tool_select_delta` - Tool selection update
- `tool_call` - Tool invocation

### System Events
- `system_message` - System message
- `system_prompt` - System prompt
- `render_media` - Media rendering request
- `user_request` - User request event
- `ping` - Ping from server
- `pong` - Pong response

## Utility Methods

### setAuthManager()

Sets or updates the AuthManager instance.

```typescript
setAuthManager(authManager: AuthManager): void
```

**Parameters:**
- `authManager` - AuthManager instance

**Example:**
```typescript
const authManager = new AuthManager({ apiUrl: 'https://localhost:8000' });
await authManager.login({ 
  username: 'your-username',
  password: 'your-password' 
});
client.setAuthManager(authManager);
```

### setAuthToken()

Updates the authentication token.

```typescript
setAuthToken(token: string): void
```

**Parameters:**
- `token` (string) - JWT authentication token

**Note:** Causes reconnection if currently connected

**Example:**
```typescript
client.setAuthToken('new-jwt-token');
```

### setSessionId()

Updates the UI session ID.

```typescript
setSessionId(sessionId: string | null): void
```

**Parameters:**
- `sessionId` - Session ID or null

**Note:** Causes reconnection if currently connected

### getTurnManager()

Gets the TurnManager instance.

```typescript
getTurnManager(): TurnManager | null
```

**Returns:** TurnManager instance or null

### getAuthManager()

Gets the AuthManager instance.

```typescript
getAuthManager(): AuthManager | null
```

**Returns:** AuthManager instance or null

### destroy()

Cleans up all resources and closes connections.

```typescript
destroy(): void
```

**Example:**
```typescript
// Clean up when done
client.destroy();
```

## Complete Example

```typescript
import { RealtimeClient, AuthManager, ConnectionState } from '@agentc/realtime-core';

async function main() {
  // Initialize authentication
  const authManager = new AuthManager({
    apiUrl: 'https://localhost:8000'
  });
  
  // Login with credentials
  const loginResponse = await authManager.login({
    username: 'your-username',
    password: 'your-password'
  });
  
  // Create client with WebSocket URL from login
  const client = new RealtimeClient({
    apiUrl: loginResponse.wsUrl,
    authManager,
    enableAudio: true,
    audioConfig: {
      enableInput: true,
      enableOutput: true,
      respectTurnState: true,
      sampleRate: 24000,
      initialVolume: 0.8
    },
    reconnection: {
      maxAttempts: 5,
      initialDelay: 1000
    },
    debug: true
  });
  
  // Set up event handlers
  client.on('connected', async () => {
    console.log('✅ Connected to Agent C');
    
    // Wait for initialization
    await client.waitForInitialization();
    console.log('✅ Initialization complete');
    
    // Show available agents
    const agents = client.getAgentsList();
    console.log('Available agents:', agents.map(a => a.name));
  });
  
  client.on('text_delta', (event) => {
    process.stdout.write(event.content);
  });
  
  client.on('completion', (event) => {
    if (!event.running) {
      console.log('\n📝 Response complete');
    }
  });
  
  client.on('audio:output', (audioData) => {
    console.log('🔊 Audio chunk:', audioData.byteLength, 'bytes');
  });
  
  client.on('user_turn_start', () => {
    console.log('🎤 Your turn to speak');
  });
  
  client.on('cancelled', () => {
    console.log('❌ Response cancelled');
  });
  
  client.on('error', (error) => {
    console.error('❌ Error:', error.message);
  });
  
  // Connect to the service
  await client.connect();
  
  // Send a text message
  client.sendText('Hello! Please introduce yourself.');
  
  // Start audio if available
  const audioStatus = client.getAudioStatus();
  if (audioStatus.isAudioEnabled) {
    try {
      await client.startAudioRecording();
      client.startAudioStreaming();
      console.log('🎤 Audio streaming started');
    } catch (error) {
      console.error('Audio setup failed:', error);
    }
  }
  
  // Set voice preference
  client.setAgentVoice('nova');
  
  // Handle cancellation
  setTimeout(() => {
    if (client.getConnectionState() === ConnectionState.CONNECTED) {
      client.cancelResponse();
    }
  }, 10000);
  
  // Clean up on exit
  process.on('SIGINT', () => {
    console.log('\nShutting down...');
    client.destroy();
    process.exit(0);
  });
}

main().catch(console.error);
```

## Error Handling

The RealtimeClient throws errors in these situations:

1. **Connection Errors**
   - Invalid API URL
   - Authentication failure (401)
   - Connection timeout
   - Network errors

2. **Configuration Errors**
   - Missing required configuration
   - Invalid configuration values
   - Missing authentication (no token or AuthManager)

3. **State Errors**
   - Attempting to send when not connected
   - Audio operations when audio not enabled
   - Invalid volume range (must be 0-1)

4. **Permission Errors**
   - Microphone permission denied
   - Browser doesn't support required APIs

### Error Event Codes

The `error` event may include these error codes from the server:

- `auth_failed` - Authentication failure
- `invalid_request` - Invalid request format
- `rate_limited` - Rate limit exceeded
- `internal_error` - Server internal error
- `session_not_found` - Session ID not found
- `agent_not_found` - Agent key not found
- `voice_not_found` - Voice ID not found
- `avatar_not_found` - Avatar ID not found

Always wrap async operations in try-catch blocks and listen for error events:

```typescript
// Handle connection errors
try {
  await client.connect();
} catch (error) {
  if (error.message.includes('Authentication')) {
    // Handle auth error - token may be expired
  } else if (error.message.includes('timeout')) {
    // Handle timeout - server may be down
  }
}

// Handle runtime errors
client.on('error', (error) => {
  console.error('Runtime error:', error);
  
  // Check error code for specific handling
  if (error.code === 'auth_failed') {
    // Refresh authentication
  }
});
```

## TypeScript Types

The RealtimeClient is fully typed. Key types include:

```typescript
import {
  RealtimeClient,
  RealtimeClientConfig,
  ConnectionState,
  AudioConfig,
  AudioStatus,
  RealtimeEventMap,
  ClientEventMap,
  ServerEventMap,
  TextDeltaEvent,
  CompletionEvent,
  ErrorEvent,
  // ... other event types
} from '@agentc/realtime-core';
```

All event handlers are type-safe, providing IntelliSense and compile-time checking.