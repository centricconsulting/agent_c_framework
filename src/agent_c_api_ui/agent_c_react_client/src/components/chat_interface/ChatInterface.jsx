import React, { useState, useRef, useEffect } from "react";
import { 
  Card,
  CardContent,
  CardFooter
} from "@/components/ui/card";
import { Separator } from "@/components/ui/separator";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { X } from "lucide-react";
import { ScrollArea } from "@/components/ui/scroll-area";
import { useSessionContext } from '@/hooks/use-session-context';
import { API_URL } from "@/config/config";
import logger from '@/lib/logger';
import { trackComponentRendering, trackChatInterfaceRendering, DEBUG_MODE } from '@/lib/diagnostic';
import storageService from '@/lib/storageService';
import { safeInspect } from '@/lib/safeSerializer';

// Import our refactored components
import MessagesList from './MessagesList';
import StatusBar from './StatusBar';
import CollapsibleOptions from './CollapsibleOptions';
import ChatInputArea from './ChatInputArea';
import FileUploadManager from './FileUploadManager';
import DragDropArea from './DragDropArea';
import { ToolCallProvider, useToolCalls } from './ToolCallContext';
import { MessageProvider, useMessageContext } from '@/contexts/MessageContext';
import ExportHTMLButton from './ExportHTMLButton';

/**
 * Main chat interface inner component that uses the tool calls context and message context
 */
