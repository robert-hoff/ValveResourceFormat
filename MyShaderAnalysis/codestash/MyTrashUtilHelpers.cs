using System.Collections.Generic;
using System.IO;
using MyShaderFile.CompiledShader;
using static MyShaderAnalysis.filearchive.FileArchive;
using static MyShaderFile.CompiledShader.ShaderUtilHelpers;

namespace MyShaderAnalysis.codestash
{
    public class MyTrashUtilHelpers
    {
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
    }
}

