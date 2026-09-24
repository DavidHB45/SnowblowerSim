#!/usr/bin/env bash
# T2 — block on the unity-ci workflow for the current branch.
# Watches every run for HEAD (push + pull_request), falling back to the latest
# run on the branch. Exits non-zero if any run fails, after printing its
# failed-job logs.
set -euo pipefail

WORKFLOW="unity-ci.yml"

if ! command -v gh >/dev/null 2>&1; then
  echo "ci-wait: gh CLI not found. Install/auth gh, or check the run in the Actions tab." >&2
  exit 2
fi

branch="$(git rev-parse --abbrev-ref HEAD)"
sha="$(git rev-parse HEAD)"

# Runs appear a few seconds after the push; wait up to ~3 minutes for one.
run_ids=""
for _ in $(seq 1 36); do
  run_ids="$(gh run list --workflow "$WORKFLOW" --branch "$branch" --limit 20 \
    --json databaseId,headSha --jq ".[] | select(.headSha == \"$sha\") | .databaseId")"
  [[ -n "$run_ids" ]] && break
  sleep 5
done

if [[ -z "$run_ids" ]]; then
  echo "ci-wait: no run for HEAD $sha yet; using the latest run on $branch." >&2
  run_ids="$(gh run list --workflow "$WORKFLOW" --branch "$branch" --limit 1 \
    --json databaseId --jq '.[0].databaseId')"
fi

if [[ -z "$run_ids" || "$run_ids" == "null" ]]; then
  echo "ci-wait: no $WORKFLOW runs found for branch $branch." >&2
  exit 2
fi

status=0
for id in $run_ids; do
  url="$(gh run view "$id" --json url --jq .url)"
  echo "ci-wait: watching $url"
  if gh run watch "$id" --exit-status --interval 30; then
    echo "ci-wait: GREEN $url"
  else
    echo "ci-wait: RED $url" >&2
    gh run view "$id" --log-failed || true
    status=1
  fi
done
exit "$status"
