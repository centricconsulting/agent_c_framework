/**
 * Audio Status and Control Handlers for React Hooks Testing
 */
import { http, HttpResponse, delay } from 'msw';

// Audio state management for tests
export const audioState = {
  isRecording: false,
  isStreaming: false,
  isMuted: false,
  volume: 1.0,
  inputDevice: 'default',
  outputDevice: 'default',
  voiceModel: 'claude-3-sonnet',
  sampleRate: 24000,
  vadEnabled: true,
  vadThreshold: 0.5
};

// Turn state for audio/chat coordination
export const turnState = {
  currentTurn: null as 'user' | 'assistant' | null,
  turnId: null as string | null,
  turnStartTime: null as string | null,
  isInterrupted: false
};

export const audioHandlers = [
  // Get audio status
  http.get('*/api/audio/status', async () => {
    await delay(20);
    return HttpResponse.json({
      success: true,
      data: {
        ...audioState,
        turnState: {
          ...turnState
        }
      }
    });
  }),

  // Start audio recording
  http.post('*/api/audio/record/start', async ({ request }) => {
    await delay(30);
    const body = await request.json() as any;
    
    if (audioState.isRecording) {
      return HttpResponse.json(
        { error: 'Already recording' },
        { status: 409 }
      );
    }
    
    audioState.isRecording = true;
    audioState.inputDevice = body.inputDevice || audioState.inputDevice;
    
    // Start a user turn
    turnState.currentTurn = 'user';
    turnState.turnId = `turn_${Date.now()}`;
    turnState.turnStartTime = new Date().toISOString();
    turnState.isInterrupted = false;
    
    return HttpResponse.json({
      success: true,
      data: {
        recordingStarted: true,
        turnId: turnState.turnId,
        timestamp: new Date().toISOString()
      }
    });
  }),

  // Stop audio recording
  http.post('*/api/audio/record/stop', async () => {
    await delay(30);
    
    if (!audioState.isRecording) {
      return HttpResponse.json(
        { error: 'Not recording' },
        { status: 409 }
      );
    }
    
    audioState.isRecording = false;
    
    // End user turn
    const turnId = turnState.turnId;
    turnState.currentTurn = null;
    turnState.turnId = null;
    
    return HttpResponse.json({
      success: true,
      data: {
        recordingStopped: true,
        turnId,
        duration: 3500, // Mock duration in ms
        timestamp: new Date().toISOString()
      }
    });
  }),

  // Start audio streaming (for assistant responses)
  http.post('*/api/audio/stream/start', async ({ request }) => {
    await delay(30);
    const body = await request.json() as any;
    
    if (audioState.isStreaming) {
      return HttpResponse.json(
        { error: 'Already streaming' },
        { status: 409 }
      );
    }
    
    audioState.isStreaming = true;
    
    // Start assistant turn
    turnState.currentTurn = 'assistant';
    turnState.turnId = `turn_${Date.now()}`;
    turnState.turnStartTime = new Date().toISOString();
    turnState.isInterrupted = false;
    
    return HttpResponse.json({
      success: true,
      data: {
        streamingStarted: true,
        turnId: turnState.turnId,
        voiceModel: body.voiceModel || audioState.voiceModel,
        timestamp: new Date().toISOString()
      }
    });
  }),

  // Stop audio streaming
  http.post('*/api/audio/stream/stop', async () => {
    await delay(30);
    
    if (!audioState.isStreaming) {
      return HttpResponse.json(
        { error: 'Not streaming' },
        { status: 409 }
      );
    }
    
    audioState.isStreaming = false;
    
    // End assistant turn
    const turnId = turnState.turnId;
    turnState.currentTurn = null;
    turnState.turnId = null;
    
    return HttpResponse.json({
      success: true,
      data: {
        streamingStopped: true,
        turnId,
        timestamp: new Date().toISOString()
      }
    });
  }),

  // Interrupt current audio (for turn interruption)
  http.post('*/api/audio/interrupt', async () => {
    await delay(20);
    
    if (!audioState.isRecording && !audioState.isStreaming) {
      return HttpResponse.json(
        { error: 'No active audio to interrupt' },
        { status: 409 }
      );
    }
    
    // Mark turn as interrupted
    turnState.isInterrupted = true;
    
    // Stop any active audio
    audioState.isRecording = false;
    audioState.isStreaming = false;
    
    const interruptedTurn = turnState.turnId;
    turnState.currentTurn = null;
    turnState.turnId = null;
    
    return HttpResponse.json({
      success: true,
      data: {
        interrupted: true,
        interruptedTurn,
        timestamp: new Date().toISOString()
      }
    });
  }),

  // Update audio settings
  http.patch('*/api/audio/settings', async ({ request }) => {
    await delay(30);
    const body = await request.json() as any;
    
    // Update audio settings
    Object.assign(audioState, body);
    
    return HttpResponse.json({
      success: true,
      data: audioState
    });
  }),

  // Get available voice models
  http.get('*/api/audio/voice-models', async () => {
    await delay(40);
    
    return HttpResponse.json({
      success: true,
      data: {
        models: [
          { id: 'claude-3-sonnet', name: 'Claude 3 Sonnet', language: 'en' },
          { id: 'claude-3-opus', name: 'Claude 3 Opus', language: 'en' },
          { id: 'claude-3-haiku', name: 'Claude 3 Haiku', language: 'en' }
        ],
        current: audioState.voiceModel
      }
    });
  }),

  // Get audio devices
  http.get('*/api/audio/devices', async () => {
    await delay(30);
    
    return HttpResponse.json({
      success: true,
      data: {
        inputDevices: [
          { id: 'default', name: 'Default Microphone' },
          { id: 'mic-1', name: 'External Microphone' }
        ],
        outputDevices: [
          { id: 'default', name: 'Default Speakers' },
          { id: 'headphones-1', name: 'Headphones' }
        ],
        currentInput: audioState.inputDevice,
        currentOutput: audioState.outputDevice
      }
    });
  }),

  // Get turn history
  http.get('*/api/audio/turns', async ({ request }) => {
    await delay(40);
    const url = new URL(request.url);
    const sessionId = url.searchParams.get('sessionId');
    
    return HttpResponse.json({
      success: true,
      data: {
        sessionId,
        turns: [
          {
            id: 'turn_1',
            type: 'user',
            startTime: '2024-01-01T10:00:00Z',
            endTime: '2024-01-01T10:00:03Z',
            duration: 3000,
            wasInterrupted: false
          },
          {
            id: 'turn_2',
            type: 'assistant',
            startTime: '2024-01-01T10:00:04Z',
            endTime: '2024-01-01T10:00:08Z',
            duration: 4000,
            wasInterrupted: false
          }
        ],
        currentTurn: turnState.currentTurn ? {
          id: turnState.turnId,
          type: turnState.currentTurn,
          startTime: turnState.turnStartTime,
          isActive: true,
          isInterrupted: turnState.isInterrupted
        } : null
      }
    });
  })
];