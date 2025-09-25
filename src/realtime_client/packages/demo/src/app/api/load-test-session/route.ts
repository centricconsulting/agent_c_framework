import { NextRequest, NextResponse } from 'next/server';
import fs from 'fs';
import path from 'path';

/**
 * API endpoint to load the test session data
 * GET /api/load-test-session
 */
export async function GET(request: NextRequest) {
  try {
    // Path to the test session file
    const filePath = path.join(
      process.cwd(), 
      '..',
      '..',
      '.scratch',
      'chat_fixes',
      'session_with_delegation.json'
    );

    // Check if file exists
    if (!fs.existsSync(filePath)) {
      console.error('[load-test-session] File not found:', filePath);
      return NextResponse.json(
        { error: 'Test session file not found' },
        { status: 404 }
      );
    }

    // Read and parse the file
    const fileContent = fs.readFileSync(filePath, 'utf-8');
    const sessionData = JSON.parse(fileContent);

    console.log('[load-test-session] Loaded session:', {
      sessionId: sessionData.session_id,
      messageCount: sessionData.messages?.length || 0
    });

    return NextResponse.json(sessionData);
  } catch (error) {
    console.error('[load-test-session] Error loading test session:', error);
    
    return NextResponse.json(
      { 
        error: 'Failed to load test session',
        details: error instanceof Error ? error.message : 'Unknown error'
      },
      { status: 500 }
    );
  }
}