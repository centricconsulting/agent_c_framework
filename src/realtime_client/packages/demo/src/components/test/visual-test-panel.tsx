/**
 * Visual Test Panel Component
 * Enhanced test controls with validation features for tool rendering
 */

'use client';

import React, { useState, useEffect } from 'react';
import { useTestSession } from '@/hooks/use-test-session';
import { useChat } from '@agentc/realtime-react';
import { 
  Button,
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
  Tabs,
  TabsContent,
  TabsList,
  TabsTrigger,
  Badge,
  ScrollArea,
  Separator,
  Label,
  Alert,
  AlertDescription,
  Tooltip,
  TooltipContent,
  TooltipProvider,
  TooltipTrigger,
} from '@agentc/realtime-ui';
import { cn } from '@/lib/utils';
import { 
  FlaskConical,
  ChevronRight,
  ChevronDown,
  CheckCircle2,
  XCircle,
  AlertTriangle,
  Info,
  Eye,
  EyeOff,
  Code2,
  MessageSquare,
  GitBranch,
  Brain,
  Activity,
  Bug,
  Maximize2,
  Minimize2,
  RefreshCw,
  Download,
  Upload,
  Clock,
  MemoryStick,
  Zap,
  Layers,
  FileJson,
  Database,
} from 'lucide-react';

/**
 * Test validation status
 */
type ValidationStatus = 'pass' | 'fail' | 'warning' | 'pending';

/**
 * Tool validation result
 */
interface ToolValidation {
  toolType: 'delegation' | 'think' | 'other';
  status: ValidationStatus;
  message: string;
  details?: any;
}

/**
 * Performance metrics
 */
interface PerformanceMetrics {
  renderTime: number;
  messageCount: number;
  memoryUsage?: number;
  scrollPerformance: 'smooth' | 'janky' | 'unknown';
}

/**
 * Visual Test Panel Props
 */
export interface VisualTestPanelProps {
  className?: string;
  defaultExpanded?: boolean;
  position?: 'bottom' | 'right' | 'floating';
}

/**
 * Enhanced Visual Test Panel Component
 */
