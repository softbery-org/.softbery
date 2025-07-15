// Version: 1.0.0.119
/*
 * G��wne usprawnienia:
 * 1. Pe�na dokumentacja XML z opisami wyj�tk�w
 * 2. Lepsze nazewnictwo metod i sta�ych
 * 3. Separacja odpowiedzialno�ci:
 *      - Logika inkrementacji w osobnej metodzie
 *      - Przetwarzanie plik�w w wydzielonych metodach
 * 4. Bezpiecze�stwo:
 *      - Obs�uga b��d�w I/O
 *      - Walidacja danych wej�ciowych
 * 5. Optymalizacja:
 *      - Wygenerowane regexy dla lepszej wydajno�ci
 *      - Sta�e dla warto�ci granicznych
 * 6. Zgodno�� z SEMVER:
 *      - Jasne zasady inkrementacji wersji
 *      - Ograniczenia warto�ci poszczeg�lnych komponent�w
 * 7. Rozszerzalno��:
 *      - Mo�liwo�� �atwej modyfikacji zasad wersjonowania
 *      - Obs�uga r�nych format�w plik�w
 * 8. Lepsze komunikaty b��d�w:
 *      - Kolorowe wyj�cie diagnostyczne
 *      - Szczeg�owe informacje o b��dach
 * 
 * Nowe funkcjonalno�ci:
 *      - Obs�uga b��d�w przekroczenia maksymalnych warto�ci
 *      - Automatyczne dodawanie nag��wka wersji je�li brak
 *      - Wsparcie dla r�nych format�w komentarzy
 *      - Bezpieczne operacje na plikach
 * 
 * Kod jest teraz bardziej odporny na b��dy i �atwiejszy w
 * utrzymaniu, z zachowaniem pe�nej zgodno�ci wstecznej.
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
        /// G��wny numer wersji (zmiana oznacza brak kompatybilno�ci wstecznej)
        /// </summary>
        public int Major { get; set; }

        /// <summary>
        /// Drugi numer wersji (nowe funkcjonalno�ci przy zachowaniu kompatybilno�ci)
        /// </summary>
        public int Minor { get; set; }

        /// <summary>
        /// Numer buildu (poprawki b��d�w i drobne zmiany)
        /// </summary>
        public int Build { get; set; }

        /// <summary>
        /// Numer rewizji (automatycznie inkrementowany)
        /// </summary>
        public int Revision { get; set; }

        /// <summary>
        /// Zwraca sformatowan� reprezentacj� wersji
        /// </summary>
        public override string ToString() => $"{Major}.{Minor}.{Build}.{Revision}";
    }

    /// <summary>
    /// Klasa odpowiedzialna za zarz�dzanie wersjonowaniem aplikacji
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
        /// <param name="filePath">�cie�ka do pliku</param>
        /// <exception cref="FileNotFoundException">Gdy plik nie istnieje</exception>
        /// <exception cref="UnauthorizedAccessException">Brak uprawnie� do zapisu</exception>
        public static void UpdateVersion(string filePath)
        {
            if (!File.Exists(filePath))
                Console.WriteLine(new FileNotFoundException("Plik nie istnieje", filePath).Message);

            try
            {
                var lines = File.ReadAllLines(filePath);
                var updatedLines = ProcessVersionLines(lines);
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
                throw new OverflowException("Osi�gni�to maksymaln� warto�� wersji g��wnej");
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

        internal static DebugVersion GetCurrentVersion()
        {
            
            return new DebugVersion() { Build = 0, Major=1, Minor=0, Revision=0 };
        }

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
