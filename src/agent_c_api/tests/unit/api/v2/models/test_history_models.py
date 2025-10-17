# tests/unit/api/v2/models/test_history_models.py

import pytest
from datetime import datetime, timedelta
# Removed UUID import
import json
from pydantic import ValidationError

from agent_c_api.api.v2.models.history_models import (
    HistorySummary, HistoryDetail, EventFilter,
    PaginationParams, HistoryListResponse,
    StoredEvent, ReplayStatus, ReplayControl
)
from agent_c_api.api.v2.models.chat_models import ChatEventUnion

pytestmark = [
    pytest.mark.unit,
    pytest.mark.models,
    pytest.mark.history
]


@pytest.fixture
def session_id():
    """Generate a session ID for testing using the MnemonicSlugs format"""
    # Using a fixed value for testing, in real code this would be generated
    # from a deterministic source like user email, timestamp, etc.
    return "tiger-castle"


@pytest.fixture
def timestamp_now():
    """Current timestamp for testing"""
    return datetime.now()


@pytest.fixture
def timestamp_later(timestamp_now):
    """Timestamp one hour in the future for testing"""
    return timestamp_now + timedelta(hours=1)


class TestHistorySummary:
    """Tests for the HistorySummary model
    
    HistorySummary contains key information about a session's history 
    including identifiers, timestamps, and basic metrics.
    """
    
    def test_valid_summary(self, session_id, timestamp_now, timestamp_later):
        """Test creating a valid history summary"""
        summary = HistorySummary(
            session_id=session_id,
            name="Test Session",
            created_at=timestamp_now,
            updated_at=timestamp_later,
            message_count=10,
            duration=3600  # 1 hour in seconds
        )
        
        assert summary.session_id == session_id
        assert summary.name == "Test Session"
        assert summary.created_at == timestamp_now
        assert summary.updated_at == timestamp_later
        assert summary.message_count == 10
        assert summary.duration == 3600
    
    def test_missing_required_field(self, session_id, timestamp_now):
        """Test validation when required field is missing"""
        with pytest.raises(ValidationError) as exc_info:
            HistorySummary(
                session_id=session_id,
                # name is missing
                created_at=timestamp_now,
                updated_at=timestamp_now,
                message_count=10,
                duration=3600
            )
        
        error_details = str(exc_info.value)
        assert "name" in error_details
        assert "field required" in error_details.lower()
    
    def test_serialization(self, session_id, timestamp_now, timestamp_later):
        """Test JSON serialization and deserialization"""
        summary = HistorySummary(
            session_id=session_id,
            name="Test Session",
            created_at=timestamp_now,
            updated_at=timestamp_later,
            message_count=10,
            duration=3600
        )
        
        # Serialize to JSON
        json_data = summary.model_dump_json()
        assert isinstance(json_data, str)
        
        # Deserialize from JSON
        data_dict = json.loads(json_data)
        assert data_dict["session_id"] == session_id
        assert data_dict["name"] == "Test Session"
        
        # Recreate from dict
        recreated = HistorySummary(**data_dict)
        assert recreated.session_id == session_id
        assert recreated.name == "Test Session"
        assert recreated.message_count == 10
    
    def test_schema_example(self):
        """Test that schema example is valid according to the model"""
        example = HistorySummary.model_config["json_schema_extra"]["example"]
        # No need to convert session_id from string to UUID now
        # Convert string timestamps to datetime objects
        example["created_at"] = datetime.fromisoformat(example["created_at"].replace("Z", "+00:00"))
        example["updated_at"] = datetime.fromisoformat(example["updated_at"].replace("Z", "+00:00"))
        
        # Should not raise validation errors
        summary = HistorySummary(**example)
        assert summary.name == "Task Planning Session"
        assert summary.message_count == 24
        assert summary.duration == 4500
        assert summary.session_id == "tiger-castle"


