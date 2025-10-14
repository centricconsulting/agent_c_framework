import { describe, it, expect, vi, beforeEach } from 'vitest';
import { EventEmitter } from '../EventEmitter';
import { serverEventFixtures, clientEventFixtures } from '../../test/fixtures/protocol-events';
import type { TextDeltaEvent, CompletionEvent } from '../types/ServerEvents';
import type { TextInputEvent } from '../types/ClientEvents';

describe('EventEmitter', () => {
  let emitter: EventEmitter;

  beforeEach(() => {
    emitter = new EventEmitter();
    vi.clearAllMocks();
  });

  describe('on()', () => {
    it('should add a listener that gets called when emit() is invoked', () => {
      const listener = vi.fn();
      const textDelta = serverEventFixtures.textDelta;

      emitter.on('text_delta', listener);
      emitter.emit('text_delta', textDelta);

      expect(listener).toHaveBeenCalledTimes(1);
      expect(listener).toHaveBeenCalledWith(textDelta);
    });

    it('should return this for chaining', () => {
      const listener = vi.fn();
      
      const result = emitter.on('completion', listener);
      
      expect(result).toBe(emitter);
    });

    it('should call multiple listeners for the same event', () => {
      const listener1 = vi.fn();
      const listener2 = vi.fn();
      const textInput = clientEventFixtures.textInput;

      emitter.on('text_input', listener1);
      emitter.on('text_input', listener2);
      emitter.emit('text_input', textInput);

      expect(listener1).toHaveBeenCalledWith(textInput);
      expect(listener2).toHaveBeenCalledWith(textInput);
    });
  });

  describe('once()', () => {
    it('should add a listener that gets called once then auto-removed', () => {
      const listener = vi.fn();
      const completion = serverEventFixtures.completionFinished;

      emitter.once('completion', listener);
      
      // First emit - listener should be called
      emitter.emit('completion', completion);
      expect(listener).toHaveBeenCalledTimes(1);
      expect(listener).toHaveBeenCalledWith(completion);

      // Second emit - listener should not be called again
      emitter.emit('completion', completion);
      expect(listener).toHaveBeenCalledTimes(1);
    });

    it('should return this for chaining', () => {
      const listener = vi.fn();
      
      const result = emitter.once('text_delta', listener);
      
      expect(result).toBe(emitter);
    });
  });

  describe('emit()', () => {
    it('should return true when listeners exist', () => {
      const listener = vi.fn();
      emitter.on('error', listener);

      const result = emitter.emit('error', serverEventFixtures.error);

      expect(result).toBe(true);
    });

    it('should return false when no listeners exist', () => {
      const result = emitter.emit('pong', serverEventFixtures.pong);

      expect(result).toBe(false);
    });

    it('should call listeners with the provided data', () => {
      const listener = vi.fn();
      const userRequest = serverEventFixtures.userRequest;

      emitter.on('user_request', listener);
      emitter.emit('user_request', userRequest);

      expect(listener).toHaveBeenCalledWith(userRequest);
    });
  });

  describe('off()', () => {
    it('should remove a listener so it does not get called', () => {
      const listener = vi.fn();
      const ping = clientEventFixtures.ping;

      emitter.on('ping', listener);
      emitter.off('ping', listener);
      emitter.emit('ping', ping);

      expect(listener).not.toHaveBeenCalled();
    });

    it('should return this for chaining', () => {
      const listener = vi.fn();
      emitter.on('ping', listener);
      
      const result = emitter.off('ping', listener);
      
      expect(result).toBe(emitter);
    });

    it('should only remove the specified listener', () => {
      const listener1 = vi.fn();
      const listener2 = vi.fn();
      const voiceList = serverEventFixtures.voiceList;

      emitter.on('voice_list', listener1);
      emitter.on('voice_list', listener2);
      emitter.off('voice_list', listener1);
      emitter.emit('voice_list', voiceList);

      expect(listener1).not.toHaveBeenCalled();
      expect(listener2).toHaveBeenCalledWith(voiceList);
    });
  });

  describe('removeAllListeners()', () => {
    it('should clear all listeners for a specific event', () => {
      const listener1 = vi.fn();
      const listener2 = vi.fn();
      const systemMessage = serverEventFixtures.systemMessage;

      emitter.on('system_message', listener1);
      emitter.on('system_message', listener2);
      emitter.removeAllListeners('system_message');
      emitter.emit('system_message', systemMessage);

      expect(listener1).not.toHaveBeenCalled();
      expect(listener2).not.toHaveBeenCalled();
    });

    it('should clear all listeners when no event is specified', () => {
      const listener1 = vi.fn();
      const listener2 = vi.fn();
      const listener3 = vi.fn();

      emitter.on('text_delta', listener1);
      emitter.on('completion', listener2);
      emitter.on('error', listener3);
      emitter.removeAllListeners();

      emitter.emit('text_delta', serverEventFixtures.textDelta);
      emitter.emit('completion', serverEventFixtures.completionRunning);
      emitter.emit('error', serverEventFixtures.error);

      expect(listener1).not.toHaveBeenCalled();
      expect(listener2).not.toHaveBeenCalled();
      expect(listener3).not.toHaveBeenCalled();
    });

    it('should return this for chaining', () => {
      const result = emitter.removeAllListeners();
      
      expect(result).toBe(emitter);
    });
  });
});