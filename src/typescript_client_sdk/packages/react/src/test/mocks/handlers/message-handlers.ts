/**
 * Message and Streaming Handlers for React Hooks Testing
 */
import { http, HttpResponse, delay } from 'msw';
import { sessionStore } from './session-handlers';

// Helper to create a mock message
export const createMockMessage = (overrides?: any) => ({
  id: `msg_${Date.now()}_${Math.random().toString(36).substr(2, 9)}`,
  role: 'assistant',
  content: 'Test message content',
  timestamp: new Date().toISOString(),
  isStreaming: false,
  ...overrides
});

// Helper to simulate streaming chunks
export const createStreamingChunks = (finalContent: string, chunkCount: number = 5) => {
  const chunks: string[] = [];
  const chunkSize = Math.ceil(finalContent.length / chunkCount);
  
  for (let i = 0; i < finalContent.length; i += chunkSize) {
    chunks.push(finalContent.slice(0, i + chunkSize));
  }
  
  return chunks;
};

export const messageHandlers = [
  // Send a message to a session
  http.post('*/api/sessions/:sessionId/messages', async ({ params, request }) => {
    const { sessionId } = params;
    const body = await request.json() as any;
    
    const session = sessionStore.get(sessionId as string);
    
    if (!session) {
      return HttpResponse.json(
        { error: 'Session not found' },
        { status: 404 }
      );
    }
    
    // Create user message
    const userMessage = createMockMessage({
      role: 'user',
      content: body.content || body.message,
      isStreaming: false
    });
    
    session.messages.push(userMessage);
    
    // Simulate processing delay
    await delay(100);
    
    // Create assistant response
    const assistantMessage = createMockMessage({
      role: 'assistant',
      content: `Response to: ${body.content || body.message}`,
      isStreaming: false
    });
    
    session.messages.push(assistantMessage);
    
    return HttpResponse.json({
      success: true,
      data: {
        userMessage,
        assistantMessage
      }
    });
  }),

  // Stream a message response (SSE endpoint)
  http.get('*/api/sessions/:sessionId/stream', async ({ params, request }) => {
    const { sessionId } = params;
    const url = new URL(request.url);
    const prompt = url.searchParams.get('prompt') || 'Test prompt';
    
    const session = sessionStore.get(sessionId as string);
    
    if (!session) {
      return HttpResponse.json(
        { error: 'Session not found' },
        { status: 404 }
      );
    }
    
    // Create a streaming response
    const encoder = new TextEncoder();
    const stream = new ReadableStream({
      async start(controller) {
        const messageId = `msg_stream_${Date.now()}`;
        const finalContent = `This is a streamed response to: ${prompt}. It includes multiple sentences to demonstrate streaming behavior.`;
        const chunks = createStreamingChunks(finalContent);
        
        // Send streaming start event
        controller.enqueue(
          encoder.encode(`data: ${JSON.stringify({
            type: 'stream_start',
            messageId,
            role: 'assistant'
          })}\n\n`)
        );
        
        // Send content chunks
        for (const chunk of chunks) {
          await delay(50); // Simulate streaming delay
          controller.enqueue(
            encoder.encode(`data: ${JSON.stringify({
              type: 'stream_chunk',
              messageId,
              content: chunk,
              isComplete: false
            })}\n\n`)
          );
        }
        
        // Send completion event
        controller.enqueue(
          encoder.encode(`data: ${JSON.stringify({
            type: 'stream_end',
            messageId,
            content: finalContent,
            isComplete: true
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

  // Get messages for a session
  http.get('*/api/sessions/:sessionId/messages', async ({ params, request }) => {
    await delay(30);
    const { sessionId } = params;
    const url = new URL(request.url);
    const limit = parseInt(url.searchParams.get('limit') || '50');
    const before = url.searchParams.get('before');
    
    const session = sessionStore.get(sessionId as string);
    
    if (!session) {
      return HttpResponse.json(
        { error: 'Session not found' },
        { status: 404 }
      );
    }
    
    let messages = session.messages || [];
    
    // Filter messages before a certain ID if specified
    if (before) {
      const beforeIndex = messages.findIndex((m: any) => m.id === before);
      if (beforeIndex > 0) {
        messages = messages.slice(0, beforeIndex);
      }
    }
    
    // Apply limit
    messages = messages.slice(-limit);
    
    return HttpResponse.json({
      success: true,
      data: {
        messages,
        hasMore: messages.length === limit
      }
    });
  }),

  // Delete a message
  http.delete('*/api/sessions/:sessionId/messages/:messageId', async ({ params }) => {
    await delay(30);
    const { sessionId, messageId } = params;
    
    const session = sessionStore.get(sessionId as string);
    
    if (!session) {
      return HttpResponse.json(
        { error: 'Session not found' },
        { status: 404 }
      );
    }
    
    const messageIndex = session.messages.findIndex((m: any) => m.id === messageId);
    
    if (messageIndex === -1) {
      return HttpResponse.json(
        { error: 'Message not found' },
        { status: 404 }
      );
    }
    
    session.messages.splice(messageIndex, 1);
    
    return HttpResponse.json({
      success: true,
      message: 'Message deleted'
    });
  }),

  // Edit a message
  http.patch('*/api/sessions/:sessionId/messages/:messageId', async ({ params, request }) => {
    await delay(40);
    const { sessionId, messageId } = params;
    const body = await request.json() as any;
    
    const session = sessionStore.get(sessionId as string);
    
    if (!session) {
      return HttpResponse.json(
        { error: 'Session not found' },
        { status: 404 }
      );
    }
    
    const message = session.messages.find((m: any) => m.id === messageId);
    
    if (!message) {
      return HttpResponse.json(
        { error: 'Message not found' },
        { status: 404 }
      );
    }
    
    // Update message content
    message.content = body.content;
    message.edited = true;
    message.editedAt = new Date().toISOString();
    
    return HttpResponse.json({
      success: true,
      data: message
    });
  })
];