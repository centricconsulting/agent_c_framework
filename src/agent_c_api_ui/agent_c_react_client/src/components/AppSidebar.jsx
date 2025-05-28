import React, { useState } from 'react';
import footerLogo from '../assets/footer-logo.svg';
import { Link, useLocation } from 'react-router-dom';
import { Home, Settings, Database, PanelLeft, Menu, History, LogOut } from 'lucide-react';
import {
  Sidebar,
  SidebarContent,
  SidebarFooter,
  SidebarHeader,
  SidebarMenu,
  SidebarMenuButton,
  SidebarMenuItem,
  SidebarProvider,
  SidebarTrigger,
  useSidebar,
} from './ui/sidebar';
import { ThemeToggle } from './ui/theme-toggle';
import { Separator } from './ui/separator';
import { cn } from '../lib/utils';
import {Icon} from "@/components/ui/icon";
import { useAuth } from '../contexts/AuthContext';
import { Popover, PopoverContent, PopoverTrigger } from './ui/popover';
import { Button } from './ui/button';

/**
 * Main application sidebar using shadcn/ui Sidebar component
 * 
 * @param {Object} props - Component props
 * @param {React.ReactNode} props.children - Content to be displayed alongside the sidebar
 * @param {boolean} [props.defaultOpen=true] - Whether the sidebar is open by default
 * @returns {JSX.Element} AppSidebar component
 */
const AppSidebar = ({ children, defaultOpen = true, collapsible = "icon" }) => {
  const location = useLocation();

  // Application navigation links with icons
  const navLinks = [
    { path: '/chat', label: 'Chat', icon: <span className="mr-3 ml-2"><Icon icon="fa-thin fa-messages" hoverIcon="fa-solid fa-messages" size="lg" /></span> },
    { path: '/rag', label: 'RAG', icon: <span className="mr-3 ml-2"><Icon icon="fa-thin fa-file-circle-plus" hoverIcon="fa-solid fa-file-circle-plus" size="lg" /></span> },
    { path: '/interactions', label: 'Event Logs', icon: <span className="mr-3 ml-2"><Icon icon="fa-thin fa-list-timeline" hoverIcon="fa-solid fa-list-timeline" size="lg" /></span> },
    { path: '/settings', label: 'Settings', icon:<span className="mr-3 ml-2"><Icon icon="fa-thin fa-gears" hoverIcon="fa-solid fa-list-gears" size="lg" /></span> },
  ];

  // Check if a path is the current active route
  const isActive = (path) => location.pathname === path;

  return (
    <SidebarProvider defaultOpen={defaultOpen}>
      <div className="sidebar-layout">
        <Sidebar collapsible={collapsible}>
          <SidebarHeader className="flex justify-start">
            <div className="sidebar-logo flex ">
              <SidebarTrigger />
              <h2 className="text-lg font-semibold sidebar-title">Agent C</h2>
            </div>
          </SidebarHeader>
          
          <SidebarContent>
            <SidebarMenu>
              {navLinks.map((link) => (
                <SidebarMenuItem key={link.path}>
                  <Link to={link.path} className="w-full">
                    <SidebarMenuButton 
                      isActive={isActive(link.path)}
                      tooltip={link.label}
                      className={cn(
                        "w-full justify-start",
                        isActive(link.path) && "font-medium"
                      )}
                    >
                      {link.icon}
                      <span className="sidebar-menu-label">{link.label}</span>
                    </SidebarMenuButton>
                  </Link>
                </SidebarMenuItem>
              ))}
            </SidebarMenu>
          </SidebarContent>

          <SidebarFooter>
            <Separator className="my-2" />
            <div className="flex flex-col items-center px-2 gap-3">
              <UserProfileSection />
              <div className="theme-toggle-container">
                <ThemeToggle/>
              </div>
              <div className="footer-logo-container">
                <img src={footerLogo} alt="Footer Logo" className="footer-logo" />
              </div>
            </div>
          </SidebarFooter>
        </Sidebar>

        <div className="sidebar-content">
          <MobileFloatingToggle/>
          {children}
        </div>
      </div>
    </SidebarProvider>
  );
};

// Mobile-only floating toggle that appears on small screens
const MobileFloatingToggle = () => {
  const { toggleSidebar, isMobile } = useSidebar();
  
  // Only render on mobile devices
  if (!isMobile) return null;
  
  return (
    <button
      aria-label="Open Sidebar"
      title="Open Sidebar"
      className="sidebar-floating-toggle"
      onClick={toggleSidebar}
    >
      <Menu size={18} />
    </button>
  );
};

// User profile section with logout functionality
const UserProfileSection = () => {
  const { user, logout } = useAuth();
  const [isOpen, setIsOpen] = useState(false);

  if (!user) return null;

  const handleLogout = async () => {
    try {
      await logout();
      setIsOpen(false);
    } catch (error) {
      console.error('Logout failed:', error);
    }
  };

  return (
    <div className="w-full px-3 mb-2 user-profile-section">
      <Popover open={isOpen} onOpenChange={setIsOpen}>
        <PopoverTrigger asChild>
          <button
            className="user-profile-button"
            title={`Signed in as ${user.name}`}
          >
            <span className="mr-3 ml-2">
              <Icon icon="fa-thin fa-user" hoverIcon="fa-solid fa-user" size="lg" />
            </span>
            <span className="sidebar-menu-label user-profile-name">{user.name}</span>
          </button>
        </PopoverTrigger>
        <PopoverContent className="user-profile-popover p-0" align="start">
          <div className="p-2">
            <div className="user-profile-popover-header">Signed in as</div>
            <div className="user-profile-popover-name">{user.name}</div>
            <Button 
              variant="destructive" 
              size="sm" 
              className="w-full" 
              onClick={handleLogout}
            >
              <LogOut size={16} className="mr-2" />
              Sign Out
            </Button>
          </div>
        </PopoverContent>
      </Popover>
    </div>
  );
};

export default AppSidebar;