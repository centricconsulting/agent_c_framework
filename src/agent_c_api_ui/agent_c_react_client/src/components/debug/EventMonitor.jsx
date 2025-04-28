import React, { useState, useEffect, useRef } from 'react';
import eventBus from '@/lib/eventBus';
import EVENTS from '@/lib/events';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Badge } from '@/components/ui/badge';
import { Card, CardContent, CardHeader, CardTitle, CardDescription, CardFooter } from '@/components/ui/card';
import { ScrollArea } from '@/components/ui/scroll-area';

/**
 * EventMonitor provides a debug UI for monitoring events dispatched through the event bus.
 * It allows filtering, clearing, and inspecting event details in real-time.
 * 
 * This component is only available in development mode.
 */
const EventMonitor = ({ maxEvents = 50 }) => {
  // State for captured events
  const [events, setEvents] = useState([]);
  const [filter, setFilter] = useState('');
  const [isPaused, setIsPaused] = useState(false);
  const [isMinimized, setIsMinimized] = useState(false);
  const [filterByCategory, setFilterByCategory] = useState('all');
  
  // Ref to track if we're in development mode
  const isDev = process.env.NODE_ENV === 'development';
  
  // Auto-scroll to bottom when new events arrive
  const scrollAreaRef = useRef(null);
  
  // Helper to categorize events
  const getEventCategory = (eventType) => {
    if (eventType.startsWith('auth.')) return 'auth';
    if (eventType.startsWith('model.')) return 'model';
    if (eventType.startsWith('session.')) return 'session';
    if (eventType.startsWith('init.')) return 'init';
    if (eventType.startsWith('storage.')) return 'storage';
    if (eventType.startsWith('api.')) return 'api';
    if (eventType.startsWith('ui.')) return 'ui';
    return 'other';
  };
  
  // Setup event capture
  useEffect(() => {
    if (!isDev) return; // Only work in development
    
    // Function to handle new events
    const handleEvent = (eventType, data, meta) => {
      if (isPaused) return; // Don't capture events when paused
      
      const category = getEventCategory(eventType);
      const timestamp = new Date();
      const formattedTime = timestamp.toLocaleTimeString() + '.' + 
                           timestamp.getMilliseconds().toString().padStart(3, '0');
      
      setEvents(prevEvents => {
        // Add the new event
        const newEvents = [
          ...prevEvents, 
          { 
            id: Date.now() + Math.random(), 
            type: eventType, 
            data, 
            meta, 
            timestamp,
            formattedTime,
            category
          }
        ];
        
        // Keep only maxEvents most recent
        return newEvents.slice(-maxEvents);
      });
    };
    
    // Subscribe to all event types
    const unsubscribers = [];
    
    // Get all event types from the EVENTS object
    const allEventTypes = Object.values(EVENTS);
    
    // Subscribe to each event type
    allEventTypes.forEach(eventType => {
      const unsubscribe = eventBus.subscribe(
        eventType,
        (data, meta) => handleEvent(eventType, data, meta),
        { componentName: 'EventMonitor' }
      );
      unsubscribers.push(unsubscribe);
    });
    
    // Also subscribe to event history
    const existingEvents = eventBus.getEventHistory();
    if (existingEvents.length > 0) {
      setEvents(existingEvents.map(e => ({
        id: Date.now() + Math.random(),
        type: e.eventType,
        data: e.data,
        meta: { publisherName: e.publisherName, publishTime: e.timestamp },
        timestamp: new Date(e.timestamp),
        formattedTime: new Date(e.timestamp).toLocaleTimeString(),
        category: getEventCategory(e.eventType)
      })));
    }
    
    // Cleanup on unmount
    return () => {
      unsubscribers.forEach(unsubscribe => unsubscribe());
    };
  }, [isPaused, maxEvents, isDev]);
  
  // Auto-scroll to bottom when new events arrive
  useEffect(() => {
    if (scrollAreaRef.current && !isPaused) {
      const scrollArea = scrollAreaRef.current;
      scrollArea.scrollTo({ top: scrollArea.scrollHeight, behavior: 'smooth' });
    }
  }, [events, isPaused]);
  
  // If not in development mode, don't render the component
  if (!isDev) {
    return null;
  }
  
  // Filter events based on search and category
  const filteredEvents = events.filter(event => {
    const matchesFilter = filter ? 
      event.type.toLowerCase().includes(filter.toLowerCase()) ||
      event.category.toLowerCase().includes(filter.toLowerCase()) ||
      JSON.stringify(event.data).toLowerCase().includes(filter.toLowerCase()) :
      true;
      
    const matchesCategory = filterByCategory === 'all' || event.category === filterByCategory;
    
    return matchesFilter && matchesCategory;
  });
  
  // If minimized, show only a small button to expand
  if (isMinimized) {
    return (
      <div className="fixed bottom-4 right-4 z-50">
        <Button 
          variant="secondary" 
          size="sm"
          onClick={() => setIsMinimized(false)}
          className="flex items-center gap-2"
        >
          <span className="w-2 h-2 rounded-full bg-green-500"></span>
          Events ({events.length})
        </Button>
      </div>
    );
  }
  
  // Event category options
  const categories = [
    { label: 'All', value: 'all' },
    { label: 'Auth', value: 'auth' },
    { label: 'Model', value: 'model' },
    { label: 'Session', value: 'session' },
    { label: 'Init', value: 'init' },
    { label: 'Storage', value: 'storage' },
    { label: 'API', value: 'api' },
    { label: 'UI', value: 'ui' },
    { label: 'Other', value: 'other' }
  ];
  
  // Color mapping for categories
  const categoryColors = {
    auth: 'bg-blue-500',
    model: 'bg-purple-500',
    session: 'bg-green-500',
    init: 'bg-amber-500',
    storage: 'bg-teal-500',
    api: 'bg-red-500',
    ui: 'bg-indigo-500',
    other: 'bg-gray-500'
  };
  
  return (
    <div className="fixed bottom-4 right-4 z-50 w-5/6 md:w-1/2 lg:w-1/3 xl:w-1/4 max-h-96 shadow-lg">
      <Card>
        <CardHeader className="pb-2">
          <div className="flex justify-between items-center">
            <CardTitle className="text-lg">Event Monitor</CardTitle>
            <div className="flex gap-2">
              <Button variant="ghost" size="sm" onClick={() => setIsMinimized(true)}>
                Minimize
              </Button>
            </div>
          </div>
          <CardDescription>
            <div className="flex space-x-2 mb-2">
              <Input 
                placeholder="Filter events..." 
                value={filter} 
                onChange={(e) => setFilter(e.target.value)}
                className="text-sm"
              />
              <Button 
                variant={isPaused ? "destructive" : "outline"}
                size="sm"
                onClick={() => setIsPaused(!isPaused)}
              >
                {isPaused ? "Resume" : "Pause"}
              </Button>
              <Button 
                variant="outline" 
                size="sm"
                onClick={() => setEvents([])}
              >
                Clear
              </Button>
            </div>
            <div className="flex flex-wrap gap-1">
              {categories.map(cat => (
                <Badge 
                  key={cat.value} 
                  variant={filterByCategory === cat.value ? "default" : "outline"}
                  className="cursor-pointer"
                  onClick={() => setFilterByCategory(cat.value)}
                >
                  {cat.label}
                </Badge>
              ))}
            </div>
          </CardDescription>
        </CardHeader>
        <CardContent className="px-2 pb-0">
          <ScrollArea className="h-60 pr-4" ref={scrollAreaRef}>
            {filteredEvents.length === 0 ? (
              <div className="text-center text-muted-foreground py-8">
                No matching events
              </div>
            ) : (
              <div className="space-y-2">
                {filteredEvents.map(event => (
                  <div 
                    key={event.id} 
                    className="border p-2 rounded-md text-xs hover:bg-muted transition-colors"
                  >
                    <div className="flex justify-between items-start">
                      <div className="font-mono break-all font-semibold max-w-[calc(100%-80px)] truncate">
                        {event.type}
                      </div>
                      <div className="text-muted-foreground text-xs">
                        {event.formattedTime}
                      </div>
                    </div>
                    <div className="flex gap-1 mt-1">
                      <Badge 
                        variant="secondary"
                        className={`text-xs ${categoryColors[event.category]} bg-opacity-20`}
                      >
                        {event.category}
                      </Badge>
                      {event.meta?.publisherName && (
                        <Badge variant="outline" className="text-xs">
                          {event.meta.publisherName}
                        </Badge>
                      )}
                    </div>
                    <div className="mt-1 bg-muted p-1 rounded overflow-auto max-h-20 font-mono text-[10px]">
                      {JSON.stringify(event.data, null, 2)}
                    </div>
                  </div>
                ))}
              </div>
            )}
          </ScrollArea>
        </CardContent>
        <CardFooter className="pt-2 pb-2 text-xs text-muted-foreground">
          {events.length} events captured • {filteredEvents.length} displayed
        </CardFooter>
      </Card>
    </div>
  );
};

export default EventMonitor;