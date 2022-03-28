using System;
using System.IO;
using System.Diagnostics;
using ValveResourceFormat.CompiledShader;
using System.Collections.Generic;
using System.Linq;
using MyShaderAnalysis.utilhelpers;
using MyShaderAnalysis.utilhelpers.snippetcode;
using static MyShaderAnalysis.utilhelpers.MyShaderUtilHelpers;
using static MyShaderAnalysis.utilhelpers.FileSystem;
using static MyShaderAnalysis.utilhelpers.ReadShaderFile;
using static ValveResourceFormat.CompiledShader.ShaderUtilHelpers;


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


            // -- process all types through single entry
            ParseVcsFiles.RunTrials();



            // Methods for building html printouts
            // -- vcs and zframe byte detail
            // PrintoutsByteAnalysis.RunTrials();
            // // -- formatted zframes
            // PrintoutsZframes1.RunTrials();
            // -- formatted vcs
            // ParseVcsFiles.RunTrials();



            // 23 March 2022
            // DemoCode.RunTrials();

            // -- This still produces a file summary overview (particularly for the static variables)
            // StaticAnalysis.RunTrials();


            // 18 March 2022
            // -- support for v62 was already written into DataReaderVcsByteAnalysis
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
            // StaticAnalysisZframes.RunTrials();
            // StaticAnalysis2.RunTrials();
            // StaticAnalysis.RunTrials();


            // ShaderAnalysis.RunTrials();



            // -- testing code
            // RegexExamples.RunTrials();
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