export function VisualTestPanel({ 
  className = '', 
  defaultExpanded = false,
  position = 'bottom' 
}: VisualTestPanelProps) {
  const { 
    testModeEnabled,
    currentScenario,
    scenarios,
    loadScenario,
    clearSession,
    reloadCurrentScenario,
    isLoading,
    error 
  } = useTestSession();
  
  const { messages } = useChat();
  
  const [isExpanded, setIsExpanded] = useState(defaultExpanded);
  const [activeTab, setActiveTab] = useState('controls');
  const [showDebugInfo, setShowDebugInfo] = useState(false);
  const [showRawData, setShowRawData] = useState(false);
  const [autoValidate, setAutoValidate] = useState(true);
  const [showValidationDetails, setShowValidationDetails] = useState(false);
  const [validations, setValidations] = useState<ToolValidation[]>([]);
  const [metrics, setMetrics] = useState<PerformanceMetrics>({
    renderTime: 0,
    messageCount: 0,
    scrollPerformance: 'unknown',
  });
  const [isFullscreen, setIsFullscreen] = useState(false);

  /**
   * Validate tool rendering
   */
  useEffect(() => {
    if (!autoValidate || !messages.length) return;

    const startTime = performance.now();
    const newValidations: ToolValidation[] = [];

    messages.forEach((message) => {
      // Skip divider messages
      if ('type' in message && message.type === 'divider') {
        return;
      }
      
      // Check for delegation tools (act_oneshot, act_chat)
      if ('content' in message && message.content && typeof message.content === 'object') {
        const content = Array.isArray(message.content) ? message.content : [message.content];
        
        content.forEach((item: any) => {
          if (item.type === 'tool_use') {
            if (item.name === 'act_oneshot' || item.name === 'act_chat') {
              // Delegation tool validation
              const hasDivider = checkSubsessionDivider(message);
              newValidations.push({
                toolType: 'delegation',
                status: hasDivider ? 'pass' : 'fail',
                message: `Delegation tool ${item.name} ${hasDivider ? 'has' : 'missing'} subsession divider`,
                details: { toolId: item.id, name: item.name }
              });
            } else if (item.name === 'think') {
              // Think tool validation
              const isThoughtBubble = checkThoughtBubbleRendering(message);
              const hasNoToolUI = !checkToolUIVisible(message);
              
              newValidations.push({
                toolType: 'think',
                status: isThoughtBubble && hasNoToolUI ? 'pass' : 'fail',
                message: `Think tool ${isThoughtBubble ? 'rendered as thought bubble' : 'not rendered correctly'}`,
                details: { 
                  toolId: item.id,
                  thoughtBubble: isThoughtBubble,
                  noToolUI: hasNoToolUI 
                }
              });
            }
          }
        });
      }
    });

    const endTime = performance.now();
    
    setValidations(newValidations);
    setMetrics({
      renderTime: endTime - startTime,
      messageCount: messages.length,
      memoryUsage: (performance as any).memory?.usedJSHeapSize,
      scrollPerformance: detectScrollPerformance(),
    });
  }, [messages, autoValidate]);

  /**
   * Check if subsession divider is present
   */
  const checkSubsessionDivider = (message: any): boolean => {
    // This would check the actual DOM or component state
    // For now, we'll simulate the check
    const messageId = 'id' in message ? message.id : message.messageId;
    const dividerElement = document.querySelector(`[data-message-id="${messageId}"] .subsession-divider`);
    return !!dividerElement;
  };

  /**
   * Check if thought bubble rendering is correct
   */
  const checkThoughtBubbleRendering = (message: any): boolean => {
    const messageId = 'id' in message ? message.id : message.messageId;
    const thoughtElement = document.querySelector(`[data-message-id="${messageId}"] .thought-bubble`);
    return !!thoughtElement;
  };

  /**
   * Check if tool UI is visible
   */
  const checkToolUIVisible = (message: any): boolean => {
    const messageId = 'id' in message ? message.id : message.messageId;
    const toolUIElement = document.querySelector(`[data-message-id="${messageId}"] .tool-call-ui`);
    return !!toolUIElement;
  };

  /**
   * Detect scroll performance
   */
  const detectScrollPerformance = (): 'smooth' | 'janky' | 'unknown' => {
    // Simple heuristic based on FPS
    const fps = (performance as any).getEntriesByType?.('measure')
      ?.filter((entry: any) => entry.name === 'scroll-fps')
      ?.pop()?.duration;
    
    if (!fps) return 'unknown';
    return fps >= 55 ? 'smooth' : 'janky';
  };

  /**
   * Export test results
   */
  const exportResults = () => {
    const results = {
      timestamp: new Date().toISOString(),
      scenario: currentScenario?.name,
      validations,
      metrics,
      messages: showRawData ? messages : undefined,
    };
    
    const blob = new Blob([JSON.stringify(results, null, 2)], { type: 'application/json' });
    const url = URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = `test-results-${Date.now()}.json`;
    a.click();
    URL.revokeObjectURL(url);
  };

  // Panel positioning classes
  const panelClasses = cn(
    'bg-background border shadow-xl transition-all duration-300',
    {
      'fixed bottom-0 left-0 right-0 z-50 rounded-t-lg': position === 'bottom',
      'fixed right-0 top-0 bottom-0 z-50 rounded-l-lg': position === 'right',
      'fixed bottom-4 right-4 z-50 rounded-lg': position === 'floating',
      'h-[400px]': position === 'bottom' && isExpanded && !isFullscreen,
      'h-[60px]': position === 'bottom' && !isExpanded,
      'h-screen': isFullscreen,
      'w-[480px]': position === 'right' && isExpanded,
      'w-[60px]': position === 'right' && !isExpanded,
      'max-w-lg': position === 'floating',
    },
    className
  );

  if (!testModeEnabled) {
    return null;
  }

  return (
    <TooltipProvider>
      <div className={panelClasses}>
        {/* Header */}
        <div className="flex items-center justify-between px-4 py-2 border-b bg-muted/40">
          <div className="flex items-center gap-2">
            <Button
              variant="ghost"
              size="icon"
              className="h-8 w-8"
              onClick={() => setIsExpanded(!isExpanded)}
            >
              {isExpanded ? (
                <ChevronDown className="h-4 w-4" />
              ) : (
                <ChevronRight className="h-4 w-4" />
              )}
            </Button>
            
            <div className="flex items-center gap-2">
              <FlaskConical className="h-5 w-5 text-primary" />
              <span className="font-semibold text-sm">Visual Test Panel</span>
            </div>

            {currentScenario && (
              <Badge variant="secondary" className="ml-2">
                {currentScenario.name}
              </Badge>
            )}
          </div>

          <div className="flex items-center gap-1">
            {/* Status Indicators */}
            <div className="flex items-center gap-2 mr-2">
              {validations.length > 0 && (
                <>
                  <StatusIndicator 
                    status={validations.filter(v => v.status === 'pass').length} 
                    type="pass"
                    total={validations.length}
                  />
                  <StatusIndicator 
                    status={validations.filter(v => v.status === 'fail').length} 
                    type="fail"
                    total={validations.length}
                  />
                </>
              )}
            </div>

            {/* Action Buttons */}
            <Tooltip>
              <TooltipTrigger asChild>
                <Button
                  variant="ghost"
                  size="icon"
                  className="h-8 w-8"
                  onClick={reloadCurrentScenario}
                  disabled={!currentScenario || isLoading}
                >
                  <RefreshCw className={cn("h-4 w-4", isLoading && "animate-spin")} />
                </Button>
              </TooltipTrigger>
              <TooltipContent>Reload Scenario</TooltipContent>
            </Tooltip>

            <Tooltip>
              <TooltipTrigger asChild>
                <Button
                  variant="ghost"
                  size="icon"
                  className="h-8 w-8"
                  onClick={exportResults}
                >
                  <Download className="h-4 w-4" />
                </Button>
              </TooltipTrigger>
              <TooltipContent>Export Results</TooltipContent>
            </Tooltip>

            <Tooltip>
              <TooltipTrigger asChild>
                <Button
                  variant="ghost"
                  size="icon"
                  className="h-8 w-8"
                  onClick={() => setIsFullscreen(!isFullscreen)}
                >
                  {isFullscreen ? (
                    <Minimize2 className="h-4 w-4" />
                  ) : (
                    <Maximize2 className="h-4 w-4" />
                  )}
                </Button>
              </TooltipTrigger>
              <TooltipContent>{isFullscreen ? 'Exit Fullscreen' : 'Fullscreen'}</TooltipContent>
            </Tooltip>
          </div>
        </div>

        {/* Content */}
        {isExpanded && (
          <Tabs value={activeTab} onValueChange={setActiveTab} className="flex-1 flex flex-col">
            <TabsList className="grid w-full grid-cols-4 px-4 pt-2">
              <TabsTrigger value="controls" className="text-xs">
                <FlaskConical className="h-3 w-3 mr-1" />
                Controls
              </TabsTrigger>
              <TabsTrigger value="validation" className="text-xs">
                <CheckCircle2 className="h-3 w-3 mr-1" />
                Validation
              </TabsTrigger>
              <TabsTrigger value="debug" className="text-xs">
                <Bug className="h-3 w-3 mr-1" />
                Debug
              </TabsTrigger>
              <TabsTrigger value="metrics" className="text-xs">
                <Activity className="h-3 w-3 mr-1" />
                Metrics
              </TabsTrigger>
            </TabsList>

            <ScrollArea className="flex-1">
              <TabsContent value="controls" className="px-4 pb-4">
                <TestControlsSection 
                  scenarios={scenarios}
                  currentScenario={currentScenario}
                  onLoadScenario={loadScenario}
                  onClearSession={clearSession}
                  isLoading={isLoading}
                  error={error}
                />
              </TabsContent>

              <TabsContent value="validation" className="px-4 pb-4">
                <ValidationSection 
                  validations={validations}
                  autoValidate={autoValidate}
                  onAutoValidateChange={setAutoValidate}
                />
              </TabsContent>

              <TabsContent value="debug" className="px-4 pb-4">
                <DebugSection 
                  messages={messages}
                  showDebugInfo={showDebugInfo}
                  showRawData={showRawData}
                  onShowDebugInfoChange={setShowDebugInfo}
                  onShowRawDataChange={setShowRawData}
                />
              </TabsContent>

              <TabsContent value="metrics" className="px-4 pb-4">
                <MetricsSection metrics={metrics} />
              </TabsContent>
            </ScrollArea>
          </Tabs>
        )}
      </div>
    </TooltipProvider>
  );
}

