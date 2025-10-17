/**
 * Message Factory for Test Data Generation
 * Uses @faker-js/faker to create realistic test data
 */

import { faker } from '@faker-js/faker';
// Define test-specific types for message factories
export interface ChatMessage {
  id: string;
  type: string;
  role: 'user' | 'assistant' | 'system';
  content: string;
  timestamp: string;
  metadata?: any;
}

export interface SystemEvent {
  id: string;
  type: string;
  event_type: string;
  timestamp: string;
  data: any;
  severity: 'info' | 'warning' | 'error';
}

export interface ErrorResponse {
  id: string;
  type: string;
  error: {
    code: string;
    message: string;
    details: any;
  };
  timestamp: string;
}

export interface TurnEvent {
  id: string;
  type: string;
  turn_id: string;
  timestamp: string;
  turn_type: 'user' | 'assistant';
  metadata?: any;
}

export interface ToolCall {
  id: string;
  type: string;
  tool_call_id: string;
  name: string;
  arguments: any;
  timestamp: string;
  status: 'pending' | 'executing' | 'completed' | 'failed';
}

export interface ToolResponse {
  id: string;
  type: string;
  tool_call_id: string;
  result: any;
  timestamp: string;
  execution_time?: number;
}

export interface AudioEvent {
  id: string;
  type: string;
  timestamp: string;
  data?: ArrayBuffer;
  metadata?: any;
}

export interface ConnectionEvent {
  id: string;
  type: string;
  timestamp: string;
  session_id: string;
  data: any;
}

/**
 * Factory for creating chat messages
 */
export class ChatMessageFactory {
  static create(overrides: Partial<ChatMessage> = {}): ChatMessage {
    return {
      id: faker.string.uuid(),
      type: 'message',
      role: faker.helpers.arrayElement(['user', 'assistant', 'system']),
      content: faker.lorem.paragraph(),
      timestamp: faker.date.recent().toISOString(),
      metadata: {
        token_count: faker.number.int({ min: 10, max: 500 }),
        processing_time: faker.number.float({ min: 0.1, max: 2.0 }),
        model: faker.helpers.arrayElement(['gpt-4', 'gpt-3.5-turbo', 'claude-3'])
      },
      ...overrides
    };
  }

  static createUserMessage(content?: string): ChatMessage {
    return this.create({
      role: 'user',
      content: content || faker.lorem.sentence()
    });
  }

  static createAssistantMessage(content?: string): ChatMessage {
    return this.create({
      role: 'assistant',
      content: content || faker.lorem.paragraph()
    });
  }

  static createSystemMessage(content?: string): ChatMessage {
    return this.create({
      role: 'system',
      content: content || 'System notification: ' + faker.lorem.sentence()
    });
  }

  static createBatch(count: number = 10): ChatMessage[] {
    return Array.from({ length: count }, () => this.create());
  }

  static createConversation(exchanges: number = 5): ChatMessage[] {
    const messages: ChatMessage[] = [];
    
    for (let i = 0; i < exchanges; i++) {
      messages.push(this.createUserMessage());
      messages.push(this.createAssistantMessage());
    }
    
    return messages;
  }
}

/**
 * Factory for creating system events
 */
export class SystemEventFactory {
  static create(overrides: Partial<SystemEvent> = {}): SystemEvent {
    const eventType = faker.helpers.arrayElement([
      'session_started',
      'session_ended',
      'connection_established',
      'connection_lost',
      'rate_limit_warning',
      'maintenance_notice'
    ]);

    return {
      id: faker.string.uuid(),
      type: 'system_event',
      event_type: eventType,
      timestamp: faker.date.recent().toISOString(),
      data: this.generateEventData(eventType),
      severity: faker.helpers.arrayElement(['info', 'warning', 'error']),
      ...overrides
    };
  }

  private static generateEventData(eventType: string): any {
    switch (eventType) {
      case 'session_started':
        return {
          session_id: faker.string.uuid(),
          user_id: faker.string.uuid(),
          start_time: faker.date.recent().toISOString()
        };
      
      case 'connection_established':
        return {
          protocol: 'websocket',
          latency: faker.number.int({ min: 10, max: 100 }),
          region: faker.location.countryCode()
        };
      
      case 'rate_limit_warning':
        return {
          current_usage: faker.number.int({ min: 800, max: 950 }),
          limit: 1000,
          reset_time: faker.date.soon().toISOString()
        };
      
      default:
        return {
          message: faker.lorem.sentence()
        };
    }
  }

