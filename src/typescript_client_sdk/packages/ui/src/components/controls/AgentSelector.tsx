"use client"

/**
 * AgentSelector Component
 * 
 * A searchable agent selector with scroll support and rich metadata display.
 * Uses React Portal for rendering outside DOM hierarchy to avoid parent constraints.
 */

import * as React from "react"
import * as ReactDOM from "react-dom"
import { Bot, Check, ChevronDown, Loader2, Search, X } from "lucide-react"
import { useAgentCData, useConnection, useRealtimeClientSafe, AgentStorage } from "@agentc/realtime-react"
import type { Agent } from "@agentc/realtime-react"
import { cn } from "../../lib/utils"
import { Badge } from "../ui/badge"
import { Button } from "../ui/button"
import { Input } from "../ui/input"
import { ScrollArea } from "../ui/scroll-area"
import { Card } from "../ui/card"

export interface AgentSelectorProps {
  className?: string
  disabled?: boolean
  showIcon?: boolean
  placeholder?: string
  searchPlaceholder?: string
}

/**
 * Format tool class names for display
 */
function formatToolName(toolClass: string): string {
  let formatted = toolClass.replace(/Tools$/, '')
  formatted = formatted.replace(/([A-Z])/g, ' $1').trim()
  return formatted
}

/**
 * Get first sentence from description
 */
function getFirstSentence(text: string): string {
  if (!text) return ''
  
  const cleanText = text.replace(/\s+/g, ' ').trim()
  const match = cleanText.match(/^[^.!?]*[.!?]/)
  
  if (match) {
    return match[0].trim()
  }
  
  if (cleanText.length > 120) {
    const truncated = cleanText.substring(0, 120)
    const lastSpace = truncated.lastIndexOf(' ')
    if (lastSpace > 100) {
      return truncated.substring(0, lastSpace) + '...'
    }
    return truncated + '...'
  }
  
  return cleanText
}

/**
 * Custom Popover component using React Portal and fixed positioning
 * Renders outside DOM hierarchy to avoid parent container constraints
 */