class TestHistoryDetail:
    """Tests for the HistoryDetail model
    
    HistoryDetail extends HistorySummary with additional information about
    the session's contents, including file records and event types.
    """
    
    def test_valid_detail(self, session_id, timestamp_now, timestamp_later):
        """Test creating a valid history detail"""
        detail = HistoryDetail(
            session_id=session_id,
            name="Test Session",
            created_at=timestamp_now,
            updated_at=timestamp_later,
            message_count=10,
            duration=3600,  # 1 hour in seconds
            files=["events_123.jsonl"],
            event_types={"message": 5, "tool_call": 3, "thinking": 2},
            metadata={"model": "gpt-4-turbo"},
            user_id="user_123",
            has_thinking=True,
            tool_calls=["web_search", "code_interpreter"]
        )
        
        # Check base fields from HistorySummary
        assert detail.session_id == session_id
        assert detail.name == "Test Session"
        assert detail.message_count == 10
        
        # Check extended fields
        assert detail.files == ["events_123.jsonl"]
        assert detail.event_types == {"message": 5, "tool_call": 3, "thinking": 2}
        assert detail.metadata == {"model": "gpt-4-turbo"}
        assert detail.user_id == "user_123"
        assert detail.has_thinking is True
        assert detail.tool_calls == ["web_search", "code_interpreter"]
    
    def test_inheritance_from_summary(self, session_id, timestamp_now, timestamp_later):
        """Test that HistoryDetail properly inherits from HistorySummary"""
        # Create a base summary
        summary = HistorySummary(
            session_id=session_id,
            name="Test Session",
            created_at=timestamp_now,
            updated_at=timestamp_later,
            message_count=10,
            duration=3600
        )
        
        # Create a detail with the same base fields
        detail = HistoryDetail(
            session_id=session_id,
            name="Test Session",
            created_at=timestamp_now,
            updated_at=timestamp_later,
            message_count=10,
            duration=3600,
            files=["events_123.jsonl"],
            event_types={"message": 5}
        )
        
        # Verify that detail is an instance of both classes
        assert isinstance(detail, HistoryDetail)
        assert isinstance(detail, HistorySummary)
        
        # Verify that base fields match
        for field in summary.model_fields_set:
            assert getattr(summary, field) == getattr(detail, field)
    
    def test_optional_fields(self, session_id, timestamp_now, timestamp_later):
        """Test optional fields can be omitted"""
        detail = HistoryDetail(
            session_id=session_id,
            name="Test Session",
            created_at=timestamp_now,
            updated_at=timestamp_later,
            message_count=10,
            duration=3600,
            files=["events_123.jsonl"],
            event_types={"message": 5}
            # metadata, user_id are optional and omitted
        )
        
        assert detail.metadata is None
        assert detail.user_id is None
        assert detail.has_thinking is False  # Default value
        assert detail.tool_calls == []  # Default value (empty list)
    
    def test_serialization(self, session_id, timestamp_now, timestamp_later):
        """Test JSON serialization and deserialization"""
        detail = HistoryDetail(
            session_id=session_id,
            name="Test Session",
            created_at=timestamp_now,
            updated_at=timestamp_later,
            message_count=10,
            duration=3600,
            files=["events_123.jsonl"],
            event_types={"message": 5, "tool_call": 3},
            metadata={"model": "gpt-4-turbo"},
            has_thinking=True
        )
        
        # Serialize to JSON
        json_data = detail.model_dump_json()
        
        # Deserialize from JSON
        data_dict = json.loads(json_data)
        assert data_dict["session_id"] == session_id
        assert data_dict["files"] == ["events_123.jsonl"]
        assert data_dict["event_types"] == {"message": 5, "tool_call": 3}
        assert data_dict["metadata"] == {"model": "gpt-4-turbo"}
        assert data_dict["has_thinking"] is True
        assert data_dict["tool_calls"] == []  # Default empty list
        
        # Recreate from dict
        recreated = HistoryDetail(**data_dict)
        assert recreated.session_id == session_id
        assert recreated.files == ["events_123.jsonl"]
        assert recreated.event_types == {"message": 5, "tool_call": 3}
    
    def test_schema_example(self):
        """Test that schema example is valid according to the model"""
        example = HistoryDetail.model_config["json_schema_extra"]["example"]
        # No need to convert session_id from string to UUID now
        # Convert string timestamps to datetime objects
        example["created_at"] = datetime.fromisoformat(example["created_at"].replace("Z", "+00:00"))
        example["updated_at"] = datetime.fromisoformat(example["updated_at"].replace("Z", "+00:00"))
        
        # Should not raise validation errors
        detail = HistoryDetail(**example)
        assert detail.name == "Task Planning Session"
        assert detail.message_count == 24
        assert len(detail.files) == 2
        assert detail.event_types["text_delta"] == 120
        assert detail.metadata["model"] == "gpt-4-turbo"
        assert detail.has_thinking is True
        assert "web_search" in detail.tool_calls


