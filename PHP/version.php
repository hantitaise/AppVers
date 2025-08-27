<?php

function main() {
    global $argc, $argv;
    // Vérifier les arguments
    if ($argc < 2) {
        echo "Usage: php version.php <patch|minor|major|Init>\n";
        exit(1);
    }

    $type = strtolower($argv[1]);
    $validTypes = ['patch', 'minor', 'major', 'init'];
    
    if (!in_array($type, $validTypes)) {
        echo "Erreur: Le type doit être 'patch', 'minor', 'major' ou 'Init'\n";
        exit(1);
    }

    $versionFile = "version.json";

    if ($type === 'init') {
        if (file_exists($versionFile)) {
            echo "File version.json already exists\n";
            exit(0);
        }

        // Valeurs par défaut
        $defaultName = basename(getcwd());
        $defaultVersion = "0.0.1";
        $defaultDescription = "";
        $defaultMain = "index.php";

        // Demander les valeurs à l'utilisateur
        echo "Nom de l'application [$defaultName]: ";
        $name = trim(fgets(STDIN));
        if (empty($name)) { $name = $defaultName; }

        echo "Version [$defaultVersion]: ";
        $version = trim(fgets(STDIN));
        if (empty($version)) { $version = $defaultVersion; }

        echo "Description [$defaultDescription]: ";
        $description = trim(fgets(STDIN));
        if (empty($description)) { $description = $defaultDescription; }

        echo "Fichier principal [$defaultMain]: ";
        $main = trim(fgets(STDIN));
        if (empty($main)) { $main = $defaultMain; }

        // Créer l'objet JSON
        $jsonData = [
            'name' => $name,
            'version' => $version,
            'description' => $description,
            'main' => $main
        ];

        // Sauvegarder dans le fichier
        $jsonContent = json_encode($jsonData, JSON_PRETTY_PRINT | JSON_UNESCAPED_UNICODE);
        file_put_contents($versionFile, $jsonContent);

        echo "Fichier version.json créé avec succès.\n";
        exit(0);
    }

    // --- Partie mise à jour de version (patch, minor, major) ---
    if (!file_exists($versionFile)) {
        echo "Erreur: Le fichier version.json est introuvable. Utilisez Init pour le créer.\n";
        exit(1);
    }

    $jsonContent = file_get_contents($versionFile);
    $json = json_decode($jsonContent, true);
    
    if ($json === null) {
        echo "Erreur: Impossible de parser le fichier JSON\n";
        exit(1);
    }

    $versionParts = explode('.', $json['version']);
    $major = (int)$versionParts[0];
    $minor = (int)$versionParts[1];
    $patch = (int)$versionParts[2];

    switch ($type) {
        case "patch":
            $patch++;
            break;
        case "minor":
            $minor++;
            $patch = 0;
            break;
        case "major":
            $major++;
            $minor = 0;
            $patch = 0;
            break;
    }

    $json['version'] = "$major.$minor.$patch";
    $updatedJson = json_encode($json, JSON_PRETTY_PRINT | JSON_UNESCAPED_UNICODE);
    file_put_contents($versionFile, $updatedJson);

    echo "Version mise à jour : " . $json['version'] . "\n";
}

// Exécuter le script
main();