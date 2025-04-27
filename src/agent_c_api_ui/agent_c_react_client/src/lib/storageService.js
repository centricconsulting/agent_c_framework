/**
 * Storage Service for Agent C React Client
 * 
 * This service centralizes all localStorage interactions, providing
 * consistent key management, error handling, and data versioning.
 */

import logger from './logger';

// Storage keys
const STORAGE_KEYS = {
  SESSION_ID: 'ui_session_id',
  AGENT_CONFIG: 'agent_config',
  THEME: 'theme',
  CHAT_HISTORY: 'chat_history',
  USER_SETTINGS: 'agentc_user_settings' // New key for user settings
};

// Current storage schema version
const STORAGE_VERSION = 1;

/**
 * Wrapper for localStorage with error handling
 */
const storage = {
  /**
   * Get an item from localStorage
   * @param {string} key - Storage key
   * @param {*} defaultValue - Default value if key doesn't exist or error occurs
   * @returns {*} Stored value or default
   */
  get: (key, defaultValue = null) => {
    try {
      const value = localStorage.getItem(key);
      logger.debug(`Retrieved from localStorage: ${key}`, 'storageService');
      logger.storageOp?.('read', key, true);
      return value !== null ? value : defaultValue;
    } catch (error) {
      logger.error(`Error reading from localStorage: ${key}`, 'storageService', { 
        error: error.message 
      });
      logger.storageOp?.('read', key, false);
      return defaultValue;
    }
  },
  
  /**
   * Get and parse a JSON item from localStorage
   * @param {string} key - Storage key
   * @param {*} defaultValue - Default value if key doesn't exist or error occurs
   * @returns {*} Parsed value or default
   */
  getJSON: (key, defaultValue = null) => {
    try {
      const value = localStorage.getItem(key);
      logger.debug(`Retrieved JSON from localStorage: ${key}`, 'storageService');
      logger.storageOp?.('read', key, true);
      
      if (value === null) {
        return defaultValue;
      }
      
      return JSON.parse(value);
    } catch (error) {
      logger.error(`Error parsing JSON from localStorage: ${key}`, 'storageService', { 
        error: error.message 
      });
      logger.storageOp?.('read', key, false);
      return defaultValue;
    }
  },
  
  /**
   * Set an item in localStorage
   * @param {string} key - Storage key
   * @param {*} value - Value to store
   * @returns {boolean} Success status
   */
  set: (key, value) => {
    try {
      localStorage.setItem(key, value);
      logger.debug(`Saved to localStorage: ${key}`, 'storageService');
      logger.storageOp?.('write', key, true);
      return true;
    } catch (error) {
      logger.error(`Error writing to localStorage: ${key}`, 'storageService', { 
        error: error.message 
      });
      logger.storageOp?.('write', key, false);
      return false;
    }
  },
  
  /**
   * Set a JSON item in localStorage
   * @param {string} key - Storage key
   * @param {*} value - Value to stringify and store
   * @returns {boolean} Success status
   */
  setJSON: (key, value) => {
    try {
      const jsonValue = JSON.stringify(value);
      localStorage.setItem(key, jsonValue);
      logger.debug(`Saved JSON to localStorage: ${key}`, 'storageService');
      logger.storageOp?.('write', key, true);
      return true;
    } catch (error) {
      logger.error(`Error writing JSON to localStorage: ${key}`, 'storageService', { 
        error: error.message 
      });
      logger.storageOp?.('write', key, false);
      return false;
    }
  },
  
  /**
   * Remove an item from localStorage
   * @param {string} key - Storage key
   * @returns {boolean} Success status
   */
  remove: (key) => {
    try {
      localStorage.removeItem(key);
      logger.debug(`Removed from localStorage: ${key}`, 'storageService');
      logger.storageOp?.('remove', key, true);
      return true;
    } catch (error) {
      logger.error(`Error removing from localStorage: ${key}`, 'storageService', { 
        error: error.message 
      });
      logger.storageOp?.('remove', key, false);
      return false;
    }
  },
  
  /**
   * Clear all items from localStorage
   * @returns {boolean} Success status
   */
  clear: () => {
    try {
      localStorage.clear();
      logger.debug('Cleared localStorage', 'storageService');
      logger.storageOp?.('clear', 'all', true);
      return true;
    } catch (error) {
      logger.error('Error clearing localStorage', 'storageService', { 
        error: error.message 
      });
      logger.storageOp?.('clear', 'all', false);
      return false;
    }
  }
};

