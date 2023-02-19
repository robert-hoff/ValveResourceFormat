using System;

#pragma warning disable CA1813 // Avoid unsealed attributes
namespace MyShaderFileKristiker.MyHelperClasses
{
    /*
     * The name of the enum can be used as the base folder for exporting any given archive
     *
     *      {name}-{platform}-{version}
     *
     */

    public enum ARCHIVE
    {
        [ArchiveDirectory("X:/dota-2-VRF-exports/dota2-export-shaders-pc-V65/shaders/vfx")]
        dota_game_pc_v65,
        [ArchiveDirectory("X:/dota-2-VRF-exports/dota2-export-shaders-pc-V65/shaders-core/vfx")]
        dota_core_pc_v65,
        [ArchiveDirectory("X:/dota-2-VRF-exports/dota2-export-shaders-vulkan-V65/shaders-core/vfx")]
        dota_core_vulkan_v65,
        [ArchiveDirectory("X:/dota-2-VRF-exports/dota2-export-shaders-vulkan-V65/shaders-game/vfx")]
        dota_game_vulkan_v65,
        [ArchiveDirectory("X:/dota-2-VRF-exports/dota2-export-shaders-pcgl/shaders-core/vfx")]
        dota_core_pcgl_v64,
        [ArchiveDirectory("X:/dota-2-VRF-exports/dota2-export-shaders-pcgl/shaders/vfx")]
        dota_game_pcgl_v64,
        [ArchiveDirectory("X:/dota-2-VRF-exports/dota2-export-shaders-pc/shaders-core/vfx")]
        dota_core_pc_v64,
        [ArchiveDirectory("X:/dota-2-VRF-exports/dota2-export-shaders-pc/shaders/vfx")]
        dota_game_pc_v64,
        [ArchiveDirectory("X:/dota-2-VRF-exports/dota2-shaders-mobile-gles/core")]
        dota_core_mobile_gles_v64,
        [ArchiveDirectory("X:/dota-2-VRF-exports/dota2-shaders-mobile-gles/dac")]
        dota_dac_mobile_gles_v64,
        [ArchiveDirectory("X:/dota-2-VRF-exports/dota2-shaders-android-vulkan/core")]
        dota_core_android_vulkan_v64,
        [ArchiveDirectory("X:/dota-2-VRF-exports/dota2-shaders-android-vulkan/dac")]
        dota_dac_android_vulkan_v64,
        [ArchiveDirectory("X:/dota-2-VRF-exports/dota2-shaders-ios-vulkan/core")]
        dota_core_ios_vulkan_v64,
        [ArchiveDirectory("X:/dota-2-VRF-exports/dota2-shaders-ios-vulkan/dac")]
        dota_dac_ios_vulkan_v64,
        [ArchiveDirectory("X:/artifact-classic-exports/artifact-shaders-pc-core")]
        artifact_classic_core_pc_v64,
        [ArchiveDirectory("X:/artifact-classic-exports/artifact-shaders-pc-dcg")]
        artifact_classic_dcg_pc_v64,
        [ArchiveDirectory("X:/hl2alyx-export/alyx-vulkan-core")]
        alyx_core_vulkan_v64,
        [ArchiveDirectory("X:/hl2alyx-export/alyx-vulkan-hlvr")]
        alyx_hlvr_vulkan_v64,
        [ArchiveDirectory("X:/dota-2-VRF-exports/manifest-20150907-pcgl-shader-v62/core")]
        dota_core_pcgl_v62,
        [ArchiveDirectory("X:/dota-2-VRF-exports/manifest-20150907-pcgl-shader-v62/dota")]
        dota_game_pcgl_v62,
        [ArchiveDirectory("X:/v62shaders-from-xpaw")]
        exampleset_pc_v62,
        [ArchiveDirectory("X:/Steam/steamapps/common/The Lab/RobotRepair/core/shaders/vfx")]
        the_lab_pc_v62,
        [ArchiveDirectory("X:/dota-2-VRF-exports/dota2-reduced-test-archive-pcgl-v64")]
        dota_testset_pcgl_v64,

        // -- experimental s&box shader, the versioning is set to 65 on the file I got while it is actually v64
        // [ArchiveDirectory("X:/s&box-shader-examples")]
        // sbox_shaders,
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class ArchiveDirectoryAttribute : Attribute
    {
        public string dirName { get; }
        public ArchiveDirectoryAttribute(string dirName)
        {
            this.dirName = dirName;
        }
    }
}
#pragma warning restore CA1813
