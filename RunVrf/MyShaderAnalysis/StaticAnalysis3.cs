using System.Diagnostics;
using RunVrf.UtilHelpers;
using ValveResourceFormat.CompiledShader;
using ValveResourceFormat.Serialization.VfxEval;
using static RunVrf.MyShaderAnalysis.MyTrashUtilHelpers;


namespace RunVrf.MyShaderAnalysis
{
    internal class StaticAnalysis3
    {

        const VcsProgramType FEAT = VcsProgramType.Features;
        const VcsProgramType VS = VcsProgramType.VertexShader;
        const VcsProgramType PS = VcsProgramType.PixelShader;


        public static void Run()
        {
            ZFrame1();
            // Parameters2();
            // ConfigurationHeaders();
            // MipMapCountFileSets();
            // CheckDBlockCountForFeaturesFiles();
        }



        public static void ZFrame1()
        {

        }


        public static void Parameters2()
        {
            // FileArchive fileArchive = new(ARCHIVE.dota_game_pc_v66, FEAT, VS, PS);
            // FileArchive fileArchive = new(ARCHIVE.dota_core_pc_v66, FEAT, VS, PS);
            // FileArchive fileArchive = new(ARCHIVE.dota_game_pc_v65, FEAT, VS, PS);
            // FileArchive fileArchive = new(ARCHIVE.dota_core_pc_v65, FEAT, VS, PS);
            // FileArchive fileArchive = new(ARCHIVE.dota_game_pcgl_v64, FEAT, VS, PS);
            // FileArchive fileArchive = new(ARCHIVE.alyx_core_vulkan_v64, FEAT, VS, PS);
            // FileArchive fileArchive = new(ARCHIVE.alyx_hlvr_vulkan_v64, FEAT, VS, PS);

            List<FileArchive> fileArchives = new()
            {
                new(ARCHIVE.dota_game_pc_v66, FEAT, VS, PS),
                // fileArchives.Add(new(ARCHIVE.dota_core_pc_v66, FEAT, VS, PS));
                new(ARCHIVE.dota_game_pc_v65, FEAT, VS, PS),
                new(ARCHIVE.dota_core_pc_v65, FEAT, VS, PS),
                new(ARCHIVE.dota_game_pcgl_v64, FEAT, VS, PS),
                new(ARCHIVE.alyx_core_vulkan_v64, FEAT, VS, PS),
                new(ARCHIVE.alyx_hlvr_vulkan_v64, FEAT, VS, PS)
            };

            foreach (var fileArchive in fileArchives)
            {
                foreach (FileVcsTokens vcsFile in fileArchive.GetFileVcsTokens())
                {
                    ShaderFile shaderFile = vcsFile.GetShaderFile();
                    foreach (ParamBlock pBlock in shaderFile.ParamBlocks)
                    {
                        // string reportLine = $"{pBlock.Arg3,-3} {pBlock.Arg4,-3} {pBlock.Arg5,-3} {pBlock.Arg6,-3}";
                        // string reportLine = $"{pBlock.Id,-3:000} {pBlock.Arg9,-3} {pBlock.Arg10,-3} {pBlock.Arg11,-3}";

                        string reportLine = $"{(int) pBlock.Format,3}    {pBlock.Format}";
                        CollectStringValue(reportLine);
                    }
                }
                Debug.WriteLine($"{fileArchive.archive}");
                PrintReport();
            }
        }


