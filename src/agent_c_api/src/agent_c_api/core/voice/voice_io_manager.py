import asyncio
import base64

import numpy as np

from asyncio import Task
from typing import TYPE_CHECKING, Optional

from agents.voice import VoicePipeline, StreamedAudioInput, TTSModelSettings, VoicePipelineConfig

from agent_c.util.logging_utils import LoggingManager
from .rt_bridge_workflow import RTBridgeWorkflow


if TYPE_CHECKING:
    from .models import AvailableVoiceModel
    from agent_c_api.core.realtime_bridge import RealtimeBridge
    from agent_c.models.events.chat import AudioInputDeltaEvent


class VoiceIOManager:
    def __init__(self, bridge: "RealtimeBridge", voice: Optional['AvailableVoiceModel'] = None):
        self.logger = LoggingManager(__name__).get_logger()
        self._bridge = bridge
        self._workflow: RTBridgeWorkflow = RTBridgeWorkflow(bridge)
        self._pipeline: Optional[VoicePipeline] = None
        self._pipeline_task: Task | None = None
        self._input: StreamedAudioInput = StreamedAudioInput()
        self._voice: Optional['AvailableVoiceModel'] = None



    @property
    def is_running(self) -> bool:
        """
        Check if the voice pipeline is currently running.

        Returns:
            bool: True if the pipeline is active, False otherwise.
        """
        return self._pipeline_task is not None and not self._pipeline_task.done()

    @property
    def workflow(self) -> RTBridgeWorkflow:
        """
        Get the current workflow instance.

        Returns:
            RTBridgeWorkflow: The workflow used in the voice pipeline.
        """
        return self._workflow

    @property
    def pipeline(self) -> Optional[VoicePipeline]:
        """
        Get the current voice pipeline instance.

        Returns:
            Optional[VoicePipeline]: The active voice pipeline, or None if not started.
        """
        return self._pipeline

    def change_voice(self, voice: 'AvailableVoiceModel'):
        """
        Change the TTS voice model used in the pipeline.

        Args:
            voice (Optional[AvailableVoiceModel]): The new voice model to use. If None, defaults will be used.
        """
        if self._voice and self._voice.voice_id == voice.voice_id:
            return

        if self.is_running:
            self.close()

        if voice.voice_id == "none":
            return

        self._voice = voice
        self.start(voice)


    def start(self, voice: Optional['AvailableVoiceModel'] = None, language: Optional[str] = None):
        """
        Start the voice pipeline to process audio input and generate responses.
        """
        if self._pipeline_task is not None:
            self._pipeline_task.cancel()

        # Disable for now
        return

        voice = voice or self._voice
        tts_voice = voice.voice_id if voice else None

        config = VoicePipelineConfig(workflow_name=f"Voice Agent - {self._bridge.ui_session_id}",
                                     tts_settings=TTSModelSettings(voice=tts_voice))

        self._voice = voice
        self._pipeline = VoicePipeline(workflow=self._workflow, config=config)
        self._pipeline_task = asyncio.create_task(self._run_pipeline())

    def add_audio_delta(self, audio_delta: 'AudioInputDeltaEvent'):
        """
        Add an audio chunk to the input stream for processing.

        Args:
            audio_delta (AudioInputDeltaEvent): The incoming audio data chunk.
        """
        audio_chunk = np.frombuffer(base64.b64decode(audio_delta.content), dtype=np.int16)
        self._input.add_audio(audio_chunk)

    async def add_audio(self, audio_delta: bytes):
        """
        Add an audio chunk to the input stream for processing.

        Args:
            audio_delta (bytes): The incoming audio data chunk.
        """
        audio_chunk = np.frombuffer(audio_delta, dtype=np.int16)
        await self._input.add_audio(audio_chunk)

    def close(self):
        """
        Close the voice pipeline and clean up resources.
        """
        if self._pipeline_task is not None:
            self._pipeline_task.cancel()
            self._pipeline_task = None

        self._pipeline = None

    async def _run_pipeline(self):
        result = await self._pipeline.run(self._input)
        async for event in result.stream():
            if event.type == "voice_stream_event_audio":
                self.logger.debug("Sending audio chunk")
                await self._bridge.websocket.send_bytes(event.data)
            elif event.type == "voice_stream_event_error":
                self.logger.error(f"UI session {self._bridge.ui_session_id }, voice pipeline error: {event.data}")

