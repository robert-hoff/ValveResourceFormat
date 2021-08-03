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

namespace TestVRF {

    class ReadExpressionsFromPackage {


        public static void DecompileDynamicExpressions(string vpk_archive)
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
                IKeyValueCollection[] dynamicParams = kv_collection.GetArray("m_dynamicParams");


                for (int j = 0; j < dynamicParams.Count(); j++)
                {
                    (string, byte[], string) nameAndData = getParamNameAndData(dynamicParams[j]);
                    if (nameAndData.Item1 == null)
                    {
                        // in the Dota 2 or HL Alyx sample sets this doesn't happen
                        Debug.WriteLine("Didn't recognize this object!");
                        continue;
                    }

                    string prop_name = nameAndData.Item1;
                    byte[] databytes = nameAndData.Item2;
                    string object_type = nameAndData.Item3;

                    if (j == 0)
                    {
                        string dirname = packageEntry.DirectoryName;
                        string filename = packageEntry.FileName;
                        string typename = packageEntry.TypeName;
                        Debug.WriteLine($"\ndynamicParams entries in {dirname}/{filename}.{typename} ({object_type})\n");
                    }


                    ParseDynamicExpressions decompiledDynamicExpression = new ParseDynamicExpressions();
                    decompiledDynamicExpression.ParseExpression(databytes);
                    Debug.WriteLine($"    {prop_name}");

                    showDatabytes(databytes);

                    // tab result (for readability)
                    Regex rx = new Regex("^", RegexOptions.Multiline);
                    string res = rx.Replace(decompiledDynamicExpression.dynamicExpressionResult, "    ");
                    Debug.WriteLine(res);


                }
            }


        }


        static void showDatabytes(byte[] databytes)
        {
            Debug.Write("    ");
            int count = 0;
            foreach (byte b in databytes)
            {
                Debug.Write(String.Format("{0:X2} ", b));
                if (++count%32==0)
                {
                    Debug.WriteLine("");
                    Debug.Write("    ");
                }
            }

            Debug.WriteLine("");
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


        static (string, byte[], string) getParamNameAndData(IKeyValueCollection dynamic_param)
        {
            if (dynamic_param.GetType() == typeof(NTROStruct))
            {
                NTROStruct ntro_object = (NTROStruct)dynamic_param;
                // string name = ntro_object.Name;

                NTROValue[] ntro_values = new NTROValue[2];
                ntro_object.Values.CopyTo(ntro_values, 0);

                if (ntro_values[0].Type == DataType.Byte && ntro_values[1].Type == DataType.String)
                {
                    byte[] databytes = (byte[])ntro_values[0].ValueObject;
                    string paramname = (string)ntro_values[1].ValueObject;
                    return (paramname, databytes, "NTROStruct");
                }
            }

            if (dynamic_param.GetType() == typeof(KVObject))
            {
                KVObject kv_object = (KVObject)dynamic_param;
                kv_object.Properties.TryGetValue("m_name", out KVValue kv_name);
                kv_object.Properties.TryGetValue("m_value", out KVValue kv_value);

                if (kv_name != null && kv_name.Type == KVType.STRING && kv_value != null && kv_value.Type == KVType.BINARY_BLOB)
                {
                    string paramname = (string)kv_name.Value;
                    byte[] databytes = (byte[])kv_value.Value;
                    return (paramname, databytes, "KVObject");
                }
            }

            return (null, null, null);
        }


    }

}