class TestEventFilter:
    """Tests for the EventFilter model
    
    EventFilter provides parameters for filtering history events by type,
    time range, and limiting the result set size.
    """
    
    def test_default_values(self):
        """Test default values are set correctly"""
        filter = EventFilter()
        assert filter.event_types is None
        assert filter.start_time is None
        assert filter.end_time is None
        assert filter.limit == 100
    
    def test_custom_values(self, timestamp_now):
        """Test setting custom values"""
        start_time = timestamp_now - timedelta(hours=1)
        end_time = timestamp_now
        
        filter = EventFilter(
            event_types=["message", "tool_call"],
            start_time=start_time,
            end_time=end_time,
            limit=50
        )
        
        assert filter.event_types == ["message", "tool_call"]
        assert filter.start_time == start_time
        assert filter.end_time == end_time
        assert filter.limit == 50
    
    def test_limit_validation(self):
        """Test validation of limit field (min/max values)"""
        # Test limit below minimum
        with pytest.raises(ValidationError) as exc_info:
            EventFilter(limit=0)
        error_details = str(exc_info.value)
        assert "limit" in error_details
        assert "greater than or equal to 1" in error_details.lower() or "minimum value" in error_details.lower()
        
        # Test limit above maximum
        with pytest.raises(ValidationError) as exc_info:
            EventFilter(limit=1001)
        error_details = str(exc_info.value)
        assert "limit" in error_details
        assert "less than or equal to 1000" in error_details.lower() or "maximum value" in error_details.lower()
        
        # Test valid limits
        assert EventFilter(limit=1).limit == 1
        assert EventFilter(limit=1000).limit == 1000
    
    def test_serialization(self, timestamp_now):
        """Test JSON serialization and deserialization"""
        start_time = timestamp_now - timedelta(hours=1)
        end_time = timestamp_now
        
        filter = EventFilter(
            event_types=["message", "tool_call"],
            start_time=start_time,
            end_time=end_time,
            limit=50
        )
        
        # Serialize to JSON
        json_data = filter.model_dump_json()
        
        # Deserialize from JSON
        data_dict = json.loads(json_data)
        assert data_dict["event_types"] == ["message", "tool_call"]
        assert data_dict["limit"] == 50
        
        # Convert string timestamps back to datetime
        data_dict["start_time"] = datetime.fromisoformat(data_dict["start_time"].replace("Z", "+00:00"))
        data_dict["end_time"] = datetime.fromisoformat(data_dict["end_time"].replace("Z", "+00:00"))
        
        # Recreate from dict
        recreated = EventFilter(**data_dict)
        assert recreated.event_types == ["message", "tool_call"]
        assert recreated.start_time is not None
        assert recreated.end_time is not None
        assert recreated.limit == 50
    
    def test_schema_examples(self):
        """Test that schema examples are valid according to the model"""
        examples = EventFilter.model_config["json_schema_extra"]["examples"]
        
        for example in examples:
            # Convert string timestamps to datetime objects if present
            if "start_time" in example and example["start_time"] is not None:
                example["start_time"] = datetime.fromisoformat(example["start_time"].replace("Z", "+00:00"))
            if "end_time" in example and example["end_time"] is not None:
                example["end_time"] = datetime.fromisoformat(example["end_time"].replace("Z", "+00:00"))
            
            # Should not raise validation errors
            filter = EventFilter(**example)
            
            # Basic validation of examples
            if "limit" in example:
                assert filter.limit == example["limit"]
            if "event_types" in example and example["event_types"] is not None:
                assert filter.event_types == example["event_types"]


