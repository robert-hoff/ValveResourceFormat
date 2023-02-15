using System;
using System.IO;
using System.Diagnostics;
using ValveResourceFormat.CompiledShader;
using System.Collections.Generic;
using System.Linq;
using MyShaderAnalysis.codestash;
using static ValveResourceFormat.CompiledShader.ShaderUtilHelpers;
using MyShaderAnalysis.utilhelpers.snippetcode;
using MyShaderAnalysis.compat;
using MyShaderAnalysis.batchtesting1;
using MyShaderAnalysis.batchtesting2;
using MyShaderAnalysis.utilhelpers.parsetrials;
using System.Globalization;
using MyShaderAnalysis.staticanalysis;

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

            // Jan 2023
            RunStaticAnalysis();
            // BatchProcessFiles();
            // OldStaticAnalysisSummaries();

            // Earlier
            // BatchTesting1()
            // BatchProcessHtmlServerFiles(); // <-- create server files
            // ParserSnippets();
            // BasicFunctionalitySnippets();
        }


        public static void RunStaticAnalysis()
        {
            StaticAnalysis3.Run();
            // StaticAnalysis2.RunTrials();
            // StaticAnalysis.RunTrials();
        }


        public static void BatchProcessFiles()
        {
            TestSpirvReflection.Run();
            // TestBatchQueries.Run();

            // from an earlier effort analysing the data in zframes
            // I'd like to do something similar for the Vulkan source code ..
            // StaticAnalysisZframes.RunTrials();
        }


        // TestSingleFileParsing.RunTrials();

        // -- "comprehensive" tests against archive names (all files)
        // R: yea, but not very user friendly!
        public static void BatchTesting1()
        {
            // 2022 sometime ..
            TestFileArchive.RunTrials();
            // TestBatchParsing.RunTrials();

            // R: Jan 2023 - cannot find this file
            // TestSpirvDll.RunTrials();

            // -- fall 2021
            // there is variously still some useful code among these files ..
            // ProgramEntriesOld.RunTrials();
        }

        public static void BatchProcessHtmlServerFiles()
        {
            // -- revise how files are managed as collections
            CreateHtmlServerFiles.RunTrials();
        }


        public static void ParserSnippets()
        {
            SingleFileParserExamples.RunTrials();
            // PrintoutVulkanCode.RunTrials();
        }


        public static void OldStaticAnalysisSummaries()
        {
            // -- all garbage
            StaticAnalysis.RunTrials();

            // StaticAnalysis2.RunTrials();
        }


        public static void BasicFunctionalitySnippets()
        {
            // ByteSequenceToString.Run();
            // -- Looking at dynamic expressions
            // ParseDynamicExpression.RunTrials();
            // EncodeZstdDictAsString.RunTrials();
            // CrcTrialsSha1.RunTrials();
            // CrcTrialsMurmur32.RunTrials();
        }

        public static void CSharpSnippets()
        {
            // RegexExamples.RunTrials();
            Snippetcode.RunTrials();
        }

        public static void TestConstraintRules()
        {
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



