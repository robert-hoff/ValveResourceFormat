using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;


namespace MyShaderAnalysis.vcsparsing {

    public class UtilHelpers {

        private static byte[] zstdDictionary = null;
        public static byte[] GetZFrameDictionary() {
            if (zstdDictionary == null) {
                zstdDictionary = File.ReadAllBytes(@"..\..\zstdictionary_2bc2fa87.dat");
            }
            return zstdDictionary;
        }

        public static FILETYPE GetVcsFileType(string filenamepath) {
            if (filenamepath.EndsWith("features.vcs")) {
                return FILETYPE.features_file;
            }
            if (filenamepath.EndsWith("vs.vcs")) {
                return FILETYPE.vs_file;
            }
            if (filenamepath.EndsWith("ps.vcs")) {
                return FILETYPE.ps_file;
            }
            if (filenamepath.EndsWith("psrs.vcs")) {
                return FILETYPE.psrs_file;
            }
            if (filenamepath.EndsWith("gs.vcs")) {
                return FILETYPE.gs_file;
            }
            throw new ShaderParserException($"don't know what this file is {filenamepath}");
        }

        public static List<string> GetVcsFiles(string dir1, FILETYPE fileType) {
            return GetVcsFiles(dir1, null, fileType, -1);
        }

        public static List<string> GetVcsFiles(string dir1, string dir2, FILETYPE fileType, int numEnding, bool sortFiles = true) {
            List<string> filesFound = new();
            if (fileType == FILETYPE.features_file || fileType == FILETYPE.any) {
                string endsWith = numEnding > -1 ? $"{numEnding}_features.vcs" : "features.vcs";
                filesFound.AddRange(GetAllFilesWithEnding(dir1, endsWith));
                filesFound.AddRange(GetAllFilesWithEnding(dir2, endsWith));
            }
            if (fileType == FILETYPE.vs_file || fileType == FILETYPE.any) {
                string endsWith = numEnding > -1 ? $"{numEnding}_vs.vcs" : "vs.vcs";
                filesFound.AddRange(GetAllFilesWithEnding(dir1, endsWith));
                filesFound.AddRange(GetAllFilesWithEnding(dir2, endsWith));
            }
            if (fileType == FILETYPE.ps_file || fileType == FILETYPE.any) {
                string endsWith = numEnding > -1 ? $"{numEnding}_ps.vcs" : "ps.vcs";
                filesFound.AddRange(GetAllFilesWithEnding(dir1, endsWith));
                filesFound.AddRange(GetAllFilesWithEnding(dir2, endsWith));
            }
            if (fileType == FILETYPE.gs_file || fileType == FILETYPE.any) {
                string endsWith = numEnding > -1 ? $"{numEnding}_gs.vcs" : "gs.vcs";
                filesFound.AddRange(GetAllFilesWithEnding(dir1, endsWith));
                filesFound.AddRange(GetAllFilesWithEnding(dir2, endsWith));
            }
            if (fileType == FILETYPE.psrs_file || fileType == FILETYPE.any) {
                string endsWith = numEnding > -1 ? $"{numEnding}_psrs.vcs" : "psrs.vcs";
                filesFound.AddRange(GetAllFilesWithEnding(dir1, endsWith));
                filesFound.AddRange(GetAllFilesWithEnding(dir2, endsWith));
            }
            if (sortFiles) {
                filesFound.Sort();
            }
            return filesFound;
        }

        public static List<string> GetAllFilesWithEnding(string dir, string endsWith) {
            List<string> filesFound = new();
            if (dir == null) {
                return filesFound;
            }
            foreach (string filenamepath in Directory.GetFiles(dir)) {
                if (filenamepath.EndsWith(endsWith)) {
                    filesFound.Add(filenamepath);
                }
            }
            return filesFound;
        }



        public static (string, string, string) GetTriple(string ft) {
            if (!ft.EndsWith("features.vcs")) {
                throw new System.Exception("not a features file");
            }
            string vs = $"{ft[0..^12]}vs.vcs";
            string ps = $"{ft[0..^12]}ps.vcs";

            if (File.Exists(vs) && File.Exists(ps)) {
                return (ft, vs, ps);
            } else {
                return (null, null, null);
                // throw new System.Exception($"ps/vs files don't exist for {ft}");
            }
        }


