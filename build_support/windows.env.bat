:: Set environment variables for agent_c config and directories
:: %USERPROFILE% is equivalent to HOME on Windows
set AGENT_C_CONFIG_PATH=%USERPROFILE%\.agent_c
set AGENT_C_IMAGES_PATH=%USERPROFILE%\.agent_c\images
set AGENT_C_AGENTS_PATH=%USERPROFILE%\.agent_c\agents
set AGENT_C_SAVED_CHAT_FOLDER=%USERPROFILE%\.agent_c\saved_sessions

:: Add mappings for workspace folders (Documents, Desktop, and Downloads)
set DOCUMENTS_WORKSPACE=%USERPROFILE%\Documents
set DESKTOP_WORKSPACE=%USERPROFILE%\Desktop
set DOWNLOADS_WORKSPACE=%USERPROFILE%\Downloads
