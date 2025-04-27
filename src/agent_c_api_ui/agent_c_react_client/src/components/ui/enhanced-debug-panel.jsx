import React, { useEffect, useState } from 'react';
import { useSessionContext } from '@/hooks/use-session-context';
import { useAuth } from '@/hooks/use-auth';
import { useModel } from '@/hooks/use-model';
import { useTheme } from '@/hooks/use-theme';
import storageService from '@/lib/storageService';
import { getContextInitializationStatus, DEBUG_MODE } from '@/lib/diagnostic';
import { safeInspect } from '@/lib/safeSerializer';

/**
 * Enhanced Debug Panel for monitoring application state and diagnosing issues
 */
const EnhancedDebugPanel = () => {
  const [isExpanded, setIsExpanded] = useState(false);
  const [isVisible, setIsVisible] = useState(false);
  const [lastUpdated, setLastUpdated] = useState(Date.now());
  const [contextStatus, setContextStatus] = useState({});
  
  // Access all contexts
  const session = useSessionContext('DebugPanel');
  const auth = useAuth('DebugPanel');
  const model = useModel('DebugPanel');
  const { theme } = useTheme('DebugPanel');
  
  // Check if localStorage has debugging enabled
  useEffect(() => {
    const checkDebugEnabled = () => {
      const debugEnabled = localStorage.getItem('enableDebugPanel') === 'true';
      setIsVisible(debugEnabled);
    };
    
    // Initial check
    checkDebugEnabled();
    
    // Setup storage event listener to detect changes
    const handleStorageChange = (e) => {
      if (e.key === 'enableDebugPanel') {
        checkDebugEnabled();
      }
    };
    
    window.addEventListener('storage', handleStorageChange);
    
    // Also create a global toggle function
    window.toggleDebugPanel = () => {
      const current = localStorage.getItem('enableDebugPanel') === 'true';
      localStorage.setItem('enableDebugPanel', (!current).toString());
      checkDebugEnabled();
    };
    
    return () => {
      window.removeEventListener('storage', handleStorageChange);
    };
  }, []);
  
  // Update context status periodically, but only when expanded
  useEffect(() => {
    // If not expanded, don't update as frequently
    if (!isExpanded) {
      return;
    }
    
    const updateContextStatus = () => {
      // Use safe copy to prevent circular references and memory issues
      const status = getContextInitializationStatus();
      // Only store a limited subset of the context status information
      const safeStatus = {};
      
      Object.keys(status).forEach(key => {
        safeStatus[key] = {
          status: status[key].status,
          startTime: status[key].startTime,
          complete: status[key].complete,
          hasErrors: status[key].errors && status[key].errors.length > 0
        };
      });
      
      setContextStatus(safeStatus);
      setLastUpdated(Date.now());
    };
    
    // Initial update
    updateContextStatus();
    
    // Update less frequently to reduce performance impact
    const intervalId = setInterval(updateContextStatus, 2000);
    
    return () => clearInterval(intervalId);
  }, [isExpanded]);
  
  // Get session ID from storage
  const storedSessionId = storageService.getSessionId();
  
  // Check if chat interface is visible
  const isChatInterfaceVisible = () => {
    const chatInterface = document.querySelector('[data-testid="chat-interface"]');
    return !!(chatInterface && 
      chatInterface.offsetWidth > 0 && 
      chatInterface.offsetHeight > 0);
  };
  
  if (!isVisible) return null;
  
  return (
    <div
      className="fixed bottom-4 right-4 z-50 bg-slate-900 text-white rounded-lg shadow-lg overflow-hidden"
      style={{
        maxWidth: isExpanded ? '500px' : '200px',
        maxHeight: isExpanded ? '80vh' : '40px',
        transition: 'all 0.3s ease',
        opacity: 0.9,
        fontSize: '12px'
      }}
    >
      {/* Header */}
      <div 
        className="p-2 bg-blue-800 flex justify-between items-center cursor-pointer"
        onClick={() => setIsExpanded(!isExpanded)}
      >
        <div className="font-bold">
          Debug Panel {isExpanded ? '(click to collapse)' : '(click to expand)'}
        </div>
        <div className="text-xs opacity-70">
          {new Date(lastUpdated).toLocaleTimeString()}
        </div>
      </div>
      
      {/* Content */}
      {isExpanded && (
        <div className="p-3 overflow-auto" style={{ maxHeight: 'calc(80vh - 40px)' }}>
          {/* Session ID Section */}
          <div className="mb-4">
            <h3 className="text-yellow-400 font-bold mb-1 border-b border-yellow-700 pb-1">Session ID</h3>
            <div className="grid grid-cols-2 gap-1">
              <div className="text-gray-400">Auth Context:</div>
              <div className={auth.sessionId ? 'text-green-400' : 'text-red-400'}>
                {auth.sessionId ? `${auth.sessionId.substring(0, 8)}...` : 'missing'}
              </div>
              
              <div className="text-gray-400">Storage:</div>
              <div className={storedSessionId ? 'text-green-400' : 'text-red-400'}>
                {storedSessionId ? `${storedSessionId.substring(0, 8)}...` : 'missing'}
              </div>
              
              <div className="text-gray-400">Match:</div>
              <div className={(auth.sessionId === storedSessionId) ? 'text-green-400' : 'text-red-400'}>
                {(auth.sessionId === storedSessionId) ? 'Yes' : 'No'}
              </div>
            </div>
          </div>
          
          {/* Context Status Section */}
          <div className="mb-4">
            <h3 className="text-yellow-400 font-bold mb-1 border-b border-yellow-700 pb-1">Context Status</h3>
            <div className="grid grid-cols-3 gap-1">
              <div className="text-gray-400">Auth:</div>
              <div 
                className={`col-span-2 ${auth.isAuthenticated ? 'text-green-400' : 'text-red-400'}`}
              >
                {auth.isInitializing ? 'Initializing...' : 
                  (auth.isAuthenticated ? 'Authenticated' : 'Not Authenticated')}
              </div>
              
              <div className="text-gray-400">Session:</div>
              <div 
                className={`col-span-2 ${session.isInitialized ? 'text-green-400' : 
                  (session.isLoading ? 'text-yellow-400' : 'text-red-400')}`}
              >
                {session.isLoading ? 'Loading...' : 
                  (session.isInitialized ? 'Initialized' : 'Not Initialized')}
              </div>
              
              <div className="text-gray-400">Ready:</div>
              <div 
                className={`col-span-2 ${session.isReady ? 'text-green-400' : 'text-red-400'}`}
              >
                {session.isReady ? 'Ready' : 'Not Ready'}
              </div>
              
              <div className="text-gray-400">Model:</div>
              <div 
                className={`col-span-2 ${model.modelName ? 'text-green-400' : 
                  (model.isLoading ? 'text-yellow-400' : 'text-red-400')}`}
              >
                {model.isLoading ? 'Loading...' : 
                  (model.modelName ? model.modelName : 'No Model Selected')}
              </div>
              
              <div className="text-gray-400">Theme:</div>
              <div className="col-span-2 text-blue-400">
                {theme || 'system'}
              </div>
            </div>
          </div>
          
          {/* DOM Status Section */}
          <div className="mb-4">
            <h3 className="text-yellow-400 font-bold mb-1 border-b border-yellow-700 pb-1">DOM Status</h3>
            <div className="grid grid-cols-2 gap-1">
              <div className="text-gray-400">Chat Interface:</div>
              <div className={document.querySelector('[data-testid="chat-interface"]') ? 'text-green-400' : 'text-red-400'}>
                {document.querySelector('[data-testid="chat-interface"]') ? 'In DOM' : 'Not in DOM'}
              </div>
              
              <div className="text-gray-400">Visible:</div>
              <div className={isChatInterfaceVisible() ? 'text-green-400' : 'text-red-400'}>
                {isChatInterfaceVisible() ? 'Yes' : 'No'}
              </div>
              
              <div className="text-gray-400">Chat Page:</div>
              <div className={document.querySelector('[data-chat-page]') ? 'text-green-400' : 'text-red-400'}>
                {document.querySelector('[data-chat-page]') ? 'In DOM' : 'Not in DOM'}
              </div>
            </div>
          </div>
          
          {/* Actions Section */}
          <div className="mb-4">
            <h3 className="text-yellow-400 font-bold mb-1 border-b border-yellow-700 pb-1">Actions</h3>
            <div className="flex flex-wrap gap-2 mt-2">
              <button 
                onClick={() => window.location.reload()}
                className="bg-blue-700 hover:bg-blue-600 text-white px-2 py-1 rounded text-xs"
              >
                Reload Page
              </button>
              
              <button 
                onClick={() => localStorage.clear() || window.location.reload()}
                className="bg-red-700 hover:bg-red-600 text-white px-2 py-1 rounded text-xs"
              >
                Clear Storage & Reload
              </button>
              
              <button 
                onClick={() => {
                  if (typeof window.inspectRenderingPath === 'function') {
                    window.inspectRenderingPath();
                  }
                }}
                className="bg-purple-700 hover:bg-purple-600 text-white px-2 py-1 rounded text-xs"
              >
                Log Rendering Path
              </button>
              
              <button 
                onClick={() => {
                  if (typeof window.checkChatVisibility === 'function') {
                    window.checkChatVisibility();
                  }
                }}
                className="bg-green-700 hover:bg-green-600 text-white px-2 py-1 rounded text-xs"
              >
                Check Chat Visibility
              </button>
              
              <button 
                onClick={() => {
                  console.log('Current Context Status:', contextStatus);
                }}
                className="bg-yellow-700 hover:bg-yellow-600 text-white px-2 py-1 rounded text-xs"
              >
                Log Context Status
              </button>
            </div>
          </div>
          
          {/* Context Initialize Details */}
          <div className="mb-4">
            <h3 className="text-yellow-400 font-bold mb-1 border-b border-yellow-700 pb-1">Context Status</h3>
            <div className="text-xs font-mono bg-gray-800 p-2 rounded overflow-auto max-h-[200px]">
              {/* Render as a table instead of raw JSON to reduce memory */}
              <table className="w-full text-left">
                <thead>
                  <tr>
                    <th className="p-1">Context</th>
                    <th className="p-1">Status</th>
                    <th className="p-1">Complete</th>
                  </tr>
                </thead>
                <tbody>
                  {Object.entries(contextStatus).map(([key, value]) => (
                    <tr key={key}>
                      <td className="p-1">{key}</td>
                      <td className={`p-1 ${value.status === 'complete' ? 'text-green-400' : value.status === 'error' ? 'text-red-400' : 'text-yellow-400'}`}>
                        {value.status}
                      </td>
                      <td className="p-1">
                        {value.complete ? '✓' : '✗'}
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          </div>
          
          {/* Local Storage */}
          <div className="mb-4">
            <h3 className="text-yellow-400 font-bold mb-1 border-b border-yellow-700 pb-1">Local Storage</h3>
            <div className="text-xs font-mono bg-gray-800 p-2 rounded overflow-auto max-h-[200px]">
              <table className="w-full text-left">
                <tbody>
                  <tr>
                    <td className="p-1 text-gray-400">Session ID:</td>
                    <td className="p-1">
                      {localStorage.getItem('ui_session_id') || 'not set'}
                    </td>
                  </tr>
                  <tr>
                    <td className="p-1 text-gray-400">Theme:</td>
                    <td className="p-1">
                      {localStorage.getItem('theme') || 'system'}
                    </td>
                  </tr>
                  <tr>
                    <td className="p-1 text-gray-400">Agent Config:</td>
                    <td className="p-1">
                      {localStorage.getItem('agent_config') ? 'set' : 'not set'}
                    </td>
                  </tr>
                  <tr>
                    <td className="p-1 text-gray-400">Debug Mode:</td>
                    <td className="p-1">
                      {localStorage.getItem('debug_mode') === 'true' ? 'enabled' : 'disabled'}
                    </td>
                  </tr>
                </tbody>
              </table>
            </div>
          </div>
        </div>
      )}
    </div>
  );
};

export default EnhancedDebugPanel;