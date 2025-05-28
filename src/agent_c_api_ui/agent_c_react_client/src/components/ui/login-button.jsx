import React from 'react';
import { Button } from '@/components/ui/button';
import { useAuth } from '@/contexts/AuthContext';
import { LockIcon, UserIcon } from 'lucide-react';

/**
 * Login button component that displays differently based on authentication state
 */
export function LoginButton({ variant = 'default', size = 'default' }) {
  const { isAuthenticated, login, loading } = useAuth();

  if (isAuthenticated) {
    return null; // Don't show login button when authenticated
  }

  return (
    <Button
      variant={variant}
      size={size}
      onClick={login}
      disabled={loading}
      className="flex items-center gap-2"
    >
      <LockIcon className="h-4 w-4" />
      {loading ? 'Loading...' : 'Sign In'}
    </Button>
  );
}

/**
 * UserProfile component for displaying user information and logout option
 */
export function UserProfile({ variant = 'ghost', size = 'sm' }) {
  const { user, logout, isAuthenticated } = useAuth();

  if (!isAuthenticated || !user) {
    return null; // Don't show when not authenticated
  }

  return (
    <div className="flex items-center gap-2">
      <Button
        variant={variant}
        size={size}
        onClick={logout}
        className="flex items-center gap-2"
      >
        <UserIcon className="h-4 w-4" />
        <span className="hidden md:inline">{user.name || 'User'}</span>
      </Button>
    </div>
  );
}