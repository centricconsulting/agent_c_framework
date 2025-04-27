// src/pages/ChatPage.jsx - fixed version with enhanced debugging
import React, { useEffect } from 'react';
import { useSessionContext } from '@/hooks/use-session-context'; // Use the hook instead of direct context access
import ChatInterface from '../components/chat_interface/ChatInterface';
import { Alert, AlertDescription } from '@/components/ui/alert';
import logger from '@/lib/logger';
import { ErrorBoundary } from '@/components/ui/error-boundary';
import { trackComponentRendering, trackChatInterfaceRendering, DEBUG_MODE } from '@/lib/diagnostic';
import { safeInspect } from '@/lib/safeSerializer';

const ChatPage = () => {
  // Use the useSessionContext hook for better error handling and logging
  const {
    sessionId,
    error,
    isLoading,
    isInitialized,
    isReady,
    isStreaming,
    activeTools,
    settingsVersion,
    handleSessionsDeleted,
    persona,
    personas,
    availableTools,
    customPrompt,
    temperature,
    modelName,
    modelConfigs,
    updateAgentSettings,
    handleEquipTools,
    modelParameters,
    selectedModel,
    handleProcessingStatus,
    isOptionsOpen,
    setIsOptionsOpen
  } = useSessionContext('ChatPage'); // Pass component name for better logging
  
  // Enhanced debug logging
  useEffect(() => {
    // Use our enhanced tracking utility
    trackComponentRendering('ChatPage', 'rendered', {
      hasSessionId: !!sessionId,
      isInitialized,
      isReady,
      hasError: !!error,
      isLoading,
      timestamp: new Date().toISOString()
    });
    
    // Also track specific chat interface rendering decision with proper type handling
    trackChatInterfaceRendering('page-render-check', {
      sessionIdType: typeof sessionId,
      sessionIdValue: sessionId,
      hasValidSessionId,
      isInitialized,
      isReady,
      hasError: !!error,
      errorMessage: error,
      isLoading,
      shouldRender: hasValidSessionId && !!isInitialized,
      timestamp: new Date().toISOString()
    });
    
    // Log this to console for immediate visibility with proper type information - but only in debug mode
    if (DEBUG_MODE) {
      console.log('🔎 ChatPage state:', safeInspect({
        sessionId, // Log the actual value
        sessionIdType: typeof sessionId,
        sessionIdLength: typeof sessionId === 'string' ? sessionId.length : 0,
        hasValidSessionId,
        isInitialized,
        isReady,
        error: error || 'none',
        isLoading,
        timestamp: new Date().toISOString()
      }));
    }
  }, [sessionId, isInitialized, isReady, error, isLoading]);

  // Log rendering decision with proper type handling
  // Check if sessionId is a string (UUID) rather than using it as a boolean
  const hasValidSessionId = typeof sessionId === 'string' && sessionId.length > 0;
  const shouldRenderChatInterface = hasValidSessionId && !!isInitialized;
  
  // Only log in debug mode
  if (DEBUG_MODE) {
    console.log('🔎 ChatPage rendering decision:', safeInspect({
      sessionId, // Log the actual sessionId value
      hasValidSessionId,
      isInitialized: !!isInitialized,
      shouldRenderChatInterface,
      error: error || 'none',
      isLoading
    }));
  }
  
  // Add global diagnostic functions
  if (typeof window !== 'undefined') {
    window.checkChatVisibility = () => {
      const hasValidSessionId = typeof sessionId === 'string' && sessionId.length > 0;
      console.log('Chat visibility check:', {
        sessionId, // Log the actual value
        sessionIdType: typeof sessionId,
        hasValidSessionId,
        isInitialized: !!isInitialized,
        shouldRender: shouldRenderChatInterface,
        timestamp: new Date().toISOString()
      });
      return shouldRenderChatInterface;
    };
  }

  useEffect(() => {
    // Record chat interface render attempts for debugging with proper type information
    if (typeof window !== 'undefined') {
      const hasValidSessionId = typeof sessionId === 'string' && sessionId.length > 0;
      window.__CHAT_INTERFACE_RENDER_CHECK = {
        timestamp: new Date().toISOString(),
        shouldRender: shouldRenderChatInterface,
        sessionId, // Store actual value
        sessionIdType: typeof sessionId,
        hasValidSessionId,
        isInitialized,
        isReady,
        error: error || null
      };
    }
  }, [shouldRenderChatInterface, sessionId, isInitialized, isReady, error]);
  
  if (isLoading) {
    logger.debug('Rendering loading state', 'ChatPage');
    return (
      <div className="flex-1 flex items-center justify-center">
        <div className="text-lg text-muted-foreground animate-pulse">
          Initializing session...
        </div>
      </div>
    );
  }

  // Prepare chatInterface props for easier debugging and error handling
  const chatInterfaceProps = {
    sessionId,
    customPrompt,
    modelName,
    modelParameters,
    onProcessingStatus: handleProcessingStatus,
    persona,
    personas,
    availableTools,
    modelConfigs,
    onEquipTools: handleEquipTools,
    selectedModel,
    onUpdateSettings: updateAgentSettings,
    isInitialized
  };
  
  // Record current props in window for console debugging, but only in debug mode
  if (DEBUG_MODE && typeof window !== 'undefined') {
    // Use a safe copy of the props to prevent circular references
    window.__CHAT_INTERFACE_PROPS = safeInspect(chatInterfaceProps);
  }

  return (
    <div className="flex flex-col flex-1 overflow-hidden chat-page">
      <div className="flex flex-col space-y-2 mb-1">
        {error && (
          <Alert variant="destructive" className="mb-2">
            <AlertDescription className="flex justify-between items-center">
              {error}
              <button
                onClick={() => {
                  // Clear error if needed (or expose a context setter)
                }}
                className="ml-2 text-sm underline hover:no-underline"
              >
                Dismiss
              </button>
            </AlertDescription>
          </Alert>
        )}
      </div>

      {/* Diagnostic element - invisible but helps with debugging */}
      <div 
        data-chat-page="true"
        data-should-render-chat={shouldRenderChatInterface ? 'true' : 'false'}
        data-session-id-type={typeof sessionId}
        data-session-id-valid={(typeof sessionId === 'string' && sessionId.length > 0) ? 'true' : 'false'}
        data-session-id-value={typeof sessionId === 'string' ? sessionId : 'invalid'}
        data-initialized={!!isInitialized ? 'true' : 'false'}
        data-ready={!!isReady ? 'true' : 'false'}
        style={{ display: 'none' }}
      />
      
      {shouldRenderChatInterface ? (
        <div className="flex-1 flex flex-col min-h-0 overflow-hidden">
          <div className="flex-1 overflow-hidden flex flex-col">
            {trackChatInterfaceRendering('about-to-render', { shouldRender: true })}
            <ErrorBoundary 
              name="ChatInterface"
              fallback={(error, errorInfo) => (
                <div className="p-4 bg-red-50 dark:bg-red-900/20 rounded">
                  <h3 className="text-lg font-semibold text-red-700 dark:text-red-300">
                    Error Rendering Chat Interface
                  </h3>
                  <p className="mt-2">
                    Something went wrong while trying to render the chat interface.
                  </p>
                  <details className="mt-2">
                    <summary className="cursor-pointer text-sm font-medium">
                      Error Details
                    </summary>
                    <div className="mt-2 text-xs bg-red-50 dark:bg-red-900/40 p-2 rounded overflow-auto max-h-[200px]">
                      <div className="font-mono">
                        {error.toString()}
                      </div>
                      {errorInfo && (
                        <div className="mt-2 font-mono text-gray-600 dark:text-gray-400 whitespace-pre-wrap">
                          {errorInfo.componentStack}
                        </div>
                      )}
                    </div>
                  </details>
                  <div className="mt-4">
                    <button 
                      onClick={() => window.location.reload()} 
                      className="px-4 py-2 bg-red-100 dark:bg-red-800 rounded hover:bg-red-200 dark:hover:bg-red-700 transition-colors"
                    >
                      Reload Page
                    </button>
                  </div>
                </div>
              )}
              onError={(error) => {
                logger.error('Error in ChatInterface', 'ChatPage', {
                  error: error.message,
                  stack: error.stack
                });
              }}
            >
              <ChatInterface 
                {...chatInterfaceProps}
              />
              {trackChatInterfaceRendering('render-attempted', { props: Object.keys(chatInterfaceProps) })}
            </ErrorBoundary>
          </div>
        </div>
      ) : (
        <div className="flex-1 flex items-center justify-center">
          <div className="max-w-lg p-6 border border-red-200 dark:border-red-800 rounded-lg bg-white dark:bg-slate-900 shadow-lg">
            <h2 className="text-xl font-semibold mb-4 text-red-600 dark:text-red-400">Chat Interface Not Available</h2>
            
            <div className="text-sm mb-4 p-3 bg-amber-50 dark:bg-amber-900/20 rounded-md">
              {error ? (
                <div className="text-red-700 dark:text-red-300">Error: {error}</div>
              ) : isLoading ? (
                <div className="text-amber-700 dark:text-amber-300">Loading session...</div>
              ) : (
                <div className="text-slate-700 dark:text-slate-300">
                  The chat interface cannot be displayed. This may be due to initialization issues.
                </div>
              )}
            </div>

            <div className="bg-slate-50 dark:bg-slate-800 p-3 rounded-md mb-4">
              <h3 className="font-medium mb-2 text-slate-700 dark:text-slate-300">Diagnostic Information</h3>
              <div className="grid grid-cols-2 gap-2 text-sm">
                <div className="text-slate-600 dark:text-slate-400">Session ID:</div>
                <div className={sessionId ? 'text-green-600 dark:text-green-400' : 'text-red-600 dark:text-red-400'}>
                  {sessionId ? 'Present' : 'Missing'}
                </div>
                
                <div className="text-slate-600 dark:text-slate-400">Initialized:</div>
                <div className={isInitialized ? 'text-green-600 dark:text-green-400' : 'text-red-600 dark:text-red-400'}>
                  {isInitialized ? 'Yes' : 'No'}
                </div>
                
                <div className="text-slate-600 dark:text-slate-400">Ready:</div>
                <div className={isReady ? 'text-green-600 dark:text-green-400' : 'text-red-600 dark:text-red-400'}>
                  {isReady ? 'Yes' : 'No'}
                </div>
                
                <div className="text-slate-600 dark:text-slate-400">Is Loading:</div>
                <div className={!isLoading ? 'text-green-600 dark:text-green-400' : 'text-amber-600 dark:text-amber-400'}>
                  {isLoading ? 'Yes' : 'No'}
                </div>
              </div>
            </div>
            
            <div className="mt-4 text-center">
              <button 
                onClick={() => window.location.reload()} 
                className="px-4 py-2 bg-blue-100 dark:bg-blue-800 rounded hover:bg-blue-200 dark:hover:bg-blue-700 transition-colors text-blue-700 dark:text-blue-200"
              >
                Reload Page
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
};

export default ChatPage;