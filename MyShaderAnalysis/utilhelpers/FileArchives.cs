using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using ValveResourceFormat.CompiledShader;
using static ValveResourceFormat.CompiledShader.ShaderUtilHelpers;

/*
 *
 * FileArchives
 *
 * Responsibility of FileArchives is to return tokens specific to base folder and base url of the archive only
 * All the methods are constant to the ARCHIVE enums
 *
 *
 *
 *
 */
namespace MyShaderAnalysis.utilhelpers
{
    public class FileArchives
    {
        private const string PROJECT_TESTDIR = "Z:/active/projects/dota2-sourcesdk-modding/shader-analysis-vcs-format/OUTPUT_DUMP";
        private const string SERVER_BASEDIR = "Z:/dev/www/vcs.codecreation.dev";
        // private const string SERVER_BASEDIR = "Z:/dev/www/vcs.codecreation.dev-SERVERBACKUP-2021-08-24";
        // private const string SERVER_BASEDIR = "Z:/dev/www/vcs.codecreation.dev-SERVERBACKUP-2022-03-23";
        private const string SERVER_TESTPATH = "/GEN-output";
        private const string DOTA_CORE_PCGL_SOURCE = "X:/dota-2-VRF-exports/dota2-export-shaders-pcgl/shaders-core/vfx";
        private const string DOTA_GAME_PCGL_SOURCE = "X:/dota-2-VRF-exports/dota2-export-shaders-pcgl/shaders/vfx";
        private const string DOTA_CORE_PC_SOURCE = "X:/dota-2-VRF-exports/dota2-export-shaders-pc/shaders-core/vfx";
        private const string DOTA_GAME_PC_SOURCE = "X:/dota-2-VRF-exports/dota2-export-shaders-pc/shaders/vfx";
        private const string DOTA_CORE_MOBILE_GLES_SOURCE = "X:/dota-2-VRF-exports/dota2-shaders-mobile-gles/core";
        private const string DOTA_DAC_MOBILE_GLES_SOURCE = "X:/dota-2-VRF-exports/dota2-shaders-mobile-gles/dac";
        private const string DOTA_CORE_ANDROID_VULKAN_SOURCE = "X:/dota-2-VRF-exports/dota2-shaders-android-vulkan/core";
        private const string DOTA_DAC_ANDROID_VULKAN_SOURCE = "X:/dota-2-VRF-exports/dota2-shaders-android-vulkan/dac";
        private const string DOTA_CORE_IOS_VULKAN_SOURCE = "X:/dota-2-VRF-exports/dota2-shaders-ios-vulkan/core";
        private const string DOTA_DAC_IOS_VULKAN_SOURCE = "X:/dota-2-VRF-exports/dota2-shaders-ios-vulkan/dac";
        private const string ARTIFACT_CLASSIC_CORE_PC_SOURCE = "X:/artifact-classic-exports/artifact-shaders-pc-core";
        private const string ARTIFACT_CLASSIC_DCG_PC_SOURCE = "X:/artifact-classic-exports/artifact-shaders-pc-dcg";
        private const string HLALYX_CORE_VULKAN_SOURCE = "X:/hl2alyx-export/alyx-vulkan-core";
        private const string HLALYX_HLVR_VULKAN_SOURCE = "X:/hl2alyx-export/alyx-vulkan-hlvr";

        // v65 not registered as an enum
        private const string DOTA_CORE_PC_SOURCE_V65 = "X:/dota-2-VRF-exports/dota2-export-shaders-pc-V65/shaders-core/vfx";
        private const string DOTA_GAME_PC_SOURCE_V65 = "X:/dota-2-VRF-exports/dota2-export-shaders-pc-V65/shaders/vfx";

        // v62 not registered as an enum
        private const string V62_EXAMPLES_SOURCE = "X:/v62shaders-from-xpaw";
        private const string THE_LAB_SOURCE = "X:/Steam/steamapps/common/The Lab/RobotRepair/core/shaders/vfx";

        public enum ARCHIVE
        {
            dotacore_pcgl, dotagame_pcgl, dotacore_pc, dotagame_pc,
            dota_core_gles, dota_dac_gles,
            artifact_classiccore_pc, artifact_classicdcg_pc,
            hl_hlvr_vulkan
        };


        public static string GetFilenamepath(ARCHIVE archive, string filename)
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
            if (archive == ARCHIVE.hl_hlvr_vulkan) return HLALYX_HLVR_VULKAN_SOURCE;
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
            if (archive == ARCHIVE.hl_hlvr_vulkan) return "hlvr-vulkan";
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
            if (archive == ARCHIVE.hl_hlvr_vulkan) return "hlvr-vulkan";
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
            if (archive == ARCHIVE.hl_hlvr_vulkan) return "vulkan";
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
            if (archive == ARCHIVE.hl_hlvr_vulkan) return "vulkan";
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


    }
}





