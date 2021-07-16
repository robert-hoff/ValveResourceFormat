using System;
using System.Diagnostics;
using System.IO;
using SteamDatabase.ValvePak;
using MyValveResourceFormat;
using MyValveResourceFormat.Blocks;
using MyValveResourceFormat.ResourceTypes;

namespace TestMyVRF {




    class Program {




        static void Mainz() {


            // trial2();
            trial1();

        }



        static string DOTA2_TEST_VPK = @"Z:\git\vcs-decompile\vrf-decompiler-bat\pak01.vpk";
        static string HUSKAR_WEAPON = "models/items/huskar/armor_of_reckless_vigor_weapon/armor_of_reckless_vigor_weapon.vmdl_c";

        static string HLALYX_TEST_VPK = @"Z:\git\vcs-decompile\vrf-decompiler-bat\gen_vpk\hl_dir.vpk";
        static string HLALYX_MAIN_VPK = @"X:\Steam\steamapps\common\Half-Life Alyx\game\hlvr\pak01_dir.vpk";
        static string ZOMBIE_CLASSIC = "models/creatures/zombie_classic/zombie_classic.vmdl_c";

        static string EXPORT_DIR = @"C:\Users\R\Desktop\temttemp";





        public static void trial1() {
            string vpkfile = HLALYX_TEST_VPK;
            var package = new SteamDatabase.ValvePak.Package();

            long millis = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            package.Read(vpkfile);

            string vpk_entry = ZOMBIE_CLASSIC;
            string filename = vpk_entry.Substring(vpk_entry.LastIndexOf("/") + 1);
            PackageEntry model_entry = package.FindEntry(vpk_entry);

            var resource = new MyValveResourceFormat.Resource();
            package.ReadEntry(model_entry, out byte[] output);
            Debug.WriteLine($"reading package {vpkfile} took {DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - millis} ms");

            // ouput.Length here is
            // 44968687
            // which matches the size of
            // models/creatures/zombie_classic/zombie_classic.vmdl_c



            millis = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            resource.Read(new MemoryStream(output));
            Debug.WriteLine($"parsing {vpk_entry} took {DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - millis} ms");
            ResourceData datablock = (ResourceData)resource.GetBlockByType(BlockType.DATA);


        }




    }




}







