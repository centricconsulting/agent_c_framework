import React, { createContext, useReducer, useContext, useEffect, useMemo } from 'react';
import logger from '@/lib/logger';
import { createInitTracer } from '@/lib/initTracer';
import eventBus from '@/lib/eventBus';

// Define initialization states
export const InitState = {
  NOT_STARTED: 'not_started',
  AUTH_PENDING: 'auth_pending',
  AUTH_COMPLETE: 'auth_complete',
  MODEL_PENDING: 'model_pending',
  MODEL_COMPLETE: 'model_complete',
  SESSION_PENDING: 'session_pending',
  COMPLETE: 'complete',
  ERROR: 'error'
};

// Define initialization events
export const InitEvents = {
  STATE_CHANGED: 'initialization:state_changed',
  ERROR: 'initialization:error',
  COMPLETE: 'initialization:complete',
  PROGRESS: 'initialization:progress',
  TIMEOUT: 'initialization:timeout',
  RESET: 'initialization:reset'
};

// Initial state for the initialization reducer
const initialState = {
  state: InitState.NOT_STARTED,
  error: null,
  progress: 0,
  startTime: null,
  completionTime: null,
  phases: {
    auth: {
      state: InitState.NOT_STARTED,
      error: null,
      startTime: null,
      completionTime: null
    },
    model: {
      state: InitState.NOT_STARTED,
      error: null,
      startTime: null,
      completionTime: null
    },
    session: {
      state: InitState.NOT_STARTED,
      error: null,
      startTime: null,
      completionTime: null
    }
  }
};

// Reducer action types
const ACTIONS = {
  SET_STATE: 'SET_STATE',
  SET_ERROR: 'SET_ERROR',
  SET_PROGRESS: 'SET_PROGRESS',
  RESET: 'RESET',
  SET_PHASE_STATE: 'SET_PHASE_STATE',
  SET_PHASE_ERROR: 'SET_PHASE_ERROR'
};

// Reducer function
function initializationReducer(state, action) {
  const now = new Date().getTime();
  
  switch (action.type) {
    case ACTIONS.SET_STATE:
      // When overall state changes
      const newState = action.payload;
      logger.debug(`Initialization state changing to: ${newState}`, 'InitializationContext', {
        previousState: state.state,
        newState
      });
      
      let stateUpdate = { state: newState };
      
      // Set start time if transitioning from NOT_STARTED
      if (state.state === InitState.NOT_STARTED && newState !== InitState.NOT_STARTED) {
        stateUpdate.startTime = now;
      }
      
      // Set completion time if transitioning to COMPLETE
      if (newState === InitState.COMPLETE) {
        stateUpdate.completionTime = now;
        stateUpdate.progress = 100;
      }
      
      // Calculate progress based on state
      if (!stateUpdate.progress) {
        switch (newState) {
          case InitState.AUTH_PENDING:
            stateUpdate.progress = 10;
            break;
          case InitState.AUTH_COMPLETE:
            stateUpdate.progress = 30;
            break;
          case InitState.MODEL_PENDING:
            stateUpdate.progress = 40;
            break;
          case InitState.MODEL_COMPLETE:
            stateUpdate.progress = 70;
            break;
          case InitState.SESSION_PENDING:
            stateUpdate.progress = 80;
            break;
          case InitState.COMPLETE:
            stateUpdate.progress = 100;
            break;
          default:
            // Keep current progress for other states
            stateUpdate.progress = state.progress;
        }
      }
      
      return { ...state, ...stateUpdate };
      
    case ACTIONS.SET_ERROR:
      // When an error occurs
      logger.error(`Initialization error: ${action.payload}`, 'InitializationContext');
      return { 
        ...state, 
        state: InitState.ERROR, 
        error: action.payload 
      };
      
    case ACTIONS.SET_PROGRESS:
      // Manually set progress percent
      return { ...state, progress: action.payload };
      
    case ACTIONS.RESET:
      // Reset to initial state
      logger.info('Resetting initialization state', 'InitializationContext');
      return { ...initialState };
      
    case ACTIONS.SET_PHASE_STATE:
      // When a specific phase's state changes
      const { phase, phaseState } = action.payload;
      logger.debug(`Initialization phase '${phase}' state changing to: ${phaseState}`, 'InitializationContext', {
        phase,
        previousState: state.phases[phase]?.state,
        newState: phaseState
      });
      
      // Cannot update non-existent phase
      if (!state.phases[phase]) {
        logger.error(`Cannot update non-existent phase: ${phase}`, 'InitializationContext');
        return state;
      }
      
      const phaseUpdate = { state: phaseState };
      
      // Set start time if transitioning from NOT_STARTED
      if (state.phases[phase].state === InitState.NOT_STARTED && phaseState !== InitState.NOT_STARTED) {
        phaseUpdate.startTime = now;
      }
      
      // Set completion time if transitioning to COMPLETE
      if (phaseState === 'complete') {
        phaseUpdate.completionTime = now;
      }
      
      return {
        ...state,
        phases: {
          ...state.phases,
          [phase]: {
            ...state.phases[phase],
            ...phaseUpdate
          }
        }
      };
      
    case ACTIONS.SET_PHASE_ERROR:
      // When a phase encounters an error
      const { errorPhase, error } = action.payload;
      logger.error(`Initialization phase '${errorPhase}' error: ${error}`, 'InitializationContext');
      
      return {
        ...state,
        phases: {
          ...state.phases,
          [errorPhase]: {
            ...state.phases[errorPhase],
            state: 'error',
            error: error
          }
        }
      };
      
    default:
      return state;
  }
}

