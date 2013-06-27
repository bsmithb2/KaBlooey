using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using KaBlooey;

namespace KaBlooey.IntegrationTests
{
    [TestFixture]
    public class KaBlooeyAddTests
    {
        [Test]
        public void WhenIAddAFile_FileShouldBeAddedToPatchFolderWithAddExtension()
        {
            string rootPath = "c:\\patch";
            string oldFolder = rootPath + "\\old";
            string newFolder = rootPath + "\\new";
            string patchFolder = rootPath + "\\patch";

            ClearFolders(oldFolder, newFolder, patchFolder);
            AddFileToFolder(newFolder, "addFile.txt");

            KaBlooey.KaBlooeyEngine.CreatePatch(oldFolder, newFolder, patchFolder);
            var result = File.Exists(Path.Combine(patchFolder, "addFile.txt.add"));
            Assert.AreEqual(true, result);
        }

        [Test]
        public void WhenIAddAFileToAChildFolder_FileShouldBeAddedToPatchChildFolderWithAddExtension()
        {
            string rootPath = "c:\\patch";
            string childFolder = "child\\";
            string oldFolder = rootPath + "\\old";
            string newFolder = rootPath + "\\new";
            string patchFolder = rootPath + "\\patch";

            ClearFolders(oldFolder, newFolder, patchFolder);
            AddFileToFolder(Path.Combine(newFolder, childFolder), "addFile.txt");

            KaBlooey.KaBlooeyEngine.CreatePatch(oldFolder, newFolder, patchFolder);
            var result = File.Exists(Path.Combine(patchFolder, Path.Combine(childFolder, "addFile.txt.add")));
            Assert.AreEqual(true, result);
        }

        [Test]
        public void WhenIHaveAnAddFileInPatchFolder_ApplyAddFileIntoExistingFolder()
        {
            string rootPath = "c:\\patch";
            string oldFolder = rootPath + "\\old";
            string newFolder = rootPath + "\\new";
            string patchFolder = rootPath + "\\patch";

            ClearFolders(oldFolder, newFolder, patchFolder);
            AddFileToFolder(patchFolder, "addFile.txt.add");
            Directory.CreateDirectory(newFolder);

            KaBlooey.KaBlooeyEngine.ApplyPatch(patchFolder, newFolder);

            var result = File.Exists(Path.Combine(newFolder, "addFile.txt"));
            Assert.AreEqual(true, result);
        }

        [Test]
        public void WhenIHaveAnAddFileInAChildPatchFolder_ApplyAddFileIntoNewChildFolder()
        {
            string rootPath = "c:\\patch";
            string oldFolder = rootPath + "\\old";
            string newFolder = rootPath + "\\new";
            string patchFolder = rootPath + "\\patch";
            string childFolder = "child\\";

            ClearFolders(oldFolder, newFolder, patchFolder);
            AddFileToFolder(Path.Combine(patchFolder, childFolder), "addFile.txt.add");
            Directory.CreateDirectory(newFolder);

            KaBlooey.KaBlooeyEngine.ApplyPatch(patchFolder, newFolder);

            var result = File.Exists(Path.Combine(newFolder, childFolder, "addFile.txt"));
            Assert.AreEqual(true, result);
        }

        private void AddFileToFolder(string newFolder, string addfileTxt)
        {
            if(!Directory.Exists(newFolder))
            {
                Directory.CreateDirectory(newFolder);
            }
            var filePath = Path.Combine(newFolder, addfileTxt);
            using(var stream = File.Create(filePath))
            {
                
            }
            Assert.IsTrue(File.Exists(filePath));
        }

        private void ClearFolders(string oldFolder, string newFolder, string patchFolder)
        {
            if (Directory.Exists(oldFolder))
            {
                Directory.Delete(oldFolder, true);
            }
            if (Directory.Exists(newFolder))
            {
                Directory.Delete(newFolder, true);
            }
            if(Directory.Exists(patchFolder))
            {
                Directory.Delete(patchFolder, true);    
            }
        }
    }
}
