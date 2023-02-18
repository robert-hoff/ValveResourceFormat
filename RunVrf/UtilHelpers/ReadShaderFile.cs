using ValveResourceFormat.CompiledShader;

namespace RunVrf.UtilHelpers
{
    class ReadShaderFile
    {
        public static ShaderFile InstantiateShaderFile(string filenamepath)
        {
            ShaderFile shaderFile = new ShaderFile();
            shaderFile.Read(filenamepath);
            return shaderFile;
        }
    }
}
