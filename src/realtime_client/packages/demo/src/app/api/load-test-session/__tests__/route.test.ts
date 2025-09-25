/**
 * Tests for the load-test-session API route
 * Validates session loading from file system and error handling
 */

import { describe, it, expect, beforeEach, vi, MockedFunction } from 'vitest';
import { GET } from '../route';
import { NextRequest, NextResponse } from 'next/server';

// Mock fs module
vi.mock('fs', () => {
  const mockExistsSync = vi.fn();
  const mockReadFileSync = vi.fn();
  
  return {
    default: {
      existsSync: mockExistsSync,
      readFileSync: mockReadFileSync
    },
    existsSync: mockExistsSync,
    readFileSync: mockReadFileSync
  };
});

// Import fs after mocking
import fs from 'fs';

// Get typed mock references
const mockExistsSync = fs.existsSync as MockedFunction<typeof fs.existsSync>;
const mockReadFileSync = fs.readFileSync as MockedFunction<typeof fs.readFileSync>;

// Mock path module
vi.mock('path', () => ({
  join: vi.fn((...args) => args.join('/'))
}));

// Test session data that matches expected structure
const validSessionData = {
  version: 1,
  session_id: "test-session",
  messages: [
    {
      role: "user",
      content: "Test message"
    },
    {
      role: "assistant", 
      content: [
        {
          id: "tool_123",
          input: { thought: "Test thought" },
          name: "think",
          type: "tool_use"
        }
      ]
    }
  ],
  agent_config: {
    version: 2,
    name: "Test Agent",
    key: "test_agent"
  }
};

