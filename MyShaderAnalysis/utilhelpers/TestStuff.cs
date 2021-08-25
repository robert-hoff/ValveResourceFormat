using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using MyShaderAnalysis.vcsparsing;
using System;
using MyShaderAnalysis.compat;
using static MyShaderAnalysis.vcsparsing.ShaderUtilHelpers;
// using static MyShaderAnalysis.vcsparsing.ZFrameFile;
using static MyShaderAnalysis.utilhelpers.FileSystem;


namespace MyShaderAnalysis.utilhelpers
{
    public class TestStuff
    {



        public static void RunTrials()
        {
            // TestFileSystem();
            // TestWriteSystem();
        }



        static OutputWriter output = new(WriteToConsole: false, WriteToDebug: true);

        static void TestWriteSystem(string outputFilenamepath = null, bool writeFile = false)
        {
            if (outputFilenamepath != null && writeFile)
            {
                output.SetOutputFile(outputFilenamepath);
                output.WriteAsHtml("title", "header");
            }

            output.DefineHeaders(new string[] { "index", "name", "arg2", "arg3" });
            output.AddTabulatedRow(new string[] { $"sdf", "1", "2" });
            output.printTabulatedValues();
        }


        static void TestFileAndPathNamesNewArchive()
        {
            // string filenamepath = $"{DOTA_CORE_PCGL_SOURCE}/generic_light_pcgl_30_features.vcs";
            string filenamepath = $"{DOTA_GAME_PC_SOURCE}/hero_pc_30_vs.vcs";
            FileTokens fileTokens = new FileTokens(filenamepath);

            Debug.WriteLine($"{fileTokens.GetBestPath().Length == 0}"); // empty string if no path found
            Debug.WriteLine($"{fileTokens.GetServerFilePath()}"); // /dota-game/pc/hero_pc_30
            Debug.WriteLine($"{fileTokens.GetServerFileDir()}"); // Z:/dev/www/vcs.codecreation.dev/dota-game/pc/hero_pc_30
            Debug.WriteLine($"{fileTokens.GetBestZframesLink(0)}"); //   Z[00000000] (plaintext if no zframe found)

        }



        static void TestFileSystem()
        {
            FileTokens spritecard = new(ARCHIVE.dotacore_pcgl, "spritecard_pcgl_30_ps.vcs");
            spritecard.GetZFrameListing();
        }


    }
}






