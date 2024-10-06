// Version: 10.0.0.55
// Version: 10.0.0.48
// Version: 10.0.0.47
// Version: 10.0.0.46
// Version: 10.0.0.44
// Version: 10.0.0.42
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

        /// <summary>
        /// Read hash code from file
        /// </summary>
        /// <param name="file">fileinfo file</param>
        /// <returns>string</returns>
        public static string ReadMD5(this FileInfo file)
        {
            using var md5 = MD5.Create();
            using var stream = File.OpenRead(file.FullName);
            return Encoding.Default.GetString(md5.ComputeHash(stream));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static string GetMD5Checksum(this FileInfo file)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(file.FullName))
                {
                    var hash = md5.ComputeHash(stream);
                    return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                }
            }
        }
    }
}
