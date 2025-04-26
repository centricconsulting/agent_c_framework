/**
 * DebugPanel component
 * A togglable UI panel for displaying logs and current application state
 */

import { useState, useEffect } from 'react';
import logger from '../../lib/logger';
import { useRenderLogger } from '../../hooks/use-logger';
import { Sheet, SheetContent, SheetHeader, SheetTitle, SheetTrigger } from './sheet';
import { Tabs, TabsContent, TabsList, TabsTrigger } from './tabs';
import { ScrollArea } from './scroll-area';

export const LogEntry = ({ entry }) => {
  const { level, timestamp, component, message, data } = entry;
  const time = new Date(timestamp).toLocaleTimeString();
  
  // Style based on log level
  const getLevelStyle = () => {
    switch (level) {
      case 'error': return 'text-red-500 font-bold';
      case 'warn': return 'text-yellow-500 font-semibold';
      case 'info': return 'text-blue-500';
      case 'debug': return 'text-gray-500';
      case 'trace': return 'text-gray-400 text-sm';
      default: return '';
    }
  };
  
  return (
    <div className={`p-2 border-b border-gray-200 ${getLevelStyle()}`}>
      <div className="flex justify-between items-start">
        <span className="font-mono text-xs">{time}</span>
        <span className="font-semibold">{component}</span>
      </div>
      <div className="mt-1">{message}</div>
      {data && (
        <details className="mt-1">
          <summary className="cursor-pointer text-xs">Data</summary>
          <pre className="text-xs mt-1 bg-gray-100 p-2 rounded">
            {JSON.stringify(data, null, 2)}
          </pre>
        </details>
      )}
    </div>
  );
};

const DebugPanel = () => {
  const [logs, setLogs] = useState(logger.getLogHistory());
  const [isOpen, setIsOpen] = useState(false);
  
  useRenderLogger('DebugPanel', { logsLength: logs.length, isOpen });
  
  useEffect(() => {
    // Subscribe to new logs
    const updateLogs = () => {
      // Use a callback form of setState to avoid dependency on current logs state
      setLogs(logger.getLogHistory());
    };
    
    const unsubscribe = logger.onNewLog(updateLogs);
    return unsubscribe;
  }, []);
  
  const clearLogs = () => {
    logger.clearLogHistory();
    setLogs([]);
  };
  
  const filteredLogs = (level) => {
    if (!level || level === 'all') return logs;
    return logs.filter(log => log.level === level);
  };
  
  return (
    <Sheet open={isOpen} onOpenChange={setIsOpen}>
      <SheetTrigger className="fixed bottom-4 right-4 z-50 bg-gray-800 text-white p-2 rounded-full shadow-lg hover:bg-gray-700">
        <span role="img" aria-label="Debug" className="text-xs">🐞</span>
      </SheetTrigger>
      <SheetContent side="right" className="w-[400px] sm:w-[540px] p-0">
        <SheetHeader className="p-4 border-b">
          <div className="flex justify-between items-center">
            <SheetTitle>Debug Panel</SheetTitle>
            <div className="space-x-2">
              <button 
                onClick={clearLogs}
                className="px-2 py-1 text-xs bg-gray-200 hover:bg-gray-300 rounded"
              >
                Clear Logs
              </button>
              <button 
                onClick={() => setIsOpen(false)}
                className="px-2 py-1 text-xs bg-red-100 hover:bg-red-200 rounded"
              >
                Close
              </button>
            </div>
          </div>
        </SheetHeader>
        
        <Tabs defaultValue="all" className="w-full">
          <TabsList className="w-full justify-start px-4 pt-2">
            <TabsTrigger value="all">All</TabsTrigger>
            <TabsTrigger value="error">Errors</TabsTrigger>
            <TabsTrigger value="warn">Warnings</TabsTrigger>
            <TabsTrigger value="info">Info</TabsTrigger>
            <TabsTrigger value="debug">Debug</TabsTrigger>
          </TabsList>
          
          {['all', 'error', 'warn', 'info', 'debug'].map(level => (
            <TabsContent key={level} value={level} className="p-0">
              <ScrollArea className="h-[80vh]">
                <div className="p-4">
                  {filteredLogs(level).length > 0 ? (
                    [...filteredLogs(level)].reverse().map((entry, index) => (
                      <LogEntry key={index} entry={entry} />
                    ))
                  ) : (
                    <div className="text-center text-gray-500 p-4">
                      No logs to display
                    </div>
                  )}
                </div>
              </ScrollArea>
            </TabsContent>
          ))}
        </Tabs>
      </SheetContent>
    </Sheet>
  );
};

export default DebugPanel;