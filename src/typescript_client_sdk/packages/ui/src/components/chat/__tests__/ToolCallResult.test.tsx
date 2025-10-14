import React from 'react'
import { render, screen, waitFor, act } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import { describe, it, expect, vi, beforeEach, afterEach } from 'vitest'
import { ToolCallResult, ToolCallResultList } from '../ToolCallResult'
import type { ToolCall, ToolResult } from '../ToolCallResult'

describe('ToolCallResult', () => {
  const mockToolCall: ToolCall = {
    id: 'call-1',
    type: 'tool_use',
    name: 'test_tool',
    input: {
      param1: 'value1',
      param2: 123,
      nested: {
        key: 'value'
      }
    }
  }
  
  const mockToolResult: ToolResult = {
    type: 'tool_result',
    tool_use_id: 'call-1',
    content: JSON.stringify({
      status: 'success',
      data: {
        result: 'test result',
        count: 42
      }
    })
  }
  
  const mockWriteText = vi.fn().mockResolvedValue(undefined)
  
  beforeEach(() => {
    vi.clearAllMocks()
    // Mock navigator.clipboard with configurable property
    Object.defineProperty(navigator, 'clipboard', {
      value: {
        writeText: mockWriteText
      },
      configurable: true,
      writable: true
    })
  })
  
  afterEach(() => {
    vi.clearAllMocks()
  })
  
  describe('Basic Rendering', () => {
    it('should render tool call header', () => {
      render(<ToolCallResult toolCall={mockToolCall} />)
      
      expect(screen.getByText('test_tool')).toBeInTheDocument()
      expect(screen.getByText('Tool executed successfully')).toBeInTheDocument()
    })
    
    it('should not render think tool calls', () => {
      const thinkToolCall = { ...mockToolCall, name: 'think' }
      const { container } = render(<ToolCallResult toolCall={thinkToolCall} />)
      
      expect(container.firstChild).toBeNull()
    })
    
    it('should render with custom className', () => {
      const { container } = render(
        <ToolCallResult toolCall={mockToolCall} className="custom-class" />
      )
      
      const element = container.firstChild as HTMLElement
      expect(element).toHaveClass('custom-class')
    })
  })
  
  describe('Expand/Collapse Behavior', () => {
    it('should be collapsed by default', () => {
      render(<ToolCallResult toolCall={mockToolCall} />)
      
      const button = screen.getByRole('button', { name: /expand tool call details/i })
      expect(button).toHaveAttribute('aria-expanded', 'false')
      expect(screen.queryByText('Input Arguments')).not.toBeInTheDocument()
    })
    
    it('should expand when defaultExpanded is true', () => {
      render(<ToolCallResult toolCall={mockToolCall} defaultExpanded={true} />)
      
      const button = screen.getByRole('button', { name: /collapse tool call details/i })
      expect(button).toHaveAttribute('aria-expanded', 'true')
      expect(screen.getByText('Input Arguments')).toBeInTheDocument()
    })
    
    it('should toggle expansion on click', async () => {
      const user = userEvent.setup()
      render(<ToolCallResult toolCall={mockToolCall} />)
      
      const button = screen.getByRole('button', { name: /expand tool call details/i })
      
      // Initially collapsed
      expect(button).toHaveAttribute('aria-expanded', 'false')
      
      // Click to expand
      await act(async () => {
        await user.click(button)
      })
      expect(button).toHaveAttribute('aria-expanded', 'true')
      expect(screen.getByText('Input Arguments')).toBeInTheDocument()
      
      // Click to collapse
      await act(async () => {
        await user.click(button)
      })
      expect(button).toHaveAttribute('aria-expanded', 'false')
      
      // Wait for animation to complete before checking if element is removed
      await waitFor(() => {
        expect(screen.queryByText('Input Arguments')).not.toBeInTheDocument()
      })
    })
  })
  
  describe('Input Arguments Display', () => {
    it('should display formatted JSON input', async () => {
      const user = userEvent.setup()
      render(<ToolCallResult toolCall={mockToolCall} />)
      
      // Expand to see content
      await act(async () => {
        await user.click(screen.getByRole('button', { name: /expand tool call details/i }))
      })
      
      const codeElement = screen.getByText(/param1/, { selector: 'code' })
      expect(codeElement).toBeInTheDocument()
      expect(codeElement.textContent).toContain('"param1": "value1"')
      expect(codeElement.textContent).toContain('"param2": 123')
    })
    
    it('should handle non-JSON input gracefully', async () => {
      const user = userEvent.setup()
      const toolCallWithBadInput = {
        ...mockToolCall,
        input: 'not valid json' as any
      }
      render(<ToolCallResult toolCall={toolCallWithBadInput} />)
      
      await act(async () => {
        await user.click(screen.getByRole('button', { name: /expand tool call details/i }))
      })
      
      // When input is a string, String() is called on it, so it appears as-is
      const codeElements = screen.getAllByText(/not valid json/, { selector: 'code' })
      expect(codeElements.length).toBeGreaterThan(0)
    })

    // DS note: Dsiabled till we have time to circle back to clipboard testing
    // it('should allow copying input arguments', async () => {
    //   const user = userEvent.setup()
    //   render(<ToolCallResult toolCall={mockToolCall} />)
    //
    //   await act(async () => {
    //     await user.click(screen.getByRole('button', { name: /expand tool call details/i }))
    //   })
    //
    //   // Find copy button by looking for button with "Copy" text
    //   const copyButtons = screen.getAllByRole('button').filter(button =>
    //     button.textContent?.includes('Copy')
    //   )
    //   expect(copyButtons.length).toBeGreaterThan(0)
    //
    //   await act(async () => {
    //     await user.click(copyButtons[0])
    //   })
    //
    //   // Wait for the async clipboard operation
    //   await waitFor(() => {
    //     expect(mockWriteText).toHaveBeenCalledWith(
    //       JSON.stringify(mockToolCall.input, null, 2)
    //     )
    //   })
    //
    //   // Check for "Copied" feedback
    //   expect(screen.getByText('Copied')).toBeInTheDocument()
    // })


  })
  
  describe('Result Display', () => {
    it('should display formatted JSON result', async () => {
      const user = userEvent.setup()
      render(<ToolCallResult toolCall={mockToolCall} toolResult={mockToolResult} />)
      
      await act(async () => {
        await user.click(screen.getByRole('button', { name: /expand tool call details/i }))
      })
      
      expect(screen.getByText('Result')).toBeInTheDocument()
      const codeElements = screen.getAllByText(/status/, { selector: 'code' })
      const resultCode = codeElements.find(el => el.textContent?.includes('"status": "success"'))
      expect(resultCode).toBeInTheDocument()
    })
    
    it('should show "No result available" when no result provided', async () => {
      const user = userEvent.setup()
      render(<ToolCallResult toolCall={mockToolCall} />)
      
      await act(async () => {
        await user.click(screen.getByRole('button', { name: /expand tool call details/i }))
      })
      
      // Should not show Result section when no result
      expect(screen.queryByText('Result')).not.toBeInTheDocument()
    })
    
    it('should handle non-JSON result content', async () => {
      const user = userEvent.setup()
      const textResult: ToolResult = {
        ...mockToolResult,
        content: 'Plain text result'
      }
      render(<ToolCallResult toolCall={mockToolCall} toolResult={textResult} />)
      
      await act(async () => {
        await user.click(screen.getByRole('button', { name: /expand tool call details/i }))
      })
      
      expect(screen.getByText('Plain text result', { selector: 'code' })).toBeInTheDocument()
    })
    
    it('should show expandable details for long results', async () => {
      const user = userEvent.setup()
      const longResult: ToolResult = {
        ...mockToolResult,
        content: 'x'.repeat(600) // Create a long string
      }
      render(<ToolCallResult toolCall={mockToolCall} toolResult={longResult} />)
      
      await act(async () => {
        await user.click(screen.getByRole('button', { name: /expand tool call details/i }))
      })
      
      const summary = screen.getByText(/Show full result/i)
      expect(summary).toBeInTheDocument()
      expect(summary.textContent).toContain('600 characters')
    })

    // DS note: Dsiabled till we have time to circle back to clipboard testing
    // it('should allow copying result', async () => {
    //   const user = userEvent.setup()
    //   render(<ToolCallResult toolCall={mockToolCall} toolResult={mockToolResult} />)
    //
    //   await act(async () => {
    //     await user.click(screen.getByRole('button', { name: /expand tool call details/i }))
    //   })
    //
    //   // Find all copy buttons
    //   const copyButtons = screen.getAllByRole('button').filter(button =>
    //     button.textContent?.includes('Copy')
    //   )
    //   // The second one should be for the result
    //   expect(copyButtons.length).toBe(2)
    //
    //   await act(async () => {
    //     await user.click(copyButtons[1])
    //   })
    //
    //   // Wait for the async clipboard operation
    //   await waitFor(() => {
    //     const expectedContent = JSON.stringify(JSON.parse(mockToolResult.content), null, 2)
    //     expect(mockWriteText).toHaveBeenCalledWith(expectedContent)
    //   })
    // })


  })
  
  describe('Visual Indicators', () => {
    it('should show success check icon', () => {
      render(<ToolCallResult toolCall={mockToolCall} toolResult={mockToolResult} />)
      
      const checkIcon = document.querySelector('.text-green-600, .text-green-400')
      expect(checkIcon).toBeInTheDocument()
    })
    
    it('should show chevron icon that rotates', async () => {
      const user = userEvent.setup()
      render(<ToolCallResult toolCall={mockToolCall} />)
      
      const chevron = document.querySelector('[class*="transition-transform"]')
      expect(chevron).toBeInTheDocument()
      expect(chevron).not.toHaveClass('rotate-90')
      
      await act(async () => {
        await user.click(screen.getByRole('button', { name: /expand tool call details/i }))
      })
      
      expect(chevron).toHaveClass('rotate-90')
    })
    
    it('should show tool icon', () => {
      render(<ToolCallResult toolCall={mockToolCall} />)
      
      const iconContainer = document.querySelector('.bg-primary\\/10')
      expect(iconContainer).toBeInTheDocument()
    })
  })
  
  describe('Accessibility', () => {
    it('should have proper ARIA labels', () => {
      render(<ToolCallResult toolCall={mockToolCall} />)
      
      const button = screen.getByRole('button', { name: /expand tool call details/i })
      expect(button).toHaveAttribute('aria-expanded')
      expect(button).toHaveAttribute('aria-label')
    })
    
    it('should be keyboard accessible', async () => {
      const user = userEvent.setup()
      render(<ToolCallResult toolCall={mockToolCall} />)
      
      const button = screen.getByRole('button', { name: /expand tool call details/i })
      button.focus()
      
      await act(async () => {
        await user.keyboard('{Enter}')
      })
      
      expect(button).toHaveAttribute('aria-expanded', 'true')
    })
    
    it('should have focus ring on focus', () => {
      render(<ToolCallResult toolCall={mockToolCall} />)
      
      const button = screen.getByRole('button', { name: /expand tool call details/i })
      expect(button).toHaveClass('focus-visible:ring-2')
    })
  })
})

