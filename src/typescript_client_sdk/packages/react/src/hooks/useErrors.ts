/**
 * useErrors - React hook for error handling
 * Manages error events that should be displayed as toasts, not in chat
 */

import React, { useEffect, useState, useCallback } from 'react';
import { Logger } from '../utils/logger';
import { useRealtimeClientSafe } from '../providers/AgentCContext';
import type { ErrorInfo } from '../types/chat';

/**
 * Return type for the useErrors hook
 */
export interface UseErrorsReturn {
  /** All errors (including dismissed ones) */
  errors: ErrorInfo[];
  
  /** Currently active (non-dismissed) errors */
  activeErrors: ErrorInfo[];
  
  /** Dismiss a specific error */
  dismissError: (id: string) => void;
  
  /** Clear all errors */
  clearErrors: () => void;
  
  /** Latest active error for simple toast display */
  latestError: ErrorInfo | null;
  
  /** Add a custom error (for client-side errors) */
  addError: (message: string, source?: string) => void;
}

/**
 * React hook for error handling
 * Manages errors that should be displayed as toasts
 */
export function useErrors(): UseErrorsReturn {
  const client = useRealtimeClientSafe();
  const [errors, setErrors] = useState<ErrorInfo[]>([]);
  const errorTimersRef = React.useRef<Map<string, NodeJS.Timeout>>(new Map());
  
  // Add a new error to the list
  const addErrorInternal = useCallback((error: ErrorInfo) => {
    const errorId = error.id;
    setErrors(prev => [...prev, error]);
    
    // Auto-remove errors after 10 seconds (not just dismiss)
    const timer = setTimeout(() => {
      setErrors(prev => prev.filter(err => err.id !== errorId));
      errorTimersRef.current.delete(errorId);
    }, 10000);
    
    // Store timer for cleanup
    errorTimersRef.current.set(errorId, timer);
  }, []);
  
  // Handle error events from the server
  const handleServerError = useCallback((event: unknown) => {
    const errorEvent = event as {
      message: string;
      source?: string;
      timestamp?: string;
    };
    
    Logger.error('[useErrors] Server error event:', errorEvent);
    
    const errorInfo: ErrorInfo = {
      id: `error-${Date.now()}-${Math.random()}`,
      message: errorEvent.message || 'An unknown error occurred',
      source: errorEvent.source,
      timestamp: errorEvent.timestamp || new Date().toISOString(),
      dismissed: false
    };
    
    addErrorInternal(errorInfo);
  }, [addErrorInternal]);
  
  // Add a custom error (for client-side errors)
  const addError = useCallback((message: string, source?: string) => {
    Logger.error('[useErrors] Client error:', { message, source });
    
    const errorInfo: ErrorInfo = {
      id: `error-${Date.now()}-${Math.random()}`,
      message,
      source: source || 'client',
      timestamp: new Date().toISOString(),
      dismissed: false
    };
    
    addErrorInternal(errorInfo);
  }, [addErrorInternal]);
  
  // Dismiss a specific error
  const dismissError = useCallback((id: string) => {
    Logger.debug('[useErrors] Dismissing error:', id);
    
    // Clear any pending timer for this error
    const timer = errorTimersRef.current.get(id);
    if (timer) {
      clearTimeout(timer);
      errorTimersRef.current.delete(id);
    }
    
    // Remove the error immediately
    setErrors(prev => prev.filter(err => err.id !== id));
  }, []);
  
  // Clear all errors
  const clearErrors = useCallback(() => {
    Logger.debug('[useErrors] Clearing all errors');
    
    // Clear all timers
    errorTimersRef.current.forEach(timer => clearTimeout(timer));
    errorTimersRef.current.clear();
    
    setErrors([]);
  }, []);
  
  // Subscribe to error events
  useEffect(() => {
    if (!client) return;
    
    const sessionManager = client.getSessionManager();
    
    if (sessionManager) {
      // Subscribe to error events from the server
      sessionManager.on('error', handleServerError);
      
      Logger.debug('[useErrors] Subscribed to error events');
    }
    
    return () => {
      // Clear all timers on unmount
      errorTimersRef.current.forEach(timer => clearTimeout(timer));
      errorTimersRef.current.clear();
      
      // Cleanup subscription
      const cleanupSessionManager = client?.getSessionManager();
      if (cleanupSessionManager) {
        cleanupSessionManager.off('error', handleServerError);
        Logger.debug('[useErrors] Unsubscribed from error events');
      }
    };
  }, [client, handleServerError]);
  
  // Computed properties
  const activeErrors = errors.filter(err => !err.dismissed);
  const latestError: ErrorInfo | null = activeErrors.length > 0 
    ? activeErrors[activeErrors.length - 1]! 
    : null;
  
  return {
    errors,
    activeErrors,
    dismissError,
    clearErrors,
    latestError,
    addError
  };
}