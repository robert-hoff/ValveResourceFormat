using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using MyShaderAnalysis.compat;
using ValveResourceFormat.CompiledShader;
using static MyShaderAnalysis.utilhelpers.FileSystem;
using static MyShaderAnalysis.utilhelpers.ReadShaderFile;
using static ValveResourceFormat.CompiledShader.ShaderUtilHelpers;

namespace MyShaderAnalysis.utilhelpers
{
    public class TestBasics
    {

        public static void RunTrials()
        {
            Trial1();
        }


        static void Trial1()
        {
            string filenamepath = $"{DOTA_GAME_PCGL_SOURCE}/multiblend_pcgl_30_vs.vcs";
            ShaderFile shaderFile = InstantiateShaderFile(filenamepath);
            Console.WriteLine($"{shaderFile.GetZFrameCount()}");
        }


    }
}
