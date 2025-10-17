# useUserData

## Purpose and Overview

The `useUserData` hook provides simplified access to the current user's profile information from the Agent C Realtime API. This hook specifically focuses on user data received from the `chat_user_data` WebSocket event after authentication, making it ideal for components that only need user information without the full configuration data.

This hook is a lightweight alternative to `useAgentCData` when you only need user profile information. It automatically updates when user data changes and provides loading and error states to handle the asynchronous nature of WebSocket communication.

## Import Statement

```typescript
import { useUserData } from '@agentc/realtime-react';
// or with specific types
import { useUserData, type UseUserDataReturn, type User } from '@agentc/realtime-react';
```

## Complete TypeScript Types

### Core Interfaces

```typescript
/**
 * Return type for the useUserData hook
 */
export interface UseUserDataReturn {
  /** Current user data from WebSocket */
  user: User | null;
  
  /** Whether user data is loading */
  isLoading: boolean;
  
  /** Error if failed to get user data */
  error: string | null;
  
  /** Refresh user data (typically happens automatically) */
  refresh: () => void;
}

/**
 * User information returned from authentication
 */
export interface User {
  user_id: string;
  user_name: string;
  email: string | null;
  first_name: string | null;
  last_name: string | null;
  is_active: boolean;
  roles: string[];
  groups: string[];
  created_at: string | null;
  last_login: string | null;
}
```

## Detailed Descriptions of Data Structures

### User Object Fields

- **`user_id`** (string): Unique identifier for the user in the system
- **`user_name`** (string): Login username or display name for the user
- **`email`** (string | null): User's email address if provided, null otherwise
- **`first_name`** (string | null): User's first name if provided
- **`last_name`** (string | null): User's last name if provided
- **`is_active`** (boolean): Whether the user account is currently active
- **`roles`** (string[]): Array of role identifiers assigned to the user (e.g., 'admin', 'user')
- **`groups`** (string[]): Array of group identifiers the user belongs to
- **`created_at`** (string | null): ISO timestamp of account creation
- **`last_login`** (string | null): ISO timestamp of the user's most recent login

### Return Value Fields

- **`user`** (User | null): The user profile object or null if not yet loaded
- **`isLoading`** (boolean): True while waiting for user data after connection
- **`error`** (string | null): Error message if user data failed to load
- **`refresh`** (function): Manual refresh function to re-fetch user data from client

## Usage Examples

### Basic User Display

```tsx
function UserProfile() {
  const { user, isLoading, error } = useUserData();
  
  if (error) {
    return <div className="error">Failed to load user: {error}</div>;
  }
  
  if (isLoading) {
    return <div className="loading">Loading user...</div>;
  }
  
  if (!user) {
    return <div>No user data available</div>;
  }
  
  return (
    <div className="user-profile">
      <h2>{user.user_name}</h2>
      <p>Email: {user.email || 'No email provided'}</p>
      <p>Status: {user.is_active ? 'Active' : 'Inactive'}</p>
    </div>
  );
}
```

### User Welcome Banner

```tsx
function WelcomeBanner() {
  const { user, isLoading } = useUserData();
  
  if (isLoading || !user) {
    return <div className="banner">Welcome to Agent C</div>;
  }
  
  const displayName = user.first_name && user.last_name 
    ? `${user.first_name} ${user.last_name}`
    : user.user_name;
  
  return (
    <div className="banner">
      <h1>Welcome back, {displayName}!</h1>
      {user.last_login && (
        <p className="last-login">
          Last login: {new Date(user.last_login).toLocaleDateString()}
        </p>
      )}
    </div>
  );
}
```

### Role-Based Access Control

