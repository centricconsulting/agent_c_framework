# SessionContext Refactor Phase 2 Debugging Guide

This document provides debugging strategies for Phase 2 of the SessionContext refactor.

## Common Issues

### Session Initialization Failures

**Symptoms:**
- Application stuck in loading state
- Error messages related to session initialization
- Components not rendering correctly

**Debugging Steps:**
1. Check browser console for errors
2. Verify the order of initialization in the React component tree
3. Check Network tab for API responses to `/initialize` endpoint
4. Add console logs to track the initialization flow:

```javascript
// Add to SessionContext.jsx
const initializeSession = async (config) => {
  console.log('Core SessionContext: initializing session with config:', config);
  setIsReady(false);
  try {
    const data = await sessionService.initialize(config);
    console.log('Core SessionContext: session initialized successfully:', data);
    // ...
  } catch (err) {
    console.error('Core SessionContext: session initialization failed:', err);
    // ...
  }
};
```

### Context Dependency Issues

**Symptoms:**
- "Cannot read property X of undefined" errors
- Components using legacy context failing

**Debugging Steps:**
1. Verify provider order in App.jsx
2. Check that LegacySessionContext correctly forwards core session state
3. Add context existence checks:

```javascript
const contextValue = useContext(LegacySessionContext);
console.log('Context value available:', Boolean(contextValue));
```

### localStorage Issues

**Symptoms:**
- Sessions not persisting between refreshes
- Multiple sessions being created

**Debugging Steps:**
1. Check Application tab > Storage > localStorage in dev tools
2. Verify that ui_session_id is being stored correctly
3. Add storage event listeners to debug writes:

```javascript
useEffect(() => {
  const handleStorage = (e) => {
    if (e.key === 'ui_session_id') {
      console.log('ui_session_id changed:', e.oldValue, '->', e.newValue);
    }
  };
  window.addEventListener('storage', handleStorage);
  return () => window.removeEventListener('storage', handleStorage);
}, []);
```

## Debugging Tools

### Session Debug Component

Create a simple debug component to display session state:

```jsx
const SessionDebug = () => {
  const sessionContext = useContext(SessionContext);
  const legacyContext = useContext(LegacySessionContext);
  
  return (
    <div style={{ position: 'fixed', bottom: 0, right: 0, background: '#f0f0f0', padding: '10px', zIndex: 9999, fontSize: '12px' }}>
      <h4>Session Debug</h4>
      <div>Core Session ID: {sessionContext?.sessionId || 'none'}</div>
      <div>Core Ready: {String(sessionContext?.isReady)}</div>
      <div>Legacy Session ID: {legacyContext?.sessionId || 'none'}</div>
      <div>Legacy Ready: {String(legacyContext?.isReady)}</div>
      <div>Legacy Initialized: {String(legacyContext?.isInitialized)}</div>
    </div>
  );
};
```

### API Call Tracing

Add this to your development environment to trace API calls:

```javascript
// In services/api.js development mode
const traceApiCall = (method, url, data) => {
  const timestamp = new Date().toISOString().substr(11, 12);
  console.group(`🌐 API ${method.toUpperCase()} ${url} [${timestamp}]`);
  if (data) console.log('Request data:', data);
  console.groupEnd();
};

// Then in each method
async function get(url, config = {}) {
  if (process.env.NODE_ENV !== 'production') {
    traceApiCall('get', url, config);
  }
  // ... rest of method
}
```

### React DevTools Profiling

1. Install React DevTools browser extension
2. Use the Profiler tab to record renders
3. Look for unexpected renders or render cascades
4. Check the Components tab to inspect context values

## Rollback Strategy

If critical issues are encountered, follow this rollback procedure:

1. Restore the original SessionContext.jsx from .scratch/SessionContext.jsx.OLD
2. Revert App.jsx changes
3. Remove any new files created (useSession.js)

## Performance Monitoring

Watch for these performance issues during testing:

1. **Render cascades** - when context changes trigger many components to re-render
2. **Multiple session initializations** - check network tab for duplicate calls
3. **Memory leaks** - watch for components not cleaning up effects or timeouts

## Testing Checklist

- [ ] Session persists across page refreshes
- [ ] Initialization with saved configuration works
- [ ] New session creation works
- [ ] Error states are properly handled and displayed
- [ ] No console errors during normal operation
- [ ] No duplicate API calls for the same operation