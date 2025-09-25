/**
 * Session Switch Test Panel Component
 * 
 * Visual interface for running and monitoring session switching tests.
 * Provides clear pass/fail indicators and detailed diagnostics.
 */

import React, { useState } from 'react';
import { 
  Card, 
  CardHeader, 
  CardTitle, 
  CardContent,
  Button,
  Alert,
  AlertDescription,
  Badge,
  Tabs,
  TabsContent,
  TabsList,
  TabsTrigger,
  ScrollArea
} from '@agentc/realtime-ui';
import { 
  CheckCircle2, 
  XCircle, 
  PlayCircle, 
  RefreshCw, 
  AlertCircle,
  Activity,
  Clock,
  Hash,
  Zap,
  Package,
  Loader2,
  ChevronRight
} from 'lucide-react';
import { useSessionSwitchTest, TestScenario, TestResult, MessageTraceEntry } from '@/hooks/useSessionSwitchTest';
import { format } from 'date-fns';

/**
 * Test scenario metadata
 */
const SCENARIO_META: Record<TestScenario, {
  name: string;
  description: string;
  icon: React.ElementType;
  expectedDuration: number;
}> = {
  'rapid-switching': {
    name: 'Rapid Session Switching',
    description: 'Switch between multiple sessions quickly to verify messages clear each time',
    icon: Zap,
    expectedDuration: 3000
  },
  'empty-session': {
    name: 'Empty Session Test',
    description: 'Switch to a new/empty session and verify messages clear',
    icon: Package,
    expectedDuration: 2000
  },
  'populated-session': {
    name: 'Populated Session Test',
    description: 'Switch between sessions with messages and verify only correct messages show',
    icon: Activity,
    expectedDuration: 4000
  },
  'loading-state': {
    name: 'Loading State Test',
    description: 'Verify no messages leak through during the loading transition',
    icon: Loader2,
    expectedDuration: 2000
  }
};

/**
 * Individual test result card
 */
function TestResultCard({ result }: { result: TestResult }) {
  const [expanded, setExpanded] = useState(false);
  const meta = SCENARIO_META[result.scenario];
  const Icon = meta.icon;
  
  return (
    <Card className={`border ${result.passed ? 'border-green-500' : 'border-red-500'}`}>
      <CardHeader 
        className="cursor-pointer"
        onClick={() => setExpanded(!expanded)}
      >
        <div className="flex items-center justify-between">
          <div className="flex items-center gap-3">
            <div className={`p-2 rounded-lg ${result.passed ? 'bg-green-100' : 'bg-red-100'}`}>
              <Icon className={`h-5 w-5 ${result.passed ? 'text-green-600' : 'text-red-600'}`} />
            </div>
            <div>
              <h4 className="font-medium">{meta.name}</h4>
              <p className="text-sm text-muted-foreground">{meta.description}</p>
            </div>
          </div>
          
          <div className="flex items-center gap-2">
            {result.passed ? (
              <Badge variant="outline" className="bg-green-50 text-green-700 border-green-300">
                <CheckCircle2 className="h-3 w-3 mr-1" />
                Passed
              </Badge>
            ) : (
              <Badge variant="outline" className="bg-red-50 text-red-700 border-red-300">
                <XCircle className="h-3 w-3 mr-1" />
                Failed
              </Badge>
            )}
            <Badge variant="outline">
              <Clock className="h-3 w-3 mr-1" />
              {result.duration}ms
            </Badge>
            <ChevronRight 
              className={`h-4 w-4 transition-transform ${expanded ? 'rotate-90' : ''}`} 
            />
          </div>
        </div>
      </CardHeader>
      
      {expanded && (
        <CardContent>
          <div className="space-y-4">
            {/* Expected vs Actual */}
            <div className="grid grid-cols-2 gap-4">
              <div>
                <h5 className="text-sm font-medium text-muted-foreground mb-1">Expected Behavior</h5>
                <p className="text-sm">{result.details.expectedBehavior}</p>
              </div>
              <div>
                <h5 className="text-sm font-medium text-muted-foreground mb-1">Actual Behavior</h5>
                <p className="text-sm">{result.details.actualBehavior}</p>
              </div>
            </div>
            
            {/* Issues */}
            {result.details.issues.length > 0 && (
              <div>
                <h5 className="text-sm font-medium text-muted-foreground mb-2">Issues Detected</h5>
                <ul className="space-y-1">
                  {result.details.issues.map((issue, idx) => (
                    <li key={idx} className="text-sm text-red-600 flex items-start gap-2">
                      <AlertCircle className="h-3 w-3 mt-0.5 flex-shrink-0" />
                      <span>{issue}</span>
                    </li>
                  ))}
                </ul>
              </div>
            )}
            
            {/* Message Trace */}
            {result.messageTrace && result.messageTrace.length > 0 && (
              <div>
                <h5 className="text-sm font-medium text-muted-foreground mb-2">Message Trace</h5>
                <ScrollArea className="h-48 w-full border rounded-md p-2">
                  <div className="space-y-1">
                    {result.messageTrace.map((entry, idx) => (
                      <MessageTraceItem key={idx} entry={entry} />
                    ))}
                  </div>
                </ScrollArea>
              </div>
            )}
          </div>
        </CardContent>
      )}
    </Card>
  );
}

