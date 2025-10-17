"use client"

/**
 * @fileoverview OutputSelector Component for Agent C Realtime SDK
 * @module @agentc/realtime-ui/components/controls
 */

/**
 * OutputSelector Component
 * 
 * A searchable, portal-based selector component that allows users to select the output mode
 * for agent responses. Supports three modes:
 * - Text Only: Agent responds with text messages only (no audio)
 * - Voice: Agent responds with synthesized speech using available voices
 * - Avatar: Agent responds with HeyGen streaming avatar (implementation pending)
 * 
 * @component
 * @example
 * ```tsx
 * // Basic usage
 * <OutputSelector />
 * 
 * // With custom configuration
 * <OutputSelector
 *   className="w-[250px]"
 *   showIcon={true}
 *   showErrorAlerts={false}
 *   ariaLabel="Choose how the agent responds"
 * />
 * 
 * // Disabled state
 * <OutputSelector disabled={true} />
 * ```
 * 
 * @remarks
 * This component requires the AgentCProvider context to be available.
 * It integrates with the SDK's voice management system and responds to
 * connection state changes automatically.
 * 
 * The component is fully accessible with ARIA labels, keyboard navigation,
 * and screen reader announcements. It follows CenSuite design patterns
 * and is WCAG 2.1 AA compliant.
 * 
 * @see {@link https://docs.agentc.ai/ui/output-selector} for detailed documentation
 * @since 0.1.0
 */

import * as React from "react"
import * as ReactDOM from "react-dom"
import { ChevronDown, Type, Mic, User, Loader2, AlertCircle, Check, Search, X } from "lucide-react"
import { 
  useAgentCData, 
  useInitializationStatus, 
  useVoiceModel, 
  useRealtimeClientSafe,
  useConnection 
} from "@agentc/realtime-react"
import { filterRegularVoices, type Voice } from "@agentc/realtime-core"
import { cn } from "../../lib/utils"
import { Button } from "../ui/button"
import { Input } from "../ui/input"
import { ScrollArea } from "../ui/scroll-area"
import { Card } from "../ui/card"
import { Badge } from "../ui/badge"
import { Alert, AlertDescription } from "../ui/alert"

/**
 * Props for the OutputSelector component
 * 
 * @interface OutputSelectorProps
 * @extends {React.HTMLAttributes<HTMLButtonElement>}
 */
export interface OutputSelectorProps {
  /**
   * Additional CSS classes to apply to the root element
   * @default undefined
   */
  className?: string
  
  /**
   * Whether the selector is disabled
   * @default false
   */
  disabled?: boolean
  
  /**
   * Whether to show the mode icon in the button
   * @default true
   */
  showIcon?: boolean
  
  /**
   * Custom aria-label for the selector button
   * Improves accessibility by providing context
   * @default "Output mode selector. Current mode: [current mode]"
   * @example "Select how Virtual Joe responds to you"
   */
  ariaLabel?: string
  
  /**
   * Whether to show error alerts inline below the selector
   * When false, errors are still logged to console
   * @default true
   */
  showErrorAlerts?: boolean
}

/**
 * Format voice description for display
 */
function formatVoiceDescription(voice: Voice): string {
  if (voice.description) {
    return voice.description
  }
  return `${voice.vendor} ${voice.voice_id}`
}

/**
 * Get a concise description for avatars, focusing on pose name
 */
function formatAvatarDescription(avatar: any): string {
  // Show pose_name as primary identifier
  return avatar.pose_name || avatar.avatar_id
}

/**
 * Custom Popover component using React Portal for OutputSelector
 * Renders outside DOM hierarchy to avoid parent container constraints
 */
