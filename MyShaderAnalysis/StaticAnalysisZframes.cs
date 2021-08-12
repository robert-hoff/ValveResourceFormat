using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using MyShaderAnalysis.vcsparsing;
using System;
using MyShaderAnalysis.compat;
using static MyShaderAnalysis.vcsparsing.UtilHelpers;


namespace MyShaderAnalysis {


    public class StaticAnalysisZframes {

        const string PCGL_DIR_CORE = @"X:\dota-2-VRF-exports\dota2-export-shaders-pcgl\shaders-core\vfx";
        const string PCGL_DIR_NOT_CORE = @"X:\dota-2-VRF-exports\dota2-export-shaders-pcgl\shaders\vfx";
        const string PC_DIR_CORE = @"X:\dota-2-VRF-exports\dota2-export-shaders-pc\shaders-core\vfx";
        const string PC_DIR_NOT_CORE = @"X:\dota-2-VRF-exports\dota2-export-shaders-pc\shaders\vfx";
        const string OUTPUT_DIR = @"Z:\active\projects\dota2-sourcesdk-modding\shader-analysis-vcs-format\OUTPUT_DUMP";
        const string SERVER_OUTPUT_DIR = @"Z:\dev\www\vcs.codecreation.dev\GEN-output";
        const string SERVER_BASEDIR = @"Z:\dev\www\vcs.codecreation.dev";
        const string OUTPUT_SUB_DIR = @"\GEN-output";


        public static void RunTrials() {
            // Trial1();


            // SurverH0LeadingData();
            // SurverH1H2ValuesInDatablocks();
            // StateSummariesValuesSeen();
            // DifferencesInSuccessiveH0H2();
            // ZFrameHeaderComparisonGivenFile();
            // CheckB0ToB3Uniqeness();
            // CheckB0ValueAgainstParameterCount();
            // CheckB0ValuesUniqeness();
            // ShowB0ValuesSelectedFile();
            SurveryEndBlockAndGlslSourceCounts();
            // DataBlockCountSurvey();
            // DataBlockCountSelectedfile();
            // SurveryH1ValuesInDatablocks();
            // SurveryH0H2RelationshipInSpriteCardPs();
            // SurveryHeaderParams();
            // SurveryBytesInLeadingDataload();
            // SurveryLeadingDataSingleFile();



            // Debug.WriteLine($"{123:x}");

            PrintReport(showCount: true);
            CloseStreamWriter();
        }






        static void Trial1() {
            // string filenamepath = $@"{PCGL_DIR_NOT_CORE}\multiblend_pcgl_30_vs.vcs"; int useZFrame = 0x24;
            // string filenamepath = $@"{PCGL_DIR_NOT_CORE}\multiblend_pcgl_30_ps.vcs"; int useZFrame = 0xc9;
            string filenamepath = $@"{PCGL_DIR_CORE}\depth_only_pcgl_30_vs.vcs"; int useZFrame = 0x68;
            // string filenamepath = $@"{PCGL_DIR_NOT_CORE}\refract_pcgl_30_ps.vcs";
            // string filenamepath = $@"{PCGL_DIR_CORE}\visualize_cloth_pcgl_40_ps.vcs";

            ShaderFile shaderFile = new(filenamepath);
            ZFrameFile zframeFile = shaderFile.GetZFrameFile(useZFrame);


            // ZFrameFile zframeFile = shaderFile.GetZFrameFileByIndex(5);

            // Debug.WriteLine($"{zframeFile.leadingData.h0}");
            // Debug.WriteLine($"{zframeFile.leadingData.h1}");
            // Debug.WriteLine($"{zframeFile.leadingData.h2}");
            // Debug.WriteLine($"{DataReader.BytesToString(zframeFile.leadingData.dataload)}");

            // zframeFile.ShowZFrameHeader();
            zframeFile.ShowLeadSummary();
            // zframeFile.ShowDatablocks();
            zframeFile.ShowTailSummary();
            // zframeFile.ShowGlslSources();
            // zframeFile.ShowEndBlocks();

        }




