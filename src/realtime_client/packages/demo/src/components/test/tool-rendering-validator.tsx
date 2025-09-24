/**
 * Tool Rendering Validator Component
 * Validates the visual rendering of delegation and think tools
 */

'use client';

import React, { useEffect, useState, useRef, useCallback } from 'react';
import { 
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
  Badge,
  Alert,
  AlertDescription,
  ScrollArea,
  Separator,
  Button,
  Tooltip,
  TooltipContent,
  TooltipProvider,
  TooltipTrigger,
} from '@agentc/realtime-ui';
import { cn } from '@/lib/utils';
import { 
  GitBranch,
  Brain,
  CheckCircle2,
  XCircle,
  AlertTriangle,
  Info,
  RefreshCw,
  Eye,
  EyeOff,
  MessageSquare,
  ArrowRight,
  ArrowDown,
  Layers,
  Hash,
} from 'lucide-react';

/**
 * Tool type definition
 */
type ToolType = 'delegation' | 'think' | 'other';

/**
 * Validation rule
 */
interface ValidationRule {
  id: string;
  name: string;
  description: string;
  selector?: string;
  check: (element: Element | null, message: any) => boolean;
  severity: 'error' | 'warning' | 'info';
}

/**
 * Validation result
 */
interface ValidationResult {
  ruleId: string;
  ruleName: string;
  passed: boolean;
  message: string;
  severity: 'error' | 'warning' | 'info';
  element?: Element | null;
  details?: any;
}

/**
 * Tool rendering validator props
 */
export interface ToolRenderingValidatorProps {
  messages: any[];
  className?: string;
  autoRun?: boolean;
  showVisualGuides?: boolean;
}

/**
 * Tool Rendering Validator Component
 */
