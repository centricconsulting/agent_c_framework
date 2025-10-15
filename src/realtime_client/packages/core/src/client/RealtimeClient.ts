/**
 * Realtime Client - Main client class for Agent C Realtime API
 */

import { EventEmitter } from '../events/EventEmitter';
import {
    RealtimeEventMap,
    ClientEventMap,
    ChatSessionNameChangedEvent,
    AgentVoiceChangedEvent,
    TextInputEvent,
    NewChatSessionEvent,
    GetUserSessionsEvent,
    GetUserSessionsResponseEvent,
    UISessionIDChangedEvent
} from '../events';
import { EventStreamProcessor } from '../events/EventStreamProcessor';
import {
    RealtimeClientConfig,
    ConnectionState,
    ReconnectionConfig,
    AudioConfig,
    mergeConfig
} from './ClientConfig';
import { WebSocketManager } from './WebSocketManager';
import { ReconnectionManager } from './ReconnectionManager';
import { AuthManager, TokenPair } from '../auth';
import { FileUploadManager } from './FileUploadManager';
import { TurnManager, ChatSessionManager } from '../session';
import { AudioService, AudioAgentCBridge, AudioOutputService } from '../audio';
import type { AudioStatus, VoiceModel } from '../audio/types';
import { VoiceManager } from '../voice';
import type { Voice, Message, User, Agent, AgentConfiguration, Avatar, Tool, UserFileResponse, FileUploadOptions } from '../events/types/CommonTypes';
import { AvatarManager } from '../avatar';

/**
 * Main client class for connecting to Agent C Realtime API
 */
export class RealtimeClient extends EventEmitter<RealtimeEventMap> {
    private config: Required<Omit<RealtimeClientConfig, 'uiSessionId' | 'headers' | 'protocols' | 'authToken' | 'authManager'>> & Pick<RealtimeClientConfig, 'uiSessionId' | 'headers' | 'protocols' | 'authToken' | 'authManager'>;
    private wsManager: WebSocketManager | null = null;
    private reconnectionManager: ReconnectionManager;
    private connectionState: ConnectionState = ConnectionState.DISCONNECTED;
    private authToken: string | null = null;
    private uiSessionId: string | null = null;  // UI session ID for WebSocket reconnection
    private authManager: AuthManager | null = null;
    private turnManager: TurnManager | null = null;
    private voiceManager: VoiceManager | null = null;
    private sessionManager: ChatSessionManager | null = null;
    private avatarManager: AvatarManager | null = null;
    private eventStreamProcessor: EventStreamProcessor | null = null;
    private fileUploadManager: FileUploadManager | null = null;
    
    // Runtime state management for agent persistence and session recovery
    private preferredAgentKey?: string;          // Agent key to use on first connection
    private isReconnecting: boolean = false;     // Track if we're in a reconnection scenario
    private currentChatSessionId?: string;       // Current chat session ID for reconnection recovery
    
    // Audio system components
    private audioService: AudioService | null = null;
    private audioBridge: AudioAgentCBridge | null = null;
    private audioOutputService: AudioOutputService | null = null;
    private audioConfig: AudioConfig | null = null;
    
    // Configuration data from WebSocket initialization events
    private userData: User | null = null;
    private agents: Agent[] = [];
    private avatars: Avatar[] = [];
    private voices: Voice[] = [];
    private tools: Tool[] = [];
    private initializationState: Set<string> = new Set();
    private isInitialized: boolean = false;

    constructor(config: RealtimeClientConfig) {
        super();
        this.config = mergeConfig(config);
        this.authToken = config.authToken || null;
        // Backward compatibility: support legacy sessionId config parameter
        this.uiSessionId = config.uiSessionId || (config as any).sessionId || null;

        // Initialize auth manager if provided
        if (config.authManager) {
            this.authManager = config.authManager;
            // Subscribe to token refresh events
            this.authManager.on('auth:tokens-refreshed', (tokens: TokenPair) => {
                this.authToken = tokens.agentCToken;
                // If connected, reconnect with new token
                if (this.isConnected()) {
                    this.disconnect();
                    this.connect();
                }
            });
            // Get initial token if available
            const tokens = this.authManager.getTokens();
            if (tokens) {
                this.authToken = tokens.agentCToken;
            }
            
            // Note: Voice and avatar data will come from WebSocket initialization events
            this.avatarManager = new AvatarManager({ availableAvatars: [] });
        }
        
        // Initialize voice manager if not already created
        if (!this.voiceManager) {
            this.voiceManager = new VoiceManager({ enableLogging: this.config.debug });
            this.setupVoiceManagerHandlers();
        }

        // Initialize reconnection manager - cast is safe because mergeConfig ensures it's defined
        this.reconnectionManager = new ReconnectionManager(this.config.reconnection as ReconnectionConfig);
        
        // Subscribe to reconnection events
        this.reconnectionManager.on('reconnecting', ({ attempt, delay }) => {
            this.setConnectionState(ConnectionState.RECONNECTING);
            this.emit('reconnecting', { attempt, delay });
        });
        
        this.reconnectionManager.on('reconnected', () => {
            this.setConnectionState(ConnectionState.CONNECTED);
            this.emit('reconnected', undefined);
        });
        
        this.reconnectionManager.on('reconnection_failed', ({ reason }) => {
            this.setConnectionState(ConnectionState.DISCONNECTED);
            this.emit('disconnected', { code: 1006, reason });
        });
        
        // Initialize turn manager if enabled
        if (this.config.enableTurnManager) {
            this.turnManager = new TurnManager(this);
        }
        
        // Initialize session manager
        this.sessionManager = new ChatSessionManager({
            maxSessions: 50,
            persistSessions: false
        });
        
        // Initialize event stream processor with session manager
        this.eventStreamProcessor = new EventStreamProcessor(this.sessionManager);
        
        // Initialize FileUploadManager
        this.fileUploadManager = new FileUploadManager(
            this.config.apiUrl,
            this.authToken || undefined,
            this.uiSessionId || undefined,
            {
                maxUploadSize: this.config.maxUploadSize,
                allowedMimeTypes: this.config.allowedMimeTypes,
                maxFilesPerMessage: this.config.maxFilesPerMessage
            }
        );
        
        // Setup session manager handlers for fetching sessions
        this.setupSessionFetchingHandlers();
        
        // Initialize audio system if enabled
        if (this.config.enableAudio && this.config.audioConfig) {
            this.audioConfig = this.config.audioConfig;
            this.initializeAudioSystem();
        }
        
        // Setup initialization event handlers
        this.setupInitializationHandlers();
    }
    
