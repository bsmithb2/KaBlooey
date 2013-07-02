using System;
using System.IO;
using Ionic.Zip;
using KaBlooey.Engine;
using NUnit.Framework;

namespace KaBlooey.IntegrationTests
{
    [TestFixture]
    public class KaBlooeyPackageAndZipTests : KaBlooeyIntegrationTests
    {
        [Test]
        public void WhenIProduceAPackageAndAskForAZipFile_IShouldRecieveAZipFile()
        {
            ClearFolders(_oldFolderLocation, _newFolderLocation, _patchFolderLocation);
            AddFileToFolder(_newFolderLocation, "addFile.txt");
            Directory.CreateDirectory(_oldFolderLocation);
            var patchFileLocation = _patchFolderLocation + "\\patch.zip";
            KaBlooeyEngine.CreatePatchIntoZip(_oldFolderLocation, _newFolderLocation, patchFileLocation);

            var result = File.Exists(patchFileLocation);
            Assert.AreEqual(true, result);

            using (Ionic.Zip.ZipFile file = new ZipFile(patchFileLocation))
            {
                Assert.IsTrue(file.ContainsEntry("addFile.txt.add"));
            }
        }

        [Test]
        public void WhenIProduceAPackageAndAskForAZipFileAndUseThatZipFileToApplyPatch_IShouldGetANewFile()
        {
            ClearFolders(_oldFolderLocation, _newFolderLocation, _patchFolderLocation);
            AddFileToFolder(_newFolderLocation, "addFile.txt");
            Directory.CreateDirectory(_oldFolderLocation);
            var patchFileLocation = _patchFolderLocation + "\\patch.zip";
            KaBlooeyEngine.CreatePatchIntoZip(_oldFolderLocation, _newFolderLocation, patchFileLocation);
            KaBlooeyEngine.ApplyPatchFromZip(_oldFolderLocation, patchFileLocation);

            Assert.IsTrue(File.Exists(Path.Combine(_oldFolderLocation, "addFile.txt")));
        }
        
        [Test]
        [ExpectedException(typeof(ApplicationException))]
        public void WhenApplyingZipFilePatches_FileDoesntExist_ShouldThrowException()
        {
            KaBlooeyEngine.ApplyPatchFromZip("", "nonexistent.zip");
        }
    }


}