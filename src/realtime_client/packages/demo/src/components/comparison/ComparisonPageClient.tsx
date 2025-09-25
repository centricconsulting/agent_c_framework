'use client';

import React, { useState, useEffect } from 'react';
import { Button, Card, CardHeader, CardTitle, Alert, AlertDescription } from '@agentc/realtime-ui';
import { StreamingPanel } from './StreamingPanel';
import { ResumedPanel } from './ResumedPanel';
import { AuthProvider, useAuth } from '@/contexts/auth-context';
import { useRouter } from 'next/navigation';

/**
 * Auth guard for comparison page
 */
function ComparisonAuthGuard({ children }: { children: React.ReactNode }) {
  const router = useRouter();
  const { isAuthenticated, isLoading } = useAuth();

  useEffect(() => {
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
 * Comparison view content
 */
export function ComparisonContent() {
  const [sessionData, setSessionData] = useState<any>(null);
  const [loadError, setLoadError] = useState<string | null>(null);
  const [isLoading, setIsLoading] = useState(false);
  const [syncScroll, setSyncScroll] = useState(true);

  // Load the test session data
  const loadTestSession = async (abortSignal?: AbortSignal) => {
    setIsLoading(true);
    setLoadError(null);
    
    try {
      const response = await fetch('/api/load-test-session', { signal: abortSignal });
      if (!response.ok) {
        throw new Error(`Failed to load session: ${response.statusText}`);
      }
      
      const data = await response.json();
      setSessionData(data);
      console.log('[ComparisonView] Loaded test session:', {
        messageCount: data.messages?.length || 0,
        sessionId: data.session_id
      });
    } catch (error) {
      // Ignore abort errors
      if (error instanceof Error && error.name === 'AbortError') {
        return;
      }
      const message = error instanceof Error ? error.message : 'Failed to load test session';
      setLoadError(message);
      console.error('[ComparisonView] Error loading test session:', error);
    } finally {
      setIsLoading(false);
    }
  };

  // Auto-load on mount
  useEffect(() => {
    const abortController = new AbortController();
    loadTestSession(abortController.signal);
    
    return () => {
      abortController.abort();
    };
  }, []);

  return (
    <div className="h-screen flex flex-col bg-background">
      {/* Header */}
      <div className="border-b bg-card px-4 py-3 flex items-center justify-between">
        <div>
          <h1 className="text-lg font-semibold">Session Comparison View</h1>
          <p className="text-sm text-muted-foreground">
            Validate that resumed sessions render identically to streaming sessions
          </p>
        </div>
        
        <div className="flex items-center gap-2">
          <label className="flex items-center gap-2 text-sm">
            <input
              type="checkbox"
              checked={syncScroll}
              onChange={(e) => setSyncScroll(e.target.checked)}
              className="rounded border-gray-300"
            />
            Sync Scroll
          </label>
          
          <Button
            onClick={() => loadTestSession()}
            disabled={isLoading}
            variant="outline"
            size="sm"
          >
            {isLoading ? 'Loading...' : 'Reload Session'}
          </Button>
          
          <Button
            onClick={() => window.location.href = '/chat'}
            variant="outline"
            size="sm"
          >
            Back to Chat
          </Button>
        </div>
      </div>

      {/* Error Alert */}
      {loadError && (
        <div className="px-4 py-2">
          <Alert variant="destructive">
            <AlertDescription>
              {loadError}
            </AlertDescription>
          </Alert>
        </div>
      )}

      {/* Main Content - Side by Side Panels */}
      <div className="flex-1 flex overflow-hidden">
        {/* Streaming Panel - Left */}
        <div className="flex-1 border-r flex flex-col">
          <div className="bg-blue-50 dark:bg-blue-900/10 px-4 py-2 border-b">
            <h2 className="font-medium text-blue-900 dark:text-blue-100">
              Streaming Session
            </h2>
            <p className="text-xs text-blue-700 dark:text-blue-200">
              Normal WebSocket connection with real-time messaging
            </p>
          </div>
          
          <div className="flex-1 overflow-hidden">
            <StreamingPanel syncScroll={syncScroll} />
          </div>
        </div>

        {/* Resumed Panel - Right */}
        <div className="flex-1 flex flex-col">
          <div className="bg-green-50 dark:bg-green-900/10 px-4 py-2 border-b">
            <h2 className="font-medium text-green-900 dark:text-green-100">
              Resumed Session
            </h2>
            <p className="text-xs text-green-700 dark:text-green-200">
              Loaded from session_with_delegation.json
            </p>
          </div>
          
          <div className="flex-1 overflow-hidden">
            <ResumedPanel 
              sessionData={sessionData} 
              syncScroll={syncScroll}
            />
          </div>
        </div>
      </div>

      {/* Validation Status Bar */}
      <div className="border-t bg-card px-4 py-2">
        <div className="flex items-center justify-between text-sm">
          <div className="flex items-center gap-4">
            <span className="text-muted-foreground">Validation Checklist:</span>
            <label className="flex items-center gap-1">
              <input type="checkbox" className="rounded border-gray-300" />
              Think tools render as gray bubbles
            </label>
            <label className="flex items-center gap-1">
              <input type="checkbox" className="rounded border-gray-300" />
              Delegation shows subsession dividers
            </label>
            <label className="flex items-center gap-1">
              <input type="checkbox" className="rounded border-gray-300" />
              Roles preserved correctly
            </label>
            <label className="flex items-center gap-1">
              <input type="checkbox" className="rounded border-gray-300" />
              Visual appearance identical
            </label>
          </div>
          
          <div className="text-xs text-muted-foreground">
            Session: {sessionData?.session_id || 'Not loaded'}
          </div>
        </div>
      </div>
    </div>
  );
}

/**
 * Main comparison page client component
 */
export default function ComparisonPageClient() {
  return (
    <AuthProvider>
      <ComparisonAuthGuard>
        <ComparisonContent />
      </ComparisonAuthGuard>
    </AuthProvider>
  );
}