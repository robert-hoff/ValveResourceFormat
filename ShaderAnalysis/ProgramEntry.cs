using System;
using System.IO;
using System.Diagnostics;
using ShaderAnalysis.utilhelpers;

namespace ShaderAnalysis
{
    class ProgramEntry
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


        static void Main()
        {
            Console.SetOut(new ToDebugWriter());

            TestVfxEvalShaderExpressions.RunTrials();
            // TestCompiledShader.RunTrials();


        }




    }

}
