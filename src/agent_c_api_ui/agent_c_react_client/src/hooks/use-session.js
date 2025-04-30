import { useContext } from 'react';
import { SessionContext } from '../contexts/SessionContext';

/**
 * Custom hook for accessing SessionContext
 * 
 * Provides access to core session functionality including:
 * - Session state (sessionId, isReady, error)
 * - Session initialization
 * - Session deletion handling
 * - Session validation
 * 
 * @returns {Object} SessionContext value
 * @throws {Error} When used outside SessionProvider
 * 
 * @example
 * // Using the hook in a component
 * function MyComponent() {
 *   const { sessionId, isReady, initializeSession } = useSession();
 *   
 *   // Now you can use session values and functions
 *   return <div>{isReady ? `Session: ${sessionId}` : 'No session'}</div>;
 * }
 */
export function useSession() {
  const context = useContext(SessionContext);
  
  if (context === undefined) {
    throw new Error('useSession must be used within a SessionProvider');
  }
  
  return context;
}