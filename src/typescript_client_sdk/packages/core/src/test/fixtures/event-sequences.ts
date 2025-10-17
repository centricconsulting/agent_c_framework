/**
 * Event Sequence Fixtures for Testing
 * Realistic event sequences that match actual server behavior
 * 
 * These fixtures provide comprehensive test scenarios for the EventStreamProcessor
 * covering all major event types and interaction patterns.
 */

import type { ServerEvent } from '../../events/types/ServerEvents';

/**
 * Standard initialization sequence sent by server on connection
 * This is what a client receives when establishing a new session
 */
export const initializationSequence: ServerEvent[] = [
  {
    type: 'system_prompt',
    session_id: 'test-session',
    role: 'system',
    content: '# Agent System Prompt\n\nYou are a helpful AI assistant.',
    format: 'markdown'
  },
  {
    type: 'chat_user_data',
    user: {
      user_id: 'user-123',
      user_name: 'testuser',
      email: 'test@example.com',
      first_name: 'Test',
      last_name: 'User',
      is_active: true,
      roles: ['user'],
      groups: ['general'],
      created_at: new Date().toISOString(),
      last_login: new Date().toISOString()
    }
  },
  {
    type: 'voice_list',
    voices: [
      {
        voice_id: 'nova',
        vendor: 'openai',
        description: 'Friendly voice',
        output_format: 'pcm16'
      },
      {
        voice_id: 'alloy',
        vendor: 'openai',
        description: 'Neutral voice',
        output_format: 'pcm16'
      }
    ]
  },
  {
    type: 'agent_voice_changed',
    voice: {
      voice_id: 'nova',
      vendor: 'openai',
      description: 'Friendly voice',
      output_format: 'pcm16'
    }
  },
  {
    type: 'agent_list',
    agents: [
      {
        name: 'Default Agent',
        key: 'default',
        agent_description: 'General purpose assistant',
        category: ['general'],
        tools: ['think', 'web_search']
      }
    ]
  },
  {
    type: 'tool_catalog',
    tools: [
      {
        name: 'general_tools',
        description: 'General purpose tools',
        schemas: {
          think: {
            type: 'function',
            function: {
              name: 'think',
              description: 'Internal reasoning',
              parameters: {
                type: 'object',
                properties: {
                  thought: { type: 'string' }
                },
                required: ['thought']
              }
            }
          }
        }
      }
    ]
  },
  {
    type: 'voice_input_supported',
    modes: ['ptt', 'vad']
  },
  {
    type: 'user_turn_start'
  }
];

/**
 * Text streaming sequence with proper deltas and completion
 * Shows how assistant responses are streamed in chunks
 */
export const textStreamingSequence: ServerEvent[] = [
  {
    type: 'user_turn_end'
  },
  {
    type: 'interaction',
    session_id: 'test-session',
    role: 'assistant',
    started: true,
    id: 'int_1'
  },
  {
    type: 'text_delta',
    session_id: 'test-session',
    role: 'assistant',
    content: 'Hello',
    format: 'text'
  },
  {
    type: 'text_delta',
    session_id: 'test-session',
    role: 'assistant',
    content: ' there',
    format: 'text'
  },
  {
    type: 'text_delta',
    session_id: 'test-session',
    role: 'assistant',
    content: '!',
    format: 'text'
  },
  {
    type: 'completion',
    session_id: 'test-session',
    role: 'assistant',
    running: false,
    completion_options: {
      model: 'claude-3-5-sonnet',
      max_tokens: 4096,
      temperature: 0.7
    },
    stop_reason: 'end_turn',
    input_tokens: 10,
    output_tokens: 3
  },
  {
    type: 'history_delta',
    session_id: 'test-session',
    role: 'assistant',
    messages: [
      {
        role: 'assistant',
        content: 'Hello there!',
        format: 'text',
        timestamp: new Date().toISOString()
      }
    ]
  },
  {
    type: 'interaction',
    session_id: 'test-session',
    role: 'assistant',
    started: false,
    id: 'int_1'
  },
  {
    type: 'user_turn_start'
  }
];

/**
 * Thought streaming sequence (role: 'assistant (thought)')
 * Shows internal reasoning process before responding
 */
