using System.Diagnostics;
using System.IO;
using MyShaderAnalysis.compat;
using MyShaderAnalysis.utilhelpers;
using MyShaderAnalysis.utilhelpers.snippetcode;
using static MyShaderAnalysis.utilhelpers.FileSystem;
using static MyShaderAnalysis.vcsparsing.ShaderUtilHelpers;

namespace MyShaderAnalysis
{


    class ProgramEntry
    {

        static void Main()
        {

            // PrintoutsZframes1.RunTrials();
            // StaticAnalysisZframes.RunTrials();

            // PrintoutsSingleFile.RunTrials();
            // StaticAnalysis2.RunTrials();
            // StaticAnalysis.RunTrials();



            // PrintoutsByteAnalysis.RunTrials();
            // ShaderAnalysis.RunTrials();

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


            Trial1();

        }


        static void Trial1()
        {
            int[] sconf = { 1, 2, 1 };

            SfBlockConfigurationMapping rules = new SfBlockConfigurationMapping(sconf);
            for (int i = 0; i < rules.MaxEnumeration(); i++)
            {
                int[] state = rules.GetConfigState(i);
                ShowIntArray(state);
            }

            // rules.ShowOffsetAndLayersArrays();

        }


    }



}



