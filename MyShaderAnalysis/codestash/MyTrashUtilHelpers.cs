using System;
using System.Collections.Generic;
using System.IO;
using ValveResourceFormat.CompiledShader;
using ValveResourceFormat.Serialization.VfxEval;

namespace MyShaderAnalysis.codestash
{
    public class MyTrashUtilHelpers
    {
        public static ShaderFile InstantiateShaderFile(string filenamepath)
        {
            ShaderFile shaderFile = new ShaderFile();
            shaderFile.Read(filenamepath);
            return shaderFile;
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
            int count = 0;
            string bytestring = "";
            for (int i = 0; i < databytes.Length; i++)
            {
                bytestring += $"{databytes[i]:X02} ";
                if (++count % breakLen == 0)
                {
                    bytestring += "\n";
                }
            }
            return bytestring.Trim();
        }

        public static List<string> GetVcsFiles(string dir1, VcsProgramType fileType, int numEnding = -1, bool sortFiles = true, int LIMIT_NR = 1000)
        {
            return GetVcsFiles(dir1, null, fileType, numEnding, sortFiles, LIMIT_NR);
        }

        public static List<string> GetVcsFiles(string dir1, string dir2 = null,
            VcsProgramType fileType = VcsProgramType.Undetermined, int numEnding = -1, bool sortFiles = true, int LIMIT_NR = 1000)
        {
            List<string> filesFound = new();
            if (fileType == VcsProgramType.Features || fileType == VcsProgramType.Undetermined)
            {
                string endsWith = numEnding > -1 ? $"{numEnding}_features.vcs" : "features.vcs";
                filesFound.AddRange(GetAllFilesWithEnding(dir1, endsWith));
                filesFound.AddRange(GetAllFilesWithEnding(dir2, endsWith));
            }
            if (fileType == VcsProgramType.VertexShader || fileType == VcsProgramType.Undetermined)
            {
                string endsWith = numEnding > -1 ? $"{numEnding}_vs.vcs" : "vs.vcs";
                filesFound.AddRange(GetAllFilesWithEnding(dir1, endsWith));
                filesFound.AddRange(GetAllFilesWithEnding(dir2, endsWith));
            }
            if (fileType == VcsProgramType.PixelShader || fileType == VcsProgramType.Undetermined)
            {
                string endsWith = numEnding > -1 ? $"{numEnding}_ps.vcs" : "ps.vcs";
                filesFound.AddRange(GetAllFilesWithEnding(dir1, endsWith));
                filesFound.AddRange(GetAllFilesWithEnding(dir2, endsWith));
            }
            if (fileType == VcsProgramType.GeometryShader || fileType == VcsProgramType.Undetermined)
            {
                string endsWith = numEnding > -1 ? $"{numEnding}_gs.vcs" : "gs.vcs";
                filesFound.AddRange(GetAllFilesWithEnding(dir1, endsWith));
                filesFound.AddRange(GetAllFilesWithEnding(dir2, endsWith));
            }
            if (fileType == VcsProgramType.PixelShaderRenderState || fileType == VcsProgramType.Undetermined)
            {
                string endsWith = numEnding > -1 ? $"{numEnding}_psrs.vcs" : "psrs.vcs";
                filesFound.AddRange(GetAllFilesWithEnding(dir1, endsWith));
                filesFound.AddRange(GetAllFilesWithEnding(dir2, endsWith));
            }
            if (fileType == VcsProgramType.DomainShader || fileType == VcsProgramType.Undetermined)
            {
                string endsWith = numEnding > -1 ? $"{numEnding}_ds.vcs" : "ds.vcs";
                filesFound.AddRange(GetAllFilesWithEnding(dir1, endsWith));
                filesFound.AddRange(GetAllFilesWithEnding(dir2, endsWith));
            }
            if (fileType == VcsProgramType.HullShader || fileType == VcsProgramType.Undetermined)
            {
                string endsWith = numEnding > -1 ? $"{numEnding}_hs.vcs" : "hs.vcs";
                filesFound.AddRange(GetAllFilesWithEnding(dir1, endsWith));
                filesFound.AddRange(GetAllFilesWithEnding(dir2, endsWith));
            }
            if (fileType == VcsProgramType.RaytracingShader || fileType == VcsProgramType.Undetermined)
            {
                string endsWith = numEnding > -1 ? $"{numEnding}_rtx.vcs" : "rtx.vcs";
                filesFound.AddRange(GetAllFilesWithEnding(dir1, endsWith));
                filesFound.AddRange(GetAllFilesWithEnding(dir2, endsWith));
            }
            if (sortFiles)
            {
                filesFound.Sort();
            }
            if (filesFound.Count <= LIMIT_NR)
            {
                return filesFound;
            } else
            {
                List<string> returnFiles = new();
                for (int i = 0; i < LIMIT_NR; i++)
                {
                    returnFiles.Add(filesFound[i]);
                }
                return returnFiles;
            }
        }

