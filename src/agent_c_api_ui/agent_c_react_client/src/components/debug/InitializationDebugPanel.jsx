/**
 * Initialization Debug Panel
 * 
 * This component displays real-time information about the initialization state
 * of various contexts in the application. It provides visibility into the
 * initialization sequence, state transitions, and any errors that occur.
 * 
 * This panel is conditionally rendered based on a debug flag and should only
 * appear in development environments.
 */

import React, { useState, useEffect } from 'react';
import { globalInitTracker, InitState } from '@/lib/initTracer';
import eventBus from '@/lib/eventBus';
import logger from '@/lib/logger';

// Styles for the debug panel
const styles = {
  container: {
    position: 'fixed',
    bottom: '10px',
    right: '10px',
    width: '350px',
    backgroundColor: 'rgba(0, 0, 0, 0.8)',
    color: 'white',
    borderRadius: '5px',
    padding: '10px',
    boxShadow: '0 0 10px rgba(0, 0, 0, 0.5)',
    fontFamily: 'monospace',
    fontSize: '12px',
    zIndex: 9999,
    maxHeight: '400px',
    overflowY: 'auto'
  },
  header: {
    display: 'flex',
    justifyContent: 'space-between',
    alignItems: 'center',
    borderBottom: '1px solid #555',
    marginBottom: '5px',
    paddingBottom: '5px'
  },
  title: {
    margin: 0,
    fontSize: '14px',
    fontWeight: 'bold'
  },
  closeButton: {
    background: 'none',
    border: 'none',
    color: 'white',
    cursor: 'pointer',
    fontWeight: 'bold'
  },
  contextItem: {
    marginBottom: '5px',
    paddingBottom: '5px',
    borderBottom: '1px solid #444'
  },
  contextName: {
    fontWeight: 'bold',
    marginBottom: '2px'
  },
  stateLabel: {
    display: 'inline-block',
    padding: '2px 4px',
    borderRadius: '3px',
    fontSize: '10px',
    marginRight: '5px'
  },
  stateUnknown: {
    backgroundColor: '#555'
  },
  stateInitializing: {
    backgroundColor: '#0099cc'
  },
  stateReady: {
    backgroundColor: '#009900'
  },
  stateError: {
    backgroundColor: '#cc0000'
  },
  stateAuth: {
    backgroundColor: '#9900cc'
  },
  stateModel: {
    backgroundColor: '#ff9900'
  },
  stateSession: {
    backgroundColor: '#00cccc'
  },
  timeInfo: {
    color: '#aaa',
    fontSize: '10px',
    marginTop: '2px'
  },
  toggleButton: {
    position: 'fixed',
    bottom: '10px',
    right: '10px',
    backgroundColor: 'rgba(0, 0, 0, 0.7)',
    color: 'white',
    border: 'none',
    borderRadius: '5px',
    padding: '5px 10px',
    cursor: 'pointer',
    fontSize: '12px',
    zIndex: 9999
  },
  historyTitle: {
    borderTop: '1px solid #555',
    marginTop: '10px',
    paddingTop: '5px',
    fontWeight: 'bold',
    fontSize: '11px'
  },
  historyItem: {
    padding: '2px 5px',
    borderBottom: '1px solid #333',
    fontSize: '10px'
  }
};

/**
 * Helper function to get style for a state
 * @param {string} state - Current state
 * @returns {Object} Combined styles for the state
 */
const getStateStyle = (state) => {
  if (!state) return { ...styles.stateLabel, ...styles.stateUnknown };
  
  if (state.includes('error')) {
    return { ...styles.stateLabel, ...styles.stateError };
  } else if (state === InitState.READY) {
    return { ...styles.stateLabel, ...styles.stateReady };
  } else if (state.startsWith('auth_')) {
    return { ...styles.stateLabel, ...styles.stateAuth };
  } else if (state.startsWith('model_')) {
    return { ...styles.stateLabel, ...styles.stateModel };
  } else if (state.startsWith('session_')) {
    return { ...styles.stateLabel, ...styles.stateSession };
  } else if (state === InitState.INITIALIZING) {
    return { ...styles.stateLabel, ...styles.stateInitializing };
  }
  
  return { ...styles.stateLabel, ...styles.stateUnknown };
};

