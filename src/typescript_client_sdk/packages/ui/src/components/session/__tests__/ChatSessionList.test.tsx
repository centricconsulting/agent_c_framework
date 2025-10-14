import { describe, it, expect, vi, beforeEach, afterEach } from 'vitest'
import { render, screen, within, waitFor } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import { ChatSessionList } from '../ChatSessionList'
import type { ChatSessionIndexEntry, SessionGroupMeta } from '@agentc/realtime-react'

// Mock the React hook
vi.mock('@agentc/realtime-react', () => ({
  useChatSessionList: vi.fn()
}))

import { useChatSessionList } from '@agentc/realtime-react'

// Helper to set viewport size for virtual scrolling tests
function setViewportSize(width: number, height: number) {
  Object.defineProperty(window, 'innerWidth', { writable: true, value: width })
  Object.defineProperty(window, 'innerHeight', { writable: true, value: height })
}

// Helper to create mock session data
function createMockSession(id: string, name: string, updatedAt: string): ChatSessionIndexEntry {
  return {
    session_id: id,
    session_name: name,
    created_at: new Date(Date.now() - 86400000).toISOString(),
    updated_at: updatedAt,
    user_id: 'test-user',
    agent_key: 'test-agent',
    agent_name: 'Test Agent',
  }
}

// Helper to create session groups
function createSessionGroups(sessions: ChatSessionIndexEntry[]): SessionGroupMeta[] {
  return [
    {
      group: 'today',
      label: 'Today',
      count: sessions.length,
      sessions: sessions
    }
  ]
}