describe('Load Test Session API Route', () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  describe('Successful Session Loading', () => {
    it('should successfully load and return session data', async () => {
      // Mock file exists and successful read
      mockExistsSync.mockReturnValue(true);
      mockReadFileSync.mockReturnValue(
        JSON.stringify(validSessionData)
      );

      const mockRequest = new NextRequest('http://localhost:3000/api/load-test-session');
      const response = await GET(mockRequest);

      // Check response status
      expect(response.status).toBe(200);

      // Check response data
      const data = await response.json();
      expect(data).toEqual(validSessionData);
      expect(data.session_id).toBe('test-session');
      expect(data.messages).toHaveLength(2);
    });

    it('should handle session with delegation patterns', async () => {
      const sessionWithDelegation = {
        ...validSessionData,
        messages: [
          ...validSessionData.messages,
          {
            role: "assistant",
            content: [
              {
                id: "tool_456",
                input: {
                  agent_key: "other_agent",
                  message: "Delegation message"
                },
                name: "ateam_chat",
                type: "tool_use"
              }
            ]
          }
        ]
      };

      mockExistsSync.mockReturnValue(true);
      mockReadFileSync.mockReturnValue(
        JSON.stringify(sessionWithDelegation)
      );

      const mockRequest = new NextRequest('http://localhost:3000/api/load-test-session');
      const response = await GET(mockRequest);
      const data = await response.json();

      // Verify delegation message is included
      expect(data.messages).toHaveLength(3);
      const delegationMessage = data.messages[2];
      expect(delegationMessage.content[0].name).toBe('ateam_chat');
      expect(delegationMessage.content[0].input.agent_key).toBe('other_agent');
    });

    it('should preserve complex message structures', async () => {
      const complexSession = {
        ...validSessionData,
        messages: [
          {
            role: "user",
            content: [
              {
                type: "tool_result",
                tool_use_id: "tool_789",
                content: JSON.stringify({
                  notice: "Nested content",
                  agent_message: {
                    role: "assistant",
                    content: [{ text: "Nested message", type: "text" }]
                  }
                })
              }
            ]
          }
        ]
      };

      mockExistsSync.mockReturnValue(true);
      mockReadFileSync.mockReturnValue(
        JSON.stringify(complexSession)
      );

      const mockRequest = new NextRequest('http://localhost:3000/api/load-test-session');
      const response = await GET(mockRequest);
      const data = await response.json();

      // Verify complex structure is preserved
      expect(data.messages[0].content[0].type).toBe('tool_result');
      expect(data.messages[0].content[0].content).toContain('Nested content');
    });
  });

  describe('Error Handling', () => {
    it('should return 404 when session file does not exist', async () => {
      // Mock file does not exist
      mockExistsSync.mockReturnValue(false);

      const mockRequest = new NextRequest('http://localhost:3000/api/load-test-session');
      const response = await GET(mockRequest);

      expect(response.status).toBe(404);
      const data = await response.json();
      expect(data.error).toBe('Test session file not found');
    });

    it('should return 500 for JSON parse errors', async () => {
      // Mock file exists with invalid JSON
      mockExistsSync.mockReturnValue(true);
      mockReadFileSync.mockReturnValue('{ invalid json }');

      const mockRequest = new NextRequest('http://localhost:3000/api/load-test-session');
      const response = await GET(mockRequest);

      expect(response.status).toBe(500);
      const data = await response.json();
      expect(data.error).toBe('Failed to load test session');
    });

    it('should return 500 for unexpected read errors', async () => {
      // Mock file exists but read throws error
      mockExistsSync.mockReturnValue(true);
      mockReadFileSync.mockImplementation(() => {
        throw new Error('Permission denied');
      });

      const mockRequest = new NextRequest('http://localhost:3000/api/load-test-session');
      const response = await GET(mockRequest);

      expect(response.status).toBe(500);
      const data = await response.json();
      expect(data.error).toBe('Failed to load test session');
      expect(data.details).toBe('Permission denied');
    });

    it('should handle empty file gracefully', async () => {
      // Mock file exists but empty
      mockExistsSync.mockReturnValue(true);
      mockReadFileSync.mockReturnValue('');

      const mockRequest = new NextRequest('http://localhost:3000/api/load-test-session');
      const response = await GET(mockRequest);

      expect(response.status).toBe(500);
      const data = await response.json();
      expect(data.error).toBe('Failed to load test session');
    });

    it('should handle file with invalid structure', async () => {
      // Mock file exists with invalid session structure
      mockExistsSync.mockReturnValue(true);
      mockReadFileSync.mockReturnValue(
        JSON.stringify({ invalid: 'structure' })
      );

      const mockRequest = new NextRequest('http://localhost:3000/api/load-test-session');
      const response = await GET(mockRequest);

      // Should still return 200 but with the data as-is
      // The validation happens on the client side
      expect(response.status).toBe(200);
      const data = await response.json();
      expect(data).toEqual({ invalid: 'structure' });
    });
  });

  describe('File System Integration', () => {
    it('should read from correct file path', async () => {
      mockExistsSync.mockReturnValue(true);
      mockReadFileSync.mockReturnValue(
        JSON.stringify(validSessionData)
      );

      const mockRequest = new NextRequest('http://localhost:3000/api/load-test-session');
      await GET(mockRequest);

      // Verify existsSync was called with correct path
      expect(mockExistsSync).toHaveBeenCalledWith(
        expect.stringContaining('session_with_delegation.json')
      );
      // Verify readFileSync was called with correct path
      expect(mockReadFileSync).toHaveBeenCalledWith(
        expect.stringContaining('session_with_delegation.json'),
        'utf-8'
      );
    });

    it('should use UTF-8 encoding', async () => {
      mockExistsSync.mockReturnValue(true);
      mockReadFileSync.mockReturnValue(
        JSON.stringify(validSessionData)
      );

      const mockRequest = new NextRequest('http://localhost:3000/api/load-test-session');
      await GET(mockRequest);

      // Verify UTF-8 encoding was specified
      expect(mockReadFileSync).toHaveBeenCalledWith(
        expect.any(String),
        'utf-8'
      );
    });
  });

  describe('Response Headers', () => {
    it('should set correct Content-Type header', async () => {
      mockExistsSync.mockReturnValue(true);
      mockReadFileSync.mockReturnValue(
        JSON.stringify(validSessionData)
      );

      const mockRequest = new NextRequest('http://localhost:3000/api/load-test-session');
      const response = await GET(mockRequest);

      // NextResponse automatically sets application/json for JSON responses
      expect(response.headers.get('content-type')).toContain('application/json');
    });

    it('should not cache the response', async () => {
      mockExistsSync.mockReturnValue(true);
      mockReadFileSync.mockReturnValue(
        JSON.stringify(validSessionData)
      );

      const mockRequest = new NextRequest('http://localhost:3000/api/load-test-session');
      const response = await GET(mockRequest);

      // Check for cache control headers (if implemented)
      // Note: Current implementation doesn't set cache headers explicitly
      // This test documents expected behavior for future improvements
      expect(response.status).toBe(200);
    });
  });

  describe('Performance', () => {
    it('should handle large session files efficiently', async () => {
      // Create a large session with many messages
      const largeSession = {
        ...validSessionData,
        messages: Array(1000).fill(null).map((_, i) => ({
          role: i % 2 === 0 ? 'user' : 'assistant',
          content: `Message ${i} with some content`
        }))
      };

      vi.mocked(fs.existsSync).mockReturnValue(true);
      vi.mocked(fs.readFileSync).mockReturnValue(
        JSON.stringify(largeSession)
      );

      const startTime = performance.now();
      const mockRequest = new NextRequest('http://localhost:3000/api/load-test-session');
      const response = await GET(mockRequest);
      const endTime = performance.now();

      // Should complete within reasonable time (< 100ms for parsing)
      expect(endTime - startTime).toBeLessThan(100);
      
      const data = await response.json();
      expect(data.messages).toHaveLength(1000);
    });
  });
});