        static void StateSummariesValuesSeen() {
            // List<string> vcsFiles = new();
            // vcsFiles.Add($@"{PCGL_DIR_NOT_CORE}\multiblend_pcgl_30_ps.vcs");
            List<string> vcsFiles = GetFileSelectionWithLimitedZframes();

            foreach (string vcsFilenamepath in vcsFiles) {
                ShaderFile shaderFile = new(vcsFilenamepath);
                for (int zframeIndex = 0; zframeIndex < shaderFile.GetZFrameCount(); zframeIndex++) {
                    ZFrameFile zframeFile = shaderFile.GetZFrameFileByIndex(zframeIndex);

                    foreach (int v in zframeFile.tailSummary) {
                        CollectIntValue(v);
                    }



                    // leadSummary values - some strange values encountered here
                    //if (zframeFile.leadSummary == null) {
                    //    continue;
                    //}
                    //foreach (int v in zframeFile.leadSummary) {
                    //    CollectIntValue(v);
                    //    if (v == 41) {
                    //        Debug.WriteLine($"{RemoveBaseDir(vcsFilenamepath)} {zframeFile.zframeId:x}");
                    //    }
                    //}

                }
            }
        }




        /*
         * doesn't appear to be any defnition limitations for these,
         * but multiples of 8 seem to occur by far the most common
         *
         *
         */
        static void DifferencesInSuccessiveH0H2() {
            List<string> vcsFiles = new();
            vcsFiles.Add($@"{PCGL_DIR_NOT_CORE}\multiblend_pcgl_30_ps.vcs");
            // List<string> vcsFiles = GetFileSelectionWithLimitedZframes();

            foreach (string vcsFilenamepath in vcsFiles) {
                ShaderFile shaderFile = new(vcsFilenamepath);
                for (int zframeIndex = 0; zframeIndex < shaderFile.GetZFrameCount(); zframeIndex++) {
                    ZFrameFile zframeFile = shaderFile.GetZFrameFileByIndex(zframeIndex);

                    for (int i = 1; i < zframeFile.dataBlocks.Count; i++) {

                        int h2last = zframeFile.dataBlocks[i - 1].h2;
                        int h2cur = zframeFile.dataBlocks[i].h2;

                        if (h2cur > 0 && h2last > 0) {
                            // Debug.WriteLine($"{h2last-h2cur}");
                            CollectIntValue(h2last - h2cur);
                        }


                        //if (diff == 4) {
                        //    Debug.WriteLine($"{RemoveBaseDir(vcsFilenamepath)} {zframeFile.zframeId:x} " +
                        //        $"{zframeFile.dataBlocks[i].blockId}");
                        //}
                    }
                }
            }
        }



        static void ZFrameHeaderComparisonGivenFile() {
            List<string> vcsFiles = new();
            // vcsFiles.Add($@"{PCGL_DIR_NOT_CORE}\multiblend_pcgl_30_ps.vcs");
            vcsFiles.Add($@"{PCGL_DIR_NOT_CORE}\multiblend_pcgl_30_vs.vcs");

            // List<string> vcsFiles = GetFileSelectionWithLimitedZframes();
            foreach (string vcsFilenamepath in vcsFiles) {
                ShaderFile shaderFile = new(vcsFilenamepath);
                for (int zframeIndex = 0; zframeIndex < shaderFile.GetZFrameCount(); zframeIndex++) {
                    ZFrameFile zframeFile = shaderFile.GetZFrameFileByIndex(zframeIndex);
                    // Debug.WriteLine($"{zframeFile.GetZFrameHeaderStringDescription()}");
                    // Debug.WriteLine($"");
                    // CollectStringValue(zframeFile.GetZFrameHeaderStringDescription());

                    if (zframeFile.zframeParams.Count == 4) {
                        Debug.WriteLine($"{zframeFile.zframeId:X}");
                    }
                }
            }
        }



