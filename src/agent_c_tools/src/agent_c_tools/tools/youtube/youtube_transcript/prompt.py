from typing import Any

from agent_c import PromptSection


class YoutubeTranscriptPrompt(PromptSection):
    def __init__(self, **data: Any):
        template = ("The youtube tool is used to retrieve and save a transcript from a user provided youtube url.  "
                    "<EXAMPLES>\n"
                    "User: 'Download the transcript from the youtube video at https://www.youtube.com/watch?v=xxYED234Xwr'"
                    "** assistant uses tool youtube-only_save_transcript with 'url = https://www.youtube.com/watch?v=xxYED234Xwr' **"
                    "Assistant: The file is saved at /workspace/youtube_workspace/xxYED234Xwr.txt"
                    "User: 'Download the transcript from the youtube video at https://www.youtube.com/watch?v=xxYED234Xwr' and save the filename as video_transcript.txt"
                    "** assistant uses tool youtube-only_save_transcript with 'url = https://www.youtube.com/watch?v=xxYED234Xwr' and file_name = 'video_transcript.txt' **"
                    "Assistant: The file is saved at /workspace/youtube_workspace/video_transcript.txt"
                    "User: 'Summarize the video from the youtube video at https://www.youtube.com/watch?v=xxYED234Xwr'"
                    "** assistant uses tool youtube-retrieve_and_save_transcript with 'url = https://www.youtube.com/watch?v=xxYED234Xwr' **"
                    "Assistant: The file is saved at /workspace/youtube_workspace/xxYED234Xwr.txt. \n"
                    "The summary is: 'This video is about how to make a cake.  The video is 10 minutes long and has 3 steps. The steps are: 1. Mix the ingredients, 2. Bake the cake, 3. Frost the cake.'"
                    "User: 'Download and summarize the transcript from the youtube video at https://www.youtube.com/watch?v=xxYED234Xwr' and save the filename as video_transcript.txt"
                    "** assistant uses tool youtube-retrieve_and_save_transcript with 'url = https://www.youtube.com/watch?v=xxYED234Xwr' and file_name = 'video_transcript.txt' **"
                    "Assistant: The file is saved at /workspace/youtube_workspace/video_transcript.txt\n"
                    "The summary is: 'This video is about how to make a cake.  The video is 10 minutes long and has 3 steps. The steps are: 1. Mix the ingredients, 2. Bake the cake, 3. Frost the cake.'"
                    "</EXAMPLES>")
        super().__init__(template=template, required=True, name="Tavily Search", render_section_header=True, **data)