/**
 * Message trace item component
 */
function MessageTraceItem({ entry }: { entry: MessageTraceEntry }) {
  const getEventColor = () => {
    switch (entry.event) {
      case 'added': return 'text-blue-600';
      case 'cleared': return 'text-green-600';
      case 'leaked': return 'text-red-600';
      case 'accumulated': return 'text-orange-600';
      default: return 'text-gray-600';
    }
  };
  
  const getEventIcon = () => {
    switch (entry.event) {
      case 'added': return '+';
      case 'cleared': return '✓';
      case 'leaked': return '⚠';
      case 'accumulated': return '⊕';
      default: return '•';
    }
  };
  
  return (
    <div className="flex items-start gap-2 text-xs font-mono">
      <span className="text-muted-foreground w-16">
        {format(new Date(entry.timestamp), 'HH:mm:ss')}
      </span>
      <span className={`w-4 ${getEventColor()}`}>{getEventIcon()}</span>
      <span className="flex-1">
        <span className={getEventColor()}>{entry.event}</span>
        {' '}
        <span className="text-muted-foreground">
          {entry.sessionId ? `[${entry.sessionId.slice(0, 8)}...]` : '[null]'}
        </span>
        {' '}
        <span>{entry.messageCount} msgs</span>
        {' - '}
        <span className="text-muted-foreground">{entry.description}</span>
      </span>
    </div>
  );
}

/**
 * Test control panel
 */
function TestControls({ 
  onRunScenario, 
  onRunAll, 
  onClear,
  isRunning,
  isReady 
}: {
  onRunScenario: (scenario: TestScenario) => void;
  onRunAll: () => void;
  onClear: () => void;
  isRunning: boolean;
  isReady: boolean;
}) {
  return (
    <Card>
      <CardHeader>
        <CardTitle>Test Controls</CardTitle>
      </CardHeader>
      <CardContent className="space-y-4">
        {/* Run All Button */}
        <Button 
          onClick={onRunAll}
          disabled={!isReady || isRunning}
          className="w-full"
          size="lg"
        >
          {isRunning ? (
            <>
              <Loader2 className="mr-2 h-4 w-4 animate-spin" />
              Running Tests...
            </>
          ) : (
            <>
              <PlayCircle className="mr-2 h-4 w-4" />
              Run All Tests
            </>
          )}
        </Button>
        
        {/* Individual Scenario Buttons */}
        <div className="space-y-2">
          <p className="text-sm text-muted-foreground">Run Individual Tests:</p>
          <div className="grid grid-cols-2 gap-2">
            {(Object.keys(SCENARIO_META) as TestScenario[]).map(scenario => {
              const meta = SCENARIO_META[scenario];
              const Icon = meta.icon;
              return (
                <Button
                  key={scenario}
                  variant="outline"
                  size="sm"
                  onClick={() => onRunScenario(scenario)}
                  disabled={!isReady || isRunning}
                >
                  <Icon className="mr-1 h-3 w-3" />
                  {meta.name.split(' ')[0]}
                </Button>
              );
            })}
          </div>
        </div>
        
        {/* Clear Results */}
        <Button 
          variant="outline"
          onClick={onClear}
          disabled={isRunning}
          className="w-full"
        >
          <RefreshCw className="mr-2 h-4 w-4" />
          Clear Results
        </Button>
      </CardContent>
    </Card>
  );
}