        public static void Parameters1()
        {
            FileArchive fileArchive = new(ARCHIVE.dota_game_pcgl_v64, FEAT, VS, PS);
            foreach (FileVcsTokens vcsFile in fileArchive.GetFileVcsTokens())
            {
                ShaderFile shaderFile = vcsFile.GetShaderFile();
                foreach (ParamBlock pBlock in shaderFile.ParamBlocks)
                {
                    // string reportLine = $" {pBlock.Lead0:00} {(int) pBlock.UiType} {(int) pBlock.VfxType} {(int) pBlock.ParamType}";
                    // string reportLine = $" {pBlock.Lead0:00}   {pBlock.Name,-40} {pBlock.UiType,-20} {pBlock.VfxType,-20} {pBlock.ParamType,-20}";
                    // string reportLine = $"{pBlock.Name,-40} {pBlock.Lead0:00}";

                    if (pBlock.Lead0 == 6 || pBlock.Lead0 == 7)
                    {
                        // string reportLine = $"{pBlock.Name,-40} {pBlock.Lead0:00}  {pBlock.UiType,-20} {pBlock.VfxType,-20} {pBlock.ParamType,-20}";
                        // string reportLine = $"{pBlock.Name,-40} {pBlock.Lead0:00}  {pBlock.Arg3,-6}  {pBlock.Arg4,-6}  {pBlock.Arg5,-6}  {pBlock.Arg6,-6}";
                        // string reportLine = $"{pBlock.Name,-40} {pBlock.Lead0:00} {pBlock.VecSize}";
                        // string reportLine = $"{pBlock.Name,-40}  {pBlock.VecSize}";
                        // string reportLine = $"{pBlock.Name,-40}  {pBlock.Id,3}  {pBlock.Arg9,3}  {pBlock.Arg10,3} {pBlock.Arg11,3}";
                        // string reportLine = $"{pBlock.Name,-40}    {pBlock.Lead0:00} {formatInts(pBlock.IntDefs)}";

                        // string reportLine = $"{pBlock.Name,-40}    {pBlock.Lead0:00}   {ParseDynamicExpression(pBlock.DynExp)}";
                        // CollectStringValue(reportLine);
                    }

                    // string reportLine = $"{pBlock.Arg3,-6}  {pBlock.Arg4,-6}  {pBlock.Arg5,-6}  {pBlock.Arg6,-6}";
                    // CollectStringValue(reportLine);

                    //if (pBlock.Arg3 > 0)
                    //{
                    //    string reportLine = $"{pBlock.Arg3,-6}  {pBlock.Name}";
                    //    CollectStringValue(reportLine);
                    //}

                    //if (pBlock.Arg4 > 0)
                    //{
                    //    string reportLine = $"{pBlock.Arg4,-6:000}  {pBlock.Name}";
                    //    CollectStringValue(reportLine);
                    //}

                }
            }
            PrintReport();
        }

        public static string formatInts(int[] args)
        {
            return $"{args[0],3},{args[1],3},{args[2],3},{args[3],3}";
        }



        public static void ConfigurationHeaders()
        {
            FileArchive fileArchive = new(ARCHIVE.dota_game_pcgl_v64, FEAT, VS, PS);
            foreach (FileVcsTokens vcsFile in fileArchive.GetFileVcsTokens())
            {
                ShaderFile shaderFile = vcsFile.GetShaderFile();
                foreach (SfBlock sfBlock in shaderFile.SfBlocks)
                {
                    // string reportLine = $"{sfBlock.name0,-40} {sfBlock.name1,-40} {sfBlock.arg2}";
                    string reportLine = $"{sfBlock.Name,-40} {sfBlock.FeatureIndex}";
                    CollectStringValue(reportLine);
                }
            }
            PrintReport();
        }

        public static void MipMapCountFileSets()
        {
            // FileArchive fileArchive = new(ARCHIVE.dota_game_vulkan_v65, FEAT, VS, PS);
            FileArchive fileArchive = new(ARCHIVE.dota_game_pcgl_v64, FEAT, VS, PS);
            string lastFile = "";
            foreach (FileVcsTokens vcsFile in fileArchive.GetFileVcsTokens())
            {
                ShaderFile shaderFile = vcsFile.GetShaderFile();
                if (!vcsFile.foldername.Equals(lastFile))
                {
                    Debug.WriteLine($"");
                    lastFile = vcsFile.foldername;
                }
                Debug.WriteLine($"{vcsFile,-80} {vcsFile.GetShaderFile().ChannelBlocks.Count}");
            }
        }

        /*
         * D-blocks are always 0 for features files
         */
        public static void CheckDBlockCountForFeaturesFiles()
        {
            // FileArchive fileArchive = new(ARCHIVE.dota_game_vulkan_v65, FEAT);
            FileArchive fileArchive = new(ARCHIVE.dota_game_pcgl_v64, FEAT, VS);
            foreach (FileVcsTokens vcsFile in fileArchive.GetFileVcsTokens())
            {
                ShaderFile shaderFile = vcsFile.GetShaderFile();
                Debug.WriteLine($"{vcsFile,-80} {shaderFile.DBlocks.Count}");
                if (shaderFile.DBlocks.Count != 0)
                {
                    throw new Exception("unecpexted value");
                }

            }
        }


        public static string ParseDynamicExpression(byte[] dynExpDatabytes)
        {
            try
            {
                return new VfxEval(dynExpDatabytes, omitReturnStatement: true).DynamicExpressionResult.Replace("UNKNOWN", "VAR");
            }
            catch (Exception)
            {
                return "[error in dyn-exp]";
            }
        }

    }
}
