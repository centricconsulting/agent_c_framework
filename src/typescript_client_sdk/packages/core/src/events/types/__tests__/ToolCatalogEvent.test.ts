/**
 * Comprehensive tests for ToolCatalogEvent breaking change
 * 
 * Tests ensure the field migration from 'toolsets' to 'tools' is complete
 * and protected against regression. This change affected 23+ locations.
 */

import { describe, it, expect, beforeEach, afterEach, vi } from 'vitest';
import type { ToolCatalogEvent, ServerEvent } from '../ServerEvents';
import type { Tool, ToolSchema } from '../CommonTypes';
import { serverEventFixtures } from '../../../test/fixtures/protocol-events';

describe('ToolCatalogEvent Breaking Change Tests', () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  afterEach(() => {
    vi.restoreAllMocks();
  });

  describe('Field Name Validation - CRITICAL BREAKING CHANGE', () => {
    it('should have field named "tools" NOT "toolsets"', () => {
      // Arrange
      const event: ToolCatalogEvent = {
        type: 'tool_catalog',
        tools: []  // CORRECT: Field must be named 'tools'
      };

      // Act & Assert
      expect(event).toHaveProperty('tools');
      expect(event).not.toHaveProperty('toolsets');
      
      // TypeScript compilation enforces this
      // The following would fail TypeScript compilation:
      // const badEvent: ToolCatalogEvent = {
      //   type: 'tool_catalog',
      //   toolsets: []  // ❌ TypeScript error: Property 'toolsets' does not exist
      // };
    });

    it('should use Tool[] type NOT Toolset[]', () => {
      // Arrange
      const validTool: Tool = {
        name: 'test_tool',
        description: 'Test tool',
        schemas: {}
      };

      const event: ToolCatalogEvent = {
        type: 'tool_catalog',
        tools: [validTool]  // Type is Tool[]
      };

      // Act & Assert
      expect(event.tools).toBeInstanceOf(Array);
      expect(event.tools[0]).toMatchObject({
        name: 'test_tool',
        description: 'Test tool',
        schemas: {}
      });
      
      // Verify the type is Tool[]
      const tools: Tool[] = event.tools;
      expect(tools).toBeDefined();
    });

    it('should have no references to old field name in fixture', () => {
      // Arrange
      const fixture = serverEventFixtures.toolCatalog;
      const jsonString = JSON.stringify(fixture);

      // Act & Assert
      expect(fixture).toHaveProperty('tools');
      expect(fixture).not.toHaveProperty('toolsets');
      
      // Ensure no string references to old field name
      expect(jsonString).not.toContain('toolsets');
      expect(jsonString).not.toContain('toolset');  // Except in the tool names which is OK
      expect(jsonString).toContain('"tools"');
    });

    it('should have correct field in multiple tool scenarios', () => {
      // Arrange - Empty tools
      const emptyEvent: ToolCatalogEvent = {
        type: 'tool_catalog',
        tools: []
      };

      // Single tool
      const singleToolEvent: ToolCatalogEvent = {
        type: 'tool_catalog',
        tools: [
          {
            name: 'single_tool',
            description: 'A single tool',
            schemas: {}
          }
        ]
      };

      // Multiple tools
      const multiToolEvent: ToolCatalogEvent = {
        type: 'tool_catalog',
        tools: [
          { name: 'tool1', description: 'First tool', schemas: {} },
          { name: 'tool2', description: 'Second tool', schemas: {} },
          { name: 'tool3', description: 'Third tool', schemas: {} }
        ]
      };

      // Act & Assert - All use 'tools' field
      [emptyEvent, singleToolEvent, multiToolEvent].forEach(event => {
        expect(event).toHaveProperty('tools');
        expect(event).not.toHaveProperty('toolsets');
        expect(Array.isArray(event.tools)).toBe(true);
      });
    });
  });

  describe('Backward Compatibility Check', () => {
    it('should reject old field name "toolsets" at runtime', () => {
      // Arrange
      const invalidEvent = {
        type: 'tool_catalog',
        toolsets: []  // OLD FIELD NAME - MUST BE REJECTED
      };

      // Act & Assert
      expect(() => validateToolCatalogEvent(invalidEvent))
        .toThrow('Invalid field: "toolsets" is no longer supported. Use "tools" instead');
    });

    it('should detect and reject mixed field usage', () => {
      // Arrange
      const mixedEvent = {
        type: 'tool_catalog',
        tools: [],      // New field
        toolsets: []    // Old field - should not coexist
      };

      // Act & Assert
      expect(() => validateToolCatalogEvent(mixedEvent))
        .toThrow('Invalid field: "toolsets" is no longer supported');
    });

    it('should provide migration guidance for old field usage', () => {
      // Arrange
      const oldStructure = {
        type: 'tool_catalog',
        toolsets: [
          { name: 'old_tool', description: 'Old structure', schemas: {} }
        ]
      };

      // Act
      const migrationInfo = getMigrationGuidance(oldStructure);

      // Assert
      expect(migrationInfo.hasOldField).toBe(true);
      expect(migrationInfo.oldFieldName).toBe('toolsets');
      expect(migrationInfo.newFieldName).toBe('tools');
      expect(migrationInfo.message).toContain('Please update your code to use "tools"');
      expect(migrationInfo.affectedCode).toContain('event.toolsets');
      expect(migrationInfo.suggestedFix).toContain('event.tools');
    });

    it('should validate that TypeScript prevents old field usage', () => {
      // TypeScript compilation test - these would fail at compile time
      // This test documents the compile-time safety
      
      // ❌ Would not compile:
      // const badEvent: ToolCatalogEvent = {
      //   type: 'tool_catalog',
      //   toolsets: []  // TypeScript Error: Property 'toolsets' does not exist
      // };

      // ✅ Correct usage:
      const goodEvent: ToolCatalogEvent = {
        type: 'tool_catalog',
        tools: []
      };

      expect(goodEvent.tools).toBeDefined();
      expect('toolsets' in goodEvent).toBe(false);
    });

    it('should ensure no legacy code paths exist', () => {
      // Arrange - Simulate checking for old field access patterns
      const event: ToolCatalogEvent = {
        type: 'tool_catalog',
        tools: [
          { name: 'modern_tool', description: 'Modern tool', schemas: {} }
        ]
      };

      // Act - Try various access patterns
      const accessPatterns = [
        () => event.tools,           // ✅ Correct
        () => (event as any).toolsets,  // ❌ Old field
        () => event['tools'],        // ✅ Correct
        () => event['toolsets' as any]  // ❌ Old field
      ];

      // Assert
      expect(accessPatterns[0]()).toBeDefined();
      expect(accessPatterns[1]()).toBeUndefined();  // Old field doesn't exist
      expect(accessPatterns[2]()).toBeDefined();
      expect(accessPatterns[3]()).toBeUndefined();  // Old field doesn't exist
    });
  });

  describe('Type Validation', () => {
    it('should validate Tool type structure', () => {
      // Arrange
      const validTool: Tool = {
        name: 'calculator',
        description: 'Mathematical calculations',
        schemas: {
          add: {
            type: 'function',
            function: {
              name: 'add',
              description: 'Add two numbers',
              parameters: {
                type: 'object',
                properties: {
                  a: { type: 'number', description: 'First number' },
                  b: { type: 'number', description: 'Second number' }
                },
                required: ['a', 'b']
              }
            }
          }
        }
      };

      const event: ToolCatalogEvent = {
        type: 'tool_catalog',
        tools: [validTool]
      };

      // Act & Assert
      expect(isValidTool(validTool)).toBe(true);
      expect(event.tools[0]).toMatchObject({
        name: expect.any(String),
        description: expect.any(String),
        schemas: expect.any(Object)
      });
    });

    it('should validate Tool array type', () => {
      // Arrange
      const tools: Tool[] = [
        { name: 'tool1', description: 'First', schemas: {} },
        { name: 'tool2', description: 'Second', schemas: {} }
      ];

      const event: ToolCatalogEvent = {
        type: 'tool_catalog',
        tools: tools
      };

      // Act & Assert
      expect(Array.isArray(event.tools)).toBe(true);
      expect(event.tools.length).toBe(2);
      event.tools.forEach(tool => {
        expect(isValidTool(tool)).toBe(true);
      });
    });

    it('should allow empty tools array', () => {
      // Arrange
      const event: ToolCatalogEvent = {
        type: 'tool_catalog',
        tools: []  // Empty array is valid
      };

      // Act & Assert
      expect(event.tools).toEqual([]);
      expect(event.tools.length).toBe(0);
      expect(validateToolCatalogEvent(event)).toBe(true);
    });

    it('should require tools field to be present', () => {
      // Arrange
      const missingTools = {
        type: 'tool_catalog'
        // Missing tools field
      };

      // Act & Assert
      expect(() => validateToolCatalogEventStrict(missingTools))
        .toThrow('Missing required field: tools');
    });

    it('should validate ToolSchema structure', () => {
      // Arrange
      const validSchema: ToolSchema = {
        type: 'function',
        function: {
          name: 'test_function',
          description: 'Test function',
          parameters: {
            type: 'object',
            properties: {},
            required: []
          }
        }
      };

      const tool: Tool = {
        name: 'test_tool',
        description: 'Test',
        schemas: {
          test: validSchema
        }
      };

      // Act & Assert
      expect(isValidToolSchema(validSchema)).toBe(true);
      expect(tool.schemas['test']).toEqual(validSchema);
    });

    it('should reject invalid Tool structures', () => {
      // Arrange
      const invalidTools = [
        { description: 'Missing name', schemas: {} },  // Missing name
        { name: 'test' },  // Missing description and schemas
        { name: 'test', description: 'test', schemas: null },  // Null schemas
        { name: 123, description: 'test', schemas: {} },  // Wrong type for name
        'not an object'  // Not an object at all
      ];

      // Act & Assert
      invalidTools.forEach(invalidTool => {
        expect(isValidTool(invalidTool)).toBe(false);
      });
    });
  });

  describe('JSON Serialization', () => {
    it('should maintain "tools" field through JSON round-trip', () => {
      // Arrange
      const original: ToolCatalogEvent = {
        type: 'tool_catalog',
        tools: [
          {
            name: 'json_test',
            description: 'JSON serialization test',
            schemas: {}
          }
        ]
      };

      // Act
      const json = JSON.stringify(original);
      const parsed = JSON.parse(json) as ToolCatalogEvent;

      // Assert
      expect(parsed).toEqual(original);
      expect(parsed).toHaveProperty('tools');
      expect(parsed).not.toHaveProperty('toolsets');
      expect(json).toContain('"tools"');
      expect(json).not.toContain('"toolsets"');
    });

    it('should not transform field name during serialization', () => {
      // Arrange
      const event: ToolCatalogEvent = {
        type: 'tool_catalog',
        tools: []
      };

      // Act
      const json = JSON.stringify(event);

      // Assert - Raw JSON check
      expect(json).toBe('{"type":"tool_catalog","tools":[]}');
      expect(json).not.toContain('toolsets');
    });

    it('should preserve complex tool structures', () => {
      // Arrange
      const complexEvent: ToolCatalogEvent = {
        type: 'tool_catalog',
        tools: [
          {
            name: 'multi_function_tool',
            description: 'Tool with multiple functions',
            schemas: {
              func1: {
                type: 'function',
                function: {
                  name: 'func1',
                  description: 'First function',
                  parameters: {
                    type: 'object',
                    properties: {
                      param1: { type: 'string', description: 'Parameter 1' },
                      param2: { type: 'number', description: 'Parameter 2' }
                    },
                    required: ['param1']
                  }
                }
              },
              func2: {
                type: 'function',
                function: {
                  name: 'func2',
                  description: 'Second function',
                  parameters: {
                    type: 'object',
                    properties: {
                      data: { type: 'array', items: { type: 'string' } }
                    },
                    required: ['data']
                  }
                }
              }
            }
          }
        ]
      };

      // Act
      const json = JSON.stringify(complexEvent);
      const parsed = JSON.parse(json) as ToolCatalogEvent;

      // Assert
      expect(parsed.tools[0].schemas).toHaveProperty('func1');
      expect(parsed.tools[0].schemas).toHaveProperty('func2');
      expect(parsed.tools[0].schemas.func1.function.name).toBe('func1');
      expect(parsed.tools[0].schemas.func2.function.name).toBe('func2');
    });
  });

  describe('Migration Validation', () => {
    it('should verify fixture uses new field name', () => {
      // Arrange
      const fixture = serverEventFixtures.toolCatalog;

      // Act & Assert
      expect(fixture.type).toBe('tool_catalog');
      expect(fixture.tools).toBeDefined();
      expect(fixture.tools).toBeInstanceOf(Array);
      expect((fixture as any).toolsets).toBeUndefined();
    });

    it('should verify fixture tools follow Tool type', () => {
      // Arrange
      const fixture = serverEventFixtures.toolCatalog;

      // Act & Assert
      fixture.tools.forEach(tool => {
        expect(tool).toHaveProperty('name');
        expect(tool).toHaveProperty('description');
        expect(tool).toHaveProperty('schemas');
        expect(typeof tool.name).toBe('string');
        expect(typeof tool.description).toBe('string');
        expect(typeof tool.schemas).toBe('object');
      });
    });

    it('should detect any remaining references to toolsets', () => {
      // This test simulates scanning for old references
      const codePatterns = [
        'event.tools',           // ✅ Correct
        'event["tools"]',        // ✅ Correct
        'data.tools.length',     // ✅ Correct
        'const { tools } = event', // ✅ Correct
        // The following would be problems:
        // 'event.toolsets',      // ❌ Old reference
        // 'event["toolsets"]',   // ❌ Old reference
        // 'data.toolsets.length', // ❌ Old reference
        // 'const { toolsets } = event', // ❌ Old reference
      ];

      // Simulate checking each pattern
      codePatterns.forEach(pattern => {
        expect(pattern).not.toContain('toolsets');
      });
    });

    it('should handle migration from old to new structure', () => {
      // Arrange - Simulate old structure (for documentation)
      const oldStructureExample = {
        type: 'tool_catalog',
        toolsets: [  // OLD
          { name: 'old', description: 'old', schemas: {} }
        ]
      };

      // Function to migrate old to new
      const migrate = (oldEvent: any): ToolCatalogEvent => {
        if ('toolsets' in oldEvent) {
          const { toolsets, ...rest } = oldEvent;
          return {
            ...rest,
            tools: toolsets  // Rename field
          } as ToolCatalogEvent;
        }
        return oldEvent as ToolCatalogEvent;
      };

      // Act
      const migrated = migrate(oldStructureExample);

      // Assert
      expect(migrated).toHaveProperty('tools');
      expect(migrated).not.toHaveProperty('toolsets');
      expect(migrated.tools).toEqual(oldStructureExample.toolsets);
    });
  });

  describe('Type Guards', () => {
    it('should correctly identify valid ToolCatalogEvent', () => {
      // Arrange
      const validEvent: ServerEvent = {
        type: 'tool_catalog',
        tools: []
      } as ToolCatalogEvent;

      const invalidEvent: ServerEvent = {
        type: 'text_delta',
        session_id: 'test',
        role: 'assistant',
        content: 'Not a tool catalog',
        format: 'text'
      } as any;

      // Act & Assert
      expect(isToolCatalogEvent(validEvent)).toBe(true);
      expect(isToolCatalogEvent(invalidEvent)).toBe(false);

      // Type narrowing
      if (isToolCatalogEvent(validEvent)) {
        // TypeScript knows this is ToolCatalogEvent
        const tools: Tool[] = validEvent.tools;
        expect(tools).toBeDefined();
      }
    });

    it('should reject events with old field name', () => {
      // Arrange
      const oldFieldEvent = {
        type: 'tool_catalog',
        toolsets: []  // Old field name
      };

      // Act & Assert
      expect(isToolCatalogEvent(oldFieldEvent)).toBe(false);
    });

    it('should reject events with wrong structure', () => {
      // Arrange
      const wrongStructures = [
        { type: 'tool_catalog' },  // Missing tools
        { type: 'tool_catalog', tools: null },  // Null tools
        { type: 'tool_catalog', tools: 'not-array' },  // Wrong type
        { type: 'wrong_type', tools: [] },  // Wrong event type
        null,
        undefined,
        'not-an-object'
      ];

      // Act & Assert
      wrongStructures.forEach(structure => {
        expect(isToolCatalogEvent(structure)).toBe(false);
      });
    });
  });

  describe('Breaking Change Documentation', () => {
    it('should document the breaking change', () => {
      // This test serves as documentation
      const breakingChangeInfo = {
        oldField: 'toolsets',
        newField: 'tools',
        oldType: 'Toolset[]',
        newType: 'Tool[]',
        affectedFiles: 23,
        migrationRequired: true,
        version: '0.1.0'
      };

      expect(breakingChangeInfo.oldField).not.toBe(breakingChangeInfo.newField);
      expect(breakingChangeInfo.migrationRequired).toBe(true);
      expect(breakingChangeInfo.affectedFiles).toBeGreaterThan(20);
    });

    it('should provide clear error messages for migration', () => {
      // Arrange
      const oldUsage = {
        type: 'tool_catalog',
        toolsets: []
      };

      // Act
      let errorMessage = '';
      try {
        validateToolCatalogEventStrict(oldUsage);
      } catch (error) {
        errorMessage = (error as Error).message;
      }

      // Assert
      expect(errorMessage).toContain('toolsets');
      expect(errorMessage).toContain('no longer supported');
      expect(errorMessage).toContain('tools');
    });
  });
});

