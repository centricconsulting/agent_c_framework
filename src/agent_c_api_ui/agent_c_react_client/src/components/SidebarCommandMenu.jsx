import React from 'react';
import { CommandIcon } from 'lucide-react';
import { useContext } from 'react';
import { SessionContext } from '../contexts/SessionContext';
import { Button } from './ui/button';
import {
  CommandDialog,
  CommandEmpty,
  CommandGroup,
  CommandInput,
  CommandItem,
  CommandList,
  CommandSeparator,
} from './ui/command';

const SidebarCommandMenu = () => {
  const [open, setOpen] = React.useState(false);
  const { theme, handleThemeChange } = useContext(SessionContext);

  // Handle keyboard shortcut to open command menu
  React.useEffect(() => {
    const down = (e) => {
      if (e.key === 'k' && (e.metaKey || e.ctrlKey)) {
        e.preventDefault();
        setOpen((open) => !open);
      }
    };

    document.addEventListener('keydown', down);
    return () => document.removeEventListener('keydown', down);
  }, []);

  return (
    <>
      <Button
        variant="ghost"
        className="sidebar-command-button"
        onClick={() => setOpen(true)}
        aria-label="Open command menu"
      >
        <CommandIcon className="h-5 w-5" />
      </Button>

      <CommandDialog open={open} onOpenChange={setOpen}>
        <CommandInput placeholder="Type a command or search..." />
        <CommandList>
          <CommandEmpty>No results found.</CommandEmpty>
          
          {/* Theme Options */}
          <CommandGroup heading="Theme">
            <CommandItem onSelect={() => handleThemeChange("light")}>
              Light Mode
              {theme === "light" && <span className="ml-auto text-xs">✓</span>}
            </CommandItem>
            <CommandItem onSelect={() => handleThemeChange("dark")}>
              Dark Mode
              {theme === "dark" && <span className="ml-auto text-xs">✓</span>}
            </CommandItem>
            <CommandItem onSelect={() => handleThemeChange("system")}>
              System Theme
              {theme === "system" && <span className="ml-auto text-xs">✓</span>}
            </CommandItem>
          </CommandGroup>

          <CommandSeparator />
          
          {/* Chat Export Placeholder Group */}
          <CommandGroup heading="Chat Actions">
            <CommandItem>
              Copy to Clipboard
            </CommandItem>
            <CommandItem>
              Export as HTML
            </CommandItem>
          </CommandGroup>

          <CommandSeparator />
          
          {/* Agent Info Placeholder Group */}
          <CommandGroup heading="Agent Information">
            <CommandItem>
              View Agent Configuration
            </CommandItem>
          </CommandGroup>
        </CommandList>
      </CommandDialog>
    </>
  );
};

export default SidebarCommandMenu;