using System;
using System.Collections.Generic;
using System.IO;
using MyShaderFile.Serialization.VfxEval;
using static MyShaderFile.CompiledShader.ShaderDataReader;

namespace MyShaderFile.CompiledShader
{
    public static class ShaderUtilHelpers
    {
        public static (VcsProgramType ProgramType, VcsPlatformType PlatformType, VcsShaderModelType ShaderModelType) ComputeVCSFileName(string filenamepath)
        {
            var fileTokens = Path.GetFileNameWithoutExtension(filenamepath).Split("_");
            if (fileTokens.Length < 4)
            {
                throw new ShaderParserException($"Filetype type unknown or not supported {filenamepath}");
            }
            var vcsProgramType = fileTokens[^1].ToLowerInvariant() switch
            {
                "features" => VcsProgramType.Features,
                "vs" => VcsProgramType.VertexShader,
                "ps" => VcsProgramType.PixelShader,
                "psrs" => VcsProgramType.PixelShaderRenderState,
                "gs" => VcsProgramType.GeometryShader,
                "cs" => VcsProgramType.ComputeShader,
                "hs" => VcsProgramType.HullShader,
                "ds" => VcsProgramType.DomainShader,
                "rtx" => VcsProgramType.RaytracingShader,
                _ => VcsProgramType.Undetermined
            };


            var vcsPlatformType = fileTokens[^3].ToLowerInvariant() switch
            {
                "pc" => VcsPlatformType.PC,
                "pcgl" => VcsPlatformType.PCGL,
                "gles" => VcsPlatformType.MOBILE_GLES,
                "vulkan" => VcsPlatformType.VULKAN,
                _ => VcsPlatformType.Undetermined
            };

            if (vcsPlatformType == VcsPlatformType.VULKAN)
            {
                vcsPlatformType = fileTokens[^4].ToLowerInvariant() switch
                {
                    "android" => VcsPlatformType.ANDROID_VULKAN,
                    "ios" => VcsPlatformType.IOS_VULKAN,
                    _ => VcsPlatformType.VULKAN
                };
            }

            var vcsShaderModelType = fileTokens[^2].ToLowerInvariant() switch
            {
                "20" => VcsShaderModelType._20,
                "2b" => VcsShaderModelType._2b,
                "30" => VcsShaderModelType._30,
                "31" => VcsShaderModelType._31,
                "40" => VcsShaderModelType._40,
                "41" => VcsShaderModelType._41,
                "50" => VcsShaderModelType._50,
                "60" => VcsShaderModelType._60,
                _ => VcsShaderModelType.Undetermined
            };

            if (vcsProgramType == VcsProgramType.Undetermined ||
                vcsPlatformType == VcsPlatformType.Undetermined ||
                vcsShaderModelType == VcsShaderModelType.Undetermined)
            {
                throw new ShaderParserException($"Filetype type unknown or not supported {filenamepath}");
            }
            else
            {
                return (vcsProgramType, vcsPlatformType, vcsShaderModelType);
            }
        }

        public static VcsProgramType ComputeVcsProgramType(string filenamepath)
        {
            return Path.GetFileName(filenamepath).Split("_").Length < 4 ?
                VcsProgramType.Undetermined
            :
             Path.GetFileName(filenamepath).Split("_")[^1].ToLowerInvariant() switch
             {
                 "features.vcs" => VcsProgramType.Features,
                 "vs.vcs" => VcsProgramType.VertexShader,
                 "ps.vcs" => VcsProgramType.PixelShader,
                 "psrs.vcs" => VcsProgramType.PixelShaderRenderState,
                 "gs.vcs" => VcsProgramType.GeometryShader,
                 "cs.vcs" => VcsProgramType.ComputeShader,
                 "hs.vcs" => VcsProgramType.HullShader,
                 "ds.vcs" => VcsProgramType.DomainShader,
                 "rtx.vcs" => VcsProgramType.RaytracingShader,
                 _ => VcsProgramType.Undetermined
             };
        }

        public static string ShortenShaderParam(string shaderParam)
        {
            if (shaderParam.Length <= 4)
            {
                return shaderParam;
            }
            var splitName = shaderParam[2..].Split("_");
            var newName = "";
            if (splitName[0] == "MODE")
            {
                if (splitName.Length == 2)
                {
                    return splitName[1].Length > 3 ? $"M_{splitName[1][0..3]}" : $"M_{splitName[1]}";
                }
                newName = "M_";
                for (var i = 1; i < splitName.Length; i++)
                {
                    newName += splitName[i][0..1];
                }
                return newName;
            }
            if (splitName.Length > 2)
            {
                for (var i = 0; i < splitName.Length && i < 5; i++)
                {
                    newName += splitName[i][..1];
                }
                return newName;
            }
            if (splitName.Length == 1)
            {
                return splitName[0].Length > 4 ? splitName[0][..4] : splitName[0];
            }
            newName = splitName[0].Length > 3 ? splitName[0][0..3] : splitName[0];
            return $"{newName}_{splitName[1][0..1]}";
        }

