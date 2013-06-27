using System.IO;
using NUnit.Framework;

namespace KaBlooey.IntegrationTests
{
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