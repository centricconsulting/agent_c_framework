import { describe, it, expect, vi, beforeEach, afterEach } from 'vitest';
import { RealtimeClient } from '../RealtimeClient';
import { WebSocketManager } from '../WebSocketManager';
import { AuthManager } from '../../auth/AuthManager';
import { ReconnectionManager } from '../ReconnectionManager';

// Mock dependencies
vi.mock('../WebSocketManager');
vi.mock('../../auth/AuthManager');
vi.mock('../ReconnectionManager');

describe('RealtimeClient - Auth Error Handling', () => {
    let client: RealtimeClient;
    let mockWsManager: any;
    let mockAuthManager: any;
    let mockReconnectionManager: any;
    
    beforeEach(() => {
        // Setup mock WebSocketManager that simulates successful connection
        mockWsManager = {
            connect: vi.fn(),
            disconnect: vi.fn(),
            isConnected: vi.fn().mockReturnValue(false),
            sendJSON: vi.fn(),
            supportsBinary: vi.fn().mockReturnValue(true),
            sendBinary: vi.fn()
        };
        
        (WebSocketManager as any).mockImplementation((options: any, callbacks: any) => {
            // Simulate successful connection after a tick
            setTimeout(() => {
                if (callbacks?.onOpen) {
                    callbacks.onOpen(new Event('open'));
                }
            }, 0);
            return mockWsManager;
        });
        
        // Setup mock AuthManager
        mockAuthManager = {
            getTokens: vi.fn(),
            getUiSessionId: vi.fn(),
            setTokens: vi.fn(),
            on: vi.fn(),
            off: vi.fn(),
            removeAllListeners: vi.fn()
        };
        
        (AuthManager as any).mockImplementation(() => mockAuthManager);
        
        // Setup mock ReconnectionManager
        mockReconnectionManager = {
            startReconnection: vi.fn().mockResolvedValue(undefined),
            stopReconnection: vi.fn(),
            reset: vi.fn(),
            on: vi.fn(),
            off: vi.fn(),
            removeAllListeners: vi.fn()
        };
        
        (ReconnectionManager as any).mockImplementation(() => mockReconnectionManager);
    });
    
    afterEach(() => {
        vi.clearAllMocks();
    });
    
    describe('connect() - Auth Validation', () => {
        it('should throw error when no auth token is available', async () => {
            // Create client without auth token
            client = new RealtimeClient({
                apiUrl: 'wss://test.example.com'
                // No authToken provided
            });
            
            // Should throw error
            await expect(client.connect()).rejects.toThrow('Authentication token is required for connection');
        });
        
        it('should emit error event when auth is missing', async () => {
            const errorSpy = vi.fn();
            
            // Create client without auth token
            client = new RealtimeClient({
                apiUrl: 'wss://test.example.com'
            });
            
            client.on('error', errorSpy);
            
            // Try to connect
            try {
                await client.connect();
            } catch (e) {
                // Expected to throw
            }
            
            // Should emit error event
            expect(errorSpy).toHaveBeenCalledWith({
                type: 'error',
                message: 'Authentication required',
                source: 'auth'
            });
        });
        
        it('should not change connection state when auth is missing', async () => {
            // Create client without auth token
            client = new RealtimeClient({
                apiUrl: 'wss://test.example.com'
            });
            
            // Initial state should be DISCONNECTED
            expect((client as any).connectionState).toBe('DISCONNECTED');
            
            // Try to connect
            try {
                await client.connect();
            } catch (e) {
                // Expected to throw
            }
            
            // State should still be DISCONNECTED (not CONNECTING)
            expect((client as any).connectionState).toBe('DISCONNECTED');
        });
        
        it('should get auth token from AuthManager if available', async () => {
            // Create client with AuthManager
            client = new RealtimeClient({
                apiUrl: 'wss://test.example.com',
                authManager: mockAuthManager
            });
            
            // Mock AuthManager returns tokens
            mockAuthManager.getTokens.mockReturnValue({
                agentCToken: 'manager-token',
                encryptedSdkToken: 'encrypted-token'
            });
            
            // Mock successful connection
            mockWsManager.connect.mockImplementation((onConnect) => {
                onConnect?.();
            });
            
            // Should successfully connect
            await client.connect();
            
            // Should have used token from AuthManager
            expect(mockAuthManager.getTokens).toHaveBeenCalled();
        });
        
        it('should prefer explicit authToken over AuthManager token', async () => {
            // Create client with both authToken and AuthManager
            client = new RealtimeClient({
                apiUrl: 'wss://test.example.com',
                authToken: 'explicit-token',
                authManager: mockAuthManager
            });
            
            // Mock AuthManager returns different tokens
            mockAuthManager.getTokens.mockReturnValue({
                agentCToken: 'manager-token',
                encryptedSdkToken: 'encrypted-token'
            });
            
            // Mock successful connection
            mockWsManager.connect.mockImplementation((onConnect) => {
                onConnect?.();
            });
            
            // Should successfully connect with explicit token
            await client.connect();
            
            // Should have the explicit token set
            expect((client as any).authToken).toBe('explicit-token');
        });
    });
    
    describe('attemptReconnection() - Auth Validation', () => {
        it('should not attempt reconnection when auth is missing', () => {
            // Create client without auth token
            client = new RealtimeClient({
                apiUrl: 'wss://test.example.com'
            });
            
            // Setup reconnection manager
            (client as any).reconnectionManager = mockReconnectionManager;
            
            // Call attemptReconnection
            (client as any).attemptReconnection();
            
            // Should NOT call startReconnection
            expect(mockReconnectionManager.startReconnection).not.toHaveBeenCalled();
        });
        
        it('should emit error event when attempting reconnection without auth', () => {
            const errorSpy = vi.fn();
            
            // Create client without auth token
            client = new RealtimeClient({
                apiUrl: 'wss://test.example.com'
            });
            
            client.on('error', errorSpy);
            
            // Setup reconnection manager
            (client as any).reconnectionManager = mockReconnectionManager;
            
            // Call attemptReconnection
            (client as any).attemptReconnection();
            
            // Should emit error event
            expect(errorSpy).toHaveBeenCalledWith({
                type: 'error',
                message: 'Authentication required for reconnection',
                source: 'auth'
            });
        });
        
        it('should check AuthManager for tokens during reconnection', () => {
            // Create client with AuthManager but no initial token
            client = new RealtimeClient({
                apiUrl: 'wss://test.example.com',
                authManager: mockAuthManager
            });
            
            // Setup reconnection manager
            (client as any).reconnectionManager = mockReconnectionManager;
            
            // Mock AuthManager has no tokens
            mockAuthManager.getTokens.mockReturnValue(null);
            
            // Call attemptReconnection
            (client as any).attemptReconnection();
            
            // Should check AuthManager
            expect(mockAuthManager.getTokens).toHaveBeenCalled();
            
            // Should NOT attempt reconnection
            expect(mockReconnectionManager.startReconnection).not.toHaveBeenCalled();
        });
        
        it('should proceed with reconnection when AuthManager provides token', () => {
            // Create client with AuthManager but no initial token
            client = new RealtimeClient({
                apiUrl: 'wss://test.example.com',
                authManager: mockAuthManager
            });
            
            // Setup reconnection manager
            (client as any).reconnectionManager = mockReconnectionManager;
            
            // Mock AuthManager returns tokens
            mockAuthManager.getTokens.mockReturnValue({
                agentCToken: 'manager-token',
                encryptedSdkToken: 'encrypted-token'
            });
            
            // Call attemptReconnection
            (client as any).attemptReconnection();
            
            // Should attempt reconnection
            expect(mockReconnectionManager.startReconnection).toHaveBeenCalled();
        });
        
        it('should stop reconnection attempts on auth error', async () => {
            // Create client with token that will be invalidated
            client = new RealtimeClient({
                apiUrl: 'wss://test.example.com',
                authToken: 'valid-token'
            });
            
            // Setup reconnection manager
            (client as any).reconnectionManager = mockReconnectionManager;
            
            // Mock reconnection that fails with auth error
            mockReconnectionManager.startReconnection.mockImplementation(async (callback) => {
                // Simulate auth failure during reconnection
                throw new Error('Authentication failed');
            });
            
            // Call attemptReconnection
            (client as any).attemptReconnection();
            
            // Wait for async operations
            await new Promise(resolve => setTimeout(resolve, 0));
            
            // Should stop reconnection on auth failure
            expect(mockReconnectionManager.stopReconnection).toHaveBeenCalled();
        });
    });
    
    describe('Debug logging', () => {
        it('should log auth errors when debug is enabled', async () => {
            const debugSpy = vi.spyOn(console, 'error').mockImplementation(() => {});
            
            // Create client with debug enabled but no auth
            client = new RealtimeClient({
                apiUrl: 'wss://test.example.com',
                debug: true
            });
            
            // Try to connect
            try {
                await client.connect();
            } catch (e) {
                // Expected to throw
            }
            
            // Should log auth error
            expect(debugSpy).toHaveBeenCalledWith(
                '[RealtimeClient] Cannot connect: No authentication token available'
            );
            
            debugSpy.mockRestore();
        });
        
        it('should log reconnection auth errors when debug is enabled', () => {
            const debugSpy = vi.spyOn(console, 'error').mockImplementation(() => {});
            
            // Create client with debug enabled but no auth
            client = new RealtimeClient({
                apiUrl: 'wss://test.example.com',
                debug: true
            });
            
            // Setup reconnection manager
            (client as any).reconnectionManager = mockReconnectionManager;
            
            // Try to reconnect
            (client as any).attemptReconnection();
            
            // Should log auth error
            expect(debugSpy).toHaveBeenCalledWith(
                '[RealtimeClient] Cannot reconnect: No authentication token available'
            );
            
            debugSpy.mockRestore();
        });
    });
});