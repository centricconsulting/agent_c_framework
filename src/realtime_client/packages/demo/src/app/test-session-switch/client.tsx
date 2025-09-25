'use client';

import React from 'react';
import { AgentCProvider } from '@agentc/realtime-react';
import { AuthProvider, useAuth } from '@/contexts/auth-context';
import { SessionSwitchTestPanel } from '@/components/testing/SessionSwitchTestPanel';
import { Button } from '@agentc/realtime-ui';
import { useRouter } from 'next/navigation';
import { ArrowLeft } from 'lucide-react';

/**
 * Auth guard wrapper
 */
function TestPageAuthGuard({ children }: { children: React.ReactNode }) {
  const router = useRouter();
  const { isAuthenticated, isLoading } = useAuth();

  React.useEffect(() => {
    if (!isLoading && !isAuthenticated) {
      router.push('/login');
    }
  }, [isLoading, isAuthenticated, router]);

  if (isLoading) {
    return (
      <div className="flex items-center justify-center min-h-screen">
        <p className="text-sm text-muted-foreground">Verifying authentication...</p>
      </div>
    );
  }

  if (!isAuthenticated) {
    return null;
  }

  return <>{children}</>;
}

/**
 * Test page content
 */
function TestPageContent() {
  const router = useRouter();
  
  return (
    <div className="min-h-screen bg-background">
      <div className="border-b">
        <div className="container mx-auto px-4 py-4 flex items-center justify-between">
          <div className="flex items-center gap-4">
            <Button
              variant="ghost"
              size="sm"
              onClick={() => router.push('/chat')}
            >
              <ArrowLeft className="mr-2 h-4 w-4" />
              Back to Chat
            </Button>
            <div className="h-6 w-px bg-border" />
            <div>
              <h1 className="text-lg font-semibold">Session Switch Testing</h1>
              <p className="text-sm text-muted-foreground">
                Validate message clearing during session switches
              </p>
            </div>
          </div>
        </div>
      </div>
      
      <div className="container mx-auto px-4 py-8">
        <SessionSwitchTestPanel />
      </div>
    </div>
  );
}

/**
 * Main client component with providers
 */
export default function SessionSwitchTestClient() {
  const apiUrl = process.env.NEXT_PUBLIC_AGENTC_API_URL || 'ws://localhost:8080';
  const debug = process.env.NODE_ENV === 'development';

  return (
    <AgentCProvider apiUrl={apiUrl} debug={debug}>
      <AuthProvider>
        <TestPageAuthGuard>
          <TestPageContent />
        </TestPageAuthGuard>
      </AuthProvider>
    </AgentCProvider>
  );
}