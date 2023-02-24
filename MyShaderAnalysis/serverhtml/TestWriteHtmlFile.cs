using MyShaderAnalysis.util;

namespace MyShaderAnalysis.serverhtml
{
    public class TestWriteHtmlFile
    {
        public static void Run()
        {
            WriteHtmlTestfile();
        }

        /*
         * NOTE - the html writing works but is not used in general
         *
         * writes a file to
         * Z:/dev/www/vcs.codecreation.dev/hello.html
         *
         * served as
         * vcs/hello.html
         *
         */
        public static void WriteHtmlTestfile()
        {
            string filenamepath = $"{ServerNames.GetServerBaseDir()}hello.html";
            FileWriter fw = new FileWriter(filenamepath);
            fw.WriteHtmlHeader("title", "header");
            fw.WriteLine("hello");
            fw.CloseStreamWriter();
        }
    }
}

