/**
 * Integration tests for EventStreamProcessor RenderMedia event handling
 * Critical: Verifies markdown content type is preserved and not transformed to HTML
 */

import { describe, it, expect, beforeEach, vi } from 'vitest';
import { EventStreamProcessor } from '../EventStreamProcessor';
import { ChatSessionManager } from '../../session/SessionManager';
import type { RenderMediaEvent } from '../types/ServerEvents';

describe('EventStreamProcessor - RenderMedia Event Handling', () => {
  let processor: EventStreamProcessor;
  let sessionManager: ChatSessionManager;
  let emittedEvents: Array<{ event: string; data: any }>;

  beforeEach(() => {
    vi.clearAllMocks();
    sessionManager = new ChatSessionManager();
    processor = new EventStreamProcessor(sessionManager);
    emittedEvents = [];

    // Capture all emitted events
    const originalEmit = sessionManager.emit.bind(sessionManager);
    sessionManager.emit = vi.fn((event: string, data?: any) => {
      emittedEvents.push({ event, data });
      return originalEmit(event, data);
    });
  });

  describe('Critical Bug Fix: Markdown Content Type Preservation', () => {
    it('should preserve text/markdown content type (NOT transform to html)', () => {
      // Arrange - This is the CRITICAL test for the bug fix
      const markdownEvent: RenderMediaEvent = {
        type: 'render_media',
        session_id: 'test-session',
        role: 'assistant',
        content_type: 'text/markdown',  // Server sends markdown
        content: '# Heading\\n\\nThis is **bold** text',
        foreign_content: false
      };

      // Act
      processor.processEvent(markdownEvent);

      // Assert - CRITICAL: contentType must be "text/markdown" NOT "html"
      const mediaEvent = emittedEvents.find(e => e.event === 'media-added');
      expect(mediaEvent).toBeDefined();
      expect(mediaEvent!.data.media.contentType).toBe('text/markdown'); // NOT 'html'
      expect(mediaEvent!.data.media.content).toBe('# Heading\\n\\nThis is **bold** text');
    });

    it('should preserve text/html content type correctly', () => {
      // Arrange
      const htmlEvent: RenderMediaEvent = {
        type: 'render_media',
        session_id: 'test-session',
        role: 'assistant',
        content_type: 'text/html',
        content: '<div>HTML content</div>',
        foreign_content: false
      };

      // Act
      processor.processEvent(htmlEvent);

      // Assert
      const mediaEvent = emittedEvents.find(e => e.event === 'media-added');
      expect(mediaEvent).toBeDefined();
      expect(mediaEvent!.data.media.contentType).toBe('text/html');
      expect(mediaEvent!.data.media.content).toBe('<div>HTML content</div>');
    });

    it('should preserve image/svg+xml content type', () => {
      // Arrange
      const svgEvent: RenderMediaEvent = {
        type: 'render_media',
        session_id: 'test-session',
        role: 'assistant',
        content_type: 'image/svg+xml',
        content: '<svg><circle cx="50" cy="50" r="40"/></svg>',
        foreign_content: false
      };

      // Act
      processor.processEvent(svgEvent);

      // Assert
      const mediaEvent = emittedEvents.find(e => e.event === 'media-added');
      expect(mediaEvent).toBeDefined();
      expect(mediaEvent!.data.media.contentType).toBe('image/svg+xml');
    });

    it('should preserve text/plain content type', () => {
      // Arrange
      const plainEvent: RenderMediaEvent = {
        type: 'render_media',
        session_id: 'test-session',
        role: 'assistant',
        content_type: 'text/plain',
        content: 'Plain text content',
        foreign_content: false
      };

      // Act
      processor.processEvent(plainEvent);

      // Assert
      const mediaEvent = emittedEvents.find(e => e.event === 'media-added');
      expect(mediaEvent).toBeDefined();
      expect(mediaEvent!.data.media.contentType).toBe('text/plain');
    });
  });

  describe('HTML Content with Foreign Flag', () => {
    it('should preserve foreign_content flag in metadata', () => {
      // Arrange
      const foreignHtmlEvent: RenderMediaEvent = {
        type: 'render_media',
        session_id: 'test-session',
        role: 'assistant',
        content_type: 'text/html',
        content: '<div>External content</div>',
        foreign_content: true  // Critical security flag
      };

      // Act
      processor.processEvent(foreignHtmlEvent);

      // Assert
      const mediaEvent = emittedEvents.find(e => e.event === 'media-added');
      expect(mediaEvent).toBeDefined();
      expect(mediaEvent!.data.media.metadata.foreign_content).toBe(true);
      expect(mediaEvent!.data.media.contentType).toBe('text/html');
    });

    it('should handle safe internal HTML content', () => {
      // Arrange
      const safeHtmlEvent: RenderMediaEvent = {
        type: 'render_media',
        session_id: 'test-session',
        role: 'assistant',
        content_type: 'text/html',
        content: '<div>Internal content</div>',
        foreign_content: false  // Safe internal content
      };

      // Act
      processor.processEvent(safeHtmlEvent);

      // Assert
      const mediaEvent = emittedEvents.find(e => e.event === 'media-added');
      expect(mediaEvent).toBeDefined();
      expect(mediaEvent!.data.media.metadata.foreign_content).toBe(false);
    });
  });

  describe('Missing Optional Fields', () => {
    it('should handle minimal event with only required fields', () => {
      // Arrange - Minimal valid event
      const minimalEvent: RenderMediaEvent = {
        type: 'render_media',
        session_id: 'test-session',
        role: 'assistant',
        content_type: 'text/plain',
        foreign_content: false
        // No content, url, name, sent_by_class, sent_by_function
      };

      // Act
      processor.processEvent(minimalEvent);

      // Assert - Should not error
      const mediaEvent = emittedEvents.find(e => e.event === 'media-added');
      expect(mediaEvent).toBeDefined();
      expect(mediaEvent!.data.media.contentType).toBe('text/plain');
      expect(mediaEvent!.data.media.content).toBeUndefined();
      expect(mediaEvent!.data.media.metadata.url).toBeUndefined();
      expect(mediaEvent!.data.media.metadata.name).toBeUndefined();
    });

    it('should handle event with all optional fields undefined', () => {
      // Arrange
      const event: RenderMediaEvent = {
        type: 'render_media',
        session_id: 'test-session',
        role: 'assistant',
        content_type: 'text/html',
        content: undefined,
        url: undefined,
        name: undefined,
        sent_by_class: undefined,
        sent_by_function: undefined,
        foreign_content: true
      };

      // Act
      processor.processEvent(event);

      // Assert
      const mediaEvent = emittedEvents.find(e => e.event === 'media-added');
      expect(mediaEvent).toBeDefined();
      expect(mediaEvent!.data.media.content).toBeUndefined();
      expect(mediaEvent!.data.media.metadata).toEqual({
        sent_by_class: undefined,
        sent_by_function: undefined,
        foreign_content: true,
        url: undefined,
        name: undefined
      });
    });
  });

  describe('URL-Only Media', () => {
    it('should handle media with URL but no content', () => {
      // Arrange
      const urlOnlyEvent: RenderMediaEvent = {
        type: 'render_media',
        session_id: 'test-session',
        role: 'assistant',
        content_type: 'image/png',
        url: 'https://example.com/image.png',
        foreign_content: true
        // No content field
      };

      // Act
      processor.processEvent(urlOnlyEvent);

      // Assert
      const mediaEvent = emittedEvents.find(e => e.event === 'media-added');
      expect(mediaEvent).toBeDefined();
      expect(mediaEvent!.data.media.contentType).toBe('image/png');
      expect(mediaEvent!.data.media.content).toBeUndefined();
      expect(mediaEvent!.data.media.metadata.url).toBe('https://example.com/image.png');
      expect(mediaEvent!.data.media.metadata.foreign_content).toBe(true);
    });

    it('should handle video with URL', () => {
      // Arrange
      const videoEvent: RenderMediaEvent = {
        type: 'render_media',
        session_id: 'test-session',
        role: 'assistant',
        content_type: 'video/mp4',
        url: 'https://example.com/video.mp4',
        name: 'Demo Video',
        foreign_content: false
      };

      // Act
      processor.processEvent(videoEvent);

      // Assert
      const mediaEvent = emittedEvents.find(e => e.event === 'media-added');
      expect(mediaEvent).toBeDefined();
      expect(mediaEvent!.data.media.contentType).toBe('video/mp4');
      expect(mediaEvent!.data.media.metadata.url).toBe('https://example.com/video.mp4');
      expect(mediaEvent!.data.media.metadata.name).toBe('Demo Video');
    });
  });

  describe('Tool Attribution', () => {
    it('should preserve tool attribution fields in metadata', () => {
      // Arrange
      const toolEvent: RenderMediaEvent = {
        type: 'render_media',
        session_id: 'test-session',
        role: 'assistant',
        content_type: 'text/html',
        content: '<div>Chart output</div>',
        sent_by_class: 'DataVisualizationTool',
        sent_by_function: 'generateChart',
        foreign_content: false
      };

      // Act
      processor.processEvent(toolEvent);

      // Assert
      const mediaEvent = emittedEvents.find(e => e.event === 'media-added');
      expect(mediaEvent).toBeDefined();
      expect(mediaEvent!.data.media.metadata.sent_by_class).toBe('DataVisualizationTool');
      expect(mediaEvent!.data.media.metadata.sent_by_function).toBe('generateChart');
    });

    it('should handle partial tool attribution', () => {
      // Arrange - Only sent_by_class provided
      const partialToolEvent: RenderMediaEvent = {
        type: 'render_media',
        session_id: 'test-session',
        role: 'assistant',
        content_type: 'text/html',
        content: '<div>Output</div>',
        sent_by_class: 'AnalysisTool',
        // sent_by_function not provided
        foreign_content: false
      };

      // Act
      processor.processEvent(partialToolEvent);

      // Assert
      const mediaEvent = emittedEvents.find(e => e.event === 'media-added');
      expect(mediaEvent).toBeDefined();
      expect(mediaEvent!.data.media.metadata.sent_by_class).toBe('AnalysisTool');
      expect(mediaEvent!.data.media.metadata.sent_by_function).toBeUndefined();
    });
  });

  describe('SVG Content Type', () => {
    it('should handle SVG with correct content type', () => {
      // Arrange
      const svgEvent: RenderMediaEvent = {
        type: 'render_media',
        session_id: 'test-session',
        role: 'assistant',
        content_type: 'image/svg+xml',
        content: '<svg xmlns="http://www.w3.org/2000/svg"><rect width="100" height="100"/></svg>',
        sent_by_class: 'GraphRenderer',
        sent_by_function: 'renderGraph',
        foreign_content: false
      };

      // Act
      processor.processEvent(svgEvent);

      // Assert
      const mediaEvent = emittedEvents.find(e => e.event === 'media-added');
      expect(mediaEvent).toBeDefined();
      expect(mediaEvent!.data.media.contentType).toBe('image/svg+xml');
      expect(mediaEvent!.data.media.content).toContain('<svg');
    });

    it('should handle foreign SVG content', () => {
      // Arrange
      const foreignSvgEvent: RenderMediaEvent = {
        type: 'render_media',
        session_id: 'test-session',
        role: 'assistant',
        content_type: 'image/svg+xml',
        content: '<svg><script>alert("XSS")</script></svg>',
        url: 'https://external.com/chart.svg',
        foreign_content: true  // Potentially dangerous
      };

      // Act
      processor.processEvent(foreignSvgEvent);

      // Assert
      const mediaEvent = emittedEvents.find(e => e.event === 'media-added');
      expect(mediaEvent).toBeDefined();
      expect(mediaEvent!.data.media.contentType).toBe('image/svg+xml');
      expect(mediaEvent!.data.media.metadata.foreign_content).toBe(true);
      expect(mediaEvent!.data.media.metadata.url).toBe('https://external.com/chart.svg');
    });
  });

  describe('Event Structure Preservation', () => {
    it('should preserve all server event fields in emitted event', () => {
      // Arrange
      const completeEvent: RenderMediaEvent = {
        type: 'render_media',
        session_id: 'test-session-123',
        role: 'assistant',
        content_type: 'application/json',
        content: '{"data": "test"}',
        url: 'https://api.example.com/data',
        name: 'API Response',
        sent_by_class: 'APIClient',
        sent_by_function: 'fetchData',
        foreign_content: true
      };

      // Act
      processor.processEvent(completeEvent);

      // Assert - All fields should be preserved
      const mediaEvent = emittedEvents.find(e => e.event === 'media-added');
      expect(mediaEvent).toBeDefined();
      
      const media = mediaEvent!.data.media;
      expect(media.role).toBe('assistant');
      expect(media.type).toBe('media');
      expect(media.content).toBe('{"data": "test"}');
      expect(media.contentType).toBe('application/json');
      expect(media.status).toBe('complete');
      
      // All metadata fields preserved
      expect(media.metadata).toEqual({
        sent_by_class: 'APIClient',
        sent_by_function: 'fetchData',
        foreign_content: true,
        url: 'https://api.example.com/data',
        name: 'API Response'
      });
    });

    it('should generate consistent ID for React key prop', () => {
      // Arrange
      const event: RenderMediaEvent = {
        type: 'render_media',
        session_id: 'test-session',
        role: 'assistant',
        content_type: 'text/plain',
        content: 'Test',
        foreign_content: false
      };

      // Act - Process same event twice
      processor.processEvent(event);
      processor.processEvent(event);

      // Assert - Different IDs generated
      const mediaEvents = emittedEvents.filter(e => e.event === 'media-added');
      expect(mediaEvents).toHaveLength(2);
      
      const id1 = mediaEvents[0].data.media.id;
      const id2 = mediaEvents[1].data.media.id;
      
      expect(id1).toMatch(/^media_\d+_[a-z0-9]+$/);
      expect(id2).toMatch(/^media_\d+_[a-z0-9]+$/);
      expect(id1).not.toBe(id2); // Different IDs
    });
  });

  describe('Edge Cases and Error Handling', () => {
    it('should handle empty content string', () => {
      // Arrange
      const emptyContentEvent: RenderMediaEvent = {
        type: 'render_media',
        session_id: 'test-session',
        role: 'assistant',
        content_type: 'text/plain',
        content: '',
        foreign_content: false
      };

      // Act
      processor.processEvent(emptyContentEvent);

      // Assert
      const mediaEvent = emittedEvents.find(e => e.event === 'media-added');
      expect(mediaEvent).toBeDefined();
      expect(mediaEvent!.data.media.content).toBe('');
    });

    it('should handle various MIME types correctly', () => {
      const mimeTypes = [
        'application/pdf',
        'application/javascript',
        'application/json',
        'text/css',
        'text/csv',
        'image/jpeg',
        'image/gif',
        'audio/mpeg',
        'video/webm'
      ];

      mimeTypes.forEach(mimeType => {
        // Arrange
        const event: RenderMediaEvent = {
          type: 'render_media',
          session_id: 'test-session',
          role: 'assistant',
          content_type: mimeType,
          foreign_content: false
        };

        // Act
        processor.processEvent(event);

        // Assert
        const mediaEvent = emittedEvents.find(
          e => e.event === 'media-added' && e.data.media.contentType === mimeType
        );
        expect(mediaEvent).toBeDefined();
        expect(mediaEvent!.data.media.contentType).toBe(mimeType);
      });
    });

    it('should handle special characters in content', () => {
      // Arrange
      const specialContentEvent: RenderMediaEvent = {
        type: 'render_media',
        session_id: 'test-session',
        role: 'assistant',
        content_type: 'text/html',
        content: '<div>Special chars: " \' & < > © ™ €</div>',
        foreign_content: false
      };

      // Act
      processor.processEvent(specialContentEvent);

      // Assert
      const mediaEvent = emittedEvents.find(e => e.event === 'media-added');
      expect(mediaEvent).toBeDefined();
      expect(mediaEvent!.data.media.content).toBe('<div>Special chars: " \' & < > © ™ €</div>');
    });
  });

  describe('ChatSessionManager Integration', () => {
    it('should emit media-added event with correct session ID', () => {
      // Arrange
      const sessionId = 'specific-session-id';
      const event: RenderMediaEvent = {
        type: 'render_media',
        session_id: sessionId,
        role: 'assistant',
        content_type: 'text/plain',
        content: 'Test',
        foreign_content: false
      };

      // Act
      processor.processEvent(event);

      // Assert
      const mediaEvent = emittedEvents.find(e => e.event === 'media-added');
      expect(mediaEvent).toBeDefined();
      expect(mediaEvent!.data.sessionId).toBe(sessionId);
    });

    it('should emit event even without active session', () => {
      // Arrange - No session set
      const event: RenderMediaEvent = {
        type: 'render_media',
        session_id: 'orphan-session',
        role: 'assistant',
        content_type: 'text/plain',
        content: 'Orphan content',
        foreign_content: false
      };

      // Act
      processor.processEvent(event);

      // Assert
      const mediaEvent = emittedEvents.find(e => e.event === 'media-added');
      expect(mediaEvent).toBeDefined();
      expect(mediaEvent!.data.sessionId).toBe('orphan-session');
    });
  });
});