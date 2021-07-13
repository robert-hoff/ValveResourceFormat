using System;
using System.Text.RegularExpressions;

namespace MyGUI.Utils {
    public static class Utils {
        private static Regex NewLineRegex = new Regex(@"\r\n|\n\r|\n|\r", RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture);

        public static string NormalizeLineEndings(string input) {
            return NewLineRegex.Replace(input, Environment.NewLine);
        }
    }
}
