import os
import wave
import pyaudio
import threading
import logging

class AudioCues:
    def __init__(self, directory=None):
        self.directory = directory or os.path.join(os.getcwd(), 'audio')
        self.sounds: dict[str, str] = {}

    def load_sounds(self):
        if len(self.sounds) > 0:
            return

        """Load all .wav files from the directory into memory."""
        if not os.path.exists(self.directory):
            logging.warning(f"The directory {self.directory} does not exist.")
            return

        for filename in os.listdir(self.directory):
            if filename.endswith('.wav'):
                sound_name = os.path.splitext(filename)[0]
                sound_path = os.path.join(self.directory, filename)
                self.sounds[sound_name] = sound_path
                logging.info(f"Loaded sound: {sound_name}")

    def play_sound(self, sound_name):
        """Play a sound in a separate thread."""
        self.load_sounds()
        if sound_name in self.sounds:
            threading.Thread(target=self._play_sound_thread, args=(self.sounds[sound_name],), daemon=True).start()
        else:
            logging.error(f"Sound '{sound_name}' not found.")

    def _play_sound_thread(self, sound_path):
        """Target method for the thread that plays the sound."""
        try:
            # Open the sound file
            wf = wave.open(sound_path, 'rb')

            # Create a PyAudio stream
            p = pyaudio.PyAudio()
            stream = p.open(format=p.get_format_from_width(wf.getsampwidth()),
                            channels=wf.getnchannels(),
                            rate=wf.getframerate(),
                            output=True)

            # Read data in chunks and play
            chunk_size = 1024
            data = wf.readframes(chunk_size)
            while data:
                stream.write(data)
                data = wf.readframes(chunk_size)

            # Stop and close the stream
            stream.stop_stream()
            stream.close()
            p.terminate()
            wf.close()
        except Exception as e:
            logging.error(f"Error playing sound '{sound_path}': {e}")


