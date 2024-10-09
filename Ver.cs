// Version: 1.0.0.118
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace softbery
{
    /* A class that contains the version of the program. */
    public class DebugVersion
    {
        /* A property of the class `Ver`. */
        public int Build { get; set; }
        /* A property of the class `Ver`. */
        public int Major { get; set; }
        /* A property of the class `Ver`. */
        public int Minor { get; set; }
        /* A property of the class `Ver`. */
        public int Revision { get; set; }
        /* A nullable array of integers. */
        public int[]? VersionTab { get; set; }
    }

    public static class Ver
    {
        /* A variable that is used to store the build version of the program. */
        static int BuildInt = 99;
        /* A variable that is used to store the major version of the program. */
        static int MajorInt = 0;
        /* A variable that is used to store the minor version of the program. */
        static int MinorInt = 9;
        /* A variable that is used to store the revision version of the program. */
        static int RevisionInt = 9999;

        public static void UpdateVersion(string file)
        {
            var text = File.ReadAllText(file);
            var lines = File.ReadAllLines(file);

            var pattern = "^.*//.*Version:.*(\\d+).(\\d+).(\\d+).(\\d+).*";
            var regex = new Regex(pattern);
            var match = regex.Matches(text);

            var count = 0;

            if (match.Count > 0)
                count = lines.Length;
            else
                count = lines.Length + 1;

            var temp = new string[count];

            if (match.Count > 0)
                lines.CopyTo(temp, 0);
            else
                lines.CopyTo(temp, 1);

            var replace = "";

            if (match.Count > 0)
            {
                replace = ReplaceVersion(match[0].Groups[0].Value.ToString());
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

        private static string ReplaceVersion(string text)
        {
            var r = new Regex(@"^.*//.*Version:").Replace(text, "");
            r = r.Replace(" ", "");
            var v = r.Split(".");

            var major = int.Parse(v[0]);
            var minor = int.Parse(v[1]);
            var build = int.Parse(v[2]);
            var revision = int.Parse(v[3]);

            var u = Ver.CountingVersion(new DebugVersion() { Major = major, Minor = minor, Build = build, Revision = revision, });

            return $"// Version: {u.Major}.{u.Minor}.{u.Build}.{u.Revision}";
        }

        public static DebugVersion CountingVersion(DebugVersion version)
        {
            version.Revision++;

            /* Checking if the revision is greater than 9999. */
            if (version.Revision > RevisionInt)
            {
                version.Revision = 0;
                version.Build++;
            }

            /* Checking if the build is greater than 99. */
            if (version.Build > BuildInt)
            {
                version.Build = 0;
                version.Minor++;
            }

            /* Checking if the minor version is greater than 12. */
            if (version.Minor > MinorInt)
            {
                version.Minor = 0;
                version.Major++;
            }

            /* Checking if the major version is less than 1. */
            if (version.Major <= MajorInt)
                version.Major++;

            return version;
        }
    }
}
