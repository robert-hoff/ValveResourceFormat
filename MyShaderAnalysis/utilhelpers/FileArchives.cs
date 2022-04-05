using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using ValveResourceFormat.CompiledShader;
using static ValveResourceFormat.CompiledShader.ShaderUtilHelpers;
using System.Reflection;

/*
 *
 * FileArchives
 *
 * Responsibility of FileArchives is to return tokens specific to base folder and base url of the archive only
 * All the methods are constant to the ARCHIVE enums
 *
 *
 */
namespace MyShaderAnalysis.codestash
{
    public class FileArchives
    {
        // private const string PROJECT_TESTDIR = "Z:/active/projects/dota2-sourcesdk-modding/shader-analysis-vcs-format/OUTPUT_DUMP";
        private const string SERVER_BASEDIR = "Z:/dev/www/vcs.codecreation.dev";
        private const string SERVER_TESTPATH = SERVER_BASEDIR + "/GEN-output";


        public static string GetServerBaseDir()
        {
            return SERVER_BASEDIR;
        }

        public static string GetServerTestDir()
        {
            return SERVER_TESTPATH;
        }

        public static string GetServerTestFile ()
        {
            return SERVER_TESTPATH + "/testfile.html";
        }



        /*
         * Returns directory of the vcs source files. E.g.
         * X:/dota-2-VRF-exports/dota2-export-shaders-pcgl/shaders/vfx
         *
         */
        public static string GetArchiveDir(ARCHIVE archive)
        {
            FieldInfo fieldInfo = typeof(ARCHIVE).GetField(archive.ToString());
            object[] attributes = fieldInfo.GetCustomAttributes(typeof(ArchiveDirectoryAttribute), true);
            return ((ArchiveDirectoryAttribute)attributes[0]).dirName;
        }



        public static string GetFilenamepath(ARCHIVE archive, string filename)
        {
            return $"{GetArchiveDir(archive)}/{filename}";
        }



    }
}



