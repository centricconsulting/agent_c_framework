/**
 * Storage Service for Agent C React Client
 * 
 * This service centralizes all localStorage interactions, providing
 * consistent key management, error handling, and data versioning.
 * 
 * Phase 2 Enhancement: Unified methods for data retrieval and storage
 * with proper expiration handling and version management.
 */

import logger from './logger';
import eventBus from './eventBus.js';

// Storage keys
const STORAGE_KEYS = {
  SESSION_ID: 'ui_session_id',
  SESSION_DATA: 'session_data',     // Enhanced to store all session data
  MODEL_DATA: 'model_data',         // Enhanced to store all model settings
  AGENT_CONFIG: 'agent_config',     // Legacy key (maintained for backward compatibility)
  THEME: 'theme',
  CHAT_HISTORY: 'chat_history',
  USER_SETTINGS: 'agentc_user_settings'
};

// Storage events
const STORAGE_EVENTS = {
  SESSION_DATA_UPDATED: 'storage:session_data_updated',
  MODEL_DATA_UPDATED: 'storage:model_data_updated',
  USER_SETTINGS_UPDATED: 'storage:user_settings_updated',
  STORAGE_ERROR: 'storage:error',
  STORAGE_CLEARED: 'storage:cleared'
};

// Current storage schema version
const STORAGE_VERSION = 2;  // Incremented for Phase 2 changes

// Expiration times in milliseconds
const EXPIRATION = {
  SESSION_DATA: 24 * 60 * 60 * 1000,     // 24 hours
  MODEL_DATA: 14 * 24 * 60 * 60 * 1000,   // 14 days
  USER_SETTINGS: 30 * 24 * 60 * 60 * 1000 // 30 days
};

// Migration handlers for version upgrades
const migrations = {
  // Migrate from version 1 to version 2
  1: function() {
    try {
      logger.debug('Migrating storage from version 1 to 2', 'storageService');
      
      // Migrate agent config to model data
      const agentConfig = storage.getJSON(STORAGE_KEYS.AGENT_CONFIG, null);
      if (agentConfig) {
        // Create model data structure
        const modelData = {
          modelName: agentConfig.modelName,
          modelParameters: agentConfig.modelParameters || {},
          storageVersion: STORAGE_VERSION,
          lastUpdated: agentConfig.lastUpdated || new Date().toISOString()
        };
        
        // Save to new structure
        storage.setJSON(STORAGE_KEYS.MODEL_DATA, modelData);
        logger.debug('Migrated agent config to model data', 'storageService');
      }
      
      // Session ID to session data migration
      const sessionId = storage.get(STORAGE_KEYS.SESSION_ID, null);
      if (sessionId) {
        const sessionData = {
          sessionId: sessionId,
          storageVersion: STORAGE_VERSION,
          created: new Date().toISOString(),
          lastUpdated: new Date().toISOString()
        };
        
        storage.setJSON(STORAGE_KEYS.SESSION_DATA, sessionData);
        logger.debug('Migrated session ID to session data', 'storageService');
      }
      
      return true;
    } catch (error) {
      logger.error('Error migrating from version 1 to 2', 'storageService', {
        error: error.message,
        stack: error.stack
      });
      return false;
    }
  }
};

