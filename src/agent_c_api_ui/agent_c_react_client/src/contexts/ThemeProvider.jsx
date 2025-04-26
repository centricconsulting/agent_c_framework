import React from 'react';
import { ThemeProvider as ThemeContextProvider } from './ThemeContext';

/**
 * ThemeProvider component
 * This is now a wrapper around our ThemeContextProvider to maintain backwards compatibility
 * and make the migration process smoother
 */
export const ThemeProvider = ({ children }) => {
  return <ThemeContextProvider>{children}</ThemeContextProvider>;
};