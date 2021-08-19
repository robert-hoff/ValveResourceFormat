using System.IO;
using System.Diagnostics;
using MyShaderAnalysis.vcsparsing;
using System.Collections.Generic;
using static MyShaderAnalysis.vcsparsing.UtilHelpers;


namespace MyShaderAnalysis {


    public class ShaderAnalysis {
        const string PCGL_DIR_CORE = @"X:\dota-2-VRF-exports\dota2-export-shaders-pcgl\shaders-core\vfx";
        const string PCGL_DIR_NOT_CORE = @"X:\dota-2-VRF-exports\dota2-export-shaders-pcgl\shaders\vfx"; // i.e. Dota2 specific
        const string PC_DIR_CORE = @"X:\dota-2-VRF-exports\dota2-export-shaders-pc\shaders-core\vfx";
        const string PC_DIR_NOT_CORE = @"X:\dota-2-VRF-exports\dota2-export-shaders-pc\shaders\vfx";
        const string ARTIFACT_CLASSIC_CORE_PC_SOURCE = "X:/artifact-classic-exports/artifact-shaders-pc-core";
        const string ARTIFACT_CLASSIC_DCG_PC_SOURCE = "X:/artifact-classic-exports/artifact-shaders-pc-dcg";

        // const string OUTPUT_DIR = @"..\..\..\GEN-OUTPUT";
        const string OUTPUT_DIR = @"Z:\active\projects\dota2-sourcesdk-modding\shader-analysis-vcs-format\OUTPUT_DUMP";




