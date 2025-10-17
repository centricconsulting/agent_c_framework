/**
 * React Provider Component for Agent C Realtime SDK
 * Manages the lifecycle of the RealtimeClient instance
 */

import React, { useEffect, useState, useRef, useMemo } from 'react';
import { 
  RealtimeClient, 
  RealtimeClientConfig,
  AuthManager,
  ChatUserDataEvent,
  AgentListEvent,
  AvatarListEvent,
  VoiceListEvent,
  ToolCatalogEvent,
  ChatSessionChangedEvent,
  ErrorEvent
} from '@agentc/realtime-core';
import { AgentCContext, AgentCContextValue, InitializationState } from './AgentCContext';
import { AgentStorage } from '../utils/agentStorage';

/**
 * Configuration props for the AgentCProvider
 */
export interface AgentCProviderProps {
  /** Child components that will have access to the RealtimeClient */
  children: React.ReactNode;
  
  /** WebSocket API URL (e.g., wss://api.example.com/rt/ws) */
  apiUrl?: string;
  
  /** JWT authentication token */
  authToken?: string;
  
  /** Optional AuthManager instance for automatic token management */
  authManager?: AuthManager;
  
  /** Complete configuration object (overrides individual props) */
  config?: RealtimeClientConfig;
  
  /** Whether to automatically connect on mount (default: false) */
  autoConnect?: boolean;
  
  /** Callback when client is successfully initialized */
  onInitialized?: (client: RealtimeClient) => void;
  
  /** Callback when all initialization events have been received */
  onInitializationComplete?: (data: InitializationState) => void;
  
  /** Callback when initialization fails */
  onError?: (error: Error) => void;
  
  /** Enable debug logging */
  debug?: boolean;
}

/**
 * Provider component that creates and manages a singleton RealtimeClient instance
 */
