import { describe, it, expect, beforeEach, vi, afterEach } from 'vitest';
import { RealtimeClient } from '../RealtimeClient';
import { FileUploadManager } from '../FileUploadManager';
import { WebSocketManager } from '../WebSocketManager';
import { AuthManager } from '../../auth/AuthManager';
import { ReconnectionManager } from '../ReconnectionManager';
import type { UserFileResponse } from '../FileUploadManager';

// Mock dependencies
vi.mock('../WebSocketManager');
vi.mock('../../auth/AuthManager');
vi.mock('../ReconnectionManager');
vi.mock('../FileUploadManager');

describe('RealtimeClient - FileUploadManager Integration', () => {
    let client: RealtimeClient;
    let mockWsManager: any;
    let mockAuthManager: any;
    let mockReconnectionManager: any;
    let mockFileUploadManager: any;
    
    beforeEach(() => {
        // Setup mock WebSocketManager
        mockWsManager = {
            connect: vi.fn(),
            disconnect: vi.fn(),
            isConnected: vi.fn().mockReturnValue(false),
            sendJSON: vi.fn(),
            supportsBinary: vi.fn().mockReturnValue(true),
            sendBinary: vi.fn()
        };
        
        (WebSocketManager as any).mockImplementation(() => mockWsManager);
        
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
            on: vi.fn(),
            off: vi.fn(),
            removeAllListeners: vi.fn(),
            reset: vi.fn()
        };
        
        (ReconnectionManager as any).mockImplementation(() => mockReconnectionManager);
        
        // Setup mock FileUploadManager
        mockFileUploadManager = {
            uploadFile: vi.fn(),
            uploadFiles: vi.fn(),
            setAuthToken: vi.fn(),
            setUiSessionId: vi.fn()
        };
        
        (FileUploadManager as any).mockImplementation(() => mockFileUploadManager);
    });
    
    afterEach(() => {
        vi.clearAllMocks();
    });

    describe('Phase 1: Critical Path - Token Synchronization', () => {
        it('should sync auth token to FileUploadManager on setAuthToken()', () => {
            // Create client with initial token
            client = new RealtimeClient({
                apiUrl: 'https://api.example.com',
                authToken: 'initial-token'
            });

            // Clear any initialization calls
            vi.clearAllMocks();

            // Update token
            const newToken = 'new-token-123';
            client.setAuthToken(newToken);

            // Verify FileUploadManager received the new token
            expect(mockFileUploadManager.setAuthToken).toHaveBeenCalledWith(newToken);
            expect(mockFileUploadManager.setAuthToken).toHaveBeenCalledTimes(1);
        });

        it('should initialize FileUploadManager with token from constructor', () => {
            const initialToken = 'constructor-token-456';
            
            client = new RealtimeClient({
                apiUrl: 'https://api.example.com',
                authToken: initialToken
            });

            // Verify FileUploadManager was created with the token
            const constructorCall = (FileUploadManager as any).mock.calls[0];
            expect(constructorCall[0]).toBe('https://api.example.com');
            expect(constructorCall[1]).toBe(initialToken);
            expect(constructorCall[2]).toBeUndefined(); // uiSessionId not set yet
            expect(constructorCall[3]).toEqual(expect.objectContaining({
                maxUploadSize: expect.any(Number),
                maxFilesPerMessage: expect.any(Number)
            }));
            // allowedMimeTypes can be undefined or an array
            expect(constructorCall[3].allowedMimeTypes).toSatisfy(
                (val: any) => val === undefined || Array.isArray(val)
            );
        });

        it('should handle multiple token updates correctly', () => {
            client = new RealtimeClient({
                apiUrl: 'https://api.example.com',
                authToken: 'initial-token'
            });

            vi.clearAllMocks();

            // Update token multiple times
            client.setAuthToken('token-1');
            client.setAuthToken('token-2');
            client.setAuthToken('token-3');

            // Verify all tokens were propagated
            expect(mockFileUploadManager.setAuthToken).toHaveBeenCalledTimes(3);
            expect(mockFileUploadManager.setAuthToken).toHaveBeenNthCalledWith(1, 'token-1');
            expect(mockFileUploadManager.setAuthToken).toHaveBeenNthCalledWith(2, 'token-2');
            expect(mockFileUploadManager.setAuthToken).toHaveBeenNthCalledWith(3, 'token-3');
        });
    });

    describe('Phase 1: Critical Path - Session ID Synchronization', () => {
        it('should sync UI session ID to FileUploadManager on ui_session_id_changed event', () => {
            client = new RealtimeClient({
                apiUrl: 'https://api.example.com',
                authToken: 'test-token'
            });

            vi.clearAllMocks();

            // Simulate ui_session_id_changed event
            const sessionId = 'session-abc-123';
            client.emit('ui_session_id_changed', {
                type: 'ui_session_id_changed',
                ui_session_id: sessionId
            });

            // Verify FileUploadManager received the session ID
            expect(mockFileUploadManager.setUiSessionId).toHaveBeenCalledWith(sessionId);
            expect(mockFileUploadManager.setUiSessionId).toHaveBeenCalledTimes(1);
        });

        it('should sync UI session ID to FileUploadManager on setUiSessionId()', () => {
            client = new RealtimeClient({
                apiUrl: 'https://api.example.com',
                authToken: 'test-token'
            });

            vi.clearAllMocks();

            // Call setUiSessionId directly
            const sessionId = 'manual-session-456';
            client.setUiSessionId(sessionId);

            // Verify FileUploadManager received the session ID
            expect(mockFileUploadManager.setUiSessionId).toHaveBeenCalledWith(sessionId);
            expect(mockFileUploadManager.setUiSessionId).toHaveBeenCalledTimes(1);
        });

        it('should handle null session ID on setUiSessionId()', () => {
            client = new RealtimeClient({
                apiUrl: 'https://api.example.com',
                authToken: 'test-token'
            });

            vi.clearAllMocks();

            // Call setUiSessionId with null
            client.setUiSessionId(null);

            // setUiSessionId should not call FileUploadManager when session ID is null
            expect(mockFileUploadManager.setUiSessionId).not.toHaveBeenCalled();
        });

        it('should initialize FileUploadManager with undefined session ID initially', () => {
            client = new RealtimeClient({
                apiUrl: 'https://api.example.com',
                authToken: 'test-token'
            });

            // Verify FileUploadManager was created with undefined session ID
            expect(FileUploadManager).toHaveBeenCalledWith(
                'https://api.example.com',
                'test-token',
                undefined,
                expect.any(Object)
            );
        });

        it('should handle multiple session ID changes correctly', () => {
            client = new RealtimeClient({
                apiUrl: 'https://api.example.com',
                authToken: 'test-token'
            });

            vi.clearAllMocks();

            // Emit multiple session ID changes
            client.emit('ui_session_id_changed', {
                type: 'ui_session_id_changed',
                ui_session_id: 'session-1'
            });
            client.emit('ui_session_id_changed', {
                type: 'ui_session_id_changed',
                ui_session_id: 'session-2'
            });

            // Verify all session IDs were propagated
            expect(mockFileUploadManager.setUiSessionId).toHaveBeenCalledTimes(2);
            expect(mockFileUploadManager.setUiSessionId).toHaveBeenNthCalledWith(1, 'session-1');
            expect(mockFileUploadManager.setUiSessionId).toHaveBeenNthCalledWith(2, 'session-2');
        });
    });

    describe('Phase 2: Critical Path - Upload Method Delegation', () => {
        it('should delegate uploadFile() to FileUploadManager', async () => {
            client = new RealtimeClient({
                apiUrl: 'https://api.example.com',
                authToken: 'test-token'
            });

            const mockFile = new File(['test content'], 'test.txt', { type: 'text/plain' });
            const mockResponse: UserFileResponse = {
                id: 'file-123',
                filename: 'test.txt',
                mime_type: 'text/plain',
                size: 12
            };

            mockFileUploadManager.uploadFile.mockResolvedValue(mockResponse);

            const result = await client.uploadFile(mockFile);

            expect(mockFileUploadManager.uploadFile).toHaveBeenCalledWith(mockFile, undefined);
            expect(result).toEqual(mockResponse);
        });

        it('should delegate uploadFile() with options to FileUploadManager', async () => {
            client = new RealtimeClient({
                apiUrl: 'https://api.example.com',
                authToken: 'test-token'
            });

            const mockFile = new File(['test content'], 'test.txt', { type: 'text/plain' });
            const progressCallback = vi.fn();
            const abortController = new AbortController();
            const options = {
                onProgress: progressCallback,
                signal: abortController.signal
            };

            const mockResponse: UserFileResponse = {
                id: 'file-123',
                filename: 'test.txt',
                mime_type: 'text/plain',
                size: 12
            };

            mockFileUploadManager.uploadFile.mockResolvedValue(mockResponse);

            const result = await client.uploadFile(mockFile, options);

            expect(mockFileUploadManager.uploadFile).toHaveBeenCalledWith(mockFile, options);
            expect(result).toEqual(mockResponse);
        });

        it('should delegate uploadFiles() to FileUploadManager', async () => {
            client = new RealtimeClient({
                apiUrl: 'https://api.example.com',
                authToken: 'test-token'
            });

            const mockFiles = [
                new File(['test 1'], 'test1.txt', { type: 'text/plain' }),
                new File(['test 2'], 'test2.txt', { type: 'text/plain' })
            ];

            const mockResponses: UserFileResponse[] = [
                { id: 'file-1', filename: 'test1.txt', mime_type: 'text/plain', size: 6 },
                { id: 'file-2', filename: 'test2.txt', mime_type: 'text/plain', size: 6 }
            ];

            mockFileUploadManager.uploadFiles.mockResolvedValue(mockResponses);

            const result = await client.uploadFiles(mockFiles);

            expect(mockFileUploadManager.uploadFiles).toHaveBeenCalledWith(mockFiles, undefined);
            expect(result).toEqual(mockResponses);
        });

        it('should delegate uploadFiles() with options to FileUploadManager', async () => {
            client = new RealtimeClient({
                apiUrl: 'https://api.example.com',
                authToken: 'test-token'
            });

            const mockFiles = [
                new File(['test 1'], 'test1.txt', { type: 'text/plain' })
            ];

            const progressCallback = vi.fn();
            const options = { onProgress: progressCallback };

            const mockResponses: UserFileResponse[] = [
                { id: 'file-1', filename: 'test1.txt', mime_type: 'text/plain', size: 6 }
            ];

            mockFileUploadManager.uploadFiles.mockResolvedValue(mockResponses);

            const result = await client.uploadFiles(mockFiles, options);

            expect(mockFileUploadManager.uploadFiles).toHaveBeenCalledWith(mockFiles, options);
            expect(result).toEqual(mockResponses);
        });
    });

    describe('Phase 2: Critical Path - Error Handling', () => {
        it('should throw error when uploadFile() called with no FileUploadManager', async () => {
            // Create client but mock FileUploadManager to not be initialized
            (FileUploadManager as any).mockImplementation(() => null);
            
            client = new RealtimeClient({
                apiUrl: 'https://api.example.com',
                authToken: 'test-token'
            });

            // Force fileUploadManager to null
            (client as any).fileUploadManager = null;

            const mockFile = new File(['test'], 'test.txt', { type: 'text/plain' });

            await expect(client.uploadFile(mockFile)).rejects.toThrow('FileUploadManager not initialized');
        });

        it('should throw error when uploadFiles() called with no FileUploadManager', async () => {
            // Create client but mock FileUploadManager to not be initialized
            (FileUploadManager as any).mockImplementation(() => null);
            
            client = new RealtimeClient({
                apiUrl: 'https://api.example.com',
                authToken: 'test-token'
            });

            // Force fileUploadManager to null
            (client as any).fileUploadManager = null;

            const mockFiles = [new File(['test'], 'test.txt', { type: 'text/plain' })];

            await expect(client.uploadFiles(mockFiles)).rejects.toThrow('FileUploadManager not initialized');
        });

        it('should propagate FileUploadManager errors correctly', async () => {
            client = new RealtimeClient({
                apiUrl: 'https://api.example.com',
                authToken: 'test-token'
            });

            const mockFile = new File(['test'], 'test.txt', { type: 'text/plain' });
            const uploadError = new Error('Upload failed with status 401');

            mockFileUploadManager.uploadFile.mockRejectedValue(uploadError);

            await expect(client.uploadFile(mockFile)).rejects.toThrow('Upload failed with status 401');
        });
    });

    describe('Phase 3: Edge Cases - Upload Before Session ID', () => {
        it('should handle upload attempt before ui_session_id_changed event', async () => {
            client = new RealtimeClient({
                apiUrl: 'https://api.example.com',
                authToken: 'test-token'
            });

            const mockFile = new File(['test'], 'test.txt', { type: 'text/plain' });
            
            // Simulate FileUploadManager throwing error for missing session ID
            mockFileUploadManager.uploadFile.mockRejectedValue(
                new Error('UI session ID required for file upload')
            );

            await expect(client.uploadFile(mockFile)).rejects.toThrow('UI session ID required for file upload');
        });
    });

    describe('Phase 3: Edge Cases - Configuration Propagation', () => {
        it('should pass upload configuration to FileUploadManager', () => {
            const customConfig = {
                apiUrl: 'https://api.example.com',
                authToken: 'test-token',
                maxUploadSize: 50 * 1024 * 1024, // 50MB
                allowedMimeTypes: ['image/png', 'image/jpeg'],
                maxFilesPerMessage: 5
            };

            client = new RealtimeClient(customConfig);

            expect(FileUploadManager).toHaveBeenCalledWith(
                'https://api.example.com',
                'test-token',
                undefined,
                {
                    maxUploadSize: 50 * 1024 * 1024,
                    allowedMimeTypes: ['image/png', 'image/jpeg'],
                    maxFilesPerMessage: 5
                }
            );
        });

        it('should use default upload configuration when not specified', () => {
            client = new RealtimeClient({
                apiUrl: 'https://api.example.com',
                authToken: 'test-token'
            });

            const constructorCall = (FileUploadManager as any).mock.calls[0];
            expect(constructorCall[0]).toBe('https://api.example.com');
            expect(constructorCall[1]).toBe('test-token');
            expect(constructorCall[2]).toBeUndefined(); // uiSessionId not set yet
            expect(constructorCall[3]).toEqual(expect.objectContaining({
                maxUploadSize: expect.any(Number),
                maxFilesPerMessage: expect.any(Number)
            }));
            // allowedMimeTypes can be undefined or an array
            expect(constructorCall[3].allowedMimeTypes).toSatisfy(
                (val: any) => val === undefined || Array.isArray(val)
            );
        });
    });

    describe('Phase 3: Edge Cases - Rapid Succession Operations', () => {
        it('should handle rapid token updates correctly', () => {
            client = new RealtimeClient({
                apiUrl: 'https://api.example.com',
                authToken: 'test-token'
            });

            vi.clearAllMocks();

            // Rapid fire token updates
            for (let i = 0; i < 10; i++) {
                client.setAuthToken(`token-${i}`);
            }

            // All should be propagated
            expect(mockFileUploadManager.setAuthToken).toHaveBeenCalledTimes(10);
            expect(mockFileUploadManager.setAuthToken).toHaveBeenLastCalledWith('token-9');
        });

        it('should handle rapid session ID updates correctly', () => {
            client = new RealtimeClient({
                apiUrl: 'https://api.example.com',
                authToken: 'test-token'
            });

            vi.clearAllMocks();

            // Rapid fire session ID updates
            for (let i = 0; i < 10; i++) {
                client.emit('ui_session_id_changed', {
                    type: 'ui_session_id_changed',
                    ui_session_id: `session-${i}`
                });
            }

            // All should be propagated
            expect(mockFileUploadManager.setUiSessionId).toHaveBeenCalledTimes(10);
            expect(mockFileUploadManager.setUiSessionId).toHaveBeenLastCalledWith('session-9');
        });
    });

    describe('Phase 3: Edge Cases - Combined Synchronization', () => {
        it('should handle token update followed by session ID update', () => {
            client = new RealtimeClient({
                apiUrl: 'https://api.example.com',
                authToken: 'initial-token'
            });

            vi.clearAllMocks();

            // Update token
            client.setAuthToken('new-token');
            
            // Then update session ID
            client.emit('ui_session_id_changed', {
                type: 'ui_session_id_changed',
                ui_session_id: 'new-session'
            });

            // Both should be synced
            expect(mockFileUploadManager.setAuthToken).toHaveBeenCalledWith('new-token');
            expect(mockFileUploadManager.setUiSessionId).toHaveBeenCalledWith('new-session');
        });

        it('should maintain synchronization after multiple connect/disconnect cycles', () => {
            client = new RealtimeClient({
                apiUrl: 'https://api.example.com',
                authToken: 'test-token'
            });

            vi.clearAllMocks();

            // Simulate connect/disconnect cycles with token/session updates
            client.setAuthToken('token-1');
            client.emit('ui_session_id_changed', {
                type: 'ui_session_id_changed',
                ui_session_id: 'session-1'
            });

            client.setAuthToken('token-2');
            client.emit('ui_session_id_changed', {
                type: 'ui_session_id_changed',
                ui_session_id: 'session-2'
            });

            // Verify all updates were propagated in order
            expect(mockFileUploadManager.setAuthToken).toHaveBeenCalledTimes(2);
            expect(mockFileUploadManager.setUiSessionId).toHaveBeenCalledTimes(2);
            expect(mockFileUploadManager.setAuthToken).toHaveBeenNthCalledWith(1, 'token-1');
            expect(mockFileUploadManager.setUiSessionId).toHaveBeenNthCalledWith(1, 'session-1');
            expect(mockFileUploadManager.setAuthToken).toHaveBeenNthCalledWith(2, 'token-2');
            expect(mockFileUploadManager.setUiSessionId).toHaveBeenNthCalledWith(2, 'session-2');
        });
    });

    describe('Phase 4: Send Integration - sendText() with File IDs', () => {
        beforeEach(() => {
            client = new RealtimeClient({
                apiUrl: 'https://api.example.com',
                authToken: 'test-token'
            });
            
            // Setup mock for connected state
            mockWsManager.isConnected.mockReturnValue(true);
            (client as any).wsManager = mockWsManager;
            (client as any).connectionState = 1; // CONNECTED
        });

        it('should send text without file IDs', () => {
            vi.clearAllMocks();

            client.sendText('Hello world');

            expect(mockWsManager.sendJSON).toHaveBeenCalledWith({
                type: 'text_input',
                text: 'Hello world'
            });
            expect(mockWsManager.sendJSON).toHaveBeenCalledTimes(1);
        });

        it('should send text with single file ID', () => {
            vi.clearAllMocks();

            const fileIds = ['file-123'];
            client.sendText('Check out this file', fileIds);

            expect(mockWsManager.sendJSON).toHaveBeenCalledWith({
                type: 'text_input',
                text: 'Check out this file',
                file_ids: ['file-123']
            });
        });

        it('should send text with multiple file IDs', () => {
            vi.clearAllMocks();

            const fileIds = ['file-123', 'file-456', 'file-789'];
            client.sendText('Here are multiple files', fileIds);

            expect(mockWsManager.sendJSON).toHaveBeenCalledWith({
                type: 'text_input',
                text: 'Here are multiple files',
                file_ids: ['file-123', 'file-456', 'file-789']
            });
        });

        it('should not include file_ids field when empty array provided', () => {
            vi.clearAllMocks();

            client.sendText('No files attached', []);

            expect(mockWsManager.sendJSON).toHaveBeenCalledWith({
                type: 'text_input',
                text: 'No files attached'
            });
            
            // Verify file_ids is not in the event
            const sentEvent = mockWsManager.sendJSON.mock.calls[0][0];
            expect(sentEvent).not.toHaveProperty('file_ids');
        });

        it('should include file_ids field only when non-empty', () => {
            vi.clearAllMocks();

            // Send with file IDs
            client.sendText('With files', ['file-1']);
            
            let sentEvent = mockWsManager.sendJSON.mock.calls[0][0];
            expect(sentEvent).toHaveProperty('file_ids');
            expect(sentEvent.file_ids).toEqual(['file-1']);

            // Send without file IDs
            client.sendText('Without files');
            
            sentEvent = mockWsManager.sendJSON.mock.calls[1][0];
            expect(sentEvent).not.toHaveProperty('file_ids');
        });

        it('should create valid TextInputEvent structure with files', () => {
            vi.clearAllMocks();

            const fileIds = ['file-abc', 'file-def'];
            client.sendText('Test message', fileIds);

            const sentEvent = mockWsManager.sendJSON.mock.calls[0][0];
            
            // Verify event structure
            expect(sentEvent).toEqual({
                type: 'text_input',
                text: 'Test message',
                file_ids: ['file-abc', 'file-def']
            });
            
            // Verify type correctness
            expect(sentEvent.type).toBe('text_input');
            expect(typeof sentEvent.text).toBe('string');
            expect(Array.isArray(sentEvent.file_ids)).toBe(true);
        });
    });

    describe('Phase 5: End-to-End Upload and Send Workflow', () => {
        beforeEach(() => {
            client = new RealtimeClient({
                apiUrl: 'https://api.example.com',
                authToken: 'test-token'
            });
            
            // Setup mock for connected state
            mockWsManager.isConnected.mockReturnValue(true);
            (client as any).wsManager = mockWsManager;
            (client as any).connectionState = 1; // CONNECTED
            
            // Setup session ID
            client.emit('ui_session_id_changed', {
                type: 'ui_session_id_changed',
                ui_session_id: 'session-123'
            });
        });

        it('should complete full upload-to-send workflow', async () => {
            vi.clearAllMocks();

            // Step 1: Upload file
            const mockFile = new File(['test content'], 'test.txt', { type: 'text/plain' });
            const mockResponse: UserFileResponse = {
                id: 'file-uploaded-123',
                filename: 'test.txt',
                mime_type: 'text/plain',
                size: 12
            };

            mockFileUploadManager.uploadFile.mockResolvedValue(mockResponse);
            const uploadResult = await client.uploadFile(mockFile);

            // Step 2: Send message with uploaded file ID
            client.sendText('Here is the file I uploaded', [uploadResult.id]);

            // Verify complete workflow
            expect(mockFileUploadManager.uploadFile).toHaveBeenCalledWith(mockFile, undefined);
            expect(mockWsManager.sendJSON).toHaveBeenCalledWith({
                type: 'text_input',
                text: 'Here is the file I uploaded',
                file_ids: ['file-uploaded-123']
            });
        });

        it('should handle multiple file upload and send workflow', async () => {
            vi.clearAllMocks();

            // Step 1: Upload multiple files
            const mockFiles = [
                new File(['content 1'], 'file1.txt', { type: 'text/plain' }),
                new File(['content 2'], 'file2.txt', { type: 'text/plain' })
            ];

            const mockResponses: UserFileResponse[] = [
                { id: 'file-1', filename: 'file1.txt', mime_type: 'text/plain', size: 9 },
                { id: 'file-2', filename: 'file2.txt', mime_type: 'text/plain', size: 9 }
            ];

            mockFileUploadManager.uploadFiles.mockResolvedValue(mockResponses);
            const uploadResults = await client.uploadFiles(mockFiles);

            // Step 2: Send message with all uploaded file IDs
            const fileIds = uploadResults.map(r => r.id);
            client.sendText('Multiple files attached', fileIds);

            // Verify workflow
            expect(mockFileUploadManager.uploadFiles).toHaveBeenCalledWith(mockFiles, undefined);
            expect(mockWsManager.sendJSON).toHaveBeenCalledWith({
                type: 'text_input',
                text: 'Multiple files attached',
                file_ids: ['file-1', 'file-2']
            });
        });

        it('should handle upload with progress tracking then send', async () => {
            vi.clearAllMocks();

            const progressCallback = vi.fn();
            const mockFile = new File(['test'], 'test.txt', { type: 'text/plain' });
            const mockResponse: UserFileResponse = {
                id: 'file-progress-123',
                filename: 'test.txt',
                mime_type: 'text/plain',
                size: 4
            };

            mockFileUploadManager.uploadFile.mockResolvedValue(mockResponse);
            
            const uploadResult = await client.uploadFile(mockFile, {
                onProgress: progressCallback
            });

            client.sendText('Upload complete', [uploadResult.id]);

            expect(mockFileUploadManager.uploadFile).toHaveBeenCalledWith(
                mockFile,
                { onProgress: progressCallback }
            );
            expect(mockWsManager.sendJSON).toHaveBeenCalledWith({
                type: 'text_input',
                text: 'Upload complete',
                file_ids: ['file-progress-123']
            });
        });

        it('should handle upload failure gracefully without sending', async () => {
            vi.clearAllMocks();

            const mockFile = new File(['test'], 'test.txt', { type: 'text/plain' });
            const uploadError = new Error('Upload failed: Network error');

            mockFileUploadManager.uploadFile.mockRejectedValue(uploadError);

            // Upload should fail
            await expect(client.uploadFile(mockFile)).rejects.toThrow('Upload failed: Network error');

            // No message should be sent
            expect(mockWsManager.sendJSON).not.toHaveBeenCalled();
        });

        it('should allow sending message without uploads', () => {
            vi.clearAllMocks();

            // Send message without any file uploads
            client.sendText('Just a text message');

            expect(mockFileUploadManager.uploadFile).not.toHaveBeenCalled();
            expect(mockFileUploadManager.uploadFiles).not.toHaveBeenCalled();
            expect(mockWsManager.sendJSON).toHaveBeenCalledWith({
                type: 'text_input',
                text: 'Just a text message'
            });
        });

        it('should maintain separate file ID arrays for different messages', async () => {
            vi.clearAllMocks();

            // Upload and send first message
            mockFileUploadManager.uploadFile.mockResolvedValueOnce({
                id: 'file-1',
                filename: 'file1.txt',
                mime_type: 'text/plain',
                size: 4
            });

            const file1 = await client.uploadFile(new File(['1'], 'file1.txt'));
            client.sendText('First message', [file1.id]);

            // Upload and send second message
            mockFileUploadManager.uploadFile.mockResolvedValueOnce({
                id: 'file-2',
                filename: 'file2.txt',
                mime_type: 'text/plain',
                size: 4
            });

            const file2 = await client.uploadFile(new File(['2'], 'file2.txt'));
            client.sendText('Second message', [file2.id]);

            // Verify separate messages with separate file IDs
            expect(mockWsManager.sendJSON).toHaveBeenCalledTimes(2);
            expect(mockWsManager.sendJSON).toHaveBeenNthCalledWith(1, {
                type: 'text_input',
                text: 'First message',
                file_ids: ['file-1']
            });
            expect(mockWsManager.sendJSON).toHaveBeenNthCalledWith(2, {
                type: 'text_input',
                text: 'Second message',
                file_ids: ['file-2']
            });
        });
    });
});