export const thoughtStreamingSequence: ServerEvent[] = [
  {
    type: 'user_turn_end'
  },
  {
    type: 'interaction',
    session_id: 'test-session',
    role: 'assistant',
    started: true,
    id: 'int_2'
  },
  {
    type: 'tool_select_delta',
    session_id: 'test-session',
    role: 'assistant',
    tool_calls: [
      {
        id: 'think_1',
        type: 'tool_use',
        name: 'think',
        input: {}
      }
    ]
  },
  {
    type: 'thought_delta',
    session_id: 'test-session',
    role: 'assistant (thought)',
    content: 'I need to',
    format: 'text'
  },
  {
    type: 'thought_delta',
    session_id: 'test-session',
    role: 'assistant (thought)',
    content: ' analyze this',
    format: 'text'
  },
  {
    type: 'thought_delta',
    session_id: 'test-session',
    role: 'assistant (thought)',
    content: ' carefully.',
    format: 'text'
  },
  {
    type: 'tool_call',
    session_id: 'test-session',
    role: 'assistant',
    active: false,
    vendor: 'anthropic',
    tool_calls: [
      {
        id: 'think_1',
        type: 'tool_use',
        name: 'think',
        input: { thought: 'I need to analyze this carefully.' }
      }
    ],
    tool_results: [
      {
        type: 'tool_result',
        tool_use_id: 'think_1',
        content: ''
      }
    ]
  },
  {
    type: 'text_delta',
    session_id: 'test-session',
    role: 'assistant',
    content: 'Based on my analysis',
    format: 'text'
  },
  {
    type: 'text_delta',
    session_id: 'test-session',
    role: 'assistant',
    content: ', here is my response.',
    format: 'text'
  },
  {
    type: 'completion',
    session_id: 'test-session',
    role: 'assistant',
    running: false,
    completion_options: {
      model: 'claude-3-5-sonnet',
      max_tokens: 4096,
      temperature: 0.7
    },
    stop_reason: 'end_turn',
    input_tokens: 20,
    output_tokens: 15
  },
  {
    type: 'history_delta',
    session_id: 'test-session',
    role: 'assistant',
    messages: [
      {
        role: 'assistant',
        content: 'Based on my analysis, here is my response.',
        format: 'text',
        timestamp: new Date().toISOString()
      }
    ]
  },
  {
    type: 'interaction',
    session_id: 'test-session',
    role: 'assistant',
    started: false,
    id: 'int_2'
  },
  {
    type: 'user_turn_start'
  }
];

/**
 * Tool call sequence with proper events
 * Shows tool selection, execution, and result handling
 */
export const toolCallSequence: ServerEvent[] = [
  {
    type: 'user_turn_end'
  },
  {
    type: 'interaction',
    session_id: 'test-session',
    role: 'assistant',
    started: true,
    id: 'int_5'
  },
  {
    type: 'text_delta',
    session_id: 'test-session',
    role: 'assistant',
    content: 'Let me calculate that for you.',
    format: 'text'
  },
  {
    type: 'completion',
    session_id: 'test-session',
    role: 'assistant',
    running: true,
    completion_options: {
      model: 'claude-3-5-sonnet',
      max_tokens: 4096,
      temperature: 0.7
    }
  },
  {
    type: 'tool_select_delta',
    session_id: 'test-session',
    role: 'assistant',
    tool_calls: [
      {
        id: 'call_calc_1',
        type: 'tool_use',
        name: 'calculator',
        input: { operation: 'add', a: 5, b: 3 }
      }
    ]
  },
  {
    type: 'tool_call',
    session_id: 'test-session',
    role: 'assistant',
    active: true,
    vendor: 'anthropic',
    tool_calls: [
      {
        id: 'call_calc_1',
        type: 'tool_use',
        name: 'calculator',
        input: { operation: 'add', a: 5, b: 3 }
      }
    ]
  },
  {
    type: 'tool_call',
    session_id: 'test-session',
    role: 'assistant',
    active: false,
    vendor: 'anthropic',
    tool_calls: [
      {
        id: 'call_calc_1',
        type: 'tool_use',
        name: 'calculator',
        input: { operation: 'add', a: 5, b: 3 }
      }
    ],
    tool_results: [
      {
        type: 'tool_result',
        tool_use_id: 'call_calc_1',
        content: '{"result": 8}'
      }
    ]
  },
  {
    type: 'text_delta',
    session_id: 'test-session',
    role: 'assistant',
    content: ' The result is 8.',
    format: 'text'
  },
  {
    type: 'completion',
    session_id: 'test-session',
    role: 'assistant',
    running: false,
    completion_options: {
      model: 'claude-3-5-sonnet',
      max_tokens: 4096,
      temperature: 0.7
    },
    stop_reason: 'end_turn',
    input_tokens: 25,
    output_tokens: 12
  },
  {
    type: 'history_delta',
    session_id: 'test-session',
    role: 'assistant',
    messages: [
      {
        role: 'assistant',
        content: 'Let me calculate that for you. The result is 8.',
        format: 'text',
        timestamp: new Date().toISOString()
      }
    ]
  },
  {
    type: 'interaction',
    session_id: 'test-session',
    role: 'assistant',
    started: false,
    id: 'int_5'
  },
  {
    type: 'user_turn_start'
  }
];

