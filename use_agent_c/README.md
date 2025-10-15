# Agent C "Quick" Install (Preview) 

The compose files here allow Docker/Rancher users to get up in running using a local version of Agent C without needing to install a bunch of developer tools on their machines.  It also provides Agent C developers to run the Agent C Client while working on the Agent C client.

> [!WARNING]
> While the new reference client offers a far better user experience han Agent C has ever had but...
> 
> It is still under heavy development and is short several features, but it's quite usable, if a little rough around the edges.
> 

## Getting started

In the `scripts` folder are Windows batch files and *nix shell scripts to allow you to run the Agent C AOI and web client behind an HTTPS proxy using self-signed certs.  Once started you will be able to access Agent C at https://localhost:5173/chat *If you forget the port and have nothing listening on 443 you will be redirected.*

These *should* be able to be double-clicked in Explorer.  Mac/Linux users will need to `chmod a+x scripts/*.sh` first.

> [!IMPORTANT]
> The container images are not yet being built and pushed on GitHub.  That will change soon but for now you must build the containers locally.
>  

### New users

1. **Build the containers** - Run `scripts/build_containers.(bat/sh)` and wait a bit.
2. **Initialize your local storage** - Run `scripts/start_agent_c.(bat/sh)`.
   - This will create a `.agent_c` folder under your home folder where it will store:
     - The configuration file
     - The user database
     - The chat session index.
     - Your saved chat files
     - Any agents you create
   - It will then open a text editor so that you can supply the various API keys for the LLMs / tools you want to use.
3. **Run Agent C** - Run `scripts/start_agent_c.(bat/sh)` again and it will see that you have a config file and launch the compose file.

### Users of the old client

1. **Build the containers** - Run `scripts/build_containers.(bat/sh)` and wait a bit.
2. **Copy the databases** - Copy the two databases in the `data` subfolder to yout `.agent_c` subfolder of your user folder.
3. **Run Agent C** - Run `scripts/start_agent_c.(bat/sh)` again and it will see that you have a config file and launch the compose file. 

### All users

The purpose of using these scripts over a simple `docker-compose up` (Which works for the fronend only compose file) is to map several well known folders to workspaces in Agent C. Docker users will have the following workspaces available for agents to access the local file system and run commands:

- documents
- desktop
- downloads

An easy was to add additional wones without building your own container is on the list.


## Logging in

There is one user, `admin` with the password `changeme`,  Except as of this writing tha ability to actualy change it if you're a user doesn't exist.. Hence preview status.