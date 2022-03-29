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
            dota_core_android_vulkan, dota_dac_android_vulkan,
            artifact_classiccore_pc, artifact_classicdcg_pc,
            alyx_core_vulkan, alyx_hlvr_vulkan,
            v62_examples, the_lab_v62
        };


        public static string GetServerBaseDir()
        {
            return SERVER_BASEDIR;
        }

        public static string GetServerTestDir()
        {
            return $"{SERVER_BASEDIR}{SERVER_TESTPATH}"; ;
        }

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
            if (archive == ARCHIVE.dota_core_android_vulkan) return DOTA_CORE_ANDROID_VULKAN_SOURCE;
            if (archive == ARCHIVE.dota_dac_android_vulkan) return DOTA_DAC_ANDROID_VULKAN_SOURCE;
            if (archive == ARCHIVE.artifact_classiccore_pc) return ARTIFACT_CLASSIC_CORE_PC_SOURCE;
            if (archive == ARCHIVE.artifact_classicdcg_pc) return ARTIFACT_CLASSIC_DCG_PC_SOURCE;
            if (archive == ARCHIVE.alyx_hlvr_vulkan) return HLALYX_HLVR_VULKAN_SOURCE;
            if (archive == ARCHIVE.alyx_core_vulkan) return HLALYX_CORE_VULKAN_SOURCE;
            if (archive == ARCHIVE.v62_examples) return V62_EXAMPLES_SOURCE;
            if (archive == ARCHIVE.the_lab_v62) return THE_LAB_SOURCE_V62;
            throw new ShaderParserException("unknown archive");
        }

        /*
         * This is used as the base folder name for the given collection
         * As long as each of these are unique the naming doesn't matter so much, stil, using the convention
         *
         *      {name}-{platform}-{version}
         *
         * Note simplified platform name. For example
         * VULKAN, ANDROID-VULKAN and IOS-VULKAN all map to 'vulkan'
         *
         */
        public static string GetArchiveName(ARCHIVE archive)
        {
            if (archive == ARCHIVE.dotacore_pcgl) return "dota-core-pcgl-v64";
            if (archive == ARCHIVE.dotagame_pcgl) return "dota-game-pcgl-v64";
            if (archive == ARCHIVE.dotacore_pc) return "dota-core-pc-v64";
            if (archive == ARCHIVE.dotagame_pc) return "dota-game-pc-v64";
            if (archive == ARCHIVE.dotacore_pc_v65) return "dota-core-pcgl-v65";
            if (archive == ARCHIVE.dotagame_pc_v65) return "dota-game-pcgl-v65";
            if (archive == ARCHIVE.dotacore_vulkan_v65) return "dota-core-vulkan-v65";
            if (archive == ARCHIVE.dotagame_vulkan_v65) return "dota-game-vulkan-v65";
            if (archive == ARCHIVE.dota_core_gles) return "dota-core-mobile-gles-v64";
            if (archive == ARCHIVE.dota_dac_gles) return "dota-game-mobile-gles-v64";
            if (archive == ARCHIVE.dota_core_android_vulkan) return "dota-core-android-vulkan-v64";
            if (archive == ARCHIVE.dota_dac_android_vulkan) return "dota-dac-android-vulkan-v64";
            if (archive == ARCHIVE.artifact_classiccore_pc) return "aclassic-core-pc-v64";
            if (archive == ARCHIVE.artifact_classicdcg_pc) return "aclassic-dcg-pc-v64";
            if (archive == ARCHIVE.alyx_hlvr_vulkan) return "alyx-hlvr-vulkan-v64";
            if (archive == ARCHIVE.alyx_core_vulkan) return "alyx-core-vulkan-v64";
            if (archive == ARCHIVE.v62_examples) return "exampleset-pc-v62";
            if (archive == ARCHIVE.the_lab_v62) return "the-lab-pc-v62";
            throw new ShaderParserException("unknown archive");
        }


    }
}



