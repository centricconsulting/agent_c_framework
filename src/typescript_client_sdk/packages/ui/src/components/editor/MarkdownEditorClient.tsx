"use client"

/**
 * Client-only wrapper for MarkdownEditor to prevent SSR issues with TipTap v3
 * 
 * This component ensures that TipTap code is never executed during server-side rendering,
 * avoiding browser API access errors during Next.js builds.
 * 
 * ## Usage
 * 
 * Use this component instead of MarkdownEditor when using SSR frameworks like Next.js:
 * 
 * ```tsx
 * import { MarkdownEditorClient } from '@agentc/ui';
 * 
 * function MyComponent() {
 *   const [value, setValue] = useState('');
 *   return (
 *     <MarkdownEditorClient
 *       value={value}
 *       onChange={setValue}
 *       onSubmit={handleSubmit}
 *     />
 *   );
 * }
 * ```
 * 
 * ## Alternative for Next.js Apps
 * 
 * If you're using Next.js and prefer to use their dynamic import with more control,
 * you can create your own wrapper:
 * 
 * ```tsx
 * import dynamic from 'next/dynamic';
 * import type { MarkdownEditorProps } from '@agentc/ui';
 * 
 * const MarkdownEditor = dynamic(
 *   () => import('@agentc/ui').then(mod => mod.MarkdownEditor),
 *   { 
 *     ssr: false,
 *     loading: () => <div>Loading editor...</div>
 *   }
 * );
 * ```
 */

import * as React from 'react';
import { cn } from '../../lib/utils';

// Import only the types, not the actual component to prevent SSR execution
import type { MarkdownEditorProps } from './types';

/**
 * Loading fallback component shown during dynamic import
 * This is a lightweight textarea that doesn't import any TipTap code
 */
const LoadingFallback = React.forwardRef<HTMLDivElement, MarkdownEditorProps>(
  ({ value = '', onChange, placeholder, disabled, className, onSubmit }, ref) => {
    const handleKeyDown = (e: React.KeyboardEvent<HTMLTextAreaElement>) => {
      // Handle Cmd/Ctrl+Enter for submit
      if (onSubmit && (e.metaKey || e.ctrlKey) && e.key === 'Enter') {
        e.preventDefault();
        onSubmit(value);
      }
    };

    const handleChange = (e: React.ChangeEvent<HTMLTextAreaElement>) => {
      onChange?.(e.target.value);
    };

    return (
      <div 
        ref={ref}
        className={cn(
          'relative w-full rounded-md border border-input bg-background',
          className
        )}
      >
        <textarea
          value={value}
          onChange={handleChange}
          onKeyDown={handleKeyDown}
          placeholder={placeholder || 'How can I help you today?'}
          disabled={disabled}
          className={cn(
            'w-full resize-none rounded-md px-3 py-2',
            'bg-transparent placeholder:text-muted-foreground',
            'focus:outline-none focus:ring-2 focus:ring-ring focus:ring-offset-2',
            'disabled:cursor-not-allowed disabled:opacity-50',
            'font-mono text-sm'
          )}
          aria-label="Markdown editor loading fallback"
        />
        <div className="absolute bottom-2 right-2 text-xs text-muted-foreground">
          Loading editor...
        </div>
      </div>
    );
  }
);

LoadingFallback.displayName = 'MarkdownEditorLoadingFallback';

/**
 * Lazy-loaded MarkdownEditor component
 * This ensures TipTap code is only loaded in the browser
 */
const LazyMarkdownEditor = React.lazy(async () => {
  const module = await import('./MarkdownEditor');
  return { default: module.MarkdownEditor };
});

/**
 * Client-only MarkdownEditor wrapper
 * 
 * This component provides the same API as MarkdownEditor but ensures
 * it only loads in the browser, preventing SSR compatibility issues.
 * 
 * During SSR and initial page load, a lightweight textarea fallback is shown.
 * Once the client hydrates, the full TipTap editor is loaded.
 * 
 * @example
 * ```tsx
 * import { MarkdownEditorClient } from '@agentc/ui';
 * 
 * function MyComponent() {
 *   const [value, setValue] = useState('');
 *   
 *   return (
 *     <MarkdownEditorClient
 *       value={value}
 *       onChange={setValue}
 *       onSubmit={(text) => console.log('Submitted:', text)}
 *     />
 *   );
 * }
 * ```
 */
export const MarkdownEditorClient = React.forwardRef<HTMLDivElement, MarkdownEditorProps>(
  (props, ref) => {
    // Check if we're in the browser
    const [isClient, setIsClient] = React.useState(false);

    React.useEffect(() => {
      // This effect only runs on the client
      setIsClient(true);
    }, []);

    // During SSR or before client hydration, show the fallback
    if (!isClient) {
      return <LoadingFallback {...props} ref={ref} />;
    }

    // Once on client, render the lazy-loaded component with Suspense
    return (
      <React.Suspense fallback={<LoadingFallback {...props} ref={ref} />}>
        <LazyMarkdownEditor {...props} ref={ref} />
      </React.Suspense>
    );
  }
);

MarkdownEditorClient.displayName = 'MarkdownEditorClient';

/**
 * Re-export the props type for convenience
 */
export type { MarkdownEditorProps } from './types';