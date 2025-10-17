/**
 * Shared types for input components
 */

export interface AgentTool {
  id: string
  name: string
  icon?: string
  category?: string
}

export interface Agent {
  id: string
  name: string
  description: string
  avatar?: string
  tools: AgentTool[]
  capabilities?: string[]
  available?: boolean
}

export type OutputMode = 'text' | 'voice' | 'avatar'

export interface OutputOption {
  id: string
  name: string
  type: OutputMode  // Updated to use OutputMode type
  available: boolean
  metadata?: {
    voiceId?: string
    avatarId?: string
    previewUrl?: string
  }
}