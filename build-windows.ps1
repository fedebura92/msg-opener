<#
  Compila MSG Opener y genera el instalador.
  Requisitos (una sola vez):
    winget install Microsoft.DotNet.SDK.8
    winget install JRSoftware.InnoSetup
  Uso:
    ./build-windows.ps1              # versión por defecto
    ./build-windows.ps1 -Version 1.0.1
#>
param([string]$Version = "1.1.0")
$ErrorActionPreference = 'Stop'
$root = $PSScriptRoot

if (-not (Get-Command dotnet -ErrorAction SilentlyContinue)) {
    throw "No se encontró .NET SDK 8. Instalalo con: winget install Microsoft.DotNet.SDK.8"
}

Write-Host "==> Publicando MSG Opener $Version..." -ForegroundColor Cyan
if (Test-Path "$root\publish") { Remove-Item "$root\publish" -Recurse -Force }
dotnet publish "$root\src\MSGOpener.csproj" -c Release -r win-x64 --self-contained true `
    -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true `
    -p:EnableCompressionInSingleFile=true -p:DebugType=None -p:Version=$Version `
    -o "$root\publish"
if ($LASTEXITCODE -ne 0) { throw "Falló dotnet publish" }

$iscc = @(
    "${env:ProgramFiles(x86)}\Inno Setup 6\ISCC.exe",
    "$env:ProgramFiles\Inno Setup 6\ISCC.exe",
    "$env:LOCALAPPDATA\Programs\Inno Setup 6\ISCC.exe"
) | Where-Object { Test-Path $_ } | Select-Object -First 1
if (-not $iscc) {
    $cmd = Get-Command ISCC.exe -ErrorAction SilentlyContinue
    if ($cmd) { $iscc = $cmd.Source }
}
if (-not $iscc) { throw "No se encontró Inno Setup 6. Instalalo con: winget install JRSoftware.InnoSetup" }

Write-Host "==> Compilando instalador..." -ForegroundColor Cyan
& $iscc "/DMyAppVersion=$Version" "$root\installer\MSGOpener.iss"
if ($LASTEXITCODE -ne 0) { throw "Falló la compilación del instalador" }

Write-Host "`nListo: $root\dist\MSGOpener-Setup-$Version.exe" -ForegroundColor Green
