# Voice Models Guide

This guide covers working with text-to-speech (TTS) voices in the Agent C Realtime SDK, including voice selection, configuration, and special modes.

## Available Voices

Agent C supports multiple TTS providers and voice models:

### OpenAI Voices

```typescript
const openAIVoices = [
  {
    voice_id: 'nova',
    name: 'Nova',
    description: 'Natural and friendly female voice',
    gender: 'female',
    accent: 'American'
  },
  {
    voice_id: 'echo', 
    name: 'Echo',
    description: 'British-accented male voice',
    gender: 'male',
    accent: 'British'
  },
  {
    voice_id: 'fable',
    name: 'Fable',
    description: 'Expressive storyteller voice',
    gender: 'neutral',
    style: 'narrative'
  },
  {
    voice_id: 'onyx',
    name: 'Onyx', 
    description: 'Deep, authoritative male voice',
    gender: 'male',
    style: 'professional'
  },
  {
    voice_id: 'alloy',
    name: 'Alloy',
    description: 'Neutral, balanced voice',
    gender: 'neutral',
    style: 'neutral'
  },
  {
    voice_id: 'shimmer',
    name: 'Shimmer',
    description: 'Warm, inviting female voice',
    gender: 'female',
    style: 'friendly'
  }
];
```

### Special Voice Modes

```typescript
// Text-only mode (no audio output)
client.setAgentVoice('none');

// Avatar mode (audio handled by HeyGen)
client.setAgentVoice('avatar');
```

## Basic Voice Selection

### Setting Voice on Client

```typescript
import { RealtimeClient, AuthManager } from '@agentc/realtime-core';

// First authenticate with Agent C
const authManager = new AuthManager({
  apiUrl: 'https://localhost:8000'
});

// Login with username/password
const loginResponse = await authManager.login('username', 'password');

// Create client with WebSocket URL from login response
const client = new RealtimeClient({
  apiUrl: loginResponse.websocketUrl,  // URL provided by login
  authManager
});

await client.connect();

// Set voice
client.setAgentVoice('nova');

// Voice manager also updates
const voiceManager = client.getVoiceManager();
const currentVoice = voiceManager?.getCurrentVoice();
console.log('Current voice:', currentVoice?.name);
```

### React Voice Selection

```tsx
import { useVoiceModel } from '@agentc/realtime-react';

function VoiceSelector() {
  const { currentVoice, availableVoices, setVoice } = useVoiceModel();
  
  return (
    <select 
      value={currentVoice?.voice_id || 'none'}
      onChange={(e) => setVoice(e.target.value)}
    >
      <option value="none">Text Only</option>
      {availableVoices.map(voice => (
        <option key={voice.voice_id} value={voice.voice_id}>
          {voice.name} - {voice.description}
        </option>
      ))}
      <option value="avatar">Avatar Mode</option>
    </select>
  );
}
```

## Voice Configuration

### Voice Properties

```typescript
interface Voice {
  voice_id: string;          // Unique identifier
  name: string;              // Display name
  vendor: string;            // Provider (e.g., 'openai')
  description?: string;      // Voice description
  output_format?: string;    // Audio format (e.g., 'pcm16')
  sample_rate?: number;      // Sample rate in Hz
  gender?: string;           // Voice gender
  accent?: string;           // Accent type
  style?: string;            // Speaking style
  languages?: string[];      // Supported languages
}
```

### Getting Voice Information

```typescript
const voiceManager = client.getVoiceManager();

// Get current voice
const current = voiceManager.getCurrentVoice();
console.log('Voice:', current?.name);
console.log('Format:', current?.output_format);
console.log('Vendor:', current?.vendor);

// Get all available voices
const voices = voiceManager.getAvailableVoices();
voices.forEach(voice => {
  console.log(`${voice.name} (${voice.voice_id}): ${voice.description}`);
});

// Get voices by vendor
const openaiVoices = voiceManager.getVoicesByVendor('openai');
console.log(`OpenAI voices: ${openaiVoices.length}`);

// Check voice properties
const nova = voiceManager.getVoiceById('nova');
if (nova) {
  console.log('Nova properties:', nova);
}
```

## Voice Modes

### Text-Only Mode

No audio output, only text responses:

