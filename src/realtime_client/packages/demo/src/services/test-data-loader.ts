/**
 * Test Data Loader Service
 * Handles loading and parsing test session JSON files
 */

import { TestSession, TestScenario } from '@/types/test-session';

/**
 * Logger for test data operations
 */
const log = {
  info: (message: string, ...args: any[]) => 
    console.log(`[TestDataLoader] ${message}`, ...args),
  error: (message: string, ...args: any[]) => 
    console.error(`[TestDataLoader] ERROR: ${message}`, ...args),
  warn: (message: string, ...args: any[]) => 
    console.warn(`[TestDataLoader] WARN: ${message}`, ...args),
};

/**
 * Test data loader service
 */
export class TestDataLoader {
  private static instance: TestDataLoader | null = null;
  private loadedSessions: Map<string, TestSession> = new Map();

  /**
   * Get singleton instance
   */
  public static getInstance(): TestDataLoader {
    if (!TestDataLoader.instance) {
      TestDataLoader.instance = new TestDataLoader();
    }
    return TestDataLoader.instance;
  }

  /**
   * Load test session from JSON file
   */
  public async loadTestSession(filePath: string): Promise<TestSession> {
    log.info(`Loading test session from: ${filePath}`);

    try {
      // Check cache first
      if (this.loadedSessions.has(filePath)) {
        log.info('Returning cached session');
        return this.loadedSessions.get(filePath)!;
      }

      // Fetch the JSON file
      const response = await fetch(filePath);
      
      if (!response.ok) {
        throw new Error(`Failed to fetch test file: ${response.statusText}`);
      }

      const sessionData: TestSession = await response.json();

      // Validate the session structure
      this.validateSession(sessionData);

      // Cache the loaded session
      this.loadedSessions.set(filePath, sessionData);

      log.info(`Successfully loaded session: ${sessionData.session_id}`);
      return sessionData;
    } catch (error) {
      log.error(`Failed to load test session: ${error}`);
      throw error;
    }
  }

  /**
   * Load test session from object (for inline test data)
   */
  public async loadTestSessionFromData(
    sessionData: TestSession, 
    cacheKey?: string
  ): Promise<TestSession> {
    log.info(`Loading test session from data: ${sessionData.session_id}`);

    try {
      // Validate the session structure
      this.validateSession(sessionData);

      // Cache if key provided
      if (cacheKey) {
        this.loadedSessions.set(cacheKey, sessionData);
      }

      return sessionData;
    } catch (error) {
      log.error(`Failed to process test session data: ${error}`);
      throw error;
    }
  }

  /**
   * Validate session structure
   */
  private validateSession(session: TestSession): void {
    if (!session.session_id) {
      throw new Error('Session missing required field: session_id');
    }

    if (!session.messages || !Array.isArray(session.messages)) {
      throw new Error('Session missing required field: messages (must be array)');
    }

    // Validate message structure
    session.messages.forEach((msg, index) => {
      if (!msg.role || !['user', 'assistant', 'system'].includes(msg.role)) {
        throw new Error(`Invalid message role at index ${index}: ${msg.role}`);
      }

      if (msg.content === undefined || msg.content === null) {
        throw new Error(`Message at index ${index} missing content`);
      }

      // Validate content blocks if array
      if (Array.isArray(msg.content)) {
        msg.content.forEach((block: any, blockIndex) => {
          if (!block.type) {
            throw new Error(
              `Content block at message ${index}, block ${blockIndex} missing type`
            );
          }
        });
      }
    });

    log.info('Session validation passed');
  }

  /**
   * Clear cached sessions
   */
  public clearCache(): void {
    this.loadedSessions.clear();
    log.info('Cleared session cache');
  }

  /**
   * Get cached session
   */
  public getCachedSession(key: string): TestSession | undefined {
    return this.loadedSessions.get(key);
  }

  /**
   * Get all cached sessions
   */
  public getAllCachedSessions(): Map<string, TestSession> {
    return new Map(this.loadedSessions);
  }

  /**
   * Parse delegation tool content from tool result
   * Extracts the actual message from the delegation tool response
   */
  public parseDelegationToolResult(content: string): {
    preamble: string;
    message: any;
  } | null {
    const preambleEnd = '---\n';
    const preambleStart = '**IMPORTANT**:';
    
    if (!content.includes(preambleStart)) {
      return null;
    }

    const preambleEndIndex = content.indexOf(preambleEnd);
    if (preambleEndIndex === -1) {
      return null;
    }

    const preamble = content.substring(0, preambleEndIndex);
    const yamlContent = content.substring(preambleEndIndex + preambleEnd.length);

    try {
      // Parse the YAML content
      // For now we'll just extract the text content using regex
      // A proper YAML parser could be added if needed
      const textMatch = yamlContent.match(/text:\s*'([\s\S]+?)'/);
      const text = textMatch ? textMatch[1] : yamlContent;

      return {
        preamble,
        message: {
          type: 'text',
          text: text,
          citations: null
        }
      };
    } catch (error) {
      log.error('Failed to parse delegation tool result', error);
      return null;
    }
  }

  /**
   * Check if a tool is a delegation tool
   */
  public isDelegationTool(toolName: string): boolean {
    return toolName.startsWith('act_') || 
           toolName.startsWith('ateam_') || 
           toolName.startsWith('aa_');
  }

  /**
   * Extract agent key from delegation tool
   */
  public getAgentKeyFromTool(toolName: string, input: any): string | undefined {
    if (toolName.startsWith('ateam_') || toolName.startsWith('aa_')) {
      return input?.agent_key;
    }
    return undefined;
  }
}

// Export singleton instance
export const testDataLoader = TestDataLoader.getInstance();