  static createBatch(count: number = 5): SystemEvent[] {
    return Array.from({ length: count }, () => this.create());
  }
}

/**
 * Factory for creating error responses
 */
export class ErrorResponseFactory {
  static create(overrides: Partial<ErrorResponse> = {}): ErrorResponse {
    const errorCode = faker.helpers.arrayElement([
      'INVALID_REQUEST',
      'AUTHENTICATION_FAILED',
      'RATE_LIMIT_EXCEEDED',
      'INTERNAL_ERROR',
      'NETWORK_ERROR',
      'TIMEOUT',
      'PERMISSION_DENIED'
    ]);

    return {
      id: faker.string.uuid(),
      type: 'error',
      error: {
        code: errorCode,
        message: this.getErrorMessage(errorCode),
        details: {
          timestamp: faker.date.recent().toISOString(),
          request_id: faker.string.uuid(),
          ...this.getErrorDetails(errorCode)
        }
      },
      timestamp: faker.date.recent().toISOString(),
      ...overrides
    };
  }

  private static getErrorMessage(code: string): string {
    const messages: Record<string, string> = {
      'INVALID_REQUEST': 'The request format is invalid',
      'AUTHENTICATION_FAILED': 'Authentication credentials are invalid or expired',
      'RATE_LIMIT_EXCEEDED': 'Too many requests, please slow down',
      'INTERNAL_ERROR': 'An internal server error occurred',
      'NETWORK_ERROR': 'Network connection failed',
      'TIMEOUT': 'Request timed out',
      'PERMISSION_DENIED': 'You do not have permission to perform this action'
    };
    
    return messages[code] || faker.lorem.sentence();
  }

  private static getErrorDetails(code: string): any {
    switch (code) {
      case 'RATE_LIMIT_EXCEEDED':
        return {
          limit: 100,
          current: faker.number.int({ min: 100, max: 150 }),
          reset_at: faker.date.soon().toISOString()
        };
      
      case 'INVALID_REQUEST':
        return {
          field: faker.helpers.arrayElement(['content', 'role', 'session_id']),
          reason: 'Field is required'
        };
      
      default:
        return {};
    }
  }

  static createValidationError(field: string): ErrorResponse {
    return this.create({
      error: {
        code: 'VALIDATION_ERROR',
        message: `Validation failed for field: ${field}`,
        details: {
          field,
          reason: faker.helpers.arrayElement(['required', 'invalid_format', 'too_long'])
        }
      }
    });
  }

  static createBatch(count: number = 3): ErrorResponse[] {
    return Array.from({ length: count }, () => this.create());
  }
}

/**
 * Factory for creating turn events
 */
export class TurnEventFactory {
  static create(overrides: Partial<TurnEvent> = {}): TurnEvent {
    const eventType = faker.helpers.arrayElement(['turn_start', 'turn_end', 'turn_interrupted']);
    
    return {
      id: faker.string.uuid(),
      type: eventType,
      turn_id: faker.string.uuid(),
      timestamp: faker.date.recent().toISOString(),
      turn_type: faker.helpers.arrayElement(['user', 'assistant']),
      metadata: {
        duration: eventType === 'turn_end' ? faker.number.float({ min: 0.5, max: 10 }) : undefined,
        interrupted: eventType === 'turn_interrupted',
        audio_detected: faker.datatype.boolean(),
        confidence: faker.number.float({ min: 0.7, max: 1.0 })
      },
      ...overrides
    };
  }

  static createTurnSequence(): TurnEvent[] {
    const turnId = faker.string.uuid();
    
    return [
      this.create({ 
        type: 'turn_start', 
        turn_id: turnId,
        turn_type: 'user' 
      }),
      this.create({ 
        type: 'turn_end', 
        turn_id: turnId,
        turn_type: 'user' 
      }),
      this.create({ 
        type: 'turn_start', 
        turn_id: faker.string.uuid(),
        turn_type: 'assistant' 
      })
    ];
  }