        /*
         * The
         * DepthPassBatchID                         2c5b5105   05 FF FF   1
         * parameter is missing in the cases where S_MODE_TOOLS_WIREFRAME is enabled
         *
         *
         */
        static void MultiblendPsHeadersWhereDepthPassIsMissing() {
            List<string> vcsFiles = new();
            vcsFiles.Add($@"{PCGL_DIR_NOT_CORE}\multiblend_pcgl_30_ps.vcs");
            // List<string> vcsFiles = GetFileSelectionWithLimitedZframes();
            foreach (string vcsFilenamepath in vcsFiles) {
                ShaderFile shaderFile = new(vcsFilenamepath);
                for (int zframeIndex = 0; zframeIndex < shaderFile.GetZFrameCount(); zframeIndex++) {
                    ZFrameFile zframeFile = shaderFile.GetZFrameFileByIndex(zframeIndex);
                    if (zframeFile.zframeParams.Count == 5) {
                        Debug.WriteLine($"{zframeFile.zframeId:X}");
                    }
                }
            }
        }



        static void CheckB0ToB3Uniqeness() {
            List<string> vcsFiles = GetFileSelectionWithLimitedZframes();
            foreach (string vcsFilenamepath in vcsFiles) {
                ShaderFile shaderFile = new(vcsFilenamepath);
                for (int zframeIndex = 0; zframeIndex < shaderFile.GetZFrameCount(); zframeIndex++) {
                    ZFrameFile zframeFile = shaderFile.GetZFrameFileByIndex(zframeIndex);
                    // Debug.WriteLine($"zframeFile {zframeFile.zframeId}");
                    foreach (ZDataBlock dataBlock in zframeFile.dataBlocks) {
                        Dictionary<int, int> boundaryValues = new();
                        if (dataBlock.dataload != null) {
                            for (int i = 0; i < dataBlock.dataload.Length; i += 4) {
                                int b0 = dataBlock.dataload[i];
                                int b1 = dataBlock.dataload[i + 1];
                                int b2 = dataBlock.dataload[i + 2];
                                int b3 = dataBlock.dataload[i + 3];
                                int combinedValue = (b3 << 24) + (b2 << 16) + (b1 << 8) + b0;
                                boundaryValues.TryGetValue(combinedValue, out int count);
                                if (count > 0) {
                                    Debug.WriteLine($"repeated value found");
                                    Debug.WriteLine($"{RemoveBaseDir(vcsFilenamepath)} {zframeFile.zframeId:x} {dataBlock.blockId}");
                                    break;
                                }
                                boundaryValues[combinedValue] = count + 1;
                            }
                        }
                    }
                }
            }
        }



        /*
         *
         * The b0 values never exceed the file's parameter count
         *
         */
        static void CheckB0ValueAgainstParameterCount() {
            // List<string> vcsFiles = new();
            // vcsFiles.Add($@"{PCGL_DIR_NOT_CORE}\multiblend_pcgl_30_ps.vcs");

            List<string> vcsFiles = GetFileSelectionWithLimitedZframes();
            foreach (string vcsFilenamepath in vcsFiles) {
                ShaderFile shaderFile = new(vcsFilenamepath);
                int paramCount = shaderFile.paramBlocks.Count;
                Debug.WriteLine($"{RemoveBaseDir(vcsFilenamepath)} param-count={paramCount}");
                for (int zframeIndex = 0; zframeIndex < shaderFile.GetZFrameCount(); zframeIndex++) {
                    ZFrameFile zframeFile = shaderFile.GetZFrameFileByIndex(zframeIndex);
                    foreach (ZDataBlock dataBlock in zframeFile.dataBlocks) {
                        if (dataBlock.dataload != null) {
                            for (int i = 0; i < dataBlock.dataload.Length; i += 4) {
                                if (dataBlock.dataload[i] > paramCount) {
                                    Debug.WriteLine($"b0 val exceeded param count");
                                }
                            }
                        }
                    }
                }
            }
        }




