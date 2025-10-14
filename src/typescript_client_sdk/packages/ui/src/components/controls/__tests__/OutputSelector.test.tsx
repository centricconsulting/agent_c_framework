/**
 * @fileoverview Unit tests for OutputSelector component
 * @module @agentc/realtime-ui/components/controls/__tests__
 */

import React from 'react'
import { describe, it, expect, vi, beforeEach, afterEach } from 'vitest'
import { render, screen, waitFor, within } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import { OutputSelector } from '../OutputSelector'
import type { Voice } from '@agentc/realtime-core'

// Note: @agentc/realtime-react is globally mocked in vitest.setup.ts
import { updateMockState } from '../../../test/mocks/realtime-react'
import { useRealtimeClientSafe, useAgentCData, useInitializationStatus, useVoiceModel, useConnection } from '@agentc/realtime-react'

// These are already mocked functions from the global mock
const mockUseRealtimeClientSafe = useRealtimeClientSafe as ReturnType<typeof vi.fn>
const mockUseAgentCData = useAgentCData as ReturnType<typeof vi.fn>
const mockUseInitializationStatus = useInitializationStatus as ReturnType<typeof vi.fn>
const mockUseVoiceModel = useVoiceModel as ReturnType<typeof vi.fn>
const mockUseConnection = useConnection as ReturnType<typeof vi.fn>

// Mock the filterRegularVoices utility
vi.mock('@agentc/realtime-core', async () => {
  const actual = await vi.importActual('@agentc/realtime-core')
  return {
    ...actual,
    filterRegularVoices: (voices: Voice[]) => {
      return voices.filter(v => v.voice_id !== 'none' && v.voice_id !== 'avatar')
    }
  }
})

// Default mock setup
const createDefaultMocks = () => {
  const setAgentVoiceMock = vi.fn().mockResolvedValue(undefined)
  const getAvatarManagerMock = vi.fn().mockReturnValue({
    getAvatarId: vi.fn().mockReturnValue(null)
  })
  
  return {
    client: {
      setAgentVoice: setAgentVoiceMock,
      getAvatarManager: getAvatarManagerMock,
      on: vi.fn(),
      off: vi.fn()
    },
    data: {
      voices: [] as Voice[],
      avatars: []
    },
    voiceModel: {
      currentVoice: { voice_id: 'none', vendor: '', description: '' } as Voice,
      isLoading: false,
      error: null as string | null
    },
    connection: {
      isConnected: true
    },
    initialization: {
      isInitialized: true
    },
    agentCData: {
      data: {
        voices: [] as Voice[],
        avatars: []
      },
      isInitialized: true,
      isLoading: false
    }
  }
}

