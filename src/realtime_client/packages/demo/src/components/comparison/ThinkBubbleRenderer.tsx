'use client';

import * as React from 'react';
import { cn } from '@/lib/utils';
import { Brain } from 'lucide-react';

interface ThinkBubbleRendererProps {
  thought: string;
  className?: string;
}

/**
 * Renders think tool content as a gray bubble
 * Matches the visual style requested for think tools
 */
export const ThinkBubbleRenderer: React.FC<ThinkBubbleRendererProps> = ({
  thought,
  className
}) => {
  return (
    <div 
      className={cn(
        "inline-flex items-start gap-2 rounded-2xl px-4 py-2.5",
        "bg-gray-100 dark:bg-gray-800 text-gray-700 dark:text-gray-300",
        "max-w-[90%]",
        className
      )}
      role="note"
      aria-label="Agent thought process"
    >
      <Brain 
        className="h-4 w-4 mt-0.5 text-gray-500 dark:text-gray-400 shrink-0" 
        aria-hidden="true"
      />
      <span className="text-sm leading-relaxed">
        {thought}
      </span>
    </div>
  );
};