using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RunVrf.UtilHelpers
{
    public class BatchTestShaderFiless
    {

        public static void RunTrials()
        {
            TestLabV62Set();
        }

        public static void TestLabV62Set()
        {
            FileArchive vcsFiles = new FileArchive(ARCHIVE.the_lab_pc_v62);
            for (int i = 0; i < vcsFiles.GetFileCount(); i++)
            {
                try
                {
                    Debug.WriteLine($"{vcsFiles.GetShaderFile(i)}");
                }
                catch (Exception)
                {

                }
            }
        }

    }
}