```tsx
function AdminPanel() {
  const { user, isLoading, error } = useUserData();
  
  // Check loading and error states
  if (isLoading) {
    return <div>Checking permissions...</div>;
  }
  
  if (error || !user) {
    return <div>Unable to verify permissions</div>;
  }
  
  // Check for admin role
  const isAdmin = user.roles.includes('admin');
  const isSuperAdmin = user.roles.includes('superadmin');
  
  if (!isAdmin) {
    return (
      <div className="access-denied">
        <h2>Access Denied</h2>
        <p>You need administrator privileges to view this page.</p>
      </div>
    );
  }
  
  return (
    <div className="admin-panel">
      <h2>Admin Dashboard</h2>
      <p>Welcome, {user.user_name}</p>
      
      {isSuperAdmin && (
        <div className="super-admin-tools">
          <h3>Super Admin Tools</h3>
          {/* Super admin only features */}
        </div>
      )}
      
      <div className="admin-tools">
        <h3>Administration</h3>
        {/* Regular admin features */}
      </div>
    </div>
  );
}
```

### User Avatar Component

```tsx
function UserAvatar() {
  const { user, isLoading } = useUserData();
  
  // Generate initials from user name
  const getInitials = () => {
    if (!user) return '?';
    
    if (user.first_name && user.last_name) {
      return `${user.first_name[0]}${user.last_name[0]}`.toUpperCase();
    }
    
    return user.user_name.substring(0, 2).toUpperCase();
  };
  
  // Generate color from user_id for consistent avatar colors
  const getAvatarColor = () => {
    if (!user) return '#gray';
    
    let hash = 0;
    for (let i = 0; i < user.user_id.length; i++) {
      hash = user.user_id.charCodeAt(i) + ((hash << 5) - hash);
    }
    const hue = hash % 360;
    return `hsl(${hue}, 70%, 50%)`;
  };
  
  return (
    <div 
      className="user-avatar"
      style={{ backgroundColor: getAvatarColor() }}
      title={user?.user_name || 'Loading...'}
    >
      {isLoading ? '...' : getInitials()}
    </div>
  );
}
```

### User Settings Form

```tsx
function UserSettings() {
  const { user, isLoading, error, refresh } = useUserData();
  const [editMode, setEditMode] = useState(false);
  const [formData, setFormData] = useState({
    first_name: '',
    last_name: '',
    email: ''
  });
  
  useEffect(() => {
    if (user) {
      setFormData({
        first_name: user.first_name || '',
        last_name: user.last_name || '',
        email: user.email || ''
      });
    }
  }, [user]);
  
  if (error) {
    return (
      <div className="error-state">
        <p>Failed to load user settings: {error}</p>
        <button onClick={refresh}>Retry</button>
      </div>
    );
  }
  
  if (isLoading || !user) {
    return <div>Loading user settings...</div>;
  }
  
  return (
    <div className="user-settings">
      <h2>User Settings</h2>
      
      <div className="read-only-fields">
        <p>User ID: {user.user_id}</p>
        <p>Username: {user.user_name}</p>
        <p>Account Created: {user.created_at ? new Date(user.created_at).toLocaleDateString() : 'Unknown'}</p>
        <p>Status: {user.is_active ? 'Active' : 'Inactive'}</p>
      </div>
      
      {editMode ? (
        <form className="edit-form">
          <input
            type="text"
            placeholder="First Name"
            value={formData.first_name}
            onChange={(e) => setFormData({ ...formData, first_name: e.target.value })}
          />
          <input
            type="text"
            placeholder="Last Name"
            value={formData.last_name}
            onChange={(e) => setFormData({ ...formData, last_name: e.target.value })}
          />
          <input
            type="email"
            placeholder="Email"
            value={formData.email}
            onChange={(e) => setFormData({ ...formData, email: e.target.value })}
          />
          <button type="submit">Save Changes</button>
          <button type="button" onClick={() => setEditMode(false)}>Cancel</button>
        </form>
      ) : (
        <div className="display-fields">
          <p>Name: {user.first_name} {user.last_name}</p>
          <p>Email: {user.email || 'Not provided'}</p>
          <button onClick={() => setEditMode(true)}>Edit Profile</button>
        </div>
      )}
    </div>
  );
}
```

### Group Membership Display

