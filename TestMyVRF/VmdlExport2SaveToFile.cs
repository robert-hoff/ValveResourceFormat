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


        static void Main() {

            // tryStuff3();
            // tryStuff2();
            // tryStuff();



            // string input_string = example3();
            // string input_string = example4();
            // string input_string = example5();
            // string input_string = example6();
            // string input_string = example7();
            // string input_string = example8();
            // string input_string = example9();
            string input_string = example10();


            byte[] databytes = parseString(input_string);
            ParseDynamicExpressions result = new ParseDynamicExpressions(databytes);

             Debug.WriteLine(result.dynamicExpressionResult);


        }




        // from post
        // https://github.com/SteamDatabase/ValveResourceFormat/issues/337
        static string example10() {
            return
            "07 CD CC 4C 3F 07 00 00 80 3F 06 20 00 08 00 07 00 00 80 3F 07 00 00 00 40 06 20 00 08 01 07 00 " +
            "00 00 00 07 00 00 80 3F 06 20 00 08 02 09 02 07 CD CC CC 3D 0F 04 3A 00 3F 00 09 00 02 41 00 09 " +
            "01 08 03 07 00 00 80 3F 09 03 15 07 00 00 80 3F 09 03 15 07 00 00 80 3F 09 03 15 06 19 00 08 04 " +
            "09 04 07 00 00 80 3F 15 00";
        }


        //
        //  a = 10+10;
        //  v1||v2 ? sin(1) : 7
        //
        //
        static string example9() {
            return "07 00 00 20 41 07 00 00 20 41 13 08 00 19 38 AE 48 52 04 17 00 1F 00 07 00 00 80 3F 02 24 00 19 "+
                "31 FB FD 02 04 29 00 34 00 07 00 00 80 3F 06 00 00 02 39 00 07 00 00 E0 40 00";
        }





        /*
         * v1 && v2 ? frac(10) : 100*100
         *
         * the expression is formed like this
         * EXT ? 0 : EXT1 ? frac(10) : 100*100
         *
         * At the branch OP check for the very specific byte pattern,
         *
         *
         *
         *
         */
        static string example8() {
            //       0  1  2  3  4  5  6  7  8  9  A  B  C  D  E  F 10 11 12 13 14 15 16 17 18 19 1A 1B 1C 1D 1E 1F ..
            return "19 38 AE 48 52 04 12 00 0A 00 07 00 00 00 00 02 17 00 19 31 FB FD 02 04 1C 00 27 00 07 00 00 20 "+
                "41 06 03 00 02 32 00 07 00 00 C8 42 07 00 00 C8 42 15 00";
        }




        /*
         * true ? (true ? 1 : 2) : 123
         *
         */
        static string example7() {
            return "07 00 00 80 3F 04 0A 00 24 00 07 00 00 80 3F 04 14 00 1C 00 07 00 00 80 3F "+
                "02 21 00 07 00 00 00 40 02 29 00 07 00 00 F6 42 00";
        }


        static string example6() {
            return "07 00 00 80 3F 07 00 00 80 40 13 08 00 07 00 00 20 41 07 00 00 20 42 13 00";
        }


        /*
         *
         *      a = length(1);
         *      a = sqrt(1);
         *      a = TextureSize(1);
         *      frac(a)
         *
         */
        static string example5() {
            return "07 00 00 80 3F 06 22 00 08 00 07 00 00 80 3F 06 11 00 08 00 07 00 00 80 3F 06 24 00 08 00 09 00 06 03 00 00";
        }




        /*
         * true ? 2 : 3
         *
         */
        static string example4() {
            //       0  1  2  3  4  5  6  7  8  9  A  B  C  D  E  F 10 11 12 13 14 15 16 17
            return "07 00 00 80 3F 04 0A 00 12 00 07 00 00 00 40 02 17 00 07 00 00 40 40 00";
        }





        static string example3() {
            // return "19 CC 85 44 96 07 CD CC 4C 3E 15 06 01 00 07 9A 99 99 3E 15 06 02 00 06 00 00 07 00 00 80 3F 13 07 CD CC 4C 3F 15 00";
            // return "19 CC 85 44 96 07 CD CC 8C 3F 15 06 00 00 07 33 33 33 3F 15 06 03 00 18 07 00 00 00 00 06 1A 00 00";
            // return "07 00 00 A0 40 07 00 00 20 41 13 07 00 00 40 40 15 00";
            // return "19 FF 10 86 F3 0C 07 00 00 20 41 0C 13 00";
            // return "19 E1 71 CF 1E 07 00 00 A0 40 17 00";


            return
                "19 38 AE 48 52 19 31 FB FD 02 0F 08 00 "+
                "19 38 AE 48 52 19 31 FB FD 02 11 08 00 "+
                "19 38 AE 48 52 19 31 FB FD 02 0D 08 00 "+
                "19 38 AE 48 52 19 31 FB FD 02 10 08 00 "+
                "19 38 AE 48 52 19 31 FB FD 02 12 08 00 "+
                "19 38 AE 48 52 19 31 FB FD 02 0E 08 00 "+
                "09 00 06 03 00 00";
        }


        static string example2() {
            return
            "07 CD CC 4C 3F 07 00 00 80 3F 06 20 00 08 00 07 00 00 80 3F 07 00 00 00 40 06 20 00 08 01 07 00 "+
            "00 00 00 07 00 00 80 3F 06 20 00 08 02 09 02 07 CD CC CC 3D 0F 04 3A 00 3F 00 09 00 02 41 00 09 "+
            "01 08 03 07 00 00 80 3F 09 03 15 07 00 00 80 3F 09 03 15 07 00 00 80 3F 09 03 15 06 19 00 08 04 "+
            "09 04 07 00 00 80 3F 15 00";
        }


        // rand(1,2)
        static string example1() {
            return "07 00 00 80 3F 07 00 00 00 40 06 20 00 00";
        }





        static void tryStuff3() {

            byte[] data = {0x07, 0xCD, 0xCC, 0x4C, 0x3F, 0x07, 0x00, 0x00, 0x80, 0x3F, 0x06, 0x20, 0x00, 0x08, 0x00, 0x07, 0x00, 0x00,
               0x80, 0x3F, 0x07, 0x00, 0x00, 0x00, 0x40, 0x06, 0x20, 0x00, 0x08, 0x01, 0x07, 0x00, 0x00, 0x00, 0x00, 0x07, 0x00, 0x00,
               0x80, 0x3F, 0x06, 0x20, 0x00, 0x08, 0x02, 0x09, 0x02, 0x07, 0xCD, 0xCC, 0xCC, 0x3D, 0x0F, 0x04, 0x3A, 0x00, 0x3F, 0x00,
               0x09, 0x00, 0x02, 0x41, 0x00, 0x09, 0x01, 0x08, 0x03, 0x07, 0x00, 0x00, 0x80, 0x3F, 0x09, 0x03, 0x15, 0x07, 0x00, 0x00,
               0x80, 0x3F, 0x09, 0x03, 0x15, 0x07, 0x00, 0x00, 0x80, 0x3F, 0x09, 0x03, 0x15, 0x06, 0x19, 0x00, 0x08, 0x04, 0x09, 0x04,
               0x07, 0x00, 0x00, 0x80, 0x3F, 0x15, 0x00 };


            new ParseDynamicExpressions(data);


        }



        static byte[] parseString(String bytestring) {
            string[] tokens = bytestring.Split(" ");
            byte[] databytes = new byte[tokens.Length];
            for (int i = 0; i < tokens.Length; i++) {
                databytes[i] = Convert.ToByte(tokens[i], 16);
            }
            return databytes;
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
