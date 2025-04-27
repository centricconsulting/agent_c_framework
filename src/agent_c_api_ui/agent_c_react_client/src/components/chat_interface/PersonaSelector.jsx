import React, {useCallback, useEffect, useState} from 'react';
import {Card, CardContent, CardHeader, CardTitle} from '@/components/ui/card';
import {Textarea} from '@/components/ui/textarea';
import {
    Select,
    SelectContent,
    SelectItem,
    SelectTrigger,
    SelectValue,
} from '@/components/ui/select';
import {Label} from '@/components/ui/label';
import {
    Tooltip,
    TooltipContent,
    TooltipProvider,
    TooltipTrigger,
} from "@/components/ui/tooltip";
import ModelParameterControls from './ModelParameterControls';


/**
 * PersonaSelector is a component that manages AI model settings, persona selection,
 * and custom prompts through a user interface.
 *
 * @component
 * @param {Object} props
 * @param {string} props.persona_name - Currently selected persona identifier
 * @param {Array<{name: string, content: string}>} props.personas - Available personas
 * @param {string} props.customPrompt - Custom instructions for the AI model
 * @param {string} props.modelName - Current model identifier
 * @param {Array<ModelConfig>} props.modelConfigs - Available model configurations
 * @param {string} props.sessionId - Current session identifier
 * @param {Object} props.modelParameters - Current model parameters
 * @param {ModelConfig} props.selectedModel - Currently selected model configuration
 * @param {Function} props.onUpdateSettings - Callback for settings updates
 * @param {boolean} props.isInitialized - Component initialization status
 */