/**
 * Wrapper for localStorage with error handling and event emission
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
      
      // Emit error event for listeners
      eventBus.publish(STORAGE_EVENTS.STORAGE_ERROR, {
        operation: 'read',
        key,
        error: error.message
      });
      
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
      
      // Emit error event for listeners
      eventBus.publish(STORAGE_EVENTS.STORAGE_ERROR, {
        operation: 'readJSON',
        key,
        error: error.message
      });
      
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
      
      // Emit error event for listeners
      eventBus.publish(STORAGE_EVENTS.STORAGE_ERROR, {
        operation: 'write',
        key,
        error: error.message
      });
      
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
      
      // Emit appropriate event based on key
      if (key === STORAGE_KEYS.SESSION_DATA) {
        eventBus.publish(STORAGE_EVENTS.SESSION_DATA_UPDATED, value);
      } else if (key === STORAGE_KEYS.MODEL_DATA) {
        eventBus.publish(STORAGE_EVENTS.MODEL_DATA_UPDATED, value);
      } else if (key === STORAGE_KEYS.USER_SETTINGS) {
        eventBus.publish(STORAGE_EVENTS.USER_SETTINGS_UPDATED, value);
      }
      
      return true;
    } catch (error) {
      logger.error(`Error writing JSON to localStorage: ${key}`, 'storageService', { 
        error: error.message 
      });
      logger.storageOp?.('write', key, false);
      
      // Emit error event for listeners
      eventBus.publish(STORAGE_EVENTS.STORAGE_ERROR, {
        operation: 'writeJSON',
        key,
        error: error.message
      });
      
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
      
      // Emit error event for listeners
      eventBus.publish(STORAGE_EVENTS.STORAGE_ERROR, {
        operation: 'remove',
        key,
        error: error.message
      });
      
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
      
      // Emit clear event
      eventBus.publish(STORAGE_EVENTS.STORAGE_CLEARED, {});
      
      return true;
    } catch (error) {
      logger.error('Error clearing localStorage', 'storageService', { 
        error: error.message 
      });
      logger.storageOp?.('clear', 'all', false);
      
      // Emit error event for listeners
      eventBus.publish(STORAGE_EVENTS.STORAGE_ERROR, {
        operation: 'clear',
        error: error.message
      });
      
      return false;
    }
  }
};

/**
 * Check if data is expired
 * @param {Object} data - Data to check for expiration
 * @param {number} maxAgeMs - Maximum age in milliseconds
 * @returns {boolean} True if data is expired
 */
const isExpired = (data, maxAgeMs) => {
  if (!data || !data.lastUpdated) {
    return true;
  }
  
  const updateTime = new Date(data.lastUpdated).getTime();
  const now = new Date().getTime();
  return (now - updateTime) > maxAgeMs;
};

/**
 * Check storage version and run migrations if needed
 */
const checkAndMigrateStorage = () => {
  try {
    // Get current storage version (defaults to 1 if not found)
    let currentVersion = 1;
    
    // Try to get version from model data first (new storage format)
    const modelData = storage.getJSON(STORAGE_KEYS.MODEL_DATA, null);
    if (modelData && modelData.storageVersion) {
      currentVersion = modelData.storageVersion;
    } else {
      // Fall back to agent config (old storage format)
      const agentConfig = storage.getJSON(STORAGE_KEYS.AGENT_CONFIG, null);
      if (agentConfig && agentConfig.storageVersion) {
        currentVersion = agentConfig.storageVersion;
      }
    }
    
    // Run migrations if current version is lower than the latest version
    if (currentVersion < STORAGE_VERSION) {
      logger.info(`Storage migration needed from v${currentVersion} to v${STORAGE_VERSION}`, 'storageService');
      
      // Run each migration in sequence
      for (let version = currentVersion; version < STORAGE_VERSION; version++) {
        if (migrations[version]) {
          const success = migrations[version]();
          if (!success) {
            logger.error(`Migration from v${version} to v${version + 1} failed`, 'storageService');
            break;
          }
        }
      }
    }
  } catch (error) {
    logger.error('Error checking or migrating storage version', 'storageService', {
      error: error.message,
      stack: error.stack
    });
  }
};

/**
 * Storage service with domain-specific storage functions
 */
