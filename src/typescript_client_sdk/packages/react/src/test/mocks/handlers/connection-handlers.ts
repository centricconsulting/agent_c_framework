/**
 * Connection and WebSocket-like Event Handlers for React Hooks Testing
 */
import { http, HttpResponse, delay } from 'msw';

// Connection state management
export const connectionState = {
  status: 'disconnected' as 'connected' | 'connecting' | 'disconnected' | 'error',
  sessionId: null as string | null,
  lastPing: null as string | null,
  reconnectAttempts: 0,
  error: null as string | null
};

// Event subscriptions for testing
const eventSubscriptions = new Set<string>();

export const connectionHandlers = [
  // Establish WebSocket-like connection
  http.post('*/api/connect', async ({ request }) => {
    await delay(100); // Simulate connection delay
    const body = await request.json() as any;
    
    if (connectionState.status === 'connected') {
      return HttpResponse.json(
        { error: 'Already connected' },
        { status: 409 }
      );
    }
    
    connectionState.status = 'connecting';
    
    // Simulate connection establishment
    await delay(200);
    
    connectionState.status = 'connected';
    connectionState.sessionId = `ws_${Date.now()}`;
    connectionState.lastPing = new Date().toISOString();
    connectionState.error = null;
    
    return HttpResponse.json({
      success: true,
      data: {
        sessionId: connectionState.sessionId,
        status: connectionState.status,
        config: body.config || {},
        timestamp: new Date().toISOString()
      }
    });
  }),

  // Disconnect WebSocket-like connection
  http.post('*/api/disconnect', async () => {
    await delay(50);
    
    if (connectionState.status === 'disconnected') {
      return HttpResponse.json(
        { error: 'Not connected' },
        { status: 409 }
      );
    }
    
    connectionState.status = 'disconnected';
    connectionState.sessionId = null;
    connectionState.lastPing = null;
    eventSubscriptions.clear();
    
    return HttpResponse.json({
      success: true,
      data: {
        message: 'Disconnected successfully',
        timestamp: new Date().toISOString()
      }
    });
  }),

  // Get connection status
  http.get('*/api/connection/status', async () => {
    await delay(20);
    
    return HttpResponse.json({
      success: true,
      data: {
        ...connectionState,
        subscriptions: Array.from(eventSubscriptions),
        timestamp: new Date().toISOString()
      }
    });
  }),

  // Ping/heartbeat endpoint
  http.post('*/api/connection/ping', async () => {
    await delay(10);
    
    if (connectionState.status !== 'connected') {
      return HttpResponse.json(
        { error: 'Not connected' },
        { status: 409 }
      );
    }
    
    connectionState.lastPing = new Date().toISOString();
    
    return HttpResponse.json({
      success: true,
      data: {
        pong: true,
        sessionId: connectionState.sessionId,
        timestamp: connectionState.lastPing
      }
    });
  }),

  // Subscribe to events
  http.post('*/api/connection/subscribe', async ({ request }) => {
    await delay(30);
    const body = await request.json() as any;
    
    if (connectionState.status !== 'connected') {
      return HttpResponse.json(
        { error: 'Not connected' },
        { status: 409 }
      );
    }
    
    const events = body.events || [];
    events.forEach((event: string) => eventSubscriptions.add(event));
    
    return HttpResponse.json({
      success: true,
      data: {
        subscribed: events,
        totalSubscriptions: eventSubscriptions.size,
        timestamp: new Date().toISOString()
      }
    });
  }),

  // Unsubscribe from events
  http.post('*/api/connection/unsubscribe', async ({ request }) => {
    await delay(30);
    const body = await request.json() as any;
    
    const events = body.events || [];
    events.forEach((event: string) => eventSubscriptions.delete(event));
    
    return HttpResponse.json({
      success: true,
      data: {
        unsubscribed: events,
        remainingSubscriptions: eventSubscriptions.size,
        timestamp: new Date().toISOString()
      }
    });
  }),

  // Reconnect endpoint
  http.post('*/api/connection/reconnect', async ({ request }) => {
    await delay(150);
    const body = await request.json() as any;
    
    if (connectionState.status === 'connected') {
      return HttpResponse.json(
        { error: 'Already connected' },
        { status: 409 }
      );
    }
    
    connectionState.reconnectAttempts++;
    
    // Simulate reconnection with possible failure
    if (connectionState.reconnectAttempts > 3 && Math.random() > 0.5) {
      connectionState.status = 'error';
      connectionState.error = 'Max reconnection attempts exceeded';
      
      return HttpResponse.json(
        { error: connectionState.error },
        { status: 503 }
      );
    }
    
    connectionState.status = 'connecting';
    
    await delay(100);
    
    connectionState.status = 'connected';
    connectionState.sessionId = body.sessionId || `ws_reconnect_${Date.now()}`;
    connectionState.lastPing = new Date().toISOString();
    connectionState.error = null;
    
    return HttpResponse.json({
      success: true,
      data: {
        sessionId: connectionState.sessionId,
        status: connectionState.status,
        reconnectAttempt: connectionState.reconnectAttempts,
        timestamp: new Date().toISOString()
      }
    });
  }),

  // Event stream endpoint (SSE for real-time events)
  http.get('*/api/connection/events', async ({ request }) => {
    const url = new URL(request.url);
    const sessionId = url.searchParams.get('sessionId');
    
    if (!sessionId || sessionId !== connectionState.sessionId) {
      return HttpResponse.json(
        { error: 'Invalid session' },
        { status: 401 }
      );
    }
    
    const encoder = new TextEncoder();
    const stream = new ReadableStream({
      async start(controller) {
        // Send connection established event
        controller.enqueue(
          encoder.encode(`data: ${JSON.stringify({
            type: 'connection_established',
            sessionId: connectionState.sessionId,
            timestamp: new Date().toISOString()
          })}\n\n`)
        );
        
        // Send turn start event
        await delay(500);
        controller.enqueue(
          encoder.encode(`data: ${JSON.stringify({
            type: 'turn_start',
            turn_id: 'turn_test_1',
            turn_type: 'user',
            timestamp: new Date().toISOString()
          })}\n\n`)
        );
        
        // Send message streaming event
        await delay(300);
        controller.enqueue(
          encoder.encode(`data: ${JSON.stringify({
            type: 'message_streaming',
            message: {
              id: 'msg_stream_1',
              role: 'assistant',
              content: 'This is a streaming message...',
              isStreaming: true
            },
            timestamp: new Date().toISOString()
          })}\n\n`)
        );
        
        // Send message complete event
        await delay(500);
        controller.enqueue(
          encoder.encode(`data: ${JSON.stringify({
            type: 'message_complete',
            message: {
              id: 'msg_stream_1',
              role: 'assistant',
              content: 'This is a streaming message that is now complete.',
              isStreaming: false
            },
            timestamp: new Date().toISOString()
          })}\n\n`)
        );
        
        // Send turn end event
        await delay(200);
        controller.enqueue(
          encoder.encode(`data: ${JSON.stringify({
            type: 'turn_end',
            turn_id: 'turn_test_1',
            turn_type: 'user',
            timestamp: new Date().toISOString()
          })}\n\n`)
        );
        
        // Send session update event
        await delay(400);
        controller.enqueue(
          encoder.encode(`data: ${JSON.stringify({
            type: 'session_updated',
            session: {
              id: 'session_test',
              name: 'Updated Session Name',
              messagesCount: 5,
              lastActivity: new Date().toISOString()
            },
            timestamp: new Date().toISOString()
          })}\n\n`)
        );
        
        // Send audio status event
        await delay(300);
        controller.enqueue(
          encoder.encode(`data: ${JSON.stringify({
            type: 'audio_status_changed',
            status: {
              isRecording: false,
              isStreaming: true,
              volume: 0.75
            },
            timestamp: new Date().toISOString()
          })}\n\n`)
        );
        
        // Send avatar event
        await delay(250);
        controller.enqueue(
          encoder.encode(`data: ${JSON.stringify({
            type: 'avatar_animation',
            animation: {
              type: 'thinking',
              duration: 2000
            },
            timestamp: new Date().toISOString()
          })}\n\n`)
        );
        
        // Send error event
        await delay(600);
        controller.enqueue(
          encoder.encode(`data: ${JSON.stringify({
            type: 'error',
            error: {
              code: 'TEST_ERROR',
              message: 'This is a test error for handling',
              recoverable: true
            },
            timestamp: new Date().toISOString()
          })}\n\n`)
        );
        
        // Send connection closing event
        await delay(1000);
        controller.enqueue(
          encoder.encode(`data: ${JSON.stringify({
            type: 'connection_closing',
            reason: 'Test complete',
            timestamp: new Date().toISOString()
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
  }),

  // Send event to server (for testing event emission)
  http.post('*/api/connection/send-event', async ({ request }) => {
    await delay(30);
    const body = await request.json() as any;
    
    if (connectionState.status !== 'connected') {
      return HttpResponse.json(
        { error: 'Not connected' },
        { status: 409 }
      );
    }
    
    // Acknowledge event receipt
    return HttpResponse.json({
      success: true,
      data: {
        eventType: body.type,
        eventData: body.data,
        processed: true,
        timestamp: new Date().toISOString()
      }
    });
  })
];