using System;
using System.IO;
using System.Linq;
using System.Xml;
using KaBlooey.Engine;
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
            AddFileToFolderWithText(_oldFolderLocation, "addFile.txt", "this is some old text");

            KaBlooeyEngine.CreatePatch(_oldFolderLocation, _newFolderLocation, _patchFolderLocation);
            var fileLocation = Path.Combine(_patchFolderLocation, "addFile.txt.changed");
            var result = File.Exists(fileLocation);
            Assert.AreEqual(true, result);

            var fileText = File.ReadAllText(fileLocation);
            Assert.AreEqual(152, fileText.Length);
        }

        [Test]
        public void WhenIChangeAFileInTheChildNewDirectory_ItShouldBeDeltaDiffed()
        {
            const string childFolder = "childChanged\\";
            ClearFolders(_oldFolderLocation, _newFolderLocation, _patchFolderLocation);
            const string textInFile = "This is some text";
            AddFileToFolderWithText(Path.Combine(_newFolderLocation, childFolder), "addFile.txt", textInFile);
            AddFileToFolderWithText(Path.Combine(_oldFolderLocation, childFolder), "addFile.txt", "oldText");

            KaBlooeyEngine.CreatePatch(_oldFolderLocation, _newFolderLocation, _patchFolderLocation);
            var fileLocation = Path.Combine(Path.Combine(_patchFolderLocation, childFolder), "addFile.txt.changed");
            var result = File.Exists(fileLocation);
            Assert.AreEqual(true, result);

            var fileText = File.ReadAllText(fileLocation);
            Assert.AreEqual(139, fileText.Length);
        }
        
        [Test]
        public void WhenIChangeAFileInTheNewChildDirectoryAndProduceADiff_ItShouldBeAppliedToTheOldFileInTheChildDirectory()
        {
            const string childFolder = "childChanged\\";
            ClearFolders(_oldFolderLocation, _newFolderLocation, _patchFolderLocation);
            const string textInFile = "This is some text";
            AddFileToFolderWithText(Path.Combine(_newFolderLocation, childFolder), "addFile.txt", textInFile);
            AddFileToFolderWithText(Path.Combine(_oldFolderLocation, childFolder), "addFile.txt", "old text");

            KaBlooeyEngine.CreatePatch(_oldFolderLocation, _newFolderLocation, _patchFolderLocation);
            var fileLocation = Path.Combine(Path.Combine(_patchFolderLocation, childFolder), "addFile.txt.changed");
            var result = File.Exists(fileLocation);
            Assert.AreEqual(true, result);

            var fileText = File.ReadAllText(fileLocation);
            Assert.AreEqual(139, fileText.Length);
            KaBlooeyEngine.ApplyPatch(_patchFolderLocation, _oldFolderLocation);
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

            KaBlooeyEngine.CreatePatch(_oldFolderLocation, _newFolderLocation, _patchFolderLocation);
            var fileLocation = Path.Combine(_patchFolderLocation, "addFile.txt.changed");
            var result = File.Exists(fileLocation);
            Assert.AreEqual(true, result);

            var fileText = File.ReadAllText(fileLocation);
            Assert.AreEqual(128, fileText.Length);

            KaBlooeyEngine.ApplyPatch(_patchFolderLocation, _oldFolderLocation);
            var oldfileLocation = Path.Combine(_oldFolderLocation, "addFile.txt");
            var oldFileresult = File.Exists(oldfileLocation);
            Assert.AreEqual(true, oldFileresult);

            var oldFileText = File.ReadAllText(oldfileLocation);
            Assert.AreEqual(textInFile, oldFileText);

        }

        [Test]
        [Category("broken")]
        [Ignore]
        //[ExpectedException(typeof(InvalidOperationException))]
        public void WhenIChangeAFileInTheNewDirectoryAndProduceADiffAndHackTheMD5_ThenTheDeltasShouldNotBeApplied_UnSerialisable()
        {
            #region HasFileText

            const string hashFileText = @"klfjafdjkal;j";
            
            #endregion
            ClearFolders(_oldFolderLocation, _newFolderLocation, _patchFolderLocation);
            const string textInFile = "This is some text";
            AddFileToFolderWithText(_newFolderLocation, "addFile.txt", textInFile);
            AddFileToFolderWithText(_oldFolderLocation, "addFile.txt", textInFile);

            KaBlooeyEngine.CreatePatch(_oldFolderLocation, _newFolderLocation, _patchFolderLocation);
            var fileLocation = Path.Combine(_patchFolderLocation, "addFile.txt.changed");
            var result = File.Exists(fileLocation);
            Assert.AreEqual(true, result);

            var fileText = File.ReadAllText(fileLocation);
            Assert.AreEqual(128, fileText.Length);
            
            var hashFileLocation = _patchFolderLocation + "\\_changedFileList.hashstore";
            Assert.IsTrue(File.Exists(hashFileLocation), hashFileLocation);
            File.Delete(hashFileLocation);
            File.WriteAllText(hashFileLocation, hashFileText);

            KaBlooeyEngine.ApplyPatch(_patchFolderLocation, _oldFolderLocation);
        }

        [Test]
        public void WhenIChangeAFileInTheNewDirectoryAndProduceADiffAndHackTheMD5_ThenTheDeltasShouldNotBeApplied_IncorrectHashValue()
        {
            #region HasFileText

            const string hashFileText = @"<?xml version=""1.0""?>
<ArrayOfChangeFileHashDetails xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">
  <ChangeFileHashDetails>
    <_patchHash>91-A1-A8-46-38-8E-E0-14-EE-9E-1D-FB-3C-3A-58-57 wrong hash</_patchHash>
    <_oldFileHash>D4-1D-8C-D9-8F-00-B2-04-E9-80-09-98-EC-F8-42-7E</_oldFileHash>
    <_newFileHash>D4-1D-8C-D9-8F-00-B2-04-E9-80-09-98-EC-F8-42-7E</_newFileHash>
    <_relativeNewFileLocation>\patchsubfolder\change.txt</_relativeNewFileLocation>
    <_relativeOldFileLocation>\patchsubfolder\change.txt</_relativeOldFileLocation>
    <_relativePatchFileLocation>\patchsubfolder\change.txt.changed</_relativePatchFileLocation>
  </ChangeFileHashDetails>
  <ChangeFileHashDetails>
    <_patchHash>91-A1-A8-46-38-8E-E0-14-EE-9E-1D-FB-3C-3A-58-57</_patchHash>
    <_oldFileHash>D4-1D-8C-D9-8F-00-B2-04-E9-80-09-98-EC-F8-42-7E</_oldFileHash>
    <_newFileHash>D4-1D-8C-D9-8F-00-B2-04-E9-80-09-98-EC-F8-42-7E</_newFileHash>
    <_relativeNewFileLocation>\change.txt</_relativeNewFileLocation>
    <_relativeOldFileLocation>\change.txt</_relativeOldFileLocation>
    <_relativePatchFileLocation>\change.txt.changed</_relativePatchFileLocation>
  </ChangeFileHashDetails>
</ArrayOfChangeFileHashDetails>";
            #endregion
            ClearFolders(_oldFolderLocation, _newFolderLocation, _patchFolderLocation);
            const string textInFile = "This is some text";
            AddFileToFolderWithText(_newFolderLocation, "addFile.txt", textInFile);
            AddFileToFolderWithText(_oldFolderLocation, "addFile.txt", textInFile);

            KaBlooeyEngine.CreatePatch(_oldFolderLocation, _newFolderLocation, _patchFolderLocation);
            var fileLocation = Path.Combine(_patchFolderLocation, "addFile.txt.changed");
            var result = File.Exists(fileLocation);
            Assert.AreEqual(true, result);

            var fileText = File.ReadAllText(fileLocation);
            Assert.AreEqual(128, fileText.Length);

            var hashFileLocation = _patchFolderLocation + "\\_changedFileList.hashstore";
            Assert.IsTrue(File.Exists(hashFileLocation), hashFileLocation);
            File.Delete(hashFileLocation);
            File.WriteAllText(hashFileLocation, hashFileText);

            KaBlooeyEngine.ApplyPatch(_patchFolderLocation, _oldFolderLocation);
        }

        [Test]
        public void WhenIChangeAFileTheFile_ItShouldProduceAValidMD5Hash()
        {
            //Do a standard diff on one file.
            WhenIChangeAFileInTheNewDirectory_ItShouldBeDeltaDiffed();
            var hashSummaryList = KaBlooeyEngine.GetHashSummary();
            Assert.AreEqual(1, hashSummaryList.Count);
            Assert.AreEqual("97-21-4F-63-22-4B-C1-E9-CC-4D-A3-77-AA-DC-E7-C7", hashSummaryList.First()._newFileHash);
            Assert.AreEqual("4C-29-5D-41-71-76-DE-BC-14-34-78-9A-5B-25-DE-27", hashSummaryList.First()._oldFileHash);
            Assert.AreEqual("F9-35-8A-2E-8E-FD-0A-C7-5F-23-40-66-6D-BA-01-EE", hashSummaryList.First()._patchHash);

            var hashFileLocation = _patchFolderLocation + "\\_changedFileList.hashstore";
            Assert.IsTrue(File.Exists(hashFileLocation), hashFileLocation);

            var list = ChangeFileHashDetails.DeserializeListFromFile(hashFileLocation);
            Assert.AreEqual(1, list.Count());
            Assert.AreEqual("97-21-4F-63-22-4B-C1-E9-CC-4D-A3-77-AA-DC-E7-C7", list.First()._newFileHash);
            Assert.AreEqual("4C-29-5D-41-71-76-DE-BC-14-34-78-9A-5B-25-DE-27", list.First()._oldFileHash);
            Assert.AreEqual("F9-35-8A-2E-8E-FD-0A-C7-5F-23-40-66-6D-BA-01-EE", list.First()._patchHash);
            Assert.AreEqual("\\addFile.txt", list.First()._relativeNewFileLocation);
            Assert.AreEqual("\\addFile.txt", list.First()._relativeOldFileLocation);
            Assert.AreEqual("\\addFile.txt.changed", list.First()._relativePatchFileLocation);
        }

        [Test]

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