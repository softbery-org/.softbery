// Version: 1.0.0.156
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
 * Author						: Paweł Tobis
 * Email							: softbery@gmail.com
 * Description					:
 * Create						: 2024-10-01 04:31:42
 * Last Modification Date: 2024-01-30 19:58:04
 */

using Microsoft.Extensions.FileSystemGlobbing.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace softbery
{
    public class Program
    {
        private static List<Tree> _trees = new List<Tree>();

        public static void Main(string[] args)
        {
            try
            {
                FileInfo fi = new(".sbconf");
                List<FileInfo> lfi = new();
                lfi.Add(fi);
                //Config = new Conf(args);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            /* Add line to with version for each file */
            _trees = FileManager.GetDataTree("./");
            var tree = "";

            var i = 0;
            foreach (var item in _trees)
            {
                tree +=
                    $"ID: {i}{Environment.NewLine}" +
                    $"File name: {item.Name}{Environment.NewLine}" +
                    $"File path: {item.Path}{Environment.NewLine}" +
                    $"File type: {item.FileType.ToString()}{Environment.NewLine}" +
                    $"File hash: {item.Hash}{Environment.NewLine}" +
                    $"{Environment.NewLine}";
                i++;
            }

            File.WriteAllText(".sbver_files_temp", tree);

            // Compare temp and file, files.
            var read_temp_files = File.ReadAllText(".sbver_files_temp");
            var read_files = File.ReadAllText(".sbver_files");

            var patterns = new string[] { @"^.*File name: (.*)$", @"^.*File path: (.*)$", @"^.*File type: (.*)$", @"^.*File hash: (.*)$" };
            var listTemp = new List<Tree>();
            var list = new List<Tree>();

            var names = Regex.Matches(read_temp_files, patterns[0]);
            var paths = Regex.Matches(read_temp_files, patterns[1]);
            var types = Regex.Matches(read_temp_files, patterns[2]);
            var hashs = Regex.Matches(read_temp_files, patterns[3]);

            for (int j= 0; j < names.Count(); j++)
            {
                listTemp.Add(new Tree() { Name = names[j].Value, Path = paths[j].Value, Hash = hashs[j].Value });
            }

            names = Regex.Matches(read_files, patterns[0]);
            paths = Regex.Matches(read_files, patterns[1]);
            types = Regex.Matches(read_files, patterns[2]);
            hashs = Regex.Matches(read_files, patterns[3]);

            for (int j = 0; j < names.Count(); j++)
            {
                list.Add(new Tree() { Name = names[j].Value, Path = paths[j].Value, Hash = hashs[j].Value });
            }

            var result = new List<Tree>();

            foreach (var item in list)
            {
                if (!listTemp.Contains(item))
                { 
                    result.Add(item);
                }
            }

            var t = "";
            foreach (var res in result)
            {
                t += res.Name+Environment.NewLine;
                t += res.Hash + Environment.NewLine;
            }
            File.WriteAllText(".sbver_backup", t);

            File.Replace(".sbver_files_temp", ".sbver_files", null);

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
                ver.VersionTab = new int[splited.Count()];

                i = 0;
                /* Iterating through the array `splited`. */
                foreach (var item in splited)
                {
                    ver.VersionTab[i] = Convert.ToInt16(splited[i]);
                    i++;
                }

                /* Changing the color of the text. */
                Console.ForegroundColor = ConsoleColor.DarkMagenta;

                ver.Major = ver.VersionTab[0];
                ver.Minor = ver.VersionTab[1];
                ver.Build = ver.VersionTab[2];
                ver.Revision = ver.VersionTab[3];

                AssemblyName appname = typeof(Program).Assembly.GetName();
                Version appver = appname.Version != null ? appname.Version : new Version(1, 0, 0, 0);

                string name = "";
                string version = "";

                if (appname.Name != null && appname.Version != null)
                {
                    name = appname.Name.ToUpper();
                    version = appver.ToString();
                }
                else
                    Console.WriteLine("Application name or version has null value.");

                /* Printing the current version of the program. */
                Console.WriteLine($"{Environment.NewLine}{name.ToUpper()} ver.{version}{Environment.NewLine}{Environment.NewLine}" +
                    $"Current thread   : {Environment.CurrentManagedThreadId}{Environment.NewLine}" +
                    $"Current process : {Environment.ProcessId}{Environment.NewLine}" +
                    $"Current user      : {Environment.UserName}{Environment.NewLine}" +
                    $"Current OS        : {Environment.OSVersion}{Environment.NewLine}" +
                    $"Current version  : {ver.Major}.{ver.Minor}.{ver.Build}.{ver.Revision}");

                ver = Ver.CountingVersion(ver);

                /* Printing the new version of the program. */
                Console.WriteLine($"New version     : {ver.Major}.{ver.Minor}.{ver.Build}.{ver.Revision}");

                /* Checking if the file `.sbver` exists. */
                if (File.Exists(".sbver"))
                {
                    File.Delete(".sbver");
                }

                /* Writing the new version of the program to the file `.sbver`. */
                File.WriteAllText(".sbver", $"{ver.Major}.{ver.Minor}.{ver.Build}.{ver.Revision}");

                /* Printing the status of the program. */
                Console.WriteLine("Status: OK");
            }
        }
    }
}
