import os
import re
from github import Github

# Get the token
token = os.environ.get("GITHUB_TOKEN")
repo_name = "IOExcept10n/BotForge"

# Initialize GitHub API
g = Github(token)
repo = g.get_repo(repo_name)

# Load issues
issues = {issue.number: issue for issue in repo.get_issues(state="all")}

# Load roadmap
with open("docs/roadmap.md", "r") as f:
    lines = f.readlines()

updated_lines = []

# Regex to find issue line
issue_pattern = re.compile(r"\[#(\d+)\]\(.*\)")

for line in lines:
    match = issue_pattern.search(line)
    if match:
        issue_number = int(match.group(1))
        if issue_number in issues:
            issue = issues[issue_number]
            # Get the state
            if issue.state == "closed":
                status = "✅ Done"
            elif "in-progress" in [l.name for l in issue.labels]:
                status = "⏳ In Progress"
            else:
                status = "❌ Planned"
            # Update string
            line = re.sub(r"(❌ Planned|⏳ In Progress|✅ Done)", status, line)
    updated_lines.append(line)

# Rewrite roadmap
with open("docs/roadmap.md", "w") as f:
    f.writelines(updated_lines)
