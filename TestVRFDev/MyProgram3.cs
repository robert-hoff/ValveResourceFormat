using System.Diagnostics;
using ValveResourceFormat;

#pragma warning disable IDE0060 // Remove unused parameter
namespace TestVRFDev
{
    public static class MyProgram3
    {
        public static void Mainz(string[] args)
        {
            string updated_file = @"X:\checkouts\ValveResourceFormat\Tests\FilesLatest\shading_pen_lines_psd_4cab66cb.vtex_c";

            Resource resource = new();
            resource.Read(updated_file);

            string data = resource.GetBlockByType(BlockType.REDI).ToString();
            Trace.WriteLine(data);
            resource.Dispose();
        }

        /*
		 * This file armor_of_reckless_vigor_weapon_4143600f.vagrp_c
		 * is references by the DATA block of armor_of_reckless_vigor_weapon.vmdl
		 *
		 *
		 *
		 */
        public static void GetDataBlocksFromFile()
        {
            // Exists in
            // X:\dota-2-VRF-exports\dota2-export-decompiled\models\items\huskar\armor_of_reckless_vigor_weapon
            // string old_file = @"X:\checkouts\ValveResourceFormat\Tests\Files\armor_of_reckless_vigor_weapon_4143600f.vagrp_c";
            string updated_file = @"X:\checkouts\ValveResourceFormat\Tests\FilesLatest\armor_of_reckless_vigor_weapon_4143600f.vagrp_c";

            var resource = new Resource();
            resource.Read(updated_file);

            var data = resource.GetBlockByType(BlockType.REDI).ToString();
            Trace.WriteLine(data);
            resource.Dispose();
        }
    }
}

