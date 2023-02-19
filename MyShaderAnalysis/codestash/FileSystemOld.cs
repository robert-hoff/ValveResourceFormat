using System.IO;
using ShaderParserException = ValveResourceFormat.CompiledShader.ShaderParserException;
using VcsProgramType = ValveResourceFormat.CompiledShader.VcsProgramType;



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
namespace MyShaderAnalysis.codestash
{

    public class FileSystemOld
    {

        public const string PROJECT_TESTDIR = "Z:/active/projects/dota2-sourcesdk-modding/shader-analysis-vcs-format/OUTPUT_DUMP";
        public const string SERVER_BASEDIR = "Z:/dev/www/vcs.codecreation.dev";
        // public const string SERVER_BASEDIR = "Z:/dev/www/vcs.codecreation.dev-SERVERBACKUP-2021-08-24";
        // public const string SERVER_BASEDIR = "Z:/dev/www/vcs.codecreation.dev-SERVERBACKUP-2022-03-23";
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

        // v65 not registered as an enum
        public const string DOTA_CORE_PC_SOURCE_V65 = "X:/dota-2-VRF-exports/dota2-export-shaders-pc-V65/shaders-core/vfx";
        public const string DOTA_GAME_PC_SOURCE_V65 = "X:/dota-2-VRF-exports/dota2-export-shaders-pc-V65/shaders/vfx";

        // v62 not registered as an enum
        public const string V62_EXAMPLES_SOURCE = "X:/v62shaders-from-xpaw";
        public const string THE_LAB_SOURCE = "X:/Steam/steamapps/common/The Lab/RobotRepair/core/shaders/vfx";

        // Jan 2023 - what does 'not registered' as an enum mean?
        public const string DOTA_CORE_VULKAN_SOURCE_V65 = "X:/dota-2-VRF-exports/dota2-export-shaders-vulkan-V65/shaders-game/vfx";
        public const string DOTA_GAME_VULKAN_SOURCE_V65 = "X:/dota-2-VRF-exports/dota2-export-shaders-vulkan-V65/shaders-core/vfx";


        public enum ARCHIVE_OLD
        {
            dotacore_pcgl, dotagame_pcgl, dotacore_pc, dotagame_pc,
            dota_core_gles, dota_dac_gles,
            artifact_classiccore_pc, artifact_classicdcg_pc,
            hl_hlvr_vulkan, dota_game_vulkan_v65, dota_core_vulkan_v65
        };



        public static string GetFilenamepath(ARCHIVE_OLD archive, string filename)
        {
            return $"{GetSourceDir(archive)}/{filename}";
        }

        public static string GetServerBaseDir()
        {
            return SERVER_BASEDIR;
        }

        public static string GetServerTestDir()
        {
            return $"{SERVER_BASEDIR}{SERVER_TESTPATH}"; ;
        }



        // todo - it may be possible to write all of this into the enums
        public static string GetSourceDir(ARCHIVE_OLD archive)
        {
            if (archive == ARCHIVE_OLD.dotacore_pcgl) return DOTA_CORE_PCGL_SOURCE;
            if (archive == ARCHIVE_OLD.dotagame_pcgl) return DOTA_GAME_PCGL_SOURCE;
            if (archive == ARCHIVE_OLD.dotacore_pc) return DOTA_CORE_PC_SOURCE;
            if (archive == ARCHIVE_OLD.dotagame_pc) return DOTA_GAME_PC_SOURCE;
            if (archive == ARCHIVE_OLD.dota_core_gles) return DOTA_CORE_MOBILE_GLES_SOURCE;
            if (archive == ARCHIVE_OLD.dota_dac_gles) return DOTA_DAC_MOBILE_GLES_SOURCE;
            if (archive == ARCHIVE_OLD.artifact_classiccore_pc) return ARTIFACT_CLASSIC_CORE_PC_SOURCE;
            if (archive == ARCHIVE_OLD.artifact_classicdcg_pc) return ARTIFACT_CLASSIC_DCG_PC_SOURCE;
            if (archive == ARCHIVE_OLD.hl_hlvr_vulkan) return HLALYX_HLVR_VULKAN_SOURCE;
            if (archive == ARCHIVE_OLD.dota_core_vulkan_v65) return DOTA_CORE_VULKAN_SOURCE_V65;
            if (archive == ARCHIVE_OLD.dota_game_vulkan_v65) return DOTA_GAME_VULKAN_SOURCE_V65;
            throw new ShaderParserException("unknown archive");
        }