        /*
         * The b0 byte within the dataloads do sometimes repeat
         * I suppose this might imply that it's meaningful to write the same parameter to multiple places
         *
         */
        static void CheckB0ValuesUniqeness() {
            // List<string> vcsFiles = new();
            // vcsFiles.Add($@"{PCGL_DIR_NOT_CORE}\multiblend_pcgl_30_vs.vcs");

            List<string> vcsFiles = GetFileSelectionWithLimitedZframes();
            foreach (string vcsFilenamepath in vcsFiles) {
                ShaderFile shaderFile = new(vcsFilenamepath);
                for (int zframeIndex = 0; zframeIndex < shaderFile.GetZFrameCount(); zframeIndex++) {
                    ZFrameFile zframeFile = shaderFile.GetZFrameFileByIndex(zframeIndex);
                    // Debug.WriteLine($"zframeFile {zframeFile.zframeId}");
                    foreach (ZDataBlock dataBlock in zframeFile.dataBlocks) {
                        Dictionary<int, int> b0Values = new();
                        if (dataBlock.dataload != null) {
                            for (int i = 0; i < dataBlock.dataload.Length; i += 4) {
                                b0Values.TryGetValue(dataBlock.dataload[i], out int count);
                                if (count > 0) {
                                    Debug.WriteLine($"repeated value found");
                                    Debug.WriteLine($"{RemoveBaseDir(vcsFilenamepath)} {zframeFile.zframeId:x} {dataBlock.blockId}");
                                    break;
                                    //int b0 = dataBlock.dataload[i];
                                    //int b1 = dataBlock.dataload[i+1];
                                    //int b2 = dataBlock.dataload[i+2];
                                    //int b3 = dataBlock.dataload[i+3];
                                    //CollectStringValue($"{b0:X02} {b1:X02} {b2:X02} {b3:X02}");

                                }
                                b0Values[dataBlock.dataload[i]] = count + 1;
                            }
                        }
                    }
                }
            }
        }



        static void ShowB0ValuesSelectedFile() {
            // List<string> vcsFiles = GetFileSelectionWithLimitedZframes();
            List<string> vcsFiles = new();
            vcsFiles.Add($@"{PCGL_DIR_NOT_CORE}\multiblend_pcgl_30_vs.vcs");
            foreach (string vcsFilenamepath in vcsFiles) {
                ShaderFile shaderFile = new(vcsFilenamepath);
                // Dictionary<int, int> b0Values = new();
                for (int zframeIndex = 0; zframeIndex < shaderFile.GetZFrameCount(); zframeIndex++) {
                    ZFrameFile zframeFile = shaderFile.GetZFrameFileByIndex(zframeIndex);
                    Debug.WriteLine($"zframeFile {zframeFile.zframeId}");
                    foreach (ZDataBlock dataBlock in zframeFile.dataBlocks) {
                        if (dataBlock.dataload != null) {
                            for (int i = 0; i < dataBlock.dataload.Length; i += 4) {
                                Debug.Write($"{dataBlock.dataload[i]:X02} ");
                            }
                            Debug.WriteLine("");
                        }
                    }
                }
            }

        }


        /*
         * nr of end blocks and source-count is not the same, but
         * nr of sources never exceeds the number of end blocks as each end block
         * needs a source reference
         *
         * the nonZeroDataBlockCount and nrEndBlocks are usually the same, but in some cases a
         * datablock that matches a valid configuration may be empty.
         * This appears to be most common when zframeId = 0, and quite uncommon if zframeId != 0
         *
         */
        static void SurveryEndBlockAndGlslSourceCounts() {
            List<string> vcsFiles = GetFileSelectionWithLimitedZframes();
            foreach (string vcsFilenamepath in vcsFiles) {
                ShaderFile shaderFile = new(vcsFilenamepath);
                for (int i = 0; i < shaderFile.GetZFrameCount(); i++) {
                    ZFrameFile zframeFile = shaderFile.GetZFrameFileByIndex(i);


                    // CollectStringValue($"{zframeFile.glslSourceCount}  {zframeFile.nrEndBlocks}");
                    if (zframeFile.nrEndBlocks != zframeFile.glslSourceCount) {
                        Debug.WriteLine($"{RemoveBaseDir(vcsFilenamepath)} {zframeFile.zframeId:x}");
                        break;
                    }


                    // CollectStringValue($"{zframeFile.nonZeroDataBlockCount}  {zframeFile.nrEndBlocks}");

                    //if (zframeFile.nonZeroDataBlockCount == 0) {
                    //    Debug.WriteLine($"{vcsFilenamepath}");
                    //    Debug.WriteLine($"{zframeFile.zframeId}");
                    //}

                    //if (zframeFile.zframeId != 0) {
                    //    CollectStringValue($"{zframeFile.nonZeroDataBlockCount}  {zframeFile.nrEndBlocks}");
                    //}


                }
            }
        }



