/**
 * MSW Server Setup for UI Package
 * Configure Mock Service Worker for UI components testing
 */

import { setupServer } from 'msw/node';
import { handlers } from './handlers';

/**
 * Setup MSW server with UI package handlers
 */
export const server = setupServer(...handlers);

/**
 * Export server instance
 */
export default server;