        public static void RunTrials() {
            // string filenamepath = PCGL_DIR_NOT_CORE + @"\multiblend_pcgl_30_vs.vcs";
            // string filenamepath = PCGL_DIR_NOT_CORE + @"\multiblend_pcgl_30_ps.vcs";
            // string filenamepath = PCGL_DIR_NOT_CORE + @"\hero_pcgl_30_features.vcs";
            // string filenamepath = PCGL_DIR_NOT_CORE + @"\hero_pcgl_30_vs.vcs";
            // string filenamepath = PCGL_DIR_NOT_CORE + @"\hero_pcgl_30_ps.vcs";
            // string filenamepath = PCGL_DIR_CORE + @"\generic_light_pcgl_30_features.vcs";
            // string filenamepath = PCGL_DIR_CORE + @"\generic_light_pcgl_30_vs.vcs";
            // string filenamepath = PCGL_DIR_CORE + @"\generic_light_pcgl_30_ps.vcs";
            // string filenamepath = PCGL_DIR_CORE + @"\apply_fog_pcgl_40_ps.vcs";
            // string filenamepath = PCGL_DIR_CORE + @"\tools_wireframe_pcgl_40_gs.vcs";
            // string filenamepath = PCGL_DIR_NOT_CORE + @"\spring_meteor_pcgl_30_vs.vcs";
            // string filenamepath = PCGL_DIR_NOT_CORE + @"\cables_pcgl_30_features.vcs";
            // string filenamepath = PCGL_DIR_NOT_CORE + @"\cables_pcgl_30_vs.vcs";
            // string filenamepath = PCGL_DIR_NOT_CORE + @"\spritecard_pcgl_30_features.vcs";
            // string filenamepath = PCGL_DIR_NOT_CORE + @"\blur_pcgl_30_features.vcs";

            // string filenamepath = PCGL_DIR_CORE + @"\physics_wireframe_pcgl_30_ps.vcs"; // frame 16
            // string filenamepath = PCGL_DIR_CORE + @"\tools_solid_pcgl_30_ps.vcs"; // frame 7
            // string filenamepath = PCGL_DIR_CORE + @"\visualize_cloth_pcgl_40_ps.vcs"; // frame 5 has an empty glsl file reference
            // string filenamepath = PCGL_DIR_CORE + @"\visualize_nav_pcgl_40_ps.vcs"; // frame 10
            // string filenamepath = PCGL_DIR_CORE + @"\tools_sprite_pcgl_40_gs.vcs"; // gs file
            // string filenamepath = PCGL_DIR_NOT_CORE + @"\hero_pcgl_40_psrs.vcs"; // psrs file
            // string filenamepath = PCGL_DIR_CORE + @"\deferred_shading_pcgl_41_ps.vcs"; // interesting zframe content
            // string filenamepath = PCGL_DIR_NOT_CORE + @"\grasstile_pcgl_30_vs.vcs";
            // string filenamepath = PCGL_DIR_NOT_CORE + @"\3dskyboxstencil_pcgl_30_features.vcs";
            // string filenamepath = PCGL_DIR_CORE + @"\tools_wireframe_pcgl_40_gs.vcs"; // this file has some very short zframes
            // string filenamepath = PCGL_DIR_NOT_CORE + @"\multiblend_pcgl_30_ps.vcs";
            // string filenamepath = PCGL_DIR_NOT_CORE + @"\blur_pcgl_30_ps.vcs";
            // string filenamepath = PCGL_DIR_CORE + @"\apply_fog_pcgl_40_ps.vcs";
            // string filenamepath = PCGL_DIR_CORE + @"\spritecard_pcgl_30_ps.vcs";
            string filenamepath = PCGL_DIR_CORE + @"\panorama_pcgl_50_ps.vcs";



            // WARN - even 3 every 100 is a lot of files
            // Write3ZFramesEveryHundedAfter300();
            // WriteFirst50ZFramesEveryFile(OUTPUT_DIR, writeAsHtml: true, saveGlsl: false);
            // WriteFirst50ZFramesEveryFile(useServerDefaultDir: true, writeAsHtml: true);

            // WriteZFrameToFile(filenamepath, 0);
            // WriteAZFrameAsHtmlByZframeId(filenamepath, 0x0);
            // ParseABunchOfZframes();
            // WriteFirstZFrameEveryFile();



            // R: currently writes the blur_pcgl_30_features.vcs + vs, ps collection into my OUTPUT dir (not the server)
            // --> There must be an updated method for this somewhere that uses the new FileTokens class
            // NO, remember the byte-analysis goes to the directory "vcs-all"
            // WriteVcsCollectionAsHtml();


            // ShowVcsByteAnalysis(filenamepath);
            // WriteAllVcsFilesToTxt();
            // WriteVcsByteAnalysisToTxt(filenamepath);

            // R: I wrote the core and dota files separately and copied them onto the server
            WriteAllVcsFilesToHtml();
            // WriteVcsByteAnalysisToHtml(filenamepath, writeHtmlLinks: true);
            // ParseAllVcsFilesDisableOutput();




            // TestZFrameSingleFile();

            // -- test or write single files
            // RunWriteZframeAsTxt();
            // ShaderFileTestSinglefiles();
            // VcsParsingTestSinglefiles();

        }




        static void WriteVcsCollectionAsHtml() {
            // string directoryToUse = PCGL_DIR_NOT_CORE;
            // string[] filenames = { "crystal_pcgl_30_features.vcs", "crystal_pcgl_30_vs.vcs", "crystal_pcgl_30_ps.vcs" };

            // (string, string, string) triple = GetTriple(PCGL_DIR_CORE+@"\visualize_cloth_pcgl_40_features.vcs");


            (string, string, string) triple = GetTriple(PCGL_DIR_CORE + @"\blur_pcgl_30_features.vcs");



            string[] filenames = { triple.Item1, triple.Item2, triple.Item3 };
            // foreach (var filename in filenames) {
            foreach (var filenamepath in filenames) {
                // string filenamepath = @$"{directoryToUse}\{filename}";
                WriteVcsByteAnalysisToHtml(filenamepath, writeHtmlLinks: true);
                ShaderFile shaderFile = new(filenamepath);
                int zFrameCount = shaderFile.GetZFrameCount();
                for (int i = 0; i < zFrameCount; i++) {
                    WriteZframeAsHtml(shaderFile, i);
                }
            }
        }



