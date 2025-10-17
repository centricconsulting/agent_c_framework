/**
 * Example: Paginated Session Loading
 * 
 * This example demonstrates how to use the new paginated session fetching
 * feature to implement infinite scrolling or lazy loading of chat sessions.
 */

import { RealtimeClient } from '../src/client/RealtimeClient';
import { AuthManager } from '../src/auth/AuthManager';

/**
 * Example 1: Basic Pagination Usage
 */
async function basicPaginationExample() {
  // Initialize AuthManager
  const authManager = new AuthManager({
    apiUrl: 'https://api.example.com',
    autoRefresh: true,
  });

  // Login
  await authManager.login({
    username: 'user@example.com',
    password: 'password123',
  });

  // The initial login response now includes paginated sessions
  const initialSessions = authManager.getSessions(); // Returns ChatSessionIndexEntry[]
  const metadata = authManager.getSessionsMetadata(); // Returns ChatSessionQueryResponse
  
  console.log(`Loaded ${initialSessions.length} of ${metadata?.total_sessions} total sessions`);

  // Create RealtimeClient with auth
  const client = new RealtimeClient({
    apiUrl: 'wss://api.example.com/rt/ws',
    authManager,
    enableTurnManager: true,
  });

  await client.connect();

  // Request more sessions as needed
  if (metadata && metadata.total_sessions > initialSessions.length) {
    // Fetch the next batch
    client.fetchUserSessions(initialSessions.length, 50);
  }
}

/**
 * Example 2: Infinite Scrolling Implementation
 */
class SessionListManager {
  private client: RealtimeClient;
  private sessionManager: any; // SessionManager type
  private isLoading = false;

  constructor(client: RealtimeClient) {
    this.client = client;
    this.sessionManager = client.getSessionManager();
    
    // Listen for session updates
    this.sessionManager?.on('sessions-index-updated', (event: any) => {
      this.onSessionsUpdated(event);
    });
  }

  /**
   * Load more sessions when user scrolls near the bottom
   */
  async loadMoreSessions() {
    if (this.isLoading) return;
    
    const hasMore = this.sessionManager?.hasMoreSessions();
    if (!hasMore) {
      console.log('All sessions loaded');
      return;
    }

    this.isLoading = true;
    
    // Request more sessions from the server
    // SessionManager will emit 'request-user-sessions' which RealtimeClient handles
    this.sessionManager?.requestMoreSessions(50);
  }

  /**
   * Handle session updates from the server
   */
  private onSessionsUpdated(event: { sessionIndex: any[], totalSessions: number }) {
    this.isLoading = false;
    
    console.log(`Session index updated:`, {
      loaded: event.sessionIndex.length,
      total: event.totalSessions,
      hasMore: event.sessionIndex.length < event.totalSessions
    });

    // Update UI with new sessions
    this.renderSessions(event.sessionIndex);
  }

  /**
   * Render sessions in the UI
   */
  private renderSessions(sessions: any[]) {
    // Your UI rendering logic here
    sessions.forEach(session => {
      console.log(`- ${session.session_name || 'Untitled'} (${session.session_id})`);
    });
  }

  /**
   * Get current session statistics
   */
  getStats() {
    const stats = this.sessionManager?.getStatistics();
    return {
      loadedSessions: stats?.sessionIndexCount || 0,
      totalAvailable: stats?.totalSessionsAvailable || 0,
      cachedFullSessions: stats?.totalSessions || 0,
    };
  }
}

/**
 * Example 3: React Hook Pattern
 */
function useSessionPagination(client: RealtimeClient) {
  const sessionManager = client.getSessionManager();
  
  // State for sessions
  const [sessions, setSessions] = useState<any[]>([]);
  const [totalSessions, setTotalSessions] = useState(0);
  const [isLoading, setIsLoading] = useState(false);

  useEffect(() => {
    if (!sessionManager) return;

    // Subscribe to updates
    const unsubscribe = sessionManager.on('sessions-index-updated', (event) => {
      setSessions(event.sessionIndex);
      setTotalSessions(event.totalSessions);
      setIsLoading(false);
    });

    // Get initial sessions
    setSessions(sessionManager.getSessionIndex());
    setTotalSessions(sessionManager.getTotalSessionCount());

    return () => {
      unsubscribe();
    };
  }, [sessionManager]);

  const loadMore = useCallback(() => {
    if (isLoading || !sessionManager) return;
    
    if (sessionManager.hasMoreSessions()) {
      setIsLoading(true);
      sessionManager.requestMoreSessions(50);
    }
  }, [sessionManager, isLoading]);

  return {
    sessions,
    totalSessions,
    hasMore: sessions.length < totalSessions,
    isLoading,
    loadMore,
  };
}

/**
 * Example 4: Complete Implementation with Error Handling
 */
async function completeExample() {
  const authManager = new AuthManager({
    apiUrl: 'https://api.example.com',
    autoRefresh: true,
  });

  try {
    // Login and get initial sessions
    const loginResponse = await authManager.login({
      username: 'user@example.com',
      password: 'password123',
    });

    console.log('Initial session batch:', {
      loaded: authManager.getSessions().length,
      total: authManager.getSessionsMetadata()?.total_sessions,
    });

    // Setup RealtimeClient
    const client = new RealtimeClient({
      apiUrl: 'wss://api.example.com/rt/ws',
      authManager,
      enableTurnManager: true,
      debug: true,
    });

    await client.connect();

    const sessionManager = client.getSessionManager();
    if (!sessionManager) {
      throw new Error('SessionManager not initialized');
    }

    // Setup pagination handler
    let currentBatch = 0;
    const batchSize = 50;

    sessionManager.on('sessions-index-updated', (event) => {
      console.log(`Batch ${++currentBatch} loaded:`, {
        newSessions: event.sessionIndex.slice(-batchSize).length,
        totalLoaded: event.sessionIndex.length,
        totalAvailable: event.totalSessions,
      });
    });

    // Load all sessions in batches
    while (sessionManager.hasMoreSessions()) {
      console.log('Fetching next batch...');
      
      // Request next batch
      client.fetchUserSessions(
        sessionManager.getSessionIndex().length,
        batchSize
      );

      // Wait for response (in real app, this would be event-driven)
      await new Promise(resolve => setTimeout(resolve, 1000));
    }

    console.log('All sessions loaded!');
    
    // Display final statistics
    const stats = sessionManager.getStatistics();
    console.log('Final statistics:', {
      sessionIndexCount: stats.sessionIndexCount,
      totalSessionsAvailable: stats.totalSessionsAvailable,
      cachedFullSessions: stats.totalSessions,
    });

    // Resume a specific session (this fetches full session data)
    const sessions = sessionManager.getSessionIndex();
    if (sessions.length > 0) {
      const sessionToResume = sessions[0];
      console.log(`Resuming session: ${sessionToResume.session_name}`);
      
      client.resumeChatSession(sessionToResume.session_id);
      // Server will respond with full ChatSession including messages
    }

  } catch (error) {
    console.error('Error in pagination example:', error);
  }
}

// Export for use in other examples
export {
  basicPaginationExample,
  SessionListManager,
  useSessionPagination,
  completeExample,
};

// Stub implementations for the example
function useState<T>(initial: T): [T, (value: T) => void] {
  return [initial, () => {}];
}

function useEffect(fn: () => (() => void) | void, deps: any[]): void {
  // Stub
}

function useCallback<T extends (...args: any[]) => any>(fn: T, deps: any[]): T {
  return fn;
}