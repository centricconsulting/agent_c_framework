/**
 * DebugPanel component
 * 
 * A lightweight debug panel that only renders in development mode
 * and has minimal performance impact.
 */

import React from 'react';

const DebugPanel = () => {
  // Only render in development mode
  if (process.env.NODE_ENV === 'production') {
    return null;
  }
  
  // Simple implementation that won't cause performance issues
  return (
    <div className="debug-panel" style={{ display: 'none' }}>
      {/* Content is hidden by default and only enabled via dev tools */}
      <div className="debug-panel-content">
        <h3>Developer Debug Panel</h3>
        <p>This panel is only available in development mode</p>
      </div>
    </div>
  );
};

// Ensure the component is properly memoized
export default React.memo(DebugPanel);