const ChatInterfaceInner = ({
  sessionId, 
  customPrompt, 
  modelName, 
  modelParameters = {}, // Provide default empty object to prevent undefined errors
  onProcessingStatus,
  // Added props for options panel
  persona,
  personas,
  availableTools,
  onEquipTools,
  modelConfigs,
  selectedModel,
  onUpdateSettings,
  isInitialized,
  className
}) => {
  // Access tool call context
  const { toolSelectionState } = useToolCalls();
  
  // Access message context
  const {
    messages,
    isStreaming,
    isUploading,
    expandedToolCallMessages,
    selectedFiles,
    handleSendMessage,
    handleCancelStream,
    toggleToolCallExpansion,
    setSelectedFiles,
    removeSelectedFile,
    getChatCopyContent,
    getChatCopyHTML
  } = useMessageContext('ChatInterfaceInner');
  
  // Access SessionContext for StatusBar props and options panel state
  const { 
    isReady, 
    activeTools, 
    settingsVersion,
    isOptionsOpen,
    setIsOptionsOpen
  } = useSessionContext('ChatInterfaceInner');
  
  // State for input
  const [inputText, setInputText] = useState("");
  
  // Refs
  const fileInputRef = useRef(null);
  
  // Effect to notify parent of processing status
  useEffect(() => {
    if (onProcessingStatus) {
      onProcessingStatus(isStreaming);
    }
  }, [isStreaming, onProcessingStatus]);
  
  /**
   * Handles file drop from drag and drop
   * @param {FileList} files - The dropped files
   */
  const handleFileDrop = (files) => {
    if (files && files.length > 0) {
      if (DEBUG_MODE) console.log('File dropped:', files[0].name);
      
      // Since we can only process one file at a time, take the first file
      const file = files[0];
      
      // Directly call the file upload function in FileUploadManager instead of
      // trying to simulate a change event on the input
      if (fileInputRef.current) {
        // Update the file input to maintain consistency
        try {
          const dataTransfer = new DataTransfer();
          dataTransfer.items.add(file);
          fileInputRef.current.files = dataTransfer.files;
        } catch (err) {
          console.warn('Could not update file input element:', err);
          // Continue anyway as we'll handle the file directly
        }
      }
      
      // Call the handleFileSelection function which will trigger the upload
      handleFileSelection({
        target: {
          files: files
        }
      });
    }
  };
  
  /**
   * Handles files pasted from clipboard
   * @param {Array<File>} files - The files from clipboard
   */
  const handleClipboardPaste = (files) => {
    if (files && files.length > 0) {
      if (DEBUG_MODE) console.log('File pasted from clipboard:', files[0].name);
      
      // Since we can only process one file at a time in the current implementation,
      // take the first file - this can be extended to handle multiple files later
      const file = files[0];
      
      // Process the pasted file
      // Directly call the file upload function similar to drag and drop handling
      if (fileInputRef.current) {
        // Update the file input to maintain consistency
        try {
          const dataTransfer = new DataTransfer();
          dataTransfer.items.add(file);
          fileInputRef.current.files = dataTransfer.files;
        } catch (err) {
          console.warn('Could not update file input element:', err);
          // Continue anyway as we'll handle the file directly
        }
      }
      
      // Call the handleFileSelection function which will trigger the upload
      handleFileSelection({
        target: {
          files: [file]
        }
      });
    }
  };

  /**
   * Handles file selection from the file picker or drag and drop
   * @param {Event} e - The change event or a synthetic event with files
   */
  const handleFileSelection = (e) => {
    if (e.target.files && e.target.files.length > 0) {
      if (DEBUG_MODE) console.log('File selected:', e.target.files[0].name);
      
      // We need to upload the file directly here instead of relying on FileUploadManager
      // to pick up the change event, which may not be happening reliably
      const file = e.target.files[0];
      
      // Create a FormData object to upload the file
      const formData = new FormData();
      formData.append("ui_session_id", sessionId);
      formData.append("file", file);
      
      // Perform the upload
      fetch(`${API_URL}/upload_file`, {
        method: "POST",
        body: formData,
      })
      .then(response => {
        if (!response.ok) {
          throw new Error(`HTTP error! status: ${response.status}`);
        }
        return response.json();
      })
      .then(data => {
        if (DEBUG_MODE) console.log('Upload successful:', safeInspect(data));
        
        // Create a file object that matches what FileUploadManager expects
        const newFile = {
          id: data.id,
          name: data.filename,
          type: data.mime_type,
          size: data.size,
          selected: true,
          processing_status: "pending",
          processing_error: null
        };
        
        // Update selected files in MessageContext
        setSelectedFiles([...selectedFiles, newFile]);
        
        // Start monitoring the processing status
        checkFileProcessingStatus(data.id);
      })
      .catch(error => {
        console.error("Error uploading file:", error?.message || error);
      });
    }
  };
  
  /**
   * Checks the processing status of a file
   * @param {string} fileId - The ID of the file to check
   */
  const checkFileProcessingStatus = async (fileId) => {
    // Poll the server every 2 seconds to check processing status
    let attempts = 0;
    const maxAttempts = 15; // 15 attempts * 2 seconds = 30 seconds max
    
    const checkStatus = async () => {
      try {
        const response = await fetch(`${API_URL}/files/${sessionId}`);
        if (!response.ok) {
          console.error(`Error fetching file status: ${response.status}`);
          return true; // Stop polling on error
        }

        const data = await response.json();

        // Find the file in the response
        const fileData = data.files.find(f => f.id === fileId);
        if (!fileData) return false;

        // Update our selected files with the current status
        setSelectedFiles(selectedFiles.map(file => {
          if (file.id === fileId) {
            return {
              ...file,
              processing_status: fileData.processing_status,
              processing_error: fileData.processing_error
            };
          }
          return file;
        }));

        // If processing is complete or failed, stop polling
        return fileData.processing_status !== "pending";
      } catch (error) {
        console.error("Error checking file status:", error?.message || error);
        return true; // Stop polling on error
      }
    };

    const pollTimer = setInterval(async () => {
      attempts++;
      const shouldStop = await checkStatus();

      if (shouldStop || attempts >= maxAttempts) {
        clearInterval(pollTimer);

        // If we hit max attempts and status is still pending, mark as failed
        if (attempts >= maxAttempts) {
          setSelectedFiles(selectedFiles.map(file => {
            if (file.id === fileId && file.processing_status === "pending") {
              return {
                ...file,
                processing_status: "failed",
                processing_error: "Processing timed out"
              };
            }
            return file;
          }));
        }
      }
    }, 2000);
  };
  
  /**
   * Opens the file picker
   */
  const openFilePicker = () => {
    FileUploadManager.openFilePicker(fileInputRef);
  };
  
  /**
   * Handles sending a message using the MessageContext
   */
  const sendMessage = () => {
    if ((!inputText.trim() && selectedFiles.length === 0) || isStreaming) return;
    
    handleSendMessage(inputText, {
      modelName,
      modelParameters,
      customPrompt
    });
    
    setInputText("");
  };
  
  /**
   * Handles keyboard shortcuts for sending messages
   * @param {KeyboardEvent} e - Keyboard event
   */
  const handleKeyPress = (e) => {
    if (e.key === "Enter" && !e.shiftKey) {
      e.preventDefault();
      sendMessage();
    }
  };
  
  // Toggle options panel visibility
  const toggleOptionsPanel = () => {
    setIsOptionsOpen(!isOptionsOpen);
  };
  
  return (
    <DragDropArea 
      onFileDrop={handleFileDrop}
      disabled={isStreaming}
      className="chat-interface-container"
    >
      <Card className="chat-interface-card">
        {/* Messages list with ScrollArea for better scrolling experience */}
        <CardContent className="chat-interface-messages flex-grow p-0 overflow-hidden">
          <ScrollArea className="h-full w-full" type="auto">
            <div className="p-2">
              <MessagesList 
                messages={messages}
                expandedToolCallMessages={expandedToolCallMessages}
                toggleToolCallExpansion={toggleToolCallExpansion}
                toolSelectionInProgress={toolSelectionState.inProgress}
                toolSelectionName={toolSelectionState.toolName}
              />
            </div>
          </ScrollArea>
        </CardContent>
        
        {/* Options Panel - conditionally rendered between messages and input */}
        {isOptionsOpen && (
          <div className="mx-4 mb-2">
            <Separator className="mb-2" />
            <CollapsibleOptions
              isOpen={isOptionsOpen}
              setIsOpen={setIsOptionsOpen}
              persona={persona}
              personas={personas}
              availableTools={availableTools}
              customPrompt={customPrompt}
              temperature={modelParameters.temperature}
              modelName={modelName}
              modelConfigs={modelConfigs || []} // Ensure we always pass an array
              sessionId={sessionId}
              isReady={isReady}
              onEquipTools={onEquipTools}
              activeTools={activeTools}
              modelParameters={modelParameters}
              selectedModel={selectedModel}
              onUpdateSettings={(type, values) => {
                console.log('ChatInterface: onUpdateSettings called with:', { type, values });
                if (typeof onUpdateSettings !== 'function') {
                  console.error('ChatInterface: onUpdateSettings is not a function', onUpdateSettings);
                  return;
                }
                return onUpdateSettings(type, values);
              }}
              isInitialized={isInitialized}
            />
            <Separator className="mt-2" />
          </div>
        )}
        
        {/* Footer with file upload and chat input */}
        <CardFooter className="chat-interface-input-area flex-col space-y-3 px-4 py-3">
          {/* File management component */}
          <FileUploadManager
            sessionId={sessionId}
            fileInputRef={fileInputRef}
            uploadedFiles={selectedFiles}
          />
          
          {/* Display selected files as badges */}
          {selectedFiles.length > 0 && (
            <div className="chat-interface-selected-files">
              <div className="flex flex-wrap gap-2">
                {selectedFiles.map((file) => (
                  <Badge key={file.id} variant="secondary" className="flex items-center gap-1">
                    {file.name}
                    <Button 
                      variant="ghost" 
                      size="sm" 
                      className="h-4 w-4 p-0 hover:bg-transparent" 
                      onClick={() => removeSelectedFile(file.id)}
                    >
                      <X className="h-3 w-3" />
                    </Button>
                  </Badge>
                ))}
              </div>
              <Separator className="mt-2" />
            </div>
          )}
          
          {/* Chat input area component */}
          <ChatInputArea
            inputText={inputText}
            setInputText={setInputText}
            isStreaming={isStreaming}
            isUploading={isUploading}
            handleSendMessage={sendMessage}
            handleKeyPress={handleKeyPress}
            openFilePicker={openFilePicker}
            toggleOptionsPanel={toggleOptionsPanel}
            fileInputRef={fileInputRef}
            handleFileSelection={handleFileSelection}
            handleCancelStream={handleCancelStream}
            handleClipboardPaste={handleClipboardPaste}
          />
          
          {/* StatusBar positioned just below the input */}
          <div className="chat-interface-status-bar">
            <StatusBar
              isReady={isReady}
              activeTools={activeTools}
              sessionId={sessionId}
              settingsVersion={settingsVersion}
              isProcessing={isStreaming}
              getChatCopyContent={getChatCopyContent}
              getChatCopyHTML={getChatCopyHTML}
              messages={messages}
            />
          </div>
        </CardFooter>
      </Card>
    </DragDropArea>
  );
};

