# Agent C Framework Setup Checklist

## Pre-requisites
- [ ] Ensure Docker/Rancher is installed and running on your system
- [ ] Verify you have your API keys ready

## Installation Check
- [ ] Determine if Agent C Framework is already installed
  - If installed: Identify location of the Agent C Framework directory
  - If not installed: Decide on installation location

## Installation/Update
- [ ] **If not installed:** Git clone the repository
  ```bash
  git clone [repository_url] [installation_path]
  ```
- [ ] **If already installed:** Navigate to the Agent C Framework directory and update
  ```bash
  cd [path_to_agent_c_framework]
  git pull
  ```

## Initial Setup
- [ ] Start Agent C services
  ```bash
  agentc up
  ```

## Configuration
- [ ] Update config file with your API keys
  - Location: [config_file_path]
  - Required keys: [list_of_required_keys]

## Database Setup
- [ ] Copy database files from build_support directory
  ```bash
  cp [build_support_directory]/[db_files] [destination]
  ```

## Final Startup
- [ ] Start Agent C services again to apply changes
  ```bash
  agentc up
  ```

## Verification
- [ ] Verify all services are running correctly
- [ ] Test basic functionality