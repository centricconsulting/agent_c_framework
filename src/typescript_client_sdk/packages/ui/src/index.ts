// Utility exports
export { cn } from './lib/utils';

// Core UI Components (CenSuite Design System)
// These are production-ready components following CenSuite patterns

// Alert
export { Alert, AlertTitle, AlertDescription } from './components/ui/alert';

// Avatar
export { Avatar, AvatarImage, AvatarFallback } from './components/ui/avatar';

// Badge
export { Badge } from './components/ui/badge';
export type { BadgeProps } from './components/ui/badge';

// Agent C Logo
export { AgentCLogo } from './components/ui/agent-c-logo';
export type { AgentCLogoProps } from './components/ui/agent-c-logo';

// Button
export { Button, buttonVariants } from './components/ui/button';
export type { ButtonProps } from './components/ui/button';

// Card
export { 
  Card, 
  CardHeader, 
  CardFooter, 
  CardTitle, 
  CardDescription, 
  CardContent 
} from './components/ui/card';

// Dropdown Menu
export {
  DropdownMenu,
  DropdownMenuTrigger,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuCheckboxItem,
  DropdownMenuRadioItem,
  DropdownMenuLabel,
  DropdownMenuSeparator,
  DropdownMenuShortcut,
  DropdownMenuGroup,
  DropdownMenuPortal,
  DropdownMenuSub,
  DropdownMenuSubContent,
  DropdownMenuSubTrigger,
  DropdownMenuRadioGroup,
} from './components/ui/dropdown-menu';

// Form
export {
  useFormField,
  Form,
  FormItem,
  FormLabel,
  FormControl,
  FormDescription,
  FormMessage,
  FormField,
} from './components/ui/form';

// Input
export { Input } from './components/ui/input';
export type { InputProps } from './components/ui/input';

// Label
export { Label } from './components/ui/label';

// ScrollArea
export { ScrollArea, ScrollBar } from './components/ui/scroll-area';

// Select
export {
  Select,
  SelectGroup,
  SelectValue,
  SelectTrigger,
  SelectContent,
  SelectLabel,
  SelectItem,
  SelectSeparator,
} from './components/ui/select';

// Separator
export { Separator } from './components/ui/separator';

// Sheet
export {
  Sheet,
  SheetPortal,
  SheetOverlay,
  SheetTrigger,
  SheetClose,
  SheetContent,
  SheetHeader,
  SheetFooter,
  SheetTitle,
  SheetDescription,
} from './components/ui/sheet';

// Skeleton
export { Skeleton } from './components/ui/skeleton';

// Slider
export { Slider } from './components/ui/slider';

// Tabs
export { Tabs, TabsList, TabsTrigger, TabsContent } from './components/ui/tabs';

// Textarea
export { Textarea } from './components/ui/textarea';
export type { TextareaProps } from './components/ui/textarea';

// Toggle Group
export { ToggleGroup, ToggleGroupItem } from './components/ui/toggle-group';

// Tooltip
export {
  Tooltip,
  TooltipTrigger,
  TooltipContent,
  TooltipProvider,
} from './components/ui/tooltip';

// Toast/Sonner
export { Toaster } from './components/ui/sonner';

// Editor component exports (legacy - use input/MarkdownEditor instead)
export { MarkdownEditor as LegacyMarkdownEditor } from './components/editor';
export type { MarkdownEditorProps as LegacyMarkdownEditorProps } from './components/editor';

// Input component exports
export { 
  // Main integrated components
  InputArea,
  // Individual components
  InputContainer, 
  RichTextEditor,
  InputToolbar,
  MicrophoneButton
} from './components/input';

export type { 
  InputAreaProps,
  InputContainerProps,
  RichTextEditorProps,
  InputToolbarProps,
  MicrophoneButtonProps,
  Agent,
  OutputMode,
  OutputOption
} from './components/input';

// Control component exports - Include the new selectors here
export {
  ConnectionButton,
  ConnectionStatus,
  ConnectionIndicator,
  AudioControls,
  AgentSelector,
  OutputSelector,
  ThemeSwitcher
} from './components/controls';

export type {
  ConnectionButtonProps,
  ConnectionStatusProps,
  AudioControlsProps,
  AgentSelectorProps,
  OutputSelectorProps,
  ThemeSwitcherProps
} from './components/controls';

// Session component exports
export {
  ChatSessionList,
  SessionNameDropdown
} from './components/session';

export type {
  ChatSessionListProps,
  SessionNameDropdownProps
} from './components/session';

// Sidebar component exports
export {
  ChatSidebar,
  SidebarTopMenu,
  UserDisplay
} from './components/sidebar';

export type {
  ChatSidebarProps,
  SidebarTopMenuProps,
  UserDisplayProps
} from './components/sidebar';

// Chat component exports
export {
  Message,
  MessageFooter,
  MessageList,
  SystemNotification,
  SystemNotificationContainer,
  TypingIndicator,
  ScrollAnchor,
  ChatMessagesView,
  typingIndicatorStyles,
  // File upload components
  FileAttachmentItem,
  FileAttachmentList,
  UploadProgressIndicator,
  ImageLightbox
  // ChatInputArea removed - use InputArea from input/ which now has file upload
  // DropOverlay removed - InputArea uses react-dropzone directly
} from './components/chat';

export type {
  MessageProps,
  MessageData,
  MessageFooterProps,
  MessageListProps,
  SystemNotificationProps,
  SystemNotificationContainerProps,
  SystemNotificationData,
  NotificationSeverity,
  TypingIndicatorProps,
  ScrollAnchorProps,
  ChatMessagesViewProps,
  // File upload component types
  FileAttachmentItemProps,
  FileAttachmentListProps,
  UploadProgressIndicatorProps,
  ImageLightboxProps
  // ChatInputAreaProps removed - use InputAreaProps from input/
  // FileDropZoneProps removed - not needed with react-dropzone
} from './components/chat';

// Audio component exports
export {
  VoiceVisualizerView
} from './components/audio';

export type {
  VoiceVisualizerViewProps
} from './components/audio';

// Layout component exports
export {
  MainContentArea
} from './components/layout';

export type {
  MainContentAreaProps,
  OutputMode as ContentOutputMode
} from './components/layout';

// Avatar component exports
export {
  AvatarDisplayView
} from './components/avatar';

export type {
  AvatarDisplayViewProps
} from './components/avatar';