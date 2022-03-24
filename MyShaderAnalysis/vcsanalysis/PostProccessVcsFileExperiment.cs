using System;
using System.Collections.Generic;
using System.IO;
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
 *
 *
 *
 *
 */
namespace MyShaderAnalysis.vcsanalysis
{
    class PostProccessVcsFileExperiment
    {


        public PostProccessVcsFileExperiment(string formattedData)
        {


            // Console.WriteLine($"{formattedData}");

            RegexExample3();
            // RegexExample2();
            // RegexExample1();

        }

        public PostProccessVcsFileExperiment(StringWriter sw)
        {
            StringBuilder sb = sw.GetStringBuilder();


            foreach (ReadOnlyMemory<char> chunk in sb.GetChunks())
            {

                Console.WriteLine($"-----------------");
                Console.WriteLine($"{chunk}");
            }
        }



        private void RegexExample4()
        {
            string sInput, sRegex;

            // The string to search.
            sInput = "VertexShader (spritecard_pcgl_30_vs.vcs) (view byte detail \\\\spritecard_pcgl_30_vs.vcs\\bytes)\nhello";

            // A very simple regular expression.
            sRegex = "cc";

            // Regex rx = new Regex(sRegex);
            // Regex rx = new Regex(@"\b(?<word>\w+)\s+(\k<word>)\b", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            Regex rx = new Regex(@"\\\\(spritecard)", RegexOptions.Compiled | RegexOptions.IgnoreCase);


            MatchCollection matches = rx.Matches(sInput);
            foreach (Match match in matches)
            {
                // GroupCollection groups = match.Groups;
                // Console.WriteLine("'{0}' repeated at ", groups[1]);
                ReplaceCC(match);
            }



            // -- Assign the replace method to the MatchEvaluator delegate.
            // MatchEvaluator myEvaluator = new MatchEvaluator(ReplaceCC);
            // -- Write out the original string.
            // Console.WriteLine(sInput);
            // -- Replace matched characters using the delegate method.
            // sInput = rx.Replace(sInput, myEvaluator);



            Console.WriteLine(sInput);
        }


        private void RegexExample3()
        {
            string sInput = "VertexShader (spritecard_pcgl_30_vs.vcs) (view byte detail \\\\spritecard_pcgl_30_vs.vcs\\bytes)\nhello";

            // Regex rx = new Regex(sRegex);
            // Regex rx = new Regex(@"\b(?<word>\w+)\s+(\k<word>)\b", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            Regex rx = new Regex(@"\\\\(spritecard)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            MatchEvaluator myEvaluator = new MatchEvaluator(ReplaceCC);
            sInput = rx.Replace(sInput, myEvaluator);

            // -- Assign the replace method to the MatchEvaluator delegate.
            // MatchEvaluator myEvaluator = new MatchEvaluator(ReplaceCC);
            // -- Write out the original string.
            // Console.WriteLine(sInput);
            // -- Replace matched characters using the delegate method.
            // sInput = rx.Replace(sInput, myEvaluator);



            Console.WriteLine(sInput);
        }



        private void RegexExample2()
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


        public string ReplaceCC(Match m)
        {
            // i++;
            // Console.WriteLine($"--");
            // Console.WriteLine($"{m.Value}");

            GroupCollection groups = m.Groups;


            return $"link[{groups[1]}]";
        }


        // public static int i = 0;




        private void RegexExample1()
        {

            Regex rx = new Regex(@"\b(?<word>\w+)\s+(\k<word>)\b", RegexOptions.Compiled | RegexOptions.IgnoreCase);

            // Define a test string.
            string text = "The the quick brown fox fox jumps over the lazy dog dog.";

            // Find matches.
            MatchCollection matches = rx.Matches(text);

            // Report the number of matches found.
            Console.WriteLine($"{matches.Count} matches found in:\n   {text}");

            // Report on each match.
            foreach (Match match in matches)
            {
                GroupCollection groups = match.Groups;
                Console.WriteLine("'{0}' repeated at positions {1} and {2}",
                                  groups["word"].Value,
                                  groups[0].Index,
                                  groups[1].Index);
            }
        }



    }
}