    /**
     * Setup session fetching handlers
     */
    private setupSessionFetchingHandlers(): void {
        if (!this.sessionManager) return;
        
        // Listen for session fetch requests from ChatSessionManager
        this.sessionManager.on('request-user-sessions', ({ offset, limit }) => {
            this.fetchUserSessions(offset, limit);
        });
        
        // Handle the response from server
        this.on('get_user_sessions_response', (event: GetUserSessionsResponseEvent) => {
            if (this.sessionManager && event.sessions) {
                // Append new sessions to the index
                this.sessionManager.setSessionIndex(event.sessions, true);
                
                if (this.config.debug) {
                    console.debug('Received user sessions:', {
                        count: event.sessions.chat_sessions.length,
                        total: event.sessions.total_sessions,
                        offset: event.sessions.offset
                    });
                }
            }
        });
        
        // Note: Session data will come from WebSocket initialization events
    }
    
    /**
     * Setup session manager event handlers
     */
    private setupSessionManagerHandlers(): void {
        if (!this.sessionManager) return;
        
        // Note: chat_session_changed events from the server are processed by EventStreamProcessor
        // which calls sessionManager.setCurrentSession() directly. The event is not re-emitted
        // by RealtimeClient, so we don't need a handler here.
        
        // Note: Text delta and completion events are exclusively handled by EventStreamProcessor
        // to prevent duplicate event emission and ensure proper message building
        
        // Handle session name changes
        this.on('chat_session_name_changed', (event: ChatSessionNameChangedEvent) => {
            if (event.session_name) {
                const currentSessionId = this.sessionManager!.getCurrentSessionId();
                if (currentSessionId) {
                    this.sessionManager!.updateSessionName(currentSessionId, event.session_name);
                    
                    if (this.config.debug) {
                        // console.debug('Session name updated:', event.session_name);
                    }
                }
            }
        });
    }
    
    /**
     * Setup voice manager event handlers
     */
    private setupVoiceManagerHandlers(): void {
        if (!this.voiceManager) return;
        
        // Subscribe to voice changes from the voice manager
        this.voiceManager.on('voice-changed', (event) => {
            if (this.audioOutputService) {
                // Convert Voice to VoiceModel for AudioOutputService
                const voiceModel = this.convertVoiceToVoiceModel(event.currentVoice);
                this.audioOutputService.setVoiceModel(voiceModel);
                
                if (this.config.debug) {
                    // console.debug('Voice model updated in AudioOutputService:', voiceModel?.voice_id || 'null');
                }
            }
        });
    }
    
    /**
     * Convert Voice to VoiceModel format for AudioOutputService
     */
    private convertVoiceToVoiceModel(voice: Voice | null): VoiceModel | null {
        if (!voice) return null;
        
        return {
            voice_id: voice.voice_id,
            format: voice.output_format || 'pcm16',
            vendor: voice.vendor,
            description: voice.description,
            sampleRate: 16000 // Default sample rate for PCM16
        };
    }
    
    /**
     * Setup handlers for initialization events sent by server on connection
     * Server sends 6 events in sequence: chat_user_data, avatar_list, voice_list,
     * agent_list, tool_catalog, chat_session_changed
     */
    private setupInitializationHandlers(): void {
        // Handle user data event
        this.on('chat_user_data', (event: any) => {
            if (event.user) {
                this.userData = event.user;
                this.initializationState.add('chat_user_data');
                
                // Update auth manager's user state if available
                if (this.authManager) {
                    (this.authManager as any).updateState({
                        user: event.user
                    });
                }
                
                if (this.config.debug) {
                    console.debug('Received user data:', event.user.user_name);
                }
                
                this.checkInitializationComplete();
            }
        });
        
        // Handle UI session ID changed event
        this.on('ui_session_id_changed', (event: UISessionIDChangedEvent) => {
            console.debug('[RealtimeClient] ui_session_id_changed handler called with:', event);
            if (event.ui_session_id) {
                // Update stored UI session ID for reconnection
                this.uiSessionId = event.ui_session_id;
                
                // Sync with file upload manager
                if (this.fileUploadManager) {
                    this.fileUploadManager.setUiSessionId(event.ui_session_id);
                }
                
                // Update auth manager's UI session ID if available
                if (this.authManager) {
                    (this.authManager as any).updateState({
                        uiSessionId: event.ui_session_id
                    });
                }
                
                if (this.config.debug) {
                    console.debug('UI Session ID updated:', event.ui_session_id);
                }
            }
        });
        
        // Listen to ChatSessionManager for chat session changes to update currentChatSessionId
        if (this.sessionManager) {
            this.sessionManager.on('chat-session-changed', ({ currentChatSession }) => {
            console.debug('[RealtimeClient] chat-session-changed handler called with:', currentChatSession);
            if (currentChatSession && currentChatSession.session_id) {
                // Update stored chat session ID for reconnection
                this.currentChatSessionId = currentChatSession.session_id;
                
                if (this.config.debug) {
                    console.debug('Chat Session ID updated for reconnection:', this.currentChatSessionId);
                }
            }
            });
        }
        
        // Handle avatar list event
        this.on('avatar_list', (event: any) => {
            if (event.avatars) {
                this.avatars = event.avatars;
                this.initializationState.add('avatar_list');
                
                // Update avatar manager
                if (this.avatarManager) {
                    this.avatarManager.updateAvailableAvatars(event.avatars);
                }
                
                if (this.config.debug) {
                    console.debug('Received avatars:', event.avatars.length);
                }
                
                this.checkInitializationComplete();
            }
        });
        
        // Handle voice list event
        this.on('voice_list', (event: any) => {
            if (event.voices) {
                this.voices = event.voices;
                this.initializationState.add('voice_list');
                
                // Update voice manager
                if (this.voiceManager) {
                    this.voiceManager.setAvailableVoices(event.voices);
                }
                
                if (this.config.debug) {
                    console.debug('Received voices:', event.voices.length);
                }
                
                this.checkInitializationComplete();
            }
        });
        
        // Handle agent list event
        this.on('agent_list', (event: any) => {
            if (event.agents) {
                this.agents = event.agents;
                this.initializationState.add('agent_list');
                
                if (this.config.debug) {
                    console.debug('Received agents:', event.agents.length);
                }
                
                this.checkInitializationComplete();
            }
        });
        
        // Handle tool catalog event
        this.on('tool_catalog', (event: any) => {
            if (event.tools) {
                this.tools = event.tools;
                this.initializationState.add('tool_catalog');
                
                if (this.config.debug) {
                    console.debug('Received tools:', event.tools.length);
                }
                
                this.checkInitializationComplete();
            }
        });
        
        // The 6th event (chat_session_changed) is already handled in setupSessionManagerHandlers
        // We track it for initialization completion
        this.on('chat_session_changed', () => {
            if (!this.initializationState.has('chat_session_changed')) {
                this.initializationState.add('chat_session_changed');
                this.checkInitializationComplete();
            }
        });
        
        // Handle agent configuration changes
        this.on('agent_configuration_changed', (event: any) => {
            if (event.agent_config && this.sessionManager) {
                // Update the current session's agent configuration
                const currentSession = this.sessionManager.getCurrentSession();
                if (currentSession) {
                    currentSession.agent_config = event.agent_config;
                    
                    if (this.config.debug) {
                        console.debug('Agent configuration updated:', event.agent_config.key);
                    }
                }
            }
        });
    }
    
