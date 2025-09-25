/**
 * Session Switch Test Page
 * 
 * Dedicated page for testing session switching functionality.
 * Validates that messages clear properly when switching between sessions.
 */

import { Metadata } from 'next';
import SessionSwitchTestClient from './client';

export const metadata: Metadata = {
  title: 'Session Switch Testing',
  description: 'Test and validate session switching behavior'
};

export default function SessionSwitchTestPage() {
  return <SessionSwitchTestClient />;
}