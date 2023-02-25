using System;
using System.Collections.Generic;
// R: added these
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using ValveResourceFormat;

namespace TestVRFDev
{
    class MyProgram2
    {
        static void Mainz(string[] args)
        {
            // Trace.WriteLine("Hello!");
            processingAlchemistVmdlModel();
        }

        static void processingAlchemistVmdlModel()
        {
            /*
             *
             * does this correspond to the pak01_dir.vpk file?
             *
             *          /models/heroes/alchemist/alchemist.vmdl_c
             *
             * Well it's not the same file, the vpk alchemist.vmdl_c is 893792 bytes
             * This file is 13856 bytes.
             *
             * This file also evaluates with a completely different set of block-types compared
             * to the one in the vpk directory.
             *
             * Well, the file in VRF was added 5 years ago, so it should be assumed that it would change
             * (by very much though)
             *
             *
             *
             */
            string alchemist_file = @"X:\checkouts\ValveResourceFormat\Tests\Files\alchemist.vmdl_c";
            var resource = new Resource
            {
                FileName = alchemist_file,
            };
            resource.Read(alchemist_file);

            var path = @"X:\checkouts\ValveResourceFormat\Tests\Files\ValidOutput";
            // files is a string[] of all the files in the directory
            // var files = Directory.GetFiles(path, "*.*txt", SearchOption.AllDirectories);

            // The expected data is organised into subfolders that match the name of the file being analysed
            // for alchemist.vmdl_c there are 4 associated files
            // X:\checkouts\ValveResourceFormat\Tests\Files\ValidOutput\alchemist.vmdl_c\DATA.txt
            // X:\checkouts\ValveResourceFormat\Tests\Files\ValidOutput\alchemist.vmdl_c\NTRO.txt
            // X:\checkouts\ValveResourceFormat\Tests\Files\ValidOutput\alchemist.vmdl_c\REDI.txt
            // X:\checkouts\ValveResourceFormat\Tests\Files\ValidOutput\alchemist.vmdl_c\RERL.txt

            // the files for alchemists.vmdl_c are
            string[] files = new string[4];
            files[0] = @"X:\checkouts\ValveResourceFormat\Tests\Files\ValidOutput\alchemist.vmdl_c\DATA.txt";
            files[1] = @"X:\checkouts\ValveResourceFormat\Tests\Files\ValidOutput\alchemist.vmdl_c\NTRO.txt";
            files[2] = @"X:\checkouts\ValveResourceFormat\Tests\Files\ValidOutput\alchemist.vmdl_c\REDI.txt";
            files[3] = @"X:\checkouts\ValveResourceFormat\Tests\Files\ValidOutput\alchemist.vmdl_c\RERL.txt";

            foreach (var file in files)
            {
                // Trace.WriteLine(file);

                var blockName = Path.GetFileNameWithoutExtension(file);
                // prints DATA, NTRO, REDI, RERL
                // Trace.WriteLine(blockName);

                // BlockType is an enum type that matches the string names
                BlockType blockType;
                Enum.TryParse(blockName, false, out blockType);

                // also DATA, NTRO, REDI, RERL
                // Trace.WriteLine(blockType);

                // verify each of the blocks in the resource against the reference files
                var actualOutput = resource.GetBlockByType(blockType).ToString();
                var expectedOutput = File.ReadAllText(file);

                Trace.WriteLine(blockType);
                // Trace.WriteLine(actualOutput);
                // break;

                // R: the bytestreams are different, what's going on?
                // for some reason there are extra tabs in expectedOutput[i]
                // The tabs are the character '09'
                // I guess some change must have happened, possibly by VRF they decided at some point to add additional tab spacing
                // (but who's supplying the tabs?)
                // 50 65 72 6d 4d 6f 64 65 6c 44 61 74 61 5f 74 0d 0a 7b 0d 0a 09 43 52 65 73 6f 75 72 63
                // 50 65 72 6d 4d 6f 64 65 6c 44 61 74 61 5f 74 0d 0a 09 7b 0d 0a 09 09 43 52 65 73 6f 75

                /*
                for (int i=0; i<100; i++)
                {
                    string val = String.Format("{0:x2} ", (byte) actualOutput[i]);
                    Trace.Write(val);
                }
                Trace.WriteLine("");
                for (int i = 0; i < 100; i++)
                {
                    string val = String.Format("{0:x2} ", (byte) expectedOutput[i]);
                    Trace.Write(val);
                }
                Trace.WriteLine("");
                */

                // From the VRF comments
                // "We don't care about Valve's messy whitespace, so just strip it."
                //
                // R: but did the whitespace for the DATA block come from the Valve file?
                // It seems like in the extraction we are not dealing with formatted data
                //
                // is the data from a data decompression that gives data with tabs?
                // I doubt this because it looks to me like the DATA block was built up somehow by VRF
                actualOutput = Regex.Replace(actualOutput, @"\s+", string.Empty);
                expectedOutput = Regex.Replace(expectedOutput, @"\s+", string.Empty);

                //Assert.AreEqual(expectedOutput, actualOutput);

                // the test was built assuming these should be the same, but
                // the assert was commented out and we can see here that they are actually not exactly the same
                //
                if (expectedOutput != actualOutput)
                {
                    Trace.WriteLine("NOT THE SAME");
                }
                else
                {
                    Trace.WriteLine("SAME");
                }

                // Trace.WriteLine(blockType);
                // Trace.WriteLine(actualOutput);
                // Trace.WriteLine(expectedOutput);

                // The test doesn't pass for the DATA block, we have among the expectedOutput
                // ExternalReference m_refMeshes[2] =
                // [
                //   ID: 84252A6CCBF12100
                //   ID: 98FCF82D85DCF76C
                // ]

                // And for the actualOutput
                // ExternalReference m_refMeshes[2] =
                // [
                //   resource: "models/heroes/alchemist/alchemist_model.vmesh"
                //   resource: "models/heroes/alchemist/alchemist_lod.vmesh"
                // ]

                // Also the resolution of the floats are different, in expectedOutput
                // float32 m_boneSphere[67] =
                // [
                //   41.146270
                //   66.513230
                // ...

                // In actualOutput
                // float32 m_boneSphere[67] =
                // [
                //   41.146267
                //   66.513229
                // ...

                // So it's evident that small teaks have been made to how the data blocks have been read
                // This must be a VRF thing, because the file supplied by the test had been unchanged
                // in the repository since 2015
                //
                // I suppose they must have figured out a way to get those
                // alchemist_model.vmesh and alchemist_lod.vmesh from the vmdl_c file
            }
            resource.Dispose();
        }

