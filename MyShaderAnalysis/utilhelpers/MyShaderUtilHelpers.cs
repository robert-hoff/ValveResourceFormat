using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ValveResourceFormat.CompiledShader;
using static ValveResourceFormat.CompiledShader.ShaderUtilHelpers;

namespace MyShaderAnalysis.utilhelpers
{
    class MyShaderUtilHelpers
    {

        public static List<string> GetVcsFiles(string dir1, VcsProgramType fileType)
        {
            return GetVcsFiles(dir1, null, fileType, -1);
        }


        public static List<string> GetVcsFiles(string dir1, string dir2, VcsProgramType fileType, int numEnding, bool sortFiles = true)
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
            if (sortFiles)
            {
                filesFound.Sort();
            }
            return filesFound;
        }

        public static List<string> GetAllFilesWithEnding(string dir, string endsWith)
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
            FileTokens filetokens = new FileTokens(vcsFileName);
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
            throw new ShaderParserException("don't know where this file belongs");
        }

        public static string GetShortName(string vcsFileName)
        {
            string shortName = "";
            string token = "";
            string filename = Path.GetFileName(vcsFileName);
            VcsProgramType vcsFiletype = ComputeVCSFileName(vcsFileName).Item1;
            if (vcsFiletype == VcsProgramType.Features)
            {
                shortName = filename[0..^16];
                token = "ft";
            } else if (vcsFiletype == VcsProgramType.PixelShaderRenderState)
            {
                shortName = filename[0..^12];
                token = "psrs";
            } else
            {
                shortName = filename[0..^10];
                token = $"{vcsFiletype.ToString()[0..^5]}";
            }
            if (!shortName.EndsWith("pcgl"))
            {
                throw new ShaderParserException("this is only implemented for pcgl files");
            }
            return $"{shortName[0..^5]}({token})";
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
                    Console.WriteLine($"{s.PadRight(80)}        {instanceCount,3}");
                } else
                {
                    Console.WriteLine($"{s}");
                }
            }

            collectValuesInt = new();
            collectValuesString = new();
        }


    }
}
