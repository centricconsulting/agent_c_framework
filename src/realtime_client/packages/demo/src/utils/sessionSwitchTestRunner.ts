/**
 * Session Switch Test Runner
 * 
 * Programmatic test runner for session switching validation.
 * Can be used in automated tests, CI/CD pipelines, or debugging.
 */

import type { TestResult, TestScenario } from '@/hooks/useSessionSwitchTest';

/**
 * Test runner configuration
 */
export interface TestRunnerConfig {
  /** Whether to run tests in verbose mode */
  verbose?: boolean;
  /** Whether to stop on first failure */
  stopOnFailure?: boolean;
  /** Custom logger function */
  logger?: (message: string, level: 'info' | 'warn' | 'error' | 'debug') => void;
  /** Timeout for entire test suite in ms */
  suiteTimeout?: number;
}

/**
 * Test suite result
 */
export interface TestSuiteResult {
  passed: boolean;
  total: number;
  passed_count: number;
  failed_count: number;
  duration: number;
  scenarios: Record<TestScenario, TestResult | null>;
  errors: string[];
}

/**
 * Session Switch Test Runner Class
 */
export class SessionSwitchTestRunner {
  private config: TestRunnerConfig;
  private results: TestResult[] = [];
  private startTime: number = 0;

  constructor(config: TestRunnerConfig = {}) {
    this.config = {
      verbose: false,
      stopOnFailure: false,
      logger: console.log,
      suiteTimeout: 30000,
      ...config
    };
  }

  /**
   * Log a message
   */
  private log(message: string, level: 'info' | 'warn' | 'error' | 'debug' = 'info') {
    if (this.config.verbose || level === 'error') {
      this.config.logger?.(
        `[SessionSwitchTest] ${message}`,
        level
      );
    }
  }

  /**
   * Validate a single test result
   */
  private validateResult(result: TestResult): {
    valid: boolean;
    issues: string[];
  } {
    const issues: string[] = [];

    // Check if test actually ran
    if (!result.scenario) {
      issues.push('Test result missing scenario identifier');
    }

    // Check if duration is reasonable
    if (result.duration <= 0) {
      issues.push('Test duration is invalid (<=0)');
    }

    if (result.duration > 10000) {
      issues.push(`Test took too long (${result.duration}ms)`);
    }

    // Check for specific failure patterns
    if (!result.passed && result.details.issues.length === 0) {
      issues.push('Test failed but no issues were recorded');
    }

    // Check for message leakage indicators
    const hasLeakage = result.details.issues.some(issue =>
      issue.toLowerCase().includes('leak') ||
      issue.toLowerCase().includes('accumulation')
    );

    if (hasLeakage) {
      issues.push('Critical: Message leakage detected');
    }

    return {
      valid: issues.length === 0,
      issues
    };
  }

  /**
   * Analyze test results for patterns
   */
  public analyzeResults(results: TestResult[]): {
    patterns: string[];
    recommendations: string[];
  } {
    const patterns: string[] = [];
    const recommendations: string[] = [];

    // Check for consistent failures
    const failedScenarios = results.filter(r => !r.passed);
    if (failedScenarios.length === results.length && results.length > 0) {
      patterns.push('All tests are failing - likely a systemic issue');
      recommendations.push('Check WebSocket connection and session management hooks');
    }

    // Check for message leakage patterns
    const leakageCount = results.filter(r =>
      r.details.issues.some(i => i.includes('leak'))
    ).length;

    if (leakageCount > 0) {
      patterns.push(`Message leakage detected in ${leakageCount} test(s)`);
      recommendations.push('Review useChat hook message clearing logic');
      recommendations.push('Check session-messages-loaded event handling');
    }

    // Check for accumulation patterns
    const accumulationCount = results.filter(r =>
      r.details.issues.some(i => i.includes('accumulation'))
    ).length;

    if (accumulationCount > 0) {
      patterns.push(`Message accumulation detected in ${accumulationCount} test(s)`);
      recommendations.push('Ensure messages are properly cleared on session switch');
      recommendations.push('Check for duplicate event listeners');
    }

    // Check for timing issues
    const timingIssues = results.filter(r =>
      r.duration > 5000 || r.details.issues.some(i => i.includes('timeout'))
    ).length;

    if (timingIssues > 0) {
      patterns.push(`Timing issues in ${timingIssues} test(s)`);
      recommendations.push('Consider increasing timeouts for slow connections');
      recommendations.push('Check for race conditions in session switching');
    }

    // Scenario-specific patterns
    const rapidSwitchResult = results.find(r => r.scenario === 'rapid-switching');
    if (rapidSwitchResult && !rapidSwitchResult.passed) {
      patterns.push('Rapid switching fails - possible race condition');
      recommendations.push('Add debouncing or queuing for session switches');
    }

    const loadingStateResult = results.find(r => r.scenario === 'loading-state');
    if (loadingStateResult && !loadingStateResult.passed) {
      patterns.push('Loading state issues - messages visible during transition');
      recommendations.push('Implement proper loading state management');
      recommendations.push('Clear messages immediately on session switch initiation');
    }

    return { patterns, recommendations };
  }

