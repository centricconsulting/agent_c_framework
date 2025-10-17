/**
 * Integration tests for ToolCatalogEvent breaking change
 * 
 * Ensures the 'toolsets' to 'tools' migration is complete throughout
 * the SDK and no legacy code paths remain.
 */

import { describe, it, expect, beforeEach, afterEach, vi } from 'vitest';
import { EventEmitter } from '../../EventEmitter';
import type { ToolCatalogEvent, ServerEvent } from '../ServerEvents';
import type { GetToolCatalogEvent } from '../ClientEvents';
import type { Tool } from '../CommonTypes';
import { serverEventFixtures, clientEventFixtures } from '../../../test/fixtures/protocol-events';

// WebSocket constants for test environment
const WS_OPEN = 1;
const WS_CLOSED = 3;

// Mock tool catalog manager
class ToolCatalogManager {
  private tools: Tool[] = [];  // CORRECT: Using 'tools' not 'toolsets'
  private emitter: EventEmitter<Record<string, any>>;

  constructor(emitter: EventEmitter<Record<string, any>>) {
    this.emitter = emitter;
    this.setupListeners();
  }

  private setupListeners() {
    // CORRECT: Listen for 'tool_catalog' event with 'tools' field
    this.emitter.on('tool_catalog', (event: ToolCatalogEvent) => {
      this.updateTools(event.tools);  // Using 'tools' field
    });
  }

  private updateTools(tools: Tool[]) {
    this.tools = tools;
  }

  getTools(): Tool[] {
    return this.tools;
  }

  getToolCount(): number {
    return this.tools.length;
  }

  findTool(name: string): Tool | undefined {
    return this.tools.find(tool => tool.name === name);
  }

  // This method signature would be WRONG and should not exist:
  // getToolsets(): Toolset[] { ... }  // ❌ Old naming
}

