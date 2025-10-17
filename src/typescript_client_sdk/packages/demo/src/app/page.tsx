"use client"

import * as React from "react"
import { useRouter } from "next/navigation"
import { isAuthenticated } from "@/lib/auth"

export default function Home() {
  const router = useRouter()

  React.useEffect(() => {
    // Check authentication status and redirect accordingly
    if (isAuthenticated()) {
      // User is authenticated, redirect to chat
      router.push("/chat")
    } else {
      // User is not authenticated, redirect to login
      router.push("/login")
    }
  }, [router])

  // Show loading state while checking authentication
  return (
    <main className="flex h-screen items-center justify-center">
      <div className="text-muted-foreground">
        Loading...
      </div>
    </main>
  )
}
