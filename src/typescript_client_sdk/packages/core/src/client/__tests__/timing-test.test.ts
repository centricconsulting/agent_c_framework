import { describe, it, expect, vi, beforeEach } from 'vitest';

describe('Timing Test', () => {
  it('should demonstrate setTimeout timing issue', async () => {
    const mockFn = vi.fn();
    
    // Simulate what handlePostInitializationRecovery does
    function deferredCall() {
      setTimeout(() => {
        mockFn('deferred');
      }, 0);
    }
    
    deferredCall();
    
    // Synchronous check - will fail
    console.log('Synchronous check:', mockFn.mock.calls.length);
    expect(mockFn.mock.calls.length).toBe(0); // Should be 0
    
    // Wait for next tick
    await new Promise(resolve => setTimeout(resolve, 0));
    
    // Async check - will pass
    console.log('After await:', mockFn.mock.calls.length);
    expect(mockFn.mock.calls.length).toBe(1);
    expect(mockFn).toHaveBeenCalledWith('deferred');
  });
});