using System.Diagnostics;

namespace RunVrf
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            // ApplicationConfiguration.Initialize();
            // Application.Run(new Form1());
            Runtests();
        }


        static void Runtests()
        {
            ParseFiles.RunTrials();
            // ReadShaderFilesFromVpk.RunTrials();
        }
    }
}
