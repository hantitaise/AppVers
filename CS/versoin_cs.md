# Compiler le projet
dotnet build

# Exécuter (depuis le dossier bin)
dotnet run -- Init
dotnet run -- patch
dotnet run -- minor
dotnet run -- major

# Ou créer un exécutable autonome
dotnet publish -c Release -r win-x64 --self-contained true


dotnet publish -c Release -p:PublishSingleFile=true --self-contained true -r win-x64