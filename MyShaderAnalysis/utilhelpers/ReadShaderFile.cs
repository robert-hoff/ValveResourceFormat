using System;
using System.IO;
using ValveResourceFormat.CompiledShader;

namespace MyShaderAnalysis.utilhelpers
{
    // public class ReadShaderFile : IDisposable
    public class ReadShaderFile
    {
        //private string filenamepath;
        //private ShaderDataReader datareader;
        //public ReadShaderFile(string filenamepath)
        //{
        //    this.filenamepath = filenamepath;
        //    datareader = new ShaderDataReader(File.OpenRead(filenamepath));
        //}
        //public ShaderFile GetShaderFile()
        //{
        //    ShaderFile shaderFile = new ShaderFile(filenamepath);
        //    return shaderFile;
        //}
        //public void Dispose()
        //{
        //    Dispose(true);
        //    GC.SuppressFinalize(this);
        //}
        //protected virtual void Dispose(bool disposing)
        //{
        //    if (disposing && datareader != null)
        //    {
        //        datareader.Dispose();
        //        datareader = null;
        //    }
        //}

        public static ShaderFile InstantiateShaderFile(string filenamepath)
        {
            ShaderFile shaderFile = new ShaderFile();
            shaderFile.Read(filenamepath);
            return shaderFile;
        }
    }
}
