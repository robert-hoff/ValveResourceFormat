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
    class VmdlExport1ReadBlock {

        static void Mainz() {

            // tryStuff();
            trial4ReadDataBlock();

        }


        public static void trial4ReadDataBlock() {
            Package package = new SteamDatabase.ValvePak.Package();
            package.Read(@"X:\Steam\steamapps\common\Half-Life Alyx\game\hlvr\pak01_dir.vpk");
            PackageEntry model_entry = package.FindEntry("models/creatures/zombie_classic/zombie_classic.vmdl_c");
            package.ReadEntry(model_entry, out byte[] output); // this obtains the vmdl_c file as byte[] output - which is 44968687 bytes long

            // Resource resource = new MyValveResourceFormat.Resource();

            // when running the GUI resource is instantiated like this (but it doesn't seem to make any difference)
            // so it's unclear why this is done ..
            Resource resource = new MyValveResourceFormat.Resource {
                FileName = "zombie_classic.vmdl_c",
            };


            resource.Read(new MemoryStream(output));
            ResourceData datablock = (ResourceData)resource.GetBlockByType(BlockType.DATA);

        }



        public static void tryStuff() {

            // {95791a46-95bc-4f6c-a70b-05bca1b7dfd2}
            Guid KV3_ENCODING_BINARY_BLOCK_COMPRESSED = new Guid(
                new byte[] { 0x46, 0x1A, 0x79, 0x95, 0xBC, 0x95, 0x6C, 0x4F, 0xA7, 0x0B, 0x05, 0xBC, 0xA1, 0xB7, 0xDF, 0xD2 });
            // {1b860500-f7d8-40c1-ad82-75a48267e714}
            Guid KV3_ENCODING_BINARY_UNCOMPRESSED = new Guid(
                new byte[] { 0x00, 0x05, 0x86, 0x1B, 0xD8, 0xF7, 0xC1, 0x40, 0xAD, 0x82, 0x75, 0xA4, 0x82, 0x67, 0xE7, 0x14 });
            // {6847348a-63a1-4f5c-a197-53806fd9b119}
            Guid KV3_ENCODING_BINARY_BLOCK_LZ4 = new Guid(
                new byte[] { 0x8A, 0x34, 0x47, 0x68, 0xA1, 0x63, 0x5C, 0x4F, 0xA1, 0x97, 0x53, 0x80, 0x6F, 0xD9, 0xB1, 0x19 });


            Debug.WriteLine(KV3_ENCODING_BINARY_BLOCK_COMPRESSED);
            Debug.WriteLine(KV3_ENCODING_BINARY_UNCOMPRESSED);
            Debug.WriteLine(KV3_ENCODING_BINARY_BLOCK_LZ4);



        }


    }
}
