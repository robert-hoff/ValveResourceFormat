using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ValveResourceFormat.CompiledShader;
using static MyShaderAnalysis.utilhelpers.ReadShaderFile;

/*
 *
 *
 * GetServerFileDir()                  Z:/dev/www/vcs.codecreation.dev/dota-game-pcgl-v64/multiblend_pcgl_30
 * GetServerFilenamepath(label)        Z:/dev/www/vcs.codecreation.dev/dota-game-pcgl-v64/multiblend_pcgl_30/multiblend_pcgl_30_ps-label.html
 * GetServerFilePath()                 /dota-game-pcgl-v64/multiblend_pcgl_30
 * GetServerFileUrl(label)             /dota-game-pcgl-v64/multiblend_pcgl_30/multiblend_pcgl_30_ps-label.html
 * GetGpuServerDir()                   Z:/dev/www/vcs.codecreation.dev/dota-game-pcgl-v64/multiblend_pcgl_30/glsl
 * GetGpuServerUrl()                   /dota-game-pcgl-v64/multiblend_pcgl_30/glsl
 * GetGpuHtmlFilename(gpuSource)       glsl-e46ad784246f747dd88a611874194020.html
 * GetGpuHtmlUrl(gpusource)            /dota-game-pcgl-v64/multiblend_pcgl_30/glsl/glsl-e46ad784246f747dd88a611874194020.html
 * GetZFramesServerDir()               Z:/dev/www/vcs.codecreation.dev/dota-game-pcgl-v64/multiblend_pcgl_30/zframes
 * GetZFramesServerPath()              /dota-game-pcgl-v64/multiblend_pcgl_30/zframes
 * GetZFrameHtmlFilenamepath(id,label) Z:/dev/www/vcs.codecreation.dev/dota-game-pcgl-v64/multiblend_pcgl_30/zframes/multiblend_pcgl_30_ps-ZFRAME00000000-label.html
 * GetZFrameHtmlFilename(id,label)     multiblend_pcgl_30_ps-ZFRAME00000000-label.html
 * GetZFrameUrl(id,label)              /dota-game-pcgl-v64/multiblend_pcgl_30/zframes/multiblend_pcgl_30_ps-ZFRAME00000000-label.html
 * GetShortName()                      multiblend(30-ps)
 * GetBaseName()                       /dota-game-pcgl-v64/multiblend_pcgl_30_ps.vcs
 *
 *
 *
 */
namespace MyShaderAnalysis.utilhelpers
{
    internal class FileVcsTokens
    {
        // used to have this - but isn't actually needed (parser determines this independently)
        // public string platformType { get; }  // pcgl, pc, vulkan

        public FileArchives.ARCHIVE archive { get; }
        public string archivename { get; }      // dota-game-pcgl, dota-core-pcgl
        public string filename { get; }         // multiblend_pcgl_30_ps.vcs
        public string filenamepath { get; }     // X:/dota-2-VRF-exports/dota2-export-shaders-pcgl/shaders/vfx/multiblend_pcgl_30_ps.vcs
        public string serverdir { get; }        // Z:/dev/www/vcs.codecreation.dev (base dir of the server)
        public string name { get; }             // name without the file extension, e.g. spritecard_pcgl_30_ps
        public string foldername { get; }       // name without the type extension, e.g. spritecard_pcgl_30
        public string namelabel { get; }        // the name upto the first '_' e.g. spritecard (good for html titles)
        public string sourceVersion { get; }    // 30,40,50,etc
        public string sourceType { get; }       // glsl, dxil, dxbc or vulkan (names gles, vulkan, android_vulkan, ios_vulkan are not used)
        public string vcstoken { get; }         // ft, vs, ps, psrs or gs


        private ShaderFile shaderFile { get; set; } = null;


