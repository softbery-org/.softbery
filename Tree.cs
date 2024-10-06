// Version: 10.0.0.214
// Version: 10.0.0.207
// Version: 10.0.0.206
// Version: 10.0.0.205
// Version: 10.0.0.203
// Version: 10.0.0.201
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace softbery
{
	public enum DataType
	{
		file,
		directory
	}

	public struct Tree
	{
		public string Name { get; set; }
		public string Path { get; set; }
		public DirectoryTree Directory { get; set; }
		public FileInfo Info { get; set; }
		public DataType FileType { get; set; }
		public string Hash { get; set; }
	}

	public struct DirectoryTree
	{
		public string Name { get; set; }
		public string Path { get; set; }
		public DirectoryInfo Info { get; set; }
		public DataType FileType { get; set; }
	}
}
