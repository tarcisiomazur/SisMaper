using System;

namespace SisMaper.Tools
{
    public static class Encrypt
    {
        public static string ToSha512(string input)
        {
            var bytes = System.Text.Encoding.UTF8.GetBytes(input);
            using var sha512 = System.Security.Cryptography.SHA512.Create();
            var hash = sha512.ComputeHash(bytes);
            return BitConverter.ToString(hash).Replace("-", "");
        }
    }
}