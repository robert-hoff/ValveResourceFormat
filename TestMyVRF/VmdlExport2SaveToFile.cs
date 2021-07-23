using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SteamDatabase.ValvePak;
using MyValveResourceFormat;
using MyValveResourceFormat.Blocks;
using MyValveResourceFormat.IO;
using MyValveResourceFormat.ResourceTypes;

namespace TestMyVRF {
    class VmdlExport2SaveToFile {


        public static void RunTrials() {

            // tryStuff3();
            // tryStuff2();
            // tryStuff();

        }



        /*
         * Read a file as a byte[]
         *
         *
         */
        static void tryStuff2() {

            // reads the file, 8456 bytes
            byte[] databytes = File.ReadAllBytes(@"X:\checkouts\ValveResourceFormat\input-valve_c\screen_russel_desktop_12_example.vmat_c");
            Debug.WriteLine(databytes.Length);


            Resource resource = new MyValveResourceFormat.Resource();
            resource.Read(new MemoryStream(databytes));
        }





        static string EXPORT_DIR = @"C:\Users\R\Desktop\temttemp";


        static void tryStuff() {
            Package package = new SteamDatabase.ValvePak.Package();
            package.Read(@"X:\Steam\steamapps\common\Half-Life Alyx\game\hlvr\pak01_dir.vpk");
            PackageEntry model_entry = package.FindEntry("models/creatures/zombie_classic/zombie_classic.vmdl_c");
            package.ReadEntry(model_entry, out byte[] output); // this obtains the vmdl_c file as byte[] output - which is 44968687 bytes long

            Resource resource = new MyValveResourceFormat.Resource();
            resource.Read(new MemoryStream(output));
            ResourceData datablock = (ResourceData)resource.GetBlockByType(BlockType.DATA);


            var exporter = new GltfModelExporter {
                FileLoader = new MyFileLoader(package),
            };

            // string filename = "zombie_classic.vmdl_c";
            string filename = "zombie_classic.gltf";
            exporter.ExportToFile(filename, EXPORT_DIR + "\\" + filename, (Model)resource.DataBlock);
        }




        class MyFileLoader : IFileLoader {
            Package package;
            private readonly Dictionary<string, Resource> CachedResources = new Dictionary<string, Resource>();

            public MyFileLoader(Package package) {
                this.package = package;
            }

            public Resource LoadFile(string file) {
                Debug.WriteLine($"FILE LOADER: {file}");

                if (CachedResources.TryGetValue(file, out var resource) && resource.Reader != null) {
                    return resource;
                }
                resource = new Resource {
                    FileName = file,
                };

                var entry = package.FindEntry(file);
                if (entry != null) {
                    // Debug.WriteLine($"Loaded \"{file}\" from current vpk");

                    package.ReadEntry(entry, out var output, false);
                    resource.Read(new MemoryStream(output));
                    CachedResources[file] = resource;
                    return resource;
                }
                return null;
            }
        }



    }
}
