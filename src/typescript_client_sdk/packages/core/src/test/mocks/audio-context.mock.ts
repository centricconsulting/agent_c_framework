/**
 * AudioContext API mock for unit testing
 * Simple vi.fn() stubs - configure behavior in individual tests
 */
import { vi } from 'vitest';

// Mock AudioNode base
export const createMockAudioNode = () => ({
  connect: vi.fn(),
  disconnect: vi.fn(),
});

// Mock GainNode
export const mockGainNode = {
  ...createMockAudioNode(),
  gain: {
    value: 1,
  },
};

// Mock AudioBufferSourceNode
export const mockAudioBufferSourceNode = {
  ...createMockAudioNode(),
  buffer: null as AudioBuffer | null,
  start: vi.fn(),
  stop: vi.fn(),
  onended: vi.fn(),
};

// Mock MediaStreamAudioSourceNode
export const mockMediaStreamAudioSourceNode = {
  ...createMockAudioNode(),
};

// Mock AudioBuffer
export const mockAudioBuffer = {
  duration: 1,
  length: 44100,
  numberOfChannels: 2,
  sampleRate: 44100,
  getChannelData: vi.fn(() => new Float32Array(44100)),
  copyFromChannel: vi.fn(),
  copyToChannel: vi.fn(),
};

// Mock AudioDestinationNode
export const mockAudioDestinationNode = {
  ...createMockAudioNode(),
  maxChannelCount: 2,
};

// Mock AudioContext
export const mockAudioContext = {
  state: 'running' as AudioContextState,
  sampleRate: 44100,
  currentTime: 0,
  destination: mockAudioDestinationNode,
  audioWorklet: {
    addModule: vi.fn(() => Promise.resolve()),
  },
  
  // Methods
  createGain: vi.fn(() => ({ ...mockGainNode })),
  createBuffer: vi.fn(() => ({ ...mockAudioBuffer })),
  createBufferSource: vi.fn(() => ({ ...mockAudioBufferSourceNode })),
  createMediaStreamSource: vi.fn(() => ({ ...mockMediaStreamAudioSourceNode })),
  resume: vi.fn(() => Promise.resolve()),
  close: vi.fn(() => Promise.resolve()),
  suspend: vi.fn(() => Promise.resolve()),
  
  // Event handlers
  onstatechange: vi.fn(),
};

export const MockAudioContextConstructor = vi.fn(() => mockAudioContext);

/**
 * Helper to reset AudioContext mock state
 */
export const resetAudioContextMock = () => {
  mockAudioContext.state = 'running';
  mockAudioContext.currentTime = 0;
  mockAudioContext.createGain.mockReset();
  mockAudioContext.createBuffer.mockReset();
  mockAudioContext.createBufferSource.mockReset();
  mockAudioContext.createMediaStreamSource.mockReset();
  mockAudioContext.resume.mockReset();
  mockAudioContext.close.mockReset();
  mockAudioContext.suspend.mockReset();
  mockAudioContext.audioWorklet.addModule.mockReset();
  mockAudioContext.onstatechange.mockReset();
  MockAudioContextConstructor.mockClear();
  
  // Reset nodes
  mockGainNode.connect.mockReset();
  mockGainNode.disconnect.mockReset();
  mockAudioBufferSourceNode.connect.mockReset();
  mockAudioBufferSourceNode.disconnect.mockReset();
  mockAudioBufferSourceNode.start.mockReset();
  mockAudioBufferSourceNode.stop.mockReset();
  mockMediaStreamAudioSourceNode.connect.mockReset();
  mockMediaStreamAudioSourceNode.disconnect.mockReset();
};