using System;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using MyShaderAnalysis.filearchive;
using MyShaderFile.CompiledShader;

namespace MyShaderAnalysis.postprocessing
{
    /*
     * Replace RichTextBox links. There are only two types of Zframe links
     *
     *  \\debug_wireframe_2d_vulkan_50_vs.vcs-ZFRAME00000000-databytes
     *  \\source\0
     *
     *
     *
     */
    class PostProcessZframeFile
    {
        private FileVcsTokens fileTokens;
        private ZFrameFile zframeFile;

        public PostProcessZframeFile(ZFrameFile zframeFile, FileVcsTokens fileTokens)
        {
            this.zframeFile = zframeFile;
            this.fileTokens = fileTokens;
        }

        public string PostProcessZframeData(string data)
        {
            string newData = getBackLink() + data;

            Regex rx = new Regex(@"\\\\([a-z0-9_\.]*)\\([a-z0-9_\.]*)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            MatchEvaluator myEvaluator = new MatchEvaluator(ReplaceZframeDoubleToken);
            newData = rx.Replace(newData, myEvaluator);

            rx = new Regex(@"\\\\([a-z0-9_\.\-]*)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            myEvaluator = new MatchEvaluator(ReplaceZframeSingleToken);
            newData = rx.Replace(newData, myEvaluator);

            return newData;
        }

        private string getBackLink()
        {
            string linkName = $"{fileTokens.GetServerFilePath()}_{fileTokens.vcstoken}.vcs";
            return $"Zframe belongs to <a href='{fileTokens.GetServerFileUrl("summary2")}'>{linkName}</a>\n";
        }

        /*
         * groups[1] equals "source"
         * groups[2] equals base 10 integer 0,1,2,3,...
         *
         * create links in the form
         *
         * ../glsl-fb3f572a73f44b3a25c8756e04c57bae.html
         *
         *
         */
        private string ReplaceZframeDoubleToken(Match m)
        {
            GroupCollection groups = m.Groups;
            if (!groups[1].ToString().Equals("source"))
            {
                throw new ShaderParserException($"Unrecognised link \\\\{groups[1]}\\{groups[2]}");
            }
            int gpuSourceId = Convert.ToInt32(groups[2].ToString(), CultureInfo.InvariantCulture);
            string gpuHtmlFilename = fileTokens.GetGpuHtmlFilename(zframeFile.GpuSources[gpuSourceId].GetEditorRefIdAsString());

            // omits printing the link if the source file is not found on the server
            // (but always prints the gpuSourceId == 0 link for reference)
            if (gpuSourceId == 0 || File.Exists($"{fileTokens.GetGpuServerDir()}/{gpuHtmlFilename}"))
            {
                return $"<a href='{fileTokens.GetGpuServerUrl()}/{gpuHtmlFilename}'>//{groups[1]}/{groups[2]}</a>";
            }
            else
            {
                return $"//{groups[1]}/{groups[2]}";
            }
        }

        // always in the form
        // \\3dskyboxstencil_pcgl_30_vs.vcs-ZFRAME00000000-databytes
        private string ReplaceZframeSingleToken(Match m)
        {
            GroupCollection groups = m.Groups;
            // parsing the id will throw if syntax isn't exactly as expected
            // (parsing isn't really necessary because the context is aware of the zframe already
            // but this is how it's done on the GUI)
            long zframeId = Convert.ToInt64(groups[1].ToString().Split('-')[^2][6..], 16);
            string htmlBytesName = fileTokens.GetZFrameUrl(zframeId, "bytes");
            return $"<a href='{htmlBytesName}'>//{groups[1]}</a>";
        }
    }
}


