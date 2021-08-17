using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ValveResourceFormat.ShaderParser;

namespace ShaderAnalysis.utilhelpers
{
    public class ReadShaderFile : IDisposable
    {

        private string filenamepath;
        private BinaryReader Reader;
        private ShaderDataReader datareader;

        public ReadShaderFile(string filenamepath)
        {
            this.filenamepath = filenamepath;
            BinaryReader binReader = new BinaryReader(File.OpenRead(filenamepath));
            datareader = new ShaderDataReader(binReader, writeToConsole: false, writeToDebug: true);
        }


        public ShaderFile GetShaderFile()
        {
            return new ShaderFile(filenamepath, datareader);
        }


        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing && Reader != null)
            {
                Reader.Dispose();
                Reader = null;
            }
        }




    }
}



