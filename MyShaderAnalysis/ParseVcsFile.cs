using MyShaderAnalysis.utilhelpers;
using MyShaderAnalysis.vcsanalysis;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ValveResourceFormat.CompiledShader;
using static MyShaderAnalysis.utilhelpers.FileArchives;


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
            shaderFile.PrintByteAnalysis(outputWriter: buffer.Write);
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
            zframeFile.PrintByteAnalysis(outputWriter: buffer.Write);
            return buffer.ToString();
        }

        public string GetGpuSource(ZFrameFile zframeFile, int gpuSourceId)
        {
            var buffer = new StringWriter(CultureInfo.InvariantCulture);
            zframeFile.PrintGpuSource(gpuSourceId, outputWriter: buffer.Write);
            return buffer.ToString();
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
            string glslServerDir = fileTokens.GetGpuServerDir(createDirs: true);
            string outputFilenamepath = $"{glslServerDir}/{fileTokens.GetGpuHtmlFilename(zframeFile.gpuSources[gpuSourceId])}";
            string htmlTitle = $"{fileTokens.vcstoken}[{zframeFile.zframeId:x}]({gpuSourceId})";
            string htmlHeader = $"{fileTokens.sourceType}[{gpuSourceId}] {Path.GetFileName(outputFilenamepath)[0..^5]}";
            WriteHtmlFile(outputFilenamepath, htmlTitle, htmlHeader, gpuSourceDetail);
        }

        public void SaveGpuSources(long zframeId, int requestedGpuSourcesToPrint)
        {
            SaveGpuSources(shaderFile.GetZFrameFile(zframeId), requestedGpuSourcesToPrint);
        }

        public void SaveGpuSources(ZFrameFile zframeFile, int requestedGpuSourcesToPrint)
        {
            int gpuSourcesToPrint = Math.Min(zframeFile.gpuSourceCount, requestedGpuSourcesToPrint);
            for (int i = 0; i < gpuSourcesToPrint; i++)
            {
                SaveGpuSourceToHtml(zframeFile, i);
            }
        }



        public void SaveAllServerFiles(int requestedZframesToPrint, int requestedGpuSourcesToPrint)
        {
            int zframesToPrint = Math.Min(shaderFile.GetZFrameCount(), requestedZframesToPrint);
            for (int i = 0; i < zframesToPrint; i++)
            {
                ZFrameFile zframeFile = shaderFile.GetZFrameFileByIndex(i);
                SaveGpuSources(zframeFile, requestedGpuSourcesToPrint);
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
            fileWriter.WriteData(htmlBody);
            fileWriter.CloseStreamWriter();
            fileWriter.Dispose();
        }


    }
}

