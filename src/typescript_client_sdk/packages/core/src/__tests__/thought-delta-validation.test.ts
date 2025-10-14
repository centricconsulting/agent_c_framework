import { describe, it, expect } from 'vitest';
import { MessageBuilder } from '../events/MessageBuilder';

describe('MessageBuilder - Thought Delta Fix Validation', () => {
  it('should create thought messages with role "assistant (thought)" and type "message"', () => {
    const builder = new MessageBuilder();
    
    // Start a thought message
    builder.startMessage('thought');
    builder.appendText('This is a thought process');
    
    const thoughtMessage = builder.finalize();
    
    // Critical validations for the fix
    expect(thoughtMessage.role).toBe('assistant (thought)');
    expect(thoughtMessage.type).toBe('message'); // NOT 'thought'
    expect(thoughtMessage.content).toBe('This is a thought process');
    expect(thoughtMessage.isCollapsed).toBe(true); // Should be collapsed by default
  });

  it('should create regular assistant messages with role "assistant" and type "message"', () => {
    const builder = new MessageBuilder();
    
    // Start a regular assistant message
    builder.startMessage('assistant');
    builder.appendText('This is a regular assistant message');
    
    const assistantMessage = builder.finalize();
    
    expect(assistantMessage.role).toBe('assistant');
    expect(assistantMessage.type).toBe('message');
    expect(assistantMessage.content).toBe('This is a regular assistant message');
    expect(assistantMessage.isCollapsed).toBeUndefined(); // Should not be collapsed
  });

  it('should handle thought to assistant transitions correctly', () => {
    const builder = new MessageBuilder();
    
    // First create a thought message
    builder.startMessage('thought');
    builder.appendText('Internal thinking...');
    const thoughtMsg = builder.finalize();
    
    // Then create a regular assistant message
    builder.startMessage('assistant');
    builder.appendText('Here is my response');
    const assistantMsg = builder.finalize();
    
    // Verify both messages have correct properties
    expect(thoughtMsg.role).toBe('assistant (thought)');
    expect(thoughtMsg.type).toBe('message');
    
    expect(assistantMsg.role).toBe('assistant');
    expect(assistantMsg.type).toBe('message');
  });

  it('should accumulate multiple thought deltas correctly', () => {
    const builder = new MessageBuilder();
    
    builder.startMessage('thought');
    
    const deltas = ['I need to ', 'think about ', 'this problem ', 'carefully.'];
    deltas.forEach(delta => builder.appendText(delta));
    
    const thoughtMessage = builder.finalize();
    
    expect(thoughtMessage.content).toBe('I need to think about this problem carefully.');
    expect(thoughtMessage.role).toBe('assistant (thought)');
    expect(thoughtMessage.type).toBe('message');
  });
});