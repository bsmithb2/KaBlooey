using System.IO;
using NUnit.Framework;

namespace KaBlooey.IntegrationTests
{
    [TestFixture]
    public class KaBlooeyChangedFileTests : KaBlooeyIntegrationTests
    {
        [Test]
        public void WhenIChangeAFileInTheNewDirectory_ItShouldBeDeltaDiffed()
        {
            ClearFolders(_oldFolderLocation, _newFolderLocation, _patchFolderLocation);
            const string textInFile = "This is some text";
            AddFileToFolderWithText(_newFolderLocation, "addFile.txt", textInFile);
            AddFileToFolderWithText(_oldFolderLocation, "addFile.txt", textInFile);

            KaBlooey.KaBlooeyEngine.CreatePatch(_oldFolderLocation, _newFolderLocation, _patchFolderLocation);
            var fileLocation = Path.Combine(_patchFolderLocation, "addFile.txt.changed");
            var result = File.Exists(fileLocation);
            Assert.AreEqual(true, result);

            var fileText = File.ReadAllText(fileLocation);
            Assert.AreEqual(128, fileText.Length);
        }

        [Test]
        public void WhenIChangeAFileInTheChildNewDirectory_ItShouldBeDeltaDiffed()
        {
            const string childFolder = "childChanged\\";
            ClearFolders(_oldFolderLocation, _newFolderLocation, _patchFolderLocation);
            const string textInFile = "This is some text";
            AddFileToFolderWithText(Path.Combine(_newFolderLocation, childFolder), "addFile.txt", textInFile);
            AddFileToFolderWithText(Path.Combine(_oldFolderLocation, childFolder), "addFile.txt", textInFile);

            KaBlooey.KaBlooeyEngine.CreatePatch(_oldFolderLocation, _newFolderLocation, _patchFolderLocation);
            var fileLocation = Path.Combine(Path.Combine(_patchFolderLocation, childFolder), "addFile.txt.changed");
            var result = File.Exists(fileLocation);
            Assert.AreEqual(true, result);

            var fileText = File.ReadAllText(fileLocation);
            Assert.AreEqual(128, fileText.Length);
        }
        
        [Test]
        public void WhenIChangeAFileInTheNewChildDirectoryAndProduceADiff_ItShouldBeAppliedToTheOldFileInTheChildDirectory()
        {
            const string childFolder = "childChanged\\";
            ClearFolders(_oldFolderLocation, _newFolderLocation, _patchFolderLocation);
            const string textInFile = "This is some text";
            AddFileToFolderWithText(Path.Combine(_newFolderLocation, childFolder), "addFile.txt", textInFile);
            AddFileToFolderWithText(Path.Combine(_oldFolderLocation, childFolder), "addFile.txt", textInFile);

            KaBlooey.KaBlooeyEngine.CreatePatch(_oldFolderLocation, _newFolderLocation, _patchFolderLocation);
            var fileLocation = Path.Combine(Path.Combine(_patchFolderLocation, childFolder), "addFile.txt.changed");
            var result = File.Exists(fileLocation);
            Assert.AreEqual(true, result);

            var fileText = File.ReadAllText(fileLocation);
            Assert.AreEqual(128, fileText.Length);
            KaBlooey.KaBlooeyEngine.ApplyPatch(_patchFolderLocation, _oldFolderLocation);
            var oldfileLocation = Path.Combine(_oldFolderLocation, childFolder, "addFile.txt");
            var oldFileresult = File.Exists(oldfileLocation);
            Assert.AreEqual(true, oldFileresult);

            var oldFileText = File.ReadAllText(oldfileLocation);
            Assert.AreEqual(textInFile, oldFileText);
        }
        [Test]
        public void WhenIChangeAFileInTheNewDirectoryAndProduceADiff_ItShouldBeAppliedToTheOldFile()
        {
            ClearFolders(_oldFolderLocation, _newFolderLocation, _patchFolderLocation);
            const string textInFile = "This is some text";
            AddFileToFolderWithText(_newFolderLocation, "addFile.txt", textInFile);
            AddFileToFolderWithText(_oldFolderLocation, "addFile.txt", textInFile);

            KaBlooey.KaBlooeyEngine.CreatePatch(_oldFolderLocation, _newFolderLocation, _patchFolderLocation);
            var fileLocation = Path.Combine(_patchFolderLocation, "addFile.txt.changed");
            var result = File.Exists(fileLocation);
            Assert.AreEqual(true, result);

            var fileText = File.ReadAllText(fileLocation);
            Assert.AreEqual(128, fileText.Length);

            KaBlooey.KaBlooeyEngine.ApplyPatch(_patchFolderLocation, _oldFolderLocation);
            var oldfileLocation = Path.Combine(_oldFolderLocation, "addFile.txt");
            var oldFileresult = File.Exists(oldfileLocation);
            Assert.AreEqual(true, oldFileresult);

            var oldFileText = File.ReadAllText(oldfileLocation);
            Assert.AreEqual(textInFile, oldFileText);

        }

        private void AddFileToFolderWithText(string folderLocation, string addfileTxt, string text)
        {
            if (!Directory.Exists(folderLocation))
            {
                Directory.CreateDirectory(folderLocation);
            }
            var filePath = Path.Combine(folderLocation, addfileTxt);
            using (var stream = File.Create(filePath))
            {
                using(var streamWriter = new StreamWriter(stream))
                {
                    streamWriter.Write(text);
                }
            }
            Assert.IsTrue(File.Exists(filePath));
        }
    }
}