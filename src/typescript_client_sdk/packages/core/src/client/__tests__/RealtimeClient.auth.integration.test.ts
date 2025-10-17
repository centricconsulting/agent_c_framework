import { describe, it, expect, vi, beforeEach, afterEach } from 'vitest';
import { RealtimeClient } from '../RealtimeClient';

describe('RealtimeClient - Auth Integration Test', () => {
    
    describe('Real-world auth error scenarios', () => {
        it('should fail fast when creating client without auth', async () => {
            // This simulates the bug scenario - no auth provided
            // Auth validation now happens at connect() time, not construction time
            const client = new RealtimeClient({
                apiUrl: 'wss://test.example.com'
                // Missing authToken or authManager
            });
            
            // Should throw when attempting to connect
            await expect(client.connect()).rejects.toThrow('Authentication token is required');
        });
        
        it('should create client successfully with authToken', () => {
            // This should work fine
            expect(() => {
                new RealtimeClient({
                    apiUrl: 'wss://test.example.com',
                    authToken: 'valid-token'
                });
            }).not.toThrow();
        });
        
        it('should not enter reconnection loop when auth fails', async () => {
            // Create a client with token
            const client = new RealtimeClient({
                apiUrl: 'wss://test.example.com',
                authToken: 'test-token'
            });
            
            // Clear the auth token to simulate token expiry
            (client as any).authToken = null;
            
            // Spy on console.error to verify error logging
            const errorSpy = vi.spyOn(console, 'error').mockImplementation(() => {});
            
            // Enable debug for error logging
            (client as any).config.debug = true;
            
            // Try to connect without auth - should fail cleanly
            await expect(client.connect()).rejects.toThrow('Authentication token is required for connection');
            
            // Should log the error
            expect(errorSpy).toHaveBeenCalledWith(
                '[RealtimeClient] Cannot connect: No authentication token available'
            );
            
            // Verify connection state didn't change
            expect((client as any).connectionState).toBe('DISCONNECTED');
            
            errorSpy.mockRestore();
        });
        
        it('should emit error event for UI handling when auth missing during connect', async () => {
            // Create a client with token
            const client = new RealtimeClient({
                apiUrl: 'wss://test.example.com',
                authToken: 'test-token'
            });
            
            // Clear the auth token to simulate token being cleared
            (client as any).authToken = null;
            
            const errorEvents: any[] = [];
            client.on('error', (event) => {
                errorEvents.push(event);
            });
            
            // Try to connect
            try {
                await client.connect();
            } catch (e) {
                // Expected to throw
            }
            
            // Should emit error event for UI
            expect(errorEvents).toHaveLength(1);
            expect(errorEvents[0]).toEqual({
                type: 'error',
                message: 'Authentication required',
                source: 'auth'
            });
        });
        
        it('should prevent infinite reconnection attempts when auth is missing', () => {
            // Create a client with token
            const client = new RealtimeClient({
                apiUrl: 'wss://test.example.com',
                authToken: 'test-token'
            });
            
            // Mock the reconnection manager
            const mockReconnectionManager = {
                startReconnection: vi.fn(),
                stopReconnection: vi.fn(),
                on: vi.fn(),
                emit: vi.fn()
            };
            (client as any).reconnectionManager = mockReconnectionManager;
            
            // Clear the auth to simulate missing auth
            (client as any).authToken = null;
            (client as any).authManager = null;
            
            // Enable debug
            (client as any).config.debug = true;
            
            // Spy on error logging
            const errorSpy = vi.spyOn(console, 'error').mockImplementation(() => {});
            
            // Attempt reconnection
            (client as any).attemptReconnection();
            
            // Should NOT start reconnection
            expect(mockReconnectionManager.startReconnection).not.toHaveBeenCalled();
            
            // Should log error
            expect(errorSpy).toHaveBeenCalledWith(
                '[RealtimeClient] Cannot reconnect: No authentication token available'
            );
            
            errorSpy.mockRestore();
        });
    });
    
    describe('Auth validation prevents crashes', () => {
        it('should not crash when repeatedly calling connect without auth', async () => {
            // Create client with token
            const client = new RealtimeClient({
                apiUrl: 'wss://test.example.com',
                authToken: 'test-token'
            });
            
            // Clear auth
            (client as any).authToken = null;
            
            // Try connecting multiple times - should fail cleanly each time
            for (let i = 0; i < 5; i++) {
                await expect(client.connect()).rejects.toThrow('Authentication token is required for connection');
            }
            
            // Should still be in disconnected state
            expect((client as any).connectionState).toBe('DISCONNECTED');
        });
        
        it('should not accumulate event listeners on repeated auth failures', async () => {
            const client = new RealtimeClient({
                apiUrl: 'wss://test.example.com',
                authToken: 'test-token'
            });
            
            // Clear auth
            (client as any).authToken = null;
            
            // Check initial listener count
            const initialListenerCount = client.listenerCount('error');
            
            // Try connecting multiple times
            for (let i = 0; i < 10; i++) {
                try {
                    await client.connect();
                } catch (e) {
                    // Expected
                }
            }
            
            // Listener count should not have grown
            const finalListenerCount = client.listenerCount('error');
            expect(finalListenerCount).toBe(initialListenerCount);
        });
    });
});