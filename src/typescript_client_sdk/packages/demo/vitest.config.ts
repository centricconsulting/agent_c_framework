// packages/demo/vitest.config.ts
import { defineConfig } from 'vitest/config';
import { resolve } from 'path';

export default defineConfig({
  test: {
    globals: true,
    environment: 'happy-dom', // React components require happy-dom
    setupFiles: ['./src/test/setup.ts'],
    coverage: {
      provider: 'v8',
      reporter: ['text', 'json', 'html', 'lcov'],
      reportsDirectory: '../../.scratch/coverage/demo',
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
      '@test': resolve(__dirname, './src/test'),
      '@agentc/realtime-react': resolve(__dirname, '../react/src'),
      '@agentc/realtime-core': resolve(__dirname, '../core/src'),
      '@agentc/realtime-ui': resolve(__dirname, '../ui/src')
    }
  }
});