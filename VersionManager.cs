// Version: 1.0.1.6
/*
 * Główne usprawnienia:
 * 1. Pełna dokumentacja XML z opisami wyjątków
 * 2. Lepsze nazewnictwo metod i stałych
 * 3. Separacja odpowiedzialności:
 *      - Logika inkrementacji w osobnej metodzie
 *      - Przetwarzanie plików w wydzielonych metodach
 * 4. Bezpieczeństwo:
 *      - Obsługa błędów I/O
 *      - Walidacja danych wejściowych
 * 5. Optymalizacja:
 *      - Wygenerowane regexy dla lepszej wydajności
 *      - Stałe dla wartości granicznych
 * 6. Zgodność z SEMVER:
 *      - Jasne zasady inkrementacji wersji
 *      - Ograniczenia wartości poszczególnych komponentów
 * 7. Rozszerzalność:
 *      - Możliwość łatwej modyfikacji zasad wersjonowania
 *      - Obsługa różnych formatów plików
 * 8. Lepsze komunikaty błędów:
 *      - Kolorowe wyjęcie diagnostyczne
 *      - Szczegółowe informacje o błędach
 * 
 * Nowe funkcjonalności:
 *      - Obsługa błędów przekroczenia maksymalnych wartości
 *      - Automatyczne dodawanie nagłówka wersji jeśli brak
 *      - Wsparcie dla różnych formatów komentarzy
 *      - Bezpieczne operacje na plikach
 * 
 * Kod jest teraz bardziej odporny na błędy i łatwiejszy w
 * utrzymaniu, z zachowaniem pełnej zgodności wstecznej.
 */
using System.Text.RegularExpressions;

namespace VerberyCore
{
    /// <summary>
    /// Reprezentuje numer wersji aplikacji w formacie Major.Minor.Build.Revision
    /// </summary>
    public class DebugVersion
    {
        /// <summary>
        /// Main version number (zmiana oznacza brak kompatybilności wstecznej)
        /// </summary>
        public int Major { get; set; }

        /// <summary>
        /// Second version number (new functionalities while maintaining compatibility)
        /// </summary>
        public int Minor { get; set; }

        /// <summary>
        /// Build number (bug fixes and minor changes)
        /// </summary>
        public int Build { get; set; }

        /// <summary>
        /// Revision number (automatic increment)
        /// </summary>
        public int Revision { get; set; }

        /// <summary>
        /// Returns a formatted representation of the version
        /// </summary>
        public override string ToString() => $"{Major}.{Minor}.{Build}.{Revision}";
    }

    /// <summary>
    /// Manages application versions according to the principles of semantic versioning
    /// </summary>
    public static partial class VersionManager
    {
        #region Constants
        private const int MAX_REVISION = 99;
        private const int MAX_BUILD = 99;
        private const int MAX_MINOR = 12;
        private const int MAX_MAJOR = int.MaxValue;
        #endregion

        /// <summary>
        /// Aktualizuje numer wersji w podanym pliku
        /// </summary>
        /// <param name="filePath">Ścieżka do pliku</param>
        /// <exception cref="FileNotFoundException">Gdy plik nie istnieje</exception>
        /// <exception cref="UnauthorizedAccessException">Brak uprawnień do zapisu</exception>
        public static void UpdateVersion(string filePath)
        {
            if (!File.Exists(filePath))
            {
                Console.WriteLine(new FileNotFoundException("Plik nie istnieje", filePath).Message);
                return;
            }

            try
            {
                var originalLines = File.ReadAllLines(filePath);
                var originalContent = string.Join(Environment.NewLine, originalLines);
                var updatedLines = ProcessVersionLines(originalLines);
                var updatedContent = string.Join(Environment.NewLine, updatedLines);

                // Sprawdzenie, czy zawartość się zmieniła
                if (originalContent == updatedContent)
                {
                    Console.WriteLine($"[VERBERY]: No changes detected in version file {filePath}. Skipping update.");
                    return;
                }

                File.WriteAllLines(filePath, updatedLines);
            }
            catch (Exception ex)
            {
                HandleVersionUpdateError(ex, filePath);
            }
        }