/**
 * Multiple tool calls in sequence
 * Shows handling of multiple tools in a single response
 */
export const multiToolCallSequence: ServerEvent[] = [
  {
    type: 'user_turn_end'
  },
  {
    type: 'interaction',
    session_id: 'test-session',
    role: 'assistant',
    started: true,
    id: 'int_6'
  },
  {
    type: 'text_delta',
    session_id: 'test-session',
    role: 'assistant',
    content: "I'll search for that information and calculate the result.",
    format: 'text'
  },
  {
    type: 'tool_select_delta',
    session_id: 'test-session',
    role: 'assistant',
    tool_calls: [
      {
        id: 'call_search_1',
        type: 'tool_use',
        name: 'web_search',
        input: { query: 'current exchange rate USD to EUR' }
      }
    ]
  },
  {
    type: 'tool_call',
    session_id: 'test-session',
    role: 'assistant',
    active: true,
    vendor: 'anthropic',
    tool_calls: [
      {
        id: 'call_search_1',
        type: 'tool_use',
        name: 'web_search',
        input: { query: 'current exchange rate USD to EUR' }
      }
    ]
  },
  {
    type: 'tool_call',
    session_id: 'test-session',
    role: 'assistant',
    active: false,
    vendor: 'anthropic',
    tool_calls: [
      {
        id: 'call_search_1',
        type: 'tool_use',
        name: 'web_search',
        input: { query: 'current exchange rate USD to EUR' }
      }
    ],
    tool_results: [
      {
        type: 'tool_result',
        tool_use_id: 'call_search_1',
        content: '{"rate": 0.92, "source": "example.com"}'
      }
    ]
  },
  {
    type: 'tool_select_delta',
    session_id: 'test-session',
    role: 'assistant',
    tool_calls: [
      {
        id: 'call_calc_2',
        type: 'tool_use',
        name: 'calculator',
        input: { operation: 'multiply', a: 100, b: 0.92 }
      }
    ]
  },
  {
    type: 'tool_call',
    session_id: 'test-session',
    role: 'assistant',
    active: true,
    vendor: 'anthropic',
    tool_calls: [
      {
        id: 'call_calc_2',
        type: 'tool_use',
        name: 'calculator',
        input: { operation: 'multiply', a: 100, b: 0.92 }
      }
    ]
  },
  {
    type: 'tool_call',
    session_id: 'test-session',
    role: 'assistant',
    active: false,
    vendor: 'anthropic',
    tool_calls: [
      {
        id: 'call_calc_2',
        type: 'tool_use',
        name: 'calculator',
        input: { operation: 'multiply', a: 100, b: 0.92 }
      }
    ],
    tool_results: [
      {
        type: 'tool_result',
        tool_use_id: 'call_calc_2',
        content: '{"result": 92}'
      }
    ]
  },
  {
    type: 'text_delta',
    session_id: 'test-session',
    role: 'assistant',
    content: ' Based on the current exchange rate of 0.92, $100 equals €92.',
    format: 'text'
  },
  {
    type: 'completion',
    session_id: 'test-session',
    role: 'assistant',
    running: false,
    completion_options: {
      model: 'claude-3-5-sonnet',
      max_tokens: 4096,
      temperature: 0.7
    },
    stop_reason: 'end_turn',
    input_tokens: 40,
    output_tokens: 25
  },
  {
    type: 'interaction',
    session_id: 'test-session',
    role: 'assistant',
    started: false,
    id: 'int_6'
  },
  {
    type: 'user_turn_start'
  }
];

