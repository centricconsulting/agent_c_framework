/**
 * Test Session Manager Service
 * Manages test scenarios and handles session resumption
 */

import { TestSession, TestScenario, TestModeConfig } from '@/types/test-session';
import { testDataLoader } from './test-data-loader';

// Predefined test scenarios
export const PREDEFINED_SCENARIOS: TestScenario[] = [
  {
    id: 'session-with-delegation',
    name: 'Session with Delegation',
    description: 'Test session containing delegation tool calls and subsessions',
    filePath: '/test-data/session_with_delegation.json',
    tags: ['delegation', 'tools', 'subsession']
  },
  {
    id: 'simple-chat',
    name: 'Simple Chat',
    description: 'Basic chat interaction without tools',
    filePath: '/test-data/simple_chat.json',
    tags: ['basic', 'chat']
  },
  {
    id: 'streaming-test',
    name: 'Streaming Test',
    description: 'Test session for validating streaming message rendering',
    filePath: '/test-data/streaming_test.json',
    tags: ['streaming', 'rendering']
  },
  {
    id: 'error-handling',
    name: 'Error Handling',
    description: 'Test session with error events',
    filePath: '/test-data/error_handling.json',
    tags: ['error', 'edge-case']
  }
];

/**
 * Logger for test session operations
 */
const log = {
  info: (message: string, ...args: any[]) => 
    console.log(`[TestSessionManager] ${message}`, ...args),
  error: (message: string, ...args: any[]) => 
    console.error(`[TestSessionManager] ERROR: ${message}`, ...args),
  warn: (message: string, ...args: any[]) => 
    console.warn(`[TestSessionManager] WARN: ${message}`, ...args),
};

/**
 * Test session manager
 */
export class TestSessionManager {
  private static instance: TestSessionManager | null = null;
  private scenarios: Map<string, TestScenario> = new Map();
  private currentScenarioId: string | null = null;
  private config: TestModeConfig;
  private resumeSessionCallback: ((session: TestSession) => Promise<void>) | null = null;

  private constructor() {
    // Initialize with default config
    this.config = {
      enabled: this.isTestModeEnabled(),
      scenarios: [],
      showTestControls: true,
      autoLoadOnMount: false
    };

    // Load predefined scenarios
    this.initializeScenarios();
  }

  /**
   * Get singleton instance
   */
  public static getInstance(): TestSessionManager {
    if (!TestSessionManager.instance) {
      TestSessionManager.instance = new TestSessionManager();
    }
    return TestSessionManager.instance;
  }

  /**
   * Check if test mode is enabled via environment or localStorage
   */
  private isTestModeEnabled(): boolean {
    // Check environment variable
    if (typeof process !== 'undefined' && process.env.NEXT_PUBLIC_TEST_MODE === 'true') {
      return true;
    }

    // Check localStorage for runtime toggle
    if (typeof window !== 'undefined') {
      const testMode = localStorage.getItem('agentc_test_mode');
      return testMode === 'true';
    }

    return false;
  }

  /**
   * Initialize scenarios
   */
  private initializeScenarios(): void {
    // Add predefined scenarios
    PREDEFINED_SCENARIOS.forEach(scenario => {
      this.scenarios.set(scenario.id, scenario);
    });

    // Update config
    this.config.scenarios = Array.from(this.scenarios.values());
    
    log.info(`Initialized with ${this.scenarios.size} test scenarios`);
  }

  /**
   * Enable or disable test mode
   */
  public setTestMode(enabled: boolean): void {
    this.config.enabled = enabled;

    // Persist to localStorage for runtime toggle
    if (typeof window !== 'undefined') {
      localStorage.setItem('agentc_test_mode', enabled.toString());
    }

    log.info(`Test mode ${enabled ? 'enabled' : 'disabled'}`);
  }

  /**
   * Check if test mode is enabled
   */
  public isEnabled(): boolean {
    return this.config.enabled;
  }

  /**
   * Get current configuration
   */
  public getConfig(): TestModeConfig {
    return { ...this.config };
  }

  /**
   * Set the session resume callback
   */
  public setResumeCallback(callback: (session: TestSession) => Promise<void>): void {
    this.resumeSessionCallback = callback;
    log.info('Resume callback registered');
  }

