using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

class VersionManager
{
    static void Main(string[] args)
    {
        if (args.Length < 1)
        {
            Console.WriteLine("Usage: VersionManager <patch|minor|major|Init>");
            Environment.Exit(1);
        }

        string type = args[0].ToLower();
        string[] validTypes = { "patch", "minor", "major", "init" };

        if (Array.IndexOf(validTypes, type) == -1)
        {
            Console.WriteLine("Erreur: Le type doit être 'patch', 'minor', 'major' ou 'Init'");
            Environment.Exit(1);
        }

        string versionFile = "version.json";

        if (type == "init")
        {
            if (File.Exists(versionFile))
            {
                Console.WriteLine("File version.json already exists");
                Environment.Exit(0);
            }

            // Valeurs par défaut
            string defaultName = new DirectoryInfo(Directory.GetCurrentDirectory()).Name;
            string defaultVersion = "0.0.1";
            string defaultDescription = "";
            string defaultMain = "index.php";

            // Demander les valeurs à l'utilisateur
            Console.Write($"Nom de l'application [{defaultName}]: ");
            string name = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(name)) name = defaultName;

            Console.Write($"Version [{defaultVersion}]: ");
            string version = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(version)) version = defaultVersion;

            Console.Write($"Description [{defaultDescription}]: ");
            string description = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(description)) description = defaultDescription;

            Console.Write($"Fichier principal [{defaultMain}]: ");
            string main = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(main)) main = defaultMain;

            // Créer l'objet JSON
            var versionData = new VersionData
            {
                Name = name,
                Version = version,
                Description = description,
                Main = main
            };

            // Sauvegarder dans le fichier
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };

            string json = JsonSerializer.Serialize(versionData, options);
            File.WriteAllText(versionFile, json, System.Text.Encoding.UTF8);

            Console.WriteLine("Fichier version.json créé avec succès.");
            Environment.Exit(0);
        }

        // --- Partie mise à jour de version (patch, minor, major) ---
        if (!File.Exists(versionFile))
        {
            Console.WriteLine("Erreur: Le fichier version.json est introuvable. Utilisez Init pour le créer.");
            Environment.Exit(1);
        }

        try
        {
            string jsonContent = File.ReadAllText(versionFile);
            VersionData versionData = JsonSerializer.Deserialize<VersionData>(jsonContent);

            if (versionData == null)
            {
                Console.WriteLine("Erreur: Impossible de parser le fichier JSON");
                Environment.Exit(1);
            }

            string[] versionParts = versionData.Version.Split('.');
            int major = int.Parse(versionParts[0]);
            int minor = int.Parse(versionParts[1]);
            int patch = int.Parse(versionParts[2]);

            switch (type)
            {
                case "patch":
                    patch++;
                    break;
                case "minor":
                    minor++;
                    patch = 0;
                    break;
                case "major":
                    major++;
                    minor = 0;
                    patch = 0;
                    break;
            }

            versionData.Version = $"{major}.{minor}.{patch}";

            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };

            string updatedJson = JsonSerializer.Serialize(versionData, options);
            File.WriteAllText(versionFile, updatedJson, System.Text.Encoding.UTF8);

            Console.WriteLine($"Version mise à jour : {versionData.Version}");
        }
        catch (JsonException)
        {
            Console.WriteLine("Erreur: Impossible de parser le fichier JSON");
            Environment.Exit(1);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur: {ex.Message}");
            Environment.Exit(1);
        }
    }
}

// Classe pour représenter les données de version
public class VersionData
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("version")]
    public string Version { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("main")]
    public string Main { get; set; } = string.Empty;
}