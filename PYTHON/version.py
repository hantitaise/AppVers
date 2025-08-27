#!/usr/bin/env python3
import json
import os
import sys
from pathlib import Path

def main():
    # Vérifier les arguments
    if len(sys.argv) < 2:
        print("Usage: python version.py <patch|minor|major|Init>")
        sys.exit(1)
    
    type_arg = sys.argv[1].lower()
    valid_types = ['patch', 'minor', 'major', 'init']
    
    if type_arg not in valid_types:
        print("Erreur: Le type doit être 'patch', 'minor', 'major' ou 'Init'")
        sys.exit(1)
    
    version_file = "version.json"
    
    if type_arg == 'init':
        if Path(version_file).exists():
            print("File version.json already exists")
            sys.exit(0)
        
        # Valeurs par défaut
        default_name = os.path.basename(os.getcwd())
        default_version = "0.0.1"
        default_description = ""
        default_main = "index.php"
        
        # Demander les valeurs à l'utilisateur
        name = input(f"Nom de l'application [{default_name}]: ").strip()
        if not name:
            name = default_name
        
        version = input(f"Version [{default_version}]: ").strip()
        if not version:
            version = default_version
        
        description = input(f"Description [{default_description}]: ").strip()
        if not description:
            description = default_description
        
        main_file = input(f"Fichier principal [{default_main}]: ").strip()
        if not main_file:
            main_file = default_main
        
        # Créer l'objet JSON
        json_data = {
            'name': name,
            'version': version,
            'description': description,
            'main': main_file
        }
        
        # Sauvegarder dans le fichier
        with open(version_file, 'w', encoding='utf-8') as f:
            json.dump(json_data, f, indent=2, ensure_ascii=False)
        
        print("Fichier version.json créé avec succès.")
        sys.exit(0)
    
    # --- Partie mise à jour de version (patch, minor, major) ---
    if not Path(version_file).exists():
        print("Erreur: Le fichier version.json est introuvable. Utilisez Init pour le créer.")
        sys.exit(1)
    
    try:
        with open(version_file, 'r', encoding='utf-8') as f:
            json_data = json.load(f)
    except json.JSONDecodeError:
        print("Erreur: Impossible de parser le fichier JSON")
        sys.exit(1)
    
    version_parts = json_data['version'].split('.')
    major = int(version_parts[0])
    minor = int(version_parts[1])
    patch = int(version_parts[2])
    
    if type_arg == "patch":
        patch += 1
    elif type_arg == "minor":
        minor += 1
        patch = 0
    elif type_arg == "major":
        major += 1
        minor = 0
        patch = 0
    
    json_data['version'] = f"{major}.{minor}.{patch}"
    
    with open(version_file, 'w', encoding='utf-8') as f:
        json.dump(json_data, f, indent=2, ensure_ascii=False)
    
    print(f"Version mise à jour : {json_data['version']}")

if __name__ == "__main__":
    main()