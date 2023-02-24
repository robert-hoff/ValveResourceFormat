using System;
using MyShaderAnalysis.filearchive;
using MyShaderFile.CompiledShader;

#pragma warning disable IDE0051 // Remove unused private members
#pragma warning disable IDE0060 // Remove unused parameter
#pragma warning disable CA1823 // Avoid unused private fields
namespace MyShaderAnalysis.compat.background
{
    public class CompatRulesNewApproach
    {
        const string PCGL_DIR_CORE = @"X:/dota-2-VRF-exports/dota2-export-shaders-pcgl/shaders-core/vfx";
        const string PCGL_DIR_NOT_CORE = @"X:/dota-2-VRF-exports/dota2-export-shaders-pcgl/shaders/vfx";

        public static void RunTrials()
        {
            Trials1();
        }
        static void Trials1()
        {
            string filenamepath = @$"{PCGL_DIR_NOT_CORE}/water_dota_pcgl_30_ps.vcs";
            ShaderFile shaderFile = ReadShaderFile.InstantiateShaderFile(filenamepath);
            foreach (SfBlock sfBlock in shaderFile.SfBlocks)
            {
                Console.WriteLine($"{sfBlock.Name}");
            }
        }

        static bool CheckZFrame(int zframeId)
        {
            return false;
        }
    }
}

