#!/bin/bash

# Ausgangsverzeichnis
BASE_DIR="/Users/svenkonz/Documents/GitHub/HolisticApp/HolisticApp"

# Name der Ausgabedatei
OUTPUT_FILE="$BASE_DIR/Project_Code_Export.txt"

# Falls die Datei existiert, vorher löschen
rm -f "$OUTPUT_FILE"

echo "🚀 Starte Code-Extraktion..."

# 1️⃣ Exportiere die Hauptdateien
FILES=("App.xaml" "App.xaml.cs" "MauiProgram.cs" "HolisticApp.csproj")
for FILE in "${FILES[@]}"; do
    if [[ -f "$BASE_DIR/$FILE" ]]; then
        echo "===== Datei: $FILE =====" >> "$OUTPUT_FILE"
        cat "$BASE_DIR/$FILE" >> "$OUTPUT_FILE"
        echo -e "\n\n" >> "$OUTPUT_FILE"
    fi
done

# 2️⃣ Suche in den Unterordnern und exportiere Dateien
SUBFOLDERS=("Models" "Views" "Data" "Properties" "ViewModels")
EXTENSIONS=("cs" "xaml" "json" "xml" "config")

for FOLDER in "${SUBFOLDERS[@]}"; do
    for EXT in "${EXTENSIONS[@]}"; do
        find "$BASE_DIR/$FOLDER" -type f -name "*.$EXT" | while read -r FILE; do
            echo "===== Datei: $FILE =====" >> "$OUTPUT_FILE"
            cat "$FILE" >> "$OUTPUT_FILE"
            echo -e "\n\n" >> "$OUTPUT_FILE"
        done
    done
done

echo "✅ Code wurde erfolgreich in $OUTPUT_FILE gespeichert."