    /**
     * Check if all initialization events have been received
     */
    private checkInitializationComplete(): void {
        const requiredEvents = [
            'chat_user_data',
            'avatar_list', 
            'voice_list',
            'agent_list',
            'tool_catalog',
            'chat_session_changed'
        ];
        
        const allReceived = requiredEvents.every(event => this.initializationState.has(event));
        
        if (allReceived && !this.isInitialized) {
            this.isInitialized = true;
            
            if (this.config.debug) {
                console.debug('Initialization complete - all 6 events received');
            }
            
            // Handle session recovery or agent preference after initialization
            this.handlePostInitializationRecovery();
            
            // Emit a custom event to signal initialization is complete
            this.emit('initialized' as any, undefined);
        }
    }
    
    /**
     * Handle session recovery or agent preference after initialization completes
     * This ensures we don't conflict with server initialization events
     */
    private handlePostInitializationRecovery(): void {
        // Defer execution to ensure WebSocket is ready for new messages
        setTimeout(() => {
            if (this.isReconnecting) {
                // We're reconnecting - session recovery is handled by URL parameters
                // The server will automatically resume the session if chat_session_id was provided
                // If not, check if we should create a new session with preferred agent
                if (!this.currentChatSessionId && this.preferredAgentKey) {
                    if (this.config.debug) {
                        console.debug('Reconnection: no session to resume, creating new session with preferred agent', this.preferredAgentKey);
                    }
                    this.newChatSession(this.preferredAgentKey);
                }
                this.isReconnecting = false;  // Clear reconnection flag
            } else if (this.preferredAgentKey && !this.currentChatSessionId) {
                // Initial connection with preferred agent (not reconnection)
                if (this.config.debug) {
                    console.debug('Initial connection: creating session with preferred agent', this.preferredAgentKey);
                }
                this.newChatSession(this.preferredAgentKey);
            }
        }, 0);
    }
    
    /**
     * Initialize the audio system components
     */
    private initializeAudioSystem(): void {
        if (!this.audioConfig) return;
        
        try {
            // Get singleton instances
            this.audioService = AudioService.getInstance();
            this.audioBridge = AudioAgentCBridge.getInstance();
            this.audioOutputService = AudioOutputService.getInstance();
            
            // Configure audio bridge with this client
            this.audioBridge.setClient(this);
            
            // The turn manager integration is handled by the AudioAgentCBridge
            // which checks turn state before streaming if respectTurnState is enabled
            
            // Set initial volume
            if (this.audioConfig.initialVolume !== undefined) {
                this.audioOutputService.setVolume(this.audioConfig.initialVolume);
            }
            
            // Subscribe to audio output events for playback
            this.on('audio:output', (audioData: ArrayBuffer) => {
                if (this.audioConfig?.enableOutput && this.audioOutputService) {
                    this.audioOutputService.playAudioChunk(audioData);
                }
            });
            
            // Subscribe to voice model changes from server
            this.on('agent_voice_changed', (event: AgentVoiceChangedEvent) => {
                if (this.voiceManager && event.voice) {
                    // Let voice manager handle the server voice change
                    this.voiceManager.handleServerVoiceChange(event.voice.voice_id);
                }
            });
            
            if (this.config.debug) {
                console.warn('Audio system initialized');
            }
        } catch (error) {
            console.error('Failed to initialize audio system:', error);
            this.emit('error', {
                type: 'error',
                message: 'Failed to initialize audio system',
                source: 'audio_init'
            });
        }
    }

