import { useNavigate } from 'react-router-dom'
import { AgentCProvider } from '@agentc/realtime-react'
import { ChatLayout } from '@agentc/realtime-ui'
import { getToken, getWebSocketUrl, logout } from '../lib/auth'

export function ChatPage() {
  const navigate = useNavigate()
  const token = getToken()
  const wsUrl = getWebSocketUrl()

  function handleLogout() {
    logout()
    navigate('/login')
  }

  if (!token) {
    navigate('/login')
    return null
  }

  return (
    <AgentCProvider
      wsUrl={wsUrl}
      authToken={token}
      autoConnect={true}
      debug={true}
    >
      <ChatLayout onLogout={handleLogout} />
    </AgentCProvider>
  )
}
