using System;
using System.IO;
using System.Collections.Generic;

using VcsProgramType = ValveResourceFormat.CompiledShader.VcsProgramType;
using ShaderParserException = ValveResourceFormat.CompiledShader.ShaderParserException;
using ShaderFile = ValveResourceFormat.CompiledShader.ShaderFile;
using ZFrameFile = ValveResourceFormat.CompiledShader.ZFrameFile;
using GpuSource = ValveResourceFormat.CompiledShader.GpuSource;

using static MyShaderAnalysis.codestash.FileSystemOld;
using static MyShaderAnalysis.codestash.MyTrashUtilHelpers;


namespace MyShaderAnalysis.codestash
{
    public class PrintoutsByteAnalysis
    {

        // const string OUTPUT_DIR = @"Z:/active/projects/dota2-sourcesdk-modding/shader-analysis-vcs-format/OUTPUT_DUMP";
        // const string SERVER_OUTPUT_DIR = @"Z:/dev/www/vcs.codecreation.dev/GEN-output";



        public static void RunTrials()
        {
            // Trial2();
            RunBatchPrintVcsBytes();
            // RunPrintVcsFilesAsBytes();
            // PrintVcsFileToScreen();
            RunBatchPrintZframeBytes();
            // RunPrintZFrameBytes();
            // DemoPrintZFrameBytes(10);
            // PrintZFrameBytesAsTestfile();
            // DemoZframeHeaderAndTitle();
            // PrintZFrameBytesToScreen()


            PrintGlslAllFiles();
            // PrintGlslSingleFiles();

        }



        static void RunBatchPrintVcsBytes()
        {
            // List<string> vcsFiles = GetVcsFiles(DOTA_CORE_PC_SOURCE, DOTA_GAME_PC_SOURCE, VcsFileType.Any, -1, LIMIT_NR: 20);
            // List<string> vcsFiles = GetVcsFiles(DOTA_CORE_PCGL_SOURCE, DOTA_GAME_PCGL_SOURCE, VcsFileType.Any, -1, LIMIT_NR: 20);
            // List<string> vcsFiles = GetVcsFiles(DOTA_CORE_MOBILE_GLES_SOURCE, DOTA_DAC_MOBILE_GLES_SOURCE, VcsProgramType.Undetermined, -1, LIMIT_NR: 20);
            // List<string> vcsFiles = GetVcsFiles(ARTIFACT_CLASSIC_CORE_PC_SOURCE, ARTIFACT_CLASSIC_DCG_PC_SOURCE, VcsProgramType.Undetermined, -1, LIMIT_NR: 20);
            List<string> vcsFiles = GetVcsFiles(DOTA_GAME_PCGL_SOURCE, VcsProgramType.Undetermined, -1, LIMIT_NR: 20);
            BatchPrintVcsBytes(vcsFiles);
        }

        static void BatchPrintVcsBytes(List<string> vcsFiles)
        {
            foreach (var filenamepath in vcsFiles)
            {
                PrintVcsFilesAsBytes(filenamepath);
            }
        }


        static void RunPrintVcsFilesAsBytes()
        {
            // string filenamepath = $"{DOTA_CORE_PCGL_SOURCE}/cs_compress_dxt5_pcgl_30_features.vcs";
            // string filenamepath = $"{DOTA_GAME_PCGL_SOURCE}/hero_pcgl_30_features.vcs";
            // string filenamepath = $"{DOTA_GAME_PCGL_SOURCE}/hero_pcgl_30_vs.vcs";
            // string filenamepath = $"{DOTA_GAME_PCGL_SOURCE}/hero_pcgl_30_ps.vcs";
            // string filenamepath = $"{DOTA_GAME_PCGL_SOURCE}/hero_pcgl_30_psrs.vcs";
            // string filenamepath = $"{DOTA_GAME_PC_SOURCE}/hero_pc_30_vs.vcs";
            // string filenamepath = $"{DOTA_GAME_PCGL_SOURCE}/multiblend_pcgl_30_features.vcs";
            // string filenamepath = $"{DOTA_GAME_PCGL_SOURCE}/multiblend_pcgl_30_vs.vcs";
            string filenamepath = $"{DOTA_GAME_PCGL_SOURCE}/multiblend_pcgl_30_ps.vcs";
            // string filenamepath = $"{DOTA_GAME_PC_SOURCE}/multiblend_pc_30_features.vcs";
            // string filenamepath = $"{DOTA_GAME_PC_SOURCE}/multiblend_pc_30_vs.vcs";
            // string filenamepath = $"{DOTA_GAME_PC_SOURCE}/multiblend_pc_30_ps.vcs";
            // string filenamepath = $"{DOTA_CORE_PCGL_SOURCE}/generic_light_pcgl_30_features.vcs";
            // string filenamepath = $"{DOTA_CORE_PCGL_SOURCE}/generic_light_pcgl_30_vs.vcs";
            // string filenamepath = $"{DOTA_CORE_PC_SOURCE}/generic_light_pc_30_features.vcs";
            // string filenamepath = $"{DOTA_CORE_PC_SOURCE}/generic_light_pc_30_vs.vcs";
            // string filenamepath = $"{DOTA_CORE_PCGL_SOURCE}/generic_light_pcgl_30_ps.vcs";
            // string filenamepath = $"{DOTA_CORE_PCGL_SOURCE}/spritecard_pcgl_30_features.vcs";
            // string filenamepath = $"{DOTA_CORE_PCGL_SOURCE}/spritecard_pcgl_30_ps.vcs";
            // string filenamepath = $"{DOTA_CORE_PCGL_SOURCE}/spritecard_pcgl_30_vs.vcs";
            // string filenamepath = $"{DOTA_CORE_PCGL_SOURCE}/msaa_resolve_cs_pcgl_50_features.vcs"; // strange file that doesn't contain any data

            PrintVcsFilesAsBytes(filenamepath);
        }


