import React, { createContext, useContext, useReducer, useCallback } from 'react';
import { API_URL } from '@/config/config';
import logger from '@/lib/logger';
import { createClipboardContent } from '@/components/chat_interface/utils/htmlChatFormatter';
import { processMessageStream } from '@/components/chat_interface/utils/MessageStreamProcessor';
import { useSessionContext } from '@/hooks/use-session-context';
import { useToolCalls } from '@/components/chat_interface/ToolCallContext';

// Initial state for the reducer
const initialState = {
  messages: [],
  isStreaming: false,
  isUploading: false,
  expandedToolCallMessages: [],
  selectedFiles: []
};

// Define action types
const MESSAGE_ACTIONS = {
  SET_MESSAGES: 'SET_MESSAGES',
  ADD_MESSAGE: 'ADD_MESSAGE',
  UPDATE_LAST_MESSAGE: 'UPDATE_LAST_MESSAGE',
  UPDATE_MESSAGE: 'UPDATE_MESSAGE',
  SET_STREAMING: 'SET_STREAMING',
  SET_UPLOADING: 'SET_UPLOADING',
  TOGGLE_TOOL_CALL_EXPANSION: 'TOGGLE_TOOL_CALL_EXPANSION',
  SET_EXPANDED_TOOL_CALLS: 'SET_EXPANDED_TOOL_CALLS',
  SET_SELECTED_FILES: 'SET_SELECTED_FILES',
  ADD_SELECTED_FILE: 'ADD_SELECTED_FILE',
  REMOVE_SELECTED_FILE: 'REMOVE_SELECTED_FILE',
  CLEAR_SELECTED_FILES: 'CLEAR_SELECTED_FILES',
  UPDATE_TOOL_CALL: 'UPDATE_TOOL_CALL'
};

// Reducer function to handle state updates
function messageReducer(state, action) {
  switch (action.type) {
    case MESSAGE_ACTIONS.SET_MESSAGES:
      return { ...state, messages: action.payload };
    case MESSAGE_ACTIONS.ADD_MESSAGE:
      return { ...state, messages: [...state.messages, action.payload] };
    case MESSAGE_ACTIONS.UPDATE_LAST_MESSAGE:
      return {
        ...state,
        messages: state.messages.map((msg, i) =>
          i === state.messages.length - 1 ? { ...msg, ...action.payload } : msg
        )
      };
    case MESSAGE_ACTIONS.UPDATE_MESSAGE:
      return {
        ...state,
        messages: state.messages.map((msg, i) =>
          i === action.payload.index ? { ...msg, ...action.payload.data } : msg
        )
      };
    case MESSAGE_ACTIONS.SET_STREAMING:
      return { ...state, isStreaming: action.payload };
    case MESSAGE_ACTIONS.SET_UPLOADING:
      return { ...state, isUploading: action.payload };
    case MESSAGE_ACTIONS.TOGGLE_TOOL_CALL_EXPANSION:
      return {
        ...state,
        expandedToolCallMessages: state.expandedToolCallMessages.includes(action.payload)
          ? state.expandedToolCallMessages.filter(idx => idx !== action.payload)
          : [...state.expandedToolCallMessages, action.payload]
      };
    case MESSAGE_ACTIONS.SET_EXPANDED_TOOL_CALLS:
      return { ...state, expandedToolCallMessages: action.payload };
    case MESSAGE_ACTIONS.SET_SELECTED_FILES:
      return { ...state, selectedFiles: action.payload };
    case MESSAGE_ACTIONS.ADD_SELECTED_FILE:
      return { ...state, selectedFiles: [...state.selectedFiles, action.payload] };
    case MESSAGE_ACTIONS.REMOVE_SELECTED_FILE:
      return {
        ...state,
        selectedFiles: state.selectedFiles.filter(file => file.id !== action.payload)
      };
    case MESSAGE_ACTIONS.CLEAR_SELECTED_FILES:
      return { ...state, selectedFiles: [] };
    case MESSAGE_ACTIONS.UPDATE_TOOL_CALL:
      return {
        ...state,
        messages: state.messages.map(message => {
          if (message.type === "tool_calls") {
            const updatedToolCalls = message.toolCalls.map(call => {
              if (call.id === action.payload.id) {
                return { ...call, ...action.payload.data };
              }
              return call;
            });
            return { ...message, toolCalls: updatedToolCalls };
          }
          return message;
        })
      };
    default:
      return state;
  }
}