class TestPaginationParams:
    """Tests for the PaginationParams model
    
    PaginationParams defines common pagination parameters used across
    various list endpoints for controlling result set size and ordering.
    """
    
    def test_default_values(self):
        """Test default values are set correctly"""
        params = PaginationParams()
        assert params.limit == 50
        assert params.offset == 0
        assert params.sort_by == "start_time"
        assert params.sort_order == "desc"
    
    def test_custom_values(self):
        """Test setting custom values"""
        params = PaginationParams(
            limit=20,
            offset=40,
            sort_by="created_at",
            sort_order="asc"
        )
        assert params.limit == 20
        assert params.offset == 40
        assert params.sort_by == "created_at"
        assert params.sort_order == "asc"
    
    def test_limit_validation(self):
        """Test validation of limit field (min/max values)"""
        # Test limit below minimum
        with pytest.raises(ValidationError) as exc_info:
            PaginationParams(limit=0)
        error_details = str(exc_info.value)
        assert "limit" in error_details
        assert "greater than or equal to 1" in error_details.lower() or "minimum value" in error_details.lower()
        
        # Test limit above maximum
        with pytest.raises(ValidationError) as exc_info:
            PaginationParams(limit=101)
        error_details = str(exc_info.value)
        assert "limit" in error_details
        assert "less than or equal to 100" in error_details.lower() or "maximum value" in error_details.lower()
        
        # Test valid limits
        assert PaginationParams(limit=1).limit == 1
        assert PaginationParams(limit=100).limit == 100
    
    def test_offset_validation(self):
        """Test validation of offset field (min value)"""
        # Test offset below minimum
        with pytest.raises(ValidationError) as exc_info:
            PaginationParams(offset=-1)
        error_details = str(exc_info.value)
        assert "offset" in error_details
        assert "greater than or equal to 0" in error_details.lower() or "minimum value" in error_details.lower()
        
        # Test valid offsets
        assert PaginationParams(offset=0).offset == 0
        assert PaginationParams(offset=1000).offset == 1000  # No upper limit
    
    def test_serialization(self):
        """Test JSON serialization and deserialization"""
        params = PaginationParams(
            limit=20,
            offset=40,
            sort_by="created_at",
            sort_order="asc"
        )
        
        # Serialize to JSON
        json_data = params.model_dump_json()
        
        # Deserialize from JSON
        data_dict = json.loads(json_data)
        assert data_dict["limit"] == 20
        assert data_dict["offset"] == 40
        assert data_dict["sort_by"] == "created_at"
        assert data_dict["sort_order"] == "asc"
        
        # Recreate from dict
        recreated = PaginationParams(**data_dict)
        assert recreated.limit == 20
        assert recreated.offset == 40
        assert recreated.sort_by == "created_at"
        assert recreated.sort_order == "asc"
    
    def test_schema_example(self):
        """Test that schema example is valid according to the model"""
        example = PaginationParams.model_config["json_schema_extra"]["example"]
        
        # Should not raise validation errors
        params = PaginationParams(**example)
        assert params.limit == 20
        assert params.offset == 40
        assert params.sort_by == "created_at"
        assert params.sort_order == "desc"


class TestHistoryListResponse:
    """Tests for the HistoryListResponse model
    
    HistoryListResponse provides paginated access to session history summaries
    along with pagination metadata.
    """
    
    def test_valid_response(self):
        """Test creating a valid history list response"""
        now = datetime.now()
        
        summary1 = HistorySummary(
            session_id="tiger-castle",
            name="Test Session 1",
            created_at=now,
            updated_at=now + timedelta(hours=1),
            message_count=10,
            duration=3600
        )
        
        summary2 = HistorySummary(
            session_id="apollo-banana",
            name="Test Session 2",
            created_at=now - timedelta(days=1),
            updated_at=now - timedelta(days=1) + timedelta(hours=2),
            message_count=20,
            duration=7200
        )
        
        response = HistoryListResponse(
            items=[summary1, summary2],
            total=42,
            limit=20,
            offset=0
        )
        
        assert len(response.items) == 2
        assert response.items[0].name == "Test Session 1"
        assert response.items[1].name == "Test Session 2"
        assert response.total == 42
        assert response.limit == 20
        assert response.offset == 0
    
    def test_empty_items_list(self):
        """Test creating a response with an empty items list"""
        response = HistoryListResponse(
            items=[],
            total=0,
            limit=20,
            offset=0
        )
        
        assert response.items == []
        assert response.total == 0
        assert response.limit == 20
        assert response.offset == 0
    
    def test_serialization(self):
        """Test JSON serialization and deserialization"""
        now = datetime.now()
        
        summary = HistorySummary(
            session_id="tiger-castle",
            name="Test Session",
            created_at=now,
            updated_at=now + timedelta(hours=1),
            message_count=10,
            duration=3600
        )
        
        response = HistoryListResponse(
            items=[summary],
            total=42,
            limit=20,
            offset=0
        )
        
        # Serialize to JSON
        json_data = response.model_dump_json()
        
        # Deserialize from JSON
        data_dict = json.loads(json_data)
        assert len(data_dict["items"]) == 1
        assert data_dict["items"][0]["name"] == "Test Session"
        assert data_dict["total"] == 42
        assert data_dict["limit"] == 20
        assert data_dict["offset"] == 0
        
        # Can't easily recreate from the JSON due to the nested datetime conversions
        # This would require special JSON decoding for testing purposes
    
    def test_schema_example(self):
        """Test that schema example is valid according to the model"""
        example = HistoryListResponse.model_config["json_schema_extra"]["example"]
        
        # Convert the nested timestamps in SessionSummary items
        for item in example["items"]:
            # No need to convert session_id from string to UUID anymore
            item["created_at"] = datetime.fromisoformat(item["created_at"].replace("Z", "+00:00"))
            item["updated_at"] = datetime.fromisoformat(item["updated_at"].replace("Z", "+00:00"))
        
        # Should not raise validation errors
        response = HistoryListResponse(**example)
        assert len(response.items) == 2
        assert response.items[0].name == "Task Planning Session"
        assert response.items[1].name == "Code Review Session"
        assert response.total == 42
        assert response.limit == 20
        assert response.offset == 0


