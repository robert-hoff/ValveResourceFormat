using System.Collections.Generic;
using System.IO;
using ValveResourceFormat.CompiledShader;

namespace MyShaderAnalysis.util
{
    class MyShaderUtilHelpers
    {

        public static List<string> GetVcsFiles(string dir1, VcsProgramType fileType,
            int numEnding = -1, bool sortFiles = true, int LIMIT_NR = 1000)
        {
            return GetVcsFiles(dir1, null, fileType, numEnding, sortFiles, LIMIT_NR);
        }

        public static List<string> GetVcsFiles(string dir1, string dir2 = null,
            VcsProgramType fileType = VcsProgramType.Undetermined,
            int numEnding = -1, bool sortFiles = true, int LIMIT_NR = 1000)
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



        // todo - duplicates from MyShaderFile.CompiledShader.ShaderUtilHelpers, but it's better to have these separated
        public static (VcsProgramType p1, VcsPlatformType p2, VcsShaderModelType p3) ComputeVCSFileName(string filenamepath)
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


        public static string GetSourceType(VcsPlatformType vcsPlatformType, VcsShaderModelType vcsShaderModelType)
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
            } else
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

