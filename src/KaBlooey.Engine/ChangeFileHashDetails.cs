using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Xml.Serialization;

namespace KaBlooey.Engine
{
    [Serializable]
    public class ChangeFileHashDetails
    {
        private readonly string _patchFilePath;
        public string _patchHash;
        private readonly string _oldFilePath;
        public string _oldFileHash;
        private readonly string _newFilePath;
        public string _newFileHash;
        public string _relativeNewFileLocation;
        public string _relativeOldFileLocation;
        public string _relativePatchFileLocation;
        private readonly FileComputeHasher _fileComputeHasher;

        public ChangeFileHashDetails()
        {
            _fileComputeHasher = new FileComputeHasher();
        }

        public ChangeFileHashDetails(string patchFilePath, string oldFilePath, string newFilePath, string rootNewFolderLocation, string rootOldFolderlocation, string rootPatchFolderLocation)
        {
            _fileComputeHasher = new FileComputeHasher();
            _patchFilePath = patchFilePath;
            _oldFilePath = oldFilePath;
            _newFilePath = newFilePath;
            _relativeNewFileLocation = newFilePath.Replace(rootNewFolderLocation, "");
            _relativeOldFileLocation = oldFilePath.Replace(rootOldFolderlocation, "");
            _relativePatchFileLocation = patchFilePath.Replace(rootPatchFolderLocation, "");
            ComputeHashes();
        }

        private void ComputeHashes()
        {
            _patchHash = FileComputeHasher.ComputeHashFromFile(_patchFilePath);
            _oldFileHash = FileComputeHasher.ComputeHashFromFile(_oldFilePath);
            _newFileHash = FileComputeHasher.ComputeHashFromFile(_newFilePath);
        }

        public static void SerializeListToFile(List<ChangeFileHashDetails> list, string fileName)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<ChangeFileHashDetails>));
            using (var stream = new FileStream(fileName, FileMode.Create))
            {
                serializer.Serialize(stream, list);
            }
            File.Copy(fileName, "d:\\filestore.xml", true);
        }

        public static List<ChangeFileHashDetails> DeserializeListFromFile(string hashFileLocation)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<ChangeFileHashDetails>));
            using (var stream = new FileStream(hashFileLocation, FileMode.Open))
            {
                return (List<ChangeFileHashDetails>) serializer.Deserialize(stream);
            }
        }
    }
}