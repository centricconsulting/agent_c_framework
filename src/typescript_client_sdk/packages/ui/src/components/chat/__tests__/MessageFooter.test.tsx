/**
 * MessageFooter Component Tests - Tool Calls in Thought Messages
 * Testing tool call display functionality for both regular and thought messages
 */

import React from 'react';
import { describe, it, expect, beforeEach, afterEach, vi } from 'vitest';
import { render, screen, within, waitFor } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { MessageFooter } from '../MessageFooter';
import type { MessageData } from '../Message';

// Mock framer-motion to avoid animation issues in tests
vi.mock('framer-motion', () => ({
  motion: {
    div: ({ children, initial, animate, exit, transition, ...props }: any) => (
      <div 
        data-testid="motion-div"
        data-initial={JSON.stringify(initial)}
        data-animate={JSON.stringify(animate)}
        data-exit={JSON.stringify(exit)}
        {...props}
      >
        {children}
      </div>
    )
  },
  AnimatePresence: ({ children }: any) => <>{children}</>
}));

// Mock Lucide React icons
vi.mock('lucide-react', () => ({
  Copy: ({ className }: any) => <div data-testid="copy-icon" className={className} />,
  Check: ({ className }: any) => <div data-testid="check-icon" className={className} />,
  Hash: ({ className }: any) => <div data-testid="hash-icon" className={className} />,
  ArrowRight: ({ className }: any) => <div data-testid="arrow-right-icon" className={className} />,
  Wrench: ({ className }: any) => <div data-testid="wrench-icon" className={className} />,
  ChevronDown: ({ className }: any) => <div data-testid="chevron-down-icon" className={className} />,
  ChevronRight: ({ className }: any) => <div data-testid="chevron-right-icon" className={className} />,
  RefreshCw: ({ className }: any) => <div data-testid="refresh-icon" className={className} />,
  Edit2: ({ className }: any) => <div data-testid="edit-icon" className={className} />,
  Clock: ({ className }: any) => <div data-testid="clock-icon" className={className} />,
  FileInput: ({ className }: any) => <div data-testid="file-input-icon" className={className} />,
  FileOutput: ({ className }: any) => <div data-testid="file-output-icon" className={className} />,
  Equal: ({ className }: any) => <div data-testid="equal-icon" className={className} />,
  Code2: ({ className }: any) => <div data-testid="code-icon" className={className} />,
}));

