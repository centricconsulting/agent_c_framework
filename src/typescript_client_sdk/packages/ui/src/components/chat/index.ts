// Chat component exports
export { Message } from './Message'
export type { MessageProps, MessageData } from './Message'

export { MessageContentRenderer } from './MessageContentRenderer'
export type { MessageContentRendererProps } from './MessageContentRenderer'

// Content Renderers
export * from './content-renderers'

export { MessageFooter } from './MessageFooter'
export type { MessageFooterProps } from './MessageFooter'

export { SystemNotification, SystemNotificationContainer } from './SystemNotification'
export type { 
  SystemNotificationProps, 
  SystemNotificationContainerProps,
  SystemNotificationData,
  NotificationSeverity 
} from './SystemNotification'

export { MessageList } from './MessageList'
export type { MessageListProps } from './MessageList'

export { TypingIndicator, typingIndicatorStyles } from './TypingIndicator'
export type { TypingIndicatorProps } from './TypingIndicator'

export { ScrollAnchor } from './ScrollAnchor'
export type { ScrollAnchorProps } from './ScrollAnchor'

export { ChatMessagesView } from './ChatMessagesView'
export type { ChatMessagesViewProps } from './ChatMessagesView'

export { SubsessionDivider } from './SubsessionDivider'
export type { SubsessionDividerProps } from './SubsessionDivider'

export { MediaRenderer } from './MediaRenderer'
export type { MediaRendererProps } from './MediaRenderer'

export { SystemMessage } from './SystemMessage'
export type { SystemMessageProps } from './SystemMessage'

// DropOverlay removed - InputArea uses react-dropzone directly

export { UploadProgressIndicator } from './UploadProgressIndicator'
export type { UploadProgressIndicatorProps } from './UploadProgressIndicator'

export { ImageLightbox } from './ImageLightbox'
export type { ImageLightboxProps } from './ImageLightbox'

export { FileAttachmentItem } from './FileAttachmentItem'
export type { FileAttachmentItemProps } from './FileAttachmentItem'

export { FileAttachmentList } from './FileAttachmentList'
export type { FileAttachmentListProps } from './FileAttachmentList'

// ChatInputArea removed - InputArea in input/ folder now has file upload features
