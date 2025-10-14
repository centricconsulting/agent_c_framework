import os
import glob
import json
import asyncio
from datetime import datetime
from typing import List, Dict, Any, Optional
from fastapi import BackgroundTasks
from agent_c_api.api.v1.interactions.interaction_models.event_model import Event, EventType
from agent_c_api.api.v1.interactions.utils.file_utils import read_jsonl_file, get_session_directory

class EventService:
    def __init__(self):
        self.active_replays = {}  # session_id -> replay state

    async def get_events(
            self,
            session_id: str,
            event_types: Optional[List[EventType]],
            start_time: Optional[str],
            end_time: Optional[str],
            limit: int
    ) -> List[Event]:
        """
        Get events for a specific session with filtering.
        """
        sessions_dir = get_session_directory()
        session_dir = os.path.join(sessions_dir, session_id)

        if not os.path.isdir(session_dir):
            return []

        # Find all JSONL files for this session
        jsonl_files = glob.glob(os.path.join(session_dir, "*.jsonl"))
        if not jsonl_files:
            return []

        # Parse datetime filters if provided
        start_datetime = None
        if start_time:
            start_datetime = datetime.fromisoformat(start_time.replace("Z", "+00:00"))

        end_datetime = None
        if end_time:
            end_datetime = datetime.fromisoformat(end_time.replace("Z", "+00:00"))

        # Collect and filter events
        all_events = []
        event_count = 0

        for file_path in jsonl_files:
            file_events = await read_jsonl_file(file_path)

            for event_data in file_events:
                # Skip if we've reached the limit
                if event_count >= limit:
                    break

                # Extract event type and timestamp
                event_type_str = event_data.get("event", {}).get("type")
                if not event_type_str:
                    continue

                # Filter by event type if specified
                if event_types and EventType(event_type_str) not in event_types:
                    continue

                # Parse timestamp
                timestamp_str = event_data.get("timestamp")
                if not timestamp_str:
                    continue

                timestamp = datetime.fromisoformat(timestamp_str.replace("Z", "+00:00"))

                # Filter by time range if specified
                if start_datetime and timestamp < start_datetime:
                    continue

                if end_datetime and timestamp > end_datetime:
                    continue

                # Create Event object
                event_obj = self._create_event_object(event_data)
                if event_obj:
                    all_events.append(event_obj)
                    event_count += 1

                # Check if we've reached the limit
                if event_count >= limit:
                    break

            # Check if we've reached the limit
            if event_count >= limit:
                break

        # Sort events by timestamp
        all_events.sort(key=lambda x: x.timestamp)
        return all_events

    async def stream_events(
            self,
            session_id: str,
            event_types: Optional[List[EventType]],
            real_time: bool,
            speed_factor: float
    ):
        """
        Stream events for a session, optionally with real-time timing.
        """
        # Get all events first
        all_events = await self.get_events(session_id, event_types, None, None, 10000)

        if not all_events:
            yield json.dumps({"error": "No events found"})
            return

        # Initialize replay state
        self.active_replays[session_id] = {
            "status": "playing",
            "current_index": 0,
            "total_events": len(all_events),
            "real_time": real_time,
            "speed_factor": speed_factor
        }

        # Stream events
        prev_timestamp = None

        for i, event in enumerate(all_events):
            # Check if replay was stopped or paused
            replay_state = self.active_replays.get(session_id, {})
            if replay_state.get("status") == "stopped":
                break

            if replay_state.get("status") == "paused":
                while replay_state.get("status") == "paused":
                    await asyncio.sleep(0.1)
                    replay_state = self.active_replays.get(session_id, {})
                    if replay_state.get("status") == "stopped":
                        break

            # Update current index
            self.active_replays[session_id]["current_index"] = i

            # Calculate delay if in real-time mode
            if real_time and prev_timestamp:
                time_diff = (event.timestamp - prev_timestamp).total_seconds()
                adjusted_delay = time_diff / speed_factor
                if adjusted_delay > 0:
                    await asyncio.sleep(adjusted_delay)

            # Send event
            yield f"data: {json.dumps(event.model_dump(mode='json'))}\n\n"

            prev_timestamp = event.timestamp

        # Cleanup when done
        if session_id in self.active_replays:
            self.active_replays[session_id]["status"] = "completed"
            
        # Send a properly formatted end message to prevent parsing errors
        yield f"data: {json.dumps({"type": "stream_complete", "message": "Event stream complete"})}\n\n"

    def get_replay_status(self, session_id: str) -> Optional[Dict[str, Any]]:
        """
        Get the current status of a session replay.
        """
        return self.active_replays.get(session_id)

    async def control_replay(
            self,
            session_id: str,
            action: str,
            position: Optional[str],
            background_tasks: Optional[BackgroundTasks]
    ) -> bool:
        """
        Control a session replay (play, pause, stop, seek).
        """
        if session_id not in self.active_replays:
            return False

        if action == "play":
            self.active_replays[session_id]["status"] = "playing"
            return True

        elif action == "pause":
            self.active_replays[session_id]["status"] = "paused"
            return True

        elif action == "stop":
            self.active_replays[session_id]["status"] = "stopped"
            return True

        elif action == "seek" and position:
            # Seek to a specific timestamp
            try:
                target_timestamp = datetime.fromisoformat(position.replace("Z", "+00:00"))

                # Get all events
                events = await self.get_events(session_id, None, None, None, 10000)

                # Find the closest event
                for i, event in enumerate(events):
                    if event.timestamp >= target_timestamp:
                        self.active_replays[session_id]["current_index"] = i
                        return True

                return False
            except:
                return False

        return False

    def _create_event_object(self, event_data: Dict[str, Any]) -> Optional[Event]:
        """
        Create an Event object from raw event data.
        """
        try:
            timestamp_str = event_data.get("timestamp")
            event_dict = event_data.get("event", {})

            if not timestamp_str or not event_dict:
                return None

            event_type_str = event_dict.get("type")
            if not event_type_str:
                return None

            # Create Event object
            return Event(
                timestamp=datetime.fromisoformat(timestamp_str.replace("Z", "+00:00")),
                type=EventType(event_type_str),
                session_id=event_dict.get("session_id"),
                role=event_dict.get("role"),
                content=event_dict.get("content"),
                format=event_dict.get("format"),
                running=event_dict.get("running"),
                active=event_dict.get("active"),
                vendor=event_dict.get("vendor"),
                tool_calls=event_dict.get("tool_calls"),
                tool_results=event_dict.get("tool_results"),
                raw=event_data
            )
        except Exception as e:
            print(f"Error creating event object: {e}")
            return None
