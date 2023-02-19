using MyShaderAnalysis.batchtesting;
using MyShaderAnalysis.codestash;
using MyShaderAnalysis.filearchive;
using MyShaderAnalysis.parsing;
using MyShaderAnalysis.serverhtml;
using MyShaderAnalysis.snippetcode;
using MyShaderAnalysis.staticanalysis;
using MyShaderAnalysis.util;
using MyShaderAnalysis.vulkanreflect;
using System;
using System.Diagnostics;
using System.IO;

namespace MyShaderAnalysis
{
    class Program
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

            // StaticAnalysisForKristiker(); // analyse zfrme data ahead of parsing
            // -- Jan 2023
            RunStaticAnalysis();
            // BatchProcessFiles();
            // -- Earlier
            // HtmlServerDumps(); // <-- create server files
            // TestFileArchives();
            // ParserSnippets();
            // OldStaticAnalysisSummaries(); // garbage
            // BasicFunctionalitySnippets();
            // CSharpSnippets();
            // TestConstraintRules();
        }

        public static void StaticAnalysisForKristiker()
        {
            StaticAnalysisZframes.RunTrials();
            // StaticAnalysis.RunTrials();
            // StaticAnalysis2.RunTrials();
        }

        public static void RunStaticAnalysis()
        {
            StaticAnalysis3.Run();
            // StaticAnalysis2.RunTrials();
            // StaticAnalysis.RunTrials();
        }

        public static void BatchProcessFiles()
        {
            BatchParsing1.RunTrials();
            // TestSpirvReflection.Run();

            // from an earlier effort analysing the data in zframes
            // I'd like to do something similar for the Vulkan source code ..
            // StaticAnalysisZframes.RunTrials();
            // TestSingleFileParsing.RunTrials();

            // -- 2022 sometime ..
            // TestBatchParsing.RunTrials();
            // TIDY NEEDED HERE
            // -- fall 2021
            // there is variously still some useful code among these files ..
            // ProgramEntriesOld.RunTrials();
        }

        public static void HtmlServerDumps()
        {
            // -- revise how files are managed as collections
            CreateHtmlServerFiles.RunTrials();
        }

        public static void TestFileArchives()
        {
            // SomeRandomTesting.RunTrials();
            TestFileArchive.RunTrials();
        }

        public static void ParserSnippets()
        {
            SingleFileParserExamples.RunTrials();
            // PrintoutVulkanCode.RunTrials();
        }

        public static void OldStaticAnalysisSummaries()
        {
            // TIDY NEEDED HERE
            // -- all garbage
            // StaticAnalysis.RunTrials();

            // StaticAnalysis2.RunTrials();
        }

        public static void BasicFunctionalitySnippets()
        {
            // ByteSequenceToString.Run();
            // -- Looking at dynamic expressions
            ParseDynamicExpression.RunTrials();
            // EncodeZstdDictAsString.RunTrials();
            // CrcTrialsSha1.RunTrials();
            // CrcTrialsMurmur32.RunTrials();
        }

        public static void CSharpSnippets()
        {
            // -- fix some style issues
            FixCsSources.Run();
            // RegexExamples.RunTrials();
            // Snippetcode.RunTrials();
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

