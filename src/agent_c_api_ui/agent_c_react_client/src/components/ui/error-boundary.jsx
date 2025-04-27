import React from 'react';
import logger from '@/lib/logger';

/**
 * Error boundary component to catch errors in child components
 * Prevents entire app from crashing when a component fails
 */
class ErrorBoundary extends React.Component {
  constructor(props) {
    super(props);
    this.state = { 
      hasError: false,
      error: null,
      errorInfo: null
    };
  }

  static getDerivedStateFromError(error) {
    // Update state so the next render will show the fallback UI
    return { hasError: true, error };
  }

  componentDidCatch(error, errorInfo) {
    // Log the error to our logging service
    logger.error(
      `Error caught by ${this.props.name || 'ErrorBoundary'}`, 
      this.props.name || 'ErrorBoundary',
      { error: error.message, stack: error.stack, componentStack: errorInfo.componentStack }
    );
    
    // Also store the error info for rendering
    this.setState({ errorInfo });
    
    // If we have diagnostic tracking available, record the error
    if (window.__CONTEXT_DIAGNOSTIC) {
      try {
        window.__CONTEXT_DIAGNOSTIC.initialization.errors.push({
          boundary: this.props.name,
          time: Date.now(),
          error: error.message,
          stack: error.stack,
          componentStack: errorInfo.componentStack
        });
      } catch (e) {
        // Ignore errors in diagnostic reporting
      }
    }
    
    // Call onError callback if provided
    if (typeof this.props.onError === 'function') {
      this.props.onError(error, errorInfo);
    }
  }

  render() {
    if (this.state.hasError) {
      // Render fallback UI
      if (this.props.fallback) {
        return this.props.fallback(this.state.error, this.state.errorInfo);
      }
      
      // Default fallback UI
      return (
        <div className="error-boundary-fallback" data-error-boundary={this.props.name || 'unknown'}>
          <h2>Something went wrong in {this.props.name || 'a component'}.</h2>
          <details style={{ whiteSpace: 'pre-wrap', marginTop: '10px', padding: '10px', backgroundColor: 'rgba(255,0,0,0.05)' }}>
            <summary>Show error details</summary>
            <p>{this.state.error && this.state.error.toString()}</p>
            <p style={{ color: '#777' }}>
              Component Stack: {this.state.errorInfo && this.state.errorInfo.componentStack}
            </p>
          </details>
          {this.props.children && (
            <div style={{ marginTop: '15px' }}>
              <button 
                onClick={() => this.setState({ hasError: false, error: null, errorInfo: null })}
                style={{
                  padding: '8px 16px',
                  backgroundColor: '#4A5568',
                  color: 'white',
                  border: 'none',
                  borderRadius: '4px',
                  cursor: 'pointer'
                }}
              >
                Try again
              </button>
            </div>
          )}
        </div>
      );
    }

    return this.props.children;
  }
}

export { ErrorBoundary };