import { describe, it, expect } from 'vitest';
import { ClientWantsCancelEvent, CancelledEvent } from '../ClientEvents';
import { CancelledEvent as ServerCancelledEvent } from '../ServerEvents';
import { ClientEventMap, ServerEventMap } from '../../EventRegistry';

describe('Cancel Events', () => {
  describe('ClientWantsCancelEvent', () => {
    it('should have correct type structure', () => {
      const event: ClientWantsCancelEvent = {
        type: 'client_wants_cancel'
      };
      
      expect(event.type).toBe('client_wants_cancel');
    });
    
    it('should be in ClientEventMap', () => {
      // Type checking - this will fail to compile if not in the map
      const eventMap: ClientEventMap = {
        client_wants_cancel: { type: 'client_wants_cancel' }
      } as ClientEventMap;
      
      expect(eventMap.client_wants_cancel).toBeDefined();
    });
  });
  
  describe('CancelledEvent', () => {
    it('should have correct type structure', () => {
      const event: ServerCancelledEvent = {
        type: 'cancelled'
      };
      
      expect(event.type).toBe('cancelled');
    });
    
    it('should be in ServerEventMap', () => {
      // Type checking - this will fail to compile if not in the map
      const eventMap: ServerEventMap = {
        cancelled: { type: 'cancelled' }
      } as ServerEventMap;
      
      expect(eventMap.cancelled).toBeDefined();
    });
  });
});