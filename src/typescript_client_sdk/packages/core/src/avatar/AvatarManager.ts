import { EventEmitter } from '../events/EventEmitter';
import { Avatar } from '../events/types/CommonTypes';
import { Logger } from '../utils/logger';

/**
 * Events emitted by the AvatarManager
 */
export interface AvatarManagerEvents {
  'avatar-session-started': { sessionId: string; avatarId: string };
  'avatar-session-ended': { sessionId: string };
  'avatar-state-changed': { active: boolean };
}

/**
 * Configuration for AvatarManager
 */
export interface AvatarManagerConfig {
  availableAvatars?: Avatar[];
}

/**
 * Minimal Avatar Manager for Agent C integration.
 * 
 * This manager does NOT handle HeyGen SDK directly. The client application is
 * responsible for:
 * 1. Establishing a session with HeyGen using their SDK
 * 2. Waiting for HeyGen's STREAM_READY event
 * 3. Calling setAvatarSession() to notify Agent C
 * 
 * The AvatarManager only tracks avatar state and communicates with Agent C.
 */
export class AvatarManager extends EventEmitter<AvatarManagerEvents> {
  private currentSessionId: string | null = null;
  private currentAvatarId: string | null = null;
  private availableAvatars: Avatar[] = [];
  private isActive = false;

  constructor(config: AvatarManagerConfig = {}) {
    super();
    if (config.availableAvatars) {
      this.availableAvatars = config.availableAvatars;
      Logger.debug('[AvatarManager] Initialized with avatars:', this.availableAvatars.map(a => a.avatar_id));
    }
  }

  /**
   * Set the avatar session after HeyGen STREAM_READY event.
   * This notifies Agent C that an avatar session is active.
   * 
   * @param sessionId - HeyGen session ID from StreamingEvents.STREAM_READY
   * @param avatarId - Avatar ID that was used to create the session
   */
  public setAvatarSession(sessionId: string, avatarId: string): void {
    if (this.currentSessionId === sessionId) {
      Logger.debug('[AvatarManager] Avatar session already active:', sessionId);
      return;
    }

    // Clear any existing session
    if (this.currentSessionId) {
      this.clearAvatarSession();
    }

    this.currentSessionId = sessionId;
    this.currentAvatarId = avatarId;
    this.isActive = true;

    Logger.info('[AvatarManager] Avatar session started:', { sessionId, avatarId });
    
    this.emit('avatar-session-started', { sessionId, avatarId });
    this.emit('avatar-state-changed', { active: true });
  }

  /**
   * Clear the current avatar session.
   * Call this when the HeyGen session ends or on error.
   */
  public clearAvatarSession(): void {
    if (!this.currentSessionId) {
      Logger.debug('[AvatarManager] No avatar session to clear');
      return;
    }

    const sessionId = this.currentSessionId;
    this.currentSessionId = null;
    this.currentAvatarId = null;
    this.isActive = false;

    Logger.info('[AvatarManager] Avatar session ended:', sessionId);
    
    this.emit('avatar-session-ended', { sessionId });
    this.emit('avatar-state-changed', { active: false });
  }

  /**
   * Get the current avatar session ID
   */
  public getSessionId(): string | null {
    return this.currentSessionId;
  }

  /**
   * Get the current avatar ID
   */
  public getAvatarId(): string | null {
    return this.currentAvatarId;
  }

  /**
   * Check if an avatar session is currently active
   */
  public isAvatarActive(): boolean {
    return this.isActive;
  }

  /**
   * Get list of available avatars from auth response
   */
  public getAvailableAvatars(): Avatar[] {
    return [...this.availableAvatars];
  }

  /**
   * Update available avatars (called when auth response is received)
   */
  public updateAvailableAvatars(avatars: Avatar[]): void {
    this.availableAvatars = avatars;
    Logger.debug('[AvatarManager] Updated available avatars:', avatars.map(a => a.avatar_id));
  }

  /**
   * Find an avatar by ID
   */
  public findAvatar(avatarId: string): Avatar | undefined {
    return this.availableAvatars.find(a => a.avatar_id === avatarId);
  }
  
  /**
   * Check if a specific avatar is available
   */
  public isAvatarAvailable(avatarId: string): boolean {
    return this.availableAvatars.some(a => a.avatar_id === avatarId);
  }

  /**
   * Clean up resources
   */
  public dispose(): void {
    this.clearAvatarSession();
    this.removeAllListeners();
  }
}