using System;
using System.IO;
using System.Collections.Generic;

using ShaderFile = ValveResourceFormat.CompiledShader.ShaderFile;
using ZFrameFile = ValveResourceFormat.CompiledShader.ZFrameFile;
using ZDataBlock = ValveResourceFormat.CompiledShader.ZDataBlock;
using VcsProgramType = ValveResourceFormat.CompiledShader.VcsProgramType;
using ConfigMappingSParams = ValveResourceFormat.CompiledShader.ConfigMappingSParams;

using static MyShaderAnalysis.codestash.MyTrashUtilHelpers;


namespace MyShaderAnalysis.codestash
{

    public class StaticAnalysisZframes
    {

        public const string DOTA_CORE_PCGL_SOURCE = "X:/dota-2-VRF-exports/dota2-export-shaders-pcgl/shaders-core/vfx";
        public const string DOTA_GAME_PCGL_SOURCE = "X:/dota-2-VRF-exports/dota2-export-shaders-pcgl/shaders/vfx";
        public const string DOTA_CORE_PC_SOURCE = "X:/dota-2-VRF-exports/dota2-export-shaders-pc/shaders-core/vfx";
        public const string DOTA_GAME_PC_SOURCE = "X:/dota-2-VRF-exports/dota2-export-shaders-pc/shaders/vfx";
        public const string DOTA_CORE_MOBILE_GLES_SOURCE = "X:/dota-2-VRF-exports/dota2-shaders-mobile-gles/core";
        public const string DOTA_DAC_MOBILE_GLES_SOURCE = "X:/dota-2-VRF-exports/dota2-shaders-mobile-gles/dac";
        public const string DOTA_CORE_ANDROID_VULKAN_SOURCE = "X:/dota-2-VRF-exports/dota2-shaders-android-vulkan/core";
        public const string DOTA_DAC_ANDROID_VULKAN_SOURCE = "X:/dota-2-VRF-exports/dota2-shaders-android-vulkan/dac";
        public const string DOTA_CORE_IOS_VULKAN_SOURCE = "X:/dota-2-VRF-exports/dota2-shaders-ios-vulkan/core";
        public const string DOTA_DAC_IOS_VULKAN_SOURCE = "X:/dota-2-VRF-exports/dota2-shaders-ios-vulkan/dac";
        public const string ARTIFACT_CLASSIC_CORE_PC_SOURCE = "X:/artifact-classic-exports/artifact-shaders-pc-core";
        public const string ARTIFACT_CLASSIC_DCG_PC_SOURCE = "X:/artifact-classic-exports/artifact-shaders-pc-dcg";
        public const string HLALYX_CORE_VULKAN_SOURCE = "X:/hl2alyx-export/alyx-vulkan-core";
        public const string HLALYX_HLVR_VULKAN_SOURCE = "X:/hl2alyx-export/alyx-vulkan-hlvr";
        const string PCGL_DIR_CORE = @"X:/dota-2-VRF-exports/dota2-export-shaders-pcgl/shaders-core/vfx";
        const string PCGL_DIR_NOT_CORE = @"X:/dota-2-VRF-exports/dota2-export-shaders-pcgl/shaders/vfx";
        const string PC_DIR_CORE = @"X:/dota-2-VRF-exports/dota2-export-shaders-pc/shaders-core/vfx";
        const string PC_DIR_NOT_CORE = @"X:/dota-2-VRF-exports/dota2-export-shaders-pc/shaders/vfx";
        const string OUTPUT_DIR = @"Z:/active/projects/dota2-sourcesdk-modding/shader-analysis-vcs-format/OUTPUT_DUMP";
        const string SERVER_OUTPUT_DIR = @"Z:/dev/www/vcs.codecreation.dev/GEN-output";
        const string SERVER_BASEDIR = @"Z:/dev/www/vcs.codecreation.dev";
        const string OUTPUT_SUB_DIR = @"/GEN-output";


        public static void RunTrials()
        {
            // Trial1();

            // float f = BitConverter.ToSingle(new byte[] { 0xff,0xff,0xff,0xff}, 0);
            // Console.WriteLine($"{f}");


            ZFrameEndBlocks5();
            // ZFrameEndBlocks4();
            // ZFrameEndBlocks3();
            // ZFrameEndBlocks2();
            // ZFrameEndBlocks();
            // CountZframeAndSourceFilesSingleFile1();
            // SurverH0LeadingData();
            // SurverH1H2ValuesInDatablocks();
            // StateSummariesValuesSeen();
            // DifferencesInSuccessiveH0H2();
            // ZFrameHeaderComparisonGivenFile();
            // CheckB0ToB3Uniqeness();
            // CheckB0ValueAgainstParameterCount();
            // CheckB0ValuesUniqeness();
            // ShowB0ValuesSelectedFile();
            // SurveryEndBlockAndGlslSourceCounts();
            // DataBlockCountSurvey();
            // DataBlockCountSelectedfile();
            // SurveryH1ValuesInDatablocks();
            // SurveryH0H2RelationshipInSpriteCardPs();
            // SurveryHeaderParams();
            // SurveryBytesInLeadingDataload();
            // SurveryLeadingDataSingleFile();



            // Console.WriteLine($"{123:x}");

            PrintReport(showCount: true);
            CloseStreamWriter();
        }


