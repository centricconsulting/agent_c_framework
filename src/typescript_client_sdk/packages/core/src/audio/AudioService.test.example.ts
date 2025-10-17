/**
 * Example test/usage file for AudioService
 * This demonstrates the expected API usage patterns
 */

import { AudioService } from './AudioService';
import type { AudioServiceStatus, AudioChunkData } from './types';

async function testAudioServiceAPI() {
  // console.log('Testing AudioService API...\n');
  
  // Get singleton instance
  const audioService = AudioService.getInstance({ debug: true });
  
  // Test 1: Permission handling
  // console.log('1. Testing permission request:');
  // const hasPermission = await audioService.requestPermission();
  // console.log(`   Permission granted: ${hasPermission}`);
  // console.log(`   Has permission check: ${audioService.hasPermission()}`);
  
  // Test 2: Status monitoring
  // console.log('\n2. Setting up status monitoring:');
  const unsubscribeStatus = audioService.onStatusChange((_status: AudioServiceStatus) => {
    // console.log(`   Status changed:`, {
    //   state: status.state,
    //   isRecording: status.isRecording,
    //   audioLevel: status.audioLevel.toFixed(2),
    //   frameCount: status.frameCount
    // });
  });
  
  // Test 3: Audio chunk subscription
  // console.log('\n3. Setting up audio chunk subscription:');
  let chunkCount = 0;
  const unsubscribeChunks = audioService.onAudioChunk((_chunk: AudioChunkData) => {
    chunkCount++;
    if (chunkCount <= 3) {
      // console.log(`   Received chunk #${chunk.frame_count}:`, {
      //   size: chunk.content.byteLength,
      //   level: chunk.audio_level.toFixed(2),
      //   sampleRate: chunk.sample_rate
      // });
    }
  });
  
  // Test 4: Recording control
  // console.log('\n4. Testing recording control:');
  
  try {
    // Start recording
    // console.log('   Starting recording...');
    await audioService.startRecording();
    
    // Check current status
    // const status = audioService.getStatus();
    // console.log('   Current status:', {
    //   state: status.state,
    //   isRecording: status.isRecording
    // });
    
    // Let it run for 2 seconds
    await new Promise(resolve => setTimeout(resolve, 2000));
    
    // Stop recording
    // console.log('   Stopping recording...');
    audioService.stopRecording();
    
    // Check final status
    // const finalStatus = audioService.getStatus();
    // console.log('   Final status:', {
    //   state: finalStatus.state,
    //   isRecording: finalStatus.isRecording,
    //   totalFrames: finalStatus.frameCount
    // });
    
  } catch (error) {
    // console.error('   Recording error:', error);
  }
  
  // Test 5: Cleanup
  // console.log('\n5. Cleaning up subscriptions:');
  unsubscribeStatus();
  unsubscribeChunks();
  // console.log('   Subscriptions cleaned up');
  
  // Test 6: Singleton behavior
  // console.log('\n6. Testing singleton behavior:');
  // const instance2 = AudioService.getInstance();
  // console.log(`   Same instance: ${audioService === instance2}`);
  
  // console.log('\nAudioService API test complete!');
}

// Example usage patterns for documentation

// Pattern 1: Basic recording with permission check
async function basicRecordingExample() {
  const audioService = AudioService.getInstance();
  
  // Request permission first
  const hasPermission = await audioService.requestPermission();
  if (!hasPermission) {
    // console.error('Microphone permission denied');
    return;
  }
  
  // Start recording
  await audioService.startRecording();
  
  // ... do something with audio ...
  
  // Stop recording
  audioService.stopRecording();
}

// Pattern 2: With status monitoring
function recordingWithStatusMonitoring() {
  const audioService = AudioService.getInstance();
  
  // Monitor status changes
  const unsubscribe = audioService.onStatusChange((status) => {
    switch (status.state) {
      case 'idle':
        // console.log('Ready to record');
        break;
      case 'initializing':
        // console.log('Setting up audio...');
        break;
      case 'ready':
        // console.log('Audio initialized');
        break;
      case 'recording':
        // console.log('Recording in progress');
        break;
      case 'failed':
        // console.error('Audio failed:', status.error);
        break;
      case 'permission-denied':
        // console.error('Permission denied');
        break;
    }
  });
  
  // Remember to clean up
  return unsubscribe;
}

// Pattern 3: Processing audio chunks
function processAudioChunks() {
  const audioService = AudioService.getInstance();
  
  const unsubscribe = audioService.onAudioChunk((_chunk) => {
    // chunk.content is ArrayBuffer with PCM16 audio data
    // console.log('Audio chunk received:', {
    //   size: chunk.content.byteLength,
    //   audioLevel: chunk.audio_level,
    //   frameNumber: chunk.frame_count
    // });
    
    // Send to server, process locally, etc.
    // sendToServer(chunk.content);
  });
  
  return unsubscribe;
}

// Export for testing
export { testAudioServiceAPI, basicRecordingExample, recordingWithStatusMonitoring, processAudioChunks };