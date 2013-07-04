using System;
using System.IO;
using System.Security.Cryptography;

namespace KaBlooey.Engine
{
    public class FileComputeHasher
    {
        public FileComputeHasher()
        {
        }

        public static string ComputeHashFromFile(string fileName)
        {
            byte[] hash = MD5.Create().ComputeHash(File.ReadAllBytes(fileName));
            return BitConverter.ToString(hash);
        }
    }
}