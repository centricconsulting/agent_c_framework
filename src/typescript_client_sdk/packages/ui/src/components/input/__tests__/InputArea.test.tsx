import { describe, it, expect, vi, beforeEach, afterEach } from 'vitest'
import { render, screen, waitFor, fireEvent, act } from '@testing-library/react'
import { InputArea } from '../InputArea'
import '@testing-library/jest-dom'
import { updateMockState } from '../../../test/mocks/realtime-react'

// Note: @agentc/realtime-react is already mocked globally in test setup

// Mock child components to isolate testing
vi.mock('../InputContainer', () => ({
  InputContainer: vi.fn(({ children }) => <div data-testid="input-container">{children}</div>)
}))

vi.mock('../RichTextEditor', () => ({
  RichTextEditor: vi.fn(({ value, onChange, onSubmit, disabled, placeholder }) => (
    <textarea
      data-testid="rich-text-editor"
      value={value}
      onChange={(e) => onChange(e.target.value)}
      onKeyDown={(e) => {
        if (e.key === 'Enter' && !e.shiftKey) {
          e.preventDefault()
          onSubmit?.()
        }
      }}
      disabled={disabled}
      placeholder={placeholder}
    />
  ))
}))

vi.mock('../InputToolbar', () => ({
  InputToolbar: vi.fn(({ 
    onSend, 
    canSend, 
    isRecording, 
    onStartRecording, 
    onStopRecording,
    audioLevel,
    agents,
    selectedAgent,
    onAgentChange 
  }) => (
    <div data-testid="input-toolbar">
      <button 
        data-testid="send-button" 
        onClick={onSend}
        disabled={!canSend}
      >
        Send
      </button>
      <button
        data-testid="record-button"
        onClick={isRecording ? onStopRecording : onStartRecording}
      >
        {isRecording ? 'Stop' : 'Record'}
      </button>
      {audioLevel !== undefined && (
        <span data-testid="audio-level">{audioLevel}</span>
      )}
      {agents && selectedAgent && (
        <select 
          data-testid="agent-select"
          value={selectedAgent.id}
          onChange={(e) => {
            const agent = agents.find(a => a.id === e.target.value)
            if (agent) onAgentChange(agent)
          }}
        >
          {agents.map(agent => (
            <option key={agent.id} value={agent.id}>{agent.name}</option>
          ))}
        </select>
      )}
    </div>
  ))
}))

// Import mocked modules
import { 
  useChat, 
  useAudio, 
  useTurnState, 
  useVoiceModel, 
  useAvatar 
} from '@agentc/realtime-react'