describe('MessageFooter - Tool Calls in Thought Messages', () => {
  // Mock clipboard API
  const mockWriteText = vi.fn().mockResolvedValue(undefined);
  
  beforeEach(() => {
    vi.clearAllMocks();
    Object.defineProperty(navigator, 'clipboard', {
      value: {
        writeText: mockWriteText
      },
      configurable: true,
      writable: true
    });
  });

  afterEach(() => {
    vi.clearAllMocks();
  });

  describe('Scenario 1: Thought Message with Single Tool Call', () => {
    it('should render footer for thought message with one tool call', () => {
      const thoughtMessage: MessageData = {
        id: 'thought-1',
        role: 'assistant (thought)',
        content: 'Thinking about the problem...',
        timestamp: new Date().toISOString(),
        toolCalls: [
          {
            id: 'tool-1',
            type: 'tool_use',
            name: 'workspace_read',
            input: { path: '//workspace/file.txt' }
          }
        ]
      };

      render(<MessageFooter message={thoughtMessage} />);

      // Footer should render for thought messages
      expect(screen.getByRole('button', { name: /show tool calls/i })).toBeInTheDocument();
      expect(screen.getByText('1 tool')).toBeInTheDocument();
    });

    it('should expand to show tool details when clicked', async () => {
      const user = userEvent.setup();
      const thoughtMessage: MessageData = {
        id: 'thought-1',
        role: 'assistant (thought)',
        content: 'Analyzing the file...',
        timestamp: new Date().toISOString(),
        toolCalls: [
          {
            id: 'tool-1',
            type: 'tool_use',
            name: 'workspace_read',
            input: { path: '//workspace/test.txt' }
          }
        ],
        toolResults: [
          {
            type: 'tool_result',
            tool_use_id: 'tool-1',
            content: 'File content here'
          }
        ]
      };

      render(<MessageFooter message={thoughtMessage} />);

      const toggleButton = screen.getByRole('button', { name: /show tool calls/i });
      await user.click(toggleButton);

      // Wait for expansion animation
      await waitFor(() => {
        expect(screen.getByText('workspace_read')).toBeInTheDocument();
      });

      expect(screen.getByText('Input Arguments')).toBeInTheDocument();
      expect(screen.getByText('Result')).toBeInTheDocument();
    });
  });

  describe('Scenario 2: Thought Message with Multiple Tool Calls', () => {
    it('should display correct count for multiple tool calls', () => {
      const thoughtMessage: MessageData = {
        id: 'thought-2',
        role: 'assistant (thought)',
        content: 'Searching multiple files...',
        timestamp: new Date().toISOString(),
        toolCalls: [
          {
            id: 'tool-1',
            type: 'tool_use',
            name: 'workspace_read',
            input: { path: '//workspace/file1.txt' }
          },
          {
            id: 'tool-2',
            type: 'tool_use',
            name: 'workspace_grep',
            input: { pattern: 'test', paths: ['//workspace/**'] }
          },
          {
            id: 'tool-3',
            type: 'tool_use',
            name: 'workspace_ls',
            input: { path: '//workspace/' }
          }
        ]
      };

      render(<MessageFooter message={thoughtMessage} />);

      expect(screen.getByText('3 tools')).toBeInTheDocument();
    });

    it('should show all tools when expanded', async () => {
      const user = userEvent.setup();
      const thoughtMessage: MessageData = {
        id: 'thought-2',
        role: 'assistant (thought)',
        content: 'Running multiple operations...',
        timestamp: new Date().toISOString(),
        toolCalls: [
          {
            id: 'tool-1',
            type: 'tool_use',
            name: 'workspace_read',
            input: { path: 'file1.txt' }
          },
          {
            id: 'tool-2',
            type: 'tool_use',
            name: 'workspace_grep',
            input: { pattern: 'test' }
          },
          {
            id: 'tool-3',
            type: 'tool_use',
            name: 'workspace_ls',
            input: { path: '/' }
          }
        ]
      };

      render(<MessageFooter message={thoughtMessage} />);

      const toggleButton = screen.getByRole('button', { name: /show tool calls/i });
      await user.click(toggleButton);

      await waitFor(() => {
        expect(screen.getByText('workspace_read')).toBeInTheDocument();
        expect(screen.getByText('workspace_grep')).toBeInTheDocument();
        expect(screen.getByText('workspace_ls')).toBeInTheDocument();
      });
    });

    it('should allow expanding individual tool details', async () => {
      const user = userEvent.setup();
      const thoughtMessage: MessageData = {
        id: 'thought-2',
        role: 'assistant (thought)',
        content: 'Multi-step process...',
        timestamp: new Date().toISOString(),
        toolCalls: [
          {
            id: 'tool-1',
            type: 'tool_use',
            name: 'tool_one',
            input: { param: 'value1' }
          },
          {
            id: 'tool-2',
            type: 'tool_use',
            name: 'tool_two',
            input: { param: 'value2' }
          }
        ],
        toolResults: [
          {
            type: 'tool_result',
            tool_use_id: 'tool-1',
            content: '{"result": "success1"}'
          },
          {
            type: 'tool_result',
            tool_use_id: 'tool-2',
            content: '{"result": "success2"}'
          }
        ]
      };

      render(<MessageFooter message={thoughtMessage} />);

      // First, show the tool calls list
      const listToggle = screen.getByRole('button', { name: /show tool calls/i });
      await user.click(listToggle);

      // Wait for tools to be visible
      await waitFor(() => {
        expect(screen.getByText('tool_one')).toBeInTheDocument();
      });

      // Find and expand first tool
      const expandButtons = screen.getAllByRole('button', { name: /expand .* details/i });
      await user.click(expandButtons[0]);

      await waitFor(() => {
        expect(screen.getByText('Input Arguments')).toBeInTheDocument();
      });

      // Check that tool-1's result is shown
      const resultCode = screen.getByText(/"result": "success1"/);
      expect(resultCode).toBeInTheDocument();
    });
  });

  describe('Scenario 3: Thought Message with Only Think Tool', () => {
    it('should not show tool indicator when only think tool exists', () => {
      const thoughtMessage: MessageData = {
        id: 'thought-3',
        role: 'assistant (thought)',
        content: 'Thinking deeply...',
        timestamp: new Date().toISOString(),
        toolCalls: [
          {
            id: 'think-1',
            type: 'tool_use',
            name: 'think',
            input: { thought: 'My reasoning here' }
          }
        ]
      };

      const { container } = render(<MessageFooter message={thoughtMessage} />);

      // Should not show tool calls button since think is filtered
      expect(screen.queryByRole('button', { name: /tool calls/i })).not.toBeInTheDocument();
      expect(screen.queryByText(/tool/i)).not.toBeInTheDocument();
    });
  });

  describe('Scenario 4: Thought Message with Mixed Tools', () => {
    it('should filter out think tool and show only non-think tools', () => {
      const thoughtMessage: MessageData = {
        id: 'thought-4',
        role: 'assistant (thought)',
        content: 'Mixed operations...',
        timestamp: new Date().toISOString(),
        toolCalls: [
          {
            id: 'think-1',
            type: 'tool_use',
            name: 'think',
            input: { thought: 'Reasoning about approach' }
          },
          {
            id: 'tool-1',
            type: 'tool_use',
            name: 'workspace_read',
            input: { path: 'file.txt' }
          },
          {
            id: 'tool-2',
            type: 'tool_use',
            name: 'workspace_grep',
            input: { pattern: 'search' }
          }
        ]
      };

      render(<MessageFooter message={thoughtMessage} />);

      // Should show "2 tools" (think filtered out)
      expect(screen.getByText('2 tools')).toBeInTheDocument();
    });

    it('should only display non-think tools when expanded', async () => {
      const user = userEvent.setup();
      const thoughtMessage: MessageData = {
        id: 'thought-4',
        role: 'assistant (thought)',
        content: 'Processing with mixed tools...',
        timestamp: new Date().toISOString(),
        toolCalls: [
          {
            id: 'think-1',
            type: 'tool_use',
            name: 'think',
            input: { thought: 'My thought process' }
          },
          {
            id: 'tool-1',
            type: 'tool_use',
            name: 'workspace_read',
            input: { path: 'test.txt' }
          },
          {
            id: 'tool-2',
            type: 'tool_use',
            name: 'workspace_grep',
            input: { pattern: 'pattern' }
          }
        ]
      };

      render(<MessageFooter message={thoughtMessage} />);

      const toggleButton = screen.getByRole('button', { name: /show tool calls/i });
      await user.click(toggleButton);

      await waitFor(() => {
        expect(screen.getByText('workspace_read')).toBeInTheDocument();
        expect(screen.getByText('workspace_grep')).toBeInTheDocument();
      });

      // Think tool should NOT be visible
      expect(screen.queryByText('think')).not.toBeInTheDocument();
    });
  });

  describe('Scenario 5: Regular Assistant Message (Regression Check)', () => {
    it('should continue working for regular assistant messages', () => {
      const assistantMessage: MessageData = {
        id: 'assist-1',
        role: 'assistant',
        content: 'Here is my response',
        timestamp: new Date().toISOString(),
        toolCalls: [
          {
            id: 'tool-1',
            type: 'tool_use',
            name: 'calculator',
            input: { operation: 'add', a: 1, b: 2 }
          }
        ]
      };

      render(<MessageFooter message={assistantMessage} />);

      expect(screen.getByText('1 tool')).toBeInTheDocument();
    });

    it('should show tool details for regular assistant messages', async () => {
      const user = userEvent.setup();
      const assistantMessage: MessageData = {
        id: 'assist-1',
        role: 'assistant',
        content: 'Calculation result',
        timestamp: new Date().toISOString(),
        toolCalls: [
          {
            id: 'tool-1',
            type: 'tool_use',
            name: 'calculator',
            input: { operation: 'multiply', a: 5, b: 10 }
          }
        ],
        toolResults: [
          {
            type: 'tool_result',
            tool_use_id: 'tool-1',
            content: '50'
          }
        ]
      };

      render(<MessageFooter message={assistantMessage} />);

      const toggleButton = screen.getByRole('button', { name: /show tool calls/i });
      await user.click(toggleButton);

      await waitFor(() => {
        expect(screen.getByText('calculator')).toBeInTheDocument();
        expect(screen.getByText('Tool executed successfully')).toBeInTheDocument();
      });

      // Expand tool details
      const expandButton = screen.getByRole('button', { name: /expand calculator details/i });
      await user.click(expandButton);

      await waitFor(() => {
        expect(screen.getByText('Input Arguments')).toBeInTheDocument();
        expect(screen.getByText('Result')).toBeInTheDocument();
      });
    });

    it('should maintain copy button functionality', async () => {
      const user = userEvent.setup();
      const assistantMessage: MessageData = {
        id: 'assist-1',
        role: 'assistant',
        content: 'Test content to copy',
        timestamp: new Date().toISOString()
      };

      render(<MessageFooter message={assistantMessage} />);

      const copyButton = screen.getByRole('button', { name: /copy message content/i });
      await user.click(copyButton);

      await waitFor(() => {
        expect(mockWriteText).toHaveBeenCalledWith('Test content to copy');
      });
    });
  });

  describe('Scenario 6: Thought Message without Tool Calls', () => {
    it('should not show tool indicator for thought without tools', () => {
      const thoughtMessage: MessageData = {
        id: 'thought-6',
        role: 'assistant (thought)',
        content: 'Just thinking, no tools',
        timestamp: new Date().toISOString()
      };

      render(<MessageFooter message={thoughtMessage} />);

      expect(screen.queryByRole('button', { name: /tool calls/i })).not.toBeInTheDocument();
      expect(screen.queryByText(/tool/i)).not.toBeInTheDocument();
    });

    it('should not show tool indicator for empty toolCalls array', () => {
      const thoughtMessage: MessageData = {
        id: 'thought-6',
        role: 'assistant (thought)',
        content: 'Thought with empty tools array',
        timestamp: new Date().toISOString(),
        toolCalls: []
      };

      render(<MessageFooter message={thoughtMessage} />);

      expect(screen.queryByRole('button', { name: /tool calls/i })).not.toBeInTheDocument();
    });
  });

  describe('Scenario 7: Resumed Session with Thought Tool Calls', () => {
    it('should display tool calls from historical thought messages', () => {
      const historicalThought: MessageData = {
        id: 'historical-1',
        role: 'assistant (thought)',
        content: 'Previous session thought',
        timestamp: new Date(Date.now() - 3600000).toISOString(), // 1 hour ago
        toolCalls: [
          {
            id: 'hist-tool-1',
            type: 'tool_use',
            name: 'workspace_grep',
            input: { pattern: 'function', paths: ['//workspace/**'] }
          }
        ],
        toolResults: [
          {
            type: 'tool_result',
            tool_use_id: 'hist-tool-1',
            content: 'Found 42 matches'
          }
        ]
      };

      render(<MessageFooter message={historicalThought} />);

      expect(screen.getByText('1 tool')).toBeInTheDocument();
    });

    it('should show complete tool information for resumed sessions', async () => {
      const user = userEvent.setup();
      const historicalThought: MessageData = {
        id: 'historical-2',
        role: 'assistant (thought)',
        content: 'Resumed session thought',
        timestamp: new Date(Date.now() - 7200000).toISOString(), // 2 hours ago
        toolCalls: [
          {
            id: 'hist-tool-1',
            type: 'tool_use',
            name: 'workspace_read',
            input: { path: '//workspace/important.txt' }
          },
          {
            id: 'hist-tool-2',
            type: 'tool_use',
            name: 'workspace_ls',
            input: { path: '//workspace/docs' }
          }
        ],
        toolResults: [
          {
            type: 'tool_result',
            tool_use_id: 'hist-tool-1',
            content: 'Important file contents'
          },
          {
            type: 'tool_result',
            tool_use_id: 'hist-tool-2',
            content: JSON.stringify(['doc1.md', 'doc2.md'])
          }
        ]
      };

      render(<MessageFooter message={historicalThought} />);

      // Expand tool calls list
      const listToggle = screen.getByRole('button', { name: /show tool calls/i });
      await user.click(listToggle);

      await waitFor(() => {
        expect(screen.getByText('workspace_read')).toBeInTheDocument();
        expect(screen.getByText('workspace_ls')).toBeInTheDocument();
      });

      // Verify both tools have results
      const successIndicators = screen.getAllByText('Tool executed successfully');
      expect(successIndicators).toHaveLength(2);
    });
  });

  describe('Edge Cases', () => {
    it('should handle null content gracefully', () => {
      const thoughtMessage: MessageData = {
        id: 'edge-1',
        role: 'assistant (thought)',
        content: null,
        timestamp: new Date().toISOString(),
        toolCalls: [
          {
            id: 'tool-1',
            type: 'tool_use',
            name: 'test_tool',
            input: { test: 'value' }
          }
        ]
      };

      expect(() => {
        render(<MessageFooter message={thoughtMessage} />);
      }).not.toThrow();

      expect(screen.getByText('1 tool')).toBeInTheDocument();
    });

    it('should handle tool calls without results', () => {
      const thoughtMessage: MessageData = {
        id: 'edge-2',
        role: 'assistant (thought)',
        content: 'Thought in progress',
        timestamp: new Date().toISOString(),
        toolCalls: [
          {
            id: 'tool-1',
            type: 'tool_use',
            name: 'pending_tool',
            input: { action: 'process' }
          }
        ]
        // No toolResults
      };

      render(<MessageFooter message={thoughtMessage} />);

      expect(screen.getByText('1 tool')).toBeInTheDocument();
    });

    it('should handle complex nested input arguments', async () => {
      const user = userEvent.setup();
      const thoughtMessage: MessageData = {
        id: 'edge-3',
        role: 'assistant (thought)',
        content: 'Complex operation',
        timestamp: new Date().toISOString(),
        toolCalls: [
          {
            id: 'tool-1',
            type: 'tool_use',
            name: 'complex_tool',
            input: {
              nested: {
                deep: {
                  value: 'test',
                  array: [1, 2, 3],
                  object: { key: 'val' }
                }
              }
            }
          }
        ]
      };

      render(<MessageFooter message={thoughtMessage} />);

      const toggleButton = screen.getByRole('button', { name: /show tool calls/i });
      await user.click(toggleButton);

      await waitFor(() => {
        expect(screen.getByText('complex_tool')).toBeInTheDocument();
      });

      // Expand tool to see input
      const expandButton = screen.getByRole('button', { name: /expand complex_tool details/i });
      await user.click(expandButton);

      await waitFor(() => {
        expect(screen.getByText('Input Arguments')).toBeInTheDocument();
      });

      // Check for nested structure in formatted JSON
      const codeElement = screen.getByText(/"nested":/);
      expect(codeElement).toBeInTheDocument();
    });

    it('should handle very long result content with details/summary', async () => {
      const user = userEvent.setup();
      const longResult = 'x'.repeat(600);
      
      const thoughtMessage: MessageData = {
        id: 'edge-4',
        role: 'assistant (thought)',
        content: 'Large result',
        timestamp: new Date().toISOString(),
        toolCalls: [
          {
            id: 'tool-1',
            type: 'tool_use',
            name: 'large_output_tool',
            input: { query: 'all' }
          }
        ],
        toolResults: [
          {
            type: 'tool_result',
            tool_use_id: 'tool-1',
            content: longResult
          }
        ]
      };

      render(<MessageFooter message={thoughtMessage} />);

      const toggleButton = screen.getByRole('button', { name: /show tool calls/i });
      await user.click(toggleButton);

      await waitFor(() => {
        expect(screen.getByText('large_output_tool')).toBeInTheDocument();
      });

      const expandButton = screen.getByRole('button', { name: /expand large_output_tool details/i });
      await user.click(expandButton);

      await waitFor(() => {
        const summary = screen.getByText(/Show full result/i);
        expect(summary).toBeInTheDocument();
        expect(summary.textContent).toContain('600 characters');
      });
    });

    it('should handle malformed tool result JSON gracefully', async () => {
      const user = userEvent.setup();
      const thoughtMessage: MessageData = {
        id: 'edge-5',
        role: 'assistant (thought)',
        content: 'Error handling',
        timestamp: new Date().toISOString(),
        toolCalls: [
          {
            id: 'tool-1',
            type: 'tool_use',
            name: 'error_tool',
            input: { action: 'test' }
          }
        ],
        toolResults: [
          {
            type: 'tool_result',
            tool_use_id: 'tool-1',
            content: 'Not valid JSON {{{' // Malformed
          }
        ]
      };

      render(<MessageFooter message={thoughtMessage} />);

      const toggleButton = screen.getByRole('button', { name: /show tool calls/i });
      await user.click(toggleButton);

      await waitFor(() => {
        expect(screen.getByText('error_tool')).toBeInTheDocument();
      });

      // Should not throw error, should display as-is
      expect(() => {
        const expandButton = screen.getByRole('button', { name: /expand error_tool details/i });
        user.click(expandButton);
      }).not.toThrow();
    });

    it('should show timestamp when showTimestamp is true', () => {
      const thoughtMessage: MessageData = {
        id: 'edge-6',
        role: 'assistant (thought)',
        content: 'Timestamped thought',
        timestamp: new Date('2024-01-15T14:30:00Z').toISOString(),
        toolCalls: [
          {
            id: 'tool-1',
            type: 'tool_use',
            name: 'test_tool',
            input: { test: 'value' }
          }
        ]
      };

      render(<MessageFooter message={thoughtMessage} showTimestamp={true} />);

      expect(screen.getByTestId('clock-icon')).toBeInTheDocument();
      // Timestamp format varies by locale, just check it exists
      const timeElements = screen.getAllByText(/\d{1,2}:\d{2}/);
      expect(timeElements.length).toBeGreaterThan(0);
    });

    it('should not show timestamp when showTimestamp is false', () => {
      const thoughtMessage: MessageData = {
        id: 'edge-7',
        role: 'assistant (thought)',
        content: 'No timestamp thought',
        timestamp: new Date().toISOString(),
        toolCalls: [
          {
            id: 'tool-1',
            type: 'tool_use',
            name: 'test_tool',
            input: { test: 'value' }
          }
        ]
      };

      render(<MessageFooter message={thoughtMessage} showTimestamp={false} />);

      expect(screen.queryByTestId('clock-icon')).not.toBeInTheDocument();
    });
  });

  describe('Copy Functionality', () => {
    it('should allow copying tool input arguments', async () => {
      const user = userEvent.setup();
      const thoughtMessage: MessageData = {
        id: 'copy-1',
        role: 'assistant (thought)',
        content: 'Copy test',
        timestamp: new Date().toISOString(),
        toolCalls: [
          {
            id: 'tool-1',
            type: 'tool_use',
            name: 'test_tool',
            input: { param1: 'value1', param2: 42 }
          }
        ]
      };

      render(<MessageFooter message={thoughtMessage} />);

      // Expand tool calls list
      const listToggle = screen.getByRole('button', { name: /show tool calls/i });
      await user.click(listToggle);

      await waitFor(() => {
        expect(screen.getByText('test_tool')).toBeInTheDocument();
      });

      // Expand tool details
      const expandButton = screen.getByRole('button', { name: /expand test_tool details/i });
      await user.click(expandButton);

      await waitFor(() => {
        expect(screen.getByText('Input Arguments')).toBeInTheDocument();
      });

      // Find and click copy button for input
      const copyButtons = screen.getAllByRole('button', { name: /copy/i });
      const inputCopyButton = copyButtons.find(btn => 
        within(btn.parentElement!).queryByText('Input Arguments') !== null ||
        btn.parentElement?.parentElement?.textContent?.includes('Input Arguments')
      );

      if (inputCopyButton) {
        await user.click(inputCopyButton);

        await waitFor(() => {
          expect(mockWriteText).toHaveBeenCalledWith(
            expect.stringContaining('"param1": "value1"')
          );
        });
      }
    });

    it('should allow copying tool result', async () => {
      const user = userEvent.setup();
      const thoughtMessage: MessageData = {
        id: 'copy-2',
        role: 'assistant (thought)',
        content: 'Result copy test',
        timestamp: new Date().toISOString(),
        toolCalls: [
          {
            id: 'tool-1',
            type: 'tool_use',
            name: 'test_tool',
            input: { action: 'get_data' }
          }
        ],
        toolResults: [
          {
            type: 'tool_result',
            tool_use_id: 'tool-1',
            content: JSON.stringify({ result: 'success', data: 'test_data' })
          }
        ]
      };

      render(<MessageFooter message={thoughtMessage} />);

      // Expand tool calls list
      const listToggle = screen.getByRole('button', { name: /show tool calls/i });
      await user.click(listToggle);

      await waitFor(() => {
        expect(screen.getByText('test_tool')).toBeInTheDocument();
      });

      // Expand tool details
      const expandButton = screen.getByRole('button', { name: /expand test_tool details/i });
      await user.click(expandButton);

      await waitFor(() => {
        expect(screen.getByText('Result')).toBeInTheDocument();
      });

      // Find copy button for result (should be second one)
      const copyButtons = screen.getAllByRole('button', { name: /copy/i });
      // Last copy button should be for result
      const resultCopyButton = copyButtons[copyButtons.length - 1];
      
      await user.click(resultCopyButton);

      await waitFor(() => {
        expect(mockWriteText).toHaveBeenCalledWith(
          expect.stringContaining('"result": "success"')
        );
      });
    });
  });

  describe('Token Count Display', () => {
    it('should display token counts for thought messages with metadata', () => {
      const thoughtMessage: MessageData = {
        id: 'token-1',
        role: 'assistant (thought)',
        content: 'Thought with tokens',
        timestamp: new Date().toISOString(),
        metadata: {
          inputTokens: 50,
          outputTokens: 120
        },
        toolCalls: []
      };

      render(<MessageFooter message={thoughtMessage} />);

      expect(screen.getByText('50')).toBeInTheDocument();
      expect(screen.getByText('120')).toBeInTheDocument();
      expect(screen.getByText('170')).toBeInTheDocument(); // Total
    });
  });

  describe('Accessibility', () => {
    it('should have proper ARIA labels for tool expansion', () => {
      const thoughtMessage: MessageData = {
        id: 'a11y-1',
        role: 'assistant (thought)',
        content: 'Accessible thought',
        timestamp: new Date().toISOString(),
        toolCalls: [
          {
            id: 'tool-1',
            type: 'tool_use',
            name: 'test_tool',
            input: { test: 'value' }
          }
        ]
      };

      render(<MessageFooter message={thoughtMessage} />);

      const toggleButton = screen.getByRole('button', { name: /show tool calls/i });
      expect(toggleButton).toHaveAttribute('aria-label');
    });

    it('should have proper ARIA labels for individual tool expansion', async () => {
      const user = userEvent.setup();
      const thoughtMessage: MessageData = {
        id: 'a11y-2',
        role: 'assistant (thought)',
        content: 'Tool accessibility',
        timestamp: new Date().toISOString(),
        toolCalls: [
          {
            id: 'tool-1',
            type: 'tool_use',
            name: 'workspace_read',
            input: { path: 'test.txt' }
          }
        ]
      };

      render(<MessageFooter message={thoughtMessage} />);

      const listToggle = screen.getByRole('button', { name: /show tool calls/i });
      await user.click(listToggle);

      await waitFor(() => {
        const expandButton = screen.getByRole('button', { name: /expand workspace_read details/i });
        expect(expandButton).toHaveAttribute('aria-label');
        expect(expandButton).toHaveAttribute('aria-expanded', 'false');
      });
    });
  });
});
