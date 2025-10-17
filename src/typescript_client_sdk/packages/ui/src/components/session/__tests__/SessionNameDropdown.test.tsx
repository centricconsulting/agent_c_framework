import { describe, it, expect, vi, beforeEach, afterEach } from 'vitest'
import { render, screen, fireEvent, waitFor, act, within } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import { SessionNameDropdown } from '../SessionNameDropdown'
import type { RealtimeClient } from '@agentc/realtime-core'

// Mock the React hook
vi.mock('@agentc/realtime-react', () => ({
  useRealtimeClientSafe: vi.fn()
}))

// Mock sonner toast
vi.mock('sonner', () => ({
  toast: {
    success: vi.fn(),
    error: vi.fn()
  }
}))

import { useRealtimeClientSafe } from '@agentc/realtime-react'
import { toast } from 'sonner'

describe('SessionNameDropdown', () => {
  let mockClient: Partial<RealtimeClient>
  const mockUseRealtimeClientSafe = vi.mocked(useRealtimeClientSafe)
  const mockToast = vi.mocked(toast)
  
  beforeEach(() => {
    // Reset all mocks
    vi.clearAllMocks()
    
    // Create mock SessionManager
    const mockSessionManager = {
      getCurrentSession: vi.fn().mockReturnValue(null),
      getSessionIds: vi.fn().mockReturnValue([]),
      on: vi.fn(),
      off: vi.fn()
    }
    
    // Create mock client
    mockClient = {
      on: vi.fn(),
      off: vi.fn(),
      sendEvent: vi.fn().mockResolvedValue(undefined),
      getSessionManager: vi.fn().mockReturnValue(mockSessionManager)
    }
    
    // Set up default mock return
    mockUseRealtimeClientSafe.mockReturnValue(mockClient as RealtimeClient)
  })
  
  afterEach(() => {
    vi.restoreAllMocks()
  })
  
  it('should render with default session name', () => {
    render(<SessionNameDropdown />)
    expect(screen.getByText('New Chat')).toBeInTheDocument()
  })
  
  it('should render with provided session name', () => {
    render(<SessionNameDropdown sessionName="My Session" />)
    expect(screen.getByText('My Session')).toBeInTheDocument()
  })
  
  it('should open dropdown menu when clicked', async () => {
    render(<SessionNameDropdown />)
    const trigger = screen.getByRole('button', { name: /session options/i })
    
    await userEvent.click(trigger)
    
    expect(screen.getByText('Rename session')).toBeInTheDocument()
    expect(screen.getByText('Delete session')).toBeInTheDocument()
  })
  
  it('should disable menu items when client is not available', async () => {
    mockUseRealtimeClientSafe.mockReturnValue(null)
    
    render(<SessionNameDropdown />)
    const trigger = screen.getByRole('button', { name: /session options/i })
    
    await userEvent.click(trigger)
    
    const renameItem = screen.getByRole('menuitem', { name: /rename session/i })
    const deleteItem = screen.getByRole('menuitem', { name: /delete session/i })
    
    expect(renameItem).toHaveAttribute('aria-disabled', 'true')
    expect(deleteItem).toHaveAttribute('aria-disabled', 'true')
  })
  
  it('should get initial session from SessionManager on mount', () => {
    // Set up mock session manager to return a current session
    const mockCurrentSession = {
      session_id: 'initial-session-123',
      session_name: 'Initial Session Name',
      display_name: 'Initial Display Name',
      messages: [],
      version: 1,
      token_count: 0,
      context_window_size: 0,
      vendor: 'openai'
    }
    
    const mockSessionManager = {
      getCurrentSession: vi.fn().mockReturnValue(mockCurrentSession),
      getSessionIds: vi.fn().mockReturnValue(['initial-session-123']),
      on: vi.fn(),
      off: vi.fn()
    }
    
    vi.mocked(mockClient.getSessionManager!).mockReturnValue(mockSessionManager as any)
    
    render(<SessionNameDropdown />)
    
    // Verify that getSessionManager was called
    expect(mockClient.getSessionManager).toHaveBeenCalled()
    expect(mockSessionManager.getCurrentSession).toHaveBeenCalled()
    
    // Verify the initial session name is displayed
    expect(screen.getByText('Initial Session Name')).toBeInTheDocument()
  })
  
  it('should use display_name when session_name is null', () => {
    // Set up mock session with null session_name but valid display_name
    const mockCurrentSession = {
      session_id: 'display-name-session',
      session_name: null,
      display_name: 'Fallback Display Name',
      messages: [],
      version: 1,
      token_count: 0,
      context_window_size: 0,
      vendor: 'openai'
    }
    
    const mockSessionManager = {
      getCurrentSession: vi.fn().mockReturnValue(mockCurrentSession),
      getSessionIds: vi.fn().mockReturnValue(['display-name-session']),
      on: vi.fn(),
      off: vi.fn()
    }
    
    vi.mocked(mockClient.getSessionManager!).mockReturnValue(mockSessionManager as any)
    
    render(<SessionNameDropdown />)
    
    // Verify the display_name is shown when session_name is null
    expect(screen.getByText('Fallback Display Name')).toBeInTheDocument()
  })
  
  it('should handle session changed events', () => {
    const mockSessionManager = {
      getCurrentSession: vi.fn().mockReturnValue(null),
      on: vi.fn(),
      off: vi.fn()
    }
    
    vi.mocked(mockClient.getSessionManager!).mockReturnValue(mockSessionManager as any)
    
    render(<SessionNameDropdown />)
    
    // Verify event listeners were registered on SessionManager and client
    expect(mockSessionManager.on).toHaveBeenCalledWith('chat-session-changed', expect.any(Function))
    expect(mockClient.on).toHaveBeenCalledWith('chat_session_name_changed', expect.any(Function))
  })
  
  it('should update session name from SessionManager chat-session-changed event', async () => {
    const mockSessionManager = {
      getCurrentSession: vi.fn().mockReturnValue(null),
      on: vi.fn(),
      off: vi.fn()
    }
    
    vi.mocked(mockClient.getSessionManager!).mockReturnValue(mockSessionManager as any)
    
    const { rerender } = render(<SessionNameDropdown />)
    
    // Get the event handler for chat-session-changed
    const [[, handler]] = vi.mocked(mockSessionManager.on).mock.calls.filter(
      ([event]) => event === 'chat-session-changed'
    )
    
    // Simulate SessionManager chat-session-changed event
    await act(async () => {
      handler({
        currentChatSession: {
          session_id: 'test-123',
          session_name: 'Updated Session',
          messages: [],
          version: 1,
          token_count: 0,
          context_window_size: 0,
          vendor: 'openai',
          display_name: 'Display Name'
        },
        previousChatSession: null
      })
    })
    
    // Force re-render to see updated state
    rerender(<SessionNameDropdown />)
    
    await waitFor(() => {
      expect(screen.getByText('Updated Session')).toBeInTheDocument()
    })
  })
  
  it('should open rename dialog when rename is clicked', async () => {
    render(<SessionNameDropdown sessionName="Current Name" />)
    
    // Open dropdown
    const trigger = screen.getByRole('button', { name: /session options/i })
    await userEvent.click(trigger)
    
    // Click rename
    const renameItem = screen.getByRole('menuitem', { name: /rename session/i })
    await userEvent.click(renameItem)
    
    // Check dialog opened
    expect(screen.getByRole('alertdialog')).toBeInTheDocument()
    expect(screen.getByText('Rename Session')).toBeInTheDocument()
    expect(screen.getByDisplayValue('Current Name')).toBeInTheDocument()
  })
  
  it('should send rename event when confirmed', async () => {
    render(<SessionNameDropdown sessionName="Old Name" />)
    
    // Open dropdown and click rename
    await userEvent.click(screen.getByRole('button', { name: /session options/i }))
    await userEvent.click(screen.getByRole('menuitem', { name: /rename session/i }))
    
    // Change name in input
    const input = screen.getByLabelText(/session name/i)
    await userEvent.clear(input)
    await userEvent.type(input, 'New Name')
    
    // Click rename button
    const renameButton = screen.getByRole('button', { name: /^rename$/i })
    await userEvent.click(renameButton)
    
    // Verify event was sent
    await waitFor(() => {
      expect(mockClient.sendEvent).toHaveBeenCalledWith({
        type: 'set_chat_session_name',
        session_name: 'New Name'
      })
    })
    
    // Verify success toast
    expect(mockToast.success).toHaveBeenCalledWith('Session renamed successfully')
  })
  
  it('should open delete confirmation dialog when delete is clicked', async () => {
    render(<SessionNameDropdown sessionName="Session to Delete" />)
    
    // Open dropdown
    await userEvent.click(screen.getByRole('button', { name: /session options/i }))
    
    // Click delete
    const deleteItem = screen.getByRole('menuitem', { name: /delete session/i })
    await userEvent.click(deleteItem)
    
    // Check confirmation dialog
    const dialog = screen.getByRole('alertdialog')
    expect(dialog).toBeInTheDocument()
    
    // Check dialog content - use within to scope queries to the dialog
    const { getByRole, getByText } = within(dialog)
    expect(getByRole('heading', { name: /delete session/i })).toBeInTheDocument()
    // The session name appears both in the button and in the dialog, so we check within the dialog specifically
    expect(getByText(/Session to Delete/)).toBeInTheDocument()
    expect(getByText(/this action cannot be undone/i)).toBeInTheDocument()
  })
  
  it('should send delete event when confirmed', async () => {
    render(<SessionNameDropdown sessionName="Session to Delete" />)
    
    // Open dropdown and click delete
    await userEvent.click(screen.getByRole('button', { name: /session options/i }))
    await userEvent.click(screen.getByRole('menuitem', { name: /delete session/i }))
    
    // Click delete button in confirmation
    const deleteButton = screen.getByRole('button', { name: /delete session$/i })
    await userEvent.click(deleteButton)
    
    // Verify event was sent
    await waitFor(() => {
      expect(mockClient.sendEvent).toHaveBeenCalledWith({
        type: 'delete_chat_session'
      })
    })
    
    // Verify success toast
    expect(mockToast.success).toHaveBeenCalledWith('Session deleted successfully')
  })
  
  it('should handle errors gracefully', async () => {
    // Make sendEvent reject
    vi.mocked(mockClient.sendEvent!).mockRejectedValue(new Error('Network error'))
    
    render(<SessionNameDropdown sessionName="Test Session" />)
    
    // Try to rename
    await userEvent.click(screen.getByRole('button', { name: /session options/i }))
    await userEvent.click(screen.getByRole('menuitem', { name: /rename session/i }))
    
    const input = screen.getByLabelText(/session name/i)
    await userEvent.clear(input)
    await userEvent.type(input, 'New Name')
    
    await userEvent.click(screen.getByRole('button', { name: /^rename$/i }))
    
    // Verify error toast
    await waitFor(() => {
      expect(mockToast.error).toHaveBeenCalledWith('Failed to rename session')
    })
  })
  
  it('should unsubscribe from events on unmount', () => {
    const mockSessionManager = {
      getCurrentSession: vi.fn().mockReturnValue(null),
      on: vi.fn(),
      off: vi.fn()
    }
    
    vi.mocked(mockClient.getSessionManager!).mockReturnValue(mockSessionManager as any)
    
    const { unmount } = render(<SessionNameDropdown />)
    
    // Verify subscriptions
    expect(mockSessionManager.on).toHaveBeenCalled()
    expect(mockClient.on).toHaveBeenCalled()
    
    // Unmount
    unmount()
    
    // Verify cleanup
    expect(mockSessionManager.off).toHaveBeenCalledWith('chat-session-changed', expect.any(Function))
    expect(mockClient.off).toHaveBeenCalledWith('chat_session_name_changed', expect.any(Function))
  })
})