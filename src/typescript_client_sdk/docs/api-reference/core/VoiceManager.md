# VoiceManager API Reference

The `VoiceManager` class manages text-to-speech voices, handles voice switching, and coordinates with the audio output system.

## Import

```typescript
import { VoiceManager } from '@agentc/realtime-core';
```

## Overview

The VoiceManager tracks available TTS voices, manages the current voice selection, and ensures proper audio format handling for different voice models.

## Constructor

```typescript
constructor(config?: VoiceManagerConfig)
```

Creates a new VoiceManager instance.

### Parameters

- `config` (VoiceManagerConfig, optional) - Configuration object

```typescript
interface VoiceManagerConfig {
  defaultVoice?: string;      // Default voice ID (default: 'nova')
  enableLogging?: boolean;     // Enable debug logging (default: false)
}
```

**Note:** VoiceManager is typically created automatically by RealtimeClient

### Example

```typescript
// Setup authentication first
const authManager = new AuthManager({
  apiUrl: 'https://localhost:8000'
});

await authManager.login('username', 'password');

// VoiceManager is created automatically
const client = new RealtimeClient({
  apiUrl: 'wss://localhost:8000/rt/ws',
  authManager
});

// Access the VoiceManager instance
const voiceManager = client.getVoiceManager();
```

## Properties

### currentVoice

Gets the currently active voice.

```typescript
get currentVoice(): Voice | null
```

**Returns:** Current voice object or null

```typescript
interface Voice {
  voice_id: string;          // Unique identifier (e.g., 'nova', 'echo')
  name: string;              // Display name
  vendor: string;            // Provider (e.g., 'openai')
  description?: string;      // Voice description
  output_format?: string;    // Audio format (e.g., 'pcm16')
}
```

**Example:**
```typescript
const voice = voiceManager.currentVoice;
if (voice) {
  console.log(`Current voice: ${voice.name} (${voice.voice_id})`);
  console.log(`Format: ${voice.output_format}`);
}
```

### currentVoiceId

Gets the ID of the currently active voice.

```typescript
get currentVoiceId(): string | null
```

**Returns:** Voice ID or null

**Example:**
```typescript
const voiceId = voiceManager.currentVoiceId;
console.log(`Using voice: ${voiceId || 'none'}`);
```

### availableVoices

Gets all available voices.

```typescript
get availableVoices(): Voice[]
```

**Returns:** Array of available voices

**Example:**
```typescript
const voices = voiceManager.availableVoices;
voices.forEach(voice => {
  console.log(`${voice.name}: ${voice.description}`);
});
```

### isAvatarMode

Checks if avatar voice mode is active.

```typescript
get isAvatarMode(): boolean
```

**Returns:** `true` if using avatar voice

**Example:**
```typescript
if (voiceManager.isAvatarMode) {
  console.log('Avatar is handling audio');
  // Don't expect audio events
}
```

### isTextOnly

Checks if text-only mode is active (no audio).

```typescript
get isTextOnly(): boolean
```

**Returns:** `true` if voice is set to 'none'

**Example:**
```typescript
if (voiceManager.isTextOnly) {
  console.log('Text-only mode, no audio output');
}
```

## Methods

### setCurrentVoice()

Sets the current voice.

```typescript
setCurrentVoice(voiceId: string, source?: 'client' | 'server'): boolean
```

**Parameters:**
- `voiceId` (string) - Voice ID to set
- `source` (string, optional) - Source of change (default: 'client')

**Returns:** `true` if voice was changed successfully

**Special Voice IDs:**
- `'none'` - Text-only mode, no audio output
- `'avatar'` - Avatar mode, audio handled by HeyGen

**Example:**
```typescript
// Set specific voice
voiceManager.setCurrentVoice('nova');

// Switch to text-only mode
voiceManager.setCurrentVoice('none');

// Switch to avatar mode
voiceManager.setCurrentVoice('avatar');

// Check if voice exists before setting
if (voiceManager.getVoiceById('echo')) {
  voiceManager.setCurrentVoice('echo');
}
```

### setAvailableVoices()

Sets the list of available voices.

```typescript
setAvailableVoices(voices: Voice[]): void
```

**Parameters:**
- `voices` (Voice[]) - Array of available voices

