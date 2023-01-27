using System;
using System.IO;
using System.Security.Cryptography;
using NUnit.Framework;
using ValveResourceFormat;
using ValveResourceFormat.ResourceTypes;

namespace Tests
{
    [TestFixture]
    public class SoundTest
    {
        [Test]
        public void TestSound()
        {
            var file = Path.Combine(TestContext.CurrentContext.TestDirectory, "Files", "beep.vsnd_c");
            using var resource = new Resource
            {
                FileName = file,
            };
            resource.Read(file);

            Assert.AreEqual(ResourceType.Sound, resource.ResourceType);

            using var hash = SHA256.Create();
            using var sound = ((Sound)resource.DataBlock).GetSoundStream();
            var actualHash = BitConverter.ToString(hash.ComputeHash(sound)).Replace("-", "", StringComparison.Ordinal);

            Assert.AreEqual("1F8BF83F3E827A3C02C6AE6B6BD23BBEBD4E18C4F877D092CF0C5B800DAAB2B7", actualHash);
        }
    }
}
