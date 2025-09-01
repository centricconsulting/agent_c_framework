import * as React from "react"
import { useState, useCallback, useEffect, useMemo } from "react"
import { cn } from "../../lib/utils"
import { 
  useChat, 
  useAudio, 
  useTurnState, 
  useVoiceModel, 
  useAvatar 
} from "@agentc/realtime-react"
import type { Voice } from "@agentc/realtime-core"
import { InputContainer } from "./InputContainer"
import { RichTextEditor } from "./RichTextEditor"
import { InputToolbar } from "./InputToolbar"
import type { Agent, OutputMode, OutputOption } from "./types"
import { AlertCircle } from "lucide-react"
import { Alert, AlertDescription } from "../ui/alert"

export interface InputAreaProps {
  // Optional overrides for advanced usage
  className?: string
  maxHeight?: string
  placeholder?: string
  agents?: Agent[]
  voiceOptions?: OutputOption[]
  avatarOptions?: OutputOption[]
  onAgentChange?: (agent: Agent) => void
  onOutputModeChange?: (mode: OutputMode, option?: OutputOption) => void
}

const InputArea: React.FC<InputAreaProps> = ({
  className,
  maxHeight = "200px",
  placeholder = "Type a message or click the microphone to speak...",
  agents: propAgents,
  voiceOptions: propVoiceOptions,
  avatarOptions: propAvatarOptions,
  onAgentChange,
  onOutputModeChange
}) => {
  // SDK hooks
  const { messages, sendMessage, isAgentTyping } = useChat()
  const { 
    isStreaming,
    startStreaming, 
    stopStreaming,
    audioLevel,
    canSendInput,
    errorMessage: audioError
  } = useAudio({ respectTurnState: true })
  const { canSendInput: turnCanSend } = useTurnState()
  const { availableVoices, currentVoice, setVoice } = useVoiceModel()
  const { 
    isAvatarActive,
    avatarSession,
    setAvatar,
    clearAvatar
  } = useAvatar()

  // Local state
  const [content, setContent] = useState('')
  const [selectedAgent, setSelectedAgent] = useState<Agent | null>(null)
  const [outputMode, setOutputMode] = useState<OutputMode>('text')
  const [selectedOutputOption, setSelectedOutputOption] = useState<OutputOption | null>(null)
  const [error, setError] = useState<string | null>(null)

  // Default agents if not provided
  const defaultAgents: Agent[] = useMemo(() => [
    {
      id: 'assistant',
      name: 'Assistant',
      description: 'General purpose AI assistant',
      avatar: '🤖',
      available: true
    },
    {
      id: 'creative',
      name: 'Creative',
      description: 'Creative writing and ideation',
      avatar: '🎨',
      available: true
    },
    {
      id: 'technical',
      name: 'Technical',
      description: 'Technical and coding assistance',
      avatar: '💻',
      available: true
    }
  ], [])

  const agents = propAgents || defaultAgents

  // Map SDK voices to OutputOptions
  const voiceOptions = useMemo(() => {
    if (propVoiceOptions) return propVoiceOptions
    
    // Default voice options if SDK doesn't provide them
    if (!availableVoices || availableVoices.length === 0) {
      return [
        { id: 'alloy', name: 'Alloy', type: 'voice' as const, available: true },
        { id: 'echo', name: 'Echo', type: 'voice' as const, available: true },
        { id: 'fable', name: 'Fable', type: 'voice' as const, available: true },
        { id: 'onyx', name: 'Onyx', type: 'voice' as const, available: true },
        { id: 'nova', name: 'Nova', type: 'voice' as const, available: true },
        { id: 'shimmer', name: 'Shimmer', type: 'voice' as const, available: true }
      ]
    }
    
    return availableVoices
      .filter((v: Voice) => v.voice_id !== 'none' && v.voice_id !== 'avatar')
      .map((v: Voice) => ({
        id: v.voice_id,
        name: v.description,
        type: 'voice' as const,
        available: true,
        metadata: { voiceId: v.voice_id }
      }))
  }, [availableVoices, propVoiceOptions])

  // Map SDK avatars to OutputOptions
  const avatarOptions = useMemo(() => {
    if (propAvatarOptions) return propAvatarOptions
    
    // Default avatar options
    return [
      { 
        id: 'avatar-1', 
        name: 'Professional', 
        type: 'avatar' as const, 
        available: true,
        metadata: { avatarId: 'avatar-1' }
      },
      { 
        id: 'avatar-2', 
        name: 'Friendly', 
        type: 'avatar' as const, 
        available: true,
        metadata: { avatarId: 'avatar-2' }
      }
    ]
  }, [propAvatarOptions])

  // Initialize selected agent
  useEffect(() => {
    if (!selectedAgent && agents.length > 0) {
      setSelectedAgent(agents[0])
    }
  }, [agents, selectedAgent])

  // Clear error after 5 seconds
  useEffect(() => {
    if (error) {
      const timer = setTimeout(() => setError(null), 5000)
      return () => clearTimeout(timer)
    }
  }, [error])

  // Handle text submission
  const handleSendText = useCallback(async () => {
    if (!canSendInput || !content.trim()) return
    
    try {
      // Stop recording if active
      if (isStreaming) {
        await stopStreaming()
      }
      
      // Send message
      await sendMessage(content)
      
      // Clear editor
      setContent('')
      setError(null)
    } catch (err) {
      console.error('Failed to send message:', err)
      setError('Failed to send message. Please try again.')
      // Keep content for retry
    }
  }, [canSendInput, content, isStreaming, stopStreaming, sendMessage])

  // Handle recording start
  const handleStartRecording = useCallback(async () => {
    if (!canSendInput) {
      setError("Can't start recording - wait for your turn")
      return
    }
    
    try {
      setError(null)
      await startStreaming()
    } catch (err) {
      console.error('Failed to start recording:', err)
      if (err instanceof Error) {
        if (err.message.includes('permission')) {
          setError('Microphone permission denied. Please check your browser settings.')
        } else {
          setError('Failed to start recording. Please try again.')
        }
      }
    }
  }, [canSendInput, startStreaming])

  // Handle recording stop
  const handleStopRecording = useCallback(async () => {
    try {
      await stopStreaming()
      setError(null)
    } catch (err) {
      console.error('Failed to stop recording:', err)
      setError('Failed to stop recording. Please try again.')
    }
  }, [stopStreaming])

  // Handle agent change
  const handleAgentChange = useCallback((agent: Agent) => {
    setSelectedAgent(agent)
    onAgentChange?.(agent)
  }, [onAgentChange])

  // Handle output mode change
  const handleOutputModeChange = useCallback(async (mode: OutputMode, option?: OutputOption) => {
    try {
      setError(null)
      setOutputMode(mode)
      setSelectedOutputOption(option || null)
      
      // Update SDK based on output mode
      switch (mode) {
        case 'text':
          // Disable voice and avatar
          await setVoice('none')
          if (isAvatarActive) {
            await clearAvatar()
          }
          break
          
        case 'voice':
          // Set voice, disable avatar
          if (option?.metadata?.voiceId) {
            await setVoice(option.metadata.voiceId)
          }
          if (isAvatarActive) {
            await clearAvatar()
          }
          break
          
        case 'avatar':
          // Enable avatar mode
          await setVoice('avatar')
          if (option?.metadata?.avatarId) {
            // For avatar, we need to create a session - using a placeholder session ID
            const sessionId = `session-${Date.now()}`
            await setAvatar(option.metadata.avatarId, sessionId)
          }
          break
      }
      
      onOutputModeChange?.(mode, option)
    } catch (err) {
      console.error('Failed to change output mode:', err)
      setError('Failed to change output mode. Please try again.')
    }
  }, [setVoice, isAvatarActive, clearAvatar, setAvatar, onOutputModeChange])

  // Stop recording if turn is lost
  useEffect(() => {
    if (isStreaming && !canSendInput) {
      handleStopRecording()
    }
  }, [isStreaming, canSendInput, handleStopRecording])

  // Determine if input should be disabled
  const isInputDisabled = !canSendInput || isAgentTyping

  return (
    <div className={cn("w-full", className)}>
      {/* Error display */}
      {(error || audioError) && (
        <Alert variant="destructive" className="mb-2">
          <AlertCircle className="h-4 w-4" />
          <AlertDescription>
            {error || audioError}
          </AlertDescription>
        </Alert>
      )}
      
      {/* Main input area */}
      <InputContainer maxHeight={maxHeight}>
        <RichTextEditor
          value={content}
          onChange={setContent}
          onSubmit={handleSendText}
          disabled={isInputDisabled}
          placeholder={
            isInputDisabled 
              ? (isAgentTyping ? "Agent is typing..." : "Wait for your turn...")
              : placeholder
          }
          className="flex-1"
        />
        
        <InputToolbar
          onSend={handleSendText}
          canSend={content.trim().length > 0 && canSendInput}
          isRecording={isStreaming}
          onStartRecording={handleStartRecording}
          onStopRecording={handleStopRecording}
          audioLevel={audioLevel}
          selectedAgent={selectedAgent || undefined}
          onAgentChange={handleAgentChange}
          outputMode={outputMode}
          onOutputModeChange={handleOutputModeChange}
        />
      </InputContainer>
      
      {/* Status indicators */}
      {(isStreaming || isAgentTyping) && (
        <div className="mt-2 px-2">
          <div className="flex items-center gap-2 text-xs text-muted-foreground">
            <div 
              className={cn(
                "h-2 w-2 rounded-full",
                isStreaming && "bg-green-500 animate-pulse",
                isAgentTyping && "bg-blue-500 animate-pulse"
              )}
            />
            <span>
              {isStreaming && "Recording..."}
              {isAgentTyping && "Agent is typing..."}
            </span>
          </div>
        </div>
      )}
    </div>
  )
}

InputArea.displayName = "InputArea"

export { InputArea }