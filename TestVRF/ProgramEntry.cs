using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ValveResourceFormat.ThirdParty;
using ValveResourceFormat.Utils;

namespace TestVRF
{
    class ProgramEntry
    {

        const string DOTA2_VPK_ARCHIVE = @"X:\Steam\steamapps\common\dota 2 beta\game\dota\pak01_dir.vpk";
        const string HLALYX_VPK_ARCHIVE = @"X:\Steam\steamapps\common\Half-Life Alyx\game\hlvr\pak01_dir.vpk";


        static void Main()
        {

            RunVfxEval.RunTrials();
            // ShaderAnalysis.RunTrials();

            // RunParseDynamicExpressions.RunTestData();
            // CheckExternalVarsHashes.checkNames();

            // CollectRenderAttributes.collectAttributes(DOTA2_VPK_ARCHIVE);
            // CollectRenderAttributes.collectAttributes(HLALYX_VPK_ARCHIVE);

            // ReadExpressionsFromPackage.DecompileDynamicExpressions(DOTA2_VPK_ARCHIVE);
            // ReadExpressionsFromPackage.DecompileDynamicExpressions(HLALYX_VPK_ARCHIVE);


        }

    }


}


