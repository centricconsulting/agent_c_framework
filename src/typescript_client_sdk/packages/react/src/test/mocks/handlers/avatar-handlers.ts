/**
 * Avatar State and Animation Handlers for React Hooks Testing
 */
import { http, HttpResponse, delay } from 'msw';

// Avatar state management for tests
export const avatarState = {
  isVisible: true,
  currentAnimation: 'idle',
  animationSpeed: 1.0,
  expressionIntensity: 1.0,
  lipSyncEnabled: true,
  headTrackingEnabled: false,
  currentExpression: 'neutral',
  position: { x: 0, y: 0, z: 0 },
  rotation: { x: 0, y: 0, z: 0 }
};

// Animation queue for testing
const animationQueue: any[] = [];

export const avatarHandlers = [
  // Get avatar status
  http.get('*/api/avatar/status', async () => {
    await delay(20);
    return HttpResponse.json({
      success: true,
      data: {
        ...avatarState,
        animationQueue: animationQueue.slice(0, 5), // Return next 5 animations
        isReady: true
      }
    });
  }),

  // Update avatar visibility
  http.post('*/api/avatar/visibility', async ({ request }) => {
    await delay(30);
    const body = await request.json() as any;
    
    avatarState.isVisible = body.visible;
    
    return HttpResponse.json({
      success: true,
      data: {
        visible: avatarState.isVisible,
        timestamp: new Date().toISOString()
      }
    });
  }),

  // Trigger avatar animation
  http.post('*/api/avatar/animate', async ({ request }) => {
    await delay(30);
    const body = await request.json() as any;
    
    const animation = {
      id: `anim_${Date.now()}`,
      type: body.animation || 'gesture',
      duration: body.duration || 1000,
      expression: body.expression || 'neutral',
      startTime: new Date().toISOString()
    };
    
    // Update current animation
    avatarState.currentAnimation = body.animation || avatarState.currentAnimation;
    
    if (body.expression) {
      avatarState.currentExpression = body.expression;
    }
    
    // Add to queue if specified
    if (body.queue) {
      animationQueue.push(animation);
    }
    
    return HttpResponse.json({
      success: true,
      data: animation
    });
  }),

  // Set avatar expression
  http.post('*/api/avatar/expression', async ({ request }) => {
    await delay(25);
    const body = await request.json() as any;
    
    avatarState.currentExpression = body.expression;
    avatarState.expressionIntensity = body.intensity || 1.0;
    
    return HttpResponse.json({
      success: true,
      data: {
        expression: avatarState.currentExpression,
        intensity: avatarState.expressionIntensity,
        timestamp: new Date().toISOString()
      }
    });
  }),

  // Update avatar position
  http.post('*/api/avatar/position', async ({ request }) => {
    await delay(20);
    const body = await request.json() as any;
    
    if (body.position) {
      avatarState.position = { ...avatarState.position, ...body.position };
    }
    
    if (body.rotation) {
      avatarState.rotation = { ...avatarState.rotation, ...body.rotation };
    }
    
    return HttpResponse.json({
      success: true,
      data: {
        position: avatarState.position,
        rotation: avatarState.rotation,
        timestamp: new Date().toISOString()
      }
    });
  }),

  // Get available animations
  http.get('*/api/avatar/animations', async () => {
    await delay(30);
    
    return HttpResponse.json({
      success: true,
      data: {
        animations: [
          { id: 'idle', name: 'Idle', category: 'base' },
          { id: 'talking', name: 'Talking', category: 'speech' },
          { id: 'thinking', name: 'Thinking', category: 'gesture' },
          { id: 'nodding', name: 'Nodding', category: 'gesture' },
          { id: 'shaking_head', name: 'Shaking Head', category: 'gesture' },
          { id: 'waving', name: 'Waving', category: 'greeting' }
        ],
        expressions: [
          { id: 'neutral', name: 'Neutral' },
          { id: 'happy', name: 'Happy' },
          { id: 'sad', name: 'Sad' },
          { id: 'surprised', name: 'Surprised' },
          { id: 'thinking', name: 'Thinking' },
          { id: 'confused', name: 'Confused' }
        ]
      }
    });
  }),

  // Sync avatar with audio (lip sync)
  http.post('*/api/avatar/sync-audio', async ({ request }) => {
    await delay(25);
    const body = await request.json() as any;
    
    if (body.audioData) {
      // Simulate processing audio for lip sync
      const visemes = generateMockVisemes(body.duration || 1000);
      
      return HttpResponse.json({
        success: true,
        data: {
          visemes,
          duration: body.duration || 1000,
          timestamp: new Date().toISOString()
        }
      });
    }
    
    return HttpResponse.json({
      success: true,
      data: {
        message: 'Audio sync started',
        timestamp: new Date().toISOString()
      }
    });
  }),

  // Update avatar settings
  http.patch('*/api/avatar/settings', async ({ request }) => {
    await delay(30);
    const body = await request.json() as any;
    
    // Update settings
    if (body.animationSpeed !== undefined) {
      avatarState.animationSpeed = body.animationSpeed;
    }
    if (body.lipSyncEnabled !== undefined) {
      avatarState.lipSyncEnabled = body.lipSyncEnabled;
    }
    if (body.headTrackingEnabled !== undefined) {
      avatarState.headTrackingEnabled = body.headTrackingEnabled;
    }
    if (body.expressionIntensity !== undefined) {
      avatarState.expressionIntensity = body.expressionIntensity;
    }
    
    return HttpResponse.json({
      success: true,
      data: avatarState
    });
  }),

  // Reset avatar to default state
  http.post('*/api/avatar/reset', async () => {
    await delay(20);
    
    // Reset to defaults
    avatarState.currentAnimation = 'idle';
    avatarState.currentExpression = 'neutral';
    avatarState.position = { x: 0, y: 0, z: 0 };
    avatarState.rotation = { x: 0, y: 0, z: 0 };
    animationQueue.length = 0;
    
    return HttpResponse.json({
      success: true,
      data: {
        message: 'Avatar reset to default state',
        state: avatarState,
        timestamp: new Date().toISOString()
      }
    });
  }),

  // Get avatar events stream (SSE for real-time updates)
  http.get('*/api/avatar/events', async () => {
    const encoder = new TextEncoder();
    const stream = new ReadableStream({
      async start(controller) {
        // Send initial state
        controller.enqueue(
          encoder.encode(`data: ${JSON.stringify({
            type: 'avatar_state',
            data: avatarState
          })}\n\n`)
        );
        
        // Simulate animation updates
        await delay(100);
        controller.enqueue(
          encoder.encode(`data: ${JSON.stringify({
            type: 'animation_start',
            data: {
              animation: 'talking',
              duration: 2000
            }
          })}\n\n`)
        );
        
        await delay(200);
        controller.enqueue(
          encoder.encode(`data: ${JSON.stringify({
            type: 'expression_change',
            data: {
              expression: 'happy',
              intensity: 0.8
            }
          })}\n\n`)
        );
        
        await delay(300);
        controller.enqueue(
          encoder.encode(`data: ${JSON.stringify({
            type: 'animation_end',
            data: {
              animation: 'talking'
            }
          })}\n\n`)
        );
        
        controller.close();
      }
    });
    
    return new HttpResponse(stream, {
      headers: {
        'Content-Type': 'text/event-stream',
        'Cache-Control': 'no-cache',
        'Connection': 'keep-alive'
      }
    });
  })
];

// Helper function to generate mock visemes for lip sync
function generateMockVisemes(duration: number) {
  const visemeCount = Math.floor(duration / 50); // One viseme every 50ms
  const visemes = [];
  const visemeTypes = ['AA', 'EE', 'II', 'OO', 'UU', 'CH', 'DD', 'FF', 'M', 'N', 'P', 'R', 'S', 'TH'];
  
  for (let i = 0; i < visemeCount; i++) {
    visemes.push({
      time: i * 50,
      viseme: visemeTypes[Math.floor(Math.random() * visemeTypes.length)],
      weight: Math.random()
    });
  }
  
  return visemes;
}