        public static string GetArchiveName(ARCHIVE_OLD archive)
        {
            if (archive == ARCHIVE_OLD.dotacore_pcgl) return "dota-core";
            if (archive == ARCHIVE_OLD.dotagame_pcgl) return "dota-game";
            if (archive == ARCHIVE_OLD.dotacore_pc) return "dota-core"; // fine to use the same name here, but need to use platform type in path names
            if (archive == ARCHIVE_OLD.dotagame_pc) return "dota-game";
            if (archive == ARCHIVE_OLD.dota_core_gles) return "dota-core";
            if (archive == ARCHIVE_OLD.dota_dac_gles) return "dota-game";
            if (archive == ARCHIVE_OLD.artifact_classiccore_pc) return "aclassic-core";
            if (archive == ARCHIVE_OLD.artifact_classicdcg_pc) return "aclassic-dcg";
            if (archive == ARCHIVE_OLD.hl_hlvr_vulkan) return "hlvr-vulkan";
            if (archive == ARCHIVE_OLD.dota_core_vulkan_v65) return "dota_core_vulkan_v65";
            if (archive == ARCHIVE_OLD.dota_game_vulkan_v65) return "dota_game_vulkan_v65";
            throw new ShaderParserException("unknown archive");
        }

        public static string GetArchiveLabel(ARCHIVE_OLD archive)
        {
            if (archive == ARCHIVE_OLD.dotacore_pcgl) return "core";
            if (archive == ARCHIVE_OLD.dotagame_pcgl) return "dota";
            // todo - fix this, don't use archive-label as part of pathname
            if (archive == ARCHIVE_OLD.dotacore_pc) return "core-pc"; // the archive-labels need to be distinct because I'm using them as part
                                                                  // of the byte-dump paths
            if (archive == ARCHIVE_OLD.dotagame_pc) return "dota-pc";
            if (archive == ARCHIVE_OLD.dota_core_gles) return "core-gles";
            if (archive == ARCHIVE_OLD.dota_dac_gles) return "dac-gles";
            if (archive == ARCHIVE_OLD.artifact_classiccore_pc) return "artifact-core";
            if (archive == ARCHIVE_OLD.artifact_classicdcg_pc) return "artifact-dcg";
            if (archive == ARCHIVE_OLD.hl_hlvr_vulkan) return "hlvr-vulkan";
            if (archive == ARCHIVE_OLD.dota_core_vulkan_v65) return "dota-core-vulkan-v65";
            if (archive == ARCHIVE_OLD.dota_game_vulkan_v65) return "dota-game-vulkan-v65";
            throw new ShaderParserException("unknown archive");
        }

        // todo - may want to change this to 'platform'
        public static string GetGpuType(ARCHIVE_OLD archive)
        {
            if (archive == ARCHIVE_OLD.dotacore_pcgl) return "pcgl";
            if (archive == ARCHIVE_OLD.dotagame_pcgl) return "pcgl";
            if (archive == ARCHIVE_OLD.dotacore_pc) return "pc";
            if (archive == ARCHIVE_OLD.dotagame_pc) return "pc";
            if (archive == ARCHIVE_OLD.dota_core_gles) return "gles";
            if (archive == ARCHIVE_OLD.dota_dac_gles) return "gles";
            if (archive == ARCHIVE_OLD.artifact_classiccore_pc) return "pc";
            if (archive == ARCHIVE_OLD.artifact_classicdcg_pc) return "pc";
            if (archive == ARCHIVE_OLD.hl_hlvr_vulkan) return "vulkan";
            if (archive == ARCHIVE_OLD.dota_core_vulkan_v65) return "vulkan";
            if (archive == ARCHIVE_OLD.dota_game_vulkan_v65) return "vulkan";
            throw new ShaderParserException("unknown archive");
        }

