"""
yt-dlp video downloader for SharePoint videos.

Executes yt-dlp to download videos with DASH manifest URLs,
matching the implementation in //sharedown/new/downloader.py
"""

import asyncio
import logging
import subprocess
from pathlib import Path
from typing import Dict, Optional



class YtdlpDownloader:
    """Handles video download using yt-dlp with DASH manifest URLs."""
    
    def __init__(self):
        """
        Initialize downloader.

        """
        self.process: Optional[subprocess.Popen] = None
        self.logger = logging.getLogger(__name__)

    
    async def download_video(self, 
                            video_url: str,
                            output_path: Path,
                            concurrent_fragments: int = 5) -> Dict[str, any]:
        """
        Download video using yt-dlp with DASH manifest URL.
        
        Args:
            video_url: DASH manifest URL (with embedded auth token)
            output_path: Full path where video should be saved
            concurrent_fragments: Number of concurrent fragment downloads
            
        Returns:
            Dictionary with download result and metadata
        """
        self.logger.info("Preparing yt-dlp download...")
        
        try:
            # Validate URL
            if not video_url.startswith('https://'):
                self.logger.error(f"Invalid URL scheme: {video_url[:50]}")
                return {'success': False, 'error': 'Invalid URL scheme'}
            
            if '{.' in video_url:
                self.logger.error("URL contains unreplaced placeholders!")
                return {'success': False, 'error': 'Unreplaced placeholders in URL'}
            
            # Build yt-dlp command
            cmd = ['yt-dlp', '-v']
            
            # Concurrent fragments
            cmd.extend(['-N', str(concurrent_fragments)])
            
            # Timeouts and retries
            cmd.extend(['--socket-timeout', '30'])
            cmd.extend(['--retries', '10'])
            cmd.extend(['--fragment-retries', '10'])
            cmd.extend(['--retry-sleep', '5'])
            
            # User agent
            user_agent = 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36'
            cmd.extend(['--user-agent', user_agent])
            
            # Output template - merge format for DASH
            cmd.extend(['-o', str(output_path)])
            cmd.extend(['--merge-output-format', 'mp4'])
            
            # Add the video URL
            cmd.append(video_url)
            
            # Log command details
            self.logger.info("=" * 80)
            self.logger.info("YT-DLP COMMAND DETAILS:")
            self.logger.info(f"  Output: {output_path}")
            self.logger.info(f"  URL length: {len(video_url)} characters")
            self.logger.info(f"  Full URL: {video_url}")
            self.logger.info("=" * 80)
            
            # Convert to absolute path
            abs_output_path = output_path.resolve()
            for i, arg in enumerate(cmd):
                if arg == str(output_path):
                    cmd[i] = str(abs_output_path)
                    break
            
            # Start download
            self.logger.info("Starting yt-dlp download - output will appear below:")
            self.logger.info("=" * 80)
            
            self.process = await asyncio.create_subprocess_exec(
                *cmd,
                stdout=None,  # Inherit stdout
                stderr=None   # Inherit stderr
            )
            
            # Wait for completion
            return_code = await self.process.wait()
            
            self.logger.info("=" * 80)
            
            if return_code == 0:
                self.logger.info("✓ VIDEO DOWNLOAD COMPLETED SUCCESSFULLY!")
                
                # Check if file exists and show size
                if output_path.exists():
                    file_size_mb = output_path.stat().st_size / (1024 * 1024)
                    self.logger.info(f"✓ File saved to: {output_path}")
                    self.logger.info(f"✓ File size: {file_size_mb:.2f} MB")
                    self.logger.info("=" * 80)
                    
                    return {
                        'success': True,
                        'output_file': str(output_path),
                        'file_size_mb': file_size_mb
                    }
                elif output_path.with_suffix('.mp4').exists():
                    actual_file = output_path.with_suffix('.mp4')
                    file_size_mb = actual_file.stat().st_size / (1024 * 1024)
                    self.logger.info(f"✓ File saved to: {actual_file}")
                    self.logger.info(f"✓ File size: {file_size_mb:.2f} MB")
                    self.logger.info("=" * 80)
                    
                    return {
                        'success': True,
                        'output_file': str(actual_file),
                        'file_size_mb': file_size_mb
                    }
                else:
                    # Check for similar files
                    parent = output_path.parent
                    stem = output_path.stem
                    matching_files = list(parent.glob(f"{stem}*"))
                    if matching_files:
                        actual_file = matching_files[0]
                        file_size_mb = actual_file.stat().st_size / (1024 * 1024)
                        self.logger.info(f"✓ File saved to: {actual_file}")
                        self.logger.info(f"✓ File size: {file_size_mb:.2f} MB")
                        self.logger.info("=" * 80)
                        
                        return {
                            'success': True,
                            'output_file': str(actual_file),
                            'file_size_mb': file_size_mb
                        }
                    else:
                        self.logger.warning(f"⚠️  Expected file not found: {output_path}")
                        self.logger.info("=" * 80)
                        return {
                            'success': False,
                            'error': 'File not found after download'
                        }
            else:
                self.logger.error(f"✗ yt-dlp failed with return code {return_code}")
                self.logger.info("=" * 80)
                return {
                    'success': False,
                    'error': f'yt-dlp return code {return_code}'
                }
                
        except Exception as e:
            self.logger.error(f"yt-dlp execution failed: {e}")
            return {
                'success': False,
                'error': str(e)
            }
        finally:
            self.process = None
    
    async def stop(self):
        """Stop the download process."""
        if self.process:
            try:
                self.process.terminate()
                await asyncio.wait_for(self.process.wait(), timeout=5.0)
            except asyncio.TimeoutError:
                self.process.kill()
                await self.process.wait()
            self.logger.info("yt-dlp download stopped")


# async def download_video(video_url: str, output_path: Path) -> Optional[Dict]:
#     """
#     Convenience function to download video using yt-dlp.
#
#     Args:
#         video_url: DASH manifest URL
#         output_path: Full path where video should be saved
#
#     Returns:
#         Download result dictionary or None
#     """
#     downloader = YtdlpDownloader(output_path.parent)
#     return await downloader.download_video(video_url, output_path)
