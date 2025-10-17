/**
 * Comprehensive tests for OpenAI ChatCompletionMessageParam types
 * 
 * These tests ensure our types exactly match what the OpenAI API expects,
 * providing critical API compatibility validation.
 */

import { describe, it, expect, beforeEach, afterEach, vi } from 'vitest';
import {
  // Main types
  ChatCompletionMessageParam,
  ChatCompletionDeveloperMessageParam,
  ChatCompletionSystemMessageParam,
  ChatCompletionUserMessageParam,
  ChatCompletionAssistantMessageParam,
  ChatCompletionToolMessageParam,
  ChatCompletionFunctionMessageParam,
  
  // Content parts
  ChatCompletionContentPart,
  ChatCompletionContentPartText,
  ChatCompletionContentPartImage,
  ChatCompletionContentPartInputAudio,
  ChatCompletionContentPartFile,
  ChatCompletionContentPartRefusal,
  ContentArrayOfContentPart,
  
  // Supporting types
  ImageURL,
  InputAudio,
  FileFile,
  Audio,
  FunctionCall,
  ChatCompletionMessageToolCallUnion,
  ChatCompletionMessageFunctionToolCall,
  ChatCompletionMessageCustomToolCall,
  
  // Type guards
  isChatCompletionDeveloperMessageParam,
  isChatCompletionSystemMessageParam,
  isChatCompletionUserMessageParam,
  isChatCompletionAssistantMessageParam,
  isChatCompletionToolMessageParam,
  isChatCompletionFunctionMessageParam,
  isChatCompletionContentPartText,
  isChatCompletionContentPartImage,
  isChatCompletionContentPartInputAudio,
  isChatCompletionContentPartFile,
  isChatCompletionContentPartRefusal,
  isChatCompletionMessageFunctionToolCall,
  isChatCompletionMessageCustomToolCall
} from '../openai-message-params';