  /**
   * Generate a detailed report
   */
  public generateReport(results: TestResult[]): string {
    const passed = results.filter(r => r.passed).length;
    const failed = results.length - passed;
    const successRate = results.length > 0 ? (passed / results.length) * 100 : 0;

    const analysis = this.analyzeResults(results);

    let report = `
SESSION SWITCH TEST REPORT
========================
Timestamp: ${new Date().toISOString()}
Total Tests: ${results.length}
Passed: ${passed}
Failed: ${failed}
Success Rate: ${successRate.toFixed(1)}%

INDIVIDUAL TEST RESULTS
-----------------------`;

    results.forEach(result => {
      const validation = this.validateResult(result);
      report += `
      
Test: ${result.scenario}
Status: ${result.passed ? 'PASSED ✓' : 'FAILED ✗'}
Duration: ${result.duration}ms
Expected: ${result.details.expectedBehavior}
Actual: ${result.details.actualBehavior}`;

      if (result.details.issues.length > 0) {
        report += `\nIssues:`;
        result.details.issues.forEach(issue => {
          report += `\n  - ${issue}`;
        });
      }

      if (!validation.valid) {
        report += `\nValidation Issues:`;
        validation.issues.forEach(issue => {
          report += `\n  ⚠ ${issue}`;
        });
      }
    });

    if (analysis.patterns.length > 0) {
      report += `

PATTERNS DETECTED
-----------------`;
      analysis.patterns.forEach(pattern => {
        report += `\n• ${pattern}`;
      });
    }

    if (analysis.recommendations.length > 0) {
      report += `

RECOMMENDATIONS
---------------`;
      analysis.recommendations.forEach(rec => {
        report += `\n→ ${rec}`;
      });
    }

    report += `

MESSAGE TRACE SUMMARY
--------------------`;

    // Summarize message trace if available
    const tracedResults = results.filter(r => r.messageTrace && r.messageTrace.length > 0);
    if (tracedResults.length > 0) {
      const totalEvents = tracedResults.reduce((sum, r) => sum + (r.messageTrace?.length || 0), 0);
      const leakEvents = tracedResults.reduce((sum, r) => 
        sum + (r.messageTrace?.filter(e => e.event === 'leaked').length || 0), 0
      );
      const accumEvents = tracedResults.reduce((sum, r) =>
        sum + (r.messageTrace?.filter(e => e.event === 'accumulated').length || 0), 0
      );

      report += `
Total Trace Events: ${totalEvents}
Leak Events: ${leakEvents}
Accumulation Events: ${accumEvents}`;
    } else {
      report += `\nNo message trace data available`;
    }

    report += `

SUMMARY
-------
${failed === 0 ? '✅ All tests passed!' : `❌ ${failed} test(s) failed`}
${analysis.patterns.length > 0 ? '⚠️  Issues detected - review recommendations' : ''}

========================
End of Report
`;

    return report;
  }

  /**
   * Export results to JSON for further analysis
   */
  public exportToJSON(results: TestResult[]): string {
    const analysis = this.analyzeResults(results);
    const suite: TestSuiteResult = {
      passed: results.every(r => r.passed),
      total: results.length,
      passed_count: results.filter(r => r.passed).length,
      failed_count: results.filter(r => !r.passed).length,
      duration: results.reduce((sum, r) => sum + r.duration, 0),
      scenarios: {
        'rapid-switching': results.find(r => r.scenario === 'rapid-switching') || null,
        'empty-session': results.find(r => r.scenario === 'empty-session') || null,
        'populated-session': results.find(r => r.scenario === 'populated-session') || null,
        'loading-state': results.find(r => r.scenario === 'loading-state') || null
      },
      errors: results.flatMap(r => r.details.issues)
    };

    return JSON.stringify({
      suite,
      analysis,
      timestamp: new Date().toISOString(),
      results
    }, null, 2);
  }

  /**
   * Run assertion checks for CI/CD integration
   */
  public assertResults(results: TestResult[]): void {
    const failed = results.filter(r => !r.passed);
    
    if (failed.length > 0) {
      const errorMessage = `Session switch tests failed: ${failed.length} of ${results.length} tests failed\n` +
        failed.map(r => `- ${r.scenario}: ${r.details.issues.join(', ')}`).join('\n');
      
      throw new Error(errorMessage);
    }
    
    // Additional assertions
    const analysis = this.analyzeResults(results);
    if (analysis.patterns.some(p => p.includes('Critical'))) {
      throw new Error('Critical issues detected in session switching');
    }
    
    this.log('All session switch tests passed!', 'info');
  }
}

/**
 * Quick test runner function for simple usage
 */
export async function runSessionSwitchTests(
  testHook: {
    runAllScenarios: () => Promise<TestResult[]>;
    isReady: boolean;
  },
  config?: TestRunnerConfig
): Promise<TestSuiteResult> {
  const runner = new SessionSwitchTestRunner(config);
  
  if (!testHook.isReady) {
    throw new Error('Test environment not ready. Ensure connection is established and sessions are loaded.');
  }
  
  const startTime = Date.now();
  
  try {
    const results = await testHook.runAllScenarios();
    const duration = Date.now() - startTime;
    
    // Generate and log report if verbose
    if (config?.verbose) {
      const report = runner.generateReport(results);
      console.log(report);
    }
    
    // Run assertions if needed
    if (process.env.NODE_ENV === 'test') {
      runner.assertResults(results);
    }
    
    return {
      passed: results.every(r => r.passed),
      total: results.length,
      passed_count: results.filter(r => r.passed).length,
      failed_count: results.filter(r => !r.passed).length,
      duration,
      scenarios: {
        'rapid-switching': results.find(r => r.scenario === 'rapid-switching') || null,
        'empty-session': results.find(r => r.scenario === 'empty-session') || null,
        'populated-session': results.find(r => r.scenario === 'populated-session') || null,
        'loading-state': results.find(r => r.scenario === 'loading-state') || null
      },
      errors: results.flatMap(r => r.details.issues)
    };
  } catch (error) {
    throw new Error(`Test suite failed: ${error}`);
  }
}

export default SessionSwitchTestRunner;