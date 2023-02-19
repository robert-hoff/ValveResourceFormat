using System;
using ValveResourceFormat.CompiledShader;
using MyShaderFileKristiker.MyHelperClasses;
using static MyShaderFileKristiker.MyHelperClasses.FileArchive;

namespace MyShaderAnalysis.statickristiker
{
    public class TestStuff
    {
        public static void RunTrials()
        {
            ShowZFrameCount();
        }

        static void ShowZFrameCount()
        {
            string filenamepath = GetFilenamepath(ARCHIVE.dota_game_pcgl_v64, "multiblend_pcgl_30_vs.vcs");
            Console.WriteLine($"{filenamepath}");
            ShaderFile shaderFile = ReadShaderFile.InstantiateShaderFile(filenamepath);
            Console.WriteLine($"{shaderFile.GetZFrameCount()} zframes");
        }
    }
}