        static void PrintVcsFilesAsBytes(string filenamepath)
        {
            FileTokensOld fileTokens = new FileTokensOld(filenamepath);
            string outputFilenamepath = $"{fileTokens.GetServerFilenamepath("bytes", createDirs: true)}";
            FileWriter fileWriter = new FileWriter(outputFilenamepath, showOutputToConsole: false);

            fileWriter.WriteHtmlHeader(GetShortName(filenamepath), RemoveBaseDir(filenamepath));
            // new PrintoutSingleFile(filenamepath, fileWriter.GetOutputWriter());

            ShaderFile shaderFile = InstantiateShaderFile(filenamepath);
            shaderFile.PrintByteDetail(outputWriter: fileWriter.GetOutputWriter());
            fileWriter.CloseStreamWriter();
        }




        static void PrintVcsFileToScreen()
        {
            // string filenamepath = $"{V62_EXAMPLES_SOURCE}/test_pcgl_30_ps.vcs";
            // string filenamepath = $"{V62_EXAMPLES_SOURCE}/test_pc_30_vs.vcs";
            // string filenamepath = $"{V62_EXAMPLES_SOURCE}/test_pc_40_gs.vcs";
            // string filenamepath = $"{V62_EXAMPLES_SOURCE}/test_pc_50_hs.vcs";
            // string filenamepath = $"{V62_EXAMPLES_SOURCE}/test_pcgl_40_ps.vcs";
            // string filenamepath = $"{THE_LAB_SOURCE}/downsample_pc_40_features.vcs";
            // string filenamepath = $"{THE_LAB_SOURCE}/downsample_pc_40_ps.vcs";
            // string filenamepath = $"{THE_LAB_SOURCE}/fogsprite_pcgl_41_features.vcs";
            // string filenamepath = $"{THE_LAB_SOURCE}/foliage_pc_41_ps.vcs";
            // string filenamepath = $"{THE_LAB_SOURCE}/foliage_pc_41_features.vcs";


            // -- using DataReaderVcsBytes
            // string filenamepath = $"{DOTA_GAME_PCGL_SOURCE}/multiblend_pcgl_30_vs.vcs";
            // DataReaderVcsBytes testfile = new DataReaderVcsBytes(filenamepath);
            // testfile.PrintByteDetail();

            // -- using ShaderFile
            string filenamepath = $"{DOTA_GAME_PCGL_SOURCE}/multiblend_pcgl_30_vs.vcs";
            ShaderFile shaderFile = InstantiateShaderFile(filenamepath);
            shaderFile.PrintByteDetail();
        }



        static void Trial2()
        {
            string filenamepath = $"{DOTA_CORE_PCGL_SOURCE}/generic_light_pcgl_30_vs.vcs";
            ShaderFile shaderFile = InstantiateShaderFile(filenamepath);
            ZFrameFile zframeFile = shaderFile.GetZFrameFileByIndex(0);

            GpuSource gpuSource = zframeFile.gpuSources[0];
            // gpuSource.sourcebytes
        }




        const int LIMIT_ZFRAME_PRINTOUT = 5;

