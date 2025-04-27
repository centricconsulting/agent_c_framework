/**
 * Debug Mode Toggle Utility
 * 
 * This script provides an easy way to toggle debug mode in the browser console.
 * It helps developers troubleshoot issues without having to modify code.
 */

// Create the debug utilities when the script loads
if (typeof window !== 'undefined') {
  // Enable debug mode function
  window.enableDebugMode = () => {
    localStorage.setItem('debug_mode', 'true');
    console.log('✅ Debug mode ENABLED. Reload the page for changes to take effect.');
    console.log('Use window.disableDebugMode() to turn debugging off.');
  };
  
  // Disable debug mode function
  window.disableDebugMode = () => {
    localStorage.setItem('debug_mode', 'false');
    console.log('🚫 Debug mode DISABLED. Reload the page for changes to take effect.');
    console.log('Use window.enableDebugMode() to turn debugging back on.');
  };
  
  // Check debug mode status
  window.checkDebugMode = () => {
    const enabled = localStorage.getItem('debug_mode') === 'true';
    console.log(`Debug mode is currently ${enabled ? 'ENABLED' : 'DISABLED'}`);
    return enabled;
  };
  
  // Toggle debug panel
  window.toggleDebugPanel = () => {
    const currentValue = localStorage.getItem('enableDebugPanel') === 'true';
    const newValue = !currentValue;
    localStorage.setItem('enableDebugPanel', newValue.toString());
    console.log(`Debug panel is now ${newValue ? 'ENABLED' : 'DISABLED'}`);
    
    // Trigger storage event so panel can react
    const event = new StorageEvent('storage', {
      key: 'enableDebugPanel',
      newValue: newValue.toString(),
      storageArea: localStorage
    });
    window.dispatchEvent(event);
  };
}

// Export the current debug state for use in the app
export const isDebugMode = typeof window !== 'undefined' && 
  localStorage.getItem('debug_mode') === 'true';

export default {
  isDebugMode
};