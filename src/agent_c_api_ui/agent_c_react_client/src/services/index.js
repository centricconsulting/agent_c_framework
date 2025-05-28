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
import configService from './config-api';
import historyService from './history-api';
import authService from './auth-api';

// Export individual services
export const api = apiService;
export const session = sessionService;
export const model = modelService;
export const tools = toolsService;
export const persona = personaService;
export const chat = chatService;
export const config = configService;
export const history = historyService;
export const auth = authService;

// Export individual functions from each service
export {
  // Core API utilities
  processApiError,
  showErrorToast,
  API_CONFIG,
} from './api';

// Authentication service functions
export {
  validateToken,
  getUserInfo,
  addAuthHeader,
} from './auth-api';

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
  setSessionModel,
  updateModelParameters,
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

// Export all service methods from config-api
export {
  getSystemConfig,
  getModelDetails as getConfigModelDetails,
  getPersonaDetails as getConfigPersonaDetails,
  getToolDetails as getConfigToolDetails,
  getModelParameters as getConfigModelParameters,
} from './config-api';

// Export all service methods from history-api
export {
  listHistories,
  getHistoryDetails,
  deleteHistory,
  getEvents,
  streamEvents,
  getReplayStatus,
  controlReplay,
} from './history-api';

// Default export combining all services
export default {
  api: apiService,
  session: sessionService,
  model: modelService,
  tools: toolsService,
  persona: personaService,
  chat: chatService,
  config: configService,
  history: historyService,
  auth: authService,
};