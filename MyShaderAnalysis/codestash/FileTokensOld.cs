using MyShaderFile.CompiledShader;
using System.Collections.Generic;
using System.IO;
using static MyShaderAnalysis.codestash.FileSystemOld;
using static MyShaderAnalysis.filearchive.FileArchive;
using static MyShaderFile.CompiledShader.ShaderUtilHelpers;

/*
 *
 *
 * GetServerFileDir()                  Z:/dev/www/vcs.codecreation.dev/dota-game/pcgl/multiblend_pcgl_30
 * GetServerFilenamepath(label)        Z:/dev/www/vcs.codecreation.dev/dota-game/pcgl/multiblend_pcgl_30/multiblend_pcgl_30_ps-label.html
 * GetServerFilePath()                 /dota-game/pcgl/multiblend_pcgl_30
 * GetServerFileUrl(label)             /dota-game/pcgl/multiblend_pcgl_30/multiblend_pcgl_30_ps-label.html
 * GetGlslServerDir()                  Z:/dev/www/vcs.codecreation.dev/dota-game/pcgl/multiblend_pcgl_30/glsl
 * GetGlslHtmlFilename(gpuSource)      glsl-e46ad784246f747dd88a611874194020.html
 * GetGlslHtmlUrl(gpusource)           /dota-game/pcgl/multiblend_pcgl_30/glsl/glsl-e46ad784246f747dd88a611874194020.html
 * GetZFramesServerDir()               Z:/dev/www/vcs.codecreation.dev/dota-game/pcgl/multiblend_pcgl_30/zframes
 * GetZFramesServerPath()              /dota-game/pcgl/multiblend_pcgl_30/zframes
 * GetZFrameHtmlFilenamepath(id,label) Z:/dev/www/vcs.codecreation.dev/dota-game/pcgl/multiblend_pcgl_30/zframes/multiblend_pcgl_30_ps-ZFRAME00000000-label.html
 * GetZFrameHtmlFilename(id,label)     multiblend_pcgl_30_ps-ZFRAME00000000-label.html
 * GetZFrameLink(id,label)             /dota-game/pcgl/multiblend_pcgl_30/zframes/multiblend_pcgl_30_ps-ZFRAME00000000-label.html
 * GetShortHandName()                  multiblend_pcgl_30_ps (dota)
 *
 *
 *
 */
// todo - get rid of  GetShortName(string vcsFileName) from MyShaderUtilHelpers (and others)
namespace MyShaderAnalysis.codestash
{
    public class FileTokensOld
    {
        public string name { get; }          // name without the file extension, e.g. spritecard_pcgl_30_ps
        public string filename { get; }
        public string filenamepath { get; }
        public string foldername { get; }    // name without the type extension, e.g. spritecard_pcgl_30
        public string namelabel { get; }     // the name upto the first '_' e.g. spritecard (good for html titles)
        public string vcstoken { get; }      // ft, vs, ps, psrs or gs
        public string sourcedir { get; }     // full directory path of the source files
        public string archivename { get; }   // dota-game or dota-core
        public string archivelabel { get; }  // dota or core (possibly s&box later). Used for generating names
        public string platformType { get; }       // pcgl, pc, vulkan
        public string sourceType { get; }    // glsl, dxil, dxbc, gles, vulkan, android_vulkan, ios_vulkan
        public string sourceVersion { get; }    // 30,40,50,etc
        public string serverdir { get; }     // full directory path of the server files
        public VcsProgramType vcsProgramType { get; }

        public FileTokensOld(string filenamepath) : this(DetermineArchiveType(filenamepath), filenamepath) { }

        /*
         * this will work passing either the filename or full filenamepath
         *
         */
        public FileTokensOld(ARCHIVE_OLD archive, string filename)
        {
            filename = Path.GetFileName(filename);
            this.filename = filename;
            this.name = filename[0..^4];
            this.foldername = name.Substring(0, name.LastIndexOf('_'));
            this.filenamepath = GetFilenamepath(archive, filename);
            this.vcsProgramType = ComputeVCSFileName(filenamepath).Item1;
            this.sourcedir = GetSourceDir(archive);
            this.archivename = GetArchiveName(archive);
            this.archivelabel = GetArchiveLabel(archive);
            this.platformType = GetGpuType(archive);
            this.sourceType = GetSourceType(archive);
            this.sourceVersion = filename.Split('_')[^2];
            this.serverdir = GetServerBaseDir();
            this.namelabel = filename.Split('_')[0];
            this.vcstoken = GetVcsToken(vcsProgramType);

            if (!File.Exists(filenamepath))
            {
                throw new ShaderParserException("file doesn't exist");
            }
        }

        /*
         * e.g. Z:/dev/www/vcs.codecreation.dev/dota-game/pcgl/spritecard_pcgl_30_ps
         *
         */
        public string GetServerFileDir(bool createDirs = false)
        {
            string serverFileDir = $"{serverdir}/{archivename}/{platformType}/{foldername}";
            if (createDirs)
            {
                Directory.CreateDirectory(serverFileDir);
            }
            return serverFileDir;
        }

        public string GetServerFilenamepath(string label, bool createDirs = false)
        {
            return $"{GetServerFileDir(createDirs)}/{name}-{label}.html";
        }

        public string GetServerFilePath()
        {
            return $"/{archivename}/{platformType}/{foldername}";
        }
        public string GetServerFileUrl(string label)
        {
            return $"/{archivename}/{platformType}/{foldername}/{name}-{label}.html";
        }