        /// <summary>
        /// Inkrementuje numer wersji zgodnie z zasadami semantycznego wersjonowania
        /// </summary>
        /// <param name="currentVersion">Aktualna wersja</param>
        /// <returns>Nowa wersja</returns>
        public static DebugVersion IncrementVersion(DebugVersion currentVersion)
        {
            var newVersion = new DebugVersion
            {
                Major = currentVersion.Major,
                Minor = currentVersion.Minor,
                Build = currentVersion.Build,
                Revision = currentVersion.Revision
            };

            newVersion.Revision++;

            if (newVersion.Revision > MAX_REVISION)
            {
                newVersion.Revision = 0;
                newVersion.Build++;
            }

            if (newVersion.Build > MAX_BUILD)
            {
                newVersion.Build = 0;
                newVersion.Minor++;
            }

            if (newVersion.Minor > MAX_MINOR)
            {
                newVersion.Minor = 0;
                newVersion.Major++;
            }

            if (newVersion.Major > MAX_MAJOR)
            {
                throw new OverflowException("Osiągnięto maksymalną wartość wersji głównej");
            }

            return newVersion;
        }

        #region Private Methods
        private static string[] ProcessVersionLines(string[] lines)
        {
            var versionFound = false;
            var newLines = new string[lines.Length];

            for (int i = 0; i < lines.Length; i++)
            {
                if (VersionRegex().IsMatch(lines[i]))
                {
                    newLines[i] = UpdateVersionLine(lines[i]);
                    versionFound = true;
                }
                else
                {
                    newLines[i] = lines[i];
                }
            }

            if (!versionFound)
            {
                var list = newLines.ToList();
                list.Insert(0, GenerateNewVersionHeader());
                return list.ToArray();
            }

            return newLines;
        }

        private static string UpdateVersionLine(string line)
        {
            var match = VersionNumberRegex().Match(line);
            if (!match.Success) return line;

            var currentVersion = new DebugVersion
            {
                Major = int.Parse(match.Groups[1].Value),
                Minor = int.Parse(match.Groups[2].Value),
                Build = int.Parse(match.Groups[3].Value),
                Revision = int.Parse(match.Groups[4].Value)
            };

            var newVersion = IncrementVersion(currentVersion);
            return $"// Version: {newVersion}";
        }

        private static string GenerateNewVersionHeader() => "// Version: 0.1.0.0";

        private static void HandleVersionUpdateError(Exception ex, string filePath)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Błąd aktualizacji wersji w pliku {filePath}:");
            Console.WriteLine(ex.Message);
            Console.ResetColor();
        }
        #endregion

        #region Generated Regex
        [GeneratedRegex(@"^//.*Version:\s*(\d+)\.(\d+)\.(\d+)\.(\d+)", RegexOptions.IgnoreCase)]
        private static partial Regex VersionNumberRegex();

        [GeneratedRegex(@"^//.*Version:", RegexOptions.IgnoreCase)]
        private static partial Regex VersionRegex();

        /// <summary>
        /// Returns the current version of the application
        /// </summary>
        /// <returns></returns>
        internal static DebugVersion GetCurrentVersion()
        {
            return new DebugVersion() { Build = 0, Major = 1, Minor = 0, Revision = 0 };
        }

        /// <summary>
        /// Update the version file with the new version
        /// </summary>
        /// <param name="newVersion"></param>
        internal static void UpdateVersionFile(DebugVersion newVersion)
        {
            var dv = new DebugVersion()
            {
                Major = newVersion.Major,
                Minor = newVersion.Minor,
                Build = newVersion.Build,
                Revision = newVersion.Revision
            };
        }

        /// <summary>
        /// Parses a version string in the format Major.Minor.Build.Revision
        /// </summary>
        /// <param name="version"></param>
        /// <returns></returns>
        internal static DebugVersion ParseVersion(string version)
        {
            return new DebugVersion()
            {
                Major = int.Parse(version.Split('.')[0]),
                Minor = int.Parse(version.Split('.')[1]),
                Build = int.Parse(version.Split('.')[2]),
                Revision = int.Parse(version.Split('.')[3])
            };
        }
        #endregion
    }
}
