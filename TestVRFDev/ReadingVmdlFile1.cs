using System;
using System.Diagnostics;
using System.IO;
using SteamDatabase.ValvePak;
using ValveResourceFormat;
using ValveResourceFormat.Blocks;
using ValveResourceFormat.IO;
using ValveResourceFormat.ResourceTypes;

namespace TestVRFDev
{
    class ReadingVmdlFile1
    {
        static void Mainz()
        {
            // trial3ReadDataBlock();
            // trial2();
            trial1();
        }

        public const string DOTA2_TEST_VPK = @"Z:\git\vcs-decompile\vrf-decompiler-bat\pak01.vpk";
        public const string HUSKAR_WEAPON = "models/items/huskar/armor_of_reckless_vigor_weapon/armor_of_reckless_vigor_weapon.vmdl_c";
        public const string HLALYX_TEST_VPK = @"Z:\git\vcs-decompile\vrf-decompiler-bat\gen_vpk\hl_dir.vpk";
        public const string HLALYX_MAIN_VPK = @"X:\Steam\steamapps\common\Half-Life Alyx\game\hlvr\pak01_dir.vpk";
        public const string ZOMBIE_CLASSIC = "models/creatures/zombie_classic/zombie_classic.vmdl_c";
        public const string EXPORT_DIR = @"C:\Users\R\Desktop\temttemp";

        public static void trial3ReadDataBlock()
        {
            Package package = new SteamDatabase.ValvePak.Package();
            package.Read(@"X:\Steam\steamapps\common\Half-Life Alyx\game\hlvr\pak01_dir.vpk");
            PackageEntry model_entry = package.FindEntry("models/creatures/zombie_classic/zombie_classic.vmdl_c");
            package.ReadEntry(model_entry, out byte[] output); // this obtains the vmdl_c file as byte[] output - which is 44968687 bytes long

            Resource resource = new ValveResourceFormat.Resource();
            resource.Read(new MemoryStream(output));
            ResourceData datablock = (ResourceData) resource.GetBlockByType(BlockType.DATA);

            ValveResourceFormat.ResourceTypes.Model datablock_to_model = (ValveResourceFormat.ResourceTypes.Model) datablock;
        }

        public static void trial2()
        {
            string vpkfile = HLALYX_TEST_VPK;
            SteamDatabase.ValvePak.Package package = new SteamDatabase.ValvePak.Package();
            package.Read(vpkfile);

            string vpk_entry = "models/creatures/zombie_classic/zombie_classic.vmdl_c";
            // string filename = vpk_entry.Substring(vpk_entry.LastIndexOf("/") + 1);

            SteamDatabase.ValvePak.PackageEntry model_entry = package.FindEntry(vpk_entry);

            package.ReadEntry(model_entry, out byte[] output);

            ValveResourceFormat.Resource resource = new ValveResourceFormat.Resource();

            // ouput.Length here is
            // 44968687
            // which matches the size of
            // models/creatures/zombie_classic/zombie_classic.vmdl_c

            long millis = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            resource.Read(new MemoryStream(output));
            Debug.WriteLine($"reading {vpkfile} took {DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - millis} ms");
            ValveResourceFormat.Blocks.ResourceData datablock = (ResourceData) resource.GetBlockByType(BlockType.DATA);
        }

        public static void trial1()
        {
            string vpkfile = HLALYX_TEST_VPK;
            var package = new SteamDatabase.ValvePak.Package();
            package.Read(vpkfile);

            string vpk_entry = ZOMBIE_CLASSIC;

            // R: remember I can use built in functions for this! e.g. Path.GetDirectoryName(fileName)
            string filename = vpk_entry.Substring(vpk_entry.LastIndexOf("/") + 1);

            PackageEntry model_entry = package.FindEntry(vpk_entry);

            var resource = new ValveResourceFormat.Resource();
            package.ReadEntry(model_entry, out byte[] output);

            long millis = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            resource.Read(new MemoryStream(output));

            Debug.WriteLine($"reading {vpkfile} took {DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - millis} ms");

            // For the GltfModelExporter to work we need to instantiate it with a class implementing
            //
            //			ValveResourceFormat.IO.IFileLoader
            //

            MyFileLoader myFileLoader = new MyFileLoader(package);

            var exporter = new GltfModelExporter
            {
                FileLoader = myFileLoader,
            };

            ResourceData datablock = (ResourceData) resource.GetBlockByType(BlockType.DATA);

            exporter.ExportToFile(filename, EXPORT_DIR + "\\" + filename, (Model) resource.DataBlock, null);
        }

        class MyFileLoader : IFileLoader
        {
            Package package;
            public MyFileLoader(Package package)
            {
                this.package = package;
            }

            public Resource LoadFile(string file)
            {
                Debug.WriteLine(file);
                return null;
            }
        }
    }
}

