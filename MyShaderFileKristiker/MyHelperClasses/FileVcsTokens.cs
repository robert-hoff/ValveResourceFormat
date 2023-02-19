using ValveResourceFormat.CompiledShader;
using static MyShaderFileKristiker.MyHelperClasses.ReadShaderFile;
using static ValveResourceFormat.CompiledShader.ShaderUtilHelpers;

namespace MyShaderFileKristiker.MyHelperClasses
{
    public class FileVcsTokens
    {
        private const string SERVER_BASEDIR = "Z:/dev/www/vcs.codecreation.dev";

        public ARCHIVE archive { get; }
        public VcsProgramType programType;
        public VcsShaderModelType shaderModelType;
        public VcsPlatformType platformType;
        public string archivename { get; }      // dota-game-pcgl, dota-core-pcgl
        public string filename { get; }         // multiblend_pcgl_30_ps.vcs
        public string filedir { get; }          // X:/dota-2-VRF-exports/dota2-export-shaders-pcgl/shaders/vfx/
        public string filenamepath { get; }     // X:/dota-2-VRF-exports/dota2-export-shaders-pcgl/shaders/vfx/multiblend_pcgl_30_ps.vcs
        public string serverdir { get; }        // Z:/dev/www/vcs.codecreation.dev (base dir of the server)
        public string name { get; }             // name without the file extension, e.g. spritecard_pcgl_30_ps
        public string foldername { get; }       // name without the type extension, e.g. spritecard_pcgl_30
        public string namelabel { get; }        // the name upto the first '_' e.g. spritecard (good for html titles)
        public string sourceVersion { get; }    // 30,40,50,etc
        public string sourceType { get; }       // glsl, dxil, dxbc or vulkan (names gles, vulkan, android_vulkan, ios_vulkan are not used)
        public string vcstoken { get; }         // ft, vs, ps, psrs or gs


        // To print to a target location, seeking to change
        // archivename
        // foldername

