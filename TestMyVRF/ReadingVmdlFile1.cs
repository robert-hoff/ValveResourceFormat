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
    class ReadingVmdlFile1 {

        static void Main() {

            // printFloatNumber();
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



        public static void printFloatNumber() {
            Debug.WriteLine("0x{0:x8}", 123123123);
        }


        public static void tryStuff() {
            Guid KV3_ENCODING_BINARY_BLOCK_COMPRESSED = new Guid(
                new byte[] { 0x46, 0x1A, 0x79, 0x95, 0xBC, 0x95, 0x6C, 0x4F, 0xA7, 0x0B, 0x05, 0xBC, 0xA1, 0xB7, 0xDF, 0xD2 });


            var i = 10 / 9.0;

            Debug.WriteLine($"hello {i}", i);

        }


    }
}
