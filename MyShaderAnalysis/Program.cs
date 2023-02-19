using MyShaderAnalysis.batchtesting;
using MyShaderAnalysis.codestash;
using MyShaderAnalysis.filearchive;
using MyShaderAnalysis.parsing;
using MyShaderAnalysis.serverhtml;
using MyShaderAnalysis.snippetcode;
using MyShaderAnalysis.staticanalysis;
using MyShaderAnalysis.util;
using MyShaderAnalysis.vulkanreflect;
using MyShaderFileKristiker.MyHelperClasses.ProgEntries;
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

            // AnalysisKristiker();
            // StaticAnalysisForKristiker(); // analyse zfrme data ahead of parsing
            // RunStaticAnalysis();
            // BatchProcessFiles();
            // ServerHtml(); // <-- create server files
            // FileArchives();
            Parsing();
            // OldStaticAnalysisSummaries(); // garbage
            // BasicFunctionalitySnippets();
            // CSharpSnippets();
            // TestConstraintRules();
        }

        // classes using the Kristiker shaders *only*
        public static void AnalysisKristiker()
        {
            RunKristiker.RunTrials();
        }

        public static void StaticAnalysisForKristiker()
        {
            StaticAnalysisZframes.RunTrials();
            // StaticAnalysis.RunTrials();
            // StaticAnalysis2.RunTrials();
        }

        public static void StaticAnalysis()
        {
            StaticAnalysis3.Run();
            // StaticAnalysisZframes.RunTrials();

            // -- tidy/refactor-out needed on these
            // StaticAnalysis2.RunTrials();
            // StaticAnalysis.RunTrials();
        }

        public static void BatchProcessFiles()
        {
            // BatchParsing2.RunTrials();
            // -- not logical to have this item in batch processing
            // TestSingleFileParsing.RunTrials();

            // -- 2022 sometime (this one is still relevant)
            BatchParsing1.RunTrials();
            // TIDY NEEDED HERE
            // -- fall 2021
            // there is variously still some useful code among these files ..
            // ProgramEntriesOld.RunTrials();
        }

        public static void ServerHtml()
        {
            // -- revise how files are managed as collections
            // CreateHtmlServerFiles.RunTrials();
            TestWriteHtmlFile.Run();
        }

        public static void FileArchives()
        {
            // SomeRandomTesting.RunTrials();
            TestFileArchive.RunTrials();
        }

        public static void Parsing()
        {
            SingleFileParserExamples.RunTrials();
            // PrintoutVulkanCode.RunTrials();
        }

        public static void vulkanReflect()
        {
            TestSpirvReflection.Run();
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

