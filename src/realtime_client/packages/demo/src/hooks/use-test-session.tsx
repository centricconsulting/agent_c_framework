/**
 * Test session hook for integrating test mode with chat functionality
 */

import { useEffect, useState, useCallback, useRef } from 'react';
import { useChat, useChatSessionList, useConnection } from '@agentc/realtime-react';
import { TestSession, TestScenario, TestModeConfig } from '@/types/test-session';
import { testSessionManager } from '@/services/test-session-manager';

/**
 * Hook return type
 */
export interface UseTestSessionReturn {
  // Test mode state
  testModeEnabled: boolean;
  testConfig: TestModeConfig;
  
  // Scenarios
  scenarios: TestScenario[];
  currentScenario: TestScenario | null;
  
  // Actions
  enableTestMode: (enabled: boolean) => void;
  loadScenario: (scenarioId: string) => Promise<void>;
  clearSession: () => void;
  reloadCurrentScenario: () => Promise<void>;
  
  // UI controls
  showTestControls: boolean;
  setShowTestControls: (show: boolean) => void;
  
  // State
  isLoading: boolean;
  error: string | null;
}

/**
 * Logger for hook operations
 */
const log = {
  info: (message: string, ...args: any[]) => 
    console.log(`[useTestSession] ${message}`, ...args),
  error: (message: string, ...args: any[]) => 
    console.error(`[useTestSession] ERROR: ${message}`, ...args),
  warn: (message: string, ...args: any[]) => 
    console.warn(`[useTestSession] WARN: ${message}`, ...args),
};

/**
 * Hook for managing test sessions
 */
export function useTestSession(): UseTestSessionReturn {
  const { clearMessages } = useChat();
  const { selectSession } = useChatSessionList();
  const { isConnected } = useConnection();
  
  const [testModeEnabled, setTestModeEnabled] = useState(() => 
    testSessionManager.isEnabled()
  );
  const [testConfig, setTestConfig] = useState<TestModeConfig>(() => 
    testSessionManager.getConfig()
  );
  const [scenarios, setScenarios] = useState<TestScenario[]>(() => 
    testSessionManager.getScenarios()
  );
  const [currentScenario, setCurrentScenario] = useState<TestScenario | null>(() => 
    testSessionManager.getCurrentScenario()
  );
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [showTestControls, setShowTestControls] = useState(() => 
    testConfig.showTestControls ?? true
  );

  // Track if we've registered the resume callback
  const callbackRegistered = useRef(false);

  /**
   * Register resume callback
   */
  useEffect(() => {
    if (!callbackRegistered.current) {
      testSessionManager.setResumeCallback(async (session: TestSession) => {
        log.info('Resume callback triggered', session.session_id);

        try {
          // Select the session to resume it
          // This will trigger the server to load the session and its messages
          await selectSession(session.session_id);

          // The React package's useChat hook will handle loading the messages
          // through the EventStreamProcessor.mapResumedMessagesToEvents() method
          // when it receives the session-messages-loaded event
          
          log.info('Session resume initiated successfully');
        } catch (err) {
          log.error('Failed to resume session:', err);
          throw err;
        }
      });

      callbackRegistered.current = true;
    }
  }, [selectSession]);

  /**
   * Enable or disable test mode
   */
  const enableTestMode = useCallback((enabled: boolean) => {
    testSessionManager.setTestMode(enabled);
    setTestModeEnabled(enabled);
    setTestConfig(testSessionManager.getConfig());
    
    if (!enabled) {
      // Clear test state when disabling
      setCurrentScenario(null);
      testSessionManager.clearCurrentSession();
    }
    
    log.info(`Test mode ${enabled ? 'enabled' : 'disabled'}`);
  }, []);

  /**
   * Load a test scenario
   */
  const loadScenario = useCallback(async (scenarioId: string) => {
    if (!isConnected) {
      const errorMsg = 'Cannot load scenario: Not connected to server';
      setError(errorMsg);
      log.error(errorMsg);
      return;
    }

    setIsLoading(true);
    setError(null);

    try {
      log.info(`Loading scenario: ${scenarioId}`);

      // Load the scenario data
      const session = await testSessionManager.loadScenario(scenarioId);

      // Resume the session using the manager
      await testSessionManager.resumeSession(session);

      // Update state
      setCurrentScenario(testSessionManager.getCurrentScenario());
      setTestConfig(testSessionManager.getConfig());

      log.info('Scenario loaded successfully');
    } catch (err) {
      const errorMsg = `Failed to load scenario: ${err}`;
      setError(errorMsg);
      log.error(errorMsg, err);
    } finally {
      setIsLoading(false);
    }
  }, [isConnected]);

  /**
   * Clear current session
   */
  const clearSession = useCallback(() => {
    clearMessages();
    testSessionManager.clearCurrentSession();
    setCurrentScenario(null);
    setTestConfig(testSessionManager.getConfig());
    log.info('Session cleared');
  }, [clearMessages]);

  /**
   * Reload current scenario
   */
  const reloadCurrentScenario = useCallback(async () => {
    if (!currentScenario) {
      setError('No scenario currently loaded');
      return;
    }

    log.info('Reloading current scenario');
    
    // Clear caches to force reload
    testSessionManager.clearCaches();
    
    // Reload the scenario
    await loadScenario(currentScenario.id);
  }, [currentScenario, loadScenario]);

  /**
   * Update test controls visibility
   */
  const handleSetShowTestControls = useCallback((show: boolean) => {
    testSessionManager.setShowTestControls(show);
    setShowTestControls(show);
    setTestConfig(testSessionManager.getConfig());
  }, []);

  /**
   * Sync with manager state on mount and when test mode changes
   */
  useEffect(() => {
    const syncState = () => {
      setTestConfig(testSessionManager.getConfig());
      setScenarios(testSessionManager.getScenarios());
      setCurrentScenario(testSessionManager.getCurrentScenario());
    };

    syncState();

    // Re-sync when test mode changes
    const interval = setInterval(() => {
      const currentEnabled = testSessionManager.isEnabled();
      if (currentEnabled !== testModeEnabled) {
        setTestModeEnabled(currentEnabled);
        syncState();
      }
    }, 1000);

    return () => clearInterval(interval);
  }, [testModeEnabled]);

  /**
   * Auto-load scenario on mount if configured
   */
  useEffect(() => {
    if (testConfig.autoLoadOnMount && 
        testConfig.currentScenarioId && 
        !currentScenario &&
        !isLoading &&
        isConnected) {
      log.info('Auto-loading scenario on mount');
      loadScenario(testConfig.currentScenarioId);
    }
  }, [testConfig.autoLoadOnMount, testConfig.currentScenarioId, currentScenario, isLoading, isConnected, loadScenario]);

  return {
    // Test mode state
    testModeEnabled,
    testConfig,
    
    // Scenarios
    scenarios,
    currentScenario,
    
    // Actions
    enableTestMode,
    loadScenario,
    clearSession,
    reloadCurrentScenario,
    
    // UI controls
    showTestControls,
    setShowTestControls: handleSetShowTestControls,
    
    // State
    isLoading,
    error
  };
}