class TestStoredEvent:
    """Tests for the StoredEvent model
    
    StoredEvent provides a standardized container for history events with
    consistent metadata, regardless of the specific event type.
    """
    
    # Since we can't easily test with actual ChatEventUnion instances without
    # additional dependencies, we'll use a minimal approach to test the structure
    
    def test_model_structure(self):
        """Test the basic structure of the model"""
        # Check that the model has the expected fields
        assert "id" in StoredEvent.model_fields
        assert "event" in StoredEvent.model_fields
        assert "timestamp" in StoredEvent.model_fields
        
        # Check the field types
        assert StoredEvent.model_fields["id"].annotation == str
        assert StoredEvent.model_fields["event"].annotation == ChatEventUnion
        assert StoredEvent.model_fields["timestamp"].annotation == datetime
    
    def test_schema_examples(self):
        """Test that schema examples have the expected structure"""
        examples = StoredEvent.model_config["json_schema_extra"]["examples"]
        
        # Check that there are examples
        assert len(examples) > 0
        
        # Check the structure of examples
        for example in examples:
            assert "id" in example
            assert "timestamp" in example
            assert "event" in example
            
            # Each event should have an event_type
            assert "event_type" in example["event"]


class TestReplayStatus:
    """Tests for the ReplayStatus model
    
    ReplayStatus provides information about the current state of a session
    replay, including playback status and position information.
    """
    
    def test_valid_status(self, session_id, timestamp_now):
        """Test creating a valid replay status"""
        status = ReplayStatus(
            session_id=session_id,
            is_playing=True,
            current_position=timestamp_now + timedelta(minutes=30),
            start_time=timestamp_now,
            end_time=timestamp_now + timedelta(hours=1)
        )
        
        assert status.session_id == session_id
        assert status.is_playing is True
        assert status.current_position == timestamp_now + timedelta(minutes=30)
        assert status.start_time == timestamp_now
        assert status.end_time == timestamp_now + timedelta(hours=1)
    
    def test_invalid_position(self, session_id, timestamp_now):
        """Test validation when current_position is outside the valid range"""
        # This test checks logical validation rather than schema validation
        # Note: In practice, this validation should be done at the service level
        # since Pydantic doesn't easily handle cross-field comparisons
        start_time = timestamp_now
        end_time = timestamp_now + timedelta(hours=1)
        
        # Position before start_time (logical error, but not a schema error)
        position_too_early = timestamp_now - timedelta(minutes=30)
        early_status = ReplayStatus(
            session_id=session_id,
            is_playing=True,
            current_position=position_too_early,
            start_time=start_time,
            end_time=end_time
        )
        assert early_status.current_position < early_status.start_time
        
        # Position after end_time (logical error, but not a schema error)
        position_too_late = timestamp_now + timedelta(hours=2)
        late_status = ReplayStatus(
            session_id=session_id,
            is_playing=True,
            current_position=position_too_late,
            start_time=start_time,
            end_time=end_time
        )
        assert late_status.current_position > late_status.end_time
    
    def test_serialization(self, session_id, timestamp_now):
        """Test JSON serialization and deserialization"""
        status = ReplayStatus(
            session_id=session_id,
            is_playing=True,
            current_position=timestamp_now + timedelta(minutes=30),
            start_time=timestamp_now,
            end_time=timestamp_now + timedelta(hours=1)
        )
        
        # Serialize to JSON
        json_data = status.model_dump_json()
        
        # Deserialize from JSON
        data_dict = json.loads(json_data)
        assert data_dict["session_id"] == session_id
        assert data_dict["is_playing"] is True
        
        # Convert string timestamps back to datetime for a full round-trip test
        data_dict["current_position"] = datetime.fromisoformat(data_dict["current_position"].replace("Z", "+00:00"))
        data_dict["start_time"] = datetime.fromisoformat(data_dict["start_time"].replace("Z", "+00:00"))
        data_dict["end_time"] = datetime.fromisoformat(data_dict["end_time"].replace("Z", "+00:00"))
        
        # Recreate from dict
        recreated = ReplayStatus(**data_dict)
        assert recreated.session_id == session_id
        assert recreated.is_playing is True
    
    def test_schema_example(self):
        """Test that schema example is valid according to the model"""
        example = ReplayStatus.model_config["json_schema_extra"]["example"]
        # No need to convert session_id from string to UUID anymore
        # Convert string timestamps to datetime objects
        example["current_position"] = datetime.fromisoformat(example["current_position"].replace("Z", "+00:00"))
        example["start_time"] = datetime.fromisoformat(example["start_time"].replace("Z", "+00:00"))
        example["end_time"] = datetime.fromisoformat(example["end_time"].replace("Z", "+00:00"))
        
        # Should not raise validation errors
        status = ReplayStatus(**example)
        assert status.is_playing is True
        assert status.current_position > status.start_time
        assert status.current_position < status.end_time