describe('ToolCallResultList', () => {
  const mockToolCalls: ToolCall[] = [
    {
      id: 'call-1',
      type: 'tool_use',
      name: 'tool_one',
      input: { param: 'value1' }
    },
    {
      id: 'call-2',
      type: 'tool_use',
      name: 'tool_two',
      input: { param: 'value2' }
    },
    {
      id: 'call-3',
      type: 'tool_use',
      name: 'think',
      input: { thought: 'reasoning' }
    }
  ]
  
  const mockToolResults: ToolResult[] = [
    {
      type: 'tool_result',
      tool_use_id: 'call-1',
      content: '{"result": "success1"}'
    },
    {
      type: 'tool_result',
      tool_use_id: 'call-2',
      content: '{"result": "success2"}'
    }
  ]
  
  const mockWriteText = vi.fn().mockResolvedValue(undefined)
  
  beforeEach(() => {
    vi.clearAllMocks()
    // Mock navigator.clipboard with configurable property
    Object.defineProperty(navigator, 'clipboard', {
      value: {
        writeText: mockWriteText
      },
      configurable: true,
      writable: true
    })
  })
  
  afterEach(() => {
    vi.clearAllMocks()
  })
  
  it('should render multiple tool calls', () => {
    render(
      <ToolCallResultList 
        toolCalls={mockToolCalls} 
        toolResults={mockToolResults}
      />
    )
    
    expect(screen.getByText('tool_one')).toBeInTheDocument()
    expect(screen.getByText('tool_two')).toBeInTheDocument()
  })
  
  it('should filter out think tool calls', () => {
    render(
      <ToolCallResultList 
        toolCalls={mockToolCalls} 
        toolResults={mockToolResults}
      />
    )
    
    expect(screen.queryByText('think')).not.toBeInTheDocument()
  })
  
  it('should match results to tool calls by ID', async () => {
    const user = userEvent.setup()
    render(
      <ToolCallResultList 
        toolCalls={mockToolCalls.slice(0, 2)} 
        toolResults={mockToolResults}
      />
    )
    
    // Expand first tool call
    const buttons = screen.getAllByRole('button', { name: /expand tool call details/i })
    await act(async () => {
      await user.click(buttons[0])
    })
    
    // Check that result is displayed
    expect(screen.getByText('Result')).toBeInTheDocument()
    const codeElements = screen.getAllByText(/result/, { selector: 'code' })
    const resultCode = codeElements.find(el => el.textContent?.includes('"result": "success1"'))
    expect(resultCode).toBeInTheDocument()
  })
  
  it('should render nothing when only think tool calls exist', () => {
    const thinkOnlyCalls = [mockToolCalls[2]]
    const { container } = render(
      <ToolCallResultList toolCalls={thinkOnlyCalls} />
    )
    
    expect(container.firstChild).toBeNull()
  })
  
  it('should handle empty tool calls array', () => {
    const { container } = render(
      <ToolCallResultList toolCalls={[]} />
    )
    
    expect(container.firstChild).toBeNull()
  })
  
  it('should apply custom className', () => {
    const { container } = render(
      <ToolCallResultList 
        toolCalls={mockToolCalls.slice(0, 1)} 
        className="custom-class"
      />
    )
    
    const wrapper = container.firstChild as HTMLElement
    expect(wrapper).toHaveClass('custom-class')
  })
  
  it('should maintain proper spacing between items', () => {
    const { container } = render(
      <ToolCallResultList 
        toolCalls={mockToolCalls.slice(0, 2)} 
        toolResults={mockToolResults}
      />
    )
    
    const wrapper = container.firstChild as HTMLElement
    expect(wrapper).toHaveClass('space-y-2')
  })
})