describe('ChatSessionList - Split Highlighting Issue', () => {
  const mockUseChatSessionList = vi.mocked(useChatSessionList)
  
  beforeEach(() => {
    vi.clearAllMocks()
    // Set viewport size for virtual scrolling
    setViewportSize(1024, 768)
  })
  
  afterEach(() => {
    vi.restoreAllMocks()
  })
  
  it('should NOT highlight any session when currentSessionId points to a session NOT in the list', async () => {
    // Create test sessions
    const sessions = [
      createMockSession('session-1', 'First Session', new Date().toISOString()),
      createMockSession('session-2', 'Second Session', new Date().toISOString()),
      createMockSession('session-3', 'Third Session', new Date().toISOString()),
    ]
    
    const sessionGroups = createSessionGroups(sessions)
    
    // Mock hook return with currentSessionId pointing to a session NOT in the list
    mockUseChatSessionList.mockReturnValue({
      sessions,
      filteredSessions: sessions,
      groupedSessions: {
        today: sessions,
        recent: [],
        past: []
      },
      sessionGroups,
      searchQuery: '',
      isLoading: false,
      isPaginationLoading: false,
      error: null,
      hasMore: false,
      totalCount: sessions.length,
      currentSessionId: 'new-session-not-in-list', // <-- THIS IS THE KEY
      loadMore: vi.fn(),
      selectSession: vi.fn(),
      deleteSession: vi.fn(),
      searchSessions: vi.fn(),
      refresh: vi.fn()
    })
    
    const { container } = render(<ChatSessionList />)
    
    // Wait for virtual scrolling to initialize and render items
    await waitFor(() => {
      const sessionItems = container.querySelectorAll('[role="option"]')
      expect(sessionItems.length).toBeGreaterThan(0)
    })
    
    // Get all session items
    const sessionItems = container.querySelectorAll('[role="option"]')
    expect(sessionItems).toHaveLength(3)
    
    // CRITICAL CHECK: None of the sessions should have bg-accent class
    sessionItems.forEach((item, index) => {
      const classList = Array.from(item.classList)
      
      // Check that bg-accent is NOT present
      expect(classList.includes('bg-accent')).toBe(false)
      
      console.log(`Session ${index + 1} classes:`, classList)
    })
  })
  
  it('should highlight the correct session when currentSessionId matches a session in the list', async () => {
    const sessions = [
      createMockSession('session-1', 'First Session', new Date().toISOString()),
      createMockSession('session-2', 'Second Session', new Date().toISOString()),
      createMockSession('session-3', 'Third Session', new Date().toISOString()),
    ]
    
    const sessionGroups = createSessionGroups(sessions)
    
    // Mock hook with currentSessionId matching session-2
    mockUseChatSessionList.mockReturnValue({
      sessions,
      filteredSessions: sessions,
      groupedSessions: {
        today: sessions,
        recent: [],
        past: []
      },
      sessionGroups,
      searchQuery: '',
      isLoading: false,
      isPaginationLoading: false,
      error: null,
      hasMore: false,
      totalCount: sessions.length,
      currentSessionId: 'session-2', // <-- Should match second session
      loadMore: vi.fn(),
      selectSession: vi.fn(),
      deleteSession: vi.fn(),
      searchSessions: vi.fn(),
      refresh: vi.fn()
    })
    
    const { container } = render(<ChatSessionList />)
    
    // Wait for virtual scrolling
    await waitFor(() => {
      const items = container.querySelectorAll('[role="option"]')
      expect(items.length).toBeGreaterThan(0)
    })
    
    const sessionItems = container.querySelectorAll('[role="option"]')
    
    // Check that only session-2 has bg-accent
    sessionItems.forEach((item, index) => {
      const classList = Array.from(item.classList)
      const sessionName = within(item as HTMLElement).getByText(sessions[index].session_name)
      
      if (sessions[index].session_id === 'session-2') {
        expect(classList.includes('bg-accent')).toBe(true)
        console.log(`✓ Session ${index + 1} (${sessions[index].session_name}) correctly has bg-accent`)
      } else {
        expect(classList.includes('bg-accent')).toBe(false)
        console.log(`✓ Session ${index + 1} (${sessions[index].session_name}) correctly does NOT have bg-accent`)
      }
    })
  })
  
  it('should demonstrate the split highlighting bug scenario', async () => {
    // SCENARIO: 
    // 1. User has session-2 selected
    // 2. User creates a new session (not in list yet)
    // 3. currentSessionId changes to the new session
    // 4. Expected: No bg-accent on any session
    // 5. Bug: First session gets bg-accent
    
    const sessions = [
      createMockSession('session-1', 'First Session', new Date().toISOString()),
      createMockSession('session-2', 'Second Session', new Date().toISOString()),
      createMockSession('session-3', 'Third Session', new Date().toISOString()),
    ]
    
    const sessionGroups = createSessionGroups(sessions)
    
    // Initial render with session-2 selected
    mockUseChatSessionList.mockReturnValue({
      sessions,
      filteredSessions: sessions,
      groupedSessions: {
        today: sessions,
        recent: [],
        past: []
      },
      sessionGroups,
      searchQuery: '',
      isLoading: false,
      isPaginationLoading: false,
      error: null,
      hasMore: false,
      totalCount: sessions.length,
      currentSessionId: 'session-2',
      loadMore: vi.fn(),
      selectSession: vi.fn(),
      deleteSession: vi.fn(),
      searchSessions: vi.fn(),
      refresh: vi.fn()
    })
    
    const { container, rerender } = render(<ChatSessionList />)
    
    // Wait for initial render
    await waitFor(() => {
      const items = container.querySelectorAll('[role="option"]')
      expect(items.length).toBeGreaterThan(0)
    })
    
    // Verify session-2 is highlighted
    let sessionItems = container.querySelectorAll('[role="option"]')
    let session2Item = sessionItems[1] // Second item
    expect(Array.from(session2Item.classList).includes('bg-accent')).toBe(true)
    console.log('✓ Initial state: session-2 is correctly highlighted')
    
    // NOW: Simulate new session creation (not in list)
    mockUseChatSessionList.mockReturnValue({
      sessions, // List unchanged
      filteredSessions: sessions,
      groupedSessions: {
        today: sessions,
        recent: [],
        past: []
      },
      sessionGroups,
      searchQuery: '',
      isLoading: false,
      isPaginationLoading: false,
      error: null,
      hasMore: false,
      totalCount: sessions.length,
      currentSessionId: 'new-session-999', // Changed to new session not in list
      loadMore: vi.fn(),
      selectSession: vi.fn(),
      deleteSession: vi.fn(),
      searchSessions: vi.fn(),
      refresh: vi.fn()
    })
    
    rerender(<ChatSessionList />)
    
    // Wait for re-render
    await waitFor(() => {
      const items = container.querySelectorAll('[role="option"]')
      expect(items.length).toBeGreaterThan(0)
    })
    
    // Check that NO session has bg-accent
    sessionItems = container.querySelectorAll('[role="option"]')
    sessionItems.forEach((item, index) => {
      const classList = Array.from(item.classList)
      const hasBgAccent = classList.includes('bg-accent')
      
      console.log(`Session ${index + 1} after new session created:`, {
        hasBgAccent,
        classes: classList.filter(c => c.includes('bg-') || c.includes('ring'))
      })
      
      // THIS IS THE TEST THAT SHOULD PASS BUT MIGHT FAIL IF THE BUG EXISTS
      expect(hasBgAccent).toBe(false)
    })
    
    console.log('✓ After new session creation: NO session has bg-accent (expected behavior)')
  })
  
  it('should isolate focusedIndex behavior from isActive behavior', async () => {
    const sessions = [
      createMockSession('session-1', 'First Session', new Date().toISOString()),
      createMockSession('session-2', 'Second Session', new Date().toISOString()),
    ]
    
    const sessionGroups = createSessionGroups(sessions)
    
    mockUseChatSessionList.mockReturnValue({
      sessions,
      filteredSessions: sessions,
      groupedSessions: {
        today: sessions,
        recent: [],
        past: []
      },
      sessionGroups,
      searchQuery: '',
      isLoading: false,
      isPaginationLoading: false,
      error: null,
      hasMore: false,
      totalCount: sessions.length,
      currentSessionId: 'new-session-not-in-list',
      loadMore: vi.fn(),
      selectSession: vi.fn(),
      deleteSession: vi.fn(),
      searchSessions: vi.fn(),
      refresh: vi.fn()
    })
    
    const { container } = render(<ChatSessionList />)
    
    // Wait for virtual scrolling
    await waitFor(() => {
      const items = container.querySelectorAll('[role="option"]')
      expect(items.length).toBeGreaterThan(0)
    })
    
    // Both isActive (bg-accent) and isFocused (ring) should be independent
    // Since currentSessionId doesn't match any session:
    // - NO session should have bg-accent (isActive)
    // - focusedIndex controls ring separately
    
    const sessionItems = container.querySelectorAll('[role="option"]')
    
    sessionItems.forEach((item) => {
      const classList = Array.from(item.classList)
      
      // Check bg-accent (isActive)
      const hasBgAccent = classList.includes('bg-accent')
      
      // Check ring (isFocused) - check for ring-2 class
      const hasRing = classList.some(c => c.startsWith('ring-'))
      
      console.log('Classes:', {
        hasBgAccent,
        hasRing,
        allClasses: classList
      })
      
      // Neither should be active when currentSessionId doesn't match
      expect(hasBgAccent).toBe(false)
    })
  })
})
