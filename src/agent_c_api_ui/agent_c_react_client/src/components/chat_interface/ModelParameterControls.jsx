import React, {useState, useEffect} from 'react';
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

    // For reasoning effort settings - OpenAI
    const [reasoningEffort, setReasoningEffort] = useState(currentParameters?.reasoning_effort);


    // Extended thinking states for Anthropic
    const [extendedThinkingEnabled, setExtendedThinkingEnabled] = useState(
        currentParameters?.extended_thinking === true ||
        (selectedModel?.parameters?.extended_thinking?.enabled === true &&
            currentParameters?.extended_thinking !== false)
    );
    const [budgetTokens, setBudgetTokens] = useState(
        (currentParameters?.extended_thinking && currentParameters?.budget_tokens) ||
        (selectedModel?.parameters?.extended_thinking?.enabled === true ?
            selectedModel?.parameters?.extended_thinking?.budget_tokens?.default : 0) || 0
    );

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
    const handleTemperatureCommit = (value) => {
        // console.log('Temperature Commit:', value);
        const temp = value[0];
        setTemperature(temp);  // Update the main temperature state
        onParameterChange('temperature', temp);
    };

    /**
     * Handles toggling the extended thinking feature
     * @param {boolean} enabled - Whether extended thinking is enabled
     */
    const handleExtendedThinkingChange = (enabled) => {
        // Update the UI state first
        setExtendedThinkingEnabled(enabled);

        // Update the parent with the enabled state
        // onParameterChange('extended_thinking', enabled);

        // If disabled, set budget_tokens to 0
        if (!enabled) {
            setBudgetTokens(0);
            onParameterChange('budget_tokens', 0);
        } else {
            // When enabling, set to default value
            const defaultValue = selectedModel?.parameters?.extended_thinking?.budget_tokens?.default || 5000;
            setBudgetTokens(defaultValue);
            onParameterChange('budget_tokens', defaultValue);
        }
    };

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
    const handleBudgetTokensCommit = (value) => {
        const tokens = value[0];
        if (tokens === 0) {
            setExtendedThinkingEnabled(false);
            // Notify parent of both changes
            // onParameterChange('extended_thinking', false);
            setBudgetTokens(0);
            onParameterChange('budget_tokens', 0);
        } else {
            // Normal case - just update budget tokens
            setBudgetTokens(tokens);
            onParameterChange('budget_tokens', tokens);
        }
    };

    useEffect(() => {
        // SUPER Defensive programming - if we don't have parameters or current parameters, don't do anything
        if (!selectedModel?.parameters || !currentParameters) {
            console.log('ModelParameterControls: Missing required parameters, skipping initialization');
            return;
        }
        
        try {    
            // Set temperature based on current or default value
            if (selectedModel.parameters?.temperature) {
                const defaultTemp = selectedModel.parameters?.temperature?.default || 0.7;
                const currentTemp = currentParameters?.temperature;
                setLocalTemperature(currentTemp !== undefined ? currentTemp : defaultTemp);
                setTemperature(currentTemp !== undefined ? currentTemp : defaultTemp);
            }

            // Set reasoning effort based on current or default value
            if (selectedModel.parameters?.reasoning_effort) {
                const defaultEffort = selectedModel.parameters?.reasoning_effort?.default || 'medium';
                const currentEffort = currentParameters?.reasoning_effort;
                setReasoningEffort(currentEffort !== undefined ? currentEffort : defaultEffort);
            }

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
        } catch (err) {
            console.error('ModelParameterControls: Error initializing parameters', err);
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
    const handleReasoningEffortChange = (value) => {
        setReasoningEffort(value);
        onParameterChange('reasoning_effort', value);
    };


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
                            className="w-full"
                            aria-labelledby={temperatureLabelId}
                            aria-valuenow={localTemperature}
                            aria-valuemin={temperatureConfig.min}
                            aria-valuemax={temperatureConfig.max}
                        />
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
                        aria-labelledby={reasoningEffortLabelId}
                    >
                        <SelectTrigger
                            id="reasoning-effort"
                            className="parameter-select-trigger"
                            aria-label="Select reasoning effort level"
                        >
                            <SelectValue placeholder="Select reasoning effort"/>
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
                        <Switch
                            id="extended-thinking"
                            checked={extendedThinkingEnabled}
                            onCheckedChange={handleExtendedThinkingChange}
                            aria-labelledby={extendedThinkingLabelId}
                            aria-describedby="extended-thinking-description"
                        />
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
                            <Slider
                                id="budget-tokens-slider"
                                min={budgetTokensConfig.min}
                                max={budgetTokensConfig.max}
                                step={budgetTokensConfig.step}
                                value={[budgetTokens]}
                                onValueChange={handleBudgetTokensChange}
                                onValueCommit={handleBudgetTokensCommit}
                                className="w-full"
                                aria-labelledby={thinkingBudgetLabelId}
                                aria-valuenow={budgetTokens}
                                aria-valuemin={budgetTokensConfig.min}
                                aria-valuemax={budgetTokensConfig.max}
                            />
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

export default ModelParameterControls;