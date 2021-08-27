using System;
using System.IO;
using System.Collections.Generic;
using ValveResourceFormat.ThirdParty;
using ValveResourceFormat.CompiledShader;
using ValveResourceFormat.Serialization.VfxEval;
using MyShaderAnalysis.utilhelpers;
using static MyShaderAnalysis.utilhelpers.FileSystem;
using static MyShaderAnalysis.utilhelpers.MyShaderUtilHelpers;
using static ValveResourceFormat.CompiledShader.ShaderUtilHelpers;

namespace MyShaderAnalysis.utilhelpers
{
    public class DataReaderV62 : ShaderDataReader
    {

        public static void RunTrials()
        {
            Trial1();
        }

        public const string V62_EXAMPLES_SOURCE = "X:/v62shaders-from-xpaw";


        static void Trial1()
        {
            // string filenamepath = $"{V62_EXAMPLES_SOURCE}/test_pc_30_ps.vcs";
            string filenamepath = $"{V62_EXAMPLES_SOURCE}/test_pcgl_30_ps.vcs";
            new DataReaderV62(filenamepath);
        }




        private VcsProgramType vcsProgramType;
        private VcsPlatformType vcsSourceType;
        private VcsShaderModelType vcsModelType;


        public DataReaderV62(string filenamepath) : base(new MemoryStream(File.ReadAllBytes(filenamepath)))
        {
            var VcsFileProperties = ComputeVCSFileName(filenamepath);
            this.vcsProgramType = VcsFileProperties.Item1;
            this.vcsSourceType = VcsFileProperties.Item2;
            this.vcsModelType = VcsFileProperties.Item3;

            ShowBytes(4, "\"vcs2\"");
            ShowBytesWithIntValue();
            BreakLine();

            ShowBytes(16, "Editor ref 1");
            ShowBytes(16, "Editor ref 2");
            BreakLine();
            int minorVersion = ReadInt32AtPosition();
            ShowBytes(4, $"{minorVersion}");
            BreakLine();

            ShowBytesWithIntValue();
            string name = ReadNullTermStringAtPosition();
            TabComment(name);
            ShowBytes(64);
            ShowBytes(64);
            ShowBytes(4);
            ShowBytes(4);
            ShowBytes(4);
            ShowBytes(4);
            ShowBytes(4);
            ShowBytes(4);
            BreakLine();

            int sRuleBlockCount = ReadInt32AtPosition();
            ShowBytes(4, $"sf-constraint blocks = {sRuleBlockCount}");
            int dlockCount = ReadInt32AtPosition();
            ShowBytes(4, $"d-blocks = {dlockCount}");
            int dRuleBlockCount = ReadInt32AtPosition();
            ShowBytes(4, $"d-constraint-blocks = {dRuleBlockCount}");
            int paramBlockCount = ReadInt32AtPosition();
            ShowBytes(4, $"param-blocks = {paramBlockCount}");
            BreakLine();

            string paramName0 = ReadNullTermStringAtPosition();
            OutputWriteLine(paramName0);
            ShowBytes(64);
            ShowBytes(64);
            ShowBytes(8);
            ShowBytes(64);
            int dynExpCheck = ReadInt32AtPosition();
            ShowBytes(4);
            if (dynExpCheck == 6 || dynExpCheck == 7)
            {
                int dynExpLen = ReadInt32AtPosition();
                ShowBytes(4, $"dynExp len = {dynExpLen}");
                ShowBytes(dynExpLen, "dynExp");
            }
            ShowBytes(4);
            ShowBytes(4);
            ShowBytes(4);
            ShowBytes(4);
            ShowBytes(4);
            ShowBytes(64);
            // string paramName1 = ReadNullTermStringAtPosition();
            // OutputWriteLine(paramName1);

            // ShowBytesWithIntValue();
            ShowBytes(16);
            ShowBytes(16);
            ShowBytes(16);
            ShowBytes(16);
            ShowBytes(16);
            ShowBytes(16);
            ShowBytes(16);
            ShowBytes(16);
            ShowBytes(64);
            BreakLine();



            string paramName1 = ReadNullTermStringAtPosition();
            OutputWriteLine(paramName1);
            ShowBytes(64);
            ShowBytes(64);
            ShowBytes(8);
            ShowBytes(64);
            ShowBytes(4);
            ShowBytes(4);
            ShowBytes(4);
            ShowBytes(4);
            ShowBytes(4);
            ShowBytes(4);
            ShowBytes(64);
            ShowBytes(16);
            ShowBytes(16);
            ShowBytes(16);
            ShowBytes(16);
            ShowBytes(16);
            ShowBytes(16);
            ShowBytes(16);
            ShowBytes(16);



            BreakLine();


            ShowBytes(500);



        }




    }
}
