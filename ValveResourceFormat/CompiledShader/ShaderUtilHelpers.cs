using System;
using System.IO;
using System.Collections.Generic;
using ValveResourceFormat.Serialization.VfxEval;

namespace ValveResourceFormat.CompiledShader
{
    public static class ShaderUtilHelpers
    {
        public static (VcsFileType, VcsSourceType, VcsModelType) ComputeVCSFileName(string filenamepath)
        {
            VcsFileType vcsFileType = VcsFileType.Undetermined;
            VcsSourceType vcsSourceType = VcsSourceType.Undetermined;
            VcsModelType vcsModelType = VcsModelType.Undetermined;

            string[] fileTokens = Path.GetFileName(filenamepath).Split("_");
            if (fileTokens.Length < 4)
            {
                throw new ShaderParserException($"Filetype type unknown or not supported {filenamepath}");
            }
            vcsFileType = fileTokens[^1].ToLower() switch
            {
                "features.vcs" => VcsFileType.Features,
                "vs.vcs" => VcsFileType.VertexShader,
                "ps.vcs" => VcsFileType.PixelShader,
                "psrs.vcs" => VcsFileType.PixelShaderRenderState,
                "gs.vcs" => VcsFileType.GeometryShader,
                "cs.vcs" => VcsFileType.ComputeShader,
                "hs.vcs" => VcsFileType.HullShader,
                "ds.vcs" => VcsFileType.DomainShader,
                "rtx.vcs" => VcsFileType.RaytracingShader,
                _ => VcsFileType.Undetermined
            };
            vcsSourceType = fileTokens[^3].ToLower() switch
            {
                "pc" => VcsSourceType.PC,
                "pcgl" => VcsSourceType.PCGL,
                "gles" => VcsSourceType.MobileGles,
                "vulkan" => VcsSourceType.Vulkan,
                _ => VcsSourceType.Undetermined
            };
            if (vcsSourceType == VcsSourceType.Vulkan)
            {
                vcsSourceType = fileTokens[^4].ToLower() switch
                {
                    "android" => VcsSourceType.AndroidVulkan,
                    "ios" => VcsSourceType.IosVulkan,
                    _ => VcsSourceType.Vulkan
                };
            }
            vcsModelType = fileTokens[^2].ToLower() switch
            {
                "20" => VcsModelType._20,
                "2b" => VcsModelType._2b,
                "30" => VcsModelType._30,
                "31" => VcsModelType._31,
                "40" => VcsModelType._40,
                "41" => VcsModelType._41,
                "50" => VcsModelType._50,
                "60" => VcsModelType._60,
                _ => VcsModelType.Undetermined
            };
            if (vcsFileType == VcsFileType.Undetermined ||
                vcsSourceType == VcsSourceType.Undetermined ||
                vcsModelType == VcsModelType.Undetermined)
            {
                throw new ShaderParserException($"Filetype type unknown or not supported {filenamepath}");
            } else
            {
                return (vcsFileType, vcsSourceType, vcsModelType);
            }
        }

        public static string ShortenShaderParam(string shaderParam)
        {
            if (shaderParam.Length <= 4)
            {
                return shaderParam;
            }
            string[] splitName = shaderParam[2..].Split("_");
            string newName = "";
            if (splitName[0] == "MODE")
            {
                if (splitName.Length == 2)
                {
                    return splitName[1].Length > 3 ? $"M_{splitName[1][0..3]}" : $"M_{splitName[1]}";
                }
                newName = "M_";
                for (int i = 1; i < splitName.Length; i++)
                {
                    newName += splitName[i][0..1];
                }
                return newName;
            }
            if (splitName.Length > 2)
            {
                for (int i = 0; i < splitName.Length && i < 5; i++)
                {
                    newName += splitName[i].Substring(0, 1);
                }
                return newName;
            }
            if (splitName.Length == 1)
            {
                return splitName[0].Length > 4 ? splitName[0].Substring(0, 4) : splitName[0];
            }
            newName = splitName[0].Length > 3 ? splitName[0][0..3] : splitName[0];
            return $"{newName}_{splitName[1][0..1]}";
        }

        public static string CombineIntArray(int[] ints0, bool includeParenth = false)
        {
            if (ints0.Length == 0) return $"_";
            string valueString = "";
            foreach (int i in ints0)
            {
                valueString += $"{i},";
            }
            valueString = valueString[0..^1];
            return includeParenth ? $"({valueString})" : $"{valueString}";
        }

        public static string CombineIntsSpaceSep(int[] ints0, int padding = 5)
        {
            if (ints0.Length == 0) return $"_".PadLeft(padding);
            string valueString = "";
            foreach (int v in ints0)
            {
                string intPadded = $"{(v != 0 ? v : "_")}".PadLeft(padding);
                valueString += $"{intPadded}";
            }
            // return $"{valueString[0..^padding]}";
            return $"{valueString}";
        }