describe('OutputSelector', () => {
  let mocks: ReturnType<typeof createDefaultMocks>

  beforeEach(() => {
    vi.clearAllMocks()
    mocks = createDefaultMocks()
    
    mockUseRealtimeClientSafe.mockReturnValue(mocks.client)
    updateMockState('agentCData', mocks.agentCData)
    updateMockState('initializationStatus', mocks.initialization)
    updateMockState('voiceModel', mocks.voiceModel)
    updateMockState('connection', mocks.connection)
  })

  afterEach(() => {
    vi.restoreAllMocks()
  })

  describe('Basic Rendering', () => {
    it('should render trigger button', () => {
      render(<OutputSelector />)
      expect(screen.getByRole('combobox')).toBeInTheDocument()
    })

    it('should display current mode in button', () => {
      render(<OutputSelector />)
      expect(screen.getByText('Text Only')).toBeInTheDocument()
    })

    it('should show icon when showIcon is true', () => {
      render(<OutputSelector showIcon={true} />)
      const button = screen.getByRole('combobox')
      const icon = button.querySelector('svg')
      expect(icon).toBeInTheDocument()
    })

    it('should not show icon when showIcon is false', () => {
      render(<OutputSelector showIcon={false} />)
      const button = screen.getByRole('combobox')
      const icon = button.querySelector('svg')
      expect(icon).not.toBeInTheDocument()
    })
  })

  describe('Mode Display States', () => {
    it('should show "Text Only" for none voice', () => {
      mocks.voiceModel.currentVoice = { voice_id: 'none', vendor: '', description: '' }
      mockUseVoiceModel.mockReturnValue(mocks.voiceModel)
      
      render(<OutputSelector />)
      expect(screen.getByText('Text Only')).toBeInTheDocument()
    })

    it('should show voice description for active voice', () => {
      mocks.voiceModel.currentVoice = {
        voice_id: 'alloy',
        vendor: 'OpenAI',
        description: 'Neutral and balanced'
      }
      mockUseVoiceModel.mockReturnValue(mocks.voiceModel)
      
      render(<OutputSelector />)
      expect(screen.getByText('Neutral and balanced')).toBeInTheDocument()
    })

    it('should show vendor and voice_id when no description', () => {
      mocks.voiceModel.currentVoice = {
        voice_id: 'echo',
        vendor: 'OpenAI',
        description: ''
      }
      mockUseVoiceModel.mockReturnValue(mocks.voiceModel)
      
      render(<OutputSelector />)
      expect(screen.getByText('OpenAI echo')).toBeInTheDocument()
    })

    it('should show avatar mode when avatar voice selected', () => {
      mocks.voiceModel.currentVoice = { voice_id: 'avatar', vendor: '', description: '' }
      mockUseVoiceModel.mockReturnValue(mocks.voiceModel)
      
      render(<OutputSelector />)
      expect(screen.getByText('Avatar Mode')).toBeInTheDocument()
    })

    it('should show avatar description when avatar is active', () => {
      mocks.voiceModel.currentVoice = { voice_id: 'avatar', vendor: '', description: '' }
      mocks.client.getAvatarManager = vi.fn().mockReturnValue({
        getAvatarId: vi.fn().mockReturnValue('avatar1')
      })
      mocks.agentCData.data.avatars = [
        { avatar_id: 'avatar1', pose_name: 'Friendly', is_public: true }
      ]
      
      mockUseVoiceModel.mockReturnValue(mocks.voiceModel)
      mockUseRealtimeClientSafe.mockReturnValue(mocks.client)
      mockUseAgentCData.mockReturnValue(mocks.agentCData)
      
      render(<OutputSelector />)
      expect(screen.getByText('Friendly')).toBeInTheDocument()
    })
  })

  describe('Portal Popover Rendering', () => {
    it('should render popover in document.body when open', async () => {
      const user = userEvent.setup()
      render(<OutputSelector />)
      
      const button = screen.getByRole('combobox')
      await user.click(button)
      
      // Popover should be in document.body, not in component tree
      const popover = document.body.querySelector('[role="listbox"]')
      expect(popover).toBeInTheDocument()
    })

    it('should render backdrop when popover is open', async () => {
      const user = userEvent.setup()
      render(<OutputSelector />)
      
      const button = screen.getByRole('combobox')
      await user.click(button)
      
      const backdrop = document.body.querySelector('.bg-black\\/20')
      expect(backdrop).toBeInTheDocument()
    })

    it('should close popover when clicking backdrop', async () => {
      const user = userEvent.setup()
      render(<OutputSelector />)
      
      await user.click(screen.getByRole('combobox'))
      
      const backdrop = document.body.querySelector('.bg-black\\/20')
      expect(backdrop).toBeInTheDocument()
      
      await user.click(backdrop!)
      
      expect(document.body.querySelector('[role="listbox"]')).not.toBeInTheDocument()
    })
  })

  describe('Tab Navigation', () => {
    beforeEach(() => {
      mocks.agentCData.data = {
        voices: [
          { voice_id: 'voice1', vendor: 'OpenAI', description: 'Voice 1' },
          { voice_id: 'voice2', vendor: 'Google', description: 'Voice 2' }
        ],
        avatars: [
          { avatar_id: 'avatar1', pose_name: 'Friendly', is_public: true }
        ]
      }
      mockUseAgentCData.mockReturnValue(mocks.agentCData)
    })

    it('should start with modes tab active', async () => {
      const user = userEvent.setup()
      render(<OutputSelector />)
      await user.click(screen.getByRole('combobox'))
      
      const modesTab = screen.getByRole('tab', { name: /modes/i })
      expect(modesTab).toHaveAttribute('aria-selected', 'true')
    })

    it('should switch to voices tab when clicked', async () => {
      const user = userEvent.setup()
      render(<OutputSelector />)
      await user.click(screen.getByRole('combobox'))
      
      const voicesTab = screen.getByRole('tab', { name: /voices/i })
      await user.click(voicesTab)
      
      expect(voicesTab).toHaveAttribute('aria-selected', 'true')
    })

    it('should show voice count in tab', async () => {
      const user = userEvent.setup()
      render(<OutputSelector />)
      await user.click(screen.getByRole('combobox'))
      
      const voicesTab = screen.getByRole('tab', { name: /voices/i })
      expect(voicesTab).toHaveTextContent('Voices (2)')
    })

    it('should show avatar count in tab', async () => {
      const user = userEvent.setup()
      render(<OutputSelector />)
      await user.click(screen.getByRole('combobox'))
      
      const avatarsTab = screen.getByRole('tab', { name: /avatars/i })
      expect(avatarsTab).toHaveTextContent('Avatars (1)')
    })

    it('should disable avatars tab when no avatars available', async () => {
      mocks.agentCData.data.avatars = []
      mockUseAgentCData.mockReturnValue(mocks.agentCData)
      
      const user = userEvent.setup()
      render(<OutputSelector />)
      await user.click(screen.getByRole('combobox'))
      
      const avatarsTab = screen.getByRole('tab', { name: /avatars/i })
      expect(avatarsTab).toBeDisabled()
    })
  })

  describe('Voice Search', () => {
    const mockVoices = [
      { voice_id: 'alloy', vendor: 'OpenAI', description: 'Neutral voice' },
      { voice_id: 'echo', vendor: 'OpenAI', description: 'Soft voice' },
      { voice_id: 'neural', vendor: 'Google', description: 'Natural voice' }
    ]

    beforeEach(() => {
      mocks.agentCData.data = { voices: mockVoices, avatars: [] }
      mockUseAgentCData.mockReturnValue(mocks.agentCData)
    })

    it('should show search input in voices tab', async () => {
      const user = userEvent.setup()
      render(<OutputSelector />)
      await user.click(screen.getByRole('combobox'))
      await user.click(screen.getByRole('tab', { name: /voices/i }))
      
      expect(screen.getByPlaceholderText('Search voices...')).toBeInTheDocument()
    })

    it('should filter voices by vendor', async () => {
      const user = userEvent.setup()
      render(<OutputSelector />)
      await user.click(screen.getByRole('combobox'))
      await user.click(screen.getByRole('tab', { name: /voices/i }))
      
      const searchInput = screen.getByPlaceholderText('Search voices...')
      await user.type(searchInput, 'Google')
      
      expect(screen.getByText('Google - neural')).toBeInTheDocument()
      expect(screen.queryByText('OpenAI - alloy')).not.toBeInTheDocument()
    })

    it('should filter voices by description', async () => {
      const user = userEvent.setup()
      render(<OutputSelector />)
      await user.click(screen.getByRole('combobox'))
      await user.click(screen.getByRole('tab', { name: /voices/i }))
      
      const searchInput = screen.getByPlaceholderText('Search voices...')
      await user.type(searchInput, 'soft')
      
      expect(screen.getByText('OpenAI - echo')).toBeInTheDocument()
      expect(screen.queryByText('OpenAI - alloy')).not.toBeInTheDocument()
    })

    it('should show no results message when no matches', async () => {
      const user = userEvent.setup()
      render(<OutputSelector />)
      await user.click(screen.getByRole('combobox'))
      await user.click(screen.getByRole('tab', { name: /voices/i }))
      
      const searchInput = screen.getByPlaceholderText('Search voices...')
      await user.type(searchInput, 'nonexistent')
      
      expect(screen.getByText('No voices found matching your search.')).toBeInTheDocument()
    })

    it('should clear search when clicking clear button', async () => {
      const user = userEvent.setup()
      render(<OutputSelector />)
      await user.click(screen.getByRole('combobox'))
      await user.click(screen.getByRole('tab', { name: /voices/i }))
      
      const searchInput = screen.getByPlaceholderText('Search voices...')
      await user.type(searchInput, 'test')
      
      const clearButton = screen.getByLabelText('Clear search')
      await user.click(clearButton)
      
      expect(searchInput).toHaveValue('')
    })
  })

  describe('Avatar Search', () => {
    const mockAvatars = [
      { avatar_id: 'avatar1', pose_name: 'Friendly', is_public: true },
      { avatar_id: 'avatar2', pose_name: 'Professional', is_public: true },
      { avatar_id: 'avatar3', pose_name: 'Casual', is_public: true }
    ]

    beforeEach(() => {
      mocks.agentCData.data = { voices: [], avatars: mockAvatars }
      mockUseAgentCData.mockReturnValue(mocks.agentCData)
    })

    it('should show search input in avatars tab', async () => {
      const user = userEvent.setup()
      render(<OutputSelector />)
      await user.click(screen.getByRole('combobox'))
      await user.click(screen.getByRole('tab', { name: /avatars/i }))
      
      expect(screen.getByPlaceholderText('Search avatars...')).toBeInTheDocument()
    })

    it('should filter avatars by pose name', async () => {
      const user = userEvent.setup()
      render(<OutputSelector />)
      await user.click(screen.getByRole('combobox'))
      await user.click(screen.getByRole('tab', { name: /avatars/i }))
      
      const searchInput = screen.getByPlaceholderText('Search avatars...')
      await user.type(searchInput, 'friendly')
      
      expect(screen.getByText('Friendly')).toBeInTheDocument()
      expect(screen.queryByText('Professional')).not.toBeInTheDocument()
    })

    it('should filter avatars by avatar_id', async () => {
      const user = userEvent.setup()
      render(<OutputSelector />)
      await user.click(screen.getByRole('combobox'))
      await user.click(screen.getByRole('tab', { name: /avatars/i }))
      
      const searchInput = screen.getByPlaceholderText('Search avatars...')
      await user.type(searchInput, 'avatar2')
      
      expect(screen.getByText('Professional')).toBeInTheDocument()
      expect(screen.queryByText('Friendly')).not.toBeInTheDocument()
    })
  })

  describe('Mode Selection', () => {
    it('should call setAgentVoice with "none" for text-only', async () => {
      const user = userEvent.setup()
      render(<OutputSelector />)
      await user.click(screen.getByRole('combobox'))
      
      const textOnlyButton = screen.getByRole('option', { name: /text only/i })
      await user.click(textOnlyButton)
      
      expect(mocks.client.setAgentVoice).toHaveBeenCalledWith('none')
    })

    it('should call setAgentVoice with voice ID', async () => {
      mocks.agentCData.data = {
        voices: [{ voice_id: 'alloy', vendor: 'OpenAI', description: 'Test voice' }],
        avatars: []
      }
      mockUseAgentCData.mockReturnValue(mocks.agentCData)
      
      const user = userEvent.setup()
      render(<OutputSelector />)
      await user.click(screen.getByRole('combobox'))
      await user.click(screen.getByRole('tab', { name: /voices/i }))
      
      const voiceOption = screen.getByRole('option', { name: /OpenAI - alloy/i })
      await user.click(voiceOption)
      
      expect(mocks.client.setAgentVoice).toHaveBeenCalledWith('alloy')
    })

    it('should close popover after selection', async () => {
      const user = userEvent.setup()
      render(<OutputSelector />)
      await user.click(screen.getByRole('combobox'))
      
      const textOnlyButton = screen.getByRole('option', { name: /text only/i })
      await user.click(textOnlyButton)
      
      await waitFor(() => {
        expect(document.body.querySelector('[role="listbox"]')).not.toBeInTheDocument()
      })
    })

    it('should show check mark for selected mode', async () => {
      mocks.voiceModel.currentVoice = { voice_id: 'none', vendor: '', description: '' }
      mockUseVoiceModel.mockReturnValue(mocks.voiceModel)
      
      const user = userEvent.setup()
      render(<OutputSelector />)
      await user.click(screen.getByRole('combobox'))
      
      const textOnlyOption = screen.getByRole('option', { name: /text only/i })
      const checkIcon = textOnlyOption.querySelector('.text-primary')
      expect(checkIcon).toBeInTheDocument()
    })

    it('should show check mark for selected voice', async () => {
      mocks.voiceModel.currentVoice = { voice_id: 'alloy', vendor: 'OpenAI', description: 'Test' }
      mocks.agentCData.data = {
        voices: [
          { voice_id: 'alloy', vendor: 'OpenAI', description: 'Test' },
          { voice_id: 'echo', vendor: 'OpenAI', description: 'Other' }
        ],
        avatars: []
      }
      mockUseVoiceModel.mockReturnValue(mocks.voiceModel)
      mockUseAgentCData.mockReturnValue(mocks.agentCData)
      
      const user = userEvent.setup()
      render(<OutputSelector />)
      await user.click(screen.getByRole('combobox'))
      await user.click(screen.getByRole('tab', { name: /voices/i }))
      
      const selectedVoice = screen.getByRole('option', { name: /OpenAI - alloy/i })
      const checkIcon = selectedVoice.querySelector('.text-primary')
      expect(checkIcon).toBeInTheDocument()
      
      const otherVoice = screen.getByRole('option', { name: /OpenAI - echo/i })
      const otherCheckIcon = otherVoice.querySelector('.text-primary')
      expect(otherCheckIcon).not.toBeInTheDocument()
    })
  })

  describe('Keyboard Navigation', () => {
    it('should close popover on Escape key', async () => {
      const user = userEvent.setup()
      render(<OutputSelector />)
      const button = screen.getByRole('combobox')
      
      await user.click(button)
      expect(document.body.querySelector('[role="listbox"]')).toBeInTheDocument()
      
      await user.keyboard('{Escape}')
      expect(document.body.querySelector('[role="listbox"]')).not.toBeInTheDocument()
    })

    it('should return focus to trigger after Escape', async () => {
      const user = userEvent.setup()
      render(<OutputSelector />)
      const button = screen.getByRole('combobox')
      
      await user.click(button)
      await user.keyboard('{Escape}')
      
      expect(button).toHaveFocus()
    })

    it('should focus search input when switching to voices tab', async () => {
      mocks.agentCData.data = {
        voices: [{ voice_id: 'test', vendor: 'Test', description: 'Test' }],
        avatars: []
      }
      mockUseAgentCData.mockReturnValue(mocks.agentCData)
      
      const user = userEvent.setup()
      render(<OutputSelector />)
      await user.click(screen.getByRole('combobox'))
      await user.click(screen.getByRole('tab', { name: /voices/i }))
      
      // Wait for focus (has 100ms delay in implementation)
      await waitFor(() => {
        expect(screen.getByPlaceholderText('Search voices...')).toHaveFocus()
      }, { timeout: 200 })
    })

    it('should focus search input when switching to avatars tab', async () => {
      mocks.agentCData.data = {
        voices: [],
        avatars: [{ avatar_id: 'test', pose_name: 'Test', is_public: true }]
      }
      mockUseAgentCData.mockReturnValue(mocks.agentCData)
      
      const user = userEvent.setup()
      render(<OutputSelector />)
      await user.click(screen.getByRole('combobox'))
      await user.click(screen.getByRole('tab', { name: /avatars/i }))
      
      // Wait for focus (has 100ms delay in implementation)
      await waitFor(() => {
        expect(screen.getByPlaceholderText('Search avatars...')).toHaveFocus()
      }, { timeout: 200 })
    })
  })

  describe('Error Handling', () => {
    it('should display error alert when showErrorAlerts is true', () => {
      mocks.voiceModel.error = 'Failed to load voices'
      mockUseVoiceModel.mockReturnValue(mocks.voiceModel)
      
      render(<OutputSelector showErrorAlerts={true} />)
      expect(screen.getByText('Failed to load voices')).toBeInTheDocument()
    })

    it('should not display error alert when showErrorAlerts is false', () => {
      mocks.voiceModel.error = 'Failed to load voices'
      mockUseVoiceModel.mockReturnValue(mocks.voiceModel)
      
      render(<OutputSelector showErrorAlerts={false} />)
      expect(screen.queryByText('Failed to load voices')).not.toBeInTheDocument()
    })

    it('should show error icon in button on error', () => {
      mocks.voiceModel.error = 'Error'
      mockUseVoiceModel.mockReturnValue(mocks.voiceModel)
      
      render(<OutputSelector />)
      const button = screen.getByRole('combobox')
      const alertIcon = button.querySelector('[class*="h-4"][class*="w-4"]')
      expect(alertIcon).toBeInTheDocument()
    })

    it('should clear error after successful voice change', async () => {
      const { rerender } = render(<OutputSelector showErrorAlerts={true} />)
      
      // Simulate error
      mocks.voiceModel.error = 'Temporary error'
      mockUseVoiceModel.mockReturnValue(mocks.voiceModel)
      rerender(<OutputSelector showErrorAlerts={true} />)
      expect(screen.getByText('Temporary error')).toBeInTheDocument()
      
      // Simulate successful voice change
      const handleVoiceChanged = mocks.client.on.mock.calls.find(
        (call: any[]) => call[0] === 'agent_voice_changed'
      )?.[1]
      
      if (handleVoiceChanged) {
        handleVoiceChanged()
      }
      
      // Error should be cleared
      mocks.voiceModel.error = null
      mockUseVoiceModel.mockReturnValue(mocks.voiceModel)
      rerender(<OutputSelector showErrorAlerts={true} />)
      expect(screen.queryByText('Temporary error')).not.toBeInTheDocument()
    })

    it('should handle setAgentVoice errors gracefully', async () => {
      mocks.client.setAgentVoice.mockRejectedValue(new Error('Network error'))
      
      const user = userEvent.setup()
      render(<OutputSelector showErrorAlerts={true} />)
      await user.click(screen.getByRole('combobox'))
      
      const textOnlyButton = screen.getByRole('option', { name: /text only/i })
      await user.click(textOnlyButton)
      
      await waitFor(() => {
        expect(screen.getByText('Failed to set text mode')).toBeInTheDocument()
      })
    })
  })

  describe('Loading States', () => {
    it('should show loading text when changing', async () => {
      mocks.client.setAgentVoice.mockImplementation(
        () => new Promise(resolve => setTimeout(resolve, 100))
      )
      
      const user = userEvent.setup()
      render(<OutputSelector />)
      await user.click(screen.getByRole('combobox'))
      
      const textOnlyButton = screen.getByRole('option', { name: /text only/i })
      await user.click(textOnlyButton)
      
      expect(screen.getByText('Loading...')).toBeInTheDocument()
    })

    it('should show spinner icon when changing', async () => {
      mocks.client.setAgentVoice.mockImplementation(
        () => new Promise(resolve => setTimeout(resolve, 100))
      )
      
      const user = userEvent.setup()
      render(<OutputSelector />)
      await user.click(screen.getByRole('combobox'))
      
      const textOnlyButton = screen.getByRole('option', { name: /text only/i })
      await user.click(textOnlyButton)
      
      const button = screen.getByRole('combobox')
      const spinner = button.querySelector('.animate-spin')
      expect(spinner).toBeInTheDocument()
    })

    it('should disable button during change', async () => {
      mocks.client.setAgentVoice.mockImplementation(
        () => new Promise(resolve => setTimeout(resolve, 100))
      )
      
      const user = userEvent.setup()
      render(<OutputSelector />)
      const button = screen.getByRole('combobox')
      
      await user.click(button)
      const textOnlyButton = screen.getByRole('option', { name: /text only/i })
      await user.click(textOnlyButton)
      
      expect(button).toBeDisabled()
    })

    it('should re-enable button after change completes', async () => {
      mocks.client.setAgentVoice.mockResolvedValue(undefined)
      
      const user = userEvent.setup()
      render(<OutputSelector />)
      const button = screen.getByRole('combobox')
      
      await user.click(button)
      const textOnlyButton = screen.getByRole('option', { name: /text only/i })
      await user.click(textOnlyButton)
      
      await waitFor(() => {
        expect(button).not.toBeDisabled()
      })
    })
  })

  describe('Connection State', () => {
    it('should disable when not connected', () => {
      mocks.connection.isConnected = false
      mockUseConnection.mockReturnValue(mocks.connection)
      
      render(<OutputSelector />)
      expect(screen.getByRole('combobox')).toBeDisabled()
    })

    it('should enable when connected', () => {
      mocks.connection.isConnected = true
      mockUseConnection.mockReturnValue(mocks.connection)
      
      render(<OutputSelector />)
      expect(screen.getByRole('combobox')).not.toBeDisabled()
    })

    it('should disable when prop disabled is true', () => {
      render(<OutputSelector disabled={true} />)
      expect(screen.getByRole('combobox')).toBeDisabled()
    })

    it('should disable when client is null', () => {
      mockUseRealtimeClientSafe.mockReturnValue(null)
      
      render(<OutputSelector />)
      expect(screen.getByRole('combobox')).toBeDisabled()
    })
  })

  describe('Data Filtering', () => {
    it('should filter out system voices', async () => {
      mocks.agentCData.data = {
        voices: [
          { voice_id: 'none', vendor: 'system', description: '' },
          { voice_id: 'avatar', vendor: 'system', description: '' },
          { voice_id: 'alloy', vendor: 'OpenAI', description: '' }
        ],
        avatars: []
      }
      mockUseAgentCData.mockReturnValue(mocks.agentCData)
      
      const user = userEvent.setup()
      render(<OutputSelector />)
      await user.click(screen.getByRole('combobox'))
      await user.click(screen.getByRole('tab', { name: /voices/i }))
      
      expect(screen.queryByText('system - none')).not.toBeInTheDocument()
      expect(screen.queryByText('system - avatar')).not.toBeInTheDocument()
      expect(screen.getByText('OpenAI - alloy')).toBeInTheDocument()
    })

    it('should only show public avatars', async () => {
      mocks.agentCData.data = {
        voices: [],
        avatars: [
          { avatar_id: 'public1', is_public: true, pose_name: 'Friendly' },
          { avatar_id: 'private1', is_public: false, pose_name: 'Secret' }
        ]
      }
      mockUseAgentCData.mockReturnValue(mocks.agentCData)
      
      const user = userEvent.setup()
      render(<OutputSelector />)
      await user.click(screen.getByRole('combobox'))
      await user.click(screen.getByRole('tab', { name: /avatars/i }))
      
      expect(screen.getByText('Friendly')).toBeInTheDocument()
      expect(screen.queryByText('Secret')).not.toBeInTheDocument()
    })
  })

  describe('Accessibility', () => {
    it('should have proper ARIA attributes on trigger', () => {
      render(<OutputSelector />)
      const button = screen.getByRole('combobox')
      
      expect(button).toHaveAttribute('aria-expanded', 'false')
      expect(button).toHaveAttribute('aria-haspopup', 'listbox')
    })

    it('should update aria-expanded when opened', async () => {
      const user = userEvent.setup()
      render(<OutputSelector />)
      const button = screen.getByRole('combobox')
      
      await user.click(button)
      expect(button).toHaveAttribute('aria-expanded', 'true')
    })

    it('should have custom aria-label when provided', () => {
      render(<OutputSelector ariaLabel="Choose response type" />)
      
      const button = screen.getByRole('combobox')
      expect(button).toHaveAttribute('aria-label', 'Choose response type')
    })

    it('should have default aria-label with current mode', () => {
      render(<OutputSelector />)
      
      const button = screen.getByRole('combobox')
      expect(button).toHaveAttribute('aria-label', 'Output mode selector. Current mode: Text Only')
    })

    it('should have aria-selected on active tab', async () => {
      const user = userEvent.setup()
      render(<OutputSelector />)
      await user.click(screen.getByRole('combobox'))
      
      const modesTab = screen.getByRole('tab', { name: /modes/i })
      expect(modesTab).toHaveAttribute('aria-selected', 'true')
    })

    it('should have live region for announcements', () => {
      const { container } = render(<OutputSelector />)
      const liveRegion = container.querySelector('[role="status"][aria-live="polite"]')
      expect(liveRegion).toBeInTheDocument()
    })

    it('should announce mode changes to screen readers', async () => {
      // Make setAgentVoice take some time so we can check the loading announcement
      mocks.client.setAgentVoice.mockImplementation(
        () => new Promise(resolve => setTimeout(resolve, 50))
      )
      
      const user = userEvent.setup()
      const { container } = render(<OutputSelector />)
      await user.click(screen.getByRole('combobox'))
      
      const textOnlyButton = screen.getByRole('option', { name: /text only/i })
      await user.click(textOnlyButton)
      
      // Check for the loading announcement immediately
      const liveRegion = container.querySelector('[role="status"]')
      expect(liveRegion).toHaveTextContent('Switching to text-only mode...')
      
      // Wait for completion and verify final announcement
      await waitFor(() => {
        expect(liveRegion).toHaveTextContent('Text-only mode activated')
      })
    })
  })

  describe('Quick Navigation', () => {
    it('should navigate from modes to voices tab via Voice option', async () => {
      mocks.agentCData.data = {
        voices: [{ voice_id: 'test', vendor: 'Test', description: '' }],
        avatars: []
      }
      mockUseAgentCData.mockReturnValue(mocks.agentCData)
      
      const user = userEvent.setup()
      render(<OutputSelector />)
      await user.click(screen.getByRole('combobox'))
      
      // Find the Voice mode button (not a direct selection, but navigation)
      const voiceNavButton = screen.getByRole('option', { name: /^Voice/ })
      await user.click(voiceNavButton)
      
      // Should now be on voices tab
      const voicesTab = screen.getByRole('tab', { name: /voices/i })
      expect(voicesTab).toHaveAttribute('aria-selected', 'true')
    })

    it('should navigate from modes to avatars tab via Avatar option', async () => {
      mocks.agentCData.data = {
        voices: [],
        avatars: [{ avatar_id: 'test', pose_name: 'Test', is_public: true }]
      }
      mockUseAgentCData.mockReturnValue(mocks.agentCData)
      
      const user = userEvent.setup()
      render(<OutputSelector />)
      await user.click(screen.getByRole('combobox'))
      
      // Find the Avatar mode button (not a direct selection, but navigation)
      const avatarNavButton = screen.getByRole('option', { name: /^Avatar/ })
      await user.click(avatarNavButton)
      
      // Should now be on avatars tab
      const avatarsTab = screen.getByRole('tab', { name: /avatars/i })
      expect(avatarsTab).toHaveAttribute('aria-selected', 'true')
    })
  })

  describe('Avatar Mode Specifics', () => {
    it('should show coming soon note in avatars tab', async () => {
      mocks.agentCData.data = {
        voices: [],
        avatars: [{ avatar_id: 'test', pose_name: 'Test', is_public: true }]
      }
      mockUseAgentCData.mockReturnValue(mocks.agentCData)
      
      const user = userEvent.setup()
      render(<OutputSelector />)
      await user.click(screen.getByRole('combobox'))
      await user.click(screen.getByRole('tab', { name: /avatars/i }))
      
      expect(screen.getByText('Avatar support coming soon')).toBeInTheDocument()
    })

    it('should announce avatar selection is not available', async () => {
      mocks.agentCData.data = {
        voices: [],
        avatars: [{ avatar_id: 'test', pose_name: 'Test Avatar', is_public: true }]
      }
      mockUseAgentCData.mockReturnValue(mocks.agentCData)
      
      const user = userEvent.setup()
      const { container } = render(<OutputSelector />)
      await user.click(screen.getByRole('combobox'))
      await user.click(screen.getByRole('tab', { name: /avatars/i }))
      
      const avatarOption = screen.getByRole('option', { name: /Test Avatar/i })
      await user.click(avatarOption)
      
      const liveRegion = container.querySelector('[role="status"]')
      expect(liveRegion).toHaveTextContent('Avatar Test Avatar selection is not yet available')
    })
  })

  describe('Complex Interaction Scenarios', () => {
    it('should reset search when switching tabs', async () => {
      mocks.agentCData.data = {
        voices: [
          { voice_id: 'voice1', vendor: 'Test', description: 'Voice 1' },
          { voice_id: 'voice2', vendor: 'Test', description: 'Voice 2' }
        ],
        avatars: [
          { avatar_id: 'avatar1', pose_name: 'Avatar 1', is_public: true }
        ]
      }
      mockUseAgentCData.mockReturnValue(mocks.agentCData)
      
      const user = userEvent.setup()
      render(<OutputSelector />)
      await user.click(screen.getByRole('combobox'))
      
      // Go to voices tab and search
      await user.click(screen.getByRole('tab', { name: /voices/i }))
      const voiceSearch = screen.getByPlaceholderText('Search voices...')
      await user.type(voiceSearch, 'test search')
      
      // Switch to avatars tab
      await user.click(screen.getByRole('tab', { name: /avatars/i }))
      
      // Search should be reset
      const avatarSearch = screen.getByPlaceholderText('Search avatars...')
      expect(avatarSearch).toHaveValue('')
    })

    it('should handle rapid mode changes gracefully', async () => {
      // Make setAgentVoice take time to complete
      mocks.client.setAgentVoice.mockImplementation(() => {
        return new Promise(resolve => {
          setTimeout(() => resolve(undefined), 100)
        })
      })
      
      const user = userEvent.setup()
      render(<OutputSelector />)
      
      // Open popover and click text-only
      await user.click(screen.getByRole('combobox'))
      const textOnlyButton = screen.getByRole('option', { name: /text only/i })
      await user.click(textOnlyButton)
      
      // Try to rapidly click again while first change is in progress
      // The button should be disabled preventing additional calls
      const button = screen.getByRole('combobox')
      expect(button).toBeDisabled()
      
      // Wait for the first call to complete
      await waitFor(() => {
        expect(button).not.toBeDisabled()
      })
      
      // Verify only one call was made
      expect(mocks.client.setAgentVoice).toHaveBeenCalledTimes(1)
    })

    it('should maintain popover state during error', async () => {
      mocks.client.setAgentVoice.mockRejectedValue(new Error('Network error'))
      
      const user = userEvent.setup()
      render(<OutputSelector />)
      await user.click(screen.getByRole('combobox'))
      
      const textOnlyButton = screen.getByRole('option', { name: /text only/i })
      await user.click(textOnlyButton)
      
      // Popover should close even on error
      await waitFor(() => {
        expect(document.body.querySelector('[role="listbox"]')).not.toBeInTheDocument()
      })
    })
  })
})