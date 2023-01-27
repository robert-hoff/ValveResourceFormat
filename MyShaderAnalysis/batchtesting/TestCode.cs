using MyShaderAnalysis.utilhelpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShaderAnalysis.batchtesting
{
    internal class TestCode
    {

        public static void Run()
        {
            ShowFileListing();
        }

        public static void ShowFileListing()
        {

            // Console.WriteLine($"{ARCHIVE.alyx_hlvr_vulkan_v64}");


            FileArchive fileArchive = new(ARCHIVE.alyx_hlvr_vulkan_v64);
            foreach (var item in fileArchive.GetFileVcsTokens())
            {
                Debug.WriteLine($"{item}");
            }


        }


    }
}