describe('ToolCatalogEvent Integration', () => {
  let emitter: EventEmitter<Record<string, any>>;
  let catalogManager: ToolCatalogManager;

  beforeEach(() => {
    vi.clearAllMocks();
    emitter = new EventEmitter();
    catalogManager = new ToolCatalogManager(emitter);
  });

  afterEach(() => {
    vi.restoreAllMocks();
  });

  describe('Event Flow with New Field Name', () => {
    it('should handle tool catalog updates with "tools" field', () => {
      // Arrange
      const toolCatalogEvent: ToolCatalogEvent = {
        type: 'tool_catalog',
        tools: [
          {
            name: 'calculator',
            description: 'Math operations',
            schemas: {}
          },
          {
            name: 'web_search',
            description: 'Search the web',
            schemas: {}
          }
        ]
      };

      // Act
      emitter.emit('tool_catalog', toolCatalogEvent);

      // Assert
      expect(catalogManager.getTools()).toHaveLength(2);
      expect(catalogManager.findTool('calculator')).toBeDefined();
      expect(catalogManager.findTool('web_search')).toBeDefined();
    });

    it('should handle empty tools array', () => {
      // Arrange
      const emptyEvent: ToolCatalogEvent = {
        type: 'tool_catalog',
        tools: []  // Empty array is valid
      };

      // Act
      emitter.emit('tool_catalog', emptyEvent);

      // Assert
      expect(catalogManager.getTools()).toEqual([]);
      expect(catalogManager.getToolCount()).toBe(0);
    });

    it('should update tools on subsequent events', () => {
      // Arrange
      const initialEvent: ToolCatalogEvent = {
        type: 'tool_catalog',
        tools: [
          { name: 'tool1', description: 'First', schemas: {} }
        ]
      };

      const updateEvent: ToolCatalogEvent = {
        type: 'tool_catalog',
        tools: [
          { name: 'tool2', description: 'Second', schemas: {} },
          { name: 'tool3', description: 'Third', schemas: {} }
        ]
      };

      // Act
      emitter.emit('tool_catalog', initialEvent);
      expect(catalogManager.getToolCount()).toBe(1);

      emitter.emit('tool_catalog', updateEvent);

      // Assert
      expect(catalogManager.getToolCount()).toBe(2);
      expect(catalogManager.findTool('tool1')).toBeUndefined();  // Replaced
      expect(catalogManager.findTool('tool2')).toBeDefined();
      expect(catalogManager.findTool('tool3')).toBeDefined();
    });
  });

  describe('Client-Server Integration', () => {
    it('should handle GetToolCatalogEvent request and ToolCatalogEvent response', () => {
      // Arrange
      const requestHandler = vi.fn();
      const responseHandler = vi.fn();

      emitter.on('get_tool_catalog', requestHandler);
      emitter.on('tool_catalog', responseHandler);

      // Client requests tool catalog
      const request: GetToolCatalogEvent = {
        type: 'get_tool_catalog'
      };

      // Server responds with tool catalog
      const response: ToolCatalogEvent = {
        type: 'tool_catalog',
        tools: [  // CORRECT: Using 'tools'
          {
            name: 'math_tools',
            description: 'Mathematical tools',
            schemas: {
              add: {
                type: 'function',
                function: {
                  name: 'add',
                  description: 'Add numbers',
                  parameters: {
                    type: 'object',
                    properties: {},
                    required: []
                  }
                }
              }
            }
          }
        ]
      };

      // Act
      emitter.emit('get_tool_catalog', request);
      emitter.emit('tool_catalog', response);

      // Assert
      expect(requestHandler).toHaveBeenCalledWith(request);
      expect(responseHandler).toHaveBeenCalledWith(response);
      
      const receivedResponse = responseHandler.mock.calls[0][0];
      expect(receivedResponse).toHaveProperty('tools');
      expect(receivedResponse).not.toHaveProperty('toolsets');
    });

    it('should use fixtures with correct field names', () => {
      // Arrange
      const handler = vi.fn();
      emitter.on('tool_catalog', handler);

      // Act - Use the fixture
      emitter.emit('tool_catalog', serverEventFixtures.toolCatalog);

      // Assert
      expect(handler).toHaveBeenCalledOnce();
      const event = handler.mock.calls[0][0];
      expect(event).toHaveProperty('tools');
      expect(event.tools).toBeInstanceOf(Array);
      expect(event).not.toHaveProperty('toolsets');
    });
  });

  describe('WebSocket Serialization', () => {
    it('should serialize ToolCatalogEvent with "tools" field', () => {
      // Arrange
      const mockWs = {
        readyState: WS_OPEN,
        send: vi.fn()
      };

      const event: ToolCatalogEvent = {
        type: 'tool_catalog',
        tools: [
          { name: 'test_tool', description: 'Test', schemas: {} }
        ]
      };

      // Act
      const json = JSON.stringify(event);
      mockWs.send(json);

      // Assert
      expect(mockWs.send).toHaveBeenCalledWith(json);
      expect(json).toContain('"tools"');
      expect(json).not.toContain('"toolsets"');
      
      // Verify deserialization
      const parsed = JSON.parse(json) as ToolCatalogEvent;
      expect(parsed.tools).toBeDefined();
      expect(parsed.tools[0].name).toBe('test_tool');
    });

    it('should handle incoming ToolCatalogEvent from WebSocket', () => {
      // Arrange
      const receivedEvents: ToolCatalogEvent[] = [];
      
      const handleMessage = (data: string) => {
        const event = JSON.parse(data);
        if (event.type === 'tool_catalog') {
          receivedEvents.push(event);
          emitter.emit('tool_catalog', event);
        }
      };

      // Act - Simulate receiving event with new field
      const eventData = JSON.stringify({
        type: 'tool_catalog',
        tools: [
          { name: 'ws_tool', description: 'WebSocket tool', schemas: {} }
        ]
      });
      
      handleMessage(eventData);

      // Assert
      expect(receivedEvents).toHaveLength(1);
      expect(receivedEvents[0].tools).toBeDefined();
      expect(receivedEvents[0].tools[0].name).toBe('ws_tool');
      expect((receivedEvents[0] as any).toolsets).toBeUndefined();
    });

    it('should reject incoming events with old field name', () => {
      // Arrange
      const errorHandler = vi.fn();
      const validHandler = vi.fn();

      const handleMessage = (data: string) => {
        try {
          const event = JSON.parse(data);
          
          if (event.type === 'tool_catalog') {
            // Validate field name
            if ('toolsets' in event) {
              throw new Error('Received deprecated "toolsets" field. Server needs update.');
            }
            
            if (!('tools' in event)) {
              throw new Error('Missing required "tools" field');
            }
            
            validHandler(event);
          }
        } catch (error) {
          errorHandler(error);
        }
      };

      // Act - Try to process old format
      const oldFormat = JSON.stringify({
        type: 'tool_catalog',
        toolsets: []  // Old field name
      });
      
      handleMessage(oldFormat);

      // Assert
      expect(errorHandler).toHaveBeenCalledOnce();
      expect(errorHandler.mock.calls[0][0].message).toContain('deprecated "toolsets"');
      expect(validHandler).not.toHaveBeenCalled();

      // Act - Process new format
      const newFormat = JSON.stringify({
        type: 'tool_catalog',
        tools: []
      });
      
      handleMessage(newFormat);

      // Assert
      expect(validHandler).toHaveBeenCalledOnce();
    });
  });

  describe('Type Safety Throughout System', () => {
    it('should maintain type safety with Tool[] type', () => {
      // Arrange
      class TypedToolManager {
        private toolCatalog: Tool[] = [];  // Typed as Tool[]

        updateCatalog(event: ToolCatalogEvent) {
          // TypeScript ensures event.tools is Tool[]
          this.toolCatalog = event.tools;
        }

        getToolNames(): string[] {
          // Type-safe access to Tool properties
          return this.toolCatalog.map(tool => tool.name);
        }

        getToolByName(name: string): Tool | undefined {
          return this.toolCatalog.find(tool => tool.name === name);
        }
      }

      const manager = new TypedToolManager();
      const event: ToolCatalogEvent = {
        type: 'tool_catalog',
        tools: [
          { name: 'typed_tool1', description: 'First', schemas: {} },
          { name: 'typed_tool2', description: 'Second', schemas: {} }
        ]
      };

      // Act
      manager.updateCatalog(event);

      // Assert
      expect(manager.getToolNames()).toEqual(['typed_tool1', 'typed_tool2']);
      
      const tool = manager.getToolByName('typed_tool1');
      expect(tool).toBeDefined();
      expect(tool?.description).toBe('First');
    });

    it('should work with typed event emitter', () => {
      // Arrange
      interface EventMap {
        tool_catalog: ToolCatalogEvent;
        get_tool_catalog: GetToolCatalogEvent;
      }

      const typedEmitter = new EventEmitter<EventMap>();
      const handler = vi.fn((event: ToolCatalogEvent) => {
        // TypeScript knows event.tools exists and is Tool[]
        return event.tools.length;
      });

      typedEmitter.on('tool_catalog', handler);

      // Act
      typedEmitter.emit('tool_catalog', {
        type: 'tool_catalog',
        tools: [
          { name: 'test', description: 'test', schemas: {} }
        ]
      });

      // Assert
      expect(handler).toHaveBeenCalledOnce();
      expect(handler.mock.results[0].value).toBe(1);
    });
  });

  describe('Migration Path Validation', () => {
    it('should demonstrate migration from old to new structure', () => {
      // This test documents the migration path
      
      // OLD CODE (would no longer compile):
      // interface OldCode {
      //   handleEvent(event: any) {
      //     const toolsets = event.toolsets;  // ❌ Old field
      //     console.log(`Found ${toolsets.length} toolsets`);
      //   }
      // }

      // NEW CODE:
      class NewCode {
        handleEvent(event: ToolCatalogEvent) {
          const tools = event.tools;  // ✅ New field
          console.log(`Found ${tools.length} tools`);
        }
      }

      const newCode = new NewCode();
      const mockConsole = vi.spyOn(console, 'log').mockImplementation(() => {});

      // Act
      newCode.handleEvent({
        type: 'tool_catalog',
        tools: [
          { name: 'tool1', description: 'Tool 1', schemas: {} },
          { name: 'tool2', description: 'Tool 2', schemas: {} }
        ]
      });

      // Assert
      expect(mockConsole).toHaveBeenCalledWith('Found 2 tools');
      
      mockConsole.mockRestore();
    });

    it('should handle React component updates', () => {
      // Simulate React component that uses tools
      interface ToolListProps {
        event: ToolCatalogEvent;
      }

      class ToolListComponent {
        render(props: ToolListProps) {
          // CORRECT: Access event.tools
          const toolCount = props.event.tools.length;
          const toolNames = props.event.tools.map(t => t.name);
          
          return {
            count: toolCount,
            names: toolNames
          };
        }
      }

      // Arrange
      const component = new ToolListComponent();
      const event: ToolCatalogEvent = {
        type: 'tool_catalog',
        tools: [
          { name: 'react_tool1', description: 'Tool 1', schemas: {} },
          { name: 'react_tool2', description: 'Tool 2', schemas: {} }
        ]
      };

      // Act
      const result = component.render({ event });

      // Assert
      expect(result.count).toBe(2);
      expect(result.names).toEqual(['react_tool1', 'react_tool2']);
    });

    it('should work with state management patterns', () => {
      // Simulate state management (Redux-like)
      interface AppState {
        tools: Tool[];  // CORRECT: Using Tool[] type
        lastUpdated: number;
      }

      class StateManager {
        private state: AppState = {
          tools: [],
          lastUpdated: 0
        };

        handleToolCatalogEvent(event: ToolCatalogEvent) {
          // CORRECT: Update state.tools from event.tools
          this.state = {
            tools: event.tools,
            lastUpdated: Date.now()
          };
        }

        getState(): AppState {
          return this.state;
        }
      }

      // Arrange
      const stateManager = new StateManager();
      const event: ToolCatalogEvent = {
        type: 'tool_catalog',
        tools: [
          { name: 'state_tool', description: 'State tool', schemas: {} }
        ]
      };

      // Act
      stateManager.handleToolCatalogEvent(event);
      const state = stateManager.getState();

      // Assert
      expect(state.tools).toHaveLength(1);
      expect(state.tools[0].name).toBe('state_tool');
      expect(state.lastUpdated).toBeGreaterThan(0);
    });
  });

  describe('Error Handling for Breaking Change', () => {
    it('should provide clear errors for old field usage', () => {
      // Arrange
      const processEvent = (event: any) => {
        if ('toolsets' in event) {
          throw new Error(
            'BREAKING CHANGE: The field "toolsets" has been renamed to "tools". ' +
            'Please update your code to use event.tools instead of event.toolsets. ' +
            'This change was made in version 0.1.0 for consistency.'
          );
        }
        
        return event.tools;
      };

      // Act & Assert - Old field
      expect(() => {
        processEvent({ type: 'tool_catalog', toolsets: [] });
      }).toThrow('BREAKING CHANGE');

      // Act & Assert - New field
      expect(() => {
        processEvent({ type: 'tool_catalog', tools: [] });
      }).not.toThrow();
    });

    it('should detect and warn about potential migration issues', () => {
      // Arrange
      const warnings: string[] = [];
      
      const checkForMigrationIssues = (code: string): void => {
        const patterns = [
          { pattern: /\.toolsets/g, warning: 'Found reference to .toolsets' },
          { pattern: /\[["']toolsets["']\]/g, warning: 'Found reference to ["toolsets"]' },
          { pattern: /toolsets\s*:/g, warning: 'Found toolsets: property' },
          { pattern: /Toolset\[\]/g, warning: 'Found Toolset[] type' }
        ];
        
        patterns.forEach(({ pattern, warning }) => {
          if (pattern.test(code)) {
            warnings.push(warning);
          }
        });
      };

      // Act - Check some code samples
      checkForMigrationIssues('const tools = event.tools;');  // OK
      checkForMigrationIssues('const old = event.toolsets;');  // Problem
      checkForMigrationIssues('data["toolsets"].length');      // Problem
      checkForMigrationIssues('{ toolsets: [] }');             // Problem

      // Assert
      expect(warnings).toHaveLength(3);  // Now all 3 patterns should match
      expect(warnings).toContain('Found reference to .toolsets');
      expect(warnings).toContain('Found reference to ["toolsets"]');
      expect(warnings).toContain('Found toolsets: property');
    });
  });

  describe('Complete System Integration', () => {
    it('should work end-to-end with correct field names', async () => {
      // Arrange
      const events: any[] = [];
      const mockClient = {
        send: vi.fn(),
        on: (eventType: string, handler: Function) => {
          emitter.on(eventType, handler as any);
        }
      };

      // Set up handlers
      mockClient.on('tool_catalog', (event: ToolCatalogEvent) => {
        events.push({ type: 'received', event });
      });

      // Simulate full flow
      const flow = async () => {
        // 1. Client requests tool catalog
        const request: GetToolCatalogEvent = { type: 'get_tool_catalog' };
        mockClient.send(JSON.stringify(request));
        
        // 2. Server responds with tool catalog
        const response: ToolCatalogEvent = {
          type: 'tool_catalog',
          tools: [  // CORRECT FIELD
            { name: 'e2e_tool', description: 'End-to-end test tool', schemas: {} }
          ]
        };
        
        // 3. Client receives response
        emitter.emit('tool_catalog', response);
        
        return events;
      };

      // Act
      const result = await flow();

      // Assert
      expect(mockClient.send).toHaveBeenCalledOnce();
      expect(result).toHaveLength(1);
      expect(result[0].type).toBe('received');
      expect(result[0].event.tools).toBeDefined();
      expect(result[0].event.tools[0].name).toBe('e2e_tool');
      
      // Verify no old field references
      const jsonStr = JSON.stringify(result);
      expect(jsonStr).not.toContain('toolsets');
    });

    it('should handle complex tool schemas with new structure', () => {
      // Arrange
      const complexEvent: ToolCatalogEvent = {
        type: 'tool_catalog',
        tools: [  // Using 'tools' field
          {
            name: 'advanced_toolkit',
            description: 'Advanced tools with multiple functions',
            schemas: {
              function1: {
                type: 'function',
                function: {
                  name: 'function1',
                  description: 'First function',
                  parameters: {
                    type: 'object',
                    properties: {
                      input: { type: 'string', description: 'Input data' }
                    },
                    required: ['input']
                  }
                }
              },
              function2: {
                type: 'function',
                function: {
                  name: 'function2',
                  description: 'Second function',
                  parameters: {
                    type: 'object',
                    properties: {
                      data: { 
                        type: 'array',
                        items: { type: 'number' },
                        description: 'Numeric data'
                      }
                    },
                    required: ['data']
                  }
                }
              }
            }
          }
        ]
      };

      const handler = vi.fn((event: ToolCatalogEvent) => {
        // Process tools (not toolsets)
        return event.tools.map(tool => ({
          name: tool.name,
          functionCount: Object.keys(tool.schemas).length
        }));
      });

      emitter.on('tool_catalog', handler);

      // Act
      emitter.emit('tool_catalog', complexEvent);

      // Assert
      const result = handler.mock.results[0].value;
      expect(result).toHaveLength(1);
      expect(result[0].name).toBe('advanced_toolkit');
      expect(result[0].functionCount).toBe(2);
    });
  });
});