/**
 * ToolCallManager - Tracks tool usage lifecycle and manages tool state
 * Manages tool state from selection through completion
 */

import { ToolCall, ToolResult } from './types/CommonTypes';
import { ToolSelectDeltaEvent, ToolCallEvent } from './types/ServerEvents';
import { Logger } from '../utils/logger';

/**
 * Tool notification for UI display
 */
export interface ToolNotification {
  id: string;
  sessionId: string;
  toolName: string;
  status: 'preparing' | 'executing' | 'complete';
  timestamp: Date;
  arguments?: string;
}

/**
 * Extended tool call with result
 */
export interface ToolCallWithResult extends ToolCall {
  result?: ToolResult;
}

/**
 * Manages tool call state and notifications
 */
export class ToolCallManager {
  private activeTools: Map<string, ToolNotification> = new Map();
  private completedToolCalls: ToolCallWithResult[] = [];
  
  constructor() {
  }
  
  /**
   * Create composite key for session-aware tool tracking
   * Format: ${sessionId}|${toolCallId}
   * Uses "|" separator as it's safer than ":" for UUID-based session IDs
   */
  private makeKey(sessionId: string, toolCallId: string): string {
    return `${sessionId}|${toolCallId}`;
  }
  
  /**
   * Handle tool selection event (before execution)
   * Processes ALL tools in the event, not just the first one
   */
  onToolSelect(event: ToolSelectDeltaEvent): ToolNotification[] {
    const sessionId = event.session_id;
    const notifications: ToolNotification[] = [];
    
    if (!event.tool_calls || event.tool_calls.length === 0) {
      throw new Error('ToolSelectDeltaEvent has no tool calls');
    }
    
    // Process ALL tool calls in the event
    event.tool_calls.forEach(toolCall => {
      const notification: ToolNotification = {
        id: toolCall.id,
        sessionId: sessionId,
        toolName: toolCall.name,
        status: 'preparing',
        timestamp: new Date(),
        arguments: JSON.stringify(toolCall.input)
      };
      
      // Use composite key for session-aware tracking
      const key = this.makeKey(sessionId, toolCall.id);
      this.activeTools.set(key, notification);
      notifications.push(notification);
      
      Logger.info(`[ToolCallManager] Tool selected: ${toolCall.name}`, {
        sessionId,
        toolCallId: toolCall.id,
        compositeKey: key,
        arguments: JSON.stringify(toolCall.input)
      });
    });
    
    return notifications;
  }
  
  /**
   * Handle tool call active event (during execution)
   * Processes ALL tools in the event, not just the first one
   */
  onToolCallActive(event: ToolCallEvent): ToolNotification[] {
    if (!event.active) {
      return [];
    }
    
    const sessionId = event.session_id;
    const notifications: ToolNotification[] = [];
    
    if (!event.tool_calls || event.tool_calls.length === 0) {
      return [];
    }
    
    // Process ALL tool calls in the event
    event.tool_calls.forEach(toolCall => {
      const key = this.makeKey(sessionId, toolCall.id);
      const notification = this.activeTools.get(key);
      
      if (notification) {
        // Update existing notification
        notification.status = 'executing';
        notifications.push(notification);
        
        Logger.info(`[ToolCallManager] Tool executing: ${notification.toolName}`, {
          sessionId,
          toolCallId: toolCall.id,
          compositeKey: key
        });
      } else {
        // Create new notification if we don't have one yet
        const newNotification: ToolNotification = {
          id: toolCall.id,
          sessionId: sessionId,
          toolName: toolCall.name,
          status: 'executing',
          timestamp: new Date(),
          arguments: JSON.stringify(toolCall.input)
        };
        
        this.activeTools.set(key, newNotification);
        notifications.push(newNotification);
        
        Logger.info(`[ToolCallManager] Tool executing (no prior selection): ${toolCall.name}`, {
          sessionId,
          toolCallId: toolCall.id,
          compositeKey: key
        });
      }
    });
    
    return notifications;
  }
  
  /**
   * Handle tool call completion event
   * Processes ALL tools in the event with session-aware tracking
   */
  onToolCallComplete(event: ToolCallEvent): ToolCallWithResult[] {
    if (event.active) {
      return [];
    }
    
    const sessionId = event.session_id;
    const newlyCompleted: ToolCallWithResult[] = [];
    
    event.tool_calls.forEach(toolCall => {
      const key = this.makeKey(sessionId, toolCall.id);
      
      // Mark as complete and remove from active
      const notification = this.activeTools.get(key);
      if (notification) {
        notification.status = 'complete';
      }
      this.activeTools.delete(key);
      
      // Find the result for this tool call
      const result = event.tool_results?.find(r => r.tool_use_id === toolCall.id);
      
      // Add to completed list
      const completedCall: ToolCallWithResult = {
        ...toolCall,
        result
      };
      
      this.completedToolCalls.push(completedCall);
      newlyCompleted.push(completedCall);
      
      Logger.info(`[ToolCallManager] Tool completed: ${toolCall.name}`, {
        sessionId,
        toolCallId: toolCall.id,
        compositeKey: key,
        hasResult: !!result,
        resultLength: result?.content?.length
      });
    });
    
    return newlyCompleted;
  }
  
