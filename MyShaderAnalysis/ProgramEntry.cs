using System;
using System.IO;
using System.Diagnostics;
using ValveResourceFormat.CompiledShader;
using System.Collections.Generic;
using System.Linq;
using MyShaderAnalysis.codestash;
using MyShaderAnalysis.codestash.snippetcode;
using static MyShaderAnalysis.codestash.MyShaderUtilHelpers;
using static MyShaderAnalysis.codestash.FileSystemOld;
using static MyShaderAnalysis.codestash.ReadShaderFile;
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


            // -- comprehensive tests against archive names (all files)
            TestBatchParsing.RunTrials();
            // TestSingleFileParsing.RunTrials();

            // -- revise how files are managed as collections
            // CreateHtmlServerFiles.RunTrials();


            // DemoCode.RunTrials();
            // PrintoutVulkanCode.RunTrials();
            // -- Looking at dynamic expressions
            // ParseDynamicExpression.RunTrials();



            // -- fall 2021
            // there is variously still some useful code among these files ..
            // ProgramEntriesOld.RunTrials();


            // -- testing code
            // TestFileUtilFunctions.RunTrials();
            // RegexExamples.RunTrials();
            // Snippetcode.RunTrials();
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



