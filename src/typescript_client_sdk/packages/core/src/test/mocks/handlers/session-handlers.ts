import { http, HttpResponse, delay } from 'msw';
// import { sessionWithDelegation } from '../fixtures/sessions'; // TODO: Fix import path
// import { createSessionFromMessages } from '../utils/session-factory'; // TODO: Fix import path

// Temporary inline function until import is fixed
const createSessionFromMessages = (messages: any[]) => ({
  session_id: 'test-session',
  messages,
  metadata: {}
});

// In-memory session storage for tests
const sessions = new Map<string, any>();

export const sessionHandlers = [
  // Create session
  http.post('*/api/sessions', async ({ request }) => {
    const body = await request.json();
    const sessionId = `session_${Date.now()}`;
    
    const session = {
      session_id: sessionId,
      session_name: body.session_name || 'Test Session',
      messages: [],
      created_at: new Date().toISOString(),
      updated_at: new Date().toISOString(),
      token_count: 0,
      metadata: body.metadata || {}
    };
    
    sessions.set(sessionId, session);
    
    return HttpResponse.json({
      success: true,
      data: session
    }, { status: 201 });
  }),

  // Get session with ChatSessionChanged simulation
  http.get('*/api/sessions/:id', async ({ params }) => {
    await delay(50); // Simulate network delay
    
    const { id } = params;
    
    // Special test cases
    if (id === 'test-delegation-session') {
      // TODO: Load from fixture file once import is fixed
      const sessionWithDelegation = {
        session_id: 'test-delegation-session',
        messages: [],
        metadata: {}
      };
      return HttpResponse.json({
        success: true,
        data: sessionWithDelegation
      });
    }
    
    if (id === 'test-resumed-session') {
      // Return a session with various message types for testing
      return HttpResponse.json({
        success: true,
        data: createSessionFromMessages([
          { role: 'user', content: 'Initial question' },
          { role: 'assistant', content: 'Let me think about this...' },
          { 
            role: 'assistant',
            content: [
              {
                type: 'tool_use',
                id: 'think-1',
                name: 'think',
                input: { thought: 'Analyzing the request...' }
              }
            ]
          },
          {
            role: 'user',
            content: [
              {
                type: 'tool_result',
                tool_use_id: 'think-1',
                content: ''
              }
            ]
          },
          { role: 'assistant', content: 'Here is my response...' }
        ])
      });
    }
    
    const session = sessions.get(id as string);
    
    if (!session) {
      return HttpResponse.json(
        { error: 'Session not found' },
        { status: 404 }
      );
    }
    
    return HttpResponse.json({
      success: true,
      data: session
    });
  }),

  // Update session messages
  http.patch('*/api/sessions/:id/messages', async ({ params, request }) => {
    const { id } = params;
    const body = await request.json();
    
    const session = sessions.get(id as string);
    
    if (!session) {
      return HttpResponse.json(
        { error: 'Session not found' },
        { status: 404 }
      );
    }
    
    session.messages = body.messages;
    session.updated_at = new Date().toISOString();
    
    return HttpResponse.json({
      success: true,
      data: session
    });
  })
];