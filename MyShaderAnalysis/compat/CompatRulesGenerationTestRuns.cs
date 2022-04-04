using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyShaderAnalysis.codestash;
using ValveResourceFormat.CompiledShader;
using static ValveResourceFormat.CompiledShader.ShaderUtilHelpers;

namespace MyShaderAnalysis.compat
{

    public class CompatRulesGenerationTestRuns
    {

        const string PCGL_DIR_CORE = @"X:/dota-2-VRF-exports/dota2-export-shaders-pcgl/shaders-core/vfx";
        const string PCGL_DIR_NOT_CORE = @"X:/dota-2-VRF-exports/dota2-export-shaders-pcgl/shaders/vfx";

        public static void RunTrials()
        {
            Trial1();

        }

        static void Trial1()
        {
            // string vcsFilenamepath = @$"{PCGL_DIR_NOT_CORE}/hero_pcgl_30_ps.vcs";
            // string vcsFilenamepath = @$"{PCGL_DIR_NOT_CORE}/multiblend_pcgl_30_ps.vcs";
            // string vcsFilenamepath = @$"{PCGL_DIR_CORE}/blur_pcgl_30_ps.vcs";
            string vcsFilenamepath = @$"{PCGL_DIR_NOT_CORE}/refract_pcgl_30_ps.vcs";
            // string vcsFilenamepath = @$"{PCGL_DIR_NOT_CORE}/water_dota_pcgl_30_ps.vcs";

            ShaderFile shaderFile =ReadShaderFile.InstantiateShaderFile(vcsFilenamepath);
            ConfigMappingSParams configGen = new(shaderFile);
            // zframeGen.ShowOffsetAndLayersArrays();


            foreach (var item in shaderFile.zframesLookup)
            {
                int[] state = configGen.GetConfigState(item.Key);
                ShowIntArray(state);

            }




        }





    }



}