    /**
     * Connect to the Realtime API
     */
    async connect(): Promise<void> {
        if (this.connectionState === ConnectionState.CONNECTED) {
            return;
        }

        if (this.connectionState === ConnectionState.CONNECTING) {
            throw new Error('Already connecting');
        }

        // Get auth token from manager if available
        if (this.authManager && !this.authToken) {
            const tokens = this.authManager.getTokens();
            if (tokens) {
                this.authToken = tokens.agentCToken;
            }
        }

        // Get UI session ID from auth manager if available
        if (this.authManager && !this.uiSessionId) {
            const uiSessionId = this.authManager.getUiSessionId();
            if (uiSessionId) {
                this.uiSessionId = uiSessionId;
            }
        }

        // Check for auth token before connecting
        if (!this.authToken) {
            // Try to get token from authManager if available
            if (this.authManager) {
                const tokens = this.authManager.getTokens();
                if (tokens) {
                    this.authToken = tokens.agentCToken;
                }
            }
            
            // If still no token, fail with error event and exception
            if (!this.authToken) {
                const errorEvent = {
                    type: 'error' as const,
                    message: 'Authentication required',
                    source: 'auth' as const
                };
                this.emit('error', errorEvent);
                
                const error = new Error('Authentication token is required for connection');
                if (this.config.debug) {
                    console.error('[RealtimeClient] Cannot connect: No authentication token available');
                }
                throw error;
            }
        }

        this.setConnectionState(ConnectionState.CONNECTING);

        return new Promise((resolve, reject) => {
            try {
                const wsUrl = this.buildWebSocketUrl();

                this.wsManager = new WebSocketManager(
                    {
                        url: wsUrl,
                        protocols: this.config.protocols,
                        // Ensure binaryType is always 'arraybuffer' for proper audio handling
                        binaryType: 'arraybuffer',
                        pingInterval: this.config.pingInterval,
                        pongTimeout: this.config.pongTimeout
                    },
                    {
                        onOpen: () => {
                            this.setConnectionState(ConnectionState.CONNECTED);
                            this.reconnectionManager.reset();
                            this.emit('connected', undefined);
                            
                            // Setup session manager handlers when connected
                            this.setupSessionManagerHandlers();
                            
                            // Reconnect audio bridge if audio is enabled
                            if (this.audioBridge && this.audioConfig?.enableInput) {
                                this.audioBridge.setClient(this);
                            }
                            
                            // Session recovery will happen after initialization completes
                            // This ensures we don't conflict with server initialization events
                            
                            resolve();
                        },
                        onClose: (event) => {
                            this.handleDisconnection(event.code, event.reason);
                        },
                        onError: () => {
                            this.emit('error', {
                                type: 'error',
                                message: 'WebSocket error occurred',
                                source: 'websocket'
                            });
                            if (this.connectionState === ConnectionState.CONNECTING) {
                                reject(new Error('Failed to connect'));
                            }
                        },
                        onMessage: (data) => {
                            this.handleMessage(data);
                        }
                    }
                );

                this.wsManager.connect();

                // Set connection timeout
                if (this.config.connectionTimeout) {
                    setTimeout(() => {
                        if (this.connectionState === ConnectionState.CONNECTING) {
                            this.wsManager?.disconnect(1000, 'Connection timeout');
                            reject(new Error('Connection timeout'));
                        }
                    }, this.config.connectionTimeout);
                }
            } catch (error) {
                this.setConnectionState(ConnectionState.DISCONNECTED);
                reject(error);
            }
        });
    }

    /**
     * Disconnect from the Realtime API
     */
    disconnect(): void {
        this.reconnectionManager.stopReconnection();
        this.isReconnecting = false;  // Clear reconnection flag if disconnect is called
        
        // Stop audio streaming if active
        if (this.audioBridge?.getStatus().isStreaming) {
            this.audioBridge.stopStreaming();
        }
        
        // Stop audio recording if active
        if (this.audioService?.getStatus().isRecording) {
            this.audioService.stopRecording();
        }
        
        // Clear audio output buffers
        if (this.audioOutputService) {
            this.audioOutputService.clearBuffers();
        }
        
        // Clear session manager accumulator
        if (this.sessionManager) {
            this.sessionManager.resetAccumulator();
        }
        
        // Reset event stream processor
        if (this.eventStreamProcessor) {
            this.eventStreamProcessor.reset();
        }
        
        // Reset initialization state for next connection
        this.initializationState.clear();
        this.isInitialized = false;

        if (this.wsManager) {
            this.wsManager.disconnect(1000, 'Client disconnect');
            this.wsManager = null;
        }

        this.setConnectionState(ConnectionState.DISCONNECTED);
    }

    /**
     * Send a raw event to the server
     * 
     * @public
     * @param event - The event object to send to the server. Must be a valid ClientEventMap event type.
     * @throws {Error} If not connected to the server
     * 
     * @example
     * ```typescript
     * // Send a custom event to the server
     * client.sendEvent({ type: 'custom_event', data: 'example' });
     * ```
     */
    public sendEvent<K extends keyof ClientEventMap>(event: ClientEventMap[K]): void {
        if (!this.wsManager || !this.wsManager.isConnected()) {
            throw new Error('Not connected to server');
        }

        this.wsManager.sendJSON(event);
    }

    /**
     * Send binary audio data to the server
     * @deprecated Use sendBinaryFrame instead
     */
    sendAudio(audioData: ArrayBuffer | ArrayBufferView): void {
        this.sendBinaryFrame(audioData);
    }

    /**
     * Send raw binary frame to the server
     * This sends audio data directly as binary, NOT wrapped in JSON events
     * @param data Raw binary audio data (PCM16 format)
     */
    sendBinaryFrame(data: ArrayBuffer | ArrayBufferView): void {
        if (!this.wsManager || !this.wsManager.isConnected()) {
            throw new Error('Not connected to server');
        }

        if (!this.wsManager.supportsBinary()) {
            throw new Error('WebSocket connection does not support binary frames');
        }

        // Send the raw binary data directly over the WebSocket
        // This is NOT wrapped in an AudioInputDelta event - it's raw PCM16 audio
        this.wsManager.sendBinary(data);
        
        if (this.config.debug) {
            // const byteLength = data instanceof ArrayBuffer ? data.byteLength : data.byteLength;
            // console.debug('Sent binary audio frame:', byteLength, 'bytes');
        }
    }

    // Client command methods

    /**
     * Get list of available agents
     */
    getAgents(): void {
        this.sendEvent({ type: 'get_agents' });
    }

    /**
     * Set the active agent
     */
    setAgent(agentKey: string): void {
        this.sendEvent({ type: 'set_agent', agent_key: agentKey });
    }

    /**
     * Get list of available avatars
     */
    getAvatars(): void {
        this.sendEvent({ type: 'get_avatars' });
    }

    /**
     * Set the avatar for the current session.
     * This creates a new HeyGen avatar session with the specified avatar.
     * 
     * @param avatarId - The ID of the avatar to set
     * @param quality - Optional quality setting (default: "auto")
     * @param videoEncoding - Optional video encoding (default: "H265")
     */
    setAvatar(avatarId: string, quality?: string, videoEncoding?: string): void {
        // Send event to Agent C to create avatar session
        this.sendEvent({
            type: 'set_avatar',
            avatar_id: avatarId,
            quality: quality,
            video_encoding: videoEncoding
        });
        
        if (this.config.debug) {
            console.debug('Setting avatar:', { avatarId, quality, videoEncoding });
        }
    }
    
