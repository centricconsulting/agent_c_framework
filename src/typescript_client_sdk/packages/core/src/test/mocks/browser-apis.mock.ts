/**
 * Browser APIs mock for unit testing
 * Simple vi.fn() stubs - configure behavior in individual tests
 */
import { vi } from 'vitest';

// Mock AbortController
export const mockAbortSignal = {
  aborted: false,
  reason: undefined as unknown,
  onabort: vi.fn(),
  addEventListener: vi.fn(),
  removeEventListener: vi.fn(),
  dispatchEvent: vi.fn(),
  throwIfAborted: vi.fn(),
};

export const mockAbortController = {
  signal: mockAbortSignal,
  abort: vi.fn((reason?: unknown) => {
    mockAbortSignal.aborted = true;
    mockAbortSignal.reason = reason;
  }),
};

export const MockAbortControllerConstructor = vi.fn(() => mockAbortController);

// Mock URL
export const mockURLSearchParams = {
  append: vi.fn(),
  delete: vi.fn(),
  get: vi.fn(),
  getAll: vi.fn(),
  has: vi.fn(),
  set: vi.fn(),
  sort: vi.fn(),
  toString: vi.fn(() => ''),
  forEach: vi.fn(),
  entries: vi.fn(),
  keys: vi.fn(),
  values: vi.fn(),
};

export const mockURL = {
  href: '',
  origin: '',
  protocol: '',
  username: '',
  password: '',
  host: '',
  hostname: '',
  port: '',
  pathname: '',
  search: '',
  searchParams: mockURLSearchParams,
  hash: '',
  toString: vi.fn(() => ''),
  toJSON: vi.fn(() => ''),
};

export const MockURLConstructor = vi.fn((url: string, _base?: string | URL) => {
  mockURL.href = url;
  mockURL.toString.mockReturnValue(url);
  return mockURL;
});

// Mock Blob
export const mockBlob = {
  size: 0,
  type: '',
  arrayBuffer: vi.fn(() => Promise.resolve(new ArrayBuffer(0))),
  text: vi.fn(() => Promise.resolve('')),
  stream: vi.fn(),
  slice: vi.fn(() => mockBlob),
};

export const MockBlobConstructor = vi.fn((parts?: BlobPart[], _options?: BlobPropertyBag) => {
  if (parts && parts.length > 0) {
    // Simple size calculation for testing
    mockBlob.size = parts.reduce((acc, part) => {
      if (typeof part === 'string') return acc + part.length;
      if (part instanceof ArrayBuffer) return acc + part.byteLength;
      return acc;
    }, 0);
  }
  return mockBlob;
});

// Mock Event
export const mockEvent = {
  type: '',
  target: null,
  currentTarget: null,
  bubbles: false,
  cancelable: false,
  defaultPrevented: false,
  composed: false,
  isTrusted: false,
  timeStamp: 0,
  preventDefault: vi.fn(),
  stopPropagation: vi.fn(),
  stopImmediatePropagation: vi.fn(),
};

export const MockEventConstructor = vi.fn((type: string, _init?: EventInit) => {
  mockEvent.type = type;
  return mockEvent;
});

// Mock CloseEvent
export const mockCloseEvent = {
  ...mockEvent,
  code: 1000,
  reason: '',
  wasClean: true,
};

export const MockCloseEventConstructor = vi.fn((type: string, init?: CloseEventInit) => {
  mockCloseEvent.type = type;
  if (init) {
    mockCloseEvent.code = init.code ?? 1000;
    mockCloseEvent.reason = init.reason ?? '';
    mockCloseEvent.wasClean = init.wasClean ?? true;
  }
  return mockCloseEvent;
});

// Mock MessageEvent
export const mockMessageEvent = {
  ...mockEvent,
  data: null as unknown,
  origin: '',
  lastEventId: '',
  source: null,
  ports: [] as MessagePort[],
};

export const MockMessageEventConstructor = vi.fn((type: string, init?: MessageEventInit) => {
  mockMessageEvent.type = type;
  if (init) {
    mockMessageEvent.data = init.data;
    mockMessageEvent.origin = init.origin ?? '';
    mockMessageEvent.lastEventId = init.lastEventId ?? '';
  }
  return mockMessageEvent;
});

/**
 * Helper to reset all browser API mocks
 */
export const resetBrowserAPIMocks = () => {
  // Reset AbortController
  mockAbortSignal.aborted = false;
  mockAbortSignal.reason = undefined;
  mockAbortSignal.onabort.mockReset();
  mockAbortSignal.addEventListener.mockReset();
  mockAbortSignal.removeEventListener.mockReset();
  mockAbortSignal.dispatchEvent.mockReset();
  mockAbortSignal.throwIfAborted.mockReset();
  mockAbortController.abort.mockReset();
  MockAbortControllerConstructor.mockClear();
  
  // Reset URL
  mockURL.href = '';
  mockURL.toString.mockReset();
  mockURL.toJSON.mockReset();
  mockURLSearchParams.set.mockReset();
  mockURLSearchParams.toString.mockReset();
  MockURLConstructor.mockClear();
  
  // Reset Blob
  mockBlob.size = 0;
  mockBlob.arrayBuffer.mockReset();
  mockBlob.text.mockReset();
  mockBlob.stream.mockReset();
  mockBlob.slice.mockReset();
  MockBlobConstructor.mockClear();
  
  // Reset Events
  mockEvent.preventDefault.mockReset();
  mockEvent.stopPropagation.mockReset();
  mockEvent.stopImmediatePropagation.mockReset();
  MockEventConstructor.mockClear();
  MockCloseEventConstructor.mockClear();
  MockMessageEventConstructor.mockClear();
};