using MyShaderAnalysis.codestash;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
 *
 * Manage shaderFile lookups through here
 *
 * Do we need to return file listings? Possibly good .. but the important part is to return objects
 *
 * Maybe, return an iterator?
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
        private List<string> vcsFiles = new();

        public FileArchive(ARCHIVE archive)
        {
            this.archive = archive;
            foreach (string filenamepath in Directory.GetFiles(FileArchives.GetArchiveDir(archive)))
            {
                if (filenamepath.EndsWith("vcs"))
                {
                    vcsFiles.Add(filenamepath.Replace("\\", "/"));
                }
            }
        }


        public IEnumerable MyIterator()
        {
            foreach (var item in vcsFiles)
            {
                yield return item;
            }
        }




    }
}