/**
 * ChatSessionChanged event with resumed messages
 * Shows session restoration with existing message history
 */
export const chatSessionChangedSequence: ServerEvent[] = [
  {
    type: 'chat_session_changed',
    chat_session: {
      version: 2,
      session_id: 'resumed-session',
      session_name: 'Resumed Chat',
      messages: [
        {
          role: 'user',
          content: [
            { type: 'text', text: 'What is TypeScript?' }
          ]
        },
        {
          role: 'assistant',
          content: [
            { type: 'text', text: 'TypeScript is a typed superset of JavaScript that compiles to plain JavaScript.' }
          ]
        },
        {
          role: 'assistant',
          content: [
            {
              type: 'tool_use',
              id: 'think-prev',
              name: 'think',
              input: { thought: 'The user is asking about TypeScript, I should provide a clear explanation.' }
            }
          ]
        },
        {
          role: 'user',
          content: [
            {
              type: 'tool_result',
              tool_use_id: 'think-prev',
              content: ''
            }
          ]
        },
        {
          role: 'user',
          content: [
            { type: 'text', text: 'Can you show me an example?' }
          ]
        },
        {
          role: 'assistant',
          content: [
            { 
              type: 'text', 
              text: 'Here\'s a simple TypeScript example:\n\n```typescript\ninterface Person {\n  name: string;\n  age: number;\n}\n\nfunction greet(person: Person): string {\n  return `Hello, ${person.name}!`;\n}\n```' 
            }
          ]
        }
      ],
      created_at: new Date(Date.now() - 3600000).toISOString(),
      updated_at: new Date(Date.now() - 1700000).toISOString(),
      token_count: 256,
      context_window_size: 128000,
      user_id: 'user-123',
      metadata: {
        topic: 'programming',
        tags: ['typescript', 'javascript']
      },
      vendor: 'anthropic',
      display_name: 'TypeScript Discussion'
    }
  },
  {
    type: 'user_turn_start'
  }
];

/**
 * RenderMedia event sequence
 * Shows rendering of rich media content
 */
export const renderMediaSequence: ServerEvent[] = [
  {
    type: 'user_turn_end'
  },
  {
    type: 'interaction',
    session_id: 'test-session',
    role: 'assistant',
    started: true,
    id: 'int_7'
  },
  {
    type: 'text_delta',
    session_id: 'test-session',
    role: 'assistant',
    content: 'Here is a visualization of the data:',
    format: 'text'
  },
  {
    type: 'completion',
    session_id: 'test-session',
    role: 'assistant',
    running: true,
    completion_options: {
      model: 'claude-3-5-sonnet',
      max_tokens: 4096,
      temperature: 0.7
    }
  },
  {
    type: 'render_media',
    session_id: 'test-session',
    role: 'assistant',
    content_type: 'text/markdown',
    content: '# Data Analysis\n\n## Key Findings\n\n- **Growth Rate**: 25%\n- **User Engagement**: High\n- **Conversion**: 3.2%\n\n### Chart\n\n```mermaid\ngraph TD\n    A[Start] --> B{Decision}\n    B -->|Yes| C[Success]\n    B -->|No| D[Try Again]\n```',
    foreign_content: false,
    name: 'Analysis Results'
  },
  {
    type: 'render_media',
    session_id: 'test-session',
    role: 'assistant',
    content_type: 'text/html',
    content: '<div class="chart"><canvas id="myChart"></canvas></div>',
    foreign_content: false,
    name: 'Interactive Chart',
    sent_by_class: 'DataVisualizationTool',
    sent_by_function: 'generateChart'
  },
  {
    type: 'text_delta',
    session_id: 'test-session',
    role: 'assistant',
    content: ' The data shows positive trends across all metrics.',
    format: 'text'
  },
  {
    type: 'completion',
    session_id: 'test-session',
    role: 'assistant',
    running: false,
    completion_options: {
      model: 'claude-3-5-sonnet',
      max_tokens: 4096,
      temperature: 0.7
    },
    stop_reason: 'end_turn',
    input_tokens: 30,
    output_tokens: 18
  },
  {
    type: 'interaction',
    session_id: 'test-session',
    role: 'assistant',
    started: false,
    id: 'int_7'
  },
  {
    type: 'user_turn_start'
  }
];

