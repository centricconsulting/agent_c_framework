// Type check file to verify components compile
import { ChatLayout } from './components/layout/ChatLayout'
import { ChatHeader } from './components/layout/ChatHeader'
// Content components moved to @agentc/realtime-ui
// import { MainContentArea, ChatMessagesView, AvatarDisplayView } from '@agentc/realtime-ui'
import { VoiceVisualizerView } from '@agentc/realtime-ui'

// These components are now imported from the UI package
import {
  ChatSidebar,
  SessionNameDropdown,
  SidebarTopMenu,
  ChatSessionList,
  UserDisplay
} from '@agentc/realtime-ui'

// This file helps verify all components can be imported and typed correctly
export {
  ChatLayout,
  ChatSidebar,
  ChatHeader,
  // MainContentArea, // Moved to @agentc/realtime-ui
  // ChatMessagesView, // Moved to @agentc/realtime-ui
  // AvatarDisplayView, // Moved to @agentc/realtime-ui
  VoiceVisualizerView,
  SessionNameDropdown,
  SidebarTopMenu,
  ChatSessionList,
  UserDisplay
}