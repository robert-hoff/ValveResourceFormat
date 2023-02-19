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
namespace MyShaderAnalysis.filearchive
{
    public class FileArchive
    {
        public ARCHIVE archive;
        private List<FileVcsTokens> vcsFiles = new();
        private ShaderFileDetail[] cachedShaderFileDetail;
        private bool useModularLookup;

        /*
         * Collects all the vcs files in the given archive as FileVcsTokens
         * vcs files must follow the expected naming convention
         * (should always be case when reading shaders from vpk archives)
         *
         *  {name}_{platform}_{shadermodel}_{programtype}.vcs
         *
         *
         */
        // signature (1,1,1)
        public FileArchive(ARCHIVE archive,
            VcsProgramType programType1,
            VcsProgramType programType2,
            VcsProgramType programType3,
            VcsShaderModelType shaderModelType = VcsShaderModelType.Undetermined,
            bool useModularLookup = false,
            int maxFiles = 10000
            )
        {
            this.archive = archive;
            this.useModularLookup = useModularLookup;
            int count = 0;
            foreach (string filenamepath in Directory.GetFiles(FileArchives.GetArchiveDir(archive)))
            {
                if (count++ > maxFiles)
                {
                    break;
                }
                if (filenamepath.EndsWith("vcs"))
                {
                    vcsFiles.Add(new FileVcsTokens(archive, Path.GetFileName(filenamepath)));
                }
            }

            // filter the list based on passed criteria
            Select(programType1, programType2, programType3);
            Select(shaderModelType);
        }

        // signature (0,0,0)
        public FileArchive(ARCHIVE archive,
            bool useModularLookup = false,
            int maxFiles = 10000) : this(archive,
                VcsProgramType.Undetermined, VcsProgramType.Undetermined, VcsProgramType.Undetermined,
                VcsShaderModelType.Undetermined, useModularLookup, maxFiles)
        { }

        // signature (1,0,0)
        public FileArchive(ARCHIVE archive,
            VcsProgramType programType1,
            bool useModularLookup = false,
            int maxFiles = 10000) : this(archive,
                programType1, VcsProgramType.Undetermined, VcsProgramType.Undetermined,
                VcsShaderModelType.Undetermined, useModularLookup, maxFiles)
        { }

        // signature (0,0,1)
        public FileArchive(ARCHIVE archive,
            VcsShaderModelType shaderModelType,
            bool useModularLookup = false,
            int maxFiles = 10000) : this(archive,
                VcsProgramType.Undetermined, VcsProgramType.Undetermined, VcsProgramType.Undetermined,
                shaderModelType, useModularLookup, maxFiles)
        { }

        // signature (1,0,1)
        public FileArchive(ARCHIVE archive,
            VcsProgramType programType1,
            VcsShaderModelType shaderModelType,
            bool useModularLookup = false,
            int maxFiles = 10000) : this(archive, programType1, VcsProgramType.Undetermined, VcsProgramType.Undetermined,
                shaderModelType, useModularLookup, maxFiles)
        { }

        // signature (1,1,0)
        public FileArchive(ARCHIVE archive,
            VcsProgramType programType1,
            VcsProgramType programType2,
            bool useModularLookup = false,
            int maxFiles = 10000) : this(archive,
                programType1, programType2, VcsProgramType.Undetermined,
                VcsShaderModelType.Undetermined, useModularLookup, maxFiles)
        { }

        public FileArchive Select(
        VcsProgramType programType1,
        VcsProgramType programType2 = VcsProgramType.Undetermined,
        VcsProgramType programType3 = VcsProgramType.Undetermined)
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
            if (programType3 == VcsProgramType.Undetermined)
            {
                programType3 = programType2;
            }
            List<FileVcsTokens> reducedFiles = new();
            foreach (FileVcsTokens vcsFile in vcsFiles)
            {
                if (vcsFile.programType == programType1 || vcsFile.programType == programType2 || vcsFile.programType == programType3)
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

        public void UseModularLookup(bool useModularLookup)
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

        public int GetFileCount()
        {
            return vcsFiles.Count;
        }

        public FileVcsTokens GetFileVcsTokens(int queryIndex)
        {
            return vcsFiles[useModularLookup ? queryIndex % vcsFiles.Count : queryIndex];
        }


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
            if (cachedShaderFileDetail == null)
            {
                cachedShaderFileDetail = new ShaderFileDetail[vcsFiles.Count];
            }
            if (cachedShaderFileDetail[fileIndex] == null)
            {
                cachedShaderFileDetail[fileIndex] = new ShaderFileDetail(vcsFiles[fileIndex], useModularLookup);
            }
            return cachedShaderFileDetail[fileIndex];
        }

        /*
         * Iterate the ShaderFile's
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
            try
            {
                return GetShaderFileDetail(queryIndex).shaderFile;
            } catch (Exception e)
            {
                throw new ShaderParserException($"FileArchive.GetShaderFile error parsing file {vcsFiles[queryIndex % vcsFiles.Count]}");
            }
        }

        public int GetZFrameCount(int queryIndex)
        {
            return GetShaderFileDetail(queryIndex).zframeCount;
        }

        public ZFrameFile GetZFrameFile(int queryIndexShader, int queryIndexFrame)
        {
            return GetShaderFileDetail(queryIndexShader).GetZFrameFile(queryIndexFrame);
        }

        public int GetSourceCount(int queryIndexShader, int queryIndexFrame)
        {
            return GetShaderFileDetail(queryIndexShader).GetSourceCount(queryIndexFrame);
        }

        private class ShaderFileDetail
        {
            public FileVcsTokens fileVcsTokens;
            public bool useModularLookup;
            public string fileName;
            public ShaderFile shaderFile;
            public int zframeCount;
            private ZFrameFile[] zFrameFiles;

            public ShaderFileDetail(FileVcsTokens fileVcsTokens, bool useModularLookup)
            {
                this.fileVcsTokens = fileVcsTokens;
                this.useModularLookup = useModularLookup;
                fileName = fileVcsTokens.filename;
                shaderFile = fileVcsTokens.GetShaderFile();
                zframeCount = shaderFile.GetZFrameCount();
                zFrameFiles = new ZFrameFile[zframeCount];
            }

            public ZFrameFile GetZFrameFile(int queryIndex)
            {
                if (zframeCount == 0)
                {
                    throw new ShaderParserException($"ShaderFileDetail.GetZFrameFile; no zframes defined, on queryIndex = {queryIndex}");
                }
                if (!useModularLookup && queryIndex >= zframeCount)
                {
                    throw new ShaderParserException($"ShaderFileDetail.GetZFrameFile; Index out of range {queryIndex}");
                }
                int fileIndex = queryIndex % zframeCount;
                if (zFrameFiles[fileIndex] == null)
                {
                    zFrameFiles[fileIndex] = shaderFile.GetZFrameFileByIndex(fileIndex);
                }
                return zFrameFiles[fileIndex];
            }

            public int GetSourceCount(int queryIndex)
            {
                return GetZFrameFile(queryIndex).GpuSourceCount;
            }
        }
    }
}