        static void DataBlockCountSurvey() {
            List<string> vcsFiles = GetFileSelectionWithLimitedZframes();
            foreach (string vcsFilenamepath in vcsFiles) {
                ShaderFile shaderFile = new(vcsFilenamepath);
                for (int i = 0; i < shaderFile.GetZFrameCount(); i++) {
                    ZFrameFile zframeFile = shaderFile.GetZFrameFileByIndex(0);
                    CollectIntValue(zframeFile.dataBlocks.Count);
                }
            }
        }



        static void DataBlockCountSelectedfile() {
            // string filenamepath = $@"{PCGL_DIR_NOT_CORE}\spritecard_pcgl_30_ps.vcs";
            // string filenamepath = $@"{PCGL_DIR_NOT_CORE}\multiblend_pcgl_30_ps.vcs";
            string filenamepath = $@"{PCGL_DIR_NOT_CORE}\multiblend_pcgl_30_vs.vcs";
            ShaderFile shaderFile = new(filenamepath);
            for (int i = 0; i < shaderFile.GetZFrameCount(); i++) {
                if (i % 1000 == 0) {
                    Debug.WriteLine($"{i}");
                }
                ZFrameFile zframeFile = shaderFile.GetZFrameFileByIndex(i);
                CollectIntValue(zframeFile.dataBlocks.Count);
            }
        }



        /*
         * If leadingData.h0 is 0 then all the datablocks data are also 0
         *
         *
         *
         */
        static void SurverH0LeadingData() {
            List<string> vcsFiles = GetFileSelectionWithLimitedZframes();
            foreach (string vcsFilenamepath in vcsFiles) {
                ShaderFile shaderFile = new(vcsFilenamepath);
                for (int zframeId = 0; zframeId < shaderFile.GetZFrameCount(); zframeId++) {
                    ZFrameFile zframeFile = shaderFile.GetZFrameFileByIndex(zframeId);
                    CollectIntValue(zframeFile.leadingData.h0);
                    if (zframeFile.leadingData.h0 == 0) {
                        Debug.WriteLine($"{ShortHandName(vcsFilenamepath),-70} {zframeFile.zframeId:x}");
                        //foreach (ZDataBlock zBlock in zframeFile.dataBlocks) {
                        //    Debug.WriteLine($"{zBlock.h0}");
                        //}
                    }
                }
            }
        }


        /*
         * The value of the h1 argument for all non-leading datablocks is 0
         *
         */
        static void SurverH1H2ValuesInDatablocks() {
            List<string> vcsFiles = GetFileSelectionWithLimitedZframes();
            foreach (string vcsFilenamepath in vcsFiles) {
                ShaderFile shaderFile = new(vcsFilenamepath);
                for (int zframeId = 0; zframeId < shaderFile.GetZFrameCount(); zframeId++) {
                    ZFrameFile zframeFile = shaderFile.GetZFrameFileByIndex(zframeId);

                    if (zframeFile.leadingData.h1 != 0) {
                        CollectIntValue(zframeFile.leadingData.h2 - zframeFile.leadingData.h1);
                        // Debug.WriteLine($"{RemoveBaseDir(vcsFilenamepath)} {zframeFile.zframeId:x}");
                    }




                    // remember zBlock.h1 is ALWAYS 0 !!!


                    //foreach (ZDataBlock zBlock in zframeFile.dataBlocks) {
                    //    if (zBlock.dataload == null) {
                    //        continue;
                    //    }
                    //    if (zBlock.h1 != 0) {
                    //        CollectIntValue(zBlock.h2 - zBlock.h1);
                    //    }
                    //}
                }
            }
        }


