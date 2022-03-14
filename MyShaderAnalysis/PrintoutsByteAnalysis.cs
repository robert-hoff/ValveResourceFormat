using System;
using System.IO;
using System.Collections.Generic;
using MyShaderAnalysis.utilhelpers;
using ValveResourceFormat.CompiledShader;
using static MyShaderAnalysis.utilhelpers.FileSystem;
using static MyShaderAnalysis.utilhelpers.MyShaderUtilHelpers;
using static MyShaderAnalysis.utilhelpers.ReadShaderFile;
using static ValveResourceFormat.CompiledShader.ShaderUtilHelpers;
using System.Diagnostics;

namespace MyShaderAnalysis
{
    public class PrintoutsByteAnalysis
    {
        public const string V62_EXAMPLES_SOURCE = "X:/v62shaders-from-xpaw";
        public const string THE_LAB_SOURCE = "X:/Steam/steamapps/common/The Lab/RobotRepair/core/shaders/vfx";
        const string OUTPUT_DIR = @"Z:/active/projects/dota2-sourcesdk-modding/shader-analysis-vcs-format/OUTPUT_DUMP";
        const string SERVER_OUTPUT_DIR = @"Z:/dev/www/vcs.codecreation.dev/GEN-output";
        // static OutputWriter output = new(WriteToConsole: false, WriteToDebug: true);
        static OutputFormatterTabulatedData output = new OutputFormatterTabulatedData();



        public static void RunTrials()
        {
            // Trial1();
            // Trial2();
            // Trial3();
            // PrintAllByteAnalysis();
            // RunSingleFileByteAnalysis();
            // PrintSingleFileByteAnalysis(filenamepath, $"{SERVER_OUTPUT_DIR}/testfile.html", writeFile: true);
            PrintSingleFileScreenOnly();
            // PrintZFramesAllFiles();
            // PrintZFramesSingleFile();
            // PrintGlslAllFiles();
            // PrintGlslSingleFiles();


            // todo - need something new here
            // output.CloseStreamWriter();
        }


        static void PrintAllByteAnalysis()
        {
            // List<string> vcsFiles = GetVcsFiles(DOTA_CORE_PC_SOURCE, DOTA_GAME_PC_SOURCE, VcsFileType.Any, -1);
            // List<string> vcsFiles = GetVcsFiles(DOTA_CORE_PCGL_SOURCE, DOTA_GAME_PCGL_SOURCE, VcsFileType.Any, -1);
            // List<string> vcsFiles = GetVcsFiles(DOTA_CORE_MOBILE_GLES_SOURCE, DOTA_DAC_MOBILE_GLES_SOURCE, VcsProgramType.Undetermined, -1);
            List<string> vcsFiles = GetVcsFiles(ARTIFACT_CLASSIC_CORE_PC_SOURCE, ARTIFACT_CLASSIC_DCG_PC_SOURCE, VcsProgramType.Undetermined, -1);
            foreach (var filenamepath in vcsFiles)
            {
                Console.WriteLine(filenamepath);
                FileTokens fileTokens = new FileTokens(filenamepath);
                if (fileTokens.vcsProgramType == VcsProgramType.ComputeShader || fileTokens.vcsProgramType == VcsProgramType.GeometryShader)
                {
                    continue;
                }
                string outputFilenamepath = $"{fileTokens.GetServerFilenamepath("bytes", createDirs: true)}";
                PrintSingleFileByteAnalysis(filenamepath, outputFilenamepath, writeFile: true, disableOutput: true);

                // todo - need something new here
                // output.CloseStreamWriter();
            }
        }



