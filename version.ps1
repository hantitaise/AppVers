param (
    [Parameter(Mandatory=$true)]
    [ValidateSet("patch", "minor", "major", "Init")]
    [string]$Type
)

$versionFile = "version.json"

if ($Type -eq "Init") {
    if (Test-Path $versionFile) {
        Write-Host "File version.json already exists"
        exit 0
    }

    # Valeurs par défaut
    $defaultName = Split-Path -Leaf (Get-Location)
    $defaultVersion = "0.0.1"
    $defaultDescription = ""
    $defaultMain = "index.php"

    # Demander les valeurs à l'utilisateur
    $name = Read-Host "Nom de l'application [$defaultName]"
    if (-not $name) { $name = $defaultName }

    $version = Read-Host "Version [$defaultVersion]"
    if (-not $version) { $version = $defaultVersion }

    $description = Read-Host "Description [$defaultDescription]"
    if (-not $description) { $description = $defaultDescription }

    $main = Read-Host "Fichier principal [$defaultMain]"
    if (-not $main) { $main = $defaultMain }

    # Créer l'objet JSON
    $json = @{
        name = $name
        version = $version
        description = $description
        main = $main
    } | ConvertTo-Json -Depth 3

    # Sauvegarder dans le fichier
    $json | Set-Content $versionFile -Encoding UTF8

    Write-Host "Fichier version.json créé avec succès."
    exit 0
}

# --- Partie mise à jour de version (patch, minor, major) ---
if (!(Test-Path $versionFile)) {
    Write-Error "Le fichier version.json est introuvable. Utilisez Init pour le créer."
    exit 1
}

$json = Get-Content $versionFile | ConvertFrom-Json
$versionParts = $json.version -split '\.'
$major = [int]$versionParts[0]
$minor = [int]$versionParts[1]
$patch = [int]$versionParts[2]

switch ($Type) {
    "patch" {
        $patch++
    }
    "minor" {
        $minor++
        $patch = 0
    }
    "major" {
        $major++
        $minor = 0
        $patch = 0
    }
}

$json.version = "$major.$minor.$patch"
$json | ConvertTo-Json -Depth 3 | Set-Content $versionFile -Encoding UTF8

Write-Host "Version mise à jour : $($json.version)"
