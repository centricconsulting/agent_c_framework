/**
 * Example integration of test harness with chat components
 * This demonstrates how to use the test session functionality
 */

'use client';

import React, { useEffect } from 'react';
import { useChat, useConnection } from '@agentc/realtime-react';
import type { ChatItem, MessageChatItem } from '@agentc/realtime-react';
import { useTestSession } from '@/hooks/use-test-session';
import { TestControls } from './test-controls';
import { Card, CardHeader, CardTitle, CardContent, ScrollArea, Badge } from '@agentc/realtime-ui';

/**
 * Example chat component with test harness integration
 */
export function TestIntegrationExample() {
  const { messages, streamingMessage, isAgentTyping } = useChat();
  const { isConnected } = useConnection();
  const { 
    testModeEnabled, 
    currentScenario,
    loadScenario,
    scenarios 
  } = useTestSession();

  /**
   * Auto-load test scenario on mount if in test mode
   */
  useEffect(() => {
    if (testModeEnabled && isConnected && !currentScenario && scenarios.length > 0) {
      // Auto-load the delegation scenario for testing
      const delegationScenario = scenarios.find(s => s.id === 'session-with-delegation');
      if (delegationScenario) {
        loadScenario(delegationScenario.id).catch(console.error);
      }
    }
  }, [testModeEnabled, isConnected, currentScenario, scenarios, loadScenario]);

  /**
   * Render message content based on type
   */
  const renderMessageContent = (content: any) => {
    if (typeof content === 'string') {
      return <p>{content}</p>;
    }

    if (Array.isArray(content)) {
      return content.map((block: any, index: number) => {
        switch (block.type) {
          case 'text':
            return <p key={index}>{block.text || block.content}</p>;
          
          case 'thinking':
            return (
              <div key={index} className="thinking-block p-2 bg-muted rounded italic">
                💭 {block.text}
              </div>
            );
          
          case 'tool_use':
            // Special handling for think tool
            if (block.name === 'think') {
              return (
                <div key={index} className="thinking-block p-2 bg-muted rounded italic">
                  💭 {block.input?.thought}
                </div>
              );
            }
            
            // Special handling for delegation tools
            if (block.name?.startsWith('act_') || 
                block.name?.startsWith('ateam_') || 
                block.name?.startsWith('aa_')) {
              return (
                <div key={index} className="delegation-block p-2 border rounded">
                  <Badge variant="secondary" className="mb-2">
                    🤝 Delegation: {block.name}
                  </Badge>
                  {block.input?.agent_key && (
                    <div className="text-sm text-muted-foreground">
                      Agent: {block.input.agent_key}
                    </div>
                  )}
                  <div className="mt-1">
                    {block.input?.request}
                  </div>
                </div>
              );
            }
            
            // Default tool rendering
            return (
              <div key={index} className="tool-block p-2 border rounded">
                <Badge variant="outline" className="mb-1">
                  🔧 Tool: {block.name}
                </Badge>
              </div>
            );
          
          case 'tool_result':
            // Parse delegation tool results
            if (block.content?.includes('**IMPORTANT**:')) {
              const parts = block.content.split('---\n');
              if (parts.length > 1) {
                const yamlContent = parts[1];
                const textMatch = yamlContent.match(/text:\s*'([\s\S]+?)'/);
                const text = textMatch ? textMatch[1] : yamlContent;
                return (
                  <div key={index} className="tool-result p-2 bg-secondary/10 rounded">
                    {text}
                  </div>
                );
              }
            }
            return null;
          
          default:
            return null;
        }
      });
    }

    return <p>{JSON.stringify(content)}</p>;
  };

  return (
    <div className="flex flex-col h-screen">
      {/* Test Controls Header */}
      {testModeEnabled && (
        <div className="border-b p-4 bg-muted/50">
          <div className="flex items-center justify-between">
            <div className="flex items-center gap-2">
              <Badge variant="secondary">Test Mode Active</Badge>
              {currentScenario && (
                <Badge variant="outline">{currentScenario.name}</Badge>
              )}
            </div>
            <TestControls compact />
          </div>
        </div>
      )}

      {/* Chat Messages Area */}
      <Card className="flex-1 m-4">
        <CardHeader>
          <CardTitle>Chat Messages</CardTitle>
        </CardHeader>
        <CardContent>
          <ScrollArea className="h-[500px] pr-4">
            <div className="space-y-4">
              {messages.map((item, index) => {
                // Skip non-message items for now
                if (item.type !== 'message') {
                  return null;
                }
                
                const message = item as MessageChatItem;
                
                return (
                  <div
                    key={index}
                    className={`message p-3 rounded ${
                      message.role === 'user' 
                        ? 'bg-primary/10 ml-auto max-w-[70%]' 
                        : 'bg-secondary/10 mr-auto max-w-[70%]'
                    }`}
                  >
                    <div className="text-xs text-muted-foreground mb-1">
                      {message.role}
                    </div>
                    <div className="message-content">
                      {renderMessageContent(message.content)}
                    </div>
                  </div>
                );
              })}

              {/* Streaming Message */}
              {streamingMessage && (
                <div className="message p-3 rounded bg-secondary/10 mr-auto max-w-[70%] animate-pulse">
                  <div className="text-xs text-muted-foreground mb-1">
                    assistant (streaming)
                  </div>
                  <div className="message-content">
                    {renderMessageContent(streamingMessage.content)}
                    <span className="inline-block w-2 h-4 bg-current animate-blink ml-1">▊</span>
                  </div>
                </div>
              )}

              {/* Typing Indicator */}
              {isAgentTyping && !streamingMessage && (
                <div className="flex items-center gap-1 text-muted-foreground">
                  <span className="text-sm">Agent is typing</span>
                  <span className="flex gap-1">
                    <span className="w-2 h-2 bg-current rounded-full animate-bounce" style={{ animationDelay: '0ms' }} />
                    <span className="w-2 h-2 bg-current rounded-full animate-bounce" style={{ animationDelay: '150ms' }} />
                    <span className="w-2 h-2 bg-current rounded-full animate-bounce" style={{ animationDelay: '300ms' }} />
                  </span>
                </div>
              )}
            </div>
          </ScrollArea>
        </CardContent>
      </Card>

      {/* Connection Status */}
      <div className="p-4 border-t">
        <div className="flex items-center gap-2">
          <div className={`w-2 h-2 rounded-full ${isConnected ? 'bg-green-500' : 'bg-red-500'}`} />
          <span className="text-sm text-muted-foreground">
            {isConnected ? 'Connected' : 'Disconnected'}
          </span>
          {testModeEnabled && currentScenario && (
            <>
              <span className="text-muted-foreground">•</span>
              <span className="text-sm text-muted-foreground">
                {messages.length} messages loaded
              </span>
            </>
          )}
        </div>
      </div>
    </div>
  );
}

/**
 * CSS for animations (add to your global styles)
 */
const animationStyles = `
@keyframes blink {
  0%, 50% { opacity: 1; }
  51%, 100% { opacity: 0; }
}

.animate-blink {
  animation: blink 1s infinite;
}
`;