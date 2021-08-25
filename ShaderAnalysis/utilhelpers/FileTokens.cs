using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System;
using ValveResourceFormat.ShaderParser;
// using static MyShaderAnalysis.utilhelpers.FileSystem;
using static ShaderAnalysis.utilhelpers.FileSystem;
// using static MyShaderAnalysis.vcsparsing.UtilHelpers;
using static ValveResourceFormat.ShaderParser.ShaderUtilHelpers;


namespace ShaderAnalysis.utilhelpers
{
    class FileTokens
    {
        public string name;          // name without the file extension, e.g. spritecard_pcgl_30_ps
        public string filename;
        public string filenamepath;
        public string foldername;    // name without the type extension, e.g. spritecard_pcgl_30
        public string namelabel;     // the name upto the first '_' e.g. spritecard (good for html titles)
        public string vcstoken;      // ft, vs, ps, psrs or gs
        public string sourcedir;     // full directory path of the source files
        public string archivename;   // dota-game or dota-core
        public string archivelabel;  // dota or core (possibly s&box later). Used for generating names
        public string gputype;       // pcgl, pc, vulkan
        public string sourcetype;    // glsl, dx11, etc.
        public string serverdir;     // full directory path of the server files
        public ValveResourceFormat.ShaderParser.VcsFileType vcsFiletype;


        public FileTokens(string filenamepath) : this(DetermineArchiveType(filenamepath), filenamepath) { }


        /*
         * this will work passing either the filename or the full filenamepath
         *
         */
        public FileTokens(ARCHIVE archive, string filename)
        {
            filename = Path.GetFileName(filename);
            this.filename = filename;
            this.name = filename[0..^4];
            this.foldername = name.Substring(0, name.LastIndexOf('_'));
            this.filenamepath = GetFilenamepath(archive, filename);
            this.vcsFiletype = GetVcsFileType(filename);
            this.sourcedir = GetSourceDir(archive);
            this.archivename = GetArchiveName(archive);
            this.archivelabel = GetArchiveLabel(archive);
            this.gputype = GetGpuType(archive);
            this.sourcetype = GetSourceType(archive);
            this.serverdir = GetServerBaseDir();
            this.namelabel = filename.Split('_')[0];
            this.vcstoken = GetVcsToken(vcsFiletype);

            if (!File.Exists(filenamepath))
            {
                throw new ShaderParserException("file doesn't exist");
            }
        }


        /*
         * e.g. Z:/dev/www/vcs.codecreation.dev/dota-game/pcgl/spritecard_pcgl_30_ps
         *
         */
        public string GetServerFileDir(bool createDir = false)
        {
            string serverFileDir = $"{serverdir}/{archivename}/{gputype}/{foldername}";
            if (createDir)
            {
                Directory.CreateDirectory(serverFileDir);
            }
            return serverFileDir;
        }

        public string GetServerFilePath(string label, bool createDirs = false)
        {
            return $"{GetServerFileDir(createDirs)}/{name}-{label}.html";
        }


        public string GetServerFilePath()
        {
            return $"/{archivename}/{gputype}/{foldername}";
        }

        public string GetZFramesServerDir(bool createDir = false)
        {
            string serverZframesDir = $"{GetServerFileDir()}/zframes";
            if (createDir)
            {
                Directory.CreateDirectory(serverZframesDir);
            }
            return serverZframesDir;
        }

        public void CreateZFramesDirectory()
        {
            GetZFramesServerDir(createDir: true);
        }

        public string GetZFramesServerPath()
        {
            return $"{GetServerFilePath()}/zframes";
        }

        public string GetZFrameHtmlFilenamepath(long zframeId)
        {
            return $"{GetZFramesServerDir()}/{name}-ZFRAME{zframeId:x08}.html";
        }

        public string GetZFrameHtmlBytesFilenamepath(long zframeId)
        {
            return $"{serverdir}/vcs-all/{archivelabel}/zsource/{name}-ZFRAME{zframeId:x08}.html";
        }

        public string GetZFrameHtmlBytesLink(long zframeId)
        {
            return $"/vcs-all/{archivelabel}/zsource/{name}-ZFRAME{zframeId:x08}.html";
        }


        public string GetZFrameHtmlFilename(long zframeId)
        {
            return $"{name}-ZFRAME{zframeId:x08}.html";
        }

        public string GetZFrameLink(long zframeId)
        {
            return $"{GetZFramesServerPath()}/{name}-ZFRAME{zframeId:x08}.html";
        }


        public List<string> GetZFrameListing()
        {
            List<string> zframeFiles = new();
            if (!Directory.Exists(GetZFramesServerDir()))
            {
                return zframeFiles;
            } else
            {
                foreach (var zframeFile in Directory.GetFiles(GetZFramesServerDir()))
                {
                    if (Path.GetFileName(zframeFile).StartsWith(name))
                    {
                        zframeFiles.Add(zframeFile);
                    }
                }
                return zframeFiles;
            }
        }

        public string GetShortHandName()
        {
            return $"{name} ({archivelabel})";
        }

        public string RemoveBaseDir()
        {
            return $"{archivelabel}/{name}";
        }

        public string GetBytePath()
        {
            return $"/vcs-all/{archivelabel}/{name}-analysis.html";
        }
        public string GetSummariesPath()
        {
            // return $"/GEN-output/sf-summaries/{archivelabel}/{name}-summary.html";
            return $"{GetServerFilePath()}/{name}-summary.html";
        }

        public string GetBestPath()
        {
            if (vcsFiletype == ValveResourceFormat.ShaderParser.VcsFileType.PixelShader || vcsFiletype == ValveResourceFormat.ShaderParser.VcsFileType.VertexShader)
            {
                string summariesPath = GetSummariesPath();
                return File.Exists($"{serverdir}{summariesPath}") ? summariesPath : "";
            } else
            {
                return GetBytePath();
            }
        }


        public string GetBestZframesLink(long zframeId)
        {
            if (File.Exists(GetZFrameHtmlFilenamepath(zframeId)))
            {
                return $"* <a href='{GetZFrameLink(zframeId)}'>Z[{zframeId:x08}]</a>";
            }
            if (File.Exists(GetZFrameHtmlBytesFilenamepath(zframeId)))
            {
                return $"  <a href='{GetZFrameHtmlBytesLink(zframeId)}'>Z[{zframeId:x08}]</a>";
            }
            return $"  Z[{zframeId:x08}]";
        }



        public override string ToString()
        {
            string fileDetails = "";
            fileDetails += $"{name} ({archivelabel})\n";
            fileDetails += $"{archivename}\n";
            fileDetails += $"{gputype}\n";
            fileDetails += $"{sourcetype}\n";
            return fileDetails;
        }



    }
}



