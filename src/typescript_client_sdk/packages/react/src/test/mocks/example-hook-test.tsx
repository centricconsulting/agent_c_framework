/**
 * Example Test File: How to Use MSW with React Hooks
 * This file demonstrates testing patterns with the MSW setup
 */
import { describe, it, expect, beforeEach, vi } from 'vitest';
import { renderHook, act, waitFor } from '@testing-library/react';
import { http, HttpResponse } from 'msw';
import { server, addTestHandler } from './server';
import { 
  resetMockStores, 
  createTestSession, 
  mockConnectedState,
  mockApiError,
  mockStreamingResponse,
  createWebSocketEvent
} from './test-helpers';

// Example hook (you would import your actual hook)
const useExampleHook = () => {
  // This is a placeholder - replace with actual hook logic
  const [messages, setMessages] = React.useState([]);
  const [isLoading, setIsLoading] = React.useState(false);
  const [error, setError] = React.useState(null);
  
  const fetchMessages = async (sessionId: string) => {
    setIsLoading(true);
    setError(null);
    
    try {
      const response = await fetch(`/api/sessions/${sessionId}/messages`);
      const data = await response.json();
      
      if (!response.ok) {
        throw new Error(data.error || 'Failed to fetch messages');
      }
      
      setMessages(data.data.messages);
    } catch (err) {
      setError(err.message);
    } finally {
      setIsLoading(false);
    }
  };
  
  return { messages, isLoading, error, fetchMessages };
};

describe('Example: Testing React Hooks with MSW', () => {
  beforeEach(() => {
    // Reset all mock stores before each test
    resetMockStores();
  });
  
  describe('Basic API Mocking', () => {
    it('should fetch messages successfully', async () => {
      // Setup: Create a test session with messages
      const session = createTestSession(3);
      
      // Render the hook
      const { result } = renderHook(() => useExampleHook());
      
      // Act: Fetch messages
      await act(async () => {
        await result.current.fetchMessages(session.id);
      });
      
      // Assert: Check the results
      expect(result.current.isLoading).toBe(false);
      expect(result.current.error).toBeNull();
      expect(result.current.messages).toHaveLength(3);
      expect(result.current.messages[0].content).toBe('Test message 0');
    });
    
    it('should handle API errors gracefully', async () => {
      // Setup: Mock an API error for this specific test
      mockApiError('/api/sessions/invalid/messages', {
        message: 'Session not found',
        status: 404
      });
      
      // Render the hook
      const { result } = renderHook(() => useExampleHook());
      
      // Act: Try to fetch messages with invalid session
      await act(async () => {
        await result.current.fetchMessages('invalid');
      });
      
      // Assert: Error should be handled
      expect(result.current.isLoading).toBe(false);
      expect(result.current.error).toBe('Session not found');
      expect(result.current.messages).toHaveLength(0);
    });
  });
  
  describe('Streaming Responses', () => {
    it('should handle streaming messages', async () => {
      // Setup: Mock a streaming response
      const streamChunks = [
        { type: 'stream_start', messageId: 'msg_1', role: 'assistant' },
        { type: 'stream_chunk', messageId: 'msg_1', content: 'Hello', isComplete: false },
        { type: 'stream_chunk', messageId: 'msg_1', content: 'Hello, how', isComplete: false },
        { type: 'stream_chunk', messageId: 'msg_1', content: 'Hello, how can I help?', isComplete: false },
        { type: 'stream_end', messageId: 'msg_1', content: 'Hello, how can I help?', isComplete: true }
      ];
      
      mockStreamingResponse('/api/sessions/test/stream', streamChunks, 100);
      
      // Your streaming hook test logic here
      // This is just an example structure
    });
  });
  
  describe('WebSocket Events', () => {
    it('should handle WebSocket-like events', async () => {
      // Setup: Mock a connected state
      const sessionId = mockConnectedState();
      
      // Create WebSocket events
      const turnStartEvent = createWebSocketEvent('turn_start', {
        turn_id: 'turn_1',
        turn_type: 'user'
      });
      
      const messageEvent = createWebSocketEvent('message', {
        id: 'msg_1',
        content: 'Test message',
        role: 'assistant'
      });
      
      // Your WebSocket event handling test logic here
    });
  });
  
  describe('Custom Request Handlers', () => {
    it('should work with custom handlers added at runtime', async () => {
      // Add a custom handler for this specific test
      addTestHandler(
        http.get('*/api/custom/endpoint', () => {
          return HttpResponse.json({
            custom: 'response',
            timestamp: new Date().toISOString()
          });
        })
      );
      
      // Test your component that uses this custom endpoint
      const response = await fetch('/api/custom/endpoint');
      const data = await response.json();
      
      expect(data.custom).toBe('response');
      expect(data.timestamp).toBeDefined();
    });
  });
  
  describe('Complex Scenarios', () => {
    it('should handle optimistic updates with rollback', async () => {
      // Setup: Create initial state
      const session = createTestSession(2);
      
      // Mock a delayed failure for optimistic update
      addTestHandler(
        http.post('*/api/sessions/:id/messages', async () => {
          // Delay to simulate network latency
          await new Promise(resolve => setTimeout(resolve, 500));
          
          // Return error after delay
          return HttpResponse.json(
            { error: 'Network error' },
            { status: 503 }
          );
        })
      );
      
      // Your optimistic update test logic here
    });
    
    it('should handle concurrent requests', async () => {
      // Create multiple sessions
      const sessions = Array.from({ length: 3 }, (_, i) => 
        createTestSession(2)
      );
      
      // Test concurrent fetching
      const promises = sessions.map(session => 
        fetch(`/api/sessions/${session.id}/messages`)
      );
      
      const responses = await Promise.all(promises);
      const data = await Promise.all(responses.map(r => r.json()));
      
      // All requests should succeed
      expect(data).toHaveLength(3);
      data.forEach(d => {
        expect(d.success).toBe(true);
        expect(d.data.messages).toHaveLength(2);
      });
    });
  });
  
  describe('Performance Testing', () => {
    it('should handle large datasets efficiently', async () => {
      // Create a large number of sessions for testing
      const largeDataset = Array.from({ length: 1000 }, (_, i) => ({
        id: `session_${i}`,
        name: `Session ${i}`,
        createdAt: new Date().toISOString()
      }));
      
      // Mock the response with large dataset
      addTestHandler(
        http.get('*/api/sessions', () => {
          return HttpResponse.json({
            success: true,
            data: { sessions: largeDataset }
          });
        })
      );
      
      // Measure performance
      const startTime = performance.now();
      
      const response = await fetch('/api/sessions');
      const data = await response.json();
      
      const endTime = performance.now();
      const duration = endTime - startTime;
      
      // Assert performance expectations
      expect(data.data.sessions).toHaveLength(1000);
      expect(duration).toBeLessThan(1000); // Should complete within 1 second
    });
  });
});

