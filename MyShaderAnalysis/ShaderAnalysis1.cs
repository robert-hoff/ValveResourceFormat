using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyShaderAnalysis.readers;


namespace MyShaderAnalysis {
    public class ShaderAnalysis1 {

        const string ANALYSIS_DIR = @"X:\checkouts\ValveResourceFormat\files_under_analysis";

        const string GLOW_OUPUT_PC_30_FEATURES = ANALYSIS_DIR + @"\compiled-shaders\glow_output_pc_30_features.vcs";
        const string GLOW_OUPUT_PC_40_FEATURES = ANALYSIS_DIR + @"\compiled-shaders\glow_output_pc_40_features.vcs";


        const string GLOW_OUPUT_PCGL_30_FEATURES = ANALYSIS_DIR + @"\compiled-shaders\glow_output_pcgl_30_features.vcs";
        const string GLOW_OUPUT_PCGL_30_PS = ANALYSIS_DIR + @"\compiled-shaders\glow_output_pcgl_30_ps.vcs";
        const string GLOW_OUPUT_PCGL_30_VS = ANALYSIS_DIR + @"\compiled-shaders\glow_output_pcgl_30_vs.vcs";

        const string GLOW_OUPUT_PCGL_40_PS = ANALYSIS_DIR + @"\compiled-shaders\glow_output_pcgl_40_ps.vcs";
        const string GLOW_OUPUT_PCGL_40_VS = ANALYSIS_DIR + @"\compiled-shaders\glow_output_pcgl_40_vs.vcs";


        const string MULTIBLEND_PCGL_30_PS = ANALYSIS_DIR + @"\compiled-shaders\multiblend_pcgl_30_ps.vcs";
        const string MULTIBLEND_PCGL_30_VS = ANALYSIS_DIR + @"\compiled-shaders\multiblend_pcgl_30_vs.vcs";


        public static void RunTrials() {
            // Trials1();
            // Trials1GetDictionary();
            // Trials2();
            // Trials3();
            Trials4();
        }


        static void Trials4() {
            new ShaderFile(GLOW_OUPUT_PCGL_30_FEATURES);

        }



        static void Trials3() {

            // MULTIBLEND!
            // ShaderReader shaderReader = new(MULTIBLEND_PCGL_30_PS);


            ShaderReader shaderReader = new(GLOW_OUPUT_PCGL_30_PS);
            // ShaderReader shaderReader = new(GLOW_OUPUT_PCGL_30_VS);


        }



        static void Trials2() {

            DataReader datareader = new(GetFile(GLOW_OUPUT_PCGL_30_VS));

            List<int> zframeIndexes = datareader.SearchForByteSequence(new byte[] { 0x28, 0xb5, 0x2f, 0xfd });

            datareader.ShowBytesSurrounding(zframeIndexes[0]);
            datareader.ShowBytesSurrounding(10);
            datareader.ShowBytesSurrounding(910);



        }




        static void Trials1GetDictionary() {
            DataReader datareader = new(GetFile(ANALYSIS_DIR + @"\zstdictionary_2bc2fa87.dat"));
            datareader.ShowBytesSurrounding();

        }



        static void Trials1() {



            DataReader datareader = new(GetFile(GLOW_OUPUT_PCGL_30_PS));

            uint magic = datareader.ReadUInt(); // the magic 0x32736376 indicates "vcs2"
            // Debug.WriteLine($"0x{magic:x08}");

            uint version = datareader.ReadUInt(); // always 64

            uint wtf = datareader.ReadUInt(); // unknown significance, CS source saying either 0 or 1


            byte[] fileIdentifier = datareader.ReadBytes(16);
            byte[] staticIdentifier = datareader.ReadBytes(16);

            // just prints 95-AD-C6-53-3C-4B-76-71-43-CF-03-07-96-00-DE-0B
            // note the identifiers are not the same between the pc and pcgl files
            Debug.WriteLine($"File identifier: {BitConverter.ToString(fileIdentifier)}");
            Debug.WriteLine($"Static identifier: {BitConverter.ToString(staticIdentifier)}");



            uint unk0_b = datareader.ReadUInt();
            Debug.WriteLine($"unk0_b {unk0_b}"); // Always 14? (no - mine is 17)

            // Chunk 1
            uint count = datareader.ReadUInt();

            Debug.WriteLine(datareader.offset);

            // ok I think I'm mostly on my own going forward, the CS guys think there's some chunk business going on
            // before the compressed parts start



        }





        static byte[] GetFile(string filepath) {
            return File.ReadAllBytes(filepath);
        }





    }
}