export function ToolRenderingValidator({
  messages,
  className = '',
  autoRun = true,
  showVisualGuides = false,
}: ToolRenderingValidatorProps) {
  const [validationResults, setValidationResults] = useState<Map<string, ValidationResult[]>>(new Map());
  const [isValidating, setIsValidating] = useState(false);
  const [showDetails, setShowDetails] = useState(true);
  const [highlightedElements, setHighlightedElements] = useState<Set<Element>>(new Set());
  const observerRef = useRef<MutationObserver | null>(null);

  // Delegation tool validation rules
  const delegationRules: ValidationRule[] = [
    {
      id: 'subsession-divider',
      name: 'Subsession Divider',
      description: 'Delegation tools should display a subsession divider',
      selector: '.subsession-divider',
      check: (element) => !!element,
      severity: 'error',
    },
    {
      id: 'nested-indentation',
      name: 'Nested Indentation',
      description: 'Delegation responses should be indented',
      selector: '.delegation-response',
      check: (element) => {
        if (!element) return false;
        const style = window.getComputedStyle(element);
        const marginLeft = parseInt(style.marginLeft) || 0;
        const paddingLeft = parseInt(style.paddingLeft) || 0;
        return (marginLeft + paddingLeft) >= 20;
      },
      severity: 'warning',
    },
    {
      id: 'delegation-header',
      name: 'Delegation Header',
      description: 'Should show which agent was delegated to',
      selector: '.delegation-header',
      check: (element) => !!element && element.textContent !== '',
      severity: 'info',
    },
    {
      id: 'delegation-result-transform',
      name: 'Result Transformation',
      description: 'Tool results should be properly transformed',
      check: (element, message) => {
        // Check if the tool result content is properly formatted
        if (!message.content) return false;
        const content = Array.isArray(message.content) ? message.content : [message.content];
        const hasToolResult = content.some((item: any) => item.type === 'tool_result');
        if (!hasToolResult) return true; // No result to transform
        
        // Check if the result is displayed as a message, not raw JSON
        const resultElement = element?.querySelector('.tool-result-content');
        if (!resultElement) return false;
        
        // Ensure it's not showing raw JSON
        const text = resultElement.textContent || '';
        return !text.includes('tool_use_id') && !text.includes('citations:');
      },
      severity: 'error',
    },
    {
      id: 'visual-hierarchy',
      name: 'Visual Hierarchy',
      description: 'Delegation should be visually distinct',
      selector: '.delegation-container',
      check: (element) => {
        if (!element) return false;
        const style = window.getComputedStyle(element);
        // Check for visual distinction (border, background, etc.)
        return style.borderLeftWidth !== '0px' || 
               style.backgroundColor !== 'transparent' ||
               style.borderWidth !== '0px';
      },
      severity: 'warning',
    },
  ];

  // Think tool validation rules
  const thinkRules: ValidationRule[] = [
    {
      id: 'thought-bubble',
      name: 'Thought Bubble Display',
      description: 'Think tools should render as thought bubbles',
      selector: '.thought-bubble',
      check: (element) => !!element,
      severity: 'error',
    },
    {
      id: 'no-tool-ui',
      name: 'No Tool UI',
      description: 'Think tools should not show tool call interface',
      selector: '.tool-call-ui',
      check: (element) => !element,
      severity: 'error',
    },
    {
      id: 'thought-styling',
      name: 'Thought Styling',
      description: 'Thoughts should have distinct visual style',
      selector: '.thought-bubble',
      check: (element) => {
        if (!element) return false;
        const style = window.getComputedStyle(element);
        // Check for distinct styling (italic, different color, etc.)
        return style.fontStyle === 'italic' || 
               style.opacity !== '1' ||
               style.color !== window.getComputedStyle(document.body).color;
      },
      severity: 'warning',
    },
    {
      id: 'thought-icon',
      name: 'Thought Icon',
      description: 'Should display a thought/brain icon',
      selector: '.thought-icon',
      check: (element) => !!element,
      severity: 'info',
    },
    {
      id: 'thought-content-preserved',
      name: 'Content Preservation',
      description: 'Thought content should be preserved with formatting',
      check: (element, message) => {
        if (!element) return false;
        const thoughtContent = element.querySelector('.thought-content');
        if (!thoughtContent) return false;
        
        // Check if line breaks and formatting are preserved
        const html = thoughtContent.innerHTML;
        return html.includes('<br') || html.includes('<p') || html.includes('\n');
      },
      severity: 'warning',
    },
  ];

  /**
   * Run validation
   */
  const runValidation = useCallback(() => {
    setIsValidating(true);
    const results = new Map<string, ValidationResult[]>();
    
    messages.forEach((message, index) => {
      const messageId = `msg-${index}`;
      const messageResults: ValidationResult[] = [];
      
      // Get the message element
      const messageElement = document.querySelector(`[data-message-id="${message.id}"]`) ||
                           document.querySelector(`[data-message-index="${index}"]`);
      
      // Determine tool type
      const toolType = getToolType(message);
      
      if (toolType === 'delegation') {
        // Run delegation validation rules
        delegationRules.forEach(rule => {
          const element = rule.selector ? messageElement?.querySelector(rule.selector) : messageElement;
          const passed = rule.check(element || null, message);
          
          messageResults.push({
            ruleId: rule.id,
            ruleName: rule.name,
            passed,
            message: passed ? `✓ ${rule.description}` : `✗ ${rule.description}`,
            severity: rule.severity,
            element,
            details: { messageIndex: index, toolType },
          });

          // Highlight failing elements
          if (!passed && element && showVisualGuides) {
            highlightElement(element, passed);
          }
        });
      } else if (toolType === 'think') {
        // Run think validation rules
        thinkRules.forEach(rule => {
          const element = rule.selector ? messageElement?.querySelector(rule.selector) : messageElement;
          const passed = rule.check(element || null, message);
          
          messageResults.push({
            ruleId: rule.id,
            ruleName: rule.name,
            passed,
            message: passed ? `✓ ${rule.description}` : `✗ ${rule.description}`,
            severity: rule.severity,
            element,
            details: { messageIndex: index, toolType },
          });

          // Highlight failing elements
          if (!passed && element && showVisualGuides) {
            highlightElement(element, passed);
          }
        });
      }
      
      if (messageResults.length > 0) {
        results.set(messageId, messageResults);
      }
    });
    
    setValidationResults(results);
    setIsValidating(false);
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [messages, showVisualGuides]);

  /**
   * Get tool type from message
   */
  const getToolType = (message: any): ToolType => {
    if (!message.content) return 'other';
    
    const content = Array.isArray(message.content) ? message.content : [message.content];
    
    for (const item of content) {
      if (item.type === 'tool_use') {
        if (item.name === 'act_oneshot' || item.name === 'act_chat') {
          return 'delegation';
        } else if (item.name === 'think') {
          return 'think';
        }
      }
    }
    
    return 'other';
  };

  /**
   * Highlight element for visual feedback
   */
  const highlightElement = (element: Element, passed: boolean) => {
    element.classList.add(passed ? 'validation-pass' : 'validation-fail');
    setHighlightedElements(prev => {
      const newSet = new Set(prev);
      newSet.add(element);
      return newSet;
    });
    
    // Remove highlight after 3 seconds
    setTimeout(() => {
      element.classList.remove('validation-pass', 'validation-fail');
      setHighlightedElements(prev => {
        const next = new Set(prev);
        next.delete(element);
        return next;
      });
    }, 3000);
  };

  /**
   * Clear highlights
   */
  const clearHighlights = () => {
    highlightedElements.forEach(element => {
      element.classList.remove('validation-pass', 'validation-fail');
    });
    setHighlightedElements(new Set());
  };

  /**
   * Setup DOM observer for auto-validation
   */
  useEffect(() => {
    if (!autoRun) return;

    // Create observer to watch for DOM changes
    observerRef.current = new MutationObserver(() => {
      // Debounce validation
      setTimeout(() => runValidation(), 500);
    });

    // Start observing
    const chatContainer = document.querySelector('.chat-messages-container');
    if (chatContainer) {
      observerRef.current.observe(chatContainer, {
        childList: true,
        subtree: true,
        attributes: true,
        attributeFilter: ['class', 'data-message-id'],
      });
    }

    return () => {
      observerRef.current?.disconnect();
    };
  }, [messages, autoRun, runValidation]);

  /**
   * Run validation when messages change
   */
  useEffect(() => {
    if (autoRun && messages.length > 0) {
      runValidation();
    }
  }, [messages, autoRun, runValidation]);

  // Calculate stats
  const totalResults = Array.from(validationResults.values()).flat();
  const passedCount = totalResults.filter(r => r.passed).length;
  const failedCount = totalResults.filter(r => !r.passed).length;
  const errorCount = totalResults.filter(r => !r.passed && r.severity === 'error').length;
  const warningCount = totalResults.filter(r => !r.passed && r.severity === 'warning').length;

  return (
    <TooltipProvider>
      <Card className={cn("w-full", className)}>
        <CardHeader>
          <div className="flex items-center justify-between">
            <div>
              <CardTitle className="text-lg flex items-center gap-2">
                <Layers className="h-5 w-5" />
                Tool Rendering Validator
              </CardTitle>
              <CardDescription className="text-sm mt-1">
                Validates delegation and think tool visual rendering
              </CardDescription>
            </div>
            
            <div className="flex items-center gap-2">
              {showVisualGuides && (
                <Badge variant="secondary" className="gap-1">
                  <Eye className="h-3 w-3" />
                  Visual Guides
                </Badge>
              )}
              
              <Button
                variant="outline"
                size="sm"
                onClick={runValidation}
                disabled={isValidating}
              >
                {isValidating ? (
                  <RefreshCw className="h-4 w-4 animate-spin" />
                ) : (
                  <RefreshCw className="h-4 w-4" />
                )}
                Validate
              </Button>
            </div>
          </div>
        </CardHeader>

        <CardContent className="space-y-4">
          {/* Summary Stats */}
          <div className="grid grid-cols-4 gap-2">
            <StatCard
              icon={<CheckCircle2 className="h-4 w-4" />}
              label="Passed"
              value={passedCount}
              color="text-green-600 dark:text-green-400"
            />
            <StatCard
              icon={<XCircle className="h-4 w-4" />}
              label="Failed"
              value={failedCount}
              color="text-red-600 dark:text-red-400"
            />
            <StatCard
              icon={<AlertTriangle className="h-4 w-4" />}
              label="Errors"
              value={errorCount}
              color="text-orange-600 dark:text-orange-400"
            />
            <StatCard
              icon={<Info className="h-4 w-4" />}
              label="Warnings"
              value={warningCount}
              color="text-yellow-600 dark:text-yellow-400"
            />
          </div>

          <Separator />

          {/* Validation Results */}
          <div className="space-y-3">
            <div className="flex items-center justify-between">
              <h3 className="text-sm font-medium">Validation Results</h3>
              <Button
                variant="ghost"
                size="sm"
                onClick={() => setShowDetails(!showDetails)}
              >
                {showDetails ? <EyeOff className="h-4 w-4" /> : <Eye className="h-4 w-4" />}
                {showDetails ? 'Hide' : 'Show'} Details
              </Button>
            </div>

            <ScrollArea className="h-[300px] pr-4">
              {validationResults.size === 0 ? (
                <Alert>
                  <Info className="h-4 w-4" />
                  <AlertDescription>
                    No validation results yet. Load a test scenario with delegation or think tools.
                  </AlertDescription>
                </Alert>
              ) : (
                <div className="space-y-3">
                  {Array.from(validationResults.entries()).map(([messageId, results]) => (
                    <MessageValidation
                      key={messageId}
                      messageId={messageId}
                      results={results}
                      showDetails={showDetails}
                    />
                  ))}
                </div>
              )}
            </ScrollArea>
          </div>

          {/* Visual Guide Legend */}
          {showVisualGuides && highlightedElements.size > 0 && (
            <>
              <Separator />
              <Alert>
                <Info className="h-4 w-4" />
                <AlertDescription className="text-xs">
                  <span className="font-medium">Visual Guides Active:</span> Failed elements are highlighted in red, passed elements in green.
                  Highlights clear automatically after 3 seconds.
                  <Button
                    variant="link"
                    size="sm"
                    className="ml-2 h-auto p-0"
                    onClick={clearHighlights}
                  >
                    Clear Now
                  </Button>
                </AlertDescription>
              </Alert>
            </>
          )}
        </CardContent>
      </Card>
    </TooltipProvider>
  );
}

/**
 * Stat Card Component
 */
function StatCard({
  icon,
  label,
  value,
  color,
}: {
  icon: React.ReactNode;
  label: string;
  value: number;
  color: string;
}) {
  return (
    <div className="flex items-center gap-2 p-2 rounded-lg border bg-card">
      <div className={cn("flex-shrink-0", color)}>{icon}</div>
      <div>
        <p className="text-xs text-muted-foreground">{label}</p>
        <p className="text-lg font-semibold">{value}</p>
      </div>
    </div>
  );
}

/**
 * Message Validation Component
 */
function MessageValidation({
  messageId,
  results,
  showDetails,
}: {
  messageId: string;
  results: ValidationResult[];
  showDetails: boolean;
}) {
  const toolType = results[0]?.details?.toolType || 'unknown';
  const hasErrors = results.some(r => !r.passed && r.severity === 'error');
  const hasWarnings = results.some(r => !r.passed && r.severity === 'warning');
  const allPassed = results.every(r => r.passed);

  return (
    <Card className={cn(
      "transition-colors",
      hasErrors && "border-red-500/50",
      !hasErrors && hasWarnings && "border-yellow-500/50",
      allPassed && "border-green-500/50"
    )}>
      <CardHeader className="pb-3 pt-3">
        <div className="flex items-center gap-2">
          {toolType === 'delegation' ? (
            <GitBranch className="h-4 w-4 text-primary" />
          ) : toolType === 'think' ? (
            <Brain className="h-4 w-4 text-primary" />
          ) : (
            <MessageSquare className="h-4 w-4 text-muted-foreground" />
          )}
          
          <span className="text-sm font-medium capitalize">{toolType} Tool</span>
          
          <div className="ml-auto flex items-center gap-1">
            {allPassed ? (
              <Badge variant="secondary" className="gap-1">
                <CheckCircle2 className="h-3 w-3" />
                All Passed
              </Badge>
            ) : (
              <>
                {hasErrors && (
                  <Badge variant="destructive" className="gap-1">
                    {results.filter(r => !r.passed && r.severity === 'error').length} Errors
                  </Badge>
                )}
                {hasWarnings && (
                  <Badge variant="secondary" className="gap-1">
                    {results.filter(r => !r.passed && r.severity === 'warning').length} Warnings
                  </Badge>
                )}
              </>
            )}
          </div>
        </div>
      </CardHeader>
      
      {showDetails && (
        <CardContent className="pt-0 pb-3">
          <div className="space-y-1">
            {results.map((result, idx) => (
              <div
                key={idx}
                className={cn(
                  "flex items-start gap-2 text-xs py-1",
                  !result.passed && "text-red-600 dark:text-red-400"
                )}
              >
                {result.passed ? (
                  <CheckCircle2 className="h-3 w-3 mt-0.5 text-green-600 dark:text-green-400" />
                ) : result.severity === 'error' ? (
                  <XCircle className="h-3 w-3 mt-0.5" />
                ) : result.severity === 'warning' ? (
                  <AlertTriangle className="h-3 w-3 mt-0.5 text-yellow-600 dark:text-yellow-400" />
                ) : (
                  <Info className="h-3 w-3 mt-0.5 text-blue-600 dark:text-blue-400" />
                )}
                <div className="flex-1">
                  <span className="font-medium">{result.ruleName}:</span> {result.message}
                </div>
              </div>
            ))}
          </div>
        </CardContent>
      )}
    </Card>
  );
}

// Add global styles for validation highlights
if (typeof document !== 'undefined') {
  const style = document.createElement('style');
  style.textContent = `
    .validation-pass {
      outline: 2px solid rgb(34, 197, 94) !important;
      outline-offset: 2px;
      animation: validation-pulse-pass 0.5s ease-out;
    }
    
    .validation-fail {
      outline: 2px solid rgb(239, 68, 68) !important;
      outline-offset: 2px;
      animation: validation-pulse-fail 0.5s ease-out;
    }
    
    @keyframes validation-pulse-pass {
      0% {
        box-shadow: 0 0 0 0 rgba(34, 197, 94, 0.7);
      }
      70% {
        box-shadow: 0 0 0 10px rgba(34, 197, 94, 0);
      }
      100% {
        box-shadow: 0 0 0 0 rgba(34, 197, 94, 0);
      }
    }
    
    @keyframes validation-pulse-fail {
      0% {
        box-shadow: 0 0 0 0 rgba(239, 68, 68, 0.7);
      }
      70% {
        box-shadow: 0 0 0 10px rgba(239, 68, 68, 0);
      }
      100% {
        box-shadow: 0 0 0 0 rgba(239, 68, 68, 0);
      }
    }
  `;
  document.head.appendChild(style);
}