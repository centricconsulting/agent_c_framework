/**
 * useToolNotifications - React hook for tool call notifications
 * Provides interface for tracking tool execution status
 */

import { useEffect, useState, useCallback, useRef } from 'react';
import { Logger } from '../utils/logger';
import { useRealtimeClientSafe } from '../providers/AgentCContext';

/**
 * Tool notification data structure
 */
export interface ToolNotification {
  id: string;
  toolName: string;
  status: 'preparing' | 'executing' | 'complete';
  timestamp: Date;
  arguments?: string;
}

/**
 * Tool call result structure
 */
export interface ToolCallResult {
  id: string;
  toolName: string;
  arguments: string;
  result: string;
  timestamp: Date;
}

/**
 * Options for the useToolNotifications hook
 */
export interface UseToolNotificationsOptions {
  /** Maximum number of notifications to keep in memory */
  maxNotifications?: number;
  
  /** Whether to auto-remove completed notifications */
  autoRemoveCompleted?: boolean;
  
  /** Delay in ms before auto-removing completed notifications */
  autoRemoveDelay?: number;
}

/**
 * Return type for the useToolNotifications hook
 */
export interface UseToolNotificationsReturn {
  /** Active tool notifications */
  notifications: ToolNotification[];
  
  /** Completed tool calls with results */
  completedToolCalls: ToolCallResult[];
  
  /** Get notification by ID */
  getNotification: (id: string) => ToolNotification | undefined;
  
  /** Clear all notifications */
  clearNotifications: () => void;
  
  /** Check if any tools are active */
  hasActiveTools: boolean;
  
  /** Check if a specific tool is active */
  isToolActive: (toolName: string) => boolean;
  
  /** Get count of active tools */
  activeToolCount: number;
}

/**
 * React hook for tool call notifications
 * Listens to SessionManager events for tool status updates
 */