        private static List<string> GetAllFilesWithEnding(string dir, string endsWith)
        {
            List<string> filesFound = new();
            if (dir == null)
            {
                return filesFound;
            }

            foreach (string filenamepath in Directory.GetFiles(dir))
            {
                if (filenamepath.EndsWith(endsWith))
                {
                    filesFound.Add(filenamepath.Replace("\\", "/"));
                }
            }
            return filesFound;
        }

        public static (string, string, string) GetTriple(string ft)
        {
            if (!ft.EndsWith("features.vcs"))
            {
                throw new System.Exception("not a features file");
            }
            string vs = $"{ft[0..^12]}vs.vcs";
            string ps = $"{ft[0..^12]}ps.vcs";

            if (File.Exists(vs) && File.Exists(ps))
            {
                return (ft, vs, ps);
            } else
            {
                return (null, null, null);
                // throw new System.Exception($"ps/vs files don't exist for {ft}");
            }
        }

        public static List<(string, string, string)> GetFeaturesVsPsFileTriple(string dir1, string dir2, int vcsFileVer = -1)
        {
            List<(string, string, string)> fileTriplets = new();
            List<string> featuresFiles = GetVcsFiles(dir1, dir2, VcsProgramType.Features, vcsFileVer);
            foreach (string featFile in featuresFiles)
            {
                string vsFile = $"{featFile[0..^12]}vs.vcs";
                string psFile = $"{featFile[0..^12]}ps.vcs";
                if (File.Exists(vsFile) && File.Exists(psFile))
                {
                    fileTriplets.Add((featFile, vsFile, psFile));
                }
            }

            fileTriplets.Sort();
            return fileTriplets;
        }

        public static string RemoveBaseDir(string filenamepath)
        {
            string dirname = Path.GetDirectoryName(filenamepath).Replace("\\", "/");
            string filename = Path.GetFileName(filenamepath).Replace("\\", "/");
            if (dirname.EndsWith(@"/shaders/vfx"))
            {
                return @"/shaders/vfx/" + filename;
            } else if (dirname.EndsWith(@"/shaders-core/vfx"))
            {
                return @"/shaders-core/vfx/" + filename;
            } else
            {
                return filenamepath;
            }
        }

        public static string ShortHandName(string filenamepath)
        {
            filenamepath = filenamepath.Replace("\\", "/");
            string dirname = Path.GetDirectoryName(filenamepath);
            string filename = Path.GetFileName(filenamepath);
            if (dirname.EndsWith(@"/shaders/vfx"))
            {
                return @"/dota/" + filename;
            } else if (dirname.EndsWith(@"/shaders-core/vfx"))
            {
                return @"/core/" + filename;
            } else
            {
                return filenamepath;
            }
        }

        public static string GetHtmlHeader(string browserTitle, string pageHeader)
        {
            string html_header = "" +
                $"<!DOCTYPE html>\n<html>\n<head>\n  <title>{browserTitle}</title>\n" +
                $"  <link href='/includes/styles.css' rel='stylesheet' type='text/css' />\n" +
                $"</head>\n<body>\n<b>{pageHeader}</b>\n<pre>";

            return html_header;
        }

        public static string GetHtmlFooter()
        {
            return "</pre>\n</html>";
        }

        public static string GetGlslHtmlLink(string glslByteString)
        {
            return $"<a href='{GetGlslHtmlFilename(glslByteString)}'>{glslByteString}</a>";
        }

        public static string GetGlslHtmlFilename(string glslByteString)
        {
            return $"glsl-{glslByteString.Trim().Replace(" ", "").ToLower()}.html";
        }

