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

        public ChangeFileHashDetails()
        {
        }

        public ChangeFileHashDetails(string patchFilePath, string oldFilePath, string newFilePath, string rootNewFolderLocation, string rootOldFolderlocation, string rootPatchFolderLocation)
        {
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
            _patchHash = ComputeHashFromFile(_patchFilePath);
            _oldFileHash = ComputeHashFromFile(_oldFilePath);
            _newFileHash = ComputeHashFromFile(_newFilePath);
        }

        private string ComputeHashFromFile(string fileName)
        {
            byte[] hash = MD5.Create().ComputeHash(File.ReadAllBytes(fileName));
            return BitConverter.ToString(hash);
        }

        public static void SerializeListToFile(List<ChangeFileHashDetails> list, string fileName)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<ChangeFileHashDetails>));
            using (var stream = new FileStream(fileName, FileMode.Create))
            {
                serializer.Serialize(stream, list);
            }
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