    /**
     * Set avatar session after HeyGen STREAM_READY event.
     * This notifies Agent C that an avatar session is active.
     * 
     * @param accessToken - HeyGen access token for the session
     * @param avatarSessionId - HeyGen avatar session ID
     */
    setAvatarSession(accessToken: string, avatarSessionId: string): void {
        // Update avatar manager state
        if (this.avatarManager) {
            this.avatarManager.setAvatarSession(avatarSessionId, avatarSessionId);
        }
        
        // Send event to Agent C with correct field names from API spec
        this.sendEvent({
            type: 'set_avatar_session',
            access_token: accessToken,
            avatar_session_id: avatarSessionId
        });
        
        // Voice manager will automatically switch to avatar voice when server responds
        if (this.config.debug) {
            // console.debug('Avatar session set:', { accessToken, avatarSessionId });
        }
    }
    
    /**
     * Clear avatar session when HeyGen session ends
     */
    clearAvatarSession(): void {
        if (this.avatarManager) {
            const sessionId = this.avatarManager.getSessionId();
            this.avatarManager.clearAvatarSession();
            
            // Optionally send clear event to server
            // The server should detect this from HeyGen webhooks but we can be explicit
            if (sessionId) {
                this.sendEvent({
                    type: 'clear_avatar_session',
                    session_id: sessionId
                });
            }
        }
    }

    /**
     * Set agent voice
     */
    setAgentVoice(voiceId: string): void {
        // Update voice manager locally
        if (this.voiceManager) {
            this.voiceManager.setCurrentVoice(voiceId, 'client');
        }
        // Send to server
        this.sendEvent({ type: 'set_agent_voice', voice_id: voiceId });
    }

    /**
     * Send text input to the agent
     * 
     * Note: The user message will be added to the session when the server
     * responds with a UserMessageEvent, maintaining the server as the
     * single source of truth for message history.
     */
    sendText(text: string, fileIds?: string[]): void {
        const event: TextInputEvent = { type: 'text_input', text };
        if (fileIds && fileIds.length > 0) {
            event.file_ids = fileIds;
        }
        this.sendEvent(event);
    }

    /**
     * Create a new chat session
     */
    newChatSession(agentKey?: string): void {
        // Clear the stored session ID since we're explicitly starting a new session
        this.currentChatSessionId = undefined;
        
        // Reset accumulator when creating new session
        if (this.sessionManager) {
            this.sessionManager.resetAccumulator();
        }
        
        const event: NewChatSessionEvent = { type: 'new_chat_session' };
        if (agentKey) {
            event.agent_key = agentKey;
        }
        this.sendEvent(event);
    }

    /**
     * Resume an existing chat session
     */
    resumeChatSession(sessionId: string): void {
        // Reset accumulator when switching sessions
        if (this.sessionManager) {
            this.sessionManager.resetAccumulator();
        }
        
        this.sendEvent({ type: 'resume_chat_session', session_id: sessionId });
    }

    /**
     * Set chat session name
     */
    setChatSessionName(sessionName: string): void {
        // Update local session name immediately for better UX
        if (this.sessionManager) {
            const currentSessionId = this.sessionManager.getCurrentSessionId();
            if (currentSessionId) {
                this.sessionManager.updateSessionName(currentSessionId, sessionName);
            }
        }
        
        this.sendEvent({ type: 'set_chat_session_name', session_name: sessionName });
    }

    /**
     * Set session metadata
     */
    setSessionMetadata(meta: Record<string, unknown>): void {
        this.sendEvent({ type: 'set_session_metadata', meta });
    }

    /**
     * Set session messages
     */
    setSessionMessages(messages: Message[]): void {
        this.sendEvent({ type: 'set_session_messages', messages });
    }
    
    /**
     * Fetch paginated list of user sessions
     * @param offset Starting offset for pagination (default 0)
     * @param limit Number of sessions to fetch (default 50)
     */
    fetchUserSessions(offset: number = 0, limit: number = 50): void {
        const event: GetUserSessionsEvent = {
            type: 'get_user_sessions',
            offset,
            limit
        };
        this.sendEvent(event);
        
        if (this.config.debug) {
            console.debug('Requesting user sessions:', { offset, limit });
        }
    }
    
    /**
     * Request list of available voices from server
     * Server responds with voice_list event
     */
    getVoices(): void {
        this.sendEvent({ type: 'get_voices' });
    }
    
    /**
     * Request tool catalog from server
     * Server responds with tool_catalog event
     */
    getToolCatalog(): void {
        this.sendEvent({ type: 'get_tool_catalog' });
    }
    
    /**
     * Send ping to server for connection health check
     * Server responds with pong event
     */
    ping(): void {
        this.sendEvent({ type: 'ping' });
    }
    
    /**
     * Cancel the current agent response
     * Sends a request to the server to cancel the ongoing agent response.
     * Server will respond with a 'cancelled' event to confirm.
     */
    cancelResponse(): void {
        const timestamp = Date.now();
        const event = { type: 'client_wants_cancel' as const };
        
        if (this.config.debug) {
            console.debug(`[${timestamp}] cancelResponse() called`);
            console.debug(`[${timestamp}] WebSocket readyState: ${this.wsManager?.getReadyState?.() ?? 'unknown'}`);
            
            // Check if there's buffered data that might delay the cancel
            const bufferedAmount = this.wsManager?.getBufferedAmount?.();
            if (bufferedAmount && bufferedAmount > 0) {
                console.warn(`[${timestamp}] WARNING: WebSocket has ${bufferedAmount} bytes buffered before cancel - this may delay the cancel event!`);
            }
        }
        
        try {
            this.sendEvent(event);
            
            if (this.config.debug) {
                console.debug(`[${timestamp}] client_wants_cancel event sent to WebSocket`);
                
                // Check buffered amount after send
                const bufferedAfter = this.wsManager?.getBufferedAmount?.();
                if (bufferedAfter && bufferedAfter > 0) {
                    console.warn(`[${timestamp}] Post-send: WebSocket still has ${bufferedAfter} bytes buffered`);
                }
            }
        } catch (error) {
            if (this.config.debug) {
                console.error(`[${timestamp}] Failed to send cancel event:`, error);
            }
            throw error;
        }
    }

