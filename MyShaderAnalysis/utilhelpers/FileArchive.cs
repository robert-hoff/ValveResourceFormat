using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using ValveResourceFormat.CompiledShader;

/*
 *
 * Manage shaderFile lookups through here
 *
 *
 *
 *
 */
namespace MyShaderAnalysis.utilhelpers
{
    public class FileArchive
    {
        private ARCHIVE archive;
        private List<FileVcsTokens> vcsFiles = new();

        private VcsProgramType programType = VcsProgramType.Undetermined;
        private VcsShaderModelType shaderModelType = VcsShaderModelType.Undetermined;

        /*
         * Collects all the vcs files in the given archive as FileVcsTokens
         * It will fail if the vcs files don't observe the expected naming convention
         *
         *  {name}_{platform}_{shadermodel}_{programtype}.vcs
         *
         *
         */
        public FileArchive(ARCHIVE archive,
            VcsProgramType programType = VcsProgramType.Undetermined,
            VcsShaderModelType shaderModelType = VcsShaderModelType.Undetermined)
        {
            this.archive = archive;
            this.programType = programType;
            this.shaderModelType = shaderModelType;
            foreach (string filenamepath in Directory.GetFiles(FileArchives.GetArchiveDir(archive)))
            {
                if (filenamepath.EndsWith("vcs"))
                {
                    vcsFiles.Add(new FileVcsTokens(archive, Path.GetFileName(filenamepath)));
                }
            }
        }

        private List<FileVcsTokens> ReduceFileListing()
        {
            List<FileVcsTokens> reducedFiles = new();
            foreach (FileVcsTokens vcsFile in vcsFiles)
            {
                if ((programType == VcsProgramType.Undetermined || programType == vcsFile.programType) &&
                    (shaderModelType == VcsShaderModelType.Undetermined || shaderModelType == vcsFile.shaderModelType))
                {
                    reducedFiles.Add(vcsFile);
                }
            }
            return reducedFiles;
        }


        /*
         * Attempt to parse the files into ShaderFile. Returns them if successful or reports error.
         *
         */
        public IEnumerable ShaderFiles()
        {
            foreach (var vcsFile in ReduceFileListing())
            {
                ShaderFile shaderFile = null;
                try
                {
                    shaderFile = vcsFile.GetShaderFile();
                }
                // may throw ShaderParserException or UnexpectedMagicException
                catch (Exception e)
                {
                    Console.WriteLine($"Error couldn't parse {vcsFile.filename} {e.Message}");
                    continue;
                }
                yield return shaderFile;
            }
        }


        /*
         * Returns the byte content of the vcs files only as byte[]
         *
         */
        public IEnumerable ShaderFileBytes()
        {
            foreach (var vcsFile in ReduceFileListing())
            {


                yield return null;
            }
        }




    }
}