const AgentSelectorPopover: React.FC<{
  open: boolean
  onClose: () => void
  agents: Agent[]
  selectedAgent: string
  onSelect: (key: string) => void
  isChanging: boolean
  triggerRef: React.RefObject<HTMLButtonElement>
}> = ({ open, onClose, agents, selectedAgent, onSelect, isChanging, triggerRef }) => {
  const [searchValue, setSearchValue] = React.useState('')
  const [position, setPosition] = React.useState({ top: 0, left: 0, width: 0, maxHeight: 400 })
  const popoverRef = React.useRef<HTMLDivElement>(null)
  const searchInputRef = React.useRef<HTMLInputElement>(null)
  
  // Filter agents based on search
  const filteredAgents = React.useMemo(() => {
    if (!searchValue) return agents
    
    const search = searchValue.toLowerCase()
    return agents.filter(agent => {
      const name = (agent.name || agent.key).toLowerCase()
      const description = (agent.agent_description || '').toLowerCase()
      const tools = (agent.tools || []).join(' ').toLowerCase()
      
      return name.includes(search) || 
             description.includes(search) || 
             tools.includes(search)
    })
  }, [agents, searchValue])
  
  // Calculate position and focus search input when opened
  React.useEffect(() => {
    if (open && triggerRef.current) {
      const rect = triggerRef.current.getBoundingClientRect()
      const viewportHeight = window.innerHeight
      const viewportWidth = window.innerWidth
      const spaceBelow = viewportHeight - rect.bottom - 20 // 20px for padding
      const spaceAbove = rect.top - 20
      
      // Calculate max height that fits in viewport
      const preferredHeight = 400
      const minHeight = 200
      
      let top: number
      let maxHeight: number
      
      // Prefer positioning below if there's enough space
      if (spaceBelow >= minHeight) {
        // Position below
        top = rect.bottom + 8
        maxHeight = Math.min(preferredHeight, spaceBelow)
      } else if (spaceAbove >= minHeight) {
        // Position above if not enough space below
        maxHeight = Math.min(preferredHeight, spaceAbove)
        top = rect.top - maxHeight - 8
      } else {
        // If neither has enough space, use the larger space
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
      const popoverWidth = 400
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
      
      // Focus search input
      setTimeout(() => searchInputRef.current?.focus(), 100)
    }
  }, [open, triggerRef])
  
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
      
      {/* Popover Content - Fixed positioning with dynamic height */}
      <div
        ref={popoverRef}
        className={cn(
          "fixed z-[9999] w-[400px] max-w-[calc(100vw-2rem)]",
          "bg-background border border-border rounded-lg shadow-xl",
          "animate-in fade-in-0 zoom-in-95",
          "flex flex-col overflow-hidden" // Add flex layout
        )}
        style={{
          top: `${position.top}px`,
          left: `${position.left}px`,
          minWidth: `${Math.max(position.width, 200)}px`,
          maxHeight: `${position.maxHeight}px`,
          height: 'auto'
        }}
        role="listbox"
        aria-label="Select an agent"
      >
        {/* Search Header - Fixed height */}
        <div className="flex-shrink-0 p-3 border-b border-border">
          <div className="flex items-center gap-2">
            <Search className="h-4 w-4 text-muted-foreground" />
            <Input
              ref={searchInputRef}
              type="text"
              placeholder="Search agents..."
              value={searchValue}
              onChange={(e) => setSearchValue(e.target.value)}
              className="flex-1 h-8 border-0 p-0 focus-visible:ring-0"
              aria-label="Search agents"
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
        
        {/* Agents List - Flexible height that fills available space */}
        <ScrollArea className="flex-1 min-h-0 overflow-auto">
          <div className="p-2">
            {filteredAgents.length === 0 ? (
              <div className="text-center py-8 text-muted-foreground text-sm">
                {searchValue ? "No agents found matching your search." : "No agents available."}
              </div>
            ) : (
              filteredAgents.map((agent) => {
                const isSelected = agent.key === selectedAgent
                const tools = agent.tools || 
                            (agent as any).tool_classes || 
                            (agent as any).toolClasses || 
                            (agent as any).equipped_tools ||
                            []
                
                return (
                  <button
                    key={agent.key}
                    onClick={() => {
                      onSelect(agent.key)
                      setSearchValue('')
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
                    aria-label={`Agent: ${agent.name}. ${agent.agent_description || 'No description'}`}
                  >
                    {/* Agent Header */}
                    <div className="flex items-start justify-between mb-1">
                      <div className="flex items-center gap-2">
                        <Bot className="h-4 w-4 text-muted-foreground mt-0.5" />
                        <span className="font-medium text-sm">
                          {agent.name || agent.key}
                        </span>
                      </div>
                      {isSelected && (
                        <Check className="h-4 w-4 text-primary" aria-label="Currently selected" />
                      )}
                    </div>
                    
                    {/* Description */}
                    {agent.agent_description && (
                      <p className="text-xs text-muted-foreground mb-2 pl-6 line-clamp-2">
                        {getFirstSentence(agent.agent_description)}
                      </p>
                    )}
                    
                    {/* Tool Badges */}
                    {Array.isArray(tools) && tools.length > 0 && (
                      <div className="flex flex-wrap gap-1 pl-6">
                        {tools.slice(0, 4).map((toolClass, index) => (
                          <Badge 
                            key={`${toolClass}-${index}`} 
                            variant="secondary" 
                            className="text-xs py-0 h-5"
                          >
                            {formatToolName(toolClass)}
                          </Badge>
                        ))}
                        {tools.length > 4 && (
                          <Badge 
                            variant="outline" 
                            className="text-xs py-0 h-5 text-muted-foreground"
                          >
                            +{tools.length - 4} more
                          </Badge>
                        )}
                      </div>
                    )}
                  </button>
                )
              })
            )}
          </div>
        </ScrollArea>
        
        {/* Footer with agent count - Fixed height */}
        <div className="flex-shrink-0 p-2 border-t border-border">
          <p className="text-xs text-muted-foreground text-center">
            {filteredAgents.length} of {agents.length} agents
            {searchValue && " matching search"}
          </p>
        </div>
      </div>
    </>,
    document.body // Render at document.body level
  )
}

export const AgentSelector = React.forwardRef<HTMLButtonElement, AgentSelectorProps>(
  ({ 
    className, 
    disabled = false, 
    showIcon = true, 
    placeholder = "Select an agent",
    searchPlaceholder = "Search agents..."
  }, ref) => {
    const client = useRealtimeClientSafe()
    const { data, isInitialized, isLoading } = useAgentCData()
    const { isConnected } = useConnection()
    
    const [isChanging, setIsChanging] = React.useState(false)
    const [open, setOpen] = React.useState(false)
    const buttonRef = React.useRef<HTMLButtonElement>(null)
    
    // Merge refs
    React.useImperativeHandle(ref, () => buttonRef.current!, [])
    
    // Use currentAgentConfig from hook as single source of truth
    const selectedAgent = data.currentAgentConfig?.key || ''
    
    const handleAgentSelect = async (agentKey: string) => {
      if (!client || agentKey === selectedAgent) return
      
      setIsChanging(true)
      setOpen(false)
      
      try {
        // Persist agent selection to localStorage
        try {
          AgentStorage.saveAgentKey(agentKey)
        } catch (error) {
          console.error('Failed to persist agent selection:', error)
          // Continue with selection even if persistence fails
        }
        
        // Update the client's preferred agent for future connections
        client.setPreferredAgentKey(agentKey)
        
        // Just call setAgent - the hook will update when server confirms
        client.setAgent(agentKey)
      } finally {
        setTimeout(() => setIsChanging(false), 300)
      }
    }
    
    const isDisabled = disabled || !client || !isConnected || isChanging
    const currentAgent = data.agents.find(agent => agent.key === selectedAgent)
    const ButtonIcon = isChanging ? Loader2 : Bot
    
    return (
      <div className="relative">
        <Button
          ref={buttonRef}
          variant="outline"
          role="combobox"
          aria-expanded={open}
          aria-haspopup="listbox"
          aria-label={`Agent selector. Current agent: ${currentAgent?.name || 'None selected'}`}
          className={cn(
            "min-w-[180px] sm:w-[200px] justify-between",
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
                  isChanging && "animate-spin"
                )} 
              />
            )}
            <span className="truncate">
              {currentAgent ? (
                currentAgent.name || currentAgent.key
              ) : (
                isLoading ? "Loading agents..." : placeholder
              )}
            </span>
          </span>
          <ChevronDown 
            className={cn(
              "h-4 w-4 opacity-50 transition-transform duration-200",
              open && "rotate-180"
            )} 
          />
        </Button>
        
        <AgentSelectorPopover
          open={open}
          onClose={() => setOpen(false)}
          agents={data.agents}
          selectedAgent={selectedAgent}
          onSelect={handleAgentSelect}
          isChanging={isChanging}
          triggerRef={buttonRef}
        />
      </div>
    )
  }
)

AgentSelector.displayName = "AgentSelector"