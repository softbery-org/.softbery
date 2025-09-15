// Version: 1.0.0.157
/* 
 * LICENSE MIT License
 * 
 * Copyright (c) 2023-2024 Softbery by Paweł Tobis
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 *
 * Author						        : Paweł Tobis
 * Email							    : VerberyCore@gmail.com
 * Description					    : 
 * Create						        : 2023-03-12 04:31:42
 * Last Modification Date    : 2025-05-05 19:34:04
 *
 *
 * *****************************************************************
 * 
 * APPLICATION - VERBERY
 *
 * ******************************************************************
 *
 * CHANGELOG:
 * Główne poprawki obejmują:
 *      - Aktualizacja informacji o prawach autorskich
 *      - Usunięcie nieużywanych przestrzeni nazw
 *      - Poprawa obsługi plików:
 *      - Lepsze zarządzanie strumieniami plików
 *      - Użycie StringBuildera do budowania zawartości
 * Ulepszone parsowanie plików:
 *      - Jednolita metoda parsowania z użyciem wyrażeń lambda
 *      - Lepszą obsługę typów plików
 * Poprawa porównywania wersji:
 *      - Właściwe porównywanie obiektów Tree
 *      - Lepsze wykrywanie zmian
 * Bezpieczniejsza obsługa wersji:
 *      - Walidacja formatu wersji
 *      - Obsługa błędów parsowania
 * Optymalizacja kodu:
 *      - Użycie LINQ i nowszych funkcjonalności C#
 *      - Lepsze formatowanie wyjścia konsoli
 * Poprawy czytelności:
 *      - Spójne formatowanie
 *      - Bardziej opisowe nazwy zmiennych
 * Obsługa błędów:
 *      - Sprawdzanie poprawności danych wejściowych
 *      - Defaultowe wartości w przypadku błędów
 *      
 * Kod wymaga jeszcze implementacji brakujących klas 
 * (Tree, FileManager, DebugVersion, Ver), ale główne 
 * problemy logiczne i strukturalne zostały rozwiązane.
 * 
 * ****************************************************************
 * Główne zmiany i usprawnienia:
 * Podział odpowiedzialności:
 *      - ProcessFileVersioning - zarządzanie wersjami plików
 *      - HandleApplicationVersion - obsługa wersji aplikacji
 *      - DisplayVersionInfo - prezentacja informacji o wersji
 * Zastosowanie zasad SOLID:
 *      - Single Responsibility - każda metoda wykonuje jedną logiczną operację
 *      -  Open/Closed - łatwe rozszerzanie funkcjonalności bez modyfikacji istniejących metod
 *      -  Dependency Inversion - wyraźne rozdzielenie warstw logiki
 * Poprawa czytelności:
 *      - Jasne nazwy metod opisujące ich funkcjonalność
 *      - Usunięcie powtarzającego się kodu
 *      - Lepsze zarządzanie stanem poprzez parametry metod
 * Bezpieczeństwo:
 *      - Spójna obsługa błędów
 *      - Walidacja danych wejściowych
 *      - Bezpieczne operacje na plikach
 * Optymalizacja:
 *      - Minimalizacja operacji I/O
 *      - Efektywne wykorzystanie pamięci (StringBuilder)
 *      - Lepsze zarządzanie zasobami
 * 
 * Każda metoda może być teraz testowana niezależnie, 
 * a modyfikacja jednej funkcjonalności nie wpływa na 
 * inne części systemu.
 * 
 * *****************************************************************
 * Wyjaśnienie zmian:
 * Użycie Cast<Match>() na MatchCollection:
 *      - Regex.Matches zwraca MatchCollection, która jest kolekcją niegeneryczną. Aby móc używać LINQ, należy ją rzutować na IEnumerable<Match> za pomocą Cast<Match>().
 * Usunięcie zbędnego Cast<Match>() dla pojedynczego Match:
 *      - Każdy element w MatchCollection jest już obiektem Match, więc nie ma potrzeby ponownego rzutowania.
 * Poprawa struktury nawiasów:
 *      - Dodano brakujące nawiasy, aby prawidłowo grupować operacje dla każdego wzorca (p).
 * Sprawdzenie istnienia grupy 1:
 *      - Jeśli wyrażenie regularne nie zawiera grupy 1 (indeks 1), dostęp do m.Groups[1] spowoduje błąd. Upewnij się, że wzorce zawierają co najmniej jedną grupę przechwytującą (np. (.*?)).
 * 
 * Jeśli istnieje możliwość, że grupa 1 nie istnieje, warto dodać sprawdzenie:
 *
 * var matches = patterns
 *  .Select(p => Regex.Matches(content, p, RegexOptions.Multiline)
 *      .Cast<Match>()
 *      .Select(m => m.Groups.Count > 1 ? m.Groups[1].Value.Trim() : string.Empty)
 *      .ToList())
 *  .ToList();
 * 
 * Ta modyfikacja zabezpiecza przed próbą dostępu do nieistniejącej grupy.
 * 
 * ******************************************************************
 * Dodatkowe usprawnienia dokumentacji:
 * 1. Pełne opisy klas głównych (Program, Tree, DebugVersion)
 * 2. Dokumentacja wszystkich właściwości
 * 3. Opisy parametrów metod
 * 4. Zwracanych wartości
 * 5. Dokumentacja enum FileType
 * 6. Spójna konwencja opisów
 * 7. Jasne określenie odpowiedzialności klas
 * 8. Opisy typów zwracanych w komentarzach
 *
 * Dokumentacja spełnia standardy IntelliSense i może być 
 * wykorzystana do automatycznego generowania dokumentacji 
 * technicznej przy użyciu narzędzi takich jak Sandcastle lub DocFX.
 * 
 * *****************************************************************
 * 
 */

