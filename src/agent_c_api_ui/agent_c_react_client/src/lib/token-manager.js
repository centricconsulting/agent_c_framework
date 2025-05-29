/**
 * Token Manager for robust token refresh handling.
 * 
 * This module provides advanced token management including:
 * - Automatic background token refresh before expiration
 * - Token expiration tracking
 * - Refresh retry with exponential backoff
 * - Event-based notification system for token lifecycle events
 */
import EventEmitter from 'eventemitter3';
//import { EventEmitter } from 'events';

// Default configuration
const DEFAULT_CONFIG = {
  // Refresh token when it has less than 5 minutes remaining (300 seconds)
  refreshThreshold: 300,
  // Minimum time between refresh attempts (in seconds)
  minRefreshInterval: 30,
  // Maximum retry attempts for failed refreshes
  maxRetryAttempts: 3,
  // Base delay for exponential backoff (in milliseconds)
  retryBaseDelay: 2000,
  // Debug mode for extra logging
  debug: false
};

/**
 * TokenManager class for managing authentication tokens and refresh cycles
 */
export class TokenManager extends EventEmitter {
  constructor(tokenProvider, config = {}) {
    super();
    this.tokenProvider = tokenProvider;
    this.config = { ...DEFAULT_CONFIG, ...config };
    
    // Token state
    this.currentToken = null;
    this.tokenExpiration = null;
    this.refreshing = false;
    this.refreshTimer = null;
    this.retryCount = 0;
    this.lastRefreshTime = 0;
    
    // Bind methods
    this.getToken = this.getToken.bind(this);
    this.scheduleRefresh = this.scheduleRefresh.bind(this);
    this.refreshToken = this.refreshToken.bind(this);
    
    // Debug logging if enabled
    if (this.config.debug) {
      this.on('debug', console.debug);
    }
  }
  
  /**
   * Initialize the token manager
   */
  async initialize() {
    this._debug('Initializing token manager - AUTO-REFRESH DISABLED');
    try {
      // Attempt to get an initial token
      await this.refreshToken();
      // Note: Auto-refresh is disabled - see scheduleRefresh method
      console.warn('Token auto-refresh is currently disabled until backend implementation is completed');
      return true;
    } catch (error) {
      this._debug('Failed to initialize token manager', error);
      return false;
    }
  }
  
  /**
   * Get the current valid token, refreshing if necessary
   * @returns {Promise<string|null>} The current token or null if unavailable
   */
  async getToken() {
    // If we don't have a token yet, try to get one
    if (!this.currentToken) {
      await this.refreshToken();
      return this.currentToken;
    }
    
    // If token is close to expiration, refresh in the background but return current token
    if (this.shouldRefresh() && !this.refreshing) {
      // Don't await this - do it in the background
      this.refreshToken().catch(err => {
        this._debug('Background token refresh failed', err);
      });
    }
    
    // Return the current token (which might be close to expiring)
    return this.currentToken;
  }
  
  /**
   * Check if the token needs to be refreshed
   * @returns {boolean} True if token should be refreshed
   */
  shouldRefresh() {
    if (!this.tokenExpiration) return true;
    
    const now = Math.floor(Date.now() / 1000);
    const timeUntilExpiration = this.tokenExpiration - now;
    
    // Check if we're within the refresh threshold
    return timeUntilExpiration <= this.config.refreshThreshold;
  }
  
