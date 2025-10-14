/**
 * Utility functions for managing agent key persistence in localStorage
 */

const AGENT_KEY_STORAGE_KEY = 'agentc_selected_agent_key';

export const AgentStorage = {
  /**
   * Save the selected agent key to localStorage
   * @param agentKey - The agent key to save
   */
  saveAgentKey(agentKey: string): void {
    try {
      if (typeof window !== 'undefined' && window.localStorage) {
        localStorage.setItem(AGENT_KEY_STORAGE_KEY, agentKey);
      }
    } catch (error) {
      // Handle quota exceeded error specifically
      if (error instanceof Error && error.name === 'QuotaExceededError') {
        console.warn('localStorage quota exceeded, agent selection not persisted');
      } else {
        console.warn('Failed to save agent key to localStorage:', error);
      }
    }
  },

  /**
   * Retrieve the saved agent key from localStorage
   * @returns The saved agent key or null if not found or empty
   */
  getAgentKey(): string | null {
    try {
      if (typeof window !== 'undefined' && window.localStorage) {
        const value = localStorage.getItem(AGENT_KEY_STORAGE_KEY);
        // Return null for empty strings to avoid setting empty agent keys
        return value && value.trim() !== '' ? value : null;
      }
    } catch (error) {
      console.warn('Failed to retrieve agent key from localStorage:', error);
    }
    return null;
  },

  /**
   * Clear the saved agent key from localStorage
   */
  clearAgentKey(): void {
    try {
      if (typeof window !== 'undefined' && window.localStorage) {
        localStorage.removeItem(AGENT_KEY_STORAGE_KEY);
      }
    } catch (error) {
      console.warn('Failed to clear agent key from localStorage:', error);
    }
  },

  /**
   * Check if localStorage is available
   * @returns true if localStorage is available and working
   */
  isStorageAvailable(): boolean {
    try {
      if (typeof window === 'undefined' || !window.localStorage) {
        return false;
      }
      const testKey = '__agentc_storage_test__';
      localStorage.setItem(testKey, 'test');
      localStorage.removeItem(testKey);
      return true;
    } catch {
      return false;
    }
  }
};