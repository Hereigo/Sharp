#!/usr/bin/env bash
# Claude Code hook: tells the agent (and the user) which git branch the code is on.
# Used for SessionStart, UserPromptSubmit and SubagentStart.

input=$(cat)
event=$(printf '%s' "$input" | sed -n 's/.*"hook_event_name"[[:space:]]*:[[:space:]]*"\([^"]*\)".*/\1/p')
[ -z "$event" ] && event="SessionStart"

cd "${CLAUDE_PROJECT_DIR:-.}" 2>/dev/null || exit 0
git rev-parse --is-inside-work-tree >/dev/null 2>&1 || exit 0

branch=$(git branch --show-current 2>/dev/null)
[ -z "$branch" ] && branch="(detached HEAD at $(git rev-parse --short HEAD 2>/dev/null))"
commit=$(git log -1 --format='%h %s' 2>/dev/null | tr -d '"\\')
dirty=$(git status --porcelain 2>/dev/null | wc -l | tr -d ' ')
upstream=$(git rev-parse --abbrev-ref --symbolic-full-name '@{u}' 2>/dev/null)
[ -z "$upstream" ] && upstream="none"

msg="Git branch: $branch | upstream: $upstream | last commit: $commit | uncommitted files: $dirty"

if [ "$event" = "SessionStart" ]; then
  # Shown to the user and added to the agent's context
  printf '{"systemMessage":"%s","hookSpecificOutput":{"hookEventName":"%s","additionalContext":"Current %s"}}\n' "$msg" "$event" "$msg"
else
  # Per prompt / per subagent: context only, to avoid noise
  printf '{"hookSpecificOutput":{"hookEventName":"%s","additionalContext":"Current %s"}}\n' "$event" "$msg"
fi
