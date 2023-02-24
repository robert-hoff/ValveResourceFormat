using System;
using System.Globalization;
using System.IO;
using MyShaderAnalysis.filearchive;
using MyShaderAnalysis.postprocessing;
using MyShaderAnalysis.util;
using MyShaderAnalysis.vulkanreflect;
using MyShaderFile.CompiledShader;
using static MyShaderFile.CompiledShader.ShaderUtilHelpers;

#pragma warning disable IDE0090 // Use 'new(...)'
namespace MyShaderAnalysis.serverhtml
{
    public class ParseVcsFile
    {
        private readonly FileVcsTokens fileTokens;
        private readonly ShaderFile shaderFile;
        private readonly bool showRichTextBoxLinks;
        private readonly bool convertLinksToHtml;

        // public int zframesToPrint { get; set; } = 0;

        public ParseVcsFile(ARCHIVE archive, string filename, bool showRichTextBoxLinks = true, bool convertLinksToHtml = true)
        {
            fileTokens = new FileVcsTokens(archive, filename);
            shaderFile = fileTokens.GetShaderFile();
            this.showRichTextBoxLinks = showRichTextBoxLinks;
            this.convertLinksToHtml = convertLinksToHtml;
        }

        public string GetVcsSummary()
        {
            StringWriter buffer = new StringWriter(CultureInfo.InvariantCulture);
            new PrintVcsFileSummary(shaderFile, buffer.Write, showRichTextBoxLinks, fileTokens.GetRelatedFiles());
            return buffer.ToString();
        }

        public string GetVcsByteSummary()
        {
            // Console.WriteLine($"parsing {shaderFile.filenamepath} byte version");
            StringWriter buffer = new StringWriter(CultureInfo.InvariantCulture);
            shaderFile.PrintByteDetail(outputWriter: buffer.Write);
            return buffer.ToString();
        }

        public string GetZframeSummary(ZFrameFile zframeFile)
        {
            StringWriter buffer = new StringWriter(CultureInfo.InvariantCulture);
            new PrintZFrameSummary(shaderFile, zframeFile, outputWriter: buffer.Write, showRichTextBoxLinks);
            return buffer.ToString();
        }

        public static string GetZframeByteSummary(ZFrameFile zframeFile)
        {
            StringWriter buffer = new StringWriter(CultureInfo.InvariantCulture);
            zframeFile.PrintByteDetail(outputWriter: buffer.Write);
            return buffer.ToString();
        }

        public static string GetGpuSource(ZFrameFile zframeFile, int gpuSourceId)
        {
            StringWriter textBuffer = new StringWriter(CultureInfo.InvariantCulture);

            // Do spirv reflection if applicable
            GpuSource gpuSource = zframeFile.GpuSources[gpuSourceId];
            if (gpuSource.Sourcebytes.Length > 0 && gpuSource is VulkanSource vulkanSource)
            {
                string reflectedSpirv = DecompileSpirvDll.DecompileVulkan(vulkanSource.GetSpirvBytes());
                textBuffer.GetStringBuilder().Clear();
                textBuffer.WriteLine(vulkanSource.GetSourceDetails());
                textBuffer.WriteLine($"// SPIR-V source ({vulkanSource.MetadataOffset}), Glsl reflection with SPIRV-Cross, KhronosGroup\n");
                textBuffer.WriteLine(reflectedSpirv);
                textBuffer.WriteLine($"// Source metadata (unknown encoding) ({vulkanSource.MetadataLength})");
                textBuffer.WriteLine($"[{vulkanSource.MetadataOffset}]");
                textBuffer.WriteLine($"{BytesToString(vulkanSource.GetMetadataBytes())}");
            }
            else
            {
                zframeFile.PrintGpuSource(gpuSourceId, outputWriter: textBuffer.Write);
            }
            return textBuffer.ToString();
        }

        public static string GetGpuByteSource(ZFrameFile zframeFile, int gpuSourceId)
        {
            GpuSource gpuSource = zframeFile.GpuSources[gpuSourceId];
            byte[] sourceBytes = gpuSource.Sourcebytes;
            string byteRepresentation = BytesToString(sourceBytes);
            return byteRepresentation;
        }

        public void SaveVcsSummaryToHtml()
        {
            string vcsFormattedSummary = GetVcsSummary();
            if (convertLinksToHtml)
            {
                vcsFormattedSummary = new PostProcessVcsFile(fileTokens).PostProcessVcsData(vcsFormattedSummary);
            }
            string htmlTitle = fileTokens.GetShortName();
            string htmlHeader = fileTokens.GetBaseName();
            string outputFilenamepath = $"{fileTokens.GetServerFilenamepath("summary2", createDirs: true)}";
            WriteHtmlFile(outputFilenamepath, htmlTitle, htmlHeader, vcsFormattedSummary);
        }

