using System;
using System.IO;
using System.Diagnostics;
using ValveResourceFormat.CompiledShader;
using static MyShaderAnalysis.utilhelpers.MyShaderUtilHelpers;
using static MyShaderAnalysis.utilhelpers.FileSystem;
using static MyShaderAnalysis.utilhelpers.ReadShaderFile;
using static ValveResourceFormat.CompiledShader.ShaderUtilHelpers;
using System.Collections.Generic;
using System.Linq;
using MyShaderAnalysis.utilhelpers;


namespace MyShaderAnalysis
{
    class ProgramEntry
    {

        public class ToDebugWriter : StringWriter
        {
            public override void WriteLine(string str0)
            {
                Debug.WriteLine(str0);
            }
            public override void Write(string str0)
            {
                Debug.Write(str0);
            }
        }

        static void Main()
        {
            Console.SetOut(new ToDebugWriter());


            // Console.WriteLine($"{0xAF6E}");
            Console.WriteLine($"{0xdc99}");

                

            // 18 March 2022
            // ParseV62Files.RunTrials();


            // 14 March 2022, R: try to read a Vulkan Zframe
            // PrintoutVulkanCode.RunTrials();
            // ParseV65Files.RunTrials();

            // Looking at a dynamic expression
            // ParseDynamicExpression.RunTrials();



            // before March 2022

            // X:\dota-2-VRF-exports\dota2-export-shaders-pcgl\shaders\vfx\water_dota_pcgl_40_features.vcs
            // var filenamepath = @"X:\dota-2-VRF-exports\dota2-export-shaders-pcgl\shaders\vfx\water_dota_pcgl_40_features.vcs";
            // string[] relatedFiles = GetRelatedFiles(filenamepath);
            // List<string> relatedFiles = GetRelatedFiles2(filenamepath);


            // DataReaderV62.RunTrials();
            // TestMobileShaderFiles.RunTrials();

            // PrintoutsZframes1.RunTrials();
            // StaticAnalysisZframes.RunTrials();

            // StaticAnalysis2.RunTrials();
            // StaticAnalysis.RunTrials();

            // support for v62 was written into PrintoutsByteAnalysis.cs
            // PrintoutsByteAnalysis.RunTrials();
            // ShaderAnalysis.RunTrials();


            // RunPrintoutsSingleFile.RunTrials();
            // TestUtilFunctions.RunTrials();
            // TestBasicParsing.RunTrials();

            // Snippetcode.RunTrials();
            // LzmaAnalysis.RunTrials();
            // TestStuff.RunTrials();
            // CrcTrialsSha1.RunTrials();
            // CrcTrialsMurmur32.RunTrials();


            // CompatRulesGenerationTestRuns.RunTrials();
            // CompatRulesGenericStaticClass.RunTrials();
            // CompatRulesMultiframe.RunTrials();
            // CompatRulesWater2.RunTrials();
            // CompatRulesNewApproach.RunTrials();
            // CompatRulesWater.RunTrials();
            // CompatRulesGlobalLitSimple.RunTrials();
            // CompatRules5.RunTrials();
            // CompatRules4.RunTrials();
            // CompatRules3.RunTrials();
            // CompatRules2.RunTrials();
            // CompatRules.RunTrials();
            // BitTestGeneration.RunTrials();



        }
    }
}



