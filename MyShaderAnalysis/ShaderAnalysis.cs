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
            string filenamepath = PCGL_DIR_NOT_CORE + @"\multiblend_pcgl_30_ps.vcs";






            // WriteZFrameToFile(filenamepath, 0);
            // ParseABunchOfZframes();
            // WriteFirstZFrmeEveryFile();
            // WriteVcsCollectionAsHtml();
            // ShowVcsByteAnalysis(filenamepath);
            // WriteAllVcsFilesToTxt();
            // WriteVcsByteAnalysisToTxt(filenamepath);
            // WriteAllVcsFilesToHtml();
            // WriteVcsByteAnalysisToHtml(filenamepath);
            // ParseAllVcsFilesDisableOutput();
        }




        static void WriteVcsCollectionAsHtml() {
            // string directoryToUse = PCGL_DIR_NOT_CORE;
            // string[] filenames = { "crystal_pcgl_30_features.vcs", "crystal_pcgl_30_vs.vcs", "crystal_pcgl_30_ps.vcs" };


            (string, string, string) triple = GetTriple(PCGL_DIR_CORE+@"\visualize_cloth_pcgl_40_features.vcs");

            string[] filenames = {triple.Item1, triple.Item2, triple.Item3};
            // foreach (var filename in filenames) {
            foreach (var filenamepath in filenames) {
                // string filenamepath = @$"{directoryToUse}\{filename}";
                WriteVcsByteAnalysisToHtml(filenamepath, writeHtmlLinks: true);
                ShaderFile shaderFile = new ShaderFile(filenamepath);
                int zFrameCount = shaderFile.GetZFrameCount();
                for (int i = 0; i < zFrameCount; i++) {
                    WriteZframeAsHtml(shaderFile, i);
                }
            }
        }


        static void WriteZframeAsHtml(ShaderFile shaderFile, int zframeIndex) {
            byte[] zframeDatabytes = GetZFrameByIndex(shaderFile, zframeIndex);
            uint zframeId = (uint) shaderFile.GetZFrameIdByIndex(zframeIndex);
            string outputFilename = GetZframeHtmlFilename(zframeId, shaderFile.filenamepath);
            string outputFilenamepath = @$"{OUTPUT_DIR}\{outputFilename}";
            DataReaderZFrameByteAnalysis zFrameParser = new DataReaderZFrameByteAnalysis(zframeDatabytes, shaderFile.vcsFiletype);
            Debug.WriteLine($"writing to {outputFilenamepath}");
            StreamWriter sw = new(outputFilenamepath);
            zFrameParser.ConfigureWriteToFile(sw, true);
            zFrameParser.SetWriteAsHtml(true);
            zFrameParser.RequestGlslFileSave(OUTPUT_DIR);
            string htmlHeader = GetHtmlHeader(outputFilename, outputFilename[0..^5]);
            sw.WriteLine($"{htmlHeader}");
            zFrameParser.PrintByteAnalysis();
            sw.WriteLine($"{GetHtmlFooter()}");
            sw.Close();
        }


        static void WriteFirstZFrmeEveryFile() {
            List<string> vcsFiles = GetVcsFiles(PCGL_DIR_CORE, PCGL_DIR_NOT_CORE, FILETYPE.any, -1);
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
                PrintZFrame(zframeDatabytes, GetVcsFileType(vcsFile), true, sw);
                sw.Close();

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
                    PrintZFrame(zframeDatabytes, shaderFile.vcsFiletype, true);
                }
            }
        }

        static void WriteZFrameToFile(string filenamepath, int zframeIndex) {
            ShaderFile shaderFile = new(filenamepath);
            byte[] zframeDatabytes = GetZFrameByIndex(shaderFile, zframeIndex);
            string outputFilenamepath = @$"{OUTPUT_DIR}\{GetZframeTxtFilename((uint)shaderFile.GetZFrameIdByIndex(zframeIndex), filenamepath)}";
            StreamWriter sw = new(outputFilenamepath);
            Debug.WriteLine($"parsing {filenamepath}");
            Debug.WriteLine($"writing to {outputFilenamepath}");
            PrintZFrame(zframeDatabytes, GetVcsFileType(filenamepath), true, sw);
            sw.Close();
        }


        static void PrintZFrame(string filenamepath, int zframeIndex, bool disableOutput = false) {
            ShaderFile shaderFile = new(filenamepath);
            byte[] zframeDatabytes = GetZFrameByIndex(shaderFile, zframeIndex);
            PrintZFrame(zframeDatabytes, GetVcsFileType(filenamepath), disableOutput);
        }

        static void PrintZFrame(byte[] databytes, FILETYPE vcsFiletype, bool disableOutput = false, StreamWriter sw = null) {
            DataReaderZFrameByteAnalysis zFrameParser = new DataReaderZFrameByteAnalysis(databytes, vcsFiletype);
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
            List<string> vcsFiles = GetVcsFiles(PCGL_DIR_NOT_CORE, null, FILETYPE.any, -1);
            foreach (string vcsFile in vcsFiles) {
                WriteVcsByteAnalysisToHtml(vcsFile);
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