```typescript
// Enable text-only mode
client.setAgentVoice('none');

// Check if in text-only mode
const voiceManager = client.getVoiceManager();
if (voiceManager.isTextOnly) {
  console.log('Text-only mode active');
  // Hide audio UI elements
  document.getElementById('audio-controls').style.display = 'none';
}

// Handle text-only responses
client.on('text_delta', (event) => {
  // Display text without expecting audio
  displayText(event.content);
});
```

### Avatar Mode

Audio handled by HeyGen avatar system:

```typescript
// Switch to avatar mode
client.setAgentVoice('avatar');

// Check if in avatar mode
if (voiceManager.isAvatarMode) {
  console.log('Avatar mode active');
  // Audio will be handled by HeyGen
  // Don't expect audio:output events
}

// Avatar mode is typically set automatically
client.setAvatarSession(heygenSessionId, avatarId);
// Voice automatically switches to 'avatar'
```

### Standard Voice Mode

Regular TTS with audio output:

```typescript
// Set a standard voice
client.setAgentVoice('nova');

// Handle audio output
client.on('audio:output', (audioData) => {
  // Audio will be played automatically
  console.log('Received audio:', audioData.byteLength, 'bytes');
});

// Monitor voice changes
voiceManager.on('voice-changed', (event) => {
  console.log('Voice changed to:', event.currentVoice?.name);
  console.log('Audio format:', event.audioFormat);
});
```

## Voice Selection UI

### Basic Voice Dropdown

```typescript
function createVoiceSelector(voiceManager: VoiceManager) {
  const select = document.createElement('select');
  
  // Add special options
  const noneOption = document.createElement('option');
  noneOption.value = 'none';
  noneOption.textContent = '📝 Text Only (No Audio)';
  select.appendChild(noneOption);
  
  // Add available voices
  voiceManager.getAvailableVoices().forEach(voice => {
    const option = document.createElement('option');
    option.value = voice.voice_id;
    option.textContent = `🔊 ${voice.name} - ${voice.description}`;
    select.appendChild(option);
  });
  
  // Add avatar option if available
  if (client.getAvailableAvatars().length > 0) {
    const avatarOption = document.createElement('option');
    avatarOption.value = 'avatar';
    avatarOption.textContent = '🎭 Avatar Mode';
    select.appendChild(avatarOption);
  }
  
  // Handle selection
  select.addEventListener('change', (e) => {
    const voiceId = (e.target as HTMLSelectElement).value;
    client.setAgentVoice(voiceId);
  });
  
  // Set current value
  select.value = voiceManager.getCurrentVoice()?.voice_id || 'none';
  
  return select;
}
```

### Advanced Voice Selector

```tsx
function AdvancedVoiceSelector() {
  const { currentVoice, availableVoices, setVoice } = useVoiceModel();
  const [filter, setFilter] = useState<'all' | 'male' | 'female' | 'neutral'>('all');
  const [previewVoice, setPreviewVoice] = useState<string | null>(null);
  
  const filteredVoices = availableVoices.filter(voice => {
    if (filter === 'all') return true;
    return voice.gender === filter;
  });
  
  const handlePreview = async (voiceId: string) => {
    setPreviewVoice(voiceId);
    
    // Temporarily switch voice
    const originalVoice = currentVoice?.voice_id;
    setVoice(voiceId);
    
    // Send preview message
    client.sendText('Hello! This is a preview of my voice.');
    
    // Switch back after delay
    setTimeout(() => {
      if (originalVoice) setVoice(originalVoice);
      setPreviewVoice(null);
    }, 5000);
  };
  
  return (
    <div className="voice-selector-advanced">
      <div className="filter-buttons">
        <button onClick={() => setFilter('all')} className={filter === 'all' ? 'active' : ''}>
          All
        </button>
        <button onClick={() => setFilter('male')} className={filter === 'male' ? 'active' : ''}>
          Male
        </button>
        <button onClick={() => setFilter('female')} className={filter === 'female' ? 'active' : ''}>
          Female
        </button>
        <button onClick={() => setFilter('neutral')} className={filter === 'neutral' ? 'active' : ''}>
          Neutral
        </button>
      </div>
      
      <div className="voice-grid">
        <div 
          className={`voice-card ${!currentVoice ? 'selected' : ''}`}
          onClick={() => setVoice('none')}
        >
          <div className="voice-icon">📝</div>
          <div className="voice-name">Text Only</div>
          <div className="voice-desc">No audio output</div>
        </div>
        
        {filteredVoices.map(voice => (
          <div 
            key={voice.voice_id}
            className={`voice-card ${currentVoice?.voice_id === voice.voice_id ? 'selected' : ''}`}
            onClick={() => setVoice(voice.voice_id)}
          >
            <div className="voice-icon">
              {voice.gender === 'male' ? '👨' : voice.gender === 'female' ? '👩' : '🧑'}
            </div>
            <div className="voice-name">{voice.name}</div>
            <div className="voice-vendor">{voice.vendor}</div>
            <div className="voice-desc">{voice.description}</div>
            <div className="voice-tags">
              {voice.accent && <span className="tag">{voice.accent}</span>}
              {voice.style && <span className="tag">{voice.style}</span>}
            </div>
            <button 
              className="preview-btn"
              onClick={(e) => {
                e.stopPropagation();
                handlePreview(voice.voice_id);
              }}
              disabled={previewVoice === voice.voice_id}
            >
              {previewVoice === voice.voice_id ? 'Playing...' : 'Preview'}
            </button>
          </div>
        ))}
      </div>
      
      <div className="current-voice-info">
        Current Voice: {currentVoice?.name || 'Text Only'}
      </div>
    </div>
  );
}
```

