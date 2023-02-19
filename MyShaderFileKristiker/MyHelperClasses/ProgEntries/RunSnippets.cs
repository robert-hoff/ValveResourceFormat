using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShaderFileKristiker.MyHelperClasses.ProgEntries
{
    internal class RunSnippets
    {
        public static void RunTrials()
        {
            // DeleteDirectory("Z:\\dev\\www\\vcs.codecreation.dev\\dota_game_pcgl_v64\\crystal_pcgl_40");
            DeleteDirectoryContent("Z:\\dev\\www\\vcs.codecreation.dev\\dota_game_pcgl_v64\\crystal_pcgl_40");
        }

        public static void DeleteDirectory(string targetDir, bool recursive = true)
        {
            if (Directory.Exists(targetDir))
            {
                Directory.Delete(targetDir, recursive);
            }
        }

        public static void DeleteDirectoryContent(string targetDir)
        {
            if (Directory.Exists(targetDir))
            {
                DirectoryInfo di = new DirectoryInfo(targetDir);
                foreach (FileInfo file in di.GetFiles())
                {
                    file.Delete();
                }
                foreach (DirectoryInfo dir in di.GetDirectories())
                {
                    dir.Delete(true);
                }
            }
        }
    }
}

