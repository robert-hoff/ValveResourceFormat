using System;
using System.Globalization;
using System.IO;
using NUnit.Framework;
using ValveResourceFormat.CompiledShader;
using static ValveResourceFormat.CompiledShader.ShaderUtilHelpers;

namespace Tests
{
    public class ShaderTest
    {
        [Test]
        public void ParseShaders()
        {
            var path = Path.Combine(TestContext.CurrentContext.TestDirectory, "Files", "Shaders");
            var files = Directory.GetFiles(path, "*.vcs");

            foreach (var file in files)
            {
                var shader = new ShaderCollection();

                using var sw = new StringWriter(CultureInfo.InvariantCulture);
                var originalOutput = Console.Out;
                Console.SetOut(sw);

                shader.Read(file);

                Console.SetOut(originalOutput);
            }
        }

        [Test]
        public void TestVcsSourceTypes()
        {
            var filenamepath1 = "/sourcedir/bloom_dota_mobile_gles_30_ps.vcs";
            Assert.AreEqual(VcsSourceType.MobileGles, GetVcsSourceType(filenamepath1));
            var filenamepath2 = "/sourcedir/bloom_dota_ios_vulkan_40_ps.vcs";
            Assert.AreEqual(VcsSourceType.IosVulkan, GetVcsSourceType(filenamepath2));
            //var filenamepath3 = "/sourcedir/flow_map_preview_android_vulkan_40_vs.vcs";
            //Assert.AreEqual(VcsSourceType.MobileGles, GetVcsSourceType(filenamepath3));
            //var filenamepath4 = "/sourcedir/";
            //Assert.AreEqual(VcsSourceType.MobileGles, GetVcsSourceType(filenamepath4));
            //var filenamepath5 = "/sourcedir/";
            //Assert.AreEqual(VcsSourceType.MobileGles, GetVcsSourceType(filenamepath5));
            //var filenamepath6 = "/sourcedir/";
            //Assert.AreEqual(VcsSourceType.MobileGles, GetVcsSourceType(filenamepath6));
            //var filenamepath7 = "/sourcedir/";
            //Assert.AreEqual(VcsSourceType.MobileGles, GetVcsSourceType(filenamepath7));
            //var filenamepath8 = "/sourcedir/";
            //Assert.AreEqual(VcsSourceType.MobileGles, GetVcsSourceType(filenamepath8));
            //var filenamepath9 = "/sourcedir/";
            //Assert.AreEqual(VcsSourceType.MobileGles, GetVcsSourceType(filenamepath9));
            //var filenamepath10 = "/sourcedir/";
            //Assert.AreEqual(VcsSourceType.MobileGles, GetVcsSourceType(filenamepath10));
            //var filenamepath11 = "/sourcedir/";
            //Assert.AreEqual(VcsSourceType.MobileGles, GetVcsSourceType(filenamepath11));



        }



    }
}