## Voice Customization

### Voice Parameters (Future)

```typescript
// Future API for voice customization
interface VoiceParameters {
  speed?: number;        // 0.5 to 2.0 (1.0 = normal)
  pitch?: number;        // -20 to 20 semitones
  volume?: number;       // 0 to 1
  emphasis?: number;     // 0 to 1
  emotion?: string;      // 'neutral', 'happy', 'sad', etc.
}

// Example usage (future)
client.setVoiceParameters({
  speed: 1.1,          // Slightly faster
  pitch: 2,            // Slightly higher
  emphasis: 0.7        // Moderate emphasis
});
```

### Language Support

```typescript
// Check language support
function getVoicesForLanguage(language: string): Voice[] {
  const voiceManager = client.getVoiceManager();
  const voices = voiceManager.getAvailableVoices();
  
  return voices.filter(voice => 
    voice.languages?.includes(language)
  );
}

// Get voices that support Spanish
const spanishVoices = getVoicesForLanguage('es');
console.log('Spanish voices:', spanishVoices);

// Switch to a multilingual voice
const multilingualVoice = voices.find(v => 
  v.languages && v.languages.length > 1
);
if (multilingualVoice) {
  client.setAgentVoice(multilingualVoice.voice_id);
}
```

## Voice Events

### Monitoring Voice Changes

```typescript
const voiceManager = client.getVoiceManager();

// Voice changed event
voiceManager.on('voice-changed', (event) => {
  console.log('Voice Changed Event:');
  console.log('  Previous:', event.previousVoice?.name);
  console.log('  Current:', event.currentVoice?.name);
  console.log('  Source:', event.source); // 'client' or 'server'
  console.log('  Format:', event.audioFormat);
  
  // Update UI based on new voice
  updateVoiceUI(event.currentVoice);
});

// Voice list updated
voiceManager.on('voices-updated', (voices) => {
  console.log('Available voices updated:', voices.length);
  
  // Refresh voice selector
  refreshVoiceSelector(voices);
});

// Voice error
voiceManager.on('voice-error', (error) => {
  console.error('Voice error:', error.message);
  
  if (error.code === 'VOICE_NOT_FOUND') {
    // Fallback to default voice
    client.setAgentVoice('nova');
  }
});
```

### Server Voice Changes

```typescript
// Server can change voice
client.on('agent_voice_changed', (event) => {
  console.log('Server changed voice to:', event.voice_id);
  
  // VoiceManager automatically updates
  // UI will receive voice-changed event
});
```

## Voice Best Practices

### 1. Provide Voice Descriptions

```typescript
function VoiceCard({ voice }: { voice: Voice }) {
  return (
    <div className="voice-card">
      <h3>{voice.name}</h3>
      <p className="description">{voice.description}</p>
      <div className="properties">
        {voice.gender && <span>Gender: {voice.gender}</span>}
        {voice.accent && <span>Accent: {voice.accent}</span>}
        {voice.style && <span>Style: {voice.style}</span>}
      </div>
      <audio controls src={`/voice-samples/${voice.voice_id}.mp3`}>
        Your browser does not support audio.
      </audio>
    </div>
  );
}
```

