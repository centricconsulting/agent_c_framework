/**
 * Central export for all test mocks
 * Import from here in your tests for consistent mocking
 */

// MSW Server setup
export {
  server,
  startMockServer,
  resetMockServer,
  stopMockServer,
  addTestHandler
} from './server';

// WebSocket mocks
export {
  mockWebSocket,
  MockWebSocketConstructor,
  resetWebSocketMock,
  WebSocketTracker,
  MockWebSocket
} from './websocket.mock';

// AudioContext mocks
export {
  createMockAudioNode,
  mockGainNode,
  mockAudioBufferSourceNode,
  mockMediaStreamAudioSourceNode,
  mockAudioBuffer,
  mockAudioDestinationNode,
  mockAudioContext,
  MockAudioContextConstructor,
  resetAudioContextMock,
} from './audio-context.mock';

// AudioWorklet mocks
export {
  mockMessagePort,
  mockAudioWorkletNode,
  MockAudioWorkletNodeConstructor,
  resetAudioWorkletMock,
} from './audio-worklet.mock';

// MediaStream mocks
export {
  mockMediaStreamTrack,
  mockMediaStream,
  mockMediaDevices,
  mockNavigator,
  resetMediaStreamMock,
} from './media-stream.mock';

// Browser API mocks
export {
  mockAbortSignal,
  mockAbortController,
  MockAbortControllerConstructor,
  mockURLSearchParams,
  mockURL,
  MockURLConstructor,
  mockBlob,
  MockBlobConstructor,
  mockEvent,
  MockEventConstructor,
  mockCloseEvent,
  MockCloseEventConstructor,
  mockMessageEvent,
  MockMessageEventConstructor,
  resetBrowserAPIMocks,
} from './browser-apis.mock';

/**
 * Helper to reset all mocks at once
 * Useful in afterEach hooks
 */
import { resetWebSocketMock } from './websocket.mock';
import { resetAudioContextMock } from './audio-context.mock';
import { resetAudioWorkletMock } from './audio-worklet.mock';
import { resetMediaStreamMock } from './media-stream.mock';
import { resetBrowserAPIMocks } from './browser-apis.mock';

export const resetAllMocks = () => {
  resetWebSocketMock();
  resetAudioContextMock();
  resetAudioWorkletMock();
  resetMediaStreamMock();
  resetBrowserAPIMocks();
};