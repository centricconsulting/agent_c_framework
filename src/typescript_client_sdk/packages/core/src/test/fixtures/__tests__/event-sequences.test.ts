/**
 * Test to verify event sequence fixtures are properly structured and usable
 */

import { describe, it, expect } from 'vitest';
import { 
  allEventSequences,
  getEventSequence,
  combineSequences,
  initializationSequence,
  textStreamingSequence,
  thoughtStreamingSequence,
  toolCallSequence,
  chatSessionChangedSequence,
  renderMediaSequence,
  audioStreamingSequence,
  errorSequence,
  cancellationSequence,
  complexInteractionSequence
} from '../event-sequences';
import { isServerEvent } from '../../../events/types/ServerEvents';

describe('Event Sequence Fixtures', () => {
  describe('Individual sequences', () => {
    it('should have valid initialization sequence', () => {
      expect(initializationSequence).toBeDefined();
      expect(initializationSequence.length).toBeGreaterThan(0);
      
      // Verify all events are valid ServerEvents
      initializationSequence.forEach(event => {
        expect(isServerEvent(event)).toBe(true);
      });
      
      // Check for expected event types
      const eventTypes = initializationSequence.map(e => e.type);
      expect(eventTypes).toContain('system_prompt');
      expect(eventTypes).toContain('user_turn_start');
    });

    it('should have valid text streaming sequence', () => {
      expect(textStreamingSequence).toBeDefined();
      expect(textStreamingSequence.length).toBeGreaterThan(0);
      
      textStreamingSequence.forEach(event => {
        expect(isServerEvent(event)).toBe(true);
      });
      
      const eventTypes = textStreamingSequence.map(e => e.type);
      expect(eventTypes).toContain('text_delta');
      expect(eventTypes).toContain('completion');
      expect(eventTypes).toContain('interaction');
    });

    it('should have valid thought streaming sequence', () => {
      expect(thoughtStreamingSequence).toBeDefined();
      expect(thoughtStreamingSequence.length).toBeGreaterThan(0);
      
      thoughtStreamingSequence.forEach(event => {
        expect(isServerEvent(event)).toBe(true);
      });
      
      const eventTypes = thoughtStreamingSequence.map(e => e.type);
      expect(eventTypes).toContain('thought_delta');
      expect(eventTypes).toContain('tool_select_delta');
      expect(eventTypes).toContain('text_delta');
    });

    it('should have valid tool call sequence', () => {
      expect(toolCallSequence).toBeDefined();
      expect(toolCallSequence.length).toBeGreaterThan(0);
      
      toolCallSequence.forEach(event => {
        expect(isServerEvent(event)).toBe(true);
      });
      
      const eventTypes = toolCallSequence.map(e => e.type);
      expect(eventTypes).toContain('tool_select_delta');
      expect(eventTypes).toContain('tool_call');
      expect(eventTypes).toContain('text_delta');
    });

    it('should have valid chat session changed sequence', () => {
      expect(chatSessionChangedSequence).toBeDefined();
      expect(chatSessionChangedSequence.length).toBeGreaterThan(0);
      
      chatSessionChangedSequence.forEach(event => {
        expect(isServerEvent(event)).toBe(true);
      });
      
      const chatEvent = chatSessionChangedSequence[0];
      expect(chatEvent.type).toBe('chat_session_changed');
      if (chatEvent.type === 'chat_session_changed') {
        expect(chatEvent.chat_session).toBeDefined();
        expect(chatEvent.chat_session.messages).toBeDefined();
        expect(Array.isArray(chatEvent.chat_session.messages)).toBe(true);
      }
    });

    it('should have valid render media sequence', () => {
      expect(renderMediaSequence).toBeDefined();
      expect(renderMediaSequence.length).toBeGreaterThan(0);
      
      renderMediaSequence.forEach(event => {
        expect(isServerEvent(event)).toBe(true);
      });
      
      const eventTypes = renderMediaSequence.map(e => e.type);
      expect(eventTypes).toContain('render_media');
      
      // Check media events have proper structure
      const mediaEvents = renderMediaSequence.filter(e => e.type === 'render_media');
      mediaEvents.forEach(event => {
        if (event.type === 'render_media') {
          expect(event.content_type).toBeDefined();
          expect(typeof event.foreign_content).toBe('boolean');
        }
      });
    });

    it('should have valid error sequence', () => {
      expect(errorSequence).toBeDefined();
      expect(errorSequence.length).toBeGreaterThan(0);
      
      errorSequence.forEach(event => {
        expect(isServerEvent(event)).toBe(true);
      });
      
      const eventTypes = errorSequence.map(e => e.type);
      expect(eventTypes).toContain('error');
      expect(eventTypes).toContain('system_message');
    });

    it('should have valid cancellation sequence', () => {
      expect(cancellationSequence).toBeDefined();
      expect(cancellationSequence.length).toBeGreaterThan(0);
      
      cancellationSequence.forEach(event => {
        expect(isServerEvent(event)).toBe(true);
      });
      
      const eventTypes = cancellationSequence.map(e => e.type);
      expect(eventTypes).toContain('cancelled');
      expect(eventTypes).toContain('text_delta');
    });

    it('should have valid complex interaction sequence', () => {
      expect(complexInteractionSequence).toBeDefined();
      expect(complexInteractionSequence.length).toBeGreaterThan(0);
      
      complexInteractionSequence.forEach(event => {
        expect(isServerEvent(event)).toBe(true);
      });
      
      // Complex sequence should have various event types
      const eventTypes = new Set(complexInteractionSequence.map(e => e.type));
      expect(eventTypes.size).toBeGreaterThan(5);
      expect(eventTypes.has('tool_select_delta')).toBe(true);
      expect(eventTypes.has('thought_delta')).toBe(true);
      expect(eventTypes.has('render_media')).toBe(true);
    });
  });

  describe('Helper functions', () => {
    it('should get sequences by name', () => {
      const textSeq = getEventSequence('textStreaming');
      expect(textSeq).toBe(textStreamingSequence);
      
      const toolSeq = getEventSequence('toolCall');
      expect(toolSeq).toBe(toolCallSequence);
    });

    it('should combine sequences', () => {
      const combined = combineSequences(
        [{ type: 'user_turn_start' }],
        [{ type: 'user_turn_end' }]
      );
      
      expect(combined.length).toBe(2);
      expect(combined[0].type).toBe('user_turn_start');
      expect(combined[1].type).toBe('user_turn_end');
    });
  });

  describe('All sequences collection', () => {
    it('should contain all expected sequences', () => {
      const sequenceNames = Object.keys(allEventSequences);
      
      expect(sequenceNames).toContain('initialization');
      expect(sequenceNames).toContain('textStreaming');
      expect(sequenceNames).toContain('thoughtStreaming');
      expect(sequenceNames).toContain('toolCall');
      expect(sequenceNames).toContain('multiToolCall');
      expect(sequenceNames).toContain('chatSessionChanged');
      expect(sequenceNames).toContain('renderMedia');
      expect(sequenceNames).toContain('foreignMedia');
      expect(sequenceNames).toContain('audioStreaming');
      expect(sequenceNames).toContain('error');
      expect(sequenceNames).toContain('cancellation');
      expect(sequenceNames).toContain('subsession');
      expect(sequenceNames).toContain('complexInteraction');
      expect(sequenceNames).toContain('sessionUpdate');
      expect(sequenceNames).toContain('agentConfig');
      expect(sequenceNames).toContain('healthCheck');
    });

    it('should have valid events in all sequences', () => {
      Object.entries(allEventSequences).forEach(([name, sequence]) => {
        expect(sequence).toBeDefined();
        expect(sequence.length).toBeGreaterThan(0);
        
        // Verify all events in sequence are valid
        sequence.forEach((event, index) => {
          expect(isServerEvent(event)).toBe(true);
        });
      });
    });
  });

  describe('Event structure validation', () => {
    it('should have proper session_id in session events', () => {
      const sessionEvents = textStreamingSequence.filter(e => 
        'session_id' in e && e.session_id !== undefined
      );
      
      sessionEvents.forEach(event => {
        if ('session_id' in event) {
          expect(typeof event.session_id).toBe('string');
          expect(event.session_id).toBeTruthy();
        }
      });
    });

    it('should have proper role in role-based events', () => {
      const roleEvents = textStreamingSequence.filter(e => 
        'role' in e && e.role !== undefined
      );
      
      roleEvents.forEach(event => {
        if ('role' in event) {
          expect(typeof event.role).toBe('string');
          expect(['assistant', 'assistant (thought)', 'user', 'system'].some(
            role => event.role === role
          )).toBe(true);
        }
      });
    });

    it('should have proper tool structure in tool events', () => {
      const toolEvents = toolCallSequence.filter(e => 
        e.type === 'tool_select_delta' || e.type === 'tool_call'
      );
      
      toolEvents.forEach(event => {
        if (event.type === 'tool_select_delta' || event.type === 'tool_call') {
          expect(event.tool_calls).toBeDefined();
          expect(Array.isArray(event.tool_calls)).toBe(true);
          
          event.tool_calls.forEach(toolCall => {
            expect(toolCall.id).toBeDefined();
            expect(toolCall.type).toBeDefined();
            expect(toolCall.name).toBeDefined();
          });
        }
      });
    });
  });
});