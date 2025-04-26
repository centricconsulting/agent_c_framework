import React from 'react';
import { Button } from './button';
import { Icon } from './icon';
import { useTheme } from '../../hooks/use-theme';
import '../../styles/components/icon.css';

export function ThemeToggle() {
  const { theme, setTheme } = useTheme();

  return (
    <div className="flex items-center space-x-1 border dark:border-gray-700 rounded-lg p-0.5 bg-white/10 dark:bg-gray-800/20 backdrop-blur-sm">
      <Button
        variant={theme === 'light' ? 'default' : 'secondary'}
        size="icon"
        onClick={() => setTheme('light')}
        title="Light Mode"
        className="hover:bg-amber-100 dark:hover:bg-amber-900/30 hover:text-amber-600 dark:hover:text-amber-400 transition-colors text-gray-700 dark:text-gray-300"
      >
        <Icon icon="fa-regular fa-sun" hoverIcon="fa-solid fa-sun" size="lg" />
        <span className="sr-only">Light Mode</span>
      </Button>
      <Button
        variant={theme === 'dark' ? 'default' : 'secondary'}
        size="icon"
        onClick={() => setTheme('dark')}
        title="Dark Mode"
        className="hover:bg-indigo-100 dark:hover:bg-indigo-900/30 hover:text-indigo-600 dark:hover:text-indigo-400 transition-colors text-gray-700 dark:text-gray-300"
      >
        <Icon icon="fa-regular fa-moon" hoverIcon="fa-solid fa-moon" size="lg" />
        <span className="sr-only">Dark Mode</span>
      </Button>
      <Button
        variant={theme === 'system' ? 'default' : 'secondary'}
        size="icon"
        onClick={() => setTheme('system')}
        title="System Theme"
        className="hover:bg-blue-100 dark:hover:bg-blue-900/30 hover:text-blue-600 dark:hover:text-blue-400 transition-colors text-gray-700 dark:text-gray-300"
      >
        <Icon icon="fa-regular fa-laptop" hoverIcon="fa-solid fa-laptop" size="lg" />
        <span className="sr-only">System Theme</span>
      </Button>
    </div>
  );
}