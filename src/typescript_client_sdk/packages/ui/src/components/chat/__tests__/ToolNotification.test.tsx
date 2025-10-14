import React from 'react'
import { render, screen, waitFor } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import { describe, it, expect, vi, beforeEach } from 'vitest'
import { ToolNotification, ToolNotificationList } from '../ToolNotification'
import type { ToolNotificationData as ToolNotificationType } from '../ToolNotification'

describe('ToolNotification', () => {
  const mockNotification: ToolNotificationType = {
    id: 'tool-1',
    toolName: 'test_tool',
    status: 'preparing',
    timestamp: new Date('2024-01-01T12:00:00Z'),
    arguments: '{"param": "value"}'
  }
  
  describe('Status Messages', () => {
    it('should display preparing status correctly', () => {
      render(<ToolNotification notification={mockNotification} />)
      
      expect(screen.getByText('Agent is preparing to use test_tool')).toBeInTheDocument()
      expect(screen.getByRole('status')).toHaveAttribute('aria-label', 'Agent is preparing to use test_tool')
    })
    
    it('should display executing status correctly', () => {
      const executingNotification = { ...mockNotification, status: 'executing' as const }
      render(<ToolNotification notification={executingNotification} />)
      
      expect(screen.getByText('Agent is using test_tool')).toBeInTheDocument()
      expect(screen.getByRole('status')).toHaveAttribute('aria-label', 'Agent is using test_tool')
    })
    
    it('should display complete status correctly', () => {
      const completeNotification = { ...mockNotification, status: 'complete' as const }
      render(<ToolNotification notification={completeNotification} />)
      
      expect(screen.getByText('Completed test_tool')).toBeInTheDocument()
      expect(screen.getByRole('status')).toHaveAttribute('aria-label', 'Completed test_tool')
    })
  })
  
  describe('Think Tool Special Handling', () => {
    const thinkNotification: ToolNotificationType = {
      id: 'think-1',
      toolName: 'think',
      status: 'preparing',
      timestamp: new Date('2024-01-01T12:00:00Z')
    }
    
    it('should display think tool preparing status', () => {
      render(<ToolNotification notification={thinkNotification} />)
      
      expect(screen.getByText('Agent is thinking...')).toBeInTheDocument()
      expect(screen.getByRole('status')).toHaveAttribute('aria-label', 'Agent is thinking...')
    })
    
    it('should display think tool executing status', () => {
      const executingThink = { ...thinkNotification, status: 'executing' as const }
      render(<ToolNotification notification={executingThink} />)
      
      expect(screen.getByText('Agent is thinking...')).toBeInTheDocument()
    })
    
    it('should display think tool complete status', () => {
      const completeThink = { ...thinkNotification, status: 'complete' as const }
      render(<ToolNotification notification={completeThink} />)
      
      expect(screen.getByText('Thought process complete')).toBeInTheDocument()
    })
    
    it('should recognize think tool via isThinkTool prop', () => {
      const notification = { ...mockNotification, toolName: 'other_tool' }
      render(<ToolNotification notification={notification} isThinkTool={true} />)
      
      expect(screen.getByText('Agent is thinking...')).toBeInTheDocument()
    })
  })
  
  describe('Visual Styling', () => {
    it('should apply preparing status styles', () => {
      render(<ToolNotification notification={mockNotification} />)
      
      const statusElement = screen.getByRole('status')
      expect(statusElement).toHaveClass('bg-muted/50', 'border-border/50', 'text-muted-foreground')
    })
    
    it('should apply executing status styles', () => {
      const executingNotification = { ...mockNotification, status: 'executing' as const }
      render(<ToolNotification notification={executingNotification} />)
      
      const statusElement = screen.getByRole('status')
      expect(statusElement).toHaveClass('bg-primary/5', 'border-primary/20', 'text-foreground')
    })
    
    it('should apply complete status styles', () => {
      const completeNotification = { ...mockNotification, status: 'complete' as const }
      render(<ToolNotification notification={completeNotification} />)
      
      const statusElement = screen.getByRole('status')
      expect(statusElement).toHaveClass('bg-green-500/5', 'border-green-500/20')
    })
  })
  
  describe('Animation', () => {
    it('should animate icon for preparing status', () => {
      render(<ToolNotification notification={mockNotification} />)
      
      const icon = document.querySelector('.animate-spin')
      expect(icon).toBeInTheDocument()
    })
    
    it('should animate icon for executing status', () => {
      const executingNotification = { ...mockNotification, status: 'executing' as const }
      render(<ToolNotification notification={executingNotification} />)
      
      const icon = document.querySelector('.animate-spin')
      expect(icon).toBeInTheDocument()
    })
    
    it('should use pulse animation for think tool', () => {
      const thinkNotification = { ...mockNotification, toolName: 'think' }
      render(<ToolNotification notification={thinkNotification} />)
      
      // For think tool, the Brain icon should get animate-pulse
      const pulseIcon = document.querySelector('.animate-pulse')
      expect(pulseIcon).toBeInTheDocument()
      
      // And it should NOT have animate-spin
      const spinIcon = document.querySelector('.animate-spin')
      expect(spinIcon).not.toBeInTheDocument()
    })
    
    it('should show progress dots for executing status', () => {
      const executingNotification = { ...mockNotification, status: 'executing' as const }
      render(<ToolNotification notification={executingNotification} />)
      
      const dots = document.querySelectorAll('.animate-pulse.rounded-full')
      expect(dots.length).toBeGreaterThan(0)
    })
  })
  
  describe('Accessibility', () => {
    it('should have proper ARIA attributes', () => {
      render(<ToolNotification notification={mockNotification} />)
      
      const status = screen.getByRole('status')
      expect(status).toHaveAttribute('aria-live', 'polite')
      expect(status).toHaveAttribute('aria-label')
    })
    
    it('should be keyboard accessible', async () => {
      render(<ToolNotification notification={mockNotification} />)
      
      // Wait for animation to complete
      await waitFor(() => {
        const statusElement = screen.getByRole('status')
        expect(statusElement).toBeVisible()
      })
    })
  })
})

