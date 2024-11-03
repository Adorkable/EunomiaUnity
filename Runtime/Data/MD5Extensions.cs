using System;
using System.IO;
using System.Security.Cryptography;

// ReSharper disable once CheckNamespace
namespace EunomiaUnity
{
    public class MD5Extensions
    {
        public static string ComputeHashString(string fileName)
        {
            // using (var md5 = System.Security.Cryptography.MD5.Create()) {
            //     var inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
            //     var hashBytes = md5.ComputeHash(inputBytes);
            //     var sb = new System.Text.StringBuilder();
            //     for (int i = 0; i < hashBytes.Length; i++) {
            //         sb.Append(hashBytes[i].ToString("X2"));
            //     }
            //     return sb.ToString();
            // }
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(fileName))
                {
                    var hash = md5.ComputeHash(stream);
                    return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                }
            }
        }
    }
}