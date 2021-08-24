using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using MyShaderAnalysis.compat;
using MyShaderAnalysis.utilhelpers;
using MyShaderAnalysis.vcsparsing;
using static MyShaderAnalysis.vcsparsing.ShaderUtilHelpers;
using static MyShaderAnalysis.utilhelpers.FileSystem;
using static MyShaderAnalysis.utilhelpers.ReadShaderFile;


namespace MyShaderAnalysis
{
    public class PrintoutsByteAnalysis
    {
        const string OUTPUT_DIR = @"Z:\active\projects\dota2-sourcesdk-modding\shader-analysis-vcs-format\OUTPUT_DUMP";
        const string SERVER_OUTPUT_DIR = @"Z:\dev\www\vcs.codecreation.dev\GEN-output";
        static OutputWriter output = new();

        public static void RunTrials()
        {
            // Trial1();
            // Trial2();
            // Trial3();

            // PrintAllByteAnalysis();

            // PrintZFramesAllFiles();
            // PrintZFramesSingleFile();
            PrintGlslAllFiles();
            // PrintGlslSingleFiles();
            output.CloseStreamWriter();
        }


        static void Trial1()
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
            string filenamepath = $"{DOTA_CORE_PCGL_SOURCE}/generic_light_pcgl_30_vs.vcs";
            // string filenamepath = $"{DOTA_CORE_PC_SOURCE}/generic_light_pc_30_features.vcs";
            // string filenamepath = $"{DOTA_CORE_PC_SOURCE}/generic_light_pc_30_vs.vcs";
            // string filenamepath = $"{DOTA_CORE_PCGL_SOURCE}/generic_light_pcgl_30_ps.vcs";
            // string filenamepath = $"{DOTA_CORE_PCGL_SOURCE}/spritecard_pcgl_30_features.vcs";
            // string filenamepath = $"{DOTA_CORE_PCGL_SOURCE}/spritecard_pcgl_30_ps.vcs";
            // string filenamepath = $"{DOTA_CORE_PCGL_SOURCE}/spritecard_pcgl_30_vs.vcs";
            // string filenamepath = $"{DOTA_CORE_PCGL_SOURCE}/msaa_resolve_cs_pcgl_50_features.vcs"; // strange file that doesn't contain any data

            PrintSingleFileByteAnalysis(filenamepath, $"{SERVER_OUTPUT_DIR}/testfile.html", writeFile: true);
        }


        static void PrintAllByteAnalysis()
        {
            // List<string> vcsFiles = GetVcsFiles(DOTA_CORE_PC_SOURCE, DOTA_GAME_PC_SOURCE, VcsFileType.Any, -1);
            List<string> vcsFiles = GetVcsFiles(DOTA_CORE_PCGL_SOURCE, DOTA_GAME_PCGL_SOURCE, VcsFileType.Any, -1);
            foreach (var filenamepath in vcsFiles)
            {
                FileTokens fileTokens = new FileTokens(filenamepath);
                if (fileTokens.vcsFiletype == VcsFileType.ComputeShader || fileTokens.vcsFiletype == VcsFileType.GeometryShader)
                {
                    continue;
                }
                string outputFilenamepath = $"{fileTokens.GetServerFileDir("bytes", createDirs: true)}";
                PrintSingleFileByteAnalysis(filenamepath, outputFilenamepath, writeFile: true, disableOutput: true);
                output.CloseStreamWriter();
            }
        }




