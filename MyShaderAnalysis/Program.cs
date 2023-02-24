using System;
using System.Diagnostics;
using System.IO;
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

/*
-- suppress new warnings and suggestions
#pragma warning disable IDE0003 // Remove qualification
#pragma warning disable IDE0011 // Add braces
#pragma warning disable IDE0017 // Simplify object initialization
#pragma warning disable IDE0028 // Simplify collection initialization
#pragma warning disable IDE0031 // Use null propagation
#pragma warning disable IDE0033 // Use explicitly provided tuple name
#pragma warning disable IDE0039 // Use local function
#pragma warning disable IDE0044 // Add readonly modifier
#pragma warning disable IDE0049 // Simplify Names
#pragma warning disable IDE0051 // Remove unused private members
#pragma warning disable IDE0052 // Remove unread private members
#pragma warning disable IDE0057 // Use range operator
#pragma warning disable IDE0059 // Unnecessary assignment of a value
#pragma warning disable IDE0060 // Remove unused parameter
#pragma warning disable IDE0066 // Convert switch statement to expression
#pragma warning disable IDE0071 // Simplify interpolation
#pragma warning disable IDE0074 // Use compound assignment
#pragma warning disable IDE0090 // Use 'new(...)'
#pragma warning disable IDE1006 // Naming Styles
#pragma warning disable CA1024 // Use properties where appropriate
#pragma warning disable CA1051 // Do not declare visible instance fields
#pragma warning disable CA1508 // Avoid dead conditional code
#pragma warning disable CA1822 // Mark members as static
#pragma warning disable CA1823 // Avoid unused private fields
#pragma warning disable CS0219 // Variable is assigned but its value is never used
#pragma warning disable CA2211 // Non-constant fields should not be visible
*/
namespace MyShaderAnalysis
{
    public class Program
    {
        public class ToDebugWriter : StringWriter
        {
            public override void WriteLine(string value)
            {
                Debug.WriteLine(value);
            }

            public override void Write(string value)
            {
                Debug.Write(value);
            }
        }

        public static void Main()
        {
            Console.SetOut(new ToDebugWriter());

            // AnalysisKristiker();
            // StaticAnalysisForKristiker(); // analyse zfrme data ahead of parsing
            // StaticAnalysis();
            // SingleFileParserExamples.RunTrials();
            // BatchProcessFiles();
            // ServerHtml(); // <-- create server files
            // FileArchives();
            // Parsing();
            // OldStaticAnalysisSummaries(); // garbage
            // BasicFunctionalitySnippets();
            CSharpSnippets();
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
            // StaticAnalysis1.RunTrials();
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
            CreateHtmlServerFiles.RunTrials();
            // TestWriteHtmlFile.Run();
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

        public static void VulkanReflect()
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
            // CrcTrialsMurmur32.RunTrials();
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

