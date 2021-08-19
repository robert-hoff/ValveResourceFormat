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


namespace MyShaderAnalysis.utilhelpers {
    public class TestFileSystem {



        public static void RunTrials() {
            Trial1();


        }


        static void Trial1() {
            FileTokens spritecard = new(ARCHIVE.dotacore_pcgl, "spritecard_pcgl_30_ps.vcs");


            // Debug.WriteLine($"{spritecard.GetZFrameServerDir()}");



            spritecard.GetZFrameListing();
        }


    }
}