        public static List<(string, string, string)> GetFeaturesVsPsFileTriple(string dir1, string dir2, int vcsFileVer = -1) {
            List<(string, string, string)> fileTriplets = new();
            List<string> featuresFiles = GetVcsFiles(dir1, dir2, FILETYPE.features_file, vcsFileVer);
            foreach (string featFile in featuresFiles) {
                string vsFile = $"{featFile[0..^12]}vs.vcs";
                string psFile = $"{featFile[0..^12]}ps.vcs";
                if (File.Exists(vsFile) && File.Exists(psFile)) {
                    fileTriplets.Add((featFile, vsFile, psFile));
                }
            }

            fileTriplets.Sort();
            return fileTriplets;
        }





        public static string RemoveBaseDir(string filenamepath) {
            string dirname = Path.GetDirectoryName(filenamepath);
            string filename = Path.GetFileName(filenamepath);
            if (dirname.EndsWith(@"\shaders\vfx")) {
                return @"\shaders\vfx\" + filename;
            } else if (dirname.EndsWith(@"\shaders-core\vfx")) {
                return @"\shaders-core\vfx\" + filename;
            } else {

                return filenamepath;
            }
        }

        public static string ShortHandName(string filenamepath) {
            string dirname = Path.GetDirectoryName(filenamepath);
            string filename = Path.GetFileName(filenamepath);
            if (dirname.EndsWith(@"\shaders\vfx")) {
                return @"\dota\" + filename;
            } else if (dirname.EndsWith(@"\shaders-core\vfx")) {
                return @"\core\" + filename;
            } else {

                return filenamepath;
            }
        }


        public static string GetHtmlHeader(string browserTitle, string pageHeader) {
            string html_header = "" +
                $"<!DOCTYPE html>\n<html>\n<head>\n  <title>{browserTitle}</title>\n" +
                $"  <link href='/includes/styles.css' rel='stylesheet' type='text/css' />\n" +
                $"</head>\n<body>\n<b>{pageHeader}</b>\n<pre>";

            return html_header;
        }

        public static string GetHtmlFooter() {
            return "</pre>\n</html>";
        }

        public static string GetGlslHtmlLink(string glslByteString) {
            return $"<a href='{GetGlslHtmlFilename(glslByteString)}'>{glslByteString}</a>";
        }

        public static string GetGlslHtmlFilename(string glslByteString) {
            return $"glsl-{glslByteString.Trim().Replace(" ", "").ToLower()}.html";
        }

        public static string GetGlslTxtFilename(string glslByteString) {
            return $"glsl-{glslByteString.Trim().Replace(" ", "").ToLower()}.txt";
        }

        // converts a features filename or filenamepath to an output file ending in ps-analysis.html
        // for links in the features file to relates ps and vs files
        public static string GetVsHtmlLink(string vcsFeaturesFilename, string urlText) {
            return $"<a href='{GetVsHtmlFilename(vcsFeaturesFilename)}'>{urlText}</a>";
        }

        public static string GetHtmlLink(string vcsFileName, string urlText = null) {
            if (urlText == null) {
                urlText = Path.GetFileName(vcsFileName);
            }
            return $"<a href='/vcs-all/{GetCoreOrDotaString(vcsFileName)}/{Path.GetFileName(vcsFileName)[0..^4]}-analysis.html'>{urlText}</a>";
        }

        public static string GetCoreOrDotaString(string vcsFileName) {
            if (Path.GetDirectoryName(vcsFileName).EndsWith("shaders\\vfx")) {
                return "dota";
            }
            if (Path.GetDirectoryName(vcsFileName).EndsWith("shaders-core\\vfx")) {
                return "core";
            }
            throw new ShaderParserException("don't know where this file belongs");
        }

        public static string GetShortName(string vcsFileName) {
            string shortName = "";
            string token = "";
            string filename = Path.GetFileName(vcsFileName);
            FILETYPE vcsFiletype = GetVcsFileType(filename);
            if (vcsFiletype == FILETYPE.features_file) {
                shortName = filename[0..^16];
                token = "ft";
            } else if (vcsFiletype == FILETYPE.psrs_file) {
                shortName = filename[0..^12];
                token = "psrs";
            } else {
                shortName = filename[0..^10];
                token = $"{vcsFiletype.ToString()[0..^5]}";
            }
            if (!shortName.EndsWith("pcgl")) {
                throw new ShaderParserException("this is only implemented for pcgl files");
            }
            return $"{shortName[0..^5]}({token})";
        }