### 2. Remember User Preference

```typescript
class VoicePreference {
  private storageKey = 'preferred_voice';
  
  save(voiceId: string) {
    localStorage.setItem(this.storageKey, voiceId);
  }
  
  load(): string | null {
    return localStorage.getItem(this.storageKey);
  }
  
  apply(client: RealtimeClient) {
    const preferred = this.load();
    if (preferred) {
      const voiceManager = client.getVoiceManager();
      
      // Check if voice still exists
      if (voiceManager?.getVoiceById(preferred)) {
        client.setAgentVoice(preferred);
      } else {
        // Voice no longer available, clear preference
        localStorage.removeItem(this.storageKey);
      }
    }
  }
}

// Use on connection
const voicePref = new VoicePreference();
client.on('connected', () => {
  voicePref.apply(client);
});

// Save on change
voiceManager.on('voice-changed', (event) => {
  if (event.source === 'client' && event.currentVoice) {
    voicePref.save(event.currentVoice.voice_id);
  }
});
```

### 3. Handle Voice Availability

```typescript
// Check voice availability before setting
function setVoiceSafely(client: RealtimeClient, voiceId: string) {
  const voiceManager = client.getVoiceManager();
  
  if (voiceId === 'none' || voiceId === 'avatar') {
    // Special modes always available
    client.setAgentVoice(voiceId);
    return true;
  }
  
  const voice = voiceManager?.getVoiceById(voiceId);
  if (voice) {
    client.setAgentVoice(voiceId);
    return true;
  } else {
    console.warn(`Voice ${voiceId} not available`);
    
    // Fallback to default
    const defaultVoice = voiceManager?.getAvailableVoices()[0];
    if (defaultVoice) {
      client.setAgentVoice(defaultVoice.voice_id);
    } else {
      // No voices available, use text-only
      client.setAgentVoice('none');
    }
    return false;
  }
}
```

### 4. Voice-Specific UI

```typescript
function VoiceAwareUI() {
  const { currentVoice, isTextOnly, isAvatarMode } = useVoiceModel();
  
  return (
    <div className="voice-aware-ui">
      {isTextOnly && (
        <div className="text-only-notice">
          📝 Text-only mode - No audio will be played
        </div>
      )}
      
      {isAvatarMode && (
        <div className="avatar-mode-notice">
          🎭 Avatar mode - Audio handled by avatar
        </div>
      )}
      
      {!isTextOnly && !isAvatarMode && (
        <div className="voice-info">
          🔊 Voice: {currentVoice?.name}
          <VolumeControl />
          <AudioVisualizer />
        </div>
      )}
    </div>
  );
}
```

### 5. Voice Testing

```typescript
class VoiceTester {
  private testPhrases = [
    "Hello! This is a test of the voice system.",
    "The quick brown fox jumps over the lazy dog.",
    "Can you hear me clearly?",
    "Testing one, two, three."
  ];
  
  async testVoice(client: RealtimeClient, voiceId: string) {
    // Save current voice
    const voiceManager = client.getVoiceManager();
    const originalVoice = voiceManager?.getCurrentVoice();
    
    try {
      // Switch to test voice
      client.setAgentVoice(voiceId);
      
      // Send test phrase
      const phrase = this.testPhrases[Math.floor(Math.random() * this.testPhrases.length)];
      client.sendText(phrase);
      
      // Wait for response
      await new Promise(resolve => setTimeout(resolve, 5000));
      
      return true;
    } catch (error) {
      console.error('Voice test failed:', error);
      return false;
    } finally {
      // Restore original voice
      if (originalVoice) {
        client.setAgentVoice(originalVoice.voice_id);
      }
    }
  }
  
  async testAllVoices(client: RealtimeClient) {
    const voiceManager = client.getVoiceManager();
    const voices = voiceManager?.getAvailableVoices() || [];
    
    const results = new Map<string, boolean>();
    
    for (const voice of voices) {
      console.log(`Testing ${voice.name}...`);
      const success = await this.testVoice(client, voice.voice_id);
      results.set(voice.voice_id, success);
    }
    
    return results;
  }
}
```

## Voice Performance

### Audio Format Optimization

