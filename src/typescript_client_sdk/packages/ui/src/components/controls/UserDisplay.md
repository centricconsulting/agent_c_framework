# UserDisplay Component Documentation

## Overview

The `UserDisplay` component is a production-ready React component for displaying user profile information in the Agent C Realtime SDK. It handles the new event-driven authentication flow where user data is received via WebSocket events after connection, rather than from the initial login response.

## Features

- **Event-Driven Data Flow**: Automatically updates when `chat_user_data` event is received
- **Multiple Display Variants**: Compact inline display or full profile card
- **Loading States**: Skeleton loaders while waiting for user data
- **Connection Status**: Optional real-time connection indicator
- **Dropdown Menu**: Configurable action menu with logout, settings, and custom items
- **Accessibility**: Full WCAG 2.1 AA compliance with ARIA labels and keyboard navigation
- **Responsive Design**: Works on mobile and desktop devices
- **Dark Mode Support**: Follows CenSuite design system color tokens

## Installation

The component is part of the `@agentc/realtime-ui` package:

```bash
npm install @agentc/realtime-ui
# or
pnpm add @agentc/realtime-ui
```

## Basic Usage

```tsx
import { UserDisplay } from '@agentc/realtime-ui';

// Simple compact display
<UserDisplay variant="compact" />

// Full profile card
<UserDisplay variant="full" />

// With menu and actions
<UserDisplay 
  variant="compact"
  showMenu
  onLogout={() => handleLogout()}
  onSettings={() => handleSettings()}
/>
```

## Props

| Prop | Type | Default | Description |
|------|------|---------|-------------|
| `variant` | `"compact" \| "full"` | `"compact"` | Display variant |
| `showMenu` | `boolean` | `false` | Show dropdown menu with actions |
| `showConnectionStatus` | `boolean` | `false` | Show WebSocket connection status |
| `avatarUrl` | `string` | - | Custom avatar image URL |
| `avatarSize` | `"sm" \| "md" \| "lg"` | `"md"` | Avatar size (compact variant only) |
| `onLogout` | `() => void` | - | Callback when logout is clicked |
| `onSettings` | `() => void` | - | Callback when settings is clicked |
| `customMenuItems` | `MenuItem[]` | - | Additional menu items |
| `className` | `string` | - | Additional CSS classes |

### MenuItem Type

```tsx
interface MenuItem {
  label: string;
  icon?: React.ReactNode;
  onClick: () => void;
  variant?: "default" | "destructive";
}
```

## Display Variants

### Compact Variant

The compact variant is designed for headers, navigation bars, and other space-constrained areas:

```tsx
<UserDisplay 
  variant="compact"
  avatarSize="sm"
  showConnectionStatus
/>
```

Features:
- Avatar with initials or image
- User name (falls back to email if no name)
- Optional email display
- Connection status indicator
- Dropdown menu support

### Full Variant

The full variant provides a detailed profile card suitable for sidebars, profile pages, or modals:

```tsx
<UserDisplay 
  variant="full"
  className="max-w-md"
  showMenu
/>
```

Features:
- Large avatar (64x64px)
- Full name and email
- Active/inactive status badge
- User roles and groups
- Account creation date
- Last login time
- Connection status
- Action menu

## Loading States

The component automatically handles loading states while waiting for user data:

1. **Initial Load**: Shows skeleton placeholders
2. **Data Received**: Transitions to display actual user information
3. **Error State**: Shows error message if data fails to load

## Data Source

The component uses these hooks from `@agentc/realtime-react`:

- `useAgentCData()` - Retrieves user data from WebSocket events
- `useInitializationStatus()` - Checks if initialization events have been received
- `useConnection()` - Monitors WebSocket connection status

## User Data Structure

The component expects user data in this format:

```tsx
interface User {
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

## Examples

### Header Integration

```tsx
function AppHeader() {
  const router = useRouter();
  
  return (
    <header className="flex items-center justify-between px-6 py-4 border-b">
      <Logo />
      
      <UserDisplay 
        variant="compact"
        showMenu
        showConnectionStatus
        onLogout={() => router.push('/logout')}
        onSettings={() => router.push('/settings')}
        customMenuItems={[
          {
            label: 'Profile',
            icon: <User className="h-4 w-4" />,
            onClick: () => router.push('/profile')
          },
          {
            label: 'Billing',
            icon: <CreditCard className="h-4 w-4" />,
            onClick: () => router.push('/billing')
          }
        ]}
      />
    </header>
  );
}
```

### Sidebar Profile

```tsx
function Sidebar() {
  return (
    <aside className="w-80 border-r h-full p-4">
      <UserDisplay 
        variant="full"
        showMenu
        showConnectionStatus
        className="mb-6"
      />
      
      <Navigation />
    </aside>
  );
}
```

### Custom Avatar

```tsx
function CustomAvatarExample() {
  const { user } = useAgentCData();
  
  return (
    <UserDisplay 
      variant="compact"
      avatarUrl={user?.avatar_url || `/api/avatar/${user?.user_id}`}
      avatarSize="lg"
    />
  );
}
```

## Standalone UserAvatar Component

The package also exports a lightweight `UserAvatar` component for displaying just the avatar:

```tsx
import { UserAvatar } from '@agentc/realtime-ui';

<UserAvatar size="md" />
<UserAvatar size="lg" avatarUrl="https://example.com/avatar.jpg" />
```

### UserAvatar Props

| Prop | Type | Default | Description |
|------|------|---------|-------------|
| `size` | `"sm" \| "md" \| "lg"` | `"md"` | Avatar size |
| `avatarUrl` | `string` | - | Custom avatar image URL |
| `className` | `string` | - | Additional CSS classes |

## Accessibility Features

- **ARIA Labels**: All interactive elements have descriptive labels
- **Keyboard Navigation**: Full keyboard support with Tab, Enter, and Escape keys
- **Screen Reader Support**: Proper role attributes and live regions
- **Focus Management**: Clear focus indicators and logical tab order
- **Status Announcements**: Connection status changes are announced to screen readers

## Styling

The component uses the CenSuite design system with Tailwind CSS classes. All colors use semantic tokens that automatically adapt to light/dark mode:

```css
--primary: Primary actions and branding
--secondary: Supporting elements
--muted: Disabled or inactive states
--destructive: Errors and destructive actions
--foreground/background: Text and backgrounds
```

## Error Handling

The component gracefully handles various error scenarios:

1. **No User Data**: Shows "No user data" message
2. **Connection Lost**: Updates connection indicator
3. **Incomplete Data**: Uses fallbacks for missing fields
4. **Loading Timeout**: Shows error state after reasonable delay

## Performance Considerations

- Uses React.memo for expensive computations
- Minimal re-renders through proper hook usage
- Skeleton loaders prevent layout shift
- Lazy loads dropdown menu content

## Browser Support

- Chrome 90+
- Firefox 88+
- Safari 14+
- Edge 90+

## Related Components

- `ConnectionButton` - WebSocket connection control
- `AudioControls` - Voice/audio management
- `ChatInterface` - Full chat UI

## Support

For issues or questions, please refer to the Agent C Realtime SDK documentation or contact the development team.