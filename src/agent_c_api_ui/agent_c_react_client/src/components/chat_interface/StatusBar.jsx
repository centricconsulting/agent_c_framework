import React from 'react';
import PropTypes from 'prop-types';
import { Button } from '@/components/ui/button';
import { Badge } from '@/components/ui/badge';
import { Card } from '@/components/ui/card';
import AgentConfigHoverCard from './AgentConfigHoverCard';
import { Activity, Wrench, Info, Copy, Download } from 'lucide-react';
import {
    Tooltip,
    TooltipContent,
    TooltipProvider,
    TooltipTrigger,
} from "@/components/ui/tooltip";
import CopyButton from './CopyButton';
import ExportHTMLButton from './ExportHTMLButton';
import { cn } from '@/lib/utils';
import { useIsMobile } from '@/hooks/use-mobile';
import { useMessageContext } from '@/contexts/MessageContext';


/**
 * StatusBar component that displays system status information, active tools, and export options.
 * Shows different statuses based on application state (initializing, processing, ready).
 *
 * @component
 * @param {Object} props
 * @param {boolean} props.isReady - Whether the agent is ready to process requests
 * @param {Array} props.activeTools - List of currently active tools
 * @param {boolean} props.isInitializing - Whether the application is initializing
 * @param {string} props.sessionId - Current session identifier
 * @param {string} props.settingsVersion - Version of the current settings
 * @param {string} props.className - Additional CSS class names
 *
 * @returns {React.ReactElement} A status bar displaying system state and actions
 */
const StatusBar = ({
    isReady,
    activeTools = [],
    isInitializing = false,
    sessionId,
    settingsVersion,
    className
}) => {
    // Use mobile detection hook to adapt UI
    const isMobile = useIsMobile();
    
    // Get message context
    const { messages, isStreaming, getChatCopyContent, getChatCopyHTML } = useMessageContext('StatusBar');

    const getStatusInfo = () => {
        if (isStreaming) {
            return {
                message: 'Processing...',
                description: 'Agent is processing your request'
            };
        }
        if (isInitializing) {
            return {
                message: 'Initializing Application...',
                description: 'Loading initial application data'
            };
        }
        if (!isReady) {
            return {
                message: 'Initializing Agent...',
                description: 'Setting up the agent and tools'
            };
        }
        return {
            message: 'Ready',
            description: 'System is ready to process requests'
        };
    };

    const statusInfo = getStatusInfo();

    return (
        <Card 
            className={cn("status-bar", className)} 
            role="status" 
            aria-live="polite"
            style={{ padding: '0.125rem 0.5rem' /* Inline override for more compact size */ }}
        >
            <div className="status-bar-section" aria-label="System status information">
                <div className="status-indicator-section">
                    <TooltipProvider>
                        <Tooltip>
                            <TooltipTrigger asChild>
                                <div 
                                    className="status-indicator-wrapper"
                                    tabIndex={0}
                                    aria-label={`System status: ${statusInfo.message}`}
                                >
                                    <Activity
                                        className={cn(
                                            isStreaming 
                                                ? 'status-indicator-icon-processing' 
                                                : isInitializing || !isReady 
                                                    ? 'status-indicator-icon-initializing' 
                                                    : 'status-indicator-icon-ready'
                                        )}
                                        aria-hidden="true"
                                    />
                                    <span className="status-text">
                                        {isMobile ? statusInfo.message : `Status: ${statusInfo.message}`}
                                    </span>
                                </div>
                            </TooltipTrigger>
                            <TooltipContent side="bottom">
                                <p>{statusInfo.description}</p>
                            </TooltipContent>
                        </Tooltip>
                    </TooltipProvider>
                    {/* Only show AgentConfigHoverCard on desktop */}
                    {isReady && sessionId && !isMobile && (
                        <AgentConfigHoverCard
                            sessionId={sessionId}
                            className="ml-2"
                            settingsVersion={settingsVersion}
                        />
                    )}
                </div>

                {isReady && activeTools && activeTools.length > 0 && (
                    <Badge 
                        variant="outline" 
                        className="tools-badge"
                        aria-label={`${activeTools.length} active tools`}
                    >
                        <Wrench className="tools-icon" aria-hidden="true" />
                        <span className="tools-count-text">
                            Active Tools: {activeTools.length}
                        </span>
                        <TooltipProvider>
                            <Tooltip>
                                <TooltipTrigger asChild>
                                    <Button 
                                        variant="ghost" 
                                        size="xs" 
                                        className="p-0 h-auto w-auto bg-transparent hover:bg-transparent"
                                        aria-label="View active tools list"
                                    >
                                        <Info className="tools-info-icon" />
                                        <span className="sr-only">View active tools</span>
                                    </Button>
                                </TooltipTrigger>
                                <TooltipContent className="tools-tooltip-content" side="top">
                                    <p className="max-w-xs text-muted-foreground">{activeTools.join(', ')}</p>
                                </TooltipContent>
                            </Tooltip>
                        </TooltipProvider>
                    </Badge>
                )}
            </div>
            
            {/* Chat export actions */}
            <div className="chat-export-actions" aria-label="Chat export options">
                <CopyButton
                    content={getChatCopyContent}
                    htmlContent={getChatCopyHTML}
                    tooltipText="Copy entire chat"
                    successText="Chat copied!"
                    size="sm"
                    variant="ghost"
                    className={cn("chat-export-button")}
                    icon={<Copy className="chat-export-icon" aria-hidden="true" />}
                    iconOnly
                    aria-label="Copy chat to clipboard"
                />
                <ExportHTMLButton
                    messages={messages}
                    tooltipText="Export as HTML"
                    filename={`chat-export-${new Date().toISOString().slice(0, 10)}.html`}
                    size="sm"
                    variant="ghost"
                    className={cn("chat-export-button")}
                    icon={<Download className="chat-export-icon" aria-hidden="true" />}
                    iconOnly
                    aria-label="Export chat as HTML file"
                />
            </div>
        </Card>
    );
};

StatusBar.propTypes = {
    isReady: PropTypes.bool,
    activeTools: PropTypes.array,
    isInitializing: PropTypes.bool,
    sessionId: PropTypes.string,
    settingsVersion: PropTypes.number,
    className: PropTypes.string
};

export default StatusBar;