        static void ZFrameEndBlocks5()
        {
            // List<string> vcsFiles = new();
            // vcsFiles.Add($@"{PCGL_DIR_NOT_CORE}/multiblend_pcgl_30_ps.vcs");
            List<string> vcsFiles = GetFileSelectionWithLimitedZframes();

            foreach (string vcsFilenamepath in vcsFiles)
            {
                if (ComputeVCSFileName(vcsFilenamepath).Item1 != VcsProgramType.PixelShader)
                {
                    continue;
                }

                ShaderFile shaderFile = InstantiateShaderFile(vcsFilenamepath);

                for (int zframeIndex = 0; zframeIndex < shaderFile.GetZFrameCount(); zframeIndex++)
                {
                    ZFrameFile zframeFile = shaderFile.GetZFrameFileByIndex(zframeIndex);
                    foreach (var endBlock in zframeFile.psEndBlocks)
                    {
                        //if (endBlock.data1 == null)
                        //{
                        //    endBlock.data1 = new byte[] { };
                        //}
                        //CollectStringValue($"{BytesToString(endBlock.data1, -1)}");

                        //string hasdata0 = endBlock.hasData0 ? "1" : "0";
                        //string hasdata1 = endBlock.hasData1 ? "1" : "0";
                        //string hasdata2 = endBlock.hasData2 ? "1" : "0";
                        //CollectStringValue($"{hasdata0} {hasdata1} {hasdata2}");

                        //if (!endBlock.hasData0 && endBlock.hasData1 && !endBlock.hasData2)
                        //{
                        //    Console.WriteLine($"{zframeFile.filenamepath}       zframeId={zframeFile.zframeId:x08}");
                        //}



                        //if (endBlock.data0 == null || endBlock.data1 == null)
                        //{
                        //    endBlock.data0 = new byte[] { };
                        //    continue;
                        //}

                        // int f1 = BitConverter.ToInt32(endBlock.data0, 4);
                        // float f2 = BitConverter.ToSingle(endBlock.data0, 8);
                        // CollectStringValue($"{BytesToString(endBlock.data0)}    {f1} {f2}");


                        string show0 = endBlock.data0 == null ? "".PadRight(47) : BytesToString(endBlock.data0);
                        string show1 = endBlock.data1 == null ? "".PadRight(59) : BytesToString(endBlock.data1);
                        CollectStringValue($"{show0}   {show1}");


                    }
                }
            }
        }


        static void ZFrameEndBlocks4()
        {
            List<string> vcsFiles = new();
            // vcsFiles.Add($@"{PCGL_DIR_CORE}/bilateral_blur_pcgl_30_ps.vcs");
            vcsFiles.Add($@"{PCGL_DIR_CORE}/complex_pcgl_50_ps.vcs");
            // vcsFiles.Add($@"{PCGL_DIR_CORE}/bilateral_blur_pcgl_30_ps.vcs");
            // vcsFiles.Add($@"{DOTA_CORE_PC_SOURCE}/tools_grid_pc_50_ps.vcs");
            // vcsFiles.Add($@"{PCGL_DIR_CORE}/copytexture_pcgl_30_ps.vcs");
            // vcsFiles.Add($@"{PCGL_DIR_CORE}/depth_only_pcgl_30_ps.vcs");
            foreach (string vcsFilenamepath in vcsFiles)
            {
                if (ComputeVCSFileName(vcsFilenamepath).Item1 != VcsProgramType.PixelShader)
                {
                    continue;
                }
                ShaderFile shaderFile = InstantiateShaderFile(vcsFilenamepath);

                // Console.WriteLine($"{vcsFilenamepath}");
                //Console.WriteLine("hi");
                //Console.WriteLine("hi2");
                //Console.WriteLine("h3");

                ConfigMappingSParams configMap = new ConfigMappingSParams(shaderFile);


                // Console.WriteLine($"{shaderFile.GetZFrameCount()}");
                for (int zIndex = 0; zIndex < shaderFile.GetZFrameCount(); zIndex++)
                {

                    ZFrameFile zframeFile = shaderFile.GetZFrameFileByIndex(zIndex);
                    int[] configState = configMap.GetConfigState(zframeFile.zframeId);
                    Console.WriteLine($"{zframeFile.filenamepath}       zframeId={zframeFile.zframeId:x08}");
                    ShowIntArray(configState);
                    foreach (var endBlock in zframeFile.psEndBlocks)
                    {

                        byte[] sec0 = new byte[3];
                        sec0[0] = endBlock.data2[0];
                        sec0[1] = endBlock.data2[1];
                        sec0[2] = endBlock.data2[2];
                        byte[] sec1 = new byte[24];
                        byte[] sec2 = new byte[32];
                        for (int i = 3; i < 27; i++)
                        {
                            sec1[i - 3] = endBlock.data2[i];
                        }
                        for (int i = 35; i < 67; i++)
                        {
                            sec2[i - 35] = endBlock.data2[i];
                        }
                        if (endBlock.data1 == null)
                        {
                            // endBlock.data1 = new byte[] { };
                            continue;
                        }
                        CollectStringValue($"{BytesToString(endBlock.data1, -1)}");

                        // Console.WriteLine($"{BytesToString(endBlock.data1, -1)}");
                        Console.WriteLine($"[{endBlock.blockIdRef,2}] {BytesToString(sec0, -1)}  {BytesToString(sec1, -1)}     {BytesToString(sec2, -1)}");
                    }
                }
            }

        }