```tsx
function UserGroups() {
  const { user, isLoading } = useUserData();
  
  if (isLoading || !user) {
    return null;
  }
  
  if (user.groups.length === 0) {
    return (
      <div className="user-groups">
        <h3>Group Memberships</h3>
        <p className="empty-state">You are not a member of any groups.</p>
      </div>
    );
  }
  
  return (
    <div className="user-groups">
      <h3>Group Memberships ({user.groups.length})</h3>
      <ul className="group-list">
        {user.groups.map(group => (
          <li key={group} className="group-item">
            <span className="group-badge">{group}</span>
          </li>
        ))}
      </ul>
    </div>
  );
}
```

### User Session Info

```tsx
function SessionInfo() {
  const { user, isLoading, refresh } = useUserData();
  const [sessionAge, setSessionAge] = useState<string>('');
  
  useEffect(() => {
    if (!user?.last_login) return;
    
    const updateAge = () => {
      const lastLogin = new Date(user.last_login!);
      const now = new Date();
      const diffMs = now.getTime() - lastLogin.getTime();
      const diffMins = Math.floor(diffMs / 60000);
      const diffHours = Math.floor(diffMins / 60);
      const diffDays = Math.floor(diffHours / 24);
      
      if (diffDays > 0) {
        setSessionAge(`${diffDays} day${diffDays > 1 ? 's' : ''} ago`);
      } else if (diffHours > 0) {
        setSessionAge(`${diffHours} hour${diffHours > 1 ? 's' : ''} ago`);
      } else {
        setSessionAge(`${diffMins} minute${diffMins > 1 ? 's' : ''} ago`);
      }
    };
    
    updateAge();
    const interval = setInterval(updateAge, 60000); // Update every minute
    
    return () => clearInterval(interval);
  }, [user?.last_login]);
  
  if (isLoading || !user) {
    return null;
  }
  
  return (
    <div className="session-info">
      <h4>Session Information</h4>
      <p>User: {user.user_name}</p>
      <p>Session Status: {user.is_active ? '🟢 Active' : '🔴 Inactive'}</p>
      {user.last_login && (
        <p>Last login: {sessionAge}</p>
      )}
      <button onClick={refresh} className="refresh-btn">
        Refresh Session
      </button>
    </div>
  );
}
```

## Initialization Event Dependencies

The `useUserData` hook depends on a single WebSocket event:

- **`chat_user_data`** - Fired after successful WebSocket connection and authentication

### Event Flow

```typescript
// Authentication and connection
client.connect() → 'connected' event

// Server sends user data
→ 'chat_user_data' event → updates user state
  {
    type: 'chat_user_data',
    user: { /* User object */ }
  }

// Hook state updates
→ isLoading: false
→ user: User object
→ error: null
```

### Connection Lifecycle

1. **Initial state**: `user: null`, `isLoading: true`, `error: null`
2. **Connected**: Waiting for `chat_user_data` event
3. **Data received**: `user` populated, `isLoading: false`
4. **Disconnected**: `user` data persists, `isLoading: false`
5. **Reconnected**: Hook automatically re-subscribes to events

## Data Freshness and Synchronization

### Automatic Updates

The hook automatically updates when:
- The `chat_user_data` event is received from the server
- The connection is re-established after disconnection
- The user data changes on the server (rare occurrence)

### Manual Refresh

Use the `refresh()` function to manually sync user data:

```tsx
function RefreshableUserCard() {
  const { user, isLoading, error, refresh } = useUserData();
  const [lastRefresh, setLastRefresh] = useState<Date>(new Date());
  
  const handleRefresh = () => {
    refresh();
    setLastRefresh(new Date());
  };
  
  return (
    <div className="user-card">
      {error && (
        <div className="error-banner">
          {error}
          <button onClick={handleRefresh}>Retry</button>
        </div>
      )}
      
      {user && (
        <>
          <h3>{user.user_name}</h3>
          <p>Last refreshed: {lastRefresh.toLocaleTimeString()}</p>
        </>
      )}
      
      <button 
        onClick={handleRefresh} 
        disabled={isLoading}
      >
        {isLoading ? 'Refreshing...' : 'Refresh'}
      </button>
    </div>
  );
}
```

