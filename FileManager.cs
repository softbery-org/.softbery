using System.IO;
using System.Text.RegularExpressions;
using System.Linq;

namespace VerberyCore
{
    /// <summary>
    /// Manager plików odpowiedzialny za operacje na drzewie katalogów i wersjonowanie plików
    /// </summary>
    public static partial class FileManager
    {
        private static readonly List<Tree> _trees = new();
        private static readonly List<string> _ignorePatterns = new();

        /// <summary>
        /// Lista plików i katalogów w drzewie
        /// </summary>
        public static List<Tree> FileList => new(_trees);

        /// <summary>
        /// Pobiera hierarchię plików i katalogów
        /// </summary>
        /// <param name="path">Ścieżka startowa</param>
        /// <returns>Lista elementów drzewa</returns>
        public static List<Tree> GetDataTree(string path)
        {
            _trees.Clear();
            LoadIgnorePatterns(path);
            if (!IgnoreDirectory(new DirectoryInfo(path)))
            {
                return ProcessDirectory(path);
            }
            return _trees;
        }

        /// <summary>
        /// Loads ignore patterns from the .sb/.ignore file
        /// </summary>
        /// <param name="basePath">Base path where the ignore file is located</param>
        private static void LoadIgnorePatterns(string basePath)
        {
            try
            {
                string ignoreFilePath = Path.Combine(basePath, ".sb", ".ignore");
                _ignorePatterns.Clear();

                if (File.Exists(ignoreFilePath))
                {
                    var patterns = File.ReadAllLines(ignoreFilePath)
                        .Select(line => line.Trim())
                        .Where(line => !string.IsNullOrEmpty(line) && !line.StartsWith("#"));

                    foreach (var pattern in patterns)
                    {
                        string normalizedPattern = pattern;
                        if (!pattern.StartsWith("/") && !pattern.StartsWith("\\"))
                        {
                            normalizedPattern = Path.Combine(basePath, pattern);
                        }
                        else
                        {
                            normalizedPattern = Path.Combine(basePath, pattern.Substring(1));
                        }
                        _ignorePatterns.Add(normalizedPattern.Replace("/", "\\"));
                    }
                }
            }
            catch (Exception ex)
            {
                HandleError(ex);
            }
        }

        private static bool IgnoreDirectory(DirectoryInfo dir)
        {
            try
            {
                string normalizedDirPath = dir.FullName.Replace("/", "\\");

                foreach (var pattern in _ignorePatterns)
                {
                    if (normalizedDirPath.Equals(pattern, StringComparison.OrdinalIgnoreCase) ||
                        normalizedDirPath.StartsWith(pattern + "\\", StringComparison.OrdinalIgnoreCase))
                    {
                        Console.WriteLine($"Ignoring directory: {dir.FullName}");
                        return true;
                    }

                    if (pattern.Contains("*") || pattern.Contains("?"))
                    {
                        string regexPattern = "^" + Regex.Escape(pattern).Replace("\\*", ".*").Replace("\\?", ".") + "$";
                        if (Regex.IsMatch(normalizedDirPath, regexPattern, RegexOptions.IgnoreCase))
                        {
                            Console.WriteLine($"Ignoring directory (pattern match): {dir.FullName}");
                            return true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                HandleError(ex);
            }
            return false;
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
            foreach (var file in directory.GetFiles().Where(f => f.Exists && !IgnoreFile(f) && IsFileAccessible(f)))
            {
                var fileTree = CreateFileTreeItem(file, directory);
                AddTreeItem(fileTree, isCsFile: file.Extension.Equals(".cs", StringComparison.OrdinalIgnoreCase));
            }
        }

        /// <summary>
        /// Checks if a file is accessible (not locked by another process)
        /// </summary>
        /// <param name="file">FileInfo object to check</param>
        /// <returns>True if the file is accessible, false otherwise</returns>
        private static bool IsFileAccessible(FileInfo file)
        {
            try
            {
                using (FileStream stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    stream.Close();
                }
                return true;
            }
            catch (IOException)
            {
                Console.WriteLine($"Skipping file (in use by another process): {file.FullName}");
                return false;
            }
            catch (Exception ex)
            {
                HandleError(ex);
                return false;
            }
        }

        private static bool IgnoreFile(FileInfo file)
        {
            try
            {
                string normalizedFilePath = file.FullName.Replace("/", "\\");

                foreach (var pattern in _ignorePatterns)
                {
                    if (normalizedFilePath.Equals(pattern, StringComparison.OrdinalIgnoreCase))
                    {
                        Console.WriteLine($"Ignoring file: {file.FullName}");
                        return true;
                    }

                    if (pattern.Contains("*") || pattern.Contains("?"))
                    {
                        string regexPattern = "^" + Regex.Escape(pattern).Replace("\\*", ".*").Replace("\\?", ".") + "$";
                        if (Regex.IsMatch(normalizedFilePath, regexPattern, RegexOptions.IgnoreCase))
                        {
                            Console.WriteLine($"Ignoring file (pattern match): {file.FullName}");
                            return true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                HandleError(ex);
            }
            return false;
        }

        private static void ProcessSubdirectories(DirectoryInfo directory)
        {
            if (!IgnoreDirectory(directory))
            {
                foreach (var subDir in directory.GetDirectories().Where(d => d.Exists))
                {
                    ProcessDirectory(subDir.FullName);
                }
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
                Hash = (info is FileInfo file && IsFileAccessible(file)) ? file.CheckMD5() : string.Empty
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
        /// Aktualizuje wersję w pliku
        /// </summary>
        /// <param name="filePath">Ścieżka do pliku</param>
        public static void UpdateFileVersion(string filePath)
        {
            try
            {
                var fileInfo = new FileInfo(filePath);
                if (!IsFileAccessible(fileInfo))
                {
                    Console.WriteLine($"Cannot update version for {filePath}: File is in use.");
                    return;
                }

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
            var defaultVersion = new DebugVersion { Major = 0, Minor = 1, Build = 0, Revision = 0 };
            return $"// Version: {defaultVersion.Major}.{defaultVersion.Minor}.{defaultVersion.Build}.{defaultVersion.Revision}";
        }

        private static void HandleError(Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Error: {ex.Message}");
            Console.ResetColor();
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