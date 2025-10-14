/**
 * Comprehensive tests for AgentConfigurationV2 security fields
 * 
 * Tests the critical blocked_tool_patterns and allowed_tool_patterns fields
 * that control agent tool access for security purposes.
 */

import { describe, it, expect, beforeEach, afterEach, vi } from 'vitest';
import type { 
  AgentConfigurationV2, 
  AgentConfigurationV1,
  AgentConfiguration 
} from '../chat-session';

describe('AgentConfigurationV2 Security Fields', () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  afterEach(() => {
    vi.restoreAllMocks();
  });

  describe('Field Presence and Types', () => {
    it('should have optional blocked_tool_patterns field as string array', () => {
      // Arrange
      const config: AgentConfigurationV2 = {
        version: 2,
        name: 'SecureAgent',
        key: 'secure_agent',
        model_id: 'gpt-4',
        tools: ['calculator', 'web_search'],
        blocked_tool_patterns: ['dangerous_*', '*_admin', 'system_*'],
        persona: 'You are a secure assistant',
        category: ['secure', 'restricted']
      };

      // Act & Assert
      expect(config.blocked_tool_patterns).toBeDefined();
      expect(Array.isArray(config.blocked_tool_patterns)).toBe(true);
      expect(config.blocked_tool_patterns).toHaveLength(3);
      config.blocked_tool_patterns.forEach(pattern => {
        expect(typeof pattern).toBe('string');
      });
    });

    it('should have optional allowed_tool_patterns field as string array', () => {
      // Arrange
      const config: AgentConfigurationV2 = {
        version: 2,
        name: 'RestrictedAgent',
        key: 'restricted_agent',
        model_id: 'claude-3',
        tools: [],
        allowed_tool_patterns: ['safe_*', 'public_*', 'read_only_*'],
        persona: 'You are a restricted assistant',
        category: ['restricted']
      };

      // Act & Assert
      expect(config.allowed_tool_patterns).toBeDefined();
      expect(Array.isArray(config.allowed_tool_patterns)).toBe(true);
      expect(config.allowed_tool_patterns).toHaveLength(3);
      config.allowed_tool_patterns.forEach(pattern => {
        expect(typeof pattern).toBe('string');
      });
    });

    it('should allow both security fields to be undefined', () => {
      // Arrange
      const config: AgentConfigurationV2 = {
        version: 2,
        name: 'DefaultAgent',
        key: 'default_agent',
        model_id: 'gpt-3.5-turbo',
        tools: ['calculator'],
        persona: 'You are a helpful assistant',
        category: ['general']
        // blocked_tool_patterns and allowed_tool_patterns are undefined
      };

      // Act & Assert
      expect(config.blocked_tool_patterns).toBeUndefined();
      expect(config.allowed_tool_patterns).toBeUndefined();
      expect(config).not.toHaveProperty('blocked_tool_patterns');
      expect(config).not.toHaveProperty('allowed_tool_patterns');
    });

    it('should allow empty arrays for security fields', () => {
      // Arrange
      const config: AgentConfigurationV2 = {
        version: 2,
        name: 'EmptySecurityAgent',
        key: 'empty_security',
        model_id: 'gpt-4',
        tools: [],
        blocked_tool_patterns: [],
        allowed_tool_patterns: [],
        persona: 'Assistant with empty security arrays',
        category: ['test']
      };

      // Act & Assert
      expect(config.blocked_tool_patterns).toEqual([]);
      expect(config.allowed_tool_patterns).toEqual([]);
      expect(config.blocked_tool_patterns).toHaveLength(0);
      expect(config.allowed_tool_patterns).toHaveLength(0);
    });

    it('should allow populated arrays with multiple patterns', () => {
      // Arrange
      const config: AgentConfigurationV2 = {
        version: 2,
        name: 'ComplexSecurityAgent',
        key: 'complex_security',
        model_id: 'claude-3-opus',
        tools: ['tool1', 'tool2', 'tool3'],
        blocked_tool_patterns: [
          'admin_*',
          '*_delete',
          'system_*',
          'sudo_*',
          '*_write_*'
        ],
        allowed_tool_patterns: [
          'read_*',
          'list_*',
          'get_*',
          'public_*'
        ],
        persona: 'Highly restricted assistant',
        category: ['secure', 'limited']
      };

      // Act & Assert
      expect(config.blocked_tool_patterns).toHaveLength(5);
      expect(config.allowed_tool_patterns).toHaveLength(4);
    });

    it('should maintain distinction from V1 configuration', () => {
      // Arrange
      const v1Config: AgentConfigurationV1 = {
        version: 1,
        name: 'V1Agent',
        key: 'v1_agent',
        model_id: 'gpt-3.5',
        tools: ['calculator'],
        persona: 'V1 assistant',
        category: ['legacy']
        // V1 should NOT have security fields
      };

      const v2Config: AgentConfigurationV2 = {
        version: 2,
        name: 'V2Agent',
        key: 'v2_agent',
        model_id: 'gpt-4',
        tools: ['calculator'],
        blocked_tool_patterns: ['dangerous_*'],
        allowed_tool_patterns: ['safe_*'],
        persona: 'V2 assistant',
        category: ['modern']
      };

      // Act & Assert
      expect('blocked_tool_patterns' in v1Config).toBe(false);
      expect('allowed_tool_patterns' in v1Config).toBe(false);
      expect('blocked_tool_patterns' in v2Config).toBe(true);
      expect('allowed_tool_patterns' in v2Config).toBe(true);
    });
  });

  describe('Pattern Validation', () => {
    it('should accept valid glob patterns', () => {
      // Arrange
      const globPatterns = [
        '*',                  // Match all
        'tool_*',            // Prefix match
        '*_tool',            // Suffix match
        '*_tool_*',          // Contains match
        'exact_tool_name',   // Exact match
        'tool_[0-9]*',       // Character class
        'tool_{read,write}', // Alternation
        'tool_?.txt',        // Single character wildcard
        '**/*_admin'         // Nested path pattern
      ];

      const config: AgentConfigurationV2 = {
        version: 2,
        name: 'GlobPatternAgent',
        key: 'glob_patterns',
        model_id: 'gpt-4',
        tools: [],
        blocked_tool_patterns: globPatterns,
        persona: 'Test glob patterns',
        category: ['test']
      };

      // Act & Assert
      expect(config.blocked_tool_patterns).toEqual(globPatterns);
      config.blocked_tool_patterns.forEach(pattern => {
        expect(typeof pattern).toBe('string');
      });
    });

    it('should accept valid regex-like patterns', () => {
      // Arrange
      const regexPatterns = [
        '^admin_.*',         // Starts with
        '.*_admin$',         // Ends with
        '^exact_match$',     // Exact match
        '.*dangerous.*',     // Contains
        'tool_[a-z]+_v[0-9]+', // Complex pattern
        '(read|write)_.*',   // Alternation
        'tool_\\d+',         // Escaped digits
        '[A-Z][a-z]*Tool'    // Mixed case pattern
      ];

      const config: AgentConfigurationV2 = {
        version: 2,
        name: 'RegexPatternAgent',
        key: 'regex_patterns',
        model_id: 'claude-3',
        tools: [],
        allowed_tool_patterns: regexPatterns,
        persona: 'Test regex patterns',
        category: ['test']
      };

      // Act & Assert
      expect(config.allowed_tool_patterns).toEqual(regexPatterns);
    });

    it('should handle empty strings in pattern arrays', () => {
      // Arrange
      const config: AgentConfigurationV2 = {
        version: 2,
        name: 'EmptyStringPatternAgent',
        key: 'empty_string_patterns',
        model_id: 'gpt-4',
        tools: [],
        blocked_tool_patterns: ['', 'valid_pattern', ''],
        allowed_tool_patterns: ['', '', ''],
        persona: 'Test empty strings',
        category: ['test']
      };

      // Act & Assert
      expect(config.blocked_tool_patterns).toContain('');
      expect(config.blocked_tool_patterns).toContain('valid_pattern');
      expect(config.allowed_tool_patterns.every(p => p === '')).toBe(true);
    });

    it('should handle special characters in patterns', () => {
      // Arrange
      const specialPatterns = [
        'tool-with-dash',
        'tool_with_underscore',
        'tool.with.dots',
        'tool@with@at',
        'tool#with#hash',
        'tool$with$dollar',
        'tool%with%percent',
        'tool&with&ampersand',
        'tool(with)parens',
        'tool[with]brackets',
        'tool{with}braces',
        'tool|with|pipe',
        'tool\\with\\backslash',
        'tool/with/slash',
        'tool:with:colon',
        'tool;with;semicolon',
        'tool"with"quotes',
        "tool'with'apostrophe",
        'tool<with>angles',
        'tool=with=equals',
        'tool+with+plus',
        'tool~with~tilde',
        'tool`with`backtick',
        'tool!with!exclamation',
        'tool?with?question',
        'tool,with,comma',
        'tool with spaces'
      ];

      const config: AgentConfigurationV2 = {
        version: 2,
        name: 'SpecialCharAgent',
        key: 'special_chars',
        model_id: 'gpt-4',
        tools: [],
        blocked_tool_patterns: specialPatterns,
        persona: 'Test special characters',
        category: ['test']
      };

      // Act & Assert
      expect(config.blocked_tool_patterns).toEqual(specialPatterns);
      config.blocked_tool_patterns.forEach(pattern => {
        expect(typeof pattern).toBe('string');
      });
    });

    it('should handle unicode characters in patterns', () => {
      // Arrange
      const unicodePatterns = [
        'tool_世界',
        'أداة_عربية',
        'outil_français',
        'инструмент_русский',
        'ツール_日本語',
        '🔧_emoji_tool',
        '工具_中文',
        'εργαλείο_ελληνικά'
      ];

      const config: AgentConfigurationV2 = {
        version: 2,
        name: 'UnicodePatternAgent',
        key: 'unicode_patterns',
        model_id: 'gpt-4',
        tools: [],
        allowed_tool_patterns: unicodePatterns,
        persona: 'Test unicode patterns',
        category: ['international']
      };

      // Act & Assert
      expect(config.allowed_tool_patterns).toEqual(unicodePatterns);
    });

    it('should handle very long patterns', () => {
      // Arrange
      const longPattern = 'a'.repeat(1000) + '_tool_' + 'b'.repeat(1000);
      
      const config: AgentConfigurationV2 = {
        version: 2,
        name: 'LongPatternAgent',
        key: 'long_patterns',
        model_id: 'gpt-4',
        tools: [],
        blocked_tool_patterns: [longPattern],
        persona: 'Test long patterns',
        category: ['test']
      };

      // Act & Assert
      expect(config.blocked_tool_patterns[0]).toHaveLength(2006);
      expect(config.blocked_tool_patterns[0]).toBe(longPattern);
    });
  });

  describe('Security Behavior Simulation', () => {
    // Helper function to simulate pattern matching
    function matchesPattern(toolName: string, pattern: string): boolean {
      // Convert glob pattern to regex
      const regexPattern = pattern
        .replace(/\*/g, '.*')
        .replace(/\?/g, '.')
        .replace(/\[/g, '\\[')
        .replace(/\]/g, '\\]');
      
      const regex = new RegExp(`^${regexPattern}$`);
      return regex.test(toolName);
    }

    // Helper function to check if tool is allowed
    function isToolAllowed(
      toolName: string,
      blockedPatterns?: string[],
      allowedPatterns?: string[]
    ): boolean {
      // If blocked patterns exist and match, tool is blocked
      if (blockedPatterns) {
        for (const pattern of blockedPatterns) {
          if (matchesPattern(toolName, pattern)) {
            return false; // Blocked takes precedence
          }
        }
      }

      // If allowed patterns exist, tool must match one
      if (allowedPatterns && allowedPatterns.length > 0) {
        for (const pattern of allowedPatterns) {
          if (matchesPattern(toolName, pattern)) {
            return true;
          }
        }
        return false; // Not in allowed list
      }

      // No restrictions, tool is allowed
      return true;
    }

    it('should block tools matching blocked patterns', () => {
      // Arrange
      const config: AgentConfigurationV2 = {
        version: 2,
        name: 'BlockingAgent',
        key: 'blocking',
        model_id: 'gpt-4',
        tools: ['admin_delete', 'user_read', 'system_write'],
        blocked_tool_patterns: ['admin_*', 'system_*'],
        persona: 'Blocking certain tools',
        category: ['secure']
      };

      // Act & Assert
      expect(isToolAllowed('admin_delete', config.blocked_tool_patterns)).toBe(false);
      expect(isToolAllowed('admin_create', config.blocked_tool_patterns)).toBe(false);
      expect(isToolAllowed('system_write', config.blocked_tool_patterns)).toBe(false);
      expect(isToolAllowed('user_read', config.blocked_tool_patterns)).toBe(true);
      expect(isToolAllowed('public_tool', config.blocked_tool_patterns)).toBe(true);
    });

    it('should only allow tools matching allowed patterns', () => {
      // Arrange
      const config: AgentConfigurationV2 = {
        version: 2,
        name: 'AllowlistAgent',
        key: 'allowlist',
        model_id: 'gpt-4',
        tools: [],
        allowed_tool_patterns: ['read_*', 'list_*', 'get_*'],
        persona: 'Only allowing specific tools',
        category: ['restricted']
      };

      // Act & Assert
      expect(isToolAllowed('read_file', undefined, config.allowed_tool_patterns)).toBe(true);
      expect(isToolAllowed('list_users', undefined, config.allowed_tool_patterns)).toBe(true);
      expect(isToolAllowed('get_data', undefined, config.allowed_tool_patterns)).toBe(true);
      expect(isToolAllowed('write_file', undefined, config.allowed_tool_patterns)).toBe(false);
      expect(isToolAllowed('delete_user', undefined, config.allowed_tool_patterns)).toBe(false);
    });

    it('should have blocked patterns take precedence over allowed patterns', () => {
      // Arrange
      const config: AgentConfigurationV2 = {
        version: 2,
        name: 'PrecedenceAgent',
        key: 'precedence',
        model_id: 'gpt-4',
        tools: [],
        blocked_tool_patterns: ['admin_*', '*_delete'],
        allowed_tool_patterns: ['*'], // Allow all
        persona: 'Testing precedence',
        category: ['test']
      };

      // Act & Assert
      // Even though all tools are allowed, blocked patterns take precedence
      expect(isToolAllowed(
        'admin_read',
        config.blocked_tool_patterns,
        config.allowed_tool_patterns
      )).toBe(false);

      expect(isToolAllowed(
        'user_delete',
        config.blocked_tool_patterns,
        config.allowed_tool_patterns
      )).toBe(false);

      expect(isToolAllowed(
        'user_read',
        config.blocked_tool_patterns,
        config.allowed_tool_patterns
      )).toBe(true);
    });

    it('should handle undefined security fields (allow all behavior)', () => {
      // Arrange
      const config: AgentConfigurationV2 = {
        version: 2,
        name: 'UnrestrictedAgent',
        key: 'unrestricted',
        model_id: 'gpt-4',
        tools: ['any_tool'],
        persona: 'No restrictions',
        category: ['open']
        // No blocked_tool_patterns or allowed_tool_patterns
      };

      // Act & Assert - All tools allowed when no patterns defined
      expect(isToolAllowed('admin_delete', config.blocked_tool_patterns, config.allowed_tool_patterns)).toBe(true);
      expect(isToolAllowed('system_write', config.blocked_tool_patterns, config.allowed_tool_patterns)).toBe(true);
      expect(isToolAllowed('any_tool', config.blocked_tool_patterns, config.allowed_tool_patterns)).toBe(true);
    });

    it('should handle empty arrays (different from undefined)', () => {
      // Arrange
      const emptyBlockedConfig: AgentConfigurationV2 = {
        version: 2,
        name: 'EmptyBlockedAgent',
        key: 'empty_blocked',
        model_id: 'gpt-4',
        tools: [],
        blocked_tool_patterns: [], // Empty array - blocks nothing
        persona: 'Empty blocked list',
        category: ['test']
      };

      const emptyAllowedConfig: AgentConfigurationV2 = {
        version: 2,
        name: 'EmptyAllowedAgent',
        key: 'empty_allowed',
        model_id: 'gpt-4',
        tools: [],
        allowed_tool_patterns: [], // Empty array - allows nothing?
        persona: 'Empty allowed list',
        category: ['test']
      };

      // Act & Assert
      // Empty blocked array blocks nothing
      expect(isToolAllowed('any_tool', emptyBlockedConfig.blocked_tool_patterns)).toBe(true);
      
      // Empty allowed array with no blocked array - implementation dependent
      // Could mean "allow all" or "allow none" - testing "allow all" interpretation
      // (Empty allow list is different from no allow list)
      expect(isToolAllowed('any_tool', undefined, emptyAllowedConfig.allowed_tool_patterns)).toBe(true);
    });

    it('should handle complex pattern combinations', () => {
      // Arrange
      const config: AgentConfigurationV2 = {
        version: 2,
        name: 'ComplexPatternsAgent',
        key: 'complex_patterns',
        model_id: 'gpt-4',
        tools: [],
        blocked_tool_patterns: [
          '*_admin',
          'system_*',
          'debug_*',
          '*_dangerous_*'
        ],
        allowed_tool_patterns: [
          'public_*',
          'user_*',
          'read_*',
          '*_safe'
        ],
        persona: 'Complex security rules',
        category: ['secure']
      };

      // Act & Assert
      // Tool matching allowed pattern but not blocked
      expect(isToolAllowed(
        'public_read',
        config.blocked_tool_patterns,
        config.allowed_tool_patterns
      )).toBe(true);

      // Tool matching both allowed and blocked (blocked wins)
      expect(isToolAllowed(
        'user_admin',  // Matches user_* (allowed) and *_admin (blocked)
        config.blocked_tool_patterns,
        config.allowed_tool_patterns
      )).toBe(false);

      // Tool not matching any pattern
      expect(isToolAllowed(
        'random_tool',
        config.blocked_tool_patterns,
        config.allowed_tool_patterns
      )).toBe(false); // Not in allowed list

      // Tool with dangerous in name (blocked)
      expect(isToolAllowed(
        'tool_dangerous_operation',
        config.blocked_tool_patterns,
        config.allowed_tool_patterns
      )).toBe(false);
    });
  });

  describe('JSON Serialization', () => {
    it('should preserve security fields through JSON serialization', () => {
      // Arrange
      const original: AgentConfigurationV2 = {
        version: 2,
        name: 'SerializationTestAgent',
        key: 'serialization_test',
        model_id: 'gpt-4',
        tools: ['tool1', 'tool2'],
        blocked_tool_patterns: ['blocked_*', '*_danger'],
        allowed_tool_patterns: ['safe_*', 'public_*'],
        persona: 'Test serialization',
        category: ['test', 'security']
      };

      // Act
      const json = JSON.stringify(original);
      const parsed = JSON.parse(json) as AgentConfigurationV2;

      // Assert
      expect(parsed).toEqual(original);
      expect(parsed.blocked_tool_patterns).toEqual(original.blocked_tool_patterns);
      expect(parsed.allowed_tool_patterns).toEqual(original.allowed_tool_patterns);
    });

    it('should maintain array order in security fields', () => {
      // Arrange
      const patterns = ['pattern1', 'pattern2', 'pattern3', 'pattern4', 'pattern5'];
      const reversedPatterns = [...patterns].reverse(); // Create a copy before reversing
      const config: AgentConfigurationV2 = {
        version: 2,
        name: 'OrderTestAgent',
        key: 'order_test',
        model_id: 'gpt-4',
        tools: [],
        blocked_tool_patterns: patterns,
        allowed_tool_patterns: reversedPatterns,
        persona: 'Test order preservation',
        category: ['test']
      };

      // Act
      const json = JSON.stringify(config);
      const parsed = JSON.parse(json) as AgentConfigurationV2;

      // Assert - Order must be preserved
      expect(parsed.blocked_tool_patterns).toEqual(patterns);
      expect(parsed.allowed_tool_patterns).toEqual(reversedPatterns);
      
      // Verify exact order
      parsed.blocked_tool_patterns?.forEach((pattern, index) => {
        expect(pattern).toBe(config.blocked_tool_patterns![index]);
      });
    });

    it('should preserve special characters in patterns through serialization', () => {
      // Arrange
      const specialPatterns = [
        'tool*with?special[chars]',
        'tool\\with\\backslash',
        'tool"with"quotes',
        "tool'with'apostrophe",
        'tool\nwith\nnewline',
        'tool\twith\ttab',
        'tool\rwith\rcarriage',
        'tool/with/slash',
        'tool{with}braces',
        'tool$with$dollar'
      ];

      const config: AgentConfigurationV2 = {
        version: 2,
        name: 'SpecialCharsSerializationAgent',
        key: 'special_serialization',
        model_id: 'gpt-4',
        tools: [],
        blocked_tool_patterns: specialPatterns,
        persona: 'Test special char serialization',
        category: ['test']
      };

      // Act
      const json = JSON.stringify(config);
      const parsed = JSON.parse(json) as AgentConfigurationV2;

      // Assert
      expect(parsed.blocked_tool_patterns).toEqual(specialPatterns);
      
      // Verify each pattern individually
      specialPatterns.forEach((pattern, index) => {
        expect(parsed.blocked_tool_patterns![index]).toBe(pattern);
      });
    });

    it('should handle undefined security fields in serialization', () => {
      // Arrange
      const config: AgentConfigurationV2 = {
        version: 2,
        name: 'UndefinedFieldsAgent',
        key: 'undefined_fields',
        model_id: 'gpt-4',
        tools: [],
        persona: 'Test undefined fields',
        category: ['test']
        // blocked_tool_patterns and allowed_tool_patterns are undefined
      };

      // Act
      const json = JSON.stringify(config);
      const parsed = JSON.parse(json) as AgentConfigurationV2;

      // Assert - undefined fields should not be in JSON
      expect(json).not.toContain('blocked_tool_patterns');
      expect(json).not.toContain('allowed_tool_patterns');
      expect(parsed.blocked_tool_patterns).toBeUndefined();
      expect(parsed.allowed_tool_patterns).toBeUndefined();
    });

    it('should handle empty arrays in serialization', () => {
      // Arrange
      const config: AgentConfigurationV2 = {
        version: 2,
        name: 'EmptyArraysAgent',
        key: 'empty_arrays',
        model_id: 'gpt-4',
        tools: [],
        blocked_tool_patterns: [],
        allowed_tool_patterns: [],
        persona: 'Test empty arrays',
        category: ['test']
      };

      // Act
      const json = JSON.stringify(config);
      const parsed = JSON.parse(json) as AgentConfigurationV2;

      // Assert - Empty arrays should be preserved
      expect(json).toContain('"blocked_tool_patterns":[]');
      expect(json).toContain('"allowed_tool_patterns":[]');
      expect(parsed.blocked_tool_patterns).toEqual([]);
      expect(parsed.allowed_tool_patterns).toEqual([]);
      expect(Array.isArray(parsed.blocked_tool_patterns)).toBe(true);
      expect(Array.isArray(parsed.allowed_tool_patterns)).toBe(true);
    });

    it('should simulate WebSocket transmission', () => {
      // Arrange
      const config: AgentConfigurationV2 = {
        version: 2,
        name: 'WebSocketAgent',
        key: 'websocket_test',
        model_id: 'gpt-4',
        tools: ['tool1'],
        blocked_tool_patterns: ['ws_blocked_*', '*_restricted'],
        allowed_tool_patterns: ['ws_allowed_*', 'public_*'],
        persona: 'WebSocket transmission test',
        category: ['network']
      };

      // Simulate WebSocket send/receive
      const simulateWebSocketTransmission = (data: any): any => {
        // Convert to string (as WebSocket would)
        const transmitted = JSON.stringify(data);
        // Receive and parse (as client would)
        return JSON.parse(transmitted);
      };

      // Act
      const received = simulateWebSocketTransmission(config);

      // Assert
      expect(received).toEqual(config);
      expect(received.blocked_tool_patterns).toEqual(config.blocked_tool_patterns);
      expect(received.allowed_tool_patterns).toEqual(config.allowed_tool_patterns);
      expect(received.version).toBe(2);
    });
  });

  describe('Type Guards and Validation', () => {
    // Helper type guard for V2 configuration
    function isAgentConfigurationV2(config: any): config is AgentConfigurationV2 {
      return config && 
             typeof config === 'object' &&
             config.version === 2 &&
             typeof config.name === 'string' &&
             typeof config.key === 'string' &&
             typeof config.model_id === 'string' &&
             Array.isArray(config.tools) &&
             typeof config.persona === 'string' &&
             Array.isArray(config.category) &&
             (config.blocked_tool_patterns === undefined || Array.isArray(config.blocked_tool_patterns)) &&
             (config.allowed_tool_patterns === undefined || Array.isArray(config.allowed_tool_patterns));
    }

    it('should correctly identify V2 configuration with security fields', () => {
      // Arrange
      const v2WithSecurity: AgentConfigurationV2 = {
        version: 2,
        name: 'V2SecurityAgent',
        key: 'v2_security',
        model_id: 'gpt-4',
        tools: [],
        blocked_tool_patterns: ['blocked_*'],
        allowed_tool_patterns: ['allowed_*'],
        persona: 'V2 with security',
        category: ['secure']
      };

      const v2WithoutSecurity: AgentConfigurationV2 = {
        version: 2,
        name: 'V2NoSecurityAgent',
        key: 'v2_no_security',
        model_id: 'gpt-4',
        tools: [],
        persona: 'V2 without security',
        category: ['open']
      };

      const v1Config: AgentConfigurationV1 = {
        version: 1,
        name: 'V1Agent',
        key: 'v1_agent',
        model_id: 'gpt-3.5',
        tools: [],
        persona: 'V1 agent',
        category: ['legacy']
      };

      // Act & Assert
      expect(isAgentConfigurationV2(v2WithSecurity)).toBe(true);
      expect(isAgentConfigurationV2(v2WithoutSecurity)).toBe(true);
      expect(isAgentConfigurationV2(v1Config)).toBe(false);
    });

    it('should validate security field types at runtime', () => {
      // Arrange
      const validateSecurityFields = (config: any): boolean => {
        if (config.blocked_tool_patterns !== undefined) {
          if (!Array.isArray(config.blocked_tool_patterns)) return false;
          if (!config.blocked_tool_patterns.every((p: any) => typeof p === 'string')) return false;
        }
        
        if (config.allowed_tool_patterns !== undefined) {
          if (!Array.isArray(config.allowed_tool_patterns)) return false;
          if (!config.allowed_tool_patterns.every((p: any) => typeof p === 'string')) return false;
        }
        
        return true;
      };

      // Valid configurations
      const validConfig = {
        blocked_tool_patterns: ['pattern1', 'pattern2'],
        allowed_tool_patterns: ['pattern3']
      };

      // Invalid configurations
      const invalidConfigs = [
        { blocked_tool_patterns: 'not_an_array' },
        { blocked_tool_patterns: [123, 456] }, // Not strings
        { allowed_tool_patterns: {} }, // Not array
        { allowed_tool_patterns: [true, false] }, // Not strings
        { blocked_tool_patterns: ['valid', 123, 'mixed'] } // Mixed types
      ];

      // Act & Assert
      expect(validateSecurityFields(validConfig)).toBe(true);
      expect(validateSecurityFields({})).toBe(true); // undefined fields are OK
      
      invalidConfigs.forEach(config => {
        expect(validateSecurityFields(config)).toBe(false);
      });
    });

    it('should handle union type discrimination', () => {
      // Arrange
      const configs: AgentConfiguration[] = [
        {
          version: 1,
          name: 'V1',
          key: 'v1',
          model_id: 'gpt-3.5',
          tools: [],
          persona: 'V1',
          category: ['v1']
        },
        {
          version: 2,
          name: 'V2',
          key: 'v2',
          model_id: 'gpt-4',
          tools: [],
          blocked_tool_patterns: ['blocked'],
          allowed_tool_patterns: ['allowed'],
          persona: 'V2',
          category: ['v2']
        }
      ];

      // Act & Assert
      configs.forEach(config => {
        if (config.version === 2) {
          // TypeScript should know this is V2
          const v2Config = config as AgentConfigurationV2;
          expect('blocked_tool_patterns' in v2Config).toBe(true);
          expect('allowed_tool_patterns' in v2Config).toBe(true);
        } else {
          // TypeScript should know this is V1
          const v1Config = config as AgentConfigurationV1;
          expect('blocked_tool_patterns' in v1Config).toBe(false);
          expect('allowed_tool_patterns' in v1Config).toBe(false);
        }
      });
    });
  });

  describe('Integration Scenarios', () => {
    it('should handle real-world security configuration', () => {
      // Arrange - Simulating a production agent with security restrictions
      const productionConfig: AgentConfigurationV2 = {
        version: 2,
        name: 'CustomerServiceAgent',
        key: 'customer_service',
        model_id: 'gpt-4',
        agent_description: 'Handles customer inquiries with restricted access',
        tools: [
          'search_knowledge_base',
          'get_customer_info',
          'create_ticket',
          'send_email'
        ],
        blocked_tool_patterns: [
          'admin_*',           // No admin tools
          '*_delete',          // No deletion tools
          '*_write_database',  // No direct DB writes
          'system_*',          // No system tools
          'execute_*',         // No code execution
          '*_payment_*',       // No payment processing
          'modify_*_critical'  // No critical modifications
        ],
        allowed_tool_patterns: [
          'read_*',            // Can read data
          'search_*',          // Can search
          'get_*',             // Can get information
          'list_*',            // Can list items
          'create_ticket',     // Specifically allowed
          'send_email',        // Specifically allowed
          'translate_*'        // Can use translation tools
        ],
        persona: 'You are a helpful customer service representative',
        category: ['customer_service', 'support', 'restricted']
      };

      // Act - Simulate checking various tools
      const toolChecks = [
        { tool: 'search_knowledge_base', expected: true },
        { tool: 'admin_panel', expected: false },
        { tool: 'delete_customer', expected: false },
        { tool: 'read_faq', expected: true },
        { tool: 'execute_script', expected: false },
        { tool: 'process_payment_refund', expected: false },
        { tool: 'get_order_status', expected: true }
      ];

      // Assert
      toolChecks.forEach(({ tool, expected }) => {
        const allowed = isToolAllowed(
          tool,
          productionConfig.blocked_tool_patterns,
          productionConfig.allowed_tool_patterns
        );
        expect(allowed).toBe(expected);
      });
    });

    it('should handle migration from V1 to V2 with security', () => {
      // Arrange - V1 configuration
      const v1Config: AgentConfigurationV1 = {
        version: 1,
        name: 'LegacyAgent',
        key: 'legacy',
        model_id: 'gpt-3.5-turbo',
        tools: ['dangerous_tool', 'safe_tool'],
        persona: 'Legacy assistant',
        category: ['general']
      };

      // Migrate to V2 with added security
      const migrateToV2WithSecurity = (v1: AgentConfigurationV1): AgentConfigurationV2 => {
        return {
          ...v1,
          version: 2,
          blocked_tool_patterns: ['dangerous_*', 'unsafe_*'],
          allowed_tool_patterns: ['safe_*', 'public_*']
        };
      };

      // Act
      const v2Config = migrateToV2WithSecurity(v1Config);

      // Assert
      expect(v2Config.version).toBe(2);
      expect(v2Config.blocked_tool_patterns).toBeDefined();
      expect(v2Config.allowed_tool_patterns).toBeDefined();
      expect(v2Config.blocked_tool_patterns).toContain('dangerous_*');
      expect(v2Config.tools).toEqual(v1Config.tools); // Tools preserved
    });

    it('should handle environment-specific security configurations', () => {
      // Arrange - Different configs for different environments
      const getConfigForEnvironment = (env: 'development' | 'staging' | 'production'): AgentConfigurationV2 => {
        const baseConfig: AgentConfigurationV2 = {
          version: 2,
          name: 'EnvironmentAgent',
          key: 'env_agent',
          model_id: 'gpt-4',
          tools: [],
          persona: 'Environment-aware assistant',
          category: ['adaptive']
        };

        switch (env) {
          case 'development':
            return {
              ...baseConfig,
              blocked_tool_patterns: [], // No restrictions in dev
              allowed_tool_patterns: undefined // Allow all
            };
          
          case 'staging':
            return {
              ...baseConfig,
              blocked_tool_patterns: ['production_*', 'customer_delete_*'],
              allowed_tool_patterns: ['*'] // Most tools allowed
            };
          
          case 'production':
            return {
              ...baseConfig,
              blocked_tool_patterns: [
                'debug_*',
                'test_*',
                'admin_*',
                '*_delete',
                'system_*'
              ],
              allowed_tool_patterns: [
                'public_*',
                'customer_read_*',
                'analytics_*'
              ]
            };
        }
      };

      // Act
      const devConfig = getConfigForEnvironment('development');
      const stagingConfig = getConfigForEnvironment('staging');
      const prodConfig = getConfigForEnvironment('production');

      // Assert
      expect(devConfig.blocked_tool_patterns).toEqual([]);
      expect(stagingConfig.blocked_tool_patterns).toHaveLength(2);
      expect(prodConfig.blocked_tool_patterns).toHaveLength(5);
      expect(prodConfig.allowed_tool_patterns).toHaveLength(3);
    });
  });

  describe('Error Cases and Edge Conditions', () => {
    it('should handle null or undefined in pattern arrays gracefully', () => {
      // This should not compile in TypeScript, but testing runtime behavior
      const config = {
        version: 2,
        name: 'NullPatternAgent',
        key: 'null_patterns',
        model_id: 'gpt-4',
        tools: [],
        blocked_tool_patterns: ['valid', null as any, undefined as any, 'another'],
        persona: 'Test null patterns',
        category: ['test']
      };

      // Runtime validation should catch this
      const validatePatterns = (patterns: any[]): boolean => {
        return patterns.every(p => typeof p === 'string');
      };

      // Act & Assert
      expect(validatePatterns(config.blocked_tool_patterns)).toBe(false);
    });

    it('should handle extremely large pattern arrays', () => {
      // Arrange - Create a large array of patterns
      const largePatternArray = Array.from(
        { length: 10000 },
        (_, i) => `pattern_${i}_*`
      );

      const config: AgentConfigurationV2 = {
        version: 2,
        name: 'LargeArrayAgent',
        key: 'large_array',
        model_id: 'gpt-4',
        tools: [],
        blocked_tool_patterns: largePatternArray,
        persona: 'Test large arrays',
        category: ['performance']
      };

      // Act
      const json = JSON.stringify(config);
      const parsed = JSON.parse(json) as AgentConfigurationV2;

      // Assert
      expect(parsed.blocked_tool_patterns).toHaveLength(10000);
      expect(parsed.blocked_tool_patterns![0]).toBe('pattern_0_*');
      expect(parsed.blocked_tool_patterns![9999]).toBe('pattern_9999_*');
    });

    it('should handle pattern conflicts and overlaps', () => {
      // Arrange - Patterns that might conflict
      const config: AgentConfigurationV2 = {
        version: 2,
        name: 'ConflictAgent',
        key: 'conflict',
        model_id: 'gpt-4',
        tools: [],
        blocked_tool_patterns: [
          'tool_*',      // Blocks all starting with tool_
          'tool_safe'    // More specific, but already blocked by tool_*
        ],
        allowed_tool_patterns: [
          'tool_*',      // Tries to allow what's blocked
          'safe_*'       // Different pattern
        ],
        persona: 'Test conflicts',
        category: ['test']
      };

      // Act - Test conflicting patterns
      // tool_test matches both blocked and allowed (blocked wins)
      const result1 = isToolAllowed(
        'tool_test',
        config.blocked_tool_patterns,
        config.allowed_tool_patterns
      );

      // safe_test only matches allowed
      const result2 = isToolAllowed(
        'safe_test',
        config.blocked_tool_patterns,
        config.allowed_tool_patterns
      );

      // Assert
      expect(result1).toBe(false); // Blocked wins
      expect(result2).toBe(true);  // Only in allowed
    });
  });
});

// Additional helper functions for pattern matching
function isToolAllowed(
  toolName: string,
  blockedPatterns?: string[],
  allowedPatterns?: string[]
): boolean {
  // Helper to convert glob to regex
  const globToRegex = (glob: string): RegExp => {
    const escaped = glob
      .replace(/[.+^${}()|[\]\\]/g, '\\$&')
      .replace(/\*/g, '.*')
      .replace(/\?/g, '.');
    return new RegExp(`^${escaped}$`);
  };

  // Check blocked patterns first (highest priority)
  if (blockedPatterns) {
    for (const pattern of blockedPatterns) {
      if (pattern && globToRegex(pattern).test(toolName)) {
        return false;
      }
    }
  }

  // Check allowed patterns
  if (allowedPatterns && allowedPatterns.length > 0) {
    for (const pattern of allowedPatterns) {
      if (pattern && globToRegex(pattern).test(toolName)) {
        return true;
      }
    }
    // If allow list exists but tool doesn't match, deny
    return false;
  }

  // No restrictions, allow by default
  return true;
}