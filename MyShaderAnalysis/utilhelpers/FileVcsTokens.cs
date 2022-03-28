using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ValveResourceFormat.CompiledShader;
using static MyShaderAnalysis.utilhelpers.FileArchives;
using static MyShaderAnalysis.utilhelpers.ReadShaderFile;

namespace MyShaderAnalysis.utilhelpers
{
    internal class FileVcsTokens
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

        private ShaderFile shaderFile { get; set; } = null;


        public FileVcsTokens(ARCHIVE archive, string filename)
        {
            filename = Path.GetFileName(filename);
            this.filenamepath = GetFilenamepath(archive, filename);
            if (!File.Exists(filenamepath))
            {
                throw new ShaderParserException("file doesn't exist");
            }

            this.filename = filename;
            this.name = filename[0..^4];
            this.foldername = name.Substring(0, name.LastIndexOf('_'));
            this.vcsProgramType = ShaderUtilHelpers.ComputeVCSFileName(filenamepath).Item1;
            this.sourcedir = GetSourceDir(archive);
            this.archivename = GetArchiveName(archive);
            this.archivelabel = GetArchiveLabel(archive);
            this.platformType = GetGpuType(archive);
            this.sourceType = GetSourceType(archive);
            this.sourceVersion = filename.Split('_')[^2];
            this.serverdir = GetServerBaseDir();
            this.namelabel = filename.Split('_')[0];
            this.vcstoken = GetVcsToken(vcsProgramType);
        }


        public ShaderFile GetShaderFile()
        {
            shaderFile ??= InstantiateShaderFile(filenamepath);
            return shaderFile;
        }






    }
}


