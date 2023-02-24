using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ValveResourceFormat.CompiledShader;

namespace ShaderAnalysis.utilhelpers
{
    class DataReaderVcsTesting : ShaderDataReader
    {

        public DataReaderVcsTesting(string filenamepath) : base(new MemoryStream(File.ReadAllBytes(filenamepath)))
        {

            ShowBytes(4);
            ShowBytes(4);
            ShowBytes(4);
            ShowBytes(4);
            ShowBytes(4, "len of name");
            BreakLine();
            string name = ReadNullTermStringAtPosition();
            ShowBytes(name.Length + 1, name);
            BreakLine();
            ShowBytes(32, 4);
            BreakLine();
            ShowBytes(4, "nr of arguments");
            ShowBytes(128);
            ShowBytes(4);
            BreakLine();
            ShowBytes(16);
            ShowBytes(16);
            ShowBytes(16);
            ShowBytes(16);
            ShowBytes(16);
            ShowBytes(16);
            ShowBytes(16);
            ShowBytes(16);
            BreakLine();
            ShowBytes(4);
            BreakLine();

            ShowBytes(400);


        }



    }
}





