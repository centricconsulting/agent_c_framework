import React from 'react';
import { CommandIcon, Settings } from 'lucide-react';
import { useState, useEffect } from 'react';
import { useSessionContext } from '../hooks/use-session-context';
import { useTheme } from '../hooks/use-theme';
import { Button } from './ui/button';
import {
  CommandDialog,
  CommandEmpty,
  CommandGroup,
  CommandInput,
  CommandItem,
  CommandList,
  CommandSeparator,
  CommandShortcut,
} from './ui/command';
import { DialogTitle } from './ui/dialog';
import { useToast } from '../hooks/use-toast';
import { Icon } from './ui/icon';
// Directly import the formatter functions as a fallback
import { formatChatAsHTML, createClipboardContent } from './chat_interface/utils/htmlChatFormatter';



const SidebarCommandMenu = () => {
  const [open, setOpen] = useState(false);
  const { toast } = useToast();
  const { theme, setTheme: handleThemeChange } = useTheme('SidebarCommandMenu');
  const { 
    messages, 
    settingsVersion,
    getChatCopyContent,
    getChatCopyHTML
  } = useSessionContext('SidebarCommandMenu');
  
  // We need the sessionId from AuthContext but we can use useSessionContext for now
  // until we update this component to use AuthContext directly
  const { sessionId } = useSessionContext('SidebarCommandMenu');

  useEffect(() => {
    // Debug logging
    console.log('SidebarCommandMenu mounted, messages count:', messages?.length || 0);
  }, [messages]);

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
        <DialogTitle className="sr-only">Command Menu</DialogTitle>
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
          
          {/* Chat Export Actions */}
          <CommandGroup heading="Chat Actions">
            <CommandItem
              onSelect={() => {
                console.log('Copy button clicked, messages:', messages?.length || 0);
                if (!messages || messages.length === 0) {
                  toast({
                    title: "No messages to copy",
                    description: "Start a conversation first",
                    variant: "destructive"
                  });
                  return;
                }

                // Try direct copying with the formatter functions first
                try {
                  // 1. Try using SessionContext methods if available
                  let plainText, html;
                  
                  if (typeof getChatCopyContent === 'function' && typeof getChatCopyHTML === 'function') {
                    console.log('Using SessionContext methods for copy');
                    plainText = getChatCopyContent();
                    html = getChatCopyHTML();
                  } else {
                    // 2. Fallback to direct usage of formatter functions
                    console.log('Using direct formatter functions for copy');
                    const clipboardContent = createClipboardContent(messages);
                    plainText = clipboardContent.text;
                    html = clipboardContent.html;
                  }
                  
                  console.log('Clipboard content generated:', !!plainText, !!html);
                  
                  // Use the Clipboard API with ClipboardItem for multiple formats
                  const clipboardItem = new ClipboardItem({
                    'text/plain': new Blob([plainText], { type: 'text/plain' }),
                    'text/html': new Blob([html], { type: 'text/html' })
                  });

                  navigator.clipboard.write([clipboardItem])
                    .then(() => {
                      toast({
                        title: "Chat copied",
                        description: "Conversation copied to clipboard"
                      });
                      setOpen(false);
                    })
                    .catch(err => {
                      console.error('Failed to copy content:', err);
                      // Fallback to text-only copying
                      navigator.clipboard.writeText(plainText)
                        .then(() => {
                          toast({
                            title: "Chat copied (text only)",
                            description: "Rich formatting not supported in your browser"
                          });
                          setOpen(false);
                        })
                        .catch(err => {
                          console.error('Text fallback failed:', err);
                          toast({
                            title: "Copy failed",
                            description: "Could not copy to clipboard",
                            variant: "destructive"
                          });
                        });
                    });
                } catch (error) {
                  console.error('Error copying content:', error);
                  toast({
                    title: "Copy failed",
                    description: error.message,
                    variant: "destructive"
                  });
                }
              }}
            >
              <Icon icon="fa-regular fa-copy" className="mr-2" />
              <span>Copy to Clipboard</span>
              <CommandShortcut>⌘+⇧+C</CommandShortcut>
            </CommandItem>
            <CommandItem
              onSelect={() => {
                console.log('Export button clicked, messages:', messages?.length || 0);
                if (!messages || messages.length === 0) {
                  toast({
                    title: "No messages to export",
                    description: "Start a conversation first",
                    variant: "destructive"
                  });
                  return;
                }

                try {
                  // Try both methods to get the HTML content
                  let html;
                  
                  if (typeof getChatCopyHTML === 'function') {
                    console.log('Using SessionContext method for HTML export');
                    html = getChatCopyHTML();
                  } else {
                    console.log('Using direct formatter function for HTML export');
                    html = formatChatAsHTML(messages);
                  }
                  
                  console.log('HTML content generated:', html ? 'yes' : 'no');
                  
                  if (!html) {
                    toast({
                      title: "Export failed",
                      description: "Could not generate HTML content",
                      variant: "destructive"
                    });
                    return;
                  }

                  // Create a blob from the HTML content
                  const blob = new Blob([html], { type: 'text/html' });

                  // Create a URL for the blob
                  const url = URL.createObjectURL(blob);

                  // Create a temporary link and trigger download
                  const link = document.createElement('a');
                  link.href = url;
                  link.download = `chat-export-${new Date().toISOString().slice(0, 10)}.html`;
                  link.setAttribute('aria-hidden', 'true');
                  document.body.appendChild(link);
                  link.click();

                  // Clean up
                  document.body.removeChild(link);
                  URL.revokeObjectURL(url);
                  
                  toast({
                    title: "Export complete",
                    description: "Chat exported as HTML file"
                  });
                  setOpen(false);
                } catch (error) {
                  console.error('Error exporting chat:', error);
                  toast({
                    title: "Export failed",
                    description: error.message,
                    variant: "destructive"
                  });
                }
              }}
            >
              <Icon icon="fa-regular fa-download" className="mr-2" />
              <span>Export as HTML</span>
              <CommandShortcut>⌘+⇧+E</CommandShortcut>
            </CommandItem>
          </CommandGroup>

          <CommandSeparator />
          
          {/* Agent Info Group */}
          <CommandGroup heading="Agent Information">
            <CommandItem
              onSelect={() => {
                // Toggle the agent info hover card programmatically
                // For now, just close the command menu
                setOpen(false);
                
                // We'll implement the actual agent info popup in the next step
                toast({
                  title: "Agent Information",
                  description: `Session ID: ${sessionId || 'Not available'}`,
                });
              }}
            >
              <Settings className="mr-2 h-4 w-4" />
              <span>View Agent Configuration</span>
              <CommandShortcut>⌘+I</CommandShortcut>
            </CommandItem>
          </CommandGroup>
        </CommandList>
      </CommandDialog>
    </>
  );
};

export default SidebarCommandMenu;