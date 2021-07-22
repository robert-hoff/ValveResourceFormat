using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestVRF
{
    class ProgramEntry
    {

        const string DOTA2_VPK_ARCHIVE = @"X:\Steam\steamapps\common\dota 2 beta\game\dota\pak01_dir.vpk";
        const string HLALYX_VPK_ARCHIVE = @"X:\Steam\steamapps\common\Half-Life Alyx\game\hlvr\pak01_dir.vpk";


        static void Main()
        {

            // RunParseDynamicExpressions.RunTestData();
            // ReadExpressionsFromPackage.DecompileDynamicExpressions(HLALYX_VPK_ARCHIVE);
            ReadExpressionsFromPackage.DecompileDynamicExpressions(DOTA2_VPK_ARCHIVE);


        }

    }


}