/**
 * Foreign content media sequence
 * Shows handling of external/untrusted content
 */
export const foreignMediaSequence: ServerEvent[] = [
  {
    type: 'user_turn_end'
  },
  {
    type: 'interaction',
    session_id: 'test-session',
    role: 'assistant',
    started: true,
    id: 'int_8'
  },
  {
    type: 'text_delta',
    session_id: 'test-session',
    role: 'assistant',
    content: "I've retrieved the requested webpage content:",
    format: 'text'
  },
  {
    type: 'render_media',
    session_id: 'test-session',
    role: 'assistant',
    content_type: 'text/html',
    content: '<iframe src="https://example.com/article"></iframe>',
    foreign_content: true, // SECURITY: External content - must be sandboxed
    url: 'https://example.com/article',
    name: 'External Article',
    sent_by_class: 'WebScraperTool',
    sent_by_function: 'fetchWebpage'
  },
  {
    type: 'text_delta',
    session_id: 'test-session',
    role: 'assistant',
    content: ' Please note this is external content.',
    format: 'text'
  },
  {
    type: 'completion',
    session_id: 'test-session',
    role: 'assistant',
    running: false,
    completion_options: {
      model: 'claude-3-5-sonnet',
      max_tokens: 4096,
      temperature: 0.7
    },
    stop_reason: 'end_turn',
    input_tokens: 20,
    output_tokens: 15
  },
  {
    type: 'interaction',
    session_id: 'test-session',
    role: 'assistant',
    started: false,
    id: 'int_8'
  },
  {
    type: 'user_turn_start'
  }
];

/**
 * Audio streaming sequence (voice mode)
 * Note: Audio data is sent as binary frames, not JSON events
 * This sequence shows the events around audio transmission
 */
export const audioStreamingSequence: ServerEvent[] = [
  {
    type: 'user_turn_start'
  },
  {
    type: 'server_listening'
  },
  // User speaks - audio input sent as binary frames
  {
    type: 'user_turn_end'
  },
  {
    type: 'user_request',
    session_id: 'test-session',
    role: 'user',
    data: {
      message: '[Transcribed from audio]: What is the weather today?'
    }
  },
  {
    type: 'interaction',
    session_id: 'test-session',
    role: 'assistant',
    started: true,
    id: 'int_audio_1'
  },
  {
    type: 'completion',
    session_id: 'test-session',
    role: 'assistant',
    running: true,
    completion_options: {
      model: 'gpt-4',
      max_tokens: 150,
      temperature: 0.7
    }
  },
  // Audio response is streamed as binary frames
  // Text transcription may also be provided
  {
    type: 'text_delta',
    session_id: 'test-session',
    role: 'assistant',
    content: "Today's weather is sunny with a high of 72°F.",
    format: 'text'
  },
  {
    type: 'completion',
    session_id: 'test-session',
    role: 'assistant',
    running: false,
    completion_options: {
      model: 'gpt-4',
      max_tokens: 150,
      temperature: 0.7
    },
    stop_reason: 'end_turn',
    input_tokens: 15,
    output_tokens: 12
  },
  {
    type: 'interaction',
    session_id: 'test-session',
    role: 'assistant',
    started: false,
    id: 'int_audio_1'
  },
  {
    type: 'user_turn_start'
  }
];

/**
 * Error event sequences
 * Shows various error scenarios
 */
