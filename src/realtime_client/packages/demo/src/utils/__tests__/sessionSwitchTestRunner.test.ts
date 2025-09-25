/**
 * Tests for sessionSwitchTestRunner utility
 * 
 * Validates the programmatic test runner for session switching:
 * - Report generation
 * - Pattern analysis
 * - JSON export functionality
 * - CI/CD integration
 */

import { describe, it, expect, beforeEach, vi, afterEach } from 'vitest';
import SessionSwitchTestRunner, { runSessionSwitchTests } from '../sessionSwitchTestRunner';
import type { TestResult } from '@/hooks/useSessionSwitchTest';

// Mock console methods to avoid test output noise
const originalConsole = { ...console };

describe('SessionSwitchTestRunner', () => {
  // Test data
  const mockTestResults: TestResult[] = [
    {
      scenario: 'rapid-switching',
      passed: true,
      duration: 1500,
      details: {
        expectedBehavior: 'Messages clear between rapid switches',
        actualBehavior: 'Messages cleared correctly',
        issues: []
      },
      messageTrace: [
        {
          timestamp: new Date().toISOString(),
          event: 'switch',
          sessionId: 'session1',
          messageCount: 2,
          messageIds: ['msg1', 'msg2']
        },
        {
          timestamp: new Date().toISOString(),
          event: 'cleared',
          sessionId: 'session2',
          messageCount: 0,
          messageIds: []
        }
      ]
    },
    {
      scenario: 'empty-session',
      passed: false,
      duration: 1000,
      details: {
        expectedBehavior: 'Empty session should have no messages',
        actualBehavior: 'Found 2 messages in empty session',
        issues: ['Message leak detected - Expected empty session but found 2 messages']
      },
      messageTrace: [
        {
          timestamp: new Date().toISOString(),
          event: 'leaked',
          sessionId: 'empty',
          messageCount: 2,
          messageIds: ['leaked1', 'leaked2']
        }
      ]
    },
    {
      scenario: 'populated-session',
      passed: true,
      duration: 2000,
      details: {
        expectedBehavior: 'Populated session maintains correct messages',
        actualBehavior: 'Messages maintained correctly',
        issues: []
      }
    },
    {
      scenario: 'loading-state',
      passed: false,
      duration: 1500,
      details: {
        expectedBehavior: 'No messages during loading',
        actualBehavior: 'Messages visible during loading state',
        issues: ['Message accumulation: Messages appeared during loading transition']
      },
      messageTrace: [
        {
          timestamp: new Date().toISOString(),
          event: 'accumulated',
          sessionId: 'loading',
          messageCount: 3,
          messageIds: ['acc1', 'acc2', 'acc3']
        }
      ]
    }
  ];
  
  beforeEach(() => {
    vi.clearAllMocks();
    console.log = vi.fn();
    console.error = vi.fn();
    console.warn = vi.fn();
  });
  
  afterEach(() => {
    Object.assign(console, originalConsole);
  });
  
  describe('Constructor', () => {
    it('should create instance with default config', () => {
      const runner = new SessionSwitchTestRunner();
      expect(runner).toBeDefined();
      expect(runner).toBeInstanceOf(SessionSwitchTestRunner);
    });
    
    it('should accept custom configuration', () => {
      const customLogger = vi.fn();
      const runner = new SessionSwitchTestRunner({
        verbose: true,
        stopOnFailure: true,
        logger: customLogger,
        suiteTimeout: 60000
      });
      
      expect(runner).toBeDefined();
    });
  });
  
  describe('analyzeResults', () => {
    it('should identify message leakage patterns', () => {
      const runner = new SessionSwitchTestRunner();
      const analysis = runner.analyzeResults(mockTestResults);
      
      expect(analysis.patterns).toContain(
        expect.stringMatching(/Message leakage detected/)
      );
      expect(analysis.recommendations).toContain(
        expect.stringMatching(/Review useChat hook|session-messages-loaded/)
      );
    });
    
    it('should identify accumulation patterns', () => {
      const runner = new SessionSwitchTestRunner();
      const analysis = runner.analyzeResults(mockTestResults);
      
      expect(analysis.patterns).toContain(
        expect.stringMatching(/Message accumulation detected/)
      );
      expect(analysis.recommendations).toContain(
        expect.stringMatching(/messages are properly cleared|duplicate event listeners/)
      );
    });
    
    it('should detect systemic failures', () => {
      const allFailedResults: TestResult[] = mockTestResults.map(r => ({
        ...r,
        passed: false,
        details: {
          ...r.details,
          issues: ['Test failed']
        }
      }));
      
      const runner = new SessionSwitchTestRunner();
      const analysis = runner.analyzeResults(allFailedResults);
      
      expect(analysis.patterns).toContain(
        'All tests are failing - likely a systemic issue'
      );
      expect(analysis.recommendations).toContain(
        'Check WebSocket connection and session management hooks'
      );
    });
    
    it('should handle empty results gracefully', () => {
      const runner = new SessionSwitchTestRunner();
      const analysis = runner.analyzeResults([]);
      
      expect(analysis.patterns).toBeDefined();
      expect(analysis.recommendations).toBeDefined();
      expect(Array.isArray(analysis.patterns)).toBe(true);
      expect(Array.isArray(analysis.recommendations)).toBe(true);
    });
    
    it('should detect timing issues', () => {
      const slowResults: TestResult[] = [{
        scenario: 'rapid-switching',
        passed: false,
        duration: 6000,
        details: {
          expectedBehavior: 'Fast switching',
          actualBehavior: 'Slow response',
          issues: ['Timeout during test']
        }
      }];
      
      const runner = new SessionSwitchTestRunner();
      const analysis = runner.analyzeResults(slowResults);
      
      expect(analysis.patterns).toContain(
        expect.stringMatching(/Timing issues/)
      );
      expect(analysis.recommendations).toContain(
        expect.stringMatching(/increasing timeouts|race conditions/)
      );
    });
    
    it('should provide scenario-specific recommendations', () => {
      const rapidFailure: TestResult[] = [{
        scenario: 'rapid-switching',
        passed: false,
        duration: 1500,
        details: {
          expectedBehavior: 'Clean switches',
          actualBehavior: 'Messages leaked',
          issues: ['Race condition detected']
        }
      }];
      
      const runner = new SessionSwitchTestRunner();
      const analysis = runner.analyzeResults(rapidFailure);
      
      expect(analysis.patterns).toContain(
        'Rapid switching fails - possible race condition'
      );
      expect(analysis.recommendations).toContain(
        'Add debouncing or queuing for session switches'
      );
    });
    
    it('should handle loading state issues', () => {
      const loadingFailure: TestResult[] = [{
        scenario: 'loading-state',
        passed: false,
        duration: 1000,
        details: {
          expectedBehavior: 'No messages during loading',
          actualBehavior: 'Messages visible',
          issues: ['Loading state compromised']
        }
      }];
      
      const runner = new SessionSwitchTestRunner();
      const analysis = runner.analyzeResults(loadingFailure);
      
      expect(analysis.patterns).toContain(
        'Loading state issues - messages visible during transition'
      );
      expect(analysis.recommendations).toContain(
        expect.stringMatching(/loading state management|Clear messages immediately/)
      );
    });
  });
  
  describe('generateReport', () => {
    it('should generate comprehensive test report', () => {
      const runner = new SessionSwitchTestRunner();
      const report = runner.generateReport(mockTestResults);
      
      expect(report).toContain('SESSION SWITCH TEST REPORT');
      expect(report).toContain('Total Tests: 4');
      expect(report).toContain('Passed: 2');
      expect(report).toContain('Failed: 2');
      expect(report).toContain('Success Rate: 50.0%');
    });
    
    it('should include individual test results', () => {
      const runner = new SessionSwitchTestRunner();
      const report = runner.generateReport(mockTestResults);
      
      expect(report).toContain('rapid-switching');
      expect(report).toContain('PASSED ✓');
      expect(report).toContain('empty-session');
      expect(report).toContain('FAILED ✗');
      expect(report).toContain('Expected empty session but found 2 messages');
    });
    
    it('should include patterns and recommendations', () => {
      const runner = new SessionSwitchTestRunner();
      const report = runner.generateReport(mockTestResults);
      
      expect(report).toContain('PATTERNS DETECTED');
      expect(report).toContain('Message leakage detected');
      expect(report).toContain('RECOMMENDATIONS');
      expect(report).toContain('Review useChat hook');
    });
    
    it('should include message trace summary', () => {
      const runner = new SessionSwitchTestRunner();
      const report = runner.generateReport(mockTestResults);
      
      expect(report).toContain('MESSAGE TRACE SUMMARY');
      expect(report).toContain('Total Trace Events:');
      expect(report).toContain('Leak Events:');
      expect(report).toContain('Accumulation Events:');
    });
    
    it('should handle results without traces', () => {
      const noTraceResults: TestResult[] = [{
        scenario: 'test',
        passed: true,
        duration: 100,
        details: {
          expectedBehavior: 'Works',
          actualBehavior: 'Works',
          issues: []
        }
      }];
      
      const runner = new SessionSwitchTestRunner();
      const report = runner.generateReport(noTraceResults);
      
      expect(report).toContain('No message trace data available');
    });
    
    it('should validate test results', () => {
      const invalidResults: TestResult[] = [{
        scenario: 'test',
        passed: false,
        duration: -1, // Invalid duration
        details: {
          expectedBehavior: 'Works',
          actualBehavior: 'Failed',
          issues: [] // No issues despite failure
        }
      }];
      
      const runner = new SessionSwitchTestRunner();
      const report = runner.generateReport(invalidResults);
      
      expect(report).toContain('Validation Issues:');
      expect(report).toContain('duration is invalid');
      expect(report).toContain('failed but no issues were recorded');
    });
  });
  
  describe('exportToJSON', () => {
    it('should export results as valid JSON', () => {
      const runner = new SessionSwitchTestRunner();
      const json = runner.exportToJSON(mockTestResults);
      
      expect(() => JSON.parse(json)).not.toThrow();
      
      const parsed = JSON.parse(json);
      expect(parsed).toHaveProperty('timestamp');
      expect(parsed).toHaveProperty('suite');
      expect(parsed).toHaveProperty('analysis');
      expect(parsed).toHaveProperty('results');
    });
    
    it('should include test suite result structure', () => {
      const runner = new SessionSwitchTestRunner();
      const json = runner.exportToJSON(mockTestResults);
      const parsed = JSON.parse(json);
      
      expect(parsed.suite).toHaveProperty('passed', false);
      expect(parsed.suite).toHaveProperty('total', 4);
      expect(parsed.suite).toHaveProperty('passed_count', 2);
      expect(parsed.suite).toHaveProperty('failed_count', 2);
      expect(parsed.suite).toHaveProperty('duration');
      expect(parsed.suite).toHaveProperty('scenarios');
      expect(parsed.suite).toHaveProperty('errors');
    });
    
    it('should include scenarios mapping', () => {
      const runner = new SessionSwitchTestRunner();
      const json = runner.exportToJSON(mockTestResults);
      const parsed = JSON.parse(json);
      
      expect(parsed.suite.scenarios).toHaveProperty('rapid-switching');
      expect(parsed.suite.scenarios).toHaveProperty('empty-session');
      expect(parsed.suite.scenarios).toHaveProperty('populated-session');
      expect(parsed.suite.scenarios).toHaveProperty('loading-state');
    });
    
    it('should include analysis in export', () => {
      const runner = new SessionSwitchTestRunner();
      const json = runner.exportToJSON(mockTestResults);
      const parsed = JSON.parse(json);
      
      expect(parsed.analysis).toHaveProperty('patterns');
      expect(parsed.analysis).toHaveProperty('recommendations');
      expect(Array.isArray(parsed.analysis.patterns)).toBe(true);
      expect(Array.isArray(parsed.analysis.recommendations)).toBe(true);
    });
    
    it('should handle empty results', () => {
      const runner = new SessionSwitchTestRunner();
      const json = runner.exportToJSON([]);
      const parsed = JSON.parse(json);
      
      expect(parsed.suite.total).toBe(0);
      expect(parsed.suite.passed_count).toBe(0);
      expect(parsed.suite.failed_count).toBe(0);
    });
  });
  
  describe('assertResults', () => {
    it('should throw error when tests fail', () => {
      const runner = new SessionSwitchTestRunner();
      
      expect(() => {
        runner.assertResults(mockTestResults);
      }).toThrow(/Session switch tests failed: 2 of 4 tests failed/);
    });
    
    it('should not throw when all tests pass', () => {
      const passingResults: TestResult[] = mockTestResults.map(r => ({
        ...r,
        passed: true,
        details: {
          ...r.details,
          issues: []
        }
      }));
      
      const runner = new SessionSwitchTestRunner();
      
      expect(() => {
        runner.assertResults(passingResults);
      }).not.toThrow();
    });
    
    it('should throw on critical issues', () => {
      const criticalResults: TestResult[] = [{
        scenario: 'test',
        passed: false,
        duration: 100,
        details: {
          expectedBehavior: 'No leaks',
          actualBehavior: 'Major leakage',
          issues: ['Critical: Message leakage detected', 'Data corruption']
        }
      }];
      
      const runner = new SessionSwitchTestRunner();
      
      expect(() => {
        runner.assertResults(criticalResults);
      }).toThrow();
    });
  });
  
  describe('runSessionSwitchTests helper', () => {
    it('should run tests with test hook', async () => {
      // Temporarily set NODE_ENV to avoid auto-assertion
      const originalEnv = process.env.NODE_ENV;
      process.env.NODE_ENV = 'production';
      
      const mockTestHook = {
        runAllScenarios: vi.fn().mockResolvedValue(mockTestResults),
        isReady: true
      };
      
      const result = await runSessionSwitchTests(mockTestHook);
      
      expect(mockTestHook.runAllScenarios).toHaveBeenCalled();
      expect(result).toHaveProperty('passed', false);
      expect(result).toHaveProperty('total', 4);
      expect(result).toHaveProperty('passed_count', 2);
      expect(result).toHaveProperty('failed_count', 2);
      
      // Restore NODE_ENV
      process.env.NODE_ENV = originalEnv;
    });
    
    it('should throw if test environment not ready', async () => {
      const mockTestHook = {
        runAllScenarios: vi.fn(),
        isReady: false
      };
      
      await expect(runSessionSwitchTests(mockTestHook))
        .rejects.toThrow('Test environment not ready');
    });
    
    it('should accept custom configuration', async () => {
      const customLogger = vi.fn();
      const mockTestHook = {
        runAllScenarios: vi.fn().mockResolvedValue([]),
        isReady: true
      };
      
      await runSessionSwitchTests(mockTestHook, {
        verbose: true,
        logger: customLogger
      });
      
      // Should execute without error
      expect(mockTestHook.runAllScenarios).toHaveBeenCalled();
    });
    
    it('should handle test execution errors', async () => {
      const mockTestHook = {
        runAllScenarios: vi.fn().mockRejectedValue(new Error('Test failed')),
        isReady: true
      };
      
      await expect(runSessionSwitchTests(mockTestHook))
        .rejects.toThrow('Test failed');
    });
    
    it('should generate full test suite result', async () => {
      // Temporarily set NODE_ENV to avoid auto-assertion
      const originalEnv = process.env.NODE_ENV;
      process.env.NODE_ENV = 'production';
      
      const mockTestHook = {
        runAllScenarios: vi.fn().mockResolvedValue(mockTestResults),
        isReady: true
      };
      
      const result = await runSessionSwitchTests(mockTestHook);
      
      expect(result.scenarios['rapid-switching']).toBeDefined();
      expect(result.scenarios['empty-session']).toBeDefined();
      expect(result.scenarios['populated-session']).toBeDefined();
      expect(result.scenarios['loading-state']).toBeDefined();
      expect(result.errors).toHaveLength(2); // Two failed tests with issues
      expect(result.duration).toBeGreaterThan(0);
      
      // Restore NODE_ENV
      process.env.NODE_ENV = originalEnv;
    });
  });
  
  describe('Verbose Logging', () => {
    it('should log success message when all tests pass in verbose mode', () => {
      const customLogger = vi.fn();
      const passingResults: TestResult[] = mockTestResults.map(r => ({
        ...r,
        passed: true,
        details: {
          ...r.details,
          issues: []
        }
      }));
      
      const runner = new SessionSwitchTestRunner({
        verbose: true,
        logger: customLogger
      });
      
      // assertResults logs on success
      runner.assertResults(passingResults);
      
      // Logger should be called with success message
      expect(customLogger).toHaveBeenCalledWith(
        '[SessionSwitchTest] All session switch tests passed!',
        'info'
      );
    });
    
    it('should accept custom logger in configuration', () => {
      const customLogger = vi.fn();
      const runner = new SessionSwitchTestRunner({
        verbose: true,
        logger: customLogger
      });
      
      expect(runner).toBeDefined();
      // Logger is stored in config and will be used when log() is called
    });
  });
});