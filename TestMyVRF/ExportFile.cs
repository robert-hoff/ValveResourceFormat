using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using MyValveResourceFormat;
using MyValveResourceFormat.IO;
using MyValveResourceFormat.ResourceTypes;

namespace TestMyVRF {
    public static class ExportFile {

        public static void Export(string fileName, Resource resource) {
            var data = FileExtract.Extract(resource).ToArray();

            string res = "";
            foreach(byte b in data) {
                res += (char) b;
            }

            // Debug.WriteLine(res);
        }


    }
}