    /**
     * Upload a file for use in chat messages
     * @param file - File object to upload
     * @param options - Upload options (progress callback, abort signal)
     * @returns Promise resolving to file metadata (id, filename, mime_type, size)
     * @throws Error if FileUploadManager not initialized, authentication missing, or upload fails
     */
    async uploadFile(file: File, options?: FileUploadOptions): Promise<UserFileResponse> {
        if (!this.fileUploadManager) {
            throw new Error('FileUploadManager not initialized');
        }
        return this.fileUploadManager.uploadFile(file, options);
    }

    /**
     * Upload multiple files for use in chat messages
     * @param files - Array of File objects to upload
     * @param options - Upload options (progress callback, abort signal)
     * @returns Promise resolving to array of file metadata
     * @throws Error if FileUploadManager not initialized, authentication missing, or upload fails
     */
    async uploadFiles(files: File[], options?: FileUploadOptions): Promise<UserFileResponse[]> {
        if (!this.fileUploadManager) {
            throw new Error('FileUploadManager not initialized');
        }
        return this.fileUploadManager.uploadFiles(files, options);
    }

    // Getters

    /**
     * Get current connection state
     */
    getConnectionState(): ConnectionState {
        return this.connectionState;
    }

    /**
     * Check if connected
     */
    isConnected(): boolean {
        return this.connectionState === ConnectionState.CONNECTED;
    }

    /**
     * Get the auth manager instance
     */
    getAuthManager(): AuthManager | null {
        return this.authManager;
    }

    /**
     * Get the turn manager instance
     */
    getTurnManager(): TurnManager | null {
        return this.turnManager;
    }
    
    /**
     * Get the voice manager instance
     */
    getVoiceManager(): VoiceManager | null {
        return this.voiceManager;
    }
    
    /**
     * Get the session manager instance
     */
    getSessionManager(): ChatSessionManager | null {
        return this.sessionManager;
    }
    
    /**
     * Get the avatar manager instance
     */
    getAvatarManager(): AvatarManager | null {
        return this.avatarManager;
    }
    
    /**
     * Get available avatars from initialization events or avatar manager
     */
    getAvailableAvatars() {
        return this.avatars.length > 0 ? this.avatars : (this.avatarManager?.getAvailableAvatars() || []);
    }
    
    /**
     * Get HeyGen access token for avatar session creation
     */
    getHeyGenAccessToken(): string | null {
        if (this.authManager) {
            const tokens = this.authManager.getTokens();
            return tokens?.heygenToken || null;
        }
        return null;
    }
    
    /**
     * Get current user data from initialization events
     */
    getUserData(): User | null {
        return this.userData;
    }
    
    /**
     * Get available agents from initialization events
     */
    getAgentsList(): Agent[] {
        return this.agents;
    }
    
    /**
     * Get current agent configuration from the active session
     * Returns null if no session is active
     */
    getCurrentAgentConfig(): AgentConfiguration | null {
        if (this.sessionManager) {
            const currentSession = this.sessionManager.getCurrentSession();
            return currentSession?.agent_config || null;
        }
        return null;
    }
    
    /**
     * Get available voices from initialization events
     */
    getVoicesList(): Voice[] {
        return this.voices;
    }
    
    /**
     * Get available tools from initialization events
     */
    getTools(): Tool[] {
        return this.tools;
    }
    
    /**
     * Check if client has completed initialization
     * All 6 initialization events must be received
     */
    isFullyInitialized(): boolean {
        return this.isInitialized;
    }
    
    /**
     * Wait for initialization to complete
     * Returns a promise that resolves when all 6 initialization events are received
     */
    waitForInitialization(): Promise<void> {
        return new Promise((resolve) => {
            if (this.isInitialized) {
                resolve();
            } else {
                this.once('initialized' as any, () => {
                    resolve();
                });
            }
        });
    }
    
    // Audio control methods
    
    /**
     * Start audio recording from microphone
     */
    async startAudioRecording(): Promise<void> {
        if (!this.audioService) {
            throw new Error('Audio system not initialized');
        }
        if (!this.audioConfig?.enableInput) {
            throw new Error('Audio input is disabled');
        }
        await this.audioService.startRecording();
    }
    
    /**
     * Stop audio recording
     */
    stopAudioRecording(): void {
        if (!this.audioService) {
            throw new Error('Audio system not initialized');
        }
        this.audioService.stopRecording();
    }
    
    /**
     * Start streaming audio to server
     */
    startAudioStreaming(): void {
        if (!this.audioBridge) {
            throw new Error('Audio system not initialized');
        }
        if (!this.isConnected()) {
            throw new Error('Not connected to server');
        }
        this.audioBridge.startStreaming();
    }
    
    /**
     * Stop streaming audio to server
     */
    stopAudioStreaming(): void {
        if (!this.audioBridge) {
            throw new Error('Audio system not initialized');
        }
        this.audioBridge.stopStreaming();
    }
    
    /**
     * Set audio playback volume
     * @param volume Volume level (0-1)
     */
    setAudioVolume(volume: number): void {
        if (!this.audioOutputService) {
            throw new Error('Audio output not initialized');
        }
        if (volume < 0 || volume > 1) {
            throw new Error('Volume must be between 0 and 1');
        }
        this.audioOutputService.setVolume(volume);
    }
    
    /**
     * Get combined audio system status
     */
    getAudioStatus(): AudioStatus {
        const inputStatus = this.audioService?.getStatus();
        const bridgeStatus = this.audioBridge?.getStatus();
        const outputStatus = this.audioOutputService?.getStatus();
        
        return {
            // Input status
            isRecording: inputStatus?.isRecording || false,
            isStreaming: bridgeStatus?.isStreaming || false,
            isProcessing: inputStatus?.state === 'recording' || false,
            hasPermission: inputStatus?.state !== 'permission-denied' && inputStatus?.state !== 'idle',
            currentLevel: inputStatus?.audioLevel || 0,
            averageLevel: inputStatus?.audioLevel || 0,  // Using current level as average for now
            
            // Output status
            isPlaying: outputStatus?.isPlaying || false,
            bufferSize: outputStatus?.queueLength || 0,
            volume: outputStatus?.volume || 1,
            
            // System status
            isAudioEnabled: !!this.audioService,
            isInputEnabled: !!this.audioConfig?.enableInput,
            isOutputEnabled: !!this.audioConfig?.enableOutput
        };
    }