        static void RunBatchPrintZframeBytes()
        {
            // List<string> vcsFiles = GetVcsFiles(DOTA_CORE_PCGL_SOURCE, DOTA_GAME_PCGL_SOURCE, VcsProgramType.Undetermined, -1, LIMIT_NR: 20);
            List<string> vcsFiles = GetVcsFiles(DOTA_GAME_PCGL_SOURCE, VcsProgramType.Undetermined, -1, LIMIT_NR: 20);
            // List<string> vcsFiles = GetVcsFiles(DOTA_CORE_MOBILE_GLES_SOURCE, DOTA_DAC_MOBILE_GLES_SOURCE, VcsFileType.Any, 30);
            // List<string> vcsFiles = GetVcsFiles(DOTA_CORE_MOBILE_GLES_SOURCE, DOTA_DAC_MOBILE_GLES_SOURCE, VcsProgramType.Undetermined, -1);
            // List<string> vcsFiles = GetVcsFiles(ARTIFACT_CLASSIC_CORE_PC_SOURCE, ARTIFACT_CLASSIC_DCG_PC_SOURCE, VcsProgramType.Undetermined, -1);
            foreach (var filenamepath in vcsFiles)
            {
                BatchPrintZframeBytes(filenamepath, LIMIT_ZFRAME_PRINTOUT);
            }
        }


        static void BatchPrintZframeBytes(string filenamepath, int limitZframes)
        {
            ShaderFile shaderFile = InstantiateShaderFile(filenamepath);
            FileTokensOld fileTokens = new FileTokensOld(filenamepath);
            int zframesToPrint = Math.Min(shaderFile.GetZFrameCount(), limitZframes);
            for (int i = 0; i < zframesToPrint; i++)
            {
                ZFrameFile zframeFile = shaderFile.GetZFrameFileByIndex(i);
                PrintZFrameBytes(zframeFile, fileTokens);
            }
        }


        static void RunPrintZFrameBytes()
        {
            int ZFRAME_INDEX = 11;
            string filenamepath = $"{DOTA_GAME_PCGL_SOURCE}/multiblend_pcgl_30_vs.vcs";
            ShaderFile shaderFile = InstantiateShaderFile(filenamepath);
            FileTokensOld fileTokens = new FileTokensOld(filenamepath);
            ZFrameFile zframe = shaderFile.GetZFrameFileByIndex(ZFRAME_INDEX);
            PrintZFrameBytes(zframe, fileTokens);
        }


        static void PrintZFrameBytes(ZFrameFile zframe, FileTokensOld fileTokens = null)
        {
            if (fileTokens == null)
            {
                fileTokens = new FileTokensOld(zframe.filenamepath);
            }
            string outputFilenamepath = fileTokens.GetZFrameHtmlFilenamepath(zframe.zframeId, "bytes", createDirs: true);
            FileWriter fileWriter = new FileWriter(outputFilenamepath, showOutputToConsole: false);

            string htmlTitle = $"{fileTokens.namelabel}-Z[0x{zframe.zframeId:x}]";
            string htmlHeader = fileTokens.GetZFrameHtmlFilename(zframe.zframeId, "bytes")[..^5];
            fileWriter.WriteHtmlHeader(htmlTitle, htmlHeader);

            zframe.PrintByteDetail(outputWriter: fileWriter.GetOutputWriter());
            fileWriter.CloseStreamWriter();
        }


        /*
         * Note this is printed by index not by zframeId
         *
         * may throw ArgumentOutOfRangeException
         *
         */
        static void DemoPrintZFrameBytes(int zframeIndex)
        {
            string filenamepath = $"{DOTA_GAME_PCGL_SOURCE}/multiblend_pcgl_30_vs.vcs";
            ShaderFile shaderFile = InstantiateShaderFile(filenamepath);
            ZFrameFile zframe = shaderFile.GetZFrameFileByIndex(zframeIndex);

            FileTokensOld fileTokens = new FileTokensOld(filenamepath);
            string outputFilenamepath = fileTokens.GetZFrameHtmlFilenamepath(zframe.zframeId, "bytes", createDirs: true);
            FileWriter fileWriter = new FileWriter(outputFilenamepath, showOutputToConsole: false);

            string htmlTitle = $"{fileTokens.namelabel}-Z[0x{zframe.zframeId:x}]";
            string htmlHeader = fileTokens.GetZFrameHtmlFilename(zframe.zframeId, "bytes")[..^5];
            fileWriter.WriteHtmlHeader(htmlTitle, htmlHeader);

            zframe.PrintByteDetail(outputWriter: fileWriter.GetOutputWriter());
            fileWriter.CloseStreamWriter();
        }


