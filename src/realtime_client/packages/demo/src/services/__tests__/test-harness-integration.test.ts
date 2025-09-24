/**
 * Integration test for test harness components
 */

import { describe, it, expect, beforeEach, vi, MockedFunction } from 'vitest';
import { TestSessionManager } from '../test-session-manager';
import { TestDataLoader } from '../test-data-loader';
import { TestSession } from '../../types/test-session';

// Mock fetch for loading JSON files
const mockFetch = vi.fn() as MockedFunction<typeof fetch>;
global.fetch = mockFetch;

describe('Test Harness Integration', () => {
  let sessionManager: TestSessionManager;
  let dataLoader: TestDataLoader;

  beforeEach(() => {
    vi.clearAllMocks();
    // Reset singletons
    (TestSessionManager as any).instance = null;
    (TestDataLoader as any).instance = null;
    
    sessionManager = TestSessionManager.getInstance();
    dataLoader = TestDataLoader.getInstance();
    dataLoader.clearCache();
  });

  it('should load and validate test session structure', async () => {
    // Mock test session data
    const mockSessionData: TestSession = {
      version: 1,
      session_id: 'test-session-123',
      token_count: 100,
      context_window_size: 1000,
      session_name: 'Test Session',
      created_at: '2024-01-01T00:00:00Z',
      updated_at: '2024-01-01T00:00:00Z',
      deleted_at: null,
      user_id: 'test-user',
      metadata: {},
      messages: [
        {
          role: 'user',
          content: 'Hello'
        },
        {
          role: 'assistant',
          content: [
            {
              type: 'text',
              text: 'Hello! How can I help you?'
            }
          ]
        }
      ]
    };

    // Mock fetch to return test data
    mockFetch.mockResolvedValue(
      new Response(JSON.stringify(mockSessionData), {
        status: 200,
        headers: { 'Content-Type': 'application/json' }
      })
    );

    // Load test session
    const session = await dataLoader.loadTestSession('/test-data/test.json');
    
    // Verify session structure
    expect(session).toBeDefined();
    expect(session.session_id).toBe('test-session-123');
    expect(session.messages).toHaveLength(2);
    expect(session.messages[0].role).toBe('user');
    expect(session.messages[1].role).toBe('assistant');
  });

  it('should manage test scenarios correctly', () => {
    // Get predefined scenarios
    const scenarios = sessionManager.getScenarios();
    
    // Verify predefined scenarios exist
    expect(scenarios).toBeDefined();
    expect(scenarios.length).toBeGreaterThan(0);
    
    // Check for specific scenarios
    const delegationScenario = scenarios.find(s => s.id === 'session-with-delegation');
    expect(delegationScenario).toBeDefined();
    expect(delegationScenario?.name).toBe('Session with Delegation');
    expect(delegationScenario?.filePath).toBe('/test-data/session_with_delegation.json');
  });

  it('should handle resume callback registration', async () => {
    const mockCallback = vi.fn();
    
    // Register callback
    sessionManager.setResumeCallback(mockCallback);
    
    // Mock session data
    const mockSession: TestSession = {
      version: 1,
      session_id: 'resume-test',
      token_count: 0,
      context_window_size: 0,
      session_name: null,
      created_at: new Date().toISOString(),
      updated_at: new Date().toISOString(),
      deleted_at: null,
      user_id: 'test',
      metadata: {},
      messages: []
    };
    
    // Mock fetch
    mockFetch.mockResolvedValue(
      new Response(JSON.stringify(mockSession), {
        status: 200,
        headers: { 'Content-Type': 'application/json' }
      })
    );
    
    // Add a custom scenario
    sessionManager.addCustomScenario({
      id: 'test-resume',
      name: 'Test Resume',
      filePath: '/test-data/resume.json'
    });
    
    // Load and resume
    await sessionManager.loadScenario('test-resume');
    await sessionManager.resumeSession('test-resume');
    
    // Verify callback was called
    expect(mockCallback).toHaveBeenCalledWith(mockSession);
  });

  it('should cache loaded sessions', async () => {
    const mockSession: TestSession = {
      version: 1,
      session_id: 'cache-test',
      token_count: 0,
      context_window_size: 0,
      session_name: null,
      created_at: new Date().toISOString(),
      updated_at: new Date().toISOString(),
      deleted_at: null,
      user_id: 'test',
      metadata: {},
      messages: []
    };
    
    mockFetch.mockResolvedValue(
      new Response(JSON.stringify(mockSession), {
        status: 200,
        headers: { 'Content-Type': 'application/json' }
      })
    );
    
    // Load twice
    const session1 = await dataLoader.loadTestSession('/test-data/cache.json');
    const session2 = await dataLoader.loadTestSession('/test-data/cache.json');
    
    // Should only fetch once (cached)
    expect(mockFetch).toHaveBeenCalledTimes(1);
    expect(session1).toBe(session2);
    
    // Verify cache
    const cached = dataLoader.getCachedSession('/test-data/cache.json');
    expect(cached).toBe(session1);
  });

  it('should validate message structure correctly', async () => {
    const invalidSession: TestSession = {
      version: 1,
      session_id: 'invalid-test',
      token_count: 0,
      context_window_size: 0,
      session_name: null,
      created_at: new Date().toISOString(),
      updated_at: new Date().toISOString(),
      deleted_at: null,
      user_id: 'test',
      metadata: {},
      messages: [
        {
          role: 'invalid' as any, // Invalid role
          content: 'Test'
        }
      ]
    };
    
    mockFetch.mockResolvedValue(
      new Response(JSON.stringify(invalidSession), {
        status: 200,
        headers: { 'Content-Type': 'application/json' }
      })
    );
    
    // Should throw validation error
    await expect(dataLoader.loadTestSession('/test-data/invalid.json'))
      .rejects.toThrow('Invalid message role');
  });

  it('should handle delegation tool parsing', () => {
    const delegationContent = `**IMPORTANT**: This is a delegation result
---
text: 'The actual message content from the delegation'`;

    const result = dataLoader.parseDelegationToolResult(delegationContent);
    
    expect(result).toBeDefined();
    expect(result?.preamble).toContain('**IMPORTANT**');
    expect(result?.message.text).toContain('The actual message content');
  });

  it('should correctly identify delegation tools', () => {
    expect(dataLoader.isDelegationTool('act_oneshot')).toBe(true);
    expect(dataLoader.isDelegationTool('ateam_chat')).toBe(true);
    expect(dataLoader.isDelegationTool('aa_command')).toBe(true);
    expect(dataLoader.isDelegationTool('regular_tool')).toBe(false);
  });

  it('should extract agent keys from delegation tools', () => {
    const input = { agent_key: 'test_agent', message: 'Hello' };
    
    expect(dataLoader.getAgentKeyFromTool('ateam_chat', input)).toBe('test_agent');
    expect(dataLoader.getAgentKeyFromTool('aa_oneshot', input)).toBe('test_agent');
    expect(dataLoader.getAgentKeyFromTool('act_oneshot', input)).toBeUndefined();
  });
});