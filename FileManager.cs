// Version: 1.0.0.212
// Version: 1.0.0.205
// Version: 1.0.0.204
// Version: 1.0.0.203
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace softbery
{
	public static partial class FileManager
	{
		public static List<Tree> FileList { get => _trees; }

		private static List<Tree> _trees = [];

		public static List<Tree> GetDataTree(string path)
		{
			return GetDirTree(path); ;
		}

		private static List<Tree> GetDirTree(string path)
		{
			try
			{
				var di = new DirectoryInfo(path);
				DirectoryTree dt = new();

				dt.Name = di.FullName;
				dt.Path = di.FullName;
				dt.Info = di;
				dt.FileType = DataType.directory;

				var files = new DirectoryInfo(path).GetFiles();
				var dirs = new DirectoryInfo(path).GetDirectories();

				if (files != null)
				{
					foreach (var file in files)
					{
						var fi = new FileInfo(file.FullName);
						if (fi.Extension == ".cs")
						{
							Tree ft = new();
							ft.Name = fi.Name;
							ft.Path = fi.FullName;
							ft.FileType = DataType.file;
							ft.Info = fi;
							ft.Directory = dt;

							if (!_trees.Contains(ft))
							{
								Ver.UpdateVersion(ft.Path);
								_trees.Add(ft);
							}
						}
						else
						{
							Tree ft = new();
							ft.Name = fi.Name;
							ft.Path = fi.FullName;
							ft.FileType = DataType.file;
							ft.Info = fi;
							ft.Directory = dt;

							if (!_trees.Contains(ft))
							{
								_trees.Add(ft);
							}
						}
					}
				}

				if (dirs != null)
				{
					foreach (var dir in dirs)
					{
						GetDataTree(dir.FullName + "/");
					}
				}
			}
			catch (Exception ex) 
			{
				Console.WriteLine(ex.Message);
				Console.ReadLine();
			}

			return _trees;
		}

		private static void updateVersion(string file)
		{
			var text = File.ReadAllText(file);
			var lines = File.ReadAllLines(file);

			var pattern = "^.*//.*Version:.*(\\d+).(\\d+).(\\d+).(\\d+).*";
			var regex = new Regex(pattern);
			var match = regex.Matches(text);
			Console.WriteLine(match[0].Groups[0].Value.ToString());

			var count = 0;
						
			if (match.Count>0)
				count = lines.Length;
			else
				count = lines.Length+1;

			var temp = new string[count];

			if (match.Count>0)
				lines.CopyTo(temp, 0);
			else
				lines.CopyTo(temp, 1);

			var replace = "";


			if (match.Count>0)
			{
				replace = replaceVersion(match[0].Groups[0].Value.ToString());
				temp[0] = replace;
			}
			else
			{
				temp[0] = "// Version: 1.0.0.0";
			}

			try
			{
				File.WriteAllLines(file, temp);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
		}

		private static string replaceVersion(string group)
		{
			var r = new Regex("^.*//.*Version:").Replace(group, "");
			r = r.Replace(" ", "");
			var v = r.Split(".");
			
			var major = int.Parse(v[0]);
			var minor = int.Parse(v[1]);
			var build = int.Parse(v[2]);
			var revision = int.Parse(v[3]);

			var u = Ver.CountingVersion(new DebugVersion() { Major = major, Minor = minor, Build = build, Revision = revision, });

			return $"// Version: {u.Major}.{u.Minor}.{u.Build}.{u.Revision}";
		}

        [GeneratedRegex(@"^.*//.*Version:")]
        private static partial Regex ReplaceRegex();
    }
}
