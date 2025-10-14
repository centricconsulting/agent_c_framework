import { render, screen, waitFor } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { describe, it, expect, vi } from 'vitest';
import { Message } from '../Message';

describe('Message Edit Button Fix Verification', () => {
  it('should render only one edit button for user messages', async () => {
    const mockUserMessage = {
      id: 'msg-1',
      role: 'user' as const,
      content: 'Test message',
      timestamp: new Date().toISOString(),
      format: 'text' as const
    };
    
    const { container } = render(
      <Message 
        message={mockUserMessage}
        onEdit={vi.fn()}
        showFooter={true}
      />
    );
    
    // Hover to reveal buttons
    const messageElement = container.querySelector('.group');
    if (!messageElement) {
      throw new Error('Message element not found');
    }
    
    await userEvent.hover(messageElement);
    
    await waitFor(() => {
      const editButtons = screen.queryAllByText('Edit');
      // Should be exactly 1 edit button (not 2)
      expect(editButtons).toHaveLength(1);
    });
  });
});