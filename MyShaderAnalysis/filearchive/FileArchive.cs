using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using MyShaderFile.CompiledShader;

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
            foreach (string filenamepath in Directory.GetFiles(FileArchive.GetArchiveDir(archive)))
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

        public string ArchiveName()
        {
            return archive.ToString();
        }

        public IEnumerable GetFileVcsTokens()
        {
            foreach (FileVcsTokens vcsFile in vcsFiles)
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
            }
            catch (Exception)
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

        /*
         * Returns directory of the vcs source files. E.g.
         * X:/dota-2-VRF-exports/dota2-export-shaders-pcgl/shaders/vfx
         *
         */
        public static string GetArchiveDir(ARCHIVE archive)
        {
            FieldInfo fieldInfo = typeof(ARCHIVE).GetField(archive.ToString());
            object[] attributes = fieldInfo.GetCustomAttributes(typeof(ArchiveDirectoryAttribute), true);
            return ((ArchiveDirectoryAttribute) attributes[0]).dirName;
        }

        public static string GetFilenamepath(ARCHIVE archive, string filename)
        {
            return $"{GetArchiveDir(archive)}/{filename}";
        }

        // Old file lookups, should really fix these
        public static List<string> GetVcsFiles(string dir1, VcsProgramType fileType,
            int numEnding = -1, bool sortFiles = true, int LIMIT_NR = 1000)
        {
            return GetVcsFiles(dir1, null, fileType, numEnding, sortFiles, LIMIT_NR);
        }

        public static List<string> GetVcsFiles(string dir1, string dir2 = null,
            VcsProgramType fileType = VcsProgramType.Undetermined,
            int numEnding = -1, bool sortFiles = true, int LIMIT_NR = 1000)
        {
            List<string> filesFound = new();
            if (fileType == VcsProgramType.Features || fileType == VcsProgramType.Undetermined)
            {
                string endsWith = numEnding > -1 ? $"{numEnding}_features.vcs" : "features.vcs";
                filesFound.AddRange(GetAllFilesWithEnding(dir1, endsWith));
                filesFound.AddRange(GetAllFilesWithEnding(dir2, endsWith));
            }
            if (fileType == VcsProgramType.VertexShader || fileType == VcsProgramType.Undetermined)
            {
                string endsWith = numEnding > -1 ? $"{numEnding}_vs.vcs" : "vs.vcs";
                filesFound.AddRange(GetAllFilesWithEnding(dir1, endsWith));
                filesFound.AddRange(GetAllFilesWithEnding(dir2, endsWith));
            }
            if (fileType == VcsProgramType.PixelShader || fileType == VcsProgramType.Undetermined)
            {
                string endsWith = numEnding > -1 ? $"{numEnding}_ps.vcs" : "ps.vcs";
                filesFound.AddRange(GetAllFilesWithEnding(dir1, endsWith));
                filesFound.AddRange(GetAllFilesWithEnding(dir2, endsWith));
            }
            if (fileType == VcsProgramType.GeometryShader || fileType == VcsProgramType.Undetermined)
            {
                string endsWith = numEnding > -1 ? $"{numEnding}_gs.vcs" : "gs.vcs";
                filesFound.AddRange(GetAllFilesWithEnding(dir1, endsWith));
                filesFound.AddRange(GetAllFilesWithEnding(dir2, endsWith));
            }
            if (fileType == VcsProgramType.PixelShaderRenderState || fileType == VcsProgramType.Undetermined)
            {
                string endsWith = numEnding > -1 ? $"{numEnding}_psrs.vcs" : "psrs.vcs";
                filesFound.AddRange(GetAllFilesWithEnding(dir1, endsWith));
                filesFound.AddRange(GetAllFilesWithEnding(dir2, endsWith));
            }
            if (fileType == VcsProgramType.DomainShader || fileType == VcsProgramType.Undetermined)
            {
                string endsWith = numEnding > -1 ? $"{numEnding}_ds.vcs" : "ds.vcs";
                filesFound.AddRange(GetAllFilesWithEnding(dir1, endsWith));
                filesFound.AddRange(GetAllFilesWithEnding(dir2, endsWith));
            }
            if (fileType == VcsProgramType.HullShader || fileType == VcsProgramType.Undetermined)
            {
                string endsWith = numEnding > -1 ? $"{numEnding}_hs.vcs" : "hs.vcs";
                filesFound.AddRange(GetAllFilesWithEnding(dir1, endsWith));
                filesFound.AddRange(GetAllFilesWithEnding(dir2, endsWith));
            }
            if (fileType == VcsProgramType.RaytracingShader || fileType == VcsProgramType.Undetermined)
            {
                string endsWith = numEnding > -1 ? $"{numEnding}_rtx.vcs" : "rtx.vcs";
                filesFound.AddRange(GetAllFilesWithEnding(dir1, endsWith));
                filesFound.AddRange(GetAllFilesWithEnding(dir2, endsWith));
            }
            if (sortFiles)
            {
                filesFound.Sort();
            }
            if (filesFound.Count <= LIMIT_NR)
            {
                return filesFound;
            }
            else
            {
                List<string> returnFiles = new();
                for (int i = 0; i < LIMIT_NR; i++)
                {
                    returnFiles.Add(filesFound[i]);
                }
                return returnFiles;
            }
        }

        public static List<string> GetAllFilesWithEnding(string dir, string endsWith)
        {
            List<string> filesFound = new();
            if (dir == null)
            {
                return filesFound;
            }

            foreach (string filenamepath in Directory.GetFiles(dir))
            {
                if (filenamepath.EndsWith(endsWith))
                {
                    filesFound.Add(filenamepath.Replace("\\", "/"));
                }
            }
            return filesFound;
        }
    }
}


