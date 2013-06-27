using System.IO;
using NUnit.Framework;

namespace KaBlooey.IntegrationTests
{
    [TestFixture]
    public class KaBlooeyDeleteTests: KaBlooeyIntegrationTests
    {
        [Test]
        public void WhenIHaveAFileInTheOldFolderThatDoesntExistInTheNew_MarkTheFileAsDeleted()
        {
            ClearFolders(_oldFolderLocation, _newFolderLocation, _patchFolderLocation);
            AddFileToFolder(_oldFolderLocation, "addFile.txt");
            Directory.CreateDirectory(_newFolderLocation);

            KaBlooey.KaBlooeyEngine.CreatePatch(_oldFolderLocation, _newFolderLocation, _patchFolderLocation);
            var result = File.Exists(Path.Combine(_patchFolderLocation, "addFile.txt.delete"));
            Assert.AreEqual(true, result);
        }
        
        [Test]
        public void WhenIHaveAFileInTheChildOldFolderThatDoesntExistInTheNew_MarkTheFileAsDeleted()
        {
            var childFolder = "deletedChild\\";
            ClearFolders(_oldFolderLocation, _newFolderLocation, _patchFolderLocation);
            AddFileToFolder(Path.Combine(_oldFolderLocation, childFolder), "addFile.txt");
            Directory.CreateDirectory(_newFolderLocation);

            KaBlooey.KaBlooeyEngine.CreatePatch(_oldFolderLocation, _newFolderLocation, _patchFolderLocation);
            var result = File.Exists(Path.Combine(_patchFolderLocation, childFolder, "addFile.txt.delete"));
            Assert.AreEqual(true, result);
        }

        [Test]
        public void WhenIHaveAFileInTheOldFolderThatIsMarkedForDeletion_ThenDeleteTheFile()
        {
            ClearFolders(_oldFolderLocation, _newFolderLocation, _patchFolderLocation);
            AddFileToFolder(_patchFolderLocation, "addFile.txt.delete");
            AddFileToFolder(_newFolderLocation, "addFile.txt");

            Directory.CreateDirectory(_newFolderLocation);

            KaBlooey.KaBlooeyEngine.ApplyPatch(_patchFolderLocation, _newFolderLocation);
            var result = File.Exists(Path.Combine(_newFolderLocation, "addFile.txt"));
            Assert.AreEqual(false, result);

        }
    }

    [TestFixture]
    public class ChangedFileTests
    {
        [Test]
        public void WhenIChangeAFileInTheNewDirectory_ItShouldBeDeltaDiffed()
        {
            
        }
    }
}