# Compiler et exécuter
cargo run -- Init
cargo run -- patch
cargo run -- minor
cargo run -- major

# Compiler en release
cargo build --release

# Exécuter le binaire compilé
./target/release/version-manager Init