/**
 * Summary stats component
 */
function TestSummary({ 
  summary 
}: { 
  summary: ReturnType<typeof useSessionSwitchTest>['summary'] 
}) {
  return (
    <Card>
      <CardHeader>
        <CardTitle>Test Summary</CardTitle>
      </CardHeader>
      <CardContent>
        <div className="grid grid-cols-2 gap-4">
          <div>
            <p className="text-sm text-muted-foreground">Total Tests</p>
            <p className="text-2xl font-bold">{summary.total}</p>
          </div>
          <div>
            <p className="text-sm text-muted-foreground">Success Rate</p>
            <p className="text-2xl font-bold">
              {summary.total > 0 ? `${summary.successRate.toFixed(0)}%` : '-'}
            </p>
          </div>
          <div>
            <p className="text-sm text-muted-foreground">Passed</p>
            <p className="text-2xl font-bold text-green-600">{summary.passed}</p>
          </div>
          <div>
            <p className="text-sm text-muted-foreground">Failed</p>
            <p className="text-2xl font-bold text-red-600">{summary.failed}</p>
          </div>
        </div>
        
        {summary.allPassed && summary.total > 0 && (
          <Alert className="mt-4 border-green-500 bg-green-50">
            <CheckCircle2 className="h-4 w-4 text-green-600" />
            <AlertDescription className="text-green-800">
              All tests passed! Session switching is working correctly.
            </AlertDescription>
          </Alert>
        )}
        
        {summary.failed > 0 && (
          <Alert className="mt-4 border-red-500 bg-red-50">
            <XCircle className="h-4 w-4 text-red-600" />
            <AlertDescription className="text-red-800">
              {summary.failed} test(s) failed. Check the detailed results below.
            </AlertDescription>
          </Alert>
        )}
      </CardContent>
    </Card>
  );
}

/**
 * Main Session Switch Test Panel
 */
