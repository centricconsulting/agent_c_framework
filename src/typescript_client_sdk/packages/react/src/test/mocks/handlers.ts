/**
 * MSW Request Handlers for React Package Tests
 * Provides mock API responses for React hooks testing
 */
import { http, HttpResponse } from 'msw';

// Base API URL (can be configured via environment variable)
const API_BASE_URL = process.env.VITE_API_URL || 'http://localhost:8000';

// Export handlers array for MSW server
export const handlers = [
  // Health check endpoint
  http.get(`${API_BASE_URL}/health`, () => {
    return HttpResponse.json(
      { status: 'ok', timestamp: new Date().toISOString() },
      { status: 200 }
    );
  }),

  // Session endpoints
  http.get(`${API_BASE_URL}/api/sessions`, () => {
    return HttpResponse.json(
      {
        sessions: [
          {
            id: 'session_1',
            name: 'Test Session 1',
            createdAt: '2024-01-01T00:00:00Z',
            updatedAt: '2024-01-01T00:00:00Z',
            metadata: {}
          },
          {
            id: 'session_2',
            name: 'Test Session 2',
            createdAt: '2024-01-02T00:00:00Z',
            updatedAt: '2024-01-02T00:00:00Z',
            metadata: {}
          }
        ]
      },
      { status: 200 }
    );
  }),

  http.post(`${API_BASE_URL}/api/sessions`, () => {
    return HttpResponse.json(
      {
        id: 'session_new',
        name: 'New Session',
        createdAt: new Date().toISOString(),
        updatedAt: new Date().toISOString(),
        metadata: {}
      },
      { status: 201 }
    );
  }),

  http.get(`${API_BASE_URL}/api/sessions/:sessionId`, ({ params }) => {
    const { sessionId } = params;
    return HttpResponse.json(
      {
        id: sessionId,
        name: `Session ${sessionId}`,
        createdAt: '2024-01-01T00:00:00Z',
        updatedAt: '2024-01-01T00:00:00Z',
        metadata: {},
        messages: []
      },
      { status: 200 }
    );
  }),

  http.delete(`${API_BASE_URL}/api/sessions/:sessionId`, () => {
    return new HttpResponse(null, { status: 204 });
  }),

  // Message endpoints
  http.get(`${API_BASE_URL}/api/sessions/:sessionId/messages`, () => {
    return HttpResponse.json(
      {
        messages: [
          {
            id: 'msg_1',
            role: 'user',
            content: 'Hello',
            timestamp: '2024-01-01T00:00:00Z'
          },
          {
            id: 'msg_2',
            role: 'assistant',
            content: 'Hi there!',
            timestamp: '2024-01-01T00:00:01Z'
          }
        ]
      },
      { status: 200 }
    );
  }),

  http.post(`${API_BASE_URL}/api/sessions/:sessionId/messages`, () => {
    return HttpResponse.json(
      {
        id: 'msg_new',
        role: 'user',
        content: 'New message',
        timestamp: new Date().toISOString()
      },
      { status: 201 }
    );
  }),

  // Avatar endpoints
  http.get(`${API_BASE_URL}/api/avatars`, () => {
    return HttpResponse.json(
      {
        avatars: [
          {
            id: 'avatar_1',
            name: 'Default Avatar',
            url: 'https://example.com/avatar1.png'
          }
        ]
      },
      { status: 200 }
    );
  }),

  // Audio configuration endpoint
  http.get(`${API_BASE_URL}/api/audio/config`, () => {
    return HttpResponse.json(
      {
        inputDevice: 'default',
        outputDevice: 'default',
        sampleRate: 16000,
        channels: 1
      },
      { status: 200 }
    );
  }),

  // Fallback handler for unmatched requests
  http.all('*', ({ request }) => {
    console.warn(`Unhandled ${request.method} request to ${request.url}`);
    return HttpResponse.json(
      { error: 'Not found' },
      { status: 404 }
    );
  })
];

// Test-specific handler overrides
export const errorHandlers = {
  serverError: http.get(`${API_BASE_URL}/api/sessions`, () => {
    return HttpResponse.json(
      { error: 'Internal server error' },
      { status: 500 }
    );
  }),
  
  networkError: http.get(`${API_BASE_URL}/api/sessions`, () => {
    return HttpResponse.error();
  }),
  
  timeout: http.get(`${API_BASE_URL}/api/sessions`, async () => {
    await new Promise(resolve => setTimeout(resolve, 60000));
    return HttpResponse.json({});
  })
};

// Helper function to create session mock data
export function createMockSession(overrides = {}) {
  return {
    id: `session_${Date.now()}`,
    name: 'Test Session',
    createdAt: new Date().toISOString(),
    updatedAt: new Date().toISOString(),
    metadata: {},
    messages: [],
    ...overrides
  };
}

// Helper function to create message mock data
export function createMockMessage(overrides = {}) {
  return {
    id: `msg_${Date.now()}`,
    role: 'assistant',
    content: 'Test message',
    timestamp: new Date().toISOString(),
    ...overrides
  };
}