using System;
using System.Collections.Generic;
using System.IO;
using ValveResourceFormat.CompiledShader;
using MyShaderAnalysis.utilhelpers;
using MyShaderAnalysis.utilhelpers.snippetcode;
using static MyShaderAnalysis.utilhelpers.FileArchives;
using static MyShaderAnalysis.utilhelpers.MyShaderUtilHelpers;
using static MyShaderAnalysis.utilhelpers.ReadShaderFile;
using static ValveResourceFormat.CompiledShader.ShaderUtilHelpers;

namespace MyShaderAnalysis.utilhelpers.parsetrials
{
    class SingleFileParserExamples
    {

        public static void RunTrials()
        {

            DecompileVulkanSource();

            // ParseV44FileIntoHtml();
            // ParseV44FileToConsole();

            // V62ExampleFiles();
            // ParseV62File();
            // ManualZFrameDecompression();
            // ShowVcsFilesForGivenArchive();
            // PrintZframeToHtml();
            // ShowFilenamePath();


            // WriteBytesToStringBuffer();
        }



        static void DecompileVulkanSource()
        {
            byte[] databytes = File.ReadAllBytes(@"../../../files_samples/source0.spv");
            // byte[] databytes = File.ReadAllBytes(@"../../../files_samples/source1.spv");
            string sourceRefection = DecompileSpirvDll.DecompileVulkan(databytes);
            Console.WriteLine($"{sourceRefection}");
        }


        static void ParseV44FileIntoHtml()
        {
            string outputFile = "Z:/dev/www/vcs.codecreation.dev/GEN-output/testfile.html";
            FileWriter writer = new FileWriter(outputFile);

            string inputFile = GetFilenamepath(ARCHIVE.dota_core_pc_v65, "debugoverlay_wireframe_pc_40_gs.vcs");

            writer.WriteHtmlHeader("title", "header");
            new DataReaderVcsBytes(inputFile, outputWriter: writer.GetOutputWriter());
            writer.CloseStreamWriter();
        }


        static void ParseV44FileToConsole()
        {
            string inputFile = GetFilenamepath(ARCHIVE.dota_core_pc_v65, "debugoverlay_wireframe_pc_40_gs.vcs");
            new DataReaderVcsBytes(inputFile).PrintByteDetail();
        }

        static void V62ParseZFrameBytes2()
        {
            // string filenamepath = GetFilenamepath(ARCHIVE.the_lab_pc_v62, "downsample_pc_40_features.vcs");
            // string filenamepath = GetFilenamepath(ARCHIVE.the_lab_pc_v62, "downsample_pc_40_ps.vcs");
            string filenamepath = GetFilenamepath(ARCHIVE.the_lab_pc_v62, "aerial_perspective_pc_30_features.vcs");
            ShaderFile shaderFile = InstantiateShaderFile(filenamepath);
            // byte[] zframeDecomp = shaderFile.GetDecompressedZFrame(0);
            ZFrameFile zframe = shaderFile.GetZFrameFile(0);
            zframe.PrintByteDetail();
        }


        static void V62ParseZFrameBytes1()
        {

        }


        static void V62InstantiateShaderFile()
        {
            string filenamepath = GetFilenamepath(ARCHIVE.the_lab_pc_v62, "downsample_pc_40_features.vcs");
            // string filenamepath = GetFilenamepath(ARCHIVE.the_lab_pc_v62, "downsample_pc_40_ps.vcs");
            // string filenamepath = GetFilenamepath(ARCHIVE.the_lab_pc_v62, "aerial_perspective_pc_30_features.vcs");
            ShaderFile shaderFile = InstantiateShaderFile(filenamepath);
            shaderFile.PrintByteDetail();
        }


        static void V62ExampleFiles()
        {
            // string filenamepath = GetFilenamepath(ARCHIVE.the_lab_pc_v62, "downsample_pc_40_features.vcs");
            // string filenamepath = GetFilenamepath(ARCHIVE.the_lab_pc_v62, "downsample_pc_40_ps.vcs");
            string filenamepath = GetFilenamepath(ARCHIVE.the_lab_pc_v62, "aerial_perspective_pc_30_features.vcs");
            new ParseV62Files(filenamepath);
        }


