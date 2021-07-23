using MyValveResourceFormat;
using MyValveResourceFormat.Blocks;
using SteamDatabase.ValvePak;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestMyVRF {
    class SerializeVmatcData {



        public static void RunTrials() {

            trial1();

        }


        const string DOTA2_VPK_ARCHIVE = @"X:\Steam\steamapps\common\dota 2 beta\game\dota\pak01_dir.vpk";


        static void trial1() {

            // so for example this entry has some nice m_dynamicParams entries
            // maps/backgrounds/models/frontpage_nemestice/materials/nem_card_3.vmat_c
            string entry_name = "maps/backgrounds/models/frontpage_nemestice/materials/nem_card_3.vmat_c";


            Package package = GetVpkPackage(DOTA2_VPK_ARCHIVE);
            PackageEntry package_entry = package.FindEntry(entry_name);
            package.ReadEntry(package_entry, out byte[] databytes);

            Resource resource = new MyValveResourceFormat.Resource();
            resource.Read(new MemoryStream(databytes));

            ResourceData datablock = (ResourceData)resource.GetBlockByType(BlockType.DATA);

            Debug.WriteLine(datablock);



        }




        static Package GetVpkPackage(string vpk_archive) {
            Package package = new Package();
            package.Read(vpk_archive);
            return package;
        }





    }
}