        static void Write3ZFramesEveryHundedAfter300() {
            // string filenamespecific = PCGL_DIR_NOT_CORE + @"\multiblend_pcgl_30_ps.vcs";
            // string filenamespecific1 = PCGL_DIR_NOT_CORE + @"\hero_pcgl_30_ps.vcs";
            // string filenamespecific2 = PCGL_DIR_NOT_CORE + @"\hero_pcgl_40_ps.vcs";
            // string filenamespecific = PCGL_DIR_CORE + @"\blur_pcgl_30_ps.vcs";
            // List<string> vcsFiles = new();
            // vcsFiles.Add(filenamespecific1);
            // vcsFiles.Add(filenamespecific2);

            List<string> vcsFiles = GetVcsFiles(PCGL_DIR_CORE, PCGL_DIR_NOT_CORE, FILETYPE.any, -1);



            foreach (string filenamepath in vcsFiles) {

                // skip hero files, they are too big
                if (filenamepath.Equals(PCGL_DIR_NOT_CORE + @"\hero_pcgl_30_ps.vcs")) {
                    continue;
                }
                if (filenamepath.Equals(PCGL_DIR_NOT_CORE + @"\hero_pcgl_40_ps.vcs")) {
                    continue;
                }


                string token = GetCoreOrDotaString(filenamepath);
                string outputdir = @$"Z:\dev\www\vcs.codecreation.dev\vcs-all\{token}\zsource";

                ShaderFile shaderFile = new(filenamepath);
                //int zframesToWrite = 100;
                //if (shaderFile.GetZFrameCount() < zframesToWrite) {
                //    zframesToWrite = shaderFile.GetZFrameCount();
                //}
                //for (int i = 0; i < zframesToWrite; i++) {
                //    WriteZframeAsHtml(shaderFile, i, outputdir);
                //}
                int i = 300;
                while (i<shaderFile.GetZFrameCount()) {
                    WriteZframeAsHtml(shaderFile, i, outputdir);
                    if (i + 1 < shaderFile.GetZFrameCount()) {
                        WriteZframeAsHtml(shaderFile, i + 1, outputdir);
                    }
                    if (i + 2 < shaderFile.GetZFrameCount()) {
                        WriteZframeAsHtml(shaderFile, i + 2, outputdir);
                    }
                    i += 100;
                }


            }

        }




        static void WriteFirst50ZFramesEveryFile(string outputdir = null, bool useServerDefaultDir = false, bool writeAsHtml = true, bool saveGlsl = true) {
            if (useServerDefaultDir && outputdir != null) {
                throw new ShaderParserException("be more specific");
            }
            if (!useServerDefaultDir && outputdir == null) {
                throw new ShaderParserException("no output dir specified");
            }

            int ZFRAME_FILES_TO_WRITE = 10;


            // string filenamepath = PCGL_DIR_NOT_CORE + @"\multiblend_pcgl_30_ps.vcs";
            // string filenamepath = PCGL_DIR_CORE + @"\blur_pcgl_30_ps.vcs";
            // List<string> vcsFiles = GetVcsFiles(PCGL_DIR_CORE, PCGL_DIR_NOT_CORE, FILETYPE.any, -1);

            List<string> vcsFiles = new();
            //vcsFiles.AddRange(GetVcsFiles(ARTIFACT_CLASSIC_DCG_PC_SOURCE, null, FILETYPE.features_file, -1));
            //vcsFiles.AddRange(GetVcsFiles(ARTIFACT_CLASSIC_DCG_PC_SOURCE, null, FILETYPE.vs_file, -1));
            //vcsFiles.AddRange(GetVcsFiles(ARTIFACT_CLASSIC_DCG_PC_SOURCE, null, FILETYPE.ps_file, -1));
            //vcsFiles.AddRange(GetVcsFiles(ARTIFACT_CLASSIC_DCG_PC_SOURCE, null, FILETYPE.psrs_file, -1));
            vcsFiles.AddRange(GetVcsFiles(ARTIFACT_CLASSIC_CORE_PC_SOURCE, null, FILETYPE.features_file, -1));
            vcsFiles.AddRange(GetVcsFiles(ARTIFACT_CLASSIC_CORE_PC_SOURCE, null, FILETYPE.vs_file, -1));
            vcsFiles.AddRange(GetVcsFiles(ARTIFACT_CLASSIC_CORE_PC_SOURCE, null, FILETYPE.ps_file, -1));
            vcsFiles.AddRange(GetVcsFiles(ARTIFACT_CLASSIC_CORE_PC_SOURCE, null, FILETYPE.psrs_file, -1));



            foreach (string filenamepath in vcsFiles) {
                string token = GetCoreOrDotaString(filenamepath);
                if (useServerDefaultDir) {
                    outputdir = @$"Z:\dev\www\vcs.codecreation.dev\vcs-all\{token}\zsource";
                }

                ShaderFile shaderFile = new(filenamepath);
                int zframesToWrite = ZFRAME_FILES_TO_WRITE;
                if (shaderFile.GetZFrameCount() < zframesToWrite) {
                    zframesToWrite = shaderFile.GetZFrameCount();
                }
                for (int i = 0; i < zframesToWrite; i++) {
                    if (writeAsHtml) {
                        WriteZframeAsHtml(shaderFile, i, outputdir, saveGlsl: saveGlsl);
                    } else {
                        WriteZframeAsTxt(shaderFile, i, outputdir, saveGlsl: saveGlsl);
                    }
                }
            }
        }