  /**
   * Force refresh the token immediately
   * @returns {Promise<string|null>} The new token or null if refresh failed
   */
  async refreshToken() {
    // Prevent concurrent refresh attempts
    if (this.refreshing) {
      this._debug('Token refresh already in progress, waiting');
      return new Promise((resolve) => {
        const handleRefresh = (token) => {
          this.removeListener('refreshed', handleRefresh);
          this.removeListener('refreshFailed', handleFailure);
          resolve(token);
        };
        
        const handleFailure = () => {
          this.removeListener('refreshed', handleRefresh);
          this.removeListener('refreshFailed', handleFailure);
          resolve(this.currentToken); // Return current token even on failure
        };
        
        this.once('refreshed', handleRefresh);
        this.once('refreshFailed', handleFailure);
      });
    }
    
    // Check if we're refreshing too frequently
    const now = Date.now();
    const timeSinceLastRefresh = (now - this.lastRefreshTime) / 1000;
    if (timeSinceLastRefresh < this.config.minRefreshInterval) {
      this._debug(`Refresh too frequent, last refresh was ${timeSinceLastRefresh.toFixed(1)}s ago`);
      return this.currentToken;
    }
    
    try {
      // Set refreshing flag to prevent concurrent refreshes
      this.refreshing = true;
      this.emit('refreshStarted');
      this._debug('Starting token refresh');
      
      // Clear any pending refresh timers
      if (this.refreshTimer) {
        clearTimeout(this.refreshTimer);
        this.refreshTimer = null;
      }
      
      // Request a new token
      const token = await this.tokenProvider();
      this.lastRefreshTime = Date.now();
      
      if (!token) {
        throw new Error('Token provider returned null or undefined token');
      }
      
      // Extract expiration from token if possible
      this.tokenExpiration = this._getTokenExpiration(token);
      this.currentToken = token;
      this.retryCount = 0; // Reset retry counter on success
      
      // Schedule the next refresh
      this.scheduleRefresh();
      
      this._debug('Token refreshed successfully', {
        expiresIn: this.tokenExpiration ? `${(this.tokenExpiration - Math.floor(Date.now() / 1000))}s` : 'unknown'
      });
      
      this.emit('refreshed', token);
      return token;
    } catch (error) {
      this._debug('Token refresh failed', error);
      this.emit('refreshFailed', error);
      
      // Implement retry with exponential backoff
      if (this.retryCount < this.config.maxRetryAttempts) {
        this.retryCount++;
        const delay = this.config.retryBaseDelay * Math.pow(2, this.retryCount - 1);
        
        this._debug(`Scheduling retry attempt ${this.retryCount} in ${delay}ms`);
        setTimeout(() => {
          this.refreshing = false; // Reset flag so retry can proceed
          this.refreshToken().catch(e => {
            this._debug('Retry attempt failed', e);
          });
        }, delay);
      } else {
        this._debug('Max retry attempts reached, giving up');
        this.emit('refreshExhausted', error);
      }
      
      return this.currentToken; // Return the existing token even on failure
    } finally {
      this.refreshing = false;
    }
  }
  
  /**
   * Schedule the next token refresh based on expiration time
   * TEMPORARILY DISABLED: Auto-refresh disabled until backend implementation is completed
   */
  scheduleRefresh() {
    if (this.refreshTimer) {
      clearTimeout(this.refreshTimer);
      this.refreshTimer = null;
    }
    
    // Auto-refresh functionality temporarily disabled
    this._debug('Auto-refresh disabled until backend implementation is completed');
    
    /* DISABLED: Auto-refresh code
    if (!this.tokenExpiration) {
      // If we don't know expiration, refresh conservatively after minRefreshInterval
      this._debug('Token expiration unknown, scheduling conservative refresh');
      this.refreshTimer = setTimeout(
        this.refreshToken, 
        this.config.minRefreshInterval * 1000
      );
      return;
    }
    
    const now = Math.floor(Date.now() / 1000);
    const timeUntilExpiration = this.tokenExpiration - now;
    
    // Calculate when to schedule the next refresh
    const refreshIn = Math.max(
      0, // Don't go negative
      timeUntilExpiration - this.config.refreshThreshold, // Refresh before expiration
      1 // Minimum 1 second delay
    );
    
    this._debug(`Scheduling next refresh in ${refreshIn} seconds`);
    
    this.refreshTimer = setTimeout(this.refreshToken, refreshIn * 1000);
    */
  }
  
  /**
   * Extract token expiration time from JWT token
   * @param {string} token - JWT token
   * @returns {number|null} Expiration timestamp or null if not determinable
   */
  _getTokenExpiration(token) {
    try {
      // JWT tokens consist of three parts separated by dots
      const parts = token.split('.');
      if (parts.length !== 3) return null;
      
      // The second part is the payload
      const payload = JSON.parse(atob(parts[1]));
      
      // 'exp' claim contains the expiration timestamp
      if (payload && payload.exp) {
        return payload.exp;
      }
      
      return null;
    } catch (error) {
      this._debug('Failed to extract token expiration', error);
      return null;
    }
  }
  
  /**
   * Internal debug logging
   * @private
   */
  _debug(message, data = null) {
    if (this.config.debug || this.listeners('debug').length > 0) {
      this.emit('debug', { message, data, timestamp: new Date() });
    }
  }
  
  /**
   * Clean up resources when done
   */
  dispose() {
    if (this.refreshTimer) {
      clearTimeout(this.refreshTimer);
      this.refreshTimer = null;
    }
    
    this.removeAllListeners();
    this.currentToken = null;
    this.tokenExpiration = null;
  }
}

export default TokenManager;