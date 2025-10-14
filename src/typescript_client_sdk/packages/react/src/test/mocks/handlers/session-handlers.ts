/**
 * Session Management Handlers for React Hooks Testing
 */
import { http, HttpResponse, delay } from 'msw';

// In-memory session storage for tests
export const sessionStore = new Map<string, any>();
export const sessionHistoryStore = new Map<string, any[]>();

// Helper to create a mock session
export const createMockSession = (id?: string, overrides?: any) => ({
  id: id || `session_${Date.now()}`,
  name: 'Test Session',
  createdAt: new Date().toISOString(),
  updatedAt: new Date().toISOString(),
  messages: [],
  metadata: {},
  status: 'active',
  ...overrides
});

export const sessionHandlers = [
  // Get all sessions (for useChatSessionList)
  http.get('*/api/sessions', async ({ request }) => {
    await delay(50); // Simulate network delay
    
    const url = new URL(request.url);
    const search = url.searchParams.get('search');
    const limit = parseInt(url.searchParams.get('limit') || '50');
    const offset = parseInt(url.searchParams.get('offset') || '0');
    
    let sessions = Array.from(sessionStore.values());
    
    // Filter by search if provided
    if (search) {
      sessions = sessions.filter(s => 
        s.name.toLowerCase().includes(search.toLowerCase())
      );
    }
    
    // Apply pagination
    const paginatedSessions = sessions.slice(offset, offset + limit);
    
    return HttpResponse.json({
      success: true,
      data: {
        sessions: paginatedSessions,
        total: sessions.length,
        hasMore: offset + limit < sessions.length
      }
    });
  }),

  // Create a new session
  http.post('*/api/sessions', async ({ request }) => {
    const body = await request.json() as any;
    const session = createMockSession(undefined, {
      name: body.name || 'New Session',
      metadata: body.metadata || {}
    });
    
    sessionStore.set(session.id, session);
    
    // Simulate optimistic update delay
    await delay(100);
    
    return HttpResponse.json({
      success: true,
      data: session
    }, { status: 201 });
  }),

  // Get a specific session
  http.get('*/api/sessions/:id', async ({ params }) => {
    await delay(30);
    const { id } = params;
    const session = sessionStore.get(id as string);
    
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

  // Update a session
  http.patch('*/api/sessions/:id', async ({ params, request }) => {
    await delay(50);
    const { id } = params;
    const body = await request.json() as any;
    
    const session = sessionStore.get(id as string);
    
    if (!session) {
      return HttpResponse.json(
        { error: 'Session not found' },
        { status: 404 }
      );
    }
    
    // Update session fields
    Object.assign(session, body, {
      updatedAt: new Date().toISOString()
    });
    
    return HttpResponse.json({
      success: true,
      data: session
    });
  }),

  // Delete a session
  http.delete('*/api/sessions/:id', async ({ params }) => {
    await delay(30);
    const { id } = params;
    
    if (!sessionStore.has(id as string)) {
      return HttpResponse.json(
        { error: 'Session not found' },
        { status: 404 }
      );
    }
    
    sessionStore.delete(id as string);
    
    return HttpResponse.json({
      success: true,
      message: 'Session deleted'
    });
  }),

  // Get session history
  http.get('*/api/sessions/:id/history', async ({ params }) => {
    await delay(40);
    const { id } = params;
    
    const history = sessionHistoryStore.get(id as string) || [];
    
    return HttpResponse.json({
      success: true,
      data: {
        sessionId: id,
        history,
        total: history.length
      }
    });
  }),

  // Switch active session (for session management)
  http.post('*/api/sessions/:id/activate', async ({ params }) => {
    await delay(20);
    const { id } = params;
    
    const session = sessionStore.get(id as string);
    
    if (!session) {
      return HttpResponse.json(
        { error: 'Session not found' },
        { status: 404 }
      );
    }
    
    // Mark all sessions as inactive
    sessionStore.forEach(s => s.isActive = false);
    
    // Mark this session as active
    session.isActive = true;
    
    return HttpResponse.json({
      success: true,
      data: session
    });
  })
];