```typescript
// Different voices may use different formats
const voiceFormats = {
  'nova': { format: 'pcm16', sampleRate: 16000, bitDepth: 16 },
  'echo': { format: 'pcm16', sampleRate: 16000, bitDepth: 16 },
  // Future voices might use different formats
  'hd-voice': { format: 'opus', sampleRate: 48000, bitRate: 64000 }
};

// Adjust audio handling based on format
function configureAudioForVoice(voice: Voice) {
  const format = voice.output_format || 'pcm16';
  
  switch (format) {
    case 'pcm16':
      // Standard PCM16 handling
      return {
        decoder: pcm16Decoder,
        sampleRate: 16000
      };
    
    case 'opus':
      // Would need Opus decoder
      return {
        decoder: opusDecoder,
        sampleRate: 48000
      };
    
    default:
      console.warn(`Unknown format: ${format}`);
      return null;
  }
}
```

### Voice Caching

```typescript
class VoiceCache {
  private cache = new Map<string, AudioBuffer[]>();
  private maxCacheSize = 50; // Cache 50 phrases per voice
  
  cacheAudio(voiceId: string, text: string, audioBuffer: AudioBuffer) {
    const key = `${voiceId}:${text}`;
    
    if (!this.cache.has(key)) {
      this.cache.set(key, []);
    }
    
    const cached = this.cache.get(key)!;
    cached.push(audioBuffer);
    
    // Limit cache size
    if (cached.length > this.maxCacheSize) {
      cached.shift();
    }
  }
  
  getCached(voiceId: string, text: string): AudioBuffer | null {
    const key = `${voiceId}:${text}`;
    const cached = this.cache.get(key);
    
    return cached && cached.length > 0 ? cached[0] : null;
  }
  
  clearCache(voiceId?: string) {
    if (voiceId) {
      // Clear specific voice
      for (const [key] of this.cache) {
        if (key.startsWith(`${voiceId}:`)) {
          this.cache.delete(key);
        }
      }
    } else {
      // Clear all
      this.cache.clear();
    }
  }
}
```

## Troubleshooting

### Voice Not Working

```typescript
function debugVoice(client: RealtimeClient) {
  const voiceManager = client.getVoiceManager();
  
  console.log('=== Voice Debug ===');
  console.log('Current voice:', voiceManager?.getCurrentVoice());
  console.log('Is text-only:', voiceManager?.isTextOnly);
  console.log('Is avatar mode:', voiceManager?.isAvatarMode);
  console.log('Available voices:', voiceManager?.getAvailableVoices().length);
  console.log('Audio format:', voiceManager?.getAudioFormat());
  
  // Check audio output
  const audioOutput = AudioOutputService.getInstance();
  const status = audioOutput.getStatus();
  console.log('Audio output status:', status);
  
  // Test voice
  console.log('Sending test message...');
  client.sendText('Testing voice output');
}
```

### Voice Quality Issues

```typescript
// Monitor voice quality
class VoiceQualityMonitor {
  private metrics = {
    packetsReceived: 0,
    packetsLost: 0,
    jitter: 0,
    latency: 0
  };
  
  onAudioReceived(audioData: ArrayBuffer) {
    this.metrics.packetsReceived++;
    
    // Check for gaps in audio
    if (this.detectGap(audioData)) {
      this.metrics.packetsLost++;
    }
    
    // Measure jitter
    this.metrics.jitter = this.calculateJitter();
  }
  
  getQualityScore(): number {
    const lossRate = this.metrics.packetsLost / this.metrics.packetsReceived;
    const jitterScore = Math.max(0, 1 - (this.metrics.jitter / 100));
    const latencyScore = Math.max(0, 1 - (this.metrics.latency / 500));
    
    return (1 - lossRate) * 0.5 + jitterScore * 0.3 + latencyScore * 0.2;
  }
  
  private detectGap(audioData: ArrayBuffer): boolean {
    // Implementation to detect audio gaps
    return false;
  }
  
  private calculateJitter(): number {
    // Implementation to calculate jitter
    return 0;
  }
}
```

## Summary

Voice models in Agent C provide:

1. **Multiple TTS options** with different characteristics
2. **Special modes** for text-only and avatar scenarios
3. **Flexible selection** through UI or API
4. **Voice customization** (future features)
5. **Performance optimization** for different formats

Choose voices based on your application needs, user preferences, and performance requirements.