    /**
     * Set the auth manager instance
     */
    setAuthManager(authManager: AuthManager): void {
        // Unsubscribe from previous manager
        if (this.authManager) {
            this.authManager.removeAllListeners();
        }

        this.authManager = authManager;
        
        // Subscribe to token refresh events
        this.authManager.on('auth:tokens-refreshed', (tokens: TokenPair) => {
            this.authToken = tokens.agentCToken;
            // If connected, reconnect with new token
            if (this.isConnected()) {
                this.disconnect();
                this.connect();
            }
        });
        
        // Get initial token if available
        const tokens = this.authManager.getTokens();
        if (tokens) {
            this.authToken = tokens.agentCToken;
        }
        
        // Note: Voice and avatar data will come from WebSocket initialization events
    }

    /**
     * Update authentication token
     */
    setAuthToken(token: string): void {
        this.authToken = token;
        
        // Sync with file upload manager
        if (this.fileUploadManager) {
            this.fileUploadManager.setAuthToken(token);
        }
        
        // If connected, we need to reconnect with new token
        if (this.isConnected()) {
            this.disconnect();
            this.connect();
        }
    }

    /**
     * Update session ID
     */
    setUiSessionId(uiSessionId: string | null): void {
        this.uiSessionId = uiSessionId;
        
        // Sync with file upload manager
        if (this.fileUploadManager && uiSessionId) {
            this.fileUploadManager.setUiSessionId(uiSessionId);
        }
        
        // If connected, we need to reconnect with new UI session ID
        if (this.isConnected()) {
            this.disconnect();
            this.connect();
        }
    }

    /**
     * Set the preferred agent key to use when creating new chat sessions.
     * This agent key will be used automatically when:
     * - Creating a new chat session on initial connection
     * - Creating a new chat session after reconnection (if no session to resume)
     * 
     * @param agentKey - The agent key to use for new sessions, or undefined to clear
     */
    public setPreferredAgentKey(agentKey: string | undefined): void {
        this.preferredAgentKey = agentKey;
    }

    // Private methods

    /**
     * Build WebSocket URL with authentication
     */
    private buildWebSocketUrl(): string {
        if (!this.authToken) {
            // CRITICAL FIX: More descriptive error for missing auth
            // This helps identify auth issues vs connection issues
            throw new Error('Authentication token is required to build WebSocket URL. Ensure auth is initialized before connecting.');
        }

        // Parse the base URL to handle protocol conversion
        const baseUrl = this.config.apiUrl;
        const parsedUrl = new URL(baseUrl);
        
        // Convert HTTP(S) to WS(S) protocol
        let protocol = parsedUrl.protocol;
        if (protocol === 'http:') {
            protocol = 'ws:';
        } else if (protocol === 'https:') {
            protocol = 'wss:';
        } else if (protocol !== 'ws:' && protocol !== 'wss:') {
            throw new Error(`Invalid protocol in apiUrl: ${protocol}. Must be http, https, ws, or wss`);
        }
        
        // Build the WebSocket URL with the correct endpoint path
        // Always use /api/rt/ws as the path, ignoring any path in the base URL
        const url = new URL(`${protocol}//${parsedUrl.host}/api/rt/ws`);
        url.searchParams.set('token', this.authToken);
        
        // CRITICAL: Always send ui_session_id if available (not session_id)
        // This identifies the client instance for reconnection
        // Try to get ui_session_id from multiple sources: direct property or AuthManager
        let effectiveUiSessionId = this.uiSessionId;
        if (!effectiveUiSessionId && this.authManager) {
            effectiveUiSessionId = this.authManager.getUiSessionId() || null;
        }
        
        console.debug('[RealtimeClient] Building WebSocket URL - uiSessionId:', effectiveUiSessionId);
        if (effectiveUiSessionId) {
            url.searchParams.set('ui_session_id', effectiveUiSessionId);
            console.debug('[RealtimeClient] Added ui_session_id to URL:', effectiveUiSessionId);
        } else {
            console.debug('[RealtimeClient] No ui_session_id available for WebSocket connection');
        }

        // Add connection context parameters
        // Priority: reconnection parameters take precedence over first connection parameters
        if (this.isReconnecting && this.currentChatSessionId) {
            // Add chat_session_id parameter for reconnection recovery
            url.searchParams.append('chat_session_id', this.currentChatSessionId);
            
            if (this.config.debug) {
                console.debug(`[WebSocket] Adding chat_session_id for reconnection: ${this.currentChatSessionId}`);
            }
        } else if (this.preferredAgentKey) {
            // Add agent_key parameter for first connection with agent preference
            url.searchParams.append('agent_key', this.preferredAgentKey);
            
            if (this.config.debug) {
                console.debug(`[WebSocket] Adding agent_key for first connection: ${this.preferredAgentKey}`);
            }
        }
        // NEVER send both parameters simultaneously - enforced by else-if structure

        return url.toString();
    }

