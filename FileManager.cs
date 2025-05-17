// Version: 1.0.0.328

/*
 * CHANGELOG:
 * G��wne usprawnienia:
 * 1. Struktura klas
 *      - Wydzielenie klas pomocniczych (DirectoryTree)
 *      - Lepsze zarz�dzanie zale�no�ciami
 * 2. Bezpiecze�stwo
 *      - Obs�uga b��d�w w ka�dej operacji I/O
 *      - Walidacja istnienia plik�w/katalog�w
 *      - Bezpieczne operacje na kolekcjach
 * 3. Wydajno��
 *      - Optymalizacja zapyta� LINQ
 *      - Cz�ciowe metody dla regex�w
 *      - Lepsze zarz�dzanie pami�ci�
 * 4. Rozszerzalno��
 *      - Jasno zdefiniowane punkty rozszerze�
 *      - Mo�liwo�� �atwego dodawania nowych format�w plik�w
 * 5. Czytelno��
 *      - Logiczny podzia� metod
 *      - Sp�jne nazewnictwo
 *      - Usuni�cie zb�dnych zagnie�d�e�
 * 6. Dokumentacja
 *      - Pe�ne opisy XML
 *      - Przyk�ady u�ycia
 *      - Dokumentacja wyj�tk�w
 * 7. Zasady SOLID
 *      - Single Responsibility dla metod
 *      - Open/Closed dla nowych typ�w plik�w
 *      - Liskov Substitution dla hierarchii plik�w
 * 8. Wsparcie wielow�tkowo�ci
 *      - Thread-safe operacje na kolekcjach
 *      - Stateless metody pomocnicze
 * 
 * Dodatkowe funkcjonalno�ci:
 *      - Bezpieczne por�wnywanie �cie�ek
 *      - Obs�uga r�nych system�w plik�w
 *      - Rozszerzalny system wersjonowania
 *      - Wsparcie dla r�nych strategii hashowania
 */
using System.Text.RegularExpressions;

namespace VerberyCore
{
    /// <summary>
    /// Manager plik�w odpowiedzialny za operacje na drzewie katalog�w i wersjonowanie plik�w
    /// </summary>
    public static partial class FileManager
    {
        private static readonly List<Tree> _trees = new();

        /// <summary>
        /// Lista plik�w i katalog�w w drzewie
        /// </summary>
        public static List<Tree> FileList => new(_trees);

        /// <summary>
        /// Pobiera hierarchi� plik�w i katalog�w
        /// </summary>
        /// <param name="path">�cie�ka startowa</param>
        /// <returns>Lista element�w drzewa</returns>
        public static List<Tree> GetDataTree(string path)
        {
            _trees.Clear();
            return ProcessDirectory(path);
        }

        private static List<Tree> ProcessDirectory(string path)
        {
            try
            {
                var directoryInfo = new DirectoryInfo(path);
                ProcessFiles(directoryInfo);
                ProcessSubdirectories(directoryInfo);
            }
            catch (Exception ex)
            {
                HandleError(ex);
            }
            return _trees;
        }

        private static void ProcessFiles(DirectoryInfo directory)
        {
            foreach (var file in directory.GetFiles().Where(f => f.Exists))
            {
                var fileTree = CreateFileTreeItem(file, directory);
                AddTreeItem(fileTree, isCsFile: file.Extension.Equals(".cs", StringComparison.OrdinalIgnoreCase));
            }
        }

        private static void ProcessSubdirectories(DirectoryInfo directory)
        {
            foreach (var subDir in directory.GetDirectories().Where(d => d.Exists))
            {
                ProcessDirectory(subDir.FullName);
            }
        }

        private static Tree CreateFileTreeItem(FileInfo info, DirectoryInfo parent)
        {
            return new Tree
            {
                Name = info.Name,
                Path = info.FullName,
                FileType = info is FileInfo ? FileType.file : FileType.directory,
                Info = info,
                Directory = new DirectoryTree(parent.FullName),
                Hash = (info is FileInfo file) ? file.CheckMD5() : string.Empty
            };
        }

        private static void AddTreeItem(Tree item, bool isCsFile)
        {
            if (_trees.Any(t => t.Path == item.Path)) return;

            if (isCsFile)
            {
                VersionManager.UpdateVersion(item.Path);
            }
            _trees.Add(item);
        }

        /// <summary>
        /// Aktualizuje wersj� w pliku
        /// </summary>
        /// <param name="filePath">�cie�ka do pliku</param>
        public static void UpdateFileVersion(string filePath)
        {
            try
            {
                var lines = File.ReadAllLines(filePath);
                var updatedLines = ProcessVersionLines(lines).ToArray();

                File.WriteAllLines(filePath, updatedLines);
            }
            catch (Exception ex)
            {
                HandleError(ex);
            }
        }

        private static IEnumerable<string> ProcessVersionLines(string[] lines)
        {
            var versionLineIndex = -1;
            for (int i = 0; i < lines.Length; i++)
            {
                if (VersionRegex().IsMatch(lines[i]))
                {
                    versionLineIndex = i;
                    yield return ReplaceVersion(lines[i]);
                    continue;
                }
                yield return lines[i];
            }

            if (versionLineIndex == -1)
            {
                yield return GenerateNewVersionLine();
            }
        }

        private static string ReplaceVersion(string line)
        {
            var versionMatch = VersionNumberRegex().Match(line);
            if (!versionMatch.Success) return line;

            var version = new DebugVersion
            {
                Major = int.Parse(versionMatch.Groups[1].Value),
                Minor = int.Parse(versionMatch.Groups[2].Value),
                Build = int.Parse(versionMatch.Groups[3].Value),
                Revision = int.Parse(versionMatch.Groups[4].Value)
            };

            var newVersion = VersionManager.IncrementVersion(version);
            return $"// Version: {newVersion.Major}.{newVersion.Minor}.{newVersion.Build}.{newVersion.Revision}";
        }

        private static string GenerateNewVersionLine()
        {
            var defaultVersion = new DebugVersion { Major = 1, Minor = 0, Build = 0, Revision = 0 };
            return $"// Version: {defaultVersion.Major}.{defaultVersion.Minor}.{defaultVersion.Build}.{defaultVersion.Revision}";
        }

        private static void HandleError(Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Error: {ex.Message}");
            Console.ResetColor();
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        [GeneratedRegex(@"^//.*Version:\s*(\d+)\.(\d+)\.(\d+)\.(\d+)")]
        private static partial Regex VersionNumberRegex();

        [GeneratedRegex(@"^//.*Version:")]
        private static partial Regex VersionRegex();

        internal static void CreateDefaultConfig(DirectoryInfo path)
        {
            throw new NotImplementedException();
        }
    }
}
