using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using MyShaderAnalysis.utilhelpers;
using ValveResourceFormat.CompiledShader;
using static MyShaderAnalysis.utilhelpers.FileSystem;
using static MyShaderAnalysis.utilhelpers.MyShaderUtilHelpers;
using static MyShaderAnalysis.utilhelpers.ReadShaderFile;
using static ValveResourceFormat.CompiledShader.ShaderUtilHelpers;
using MyShaderAnalysis.vcsanalysis;
using static ValveResourceFormat.CompiledShader.ShaderDataReader;
using System.Diagnostics;
using System.Globalization;

/*
 *
 * CreateHtmlServerFiles
 *
 * provide a single entry point for managing the printout of vcs files and their dependencies
 *
 *
 *
 *
 *
 */
namespace MyShaderAnalysis
{

    class CreateHtmlServerFiles
    {

        public static void RunTrials()
        {
            new CreateHtmlServerFiles();
        }


        public CreateHtmlServerFiles()
        {

            Console.WriteLine($"CreateHtmlServerFiles");
        }




    }


}