/**
 * Status Indicator Component
 */
function StatusIndicator({ 
  status, 
  type, 
  total 
}: { 
  status: number; 
  type: 'pass' | 'fail'; 
  total: number;
}) {
  const percentage = total > 0 ? Math.round((status / total) * 100) : 0;
  
  return (
    <Tooltip>
      <TooltipTrigger asChild>
        <div className={cn(
          "flex items-center gap-1 px-2 py-1 rounded-md text-xs font-medium",
          type === 'pass' ? 'bg-green-100 text-green-800 dark:bg-green-900/20 dark:text-green-400' :
          'bg-red-100 text-red-800 dark:bg-red-900/20 dark:text-red-400'
        )}>
          {type === 'pass' ? (
            <CheckCircle2 className="h-3 w-3" />
          ) : (
            <XCircle className="h-3 w-3" />
          )}
          {status}
        </div>
      </TooltipTrigger>
      <TooltipContent>
        {percentage}% {type === 'pass' ? 'passing' : 'failing'}
      </TooltipContent>
    </Tooltip>
  );
}

/**
 * Test Controls Section
 */
function TestControlsSection({
  scenarios,
  currentScenario,
  onLoadScenario,
  onClearSession,
  isLoading,
  error,
}: any) {
  const [selectedScenarioId, setSelectedScenarioId] = useState('');

  return (
    <div className="space-y-4">
      {/* Current Scenario */}
      {currentScenario && (
        <Card>
          <CardHeader className="pb-3">
            <CardTitle className="text-sm">Active Scenario</CardTitle>
          </CardHeader>
          <CardContent>
            <div className="space-y-2">
              <div className="flex items-center justify-between">
                <span className="text-sm font-medium">{currentScenario.name}</span>
                <Badge variant="secondary">
                  <CheckCircle2 className="h-3 w-3 mr-1" />
                  Loaded
                </Badge>
              </div>
              {currentScenario.description && (
                <p className="text-xs text-muted-foreground">
                  {currentScenario.description}
                </p>
              )}
            </div>
          </CardContent>
        </Card>
      )}

      {/* Scenario Selection */}
      <div className="space-y-2">
        <Label className="text-sm font-medium">Load Test Scenario</Label>
        <div className="grid grid-cols-2 gap-2">
          {scenarios.map((scenario: any) => (
            <Button
              key={scenario.id}
              variant={selectedScenarioId === scenario.id ? 'default' : 'outline'}
              size="sm"
              className="justify-start"
              onClick={() => {
                setSelectedScenarioId(scenario.id);
                onLoadScenario(scenario.id);
              }}
              disabled={isLoading}
            >
              <FileJson className="h-3 w-3 mr-2" />
              {scenario.name}
            </Button>
          ))}
        </div>
      </div>

      {/* Actions */}
      <div className="flex gap-2">
        <Button
          variant="destructive"
          size="sm"
          onClick={onClearSession}
          disabled={!currentScenario || isLoading}
          className="flex-1"
        >
          Clear Session
        </Button>
      </div>

      {/* Error Alert */}
      {error && (
        <Alert variant="destructive">
          <AlertTriangle className="h-4 w-4" />
          <AlertDescription>{error}</AlertDescription>
        </Alert>
      )}
    </div>
  );
}