        // applicable to glsl and gles files
        // todo - this should now be applicable in general (test this)
        public string GetGlslServerDir(bool createDirs = false)
        {
            string serverGlslDir = $"{GetServerFileDir()}/{sourceType}";
            if (createDirs)
            {
                Directory.CreateDirectory(serverGlslDir);
            }
            return serverGlslDir;
        }

        public string GetGlslServerUrl(bool createDirs = false)
        {
            return $"{GetServerFilePath()}/{sourceType}";
        }

        // -todo - not general, only works for glsl
        public string GetGlslHtmlFilename(string glslLabelOrRef)
        {
            return $"{sourceType}-{glslLabelOrRef}.html";
        }

        public string GetGlslHtmlFilenameGeneral(string glslLabelOrRef)
        {
            return $"{sourceType}-{glslLabelOrRef}.html";
        }

        // -todo not general, only works for glsl
        public string GetGlslHtmlUrl(string glslLabelOrRef)
        {
            // sourceType may be either "glsl" or "gles" (which is considered as a GlslSource datablock)
            return $"{GetServerFilePath()}/{sourceType}/{GetGlslHtmlFilename(glslLabelOrRef)}";
        }

        public string GetGlslHtmlUrlGeneral(string glslLabelOrRef)
        {
            return $"{GetServerFilePath()}/{sourceType}/{GetGlslHtmlFilenameGeneral(glslLabelOrRef)}";
        }

        public string GetZFramesServerDir(bool createDirs = false)
        {
            string serverZframesDir = $"{GetServerFileDir()}/zframes";
            if (createDirs)
            {
                Directory.CreateDirectory(serverZframesDir);
            }
            return serverZframesDir;
        }

        public void CreateZFramesDirectory(bool createDirs = true)
        {
            GetZFramesServerDir(createDirs);
        }

        public string GetZFramesServerPath()
        {
            return $"{GetServerFilePath()}/zframes";
        }

        public string GetZFrameHtmlFilenamepath(long zframeId, string label, bool createDirs = true)
        {
            return $"{GetZFramesServerDir(createDirs)}/{name}-ZFRAME{zframeId:x08}-{label}.html";
        }

        public string GetZFrameHtmlFilename(long zframeId, string label = "")
        {
            if (label.Length > 0)
            {
                label = $"-{label}";
            }
            return $"{name}-ZFRAME{zframeId:x08}{label}.html";
        }

        public string GetZFrameLink(long zframeId, string label)
        {
            if (label.Length > 0)
            {
                label = $"-{label}";
            }
            return $"{GetZFramesServerPath()}/{name}-ZFRAME{zframeId:x08}{label}.html";
        }

        public List<string> GetZFrameListing()
        {
            List<string> zframeFiles = new();
            if (!Directory.Exists(GetZFramesServerDir()))
            {
                return zframeFiles;
            } else
            {
                foreach (string zframeFile in Directory.GetFiles(GetZFramesServerDir()))
                {
                    if (Path.GetFileName(zframeFile).StartsWith(name))
                    {
                        zframeFiles.Add(zframeFile);
                    }
                }
                return zframeFiles;
            }
        }

        // hero_pcgl_30_ps (dota)
        public string GetShortHandName()
        {
            return $"{name} ({archivelabel})";
        }

        public string GetShortName()
        {
            return $"{namelabel}({platformType}-{vcstoken})";
        }

        public string GetBaseName()
        {
            return $"/{archivename}/{platformType}/{filename}";
        }

        // hero(pcgl-ps)
        public string GetAbbreviatedName()
        {
            string source_type = platformType;
            if (platformType.Equals("pc"))
            {
                source_type = sourceVersion.Equals("30") ? "dxil" : "dxbc";
            }
            return $"{namelabel}({source_type}-{vcstoken})";
        }

        // todo - this one is a bit weird, clean up any calling references
        public string RemoveBaseDir()
        {
            return MyTrashUtilHelpers.RemoveBaseDir(filenamepath);
        }

        // todo - this can be replaced by getPath() and label
        public string GetSummariesPath()
        {
            // return $"/GEN-output/sf-summaries/{archivelabel}/{name}-summary.html";
            return $"{GetServerFilePath()}/{name}-summary.html";
        }

        // todo - get rid of this shit
        public string GetBestPath()
        {
            if (vcsProgramType == VcsProgramType.PixelShader || vcsProgramType == VcsProgramType.VertexShader)
            {
                string summariesPath = GetSummariesPath();
                return File.Exists($"{serverdir}{summariesPath}") ? summariesPath : "";
            } else
            {
                return "";
            }
        }

        public string GetBestZframesLink(long zframeId, bool noBrackets = false)
        {
            if (File.Exists(GetZFrameHtmlFilenamepath(zframeId, "summary")))
            {
                return noBrackets ?
                    $"<a href='{GetZFrameLink(zframeId, "summary")}'>{zframeId:x08}</a>" :
                    $"<a href='{GetZFrameLink(zframeId, "summary")}'>Z[{zframeId:x08}]</a>";
            }
            // no zframe exists return plaintext
            return noBrackets ? $"{zframeId:x08}" : $"Z[{zframeId:x08}]";
        }

        public override string ToString()
        {
            string fileDetails = "";
            fileDetails += $"{name} ({archivelabel})\n";
            fileDetails += $"{archivename}\n";
            fileDetails += $"{platformType}\n";
            fileDetails += $"{sourceType}\n";
            return fileDetails;
        }
    }
}

