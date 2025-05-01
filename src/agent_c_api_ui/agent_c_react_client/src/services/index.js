/**
 * API Services Index
 * 
 * This file exports all API services for easy import throughout the application.
 */

import apiService from './api';
import sessionService from './session-api';
import modelService from './model-api';
import toolsService from './tools-api';
import personaService from './persona-api';
import chatService from './chat-api';

// Export individual services
export const api = apiService;
export const session = sessionService;
export const model = modelService;
export const tools = toolsService;
export const persona = personaService;
export const chat = chatService;

// Export individual functions from each service
export {
  // Core API utilities
  processApiError,
  showErrorToast,
  API_CONFIG,
} from './api';

// Export all service methods from session-api
export {
  initialize,
  createSession,
  getSession,
  updateSession,
  deleteSession,
  getSessionTemplates,
  sendMessage,
  getMessages,
  getSessionUIState,
  updateSessionUIState,
  stopGeneration,
  exportSession,
} from './session-api';

// Export all service methods from model-api
export {
  getModels,
  getModelDetails,
  getModelParameters,
  setModel,
  updateParameters,
  getDefaultParameters,
} from './model-api';

// Export all service methods from tools-api
export {
  getTools,
  getToolDetails,
  executeTool,
  getSessionTools,
  updateSessionTools,
  getToolCategories,
} from './tools-api';

// Export all service methods from persona-api
export {
  getPersonas,
  getPersonaDetails,
  setSessionPersona,
  getSessionPersona,
  getPersonaCategories,
} from './persona-api';

// Export all service methods from chat-api
export {
  sendStreamingMessage,
  uploadAttachment,
  downloadAttachment,
  deleteMessage,
  editMessage,
  regenerateResponse,
} from './chat-api';

// Default export combining all services
export default {
  api: apiService,
  session: sessionService,
  model: modelService,
  tools: toolsService,
  persona: personaService,
  chat: chatService,
};