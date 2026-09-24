#!/usr/bin/env bash
# Prints "version=<m_EditorVersion>" from unity/ProjectSettings/ProjectVersion.txt
# (GitHub Actions output format). Fails with a clear message if the Unity
# project has not been created/committed yet.
set -euo pipefail

f="$(dirname "$0")/../unity/ProjectSettings/ProjectVersion.txt"
if [[ ! -f "$f" ]]; then
  echo "::error file=unity/ProjectSettings/ProjectVersion.txt::Unity project not committed yet. LOCAL STEP: create the URP project in the Editor and commit unity/ProjectSettings/ (see PR 'LOCAL STEPS for Dave')." >&2
  exit 1
fi

v="$(sed -n 's/^m_EditorVersion: *//p' "$f" | tr -d '\r')"
if [[ -z "$v" ]]; then
  echo "::error file=unity/ProjectSettings/ProjectVersion.txt::m_EditorVersion not found." >&2
  exit 1
fi
echo "version=$v"
