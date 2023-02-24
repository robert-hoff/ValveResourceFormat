using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace MyShaderAnalysis.util
{
    internal class FixCsSources
    {
        // MyShaderAnalysis directory
        private static string SOURCE_DIR = @"../../";
        // private static string SOURCE_DIR = @"../../../TestVRF";
        private static string TEST_FILE = @"../../Program.cs";

        public static void Run()
        {
            FixAllFiles();
            // FixStylesForFile(TEST_FILE);
            // ShowFixesForFile(TEST_FILE);
            // ShowCsFilesWalkDirectory();
        }

        public static void CopyOverEditorConfigs() { }

        public static void FixAllFiles()
        {
            foreach (string filenamepath in GetCsFilesWalkDirectory(SOURCE_DIR))
            {
                FixStylesForFile(filenamepath);
            }
        }

        // replace line endings and trim trailing spaces
        public static void FixStylesForFile(string filenamepath)
        {
            List<string> sourceLines = GetModifiedSourceLines(filenamepath);
            FileWriter fw = new FileWriter(filenamepath);
            foreach (string line in sourceLines)
            {
                fw.WriteLine(line.TrimEnd());
            }
            fw.CloseStreamWriter();
        }

        public static void ShowFixesForFile(string filenamepath)
        {
            List<string> sourceLines = GetModifiedSourceLines(filenamepath);
            foreach (string item in sourceLines)
            {
                Debug.WriteLine($"{item}");
            }
            Debug.WriteLine($"END");
        }

        public static List<string> GetModifiedSourceLines(string filenamepath)
        {
            List<string> sourceLines = ReadFileAsStringList(filenamepath);
            sourceLines = RemoveTrailingSpaces(sourceLines);
            sourceLines = RemoveDoubleBlankLines(sourceLines);
            sourceLines = RemoveBlankLinesFollowingBracket(sourceLines);
            sourceLines = RemoveBlankLinesLeadingBracket(sourceLines);
            sourceLines = RemoveMultipleEndingSpaces(sourceLines);
            return sourceLines;
        }

        /*
         * Remove any blank lines found beyond the last two
         *
         */
        public static List<string> RemoveMultipleEndingSpaces(List<string> lines)
        {
            bool[] removeLines = new bool[lines.Count];
            int lastBracketFound = 999999999;
            for (int i = 0; i < lines.Count; i++)
            {
                if (lines[i].Length > 0 && lines[i][0] == '}' && lines[i].Trim().Equals("}"))
                {
                    if (lastBracketFound < 999999999)
                    {
                        throw new Exception("unexpected syntax");
                    }
                    lastBracketFound = i;
                }
                if (string.IsNullOrEmpty(lines[i].Trim()) && i > (lastBracketFound + 1))
                {
                    removeLines[i] = true;
                }
            }
            return ApplyRemoveListedIndexes(lines, removeLines);
        }

        /*
         * Remove any blank lines leading curly braket '}' (but not '{')
         *
         */
        public static List<string> RemoveBlankLinesLeadingBracket(List<string> lines)
        {
            bool[] removeLines = new bool[lines.Count];
            int prevSingleBracket = -1;
            for (int i = lines.Count - 1; i >= 0; i--)
            {
                if (lines[i].Length > 0 && lines[i].Trim().Equals("}"))
                {
                    prevSingleBracket = i;
                }
                if (prevSingleBracket == i + 1 && string.IsNullOrEmpty(lines[i].Trim()))
                {
                    removeLines[i] = true;
                    prevSingleBracket = i;
                }
            }
            return ApplyRemoveListedIndexes(lines, removeLines);
        }

        /*
         * Remove any blank lines following curly braket '{' (but not '}')
         *
         */
        public static List<string> RemoveBlankLinesFollowingBracket(List<string> lines)
        {
            bool[] removeLines = new bool[lines.Count];
            int prevSingleBracket = -1;
            for (int i = 0; i < lines.Count; i++)
            {
                if (lines[i].Length > 0 && lines[i].Trim().Equals("{"))
                {
                    prevSingleBracket = i;
                }
                if (prevSingleBracket == i - 1 && string.IsNullOrEmpty(lines[i].Trim()))
                {
                    removeLines[i] = true;
                    prevSingleBracket = i;
                }
            }
            return ApplyRemoveListedIndexes(lines, removeLines);
        }

        /*
         * removes double blanks (or multiple blank lines) and any leading
         * blank lines at the beginning of the file.
         *
         */
        public static List<string> RemoveDoubleBlankLines(List<string> lines)
        {
            bool[] removeLines = new bool[lines.Count];
            int prevBlank = -1;
            for (int i = 0; i < lines.Count; i++)
            {
                if (string.IsNullOrEmpty(lines[i].Trim()))
                {
                    if (i - 1 == prevBlank)
                    {
                        removeLines[i] = true;
                    }
                    prevBlank = i;
                }
                // stop at the last bracket
                if (lines[i].Length > 0 && lines[i][0].Equals('}'))
                {
                    break;
                }
            }
            return ApplyRemoveListedIndexes(lines, removeLines);
        }

        public static List<string> ApplyRemoveListedIndexes(List<string> lines, bool[] removeLines)
        {
            List<string> modifiedLines = new();
            for (int i = 0; i < removeLines.Length; i++)
            {
                if (!removeLines[i])
                {
                    modifiedLines.Add(lines[i]);
                }
            }
            return modifiedLines;
        }

        public static List<string> RemoveTrailingSpaces(List<string> lines)
        {
            List<string> modifiedLines = new();
            foreach (string line in lines)
            {
                string modifiedLine = line.TrimEnd();
                modifiedLines.Add(modifiedLine);
            }
            return modifiedLines;
        }

        public static List<string> ReadFileAsStringList(string filenamepath, bool omitEmptyLines = false)
        {
            List<string> data = new();
            string[] lines = File.ReadAllLines($"{filenamepath}");
            foreach (string line in lines)
            {
                if (!omitEmptyLines || !string.IsNullOrEmpty(line.Trim()))
                {
                    data.Add(line);
                }
            }
            return data;
        }

        public static void ShowCsFilesWalkDirectory()
        {
            foreach (string file in GetCsFilesWalkDirectory(SOURCE_DIR))
            {
                Debug.WriteLine($"{file}");
            }
        }

        public static string[] GetCsFilesWalkDirectory(string path)
        {
            return Directory.GetFileSystemEntries(path, "*.cs", SearchOption.AllDirectories);
        }
    }
}
