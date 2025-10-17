/**
 * Client module exports
 */

export { RealtimeClient } from './RealtimeClient';
export { WebSocketManager } from './WebSocketManager';
export { ReconnectionManager } from './ReconnectionManager';
export { FileUploadManager } from './FileUploadManager';
export {
    RealtimeClientConfig,
    ConnectionState,
    ReconnectionConfig,
    defaultConfig,
    defaultReconnectionConfig,
    mergeConfig
} from './ClientConfig';

// Re-export commonly used types for convenience
export type {
    WebSocketManagerOptions,
    WebSocketManagerCallbacks
} from './WebSocketManager';