        /*
         * The value of the h1 argument for all non-leading datablocks is 0
         *
         */
        static void SurveryH1ValuesInDatablocks() {
            List<string> vcsFiles = GetFileSelectionWithLimitedZframes();
            foreach (string vcsFilenamepath in vcsFiles) {
                ShaderFile shaderFile = new(vcsFilenamepath);
                for (int i = 0; i < shaderFile.GetZFrameCount(); i++) {
                    ZFrameFile zframeFile = shaderFile.GetZFrameFileByIndex(i);

                    // check h0 values
                    //foreach (ZDataBlock zBlock in zframeFile.dataBlocks) {
                    //    CollectIntValue(zBlock.h0);
                    //}

                    // check h0-h2 difference
                    //CollectIntValue(zframeFile.leadingData.h0 - zframeFile.leadingData.h2);
                    //foreach (ZDataBlock zBlock in zframeFile.dataBlocks) {
                    //    CollectIntValue(zBlock.h0-zBlock.h2);
                    //}

                    // check h0-h2 ratio (shows nan if h0==0 and inf if h2==0)
                    // this ratio can be anything and is not interesting
                    //CollectStringValue($"{(double)zframeFile.leadingData.h0 / zframeFile.leadingData.h2}");
                    //foreach (ZDataBlock zBlock in zframeFile.dataBlocks) {
                    //    CollectStringValue($"{(double)zBlock.h0 / zBlock.h2}");
                    //}

                    // check h2 values
                    //foreach (ZDataBlock zBlock in zframeFile.dataBlocks) {
                    //    CollectIntValue(zBlock.h2);
                    //}

                }
            }
        }


        /*
         * for spritecard_pcgl_30_ps.vcs the differece h0-h2 in
         * all datablocks (including leading data) is zero
         *
         */
        static void SurveryH0H2RelationshipInSpriteCardPs() {
            // string filenamepath = $@"{PCGL_DIR_NOT_CORE}\multiblend_pcgl_30_ps.vcs";
            string filenamepath = $@"{PCGL_DIR_NOT_CORE}\spritecard_pcgl_30_ps.vcs";
            ShaderFile shaderFile = new(filenamepath);
            for (int i = 0; i < shaderFile.GetZFrameCount(); i++) {
                if (i % 1000 == 0) {
                    Debug.WriteLine($"{i}");
                }
                ZFrameFile zframeFile = shaderFile.GetZFrameFileByIndex(i);
                CollectIntValue(zframeFile.leadingData.h0 - zframeFile.leadingData.h2);
                foreach (ZDataBlock zBlock in zframeFile.dataBlocks) {
                    CollectIntValue(zBlock.h0 - zBlock.h2);
                }
            }
        }



        static void SurveryHeaderParams() {
            List<string> vcsFiles = GetFileSelectionWithLimitedZframes();
            foreach (string vcsFilenamepath in vcsFiles) {
                ShaderFile shaderFile = new(vcsFilenamepath);
                List<ZFrameFile> zframeFiles = new();
                for (int i = 0; i < shaderFile.GetZFrameCount(); i++) {
                    zframeFiles.Add(shaderFile.GetZFrameFileByIndex(i));
                }
                foreach (ZFrameFile zframeFile in zframeFiles) {

                    // zframeFile.ShowZFrameHeader();
                    foreach (var zparam in zframeFile.zframeParams) {
                        CollectStringValue(zparam.ToString());
                    }
                }
            }
        }