        public FileVcsTokens(FileArchives.ARCHIVE archive, string filename)
        {
            this.archive = archive;
            this.archivename = FileArchives.GetArchiveName(archive);
            filename = Path.GetFileName(filename);
            this.filename = filename;
            this.filenamepath = $"{FileArchives.GetSourceDir(archive)}/{filename}";
            if (!File.Exists(filenamepath))
            {
                throw new ShaderParserException("file doesn't exist");
            }
            this.serverdir = FileArchives.GetServerBaseDir();
            this.name = filename[0..^4];
            this.foldername = name.Substring(0, name.LastIndexOf('_'));
            this.namelabel = filename.Split('_')[0];

            (VcsProgramType, VcsPlatformType, VcsShaderModelType) vcsTypes = ShaderUtilHelpers.ComputeVCSFileName(filenamepath);
            VcsProgramType vcsProgramType = vcsTypes.Item1;
            VcsPlatformType vcsPlatformType = vcsTypes.Item2;
            VcsShaderModelType vcsShaderModelType = vcsTypes.Item3;

            this.sourceVersion = filename.Split('_')[^2];
            this.sourceType = MyShaderUtilHelpers.GetSourceType(vcsPlatformType, vcsShaderModelType);

            this.vcstoken = ShaderUtilHelpers.ComputeVcsProgramType(filenamepath) switch
            {
                VcsProgramType.Features => "ft",
                VcsProgramType.VertexShader => "vs",
                VcsProgramType.PixelShader => "ps",
                VcsProgramType.PixelShaderRenderState => "psrs",
                VcsProgramType.GeometryShader => "cs",
                VcsProgramType.ComputeShader => "hs",
                VcsProgramType.HullShader => "ds",
                VcsProgramType.DomainShader => "gs",
                VcsProgramType.RaytracingShader => "rtx",
                _ => throw new ShaderParserException("couldn't determine vcsProgramType")
            };
        }


        public ShaderFile GetShaderFile()
        {
            shaderFile ??= InstantiateShaderFile(filenamepath);
            return shaderFile;
        }


        /*
         * Z:/dev/www/vcs.codecreation.dev/dota-game-pcgl-v64/multiblend_pcgl_30
         *
         */
        public string GetServerFileDir(bool createDirs = false)
        {
            string serverFileDir = $"{serverdir}/{archivename}/{foldername}";
            if (createDirs)
            {
                Directory.CreateDirectory(serverFileDir);
            }
            return serverFileDir;
        }

        /*
         * Z:/dev/www/vcs.codecreation.dev/dota-game-pcgl-v64/multiblend_pcgl_30/multiblend_pcgl_30_ps-label.html
         *
         */
        public string GetServerFilenamepath(string label, bool createDirs = false)
        {
            return $"{GetServerFileDir(createDirs)}/{name}-{label}.html";
        }

        /*
         * /dota-game-pcgl-v64/multiblend_pcgl_30
         */
        public string GetServerFilePath()
        {
            return $"/{archivename}/{foldername}";
        }

        /*
         * /dota-game-pcgl-v64/multiblend_pcgl_30/multiblend_pcgl_30_ps-label.html
         */
        public string GetServerFileUrl(string label)
        {
            return $"/{archivename}/{foldername}/{name}-{label}.html";
        }

        /*
         * Z:/dev/www/vcs.codecreation.dev/dota-game-pcgl-v64/multiblend_pcgl_30/glsl
         */
        public string GetGpuServerDir(bool createDirs = false)
        {
            string serverGlslDir = $"{GetServerFileDir()}/{sourceType}";
            if (createDirs)
            {
                Directory.CreateDirectory(serverGlslDir);
            }
            return serverGlslDir;
        }

        /*
         * /dota-game-pcgl-v64/multiblend_pcgl_30/glsl
         */
        public string GetGpuServerUrl()
        {
            return $"{GetServerFilePath()}/{sourceType}";
        }

