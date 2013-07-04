using System.IO;
using KaBlooey.Engine;
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

            KaBlooeyEngine.CreatePatch(_oldFolderLocation, _newFolderLocation, _patchFolderLocation);
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

            KaBlooeyEngine.CreatePatch(_oldFolderLocation, _newFolderLocation, _patchFolderLocation);
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

            KaBlooeyEngine.ApplyPatch(_patchFolderLocation, _newFolderLocation, true);
            var result = File.Exists(Path.Combine(_newFolderLocation, "addFile.txt"));
            Assert.AreEqual(false, result);

        }

        [Test]
        public void WhenIHaveFilesInMultipleFolders_TheyBothShouldDeleteOnce()
        {
            ClearFolders(_oldFolderLocation, _newFolderLocation, _patchFolderLocation);
            AddFileToFolder(_oldFolderLocation, "delete.txt");
            AddFileToFolder(_oldFolderLocation, "change.txt");
            AddFileToFolder(_newFolderLocation, "change.txt");
            AddFileToFolder(_newFolderLocation, "add.txt");
            Directory.CreateDirectory(Path.Combine(_oldFolderLocation, "patchsubfolder"));
            AddFileToFolder(_oldFolderLocation, "patchsubfolder\\subfolderFile.txt");
            Directory.CreateDirectory(Path.Combine(_newFolderLocation, "patchsubfolder"));
            AddFileToFolder(_oldFolderLocation, "patchsubfolder\\change.txt");
            AddFileToFolder(_newFolderLocation, "patchsubfolder\\change.txt");
            AddFileToFolder(_newFolderLocation, "patchsubfolder\\add.txt");
            
            Directory.CreateDirectory(_newFolderLocation);

            KaBlooeyEngine.CreatePatch(_oldFolderLocation, _newFolderLocation, _patchFolderLocation);
            var result = File.Exists(Path.Combine(_patchFolderLocation, "delete.txt.delete"));
            Assert.AreEqual(true, result);

            result = File.Exists(Path.Combine(_patchFolderLocation, "patchsubfolder\\subfolderFile.txt.delete"));
            Assert.AreEqual(true, result);
        }
    }
}