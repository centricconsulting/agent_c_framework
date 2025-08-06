# Import original models for backward compatibility
from .models import (
    ContentBlockType,
    BaseContentBlock,
    MessageRole,
    StopInfo,
    TokenUsage,
    ProviderMetadata
)

# Import enhanced models (with backward compatibility aliases)
from .enhanced_models import (
    # Enhanced models
    EnhancedCommonChatMessage,
    EnhancedContentBlock,
    EnhancedTextContentBlock,
    EnhancedToolUseContentBlock,
    EnhancedToolResultContentBlock,
    EnhancedThinkingContentBlock,
    
    # New enums
    ValidityState,
    OutcomeStatus,
    ReasoningType,
    
    # Backward compatibility aliases (point to enhanced versions)
    CommonChatMessage,
    ContentBlock,
    TextContentBlock,
    ToolUseContentBlock,
    ToolResultContentBlock,
    ThinkingContentBlock,
    ImageContentBlock,
    AudioContentBlock
)

# Import enhanced converters
from .enhanced_converters import (
    # Enhanced converter classes
    EnhancedAnthropicConverter,
    EnhancedOpenAIConverter,
    EnhancedProviderTranslationLayer,
    
    # Enums and exceptions
    ProviderType,
    ProviderCapability,
    TranslationError,
    TranslationAuditEntry,
    
    # Convenience functions
    create_translation_layer,
    translate_from_anthropic,
    translate_from_openai,
    translate_to_anthropic,
    translate_to_openai
)

# Import original converters for backward compatibility
from .converters import (
    AnthropicConverter,
    OpenAIConverter,
    CommonChatMessageConverter
)