function PersonaSelector({
                             persona_name,
                             personas,
                             customPrompt,
                             modelName,
                             modelConfigs,
                             sessionId,
                             modelParameters,
                             selectedModel,
                             onUpdateSettings,
                             isInitialized
                         }) {
    // Defensive check for modelConfigs - ensure it's an array
    const [safeModelConfigs, setSafeModelConfigs] = useState([]);
    
    // Initialize with a default empty array immediately to prevent undefined errors
    React.useEffect(() => {
        console.log('PersonaSelector received modelConfigs:', modelConfigs);
        if (Array.isArray(modelConfigs)) {
            setSafeModelConfigs(modelConfigs);
        } else if (modelConfigs && typeof modelConfigs === 'object') {
            // Try to convert object to array if possible
            setSafeModelConfigs(Object.values(modelConfigs));
        } else {
            console.warn('PersonaSelector: modelConfigs is not an array or valid object, using empty array');
            setSafeModelConfigs([]);
        }
    }, [modelConfigs]);
    
    // CRITICAL: Ensure modelConfigs is never undefined before any operations
    
    useEffect(() => {
        if (modelConfigs && Array.isArray(modelConfigs)) {
            // It's already an array, use it directly
            setSafeModelConfigs(modelConfigs);
        } else if (modelConfigs && typeof modelConfigs === 'object') {
            console.error('PersonaSelector received modelConfigs that is not an array:', modelConfigs);
            // Try to convert to array if possible
            setSafeModelConfigs(Object.values(modelConfigs));
        } else {
            console.error('PersonaSelector received undefined or invalid modelConfigs');
            setSafeModelConfigs([]); // Default to empty array
        }
    }, [modelConfigs]);
    // Local UI state only
    const [error, setError] = useState(null);
    const [selectedPersona, setSelectedPersona] = useState(persona_name || 'default');
    const [localCustomPrompt, setLocalCustomPrompt] = useState(customPrompt);

    // Sync local UI state with prop
    useEffect(() => {
        if (isInitialized) {
            setSelectedPersona(persona_name);
            setLocalCustomPrompt(customPrompt);
        }
    }, [isInitialized, persona_name, customPrompt]);

    /**
     * Handles persona selection changes
     * @param {string} value - Selected persona identifier
     */
    const handlePersonaChange = useCallback((value) => {
        setSelectedPersona(value);
        const selectedPersonaData = personas.find(p => p.name === value);
        if (selectedPersonaData) {
            onUpdateSettings('SETTINGS_UPDATE', {
                persona_name: value,
                customPrompt: selectedPersonaData.content
            });
        }
    }, [personas, onUpdateSettings]);

    /**
     * Handles changes to the custom prompt textarea
     * @param {React.ChangeEvent<HTMLTextAreaElement>} e - Change event
     */
    const handleCustomPromptChange = useCallback((e) => {
        // Update local state immediately for responsive UI
        // console.log('Custom Prompt Change:', e.target.value);
        setLocalCustomPrompt(e.target.value);
    }, []);

    /**
     * Triggers settings update when custom prompt editing is complete
     */
    const handleCustomPromptBlur = useCallback(() => {
        // Only send update if the value has actually changed
        if (localCustomPrompt !== customPrompt) {
            console.log('User Changed Custom Prompt:', localCustomPrompt);
            onUpdateSettings('SETTINGS_UPDATE', {
                customPrompt: localCustomPrompt
            });
        }
    }, [localCustomPrompt, customPrompt, onUpdateSettings]);

    /**
     * Handles model parameter changes
     * @param {string} paramName - Name of the parameter to update
     * @param {any} value - New parameter value
     */
    const handleParameterChange = useCallback((paramName, value) => {
        onUpdateSettings('PARAMETER_UPDATE', {
            [paramName]: value
        });
    }, [onUpdateSettings]);

    /**
     * Handles model selection changes
     * @param {string} selectedValue - Selected model identifier
     */
    const handleModelChange = useCallback((selectedValue) => {
        // Defensive check: verify safeModelConfigs exists and is an array
        if (!Array.isArray(safeModelConfigs)) {
            console.error('handleModelChange: safeModelConfigs is not an array:', safeModelConfigs);
            return;
        }

        try {
            // Use our safe model configs array that's guaranteed to be an array
            const model = safeModelConfigs.find(model => model && model.id === selectedValue);
            if (model && model.id && model.backend) {
                console.log('Selected model:', model.id);
                onUpdateSettings('MODEL_CHANGE', {
                    modelName: model.id,
                    backend: model.backend
                });
            } else {
                console.error(`Could not find valid model with ID ${selectedValue} in available models`);
            }
        } catch (error) {
            console.error('Error in handleModelChange:', error);
        }
    }, [safeModelConfigs, onUpdateSettings]);

    return (
        <Card className="persona-selector-card" role="region" aria-label="Persona and model settings">
            <CardHeader className="pb-2">
                <CardTitle>Settings</CardTitle>
            </CardHeader>
            <CardContent className="persona-selector-content">
                {/* Persona Selection */}
                <div className="persona-selector-section">
                    <Label htmlFor="persona-select">Load Persona Prompt</Label>
                    <Select 
                        value={selectedPersona} 
                        onValueChange={handlePersonaChange}
                        aria-label="Select a persona"
                    >
                        <SelectTrigger
                            id="persona-select"
                            className="persona-selector-select-trigger"
                            aria-label="Available personas"
                        >
                            <SelectValue placeholder="Select a persona"/>
                        </SelectTrigger>
                        <SelectContent className="persona-selector-select-content">
                            {personas.map((p) => (
                                <SelectItem
                                    key={p.name}
                                    value={p.name}
                                    className="persona-selector-select-item"
                                >
                                    {p.name}
                                </SelectItem>
                            ))}
                        </SelectContent>
                    </Select>
                    {error && (
                        <div 
                            className="persona-selector-error" 
                            role="alert" 
                            aria-live="assertive"
                        >
                            Error: {error}
                        </div>
                    )}
                </div>

                {/* Custom Instructions */}
                <div className="persona-selector-section">
                    <Label htmlFor="custom-prompt">Customize Persona Instructions</Label>
                    <Textarea
                        id="custom-prompt"
                        value={localCustomPrompt}
                        onChange={handleCustomPromptChange}
                        onBlur={handleCustomPromptBlur}
                        className="persona-selector-textarea"
                        placeholder="You are a helpful assistant."
                        aria-label="Custom persona instructions"
                        aria-describedby="custom-prompt-description"
                    />
                    <div id="custom-prompt-description" className="sr-only">
                        Enter custom instructions for the AI to follow during the conversation
                    </div>
                </div>

                <div className="persona-selector-grid">
                    {/* Model Selection */}
                    <div className="persona-selector-section">
                        <Label htmlFor="model-select">Model</Label>
                        <Select
                            value={modelName}
                            onValueChange={handleModelChange}
                            aria-label="Select an AI model"
                        >
                            <SelectTrigger
                                id="model-select"
                                className="persona-selector-select-trigger"
                                aria-label="Available AI models"
                            >
                                <SelectValue placeholder="Select model"/>
                            </SelectTrigger>
                            <SelectContent className="persona-selector-select-content">
                                {safeModelConfigs && safeModelConfigs.length > 0 ? Object.entries(safeModelConfigs.reduce((acc, model) => {
                                    // Skip any models that don't have a backend property
                                    if (!model || !model.backend) return acc;
                                    
                                    if (!acc[model.backend]) acc[model.backend] = [];
                                    acc[model.backend].push(model);
                                    return acc;
                                }, {})).map(([vendor, vendorModels]) => (
                                    <React.Fragment key={vendor}>
                                        <SelectItem
                                            value={`header-${vendor}`}
                                            disabled
                                            className="persona-selector-vendor-header"
                                        >
                                            {vendor.toUpperCase()}
                                        </SelectItem>
                                        {vendorModels && vendorModels.length > 0 ? 
                                          vendorModels.map((model) => (
                                            model && model.id ? (
                                            <TooltipProvider key={model.id}>
                                                <Tooltip>
                                                    <TooltipTrigger asChild>
                                                        <SelectItem
                                                            value={model.id}
                                                            className="persona-selector-select-item"
                                                        >
                                                            {model.label}
                                                        </SelectItem>
                                                    </TooltipTrigger>
                                                    <TooltipContent className="persona-selector-tooltip-content">
                                                        <p>{model.description}</p>
                                                        <p className="persona-selector-tooltip-model-type">
                                                            Type: {model.model_type}
                                                        </p>
                                                    </TooltipContent>
                                                </Tooltip>
                                            </TooltipProvider>
                                            ) : null
                                          )) : null}
                                    </React.Fragment>
                                )) : <SelectItem value="no-models" disabled>No models available</SelectItem>}
                            </SelectContent>
                        </Select>
                    </div>

                    {/* Model Parameters */}
                    <ModelParameterControls
                        selectedModel={selectedModel}
                        onParameterChange={handleParameterChange}
                        currentParameters={modelParameters}
                    />
                </div>
            </CardContent>
        </Card>
    );
}

export default PersonaSelector;