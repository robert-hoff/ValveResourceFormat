using System.Collections.Generic;

namespace MyShaderAnalysis.util
{
    public class SaveDataToFile
    {
        private const string DEFAULT_OUTPUT_DIR = "../../../data-output-samples";

        public static void SaveSingleColumnIntData(string filename, ICollection<int> data, bool saveAsHex = false)
        {
            string filenamepath = $"{DEFAULT_OUTPUT_DIR}/{filename}";
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
        }

        public static void SaveSingleColumnStringData(string filename, ICollection<string> data)
        {
            string filenamepath = $"{DEFAULT_OUTPUT_DIR}/{filename}";
            FileWriter fileWriter = new FileWriter(filenamepath);
            foreach (string d in data)
            {
                fileWriter.WriteLine($"{d}");
            }
            fileWriter.CloseStreamWriter();
        }
    }
}