describe('ToolNotificationList', () => {
  const notifications: ToolNotificationType[] = [
    {
      id: 'tool-1',
      toolName: 'tool_one',
      status: 'preparing',
      timestamp: new Date('2024-01-01T12:00:00Z')
    },
    {
      id: 'tool-2',
      toolName: 'tool_two',
      status: 'executing',
      timestamp: new Date('2024-01-01T12:00:10Z')
    },
    {
      id: 'tool-3',
      toolName: 'tool_three',
      status: 'complete',
      timestamp: new Date('2024-01-01T12:00:20Z')
    },
    {
      id: 'tool-4',
      toolName: 'tool_four',
      status: 'preparing',
      timestamp: new Date('2024-01-01T12:00:30Z')
    }
  ]
  
  it('should render multiple notifications', () => {
    render(<ToolNotificationList notifications={notifications.slice(0, 3)} />)
    
    expect(screen.getByText('Agent is preparing to use tool_one')).toBeInTheDocument()
    expect(screen.getByText('Agent is using tool_two')).toBeInTheDocument()
    expect(screen.getByText('Completed tool_three')).toBeInTheDocument()
  })
  
  it('should respect maxNotifications limit', () => {
    render(<ToolNotificationList notifications={notifications} maxNotifications={2} />)
    
    // Should only show the last 2 notifications
    expect(screen.queryByText('Agent is preparing to use tool_one')).not.toBeInTheDocument()
    expect(screen.queryByText('Agent is using tool_two')).not.toBeInTheDocument()
    expect(screen.getByText('Completed tool_three')).toBeInTheDocument()
    expect(screen.getByText('Agent is preparing to use tool_four')).toBeInTheDocument()
  })
  
  it('should render nothing when notifications array is empty', () => {
    const { container } = render(<ToolNotificationList notifications={[]} />)
    
    expect(container.firstChild).toBeNull()
  })
  
  it('should handle think tool notifications correctly', () => {
    const thinkNotifications: ToolNotificationType[] = [
      {
        id: 'think-1',
        toolName: 'think',
        status: 'executing',
        timestamp: new Date()
      },
      {
        id: 'tool-1',
        toolName: 'other_tool',
        status: 'preparing',
        timestamp: new Date()
      }
    ]
    
    render(<ToolNotificationList notifications={thinkNotifications} />)
    
    expect(screen.getByText('Agent is thinking...')).toBeInTheDocument()
    expect(screen.getByText('Agent is preparing to use other_tool')).toBeInTheDocument()
  })
  
  it('should maintain proper spacing between notifications', () => {
    const { container } = render(
      <ToolNotificationList notifications={notifications.slice(0, 2)} />
    )
    
    const wrapper = container.firstChild
    expect(wrapper).toHaveClass('space-y-2')
  })
  
  it('should apply custom className', () => {
    const { container } = render(
      <ToolNotificationList 
        notifications={notifications.slice(0, 1)} 
        className="custom-class"
      />
    )
    
    const wrapper = container.firstChild
    expect(wrapper).toHaveClass('custom-class')
  })
})