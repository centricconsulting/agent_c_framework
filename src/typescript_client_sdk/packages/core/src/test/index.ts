/**
 * Test Utilities Export
 * Central export for all test utilities, mocks, and factories
 */

// Export new mocks
export * from './mocks';

// TODO: Legacy mock exports - to be migrated
// The following exports are commented out as the files don't exist yet
// export {
//   MockWebSocket,
//   mockWebSocketConstructor,
//   WebSocketMock,
//   createMockWebSocket,
//   setupWebSocketMock
// } from './mocks/mock-websocket';

// export {
//   MockAudioContext,
//   MockAudioBuffer,
//   MockMediaStream,
//   MockMediaStreamTrack,
//   MockMediaRecorder,
//   mockGetUserMedia,
//   setupAudioMocks
// } from './mocks/audio-mocks';

// Test Helpers
export {
  sleep,
  waitFor,
  nextTick,
  createMockAgentCEvent,
  createMockAudioBuffer,
  createDeferred,
  MockTimers,
  spyOnConsole,
  createMockEventEmitter
} from './utils/test-helpers';

// Realtime Test Utils
export {
  defaultTestConfig,
  createMockRealtimeClient,
  waitForSocketConnection,
  simulateTurnSequence,
  TestResponseBuilder,
  createMockChatSessionManager,
  createMockTurnManager,
  createMockAuthManager,
  RealtimeTestHarness
} from './utils/realtime-test-utils';

// Message Factories
export {
  ChatMessageFactory,
  SystemEventFactory,
  ErrorResponseFactory,
  TurnEventFactory,
  ToolCallFactory,
  ToolResponseFactory,
  AudioEventFactory,
  ConnectionEventFactory
} from './factories/message-factory';

// Re-export message factory types
export type {
  ChatMessage,
  SystemEvent,
  ErrorResponse,
  TurnEvent,
  ToolCall,
  ToolResponse,
  AudioEvent,
  ConnectionEvent
} from './factories/message-factory';

// MSW utilities
export {
  DELAYS,
  STATUS,
  successResponse,
  errorResponse,
  networkErrorHandler,
  timeoutHandler,
  paginatedResponse,
  authenticatedHandler,
  createCRUDHandlers,
  RequestTracker,
  createFileHandlers,
  createHandlersFromConfig
} from './utils/msw-utils';

// MSW handlers and server
// TODO: MSW exports - to be implemented
// export { handlers, authHandlers, sessionHandlers, userHandlers, errorHandlers, scenarioHandlers } from './mocks/handlers';
// export { server, serverLifecycle, useHandlers, replaceAllHandlers, waitForRequest, expectRequest } from './mocks/server';