class TestReplayControl:
    """Tests for the ReplayControl model
    
    ReplayControl defines the available actions and parameters for controlling
    a session replay, supporting actions like play, pause, and seek.
    """
    
    def test_play_action(self):
        """Test creating a valid play control"""
        control = ReplayControl(
            action="play",
            speed=1.5
        )
        assert control.action == "play"
        assert control.speed == 1.5
        assert control.position is None
    
    def test_pause_action(self):
        """Test creating a valid pause control"""
        control = ReplayControl(action="pause")
        assert control.action == "pause"
        assert control.speed is None
        assert control.position is None
    
    def test_seek_action(self, timestamp_now):
        """Test creating a valid seek control"""
        control = ReplayControl(
            action="seek",
            position=timestamp_now
        )
        assert control.action == "seek"
        assert control.position == timestamp_now
        assert control.speed is None
    
    def test_invalid_action(self):
        """Test validation for invalid action"""
        with pytest.raises(ValidationError) as exc_info:
            ReplayControl(action="rewind")
        
        error_details = str(exc_info.value)
        assert "action" in error_details
        assert "not a valid enumeration member" in error_details.lower() or "Input should be 'play', 'pause' or 'seek'" in error_details
    
    def test_speed_validation(self):
        """Test validation of speed field (min/max values)"""
        # Test speed below minimum
        with pytest.raises(ValidationError) as exc_info:
            ReplayControl(action="play", speed=0.05)
        error_details = str(exc_info.value)
        assert "speed" in error_details
        assert "greater than or equal to 0.1" in error_details.lower() or "minimum value" in error_details.lower()
        
        # Test speed above maximum
        with pytest.raises(ValidationError) as exc_info:
            ReplayControl(action="play", speed=11.0)
        error_details = str(exc_info.value)
        assert "speed" in error_details
        assert "less than or equal to 10" in error_details.lower() or "maximum value" in error_details.lower()
        
        # Test valid speeds
        assert ReplayControl(action="play", speed=0.1).speed == 0.1
        assert ReplayControl(action="play", speed=1.0).speed == 1.0
        assert ReplayControl(action="play", speed=10.0).speed == 10.0
    
    def test_serialization(self, timestamp_now):
        """Test JSON serialization and deserialization"""
        control = ReplayControl(
            action="seek",
            position=timestamp_now
        )
        
        # Serialize to JSON
        json_data = control.model_dump_json()
        
        # Deserialize from JSON
        data_dict = json.loads(json_data)
        assert data_dict["action"] == "seek"
        assert "position" in data_dict
        assert data_dict["speed"] is None
        
        # Convert string timestamp back to datetime for a full round-trip test
        data_dict["position"] = datetime.fromisoformat(data_dict["position"].replace("Z", "+00:00"))
        
        # Recreate from dict
        recreated = ReplayControl(**data_dict)
        assert recreated.action == "seek"
        assert recreated.position == timestamp_now
    
    def test_schema_examples(self):
        """Test that schema examples are valid according to the model"""
        examples = ReplayControl.model_config["json_schema_extra"]["examples"]
        
        for example in examples:
            # Convert string timestamp to datetime object if present
            if "position" in example and example["position"] is not None:
                example["position"] = datetime.fromisoformat(example["position"].replace("Z", "+00:00"))
            
            # Should not raise validation errors
            control = ReplayControl(**example)
            assert control.action in ["play", "pause", "seek"]
            
            # Basic validation of examples
            if control.action == "play" and "speed" in example:
                assert control.speed == example["speed"]
            if control.action == "seek" and "position" in example:
                assert control.position is not None


