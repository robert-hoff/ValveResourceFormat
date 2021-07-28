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




        const string ANALYSIS_DIR = @"X:\checkouts\ValveResourceFormat\files_under_analysis\compiled-shaders";
        // const string ANALYSIS_DIR = @"X:\dota-2-VRF-exports\dota2-export-shaders-pcgl\shaders-core\vfx";

        // PCGL dirs
        const string ANALYSIS_DIR1 = @"X:\dota-2-VRF-exports\dota2-export-shaders-pcgl\shaders\vfx";
        const string ANALYSIS_DIR2 = @"X:\dota-2-VRF-exports\dota2-export-shaders-pcgl\shaders-core\vfx";


        const string GLOW_OUPUT_PC_30_FEATURES = ANALYSIS_DIR + @"\glow_output_pc_30_features.vcs";
        const string GLOW_OUPUT_PC_40_FEATURES = ANALYSIS_DIR + @"\glow_output_pc_40_features.vcs";
        const string GLOW_OUPUT_PCGL_30_FEATURES = ANALYSIS_DIR + @"\glow_output_pcgl_30_features.vcs";
        const string GLOW_OUPUT_PCGL_30_PS = ANALYSIS_DIR + @"\glow_output_pcgl_30_ps.vcs";
        const string GLOW_OUPUT_PCGL_30_VS = ANALYSIS_DIR + @"\glow_output_pcgl_30_vs.vcs";
        const string GLOW_OUPUT_PCGL_40_PS = ANALYSIS_DIR + @"\glow_output_pcgl_40_ps.vcs";
        const string GLOW_OUPUT_PCGL_40_VS = ANALYSIS_DIR + @"\glow_output_pcgl_40_vs.vcs";
        const string MULTIBLEND_PCGL_30_PS = ANALYSIS_DIR + @"\multiblend_pcgl_30_ps.vcs";
        const string MULTIBLEND_PCGL_30_VS = ANALYSIS_DIR + @"\multiblend_pcgl_30_vs.vcs";
        const string REFRACT_PCGL_30_FEATURES = ANALYSIS_DIR + @"\refract_pcgl_30_features.vcs";
        const string UI_TWOTEXTURE_PCGL_30_FEATURES = ANALYSIS_DIR + @"\ui_twotexture_pcgl_30_features.vcs";
        const string UI_TWOTEXTURE_PCGL_30_PS = ANALYSIS_DIR + @"\ui_twotexture_pcgl_30_ps.vcs";
        const string UI_TWOTEXTURE_PCGL_30_VS = ANALYSIS_DIR + @"\ui_twotexture_pcgl_30_vs.vcs";



        const string OUTPUT_DIR = @"Z:\active\projects\dota2-sourcesdk-modding\shader-analysis-vcs-format\output-dump";




        public static void RunTrials() {
            // Trials1();
            // Trials1GetDictionary();
            // Trials2();
            // Trials3();


            // ParseShaderFile(ANALYSIS_DIR+@"\3dskyboxstencil_pcgl_30_features.vcs");
            // ParseShaderFile(ANALYSIS_DIR+@"\3dskyboxstencil_pcgl_30_ps.vcs");
            // ParseShaderFile(ANALYSIS_DIR+@"\3dskyboxstencil_pcgl_30_vs.vcs");


            // ParseShaderFile(ANALYSIS_DIR+@"\3dskyboxstencil_pcgl_30_features.vcs");
            // ParseShaderFile(ANALYSIS_DIR+@"\bloom_dota_cs_pcgl_30_features.vcs");
            // ParseShaderFile(ANALYSIS_DIR+@"\bloom_dota_pcgl_30_features.vcs");
            // ParseShaderFile(ANALYSIS_DIR+@"\crystal_pcgl_30_features.vcs");



            // ParseShaderFile(ANALYSIS_DIR+@"\blur_cs_pcgl_30_features.vcs");
            // ParseShaderFile(ANALYSIS_DIR+@"\deferred_post_process_experimental_pcgl_30_features.vcs");
            // ParseShaderFile(ANALYSIS_DIR+@"\spritecard_pcgl_30_features.vcs");
            // ParseShaderFile(ANALYSIS_DIR+@"\visualize_physics_pcgl_40_gs.vcs");
            // ParseShaderFile(ANALYSIS_DIR+@"\spritecard_pcgl_40_psrs.vcs");
            // ParseShaderFile(ANALYSIS_DIR+@"\sky_pcgl_30_features.vcs");
            // ParseShaderFile(ANALYSIS_DIR+@"\tools_sprite_pcgl_40_features.vcs");
            // ParseShaderFile(ANALYSIS_DIR+@"\spritecard_pcgl_30_features.vcs");
            // ParseShaderFile(ANALYSIS_DIR+@"\vr_warp_pcgl_50_ps.vcs");
            // ParseShaderFile(ANALYSIS_DIR+@"\tools_visualize_tangent_frame_pcgl_40_vs.vcs");
            // ParseShaderFile(ANALYSIS_DIR2+@"\blur_pcgl_30_features.vcs");



            ParseAllShadersAnalysis(ANALYSIS_DIR1);
            ParseAllShadersAnalysis(ANALYSIS_DIR2);


            // ParseAllShadersToFile(ANALYSIS_DIR1);
            // ParseAllShadersToFile(ANALYSIS_DIR2);

        }


        static void ParseAllShadersAnalysis(string path) {



            //for (int i = 0; i < 8; i++) {
            //    Debug.WriteLine($"({i*32+216}) 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00");
            //}




            Dictionary<int, int> collectValuesInt = new();
            Dictionary<string, int> collectValuesString = new();

            Debug.WriteLine(path);
            bool[] requestCount = { false };
            bool[] requestShowFile = { false };


            string[] fileListing = Directory.GetFiles(path);
            foreach (string filepath in fileListing) {
                if (Path.GetExtension(filepath) != ".vcs") {
                    continue;
                }

                ShaderFile2 parser = new(filepath);

                parser.datareader.collectValuesInt = collectValuesInt;
                parser.datareader.collectValuesString = collectValuesString;
                parser.datareader.requestCount = requestCount;
                // parser.datareader.requestShowFile = requestShowFile;
                parser.datareader.filepath = filepath;

                parser.SetDisableOutput(true);
                parser.ParseShader();

                //if (requestShowFile[0]) {
                //    Debug.WriteLine(filepath);
                //}



            }

            // why the frick doesn't this work? (oh, it's because the last file doesn't contain a D-block)
            // showIntCount = parser.datareader.requestCount;
            // showStringCount = parser.datareader.requestCount;






            List<int> intvalues = new();
            foreach (int i in collectValuesInt.Keys) {
                intvalues.Add(i);
            }
            intvalues.Sort();
            foreach (int i in intvalues) {
                if (requestCount[0]) {
                    collectValuesInt.TryGetValue(i, out int instanceCount);
                    Debug.WriteLine($"{i,5}        {instanceCount,3}");
                } else {
                    Debug.WriteLine($"{i}");
                }
            }


            List<string> stringvalues = new();
            foreach (string s in collectValuesString.Keys) {
                stringvalues.Add(s);
            }
            stringvalues.Sort();


            // ParseDynamicExpressionShader dynParser = new ParseDynamicExpressionShader();

            foreach (string s in stringvalues) {
                if (requestCount[0]) {

                    // parse dynamic expression
                    // byte[] databytes = parseString(s);
                    // dynParser.ParseExpression(databytes);
                    // Debug.Write("// "+dynParser.dynamicExpressionResult.Trim().Replace("\n", "\n// "));
                    // Debug.WriteLine("");


                    collectValuesString.TryGetValue(s, out int instanceCount);
                    Debug.WriteLine($"{s}        {instanceCount,3}");
                    // Debug.WriteLine("");
                } else {
                    Debug.WriteLine($"{s}");
                }
            }

        }


        static byte[] parseString(String bytestring) {
            // string[] tokens = bytestring.Trim().Split(" ");
            string[] tokens = bytestring.Replace("\n", "").Trim().Split(" ");
            byte[] databytes = new byte[tokens.Length];
            for (int i = 0; i < tokens.Length; i++) {
                databytes[i] = Convert.ToByte(tokens[i], 16);
            }
            return databytes;
        }


        static void ParseAllShadersToFile(string path) {
            string[] fileListing = Directory.GetFiles(path);
            foreach (string filenamepath in fileListing) {
                if (Path.GetExtension(filenamepath) != ".vcs") {
                    continue;
                }
                try {
                    Debug.WriteLine($"attempting to parse {filenamepath}");
                    ParseShaderFile(filenamepath, true);
                } catch (Exception) {
                }
            }
        }


        static void ParseShaderFile(string filepath) {
            ParseShaderFile(filepath, false);
        }


        static void ParseShaderFile(string filepath, bool writeToFile) {
            ShaderFile2 parser = new ShaderFile2(filepath);
            if (writeToFile) {
                parser.ConfigureWriteToFile(OUTPUT_DIR);
            }
            parser.ParseShader();
        }


        static FileStream TouchFile(string filenamepath) {
            string filename = Path.GetFileName(filenamepath);
            filename = filename[0..^4] + "-ANALYSIS.txt";
            string newFilenamepath = @$"{OUTPUT_DIR}\{filename}";

            if (!File.Exists(newFilenamepath)) {
                FileStream filestream = File.Create(newFilenamepath);
                Debug.WriteLine($"CREATED FILE {newFilenamepath}");
                return filestream;
            } else {
                // Debug.WriteLine($"File already exists!!! {newFilepath}");
                return null;


                // FileStream filestream = File.Create(newFilepath);
                // return filestream;
            }
        }


        static void WriteAllBytesToTemplateFile(string filepath) {
            DataReader datareader = new(File.ReadAllBytes(filepath));

            FileStream stream = TouchFile(filepath);
            if (stream == null) {
                return;
            }

            while (datareader.HasData()) {
                string bytesAsString = datareader.ReadBytesAsString(1024, 32);
                stream.Write(Encoding.ASCII.GetBytes(bytesAsString));
            }

            stream.Flush();
            stream.Close();

            Debug.WriteLine("finished writing");
        }




        static void Trials3() {

            // MULTIBLEND!
            // ShaderReader shaderReader = new(MULTIBLEND_PCGL_30_PS);
            // ShaderReader shaderReader = new(GLOW_OUPUT_PCGL_30_PS);
            // ShaderReader shaderReader = new(GLOW_OUPUT_PCGL_30_VS);


        }





        // OLD CRAP - DON'T MIX UP FILES
        // **********************************************************************
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
