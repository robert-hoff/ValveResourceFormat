using MyShaderAnalysis.filearchive;
using MyShaderAnalysis.util;
using MyShaderAnalysis.vulkanreflect;
using System;
using System.Collections.Generic;
using System.IO;
using MyShaderFile.CompiledShader;
using static MyShaderAnalysis.codestash.FileSystemOld;
using static MyShaderAnalysis.filearchive.FileArchive;
using static MyShaderAnalysis.filearchive.ReadShaderFile;
using static MyShaderFile.CompiledShader.ShaderUtilHelpers;
using System.Diagnostics;

/*
 * SINGLE FILE PARSING ALSO IN
 * batchtesting.TestSingleFileParsing
 *
 */
namespace MyShaderAnalysis.parsing
{
    class SingleFileParserExamples
    {
        public static void RunTrials()
        {

            // ParseAndPrintFile(ARCHIVE.dota_game_vulkan_v66, "cables_vulkan_40_features.vcs");
            // ParseAndPrintFile(ARCHIVE.dota_core_vulkan_v66, "rtx_binlights_vulkan_60_features.vcs");
            // ParseAndPrintFile(ARCHIVE.dota_core_vulkan_v66, "rtx_binlights_vulkan_60_rtx.vcs");
            // ParseAndPrintFile(ARCHIVE.dota_core_vulkan_v66, "aoproxy_splat_vulkan_50_ps.vcs");

            // ParseAndPrintFile(ARCHIVE.dota_core_vulkan_v66, "aoproxy_splat_vulkan_50_ps.vcs");
            ParseAndPrintZFile(ARCHIVE.dota_core_vulkan_v66, "aoproxy_splat_vulkan_50_ps.vcs", zFrameId: 0);

            // ParseV66Files.RunTrials();
            // TestShaderFilesBytesShowOutput();

            // -- earlier (2022)
            // DecompileVulkanSource();
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

        public static void TestShaderFilesBytesShowOutput()
        {
            FileArchive vcsArchive = new FileArchive(ARCHIVE.dota_testset_pcgl_v64, VcsProgramType.VertexShader, VcsShaderModelType._30);
            foreach (FileVcsTokens vcsTokens in vcsArchive.GetFileVcsTokens())
            {
                new DataReaderVcsBytes(vcsTokens.filenamepath).PrintByteDetail();
            }
        }

        // -- earlier (2022)

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

            ShaderDataReader datareader = new ShaderDataReader(new MemoryStream(File.ReadAllBytes(filenamepath)), buffer.Write);
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



        public static void ParseAndPrintZFile(ARCHIVE archive, string filename, int zFrameId)
        {
            FileVcsTokens vcsTokens = new(archive, filename);
            ShaderFile shaderFile = vcsTokens.GetShaderFile();
            ZFrameFile zFile = shaderFile.GetZFrameFile(zFrameId);
            // zFile.PrintByteDetail();
            byte[] zframeBytes = shaderFile.GetDecompressedZFrame(zFrameId);
            DataReaderZFrameBytes reader = new DataReaderZFrameBytes(zframeBytes, shaderFile.VcsProgramType,
                shaderFile.VcsPlatformType, shaderFile.VcsShaderModelType, shaderFile.VcsVersion);
            reader.PrintByteDetail();
        }

        public static void ParseAndPrintFile(ARCHIVE archive, string filename)
        {
            FileVcsTokens vcsTokens = new(archive, filename);
            ShaderFile shaderFile = vcsTokens.GetShaderFile();
            new PrintVcsFileSummary(shaderFile);
        }


    }
}

