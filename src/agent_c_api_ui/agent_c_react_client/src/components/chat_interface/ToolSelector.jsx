import React, { useState, useCallback, useMemo, useEffect } from 'react';
import { cn } from '@/lib/utils';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Checkbox } from '@/components/ui/checkbox';
import { Button } from '@/components/ui/button';
import { ScrollArea } from '@/components/ui/scroll-area';
import { useToast } from '@/hooks/use-toast';
import {
    Tooltip,
    TooltipContent,
    TooltipProvider,
    TooltipTrigger,
} from '@/components/ui/tooltip';
import { Badge } from '@/components/ui/badge';
import { Icon } from '@/components/ui/icon';
import { Tabs, TabsList, TabsTrigger, TabsContent } from '@/components/ui/tabs';
import logger from '@/lib/logger';

/**
 * Displays essential tools that cannot be toggled
 * @component
 * @param {Object} props
 * @param {Tool[]} [props.tools=[]] - Array of essential tools
 */
const EssentialTools = React.memo(({ tools = [] }) => (
    <div className="essential-tools-container">
        <h3 className="essential-tools-title">Essential Tools</h3>
        <div className="essential-tools-content">
            <div className="essential-tools-badges">
                {tools.map((tool) => (
                    <TooltipProvider key={tool.name}>
                        <Tooltip>
                            <TooltipTrigger asChild>
                                <Badge
                                    variant="secondary"
                                    className="essential-tool-badge"
                                >
                                    <div className="essential-tool-badge-content">
                                        <div className="essential-tool-badge-dot"></div>
                                        {tool.name}
                                    </div>
                                </Badge>
                            </TooltipTrigger>
                            <TooltipContent className="tool-tooltip-content">
                                <p className="max-w-xs">{tool.doc || 'No description available'}</p>
                            </TooltipContent>
                        </Tooltip>
                    </TooltipProvider>
                ))}
            </div>
        </div>
    </div>
));

/**
 * Displays a category of tools with selection capabilities
 * @component
 * @param {Object} props
 * @param {string} props.title - Category title
 * @param {Tool[]} props.tools - Tools in this category
 * @param {Set<string>} props.selectedTools - Set of selected tool names
 * @param {string[]} props.activeTools - Array of active tool names
 * @param {(toolName: string) => void} props.onToolToggle - Tool toggle callback
 * @param {boolean} props.disabled - Whether tool selection is disabled
 */
const ToolCategory = React.memo(({ title, tools = [], selectedTools, activeTools, onToolToggle, disabled = false }) => (
    <div className="tool-category-container">
        <div className="tool-category-content">
            <div className="tool-category-grid">
                {tools.map((tool) => {
                    const isSelected = selectedTools.has(tool.name);
                    const isActive = activeTools.includes(tool.name);
                    return (
                        <TooltipProvider key={tool.name}>
                            <Tooltip>
                                <TooltipTrigger asChild>
                                    <div
                                        className={cn(
                                            "tool-item", 
                                            isActive && "active",
                                            disabled && "disabled"
                                        )}
                                    >
                                        <Checkbox
                                            id={tool.name}
                                            checked={isSelected}
                                            onCheckedChange={() => onToolToggle(tool.name)}
                                            disabled={disabled}
                                        />
                                        <label
                                            htmlFor={tool.name}
                                            className={cn(
                                                "tool-item-label",
                                                isActive && "active",
                                                disabled && "disabled"
                                            )}
                                        >
                                            {tool.name}
                                            {isActive && (
                                                <span className="tool-active-badge">
                                                    <Icon icon="fa-regular fa-check" hoverIcon="fa-solid fa-check" className="tool-active-icon"/>
                                                    Active
                                                </span>
                                            )}
                                        </label>
                                    </div>
                                </TooltipTrigger>
                                <TooltipContent className="tool-tooltip-content">
                                    <p className="max-w-xs">{tool.doc || 'No description available'}</p>
                                </TooltipContent>
                            </Tooltip>
                        </TooltipProvider>
                    );
                })}
            </div>
        </div>
    </div>
), (prevProps, nextProps) => {
    // Custom comparison function for memoization
    return (
        prevProps.title === nextProps.title &&
        prevProps.tools === nextProps.tools &&
        prevProps.disabled === nextProps.disabled &&
        JSON.stringify(Array.from(prevProps.selectedTools)) === JSON.stringify(Array.from(nextProps.selectedTools)) &&
        JSON.stringify(prevProps.activeTools) === JSON.stringify(nextProps.activeTools)
    );
});