        static void RunSingleFileByteAnalysis()
        {
            // string filenamepath = $"{DOTA_CORE_PCGL_SOURCE}/cs_compress_dxt5_pcgl_30_features.vcs";
            // string filenamepath = $"{DOTA_GAME_PCGL_SOURCE}/hero_pcgl_30_features.vcs";
            // string filenamepath = $"{DOTA_GAME_PCGL_SOURCE}/hero_pcgl_30_vs.vcs";
            // string filenamepath = $"{DOTA_GAME_PCGL_SOURCE}/hero_pcgl_30_ps.vcs";
            // string filenamepath = $"{DOTA_GAME_PCGL_SOURCE}/hero_pcgl_30_psrs.vcs";
            // string filenamepath = $"{DOTA_GAME_PC_SOURCE}/hero_pc_30_vs.vcs";
            // string filenamepath = $"{DOTA_GAME_PCGL_SOURCE}/multiblend_pcgl_30_features.vcs";
            // string filenamepath = $"{DOTA_GAME_PCGL_SOURCE}/multiblend_pcgl_30_vs.vcs";
            // string filenamepath = $"{DOTA_GAME_PCGL_SOURCE}/multiblend_pcgl_30_ps.vcs";
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

        }



        static void PrintSingleFileByteAnalysis(string filenamepath, string outputFilenamepath = null,
            bool writeFile = false, bool disableOutput = false)
        {
            ShaderFile shaderFile = null;
            //try
            //{
                shaderFile = InstantiateShaderFile(filenamepath);
            //} catch (Exception e)
            //{
            //    Console.WriteLine($"ERROR! couldn't parse {filenamepath}");
            //    Console.WriteLine(e.Message);
            //    return;
            //}


            // todo - need something new here
            /*
            FileTokens fileTokens = new FileTokens(filenamepath);
            if (outputFilenamepath != null && writeFile)
            {
                output.SetOutputFile(outputFilenamepath);
                output.WriteAsHtml(fileTokens.GetAbbreviatedName(), $"{ShortHandName(filenamepath)}");
                if (disableOutput)
                {
                    output.DisableOutput();
                }
                shaderFile.datareader.OutputWriter = (x) => { output.sw.Write(x); };
                // shaderFile.datareader.OutputWriter = (x) => { Console.Write(x); };
            }
            shaderFile.PrintByteAnalysis();
            */
        }


        static void PrintSingleFileScreenOnly()
        {
            // string filenamepath = $"{V62_EXAMPLES_SOURCE}/test_pcgl_30_ps.vcs";
            // string filenamepath = $"{V62_EXAMPLES_SOURCE}/test_pc_30_vs.vcs";
            // string filenamepath = $"{V62_EXAMPLES_SOURCE}/test_pc_40_gs.vcs";
            // string filenamepath = $"{V62_EXAMPLES_SOURCE}/test_pc_50_hs.vcs";
            // string filenamepath = $"{V62_EXAMPLES_SOURCE}/test_pcgl_40_ps.vcs";
            // string filenamepath = $"{THE_LAB_SOURCE}/downsample_pc_40_ps.vcs";
            // string filenamepath = $"{THE_LAB_SOURCE}/downsample_pc_40_features.vcs";
            // string filenamepath = $"{THE_LAB_SOURCE}/fogsprite_pcgl_41_features.vcs";
            // string filenamepath = $"{THE_LAB_SOURCE}/foliage_pc_41_ps.vcs";
            string filenamepath = $"{THE_LAB_SOURCE}/foliage_pc_41_features.vcs";



            DataReaderVcsByteAnalysis testfile = new DataReaderVcsByteAnalysis(filenamepath);
            testfile.PrintByteAnalysis();


        }



        static void Trial2()
        {
            string filenamepath = $"{DOTA_CORE_PCGL_SOURCE}/generic_light_pcgl_30_vs.vcs";
            ShaderFile shaderFile = InstantiateShaderFile(filenamepath);
            ZFrameFile zframeFile = shaderFile.GetZFrameFileByIndex(0);

            GpuSource gpuSource = zframeFile.gpuSources[0];
            // gpuSource.sourcebytes



        }


        static void Trial3()
        {
            string filenamepath = $"{DOTA_CORE_PCGL_SOURCE}/generic_light_pcgl_30_vs.vcs";
            ShaderFile shaderFile = InstantiateShaderFile(filenamepath);
            ZFrameFile zframeFile = shaderFile.GetZFrameFileByIndex(0);
            PrintZFrameByteAnalysis(zframeFile, $"{SERVER_OUTPUT_DIR}/testfile.html", writeFile: true);


        }