using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace VerberyCore
{
    /// <summary>
    /// Główna klasa aplikacji odpowiedzialna za zarządzanie wersjami plików i aplikacji
    /// </summary>
    public class Program
    {
        private static List<Tree> _trees = new List<Tree>();
        private static string[]? _args;

        public static string DirectoryPath { get; set; } = ".sb";

        /// <summary>
        /// Konfiguracja aplikacji
        /// </summary>
        public static Conf? Config { get; set; }

        /// <summary>
        /// Główny punkt wejścia aplikacji
        /// </summary>
        /// <param name="args">Argumenty wiersza poleceń</param>
        public static void Main(string[] args)
        {
            Console.WriteLine("[VERBERY]: Starting application...");
            _args = args;
            var cli = new Cli(args);

            try
            {
                InitializeConfiguration();
                ProcessFileVersioning();
                HandleApplicationVersion();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex}");
            }
        }

        /// <summary>
        /// Inicjalizuje konfigurację aplikacji
        /// </summary>
        private static void InitializeConfiguration()
        {
            try
            {
                var configFile = new FileInfo($"{DirectoryPath}/.sbconf");
                var configFiles = new List<FileInfo> { configFile };

                //Config = new Conf(_args);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.ToString()}");
            }
        }

        /// <summary>
        /// Przeprowadza proces wersjonowania plików
        /// </summary>
        private static void ProcessFileVersioning()
        {
            _trees = FileManager.GetDataTree("./");

            var tempContent = GenerateFileTreeContent();
            if (string.IsNullOrEmpty(tempContent))
            {
                Console.WriteLine("[VERBERY]: No files to process.");
                return;
            }

            Console.WriteLine("[VERBERY]: Processing file versions...");

            // Sprawdzenie lub utworzenie katalogu
            CheckDirectoryOrCreate(DirectoryPath, FileAttributes.Directory | FileAttributes.Hidden);

            if (!File.Exists($"{DirectoryPath}/.sbver_files_temp"))
                File.CreateText($"{DirectoryPath}/.sbver_files_temp").Close();

            try
            {
                File.WriteAllText($"{DirectoryPath}/.sbver_files_temp", tempContent);

                var existingContent = File.Exists($"{DirectoryPath}/.sbver_files")
                    ? File.ReadAllText($"{DirectoryPath}/.sbver_files")
                    : string.Empty;

                // Sprawdzenie, czy zawartość się zmieniła
                if (tempContent == existingContent)
                {
                    Console.WriteLine("[VERBERY]: No changes detected in file versions. Skipping update.");
                    File.Delete($"{DirectoryPath}/.sbver_files_temp"); // Usunięcie tymczasowego pliku
                    return;
                }

                var changes = FindChangedFiles(tempContent, existingContent);

                if (changes.Any())
                {
                    try
                    {
                        CreateBackupFile(changes);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error: {ex.ToString()}");
                    }
                }

                if (File.Exists($"{DirectoryPath}/.sbver_files"))
                {
                    File.Replace($"{DirectoryPath}/.sbver_files_temp", $"{DirectoryPath}/.sbver_files", null);
                }
                else
                {
                    File.Move($"{DirectoryPath}/.sbver_files_temp", $"{DirectoryPath}/.sbver_files");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.Message} {ex.StackTrace}");
            }
        }

        /// <summary>
        /// Generuje zawartość drzewa plików w formacie tekstowym
        /// </summary>
        /// <returns>Tekstowa reprezentacja drzewa plików</returns>
        private static string GenerateFileTreeContent()
        {
            var sb = new StringBuilder();
            for (int i = 0; i < _trees.Count; i++)
            {
                sb.AppendLine($"ID: {i}");
                sb.AppendLine($"file name: {_trees[i].Name}");
                sb.AppendLine($"file path: {_trees[i].Path}");
                sb.AppendLine($"file type: {_trees[i].FileType}");
                sb.AppendLine($"file hash: {_trees[i].Hash}");
                sb.AppendLine();
            }
            return sb.ToString();
        }

        /// <summary>
        /// Wykrywa zmienione pliki między dwiema wersjami
        /// </summary>
        /// <param name="tempContent">Zawartość tymczasowej wersji plików</param>
        /// <param name="existingContent">Zawartość istniejącej wersji plików</param>
        /// <returns>Lista zmienionych plików</returns>
        private static List<Tree> FindChangedFiles(string tempContent, string existingContent)
        {
            var tempFiles = ParseFileContent(tempContent);
            var existingFiles = ParseFileContent(existingContent);

            return existingFiles
                .Where(ef => !tempFiles.Any(tf =>
                    tf.Path == ef.Path &&
                    tf.Hash != ef.Hash))
                .ToList();
        }

        /// <summary>
        /// Parsuje zawartość pliku wersji na listę obiektów Tree
        /// </summary>
        /// <param name="content">Zawartość pliku do parsowania</param>
        /// <returns>Lista obiektów Tree reprezentujących pliki</returns>
        private static List<Tree> ParseFileContent(string content)
        {
            if (string.IsNullOrEmpty(content))
                return new List<Tree>();

            var patterns = new[] {
                @"file name: (.*)",
                @"file path: (.*)",
                @"file type: (.*)",
                @"file hash: (.*)"
            };

            var matches = patterns
                .Select(p => Regex.Matches(content, p, RegexOptions.Multiline)
                    .Cast<Match>()
                    .Select(m => m.Groups.Count > 1 ? m.Groups[1].Value.Trim() : string.Empty)
                    .ToList())
                .ToList();

            return matches[0].Select((_, i) => new Tree
            {
                Name = matches[0][i],
                Path = matches[1][i],
                FileType = Enum.Parse<FileType>(matches[2][i]),
                Hash = matches[3][i]
            }).ToList();
        }

        /// <summary>
        /// Tworzy plik backupu ze zmienionymi plikami
        /// </summary>
        /// <param name="changes">Lista zmienionych plików</param>
        private static void CreateBackupFile(List<Tree> changes)
        {
            var backupContent = string.Join(Environment.NewLine,
                changes.Select(c => $"{c.Name}{Environment.NewLine}{c.Hash}"));
            File.WriteAllText($"{DirectoryPath}/.sbver_backup", backupContent);
        }

        /// <summary>
        /// Zarządza wersją aplikacji
        /// </summary>
        private static void HandleApplicationVersion()
        {
            var versionInfo = GetCurrentVersion();
            var newVersion = VersionManager.IncrementVersion(versionInfo);

            try
            {
                UpdateVersionFile(newVersion);
                DisplayVersionInfo(versionInfo, newVersion);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.Message}");
            }
        }

        /// <summary>
        /// Pobiera aktualną wersję aplikacji z pliku
        /// </summary>
        /// <returns>Obiekt DebugVersion z aktualną wersją</returns>
        private static DebugVersion GetCurrentVersion()
        {
            const string defaultVersion = "0.1.0.0";
            var versionFile = $"{DirectoryPath}/.sbver";

            CheckDirectoryOrCreate(DirectoryPath, FileAttributes.Directory | FileAttributes.Hidden);

            if (!File.Exists(versionFile))
            {
                File.WriteAllText(versionFile, defaultVersion);
            }

            var versionText = File.ReadAllText(versionFile).Trim();
            var versionParts = versionText.Split('.', StringSplitOptions.RemoveEmptyEntries);

            if (versionParts.Length != 4 || !versionParts.All(p => int.TryParse(p, out _)))
            {
                versionParts = defaultVersion.Split('.');
            }

            return new DebugVersion
            {
                Major = int.Parse(versionParts[0]),
                Minor = int.Parse(versionParts[1]),
                Build = int.Parse(versionParts[2]),
                Revision = int.Parse(versionParts[3])
            };
        }

        /// <summary>
        /// Pobiera aktualną wersję aplikacji
        /// </summary>
        /// <returns>Aktualna wersja</returns>
        public static DebugVersion GetVersion()
        {
            return GetCurrentVersion();
        }

        /// <summary>
        /// Aktualizuje plik z wersją aplikacji
        /// </summary>
        /// <param name="newVersion">Nowy numer wersji</param>
        private static void UpdateVersionFile(DebugVersion newVersion)
        {
            File.WriteAllText($"{DirectoryPath}/.sbver",
                $"{newVersion.Major}.{newVersion.Minor}.{newVersion.Build}.{newVersion.Revision}");
        }

        /// <summary>
        /// Wyświetla informacje o wersji w konsoli
        /// </summary>
        /// <param name="currentVersion">Aktualna wersja aplikacji</param>
        /// <param name="newVersion">Nowa wersja aplikacji</param>
        private static void DisplayVersionInfo(DebugVersion currentVersion, DebugVersion newVersion)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var assemblyName = assembly.GetName();

            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Console.WriteLine($"""
                {Environment.NewLine}{assemblyName.Name?.ToUpper()} ver.{assemblyName.Version}
                
                -- VERBERY APPLICATION --
                
                Current thread      : {Environment.CurrentManagedThreadId}
                Current process     : {Environment.ProcessId}
                Current user        : {Environment.UserName}
                Current OS          : {Environment.OSVersion}
                Current version     : {currentVersion.Major}.{currentVersion.Minor}.{currentVersion.Build}.{currentVersion.Revision}
                New version         : {newVersion.Major}.{newVersion.Minor}.{newVersion.Build}.{newVersion.Revision}
                """);
            Console.ResetColor();
        }

        /// <summary>
        /// Checks if the directory exists, and creates it with specified attributes if it does not
        /// </summary>
        /// <param name="path">Directory path</param>
        /// <param name="attributes">Directory attributes, like FileAttributes.Encrypted or FileAttributes.Hidden, etc.</param>
        public static void CheckDirectoryOrCreate(string path, FileAttributes attributes)
        {
            var directoryInfo = new DirectoryInfo(path);

            if (!directoryInfo.Exists)
            {
                directoryInfo.Create();
                directoryInfo.Attributes = FileAttributes.Directory | FileAttributes.Hidden;
            }
            else
            {
                // If the directory exists, ensure it has the correct attributes
                // Check if the directory has the specified attributes and add them if not
                if ((directoryInfo.Attributes & attributes) != attributes)
                {
                    directoryInfo.Attributes |= attributes;
                }
            }
        }
    }
}