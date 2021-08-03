using System.IO;
using System.Diagnostics;
using MyShaderAnalysis.vcsparsing;
using System.Collections.Generic;
using static MyShaderAnalysis.vcsparsing.UtilHelpers;


namespace MyShaderAnalysis {


    public class ShaderAnalysis {
        const string PCGL_DIR_CORE = @"X:\dota-2-VRF-exports\dota2-export-shaders-pcgl\shaders-core\vfx";
        const string PCGL_DIR_NOT_CORE = @"X:\dota-2-VRF-exports\dota2-export-shaders-pcgl\shaders\vfx";
        const string PC_DIR_CORE = @"X:\dota-2-VRF-exports\dota2-export-shaders-pc\shaders-core\vfx";
        const string PC_DIR_NOT_CORE = @"X:\dota-2-VRF-exports\dota2-export-shaders-pc\shaders\vfx";
        // const string OUTPUT_DIR = @"..\..\..\GEN-OUTPUT";
        const string OUTPUT_DIR = @"Z:\active\projects\dota2-sourcesdk-modding\shader-analysis-vcs-format\OUTPUT_DUMP";



        public static void RunTrials() {

            // StaticAnalysisSelectedSets();
            // WriteZFramesToFile();
            // ParseZFrames();
            ParseVcsFiles();
        }





        static void ParseZFrames() {


            IncrementValue1 myinc = new();

            testOverload(myinc + 10);



            Debug.WriteLine(myinc);



        }


        static void testOverload(IncrementValue1 sdf) {
            Debug.WriteLine($"hello");
        }







        static void ParseVcsFiles() {
            // string filenamepath = PCGL_DIR_NOT_CORE + @"\multiblend_pcgl_30_vs.vcs";
            // string filenamepath = PCGL_DIR_NOT_CORE + @"\multiblend_pcgl_30_ps.vcs";
            // string filenamepath = PCGL_DIR_NOT_CORE + @"\hero_pcgl_30_features.vcs";
            // string filenamepath = PCGL_DIR_NOT_CORE + @"\hero_pcgl_30_vs.vcs";
            // string filenamepath = PCGL_DIR_NOT_CORE + @"\hero_pcgl_30_ps.vcs";
            // string filenamepath = PCGL_DIR_CORE + @"\generic_light_pcgl_30_features.vcs";
            string filenamepath = PCGL_DIR_CORE + @"\generic_light_pcgl_30_vs.vcs";
            // string filenamepath = PCGL_DIR_CORE + @"\generic_light_pcgl_30_ps.vcs";
            // string filenamepath = PCGL_DIR_CORE + @"\apply_fog_pcgl_40_ps.vcs";
            // string filenamepath = PCGL_DIR_CORE + @"\tools_wireframe_pcgl_40_gs.vcs";
            // string filenamepath = PCGL_DIR_NOT_CORE + @"\spring_meteor_pcgl_30_vs.vcs";
            // string filenamepath = PCGL_DIR_NOT_CORE + @"\cables_pcgl_30_features.vcs";
            // string filenamepath = PCGL_DIR_NOT_CORE + @"\cables_pcgl_30_vs.vcs";
            // string filenamepath = PCGL_DIR_NOT_CORE + @"\spritecard_pcgl_30_features.vcs";
            // string filenamepath = PCGL_DIR_NOT_CORE + @"\blur_pcgl_30_features.vcs";


            ShowVcsByteAnalysis(filenamepath);
            // WriteAllVcsFilesToTxt();
            // WriteVcsByteAnalysisToTxt(filenamepath);
            // WriteAllVcsFilesToHtml();
            // WriteVcsByteAnalysisToHtml(filenamepath);
            // ParseAllVcsFilesDisableOutput();
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
            List<string> vcsFiles = GetVcsFiles(PCGL_DIR_CORE, PCGL_DIR_NOT_CORE, FILETYPE.any, -1);
            foreach (string vcsFile in vcsFiles) {
                WriteVcsByteAnalysisToHtml(vcsFile);
            }
        }

        static void WriteVcsByteAnalysisToHtml(string filenamepath) {
            DataReaderVcsByteAnalysis shaderByteAnalysis = new(filenamepath);
            string fileoutputNamepath = $"{OUTPUT_DIR}/{Path.GetFileName(filenamepath)[0..^4]}-analysis.html";
            Debug.WriteLine($"writing to {fileoutputNamepath}");
            StreamWriter sw = new(fileoutputNamepath);
            sw.WriteLine(GetHtmlHeader(Path.GetFileName(filenamepath), RemoveBaseDir(filenamepath)));
            // shaderByteAnalysis.SetWriteHtmlLinks(true); // don't want this here if not also writing the zframes
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
                typesParsed[(int) GetVcsFileType(filenamepath)]++;
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









