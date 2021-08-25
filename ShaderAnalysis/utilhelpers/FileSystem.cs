using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ValveResourceFormat.ShaderParser;


namespace ShaderAnalysis.utilhelpers
{

    public class FileSystem
    {

        private const string PROJECT_TESTDIR = "Z:/active/projects/dota2-sourcesdk-modding/shader-analysis-vcs-format/OUTPUT_DUMP";
        private const string SERVER_BASEDIR = "Z:/dev/www/vcs.codecreation.dev";
        private const string SERVER_TESTPATH = "/GEN-output";
        private const string SERVER_BYTEPATH = "/vcs-all";
        public const string DOTA_CORE_PCGL_SOURCE = "X:/dota-2-VRF-exports/dota2-export-shaders-pcgl/shaders-core/vfx";
        public const string DOTA_GAME_PCGL_SOURCE = "X:/dota-2-VRF-exports/dota2-export-shaders-pcgl/shaders/vfx";
        public const string DOTA_CORE_PC_SOURCE = "X:/dota-2-VRF-exports/dota2-export-shaders-pc/shaders-core/vfx";
        public const string DOTA_GAME_PC_SOURCE = "X:/dota-2-VRF-exports/dota2-export-shaders-pc/shaders/vfx";
        public const string ARTIFACT_CLASSIC_CORE_PC_SOURCE = "X:/artifact-classic-exports/artifact-shaders-pc-core";
        public const string ARTIFACT_CLASSIC_DCG_PC_SOURCE = "X:/artifact-classic-exports/artifact-shaders-pc-dcg";



        public enum ARCHIVE { dotacore_pcgl, dotagame_pcgl };



        public static string GetFilenamepath(ARCHIVE archive, string filename)
        {
            return $"{GetSourceDir(archive)}/{filename}";
        }

        public static string GetServerBaseDir()
        {
            return SERVER_BASEDIR;
        }


        public static string GetSourceDir(ARCHIVE archive)
        {
            if (archive == ARCHIVE.dotacore_pcgl) return DOTA_CORE_PCGL_SOURCE;
            if (archive == ARCHIVE.dotagame_pcgl) return DOTA_GAME_PCGL_SOURCE;
            throw new ShaderParserException("unknown archive");
        }

        public static string GetArchiveName(ARCHIVE archive)
        {
            if (archive == ARCHIVE.dotacore_pcgl) return "dota-core";
            if (archive == ARCHIVE.dotagame_pcgl) return "dota-game";
            throw new ShaderParserException("unknown archive");
        }

        public static string GetArchiveLabel(ARCHIVE archive)
        {
            if (archive == ARCHIVE.dotacore_pcgl) return "core";
            if (archive == ARCHIVE.dotagame_pcgl) return "dota";
            throw new ShaderParserException("unknown archive");
        }

        public static string GetGpuType(ARCHIVE archive)
        {
            if (archive == ARCHIVE.dotacore_pcgl) return "pcgl";
            if (archive == ARCHIVE.dotagame_pcgl) return "pcgl";
            throw new ShaderParserException("unknown archive");
        }

        public static string GetSourceType(ARCHIVE archive)
        {
            if (archive == ARCHIVE.dotacore_pcgl) return "glsl";
            if (archive == ARCHIVE.dotagame_pcgl) return "glsl";
            throw new ShaderParserException("unknown archive");
        }

        public static string GetVcsToken(ValveResourceFormat.ShaderParser.VcsFileType vcsFiletype)
        {
            if (vcsFiletype == ValveResourceFormat.ShaderParser.VcsFileType.Features)
            {
                return "ft";
            } else
            {
                return vcsFiletype.ToString()[0..^5];
            }
        }



        public static ARCHIVE DetermineArchiveType(string vcsFileName)
        {
            if (Path.GetDirectoryName(vcsFileName).EndsWith("shaders\\vfx"))
            {
                return ARCHIVE.dotagame_pcgl;
            }
            if (Path.GetDirectoryName(vcsFileName).EndsWith("shaders-core\\vfx"))
            {
                return ARCHIVE.dotacore_pcgl;
            }
            throw new ShaderParserException("don't know where this file belongs");
        }






    }
}
