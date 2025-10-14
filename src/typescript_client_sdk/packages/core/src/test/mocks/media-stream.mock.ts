/**
 * MediaStream API mock for unit testing
 * Simple vi.fn() stubs - configure behavior in individual tests
 */
import { vi } from 'vitest';

// Mock MediaStreamTrack
export const mockMediaStreamTrack = {
  id: 'mock-track-id',
  kind: 'audio' as 'audio' | 'video',
  label: 'Mock Audio Track',
  enabled: true,
  muted: false,
  readyState: 'live' as MediaStreamTrackState,
  
  // Methods
  stop: vi.fn(),
  getSettings: vi.fn(() => ({
    sampleRate: 44100,
    channelCount: 1,
    echoCancellation: true,
    noiseSuppression: true,
  })),
  getCapabilities: vi.fn(() => ({})),
  getConstraints: vi.fn(() => ({})),
  applyConstraints: vi.fn(() => Promise.resolve()),
  clone: vi.fn(),
  
  // Event handlers
  onended: vi.fn(),
  onmute: vi.fn(),
  onunmute: vi.fn(),
  addEventListener: vi.fn(),
  removeEventListener: vi.fn(),
};

// Mock MediaStream
export const mockMediaStream = {
  id: 'mock-stream-id',
  active: true,
  
  // Methods
  getTracks: vi.fn(() => [mockMediaStreamTrack]),
  getAudioTracks: vi.fn(() => [mockMediaStreamTrack]),
  getVideoTracks: vi.fn(() => []),
  getTrackById: vi.fn(),
  addTrack: vi.fn(),
  removeTrack: vi.fn(),
  clone: vi.fn(),
  
  // Event handlers
  onaddtrack: vi.fn(),
  onremovetrack: vi.fn(),
  onactive: vi.fn(),
  oninactive: vi.fn(),
  addEventListener: vi.fn(),
  removeEventListener: vi.fn(),
};

// Mock navigator.mediaDevices
export const mockMediaDevices = {
  getUserMedia: vi.fn(() => Promise.resolve(mockMediaStream)),
  getDisplayMedia: vi.fn(() => Promise.resolve(mockMediaStream)),
  enumerateDevices: vi.fn(() => Promise.resolve([])),
  getSupportedConstraints: vi.fn(() => ({})),
  
  // Event handlers
  ondevicechange: vi.fn(),
  addEventListener: vi.fn(),
  removeEventListener: vi.fn(),
};

// Mock navigator
export const mockNavigator = {
  mediaDevices: mockMediaDevices,
};

/**
 * Helper to reset MediaStream mock state
 */
export const resetMediaStreamMock = () => {
  // Reset MediaStreamTrack
  mockMediaStreamTrack.stop.mockReset();
  mockMediaStreamTrack.getSettings.mockReset();
  mockMediaStreamTrack.getCapabilities.mockReset();
  mockMediaStreamTrack.getConstraints.mockReset();
  mockMediaStreamTrack.applyConstraints.mockReset();
  mockMediaStreamTrack.clone.mockReset();
  mockMediaStreamTrack.addEventListener.mockReset();
  mockMediaStreamTrack.removeEventListener.mockReset();
  
  // Reset MediaStream
  mockMediaStream.getTracks.mockReset();
  mockMediaStream.getAudioTracks.mockReset();
  mockMediaStream.getVideoTracks.mockReset();
  mockMediaStream.getTrackById.mockReset();
  mockMediaStream.addTrack.mockReset();
  mockMediaStream.removeTrack.mockReset();
  mockMediaStream.clone.mockReset();
  mockMediaStream.addEventListener.mockReset();
  mockMediaStream.removeEventListener.mockReset();
  
  // Reset mediaDevices
  mockMediaDevices.getUserMedia.mockReset();
  mockMediaDevices.getDisplayMedia.mockReset();
  mockMediaDevices.enumerateDevices.mockReset();
  mockMediaDevices.getSupportedConstraints.mockReset();
  mockMediaDevices.addEventListener.mockReset();
  mockMediaDevices.removeEventListener.mockReset();
  
  // Reset default return values
  mockMediaStream.getTracks.mockReturnValue([mockMediaStreamTrack]);
  mockMediaStream.getAudioTracks.mockReturnValue([mockMediaStreamTrack]);
  mockMediaStream.getVideoTracks.mockReturnValue([]);
  mockMediaDevices.getUserMedia.mockResolvedValue(mockMediaStream);
};