using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

/*
 *
 * Regular expressions reference
 * https://docs.microsoft.com/en-us/dotnet/standard/base-types/regular-expression-language-quick-reference
 *
 *
 *
 */
namespace MyShaderAnalysis.codestash.snippetcode
{
    class RegexExamples
    {
        public static void RunTrials()
        {
            // new RegexExamples().RegexExample2();
            new RegexExamples().RegexExample1();
        }


        /*
         * performs replacements on the strings of the type
         *
         *      \\token0\\token1
         *
         */
        private void RegexExample2()
        {
            string sInput = "VertexShader (spritecard_pcgl_30_vs.vcs) (view byte detail \\\\spritecard_pcgl_30_vs.vcs\\bytes)\nnext line";
            Regex rx = new Regex(@"\\\\([a-z0-9_\.]*)\\([a-z0-9_\.]*)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            // -- Assign the replace method to the MatchEvaluator delegate.
            MatchEvaluator myEvaluator = new MatchEvaluator(ReplaceDoubleToken);
            // -- Replace matched characters using the delegate method.
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


        private void RegexExample1()
        {
            string sInput = "VertexShader (spritecard_pcgl_30_vs.vcs) (view byte detail \\\\spritecard_pcgl_30_vs.vcs\\bytes)\nhello";
            Regex rx = new Regex(@"\\\\(spritecard)", RegexOptions.Compiled | RegexOptions.IgnoreCase);

            MatchCollection matches = rx.Matches(sInput);
            foreach (Match match in matches)
            {
                GroupCollection groups = match.Groups;
                Console.WriteLine("'match found: {0}", groups[1]);
            }
        }



    }
}