export const errorSequence: ServerEvent[] = [
  {
    type: 'error',
    message: 'Rate limit exceeded. Please try again in 60 seconds.',
    source: 'api'
  },
  {
    type: 'system_message',
    session_id: 'test-session',
    role: 'system',
    content: 'Connection interrupted. Attempting to reconnect...',
    format: 'text',
    severity: 'warning'
  },
  {
    type: 'error',
    message: 'WebSocket connection failed',
    source: 'websocket'
  },
  {
    type: 'system_message',
    session_id: 'test-session',
    role: 'system',
    content: 'Reconnection successful',
    format: 'text',
    severity: 'info'
  }
];

/**
 * Cancellation sequence
 * Shows how the server responds to cancellation requests
 */
export const cancellationSequence: ServerEvent[] = [
  {
    type: 'user_turn_end'
  },
  {
    type: 'interaction',
    session_id: 'test-session',
    role: 'assistant',
    started: true,
    id: 'int_9'
  },
  {
    type: 'text_delta',
    session_id: 'test-session',
    role: 'assistant',
    content: 'Let me help you with that. First, I need to',
    format: 'text'
  },
  {
    type: 'text_delta',
    session_id: 'test-session',
    role: 'assistant',
    content: ' analyze the data and then',
    format: 'text'
  },
  // User cancels the response
  {
    type: 'cancelled'
  },
  {
    type: 'system_message',
    session_id: 'test-session',
    role: 'system',
    content: 'Response cancelled by user',
    format: 'text',
    severity: 'info'
  },
  {
    type: 'interaction',
    session_id: 'test-session',
    role: 'assistant',
    started: false,
    id: 'int_9'
  },
  {
    type: 'user_turn_start'
  }
];

/**
 * Subsession sequence
 * Shows nested agent interactions (e.g., agent calling another agent)
 */
export const subsessionSequence: ServerEvent[] = [
  {
    type: 'user_turn_end'
  },
  {
    type: 'interaction',
    session_id: 'test-session',
    role: 'assistant',
    started: true,
    id: 'int_10'
  },
  {
    type: 'text_delta',
    session_id: 'test-session',
    role: 'assistant',
    content: "I'll consult with a specialist for this.",
    format: 'text'
  },
  {
    type: 'subsession_started',
    session_id: 'sub-session-1',
    role: 'assistant',
    sub_session_type: 'oneshot',
    sub_agent_type: 'team',
    prime_agent_key: 'default',
    sub_agent_key: 'specialist',
    parent_session_id: 'test-session',
    user_session_id: 'test-session'
  },
  // Events from the subsession would appear here
  {
    type: 'text_delta',
    session_id: 'sub-session-1',
    role: 'assistant',
    content: '[Specialist]: Based on my analysis...',
    format: 'text',
    parent_session_id: 'test-session',
    user_session_id: 'test-session'
  },
  {
    type: 'subsession_ended',
    session_id: 'sub-session-1',
    role: 'assistant',
    parent_session_id: 'test-session',
    user_session_id: 'test-session'
  },
  {
    type: 'text_delta',
    session_id: 'test-session',
    role: 'assistant',
    content: ' The specialist confirms that the answer is correct.',
    format: 'text'
  },
  {
    type: 'completion',
    session_id: 'test-session',
    role: 'assistant',
    running: false,
    completion_options: {
      model: 'claude-3-5-sonnet',
      max_tokens: 4096,
      temperature: 0.7
    },
    stop_reason: 'end_turn',
    input_tokens: 50,
    output_tokens: 35
  },
  {
    type: 'interaction',
    session_id: 'test-session',
    role: 'assistant',
    started: false,
    id: 'int_10'
  },
  {
    type: 'user_turn_start'
  }
];

/**
 * Complex mixed interaction sequence
 * Shows a realistic conversation with multiple event types
 */