        public static string GetVsHtmlFilename(string vcsFeaturesFilename) {
            if (!vcsFeaturesFilename.EndsWith("features.vcs")) {
                throw new ShaderParserException($"this needs to be features vcs file {vcsFeaturesFilename}");
            }
            return $"{Path.GetFileName(vcsFeaturesFilename)[0..^12]}vs-analysis.html";
        }

        public static string GetPsHtmlLink(string vcsFeaturesFilename, string urlText) {
            return $"<a href='{GetPsHtmlFilename(vcsFeaturesFilename)}'>{urlText}</a>";
        }

        // converts a features filename or filenamepath to an output file ending in ps-analysis.html
        public static string GetPsHtmlFilename(string vcsFeaturesFilename) {
            if (!vcsFeaturesFilename.EndsWith("features.vcs")) {
                throw new ShaderParserException($"this needs to be features vcs file {vcsFeaturesFilename}");
            }
            return $"{Path.GetFileName(vcsFeaturesFilename)[0..^12]}ps-analysis.html";
        }

        public static string GetZframeTxtFilename(uint zframeId, string vcsFilename) {
            return $"{Path.GetFileName(vcsFilename)[0..^4]}-ZFRAME{zframeId:x08}.txt";
        }

        public static string GetZframeHtmlFilename(uint zframeId, string vcsFilename) {
            return $"{Path.GetFileName(vcsFilename)[0..^4]}-ZFRAME{zframeId:x08}.html";
        }

        public static string GetZframeHtmlLink(uint zframeId, string vcsFilenamepath, string basedir = "") {
            return $"<a href='{basedir}{GetZframeHtmlFilename(zframeId, Path.GetFileName(vcsFilenamepath))}'>zframe[0x{zframeId:x08}]</a>";
        }

        public static string GetZframeHtmlLinkCheckExists(uint zframeId, string vcsFilenamepath, string serverdir, string basedir = "") {
            string zframeName = $"Z[0x{zframeId:x08}]";
            string zframeHtmlFilename = GetZframeHtmlFilename(zframeId, Path.GetFileName(vcsFilenamepath));
            if (File.Exists($"{serverdir}/{basedir}/{zframeHtmlFilename}")) {
                return $"  <a href='{basedir}{GetZframeHtmlFilename(zframeId, Path.GetFileName(vcsFilenamepath))}'>{zframeName}</a>";
            } else {
                return $"  {zframeName}";
            }
        }



        public static uint MurmurHashPiSeed(byte[] data) {
            uint PI_SEED = 0x31415926;
            return MurmurHash(data, PI_SEED);
        }
        public static uint MurmurHashPiSeed(string data) {
            uint PI_SEED = 0x31415926;
            return MurmurHash(data, PI_SEED);
        }
        public static uint MurmurHash(string data, uint seed) => MurmurHash(Encoding.ASCII.GetBytes(data), seed);
        public static uint MurmurHash(byte[] data, uint seed) {
            const uint M = 0x5bd1e995;
            const int R = 24;
            int length = data.Length;
            if (length == 0) {
                return 0;
            }
            uint h = seed ^ (uint)length;
            int ind = 0;
            while (length >= 4) {
                uint k = (uint)(data[ind++] | data[ind++] << 8 | data[ind++] << 16 | data[ind++] << 24);
                k *= M;
                k ^= k >> R;
                k *= M;
                h *= M;
                h ^= k;
                length -= 4;
            }
            switch (length) {
                case 3:
                    h ^= (ushort)(data[ind++] | data[ind++] << 8);
                    h ^= (uint)(data[ind] << 16);
                    h *= M;
                    break;
                case 2:
                    h ^= (ushort)(data[ind++] | data[ind] << 8);
                    h *= M;
                    break;
                case 1:
                    h ^= data[ind];
                    h *= M;
                    break;
                default:
                    break;
            }
            h ^= h >> 13;
            h *= M;
            h ^= h >> 15;
            return h;
        }



        private static Dictionary<int, int> collectValuesInt = new();
        private static Dictionary<string, int> collectValuesString = new();

        public static void CollectIntValue(int val) {
            int currIterator = collectValuesInt.GetValueOrDefault(val, 0);
            collectValuesInt[val] = currIterator + 1;
        }
        public static void CollectStringValue(string val) {
            int currIterator = collectValuesString.GetValueOrDefault(val, 0);
            collectValuesString[val] = currIterator + 1;
        }