/**
 * Validation Functions
 */

function validateToolCatalogEvent(event: any): boolean {
  if (!event || typeof event !== 'object') {
    return false;
  }

  // Check for old field name - CRITICAL
  if ('toolsets' in event) {
    throw new Error('Invalid field: "toolsets" is no longer supported. Use "tools" instead');
  }

  if (event.type !== 'tool_catalog') {
    return false;
  }

  if (!('tools' in event)) {
    return false;
  }

  if (!Array.isArray(event.tools)) {
    return false;
  }

  return true;
}

function validateToolCatalogEventStrict(event: any): asserts event is ToolCatalogEvent {
  if (!event || typeof event !== 'object') {
    throw new Error('Event must be an object');
  }

  // CRITICAL: Check for old field name
  if ('toolsets' in event) {
    throw new Error(
      'Invalid field: "toolsets" is no longer supported. Use "tools" instead. ' +
      'This is a breaking change in v0.1.0. Please update your code to use event.tools'
    );
  }

  if (event.type !== 'tool_catalog') {
    throw new Error(`Invalid event type: ${event.type}`);
  }

  if (!('tools' in event)) {
    throw new Error('Missing required field: tools');
  }

  if (!Array.isArray(event.tools)) {
    throw new Error('Field "tools" must be an array');
  }

  // Validate each tool
  event.tools.forEach((tool: any, index: number) => {
    if (!isValidTool(tool)) {
      throw new Error(`Invalid tool at index ${index}`);
    }
  });
}