### Caching Behavior

- User data is cached in the RealtimeClient instance
- Data persists during disconnections (useful for offline states)
- The hook reads from the client cache, not maintaining its own copy
- Multiple components using the hook share the same data source

## StrictMode Compatibility

The `useUserData` hook is fully compatible with React StrictMode:

- **Double mounting**: Properly cleans up event listeners in the effect cleanup
- **Double effect execution**: Uses stable `useCallback` for the `updateUserData` function
- **No side effects**: All state updates are idempotent

Example with StrictMode:

```tsx
// App.tsx
import { StrictMode } from 'react';
import { AgentCProvider } from '@agentc/realtime-react';

function App() {
  return (
    <StrictMode>
      <AgentCProvider config={config}>
        <UserDashboard />
      </AgentCProvider>
    </StrictMode>
  );
}

// UserDashboard.tsx
function UserDashboard() {
  // Safe to use in StrictMode - no duplicate subscriptions
  const { user, isLoading } = useUserData();
  
  // Component renders correctly even with double mounting
  if (isLoading) return <div>Loading...</div>;
  if (!user) return <div>No user</div>;
  
  return <div>Welcome {user.user_name}</div>;
}
```

## Best Practices for Null Checking and Error Handling

### Defensive Null Checking

Always check for null values before accessing user properties:

```tsx
function SafeUserDisplay() {
  const { user, isLoading, error } = useUserData();
  
  // Guard clauses for each state
  if (error) {
    console.error('User data error:', error);
    return <ErrorFallback message={error} />;
  }
  
  if (isLoading) {
    return <LoadingSpinner />;
  }
  
  if (!user) {
    return <EmptyState message="No user data available" />;
  }
  
  // Safe property access with fallbacks
  const email = user.email ?? 'No email';
  const fullName = user.first_name && user.last_name
    ? `${user.first_name} ${user.last_name}`
    : user.user_name;
  
  const memberSince = user.created_at 
    ? new Date(user.created_at).getFullYear()
    : 'Unknown';
  
  return (
    <div>
      <h2>{fullName}</h2>
      <p>Email: {email}</p>
      <p>Member since: {memberSince}</p>
    </div>
  );
}
```

### Optional Chaining and Nullish Coalescing

Use modern JavaScript operators for safe access:

```tsx
function UserMetadata() {
  const { user } = useUserData();
  
  // Safe optional chaining
  const createdDate = user?.created_at 
    ? new Date(user.created_at) 
    : null;
  
  // Nullish coalescing for defaults
  const displayName = user?.first_name ?? user?.user_name ?? 'Guest';
  const roleCount = user?.roles?.length ?? 0;
  const primaryRole = user?.roles?.[0] ?? 'user';
  
  return (
    <div className="metadata">
      <p>Name: {displayName}</p>
      <p>Primary Role: {primaryRole}</p>
      <p>Total Roles: {roleCount}</p>
      {createdDate && (
        <p>Member for {Math.floor((Date.now() - createdDate.getTime()) / (1000 * 60 * 60 * 24))} days</p>
      )}
    </div>
  );
}
```

### Error Boundary Integration

Wrap components with error boundaries for resilience:

```tsx
import { ErrorBoundary } from 'react-error-boundary';

function UserSection() {
  return (
    <ErrorBoundary
      fallbackRender={({ error, resetErrorBoundary }) => (
        <div className="error-fallback">
          <h3>User data unavailable</h3>
          <p>{error.message}</p>
          <button onClick={resetErrorBoundary}>Try Again</button>
        </div>
      )}
      onError={(error) => {
        // Log to error reporting service
        console.error('User data error:', error);
      }}
    >
      <UserDataConsumer />
    </ErrorBoundary>
  );
}

function UserDataConsumer() {
  const { user, error } = useUserData();
  
  // Throw to trigger error boundary
  if (error) {
    throw new Error(`Failed to load user: ${error}`);
  }
  
  // Component assumes user exists after error check
  return <div>User: {user?.user_name}</div>;
}
```

### Loading State Patterns

