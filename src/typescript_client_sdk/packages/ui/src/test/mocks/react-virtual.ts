/**
 * Mock for @tanstack/react-virtual
 * Provides a simplified implementation for testing virtual scrolling
 */

import { vi } from 'vitest';
import type { VirtualItem, Virtualizer } from '@tanstack/react-virtual';

/**
 * Create a mock virtual item
 */
function createMockVirtualItem(index: number, size: number): VirtualItem {
  return {
    key: `item-${index}`,
    index,
    start: index * size,
    end: (index + 1) * size,
    size,
    lane: 0,
  };
}

/**
 * Create a mock virtualizer instance
 */
function createMockVirtualizer(count: number, estimateSize: (index: number) => number): Virtualizer<HTMLDivElement, Element> {
  // Generate all virtual items (in real implementation, this would be optimized)
  const virtualItems = Array.from({ length: count }, (_, index) => {
    const size = estimateSize(index);
    return createMockVirtualItem(index, size);
  });
  
  const totalSize = virtualItems.reduce((sum, item) => sum + item.size, 0);
  
  return {
    getVirtualItems: vi.fn(() => virtualItems),
    getTotalSize: vi.fn(() => totalSize),
    scrollToIndex: vi.fn(),
    scrollToOffset: vi.fn(),
    scrollBy: vi.fn(),
    measure: vi.fn(),
    measureElement: vi.fn(),
    options: {} as any,
    scrollElement: null,
    scrollRect: { width: 0, height: 0 } as any,
    scrollOffset: 0,
    scrollDirection: null,
    isScrolling: false,
    range: virtualItems.length > 0 ? {
      startIndex: 0,
      endIndex: virtualItems.length - 1,
      overscan: 5,
      count: virtualItems.length
    } : null,
  } as unknown as Virtualizer<HTMLDivElement, Element>;
}

/**
 * Mock implementation of useVirtualizer
 */
export const useVirtualizer = vi.fn((options: any) => {
  const { count, estimateSize } = options;
  
  // Create a simple mock virtualizer that returns all items
  return createMockVirtualizer(count, estimateSize);
});

/**
 * Export the mock module
 */
export default {
  useVirtualizer,
};
