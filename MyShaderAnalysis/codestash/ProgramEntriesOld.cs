using System;
using System.Collections.Generic;
using static MyShaderAnalysis.codestash.MyTrashUtilHelpers;

/*
 *
 * The trio of methods given in ProduceServerSet() producing a full html server pintout functioning correctly at the commit
 *
 * 28 March 2022 - 'separated out old files for refactoring'
 *
 *
 *
 */
namespace MyShaderAnalysis.codestash
{
    class ProgramEntriesOld
    {
        public static void RunTrials()
        {
            // ProduceServerSet();

            // -- fall 2021
            // DataReaderV62.RunTrials();
            // TestMobileShaderFiles.RunTrials();
            // StaticAnalysisZframes.RunTrials();
            // StaticAnalysis2.RunTrials();
            StaticAnalysis.RunTrials();
            // LzmaAnalysis.RunTrials();

            // TestUtilFunctionsOld.RunTrials();

            // -- truly old code (doesn't work any longer)
            // ShaderAnalysis.RunTrials();

            // -- get related files util method
            // TestGetRelatedFiles();
        }

        private static void ProduceServerSet()
        {
            // Taken together these three will print all (limited to first few) related files for the dotagame_pcgl set
            // -- vcs and zframe byte detail + glsl source
            PrintoutsByteAnalysis.RunTrials();
            // -- formatted zframes
            PrintoutsZframes1.RunTrials();
            // -- formatted vcs
            ParseVcsFilesOld1.RunTrials();
        }

        private static void TestGetRelatedFiles()
        {
            var filenamepath = @"X:\dota-2-VRF-exports\dota2-export-shaders-pcgl\shaders\vfx\water_dota_pcgl_40_features.vcs";
            // string[] relatedFiles = GetRelatedFiles(filenamepath);
            List<string> relatedFiles = GetRelatedFiles2(filenamepath);
            foreach (string relatedFile in relatedFiles)
            {
                Console.WriteLine($"{relatedFile}");
            }
        }
    }
}