        static void WriteAZFrameAsHtmlByZframeId(string filenamepath, long zframeId, string outputdir = null) {
            if (outputdir == null) {
                outputdir = OUTPUT_DIR;
            }
            ShaderFile shaderFile = new(filenamepath);
            byte[] zframeDatabytes = shaderFile.GetDecompressedZFrame(zframeId);
            string outputFilename = GetZframeHtmlFilename((uint) zframeId, shaderFile.filenamepath);
            string outputFilenamepath = @$"{outputdir}\{outputFilename}";
            DataReaderZFrameByteAnalysis zFrameParser = new(zframeDatabytes, shaderFile.vcsFiletype, shaderFile.vcsSourceType);
            Debug.WriteLine($"writing to {outputFilenamepath}");
            StreamWriter sw = new(outputFilenamepath);
            zFrameParser.ConfigureWriteToFile(sw, true);
            zFrameParser.SetWriteAsHtml(true);
            zFrameParser.RequestGlslFileSave(outputdir);
            string htmlHeader = GetHtmlHeader(outputFilename, outputFilename[0..^5]);
            sw.WriteLine($"{htmlHeader}");
            zFrameParser.PrintByteAnalysis();
            sw.WriteLine($"{GetHtmlFooter()}");
            sw.Close();
        }







        static void WriteZframeAsHtml(ShaderFile shaderFile, int zframeIndex, string outputdir = null, bool saveGlsl = true) {
            if (outputdir == null) {
                outputdir = OUTPUT_DIR;
            }

            byte[] zframeDatabytes = GetZFrameByIndex(shaderFile, zframeIndex);
            uint zframeId = (uint)shaderFile.GetZFrameIdByIndex(zframeIndex);
            string outputFilename = GetZframeHtmlFilename(zframeId, shaderFile.filenamepath);
            string outputFilenamepath = @$"{outputdir}\{outputFilename}";
            DataReaderZFrameByteAnalysis zFrameParser = new(zframeDatabytes, shaderFile.vcsFiletype, shaderFile.vcsSourceType);
            Debug.WriteLine($"writing to {outputFilenamepath}");
            StreamWriter sw = new(outputFilenamepath);
            zFrameParser.ConfigureWriteToFile(sw, true);
            zFrameParser.SetWriteAsHtml(true);
            if (saveGlsl) {
                zFrameParser.RequestGlslFileSave(outputdir);
            }
            string htmlHeader = GetHtmlHeader(outputFilename, outputFilename[0..^5]);
            sw.WriteLine($"{htmlHeader}");
            zFrameParser.PrintByteAnalysis();
            sw.WriteLine($"{GetHtmlFooter()}");
            sw.Close();
        }



        static void RunWriteZframeAsTxt() {


            // string filenamepath = PCGL_DIR_CORE + @"\rendermicrobenchmark_pcgl_40_ps.vcs";
            // string filenamepath = PCGL_DIR_CORE + @"\sky_pcgl_40_ps.vcs";
            // ShaderFile shaderFile = new(filenamepath);
            // WriteZframeAsTxt(shaderFile, 2, null, true);

            int tabchar = '\t';

            Debug.WriteLine($"{tabchar:x}");

        }





