using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ValveResourceFormat.CompiledShader;

namespace RunVrf
{
    class ParseFiles
    {

        public static void RunTrials()
        {
            Go1();
        }



        public static void Go1()
        {
            ZFrameFile zFrameFile = GetZFrameFile();
            //Debug.WriteLine($"{zFrameFile.ZframeParams[0].HasStaticVal}");
            //Debug.WriteLine($"{zFrameFile.ZframeParams[0].StaticValBool}");
            //Debug.WriteLine($"{zFrameFile.ZframeParams[0].StaticValFloat}");
            //Debug.WriteLine($"{zFrameFile.ZframeParams[0].StaticValInt}");
            //Debug.WriteLine($"{zFrameFile.ZframeParams[0].HeaderOperator}");


            foreach (var zFrameParams in zFrameFile.ZframeParams)
            {
                Debug.WriteLine($"{zFrameParams}");
                Debug.WriteLine($"{zFrameParams.LinkedParameterIndex}");
            }

        }



        static string DOTA_PCGL_V64_GAME = @"X:\dota-2-VRF-exports\dota2-export-shaders-pcgl\shaders\vfx\";
        // static string inputFile = $"{DOTA_PCGL_V64_GAME}cables_pcgl_40_vs.vcs"; static int zFrameIndex = 0;
        // static string inputFile = $"{DOTA_PCGL_V64_GAME}hero_pcgl_40_vs.vcs"; static int zFrameIndex = 0x17fa2;
        static string inputFile = $"{DOTA_PCGL_V64_GAME}hero_pcgl_40_ps.vcs"; static int zFrameIndex = 0x18cbfb1;

        public static ZFrameFile GetZFrameFile()
        {
            ShaderFile shaderFile = new ShaderFile();
            shaderFile.Read(inputFile);
            return shaderFile.GetZFrameFile(zFrameIndex);
        }








    }
}