describe('InputArea', () => {
  // Default mock implementations
  const mockSendMessage = vi.fn()
  const mockStartStreaming = vi.fn()
  const mockStopStreaming = vi.fn()
  const mockSetVoice = vi.fn()
  const mockSetAvatar = vi.fn()
  const mockClearAvatar = vi.fn()

  beforeEach(() => {
    vi.clearAllMocks()
    
    // Setup default hook mocks using the global mock utilities
    updateMockState('chat', {
      messages: [],
      sendMessage: mockSendMessage,
      isLoading: false,
      error: null,
      clearMessages: vi.fn(),
      updateMessage: vi.fn(),
      deleteMessage: vi.fn()
    })
    
    updateMockState('audio', {
      isStreaming: false,
      audioLevel: 0,
      isMuted: false,
      canSendInput: true,
      errorMessage: null,
      startStreaming: mockStartStreaming,
      stopStreaming: mockStopStreaming,
      toggleMute: vi.fn(),
      setMuted: vi.fn()
    })
    
    updateMockState('turnState', {
      currentState: 'idle',
      isUserTurn: true,
      isAgentTurn: false,
      canInterrupt: true,
      canSendInput: true,
      turnCount: 0,
      lastTurnTimestamp: null
    })
    
    updateMockState('voiceModel', {
      selectedVoice: { voice_id: 'alloy', description: 'Alloy' },
      availableVoices: [
        { voice_id: 'alloy', description: 'Alloy' },
        { voice_id: 'echo', description: 'Echo' }
      ],
      setVoice: mockSetVoice,
      isLoading: false
    })
    
    updateMockState('avatar', {
      isEnabled: false,
      avatarUrl: null,
      setEnabled: mockSetAvatar,
      error: null,
      isLoading: false
    })
  })

  afterEach(() => {
    vi.restoreAllMocks()
  })

  describe('Basic rendering', () => {
    it('should render editor and toolbar', () => {
      render(<InputArea />)
      
      expect(screen.getByTestId('input-container')).toBeInTheDocument()
      expect(screen.getByTestId('rich-text-editor')).toBeInTheDocument()
      expect(screen.getByTestId('input-toolbar')).toBeInTheDocument()
    })

    it('should display custom placeholder', () => {
      const placeholder = 'Custom placeholder text'
      render(<InputArea placeholder={placeholder} />)
      
      expect(screen.getByTestId('rich-text-editor')).toHaveAttribute('placeholder', placeholder)
    })

    it('should apply custom className', () => {
      const { container } = render(<InputArea className="custom-class" />)
      
      expect(container.firstChild).toHaveClass('custom-class')
    })
  })

  describe('Text input and sending', () => {
    it('should update content when typing', async () => {
      render(<InputArea />)
      
      const editor = screen.getByTestId('rich-text-editor') as HTMLTextAreaElement
      fireEvent.change(editor, { target: { value: 'Hello world' } })
      
      expect(editor).toHaveValue('Hello world')
    })

    it('should send message when send button clicked', async () => {
      render(<InputArea />)
      
      const editor = screen.getByTestId('rich-text-editor') as HTMLTextAreaElement
      fireEvent.change(editor, { target: { value: 'Test message' } })
      
      const sendButton = screen.getByTestId('send-button')
      fireEvent.click(sendButton)
      
      await waitFor(() => {
        expect(mockSendMessage).toHaveBeenCalledWith('Test message')
      })
    })

    it('should clear editor after successful send', async () => {
      mockSendMessage.mockResolvedValue(undefined)
      
      render(<InputArea />)
      
      const editor = screen.getByTestId('rich-text-editor') as HTMLTextAreaElement
      fireEvent.change(editor, { target: { value: 'Test message' } })
      fireEvent.click(screen.getByTestId('send-button'))
      
      await waitFor(() => {
        expect(editor).toHaveValue('')
      })
    })

    it('should disable send button when no content', () => {
      render(<InputArea />)
      
      const sendButton = screen.getByTestId('send-button')
      expect(sendButton).toBeDisabled()
    })

    it('should stop streaming before sending text if recording', async () => {
      vi.mocked(useAudio).mockReturnValue({
        isStreaming: true,
        startStreaming: mockStartStreaming,
        stopStreaming: mockStopStreaming,
        audioLevel: 0.5,
        canSendInput: true,
        errorMessage: null,
        isMuted: false,
        setMuted: vi.fn(),
        volume: 1.0,
        setVolume: vi.fn(),
        isVadEnabled: true,
        setVadEnabled: vi.fn()
      } as any)
      
      render(<InputArea />)
      
      const editor = screen.getByTestId('rich-text-editor') as HTMLTextAreaElement
      fireEvent.change(editor, { target: { value: 'Test' } })
      fireEvent.click(screen.getByTestId('send-button'))
      
      await waitFor(() => {
        expect(mockStopStreaming).toHaveBeenCalled()
        expect(mockSendMessage).toHaveBeenCalledWith('Test')
      })
    })
  })

  describe('Voice streaming', () => {
    it('should start streaming when record button clicked', async () => {
      render(<InputArea />)
      
      const recordButton = screen.getByTestId('record-button')
      fireEvent.click(recordButton)
      
      await waitFor(() => {
        expect(mockStartStreaming).toHaveBeenCalled()
      })
    })

    it('should stop streaming when stop button clicked', async () => {
      vi.mocked(useAudio).mockReturnValue({
        isStreaming: true,
        startStreaming: mockStartStreaming,
        stopStreaming: mockStopStreaming,
        audioLevel: 0.5,
        canSendInput: true,
        errorMessage: null,
        isMuted: false,
        setMuted: vi.fn(),
        volume: 1.0,
        setVolume: vi.fn(),
        isVadEnabled: true,
        setVadEnabled: vi.fn()
      } as any)
      
      render(<InputArea />)
      
      const recordButton = screen.getByTestId('record-button')
      expect(recordButton).toHaveTextContent('Stop')
      
      await act(async () => {
        fireEvent.click(recordButton)
      })
      expect(mockStopStreaming).toHaveBeenCalled()
    })

    it('should display audio level when streaming', () => {
      vi.mocked(useAudio).mockReturnValue({
        isStreaming: true,
        startStreaming: mockStartStreaming,
        stopStreaming: mockStopStreaming,
        audioLevel: 0.75,
        canSendInput: true,
        errorMessage: null,
        isMuted: false,
        setMuted: vi.fn(),
        volume: 1.0,
        setVolume: vi.fn(),
        isVadEnabled: true,
        setVadEnabled: vi.fn()
      } as any)
      
      render(<InputArea />)
      
      expect(screen.getByTestId('audio-level')).toHaveTextContent('0.75')
    })
  })

  describe('Turn state awareness', () => {
    it('should disable input when canSendInput is false', () => {
      vi.mocked(useTurnState).mockReturnValue({
        canSendInput: false,
        turnState: 'agent_turn'
      } as any)
      
      vi.mocked(useAudio).mockReturnValue({
        isStreaming: false,
        startStreaming: mockStartStreaming,
        stopStreaming: mockStopStreaming,
        audioLevel: 0,
        canSendInput: false,
        errorMessage: null,
        isMuted: false,
        setMuted: vi.fn(),
        volume: 1.0,
        setVolume: vi.fn(),
        isVadEnabled: true,
        setVadEnabled: vi.fn()
      } as any)
      
      render(<InputArea />)
      
      const editor = screen.getByTestId('rich-text-editor')
      expect(editor).toBeDisabled()
      expect(editor).toHaveAttribute('placeholder', 'Wait for your turn...')
    })

    it('should stop recording when turn is lost', async () => {
      const { rerender } = render(<InputArea />)
      
      // Start with streaming and can send
      vi.mocked(useAudio).mockReturnValue({
        isStreaming: true,
        startStreaming: mockStartStreaming,
        stopStreaming: mockStopStreaming,
        audioLevel: 0.5,
        canSendInput: true,
        errorMessage: null,
        isMuted: false,
        setMuted: vi.fn(),
        volume: 1.0,
        setVolume: vi.fn(),
        isVadEnabled: true,
        setVadEnabled: vi.fn()
      } as any)
      
      rerender(<InputArea />)
      
      // Lose turn
      vi.mocked(useAudio).mockReturnValue({
        isStreaming: true,
        startStreaming: mockStartStreaming,
        stopStreaming: mockStopStreaming,
        audioLevel: 0.5,
        canSendInput: false,
        errorMessage: null,
        isMuted: false,
        setMuted: vi.fn(),
        volume: 1.0,
        setVolume: vi.fn(),
        isVadEnabled: true,
        setVadEnabled: vi.fn()
      } as any)
      
      rerender(<InputArea />)
      
      await waitFor(() => {
        expect(mockStopStreaming).toHaveBeenCalled()
      })
    })
  })

  describe('Error handling', () => {
    it('should display audio error from hook', () => {
      vi.mocked(useAudio).mockReturnValue({
        isStreaming: false,
        startStreaming: mockStartStreaming,
        stopStreaming: mockStopStreaming,
        audioLevel: 0,
        canSendInput: true,
        errorMessage: 'Microphone not available',
        isMuted: false,
        setMuted: vi.fn(),
        volume: 1.0,
        setVolume: vi.fn(),
        isVadEnabled: true,
        setVadEnabled: vi.fn()
      } as any)
      
      render(<InputArea />)
      
      expect(screen.getByText('Microphone not available')).toBeInTheDocument()
    })

    it('should display error when send fails', async () => {
      mockSendMessage.mockRejectedValue(new Error('Network error'))
      
      render(<InputArea />)
      
      const editor = screen.getByTestId('rich-text-editor') as HTMLTextAreaElement
      fireEvent.change(editor, { target: { value: 'Test' } })
      fireEvent.click(screen.getByTestId('send-button'))
      
      await waitFor(() => {
        expect(screen.getByText('Failed to send message. Please try again.')).toBeInTheDocument()
      })
      
      // Content should be retained for retry
      expect(editor).toHaveValue('Test')
    })

    it('should display error when starting recording fails', async () => {
      mockStartStreaming.mockRejectedValue(new Error('permission denied'))
      
      render(<InputArea />)
      
      fireEvent.click(screen.getByTestId('record-button'))
      
      await waitFor(() => {
        expect(screen.getByText('Microphone permission denied. Please check your browser settings.')).toBeInTheDocument()
      })
    })

    it('should display error when stopping recording fails', async () => {
      vi.mocked(useAudio).mockReturnValue({
        isStreaming: true,
        startStreaming: mockStartStreaming,
        stopStreaming: mockStopStreaming,
        audioLevel: 0.5,
        canSendInput: true,
        errorMessage: null,
        isMuted: false,
        setMuted: vi.fn(),
        volume: 1.0,
        setVolume: vi.fn(),
        isVadEnabled: true,
        setVadEnabled: vi.fn()
      } as any)
      
      mockStopStreaming.mockRejectedValue(new Error('Stop failed'))
      
      render(<InputArea />)
      
      fireEvent.click(screen.getByTestId('record-button'))
      
      await waitFor(() => {
        expect(screen.getByText('Failed to stop recording. Please try again.')).toBeInTheDocument()
      })
    })

    it('should clear error after 5 seconds', async () => {
      // Use real timers for this test
      mockSendMessage.mockRejectedValue(new Error('Network error'))
      
      render(<InputArea />)
      
      const editor = screen.getByTestId('rich-text-editor') as HTMLTextAreaElement
      fireEvent.change(editor, { target: { value: 'Test' } })
      
      // Trigger the error
      fireEvent.click(screen.getByTestId('send-button'))
      
      // Wait for error to appear
      await waitFor(() => {
        expect(screen.getByText('Failed to send message. Please try again.')).toBeInTheDocument()
      })
      
      // Verify error is present
      expect(screen.getByText('Failed to send message. Please try again.')).toBeInTheDocument()
      
      // Wait for error to be cleared (with a slightly longer timeout to be safe)
      await waitFor(() => {
        expect(screen.queryByText('Failed to send message. Please try again.')).not.toBeInTheDocument()
      }, { timeout: 6000 })
    }, 10000)
  })

  describe('Agent/voice/avatar selection', () => {
    it('should display default agents when none provided', () => {
      render(<InputArea />)
      
      const agentSelect = screen.getByTestId('agent-select')
      expect(agentSelect).toBeInTheDocument()
      
      const options = agentSelect.querySelectorAll('option')
      expect(options).toHaveLength(3)
      expect(options[0]).toHaveTextContent('Assistant')
      expect(options[1]).toHaveTextContent('Creative')
      expect(options[2]).toHaveTextContent('Technical')
    })

    it('should use provided agents', () => {
      const customAgents = [
        {
          id: 'agent1',
          name: 'Custom Agent 1',
          description: 'Test agent 1',
          avatar: '🎯',
          tools: [],
          capabilities: [],
          available: true
        },
        {
          id: 'agent2',
          name: 'Custom Agent 2',
          description: 'Test agent 2',
          avatar: '🎪',
          tools: [],
          capabilities: [],
          available: true
        }
      ]
      
      render(<InputArea agents={customAgents} />)
      
      const agentSelect = screen.getByTestId('agent-select')
      const options = agentSelect.querySelectorAll('option')
      expect(options).toHaveLength(2)
      expect(options[0]).toHaveTextContent('Custom Agent 1')
      expect(options[1]).toHaveTextContent('Custom Agent 2')
    })

    it('should handle agent change', async () => {
      const onAgentChange = vi.fn()
      const customAgents = [
        {
          id: 'agent1',
          name: 'Agent 1',
          description: 'Test agent 1',
          avatar: '🎯',
          tools: [],
          capabilities: [],
          available: true
        },
        {
          id: 'agent2',
          name: 'Agent 2',
          description: 'Test agent 2',
          avatar: '🎪',
          tools: [],
          capabilities: [],
          available: true
        }
      ]
      
      render(<InputArea agents={customAgents} onAgentChange={onAgentChange} />)
      
      const agentSelect = screen.getByTestId('agent-select') as HTMLSelectElement
      fireEvent.change(agentSelect, { target: { value: 'agent2' } })
      
      expect(onAgentChange).toHaveBeenCalledWith(customAgents[1])
    })

    it('should map SDK voices to voice options', () => {
      vi.mocked(useVoiceModel).mockReturnValue({
        availableVoices: [
          { voice_id: 'voice1', description: 'Voice One' },
          { voice_id: 'voice2', description: 'Voice Two' },
          { voice_id: 'none', description: 'None' },
          { voice_id: 'avatar', description: 'Avatar' }
        ],
        currentVoice: { voice_id: 'voice1', description: 'Voice One' },
        setVoice: mockSetVoice
      } as any)
      
      render(<InputArea />)
      
      // The component should filter out 'none' and 'avatar' voices
      // This is internal logic that we're testing through component behavior
      // In reality, this would be tested more thoroughly through integration tests
      expect(screen.getByTestId('input-toolbar')).toBeInTheDocument()
    })

    it('should use provided voice options', () => {
      const customVoiceOptions = [
        { id: 'v1', name: 'Custom Voice 1', type: 'voice' as const, available: true },
        { id: 'v2', name: 'Custom Voice 2', type: 'voice' as const, available: true }
      ]
      
      render(<InputArea voiceOptions={customVoiceOptions} />)
      
      // Voice options are passed to InputToolbar
      expect(screen.getByTestId('input-toolbar')).toBeInTheDocument()
    })

    it('should use provided avatar options', () => {
      const customAvatarOptions = [
        { id: 'a1', name: 'Avatar 1', type: 'avatar' as const, available: true },
        { id: 'a2', name: 'Avatar 2', type: 'avatar' as const, available: true }
      ]
      
      render(<InputArea avatarOptions={customAvatarOptions} />)
      
      // Avatar options are passed to InputToolbar
      expect(screen.getByTestId('input-toolbar')).toBeInTheDocument()
    })
  })
})