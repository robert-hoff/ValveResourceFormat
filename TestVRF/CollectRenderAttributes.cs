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
using ValveResourceFormat;
using ValveResourceFormat.Blocks;
using ValveResourceFormat.IO;
using ValveResourceFormat.ResourceTypes;
using ValveResourceFormat.Serialization;
using ValveResourceFormat.Serialization.KeyValues;
using ValveResourceFormat.Serialization.NTRO;
using ValveResourceFormat.ThirdParty;

namespace TestVRF
{

    class CollectRenderAttributes
    {
        static Dictionary<uint, string> externalVarsReference = new Dictionary<uint, string>();

        public static void collectAttributes(String vpk_archive)
        {
            Package package = getVpkPackage(vpk_archive);
            List<PackageEntry> vmat_entries = getVmatcPackageEntries(package);

            foreach (PackageEntry packageEntry in vmat_entries)
            {
                package.ReadEntry(packageEntry, out byte[] output);
                Resource resource = new ValveResourceFormat.Resource();
                resource.Read(new MemoryStream(output));
                Material materialdata = (Material)resource.GetBlockByType(BlockType.DATA);
                KeyValuesOrNTRO kv_or_ntro_block = (KeyValuesOrNTRO)materialdata;
                IKeyValueCollection kv_collection = kv_or_ntro_block.Data;

                string[] renderAttributesUsed = kv_collection.GetArray<string>("m_renderAttributesUsed");
                foreach (string s in renderAttributesUsed)
                {
                    uint murmur32 = getMurmur32(s);
                    externalVarsReference.TryGetValue(murmur32, out string val);
                    if (val == null)
                    {
                        externalVarsReference.Add(murmur32, s);
                    }
                }
            }


            foreach (KeyValuePair<uint, string> entry in externalVarsReference)
            {
                Debug.WriteLine($"externalVarsReference.Add(0x{entry.Key:x08}, \"{entry.Value}\");");
            }


        }


        static uint getMurmur32(String attributeName)
        {
            uint MURMUR2SEED = 0x31415926; // pi
            return MurmurHash2.Hash(attributeName.ToLower(), MURMUR2SEED); ;
        }


        static Package getVpkPackage(string vpk_archive)
        {
            Package package = new SteamDatabase.ValvePak.Package();
            package.Read(vpk_archive);
            return package;
        }


        static List<PackageEntry> getVmatcPackageEntries(Package package)
        {
            Dictionary<string, List<PackageEntry>> entries = package.Entries;
            entries.TryGetValue("vmat_c", out List<PackageEntry> vmat_entries);
            return vmat_entries;
        }

    }

}



