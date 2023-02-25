using System.Diagnostics;
using ValveResourceFormat;

namespace TestVRFDev
{
    class MyProgram3
    {
        static void Mainz(string[] args)
        {
            // Trace.WriteLine("Hello!");
            string updated_file = @"X:\checkouts\ValveResourceFormat\Tests\FilesLatest\shading_pen_lines_psd_4cab66cb.vtex_c";

            var resource = new Resource();
            resource.Read(updated_file);

            var data = resource.GetBlockByType(BlockType.REDI).ToString();
            // Trace.WriteLine(data);
        }

        /*
		 * This file armor_of_reckless_vigor_weapon_4143600f.vagrp_c
		 * is references by the DATA block of armor_of_reckless_vigor_weapon.vmdl
		 *
		 *
		 *
		 */
        static void getDataBlocksFromFile()
        {
            // Exists in
            // X:\dota-2-VRF-exports\dota2-export-decompiled\models\items\huskar\armor_of_reckless_vigor_weapon
            // string old_file = @"X:\checkouts\ValveResourceFormat\Tests\Files\armor_of_reckless_vigor_weapon_4143600f.vagrp_c";
            string updated_file = @"X:\checkouts\ValveResourceFormat\Tests\FilesLatest\armor_of_reckless_vigor_weapon_4143600f.vagrp_c";

            var resource = new Resource();
            resource.Read(updated_file);

            var data = resource.GetBlockByType(BlockType.REDI).ToString();
            Trace.WriteLine(data);
        }
    }
}

