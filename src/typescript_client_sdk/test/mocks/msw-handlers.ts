/**
 * MSW handlers for mocking Agent C API
 */

import { http, HttpResponse, ws } from 'msw';
import { createMockJWT } from '../utils/test-helpers';

// Base API URL
const API_BASE_URL = process.env.VITE_API_BASE_URL || 'http://localhost:8080';
const WS_BASE_URL = process.env.VITE_WS_BASE_URL || 'ws://localhost:8080';

/**
 * Mock login response
 */
export const mockLoginResponse = {
  token: createMockJWT({ sub: 'test-user' }, 3600),
  heyGenToken: 'mock-heygen-token',
  voices: [
    {
      id: 'alloy',
      name: 'Alloy',
      provider: 'openai',
      format: 'pcm16',
      sampleRate: 24000,
      channels: 1,
    },
    {
      id: 'echo',
      name: 'Echo',
      provider: 'openai',
      format: 'pcm16',
      sampleRate: 24000,
      channels: 1,
    },
    {
      id: 'nova',
      name: 'Nova',
      provider: 'openai',
      format: 'pcm16',
      sampleRate: 24000,
      channels: 1,
    },
    {
      id: 'avatar',
      name: 'Avatar',
      provider: 'heygen',
      format: 'none',
      sampleRate: 0,
      channels: 0,
    },
    {
      id: 'none',
      name: 'Text Only',
      provider: 'none',
      format: 'none',
      sampleRate: 0,
      channels: 0,
    },
  ],
  avatars: [
    {
      id: 'josh_lite3_20230714',
      name: 'Josh',
      description: 'Professional male avatar',
    },
    {
      id: 'anna_public_3_20240108',
      name: 'Anna',
      description: 'Professional female avatar',
    },
  ],
  user: {
    id: 'test-user',
    name: 'Test User',
    email: 'test@example.com',
  },
};

/**
 * REST API handlers
 */
export const restHandlers = [
  // Login endpoint
  http.post(`${API_BASE_URL}/auth/login`, async ({ request }) => {
    const body = await request.json() as any;
    
    if (body.username === 'test' && body.password === 'test') {
      return HttpResponse.json(mockLoginResponse);
    }
    
    return HttpResponse.json(
      { error: 'Invalid credentials' },
      { status: 401 }
    );
  }),

  // Refresh token endpoint
  http.post(`${API_BASE_URL}/auth/refresh`, async ({ request }) => {
    const authHeader = request.headers.get('Authorization');
    
    if (authHeader && authHeader.startsWith('Bearer ')) {
      return HttpResponse.json({
        token: createMockJWT({ sub: 'test-user' }, 3600),
      });
    }
    
    return HttpResponse.json(
      { error: 'Invalid token' },
      { status: 401 }
    );
  }),

  // Logout endpoint
  http.post(`${API_BASE_URL}/auth/logout`, () => {
    return HttpResponse.json({ success: true });
  }),

  // Get session endpoint
  http.get(`${API_BASE_URL}/sessions/:sessionId`, ({ params }) => {
    const { sessionId } = params;
    
    return HttpResponse.json({
      id: sessionId,
      name: `Session ${sessionId}`,
      createdAt: new Date().toISOString(),
      messages: [],
      metadata: {},
    });
  }),

  // List sessions endpoint
  http.get(`${API_BASE_URL}/sessions`, () => {
    return HttpResponse.json({
      sessions: [
        {
          id: 'session-1',
          name: 'Session 1',
          createdAt: new Date().toISOString(),
        },
        {
          id: 'session-2',
          name: 'Session 2',
          createdAt: new Date().toISOString(),
        },
      ],
    });
  }),
];

/**
 * WebSocket handlers
 */
export const websocketHandlers = [
  ws.link(`${WS_BASE_URL}/realtime`).addEventListener('connection', ({ client }) => {
    console.log('WebSocket client connected');
    
    // Send initial connection event
    client.send(JSON.stringify({
      type: 'connection_established',
      timestamp: new Date().toISOString(),
      sessionId: 'test-session',
    }));
    
    // Handle incoming messages
    client.addEventListener('message', (event) => {
      const data = event.data;
      
      if (typeof data === 'string') {
        try {
          const message = JSON.parse(data);
          
          // Echo back different responses based on message type
          switch (message.type) {
            case 'text_input':
              // Simulate text response
              client.send(JSON.stringify({
                type: 'text_start',
                timestamp: new Date().toISOString(),
              }));
              
              // Send text deltas
              const words = 'Hello! This is a test response.'.split(' ');
              words.forEach((word, index) => {
                setTimeout(() => {
                  client.send(JSON.stringify({
                    type: 'text_delta',
                    content: word + (index < words.length - 1 ? ' ' : ''),
                    timestamp: new Date().toISOString(),
                  }));
                }, index * 100);
              });
              
              // Send text end
              setTimeout(() => {
                client.send(JSON.stringify({
                  type: 'text_end',
                  timestamp: new Date().toISOString(),
                }));
              }, words.length * 100);
              break;
              
            case 'set_agent_voice':
              // Acknowledge voice change
              client.send(JSON.stringify({
                type: 'voice_changed',
                voiceId: message.voiceId,
                timestamp: new Date().toISOString(),
              }));
              break;
              
            case 'user_turn_start':
              // Acknowledge turn start
              client.send(JSON.stringify({
                type: 'user_turn_started',
                timestamp: new Date().toISOString(),
              }));
              break;
              
            case 'user_turn_end':
              // Acknowledge turn end and start agent turn
              client.send(JSON.stringify({
                type: 'user_turn_ended',
                timestamp: new Date().toISOString(),
              }));
              
              setTimeout(() => {
                client.send(JSON.stringify({
                  type: 'agent_turn_start',
                  timestamp: new Date().toISOString(),
                }));
              }, 100);
              break;
              
            default:
              // Echo unknown messages
              client.send(JSON.stringify({
                type: 'echo',
                original: message,
                timestamp: new Date().toISOString(),
              }));
          }
        } catch (error) {
          console.error('Failed to parse WebSocket message:', error);
        }
      } else if (data instanceof ArrayBuffer) {
        // Handle binary audio data
        // Echo back with simulated audio response
        const responseAudio = new ArrayBuffer(1024);
        client.send(responseAudio);
      }
    });
    
    // Handle disconnection
    client.addEventListener('close', () => {
      console.log('WebSocket client disconnected');
    });
  }),
];

/**
 * All handlers combined
 */
export const handlers = [...restHandlers];