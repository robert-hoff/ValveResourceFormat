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


            // X:\dota-2-VRF-exports\dota2-export-shaders-pcgl\shaders\vfx\water_dota_pcgl_40_features.vcs
            // var filenamepath = @"X:\dota-2-VRF-exports\dota2-export-shaders-pcgl\shaders\vfx\water_dota_pcgl_40_features.vcs";
            // string[] relatedFiles = GetRelatedFiles(filenamepath);
            // List<string> relatedFiles = GetRelatedFiles2(filenamepath);

            // CollectConfigurations.RunTrials();


            // DataReaderV62.RunTrials();
            // TestMobileShaderFiles.RunTrials();

            // PrintoutsZframes1.RunTrials();
            // StaticAnalysisZframes.RunTrials();

            // StaticAnalysis2.RunTrials();
            // StaticAnalysis.RunTrials();

            // PrintoutsByteAnalysis.RunTrials();
            // ShaderAnalysis.RunTrials();



            RunPrintoutsSingleFile.RunTrials();
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



