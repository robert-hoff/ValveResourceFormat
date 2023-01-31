using System;
using System.Globalization;
using System.IO;

using ValveResourceFormat.CompiledShader;
using MyShaderAnalysis.utilhelpers;
using MyShaderAnalysis.vcsanalysis;
using static ValveResourceFormat.CompiledShader.ShaderUtilHelpers;
using System.Diagnostics;

namespace MyShaderAnalysis
{
    public class ParseVcsFile
    {
        private FileVcsTokens fileTokens;
        private ShaderFile shaderFile;
        private bool showRichTextBoxLinks;
        private bool convertLinksToHtml;

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
            var buffer = new StringWriter(CultureInfo.InvariantCulture);
            new PrintVcsFileSummary(shaderFile, buffer.Write, showRichTextBoxLinks, fileTokens.GetRelatedFiles());
            return buffer.ToString();
        }

        public string GetVcsByteSummary()
        {
            // Console.WriteLine($"parsing {shaderFile.filenamepath} byte version");
            var buffer = new StringWriter(CultureInfo.InvariantCulture);
            shaderFile.PrintByteDetail(outputWriter: buffer.Write);
            return buffer.ToString();
        }

        public string GetZframeSummary(ZFrameFile zframeFile)
        {
            var buffer = new StringWriter(CultureInfo.InvariantCulture);
            new PrintZFrameSummary(shaderFile, zframeFile, outputWriter: buffer.Write, showRichTextBoxLinks);
            return buffer.ToString();
        }

        public string GetZframeByteSummary(ZFrameFile zframeFile)
        {
            var buffer = new StringWriter(CultureInfo.InvariantCulture);
            zframeFile.PrintByteDetail(outputWriter: buffer.Write);
            return buffer.ToString();
        }

        public string GetGpuSource(ZFrameFile zframeFile, int gpuSourceId)
        {
            var textBuffer = new StringWriter(CultureInfo.InvariantCulture);

            // Do spirv reflection if applicable
            var gpuSource = zframeFile.gpuSources[gpuSourceId];
            if (gpuSource.sourcebytes.Length > 0 && gpuSource is VulkanSource)
            {
                VulkanSource vulkanSource = (VulkanSource)gpuSource;
                var reflectedSpirv = DecompileSpirvDll.DecompileVulkan(vulkanSource.GetSpirvBytes());
                textBuffer.GetStringBuilder().Clear();
                textBuffer.WriteLine(vulkanSource.GetSourceDetails());
                textBuffer.WriteLine($"// SPIR-V source ({vulkanSource.metadataOffset}), Glsl reflection with SPIRV-Cross, KhronosGroup\n");
                textBuffer.WriteLine(reflectedSpirv);
                textBuffer.WriteLine($"// Source metadata (unknown encoding) ({vulkanSource.metadataLength})");
                textBuffer.WriteLine($"[{vulkanSource.metadataOffset}]");
                textBuffer.WriteLine($"{BytesToString(vulkanSource.GetMetadataBytes())}");
            } else
            {
                zframeFile.PrintGpuSource(gpuSourceId, outputWriter: textBuffer.Write);
            }
            return textBuffer.ToString();
        }


        public string GetGpuByteSource(ZFrameFile zframeFile, int gpuSourceId)
        {
            var gpuSource = zframeFile.gpuSources[gpuSourceId];
            var sourceBytes = gpuSource.sourcebytes;
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
            string htmlTitle = $"{fileTokens.vcstoken}[{zframeFile.zframeId:x}]";
            string htmlHeader = fileTokens.GetZFrameHtmlFilename(zframeFile.zframeId)[..^5];
            string outputFilenamepath = fileTokens.GetZFrameHtmlFilenamepath(zframeFile.zframeId, "summary", createDirs: true);
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
            string htmlTitle = $"{fileTokens.vcstoken}[{zframeFile.zframeId:x}] bytes";
            string htmlHeader = fileTokens.GetZFrameHtmlFilename(zframeFile.zframeId, "bytes")[..^5];
            string outputFilenamepath = fileTokens.GetZFrameHtmlFilenamepath(zframeFile.zframeId, "bytes", createDirs: true);
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
            string outputFilenamepath = $"{gpuServerDir}/{fileTokens.GetGpuHtmlFilename(zframeFile.gpuSources[gpuSourceId].GetEditorRefIdAsString())}";

            GpuSource gpuSource = zframeFile.gpuSources[gpuSourceId];
            string headerText = "";
            if (gpuSource is not VulkanSource)
            {
                headerText += $"Source ref. {gpuSource.GetEditorRefIdAsString()}\n";
            }
            headerText += "Source belongs to ";
            headerText += $"<a href='{fileTokens.GetServerFileUrl("summary2")}'>{fileTokens.filename}</a> ";
            headerText += $"<a href='{fileTokens.GetZFrameUrl(zframeFile.zframeId, "summary")}'>zframe[0x{zframeFile.zframeId:x}]</a>\n";
            string gpuByteSourceFilenamepath = $"{gpuServerDir}/" +
                $"{fileTokens.GetGpuHtmlFilename(gpuSource.GetEditorRefIdAsString() + "-bytes")}";
            if (File.Exists(gpuByteSourceFilenamepath))
            {
                string gpuSourceUrl = $"{fileTokens.GetGpuServerUrl()}/" +
                    $"{fileTokens.GetGpuHtmlFilename(gpuSource.GetEditorRefIdAsString() + "-bytes")}";
                headerText += $"View byte detail <a href='{gpuSourceUrl}'>{gpuSource.GetEditorRefIdAsString()}-bytes<a>\n";
            }
            headerText += $"\n";
            string htmlTitle = $"{fileTokens.vcstoken}[{zframeFile.zframeId:x}]({gpuSourceId})";
            string htmlHeader = $"{fileTokens.archivename} zframe[0x{zframeFile.zframeId:x}] source[{gpuSourceId}]";
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
                $"{fileTokens.GetGpuHtmlFilename(zframeFile.gpuSources[gpuSourceId].GetEditorRefIdAsString()+"-bytes")}";
            string htmlTitle = $"{fileTokens.vcstoken}[{zframeFile.zframeId:x}]({gpuSourceId})";
            string htmlHeader = $"{fileTokens.sourceType}[{gpuSourceId}] {Path.GetFileName(outputFilenamepath)[0..^5]}";
            WriteHtmlFile(outputFilenamepath, htmlTitle, htmlHeader, gpuSourceDetail);
        }

        public void SaveGpuSourcesToHtml(long zframeId, int requestedGpuSourcesToPrint, bool saveGpuByteDetail = false)
        {
            SaveGpuSourcesToHtml(shaderFile.GetZFrameFile(zframeId), requestedGpuSourcesToPrint, saveGpuByteDetail);
        }
        public void SaveGpuSourcesToHtml(ZFrameFile zframeFile, int requestedGpuSourcesToPrint, bool saveGpuByteDetail = false)
        {
            int gpuSourcesToPrint = Math.Min(zframeFile.gpuSourceCount, requestedGpuSourcesToPrint);
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
            fileWriter.WriteText(htmlBody);
            fileWriter.CloseStreamWriter();
            fileWriter.Dispose();
        }
    }
}

