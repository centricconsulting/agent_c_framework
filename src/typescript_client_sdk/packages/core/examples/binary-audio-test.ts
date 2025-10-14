/**
 * Example: Binary Audio Frame Handling
 * Demonstrates the correct way to send and receive binary audio
 */

import { RealtimeClient } from '../src/client/RealtimeClient';

async function testBinaryAudioTransmission() {
  // Create client with proper configuration
  const client = new RealtimeClient({
    apiUrl: 'wss://api.example.com/rt/ws',
    authToken: 'your_jwt_token',
    binaryType: 'arraybuffer',  // Ensure binary support
    debug: true
  });

  // Connect to the server
  await client.connect();
  console.log('Connected to Agent C Realtime API');

  // Listen for binary audio output from server (TTS)
  client.on('audio:output', (audioData: ArrayBuffer) => {
    console.log(`Received TTS audio: ${audioData.byteLength} bytes`);
    // In a real app, you would play this audio:
    // audioPlayer.playPCM16(audioData);
  });

  // Example 1: Send binary audio (microphone input simulation)
  // This would typically come from your AudioWorklet
  const simulateMicrophoneInput = () => {
    // Create a buffer of PCM16 audio data (silence in this example)
    const sampleRate = 16000;  // 16kHz
    const durationMs = 100;    // 100ms chunks
    const numSamples = (sampleRate * durationMs) / 1000;
    const audioBuffer = new ArrayBuffer(numSamples * 2); // 2 bytes per PCM16 sample
    
    // Fill with silence (zeros) for this example
    const view = new Int16Array(audioBuffer);
    for (let i = 0; i < view.length; i++) {
      view[i] = 0;  // In real use, this would be actual audio samples
    }

    // CORRECT: Send raw binary audio directly
    client.sendBinaryFrame(audioBuffer);
    console.log(`Sent audio chunk: ${audioBuffer.byteLength} bytes`);
  };

  // Example 2: Wrong way (for comparison - DON'T DO THIS)
  const wrongWayExample = () => {
    // ❌ WRONG: Don't wrap audio in JSON events
    // This adds unnecessary overhead and complexity
    /*
    const audioBuffer = new ArrayBuffer(1024);
    const base64Audio = btoa(String.fromCharCode(...new Uint8Array(audioBuffer)));
    client.send({
      type: 'audio_input_delta',
      audio: base64Audio
    });
    */
    console.log('Note: Audio should NOT be sent as JSON events');
  };

  // Example 3: Send control messages (these ARE JSON)
  const sendControlMessages = () => {
    // Text input - sent as JSON
    client.sendText("Hello, can you hear me?");
    
    // Voice change - sent as JSON
    client.setAgentVoice("openai-nova");
    
    console.log('Control messages sent as JSON events');
  };

  // Simulate sending audio chunks
  console.log('\n--- Starting audio transmission test ---');
  
  // Send a few audio chunks
  for (let i = 0; i < 5; i++) {
    simulateMicrophoneInput();
    await new Promise(resolve => setTimeout(resolve, 100));
  }

  // Send some control messages
  sendControlMessages();

  // Listen for text responses
  client.on('text_delta', (event) => {
    console.log('Agent response:', event.content);
  });

  // Clean up after test
  setTimeout(() => {
    client.disconnect();
    console.log('\n--- Test completed ---');
  }, 5000);
}

// Run the test
testBinaryAudioTransmission().catch(console.error);