        public static string CombineIntArray(int[] ints0, bool includeParenth = false)
        {
            if (ints0.Length == 0)
            {
                return $"_";
            }

            var valueString = "";
            foreach (var i in ints0)
            {
                valueString += $"{i},";
            }
            valueString = valueString[0..^1];
            return includeParenth ? $"({valueString})" : $"{valueString}";
        }

        public static string CombineIntsSpaceSep(int[] ints0, int padding = 5)
        {
            if (ints0.Length == 0)
            {
                return $"_".PadLeft(padding);
            }

            var valueString = "";
            foreach (var v in ints0)
            {
                var intPadded = $"{(v != 0 ? v : "_")}".PadLeft(padding);
                valueString += $"{intPadded}";
            }
            // return $"{valueString[0..^padding]}";
            return $"{valueString}";
        }

        public static string CombineStringsSpaceSep(string[] strings0, int padding = 5)
        {
            var combinedString = "";
            foreach (var s in strings0)
            {
                combinedString += s.PadLeft(padding);
            }
            return combinedString;
        }

        public static string CombineStringArray(string[] strings0, bool includeParenth = false)
        {
            if (strings0.Length == 0)
            {
                return $"_";
            }

            var combinedString = "";
            foreach (var s in strings0)
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
            var line = strings0[0] + ", ";
            for (var i = 1; i < strings0.Length; i++)
            {
                if (line.Length + strings0[i].Length + 1 < breakLen)
                {
                    line += strings0[i] + ", ";
                }
                else
                {
                    stringCollection.Add(line[0..^2]);
                    line = strings0[i] + ", ";
                }
            }
            if (line.Length > 0)
            {
                stringCollection.Add(line[0..^2]);
            }
            return stringCollection.ToArray();
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

        public static void ShowIntArray(int[] ints0, int padding = 5, string label = null, bool hex = false)
        {
            var intsString = "";
            foreach (var v in ints0)
            {
                var val = hex ? $"{v:x}" : $"{v}";
                intsString += $"{(v != 0 ? val : "_")}".PadLeft(padding);
            }
            var labelstr = (label != null && hex) ? $"{label}(0x)" : $"{label}";
            labelstr = label != null ? $"{labelstr,12} = " : "";
            Console.WriteLine($"{labelstr}{intsString.Trim()}");
        }

        public static string ParseDynamicExpression(byte[] dynExpDatabytes)
        {
            try
            {
                return new VfxEval(dynExpDatabytes, omitReturnStatement: true).DynamicExpressionResult.Replace("UNKNOWN", "VAR", StringComparison.InvariantCulture);
            }
            catch (Exception)
            {
                return "[error in dyn-exp]";
            }
        }

        public class OutputFormatterTabulatedData
        {
            public HandleOutputWrite OutputWriter { get; set; }

            public OutputFormatterTabulatedData(HandleOutputWrite OutputWriter = null)
            {
                this.OutputWriter = OutputWriter ?? ((x) => { Console.Write(x); });
            }

            public void Write(string text)
            {
                OutputWriter(text);
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
                foreach (var s in headers)
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
                for (var i = 0; i < rowMembers.Length; i++)
                {
                    var multipleLines = rowMembers[i].Split("\n");
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
                for (var i = 1; i < multipleLines.Length; i++)
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
                for (var i = 0; i < headerValues.Count; i++)
                {
                    newRow.Add("");
                }
                return newRow;
            }

            public List<string> BuildTabulatedRows(int spacing = 2, bool reverse = false)
            {
                List<string> tabbedRows = new();
                if (tabulatedValues.Count == 1 && tabulatedValues[0].Count == 0)
                {
                    return tabbedRows;
                }
                foreach (var rowTokens in tabulatedValues)
                {
                    var tabbedRow = "";
                    for (var i = 0; i < rowTokens.Count; i++)
                    {
                        var pad = columnWidths[i] + spacing;
                        tabbedRow += $"{rowTokens[i].PadRight(pad)}";
                    }
                    if (tabbedRow.Length > 0)
                    {
                        tabbedRows.Add(tabbedRow[..^spacing]);
                    }
                }
                if (reverse)
                {
                    tabbedRows.Reverse();
                }
                return tabbedRows;
            }

            public void PrintTabulatedValues(int spacing = 2)
            {
                List<string> tabbedRows = BuildTabulatedRows(spacing);
                foreach (var row in tabbedRows)
                {
                    WriteLine(row);
                }
            }
        }
    }
}
