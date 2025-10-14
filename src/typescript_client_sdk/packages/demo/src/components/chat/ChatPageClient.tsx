'use client';

import React, { useEffect, useState } from 'react';
import { useRouter } from 'next/navigation';
import { AuthProvider, useAuth } from '@/contexts/auth-context';
import { ClientProvider } from '@/components/providers/client-provider';
import { ChatLayout } from '@/components/layout/ChatLayout';
import { Skeleton, Card, CardHeader, CardTitle, CardDescription, Button } from '@agentc/realtime-ui';


/**
 * AuthGuard component - Ensures user is authenticated before rendering children
 */
function AuthGuard({ children }: { children: React.ReactNode }) {
  const router = useRouter();
  const { isAuthenticated, isLoading } = useAuth();

  useEffect(() => {
    // Redirect to login if not authenticated after loading
    if (!isLoading && !isAuthenticated) {
      router.push('/login');
    }
  }, [isLoading, isAuthenticated, router]);

  // Show loading state while checking authentication
  if (isLoading) {
    return (
      <div className="flex flex-col items-center justify-center min-h-screen space-y-4">
        <div className="space-y-2 text-center">
          <Skeleton className="h-8 w-48 mx-auto" />
          <Skeleton className="h-4 w-36 mx-auto" />
        </div>
        <p className="text-sm text-muted-foreground">
          Verifying authentication...
        </p>
      </div>
    );
  }

  // Don't render children if not authenticated
  if (!isAuthenticated) {
    return (
      <div className="flex items-center justify-center min-h-screen p-4">
        <Card className="max-w-md">
          <CardHeader>
            <CardTitle>Authentication Required</CardTitle>
            <CardDescription>
              Please log in to access the chat interface.
            </CardDescription>
          </CardHeader>
          <div className="p-6 pt-0">
            <Button 
              onClick={() => router.push('/login')}
              className="w-full"
            >
              Go to Login
            </Button>
          </div>
        </Card>
      </div>
    );
  }

  // Render children when authenticated
  return <>{children}</>;
}

/**
 * Chat content component - The main chat interface with new layout
 */
function ChatContent() {
  const [sessionName, setSessionName] = useState<string>('New Chat');
  const [outputMode, setOutputMode] = useState<'chat' | 'avatar' | 'voice'>('chat');


  // Handle output mode changes
  const handleOutputModeChange = (mode: string) => {
    const modeMap: Record<string, 'chat' | 'avatar' | 'voice'> = {
      'text': 'chat',
      'voice': 'voice',
      'avatar': 'avatar'
    };
    setOutputMode(modeMap[mode] || 'chat');
  };

  return (
    <ChatLayout 
      outputMode={outputMode}
      sessionName={sessionName}
    />
  );
}

/**
 * Main chat page client component
 */
export default function ChatPageClient() {
  return (
    <AuthProvider>
      <AuthGuard>
        <ClientProvider
          apiUrl={process.env.NEXT_PUBLIC_AGENTC_API_URL}
          enableAudio={true}
          enableTurnManagement={true}
          respectTurnState={true}
        >
          <ChatContent />
        </ClientProvider>
      </AuthGuard>
    </AuthProvider>
  );
}