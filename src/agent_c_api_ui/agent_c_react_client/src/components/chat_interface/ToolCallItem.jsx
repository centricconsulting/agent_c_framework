import React, { useState } from 'react';
import PropTypes from 'prop-types';
import { PocketKnife, ChevronDown } from 'lucide-react';
import { Collapsible, CollapsibleTrigger, CollapsibleContent } from "@/components/ui/collapsible";
import { Card, CardContent } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { cn } from "@/lib/utils";
import CopyButton from './CopyButton';

/**
 * Format data (arguments or results) as pretty JSON or string
 * @param {any} data - The data to format
 * @returns {string} - Formatted string representation
 */
function formatData(data) {
  if (!data) return "";

  try {
    if (typeof data === "string") {
      // Check if it looks like JSON before trying to parse
      if (data.trim().startsWith('{') || data.trim().startsWith('[')) {
        const parsed = JSON.parse(data);
        return JSON.stringify(parsed, null, 2);
      }
      return data;
    }
    return JSON.stringify(data, null, 2);
  } catch (e) {
    // If JSON parsing fails, return as string
    return String(data);
  }
}

/**
 * ToolCallItem component displays individual tool calls with their arguments and results.
 * 
 * @param {Object} props - Component props
 * @param {Object} props.tool - The tool object with name, arguments, and id
 * @param {any} props.results - The results of the tool call
 * @param {boolean} [props.integrated=false] - Whether this is integrated in an assistant message
 * @param {string} [props.className] - Optional additional class names
 */
const ToolCallItem = ({ tool, results, integrated = false, className = undefined }) => {
  const [isOpen, setIsOpen] = useState(false); // Default to collapsed

  if (!tool?.name) return null;

  const formattedArguments = tool.arguments ? formatData(tool.arguments) : "";
  const formattedResults = results ? formatData(results) : "";
  
  // Generate unique IDs for ARIA relationships
  const contentId = `tool-call-content-${tool.id || Math.random().toString(36).substr(2, 9)}`;

  // Determine styling based on integrated mode
  const cardStyles = integrated 
    ? "tool-call-item tool-call-item-integrated bg-transparent mb-2 overflow-hidden" 
    : "tool-call-item border border-primary/20 rounded-lg mb-2 overflow-hidden shadow-sm";
    
  const headerStyles = integrated
    ? "tool-call-item-header bg-muted/40 hover:bg-muted/60 border border-border rounded-md"
    : "tool-call-item-header bg-primary/5 hover:bg-primary/10 rounded-lg";
    
  const contentStyles = integrated
    ? "tool-call-item-content bg-muted/20 rounded-b-md border border-t-0 border-border p-3"
    : "tool-call-item-content border-t border-primary/20 bg-card p-4 rounded-b-lg";
    
  const codeStyles = "tool-call-code bg-muted/50 p-2 rounded-md text-sm font-mono whitespace-pre-wrap overflow-x-auto max-w-full border border-border";
    
  const sectionTitleStyles = "text-sm font-semibold text-primary";

  return (
    <Card 
      className={cn(cardStyles, className)}
      role="region"
      aria-label={`Tool call: ${tool.name}`}
    >
      <Collapsible open={isOpen} onOpenChange={setIsOpen}>
        <CollapsibleTrigger asChild>
          <div className={cn(
            headerStyles,
            "flex items-center justify-between p-3 cursor-pointer transition-colors"
          )}
          aria-expanded={isOpen}
          aria-controls={contentId}
          >
            <div className="flex items-center gap-2">
              <PocketKnife className="h-5 w-5 text-primary" aria-hidden="true" />
              <span className="font-medium text-foreground">
                {tool.name}
              </span>
            </div>
            <div className="flex items-center gap-1">
              {/* Copy entire tool call including arguments and results */}
              <div onClick={(e) => e.stopPropagation()} aria-label="Copy tool call data">
                <CopyButton
                  content={() => {
                    const toolData = {
                      name: tool.name,
                      arguments: tool.arguments ? (typeof tool.arguments === 'string' && tool.arguments.trim().startsWith('{') ? 
                        JSON.parse(tool.arguments) : tool.arguments) : {},
                      results: results
                    };
                    return JSON.stringify(toolData, null, 2);
                  }}
                  tooltipText="Copy tool call"
                  variant="ghost"
                  size="sm"
                  className="text-primary hover:text-primary/80 hover:bg-primary/10"
                />
              </div>
              <ChevronDown
                className={cn(
                  "h-4 w-4 text-primary transition-transform",
                  isOpen && "rotate-180"
                )}
                aria-hidden="true"
              />
            </div>
          </div>
        </CollapsibleTrigger>

        <CollapsibleContent id={contentId}>
          <CardContent className={cn(contentStyles, "p-0")}>
            {formattedArguments && (
              <div className="tool-call-item-section mt-2">
                <div className="tool-call-item-section-header flex justify-between items-center mb-1">
                  <h5 className={sectionTitleStyles}>Arguments:</h5>
                  <CopyButton
                    content={formattedArguments}
                    tooltipText="Copy arguments"
                    variant="ghost"
                    size="xs"
                    className="text-primary hover:text-primary/80 hover:bg-primary/10"
                  />
                </div>
                <pre className={codeStyles}>
                  {formattedArguments}
                </pre>
              </div>
            )}

            {formattedResults && (
              <div className="tool-call-item-section mt-2">
                <div className="tool-call-item-section-header flex justify-between items-center mb-1">
                  <h5 className={sectionTitleStyles}>Results:</h5>
                  <CopyButton
                    content={formattedResults}
                    tooltipText="Copy results"
                    variant="ghost"
                    size="xs"
                    className="text-primary hover:text-primary/80 hover:bg-primary/10"
                  />
                </div>
                <pre className={codeStyles}>
                  {formattedResults}
                </pre>
              </div>
            )}
          </CardContent>
        </CollapsibleContent>
      </Collapsible>
    </Card>
  );
};

ToolCallItem.propTypes = {
  tool: PropTypes.shape({
    name: PropTypes.string.isRequired,
    arguments: PropTypes.oneOfType([PropTypes.string, PropTypes.object]),
    id: PropTypes.string
  }).isRequired,
  results: PropTypes.any,
  integrated: PropTypes.bool,
  className: PropTypes.string
};

// Default props are now handled via parameter destructuring with default values

// Create a memoized version of ToolCallItem to prevent unnecessary re-renders
const MemoizedToolCallItem = React.memo(ToolCallItem, (prevProps, nextProps) => {
  // Compare tool properties
  const prevTool = prevProps.tool;
  const nextTool = nextProps.tool;
  
  if (prevTool.name !== nextTool.name) return false;
  if (prevTool.id !== nextTool.id) return false;
  
  // Compare arguments (could be strings or objects)
  const prevArgs = prevTool.arguments;
  const nextArgs = nextTool.arguments;
  
  if (typeof prevArgs !== typeof nextArgs) return false;
  
  if (typeof prevArgs === 'object' && prevArgs !== null) {
    if (JSON.stringify(prevArgs) !== JSON.stringify(nextArgs)) return false;
  } else if (prevArgs !== nextArgs) {
    return false;
  }
  
  // Compare results (could be any type)
  // For simplicity and reliability, we'll use JSON.stringify for comparison
  if (JSON.stringify(prevProps.results) !== JSON.stringify(nextProps.results)) {
    return false;
  }
  
  // Compare integrated mode
  if (prevProps.integrated !== nextProps.integrated) return false;
  
  // If we got here, nothing important changed, so prevent re-render
  return true;
});

export default MemoizedToolCallItem;