'use client';

import React, { useEffect } from 'react';
import { useInitializationStatus, useAgentCData } from '@agentc/realtime-react';
import { Skeleton, Alert, AlertDescription } from '@agentc/realtime-ui';
import { CheckCircle2, Circle, Loader2 } from 'lucide-react';
import { Logger } from '@/utils/logger';

/**
 * Wrapper component that displays initialization status
 * Shows loading state while waiting for WebSocket events
 */
export interface InitializationWrapperProps {
  children: React.ReactNode;
  showDebug?: boolean;
}

export function InitializationWrapper({ 
  children, 
  showDebug = false 
}: InitializationWrapperProps) {
  const { isInitialized, isLoading, connectionState } = useInitializationStatus();
  const { data } = useAgentCData();
  const { user, agents, avatars, voices, tools } = data;
  
  // Log initialization state for debugging
  useEffect(() => {
    if (showDebug) {
      Logger.debug('[InitializationWrapper] Status:', {
        isInitialized,
        isLoading,
        connectionState,
        hasUser: !!user,
        agentsCount: agents.length,
        avatarsCount: avatars.length,
        voicesCount: voices.length,
        toolsCount: tools.length
      });
    }
  }, [isInitialized, isLoading, connectionState, user, agents, avatars, voices, tools, showDebug]);
  
  // Show loading state while initializing
  if (!isInitialized) {
    return (
      <div className="flex flex-col items-center justify-center min-h-screen space-y-6 p-4">
        <div className="space-y-4 w-full max-w-md">
          <div className="flex items-center justify-center mb-4">
            <Loader2 className="h-8 w-8 animate-spin text-primary" />
          </div>
          
          <h2 className="text-lg font-semibold text-center">
            Initializing Agent C Realtime...
          </h2>
          
          <div className="space-y-2">
            <InitStatusItem 
              name="User Profile" 
              eventKey="chat_user_data"
              received={!!user} 
            />
            <InitStatusItem 
              name="Available Agents" 
              eventKey="agent_list"
              received={agents.length > 0} 
            />
            <InitStatusItem 
              name="Avatar Library" 
              eventKey="avatar_list"
              received={avatars.length > 0} 
            />
            <InitStatusItem 
              name="Voice Models" 
              eventKey="voice_list"
              received={voices.length > 0} 
            />
            <InitStatusItem 
              name="Tool Catalog" 
              eventKey="tool_catalog"
              received={tools.length > 0} 
            />
            <InitStatusItem 
              name="Chat Session" 
              eventKey="chat_session_changed"
              received={isInitialized} 
            />
          </div>
          
          {isLoading && (
            <p className="text-sm text-muted-foreground text-center mt-4">
              Loading initialization data...
            </p>
          )}
        </div>
      </div>
    );
  }
  
  // Show debug info if enabled
  if (showDebug && isInitialized) {
    return (
      <>
        <div className="fixed top-0 right-0 m-4 p-4 bg-card border rounded-lg shadow-lg z-50 max-w-xs">
          <h3 className="text-sm font-semibold mb-2">Initialization Complete</h3>
          <div className="text-xs space-y-1">
            <div>User: {user?.user_name || 'Unknown'}</div>
            <div>Agents: {agents.length}</div>
            <div>Avatars: {avatars.length}</div>
            <div>Voices: {voices.length}</div>
            <div>Tools: {tools.length}</div>
          </div>
        </div>
        {children}
      </>
    );
  }
  
  // Render children when initialized
  return <>{children}</>;
}

/**
 * Individual initialization status item
 */
function InitStatusItem({ 
  name, 
  eventKey, 
  received 
}: { 
  name: string;
  eventKey: string;
  received: boolean;
}) {
  return (
    <div className="flex items-center justify-between p-2 rounded-lg bg-muted/50">
      <span className="text-sm">{name}</span>
      <div className="flex items-center gap-2">
        {received ? (
          <>
            <span className="text-xs text-muted-foreground">Ready</span>
            <CheckCircle2 className="h-4 w-4 text-green-500" />
          </>
        ) : (
          <>
            <span className="text-xs text-muted-foreground">Loading...</span>
            <Circle className="h-4 w-4 text-muted-foreground" />
          </>
        )}
      </div>
    </div>
  );
}