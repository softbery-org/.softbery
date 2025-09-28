// Version: 0.1.0.5
/* 
 * LICENSE_MIT License
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
 * Email							    : VerberyCore@softbery.org
 * Description					    : 
 * Create						        : 2025-05-06 22:39:42
 * Last Modification Date    : 2025-05-06 22:39:42
 */

using System.CommandLine;
using System.CommandLine.IO;
using System.CommandLine.NamingConventionBinder;
using System.Reflection;

namespace VerberyCore
{
    /// <summary>
    /// Command-line interface for managing version control and file metadata.
    /// </summary>
    public class Cli
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Cli"/> class.
        /// </summary>
        public Cli(string[] args)
        {
            CommandRun(args).GetAwaiter();
        }

        /// <summary>
        /// Runs the command-line interface with the specified arguments.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        private static async Task<int> CommandRun(string[] args)
        {
            var rootCommand = new RootCommand("Intelligent version control and file metadata management")
            {
                new Option<bool>(new[] { "--verbose", "-v" }, "Enable detailed logging"),
                new Option<bool>(new[] { "--silent", "-s" }, "Suppress all output"),
                new Option<bool>(new[] { "--version", "-V" }, "Display current version"),
                new Option<bool>(new[] { "--help", "-h" }, "Show help information"),
                new Option<bool>(new[] { "--debug", "-d" }, "Enable debug mode")
            };

            rootCommand.AddCommand(BuildInitCommand());
            rootCommand.AddCommand(BuildScanCommand());
            rootCommand.AddCommand(BuildVersionCommand());
            rootCommand.AddCommand(BuildHashCommand());
            rootCommand.AddCommand(BuildBackupCommand());

            rootCommand.Handler = CommandHandler.Create<bool, bool, bool, IConsole>((verbose, silent, version, console) =>
            {
                Logger.Initialize(verbose, silent);

                if (version)
                {
                    Logger.Info($"Version: {Assembly.GetExecutingAssembly().GetName().Version}");
                    return 0;
                }
                if (console != null)
                {
                    console.Error.WriteLine("Specify a command");
                }
                return 1;
            });

            return await rootCommand.InvokeAsync(args);
        }

        private static Command BuildBackupCommand()
        {
            throw new NotImplementedException();
        }

        private static Command BuildHashCommand()
        {
            throw new NotImplementedException();
        }

        #region Command Builders
        private static Command BuildInitCommand()
        {
            var command = new Command("init", "Initialize new project")
            {
                new Argument<DirectoryInfo>("path", "Project directory"),
                new Option<string>(new[] { "--template", "-t" }, "Use predefined template"),
                new Option<bool>(new[] { "--force", "-f" }, "Overwrite existing config"),
                new Option<string>(new[] { "--vcs", "-c" }, "Initialize version control (git/svn)")
            };

            command.Handler = CommandHandler.Create<DirectoryInfo, string, bool, string>((path, template, force, vcs) =>
            {
                try
                {
                    Logger.Info($"Initializing project at: {path.FullName}");

                    if (!path.Exists) Directory.CreateDirectory(path.FullName);

                    if (force || !File.Exists(Path.Combine(path.FullName, ".sbconf")))
                    {
                        FileManager.CreateDefaultConfig(path);
                        Logger.Info("Configuration file created");
                    }

                    if (!string.IsNullOrEmpty(vcs))
                    {
                        VersionControlSystem.Initialize(vcs, path);
                        Logger.Info($"{vcs.ToUpper()} repository initialized");
                    }

                    return 0;
                }
                catch (Exception ex)
                {
                    Logger.Error($"Initialization failed: {ex.Message}");
                    return ex.HResult;
                }
            });

            return command;
        }

        private static Command BuildScanCommand()
        {
            var command = new Command("scan", "Analyze directory structure")
            {
                new Argument<DirectoryInfo>("path", () => new DirectoryInfo(Directory.GetCurrentDirectory())),
                new Option<bool>(new[] { "--deep", "-d" }, "Recursive directory analysis"),
                new Option<bool>(new[] { "--hash", "-h" }, "Generate file hashes"),
                new Option<bool>(new[] { "--tree", "-t" }, "Display directory tree"),
                new Option<string>(new[] { "--format", "-f" }, () => "text", "Output format")
            };

            command.Handler = CommandHandler.Create<DirectoryInfo, bool, bool, bool, string>((path, deep, hash, tree, format) =>
            {
                try
                {
                    Logger.Info($"Scanning directory: {path.FullName}");
                    var scanOptions = new ScanOptions
                    {
                        Recursive = deep,
                        GenerateHashes = hash,
                        OutputFormat = format
                    };

                    var result = FileManager.GetDataTree(path.FullName);

                    if (tree)
                    {
                        Console.WriteLine(result);
                    }

                    return 0;
                }
                catch (Exception ex)
                {
                    Logger.Error($"Scan failed: {ex.Message}");
                    return ex.HResult;
                }
            });

            return command;
        }