export function SessionSwitchTestPanel() {
  const {
    isRunning,
    currentScenario,
    results,
    messageTrace,
    errors,
    summary,
    runScenario,
    runAllScenarios,
    clearResults,
    isConnected,
    isReady,
    currentSessionId,
    messageCount,
    sessionCount
  } = useSessionSwitchTest({
    rapidSwitchCount: 5,
    rapidSwitchDelay: 100,
    messageLoadTimeout: 3000,
    enableTracing: true
  });

  // State management
  const [activeTab, setActiveTab] = useState<'results' | 'trace' | 'status'>('results');

  return (
    <div className="space-y-6">
      {/* Header */}
      <div>
        <h2 className="text-2xl font-bold">Session Switching Test Suite</h2>
        <p className="text-muted-foreground">
          Validate that messages clear properly when switching between sessions
        </p>
      </div>
      
      {/* Connection Status */}
      {!isReady && (
        <Alert variant="destructive">
          <AlertCircle className="h-4 w-4" />
          <AlertDescription>
            {!isConnected 
              ? 'Not connected to server. Please connect to run tests.'
              : sessionCount < 2
              ? `Need at least 2 sessions to run tests (current: ${sessionCount})`
              : 'Test environment not ready'}
          </AlertDescription>
        </Alert>
      )}
      
      {/* Current State */}
      <Card>
        <CardContent className="pt-6">
          <div className="flex items-center justify-between">
            <div className="flex items-center gap-8">
              <div>
                <p className="text-sm text-muted-foreground">Connection</p>
                <div className="flex items-center gap-2">
                  <div className={`h-2 w-2 rounded-full ${isConnected ? 'bg-green-500' : 'bg-red-500'}`} />
                  <span className="font-medium">{isConnected ? 'Connected' : 'Disconnected'}</span>
                </div>
              </div>
              <div>
                <p className="text-sm text-muted-foreground">Current Session</p>
                <p className="font-mono text-sm">{currentSessionId?.slice(0, 12)}...</p>
              </div>
              <div>
                <p className="text-sm text-muted-foreground">Messages</p>
                <p className="font-medium">{messageCount}</p>
              </div>
              <div>
                <p className="text-sm text-muted-foreground">Available Sessions</p>
                <p className="font-medium">{sessionCount}</p>
              </div>
            </div>
            
            {isRunning && currentScenario && (
              <div className="flex items-center gap-2">
                <Loader2 className="h-4 w-4 animate-spin" />
                <span className="text-sm">Running: {SCENARIO_META[currentScenario].name}</span>
              </div>
            )}
          </div>
        </CardContent>
      </Card>
      
      {/* Main Layout */}
      <div className="grid grid-cols-3 gap-6">
        {/* Left: Controls & Summary */}
        <div className="space-y-4">
          <TestControls
            onRunScenario={runScenario}
            onRunAll={runAllScenarios}
            onClear={clearResults}
            isRunning={isRunning}
            isReady={isReady}
          />
          
          <TestSummary summary={summary} />
        </div>
        
        {/* Right: Results */}
        <div className="col-span-2">
          <Tabs value={activeTab} onValueChange={(v) => setActiveTab(v as any)}>
            <TabsList className="grid w-full grid-cols-3">
              <TabsTrigger value="results">
                Test Results
                {results.length > 0 && (
                  <Badge variant="secondary" className="ml-2">
                    {results.length}
                  </Badge>
                )}
              </TabsTrigger>
              <TabsTrigger value="trace">
                Message Trace
                {messageTrace.length > 0 && (
                  <Badge variant="secondary" className="ml-2">
                    {messageTrace.length}
                  </Badge>
                )}
              </TabsTrigger>
              <TabsTrigger value="status">
                Errors
                {errors.length > 0 && (
                  <Badge variant="destructive" className="ml-2">
                    {errors.length}
                  </Badge>
                )}
              </TabsTrigger>
            </TabsList>
            
            <TabsContent value="results" className="space-y-4">
              {results.length === 0 ? (
                <Card>
                  <CardContent className="py-12 text-center">
                    <p className="text-muted-foreground">
                      No test results yet. Click &quot;Run All Tests&quot; to start.
                    </p>
                  </CardContent>
                </Card>
              ) : (
                results.map((result, idx) => (
                  <TestResultCard key={idx} result={result} />
                ))
              )}
            </TabsContent>
            
            <TabsContent value="trace">
              <Card>
                <CardHeader>
                  <CardTitle>Message Trace Log</CardTitle>
                </CardHeader>
                <CardContent>
                  {messageTrace.length === 0 ? (
                    <p className="text-muted-foreground text-center py-4">
                      No trace events recorded yet.
                    </p>
                  ) : (
                    <ScrollArea className="h-[400px] w-full">
                      <div className="space-y-1">
                        {messageTrace.map((entry, idx) => (
                          <MessageTraceItem key={idx} entry={entry} />
                        ))}
                      </div>
                    </ScrollArea>
                  )}
                </CardContent>
              </Card>
            </TabsContent>
            
            <TabsContent value="status">
              <Card>
                <CardHeader>
                  <CardTitle>Error Log</CardTitle>
                </CardHeader>
                <CardContent>
                  {errors.length === 0 ? (
                    <p className="text-muted-foreground text-center py-4">
                      No errors detected.
                    </p>
                  ) : (
                    <div className="space-y-2">
                      {errors.map((error, idx) => (
                        <Alert key={idx} variant="destructive">
                          <AlertCircle className="h-4 w-4" />
                          <AlertDescription>{error}</AlertDescription>
                        </Alert>
                      ))}
                    </div>
                  )}
                </CardContent>
              </Card>
            </TabsContent>
          </Tabs>
        </div>
      </div>
    </div>
  );
}

export default SessionSwitchTestPanel;