        static void PrintZFrameBytesAsTestfile()
        {
            long ZFRAME_ID = 0;
            string filenamepath = $"{DOTA_CORE_PCGL_SOURCE}/generic_light_pcgl_30_vs.vcs";
            ShaderFile shaderFile = InstantiateShaderFile(filenamepath);
            ZFrameFile zframeFile = shaderFile.GetZFrameFileByIndex(0);
            FileTokensOld fileTokens = new FileTokensOld(filenamepath);

            string outputFilenamepath = $"{GetServerTestDir()}/testfile.html";
            FileWriter fileWriter = new FileWriter(outputFilenamepath, showOutputToConsole: false);

            string htmlTitle = $"{fileTokens.namelabel}-Z[0x{ZFRAME_ID:x}]";
            string htmlHeader = fileTokens.GetZFrameHtmlFilename(ZFRAME_ID, "bytes")[..^5];
            fileWriter.WriteHtmlHeader(htmlTitle, htmlHeader);

            zframeFile.PrintByteDetail(outputWriter: fileWriter.GetOutputWriter());
            fileWriter.CloseStreamWriter();
        }



        static void DemoZframeHeaderAndTitle()
        {
            string filenamepath = $"{DOTA_GAME_PCGL_SOURCE}/multiblend_pcgl_30_vs.vcs";
            FileTokensOld fileTokens = new FileTokensOld(filenamepath);
            int zframeId = 20;

            // html title
            Console.WriteLine($"{fileTokens.namelabel}-Z[0x{zframeId:x}]");

            // html header
            Console.WriteLine(fileTokens.GetZFrameHtmlFilename(zframeId, "bytes")[..^5]);
        }


        /*
         * there are two ways to print the zframe bytes
         * DataReaderZFrameBytes.cs is used for fixing bugs and analysis version changes
         *
         */
        static void PrintZFrameBytesToScreen()
        {
            // -- using the datareader directly
            // string filenamepath = $"{DOTA_GAME_PCGL_SOURCE}/multiblend_pcgl_30_vs.vcs";
            // ShaderFile shaderFile = InstantiateShaderFile(filenamepath);
            // byte[] zframeBytes = shaderFile.GetDecompressedZFrameByIndex(0);
            // new DataReaderZFrameBytes(zframeBytes, shaderFile.vcsProgramType, shaderFile.vcsPlatformType, shaderFile.vcsShaderModelType);

            string filenamepath = $"{DOTA_GAME_PCGL_SOURCE}/multiblend_pcgl_30_vs.vcs";
            ShaderFile shaderFile = InstantiateShaderFile(filenamepath);
            ZFrameFile zframe = shaderFile.GetZFrameFileByIndex(0);
            zframe.PrintByteDetail();
        }


        static int LIMIT_GPU_SOURCES_TO_PRINT = 2;

        static void PrintGlslAllFiles()
        {

            // List<string> vcsFiles = GetVcsFiles(DOTA_CORE_PCGL_SOURCE, DOTA_GAME_PCGL_SOURCE, VcsProgramType.Undetermined, -1, LIMIT_NR: 20);
            List<string> vcsFiles = GetVcsFiles(DOTA_GAME_PCGL_SOURCE, VcsProgramType.Undetermined, -1, LIMIT_NR: 20);
            // List<string> vcsFiles = GetVcsFiles(DOTA_GAME_PCGL_SOURCE, DOTA_CORE_PCGL_SOURCE, VcsProgramType.Undetermined, 30, LIMIT_NR: 20);
            // List<string> vcsFiles = GetVcsFiles(DOTA_DAC_MOBILE_GLES_SOURCE, DOTA_CORE_MOBILE_GLES_SOURCE, VcsProgramType.Undetermined, 30, LIMIT_NR: 20);
            // List<string> vcsFiles = GetVcsFiles(DOTA_DAC_MOBILE_GLES_SOURCE, DOTA_CORE_MOBILE_GLES_SOURCE, VcsProgramType.Undetermined, -1, LIMIT_NR: 20);

            foreach (var filenamepath in vcsFiles)
            {
                FileTokensOld fileTokens = new FileTokensOld(filenamepath);
                if (!fileTokens.sourceType.Equals("glsl") && !fileTokens.sourceType.Equals("gles"))
                {
                    throw new ShaderParserException("This only makes sense for glsl or gles sources");
                }
                string glslServerDir = fileTokens.GetGlslServerDir(createDirs: true);
                ShaderFile shaderFile = InstantiateShaderFile(filenamepath);
                int zframesToPrint = Math.Min(shaderFile.GetZFrameCount(), LIMIT_ZFRAME_PRINTOUT);
                for (int i = 0; i < zframesToPrint; i++)
                {
                    ZFrameFile zframeFile = shaderFile.GetZFrameFileByIndex(i);
                    int gpuSourceToPrint = Math.Min(zframeFile.gpuSources.Count, LIMIT_GPU_SOURCES_TO_PRINT);
                    for (int j = 0; j < gpuSourceToPrint; j++)
                    {
                        var glslSource = zframeFile.gpuSources[j];
                        string outputFilenamepath = $"{glslServerDir}/{fileTokens.GetGlslHtmlFilename(glslSource.GetEditorRefIdAsString())}";
                        WriteBytesToFile(glslSource.sourcebytes, outputFilenamepath);
                    }

                    //foreach (var glslSource in zframeFile.gpuSources)
                    //{
                    //    string outputFilenamepath = $"{glslServerDir}/{fileTokens.GetGlslHtmlFilename((GlslSource)glslSource)}";
                    //    WriteBytesToFile(glslSource.sourcebytes, outputFilenamepath, overWrite: false);
                    //}
                }
            }
        }