const OutputSelectorPopover: React.FC<{
  open: boolean
  onClose: () => void
  currentMode: { type: string; label: string; icon: any }
  voices: Voice[]
  avatars: any[]
  onSelectTextOnly: () => void
  onSelectVoice: (voiceId: string) => void
  onSelectAvatar: (avatarId: string) => void
  isChanging: boolean
  triggerRef: React.RefObject<HTMLButtonElement>
  currentVoiceId?: string
  currentAvatarId?: string
}> = ({ 
  open, 
  onClose, 
  currentMode,
  voices, 
  avatars, 
  onSelectTextOnly, 
  onSelectVoice, 
  onSelectAvatar, 
  isChanging, 
  triggerRef,
  currentVoiceId,
  currentAvatarId
}) => {
  const [searchValue, setSearchValue] = React.useState('')
  const [activeTab, setActiveTab] = React.useState<'modes' | 'voices' | 'avatars'>('modes')
  const [position, setPosition] = React.useState({ top: 0, left: 0, width: 0, maxHeight: 400 })
  const popoverRef = React.useRef<HTMLDivElement>(null)
  const searchInputRef = React.useRef<HTMLInputElement>(null)
  
  // Filter voices based on search
  const filteredVoices = React.useMemo(() => {
    if (!searchValue || activeTab !== 'voices') return voices
    
    const search = searchValue.toLowerCase()
    return voices.filter(voice => {
      const vendor = (voice.vendor || '').toLowerCase()
      const voiceId = (voice.voice_id || '').toLowerCase()
      const description = (voice.description || '').toLowerCase()
      
      return vendor.includes(search) || 
             voiceId.includes(search) || 
             description.includes(search)
    })
  }, [voices, searchValue, activeTab])
  
  // Filter avatars based on search
  const filteredAvatars = React.useMemo(() => {
    if (!searchValue || activeTab !== 'avatars') return avatars
    
    const search = searchValue.toLowerCase()
    return avatars.filter(avatar => {
      const avatarId = (avatar.avatar_id || '').toLowerCase()
      const poseName = (avatar.pose_name || '').toLowerCase()
      
      return avatarId.includes(search) || poseName.includes(search)
    })
  }, [avatars, searchValue, activeTab])
  
  // Calculate position and focus search input when opened
  React.useEffect(() => {
    if (open && triggerRef.current) {
      const rect = triggerRef.current.getBoundingClientRect()
      const viewportHeight = window.innerHeight
      const viewportWidth = window.innerWidth
      const spaceBelow = viewportHeight - rect.bottom - 20
      const spaceAbove = rect.top - 20
      
      // Calculate max height that fits in viewport
      const preferredHeight = 450
      const minHeight = 250
      
      let top: number
      let maxHeight: number
      
      // Prefer positioning below if there's enough space
      if (spaceBelow >= minHeight) {
        top = rect.bottom + 8
        maxHeight = Math.min(preferredHeight, spaceBelow)
      } else if (spaceAbove >= minHeight) {
        maxHeight = Math.min(preferredHeight, spaceAbove)
        top = rect.top - maxHeight - 8
      } else {
        if (spaceBelow > spaceAbove) {
          top = rect.bottom + 8
          maxHeight = spaceBelow
        } else {
          maxHeight = spaceAbove
          top = rect.top - maxHeight - 8
        }
      }
      
      // Ensure left position keeps popover in viewport
      let left = rect.left
      const popoverWidth = 420
      if (left + popoverWidth > viewportWidth - 20) {
        left = viewportWidth - popoverWidth - 20
      }
      if (left < 20) {
        left = 20
      }
      
      setPosition({
        top,
        left,
        width: rect.width,
        maxHeight
      })
      
      // Focus search input if on a searchable tab
      if (activeTab !== 'modes') {
        setTimeout(() => searchInputRef.current?.focus(), 100)
      }
    }
  }, [open, triggerRef, activeTab])
  
  // Handle click outside
  React.useEffect(() => {
    if (!open) return
    
    const handleClickOutside = (event: MouseEvent) => {
      if (
        popoverRef.current && 
        !popoverRef.current.contains(event.target as Node) &&
        triggerRef.current &&
        !triggerRef.current.contains(event.target as Node)
      ) {
        onClose()
      }
    }
    
    document.addEventListener('mousedown', handleClickOutside)
    return () => document.removeEventListener('mousedown', handleClickOutside)
  }, [open, onClose, triggerRef])
  
  // Handle escape key
  React.useEffect(() => {
    if (!open) return
    
    const handleEscape = (event: KeyboardEvent) => {
      if (event.key === 'Escape') {
        onClose()
        triggerRef.current?.focus()
      }
    }
    
    document.addEventListener('keydown', handleEscape)
    return () => document.removeEventListener('keydown', handleEscape)
  }, [open, onClose, triggerRef])
  
  // Reset search when changing tabs
  React.useEffect(() => {
    setSearchValue('')
  }, [activeTab])
  
  if (!open) return null
  
  // Use portal to render outside DOM hierarchy
  return ReactDOM.createPortal(
    <>
      {/* Full-screen backdrop */}
      <div 
        className="fixed inset-0 z-[9998] bg-black/20"
        onClick={onClose}
        aria-hidden="true"
      />
      
      {/* Popover Content */}
      <div
        ref={popoverRef}
        className={cn(
          "fixed z-[9999] w-[420px] max-w-[calc(100vw-2rem)]",
          "bg-background border border-border rounded-lg shadow-xl",
          "animate-in fade-in-0 zoom-in-95",
          "flex flex-col overflow-hidden"
        )}
        style={{
          top: `${position.top}px`,
          left: `${position.left}px`,
          minWidth: `${Math.max(position.width, 250)}px`,
          maxHeight: `${position.maxHeight}px`,
          height: 'auto'
        }}
        role="listbox"
        aria-label="Select output mode"
      >
        {/* Tab Header */}
        <div className="flex-shrink-0 border-b border-border">
          <div className="flex">
            <button
              className={cn(
                "flex-1 px-4 py-2 text-sm font-medium transition-colors",
                "hover:bg-accent/50 focus:outline-none focus:bg-accent",
                activeTab === 'modes' && "bg-accent border-b-2 border-primary"
              )}
              onClick={() => setActiveTab('modes')}
              aria-selected={activeTab === 'modes'}
              role="tab"
            >
              Modes
            </button>
            <button
              className={cn(
                "flex-1 px-4 py-2 text-sm font-medium transition-colors",
                "hover:bg-accent/50 focus:outline-none focus:bg-accent",
                activeTab === 'voices' && "bg-accent border-b-2 border-primary"
              )}
              onClick={() => setActiveTab('voices')}
              aria-selected={activeTab === 'voices'}
              role="tab"
            >
              Voices ({voices.length})
            </button>
            <button
              className={cn(
                "flex-1 px-4 py-2 text-sm font-medium transition-colors",
                "hover:bg-accent/50 focus:outline-none focus:bg-accent",
                activeTab === 'avatars' && "bg-accent border-b-2 border-primary",
                avatars.length === 0 && "opacity-50 cursor-not-allowed"
              )}
              onClick={() => avatars.length > 0 && setActiveTab('avatars')}
              disabled={avatars.length === 0}
              aria-selected={activeTab === 'avatars'}
              role="tab"
            >
              Avatars ({avatars.length})
            </button>
          </div>
        </div>
        
        {/* Search Bar (for voices and avatars) */}
        {activeTab !== 'modes' && (
          <div className="flex-shrink-0 p-3 border-b border-border">
            <div className="flex items-center gap-2">
              <Search className="h-4 w-4 text-muted-foreground" />
              <Input
                ref={searchInputRef}
                type="text"
                placeholder={activeTab === 'voices' ? "Search voices..." : "Search avatars..."}
                value={searchValue}
                onChange={(e) => setSearchValue(e.target.value)}
                className="flex-1 h-8 border-0 p-0 focus-visible:ring-0"
                aria-label={activeTab === 'voices' ? "Search voices" : "Search avatars"}
              />
              {searchValue && (
                <Button
                  variant="ghost"
                  size="icon"
                  className="h-6 w-6"
                  onClick={() => setSearchValue('')}
                  aria-label="Clear search"
                >
                  <X className="h-3 w-3" />
                </Button>
              )}
            </div>
          </div>
        )}
        
        {/* Content Area */}
        <ScrollArea className="flex-1 min-h-0 overflow-auto">
          <div className="p-2">
            {/* Modes Tab */}
            {activeTab === 'modes' && (
              <div className="space-y-1">
                {/* Text Only Mode */}
                <button
                  onClick={() => {
                    onSelectTextOnly()
                    onClose()
                  }}
                  disabled={isChanging}
                  className={cn(
                    "w-full text-left p-3 rounded-md",
                    "transition-colors duration-150",
                    "hover:bg-accent/50 focus:bg-accent focus:outline-none",
                    "disabled:opacity-50 disabled:cursor-not-allowed",
                    currentMode.type === 'text' && "bg-accent"
                  )}
                  role="option"
                  aria-selected={currentMode.type === 'text'}
                >
                  <div className="flex items-center justify-between">
                    <div className="flex items-center gap-3">
                      <Type className="h-5 w-5 text-muted-foreground" />
                      <div>
                        <div className="font-medium">Text Only</div>
                        <div className="text-xs text-muted-foreground">Responses without audio</div>
                      </div>
                    </div>
                    {currentMode.type === 'text' && (
                      <Check className="h-4 w-4 text-primary" />
                    )}
                  </div>
                </button>
                
                {/* Voice Mode (Quick Select) */}
                <button
                  onClick={() => setActiveTab('voices')}
                  className={cn(
                    "w-full text-left p-3 rounded-md",
                    "transition-colors duration-150",
                    "hover:bg-accent/50 focus:bg-accent focus:outline-none",
                    currentMode.type === 'voice' && "bg-accent"
                  )}
                  role="option"
                >
                  <div className="flex items-center justify-between">
                    <div className="flex items-center gap-3">
                      <Mic className="h-5 w-5 text-muted-foreground" />
                      <div>
                        <div className="font-medium">Voice</div>
                        <div className="text-xs text-muted-foreground">
                          {currentMode.type === 'voice' ? currentMode.label : 'Select a voice'}
                        </div>
                      </div>
                    </div>
                    <ChevronDown className="h-4 w-4 text-muted-foreground -rotate-90" />
                  </div>
                </button>
                
                {/* Avatar Mode (Quick Select) */}
                <button
                  onClick={() => avatars.length > 0 ? setActiveTab('avatars') : null}
                  disabled={avatars.length === 0}
                  className={cn(
                    "w-full text-left p-3 rounded-md",
                    "transition-colors duration-150",
                    "hover:bg-accent/50 focus:bg-accent focus:outline-none",
                    "disabled:opacity-50 disabled:cursor-not-allowed",
                    currentMode.type === 'avatar' && "bg-accent"
                  )}
                  role="option"
                >
                  <div className="flex items-center justify-between">
                    <div className="flex items-center gap-3">
                      <User className="h-5 w-5 text-muted-foreground" />
                      <div>
                        <div className="font-medium">Avatar</div>
                        <div className="text-xs text-muted-foreground">
                          {avatars.length === 0 ? 'No avatars available' :
                           currentMode.type === 'avatar' ? currentMode.label : 'Select an avatar'}
                        </div>
                      </div>
                    </div>
                    {avatars.length > 0 && (
                      <ChevronDown className="h-4 w-4 text-muted-foreground -rotate-90" />
                    )}
                  </div>
                </button>
              </div>
            )}
            
            {/* Voices Tab */}
            {activeTab === 'voices' && (
              <>
                {filteredVoices.length === 0 ? (
                  <div className="text-center py-8 text-muted-foreground text-sm">
                    {searchValue ? "No voices found matching your search." : "No voices available."}
                  </div>
                ) : (
                  filteredVoices.map((voice) => {
                    const isSelected = voice.voice_id === currentVoiceId && currentMode.type === 'voice'
                    
                    return (
                      <button
                        key={voice.voice_id}
                        onClick={() => {
                          onSelectVoice(voice.voice_id)
                          setSearchValue('')
                          onClose()
                        }}
                        disabled={isChanging}
                        className={cn(
                          "w-full text-left p-3 rounded-md mb-1",
                          "transition-colors duration-150",
                          "hover:bg-accent/50 focus:bg-accent focus:outline-none",
                          "disabled:opacity-50 disabled:cursor-not-allowed",
                          isSelected && "bg-accent"
                        )}
                        role="option"
                        aria-selected={isSelected}
                      >
                        <div className="flex items-start justify-between">
                          <div className="flex items-center gap-2 flex-1">
                            <Mic className="h-4 w-4 text-muted-foreground mt-0.5 flex-shrink-0" />
                            <div className="min-w-0">
                              <div className="font-medium text-sm">
                                {voice.vendor} - {voice.voice_id}
                              </div>
                              {voice.description && (
                                <div className="text-xs text-muted-foreground mt-0.5 line-clamp-2">
                                  {voice.description}
                                </div>
                              )}
                            </div>
                          </div>
                          {isSelected && (
                            <Check className="h-4 w-4 text-primary flex-shrink-0 ml-2" />
                          )}
                        </div>
                      </button>
                    )
                  })
                )}
              </>
            )}
            
            {/* Avatars Tab */}
            {activeTab === 'avatars' && (
              <>
                {filteredAvatars.length === 0 ? (
                  <div className="text-center py-8 text-muted-foreground text-sm">
                    {searchValue ? "No avatars found matching your search." : "No avatars available."}
                  </div>
                ) : (
                  <>
                    {filteredAvatars.map((avatar) => {
                      const isSelected = avatar.avatar_id === currentAvatarId && currentMode.type === 'avatar'
                      
                      return (
                        <button
                          key={avatar.avatar_id}
                          onClick={() => {
                            onSelectAvatar(avatar.avatar_id)
                            setSearchValue('')
                            onClose()
                          }}
                          disabled={isChanging}
                          className={cn(
                            "w-full text-left p-3 rounded-md mb-1",
                            "transition-colors duration-150",
                            "hover:bg-accent/50 focus:bg-accent focus:outline-none",
                            "disabled:opacity-50 disabled:cursor-not-allowed",
                            isSelected && "bg-accent"
                          )}
                          role="option"
                          aria-selected={isSelected}
                        >
                          <div className="flex items-start justify-between">
                            <div className="flex items-center gap-2 flex-1">
                              <User className="h-4 w-4 text-muted-foreground mt-0.5 flex-shrink-0" />
                              <div className="min-w-0">
                                <div className="font-medium text-sm">
                                  {formatAvatarDescription(avatar)}
                                </div>
                                {avatar.avatar_id !== avatar.pose_name && (
                                  <div className="text-xs text-muted-foreground mt-0.5">
                                    ID: {avatar.avatar_id}
                                  </div>
                                )}
                              </div>
                            </div>
                            {isSelected && (
                              <Check className="h-4 w-4 text-primary flex-shrink-0 ml-2" />
                            )}
                          </div>
                        </button>
                      )
                    })}
                    
                    {/* Coming Soon Note */}
                    <div className="mt-3 p-3 bg-muted/50 rounded-md">
                      <p className="text-xs text-muted-foreground text-center">
                        Avatar support coming soon
                      </p>
                    </div>
                  </>
                )}
              </>
            )}
          </div>
        </ScrollArea>
        
        {/* Footer with counts */}
        <div className="flex-shrink-0 p-2 border-t border-border">
          <p className="text-xs text-muted-foreground text-center">
            {activeTab === 'modes' && 'Select an output mode'}
            {activeTab === 'voices' && (
              <>
                {filteredVoices.length} of {voices.length} voices
                {searchValue && " matching search"}
              </>
            )}
            {activeTab === 'avatars' && (
              <>
                {filteredAvatars.length} of {avatars.length} avatars
                {searchValue && " matching search"}
              </>
            )}
          </p>
        </div>
      </div>
    </>,
    document.body // Render at document.body level
  )
}