        public static string GetGlslTxtFilename(string glslByteString)
        {
            return $"glsl-{glslByteString.Trim().Replace(" ", "").ToLower()}.txt";
        }

        // converts a features filename or filenamepath to an output file ending in ps-analysis.html
        // for links in the features file to relates ps and vs files
        public static string GetVsHtmlLink(string vcsFeaturesFilename, string urlText)
        {
            return $"<a href='{GetVsHtmlFilename(vcsFeaturesFilename)}'>{urlText}</a>";
        }

        public static string GetHtmlLink(string vcsFileName, string label, string urlText = null)
        {
            if (urlText == null)
            {
                urlText = Path.GetFileName(vcsFileName);
            }
            // return $"<a href='/vcs-all/{GetCoreOrDotaString(vcsFileName)}/{Path.GetFileName(vcsFileName)[0..^4]}-analysis.html'>{urlText}</a>";
            FileTokensOld filetokens = new FileTokensOld(vcsFileName);
            return $"<a href='{filetokens.GetServerFileUrl(label)}'>{urlText}</a>";
        }

        public static string GetCoreOrDotaString(string vcsFileName)
        {
            if (Path.GetDirectoryName(vcsFileName).EndsWith("shaders\\vfx"))
            {
                return "dota";
            }
            if (Path.GetDirectoryName(vcsFileName).EndsWith("shaders-core\\vfx"))
            {
                return "core";
            }
            if (Path.GetDirectoryName(vcsFileName).EndsWith("artifact-shaders-pc-core"))
            {
                return "artifact-core";
            }
            if (Path.GetDirectoryName(vcsFileName).EndsWith("artifact-shaders-pc-dcg"))
            {
                return "artifact-dcg";
            }
            if (vcsFileName.Contains("mobile-gles/core"))
            {
                return "dota-core-gles";
            }
            if (vcsFileName.Contains("mobile-gles/dac"))
            {
                return "dota-dac-gles";
            }
            if (vcsFileName.Contains("alyx-vulkan-hlvr"))
            {
                return "hlvr";
            }
            throw new ShaderParserException("don't know where this file belongs");
        }

        /*
         *
         * E.g.
         * cs_compress_stb_dxt5_pcgl_30_features.vcs"
         * becomes
         * cs_compress_stb_dxt5(ft)
         *
         */
        public static string GetShortName(string vcsFileName)
        {
            string shortName = "";
            string token = "";
            string filename = Path.GetFileName(vcsFileName);
            VcsProgramType vcsProgramType = ComputeVCSFileName(vcsFileName).Item1;
            token = vcsProgramType switch
            {
                VcsProgramType.Features => "ft",
                VcsProgramType.VertexShader => "vs",
                VcsProgramType.PixelShader => "ps",
                VcsProgramType.GeometryShader => "gs",
                VcsProgramType.HullShader => "hs",
                VcsProgramType.DomainShader => "ds",
                VcsProgramType.ComputeShader => "cs",
                VcsProgramType.PixelShaderRenderState => "psrs",
                VcsProgramType.RaytracingShader => "rtx",
                _ => throw new ShaderParserException("not possible")
            };

            string[] fileTokens = filename.Split('_');
            if (fileTokens.Length < 4)
            {
                throw new ShaderParserException("not a valid filename");
            }
            shortName = fileTokens[0];
            for (int i = 1; i < fileTokens.Length - 3; i++)
            {
                shortName += $"_{fileTokens[i]}";
            }
            return $"{shortName}({token})";
        }

        public static string GetVsHtmlFilename(string vcsFeaturesFilename)
        {
            if (!vcsFeaturesFilename.EndsWith("features.vcs"))
            {
                throw new ShaderParserException($"this needs to be features vcs file {vcsFeaturesFilename}");
            }
            return $"{Path.GetFileName(vcsFeaturesFilename)[0..^12]}vs-analysis.html";
        }

        public static string GetPsHtmlLink(string vcsFeaturesFilename, string urlText)
        {
            return $"<a href='{GetPsHtmlFilename(vcsFeaturesFilename)}'>{urlText}</a>";
        }

