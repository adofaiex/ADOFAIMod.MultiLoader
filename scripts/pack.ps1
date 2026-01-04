dotnet build -c Release
if ($LASTEXITCODE -eq 0) {
    dotnet script scripts/pack.csx
}
