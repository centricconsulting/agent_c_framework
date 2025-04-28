import React, { useRef, useEffect } from 'react';
import { Card, CardContent } from "@/components/ui/card";
import { cn } from "@/lib/utils";
import ModelIcon from './ModelIcon';
import CopyButton from './CopyButton';
import MarkdownMessage from "@/components/chat_interface/MarkdownMessage";

/**
 * ThoughtDisplay component displays AI thinking processes in a visually distinct container
 * with auto-scrolling functionality for streaming content.
 * 
 * @param {Object} props - Component props
 * @param {string} props.content - The thinking content to display
 * @param {string} props.vendor - The AI model vendor
 * @param {string} [props.className] - Optional additional class names
 */
const ThoughtDisplay = ({ content, vendor, className }) => {
    const contentRef = useRef(null);
    const markdownRef = useRef(null);

    // Handle auto-scrolling for ongoing content streaming
    useEffect(() => {
        if (!contentRef.current) return;

        // Check if user is scrolled near the bottom (within 100px)
        const isNearBottom =
            contentRef.current.scrollHeight - contentRef.current.scrollTop - contentRef.current.clientHeight < 100;

        // Only auto-scroll if user is already near the bottom
        if (isNearBottom) {
            contentRef.current.scrollTop = contentRef.current.scrollHeight;
        }
    }, [content]);

    return (
        <div className={cn("flex justify-start items-start gap-2 group mb-3", className)}>
            <div className="flex-shrink-0 mt-1">
                <ModelIcon vendor={vendor} />
            </div>
            
            <Card className="thought-content" >
                <CardContent className="flex justify-between items-start gap-2 p-2">
                    <div
                        ref={contentRef}
                        className="thought-scrollable-content flex-1 scrollbar-thin scrollbar-thumb-primary/30 scrollbar-track-primary/10"
                    >
                        <div ref={markdownRef}>
                            <MarkdownMessage content={content} />
                        </div>
                    </div>
                    
                    <CopyButton
                        content={content}
                        tooltipText="Copy thinking"
                        position="left"
                        variant="secondary"
                        size="xs"
                        className="mt-1 flex-shrink-0 opacity-0 group-hover:opacity-100 transition-opacity"
                    />
                </CardContent>
            </Card>
        </div>
    );
};

// Create a memoized version of ThoughtDisplay to prevent unnecessary re-renders
const MemoizedThoughtDisplay = React.memo(ThoughtDisplay, (prevProps, nextProps) => {
  // Only re-render if essential props have changed
  return (
    prevProps.content === nextProps.content &&
    prevProps.vendor === nextProps.vendor
  );
});

export default MemoizedThoughtDisplay;