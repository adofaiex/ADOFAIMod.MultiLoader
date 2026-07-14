param(
    [string]$ProjectName = ""
)

$ErrorActionPreference = "Stop"
$oldName = "AdofaiMod.MultiLoader"

if (-not $ProjectName) {
    $ProjectName = Split-Path -Leaf (Get-Location)
    Write-Host "Using directory name as project name: $ProjectName"
}

Write-Host "Renaming project from '$oldName' to '$ProjectName'..."

# Replace in file contents
Get-ChildItem -Recurse -File | Where-Object {
    $_.Extension -notin @('.dll', '.exe', '.pdb', '.nupkg', '.png', '.jpg')
} | ForEach-Object {
    $content = Get-Content -Path $_.FullName -Raw -ErrorAction SilentlyContinue
    if ($content -and $content.Contains($oldName)) {
        $content = $content.Replace($oldName, $ProjectName)
        Set-Content -Path $_.FullName -Value $content -NoNewline
        Write-Host "  updated: $($_.FullName)"
    }
}

# Rename files and directories containing the old name
Get-ChildItem -Recurse -File | Where-Object { $_.Name.Contains($oldName) } | ForEach-Object {
    $newName = $_.Name.Replace($oldName, $ProjectName)
    Rename-Item -Path $_.FullName -NewName $newName
    Write-Host "  renamed: $($_.Name) -> $newName"
}

Get-ChildItem -Recurse -Directory | Where-Object { $_.Name.Contains($oldName) } | Sort-Object FullName -Descending | ForEach-Object {
    $newName = $_.Name.Replace($oldName, $ProjectName)
    Rename-Item -Path $_.FullName -NewName $newName
    Write-Host "  renamed dir: $($_.Name) -> $newName"
}

# Rename solution file
$slnPath = "$oldName.sln"
if (Test-Path $slnPath) {
    $newSlnPath = "$ProjectName.sln"
    Rename-Item -Path $slnPath -NewName $newSlnPath
    Write-Host "  renamed: $slnPath -> $newSlnPath"
}

# Remove old .git and re-init
if (Test-Path ".git") {
    Remove-Item -Recurse -Force ".git"
    git init
    git add -A
    git commit -m "init: $ProjectName"
    Write-Host "Git repository re-initialized."
}

Write-Host "Done. Project '$ProjectName' is ready."
