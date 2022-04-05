using System;
using MyShaderAnalysis.codestash;
using MyShaderAnalysis.utilhelpers;
using ValveResourceFormat.CompiledShader;

namespace MyShaderAnalysis.compat
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
            foreach (var sfBlock in shaderFile.sfBlocks)
            {

                Console.WriteLine($"{sfBlock.name0}");
            }


        }






        static bool CheckZFrame(int zframeId)
        {



            return false;
        }








    }



}