        static void WriteZframeAsTxt(ShaderFile shaderFile, int zframeIndex, string outputdir = null, bool saveGlsl = true) {
            if (outputdir == null) {
                outputdir = OUTPUT_DIR;
            }

            byte[] zframeDatabytes = GetZFrameByIndex(shaderFile, zframeIndex);
            uint zframeId = (uint)shaderFile.GetZFrameIdByIndex(zframeIndex);
            string outputFilename = GetZframeTxtFilename(zframeId, shaderFile.filenamepath);
            string outputFilenamepath = @$"{outputdir}\{outputFilename}";
            DataReaderZFrameByteAnalysis zFrameParser = new(zframeDatabytes, shaderFile.vcsFiletype, shaderFile.vcsSourceType);
            Debug.WriteLine($"writing to {outputFilenamepath}");
            StreamWriter sw = new(outputFilenamepath);
            zFrameParser.ConfigureWriteToFile(sw, true);
            zFrameParser.SetWriteAsHtml(false);
            if (saveGlsl) {
                zFrameParser.RequestGlslFileSave(outputdir);
            }
            zFrameParser.PrintByteAnalysis();
            sw.Close();
        }





        static void WriteFirstZFrameEveryFile() {
            // List<string> vcsFiles = GetVcsFiles(PCGL_DIR_CORE, PCGL_DIR_NOT_CORE, FILETYPE.any, -1);
            List<string> vcsFiles = GetVcsFiles(PCGL_DIR_CORE, PCGL_DIR_NOT_CORE, FILETYPE.ps_file, 30);
            foreach (string vcsFile in vcsFiles) {
                ShaderFile shaderFile = new(vcsFile);
                if (shaderFile.GetZFrameCount() == 0) {
                    continue;
                }
                byte[] zframeDatabytes = GetZFrameByIndex(shaderFile, 0);
                string outputFilenamepath = @$"{OUTPUT_DIR}\{GetZframeTxtFilename((uint)shaderFile.GetZFrameIdByIndex(0), vcsFile)}";
                StreamWriter sw = new(outputFilenamepath);
                Debug.WriteLine($"parsing {vcsFile}");
                Debug.WriteLine($"writing to {outputFilenamepath}");
                PrintZFrame(zframeDatabytes, shaderFile.vcsFiletype, shaderFile.vcsSourceType, true, sw);
                sw.Close();

                break;
            }
        }

        static void ParseABunchOfZframes() {
            List<string> vcsFiles = GetVcsFiles(PCGL_DIR_CORE, PCGL_DIR_NOT_CORE, FILETYPE.any, -1);
            foreach (string vcsFile in vcsFiles) {
                ShaderFile shaderFile = new(vcsFile);
                int zcount = shaderFile.GetZFrameCount();
                int nrToParse = zcount > 100 ? 100 : zcount;
                if (nrToParse == 0) {
                    Debug.WriteLine($"no zframes to parse for {RemoveBaseDir(shaderFile.filenamepath)}");
                    continue;
                }
                Debug.WriteLine($"parsing {RemoveBaseDir(shaderFile.filenamepath)} frames [{0},{nrToParse})");
                for (int i = 0; i < nrToParse; i++) {
                    byte[] zframeDatabytes = GetZFrameByIndex(shaderFile, i);
                    PrintZFrame(zframeDatabytes, shaderFile.vcsFiletype, shaderFile.vcsSourceType, true);
                }
            }
        }


        //static void WriteZFrameToFileByZframeId(string filenamepath, long zframeId) {
        //    ShaderFile shaderFile = new(filenamepath);
        //    byte[] zframeDatabytes = shaderFile.GetDecompressedZFrame(zframeId);
        //    string outputFilenamepath = @$"{OUTPUT_DIR}\{GetZframeTxtFilename((uint) zframeId, filenamepath)}";
        //    StreamWriter sw = new(outputFilenamepath);
        //    Debug.WriteLine($"parsing {filenamepath}");
        //    Debug.WriteLine($"writing to {outputFilenamepath}");
        //    PrintZFrame(zframeDatabytes, GetVcsFileType(filenamepath), true, sw);
        //    sw.Close();
        //}



