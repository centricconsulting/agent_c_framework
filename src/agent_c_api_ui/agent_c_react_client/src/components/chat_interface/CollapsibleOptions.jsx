import React from 'react';
import PropTypes from 'prop-types';
import { cn } from "@/lib/utils";
import {
    Collapsible,
    CollapsibleContent,
    CollapsibleTrigger,
} from "@/components/ui/collapsible";
import { Button } from "@/components/ui/button";
import { ChevronDown, ChevronUp, Settings, Wrench } from 'lucide-react';
import { 
    Card, 
    CardHeader, 
    CardTitle, 
    CardContent 
} from '@/components/ui/card';
import { 
    Tabs, 
    TabsList, 
    TabsTrigger, 
    TabsContent 
} from "@/components/ui/tabs";
import PersonaSelector from './PersonaSelector';
import ToolSelector from './ToolSelector';

/**
 * A collapsible options panel that provides persona and tool selection functionality.
 * It uses tabs to organize settings and tools sections for better user experience.
 *
 * @component
 * @param {Object} props
 * @param {boolean} props.isOpen - Current open/closed state of the panel
 * @param {Function} props.setIsOpen - Callback to update open/closed state
 * @param {string} props.persona - Currently selected persona name (passed as persona_name to PersonaSelector)
 * @param {string} props.customPrompt - Custom prompt text for the AI
 * @param {number} props.temperature - Model temperature setting (0-1)
 * @param {string} props.modelName - Current model name displayed to the user
 * @param {Object} props.modelConfigs - Available model configurations object
 * @param {string} props.sessionId - Current session identifier for API calls
 * @param {Array} props.personas - Available personas list
 * @param {Array} props.availableTools - List of available tools that can be equipped
 * @param {Function} props.onEquipTools - Callback for tool equipment changes
 * @param {Array} props.activeTools - Currently equipped tools
 * @param {boolean} props.isReady - Component ready state (used by ToolSelector)
 * @param {Object} props.modelParameters - Current model parameters configuration
 * @param {string} props.selectedModel - Selected model identifier for API
 * @param {Function} props.onUpdateSettings - Settings update callback
 * @param {boolean} props.isInitialized - Component initialization state
 * @param {string} props.className - Optional additional CSS class names
 *
 * @returns {React.ReactElement} A collapsible options panel component with tabs for settings and tools
 */
const CollapsibleOptions = ({
    isOpen,
    setIsOpen,
    persona,
    customPrompt,
    temperature,
    modelName,
    modelConfigs,
    sessionId,
    personas,
    availableTools,
    onEquipTools,
    activeTools,
    isReady,
    modelParameters,
    selectedModel,
    onUpdateSettings,
    isInitialized,
    className
}) => {
    return (
        <Card 
            className={cn(
                "w-full mb-2 bg-card/50 backdrop-blur-sm border-border shadow-sm collapsible-options-card", 
                className
            )}
            role="region"
            aria-label="Configuration options"
        >
            <Collapsible 
                    open={isOpen} 
                    onOpenChange={setIsOpen}
                    aria-expanded={isOpen}
                >
                <CardHeader className="py-2 px-3 collapsible-options-header">
                    <div className="flex items-center justify-between">
                        <CardTitle className="text-base font-medium" id="options-panel-title">
                            Options Panel
                        </CardTitle>
                        <CollapsibleTrigger asChild>
                            <Button 
                                variant="ghost" 
                                size="sm" 
                                className="w-9 p-0 collapsible-options-trigger"
                                aria-label={isOpen ? "Collapse options panel" : "Expand options panel"}
                                aria-controls="options-panel-content"
                            >
                                {isOpen ? 
                                    <ChevronUp className="h-4 w-4" /> : 
                                    <ChevronDown className="h-4 w-4" />
                                }
                                <span className="sr-only">{isOpen ? "Collapse" : "Expand"} options panel</span>
                            </Button>
                        </CollapsibleTrigger>
                    </div>
                </CardHeader>
                <CardContent className="px-3 pt-0 pb-4 collapsible-options-content">
                    <CollapsibleContent className="collapsible-animation" id="options-panel-content">
                        <Tabs defaultValue="settings" className="w-full">
                            <TabsList className="w-full mb-4 border border-border bg-muted p-1 rounded-md shadow-sm collapsible-options-tabs-list" aria-label="Configuration options tabs">
                                <TabsTrigger 
                                    value="settings" 
                                    className="flex items-center gap-1 w-1/2 font-medium"
                                    aria-label="Settings tab"
                                >
                                    <Settings className="h-4 w-4" />
                                    <span>Settings</span>
                                </TabsTrigger>
                                <TabsTrigger 
                                    value="tools" 
                                    className="flex items-center gap-1 w-1/2 font-medium"
                                    aria-label="Tools tab"
                                >
                                    <Wrench className="h-4 w-4" />
                                    <span>Tools</span>
                                </TabsTrigger>
                            </TabsList>
                            
                            <TabsContent value="settings" className="mt-0">
                                <PersonaSelector
                                    persona_name={persona}
                                    personas={personas}
                                    customPrompt={customPrompt}
                                    temperature={temperature}
                                    modelName={modelName}
                                    modelConfigs={modelConfigs}
                                    sessionId={sessionId}
                                    modelParameters={modelParameters}
                                    selectedModel={selectedModel}
                                    onUpdateSettings={onUpdateSettings}
                                    isInitialized={isInitialized}
                                />
                            </TabsContent>
                            
                            <TabsContent value="tools" className="mt-0">
                                <ToolSelector
                                    availableTools={availableTools}
                                    onEquipTools={onEquipTools}
                                    activeTools={activeTools}
                                    sessionId={sessionId}
                                    isReady={isReady}
                                />
                            </TabsContent>
                        </Tabs>
                    </CollapsibleContent>
                </CardContent>
            </Collapsible>
        </Card>
    );
};

CollapsibleOptions.propTypes = {
    isOpen: PropTypes.bool,
    setIsOpen: PropTypes.func.isRequired,
    persona: PropTypes.string,
    customPrompt: PropTypes.string,
    temperature: PropTypes.number,
    modelName: PropTypes.string,
    modelConfigs: PropTypes.array.isRequired, // Must be an array of model configurations, not an object
    sessionId: PropTypes.string.isRequired,
    personas: PropTypes.array,
    availableTools: PropTypes.array,
    onEquipTools: PropTypes.func,
    activeTools: PropTypes.array,
    isReady: PropTypes.bool,
    modelParameters: PropTypes.object,
    selectedModel: PropTypes.string,
    onUpdateSettings: PropTypes.func,
    isInitialized: PropTypes.bool,
    className: PropTypes.string
};

export default CollapsibleOptions;