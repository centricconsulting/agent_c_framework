/**
 * User Info Display Component
 * Demonstrates accessing full user data from WebSocket
 */

'use client';

import React from 'react';
import { useAuth } from '@/contexts/auth-context';
import { useUserData } from '@agentc/realtime-react';
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
  Badge,
  Skeleton
} from '@agentc/realtime-ui';
import { User, Mail, Calendar, Shield, Users } from 'lucide-react';

export function UserInfoDisplay() {
  const { isAuthenticated, getUiSessionId } = useAuth();
  const { user, isLoading } = useUserData();
  
  const uiSessionId = getUiSessionId();

  if (isLoading) {
    return (
      <Card className="w-full max-w-2xl">
        <CardHeader>
          <Skeleton className="h-6 w-32" />
          <Skeleton className="h-4 w-48 mt-2" />
        </CardHeader>
        <CardContent className="space-y-4">
          <Skeleton className="h-4 w-full" />
          <Skeleton className="h-4 w-3/4" />
          <Skeleton className="h-4 w-1/2" />
        </CardContent>
      </Card>
    );
  }

  if (!isAuthenticated) {
    return (
      <Card className="w-full max-w-2xl">
        <CardHeader>
          <CardTitle>Not Authenticated</CardTitle>
          <CardDescription>Please log in to view user information</CardDescription>
        </CardHeader>
      </Card>
    );
  }

  if (!user) {
    return (
      <Card className="w-full max-w-2xl">
        <CardHeader>
          <CardTitle>Loading User Data</CardTitle>
          <CardDescription>
            Waiting for user data from WebSocket connection...
          </CardDescription>
        </CardHeader>
      </Card>
    );
  }

  const formatDate = (dateString: string | undefined | null) => {
    if (!dateString) return 'N/A';
    try {
      return new Date(dateString).toLocaleString();
    } catch {
      return dateString;
    }
  };

  return (
    <Card className="w-full max-w-2xl">
      <CardHeader>
        <CardTitle className="flex items-center gap-2">
          <User className="h-5 w-5" />
          User Information
        </CardTitle>
        <CardDescription>
          Complete user profile data from WebSocket connection
        </CardDescription>
      </CardHeader>
      <CardContent className="space-y-6">
        {/* Basic Information */}
        <div className="space-y-3">
          <h3 className="text-sm font-semibold text-muted-foreground">Basic Information</h3>
          <div className="grid gap-2">
            <div className="flex items-center gap-2">
              <span className="text-sm font-medium w-32">User ID:</span>
              <span className="text-sm text-muted-foreground">{user.user_id}</span>
            </div>
            <div className="flex items-center gap-2">
              <span className="text-sm font-medium w-32">Username:</span>
              <span className="text-sm text-muted-foreground">{user.user_name || 'N/A'}</span>
            </div>
            <div className="flex items-center gap-2">
              <span className="text-sm font-medium w-32">First Name:</span>
              <span className="text-sm text-muted-foreground">{user.first_name || 'N/A'}</span>
            </div>
            <div className="flex items-center gap-2">
              <span className="text-sm font-medium w-32">Last Name:</span>
              <span className="text-sm text-muted-foreground">{user.last_name || 'N/A'}</span>
            </div>
            <div className="flex items-center gap-2">
              <Mail className="h-4 w-4" />
              <span className="text-sm font-medium">Email:</span>
              <span className="text-sm text-muted-foreground">{user.email || 'N/A'}</span>
            </div>
          </div>
        </div>

        {/* Status Information */}
        <div className="space-y-3">
          <h3 className="text-sm font-semibold text-muted-foreground">Status</h3>
          <div className="grid gap-2">
            <div className="flex items-center gap-2">
              <span className="text-sm font-medium w-32">Active:</span>
              <Badge variant={user.is_active ? 'default' : 'secondary'}>
                {user.is_active ? 'Active' : 'Inactive'}
              </Badge>
            </div>
            <div className="flex items-center gap-2">
              <Calendar className="h-4 w-4" />
              <span className="text-sm font-medium">Created:</span>
              <span className="text-sm text-muted-foreground">
                {formatDate(user.created_at)}
              </span>
            </div>
            <div className="flex items-center gap-2">
              <Calendar className="h-4 w-4" />
              <span className="text-sm font-medium">Last Login:</span>
              <span className="text-sm text-muted-foreground">
                {formatDate(user.last_login)}
              </span>
            </div>
          </div>
        </div>

        {/* Roles and Groups */}
        <div className="space-y-3">
          <h3 className="text-sm font-semibold text-muted-foreground flex items-center gap-2">
            <Shield className="h-4 w-4" />
            Roles & Permissions
          </h3>
          <div className="space-y-2">
            <div>
              <span className="text-sm font-medium">Roles:</span>
              <div className="flex flex-wrap gap-2 mt-1">
                {user.roles && user.roles.length > 0 ? (
                  user.roles.map((role: string) => (
                    <Badge key={role} variant="secondary">
                      {role}
                    </Badge>
                  ))
                ) : (
                  <span className="text-sm text-muted-foreground">No roles assigned</span>
                )}
              </div>
            </div>
            <div>
              <span className="text-sm font-medium flex items-center gap-2">
                <Users className="h-4 w-4" />
                Groups:
              </span>
              <div className="flex flex-wrap gap-2 mt-1">
                {user.groups && user.groups.length > 0 ? (
                  user.groups.map((group: string) => (
                    <Badge key={group} variant="outline">
                      {group}
                    </Badge>
                  ))
                ) : (
                  <span className="text-sm text-muted-foreground">No groups assigned</span>
                )}
              </div>
            </div>
          </div>
        </div>

        {/* Session Information */}
        {uiSessionId && (
          <div className="space-y-3">
            <h3 className="text-sm font-semibold text-muted-foreground">Session</h3>
            <div className="flex items-center gap-2">
              <span className="text-sm font-medium w-32">UI Session ID:</span>
              <code className="text-xs bg-muted px-2 py-1 rounded">
                {uiSessionId}
              </code>
            </div>
          </div>
        )}

        {/* Debug Info */}
        <div className="space-y-3">
          <details className="cursor-pointer">
            <summary className="text-sm font-semibold text-muted-foreground">
              Debug: Raw User Data
            </summary>
            <pre className="mt-2 text-xs bg-muted p-3 rounded overflow-auto max-h-60">
              {JSON.stringify(user, null, 2)}
            </pre>
          </details>
        </div>

        {/* Note about data source */}
        <div className="text-xs text-muted-foreground p-3 bg-muted/50 rounded">
          <strong>Note:</strong> This user data comes from the WebSocket connection&apos;s 
          <code className="mx-1 px-1 bg-background rounded">chat_user_data</code> event, 
          not from the login response. The authentication context only manages tokens and 
          authentication state.
        </div>
      </CardContent>
    </Card>
  );
}