        /*
         * This is the first part of the test in Tests/Test.cl method ReadlBlocks()
         * I don't understand all the class reflection stuff, GetMember and GetCustomAttributes
         * are System methods
         *
         *              typeof(ResourceType).GetMember
         *              (ExtensionAttribute) type.GetCustomAttributes(typeof(ExtensionAttribute), false)
         *
         *
         * It seems this part of the test just checks that the file can be read and that the
         * extension is a recognised type
         *
         *
         */
        static void extensionAndResourceTypeAttribute()
        {
            string file = @"X:\checkouts\ValveResourceFormat\Tests\Files\alchemist.vmdl_c";
            var resource = new Resource
            {
                FileName = file,
            };
            resource.Read(file);
            // resource.ResourceType will
            Trace.WriteLine(resource.ResourceType); // prints Model, ResourceType is an enum

            // R: This gets the extension and removes the _c part
            // For the file alchemist.vmdl_c extension becomes .vmdl
            var extension = Path.GetExtension(file);
            if (extension.EndsWith("_c", StringComparison.Ordinal))
            {
                extension = extension.Substring(0, extension.Length - 2);
            }

            // var type = typeof(ResourceType).GetMember(resource.ResourceType.ToString()).First();

            // R: this is equivalent to
            // var type = typeof(ResourceType).GetMember("Model").First();

            // this GetMember("Model") returns the array MemberInfo[]
            // typeof(ResourceType).GetMember("Model");

            // so we can also write
            // var type = typeof(ResourceType).GetMember("Model")[0];
            var type = typeof(ResourceType).GetMember("Model").First();

            // this will print "ValveResourceFormat.ResourceType Model"
            // so it must be understood that this is the enumerated type associated with the Model type
            // or .. the system (Visual Studio) identifies type as 'System.Reflection.MemberInfo'
            Trace.WriteLine(type);

            // attribute is just evaluated as the string ".vmdl"
            var attribute = "." + ((ExtensionAttribute) type.GetCustomAttributes(typeof(ExtensionAttribute), false).First()).Extension;
            Trace.WriteLine(attribute);

            // The test in ReadBlocks() checks that extension and attribute are the same
            // they are both the string ".vmdl"

            // Assert.AreEqual(extension, attribute, file);
            resource.Dispose();
        }

        static void listFilesInDirectory()
        {
            var resources = new Dictionary<string, Resource>();
            string FILEPATH = @"X:\checkouts\ValveResourceFormat\Tests\Files";
            var files = Directory.GetFiles(FILEPATH, "*.*_c");

            foreach (string s in files)
            {
                Trace.WriteLine(s);
            }
        }

        // from Tests/Test.cs, PackageInResourceThrows()
        static void codeFromTestClass()
        {
            // R: the vpk magic number is is 34 12 AA 55
            // and a vpk file bytesream will start as
            // 34 12 AA 55 02 00 00 00

            // Resource seems to be generic type to represent a "Valve Resource"
            Resource resource = new Resource();
            var data = new byte[] { 0x34, 0x12, 0xAA, 0x55, 0x02, 0x00 };
            // This memory stream seems to be part of System.IO
            MemoryStream ms = new MemoryStream(data);

            // If I pass this data to the resource the exception "Use ValvePak library to parse VPK files." will be thrown
            // i.e. the resource is indicating that it is not meant for vpk archives
            // resource.Read(ms);
            resource.Dispose();
        }
    }
}