        // two ways to print the byte analysis (keep these both functionally identical)
        static void ParseV62File()
        {
            string filenamepath = GetFilenamepath(ARCHIVE.the_lab_pc_v62, "bilateral_blur_pc_30_ps.vcs");
            // -- original approach
            new DataReaderVcsBytes(filenamepath).PrintByteDetail();
            // -- production
            ShaderFile shaderFile = InstantiateShaderFile(filenamepath);
            shaderFile.PrintByteDetail();
        }


        static void ManualZFrameDecompression()
        {
            // string filenamepath = $"{DOTA_GAME_PC_SOURCE_V65}/crystal_pc_40_vs.vcs";
            string filenamepath = GetFilenamepath(ARCHIVE.dota_game_pc_v65, "crystal_pc_40_vs.vcs");
            ShaderFile shaderFile = InstantiateShaderFile(filenamepath);

            byte[] result = shaderFile.GetCompressedZFrameData(0);

            // Console.WriteLine($"{shaderFile.GetZFrameCount()}");
            // Console.WriteLine($"{result.Length}");
            // Console.WriteLine(BytesToString(result));

            byte[] uncompressedData = DecompressZstdFrame(result);
            Console.WriteLine(BytesToString(uncompressedData));

            // Console.WriteLine(uncompressedData.Length);
        }


        static byte[] DecompressZstdFrame(byte[] compressedZstdFrame)
        {
            byte[] zstdDict = ZstdDictionary.GetDictionary();
            var zstdDecoder = new ZstdSharp.Decompressor();
            zstdDecoder.LoadDictionary(zstdDict);
            byte[] decompressedData = zstdDecoder.Unwrap(compressedZstdFrame).ToArray();
            return decompressedData;
        }


        static void ShowVcsFilesForGivenArchive()
        {

            // gets all files ending with 40_vs.vcs from the given directory
            List<string> vcsFiles = GetVcsFiles(GetArchiveDir(ARCHIVE.dota_game_pcgl_v64), null, VcsProgramType.VertexShader, 40);

            foreach (var filenamepath in vcsFiles)
            {
                Console.WriteLine(filenamepath);
            }
        }


        /*
         * A very useful approach in general for working on parsing and printing layouts
         * For example, while investigating parsing, print the datareader output into testfile.html
         *
         */
        static void PrintZFrameToHtml()
        {
            string outputFile = "Z:/dev/www/vcs.codecreation.dev/GEN-output/testfile.html";
            Console.WriteLine($"writing {outputFile}");

            StreamWriter sw = new StreamWriter(outputFile);
            sw.WriteLine(GetHtmlHeader("Zframe testing", "Zframe testing"));
            string vcsTestFile = GetFilenamepath(ARCHIVE.alyx_hlvr_vulkan_v64, "solidcolor_vulkan_50_vs.vcs");
            ShaderFile shaderFile = InstantiateShaderFile(vcsTestFile);
            ZFrameFile zframeFile = shaderFile.GetZFrameFileByIndex(0);
            new PrintZFrameSummary(shaderFile, zframeFile, (x) => { sw.Write(x); }, showRichTextBoxLinks: true);
            sw.WriteLine(GetHtmlFooter());
            sw.Close();
        }


        /*
         * This records everything up to where the exception is thrown
         *
         */
        static void WriteBytesToStringBuffer()
        {
            string filenamepath = GetFilenamepath(ARCHIVE.dota_game_pcgl_v64, "multiblend_pcgl_30_vs.vcs");
            var buffer = new StringWriter();

            ShaderDataReader datareader = new ShaderDataReader(filenamepath, buffer.Write);
            datareader.ShowByteCount();
            datareader.ShowBytes(200);
            try
            {
                datareader.ShowEndOfFile();
            } catch (Exception) { }

            Console.WriteLine($"{buffer}");
        }




        static void ShowFilenamePath()
        {
            string filenamepath = GetFilenamepath(ARCHIVE.dota_game_pcgl_v64, "multiblend_pcgl_30_vs.vcs");
            Console.WriteLine($"{filenamepath}");
        }



        private static string GetHtmlHeader(string browserTitle, string pageHeader)
        {
            string html_header = "" +
                $"<!DOCTYPE html>\n<html>\n<head>\n  <title>{browserTitle}</title>\n" +
                $"  <link href='/includes/styles.css' rel='stylesheet' type='text/css' />\n" +
                $"</head>\n<body>\n<b>{pageHeader}</b>\n<pre>";
            return html_header;
        }
        private static string GetHtmlFooter()
        {
            return "</pre>\n</html>";
        }


    }
}


