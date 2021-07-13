using SteamDatabase.ValvePak;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestVRF {
	class ReadingVmdlFile1 {

		static void Mainz() {


			trial1();


		}


		static string HLALYX_TEST_VPK = @"Z:\git\vcs-decompile\vrf-decompiler-bat\gen_vpk\hl_dir.vpk";
		static string HLALYX_MAIN_VPK = @"X:\Steam\steamapps\common\Half-Life Alyx\game\hlvr\pak01_dir.vpk";
		static string ZOMBIE_CLASSIC = "models/creatures/zombie_classic/zombie_classic.vmdl_c";
		static string EXPORT_DIR = @"C:\Users\R\Desktop\temttemp";


		public static void trial1() {
			string vpkfile = HLALYX_TEST_VPK;
			var package = new SteamDatabase.ValvePak.Package();
			package.Read(vpkfile);

			string vpk_entry = ZOMBIE_CLASSIC;
			string filename = vpk_entry.Substring(vpk_entry.LastIndexOf("/") + 1);
			PackageEntry model_entry = package.FindEntry(vpk_entry);

			package.ReadEntry(model_entry, out byte[] output);

			// ouput.Length here is
			// 44968687
			// which matches the size of
			// models/creatures/zombie_classic/zombie_classic.vmdl_c


			// Now I want to replicate the processes that happen here
			var resource = new ValveResourceFormat.Resource();

			// this read method takes 2500 ms
			resource.Read(new MemoryStream(output));





		}



		public static void trial9() {
			var resource = new ValveResourceFormat.Resource();
			resource.Read(new MemoryStream(null));
		}


	}
}
