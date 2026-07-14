#!/usr/bin/env bash
set -euo pipefail

old_name="AdofaiMod.MultiLoader"
project_name="${1:-$(basename "$(pwd)")}"

echo "Renaming project from '$old_name' to '$project_name'..."

# Replace in file contents
while IFS= read -r file; do
    case "$file" in
        *.dll|*.exe|*.pdb|*.nupkg|*.png|*.jpg) continue ;;
    esac
    if grep -q "$old_name" "$file" 2>/dev/null; then
        sed -i "s/$old_name/$project_name/g" "$file"
        echo "  updated: $file"
    fi
done < <(find . -type f -not -path './.git/*' -not -name 'init.sh' -not -name 'init.ps1')

# Rename files containing old_name
while IFS= read -r file; do
    dir=$(dirname "$file")
    base=$(basename "$file")
    new_base="${base//$old_name/$project_name}"
    if [ "$base" != "$new_base" ]; then
        mv "$file" "$dir/$new_base"
        echo "  renamed: $base -> $new_base"
    fi
done < <(find . -type f -name "*$old_name*" -not -path './.git/*')

# Rename directories containing old_name (bottom-up)
while IFS= read -r dir; do
    parent=$(dirname "$dir")
    base=$(basename "$dir")
    new_base="${base//$old_name/$project_name}"
    if [ "$base" != "$new_base" ]; then
        mv "$dir" "$parent/$new_base"
        echo "  renamed dir: $base -> $new_base"
    fi
done < <(find . -type d -name "*$old_name*" -not -path './.git/*' | sort -r)

# Rename solution file
if [ -f "$old_name.sln" ]; then
    mv "$old_name.sln" "$project_name.sln"
    echo "  renamed: $old_name.sln -> $project_name.sln"
fi

# Re-init git
if [ -d ".git" ]; then
    rm -rf ".git"
    git init
    git add -A
    git commit -m "init: $project_name"
    echo "Git repository re-initialized."
fi

echo "Done. Project '$project_name' is ready."
