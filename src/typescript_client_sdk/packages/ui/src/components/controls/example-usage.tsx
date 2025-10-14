/**
 * Example usage of OutputSelector and AgentSelector components
 * 
 * This example demonstrates how to use the selector components
 * in a typical chat interface control panel.
 */

import * as React from 'react'
import { OutputSelector } from './OutputSelector'
import { AgentSelector } from './AgentSelector'

export function ChatControls() {
  return (
    <div className="flex items-center gap-2 p-4 border rounded-md bg-background">
      {/* Output Mode Selector */}
      <div className="flex flex-col gap-1">
        <label className="text-sm font-medium text-muted-foreground">
          Output Mode
        </label>
        <OutputSelector />
      </div>
      
      {/* Agent Selector */}
      <div className="flex flex-col gap-1">
        <label className="text-sm font-medium text-muted-foreground">
          AI Agent
        </label>
        <AgentSelector />
      </div>
    </div>
  )
}

// Example with props customization
export function CustomizedControls() {
  return (
    <div className="flex flex-col gap-4">
      {/* Compact version without icons */}
      <div className="flex gap-2">
        <OutputSelector 
          showIcon={false}
          className="w-40" 
        />
        <AgentSelector 
          showIcon={false}
          className="w-40"
          placeholder="Choose AI"
        />
      </div>
      
      {/* Full featured version */}
      <div className="flex gap-2">
        <OutputSelector />
        <AgentSelector />
      </div>
      
      {/* Disabled state example */}
      <div className="flex gap-2">
        <OutputSelector disabled />
        <AgentSelector disabled />
      </div>
    </div>
  )
}

// Example integrated into a chat header
export function ChatHeader() {
  return (
    <header className="flex items-center justify-between p-4 border-b">
      <div className="flex items-center gap-4">
        <h1 className="text-xl font-semibold">Agent C Realtime Chat</h1>
      </div>
      
      <div className="flex items-center gap-2">
        <OutputSelector />
        <AgentSelector />
      </div>
    </header>
  )
}