        // THIS OUTPUTS FUCKING txt (which can be good!)
        static void WriteZFrameToFile(string filenamepath, int zframeIndex) {
            ShaderFile shaderFile = new(filenamepath);
            byte[] zframeDatabytes = GetZFrameByIndex(shaderFile, zframeIndex);
            string outputFilenamepath = @$"{OUTPUT_DIR}\{GetZframeTxtFilename((uint)shaderFile.GetZFrameIdByIndex(zframeIndex), filenamepath)}";
            StreamWriter sw = new(outputFilenamepath);
            Debug.WriteLine($"parsing {filenamepath}");
            Debug.WriteLine($"writing to {outputFilenamepath}");
            PrintZFrame(zframeDatabytes, shaderFile.vcsFiletype, shaderFile.vcsSourceType, true, sw);
            sw.Close();
        }



        static void TestZFrameSingleFile() {
            // string filenamepath = PCGL_DIR_NOT_CORE + @"\multiblend_pcgl_30_vs.vcs";
            // string filenamepath = PCGL_DIR_NOT_CORE + @"\multiblend_pcgl_30_ps.vcs";
            // string filenamepath = ARTIFACT_CLASSIC_CORE_PC_SOURCE + @"\aerial_perspective_pc_30_ps.vcs";
            // string filenamepath = PCGL_DIR_CORE + @"\convolve_environment_map_pcgl_41_vs.vcs";
            // string filenamepath = PCGL_DIR_CORE + @"\panorama_pcgl_40_ps.vcs";
            string filenamepath = PCGL_DIR_NOT_CORE + @"\dof2_pcgl_30_ps.vcs";
            // ShaderFile shaderfile = new(filenamepath);

            PrintZFrame(filenamepath, 0x0);
        }



        static void PrintZFrame(string filenamepath, int zframeIndex, bool disableOutput = false) {
            ShaderFile shaderFile = new(filenamepath);
            byte[] zframeDatabytes = GetZFrameByIndex(shaderFile, zframeIndex);
            PrintZFrame(zframeDatabytes, shaderFile.vcsFiletype, shaderFile.vcsSourceType, disableOutput);
        }

        static void PrintZFrame(byte[] databytes, FILETYPE vcsFiletype, VcsSourceType sourceType,
            bool disableOutput = false, StreamWriter sw = null) {
            DataReaderZFrameByteAnalysis zFrameParser = new(databytes, vcsFiletype, sourceType);
            zFrameParser.SetDisableOutput(disableOutput);
            if (sw != null) {
                zFrameParser.ConfigureWriteToFile(sw, disableOutput);
            }
            zFrameParser.PrintByteAnalysis();
        }

        static byte[] GetZFrameByIndex(ShaderFile shaderFile, int zframeIndex) {
            if (zframeIndex >= shaderFile.GetZFrameCount()) {
                throw new ShaderParserException($"Can't get zframeIndex {zframeIndex} for " +
                    $"{RemoveBaseDir(shaderFile.filenamepath)} zmax={shaderFile.GetZFrameCount() - 1}");
            }
            return shaderFile.GetDecompressedZFrameByIndex(zframeIndex);
        }





        /*
         * parse all files with the ShaderFile class (only shows status output)
         */
        static void TestShaderFile() {
            List<string> vcsFiles = GetVcsFiles(PCGL_DIR_CORE, PCGL_DIR_NOT_CORE, FILETYPE.any, -1);
            foreach (string filenamepath in vcsFiles) {
                Debug.Write($"parsing {RemoveBaseDir(filenamepath)}");
                ShaderFile shaderFile = new(filenamepath);
                Debug.WriteLine($"{shaderFile.GetZFrameCount()}");
            }
        }



        static void ShaderFileTestSinglefiles() {
            // string filenamepath = PCGL_DIR_NOT_CORE + @"\multiblend_pcgl_30_vs.vcs";
            // string filenamepath = PCGL_DIR_NOT_CORE + @"\multiblend_pcgl_30_ps.vcs";
            // string filenamepath = ARTIFACT_CLASSIC_CORE_PC_SOURCE + @"\aerial_perspective_pc_30_ps.vcs";
            string filenamepath = PCGL_DIR_CORE + @"\apply_fog_pcgl_40_features.vcs";
            ShaderFile shaderfile = new(filenamepath);
            // shaderfile.ShowZFrames();

        }


