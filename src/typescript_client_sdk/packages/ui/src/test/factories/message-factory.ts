/**
 * Message Factory for Testing
 * Generate realistic message data for chat component tests
 */

import { faker } from '@faker-js/faker';

export interface TestMessage {
  id: string;
  role: 'user' | 'assistant' | 'system';
  content: string;
  timestamp: string;
  status?: 'sending' | 'sent' | 'failed' | 'delivered';
  metadata?: {
    model?: string;
    tokens?: number;
    duration?: number;
    edited?: boolean;
    editedAt?: string;
  };
  attachments?: Array<{
    id: string;
    type: 'image' | 'file' | 'code';
    name: string;
    url?: string;
    size?: number;
  }>;
}

export class MessageFactory {
  /**
   * Create a single message
   */
  static create(overrides?: Partial<TestMessage>): TestMessage {
    return {
      id: faker.string.uuid(),
      role: faker.helpers.arrayElement(['user', 'assistant'] as const),
      content: faker.lorem.paragraph(),
      timestamp: faker.date.recent().toISOString(),
      status: 'sent',
      ...overrides
    };
  }

  /**
   * Create a user message
   */
  static createUserMessage(content?: string): TestMessage {
    return this.create({
      role: 'user',
      content: content || faker.lorem.sentence(),
      status: 'sent'
    });
  }

  /**
   * Create an assistant message
   */
  static createAssistantMessage(content?: string): TestMessage {
    return this.create({
      role: 'assistant',
      content: content || faker.lorem.paragraph(),
      status: 'delivered',
      metadata: {
        model: 'gpt-4',
        tokens: faker.number.int({ min: 10, max: 500 }),
        duration: faker.number.int({ min: 100, max: 5000 })
      }
    });
  }

  /**
   * Create a system message
   */
  static createSystemMessage(content?: string): TestMessage {
    return this.create({
      role: 'system',
      content: content || 'System notification',
      status: 'delivered'
    });
  }

  /**
   * Create a message with error
   */
  static createFailedMessage(): TestMessage {
    return this.create({
      role: 'user',
      content: faker.lorem.sentence(),
      status: 'failed'
    });
  }

  /**
   * Create a message that's being sent
   */
  static createSendingMessage(): TestMessage {
    return this.create({
      role: 'user',
      content: faker.lorem.sentence(),
      status: 'sending'
    });
  }

  /**
   * Create a message with attachments
   */
  static createMessageWithAttachments(): TestMessage {
    return this.create({
      attachments: [
        {
          id: faker.string.uuid(),
          type: faker.helpers.arrayElement(['image', 'file', 'code'] as const),
          name: faker.system.fileName(),
          url: faker.internet.url(),
          size: faker.number.int({ min: 1000, max: 5000000 })
        }
      ]
    });
  }

  /**
   * Create a long message
   */
  static createLongMessage(): TestMessage {
    return this.create({
      content: faker.lorem.paragraphs(5)
    });
  }

  /**
   * Create a code block message
   */
  static createCodeMessage(): TestMessage {
    const code = `
\`\`\`javascript
function example() {
  console.log("Hello, world!");
  return true;
}
\`\`\`
    `.trim();
    
    return this.create({
      role: 'assistant',
      content: code
    });
  }

  /**
   * Create a markdown message
   */
  static createMarkdownMessage(): TestMessage {
    const markdown = `
# Heading 1
## Heading 2

This is a **bold** text and this is *italic*.

- List item 1
- List item 2
  - Nested item

> This is a blockquote

[Link text](https://example.com)
    `.trim();
    
    return this.create({
      role: 'assistant',
      content: markdown
    });
  }

  /**
   * Create an edited message
   */
  static createEditedMessage(): TestMessage {
    const originalTime = faker.date.recent();
    const editTime = faker.date.recent({ days: 1, refDate: originalTime });
    
    return this.create({
      timestamp: originalTime.toISOString(),
      metadata: {
        edited: true,
        editedAt: editTime.toISOString()
      }
    });
  }

  /**
   * Create a conversation (array of messages)
   */
  static createConversation(messageCount: number = 10): TestMessage[] {
    const messages: TestMessage[] = [];
    
    for (let i = 0; i < messageCount; i++) {
      const isUser = i % 2 === 0;
      messages.push(
        isUser 
          ? this.createUserMessage()
          : this.createAssistantMessage()
      );
    }
    
    // Ensure timestamps are in order
    messages.sort((a, b) => 
      new Date(a.timestamp).getTime() - new Date(b.timestamp).getTime()
    );
    
    return messages;
  }

  /**
   * Create a conversation with various message types
   */
  static createMixedConversation(): TestMessage[] {
    return [
      this.createSystemMessage('Chat started'),
      this.createUserMessage('Hello, can you help me?'),
      this.createAssistantMessage('Of course! I\'d be happy to help. What do you need assistance with?'),
      this.createUserMessage('Can you show me some code?'),
      this.createCodeMessage(),
      this.createUserMessage('Thanks! Can you format that as markdown?'),
      this.createMarkdownMessage(),
      this.createMessageWithAttachments(),
      this.createEditedMessage(),
      this.createSystemMessage('Connection restored')
    ];
  }

  /**
   * Create messages for edge case testing
   */
  static createEdgeCases(): Record<string, TestMessage> {
    return {
      empty: this.create({ content: '' }),
      whitespace: this.create({ content: '   \n\t  ' }),
      emoji: this.create({ content: '😀 🎉 🚀 ❤️' }),
      unicode: this.create({ content: '你好世界 مرحبا بالعالم' }),
      special: this.create({ content: '<script>alert("XSS")</script>' }),
      veryLong: this.create({ content: 'a'.repeat(10000) }),
      multiline: this.create({ content: 'Line 1\nLine 2\nLine 3' }),
      url: this.create({ content: 'Check out https://example.com' }),
      mention: this.create({ content: 'Hello @user and @assistant' }),
      hashtag: this.create({ content: 'Working on #feature #bugfix' })
    };
  }

  /**
   * Create messages for performance testing
   */
  static createManyMessages(count: number = 1000): TestMessage[] {
    return Array.from({ length: count }, (_, i) => 
      this.create({
        id: `msg-${i}`,
        role: i % 2 === 0 ? 'user' : 'assistant'
      })
    );
  }

  /**
   * Create real-time streaming message
   */
  static createStreamingMessage(content: string = ''): TestMessage {
    return this.create({
      role: 'assistant',
      content,
      status: 'sending',
      metadata: {
        model: 'gpt-4',
        tokens: 0,
        duration: 0
      }
    });
  }

  /**
   * Update streaming message content
   */
  static updateStreamingMessage(
    message: TestMessage, 
    additionalContent: string
  ): TestMessage {
    return {
      ...message,
      content: message.content + additionalContent,
      metadata: {
        ...message.metadata,
        tokens: (message.metadata?.tokens || 0) + additionalContent.split(' ').length
      }
    };
  }

  /**
   * Complete streaming message
   */
  static completeStreamingMessage(message: TestMessage): TestMessage {
    return {
      ...message,
      status: 'delivered',
      metadata: {
        ...message.metadata,
        duration: faker.number.int({ min: 100, max: 3000 })
      }
    };
  }
}