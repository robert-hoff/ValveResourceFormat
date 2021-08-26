using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ValveResourceFormat.CompiledShader;

namespace ShaderAnalysis.utilhelpers
{

    class FileHelpers
    {

        public static List<string> GetVcsFiles(string dir1, VcsProgramType programType) {
            return GetVcsFiles(dir1, null, programType, -1);
        }


        public static List<string> GetVcsFiles(string dir1, string dir2, VcsProgramType programType, int numEnding, bool sortFiles = true) {
            List<string> filesFound = new();
            if (programType == VcsProgramType.Features || programType == VcsProgramType.Undetermined) {
                string endsWith = numEnding > -1 ? $"{numEnding}_features.vcs" : "features.vcs";
                filesFound.AddRange(GetAllFilesWithEnding(dir1, endsWith));
                filesFound.AddRange(GetAllFilesWithEnding(dir2, endsWith));
            }
            if (programType == VcsProgramType.VertexShader || programType == VcsProgramType.Undetermined) {
                string endsWith = numEnding > -1 ? $"{numEnding}_vs.vcs" : "vs.vcs";
                filesFound.AddRange(GetAllFilesWithEnding(dir1, endsWith));
                filesFound.AddRange(GetAllFilesWithEnding(dir2, endsWith));
            }
            if (programType == VcsProgramType.PixelShader || programType == VcsProgramType.Undetermined) {
                string endsWith = numEnding > -1 ? $"{numEnding}_ps.vcs" : "ps.vcs";
                filesFound.AddRange(GetAllFilesWithEnding(dir1, endsWith));
                filesFound.AddRange(GetAllFilesWithEnding(dir2, endsWith));
            }
            if (programType == VcsProgramType.GeometryShader || programType == VcsProgramType.Undetermined) {
                string endsWith = numEnding > -1 ? $"{numEnding}_gs.vcs" : "gs.vcs";
                filesFound.AddRange(GetAllFilesWithEnding(dir1, endsWith));
                filesFound.AddRange(GetAllFilesWithEnding(dir2, endsWith));
            }
            if (programType == VcsProgramType.PixelShaderRenderState || programType == VcsProgramType.Undetermined) {
                string endsWith = numEnding > -1 ? $"{numEnding}_psrs.vcs" : "psrs.vcs";
                filesFound.AddRange(GetAllFilesWithEnding(dir1, endsWith));
                filesFound.AddRange(GetAllFilesWithEnding(dir2, endsWith));
            }
            if (sortFiles) {
                filesFound.Sort();
            }
            return filesFound;
        }


        public static List<string> GetAllFilesWithEnding(string dir, string endsWith) {
            List<string> filesFound = new();
            if (dir == null) {
                return filesFound;
            }
            foreach (string filenamepath in Directory.GetFiles(dir)) {
                if (filenamepath.EndsWith(endsWith)) {
                    filesFound.Add(filenamepath);
                }
            }
            return filesFound;
        }







    }
}