/**
 * OutputSelector component for selecting agent output modes
 * 
 * @component
 * @param {OutputSelectorProps} props - Component props
 * @param {React.Ref<HTMLButtonElement>} ref - Forward ref to the trigger button
 * @returns {JSX.Element} The rendered OutputSelector component
 * 
 * @example
 * ```tsx
 * const ref = useRef<HTMLButtonElement>(null);
 * 
 * <OutputSelector
 *   ref={ref}
 *   className="my-selector"
 *   showIcon={true}
 *   disabled={false}
 * />
 * ```
 */
export const OutputSelector = React.forwardRef<HTMLButtonElement, OutputSelectorProps>(
  ({ className, disabled = false, showIcon = true, ariaLabel, showErrorAlerts = true }, ref) => {
    const client = useRealtimeClientSafe()
    const { data, isInitialized, isLoading } = useAgentCData()
    const { isInitialized: connectionInitialized } = useInitializationStatus()
    const { currentVoice, isLoading: voiceLoading, error: voiceError } = useVoiceModel()
    const { isConnected } = useConnection()
    
    const [isChanging, setIsChanging] = React.useState(false)
    const [error, setError] = React.useState<string | null>(null)
    const [open, setOpen] = React.useState(false)
    const buttonRef = React.useRef<HTMLButtonElement>(null)
    
    // Merge refs
    React.useImperativeHandle(ref, () => buttonRef.current!, [])
    
    // Live region for screen reader announcements
    const [announcement, setAnnouncement] = React.useState<string>('')

    // Derive current mode from SDK state
    const currentMode = React.useMemo(() => {
      if (!currentVoice || currentVoice.voice_id === 'none') {
        return {
          type: 'text',
          label: 'Text Only',
          icon: Type
        }
      }
      
      if (currentVoice.voice_id === 'avatar') {
        // Avatar mode
        const avatarId = client?.getAvatarManager?.()?.getAvatarId?.()
        const avatar = data.avatars.find(a => a.avatar_id === avatarId)
        return {
          type: 'avatar',
          label: avatar ? formatAvatarDescription(avatar) : 'Avatar Mode',
          icon: User
        }
      }
      
      // Voice mode - use description or fallback to vendor + voice_id
      return {
        type: 'voice',
        label: formatVoiceDescription(currentVoice),
        icon: Mic
      }
    }, [currentVoice, data.avatars, client])
    
    // Get current avatar ID for selection state
    const currentAvatarId = React.useMemo(() => {
      if (currentMode.type !== 'avatar') return undefined
      const avatarId = client?.getAvatarManager?.()?.getAvatarId?.()
      return avatarId || undefined
    }, [currentMode.type, client])

    // Filter only public avatars (remove overly restrictive status check)
    const activeAvatars = React.useMemo(() => {
      if (!data.avatars || !Array.isArray(data.avatars)) return []
      return data.avatars.filter(
        avatar => avatar.is_public
      )
    }, [data.avatars])
    
    // Filter regular voices (exclude system voices like 'none' and 'avatar')
    const filteredVoices = React.useMemo(() => {
      if (!data.voices || !Array.isArray(data.voices)) return []
      return filterRegularVoices(data.voices)
    }, [data.voices])

    /**
     * Handle Text Only mode selection
     * Sends 'none' as voice_id to disable audio output
     */
    const handleTextOnly = React.useCallback(async () => {
      if (!client || isChanging) return
      
      setIsChanging(true)
      setError(null)
      setAnnouncement('Switching to text-only mode...')
      
      try {
        await client.setAgentVoice('none')
        // Small delay to ensure loading message is visible to screen readers
        await new Promise(resolve => setTimeout(resolve, 10))
        setAnnouncement('Text-only mode activated')
      } catch (error) {
        const errorMessage = 'Failed to set text mode'
        console.error(errorMessage, error)
        setError(errorMessage)
        setAnnouncement('Failed to switch to text-only mode')
      } finally {
        setIsChanging(false)
      }
    }, [client, isChanging])

    /**
     * Handle voice selection
     * @param {string} voiceId - The ID of the voice to select
     */
    const handleVoiceSelect = React.useCallback(async (voiceId: string) => {
      if (!client || isChanging) return
      
      const voice = filteredVoices.find(v => v.voice_id === voiceId)
      const voiceLabel = formatVoiceDescription(voice || { voice_id: voiceId, vendor: 'Unknown' } as Voice)
      
      setIsChanging(true)
      setError(null)
      setAnnouncement(`Switching to ${voiceLabel}...`)
      
      try {
        await client.setAgentVoice(voiceId)
        // Small delay to ensure loading message is visible to screen readers
        await new Promise(resolve => setTimeout(resolve, 10))
        setAnnouncement(`Voice changed to ${voiceLabel}`)
      } catch (error) {
        const errorMessage = `Failed to set voice: ${voiceId}`
        console.error(errorMessage, error)
        setError(errorMessage)
        setAnnouncement(`Failed to switch to ${voiceLabel}`)
      } finally {
        setIsChanging(false)
      }
    }, [client, filteredVoices, isChanging])

    /**
     * Handle avatar selection (currently deferred)
     * @param {string} avatarId - The ID of the avatar to select
     * @todo Implement HeyGen avatar integration
     */
    const handleAvatarSelect = React.useCallback((avatarId: string) => {
      // Deferred implementation - log selection only
      const avatar = data.avatars.find(a => a.avatar_id === avatarId)
      const avatarLabel = avatar ? formatAvatarDescription(avatar) : avatarId
      
      // Avatar selection is deferred - will be implemented with HeyGen integration
      setAnnouncement(`Avatar ${avatarLabel} selection is not yet available`)
    }, [data.avatars])

    // Listen for voice change confirmations and clear errors
    React.useEffect(() => {
      if (!client) return

      const handleVoiceChanged = () => {
        // Voice successfully changed, clear any previous errors
        setError(null) // Clear any previous errors on successful change
      }

      client.on('agent_voice_changed', handleVoiceChanged)
      
      return () => {
        client.off('agent_voice_changed', handleVoiceChanged)
      }
    }, [client])
    
    // Clear error after timeout
    React.useEffect(() => {
      if (error) {
        const timer = setTimeout(() => setError(null), 5000)
        return () => clearTimeout(timer)
      }
    }, [error])

    // Connection state awareness - only disable when actually changing or not connected
    const isDisabled = disabled || !client || !isConnected || isChanging
    
    // Combined error state
    const displayError = error || voiceError
    
    // Determine button label - only show Loading when actually changing
    const buttonLabel = React.useMemo(() => {
      if (isChanging) return 'Loading...'
      // Show current mode even if initial loading is happening
      return currentMode.label
    }, [isChanging, currentMode.label])
    
    // Determine button icon
    const ButtonIcon = React.useMemo(() => {
      if (isChanging) return Loader2
      if (displayError) return AlertCircle
      return currentMode.icon
    }, [isChanging, displayError, currentMode.icon])

    return (
      <div className="relative">
        {/* Error Alert */}
        {showErrorAlerts && displayError && (
          <Alert variant="destructive" className="mb-2">
            <AlertCircle className="h-4 w-4" />
            <AlertDescription>{displayError}</AlertDescription>
          </Alert>
        )}
        
        {/* Screen reader live region for announcements */}
        <div 
          role="status" 
          aria-live="polite" 
          aria-atomic="true" 
          className="sr-only"
        >
          {announcement}
        </div>
        
        <Button
          ref={buttonRef}
          variant={displayError ? "destructive" : "outline"}
          role="combobox"
          aria-expanded={open}
          aria-haspopup="listbox"
          aria-label={ariaLabel || `Output mode selector. Current mode: ${buttonLabel}`}
          className={cn(
            "min-w-[180px] sm:w-[200px] justify-between transition-all duration-200",
            "focus-visible:ring-2 focus-visible:ring-ring focus-visible:ring-offset-2",
            isDisabled && "opacity-50 cursor-not-allowed",
            className
          )}
          disabled={isDisabled}
          onClick={() => setOpen(!open)}
        >
          <span className="flex items-center gap-2 truncate">
            {showIcon && (
              <ButtonIcon 
                className={cn(
                  "h-4 w-4 flex-shrink-0",
                  (isChanging || voiceLoading) && "animate-spin"
                )} 
              />
            )}
            <span className="truncate">
              {buttonLabel}
            </span>
          </span>
          {showIcon && (
            <ChevronDown 
              className={cn(
                "h-4 w-4 opacity-50 transition-transform duration-200",
                open && "rotate-180"
              )} 
            />
          )}
        </Button>
        
        <OutputSelectorPopover
          open={open}
          onClose={() => setOpen(false)}
          currentMode={currentMode}
          voices={filteredVoices}
          avatars={activeAvatars}
          onSelectTextOnly={handleTextOnly}
          onSelectVoice={handleVoiceSelect}
          onSelectAvatar={handleAvatarSelect}
          isChanging={isChanging}
          triggerRef={buttonRef}
          currentVoiceId={currentVoice?.voice_id || undefined}
          currentAvatarId={currentAvatarId}
        />
      </div>
    )
  }
)

OutputSelector.displayName = "OutputSelector"