/**
 * Audio Worklet Processor for PCM16 conversion
 * Processes Web Audio float32 samples to PCM16 (16-bit signed integers)
 * Runs in a separate thread for optimal performance
 */

class PCM16Processor extends AudioWorkletProcessor {
  constructor() {
    super();
    
    // Configuration
    this.bufferSize = 2048; // Samples to accumulate before sending
    this.sampleBuffer = new Float32Array(this.bufferSize);
    this.bufferIndex = 0;
    
    // State
    this.isProcessing = false;
    
    // Handle messages from main thread
    this.port.onmessage = (event) => {
      this.handleMessage(event.data);
    };
    
    // Notify main thread that processor is ready
    this.port.postMessage({ type: 'ready' });
  }
  
  /**
   * Handle control messages from main thread
   */
  handleMessage(data) {
    switch (data.type) {
      case 'start':
        this.isProcessing = true;
        this.bufferIndex = 0;
        this.port.postMessage({ type: 'started' });
        break;
        
      case 'stop':
        this.isProcessing = false;
        this.bufferIndex = 0;
        this.port.postMessage({ type: 'stopped' });
        break;
        
      case 'configure':
        if (data.bufferSize && data.bufferSize > 0) {
          this.bufferSize = data.bufferSize;
          this.sampleBuffer = new Float32Array(this.bufferSize);
          this.bufferIndex = 0;
        }
        break;
        
      default:
        console.warn('Unknown message type:', data.type);
    }
  }
  
  /**
   * Convert Float32Array to PCM16 (Int16Array)
   * @param {Float32Array} float32Data - Input audio samples (-1 to 1)
   * @returns {Int16Array} PCM16 audio data
   */
  convertToPCM16(float32Data) {
    const pcm16 = new Int16Array(float32Data.length);
    
    for (let i = 0; i < float32Data.length; i++) {
      // Clamp to [-1, 1] range
      let sample = Math.max(-1, Math.min(1, float32Data[i]));
      
      // Convert to 16-bit signed integer
      // Multiply by 32767 (max positive value for Int16)
      pcm16[i] = Math.floor(sample * 32767);
    }
    
    return pcm16;
  }
  
  /**
   * Calculate RMS (Root Mean Square) for audio level
   * @param {Float32Array} samples - Audio samples
   * @returns {number} RMS value (0 to 1)
   */
  calculateRMS(samples) {
    let sum = 0;
    for (let i = 0; i < samples.length; i++) {
      sum += samples[i] * samples[i];
    }
    return Math.sqrt(sum / samples.length);
  }
  
  /**
   * Process audio samples from the Web Audio API
   * Called automatically by the browser's audio system
   * 
   * @param {Float32Array[][]} inputs - Input audio data
   * @param {Float32Array[][]} outputs - Output audio data (pass-through)
   * @param {Object} parameters - AudioParam values
   * @returns {boolean} true to keep processor alive
   */
  process(inputs, outputs, parameters) {
    // Get the first input's first channel (mono)
    const input = inputs[0];
    
    if (!input || !input[0] || !this.isProcessing) {
      // No input or not processing, keep alive but don't process
      return true;
    }
    
    const inputChannel = input[0];
    
    // Pass audio through to output (monitoring)
    const output = outputs[0];
    if (output && output[0]) {
      output[0].set(inputChannel);
    }
    
    // Accumulate samples in buffer
    for (let i = 0; i < inputChannel.length; i++) {
      this.sampleBuffer[this.bufferIndex++] = inputChannel[i];
      
      // When buffer is full, convert and send
      if (this.bufferIndex >= this.bufferSize) {
        this.sendAudioChunk();
      }
    }
    
    // Keep processor alive
    return true;
  }
  
  /**
   * Convert accumulated samples to PCM16 and send to main thread
   */
  sendAudioChunk() {
    // Extract the filled portion of the buffer
    const samples = this.sampleBuffer.slice(0, this.bufferIndex);
    
    // Convert to PCM16
    const pcm16Data = this.convertToPCM16(samples);
    
    // Calculate audio level for visualization
    const audioLevel = this.calculateRMS(samples);
    
    // Create ArrayBuffer from PCM16 data
    const audioBuffer = pcm16Data.buffer;
    
    // Send to main thread
    // Note: ArrayBuffer is transferred (zero-copy), not cloned
    this.port.postMessage({
      type: 'audio_chunk',
      audioBuffer: audioBuffer,
      audioLevel: audioLevel,
      sampleCount: this.bufferIndex,
      timestamp: globalThis.currentTime, // AudioWorkletGlobalScope.currentTime
      sampleRate: globalThis.sampleRate  // AudioWorkletGlobalScope.sampleRate
    }, [audioBuffer]); // Transfer ownership of ArrayBuffer
    
    // Reset buffer
    this.bufferIndex = 0;
  }
}

// Register the processor
registerProcessor('pcm16-processor', PCM16Processor);