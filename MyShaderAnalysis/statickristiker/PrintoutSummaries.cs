using System.Diagnostics;
using System.IO;
using ValveResourceFormat.CompiledShader;
using MyShaderFileKristiker.MyHelperClasses;

namespace MyShaderAnalysis.statickristiker
{
    class PrintoutSummaries
    {
        static string DOTA_PCGL_V64_GAME = @"X:\dota-2-VRF-exports\dota2-export-shaders-pcgl\shaders\vfx\";

        // static string inputFile = $"{DOTA_PCGL_V64_GAME}cables_pcgl_40_vs.vcs"; static int zFrameIndex = 0;
        // static string inputFile = $"{DOTA_PCGL_V64_GAME}hero_pcgl_40_vs.vcs"; static int zFrameIndex = 0x17fa2;
        // static string inputFile = $"{DOTA_PCGL_V64_GAME}hero_pcgl_40_ps.vcs"; static int zFrameIndex = 0x18cbfb1;
        // static string inputFile = $"{DOTA_PCGL_V64_GAME}spring_meteor_pcgl_30_vs.vcs"; static int zFrameIndex = 0;
        static string inputFile = $"{DOTA_PCGL_V64_GAME}crystal_pcgl_40_vs.vcs"; static int zFrameIndex = 2;
        // static string inputFile = $"{DOTA_PCGL_V64_GAME}crystal_pcgl_40_ps.vcs"; static int zFrameIndex = 0;
        // static string inputFile = $"{DOTA_PCGL_V64_GAME}crystal_pcgl_40_features.vcs"; static int zFrameIndex = 0;
        // static string inputFile = $"{DOTA_PCGL_V64_GAME}3dskyboxstencil_pcgl_40_ps.vcs"; static int zFrameIndex = 0;
        // static string inputFile = $"{DOTA_PCGL_V64_GAME}multiblend_pcgl_40_ps.vcs"; static int zFrameIndex = 0;

        // static bool writeToDefaultFile = true;
        static bool writeToDefaultFile = false;
        static string defaultFile = "output2.html";


        public static void RunTrials()
        {
            // PrintShaderFileToHtml();
            PrintZframeToHtml();
            PrintZframeToHtmlByteVersion();
            // ShowZFrameParameters();
        }

        public static void PrintShaderFileToHtml()
        {
            string htmlTitle = $"{Path.GetFileName(inputFile)}";
            string htmlHeader = htmlTitle;
            ShaderFile shaderFile = GetShaderFile();
            string outputFilename = writeToDefaultFile ? defaultFile : OutputFilenameShaderFile();
            FileWriter fw = new FileWriter(outputFilename, true, true, htmlTitle, htmlHeader);
            _ = new PrintVcsFileSummary(shaderFile, (x) => { fw.Write(x); }, showRichTextBoxLinks: true);
            fw.CloseStreamWriter();
        }

        public static void PrintZframeToHtml()
        {
            string htmlTitle = $"Z[0x{zFrameIndex:x}]";
            string htmlHeader = $"{Path.GetFileName(inputFile)}  Z[0x{zFrameIndex:x}]";
            string outputFilename = writeToDefaultFile ? defaultFile : OutputFilenameZframe();
            FileWriter fw = new FileWriter(outputFilename, true, true, htmlTitle, htmlHeader);
            ShaderFile shaderFile = GetShaderFile();
            ZFrameFile zFrameFile = GetZFrameFile(shaderFile);
            _ = new PrintZFrameSummary(shaderFile, zFrameFile, (x) => { fw.Write(x); }, showRichTextBoxLinks: true);
            fw.CloseStreamWriter();
        }

        public static void PrintZframeToHtmlByteVersion()
        {
            string htmlTitle = $"Z[0x{zFrameIndex:x}]";
            string htmlHeader = $"{Path.GetFileName(inputFile)}  Z[0x{zFrameIndex:x}]";
            string outputFilename = writeToDefaultFile ? defaultFile : OutputFilenameZframeBytes();
            FileWriter fw = new FileWriter(outputFilename, true, true, htmlTitle, htmlHeader);
            ShaderFile shaderFile = GetShaderFile();
            ZFrameFile zFrameFile = GetZFrameFile(shaderFile);
            zFrameFile.PrintByteDetail((x) => { fw.Write(x); });
            fw.CloseStreamWriter();
        }

        public static ShaderFile GetShaderFile()
        {
            ShaderFile shaderFile = new ShaderFile();
            shaderFile.Read(inputFile);
            return shaderFile;
        }

        public static ZFrameFile GetZFrameFile(ShaderFile shaderFile)
        {
            return shaderFile.GetZFrameFile(zFrameIndex);
        }


        private static string OutputFilenameShaderFile()
        {
            return $"{Path.GetFileNameWithoutExtension(inputFile)}.html";
        }

        private static string OutputFilenameZframe()
        {
            string shaderfilename = Path.GetFileNameWithoutExtension(inputFile);
            return $"{shaderfilename}-Z[0x{zFrameIndex:x}].html";
        }

        private static string OutputFilenameZframeBytes()
        {
            string shaderfilename = Path.GetFileNameWithoutExtension(inputFile);
            return $"{shaderfilename}-Z[0x{zFrameIndex:x}]-byte.html";
        }

        public static void ShowZFrameParameters()
        {
            ShaderFile shaderFile = GetShaderFile();
            ZFrameFile zFrameFile = GetZFrameFile(shaderFile);
            Debug.WriteLine($"{zFrameFile.Attributes[0].HasStaticVal}");
            Debug.WriteLine($"{zFrameFile.Attributes[0].StaticValBool}");
            Debug.WriteLine($"{zFrameFile.Attributes[0].StaticValFloat}");
            Debug.WriteLine($"{zFrameFile.Attributes[0].StaticValInt}");
            Debug.WriteLine($"{zFrameFile.Attributes[0].HeaderOperator}");
        }
    }
}
