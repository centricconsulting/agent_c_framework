/**
 * React Context for Agent C Realtime SDK
 * Provides access to the RealtimeClient instance throughout the React component tree
 */

import { createContext, useContext } from 'react';
import type { 
  RealtimeClient,
  Agent,
  Avatar,
  Voice,
  User,
  Tool,
  ChatSession
} from '@agentc/realtime-core';

/**
 * Initialization state tracking
 */
export interface InitializationState {
  /** Whether initialization events have been received */
  isInitialized: boolean;
  /** Tracks which events have been received */
  receivedEvents: Set<string>;
  /** User data from chat_user_data event */
  user: User | null;
  /** Available agents from agent_list event */
  agents: Agent[];
  /** Available avatars from avatar_list event */
  avatars: Avatar[];
  /** Available voices from voice_list event */
  voices: Voice[];
  /** Available tools from tool_catalog event */
  tools: Tool[];
  /** Current chat session from chat_session_changed event */
  currentSession: ChatSession | null;
}

/**
 * Context value containing the RealtimeClient instance and related utilities
 */
export interface AgentCContextValue {
  /** The RealtimeClient instance */
  client: RealtimeClient | null;
  /** Whether the client is currently initializing */
  isInitializing: boolean;
  /** Any error that occurred during initialization */
  error: Error | null;
  /** Initialization state tracking */
  initialization: InitializationState;
}

/**
 * React Context for providing RealtimeClient throughout the app
 */
export const AgentCContext = createContext<AgentCContextValue | undefined>(undefined);

/**
 * Custom hook to access the RealtimeClient from context
 * @throws Error if used outside of AgentCProvider
 */
export function useRealtimeClient(): RealtimeClient {
  const context = useContext(AgentCContext);
  
  if (!context) {
    throw new Error(
      'useRealtimeClient must be used within an AgentCProvider. ' +
      'Please wrap your application with <AgentCProvider> at the root level.'
    );
  }
  
  if (context.error) {
    throw context.error;
  }
  
  if (context.isInitializing) {
    throw new Error(
      'RealtimeClient is still initializing. Please wait for initialization to complete.'
    );
  }
  
  if (!context.client) {
    throw new Error(
      'RealtimeClient is not available. This may indicate a configuration error.'
    );
  }
  
  return context.client;
}

/**
 * Custom hook to access the full context value
 * Useful for checking initialization state or errors
 */
export function useAgentCContext(): AgentCContextValue {
  const context = useContext(AgentCContext);
  
  if (!context) {
    throw new Error(
      'useAgentCContext must be used within an AgentCProvider. ' +
      'Please wrap your application with <AgentCProvider> at the root level.'
    );
  }
  
  return context;
}

/**
 * Custom hook to safely access the RealtimeClient or null
 * Does not throw if client is not available
 */
export function useRealtimeClientSafe(): RealtimeClient | null {
  const context = useContext(AgentCContext);
  
  if (!context) {
    return null;
  }
  
  return context.client;
}

/**
 * Custom hook to access Agent C configuration data (users, agents, avatars, voices, tools)
 * This data is populated from WebSocket events after connection
 */
export function useAgentCData() {
  const context = useContext(AgentCContext);
  
  if (!context) {
    throw new Error(
      'useAgentCData must be used within an AgentCProvider. ' +
      'Please wrap your application with <AgentCProvider> at the root level.'
    );
  }
  
  return {
    user: context.initialization.user,
    agents: context.initialization.agents,
    avatars: context.initialization.avatars,
    voices: context.initialization.voices,
    tools: context.initialization.tools,
    currentSession: context.initialization.currentSession
  };
}

/**
 * Custom hook to check if all initialization events have been received
 * Returns true when all required data has been loaded from the server
 */
export function useInitializationStatus() {
  const context = useContext(AgentCContext);
  
  if (!context) {
    throw new Error(
      'useInitializationStatus must be used within an AgentCProvider. ' +
      'Please wrap your application with <AgentCProvider> at the root level.'
    );
  }
  
  return {
    isInitialized: context.initialization.isInitialized,
    receivedEvents: context.initialization.receivedEvents,
    pendingEvents: getPendingEvents(context.initialization.receivedEvents)
  };
}

/**
 * Helper function to determine which events are still pending
 */
function getPendingEvents(receivedEvents: Set<string>): string[] {
  const requiredEvents = [
    'chat_user_data',
    'avatar_list', 
    'voice_list',
    'agent_list',
    'tool_catalog',
    'chat_session_changed'
  ];
  
  return requiredEvents.filter(event => !receivedEvents.has(event));
}