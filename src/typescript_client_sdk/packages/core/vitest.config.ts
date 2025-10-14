// packages/core/vitest.config.ts
import { defineConfig } from 'vitest/config';
import { resolve } from 'path';

export default defineConfig({
  test: {
    globals: true,
    environment: 'happy-dom', // Use happy-dom for browser API mocking
    setupFiles: ['./src/test/setup.ts'],
    coverage: {
      provider: 'v8',
      reporter: ['text', 'json', 'html', 'lcov'],
      reportsDirectory: '../../.scratch/coverage/core',
      exclude: [
        'node_modules/**',
        'dist/**',
        '**/*.config.*',
        '**/*.d.ts',
        '**/__tests__/**',
        '**/__mocks__/**',
        '**/test/setup.ts'
      ],
      thresholds: {
        // Temporarily set to 0 during test infrastructure reset
        // TODO: Restore to 80% when actual tests are written
        branches: 0,
        functions: 0,
        lines: 0,
        statements: 0
      }
    },
    testTimeout: 10000,
    hookTimeout: 10000
  },
  resolve: {
    alias: {
      '@': resolve(__dirname, './src'),
      '@test': resolve(__dirname, './src/test')
    }
  }
});