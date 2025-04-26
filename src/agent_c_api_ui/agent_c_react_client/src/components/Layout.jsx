import React from 'react';
import { useLocation } from 'react-router-dom';
import AppSidebar from './AppSidebar';
import PageHeader from './PageHeader';
import { cn } from '../lib/utils';
import { useLogger } from '../hooks/use-logger';

/**
 * Main application layout with sidebar and content area
 * 
 * @param {Object} props - Component props
 * @param {React.ReactNode} props.children - Page content
 * @param {string} [props.title] - Page title for header (optional)
 * @param {React.ReactNode} [props.headerActions] - Actions to display in header (optional)
 * @param {boolean} [props.showHeader=true] - Whether to show the page header
 * @param {string} [props.className] - Additional CSS classes for the content
 * @returns {JSX.Element} Layout component
 */
const Layout = ({
  children,
  title,
  headerActions,
  showHeader = true,
  className,
}) => {
  const location = useLocation();
  const isHome = location.pathname === '/';
  
  // Simple component logging
  useLogger('Layout');

  return (
    <AppSidebar>
      <main className={cn(
        "layout-main",
        isHome ? "layout-main-home" : "layout-main-page",
        className
      )} style={{ display: 'flex', flexDirection: 'column', flex: '1 1 auto' }}>
        {children}
      </main>
    </AppSidebar>
  );
};

export default Layout;