// Example: Testing with React Testing Library and MSW
import React from 'react';
import { render, screen, fireEvent, waitFor } from '@testing-library/react';

// Example component that uses hooks
const ChatComponent: React.FC = () => {
  const [messages, setMessages] = React.useState<any[]>([]);
  const [input, setInput] = React.useState('');
  const [isLoading, setIsLoading] = React.useState(false);
  
  const sendMessage = async () => {
    setIsLoading(true);
    
    try {
      const response = await fetch('/api/sessions/current/messages', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ content: input })
      });
      
      const data = await response.json();
      
      if (data.success) {
        setMessages([...messages, data.data.userMessage, data.data.assistantMessage]);
        setInput('');
      }
    } finally {
      setIsLoading(false);
    }
  };
  
  return (
    <div>
      <div data-testid="messages">
        {messages.map((msg, i) => (
          <div key={i} data-testid={`message-${i}`}>
            {msg.role}: {msg.content}
          </div>
        ))}
      </div>
      <input
        data-testid="message-input"
        value={input}
        onChange={(e) => setInput(e.target.value)}
        disabled={isLoading}
      />
      <button
        data-testid="send-button"
        onClick={sendMessage}
        disabled={isLoading || !input}
      >
        {isLoading ? 'Sending...' : 'Send'}
      </button>
    </div>
  );
};

describe('Example: Testing React Components with MSW', () => {
  beforeEach(() => {
    resetMockStores();
    // Create a current session for testing
    createTestSession(0);
  });
  
  it('should send and display messages', async () => {
    // Render the component
    render(<ChatComponent />);
    
    // Type a message
    const input = screen.getByTestId('message-input');
    const sendButton = screen.getByTestId('send-button');
    
    fireEvent.change(input, { target: { value: 'Hello!' } });
    
    // Send the message
    fireEvent.click(sendButton);
    
    // Wait for the response
    await waitFor(() => {
      expect(screen.getByText('user: Hello!')).toBeInTheDocument();
      expect(screen.getByText('assistant: Response to: Hello!')).toBeInTheDocument();
    });
    
    // Input should be cleared
    expect(input).toHaveValue('');
  });
});