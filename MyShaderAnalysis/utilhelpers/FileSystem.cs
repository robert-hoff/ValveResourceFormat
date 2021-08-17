using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using MyShaderAnalysis.vcsparsing;
using System;
using MyShaderAnalysis.compat;
using static MyShaderAnalysis.vcsparsing.UtilHelpers;



/*
 * NOTE - let's just use forward slashes in ALL cases because this is what HTML wants,
 * and it doesn't make any difference for local files
 *
 *
 * There are different contexts where different bits of info related to names and directories are needed
 *
 *
 * Saving a zframe to html file.
 * - base directory to the server
 * - relative directory to the zframe folder
 * - zframe filename
 * - html title
 * - html header
 *
 *
 * Creating a file-listing over existing zframes
 * - folder where the zframe files are (we'll use an updated folder layout)
 * - standardised zframe filename
 *
 *
 * creating links referring back to old files (specifically the /vcs-all/ bytedumps)
 * - this uses a different folder layout which I currently don't want to change
 * - several of the tokens above will still be the same
 * - define this as the SERVER_BYTEPATH folder
 *
 *
 *
 * Use the names GetDirectory(..) and GetPath(..) to distinguish between file-management and html-linking
 *
 *
 *
 * Now .. it seems natural to also retrieve the files from here. This is definitely an advantage
 * because there are some tokens I can determine immediately if I do that
 *
 *
 *
 *
 *
 *
 */
namespace MyShaderAnalysis.utilhelpers {

    public class FileSystem {

        private const string PROJECT_TESTDIR = "Z:/active/projects/dota2-sourcesdk-modding/shader-analysis-vcs-format/OUTPUT_DUMP";
        private const string SERVER_BASEDIR = "Z:/dev/www/vcs.codecreation.dev";
        private const string SERVER_TESTPATH = "/GEN-output";
        private const string SERVER_BYTEPATH = "/vcs-all";
        private const string DOTA_CORE_PCGL_SOURCE = "X:/dota-2-VRF-exports/dota2-export-shaders-pcgl/shaders-core/vfx";
        private const string DOTA_GAME_PCGL_SOURCE = "X:/dota-2-VRF-exports/dota2-export-shaders-pcgl/shaders/vfx";
        private const string DOTA_CORE_PC_SOURCE = "X:/dota-2-VRF-exports/dota2-export-shaders-pc/shaders-core/vfx";
        private const string DOTA_GAME_PC_SOURCE = "X:/dota-2-VRF-exports/dota2-export-shaders-pc/shaders/vfx";


        public enum ARCHIVE { dotacore_pcgl, dotagame_pcgl };



        public static string GetFilenamepath(ARCHIVE archive, string filename) {
            return $"{GetSourceDir(archive)}/{filename}";
        }

        public static string GetServerBaseDir() {
            return SERVER_BASEDIR;
        }


        public static string GetSourceDir(ARCHIVE archive) {
            if (archive == ARCHIVE.dotacore_pcgl) return DOTA_CORE_PCGL_SOURCE;
            if (archive == ARCHIVE.dotagame_pcgl) return DOTA_GAME_PCGL_SOURCE;
            throw new ShaderParserException("unknown archive");
        }

        public static string GetArchiveName(ARCHIVE archive) {
            if (archive == ARCHIVE.dotacore_pcgl) return "dota-core";
            if (archive == ARCHIVE.dotagame_pcgl) return "dota-game";
            throw new ShaderParserException("unknown archive");
        }

        public static string GetArchiveLabel(ARCHIVE archive) {
            if (archive == ARCHIVE.dotacore_pcgl) return "core";
            if (archive == ARCHIVE.dotagame_pcgl) return "dota";
            throw new ShaderParserException("unknown archive");
        }

        public static string GetGpuType(ARCHIVE archive) {
            if (archive == ARCHIVE.dotacore_pcgl) return "pcgl";
            if (archive == ARCHIVE.dotagame_pcgl) return "pcgl";
            throw new ShaderParserException("unknown archive");
        }

        public static string GetSourceType(ARCHIVE archive) {
            if (archive == ARCHIVE.dotacore_pcgl) return "glsl";
            if (archive == ARCHIVE.dotagame_pcgl) return "glsl";
            throw new ShaderParserException("unknown archive");
        }

        public static string GetVcsToken(FILETYPE vcsFiletype) {
            if (vcsFiletype == FILETYPE.features_file) {
                return "ft";
            } else {
                return vcsFiletype.ToString()[0..^5];
            }
        }



        public static ARCHIVE DetermineArchiveType(string vcsFileName) {
            if (Path.GetDirectoryName(vcsFileName).EndsWith("shaders\\vfx")) {
                return ARCHIVE.dotagame_pcgl;
            }
            if (Path.GetDirectoryName(vcsFileName).EndsWith("shaders-core\\vfx")) {
                return ARCHIVE.dotacore_pcgl;
            }
            throw new ShaderParserException("don't know where this file belongs");
        }






    }
}





