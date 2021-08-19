


using MyShaderAnalysis.compat;
using MyShaderAnalysis.utilhelpers;
using System.Diagnostics;
using System.IO;
using static MyShaderAnalysis.utilhelpers.FileSystem;

namespace MyShaderAnalysis {


    class ProgramEntry {

        static void Main() {


            // FileTokens vcsFile = new FileTokens(ARCHIVE.dotagame_pcgl, "water_dota_pcgl_30_vs.vcs");
            // FileTokens vcsFile = new(ARCHIVE.dotagame_pcgl, "global_lit_simple_pcgl_30_vs.vcs");
            // Debug.WriteLine($"{vcsFile.GetBestZframesLink(0x15d)}");




            // LzmaAnalysis.RunTrials();
            // TestFileSystem.RunTrials();

            StaticAnalysisZframes2.RunTrials();
            // StaticAnalysisZframes.RunTrials();

            // StaticAnalysis2.RunTrials();
            // StaticAnalysis.RunTrials();

            // ShaderAnalysis.RunTrials();
            // ShaderAnalysis03.RunTrials();
            // ShaderAnalysis02.RunTrials(); // filewriting here still works (set to write to file)
            // ShaderAnalysis01.RunTrials();

            // ShaderTesting01.RunTrials();
            // Snippetcode.RunTrials();



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