        static void ZFrameEndBlocks3()
        {
            List<string> vcsFiles = new();
            // vcsFiles.Add($@"{PCGL_DIR_NOT_CORE}/multiblend_pcgl_30_ps.vcs");
            // List<string> vcsFiles = GetFileSelectionWithLimitedZframes();

            foreach (string vcsFilenamepath in vcsFiles)
            {
                if (ComputeVCSFileName(vcsFilenamepath).Item1 != VcsProgramType.PixelShader)
                {
                    continue;
                }

                ShaderFile shaderFile = InstantiateShaderFile(vcsFilenamepath);
                for (int zframeIndex = 0; zframeIndex < shaderFile.GetZFrameCount(); zframeIndex++)
                {
                    ZFrameFile zframeFile = shaderFile.GetZFrameFileByIndex(zframeIndex);
                    foreach (var endBlock in zframeFile.psEndBlocks)
                    {
                        if (endBlock.data2 == null || endBlock.data2.Length == 0)
                        {
                            continue;
                        }
                        byte[] sec1 = new byte[24];
                        byte[] sec2 = new byte[32];
                        for (int i = 3; i < 27; i++)
                        {
                            sec1[i - 3] = endBlock.data2[i];
                        }
                        for (int i = 35; i < 67; i++)
                        {
                            sec2[i - 35] = endBlock.data2[i];
                        }
                        // if (endBlock.data2[11] == 8)
                        // if (endBlock.data2[19] == 12)
                        if (endBlock.data2[0] == 1)
                        {
                            Console.WriteLine(vcsFilenamepath);
                            Console.WriteLine($"{BytesToString(sec1, -1)}     {BytesToString(sec2, -1)}");
                        }

                    }
                }
            }
        }


        static void ZFrameEndBlocks2()
        {
            // List<string> vcsFiles = new();
            // vcsFiles.Add($@"{PCGL_DIR_NOT_CORE}/multiblend_pcgl_30_ps.vcs");
            List<string> vcsFiles = GetFileSelectionWithLimitedZframes();

            foreach (string vcsFilenamepath in vcsFiles)
            {
                if (ComputeVCSFileName(vcsFilenamepath).Item1 != VcsProgramType.PixelShader)
                {
                    continue;
                }

                ShaderFile shaderFile = InstantiateShaderFile(vcsFilenamepath);
                for (int zframeIndex = 0; zframeIndex < shaderFile.GetZFrameCount(); zframeIndex++)
                {
                    ZFrameFile zframeFile = shaderFile.GetZFrameFileByIndex(zframeIndex);
                    foreach (var endBlock in zframeFile.psEndBlocks)
                    {
                        if (endBlock.data2 == null || endBlock.data2.Length == 0)
                        {
                            continue;
                        }
                        byte[] sec1 = new byte[24];
                        byte[] sec2 = new byte[32];
                        for (int i = 3; i < 27; i++)
                        {
                            sec1[i - 3] = endBlock.data2[i];
                        }
                        for (int i = 35; i < 67; i++)
                        {
                            sec2[i - 35] = endBlock.data2[i];
                        }
                        CollectStringValue($"{BytesToString(sec1, -1)}     {BytesToString(sec2, -1)}");
                    }
                }
            }
        }



        static void ZFrameEndBlocks()
        {
            // List<string> vcsFiles = new();
            // vcsFiles.Add($@"{PCGL_DIR_NOT_CORE}/multiblend_pcgl_30_ps.vcs");
            List<string> vcsFiles = GetFileSelectionWithLimitedZframes();

            foreach (string vcsFilenamepath in vcsFiles)
            {
                if (ComputeVCSFileName(vcsFilenamepath).Item1 != VcsProgramType.PixelShader)
                {
                    continue;
                }

                ShaderFile shaderFile = InstantiateShaderFile(vcsFilenamepath);

                for (int zframeIndex = 0; zframeIndex < shaderFile.GetZFrameCount(); zframeIndex++)
                {
                    ZFrameFile zframeFile = shaderFile.GetZFrameFileByIndex(zframeIndex);
                    foreach (var endBlock in zframeFile.psEndBlocks)
                    {
                        if (endBlock.data2 == null || endBlock.data2.Length == 0)
                        {
                            continue;
                        }
                        CollectStringValue($"{BytesToString(endBlock.data2, -1)}");
                    }
                }
            }
        }





