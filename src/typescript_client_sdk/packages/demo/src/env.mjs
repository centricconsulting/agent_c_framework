import { createEnv } from "@t3-oss/env-nextjs"
import { z } from "zod"

export const env = createEnv({
  server: {
    AGENT_C_API_URL: z.string().url().optional(),
  },
  client: {
    NEXT_PUBLIC_APP_URL: z.string().min(1).optional().default('http://localhost:3000'),
    NEXT_PUBLIC_AGENTC_API_URL: z.string().optional(),
  },
  runtimeEnv: {
    AGENT_C_API_URL: process.env.AGENT_C_API_URL,
    NEXT_PUBLIC_APP_URL: process.env.NEXT_PUBLIC_APP_URL || 'http://localhost:3000',
    NEXT_PUBLIC_AGENTC_API_URL: process.env.NEXT_PUBLIC_AGENTC_API_URL,
  },
  skipValidation: !!process.env.SKIP_ENV_VALIDATION,
})