        // converts a features filename or filenamepath to an output file ending in ps-analysis.html
        public static string GetPsHtmlFilename(string vcsFeaturesFilename)
        {
            if (!vcsFeaturesFilename.EndsWith("features.vcs"))
            {
                throw new ShaderParserException($"this needs to be features vcs file {vcsFeaturesFilename}");
            }
            return $"{Path.GetFileName(vcsFeaturesFilename)[0..^12]}ps-analysis.html";
        }

        public static string GetZframeTxtFilename(long zframeId, string vcsFilename)
        {
            return $"{Path.GetFileName(vcsFilename)[0..^4]}-ZFRAME{zframeId:x08}.txt";
        }

        public static string GetZframeHtmlFilename(long zframeId, string label, string vcsFilename)
        {
            if (label.Length > 0)
            {
                label = $"-{label}";
            }
            return $"{Path.GetFileName(vcsFilename)[0..^4]}-ZFRAME{zframeId:x08}{label}.html";
        }

        public static string GetZframeHtmlLink(long zframeId, string label, string vcsFilenamepath, string basepath = "")
        {
            return $"<a href='{basepath}{GetZframeHtmlFilename(zframeId, label, Path.GetFileName(vcsFilenamepath))}'>zframe[0x{zframeId:x08}]</a>";
        }

        public static string GetZframeHtmlLinkCheckExists(long zframeId, string label, string vcsFilenamepath, string serverdir, string basedir = "")
        {
            string zframeName = $"Z[0x{zframeId:x08}]";
            string zframeHtmlFilename = GetZframeHtmlFilename(zframeId, label, Path.GetFileName(vcsFilenamepath));
            if (File.Exists($"{serverdir}/{basedir}/{zframeHtmlFilename}"))
            {
                return $"  <a href='{basedir}{GetZframeHtmlFilename(zframeId, label, Path.GetFileName(vcsFilenamepath))}'>{zframeName}</a>";
            } else
            {
                return $"  {zframeName}";
            }
        }

        /*
         * E.g.
         * filenamepath = $"{DOTA_GAME_PCGL_SOURCE}/hero_pcgl_30_ps.vcs"
         *
         * returns
         * ../shaders/vfx/hero_pcgl_30_features.vcs
         * ../shaders/vfx/hero_pcgl_30_vs.vcs
         * ../shaders/vfx/hero_pcgl_30_psrs.vcs
         *
         *
         */
        public static string[] GetRelatedFiles(string filenamepath)
        {
            string filename = Path.GetFileName(filenamepath);
            string collectionName = filename.Substring(0, filename.LastIndexOf('_'));
            List<string> relatedFiles = new();
            string featuresFile = null;
            foreach (var f in Directory.GetFiles(Path.GetDirectoryName(filenamepath)))
            {
                // if (Path.GetFileName(f).StartsWith(collectionName) && !Path.GetFileName(f).Equals(filename))
                if (Path.GetFileName(f).StartsWith(collectionName))
                {
                    if (f.EndsWith("features.vcs"))
                    {
                        featuresFile = f.Replace("\\", "/");
                    } else if (f.EndsWith("vs.vcs"))
                    {
                        relatedFiles.Insert(0, f.Replace("\\", "/"));
                    } else
                    {
                        relatedFiles.Add(f.Replace("\\", "/"));
                    }
                }
            }
            if (featuresFile != null)
            {
                relatedFiles.Insert(0, featuresFile);
            }
            return relatedFiles.ToArray();
        }

        /*
         * not sorted in any way, backslashes not replaced, also includes the input file.
         * Returns filenames only (does not include path)
         * E.g.
         * {hero_pcgl_30_features.vcs, hero_pcgl_30_ps.vcs, hero_pcgl_30_psrs.vcs, hero_pcgl_30_vs.vcs}
         *
         */
        public static List<string> GetRelatedFiles2(string filenamepath)
        {
            string filename = Path.GetFileName(filenamepath);
            string vcsCollectionName = filename.Substring(0, filename.LastIndexOf('_')); // in the form water_dota_pcgl_40
            List<string> relatedFiles = new();
            foreach (var f in Directory.GetFiles(Path.GetDirectoryName(filenamepath)))
            {
                if (Path.GetFileName(f).StartsWith(vcsCollectionName))
                {
                    relatedFiles.Add(Path.GetFileName(f));
                }
            }
            return relatedFiles;
        }