/**
 * Format milliseconds as a readable string
 * @param {number} ms - Milliseconds
 * @returns {string} Formatted time string
 */
const formatTime = (ms) => {
  if (ms === undefined || ms === null) return 'n/a';
  if (ms < 1000) return `${ms}ms`;
  return `${(ms / 1000).toFixed(1)}s`;
};

/**
 * Initialization Debug Panel Component
 */
const InitializationDebugPanel = ({ isVisible = true }) => {
  // State for panel visibility
  const [showPanel, setShowPanel] = useState(isVisible);
  
  // State for context states and transition history
  const [contextStates, setContextStates] = useState(new Map());
  const [stateHistory, setStateHistory] = useState([]);
  
  // Update state from tracker periodically
  useEffect(() => {
    // Skip if panel is hidden
    if (!showPanel) return;
    
    const updateInterval = setInterval(() => {
      // Get latest states from tracker
      const latestStates = globalInitTracker.getAllContextStates();
      setContextStates(new Map(latestStates));
      
      // Get state history from logger
      const history = logger.getStateHistory();
      setStateHistory(history);
    }, 500);
    
    return () => clearInterval(updateInterval);
  }, [showPanel]);
  
  // Subscribe to initialization events
  useEffect(() => {
    // Skip if panel is hidden
    if (!showPanel) return;
    
    const unsubscribe = eventBus.subscribe('init_state_changed', (data) => {
      // Update immediately on state changes for more responsive UI
      setContextStates(prevStates => {
        const newStates = new Map(prevStates);
        const { contextName, newState } = data;
        
        if (contextName && newState) {
          newStates.set(contextName, {
            state: newState,
            timestamp: Date.now(),
            details: data
          });
        }
        
        return newStates;
      });
    }, { componentName: 'InitializationDebugPanel' });
    
    return () => unsubscribe();
  }, [showPanel]);
  
  // If panel is hidden, only show the toggle button
  if (!showPanel) {
    return (
      <button 
        style={styles.toggleButton}
        onClick={() => setShowPanel(true)}
      >
        Show Init Debug
      </button>
    );
  }
  
  // Calculate overall status
  const allContexts = Array.from(contextStates.entries());
  const hasErrors = allContexts.some(([_, data]) => data.state.includes('error'));
  const allReady = allContexts.length > 0 && allContexts.every(([_, data]) => data.state === InitState.READY);
  const status = hasErrors ? 'Error' : allReady ? 'Ready' : 'Initializing';
  
  return (
    <div style={styles.container}>
      <div style={styles.header}>
        <h3 style={styles.title}>Initialization Debug {status}</h3>
        <button style={styles.closeButton} onClick={() => setShowPanel(false)}>×</button>
      </div>
      
      {/* Display each context with its state */}
      {allContexts.length === 0 ? (
        <div>No context initialization data available yet.</div>
      ) : (
        allContexts.map(([contextName, data]) => {
          const { state, timestamp, details } = data;
          const elapsed = details?.elapsed || 0;
          
          return (
            <div key={contextName} style={styles.contextItem}>
              <div style={styles.contextName}>{contextName}</div>
              <div>
                <span style={getStateStyle(state)}>{state}</span>
                {details?.previousState && (
                  <span style={{ fontSize: '10px', color: '#999' }}>
                    from {details.previousState}
                  </span>
                )}
              </div>
              <div style={styles.timeInfo}>
                Elapsed: {formatTime(elapsed)}
                {details?.timeSinceLastUpdate && (
                  <span> (Last change: {formatTime(details.timeSinceLastUpdate)})</span>
                )}
              </div>
            </div>
          );
        })
      )}
      
      {/* State transition history */}
      {stateHistory.length > 0 && (
        <div>
          <div style={styles.historyTitle}>Recent State Transitions</div>
          {stateHistory.slice(-5).map((entry, index) => (
            <div key={index} style={styles.historyItem}>
              <div>
                <span style={getStateStyle(entry.newState)}>
                  {entry.contextName}: {entry.previousState} → {entry.newState}
                </span>
              </div>
              <div style={{ fontSize: '9px', color: '#888' }}>
                {new Date(entry.timestamp).toLocaleTimeString()} 
                ({formatTime(entry.durationMs)})
              </div>
            </div>
          ))}
        </div>
      )}
    </div>
  );
};

export default InitializationDebugPanel;