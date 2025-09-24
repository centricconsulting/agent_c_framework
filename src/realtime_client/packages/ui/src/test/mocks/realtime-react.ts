/**
 * Mock for @agentc/realtime-react package
 * Provides consistent mock implementations for all hooks used in UI components
 */

import { vi } from 'vitest';

// Default mock states
export const defaultMockStates = {
  audio: {
    isRecording: false,
    audioLevel: 0,
    isMuted: false,
    startRecording: vi.fn(),
    stopRecording: vi.fn(),
    toggleMute: vi.fn(),
    setMuted: vi.fn(),
  },
  errors: {
    errors: [],
    addError: vi.fn(),
    dismissError: vi.fn(),
    clearErrors: vi.fn(),
  },
  connection: {
    isConnected: false,
    connectionState: 'disconnected' as const,
    connect: vi.fn(),
    disconnect: vi.fn(),
    error: null,
    stats: {
      latency: 0,
      messagesReceived: 0,
      messagesSent: 0,
      reconnectAttempts: 0,
      bytesReceived: 0,
      bytesSent: 0,
    },
  },
  chat: {
    messages: [],
    sendMessage: vi.fn(),
    isLoading: false,
    error: null,
    clearMessages: vi.fn(),
    updateMessage: vi.fn(),
    deleteMessage: vi.fn(),
    isSubSessionMessage: vi.fn(() => false),
    isAgentTyping: false,
    streamingMessage: null,
  },
  turnState: {
    currentState: 'idle' as const,
    isUserTurn: false,
    isAgentTurn: false,
    canInterrupt: true,
    turnCount: 0,
    lastTurnTimestamp: null,
  },
  voiceModel: {
    selectedVoice: null,
    availableVoices: [],
    setVoice: vi.fn(),
    isLoading: false,
  },
  outputMode: {
    mode: 'voice' as const,
    setMode: vi.fn(),
    isTransitioning: false,
  },
  avatar: {
    isEnabled: false,
    avatarUrl: null,
    setEnabled: vi.fn(),
    error: null,
    isLoading: false,
  },
  userData: {
    user: null,
    isLoading: false,
    error: null,
  },
  agentCData: {
    agents: [],
    voices: [],
    avatars: [],
    tools: [],
    selectedAgent: null,
    selectAgent: vi.fn(),
    isLoading: false,
    error: null,
  },
  initializationStatus: {
    isInitialized: false,
    isConnecting: false,
    connectionState: 'disconnected' as const,
    error: null,
    retryCount: 0,
  },
  chatSessionList: {
    sessions: [],
    groupedSessions: {},
    isLoading: false,
    error: null,
    refreshSessions: vi.fn(),
    deleteSession: vi.fn(),
    createSession: vi.fn(),
    selectSession: vi.fn(),
    selectedSessionId: null,
  },
  toolNotifications: {
    notifications: [],
    completedToolCalls: [],
    getNotification: vi.fn(),
    clearNotifications: vi.fn(),
    hasActiveTools: false,
    isToolActive: vi.fn(() => false),
    activeToolCount: 0,
  },
};

// Hook mocks
export const useAudio = vi.fn(() => defaultMockStates.audio);
export const useErrors = vi.fn(() => defaultMockStates.errors);
export const useConnection = vi.fn(() => defaultMockStates.connection);
export const useChat = vi.fn(() => defaultMockStates.chat);
export const useTurnState = vi.fn(() => defaultMockStates.turnState);
export const useVoiceModel = vi.fn(() => defaultMockStates.voiceModel);
export const useOutputMode = vi.fn(() => defaultMockStates.outputMode);
export const useAvatar = vi.fn(() => defaultMockStates.avatar);
export const useUserData = vi.fn(() => defaultMockStates.userData);
export const useAgentCData = vi.fn(() => defaultMockStates.agentCData);
export const useInitializationStatus = vi.fn(() => defaultMockStates.initializationStatus);
export const useChatSessionList = vi.fn(() => defaultMockStates.chatSessionList);
export const useToolNotifications = vi.fn(() => defaultMockStates.toolNotifications);

