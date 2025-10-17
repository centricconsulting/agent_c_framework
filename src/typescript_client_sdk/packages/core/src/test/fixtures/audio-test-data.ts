/**
 * Audio Test Data Fixtures
 * Sample audio data for testing audio processing functionality
 */

/**
 * Generate a sine wave audio buffer
 */
export function generateSineWave(
  frequency: number = 440, // A4 note
  duration: number = 1000, // milliseconds
  sampleRate: number = 24000
): ArrayBuffer {
  const samples = Math.floor((duration / 1000) * sampleRate);
  const buffer = new ArrayBuffer(samples * 2); // 16-bit audio
  const view = new Int16Array(buffer);
  
  for (let i = 0; i < samples; i++) {
    const value = Math.sin(2 * Math.PI * frequency * i / sampleRate);
    view[i] = Math.floor(value * 32767); // Convert to 16-bit PCM
  }
  
  return buffer;
}

/**
 * Generate white noise audio buffer
 */
export function generateWhiteNoise(
  duration: number = 1000,
  sampleRate: number = 24000
): ArrayBuffer {
  const samples = Math.floor((duration / 1000) * sampleRate);
  const buffer = new ArrayBuffer(samples * 2);
  const view = new Int16Array(buffer);
  
  for (let i = 0; i < samples; i++) {
    view[i] = Math.floor((Math.random() * 2 - 1) * 32767);
  }
  
  return buffer;
}

/**
 * Generate silence audio buffer
 */
export function generateSilence(
  duration: number = 1000,
  sampleRate: number = 24000
): ArrayBuffer {
  const samples = Math.floor((duration / 1000) * sampleRate);
  const buffer = new ArrayBuffer(samples * 2);
  // ArrayBuffer is initialized with zeros, so this is silence
  return buffer;
}

/**
 * Generate a frequency sweep (chirp) audio buffer
 */
export function generateChirp(
  startFreq: number = 220,
  endFreq: number = 880,
  duration: number = 1000,
  sampleRate: number = 24000
): ArrayBuffer {
  const samples = Math.floor((duration / 1000) * sampleRate);
  const buffer = new ArrayBuffer(samples * 2);
  const view = new Int16Array(buffer);
  
  for (let i = 0; i < samples; i++) {
    const t = i / sampleRate;
    const freq = startFreq + (endFreq - startFreq) * (i / samples);
    const value = Math.sin(2 * Math.PI * freq * t);
    view[i] = Math.floor(value * 32767);
  }
  
  return buffer;
}

/**
 * Generate DTMF tone (phone dialing tone)
 */
export function generateDTMF(
  digit: string,
  duration: number = 200,
  sampleRate: number = 24000
): ArrayBuffer {
  // DTMF frequency pairs
  const dtmfFreqs: Record<string, [number, number]> = {
    '1': [697, 1209], '2': [697, 1336], '3': [697, 1477], 'A': [697, 1633],
    '4': [770, 1209], '5': [770, 1336], '6': [770, 1477], 'B': [770, 1633],
    '7': [852, 1209], '8': [852, 1336], '9': [852, 1477], 'C': [852, 1633],
    '*': [941, 1209], '0': [941, 1336], '#': [941, 1477], 'D': [941, 1633]
  };
  
  const freqs = dtmfFreqs[digit] || [697, 1209]; // Default to '1'
  const samples = Math.floor((duration / 1000) * sampleRate);
  const buffer = new ArrayBuffer(samples * 2);
  const view = new Int16Array(buffer);
  
  for (let i = 0; i < samples; i++) {
    const t = i / sampleRate;
    const value = (
      Math.sin(2 * Math.PI * freqs[0] * t) +
      Math.sin(2 * Math.PI * freqs[1] * t)
    ) / 2;
    view[i] = Math.floor(value * 32767);
  }
  
  return buffer;
}

/**
 * Generate a complex waveform with multiple harmonics
 */
export function generateComplexWave(
  fundamental: number = 220,
  harmonics: number[] = [2, 3, 4],
  amplitudes: number[] = [0.5, 0.3, 0.2],
  duration: number = 1000,
  sampleRate: number = 24000
): ArrayBuffer {
  const samples = Math.floor((duration / 1000) * sampleRate);
  const buffer = new ArrayBuffer(samples * 2);
  const view = new Int16Array(buffer);
  
  for (let i = 0; i < samples; i++) {
    const t = i / sampleRate;
    let value = Math.sin(2 * Math.PI * fundamental * t); // Fundamental
    
    // Add harmonics
    for (let h = 0; h < harmonics.length; h++) {
      const harmonic = harmonics[h];
      const amplitude = amplitudes[h] || 0.3;
      value += amplitude * Math.sin(2 * Math.PI * fundamental * harmonic * t);
    }
    
    // Normalize
    value = value / (1 + amplitudes.reduce((a, b) => a + b, 0));
    view[i] = Math.floor(value * 32767);
  }
  
  return buffer;
}