        public static string CombineStringsSpaceSep(string[] strings0, int padding = 5)
        {
            string combinedString = "";
            foreach (string s in strings0)
            {
                combinedString += s.PadLeft(padding);
            }
            return combinedString;
        }

        public static string CombineStringArray(string[] strings0, bool includeParenth = false)
        {
            if (strings0.Length == 0) return $"_";
            string combinedString = "";
            foreach (string s in strings0)
            {
                combinedString += $"{s}, ";
            }
            combinedString = combinedString[0..^2];
            return includeParenth ? $"({combinedString})" : $"{combinedString}";
        }

        public static string[] CombineValuesBreakString(string[] strings0, int breakLen)
        {
            List<string> stringCollection = new();
            if (strings0.Length == 0)
            {
                stringCollection.Add("");
                return stringCollection.ToArray();
            }
            string line = strings0[0] + ", ";
            for (int i = 1; i < strings0.Length; i++)
            {
                if (line.Length + strings0[i].Length + 1 < breakLen)
                {
                    line += strings0[i] + ", ";
                } else
                {
                    stringCollection.Add(line[0..^2]);
                    line = strings0[i] + ", ";
                }
            }
            if (line.Length > 0)
            {
                stringCollection.Add(line[0..^2]);
            }
            return stringCollection.ToArray(); ;
        }

        public static void ShowIntArray(int[] ints0, int padding = 5, string label = null, bool hex = false)
        {
            string intsString = "";
            foreach (int v in ints0)
            {
                string val = hex ? $"{v:x}" : $"{v}";
                intsString += $"{(v != 0 ? val : "_")}".PadLeft(padding);
            }
            string labelstr = (label != null && hex) ? $"{label}(0x)" : $"{label}";
            labelstr = label != null ? $"{labelstr,12} = " : "";
            Console.WriteLine($"{labelstr}{intsString.Trim()}");
        }

        public static string ParseDynamicExpression(byte[] dynExpDatabytes)
        {
            try
            {
                return new VfxEval(dynExpDatabytes).DynamicExpressionResult.Replace("UNKNOWN", "VAR"); ;
            } catch (Exception)
            {
                return "[error in dyn-exp]";
            }
        }

        public class OutputFormatterTabulatedData
        {
            private bool WriteToConsole = true;

            public OutputFormatterTabulatedData() { }
            public void Write(string text)
            {
                if (WriteToConsole)
                {
                    Console.Write(text);
                }
            }

            public void WriteLine(string text)
            {
                Write(text + "\n");
            }

            public void BreakLine()
            {
                Write("\n");
            }
            private List<string> headerValues;
            private List<List<string>> tabulatedValues;
            private List<int> columnWidths;

            public void DefineHeaders(string[] headers)
            {
                headerValues = new();
                tabulatedValues = new();
                columnWidths = new();
                foreach (string s in headers)
                {
                    headerValues.Add(s);
                    columnWidths.Add(s.Length);
                }
                tabulatedValues.Add(headerValues);
            }
            public void AddTabulatedRow(string[] rowMembers)
            {
                if (headerValues.Count != rowMembers.Length)
                {
                    throw new ShaderParserException("wrong number of columns");
                }
                List<string> newRow = new();
                List<List<string>> additionalRows = new();
                for (int i = 0; i < rowMembers.Length; i++)
                {
                    string[] multipleLines = rowMembers[i].Split("\n");
                    if (multipleLines.Length > 1)
                    {
                        AddExtraLines(additionalRows, multipleLines, i);
                    }

                    newRow.Add(multipleLines[0]);
                    if (multipleLines[0].Length > columnWidths[i])
                    {
                        columnWidths[i] = multipleLines[0].Length;
                    }
                }
                tabulatedValues.Add(newRow);
                foreach (var additionalRow in additionalRows)
                {
                    tabulatedValues.Add(additionalRow);
                }
            }
            private void AddExtraLines(List<List<string>> additionalRows, string[] multipleLines, int ind)
            {
                for (int i = 1; i < multipleLines.Length; i++)
                {
                    if (additionalRows.Count < i)
                    {
                        additionalRows.Add(EmptyRow());
                    }
                    additionalRows[i - 1][ind] = multipleLines[i];

                    if (multipleLines[i].Length > columnWidths[ind])
                    {
                        columnWidths[ind] = multipleLines[i].Length;
                    }
                }
            }
            private List<string> EmptyRow()
            {
                List<string> newRow = new();
                for (int i = 0; i < headerValues.Count; i++)
                {
                    newRow.Add("");
                }
                return newRow;
            }
            public void PrintTabulatedValues(int spacing = 2)
            {
                if (tabulatedValues.Count == 1 && tabulatedValues[0].Count == 0)
                {
                    return;
                }
                foreach (var row in tabulatedValues)
                {
                    for (int i = 0; i < row.Count; i++)
                    {
                        int pad = columnWidths[i] + spacing;
                        Write($"{row[i].PadRight(pad)}");
                    }
                    Write("\n");
                }
            }
        }


    }
}
