import React, { createContext, useState, useEffect } from 'react';
import logger from '@/lib/logger';

export const ModelContext = createContext();

export const ModelProvider = ({ children }) => {
    logger.info('ModelProvider initializing', 'ModelProvider');
    
    // Model configuration state
    const [modelConfigs, setModelConfigs] = useState([]);
    const [modelName, setModelName] = useState("");
    const [selectedModel, setSelectedModel] = useState(null);
    const [modelParameters, setModelParameters] = useState({});
    
    // Save model config to localStorage
    const saveConfigToStorage = () => {
        try {
            const savedConfig = localStorage.getItem("agent_config");
            let config = savedConfig ? JSON.parse(savedConfig) : {};
            
            // Update with current model settings
            config = {
                ...config,
                modelName: modelName,
                modelParameters: modelParameters,
                lastUpdated: new Date().toISOString()
            };
            
            localStorage.setItem("agent_config", JSON.stringify(config));
            logger.debug('Saved model configuration to localStorage', 'ModelContext');
            logger.storageOp('write', 'agent_config', true);
        } catch (err) {
            logger.error('Failed to save model configuration to localStorage', 'ModelContext', { error: err.message });
            logger.storageOp('write', 'agent_config', false);
        }
    };

    // Change the current model
    const changeModel = async (newModelName) => {
        if (!modelConfigs || modelConfigs.length === 0) {
            logger.error('Cannot change model - no model configurations available', 'ModelContext');
            return false;
        }
        
        // Find the model in configurations
        const model = modelConfigs.find(m => m.id === newModelName);
        if (!model) {
            logger.error(`Model with name ${newModelName} not found in configurations`, 'ModelContext');
            return false;
        }
        
        // Set the model name and selected model
        setModelName(newModelName);
        setSelectedModel(model);
        
        // Update parameters based on selected model's defaults
        const updatedParameters = { ...modelParameters };
        
        // Handle temperature
        if (model.parameters?.temperature) {
            updatedParameters.temperature = model.parameters.temperature.default;
        }
        
        // Handle reasoning_effort for OpenAI models
        if (model.parameters?.reasoning_effort) {
            updatedParameters.reasoning_effort = model.parameters.reasoning_effort.default;
        }
        
        // Handle extended_thinking for Claude models
        if (model.parameters?.extended_thinking) {
            // Get the default enabled state
            const defaultEnabled = (typeof model.parameters.extended_thinking === 'object')
                ? model.parameters.extended_thinking.enabled
                : model.parameters.extended_thinking;
            
            updatedParameters.extended_thinking = defaultEnabled;
            
            // Set budget_tokens if extended_thinking is enabled
            if (defaultEnabled) {
                const defaultBudgetTokens = model.parameters.extended_thinking.budget_tokens?.default || 4000;
                updatedParameters.budget_tokens = defaultBudgetTokens;
            }
        }
        
        // Set the updated parameters
        setModelParameters(updatedParameters);
        
        // Save changes to localStorage
        saveConfigToStorage();
        
        logger.info('Model changed successfully', 'ModelContext', { 
            modelName: newModelName, 
            parameters: updatedParameters
        });
        
        return true;
    };
    
    // Update model parameters
    const updateModelParameters = async (parameterUpdates) => {
        if (!parameterUpdates || Object.keys(parameterUpdates).length === 0) {
            logger.warn('No parameter updates provided', 'ModelContext');
            return false;
        }
        
        // Apply parameter updates to current model parameters
        const updatedParameters = { ...modelParameters, ...parameterUpdates };
        setModelParameters(updatedParameters);
        
        // Save changes to localStorage
        saveConfigToStorage();
        
        logger.info('Model parameters updated successfully', 'ModelContext', { 
            updates: parameterUpdates, 
            newParameters: updatedParameters
        });
        
        return true;
    };
    
    return (
        <ModelContext.Provider
            value={{
                modelConfigs,
                setModelConfigs,
                modelName,
                setModelName,
                selectedModel,
                setSelectedModel,
                modelParameters,
                setModelParameters,
                changeModel,
                updateModelParameters,
                saveConfigToStorage
            }}
        >
            {children}
        </ModelContext.Provider>
    );
};