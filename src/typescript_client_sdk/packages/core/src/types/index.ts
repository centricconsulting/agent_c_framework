/**
 * Common type definitions
 */

export type Nullable<T> = T | null;
export type Optional<T> = T | undefined;
export type AsyncFunction<T = void> = () => Promise<T>;

export interface ErrorDetails {
    code: string;
    message: string;
    details?: unknown;
}

// Export chat session types
export * from './chat-session';

// Export Anthropic message param types
export * from './message-params';

// Export OpenAI message param types
export * from './openai-message-params';

// Export unified chat types
export * from './ChatTypes';