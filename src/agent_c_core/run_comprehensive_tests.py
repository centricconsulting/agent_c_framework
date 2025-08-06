#!/usr/bin/env python3
"""
Comprehensive Test Runner for Intelligent Message Management System.

This script runs the complete test suite including unit tests, integration tests,
performance benchmarks, and validation checks for the intelligent message
management system.
"""

import os
import sys
import subprocess
import time
import argparse
from pathlib import Path
from typing import List, Dict, Any


class TestRunner:
    """Comprehensive test runner with reporting and analysis."""
    
    def __init__(self, verbose: bool = False):
        self.verbose = verbose
        self.results = {}
        self.start_time = None
        self.project_root = Path(__file__).parent
    
    def log(self, message: str, level: str = "INFO"):
        """Log message with timestamp."""
        timestamp = time.strftime("%Y-%m-%d %H:%M:%S")
        if self.verbose or level in ["ERROR", "WARNING"]:
            print(f"[{timestamp}] {level}: {message}")
    
    def run_command(self, command: List[str], description: str) -> Dict[str, Any]:
        """Run a command and capture results."""
        self.log(f"Running {description}...")
        start_time = time.time()
        
        try:
            result = subprocess.run(
                command,
                capture_output=True,
                text=True,
                cwd=self.project_root,
                timeout=1800  # 30 minute timeout
            )
            
            duration = time.time() - start_time
            
            return {
                'success': result.returncode == 0,
                'duration': duration,
                'stdout': result.stdout,
                'stderr': result.stderr,
                'returncode': result.returncode
            }
            
        except subprocess.TimeoutExpired:
            return {
                'success': False,
                'duration': time.time() - start_time,
                'stdout': '',
                'stderr': 'Test timed out after 30 minutes',
                'returncode': -1
            }
        except Exception as e:
            return {
                'success': False,
                'duration': time.time() - start_time,
                'stdout': '',
                'stderr': str(e),
                'returncode': -2
            }
    
    def run_unit_tests(self) -> bool:
        """Run unit tests."""
        self.log("=" * 60)
        self.log("RUNNING UNIT TESTS")
        self.log("=" * 60)
        
        command = [
            sys.executable, "-m", "pytest",
            "tests/unit/",
            "-v",
            "--tb=short",
            "--cov=src/agent_c",
            "--cov-report=term-missing",
            "--cov-report=html:htmlcov/unit",
            "--junit-xml=test-results-unit.xml",
            "-m", "not slow"
        ]
        
        result = self.run_command(command, "Unit Tests")
        self.results['unit_tests'] = result
        
        if result['success']:
            self.log("✓ Unit tests PASSED", "SUCCESS")
        else:
            self.log("✗ Unit tests FAILED", "ERROR")
            if self.verbose:
                self.log(f"STDOUT:\n{result['stdout']}")
                self.log(f"STDERR:\n{result['stderr']}")
        
        return result['success']
    
    def run_integration_tests(self) -> bool:
        """Run integration tests."""
        self.log("=" * 60)
        self.log("RUNNING INTEGRATION TESTS")
        self.log("=" * 60)
        
        command = [
            sys.executable, "-m", "pytest",
            "tests/integration/",
            "-v",
            "--tb=short",
            "--junit-xml=test-results-integration.xml",
            "-m", "not slow"
        ]
        
        result = self.run_command(command, "Integration Tests")
        self.results['integration_tests'] = result
        
        if result['success']:
            self.log("✓ Integration tests PASSED", "SUCCESS")
        else:
            self.log("✗ Integration tests FAILED", "ERROR")
            if self.verbose:
                self.log(f"STDOUT:\n{result['stdout']}")
                self.log(f"STDERR:\n{result['stderr']}")
        
        return result['success']
    
    def run_performance_tests(self) -> bool:
        """Run performance and benchmark tests."""
        self.log("=" * 60)
        self.log("RUNNING PERFORMANCE TESTS")
        self.log("=" * 60)
        
        command = [
            sys.executable, "-m", "pytest",
            "tests/integration/test_testing_infrastructure_integration.py::TestPerformanceBenchmarks",
            "-v",
            "--tb=short",
            "--junit-xml=test-results-performance.xml",
            "-s"  # Don't capture output for performance logs
        ]
        
        result = self.run_command(command, "Performance Tests")
        self.results['performance_tests'] = result
        
        if result['success']:
            self.log("✓ Performance tests PASSED", "SUCCESS")
        else:
            self.log("✗ Performance tests FAILED", "ERROR")
            if self.verbose:
                self.log(f"STDOUT:\n{result['stdout']}")
                self.log(f"STDERR:\n{result['stderr']}")
        
        return result['success']
    
    def run_stress_tests(self) -> bool:
        """Run stress tests."""
        self.log("=" * 60)
        self.log("RUNNING STRESS TESTS")
        self.log("=" * 60)
        
        command = [
            sys.executable, "-m", "pytest",
            "tests/integration/test_testing_infrastructure_integration.py::TestStressTests",
            "-v",
            "--tb=short",
            "--junit-xml=test-results-stress.xml",
            "-s"
        ]
        
        result = self.run_command(command, "Stress Tests")
        self.results['stress_tests'] = result
        
        if result['success']:
            self.log("✓ Stress tests PASSED", "SUCCESS")
        else:
            self.log("✗ Stress tests FAILED", "ERROR")
            if self.verbose:
                self.log(f"STDOUT:\n{result['stdout']}")
                self.log(f"STDERR:\n{result['stderr']}")
        
        return result['success']
    
    def run_migration_tests(self) -> bool:
        """Run migration tests."""
        self.log("=" * 60)
        self.log("RUNNING MIGRATION TESTS")
        self.log("=" * 60)
        
        command = [
            sys.executable, "-m", "pytest",
            "tests/unit/models/chat_history/test_session_migration.py",
            "tests/integration/test_testing_infrastructure_integration.py::TestMigrationValidation",
            "-v",
            "--tb=short",
            "--junit-xml=test-results-migration.xml"
        ]
        
        result = self.run_command(command, "Migration Tests")
        self.results['migration_tests'] = result
        
        if result['success']:
            self.log("✓ Migration tests PASSED", "SUCCESS")
        else:
            self.log("✗ Migration tests FAILED", "ERROR")
            if self.verbose:
                self.log(f"STDOUT:\n{result['stdout']}")
                self.log(f"STDERR:\n{result['stderr']}")
        
        return result['success']
    
    def run_validation_tests(self) -> bool:
        """Run validation and regression tests."""
        self.log("=" * 60)
        self.log("RUNNING VALIDATION TESTS")
        self.log("=" * 60)
        
        command = [
            sys.executable, "-m", "pytest",
            "tests/integration/test_testing_infrastructure_integration.py::TestValidationUtilities",
            "tests/integration/test_context_integration.py::TestBackwardCompatibilityGuarantees",
            "-v",
            "--tb=short",
            "--junit-xml=test-results-validation.xml"
        ]
        
        result = self.run_command(command, "Validation Tests")
        self.results['validation_tests'] = result
        
        if result['success']:
            self.log("✓ Validation tests PASSED", "SUCCESS")
        else:
            self.log("✗ Validation tests FAILED", "ERROR")
            if self.verbose:
                self.log(f"STDOUT:\n{result['stdout']}")
                self.log(f"STDERR:\n{result['stderr']}")
        
        return result['success']
    
    def run_logging_tests(self) -> bool:
        """Run structured logging integration tests."""
        self.log("=" * 60)
        self.log("RUNNING LOGGING INTEGRATION TESTS")
        self.log("=" * 60)
        
        command = [
            sys.executable, "-m", "pytest",
            "tests/integration/test_structured_logging_integration.py",
            "-v",
            "--tb=short",
            "--junit-xml=test-results-logging.xml"
        ]
        
        result = self.run_command(command, "Logging Integration Tests")
        self.results['logging_tests'] = result
        
        if result['success']:
            self.log("✓ Logging integration tests PASSED", "SUCCESS")
        else:
            self.log("✗ Logging integration tests FAILED", "ERROR")
            if self.verbose:
                self.log(f"STDOUT:\n{result['stdout']}")
                self.log(f"STDERR:\n{result['stderr']}")
        
        return result['success']
    
    def run_comprehensive_system_tests(self) -> bool:
        """Run comprehensive system integration tests."""
        self.log("=" * 60)
        self.log("RUNNING COMPREHENSIVE SYSTEM TESTS")
        self.log("=" * 60)
        
        command = [
            sys.executable, "-m", "pytest",
            "tests/integration/test_comprehensive_system_integration.py",
            "-v",
            "--tb=short",
            "--junit-xml=test-results-system.xml"
        ]
        
        result = self.run_command(command, "Comprehensive System Tests")
        self.results['system_tests'] = result
        
        if result['success']:
            self.log("✓ Comprehensive system tests PASSED", "SUCCESS")
        else:
            self.log("✗ Comprehensive system tests FAILED", "ERROR")
            if self.verbose:
                self.log(f"STDOUT:\n{result['stdout']}")
                self.log(f"STDERR:\n{result['stderr']}")
        
        return result['success']
    
    def generate_coverage_report(self):
        """Generate comprehensive coverage report."""
        self.log("=" * 60)
        self.log("GENERATING COVERAGE REPORT")
        self.log("=" * 60)
        
        command = [
            sys.executable, "-m", "pytest",
            "--cov=src/agent_c",
            "--cov-report=html:htmlcov/comprehensive",
            "--cov-report=term-missing",
            "--cov-report=xml:coverage-comprehensive.xml",
            "--cov-fail-under=85",
            "tests/"
        ]
        
        result = self.run_command(command, "Coverage Report Generation")
        self.results['coverage'] = result
        
        if result['success']:
            self.log("✓ Coverage report generated successfully", "SUCCESS")
            self.log("Coverage report available at: htmlcov/comprehensive/index.html")
        else:
            self.log("✗ Coverage report generation failed", "ERROR")
    
    def print_summary(self):
        """Print test execution summary."""
        self.log("=" * 60)
        self.log("TEST EXECUTION SUMMARY")
        self.log("=" * 60)
        
        total_duration = time.time() - self.start_time if self.start_time else 0
        
        # Count results
        total_tests = len(self.results)
        passed_tests = sum(1 for r in self.results.values() if r['success'])
        failed_tests = total_tests - passed_tests
        
        # Print summary
        self.log(f"Total test suites: {total_tests}")
        self.log(f"Passed: {passed_tests}")
        self.log(f"Failed: {failed_tests}")
        self.log(f"Total execution time: {total_duration:.2f} seconds")
        
        # Print detailed results
        self.log("\nDetailed Results:")
        for test_name, result in self.results.items():
            status = "✓ PASS" if result['success'] else "✗ FAIL"
            duration = result['duration']
            self.log(f"  {test_name:25} {status:8} ({duration:6.2f}s)")
        
        # Print failure details
        if failed_tests > 0:
            self.log("\nFailure Details:")
            for test_name, result in self.results.items():
                if not result['success']:
                    self.log(f"\n{test_name} FAILED:")
                    if result['stderr']:
                        self.log(f"  Error: {result['stderr']}")
                    if result['returncode'] != 0:
                        self.log(f"  Return code: {result['returncode']}")
        
        # Overall result
        if failed_tests == 0:
            self.log("\n🎉 ALL TESTS PASSED! 🎉", "SUCCESS")
            return True
        else:
            self.log(f"\n❌ {failed_tests} TEST SUITE(S) FAILED ❌", "ERROR")
            return False
    
    def run_all_tests(self, test_categories: List[str] = None) -> bool:
        """Run all test categories or specified categories."""
        self.start_time = time.time()
        
        # Default to all categories if none specified
        if not test_categories:
            test_categories = [
                'unit', 'integration', 'system', 'performance', 
                'stress', 'migration', 'validation', 'logging'
            ]
        
        self.log("Starting comprehensive test execution...")
        self.log(f"Test categories: {', '.join(test_categories)}")
        
        # Run test categories
        all_passed = True
        
        if 'unit' in test_categories:
            all_passed &= self.run_unit_tests()
        
        if 'integration' in test_categories:
            all_passed &= self.run_integration_tests()
        
        if 'system' in test_categories:
            all_passed &= self.run_comprehensive_system_tests()
        
        if 'performance' in test_categories:
            all_passed &= self.run_performance_tests()
        
        if 'stress' in test_categories:
            all_passed &= self.run_stress_tests()
        
        if 'migration' in test_categories:
            all_passed &= self.run_migration_tests()
        
        if 'validation' in test_categories:
            all_passed &= self.run_validation_tests()
        
        if 'logging' in test_categories:
            all_passed &= self.run_logging_tests()
        
        # Generate coverage report
        if 'unit' in test_categories or 'integration' in test_categories:
            self.generate_coverage_report()
        
        # Print summary
        success = self.print_summary()
        
        return success and all_passed


def main():
    """Main entry point."""
    parser = argparse.ArgumentParser(
        description="Comprehensive test runner for Intelligent Message Management System"
    )
    
    parser.add_argument(
        '--categories', '-c',
        nargs='+',
        choices=['unit', 'integration', 'system', 'performance', 'stress', 'migration', 'validation', 'logging'],
        help='Test categories to run (default: all)'
    )
    
    parser.add_argument(
        '--verbose', '-v',
        action='store_true',
        help='Verbose output'
    )
    
    parser.add_argument(
        '--quick',
        action='store_true',
        help='Run quick tests only (unit + integration)'
    )
    
    args = parser.parse_args()
    
    # Determine test categories
    if args.quick:
        test_categories = ['unit', 'integration']
    else:
        test_categories = args.categories
    
    # Create and run test runner
    runner = TestRunner(verbose=args.verbose)
    success = runner.run_all_tests(test_categories)
    
    # Exit with appropriate code
    sys.exit(0 if success else 1)


if __name__ == "__main__":
    main()