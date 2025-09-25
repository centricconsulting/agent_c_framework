/**
 * Session Switching Test Harness
 * 
 * Comprehensive testing utilities for validating session switching behavior,
 * particularly focused on verifying that messages clear properly when switching
 * between sessions.
 */

import { useState, useEffect, useRef, useCallback, useMemo } from 'react';
import { useChat, useChatSessionList, useConnection, isMessageItem } from '@agentc/realtime-react';
import type { ChatItem, MessageChatItem } from '@agentc/realtime-react';

/**
 * Test scenario types
 */
export type TestScenario = 
  | 'rapid-switching'
  | 'empty-session'
  | 'populated-session'
  | 'loading-state';

/**
 * Test result for a single scenario
 */
export interface TestResult {
  scenario: TestScenario;
  passed: boolean;
  timestamp: number;
  duration: number;
  details: {
    expectedBehavior: string;
    actualBehavior: string;
    issues: string[];
  };
  messageTrace?: MessageTraceEntry[];
}

/**
 * Message trace entry for debugging
 */
export interface MessageTraceEntry {
  timestamp: number;
  event: 'added' | 'cleared' | 'leaked' | 'accumulated';
  sessionId: string | null;
  messageCount: number;
  messageIds: string[];
  description: string;
}

/**
 * Session switch test configuration
 */
export interface SessionSwitchTestConfig {
  /** Number of rapid switches to perform */
  rapidSwitchCount?: number;
  /** Delay between rapid switches in ms */
  rapidSwitchDelay?: number;
  /** Timeout for waiting for messages to load */
  messageLoadTimeout?: number;
  /** Enable detailed message tracing */
  enableTracing?: boolean;
}

/**
 * Test harness state
 */
interface TestHarnessState {
  isRunning: boolean;
  currentScenario: TestScenario | null;
  results: TestResult[];
  messageTrace: MessageTraceEntry[];
  testSessions: TestSession[];
  errors: string[];
}

/**
 * Test session data
 */
interface TestSession {
  id: string;
  name: string;
  expectedMessageCount: number;
  messages: ChatItem[];
  createdAt: number;
}

/**
 * Session Switch Test Hook
 * 
 * Provides comprehensive testing utilities for validating session switching behavior.
 */
