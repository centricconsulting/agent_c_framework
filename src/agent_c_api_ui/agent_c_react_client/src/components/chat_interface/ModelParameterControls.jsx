import React, {useState, useEffect, useCallback} from 'react';
import { useToast } from "@/hooks/use-toast";
import PropTypes from 'prop-types';
import {Label} from '@/components/ui/label';
import {Slider} from '@/components/ui/slider';
import {Switch} from '@/components/ui/switch';
import {
    Select,
    SelectContent,
    SelectItem,
    SelectTrigger,
    SelectValue,
} from '@/components/ui/select';

/**
 * ModelParameterControls provides an interface for adjusting AI model parameters
 * including temperature and reasoning effort controls.
 *
 * @component
 * @param {Object} props
 * @param {Object} props.selectedModel - Selected model configuration
 * @param {Object} props.selectedModel.parameters - Model parameter configurations
 * @param {Function} props.onParameterChange - Callback for parameter changes
 * @param {Object} [props.currentParameters={}] - Current parameter values
 */
const ModelParameterControls = ({
                                    selectedModel,
                                    onParameterChange,
                                    currentParameters = {},
                                    id = 'model-parameters'
                                }) => {
    // For temperature settings - Used by non-reasoning models
    const [temperature, setTemperature] = useState(currentParameters?.temperature);
    const [localTemperature, setLocalTemperature] = useState(currentParameters?.temperature);
    const [temperatureUpdating, setTemperatureUpdating] = useState(false);

    // For reasoning effort settings - OpenAI
    const [reasoningEffort, setReasoningEffort] = useState(currentParameters?.reasoning_effort);
    const [reasoningEffortUpdating, setReasoningEffortUpdating] = useState(false);
    
    // Toast hook for feedback
    const { toast } = useToast();


    // Extended thinking states for Anthropic
    const [extendedThinkingEnabled, setExtendedThinkingEnabled] = useState(
        currentParameters?.extended_thinking === true ||
        (selectedModel?.parameters?.extended_thinking?.enabled === true &&
            currentParameters?.extended_thinking !== false)
    );
    const [extendedThinkingUpdating, setExtendedThinkingUpdating] = useState(false);
    
    const [budgetTokens, setBudgetTokens] = useState(
        (currentParameters?.extended_thinking && currentParameters?.budget_tokens) ||
        (selectedModel?.parameters?.extended_thinking?.enabled === true ?
            selectedModel?.parameters?.extended_thinking?.budget_tokens?.default : 0) || 0
    );
    const [budgetTokensUpdating, setBudgetTokensUpdating] = useState(false);

    /**
     * Handles real-time temperature slider changes
     * @param {number[]} value - Array containing single temperature value
     */
    const handleTemperatureChange = (value) => {
        // console.log('Temperature Change:', value);
        const temp = value[0];
        setLocalTemperature(temp);
    };

    /**
     * Commits temperature changes to the backend
     * @param {number[]} value - Array containing single temperature value
     */
    const handleTemperatureCommit = useCallback(async (value) => {
        const temp = value[0];
        setTemperature(temp);  // Update the main temperature state for UI
        setTemperatureUpdating(true);
        
        try {
            // Call the API to update the temperature
            const success = await onParameterChange('temperature', temp);
            
            if (!success) {
                // Update failed - revert to previous value
                setTemperature(currentParameters?.temperature);
                setLocalTemperature(currentParameters?.temperature);
                toast({
                    title: "Update Failed",
                    description: "Could not update temperature. Please try again.",
                    variant: "destructive"
                });
            }
        } catch (err) {
            // Error handling
            console.error('Error updating temperature:', err);
            // Revert to previous value
            setTemperature(currentParameters?.temperature);
            setLocalTemperature(currentParameters?.temperature);
            toast({
                title: "Error",
                description: err.message || "An unknown error occurred",
                variant: "destructive"
            });
        } finally {
            setTemperatureUpdating(false);
        }
    }, [onParameterChange, currentParameters?.temperature, toast]);

    /**
     * Handles toggling the extended thinking feature
     * @param {boolean} enabled - Whether extended thinking is enabled
     */
    const handleExtendedThinkingChange = useCallback(async (enabled) => {
        // Update the UI state first for responsiveness
        setExtendedThinkingEnabled(enabled);
        setExtendedThinkingUpdating(true);
        
        try {
            let success;
            
            // If disabled, set budget_tokens to 0
            if (!enabled) {
                setBudgetTokens(0);
                success = await onParameterChange('budget_tokens', 0);
            } else {
                // When enabling, set to default value
                const defaultValue = selectedModel?.parameters?.extended_thinking?.budget_tokens?.default || 5000;
                setBudgetTokens(defaultValue);
                success = await onParameterChange('budget_tokens', defaultValue);
            }
            
            if (!success) {
                // Update failed - revert to previous state
                const previousState = !enabled;
                setExtendedThinkingEnabled(previousState);
                toast({
                    title: "Update Failed",
                    description: `Could not ${enabled ? 'enable' : 'disable'} extended thinking. Please try again.`,
                    variant: "destructive"
                });
            }
        } catch (err) {
            // Error handling
            console.error('Error updating extended thinking:', err);
            // Revert to previous state
            setExtendedThinkingEnabled(!enabled);
            toast({
                title: "Error",
                description: err.message || "An unknown error occurred",
                variant: "destructive"
            });
        } finally {
            setExtendedThinkingUpdating(false);
        }
    }, [onParameterChange, selectedModel?.parameters?.extended_thinking?.budget_tokens?.default, toast]);

    /**
     * Handles changes to the budget tokens slider
     * @param {number[]} value - Array containing single budget tokens value
     */
    const handleBudgetTokensChange = (value) => {
        const tokens = value[0];
        setBudgetTokens(tokens);
    };

    /**
     * Commits budget tokens changes to the backend
     * @param {number[]} value - Array containing single budget tokens value
     */
    const handleBudgetTokensCommit = useCallback(async (value) => {
        const tokens = value[0];
        setBudgetTokensUpdating(true);
        
        try {
            let success;
            
            if (tokens === 0) {
                setExtendedThinkingEnabled(false);
                setBudgetTokens(0);
                success = await onParameterChange('budget_tokens', 0);
            } else {
                // Normal case - just update budget tokens
                setBudgetTokens(tokens);
                success = await onParameterChange('budget_tokens', tokens);
            }
            
            if (!success) {
                // Update failed - revert to previous values
                const previousTokens = currentParameters?.budget_tokens || 0;
                setBudgetTokens(previousTokens);
                setExtendedThinkingEnabled(previousTokens > 0);
                toast({
                    title: "Update Failed",
                    description: "Could not update thinking budget. Please try again.",
                    variant: "destructive"
                });
            }
        } catch (err) {
            // Error handling
            console.error('Error updating budget tokens:', err);
            // Revert to previous values
            const previousTokens = currentParameters?.budget_tokens || 0;
            setBudgetTokens(previousTokens);
            setExtendedThinkingEnabled(previousTokens > 0);
            toast({
                title: "Error",
                description: err.message || "An unknown error occurred",
                variant: "destructive"
            });
        } finally {
            setBudgetTokensUpdating(false);
        }
    }, [onParameterChange, currentParameters?.budget_tokens, toast]);

    useEffect(() => {
        // console.log('Current Parameters:', currentParameters);
        // console.log('Selected Model Parameters:', selectedModel?.parameters);
        if (selectedModel?.parameters) {
            // Set temperature based on current or default value
            const defaultTemp = selectedModel.parameters?.temperature?.default;
            console.log('Default Temperature:', defaultTemp);
            setLocalTemperature(currentParameters?.temperature ?? defaultTemp);

            // Set reasoning effort based on current or default value
            const defaultEffort = selectedModel.parameters?.reasoning_effort?.default;
            setReasoningEffort(currentParameters?.reasoning_effort ?? defaultEffort);

            // Set extended thinking parameters based on model config or default
            if (selectedModel.parameters?.extended_thinking) {
                const defaultEnabled = selectedModel.parameters.extended_thinking.enabled === true;
                const currentEnabled = currentParameters?.extended_thinking;
                setExtendedThinkingEnabled(currentEnabled !== undefined ? currentEnabled : defaultEnabled);

                const defaultBudget = selectedModel.parameters.extended_thinking.budget_tokens?.default || 5000;
                const currentBudget = currentParameters?.budget_tokens;
                setBudgetTokens(currentEnabled === false ? 0 : (currentBudget !== undefined ? currentBudget : defaultBudget));
            } else {
                setExtendedThinkingEnabled(false);
                setBudgetTokens(0);
            }
        }
    }, [selectedModel, currentParameters]);

    if (!selectedModel?.parameters) return null;

    /**
     * Temperature slider configuration
     * @type {Object}
     * @property {number} min - Minimum temperature value
     * @property {number} max - Maximum temperature value
     * @property {number} step - Temperature adjustment increment, fixed at 0.1
     * @property {number} default - Default temperature value
     */
    const temperatureConfig = {
        min: selectedModel.parameters?.temperature?.min,
        max: selectedModel.parameters?.temperature?.max,
        step: 0.1,
        default: selectedModel.parameters?.temperature?.default
    };
    // console.log('Temperature Config:', temperatureConfig);
    // console.log('Slider Props:', {
    //     min: temperatureConfig.min,
    //     max: temperatureConfig.max,
    //     step: temperatureConfig.step,
    //     value: localTemperature
    // });

    /**
     * Retrieves reasoning effort options from model configuration
     * @returns {string[]|null} Array of reasoning effort options or null if not configured
     */
    const getReasoningEffortOptions = () => {
        const config = selectedModel.parameters?.reasoning_effort;
        if (!config) return null;
        if (Array.isArray(config.options)) return config.options;
        if (Array.isArray(config)) return config;
        if (typeof config === 'object') {
            return config.values || config.choices || ['low', 'medium', 'high'];
        }
        return ['low', 'medium', 'high'];
    };

    const reasoningEffortOptions = getReasoningEffortOptions();

    /**
     * Handles changes to reasoning effort selection - OpenAI
     * @param {string} value - Selected reasoning effort level
     */
    const handleReasoningEffortChange = useCallback(async (value) => {
        // Update local state immediately for UI responsiveness
        setReasoningEffort(value);
        setReasoningEffortUpdating(true);
        
        try {
            // Call the API to update the reasoning effort
            const success = await onParameterChange('reasoning_effort', value);
            
            if (!success) {
                // Update failed - revert to previous value
                setReasoningEffort(currentParameters?.reasoning_effort);
                toast({
                    title: "Update Failed",
                    description: "Could not update reasoning effort. Please try again.",
                    variant: "destructive"
                });
            }
        } catch (err) {
            // Error handling
            console.error('Error updating reasoning effort:', err);
            // Revert to previous value
            setReasoningEffort(currentParameters?.reasoning_effort);
            toast({
                title: "Error",
                description: err.message || "An unknown error occurred",
                variant: "destructive"
            });
        } finally {
            setReasoningEffortUpdating(false);
        }
    }, [onParameterChange, currentParameters?.reasoning_effort, toast]);


    /**
     * Budget tokens slider configuration
     * @type {Object}
     * @property {number} min - Minimum budget tokens value
     * @property {number} max - Maximum budget tokens value
     * @property {number} step - Budget tokens adjustment increment
     * @property {number} default - Default budget tokens value
     */
    const budgetTokensConfig = {
        min: selectedModel.parameters?.extended_thinking?.budget_tokens?.min,
        max: selectedModel.parameters?.extended_thinking?.budget_tokens?.max,
        step: 500,
        default: selectedModel.parameters?.extended_thinking?.budget_tokens?.default
    };

    const parametersGroupId = `${id}-controls`;
    const temperatureLabelId = `${id}-temperature-label`;
    const reasoningEffortLabelId = `${id}-reasoning-effort-label`;
    const extendedThinkingLabelId = `${id}-extended-thinking-label`;
    const thinkingBudgetLabelId = `${id}-thinking-budget-label`;

    return (
        <div 
            className="parameter-controls-container"
            id={id}
            role="group" 
            aria-label="Model parameter controls"
        >
            {selectedModel.parameters?.temperature && (
                <div 
                    className="parameter-section"
                    role="group" 
                    aria-labelledby={temperatureLabelId}
                >
                    <div className="parameter-header">
                        <Label className="parameter-label" id={temperatureLabelId}>Temperature</Label>
                        <span className="parameter-value-badge" aria-live="polite">
                            {localTemperature.toFixed(1)}
                        </span>
                    </div>
                    <div className="parameter-slider-container">
                        <div className="parameter-slider-labels" aria-hidden="true">
                            <span>Focused</span>
                            <span>Balanced</span>
                            <span>Creative</span>
                        </div>
                        <div className="parameter-slider-markers" aria-hidden="true">
                            <div className="parameter-slider-marker"></div>
                            <div className="parameter-slider-marker"></div>
                            <div className="parameter-slider-marker"></div>
                            <div className="parameter-slider-marker"></div>
                            <div className="parameter-slider-marker"></div>
                        </div>
                        <Slider
                            id="temperature-slider"
                            min={temperatureConfig.min}
                            max={temperatureConfig.max}
                            step={temperatureConfig.step}
                            value={[localTemperature]}
                            onValueChange={handleTemperatureChange}  // Smooth UI updates
                            onValueCommit={handleTemperatureCommit}  // Backend update on finish
                            className={`w-full ${temperatureUpdating ? 'updating' : ''}`}
                            disabled={temperatureUpdating}
                            aria-labelledby={temperatureLabelId}
                            aria-valuenow={localTemperature}
                            aria-valuemin={temperatureConfig.min}
                            aria-valuemax={temperatureConfig.max}
                        />
                        {temperatureUpdating && (
                            <div className="parameter-updating-indicator">Updating...</div>
                        )}
                    </div>
                    <div className="parameter-helper-text" id="temperature-helper">
                        Higher values make output more creative but less predictable
                    </div>
                </div>
            )}

            {selectedModel.parameters?.reasoning_effort && reasoningEffortOptions && (
                <div 
                    className="parameter-section" 
                    role="group" 
                    aria-labelledby={reasoningEffortLabelId}
                >
                    <Label 
                        htmlFor="reasoning-effort" 
                        className="parameter-label"
                        id={reasoningEffortLabelId}
                    >
                        Reasoning Effort
                    </Label>
                    <Select
                        value={reasoningEffort}
                        onValueChange={handleReasoningEffortChange}
                        disabled={reasoningEffortUpdating}
                        aria-labelledby={reasoningEffortLabelId}
                    >
                        <SelectTrigger
                            id="reasoning-effort"
                            className={`parameter-select-trigger ${reasoningEffortUpdating ? 'updating' : ''}`}
                            aria-label="Select reasoning effort level"
                        >
                            {reasoningEffortUpdating ? (
                                <div className="parameter-updating">Updating...</div>
                            ) : (
                                <SelectValue placeholder="Select reasoning effort"/>
                            )}
                        </SelectTrigger>
                        <SelectContent className="parameter-select-content">
                            {reasoningEffortOptions.map(option => (
                                <SelectItem
                                    key={option}
                                    value={option}
                                    className="parameter-select-item"
                                >
                                    {option.charAt(0).toUpperCase() + option.slice(1)}
                                </SelectItem>
                            ))}
                        </SelectContent>
                    </Select>
                </div>
            )}

            {selectedModel.parameters?.extended_thinking && (
                <div 
                    className="parameter-section"
                    role="group" 
                    aria-labelledby={extendedThinkingLabelId}
                >
                    <div className="parameter-header">
                        <Label 
                            htmlFor="extended-thinking" 
                            className="parameter-label"
                            id={extendedThinkingLabelId}
                        >
                            Extended Thinking
                        </Label>
                        <div className="switch-container">
                            <Switch
                                id="extended-thinking"
                                checked={extendedThinkingEnabled}
                                onCheckedChange={handleExtendedThinkingChange}
                                disabled={extendedThinkingUpdating}
                                className={extendedThinkingUpdating ? 'updating' : ''}
                                aria-labelledby={extendedThinkingLabelId}
                                aria-describedby="extended-thinking-description"
                            />
                            {extendedThinkingUpdating && (
                                <div className="parameter-updating-indicator">Updating...</div>
                            )}
                        </div>
                    </div>
                    <div 
                        className="parameter-helper-text"
                        id="extended-thinking-description"
                    >
                        Enable deep reasoning for complex problems
                    </div>

                    {extendedThinkingEnabled && (
                        <div 
                            className="extended-thinking-section"
                            role="group" 
                            aria-labelledby={thinkingBudgetLabelId}
                        >
                            <div className="parameter-header">
                                <Label 
                                    className="parameter-label"
                                    id={thinkingBudgetLabelId}
                                    htmlFor="budget-tokens-slider"
                                >
                                    Thinking Budget
                                </Label>
                                <span 
                                    className="parameter-value-badge"
                                    aria-live="polite"
                                >
                                    {budgetTokens.toLocaleString()} tokens
                                </span>
                            </div>
                            <div className="slider-container">
                                <Slider
                                    id="budget-tokens-slider"
                                    min={budgetTokensConfig.min}
                                    max={budgetTokensConfig.max}
                                    step={budgetTokensConfig.step}
                                    value={[budgetTokens]}
                                    onValueChange={handleBudgetTokensChange}
                                    onValueCommit={handleBudgetTokensCommit}
                                    disabled={budgetTokensUpdating}
                                    className={`w-full ${budgetTokensUpdating ? 'updating' : ''}`}
                                    aria-labelledby={thinkingBudgetLabelId}
                                    aria-valuenow={budgetTokens}
                                    aria-valuemin={budgetTokensConfig.min}
                                    aria-valuemax={budgetTokensConfig.max}
                                />
                                {budgetTokensUpdating && (
                                    <div className="parameter-updating-indicator">Updating...</div>
                                )}
                            </div>
                            <div 
                                className="parameter-helper-text"
                                id="thinking-budget-description"
                            >
                                Higher values allow more thorough analysis but may take longer
                            </div>
                        </div>
                    )}
                </div>
            )}
        </div>
    );
};