// Create context with default values to prevent null/undefined issues
export const MessageContext = createContext({
  // Default state values
  ...initialState,
  // Default dispatch function to avoid errors
  dispatch: () => {
    console.warn('Message context dispatch called before initialization');
    return Promise.resolve(false);
  },
  // Methods with default implementations
  setMessages: () => console.warn('Message context not initialized: setMessages'),
  addMessage: () => console.warn('Message context not initialized: addMessage'),
  updateLastMessage: () => console.warn('Message context not initialized: updateLastMessage'),
  setIsStreaming: () => console.warn('Message context not initialized: setIsStreaming'),
  setIsUploading: () => console.warn('Message context not initialized: setIsUploading'),
  toggleToolCallExpansion: () => console.warn('Message context not initialized: toggleToolCallExpansion'),
  handleSendMessage: async () => {
    console.warn('Message context not initialized: handleSendMessage');
    return false;
  },
  handleCancelStream: async () => {
    console.warn('Message context not initialized: handleCancelStream');
    return false;
  },
  formatMessageForCopy: () => '',
  formatChatForCopy: () => '',
  getChatCopyContent: () => '',
  getChatCopyHTML: () => '',
  addSelectedFile: () => console.warn('Message context not initialized: addSelectedFile'),
  removeSelectedFile: () => console.warn('Message context not initialized: removeSelectedFile'),
  clearSelectedFiles: () => console.warn('Message context not initialized: clearSelectedFiles'),
  updateToolCall: () => console.warn('Message context not initialized: updateToolCall')
});

/**
 * MessageProvider manages all chat message related state and operations
 */