class TestHistoryModelIntegrations:
    """Tests for interactions between history models
    
    These tests verify that the history models work correctly together
    and with related models from other modules.
    """
    
    def test_history_list_with_summaries(self):
        """Test HistoryListResponse with multiple HistorySummary items"""
        now = datetime.now()
        
        # Create multiple summaries with MnemonicSlug-style IDs
        summaries = [
            HistorySummary(
                session_id=f"animal-color-{i}",  # Using a pattern for test IDs
                name=f"Test Session {i}",
                created_at=now - timedelta(days=i),
                updated_at=now - timedelta(days=i) + timedelta(hours=1),
                message_count=10 * i,
                duration=3600 * i
            ) for i in range(1, 4)  # Create 3 summaries
        ]
        
        # Create a response with those summaries
        response = HistoryListResponse(
            items=summaries,
            total=10,
            limit=20,
            offset=0
        )
        
        # Verify the response contains the summaries
        assert len(response.items) == 3
        assert response.items[0].name == "Test Session 1"
        assert response.items[1].name == "Test Session 2"
        assert response.items[2].name == "Test Session 3"
        
        # Verify the IDs use the MnemonicSlugs format
        assert response.items[0].session_id == "animal-color-1"
        assert response.items[1].session_id == "animal-color-2"
        assert response.items[2].session_id == "animal-color-3"
        
        # Verify we can serialize and deserialize the whole response
        json_data = response.model_dump_json()
        # We'll just check that it's valid JSON since full deserialization
        # requires handling nested datetime conversions
        data_dict = json.loads(json_data)
        assert len(data_dict["items"]) == 3
        assert data_dict["items"][0]["session_id"] == "animal-color-1"


class TestMnemonicSlugFormatting:
    """Tests specific to the MnemonicSlug ID format
    
    These tests verify that the models properly handle the MnemonicSlugs ID format,
    which uses readable word combinations instead of UUIDs.
    """
    
    def test_various_id_formats(self):
        """Test different valid ID formats in MnemonicSlugs format"""
        now = datetime.now()
        
        # Test a simple two-word slug
        summary1 = HistorySummary(
            session_id="tiger-castle",
            name="Test Session 1",
            created_at=now,
            updated_at=now,
            message_count=10,
            duration=3600
        )
        assert summary1.session_id == "tiger-castle"
        
        # Test a three-word slug
        summary2 = HistorySummary(
            session_id="apple-banana-cherry",
            name="Test Session 2",
            created_at=now,
            updated_at=now,
            message_count=10,
            duration=3600
        )
        assert summary2.session_id == "apple-banana-cherry"
        
        # Test a hierarchical ID with colons
        summary3 = HistorySummary(
            session_id="tiger-castle:apollo:banana",  # User:Session:Message hierarchy
            name="Test Session 3",
            created_at=now,
            updated_at=now,
            message_count=10,
            duration=3600
        )
        assert summary3.session_id == "tiger-castle:apollo:banana"
    
    def test_serialization_with_mnemonic_ids(self):
        """Test serialization and deserialization with MnemonicSlug IDs"""
        now = datetime.now()
        
        summary = HistorySummary(
            session_id="tiger-castle",
            name="Test Session",
            created_at=now,
            updated_at=now,
            message_count=10,
            duration=3600
        )
        
        # Serialize to JSON
        json_data = summary.model_dump_json()
        
        # Deserialize from JSON
        data_dict = json.loads(json_data)
        assert data_dict["session_id"] == "tiger-castle"
        
        # Round-trip test
        recreated = HistorySummary(**data_dict)
        assert recreated.session_id == "tiger-castle"