/* 
 * MIT License
 * 
 * Copyright (c) 2023 Softbery by Paweł Tobis
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
 * Author                : Paweł Tobis
 * Email                 : softbery@gmail.com
 * Description           :
 * Create                : 2023-02-24 04:31:42
 * Last Modification Date: 2023-03-02 19:58:04
 */

using System.Reflection;
using System.Reflection.Metadata;
using System.Text;

/* Creating a namespace called `Versioner`. */
namespace Versioner
{
	/* A class that contains the version of the program. */
	public class DebugVersion
	{
		/* A property of the class `Ver`. */
		public int build { get; set; }
		/* A property of the class `Ver`. */
		public int major { get; set; }
		/* A property of the class `Ver`. */
		public int minor { get; set; }
		/* A property of the class `Ver`. */
		public int revision { get; set; }
		/* A nullable array of integers. */
		public int[]? versiontab { get; set; }
	}

	/* The main class of the program. */
	public class Program
	{
		/* A variable that is used to store the build version of the program. */
		static int BuildInt = 99;
		/* A variable that is used to store the major version of the program. */
		static int MajorInt = 0;
		/* A variable that is used to store the minor version of the program. */
		static int MinorInt = 9;
		/* A variable that is used to store the revision version of the program. */
		static int RevisionInt = 999;
		/// <summary>
		/// The main function of the program.
		/// </summary>
		/// <param name="args">This is an array of strings that contains the command-line
		/// arguments.</param>
		public static void Main(string[] args)
		{
			/* Checking if the file `.sbver` exists. */
			if (!File.Exists(".sbver"))
			{
				File.WriteAllText(".sbver", $"1.0.0.0");
			}

			var txt = File.ReadAllLines(".sbver", Encoding.UTF8);
			/* Declaring a variable of type `string` and assigning it the value `""`. */
			string trimed = "";
			/* Iterating through the array `txt`. */
			foreach (var item in txt)
			{
				trimed = item.Trim();
			}

			/* Checking if the file `.sbver` is empty. */
			if (txt.Count() >= 0)
			{
				/* Splitting the string `trimed` by the character `.` and removing the empty entries. */
				var splited = trimed.Split(".", StringSplitOptions.RemoveEmptyEntries);
				/* Creating a new instance of the class `Ver`. */
				var ver = new DebugVersion();

				/* Creating a new array of integers with the size of the array `splited`. */
				ver.versiontab = new int[splited.Count()];

				int i = 0;
				/* Iterating through the array `splited`. */
				foreach (var item in splited)
				{
					ver.versiontab[i] = Convert.ToInt16(splited[i]);
					i++;
				}

				/* Changing the color of the text. */
				Console.ForegroundColor = ConsoleColor.DarkMagenta;

				ver.major = ver.versiontab[0];
				ver.minor = ver.versiontab[1];
				ver.build = ver.versiontab[2];
				ver.revision = ver.versiontab[3];

				AssemblyName appname = typeof(Program).Assembly.GetName();
				Version appver = appname.Version!=null ? appname.Version: new Version(1,0,0,0);

				string name="";
				string version="";
				
				if(appname.Name!=null && appname.Version!=null)
				{
					name = appname.Name.ToUpper();
					version = appver.ToString();
				}
				else
					Console.WriteLine("Application name or version has null value.");	
				
				/* Printing the current version of the program. */
				Console.WriteLine($"{Environment.NewLine}{name.ToUpper()} ver.{version}{Environment.NewLine}{Environment.NewLine}Current thread  : {Environment.CurrentManagedThreadId}{Environment.NewLine}Current process : {Environment.ProcessId}{Environment.NewLine}Current user    : {Environment.UserName}{Environment.NewLine}Current OS      : {Environment.OSVersion}{Environment.NewLine}Current version : {ver.major}.{ver.minor}.{ver.build}.{ver.revision}");

				ver = UpdateVersion(ver);

				/* Printing the new version of the program. */
				Console.WriteLine($"New version     : {ver.major}.{ver.minor}.{ver.build}.{ver.revision}");

				/* Checking if the file `.sbver` exists. */
				if (File.Exists(".sbver"))
				{
					File.Delete(".sbver");
				}

				/* Writing the new version of the program to the file `.sbver`. */
				File.WriteAllText(".sbver", $"{ver.major}.{ver.minor}.{ver.build}.{ver.revision}");

				/* Printing the status of the program. */
				Console.WriteLine("Status: OK");
			}
		}

		private static DebugVersion UpdateVersion(DebugVersion version)
		{
			var ver = new DebugVersion();
			ver.major = version.major;
			ver.minor = version.minor;
			ver.build = version.build;
			ver.revision = version.revision;

			ver.revision++;

			/* Checking if the revision is greater than 9999. */
			if (ver.revision > RevisionInt)
			{
				ver.revision = 0;
				ver.build++;
			}

			/* Checking if the build is greater than 99. */
			if (ver.build > BuildInt)
			{
				ver.build = 0;
				ver.minor++;
			}

			/* Checking if the minor version is greater than 12. */
			if (ver.minor > MinorInt)
			{
				ver.minor = 0;
				ver.major++;
			}

			/* Checking if the major version is less than 1. */
			if (ver.major <= MajorInt)
				ver.major++;

			return ver;
		}
	}
}