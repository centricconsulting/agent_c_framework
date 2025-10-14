import { describe, it, expect, vi } from 'vitest'
import { render, screen } from '@testing-library/react'
import { SessionItem } from '../ChatSessionList'
import type { ChatSessionIndexEntry } from '@agentc/realtime-react'

// Create a mock session for testing
function createMockSession(id: string, name: string): ChatSessionIndexEntry {
  return {
    session_id: id,
    session_name: name,
    created_at: new Date().toISOString(),
    updated_at: new Date().toISOString(),
    user_id: 'test-user',
    agent_key: 'test-agent',
    agent_name: 'Test Agent',
  }
}

describe('SessionItem - Highlighting Logic', () => {
  it('should have bg-accent when isActive is true', () => {
    const session = createMockSession('session-1', 'Test Session')
    
    const { container } = render(
      <SessionItem
        session={session}
        isActive={true}
        isFocused={false}
        onSelect={vi.fn()}
        onDelete={vi.fn()}
        onFocus={vi.fn()}
        isDeleting={false}
        index={0}
        totalCount={1}
      />
    )
    
    const sessionItem = container.querySelector('[role="option"]')
    expect(sessionItem).toHaveClass('bg-accent')
  })
  
  it('should NOT have bg-accent when isActive is false', () => {
    const session = createMockSession('session-1', 'Test Session')
    
    const { container } = render(
      <SessionItem
        session={session}
        isActive={false}
        isFocused={false}
        onSelect={vi.fn()}
        onDelete={vi.fn()}
        onFocus={vi.fn()}
        isDeleting={false}
        index={0}
        totalCount={1}
      />
    )
    
    const sessionItem = container.querySelector('[role="option"]')
    expect(sessionItem).not.toHaveClass('bg-accent')
  })
  
  it('should have ring when isFocused is true', () => {
    const session = createMockSession('session-1', 'Test Session')
    
    const { container } = render(
      <SessionItem
        session={session}
        isActive={false}
        isFocused={true}
        onSelect={vi.fn()}
        onDelete={vi.fn()}
        onFocus={vi.fn()}
        isDeleting={false}
        index={0}
        totalCount={1}
      />
    )
    
    const sessionItem = container.querySelector('[role="option"]')
    const classList = Array.from(sessionItem?.classList || [])
    
    // Should have ring-2 class
    expect(classList.some(c => c.includes('ring-2'))).toBe(true)
  })
  
  it('should correctly handle split state: isActive but not isFocused', () => {
    const session = createMockSession('session-1', 'Test Session')
    
    const { container } = render(
      <SessionItem
        session={session}
        isActive={true}
        isFocused={false}
        onSelect={vi.fn()}
        onDelete={vi.fn()}
        onFocus={vi.fn()}
        isDeleting={false}
        index={0}
        totalCount={1}
      />
    )
    
    const sessionItem = container.querySelector('[role="option"]')
    const classList = Array.from(sessionItem?.classList || [])
    
    // Should have bg-accent but NOT ring
    expect(classList.includes('bg-accent')).toBe(true)
    expect(classList.some(c => c.includes('ring-2'))).toBe(false)
  })
  
  it('should correctly handle opposite split: isFocused but not isActive', () => {
    const session = createMockSession('session-1', 'Test Session')
    
    const { container } = render(
      <SessionItem
        session={session}
        isActive={false}
        isFocused={true}
        onSelect={vi.fn()}
        onDelete={vi.fn()}
        onFocus={vi.fn()}
        isDeleting={false}
        index={0}
        totalCount={1}
      />
    )
    
    const sessionItem = container.querySelector('[role="option"]')
    const classList = Array.from(sessionItem?.classList || [])
    
    // Should have ring but NOT bg-accent
    expect(classList.some(c => c.includes('ring-2'))).toBe(true)
    expect(classList.includes('bg-accent')).toBe(false)
  })
  
  it('should have neither when both isActive and isFocused are false', () => {
    const session = createMockSession('session-1', 'Test Session')
    
    const { container } = render(
      <SessionItem
        session={session}
        isActive={false}
        isFocused={false}
        onSelect={vi.fn()}
        onDelete={vi.fn()}
        onFocus={vi.fn()}
        isDeleting={false}
        index={0}
        totalCount={1}
      />
    )
    
    const sessionItem = container.querySelector('[role="option"]')
    const classList = Array.from(sessionItem?.classList || [])
    
    // Should have neither
    expect(classList.includes('bg-accent')).toBe(false)
    expect(classList.some(c => c.includes('ring-2'))).toBe(false)
  })
})

describe('SessionItem Component - isActive Calculation Logic', () => {
  it('demonstrates correct isActive calculation when currentSessionId matches', () => {
    const session = createMockSession('session-1', 'Test Session')
    const currentSessionId = 'session-1'
    
    // This is the calculation from ChatSessionList.tsx
    const isActive = session.session_id === currentSessionId
    
    expect(isActive).toBe(true)
    
    const { container } = render(
      <SessionItem
        session={session}
        isActive={isActive}
        isFocused={false}
        onSelect={vi.fn()}
        onDelete={vi.fn()}
        onFocus={vi.fn()}
        isDeleting={false}
        index={0}
        totalCount={1}
      />
    )
    
    expect(container.querySelector('[role="option"]')).toHaveClass('bg-accent')
  })
  
  it('demonstrates correct isActive calculation when currentSessionId does NOT match', () => {
    const session = createMockSession('session-1', 'Test Session')
    const currentSessionId = 'different-session-id'
    
    // This is the calculation from ChatSessionList.tsx
    const isActive = session.session_id === currentSessionId
    
    expect(isActive).toBe(false)
    
    const { container } = render(
      <SessionItem
        session={session}
        isActive={isActive}
        isFocused={false}
        onSelect={vi.fn()}
        onDelete={vi.fn()}
        onFocus={vi.fn()}
        isDeleting={false}
        index={0}
        totalCount={1}
      />
    )
    
    expect(container.querySelector('[role="option"]')).not.toHaveClass('bg-accent')
  })
  
  it('demonstrates the expected behavior when currentSessionId points to a session NOT in the list', () => {
    // Scenario: User has 3 sessions, none should be highlighted when currentSessionId = 'new-session-999'
    const sessions = [
      createMockSession('session-1', 'First Session'),
      createMockSession('session-2', 'Second Session'),
      createMockSession('session-3', 'Third Session'),
    ]
    
    const currentSessionId = 'new-session-999' // Not in the list
    
    sessions.forEach((session) => {
      const isActive = session.session_id === currentSessionId
      expect(isActive).toBe(false) // None should match
      
      const { container } = render(
        <SessionItem
          session={session}
          isActive={isActive}
          isFocused={false}
          onSelect={vi.fn()}
          onDelete={vi.fn()}
          onFocus={vi.fn()}
          isDeleting={false}
          index={0}
          totalCount={3}
        />
      )
      
      expect(container.querySelector('[role="option"]')).not.toHaveClass('bg-accent')
    })
  })
})
