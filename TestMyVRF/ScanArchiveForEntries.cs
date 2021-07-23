using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using SteamDatabase.ValvePak;
using MyValveResourceFormat;
using MyValveResourceFormat.Blocks;
using MyValveResourceFormat.IO;
using MyValveResourceFormat.ResourceTypes;
using MyValveResourceFormat.Serialization;
using MyValveResourceFormat.Serialization.KeyValues;
using MyValveResourceFormat.Serialization.NTRO;


namespace TestMyVRF {
    class ScanArchiveForEntries {


        public static void RunTrials() {

            trial1();


        }


        const string DOTA2_VPK_ARCHIVE = @"X:\Steam\steamapps\common\dota 2 beta\game\dota\pak01_dir.vpk";




        static void trial1() {

            Package package = GetVpkPackage(DOTA2_VPK_ARCHIVE);
            List<PackageEntry> vmat_entries = getVmatcPackageEntries(package);



            object c = package.Entries;


            foreach (PackageEntry packageEntry in vmat_entries) {

                package.ReadEntry(packageEntry, out byte[] output);
                Resource resource = new Resource();
                resource.Read(new MemoryStream(output));

                Material materialdata = (Material)resource.GetBlockByType(BlockType.DATA);
                KeyValuesOrNTRO kv_or_ntro_block = (KeyValuesOrNTRO)materialdata;
                IKeyValueCollection kv_collection = kv_or_ntro_block.Data;


                // IKeyValueCollection[] textureAttributes = kv_collection.GetArray<string>("m_textureAttributes");
                string[] textureAttributes = kv_collection.GetArray<string>("m_textureAttributes");


                object obj_textatt = kv_collection.GetProperty<object>("m_textureAttributes");

                if (textureAttributes.Count() > 0) {


                }

                if (obj_textatt.GetType() == typeof(KVObject)) {

                    KVObject kv_obj = (KVObject)obj_textatt;
                    if (kv_obj.Properties.Count>0) {

                    }


                }




                if (textureAttributes != null) {


                }



            }




        }




        static List<PackageEntry> getVtexcPackageEntries(Package package) {
            Dictionary<string, List<PackageEntry>> entries = package.Entries;
            entries.TryGetValue("vtex_c", out List<PackageEntry> vmat_entries);
            return vmat_entries;
        }


        static List<PackageEntry> getVmatcPackageEntries(Package package) {
            Dictionary<string, List<PackageEntry>> entries = package.Entries;
            entries.TryGetValue("vmat_c", out List<PackageEntry> vmat_entries);
            return vmat_entries;
        }

        static Package GetVpkPackage(string vpk_archive) {
            Package package = new Package();
            package.Read(vpk_archive);
            return package;
        }




    }
}
