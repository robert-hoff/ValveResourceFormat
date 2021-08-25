using System;
using System.IO;
using MyShaderAnalysis.vcsparsing;

namespace MyShaderAnalysis.utilhelpers
{
    public class ReadShaderFile : IDisposable
    {
        private string filenamepath;
        private ShaderDataReader datareader;

        public ReadShaderFile(string filenamepath)
        {
            this.filenamepath = filenamepath;
            datareader = new ShaderDataReader(File.OpenRead(filenamepath);
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


        public static ShaderFile InstantiateShaderFile(string filenamepath)
        {
            ShaderFile shaderFile = new ReadShaderFile(filenamepath).GetShaderFile();
            return shaderFile;
        }


    }
}



