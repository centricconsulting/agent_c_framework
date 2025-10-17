import { Skeleton } from '@agentc/realtime-ui';

/**
 * Root loading component
 * Shown during page compilation and route transitions
 * Prevents chunk loading errors during initial dev compilation
 */
export default function Loading() {
  return (
    <div className="flex h-screen items-center justify-center">
      <div className="space-y-4 text-center">
        <Skeleton className="h-12 w-64 mx-auto" />
        <Skeleton className="h-4 w-48 mx-auto" />
        <p className="text-sm text-muted-foreground mt-4">
          Loading...
        </p>
      </div>
    </div>
  );
}
