// src/utils/logger.ts
export enum LogLevel {
  ERROR = 0,
  WARN = 1,
  INFO = 2,
  DEBUG = 3,
  TRACE = 4
}

// Declare global to avoid TypeScript errors
declare const process: any;

export class Logger {
  private static level: LogLevel = 
    (typeof process !== 'undefined' && process?.env?.NODE_ENV === 'test') ? LogLevel.ERROR :
    (typeof process !== 'undefined' && process?.env?.NODE_ENV === 'development') ? LogLevel.DEBUG :
    LogLevel.WARN;

  static setLevel(level: LogLevel) {
    this.level = level;
  }

  static error(message: string, ...args: any[]) {
    if (this.level >= LogLevel.ERROR) {
      console.error(`[ERROR] ${message}`, ...args);
    }
  }

  static warn(message: string, ...args: any[]) {
    if (this.level >= LogLevel.WARN) {
      console.warn(`[WARN] ${message}`, ...args);
    }
  }

  static info(message: string, ...args: any[]) {
    if (this.level >= LogLevel.INFO) {
      console.info(`[INFO] ${message}`, ...args);
    }
  }

  static debug(message: string, ...args: any[]) {
    if (this.level >= LogLevel.DEBUG) {
      console.debug(`[DEBUG] ${message}`, ...args);
    }
  }

  static trace(message: string, ...args: any[]) {
    if (this.level >= LogLevel.TRACE) {
      console.trace(`[TRACE] ${message}`, ...args);
    }
  }
}