export function useSessionSwitchTest(config: SessionSwitchTestConfig = {}) {
  const {
    rapidSwitchCount = 5,
    rapidSwitchDelay = 100,
    messageLoadTimeout = 3000,
    enableTracing = true
  } = config;

  // SDK hooks
  const { messages, currentSessionId, clearMessages, error: chatError } = useChat();
  const { 
    sessions, 
    selectSession, 
    currentSessionId: listSessionId,
    isLoading: isLoadingSessions 
  } = useChatSessionList();
  const { isConnected } = useConnection();

  // Test harness state
  const [state, setState] = useState<TestHarnessState>({
    isRunning: false,
    currentScenario: null,
    results: [],
    messageTrace: [],
    testSessions: [],
    errors: []
  });

  // Refs for tracking state during tests
  const previousMessagesRef = useRef<ChatItem[]>([]);
  const previousSessionIdRef = useRef<string | null>(null);
  const messageAccumulatorRef = useRef<Map<string, Set<string>>>(new Map());
  const scenarioStartTimeRef = useRef<number>(0);

  /**
   * Add a message trace entry
   */
  const addTrace = useCallback((entry: Omit<MessageTraceEntry, 'timestamp'>) => {
    if (!enableTracing) return;
    
    setState(prev => ({
      ...prev,
      messageTrace: [
        ...prev.messageTrace,
        { ...entry, timestamp: Date.now() }
      ]
    }));
  }, [enableTracing]);

  /**
   * Detect message leakage between sessions
   */
  const detectMessageLeakage = useCallback((
    oldMessages: ChatItem[],
    newMessages: ChatItem[],
    oldSessionId: string | null,
    newSessionId: string | null
  ): string[] => {
    const issues: string[] = [];
    
    if (oldSessionId === newSessionId) {
      return issues;
    }

    // Check if any old messages appear in new session
    const oldMessageIds = new Set(oldMessages.map(m => m.id).filter(Boolean));
    const newMessageIds = new Set(newMessages.map(m => m.id).filter(Boolean));
    
    const leakedIds = Array.from(oldMessageIds).filter(id => newMessageIds.has(id));
    
    if (leakedIds.length > 0) {
      issues.push(`Message leakage detected: ${leakedIds.length} messages from session ${oldSessionId} appear in session ${newSessionId}`);
      
      addTrace({
        event: 'leaked',
        sessionId: newSessionId,
        messageCount: leakedIds.length,
        messageIds: leakedIds as string[],
        description: `Messages leaked from ${oldSessionId} to ${newSessionId}`
      });
    }

    return issues;
  }, [addTrace]);

  /**
   * Detect message accumulation (messages not clearing)
   */
  const detectMessageAccumulation = useCallback((
    sessionId: string | null,
    messages: ChatItem[]
  ): string[] => {
    const issues: string[] = [];
    
    if (!sessionId) return issues;

    const messageIds = new Set(messages.map(m => m.id).filter(Boolean));
    
    // Check if we've seen this session before
    if (messageAccumulatorRef.current.has(sessionId)) {
      const previousIds = messageAccumulatorRef.current.get(sessionId)!;
      const unexpectedIds = Array.from(messageIds).filter(
        id => !previousIds.has(id as string)
      );
      
      // In a properly functioning system, switching back to a session
      // should show the same messages, not accumulated ones
      if (messageIds.size > previousIds.size + 1) { // +1 for possible new message
        issues.push(`Message accumulation detected: Session ${sessionId} has ${messageIds.size - previousIds.size} extra messages`);
        
        addTrace({
          event: 'accumulated',
          sessionId,
          messageCount: messageIds.size,
          messageIds: Array.from(messageIds) as string[],
          description: `Unexpected message accumulation in session ${sessionId}`
        });
      }
    }
    
    // Update accumulator
    messageAccumulatorRef.current.set(sessionId, messageIds as Set<string>);
    
    return issues;
  }, [addTrace]);

  /**
   * Monitor message changes
   */
  useEffect(() => {
    // Skip if not running tests
    if (!state.isRunning) return;

    // Detect changes
    const oldMessages = previousMessagesRef.current;
    const oldSessionId = previousSessionIdRef.current;
    
    if (currentSessionId !== oldSessionId) {
      // Session changed
      addTrace({
        event: 'cleared',
        sessionId: currentSessionId,
        messageCount: messages.length,
        messageIds: messages.map(m => m.id).filter(Boolean) as string[],
        description: `Session switched from ${oldSessionId} to ${currentSessionId}`
      });
      
      // Check for issues
      const leakageIssues = detectMessageLeakage(
        oldMessages,
        messages,
        oldSessionId,
        currentSessionId
      );
      
      const accumulationIssues = detectMessageAccumulation(
        currentSessionId,
        messages
      );
      
      if (leakageIssues.length > 0 || accumulationIssues.length > 0) {
        setState(prev => ({
          ...prev,
          errors: [...prev.errors, ...leakageIssues, ...accumulationIssues]
        }));
      }
    } else if (messages.length !== oldMessages.length) {
      // Messages changed within same session
      addTrace({
        event: messages.length > oldMessages.length ? 'added' : 'cleared',
        sessionId: currentSessionId,
        messageCount: messages.length,
        messageIds: messages.map(m => m.id).filter(Boolean) as string[],
        description: `Message count changed from ${oldMessages.length} to ${messages.length}`
      });
    }
    
    // Update refs
    previousMessagesRef.current = [...messages];
    previousSessionIdRef.current = currentSessionId;
  }, [
    messages, 
    currentSessionId, 
    state.isRunning,
    addTrace,
    detectMessageLeakage,
    detectMessageAccumulation
  ]);

  /**
   * Wait for condition with timeout
   */
  const waitForCondition = useCallback(async (
    condition: () => boolean,
    timeout: number = messageLoadTimeout,
    description: string = 'condition'
  ): Promise<boolean> => {
    const startTime = Date.now();
    
    while (!condition() && Date.now() - startTime < timeout) {
      await new Promise(resolve => setTimeout(resolve, 50));
    }
    
    const success = condition();
    if (!success) {
      console.warn(`Timeout waiting for ${description}`);
    }
    
    return success;
  }, [messageLoadTimeout]);

  /**
   * Create a test result
   */
  const createResult = useCallback((
    scenario: TestScenario,
    passed: boolean,
    expectedBehavior: string,
    actualBehavior: string,
    issues: string[]
  ): TestResult => {
    const duration = Date.now() - scenarioStartTimeRef.current;
    
    return {
      scenario,
      passed,
      timestamp: Date.now(),
      duration,
      details: {
        expectedBehavior,
        actualBehavior,
        issues
      },
      messageTrace: enableTracing ? [...state.messageTrace] : undefined
    };
  }, [enableTracing, state.messageTrace]);

  /**
   * Test Scenario: Rapid Session Switching
   */
  const testRapidSwitching = useCallback(async (): Promise<TestResult> => {
    console.log('[SessionSwitchTest] Starting rapid switching test...');
    scenarioStartTimeRef.current = Date.now();
    
    const issues: string[] = [];
    const sessionIds: string[] = sessions.slice(0, Math.min(rapidSwitchCount, sessions.length))
      .map(s => s.session_id);
    
    if (sessionIds.length < 2) {
      return createResult(
        'rapid-switching',
        false,
        'Switch rapidly between multiple sessions',
        'Not enough sessions available for testing',
        ['Need at least 2 sessions for rapid switching test']
      );
    }

    // Clear accumulator for fresh test
    messageAccumulatorRef.current.clear();
    
    // Perform rapid switches
    for (let i = 0; i < rapidSwitchCount; i++) {
      const targetSessionId = sessionIds[i % sessionIds.length];
      
      console.log(`[SessionSwitchTest] Switch ${i + 1}/${rapidSwitchCount} to ${targetSessionId}`);
      selectSession(targetSessionId);
      
      // Wait briefly for switch to take effect
      await new Promise(resolve => setTimeout(resolve, rapidSwitchDelay));
      
      // Check that session actually switched
      if (currentSessionId !== targetSessionId) {
        issues.push(`Switch ${i + 1}: Failed to switch to session ${targetSessionId}`);
      }
      
      // Check for message issues (detection happens in useEffect)
    }
    
    // Wait for final messages to settle
    await new Promise(resolve => setTimeout(resolve, 500));
    
    const passed = issues.length === 0 && state.errors.length === 0;
    
    return createResult(
      'rapid-switching',
      passed,
      'Messages should clear completely on each session switch with no leakage',
      passed 
        ? 'All session switches completed cleanly'
        : `Issues detected during rapid switching: ${[...issues, ...state.errors].join('; ')}`,
      [...issues, ...state.errors]
    );
  }, [
    sessions,
    rapidSwitchCount,
    rapidSwitchDelay,
    selectSession,
    currentSessionId,
    state.errors,
    createResult
  ]);

  /**
   * Test Scenario: Empty Session
   */
  const testEmptySession = useCallback(async (): Promise<TestResult> => {
    console.log('[SessionSwitchTest] Starting empty session test...');
    scenarioStartTimeRef.current = Date.now();
    
    const issues: string[] = [];
    
    // First, switch to a populated session
    const populatedSession = sessions.find(s => s.session_id !== currentSessionId);
    if (!populatedSession) {
      return createResult(
        'empty-session',
        false,
        'Switch from populated to empty session',
        'No other sessions available',
        ['Need at least 2 sessions for empty session test']
      );
    }
    
    // Switch to populated session
    selectSession(populatedSession.session_id);
    await waitForCondition(
      () => currentSessionId === populatedSession.session_id,
      2000,
      'session switch to populated'
    );
    
    // Record message count
    const populatedMessageCount = messages.length;
    console.log(`[SessionSwitchTest] Populated session has ${populatedMessageCount} messages`);
    
    // Create a new session (which should be empty)
    // For this test, we'll switch to a session that likely has no messages
    // In a real implementation, you might create a new session programmatically
    const emptySessionCandidate = sessions[sessions.length - 1]; // Newest session, likely empty
    
    if (emptySessionCandidate) {
      selectSession(emptySessionCandidate.session_id);
      await waitForCondition(
        () => currentSessionId === emptySessionCandidate.session_id,
        2000,
        'session switch to empty'
      );
      
      // Wait for messages to potentially load
      await new Promise(resolve => setTimeout(resolve, 500));
      
      // Check that messages cleared
      if (messages.length > 0) {
        // Check if these are legitimately from the new session
        const hasOldMessages = messages.some(m => {
          if (isMessageItem(m)) {
            return m.metadata?.sessionId === populatedSession.session_id;
          }
          return false;
        });
        
        if (hasOldMessages) {
          issues.push(`Messages from previous session (${populatedSession.session_id}) still present`);
        }
      }
      
      console.log(`[SessionSwitchTest] Empty session has ${messages.length} messages`);
    }
    
    const passed = issues.length === 0 && state.errors.length === 0;
    
    return createResult(
      'empty-session',
      passed,
      'Messages should clear when switching to an empty session',
      passed
        ? 'Messages cleared successfully when switching to empty session'
        : `Issues detected: ${[...issues, ...state.errors].join('; ')}`,
      [...issues, ...state.errors]
    );
  }, [
    sessions,
    currentSessionId,
    messages,
    selectSession,
    waitForCondition,
    state.errors,
    createResult
  ]);

  /**
   * Test Scenario: Populated Session
   */
  const testPopulatedSession = useCallback(async (): Promise<TestResult> => {
    console.log('[SessionSwitchTest] Starting populated session test...');
    scenarioStartTimeRef.current = Date.now();
    
    const issues: string[] = [];
    
    // Get two different sessions
    const session1 = sessions[0];
    const session2 = sessions[1];
    
    if (!session1 || !session2) {
      return createResult(
        'populated-session',
        false,
        'Switch between populated sessions',
        'Not enough sessions available',
        ['Need at least 2 sessions for populated session test']
      );
    }
    
    // Clear accumulator for fresh test
    messageAccumulatorRef.current.clear();
    
    // Switch to first session
    selectSession(session1.session_id);
    await waitForCondition(
      () => currentSessionId === session1.session_id,
      2000,
      'switch to session 1'
    );
    
    // Wait for messages to load
    await new Promise(resolve => setTimeout(resolve, 500));
    
    const session1Messages = [...messages];
    const session1MessageIds = new Set(
      session1Messages.map(m => m.id).filter(Boolean)
    );
    
    console.log(`[SessionSwitchTest] Session 1 has ${session1Messages.length} messages`);
    
    // Switch to second session
    selectSession(session2.session_id);
    await waitForCondition(
      () => currentSessionId === session2.session_id,
      2000,
      'switch to session 2'
    );
    
    // Wait for messages to load
    await new Promise(resolve => setTimeout(resolve, 500));
    
    const session2Messages = [...messages];
    const session2MessageIds = new Set(
      session2Messages.map(m => m.id).filter(Boolean)
    );
    
    console.log(`[SessionSwitchTest] Session 2 has ${session2Messages.length} messages`);
    
    // Check for message mixing
    const commonIds = Array.from(session1MessageIds).filter(id => 
      session2MessageIds.has(id as string)
    );
    
    if (commonIds.length > 0) {
      issues.push(`${commonIds.length} messages appear in both sessions - possible leakage`);
    }
    
    // Switch back to first session
    selectSession(session1.session_id);
    await waitForCondition(
      () => currentSessionId === session1.session_id,
      2000,
      'switch back to session 1'
    );
    
    // Wait for messages to load
    await new Promise(resolve => setTimeout(resolve, 500));
    
    const session1MessagesReturn = [...messages];
    
    console.log(`[SessionSwitchTest] Session 1 return has ${session1MessagesReturn.length} messages`);
    
    // Check that we have the same messages as before (not accumulated)
    if (session1MessagesReturn.length > session1Messages.length + 1) {
      issues.push(`Session 1 has ${session1MessagesReturn.length - session1Messages.length} extra messages after switching back`);
    }
    
    const passed = issues.length === 0 && state.errors.length === 0;
    
    return createResult(
      'populated-session',
      passed,
      'Each session should show only its own messages',
      passed
        ? 'Sessions maintained separate message lists correctly'
        : `Issues detected: ${[...issues, ...state.errors].join('; ')}`,
      [...issues, ...state.errors]
    );
  }, [
    sessions,
    currentSessionId,
    messages,
    selectSession,
    waitForCondition,
    state.errors,
    createResult
  ]);

  /**
   * Test Scenario: Loading State
   */
  const testLoadingState = useCallback(async (): Promise<TestResult> => {
    console.log('[SessionSwitchTest] Starting loading state test...');
    scenarioStartTimeRef.current = Date.now();
    
    const issues: string[] = [];
    let loadingStateObserved = false;
    let messagessDuringLoading: ChatItem[] = [];
    
    const targetSession = sessions.find(s => s.session_id !== currentSessionId);
    if (!targetSession) {
      return createResult(
        'loading-state',
        false,
        'Monitor loading state during session switch',
        'No other sessions available',
        ['Need at least 2 sessions for loading state test']
      );
    }
    
    // Monitor loading state
    const checkInterval = setInterval(() => {
      if (isLoadingSessions) {
        loadingStateObserved = true;
        messagessDuringLoading = [...messages];
      }
    }, 10);
    
    // Perform session switch
    selectSession(targetSession.session_id);
    
    // Wait for switch to complete
    await waitForCondition(
      () => currentSessionId === targetSession.session_id,
      2000,
      'session switch'
    );
    
    // Wait a bit more to ensure loading completes
    await new Promise(resolve => setTimeout(resolve, 500));
    
    clearInterval(checkInterval);
    
    // Check if messages leaked during loading
    if (messagessDuringLoading.length > 0 && loadingStateObserved) {
      const duringLoadingIds = new Set(
        messagessDuringLoading.map(m => m.id).filter(Boolean)
      );
      const finalIds = new Set(
        messages.map(m => m.id).filter(Boolean)
      );
      
      const leakedDuringLoading = Array.from(duringLoadingIds).filter(id =>
        !finalIds.has(id as string)
      );
      
      if (leakedDuringLoading.length > 0) {
        issues.push(`${leakedDuringLoading.length} messages appeared during loading but disappeared after`);
      }
    }
    
    const passed = issues.length === 0 && state.errors.length === 0;
    
    return createResult(
      'loading-state',
      passed,
      'No messages should leak through during loading transition',
      passed
        ? 'Loading state handled correctly with no message leakage'
        : `Issues detected: ${[...issues, ...state.errors].join('; ')}`,
      [...issues, ...state.errors]
    );
  }, [
    sessions,
    currentSessionId,
    messages,
    isLoadingSessions,
    selectSession,
    waitForCondition,
    state.errors,
    createResult
  ]);

  /**
   * Run a specific test scenario
   */
  const runScenario = useCallback(async (scenario: TestScenario): Promise<TestResult> => {
    // Clear previous errors
    setState(prev => ({
      ...prev,
      isRunning: true,
      currentScenario: scenario,
      errors: [],
      messageTrace: enableTracing ? [] : prev.messageTrace
    }));

    let result: TestResult;
    
    try {
      switch (scenario) {
        case 'rapid-switching':
          result = await testRapidSwitching();
          break;
        case 'empty-session':
          result = await testEmptySession();
          break;
        case 'populated-session':
          result = await testPopulatedSession();
          break;
        case 'loading-state':
          result = await testLoadingState();
          break;
        default:
          throw new Error(`Unknown scenario: ${scenario}`);
      }
    } catch (error) {
      result = createResult(
        scenario,
        false,
        'Test should complete without errors',
        `Test failed with error: ${error}`,
        [error instanceof Error ? error.message : String(error)]
      );
    }
    
    // Update state with result
    setState(prev => ({
      ...prev,
      isRunning: false,
      currentScenario: null,
      results: [...prev.results, result]
    }));
    
    return result;
  }, [
    enableTracing,
    testRapidSwitching,
    testEmptySession,
    testPopulatedSession,
    testLoadingState,
    createResult
  ]);

  /**
   * Run all test scenarios
   */
  const runAllScenarios = useCallback(async (): Promise<TestResult[]> => {
    const scenarios: TestScenario[] = [
      'rapid-switching',
      'empty-session',
      'populated-session',
      'loading-state'
    ];
    
    const results: TestResult[] = [];
    
    for (const scenario of scenarios) {
      console.log(`[SessionSwitchTest] Running scenario: ${scenario}`);
      const result = await runScenario(scenario);
      results.push(result);
      
      // Brief pause between scenarios
      await new Promise(resolve => setTimeout(resolve, 1000));
    }
    
    return results;
  }, [runScenario]);

  /**
   * Clear test results
   */
  const clearResults = useCallback(() => {
    setState(prev => ({
      ...prev,
      results: [],
      messageTrace: [],
      errors: []
    }));
    messageAccumulatorRef.current.clear();
  }, []);

  /**
   * Get summary of test results
   */
  const summary = useMemo(() => {
    const total = state.results.length;
    const passed = state.results.filter(r => r.passed).length;
    const failed = total - passed;
    const scenarios = {
      'rapid-switching': state.results.find(r => r.scenario === 'rapid-switching'),
      'empty-session': state.results.find(r => r.scenario === 'empty-session'),
      'populated-session': state.results.find(r => r.scenario === 'populated-session'),
      'loading-state': state.results.find(r => r.scenario === 'loading-state')
    };
    
    return {
      total,
      passed,
      failed,
      successRate: total > 0 ? (passed / total) * 100 : 0,
      scenarios,
      allPassed: total > 0 && failed === 0
    };
  }, [state.results]);

  return {
    // State
    isRunning: state.isRunning,
    currentScenario: state.currentScenario,
    results: state.results,
    messageTrace: state.messageTrace,
    errors: state.errors,
    summary,
    
    // Test controls
    runScenario,
    runAllScenarios,
    clearResults,
    
    // Connection state
    isConnected,
    isReady: isConnected && !isLoadingSessions && sessions.length >= 2,
    
    // Current state
    currentSessionId,
    messageCount: messages.length,
    sessionCount: sessions.length
  };
}

/**
 * Export types for external use
 */
export type UseSessionSwitchTestReturn = ReturnType<typeof useSessionSwitchTest>;