Implement progressive loading states:

```tsx
function ProgressiveUserLoad() {
  const { user, isLoading, error, refresh } = useUserData();
  const [retryCount, setRetryCount] = useState(0);
  
  const handleRetry = () => {
    setRetryCount(prev => prev + 1);
    refresh();
  };
  
  // Error state with retry logic
  if (error) {
    return (
      <div className="error-state">
        <p>Unable to load user profile</p>
        {retryCount < 3 ? (
          <button onClick={handleRetry}>
            Retry ({3 - retryCount} attempts remaining)
          </button>
        ) : (
          <p>Please check your connection and reload the page.</p>
        )}
      </div>
    );
  }
  
  // Loading skeleton
  if (isLoading) {
    return (
      <div className="user-skeleton">
        <div className="skeleton-avatar" />
        <div className="skeleton-text" />
        <div className="skeleton-text short" />
      </div>
    );
  }
  
  // Empty state
  if (!user) {
    return (
      <div className="empty-state">
        <p>No user profile found</p>
        <button onClick={refresh}>Load Profile</button>
      </div>
    );
  }
  
  // Success state
  return (
    <div className="user-loaded">
      <h2>{user.user_name}</h2>
      {/* Full user UI */}
    </div>
  );
}
```

### Type-Safe Access Patterns

Create type-safe helper functions:

```tsx
// helpers/userHelpers.ts
export function getUserDisplayName(user: User | null): string {
  if (!user) return 'Guest';
  
  if (user.first_name && user.last_name) {
    return `${user.first_name} ${user.last_name}`;
  }
  
  return user.user_name;
}

export function hasRole(user: User | null, role: string): boolean {
  return user?.roles?.includes(role) ?? false;
}

export function isInGroup(user: User | null, group: string): boolean {
  return user?.groups?.includes(group) ?? false;
}

export function getUserInitials(user: User | null): string {
  if (!user) return '?';
  
  if (user.first_name && user.last_name) {
    return `${user.first_name[0]}${user.last_name[0]}`.toUpperCase();
  }
  
  return user.user_name.substring(0, 2).toUpperCase();
}

// Component using helpers
function TypeSafeUserCard() {
  const { user } = useUserData();
  
  return (
    <div className="user-card">
      <div className="avatar">{getUserInitials(user)}</div>
      <h3>{getUserDisplayName(user)}</h3>
      {hasRole(user, 'admin') && <span className="admin-badge">Admin</span>}
      {isInGroup(user, 'premium') && <span className="premium-badge">Premium</span>}
    </div>
  );
}
```

## Performance Considerations

- The hook is lightweight and only subscribes to user-specific events
- Data is cached at the client level, not duplicated per component
- Multiple components can use the hook without performance impact
- Consider memoization for derived values:

```tsx
function OptimizedUserDisplay() {
  const { user, isLoading } = useUserData();
  
  // Memoize expensive computations
  const userStats = useMemo(() => {
    if (!user) return null;
    
    return {
      displayName: getUserDisplayName(user),
      initials: getUserInitials(user),
      isAdmin: hasRole(user, 'admin'),
      isPremium: isInGroup(user, 'premium'),
      accountAge: user.created_at 
        ? Math.floor((Date.now() - new Date(user.created_at).getTime()) / (1000 * 60 * 60 * 24))
        : 0
    };
  }, [user]);
  
  if (isLoading || !userStats) {
    return <LoadingSpinner />;
  }
  
  return (
    <div>
      <h2>{userStats.displayName}</h2>
      <p>Account age: {userStats.accountAge} days</p>
      {userStats.isAdmin && <AdminBadge />}
      {userStats.isPremium && <PremiumBadge />}
    </div>
  );
}
```

## See Also

- [`useAgentCData`](./useAgentCData.md) - For accessing complete configuration data including user
- [`useRealtimeClient`](./useRealtimeClient.md) - For direct client access
- [`useConnection`](./useConnection.md) - For monitoring connection state
- [AgentCProvider](../providers/AgentCProvider.md) - Provider setup and configuration