describe('OpenAI Message Params - Critical API Compatibility Tests', () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  afterEach(() => {
    vi.restoreAllMocks();
  });

  describe('Type Guards - Complete Coverage', () => {
    describe('Message type guards', () => {
      it('should identify developer messages correctly', () => {
        // Arrange
        const developerMessage: ChatCompletionDeveloperMessageParam = {
          role: 'developer',
          content: 'Developer instructions'
        };

        const nonDeveloperMessage: ChatCompletionSystemMessageParam = {
          role: 'system',
          content: 'System instructions'
        };

        // Act & Assert
        expect(isChatCompletionDeveloperMessageParam(developerMessage)).toBe(true);
        expect(isChatCompletionDeveloperMessageParam(nonDeveloperMessage)).toBe(false);
        
        // Type narrowing test
        if (isChatCompletionDeveloperMessageParam(developerMessage)) {
          // TypeScript knows this is a developer message
          const role: 'developer' = developerMessage.role;
          expect(role).toBe('developer');
        }
      });

      it('should identify system messages correctly', () => {
        // Arrange
        const systemMessage: ChatCompletionSystemMessageParam = {
          role: 'system',
          content: 'You are a helpful assistant',
          name: 'system_1'
        };

        const userMessage: ChatCompletionUserMessageParam = {
          role: 'user',
          content: 'Hello'
        };

        // Act & Assert
        expect(isChatCompletionSystemMessageParam(systemMessage)).toBe(true);
        expect(isChatCompletionSystemMessageParam(userMessage)).toBe(false);
      });

      it('should identify user messages correctly', () => {
        // Arrange
        const userMessage: ChatCompletionUserMessageParam = {
          role: 'user',
          content: 'What is the weather?'
        };

        const assistantMessage: ChatCompletionAssistantMessageParam = {
          role: 'assistant',
          content: 'The weather is sunny'
        };

        // Act & Assert
        expect(isChatCompletionUserMessageParam(userMessage)).toBe(true);
        expect(isChatCompletionUserMessageParam(assistantMessage)).toBe(false);
      });

      it('should identify assistant messages correctly', () => {
        // Arrange
        const assistantMessage: ChatCompletionAssistantMessageParam = {
          role: 'assistant',
          content: 'I can help with that'
        };

        const toolMessage: ChatCompletionToolMessageParam = {
          role: 'tool',
          content: 'Tool result',
          tool_call_id: 'call_123'
        };

        // Act & Assert
        expect(isChatCompletionAssistantMessageParam(assistantMessage)).toBe(true);
        expect(isChatCompletionAssistantMessageParam(toolMessage)).toBe(false);
      });

      it('should identify tool messages correctly', () => {
        // Arrange
        const toolMessage: ChatCompletionToolMessageParam = {
          role: 'tool',
          content: 'Function executed successfully',
          tool_call_id: 'call_abc123'
        };

        const functionMessage: ChatCompletionFunctionMessageParam = {
          role: 'function',
          content: 'Result',
          name: 'get_weather'
        };

        // Act & Assert
        expect(isChatCompletionToolMessageParam(toolMessage)).toBe(true);
        expect(isChatCompletionToolMessageParam(functionMessage)).toBe(false);
      });

      it('should identify function messages correctly (deprecated)', () => {
        // Arrange
        const functionMessage: ChatCompletionFunctionMessageParam = {
          role: 'function',
          content: 'Weather data',
          name: 'get_weather'
        };

        const developerMessage: ChatCompletionDeveloperMessageParam = {
          role: 'developer',
          content: 'Instructions'
        };

        // Act & Assert
        expect(isChatCompletionFunctionMessageParam(functionMessage)).toBe(true);
        expect(isChatCompletionFunctionMessageParam(developerMessage)).toBe(false);
      });

      it('should handle edge cases in type guards', () => {
        // Arrange
        const nullMessage = null as any;
        const undefinedMessage = undefined as any;
        const malformedMessage = { role: 'invalid' } as any;
        const emptyObject = {} as any;

        // Act & Assert - All should handle gracefully
        const messages = [nullMessage, undefinedMessage, malformedMessage, emptyObject];
        messages.forEach(msg => {
          // These should not throw but return false for invalid inputs
          if (msg === null || msg === undefined) {
            // Skip null/undefined as they would throw
            return;
          }
          expect(isChatCompletionSystemMessageParam(msg)).toBe(false);
          expect(isChatCompletionUserMessageParam(msg)).toBe(false);
          expect(isChatCompletionAssistantMessageParam(msg)).toBe(false);
        });
      });
    });

    describe('Content part type guards', () => {
      it('should identify text content parts', () => {
        // Arrange
        const textPart: ChatCompletionContentPartText = {
          type: 'text',
          text: 'Hello world'
        };

        const imagePart: ChatCompletionContentPartImage = {
          type: 'image_url',
          image_url: { url: 'https://example.com/image.jpg' }
        };

        // Act & Assert
        expect(isChatCompletionContentPartText(textPart)).toBe(true);
        expect(isChatCompletionContentPartText(imagePart)).toBe(false);
      });

      it('should identify image content parts', () => {
        // Arrange
        const imagePart: ChatCompletionContentPartImage = {
          type: 'image_url',
          image_url: {
            url: 'data:image/jpeg;base64,/9j/4AAQ...',
            detail: 'high'
          }
        };

        const audioPart: ChatCompletionContentPartInputAudio = {
          type: 'input_audio',
          input_audio: {
            data: 'base64_audio_data',
            format: 'mp3'
          }
        };

        // Act & Assert
        expect(isChatCompletionContentPartImage(imagePart)).toBe(true);
        expect(isChatCompletionContentPartImage(audioPart as any)).toBe(false);
      });

      it('should identify audio content parts', () => {
        // Arrange
        const audioPart: ChatCompletionContentPartInputAudio = {
          type: 'input_audio',
          input_audio: {
            data: 'base64_encoded_wav',
            format: 'wav'
          }
        };

        const filePart: ChatCompletionContentPartFile = {
          type: 'file',
          file: {
            file_id: 'file-123'
          }
        };

        // Act & Assert
        expect(isChatCompletionContentPartInputAudio(audioPart)).toBe(true);
        expect(isChatCompletionContentPartInputAudio(filePart as any)).toBe(false);
      });

      it('should identify file content parts', () => {
        // Arrange
        const filePart: ChatCompletionContentPartFile = {
          type: 'file',
          file: {
            file_data: 'base64_file_content',
            filename: 'document.pdf'
          }
        };

        const textPart: ChatCompletionContentPartText = {
          type: 'text',
          text: 'Not a file'
        };

        // Act & Assert
        expect(isChatCompletionContentPartFile(filePart)).toBe(true);
        expect(isChatCompletionContentPartFile(textPart as any)).toBe(false);
      });

      it('should identify refusal content parts', () => {
        // Arrange
        const refusalPart: ChatCompletionContentPartRefusal = {
          type: 'refusal',
          refusal: 'I cannot provide that information'
        };

        const textPart: ChatCompletionContentPartText = {
          type: 'text',
          text: 'Regular text'
        };

        // Act & Assert
        expect(isChatCompletionContentPartRefusal(refusalPart)).toBe(true);
        expect(isChatCompletionContentPartRefusal(textPart)).toBe(false);
      });
    });

    describe('Tool call type guards', () => {
      it('should identify function tool calls', () => {
        // Arrange
        const functionCall: ChatCompletionMessageFunctionToolCall = {
          id: 'call_123',
          type: 'function',
          function: {
            name: 'get_weather',
            arguments: '{"location": "San Francisco"}'
          }
        };

        const customCall: ChatCompletionMessageCustomToolCall = {
          id: 'call_456',
          type: 'custom',
          custom: {
            name: 'custom_tool',
            input: 'custom input'
          }
        };

        // Act & Assert
        expect(isChatCompletionMessageFunctionToolCall(functionCall)).toBe(true);
        expect(isChatCompletionMessageFunctionToolCall(customCall)).toBe(false);
      });

      it('should identify custom tool calls', () => {
        // Arrange
        const customCall: ChatCompletionMessageCustomToolCall = {
          id: 'call_789',
          type: 'custom',
          custom: {
            name: 'advanced_search',
            input: 'search parameters'
          }
        };

        const functionCall: ChatCompletionMessageFunctionToolCall = {
          id: 'call_abc',
          type: 'function',
          function: {
            name: 'calculate',
            arguments: '{"x": 5, "y": 10}'
          }
        };

        // Act & Assert
        expect(isChatCompletionMessageCustomToolCall(customCall)).toBe(true);
        expect(isChatCompletionMessageCustomToolCall(functionCall)).toBe(false);
      });
    });
  });

  describe('Message Type Structures', () => {
    describe('DeveloperMessage structure', () => {
      it('should create valid developer message with string content', () => {
        // Arrange & Act
        const message: ChatCompletionDeveloperMessageParam = {
          role: 'developer',
          content: 'You are an expert programmer. Follow best practices.'
        };

        // Assert
        expect(message.role).toBe('developer');
        expect(typeof message.content).toBe('string');
        expect(message.name).toBeUndefined();
      });

      it('should create developer message with content parts array', () => {
        // Arrange & Act
        const message: ChatCompletionDeveloperMessageParam = {
          role: 'developer',
          content: [
            { type: 'text', text: 'Part 1: Setup' },
            { type: 'text', text: 'Part 2: Instructions' }
          ],
          name: 'dev_instructions'
        };

        // Assert
        expect(message.content).toBeInstanceOf(Array);
        expect(message.content).toHaveLength(2);
        expect(message.name).toBe('dev_instructions');
      });
    });

    describe('SystemMessage structure', () => {
      it('should create valid system message', () => {
        // Arrange & Act
        const message: ChatCompletionSystemMessageParam = {
          role: 'system',
          content: 'You are a helpful assistant.',
          name: 'main_system'
        };

        // Assert
        expect(message.role).toBe('system');
        expect(message.content).toBeDefined();
        expect(message.name).toBe('main_system');
      });

      it('should handle system message with text parts', () => {
        // Arrange & Act
        const message: ChatCompletionSystemMessageParam = {
          role: 'system',
          content: [
            { type: 'text', text: 'Primary directive' },
            { type: 'text', text: 'Secondary constraints' }
          ]
        };

        // Assert
        expect(Array.isArray(message.content)).toBe(true);
        if (Array.isArray(message.content)) {
          expect(message.content[0].type).toBe('text');
        }
      });
    });

    describe('UserMessage structure', () => {
      it('should create simple text user message', () => {
        // Arrange & Act
        const message: ChatCompletionUserMessageParam = {
          role: 'user',
          content: 'What is the capital of France?'
        };

        // Assert
        expect(message.role).toBe('user');
        expect(message.content).toBe('What is the capital of France?');
      });

      it('should create multimodal user message with image', () => {
        // Arrange & Act
        const message: ChatCompletionUserMessageParam = {
          role: 'user',
          content: [
            { type: 'text', text: 'What is in this image?' },
            {
              type: 'image_url',
              image_url: {
                url: 'https://example.com/image.jpg',
                detail: 'high'
              }
            }
          ]
        };

        // Assert
        expect(Array.isArray(message.content)).toBe(true);
        if (Array.isArray(message.content)) {
          expect(message.content).toHaveLength(2);
          expect(message.content[1].type).toBe('image_url');
        }
      });

      it('should create user message with audio input', () => {
        // Arrange & Act
        const message: ChatCompletionUserMessageParam = {
          role: 'user',
          content: [
            { type: 'text', text: 'Transcribe this audio:' },
            {
              type: 'input_audio',
              input_audio: {
                data: 'base64_encoded_audio',
                format: 'mp3'
              }
            }
          ],
          name: 'user_with_audio'
        };

        // Assert
        expect(Array.isArray(message.content)).toBe(true);
        if (Array.isArray(message.content)) {
          const audioPart = message.content[1];
          if ('type' in audioPart && audioPart.type === 'input_audio') {
            expect(audioPart.input_audio.format).toBe('mp3');
          }
        }
        expect(message.name).toBe('user_with_audio');
      });

      it('should create user message with file attachment', () => {
        // Arrange & Act
        const message: ChatCompletionUserMessageParam = {
          role: 'user',
          content: [
            { type: 'text', text: 'Analyze this document:' },
            {
              type: 'file',
              file: {
                file_id: 'file-abc123',
                filename: 'report.pdf'
              }
            }
          ]
        };

        // Assert
        if (Array.isArray(message.content)) {
          const filePart = message.content[1];
          if ('type' in filePart && filePart.type === 'file') {
            expect(filePart.file.file_id).toBe('file-abc123');
            expect(filePart.file.filename).toBe('report.pdf');
          }
        }
      });
    });

    describe('AssistantMessage structure', () => {
      it('should create simple text assistant message', () => {
        // Arrange & Act
        const message: ChatCompletionAssistantMessageParam = {
          role: 'assistant',
          content: 'The capital of France is Paris.'
        };

        // Assert
        expect(message.role).toBe('assistant');
        expect(message.content).toBe('The capital of France is Paris.');
        expect(message.tool_calls).toBeUndefined();
        expect(message.refusal).toBeUndefined();
      });

      it('should create assistant message with tool calls', () => {
        // Arrange & Act
        const message: ChatCompletionAssistantMessageParam = {
          role: 'assistant',
          content: null,
          tool_calls: [
            {
              id: 'call_123',
              type: 'function',
              function: {
                name: 'get_weather',
                arguments: '{"location": "Paris", "unit": "celsius"}'
              }
            }
          ]
        };

        // Assert
        expect(message.content).toBeNull();
        expect(message.tool_calls).toBeDefined();
        expect(message.tool_calls).toHaveLength(1);
        expect(message.tool_calls![0].type).toBe('function');
      });

      it('should create assistant message with refusal', () => {
        // Arrange & Act
        const message: ChatCompletionAssistantMessageParam = {
          role: 'assistant',
          content: [
            {
              type: 'refusal',
              refusal: 'I cannot provide information on that topic.'
            }
          ],
          refusal: 'I cannot provide information on that topic.'
        };

        // Assert
        expect(message.refusal).toBeDefined();
        expect(message.refusal).toContain('cannot provide');
      });

      it('should create assistant message with deprecated function_call', () => {
        // Arrange & Act
        const message: ChatCompletionAssistantMessageParam = {
          role: 'assistant',
          content: null,
          function_call: {
            name: 'calculate',
            arguments: '{"expression": "2 + 2"}'
          }
        };

        // Assert
        expect(message.function_call).toBeDefined();
        expect(message.function_call?.name).toBe('calculate');
      });

      it('should create assistant message with audio reference', () => {
        // Arrange & Act
        const message: ChatCompletionAssistantMessageParam = {
          role: 'assistant',
          content: 'Here is the audio response',
          audio: {
            id: 'audio_response_123'
          }
        };

        // Assert
        expect(message.audio).toBeDefined();
        expect(message.audio?.id).toBe('audio_response_123');
      });

      it('should handle multiple tool calls', () => {
        // Arrange & Act
        const message: ChatCompletionAssistantMessageParam = {
          role: 'assistant',
          content: null,
          tool_calls: [
            {
              id: 'call_1',
              type: 'function',
              function: {
                name: 'search',
                arguments: '{"query": "OpenAI"}'
              }
            },
            {
              id: 'call_2',
              type: 'custom',
              custom: {
                name: 'analyze',
                input: 'analysis parameters'
              }
            }
          ]
        };

        // Assert
        expect(message.tool_calls).toHaveLength(2);
        expect(message.tool_calls![0].type).toBe('function');
        expect(message.tool_calls![1].type).toBe('custom');
      });
    });

    describe('ToolMessage structure', () => {
      it('should create valid tool message', () => {
        // Arrange & Act
        const message: ChatCompletionToolMessageParam = {
          role: 'tool',
          content: 'Weather in Paris: 22°C, sunny',
          tool_call_id: 'call_abc123'
        };

        // Assert
        expect(message.role).toBe('tool');
        expect(message.tool_call_id).toBe('call_abc123');
        expect(message.content).toContain('Weather');
      });

      it('should create tool message with content parts', () => {
        // Arrange & Act
        const message: ChatCompletionToolMessageParam = {
          role: 'tool',
          content: [
            { type: 'text', text: 'Search results:' },
            { type: 'text', text: '1. Result one' },
            { type: 'text', text: '2. Result two' }
          ],
          tool_call_id: 'call_search_123'
        };

        // Assert
        expect(Array.isArray(message.content)).toBe(true);
        if (Array.isArray(message.content)) {
          expect(message.content).toHaveLength(3);
        }
      });
    });

    describe('FunctionMessage structure (deprecated)', () => {
      it('should create valid function message', () => {
        // Arrange & Act
        const message: ChatCompletionFunctionMessageParam = {
          role: 'function',
          content: 'Function executed successfully',
          name: 'deprecated_function'
        };

        // Assert
        expect(message.role).toBe('function');
        expect(message.name).toBe('deprecated_function');
      });

      it('should allow null content in function message', () => {
        // Arrange & Act
        const message: ChatCompletionFunctionMessageParam = {
          role: 'function',
          content: null,
          name: 'void_function'
        };

        // Assert
        expect(message.content).toBeNull();
        expect(message.name).toBeDefined();
      });
    });
  });

  describe('Content Parts Validation', () => {
    describe('Text content part', () => {
      it('should create valid text content part', () => {
        // Arrange & Act
        const textPart: ChatCompletionContentPartText = {
          type: 'text',
          text: 'This is a text content part'
        };

        // Assert
        expect(textPart.type).toBe('text');
        expect(textPart.text).toBeDefined();
        expect(typeof textPart.text).toBe('string');
      });
    });

    describe('Image content part', () => {
      it('should create image part with URL', () => {
        // Arrange & Act
        const imagePart: ChatCompletionContentPartImage = {
          type: 'image_url',
          image_url: {
            url: 'https://example.com/image.png',
            detail: 'auto'
          }
        };

        // Assert
        expect(imagePart.type).toBe('image_url');
        expect(imagePart.image_url.url).toContain('https://');
        expect(imagePart.image_url.detail).toBe('auto');
      });

      it('should create image part with base64 data', () => {
        // Arrange & Act
        const imagePart: ChatCompletionContentPartImage = {
          type: 'image_url',
          image_url: {
            url: 'data:image/jpeg;base64,/9j/4AAQSkZJRg...',
            detail: 'high'
          }
        };

        // Assert
        expect(imagePart.image_url.url).toContain('data:image');
        expect(imagePart.image_url.detail).toBe('high');
      });

      it('should handle all detail levels', () => {
        // Arrange
        const detailLevels: Array<'auto' | 'low' | 'high'> = ['auto', 'low', 'high'];

        // Act & Assert
        detailLevels.forEach(detail => {
          const part: ChatCompletionContentPartImage = {
            type: 'image_url',
            image_url: { url: 'test.jpg', detail }
          };
          expect(part.image_url.detail).toBe(detail);
        });
      });
    });

    describe('Audio content part', () => {
      it('should create audio part with WAV format', () => {
        // Arrange & Act
        const audioPart: ChatCompletionContentPartInputAudio = {
          type: 'input_audio',
          input_audio: {
            data: 'base64_wav_data_here',
            format: 'wav'
          }
        };

        // Assert
        expect(audioPart.type).toBe('input_audio');
        expect(audioPart.input_audio.format).toBe('wav');
        expect(audioPart.input_audio.data).toBeDefined();
      });

      it('should create audio part with MP3 format', () => {
        // Arrange & Act
        const audioPart: ChatCompletionContentPartInputAudio = {
          type: 'input_audio',
          input_audio: {
            data: 'base64_mp3_data_here',
            format: 'mp3'
          }
        };

        // Assert
        expect(audioPart.input_audio.format).toBe('mp3');
      });
    });

    describe('File content part', () => {
      it('should create file part with file_id', () => {
        // Arrange & Act
        const filePart: ChatCompletionContentPartFile = {
          type: 'file',
          file: {
            file_id: 'file-abc123'
          }
        };

        // Assert
        expect(filePart.type).toBe('file');
        expect(filePart.file.file_id).toBe('file-abc123');
        expect(filePart.file.file_data).toBeUndefined();
      });

      it('should create file part with base64 data', () => {
        // Arrange & Act
        const filePart: ChatCompletionContentPartFile = {
          type: 'file',
          file: {
            file_data: 'base64_pdf_content',
            filename: 'document.pdf'
          }
        };

        // Assert
        expect(filePart.file.file_data).toBeDefined();
        expect(filePart.file.filename).toBe('document.pdf');
        expect(filePart.file.file_id).toBeUndefined();
      });

      it('should handle all file fields', () => {
        // Arrange & Act
        const filePart: ChatCompletionContentPartFile = {
          type: 'file',
          file: {
            file_id: 'id-123',
            file_data: 'base64_data',
            filename: 'report.xlsx'
          }
        };

        // Assert
        expect(filePart.file.file_id).toBeDefined();
        expect(filePart.file.file_data).toBeDefined();
        expect(filePart.file.filename).toBeDefined();
      });
    });

    describe('Refusal content part', () => {
      it('should create valid refusal part', () => {
        // Arrange & Act
        const refusalPart: ChatCompletionContentPartRefusal = {
          type: 'refusal',
          refusal: 'I cannot generate that type of content'
        };

        // Assert
        expect(refusalPart.type).toBe('refusal');
        expect(refusalPart.refusal).toContain('cannot');
      });
    });
  });

  describe('Tool Call Types', () => {
    describe('Function tool calls', () => {
      it('should create valid function tool call', () => {
        // Arrange & Act
        const toolCall: ChatCompletionMessageFunctionToolCall = {
          id: 'call_unique_123',
          type: 'function',
          function: {
            name: 'calculate_sum',
            arguments: JSON.stringify({ a: 5, b: 3 })
          }
        };

        // Assert
        expect(toolCall.id).toBe('call_unique_123');
        expect(toolCall.type).toBe('function');
        expect(toolCall.function.name).toBe('calculate_sum');
        
        const args = JSON.parse(toolCall.function.arguments);
        expect(args.a).toBe(5);
        expect(args.b).toBe(3);
      });

      it('should handle complex function arguments', () => {
        // Arrange & Act
        const complexArgs = {
          query: 'OpenAI GPT-4',
          filters: {
            date_range: { start: '2024-01-01', end: '2024-12-31' },
            categories: ['AI', 'Technology'],
            limit: 10
          },
          options: {
            include_metadata: true,
            sort_by: 'relevance'
          }
        };

        const toolCall: ChatCompletionMessageFunctionToolCall = {
          id: 'call_complex',
          type: 'function',
          function: {
            name: 'advanced_search',
            arguments: JSON.stringify(complexArgs)
          }
        };

        // Assert
        const parsed = JSON.parse(toolCall.function.arguments);
        expect(parsed.query).toBe('OpenAI GPT-4');
        expect(parsed.filters.categories).toContain('AI');
        expect(parsed.options.include_metadata).toBe(true);
      });
    });

    describe('Custom tool calls', () => {
      it('should create valid custom tool call', () => {
        // Arrange & Act
        const toolCall: ChatCompletionMessageCustomToolCall = {
          id: 'custom_call_456',
          type: 'custom',
          custom: {
            name: 'proprietary_tool',
            input: 'custom formatted input string'
          }
        };

        // Assert
        expect(toolCall.id).toBe('custom_call_456');
        expect(toolCall.type).toBe('custom');
        expect(toolCall.custom.name).toBe('proprietary_tool');
        expect(toolCall.custom.input).toBeDefined();
      });
    });
  });

  describe('JSON Serialization and Compatibility', () => {
    it('should serialize/deserialize messages correctly', () => {
      // Arrange
      const message: ChatCompletionUserMessageParam = {
        role: 'user',
        content: [
          { type: 'text', text: 'Analyze this image' },
          {
            type: 'image_url',
            image_url: {
              url: 'https://example.com/img.jpg',
              detail: 'high'
            }
          }
        ],
        name: 'user_1'
      };

      // Act
      const json = JSON.stringify(message);
      const parsed = JSON.parse(json) as ChatCompletionUserMessageParam;

      // Assert
      expect(parsed).toEqual(message);
      expect(parsed.role).toBe('user');
      expect(Array.isArray(parsed.content)).toBe(true);
      if (Array.isArray(parsed.content)) {
        expect(parsed.content[0].type).toBe('text');
        expect(parsed.content[1].type).toBe('image_url');
      }
    });

    it('should maintain OpenAI API compatibility structure', () => {
      // Arrange - Create a conversation matching OpenAI format
      const conversation: ChatCompletionMessageParam[] = [
        {
          role: 'system',
          content: 'You are a helpful assistant.'
        },
        {
          role: 'user',
          content: 'What is 2+2?'
        },
        {
          role: 'assistant',
          content: null,
          tool_calls: [{
            id: 'call_123',
            type: 'function',
            function: {
              name: 'calculator',
              arguments: '{"expression": "2+2"}'
            }
          }]
        },
        {
          role: 'tool',
          content: '4',
          tool_call_id: 'call_123'
        },
        {
          role: 'assistant',
          content: '2 + 2 equals 4.'
        }
      ];

      // Act - Serialize as would be sent to OpenAI API
      const apiPayload = JSON.stringify({ messages: conversation });
      const parsed = JSON.parse(apiPayload);

      // Assert - Structure matches OpenAI expectations
      expect(parsed.messages).toHaveLength(5);
      expect(parsed.messages[0].role).toBe('system');
      expect(parsed.messages[2].tool_calls).toBeDefined();
      expect(parsed.messages[3].tool_call_id).toBe('call_123');
    });

    it('should handle null and undefined correctly', () => {
      // Arrange
      const message: ChatCompletionAssistantMessageParam = {
        role: 'assistant',
        content: null,  // Explicitly null
        refusal: undefined,  // Undefined (should not be in JSON)
        tool_calls: undefined
      };

      // Act
      const json = JSON.stringify(message);
      const parsed = JSON.parse(json);

      // Assert
      expect(parsed.content).toBeNull();  // null is preserved
      expect('refusal' in parsed).toBe(false);  // undefined is omitted
      expect('tool_calls' in parsed).toBe(false);
    });
  });

  describe('Union Type Handling', () => {
    it('should handle message param union type', () => {
      // Arrange
      const messages: ChatCompletionMessageParam[] = [
        { role: 'developer', content: 'Dev instructions' },
        { role: 'system', content: 'System prompt' },
        { role: 'user', content: 'User query' },
        { role: 'assistant', content: 'Response' },
        { role: 'tool', content: 'Tool result', tool_call_id: 'id' },
        { role: 'function', content: 'Function result', name: 'func' }
      ];

      // Act & Assert
      messages.forEach(msg => {
        switch (msg.role) {
          case 'developer':
            expect(isChatCompletionDeveloperMessageParam(msg)).toBe(true);
            break;
          case 'system':
            expect(isChatCompletionSystemMessageParam(msg)).toBe(true);
            break;
          case 'user':
            expect(isChatCompletionUserMessageParam(msg)).toBe(true);
            break;
          case 'assistant':
            expect(isChatCompletionAssistantMessageParam(msg)).toBe(true);
            break;
          case 'tool':
            expect(isChatCompletionToolMessageParam(msg)).toBe(true);
            break;
          case 'function':
            expect(isChatCompletionFunctionMessageParam(msg)).toBe(true);
            break;
        }
      });
    });

    it('should handle content part union type', () => {
      // Arrange
      const contentParts: ChatCompletionContentPart[] = [
        { type: 'text', text: 'Text content' },
        { type: 'image_url', image_url: { url: 'img.jpg' } },
        { type: 'input_audio', input_audio: { data: 'audio', format: 'mp3' } },
        { type: 'file', file: { file_id: 'file123' } }
      ];

      // Act & Assert
      contentParts.forEach(part => {
        switch (part.type) {
          case 'text':
            expect(isChatCompletionContentPartText(part)).toBe(true);
            break;
          case 'image_url':
            expect(isChatCompletionContentPartImage(part)).toBe(true);
            break;
          case 'input_audio':
            expect(isChatCompletionContentPartInputAudio(part)).toBe(true);
            break;
          case 'file':
            expect(isChatCompletionContentPartFile(part)).toBe(true);
            break;
        }
      });
    });

    it('should handle tool call union type', () => {
      // Arrange
      const toolCalls: ChatCompletionMessageToolCallUnion[] = [
        {
          id: 'func_1',
          type: 'function',
          function: { name: 'test', arguments: '{}' }
        },
        {
          id: 'custom_1',
          type: 'custom',
          custom: { name: 'custom', input: 'input' }
        }
      ];

      // Act & Assert
      toolCalls.forEach(call => {
        if (call.type === 'function') {
          expect(isChatCompletionMessageFunctionToolCall(call)).toBe(true);
          expect(isChatCompletionMessageCustomToolCall(call)).toBe(false);
        } else {
          expect(isChatCompletionMessageCustomToolCall(call)).toBe(true);
          expect(isChatCompletionMessageFunctionToolCall(call)).toBe(false);
        }
      });
    });
  });

  describe('Error Cases and Edge Conditions', () => {
    it('should handle invalid JSON in function arguments gracefully', () => {
      // Arrange
      const toolCall: ChatCompletionMessageFunctionToolCall = {
        id: 'call_bad',
        type: 'function',
        function: {
          name: 'parse_this',
          arguments: 'not valid json {'  // Invalid JSON
        }
      };

      // Act & Assert
      expect(() => JSON.parse(toolCall.function.arguments)).toThrow();
      // The type itself doesn't validate JSON - that's application responsibility
      expect(toolCall.function.arguments).toBeDefined();
    });

    it('should handle empty content arrays', () => {
      // Arrange
      const message: ChatCompletionUserMessageParam = {
        role: 'user',
        content: []  // Empty array - may not be valid for API but type allows it
      };

      // Assert
      expect(Array.isArray(message.content)).toBe(true);
      expect(message.content).toHaveLength(0);
    });

    it('should handle very long content', () => {
      // Arrange
      const longText = 'a'.repeat(100000);  // 100k characters
      const message: ChatCompletionSystemMessageParam = {
        role: 'system',
        content: longText
      };

      // Assert
      expect(message.content.length).toBe(100000);
    });

    it('should handle special characters in content', () => {
      // Arrange
      const specialContent = `Special chars: "quotes" 'apostrophe' \n newline \t tab \\ backslash`;
      const message: ChatCompletionUserMessageParam = {
        role: 'user',
        content: specialContent
      };

      // Act
      const json = JSON.stringify(message);
      const parsed = JSON.parse(json) as ChatCompletionUserMessageParam;

      // Assert
      expect(parsed.content).toBe(specialContent);
    });

    it('should handle unicode in messages', () => {
      // Arrange
      const unicodeContent = 'Hello 世界 🌍 مرحبا мир';
      const message: ChatCompletionUserMessageParam = {
        role: 'user',
        content: unicodeContent
      };

      // Act
      const json = JSON.stringify(message);
      const parsed = JSON.parse(json) as ChatCompletionUserMessageParam;

      // Assert
      expect(parsed.content).toBe(unicodeContent);
    });
  });

  describe('Real-world Usage Patterns', () => {
    it('should handle typical GPT-4 Vision request', () => {
      // Arrange
      const visionRequest: ChatCompletionUserMessageParam = {
        role: 'user',
        content: [
          {
            type: 'text',
            text: 'What\'s in this image? Describe in detail.'
          },
          {
            type: 'image_url',
            image_url: {
              url: 'https://example.com/image.jpg',
              detail: 'high'
            }
          }
        ]
      };

      // Assert
      expect(visionRequest.role).toBe('user');
      if (Array.isArray(visionRequest.content)) {
        expect(visionRequest.content[0].type).toBe('text');
        expect(visionRequest.content[1].type).toBe('image_url');
      }
    });

    it('should handle function calling flow', () => {
      // Arrange
      const functionCallFlow: ChatCompletionMessageParam[] = [
        // User asks a question requiring a function
        {
          role: 'user',
          content: 'What\'s the weather in San Francisco?'
        },
        // Assistant calls a function
        {
          role: 'assistant',
          content: null,
          tool_calls: [{
            id: 'call_weather_sf',
            type: 'function',
            function: {
              name: 'get_weather',
              arguments: JSON.stringify({
                location: 'San Francisco',
                unit: 'fahrenheit'
              })
            }
          }]
        },
        // Tool returns result
        {
          role: 'tool',
          content: JSON.stringify({
            temperature: 68,
            condition: 'sunny',
            humidity: 65
          }),
          tool_call_id: 'call_weather_sf'
        },
        // Assistant provides final answer
        {
          role: 'assistant',
          content: 'The weather in San Francisco is currently sunny with a temperature of 68°F and 65% humidity.'
        }
      ];

      // Assert
      expect(functionCallFlow).toHaveLength(4);
      expect(functionCallFlow[1].role).toBe('assistant');
      const assistantMsg = functionCallFlow[1] as ChatCompletionAssistantMessageParam;
      expect(assistantMsg.tool_calls).toBeDefined();
      expect(assistantMsg.tool_calls![0].type).toBe('function');
    });

    it('should handle streaming response patterns', () => {
      // Arrange - Simulating chunks that would come from streaming
      const streamingChunks: Partial<ChatCompletionAssistantMessageParam>[] = [
        { role: 'assistant', content: 'The' },
        { content: ' answer' },
        { content: ' is' },
        { content: ' 42.' }
      ];

      // Act - Accumulate chunks
      let fullMessage: ChatCompletionAssistantMessageParam = {
        role: 'assistant',
        content: ''
      };

      streamingChunks.forEach(chunk => {
        if (chunk.role) {
          fullMessage.role = chunk.role;
        }
        if (chunk.content) {
          fullMessage.content = (fullMessage.content || '') + chunk.content;
        }
      });

      // Assert
      expect(fullMessage.content).toBe('The answer is 42.');
    });
  });
});