  static createInterruptedTurn(): TurnEvent[] {
    const turnId = faker.string.uuid();
    
    return [
      this.create({ 
        type: 'turn_start', 
        turn_id: turnId,
        turn_type: 'assistant' 
      }),
      this.create({ 
        type: 'turn_interrupted', 
        turn_id: turnId,
        turn_type: 'assistant' 
      })
    ];
  }
}

/**
 * Factory for creating tool calls
 */
export class ToolCallFactory {
  static create(overrides: Partial<ToolCall> = {}): ToolCall {
    const toolName = faker.helpers.arrayElement([
      'search_web',
      'calculate',
      'get_weather',
      'send_email',
      'create_calendar_event'
    ]);

    return {
      id: faker.string.uuid(),
      type: 'tool_call',
      tool_call_id: `call_${faker.string.alphanumeric(24)}`,
      name: toolName,
      arguments: this.generateToolArguments(toolName),
      timestamp: faker.date.recent().toISOString(),
      status: faker.helpers.arrayElement(['pending', 'executing', 'completed', 'failed']),
      ...overrides
    };
  }

  private static generateToolArguments(toolName: string): any {
    switch (toolName) {
      case 'search_web':
        return {
          query: faker.lorem.sentence(),
          num_results: faker.number.int({ min: 1, max: 10 })
        };
      
      case 'calculate':
        return {
          expression: `${faker.number.int({ min: 1, max: 100 })} + ${faker.number.int({ min: 1, max: 100 })}`
        };
      
      case 'get_weather':
        return {
          location: faker.location.city(),
          units: faker.helpers.arrayElement(['celsius', 'fahrenheit'])
        };
      
      case 'send_email':
        return {
          to: faker.internet.email(),
          subject: faker.lorem.sentence(),
          body: faker.lorem.paragraph()
        };
      
      case 'create_calendar_event':
        return {
          title: faker.lorem.sentence(),
          start_time: faker.date.soon().toISOString(),
          duration_minutes: faker.number.int({ min: 15, max: 120 })
        };
      
      default:
        return { data: faker.lorem.sentence() };
    }
  }

  static createWithResponse(): { call: ToolCall; response: ToolResponse } {
    const call = this.create({ status: 'completed' });
    const response = ToolResponseFactory.createForToolCall(call);
    
    return { call, response };
  }

  static createBatch(count: number = 3): ToolCall[] {
    return Array.from({ length: count }, () => this.create());
  }
}

/**
 * Factory for creating tool responses
 */
export class ToolResponseFactory {
  static create(overrides: Partial<ToolResponse> = {}): ToolResponse {
    return {
      id: faker.string.uuid(),
      type: 'tool_response',
      tool_call_id: `call_${faker.string.alphanumeric(24)}`,
      result: {
        success: faker.datatype.boolean(),
        data: faker.helpers.arrayElement([
          { answer: faker.lorem.paragraph() },
          { results: Array.from({ length: 3 }, () => faker.lorem.sentence()) },
          { value: faker.number.float({ min: 0, max: 1000 }) }
        ])
      },
      timestamp: faker.date.recent().toISOString(),
      execution_time: faker.number.float({ min: 0.1, max: 5.0 }),
      ...overrides
    };
  }

  static createForToolCall(toolCall: ToolCall): ToolResponse {
    return this.create({
      tool_call_id: toolCall.tool_call_id,
      result: this.generateResultForTool(toolCall.name)
    });
  }

  private static generateResultForTool(toolName: string): any {
    switch (toolName) {
      case 'search_web':
        return {
          success: true,
          data: {
            results: Array.from({ length: 3 }, () => ({
              title: faker.lorem.sentence(),
              url: faker.internet.url(),
              snippet: faker.lorem.paragraph()
            }))
          }
        };
      
      case 'calculate':
        return {
          success: true,
          data: {
            result: faker.number.float({ min: 0, max: 1000 })
          }
        };
      
      case 'get_weather':
        return {
          success: true,
          data: {
            temperature: faker.number.int({ min: -20, max: 40 }),
            condition: faker.helpers.arrayElement(['sunny', 'cloudy', 'rainy', 'snowy']),
            humidity: faker.number.int({ min: 0, max: 100 })
          }
        };
      
      default:
        return {
          success: true,
          data: { message: faker.lorem.sentence() }
        };
    }
  }