        private static Command BuildVersionCommand()
        {
            var bumpCommand = new Command("bump", "Increment version number")
            {
                new Option<bool>("--major", "Increment major version"),
                new Option<bool>("--minor", "Increment minor version"),
                new Option<bool>("--build", "Increment build number"),
                new Option<bool>("--revision", "Auto-increment revision")
            };

            bumpCommand.Handler = CommandHandler.Create<bool, bool, bool, bool>((major, minor, build, revision) =>
            {
                try
                {
                    var currentVersion = VersionManager.GetCurrentVersion();
                    var newVersion = VersionManager.IncrementVersion(currentVersion);
                    /*,
                        major ? VersionComponent.Major :
                        minor ? VersionComponent.Minor :
                        build ? VersionComponent.Build :
                        VersionComponent.Revision);*/
                    VersionManager.UpdateVersionFile(newVersion);
                    Logger.Info($"New version: {newVersion}");
                    return 0;
                }
                catch (Exception ex)
                {
                    Logger.Error($"Version bump failed: {ex.Message}");
                    return ex.HResult;
                }
            });

            // Use constructor overload to add the argument
            var setCommand = new Command("set", "Manually set version")
            {
                 new Argument<string>("version", "New version in format X.Y.Z.W")
            };
            {
                var handler = CommandHandler.Create<string>(version =>
                {
                    try
                    {
                        var newVersion = VersionManager.ParseVersion(version);
                        VersionManager.UpdateVersionFile(newVersion);
                        Logger.Info($"Version set to: {newVersion}");
                        return 0;
                    }
                    catch (Exception ex)
                    {
                        Logger.Error($"Invalid version: {ex.Message}");
                        return 1;
                    }
                });
            };

            var showCommand = new Command("show", "Display current version")
            {
                Handler = CommandHandler.Create(() =>
                {
                    var version = VersionManager.GetCurrentVersion();
                    Console.WriteLine(version);
                    return 0;
                })
            };

            var versionCommand = new Command("version", "Manage versioning operations")
            {
                bumpCommand,
                setCommand,
                showCommand
            };

            return versionCommand;
        }
        #endregion

        #region Helper Classes
        private static class Logger
        {
            private static bool _verbose;
            private static bool _silent;
            /// <summary>
            /// Initializes the logger with specified verbosity and silence options.`
            /// </summary>
            /// <param name="verbose"></param>
            /// <param name="silent"></param>
            public static void Initialize(bool verbose, bool silent)
            {
                _verbose = verbose;
                _silent = silent;
            }
            /// <summary>
            /// Logs an informational message to the standard output.
            /// </summary>
            /// <param name="message"></param>
            public static void Info(string message)
            {
                if (!_silent) Console.WriteLine($"[INFO] {message}");
            }
            /// <summary>
            /// Logs an error message to the standard error output.
            /// </summary>
            /// <param name="message"></param>
            public static void Error(string message)
            {
                if (!_silent) Console.Error.WriteLine($"[ERROR] {message}");
            }
            /// <summary>
            /// Logs a debug message if verbose mode is enabled.
            /// </summary>
            /// <param name="message"></param>
            public static void Debug(string message)
            {
                if (_verbose && !_silent) Console.WriteLine($"[DEBUG] {message}");
            }
        }

        /// <summary>
        /// Represents a version number with major, minor, build, and revision components.
        /// </summary>
        public enum VersionComponent { Major, Minor, Build, Revision }

        /// <summary>
        /// Represents options for scanning directories and files.
        /// </summary>
        public class ScanOptions
        {
            /// <summary>
            /// Indicates whether the scan should be recursive.
            /// </summary>
            public bool Recursive { get; set; }
            /// <summary>
            /// Indicates whether to generate hashes for files during the scan.
            /// </summary>
            public bool GenerateHashes { get; set; }
            /// <summary>
            /// Specifies the output format for the scan results.
            /// </summary>
            public string OutputFormat { get; set; }
        }
        #endregion
    }

    // Przykładowa implementacja pozostałych klas
    /// <summary>
    /// Represents a file manager that handles file operations and metadata.
    /// </summary>
    public static class VersionControlSystem
    {
        /// <summary>
        /// Initializes the version control system for the specified path.
        /// </summary>
        /// <param name="vcsType"></param>
        /// <param name="path"></param>
        public static void Initialize(string vcsType, DirectoryInfo path)
        {
            // Implementacja inicjalizacji VCS
        }
    }

    // Fix for CS0051: Make Program.ScanOptions public to match the accessibility of FileManager.GetDataTree
    /// <summary>
    /// Represents options for scanning directories and files.
    /// </summary>
    public class ScanOptions
    {
        /// <summary>
        /// Indicates whether the scan should be recursive.
        /// </summary>
        public bool Recursive { get; set; }
        /// <summary>
        /// Indicates whether to generate hashes for files during the scan.
        /// </summary>
        public bool GenerateHashes { get; set; }
        /// <summary>
        /// Specifies the output format for the scan results.
        /// </summary>
        public string OutputFormat { get; set; } = string.Empty;
    
    // Fix for CS1591: Add XML documentation for the public method FileManager.GetDataTree
    /// <summary>
    /// Retrieves a data tree representation of the specified directory path.
    /// </summary>
    /// <param name="path">The directory path to scan.</param>
    /// <param name="options">Options for scanning, such as recursion and hash generation.</param>
    /// <returns>A list of Tree objects representing the directory structure.</returns>
    public static List<Tree> GetDataTree(string path, ScanOptions options)
        {
            // Implementation of directory scanning
            return new List<Tree>();
        }

        // Fix for IDE0060: Ensure parameters are used or explicitly suppress the warning if they are part of the public API
        /// <summary>
        /// Retrieves a data tree representation of the specified directory path with API support.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="options"></param>
        /// <param name="api"></param>
        /// <returns></returns>
        public static List<Tree> GetDataTree(string path, ScanOptions options, object api)
        {
            // Use the parameters in the implementation or suppress the warning if they are intentionally unused
            _ = path; // Suppress IDE0060 for 'path'
            _ = options; // Suppress IDE0060 for 'options'

            // Implementation of directory scanning
            return new List<Tree>();
        }
    }
}
