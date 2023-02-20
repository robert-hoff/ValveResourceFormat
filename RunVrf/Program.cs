using System.Diagnostics;
using RunVrf.MyShaderAnalysis;
using RunVrf.UtilHelpers;

namespace RunVrf
{
    internal static class Program
    {
        public class ToDebugWriter : StringWriter
        {
            public override void WriteLine(string str0)
            {
                Debug.WriteLine(str0);
            }
            public override void Write(string str0)
            {
                Debug.Write(str0);
            }
        }

        [STAThread]
        static void Main()
        {
            Console.SetOut(new ToDebugWriter());
            // ApplicationConfiguration.Initialize();
            // Application.Run(new Form1());

            // StaticAnalysis();
            // StaticSingleFileInspection();
            BatchTestFiles();
            // SingleFilePrintouts();
        }



        static void StaticAnalysis()
        {
            // StaticAnalysis3.Run();
        }

        static void StaticSingleFileInspection()
        {
            StaticSingleFileZF.RunTrials();
            // StaticSingleSF.RunTrials();
        }

        static void BatchTestFiles()
        {
            BatchTestShaderFiless.RunTrials();
        }

        static void SingleFilePrintouts()
        {
            PrintoutSummaries.RunTrials();
            // ReadShaderFilesFromVpk.RunTrials();
        }
    }
}

