/**
 * Events - Standardized event types for the event bus
 * 
 * This module defines standard event types for use with the eventBus.
 * Events are organized by category to make them easier to find and use.
 */

// Auth Events
export const AUTH_EVENTS = {
  SESSION_CREATED: 'auth.session.created',
  SESSION_UPDATED: 'auth.session.updated',
  SESSION_DELETED: 'auth.session.deleted',
  AUTH_ERROR: 'auth.error',
  SESSION_EXPIRED: 'auth.session.expired',
  AUTH_STATUS_CHANGED: 'auth.status.changed'
};

// Model Events
export const MODEL_EVENTS = {
  MODEL_CHANGED: 'model.changed',
  MODEL_PARAMETERS_UPDATED: 'model.parameters.updated',
  MODEL_ERROR: 'model.error',
  MODELS_LOADED: 'model.loaded',
  MODEL_CONFIG_SAVED: 'model.config.saved'
};

// Session Events
export const SESSION_EVENTS = {
  AGENT_INITIALIZED: 'session.agent.initialized',
  TOOLS_UPDATED: 'session.tools.updated',
  SETTINGS_UPDATED: 'session.settings.updated',
  SESSION_ERROR: 'session.error',
  MESSAGE_SENT: 'session.message.sent',
  MESSAGE_RECEIVED: 'session.message.received',
  STREAMING_STARTED: 'session.streaming.started',
  STREAMING_PROGRESS: 'session.streaming.progress',
  STREAMING_ENDED: 'session.streaming.ended',
  CONVERSATION_CLEARED: 'session.conversation.cleared',
  CONVERSATION_EXPORTED: 'session.conversation.exported'
};

// Initialization Events
export const INIT_EVENTS = {
  INITIALIZATION_STARTED: 'init.started',
  AUTH_PHASE_STARTED: 'init.auth.started',
  AUTH_PHASE_COMPLETED: 'init.auth.completed',
  AUTH_PHASE_ERROR: 'init.auth.error',
  MODEL_PHASE_STARTED: 'init.model.started',
  MODEL_PHASE_COMPLETED: 'init.model.completed',
  MODEL_PHASE_ERROR: 'init.model.error',
  SESSION_PHASE_STARTED: 'init.session.started',
  SESSION_PHASE_COMPLETED: 'init.session.completed',
  SESSION_PHASE_ERROR: 'init.session.error',
  INITIALIZATION_COMPLETED: 'init.completed',
  INITIALIZATION_ERROR: 'init.error'
};

// Storage Events
export const STORAGE_EVENTS = {
  SESSION_DATA_UPDATED: 'storage.session.updated',
  MODEL_DATA_UPDATED: 'storage.model.updated',
  USER_SETTINGS_UPDATED: 'storage.settings.updated',
  STORAGE_ERROR: 'storage.error',
  STORAGE_CLEARED: 'storage.cleared'
};

// API Events
export const API_EVENTS = {
  REQUEST_STARTED: 'api.request.started',
  REQUEST_COMPLETED: 'api.request.completed',
  REQUEST_ERROR: 'api.request.error',
  NETWORK_STATUS_CHANGED: 'api.network.statusChanged',
  SERVICE_STATUS_CHANGED: 'api.service.statusChanged',
  RETRY_ATTEMPT: 'api.retry.attempt'
};

// UI Events
export const UI_EVENTS = {
  THEME_CHANGED: 'ui.theme.changed',
  LAYOUT_CHANGED: 'ui.layout.changed',
  SIDEBAR_TOGGLED: 'ui.sidebar.toggled',
  ERROR_DISPLAYED: 'ui.error.displayed',
  MODAL_OPENED: 'ui.modal.opened',
  MODAL_CLOSED: 'ui.modal.closed'
};

// Export all event categories
export const EVENTS = {
  ...AUTH_EVENTS,
  ...MODEL_EVENTS,
  ...SESSION_EVENTS,
  ...INIT_EVENTS,
  ...STORAGE_EVENTS,
  ...API_EVENTS,
  ...UI_EVENTS
};

export default EVENTS;