using System.Diagnostics;
using ValveResourceFormat.CompiledShader;

namespace MyShaderFileKristiker.MyHelperClasses
{
    public class TestHelperClasses
    {
        public static void RunTrials()
        {
            Go1();
        }

        public static void Go1()
        {
            ARCHIVE archive = ARCHIVE.dota_game_pcgl_v64;
            FileArchive fileArchive = new(archive, VcsShaderModelType._40, useModularLookup: true);
            Debug.WriteLine($"{fileArchive.archive}");
        }

    }
}
