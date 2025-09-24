/**
 * Test session types for loading and managing test chat sessions
 */

/**
 * Content block types based on vendor-specific models
 */
export type TextBlockParam = {
  type: 'text';
  text: string;
  citations?: any;
};

export type ImageBlockParam = {
  type: 'image';
  source: {
    type: 'base64';
    media_type: string;
    data: string;
  };
};

export type DocumentBlockParam = {
  type: 'document';
  source: {
    type: 'base64';
    media_type: string;
    data: string;
  };
};

export type ThinkingBlockParam = {
  type: 'thinking';
  text: string;
};

export type ToolUseBlockParam = {
  type: 'tool_use';
  id: string;
  name: string;
  input: Record<string, any>;
};

export type ToolResultBlockParam = {
  type: 'tool_result';
  tool_use_id: string;
  content: string | any;
};

export type ContentBlock = 
  | TextBlockParam 
  | ImageBlockParam 
  | DocumentBlockParam 
  | ThinkingBlockParam 
  | ToolUseBlockParam 
  | ToolResultBlockParam;

/**
 * Message structure from test session files
 */
export interface TestSessionMessage {
  role: 'user' | 'assistant' | 'system';
  content: string | ContentBlock[];
}

/**
 * Agent configuration from test sessions
 */
export interface TestAgentConfig {
  version: number;
  name: string;
  agent_description?: string;
  tools?: string[];
  blocked_tool_patterns?: string[];
  allowed_tool_patterns?: string[];
  key?: string;
  model_id?: string;
  agent_params?: Record<string, any>;
  prompt_metadata?: Record<string, any>;
  persona?: string;
  uid?: string;
  category?: string[];
}

/**
 * Complete test session structure
 */
export interface TestSession {
  version: number;
  session_id: string;
  token_count: number;
  context_window_size: number;
  session_name: string | null;
  created_at: string;
  updated_at: string;
  deleted_at: string | null;
  user_id: string;
  metadata: Record<string, any>;
  messages: TestSessionMessage[];
  agent_config?: TestAgentConfig;
}

/**
 * Test scenario configuration
 */
export interface TestScenario {
  id: string;
  name: string;
  description?: string;
  filePath: string;
  sessionData?: TestSession;
  tags?: string[];
}

/**
 * Test mode configuration
 */
export interface TestModeConfig {
  enabled: boolean;
  scenarios: TestScenario[];
  currentScenarioId?: string;
  autoLoadOnMount?: boolean;
  showTestControls?: boolean;
}