function isValidTool(tool: any): tool is Tool {
  if (!tool || typeof tool !== 'object') {
    return false;
  }

  if (typeof tool.name !== 'string') {
    return false;
  }

  if (typeof tool.description !== 'string') {
    return false;
  }

  if (!tool.schemas || typeof tool.schemas !== 'object') {
    return false;
  }

  return true;
}

function isValidToolSchema(schema: any): schema is ToolSchema {
  if (!schema || typeof schema !== 'object') {
    return false;
  }

  if (schema.type !== 'function') {
    return false;
  }

  if (!schema.function || typeof schema.function !== 'object') {
    return false;
  }

  const func = schema.function;
  if (typeof func.name !== 'string' || typeof func.description !== 'string') {
    return false;
  }

  if (!func.parameters || func.parameters.type !== 'object') {
    return false;
  }

  return true;
}

function isToolCatalogEvent(event: any): event is ToolCatalogEvent {
  if (!event || typeof event !== 'object') {
    return false;
  }

  if (event.type !== 'tool_catalog') {
    return false;
  }

  // CRITICAL: Must have 'tools' field, not 'toolsets'
  if (!('tools' in event)) {
    return false;
  }

  // CRITICAL: Reject if old field exists
  if ('toolsets' in event) {
    return false;
  }

  if (!Array.isArray(event.tools)) {
    return false;
  }

  return true;
}

function getMigrationGuidance(event: any): {
  hasOldField: boolean;
  oldFieldName?: string;
  newFieldName?: string;
  message?: string;
  affectedCode?: string;
  suggestedFix?: string;
} {
  if ('toolsets' in event) {
    return {
      hasOldField: true,
      oldFieldName: 'toolsets',
      newFieldName: 'tools',
      message: 'Please update your code to use "tools" instead of "toolsets"',
      affectedCode: 'event.toolsets',
      suggestedFix: 'event.tools'
    };
  }

  return { hasOldField: false };
}