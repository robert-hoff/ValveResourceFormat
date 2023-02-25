using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using SteamDatabase.ValvePak;
using ValveResourceFormat;
using ValveResourceFormat.Blocks;
using ValveResourceFormat.IO;
using ValveResourceFormat.ResourceTypes;

#pragma warning disable IDE0059 // Unnecessary assignment of a value
namespace TestVRFDev
{
    class ReadingVmdlFile2
    {
        public static void RunTrials()
        {
            Trial4ReadDataBlock();
            // Trial3ReadDataBlock();
            // Trial2ReadDataBlock();
            // Trial1ReadDataBlock();
        }

        private const string EXPORT_DIR = @"C:\Users\R\Desktop\temttemp";

        public static void Trial4ReadDataBlock()
        {
            Package package = new Package();
            package.Read(@"X:\Steam\steamapps\common\Half-Life Alyx\game\hlvr\pak01_dir.vpk");
            PackageEntry model_entry = package.FindEntry("models/creatures/zombie_classic/zombie_classic.vmdl_c");
            package.ReadEntry(model_entry, out byte[] output); // this obtains the vmdl_c file as byte[] output - which is 44968687 bytes long
            package.Dispose();

            // Resource resource = new ValveResourceFormat.Resource();

            // when running the GUI resource is instantiated like this (but it doesn't seem to make any difference)
            // so it's unclear why this is done ..
            Resource resource = new Resource
            {
                FileName = "zombie_classic.vmdl_c",
            };

            resource.Read(new MemoryStream(output));
            resource.Dispose();
            // ResourceData datablock = (ResourceData) resource.GetBlockByType(BlockType.DATA);
            _ = (ResourceData) resource.GetBlockByType(BlockType.DATA);
        }

        public static void Trial3ReadDataBlock()
        {
            Package package = new Package();
            package.Read(@"X:\Steam\steamapps\common\Half-Life Alyx\game\hlvr\pak01_dir.vpk");
            PackageEntry model_entry = package.FindEntry("models/creatures/zombie_classic/zombie_hanging.vmdl_c");
            package.ReadEntry(model_entry, out byte[] output); // this obtains the vmdl_c file as byte[] output - which is 44968687 bytes long

            Resource resource = new Resource();
            resource.Read(new MemoryStream(output));
            ResourceData datablock = (ResourceData) resource.GetBlockByType(BlockType.DATA);


            // ValveResourceFormat.ResourceTypes.Model datablock_to_model = (ValveResourceFormat.ResourceTypes.Model)datablock;

            var exporter = new GltfModelExporter
            {
                FileLoader = new MyFileLoader(package),
            };

            string filename = "zombie_hanging.vmdl_c";
            exporter.ExportToFile(filename, EXPORT_DIR + "\\" + filename, (Model) resource.DataBlock, null);
            resource.Dispose();
        }

        public static void Trial2ReadDataBlock()
        {
            Package package = new Package();
            package.Read(@"X:\Steam\steamapps\common\Half-Life Alyx\game\hlvr\pak01_dir.vpk");
            PackageEntry model_entry = package.FindEntry("models/creatures/zombie_classic/zombie_classic.vmdl_c");
            package.ReadEntry(model_entry, out byte[] output); // this obtains the vmdl_c file as byte[] output - which is 44968687 bytes long

            Resource resource = new Resource();
            resource.Read(new MemoryStream(output));
            ResourceData datablock = (ResourceData) resource.GetBlockByType(BlockType.DATA);

            // ValveResourceFormat.ResourceTypes.Model datablock_to_model = (ValveResourceFormat.ResourceTypes.Model)datablock;

            var exporter = new GltfModelExporter
            {
                FileLoader = new MyFileLoader(package),
            };

            string filename = "zombie_classic.vmdl_c";
            exporter.ExportToFile(filename, EXPORT_DIR + "\\" + filename, (Model) resource.DataBlock, null);
            resource.Dispose();
        }

        public static void Trial1ReadDataBlock()
        {
            Package package = new Package();
            package.Read(@"Z:\git\vcs-decompile\vrf-decompiler-bat\pak01.vpk");
            PackageEntry model_entry = package.FindEntry("models/items/huskar/armor_of_reckless_vigor_weapon/armor_of_reckless_vigor_weapon.vmdl_c");
            package.ReadEntry(model_entry, out byte[] output); // this obtains the vmdl_c file as byte[] output - which is 44968687 bytes long

            Resource resource = new Resource();
            resource.Read(new MemoryStream(output));
            ResourceData datablock = (ResourceData) resource.GetBlockByType(BlockType.DATA);

            // ValveResourceFormat.ResourceTypes.Model datablock_to_model = (ValveResourceFormat.ResourceTypes.Model)datablock;

            var exporter = new GltfModelExporter
            {
                FileLoader = new MyFileLoader(package),
            };

            string resourceName = "armor_of_reckless_vigor_weapon.vmdl_c";
            // The target type gltf or glb will be inferred from the fileNameOutput string
            // This inference may be performed by the third party class SharpGLTF.Schema2
            // because it seems the outpath is sent to it as stated here
            string fileNameOutput = @"C:\Users\R\Desktop\temttemp\armor_of_reckless_vigor_weapon.gltf";
            // string fileNameOutput = @"C:\Users\R\Desktop\temttemp\armor_of_reckless_vigor_weapon.vmdl_c";
            exporter.ExportToFile(resourceName, fileNameOutput, (Model) resource.DataBlock, null);
            resource.Dispose();
        }

        class MyFileLoader : IFileLoader
        {
            Package package;
            private readonly Dictionary<string, Resource> CachedResources = new();

            public MyFileLoader(Package package)
            {
                this.package = package;
            }

            public Resource LoadFile(string file)
            {
                Debug.WriteLine($"FILE LOADER: {file}");
                // return null;

                if (CachedResources.TryGetValue(file, out var resource) && resource.Reader != null)
                {
                    return resource;
                }
                resource = new Resource
                {
                    FileName = file,
                };

                var entry = package.FindEntry(file);
                if (entry != null)
                {
                    // Debug.WriteLine($"Loaded \"{file}\" from current vpk");

                    package.ReadEntry(entry, out var output, false);
                    resource.Read(new MemoryStream(output));
                    CachedResources[file] = resource;
                    return resource;
                }
                resource.Dispose();
                return null;
            }
        }
    }
}

