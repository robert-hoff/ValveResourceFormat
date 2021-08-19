using System;
using System.IO;
using MyShaderAnalysis.vcsparsing;

namespace MyShaderAnalysis.utilhelpers
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


        public static ShaderFile InstantiateShaderFile(string filenamepath)
        {
            ShaderFile shaderFile = new ReadShaderFile(filenamepath).GetShaderFile();
            return shaderFile;
        }


    }
}