export const MessageProvider = ({ children }) => {
  // Initialize logger
  logger.info('MessageProvider initializing', 'MessageProvider');
  
  // Get session context and tool calls context
  const { sessionId, handleProcessingStatus } = useSessionContext('MessageProvider');
  const {
    handleToolStart,
    handleToolEnd,
    updateToolSelectionState,
    toolSelectionState
  } = useToolCalls();
  
  // Initialize state with useReducer
  const [state, dispatch] = useReducer(messageReducer, initialState);
  
  // Destructure state for easier access
  const {
    messages,
    isStreaming,
    isUploading,
    expandedToolCallMessages,
    selectedFiles
  } = state;
  
  // Action creator helper functions
  const setMessages = useCallback((messages) => dispatch({
    type: MESSAGE_ACTIONS.SET_MESSAGES,
    payload: messages
  }), []);
  
  const addMessage = useCallback((message) => dispatch({
    type: MESSAGE_ACTIONS.ADD_MESSAGE,
    payload: message
  }), []);
  
  const updateLastMessage = useCallback((data) => dispatch({
    type: MESSAGE_ACTIONS.UPDATE_LAST_MESSAGE,
    payload: data
  }), []);
  
  const updateMessage = useCallback((index, data) => dispatch({
    type: MESSAGE_ACTIONS.UPDATE_MESSAGE,
    payload: { index, data }
  }), []);
  
  const setIsStreaming = useCallback((value) => {
    dispatch({ type: MESSAGE_ACTIONS.SET_STREAMING, payload: value });
    if (handleProcessingStatus) {
      handleProcessingStatus(value);
    }
  }, [handleProcessingStatus]);
  
  const setIsUploading = useCallback((value) => dispatch({
    type: MESSAGE_ACTIONS.SET_UPLOADING,
    payload: value
  }), []);
  
  const toggleToolCallExpansion = useCallback((messageIdx) => dispatch({
    type: MESSAGE_ACTIONS.TOGGLE_TOOL_CALL_EXPANSION,
    payload: messageIdx
  }), []);
  
  const setSelectedFiles = useCallback((files) => dispatch({
    type: MESSAGE_ACTIONS.SET_SELECTED_FILES,
    payload: files
  }), []);
  
  const addSelectedFile = useCallback((file) => dispatch({
    type: MESSAGE_ACTIONS.ADD_SELECTED_FILE,
    payload: file
  }), []);
  
  const removeSelectedFile = useCallback((fileId) => dispatch({
    type: MESSAGE_ACTIONS.REMOVE_SELECTED_FILE,
    payload: fileId
  }), []);
  
  const clearSelectedFiles = useCallback(() => dispatch({
    type: MESSAGE_ACTIONS.CLEAR_SELECTED_FILES
  }), []);
  
  const updateToolCall = useCallback((id, data) => dispatch({
    type: MESSAGE_ACTIONS.UPDATE_TOOL_CALL,
    payload: { id, data }
  }), []);
  
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
   * Cancels the current streaming response by sending a request to the cancel endpoint
   */
  const handleCancelStream = useCallback(async () => {
    if (!isStreaming || !sessionId) return;
    
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
        logger.debug("Stream cancelled successfully", "MessageContext");
        // We'll let the stream processing handle setting isStreaming to false
        // as the stream will close naturally after cancellation
      }
    } catch (error) {
      console.error("Error cancelling stream:", error?.message || error);
    }
  }, [isStreaming, sessionId]);
  
  /**
   * Sends a message to the chat backend and processes the response stream
   * @param {string} inputText - The text input from the user
   * @param {Object} options - Additional options for sending the message
   * @returns {Promise<boolean>} - Whether the message was sent successfully
   */
  const handleSendMessage = useCallback(async (inputText, options = {}) => {
    const { modelName, modelParameters, customPrompt } = options;
    
    // Don't send if empty or already streaming
    if ((!inputText?.trim() && selectedFiles.length === 0) || isStreaming || !sessionId) return false;
    
    try {
      setIsStreaming(true);
      
      // Add user message to the UI
      addMessage({
        role: "user",
        type: "content",
        content: inputText,
        files: selectedFiles.length > 0 ? selectedFiles.map(f => f.name) : undefined
      });
      
      // Prepare form data for API request
      const formData = new FormData();
      formData.append("ui_session_id", sessionId);
      formData.append("message", inputText || "");
      formData.append("custom_prompt", customPrompt || "");
      
      if (selectedFiles.length > 0) {
        formData.append("file_ids", JSON.stringify(selectedFiles.map(f => f.id)));
      }
      
      // Check if modelParameters exists before accessing its properties
      const params = modelParameters || {};
      
      // Add the correct parameter based on model type
      if (params.temperature !== undefined) {
        formData.append("temperature", params.temperature);
      }
      if (params.reasoning_effort !== undefined) {
        formData.append("reasoning_effort", params.reasoning_effort);
      }
      
      formData.append("llm_model", modelName || "");
      
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
          addMessage({
            role: "system",
            type: "error",
            content: `Error: ${content}`,
            critical: critical || false
          });
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
          
          const lastMessage = messages[messages.length - 1];
          if (lastMessage?.type === "tool_calls") {
            // Update existing tool calls message
            updateLastMessage({ 
              toolCalls: [...(lastMessage.toolCalls || []), ...newToolCalls] 
            });
          } else {
            // Create new tool calls message
            addMessage({
              role: "assistant",
              type: "tool_calls",
              toolCalls: newToolCalls
            });
          }
        },
        
        onContent: ({ content, vendor }) => {
          const last = messages[messages.length - 1];
          if (last?.role === "assistant" && last?.type === "content") {
            updateLastMessage({
              content: last.content + content,
              vendor: vendor || last.vendor
            });
          } else {
            addMessage({
              role: "assistant",
              type: "content",
              content: content,
              vendor: vendor || 'unknown'
            });
          }
        },
        
        onToolResults: (toolResults) => {
          if (toolResults) {
            toolResults.forEach((result) => {
              const updatedCall = handleToolEnd(result);
              
              if (updatedCall) {
                updateToolCall(updatedCall.id, { results: updatedCall.results });
              }
            });
          }
        },
        
        onRenderMedia: ({ content, contentType, metadata }) => {
          addMessage({
            role: "assistant",
            type: "media",
            content: content,
            contentType: contentType,
            metadata: metadata
          });
        },
        
        onCompletionStatus: ({ running, inputTokens, outputTokens, totalTokens }) => {
          if (!running && (inputTokens || outputTokens)) {
            // Find the last assistant content message
            const lastAssistantMessageIndex = [...messages].reverse().findIndex(
              (msg) => msg.role === "assistant" && msg.type === "content"
            );
            
            if (lastAssistantMessageIndex !== -1) {
              // Convert from reverse index to regular index
              const actualIndex = messages.length - 1 - lastAssistantMessageIndex;
              updateMessage(actualIndex, {
                tokenUsage: {
                  prompt_tokens: inputTokens,
                  completion_tokens: outputTokens,
                  total_tokens: inputTokens + outputTokens,
                },
              });
            }
          }
        },
        
        onThoughtDelta: ({ content, vendor }) => {
          const last = messages[messages.length - 1];
          if (last?.role === "assistant" && last?.type === "thinking") {
            updateLastMessage({
              content: last.content + content,
              vendor: vendor || last.vendor
            });
          } else {
            addMessage({
              role: "assistant",
              type: "thinking",
              content: content,
              vendor: vendor || 'unknown'
            });
          }
        },
        
        onUnknownType: (data) => {
          console.warn("Unknown message type:", data.type);
        }
      }).catch(error => {
        console.error("Error processing stream:", error?.message || error);
        addMessage({
          role: "system",
          type: "error",
          content: `Error: ${error.message}`,
        });
      }).finally(() => {
        setIsStreaming(false);
        clearSelectedFiles();
      });
      
      return true;
    } catch (error) {
      console.error("Error in chat:", error?.message || error);
      addMessage({
        role: "system",
        type: "content",
        content: `Error: ${error.message}`,
      });
      setIsStreaming(false);
      clearSelectedFiles();
      return false;
    }
  }, [addMessage, clearSelectedFiles, handleToolEnd, handleToolStart, messages, selectedFiles, sessionId, setIsStreaming, updateLastMessage, updateMessage, updateToolCall, updateToolSelectionState]);
  
  // Create a value object with all the state and actions
  const contextValue = {
    // State from reducer
    messages,
    isStreaming,
    isUploading,
    expandedToolCallMessages,
    selectedFiles,
    toolSelectionState,
    
    // Expose dispatch for advanced usage
    dispatch,
    
    // Methods
    setMessages,
    addMessage,
    updateLastMessage,
    updateMessage,
    setIsStreaming,
    setIsUploading,
    toggleToolCallExpansion,
    setSelectedFiles,
    addSelectedFile,
    removeSelectedFile,
    clearSelectedFiles,
    updateToolCall,
    handleSendMessage,
    handleCancelStream,
    formatMessageForCopy,
    formatChatForCopy,
    getChatCopyContent,
    getChatCopyHTML
  };
  
  return (
    <MessageContext.Provider value={contextValue}>
      {children}
    </MessageContext.Provider>
  );
};

/**
 * Custom hook that provides access to the MessageContext
 * @param {string} componentName - Name of the component using this hook (for logging)
 * @returns {Object} The MessageContext value
 */
export const useMessageContext = (componentName = 'unknown') => {
  const context = useContext(MessageContext);
  
  if (!context) {
    const error = 'useMessageContext must be used within a MessageProvider';
    logger.error(error, 'useMessageContext', { componentName });
    throw new Error(error);
  }
  
  return context;
};