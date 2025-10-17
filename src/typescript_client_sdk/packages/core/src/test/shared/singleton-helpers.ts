import { vi } from 'vitest';

/**
 * Creates a mock singleton instance with common methods
 * @param methods - Array of method names to mock
 * @returns Mock singleton with getInstance returning the mocked methods
 */
export const createMockSingleton = (methods: string[] = []) => ({
  getInstance: vi.fn(() => 
    methods.reduce((acc, method) => ({ ...acc, [method]: vi.fn() }), {})
  )
});

/**
 * Asserts that getInstance was called exactly once
 */
export const expectSingletonCalled = (singleton: any) => {
  expect(singleton.getInstance).toHaveBeenCalledTimes(1);
};

/**
 * Creates a simple mock Logger singleton
 */
export const createMockLogger = () => ({
  getInstance: () => ({
    debug: vi.fn(),
    info: vi.fn(),
    warn: vi.fn(),
    error: vi.fn()
  })
});