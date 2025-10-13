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

### Recovering from Working on the Wrong Branch

If you haven't committed yet (easiest option):

```bash
# Stash your changes, switch branches, and apply them
git stash
git checkout correct-branch
git stash pop

# Or even simpler, Git will often let you just switch branches directly if there are no conflicts:
git checkout correct-branch
# (Git will bring your uncommitted changes with you!)
```

If you already committed to the wrong branch:

**Option 1 - Cherry-pick (cleaner):**
```bash
# Note the commit hash (use git log to find it)
git log  # copy the commit hash

# Switch to the correct branch
git checkout correct-branch

# Apply that specific commit
git cherry-pick <commit-hash>

# Go back to wrong branch and undo the commit
git checkout wrong-branch
git reset --hard HEAD~1
```

**Option 2 - Create a new branch from where you are:**
```bash
# Creates a new branch with your current commit
git checkout -b correct-branch-name

# Then go back to the wrong branch and reset it
git checkout wrong-branch
git reset --hard HEAD~1
```

**Note:** The stash method is definitely the easiest if you catch it before committing! Git is pretty forgiving about moving uncommitted work around between branches.

---
*Last Updated: October 13, 2025 - 3:50PM*