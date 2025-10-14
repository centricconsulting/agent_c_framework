from agents.voice import VoiceWorkflowBase

class RTBridgeWorkflow(VoiceWorkflowBase):
    def __init__(self, bridge: "RealtimeBridge"):
        self._bridge = bridge

    async def run(self, transcription: str):
        async for response in self._bridge.iter_interact(transcription):
            yield response
