using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using MyShaderAnalysis.vcsparsing;
using System;
using MyShaderAnalysis.compat;
using static MyShaderAnalysis.vcsparsing.ShaderUtilHelpers;



/*
 * NOTE - use forward slashes in ALL cases because this is what HTML wants,
 * and it doesn't make any difference for local files
 *
 *
 * There are different contexts where different bits of info related to names and directories are needed
 *
 * Saving a zframe to html file.
 * - base directory to the server
 * - relative directory to the zframe folder
 * - zframe filename
 * - html title
 * - html header
 *
 * Creating a file-listing over existing zframes
 * - folder where the zframe files are (with the updated folder layout)
 * - standard zframe filename, note the addition of 'labels' to support different sets
 *
 *
 * Use the names GetDirectory(..) and GetPath(..) to distinguish between file-management and html-linking
 *
 *
 * Consider
 * --------
 * - it seems natural to also retrieve the files from here. This is definitely an advantage
 * because there are some tokens I can determine immediately by doing that. Currently I'm dependend on
 * the meethod DetermineArchiveType(..) - but it's not terrible
 * - write tokens into ShaderFile? (this would be sensible if I also didn't need to copy this into VRF)
 * - drop support for vcs-all target
 * - delete functionality to clean particular sets
 *
 *
 */
namespace MyShaderAnalysis.utilhelpers
{

    public class FileSystem
    {

        public const string PROJECT_TESTDIR = "Z:/active/projects/dota2-sourcesdk-modding/shader-analysis-vcs-format/OUTPUT_DUMP";
        public const string SERVER_BASEDIR = "Z:/dev/www/vcs.codecreation.dev";
        public const string SERVER_TESTPATH = "/GEN-output";
        public const string SERVER_BYTEPATH = "/vcs-all";
        public const string DOTA_CORE_PCGL_SOURCE = "X:/dota-2-VRF-exports/dota2-export-shaders-pcgl/shaders-core/vfx";
        public const string DOTA_GAME_PCGL_SOURCE = "X:/dota-2-VRF-exports/dota2-export-shaders-pcgl/shaders/vfx";
        public const string DOTA_CORE_PC_SOURCE = "X:/dota-2-VRF-exports/dota2-export-shaders-pc/shaders-core/vfx";
        public const string DOTA_GAME_PC_SOURCE = "X:/dota-2-VRF-exports/dota2-export-shaders-pc/shaders/vfx";
        public const string DOTA_CORE_MOBILE_GLES_SOURCE = "X:/dota-2-VRF-exports/dota2-shaders-mobile-gles/core";
        public const string DOTA_DAC_MOBILE_GLES_SOURCE = "X:/dota-2-VRF-exports/dota2-shaders-mobile-gles/dac";
        public const string DOTA_CORE_ANDROID_VULKAN_SOURCE = "X:/dota-2-VRF-exports/dota2-shaders-android-vulkan/core";
        public const string DOTA_DAC_ANDROID_VULKAN_SOURCE = "X:/dota-2-VRF-exports/dota2-shaders-android-vulkan/dac";
        public const string DOTA_CORE_IOS_VULKAN_SOURCE = "X:/dota-2-VRF-exports/dota2-shaders-ios-vulkan/core";
        public const string DOTA_DAC_IOS_VULKAN_SOURCE = "X:/dota-2-VRF-exports/dota2-shaders-ios-vulkan/dac";
        public const string ARTIFACT_CLASSIC_CORE_PC_SOURCE = "X:/artifact-classic-exports/artifact-shaders-pc-core";
        public const string ARTIFACT_CLASSIC_DCG_PC_SOURCE = "X:/artifact-classic-exports/artifact-shaders-pc-dcg";
        public const string HLALYX_CORE_VULKAN_SOURCE = "X:/hl2alyx-export/alyx-vulkan-core";
        public const string HLALYX_HLVR_VULKAN_SOURCE = "X:/hl2alyx-export/alyx-vulkan-hlvr";




        public enum ARCHIVE { dotacore_pcgl, dotagame_pcgl, dotacore_pc, dotagame_pc,
            dota_core_gles, dota_dac_gles,
            artifact_classiccore_pc, artifact_classicdcg_pc };



        public static string GetFilenamepath(ARCHIVE archive, string filename)
        {
            return $"{GetSourceDir(archive)}/{filename}";
        }

        public static string GetServerBaseDir()
        {
            return SERVER_BASEDIR;
        }


        // todo - it may be possible to write all of this into the enums
        public static string GetSourceDir(ARCHIVE archive)
        {
            if (archive == ARCHIVE.dotacore_pcgl) return DOTA_CORE_PCGL_SOURCE;
            if (archive == ARCHIVE.dotagame_pcgl) return DOTA_GAME_PCGL_SOURCE;
            if (archive == ARCHIVE.dotacore_pc) return DOTA_CORE_PC_SOURCE;
            if (archive == ARCHIVE.dotagame_pc) return DOTA_GAME_PC_SOURCE;
            if (archive == ARCHIVE.dota_core_gles) return DOTA_CORE_MOBILE_GLES_SOURCE;
            if (archive == ARCHIVE.dota_dac_gles) return DOTA_DAC_MOBILE_GLES_SOURCE;
            if (archive == ARCHIVE.artifact_classiccore_pc) return ARTIFACT_CLASSIC_CORE_PC_SOURCE;
            if (archive == ARCHIVE.artifact_classicdcg_pc) return ARTIFACT_CLASSIC_DCG_PC_SOURCE;
            throw new ShaderParserException("unknown archive");
        }

