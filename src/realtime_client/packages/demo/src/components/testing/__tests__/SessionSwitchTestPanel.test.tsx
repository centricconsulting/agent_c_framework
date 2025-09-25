/**
 * Tests for SessionSwitchTestPanel component
 * 
 * Validates the visual test panel for session switching:
 * - Interactive UI elements work correctly
 * - Test results are displayed properly
 * - Visual indicators show pass/fail states
 * - Message traces are viewable
 */

import React from 'react';
import { describe, it, expect, beforeEach, vi } from 'vitest';
import { render, screen, fireEvent, waitFor } from '@testing-library/react';
import { SessionSwitchTestPanel } from '../SessionSwitchTestPanel';
import { useSessionSwitchTest } from '@/hooks/useSessionSwitchTest';

// Mock the test hook
vi.mock('@/hooks/useSessionSwitchTest');

// Mock lucide-react icons
vi.mock('lucide-react', () => ({
  Play: () => <span>PlayIcon</span>,
  CheckCircle: () => <span>CheckIcon</span>,
  XCircle: () => <span>XIcon</span>,
  AlertCircle: () => <span>AlertIcon</span>,
  AlertTriangle: () => <span>AlertTriangleIcon</span>,
  Clock: () => <span>ClockIcon</span>,
  ChevronRight: () => <span>ChevronIcon</span>,
  ChevronDown: () => <span>ChevronDownIcon</span>,
  RotateCcw: () => <span>ResetIcon</span>,
  Zap: () => <span>ZapIcon</span>,
  Package: () => <span>PackageIcon</span>,
  Database: () => <span>DatabaseIcon</span>,
  Loader2: () => <span>LoaderIcon</span>,
  Info: () => <span>InfoIcon</span>,
  Settings: () => <span>SettingsIcon</span>,
  Activity: () => <span>ActivityIcon</span>
}));

