use std::fs;
use std::io;
use std::path::Path;
use std::process;
use serde::{Deserialize, Serialize};
use thiserror::Error;

#[derive(Debug, Serialize, Deserialize)]
struct VersionData {
    name: String,
    version: String,
    description: String,
    #[serde(rename = "main")]
    main_file: String,
}

#[derive(Error, Debug)]
enum VersionError {
    #[error("IO error: {0}")]
    Io(#[from] io::Error),
    #[error("JSON error: {0}")]
    Json(#[from] serde_json::Error),
    #[error("Parse error: {0}")]
    Parse(#[from] std::num::ParseIntError),
    #[error("Invalid version format")]
    InvalidVersionFormat,
    #[error("Invalid update type")]
    InvalidUpdateType,
    #[error("{0}")]
    Custom(String),
}

fn main() {
    if let Err(e) = run() {
        eprintln!("Erreur: {}", e);
        process::exit(1);
    }
}

fn run() -> Result<(), VersionError> {
    let args: Vec<String> = std::env::args().collect();
    
    if args.len() < 2 {
        return Err(VersionError::Custom("Usage: version-manager <patch|minor|major|Init>".into()));
    }

    let type_arg = args[1].to_lowercase();
    let valid_types = ["patch", "minor", "major", "init"];

    if !valid_types.contains(&type_arg.as_str()) {
        return Err(VersionError::Custom("Le type doit être 'patch', 'minor', 'major' ou 'Init'".into()));
    }

    let version_file = "version.json";

    if type_arg == "init" {
        init_version_file(version_file)?;
    } else {
        update_version_file(version_file, &type_arg)?;
    }

    Ok(())
}

fn init_version_file(version_file: &str) -> Result<(), VersionError> {
    if Path::new(version_file).exists() {
        println!("File version.json already exists");
        return Ok(());
    }

    let current_dir = std::env::current_dir()?;
    let default_name = current_dir.file_name().unwrap().to_str().unwrap().to_string();
    let default_version = "0.0.1".to_string();
    let default_description = "".to_string();
    let default_main = "index.php".to_string();

    let name = read_input(&format!("Nom de l'application [{}]", default_name), &default_name);
    let version = read_input(&format!("Version [{}]", default_version), &default_version);
    let description = read_input(&format!("Description [{}]", default_description), &default_description);
    let main_file = read_input(&format!("Fichier principal [{}]", default_main), &default_main);

    let version_data = VersionData {
        name,
        version,
        description,
        main_file,
    };

    let json = serde_json::to_string_pretty(&version_data)?;
    fs::write(version_file, json)?;
    println!("Fichier version.json créé avec succès.");

    Ok(())
}

fn update_version_file(version_file: &str, update_type: &str) -> Result<(), VersionError> {
    if !Path::new(version_file).exists() {
        return Err(VersionError::Custom("Le fichier version.json est introuvable. Utilisez Init pour le créer.".into()));
    }

    let content = fs::read_to_string(version_file)?;
    let mut version_data: VersionData = serde_json::from_str(&content)?;

    let new_version = update_version_string(&version_data.version, update_type)?;
    version_data.version = new_version.clone();

    let json = serde_json::to_string_pretty(&version_data)?;
    fs::write(version_file, json)?;
    println!("Version mise à jour : {}", new_version);

    Ok(())
}

fn update_version_string(version: &str, update_type: &str) -> Result<String, VersionError> {
    let parts: Vec<&str> = version.split('.').collect();
    if parts.len() != 3 {
        return Err(VersionError::InvalidVersionFormat);
    }

    let mut major: u32 = parts[0].parse()?;
    let mut minor: u32 = parts[1].parse()?;
    let mut patch: u32 = parts[2].parse()?;

    match update_type {
        "patch" => patch += 1,
        "minor" => {
            minor += 1;
            patch = 0;
        }
        "major" => {
            major += 1;
            minor = 0;
            patch = 0;
        }
        _ => return Err(VersionError::InvalidUpdateType),
    }

    Ok(format!("{}.{}.{}", major, minor, patch))
}

fn read_input(prompt: &str, default_value: &str) -> String {
    println!("{}", prompt);
    let mut input = String::new();
    match io::stdin().read_line(&mut input) {
        Ok(_) => {
            let input = input.trim();
            if input.is_empty() {
                default_value.to_string()
            } else {
                input.to_string()
            }
        }
        Err(_) => default_value.to_string(),
    }
}