/**
 * ChatInterface component provides a complete chat interface with support for
 * message streaming, file uploads, and various message types including text,
 * media, and tool calls.
 */
const ChatInterface = (props) => {
  console.group('🔍 ChatInterface Mounting');
  
  // Prepare props info for logging
  const propsInfo = {
    sessionId: props.sessionId,
    isInitialized: props.isInitialized,
    hasCustomPrompt: !!props.customPrompt,
    modelName: props.modelName,
    hasModelParameters: !!props.modelParameters,
    propsKeys: Object.keys(props)
  };
  
  if (DEBUG_MODE) console.log('Props received:', safeInspect(propsInfo));
  
  // Track component mounting with our enhanced tracking
  trackChatInterfaceRendering('start', {
    props: propsInfo,
    timestamp: Date.now(),
    isMounting: true
  });
  
  // Check DOM after render using useEffect
  useEffect(() => {
    // Log session ID and debug information
    if (DEBUG_MODE) console.log('📋 Session ID check from ChatInterface:', {
      propSessionId: props.sessionId,
      storedSessionId: storageService.getSessionId(),
      match: props.sessionId === storageService.getSessionId()
    });
    
    // Track the session ID match status
    trackChatInterfaceRendering('session-id-check', {
      propSessionId: props.sessionId,
      storedSessionId: storageService.getSessionId(),
      match: props.sessionId === storageService.getSessionId(),
      storage: JSON.stringify({
        hasSessionId: !!storageService.getSessionId(),
        sessionIdLength: storageService.getSessionId()?.length
      })
    });

    const chatInterfaceElement = document.querySelector('.chat-interface-card');
    const chatInterfaceContainer = document.querySelector('.chat-interface-container');
    
    const domStatus = {
      cardExists: !!chatInterfaceElement,
      containerExists: !!chatInterfaceContainer,
      visible: chatInterfaceElement ? (
        window.getComputedStyle(chatInterfaceElement).display !== 'none' &&
        window.getComputedStyle(chatInterfaceElement).visibility !== 'hidden'
      ) : false
    };
    
    if (DEBUG_MODE) console.log('Chat interface in DOM after mount:', safeInspect(domStatus));
    
    // Track mounting with our enhanced tracking
    trackChatInterfaceRendering('mount', {
      inDOM: domStatus.cardExists,
      visible: domStatus.visible,
      sessionId: props.sessionId,
      isInitialized: props.isInitialized,
      timestamp: Date.now()
    });
    
    // Deeper DOM inspection for debugging
    if (chatInterfaceElement) {
      // Check parent visibility up to 3 levels
      let parent = chatInterfaceElement.parentElement;
      let depth = 0;
      const parentInfo = [];
      
      while (parent && depth < 3) {
        const style = window.getComputedStyle(parent);
        parentInfo.push({
          tag: parent.tagName,
          className: parent.className,
          display: style.display,
          visibility: style.visibility,
          height: style.height,
          overflow: style.overflow
        });
        parent = parent.parentElement;
        depth++;
      }
      
      if (DEBUG_MODE) console.log('Chat interface parent elements:', safeInspect(parentInfo));
    }
    
    console.groupEnd();
    
    // Removed interval-based visibility checking to improve performance
    // One-time check is sufficient for initialization
    const element = document.querySelector('.chat-interface-card');
    if (element) {
      const style = window.getComputedStyle(element);
      const isVisible = style.display !== 'none' && style.visibility !== 'hidden';
      
      // Set initial visibility state
      domStatus.visible = isVisible;
      
      // Initial tracking
      trackChatInterfaceRendering('update', {
        inDOM: true,
        visible: isVisible,
        initialCheck: true
      });
    }
    
    return () => {
      logger.debug('ChatInterface unmounting', 'ChatInterface');
      trackChatInterfaceRendering('unmount', {
        reason: 'component-unmount',
        sessionId: props.sessionId
      });
    };
  }, [props.sessionId, props.isInitialized]);
  
  return (
    <ToolCallProvider>
      <MessageProvider>
        {/* Invisible diagnostic element */}
        <div 
          data-testid="chat-interface"
          data-chat-interface-mounted="true"
          data-session-id={!!props.sessionId}
          data-is-initialized={props.isInitialized}
          data-is-ready={props.isReady}
          data-session-id-value={props.sessionId || 'missing'}
          data-stored-session-id={storageService.getSessionId() || 'missing'}
          style={{ display: 'none' }}
        />
        
        <ChatInterfaceInner {...props} />
      </MessageProvider>
    </ToolCallProvider>
  );
};

export default ChatInterface;