/**
 * ToolSelector is a component that manages the selection and equipment of tools for an agent.
 *
 * @component
 * @param {Object} props
 * @param {Object} props.availableTools - Configuration object containing tool categories and groups
 * @param {Array} props.availableTools.essential_tools - Array of essential tool objects
 * @param {Array} props.availableTools.categories - Array of category names
 * @param {Object} props.availableTools.groups - Object mapping categories to tool arrays
 * @param {Function} props.onEquipTools - Callback function when tools are equipped
 * @param {Array} [props.activeTools=[]] - Array of currently active tool names
 * @param {string} props.sessionId - Current session identifier
 * @param {boolean} props.isReady - Flag indicating if the agent is ready
 */
const ToolSelector = ({
    availableTools,
    onEquipTools,
    activeTools = [],
    sessionId,
    isReady
}) => {
    // Local UI state
    const [selectedTools, setSelectedTools] = useState(() => new Set(activeTools));
    const [error, setError] = useState(null);
    const [isLoading, setIsLoading] = useState(false);
    const [isVerifying, setIsVerifying] = useState(false);
    const { toast } = useToast();

    // Sync selectedTools with activeTools when they change
    useEffect(() => {
        if (activeTools && activeTools.length > 0) {
            logger.debug('Syncing selected tools with active tools', 'ToolSelector', {
                activeToolsCount: activeTools.length,
                selectedToolsCount: selectedTools.size
            });
            
            setSelectedTools(new Set(activeTools));
        }
    }, [activeTools]);

    /**
     * Toggles a tool's selection state
     * @param {string} toolName - Name of the tool to toggle
     */
    const handleToolToggle = useCallback((toolName) => {
        if (!isReady || isLoading || isVerifying) {
            toast({
                title: 'Action not available',
                description: isLoading || isVerifying ? 'Tool update in progress' : 'Please wait for agent initialization',
                variant: 'destructive',
            });
            return;
        }
        
        setSelectedTools((prev) => {
            const newSelected = new Set(prev);
            if (newSelected.has(toolName)) {
                newSelected.delete(toolName);
            } else {
                newSelected.add(toolName);
            }
            return newSelected;
        });
    }, [isReady, isLoading, isVerifying, toast]);

    /**
     * Handles the equipping of selected tools
     * @async
     * @throws {Error} When tool equipment fails
     */
    const handleEquipTools = useCallback(async () => {
        if (!isReady || isLoading || isVerifying) {
            toast({
                title: 'Action not available',
                description: isLoading || isVerifying ? 'Tool update already in progress' : 'Please wait for agent initialization',
                variant: 'destructive',
            });
            return;
        }
        
        setIsLoading(true);
        setError(null);
        
        try {
            const toolsToEquip = Array.from(selectedTools);
            logger.info('Equipping tools', 'ToolSelector', { toolCount: toolsToEquip.length });
            
            // Call the equip tools method from session context
            const success = await onEquipTools(toolsToEquip);
            
            if (!success) {
                throw new Error('Failed to equip tools - operation returned failure');
            }
            
            // Additional verification step
            setIsVerifying(true);
            
            // Wait a short time to ensure backend state updates
            await new Promise(resolve => setTimeout(resolve, 500));
            
            // Get the active tools again to verify changes
            const newActiveToolsSet = new Set(activeTools);
            const missingTools = toolsToEquip.filter(tool => !newActiveToolsSet.has(tool));
            
            if (missingTools.length > 0) {
                // Some tools failed to equip
                logger.warn('Some tools failed to equip', 'ToolSelector', { missingTools });
                
                toast({
                    title: 'Partial success',
                    description: `Some tools couldn't be equipped: ${missingTools.join(', ')}`,
                    variant: 'warning',
                });
            } else {
                // All tools successfully equipped
                toast({
                    title: 'Success',
                    description: 'All tools equipped successfully!',
                });
                
                // Update our selected tools to match what was actually equipped
                setSelectedTools(new Set(activeTools));
            }
        } catch (error) {
            logger.error('Error equipping tools', 'ToolSelector', {
                error: error.message,
                stack: error.stack
            });
            
            setError('Failed to equip tools: ' + error.message);
            
            toast({
                title: 'Error',
                description: 'Failed to equip tools: ' + error.message,
                variant: 'destructive',
            });
        } finally {
            setIsLoading(false);
            setIsVerifying(false);
        }
    }, [isReady, isLoading, isVerifying, selectedTools, activeTools, onEquipTools, toast]);

    // Compute default tab value
    const defaultTabValue = useMemo(() => {
        if (!availableTools) return '';
        
        if (availableTools.categories.includes("core")) {
            return "core";
        } else if (availableTools.essential_tools && availableTools.essential_tools.length > 0) {
            return "essential";
        } else if (availableTools.categories.length > 0) {
            return availableTools.categories[0];
        }
        
        return '';
    }, [availableTools]);

    // Render waiting message if not ready
    if (!sessionId || !isReady) {
        return (
            <Card className="tool-selector">
                <CardHeader>
                    <CardTitle>Available Tools</CardTitle>
                </CardHeader>
                <CardContent>
                    <div className="tool-selector-waiting">
                        <Icon icon="fa-regular fa-circle-exclamation" hoverIcon="fa-solid fa-circle-exclamation" size="lg" className="text-yellow-500" />
                        <p>Waiting for agent initialization...</p>
                    </div>
                </CardContent>
            </Card>
        );
    }

    // Render no tools message if none available
    if (!availableTools || !Object.keys(availableTools).length) {
        return (
            <Card className="tool-selector">
                <CardHeader>
                    <CardTitle>Available Tools</CardTitle>
                </CardHeader>
                <CardContent>
                    <div className="tool-selector-waiting">
                        <p className="text-muted-foreground">No tools available</p>
                    </div>
                </CardContent>
            </Card>
        );
    }

    // Operation state for UI display
    const isOperationInProgress = isLoading || isVerifying;
    const operationStatus = isVerifying ? 'Verifying...' : (isLoading ? 'Updating...' : '');

    return (
        <Card className="tool-selector">
            <CardHeader>
                <CardTitle className="flex justify-between items-center">
                    <span>Available Tools</span>
                    {operationStatus && (
                        <span className="text-sm text-muted-foreground">{operationStatus}</span>
                    )}
                </CardTitle>
            </CardHeader>
            <CardContent>
                {error && (
                    <div className="tool-selector-error">
                        {error}
                    </div>
                )}
                <Button
                    onClick={handleEquipTools}
                    disabled={!isReady || isOperationInProgress}
                    variant="default"
                    className="tool-selector-equip-button"
                >
                    {isOperationInProgress ? (
                        <>
                            <Icon icon="fa-regular fa-spinner-third" className="mr-2 animate-spin" />
                            {operationStatus || 'Updating Tools...'}
                        </>
                    ) : (
                        <>
                            <Icon icon="fa-regular fa-check-circle" hoverIcon="fa-solid fa-check-circle" className="mr-2" />
                            Equip Selected Tools
                        </>
                    )}
                </Button>

                <Tabs 
                    defaultValue={defaultTabValue} 
                    className="tool-selector-tabs"
                >
                    <TabsList className="tool-selector-tabs-list">
                        {availableTools.essential_tools && availableTools.essential_tools.length > 0 && (
                            <TabsTrigger 
                                value="essential" 
                                className="tool-selector-tab-trigger"
                            >
                                Essential
                            </TabsTrigger>
                        )}
                        {availableTools.categories.map((category) => (
                            <TabsTrigger 
                                key={category} 
                                value={category} 
                                className="tool-selector-tab-trigger"
                            >
                                {category}
                            </TabsTrigger>
                        ))}
                    </TabsList>

                    <ScrollArea className="tool-selector-content">
                        {availableTools.essential_tools && availableTools.essential_tools.length > 0 && (
                            <TabsContent value="essential" className="mt-0">
                                <EssentialTools tools={availableTools.essential_tools}/>
                            </TabsContent>
                        )}
                        
                        {availableTools.categories.map((category) => (
                            <TabsContent key={category} value={category} className="mt-0">
                                <ToolCategory
                                    title={category}
                                    tools={availableTools.groups[category] || []}
                                    selectedTools={selectedTools}
                                    activeTools={activeTools}
                                    onToolToggle={handleToolToggle}
                                    disabled={isOperationInProgress}
                                />
                            </TabsContent>
                        ))}
                    </ScrollArea>
                </Tabs>
            </CardContent>
        </Card>
    );
};

export default React.memo(ToolSelector);