export const complexInteractionSequence: ServerEvent[] = [
  // User asks a complex question
  {
    type: 'user_turn_end'
  },
  {
    type: 'user_request',
    session_id: 'test-session',
    role: 'user',
    data: {
      message: 'Can you analyze this data and create a visualization?'
    }
  },
  {
    type: 'interaction',
    session_id: 'test-session',
    role: 'assistant',
    started: true,
    id: 'int_complex_1'
  },
  // Assistant thinks
  {
    type: 'tool_select_delta',
    session_id: 'test-session',
    role: 'assistant',
    tool_calls: [
      {
        id: 'think_complex_1',
        type: 'tool_use',
        name: 'think',
        input: {}
      }
    ]
  },
  {
    type: 'thought_delta',
    session_id: 'test-session',
    role: 'assistant (thought)',
    content: 'I need to analyze the data first, then create an appropriate visualization.',
    format: 'text'
  },
  // Assistant responds
  {
    type: 'text_delta',
    session_id: 'test-session',
    role: 'assistant',
    content: "I'll analyze your data and create a visualization.",
    format: 'text'
  },
  // Calls data analysis tool
  {
    type: 'tool_select_delta',
    session_id: 'test-session',
    role: 'assistant',
    tool_calls: [
      {
        id: 'analyze_1',
        type: 'tool_use',
        name: 'data_analyzer',
        input: { data: '[user data]' }
      }
    ]
  },
  {
    type: 'tool_call',
    session_id: 'test-session',
    role: 'assistant',
    active: true,
    vendor: 'anthropic',
    tool_calls: [
      {
        id: 'analyze_1',
        type: 'tool_use',
        name: 'data_analyzer',
        input: { data: '[user data]' }
      }
    ]
  },
  {
    type: 'tool_call',
    session_id: 'test-session',
    role: 'assistant',
    active: false,
    vendor: 'anthropic',
    tool_calls: [
      {
        id: 'analyze_1',
        type: 'tool_use',
        name: 'data_analyzer',
        input: { data: '[user data]' }
      }
    ],
    tool_results: [
      {
        type: 'tool_result',
        tool_use_id: 'analyze_1',
        content: '{"mean": 45.2, "median": 42, "std": 8.7}'
      }
    ]
  },
  // Creates visualization
  {
    type: 'text_delta',
    session_id: 'test-session',
    role: 'assistant',
    content: ' Here are the results:',
    format: 'text'
  },
  {
    type: 'render_media',
    session_id: 'test-session',
    role: 'assistant',
    content_type: 'text/html',
    content: '<div id="chart"><!-- Chart HTML --></div>',
    foreign_content: false,
    name: 'Data Visualization',
    sent_by_class: 'ChartTool',
    sent_by_function: 'createChart'
  },
  {
    type: 'text_delta',
    session_id: 'test-session',
    role: 'assistant',
    content: '\n\nThe analysis shows a mean of 45.2 with a standard deviation of 8.7.',
    format: 'text'
  },
  {
    type: 'completion',
    session_id: 'test-session',
    role: 'assistant',
    running: false,
    completion_options: {
      model: 'claude-3-5-sonnet',
      max_tokens: 4096,
      temperature: 0.7
    },
    stop_reason: 'end_turn',
    input_tokens: 75,
    output_tokens: 52
  },
  {
    type: 'history_delta',
    session_id: 'test-session',
    role: 'assistant',
    messages: [
      {
        role: 'assistant',
        content: "I'll analyze your data and create a visualization. Here are the results:\n\nThe analysis shows a mean of 45.2 with a standard deviation of 8.7.",
        format: 'text',
        timestamp: new Date().toISOString()
      }
    ]
  },
  {
    type: 'interaction',
    session_id: 'test-session',
    role: 'assistant',
    started: false,
    id: 'int_complex_1'
  },
  {
    type: 'user_turn_start'
  }
];

/**
 * Session metadata update sequence
 * Shows how session metadata and name can be updated
 */
export const sessionUpdateSequence: ServerEvent[] = [
  {
    type: 'chat_session_name_changed',
    session_name: 'Data Analysis Session',
    session_id: 'test-session'
  },
  {
    type: 'session_metadata_changed',
    meta: {
      topic: 'data-analysis',
      priority: 'high',
      tags: ['visualization', 'statistics'],
      last_activity: new Date().toISOString()
    }
  },
  {
    type: 'system_message',
    session_id: 'test-session',
    role: 'system',
    content: 'Session metadata updated',
    format: 'text',
    severity: 'info'
  }
];

