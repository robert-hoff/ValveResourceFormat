using System;
using System.Diagnostics;
using System.IO;
using SteamDatabase.ValvePak;
using MyValveResourceFormat;
using MyValveResourceFormat.Blocks;
using MyValveResourceFormat.ResourceTypes;
using MyValveResourceFormat.ThirdParty;

namespace TestMyVRF {


    class ProgramEntry {


        static void Main() {


            // SerializeVmatcData.RunTrials();
            // ScanArchiveForEntries.RunTrials();


            // DoNotReflect
            uint murmur32 = MurmurHash2.Hash("RepresentativeTexture".ToLower(), 0x31415926);
            Debug.WriteLine("{0:x}",murmur32);


        }




    }




}