/**
 * Validation Section
 */
function ValidationSection({
  validations,
  autoValidate,
  onAutoValidateChange,
}: any) {
  const delegationValidations = validations.filter((v: any) => v.toolType === 'delegation');
  const thinkValidations = validations.filter((v: any) => v.toolType === 'think');

  return (
    <div className="space-y-4">
      {/* Auto-validate Toggle */}
      <div className="flex items-center justify-between">
        <Label htmlFor="auto-validate" className="text-sm font-medium">
          Auto-validate on Changes
        </Label>
        <Button
          id="auto-validate"
          variant={autoValidate ? "default" : "outline"}
          size="sm"
          onClick={() => onAutoValidateChange(!autoValidate)}
        >
          {autoValidate ? 'Enabled' : 'Disabled'}
        </Button>
      </div>

      <Separator />

      {/* Delegation Tool Validations */}
      <div className="space-y-2">
        <div className="flex items-center gap-2">
          <GitBranch className="h-4 w-4 text-primary" />
          <h4 className="text-sm font-medium">Delegation Tools</h4>
          {delegationValidations.length > 0 && (
            <Badge variant="outline" className="ml-auto">
              {delegationValidations.filter((v: any) => v.status === 'pass').length}/{delegationValidations.length}
            </Badge>
          )}
        </div>
        
        <div className="space-y-1">
          {delegationValidations.length === 0 ? (
            <p className="text-xs text-muted-foreground">No delegation tools found</p>
          ) : (
            delegationValidations.map((validation: any, idx: number) => (
              <ValidationItem key={idx} validation={validation} />
            ))
          )}
        </div>
      </div>

      <Separator />

      {/* Think Tool Validations */}
      <div className="space-y-2">
        <div className="flex items-center gap-2">
          <Brain className="h-4 w-4 text-primary" />
          <h4 className="text-sm font-medium">Think Tools</h4>
          {thinkValidations.length > 0 && (
            <Badge variant="outline" className="ml-auto">
              {thinkValidations.filter((v: any) => v.status === 'pass').length}/{thinkValidations.length}
            </Badge>
          )}
        </div>
        
        <div className="space-y-1">
          {thinkValidations.length === 0 ? (
            <p className="text-xs text-muted-foreground">No think tools found</p>
          ) : (
            thinkValidations.map((validation: any, idx: number) => (
              <ValidationItem key={idx} validation={validation} />
            ))
          )}
        </div>
      </div>
    </div>
  );
}