        public FileVcsTokens(ARCHIVE archive, string filename)
        {
            this.archive = archive;
            archivename = archive.ToString();
            filename = Path.GetFileName(filename);
            this.filename = filename;
            this.filedir = $"{FileArchive.GetArchiveDir(archive)}/";
            filenamepath = $"{filedir}{filename}";
            if (!File.Exists(filenamepath))
            {
                throw new ShaderParserException("file doesn't exist");
            }
            serverdir = SERVER_BASEDIR;
            name = filename[0..^4];
            foldername = name.Substring(0, name.LastIndexOf('_'));
            namelabel = filename.Split('_')[0];

            (VcsProgramType, VcsPlatformType, VcsShaderModelType) vcsTypes = ComputeVCSFileName(filenamepath);
            programType = vcsTypes.Item1;
            platformType = vcsTypes.Item2;
            shaderModelType = vcsTypes.Item3;

            sourceVersion = filename.Split('_')[^2];
            sourceType = GetSourceDescription(platformType, shaderModelType);

            vcstoken = programType switch
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

        private ShaderFile shaderFile { get; set; } = null;

        public ShaderFile GetShaderFile()
        {
            shaderFile ??= InstantiateShaderFile(filenamepath);
            return shaderFile;
        }

        public ZFrameFile GetZframeFile(long zframeId)
        {
            ShaderFile shaderFile = GetShaderFile();
            return shaderFile.GetZFrameFile(zframeId);
        }

        public ZFrameFile GetZframeFileByIndex(int zframeIndex)
        {
            ShaderFile shaderFile = GetShaderFile();
            return shaderFile.GetZFrameFileByIndex(zframeIndex);
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
        public string GetServerFilenamepath(string label = "", bool createDirs = false)
        {
            return label.Length == 0 ?
            $"{GetServerFileDir(createDirs)}/{name}.html"
            :
            $"{GetServerFileDir(createDirs)}/{name}-{label}.html";
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
        public string GetServerFileUrl(string label = "")
        {
            return label.Length == 0 ?
            $"/{archivename}/{foldername}/{name}.html" :
            $"/{archivename}/{foldername}/{name}-{label}.html";
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
         *
         * E.g. append "-bytes"
         * GetGpuHtmlFilename($"{name}-bytes")
         *
         * to get the "bytes" version of the file
         *
         */
        public string GetGpuHtmlFilename(string labelOrRefId)
        {
            return $"{sourceType}-{labelOrRefId}.html";
        }

        /*
         * /dota-game-pcgl-v64/multiblend_pcgl_30/glsl/glsl-e46ad784246f747dd88a611874194020.html
         *
         * append "-bytes"
         * GetGpuHtmlUrl($"{name}-bytes")
         *
         * to get the "bytes" version of the file
         *
         */
        public string GetGpuHtmlUrl(string labelOrRefId)
        {
            return $"{GetServerFilePath()}/{sourceType}/{GetGpuHtmlFilename(labelOrRefId)}";
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
        public string GetZFrameHtmlFilenamepath(long zframeId, string label = "", bool createDirs = true)
        {
            return label.Length == 0 ?
            $"{GetZFramesServerDir(createDirs)}/{name}-Z{zframeId:x08}.html"
                :
            $"{GetZFramesServerDir(createDirs)}/{name}-Z{zframeId:x08}-{label}.html";
        }

        /*
         * multiblend_pcgl_30_ps-Z00000000-label.html
         */
        public string GetZFrameHtmlFilename(long zframeId, string label = "")
        {
            return label.Length == 0 ?
            $"{name}-Z{zframeId:x08}.html"
                :
            $"{name}-Z{zframeId:x08}-{label}.html";
        }

        /*
         * /dota-game-pcgl-v64/multiblend_pcgl_30/zframes/multiblend_pcgl_30_ps-ZFRAME00000000-label.html
         */
        public string GetZFrameUrl(long zframeId, string label)
        {
            return label.Length == 0 ?
            $"{GetZFramesServerPath()}/{name}-Z{zframeId:x08}.html"
                :
            $"{GetZFramesServerPath()}/{name}-Z{zframeId:x08}-{label}.html";
        }

        // Do this later, if bothered
        //public string GetGpuSourceUrl(long zframeId, string label)
        //{
        //    if (label.Length > 0)
        //    {
        //        label = $"-{label}";
        //    }
        //    return $"{GetZFramesServerPath()}/{name}-ZFRAME{zframeId:x08}{label}.html";
        //}

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

        public string GetZFrameLinkIfOneExists(long zframeId, bool noBrackets = false)
        {
            if (File.Exists(GetZFrameHtmlFilenamepath(zframeId, "")))
            {
                return noBrackets ?
                    $"<a href='{GetZFrameUrl(zframeId, "")}'>{zframeId:x08}</a>" :
                    $"<a href='{GetZFrameUrl(zframeId, "")}'>Z[{zframeId:x08}]</a>";
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

        public override string ToString()
        {
            return $"{filename}";
        }

        public static string GetSourceDescription(VcsPlatformType vcsPlatformType, VcsShaderModelType vcsShaderModelType)
        {
            if (vcsPlatformType == VcsPlatformType.PC)
            {
                switch (vcsShaderModelType)
                {
                    case VcsShaderModelType._20:
                    case VcsShaderModelType._2b:
                    case VcsShaderModelType._30:
                    case VcsShaderModelType._31:
                        return "dxil";
                    case VcsShaderModelType._40:
                    case VcsShaderModelType._41:
                    case VcsShaderModelType._50:
                    case VcsShaderModelType._60:
                        return "dxbc";
                    default:
                        throw new ShaderParserException($"Unknown or unsupported model type {vcsPlatformType} {vcsShaderModelType}");
                }
            }
            else
            {
                switch (vcsPlatformType)
                {
                    case VcsPlatformType.PCGL:
                    case VcsPlatformType.MOBILE_GLES:
                        return "glsl";
                    case VcsPlatformType.VULKAN:
                    case VcsPlatformType.ANDROID_VULKAN:
                    case VcsPlatformType.IOS_VULKAN:
                        return "vulkan";
                    default:
                        throw new ShaderParserException($"Unknown or unsupported source type {vcsPlatformType}");
                }
            }
        }
    }
}