        static void Trial1()
        {
            // string filenamepath = $@"{PCGL_DIR_NOT_CORE}/multiblend_pcgl_30_vs.vcs"; int useZFrame = 0x24;
            // string filenamepath = $@"{PCGL_DIR_NOT_CORE}/multiblend_pcgl_30_ps.vcs"; int useZFrame = 0xc9;
            string filenamepath = $@"{PCGL_DIR_CORE}/depth_only_pcgl_30_vs.vcs"; int useZFrame = 0x68;
            // string filenamepath = $@"{PCGL_DIR_NOT_CORE}/refract_pcgl_30_ps.vcs";
            // string filenamepath = $@"{PCGL_DIR_CORE}/visualize_cloth_pcgl_40_ps.vcs";

            ShaderFile shaderFile = InstantiateShaderFile(filenamepath);
            ZFrameFile zframeFile = shaderFile.GetZFrameFile(useZFrame);


            // ZFrameFile zframeFile = shaderFile.GetZFrameFileByIndex(5);

            // Console.WriteLine($"{zframeFile.leadingData.h0}");
            // Console.WriteLine($"{zframeFile.leadingData.h1}");
            // Console.WriteLine($"{zframeFile.leadingData.h2}");
            // Console.WriteLine($"{DataReader.BytesToString(zframeFile.leadingData.dataload)}");

            // R: took these out
            // zframeFile.ShowZFrameHeader();
            // zframeFile.ShowLeadSummary();
            // zframeFile.ShowDatablocks();
            // zframeFile.ShowTailSummary();
            // zframeFile.ShowGlslSources();
            // zframeFile.ShowEndBlocks();

        }








        /*
         *
         * global_lit_simple_pcgl_30.vs.vcs
         * has 10360 source reference and 3960 unique sources. The reference distribution is quite interesting
         *
         *                       multiple-reference
         *          800 files    1
         *         1860 files    2
         *         1140 files    3
         *          160 files    4
         *
         *
         * multiblend_pcgl_30_ps.vcs
         * has 84218 source references and 39610 unique sources. Some sources are references upto 18 times
         *
         *                  referenced (number of times)
         *     10440        1
         *     18099        2
         *      9690        3
         *       897        4
         *       149        6
         *         2        7
         *         1        8
         *       327       12
         *         2       14
         *         3       18      (3 files are referenced 18 times)
         *
         *
         * hero_pcgl_30_ps.vcs (takes about 1:40 to process)
         *
         * 852120 source references and 571944 unique source - so most sources are not shared!
         * Actually the bulk of these have only one reference.
         *
         * The source distribution count goes from
         * (1,508712) to (40,182). i.e.182 files are shared 40 times, which is the max
         *
         *
         *
         * NB - remember to set a breakpoint!
         *
         */
        static void CountZframeAndSourceFilesSingleFile1()
        {
            // string filenamepath = $@"{PCGL_DIR_NOT_CORE}/multiblend_pcgl_30_vs.vcs";
            // string filenamepath = $@"{PCGL_DIR_NOT_CORE}/multiblend_pcgl_30_ps.vcs";
            // string filenamepath = $@"{PCGL_DIR_NOT_CORE}/hero_pcgl_30_ps.vcs";
            string filenamepath = $@"{PCGL_DIR_NOT_CORE}/global_lit_simple_pcgl_30_vs.vcs";

            ShaderFile shaderFile = InstantiateShaderFile(filenamepath);
            // Console.WriteLine($"{shaderFile.zframesLookup.Count}");

            int sourceReferencesCount = 0;
            Dictionary<string, int> sourceLookup = new();
            int zframecount = 0;

            for (int zframeIndex = 0; zframeIndex < shaderFile.GetZFrameCount(); zframeIndex++)
            {
                zframecount++;
                if (zframecount % 1000 == 0)
                {
                    Console.WriteLine($"{zframecount}");
                }

                ZFrameFile zframeFile = shaderFile.GetZFrameFileByIndex(zframeIndex);
                sourceReferencesCount += zframeFile.gpuSourceCount;
                foreach (var item in zframeFile.gpuSources)
                {
                    string glslSourceId = item.GetEditorRefIdAsString();
                    sourceLookup.TryGetValue(item.GetEditorRefIdAsString(), out int count);
                    sourceLookup[glslSourceId] = count + 1;
                }
            }


            Console.WriteLine($"source references = {sourceReferencesCount}");
            SortedDictionary<int, int> referenceDistribution = new();

            foreach (var item in sourceLookup)
            {
                referenceDistribution.TryGetValue(item.Value, out int count);
                referenceDistribution[item.Value] = count + 1;
            }


        }