        public void SaveVcsByteSummaryToHtml()
        {
            string vcsBytesDetail = GetVcsByteSummary();
            string htmlTitle = fileTokens.GetShortName();
            string htmlHeader = $"{fileTokens.GetBaseName()} (byte detail)";
            string outputFilenamepath = $"{fileTokens.GetServerFilenamepath("bytes", createDirs: true)}";
            WriteHtmlFile(outputFilenamepath, htmlTitle, htmlHeader, vcsBytesDetail);
        }

        // may throw 'KeyNotFoundException'
        public void SaveZframeSummaryToHtml(int zframeId)
        {
            SaveZframeSummaryToHtml(shaderFile.GetZFrameFile(zframeId));
        }

        public void SaveZframeSummaryToHtml(ZFrameFile zframeFile)
        {
            string zframeFormattedSummary = GetZframeSummary(zframeFile);
            if (convertLinksToHtml)
            {
                zframeFormattedSummary = new PostProcessZframeFile(zframeFile, fileTokens).PostProcessZframeData(zframeFormattedSummary);
            }
            string htmlTitle = $"{fileTokens.vcstoken}[{zframeFile.ZframeId:x}]";
            string htmlHeader = fileTokens.GetZFrameHtmlFilename(zframeFile.ZframeId)[..^5];
            string outputFilenamepath = fileTokens.GetZFrameHtmlFilenamepath(zframeFile.ZframeId, "summary", createDirs: true);
            WriteHtmlFile(outputFilenamepath, htmlTitle, htmlHeader, zframeFormattedSummary);
        }

        public void SaveZframeSummaries(int requestedZframesToPrint)
        {
            int zframesToPrint = Math.Min(shaderFile.GetZFrameCount(), requestedZframesToPrint);
            for (int i = 0; i < zframesToPrint; i++)
            {
                ZFrameFile zframeFile = shaderFile.GetZFrameFileByIndex(i);
                SaveZframeSummaryToHtml(zframeFile);
            }
        }

        public void SaveZframeByteSummaryToHtml(long zframeId)
        {
            SaveZframeByteSummaryToHtml(shaderFile.GetZFrameFile(zframeId));
        }

        public void SaveZframeByteSummaryToHtml(ZFrameFile zframeFile)
        {
            string zframeByteDetail = GetZframeByteSummary(zframeFile);
            string htmlTitle = $"{fileTokens.vcstoken}[{zframeFile.ZframeId:x}] bytes";
            string htmlHeader = fileTokens.GetZFrameHtmlFilename(zframeFile.ZframeId, "bytes")[..^5];
            string outputFilenamepath = fileTokens.GetZFrameHtmlFilenamepath(zframeFile.ZframeId, "bytes", createDirs: true);
            WriteHtmlFile(outputFilenamepath, htmlTitle, htmlHeader, zframeByteDetail);
        }

        public void SaveZframeByteSummaries(int requestedZframesToPrint)
        {
            int zframesToPrint = Math.Min(shaderFile.GetZFrameCount(), requestedZframesToPrint);
            for (int i = 0; i < zframesToPrint; i++)
            {
                ZFrameFile zframeFile = shaderFile.GetZFrameFileByIndex(i);
                SaveZframeByteSummaryToHtml(zframeFile);
            }
        }

        public void SaveGpuSourceToHtml(long zframeId, int gpuSourceId)
        {
            SaveGpuSourceToHtml(shaderFile.GetZFrameFile(zframeId), gpuSourceId);
        }

