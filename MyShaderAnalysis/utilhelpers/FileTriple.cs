using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using MyShaderAnalysis.vcsparsing;
using System;
using MyShaderAnalysis.compat;
using static MyShaderAnalysis.vcsparsing.UtilHelpers;
using static MyShaderAnalysis.utilhelpers.FileSystem;


namespace MyShaderAnalysis.utilhelpers {


    public class FileTriple {


        public FileTokens ftFile;
        public FileTokens vsFile;
        public FileTokens psFile;


        public FileTriple(ARCHIVE archive, string ftFileName) {
            ftFile = new FileTokens(archive, ftFileName);
            if (ftFile.vcsFiletype != FILETYPE.features_file) {
                throw new System.Exception("not a features file");
            }
            string vsNamepath = $"{ftFile.filenamepath[0..^12]}vs.vcs";
            string psNamepath = $"{ftFile.filenamepath[0..^12]}ps.vcs";
            if (File.Exists(vsNamepath) && File.Exists(psNamepath)) {
                vsFile = new FileTokens(archive, vsNamepath);
                psFile = new FileTokens(archive, psNamepath);
            } else {
                throw new ShaderParserException($"this features file doesn't have associated vs and ps files {ftFile.GetShortHandName()}");
            }
        }


        public FileTriple(ARCHIVE archive, string ftFileName, string psFileName, string vsFileName) {
            ftFile = new FileTokens(archive, ftFileName);
            ftFile = new FileTokens(archive, psFileName);
            ftFile = new FileTokens(archive, vsFileName);
        }



        public static FileTriple GetTripleIfExists(ARCHIVE archive, string ftFilename) {
            ftFilename = Path.GetFileName(ftFilename);
            FileTokens ftTokens = new(archive, ftFilename);
            if (!ftFilename.EndsWith("features.vcs")) {
                throw new System.Exception("not a features file");
            }
            string vsNamepath = $"{ftTokens.filenamepath[0..^12]}vs.vcs";
            string psNamepath = $"{ftTokens.filenamepath[0..^12]}ps.vcs";
            if (File.Exists(vsNamepath) && File.Exists(psNamepath)) {
                return new FileTriple(archive, ftFilename, vsNamepath, psNamepath);
            } else {
                return null;
            }
        }


        public static List<FileTriple> GetFeaturesVsPsFileTriple(string dir1, string dir2, int vcsFileVer = -1) {
            List<FileTriple> fileTriples = new();
            List<string> featuresFiles = GetVcsFiles(dir1, dir2, FILETYPE.features_file, vcsFileVer);
            List<string> validFeaturesFiles = new();
            foreach (string ftFilenamepath in featuresFiles) {
                string vsFile = $"{ftFilenamepath[0..^12]}vs.vcs";
                string psFile = $"{ftFilenamepath[0..^12]}ps.vcs";
                if (File.Exists(vsFile) && File.Exists(psFile)) {
                    validFeaturesFiles.Add(ftFilenamepath);
                }
            }
            validFeaturesFiles.Sort();
            foreach (string ftFilenamepath in validFeaturesFiles) {
                ARCHIVE archive = GetCoreOrDotaString(ftFilenamepath).Equals("core") ? ARCHIVE.dotacore_pcgl : ARCHIVE.dotagame_pcgl;
                fileTriples.Add(new FileTriple(archive, ftFilenamepath));
            }

             return fileTriples;
        }




    }





}





