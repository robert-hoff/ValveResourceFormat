using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using MyShaderAnalysis.utilhelpers;
using ValveResourceFormat.CompiledShader;
using static MyShaderAnalysis.utilhelpers.FileArchives;
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
 * Possibly, define ShaderCollection, for example
 *
 *      3dskyboxstencil_pcgl_30 (dota-game, pcgl)
 *
 * defined by the shader collection name (3dskyboxstencil_pcgl_30) archive name (dota-game) and platform (pcgl)
 *
 * The colletion should have knowledge of all the related shaders (features.vcs, vs.vcs, ps.vcs)
 *
 *      3dskyboxstencil_pcgl_30_features.vcs
 *      3dskyboxstencil_pcgl_30_vs.vcs
 *      3dskyboxstencil_pcgl_30_ps.vcs
 *
 * and related zframes, and gpu sources
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








