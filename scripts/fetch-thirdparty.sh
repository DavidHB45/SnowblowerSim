#!/usr/bin/env bash
# Downloads every entry in unity/Assets/ThirdParty/manifest.json into
# unity/Assets/ThirdParty/<source>/<id>/ and writes <id>.license.txt beside it.
# Runs on Dave's machine (Git Bash is fine), NOT in the cloud container.
# Needs only curl plus unzip (or tar) for ambientCG zips. No jq/python.
#
#   scripts/fetch-thirdparty.sh            # fetch missing entries
#   scripts/fetch-thirdparty.sh --force    # re-download everything
#
# Sources:
#   polyhaven : 4k HDRI (.hdr) or all 4k JPG texture maps, via api.polyhaven.com/files/<id>
#   ambientcg : <id>_4K-JPG.zip from ambientcg.com/get
set -uo pipefail

ROOT="$(cd "$(dirname "$0")/.." && pwd)"
TP="$ROOT/unity/Assets/ThirdParty"
MANIFEST="$TP/manifest.json"
UA="SnowblowerSim-fetch/1.0"
FORCE=0
[[ "${1:-}" == "--force" ]] && FORCE=1

# manifest.json keeps one object per line so it can be read without a JSON parser.
field() { sed -n "s/.*\"$1\": *\"\([^\"]*\)\".*/\1/p" <<<"$2"; }

get() { curl -fL --retry 3 -A "$UA" -o "$2" "$1"; }

fetch_polyhaven() {  # id dest
  local files urls
  files="$(curl -fsSL -A "$UA" "https://api.polyhaven.com/files/$1")" || return 1
  urls="$(grep -oE 'https://dl\.polyhaven\.org/[^"]*_4k\.hdr' <<<"$files" | sort -u | head -n 1)"
  if [[ -z "$urls" ]]; then
    urls="$(grep -oE 'https://dl\.polyhaven\.org/file/ph-assets/Textures/jpg/4k/[^"]*_4k\.jpg' <<<"$files" | sort -u)"
  fi
  [[ -n "$urls" ]] || { echo "  no 4k HDRI/JPG maps listed for $1" >&2; return 1; }
  for u in $urls; do
    echo "  $u"
    get "$u" "$2/$(basename "$u")" || return 1
  done
}

fetch_ambientcg() {  # id dest
  local zip="$2/$1_4K-JPG.zip"
  get "https://ambientcg.com/get?file=$1_4K-JPG.zip" "$zip" || return 1
  if command -v unzip >/dev/null 2>&1; then unzip -oq "$zip" -d "$2"; else tar -xf "$zip" -C "$2"; fi || return 1
  rm -f "$zip"
}

write_license() {  # id source url license dest
  cat > "$5/$1.license.txt" <<EOF
Asset:    $1
Source:   $2
URL:      $3
License:  $4
Fetched:  $(date -u +%Y-%m-%d)
$( [[ "$4" == "CC0" ]] && echo "CC0 1.0 Universal (public domain dedication): https://creativecommons.org/publicdomain/zero/1.0/" )
Record this asset in docs/asset-recon/LICENSES-AND-ATTRIBUTION.md.
EOF
}

[[ -f "$MANIFEST" ]] || { echo "missing $MANIFEST" >&2; exit 1; }

failed=()
while IFS= read -r line; do
  id="$(field id "$line")"
  [[ -n "$id" ]] || continue
  source="$(field source "$line")"; url="$(field url "$line")"
  license="$(field license "$line")"; dest="$TP/$(field destFolder "$line")"

  if [[ $FORCE -eq 0 && -f "$dest/$id.license.txt" ]] && ls "$dest" | grep -qv '\.license\.txt'; then
    echo "skip  $source/$id (already fetched)"
    continue
  fi

  echo "fetch $source/$id"
  mkdir -p "$dest"
  case "$source" in
    polyhaven) fetch_polyhaven "$id" "$dest" ;;
    ambientcg) fetch_ambientcg "$id" "$dest" ;;
    *) echo "  unknown source '$source'" >&2; false ;;
  esac
  if [[ $? -eq 0 ]]; then
    write_license "$id" "$source" "$url" "$license" "$dest"
  else
    failed+=("$source/$id")
  fi
done < "$MANIFEST"

if [[ ${#failed[@]} -gt 0 ]]; then
  echo "FAILED: ${failed[*]}" >&2
  exit 1
fi
echo "done. Commit only *.license.txt (the rest is gitignored)."
