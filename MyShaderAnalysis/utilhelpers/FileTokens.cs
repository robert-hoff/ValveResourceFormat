using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using MyShaderAnalysis.vcsparsing;
using System;
using MyShaderAnalysis.compat;
using static MyShaderAnalysis.vcsparsing.UtilHelpers;
using static MyShaderAnalysis.utilhelpers.FileSystem;


/*
 *
 * Instantiate this on a per-file basis
 *
 *
 *
 *
 */
namespace MyShaderAnalysis.utilhelpers {


    public class FileTokens {


        public string name;          // name without the file extension, e.g. spritecard_pcgl_30_ps
        public string filename;
        public string filenamepath;
        public string namelabel;     // the name upto the first '_' e.g. spritecard (good for html titles)
        public string sourcedir;     // full directory path of the source files
        public string archivename;   // dota-game or dota-core
        public string archivelabel;  // dota or core (possibly s&box later). Used for generating names
        public string gputype;       // pcgl, pc, vulkan
        public string sourcetype;    // glsl, dx11, etc.
        public string serverdir;     // full directory path of the server files
        public FILETYPE vcsFiletype;


        public FileTokens(ARCHIVE archive, string filename) {
            this.filename = filename;
            this.name = filename[0..^4];
            this.filenamepath = GetFilenamepath(archive, filename);
            this.vcsFiletype = GetVcsFileType(filename);
            this.sourcedir = GetSourceDir(archive);
            this.archivename = GetArchiveName(archive);
            this.archivelabel = GetArchiveLabel(archive);
            this.gputype = GetGpuType(archive);
            this.sourcetype = GetSourceType(archive);
            this.serverdir = GetServerDir();
            this.namelabel = filename.Split('_')[0];

            if (!File.Exists(filenamepath)) {
                throw new ShaderParserException("file doesn't exist");
            }
        }



        public string GetZFrameServerDir() {
            return $"{serverdir}/{archivename}/{gputype}/{name}/zframes";
        }

        public List<string> GetZFrameListing() {
            List<string> zframeFiles = new();
            if (!Directory.Exists(GetZFrameServerDir())) {
                return zframeFiles;
            } else {
                foreach (var zframeFile in Directory.GetFiles(GetZFrameServerDir()) {
                    zframeFiles.Add(zframeFile);
                }
                return zframeFiles;
            }
        }



        public override string ToString() {
            string fileDetails = "";
            fileDetails += $"{name} ({archivelabel})\n";
            fileDetails += $"{archivename}\n";
            fileDetails += $"{gputype}\n";
            fileDetails += $"{sourcetype}\n";
            return fileDetails;
        }



    }



}