/**
 * Validation Item Component
 */
function ValidationItem({ validation }: { validation: ToolValidation }) {
  const [showDetails, setShowDetails] = useState(false);

  return (
    <div className="space-y-1">
      <button
        onClick={() => setShowDetails(!showDetails)}
        className="flex items-center gap-2 w-full p-2 rounded-md hover:bg-muted/50 text-left transition-colors"
      >
        {validation.status === 'pass' ? (
          <CheckCircle2 className="h-4 w-4 text-green-500" />
        ) : validation.status === 'fail' ? (
          <XCircle className="h-4 w-4 text-red-500" />
        ) : (
          <AlertTriangle className="h-4 w-4 text-yellow-500" />
        )}
        <span className="text-xs flex-1">{validation.message}</span>
        <ChevronRight className={cn(
          "h-3 w-3 transition-transform",
          showDetails && "rotate-90"
        )} />
      </button>
      {showDetails && validation.details && (
        <pre className="text-xs bg-muted p-2 rounded-md ml-6 overflow-auto">
          {JSON.stringify(validation.details, null, 2)}
        </pre>
      )}
    </div>
  );
}

/**
 * Debug Section
 */
function DebugSection({
  messages,
  showDebugInfo,
  showRawData,
  onShowDebugInfoChange,
  onShowRawDataChange,
}: any) {
  return (
    <div className="space-y-4">
      {/* Debug Toggles */}
      <div className="space-y-2">
        <div className="flex items-center justify-between">
          <Label htmlFor="show-debug" className="text-sm">
            Show Message Metadata
          </Label>
          <Button
            id="show-debug"
            variant={showDebugInfo ? "default" : "outline"}
            size="sm"
            onClick={() => onShowDebugInfoChange(!showDebugInfo)}
          >
            {showDebugInfo ? 'Show' : 'Hide'}
          </Button>
        </div>

        <div className="flex items-center justify-between">
          <Label htmlFor="show-raw" className="text-sm">
            Show Raw Message Data
          </Label>
          <Button
            id="show-raw"
            variant={showRawData ? "default" : "outline"}
            size="sm"
            onClick={() => onShowRawDataChange(!showRawData)}
          >
            {showRawData ? 'Show' : 'Hide'}
          </Button>
        </div>
      </div>

      <Separator />

      {/* Message Info */}
      <div className="space-y-2">
        <div className="flex items-center gap-2">
          <MessageSquare className="h-4 w-4 text-primary" />
          <h4 className="text-sm font-medium">Messages</h4>
          <Badge variant="outline" className="ml-auto">
            {messages.length}
          </Badge>
        </div>

        {showRawData && messages.length > 0 && (
          <Card>
            <CardContent className="p-2">
              <ScrollArea className="h-[200px]">
                <pre className="text-xs">
                  {JSON.stringify(messages, null, 2)}
                </pre>
              </ScrollArea>
            </CardContent>
          </Card>
        )}
      </div>
    </div>
  );
}

