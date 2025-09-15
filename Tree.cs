// Version: 1.0.1.0
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VerberyCore
{
    /// <summary>
    /// Typy plik�w obs�ugiwane przez system
    /// </summary>
    public enum FileType
	{
        /// <summary>
        /// Zwyk�y plik
        /// </summary>
        file,
        /// <summary>
        /// Katalog
        /// </summary>
        directory,
        /// <summary>
        /// Link symboliczny
        /// </summary>
        symlink
    }

    /// <summary>
    /// Reprezentuje pojedynczy plik w systemie wersjonowania
    /// </summary>
    public class Tree
	{
        /// <summary>
        /// Nazwa pliku
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// �cie�ka do pliku
        /// </summary>
        public string Path { get; set; } = string.Empty;

        /// <summary>
        /// Typ pliku (enum FileType)
        /// </summary>
        public FileType FileType { get; set; }

        /// <summary>
        /// Skr�t pliku (hash)
        /// </summary>
        public string? Hash { get; set; }
        /// <summary>
        /// Drzewo katalog�w
        /// </summary>
        public DirectoryTree? Directory { get; set; }
        /// <summary>
        /// Informacje o pliku
        /// </summary>
        public FileInfo? Info { get; set; }
        /// <summary>
        /// Pe�na �cie�ka do pliku
        /// </summary>
        public string? FullPath { get; set; }
        /// <summary>
        /// Wersja pliku
        /// </summary>
        public string? Version { get; set; }
    }

    /// <summary>
    /// Reprezentuje drzewo katalog�w
    /// </summary>
	public class DirectoryTree
	{
        /// <summary>
        /// Nazwa katalogu
        /// </summary>
		public string Name { get; set; }
        /// <summary>
        /// �cie�ka do katalogu
        /// </summary>
		public string Path { get; set; }
        /// <summary>
        /// Informacja o katalogu
        /// </summary>
		public DirectoryInfo Info { get; set; }
        /// <summary>
        /// Typ katalogu (enum FileType)
        /// </summary>
		public FileType FileType { get; set; }

        /// <summary>
        /// Inicjalizuje nowy obiekt DirectoryTree
        /// </summary>
        /// <param name="path">�cie�ka do listy katalogu.</param>
        public DirectoryTree(string path)
        {
            Path = path;
            Info = new DirectoryInfo(path);
            Name = Info.Name;
            FileType = FileType.directory;
        }

    }
}
