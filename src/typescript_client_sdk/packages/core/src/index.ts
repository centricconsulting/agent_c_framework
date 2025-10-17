/**
 * @agentc/realtime-core
 * Core SDK for Agent C Realtime API
 * 
 * This is the main entry point for the core SDK package.
 * It exports all public APIs for framework-agnostic usage.
 */

// Re-export main client
export * from './client';

// Re-export event types and utilities
export * from './events';

// Export authentication module
export * from './auth';

// Export audio module
export * from './audio';
export * from './session';
export * from './voice';
export * from './avatar';
export * from './types';
export * from './utils';

// Package version
export const VERSION = '0.1.0';