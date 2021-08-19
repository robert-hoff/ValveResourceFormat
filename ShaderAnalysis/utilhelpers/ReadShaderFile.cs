using System;
using System.IO;
using ValveResourceFormat.ShaderParser;

namespace ShaderAnalysis.utilhelpers
{
    public class ReadShaderFile : IDisposable
    {
        private string filenamepath;
        private ShaderDataReader datareader;

        public ReadShaderFile(string filenamepath)
        {
            this.filenamepath = filenamepath;
            datareader = new ShaderDataReader(File.OpenRead(filenamepath), writeToConsole: false, writeToDebug: true);
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
            if (disposing && datareader != null)
            {
                datareader.Dispose();
                datareader = null;
            }
        }




    }
}