    /**
     * Handle incoming messages from WebSocket
     * Supports both JSON events and binary audio frames
     */
    private handleMessage(data: string | ArrayBuffer): void {
        if (typeof data === 'string') {
            // JSON message - parse as event
            try {
                const event = JSON.parse(data);
                // Log EVERY event received
                console.debug('[RealtimeClient] Raw event received:', event.type); //, event);
                
                if (event && typeof event.type === 'string') {
                    // Debug logging for ui_session_id_changed
                    if (event.type === 'ui_session_id_changed') {
                        console.debug('[RealtimeClient] Received ui_session_id_changed event:', event);
                    }
                    
                    // Debug logging for turn and cancel events
                    if (this.config.debug && 
                        (event.type === 'user_turn_start' || 
                         event.type === 'user_turn_end' || 
                         event.type === 'agent_turn_start' ||
                         event.type === 'agent_turn_end' ||
                         event.type === 'cancelled')) {
                        console.debug(`[${Date.now()}] Received ${event.type} event from server`);
                    }
                    
                    // Handle ping events with pong response
                    if (event.type === 'ping') {
                        // Send pong directly via WebSocket, not through sendEvent
                        this.wsManager?.sendJSON({ type: 'pong' });
                        if (this.config.debug) {
                            // console.debug('Received ping, sent pong');
                        }
                        return;
                    }
                    
                    // Handle pong events (no action needed, just acknowledge)
                    if (event.type === 'pong') {
                        if (this.config.debug) {
                            // console.debug('Received pong');
                        }
                        return;
                    }
                    
                    // Process events through EventStreamProcessor if applicable
                    let processedByEventStream = false;
                    if (this.eventStreamProcessor) {
                        const eventTypesToProcess = [
                            'interaction',
                            'text_delta',
                            'thought_delta',
                            'completion',
                            'tool_select_delta',
                            'tool_call',
                            'render_media',
                            'system_message',
                            'error',
                            'history_delta',
                            'chat_session_changed',
                            'user_message',
                            'anthropic_user_message',
                            'subsession_started',
                            'subsession_ended',
                            'cancelled'
                        ];
                        
                        if (eventTypesToProcess.includes(event.type)) {
                            this.eventStreamProcessor.processEvent(event);
                            processedByEventStream = true;
                        }
                    }
                    
                    // Only emit events that weren't processed by EventStreamProcessor
                    // EventStreamProcessor handles its own event emission for streaming events
                    if (!processedByEventStream) {
                        // Debug ui_session_id_changed specifically
                        if (event.type === 'ui_session_id_changed') {
                            console.debug('[RealtimeClient] About to emit ui_session_id_changed:', event);
                        }
                        this.emit(event.type as keyof RealtimeEventMap, event as RealtimeEventMap[keyof RealtimeEventMap]);
                    }
                    
                    if (this.config.debug) {
                        // console.debug('Received event:', event.type, event);
                    }
                } else {
                    console.warn('Invalid event structure:', event);
                }
            } catch (error) {
                console.error('Failed to parse JSON message:', error);
                this.emit('error', {
                    type: 'error',
                    message: 'Failed to parse server message',
                    source: 'message_parser'
                });
            }
        } else if (data instanceof ArrayBuffer) {
            // Binary message - this is raw audio data from the server
            // Emit as 'audio:output' for consistency with the audio system
            this.emit('audio:output', data);
            
            // Also emit the legacy event for backward compatibility
            this.emit('binary_audio', data);
            
            if (this.config.debug) {
                // console.debug('Received binary audio frame:', data.byteLength, 'bytes');
            }
        } else {
            console.warn('Unknown message type:', typeof data);
        }
    }

    /**
     * Handle disconnection and trigger reconnection if needed
     */
    private handleDisconnection(code: number, reason: string): void {
        const wasConnected = this.connectionState === ConnectionState.CONNECTED;
        this.setConnectionState(ConnectionState.DISCONNECTED);
        
        this.emit('disconnected', { code, reason });

        // Attempt reconnection if configured and wasn't a clean disconnect
        if (wasConnected && this.config.autoReconnect && code !== 1000) {
            this.attemptReconnection();
        }
    }

    /**
     * Attempt to reconnect to the server
     */
    private attemptReconnection(): void {
        // Check if we have auth before attempting reconnection
        if (!this.authToken && (!this.authManager || !this.authManager.getTokens()?.agentCToken)) {
            if (this.config.debug) {
                console.error('[RealtimeClient] Cannot reconnect: No authentication token available');
            }
            // Emit error event for UI handling
            const errorEvent = {
                type: 'error' as const,
                message: 'Authentication required for reconnection',
                source: 'auth' as const
            };
            this.emit('error', errorEvent);
            // Don't attempt reconnection without auth
            return;
        }
        
        this.isReconnecting = true;  // Mark that we're reconnecting
        // The current chat session ID is already stored and will be used in buildWebSocketUrl
        
        this.reconnectionManager.startReconnection(async () => {
            await this.connect();
        }).catch((error) => {
            this.isReconnecting = false;  // Clear reconnection flag on failure
            if (this.config.debug) {
                console.error('Reconnection failed:', error);
            }
            
            // If it's an auth error, don't keep retrying
            if (error.message && error.message.includes('Authentication')) {
                this.reconnectionManager.stopReconnection();
            }
        });
    }

    /**
     * Update connection state and emit event
     */
    private setConnectionState(state: ConnectionState): void {
        if (this.connectionState !== state) {
            this.connectionState = state;
            
            if (this.config.debug) {
                // console.debug('Connection state changed:', ConnectionState[state]);
            }
        }
    }

    /**
     * Clean up resources
     */
    destroy(): void {
        // Stop and clean up audio first
        if (this.audioBridge?.getStatus().isStreaming) {
            this.audioBridge.stopStreaming();
        }
        
        if (this.audioService?.getStatus().isRecording) {
            this.audioService.stopRecording();
        }
        
        // Note: We don't destroy the singletons, just clean up our references
        this.audioService = null;
        
        if (this.audioBridge) {
            this.audioBridge.setClient(null);
            this.audioBridge = null;
        }
        
        if (this.audioOutputService) {
            this.audioOutputService.clearBuffers();
            this.audioOutputService = null;
        }
        
        this.disconnect();
        
        if (this.authManager) {
            this.authManager.removeAllListeners();
        }
        
        if (this.turnManager) {
            this.turnManager.destroy();
            this.turnManager = null;
        }
        
        if (this.voiceManager) {
            this.voiceManager.removeAllListeners();
            this.voiceManager = null;
        }
        
        if (this.sessionManager) {
            this.sessionManager.destroy();
            this.sessionManager = null;
        }
        
        if (this.avatarManager) {
            this.avatarManager.dispose();
            this.avatarManager = null;
        }
        
        if (this.eventStreamProcessor) {
            this.eventStreamProcessor.destroy();
            this.eventStreamProcessor = null;
        }
        
        this.removeAllListeners();
    }
}