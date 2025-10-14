import { vi } from 'vitest'

/**
 * Creates mock elements and spies for testing the sentinel element pattern
 */
export const createSentinelMocks = () => {
  const sentinel = document.createElement('div')
  sentinel.dataset.testid = 'scroll-sentinel'
  sentinel.style.height = '1px'
  sentinel.style.visibility = 'hidden'
  
  const scrollSpy = vi.fn()
  sentinel.scrollIntoView = scrollSpy
  
  const originalQuerySelector = document.querySelector
  const originalQuerySelectorAll = document.querySelectorAll
  
  // Mock querySelector to return sentinel when requested
  document.querySelector = vi.fn((selector: string) => {
    if (selector.includes('scroll-sentinel')) {
      return sentinel
    }
    return originalQuerySelector.call(document, selector)
  }) as typeof document.querySelector
  
  document.querySelectorAll = vi.fn((selector: string) => {
    if (selector.includes('scroll-sentinel')) {
      return [sentinel] as any
    }
    return originalQuerySelectorAll.call(document, selector)
  }) as typeof document.querySelectorAll
  
  return {
    sentinel,
    scrollSpy,
    cleanup: () => {
      document.querySelector = originalQuerySelector
      document.querySelectorAll = originalQuerySelectorAll
    }
  }
}

/**
 * Mocks scroll measurements for testing scroll behavior
 */
export const mockScrollMeasurements = (config: {
  scrollHeight: number
  clientHeight: number
  initialScrollTop?: number
}) => {
  let scrollTop = config.initialScrollTop ?? 0
  const scrollHistory: number[] = [scrollTop]
  
  Object.defineProperty(HTMLElement.prototype, 'scrollHeight', {
    configurable: true,
    value: config.scrollHeight
  })
  
  Object.defineProperty(HTMLElement.prototype, 'clientHeight', {
    configurable: true,
    value: config.clientHeight
  })
  
  Object.defineProperty(HTMLElement.prototype, 'scrollTop', {
    configurable: true,
    get: () => scrollTop,
    set: (value: number) => {
      scrollTop = value
      scrollHistory.push(value)
    }
  })
  
  return {
    getScrollTop: () => scrollTop,
    setScrollTop: (value: number) => {
      scrollTop = value
      scrollHistory.push(value)
    },
    getMaxScroll: () => config.scrollHeight - config.clientHeight,
    getScrollPercentage: () => {
      const max = config.scrollHeight - config.clientHeight
      return max > 0 ? (scrollTop / max) * 100 : 0
    },
    getScrollHistory: () => scrollHistory,
    isScrolledToBottom: (threshold = 0) => {
      const max = config.scrollHeight - config.clientHeight
      return Math.abs(scrollTop - max) <= threshold
    },
    scrollToBottom: () => {
      const max = config.scrollHeight - config.clientHeight
      scrollTop = max
      scrollHistory.push(max)
    },
    scrollTo80Percent: () => {
      const max = config.scrollHeight - config.clientHeight
      const eightyPercent = Math.floor(max * 0.8)
      scrollTop = eightyPercent
      scrollHistory.push(eightyPercent)
    }
  }
}

/**
 * Creates a mock scroll container with tracking
 */
export const createScrollContainer = (config?: {
  scrollHeight?: number
  clientHeight?: number
  initialScrollTop?: number
}) => {
  const defaultConfig = {
    scrollHeight: config?.scrollHeight ?? 1000,
    clientHeight: config?.clientHeight ?? 500,
    initialScrollTop: config?.initialScrollTop ?? 0
  }
  
  const container = document.createElement('div')
  container.setAttribute('role', 'log')
  
  let scrollTop = defaultConfig.initialScrollTop
  const scrollEvents: { time: number; position: number }[] = []
  
  Object.defineProperty(container, 'scrollHeight', {
    value: defaultConfig.scrollHeight,
    writable: true
  })
  
  Object.defineProperty(container, 'clientHeight', {
    value: defaultConfig.clientHeight,
    writable: true
  })
  
  Object.defineProperty(container, 'scrollTop', {
    get: () => scrollTop,
    set: (value: number) => {
      scrollTop = value
      scrollEvents.push({
        time: Date.now(),
        position: value
      })
    }
  })
  
  // Mock scrollIntoView for child elements
  container.scrollIntoView = vi.fn()
  
  return {
    container,
    getScrollEvents: () => scrollEvents,
    getLastScrollPosition: () => scrollEvents[scrollEvents.length - 1]?.position ?? 0,
    clearScrollEvents: () => scrollEvents.length = 0
  }
}

/**
 * Simulates batch message loading
 */
export const createBulkMessages = (count: number, config?: {
  startTime?: Date
  alternateRoles?: boolean
  includeSystemMessages?: boolean
}) => {
  const messages = []
  const startTime = config?.startTime ?? new Date()
  
  for (let i = 0; i < count; i++) {
    let role: 'user' | 'assistant' | 'system' = 'user'
    
    if (config?.alternateRoles) {
      role = i % 2 === 0 ? 'user' : 'assistant'
    }
    
    if (config?.includeSystemMessages && i % 5 === 0) {
      role = 'system'
    }
    
    messages.push({
      role,
      content: `Message ${i}: ${generateMessageContent(i)}`,
      timestamp: new Date(startTime.getTime() + i * 60000).toISOString()
    })
  }
  
  return messages
}

/**
 * Generates varied message content for testing
 */
function generateMessageContent(index: number): string {
  const contentTypes = [
    'This is a short message.',
    'This is a medium length message that contains more text to simulate real chat content.',
    `This is a long message with multiple lines.
    It includes line breaks and formatting.
    This helps test scroll behavior with varied content heights.
    Some messages might be quite long in real usage.`,
    '```javascript\nconst code = "This is a code block";\nconsole.log(code);\n```',
    '# Heading\n\nThis message has **markdown** formatting with _emphasis_ and [links](http://example.com).'
  ]
  
  return contentTypes[index % contentTypes.length]
}

/**
 * Waits for scroll to complete
 */
export const waitForScroll = async (timeout = 200) => {
  return new Promise(resolve => setTimeout(resolve, timeout))
}

/**
 * Mock performance timing for scroll measurements
 */
export const mockPerformanceTimer = () => {
  let startTime = 0
  const measurements: { name: string; duration: number }[] = []
  
  const originalNow = performance.now
  
  performance.now = vi.fn(() => {
    return Date.now() - startTime
  })
  
  return {
    start: () => {
      startTime = Date.now()
    },
    measure: (name: string) => {
      const duration = performance.now()
      measurements.push({ name, duration })
      return duration
    },
    getMeasurements: () => measurements,
    cleanup: () => {
      performance.now = originalNow
    }
  }
}