        public static void PrintReport(bool showCount = true) {
            List<int> intvalues = new();
            foreach (int i in collectValuesInt.Keys) {
                intvalues.Add(i);
            }
            intvalues.Sort();
            foreach (int i in intvalues) {
                if (showCount) {
                    collectValuesInt.TryGetValue(i, out int instanceCount);
                    Debug.WriteLine($"{i,5}        {instanceCount,3}");
                } else {
                    Debug.WriteLine($"{i}");
                }
            }

            List<string> stringvalues = new();
            foreach (string s in collectValuesString.Keys) {
                stringvalues.Add(s);
            }
            stringvalues.Sort();
            foreach (string s in stringvalues) {
                if (showCount) {
                    collectValuesString.TryGetValue(s, out int instanceCount);
                    Debug.WriteLine($"{s.PadRight(80)}        {instanceCount,3}");
                } else {
                    Debug.WriteLine($"{s}");
                }
            }

            collectValuesInt = new();
            collectValuesString = new();
        }



        public static string ShortenShaderParam(string shaderParam) {
            if (shaderParam.Length <= 4) {
                return shaderParam;
            }
            string[] splitName = shaderParam[2..].Split("_");


            string newName = "";

            if (splitName[0] == "MODE") {
                if (splitName.Length == 2) {
                    return splitName[1].Length > 3 ? $"M_{splitName[1][0..3]}" : $"M_{splitName[1]}";
                }
                newName = "M_";
                for (int i = 1; i < splitName.Length; i++) {
                    newName += splitName[i][0..1];
                }
                return newName;
            }

            if (splitName.Length > 2) {
                for (int i = 0; i < splitName.Length && i < 5; i++) {
                    newName += splitName[i].Substring(0, 1);
                }
                return newName;
            }

            if (splitName.Length == 1) {
                return splitName[0].Length > 4 ? splitName[0].Substring(0,4) : splitName[0];
            }

            newName = splitName[0].Length > 3 ? splitName[0][0..3] : splitName[0];
            return $"{newName}_{splitName[1][0..1]}";
        }



        public static string CombineIntArray(int[] ints0, bool includeParenth = false) {
            if (ints0.Length == 0) return $"_";
            string valueString = "";
            foreach (int i in ints0) {
                valueString += $"{i},";
            }
            valueString = valueString[0..^1];
            return includeParenth ? $"({valueString})" : $"{valueString}";
        }

        public static string CombineIntsSpaceSep(int[] ints0, int padding = 5) {
            if (ints0.Length == 0) return $"_".PadLeft(padding);
            string valueString = "";
            foreach (int v in ints0) {
                string intPadded = $"{(v!=0 ? v:"_")}".PadLeft(padding);
                valueString += $"{intPadded}";
            }
            // return $"{valueString[0..^padding]}";
            return $"{valueString}";
        }

        public static string CombineStringsSpaceSep(string[] strings0, int padding = 5) {
            string combinedString = "";
            foreach (string s in strings0) {
                combinedString += s.PadLeft(padding);
            }
            return combinedString;
        }



        public static string CombineStringArray(string[] strings0, bool includeParenth = false) {
            if (strings0.Length == 0) return $"_";
            string combinedString = "";
            foreach (string s in strings0) {
                combinedString += $"{s}, ";
            }
            combinedString = combinedString[0..^2];
            return includeParenth ? $"({combinedString})" : $"{combinedString}";
        }

        public static void ShowIntArray(int[] ints0, int padding = 5, string label = null, bool hex = false) {
            string intsString = "";
            foreach (int v in ints0) {
                string val = hex ? $"{v:x}" : $"{v}";
                intsString += $"{(v!=0 ? val:"_")}".PadLeft(padding);
            }
            string labelstr = (label != null && hex) ? $"{label}(0x)" : $"{label}";
            labelstr = label != null ? $"{labelstr,12} = " : "";
            Debug.WriteLine($"{labelstr}{intsString.Trim()}");
        }


        public static string[] CombineValuesBreakString(string[] strings0, int breakLen) {
            List<string> stringCollection = new();
            if (strings0.Length == 0) {
                stringCollection.Add("");
                return stringCollection.ToArray();
            }
            string line = strings0[0] + ", ";
            for (int i = 1; i < strings0.Length; i++) {
                if (line.Length + strings0[i].Length + 1 < breakLen) {
                    line += strings0[i] + ", ";
                } else {
                    stringCollection.Add(line[0..^2]);
                    line = strings0[i] + ", ";
                }
            }
            if (line.Length > 0) {
                stringCollection.Add(line[0..^2]);
            }
            return stringCollection.ToArray(); ;
        }




    }
}









