using System;
using System.IO;
using ValveResourceFormat.CompiledShader;

namespace MyShaderAnalysis.filearchive
{
    public class ReadShaderFile
    {
        /*
         * This method is provided so that the interface shown by ShaderFile is the same
         * as the one used in the VRF library
         *
         */
        public static ShaderFile InstantiateShaderFile(string filenamepath)
        {
            ShaderFile shaderFile = new ShaderFile();
            shaderFile.Read(filenamepath);
            return shaderFile;
        }
    }
}