// Mock for useRealtimeClient and useRealtimeClientSafe
export const useRealtimeClient = vi.fn(() => null);
export const useRealtimeClientSafe = vi.fn(() => null);

// Utility function mocks
export const getConnectionStateString = vi.fn((state: string) => state);

// Provider mock
export const AgentCProvider = vi.fn(({ children }) => children);

// Type exports to match the real package
export type ConnectionState = 'disconnected' | 'connecting' | 'connected' | 'error';
export type OutputMode = 'voice' | 'text' | 'both';
export type TurnState = 'idle' | 'user_speaking' | 'agent_speaking' | 'processing';

// ChatItem type exports
export type ChatItemType = 'message' | 'divider' | 'media' | 'system_alert';

export interface ChatItem {
  id: string;
  type: ChatItemType;
  timestamp?: string;
}

export interface MessageChatItem extends ChatItem {
  type: 'message';
  role: string;
  content: any;
  isSubSession?: boolean;
}

export interface DividerChatItem extends ChatItem {
  type: 'divider';
  dividerType: 'start' | 'end';
  metadata?: any;
}

export interface MediaChatItem extends ChatItem {
  type: 'media';
  content: string;
  contentType: string;
  metadata?: any;
}

export interface SystemAlertChatItem extends ChatItem {
  type: 'system_alert';
  content: string;
  severity: 'info' | 'warning' | 'error';
  format: 'markdown' | 'text';
}

// Type guards
export const isMessageItem = vi.fn((item: ChatItem): item is MessageChatItem => item.type === 'message');
export const isDividerItem = vi.fn((item: ChatItem): item is DividerChatItem => item.type === 'divider');
export const isMediaItem = vi.fn((item: ChatItem): item is MediaChatItem => item.type === 'media');
export const isSystemAlertItem = vi.fn((item: ChatItem): item is SystemAlertChatItem => item.type === 'system_alert');

// Helper function to update mock states
export function updateMockState(hook: string, newState: any) {
  const hookMap = {
    audio: useAudio,
    connection: useConnection,
    chat: useChat,
    turnState: useTurnState,
    voiceModel: useVoiceModel,
    outputMode: useOutputMode,
    avatar: useAvatar,
    userData: useUserData,
    agentCData: useAgentCData,
    initializationStatus: useInitializationStatus,
    chatSessionList: useChatSessionList,
    toolNotifications: useToolNotifications,
    errors: useErrors,
  };
  
  const mockHook = hookMap[hook as keyof typeof hookMap];
  if (mockHook) {
    mockHook.mockReturnValue({
      ...defaultMockStates[hook as keyof typeof defaultMockStates],
      ...newState,
    });
  }
}

// Helper function to reset all mocks to default state
export function resetAllMocks() {
  useAudio.mockReturnValue(defaultMockStates.audio);
  useConnection.mockReturnValue(defaultMockStates.connection);
  useChat.mockReturnValue(defaultMockStates.chat);
  useTurnState.mockReturnValue(defaultMockStates.turnState);
  useVoiceModel.mockReturnValue(defaultMockStates.voiceModel);
  useOutputMode.mockReturnValue(defaultMockStates.outputMode);
  useAvatar.mockReturnValue(defaultMockStates.avatar);
  useUserData.mockReturnValue(defaultMockStates.userData);
  useAgentCData.mockReturnValue(defaultMockStates.agentCData);
  useInitializationStatus.mockReturnValue(defaultMockStates.initializationStatus);
  useChatSessionList.mockReturnValue(defaultMockStates.chatSessionList);
  useToolNotifications.mockReturnValue(defaultMockStates.toolNotifications);
  useErrors.mockReturnValue(defaultMockStates.errors);
}