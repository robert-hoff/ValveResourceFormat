using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace MyShaderAnalysis.util
{
    class ReadDataFromFile
    {
        // static readonly string IMPORT_FOLDER = "../../../../input-samples/";
        public const string IMPORT_FOLDER = "../../../input/";
        public const string OUTPUT_FOLDER = "../../../../output/";

        public static List<int> ReadSingleColumnIntData(string filename, string folder = IMPORT_FOLDER)
        {
            List<int> data = new();
            string[] lines = File.ReadAllLines($"{folder}/{filename}");
            foreach (string line in lines)
            {
                data.Add(int.Parse(line));
            }
            return data;
        }
        public static List<string> ReadSingleColumnStringData(string filename, string folder = IMPORT_FOLDER)
        {
            List<string> data = new();
            string[] lines = File.ReadAllLines($"{folder}/{filename}");
            foreach (string line in lines)
            {
                if (line.Trim().Length > 0)
                {
                    data.Add(line.Trim());
                }
            }
            return data;
        }
        public static void SaveSingleColumnData(string filename, List<int> data, bool saveAsHex = false)
        {
            string filenamepath = $"{OUTPUT_FOLDER}/{filename}";
            FileWriter fileWriter = new FileWriter(filenamepath);
            foreach (int d in data)
            {
                if (saveAsHex)
                {
                    fileWriter.WriteLine($"0x{d:X}");
                }
                else
                {
                    fileWriter.WriteLine($"{d}");
                }
            }
            fileWriter.CloseStreamWriter();

            Debug.WriteLine($"data written to {filenamepath}");
        }
    }
}

