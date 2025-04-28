import React, {useCallback, useEffect, useState, useRef} from 'react';
import { useToast } from "@/hooks/use-toast";
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
                             modelConfigs = [], // Default to empty array
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
        try {
            if (Array.isArray(modelConfigs)) {
                // Make sure each model has the required properties
                const validModels = modelConfigs.filter(model => 
                    model && model.id && model.backend && model.label
                );
                
                if (validModels.length !== modelConfigs.length) {
                    console.warn('PersonaSelector: Some models are missing required properties', {
                        total: modelConfigs.length,
                        valid: validModels.length
                    });
                }
                
                setSafeModelConfigs(validModels.length > 0 ? validModels : []);
            } else if (modelConfigs && typeof modelConfigs === 'object') {
                // Try to convert object to array if possible
                const modelsArray = Object.values(modelConfigs);
                const validModels = modelsArray.filter(model => 
                    model && model.id && model.backend && model.label
                );
                setSafeModelConfigs(validModels);
            } else {
                console.warn('PersonaSelector: modelConfigs is not an array or valid object, using empty array');
                setSafeModelConfigs([]);
            }
        } catch (err) {
            console.error('Error processing modelConfigs:', err);
            setSafeModelConfigs([]);
            setError('Error loading model configurations');
        }
    }, [modelConfigs]);

    // Local UI state only
    const [error, setError] = useState(null);
    const [selectedPersona, setSelectedPersona] = useState(persona_name || 'default');
    const [localCustomPrompt, setLocalCustomPrompt] = useState(customPrompt);
    
    // Loading state indicators
    const [modelChanging, setModelChanging] = useState(false);
    const [personaChanging, setPersonaChanging] = useState(false);
    const [promptUpdating, setPromptUpdating] = useState(false);
    const promptUpdateTimeoutRef = useRef(null);
    
    // Toast hook for feedback
    const { toast } = useToast();

    // Sync local UI state with prop
    useEffect(() => {
        if (isInitialized) {
            setSelectedPersona(persona_name);
            setLocalCustomPrompt(customPrompt);
        }
    }, [isInitialized, persona_name, customPrompt]);
    
    // Clean up the prompt update timeout on unmount
    useEffect(() => {
        return () => {
            if (promptUpdateTimeoutRef.current) {
                clearTimeout(promptUpdateTimeoutRef.current);
            }
        };
    }, []);

    /**
     * Handles persona selection changes
     * @param {string} value - Selected persona identifier
     */
    const handlePersonaChange = useCallback(async (value) => {
        setPersonaChanging(true);
        setSelectedPersona(value); // Update UI immediately for responsiveness
        
        try {
            const selectedPersonaData = personas.find(p => p.name === value);
            if (selectedPersonaData) {
                // Make the API call to update settings
                const success = await onUpdateSettings('SETTINGS_UPDATE', {
                    persona_name: value,
                    customPrompt: selectedPersonaData.content
                });
                
                if (success) {
                    // Also update the local custom prompt to match
                    setLocalCustomPrompt(selectedPersonaData.content);
                    
                    // Show success message
                    toast({
                        title: "Persona Changed",
                        description: `Now using ${selectedPersonaData.name} persona`,
                        variant: "success"
                    });
                    setError(null); // Clear any existing errors
                } else {
                    // Revert UI if the API call failed
                    setSelectedPersona(persona_name);
                    setError(`Failed to change persona to ${value}`);
                    toast({
                        title: "Persona Change Failed",
                        description: "Could not change persona. Please try again.",
                        variant: "destructive"
                    });
                }
            } else {
                setError(`Could not find persona: ${value}`);
            }
        } catch (err) {
            // Handle errors
            console.error('Error changing persona:', err);
            setError(`Error changing persona: ${err.message}`);
            // Revert UI state on error
            setSelectedPersona(persona_name);
            toast({
                title: "Error",
                description: err.message || "An unknown error occurred",
                variant: "destructive"
            });
        } finally {
            setPersonaChanging(false);
        }
    }, [personas, onUpdateSettings, toast, persona_name]);

    /**
     * Handles changes to the custom prompt textarea
     * @param {React.ChangeEvent<HTMLTextAreaElement>} e - Change event
     */
    const handleCustomPromptChange = useCallback((e) => {
        // Update local state immediately for responsive UI
        setLocalCustomPrompt(e.target.value);
        
        // Clear any existing timeout to implement debouncing
        if (promptUpdateTimeoutRef.current) {
            clearTimeout(promptUpdateTimeoutRef.current);
        }
        
        // Set a new timeout for debounced updates
        promptUpdateTimeoutRef.current = setTimeout(() => {
            handleCustomPromptUpdate(e.target.value);
        }, 1000); // 1 second debounce
    }, []);
    
    // Separate function to handle the actual update
    const handleCustomPromptUpdate = useCallback(async (value) => {
        // Skip if value hasn't changed
        if (value === customPrompt) return;
        
        setPromptUpdating(true);
        try {
            // Call the API to update the prompt
            const success = await onUpdateSettings('SETTINGS_UPDATE', {
                customPrompt: value
            });
            
            if (success) {
                // Success case - the prompt was updated
                setError(null);
            } else {
                // Failure case - revert to previous value
                setLocalCustomPrompt(customPrompt);
                setError('Failed to update custom prompt');
                toast({
                    title: "Update Failed",
                    description: "Could not update custom prompt. Please try again.",
                    variant: "destructive"
                });
            }
        } catch (err) {
            // Error handling
            console.error('Error updating custom prompt:', err);
            setLocalCustomPrompt(customPrompt); // Revert to previous value
            setError(`Error updating prompt: ${err.message}`);
            toast({
                title: "Error",
                description: err.message || "An unknown error occurred",
                variant: "destructive"
            });
        } finally {
            setPromptUpdating(false);
        }
    }, [customPrompt, onUpdateSettings, toast]);

    /**
     * Triggers settings update when custom prompt editing is complete
     */
    const handleCustomPromptBlur = useCallback(() => {
        // Only send update if the value has actually changed
        if (localCustomPrompt !== customPrompt) {
            console.log('User Changed Custom Prompt:', localCustomPrompt);
            // Clear any existing timeout first
            if (promptUpdateTimeoutRef.current) {
                clearTimeout(promptUpdateTimeoutRef.current);
                promptUpdateTimeoutRef.current = null;
            }
            // Update immediately on blur
            handleCustomPromptUpdate(localCustomPrompt);
        }
    }, [localCustomPrompt, customPrompt]);

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
    const handleModelChange = useCallback(async (selectedValue) => {
        // Debug the incoming value
        console.log('handleModelChange called with selectedValue:', selectedValue);
        setModelChanging(true);

        try {
            // Defensive check: verify safeModelConfigs exists and is an array
            if (!Array.isArray(safeModelConfigs)) {
                console.error('handleModelChange: safeModelConfigs is not an array:', safeModelConfigs);
                setError('Invalid model configuration data');
                return;
            }

            // Use our safe model configs array that's guaranteed to be an array
            const model = safeModelConfigs.find(model => model && model.id === selectedValue);
            console.log('Found model:', model);
            
            if (model && model.id && model.backend) {
                console.log('Selected model:', model.id, 'backend:', model.backend);
                if (typeof onUpdateSettings === 'function') {
                    console.log('Calling onUpdateSettings with MODEL_CHANGE action');
                    
                    // Use our enhanced API call with better success/error handling
                    const success = await onUpdateSettings('MODEL_CHANGE', {
                        modelName: model.id,
                        backend: model.backend
                    });

                    if (success) {
                        // Show success toast if we have access to toast function
                        toast({
                            title: "Model Changed",
                            description: `Now using ${model.label || model.id}`,
                            variant: "success"
                        });
                        setError(null); // Clear any existing errors
                    } else {
                        // Show error toast if we have access to toast function
                        setError(`Failed to change model to ${model.label || model.id}`);
                        toast({
                            title: "Model Change Failed",
                            description: "Could not change model. Please try again.",
                            variant: "destructive"
                        });
                    }
                } else {
                    console.error('onUpdateSettings is not a function:', onUpdateSettings);
                    setError('Internal error: Cannot update model settings');
                }
            } else {
                console.error(`Could not find valid model with ID ${selectedValue} in available models`);
                console.log('Available models:', safeModelConfigs);
                setError(`Could not find valid model with ID ${selectedValue}`);
            }
        } catch (error) {
            console.error('Error in handleModelChange:', error);
            setError(`Error changing model: ${error.message}`);
            toast({
                title: "Error",
                description: error.message || "An unknown error occurred",
                variant: "destructive"
            });
        } finally {
            setModelChanging(false);
        }
    }, [safeModelConfigs, onUpdateSettings, toast]);

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
                        disabled={personaChanging}
                        aria-label="Select a persona"
                    >
                        <SelectTrigger
                            id="persona-select"
                            className={`persona-selector-select-trigger ${personaChanging ? 'loading' : ''}`}
                            aria-label="Available personas"
                        >
                            {personaChanging ? (
                                <div className="persona-selector-loading">Loading...</div>
                            ) : (
                                <SelectValue placeholder="Select a persona"/>
                            )}
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
                    <div className="prompt-textarea-container">
                        <Textarea
                            id="custom-prompt"
                            value={localCustomPrompt}
                            onChange={handleCustomPromptChange}
                            onBlur={handleCustomPromptBlur}
                            className={`persona-selector-textarea ${promptUpdating ? 'updating' : ''}`}
                            placeholder="You are a helpful assistant."
                            aria-label="Custom persona instructions"
                            aria-describedby="custom-prompt-description"
                            disabled={promptUpdating}
                        />
                        {promptUpdating && (
                            <div className="prompt-updating-indicator">Saving...</div>
                        )}
                    </div>
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
                            disabled={modelChanging}
                            aria-label="Select an AI model"
                        >
                            <SelectTrigger
                                id="model-select"
                                className={`persona-selector-select-trigger ${modelChanging ? 'loading' : ''}`}
                                aria-label="Available AI models"
                            >
                                {modelChanging ? (
                                    <div className="persona-selector-loading">Loading...</div>
                                ) : (
                                    <SelectValue placeholder="Select model"/>
                                )}
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

// Memoize the PersonaSelector component to prevent unnecessary re-renders
const MemoizedPersonaSelector = React.memo(PersonaSelector, (prevProps, nextProps) => {
    // Only re-render if these specific props change
    return (
        prevProps.persona_name === nextProps.persona_name &&
        prevProps.customPrompt === nextProps.customPrompt &&
        prevProps.modelName === nextProps.modelName &&
        prevProps.isInitialized === nextProps.isInitialized &&
        JSON.stringify(prevProps.modelParameters) === JSON.stringify(nextProps.modelParameters)
    );
});

export default MemoizedPersonaSelector;