describe('SessionSwitchTestPanel', () => {
  const mockRunScenario = vi.fn();
  const mockRunAllScenarios = vi.fn();
  const mockClearResults = vi.fn();
  
  const defaultHookReturn = {
    runScenario: mockRunScenario,
    runAllScenarios: mockRunAllScenarios,
    clearResults: mockClearResults,
    isRunning: false,
    currentScenario: null,
    results: [],
    messageTrace: [],
    errors: [],
    summary: {
      total: 0,
      passed: 0,
      failed: 0,
      successRate: 0,
      scenarios: {},
      allPassed: true
    },
    isConnected: true,
    isReady: true,
    currentSessionId: 'session1',
    messageCount: 0,
    sessionCount: 2
  };
  
  beforeEach(() => {
    vi.clearAllMocks();
    vi.mocked(useSessionSwitchTest).mockReturnValue(defaultHookReturn);
  });
  
  describe('Rendering', () => {
    it('should render all test option buttons', () => {
      render(<SessionSwitchTestPanel />);
      
      expect(screen.getByText('Rapid Session Switching')).toBeInTheDocument();
      expect(screen.getByText('Empty Session Test')).toBeInTheDocument();
      expect(screen.getByText('Populated Session Test')).toBeInTheDocument();
      expect(screen.getByText('Loading State Test')).toBeInTheDocument();
    });
    
    it('should render control buttons', () => {
      render(<SessionSwitchTestPanel />);
      
      expect(screen.getByText('Run All Tests')).toBeInTheDocument();
      expect(screen.getByText('Clear Results')).toBeInTheDocument();
    });
    
    it('should show initial state with no results', () => {
      render(<SessionSwitchTestPanel />);
      
      expect(screen.getByText('No test results yet')).toBeInTheDocument();
      expect(screen.queryByText('Test Results')).not.toBeInTheDocument();
    });
  });
  
  describe('Test Execution', () => {
    it('should call runTest when clicking individual test button', () => {
      render(<SessionSwitchTestPanel />);
      
      const rapidTestButton = screen.getByText('Rapid Session Switching')
        .closest('button');
      
      fireEvent.click(rapidTestButton!);
      
      expect(mockRunTest).toHaveBeenCalledWith('rapid');
    });
    
    it('should call runAllTests when clicking Run All Tests button', () => {
      render(<SessionSwitchTestPanel />);
      
      const runAllButton = screen.getByText('Run All Tests');
      fireEvent.click(runAllButton);
      
      expect(mockRunAllTests).toHaveBeenCalled();
    });
    
    it('should disable buttons while tests are running', () => {
      vi.mocked(useSessionSwitchTest).mockReturnValue({
        ...defaultHookReturn,
        isRunning: true,
        currentTest: 'rapid'
      });
      
      render(<SessionSwitchTestPanel />);
      
      // Test buttons should be disabled
      const rapidTestButton = screen.getByText('Rapid Session Switching')
        .closest('button');
      expect(rapidTestButton).toBeDisabled();
      
      // Run All button should be disabled
      const runAllButton = screen.getByText('Run All Tests');
      expect(runAllButton).toBeDisabled();
    });
    
    it('should show running indicator on active test', () => {
      vi.mocked(useSessionSwitchTest).mockReturnValue({
        ...defaultHookReturn,
        isRunning: true,
        currentTest: 'rapid'
      });
      
      render(<SessionSwitchTestPanel />);
      
      // Should show loader icon for running test
      expect(screen.getByText('LoaderIcon')).toBeInTheDocument();
      expect(screen.getByText('Running...')).toBeInTheDocument();
    });
  });
  
  describe('Results Display', () => {
    it('should display pass results with check icon', () => {
      vi.mocked(useSessionSwitchTest).mockReturnValue({
        ...defaultHookReturn,
        results: {
          rapid: {
            name: 'Rapid Session Switching',
            passed: true,
            duration: 1500,
            messageCount: 4,
            errors: []
          }
        }
      });
      
      render(<SessionSwitchTestPanel />);
      
      expect(screen.getByText('Test Results')).toBeInTheDocument();
      expect(screen.getByText('CheckIcon')).toBeInTheDocument();
      expect(screen.getByText('PASS')).toBeInTheDocument();
      
      // Badge should have success variant styling
      const badge = screen.getByText('PASS').closest('div');
      expect(badge).toHaveClass('bg-green-100');
    });
    
    it('should display fail results with X icon', () => {
      vi.mocked(useSessionSwitchTest).mockReturnValue({
        ...defaultHookReturn,
        results: {
          empty: {
            name: 'Empty Session Test',
            passed: false,
            duration: 1000,
            messageCount: 2,
            errors: ['Expected empty session but found 2 messages']
          }
        }
      });
      
      render(<SessionSwitchTestPanel />);
      
      expect(screen.getByText('XIcon')).toBeInTheDocument();
      expect(screen.getByText('FAIL')).toBeInTheDocument();
      
      // Badge should have destructive variant styling
      const badge = screen.getByText('FAIL').closest('div');
      expect(badge).toHaveClass('bg-red-100');
    });
    
    it('should display test duration and message count', () => {
      vi.mocked(useSessionSwitchTest).mockReturnValue({
        ...defaultHookReturn,
        results: {
          populated: {
            name: 'Populated Session Test',
            passed: true,
            duration: 2000,
            messageCount: 10,
            errors: []
          }
        }
      });
      
      render(<SessionSwitchTestPanel />);
      
      expect(screen.getByText('2000ms')).toBeInTheDocument();
      expect(screen.getByText('10 messages')).toBeInTheDocument();
    });
    
    it('should display error messages for failed tests', () => {
      const errorMessage = 'Messages leaked between sessions';
      
      vi.mocked(useSessionSwitchTest).mockReturnValue({
        ...defaultHookReturn,
        results: {
          rapid: {
            name: 'Rapid Session Switching',
            passed: false,
            duration: 1500,
            messageCount: 6,
            errors: [errorMessage]
          }
        }
      });
      
      render(<SessionSwitchTestPanel />);
      
      // Error should be displayed
      expect(screen.getByText(errorMessage)).toBeInTheDocument();
      
      // Should be in an error-styled container
      const errorContainer = screen.getByText(errorMessage).closest('div');
      expect(errorContainer).toHaveClass('bg-red-50');
    });
    
    it('should display multiple test results', () => {
      vi.mocked(useSessionSwitchTest).mockReturnValue({
        ...defaultHookReturn,
        results: {
          rapid: {
            name: 'Rapid Session Switching',
            passed: true,
            duration: 1500,
            messageCount: 4,
            errors: []
          },
          empty: {
            name: 'Empty Session Test',
            passed: false,
            duration: 1000,
            messageCount: 2,
            errors: ['Not empty']
          }
        }
      });
      
      render(<SessionSwitchTestPanel />);
      
      // Both results should be displayed
      expect(screen.getByText('Rapid Session Switching')).toBeInTheDocument();
      expect(screen.getByText('Empty Session Test')).toBeInTheDocument();
      
      // Should show both pass and fail badges
      expect(screen.getByText('PASS')).toBeInTheDocument();
      expect(screen.getByText('FAIL')).toBeInTheDocument();
    });
  });
  
  describe('Message Trace Display', () => {
    it('should show message trace section when traces exist', () => {
      const mockTrace = [
        {
          timestamp: '2024-01-01T12:00:00Z',
          sessionId: 'session1',
          event: 'switch',
          messageCount: 2,
          messageIds: ['msg1', 'msg2']
        }
      ];
      
      vi.mocked(useSessionSwitchTest).mockReturnValue({
        ...defaultHookReturn,
        messageTrace: mockTrace,
        results: { rapid: { name: 'Test', passed: true, duration: 100, messageCount: 2, errors: [] } }
      });
      
      render(<SessionSwitchTestPanel />);
      
      expect(screen.getByText('Message Trace')).toBeInTheDocument();
    });
    
    it('should display trace entries in expandable format', () => {
      const mockTrace = [
        {
          timestamp: '2024-01-01T12:00:00Z',
          sessionId: 'session1',
          event: 'switch',
          messageCount: 2,
          messageIds: ['msg1', 'msg2']
        },
        {
          timestamp: '2024-01-01T12:00:01Z',
          sessionId: 'session2',
          event: 'cleared',
          messageCount: 0,
          messageIds: []
        }
      ];
      
      vi.mocked(useSessionSwitchTest).mockReturnValue({
        ...defaultHookReturn,
        messageTrace: mockTrace,
        results: { rapid: { name: 'Test', passed: true, duration: 100, messageCount: 2, errors: [] } }
      });
      
      render(<SessionSwitchTestPanel />);
      
      // Should show trace entries
      expect(screen.getByText(/switch/)).toBeInTheDocument();
      expect(screen.getByText(/session1/)).toBeInTheDocument();
      expect(screen.getByText(/2 messages/)).toBeInTheDocument();
      
      // Click to expand first trace entry
      const expandButton = screen.getAllByText('ChevronIcon')[0].closest('button');
      fireEvent.click(expandButton!);
      
      // Should show expanded view (chevron changes to down)
      expect(screen.getByText('ChevronDownIcon')).toBeInTheDocument();
      
      // Should show message IDs in expanded view
      expect(screen.getByText(/msg1/)).toBeInTheDocument();
      expect(screen.getByText(/msg2/)).toBeInTheDocument();
    });
  });
  
  describe('Clear Results', () => {
    it('should call clearResults when clicking Clear Results button', () => {
      vi.mocked(useSessionSwitchTest).mockReturnValue({
        ...defaultHookReturn,
        results: {
          rapid: {
            name: 'Test',
            passed: true,
            duration: 100,
            messageCount: 2,
            errors: []
          }
        }
      });
      
      render(<SessionSwitchTestPanel />);
      
      const clearButton = screen.getByText('Clear Results');
      fireEvent.click(clearButton);
      
      expect(mockClearResults).toHaveBeenCalled();
    });
    
    it('should disable Clear Results button when no results exist', () => {
      render(<SessionSwitchTestPanel />);
      
      const clearButton = screen.getByText('Clear Results');
      expect(clearButton).toBeDisabled();
    });
  });
  
  describe('Responsive Design', () => {
    it('should render properly on mobile viewport', () => {
      // Mock mobile viewport
      global.innerWidth = 375;
      global.innerHeight = 667;
      
      render(<SessionSwitchTestPanel />);
      
      // Components should still be present
      expect(screen.getByText('Session Switch Testing')).toBeInTheDocument();
      expect(screen.getByText('Run All Tests')).toBeInTheDocument();
      
      // Test cards should stack vertically on mobile (checked via grid classes)
      const testGrid = screen.getByText('Rapid Session Switching').closest('div')?.parentElement;
      expect(testGrid).toHaveClass('grid-cols-1', 'sm:grid-cols-2');
    });
  });
  
  describe('Accessibility', () => {
    it('should have proper ARIA labels and roles', () => {
      render(<SessionSwitchTestPanel />);
      
      // Buttons should be properly labeled
      const rapidTestButton = screen.getByText('Rapid Session Switching').closest('button');
      expect(rapidTestButton).toHaveAttribute('type', 'button');
      
      // Status indicators should have proper roles
      vi.mocked(useSessionSwitchTest).mockReturnValue({
        ...defaultHookReturn,
        results: {
          rapid: {
            name: 'Test',
            passed: true,
            duration: 100,
            messageCount: 2,
            errors: []
          }
        }
      });
      
      const { container } = render(<SessionSwitchTestPanel />);
      
      // Check for semantic HTML
      expect(container.querySelector('section')).toBeInTheDocument();
    });
  });
});