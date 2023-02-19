using System.Collections.Generic;
using ValveResourceFormat.CompiledShader;

namespace MyShaderAnalysis.snippetcode
{
    public class StatCounter
    {
        private (int, int) totalTally = (0, 0);
        private Dictionary<VcsProgramType, (int, int)> stats = new();

        public StatCounter()
        {
            stats[VcsProgramType.Features] = (0, 0);
            stats[VcsProgramType.VertexShader] = (0, 0);
            stats[VcsProgramType.PixelShader] = (0, 0);
            stats[VcsProgramType.GeometryShader] = (0, 0);
            stats[VcsProgramType.HullShader] = (0, 0);
            stats[VcsProgramType.DomainShader] = (0, 0);
            stats[VcsProgramType.ComputeShader] = (0, 0);
            stats[VcsProgramType.PixelShaderRenderState] = (0, 0);
            stats[VcsProgramType.RaytracingShader] = (0, 0);
        }

        public void recPassed(VcsProgramType vcsProgType)
        {
            stats[vcsProgType] = (stats[vcsProgType].Item1 + 1, stats[vcsProgType].Item2 + 1);
            totalTally = (totalTally.Item1 + 1, totalTally.Item2 + 1);
        }

        public void recFail(VcsProgramType vcsProgType)
        {
            stats[vcsProgType] = (stats[vcsProgType].Item1 + 0, stats[vcsProgType].Item2 + 1);
            totalTally = (totalTally.Item1 + 0, totalTally.Item2 + 1);
        }

        public string getReport()
        {
            int nrPassed = totalTally.Item1;
            int nrFailed = totalTally.Item2 - totalTally.Item1;
            int nrTotal = totalTally.Item2;

            string detailString = "";
            foreach (var fileStat in stats)
            {
                detailString += getVcsStatToken(fileStat);
            }

            // string summaryString = $"SUMMARY/n file-count({nrTotal}) passed[{nrPassed}/{nrTotal}] failed[{nrFailed}/{nrTotal}]";
            string summaryString = $"(summary) {nrPassed} of {nrTotal} files parsed without error";
            if (nrFailed > 0)
            {
                summaryString += $", {nrFailed} file(s) failed";
            }
            return $"{summaryString}\n(detail) {detailString.Trim()}";
        }

        private static string getVcsStatToken(KeyValuePair<VcsProgramType, (int, int)> fileStat)
        {
            // show if there exists at least 1 record
            if (fileStat.Value.Item2 > 0)
            {
                return $"{getVcsToken(fileStat.Key)}[{fileStat.Value.Item1}/{fileStat.Value.Item2}] ";
            } else
            {
                return "";
            }
        }

        private static string getVcsToken(VcsProgramType vcsType)
        {
            return vcsType switch
            {
                VcsProgramType.Features => "features.vcs",
                VcsProgramType.VertexShader => "vs.vcs",
                VcsProgramType.PixelShader => "ps.vcs",
                VcsProgramType.PixelShaderRenderState => "psrs.vcs",
                VcsProgramType.GeometryShader => "gs.vcs",
                VcsProgramType.ComputeShader => "cs.vcs",
                VcsProgramType.HullShader => "hs.vcs",
                VcsProgramType.DomainShader => "ds.vcs",
                VcsProgramType.RaytracingShader => "rtx.vcs",
                _ => throw new ShaderParserException("not possible")
            };
        }
    }
}

