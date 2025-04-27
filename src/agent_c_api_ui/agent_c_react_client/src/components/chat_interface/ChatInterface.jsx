import React, { useState, useRef, useEffect, useCallback } from "react";
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
import { createClipboardContent } from '@/components/chat_interface/utils/htmlChatFormatter';
import { processMessageStream } from './utils/MessageStreamProcessor';
import { cn } from "@/lib/utils";

// Import our refactored components
import MessagesList from './MessagesList';
import StatusBar from './StatusBar';
import CollapsibleOptions from './CollapsibleOptions';
import ChatInputArea from './ChatInputArea';
import FileUploadManager from './FileUploadManager';
import DragDropArea from './DragDropArea';
import { ToolCallProvider, useToolCalls } from './ToolCallContext';
import ExportHTMLButton from './ExportHTMLButton';

/**
 * Main chat interface inner component that uses the tool calls context
 */
const ChatInterfaceInner = ({
  sessionId, 
  customPrompt, 
  modelName, 
  modelParameters, 
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
  const { 
    handleToolStart, 
    handleToolEnd, 
    updateToolSelectionState,
    toolSelectionState
  } = useToolCalls();
  
  // Access SessionContext for StatusBar props and options panel state
  const { 
    isReady, 
    activeTools, 
    settingsVersion,
    isOptionsOpen,
    setIsOptionsOpen
  } = useSessionContext('ChatInterfaceInner');
  
  // State for messages and UI
  const [messages, setMessages] = useState([]);
  const [inputText, setInputText] = useState("");
  const [isStreaming, setIsStreaming] = useState(false);
  const [isUploading, setIsUploading] = useState(false);
  const [expandedToolCallMessages, setExpandedToolCallMessages] = useState([]);
  const [selectedFiles, setSelectedFiles] = useState([]);
  
  // Refs
  const fileInputRef = useRef(null);
  
  // Effect to notify parent of processing status
  useEffect(() => {
    if (onProcessingStatus) {
      onProcessingStatus(isStreaming);
    }
  }, [isStreaming, onProcessingStatus]);
  
  // Toggle the expanded state of tool calls for a specific message
  const toggleToolCallExpansion = (messageIdx) => {
    setExpandedToolCallMessages(prev => {
      if (prev.includes(messageIdx)) {
        return prev.filter(idx => idx !== messageIdx);
      } else {
        return [...prev, messageIdx];
      }
    });
  };
  
  // Helper function to format a message for copying
  const formatMessageForCopy = useCallback((msg) => {
    if (msg.role === 'user') {
      return `User: ${msg.content}\n`;
    } else if (msg.role === 'assistant' && msg.type === 'content') {
      return `Assistant: ${msg.content}\n`;
    } else if (msg.role === 'assistant' && msg.type === 'thinking') {
      return `Assistant (thinking): ${msg.content}\n`;
    } else if (msg.type === 'tool_calls') {
      // Format tool calls
      let result = `Assistant (tool): Using ${msg.toolCalls.map(t => t.name || t.function?.name).join(', ')}\n`;
      msg.toolCalls.forEach(tool => {
        const toolName = tool.name || tool.function?.name;
        const toolArgs = tool.arguments || tool.function?.arguments;
        if (toolArgs) {
          result += `  ${toolName} Arguments: ${typeof toolArgs === 'string' ? toolArgs : JSON.stringify(toolArgs)}\n`;
        }
        if (tool.results) {
          result += `  ${toolName} Results: ${typeof tool.results === 'string' ? tool.results : JSON.stringify(tool.results)}\n`;
        }
      });
      return result;
    } else if (msg.type === 'media') {
      return `Assistant (media): Shared ${msg.contentType} content\n`;
    } else if (msg.role === 'system') {
      return `System: ${msg.content}\n`;
    }
    return '';
  }, []);
  
  // Format the entire chat for copying
  const formatChatForCopy = useCallback(() => {
    return messages.map(formatMessageForCopy).join('\n');
  }, [messages, formatMessageForCopy]);
  
  // Get both text and HTML versions for the entire chat
  const getChatCopyContent = useCallback(() => {
    const clipboardContent = createClipboardContent(messages);
    return clipboardContent.text;
  }, [messages]);
  
  // Get HTML version for rich copying
  const getChatCopyHTML = useCallback(() => {
    const clipboardContent = createClipboardContent(messages);
    return clipboardContent.html;
  }, [messages]);
  
  /**
   * Handles file drop from drag and drop
   * @param {FileList} files - The dropped files
   */
  /**
   * Handles file drop from drag and drop
   * @param {FileList} files - The dropped files
   */
  const handleFileDrop = (files) => {
    if (files && files.length > 0) {
      console.log('File dropped:', files[0].name);
      
      // Set uploading state first
      setIsUploading(true);
      
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
      console.log('File pasted from clipboard:', files[0].name);
      
      // Set uploading state first
      setIsUploading(true);
      
      // Since we can only process one file at a time in the current implementation,
      // take the first file - this can be extended to handle multiple files later
      const file = files[0];
      
      // We no longer need to add a message here as the file uploaded message will be sufficient
      
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
   * Handles file upload notification
   * @param {Object} file - The file that was uploaded
   */
  const handleFileUploaded = (file) => {
    setMessages((prev) => [
      ...prev,
      {
        role: "system",
        type: "content",
        content: `File uploaded: ${file.name}`,
      },
    ]);
    setIsUploading(false);
  };
  
  /**
   * Handles file upload error
   * @param {string} errorMessage - The error message
   */
  const handleFileError = (errorMessage) => {
    setMessages((prev) => [
      ...prev,
      {
        role: "system",
        type: "error",
        content: `Error uploading file: ${errorMessage}`,
      },
    ]);
    setIsUploading(false);
  };
  
  /**
   * Handles changes to selected files
   * @param {Array} files - The currently selected files
   */
  const handleSelectedFilesChange = (files) => {
    setSelectedFiles(files);
  };

  /**
   * Handles file selection from the file picker or drag and drop
   * @param {Event} e - The change event or a synthetic event with files
   */
  const handleFileSelection = (e) => {
    if (e.target.files && e.target.files.length > 0) {
      console.log('File selected:', e.target.files[0].name);
      setIsUploading(true);
      
      // We need to upload the file directly here instead of relying on FileUploadManager
      // to pick up the change event, which may not be happening reliably
      const file = e.target.files[0];
      
      // Create a FormData object to upload the file
      const formData = new FormData();
      formData.append("ui_session_id", sessionId);
      formData.append("file", file);
      
      // Add the file to the UI first to provide immediate feedback
      setMessages((prev) => [
        ...prev,
        {
          role: "system",
          type: "content",
          content: `Uploading file: ${file.name}...`,
        },
      ]);
      
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
        console.log('Upload successful:', data);
        
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
        
        // Update selected files
        setSelectedFiles(prev => [...prev, newFile]);
        
        // Notify UI of successful upload
        setMessages((prev) => {
          // Replace the 'uploading' message with 'uploaded'
          const lastIndex = prev.length - 1;
          const lastMessage = prev[lastIndex];
          
          if (lastMessage.role === "system" && lastMessage.content.startsWith("Uploading file: ")) {
            // Replace the last message
            return [
              ...prev.slice(0, lastIndex),
              {
                role: "system",
                type: "content",
                content: `File uploaded: ${file.name}`,
              },
            ];
          }
          
          // Or just add a new message
          return [
            ...prev,
            {
              role: "system",
              type: "content",
              content: `File uploaded: ${file.name}`,
            },
          ];
        });
        
        // Start monitoring the processing status
        checkFileProcessingStatus(data.id);
        
        setIsUploading(false);
      })
      .catch(error => {
        console.error("Error uploading file:", error);
        setMessages((prev) => [
          ...prev,
          {
            role: "system",
            type: "error",
            content: `Error uploading file: ${error.message}`,
          },
        ]);
        setIsUploading(false);
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
        setSelectedFiles(prev => prev.map(file => {
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
        console.error("Error checking file status:", error);
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
          setSelectedFiles(prev => prev.map(file => {
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
   * Cancels the current streaming response by sending a request to the cancel endpoint
   */
  const handleCancelStream = async () => {
    if (!isStreaming) return;
    
    try {
      const formData = new FormData();
      formData.append("ui_session_id", sessionId);
      
      const response = await fetch(`${API_URL}/cancel`, {
        method: "POST",
        body: formData,
      });
      
      if (!response.ok) {
        console.error(`Error cancelling stream: ${response.status}`);
      } else {
        console.log("Stream cancelled successfully");
        // We'll let the stream processing handle setting isStreaming to false
        // as the stream will close naturally after cancellation
      }
    } catch (error) {
      console.error("Error cancelling stream:", error);
    }
  };
  
  /**
   * Sends a message to the chat backend and processes the response stream
   * @returns {Promise<void>}
   */
  const handleSendMessage = async () => {
    // Don't send if empty or already streaming
    if ((!inputText.trim() && selectedFiles.length === 0) || isStreaming) return;
    
    try {
      setIsStreaming(true);
      
      // Add user message to the UI
      setMessages((prev) => [
        ...prev,
        {
          role: "user",
          type: "content",
          content: inputText,
          files: selectedFiles.length > 0 ? selectedFiles.map(f => f.name) : undefined
        },
      ]);
      
      const userText = inputText;
      setInputText("");
      
      // Prepare form data for API request
      const formData = new FormData();
      formData.append("ui_session_id", sessionId);
      formData.append("message", userText);
      formData.append("custom_prompt", customPrompt || "");
      
      if (selectedFiles.length > 0) {
        formData.append("file_ids", JSON.stringify(selectedFiles.map(f => f.id)));
      }
      
      // Add the correct parameter based on model type
      if (modelParameters.temperature !== undefined) {
        formData.append("temperature", modelParameters.temperature);
      }
      if (modelParameters.reasoning_effort !== undefined) {
        formData.append("reasoning_effort", modelParameters.reasoning_effort);
      }
      
      formData.append("llm_model", modelName);
      
      // Make the API request
      const response = await fetch(`${API_URL}/chat`, {
        method: "POST",
        body: formData,
      });
      
      if (!response.ok) {
        throw new Error(`HTTP error! status: ${response.status}`);
      }
      
      if (!response.body) {
        throw new Error("No response body");
      }
      
      // Process the message stream with our utility
      processMessageStream(response.body, {
        onMessage: ({ content, critical }) => {
          setMessages((prev) => [
            ...prev,
            {
              role: "system",
              type: "error",
              content: `Error: ${content}`,
              critical: critical || false
            },
          ]);
          setIsStreaming(false);
        },
        
        onToolSelect: (selectionState) => {
          updateToolSelectionState(selectionState);
        },
        
        onToolCalls: (toolCalls) => {
          // Filter out 'think' tool calls - they should not be displayed
          const displayableToolCalls = toolCalls.filter(tool => 
            tool.name !== 'think' && tool.function?.name !== 'think'
          );
          
          // If all tool calls were 'think' tools, don't display anything
          if (displayableToolCalls.length === 0) return;
          
          const newToolCalls = handleToolStart(displayableToolCalls);
          
          setMessages((prev) => {
            const lastMessage = prev[prev.length - 1];
            if (lastMessage?.type === "tool_calls") {
              // Update existing tool calls message
              return prev.map((msg, i) =>
                i === prev.length - 1
                  ? {...msg, toolCalls: [...(msg.toolCalls || []), ...newToolCalls]}
                  : msg
              );
            }
            // Create new tool calls message
            return [...prev, {
              role: "assistant",
              type: "tool_calls",
              toolCalls: newToolCalls
            }];
          });
        },
        
        onContent: ({ content, vendor }) => {
          setMessages((prev) => {
            const last = prev[prev.length - 1];
            if (last?.role === "assistant" && last?.type === "content") {
              return prev.map((msg, i) =>
                i === prev.length - 1
                  ? {
                    ...msg,
                    content: msg.content + content,
                    vendor: vendor || msg.vendor
                  }
                  : msg
              );
            }
            return [...prev, {
              role: "assistant",
              type: "content",
              content: content,
              vendor: vendor || 'unknown'
            }];
          });
        },
        
        onToolResults: (toolResults) => {
          if (toolResults) {
            toolResults.forEach((result) => {
              const updatedCall = handleToolEnd(result);
              
              if (updatedCall) {
                setMessages((prev) => {
                  return prev.map((message) => {
                    if (message.type === "tool_calls") {
                      const updatedToolCalls = message.toolCalls.map((call) => {
                        if (call.id === updatedCall.id) {
                          return { ...call, results: updatedCall.results };
                        }
                        return call;
                      });
                      return { ...message, toolCalls: updatedToolCalls };
                    }
                    return message;
                  });
                });
              }
            });
          }
        },
        
        onRenderMedia: ({ content, contentType, metadata }) => {
          setMessages((prev) => [
            ...prev,
            {
              role: "assistant",
              type: "media",
              content: content,
              contentType: contentType,
              metadata: metadata
            },
          ]);
        },
        
        onCompletionStatus: ({ running, inputTokens, outputTokens, totalTokens }) => {
          if (!running && (inputTokens || outputTokens)) {
            setMessages((prev) => {
              const lastAssistantMessage = [...prev].reverse().find(
                (msg) => msg.role === "assistant" && msg.type === "content"
              );
              if (!lastAssistantMessage) return prev;
              return prev.map((msg) => {
                if (msg === lastAssistantMessage) {
                  return {
                    ...msg,
                    tokenUsage: {
                      prompt_tokens: inputTokens,
                      completion_tokens: outputTokens,
                      total_tokens: inputTokens + outputTokens,
                    },
                  };
                }
                return msg;
              });
            });
          }
        },
        
        onThoughtDelta: ({ content, vendor }) => {
          setMessages((prev) => {
            const last = prev[prev.length - 1];
            if (last?.role === "assistant" && last?.type === "thinking") {
              return prev.map((msg, i) =>
                i === prev.length - 1
                  ? {
                    ...msg,
                    content: msg.content + content,
                    vendor: vendor || msg.vendor
                  }
                  : msg
              );
            }
            return [...prev, {
              role: "assistant",
              type: "thinking",
              content: content,
              vendor: vendor || 'unknown'
            }];
          });
        },
        
        onUnknownType: (data) => {
          console.warn("Unknown message type:", data.type);
        }
      }).catch(error => {
        console.error("Error processing stream:", error);
        setMessages((prev) => [
          ...prev,
          {
            role: "system",
            type: "error",
            content: `Error: ${error.message}`,
          },
        ]);
      }).finally(() => {
        setIsStreaming(false);
        setSelectedFiles([]);
      });
      
    } catch (error) {
      console.error("Error in chat:", error);
      setMessages((prev) => [
        ...prev,
        {
          role: "system",
          type: "content",
          content: `Error: ${error.message}`,
        },
      ]);
      setIsStreaming(false);
      setSelectedFiles([]);
    }
  };
  
  /**
   * Handles keyboard shortcuts for sending messages
   * @param {KeyboardEvent} e - Keyboard event
   */
  const handleKeyPress = (e) => {
    if (e.key === "Enter" && !e.shiftKey) {
      e.preventDefault();
      handleSendMessage();
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
              modelConfigs={modelConfigs}
              sessionId={sessionId}
              isReady={isReady}
              onEquipTools={onEquipTools}
              activeTools={activeTools}
              modelParameters={modelParameters}
              selectedModel={selectedModel}
              onUpdateSettings={onUpdateSettings}
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
            onFileUploadComplete={handleFileUploaded}
            onFileError={handleFileError}
            onSelectedFilesChange={handleSelectedFilesChange}
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
                      onClick={() => setSelectedFiles(selectedFiles.filter(f => f.id !== file.id))}
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
            handleSendMessage={handleSendMessage}
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
  return (
    <ToolCallProvider>
      <ChatInterfaceInner {...props} />
    </ToolCallProvider>
  );
};

export default ChatInterface;