        static void VcsParsingTestSinglefiles() {
            // string filenamepath = PCGL_DIR_NOT_CORE + @"\multiblend_pcgl_30_vs.vcs";
            // string filenamepath = PCGL_DIR_NOT_CORE + @"\multiblend_pcgl_30_ps.vcs";
            // string filenamepath = PCGL_DIR_NOT_CORE + @"\hero_pcgl_30_features.vcs";
            // string filenamepath = PCGL_DIR_NOT_CORE + @"\hero_pcgl_30_vs.vcs";
            // string filenamepath = PCGL_DIR_NOT_CORE + @"\hero_pcgl_30_ps.vcs";
            // string filenamepath = PCGL_DIR_CORE + @"\generic_light_pcgl_30_features.vcs";
            // string filenamepath = PCGL_DIR_CORE + @"\generic_light_pcgl_30_vs.vcs";
            // string filenamepath = PCGL_DIR_CORE + @"\generic_light_pcgl_30_ps.vcs";
            // string filenamepath = PCGL_DIR_CORE + @"\apply_fog_pcgl_40_ps.vcs";
            // string filenamepath = PCGL_DIR_CORE + @"\tools_wireframe_pcgl_40_gs.vcs";
            // string filenamepath = PCGL_DIR_NOT_CORE + @"\spring_meteor_pcgl_30_vs.vcs";
            // string filenamepath = PC_DIR_NOT_CORE + @"\spring_meteor_pc_30_vs.vcs";
            // string filenamepath = PCGL_DIR_CORE + @"\apply_fog_pcgl_40_features.vcs";
            // string filenamepath = PCGL_DIR_CORE + @"\tools_generic_pcgl_40_ps.vcs";
            string filenamepath = PCGL_DIR_CORE + @"\visualize_cloth_pcgl_40_ps.vcs";

            // string filenamepath = ARTIFACT_CLASSIC_CORE_PC_SOURCE + @"\aerial_perspective_pc_30_ps.vcs";


            //   PC_DIR_CORE

            ShowVcsByteAnalysis(filenamepath);
        }



        static void ShowVcsByteAnalysis(string filenamepath) {
            Debug.WriteLine($"parsing {RemoveBaseDir(filenamepath)}\n");
            DataReaderVcsByteAnalysis shaderByteAnalysis = new(filenamepath);
            shaderByteAnalysis.SetShortenOutput(false);
            shaderByteAnalysis.PrintByteAnalysis();
        }

        static void WriteAllVcsFilesToTxt() {
            List<string> vcsFiles = GetVcsFiles(PCGL_DIR_CORE, PCGL_DIR_NOT_CORE, FILETYPE.any, -1);
            foreach (string vcsFile in vcsFiles) {
                WriteVcsByteAnalysisToTxt(vcsFile);
            }
        }

        static void WriteVcsByteAnalysisToTxt(string filenamepath) {
            DataReaderVcsByteAnalysis shaderByteAnalysis = new(filenamepath);
            string fileoutputNamepath = $"{OUTPUT_DIR}/{Path.GetFileName(filenamepath)[0..^4]}-analysis.txt";
            Debug.WriteLine($"writing to {fileoutputNamepath}");
            StreamWriter sw = new(fileoutputNamepath);
            sw.WriteLine($"parsing {RemoveBaseDir(filenamepath)}\n");
            shaderByteAnalysis.ConfigureWriteToFile(sw, true);
            shaderByteAnalysis.PrintByteAnalysis();
            sw.Close();
        }


