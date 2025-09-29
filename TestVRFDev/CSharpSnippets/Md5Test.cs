using System;
using System.Collections.Generic;
using System.Diagnostics;
// using System.Security.Cryptography;

#pragma warning disable CA5351 // Do Not Use Broken Cryptographic Algorithms
namespace TestVRFDev.CSharpSnippets
{
    public static class Md5Test
    {

        public static void Run()
        {
            Trial1();
        }

        public static void Trial1()
        {
            byte[] data = Array.Empty<byte>();
            // byte[] data = StringToByte(" ");
            byte[] result = System.Security.Cryptography.MD5.HashData(data);
            Debug.WriteLine($"{BytesToString(result)}");
        }

        public static byte[] StringToByte(string input)
        {
            List<byte> data = new();
            foreach (char c in input)
            {
                data.Add((byte) c);
            }
            return data.ToArray();
        }

        public static string BytesToString(byte[] databytes, int breakLen = 32)
        {
            if (databytes == null || databytes.Length == 0)
            {
                return "";
            }
            if (breakLen == -1)
            {
                breakLen = int.MaxValue;
            }
            var count = 0;
            var bytestring = "";
            for (var i = 0; i < databytes.Length; i++)
            {
                bytestring += $"{databytes[i]:X02} ";
                if (++count % breakLen == 0)
                {
                    bytestring += "\n";
                }
            }
            return bytestring.Trim();
        }

    }
}