/**
 * Agent and voice configuration sequence
 * Shows agent switching and voice changes
 */
export const agentConfigSequence: ServerEvent[] = [
  {
    type: 'agent_list',
    agents: [
      {
        name: 'Default Agent',
        key: 'default',
        agent_description: 'General purpose assistant',
        category: ['general'],
        tools: ['think', 'web_search']
      },
      {
        name: 'Code Expert',
        key: 'code-expert',
        agent_description: 'Programming specialist',
        category: ['technical', 'programming'],
        tools: ['code_interpreter', 'debugger']
      }
    ]
  },
  {
    type: 'agent_configuration_changed',
    agent_config: {
      version: 2,
      name: 'Code Expert',
      key: 'code-expert',
      model_id: 'claude-3-5-sonnet-20241022',
      agent_description: 'Programming specialist',
      tools: ['code_interpreter', 'debugger'],
      agent_params: {
        type: 'claude_reasoning',
        model_name: 'claude-3-5-sonnet-20241022',
        max_tokens: 64000,
        temperature: 0.3
      },
      persona: 'You are an expert programmer.',
      category: ['technical', 'programming']
    }
  },
  {
    type: 'voice_list',
    voices: [
      {
        voice_id: 'nova',
        vendor: 'openai',
        description: 'Friendly voice',
        output_format: 'pcm16'
      },
      {
        voice_id: 'echo',
        vendor: 'openai',
        description: 'Professional voice',
        output_format: 'pcm16'
      }
    ]
  },
  {
    type: 'agent_voice_changed',
    voice: {
      voice_id: 'echo',
      vendor: 'openai',
      description: 'Professional voice',
      output_format: 'pcm16'
    }
  },
  {
    type: 'system_message',
    session_id: 'test-session',
    role: 'system',
    content: 'Switched to Code Expert agent with Echo voice',
    format: 'text',
    severity: 'info'
  }
];

/**
 * Ping/Pong health check sequence
 * Shows connection health monitoring
 */
export const healthCheckSequence: ServerEvent[] = [
  {
    type: 'pong'
  },
  {
    type: 'system_message',
    session_id: 'test-session',
    role: 'system',
    content: 'Connection healthy - latency: 45ms',
    format: 'text',
    severity: 'info'
  }
];

/**
 * Helper function to create a custom event sequence
 * Useful for generating test scenarios programmatically
 */
export function createCustomSequence(events: Partial<ServerEvent>[]): ServerEvent[] {
  return events.map(event => ({
    ...event,
    // Add any default values if needed
  } as ServerEvent));
}

/**
 * Export all sequences as a collection for easy iteration in tests
 */
export const allEventSequences = {
  initialization: initializationSequence,
  textStreaming: textStreamingSequence,
  thoughtStreaming: thoughtStreamingSequence,
  toolCall: toolCallSequence,
  multiToolCall: multiToolCallSequence,
  chatSessionChanged: chatSessionChangedSequence,
  renderMedia: renderMediaSequence,
  foreignMedia: foreignMediaSequence,
  audioStreaming: audioStreamingSequence,
  error: errorSequence,
  cancellation: cancellationSequence,
  subsession: subsessionSequence,
  complexInteraction: complexInteractionSequence,
  sessionUpdate: sessionUpdateSequence,
  agentConfig: agentConfigSequence,
  healthCheck: healthCheckSequence
};

/**
 * Test helper: Get a sequence by name
 */
export function getEventSequence(name: keyof typeof allEventSequences): ServerEvent[] {
  return allEventSequences[name];
}

/**
 * Test helper: Combine multiple sequences
 */
export function combineSequences(...sequences: ServerEvent[][]): ServerEvent[] {
  return sequences.flat();
}

/**
 * Test helper: Add delays between events for timing tests
 */
export function withDelays(sequence: ServerEvent[], delayMs: number): Array<ServerEvent | (() => Promise<void>)> {
  const result: Array<ServerEvent | (() => Promise<void>)> = [];
  sequence.forEach((event, index) => {
    result.push(event);
    if (index < sequence.length - 1) {
      result.push(() => new Promise(resolve => setTimeout(resolve, delayMs)));
    }
  });
  return result;
}