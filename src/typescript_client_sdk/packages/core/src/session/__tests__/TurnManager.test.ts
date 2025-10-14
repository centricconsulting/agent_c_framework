/**
 * Unit tests for TurnManager
 * Tests simple state management and event handling
 */

import { describe, it, expect, beforeEach, afterEach, vi } from 'vitest';
import { TurnManager } from '../TurnManager';
import { RealtimeClient } from '../../client/RealtimeClient';
import { serverEventFixtures } from '../../test/fixtures/protocol-events';

// Mock RealtimeClient
vi.mock('../../client/RealtimeClient');

describe('TurnManager', () => {
  let turnManager: TurnManager;
  let mockClient: RealtimeClient;

  beforeEach(() => {
    vi.clearAllMocks();
    
    // Create a mock RealtimeClient
    mockClient = {
      on: vi.fn(),
      off: vi.fn(),
      emit: vi.fn(),
      removeAllListeners: vi.fn(),
    } as unknown as RealtimeClient;

    turnManager = new TurnManager(mockClient);
  });

  afterEach(() => {
    vi.restoreAllMocks();
  });

  describe('constructor', () => {
    it('should initialize with canSendInput as false', () => {
      expect(turnManager.canSendInput).toBe(false);
    });

    it('should register event listeners on the client', () => {
      expect(mockClient.on).toHaveBeenCalledWith('user_turn_start', expect.any(Function));
      expect(mockClient.on).toHaveBeenCalledWith('user_turn_end', expect.any(Function));
      expect(mockClient.on).toHaveBeenCalledTimes(2);
    });
  });

  describe('canSendInput getter', () => {
    it('should return the current boolean state', () => {
      expect(turnManager.canSendInput).toBe(false);
      
      // Get the registered handler for user_turn_start
      const startHandler = (mockClient.on as ReturnType<typeof vi.fn>).mock.calls
        .find(call => call[0] === 'user_turn_start')?.[1];
      
      // Simulate user turn start event
      if (startHandler) {
        startHandler(serverEventFixtures.userTurnStart);
      }
      
      expect(turnManager.canSendInput).toBe(true);
    });
  });

  describe('event handlers', () => {
    it('should set canSendInput to true on user_turn_start event', () => {
      // Get the registered handler
      const handler = (mockClient.on as ReturnType<typeof vi.fn>).mock.calls
        .find(call => call[0] === 'user_turn_start')?.[1];
      
      expect(handler).toBeDefined();
      expect(turnManager.canSendInput).toBe(false);
      
      // Trigger the handler with the fixture
      handler(serverEventFixtures.userTurnStart);
      
      expect(turnManager.canSendInput).toBe(true);
    });

    it('should set canSendInput to false on user_turn_end event', () => {
      // First set it to true
      const startHandler = (mockClient.on as ReturnType<typeof vi.fn>).mock.calls
        .find(call => call[0] === 'user_turn_start')?.[1];
      startHandler(serverEventFixtures.userTurnStart);
      expect(turnManager.canSendInput).toBe(true);
      
      // Get the end handler
      const endHandler = (mockClient.on as ReturnType<typeof vi.fn>).mock.calls
        .find(call => call[0] === 'user_turn_end')?.[1];
      
      expect(endHandler).toBeDefined();
      
      // Trigger the handler with the fixture
      endHandler(serverEventFixtures.userTurnEnd);
      
      expect(turnManager.canSendInput).toBe(false);
    });

    it('should emit turn-state-changed event when state changes', () => {
      const listener = vi.fn();
      turnManager.on('turn-state-changed', listener);
      
      // Get the start handler
      const startHandler = (mockClient.on as ReturnType<typeof vi.fn>).mock.calls
        .find(call => call[0] === 'user_turn_start')?.[1];
      
      // Trigger state change
      startHandler(serverEventFixtures.userTurnStart);
      
      expect(listener).toHaveBeenCalledWith({ canSendInput: true });
    });

    it('should not emit turn-state-changed if state does not change', () => {
      const listener = vi.fn();
      turnManager.on('turn-state-changed', listener);
      
      // Get the end handler (already false, so no change)
      const endHandler = (mockClient.on as ReturnType<typeof vi.fn>).mock.calls
        .find(call => call[0] === 'user_turn_end')?.[1];
      
      // Trigger with same state
      endHandler(serverEventFixtures.userTurnEnd);
      
      expect(listener).not.toHaveBeenCalled();
    });
  });

  describe('cleanup', () => {
    it('should remove listeners from client', () => {
      turnManager.cleanup();
      
      expect(mockClient.off).toHaveBeenCalledWith('user_turn_start', expect.any(Function));
      expect(mockClient.off).toHaveBeenCalledWith('user_turn_end', expect.any(Function));
      expect(mockClient.off).toHaveBeenCalledTimes(2);
    });

    it('should remove all internal listeners', () => {
      const removeAllListenersSpy = vi.spyOn(turnManager, 'removeAllListeners');
      
      turnManager.cleanup();
      
      expect(removeAllListenersSpy).toHaveBeenCalled();
    });
  });

  describe('destroy', () => {
    it('should call cleanup when destroy is called', () => {
      const cleanupSpy = vi.spyOn(turnManager, 'cleanup');
      
      turnManager.destroy();
      
      expect(cleanupSpy).toHaveBeenCalled();
    });
  });
});