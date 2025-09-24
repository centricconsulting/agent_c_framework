/**
 * Test Controls Component
 * Provides UI for managing test sessions and scenarios
 */

'use client';

import React, { useState } from 'react';
import { useTestSession } from '@/hooks/use-test-session';
import { 
  Button,
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
  Card,
  CardHeader,
  CardTitle,
  CardDescription,
  CardContent,
  Label,
  Alert,
  AlertDescription,
  Separator,
  Badge,
  ScrollArea,
  Skeleton
} from '@agentc/realtime-ui';
import { 
  FlaskConical, 
  Play, 
  RotateCcw, 
  X, 
  ChevronDown, 
  ChevronUp,
  FileJson,
  AlertCircle,
  CheckCircle
} from 'lucide-react';

/**
 * Test controls component props
 */
export interface TestControlsProps {
  className?: string;
  compact?: boolean;
}

/**
 * Test controls component
 */
export function TestControls({ className = '', compact = false }: TestControlsProps) {
  const {
    testModeEnabled,
    testConfig,
    scenarios,
    currentScenario,
    enableTestMode,
    loadScenario,
    clearSession,
    reloadCurrentScenario,
    showTestControls,
    setShowTestControls,
    isLoading,
    error
  } = useTestSession();

  const [isExpanded, setIsExpanded] = useState(!compact);
  const [selectedScenarioId, setSelectedScenarioId] = useState<string>(
    currentScenario?.id || ''
  );

  /**
   * Handle scenario selection
   */
  const handleScenarioSelect = (scenarioId: string) => {
    setSelectedScenarioId(scenarioId);
  };

  /**
   * Handle load scenario
   */
  const handleLoadScenario = async () => {
    if (selectedScenarioId) {
      await loadScenario(selectedScenarioId);
    }
  };

  /**
   * Handle test mode toggle
   */
  const handleTestModeToggle = (checked: boolean) => {
    enableTestMode(checked);
  };

  // Don't render if test mode is disabled and not showing controls
  if (!testModeEnabled && !showTestControls) {
    return null;
  }

  // Compact mode - just a toggle button
  if (compact && !isExpanded) {
    return (
      <Button
        variant="outline"
        size="sm"
        className={`${className} gap-2`}
        onClick={() => setIsExpanded(true)}
        disabled={!testModeEnabled}
      >
        <FlaskConical className="h-4 w-4" />
        Test Mode
        <ChevronDown className="h-3 w-3" />
      </Button>
    );
  }

  return (
    <Card className={`${className} ${compact ? 'fixed bottom-4 right-4 z-50 max-w-md' : ''}`}>
      <CardHeader className="pb-3">
        <div className="flex items-center justify-between">
          <div className="flex items-center gap-2">
            <FlaskConical className="h-5 w-5 text-primary" />
            <CardTitle className="text-lg">Test Session Manager</CardTitle>
          </div>
          {compact && (
            <Button
              variant="ghost"
              size="icon"
              className="h-8 w-8"
              onClick={() => setIsExpanded(false)}
            >
              <ChevronUp className="h-4 w-4" />
            </Button>
          )}
        </div>
        <CardDescription className="text-sm">
          Load and test chat sessions for validation
        </CardDescription>
      </CardHeader>

      <CardContent className="space-y-4">
        {/* Test Mode Toggle */}
        <div className="flex items-center justify-between">
          <Label htmlFor="test-mode" className="text-sm font-medium">
            Test Mode
          </Label>
          <Button
            id="test-mode"
            variant={testModeEnabled ? "default" : "outline"}
            size="sm"
            onClick={() => handleTestModeToggle(!testModeEnabled)}
          >
            {testModeEnabled ? 'Enabled' : 'Disabled'}
          </Button>
        </div>

        {testModeEnabled && (
          <>
            <Separator />

            {/* Current Session Info */}
            {currentScenario && (
              <div className="space-y-2">
                <div className="text-sm font-medium">Current Session</div>
                <div className="p-3 bg-muted rounded-lg space-y-1">
                  <div className="flex items-center justify-between">
                    <span className="text-sm font-medium">{currentScenario.name}</span>
                    <Badge variant="secondary" className="text-xs">
                      <CheckCircle className="h-3 w-3 mr-1" />
                      Loaded
                    </Badge>
                  </div>
                  {currentScenario.description && (
                    <p className="text-xs text-muted-foreground">
                      {currentScenario.description}
                    </p>
                  )}
                  {currentScenario.tags && currentScenario.tags.length > 0 && (
                    <div className="flex gap-1 flex-wrap pt-1">
                      {currentScenario.tags.map((tag) => (
                        <Badge key={tag} variant="outline" className="text-xs">
                          {tag}
                        </Badge>
                      ))}
                    </div>
                  )}
                </div>
              </div>
            )}

            {/* Scenario Selection */}
            <div className="space-y-2">
              <Label htmlFor="scenario-select" className="text-sm font-medium">
                Select Test Scenario
              </Label>
              <div className="flex gap-2">
                <Select
                  value={selectedScenarioId}
                  onValueChange={handleScenarioSelect}
                  disabled={isLoading}
                >
                  <SelectTrigger id="scenario-select" className="flex-1">
                    <SelectValue placeholder="Choose a test scenario" />
                  </SelectTrigger>
                  <SelectContent>
                    <ScrollArea className="max-h-[200px]">
                      {scenarios.map((scenario) => (
                        <SelectItem key={scenario.id} value={scenario.id}>
                          <div className="flex items-center gap-2">
                            <FileJson className="h-3 w-3" />
                            <span>{scenario.name}</span>
                          </div>
                        </SelectItem>
                      ))}
                    </ScrollArea>
                  </SelectContent>
                </Select>
                <Button
                  size="icon"
                  variant="default"
                  onClick={handleLoadScenario}
                  disabled={!selectedScenarioId || isLoading}
                  title="Load selected scenario"
                >
                  {isLoading ? (
                    <Skeleton className="h-4 w-4 rounded-full" />
                  ) : (
                    <Play className="h-4 w-4" />
                  )}
                </Button>
              </div>
            </div>

            {/* Action Buttons */}
            <div className="flex gap-2">
              <Button
                variant="outline"
                size="sm"
                className="flex-1"
                onClick={reloadCurrentScenario}
                disabled={!currentScenario || isLoading}
              >
                <RotateCcw className="h-3 w-3 mr-1" />
                Reload
              </Button>
              <Button
                variant="outline"
                size="sm"
                className="flex-1"
                onClick={clearSession}
                disabled={!currentScenario || isLoading}
              >
                <X className="h-3 w-3 mr-1" />
                Clear
              </Button>
            </div>

            {/* Error Display */}
            {error && (
              <Alert variant="destructive" className="py-2">
                <AlertCircle className="h-4 w-4" />
                <AlertDescription className="text-sm">
                  {error}
                </AlertDescription>
              </Alert>
            )}

            {/* Loading State */}
            {isLoading && (
              <div className="flex items-center justify-center py-2">
                <div className="flex items-center gap-2 text-sm text-muted-foreground">
                  <Skeleton className="h-4 w-4 rounded-full" />
                  Loading scenario...
                </div>
              </div>
            )}

            {/* Debug Info */}
            {process.env.NODE_ENV === 'development' && currentScenario?.sessionData && (
              <>
                <Separator />
                <div className="space-y-1 text-xs text-muted-foreground">
                  <div>Session ID: {currentScenario.sessionData.session_id}</div>
                  <div>Messages: {currentScenario.sessionData.messages.length}</div>
                  <div>Created: {new Date(currentScenario.sessionData.created_at).toLocaleString()}</div>
                </div>
              </>
            )}
          </>
        )}
      </CardContent>
    </Card>
  );
}

/**
 * Floating test control button
 * Shows a minimal floating button to access test controls
 */
export function FloatingTestButton() {
  const { testModeEnabled } = useTestSession();
  const [showControls, setShowControls] = useState(false);

  if (!testModeEnabled) {
    return null;
  }

  return (
    <>
      {!showControls && (
        <Button
          variant="default"
          size="icon"
          className="fixed bottom-4 right-4 z-50 h-12 w-12 rounded-full shadow-lg"
          onClick={() => setShowControls(true)}
          title="Open test controls"
        >
          <FlaskConical className="h-5 w-5" />
        </Button>
      )}
      
      {showControls && (
        <TestControls 
          compact={true} 
          className="shadow-xl"
        />
      )}
    </>
  );
}