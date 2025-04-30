// src/pages/ChatPage.jsx
import React, { useContext } from 'react';
import { LegacySessionContext } from '@/contexts/LegacySessionContext';
import ChatInterface from '../components/chat_interface/ChatInterface';
// Remove the CollapsibleOptions import since it will be used directly in ChatInterface
// import CollapsibleOptions from '@/components/chat_interface//CollapsibleOptions';
import { Alert, AlertDescription } from '@/components/ui/alert';

const ChatPage = () => {
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
  } = useContext(LegacySessionContext);

  if (isLoading) {
    return (
      <div className="flex-1 flex items-center justify-center">
        <div className="text-lg text-muted-foreground animate-pulse">
          Initializing session...
        </div>
      </div>
    );
  }

  return (
    <div className="flex flex-col flex-1 overflow-hidden">
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

      {sessionId && isInitialized && (
        <div className="flex-1 flex flex-col min-h-0 overflow-hidden">
          {/* CollapsibleOptions removed from here since it's now in ChatInterface */}
          
          <div className="flex-1 overflow-hidden flex flex-col">
            <ChatInterface
              sessionId={sessionId}
              customPrompt={customPrompt}
              modelName={modelName}
              modelParameters={modelParameters}
              onProcessingStatus={handleProcessingStatus}
              // Added props for options panel
              persona={persona}
              personas={personas}
              availableTools={availableTools}
              modelConfigs={modelConfigs}
              onEquipTools={handleEquipTools}
              selectedModel={selectedModel}
              onUpdateSettings={updateAgentSettings}
              isInitialized={isInitialized}
            />
            {/* StatusBar moved to ChatInterface component */}
          </div>
        </div>
      )}
    </div>
  );
};

export default ChatPage;