  /**
   * Get all available scenarios
   */
  public getScenarios(): TestScenario[] {
    return Array.from(this.scenarios.values());
  }

  /**
   * Get current scenario
   */
  public getCurrentScenario(): TestScenario | null {
    if (!this.currentScenarioId) {
      return null;
    }
    return this.scenarios.get(this.currentScenarioId) || null;
  }

  /**
   * Load a test scenario
   */
  public async loadScenario(scenarioId: string): Promise<TestSession> {
    log.info(`Loading scenario: ${scenarioId}`);

    const scenario = this.scenarios.get(scenarioId);
    if (!scenario) {
      throw new Error(`Scenario not found: ${scenarioId}`);
    }

    try {
      // Load session data if not already loaded
      if (!scenario.sessionData) {
        scenario.sessionData = await testDataLoader.loadTestSession(scenario.filePath);
      }

      // Update current scenario
      this.currentScenarioId = scenarioId;
      this.config.currentScenarioId = scenarioId;

      log.info(`Successfully loaded scenario: ${scenario.name}`);
      return scenario.sessionData;
    } catch (error) {
      log.error(`Failed to load scenario ${scenarioId}:`, error);
      throw error;
    }
  }

  /**
   * Resume a test session
   */
  public async resumeSession(sessionOrId: TestSession | string): Promise<void> {
    log.info('Resuming test session');

    if (!this.resumeSessionCallback) {
      throw new Error('No resume callback registered. Cannot resume session.');
    }

    try {
      let session: TestSession;

      if (typeof sessionOrId === 'string') {
        // Load scenario by ID
        session = await this.loadScenario(sessionOrId);
      } else {
        // Use provided session
        session = sessionOrId;
      }

      // Call the resume callback
      await this.resumeSessionCallback(session);
      
      log.info(`Successfully resumed session: ${session.session_id}`);
    } catch (error) {
      log.error('Failed to resume session:', error);
      throw error;
    }
  }

  /**
   * Clear current session
   */
  public clearCurrentSession(): void {
    this.currentScenarioId = null;
    this.config.currentScenarioId = undefined;
    log.info('Cleared current session');
  }

  /**
   * Add a custom scenario
   */
  public addCustomScenario(scenario: TestScenario): void {
    this.scenarios.set(scenario.id, scenario);
    this.config.scenarios = Array.from(this.scenarios.values());
    log.info(`Added custom scenario: ${scenario.name}`);
  }

  /**
   * Remove a scenario
   */
  public removeScenario(scenarioId: string): boolean {
    const result = this.scenarios.delete(scenarioId);
    if (result) {
      this.config.scenarios = Array.from(this.scenarios.values());
      
      // Clear current if it was removed
      if (this.currentScenarioId === scenarioId) {
        this.clearCurrentSession();
      }
      
      log.info(`Removed scenario: ${scenarioId}`);
    }
    return result;
  }

  /**
   * Load test data from file path
   */
  public async loadTestDataFromFile(filePath: string): Promise<TestSession> {
    return testDataLoader.loadTestSession(filePath);
  }

  /**
   * Load test data from object
   */
  public async loadTestDataFromObject(data: TestSession, cacheKey?: string): Promise<TestSession> {
    return testDataLoader.loadTestSessionFromData(data, cacheKey);
  }

  /**
   * Get all cached sessions
   */
  public getCachedSessions(): Map<string, TestSession> {
    return testDataLoader.getAllCachedSessions();
  }

  /**
   * Clear all caches
   */
  public clearCaches(): void {
    testDataLoader.clearCache();
    this.scenarios.forEach(scenario => {
      scenario.sessionData = undefined;
    });
    log.info('Cleared all caches');
  }

  /**
   * Toggle test controls visibility
   */
  public setShowTestControls(show: boolean): void {
    this.config.showTestControls = show;
    log.info(`Test controls ${show ? 'shown' : 'hidden'}`);
  }

  /**
   * Export current session for debugging
   */
  public exportCurrentSession(): string | null {
    const scenario = this.getCurrentScenario();
    if (!scenario?.sessionData) {
      return null;
    }
    return JSON.stringify(scenario.sessionData, null, 2);
  }
}

// Export singleton instance
export const testSessionManager = TestSessionManager.getInstance();