// Version: 1.0.0.9
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace softbery
{
    internal static class FileHash
    {
        /// <summary>
        /// Get / check MD5 file hash.
        /// </summary>
        /// <param name="file">FileInfo</param>
        /// <returns>File hash in string</returns>
        public static string CheckMD5(this FileInfo file)
        {
            using var md5 = MD5.Create();
            using var stream = File.OpenRead(file.FullName);
            return Encoding.Default.GetString(md5.ComputeHash(stream));
        }


    }
}
