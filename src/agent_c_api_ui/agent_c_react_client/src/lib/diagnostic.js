/**
 * Diagnostic utilities for debugging UI issues
 * Provides tools for diagnosing and resolving common problems
 */

import logger from './logger';
import storageService from './storageService';
import { API_URL } from '@/config/config';

/**
 * Run diagnostic tests and report results to console
 * @returns {string} Summary message
 */
export function runDiagnostics() {
    logger.info('Starting diagnostic tests', 'diagnostics');
    const diagnosticId = logger.group('Diagnostic Tests', 'diagnostics');
    
    const results = {};
    
    // Check localStorage state
    logger.info('Checking localStorage state', 'diagnostics');
    try {
        const sessionId = storageService.getSessionId();
        const agentConfig = storageService.getAgentConfig();
        const theme = storageService.getTheme();

        results.localStorage = {
            sessionId,
            hasAgentConfig: !!agentConfig,
            configKeys: agentConfig ? Object.keys(agentConfig) : [],
            theme
        };
        
        logger.debug('LocalStorage state', 'diagnostics', results.localStorage);
    } catch (e) {
        logger.error('Error checking localStorage state', 'diagnostics', { error: e.message });
        results.localStorage = { error: e.message };
    }

    // Check theme state
    logger.info('Checking theme state', 'diagnostics');
    try {
        const html = document.documentElement;
        results.theme = {
            className: html.className,
            hasDarkClass: html.classList.contains('dark'),
            hasLightClass: html.classList.contains('light'),
            storedTheme: storageService.getTheme(),
            systemDarkMode: window.matchMedia('(prefers-color-scheme: dark)').matches
        };
        
        logger.debug('Theme state', 'diagnostics', results.theme);
    } catch (e) {
        logger.error('Error checking theme state', 'diagnostics', { error: e.message });
        results.theme = { error: e.message };
    }

    // Check component tree
    logger.info('Checking component tree', 'diagnostics');
    try {
        const rootElement = document.getElementById('root');
        const layoutElement = document.querySelector('.app-layout');
        const chatInterface = document.querySelector('.chat-interface-card');
        
        results.components = {
            rootMounted: !!rootElement,
            layoutPresent: !!layoutElement,
            chatInterfacePresent: !!chatInterface
        };
        
        logger.debug('Component tree state', 'diagnostics', results.components);
    } catch (e) {
        logger.error('Error checking component tree', 'diagnostics', { error: e.message });
        results.components = { error: e.message };
    }

    // Check network state
    logger.info('Checking network state', 'diagnostics');
    try {
        results.network = {
            isOnline: navigator.onLine
        };
        
        // Check if the API is reachable (asynchronous)
        const apiUrl = API_URL || 'http://localhost:8000/api/v1';
        logger.debug(`Checking API availability at ${apiUrl}`, 'diagnostics');
        
        fetch(`${apiUrl}/health`)
            .then(response => {
                results.network.apiReachable = response.ok;
                logger.debug('API endpoint status', 'diagnostics', { 
                    reachable: response.ok, 
                    status: response.status 
                });
            })
            .catch(err => {
                results.network.apiReachable = false;
                results.network.apiError = err.message;
                logger.error('API endpoint unreachable', 'diagnostics', { error: err.message });
            });
    } catch (e) {
        logger.error('Error checking network state', 'diagnostics', { error: e.message });
        results.network = { error: e.message };
    }
    
    // Check performance metrics
    logger.info('Collecting performance metrics', 'diagnostics');
    try {
        const metrics = logger.getPerformanceMetrics();
        results.performance = {
            metrics,
            // Extract summary for the most important operations
            apiCallStats: Object.entries(metrics.apiService || {}).map(([op, stats]) => ({
                operation: op,
                count: stats.count,
                avgDuration: Math.round(stats.total / stats.count)
            }))
        };
        
        logger.debug('Performance metrics', 'diagnostics', { 
            metricGroups: Object.keys(metrics),
            apiCallCount: results.performance.apiCallStats.length
        });
    } catch (e) {
        logger.error('Error collecting performance metrics', 'diagnostics', { error: e.message });
        results.performance = { error: e.message };
    }
    
    // Log summary
    logger.info('Diagnostic tests completed', 'diagnostics', {
        sessionId: results.localStorage?.sessionId,
        theme: results.theme?.className,
        isOnline: results.network?.isOnline
    });
    logger.groupEnd(diagnosticId, 'Diagnostic Tests', 'diagnostics');
    
    // Also output to console in a more readable format
    console.group('🔍 DIAGNOSTIC REPORT');
    console.log('📦 LocalStorage:', results.localStorage);
    console.log('🎨 Theme:', results.theme);
    console.log('🧩 Components:', results.components);
    console.log('🌐 Network:', results.network);
    console.log('⚡ Performance:', {
        apiCalls: results.performance?.apiCallStats
    });
    console.groupEnd();

    return 'Diagnostics complete. See console for details.';
}

/**
 * Clear session state and reload the page
 * @returns {string} Status message
 */
export function clearSessionState() {
    logger.info('Clearing session state', 'diagnostics');
    
    try {
        // Record what we're clearing
        const sessionId = storageService.getSessionId();
        logger.debug('Removing session', 'diagnostics', { sessionId });
        
        // Clear specific items
        storageService.removeSessionId();
        storageService.storage.remove(storageService.STORAGE_KEYS.AGENT_CONFIG);
        
        logger.info('Session state cleared successfully', 'diagnostics');
        logger.debug('Reloading application', 'diagnostics');
        
        // Console log for user visibility
        console.log('🧹 Session state cleared');
        console.log('🔄 Reloading app...');
        
        // Small delay before reload to ensure UI updates
        setTimeout(() => {
            window.location.reload();
        }, 500);
        
        return 'Session state cleared. Reloading...'
    } catch (e) {
        logger.error('Failed to clear session state', 'diagnostics', { error: e.message });
        console.error('❌ Error clearing session state:', e);
        return `Error clearing session state: ${e.message}`
    }
}

/**
 * Generate diagnostic information for sharing/troubleshooting
 * @returns {Object} Sanitized diagnostic information
 */
export function getDiagnosticInfo() {
    logger.info('Generating diagnostic information', 'diagnostics');
    
    const info = {
        timestamp: new Date().toISOString(),
        environment: {
            userAgent: navigator.userAgent,
            language: navigator.language,
            windowDimensions: `${window.innerWidth}x${window.innerHeight}`,
            devicePixelRatio: window.devicePixelRatio,
            timeZone: Intl.DateTimeFormat().resolvedOptions().timeZone
        },
        application: {
            apiUrl: API_URL,
            theme: storageService.getTheme(),
            logLevel: logger.getLevelName(),
            // Sanitized session - only include existence, not actual ID
            hasActiveSession: !!storageService.getSessionId(),
            // Check if required DOM elements exist
            rootElementExists: !!document.getElementById('root'),
            layoutExists: !!document.querySelector('.app-layout')
        }
    };
    
    logger.debug('Diagnostic information generated', 'diagnostics');
    return info;
}