        static void StateSummariesValuesSeen()
        {
            // List<string> vcsFiles = new();
            // vcsFiles.Add($@"{PCGL_DIR_NOT_CORE}/multiblend_pcgl_30_ps.vcs");
            List<string> vcsFiles = GetFileSelectionWithLimitedZframes();

            foreach (string vcsFilenamepath in vcsFiles)
            {
                ShaderFile shaderFile = InstantiateShaderFile(vcsFilenamepath);
                for (int zframeIndex = 0; zframeIndex < shaderFile.GetZFrameCount(); zframeIndex++)
                {
                    ZFrameFile zframeFile = shaderFile.GetZFrameFileByIndex(zframeIndex);

                    foreach (int v in zframeFile.trailingSummary)
                    {
                        CollectIntValue(v);
                    }



                    // leadSummary values - some strange values encountered here
                    //if (zframeFile.leadSummary == null) {
                    //    continue;
                    //}
                    //foreach (int v in zframeFile.leadSummary) {
                    //    CollectIntValue(v);
                    //    if (v == 41) {
                    //        Console.WriteLine($"{RemoveBaseDir(vcsFilenamepath)} {zframeFile.zframeId:x}");
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
        static void DifferencesInSuccessiveH0H2()
        {
            List<string> vcsFiles = new();
            vcsFiles.Add($@"{PCGL_DIR_NOT_CORE}/multiblend_pcgl_30_ps.vcs");
            // List<string> vcsFiles = GetFileSelectionWithLimitedZframes();

            foreach (string vcsFilenamepath in vcsFiles)
            {
                ShaderFile shaderFile = InstantiateShaderFile(vcsFilenamepath);
                for (int zframeIndex = 0; zframeIndex < shaderFile.GetZFrameCount(); zframeIndex++)
                {
                    ZFrameFile zframeFile = shaderFile.GetZFrameFileByIndex(zframeIndex);

                    for (int i = 1; i < zframeFile.dataBlocks.Count; i++)
                    {

                        int h2last = zframeFile.dataBlocks[i - 1].h2;
                        int h2cur = zframeFile.dataBlocks[i].h2;

                        if (h2cur > 0 && h2last > 0)
                        {
                            // Console.WriteLine($"{h2last-h2cur}");
                            CollectIntValue(h2last - h2cur);
                        }


                        //if (diff == 4) {
                        //    Console.WriteLine($"{RemoveBaseDir(vcsFilenamepath)} {zframeFile.zframeId:x} " +
                        //        $"{zframeFile.dataBlocks[i].blockId}");
                        //}
                    }
                }
            }
        }



        static void ZFrameHeaderComparisonGivenFile()
        {
            List<string> vcsFiles = new();
            // vcsFiles.Add($@"{PCGL_DIR_NOT_CORE}/multiblend_pcgl_30_ps.vcs");
            vcsFiles.Add($@"{PCGL_DIR_NOT_CORE}/multiblend_pcgl_30_vs.vcs");

            // List<string> vcsFiles = GetFileSelectionWithLimitedZframes();
            foreach (string vcsFilenamepath in vcsFiles)
            {
                ShaderFile shaderFile = InstantiateShaderFile(vcsFilenamepath);
                for (int zframeIndex = 0; zframeIndex < shaderFile.GetZFrameCount(); zframeIndex++)
                {
                    ZFrameFile zframeFile = shaderFile.GetZFrameFileByIndex(zframeIndex);
                    // Console.WriteLine($"{zframeFile.GetZFrameHeaderStringDescription()}");
                    // Console.WriteLine($"");
                    // CollectStringValue(zframeFile.GetZFrameHeaderStringDescription());

                    if (zframeFile.zframeParams.Count == 4)
                    {
                        Console.WriteLine($"{zframeFile.zframeId:X}");
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
        static void MultiblendPsHeadersWhereDepthPassIsMissing()
        {
            List<string> vcsFiles = new();
            vcsFiles.Add($@"{PCGL_DIR_NOT_CORE}/multiblend_pcgl_30_ps.vcs");
            // List<string> vcsFiles = GetFileSelectionWithLimitedZframes();
            foreach (string vcsFilenamepath in vcsFiles)
            {
                ShaderFile shaderFile = InstantiateShaderFile(vcsFilenamepath);
                for (int zframeIndex = 0; zframeIndex < shaderFile.GetZFrameCount(); zframeIndex++)
                {
                    ZFrameFile zframeFile = shaderFile.GetZFrameFileByIndex(zframeIndex);
                    if (zframeFile.zframeParams.Count == 5)
                    {
                        Console.WriteLine($"{zframeFile.zframeId:X}");
                    }
                }
            }
        }



        static void CheckB0ToB3Uniqeness()
        {
            List<string> vcsFiles = GetFileSelectionWithLimitedZframes();
            foreach (string vcsFilenamepath in vcsFiles)
            {
                ShaderFile shaderFile = InstantiateShaderFile(vcsFilenamepath);
                for (int zframeIndex = 0; zframeIndex < shaderFile.GetZFrameCount(); zframeIndex++)
                {
                    ZFrameFile zframeFile = shaderFile.GetZFrameFileByIndex(zframeIndex);
                    // Console.WriteLine($"zframeFile {zframeFile.zframeId}");
                    foreach (ZDataBlock dataBlock in zframeFile.dataBlocks)
                    {
                        Dictionary<int, int> boundaryValues = new();
                        if (dataBlock.dataload != null)
                        {
                            for (int i = 0; i < dataBlock.dataload.Length; i += 4)
                            {
                                int b0 = dataBlock.dataload[i];
                                int b1 = dataBlock.dataload[i + 1];
                                int b2 = dataBlock.dataload[i + 2];
                                int b3 = dataBlock.dataload[i + 3];
                                int combinedValue = (b3 << 24) + (b2 << 16) + (b1 << 8) + b0;
                                boundaryValues.TryGetValue(combinedValue, out int count);
                                if (count > 0)
                                {
                                    Console.WriteLine($"repeated value found");
                                    Console.WriteLine($"{RemoveBaseDir(vcsFilenamepath)} {zframeFile.zframeId:x} {dataBlock.blockId}");
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
        static void CheckB0ValueAgainstParameterCount()
        {
            // List<string> vcsFiles = new();
            // vcsFiles.Add($@"{PCGL_DIR_NOT_CORE}/multiblend_pcgl_30_ps.vcs");

            List<string> vcsFiles = GetFileSelectionWithLimitedZframes();
            foreach (string vcsFilenamepath in vcsFiles)
            {
                ShaderFile shaderFile = InstantiateShaderFile(vcsFilenamepath);
                int paramCount = shaderFile.paramBlocks.Count;
                Console.WriteLine($"{RemoveBaseDir(vcsFilenamepath)} param-count={paramCount}");
                for (int zframeIndex = 0; zframeIndex < shaderFile.GetZFrameCount(); zframeIndex++)
                {
                    ZFrameFile zframeFile = shaderFile.GetZFrameFileByIndex(zframeIndex);
                    foreach (ZDataBlock dataBlock in zframeFile.dataBlocks)
                    {
                        if (dataBlock.dataload != null)
                        {
                            for (int i = 0; i < dataBlock.dataload.Length; i += 4)
                            {
                                if (dataBlock.dataload[i] > paramCount)
                                {
                                    Console.WriteLine($"b0 val exceeded param count");
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
        static void CheckB0ValuesUniqeness()
        {
            // List<string> vcsFiles = new();
            // vcsFiles.Add($@"{PCGL_DIR_NOT_CORE}/multiblend_pcgl_30_vs.vcs");

            List<string> vcsFiles = GetFileSelectionWithLimitedZframes();
            foreach (string vcsFilenamepath in vcsFiles)
            {
                ShaderFile shaderFile = InstantiateShaderFile(vcsFilenamepath);
                for (int zframeIndex = 0; zframeIndex < shaderFile.GetZFrameCount(); zframeIndex++)
                {
                    ZFrameFile zframeFile = shaderFile.GetZFrameFileByIndex(zframeIndex);
                    // Console.WriteLine($"zframeFile {zframeFile.zframeId}");
                    foreach (ZDataBlock dataBlock in zframeFile.dataBlocks)
                    {
                        Dictionary<int, int> b0Values = new();
                        if (dataBlock.dataload != null)
                        {
                            for (int i = 0; i < dataBlock.dataload.Length; i += 4)
                            {
                                b0Values.TryGetValue(dataBlock.dataload[i], out int count);
                                if (count > 0)
                                {
                                    Console.WriteLine($"repeated value found");
                                    Console.WriteLine($"{RemoveBaseDir(vcsFilenamepath)} {zframeFile.zframeId:x} {dataBlock.blockId}");
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



        static void ShowB0ValuesSelectedFile()
        {
            // List<string> vcsFiles = GetFileSelectionWithLimitedZframes();
            List<string> vcsFiles = new();
            vcsFiles.Add($@"{PCGL_DIR_NOT_CORE}/multiblend_pcgl_30_vs.vcs");
            foreach (string vcsFilenamepath in vcsFiles)
            {
                ShaderFile shaderFile = InstantiateShaderFile(vcsFilenamepath);
                // Dictionary<int, int> b0Values = new();
                for (int zframeIndex = 0; zframeIndex < shaderFile.GetZFrameCount(); zframeIndex++)
                {
                    ZFrameFile zframeFile = shaderFile.GetZFrameFileByIndex(zframeIndex);
                    Console.WriteLine($"zframeFile {zframeFile.zframeId}");
                    foreach (ZDataBlock dataBlock in zframeFile.dataBlocks)
                    {
                        if (dataBlock.dataload != null)
                        {
                            for (int i = 0; i < dataBlock.dataload.Length; i += 4)
                            {
                                Console.Write($"{dataBlock.dataload[i]:X02} ");
                            }
                            Console.WriteLine("");
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
        static void SurveryEndBlockAndGlslSourceCounts()
        {
            List<string> vcsFiles = GetFileSelectionWithLimitedZframes();
            foreach (string vcsFilenamepath in vcsFiles)
            {
                ShaderFile shaderFile = InstantiateShaderFile(vcsFilenamepath);
                for (int i = 0; i < shaderFile.GetZFrameCount(); i++)
                {
                    ZFrameFile zframeFile = shaderFile.GetZFrameFileByIndex(i);


                    // CollectStringValue($"{zframeFile.glslSourceCount}  {zframeFile.nrEndBlocks}");
                    if (zframeFile.nrEndBlocks != zframeFile.gpuSourceCount)
                    {
                        Console.WriteLine($"{RemoveBaseDir(vcsFilenamepath)} {zframeFile.zframeId:x}");
                        break;
                    }


                    // CollectStringValue($"{zframeFile.nonZeroDataBlockCount}  {zframeFile.nrEndBlocks}");

                    //if (zframeFile.nonZeroDataBlockCount == 0) {
                    //    Console.WriteLine($"{vcsFilenamepath}");
                    //    Console.WriteLine($"{zframeFile.zframeId}");
                    //}

                    //if (zframeFile.zframeId != 0) {
                    //    CollectStringValue($"{zframeFile.nonZeroDataBlockCount}  {zframeFile.nrEndBlocks}");
                    //}


                }
            }
        }



        static void DataBlockCountSurvey()
        {
            List<string> vcsFiles = GetFileSelectionWithLimitedZframes();
            foreach (string vcsFilenamepath in vcsFiles)
            {
                ShaderFile shaderFile = InstantiateShaderFile(vcsFilenamepath);
                for (int i = 0; i < shaderFile.GetZFrameCount(); i++)
                {
                    ZFrameFile zframeFile = shaderFile.GetZFrameFileByIndex(0);
                    CollectIntValue(zframeFile.dataBlocks.Count);
                }
            }
        }



        static void DataBlockCountSelectedfile()
        {
            // string filenamepath = $@"{PCGL_DIR_NOT_CORE}/spritecard_pcgl_30_ps.vcs";
            // string filenamepath = $@"{PCGL_DIR_NOT_CORE}/multiblend_pcgl_30_ps.vcs";
            string filenamepath = $@"{PCGL_DIR_NOT_CORE}/multiblend_pcgl_30_vs.vcs";
            ShaderFile shaderFile = InstantiateShaderFile(filenamepath);
            for (int i = 0; i < shaderFile.GetZFrameCount(); i++)
            {
                if (i % 1000 == 0)
                {
                    Console.WriteLine($"{i}");
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
        static void SurverH0LeadingData()
        {
            List<string> vcsFiles = GetFileSelectionWithLimitedZframes();
            foreach (string vcsFilenamepath in vcsFiles)
            {
                ShaderFile shaderFile = InstantiateShaderFile(vcsFilenamepath);
                for (int zframeId = 0; zframeId < shaderFile.GetZFrameCount(); zframeId++)
                {
                    ZFrameFile zframeFile = shaderFile.GetZFrameFileByIndex(zframeId);
                    CollectIntValue(zframeFile.leadingData.h0);
                    if (zframeFile.leadingData.h0 == 0)
                    {
                        Console.WriteLine($"{ShortHandName(vcsFilenamepath),-70} {zframeFile.zframeId:x}");
                        //foreach (ZDataBlock zBlock in zframeFile.dataBlocks) {
                        //    Console.WriteLine($"{zBlock.h0}");
                        //}
                    }
                }
            }
        }


        /*
         * The value of the h1 argument for all non-leading datablocks is 0
         *
         */
        static void SurverH1H2ValuesInDatablocks()
        {
            List<string> vcsFiles = GetFileSelectionWithLimitedZframes();
            foreach (string vcsFilenamepath in vcsFiles)
            {
                ShaderFile shaderFile = InstantiateShaderFile(vcsFilenamepath);
                for (int zframeId = 0; zframeId < shaderFile.GetZFrameCount(); zframeId++)
                {
                    ZFrameFile zframeFile = shaderFile.GetZFrameFileByIndex(zframeId);

                    if (zframeFile.leadingData.h1 != 0)
                    {
                        CollectIntValue(zframeFile.leadingData.h2 - zframeFile.leadingData.h1);
                        // Console.WriteLine($"{RemoveBaseDir(vcsFilenamepath)} {zframeFile.zframeId:x}");
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
        static void SurveryH1ValuesInDatablocks()
        {
            List<string> vcsFiles = GetFileSelectionWithLimitedZframes();
            foreach (string vcsFilenamepath in vcsFiles)
            {
                ShaderFile shaderFile = InstantiateShaderFile(vcsFilenamepath);
                for (int i = 0; i < shaderFile.GetZFrameCount(); i++)
                {
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
        static void SurveryH0H2RelationshipInSpriteCardPs()
        {
            // string filenamepath = $@"{PCGL_DIR_NOT_CORE}/multiblend_pcgl_30_ps.vcs";
            string filenamepath = $@"{PCGL_DIR_NOT_CORE}/spritecard_pcgl_30_ps.vcs";
            ShaderFile shaderFile = InstantiateShaderFile(filenamepath);
            for (int i = 0; i < shaderFile.GetZFrameCount(); i++)
            {
                if (i % 1000 == 0)
                {
                    Console.WriteLine($"{i}");
                }
                ZFrameFile zframeFile = shaderFile.GetZFrameFileByIndex(i);
                CollectIntValue(zframeFile.leadingData.h0 - zframeFile.leadingData.h2);
                foreach (ZDataBlock zBlock in zframeFile.dataBlocks)
                {
                    CollectIntValue(zBlock.h0 - zBlock.h2);
                }
            }
        }



        static void SurveryHeaderParams()
        {
            List<string> vcsFiles = GetFileSelectionWithLimitedZframes();
            foreach (string vcsFilenamepath in vcsFiles)
            {
                ShaderFile shaderFile = InstantiateShaderFile(vcsFilenamepath);
                List<ZFrameFile> zframeFiles = new();
                for (int i = 0; i < shaderFile.GetZFrameCount(); i++)
                {
                    zframeFiles.Add(shaderFile.GetZFrameFileByIndex(i));
                }
                foreach (ZFrameFile zframeFile in zframeFiles)
                {

                    // zframeFile.ShowZFrameHeader();
                    foreach (var zparam in zframeFile.zframeParams)
                    {
                        CollectStringValue(zparam.ToString());
                    }
                }
            }
        }



        static void SurveryBytesInLeadingDataload()
        {
            List<string> vcsFiles = GetFileSelectionWithLimitedZframes();
            foreach (string vcsFilenamepath in vcsFiles)
            {
                ShaderFile shaderFile = InstantiateShaderFile(vcsFilenamepath);
                List<ZFrameFile> zframeFiles = new();
                for (int i = 0; i < shaderFile.GetZFrameCount(); i++)
                {
                    zframeFiles.Add(shaderFile.GetZFrameFileByIndex(i));
                }
                foreach (ZFrameFile zframeFile in zframeFiles)
                {
                    for (int i = 3; i < zframeFile.leadingData.dataload.Length; i += 4)
                    {
                        if (zframeFile.leadingData.dataload[i] == 0)
                        {
                            CollectStringValue($"{zframeFile.leadingData.dataload[i - 1]:X02} {zframeFile.leadingData.dataload[i]:X02}");
                            if (zframeFile.leadingData.dataload[i - 1] == 0x35 && zframeFile.leadingData.dataload[i] == 0)
                            {
                                Console.WriteLine($"{zframeFile.filenamepath}");
                                Console.WriteLine($"{zframeFile.zframeId}");
                                goto breakhere;
                            }
                        }
                    }
                }
            }
breakhere: Console.WriteLine("");
        }



        /*
         * A big problem with surverying zframes is that it creates insane workloads
         * decompressing and parsing hero_pcgl_30_ps.vcs taking 20 GB of data.
         * I've setup a selection that chooses files with 4000 zframes or less only, which is pretty manageable
         *
         *
         */
        static void SurveryLeadingDataSingleFile()
        {

            List<string> vcsFiles = GetFileSelectionWithLimitedZframes();
            foreach (string vcsFilenamepath in vcsFiles)
            {
                ShaderFile shaderFile = InstantiateShaderFile(vcsFilenamepath);
                List<ZFrameFile> zframeFiles = new();
                for (int i = 0; i < shaderFile.GetZFrameCount(); i++)
                {
                    zframeFiles.Add(shaderFile.GetZFrameFileByIndex(i));
                }
                foreach (ZFrameFile zframeFile in zframeFiles)
                {
                    CollectStringValue($"{zframeFile.leadingData.h0,2} {zframeFile.leadingData.h1,2} {zframeFile.leadingData.h2,2}");
                }
            }
        }





        static List<string> GetFileSelectionWithLimitedZframes()
        {
            List<string> vcsFiles = new();
            List<string> selectedFiles = GetVcsFiles(PCGL_DIR_CORE, PCGL_DIR_NOT_CORE, VcsProgramType.Undetermined, 30);
            // List<string> selectedFiles = GetVcsFiles(PCGL_DIR_CORE, PCGL_DIR_NOT_CORE, VcsProgramType.Undetermined, -1);
            // List<string> selectedFiles = GetVcsFiles(PC_DIR_CORE, PC_DIR_NOT_CORE, VcsProgramType.Undetermined, -1);
            // List<string> selectedFiles = GetVcsFiles(ARTIFACT_CLASSIC_CORE_PC_SOURCE, ARTIFACT_CLASSIC_DCG_PC_SOURCE, VcsProgramType.Undetermined, -1);
            // List<string> selectedFiles = GetVcsFiles(DOTA_CORE_MOBILE_GLES_SOURCE, DOTA_DAC_MOBILE_GLES_SOURCE, VcsProgramType.Undetermined, -1);

            // something wrong in the vulkan source
            // List<string> selectedFiles = GetVcsFiles(HLALYX_CORE_VULKAN_SOURCE, HLALYX_HLVR_VULKAN_SOURCE, VcsProgramType.Undetermined, -1);

            foreach (string checkVcsFile in selectedFiles)
            {
                ShaderFile shaderFile = null;
                try
                {
                    shaderFile = InstantiateShaderFile(checkVcsFile);
                } catch (Exception)
                {
                    continue;
                }
                if (shaderFile.GetZFrameCount() < 4000 && shaderFile.GetZFrameCount() > 0)
                {
                    vcsFiles.Add(checkVcsFile);
                }
            }

            return vcsFiles;
        }

        // needs work! currently returning null. perhaps add interesting test cases as I go along
        static List<string> GetManualFileSelection()
        {
            // don't do this one yet!
            // string filenamepath = $@"{PCGL_DIR_NOT_CORE}/hero_pcgl_30_ps.vcs";
            List<string> vcsFiles = new();
            vcsFiles.Add($@"{PCGL_DIR_NOT_CORE}/multiblend_pcgl_30_ps.vcs");
            vcsFiles.Add($@"{PCGL_DIR_NOT_CORE}/multiblend_pcgl_30_vs.vcs");
            vcsFiles.Add($@"{PCGL_DIR_NOT_CORE}/grasstile_pcgl_41_ps.vcs");
            vcsFiles.Add($@"{PCGL_DIR_NOT_CORE}/grasstile_pcgl_41_vs.vcs");
            return null;
        }







        private static StreamWriter sw = null;
        private static bool DisableOutput = false;
        private static bool WriteAsHtml = false;
        private static bool swWriterAlreadyClosed = false;


        // This basestream != null is nonsense, it doesn't check if the file is open
        private static void CloseStreamWriter()
        {
            if (WriteAsHtml && !swWriterAlreadyClosed)
            {
                sw.WriteLine(GetHtmlFooter());
            }
            if (sw != null && !swWriterAlreadyClosed)
            {
                sw.Close();
            }
        }

        public static void OutputWrite(string text)
        {
            if (!DisableOutput)
            {
                Console.Write(text);
            }
            if (sw != null)
            {
                sw.Write(text);
            }
        }
        public static void OutputWriteLine(string text)
        {
            OutputWrite(text + "\n");
        }


    }



}







