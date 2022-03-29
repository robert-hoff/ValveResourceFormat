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
        private const string DOTA_CORE_PC_SOURCE_V65 = "X:/dota-2-VRF-exports/dota2-export-shaders-pc-V65/shaders-core/vfx";
        private const string DOTA_GAME_PC_SOURCE_V65 = "X:/dota-2-VRF-exports/dota2-export-shaders-pc-V65/shaders/vfx";
        private const string DOTA_CORE_VULKAN_SOURCE_V65 = "X:/dota-2-VRF-exports/dota2-export-shaders-vulkan-V65/shaders-core/vfx";
        private const string DOTA_GAME_VULKAN_SOURCE_V65 = "X:/dota-2-VRF-exports/dota2-export-shaders-vulkan-V65/shaders-game/vfx";
        private const string V62_EXAMPLES_SOURCE = "X:/v62shaders-from-xpaw";
        private const string THE_LAB_SOURCE_V62 = "X:/Steam/steamapps/common/The Lab/RobotRepair/core/shaders/vfx";

        public enum ARCHIVE
        {
            dotacore_pcgl, dotagame_pcgl, dotacore_pc, dotagame_pc,
            dotacore_pc_v65, dotagame_pc_v65,
            dotacore_vulkan_v65, dotagame_vulkan_v65,
            dota_core_gles, dota_dac_gles,
            artifact_classiccore_pc, artifact_classicdcg_pc,
            alyx_core_vulkan, alyx_hlvr_vulkan,
            v62_examples, the_lab_v62
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
            if (archive == ARCHIVE.dotacore_pc_v65) return DOTA_CORE_PC_SOURCE_V65;
            if (archive == ARCHIVE.dotagame_pc_v65) return DOTA_GAME_PC_SOURCE_V65;
            if (archive == ARCHIVE.dotacore_vulkan_v65) return DOTA_CORE_VULKAN_SOURCE_V65;
            if (archive == ARCHIVE.dotagame_vulkan_v65) return DOTA_GAME_VULKAN_SOURCE_V65;
            if (archive == ARCHIVE.dota_core_gles) return DOTA_CORE_MOBILE_GLES_SOURCE;
            if (archive == ARCHIVE.dota_dac_gles) return DOTA_DAC_MOBILE_GLES_SOURCE;
            if (archive == ARCHIVE.artifact_classiccore_pc) return ARTIFACT_CLASSIC_CORE_PC_SOURCE;
            if (archive == ARCHIVE.artifact_classicdcg_pc) return ARTIFACT_CLASSIC_DCG_PC_SOURCE;
            if (archive == ARCHIVE.alyx_hlvr_vulkan) return HLALYX_HLVR_VULKAN_SOURCE;
            if (archive == ARCHIVE.alyx_core_vulkan) return HLALYX_CORE_VULKAN_SOURCE;
            if (archive == ARCHIVE.v62_examples) return V62_EXAMPLES_SOURCE;
            if (archive == ARCHIVE.the_lab_v62) return THE_LAB_SOURCE_V62;
            throw new ShaderParserException("unknown archive");
        }

        // todo - can I write GetArchiveName(ARCHIVE archive) and GetPlatformType(ARCHIVE archive) as a single method?
        // todo - i.e. returning strings of the type "/dota-core/pcgl" (which can then be thought of as a directory)
        // todo - OR, return the archive name and platform type as a unique string, e.g. "dota-core-pcgl-v65"
        // (using "dota-core-pcgl-v65", etc as the new base directory for the given collection)
        public static string GetArchiveName(ARCHIVE archive)
        {
            if (archive == ARCHIVE.dotacore_pcgl) return "dota-core";
            if (archive == ARCHIVE.dotagame_pcgl) return "dota-game";
            if (archive == ARCHIVE.dotacore_pc) return "dota-core"; // fine to use the same name here, but need to use platform type in path names
            if (archive == ARCHIVE.dotagame_pc) return "dota-game";
            if (archive == ARCHIVE.dotacore_pc_v65) return "dota-core-v65";
            if (archive == ARCHIVE.dotagame_pc_v65) return "dota-game-v65";
            if (archive == ARCHIVE.dotacore_vulkan_v65) return "dota-core-v65";
            if (archive == ARCHIVE.dotagame_vulkan_v65) return "dota-game-v65";
            if (archive == ARCHIVE.dota_core_gles) return "dota-core";
            if (archive == ARCHIVE.dota_dac_gles) return "dota-game";
            if (archive == ARCHIVE.artifact_classiccore_pc) return "aclassic-core";
            if (archive == ARCHIVE.artifact_classicdcg_pc) return "aclassic-dcg";
            if (archive == ARCHIVE.alyx_hlvr_vulkan) return "alyx-hlvr";
            if (archive == ARCHIVE.alyx_core_vulkan) return "alyx-core";
            if (archive == ARCHIVE.v62_examples) return "v62-examples";
            if (archive == ARCHIVE.the_lab_v62) return "the-lab-v62";
            throw new ShaderParserException("unknown archive");
        }

        /*
         * Simplified platform name. For example
         * VULKAN, ANDROID-VULKAN and IOS-VULKAN all map to 'vulkan'
         * The important thing here is that {archivename}{platformType} is unique where
         *
         * archivename = GetArchiveName(ARCHIVE archive)        // method above
         * platformType = GetPlatformType(ARCHIVE archive)      // this method
         *
         *
         */
        public static string GetPlatformType(ARCHIVE archive)
        {
            if (archive == ARCHIVE.dotacore_pcgl) return "pcgl";
            if (archive == ARCHIVE.dotagame_pcgl) return "pcgl";
            if (archive == ARCHIVE.dotacore_pc) return "pc";
            if (archive == ARCHIVE.dotagame_pc) return "pc";
            if (archive == ARCHIVE.dotacore_pc_v65) return "pc";
            if (archive == ARCHIVE.dotagame_pc_v65) return "pc";
            if (archive == ARCHIVE.dotacore_vulkan_v65) return "vulkan";
            if (archive == ARCHIVE.dotagame_vulkan_v65) return "vulkan";
            if (archive == ARCHIVE.dota_core_gles) return "gles";
            if (archive == ARCHIVE.dota_dac_gles) return "gles";
            if (archive == ARCHIVE.artifact_classiccore_pc) return "pc";
            if (archive == ARCHIVE.artifact_classicdcg_pc) return "pc";
            if (archive == ARCHIVE.alyx_hlvr_vulkan) return "vulkan";
            if (archive == ARCHIVE.alyx_core_vulkan) return "vulkan";
            if (archive == ARCHIVE.v62_examples) return "pc";
            if (archive == ARCHIVE.the_lab_v62) return "pc";
            throw new ShaderParserException("unknown archive");
        }


        // todo - start using defined enums
        // todo - I want dxil and dxbc instead of pc (but this doesn't actually only depend on the archive)
        // dxil/dxbc also depends on the 'VcsShaderModelType', i.e. <30=dxil and >40=dxbc
        // todo - might want to name this PLATFORM type (and distinguish from 'source type')
        // todo - rewrite this as a context-specific method in FileVcsTokens

        /*
         * NOTE
         * The reason this shouldn't be here is because FileArchives class
         * can then be thought of as controlling the base folder of the server only
         * this token is used for generating the sub-folders of the generated source files
         *
         */
        public static string GetSourceType(ARCHIVE archive)
        {
            if (archive == ARCHIVE.dotacore_pcgl) return "glsl";
            if (archive == ARCHIVE.dotagame_pcgl) return "glsl";
            if (archive == ARCHIVE.dotacore_pc) return "pc";
            if (archive == ARCHIVE.dotagame_pc) return "pc";
            if (archive == ARCHIVE.dotacore_pc_v65) return "pc";
            if (archive == ARCHIVE.dotagame_pc_v65) return "pc";
            if (archive == ARCHIVE.dotacore_vulkan_v65) return "vulkan";
            if (archive == ARCHIVE.dotagame_vulkan_v65) return "vulkan";
            if (archive == ARCHIVE.dota_core_gles) return "gles";
            if (archive == ARCHIVE.dota_dac_gles) return "gles";
            if (archive == ARCHIVE.artifact_classiccore_pc) return "pc";
            if (archive == ARCHIVE.artifact_classicdcg_pc) return "pc";
            if (archive == ARCHIVE.alyx_hlvr_vulkan) return "vulkan";
            if (archive == ARCHIVE.alyx_core_vulkan) return "vulkan";
            if (archive == ARCHIVE.v62_examples) return "pc";
            if (archive == ARCHIVE.the_lab_v62) return "pc";
            throw new ShaderParserException("unknown archive");
        }


        // todo - this doesn't really belong here
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





