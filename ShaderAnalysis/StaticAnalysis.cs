using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System;
using ValveResourceFormat.ShaderParser;
using ShaderAnalysis.utilhelpers;
using static ShaderAnalysis.utilhelpers.FileHelpers;
// using static ValveResourceFormat.ShaderParser.ShaderUtilHelpers;

namespace ShaderAnalysis
{
    public class StaticAnalysis
    {


        public static void RunTrials()
        {
            Trial2();
        }


        static void Trial2()
        {
            // string filenamepath = $"{FileSystem.DOTA_CORE_PCGL_SOURCE}/";



            Debug.WriteLine($"hello?");

        }



        static void Trial1()
        {
            List<string> vcsFiles = GetVcsFiles(FileSystem.DOTA_CORE_PCGL_SOURCE, FileSystem.DOTA_GAME_PCGL_SOURCE, VcsFiletype.Features, -1);

            foreach (var item in vcsFiles)
            {
                // ShaderFile shaderFile = new(item);
            }


        }




    }




}

