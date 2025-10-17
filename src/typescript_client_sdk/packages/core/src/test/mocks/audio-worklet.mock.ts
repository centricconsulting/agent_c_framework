/**
 * AudioWorklet API mock for unit testing
 * Simple vi.fn() stubs - configure behavior in individual tests
 */
import { vi } from 'vitest';

// Mock MessagePort
export const mockMessagePort = {
  postMessage: vi.fn(),
  close: vi.fn(),
  onmessage: vi.fn(),
  onmessageerror: vi.fn(),
  addEventListener: vi.fn(),
  removeEventListener: vi.fn(),
  start: vi.fn(),
};

// Mock AudioWorkletNode
export const mockAudioWorkletNode = {
  port: mockMessagePort,
  context: null as BaseAudioContext | null,
  numberOfInputs: 1,
  numberOfOutputs: 1,
  channelCount: 2,
  channelCountMode: 'max' as ChannelCountMode,
  channelInterpretation: 'speakers' as ChannelInterpretation,
  
  // AudioNode methods
  connect: vi.fn(),
  disconnect: vi.fn(),
  
  // Event handlers
  onprocessorerror: vi.fn(),
};

export const MockAudioWorkletNodeConstructor = vi.fn(() => mockAudioWorkletNode);

/**
 * Helper to reset AudioWorkletNode mock state
 */
export const resetAudioWorkletMock = () => {
  mockMessagePort.postMessage.mockReset();
  mockMessagePort.close.mockReset();
  mockMessagePort.onmessage.mockReset();
  mockMessagePort.onmessageerror.mockReset();
  mockMessagePort.addEventListener.mockReset();
  mockMessagePort.removeEventListener.mockReset();
  mockMessagePort.start.mockReset();
  
  mockAudioWorkletNode.connect.mockReset();
  mockAudioWorkletNode.disconnect.mockReset();
  mockAudioWorkletNode.onprocessorerror.mockReset();
  
  MockAudioWorkletNodeConstructor.mockClear();
};