using System.Collections.Generic;
using System.IO;
using MyShaderFile.CompiledShader;
using static MyShaderAnalysis.codestash.FileSystemOld;
using static MyShaderAnalysis.filearchive.FileArchive;
using static MyShaderFile.CompiledShader.ShaderUtilHelpers;

#pragma warning disable IDE1006 // Naming Styles
namespace MyShaderAnalysis.codestash
{
    public class FileTriple
    {
        public FileTokensOld ftFile { get; }
        public FileTokensOld vsFile { get; }
        public FileTokensOld psFile { get; }

        public FileTriple(ARCHIVE_OLD archive, string ftFileName)
        {
            ftFile = new FileTokensOld(archive, ftFileName);
            if (ftFile.vcsProgramType != VcsProgramType.Features)
            {
                throw new System.Exception("not a features file");
            }
            string vsNamepath = $"{ftFile.filenamepath[0..^12]}vs.vcs";
            string psNamepath = $"{ftFile.filenamepath[0..^12]}ps.vcs";
            if (File.Exists(vsNamepath) && File.Exists(psNamepath))
            {
                vsFile = new FileTokensOld(archive, vsNamepath);
                psFile = new FileTokensOld(archive, psNamepath);
            }
            else
            {
                throw new ShaderParserException($"this features file doesn't have associated vs and ps files {ftFile.GetShortHandName()}");
            }
        }

        public FileTriple(ARCHIVE_OLD archive, string ftFileName, string vsFileName, string psFileName)
        {
            ftFile = new FileTokensOld(archive, ftFileName);
            vsFile = new FileTokensOld(archive, vsFileName);
            psFile = new FileTokensOld(archive, psFileName);
        }

        public static FileTriple GetTripleIfExists(ARCHIVE_OLD archive, string ftFilename)
        {
            ftFilename = Path.GetFileName(ftFilename);
            FileTokensOld ftTokens = new(archive, ftFilename);
            if (!ftFilename.EndsWith("features.vcs"))
            {
                throw new System.Exception("not a features file");
            }
            string vsNamepath = $"{ftTokens.filenamepath[0..^12]}vs.vcs";
            string psNamepath = $"{ftTokens.filenamepath[0..^12]}ps.vcs";
            if (File.Exists(vsNamepath) && File.Exists(psNamepath))
            {
                return new FileTriple(archive, ftFilename, vsNamepath, psNamepath);
            }
            else
            {
                return null;
            }
        }

        public static List<FileTriple> GetFeaturesVsPsFileTriple(string dir1, string dir2, int vcsFileVer = -1)
        {
            List<FileTriple> fileTriples = new();
            List<string> featuresFiles = GetVcsFiles(dir1, dir2, VcsProgramType.Features, vcsFileVer);
            List<string> validFeaturesFiles = new();
            foreach (string ftFilenamepath in featuresFiles)
            {
                string vsFile = $"{ftFilenamepath[0..^12]}vs.vcs";
                string psFile = $"{ftFilenamepath[0..^12]}ps.vcs";
                if (File.Exists(vsFile) && File.Exists(psFile))
                {
                    validFeaturesFiles.Add(ftFilenamepath);
                }
            }
            validFeaturesFiles.Sort();
            foreach (string ftFilenamepath in validFeaturesFiles)
            {
                // ARCHIVE_OLD archive = GetCoreOrDotaString(ftFilenamepath).Equals("core") ? ARCHIVE.dotacore_pcgl : ARCHIVE.dotagame_pcgl;
                ARCHIVE_OLD archive = DetermineArchiveType(ftFilenamepath);
                fileTriples.Add(new FileTriple(archive, ftFilenamepath));
            }
            return fileTriples;
        }

        // untested, wrote it in to reduce a vcsFiles collection to only the features files
        public static List<FileTriple> GetFeaturesVsPsFileTriple(List<string> vcsFiles)
        {
            List<FileTriple> fileTriples = new();
            List<string> featuresFiles = new();
            foreach (string vcsFile in vcsFiles)
            {
                if (ComputeVcsProgramType(vcsFile) == VcsProgramType.Features)
                {
                    featuresFiles.Add(vcsFile);
                }
            }
            List<string> validFeaturesFiles = new();
            foreach (string ftFilenamepath in featuresFiles)
            {
                string vsFile = $"{ftFilenamepath[0..^12]}vs.vcs";
                string psFile = $"{ftFilenamepath[0..^12]}ps.vcs";
                if (File.Exists(vsFile) && File.Exists(psFile))
                {
                    validFeaturesFiles.Add(ftFilenamepath);
                }
            }
            validFeaturesFiles.Sort();
            foreach (string ftFilenamepath in validFeaturesFiles)
            {
                // ARCHIVE archive = GetCoreOrDotaString(ftFilenamepath).Equals("core") ? ARCHIVE.dotacore_pcgl : ARCHIVE.dotagame_pcgl;
                ARCHIVE_OLD archive = DetermineArchiveType(ftFilenamepath);
                fileTriples.Add(new FileTriple(archive, ftFilenamepath));
            }
            return fileTriples;
        }
    }
}