**Note:** Usually called automatically from AuthManager

**Example:**
```typescript
const voices = [
  {
    voice_id: 'nova',
    name: 'Nova',
    vendor: 'openai',
    description: 'Natural and friendly',
    output_format: 'pcm16'
  },
  {
    voice_id: 'echo',
    name: 'Echo',
    vendor: 'openai',
    description: 'British accent',
    output_format: 'pcm16'
  }
];

voiceManager.setAvailableVoices(voices);
```

### getVoiceById()

Gets a specific voice by ID.

```typescript
getVoiceById(voiceId: string): Voice | undefined
```

**Parameters:**
- `voiceId` (string) - Voice ID to find

**Returns:** Voice object or undefined

**Example:**
```typescript
const nova = voiceManager.getVoiceById('nova');
if (nova) {
  console.log(`Nova voice: ${nova.description}`);
  console.log(`Format: ${nova.output_format}`);
}
```

### getVoicesByVendor()

Gets all voices from a specific vendor.

```typescript
getVoicesByVendor(vendor: string): Voice[]
```

**Parameters:**
- `vendor` (string) - Vendor name (e.g., 'openai')

**Returns:** Array of voices from vendor

**Example:**
```typescript
const openaiVoices = voiceManager.getVoicesByVendor('openai');
console.log(`OpenAI voices: ${openaiVoices.length}`);
openaiVoices.forEach(voice => {
  console.log(`  - ${voice.name}`);
});
```

### getAudioFormat()

Gets the audio format for the current voice.

```typescript
getAudioFormat(): string
```

**Returns:** Audio format string (e.g., 'pcm16')

**Example:**
```typescript
const format = voiceManager.getAudioFormat();
console.log(`Current audio format: ${format}`);

if (format === 'pcm16') {
  console.log('Using 16-bit PCM audio');
}
```

### supportsFormat()

Checks if a voice supports a specific audio format.

```typescript
supportsFormat(voiceId: string, format: string): boolean
```

**Parameters:**
- `voiceId` (string) - Voice ID to check
- `format` (string) - Audio format to check

**Returns:** `true` if format is supported

**Example:**
```typescript
if (voiceManager.supportsFormat('nova', 'pcm16')) {
  console.log('Nova supports PCM16 format');
}
```

### handleServerVoiceChange()

Handles voice change events from the server.

```typescript
handleServerVoiceChange(voiceId: string): void
```

**Parameters:**
- `voiceId` (string) - New voice ID from server

**Note:** Usually called internally by RealtimeClient

**Example:**
```typescript
// This is typically called internally
client.on('agent_voice_changed', (event) => {
  voiceManager.handleServerVoiceChange(event.voice_id);
});
```

### reset()

Resets to default voice.

```typescript
reset(): void
```

**Example:**
```typescript
// Reset to default voice (usually 'nova')
voiceManager.reset();
```

## Event Handling

VoiceManager extends EventEmitter and emits voice-related events:

### Events

- `voice-changed` - Voice selection changed
- `voices-updated` - Available voices list updated
- `voice-error` - Voice-related error occurred

### on()

Subscribe to voice events.

```typescript
on(event: string, handler: Function): void
```

**Example:**
```typescript
voiceManager.on('voice-changed', (event: VoiceChangeEvent) => {
  console.log(`Voice changed: ${event.previousVoice?.voice_id} → ${event.currentVoice?.voice_id}`);
  console.log(`Source: ${event.source}`);
  console.log(`Format: ${event.audioFormat}`);
});

voiceManager.on('voices-updated', (voices: Voice[]) => {
  console.log(`${voices.length} voices now available`);
  updateVoiceSelector(voices);
});

voiceManager.on('voice-error', (error: VoiceError) => {
  console.error(`Voice error: ${error.message}`);
  console.error(`Voice ID: ${error.voiceId}`);
});
```

### Event Types

```typescript
interface VoiceChangeEvent {
  previousVoice: Voice | null;
  currentVoice: Voice | null;
  source: 'client' | 'server';
  audioFormat: string;
}

interface VoiceError {
  message: string;
  voiceId?: string;
  code?: string;
}
```

## Voice Models

### Standard Voices

OpenAI TTS voices with PCM16 output:

```typescript
// Available OpenAI voices
const voices = [
  { voice_id: 'nova', name: 'Nova' },     // Natural, friendly
  { voice_id: 'echo', name: 'Echo' },     // British accent
  { voice_id: 'fable', name: 'Fable' },   // Storyteller
  { voice_id: 'onyx', name: 'Onyx' },     // Deep, authoritative
  { voice_id: 'alloy', name: 'Alloy' },   // Neutral
  { voice_id: 'shimmer', name: 'Shimmer' } // Warm, inviting
];
```

### Special Modes

```typescript
// Text-only mode (no audio)
voiceManager.setCurrentVoice('none');

// Avatar mode (audio handled by HeyGen)
voiceManager.setCurrentVoice('avatar');
```

## Integration with Audio Output

The VoiceManager automatically updates the AudioOutputService:

```typescript
// When voice changes, audio output is updated
voiceManager.on('voice-changed', (event) => {
  // AudioOutputService is automatically notified
  // It will adjust format handling based on new voice
});

// The audio format affects playback
const format = voiceManager.getAudioFormat();
if (format === 'pcm16') {
  // AudioOutputService expects 16-bit PCM data
  // Sample rate: 16000 Hz
}
```

## Complete Example

```typescript
import { RealtimeClient, AuthManager } from '@agentc/realtime-core';

async function voiceManagementExample() {
  // Setup authentication
  const authManager = new AuthManager({
    apiUrl: 'https://localhost:8000'
  });
  
  await authManager.login('username', 'password');
  
  // Create client
  const client = new RealtimeClient({
    apiUrl: 'wss://localhost:8000/rt/ws',
    authManager,
    enableAudio: true
  });
  
  // Get voice manager
  const voiceManager = client.getVoiceManager();
  
  if (!voiceManager) {
    console.error('Voice manager not initialized');
    return;
  }
  
  // Display available voices
  console.log('📢 Available Voices:');
  voiceManager.availableVoices.forEach(voice => {
    console.log(`  ${voice.name} (${voice.voice_id})`);
    console.log(`    Vendor: ${voice.vendor}`);
    console.log(`    Format: ${voice.output_format}`);
    if (voice.description) {
      console.log(`    Description: ${voice.description}`);
    }
  });
  
  // Subscribe to voice changes
  voiceManager.on('voice-changed', (event) => {
    console.log('\n🔊 Voice Changed:');
    if (event.previousVoice) {
      console.log(`  From: ${event.previousVoice.name}`);
    }
    if (event.currentVoice) {
      console.log(`  To: ${event.currentVoice.name}`);
      console.log(`  Format: ${event.audioFormat}`);
    } else {
      console.log(`  To: No voice (text-only)`);
    }
    console.log(`  Source: ${event.source}`);
    
    // Update UI based on voice mode
    updateVoiceUI(event.currentVoice);
  });
  
  voiceManager.on('voice-error', (error) => {
    console.error('❌ Voice Error:', error.message);
  });
  
  // Connect to service
  await client.connect();
  
  // Demo: Cycle through voices
  console.log('\n🎭 Voice Demo Starting...\n');
  
  // Test each voice
  const testVoices = ['nova', 'echo', 'fable', 'none', 'nova'];
  
  for (const voiceId of testVoices) {
    console.log(`\nSwitching to: ${voiceId}`);
    
    // Change voice on client
    const changed = voiceManager.setCurrentVoice(voiceId);
    
    if (changed) {
      // Also notify server
      client.setAgentVoice(voiceId);
      
      // Send test message
      if (voiceId === 'none') {
        client.sendText('This is text-only mode, no audio output.');
      } else {
        client.sendText(`Hello! This is the ${voiceId} voice speaking.`);
      }
      
      // Check current state
      console.log(`Current voice: ${voiceManager.currentVoiceId}`);
      console.log(`Is text-only: ${voiceManager.isTextOnly}`);
      console.log(`Is avatar mode: ${voiceManager.isAvatarMode}`);
      console.log(`Audio format: ${voiceManager.getAudioFormat()}`);
      
      // Wait for response
      await new Promise(resolve => setTimeout(resolve, 5000));
    } else {
      console.log(`Failed to switch to ${voiceId}`);
    }
  }
  
  // Get OpenAI voices specifically
  console.log('\n🤖 OpenAI Voices:');
  const openaiVoices = voiceManager.getVoicesByVendor('openai');
  openaiVoices.forEach(voice => {
    console.log(`  - ${voice.name}: ${voice.description || 'No description'}`);
  });
  
  // Create voice selector UI
  function createVoiceSelector() {
    const voices = voiceManager.availableVoices;
    
    // Add special options
    const options = [
      { value: 'none', label: 'Text Only (No Audio)' },
      ...voices.map(v => ({ value: v.voice_id, label: v.name }))
    ];
    
    // If avatars available, add avatar option
    const avatars = client.getAvailableAvatars();
    if (avatars.length > 0) {
      options.push({ value: 'avatar', label: 'Avatar Mode' });
    }
    
    return options;
  }
  
  const voiceOptions = createVoiceSelector();
  console.log('\n📋 Voice Selector Options:');
  voiceOptions.forEach(opt => {
    console.log(`  ${opt.value}: ${opt.label}`);
  });
  
  function updateVoiceUI(voice: Voice | null) {
    if (!voice) {
      console.log('UI: Text-only mode enabled');
      // Hide audio indicators
    } else if (voice.voice_id === 'avatar') {
      console.log('UI: Avatar mode enabled');
      // Show avatar video element
    } else {
      console.log(`UI: Voice "${voice.name}" selected`);
      // Show audio visualizer
    }
  }
}

voiceManagementExample().catch(console.error);
```

