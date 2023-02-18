using SteamDatabase.ValvePak;
using System.Diagnostics;

namespace RunVrf
{
    internal class ReadShaderFilesFromVpk
    {


        public static void RunTrials()
        {
            Go1();
        }

        public static void Go1()
        {
            Debug.WriteLine($"hello");
        }


        static Package GetVpkPackage(string vpkFilenamepath)
        {
            Package package = new SteamDatabase.ValvePak.Package();
            package.Read(vpkFilenamepath);
            return package;
        }

        static List<PackageEntry> GetVmatcPackageEntries(Package package)
        {
            Dictionary<string, List<PackageEntry>> entries = package.Entries;
            entries.TryGetValue("vmat_c", out List<PackageEntry> vmat_entries);
            return vmat_entries;
        }
    }
}
