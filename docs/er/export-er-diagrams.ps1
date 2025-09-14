Param(
    [string]$OutDir = "./rendered"
)

Write-Host "== ER Diagram Export =="

if (-not (Test-Path $OutDir)) { New-Item -ItemType Directory -Path $OutDir | Out-Null }

$diagrams = @(
    'global-er.mmd',
    'employee-domain.mmd',
    'access-mapping.mmd',
    'identity-zones.mmd',
    'hardware-topology.mmd'
)

# Verify mermaid CLI
$mermaid = (Get-Command npx -ErrorAction SilentlyContinue)
if (-not $mermaid) {
    Write-Error "npx command not found. Install Node.js first."; exit 1
}

foreach ($d in $diagrams) {
    $inFile = Join-Path $PSScriptRoot $d
    if (-not (Test-Path $inFile)) { Write-Warning "Skipping missing $d"; continue }
    $base = [System.IO.Path]::GetFileNameWithoutExtension($d)
    $svg = Join-Path $OutDir ($base + '.svg')
    $png = Join-Path $OutDir ($base + '.png')
    Write-Host "Rendering $d -> $svg / $png"
    try {
        # Render SVG
    npx -y @mermaid-js/mermaid-cli mmdc -i $inFile -o $svg 2>$null
        if (-not (Test-Path $svg)) { Write-Warning "Failed to generate $svg" }
        # Render PNG
    npx -y @mermaid-js/mermaid-cli mmdc -i $inFile -o $png 2>$null
        if (-not (Test-Path $png)) { Write-Warning "Failed to generate $png" }
    }
    catch {
        Write-Warning ("Error rendering {0}: {1}" -f $d, $_.Exception.Message)
    }
}

Write-Host "Export complete. Files in $OutDir"