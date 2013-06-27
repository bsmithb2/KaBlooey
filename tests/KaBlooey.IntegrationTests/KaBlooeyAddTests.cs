﻿using System;
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
    public class KaBlooeyAddTests : KaBlooeyIntegrationTests
    {
        [Test]
        public void WhenIAddAFile_FileShouldBeAddedToPatchFolderWithAddExtension()
        {
            ClearFolders(_oldFolderLocation, _newFolderLocation, _patchFolderLocation);
            AddFileToFolder(_newFolderLocation, "addFile.txt");
            Directory.CreateDirectory(_oldFolderLocation);
            KaBlooey.KaBlooeyEngine.CreatePatch(_oldFolderLocation, _newFolderLocation, _patchFolderLocation);
            var result = File.Exists(Path.Combine(_patchFolderLocation, "addFile.txt.add"));
            Assert.AreEqual(true, result);
        }

        [Test]
        public void WhenIAddAFileToAChildFolder_FileShouldBeAddedToPatchChildFolderWithAddExtension()
        {
            string childFolder = "child\\";
            
            ClearFolders(_oldFolderLocation, _newFolderLocation, _patchFolderLocation);
            AddFileToFolder(Path.Combine(_newFolderLocation, childFolder), "addFile.txt");
            Directory.CreateDirectory(_oldFolderLocation);
            KaBlooey.KaBlooeyEngine.CreatePatch(_oldFolderLocation, _newFolderLocation, _patchFolderLocation);
            var result = File.Exists(Path.Combine(_patchFolderLocation, Path.Combine(childFolder, "addFile.txt.add")));
            Assert.AreEqual(true, result);
        }

        [Test]
        public void WhenIHaveAnAddFileInPatchFolder_ApplyAddFileIntoExistingFolder()
        {
            ClearFolders(_oldFolderLocation, _newFolderLocation, _patchFolderLocation);
            AddFileToFolder(_patchFolderLocation, "addFile.txt.add");
            Directory.CreateDirectory(_newFolderLocation);

            KaBlooey.KaBlooeyEngine.ApplyPatch(_patchFolderLocation, _newFolderLocation);

            var result = File.Exists(Path.Combine(_newFolderLocation, "addFile.txt"));
            Assert.AreEqual(true, result);
        }

        [Test]
        public void WhenIHaveAnAddFileInAChildPatchFolder_ApplyAddFileIntoNewChildFolder()
        {
            string childFolder = "child\\";

            ClearFolders(_oldFolderLocation, _newFolderLocation, _patchFolderLocation);
            AddFileToFolder(Path.Combine(_patchFolderLocation, childFolder), "addFile.txt.add");
            Directory.CreateDirectory(_newFolderLocation);

            KaBlooey.KaBlooeyEngine.ApplyPatch(_patchFolderLocation, _newFolderLocation);

            var result = File.Exists(Path.Combine(_newFolderLocation, childFolder, "addFile.txt"));
            Assert.AreEqual(true, result);
        }
    }

    public class KaBlooeyIntegrationTests
    {
        protected readonly string _rootPath = "c:\\patch";
        protected readonly string _oldFolderLocation;
        protected string _newFolderLocation;
        protected string _patchFolderLocation;

        public KaBlooeyIntegrationTests()
        {
            _oldFolderLocation = _rootPath + "\\old";
            _newFolderLocation = _rootPath + "\\new";
            _patchFolderLocation = _rootPath + "\\patch";
        }

        protected void AddFileToFolder(string newFolder, string addfileTxt)
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

        protected void ClearFolders(string oldFolder, string newFolder, string patchFolder)
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
