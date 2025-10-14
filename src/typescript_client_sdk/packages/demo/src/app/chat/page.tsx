'use client';

import dynamicImport from 'next/dynamic';
import { Skeleton } from '@agentc/realtime-ui';

// Dynamically import the chat client component with no SSR
const ChatPageClient = dynamicImport(
  () => import('@/components/chat/ChatPageClient'),
  { 
    ssr: false,
    loading: () => (
      <div className="flex flex-col items-center justify-center min-h-screen space-y-4">
        <div className="space-y-2 text-center">
          <Skeleton className="h-8 w-48 mx-auto" />
          <Skeleton className="h-4 w-36 mx-auto" />
        </div>
        <p className="text-sm text-muted-foreground">
          Loading chat interface...
        </p>
      </div>
    )
  }
);

/**
 * Main chat page component
 * Uses dynamic import to avoid SSR issues with WebSocket and browser-only features
 */
export default function ChatPage() {
  return <ChatPageClient />;
}