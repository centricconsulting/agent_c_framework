/**
 * MSW Verification Test
 * Tests to verify that MSW is properly configured and working
 */
import { describe, it, expect, beforeAll, afterEach, afterAll } from 'vitest';
import { http, HttpResponse } from 'msw';
import { server, addTestHandler, startMockServer, stopMockServer, resetMockServer } from './mocks/server';

describe('MSW Setup Verification', () => {
  beforeAll(() => {
    startMockServer();
  });

  afterEach(() => {
    resetMockServer();
  });

  afterAll(() => {
    stopMockServer();
  });

  it('should have MSW server configured', () => {
    expect(server).toBeDefined();
    expect(typeof server.listen).toBe('function');
    expect(typeof server.close).toBe('function');
    expect(typeof server.resetHandlers).toBe('function');
  });

  it('should handle a mocked API request', async () => {
    const API_BASE_URL = process.env.VITE_API_URL || 'http://localhost:8000';
    
    // Make a fetch request to the mocked endpoint
    const response = await fetch(`${API_BASE_URL}/health`);
    const data = await response.json();
    
    expect(response.status).toBe(200);
    expect(data.status).toBe('ok');
    expect(data.timestamp).toBeDefined();
  });

  it('should allow adding runtime handlers', async () => {
    const API_BASE_URL = process.env.VITE_API_URL || 'http://localhost:8000';
    
    // Add a custom handler for this test
    addTestHandler(
      http.get(`${API_BASE_URL}/test-endpoint`, () => {
        return HttpResponse.json(
          { message: 'Custom handler response' },
          { status: 200 }
        );
      })
    );
    
    const response = await fetch(`${API_BASE_URL}/test-endpoint`);
    const data = await response.json();
    
    expect(response.status).toBe(200);
    expect(data.message).toBe('Custom handler response');
  });

  it('should handle session API requests', async () => {
    const API_BASE_URL = process.env.VITE_API_URL || 'http://localhost:8000';
    
    const response = await fetch(`${API_BASE_URL}/api/sessions`);
    const data = await response.json();
    
    expect(response.status).toBe(200);
    expect(data.sessions).toBeDefined();
    expect(Array.isArray(data.sessions)).toBe(true);
    expect(data.sessions.length).toBeGreaterThan(0);
    expect(data.sessions[0].id).toBe('session_1');
  });

  it('should handle POST requests', async () => {
    const API_BASE_URL = process.env.VITE_API_URL || 'http://localhost:8000';
    
    const response = await fetch(`${API_BASE_URL}/api/sessions`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify({ name: 'New Session' })
    });
    const data = await response.json();
    
    expect(response.status).toBe(201);
    expect(data.id).toBe('session_new');
    expect(data.name).toBe('New Session');
  });

  it('should handle 404 for unmatched routes', async () => {
    const API_BASE_URL = process.env.VITE_API_URL || 'http://localhost:8000';
    
    const response = await fetch(`${API_BASE_URL}/api/nonexistent`);
    const data = await response.json();
    
    expect(response.status).toBe(404);
    expect(data.error).toBe('Not found');
  });

  it('should support error simulation', async () => {
    const API_BASE_URL = process.env.VITE_API_URL || 'http://localhost:8000';
    
    // Override with error handler
    addTestHandler(
      http.get(`${API_BASE_URL}/api/sessions`, () => {
        return HttpResponse.json(
          { error: 'Simulated server error' },
          { status: 500 }
        );
      })
    );
    
    const response = await fetch(`${API_BASE_URL}/api/sessions`);
    const data = await response.json();
    
    expect(response.status).toBe(500);
    expect(data.error).toBe('Simulated server error');
  });
});

describe('MSW Mock Helpers', () => {
  it('should create mock session data', async () => {
    const { createMockSession } = await import('./mocks/handlers');
    
    const session = createMockSession({ name: 'Custom Session' });
    
    expect(session.id).toMatch(/^session_\d+$/);
    expect(session.name).toBe('Custom Session');
    expect(session.createdAt).toBeDefined();
    expect(session.messages).toEqual([]);
  });

  it('should create mock message data', async () => {
    const { createMockMessage } = await import('./mocks/handlers');
    
    const message = createMockMessage({ content: 'Test content', role: 'user' });
    
    expect(message.id).toMatch(/^msg_\d+$/);
    expect(message.content).toBe('Test content');
    expect(message.role).toBe('user');
    expect(message.timestamp).toBeDefined();
  });
});