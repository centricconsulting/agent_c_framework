'use client';

import React, { useEffect, useRef, useState, useCallback } from 'react';
import { AgentCProvider } from '@agentc/realtime-react';
import { ComparisonChatView } from './ComparisonChatView';
import { RealtimeClient } from '@agentc/realtime-core';

import { useAuth } from '@/contexts/auth-context';
import type { ChatSession } from '@agentc/realtime-core';

interface ResumedPanelProps {
  sessionData: any;
  syncScroll?: boolean;
}

/**
 * Custom hook to simulate resumed session loading without WebSocket
 */
function useResumedSession(sessionData: any, client: RealtimeClient | null) {
  const [isLoaded, setIsLoaded] = useState(false);

  useEffect(() => {
    if (!client || !sessionData || isLoaded) return;

    const loadSession = () => {
      try {
        console.log('[ResumedPanel] Loading resumed session data (without WebSocket)');
        
        // Get the session manager
        const sessionManager = client.getSessionManager();
        if (!sessionManager) {
          console.error('[ResumedPanel] No session manager available');
          return;
        }

        // Convert the session data to proper format
        const chatSession: ChatSession = {
          session_id: sessionData.session_id,
          token_count: sessionData.token_count || 0,
          context_window_size: sessionData.context_window_size || 0,
          session_name: sessionData.session_name || 'Resumed Session',
          created_at: sessionData.created_at,
          updated_at: sessionData.updated_at,
          deleted_at: sessionData.deleted_at || null,
          user_id: sessionData.user_id,
          messages: sessionData.messages || [],
          metadata: sessionData.metadata || {},
          version: sessionData.version || 1,
          vendor: sessionData.vendor || 'agentc',
          display_name: sessionData.display_name || sessionData.session_name || 'Resumed Session'
        };

        // Set the session
        sessionManager.setCurrentSession(chatSession);
        
        // Process messages through EventStreamProcessor if available
        const eventProcessor = (client as any).eventProcessor;
        if (eventProcessor && eventProcessor.processHistoryEvent) {
          console.log('[ResumedPanel] Processing messages through EventStreamProcessor');
          
          // Create a history event with the messages
          const historyEvent = {
            type: 'history',
            chat_session: chatSession
          };
          
          eventProcessor.processHistoryEvent(historyEvent);
        } else {
          // Fallback: emit session-messages-loaded directly
          console.log('[ResumedPanel] Emitting session-messages-loaded event directly');
          
          // Convert messages to the expected format
          const formattedMessages = sessionData.messages.map((msg: any, idx: number) => ({
            ...msg,
            id: `resumed-${idx}`,
            timestamp: new Date().toISOString(),
            isSubSession: false,
            type: 'message'
          }));
          
          sessionManager.emit('session-messages-loaded', {
            sessionId: sessionData.session_id,
            messages: formattedMessages
          });
        }
        
        setIsLoaded(true);
        console.log('[ResumedPanel] Session loaded successfully');
      } catch (error) {
        console.error('[ResumedPanel] Error loading session:', error);
      }
    };

    // Load the session immediately without WebSocket connection
    // Small delay to ensure client is fully initialized
    const timer = setTimeout(() => {
      loadSession();
    }, 100);

    return () => clearTimeout(timer);
  }, [client, sessionData, isLoaded]);

  return isLoaded;
}

/**
 * Resumed panel content
 */
function ResumedContent({ sessionData, syncScroll }: ResumedPanelProps) {
  const scrollRef = useRef<HTMLDivElement>(null);
  const [client, setClient] = useState<RealtimeClient | null>(null);
  
  // Use the resumed session hook
  const isSessionLoaded = useResumedSession(sessionData, client);

  // Handle synchronized scrolling
  useEffect(() => {
    if (!syncScroll || !scrollRef.current) return;

    const handleScroll = (e: Event) => {
      const customEvent = e as CustomEvent<{ scrollTop: number }>;
      if (customEvent.detail && typeof customEvent.detail.scrollTop === 'number') {
        if (scrollRef.current) {
          scrollRef.current.scrollTop = customEvent.detail.scrollTop;
        }
      }
    };

    window.addEventListener('streaming-panel-scroll', handleScroll as EventListener);
    return () => {
      window.removeEventListener('streaming-panel-scroll', handleScroll as EventListener);
    };
  }, [syncScroll]);

  // Emit scroll events for synchronization
  const handleScroll = () => {
    if (!syncScroll || !scrollRef.current) return;
    
    const event = new CustomEvent('resumed-panel-scroll', {
      detail: { scrollTop: scrollRef.current.scrollTop }
    });
    window.dispatchEvent(event);
  };

  // Handle client initialization
  const handleInitialized = useCallback((realtimeClient: RealtimeClient) => {
    console.log('[ResumedPanel] Client initialized');
    setClient(realtimeClient);
  }, []);

  return (
    <div className="h-full flex flex-col">
      {/* Status indicator */}
      {!isSessionLoaded && (
        <div className="px-4 py-2 bg-yellow-50 dark:bg-yellow-900/10 text-yellow-800 dark:text-yellow-200 text-sm">
          Loading session data...
        </div>
      )}

      {/* Messages Area */}
      <div 
        ref={scrollRef}
        className="flex-1 overflow-y-auto"
        onScroll={handleScroll}
      >
        <ComparisonChatView className="h-full" />
      </div>

      {/* Read-only indicator instead of input */}
      <div className="border-t bg-accent/10 p-4 text-center text-sm text-muted-foreground">
        📖 Read-only session view - Loaded from session_with_delegation.json
      </div>
    </div>
  );
}

/**
 * Resumed Panel - Shows pre-loaded session data
 */
export function ResumedPanel({ sessionData, syncScroll }: ResumedPanelProps) {
  const { getAuthToken } = useAuth();
  const token = getAuthToken();

  // Create configuration for the resumed session
  const config = React.useMemo(() => {
    if (!token) return null;

    return {
      apiUrl: process.env.NEXT_PUBLIC_AGENTC_API_URL || 'wss://agent-c-prod.censius.ai',
      authToken: token,
      enableAudio: true,
      autoReconnect: false, // Don't auto-reconnect for resumed view
      reconnection: {
        enabled: false
      },
      audioConfig: {
        sampleRate: 24000,
        channelCount: 1,
        echoCancellation: true,
        noiseSuppression: true,
        autoGainControl: true
      }
    };
  }, [token]);

  if (!config) {
    return (
      <div className="flex items-center justify-center h-full">
        <p className="text-sm text-muted-foreground">Waiting for authentication...</p>
      </div>
    );
  }

  return (
    <AgentCProvider 
      config={config}
      autoConnect={false}
      debug={true}
      onInitialized={(client) => {
        // Manually handle the client without connecting to WebSocket
        console.log('[ResumedPanel] Client initialized without WebSocket connection');
      }}
    >
      <ResumedContent sessionData={sessionData} syncScroll={syncScroll} />
    </AgentCProvider>
  );
}