export function useToolNotifications(
  options: UseToolNotificationsOptions = {}
): UseToolNotificationsReturn {
  const { 
    maxNotifications = 10,
    autoRemoveCompleted = false,
    autoRemoveDelay = 3000
  } = options;
  
  const client = useRealtimeClientSafe();
  
  // State
  const [notifications, setNotifications] = useState<Map<string, ToolNotification>>(new Map());
  const [completedToolCalls, setCompletedToolCalls] = useState<ToolCallResult[]>([]);
  
  // Refs for cleanup timers
  const removalTimersRef = useRef<Map<string, NodeJS.Timeout>>(new Map());
  
  // Clear all notifications
  const clearNotifications = useCallback(() => {
    // Clear any pending removal timers
    removalTimersRef.current.forEach(timer => clearTimeout(timer));
    removalTimersRef.current.clear();
    
    setNotifications(new Map());
    setCompletedToolCalls([]);
  }, []);
  
  // Get notification by ID
  const getNotification = useCallback((id: string): ToolNotification | undefined => {
    return notifications.get(id);
  }, [notifications]);
  
  // Check if a specific tool is active
  const isToolActive = useCallback((toolName: string): boolean => {
    return Array.from(notifications.values()).some(
      n => n.toolName === toolName && (n.status === 'preparing' || n.status === 'executing')
    );
  }, [notifications]);
  
  // Subscribe to tool notification events
  useEffect(() => {
    if (!client) return;
    
    const sessionManager = client.getSessionManager();
    if (!sessionManager) return;
    
    // Handle tool notification events
    const handleToolNotification = (notification: ToolNotification) => {
      Logger.debug('[useToolNotifications] Tool notification received:', notification);
      
      setNotifications(prev => {
        const next = new Map(prev);
        
        // Update or add the notification
        next.set(notification.id, notification);
        
        // Limit notifications if needed
        if (maxNotifications && next.size > maxNotifications) {
          // Remove oldest notifications
          const entries = Array.from(next.entries());
          const toRemove = entries.slice(0, entries.length - maxNotifications);
          toRemove.forEach(([id]) => next.delete(id));
        }
        
        // ADD EVE'S LOGGING:
        console.log('[useToolNotifications] Updated notifications Map size:', next.size);
        console.log('[useToolNotifications] Updated notifications array:', Array.from(next.values()));
        
        return next;
      });
      
      // Handle auto-removal for completed notifications
      if (notification.status === 'complete' && autoRemoveCompleted) {
        // Clear any existing timer for this notification
        const existingTimer = removalTimersRef.current.get(notification.id);
        if (existingTimer) {
          clearTimeout(existingTimer);
        }
        
        // Set new removal timer
        const timer = setTimeout(() => {
          setNotifications(prev => {
            const next = new Map(prev);
            next.delete(notification.id);
            return next;
          });
          removalTimersRef.current.delete(notification.id);
        }, autoRemoveDelay);
        
        removalTimersRef.current.set(notification.id, timer);
      }
    };
    
    // Handle tool notification removal events
    const handleToolNotificationRemoved = (event: { sessionId: string; toolCallId: string }) => {
      Logger.debug('[useToolNotifications] Tool notification removed:', event);
      
      const toolId = event.toolCallId;
      
      // Clear any pending removal timer
      const timer = removalTimersRef.current.get(toolId);
      if (timer) {
        clearTimeout(timer);
        removalTimersRef.current.delete(toolId);
      }
      
      // Remove the notification and get it for potential completed list addition
      setNotifications(prev => {
        const next = new Map(prev);
        const notification = prev.get(toolId);
        next.delete(toolId);
        
        // If it was a completed notification with arguments, add to completed list
        if (notification && notification.status === 'complete' && notification.arguments) {
          const toolCallResult: ToolCallResult = {
            id: notification.id,
            toolName: notification.toolName,
            arguments: notification.arguments,
            result: '', // Will be populated from tool_call event
            timestamp: notification.timestamp
          };
          
          setCompletedToolCalls(prev => {
            const next = [...prev, toolCallResult];
            // Limit completed calls if needed
            if (maxNotifications && next.length > maxNotifications) {
              return next.slice(-maxNotifications);
            }
            return next;
          });
        }
        
        return next;
      });
    };
    
    // Handle tool call complete events (with results)
    const handleToolCallComplete = (event: any) => {
      Logger.debug('[useToolNotifications] Tool call complete:', event);
      
      if (!event.toolCalls || !Array.isArray(event.toolCalls)) return;
      
      event.toolCalls.forEach((toolCall: any) => {
        const result = event.toolResults?.find((r: any) => 
          r.tool_use_id === toolCall.id
        );
        
        if (result) {
          const toolCallResult: ToolCallResult = {
            id: toolCall.id,
            toolName: toolCall.name,
            arguments: JSON.stringify(toolCall.input),
            result: result.content,
            timestamp: new Date()
          };
          
          setCompletedToolCalls(prev => {
            // Check if we already have this tool call
            const exists = prev.some(tc => tc.id === toolCallResult.id);
            if (exists) return prev;
            
            const next = [...prev, toolCallResult];
            // Limit completed calls if needed
            if (maxNotifications && next.length > maxNotifications) {
              return next.slice(-maxNotifications);
            }
            return next;
          });
        }
      });
    };
    
    // Handle nuclear cleanup (user turn start)
    const handleAllNotificationsCleared = () => {
      Logger.debug('[useToolNotifications] All notifications cleared (user turn start)');
      clearNotifications();
    };
    
    // Subscribe to events
    sessionManager.on('tool-notification', handleToolNotification);
    sessionManager.on('tool-notification-removed', handleToolNotificationRemoved);
    sessionManager.on('tool-call-complete', handleToolCallComplete);
    sessionManager.on('all-notifications-cleared', handleAllNotificationsCleared);
    
    return () => {
      // Clear all timers
      removalTimersRef.current.forEach(timer => clearTimeout(timer));
      removalTimersRef.current.clear();
      
      // Unsubscribe from events
      const cleanupSessionManager = client?.getSessionManager();
      if (cleanupSessionManager) {
        cleanupSessionManager.off('tool-notification', handleToolNotification);
        cleanupSessionManager.off('tool-notification-removed', handleToolNotificationRemoved);
        cleanupSessionManager.off('tool-call-complete', handleToolCallComplete);
        cleanupSessionManager.off('all-notifications-cleared', handleAllNotificationsCleared);
      }
    };
  }, [client, maxNotifications, autoRemoveCompleted, autoRemoveDelay]); // Removed notifications to prevent re-registration
  
  // Computed properties
  const notificationsArray = Array.from(notifications.values());
  const hasActiveTools = notificationsArray.some(
    n => n.status === 'preparing' || n.status === 'executing'
  );
  const activeToolCount = notificationsArray.filter(
    n => n.status === 'preparing' || n.status === 'executing'
  ).length;
  
  return {
    notifications: notificationsArray,
    completedToolCalls,
    getNotification,
    clearNotifications,
    hasActiveTools,
    isToolActive,
    activeToolCount
  };
}