/**
 * Metrics Section
 */
function MetricsSection({ metrics }: { metrics: PerformanceMetrics }) {
  return (
    <div className="space-y-4">
      {/* Performance Metrics */}
      <div className="grid grid-cols-2 gap-3">
        <MetricCard
          icon={<Clock className="h-4 w-4" />}
          label="Render Time"
          value={`${metrics.renderTime.toFixed(2)}ms`}
          status={metrics.renderTime < 16 ? 'good' : metrics.renderTime < 50 ? 'warning' : 'bad'}
        />
        
        <MetricCard
          icon={<Layers className="h-4 w-4" />}
          label="Messages"
          value={metrics.messageCount}
          status="neutral"
        />

        {metrics.memoryUsage && (
          <MetricCard
            icon={<MemoryStick className="h-4 w-4" />}
            label="Memory"
            value={`${(metrics.memoryUsage / 1024 / 1024).toFixed(1)}MB`}
            status={metrics.memoryUsage < 50000000 ? 'good' : 'warning'}
          />
        )}

        <MetricCard
          icon={<Zap className="h-4 w-4" />}
          label="Scroll"
          value={metrics.scrollPerformance}
          status={metrics.scrollPerformance === 'smooth' ? 'good' : 
                 metrics.scrollPerformance === 'janky' ? 'bad' : 'neutral'}
        />
      </div>

      {/* Performance Tips */}
      <Card>
        <CardHeader className="pb-2">
          <CardTitle className="text-xs flex items-center gap-1">
            <Info className="h-3 w-3" />
            Performance Tips
          </CardTitle>
        </CardHeader>
        <CardContent>
          <ul className="text-xs text-muted-foreground space-y-1">
            <li>• Keep render time below 16ms for 60fps</li>
            <li>• Monitor memory usage for large sessions</li>
            <li>• Use virtualization for long message lists</li>
            <li>• Debounce rapid state updates</li>
          </ul>
        </CardContent>
      </Card>
    </div>
  );
}

/**
 * Metric Card Component
 */
function MetricCard({
  icon,
  label,
  value,
  status,
}: {
  icon: React.ReactNode;
  label: string;
  value: any;
  status: 'good' | 'warning' | 'bad' | 'neutral';
}) {
  return (
    <Card>
      <CardContent className="p-3">
        <div className="flex items-center gap-2">
          <div className={cn(
            "p-1.5 rounded-md",
            status === 'good' && "bg-green-100 text-green-700 dark:bg-green-900/20 dark:text-green-400",
            status === 'warning' && "bg-yellow-100 text-yellow-700 dark:bg-yellow-900/20 dark:text-yellow-400",
            status === 'bad' && "bg-red-100 text-red-700 dark:bg-red-900/20 dark:text-red-400",
            status === 'neutral' && "bg-muted text-muted-foreground"
          )}>
            {icon}
          </div>
          <div className="flex-1">
            <p className="text-xs text-muted-foreground">{label}</p>
            <p className="text-sm font-semibold">{value}</p>
          </div>
        </div>
      </CardContent>
    </Card>
  );
}