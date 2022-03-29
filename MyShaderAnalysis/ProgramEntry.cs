using System;
using System.IO;
using System.Diagnostics;
using ValveResourceFormat.CompiledShader;
using System.Collections.Generic;
using System.Linq;
using MyShaderAnalysis.utilhelpers;
using MyShaderAnalysis.utilhelpers.snippetcode;
using static MyShaderAnalysis.utilhelpers.MyShaderUtilHelpers;
using static MyShaderAnalysis.utilhelpers.FileSystemOld;
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


            // -- revise how files are managed as collections
            CreateHtmlServerFiles.RunTrials();


            // -- process all types through single entry
            // ParseVcsFiles.RunTrials();

            // -- test v62 parsers here,
            // DemoCode.RunTrials();

            // -- This still produces a file summary overview (particularly for the static variables)
            // StaticAnalysis.RunTrials();


            // -- support for v62 was already written into DataReaderVcsBytes
            // ParseV62Files.RunTrials();

            // PrintoutVulkanCode.RunTrials();
            // ParseV65Files.RunTrials();

            // Looking at a dynamic expression
            // ParseDynamicExpression.RunTrials();


            // -- fall 2021
            // ProgramEntriesOld.RunTrials();



            // -- testing code
            // TestFileUtilFunctions.RunTrials();
            // RegexExamples.RunTrials();
            // TestBasicParsing.RunTrials();
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