        // todo - start using defined enums
        public static string GetSourceType(ARCHIVE_OLD archive)
        {
            if (archive == ARCHIVE_OLD.dotacore_pcgl) return "glsl";
            if (archive == ARCHIVE_OLD.dotagame_pcgl) return "glsl";
            if (archive == ARCHIVE_OLD.dotacore_pc) return "pc";
            if (archive == ARCHIVE_OLD.dotagame_pc) return "pc";
            if (archive == ARCHIVE_OLD.dota_core_gles) return "gles";
            if (archive == ARCHIVE_OLD.dota_dac_gles) return "gles";
            if (archive == ARCHIVE_OLD.artifact_classiccore_pc) return "pc"; // todo - I want dxil and dxbc here
            if (archive == ARCHIVE_OLD.artifact_classicdcg_pc) return "pc";
            if (archive == ARCHIVE_OLD.hl_hlvr_vulkan) return "vulkan";
            if (archive == ARCHIVE_OLD.dota_core_vulkan_v65) return "vulkan";
            if (archive == ARCHIVE_OLD.dota_game_vulkan_v65) return "vulkan";
            throw new ShaderParserException("unknown archive");
        }

        public static string GetVcsToken(VcsProgramType vcsFiletype)
        {
            if (vcsFiletype == VcsProgramType.Features) return "ft"; ;
            if (vcsFiletype == VcsProgramType.VertexShader) return "vs"; ;
            if (vcsFiletype == VcsProgramType.PixelShader) return "ps"; ;
            if (vcsFiletype == VcsProgramType.PixelShaderRenderState) return "psrs"; ;
            if (vcsFiletype == VcsProgramType.ComputeShader) return "cs"; ;
            if (vcsFiletype == VcsProgramType.HullShader) return "hs"; ;
            if (vcsFiletype == VcsProgramType.DomainShader) return "ds"; ;
            if (vcsFiletype == VcsProgramType.GeometryShader) return "gs"; ;
            if (vcsFiletype == VcsProgramType.RaytracingShader) return "rtx"; ;
            throw new ShaderParserException($"Unknown vcsFileType {vcsFiletype}");
        }


        // todo - problem here with the backslashes
        // it's not consistent, because Path.GetDirectory() always returns string with backslashes
        // I sometimes need to make comparisong with backslashes in the name, in other places I make comparisong
        // with forward slashes
        public static ARCHIVE_OLD DetermineArchiveType(string vcsFileName)
        {
            // the built-in method Path.GetDirectoryName always returns a path containing back-slashes
            if (Path.GetDirectoryName(vcsFileName).EndsWith("shaders\\vfx") && vcsFileName.Contains("_pcgl_"))
            {
                return ARCHIVE_OLD.dotagame_pcgl;
            }
            if (Path.GetDirectoryName(vcsFileName).EndsWith("shaders-core\\vfx") && vcsFileName.Contains("_pcgl_"))
            {
                return ARCHIVE_OLD.dotacore_pcgl;
            }
            if (Path.GetDirectoryName(vcsFileName).EndsWith("shaders\\vfx") && vcsFileName.Contains("_pc_"))
            {
                return ARCHIVE_OLD.dotagame_pc;
            }
            if (Path.GetDirectoryName(vcsFileName).EndsWith("shaders-core\\vfx") && vcsFileName.Contains("_pc_"))
            {
                return ARCHIVE_OLD.dotacore_pc;
            }
            if (vcsFileName.Contains("dota2-shaders-mobile-gles/core"))
            {
                return ARCHIVE_OLD.dota_core_gles;
            }
            if (vcsFileName.Contains("dota2-shaders-mobile-gles/dac"))
            {
                return ARCHIVE_OLD.dota_dac_gles;
            }
            if (Path.GetDirectoryName(vcsFileName).EndsWith("artifact-shaders-pc-core"))
            {
                return ARCHIVE_OLD.artifact_classiccore_pc;
            }
            if (Path.GetDirectoryName(vcsFileName).EndsWith("artifact-shaders-pc-dcg"))
            {
                return ARCHIVE_OLD.artifact_classicdcg_pc;
            }
            if (Path.GetDirectoryName(vcsFileName).EndsWith("alyx-vulkan-hlvr"))
            {
                return ARCHIVE_OLD.hl_hlvr_vulkan;
            }
            if (Path.GetDirectoryName(vcsFileName).EndsWith("dota2-export-shaders-vulkan-V65\\shaders-core\\vfx"));
            {
                return ARCHIVE_OLD.dota_core_vulkan_v65;
            }
            if (Path.GetDirectoryName(vcsFileName).EndsWith("dota2-export-shaders-vulkan-V65\\shaders-game\\vfx"));
            {
                return ARCHIVE_OLD.dota_core_vulkan_v65;
            }
            throw new ShaderParserException("don't know where this file belongs");
        }
    }
}


