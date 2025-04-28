import React, {useState} from 'react';
import {ChevronDown, Maximize2} from 'lucide-react';
import {Dialog, DialogContent} from "@/components/ui/dialog";
import {Card} from "@/components/ui/card";
import CopyButton from './CopyButton';
import MarkdownMessage from "@/components/chat_interface/MarkdownMessage";

const MediaMessage = ({message}) => {
    const [isExpanded, setIsExpanded] = useState(true);
    const [isFullscreen, setIsFullscreen] = useState(false);

    if (!message?.content || !message?.contentType) {
        console.warn('MediaMessage: Invalid message structure', message);
        return null;
    }

    const toggleFullscreen = (e) => {
        e.stopPropagation();
        setIsFullscreen(!isFullscreen);
    };

    // Get content to copy based on media type
    const getCopyContent = () => {
        // For SVG, return the SVG code
        if (message.contentType === 'image/svg+xml') {
            return message.content;
        }
        // For HTML, return the HTML code
        if (message.contentType === "text/html") {
            return message.content;
        }
        if (message.contentType === "text/markdown") {
            return message.content; // Return raw markdown for copying
        }
        // For images, we can't copy the binary data directly,
        // so we'll create a data URL that could be used in an img tag
        if (message.contentType?.startsWith('image/')) {
            return `data:${message.contentType};base64,${message.content}`;
        }
        return message.content;
    };

    const MediaContentWrapper = ({children, allowFullscreen = false}) => (
        <div className="media-message-media-wrapper">
            {allowFullscreen && (
                <button
                    onClick={toggleFullscreen}
                    className="media-message-fullscreen-button"
                >
                    <Maximize2 className="media-message-fullscreen-icon"/>
                </button>
            )}
            {children}
        </div>
    );

    const renderContent = () => {
        if (message.contentType === 'image/svg+xml') {
            return (
                <MediaContentWrapper allowFullscreen>
                    <div
                        className="w-full"
                        dangerouslySetInnerHTML={{__html: message.content}}
                    />
                </MediaContentWrapper>
            );
        }

        if (message.contentType === "text/html") {
            return (
                <MediaContentWrapper>
                    <div className="media-message-html-content" dangerouslySetInnerHTML={{__html: message.content}}/>
                </MediaContentWrapper>
            );
        }
        if (message.contentType === "text/markdown") {
            return (
                <MediaContentWrapper>
                    <div className="markdown-content">
                        <MarkdownMessage content={message.content}/>
                    </div>
                </MediaContentWrapper>
            );
        }

        if (message.contentType?.startsWith('image/')) {
            return (
                <MediaContentWrapper allowFullscreen>
                    <img
                        src={`data:${message.contentType};base64,${message.content}`}
                        alt="Generated Content"
                        className="media-message-image"
                        loading="lazy"
                    />
                </MediaContentWrapper>
            );
        }

        return null;
    };

    const renderMetadata = () => {
        if (!message.metadata) return null;

        const {sent_by_class, sent_by_function} = message.metadata;
        if (!sent_by_class && !sent_by_function) return null;

        return (
            <div className="media-message-metadata">
                <span className="media-message-metadata-text">
                    {sent_by_class}
                    {sent_by_class && sent_by_function && <span className="media-message-metadata-separator">{"\u2022"}</span>}
                    {sent_by_function}
                </span>
            </div>
        );
    };

    const FullscreenDialog = () => (
        <Dialog open={isFullscreen} onOpenChange={setIsFullscreen}>
            <DialogContent className="p-0 overflow-hidden max-w-7xl" style={{ backgroundColor: 'var(--media-message-background)' }}>
                <div className="media-message-fullscreen-content">
                    {message.contentType === 'image/svg+xml' ? (
                        <div
                            className="w-full h-full flex items-center justify-center"
                            dangerouslySetInnerHTML={{__html: message.content}}
                        />
                    ) : message.contentType?.startsWith('image/') ? (
                        <img
                            src={`data:${message.contentType};base64,${message.content}`}
                            alt="Generated content"
                            className="media-message-fullscreen-image"
                        />
                    ) : (
                        <div dangerouslySetInnerHTML={{__html: message.content}}/>
                    )}
                </div>
            </DialogContent>
        </Dialog>
    );

    const getContentTypeDisplay = () => {
        // Handle both full MIME types and short versions
        if (message.contentType.includes('/')) {
            return message.contentType.split('/')[1].toUpperCase();
        }
        return message.contentType.toUpperCase();
    };

    return (
        <>
            <Card className={`media-message-card ${isExpanded ? 'media-message-card-expanded' : 'media-message-card-collapsed'}`}>
                <div
                    className="media-message-header"
                    onClick={() => setIsExpanded(!isExpanded)}
                >
                    <div className="media-message-header-content">
                        {renderMetadata()}
                    </div>
                    <div className="media-message-header-actions">
                        {/* Copy button that stops event propagation */}
                        <div onClick={(e) => e.stopPropagation()}>
                            <CopyButton
                                content={getCopyContent}
                                tooltipText={`Copy ${message.contentType.split('/')[1]}`}
                                variant="ghost"
                                className="media-message-copy-button"
                            />
                        </div>
                        <ChevronDown
                            className={`media-message-expand-icon ${isExpanded ? "media-message-expand-icon-expanded" : ""}`}
                        />
                    </div>
                </div>

                {isExpanded && (
                    <div className="media-message-content">
                        <div className="media-message-content-wrapper">
                            {renderContent()}
                        </div>
                    </div>
                )}
            </Card>

            {isFullscreen && <FullscreenDialog/>}
        </>
    );
};

// Create a memoized version of MediaMessage to prevent unnecessary re-renders
const MemoizedMediaMessage = React.memo(MediaMessage, (prevProps, nextProps) => {
  // Deep compare the message objects
  const prevMsg = prevProps.message;
  const nextMsg = nextProps.message;
  
  // Basic content and type checks
  if (prevMsg.contentType !== nextMsg.contentType) return false;
  
  // For non-binary content (SVG, HTML, Markdown), compare the content directly
  if ([
    'image/svg+xml', 
    'text/html', 
    'text/markdown'
  ].includes(prevMsg.contentType)) {
    if (prevMsg.content !== nextMsg.content) return false;
  } 
  // For binary content like images, we'll do a simple check
  else if (prevMsg.contentType?.startsWith('image/')) {
    // If the content length has changed, it's definitely different
    if (prevMsg.content.length !== nextMsg.content.length) return false;
    
    // For binary data, we could do a more detailed comparison,
    // but for performance reasons we'll assume it's the same if length matches
    // since images are typically immutable once rendered
  }
  
  // Check metadata
  const prevMeta = prevMsg.metadata || {};
  const nextMeta = nextMsg.metadata || {};
  
  if (prevMeta.sent_by_class !== nextMeta.sent_by_class) return false;
  if (prevMeta.sent_by_function !== nextMeta.sent_by_function) return false;
  
  // If we got here, the message content appears to be the same
  return true;
});

export default MemoizedMediaMessage;