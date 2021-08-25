using MyValveResourceFormat;
using MyValveResourceFormat.Blocks;
using MyValveResourceFormat.IO;
using SteamDatabase.ValvePak;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;


namespace TestMyVRF {
    class ExportVmatcData {


        public static void RunTrials() {
            // trial1();
            trial2();
        }


        public const string EXPORT_DIR = @"C:\Users\R\Desktop\temptemp";
        const string DOTA2_VPK_ARCHIVE = @"X:\Steam\steamapps\common\dota 2 beta\game\dota\pak01_dir.vpk";
        const string HLALYX_VPK_ARCHIVE = @"X:\Steam\steamapps\common\Half-Life Alyx\game\hlvr\pak01_dir.vpk";



        static void trial2() {
            Package package = getVpkPackage(DOTA2_VPK_ARCHIVE);
            List<PackageEntry> vmat_entries = getVmatcPackageEntries(package);

            int count = 0;

            foreach (PackageEntry packageEntry in vmat_entries) {

                package.ReadEntry(packageEntry, out byte[] output);
                Resource resource = new MyValveResourceFormat.Resource();
                resource.Read(new MemoryStream(output));


                string filename = $"{packageEntry.DirectoryName}/{packageEntry.FileName}.{packageEntry.TypeName}";
                //Debug.WriteLine(filename);


                ExportFile.Export(filename, resource);

                count++;
                if (count==1000000) {
                    break;
                }


            }
        }




        static void trial1() {

            //ExportFile.Export(fileName, new ExportData {
            //    Resource = resource,
            //    VrfGuiContext = new VrfGuiContext(null, package),
            //});

            Package package = new SteamDatabase.ValvePak.Package();
            package.Read(@"Z:\git\vcs-decompile\vrf-decompiler-bat\pak01.vpk");
            PackageEntry model_entry = package.FindEntry("asdfsdaf.vmat_c");
            package.ReadEntry(model_entry, out byte[] output); // this obtains the vmat_c file as byte[] output

            Resource resource = new Resource();
            resource.Read(new MemoryStream(output));
            // ResourceData datablock = (ResourceData)resource.GetBlockByType(BlockType.DATA);

            string filename = "asdfsdaf.vmat_c";
            ExportFile.Export(filename, resource);
        }


        class MyFileLoader : IFileLoader {
            Package package;
            private readonly Dictionary<string, Resource> CachedResources = new Dictionary<string, Resource>();

            public MyFileLoader(Package package) {
                this.package = package;
            }

            public Resource LoadFile(string file) {
                Debug.WriteLine($"FILE LOADER: {file}");
                // return null;


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




        static Package getVpkPackage(string vpk_archive) {
            Package package = new SteamDatabase.ValvePak.Package();
            package.Read(vpk_archive);
            return package;
        }
        static List<PackageEntry> getVmatcPackageEntries(Package package) {
            Dictionary<string, List<PackageEntry>> entries = package.Entries;
            entries.TryGetValue("vmat_c", out List<PackageEntry> vmat_entries);
            return vmat_entries;
        }


    }
}
