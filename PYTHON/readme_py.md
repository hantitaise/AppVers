# Rendre le script exécutable (Linux/Mac)
chmod +x version.py

# Initialiser le fichier version.json
python version.py Init

# Mettre à jour la version
python version.py patch
python version.py minor
python version.py major