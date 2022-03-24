using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MyShaderAnalysis.utilhelpers;


namespace MyShaderAnalysis.vcsanalysis
{
    /*
     * Replace RichTextBox links. They are in the form
     *
     *  -- links from vcs files
     *: \\debug_wireframe_2d_vulkan_50_features.vcs
     *  \\debug_wireframe_2d_vulkan_50_features.vcs\bytes
     *  \\00000000
     *
     *  -- links from zframe files
     *  \\debug_wireframe_2d_vulkan_50_vs.vcs-ZFRAME00000000-databytes
     *  \\source\0
     *
     * In GUI.Types.Viewers.CompiledShader whether it is a vcs file or zframe is known,
     * in this context we should also know this distinction in advance.
     * We can also check when URL's are processed whether or not the file is present on disk.
     * In the case of source links also need to know what type of source it is, the source
     * printouts are organised using their type and ref-id (a 32 length hex string)
     *
     *
     */
    class PostProcessVcsFile
    {

        public PostProcessVcsFile(FileTokens fileTokens, string formattedData)
        {
            Console.WriteLine($"{formattedData}");
            // RegexExample3();
            // RegexExample2();
            // RegexExample1();

            // Console.WriteLine(fileTokens.GetServerFileUrl("sdf"));


        }


        private void RegexExample3()
        {
            string sInput = "VertexShader (spritecard_pcgl_30_vs.vcs) (view byte detail \\\\spritecard_pcgl_30_vs.vcs\\bytes)\nhello";
            Regex rx = new Regex(@"\\\\([a-z0-9_\.]*)\\([a-z0-9_\.]*)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            MatchEvaluator myEvaluator = new MatchEvaluator(ReplaceDoubleToken);
            sInput = rx.Replace(sInput, myEvaluator);



            Console.WriteLine(sInput);
        }


        public string ReplaceDoubleToken(Match m)
        {
            // i++;
            // Console.WriteLine($"--");
            // Console.WriteLine($"{m.Value}");

            GroupCollection groups = m.Groups;
            return $"[replacing {groups[1]} and {groups[2]}]";
        }




    }
}
