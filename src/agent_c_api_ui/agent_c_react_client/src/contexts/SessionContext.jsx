import React, { createContext, useState, useEffect } from 'react';
import { session as sessionService } from '../services';

/**
 * SessionContext provides core session management functionality including:
 * - Session initialization
 * - Session state tracking
 * - Session persistence
 * - Session validation
 * - Error handling
 */
export const SessionContext = createContext();

export const SessionProvider = ({ children }) => {
  // Core session state only
  const [sessionId, setSessionId] = useState(null);
  const [isReady, setIsReady] = useState(false);
  const [isInitialized, setIsInitialized] = useState(false);
  const [error, setError] = useState(null);
  const [isValidating, setIsValidating] = useState(false);

  /**
   * Initialize a session with the provided configuration
   * @param {Object} config - Session configuration object
   * @returns {Promise<string>} - The session ID if successful
   * @throws {Error} - If initialization fails
   */
  const initializeSession = async (config) => {
    setIsReady(false);
    try {
      const data = await sessionService.initialize(config);
      if (data.ui_session_id) {
        localStorage.setItem("ui_session_id", data.ui_session_id);
        setSessionId(data.ui_session_id);
        setIsReady(true);
        setError(null);
        return data.ui_session_id;
      } else {
        throw new Error("No ui_session_id in response");
      }
    } catch (err) {
      setIsReady(false);
      setError(`Session initialization failed: ${err.message}`);
      throw err;
    }
  };

  /**
   * Clear session when sessions are deleted
   */
  const handleSessionsDeleted = () => {
    localStorage.removeItem("ui_session_id");
    setSessionId(null);
    setIsReady(false);
    setError(null);
    setIsInitialized(false);
  };

  /**
   * Validate session by checking with backend
   * @param {string} sessionIdToCheck - Session ID to validate
   * @returns {Promise<boolean>} - True if session is valid
   */
  const validateSession = async (sessionIdToCheck) => {
    if (!sessionIdToCheck) return false;
    
    try {
      // Use silent404 option to avoid logging expected 404 errors
      const result = await sessionService.getSession(sessionIdToCheck, { silent404: true });
      return result !== null;
    } catch (err) {
      // We're silently handling 404s, so any error here is unexpected
      console.error("Unexpected error during session validation:", err);
      return false;
    }
  };

  // Check for existing session on mount
  useEffect(() => {
    const checkExistingSession = async () => {
      const savedSessionId = localStorage.getItem("ui_session_id");
      if (savedSessionId) {
        try {
          setIsValidating(true);
          // Validate session with backend
          const isValid = await validateSession(savedSessionId);
          if (isValid) {
            setSessionId(savedSessionId);
            setIsReady(true);
            setError(null);
          } else {
            // Session no longer valid on backend - this is normal, not an error
            // Just clear it silently and we'll create a new one when needed
            localStorage.removeItem("ui_session_id");
            setSessionId(null);
            setIsReady(false);
            // Don't set an error for a normal condition
            setError(null);
          }
        } catch (err) {
          // Unexpected error during validation, clear session
          localStorage.removeItem("ui_session_id");
          setError(`Session validation error: ${err.message}`);
          setSessionId(null);
          setIsReady(false);
        } finally {
          setIsValidating(false);
        }
      }
    };

    checkExistingSession();
  }, []);

  return (
    <SessionContext.Provider
      value={{
        sessionId,
        isReady,
        isInitialized,
        setIsInitialized,
        error,
        setError,
        isValidating,
        initializeSession,
        handleSessionsDeleted,
        validateSession
      }}
    >
      {children}
    </SessionContext.Provider>
  );
};