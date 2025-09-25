'use client';

import React, { useEffect, useRef } from 'react';
import { ClientProvider } from '@/components/providers/client-provider';
import { ComparisonChatView } from './ComparisonChatView';
import { InputAreaWrapper } from '@/components/input/InputAreaWrapper';

interface StreamingPanelProps {
  syncScroll?: boolean;
}

/**
 * Streaming panel content - uses ChatMessagesView directly
 */
function StreamingContent({ syncScroll }: { syncScroll?: boolean }) {
  const scrollRef = useRef<HTMLDivElement>(null);

  // Handle synchronized scrolling
  useEffect(() => {
    if (!syncScroll || !scrollRef.current) return;

    const handleScroll = (e: Event) => {
      const customEvent = e as CustomEvent<{ scrollTop: number }>;
      if (customEvent.detail && typeof customEvent.detail.scrollTop === 'number') {
        if (scrollRef.current) {
          scrollRef.current.scrollTop = customEvent.detail.scrollTop;
        }
      }
    };

    window.addEventListener('resumed-panel-scroll', handleScroll as EventListener);
    return () => {
      window.removeEventListener('resumed-panel-scroll', handleScroll as EventListener);
    };
  }, [syncScroll]);

  // Emit scroll events for synchronization
  const handleScroll = () => {
    if (!syncScroll || !scrollRef.current) return;
    
    const event = new CustomEvent('streaming-panel-scroll', {
      detail: { scrollTop: scrollRef.current.scrollTop }
    });
    window.dispatchEvent(event);
  };

  return (
    <div className="h-full flex flex-col">
      {/* Messages Area */}
      <div 
        ref={scrollRef}
        className="flex-1 overflow-y-auto"
        onScroll={handleScroll}
      >
        <ComparisonChatView className="h-full" />
      </div>

      {/* Input Area */}
      <div className="border-t bg-accent/10 p-4">
        <InputAreaWrapper className="w-full max-w-4xl mx-auto" />
      </div>
    </div>
  );
}

/**
 * Streaming Panel - Shows normal WebSocket-connected chat
 */
export function StreamingPanel({ syncScroll }: StreamingPanelProps) {
  return (
    <ClientProvider
      apiUrl={process.env.NEXT_PUBLIC_AGENTC_API_URL}
      enableAudio={true}
      enableTurnManagement={true}
      respectTurnState={true}
    >
      <StreamingContent syncScroll={syncScroll} />
    </ClientProvider>
  );
}