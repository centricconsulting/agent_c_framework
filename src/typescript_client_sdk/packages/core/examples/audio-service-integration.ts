/**
 * Example: AudioService Integration with Binary Frame Transmission
 * Shows how to connect the AudioService to RealtimeClient for real-time voice
 */

import { RealtimeClient } from '../src/client/RealtimeClient';
import { AudioService } from '../src/audio/AudioService';
import { AudioChunkData } from '../src/audio/types';

async function setupVoiceChat() {
  // 1. Initialize the Realtime Client
  const client = new RealtimeClient({
    apiUrl: 'wss://api.agentc.ai/rt/ws',
    authToken: 'your_jwt_token',
    debug: true
  });

  // 2. Initialize the Audio Service
  const audioService = AudioService.getInstance({
    sampleRate: 16000,      // 16kHz for voice
    channelCount: 1,        // Mono
    chunkDuration: 100,     // 100ms chunks
    debug: true
  });

  // 3. Connect to the server
  await client.connect();
  console.log('✓ Connected to Agent C Realtime API');

  // 4. Request microphone permission
  const hasPermission = await audioService.requestPermission();
  if (!hasPermission) {
    console.error('❌ Microphone permission denied');
    return;
  }
  console.log('✓ Microphone permission granted');

  // 5. Connect AudioService to RealtimeClient
  // This is the KEY integration - raw binary audio from the worklet
  // is sent directly to the server without any JSON wrapping
  const unsubscribe = audioService.onAudioChunk((chunk: AudioChunkData) => {
    // CORRECT: Send the raw PCM16 audio buffer directly
    // No base64 encoding, no JSON wrapping - just binary
    client.sendBinaryFrame(chunk.buffer);
    
    // Optional: Monitor audio levels
    if (chunk.audioLevel > 0.1) {
      console.log(`🎤 Speaking... (level: ${chunk.audioLevel.toFixed(2)})`);
    }
  });

  // 6. Handle incoming audio from server (TTS)
  client.on('audio:output', (audioData: ArrayBuffer) => {
    console.log(`🔊 Received TTS audio: ${audioData.byteLength} bytes`);
    // In a real app, you would play this audio:
    // audioPlayer.playPCM16(audioData);
  });

  // 7. Handle text responses
  client.on('text_delta', (event) => {
    console.log('💬 Agent:', event.content);
  });

  // 8. Handle turn management (optional)
  client.on('user_turn_start', () => {
    console.log('🟢 Your turn to speak');
    audioService.startRecording();
  });

  client.on('user_turn_end', () => {
    console.log('🔴 Agent is responding');
    audioService.stopRecording();
  });

  // 9. Start recording
  await audioService.startRecording();
  console.log('🎤 Recording started - speak now!');

  // Example conversation flow
  setTimeout(() => {
    // Send a text message (this IS sent as JSON)
    client.sendText("Hello! Can you hear me?");
  }, 2000);

  // Clean up after 30 seconds
  setTimeout(async () => {
    console.log('\n--- Cleaning up ---');
    
    // Stop recording
    audioService.stopRecording();
    
    // Unsubscribe from audio chunks
    unsubscribe();
    
    // Disconnect from server
    client.disconnect();
    
    console.log('✓ Disconnected');
  }, 30000);
}

// Alternative: Manual audio control without turn management
async function manualAudioControl() {
  const client = new RealtimeClient({
    apiUrl: 'wss://api.agentc.ai/rt/ws',
    authToken: 'your_jwt_token',
    enableTurnManager: false,  // Disable automatic turn management
    debug: true
  });

  const audioService = AudioService.getInstance();

  await client.connect();
  await audioService.requestPermission();

  // Manual control - you decide when to send audio
  let isRecording = false;
  let audioSubscription: (() => void) | null = null;

  // Toggle recording with a button press (simulated here)
  const toggleRecording = async () => {
    if (isRecording) {
      // Stop recording
      audioService.stopRecording();
      if (audioSubscription) {
        audioSubscription();
        audioSubscription = null;
      }
      console.log('⏹️ Stopped recording');
    } else {
      // Start recording and streaming
      audioSubscription = audioService.onAudioChunk((chunk) => {
        // Send raw binary audio to server
        client.sendBinaryFrame(chunk.buffer);
      });
      await audioService.startRecording();
      console.log('⏺️ Started recording');
    }
    isRecording = !isRecording;
  };

  // Example: Toggle recording every 5 seconds
  setInterval(toggleRecording, 5000);

  // Handle incoming audio
  client.on('audio:output', (audioData: ArrayBuffer) => {
    console.log(`Received audio response: ${audioData.byteLength} bytes`);
    // Play the audio...
  });
}

// WRONG EXAMPLE - What NOT to do
function wrongWayExample() {
  // ❌ DON'T wrap audio in JSON events
  /*
  audioService.onAudioChunk((chunk) => {
    // WRONG: Converting to base64 and wrapping in JSON
    const base64Audio = btoa(String.fromCharCode(...new Uint8Array(chunk.buffer)));
    client.send({
      type: 'audio_input_delta',
      audio: base64Audio
    });
  });
  */

  // ❌ DON'T try to parse binary audio as JSON
  /*
  client.on('binary_audio', (data) => {
    // WRONG: Binary audio is not JSON
    const event = JSON.parse(data); // This will fail!
  });
  */

  console.log('These examples show what NOT to do');
}

// Run the example
setupVoiceChat().catch(console.error);