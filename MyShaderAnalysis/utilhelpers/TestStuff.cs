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



        static OutputWriter output = new();

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


        static void TestFileSystem()
        {
            FileTokens spritecard = new(ARCHIVE.dotacore_pcgl, "spritecard_pcgl_30_ps.vcs");
            spritecard.GetZFrameListing();
        }


    }
}