ModelParameterControls.propTypes = {
    /**
     * Selected model configuration object containing parameter definitions
     */
    selectedModel: PropTypes.shape({
        parameters: PropTypes.shape({
            temperature: PropTypes.shape({
                min: PropTypes.number,
                max: PropTypes.number,
                default: PropTypes.number,
            }),
            reasoning_effort: PropTypes.oneOfType([
                PropTypes.array,
                PropTypes.shape({
                    options: PropTypes.array,
                    default: PropTypes.string,
                })
            ]),
            extended_thinking: PropTypes.shape({
                enabled: PropTypes.bool,
                budget_tokens: PropTypes.shape({
                    min: PropTypes.number,
                    max: PropTypes.number,
                    default: PropTypes.number,
                })
            })
        })
    }).isRequired,
    
    /**
     * Callback function for parameter changes
     */
    onParameterChange: PropTypes.func.isRequired,
    
    /**
     * Current parameter values
     */
    currentParameters: PropTypes.shape({
        temperature: PropTypes.number,
        reasoning_effort: PropTypes.string,
        extended_thinking: PropTypes.bool,
        budget_tokens: PropTypes.number,
    }),
    
    /**
     * Unique identifier for the component
     */
    id: PropTypes.string,
};

// Memoize the component to prevent unnecessary re-renders
const MemoizedModelParameterControls = React.memo(ModelParameterControls, (prevProps, nextProps) => {
    // Only re-render if model or parameters change
    return (
        prevProps.selectedModel?.id === nextProps.selectedModel?.id &&
        JSON.stringify(prevProps.currentParameters) === JSON.stringify(nextProps.currentParameters)
    );
});

export default MemoizedModelParameterControls;