        const int LIMIT_ZFRAME_PRINTOUT = 5;


        static void PrintZFramesAllFiles()
        {
            // List<string> vcsFiles = GetVcsFiles(DOTA_CORE_PCGL_SOURCE, DOTA_GAME_PCGL_SOURCE, VcsFileType.Any, -1);
            // List<string> vcsFiles = GetVcsFiles(DOTA_CORE_MOBILE_GLES_SOURCE, DOTA_DAC_MOBILE_GLES_SOURCE, VcsFileType.Any, 30);
            // List<string> vcsFiles = GetVcsFiles(DOTA_CORE_MOBILE_GLES_SOURCE, DOTA_DAC_MOBILE_GLES_SOURCE, VcsProgramType.Undetermined, -1);
            List<string> vcsFiles = GetVcsFiles(ARTIFACT_CLASSIC_CORE_PC_SOURCE, ARTIFACT_CLASSIC_DCG_PC_SOURCE, VcsProgramType.Undetermined, -1);

            foreach (var filenamepath in vcsFiles)
            {
                Console.WriteLine($"{filenamepath}");
                FileTokens fileTokens = new FileTokens(filenamepath);
                string zframesServerDir = fileTokens.GetZFramesServerDir(createDirs: true);

                ShaderFile shaderFile = null;
                try
                {
                    shaderFile = InstantiateShaderFile(filenamepath);
                } catch (ShaderParserException e)
                {
                    Console.WriteLine($"ERROR! couldn't parse {filenamepath}");
                    Console.WriteLine(e);
                    continue;
                }
                int zframesToPrint = Math.Min(shaderFile.GetZFrameCount(), LIMIT_ZFRAME_PRINTOUT);
                for (int i = 0; i < zframesToPrint; i++)
                {
                    ZFrameFile zframeFile = shaderFile.GetZFrameFileByIndex(i);
                    string zframeHtmlFilename = fileTokens.GetZFrameHtmlFilename(zframeFile.zframeId, "bytes");
                    PrintZFrameByteAnalysis(zframeFile, $"{zframesServerDir}/{zframeHtmlFilename}", writeFile: true, disableOutput: true);

                    // todo - need something new here
                    // output.CloseStreamWriter();
                }
            }
        }


        static void PrintZFramesSingleFile()
        {
            // string filenamepath = $"{DOTA_CORE_PCGL_SOURCE}/apply_fog_pcgl_40_ps.vcs";
            // string filenamepath = $"{DOTA_CORE_PCGL_SOURCE}/bilateral_blur_pcgl_30_ps.vcs";
            // string filenamepath = $"{DOTA_GAME_PCGL_SOURCE}/multiblend_pcgl_30_vs.vcs";
            string filenamepath = $"{DOTA_CORE_MOBILE_GLES_SOURCE}/copytexture_mobile_gles_30_vs.vcs";
            FileTokens fileTokens = new FileTokens(filenamepath);
            string zframesServerDir = fileTokens.GetZFramesServerDir(createDirs: true);

            ShaderFile shaderFile = InstantiateShaderFile(filenamepath);
            int zframesToPrint = Math.Min(shaderFile.GetZFrameCount(), LIMIT_ZFRAME_PRINTOUT);
            for (int i = 0; i < zframesToPrint; i++)
            {
                ZFrameFile zframeFile = shaderFile.GetZFrameFileByIndex(i);
                string zframeHtmlFilename = fileTokens.GetZFrameHtmlFilename(zframeFile.zframeId, "bytes");
                PrintZFrameByteAnalysis(zframeFile, $"{zframesServerDir}/{zframeHtmlFilename}", writeFile: true, disableOutput: true);

                // todo - need something new here
                // output.CloseStreamWriter();
            }
        }