        /*
         * This currently only makes sense for glsl sources (it's the only one where the bytes are the sourcecode)
         *
         * NOTE - method not in use!!
         *
         *
         */
        static void PrintGlslSingleFiles()
        {
            // string filenamepath = $"{DOTA_CORE_PCGL_SOURCE}/apply_fog_pcgl_40_ps.vcs";
            string filenamepath = $"{DOTA_CORE_PCGL_SOURCE}/bilateral_blur_pcgl_30_ps.vcs";
            // string filenamepath = $"{DOTA_GAME_PCGL_SOURCE}/multiblend_pcgl_30_vs.vcs";
            FileTokensOld fileTokens = new FileTokensOld(filenamepath);
            if (!fileTokens.sourceType.Equals("glsl"))
            {
                throw new ShaderParserException("This only makes sense for glsl sources");
            }
            string glslServerDir = fileTokens.GetGlslServerDir(createDirs: true);
            ShaderFile shaderFile = InstantiateShaderFile(filenamepath);
            int zframesToPrint = Math.Min(shaderFile.GetZFrameCount(), LIMIT_ZFRAME_PRINTOUT);
            for (int i = 0; i < zframesToPrint; i++)
            {
                ZFrameFile zframeFile = shaderFile.GetZFrameFileByIndex(i);

                int gpuSourceToPrint = Math.Min(zframeFile.gpuSources.Count, LIMIT_GPU_SOURCES_TO_PRINT);
                for (int j = 0; j < gpuSourceToPrint; j++)
                {
                    var glslSource = zframeFile.gpuSources[j];
                    string outputFilenamepath = $"{glslServerDir}/{fileTokens.GetGlslHtmlFilename(glslSource.GetEditorRefIdAsString())}";
                    WriteBytesToFile(glslSource.sourcebytes, outputFilenamepath);
                }


                //foreach (var glslSource in zframeFile.gpuSources)
                //{
                //    string outputFilenamepath = $"{glslServerDir}/{fileTokens.GetGlslHtmlFilename((GlslSource)glslSource)}";
                //    WriteBytesToFile(glslSource.sourcebytes, outputFilenamepath);
                //}
            }
        }


        /*
         * This method is only called by the two functions above PrintGlslAllFiles() and PrintGlslSingleFiles()
         * (it only makes sense to write databytes here directly to file if the databytes are ascii text)
         *
         */
        static void WriteBytesToFile(byte[] databytes, string outputFilenamepath, bool writeAsHtml = true, bool overWrite = false)
        {
            if (!overWrite && File.Exists(outputFilenamepath))
            {
                Console.WriteLine($"SKIPPING {outputFilenamepath}");
                return;
            }
            Console.WriteLine($"writing to {outputFilenamepath}");

            if (writeAsHtml)
            {
                string htmlTitle = Path.GetFileName(outputFilenamepath)[0..^5];
                StreamWriter databytesFileWriter = new(outputFilenamepath);
                string htmlHeader = GetHtmlHeader(htmlTitle, htmlTitle);
                databytesFileWriter.WriteLine($"{htmlHeader}");
                databytesFileWriter.Flush();
                databytesFileWriter.BaseStream.Write(databytes, 0, databytes.Length);
                databytesFileWriter.Flush();
                databytesFileWriter.WriteLine($"{GetHtmlFooter()}");
                databytesFileWriter.Flush();
                databytesFileWriter.Close();
            }
            else
            {
                File.WriteAllBytes(outputFilenamepath, databytes);
            }
        }




    }
}
