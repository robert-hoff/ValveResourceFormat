using System.Text.RegularExpressions;
using ValveResourceFormat.CompiledShader;
using static ValveResourceFormat.CompiledShader.ShaderUtilHelpers;

namespace MyShaderFileKristiker.MyHelperClasses
{
    /*
     * Replace RichTextBox links. They are in the form
     *
     *  -- links from vcs files
     *: \\debug_wireframe_2d_vulkan_50_features.vcs
     *  \\debug_wireframe_2d_vulkan_50_features.vcs\bytes
     *  \\00000000
     *
     *  -- links from zframe files (using PostProcessZframeFile.cs for these)
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
        private FileVcsTokens fileVcsTokens;

        public PostProcessVcsFile(FileVcsTokens fileTokens)
        {
            this.fileVcsTokens = fileTokens;
        }

        public string PostProcessVcsData(string data)
        {
            Regex rx = new Regex(@"\\\\([a-z0-9_\.]*)\\([a-z0-9_\.]*)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            MatchEvaluator myEvaluator = new MatchEvaluator(ReplaceVcsDoubleToken);
            string newData = rx.Replace(data, myEvaluator);

            rx = new Regex(@"\\\\([a-z0-9_\.]*)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            myEvaluator = new MatchEvaluator(ReplaceVcsSingleToken);
            newData = rx.Replace(newData, myEvaluator);

            return newData;
        }

        private string ReplaceVcsDoubleToken(Match m)
        {
            GroupCollection groups = m.Groups;
            switch (groups[2].ToString())
            {
                case "bytes":
                    return $"<a href='{fileVcsTokens.GetServerFileUrl("bytes")}'>{fileVcsTokens.filename}/bytes</a>";

                default:
                    throw new ShaderParserException($"Unrecognised link token: {groups[2].ToString()}"); ;
            }
        }

        // either a file name or a zframeId
        private string ReplaceVcsSingleToken(Match m)
        {
            GroupCollection groups = m.Groups;
            string[] fileStrTokens = Path.GetFileNameWithoutExtension(groups[1].ToString()).Split("_");
            if (fileStrTokens.Length >= 4)
            {
                VcsProgramType programType = ComputeVcsProgramType(fileStrTokens[^1]);
                return $"<a href='{fileVcsTokens.GetServerFilePath()}/{groups[1].ToString()[..^4]}.html'>{groups[1]}</a>";
            }
            else
            {
                long zframeId = Convert.ToInt64(groups[1].ToString(), 16);
                return fileVcsTokens.GetZFrameLinkIfOneExists(zframeId, noBrackets: true);
            }
        }
    }
}
