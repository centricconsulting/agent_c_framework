import { renderHook, act, waitFor } from '@testing-library/react'
import { describe, it, expect, vi, beforeEach, afterEach } from 'vitest'
import { useToolNotifications } from '../useToolNotifications'
import type { ToolNotification, ToolCallResult } from '../useToolNotifications'
import * as AgentCContext from '../../providers/AgentCContext'
import React from 'react'

// Mock the logger
vi.mock('../../utils/logger', () => ({
  Logger: {
    debug: vi.fn(),
    info: vi.fn(),
    warn: vi.fn(),
    error: vi.fn()
  }
}))

describe('useToolNotifications', () => {
  let mockClient: any
  let mockSessionManager: any
  let eventHandlers: Map<string, Set<Function>>
  
  beforeEach(() => {
    vi.clearAllMocks()
    vi.useFakeTimers()
    
    // Create event handler storage
    eventHandlers = new Map()
    
    // Mock SessionManager
    mockSessionManager = {
      on: vi.fn((event: string, handler: Function) => {
        if (!eventHandlers.has(event)) {
          eventHandlers.set(event, new Set())
        }
        eventHandlers.get(event)!.add(handler)
      }),
      off: vi.fn((event: string, handler: Function) => {
        eventHandlers.get(event)?.delete(handler)
      }),
      emit: (event: string, data: any) => {
        eventHandlers.get(event)?.forEach(handler => handler(data))
      }
    }
    
    // Mock RealtimeClient
    mockClient = {
      getSessionManager: vi.fn(() => mockSessionManager),
      isConnected: vi.fn(() => true),
      on: vi.fn(),
      off: vi.fn()
    }
    
    // Mock the useRealtimeClientSafe hook to return our mock client
    vi.spyOn(AgentCContext, 'useRealtimeClientSafe').mockReturnValue(mockClient)
  })
  
  afterEach(() => {
    vi.useRealTimers()
    vi.restoreAllMocks()
  })
  
  const wrapper = ({ children }: { children: React.ReactNode }) => (
    <>{children}</>
  )
  
  describe('Basic Functionality', () => {
    it('should initialize with empty state', () => {
      const { result } = renderHook(() => useToolNotifications(), { wrapper })
      
      expect(result.current.notifications).toEqual([])
      expect(result.current.completedToolCalls).toEqual([])
      expect(result.current.hasActiveTools).toBe(false)
      expect(result.current.activeToolCount).toBe(0)
    })
    
    it('should handle tool notification events', () => {
      const { result } = renderHook(() => useToolNotifications(), { wrapper })
      
      const notification: ToolNotification = {
        id: 'tool-1',
        toolName: 'test_tool',
        status: 'preparing',
        timestamp: new Date(),
        arguments: '{"param": "value"}'
      }
      
      act(() => {
        mockSessionManager.emit('tool-notification', notification)
      })
      
      expect(result.current.notifications).toHaveLength(1)
      expect(result.current.notifications[0]).toEqual(notification)
      expect(result.current.hasActiveTools).toBe(true)
      expect(result.current.activeToolCount).toBe(1)
    })
    
    it('should update existing notification', () => {
      const { result } = renderHook(() => useToolNotifications(), { wrapper })
      
      const notification: ToolNotification = {
        id: 'tool-1',
        toolName: 'test_tool',
        status: 'preparing',
        timestamp: new Date()
      }
      
      act(() => {
        mockSessionManager.emit('tool-notification', notification)
      })
      
      expect(result.current.notifications[0]!.status).toBe('preparing')
      
      const updatedNotification = { ...notification, status: 'executing' as const }
      
      act(() => {
        mockSessionManager.emit('tool-notification', updatedNotification)
      })
      
      expect(result.current.notifications).toHaveLength(1)
      expect(result.current.notifications[0]!.status).toBe('executing')
    })
    
    it('should handle tool notification removal', () => {
      const { result } = renderHook(() => useToolNotifications(), { wrapper })
      
      const notification: ToolNotification = {
        id: 'tool-1',
        toolName: 'test_tool',
        status: 'complete',
        timestamp: new Date(),
        arguments: '{"param": "value"}'
      }
      
      act(() => {
        mockSessionManager.emit('tool-notification', notification)
      })
      
      expect(result.current.notifications).toHaveLength(1)
      
      act(() => {
        mockSessionManager.emit('tool-notification-removed', 'tool-1')
      })
      
      expect(result.current.notifications).toHaveLength(0)
    })
  })
  
  describe('Tool State Tracking', () => {
    it('should track active tools correctly', () => {
      const { result } = renderHook(() => useToolNotifications(), { wrapper })
      
      const preparingTool: ToolNotification = {
        id: 'tool-1',
        toolName: 'tool_one',
        status: 'preparing',
        timestamp: new Date()
      }
      
      const executingTool: ToolNotification = {
        id: 'tool-2',
        toolName: 'tool_two',
        status: 'executing',
        timestamp: new Date()
      }
      
      const completeTool: ToolNotification = {
        id: 'tool-3',
        toolName: 'tool_three',
        status: 'complete',
        timestamp: new Date()
      }
      
      act(() => {
        mockSessionManager.emit('tool-notification', preparingTool)
        mockSessionManager.emit('tool-notification', executingTool)
        mockSessionManager.emit('tool-notification', completeTool)
      })
      
      expect(result.current.hasActiveTools).toBe(true)
      expect(result.current.activeToolCount).toBe(2) // preparing + executing
      expect(result.current.isToolActive('tool_one')).toBe(true)
      expect(result.current.isToolActive('tool_two')).toBe(true)
      expect(result.current.isToolActive('tool_three')).toBe(false)
    })
    
    it('should get notification by ID', () => {
      const { result } = renderHook(() => useToolNotifications(), { wrapper })
      
      const notification: ToolNotification = {
        id: 'tool-1',
        toolName: 'test_tool',
        status: 'preparing',
        timestamp: new Date()
      }
      
      act(() => {
        mockSessionManager.emit('tool-notification', notification)
      })
      
      expect(result.current.getNotification('tool-1')).toEqual(notification)
      expect(result.current.getNotification('nonexistent')).toBeUndefined()
    })
  })
  
  describe('Auto-removal', () => {
    it('should auto-remove completed notifications when enabled', () => {
      const { result } = renderHook(
        () => useToolNotifications({
          autoRemoveCompleted: true,
          autoRemoveDelay: 1000
        }),
        { wrapper }
      )
      
      const notification: ToolNotification = {
        id: 'tool-1',
        toolName: 'test_tool',
        status: 'complete',
        timestamp: new Date()
      }
      
      act(() => {
        mockSessionManager.emit('tool-notification', notification)
      })
      
      expect(result.current.notifications).toHaveLength(1)
      
      act(() => {
        vi.advanceTimersByTime(1000)
      })
      
      // Wait for state update after timer
      waitFor(() => {
        expect(result.current.notifications).toHaveLength(0)
      })
    })
    
    it('should not auto-remove when disabled', () => {
      const { result } = renderHook(
        () => useToolNotifications({
          autoRemoveCompleted: false
        }),
        { wrapper }
      )
      
      const notification: ToolNotification = {
        id: 'tool-1',
        toolName: 'test_tool',
        status: 'complete',
        timestamp: new Date()
      }
      
      act(() => {
        mockSessionManager.emit('tool-notification', notification)
      })
      
      expect(result.current.notifications).toHaveLength(1)
      
      act(() => {
        vi.advanceTimersByTime(10000)
      })
      
      expect(result.current.notifications).toHaveLength(1)
    })
    
    it('should cancel auto-removal timer on manual removal', () => {
      const { result } = renderHook(
        () => useToolNotifications({
          autoRemoveCompleted: true,
          autoRemoveDelay: 1000
        }),
        { wrapper }
      )
      
      const notification: ToolNotification = {
        id: 'tool-1',
        toolName: 'test_tool',
        status: 'complete',
        timestamp: new Date()
      }
      
      act(() => {
        mockSessionManager.emit('tool-notification', notification)
      })
      
      expect(result.current.notifications).toHaveLength(1)
      
      // Manual removal before timer
      act(() => {
        mockSessionManager.emit('tool-notification-removed', 'tool-1')
      })
      
      expect(result.current.notifications).toHaveLength(0)
      
      // Advance time - should not cause any issues
      act(() => {
        vi.advanceTimersByTime(1000)
      })
      
      // Wait for state update after timer
      waitFor(() => {
        expect(result.current.notifications).toHaveLength(0)
      })
    })
  })
  
  describe('Completed Tool Calls', () => {
    it('should track completed tool calls with results', () => {
      const { result } = renderHook(() => useToolNotifications(), { wrapper })
      
      const toolCallEvent = {
        toolCalls: [
          {
            id: 'call-1',
            name: 'test_tool',
            input: { param: 'value' }
          }
        ],
        toolResults: [
          {
            tool_use_id: 'call-1',
            content: '{"result": "success"}'
          }
        ]
      }
      
      act(() => {
        mockSessionManager.emit('tool-call-complete', toolCallEvent)
      })
      
      expect(result.current.completedToolCalls).toHaveLength(1)
      expect(result.current.completedToolCalls[0]).toMatchObject({
        id: 'call-1',
        toolName: 'test_tool',
        arguments: JSON.stringify({ param: 'value' }),
        result: '{"result": "success"}'
      })
    })
    
    it('should not duplicate completed tool calls', () => {
      const { result } = renderHook(() => useToolNotifications(), { wrapper })
      
      const toolCallEvent = {
        toolCalls: [
          {
            id: 'call-1',
            name: 'test_tool',
            input: { param: 'value' }
          }
        ],
        toolResults: [
          {
            tool_use_id: 'call-1',
            content: '{"result": "success"}'
          }
        ]
      }
      
      act(() => {
        mockSessionManager.emit('tool-call-complete', toolCallEvent)
        mockSessionManager.emit('tool-call-complete', toolCallEvent)
      })
      
      expect(result.current.completedToolCalls).toHaveLength(1)
    })
  })
  
  describe('Max Notifications Limit', () => {
    it('should respect maxNotifications limit', () => {
      const { result } = renderHook(
        () => useToolNotifications({ maxNotifications: 3 }),
        { wrapper }
      )
      
      // Add 5 notifications
      for (let i = 1; i <= 5; i++) {
        const notification: ToolNotification = {
          id: `tool-${i}`,
          toolName: `tool_${i}`,
          status: 'preparing',
          timestamp: new Date()
        }
        
        act(() => {
          mockSessionManager.emit('tool-notification', notification)
        })
      }
      
      // Should only keep the last 3
      expect(result.current.notifications).toHaveLength(3)
      expect(result.current.notifications[0]!.id).toBe('tool-3')
      expect(result.current.notifications[1]!.id).toBe('tool-4')
      expect(result.current.notifications[2]!.id).toBe('tool-5')
    })
    
    it('should limit completed tool calls', () => {
      const { result } = renderHook(
        () => useToolNotifications({ maxNotifications: 2 }),
        { wrapper }
      )
      
      // Add 3 completed tool calls
      for (let i = 1; i <= 3; i++) {
        const toolCallEvent = {
          toolCalls: [
            {
              id: `call-${i}`,
              name: `tool_${i}`,
              input: { param: `value${i}` }
            }
          ],
          toolResults: [
            {
              tool_use_id: `call-${i}`,
              content: `{"result": "success${i}"}`
            }
          ]
        }
        
        act(() => {
          mockSessionManager.emit('tool-call-complete', toolCallEvent)
        })
      }
      
      // Should only keep the last 2
      expect(result.current.completedToolCalls).toHaveLength(2)
      expect(result.current.completedToolCalls[0]!.id).toBe('call-2')
      expect(result.current.completedToolCalls[1]!.id).toBe('call-3')
    })
  })
  
  describe('Clear Notifications', () => {
    it('should clear all notifications and completed calls', () => {
      const { result } = renderHook(() => useToolNotifications(), { wrapper })
      
      // Add notifications
      const notification: ToolNotification = {
        id: 'tool-1',
        toolName: 'test_tool',
        status: 'preparing',
        timestamp: new Date()
      }
      
      const toolCallEvent = {
        toolCalls: [
          {
            id: 'call-1',
            name: 'test_tool',
            input: { param: 'value' }
          }
        ],
        toolResults: [
          {
            tool_use_id: 'call-1',
            content: '{"result": "success"}'
          }
        ]
      }
      
      act(() => {
        mockSessionManager.emit('tool-notification', notification)
        mockSessionManager.emit('tool-call-complete', toolCallEvent)
      })
      
      expect(result.current.notifications).toHaveLength(1)
      expect(result.current.completedToolCalls).toHaveLength(1)
      
      act(() => {
        result.current.clearNotifications()
      })
      
      expect(result.current.notifications).toHaveLength(0)
      expect(result.current.completedToolCalls).toHaveLength(0)
    })
    
    it('should clear pending auto-removal timers', () => {
      const { result } = renderHook(
        () => useToolNotifications({
          autoRemoveCompleted: true,
          autoRemoveDelay: 1000
        }),
        { wrapper }
      )
      
      const notification: ToolNotification = {
        id: 'tool-1',
        toolName: 'test_tool',
        status: 'complete',
        timestamp: new Date()
      }
      
      act(() => {
        mockSessionManager.emit('tool-notification', notification)
      })
      
      act(() => {
        result.current.clearNotifications()
      })
      
      // Advance time - should not cause any issues
      act(() => {
        vi.advanceTimersByTime(1000)
      })
      
      // Wait for state update after timer
      waitFor(() => {
        expect(result.current.notifications).toHaveLength(0)
      })
    })
  })
  
  describe('Cleanup', () => {
    it('should clean up event listeners on unmount', () => {
      const { unmount } = renderHook(() => useToolNotifications(), { wrapper })
      
      expect(mockSessionManager.on).toHaveBeenCalledTimes(3)
      
      unmount()
      
      expect(mockSessionManager.off).toHaveBeenCalledTimes(3)
    })
    
    it('should clear all timers on unmount', () => {
      const { result, unmount } = renderHook(
        () => useToolNotifications({
          autoRemoveCompleted: true,
          autoRemoveDelay: 1000
        }),
        { wrapper }
      )
      
      // Add multiple completed notifications
      for (let i = 1; i <= 3; i++) {
        const notification: ToolNotification = {
          id: `tool-${i}`,
          toolName: `test_tool_${i}`,
          status: 'complete',
          timestamp: new Date()
        }
        
        act(() => {
          mockSessionManager.emit('tool-notification', notification)
        })
      }
      
      unmount()
      
      // Advance time - should not cause any state updates
      act(() => {
        vi.advanceTimersByTime(1000)
      })
      
      // No errors should occur
    })
  })
})