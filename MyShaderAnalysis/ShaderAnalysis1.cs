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

        // selected files
        const string ANALYSIS_DIR = @"X:\checkouts\ValveResourceFormat\files_under_analysis\compiled-shaders";

        // PCGL dirs
        const string ANALYSIS_DIR_CORE = @"X:\dota-2-VRF-exports\dota2-export-shaders-pcgl\shaders-core\vfx";
        const string ANALYSIS_DIR_NOT_CORE = @"X:\dota-2-VRF-exports\dota2-export-shaders-pcgl\shaders\vfx";
        const string OUTPUT_DIR = @"Z:\active\projects\dota2-sourcesdk-modding\shader-analysis-vcs-format\output-dump";

        const string EXPORT_DIR_PC_CORE = @"X:\dota-2-VRF-exports\dota2-export-shaders-pc\shaders-core\vfx";
        const string EXPORT_DIR_PC_NOT_CORE = @"X:\dota-2-VRF-exports\dota2-export-shaders-pc\shaders\vfx";




        public static void RunTrials() {
            // Trials1();
            // Trials1GetDictionary();
            // Trials2();
            // Trials3();


            // GS file
            // ParseShaderFile(ANALYSIS_DIR_CORE+@"\visualize_physics_pcgl_30_gs.vcs");
            // PSRS file
            // ParseShaderFile(ANALYSIS_DIR_NOT_CORE+@"\spritecard_pcgl_40_psrs.vcs");

            // NOT PCGL
            // ParseShaderFile(EXPORT_DIR_PC_NOT_CORE+@"\3dskyboxstencil_pc_30_features.vcs");


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
            // ParseShaderFile(ANALYSIS_DIR+@"\sky_pcgl_30_features.vcs");
            // ParseShaderFile(ANALYSIS_DIR+@"\tools_sprite_pcgl_40_features.vcs");
            // ParseShaderFile(ANALYSIS_DIR+@"\spritecard_pcgl_30_features.vcs");
            // ParseShaderFile(ANALYSIS_DIR+@"\vr_warp_pcgl_50_ps.vcs");
            // ParseShaderFile(ANALYSIS_DIR+@"\tools_visualize_tangent_frame_pcgl_40_vs.vcs");


            // ParseShaderFile(ANALYSIS_DIR_CORE+@"\blur_pcgl_30_features.vcs");
            // ParseShaderFile(ANALYSIS_DIR_NOT_CORE+@"\multiblend_pcgl_30_features.vcs");

            ParseShaderFile(ANALYSIS_DIR_CORE+@"\visualize_cloth_pcgl_40_features.vcs");
            

            // ParseShaderFile(ANALYSIS_DIR_CORE+@"\generic_light_pcgl_30_features.vcs");
            // ParseShaderFile(ANALYSIS_DIR_CORE+@"\generic_light_pcgl_30_vs.vcs");
            // ParseShaderFile(ANALYSIS_DIR_CORE+@"\generic_light_pcgl_30_ps.vcs");


            // -- SCAN ALL FILES, COLLECTING VALUES
            // ParseAllShadersAnalysis(ANALYSIS_DIR_NOT_CORE, false);
            // ParseAllShadersAnalysis(ANALYSIS_DIR_CORE, true);


            // NOTE - some files share the same name, doing the main dir (ANALYSIS_DIR_NOT_CORE) last
            // will guarantee all those files are the ones I expect

            // --- WRITE ALL SHADER FILES TO DISK
            // ParseAllShadersToFile(ANALYSIS_DIR_CORE);
            // ParseAllShadersToFile(ANALYSIS_DIR_NOT_CORE);


            // WriteAllBytesToTemplateFile(@"Z:\git\vcs-decompile\output2\zframe001.txt");

        }








        private static Dictionary<int, int> collectValuesInt = new();
        private static Dictionary<string, int> collectValuesString = new();



        static void ParseAllShadersAnalysis(string path, bool printData) {



            //for (int i = 0; i < 8; i++) {
            //    Debug.WriteLine($"({i*32+216}) 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00");
            //}



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



            if (!printData) {
                return;
            }



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
                    // NOTE requestCount[0] must be enabled to do this here!!!
                    //byte[] databytes = parseString(s);
                    //dynParser.ParseExpression(databytes);
                    //Debug.Write("// " + dynParser.dynamicExpressionResult.Trim().Replace("\n", "\n// "));
                    //Debug.WriteLine("");


                    collectValuesString.TryGetValue(s, out int instanceCount);
                    Debug.WriteLine($"{s}        {instanceCount,3}");
                    // Debug.WriteLine("");
                } else {
                    Debug.WriteLine($"{s}");
                }
            }

            //Debug.WriteLine("");
            //Debug.WriteLine("");
            //Debug.WriteLine("");
            //Debug.WriteLine("");
            //Debug.WriteLine("");
            //Debug.WriteLine("");


            if (printData) {
                collectValuesInt = new();
                collectValuesString = new();
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


            Debug.WriteLine(newFilenamepath);

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
            DataReader datareader = new(GetFile(ANALYSIS_DIR+@"\glow_output_pcgl_30_Vs.vcs"));
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

            DataReader datareader = new(GetFile(ANALYSIS_DIR+@"\glow_output_pcgl_30_ps.vcs"));

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