        static void SurveryBytesInLeadingDataload() {
            List<string> vcsFiles = GetFileSelectionWithLimitedZframes();
            foreach (string vcsFilenamepath in vcsFiles) {
                ShaderFile shaderFile = new(vcsFilenamepath);
                List<ZFrameFile> zframeFiles = new();
                for (int i = 0; i < shaderFile.GetZFrameCount(); i++) {
                    zframeFiles.Add(shaderFile.GetZFrameFileByIndex(i));
                }
                foreach (ZFrameFile zframeFile in zframeFiles) {
                    for (int i = 3; i < zframeFile.leadingData.dataload.Length; i += 4) {
                        if (zframeFile.leadingData.dataload[i] == 0) {
                            CollectStringValue($"{zframeFile.leadingData.dataload[i - 1]:X02} {zframeFile.leadingData.dataload[i]:X02}");
                            if (zframeFile.leadingData.dataload[i - 1] == 0x35 && zframeFile.leadingData.dataload[i] == 0) {
                                Debug.WriteLine($"{zframeFile.filenamepath}");
                                Debug.WriteLine($"{zframeFile.zframeId}");
                                goto breakhere;
                            }
                        }
                    }
                }
            }
            breakhere: Debug.WriteLine("");
        }



        /*
         * A big problem with surverying zframes is that it creates insane workloads
         * decompressing and parsing hero_pcgl_30_ps.vcs taking 20 GB of data.
         * I've setup a selection that chooses files with 4000 zframes or less only, which is pretty manageable
         *
         *
         */
        static void SurveryLeadingDataSingleFile() {

            List<string> vcsFiles = GetFileSelectionWithLimitedZframes();
            foreach (string vcsFilenamepath in vcsFiles) {
                ShaderFile shaderFile = new(vcsFilenamepath);
                List<ZFrameFile> zframeFiles = new();
                for (int i = 0; i < shaderFile.GetZFrameCount(); i++) {
                    zframeFiles.Add(shaderFile.GetZFrameFileByIndex(i));
                }
                foreach (ZFrameFile zframeFile in zframeFiles) {
                    CollectStringValue($"{zframeFile.leadingData.h0,2} {zframeFile.leadingData.h1,2} {zframeFile.leadingData.h2,2}");
                }
            }
        }





        static List<string> GetFileSelectionWithLimitedZframes() {
            List<string> vcsFiles = new();
            List<string> selectedFiles = GetVcsFiles(PCGL_DIR_CORE, PCGL_DIR_NOT_CORE, FILETYPE.any, 30);
            // List<string> selectedFiles = GetVcsFiles(PCGL_DIR_CORE, PCGL_DIR_NOT_CORE, FILETYPE.vs_file, 30);


            foreach (string checkVcsFile in selectedFiles) {
                ShaderFile shaderFile = new ShaderFile(checkVcsFile);
                if (shaderFile.GetZFrameCount() < 4000 && shaderFile.GetZFrameCount() > 0) {
                    vcsFiles.Add(checkVcsFile);
                }
            }

            return vcsFiles;
        }

        // needs work! currently returning null. perhaps add interesting test cases as I go along
        static List<string> GetManualFileSelection() {
            // don't do this one yet!
            // string filenamepath = $@"{PCGL_DIR_NOT_CORE}\hero_pcgl_30_ps.vcs";
            List<string> vcsFiles = new();
            vcsFiles.Add($@"{PCGL_DIR_NOT_CORE}\multiblend_pcgl_30_ps.vcs");
            vcsFiles.Add($@"{PCGL_DIR_NOT_CORE}\multiblend_pcgl_30_vs.vcs");
            vcsFiles.Add($@"{PCGL_DIR_NOT_CORE}\grasstile_pcgl_41_ps.vcs");
            vcsFiles.Add($@"{PCGL_DIR_NOT_CORE}\grasstile_pcgl_41_vs.vcs");
            return null;
        }







        private static StreamWriter sw = null;
        private static bool DisableOutput = false;
        private static bool WriteAsHtml = false;
        private static bool swWriterAlreadyClosed = false;


        // This basestream != null is nonsense, it doesn't check if the file is open
        private static void CloseStreamWriter() {
            if (WriteAsHtml && !swWriterAlreadyClosed) {
                sw.WriteLine(GetHtmlFooter());
            }
            if (sw != null && !swWriterAlreadyClosed) {
                sw.Close();
            }
        }

        public static void OutputWrite(string text) {
            if (!DisableOutput) {
                Debug.Write(text);
            }
            if (sw != null) {
                sw.Write(text);
            }
        }
        public static void OutputWriteLine(string text) {
            OutputWrite(text + "\n");
        }


    }



}