export function AgentCProvider({
  children,
  apiUrl,
  authToken,
  authManager,
  config,
  autoConnect = false,
  onInitialized,
  onInitializationComplete,
  onError,
  debug = false
}: AgentCProviderProps): React.ReactElement {
  const [client, setClient] = useState<RealtimeClient | null>(null);
  const [isInitializing, setIsInitializing] = useState(true);
  const [error, setError] = useState<Error | null>(null);
  
  // Initialization state tracking
  const [initialization, setInitialization] = useState<InitializationState>({
    isInitialized: false,
    receivedEvents: new Set<string>(),
    user: null,
    agents: [],
    avatars: [],
    voices: [],
    tools: [],
    currentSession: null
  });
  
  // Use refs to track initialization and cleanup state for StrictMode compatibility
  const initializationRef = useRef(false);
  const clientRef = useRef<RealtimeClient | null>(null);
  const cleanedUpRef = useRef(false); // Track if we've cleaned up (for StrictMode)
  
  // Store event handler references for proper cleanup
  const handlersRef = useRef<{
    chatUserData?: (data: ChatUserDataEvent) => void;
    agentList?: (data: AgentListEvent) => void;
    avatarList?: (data: AvatarListEvent) => void;
    voiceList?: (data: VoiceListEvent) => void;
    toolCatalog?: (data: ToolCatalogEvent) => void;
    chatSessionChanged?: (data: ChatSessionChangedEvent) => void;
    connected?: () => void;
    disconnected?: (data: { code: number; reason: string }) => void;
    error?: (error: ErrorEvent) => void;
  }>({});
  
  // Build configuration from props or use provided config
  const clientConfig = useMemo((): RealtimeClientConfig | null => {
    // If full config is provided, use it directly
    if (config) {
      return config;
    }
    
    // Build config from individual props
    if (!apiUrl) {
      // Try to get from environment variable
      const envApiUrl = process.env.REACT_APP_AGENTC_API_URL || 
                        process.env.NEXT_PUBLIC_AGENTC_API_URL;
      
      if (!envApiUrl) {
        console.error('AgentCProvider: apiUrl is required. Provide it as a prop or set REACT_APP_AGENTC_API_URL environment variable.');
        return null;
      }
      
      return {
        apiUrl: envApiUrl,
        authToken,
        authManager,
        debug,
        autoReconnect: true,
        enableAudio: false // Audio requires explicit opt-in
      };
    }
    
    return {
      apiUrl,
      authToken,
      authManager,
      debug,
      autoReconnect: true,
      enableAudio: false // Audio requires explicit opt-in
    };
  }, [apiUrl, authToken, authManager, config, debug]);
  
  // Initialize the client
  useEffect(() => {
    // Define cleanup function that will always be returned
    const cleanup = () => {
      // Skip if already cleaned up (prevents double-cleanup in StrictMode)
      if (cleanedUpRef.current) {
        if (debug) {
          console.warn('AgentCProvider: Skipping cleanup - already cleaned up');
        }
        return;
      }
      
      if (clientRef.current) {
        if (debug) {
          // console.log('AgentCProvider: Cleaning up RealtimeClient');
        }
        
        // Mark as cleaned up FIRST to prevent re-entry
        cleanedUpRef.current = true;
        
        // Remove all event listeners BEFORE destroying client
        if (handlersRef.current.chatUserData) {
          clientRef.current.off('chat_user_data', handlersRef.current.chatUserData);
        }
        if (handlersRef.current.agentList) {
          clientRef.current.off('agent_list', handlersRef.current.agentList);
        }
        if (handlersRef.current.avatarList) {
          clientRef.current.off('avatar_list', handlersRef.current.avatarList);
        }
        if (handlersRef.current.voiceList) {
          clientRef.current.off('voice_list', handlersRef.current.voiceList);
        }
        if (handlersRef.current.toolCatalog) {
          clientRef.current.off('tool_catalog', handlersRef.current.toolCatalog);
        }
        if (handlersRef.current.chatSessionChanged) {
          clientRef.current.off('chat_session_changed', handlersRef.current.chatSessionChanged);
        }
        if (handlersRef.current.connected) {
          clientRef.current.off('connected', handlersRef.current.connected);
        }
        if (handlersRef.current.disconnected) {
          clientRef.current.off('disconnected', handlersRef.current.disconnected);
        }
        if (handlersRef.current.error) {
          clientRef.current.off('error', handlersRef.current.error);
        }
        
        // Disconnect if connected
        if (clientRef.current.isConnected()) {
          clientRef.current.disconnect();
        }
        
        // Destroy the client to clean up all resources
        clientRef.current.destroy();
        
        // DON'T null the ref - keep it so we can track state across StrictMode cycles
        // clientRef.current = null;
      }
      
      // Clear handler references
      handlersRef.current = {};
      
      // CRITICAL FIX: Reset initializationRef for React StrictMode
      // React StrictMode double-invokes effects (mount → cleanup → remount)
      // Refs DO persist across this cycle, so we MUST reset the flag
      // to allow the remount to initialize properly.
      initializationRef.current = false;
    };
    
    // Prevent double initialization in React StrictMode
    // Once we've initialized once (initializationRef.current = true), skip subsequent runs
    // This flag persists across StrictMode's cleanup/re-run cycle
    if (initializationRef.current) {
      if (debug) {
        console.warn('AgentCProvider: Skipping initialization - already initialized (StrictMode protection)');
      }
      // Reset cleanup flag so final unmount can clean up
      cleanedUpRef.current = false;
      return cleanup; // Still return cleanup function!
    }
    
    if (!clientConfig) {
      const error = new Error('Invalid configuration: Missing required parameters');
      setError(error);
      setIsInitializing(false);
      onError?.(error);
      return cleanup; // Return cleanup even on error
    }
    
    initializationRef.current = true;
    
    try {
      if (debug) {
        console.warn('AgentCProvider: Initializing RealtimeClient with config:', {
          ...clientConfig,
          authToken: clientConfig.authToken ? '[REDACTED]' : undefined
        });
      }
      
      const newClient = new RealtimeClient(clientConfig);
      clientRef.current = newClient;
      
      // Set up initialization event listeners
      const requiredInitEvents = new Set([
        'chat_user_data',
        'avatar_list',
        'voice_list',
        'agent_list',
        'tool_catalog',
        'chat_session_changed'
      ]);
      
      const updateInitialization = (eventType: string, data: any) => {
        setInitialization(prev => {
          const newState = { ...prev };
          const newReceivedEvents = new Set(prev.receivedEvents);
          newReceivedEvents.add(eventType);
          
          // Update specific data based on event type
          switch (eventType) {
            case 'chat_user_data':
              newState.user = data.user;
              break;
            case 'agent_list':
              newState.agents = data.agents;
              break;
            case 'avatar_list':
              newState.avatars = data.avatars;
              break;
            case 'voice_list':
              newState.voices = data.voices;
              break;
            case 'tool_catalog':
              newState.tools = data.tools;
              break;
            case 'chat_session_changed':
              newState.currentSession = data.chat_session;
              break;
          }
          
          newState.receivedEvents = newReceivedEvents;
          
          // Check if all required events have been received
          const allReceived = Array.from(requiredInitEvents).every(
            event => newReceivedEvents.has(event)
          );
          newState.isInitialized = allReceived;
          
          if (allReceived && !prev.isInitialized) {
            if (debug) {
              console.warn('AgentCProvider: All initialization events received');
            }
            // Invoke callback when initialization is complete
            if (onInitializationComplete) {
              onInitializationComplete(newState);
            }
          }
          
          return newState;
        });
      };
      
      // Create and store event handlers for proper cleanup
      handlersRef.current.chatUserData = (data: ChatUserDataEvent) => {
        if (debug) console.warn('AgentCProvider: Received chat_user_data event');
        updateInitialization('chat_user_data', data);
      };
      
      handlersRef.current.agentList = (data: AgentListEvent) => {
        if (debug) console.warn('AgentCProvider: Received agent_list event');
        updateInitialization('agent_list', data);
      };
      
      handlersRef.current.avatarList = (data: AvatarListEvent) => {
        if (debug) console.warn('AgentCProvider: Received avatar_list event');
        updateInitialization('avatar_list', data);
      };
      
      handlersRef.current.voiceList = (data: VoiceListEvent) => {
        if (debug) console.warn('AgentCProvider: Received voice_list event');
        updateInitialization('voice_list', data);
      };
      
      handlersRef.current.toolCatalog = (data: ToolCatalogEvent) => {
        if (debug) console.warn('AgentCProvider: Received tool_catalog event');
        updateInitialization('tool_catalog', data);
      };
      
      handlersRef.current.chatSessionChanged = (data: ChatSessionChangedEvent) => {
        if (debug) console.warn('AgentCProvider: Received chat_session_changed event');
        updateInitialization('chat_session_changed', data);
      };
      
      // Register event handlers using stored references
      newClient.on('chat_user_data', handlersRef.current.chatUserData);
      newClient.on('agent_list', handlersRef.current.agentList);
      newClient.on('avatar_list', handlersRef.current.avatarList);
      newClient.on('voice_list', handlersRef.current.voiceList);
      newClient.on('tool_catalog', handlersRef.current.toolCatalog);
      newClient.on('chat_session_changed', handlersRef.current.chatSessionChanged);
      
      // Set up connection state listener for debugging
      if (debug) {
        handlersRef.current.connected = () => {
          console.warn('AgentCProvider: Client connected');
        };
        
        handlersRef.current.disconnected = ({ code, reason }: { code: number; reason: string }) => {
          console.warn('AgentCProvider: Client disconnected', { code, reason });
        };
        
        handlersRef.current.error = (error: ErrorEvent) => {
          console.error('AgentCProvider: Client error', error);
        };
        
        newClient.on('connected', handlersRef.current.connected);
        newClient.on('disconnected', handlersRef.current.disconnected);
        newClient.on('error', handlersRef.current.error);
      }
      
      // CRITICAL FIX: Load saved agent preference from localStorage
      // This connects the UI layer's persistence to the Core layer's connection logic
      try {
        const savedAgentKey = AgentStorage.getAgentKey();
        if (savedAgentKey) {
          newClient.setPreferredAgentKey(savedAgentKey);
          if (debug) {
            console.warn('AgentCProvider: Set preferred agent from localStorage:', savedAgentKey);
          }
        }
      } catch (err) {
        // Don't fail initialization if localStorage access fails
        console.warn('AgentCProvider: Failed to load agent preference from localStorage:', err);
      }
      
      setClient(newClient);
      setIsInitializing(false);
      setError(null);
      
      onInitialized?.(newClient);
      
      // Auto-connect if requested
      if (autoConnect) {
        newClient.connect().catch((err: Error) => {
          console.error('AgentCProvider: Auto-connect failed', err);
          const connectError = new Error(`Failed to auto-connect: ${err.message}`);
          setError(connectError);
          onError?.(connectError);
        });
      }
    } catch (err) {
      const initError = err instanceof Error ? err : new Error('Failed to initialize RealtimeClient');
      console.error('AgentCProvider: Initialization failed', initError);
      setError(initError);
      setIsInitializing(false);
      onError?.(initError);
      initializationRef.current = false;
    }
    
    // Return cleanup function (defined at the top of the effect)
    return cleanup;
  }, [clientConfig, autoConnect, onInitialized, onInitializationComplete, onError, debug]);
  
  // Update auth token if it changes
  useEffect(() => {
    if (client && authToken && !authManager) {
      // Only update token if not using AuthManager (which handles its own tokens)
      client.setAuthToken(authToken);
      
      if (debug) {
        // console.log('AgentCProvider: Auth token updated');
      }
    }
  }, [client, authToken, authManager, debug]);
  
  // Update auth manager if it changes
  useEffect(() => {
    if (client && authManager) {
      client.setAuthManager(authManager);
      
      if (debug) {
        // console.log('AgentCProvider: AuthManager updated');
      }
    }
  }, [client, authManager, debug]);
  
  // Context value
  const contextValue = useMemo<AgentCContextValue>(() => ({
    client,
    isInitializing,
    error,
    initialization
  }), [client, isInitializing, error, initialization]);
  
  return (
    <AgentCContext.Provider value={contextValue}>
      {children}
    </AgentCContext.Provider>
  );
}

/**
 * HOC to wrap a component with AgentCProvider
 * Useful for quick setup in examples or tests
 */
export function withAgentCProvider<P extends object>(
  Component: React.ComponentType<P>,
  providerProps: Omit<AgentCProviderProps, 'children'>
): React.ComponentType<P> {
  return function WrappedComponent(props: P) {
    return (
      <AgentCProvider {...providerProps}>
        <Component {...props} />
      </AgentCProvider>
    );
  };
}