        static void PrintZFrameByteAnalysis(ZFrameFile zframeFile, string outputFilenamepath = null,
            bool writeFile = false, bool disableOutput = false)
        {
            FileTokens fileTokens = new FileTokens(zframeFile.filenamepath);
            if (outputFilenamepath != null && writeFile)
            {


                /*
                output.SetOutputFile(outputFilenamepath);
                output.WriteAsHtml($"{fileTokens.namelabel}-Z[0x{zframeFile.zframeId:x}]",
                    $"{GetZframeHtmlFilename(zframeFile.zframeId, "", zframeFile.filenamepath)[0..^5]}");
                if (disableOutput)
                {
                    output.DisableOutput();
                }
                if (disableOutput)
                {
                    zframeFile.datareader.OutputWriter = (x) => { output.sw.Write(x); };
                } else
                {
                    zframeFile.datareader.OutputWriter = (x) => {
                        output.sw.Write(x);
                        Console.Write(x);
                    };
                }
                */
            }
            zframeFile.PrintByteAnalysis();
        }



        static void PrintGlslAllFiles()
        {
            // List<string> vcsFiles = GetVcsFiles(DOTA_CORE_PCGL_SOURCE, DOTA_GAME_PCGL_SOURCE, VcsFileType.Any, -1);
            // List<string> vcsFiles = GetVcsFiles(DOTA_CORE_PCGL_SOURCE, DOTA_GAME_PCGL_SOURCE, VcsFileType.Any, 30);
            // List<string> vcsFiles = GetVcsFiles(DOTA_CORE_MOBILE_GLES_SOURCE, DOTA_DAC_MOBILE_GLES_SOURCE, VcsFileType.Any, 30);
            List<string> vcsFiles = GetVcsFiles(DOTA_CORE_MOBILE_GLES_SOURCE, DOTA_DAC_MOBILE_GLES_SOURCE, VcsProgramType.Undetermined, -1);

            foreach (var filenamepath in vcsFiles)
            {
                FileTokens fileTokens = new FileTokens(filenamepath);
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
                    foreach (var glslSource in zframeFile.gpuSources)
                    {
                        string outputFilenamepath = $"{glslServerDir}/{fileTokens.GetGlslHtmlFilename((GlslSource)glslSource)}";
                        WriteBytesToFile(glslSource.sourcebytes, outputFilenamepath, overWrite: false);
                    }
                }
            }
        }



        /*
         * This currently only makes sense for glsl sources (it's the only one where the bytes are the sourcecode)
         *
         */
        static void PrintGlslSingleFiles()
        {
            // string filenamepath = $"{DOTA_CORE_PCGL_SOURCE}/apply_fog_pcgl_40_ps.vcs";
            string filenamepath = $"{DOTA_CORE_PCGL_SOURCE}/bilateral_blur_pcgl_30_ps.vcs";
            // string filenamepath = $"{DOTA_GAME_PCGL_SOURCE}/multiblend_pcgl_30_vs.vcs";
            FileTokens fileTokens = new FileTokens(filenamepath);
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
                foreach (var glslSource in zframeFile.gpuSources)
                {
                    string outputFilenamepath = $"{glslServerDir}/{fileTokens.GetGlslHtmlFilename((GlslSource)glslSource)}";
                    WriteBytesToFile(glslSource.sourcebytes, outputFilenamepath);
                }
            }
        }



        static void WriteBytesToFile(byte[] databytes, string outputFilenamepath, bool writeAsHtml = true, bool overWrite = false)
        {
            if (!overWrite && File.Exists(outputFilenamepath))
            {
                return;
            }
            Console.WriteLine($"writing to {outputFilenamepath}");

            if (writeAsHtml)
            {
                string htmlTitle = Path.GetFileName(outputFilenamepath)[0..^5];
                StreamWriter glslFileWriter = new(outputFilenamepath);
                string htmlHeader = GetHtmlHeader(htmlTitle, htmlTitle);
                glslFileWriter.WriteLine($"{htmlHeader}");
                glslFileWriter.Flush();
                glslFileWriter.BaseStream.Write(databytes, 0, databytes.Length);
                glslFileWriter.Flush();
                glslFileWriter.WriteLine($"{GetHtmlFooter()}");
                glslFileWriter.Flush();
                glslFileWriter.Close();
            } else
            {
                File.WriteAllBytes(outputFilenamepath, databytes);
            }




        }







    }
}