  /**
   * Get all active tool notifications
   */
  getActiveNotifications(): ToolNotification[] {
    return Array.from(this.activeTools.values())
      .filter(n => n.status !== 'complete');
  }
  
  /**
   * Get all completed tool calls
   */
  getCompletedToolCalls(): ToolCallWithResult[] {
    return [...this.completedToolCalls];
  }
  
  /**
   * Get a specific tool notification by session ID and tool call ID
   */
  getNotification(sessionId: string, toolCallId: string): ToolNotification | undefined {
    const key = this.makeKey(sessionId, toolCallId);
    return this.activeTools.get(key);
  }
  
  /**
   * Check if a tool is currently active
   */
  isToolActive(sessionId: string, toolCallId: string): boolean {
    const key = this.makeKey(sessionId, toolCallId);
    const notification = this.activeTools.get(key);
    return notification !== undefined && notification.status !== 'complete';
  }
  
  /**
   * Get count of active tools
   */
  getActiveToolCount(): number {
    return this.getActiveNotifications().length;
  }
  
  /**
   * Get active notifications for a specific session (optional filter)
   */
  getActiveNotificationsForSession(sessionId: string): ToolNotification[] {
    return Array.from(this.activeTools.values())
      .filter(n => n.sessionId === sessionId && n.status !== 'complete');
  }
  
  /**
   * Clear all active notifications for a specific session
   * Used when an interaction ends for a session
   */
  clearSessionNotifications(sessionId: string): void {
    const keysToDelete: string[] = [];
    const notificationsCleared: Array<{ toolName: string; notificationId: string }> = [];
    
    // Find all keys for this session
    this.activeTools.forEach((notification, key) => {
      if (notification.sessionId === sessionId) {
        keysToDelete.push(key);
        notificationsCleared.push({
          toolName: notification.toolName,
          notificationId: notification.id
        });
        
        // Debug logging for each notification removed
        Logger.debug('[ToolCallManager] Clearing notification', {
          sessionId,
          toolName: notification.toolName,
          notificationId: notification.id,
          trigger: 'interaction_end'
        });
      }
    });
    
    // Delete all keys for this session
    keysToDelete.forEach(key => {
      this.activeTools.delete(key);
    });
    
    Logger.info(`[ToolCallManager] Cleared ${keysToDelete.length} notifications for session: ${sessionId}`, {
      notifications: notificationsCleared
    });
  }
  
  /**
   * Clear ALL active notifications (nuclear cleanup)
   * Used when user turn starts as a safety net
   */
  clearAllActiveNotifications(): void {
    const notificationsCleared: Array<{ sessionId: string; toolName: string; notificationId: string }> = [];
    
    // Debug logging for each notification removed
    this.activeTools.forEach((notification) => {
      notificationsCleared.push({
        sessionId: notification.sessionId,
        toolName: notification.toolName,
        notificationId: notification.id
      });
      
      Logger.debug('[ToolCallManager] Clearing notification', {
        sessionId: notification.sessionId,
        toolName: notification.toolName,
        notificationId: notification.id,
        trigger: 'user_turn_start'
      });
    });
    
    const count = this.activeTools.size;
    this.activeTools.clear();
    
    Logger.info(`[ToolCallManager] Cleared all ${count} active notifications`, {
      notifications: notificationsCleared
    });
  }
  
  /**
   * Clear completed tool calls
   */
  clearCompleted(): void {
    this.completedToolCalls = [];
    Logger.debug('[ToolCallManager] Cleared completed tool calls');
  }
  
  /**
   * Reset all state
   */
  reset(): void {
    this.activeTools.clear();
    this.completedToolCalls = [];
    Logger.debug('[ToolCallManager] ToolCallManager reset');
  }
  
  /**
   * Get statistics about tool usage
   */
  getStatistics(): {
    activeCount: number;
    completedCount: number;
    totalCount: number;
  } {
    return {
      activeCount: this.getActiveToolCount(),
      completedCount: this.completedToolCalls.length,
      totalCount: this.getActiveToolCount() + this.completedToolCalls.length
    };
  }
}