        public static string GetArchiveName(ARCHIVE archive)
        {
            if (archive == ARCHIVE.dotacore_pcgl) return "dota-core";
            if (archive == ARCHIVE.dotagame_pcgl) return "dota-game";
            if (archive == ARCHIVE.dotacore_pc) return "dota-core"; // fine to use the same name here, but need to use platform type in path names
            if (archive == ARCHIVE.dotagame_pc) return "dota-game";
            if (archive == ARCHIVE.dota_core_gles) return "dota-core";
            if (archive == ARCHIVE.dota_dac_gles) return "dota-game";
            if (archive == ARCHIVE.artifact_classiccore_pc) return "aclassic-core";
            if (archive == ARCHIVE.artifact_classicdcg_pc) return "aclassic-dcg";
            throw new ShaderParserException("unknown archive");
        }

        public static string GetArchiveLabel(ARCHIVE archive)
        {
            if (archive == ARCHIVE.dotacore_pcgl) return "core";
            if (archive == ARCHIVE.dotagame_pcgl) return "dota";
            // todo - fix this, don't use archive-label as part of pathname
            if (archive == ARCHIVE.dotacore_pc) return "core-pc"; // the archive-labels need to be distinct because I'm using them as part
                                                                  // of the byte-dump paths
            if (archive == ARCHIVE.dotagame_pc) return "dota-pc";
            if (archive == ARCHIVE.dota_core_gles) return "core-gles";
            if (archive == ARCHIVE.dota_dac_gles) return "dac-gles";
            if (archive == ARCHIVE.artifact_classiccore_pc) return "artifact-core";
            if (archive == ARCHIVE.artifact_classicdcg_pc) return "artifact-dcg";
            throw new ShaderParserException("unknown archive");
        }

        // todo - may want to change this to 'platform'
        public static string GetGpuType(ARCHIVE archive)
        {
            if (archive == ARCHIVE.dotacore_pcgl) return "pcgl";
            if (archive == ARCHIVE.dotagame_pcgl) return "pcgl";
            if (archive == ARCHIVE.dotacore_pc) return "pc";
            if (archive == ARCHIVE.dotagame_pc) return "pc";
            if (archive == ARCHIVE.dota_core_gles) return "gles";
            if (archive == ARCHIVE.dota_dac_gles) return "gles";
            if (archive == ARCHIVE.artifact_classiccore_pc) return "pc";
            if (archive == ARCHIVE.artifact_classicdcg_pc) return "pc";
            throw new ShaderParserException("unknown archive");
        }

        // todo - start using defined enums
        public static string GetSourceType(ARCHIVE archive)
        {
            if (archive == ARCHIVE.dotacore_pcgl) return "glsl";
            if (archive == ARCHIVE.dotagame_pcgl) return "glsl";
            if (archive == ARCHIVE.dotacore_pc) return "pc";
            if (archive == ARCHIVE.dotagame_pc) return "pc";
            if (archive == ARCHIVE.dota_core_gles) return "gles";
            if (archive == ARCHIVE.dota_dac_gles) return "gles";
            if (archive == ARCHIVE.artifact_classiccore_pc) return "pc"; // todo - I want dxil and dxbc here
            if (archive == ARCHIVE.artifact_classicdcg_pc) return "pc";
            throw new ShaderParserException("unknown archive");
        }

        public static string GetVcsToken(VcsFileType vcsFiletype)
        {
            if (vcsFiletype == VcsFileType.Features) return "ft"; ;
            if (vcsFiletype == VcsFileType.VertexShader) return "vs"; ;
            if (vcsFiletype == VcsFileType.PixelShader) return "ps"; ;
            if (vcsFiletype == VcsFileType.PixelShaderRenderState) return "psrs"; ;
            if (vcsFiletype == VcsFileType.ComputeShader) return "cs"; ;
            if (vcsFiletype == VcsFileType.HullShader) return "hs"; ;
            if (vcsFiletype == VcsFileType.DomainShader) return "ds"; ;
            if (vcsFiletype == VcsFileType.GeometryShader) return "gs"; ;
            if (vcsFiletype == VcsFileType.RaytracingShader) return "rtx"; ;
            throw new ShaderParserException($"Unknown vcsFileType {vcsFiletype}");
        }


        // todo - problem here with the backslashes, it's not consistent, if I call this with the consts from FileSystem it will fail
        public static ARCHIVE DetermineArchiveType(string vcsFileName)
        {
            if (Path.GetDirectoryName(vcsFileName).EndsWith("shaders/vfx") && vcsFileName.Contains("_pcgl_"))
            {
                return ARCHIVE.dotagame_pcgl;
            }
            if (Path.GetDirectoryName(vcsFileName).EndsWith("shaders-core/vfx") && vcsFileName.Contains("_pcgl_"))
            {
                return ARCHIVE.dotacore_pcgl;
            }
            if (Path.GetDirectoryName(vcsFileName).EndsWith("shaders/vfx") && vcsFileName.Contains("_pc_"))
            {
                return ARCHIVE.dotagame_pc;
            }
            if (Path.GetDirectoryName(vcsFileName).EndsWith("shaders-core/vfx") && vcsFileName.Contains("_pc_"))
            {
                return ARCHIVE.dotacore_pc;
            }
            if (vcsFileName.Contains("dota2-shaders-mobile-gles/core"))
            {
                return ARCHIVE.dota_core_gles;
            }
            if (vcsFileName.Contains("dota2-shaders-mobile-gles/dac"))
            {
                return ARCHIVE.dota_dac_gles;
            }
            if (Path.GetDirectoryName(vcsFileName).EndsWith("artifact-shaders-pc-core"))
            {
                return ARCHIVE.artifact_classiccore_pc;
            }
            if (Path.GetDirectoryName(vcsFileName).EndsWith("artifact-shaders-pc-dcg"))
            {
                return ARCHIVE.artifact_classicdcg_pc;
            }
            throw new ShaderParserException("don't know where this file belongs");
        }






    }
}





