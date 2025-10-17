import { NextRequest, NextResponse } from 'next/server';
import { env } from '@/env.mjs';
import { Logger } from '@/utils/logger';

// Force dynamic rendering for this route since it uses request headers
export const dynamic = 'force-dynamic';

/**
 * JWT payload structure
 */
interface JWTPayload {
  sub: string;
  exp: number;
  iat: number;
  [key: string]: any;
}

/**
 * Session response with WebSocket connection info
 */
interface SessionResponse {
  websocketUrl: string;
  user: {
    id: string;
    [key: string]: any;
  };
  expiresAt: number;
}

/**
 * Parse JWT token without verification (for basic validation)
 * Server should verify properly with the secret
 */
function parseJWT(token: string): JWTPayload | null {
  try {
    const parts = token.split('.');
    if (parts.length !== 3) {
      return null;
    }
    
    const payload = parts[1];
    // Handle URL-safe base64
    const base64 = payload.replace(/-/g, '+').replace(/_/g, '/');
    // Add padding if needed
    const padded = base64 + '='.repeat((4 - base64.length % 4) % 4);
    const decoded = Buffer.from(padded, 'base64').toString('utf-8');
    return JSON.parse(decoded);
  } catch (error) {
    Logger.error('Failed to parse JWT:', error);
    return null;
  }
}

/**
 * GET /api/auth/session
 * Returns WebSocket connection information for authenticated users
 */
export async function GET(request: NextRequest) {
  try {
    // Extract the Bearer token from the Authorization header
    const authHeader = request.headers.get('Authorization');
    
    if (!authHeader || !authHeader.startsWith('Bearer ')) {
      return NextResponse.json(
        { error: 'Authorization header with Bearer token required' },
        { status: 401 }
      );
    }

    const token = authHeader.substring(7); // Remove "Bearer " prefix

    // Parse the JWT to extract user information
    const payload = parseJWT(token);
    
    if (!payload) {
      return NextResponse.json(
        { error: 'Invalid token format' },
        { status: 401 }
      );
    }

    // Check if token is expired
    const currentTime = Math.floor(Date.now() / 1000);
    if (payload.exp && payload.exp < currentTime) {
      return NextResponse.json(
        { error: 'Token has expired' },
        { status: 401 }
      );
    }

    // Get the Agent C API URL from server-side environment variable
    const agentCApiUrl = env.AGENT_C_API_URL || process.env.AGENT_C_API_URL;
    
    if (!agentCApiUrl) {
      Logger.error('AGENT_C_API_URL is not configured');
      return NextResponse.json(
        { error: 'Server configuration error' },
        { status: 500 }
      );
    }

    // Construct the WebSocket URL from the API URL
    // Convert https:// to wss:// or http:// to ws://
    let websocketUrl = agentCApiUrl
      .replace('https://', 'wss://')
      .replace('http://', 'ws://');
    
    // Add the WebSocket endpoint path
    websocketUrl = `${websocketUrl}/rt/ws`;

    // Build the session response
    const sessionResponse: SessionResponse = {
      websocketUrl,
      user: {
        id: payload.sub,
        ...payload,
      },
      expiresAt: payload.exp,
    };

    // Optionally, we could verify the token with the Agent C API here
    // For now, we trust the JWT if it's not expired and properly formatted

    return NextResponse.json(sessionResponse, { status: 200 });

  } catch (error) {
    Logger.error('Session endpoint error:', error);
    
    return NextResponse.json(
      { error: 'Failed to retrieve session information' },
      { status: 500 }
    );
  }
}