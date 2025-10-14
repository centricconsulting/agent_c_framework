/**
 * Input Components for Agent C Realtime UI
 * 
 * The InputArea is the main component that integrates all input functionality.
 * Individual components are also exported for advanced customization.
 */

// Main integrated components
export { InputArea } from './InputArea'
export type { InputAreaProps } from './InputArea'

// Individual components for advanced usage
export { InputContainer } from './InputContainer'
export type { InputContainerProps } from './InputContainer'

export { RichTextEditor } from './RichTextEditor'
export type { RichTextEditorProps } from './RichTextEditor'

export { InputToolbar } from './InputToolbar'
export type { InputToolbarProps } from './InputToolbar'

export { MicrophoneButton } from './MicrophoneButton'
export type { MicrophoneButtonProps } from './MicrophoneButton'

// Note: AgentSelector has been moved to controls/AgentSelector
// It no longer requires props as it uses SDK hooks directly
// Note: OutputSelector has been moved to controls/OutputSelector
// It no longer requires props as it uses SDK hooks directly

// Types
export type { 
  Agent,
  AgentTool, 
  OutputMode, 
  OutputOption 
} from './types'