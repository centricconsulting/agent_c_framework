from typing import Any

from agent_c.prompting.prompt_section import PromptSection


class GithubSection(PromptSection):
    """
    Prompt section for GitHub tools.
    """

    def __init__(self, **data: Any):
        TEMPLATE = (
            "The GitHub Tools provide access to GitHub operations and data:\n"
            "\n"
            "### Repository Operations\n"
            "- **search_repositories**: Search for repositories matching a query\n"
            "- **get_file_contents**: Get the contents of a file from a repository\n"
            "- **list_commits**: List commits for a repository\n"
            "- **search_code**: Search for code within repositories\n"
            "- **get_commit**: Get details of a specific commit\n"
            "- **list_branches**: List branches in a repository\n"
            "- **list_tags**: List tags in a repository\n"
            "- **get_tag**: Get details of a specific tag\n"
            "- **create_or_update_file**: Create or update a file in a repository\n"
            "- **create_repository**: Create a new repository\n"
            "- **fork_repository**: Fork an existing repository\n"
            "- **create_branch**: Create a new branch in a repository\n"
            "- **push_files**: Push multiple files to a repository\n"
            "- **delete_file**: Delete a file from a repository\n"
            "\n"
            "### Issue Management\n"
            "- **get_issue**: Get details of a specific issue\n"
            "- **search_issues**: Search for issues matching a query\n"
            "- **list_issues**: List issues for a repository\n"
            "- **get_issue_comments**: Get comments for an issue\n"
            "- **create_issue**: Create a new issue\n"
            "- **add_issue_comment**: Add a comment to an issue\n"
            "- **update_issue**: Update an existing issue\n"
            "- **assign_copilot_to_issue**: Assign GitHub Copilot to an issue\n"
            "\n"
            "### Pull Request Operations\n"
            "- **get_pull_request**: Get details of a specific pull request\n"
            "- **list_pull_requests**: List pull requests for a repository\n"
            "- **get_pull_request_files**: Get files changed in a pull request\n"
            "- **get_pull_request_status**: Get the status of a pull request\n"
            "- **get_pull_request_comments**: Get comments on a pull request\n"
            "- **get_pull_request_reviews**: Get reviews for a pull request\n"
            "- **get_pull_request_diff**: Get the diff for a pull request\n"
            "- **create_pull_request**: Create a new pull request\n"
            "- **update_pull_request**: Update an existing pull request\n"
            "- **merge_pull_request**: Merge a pull request\n"
            "- **update_pull_request_branch**: Update a pull request branch\n"
            "- **request_copilot_review**: Request a review from GitHub Copilot\n"
            "- **create_pull_request_review**: Create and submit a pull request review\n"
            "\n"
            "### Security Tools\n"
            "- **get_code_scanning_alert**: Get details of a code scanning alert\n"
            "- **list_code_scanning_alerts**: List code scanning alerts for a repository\n"
            "- **get_secret_scanning_alert**: Get details of a secret scanning alert\n"
            "- **list_secret_scanning_alerts**: List secret scanning alerts for a repository\n"
            "\n"
            "### Notifications\n"
            "- **list_notifications**: List notifications for the current user\n"
            "- **get_notification_details**: Get details of a specific notification\n"
            "- **dismiss_notification**: Dismiss a notification\n"
            "- **mark_all_notifications_read**: Mark all notifications as read\n"
            "\n"
            "### User Information\n"
            "- **get_me**: Get information about the authenticated user\n"
            "- **search_users**: Search for users matching a query\n"
            "\n"
            "**Note**: This tool requires GitHub authentication via a personal access token.\n"
            "Operations are subject to GitHub API rate limits and permissions."
        )
        super().__init__(
            template=TEMPLATE, 
            required=True, 
            name="GitHub Tools", 
            render_section_header=True, 
            **data
        )