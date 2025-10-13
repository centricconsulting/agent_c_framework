# Quick Tips Reference

A collection of helpful reminders for common tasks and procedures.

## Development Environment

### Starting the API
```bash
# From the root directory
./scripts/start_api.sh
```

### Starting the Realtime Client
```bash
# From the //src/realtime_client directory
pnpm run dev
```

Alternatively:
```bash
# From any directory
./scripts/rebuild.sh
```

**Note:** Use the rebuild script if you have not run the fetch_latest script. This is an alternative way to ensure your environment is up to date.

### Fetching Latest Documentation
```bash
# From the root directory
./scripts/fetch_latest.sh
```

**Note:** This script CURRENTLY runs just `git pull` internally. However, the upstream release_1.0-pre branch is currently being worked on due to a couple of bugs causing errors. The script may need to be updated to `git pull upstream release_1.0-pre` when that branch is ready.

## Git Operations

### Pulling Specific Files/Directories from Another Branch
```bash
# Make sure you're on the branch where you want the file/directory
git checkout your-branch-name

# Grab a specific file from MAIN
git checkout main -- path/to/file.txt

# Or grab an entire directory from MAIN
git checkout main -- path/to/directory/
```

Then commit the changes:
```bash
git add .
git commit -m "Merged specific file/directory from main"
```

**Note:** This is useful when you want to pull something from the main branch to keep certain files or directories collected together without having to merge the entire branch.

### Updating Local Repo After Branch Rename
```bash
# Update local branch name
git branch -m old-name new-name

# Fetch from remote to update references
git fetch origin

# Set upstream branch to track the renamed remote branch
git branch -u origin/new-name new-name
```

**Note:** Use this when a repository branch name has been changed and you need to update your local environment to recognize the new name.

### Creating and Managing Branches
```bash
# Create a new branch and switch to it immediately
git checkout -b new-branch-name

# Or with newer Git syntax
git switch -c new-branch-name

# Create a branch without switching to it
git branch new-branch-name
```

Common workflow:
```bash
# Create and switch to new branch
git checkout -b feature/my-new-feature

# Make your changes, then push to GitHub
git push origin feature/my-new-feature
```

---
*Last Updated: October 13, 2025 - 3:45PM*