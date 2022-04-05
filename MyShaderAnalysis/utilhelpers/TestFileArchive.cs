using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShaderAnalysis.utilhelpers
{
    public class TestFileArchive
    {

        public static void RunTrials()
        {
            Trial1();
        }

        public static void Trial1()
        {
            FileArchive vcsArchive = new FileArchive(ARCHIVE.dota_game_pcgl_v64);
            foreach (var item in vcsArchive.MyIterator())
            {
                Console.WriteLine($"{item}");
            }
        }


    }
}