const storageService = {
  // Raw storage access
  storage,
  STORAGE_KEYS,
  STORAGE_EVENTS,
  
  /**
   * Initialize storage service
   * This should be called at application startup to migrate data if needed
   */
  initialize: () => {
    logger.info('Initializing storage service', 'storageService');
    checkAndMigrateStorage();
    return true;
  },
  
  /**
   * Get all session-related data
   * @returns {Object|null} Session data or null if not found/expired
   */
  getSessionData: () => {
    // Try to get from unified storage format first
    const sessionData = storage.getJSON(STORAGE_KEYS.SESSION_DATA, null);
    
    // Check if we have valid session data in the new format
    if (sessionData && sessionData.sessionId) {
      // Check if data is expired
      if (isExpired(sessionData, EXPIRATION.SESSION_DATA)) {
        logger.info('Session data found but expired', 'storageService');
        storage.remove(STORAGE_KEYS.SESSION_DATA);
        return null;
      }
      return sessionData;
    }
    
    // Fall back to legacy format if needed
    const sessionId = storage.get(STORAGE_KEYS.SESSION_ID, null);
    if (sessionId) {
      // Create a session data object from the legacy format
      const legacyData = {
        sessionId,
        created: new Date().toISOString(),
        lastUpdated: new Date().toISOString(),
        storageVersion: STORAGE_VERSION
      };
      
      // Save in new format for future use
      storage.setJSON(STORAGE_KEYS.SESSION_DATA, legacyData);
      
      return legacyData;
    }
    
    return null;
  },
  
  /**
   * Save session data
   * @param {Object} data - Session data to save
   * @returns {boolean} Success status
   */
  saveSessionData: (data) => {
    if (!data || !data.sessionId) {
      logger.error('Invalid session data provided', 'storageService');
      return false;
    }
    
    // Prepare data for storage with timestamp and version
    const sessionData = {
      ...data,
      lastUpdated: new Date().toISOString(),
      storageVersion: STORAGE_VERSION
    };
    
    // If created timestamp is missing, add it
    if (!sessionData.created) {
      sessionData.created = sessionData.lastUpdated;
    }
    
    // Save in both formats for backward compatibility
    storage.set(STORAGE_KEYS.SESSION_ID, data.sessionId);
    return storage.setJSON(STORAGE_KEYS.SESSION_DATA, sessionData);
  },
  
  /**
   * Remove session data
   * @returns {boolean} Success status
   */
  removeSessionData: () => {
    storage.remove(STORAGE_KEYS.SESSION_ID);  // Remove legacy data
    return storage.remove(STORAGE_KEYS.SESSION_DATA);
  },
  
  /**
   * Legacy methods maintained for backward compatibility
   */
  getSessionId: () => {
    const sessionData = storageService.getSessionData();
    return sessionData ? sessionData.sessionId : null;
  },
  
  saveSessionId: (sessionId) => {
    return storageService.saveSessionData({ sessionId });
  },
  
  removeSessionId: () => {
    return storageService.removeSessionData();
  },
  
  /**
   * Get all model-related data
   * @returns {Object} Model data or empty object if not found/expired
   */
  getModelData: () => {
    // Try to get from unified storage format first
    const modelData = storage.getJSON(STORAGE_KEYS.MODEL_DATA, null);
    
    // Check if we have valid model data in the new format
    if (modelData && (modelData.modelName || modelData.modelParameters)) {
      // Check if data is expired
      if (isExpired(modelData, EXPIRATION.MODEL_DATA)) {
        logger.info('Model data found but expired', 'storageService');
        storage.remove(STORAGE_KEYS.MODEL_DATA);
        return {};
      }
      return modelData;
    }
    
    // Fall back to legacy format if needed
    const legacyConfig = storage.getJSON(STORAGE_KEYS.AGENT_CONFIG, null);
    if (legacyConfig && (legacyConfig.modelName || legacyConfig.modelParameters)) {
      // Check if legacy config is expired
      if (isExpired(legacyConfig, EXPIRATION.MODEL_DATA)) {
        logger.info('Legacy model config found but expired', 'storageService');
        storage.remove(STORAGE_KEYS.AGENT_CONFIG);
        return {};
      }
      
      // Create a model data object from the legacy format
      const newModelData = {
        modelName: legacyConfig.modelName,
        modelParameters: legacyConfig.modelParameters || {},
        lastUpdated: legacyConfig.lastUpdated || new Date().toISOString(),
        storageVersion: STORAGE_VERSION
      };
      
      // Save in new format for future use
      storage.setJSON(STORAGE_KEYS.MODEL_DATA, newModelData);
      
      return newModelData;
    }
    
    return {};
  },
  
  /**
   * Save model data
   * @param {Object} data - Model data to save
   * @returns {boolean} Success status
   */
  saveModelData: (data) => {
    if (!data) {
      logger.error('Invalid model data provided', 'storageService');
      return false;
    }
    
    // Prepare data for storage with timestamp and version
    const modelData = {
      ...data,
      lastUpdated: new Date().toISOString(),
      storageVersion: STORAGE_VERSION
    };
    
    // Save in both formats for backward compatibility
    const legacyConfig = {
      modelName: data.modelName,
      modelParameters: data.modelParameters,
      lastUpdated: modelData.lastUpdated,
      storageVersion: STORAGE_VERSION
    };
    storage.setJSON(STORAGE_KEYS.AGENT_CONFIG, legacyConfig);
    
    return storage.setJSON(STORAGE_KEYS.MODEL_DATA, modelData);
  },
  
  /**
   * Update part of the model data
   * @param {Object} partialData - Partial model data to update
   * @returns {boolean} Success status
   */
  updateModelData: (partialData) => {
    const currentData = storageService.getModelData();
    return storageService.saveModelData({
      ...currentData,
      ...partialData
    });
  },
  
  /**
   * Get model preferences (convenience method for modelName and modelParameters)
   * @returns {Object|null} Model preferences or null if not found
   */
  getModelPreferences: () => {
    const modelData = storageService.getModelData();
    if (!modelData || !modelData.modelName) {
      return null;
    }
    
    return {
      modelId: modelData.modelName,
      parameters: modelData.modelParameters || {}
    };
  },
  
  /**
   * Legacy methods maintained for backward compatibility
   */
  getAgentConfig: () => {
    const modelData = storageService.getModelData();
    // Convert to legacy format
    return {
      modelName: modelData.modelName,
      modelParameters: modelData.modelParameters,
      lastUpdated: modelData.lastUpdated,
      storageVersion: modelData.storageVersion
    };
  },
  
  saveAgentConfig: (config) => {
    return storageService.saveModelData({
      modelName: config.modelName,
      modelParameters: config.modelParameters
    });
  },
  
  updateAgentConfig: (partialConfig) => {
    return storageService.updateModelData(partialConfig);
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
   * Get user settings data
   * @returns {Object} User settings or empty object if not found/expired
   */
  getUserSettings: () => {
    try {
      // Get settings from storage
      const settings = storage.getJSON(STORAGE_KEYS.USER_SETTINGS, null);
      
      // Check if we have valid settings
      if (!settings) {
        return {};
      }
      
      // Check if settings are expired
      if (settings.timestamp) {
        const timestamp = settings.timestamp;
        const now = Date.now();
        
        if (now - timestamp > EXPIRATION.USER_SETTINGS) {
          logger.info('User settings found but expired', 'storageService');
          storage.remove(STORAGE_KEYS.USER_SETTINGS);
          return {};
        }
      }
      
      return settings;
    } catch (err) {
      logger.error('Failed to load user settings', 'storageService', { 
        error: err.message,
        stack: err.stack
      });
      return {};
    }
  },
  
  /**
   * Save user settings
   * @param {Object} settings - User settings to save
   * @returns {boolean} Success status
   */
  saveUserSettings: (settings) => {
    try {
      if (!settings) {
        logger.error('Invalid user settings provided', 'storageService');
        return false;
      }
      
      // Prepare settings for storage with timestamp and version
      const userSettings = {
        ...settings,
        timestamp: Date.now(),
        storageVersion: STORAGE_VERSION
      };
      
      return storage.setJSON(STORAGE_KEYS.USER_SETTINGS, userSettings);
    } catch (err) {
      logger.error('Failed to save user settings', 'storageService', { 
        error: err.message,
        stack: err.stack
      });
      return false;
    }
  },
  
  /**
   * Update part of the user settings
   * @param {Object} partialSettings - Partial settings to update
   * @returns {boolean} Success status
   */
  updateUserSettings: (partialSettings) => {
    try {
      const currentSettings = storageService.getUserSettings();
      return storageService.saveUserSettings({
        ...currentSettings,
        ...partialSettings
      });
    } catch (err) {
      logger.error('Failed to update user settings', 'storageService', { 
        error: err.message,
        stack: err.stack
      });
      return false;
    }
  },
  
  /**
   * Legacy methods maintained for backward compatibility
   */
  saveSettings: async (settings) => {
    return storageService.saveUserSettings(settings);
  },

  loadSettings: async () => {
    return storageService.getUserSettings();
  }
};

export default storageService;