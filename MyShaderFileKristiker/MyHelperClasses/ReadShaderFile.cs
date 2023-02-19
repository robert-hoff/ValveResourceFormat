using ValveResourceFormat.CompiledShader;

namespace MyShaderFileKristiker.MyHelperClasses
{
    public class ReadShaderFile
    {
        public static ShaderFile InstantiateShaderFile(string filenamepath)
        {
            ShaderFile shaderFile = new ShaderFile();
            shaderFile.Read(filenamepath);
            return shaderFile;
        }
    }
}
