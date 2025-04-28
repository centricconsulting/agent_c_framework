/**
 * EnhancedDebugPanel component
 * 
 * A lightweight development-only performance monitoring panel
 * that has minimal impact on application performance.
 * Only initialized in development mode with feature flag control.
 */

import React, { useState, useEffect, useCallback, useMemo } from 'react';

const EnhancedDebugPanel = () => {
  // Immediately return null in production mode
  if (process.env.NODE_ENV === 'production') {
    return null;
  }

  // State for performance metrics
  const [metrics, setMetrics] = useState({
    fps: 0,
    memory: null,
    componentRenderCount: {},
    visible: false
  });

  // Only collect metrics when panel is visible
  const [isVisible, setIsVisible] = useState(false);

  // Toggle visibility handler
  const toggleVisibility = useCallback(() => {
    setIsVisible(prev => !prev);
  }, []);

  // Register keyboard shortcut (Ctrl+Shift+D) to toggle panel
  useEffect(() => {
    const handleKeyDown = (e) => {
      if (e.ctrlKey && e.shiftKey && e.key === 'D') {
        toggleVisibility();
        e.preventDefault();
      }
    };

    window.addEventListener('keydown', handleKeyDown);
    return () => window.removeEventListener('keydown', handleKeyDown);
  }, [toggleVisibility]);

  // Collect performance metrics when visible
  useEffect(() => {
    if (!isVisible) return;

    let frameCount = 0;
    let lastTime = performance.now();
    let rafId;

    const measureFPS = () => {
      frameCount++;
      const now = performance.now();
      const elapsed = now - lastTime;

      if (elapsed >= 1000) {
        const fps = Math.round((frameCount * 1000) / elapsed);
        setMetrics(prev => ({
          ...prev,
          fps,
          memory: window.performance?.memory ? {
            usedJSHeapSize: Math.round(window.performance.memory.usedJSHeapSize / (1024 * 1024)),
            totalJSHeapSize: Math.round(window.performance.memory.totalJSHeapSize / (1024 * 1024))
          } : null
        }));

        frameCount = 0;
        lastTime = now;
      }

      rafId = requestAnimationFrame(measureFPS);
    };

    measureFPS();
    return () => cancelAnimationFrame(rafId);
  }, [isVisible]);

  // Don't render anything if not visible
  if (!isVisible) {
    return null;
  }

  // Render a simple panel with minimal DOM and styling impact
  return (
    <div 
      className="enhanced-debug-panel" 
      style={{
        position: 'fixed',
        bottom: '10px',
        right: '10px',
        background: 'rgba(0, 0, 0, 0.7)',
        color: 'white',
        padding: '10px',
        fontSize: '12px',
        zIndex: 9999,
        fontFamily: 'monospace',
        borderRadius: '4px',
        maxWidth: '350px',
        maxHeight: '200px',
        overflow: 'auto'
      }}
    >
      <div>
        <div style={{ marginBottom: '5px', fontWeight: 'bold' }}>
          Performance Monitor (Dev Only)
        </div>
        <div>FPS: {metrics.fps}</div>
        {metrics.memory && (
          <div>
            Memory: {metrics.memory.usedJSHeapSize}MB / {metrics.memory.totalJSHeapSize}MB
          </div>
        )}
        <button 
          onClick={toggleVisibility}
          style={{
            background: '#333',
            color: 'white',
            border: '1px solid #555',
            padding: '2px 5px',
            marginTop: '5px',
            cursor: 'pointer',
            fontSize: '10px'
          }}
        >
          Close
        </button>
      </div>
    </div>
  );
};

// Ensure the component is properly memoized
export default React.memo(EnhancedDebugPanel);
