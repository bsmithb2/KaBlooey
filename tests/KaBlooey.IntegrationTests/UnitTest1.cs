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
            string rootPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase);
            rootPath = rootPath.Substring(6, rootPath.Length - 6);
            string oldFolder = rootPath + "\\old";
            string newFolder = rootPath + "\\new";
            string patchFolder = rootPath + "\\patch";

            ClearFolders(oldFolder, newFolder, patchFolder);

            AddFileToNewFolder(newFolder, "addFile.txt");

            KaBlooey.KaBlooeyEngine.CreatePatch(oldFolder, newFolder, patchFolder);
            var result = File.Exists(Path.Combine(patchFolder, "addFile.txt.add"));
            Assert.AreEqual(true, result);
        }

        private void AddFileToNewFolder(string newFolder, string addfileTxt)
        {
            if(!Directory.Exists(newFolder))
            {
                Directory.CreateDirectory(newFolder);
            }
            var filePath = Path.Combine(newFolder, addfileTxt);
            File.Create(filePath);
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
