'use client'

import * as React from 'react'
import { ChevronDown, ChevronUp, AlertCircle } from 'lucide-react'
import { cn } from '../../lib/utils'
import { Button } from '../ui/button'
import { Slider } from '../ui/slider'
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '../ui/select'
import { RecordingButton } from './RecordingButton'
import { MuteToggle } from './MuteToggle'
import { useAudio } from '@agentc/realtime-react'

export interface AudioControlsPanelProps extends React.HTMLAttributes<HTMLDivElement> {
  /**
   * Layout orientation
   */
  orientation?: 'horizontal' | 'vertical'
  /**
   * Allow panel to be collapsed
   */
  collapsible?: boolean
  /**
   * Show device selector
   */
  showDeviceSelector?: boolean
  /**
   * Show audio level meter
   */
  showLevelMeter?: boolean
}

export const AudioControlsPanel = React.forwardRef<HTMLDivElement, AudioControlsPanelProps>(
  ({ 
    className,
    orientation = 'horizontal',
    collapsible = false,
    showDeviceSelector = false,
    showLevelMeter = false,
    ...props 
  }, ref) => {
    const { 
      audioLevel, 
      isRecording,
      volume = 50,
      setVolume = () => {},
      isMuted,
      setMuted,
      inputDevice = 'default',
      setInputDevice = () => {}
    } = useAudio()
    
    const [isCollapsed, setIsCollapsed] = React.useState(false)
    const [devices, setDevices] = React.useState<MediaDeviceInfo[]>([])
    const [permissionError, setPermissionError] = React.useState<string | null>(null)
    
    // Detect if on mobile
    const [isMobile, setIsMobile] = React.useState(() => {
      if (typeof window === 'undefined') return false
      return window.innerWidth < 768
    })
    
    // Handle window resize
    React.useEffect(() => {
      const handleResize = () => {
        setIsMobile(window.innerWidth < 768)
      }
      
      window.addEventListener('resize', handleResize)
      
      return () => {
        window.removeEventListener('resize', handleResize)
      }
    }, [])
    
    // Load audio devices
    React.useEffect(() => {
      if (!showDeviceSelector) return
      
      const loadDevices = async () => {
        try {
          const deviceList = await navigator.mediaDevices.enumerateDevices()
          const audioInputs = deviceList.filter(device => device.kind === 'audioinput')
          setDevices(audioInputs)
        } catch (err) {
          setPermissionError('Permission required to access audio devices')
        }
      }
      
      loadDevices()
      
      // Listen for device changes
      navigator.mediaDevices?.addEventListener('devicechange', loadDevices)
      return () => {
        navigator.mediaDevices?.removeEventListener('devicechange', loadDevices)
      }
    }, [showDeviceSelector])
    
    // Handle volume changes
    const handleVolumeChange = React.useCallback((value: number[]) => {
      const newVolume = value[0]
      setVolume(newVolume)
      if (newVolume === 0) {
        setMuted(true)
      } else if (isMuted && newVolume > 0) {
        setMuted(false)
      }
    }, [setVolume, setMuted, isMuted])
    
    // Debounced volume update
    const debouncedVolumeChange = React.useMemo(
      () => {
        let timeoutId: ReturnType<typeof setTimeout>
        return (value: number[]) => {
          clearTimeout(timeoutId)
          timeoutId = setTimeout(() => handleVolumeChange(value), 100)
        }
      },
      [handleVolumeChange]
    )
    
    // Check for audio clipping
    const isClipping = isRecording && audioLevel > 0.95
    const isSilent = isRecording && audioLevel < 0.02
    
    const flexDirection = orientation === 'horizontal' 
      ? (isMobile ? 'flex-col' : 'flex-row')
      : 'flex-col'
    
    if (collapsible && isCollapsed) {
      return (
        <div className={cn('inline-flex items-center gap-2', className)} ref={ref} {...props}>
          <RecordingButton size="small" />
          <Button
            variant="ghost"
            size="icon"
            onClick={() => setIsCollapsed(false)}
            aria-label="Expand audio controls"
          >
            <ChevronDown className="h-4 w-4" />
          </Button>
        </div>
      )
    }
    
    return (
      <div
        ref={ref}
        role="group"
        aria-label="Audio controls"
        className={cn(
          'flex gap-4 p-4 bg-background border rounded-lg',
          flexDirection,
          className
        )}
        aria-live="polite"
        {...props}
      >
        {/* Main Controls */}
        <div className={cn('flex items-center gap-2', isMobile && 'gap-1')}>
          <RecordingButton size={isMobile ? 'small' : 'default'} />
          <MuteToggle size={isMobile ? 'small' : 'default'} />
        </div>
        
        {/* Volume Control */}
        <div className="flex items-center gap-2 flex-1">
          <label 
            htmlFor="volume-slider"
            className={cn('text-sm', isMobile && 'sr-only')}
          >
            Volume
          </label>
          <Slider
            id="volume-slider"
            value={[isMuted ? 0 : volume]}
            onValueChange={debouncedVolumeChange}
            max={100}
            step={1}
            className="flex-1 min-w-[100px]"
            aria-label="Volume"
            aria-valuemin={0}
            aria-valuemax={100}
            aria-valuenow={volume}
            disabled={isMuted}
          />
          <div 
            role="progressbar"
            aria-valuenow={volume}
            aria-valuemin={0}
            aria-valuemax={100}
            className="text-xs text-muted-foreground w-10"
          >
            {volume}%
          </div>
        </div>
        
        {/* Audio Level Meter */}
        {showLevelMeter && (
          <div className="flex items-center gap-2">
            <span className={cn('text-xs', isMobile && 'sr-only')}>Level</span>
            <div 
              role="meter"
              aria-valuenow={Math.round(audioLevel * 100)}
              aria-valuemin={0}
              aria-valuemax={100}
              className="w-24 h-2 bg-muted rounded-full overflow-hidden"
            >
              <div 
                className={cn(
                  'h-full transition-all duration-75',
                  isClipping ? 'bg-destructive' : 'bg-primary'
                )}
                style={{ width: `${audioLevel * 100}%` }}
              />
            </div>
            {isClipping && (
              <div role="alert" className="flex items-center gap-1 text-destructive">
                <AlertCircle className="h-3 w-3" />
                <span className="text-xs">Audio clipping</span>
              </div>
            )}
            {isSilent && (
              <span className="text-xs text-muted-foreground">No audio detected</span>
            )}
          </div>
        )}
        
        {/* Device Selector */}
        {showDeviceSelector && !permissionError && devices.length > 0 && (
          <Select value={inputDevice} onValueChange={setInputDevice}>
            <SelectTrigger 
              className="w-[180px]"
              aria-label="Input device"
            >
              <SelectValue placeholder="Select microphone" />
            </SelectTrigger>
            <SelectContent>
              {devices.map(device => (
                <SelectItem key={device.deviceId} value={device.deviceId}>
                  {device.label || `Microphone ${device.deviceId.slice(0, 5)}`}
                </SelectItem>
              ))}
            </SelectContent>
          </Select>
        )}
        
        {/* Permission Error */}
        {permissionError && (
          <div className="text-xs text-destructive">
            {permissionError}
          </div>
        )}
        
        {/* Collapse Button */}
        {collapsible && (
          <Button
            variant="ghost"
            size="icon"
            onClick={() => setIsCollapsed(true)}
            aria-label="Collapse audio controls"
            className={cn(isMobile && 'h-8 w-8')}
          >
            <ChevronUp className="h-4 w-4" />
          </Button>
        )}
      </div>
    )
  }
)

AudioControlsPanel.displayName = 'AudioControlsPanel'