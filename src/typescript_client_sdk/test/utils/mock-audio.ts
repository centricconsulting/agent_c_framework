/**
 * Mock Audio API implementations for testing
 */

import { vi } from 'vitest';

export class MockAudioContext {
  sampleRate = 48000;
  currentTime = 0;
  state: AudioContextState = 'running';
  baseLatency = 0.01;
  outputLatency = 0.02;
  destination = {
    maxChannelCount: 2,
    channelCount: 2,
    channelCountMode: 'max' as ChannelCountMode,
    channelInterpretation: 'speakers' as ChannelInterpretation,
    numberOfInputs: 1,
    numberOfOutputs: 0,
    connect: vi.fn(),
    disconnect: vi.fn(),
  };

  createGain = vi.fn(() => ({
    gain: { 
      value: 1,
      setValueAtTime: vi.fn(),
      linearRampToValueAtTime: vi.fn(),
      exponentialRampToValueAtTime: vi.fn(),
      setTargetAtTime: vi.fn(),
      setValueCurveAtTime: vi.fn(),
      cancelScheduledValues: vi.fn(),
      cancelAndHoldAtTime: vi.fn(),
    },
    connect: vi.fn(),
    disconnect: vi.fn(),
    channelCount: 2,
    channelCountMode: 'max' as ChannelCountMode,
    channelInterpretation: 'speakers' as ChannelInterpretation,
    numberOfInputs: 1,
    numberOfOutputs: 1,
  }));

  createBufferSource = vi.fn(() => ({
    buffer: null,
    playbackRate: { value: 1 },
    detune: { value: 0 },
    loop: false,
    loopStart: 0,
    loopEnd: 0,
    connect: vi.fn(),
    disconnect: vi.fn(),
    start: vi.fn(),
    stop: vi.fn(),
    addEventListener: vi.fn(),
    removeEventListener: vi.fn(),
    dispatchEvent: vi.fn(),
  }));

  createMediaStreamSource = vi.fn((stream: MediaStream) => ({
    mediaStream: stream,
    connect: vi.fn(),
    disconnect: vi.fn(),
    channelCount: 2,
    channelCountMode: 'max' as ChannelCountMode,
    channelInterpretation: 'speakers' as ChannelInterpretation,
    numberOfInputs: 0,
    numberOfOutputs: 1,
  }));

  createAnalyser = vi.fn(() => ({
    fftSize: 2048,
    frequencyBinCount: 1024,
    minDecibels: -100,
    maxDecibels: -30,
    smoothingTimeConstant: 0.8,
    connect: vi.fn(),
    disconnect: vi.fn(),
    getByteTimeDomainData: vi.fn((array: Uint8Array) => {
      // Fill with silence (128 is the center value for 8-bit audio)
      array.fill(128);
    }),
    getFloatTimeDomainData: vi.fn((array: Float32Array) => {
      array.fill(0);
    }),
    getByteFrequencyData: vi.fn((array: Uint8Array) => {
      array.fill(0);
    }),
    getFloatFrequencyData: vi.fn((array: Float32Array) => {
      array.fill(-Infinity);
    }),
  }));

  createScriptProcessor = vi.fn((bufferSize: number) => ({
    bufferSize,
    onaudioprocess: null,
    connect: vi.fn(),
    disconnect: vi.fn(),
    addEventListener: vi.fn(),
    removeEventListener: vi.fn(),
    dispatchEvent: vi.fn(),
  }));

  decodeAudioData = vi.fn().mockResolvedValue({
    length: 48000,
    duration: 1,
    sampleRate: 48000,
    numberOfChannels: 2,
    getChannelData: vi.fn(() => new Float32Array(48000)),
    copyFromChannel: vi.fn(),
    copyToChannel: vi.fn(),
  });

  suspend = vi.fn().mockResolvedValue(undefined);
  resume = vi.fn().mockResolvedValue(undefined);
  close = vi.fn().mockResolvedValue(undefined);

  addEventListener = vi.fn();
  removeEventListener = vi.fn();
  dispatchEvent = vi.fn();
}

export class MockMediaStream {
  id: string;
  active = true;
  tracks: MockMediaStreamTrack[] = [];

  constructor() {
    this.id = Math.random().toString(36).substr(2, 9);
    this.tracks = [new MockMediaStreamTrack('audio')];
  }

  getTracks() {
    return this.tracks;
  }

  getAudioTracks() {
    return this.tracks.filter(t => t.kind === 'audio');
  }

  getVideoTracks() {
    return this.tracks.filter(t => t.kind === 'video');
  }

  addTrack = vi.fn();
  removeTrack = vi.fn();
  clone = vi.fn(() => new MockMediaStream());
  getTrackById = vi.fn();
  addEventListener = vi.fn();
  removeEventListener = vi.fn();
  dispatchEvent = vi.fn();
}

export class MockMediaStreamTrack {
  id: string;
  kind: 'audio' | 'video';
  enabled = true;
  muted = false;
  label = '';
  readyState: 'live' | 'ended' = 'live';

  constructor(kind: 'audio' | 'video' = 'audio') {
    this.id = Math.random().toString(36).substr(2, 9);
    this.kind = kind;
    this.label = `Mock ${kind} track`;
  }

  stop = vi.fn(() => {
    this.readyState = 'ended';
  });

  clone = vi.fn(() => new MockMediaStreamTrack(this.kind));
  getCapabilities = vi.fn(() => ({}));
  getConstraints = vi.fn(() => ({}));
  getSettings = vi.fn(() => ({}));
  applyConstraints = vi.fn().mockResolvedValue(undefined);
  addEventListener = vi.fn();
  removeEventListener = vi.fn();
  dispatchEvent = vi.fn();
}

export function createMockMediaDevices() {
  return {
    getUserMedia: vi.fn().mockResolvedValue(new MockMediaStream()),
    enumerateDevices: vi.fn().mockResolvedValue([
      {
        deviceId: 'default',
        groupId: 'default',
        kind: 'audioinput',
        label: 'Default Audio Input',
      },
      {
        deviceId: 'default',
        groupId: 'default',
        kind: 'audiooutput',
        label: 'Default Audio Output',
      },
    ]),
    getSupportedConstraints: vi.fn(() => ({
      autoGainControl: true,
      echoCancellation: true,
      noiseSuppression: true,
      sampleRate: true,
      sampleSize: true,
      channelCount: true,
    })),
    addEventListener: vi.fn(),
    removeEventListener: vi.fn(),
    dispatchEvent: vi.fn(),
  };
}

export function setupAudioMocks() {
  global.AudioContext = MockAudioContext as any;
  global.MediaStream = MockMediaStream as any;
  global.MediaStreamTrack = MockMediaStreamTrack as any;
  
  if (!global.navigator) {
    (global as any).navigator = {};
  }
  
  Object.defineProperty(global.navigator, 'mediaDevices', {
    value: createMockMediaDevices(),
    writable: true,
    configurable: true,
  });
}