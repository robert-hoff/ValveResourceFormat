using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
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
        private ShaderFileDetail[] cachedShaderFileDetail;
        private Boolean useModularLookup;

        /*
         * Collects all the vcs files in the given archive as FileVcsTokens
         * vcs files must follow the expected naming convention
         * (should always be case when reading shaders from vpk archives)
         *
         *  {name}_{platform}_{shadermodel}_{programtype}.vcs
         *
         *
         */
        public FileArchive(ARCHIVE archive,
            VcsProgramType programType1 = VcsProgramType.Undetermined,
            VcsProgramType programType2 = VcsProgramType.Undetermined,
            VcsShaderModelType shaderModelType = VcsShaderModelType.Undetermined,
            bool useModularLookup = false,
            int LIMIT_NR = 10000)
        {
            this.archive = archive;
            int count = 0;
            foreach (string filenamepath in Directory.GetFiles(FileArchives.GetArchiveDir(archive)))
            {
                if (count++ > LIMIT_NR)
                {
                    break;
                }
                if (filenamepath.EndsWith("vcs"))
                {
                    vcsFiles.Add(new FileVcsTokens(archive, Path.GetFileName(filenamepath)));
                }
            }

            // filter the list based on passed criteria
            Select(programType1, programType2);
            Select(shaderModelType);
        }


        public FileArchive Select(
            VcsProgramType programType1,
            VcsProgramType programType2 = VcsProgramType.Undetermined)
        {
            cachedShaderFileDetail = null;
            if (programType1 == VcsProgramType.Undetermined)
            {
                return this;
            }
            if (programType2 == VcsProgramType.Undetermined)
            {
                programType2 = programType1;
            }
            List<FileVcsTokens> reducedFiles = new();
            foreach (FileVcsTokens vcsFile in vcsFiles)
            {
                if (vcsFile.programType == programType1 || vcsFile.programType == programType2)
                {
                    reducedFiles.Add(vcsFile);
                }
            }
            vcsFiles = reducedFiles;
            return this;
        }

        public FileArchive Select(VcsShaderModelType shaderModelType1)
        {
            if (shaderModelType1 == VcsShaderModelType.Undetermined)
            {
                return this;
            }
            List<FileVcsTokens> reducedFiles = new();
            foreach (FileVcsTokens vcsFile in vcsFiles)
            {
                if (vcsFile.shaderModelType == shaderModelType1)
                {
                    reducedFiles.Add(vcsFile);
                }
            }
            vcsFiles = reducedFiles;
            return this;
        }

        public void UseModularLookup(Boolean useModularLookup)
        {
            this.useModularLookup = useModularLookup;
        }


        public IEnumerable GetFileVcsTokens()
        {
            foreach (var vcsFile in vcsFiles)
            {
                yield return vcsFile;
            }
        }





        /*
        private int GetFileIndex(int queryIndex)
        {
            if (vcsFiles.Count == 0)
            {
                throw new ShaderParserException($"FileArchive.GetFileIndex; archive is empty! queryIndex = {queryIndex}");
            }
            if (!useModularLookup && queryIndex >= vcsFiles.Count)
            {
                throw new ShaderParserException($"FileArchive.GetFileIndex; Index out of range {fileIndex}");
            }
            int fileIndex = queryIndex % vcsFiles.Count;
            return fileIndex;
        }
        */

        private ShaderFileDetail GetShaderFileDetail(int queryIndex)
        {
            if (vcsFiles.Count == 0)
            {
                throw new ShaderParserException($"FileArchive.GetFileIndex; archive is empty! queryIndex = {queryIndex}");
            }
            if (!useModularLookup && queryIndex >= vcsFiles.Count)
            {
                throw new ShaderParserException($"FileArchive.GetFileIndex; Index out of range {queryIndex}");
            }
            int fileIndex = queryIndex % vcsFiles.Count;
            if (cachedShaderFileDetail[fileIndex] == null)
            {
                cachedShaderFileDetail = new ShaderFileDetail[vcsFiles.Count];
            }
            if (cachedShaderFileDetail[fileIndex] == null)
            {
                cachedShaderFileDetail[fileIndex] = new ShaderFileDetail(vcsFiles[fileIndex]);
            }
            return cachedShaderFileDetail[fileIndex];
        }


        /*
         * Attempt to parse the files into ShaderFile. Returns them if successful or reports error.
         *
         */
        public IEnumerable GetShaderFiles()
        {
            for (int i = 0; i < vcsFiles.Count; i++)
            {
                yield return GetShaderFile(i);
            }
        }


        public ShaderFile GetShaderFile(int queryIndex)
        {


            return null;
        }

        /*
         *

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


        */



        private class ShaderFileDetail
        {
            public FileVcsTokens fileVcsTokens;
            public string fileName;
            public ShaderFile shaderFile;
            public int zframeCount;

            public ShaderFileDetail(FileVcsTokens fileVcsTokens)
            {
                this.fileVcsTokens = fileVcsTokens;
                this.fileName = fileVcsTokens.filename;
                this.shaderFile = fileVcsTokens.GetShaderFile();
                this.zframeCount = shaderFile.GetZFrameCount();
            }
        }




    }
}






