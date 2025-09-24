/**
 * Core React hooks for Agent C Realtime SDK
 * Export all hooks from this central location
 */

// Audio control
export { useAudio } from './useAudio';
export type { UseAudioOptions, UseAudioReturn } from './useAudio';

// Turn state management
export { useTurnState } from './useTurnState';
export type { UseTurnStateOptions, UseTurnStateReturn, TurnStateEvent } from './useTurnState';

// Voice model management
export { useVoiceModel } from './useVoiceModel';
export type { UseVoiceModelReturn } from './useVoiceModel';

// Output mode coordination
export { useOutputMode } from './useOutputMode';
export type { UseOutputModeOptions, UseOutputModeReturn, OutputMode } from './useOutputMode';

// Chat functionality
export { useChat } from './useChat';
export type { UseChatOptions, UseChatReturn } from './useChat';

// Connection management
export { useConnection, getConnectionStateString } from './useConnection';
export type { UseConnectionReturn, ConnectionStats, ConnectionStateString } from './useConnection';

// Avatar management
export { useAvatar } from './useAvatar';
export type { UseAvatarReturn } from './useAvatar';

// User data from WebSocket
export { useUserData } from './useUserData';
export type { UseUserDataReturn, User } from './useUserData';

// All initialization data from WebSocket
export { useAgentCData } from './useAgentCData';
export type { UseAgentCDataReturn, AgentCData, Agent, Voice, Avatar, Tool } from './useAgentCData';

// Initialization status tracking
export { useInitializationStatus, ConnectionState } from './useInitializationStatus';
export type { UseInitializationStatusReturn } from './useInitializationStatus';

// Chat session list management
export { useChatSessionList } from './useChatSessionList';
export type { 
  UseChatSessionListOptions, 
  UseChatSessionListReturn,
  GroupedSessions,
  SessionGroup,
  SessionGroupMeta 
} from './useChatSessionList';

// Tool notifications
export { useToolNotifications } from './useToolNotifications';
export type {
  UseToolNotificationsOptions,
  UseToolNotificationsReturn,
  ToolNotification,
  ToolCallResult
} from './useToolNotifications';

// Error handling
export { useErrors } from './useErrors';
export type { UseErrorsReturn } from './useErrors';