        static void WriteAllVcsFilesToHtml() {
            // List<string> vcsFiles = GetVcsFiles(PCGL_DIR_CORE, PCGL_DIR_NOT_CORE, FILETYPE.any, -1);
            // List<string> vcsFiles = GetVcsFiles(PCGL_DIR_NOT_CORE, null, FILETYPE.any, -1);
            // List<string> vcsFiles = GetVcsFiles(PCGL_DIR_CORE, null, FILETYPE.any, -1);

            List<string> vcsFiles = new();
            vcsFiles.AddRange(GetVcsFiles(ARTIFACT_CLASSIC_CORE_PC_SOURCE, null, FILETYPE.features_file, -1));
            vcsFiles.AddRange(GetVcsFiles(ARTIFACT_CLASSIC_CORE_PC_SOURCE, null, FILETYPE.vs_file, -1));
            vcsFiles.AddRange(GetVcsFiles(ARTIFACT_CLASSIC_CORE_PC_SOURCE, null, FILETYPE.ps_file, -1));
            vcsFiles.AddRange(GetVcsFiles(ARTIFACT_CLASSIC_CORE_PC_SOURCE, null, FILETYPE.psrs_file, -1));
            //List<string> vcsFiles = new();
            //vcsFiles.AddRange(GetVcsFiles(ARTIFACT_CLASSIC_DCG_PC_SOURCE, null, FILETYPE.features_file, -1));
            //vcsFiles.AddRange(GetVcsFiles(ARTIFACT_CLASSIC_DCG_PC_SOURCE, null, FILETYPE.vs_file, -1));
            //vcsFiles.AddRange(GetVcsFiles(ARTIFACT_CLASSIC_DCG_PC_SOURCE, null, FILETYPE.ps_file, -1));
            //vcsFiles.AddRange(GetVcsFiles(ARTIFACT_CLASSIC_DCG_PC_SOURCE, null, FILETYPE.psrs_file, -1));

            foreach (string vcsFile in vcsFiles) {
                WriteVcsByteAnalysisToHtml(vcsFile, writeHtmlLinks: true);
            }
        }


        /*
         * set writeHtmlLinks explicitly, want this is also printing zframes
         *
         */
        static void WriteVcsByteAnalysisToHtml(string filenamepath, bool writeHtmlLinks = false) {
            DataReaderVcsByteAnalysis shaderByteAnalysis = new(filenamepath);
            string fileoutputNamepath = $"{OUTPUT_DIR}/{Path.GetFileName(filenamepath)[0..^4]}-analysis.html";
            Debug.WriteLine($"writing to {fileoutputNamepath}");
            StreamWriter sw = new(fileoutputNamepath);
            sw.WriteLine(GetHtmlHeader(Path.GetFileName(filenamepath), RemoveBaseDir(filenamepath)));
            shaderByteAnalysis.SetWriteHtmlLinks(writeHtmlLinks);
            shaderByteAnalysis.ConfigureWriteToFile(sw, true);
            shaderByteAnalysis.PrintByteAnalysis();
            sw.Close();
        }

        /*
         * This is meaningful to test the parser against all files, if unexpected output is found
         * or end of file is not reached the parser will throw ShaderParserException()
         *
         */
        static void ParseAllVcsFilesDisableOutput() {
            List<string> vcsFiles = GetVcsFiles(PCGL_DIR_CORE, PCGL_DIR_NOT_CORE, FILETYPE.any, -1);
            int filesParsed = 0;
            int[] typesParsed = new int[10];
            foreach (string filenamepath in vcsFiles) {
                Debug.Write($"parsing {RemoveBaseDir(filenamepath)}");
                DataReaderVcsByteAnalysis shaderByteAnalysis = new(filenamepath);
                shaderByteAnalysis.SetShortenOutput(false);
                shaderByteAnalysis.SetDisableOutput(true);
                shaderByteAnalysis.PrintByteAnalysis();
                Debug.WriteLine($" [SUCCESS]");
                typesParsed[(int)GetVcsFileType(filenamepath)]++;
                filesParsed++;
            }
            Debug.WriteLine($"{filesParsed} files were parsed. " +
                $"features({typesParsed[(int)FILETYPE.features_file]}), " +
                $"vs({typesParsed[(int)FILETYPE.vs_file]}), " +
                $"ps({typesParsed[(int)FILETYPE.ps_file]}), " +
                $"gs({typesParsed[(int)FILETYPE.gs_file]}), " +
                $"psrs({typesParsed[(int)FILETYPE.psrs_file]})");
        }



    }








}









