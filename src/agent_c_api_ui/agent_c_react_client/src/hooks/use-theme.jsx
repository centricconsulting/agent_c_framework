/**
 * Custom hook for accessing the ThemeContext
 */

import { useTheme as useThemeContext } from '../contexts/ThemeContext';

/**
 * Hook that provides access to theme state and theme change functionality
 * @returns {Object} The ThemeContext value { theme, setTheme }
 */
export const useTheme = () => {
  return useThemeContext();
};