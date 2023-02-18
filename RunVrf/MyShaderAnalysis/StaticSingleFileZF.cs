using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ValveResourceFormat.CompiledShader;

namespace RunVrf.MyShaderAnalysis
{
    class StaticSingleFileZF
    {
        static string DOTA_PCGL_V64_GAME = @"X:\dota-2-VRF-exports\dota2-export-shaders-pcgl\shaders\vfx\";

        // static string inputFile = $"{DOTA_PCGL_V64_GAME}cables_pcgl_40_vs.vcs"; static int zFrameIndex = 0;
        // static string inputFile = $"{DOTA_PCGL_V64_GAME}hero_pcgl_40_vs.vcs"; static int zFrameIndex = 0x17fa2;
        // static string inputFile = $"{DOTA_PCGL_V64_GAME}hero_pcgl_40_ps.vcs"; static int zFrameIndex = 0x18cbfb1;
        // static string inputFile = $"{DOTA_PCGL_V64_GAME}spring_meteor_pcgl_30_vs.vcs"; static int zFrameIndex = 0;
        static string inputFile = $"{DOTA_PCGL_V64_GAME}crystal_pcgl_40_vs.vcs"; static int zFrameIndex = 0;
        // static string inputFile = $"{DOTA_PCGL_V64_GAME}crystal_pcgl_40_ps.vcs"; static int zFrameIndex = 0;
        // static string inputFile = $"{DOTA_PCGL_V64_GAME}crystal_pcgl_40_features.vcs"; static int zFrameIndex = 0;


        public static void RunTrials()
        {
            ShowZFrameParameters2();
            // ShowZFrameParameters1();
        }



        public static void ShowZFrameParameters2()
        {
            ZFrameFile zFrame = GetZFrameFile();
            Debug.WriteLine($"{zFrame}");
        }


        public static void ShowZFrameParameters1()
        {
            ShaderFile shaderFile = GetShaderFile();
            ZFrameFile zFrame = GetZFrameFile(shaderFile);
            Debug.WriteLine($"{zFrame.DataBlocks.Count}");

        }

        public static ZFrameFile GetZFrameFile()
        {
            ShaderFile shaderFile = GetShaderFile();
            return GetZFrameFile(shaderFile);
        }
        public static ZFrameFile GetZFrameFile(ShaderFile shaderFile)
        {
            return shaderFile.GetZFrameFile(zFrameIndex);
        }
        public static ShaderFile GetShaderFile()
        {
            ShaderFile shaderFile = new ShaderFile();
            shaderFile.Read(inputFile);
            return shaderFile;
        }

    }
}
