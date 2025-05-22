#!/bin/zsh

# ─────── CONFIG ───────
# path to your AppVersion.cs
VersionFile="./Models/Static/AppVersion.cs"
# git branch to bump on
Branch="dev2"
# ──────────────────────

# make sure file exists
if [[ ! -f $VersionFile ]]; then
  echo "❌ File not found: $VersionFile"
  exit 1
fi

# kill the application if it's running already
killall -HUP AvaTerminal3 2>/dev/null

# extract current version "X.Y.Z"
currentVersion=$(grep 'VersionInfo' "$VersionFile" | gsed -E 's/.*"([0-9]+\.[0-9]+\.[0-9]+)".*/\1/')
IFS='.' read -r major minor patch <<< "$currentVersion"

# bump type arg: major, minor or patch (default: patch)
bumpType="${1:-patch}"

case "$bumpType" in
  major)
    ((major++))
    minor=0
    patch=0
    ;;
  minor)
    ((minor++))
    patch=0
    ;;
  patch)
    ((patch++))
    ;;
  *)
    echo "❌ Invalid bump type: $bumpType (use: major, minor, patch)"
    exit 1
    ;;
esac

newVersion="${major}.${minor}.${patch}"
echo "🔧 Bumping AppVersion: $currentVersion → $newVersion"

# update the C# file in‐place
# matches: public static readonly string VersionInfo = "X.Y.Z";
gsed -i'' -E "s|(VersionInfo = \")([0-9]+\.[0-9]+\.[0-9]+)(\";)|\1$newVersion\3|" "$VersionFile"
echo "✅ Updated $VersionFile → v$newVersion"

# clean the project
dotnet clean

# ───── GIT STUFF ─────
#git checkout "$Branch"
#git add "$VersionFile"
git add .
git commit -m "🔧 Bumping AppVersion: $currentVersion → $newVersion"
#git pull --rebase origin "$Branch"
git push -u origin "$Branch"
#git tag -s "v$newVersion" -m "Release v$newVersion"
#git push origin "v$newVersion"
#echo "🔖 Tagged and pushed release: https://github.com/repasscloud/ssh-c/releases/tag/v$newVersion"

# run the newly created program
dotnet run -f net9.0-maccatalyst 2>&1 | tee _logs/launch.log
