using System.Collections.Generic;
using System.IO;
using ValveResourceFormat.CompiledShader;

/*
 * NOTE - Contains duplicate methods of the production class
 *
 */
namespace MyShaderAnalysis.util
{
    class MyShaderUtilHelpers
    {
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
            var count = 0;
            var bytestring = "";
            for (var i = 0; i < databytes.Length; i++)
            {
                bytestring += $"{databytes[i]:X02} ";
                if (++count % breakLen == 0)
                {
                    bytestring += "\n";
                }
            }
            return bytestring.Trim();
        }

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

