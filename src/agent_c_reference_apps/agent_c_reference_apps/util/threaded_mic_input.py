import time
import base64
import threading
import sounddevice as sd

from queue import Queue
from typing import Union, Optional

from agent_c.util import MnemonicSlugs
from agent_c.models.events.audio import AudioInputBeginEvent, AudioInputDeltaEvent, AudioInputEndEvent

CHUNK_LENGTH_S = 0.05  # 100ms
SAMPLE_RATE = 24000
CHANNELS = 1


class MicInputThread:
    def __init__(self, **kwargs):
        self.allow_mic_input = kwargs.get("allow_mic_input", threading.Event())
        self.input_active = kwargs.get("input_active", threading.Event())
        self.output_queue = kwargs.get("output_queue", Queue())
        self.exit_event = kwargs.get("exit_event", threading.Event())
        self.stop_event = threading.Event()
        self.thread = Union[threading.Thread, None]
        self.chunk_length_s = kwargs.get("chunk_length_s", CHUNK_LENGTH_S)
        self.sample_rate = kwargs.get("sample_rate", SAMPLE_RATE)
        self.format = kwargs.get("format", "int16")
        self.channels = kwargs.get("channels", CHANNELS)

    def join(self):
        if self.thread is not None:
            self.thread.join()

    def exit(self):
        self.exit_event.set()

    def start(self):
        self.exit_event.clear()
        self.thread = threading.Thread(target=self._listen, daemon=True)
        self.thread.start()
        return self.thread

    def stop(self):
        if self.thread is not None:
            self.stop_event.set()
            self.thread.join()
            self.thread = None


    def _listen(self):
        audio_id: Optional[str] = None
        read_size = int(self.sample_rate * 0.02)
        stream = sd.InputStream(channels=self.channels, samplerate=self.sample_rate, dtype="int16")
        stream.start()

        try:
            while not self.stop_event.is_set() and not self.exit_event.is_set():
                if stream.read_available < read_size:
                    time.sleep(0.01)
                    continue

                data, _ = stream.read(read_size)

                if self.allow_mic_input.is_set() and self.input_active.is_set():
                    if audio_id is None:
                        audio_id = MnemonicSlugs.generate_slug(3)
                        self.output_queue.put(AudioInputBeginEvent(id=audio_id, sample_rate=self.sample_rate, channels=self.channels,
                                                                   format=self.format, chunk_length_s=self.chunk_length_s))

                    self.output_queue.put(AudioInputDeltaEvent(id=audio_id, audio=base64.b64encode(data).decode("utf-8")))
                elif audio_id is not None:
                    self.output_queue.put(AudioInputEndEvent(id=audio_id))
                    audio_id = None

                time.sleep(0.01)

            if audio_id:
                self.output_queue.put(AudioInputEndEvent(id=audio_id))
        finally:
            stream.stop()
            stream.close()