/**
 * Storage service with domain-specific storage functions
 */
const storageService = {
  // Raw storage access
  storage,
  STORAGE_KEYS,
  
  /**
   * Get the current session ID from storage
   * @returns {string|null} Session ID or null if not found
   */
  getSessionId: () => {
    return storage.get(STORAGE_KEYS.SESSION_ID, null);
  },
  
  /**
   * Save session ID to storage
   * @param {string} sessionId - Session ID to save
   * @returns {boolean} Success status
   */
  saveSessionId: (sessionId) => {
    return storage.set(STORAGE_KEYS.SESSION_ID, sessionId);
  },
  
  /**
   * Remove session ID from storage
   * @returns {boolean} Success status
   */
  removeSessionId: () => {
    return storage.remove(STORAGE_KEYS.SESSION_ID);
  },
  
  /**
   * Get agent configuration from storage
   * @returns {Object} Agent configuration or empty object if not found
   */
  getAgentConfig: () => {
    const config = storage.getJSON(STORAGE_KEYS.AGENT_CONFIG, {});
    
    // Check if config is expired (14 days)
    if (config.lastUpdated) {
      const configAge = new Date() - new Date(config.lastUpdated);
      const maxAgeMs = 14 * 24 * 60 * 60 * 1000; // 14 days
      
      if (configAge > maxAgeMs) {
        logger.info('Saved configuration is too old (>14 days), using defaults', 'storageService');
        storage.remove(STORAGE_KEYS.AGENT_CONFIG);
        return {};
      }
    }
    
    return config;
  },
  
  /**
   * Save agent configuration to storage
   * @param {Object} config - Configuration to save
   * @returns {boolean} Success status
   */
  saveAgentConfig: (config) => {
    const configToSave = {
      ...config,
      storageVersion: STORAGE_VERSION,
      lastUpdated: new Date().toISOString()
    };
    
    return storage.setJSON(STORAGE_KEYS.AGENT_CONFIG, configToSave);
  },
  
  /**
   * Update part of the agent configuration
   * @param {Object} partialConfig - Partial configuration to update
   * @returns {boolean} Success status
   */
  updateAgentConfig: (partialConfig) => {
    const currentConfig = storageService.getAgentConfig();
    return storageService.saveAgentConfig({
      ...currentConfig,
      ...partialConfig
    });
  },
  
  /**
   * Get theme preference from storage
   * @returns {string} Theme preference ('light', 'dark', or 'system')
   */
  getTheme: () => {
    return storage.get(STORAGE_KEYS.THEME, 'system');
  },
  
  /**
   * Save theme preference to storage
   * @param {string} theme - Theme preference
   * @returns {boolean} Success status
   */
  saveTheme: (theme) => {
    return storage.set(STORAGE_KEYS.THEME, theme);
  },

  /**
   * Save user settings to localStorage with timestamp
   * @param {Object} settings - User settings to save
   * @returns {Promise<boolean>} - Success status
   */
  saveSettings: async (settings) => {
    try {
      // Add timestamp if not already present
      if (!settings.timestamp) {
        settings.timestamp = Date.now();
      }
      
      // Save to localStorage
      return storage.setJSON(STORAGE_KEYS.USER_SETTINGS, settings);
    } catch (err) {
      logger.error('Failed to save settings to localStorage:', 'storageService', { 
        error: err.message,
        stack: err.stack
      });
      return false;
    }
  },

  /**
   * Load user settings from localStorage
   * @returns {Promise<Object|null>} - Loaded settings or null if not found
   */
  loadSettings: async () => {
    try {
      return storage.getJSON(STORAGE_KEYS.USER_SETTINGS, null);
    } catch (err) {
      logger.error('Failed to load settings from localStorage:', 'storageService', { 
        error: err.message,
        stack: err.stack
      });
      return null;
    }
  }
};

export default storageService;