/**
 * Audio test configurations
 */
export const audioConfigs = {
  standard: {
    sampleRate: 24000,
    channels: 1,
    bitsPerSample: 16,
    format: 'pcm16'
  },
  highQuality: {
    sampleRate: 48000,
    channels: 2,
    bitsPerSample: 24,
    format: 'pcm24'
  },
  lowBandwidth: {
    sampleRate: 16000,
    channels: 1,
    bitsPerSample: 16,
    format: 'pcm16'
  },
  opus: {
    sampleRate: 48000,
    channels: 1,
    bitrate: 32000,
    format: 'opus'
  }
};

/**
 * Sample audio levels for testing
 */
export const audioLevels = {
  silent: 0,
  veryQuiet: 0.01,
  quiet: 0.1,
  normal: 0.5,
  loud: 0.8,
  veryLoud: 0.95,
  clipping: 1.0
};

/**
 * Convert audio buffer to base64 string
 */
export function audioBufferToBase64(buffer: ArrayBuffer): string {
  const bytes = new Uint8Array(buffer);
  let binary = '';
  for (let i = 0; i < bytes.byteLength; i++) {
    binary += String.fromCharCode(bytes[i]);
  }
  return btoa(binary);
}

/**
 * Convert base64 string to audio buffer
 */
export function base64ToAudioBuffer(base64: string): ArrayBuffer {
  const binary = atob(base64);
  const buffer = new ArrayBuffer(binary.length);
  const bytes = new Uint8Array(buffer);
  for (let i = 0; i < binary.length; i++) {
    bytes[i] = binary.charCodeAt(i);
  }
  return buffer;
}

/**
 * Calculate RMS (Root Mean Square) of audio buffer
 */
export function calculateRMS(buffer: ArrayBuffer): number {
  const view = new Int16Array(buffer);
  let sum = 0;
  
  for (let i = 0; i < view.length; i++) {
    const normalized = view[i] / 32768; // Normalize to [-1, 1]
    sum += normalized * normalized;
  }
  
  return Math.sqrt(sum / view.length);
}

/**
 * Detect if audio buffer contains silence
 */
export function isSilent(buffer: ArrayBuffer, threshold: number = 0.01): boolean {
  return calculateRMS(buffer) < threshold;
}

/**
 * Mix two audio buffers
 */
export function mixAudioBuffers(
  buffer1: ArrayBuffer,
  buffer2: ArrayBuffer,
  ratio: number = 0.5
): ArrayBuffer {
  const view1 = new Int16Array(buffer1);
  const view2 = new Int16Array(buffer2);
  const length = Math.min(view1.length, view2.length);
  const mixed = new ArrayBuffer(length * 2);
  const mixedView = new Int16Array(mixed);
  
  for (let i = 0; i < length; i++) {
    const sample1 = view1[i] / 32768;
    const sample2 = view2[i] / 32768;
    const mixedSample = sample1 * (1 - ratio) + sample2 * ratio;
    mixedView[i] = Math.floor(mixedSample * 32767);
  }
  
  return mixed;
}

/**
 * Apply gain to audio buffer
 */
export function applyGain(buffer: ArrayBuffer, gain: number): ArrayBuffer {
  const view = new Int16Array(buffer);
  const output = new ArrayBuffer(buffer.byteLength);
  const outputView = new Int16Array(output);
  
  for (let i = 0; i < view.length; i++) {
    const sample = view[i] * gain;
    // Clamp to prevent overflow
    outputView[i] = Math.max(-32768, Math.min(32767, Math.floor(sample)));
  }
  
  return output;
}

/**
 * Generate test audio samples
 */
export const testAudioSamples = {
  shortTone: () => generateSineWave(440, 100),
  mediumTone: () => generateSineWave(440, 500),
  longTone: () => generateSineWave(440, 2000),
  noise: () => generateWhiteNoise(1000),
  silence: () => generateSilence(1000),
  chirp: () => generateChirp(220, 880, 1000),
  dtmf: (digit: string = '5') => generateDTMF(digit, 200),
  complex: () => generateComplexWave(220, [2, 3, 5], [0.5, 0.3, 0.1], 1000)
};