// Create the context
export const InitializationContext = createContext();

/**
 * Provider component that manages the initialization state of the application
 */
export const InitializationProvider = ({ children }) => {
  const [state, dispatch] = useReducer(initializationReducer, initialState);
  
  // Create a tracer for debugging
  const tracer = useMemo(() => createInitTracer('InitializationContext'), []);
  
  // Action creator helpers
  const setInitState = (newState) => {
    dispatch({ type: ACTIONS.SET_STATE, payload: newState });
    eventBus.publish(InitEvents.STATE_CHANGED, { state: newState });
    tracer.setState(newState);
  };
  
  const setInitError = (error) => {
    dispatch({ type: ACTIONS.SET_ERROR, payload: error });
    eventBus.publish(InitEvents.ERROR, { error });
    tracer.setError(error);
  };
  
  const setInitProgress = (progress) => {
    dispatch({ type: ACTIONS.SET_PROGRESS, payload: progress });
    eventBus.publish(InitEvents.PROGRESS, { progress });
  };
  
  const resetInitialization = () => {
    dispatch({ type: ACTIONS.RESET });
    eventBus.publish(InitEvents.RESET);
    tracer.setState(InitState.NOT_STARTED);
  };
  
  const setPhaseState = (phase, phaseState) => {
    dispatch({ 
      type: ACTIONS.SET_PHASE_STATE, 
      payload: { phase, phaseState } 
    });
    
    const eventData = { phase, state: phaseState };
    eventBus.publish(`${InitEvents.STATE_CHANGED}:${phase}`, eventData);
    tracer.setState(`${phase}_${phaseState}`);
    
    // If a phase completes, check if all phases are complete
    if (phaseState === 'complete') {
      checkAllPhasesComplete();
    }
  };
  
  const setPhaseError = (phase, error) => {
    dispatch({ 
      type: ACTIONS.SET_PHASE_ERROR, 
      payload: { errorPhase: phase, error } 
    });
    
    const eventData = { phase, error };
    eventBus.publish(`${InitEvents.ERROR}:${phase}`, eventData);
    tracer.setError(error);
  };
  
  // Helper to check if all phases are complete
  const checkAllPhasesComplete = () => {
    const { auth, model, session } = state.phases;
    
    if (auth.state === 'complete' && model.state === 'complete' && session.state === 'complete') {
      setInitState(InitState.COMPLETE);
      eventBus.publish(InitEvents.COMPLETE);
    }
  };
  
  // Start initialization sequence when component mounts
  useEffect(() => {
    logger.info('Initialization sequence starting', 'InitializationContext');
    setInitState(InitState.AUTH_PENDING);
    setPhaseState('auth', 'pending');
    
    // Return cleanup function
    return () => {
      // Any cleanup needed
    };
  }, []);
  
  // Provide context value to children
  const contextValue = {
    // State
    state: state.state,
    error: state.error,
    progress: state.progress,
    phases: state.phases,
    isComplete: state.state === InitState.COMPLETE,
    hasError: state.state === InitState.ERROR,
    
    // Timing information
    startTime: state.startTime,
    completionTime: state.completionTime,
    duration: state.completionTime ? (state.completionTime - state.startTime) : null,
    
    // Methods for managing initialization state
    setInitState,
    setInitError,
    setInitProgress,
    resetInitialization,
    setPhaseState,
    setPhaseError,
    
    // Phase state helpers
    startAuthPhase: () => setPhaseState('auth', 'pending'),
    completeAuthPhase: () => setPhaseState('auth', 'complete'),
    authError: (error) => setPhaseError('auth', error),
    
    startModelPhase: () => setPhaseState('model', 'pending'),
    completeModelPhase: () => setPhaseState('model', 'complete'),
    modelError: (error) => setPhaseError('model', error),
    
    startSessionPhase: () => setPhaseState('session', 'pending'),
    completeSessionPhase: () => setPhaseState('session', 'complete'),
    sessionError: (error) => setPhaseError('session', error),
  };
  
  return (
    <InitializationContext.Provider value={contextValue}>
      {children}
    </InitializationContext.Provider>
  );
};

/**
 * Hook for accessing the initialization context
 */
export const useInitialization = () => {
  const context = useContext(InitializationContext);
  if (context === undefined) {
    throw new Error('useInitialization must be used within an InitializationProvider');
  }
  return context;
};