        /*
         * glsl-e46ad784246f747dd88a611874194020.html
         */
        public string GetGpuHtmlFilename(GpuSource gpuSource)
        {
            return $"{sourceType}-{gpuSource.GetEditorRefIdAsString()}.html";
        }

        /*
         * /dota-game-pcgl-v64/multiblend_pcgl_30/glsl/glsl-e46ad784246f747dd88a611874194020.html
         */
        public string GetGpuHtmlUrl(GpuSource gpuSource)
        {
            return $"{GetServerFilePath()}/{sourceType}/{GetGpuHtmlFilename(gpuSource)}";
        }

        /*
         * Z:/dev/www/vcs.codecreation.dev/dota-game-pcgl-v64/multiblend_pcgl_30/zframes
         */
        public string GetZFramesServerDir(bool createDirs = false)
        {
            string serverZframesDir = $"{GetServerFileDir()}/zframes";
            if (createDirs)
            {
                Directory.CreateDirectory(serverZframesDir);
            }
            return serverZframesDir;
        }

        /*
         * /dota-game-pcgl-v64/multiblend_pcgl_30/zframes
         */
        public string GetZFramesServerPath()
        {
            return $"{GetServerFilePath()}/zframes";
        }

        /*
         * Z:/dev/www/vcs.codecreation.dev/dota-game-pcgl-v64/multiblend_pcgl_30/zframes/multiblend_pcgl_30_ps-ZFRAME00000000-label.html
         */
        public string GetZFrameHtmlFilenamepath(long zframeId, string label, bool createDirs = true)
        {
            return $"{GetZFramesServerDir(createDirs)}/{name}-ZFRAME{zframeId:x08}-{label}.html";
        }

        /*
         * multiblend_pcgl_30_ps-ZFRAME00000000-label.html
         */
        public string GetZFrameHtmlFilename(long zframeId, string label = "")
        {
            if (label.Length > 0)
            {
                label = $"-{label}";
            }
            return $"{name}-ZFRAME{zframeId:x08}{label}.html";
        }

        /*
         * /dota-game-pcgl-v64/multiblend_pcgl_30/zframes/multiblend_pcgl_30_ps-ZFRAME00000000-label.html
         */
        public string GetZFrameUrl(long zframeId, string label)
        {
            if (label.Length > 0)
            {
                label = $"-{label}";
            }
            return $"{GetZFramesServerPath()}/{name}-ZFRAME{zframeId:x08}{label}.html";
        }

        /*
         * multiblend(30-ft)
         * multiblend(30-vs)
         * multiblend(30-ps)
         *
         */
        public string GetShortName()
        {
            return $"{namelabel}({sourceVersion}-{vcstoken})";
        }

        /*
         * /dota-game-pcgl-v64/multiblend_pcgl_30_ps.vcs
         *
         * NOTE - this doesn't point to a file, the full path is
         * /dota-game-pcgl-v64/multiblend_pcgl_30/multiblend_pcgl_30_ps.vcs
         */
        public string GetBaseName()
        {
            return $"/{archivename}/{filename}";
        }


        // todo - this is a bit weird
        public string GetBestZframesLink(long zframeId, bool noBrackets = false)
        {
            if (File.Exists(GetZFrameHtmlFilenamepath(zframeId, "summary")))
            {
                return noBrackets ?
                    $"<a href='{GetZFrameUrl(zframeId, "summary")}'>{zframeId:x08}</a>" :
                    $"<a href='{GetZFrameUrl(zframeId, "summary")}'>Z[{zframeId:x08}]</a>";
            }
            // no zframe exists return plaintext
            return noBrackets ? $"{zframeId:x08}" : $"Z[{zframeId:x08}]";
        }


        /*
         * {multiblend_pcgl_30_features.vcs, multiblend_pcgl_30_vs.vcs, multiblend_pcgl_30_ps.vcs}
         *
         */
        public List<string> GetRelatedFiles()
        {
            return FileVcsCollection.GetRelatedFiles(archive, foldername);
        }


    }
}


