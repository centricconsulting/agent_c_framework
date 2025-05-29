import React, { useEffect, useState } from 'react';
import { useAuth } from '@/contexts/AuthContext';
import { getTokenManager, refreshAuthToken } from '@/lib/auth-helper';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardDescription, CardFooter, CardHeader, CardTitle } from '@/components/ui/card';

/**
 * Component to demonstrate and test token refresh functionality
 * This is primarily for development/testing and would typically not be included in production
 */
export function TokenRefreshDemo() {
  const { isAuthenticated, tokenRefreshError, forceRefreshToken } = useAuth();
  const [tokenInfo, setTokenInfo] = useState({ token: null, expiration: null });
  const [refreshCount, setRefreshCount] = useState(0);
  const [lastRefresh, setLastRefresh] = useState(null);
  const [backgroundRefreshEnabled, setBackgroundRefreshEnabled] = useState(true);
  
  // Function to decode token information
  const updateTokenInfo = async () => {
    try {
      const tokenManager = getTokenManager();
      if (!tokenManager) {
        setTokenInfo({ token: 'Token manager not available', expiration: null });
        return;
      }
      
      const token = await tokenManager.getToken();
      if (!token) {
        setTokenInfo({ token: 'No token available', expiration: null });
        return;
      }
      
      // Get just first 10 chars of token for display
      const tokenPreview = `${token.substring(0, 10)}...`;
      
      // Decode expiration from token
      try {
        const parts = token.split('.');
        if (parts.length === 3) {
          const payload = JSON.parse(atob(parts[1]));
          const expiration = payload.exp ? new Date(payload.exp * 1000) : null;
          
          setTokenInfo({ 
            token: tokenPreview, 
            expiration: expiration,
            timeRemaining: expiration ? Math.floor((expiration - new Date()) / 1000) : null,
            iat: payload.iat ? new Date(payload.iat * 1000) : null,
            sub: payload.sub || null,
            aud: payload.aud || null,
          });
        } else {
          setTokenInfo({ token: tokenPreview, expiration: 'Invalid token format' });
        }
      } catch (error) {
        console.error('Error decoding token:', error);
        setTokenInfo({ token: tokenPreview, expiration: 'Error decoding' });
      }
    } catch (error) {
      console.error('Error getting token info:', error);
      setTokenInfo({ token: 'Error', expiration: null });
    }
  };
  
  // Handle manual token refresh
  const handleRefreshToken = async () => {
    try {
      setLastRefresh(new Date());
      setRefreshCount(prev => prev + 1);
      await forceRefreshToken();
      await updateTokenInfo();
    } catch (error) {
      console.error('Manual refresh failed:', error);
    }
  };
  
  // Toggle background refresh
  const toggleBackgroundRefresh = () => {
    const tokenManager = getTokenManager();
    if (tokenManager) {
      if (backgroundRefreshEnabled) {
        // Disable by clearing any refresh timer
        if (tokenManager.refreshTimer) {
          clearTimeout(tokenManager.refreshTimer);
          tokenManager.refreshTimer = null;
        }
      } else {
        // Re-enable by scheduling refresh
        tokenManager.scheduleRefresh();
      }
      setBackgroundRefreshEnabled(!backgroundRefreshEnabled);
    }
  };
  
  // Update token info periodically
  useEffect(() => {
    if (!isAuthenticated) return;
    
    // Initial update
    updateTokenInfo();
    
    // Set up periodic update
    const interval = setInterval(() => {
      updateTokenInfo();
    }, 1000); // Update every second to show countdown
    
    return () => clearInterval(interval);
  }, [isAuthenticated]);
  
  // Set up token manager event listeners
  useEffect(() => {
    if (!isAuthenticated) return;
    
    const tokenManager = getTokenManager();
    if (!tokenManager) return;
    
    const handleRefresh = () => {
      setRefreshCount(prev => prev + 1);
      setLastRefresh(new Date());
      updateTokenInfo();
    };
    
    tokenManager.on('refreshed', handleRefresh);
    
    return () => {
      tokenManager.off('refreshed', handleRefresh);
    };
  }, [isAuthenticated]);
  
  if (!isAuthenticated) {
    return null; // Don't show when not authenticated
  }
  
  return (
    <Card className="w-full max-w-lg">
      <CardHeader>
        <CardTitle>Token Refresh Demo</CardTitle>
        <CardDescription>Monitor and test token refresh functionality</CardDescription>
      </CardHeader>
      <CardContent className="space-y-4">
        <div>
          <p className="text-sm font-medium">Token Preview:</p>
          <p className="text-sm font-mono">{tokenInfo.token || 'N/A'}</p>
        </div>
        
        {tokenInfo.expiration && (
          <div>
            <p className="text-sm font-medium">Expires At:</p>
            <p className="text-sm">{tokenInfo.expiration.toLocaleString()}</p>
          </div>
        )}
        
        {tokenInfo.timeRemaining !== null && (
          <div>
            <p className="text-sm font-medium">Time Remaining:</p>
            <p className={`text-sm ${tokenInfo.timeRemaining < 300 ? 'text-red-500 font-bold' : ''}`}>
              {Math.floor(tokenInfo.timeRemaining / 60)}m {tokenInfo.timeRemaining % 60}s
            </p>
          </div>
        )}
        
        {tokenInfo.iat && (
          <div>
            <p className="text-sm font-medium">Issued At:</p>
            <p className="text-sm">{tokenInfo.iat.toLocaleString()}</p>
          </div>
        )}
        
        <div>
          <p className="text-sm font-medium">Refresh Count:</p>
          <p className="text-sm">{refreshCount}</p>
        </div>
        
        {lastRefresh && (
          <div>
            <p className="text-sm font-medium">Last Refresh:</p>
            <p className="text-sm">{lastRefresh.toLocaleString()}</p>
          </div>
        )}
        
        {tokenRefreshError && (
          <div className="bg-red-100 border border-red-400 text-red-700 px-4 py-3 rounded">
            <p className="text-sm font-bold">Refresh Error:</p>
            <p className="text-sm">{tokenRefreshError}</p>
          </div>
        )}
      </CardContent>
      <CardFooter className="flex justify-between">
        <Button onClick={handleRefreshToken} variant="outline">
          Refresh Token Now
        </Button>
        <Button 
          onClick={toggleBackgroundRefresh} 
          variant={backgroundRefreshEnabled ? "destructive" : "default"}
        >
          {backgroundRefreshEnabled ? 'Disable Auto-Refresh' : 'Enable Auto-Refresh'}
        </Button>
      </CardFooter>
    </Card>
  );
}

export default TokenRefreshDemo;