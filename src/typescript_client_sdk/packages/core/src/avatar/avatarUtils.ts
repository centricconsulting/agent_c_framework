import { Avatar } from '../events/types/CommonTypes';

/**
 * Avatar utility functions for working with Agent C avatars.
 * 
 * Note: These utilities work with Agent C avatar data, not HeyGen SDK.
 * The client application handles HeyGen SDK integration directly.
 */

/**
 * Check if avatar data is valid
 */
export function isValidAvatar(avatar: unknown): avatar is Avatar {
  return (
    !!avatar &&
    typeof avatar === 'object' &&
    'avatar_id' in avatar &&
    typeof (avatar as any).avatar_id === 'string' &&
    'status' in avatar &&
    typeof (avatar as any).status === 'string'
  );
}

/**
 * Format avatar for display (e.g., in a dropdown)
 */
export function formatAvatarDisplay(avatar: Avatar): string {
  // Use avatar_id as display name since Avatar doesn't have a name field
  // You could also use pose_name if that's more descriptive
  return avatar.avatar_id;
}

/**
 * Get avatar by ID from a list
 */
export function getAvatarById(avatars: Avatar[], avatarId: string): Avatar | undefined {
  return avatars.find(a => a.avatar_id === avatarId);
}

/**
 * Sort avatars alphabetically by ID
 */
export function sortAvatarsById(avatars: Avatar[]): Avatar[] {
  return [...avatars].sort((a, b) => {
    return a.avatar_id.localeCompare(b.avatar_id);
  });
}

/**
 * Filter avatars by a search term
 */
export function filterAvatars(avatars: Avatar[], searchTerm: string): Avatar[] {
  const term = searchTerm.toLowerCase();
  return avatars.filter(avatar => {
    const id = avatar.avatar_id.toLowerCase();
    const pose = (avatar.pose_name || '').toLowerCase();
    return id.includes(term) || pose.includes(term);
  });
}

/**
 * Group avatars by a property (useful for categorized display)
 */
export function groupAvatars(avatars: Avatar[]): Map<string, Avatar[]> {
  const groups = new Map<string, Avatar[]>();
  
  avatars.forEach(avatar => {
    // Group by first letter of avatar_id
    // Could be extended to group by status, pose_name, etc.
    const key = avatar.avatar_id.charAt(0).toUpperCase();
    const group = groups.get(key) || [];
    group.push(avatar);
    groups.set(key, group);
  });
  
  return groups;
}