        public void SaveGpuSourceToHtml(ZFrameFile zframeFile, int gpuSourceId)
        {
            string gpuSourceDetail = GetGpuSource(zframeFile, gpuSourceId);
            string gpuServerDir = fileTokens.GetGpuServerDir(createDirs: true);
            string outputFilenamepath = $"{gpuServerDir}/{fileTokens.GetGpuHtmlFilename(zframeFile.GpuSources[gpuSourceId].GetEditorRefIdAsString())}";

            GpuSource gpuSource = zframeFile.GpuSources[gpuSourceId];
            string headerText = "";
            if (gpuSource is not VulkanSource)
            {
                headerText += $"Source ref. {gpuSource.GetEditorRefIdAsString()}\n";
            }
            headerText += "Source belongs to ";
            headerText += $"<a href='{fileTokens.GetServerFileUrl("summary2")}'>{fileTokens.filename}</a> ";
            headerText += $"<a href='{fileTokens.GetZFrameUrl(zframeFile.ZframeId, "summary")}'>zframe[0x{zframeFile.ZframeId:x}]</a>\n";
            string gpuByteSourceFilenamepath = $"{gpuServerDir}/" +
                $"{fileTokens.GetGpuHtmlFilename(gpuSource.GetEditorRefIdAsString() + "-bytes")}";
            if (File.Exists(gpuByteSourceFilenamepath))
            {
                string gpuSourceUrl = $"{fileTokens.GetGpuServerUrl()}/" +
                    $"{fileTokens.GetGpuHtmlFilename(gpuSource.GetEditorRefIdAsString() + "-bytes")}";
                headerText += $"View byte detail <a href='{gpuSourceUrl}'>{gpuSource.GetEditorRefIdAsString()}-bytes<a>\n";
            }
            headerText += $"\n";
            string htmlTitle = $"{fileTokens.vcstoken}[{zframeFile.ZframeId:x}]({gpuSourceId})";
            string htmlHeader = $"{fileTokens.filename} zframe[0x{zframeFile.ZframeId:x}] source[{gpuSourceId}]";
            WriteHtmlFile(outputFilenamepath, htmlTitle, htmlHeader, $"{headerText}{gpuSourceDetail}");
        }

        public void SaveGpuByteSourceToHtml(long zframeId, int gpuSourceId)
        {
            SaveGpuByteSourceToHtml(shaderFile.GetZFrameFile(zframeId), gpuSourceId);
        }
        public void SaveGpuByteSourceToHtml(ZFrameFile zframeFile, int gpuSourceId)
        {
            StringWriter textBuffer = new();
            zframeFile.PrintGpuSource(gpuSourceId, outputWriter: textBuffer.Write);
            string gpuSourceDetail = textBuffer.ToString();
            string gpuServerDir = fileTokens.GetGpuServerDir(createDirs: true);
            string outputFilenamepath = $"{gpuServerDir}/" +
                $"{fileTokens.GetGpuHtmlFilename(zframeFile.GpuSources[gpuSourceId].GetEditorRefIdAsString() + "-bytes")}";
            string htmlTitle = $"{fileTokens.filename}[{zframeFile.ZframeId:x}]({gpuSourceId})";
            string htmlHeader = $"{fileTokens.sourceType}[{gpuSourceId}] {Path.GetFileName(outputFilenamepath)[0..^5]}";
            WriteHtmlFile(outputFilenamepath, htmlTitle, htmlHeader, gpuSourceDetail);

            // -- if I want toprint the source files (in this example DXBC
            // string filename = outputFilenamepath[0..^11];
            // File.WriteAllBytes($"{filename}.dx", zframeFile.gpuSources[gpuSourceId].sourcebytes);
        }

        public void SaveGpuSourcesToHtml(long zframeId, int requestedGpuSourcesToPrint, bool saveGpuByteDetail = false)
        {
            SaveGpuSourcesToHtml(shaderFile.GetZFrameFile(zframeId), requestedGpuSourcesToPrint, saveGpuByteDetail);
        }
        public void SaveGpuSourcesToHtml(ZFrameFile zframeFile, int requestedGpuSourcesToPrint, bool saveGpuByteDetail = false)
        {
            int gpuSourcesToPrint = Math.Min(zframeFile.GpuSourceCount, requestedGpuSourcesToPrint);
            for (int i = 0; i < gpuSourcesToPrint; i++)
            {
                if (saveGpuByteDetail)
                {
                    SaveGpuByteSourceToHtml(zframeFile, i);
                }
                SaveGpuSourceToHtml(zframeFile, i);
            }
        }
        public void SaveAllServerFiles(int requestedZframesToPrint, int requestedGpuSourcesToPrint, bool saveGpuByteDetail = false)
        {
            int zframesToPrint = Math.Min(shaderFile.GetZFrameCount(), requestedZframesToPrint);
            for (int i = 0; i < zframesToPrint; i++)
            {
                ZFrameFile zframeFile = shaderFile.GetZFrameFileByIndex(i);
                SaveGpuSourcesToHtml(zframeFile, requestedGpuSourcesToPrint, saveGpuByteDetail);
                SaveZframeByteSummaryToHtml(zframeFile);
                SaveZframeSummaryToHtml(zframeFile);
            }
            SaveVcsByteSummaryToHtml();
            SaveVcsSummaryToHtml();
        }

        public static void WriteHtmlFile(string outputFilenamepath, string htmlTitle, string htmlHeader, string htmlBody)
        {
            FileWriter fileWriter = new FileWriter(outputFilenamepath, showOutputToConsole: false);
            fileWriter.WriteHtmlHeader(htmlTitle, htmlHeader);
            fileWriter.WriteLine(htmlBody);
            fileWriter.CloseStreamWriter();
            fileWriter.Dispose();
        }
    }
}