  static createError(toolCallId: string, error: string): ToolResponse {
    return this.create({
      tool_call_id: toolCallId,
      result: {
        success: false,
        error: {
          code: 'TOOL_EXECUTION_ERROR',
          message: error || faker.lorem.sentence()
        }
      }
    });
  }
}

/**
 * Factory for creating audio events
 */
export class AudioEventFactory {
  static create(overrides: Partial<AudioEvent> = {}): AudioEvent {
    const eventType = faker.helpers.arrayElement(['audio_start', 'audio_data', 'audio_end']);
    
    return {
      id: faker.string.uuid(),
      type: eventType,
      timestamp: faker.date.recent().toISOString(),
      data: eventType === 'audio_data' ? this.generateAudioData() : undefined,
      metadata: {
        sample_rate: 24000,
        channels: 1,
        format: 'pcm16',
        duration: eventType === 'audio_end' ? faker.number.float({ min: 0.5, max: 10 }) : undefined
      },
      ...overrides
    };
  }

  private static generateAudioData(): ArrayBuffer {
    const samples = faker.number.int({ min: 1024, max: 4096 });
    const buffer = new ArrayBuffer(samples * 2); // 16-bit audio
    const view = new Int16Array(buffer);
    
    // Generate simple sine wave
    const frequency = 440; // A4 note
    const sampleRate = 24000;
    
    for (let i = 0; i < samples; i++) {
      view[i] = Math.sin(2 * Math.PI * frequency * i / sampleRate) * 32767;
    }
    
    return buffer;
  }

  static createAudioStream(duration: number = 1000): AudioEvent[] {
    const events: AudioEvent[] = [];
    const chunks = Math.ceil(duration / 100); // 100ms chunks
    
    events.push(this.create({ type: 'audio_start' }));
    
    for (let i = 0; i < chunks; i++) {
      events.push(this.create({ type: 'audio_data' }));
    }
    
    events.push(this.create({ 
      type: 'audio_end',
      metadata: { duration: duration / 1000 }
    }));
    
    return events;
  }
}

/**
 * Factory for creating connection events
 */
export class ConnectionEventFactory {
  static create(overrides: Partial<ConnectionEvent> = {}): ConnectionEvent {
    const eventType = faker.helpers.arrayElement([
      'connected',
      'disconnected',
      'reconnecting',
      'error'
    ]);

    return {
      id: faker.string.uuid(),
      type: eventType,
      timestamp: faker.date.recent().toISOString(),
      session_id: faker.string.uuid(),
      data: this.generateConnectionData(eventType),
      ...overrides
    };
  }

  private static generateConnectionData(eventType: string): any {
    switch (eventType) {
      case 'connected':
        return {
          protocol: 'websocket',
          latency: faker.number.int({ min: 10, max: 100 }),
          version: '1.0.0'
        };
      
      case 'disconnected':
        return {
          reason: faker.helpers.arrayElement(['client_disconnect', 'timeout', 'error']),
          code: faker.helpers.arrayElement([1000, 1001, 1006])
        };
      
      case 'reconnecting':
        return {
          attempt: faker.number.int({ min: 1, max: 5 }),
          max_attempts: 5,
          delay: faker.number.int({ min: 1000, max: 5000 })
        };
      
      case 'error':
        return {
          error: faker.helpers.arrayElement(['network_error', 'auth_failed', 'timeout']),
          message: faker.lorem.sentence()
        };
      
      default:
        return {};
    }
  }

  static createConnectionSequence(): ConnectionEvent[] {
    const sessionId = faker.string.uuid();
    
    return [
      this.create({ type: 'connected', session_id: sessionId }),
      this.create({ type: 'disconnected', session_id: sessionId }),
      this.create({ type: 'reconnecting', session_id: sessionId }),
      this.create({ type: 'connected', session_id: sessionId })
    ];
  }
}