        static void PrintSingleFileByteAnalysis(string filenamepath, string outputFilenamepath = null,
            bool writeFile = false, bool disableOutput = false)
        {
            FileTokens fileTokens = new FileTokens(filenamepath);
            if (outputFilenamepath != null && writeFile)
            {
                output.SetOutputFile(outputFilenamepath);
                output.WriteAsHtml(fileTokens.GetAbbreviatedName(), $"{ShortHandName(filenamepath)}");
                if (disableOutput)
                {
                    output.DisableOutput();
                }
            }
            ShaderFile shaderFile = InstantiateShaderFile(filenamepath);
            shaderFile.datareader.ConfigureWriteToFile(output.sw, disableOutput);

            shaderFile.PrintByteAnalysis();
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



        const int LIMIT_ZFRAME_PRINTOUT = 50;


        static void PrintZFramesAllFiles()
        {
            List<string> vcsFiles = GetVcsFiles(DOTA_CORE_PCGL_SOURCE, DOTA_GAME_PCGL_SOURCE, VcsFileType.Any, -1);
            foreach (var filenamepath in vcsFiles)
            {
                FileTokens fileTokens = new FileTokens(filenamepath);
                string zframesServerDir = fileTokens.GetZFramesServerDir(createDirs: true);

                ShaderFile shaderFile = InstantiateShaderFile(filenamepath);
                int zframesToPrint = Math.Min(shaderFile.GetZFrameCount(), LIMIT_ZFRAME_PRINTOUT);
                for (int i = 0; i < zframesToPrint; i++)
                {
                    ZFrameFile zframeFile = shaderFile.GetZFrameFileByIndex(i);
                    string zframeHtmlFilename = fileTokens.GetZFrameHtmlFilename(zframeFile.zframeId, "bytes");
                    // Debug.WriteLine($"{zframeHtmlFilename}");
                    PrintZFrameByteAnalysis(zframeFile, $"{zframesServerDir}/{zframeHtmlFilename}", writeFile: true, disableOutput: true);
                    output.CloseStreamWriter();
                }
            }
        }


        static void PrintZFramesSingleFile()
        {
            // string filenamepath = $"{DOTA_CORE_PCGL_SOURCE}/apply_fog_pcgl_40_ps.vcs";
            string filenamepath = $"{DOTA_CORE_PCGL_SOURCE}/bilateral_blur_pcgl_30_ps.vcs";
            // string filenamepath = $"{DOTA_GAME_PCGL_SOURCE}/multiblend_pcgl_30_vs.vcs";
            FileTokens fileTokens = new FileTokens(filenamepath);
            string zframesServerDir = fileTokens.GetZFramesServerDir(createDirs: true);

            ShaderFile shaderFile = InstantiateShaderFile(filenamepath);
            int zframesToPrint = Math.Min(shaderFile.GetZFrameCount(), LIMIT_ZFRAME_PRINTOUT);
            for (int i = 0; i < zframesToPrint; i++)
            {
                ZFrameFile zframeFile = shaderFile.GetZFrameFileByIndex(i);
                string zframeHtmlFilename = fileTokens.GetZFrameHtmlFilename(zframeFile.zframeId, "bytes");
                // Debug.WriteLine($"{zframeHtmlFilename}");
                PrintZFrameByteAnalysis(zframeFile, $"{zframesServerDir}/{zframeHtmlFilename}", writeFile: true, disableOutput: true);
                output.CloseStreamWriter();
            }
        }




        static void PrintZFrameByteAnalysis(ZFrameFile zframeFile, string outputFilenamepath = null,
            bool writeFile = false, bool disableOutput = false)
        {
            FileTokens fileTokens = new FileTokens(zframeFile.filenamepath);
            if (outputFilenamepath != null && writeFile)
            {
                output.SetOutputFile(outputFilenamepath);
                output.WriteAsHtml($"{fileTokens.namelabel}-Z[0x{zframeFile.zframeId:x}]",
                    $"{GetZframeHtmlFilename(zframeFile.zframeId, "", zframeFile.filenamepath)[0..^5]}");
                if (disableOutput)
                {
                    output.DisableOutput();
                }
            }
            zframeFile.datareader.ConfigureWriteToFile(output.sw, disableOutput);
            zframeFile.PrintByteAnalysis();
        }




        static void PrintGlslAllFiles()
        {
            // List<string> vcsFiles = GetVcsFiles(DOTA_CORE_PCGL_SOURCE, DOTA_GAME_PCGL_SOURCE, VcsFileType.Any, -1);
            List<string> vcsFiles = GetVcsFiles(DOTA_CORE_PCGL_SOURCE, DOTA_GAME_PCGL_SOURCE, VcsFileType.Any, 30);

            foreach (var filenamepath in vcsFiles)
            {
                FileTokens fileTokens = new FileTokens(filenamepath);
                if (!fileTokens.sourcetype.Equals("glsl"))
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
            if (!fileTokens.sourcetype.Equals("glsl"))
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
            Debug.WriteLine($"writing to {outputFilenamepath}");

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