        public static (VcsProgramType, VcsPlatformType, VcsShaderModelType) ComputeVCSFileName(string filenamepath)
        {
            VcsProgramType vcsProgramType = VcsProgramType.Undetermined;
            VcsPlatformType vcsPlatformType = VcsPlatformType.Undetermined;
            VcsShaderModelType vcsShaderModelType = VcsShaderModelType.Undetermined;

            string[] fileTokens = Path.GetFileName(filenamepath).Split("_");
            if (fileTokens.Length < 4)
            {
                throw new ShaderParserException($"Filetype type unknown or not supported {filenamepath}");
            }
            vcsProgramType = fileTokens[^1].ToLower() switch
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
            vcsPlatformType = fileTokens[^3].ToLower() switch
            {
                "pc" => VcsPlatformType.PC,
                "pcgl" => VcsPlatformType.PCGL,
                "gles" => VcsPlatformType.MOBILE_GLES,
                "vulkan" => VcsPlatformType.VULKAN,
                _ => VcsPlatformType.Undetermined
            };
            if (vcsPlatformType == VcsPlatformType.VULKAN)
            {
                vcsPlatformType = fileTokens[^4].ToLower() switch
                {
                    "android" => VcsPlatformType.ANDROID_VULKAN,
                    "ios" => VcsPlatformType.IOS_VULKAN,
                    _ => VcsPlatformType.VULKAN
                };
            }
            vcsShaderModelType = fileTokens[^2].ToLower() switch
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
            } else
            {
                return (vcsProgramType, vcsPlatformType, vcsShaderModelType);
            }
        }

        public static VcsProgramType ComputeVcsProgramType(string filenamepath)
        {
            return Path.GetFileName(filenamepath).Split("_").Length < 4 ?
                VcsProgramType.Undetermined
            :
             Path.GetFileName(filenamepath).Split("_")[^1].ToLower() switch
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

        public static string ParseDynamicExpression(byte[] dynExpDatabytes)
        {
            try
            {
                return new VfxEval(dynExpDatabytes, omitReturnStatement: true).DynamicExpressionResult.Replace("UNKNOWN", "VAR"); ;
            } catch (Exception)
            {
                return "[error in dyn-exp]";
            }
        }

        // --- collect int and string mechanisms

        public static SortedDictionary<int, int> collectValuesInt = new();
        public static SortedDictionary<string, int> collectValuesString = new();

        public static void CollectIntValue(int val)
        {
            int currIterator = collectValuesInt.GetValueOrDefault(val, 0);
            collectValuesInt[val] = currIterator + 1;
        }
        public static void CollectStringValue(string val)
        {
            int currIterator = collectValuesString.GetValueOrDefault(val, 0);
            collectValuesString[val] = currIterator + 1;
        }

        public static void PrintReport(bool showCount = true)
        {
            List<int> intvalues = new();
            foreach (int i in collectValuesInt.Keys)
            {
                intvalues.Add(i);
            }
            intvalues.Sort();
            foreach (int i in intvalues)
            {
                if (showCount)
                {
                    collectValuesInt.TryGetValue(i, out int instanceCount);
                    Console.WriteLine($"{i,5}        {instanceCount,3}");
                } else
                {
                    Console.WriteLine($"{i}");
                }
            }

            List<string> stringvalues = new();
            foreach (string s in collectValuesString.Keys)
            {
                stringvalues.Add(s);
            }
            stringvalues.Sort();
            foreach (string s in stringvalues)
            {
                if (showCount)
                {
                    collectValuesString.TryGetValue(s, out int instanceCount);
                    Console.WriteLine($"{s.PadRight(120)}        {instanceCount,3}");
                } else
                {
                    Console.WriteLine($"{s}");
                }
            }

            collectValuesInt = new();
            collectValuesString = new();
        }

        public delegate void MyHandleOutputWrite(string s);

        public class OutputFormatterTabulatedData
        {
            public HandleOutputWrite outputWriter { get; set; }

            public OutputFormatterTabulatedData(HandleOutputWrite outputWriter = null)
            {
                this.outputWriter = outputWriter ?? ((x) => { Console.Write(x); });
            }

            public void Write(string text)
            {
                outputWriter(text);
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
            return stringCollection.ToArray();
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
    }
}
