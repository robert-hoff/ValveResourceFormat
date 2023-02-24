using System.Reflection;

namespace MyShaderAnalysis.serverhtml
{
    public class ServerNames
    {
        // private const string PROJECT_TESTDIR = "Z:/active/projects/dota2-sourcesdk-modding/shader-analysis-vcs-format/OUTPUT_DUMP";
        private const string SERVER_BASEDIR = "Z:/dev/www/vcs.codecreation.dev/";
        private const string SERVER_TESTPATH = SERVER_BASEDIR + "GEN-output/";

        public static string GetServerBaseDir()
        {
            return SERVER_BASEDIR;
        }

        public static string GetServerTestDir()
        {
            return SERVER_TESTPATH;
        }

        public static string GetServerTestFile(int num = 0)
        {
            return SERVER_TESTPATH + $"testfile{(num > 0 ? num : "")}.html";
        }
    }
}