## Best Practices

1. **Check voice availability before setting:**
```typescript
const voiceId = 'echo';
if (voiceManager.getVoiceById(voiceId)) {
  voiceManager.setCurrentVoice(voiceId);
} else {
  console.error(`Voice ${voiceId} not available`);
  // Use default or show error
}
```

2. **Handle special modes properly:**
```typescript
// Before playing audio
if (voiceManager.isTextOnly) {
  // Don't initialize audio output
  return;
}

if (voiceManager.isAvatarMode) {
  // Let HeyGen handle audio
  return;
}

// Normal audio output
playAudio(audioData);
```

3. **Sync client and server voice:**
```typescript
// When user selects voice
function onVoiceSelect(voiceId: string) {
  // Update local state
  voiceManager.setCurrentVoice(voiceId, 'client');
  
  // Notify server
  client.setAgentVoice(voiceId);
}
```

4. **Provide voice descriptions to users:**
```typescript
function getVoiceDescription(voiceId: string): string {
  const voice = voiceManager.getVoiceById(voiceId);
  if (!voice) return 'Unknown voice';
  
  return voice.description || voice.name;
}
```

5. **Handle voice errors gracefully:**
```typescript
voiceManager.on('voice-error', (error) => {
  // Fallback to default
  voiceManager.reset();
  
  // Notify user
  showNotification(`Voice error: ${error.message}`);
});
```

## Voice Format Compatibility

Different voices may support different audio formats:

```typescript
// Check format compatibility
const voice = voiceManager.getVoiceById('nova');
if (voice?.output_format === 'pcm16') {
  // Configure audio output for PCM16
  // Sample rate: 16000 Hz
  // Bit depth: 16 bits
  // Channels: 1 (mono)
}

// Future voices might support other formats
if (voice?.output_format === 'mp3') {
  // Would need different audio handling
}
```

## Troubleshooting

### Common Issues

**Voice changes not taking effect:**
- Ensure both client and server are updated
- Check that voice ID is valid
- Verify audio system is initialized

**No audio output after voice change:**
- Check if switched to 'none' (text-only)
- Verify voice supports current audio format
- Check AudioOutputService is running

**Avatar voice not working:**
- Ensure HeyGen session is active
- Check avatar session was set with `client.setAvatarSession()`
- Verify HeyGen token is valid

**Voice list is empty:**
- Check AuthManager has logged in successfully
- Verify API response includes voices
- Check network requests for errors

## TypeScript Types

```typescript
import {
  VoiceManager,
  VoiceManagerConfig,
  Voice,
  VoiceChangeEvent,
  VoiceError
} from '@agentc/realtime-core';
```

All methods and properties are fully typed for TypeScript applications.