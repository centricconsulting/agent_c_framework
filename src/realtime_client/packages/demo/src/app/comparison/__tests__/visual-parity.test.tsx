/**
 * Visual Parity Tests for Comparison View
 * Validates that resumed sessions render identically to streaming sessions
 * Focuses on think tools, delegation, and role preservation
 */

import React from 'react';
import { describe, it, expect, beforeEach, vi } from 'vitest';
import { render, screen, within } from '@testing-library/react';
import { ComparisonChatView } from '@/components/comparison/ComparisonChatView';
import { ThinkBubbleRenderer } from '@/components/comparison/ThinkBubbleRenderer';

// Mock the useChat hook with specific test data
const mockMessages: any[] = [];

vi.mock('@agentc/realtime-react', () => ({
  useChat: vi.fn(() => ({
    messages: mockMessages,
    sendMessage: vi.fn(),
    isConnected: true,
    connectionState: 'connected',
    isAgentTyping: false,
    streamingMessage: null
  }))
}));

describe('Visual Parity Tests', () => {
  beforeEach(() => {
    vi.clearAllMocks();
    // Reset messages for each test
    mockMessages.length = 0;
  });

  describe('Think Tool Rendering', () => {
    it('should render think tools as gray bubbles', () => {
      render(
        <ThinkBubbleRenderer thought="Processing the request..." />
      );

      const bubble = screen.getByRole('note', { name: /Agent thought process/i });
      
      // Check gray background styling
      expect(bubble).toHaveClass('bg-gray-100');
      expect(bubble).toHaveClass('dark:bg-gray-800');
      
      // Check text color
      expect(bubble).toHaveClass('text-gray-700');
      expect(bubble).toHaveClass('dark:text-gray-300');
      
      // Check for Brain icon
      const icon = bubble.querySelector('svg');
      expect(icon).toBeTruthy();
      
      // Check content
      expect(screen.getByText('Processing the request...')).toBeTruthy();
    });

    it('should render think tools correctly in chat view', () => {
      // Add message with think tool
      mockMessages.push({
        id: 'msg-1',
        role: 'assistant',
        type: 'message',
        content: [
          {
            type: 'tool_use',
            name: 'think',
            id: 'think-1',
            input: { thought: 'Analyzing the problem...' }
          }
        ]
      });

      render(<ComparisonChatView />);

      // Think tool should render as gray bubble
      const thoughtBubble = screen.getByRole('note', { name: /Agent thought process/i });
      expect(thoughtBubble).toBeTruthy();
      expect(thoughtBubble).toHaveClass('bg-gray-100');
      expect(screen.getByText('Analyzing the problem...')).toBeTruthy();
    });

    it('should NOT render think tools as regular tool boxes', () => {
      mockMessages.push({
        id: 'msg-2',
        role: 'assistant', 
        type: 'message',
        content: [
          {
            type: 'tool_use',
            name: 'think',
            id: 'think-2',
            input: { thought: 'This should be gray' }
          }
        ]
      });

      render(<ComparisonChatView />);

      // Should NOT have the blue tool box styling
      const allElements = screen.queryAllByText(/Using think/i);
      expect(allElements.length).toBe(0);
      
      // Should have gray bubble instead
      const thoughtBubble = screen.getByRole('note');
      expect(thoughtBubble).toHaveClass('bg-gray-100');
    });
  });

  describe('Delegation Subsession Dividers', () => {
    it('should render subsession start divider', () => {
      mockMessages.push({
        type: 'divider',
        dividerType: 'start',
        metadata: { subAgentKey: 'helper_agent' }
      });

      render(<ComparisonChatView />);

      expect(screen.getByText(/Subsession Started/)).toBeTruthy();
      expect(screen.getByText(/helper_agent/)).toBeTruthy();
      
      // Check for start icon
      const container = screen.getByText('Subsession Started').parentElement;
      expect(container?.querySelector('svg')).toBeTruthy();
    });

    it('should render subsession end divider', () => {
      mockMessages.push({
        type: 'divider',
        dividerType: 'end',
        metadata: { subAgentKey: 'helper_agent' }
      });

      render(<ComparisonChatView />);

      expect(screen.getByText(/Subsession Ended/)).toBeTruthy();
      expect(screen.getByText(/helper_agent/)).toBeTruthy();
    });

    it('should render delegation tool differently from think tool', () => {
      mockMessages.push({
        id: 'msg-3',
        role: 'assistant',
        type: 'message',
        content: [
          {
            type: 'tool_use',
            name: 'ateam_chat',
            id: 'delegation-1',
            input: { 
              agent_key: 'specialist',
              message: 'Please help with this task'
            }
          }
        ]
      });

      render(<ComparisonChatView />);

      // Delegation should render as blue tool box, NOT gray
      expect(screen.getByText('Using ateam_chat')).toBeTruthy();
      
      // Should have blue styling, not gray
      const toolBox = screen.getByText('Using ateam_chat').parentElement?.parentElement;
      expect(toolBox).toHaveClass('border-blue-200');
      // Check it has blue background (class includes /50 for opacity)
      expect(toolBox?.className).toMatch(/bg-blue/);
    });
  });

  describe('Role Preservation', () => {
    it('should align user messages to the left', () => {
      mockMessages.push({
        id: 'user-msg',
        role: 'user',
        type: 'message',
        content: 'Hello from user'
      });

      render(<ComparisonChatView />);

      const userMessage = screen.getByText('Hello from user');
      const messageRow = userMessage.closest('.group');
      
      // User messages should have flex-row-reverse
      expect(messageRow).toHaveClass('flex-row-reverse');
    });

    it('should align assistant messages to the right', () => {
      mockMessages.push({
        id: 'assistant-msg',
        role: 'assistant',
        type: 'message',
        content: [
          { type: 'text', text: 'Response from assistant' }
        ]
      });

      render(<ComparisonChatView />);

      const assistantMessage = screen.getByText('Response from assistant');
      const messageRow = assistantMessage.closest('.group');
      
      // Assistant messages should NOT have flex-row-reverse
      expect(messageRow).not.toHaveClass('flex-row-reverse');
    });

    it('should preserve role styling for complex messages', () => {
      mockMessages.push(
        {
          id: 'user-1',
          role: 'user',
          type: 'message',
          content: 'User question'
        },
        {
          id: 'assistant-1',
          role: 'assistant',
          type: 'message',
          content: [
            {
              type: 'tool_use',
              name: 'think',
              id: 'think-3',
              input: { thought: 'Let me think...' }
            }
          ]
        },
        {
          id: 'assistant-2',
          role: 'assistant',
          type: 'message',
          content: [
            { type: 'text', text: 'Here is my response' }
          ]
        }
      );

      render(<ComparisonChatView />);

      // Check all messages maintain correct alignment
      const userMsg = screen.getByText('User question').closest('.group');
      expect(userMsg).toHaveClass('flex-row-reverse');

      const thinkMsg = screen.getByText('Let me think...').closest('.group');
      expect(thinkMsg).not.toHaveClass('flex-row-reverse');

      const responseMsg = screen.getByText('Here is my response').closest('.group');
      expect(responseMsg).not.toHaveClass('flex-row-reverse');
    });
  });

  describe('Complete Session Rendering', () => {
    it('should render a complete session with all features correctly', () => {
      // Simulate a complete session like in session_with_delegation.json
      mockMessages.push(
        // User message
        {
          id: '1',
          role: 'user',
          type: 'message',
          content: 'Message from user'
        },
        // Assistant think
        {
          id: '2',
          role: 'assistant',
          type: 'message',
          content: [
            {
              type: 'tool_use',
              name: 'think',
              id: 'think-4',
              input: { thought: 'thought from agent' }
            }
          ]
        },
        // Assistant response
        {
          id: '3',
          role: 'assistant',
          type: 'message',
          content: [
            { type: 'text', text: "I'll delegate this request." }
          ]
        },
        // Delegation divider start
        {
          type: 'divider',
          dividerType: 'start',
          metadata: { subAgentKey: 'realtime_core_coordinator' }
        },
        // Delegation tool use
        {
          id: '4',
          role: 'assistant',
          type: 'message',
          content: [
            {
              type: 'tool_use',
              name: 'ateam_chat',
              id: 'delegation-2',
              input: {
                agent_key: 'realtime_core_coordinator',
                message: 'Hello other agent please to the thing'
              }
            }
          ]
        },
        // Delegation divider end
        {
          type: 'divider',
          dividerType: 'end',
          metadata: { subAgentKey: 'realtime_core_coordinator' }
        },
        // Final response
        {
          id: '5',
          role: 'assistant',
          type: 'message',
          content: [
            { type: 'text', text: 'response to user?' }
          ]
        }
      );

      render(<ComparisonChatView />);

      // Verify all elements render correctly
      // 1. User message (left-aligned)
      const userMsg = screen.getByText('Message from user');
      expect(userMsg.closest('.group')).toHaveClass('flex-row-reverse');

      // 2. Think tool (gray bubble)
      const thinkBubble = screen.getByRole('note');
      expect(thinkBubble).toHaveClass('bg-gray-100');
      expect(screen.getByText('thought from agent')).toBeTruthy();

      // 3. Regular assistant message
      expect(screen.getByText("I'll delegate this request.")).toBeTruthy();

      // 4. Subsession dividers
      expect(screen.getByText(/Subsession Started/)).toBeTruthy();
      expect(screen.getByText(/Subsession Ended/)).toBeTruthy();
      expect(screen.getAllByText(/realtime_core_coordinator/).length).toBeGreaterThan(0);

      // 5. Delegation tool (blue box, not gray)
      const delegationTool = screen.getByText('Using ateam_chat');
      const delegationBox = delegationTool.parentElement?.parentElement;
      expect(delegationBox?.className).toMatch(/bg-blue/);

      // 6. Final response
      expect(screen.getByText('response to user?')).toBeTruthy();
    });
  });

  describe('Visual Consistency', () => {
    it('should maintain consistent spacing between messages', () => {
      mockMessages.push(
        {
          id: 'msg-1',
          role: 'user',
          type: 'message',
          content: 'First message'
        },
        {
          id: 'msg-2',
          role: 'assistant',
          type: 'message',
          content: [{ type: 'text', text: 'Second message' }]
        },
        {
          id: 'msg-3',
          role: 'user',
          type: 'message',
          content: 'Third message'
        }
      );

      render(<ComparisonChatView />);

      const container = screen.getByText('First message').closest('.space-y-4');
      expect(container).toHaveClass('space-y-4');
    });

    it('should apply consistent typography styles', () => {
      mockMessages.push({
        id: 'msg-typography',
        role: 'assistant',
        type: 'message',
        content: [
          { type: 'text', text: '# Heading\n\nParagraph text with **bold** and *italic*.' }
        ]
      });

      render(<ComparisonChatView />);

      // Markdown should be rendered with prose classes
      const content = screen.getByText(/Heading/).closest('.prose');
      expect(content).toHaveClass('prose');
      expect(content).toHaveClass('prose-sm');
      expect(content).toHaveClass('max-w-none');
    });
  });
});