using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RunVrf.UtilHelpers.LSystemsMG.Util.External;
using ValveResourceFormat.CompiledShader;

namespace RunVrf
{
    class ParseFiles
    {

        public static void RunTrials()
        {
            // Trials1();
            PrintZframeToHtml();
            // PrintZframeByteVersion();
        }



        public static void Trials1()
        {
            ZFrameFile zFrameFile = GetZFrameFile();
            //Debug.WriteLine($"{zFrameFile.ZframeParams[0].HasStaticVal}");
            //Debug.WriteLine($"{zFrameFile.ZframeParams[0].StaticValBool}");
            //Debug.WriteLine($"{zFrameFile.ZframeParams[0].StaticValFloat}");
            //Debug.WriteLine($"{zFrameFile.ZframeParams[0].StaticValInt}");
            //Debug.WriteLine($"{zFrameFile.ZframeParams[0].HeaderOperator}");


            //foreach (var zFrameParams in zFrameFile.ZframeParams)
            //{
            //    Debug.WriteLine($"{zFrameParams}");
            //    Debug.WriteLine($"{zFrameParams.LinkedParameterIndex}");
            //}


        }


        public static void PrintZframeToHtml()
        {
            string htmlTitle = $"Z[0x{zFrameIndex:x}]";
            string htmlHeader = $"{Path.GetFileName(inputFile)}  Z[0x{zFrameIndex:x}]";
            FileWriter fw = new FileWriter(OUTPUT_FILENAME, true, true, htmlTitle, htmlHeader);
            ZFrameFile zFrameFile = GetZFrameFile();
            _ = new PrintZFrameSummary(shaderFile, zFrameFile, (x) => { fw.Write(x); }, showRichTextBoxLinks: true);
            fw.CloseStreamWriter();
        }

        public static void PrintZframeByteVersion()
        {
            string htmlTitle = $"Z[0x{zFrameIndex:x}]";
            string htmlHeader = $"{Path.GetFileName(inputFile)}  Z[0x{zFrameIndex:x}]";
            FileWriter fw = new FileWriter(OUTPUT_FILENAME_BYTES, true, true, htmlTitle, htmlHeader);
            ZFrameFile zFrameFile = GetZFrameFile();
            zFrameFile.PrintByteDetail((x) => { fw.Write(x); });
            fw.CloseStreamWriter();

        }


        static string OUTPUT_FILENAME = "output2.html";
        static string OUTPUT_FILENAME_BYTES = "output2-bytes.html";
        static ShaderFile shaderFile;


        static string DOTA_PCGL_V64_GAME = @"X:\dota-2-VRF-exports\dota2-export-shaders-pcgl\shaders\vfx\";
        // static string inputFile = $"{DOTA_PCGL_V64_GAME}cables_pcgl_40_vs.vcs"; static int zFrameIndex = 0;
        // static string inputFile = $"{DOTA_PCGL_V64_GAME}hero_pcgl_40_vs.vcs"; static int zFrameIndex = 0x17fa2;
        // static string inputFile = $"{DOTA_PCGL_V64_GAME}hero_pcgl_40_ps.vcs"; static int zFrameIndex = 0x18cbfb1;
        static string inputFile = $"{DOTA_PCGL_V64_GAME}spring_meteor_pcgl_30_vs.vcs"; static int zFrameIndex = 0;

        public static ZFrameFile GetZFrameFile()
        {
            shaderFile = new ShaderFile();
            shaderFile.Read(inputFile);
            return shaderFile.GetZFrameFile(zFrameIndex);
        }








    }
}


