import React from 'react';
import dynamicImport from 'next/dynamic';
import { Skeleton } from '@agentc/realtime-ui';

// Force dynamic rendering to prevent static generation issues
export const dynamic = 'force-dynamic';
export const revalidate = 0;

// Dynamically import the comparison client component with no SSR
const ComparisonPageClient = dynamicImport(
  () => import('@/components/comparison/ComparisonPageClient'),
  { 
    ssr: false,
    loading: () => (
      <div className="flex flex-col items-center justify-center min-h-screen space-y-4">
        <div className="space-y-2 text-center">
          <Skeleton className="h-8 w-64 mx-auto" />
          <Skeleton className="h-4 w-48 mx-auto" />
        </div>
        <p className="text-sm text-muted-foreground">
          Loading comparison view...
        </p>
      </div>
    )
  }
);

/**
 * Comparison page for validating resumed sessions vs streaming sessions
 * Shows side-by-side view to ensure identical rendering
 */
export default function ComparisonPage() {
  return <ComparisonPageClient />;
}